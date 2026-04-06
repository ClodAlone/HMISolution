using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Opc.Ua;
using Serilog;

namespace SimpleOpcFileServer
{
    public enum SimulationFunction
    {
        Ramp,
        Sine,
        Cosine,
        Triangle,
        Square,
        Sawtooth,
        Random,
        RandomInt,
        Blink,
        Counter,
        Pulse
    }

    public class SimulationConfig
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SimulationFunction Function { get; set; } = SimulationFunction.Sine;

        /// <summary>Period in milliseconds for one full cycle.</summary>
        public double Period { get; set; } = 10000;

        /// <summary>Amplitude (peak value). The signal oscillates between -Amplitude and +Amplitude (or 0..Amplitude for unsigned waveforms).</summary>
        public double Amplitude { get; set; } = 100;

        /// <summary>Vertical offset added to the signal.</summary>
        public double Offset { get; set; } = 0;

        /// <summary>Phase shift in degrees (0–360).</summary>
        public double Phase { get; set; } = 0;

        /// <summary>Poll / update interval in milliseconds.</summary>
        public int PollTime { get; set; } = 1000;

        /// <summary>Minimum value for Random / RandomInt.</summary>
        public double Min { get; set; } = 0;

        /// <summary>Maximum value for Random / RandomInt.</summary>
        public double Max { get; set; } = 100;

        /// <summary>Step size per tick for Ramp and Counter.</summary>
        public double Step { get; set; } = 1;

        /// <summary>Duty cycle (0.0–1.0) for Square and Pulse waveforms.</summary>
        public double DutyCycle { get; set; } = 0.5;
    }

    public class SimulationDriver : IDriver, IDisposable
    {
        public string Key => "Simulation";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly List<SimulationItem> _items = new();
        private readonly object _lock = new();
        private volatile SimulationItem[] _snapshot = []; // cached; rebuilt lazily
        private volatile bool _snapshotDirty;
        private Timer? _timer;
        private int _pollInterval = 100;
        private int _minPollTime = int.MaxValue;
        private bool _disposed;
        private readonly ISystemContext _context;
        private readonly Random _random = new();
        private readonly DateTime _startTime = DateTime.UtcNow;
        private static readonly JsonSerializerOptions s_jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public SimulationDriver(ISystemContext context)
        {
            _context = context;
        }

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var config = JsonSerializer.Deserialize<SimulationConfig>(configJson, s_jsonOptions);
            if (config == null) return;

            var item = new SimulationItem { Variable = variable, Config = config };
            lock (_lock)
            {
                _items.Add(item);
                // Track min incrementally — O(1) instead of O(n) scan
                if (config.PollTime < _minPollTime)
                    _minPollTime = config.PollTime;
                _pollInterval = Math.Max(50, _minPollTime);
                _snapshotDirty = true;
            }
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_timer == null && _items.Count > 0)
                    _timer = new Timer(Poll, null, _pollInterval, Timeout.Infinite);
            }
        }

        private void RebuildSnapshotIfNeeded()
        {
            if (!_snapshotDirty) return;
            lock (_lock)
            {
                if (!_snapshotDirty) return;
                _snapshot = [.. _items];
                _snapshotDirty = false;
            }
        }

        private void Poll(object? state)
        {
            if (_disposed) return;
            var _sw = System.Diagnostics.Stopwatch.StartNew();

            // Rebuild snapshot if items were added since last poll
            RebuildSnapshotIfNeeded();
            var snapshot = _snapshot;

            var now = DateTime.UtcNow;
            var elapsed = (now - _startTime).TotalMilliseconds;

            foreach (var item in snapshot)
            {
                try
                {
                    // Respect per-item PollTime: skip items whose interval hasn't elapsed
                    if (now - item.LastPollTime < item.PollTimeSpan)
                        continue;
                    item.LastPollTime = now;

                    var value = Compute(item, elapsed);
                    item.Variable.Value = value;
                    item.Variable.StatusCode = StatusCodes.Good;
                    item.Variable.Timestamp = now;
                    item.Variable.ClearChangeMasks(_context, false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Simulation error for {Name}: {Message}", item.Variable.DisplayName, ex.Message);
                    item.Variable.StatusCode = StatusCodes.Bad;
                    item.Variable.Timestamp = now;
                    item.Variable.ClearChangeMasks(_context, false);
                }
            }

            if (!_disposed) _timer?.Change(_pollInterval, Timeout.Infinite);
            OnCycleCompleted?.Invoke(_sw.Elapsed.TotalMilliseconds);
        }

        private object Compute(SimulationItem item, double elapsedMs)
        {
            var cfg = item.Config;
            var period = cfg.Period > 0 ? cfg.Period : 10000;
            var phaseRad = cfg.Phase * Math.PI / 180.0;
            var t = (elapsedMs % period) / period; // normalized 0..1
            var angle = t * 2.0 * Math.PI + phaseRad;

            return cfg.Function switch
            {
                SimulationFunction.Sine => cfg.Offset + cfg.Amplitude * Math.Sin(angle),
                SimulationFunction.Cosine => cfg.Offset + cfg.Amplitude * Math.Cos(angle),

                SimulationFunction.Ramp =>
                    cfg.Offset + (item.TickCount++ * cfg.Step) % (cfg.Amplitude == 0 ? 1 : cfg.Amplitude),

                SimulationFunction.Triangle =>
                    cfg.Offset + cfg.Amplitude * (2.0 * Math.Abs(2.0 * t - 1.0) - 1.0),

                SimulationFunction.Square =>
                    cfg.Offset + (t < cfg.DutyCycle ? cfg.Amplitude : -cfg.Amplitude),

                SimulationFunction.Sawtooth =>
                    cfg.Offset + cfg.Amplitude * (2.0 * t - 1.0),

                SimulationFunction.Random =>
                    cfg.Min + _random.NextDouble() * (cfg.Max - cfg.Min),

                SimulationFunction.RandomInt =>
                    _random.Next((int)cfg.Min, (int)cfg.Max + 1),

                SimulationFunction.Blink =>
                    t < cfg.DutyCycle,

                SimulationFunction.Counter =>
                    (double)(item.TickCount++ * (long)cfg.Step),

                SimulationFunction.Pulse =>
                    t < cfg.DutyCycle ? 1.0 : 0.0,

                _ => 0.0
            };
        }

        public void Dispose()
        {
            _disposed = true;
            _timer?.Dispose();
        }

        private class SimulationItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public SimulationConfig Config { get; set; } = null!;
            public long TickCount { get; set; }
            public DateTime LastPollTime { get; set; }
            public TimeSpan PollTimeSpan => TimeSpan.FromMilliseconds(Config.PollTime);
        }
    }
}
