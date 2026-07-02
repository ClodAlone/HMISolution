// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Microsoft.Data.Sqlite;
using SharedModels;
using SharedModels.CloudRelay;

namespace CloudBridge;

/// <summary>
/// SQLite-backed store-and-forward queue for edge-to-cloud data sync.
/// When the cloud hub is unreachable, value snapshots are spooled to disk.
/// Once connectivity is restored, spooled rows are drained in batches.
/// Thread-safe: the spool is accessed from the OPC notification callback
/// (write) and the drain timer (read + delete).
/// </summary>
public sealed class StoreAndForwardQueue : IDisposable
{
    private readonly StoreAndForwardConfig _config;
    private SqliteConnection? _conn;
    private readonly object _lock = new();
    private bool _disposed;

    public StoreAndForwardQueue(StoreAndForwardConfig config, string projectDir)
    {
        _config = config;

        var spoolPath = config.SpoolPath;
        if (!Path.IsPathRooted(spoolPath))
            spoolPath = Path.Combine(projectDir, spoolPath);
        spoolPath = Path.GetFullPath(spoolPath);

        var dir = Path.GetDirectoryName(spoolPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        _conn = new SqliteConnection($"Data Source={spoolPath}");
        _conn.Open();

        // WAL mode for concurrent read/write without blocking
        using var walCmd = _conn.CreateCommand();
        walCmd.CommandText = "PRAGMA journal_mode=WAL;";
        walCmd.ExecuteNonQuery();

        using var cmd = _conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS spool (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                time TEXT NOT NULL,
                path TEXT NOT NULL,
                value TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS idx_spool_id ON spool (id);
        ";
        cmd.ExecuteNonQuery();

        Log($"Store-and-forward queue opened at {spoolPath}");
        var pending = GetPendingCount();
        if (pending > 0)
            Log($"  {pending} row(s) pending from previous session");
    }

    /// <summary>
    /// Enqueue a batch of tag values that could not be pushed to the cloud.
    /// Called from the OPC notification thread when the hub is disconnected.
    /// </summary>
    public void Enqueue(List<TagValueDto> values)
    {
        if (_disposed || values.Count == 0) return;

        lock (_lock)
        {
            if (_conn == null) return;

            using var tx = _conn.BeginTransaction();
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = "INSERT INTO spool (time, path, value) VALUES (@t, @p, @v)";

            var pTime = cmd.CreateParameter(); pTime.ParameterName = "@t"; cmd.Parameters.Add(pTime);
            var pPath = cmd.CreateParameter(); pPath.ParameterName = "@p"; cmd.Parameters.Add(pPath);
            var pVal = cmd.CreateParameter(); pVal.ParameterName = "@v"; cmd.Parameters.Add(pVal);

            var now = DateTime.UtcNow.ToString("o");
            foreach (var tv in values)
            {
                pTime.Value = now;
                pPath.Value = tv.Path;
                pVal.Value = tv.Value;
                cmd.ExecuteNonQuery();
            }

            tx.Commit();
        }

        // Purge excess rows if over the limit
        PurgeExcess();
    }

    /// <summary>
    /// Dequeue up to <paramref name="batchSize"/> rows for forwarding to the cloud.
    /// Returns the rows and their database IDs so they can be deleted after
    /// successful transmission.
    /// </summary>
    public (List<TagValueDto> Values, List<long> Ids) Dequeue(int batchSize)
    {
        var values = new List<TagValueDto>();
        var ids = new List<long>();
        if (_disposed) return (values, ids);

        lock (_lock)
        {
            if (_conn == null) return (values, ids);

            using var cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT id, path, value FROM spool ORDER BY id ASC LIMIT @n";
            cmd.Parameters.AddWithValue("@n", batchSize);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(reader.GetInt64(0));
                values.Add(new TagValueDto
                {
                    Path = reader.GetString(1),
                    Value = reader.GetString(2)
                });
            }
        }

        return (values, ids);
    }

    /// <summary>
    /// Acknowledge that the given rows were successfully sent.
    /// Deletes them from the spool.
    /// </summary>
    public void Acknowledge(List<long> ids)
    {
        if (_disposed || ids.Count == 0) return;

        lock (_lock)
        {
            if (_conn == null) return;

            // Delete in batches of 500 to stay within SQLite limits
            for (int i = 0; i < ids.Count; i += 500)
            {
                var batch = ids.Skip(i).Take(500).ToList();
                using var cmd = _conn.CreateCommand();
                var placeholders = string.Join(",", batch.Select((_, idx) => $"@id{idx}"));
                cmd.CommandText = $"DELETE FROM spool WHERE id IN ({placeholders})";
                for (int j = 0; j < batch.Count; j++)
                    cmd.Parameters.AddWithValue($"@id{j}", batch[j]);
                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>Returns the number of rows still in the spool.</summary>
    public long GetPendingCount()
    {
        if (_disposed) return 0;
        lock (_lock)
        {
            if (_conn == null) return 0;
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM spool";
            return (long)(cmd.ExecuteScalar() ?? 0);
        }
    }

    private void PurgeExcess()
    {
        if (_config.MaxRows <= 0) return;
        lock (_lock)
        {
            if (_conn == null) return;

            using var countCmd = _conn.CreateCommand();
            countCmd.CommandText = "SELECT COUNT(*) FROM spool";
            var count = (long)(countCmd.ExecuteScalar() ?? 0);

            if (count > _config.MaxRows)
            {
                var excess = count - _config.MaxRows;
                using var delCmd = _conn.CreateCommand();
                delCmd.CommandText = $"DELETE FROM spool WHERE id IN (SELECT id FROM spool ORDER BY id ASC LIMIT @n)";
                delCmd.Parameters.AddWithValue("@n", excess);
                delCmd.ExecuteNonQuery();
                Log($"Purged {excess} oldest spool row(s) (limit: {_config.MaxRows})");
            }
        }
    }

    private static void Log(string message) =>
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SAF] {message}");

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        lock (_lock)
        {
            _conn?.Close();
            _conn?.Dispose();
            _conn = null;
        }
    }
}
