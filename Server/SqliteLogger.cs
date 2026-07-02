// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Data.Sqlite;
using Opc.Ua;
using SharedModels;

namespace SimpleOpcFileServer
{
    public class SqliteLogger : IVariableLogger
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly ConcurrentDictionary<string, object> _lastLoggedValues = new();
        private readonly ConcurrentDictionary<string, DateTime> _lastCleanupTimes = new();
        private SqliteConnection? _connection;
        private readonly object _lock = new();
        private bool _initialized;

        // Channel-based write queue replaces Task.Run + lock per variable.
        // A single consumer drains entries in batches within a transaction.
        private readonly Channel<WriteEntry> _writeChannel = Channel.CreateBounded<WriteEntry>(
            new BoundedChannelOptions(200_000)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = false
            });
        private Task? _writerTask;
        private readonly CancellationTokenSource _cts = new();
        private const int BatchFlushIntervalMs = 200;
        private const int MaxBatchSize = 5_000;

        private SqliteCommand? _insertCmd;
        private SqliteParameter? _pTime, _pName, _pVal, _pValStr, _pQuality;

        private readonly record struct WriteEntry(string NodeIdId, DateTime Timestamp, object Value, bool IsNumeric, uint Quality, TimeSpan? MaxAge);

        public SqliteLogger(string filePath, string tableName)
        {
            // If the path is empty or ":memory:", use in-memory; otherwise use the file
            if (string.IsNullOrWhiteSpace(filePath) || filePath == ":memory:")
                _connectionString = "Data Source=:memory:";
            else
                _connectionString = $"Data Source={filePath}";

            _tableName = tableName;
        }

        public void Initialize()
        {
            try
            {
                // Keep a single connection open for the lifetime of the logger.
                // For file-based SQLite this avoids re-opening overhead;
                // for in-memory SQLite the DB only exists while the connection is open.
                _connection = new SqliteConnection(_connectionString);
                _connection.Open();

                // Enable WAL mode for file-based databases (better concurrent read/write)
                if (!_connectionString.Contains(":memory:"))
                {
                    using var walCmd = _connection.CreateCommand();
                    walCmd.CommandText = "PRAGMA journal_mode=WAL;";
                    walCmd.ExecuteNonQuery();

                    using var syncCmd = _connection.CreateCommand();
                    syncCmd.CommandText = "PRAGMA synchronous=NORMAL;";
                    syncCmd.ExecuteNonQuery();
                }

                using var cmd = _connection.CreateCommand();
                cmd.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS {_tableName} (
                        time TEXT NOT NULL,
                        variable_name TEXT NOT NULL,
                        value REAL NULL,
                        value_str TEXT NULL,
                        quality INTEGER
                    );
                    CREATE INDEX IF NOT EXISTS idx_{_tableName}_name_time 
                        ON {_tableName} (variable_name, time);
                ";
                cmd.ExecuteNonQuery();

                // Prepare the reusable insert command
                _insertCmd = _connection.CreateCommand();
                _insertCmd.CommandText = $"INSERT INTO {_tableName} (time, variable_name, value, value_str, quality) VALUES ($time, $name, $val, $valstr, $quality)";
                _pTime = _insertCmd.Parameters.Add("$time", SqliteType.Text);
                _pName = _insertCmd.Parameters.Add("$name", SqliteType.Text);
                _pVal = _insertCmd.Parameters.Add("$val", SqliteType.Real);
                _pValStr = _insertCmd.Parameters.Add("$valstr", SqliteType.Text);
                _pQuality = _insertCmd.Parameters.Add("$quality", SqliteType.Integer);

                _initialized = true;
                _writerTask = Task.Run(WriterLoopAsync);

                Serilog.Log.Information("SQLite logger initialized ({ConnectionString})", _connectionString);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Failed to initialize SQLite logger: {Message}", ex.Message);
            }
        }

        public void Log(BaseDataVariableState variable, DataLoggingConfig config)
        {
            if (!_initialized || _connection == null) return;
            if (variable.Value == null) return;

            string key = variable.NodeId.ToString();
            object value = variable.Value;

            // Check hysteresis
            if (_lastLoggedValues.TryGetValue(key, out var lastValue) && lastValue != null)
            {
                if (IsNumeric(value) && IsNumeric(lastValue))
                {
                    double v1 = Convert.ToDouble(value);
                    double v2 = Convert.ToDouble(lastValue);
                    if (Math.Abs(v1 - v2) < config.Hysteresis)
                        return;
                }
                else
                {
                    if (value.Equals(lastValue)) return;
                }
            }

            string nodeIdId = variable.NodeId.Identifier.ToString()!;
            _writeChannel.Writer.TryWrite(new WriteEntry(
                nodeIdId,
                DateTime.UtcNow,
                value,
                IsNumeric(value),
                (uint)variable.StatusCode.Code,
                config.MaxAge));
            _lastLoggedValues[key] = value;
        }

        private async Task WriterLoopAsync()
        {
            var batch = new List<WriteEntry>(MaxBatchSize);
            var reader = _writeChannel.Reader;

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    batch.Clear();

                    if (await reader.WaitToReadAsync(_cts.Token))
                    {
                        while (batch.Count < MaxBatchSize && reader.TryRead(out var entry))
                            batch.Add(entry);
                    }

                    if (batch.Count == 0) continue;

                    if (batch.Count < MaxBatchSize / 2)
                    {
                        await Task.Delay(BatchFlushIntervalMs, _cts.Token);
                        while (batch.Count < MaxBatchSize && reader.TryRead(out var entry))
                            batch.Add(entry);
                    }

                    FlushBatch(batch);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "SQLite writer loop error: {Message}", ex.Message);
            }

            batch.Clear();
            while (reader.TryRead(out var entry))
                batch.Add(entry);
            if (batch.Count > 0)
                FlushBatch(batch);
        }

        private void FlushBatch(List<WriteEntry> batch)
        {
            try
            {
                lock (_lock)
                {
                    if (_connection == null || _insertCmd == null) return;

                    using var tx = _connection.BeginTransaction();
                    _insertCmd.Transaction = tx;

                    foreach (var entry in batch)
                    {
                        if (entry.MaxAge.HasValue)
                        {
                            var interval = entry.MaxAge.Value.TotalMinutes < 10 ? entry.MaxAge.Value : TimeSpan.FromMinutes(5);

                            if (!_lastCleanupTimes.ContainsKey(entry.NodeIdId) || (entry.Timestamp - _lastCleanupTimes[entry.NodeIdId]) > interval)
                            {
                                using var cleanCmd = _connection.CreateCommand();
                                cleanCmd.Transaction = tx;
                                cleanCmd.CommandText = $"DELETE FROM {_tableName} WHERE variable_name = @n AND time < @t_limit";
                                cleanCmd.Parameters.AddWithValue("@n", entry.NodeIdId);
                                cleanCmd.Parameters.AddWithValue("@t_limit", (entry.Timestamp - entry.MaxAge.Value).ToString("o"));
                                cleanCmd.ExecuteNonQuery();

                                _lastCleanupTimes[entry.NodeIdId] = entry.Timestamp;
                            }
                        }

                        _pTime!.Value = entry.Timestamp.ToString("o");
                        _pName!.Value = entry.NodeIdId;

                        if (entry.IsNumeric)
                        {
                            _pVal!.Value = Convert.ToDouble(entry.Value);
                            _pValStr!.Value = DBNull.Value;
                        }
                        else
                        {
                            _pVal!.Value = DBNull.Value;
                            _pValStr!.Value = entry.Value.ToString() ?? "";
                        }

                        _pQuality!.Value = (long)entry.Quality;
                        _insertCmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    _insertCmd.Transaction = null;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "SQLite batch flush error: {Message}", ex.Message);
            }
        }

        public List<DataValue> ReadHistory(string variableNodeId, DateTime startTime, DateTime endTime)
        {
            var values = new List<DataValue>();
            if (!_initialized || _connection == null) return values;

            try
            {
                lock (_lock)
                {
                    using var cmd = _connection.CreateCommand();
                    cmd.CommandText = $"SELECT time, value, value_str, quality FROM {_tableName} WHERE variable_name = @n AND time >= @s AND time <= @e ORDER BY time ASC";
                    cmd.Parameters.AddWithValue("@n", variableNodeId);
                    cmd.Parameters.AddWithValue("@s", startTime.ToString("o"));
                    cmd.Parameters.AddWithValue("@e", endTime.ToString("o"));

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var time = DateTime.Parse(reader.GetString(0)).ToUniversalTime();
                        object val = DBNull.Value;
                        if (!reader.IsDBNull(1)) val = reader.GetDouble(1);
                        else if (!reader.IsDBNull(2)) val = reader.GetString(2);

                        var quality = (uint)reader.GetInt32(3);

                        values.Add(new DataValue
                        {
                            Value = val,
                            SourceTimestamp = time,
                            ServerTimestamp = time,
                            StatusCode = new StatusCode(quality)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "SQLite history read error: {Message}", ex.Message);
            }

            return values;
        }

        private static bool IsNumeric(object? val)
        {
            return val is double or float or int or long or short or ushort or uint or ulong or byte or sbyte or decimal;
        }


        /// <summary>
        /// Replay data-logging entries received from the primary server during redundancy sync.
        /// Inserts directly without deadband/hysteresis checks.
        /// </summary>
        public void ReplayEntries(List<RedundancyService.LogEntry> entries)
        {
            if (!_initialized || _connection == null || entries.Count == 0) return;

            lock (_lock)
            {
                try
                {
                    using var tx = _connection.BeginTransaction();
                    using var cmd = _connection.CreateCommand();
                    cmd.CommandText = $"INSERT INTO {_tableName} (time, variable_name, value, value_str, quality) VALUES ($time, $name, $val, $valstr, $quality)";

                    var pTime = cmd.Parameters.Add("$time", Microsoft.Data.Sqlite.SqliteType.Text);
                    var pName = cmd.Parameters.Add("$name", Microsoft.Data.Sqlite.SqliteType.Text);
                    var pVal = cmd.Parameters.Add("$val", Microsoft.Data.Sqlite.SqliteType.Real);
                    var pValStr = cmd.Parameters.Add("$valstr", Microsoft.Data.Sqlite.SqliteType.Text);
                    var pQuality = cmd.Parameters.Add("$quality", Microsoft.Data.Sqlite.SqliteType.Integer);

                    foreach (var entry in entries)
                    {
                        pTime.Value = entry.Time.ToString("o");
                        pName.Value = entry.VariableName;
                        pVal.Value = entry.NumericValue.HasValue ? (object)entry.NumericValue.Value : DBNull.Value;
                        pValStr.Value = entry.StringValue != null ? (object)entry.StringValue : DBNull.Value;
                        pQuality.Value = (long)entry.Quality;
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    Serilog.Log.Debug("[Redundancy] Replayed {Count} log entries to SQLite", entries.Count);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning("[Redundancy] SQLite replay error: {Error}", ex.Message);
                }
            }
        }

        public DateTime? GetLatestTimestamp()
        {
            if (!_initialized || _connection == null) return null;
            lock (_lock)
            {
                try
                {
                    using var cmd = _connection.CreateCommand();
                    cmd.CommandText = $"SELECT MAX(time) FROM {_tableName}";
                    var result = cmd.ExecuteScalar();
                    if (result is string s && DateTime.TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                        return dt;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Debug("GetLatestTimestamp error: {Error}", ex.Message);
                }
            }
            return null;
        }

        public List<RedundancyService.LogEntry> ReadEntriesSince(DateTime sinceUtc, int maxRows = 100_000)
        {
            var entries = new List<RedundancyService.LogEntry>();
            if (!_initialized || _connection == null) return entries;
            lock (_lock)
            {
                try
                {
                    using var cmd = _connection.CreateCommand();
                    cmd.CommandText = $"SELECT time, variable_name, value, value_str, quality FROM {_tableName} WHERE time > $since ORDER BY time ASC LIMIT $limit";
                    cmd.Parameters.AddWithValue("$since", sinceUtc.ToString("o"));
                    cmd.Parameters.AddWithValue("$limit", maxRows);
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        entries.Add(new RedundancyService.LogEntry
                        {
                            Time = DateTime.TryParse(reader.GetString(0), null, System.Globalization.DateTimeStyles.RoundtripKind, out var t) ? t : sinceUtc,
                            VariableName = reader.GetString(1),
                            NumericValue = reader.IsDBNull(2) ? null : reader.GetDouble(2),
                            StringValue = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Quality = reader.IsDBNull(4) ? 0u : (uint)reader.GetInt64(4)
                        });
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning("ReadEntriesSince error: {Error}", ex.Message);
                }
            }
            return entries;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _writeChannel.Writer.TryComplete();
            _writerTask?.Wait(TimeSpan.FromSeconds(5));

            lock (_lock)
            {
                _insertCmd?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
                _connection = null;
            }
            _cts.Dispose();
        }
    }
}
