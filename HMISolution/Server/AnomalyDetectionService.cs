using Opc.Ua;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Periodically scans registered variables for statistical anomalies using
/// z-score analysis on recent historical data.
/// </summary>
public sealed class AnomalyDetectionService : IDisposable
{
    private readonly SimpleFileServerNodeManager _nodeManager;
    private readonly AnomalyDetectionSettings _settings;
    private readonly EventLogger? _eventLogger;
    private readonly NotificationManager? _notificationManager;
    private Timer? _timer;

    private readonly List<RegistrationEntry> _registrations = new();
    private readonly object _lock = new();

    private long _totalAnomaliesDetected;
    private long _totalPredictiveAlarms;

    public AnomalyDetectionService(
        SimpleFileServerNodeManager nodeManager,
        AnomalyDetectionSettings settings,
        EventLogger? eventLogger,
        NotificationManager? notificationManager)
    {
        _nodeManager = nodeManager;
        _settings = settings;
        _eventLogger = eventLogger;
        _notificationManager = notificationManager;
    }

    /// <summary>Register a variable for anomaly monitoring.</summary>
    public void Register(string variablePath, AnomalyDetectionConfig config, AlarmConfig? alarmConfig, IVariableLogger logger)
    {
        lock (_lock)
        {
            _registrations.Add(new RegistrationEntry
            {
                VariablePath = variablePath,
                Config = config,
                AlarmConfig = alarmConfig,
                Logger = logger
            });
        }
    }

    /// <summary>Start the periodic analysis timer.</summary>
    public void Start()
    {
        var interval = TimeSpan.FromSeconds(_settings.IntervalSeconds > 0 ? _settings.IntervalSeconds : 30);
        _timer = new Timer(Analyze, null, interval, interval);
        Log.Information("Anomaly detection started — interval {Interval}s, {Count} variables registered",
            _settings.IntervalSeconds, _registrations.Count);
    }

    private void Analyze(object? state)
    {
        List<RegistrationEntry> snapshot;
        lock (_lock)
        {
            snapshot = new List<RegistrationEntry>(_registrations);
        }

        foreach (var entry in snapshot)
        {
            try
            {
                var history = entry.Logger.ReadHistory(entry.VariablePath,
                    DateTime.UtcNow.AddMinutes(-30), DateTime.UtcNow);

                if (history == null || history.Count < _settings.MinSampleCount)
                    continue;

                var values = new List<double>();
                foreach (var dv in history)
                {
                    if (dv.Value != null)
                    {
                        try { values.Add(Convert.ToDouble(dv.Value)); }
                        catch { /* skip non-numeric */ }
                    }
                }

                if (values.Count < _settings.MinSampleCount) continue;

                double mean = values.Average();
                double stdDev = Math.Sqrt(values.Sum(v => (v - mean) * (v - mean)) / values.Count);

                if (stdDev < 1e-12) continue;

                double latest = values[^1];
                double zScore = Math.Abs((latest - mean) / stdDev);
                double threshold = entry.Config.Sensitivity > 0 ? entry.Config.Sensitivity : 3.0;

                if (zScore > threshold)
                {
                    Interlocked.Increment(ref _totalAnomaliesDetected);
                    _eventLogger?.LogAlarm("Warning", entry.VariablePath,
                        $"Anomaly detected: z-score={zScore:F2} (threshold={threshold:F2}), value={latest}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Anomaly detection error for {Variable}", entry.VariablePath);
            }
        }
    }

    /// <summary>Build a diagnostics snapshot.</summary>
    public AnomalyDetectionSnapshot GetSnapshot()
    {
        return new AnomalyDetectionSnapshot
        {
            MonitoredVariables = _registrations.Count,
            TotalAnomaliesDetected = Interlocked.Read(ref _totalAnomaliesDetected),
            TotalPredictiveAlarms = Interlocked.Read(ref _totalPredictiveAlarms)
        };
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    private class RegistrationEntry
    {
        public string VariablePath { get; set; } = "";
        public AnomalyDetectionConfig Config { get; set; } = new();
        public AlarmConfig? AlarmConfig { get; set; }
        public IVariableLogger Logger { get; set; } = null!;
    }
}
