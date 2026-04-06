using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Serilog;
using SharedModels;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Background service that periodically reads historical data for variables with
    /// <see cref="AnomalyDetectionConfig"/> enabled, applies Z-score anomaly detection
    /// and linear-trend prediction, and generates predictive alarms via the event logger
    /// and notification service.
    /// </summary>
    public sealed class AnomalyDetectionService : IDisposable
    {
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly AnomalyDetectionSettings _settings;
        private readonly EventLogger? _eventLogger;
        private readonly NotificationService? _notificationService;
        private readonly CancellationTokenSource _cts = new();
        private Task? _loopTask;

        // Per-variable tracking state
        private readonly ConcurrentDictionary<string, VariableAnomalyState> _states = new();

        // Diagnostics counters
        private long _totalAnomalies;
        private long _totalPredictive;

        /// <summary>
        /// Registered monitored entries: variable path → (config, alarm config, variable logger).
        /// Populated during initialization by the node manager.
        /// </summary>
        private readonly List<MonitoredVariable> _monitored = new();

        public AnomalyDetectionService(
            SimpleFileServerNodeManager nodeManager,
            AnomalyDetectionSettings settings,
            EventLogger? eventLogger,
            NotificationService? notificationService)
        {
            _nodeManager = nodeManager;
            _settings = settings;
            _eventLogger = eventLogger;
            _notificationService = notificationService;
        }

        /// <summary>Register a variable for anomaly monitoring.</summary>
        public void Register(string variablePath, AnomalyDetectionConfig config, AlarmConfig? alarmConfig, IVariableLogger? logger)
        {
            if (!config.Enabled || logger == null) return;

            _monitored.Add(new MonitoredVariable
            {
                VariablePath = variablePath,
                Config = config,
                AlarmConfig = alarmConfig,
                Logger = logger
            });
        }

        /// <summary>Start the background analysis loop.</summary>
        public void Start()
        {
            if (_monitored.Count == 0) return;

            DiagnosticsCollector.Instance.Register("AnomalyDetection", "Service");
            DiagnosticsCollector.Instance.AlarmAnomalyProvider = BuildSnapshot;

            _loopTask = Task.Run(AnalysisLoop);
            Log.Information("Anomaly detection service started — monitoring {Count} variable(s), interval {Interval}s",
                _monitored.Count, _settings.IntervalSeconds);
            _eventLogger?.LogSystem("Info", "AnomalyDetection",
                $"Anomaly detection started — {_monitored.Count} variable(s) monitored every {_settings.IntervalSeconds}s");
        }

        private async Task AnalysisLoop()
        {
            var interval = TimeSpan.FromSeconds(Math.Max(_settings.IntervalSeconds, 5));

            // For large variable counts, process in batches across multiple cycles
            // to avoid overwhelming the SQLite logger with 100K+ ReadHistory queries.
            const int MaxPerCycle = 1000;
            int offset = 0;

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(interval, _cts.Token);
                }
                catch (OperationCanceledException) { break; }

                var sw = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    int count = _monitored.Count;
                    int batchSize = Math.Min(MaxPerCycle, count);
                    if (offset >= count) offset = 0;
                    int end = Math.Min(offset + batchSize, count);

                    for (int i = offset; i < end; i++)
                    {
                        if (_cts.Token.IsCancellationRequested) break;
                        AnalyzeVariable(_monitored[i]);
                    }

                    offset = end >= count ? 0 : end;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Anomaly detection loop error: {Message}", ex.Message);
                }
                sw.Stop();
                DiagnosticsCollector.Instance.RecordCycle("AnomalyDetection", "Service", sw.Elapsed.TotalMilliseconds);
            }
        }

        private void AnalyzeVariable(MonitoredVariable entry)
        {
            try
            {
                var now = DateTime.UtcNow;
                var windowStart = now.AddMinutes(-entry.Config.WindowMinutes);
                var history = entry.Logger.ReadHistory(entry.VariablePath, windowStart, now);

                // Extract numeric values with timestamps
                var points = new List<(DateTime Time, double Value)>();
                foreach (var dv in history)
                {
                    var numVal = AlarmEvaluator.GetNumericValue(dv.Value);
                    if (numVal.HasValue)
                        points.Add((dv.SourceTimestamp, numVal.Value));
                }

                if (points.Count < _settings.MinSampleCount) return;

                var state = _states.GetOrAdd(entry.VariablePath, _ => new VariableAnomalyState());

                double mean = points.Average(p => p.Value);
                double variance = points.Average(p => (p.Value - mean) * (p.Value - mean));
                double stdDev = Math.Sqrt(variance);

                // Use the most recent value for anomaly testing
                var latest = points[^1];

                // ─── Z-Score Anomaly Detection ───
                if (stdDev > 1e-10) // Avoid division by zero for constant signals
                {
                    double zScore = Math.Abs(latest.Value - mean) / stdDev;

                    if (zScore > entry.Config.Sensitivity)
                    {
                        // Check cooldown
                        if (!state.LastAnomalyTime.HasValue ||
                            (now - state.LastAnomalyTime.Value).TotalSeconds >= entry.Config.CooldownSeconds)
                        {
                            state.LastAnomalyTime = now;
                            Interlocked.Increment(ref _totalAnomalies);

                            var message = $"Anomaly detected on {entry.VariablePath}: " +
                                          $"value={latest.Value:G6}, Z-score={zScore:F2} " +
                                          $"(threshold={entry.Config.Sensitivity:F1}), " +
                                          $"mean={mean:G6}, stdDev={stdDev:G4}";

                            Log.Warning(message);
                            _eventLogger?.LogAlarm("Warning", entry.VariablePath, message,
                                $"ZScore={zScore:F2} Mean={mean:G6} StdDev={stdDev:G4} Window={entry.Config.WindowMinutes}min Samples={points.Count}");

                            if (entry.AlarmConfig is { NotifyOnActivation: true })
                                _notificationService?.NotifyAlarmActivated(entry.VariablePath, message, entry.Config.Severity);

                            state.ActiveAnomaly = new AnomalyEntry
                            {
                                VariablePath = entry.VariablePath,
                                DetectionType = "ZScore",
                                CurrentValue = latest.Value,
                                Mean = Math.Round(mean, 6),
                                StdDev = Math.Round(stdDev, 6),
                                ZScore = Math.Round(zScore, 2),
                                DetectedAtUtc = now,
                                Message = message
                            };
                        }
                    }
                    else
                    {
                        // Clear active anomaly once value returns to normal
                        if (state.ActiveAnomaly?.DetectionType == "ZScore")
                            state.ActiveAnomaly = null;
                    }
                }

                // ─── Trend-Based Predictive Alarm ───
                if (entry.Config.EnableTrendPrediction && entry.AlarmConfig != null && points.Count >= 3)
                {
                    // Linear regression: y = slope * t + intercept (t in minutes from first point)
                    double tBase = points[0].Time.Ticks;
                    double n = points.Count;
                    double sumT = 0, sumV = 0, sumTV = 0, sumT2 = 0;

                    foreach (var (time, value) in points)
                    {
                        double t = (time.Ticks - tBase) / (TimeSpan.TicksPerMinute * 1.0);
                        sumT += t;
                        sumV += value;
                        sumTV += t * value;
                        sumT2 += t * t;
                    }

                    double denom = n * sumT2 - sumT * sumT;
                    if (Math.Abs(denom) > 1e-15)
                    {
                        double slope = (n * sumTV - sumT * sumV) / denom;
                        double intercept = (sumV - slope * sumT) / n;

                        // Project forward from the latest time
                        double tNow = (latest.Time.Ticks - tBase) / (TimeSpan.TicksPerMinute * 1.0);
                        double horizon = entry.Config.PredictionHorizonMinutes;

                        var cfg = entry.AlarmConfig;
                        double highLimit = cfg.HighHighLimit ?? cfg.HighLimit;
                        double lowLimit = cfg.LowLowLimit ?? cfg.LowLimit;

                        string predictedLimit = "";
                        double? minutesToBreach = null;

                        if (slope > 1e-10)
                        {
                            // Rising trend — check high limit
                            double tBreach = (highLimit - intercept) / slope;
                            double minsUntilBreach = tBreach - tNow;
                            if (minsUntilBreach > 0 && minsUntilBreach <= horizon)
                            {
                                predictedLimit = cfg.HighHighLimit.HasValue && highLimit == cfg.HighHighLimit.Value ? "HighHigh" : "High";
                                minutesToBreach = Math.Round(minsUntilBreach, 1);
                            }
                        }
                        else if (slope < -1e-10)
                        {
                            // Falling trend — check low limit
                            double tBreach = (lowLimit - intercept) / slope;
                            double minsUntilBreach = tBreach - tNow;
                            if (minsUntilBreach > 0 && minsUntilBreach <= horizon)
                            {
                                predictedLimit = cfg.LowLowLimit.HasValue && lowLimit == cfg.LowLowLimit.Value ? "LowLow" : "Low";
                                minutesToBreach = Math.Round(minsUntilBreach, 1);
                            }
                        }

                        if (minutesToBreach.HasValue && !string.IsNullOrEmpty(predictedLimit))
                        {
                            // Check cooldown for trend predictions
                            if (!state.LastPredictionTime.HasValue ||
                                (now - state.LastPredictionTime.Value).TotalSeconds >= entry.Config.CooldownSeconds)
                            {
                                state.LastPredictionTime = now;
                                Interlocked.Increment(ref _totalPredictive);

                                var message = $"Predictive alarm on {entry.VariablePath}: " +
                                              $"trend projects {predictedLimit} limit breach in ~{minutesToBreach:F1} min " +
                                              $"(current={latest.Value:G6}, slope={slope:G4}/min)";

                                Log.Warning(message);
                                _eventLogger?.LogAlarm("Warning", entry.VariablePath, message,
                                    $"Prediction: {predictedLimit} breach in {minutesToBreach:F1}min, slope={slope:G4}/min, " +
                                    $"current={latest.Value:G6}, limit={highLimit}/{lowLimit}");

                                if (entry.AlarmConfig is { NotifyOnActivation: true })
                                    _notificationService?.NotifyAlarmActivated(entry.VariablePath, message, entry.Config.Severity);

                                state.ActivePrediction = new AnomalyEntry
                                {
                                    VariablePath = entry.VariablePath,
                                    DetectionType = "TrendPrediction",
                                    CurrentValue = latest.Value,
                                    Mean = Math.Round(mean, 6),
                                    StdDev = Math.Round(stdDev, 6),
                                    PredictedMinutesToBreach = minutesToBreach,
                                    PredictedLimit = predictedLimit,
                                    DetectedAtUtc = now,
                                    Message = message
                                };
                            }
                        }
                        else
                        {
                            // No breach predicted — clear
                            state.ActivePrediction = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Anomaly detection error for {Path}: {Message}", entry.VariablePath, ex.Message);
            }
        }

        /// <summary>Build a diagnostics snapshot for the /diag endpoint.</summary>
        internal AnomalyDetectionSnapshot BuildSnapshot()
        {
            var active = new List<AnomalyEntry>();
            foreach (var kvp in _states)
            {
                if (kvp.Value.ActiveAnomaly != null) active.Add(kvp.Value.ActiveAnomaly);
                if (kvp.Value.ActivePrediction != null) active.Add(kvp.Value.ActivePrediction);
            }

            return new AnomalyDetectionSnapshot
            {
                Timestamp = DateTime.UtcNow,
                MonitoredVariables = _monitored.Count,
                TotalAnomaliesDetected = Interlocked.Read(ref _totalAnomalies),
                TotalPredictiveAlarms = Interlocked.Read(ref _totalPredictive),
                ActiveAnomalies = active
            };
        }

        public void Dispose()
        {
            _cts.Cancel();
            try { _loopTask?.Wait(TimeSpan.FromSeconds(3)); } catch { /* shutdown */ }
            _cts.Dispose();
        }

        // ─── Internal types ──────────────────────────────────────────

        private sealed class MonitoredVariable
        {
            public required string VariablePath { get; init; }
            public required AnomalyDetectionConfig Config { get; init; }
            public AlarmConfig? AlarmConfig { get; init; }
            public required IVariableLogger Logger { get; init; }
        }

        private sealed class VariableAnomalyState
        {
            public DateTime? LastAnomalyTime { get; set; }
            public DateTime? LastPredictionTime { get; set; }
            public AnomalyEntry? ActiveAnomaly { get; set; }
            public AnomalyEntry? ActivePrediction { get; set; }
        }
    }
}
