using Microsoft.Data.Sqlite;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// SQLite-backed event journal that records server events:
/// alarms, user authentication, driver events, system/config events.
/// Periodically purges events older than <see cref="EventLogConfig.MaxAgeDays"/>.
/// </summary>
public sealed class EventLogger : IDisposable
{
    private SqliteConnection? _connection;
    private readonly object _lock = new();
    private bool _initialized;
    private readonly int _maxAgeDays;
    private DateTime _lastCleanup = DateTime.MinValue;
    private Timer? _cleanupTimer;

    private const string TableName = "server_events";

    public EventLogger(string dbPath, int maxAgeDays)
    {
        _maxAgeDays = maxAgeDays;
        var connectionString = $"Data Source={dbPath}";

        try
        {
            // Ensure directory exists
            var dir = Path.GetDirectoryName(Path.GetFullPath(dbPath));
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _connection = new SqliteConnection(connectionString);
            _connection.Open();

            // WAL mode for better concurrent read/write
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
                    category    TEXT    NOT NULL,
                    severity    TEXT    NOT NULL,
                    source      TEXT    NOT NULL,
                    message     TEXT    NOT NULL,
                    details     TEXT
                );
                CREATE INDEX IF NOT EXISTS idx_{TableName}_time
                    ON {TableName} (time);
                CREATE INDEX IF NOT EXISTS idx_{TableName}_category_time
                    ON {TableName} (category, time);
            ";
            cmd.ExecuteNonQuery();

            _initialized = true;

            // Start periodic cleanup (every 15 minutes)
            if (_maxAgeDays > 0)
            {
                _cleanupTimer = new Timer(_ => PurgeOldEvents(), null,
                    TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(15));
            }

            Log.Information("Event logger initialized: {DbPath} (maxAge={MaxAgeDays}d)", dbPath, maxAgeDays);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to initialize event logger: {Message}", ex.Message);
        }
    }

    // ─── Public logging methods ─────────────────────────────────────────

    /// <summary>Log an alarm activation or state change.</summary>
    public void LogAlarm(string severity, string source, string message, string? details = null)
        => LogEvent("Alarm", severity, source, message, details);

    /// <summary>Log a user authentication event (login, logout, failed login).</summary>
    public void LogAuth(string severity, string source, string message, string? details = null)
        => LogEvent("Auth", severity, source, message, details);

    /// <summary>Log a driver event (loaded, error, etc.).</summary>
    public void LogDriver(string severity, string source, string message, string? details = null)
        => LogEvent("Driver", severity, source, message, details);

    /// <summary>Log a system event (startup, shutdown, config reload, etc.).</summary>
    public void LogSystem(string severity, string source, string message, string? details = null)
        => LogEvent("System", severity, source, message, details);

    /// <summary>Log an unhandled exception / crash event.</summary>
    public void LogCrash(string severity, string source, string message, string? details = null)
        => LogEvent("Crash", severity, source, message, details);

    // ─── Core logging ───────────────────────────────────────────────────

    private void LogEvent(string category, string severity, string source, string message, string? details)
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
                        INSERT INTO {TableName} (time, category, severity, source, message, details)
                        VALUES (@t, @cat, @sev, @src, @msg, @det)";
                    cmd.Parameters.AddWithValue("@t", DateTime.UtcNow.ToString("o"));
                    cmd.Parameters.AddWithValue("@cat", category);
                    cmd.Parameters.AddWithValue("@sev", severity);
                    cmd.Parameters.AddWithValue("@src", source);
                    cmd.Parameters.AddWithValue("@msg", message);
                    cmd.Parameters.AddWithValue("@det", (object?)details ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Event log write error: {Message}", ex.Message);
            }
        });
    }

    // ─── Cleanup ────────────────────────────────────────────────────────

    private void PurgeOldEvents()
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
                    Log.Debug("Event log: purged {Count} events older than {Days} days", deleted, _maxAgeDays);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Event log cleanup error: {Message}", ex.Message);
        }
    }

    // ─── Query helpers ─────────────────────────────────────────────────

    /// <summary>
    /// Returns the timestamp of the most recent event in the log, or null if the log is empty.
    /// </summary>
    public DateTime? GetLastEventTime()
    {
        if (!_initialized) return null;

        try
        {
            lock (_lock)
            {
                if (_connection == null) return null;

                using var cmd = _connection.CreateCommand();
                cmd.CommandText = $"SELECT time FROM {TableName} ORDER BY id DESC LIMIT 1";
                var result = cmd.ExecuteScalar();
                if (result is string timeStr && DateTime.TryParse(timeStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                    return dt;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Event log query error: {Message}", ex.Message);
        }

        return null;
    }

    /// <summary>Query events with optional filters.</summary>
    public List<EventRecord> QueryEvents(DateTime? start = null, DateTime? end = null, string? category = null, int max = 500)
    {
        var results = new List<EventRecord>();
        if (!_initialized) return results;

        lock (_lock)
        {
            if (_connection == null) return results;

            using var cmd = _connection.CreateCommand();
            var where = new List<string>();
            if (start.HasValue)
            {
                where.Add("time >= @start");
                cmd.Parameters.AddWithValue("@start", start.Value.ToString("o"));
            }
            if (end.HasValue)
            {
                where.Add("time <= @end");
                cmd.Parameters.AddWithValue("@end", end.Value.ToString("o"));
            }
            if (!string.IsNullOrEmpty(category))
            {
                where.Add("category = @cat");
                cmd.Parameters.AddWithValue("@cat", category);
            }

            var whereClause = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";
            cmd.CommandText = $"SELECT time, category, severity, source, message, details FROM {TableName} {whereClause} ORDER BY time DESC LIMIT @max";
            cmd.Parameters.AddWithValue("@max", max);

            try
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new EventRecord
                    {
                        Time = reader.GetString(0),
                        Category = reader.GetString(1),
                        Severity = reader.GetString(2),
                        Source = reader.GetString(3),
                        Message = reader.GetString(4),
                        Details = reader.IsDBNull(5) ? null : reader.GetString(5)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Event log query error");
            }
        }

        return results;
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

    public class EventRecord
    {
        public string Time { get; set; } = "";
        public string Category { get; set; } = "";
        public string Severity { get; set; } = "";
        public string Source { get; set; } = "";
        public string Message { get; set; } = "";
        public string? Details { get; set; }
    }
}
