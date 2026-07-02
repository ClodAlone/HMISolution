// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Microsoft.Data.Sqlite;
using SharedModels;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SimpleOpcFileServer;

/// <summary>
/// FDA 21 CFR Part 11 compliant audit trail service with cryptographic integrity verification.
/// Maintains a tamper-proof chain of audit records using SHA256 hash linking.
/// </summary>
public sealed class AuditTrailService : IDisposable
{
    private SqliteConnection? _connection;
    private readonly object _lock = new();
    private bool _initialized;
    private readonly ComplianceConfig _config;
    private string? _lastRecordHash;
    private Timer? _cleanupTimer;
    private readonly ILogger _logger;

    private const string TableName = "audit_trail";

    public AuditTrailService(ComplianceConfig config, ILogger logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (!_config.Enabled)
        {
            _logger.LogInformation("FDA 21 CFR Part 11 compliance mode is disabled");
            return;
        }

        try
        {
            InitializeDatabase();
            LoadLastRecordHash();

            // Start periodic cleanup if retention is enabled
            if (_config.AuditRetentionDays > 0)
            {
                _cleanupTimer = new Timer(_ => PurgeOldRecords(), null,
                    TimeSpan.FromHours(1), TimeSpan.FromHours(24));
            }

            _logger.LogInformation("Audit trail service initialized (retention={RetentionDays}d)", 
                _config.AuditRetentionDays);

            // Verify integrity on startup if enabled
            if (_config.VerifyIntegrityOnStartup)
            {
                Task.Run(async () =>
                {
                    var result = await VerifyIntegrityAsync();
                    if (!result.IsIntact)
                    {
                        _logger.LogCritical("Audit trail integrity verification FAILED: {Message}", result.Message);
                    }
                    else
                    {
                        _logger.LogInformation("Audit trail integrity verified: {TotalRecords} records", result.TotalRecords);
                    }
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize audit trail service");
        }
    }

    private void InitializeDatabase()
    {
        var connectionString = $"Data Source={_config.AuditDbPath}";

        // Ensure directory exists
        var dir = Path.GetDirectoryName(Path.GetFullPath(_config.AuditDbPath));
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
                id                      INTEGER PRIMARY KEY AUTOINCREMENT,
                timestamp               TEXT    NOT NULL,
                event_type              TEXT    NOT NULL,
                username                TEXT    NOT NULL,
                full_name               TEXT    NOT NULL,
                action                  TEXT    NOT NULL,
                affected_entity         TEXT    NOT NULL,
                old_value               TEXT,
                new_value               TEXT,
                reason_for_change       TEXT,
                signature_username      TEXT,
                signature_full_name     TEXT,
                signature_timestamp     TEXT,
                signature_meaning       TEXT,
                signature_comment       TEXT,
                second_signature_username  TEXT,
                second_signature_full_name TEXT,
                second_signature_timestamp TEXT,
                second_signature_meaning   TEXT,
                second_signature_comment   TEXT,
                details                 TEXT,
                source                  TEXT    NOT NULL,
                previous_record_hash    TEXT,
                record_hash             TEXT    NOT NULL,
                session_id              TEXT,
                client_ip_address       TEXT
            );

            CREATE INDEX IF NOT EXISTS idx_{TableName}_timestamp
                ON {TableName} (timestamp);
            CREATE INDEX IF NOT EXISTS idx_{TableName}_event_type
                ON {TableName} (event_type);
            CREATE INDEX IF NOT EXISTS idx_{TableName}_username
                ON {TableName} (username);
            CREATE INDEX IF NOT EXISTS idx_{TableName}_affected_entity
                ON {TableName} (affected_entity);
        ";
        cmd.ExecuteNonQuery();

        _initialized = true;
    }

    private void LoadLastRecordHash()
    {
        if (!_initialized || _connection == null) return;

        lock (_lock)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = $"SELECT record_hash FROM {TableName} ORDER BY id DESC LIMIT 1";
            _lastRecordHash = cmd.ExecuteScalar() as string;
        }
    }

    /// <summary>
    /// Log an audit trail event with cryptographic hash chain.
    /// </summary>
    public async Task<long> LogAsync(AuditRecord record)
    {
        if (!_initialized || _connection == null || !_config.Enabled)
            return -1;

        if (record == null)
            throw new ArgumentNullException(nameof(record));

        try
        {
            // Set timestamp if not already set
            if (record.Timestamp == default)
                record.Timestamp = DateTime.UtcNow;

            // Link to previous record
            record.PreviousRecordHash = _lastRecordHash;

            // Calculate hash for this record
            record.RecordHash = ComputeRecordHash(record);

            long id;
            lock (_lock)
            {
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = $@"
                    INSERT INTO {TableName} (
                        timestamp, event_type, username, full_name, action, affected_entity,
                        old_value, new_value, reason_for_change,
                        signature_username, signature_full_name, signature_timestamp, signature_meaning, signature_comment,
                        second_signature_username, second_signature_full_name, second_signature_timestamp, second_signature_meaning, second_signature_comment,
                        details, source, previous_record_hash, record_hash, session_id, client_ip_address
                    ) VALUES (
                        @timestamp, @event_type, @username, @full_name, @action, @affected_entity,
                        @old_value, @new_value, @reason_for_change,
                        @signature_username, @signature_full_name, @signature_timestamp, @signature_meaning, @signature_comment,
                        @second_signature_username, @second_signature_full_name, @second_signature_timestamp, @second_signature_meaning, @second_signature_comment,
                        @details, @source, @previous_record_hash, @record_hash, @session_id, @client_ip_address
                    )";

                cmd.Parameters.AddWithValue("@timestamp", record.Timestamp.ToString("o"));
                cmd.Parameters.AddWithValue("@event_type", record.EventType.ToString());
                cmd.Parameters.AddWithValue("@username", record.Username);
                cmd.Parameters.AddWithValue("@full_name", record.FullName);
                cmd.Parameters.AddWithValue("@action", record.Action);
                cmd.Parameters.AddWithValue("@affected_entity", record.AffectedEntity);
                cmd.Parameters.AddWithValue("@old_value", (object?)record.OldValue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@new_value", (object?)record.NewValue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@reason_for_change", (object?)record.ReasonForChange ?? DBNull.Value);

                // Primary signature
                cmd.Parameters.AddWithValue("@signature_username", (object?)record.Signature?.Username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@signature_full_name", (object?)record.Signature?.FullName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@signature_timestamp", record.Signature != null ? record.Signature.SignedAt.ToString("o") : DBNull.Value);
                cmd.Parameters.AddWithValue("@signature_meaning", (object?)record.Signature?.Meaning ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@signature_comment", (object?)record.Signature?.Comment ?? DBNull.Value);

                // Second signature (for dual approval)
                cmd.Parameters.AddWithValue("@second_signature_username", (object?)record.SecondSignature?.Username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@second_signature_full_name", (object?)record.SecondSignature?.FullName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@second_signature_timestamp", record.SecondSignature != null ? record.SecondSignature.SignedAt.ToString("o") : DBNull.Value);
                cmd.Parameters.AddWithValue("@second_signature_meaning", (object?)record.SecondSignature?.Meaning ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@second_signature_comment", (object?)record.SecondSignature?.Comment ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@details", (object?)record.Details ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@source", record.Source);
                cmd.Parameters.AddWithValue("@previous_record_hash", (object?)record.PreviousRecordHash ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@record_hash", record.RecordHash);
                cmd.Parameters.AddWithValue("@session_id", (object?)record.SessionId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@client_ip_address", (object?)record.ClientIpAddress ?? DBNull.Value);

                cmd.ExecuteNonQuery();

                // Get the ID of the inserted record
                cmd.CommandText = "SELECT last_insert_rowid()";
                id = (long)cmd.ExecuteScalar()!;

                // Update last record hash
                _lastRecordHash = record.RecordHash;
            }

            record.Id = id;
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit record: {EventType} by {Username}", 
                record.EventType, record.Username);
            return -1;
        }
    }

    /// <summary>
    /// Compute SHA256 hash of the audit record for integrity verification.
    /// </summary>
    private string ComputeRecordHash(AuditRecord record)
    {
        var dataToHash = new StringBuilder();
        dataToHash.Append(record.Timestamp.ToString("o"));
        dataToHash.Append('|');
        dataToHash.Append(record.EventType);
        dataToHash.Append('|');
        dataToHash.Append(record.Username);
        dataToHash.Append('|');
        dataToHash.Append(record.FullName);
        dataToHash.Append('|');
        dataToHash.Append(record.Action);
        dataToHash.Append('|');
        dataToHash.Append(record.AffectedEntity);
        dataToHash.Append('|');
        dataToHash.Append(record.OldValue ?? "");
        dataToHash.Append('|');
        dataToHash.Append(record.NewValue ?? "");
        dataToHash.Append('|');
        dataToHash.Append(record.ReasonForChange ?? "");
        dataToHash.Append('|');
        dataToHash.Append(record.Signature != null ? JsonSerializer.Serialize(record.Signature) : "");
        dataToHash.Append('|');
        dataToHash.Append(record.SecondSignature != null ? JsonSerializer.Serialize(record.SecondSignature) : "");
        dataToHash.Append('|');
        dataToHash.Append(record.Details ?? "");
        dataToHash.Append('|');
        dataToHash.Append(record.Source);
        dataToHash.Append('|');
        dataToHash.Append(record.PreviousRecordHash ?? "");

        var bytes = Encoding.UTF8.GetBytes(dataToHash.ToString());
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Verify the integrity of the entire audit trail.
    /// </summary>
    public async Task<AuditIntegrityResult> VerifyIntegrityAsync()
    {
        var result = new AuditIntegrityResult();

        if (!_initialized || _connection == null || !_config.Enabled)
        {
            result.Message = "Audit trail service is not initialized or compliance mode is disabled";
            return result;
        }

        try
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    using var cmd = _connection.CreateCommand();
                    cmd.CommandText = $@"
                        SELECT id, timestamp, event_type, username, full_name, action, affected_entity,
                               old_value, new_value, reason_for_change,
                               signature_username, signature_full_name, signature_timestamp, signature_meaning, signature_comment,
                               second_signature_username, second_signature_full_name, second_signature_timestamp, second_signature_meaning, second_signature_comment,
                               details, source, previous_record_hash, record_hash
                        FROM {TableName}
                        ORDER BY id ASC";

                    using var reader = cmd.ExecuteReader();
                    string? expectedPreviousHash = null;

                    while (reader.Read())
                    {
                        result.TotalRecords++;
                        var id = reader.GetInt64(0);

                        // Reconstruct record
                        var record = new AuditRecord
                        {
                            Id = id,
                            Timestamp = DateTime.Parse(reader.GetString(1)),
                            EventType = Enum.Parse<ComplianceEventType>(reader.GetString(2)),
                            Username = reader.GetString(3),
                            FullName = reader.GetString(4),
                            Action = reader.GetString(5),
                            AffectedEntity = reader.GetString(6),
                            OldValue = reader.IsDBNull(7) ? null : reader.GetString(7),
                            NewValue = reader.IsDBNull(8) ? null : reader.GetString(8),
                            ReasonForChange = reader.IsDBNull(9) ? null : reader.GetString(9),
                            Details = reader.IsDBNull(20) ? null : reader.GetString(20),
                            Source = reader.GetString(21),
                            PreviousRecordHash = reader.IsDBNull(22) ? null : reader.GetString(22)
                        };

                        // Reconstruct signatures
                        if (!reader.IsDBNull(10))
                        {
                            record.Signature = new ElectronicSignature
                            {
                                Username = reader.GetString(10),
                                FullName = reader.GetString(11),
                                SignedAt = DateTime.Parse(reader.GetString(12)),
                                Meaning = reader.GetString(13),
                                Comment = reader.IsDBNull(14) ? null : reader.GetString(14)
                            };
                        }

                        if (!reader.IsDBNull(15))
                        {
                            record.SecondSignature = new ElectronicSignature
                            {
                                Username = reader.GetString(15),
                                FullName = reader.GetString(16),
                                SignedAt = DateTime.Parse(reader.GetString(17)),
                                Meaning = reader.GetString(18),
                                Comment = reader.IsDBNull(19) ? null : reader.GetString(19)
                            };
                        }

                        var storedHash = reader.GetString(23);

                        // Verify hash chain
                        if (record.PreviousRecordHash != expectedPreviousHash)
                        {
                            result.FailedRecords++;
                            result.FailedRecordIds.Add(id);
                            _logger.LogWarning("Audit record {Id}: Previous hash mismatch (expected={Expected}, actual={Actual})",
                                id, expectedPreviousHash, record.PreviousRecordHash);
                        }

                        // Verify record hash
                        var computedHash = ComputeRecordHash(record);
                        if (computedHash != storedHash)
                        {
                            result.FailedRecords++;
                            if (!result.FailedRecordIds.Contains(id))
                                result.FailedRecordIds.Add(id);
                            _logger.LogWarning("Audit record {Id}: Hash verification failed", id);
                        }

                        expectedPreviousHash = storedHash;
                    }
                }
            });

            result.IsIntact = result.FailedRecords == 0;
            result.Message = result.IsIntact
                ? $"Audit trail integrity verified successfully. {result.TotalRecords} records checked."
                : $"Audit trail integrity check FAILED. {result.FailedRecords} of {result.TotalRecords} records failed verification.";
        }
        catch (Exception ex)
        {
            result.Message = $"Error during integrity verification: {ex.Message}";
            _logger.LogError(ex, "Failed to verify audit trail integrity");
        }

        return result;
    }

    /// <summary>
    /// Query audit records with optional filtering.
    /// </summary>
    public async Task<List<AuditRecord>> QueryAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? username = null,
        ComplianceEventType? eventType = null,
        string? affectedEntity = null,
        int limit = 1000)
    {
        var records = new List<AuditRecord>();

        if (!_initialized || _connection == null || !_config.Enabled)
            return records;

        try
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    using var cmd = _connection.CreateCommand();
                    var whereClause = new List<string>();

                    if (startDate.HasValue)
                    {
                        whereClause.Add("timestamp >= @startDate");
                        cmd.Parameters.AddWithValue("@startDate", startDate.Value.ToString("o"));
                    }
                    if (endDate.HasValue)
                    {
                        whereClause.Add("timestamp <= @endDate");
                        cmd.Parameters.AddWithValue("@endDate", endDate.Value.ToString("o"));
                    }
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        whereClause.Add("username LIKE @username");
                        cmd.Parameters.AddWithValue("@username", $"%{username}%");
                    }
                    if (eventType.HasValue)
                    {
                        whereClause.Add("event_type = @eventType");
                        cmd.Parameters.AddWithValue("@eventType", eventType.Value.ToString());
                    }
                    if (!string.IsNullOrWhiteSpace(affectedEntity))
                    {
                        whereClause.Add("affected_entity LIKE @affectedEntity");
                        cmd.Parameters.AddWithValue("@affectedEntity", $"%{affectedEntity}%");
                    }

