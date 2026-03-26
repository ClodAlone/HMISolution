using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Manages server redundancy via heartbeat monitoring between primary and standby instances.
/// Features:
/// - Heartbeat-based partner monitoring with automatic failover
/// - Variable state synchronization from primary to standby
/// - Database replication health checking
/// - Failover/switchback event callbacks for node manager activation
/// - Client reconnect support via redundant server URL list
/// </summary>
public sealed class RedundancyManager : IDisposable
{
    private readonly RedundancyConfig _config;
    private readonly EventLogger? _eventLogger;
    private readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(5) };
    private Timer? _heartbeatTimer;
    private Timer? _stateSyncTimer;
    private Timer? _dbReplicationTimer;
    private int _missedHeartbeats;
    private bool _partnerAlive = true;
    private bool _isActiveServer;
    private DateTime _lastPartnerSeen = DateTime.UtcNow;
    private readonly object _lock = new();

    // State sync: variable snapshots pushed from primary to standby
    private readonly ConcurrentDictionary<string, VariableStateEntry> _receivedState = new();

    /// <summary>Whether this server is the currently active (serving) instance.</summary>
    public bool IsActiveServer => _isActiveServer;

    /// <summary>Whether the partner server is reachable.</summary>
    public bool PartnerAlive => _partnerAlive;

    /// <summary>Last time the partner was successfully contacted.</summary>
    public DateTime LastPartnerSeen => _lastPartnerSeen;

    /// <summary>Current role description.</summary>
    public string CurrentStatus { get; private set; }

    /// <summary>Current database replication status.</summary>
    public string ReplicationStatus { get; private set; } = "Unknown";

    /// <summary>Redundant OPC UA server URLs for client reconnection on failover.</summary>
    public IReadOnlyList<string> ServerUrls => _config.ServerUrls;

    /// <summary>Raised when this server transitions to active.</summary>
    public event Action? OnBecameActive;

    /// <summary>Raised when this server transitions to standby.</summary>
    public event Action? OnBecameStandby;

    /// <summary>Raised when failover occurs.</summary>
    public event Action<string>? OnFailover;

    /// <summary>Raised when switchback occurs.</summary>
    public event Action<string>? OnSwitchback;

    /// <summary>Raised when database replication health changes.</summary>
    public event Action<string, bool>? OnReplicationStatusChanged;

    public RedundancyManager(RedundancyConfig config, EventLogger? eventLogger)
    {
        _config = config;
        _eventLogger = eventLogger;
        _isActiveServer = config.Role.Equals("Primary", StringComparison.OrdinalIgnoreCase);
        CurrentStatus = _isActiveServer ? "Primary (Active)" : "Standby (Waiting)";
        DiagnosticsCollector.Instance.Register("Redundancy", config.Role, status: CurrentStatus);
    }

    public void Start()
    {
        if (!_config.Enabled || string.IsNullOrEmpty(_config.PartnerEndpoint)) return;

        var interval = TimeSpan.FromSeconds(Math.Max(1, _config.HeartbeatIntervalSeconds));
        _heartbeatTimer = new Timer(CheckPartner, null, interval, interval);

        if (_config.StateSyncIntervalMs > 0)
        {
            var syncInterval = TimeSpan.FromMilliseconds(_config.StateSyncIntervalMs);
            _stateSyncTimer = new Timer(StateSyncTick, null, syncInterval, syncInterval);
        }

        if (!string.IsNullOrEmpty(_config.PartnerDatabaseConnectionString))
        {
            _dbReplicationTimer = new Timer(CheckDatabaseReplication, null,
                TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30));
        }

        Log.Information("Redundancy manager started: Role={Role}, Partner={Partner}, Interval={Interval}s, Failover after {Missed} missed heartbeats, StateSync={SyncMs}ms",
            _config.Role, _config.PartnerEndpoint, _config.HeartbeatIntervalSeconds,
            _config.FailoverMissedHeartbeats, _config.StateSyncIntervalMs);

        _eventLogger?.LogSystem("Info", "Redundancy",
            $"Redundancy manager started — Role: {_config.Role}, Partner: {_config.PartnerEndpoint}");

        if (_isActiveServer) OnBecameActive?.Invoke();
        else OnBecameStandby?.Invoke();
    }

    private async void CheckPartner(object? state)
    {
        try
        {
            var response = await _httpClient.GetAsync(_config.PartnerEndpoint);
            if (response.IsSuccessStatusCode)
            {
                _missedHeartbeats = 0;
                _lastPartnerSeen = DateTime.UtcNow;

                if (!_partnerAlive)
                {
                    _partnerAlive = true;
                    Log.Information("Redundancy: Partner server at {Endpoint} is back online", _config.PartnerEndpoint);
                    _eventLogger?.LogSystem("Info", "Redundancy", $"Partner server recovered at {_config.PartnerEndpoint}");
                }

                if (_config.Role.Equals("Standby", StringComparison.OrdinalIgnoreCase))
                {
                    if (_isActiveServer)
                    {
                        if (_config.AutoSwitchback)
                        {
                            Log.Information("Redundancy: Primary recovered — auto-switching back to standby");
                            _eventLogger?.LogSystem("Info", "Redundancy", "SWITCHBACK: Returning to standby — primary server recovered");
                            _isActiveServer = false;
                            CurrentStatus = "Standby (Switchback)";
                            OnBecameStandby?.Invoke();
                            OnSwitchback?.Invoke("Primary recovered — automatic switchback");
                        }
                        else
                        {
                            CurrentStatus = "Active (Primary Recovered — manual switchback)";
                            Log.Information("Redundancy: Primary server recovered. Standby remains active (manual switchback required).");
                        }
                    }
                    else { CurrentStatus = "Standby (Partner Active)"; }
                }
                else { CurrentStatus = "Primary (Active)"; }
            }
            else { HandleMissedHeartbeat(); }
        }
        catch { HandleMissedHeartbeat(); }

        DiagnosticsCollector.Instance.SetStatus("Redundancy", _config.Role, CurrentStatus,
            _partnerAlive ? null : $"Partner unreachable — {_missedHeartbeats} missed heartbeats");
    }

    private void HandleMissedHeartbeat()
    {
        _missedHeartbeats++;
        if (_partnerAlive && _missedHeartbeats >= _config.FailoverMissedHeartbeats)
        {
            _partnerAlive = false;
            Log.Warning("Redundancy: Partner server at {Endpoint} unreachable after {Missed} missed heartbeats", _config.PartnerEndpoint, _missedHeartbeats);
            _eventLogger?.LogSystem("Warning", "Redundancy", $"Partner server unreachable after {_missedHeartbeats} missed heartbeats at {_config.PartnerEndpoint}");

            if (_config.Role.Equals("Standby", StringComparison.OrdinalIgnoreCase) && !_isActiveServer)
            {
                _isActiveServer = true;
                CurrentStatus = "Active (Failover)";
                var reason = $"FAILOVER: Standby promoted to active — primary unreachable after {_missedHeartbeats} missed heartbeats";
                Log.Warning("REDUNDANCY FAILOVER: Standby server promoting to ACTIVE");
                _eventLogger?.LogSystem("Critical", "Redundancy", reason);
                OnBecameActive?.Invoke();
                OnFailover?.Invoke(reason);
            }
            else { CurrentStatus = "Primary (Partner Down)"; }
        }
    }

    // ─── State synchronization ──────────────────────────────────────

    /// <summary>Delegate set by the node manager to provide the current variable state snapshot.</summary>
    public Func<Dictionary<string, string>>? GetStateSnapshot { get; set; }

    /// <summary>Delegate set by the node manager to apply a received state snapshot from the partner.</summary>
    public Action<Dictionary<string, string>>? ApplyStateSnapshot { get; set; }

    private async void StateSyncTick(object? state)
    {
        if (!_isActiveServer || !_partnerAlive) return;
        if (GetStateSnapshot == null) return;
        try
        {
            var snapshot = GetStateSnapshot();
            if (snapshot.Count == 0) return;
            var json = JsonSerializer.Serialize(snapshot);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var syncUrl = _config.PartnerEndpoint.TrimEnd('/') + "/state-sync";
            var response = await _httpClient.PostAsync(syncUrl, content);
            if (!response.IsSuccessStatusCode)
                Log.Debug("Redundancy: State sync to partner failed — HTTP {Status}", response.StatusCode);
        }
        catch (Exception ex) { Log.Debug("Redundancy: State sync error — {Error}", ex.Message); }
    }

    /// <summary>Called when this server receives a state sync push from the partner.</summary>
    public void ReceiveStateSync(Dictionary<string, string> stateSnapshot)
    {
        if (_isActiveServer) return;
        foreach (var kv in stateSnapshot)
            _receivedState[kv.Key] = new VariableStateEntry { Value = kv.Value, ReceivedAt = DateTime.UtcNow };
        ApplyStateSnapshot?.Invoke(stateSnapshot);
        Log.Debug("Redundancy: Received state sync with {Count} variables", stateSnapshot.Count);
    }

    /// <summary>Gets the last received state for a variable path.</summary>
    public string? GetLastReceivedValue(string variablePath)
    {
        return _receivedState.TryGetValue(variablePath, out var entry) ? entry.Value : null;
    }

    // ─── Database replication health check ───────────────────────────

    private async void CheckDatabaseReplication(object? state)
    {
        if (string.IsNullOrEmpty(_config.PartnerDatabaseConnectionString)) return;
        bool healthy; string status;
        try
        {
            var response = await _httpClient.GetAsync(_config.PartnerEndpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                healthy = true; status = "Healthy";
                if (root.TryGetProperty("Subsystems", out var subsystems))
                {
                    foreach (var sub in subsystems.EnumerateArray())
                    {
                        if (sub.TryGetProperty("Category", out var cat) && cat.GetString() == "DataLogger")
                        {
                            var subStatus = sub.TryGetProperty("Status", out var s) ? s.GetString() : "Unknown";
                            var lastError = sub.TryGetProperty("LastError", out var e) ? e.GetString() : null;
                            if (subStatus == "Error") { healthy = false; status = $"Partner DataLogger error: {lastError}"; }
                            break;
                        }
                    }
                }
            }
            else { healthy = false; status = "Partner unreachable"; }
        }
        catch (Exception ex) { healthy = false; status = $"Check failed: {ex.Message}"; }

        var previousStatus = ReplicationStatus;
        ReplicationStatus = status;
        if (previousStatus != status)
        {
            if (healthy) Log.Information("Redundancy: Database replication status — {Status}", status);
            else
            {
                Log.Warning("Redundancy: Database replication issue — {Status}", status);
                _eventLogger?.LogSystem("Warning", "Redundancy", $"Database replication: {status}");
            }
            OnReplicationStatusChanged?.Invoke(status, healthy);
        }
        DiagnosticsCollector.Instance.SetStatus("Redundancy", _config.Role, CurrentStatus, healthy ? null : $"DB Replication: {status}");
    }

    // ─── Manual operations ──────────────────────────────────────────

    /// <summary>Manually force this server to become active.</summary>
    public bool ForceActive(string reason)
    {
        lock (_lock)
        {
            if (_isActiveServer) return false;
            _isActiveServer = true;
            CurrentStatus = "Active (Manual)";
            Log.Warning("Redundancy: Manually forced to active — {Reason}", reason);
            _eventLogger?.LogSystem("Warning", "Redundancy", $"Manual promotion to active: {reason}");
            OnBecameActive?.Invoke();
            return true;
        }
    }

    /// <summary>Manually force this server to standby.</summary>
    public bool ForceStandby(string reason)
    {
        lock (_lock)
        {
            if (!_isActiveServer) return false;
            _isActiveServer = false;
            CurrentStatus = "Standby (Manual)";
            Log.Warning("Redundancy: Manually forced to standby — {Reason}", reason);
            _eventLogger?.LogSystem("Warning", "Redundancy", $"Manual demotion to standby: {reason}");
            OnBecameStandby?.Invoke();
            OnSwitchback?.Invoke($"Manual switchback: {reason}");
            return true;
        }
    }

    /// <summary>Gets the current redundancy status snapshot.</summary>
    public RedundancyStatus GetStatus()
    {
        return new RedundancyStatus
        {
            Role = _config.Role,
            IsActive = _isActiveServer,
            PartnerEndpoint = _config.PartnerEndpoint,
            PartnerAlive = _partnerAlive,
            LastPartnerSeen = _lastPartnerSeen,
            MissedHeartbeats = _missedHeartbeats,
            Status = CurrentStatus,
            ReplicationStatus = ReplicationStatus,
            ServerUrls = _config.ServerUrls,
            AutoSwitchback = _config.AutoSwitchback,
            StateSyncEnabled = _config.StateSyncIntervalMs > 0,
            SyncedVariableCount = _receivedState.Count
        };
    }

    public void Dispose()
    {
        _heartbeatTimer?.Dispose();
        _stateSyncTimer?.Dispose();
        _dbReplicationTimer?.Dispose();
        _httpClient.Dispose();
    }
}

public class RedundancyStatus
{
    public string Role { get; set; } = "";
    public bool IsActive { get; set; }
    public string PartnerEndpoint { get; set; } = "";
    public bool PartnerAlive { get; set; }
    public DateTime LastPartnerSeen { get; set; }
    public int MissedHeartbeats { get; set; }
    public string Status { get; set; } = "";
    public string ReplicationStatus { get; set; } = "Unknown";
    public List<string> ServerUrls { get; set; } = new();
    public bool AutoSwitchback { get; set; }
    public bool StateSyncEnabled { get; set; }
    public int SyncedVariableCount { get; set; }
}

/// <summary>Entry tracking a received variable state from the partner server.</summary>
internal class VariableStateEntry
{
    public string Value { get; set; } = "";
    public DateTime ReceivedAt { get; set; }
}
