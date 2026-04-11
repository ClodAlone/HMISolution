using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Opc.Ua;
using Serilog;

namespace SimpleOpcFileServer
{
    public class RestConfig
    {
        public string Url { get; set; } = "";
        public string? JsonPath { get; set; }
        public int PollTime { get; set; } = 1000;
    }

    public class RestDriver : IDriver, IDisposable
    {
        public string Key => "Rest";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly Dictionary<string, RestDevice> _devices = new();
        private readonly object _lock = new();
        private bool _disposed;
        private readonly ISystemContext _context;

        public RestDriver(ISystemContext context) { _context = context; }

        private void RaiseError(string source, string message) => OnError?.Invoke(source, message);
        private void RaiseCycle(double ms) => OnCycleCompleted?.Invoke(ms);

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var restConfig = JsonSerializer.Deserialize<RestConfig>(configJson);
            if (restConfig == null) return;

            lock (_lock)
            {
                var key = restConfig.Url;
                if (!_devices.TryGetValue(key, out var device))
                {
                    device = new RestDevice(key, _context, RaiseError, RaiseCycle);
                    _devices[key] = device;
                }
                device.AddItem(new RestItem { Variable = variable, Config = restConfig });
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

        private class RestDevice : IDisposable
        {
            private readonly string _url;
            private readonly ISystemContext _context;
            private readonly Action<string, string>? _onError;
            private readonly Action<double>? _onCycle;
            private readonly List<RestItem> _items = new();
            private Timer? _timer;
            private int _pollInterval = 1000;
            private int _minPollTime = int.MaxValue;
            private bool _disposed;
            private readonly object _deviceLock = new();
            private readonly HttpClient _client = new();
            private string? _lastLoggedError;
            private int _consecutiveErrors;
            private const int MaxBackoffMs = 30_000;

            public RestDevice(string url, ISystemContext context, Action<string, string>? onError = null, Action<double>? onCycle = null) { _url = url; _context = context; _onError = onError; _onCycle = onCycle; }

            public void AddItem(RestItem item)
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

            private async void Poll(object? state)
            {
                if (_disposed) return;
                var _sw = System.Diagnostics.Stopwatch.StartNew();
                bool anyError = false;
                List<RestItem> snapshot;
                lock (_deviceLock) { if (_disposed) return; snapshot = new List<RestItem>(_items); }

                try
                {
                    var response = await _client.GetAsync(_url);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(json);
                        foreach (var item in snapshot)
                        {
                            try { UpdateItem(item, doc.RootElement); }
                            catch { UpdateError(item.Variable); }
                        }
                    }
                    else foreach (var item in snapshot) UpdateError(item.Variable);
                }
                catch (Exception ex)
                {
                    if (ex.Message != _lastLoggedError)
                    {
                        Log.Error(ex, "REST fetch error for {Url}: {Message}", _url, ex.Message);
                        _lastLoggedError = ex.Message;
                    }
                    else Log.Debug("REST fetch error (repeated) for {Url}: {Message}", _url, ex.Message);
                    _onError?.Invoke(_url, $"Fetch error: {ex.Message}");
                    foreach (var item in snapshot) UpdateError(item.Variable, ex.Message);
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

            private void UpdateItem(RestItem item, JsonElement root)
            {
                var target = root;
                if (!string.IsNullOrEmpty(item.Config.JsonPath))
                {
                    foreach (var p in item.Config.JsonPath.Split('.'))
                    {
                        if (!target.TryGetProperty(p, out var next)) { UpdateError(item.Variable); return; }
                        target = next;
                    }
                }
                object? val = null;
                if (item.Variable.DataType == DataTypeIds.Double) val = target.GetDouble();
                else if (item.Variable.DataType == DataTypeIds.Int32) val = target.GetInt32();
                else if (item.Variable.DataType == DataTypeIds.Boolean) val = target.GetBoolean();
                else val = target.ToString();

                if (val != null)
                {
                    item.Variable.Value = val; item.Variable.StatusCode = StatusCodes.Good;
                    item.Variable.Timestamp = DateTime.UtcNow; item.Variable.ClearChangeMasks(_context, false);
                }
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

            public void Dispose() { _disposed = true; _timer?.Dispose(); _client.Dispose(); }
        }

        private class RestItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public RestConfig Config { get; set; } = null!;
        }
    }
}
