using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR.Client;
using SharedModels;
using SharedModels.CloudRelay;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Aggregates live data from multiple CloudRelay-connected sites into a single
/// in-memory model used by the Multi-Site Dashboard component.
/// Each site gets its own SignalR connection; values and alarms are merged.
/// </summary>
public sealed class MultiSiteAggregator : IDisposable
{
    private readonly List<SiteConnection> _sites = new();
    private readonly object _lock = new();
    private System.Threading.Timer? _refreshTimer;

    /// <summary>Fires when any site's data changes (connection, values, alarms).</summary>
    public event Action? StateChanged;

    /// <summary>Snapshot of all site states for the dashboard to render.</summary>
    public IReadOnlyList<SiteState> Sites
    {
        get { lock (_lock) { return _sites.Select(s => s.CurrentState).ToList(); } }
    }

    /// <summary>Total number of active (unacknowledged) alarms across all sites.</summary>
    public int TotalActiveAlarms => Sites.Sum(s => s.ActiveAlarmCount);

    public async Task InitializeAsync(MultiSiteDashboardConfig config)
    {
        Dispose();

        if (!config.Enabled || config.Sites.Count == 0) return;

        foreach (var site in config.Sites.Where(s => s.Enabled))
        {
            var conn = new SiteConnection(site, () => NotifyChanged());
            _sites.Add(conn);
            _ = conn.ConnectAsync(); // fire and forget — each site connects independently
        }

        // Periodic refresh for elapsed-time display and reconnection
        var interval = Math.Max(config.RefreshIntervalSeconds, 2) * 1000;
        _refreshTimer = new System.Threading.Timer(_ =>
        {
            foreach (var s in _sites)
            {
                if (!s.IsConnected)
                    _ = s.ConnectAsync();
            }
            NotifyChanged();
        }, null, interval, interval);

        await Task.CompletedTask;
    }

    /// <summary>Subscribe to specific KPI variable paths on a site.</summary>
    public async Task SubscribeKpis(string siteId)
    {
        SiteConnection? conn;
        lock (_lock) { conn = _sites.FirstOrDefault(s => s.Config.Id == siteId); }
        if (conn != null)
            await conn.SubscribeKpis();
    }

    /// <summary>Subscribe all sites' KPIs.</summary>
    public async Task SubscribeAll()
    {
        List<SiteConnection> snapshot;
        lock (_lock) { snapshot = _sites.ToList(); }
        foreach (var s in snapshot)
            await s.SubscribeKpis();
    }

    private void NotifyChanged() => StateChanged?.Invoke();

    public void Dispose()
    {
        _refreshTimer?.Dispose();
        _refreshTimer = null;
        lock (_lock)
        {
            foreach (var s in _sites)
                s.Dispose();
            _sites.Clear();
        }
    }

    // ─────────────────────────────────────────────────────────────

    /// <summary>Read-only snapshot of a single site's live state.</summary>
    public class SiteState
    {
        public string Id { get; init; } = "";
        public string Name { get; init; } = "";
        public bool IsConnected { get; init; }
        public bool BridgeOnline { get; init; }
        public string Status { get; init; } = "Disconnected";
        public int ActiveAlarmCount { get; init; }
        public List<SiteKpiValue> Kpis { get; init; } = new();
        public DateTime LastUpdate { get; init; }
        public string? ErrorMessage { get; init; }
    }

    /// <summary>A single KPI's current value for display.</summary>
    public class SiteKpiValue
    {
        public string Label { get; init; } = "";
        public string Value { get; init; } = "";
        public string Unit { get; init; } = "";
        public string Level { get; init; } = "ok"; // "ok", "warn", "alarm"
    }

    // ─────────────────────────────────────────────────────────────

    private sealed class SiteConnection : IDisposable
    {
        public SiteConfig Config { get; }
        public bool IsConnected => _hub?.State == HubConnectionState.Connected;

        private HubConnection? _hub;
        private readonly ConcurrentDictionary<string, string> _values = new();
        private readonly Action _onChanged;
        private int _alarmCount;
        private bool _bridgeOnline;
        private string? _error;
        private DateTime _lastUpdate;
        private bool _subscribed;

        public SiteConnection(SiteConfig config, Action onChanged)
        {
            Config = config;
            _onChanged = onChanged;
        }

        public SiteState CurrentState => new()
        {
            Id = Config.Id,
            Name = Config.Name,
            IsConnected = IsConnected,
            BridgeOnline = _bridgeOnline,
            Status = !IsConnected ? "Disconnected"
                : !_bridgeOnline ? "Hub connected, bridge offline"
                : "Online",
            ActiveAlarmCount = _alarmCount,
            Kpis = Config.Kpis.Select(k =>
            {
                var raw = _values.TryGetValue(k.VariablePath, out var v) ? v : "";
                var level = "ok";
                if (double.TryParse(raw, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out var d))
                {
                    if (k.AlarmThreshold.HasValue && d >= k.AlarmThreshold.Value) level = "alarm";
                    else if (k.WarningThreshold.HasValue && d >= k.WarningThreshold.Value) level = "warn";
                }
                return new SiteKpiValue
                {
                    Label = k.Label,
                    Value = string.IsNullOrEmpty(raw) ? "–" : raw,
                    Unit = k.Unit,
                    Level = level
                };
            }).ToList(),
            LastUpdate = _lastUpdate,
            ErrorMessage = _error
        };

        public async Task ConnectAsync()
        {
            if (IsConnected) return;
            try
            {
                _error = null;
                var url = string.IsNullOrEmpty(Config.ApiKey)
                    ? Config.HubUrl
                    : $"{Config.HubUrl}?apiKey={Uri.EscapeDataString(Config.ApiKey)}";

                _hub = new HubConnectionBuilder()
                    .WithUrl(url)
                    .WithAutomaticReconnect()
                    .Build();

                _hub.On<List<TagValueDto>>(HubMethods.ValuesUpdated, values =>
                {
                    foreach (var v in values)
                        _values[v.Path] = v.Value;
                    _lastUpdate = DateTime.UtcNow;
                    _onChanged();
                });

                _hub.On<List<AlarmEntryDto>>(HubMethods.AlarmsUpdated, alarms =>
                {
                    _alarmCount = alarms.Count(a => !a.IsAcked);
                    _lastUpdate = DateTime.UtcNow;
                    _onChanged();
                });

                _hub.On<bool>(HubMethods.BridgeStatusChanged, online =>
                {
                    _bridgeOnline = online;
                    _onChanged();
                });

                _hub.Reconnected += async _ =>
                {
                    await _hub.SendAsync("JoinAsViewer");
                    _subscribed = false;
                    await SubscribeKpis();
                    _onChanged();
                };

                _hub.Closed += _ => { _onChanged(); return Task.CompletedTask; };

                await _hub.StartAsync();
                await _hub.SendAsync("JoinAsViewer");
                _onChanged();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                _onChanged();
            }
        }

        public async Task SubscribeKpis()
        {
            if (!IsConnected || _subscribed || Config.Kpis.Count == 0) return;
            try
            {
                var paths = Config.Kpis.Select(k => k.VariablePath).Where(p => !string.IsNullOrEmpty(p)).ToList();
                if (paths.Count > 0)
                {
                    await _hub!.SendAsync(HubMethods.Subscribe, paths, 2);
                    _subscribed = true;
                }
            }
            catch { /* site may be unreachable */ }
        }

        public void Dispose()
        {
            try { _hub?.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(2)); }
            catch { }
            _hub = null;
        }
    }
}