                    var where = whereClause.Any() ? "WHERE " + string.Join(" AND ", whereClause) : "";

                    cmd.CommandText = $@"
                        SELECT id, timestamp, event_type, username, full_name, action, affected_entity,
                               old_value, new_value, reason_for_change,
                               signature_username, signature_full_name, signature_timestamp, signature_meaning, signature_comment,
                               second_signature_username, second_signature_full_name, second_signature_timestamp, second_signature_meaning, second_signature_comment,
                               details, source, previous_record_hash, record_hash, session_id, client_ip_address
                        FROM {TableName}
                        {where}
                        ORDER BY id DESC
                        LIMIT @limit";

                    cmd.Parameters.AddWithValue("@limit", limit);

                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var record = new AuditRecord
                        {
                            Id = reader.GetInt64(0),
                            Timestamp = DateTime.Parse(reader.GetString(1)),
                            EventType = Enum.Parse<ComplianceEventType>(reader.GetString(2)),
                            Username = reader.GetString(3),
                            FullName = reader.GetString(4),
                            Action = reader.GetString(5),
                            AffectedEntity = reader.GetString(6),
                            OldValue = reader.IsDBNull(7) ? null : reader.GetString(7),
                            NewValue = reader.IsDBNull(8) ? null : reader.GetString(8),
                            ReasonForChange = reader.IsDBNull(9) ? null : reader.GetString(9),
                            Details = reader.IsDBNull(20) ? null : reader.GetString(20),
                            Source = reader.GetString(21),
                            PreviousRecordHash = reader.IsDBNull(22) ? null : reader.GetString(22),
                            RecordHash = reader.GetString(23),
                            SessionId = reader.IsDBNull(24) ? null : reader.GetString(24),
                            ClientIpAddress = reader.IsDBNull(25) ? null : reader.GetString(25)
                        };

