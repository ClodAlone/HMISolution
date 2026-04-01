using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Npgsql;
using Opc.Ua;
using SharedModels;
using System.Collections.Generic;

namespace SimpleOpcFileServer
{
    public class TimescaleLogger : IVariableLogger
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly ConcurrentDictionary<string, object> _lastLoggedValues = new();
        private readonly ConcurrentDictionary<string, DateTime> _lastCleanupTimes = new();
        private bool _tablesCreated = false;

        public TimescaleLogger(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        public void Initialize()
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                // Create table
                using (var cmd = new NpgsqlCommand($@"
                    CREATE TABLE IF NOT EXISTS {_tableName} (
                        time TIMESTAMPTZ NOT NULL,
                        variable_name TEXT NOT NULL,
                        value DOUBLE PRECISION NULL,
                        value_str TEXT NULL,
                        quality INTEGER
                    );
                ", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Convert to hypertable (ignore error if already hypertable)
                try
                {
                    using (var cmd = new NpgsqlCommand($"SELECT create_hypertable('{_tableName}', 'time', if_not_exists => TRUE);", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, Strings.Timescale_HypertableWarning, ex.Message);
                }

                _tablesCreated = true;
                Serilog.Log.Information(Strings.Timescale_Initialized);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, Strings.Timescale_InitFailed, ex.Message);
            }
        }

        public void Log(BaseDataVariableState variable, DataLoggingConfig config)
        {
            if (!_tablesCreated) return;
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
                    {
                        return; // Change too small
                    }
                }
                else
                {
                    if (value.Equals(lastValue)) return; // No change
                }
            }

            // Log it
            // Fire and forget task to avoid blocking driver thread
            Task.Run(() => LogToDb(variable, value, config.MaxAge));
            
