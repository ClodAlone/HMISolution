using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Opc.Ua;
using S7.Net;
using Serilog;

namespace SimpleOpcFileServer
{
    public class S7Config
    {
        public string IpAddress { get; set; } = "";
        public int Rack { get; set; }
        public int Slot { get; set; }
        public string Address { get; set; } = "";
        public int PollTime { get; set; } = 1000;
    }

    public class S7Driver : IDriver, IDisposable
    {
        public string Key => "S7";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly Dictionary<string, S7Device> _devices = new();
        private readonly object _lock = new();
        private bool _disposed;
        private readonly ISystemContext _context;

        public S7Driver(ISystemContext context) { _context = context; }

        private void RaiseError(string source, string message) => OnError?.Invoke(source, message);
        private void RaiseCycle(double ms) => OnCycleCompleted?.Invoke(ms);

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var s7Config = JsonSerializer.Deserialize<S7Config>(configJson);
            if (s7Config == null) return;

            lock (_lock)
            {
                var key = $"{s7Config.IpAddress}:{s7Config.Rack}:{s7Config.Slot}";
                if (!_devices.TryGetValue(key, out var device))
                {
                    device = new S7Device(key, s7Config.IpAddress, (short)s7Config.Rack, (short)s7Config.Slot, _context, RaiseError, RaiseCycle);
                    _devices[key] = device;
                }
                device.AddItem(new S7Item { Variable = variable, Config = s7Config });
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

        private class S7Device : IDisposable
        {
            private readonly string _key;
            private readonly string _ip;
            private readonly short _rack, _slot;
            private readonly ISystemContext _context;
            private readonly Action<string, string>? _onError;
            private readonly Action<double>? _onCycle;
            private readonly List<S7Item> _items = new();
            private Timer? _timer;
            private Plc? _plc;
            private int _pollInterval = 1000;
            private int _minPollTime = int.MaxValue;
            private bool _disposed;
            private readonly object _deviceLock = new();
            private string? _lastLoggedError;
            private int _consecutiveErrors;
            private const int MaxBackoffMs = 30_000;

            public S7Device(string key, string ip, short rack, short slot, ISystemContext context, Action<string, string>? onError = null, Action<double>? onCycle = null)
            { _key = key; _ip = ip; _rack = rack; _slot = slot; _context = context; _onError = onError; _onCycle = onCycle; }

            public void AddItem(S7Item item)
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
                List<S7Item> itemsToPoll;
                Plc? plc;
                lock (_deviceLock)
                {
                    if (_disposed) return;
                    if (_plc == null) _plc = new Plc(CpuType.S71200, _ip, _rack, _slot);
                    plc = _plc;
                    itemsToPoll = new List<S7Item>(_items);
                }
                if (plc == null) return;

                try
                {
                    if (!plc.IsConnected)
                    {
                        try { plc.Open(); }
                        catch (Exception ex)
                        {
                            if (ex.Message != _lastLoggedError)
                            {
                                Log.Error(ex, "S7 connection error for {Key}: {Message}", _key, ex.Message);
                                _lastLoggedError = ex.Message;
                            }
                            else Log.Debug("S7 connection error (repeated) for {Key}: {Message}", _key, ex.Message);
                            _onError?.Invoke(_key, $"Connection error: {ex.Message}");
                            foreach (var item in itemsToPoll) UpdateError(item.Variable, ex.Message);
                            anyError = true;
                            return;
                        }
                    }
                    if (plc.IsConnected)
                    {
                        foreach (var item in itemsToPoll)
                        {
                            try
                            {
                                var val = plc.Read(item.Config.Address);
                                if (val != null) Update(item.Variable, val);
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message != _lastLoggedError)
                                {
                                    Log.Error(ex, "S7 read error for {Name}: {Message}", item.Variable.DisplayName, ex.Message);
                                    _lastLoggedError = ex.Message;
                                }
                                else Log.Debug("S7 read error (repeated) for {Name}: {Message}", item.Variable.DisplayName, ex.Message);
                                _onError?.Invoke(_key, $"Read error ({item.Variable.DisplayName}): {ex.Message}");
                                UpdateError(item.Variable, ex.Message);
                                anyError = true;
                            }
                        }
                    }
                    else foreach (var item in itemsToPoll) UpdateError(item.Variable, "Not connected");
                }
                catch (Exception ex)
                {
                    if (ex.Message != _lastLoggedError)
                    {
                        Log.Error(ex, "S7 PLC error for {Key}: {Message}", _key, ex.Message);
                        _lastLoggedError = ex.Message;
                    }
                    else Log.Debug("S7 PLC error (repeated) for {Key}: {Message}", _key, ex.Message);
                    _onError?.Invoke(_key, $"PLC error: {ex.Message}");
                    foreach (var item in itemsToPoll) UpdateError(item.Variable, ex.Message);
                    lock (_deviceLock) { if (_plc == plc) DisposePlc(); }
                    anyError = true;
                }
                finally
                {
                    if (anyError) _consecutiveErrors++; else { _consecutiveErrors = 0; _lastLoggedError = null; }
                    var delay = _consecutiveErrors > 0 ? Math.Min(_pollInterval * (1 << Math.Min(_consecutiveErrors, 10)), MaxBackoffMs) : _pollInterval;
                    _onCycle?.Invoke(_sw.Elapsed.TotalMilliseconds);
                    if (!_disposed) _timer?.Change(delay, Timeout.Infinite);
                }
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

            private void DisposePlc() { try { _plc?.Close(); } catch { } _plc = null; }
            public void Dispose() { _disposed = true; _timer?.Dispose(); DisposePlc(); }
        }

        private class S7Item
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public S7Config Config { get; set; } = null!;
        }
    }
}
