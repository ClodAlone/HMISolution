// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Microsoft.Data.SqlClient;
using Opc.Ua;
using Serilog;

namespace SimpleOpcFileServer
{
    public class SqlConfig
    {
        public string ConnectionString { get; set; } = "";
        public string Query { get; set; } = "";
        public int PollTime { get; set; } = 1000;
    }

    public class SqlDriver : IDriver, IDisposable
    {
        public string Key => "Sql";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly Dictionary<string, SqlDevice> _devices = new();
        private readonly object _lock = new();
        private bool _disposed;
        private readonly ISystemContext _context;

        public SqlDriver(ISystemContext context) { _context = context; }

        private void RaiseError(string source, string message) => OnError?.Invoke(source, message);
        private void RaiseCycle(double ms) => OnCycleCompleted?.Invoke(ms);

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var sqlConfig = JsonSerializer.Deserialize<SqlConfig>(configJson);
            if (sqlConfig == null) return;

            lock (_lock)
            {
                var key = sqlConfig.ConnectionString;
                if (!_devices.TryGetValue(key, out var device))
                {
                    device = new SqlDevice(key, _context, RaiseError, RaiseCycle);
                    _devices[key] = device;
                }
                var item = new SqlItem { Variable = variable, Config = sqlConfig };
                device.AddItem(item);
                variable.OnSimpleWriteValue = (ISystemContext ctx, NodeState node, ref object value) => device.Write(item, value);
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

        private class SqlDevice : IDisposable
        {
            private readonly string _connectionString;
            private readonly ISystemContext _context;
            private readonly Action<string, string>? _onError;
            private readonly Action<double>? _onCycle;
            private readonly List<SqlItem> _items = new();
            private Timer? _timer;
            private int _pollInterval = 1000;
            private int _minPollTime = int.MaxValue;
            private bool _disposed;
            private readonly object _deviceLock = new();
            private string? _lastLoggedError;
            private int _consecutiveErrors;
            private const int MaxBackoffMs = 30_000;

            public SqlDevice(string connectionString, ISystemContext context, Action<string, string>? onError = null, Action<double>? onCycle = null) { _connectionString = connectionString; _context = context; _onError = onError; _onCycle = onCycle; }

            public void AddItem(SqlItem item)
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
                List<SqlItem> itemsToPoll;
                lock (_deviceLock) { if (_disposed) return; itemsToPoll = new List<SqlItem>(_items); }

                try
                {
                    using var conn = new SqlConnection(_connectionString);
                    conn.Open();
                    foreach (var item in itemsToPoll)
                    {
                        try
                        {
                            using var cmd = new SqlCommand(item.Config.Query, conn);
                            var result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value) Update(item.Variable, result);
                            else UpdateError(item.Variable, "No result");
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message != _lastLoggedError)
                            {
                                Log.Error(ex, "SQL query error for {Name}: {Message}", item.Variable.DisplayName, ex.Message);
                                _lastLoggedError = ex.Message;
                            }
                            else Log.Debug("SQL query error (repeated) for {Name}: {Message}", item.Variable.DisplayName, ex.Message);
                            _onError?.Invoke(_connectionString, $"Query error ({item.Variable.DisplayName}): {ex.Message}");
                            UpdateError(item.Variable, ex.Message);
                            anyError = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message != _lastLoggedError)
                    {
                        Log.Error(ex, "SQL connection error: {Message}", ex.Message);
                        _lastLoggedError = ex.Message;
                    }
                    else Log.Debug("SQL connection error (repeated): {Message}", ex.Message);
                    _onError?.Invoke(_connectionString, $"Connection error: {ex.Message}");
                    foreach (var item in itemsToPoll) UpdateError(item.Variable, ex.Message);
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
                try
                {
                    variable.Value = value; variable.StatusCode = StatusCodes.Good;
                    variable.Timestamp = DateTime.UtcNow; variable.ClearChangeMasks(_context, false);
                }
                catch { }
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

            public ServiceResult Write(SqlItem item, object value)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(item.Config.Query))
                        return ServiceResult.Create(StatusCodes.BadConfigurationError, "SQL query missing");

                    var query = item.Config.Query;
                    var valueText = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
                    if (query.Contains("{value}", StringComparison.OrdinalIgnoreCase))
                        query = query.Replace("{value}", valueText, StringComparison.OrdinalIgnoreCase);

                    var normalized = query.Trim();
                    if (!normalized.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase)
                        && !normalized.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase)
                        && !normalized.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase)
                        && !normalized.StartsWith("MERGE", StringComparison.OrdinalIgnoreCase)
                        && !normalized.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase))
                        return ServiceResult.Create(StatusCodes.BadNotWritable, "SQL query is not a write statement");

                    using var conn = new SqlConnection(_connectionString);
                    conn.Open();
                    using var cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    item.Variable.Value = value; item.Variable.StatusCode = StatusCodes.Good;
                    item.Variable.Timestamp = DateTime.UtcNow; item.Variable.ClearChangeMasks(_context, false);
                    return ServiceResult.Good;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "SQL write error: {Message}", ex.Message);
                    _onError?.Invoke(_connectionString, $"Write error: {ex.Message}");
                    return ServiceResult.Create(ex, StatusCodes.BadUnexpectedError, ex.Message);
                }
            }

            public void Dispose() { _disposed = true; _timer?.Dispose(); }
        }

        private class SqlItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public SqlConfig Config { get; set; } = null!;
        }
    }
}
