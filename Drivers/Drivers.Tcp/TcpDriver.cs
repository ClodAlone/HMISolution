using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Opc.Ua;
using Serilog;

namespace SimpleOpcFileServer
{
    public class TcpConfig
    {
        public string IpAddress { get; set; } = "";
        public int Port { get; set; }
        public string Command { get; set; } = "";
        public string? Regex { get; set; }
        public int PollTime { get; set; } = 1000;
    }

    public class TcpDriver : IDriver, IDisposable
    {
        public string Key => "Tcp";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly Dictionary<string, TcpDevice> _devices = new();
        private readonly object _lock = new();
        private bool _disposed;
        private readonly ISystemContext _context;

        public TcpDriver(ISystemContext context) { _context = context; }

        private void RaiseError(string source, string message) => OnError?.Invoke(source, message);
        private void RaiseCycle(double ms) => OnCycleCompleted?.Invoke(ms);

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var tcpConfig = JsonSerializer.Deserialize<TcpConfig>(configJson);
            if (tcpConfig == null) return;

            lock (_lock)
            {
                var key = $"{tcpConfig.IpAddress}:{tcpConfig.Port}";
                if (!_devices.TryGetValue(key, out var device))
                {
                    device = new TcpDevice(key, tcpConfig.IpAddress, tcpConfig.Port, _context, RaiseError, RaiseCycle);
                    _devices[key] = device;
                }
                device.AddItem(new TcpItem { Variable = variable, Config = tcpConfig });
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

        private class TcpDevice : IDisposable
        {
            private readonly string _key;
            private readonly string _ip;
            private readonly int _port;
            private readonly ISystemContext _context;
            private readonly Action<string, string>? _onError;
            private readonly Action<double>? _onCycle;
            private readonly List<TcpItem> _items = new();
            private Timer? _timer;
            private int _pollInterval = 1000;
            private int _minPollTime = int.MaxValue;
            private bool _disposed;
            private readonly object _deviceLock = new();

            public TcpDevice(string key, string ip, int port, ISystemContext context, Action<string, string>? onError = null, Action<double>? onCycle = null)
            { _key = key; _ip = ip; _port = port; _context = context; _onError = onError; _onCycle = onCycle; }

            public void AddItem(TcpItem item)
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
                List<TcpItem> itemsToPoll;
                lock (_deviceLock) { if (_disposed) return; itemsToPoll = new List<TcpItem>(_items); }

                try
                {
                    foreach (var item in itemsToPoll)
                    {
                        try
                        {
                            using var client = new TcpClient();
                            if (client.ConnectAsync(_ip, _port).Wait(1000))
                            {
                                using var stream = client.GetStream();
                                var cmdStr = item.Config.Command.Replace("\\r", "\r").Replace("\\n", "\n");
                                var data = Encoding.ASCII.GetBytes(cmdStr);
                                stream.Write(data, 0, data.Length);
                                stream.ReadTimeout = 1000;
                                var buffer = new byte[1024];
                                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                                if (bytesRead > 0)
                                {
                                    var response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                                    string valStr = response;
                                    if (!string.IsNullOrEmpty(item.Config.Regex))
                                    {
                                        var match = System.Text.RegularExpressions.Regex.Match(response, item.Config.Regex);
                                        if (match.Success) valStr = match.Groups.Count > 1 ? match.Groups[1].Value : match.Value;
                                    }
                                    Update(item.Variable, valStr);
                                }
                                else UpdateError(item.Variable, "No response");
                            }
                            else UpdateError(item.Variable, "Connection timeout");
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "TCP poll error for {Name}: {Message}", item.Variable.DisplayName, ex.Message);
                            _onError?.Invoke(_key, $"Poll error ({item.Variable.DisplayName}): {ex.Message}");
                            UpdateError(item.Variable, ex.Message);
                        }
                    }
                }
                finally { _onCycle?.Invoke(_sw.Elapsed.TotalMilliseconds); if (!_disposed) _timer?.Change(_pollInterval, Timeout.Infinite); }
            }

            private void Update(BaseDataVariableState variable, string valStr)
            {
                object? value = null;
                if (variable.DataType == DataTypeIds.Double) value = double.TryParse(valStr, out var d) ? d : 0.0;
                else if (variable.DataType == DataTypeIds.Int32) value = int.TryParse(valStr, out var i) ? i : 0;
                else if (variable.DataType == DataTypeIds.Boolean) value = bool.TryParse(valStr, out var b) ? b : false;
                else value = valStr;
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

            public void Dispose() { _disposed = true; _timer?.Dispose(); }
        }

        private class TcpItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public TcpConfig Config { get; set; } = null!;
        }
    }
}
