namespace SharedModels;

/// <summary>
/// PID auto-tuning via the relay feedback (Åström–Hägglund) method.
/// 
/// The algorithm applies a symmetric relay (+d / -d) around a setpoint and
/// observes the resulting oscillation. From the oscillation amplitude (a) and
/// period (Pu), the ultimate gain (Ku) and ultimate period are derived, then
/// Ziegler–Nichols tuning rules compute Kp, Ki, Kd.
///
/// This class is a pure computation engine — no OPC/server dependencies.
/// </summary>
public class PidAutoTuner
{
    /// <summary>Input parameters for a relay auto-tune experiment.</summary>
    public class TuneRequest
    {
        /// <summary>Desired setpoint (process variable target).</summary>
        public double Setpoint { get; set; }

        /// <summary>Relay half-amplitude (output switches between +RelayAmplitude and -RelayAmplitude around the bias).</summary>
        public double RelayAmplitude { get; set; } = 10;

        /// <summary>Output bias (steady-state output around which the relay toggles).</summary>
        public double OutputBias { get; set; } = 50;

        /// <summary>Hysteresis band around the setpoint to prevent relay chattering.</summary>
        public double Hysteresis { get; set; } = 0.5;

        /// <summary>Minimum number of full oscillation cycles before computing gains. Default: 3.</summary>
        public int MinCycles { get; set; } = 3;

        /// <summary>Maximum experiment duration in seconds. Default: 300.</summary>
        public double MaxDurationSeconds { get; set; } = 300;

        /// <summary>PID type to compute: "P", "PI", or "PID". Default: "PID".</summary>
        public string PidType { get; set; } = "PID";
    }

    /// <summary>Results of a relay auto-tune experiment.</summary>
    public class TuneResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        /// <summary>Ultimate gain (Ku).</summary>
        public double UltimateGain { get; set; }

        /// <summary>Ultimate period (Pu) in seconds.</summary>
        public double UltimatePeriod { get; set; }

        /// <summary>Oscillation amplitude observed in the process variable.</summary>
        public double OscillationAmplitude { get; set; }

        /// <summary>Computed proportional gain.</summary>
        public double Kp { get; set; }

        /// <summary>Computed integral time (seconds). 0 if P-only.</summary>
        public double Ti { get; set; }

        /// <summary>Computed derivative time (seconds). 0 if P or PI.</summary>
        public double Td { get; set; }

        /// <summary>Ki = Kp / Ti (for convenience). 0 if P-only.</summary>
        public double Ki { get; set; }

        /// <summary>Kd = Kp * Td (for convenience). 0 if P or PI.</summary>
        public double Kd { get; set; }

        /// <summary>Number of oscillation cycles observed.</summary>
        public int CyclesObserved { get; set; }

