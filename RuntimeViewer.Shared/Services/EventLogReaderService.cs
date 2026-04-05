using Microsoft.Data.Sqlite;

namespace RuntimeViewer.Shared.Services;

/// <summary>A single event row from the server event log database.</summary>
public class EventLogEntry
{
    public long Id { get; set; }
    public DateTime Time { get; set; }
    public string Category { get; set; } = "";
    public string Severity { get; set; } = "";
    public string Source { get; set; } = "";
    public string Message { get; set; } = "";
    public string? Details { get; set; }
}

/// <summary>
/// Reads events from the server_events SQLite database created by <c>EventLogger</c>.
/// Resolves the database path from the <see cref="ProjectService"/> configuration.
/// </summary>
public class EventLogReaderService
{
    private readonly ProjectService _project;

    public EventLogReaderService(ProjectService project)
    {
        _project = project;
    }

    /// <summary>
    /// Read events from the event log database.
    /// </summary>
    /// <param name="timeRangeMinutes">Time range in minutes from now (0 = all).</param>
    /// <param name="maxRows">Maximum number of rows to return.</param>
    /// <param name="categories">Category filter (empty = all). e.g. "Alarm,Auth".</param>
    public Task<List<EventLogEntry>> ReadEventsAsync(int timeRangeMinutes, int maxRows, string? categories = null)
    {
        return Task.Run(() => ReadEvents(timeRangeMinutes, maxRows, categories));
    }

    private List<EventLogEntry> ReadEvents(int timeRangeMinutes, int maxRows, string? categories)
    {
        var results = new List<EventLogEntry>();
        var dbPath = ResolveDbPath();
        if (dbPath == null || !File.Exists(dbPath))
            return results;

        try
        {
            using var conn = new SqliteConnection($"Data Source={dbPath};Mode=ReadOnly");
            conn.Open();

            // Build query
            var conditions = new List<string>();
            var cmd = conn.CreateCommand();

            if (timeRangeMinutes > 0)
            {
                conditions.Add("time >= @startTime");
                cmd.Parameters.AddWithValue("@startTime", DateTime.UtcNow.AddMinutes(-timeRangeMinutes).ToString("o"));
            }

            if (!string.IsNullOrWhiteSpace(categories))
            {
                var cats = categories.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (cats.Length > 0)
                {
                    var placeholders = new List<string>();
                    for (int i = 0; i < cats.Length; i++)
                    {
                        var paramName = $"@cat{i}";
                        placeholders.Add(paramName);
                        cmd.Parameters.AddWithValue(paramName, cats[i]);
                    }
                    conditions.Add($"category IN ({string.Join(", ", placeholders)})");
                }
            }

            var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";
            cmd.CommandText = $@"
                SELECT id, time, category, severity, source, message, details
                FROM server_events
                {whereClause}
                ORDER BY time DESC
                LIMIT @maxRows";
            cmd.Parameters.AddWithValue("@maxRows", maxRows > 0 ? maxRows : 200);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new EventLogEntry
                {
                    Id = reader.GetInt64(0),
                    Time = DateTime.TryParse(reader.GetString(1), null, System.Globalization.DateTimeStyles.RoundtripKind, out var t) ? t : DateTime.MinValue,
                    Category = reader.GetString(2),
                    Severity = reader.GetString(3),
                    Source = reader.GetString(4),
                    Message = reader.GetString(5),
                    Details = reader.IsDBNull(6) ? null : reader.GetString(6)
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EventLogReader error: {ex.Message}");
        }

        return results;
    }

    private string? ResolveDbPath()
    {
        var model = _project.Model;
        if (model == null) return null;

        var evtCfg = model.Server?.EventLog;
        var dbPath = evtCfg?.DbPath ?? "events.db";

        if (!Path.IsPathRooted(dbPath) && !string.IsNullOrEmpty(_project.ConfigPath))
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(_project.ConfigPath));
            if (!string.IsNullOrEmpty(dir))
            {
                // Check Data subfolder first (new convention), then root (backward compat)
                var dataPath = Path.Combine(dir, "Data", dbPath);
                dbPath = File.Exists(dataPath) ? dataPath : Path.Combine(dir, dbPath);
            }
        }

        return dbPath;
    }
}