            _lastLoggedValues[key] = value;
        }

        private void LogToDb(BaseDataVariableState variable, object value, TimeSpan? maxAge)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                
                string nodeIdId = variable.NodeId.Identifier.ToString();

                // Cleanup if maxAge is set
                if (maxAge.HasValue)
                {
                    // Clean only occasionally or always? Always is safest for this requirement.
                    // But deleting on every insert is heavy.
                    // Maybe optimize later. For now, just execute delete.
                    
                    var now = DateTime.UtcNow;
                    var interval = maxAge.Value.TotalMinutes < 10 ? maxAge.Value : TimeSpan.FromMinutes(5);
                    
                    if (!_lastCleanupTimes.ContainsKey(nodeIdId) || (now - _lastCleanupTimes[nodeIdId]) > interval)
                    {
                        using var cleanCmd = new NpgsqlCommand($"DELETE FROM {_tableName} WHERE variable_name = @n AND time < @t_limit", conn);
                        cleanCmd.Parameters.AddWithValue("n", nodeIdId ?? variable.DisplayName.Text);
                        cleanCmd.Parameters.AddWithValue("t_limit", now - maxAge.Value);
                        cleanCmd.ExecuteNonQuery();
                        
                        _lastCleanupTimes[nodeIdId] = now;
                    }
                }

                using var cmd = new NpgsqlCommand($"INSERT INTO {_tableName} (time, variable_name, value, value_str, quality) VALUES (@t, @n, @v, @s, @q)", conn);
                cmd.Parameters.AddWithValue("t", DateTime.UtcNow); 
                // Use NodeId Identifier (string path) to be unique
                // var nodeIdId = variable.NodeId.Identifier.ToString(); // Already defined above
                cmd.Parameters.AddWithValue("n", nodeIdId ?? variable.DisplayName.Text);
                
                if (IsNumeric(value))
                {
                    cmd.Parameters.AddWithValue("v", Convert.ToDouble(value));
                    cmd.Parameters.AddWithValue("s", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("v", DBNull.Value);
                    cmd.Parameters.AddWithValue("s", value.ToString() ?? "");
                }
                
                cmd.Parameters.AddWithValue("q", variable.StatusCode.Code);
                
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, Strings.Timescale_LogkError, ex.Message);
            }
        }

        public List<DataValue> ReadHistory(string variableNodeId, DateTime startTime, DateTime endTime)
        {
            var values = new List<DataValue>();
            if (!_tablesCreated) return values;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                using var cmd = new NpgsqlCommand(
                    $"SELECT time, value, value_str, quality FROM {_tableName} WHERE variable_name = @n AND time >= @s AND time <= @e ORDER BY time ASC", 
                    conn);
                
                cmd.Parameters.AddWithValue("n", variableNodeId);
                cmd.Parameters.AddWithValue("s", startTime);
                cmd.Parameters.AddWithValue("e", endTime);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var time = reader.GetDateTime(0);
                    object val = DBNull.Value;
                    if (!reader.IsDBNull(1)) val = reader.GetDouble(1);
                    else if (!reader.IsDBNull(2)) val = reader.GetString(2);
                    
                    var quality = (uint)reader.GetInt32(3);

                    var dv = new DataValue
                    {
                        Value = val,
                        SourceTimestamp = time,
                        ServerTimestamp = time,
                        StatusCode = new StatusCode(quality)
                    };
                    values.Add(dv);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, Strings.Timescale_HistoryReadError, ex.Message);
            }
            return values;
        }

        private bool IsNumeric(object? val)
        {
            if (val == null) return false;
            return val is double || val is float || val is int || val is long || val is short || val is ushort || val is uint || val is ulong || val is byte || val is sbyte || val is decimal;
        }


        /// <summary>
        /// Replay data-logging entries received from the primary server during redundancy sync.
        /// Inserts directly without deadband/hysteresis checks.
        /// </summary>
        public void ReplayEntries(List<RedundancyService.LogEntry> entries)
        {
            if (string.IsNullOrEmpty(_connectionString) || entries.Count == 0) return;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();

                using var tx = conn.BeginTransaction();
                foreach (var entry in entries)
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = $"INSERT INTO {_tableName} (time, variable_name, value, value_str, quality) VALUES (@time, @name, @val, @valstr, @quality)";
                    cmd.Parameters.AddWithValue("time", entry.Time);
                    cmd.Parameters.AddWithValue("name", entry.VariableName);
                    cmd.Parameters.AddWithValue("val", entry.NumericValue.HasValue ? (object)entry.NumericValue.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("valstr", entry.StringValue != null ? (object)entry.StringValue : DBNull.Value);
                    cmd.Parameters.AddWithValue("quality", (long)entry.Quality);
                    cmd.ExecuteNonQuery();
                }
                tx.Commit();

                Serilog.Log.Debug("[Redundancy] Replayed {Count} log entries to TimescaleDB", entries.Count);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning("[Redundancy] TimescaleDB replay error: {Error}", ex.Message);
            }
        }

        public DateTime? GetLatestTimestamp()
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT MAX(time) FROM {_tableName}";
                var result = cmd.ExecuteScalar();
                if (result is DateTime dt) return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }
            catch (Exception ex)
            {
                Serilog.Log.Debug("GetLatestTimestamp error: {Error}", ex.Message);
            }
            return null;
        }

        public List<RedundancyService.LogEntry> ReadEntriesSince(DateTime sinceUtc, int maxRows = 100_000)
        {
            var entries = new List<RedundancyService.LogEntry>();
            if (string.IsNullOrEmpty(_connectionString)) return entries;
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT time, variable_name, value, value_str, quality FROM {_tableName} WHERE time > @since ORDER BY time ASC LIMIT @limit";
                cmd.Parameters.AddWithValue("since", sinceUtc);
                cmd.Parameters.AddWithValue("limit", maxRows);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    entries.Add(new RedundancyService.LogEntry
                    {
                        Time = reader.GetDateTime(0),
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
            return entries;
        }

        public void Dispose()
        {
            // Nothing to dispose really connection is per call
        }
    }
}