        /// <summary>Generated Structured Text code implementing the tuned PID.</summary>
        public string GeneratedCode { get; set; } = "";
    }

    /// <summary>
    /// Tracks relay state during the live experiment. Fed sample-by-sample.
    /// </summary>
    public class RelayExperiment
    {
        private readonly TuneRequest _req;
        private bool _relayHigh;
        private readonly List<double> _crossingTimes = new();
        private double _pvMin = double.MaxValue;
        private double _pvMax = double.MinValue;
        private double _startTime;
        private bool _started;
        private int _halfCycles;

        public RelayExperiment(TuneRequest request)
        {
            _req = request;
            _relayHigh = true; // start with relay high
        }

        /// <summary>
        /// Feed a new process variable sample.
        /// Returns the relay output to write and whether the experiment is complete.
        /// </summary>
        public (double Output, bool Complete) ProcessSample(double pv, double elapsedSeconds)
        {
            if (!_started)
            {
                _startTime = elapsedSeconds;
                _started = true;
            }

            // Track min/max PV for amplitude calculation
            _pvMin = Math.Min(_pvMin, pv);
            _pvMax = Math.Max(_pvMax, pv);

            // Relay switching with hysteresis
            bool switched = false;
            if (_relayHigh && pv > _req.Setpoint + _req.Hysteresis)
            {
                _relayHigh = false;
                switched = true;
            }
            else if (!_relayHigh && pv < _req.Setpoint - _req.Hysteresis)
            {
                _relayHigh = true;
                switched = true;
            }

            if (switched)
            {
                _crossingTimes.Add(elapsedSeconds);
                _halfCycles++;

                // Reset min/max tracking after first full cycle to exclude initial transient
                if (_halfCycles == 2)
                {
                    _pvMin = double.MaxValue;
                    _pvMax = double.MinValue;
                }
            }

            double output = _req.OutputBias + (_relayHigh ? _req.RelayAmplitude : -_req.RelayAmplitude);

            // Check completion: need MinCycles full cycles (2 half-cycles each) after transient
            int fullCycles = (_halfCycles - 2) / 2; // exclude first cycle as transient
            bool complete = fullCycles >= _req.MinCycles;

            // Timeout
            if (elapsedSeconds - _startTime > _req.MaxDurationSeconds)
                complete = true;

            return (output, complete);
        }

        /// <summary>Compute tuning results from the collected oscillation data.</summary>
        public TuneResult ComputeResult()
        {
            // Need at least 4 crossings (2 full cycles) after transient
            if (_crossingTimes.Count < 4)
            {
                return new TuneResult
                {
                    Success = false,
                    Message = $"Insufficient oscillation detected ({_crossingTimes.Count} crossings). " +
                              "Check that the relay amplitude is large enough and the process responds."
                };
            }

            // Calculate average period from crossings (skip first 2 as transient)
            var periods = new List<double>();
            for (int i = 3; i < _crossingTimes.Count; i += 2)
            {
                if (i >= 2)
                    periods.Add(_crossingTimes[i] - _crossingTimes[i - 2]);
            }

            if (periods.Count == 0)
            {
                return new TuneResult
                {
                    Success = false,
                    Message = "Could not compute oscillation period."
                };
            }

            double pu = periods.Average(); // ultimate period
            double a = (_pvMax - _pvMin) / 2.0; // oscillation amplitude

            if (a < 1e-9)
            {
                return new TuneResult
                {
                    Success = false,
                    Message = "Oscillation amplitude too small. Increase relay amplitude."
                };
            }

            // Ultimate gain: Ku = 4d / (π·a)  (relay feedback formula)
            double ku = (4.0 * _req.RelayAmplitude) / (Math.PI * a);

            return ComputeGains(ku, pu, a, periods.Count, _req.PidType);
        }
    }

    /// <summary>
    /// Compute PID gains from ultimate gain and period using Ziegler–Nichols rules.
    /// </summary>
    public static TuneResult ComputeGains(double ku, double pu, double amplitude, int cycles, string pidType)
    {
        double kp, ti, td;

        switch (pidType.ToUpperInvariant())
        {
            case "P":
                kp = 0.50 * ku;
                ti = 0;
                td = 0;
                break;
            case "PI":
                kp = 0.45 * ku;
                ti = pu / 1.2;
                td = 0;
                break;
            case "PID":
            default:
                kp = 0.60 * ku;
                ti = pu / 2.0;
                td = pu / 8.0;
                break;
        }

        double ki = ti > 0 ? kp / ti : 0;
        double kd = kp * td;

        var result = new TuneResult
        {
            Success = true,
            Message = $"Auto-tune complete. {cycles} oscillation cycle(s) analysed.",
            UltimateGain = Math.Round(ku, 6),
            UltimatePeriod = Math.Round(pu, 4),
            OscillationAmplitude = Math.Round(amplitude, 4),
            Kp = Math.Round(kp, 6),
            Ti = Math.Round(ti, 4),
            Td = Math.Round(td, 4),
            Ki = Math.Round(ki, 6),
            Kd = Math.Round(kd, 6),
            CyclesObserved = cycles
        };

        result.GeneratedCode = GenerateStCode(result, pidType);
        return result;
    }

    /// <summary>
    /// Generate IEC 61131-3 Structured Text code for a PID controller with the computed gains.
    /// Uses the existing PLC engine's Read()/Write() functions.
    /// </summary>
    public static string GenerateStCode(TuneResult result, string pidType)
    {
        var lines = new List<string>
        {
            "(* Auto-Tuned PID Controller *)",
            $"(* Kp={result.Kp}  Ki={result.Ki}  Kd={result.Kd} *)",
            $"(* Ku={result.UltimateGain}  Pu={result.UltimatePeriod}s  Method=Ziegler-Nichols *)",
            "",
            "VAR",
            $"  Kp : REAL := {result.Kp.ToString(System.Globalization.CultureInfo.InvariantCulture)};",
        };

        if (pidType.ToUpperInvariant() is "PI" or "PID")
            lines.Add($"  Ki : REAL := {result.Ki.ToString(System.Globalization.CultureInfo.InvariantCulture)};");
        if (pidType.ToUpperInvariant() == "PID")
            lines.Add($"  Kd : REAL := {result.Kd.ToString(System.Globalization.CultureInfo.InvariantCulture)};");

        lines.AddRange(new[]
        {
            "  Setpoint : REAL;",
            "  PV : REAL;",
            "  Error : REAL;",
        });

        if (pidType.ToUpperInvariant() == "PID")
            lines.Add("  PrevError : REAL;");
        if (pidType.ToUpperInvariant() is "PI" or "PID")
            lines.Add("  Integral : REAL;");
        if (pidType.ToUpperInvariant() == "PID")
            lines.Add("  Derivative : REAL;");

        lines.AddRange(new[]
        {
            "  Output : REAL;",
            "  OutMin : REAL := 0.0;",
            "  OutMax : REAL := 100.0;",
            "END_VAR",
            "",
            "(* Read process variable and setpoint *)",
            "Setpoint := Read('TODO_SETPOINT_PATH');",
            "PV := Read('TODO_PV_PATH');",
            "",
            "(* Calculate error *)",
            "Error := Setpoint - PV;",
            "",
            "(* Proportional term *)",
            "Output := Kp * Error;",
        });

        if (pidType.ToUpperInvariant() is "PI" or "PID")
        {
            lines.AddRange(new[]
            {
                "",
                "(* Integral term with anti-windup *)",
                "Integral := Integral + Error;",
                "IF Integral * Ki > OutMax THEN Integral := OutMax / Ki; END_IF;",
                "IF Integral * Ki < OutMin THEN Integral := OutMin / Ki; END_IF;",
                "Output := Output + Ki * Integral;",
            });
        }

        if (pidType.ToUpperInvariant() == "PID")
        {
            lines.AddRange(new[]
            {
                "",
                "(* Derivative term *)",
                "Derivative := Error - PrevError;",
                "Output := Output + Kd * Derivative;",
            });
        }

        lines.AddRange(new[]
        {
            "",
            "(* Clamp output *)",
            "IF Output > OutMax THEN Output := OutMax; END_IF;",
            "IF Output < OutMin THEN Output := OutMin; END_IF;",
            "",
            "(* Write output and save state *)",
            "Write('TODO_OUTPUT_PATH', Output);",
            "PrevError := Error;",
        });

        return string.Join("\n", lines);
    }
}
