using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Tracks equipment runtime hours, evaluates preventive maintenance schedules,
/// and creates OPC UA variables under _Assets/{Name}/ for live status.
///
/// Published variables per asset:
///   _Assets.{Name}.Running          (Boolean)  — current running state
///   _Assets.{Name}.RuntimeHours     (Double)   — accumulated runtime hours
///   _Assets.{Name}.Faulted          (Boolean)  — current fault state
///   _Assets.{Name}.ServiceDue       (Boolean)  — true when any schedule is due
///   _Assets.{Name}.NextServiceIn    (Double)   — hours until next service (min across schedules)
///   _Assets.{Name}.NextServiceName  (String)   — name of the nearest-due schedule
/// </summary>
public sealed class AssetManager : IDisposable
{
    private readonly SimpleFileServerNodeManager _nodeManager;
    private readonly List<AssetState> _assets = new();
    private Timer? _timer;
    private readonly CancellationTokenSource _cts = new();
    private DateTime _lastTick = DateTime.UtcNow;

    private const string Folder = "_Assets";
    private const int TickIntervalSeconds = 10;

    public AssetManager(SimpleFileServerNodeManager nodeManager)
    {
        _nodeManager = nodeManager;
    }

    public void Initialize(List<AssetConfig> configs)
    {
        foreach (var config in configs)
        {
            DiagnosticsCollector.Instance.Register("Asset", config.Name, config.Enabled);
            if (!config.Enabled) continue;

            _assets.Add(new AssetState(config));
        }

        if (_assets.Count > 0)
        {
            _lastTick = DateTime.UtcNow;
            _timer = new Timer(Tick, null,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(TickIntervalSeconds));
            Log.Information("AssetManager initialized with {Count} asset(s).", _assets.Count);
        }
    }

