using Microsoft.Data.Sqlite;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// SQLite-backed audit trail that records operator actions:
/// variable writes, alarm acknowledgments, recipe operations, login/logout.
/// Stored in a separate database for compliance (FDA 21 CFR Part 11).
/// </summary>
public sealed class AuditTrailLogger : IDisposable
{
    private SqliteConnection? _connection;
    private readonly object _lock = new();
    private bool _initialized;
    private readonly int _maxAgeDays;
    private Timer? _cleanupTimer;

    private const string TableName = "audit_trail";

    public AuditTrailLogger(string dbPath, int maxAgeDays)
    {
        _maxAgeDays = maxAgeDays;

        try
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(dbPath));
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _connection = new SqliteConnection($"Data Source={dbPath}");
            _connection.Open();

            using (var walCmd = _connection.CreateCommand())
            {
                walCmd.CommandText = "PRAGMA journal_mode=WAL;";
                walCmd.ExecuteNonQuery();
            }

            using var cmd = _connection.CreateCommand();
            cmd.CommandText = $@"
                CREATE TABLE IF NOT EXISTS {TableName} (
                    id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    time        TEXT    NOT NULL,
                    action      TEXT    NOT NULL,
                    username    TEXT    NOT NULL,
                    target      TEXT    NOT NULL,
                    old_value   TEXT,
                    new_value   TEXT,
                    details     TEXT,
                    client_info TEXT
                );
                CREATE INDEX IF NOT EXISTS idx_{TableName}_time
                    ON {TableName} (time);
                CREATE INDEX IF NOT EXISTS idx_{TableName}_action_time
                    ON {TableName} (action, time);
                CREATE INDEX IF NOT EXISTS idx_{TableName}_user_time
                    ON {TableName} (username, time);
            ";
            cmd.ExecuteNonQuery();

            _initialized = true;

            if (_maxAgeDays > 0)
            {
                _cleanupTimer = new Timer(_ => PurgeOldRecords(), null,
                    TimeSpan.FromMinutes(5), TimeSpan.FromHours(1));
            }

