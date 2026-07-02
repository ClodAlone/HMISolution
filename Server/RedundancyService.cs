// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Implements server redundancy with primary/standby failover.
///
/// Responsibilities:
/// - Periodic heartbeat to the partner server (HTTP GET /redundancy/heartbeat)
/// - Failover detection when partner heartbeats are missed
/// - State sync: primary pushes variable snapshots + data-logging batches to standby
/// - Driver gating: drivers only poll on the active server
/// - Automatic switch-back when the original primary recovers (optional)
///
/// Communication uses a lightweight HTTP listener on the diagnostics port
/// (endpoints under /redundancy/*) and an HttpClient for outbound calls.
/// </summary>
public sealed class RedundancyService : IDisposable
{
    // ─── Public state ────────────────────────────────────────────

    /// <summary>Current role this instance is executing.</summary>
    public RedundancyRole ActiveRole { get; private set; }

    /// <summary>Whether this instance is the one actively running drivers and logging.</summary>
    public bool IsActive => ActiveRole == RedundancyRole.Active;

    /// <summary>Whether scripts should run on this instance (active, or standby with ScriptsRunOnStandby).</summary>
    public bool ShouldRunScripts => IsActive || _config.ScriptsRunOnStandby;

    /// <summary>Whether PLC programs should run on this instance (active, or standby with PlcRunOnStandby).</summary>
    public bool ShouldRunPlc => IsActive || _config.PlcRunOnStandby;

    /// <summary>Whether the partner is currently reachable.</summary>
    public bool PartnerAlive { get; private set; }

    /// <summary>UTC time of the last successful heartbeat from the partner.</summary>
    public DateTime LastPartnerHeartbeat { get; private set; }

    /// <summary>Fires when the active/standby role changes.</summary>
    public event Action<RedundancyRole>? RoleChanged;

    // ─── Configuration ───────────────────────────────────────────

    private readonly RedundancyConfig _config;
    private readonly string _configuredRole;
    private readonly TimeSpan _heartbeatInterval;
    private readonly int _failoverMissedBeats;
    private readonly TimeSpan _syncInterval;

    // ─── Internal state ──────────────────────────────────────────

    private Timer? _heartbeatTimer;
    private Timer? _syncTimer;
    private int _missedHeartbeats;
    private readonly HttpClient _http;
    private readonly ConcurrentDictionary<string, BaseDataVariableState> _variables;
    private IVariableLogger? _logger;
    private EventLogger? _eventLogger;

    // State-sync buffer: variable values received from primary, to be applied on standby
    private readonly ConcurrentDictionary<string, VariableSnapshot> _pendingSnapshots = new();

    // Data-logging replication buffer
    private readonly ConcurrentQueue<LogEntry> _replicationQueue = new();
    private const int MaxReplicationQueueSize = 50_000;

    // Event replication buffer
    private readonly ConcurrentQueue<EventEntry> _eventReplicationQueue = new();
    private const int MaxEventReplicationQueueSize = 10_000;

    public RedundancyService(
        RedundancyConfig config,
        ConcurrentDictionary<string, BaseDataVariableState> variables)
    {
        _config = config;
        _variables = variables;
        _configuredRole = config.Role;
        _heartbeatInterval = TimeSpan.FromSeconds(Math.Max(config.HeartbeatIntervalSeconds, 1));
        _failoverMissedBeats = Math.Max(config.FailoverMissedHeartbeats, 2);
        _syncInterval = TimeSpan.FromMilliseconds(Math.Max(config.StateSyncIntervalMs, 1000));

        ActiveRole = _configuredRole == "Primary" ? RedundancyRole.Active : RedundancyRole.Standby;

        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    }

    /// <summary>Inject the logger and event logger after they are created in LoadModel.</summary>
    public void SetLogger(IVariableLogger? logger) => _logger = logger;
    public void SetEventLogger(EventLogger? eventLogger) => _eventLogger = eventLogger;

    // ─── Lifecycle ───────────────────────────────────────────────