                        // Reconstruct signatures
                        if (!reader.IsDBNull(10))
                        {
                            record.Signature = new ElectronicSignature
                            {
                                Username = reader.GetString(10),
                                FullName = reader.GetString(11),
                                SignedAt = DateTime.Parse(reader.GetString(12)),
                                Meaning = reader.GetString(13),
                                Comment = reader.IsDBNull(14) ? null : reader.GetString(14),
                                IsVerified = true
                            };
                        }

                        if (!reader.IsDBNull(15))
                        {
                            record.SecondSignature = new ElectronicSignature
                            {
                                Username = reader.GetString(15),
                                FullName = reader.GetString(16),
                                SignedAt = DateTime.Parse(reader.GetString(17)),
                                Meaning = reader.GetString(18),
                                Comment = reader.IsDBNull(19) ? null : reader.GetString(19),
                                IsVerified = true
                            };
                        }

                        records.Add(record);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query audit records");
        }

        return records;
    }

    /// <summary>
    /// Get audit statistics for reporting.
    /// </summary>
    public async Task<Dictionary<string, object>> GetStatisticsAsync()
    {
        var stats = new Dictionary<string, object>();

        if (!_initialized || _connection == null || !_config.Enabled)
            return stats;

        try
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    using var cmd = _connection.CreateCommand();

                    // Total records
                    cmd.CommandText = $"SELECT COUNT(*) FROM {TableName}";
                    stats["TotalRecords"] = (long)cmd.ExecuteScalar()!;

                    // Records by event type
                    cmd.CommandText = $"SELECT event_type, COUNT(*) FROM {TableName} GROUP BY event_type";
                    var eventTypeCounts = new Dictionary<string, long>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            eventTypeCounts[reader.GetString(0)] = reader.GetInt64(1);
                        }
                    }
                    stats["EventTypeCounts"] = eventTypeCounts;

                    // Active users (last 30 days)
                    cmd.CommandText = $@"
                        SELECT COUNT(DISTINCT username) 
                        FROM {TableName} 
                        WHERE timestamp >= datetime('now', '-30 days')";
                    stats["ActiveUsers30Days"] = (long)cmd.ExecuteScalar()!;

                    // Oldest and newest records
                    cmd.CommandText = $"SELECT MIN(timestamp), MAX(timestamp) FROM {TableName}";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            stats["OldestRecord"] = reader.GetString(0);
                            stats["NewestRecord"] = reader.GetString(1);
                        }
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get audit statistics");
        }

        return stats;
    }

    private void PurgeOldRecords()
    {
        if (!_initialized || _connection == null || !_config.Enabled || _config.AuditRetentionDays == 0)
            return;

        try
        {
            lock (_lock)
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-_config.AuditRetentionDays);
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = $"DELETE FROM {TableName} WHERE timestamp < @cutoffDate";
                cmd.Parameters.AddWithValue("@cutoffDate", cutoffDate.ToString("o"));
                var deleted = cmd.ExecuteNonQuery();

                if (deleted > 0)
                {
                    _logger.LogInformation("Purged {Count} audit records older than {Days} days", 
                        deleted, _config.AuditRetentionDays);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to purge old audit records");
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        _connection?.Dispose();
    }
}