            Log.Information("Audit trail initialized: {DbPath} (maxAge={MaxAgeDays}d)", dbPath, maxAgeDays);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initialize audit trail: {Message}", ex.Message);
        }
    }

    /// <summary>Log a variable write by an operator.</summary>
    public void LogVariableWrite(string username, string variablePath, string? oldValue, string? newValue, string? clientInfo = null)
        => LogEntry("VariableWrite", username, variablePath, oldValue, newValue, null, clientInfo);

    /// <summary>Log an alarm acknowledgment.</summary>
    public void LogAlarmAcknowledge(string username, string alarmPath, string? comment = null)
        => LogEntry("AlarmAcknowledge", username, alarmPath, null, null, comment, null);

    /// <summary>Log an alarm confirm.</summary>
    public void LogAlarmConfirm(string username, string alarmPath, string? comment = null)
        => LogEntry("AlarmConfirm", username, alarmPath, null, null, comment, null);

    /// <summary>Log an alarm shelve operation.</summary>
    public void LogAlarmShelve(string username, string alarmPath, string? details = null)
        => LogEntry("AlarmShelve", username, alarmPath, null, null, details, null);

    /// <summary>Log an alarm unshelve operation.</summary>
    public void LogAlarmUnshelve(string username, string alarmPath, string? details = null)
        => LogEntry("AlarmUnshelve", username, alarmPath, null, null, details, null);

    /// <summary>Log a recipe operation (load, save, activate, delete).</summary>
    public void LogRecipeAction(string username, string recipeName, string action, string targetRecipeName)
        => LogEntry($"Recipe{action}", username, recipeName, null, targetRecipeName, null, null);

    /// <summary>Log a user login.</summary>
    public void LogLogin(string username, string? clientInfo = null)
        => LogEntry("Login", username, "", null, null, null, clientInfo);

    /// <summary>Log a user logout.</summary>
    public void LogLogout(string username, string? clientInfo = null)
        => LogEntry("Logout", username, "", null, null, null, clientInfo);

    /// <summary>Log a configuration change.</summary>
    public void LogConfigChange(string username, string target, string? details = null)
        => LogEntry("ConfigChange", username, target, null, null, details, null);

    /// <summary>Query audit records for the REST API.</summary>
    public List<AuditRecord> Query(DateTime? startTime = null, DateTime? endTime = null,
        string? action = null, string? username = null, int maxRows = 1000)
    {
        var records = new List<AuditRecord>();
        if (!_initialized) return records;

        try
        {
            lock (_lock)
            {
                if (_connection == null) return records;

                using var cmd = _connection.CreateCommand();
                var conditions = new List<string>();
                if (startTime.HasValue)
                {
                    conditions.Add("time >= @start");
                    cmd.Parameters.AddWithValue("@start", startTime.Value.ToString("o"));
                }
                if (endTime.HasValue)
                {
                    conditions.Add("time <= @end");
                    cmd.Parameters.AddWithValue("@end", endTime.Value.ToString("o"));
                }
                if (!string.IsNullOrEmpty(action))
                {
                    conditions.Add("action = @action");
                    cmd.Parameters.AddWithValue("@action", action);
                }
                if (!string.IsNullOrEmpty(username))
                {
                    conditions.Add("username = @user");
                    cmd.Parameters.AddWithValue("@user", username);
                }

                var where = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";
                cmd.CommandText = $"SELECT id, time, action, username, target, old_value, new_value, details, client_info FROM {TableName} {where} ORDER BY id DESC LIMIT {maxRows}";

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    records.Add(new AuditRecord
                    {
                        Id = reader.GetInt64(0),
                        Time = reader.GetString(1),
                        Action = reader.GetString(2),
                        Username = reader.GetString(3),
                        Target = reader.GetString(4),
                        OldValue = reader.IsDBNull(5) ? null : reader.GetString(5),
                        NewValue = reader.IsDBNull(6) ? null : reader.GetString(6),
                        Details = reader.IsDBNull(7) ? null : reader.GetString(7),
                        ClientInfo = reader.IsDBNull(8) ? null : reader.GetString(8)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Audit trail query error");
        }

        return records;
    }

    private void LogEntry(string action, string username, string target,
        string? oldValue, string? newValue, string? details, string? clientInfo)
    {
        if (!_initialized) return;

        Task.Run(() =>
        {
            try
            {
                lock (_lock)
                {
                    if (_connection == null) return;

                    using var cmd = _connection.CreateCommand();
                    cmd.CommandText = $@"
                        INSERT INTO {TableName} (time, action, username, target, old_value, new_value, details, client_info)
                        VALUES (@t, @act, @user, @tgt, @old, @new, @det, @cli)";
                    cmd.Parameters.AddWithValue("@t", DateTime.UtcNow.ToString("o"));
                    cmd.Parameters.AddWithValue("@act", action);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@tgt", target);
                    cmd.Parameters.AddWithValue("@old", (object?)oldValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@new", (object?)newValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@det", (object?)details ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@cli", (object?)clientInfo ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Audit trail write error: {Message}", ex.Message);
            }
        });
    }

    private void PurgeOldRecords()
    {
        if (_maxAgeDays <= 0 || !_initialized) return;

        try
        {
            lock (_lock)
            {
                if (_connection == null) return;

                var cutoff = DateTime.UtcNow.AddDays(-_maxAgeDays).ToString("o");
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = $"DELETE FROM {TableName} WHERE time < @cutoff";
                cmd.Parameters.AddWithValue("@cutoff", cutoff);
                var deleted = cmd.ExecuteNonQuery();

                if (deleted > 0)
                    Log.Debug("Audit trail: purged {Count} records older than {Days} days", deleted, _maxAgeDays);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Audit trail cleanup error");
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        lock (_lock)
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }
    }
}

/// <summary>DTO for audit trail query results.</summary>
public class AuditRecord
{
    public long Id { get; set; }
    public string Time { get; set; } = "";
    public string Action { get; set; } = "";
    public string Username { get; set; } = "";
    public string Target { get; set; } = "";
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Details { get; set; }
    public string? ClientInfo { get; set; }
}