    public void Start()
    {
        Log.Information("[Redundancy] Starting as {Role} (configured: {Configured}), partner: {Partner}",
            ActiveRole, _configuredRole, _config.PartnerEndpoint);

        _eventLogger?.LogSystem("Info", "Redundancy", $"Service started as {ActiveRole} (configured: {_configuredRole})");

        // Heartbeat timer — checks partner health
        _heartbeatTimer = new Timer(_ => _ = CheckPartnerAsync(), null, TimeSpan.FromSeconds(2), _heartbeatInterval);

        // Sync timer — primary pushes state to standby
        if (_config.StateSyncIntervalMs > 0)
        {
            _syncTimer = new Timer(_ => _ = SyncStateAsync(), null, _syncInterval, _syncInterval);
        }

        // History gap-fill: on startup, check if we missed log entries while we were down
        _ = Task.Run(async () =>
        {
            // Wait a few seconds for the partner to be reachable
            await Task.Delay(TimeSpan.FromSeconds(5));
            await FillHistoryGapAsync();
        });
    }

    // ─── Heartbeat ───────────────────────────────────────────────

    private async Task CheckPartnerAsync()
    {
        try
        {
            var url = $"{_config.PartnerEndpoint.TrimEnd('/')}/redundancy/heartbeat";
            var response = await _http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var status = JsonSerializer.Deserialize<HeartbeatResponse>(json, _jsonOptions);

                _missedHeartbeats = 0;
                PartnerAlive = true;
                LastPartnerHeartbeat = DateTime.UtcNow;

                // Auto switch-back: if we're a standby that promoted, and the original primary is back
                if (_config.AutoSwitchback
                    && _configuredRole == "Standby"
                    && ActiveRole == RedundancyRole.Active
                    && status?.Role == "Active")
                {
                    // The original primary recovered and is also active — we should step down
                    Log.Information("[Redundancy] Original primary recovered. Switching back to Standby.");
                    _eventLogger?.LogSystem("Info", "Redundancy", "Auto switch-back: returning to Standby role");
                    SetRole(RedundancyRole.Standby);
                }
                else if (_configuredRole == "Standby"
                         && ActiveRole == RedundancyRole.Active
                         && _config.AutoSwitchback
                         && status?.ConfiguredRole == "Primary")
                {
                    // Primary is back online. Step down.
                    Log.Information("[Redundancy] Primary is alive again. Stepping down to Standby.");
                    _eventLogger?.LogSystem("Info", "Redundancy", "Primary recovered — stepping down to Standby");
                    SetRole(RedundancyRole.Standby);
                }
            }
            else
            {
                OnHeartbeatMissed();
            }
        }
        catch
        {
            OnHeartbeatMissed();
        }
    }

    private void OnHeartbeatMissed()
    {
        _missedHeartbeats++;
        if (_missedHeartbeats >= _failoverMissedBeats && PartnerAlive)
        {
            PartnerAlive = false;
            Log.Warning("[Redundancy] Partner unreachable after {Missed} missed heartbeats.", _missedHeartbeats);
            _eventLogger?.LogSystem("Warning", "Redundancy", $"Partner unreachable ({_missedHeartbeats} missed heartbeats)");

            // If we're standby and the primary is gone, promote ourselves
            if (ActiveRole == RedundancyRole.Standby)
            {
                Log.Warning("[Redundancy] FAILOVER: Promoting Standby to Active.");
                _eventLogger?.LogSystem("Warning", "Redundancy", "FAILOVER: Promoting to Active role");
                SetRole(RedundancyRole.Active);
            }
        }
    }

    private void SetRole(RedundancyRole newRole)
    {
        if (ActiveRole == newRole) return;
        var oldRole = ActiveRole;
        ActiveRole = newRole;
        Log.Information("[Redundancy] Role changed: {Old} → {New}", oldRole, newRole);
        _eventLogger?.LogSystem("Info", "Redundancy", $"Role changed: {oldRole} → {newRole}");
        RoleChanged?.Invoke(newRole);
    }

    // ─── State Sync (Primary → Standby) ─────────────────────────

    /// <summary>Primary pushes variable values and queued log entries to standby.</summary>
    private async Task SyncStateAsync()
    {
        if (!IsActive || !PartnerAlive) return;

        try
        {
            // 1. Variable snapshot
            var snapshot = new StateSyncPayload
            {
                Timestamp = DateTime.UtcNow,
                Variables = _variables
                    .Where(kv => kv.Value.Value != null)
                    .Select(kv => new VariableSnapshot
                    {
                        Path = kv.Key,
                        Value = kv.Value.Value?.ToString() ?? "",
                        StatusCode = kv.Value.StatusCode.Code,
                        Timestamp = kv.Value.Timestamp
                    })
                    .ToList()
            };

            // 2. Drain replication queue (data-logging entries)
            var logBatch = new List<LogEntry>();
            while (logBatch.Count < 5000 && _replicationQueue.TryDequeue(out var entry))
            {
                logBatch.Add(entry);
            }
            snapshot.LogEntries = logBatch;

            // 3. Drain event replication queue
            var eventBatch = new List<EventEntry>();
            while (eventBatch.Count < 2000 && _eventReplicationQueue.TryDequeue(out var evt))
            {
                eventBatch.Add(evt);
            }
            snapshot.EventEntries = eventBatch;

            var json = JsonSerializer.Serialize(snapshot, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{_config.PartnerEndpoint.TrimEnd('/')}/redundancy/sync";
            var response = await _http.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                Log.Debug("[Redundancy] State sync failed: {Status}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Log.Debug("[Redundancy] State sync error: {Error}", ex.Message);
        }
    }

    /// <summary>
    /// Enqueue a data-logging entry for replication to the standby.
    /// Called by the logging pipeline on the active server.
    /// </summary>
    public void EnqueueLogReplication(string variableName, DateTime time, double? numericValue, string? stringValue, uint quality)
    {
        if (!IsActive || _replicationQueue.Count >= MaxReplicationQueueSize) return;
        _replicationQueue.Enqueue(new LogEntry
        {
            VariableName = variableName,
            Time = time,
            NumericValue = numericValue,
            StringValue = stringValue,
            Quality = quality
        });
    }

    /// <summary>
    /// Enqueue an event log entry for replication to the standby.
    /// Called by the event logger on the active server.
    /// </summary>
    public void EnqueueEventReplication(string category, string severity, string source, string message, string? details)
    {
        if (!IsActive || _eventReplicationQueue.Count >= MaxEventReplicationQueueSize) return;
        _eventReplicationQueue.Enqueue(new EventEntry
        {
            Time = DateTime.UtcNow,
            Category = category,
            Severity = severity,
            Source = source,
            Message = message,
            Details = details
        });
    }

    // ─── Inbound HTTP handler (called by DiagnosticsCollector) ───

    /// <summary>
    /// Handle incoming /redundancy/* requests from the partner.
    /// Returns JSON response body, or null if the path is not handled.
    /// </summary>
    public string? HandleRequest(string method, string path, string body)
    {
        if (path.Equals("/redundancy/heartbeat", StringComparison.OrdinalIgnoreCase) && method == "GET")
        {
            var resp = new HeartbeatResponse
            {
                Role = ActiveRole.ToString(),
                ConfiguredRole = _configuredRole,
                Timestamp = DateTime.UtcNow,
                PartnerAlive = PartnerAlive,
                LatestLogTimestamp = _logger?.GetLatestTimestamp()
            };
            return JsonSerializer.Serialize(resp, _jsonOptions);
        }

        if (path.Equals("/redundancy/sync", StringComparison.OrdinalIgnoreCase) && method == "POST")
        {
            return HandleSyncReceive(body);
        }

        if (path.StartsWith("/redundancy/history-gap", StringComparison.OrdinalIgnoreCase) && method == "GET")
        {
            return HandleHistoryGapRequest(path.Contains('?') ? path[(path.IndexOf('?') + 1)..] : "");
        }

        if (path.StartsWith("/redundancy/event-gap", StringComparison.OrdinalIgnoreCase) && method == "GET")
        {
            return HandleEventGapRequest(path.Contains('?') ? path[(path.IndexOf('?') + 1)..] : "");
        }

        return null;
    }

    private string HandleSyncReceive(string body)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<StateSyncPayload>(body, _jsonOptions);
            if (payload == null)
                return "{\"ok\":false,\"error\":\"Invalid payload\"}";

            // Apply variable values (standby keeps variables in sync)
            if (payload.Variables != null)
            {
                foreach (var snap in payload.Variables)
                {
                    _pendingSnapshots[snap.Path] = snap;
                }
                ApplyPendingSnapshots();
            }

            // Replay log entries into local logger
            if (payload.LogEntries is { Count: > 0 } && _logger != null)
            {
                ReplayLogEntries(payload.LogEntries);
            }

            // Replay event entries into local event logger
            if (payload.EventEntries is { Count: > 0 } && _eventLogger != null)
            {
                _eventLogger.ReplayEvents(payload.EventEntries);
            }

            var ack = new SyncAck
            {
                Ok = true,
                VariablesApplied = payload.Variables?.Count ?? 0,
                LogEntriesApplied = (payload.LogEntries?.Count ?? 0) + (payload.EventEntries?.Count ?? 0),
                Timestamp = DateTime.UtcNow
            };
            return JsonSerializer.Serialize(ack, _jsonOptions);
        }
        catch (Exception ex)
        {
            Log.Warning("[Redundancy] Sync receive error: {Error}", ex.Message);
            return $"{{\"ok\":false,\"error\":\"{ex.Message}\"}}";
        }
    }

    /// <summary>Apply received variable snapshots to local OPC variables.</summary>
    private void ApplyPendingSnapshots()
    {
        foreach (var (path, snap) in _pendingSnapshots)
        {
            if (!_variables.TryGetValue(path, out var variable)) continue;

            try
            {
                // Best-effort type conversion
                object? value = snap.Value;
                if (variable.DataType == DataTypeIds.Boolean && bool.TryParse(snap.Value, out var b)) value = b;
                else if (variable.DataType == DataTypeIds.Int32 && int.TryParse(snap.Value, out var i32)) value = i32;
                else if (variable.DataType == DataTypeIds.Int64 && long.TryParse(snap.Value, out var i64)) value = i64;
                else if (variable.DataType == DataTypeIds.Float && float.TryParse(snap.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var f)) value = f;
                else if (variable.DataType == DataTypeIds.Double && double.TryParse(snap.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var d)) value = d;
                else if (variable.DataType == DataTypeIds.UInt32 && uint.TryParse(snap.Value, out var u32)) value = u32;
                else if (variable.DataType == DataTypeIds.UInt16 && ushort.TryParse(snap.Value, out var u16)) value = u16;
                else if (variable.DataType == DataTypeIds.Int16 && short.TryParse(snap.Value, out var i16)) value = i16;

                variable.Value = value;
                variable.StatusCode = new StatusCode(snap.StatusCode);
                variable.Timestamp = snap.Timestamp;
            }
            catch { /* skip individual variable errors */ }
        }
        _pendingSnapshots.Clear();
    }

    /// <summary>Replay data-logging entries received from the primary into the local DB.</summary>
    private void ReplayLogEntries(List<LogEntry> entries)
    {
        if (_logger == null) return;

        // We use the logger's raw SQL path since IVariableLogger.Log() expects a BaseDataVariableState
        // For SQLite, we can use a direct insert approach
        if (_logger is SqliteLogger sqliteLogger)
        {
            sqliteLogger.ReplayEntries(entries);
        }
        else if (_logger is TimescaleLogger timescaleLogger)
        {
            timescaleLogger.ReplayEntries(entries);
        }
    }

    // ─── History Gap-Fill (startup catch-up) ─────────────────────

    /// <summary>
    /// On startup, compare our latest log timestamp with the partner's.
    /// If the partner has newer entries, request them and replay locally.
    /// This covers the period when this server was down/restarting.
    /// </summary>
    private async Task FillHistoryGapAsync()
    {
        try
        {
            // 1. Data-logging gap-fill
            if (_logger != null)
            {
                var localLatest = _logger.GetLatestTimestamp();
                if (localLatest != null)
                {
                    await FillGapForEndpoint(
                        "history-gap",
                        localLatest.Value,
                        resp => {
                            if (resp?.Entries is { Count: > 0 })
                            {
                                ReplayLogEntries(resp.Entries);
                                Log.Information("[Redundancy] Data-log gap-fill: {Count} entries replayed", resp.Entries.Count);
                                _eventLogger?.LogSystem("Info", "Redundancy", $"Data-log gap-fill: {resp.Entries.Count} entries replayed");
                            }
                        });
                }
                else
                {
                    Log.Information("[Redundancy] No local log history — skipping data-log gap-fill (first start).");
                }
            }

            // 2. Event-log gap-fill
            if (_eventLogger != null)
            {
                var localLatestEvent = _eventLogger.GetLatestEventTimestamp();
                if (localLatestEvent != null)
                {
                    await FillGapForEndpoint(
                        "event-gap",
                        localLatestEvent.Value,
                        resp => {
                            if (resp?.EventEntries is { Count: > 0 })
                            {
                                _eventLogger.ReplayEvents(resp.EventEntries);
                                Log.Information("[Redundancy] Event-log gap-fill: {Count} events replayed", resp.EventEntries.Count);
                                _eventLogger.LogSystem("Info", "Redundancy", $"Event-log gap-fill: {resp.EventEntries.Count} events replayed");
                            }
                        });
                }
                else
                {
                    Log.Information("[Redundancy] No local event history — skipping event-log gap-fill (first start).");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warning("[Redundancy] Gap-fill error: {Error}", ex.Message);
        }
    }

    private async Task FillGapForEndpoint(string endpoint, DateTime sinceUtc, Action<HistoryGapResponse?> applyFn)
    {
        var sinceIso = sinceUtc.ToString("o");
        var url = $"{_config.PartnerEndpoint.TrimEnd('/')}/redundancy/{endpoint}?since={Uri.EscapeDataString(sinceIso)}";

        Log.Information("[Redundancy] Requesting {Endpoint} gap-fill since {Since}...", endpoint, sinceIso);

        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            Log.Debug("[Redundancy] {Endpoint} gap-fill request failed: {Status}", endpoint, response.StatusCode);
            return;
        }

        var json = await response.Content.ReadAsStringAsync();
        var gapResponse = JsonSerializer.Deserialize<HistoryGapResponse>(json, _jsonOptions);
        applyFn(gapResponse);
    }

    /// <summary>Handle GET /redundancy/history-gap?since=ISO8601 from the partner.</summary>
    private string HandleHistoryGapRequest(string queryString)
    {
        try
        {
            // Parse "since" from query string
            var sinceParam = "";
            if (queryString.Contains("since=", StringComparison.OrdinalIgnoreCase))
            {
                var idx = queryString.IndexOf("since=", StringComparison.OrdinalIgnoreCase) + 6;
                var end = queryString.IndexOf('&', idx);
                sinceParam = end > 0 ? queryString[idx..end] : queryString[idx..];
                sinceParam = Uri.UnescapeDataString(sinceParam);
            }

            if (!DateTime.TryParse(sinceParam, null, System.Globalization.DateTimeStyles.RoundtripKind, out var sinceUtc))
            {
                return "{\"error\":\"Invalid or missing 'since' parameter\"}";
            }

            if (_logger == null)
            {
                return JsonSerializer.Serialize(new HistoryGapResponse
                {
                    PartnerLatestTimestamp = null,
                    Entries = new()
                }, _jsonOptions);
            }

            var entries = _logger.ReadEntriesSince(sinceUtc);
            var latestTs = _logger.GetLatestTimestamp();

            var resp = new HistoryGapResponse
            {
                PartnerLatestTimestamp = latestTs,
                Entries = entries
            };

            Log.Information("[Redundancy] Serving history gap-fill: {Count} entries since {Since}", entries.Count, sinceParam);
            return JsonSerializer.Serialize(resp, _jsonOptions);
        }
        catch (Exception ex)
        {
            Log.Warning("[Redundancy] history-gap handler error: {Error}", ex.Message);
            return $"{{\"error\":\"{ex.Message}\"}}";
        }
    }

    /// <summary>Handle GET /redundancy/event-gap?since=ISO8601 from the partner.</summary>
    private string HandleEventGapRequest(string queryString)
    {
        try
        {
            var sinceParam = "";
            if (queryString.Contains("since=", StringComparison.OrdinalIgnoreCase))
            {
                var idx = queryString.IndexOf("since=", StringComparison.OrdinalIgnoreCase) + 6;
                var end = queryString.IndexOf('&', idx);
                sinceParam = end > 0 ? queryString[idx..end] : queryString[idx..];
                sinceParam = Uri.UnescapeDataString(sinceParam);
            }

            if (!DateTime.TryParse(sinceParam, null, System.Globalization.DateTimeStyles.RoundtripKind, out var sinceUtc))
            {
                return "{\"error\":\"Invalid or missing 'since' parameter\"}";
            }

            if (_eventLogger == null)
            {
                return JsonSerializer.Serialize(new HistoryGapResponse
                {
                    PartnerLatestEventTimestamp = null,
                    EventEntries = new()
                }, _jsonOptions);
            }

            var events = _eventLogger.ReadEventsSince(sinceUtc);
            var latestTs = _eventLogger.GetLatestEventTimestamp();

            var resp = new HistoryGapResponse
            {
                PartnerLatestEventTimestamp = latestTs,
                EventEntries = events
            };

            Log.Information("[Redundancy] Serving event gap-fill: {Count} events since {Since}", events.Count, sinceParam);
            return JsonSerializer.Serialize(resp, _jsonOptions);
        }
        catch (Exception ex)
        {
            Log.Warning("[Redundancy] event-gap handler error: {Error}", ex.Message);
            return $"{{\"error\":\"{ex.Message}\"}}";
        }
    }

    // ─── Diagnostics ─────────────────────────────────────────────

    public RedundancyDiagnostics GetDiagnostics() => new()
    {
        ConfiguredRole = _configuredRole,
        ActiveRole = ActiveRole.ToString(),
        IsActive = IsActive,
        PartnerEndpoint = _config.PartnerEndpoint,
        PartnerAlive = PartnerAlive,
        LastPartnerHeartbeat = LastPartnerHeartbeat,
        MissedHeartbeats = _missedHeartbeats,
        ReplicationQueueSize = _replicationQueue.Count
    };

    // ─── Cleanup ─────────────────────────────────────────────────

    public void Dispose()
    {
        _heartbeatTimer?.Dispose();
        _syncTimer?.Dispose();
        _http.Dispose();
    }

    // ─── JSON options ────────────────────────────────────────────

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    // ─── DTOs ────────────────────────────────────────────────────

    public class HeartbeatResponse
    {
        public string Role { get; set; } = "";
        public string ConfiguredRole { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public bool PartnerAlive { get; set; }
        public DateTime? LatestLogTimestamp { get; set; }
    }

    public class StateSyncPayload
    {
        public DateTime Timestamp { get; set; }
        public List<VariableSnapshot>? Variables { get; set; }
        public List<LogEntry>? LogEntries { get; set; }
        public List<EventEntry>? EventEntries { get; set; }
    }

    public class VariableSnapshot
    {
        public string Path { get; set; } = "";
        public string Value { get; set; } = "";
        public uint StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class LogEntry
    {
        public string VariableName { get; set; } = "";
        public DateTime Time { get; set; }
        public double? NumericValue { get; set; }
        public string? StringValue { get; set; }
        public uint Quality { get; set; }
    }

    public class SyncAck
    {
        public bool Ok { get; set; }
        public int VariablesApplied { get; set; }
        public int LogEntriesApplied { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class HistoryGapResponse
    {
        public DateTime? PartnerLatestTimestamp { get; set; }
        public List<LogEntry> Entries { get; set; } = new();
        public DateTime? PartnerLatestEventTimestamp { get; set; }
        public List<EventEntry> EventEntries { get; set; } = new();
    }

    public class EventEntry
    {
        public DateTime Time { get; set; }
        public string Category { get; set; } = "";
        public string Severity { get; set; } = "";
        public string Source { get; set; } = "";
        public string Message { get; set; } = "";
        public string? Details { get; set; }
    }

    public class RedundancyDiagnostics
    {
        public string ConfiguredRole { get; set; } = "";
        public string ActiveRole { get; set; } = "";
        public bool IsActive { get; set; }
        public string PartnerEndpoint { get; set; } = "";
        public bool PartnerAlive { get; set; }
        public DateTime LastPartnerHeartbeat { get; set; }
        public int MissedHeartbeats { get; set; }
        public int ReplicationQueueSize { get; set; }
    }
}

public enum RedundancyRole
{
    Active,
    Standby
}
