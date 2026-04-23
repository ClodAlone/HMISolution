using Microsoft.Data.Sqlite;
using SharedModels;

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

            Serilog.Log.Information("Event logger initialized: {DbPath} (maxAge={MaxAgeDays}d)", dbPath, maxAgeDays);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Failed to initialize event logger: {Message}", ex.Message);
        }
    }

    // ─── Public logging methods ─────────────────────────────────────────

    /// <summary>Log an alarm activation or state change.</summary>
    public void LogAlarm(string severity, string source, string message, string? details = null)
        => Log("Alarm", severity, source, message, details);

    /// <summary>Log a user authentication event (login, logout, failed login).</summary>
    public void LogAuth(string severity, string source, string message, string? details = null)
        => Log("Auth", severity, source, message, details);

    /// <summary>Log a driver event (loaded, error, etc.).</summary>
    public void LogDriver(string severity, string source, string message, string? details = null)
        => Log("Driver", severity, source, message, details);

    /// <summary>Log a system event (startup, shutdown, config reload, etc.).</summary>
    public void LogSystem(string severity, string source, string message, string? details = null)
        => Log("System", severity, source, message, details);

    /// <summary>Log an unhandled exception / crash event.</summary>
    public void LogCrash(string severity, string source, string message, string? details = null)
        => Log("Crash", severity, source, message, details);

    // ─── Core logging ───────────────────────────────────────────────────

    /// <summary>
    /// Optional callback invoked after each event is logged.
    /// Used by redundancy to enqueue events for replication to the partner.
    /// Parameters: category, severity, source, message, details.
    /// </summary>
    public Action<string, string, string, string, string?>? OnEventLogged { get; set; }

    private void Log(string category, string severity, string source, string message, string? details)
    {
        if (!_initialized) return;

        // Notify replication hook (non-blocking, before async write)
        try { OnEventLogged?.Invoke(category, severity, source, message, details); }
        catch { /* replication hook must not break logging */ }

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
                Serilog.Log.Error(ex, "Event log write error: {Message}", ex.Message);
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
                    Serilog.Log.Debug("Event log: purged {Count} events older than {Days} days", deleted, _maxAgeDays);
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Event log cleanup error: {Message}", ex.Message);
        }
    }

    // ─── Query helpers ─────────────────────────────────────────────────

    /// <summary>
    /// Query events from the log with optional filtering.
    /// Returns a list of dictionaries containing event data.
    /// </summary>
    public List<Dictionary<string, object>> QueryEvents(
        string category = "",
        string severity = "",
        DateTime? startTime = null,
        DateTime? endTime = null,
        int limit = 100)
    {
        var results = new List<Dictionary<string, object>>();
        if (!_initialized) return results;

        try
        {
            lock (_lock)
            {
                if (_connection == null) return results;

                var conditions = new List<string>();
                if (!string.IsNullOrEmpty(category))
                    conditions.Add("category = @category");
                if (!string.IsNullOrEmpty(severity))
                    conditions.Add("severity = @severity");
                if (startTime.HasValue)
                    conditions.Add("time >= @startTime");
                if (endTime.HasValue)
                    conditions.Add("time <= @endTime");

                var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

                using var cmd = _connection.CreateCommand();
                cmd.CommandText = $"SELECT time, category, severity, source, message, details FROM {TableName} {whereClause} ORDER BY time DESC LIMIT @limit";

                if (!string.IsNullOrEmpty(category))
                    cmd.Parameters.AddWithValue("@category", category);
                if (!string.IsNullOrEmpty(severity))
                    cmd.Parameters.AddWithValue("@severity", severity);
                if (startTime.HasValue)
                    cmd.Parameters.AddWithValue("@startTime", startTime.Value.ToString("o"));
                if (endTime.HasValue)
                    cmd.Parameters.AddWithValue("@endTime", endTime.Value.ToString("o"));
                cmd.Parameters.AddWithValue("@limit", limit);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new Dictionary<string, object>
                    {
                        ["time"] = reader.GetString(0),
                        ["category"] = reader.GetString(1),
                        ["severity"] = reader.GetString(2),
                        ["source"] = reader.GetString(3),
                        ["message"] = reader.GetString(4),
                        ["details"] = reader.IsDBNull(5) ? "" : reader.GetString(5)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Event log query error: {Message}", ex.Message);
        }

        return results;
    }

    /// <summary>
    /// Returns the timestamp of the most recent event in the log, or null if the log is empty.
    /// Useful for determining when the server was last active.
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
            Serilog.Log.Error(ex, "Event log query error: {Message}", ex.Message);
        }

        return null;
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

    // ─── Redundancy helpers ────────────────────────────────────────────

    /// <summary>
    /// Returns the UTC timestamp of the most recent event, or null if empty.
    /// Used by redundancy to detect event history gaps after a restart.
    /// </summary>
    public DateTime? GetLatestEventTimestamp()
    {
        if (!_initialized) return null;
        try
        {
            lock (_lock)
            {
                if (_connection == null) return null;
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = $"SELECT MAX(time) FROM {TableName}";
                var result = cmd.ExecuteScalar();
                if (result is string s && DateTime.TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                    return dt;
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Debug("GetLatestEventTimestamp error: {Error}", ex.Message);
        }
        return null;
    }

    /// <summary>Read all events after the given UTC time. Used by redundancy gap-fill.</summary>
    public List<RedundancyService.EventEntry> ReadEventsSince(DateTime sinceUtc, int maxRows = 100_000)
    {
        var entries = new List<RedundancyService.EventEntry>();
        if (!_initialized) return entries;
        try
        {
            lock (_lock)
            {
                if (_connection == null) return entries;
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = $"SELECT time, category, severity, source, message, details FROM {TableName} WHERE time > @since ORDER BY time ASC LIMIT @limit";
                cmd.Parameters.AddWithValue("@since", sinceUtc.ToString("o"));
                cmd.Parameters.AddWithValue("@limit", maxRows);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    entries.Add(new RedundancyService.EventEntry
                    {
                        Time = DateTime.TryParse(reader.GetString(0), null, System.Globalization.DateTimeStyles.RoundtripKind, out var t) ? t : sinceUtc,
                        Category = reader.GetString(1),
                        Severity = reader.GetString(2),
                        Source = reader.GetString(3),
                        Message = reader.GetString(4),
                        Details = reader.IsDBNull(5) ? null : reader.GetString(5)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning("ReadEventsSince error: {Error}", ex.Message);
        }
        return entries;
    }

    /// <summary>
    /// Replay event entries received from the partner server during redundancy sync.
    /// Inserts directly without duplicate checks (events are append-only and time-ordered).
    /// </summary>
    public void ReplayEvents(List<RedundancyService.EventEntry> events)
    {
        if (!_initialized || _connection == null || events.Count == 0) return;

        lock (_lock)
        {
            try
            {
                using var tx = _connection.BeginTransaction();
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = $"INSERT INTO {TableName} (time, category, severity, source, message, details) VALUES (@t, @cat, @sev, @src, @msg, @det)";

                var pTime = cmd.Parameters.Add("@t", SqliteType.Text);
                var pCat = cmd.Parameters.Add("@cat", SqliteType.Text);
                var pSev = cmd.Parameters.Add("@sev", SqliteType.Text);
                var pSrc = cmd.Parameters.Add("@src", SqliteType.Text);
                var pMsg = cmd.Parameters.Add("@msg", SqliteType.Text);
                var pDet = cmd.Parameters.Add("@det", SqliteType.Text);

                foreach (var e in events)
                {
                    pTime.Value = e.Time.ToString("o");
                    pCat.Value = e.Category;
                    pSev.Value = e.Severity;
                    pSrc.Value = e.Source;
                    pMsg.Value = e.Message;
                    pDet.Value = e.Details != null ? (object)e.Details : DBNull.Value;
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                Serilog.Log.Debug("[Redundancy] Replayed {Count} event entries", events.Count);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning("[Redundancy] Event replay error: {Error}", ex.Message);
            }
        }
    }
}
