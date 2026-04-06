using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Opc.Ua;
using Serilog;

namespace SimpleOpcFileServer
{
    public class CsvConfig
    {
        public string FilePath { get; set; } = "";
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public string? Key { get; set; }
        public int PollTime { get; set; } = 1000;
    }

    public class CsvDriver : IDriver, IDisposable
    {
        public string Key => "Csv";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly Dictionary<string, CsvDevice> _devices = new();
        private readonly object _lock = new();
        private bool _disposed;
        private readonly ISystemContext _context;

        public CsvDriver(ISystemContext context)
        {
            _context = context;
        }

        private void RaiseError(string source, string message) => OnError?.Invoke(source, message);
        private void RaiseCycle(double ms) => OnCycleCompleted?.Invoke(ms);

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var csvConfig = JsonSerializer.Deserialize<CsvConfig>(configJson);
            if (csvConfig == null) return;

            lock (_lock)
            {
                var key = csvConfig.FilePath;
                if (!_devices.TryGetValue(key, out var device))
                {
                    device = new CsvDevice(key, _context, RaiseError, RaiseCycle);
                    _devices[key] = device;
                }
                device.AddItem(new CsvItem { Variable = variable, Config = csvConfig });
            }
        }

        public void Start()
        {
            lock (_lock) { foreach (var d in _devices.Values) d.Start(); }
        }

        public void Dispose()
        {
            _disposed = true;
            foreach (var d in _devices.Values) d.Dispose();
            _devices.Clear();
        }

        private class CsvDevice : IDisposable
        {
            private readonly string _filePath;
            private readonly ISystemContext _context;
            private readonly Action<string, string>? _onError;
            private readonly Action<double>? _onCycle;
            private readonly List<CsvItem> _items = new();
            private Timer? _timer;
            private int _pollInterval = 1000;
            private int _minPollTime = int.MaxValue;
            private bool _disposed;
            private readonly object _deviceLock = new();

            public CsvDevice(string filePath, ISystemContext context, Action<string, string>? onError = null, Action<double>? onCycle = null)
            {
                _filePath = filePath;
                _context = context;
                _onError = onError;
                _onCycle = onCycle;
            }

            public void AddItem(CsvItem item)
            {
                lock (_deviceLock)
                {
                    _items.Add(item);
                    if (item.Config.PollTime > 0 && item.Config.PollTime < _minPollTime)
                        _minPollTime = item.Config.PollTime;
                    _pollInterval = _minPollTime < int.MaxValue ? _minPollTime : 1000;
                }
            }

            public void Start()
            {
                lock (_deviceLock)
                {
                    if (_timer == null && _items.Count > 0)
                        _timer = new Timer(Poll, null, _pollInterval, Timeout.Infinite);
                }
            }


            private void Poll(object? state)
            {
                if (_disposed) return;
                var _sw = System.Diagnostics.Stopwatch.StartNew();

                List<CsvItem> itemsToPoll;
                lock (_deviceLock)
                {
                    if (_disposed) return;
                    itemsToPoll = new List<CsvItem>(_items);
                }

                try
                {
                    if (File.Exists(_filePath))
                    {
                        using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using var sr = new StreamReader(fs);
                        var lines = new List<string[]>();
                        string? line;
                        while ((line = sr.ReadLine()) != null)
                            lines.Add(line.Split(new[] { ',', ';' }));

                        foreach (var item in itemsToPoll)
                        {
                            try
                            {
                                string valStr = "";
                                if (!string.IsNullOrEmpty(item.Config.Key))
                                {
                                    var foundRow = lines.FirstOrDefault(r => r.Length > 0 && r[0] == item.Config.Key);
                                    if (foundRow != null && foundRow.Length > item.Config.ColumnIndex)
                                        valStr = foundRow[item.Config.ColumnIndex];
                                }
                                else
                                {
                                    if (lines.Count > item.Config.RowIndex && lines[item.Config.RowIndex].Length > item.Config.ColumnIndex)
                                        valStr = lines[item.Config.RowIndex][item.Config.ColumnIndex];
                                }

                                if (!string.IsNullOrEmpty(valStr))
                                    Update(item.Variable, valStr);
                                else
                                    UpdateError(item.Variable, "Value not found");
                            }
                            catch (Exception ex) { UpdateError(item.Variable, ex.Message); }
                        }
                    }
                    else
                    {
                        foreach (var item in itemsToPoll) UpdateError(item.Variable, "File not found");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "CSV read error for {File}: {Message}", _filePath, ex.Message);
                    _onError?.Invoke(_filePath, $"Read error: {ex.Message}");
                    foreach (var item in itemsToPoll) UpdateError(item.Variable, ex.Message);
                }
                finally
                {
                    _onCycle?.Invoke(_sw.Elapsed.TotalMilliseconds);
                    if (!_disposed) _timer?.Change(_pollInterval, Timeout.Infinite);
                }
            }

            private void Update(BaseDataVariableState variable, string valStr)
            {
                object? value = null;
                if (variable.DataType == DataTypeIds.Double) value = double.TryParse(valStr, out var d) ? d : 0.0;
                else if (variable.DataType == DataTypeIds.Int32) value = int.TryParse(valStr, out var i) ? i : 0;
                else if (variable.DataType == DataTypeIds.Boolean) value = bool.TryParse(valStr, out var b) ? b : false;
                else value = valStr;

                if (value != null)
                {
                    variable.Value = value;
                    variable.StatusCode = StatusCodes.Good;
                    variable.Timestamp = DateTime.UtcNow;
                    variable.ClearChangeMasks(_context, false);
                }
            }

            private void UpdateError(BaseDataVariableState variable, string? message = null)
            {
                variable.StatusCode = StatusCodes.Bad;
                variable.Timestamp = DateTime.UtcNow;
                variable.ClearChangeMasks(_context, false);
                if (!string.IsNullOrEmpty(message))
                {
                    var lastErrorNode = variable.FindChild(_context, new QualifiedName("LastError", variable.BrowseName.NamespaceIndex));
                    if (lastErrorNode is BaseVariableState lastErrorVar)
                    {
                        lastErrorVar.Value = message;
                        lastErrorVar.Timestamp = DateTime.UtcNow;
                        lastErrorVar.ClearChangeMasks(_context, false);
                    }
                }
            }

            public void Dispose()
            {
                _disposed = true;
                _timer?.Dispose();
            }
        }

        private class CsvItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public CsvConfig Config { get; set; } = null!;
        }
    }
}