    private void Tick(object? state)
    {
        if (_cts.IsCancellationRequested) return;

        var now = DateTime.UtcNow;
        var elapsedHours = (now - _lastTick).TotalHours;
        _lastTick = now;

        foreach (var asset in _assets)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                EvaluateAsset(asset, elapsedHours, now);
                sw.Stop();
                DiagnosticsCollector.Instance.RecordCycle("Asset", asset.Config.Name, sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                DiagnosticsCollector.Instance.RecordCycle("Asset", asset.Config.Name, sw.Elapsed.TotalMilliseconds, error: ex.Message);
                Log.Error(ex, "AssetManager error for '{Name}': {Message}", asset.Config.Name, ex.Message);
            }
        }
    }

    private void EvaluateAsset(AssetState asset, double elapsedHours, DateTime now)
    {
        var prefix = $"{Folder}.{asset.Config.Name}";

        // --- Determine running state ---
        bool isRunning = false;
        if (!string.IsNullOrEmpty(asset.Config.RunningVariablePath))
        {
            var val = _nodeManager.ReadVariable(asset.Config.RunningVariablePath);
            isRunning = IsTruthy(val);
        }

        // --- Accumulate runtime ---
        if (isRunning)
        {
            asset.RuntimeHours += elapsedHours;
        }

        // --- Determine fault state ---
        bool isFaulted = false;
        if (!string.IsNullOrEmpty(asset.Config.FaultVariablePath))
        {
            var val = _nodeManager.ReadVariable(asset.Config.FaultVariablePath);
            isFaulted = IsTruthy(val);
        }

        // --- Evaluate maintenance schedules ---
        bool anyServiceDue = false;
        double minHoursRemaining = double.MaxValue;
        string nearestScheduleName = "";

        foreach (var sched in asset.Config.MaintenanceSchedules)
        {
            double hoursSinceReset = asset.RuntimeHours - sched.LastResetRuntimeHours;
            double hoursRemaining = sched.IntervalHours - hoursSinceReset;

            // Calendar-based check
            if (sched.IntervalDays > 0 && sched.LastResetUtc.HasValue)
            {
                double daysSinceReset = (now - sched.LastResetUtc.Value).TotalDays;
                double daysRemaining = sched.IntervalDays - daysSinceReset;
                // Convert remaining days to equivalent hours for comparison
                double calendarHoursRemaining = daysRemaining * 24.0;
                if (calendarHoursRemaining < hoursRemaining)
                    hoursRemaining = calendarHoursRemaining;
            }
            else if (sched.IntervalDays > 0 && !sched.LastResetUtc.HasValue)
            {
                // No reset date — treat as immediately due by calendar
                hoursRemaining = 0;
            }

            if (hoursRemaining <= 0)
            {
                anyServiceDue = true;

                // Raise alarm if not already active
                if (!asset.ActiveAlarms.Contains(sched.Id))
                {
                    asset.ActiveAlarms.Add(sched.Id);
                    _nodeManager.RaiseAssetAlarm(asset.Config.Name, sched.Name,
                        $"Service due: {sched.Name} on {asset.Config.Name} (interval: {sched.IntervalHours}h)",
                        sched.Severity, isWarning: false);
                }
            }
            else
            {
                // Check warning threshold
                double warningThreshold = sched.IntervalHours * (1.0 - sched.WarningPercent / 100.0);
                if (hoursRemaining <= warningThreshold && !asset.ActiveWarnings.Contains(sched.Id))
                {
                    asset.ActiveWarnings.Add(sched.Id);
                    _nodeManager.RaiseAssetAlarm(asset.Config.Name, sched.Name,
                        $"Service approaching: {sched.Name} on {asset.Config.Name} ({hoursRemaining:F0}h remaining)",
                        (ushort)Math.Max(1, sched.Severity / 2), isWarning: true);
                }

                // Clear service-due alarm if hours remaining went positive (after reset)
                if (asset.ActiveAlarms.Contains(sched.Id))
                {
                    asset.ActiveAlarms.Remove(sched.Id);
                    _nodeManager.ClearAssetAlarm(asset.Config.Name, sched.Name);
                }
            }

            if (hoursRemaining < minHoursRemaining)
            {
                minHoursRemaining = hoursRemaining;
                nearestScheduleName = sched.Name;
            }
        }

        // --- Publish OPC variables ---
        WriteVar($"{prefix}.Running", isRunning);
        WriteVar($"{prefix}.RuntimeHours", Math.Round(asset.RuntimeHours, 2));
        WriteVar($"{prefix}.Faulted", isFaulted);
        WriteVar($"{prefix}.ServiceDue", anyServiceDue);
        WriteVar($"{prefix}.NextServiceIn", minHoursRemaining == double.MaxValue ? 0.0 : Math.Round(minHoursRemaining, 1));
        WriteVar($"{prefix}.NextServiceName", nearestScheduleName);
    }

    /// <summary>Reset a maintenance schedule (called after service is performed).</summary>
    public void ResetSchedule(string assetName, string scheduleId)
    {
        var asset = _assets.FirstOrDefault(a =>
            a.Config.Name.Equals(assetName, StringComparison.OrdinalIgnoreCase));
        if (asset == null) return;

        var sched = asset.Config.MaintenanceSchedules.FirstOrDefault(s =>
            s.Id.Equals(scheduleId, StringComparison.OrdinalIgnoreCase));
        if (sched == null) return;

        sched.LastResetUtc = DateTime.UtcNow;
        sched.LastResetRuntimeHours = asset.RuntimeHours;
        asset.ActiveAlarms.Remove(sched.Id);
        asset.ActiveWarnings.Remove(sched.Id);
        _nodeManager.ClearAssetAlarm(assetName, sched.Name);

        Log.Information("Asset '{Asset}' schedule '{Schedule}' reset at {Hours:F1}h",
            assetName, sched.Name, asset.RuntimeHours);
    }

    private void WriteVar(string path, object value)
    {
        try { _nodeManager.WriteVariable(path, value); }
        catch { /* variable may not exist yet on first tick */ }
    }

    private static bool IsTruthy(object? value)
    {
        if (value == null) return false;
        var s = value.ToString();
        return s is "True" or "true" or "1";
    }

    public void Dispose()
    {
        _cts.Cancel();
        _timer?.Dispose();
    }

    private sealed class AssetState
    {
        public AssetConfig Config { get; }
        public double RuntimeHours { get; set; }
        public HashSet<string> ActiveAlarms { get; } = new();
        public HashSet<string> ActiveWarnings { get; } = new();

        public AssetState(AssetConfig config)
        {
            Config = config;
            RuntimeHours = config.InitialRuntimeHours;
        }
    }
}
