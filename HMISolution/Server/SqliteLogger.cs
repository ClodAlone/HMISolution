using System.Collections.Concurrent;
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
        private LoggingCache? _cache;

        public SqliteLogger(string filePath, string tableName)
        {
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
                _connection = new SqliteConnection(_connectionString);
                _connection.Open();
                if (!_connectionString.Contains(":memory:"))
                {
                    using var walCmd = _connection.CreateCommand();
                    walCmd.CommandText = "PRAGMA journal_mode=WAL;";
                    walCmd.ExecuteNonQuery();
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
                _initialized = true;
                _cache = new LoggingCache();
                _cache.OnOverflow += () => Serilog.Log.Warning(
                    "SQLite logging cache overflow: entries being dropped (max={MaxSize})", _cache.MaxSize);
                Serilog.Log.Information("SQLite logger initialized ({ConnectionString}), cache max={MaxSize}",
                    _connectionString, _cache.MaxSize);
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
            _cache?.TryEnqueue(() => LogToDb(variable, value, config.MaxAge));
            _lastLoggedValues[key] = value;
        }

        private void LogToDb(BaseDataVariableState variable, object value, TimeSpan? maxAge)
        {
            try
            {
                lock (_lock)
                {
                    if (_connection == null) return;
                    string nodeIdId = variable.NodeId.Identifier.ToString()!;
                    var now = DateTime.UtcNow;
                    if (maxAge.HasValue)
                    {
                        var interval = maxAge.Value.TotalMinutes < 10 ? maxAge.Value : TimeSpan.FromMinutes(5);
                        if (!_lastCleanupTimes.ContainsKey(nodeIdId) || (now - _lastCleanupTimes[nodeIdId]) > interval)
                        {
                            using var cleanCmd = _connection.CreateCommand();
                            cleanCmd.CommandText = $"DELETE FROM {_tableName} WHERE variable_name = @n AND time < @t_limit";
                            cleanCmd.Parameters.AddWithValue("@n", nodeIdId);
                            cleanCmd.Parameters.AddWithValue("@t_limit", (now - maxAge.Value).ToString("o"));
                            cleanCmd.ExecuteNonQuery();
                            _lastCleanupTimes[nodeIdId] = now;
                        }
                    }
                    using var cmd = _connection.CreateCommand();
                    cmd.CommandText = $"INSERT INTO {_tableName} (time, variable_name, value, value_str, quality) VALUES (@t, @n, @v, @s, @q)";
                    cmd.Parameters.AddWithValue("@t", now.ToString("o"));
                    cmd.Parameters.AddWithValue("@n", nodeIdId);
                    if (IsNumeric(value))
                    {
                        cmd.Parameters.AddWithValue("@v", Convert.ToDouble(value));
                        cmd.Parameters.AddWithValue("@s", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@v", DBNull.Value);
                        cmd.Parameters.AddWithValue("@s", value.ToString() ?? "");
                    }
                    cmd.Parameters.AddWithValue("@q", (int)variable.StatusCode.Code);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "SQLite log error: {Message}", ex.Message);
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

        public LoggerCacheStats? GetCacheStats() => _cache?.GetStats();

        private static bool IsNumeric(object? val)
        {
            return val is double or float or int or long or short or ushort or uint or ulong or byte or sbyte or decimal;
        }

        public void Dispose()
        {
            _cache?.Dispose();
            lock (_lock)
            {
                _connection?.Close();
                _connection?.Dispose();
                _connection = null;
            }
        }
    }
}
