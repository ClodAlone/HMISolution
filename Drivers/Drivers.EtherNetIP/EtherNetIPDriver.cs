using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Opc.Ua;
using libplctag;
using Serilog;

namespace SimpleOpcFileServer
{
    public class EtherNetIPConfig
    {
        public string IpAddress { get; set; } = "";
        public string Path { get; set; } = "";
        public string Tag { get; set; } = "";
        public string Type { get; set; } = "";
        public int PollTime { get; set; } = 1000;
    }

    public class EtherNetIPDriver : IDriver, IDisposable
    {
        public string Key => "EtherNetIP";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly Dictionary<string, EtherNetIPDevice> _devices = new();
        private readonly object _lock = new();
        private bool _disposed;
        private readonly ISystemContext _context;

        public EtherNetIPDriver(ISystemContext context) { _context = context; }

        private void RaiseError(string source, string message) => OnError?.Invoke(source, message);
        private void RaiseCycle(double ms) => OnCycleCompleted?.Invoke(ms);

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var eipConfig = JsonSerializer.Deserialize<EtherNetIPConfig>(configJson);
            if (eipConfig == null) return;

            lock (_lock)
            {
                var key = eipConfig.IpAddress;
                if (!_devices.TryGetValue(key, out var device))
                {
                    device = new EtherNetIPDevice(key, _context, RaiseError, RaiseCycle);
                    _devices[key] = device;
                }
                device.AddItem(new EipItem { Variable = variable, Config = eipConfig });
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

        private class EtherNetIPDevice : IDisposable
        {
            private readonly string _ip;
            private readonly ISystemContext _context;
            private readonly Action<string, string>? _onError;
            private readonly Action<double>? _onCycle;
            private readonly List<EipItem> _items = new();
            private Timer? _timer;
            private int _pollInterval = 1000;
            private int _minPollTime = int.MaxValue;
            private bool _disposed;
            private readonly object _deviceLock = new();
            private string? _lastLoggedError;
            private int _consecutiveErrors;
            private const int MaxBackoffMs = 30_000;

            public EtherNetIPDevice(string ip, ISystemContext context, Action<string, string>? onError = null, Action<double>? onCycle = null) { _ip = ip; _context = context; _onError = onError; _onCycle = onCycle; }

            public void AddItem(EipItem item)
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
                bool anyError = false;
                lock (_deviceLock)
                {
                    if (_disposed) return;
                    foreach (var item in _items)
                    {
                        try
                        {
                            if (item.LibTag == null)
                            {
                                int elemSize = item.Config.Type switch
                                {
                                    "BOOL" or "SINT" => 1,
                                    "INT" => 2,
                                    _ => 4
                                };
                                item.LibTag = new Tag
                                {
                                    Gateway = _ip, Path = item.Config.Path, Name = item.Config.Tag,
                                    PlcType = PlcType.ControlLogix, Protocol = Protocol.ab_eip,
                                    ElementSize = elemSize, ElementCount = 1, Timeout = TimeSpan.FromSeconds(2)
                                };
                            }
                            item.LibTag.Read();
                            object? val = item.Config.Type switch
                            {
                                "DINT" => item.LibTag.GetInt32(0),
                                "REAL" => item.LibTag.GetFloat32(0),
                                "BOOL" => item.LibTag.GetUInt8(0) > 0,
                                "INT" => item.LibTag.GetInt16(0),
                                "SINT" => item.LibTag.GetInt8(0),
                                _ => null
                            };
                            if (val != null) Update(item.Variable, val);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message != _lastLoggedError)
                            {
                                Log.Error(ex, "EtherNetIP Error: {Message}", ex.Message);
                                _lastLoggedError = ex.Message;
                            }
                            else Log.Debug("EtherNetIP Error (repeated): {Message}", ex.Message);
                            _onError?.Invoke(_ip, $"Error: {ex.Message}");
                            UpdateError(item.Variable, ex.Message);
                            try { item.LibTag?.Dispose(); } catch { }
                            item.LibTag = null;
                            anyError = true;
                        }
                    }
                }
                if (anyError) _consecutiveErrors++; else { _consecutiveErrors = 0; _lastLoggedError = null; }
                var delay = _consecutiveErrors > 0 ? Math.Min(_pollInterval * (1 << Math.Min(_consecutiveErrors, 10)), MaxBackoffMs) : _pollInterval;
                _onCycle?.Invoke(_sw.Elapsed.TotalMilliseconds);
                if (!_disposed) _timer?.Change(delay, Timeout.Infinite);
            }

            private void Update(BaseDataVariableState variable, object value)
            {
                variable.Value = value; variable.StatusCode = StatusCodes.Good;
                variable.Timestamp = DateTime.UtcNow; variable.ClearChangeMasks(_context, false);
            }

            private void UpdateError(BaseDataVariableState variable, string? message = null)
            {
                variable.StatusCode = StatusCodes.Bad; variable.Timestamp = DateTime.UtcNow;
                variable.ClearChangeMasks(_context, false);
                if (!string.IsNullOrEmpty(message))
                {
                    var n = variable.FindChild(_context, new QualifiedName("LastError", variable.BrowseName.NamespaceIndex));
                    if (n is BaseVariableState v) { v.Value = message; v.Timestamp = DateTime.UtcNow; v.ClearChangeMasks(_context, false); }
                }
            }

            public void Dispose()
            {
                _disposed = true; _timer?.Dispose();
                lock (_deviceLock) { foreach (var item in _items) item.LibTag?.Dispose(); _items.Clear(); }
            }
        }

        private class EipItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public EtherNetIPConfig Config { get; set; } = null!;
            public Tag? LibTag { get; set; }
        }
    }
}
