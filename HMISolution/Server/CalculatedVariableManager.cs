using System.Collections.Concurrent;
using System.Globalization;
using System.Text.RegularExpressions;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Manages calculated (virtual) variables whose values are computed from expressions
/// referencing other OPC variables, or from built-in aggregate functions.
/// Each calculated variable evaluates cyclically at its configured interval.
/// Expressions use Read("path") to reference OPC variable values and support
/// standard C# math operators and System.Math methods.
/// Aggregate mode supports: Avg, Sum, Min, Max, Count, RateOfChange, Delta,
/// RunningAvg, RunningMin, RunningMax, StdDev over configurable time windows.
/// </summary>
public sealed class CalculatedVariableManager : IDisposable
{
    private readonly SimpleFileServerNodeManager _nodeManager;
    private readonly List<ICalculatedRunner> _runners = new();
    private readonly CancellationTokenSource _cts = new();

    public CalculatedVariableManager(SimpleFileServerNodeManager nodeManager)
    {
        _nodeManager = nodeManager;
    }

    public void Initialize(List<CalculatedVariableConfig> configs)
    {
        foreach (var config in configs)
        {
            DiagnosticsCollector.Instance.Register("Calculated", config.Name, config.Enabled);
            if (!config.Enabled) continue;

            try
            {
                ICalculatedRunner runner;
                if (!string.IsNullOrEmpty(config.AggregateFunction))
                {
                    runner = new AggregateRunner(config, _nodeManager, _cts.Token);
                    Log.Information("Calculated aggregate '{Name}' started (function={Fn}, interval={IntervalMs}ms, window={WindowSec}s)",
                        config.Name, config.AggregateFunction, config.IntervalMs, config.AggregateWindowSeconds);
                }
                else
                {
                    runner = new CalculatedRunner(config, _nodeManager, _cts.Token);
                    Log.Information("Calculated variable '{Name}' started (interval={IntervalMs}ms)",
                        config.Name, config.IntervalMs);
                }
                _runners.Add(runner);
                runner.Start();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize calculated variable '{Name}'", config.Name);
                DiagnosticsCollector.Instance.SetStatus("Calculated", config.Name, "Error", ex.Message);
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        foreach (var r in _runners) r.Stop();
        _cts.Dispose();
    }
}

internal interface ICalculatedRunner
{
    void Start();
    void Stop();
}

internal sealed class CalculatedRunner : ICalculatedRunner
{
    private readonly CalculatedVariableConfig _config;
    private readonly SimpleFileServerNodeManager _nodeManager;
    private readonly CancellationToken _token;
    private Task? _task;

    // Pre-parsed Read("...") references in the expression
    private readonly List<string> _referencedPaths;
    private readonly string _evaluableExpression;

    private static readonly Regex ReadPattern = new(@"Read\(""([^""]+)""\)", RegexOptions.Compiled);

    public CalculatedRunner(CalculatedVariableConfig config, SimpleFileServerNodeManager nodeManager, CancellationToken token)
    {
        _config = config;
        _nodeManager = nodeManager;
        _token = token;

        // Extract all Read("path") references
        _referencedPaths = new List<string>();
        var matches = ReadPattern.Matches(config.Expression);
        foreach (Match m in matches)
            _referencedPaths.Add(m.Groups[1].Value);

        // Pre-process expression: replace Read("path") with placeholders {0}, {1}, etc.
        int idx = 0;
        _evaluableExpression = ReadPattern.Replace(config.Expression, _ => $"{{{idx++}}}");
    }

    public void Start()
    {
        _task = Task.Run(async () =>
        {
            while (!_token.IsCancellationRequested)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    var value = Evaluate();
                    var outputPath = $"{_config.FolderPath}.{_config.Name}";
                    _nodeManager.WriteVariable(outputPath, value!);
                    sw.Stop();
                    DiagnosticsCollector.Instance.RecordCycle("Calculated", _config.Name, sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    sw.Stop();
                    DiagnosticsCollector.Instance.SetStatus("Calculated", _config.Name, "Error", ex.Message);
                }

                try { await Task.Delay(_config.IntervalMs, _token); }
                catch (OperationCanceledException) { break; }
            }
        }, _token);
    }

    public void Stop()
    {
        try { _task?.Wait(2000); } catch { }
    }

    private object? Evaluate()
    {
        // Read all referenced variable values
        var values = new double[_referencedPaths.Count];
        for (int i = 0; i < _referencedPaths.Count; i++)
        {
            var raw = _nodeManager.ReadVariable(_referencedPaths[i]);
            values[i] = Convert.ToDouble(raw, CultureInfo.InvariantCulture);
        }

        // Build the expression string with resolved values
        var args = values.Select(v => v.ToString("R", CultureInfo.InvariantCulture)).ToArray();
        var expr = string.Format(_evaluableExpression, args);

        // Evaluate using DataTable.Compute for simple math, or a lightweight evaluator
        var result = EvaluateSimpleExpression(expr);

        return _config.Type switch
        {
            "Int32" => Convert.ToInt32(result),
            "Boolean" => Convert.ToBoolean(result),
            "String" => Convert.ToString(result, CultureInfo.InvariantCulture),
            _ => Convert.ToDouble(result, CultureInfo.InvariantCulture)
        };
    }

    /// <summary>
    /// Evaluates simple arithmetic expressions including Math.* functions.
    /// Supports: +, -, *, /, %, parentheses, Math.Round, Math.Abs, Math.Sqrt,
    /// Math.Min, Math.Max, Math.Pow, Math.Clamp, Math.Floor, Math.Ceiling.
    /// </summary>
    private static double EvaluateSimpleExpression(string expression)
    {
        // Pre-process Math.* functions
        expression = ProcessMathFunctions(expression);

        // Use DataTable.Compute for basic arithmetic
        using var dt = new System.Data.DataTable();
        var result = dt.Compute(expression, null);
        return Convert.ToDouble(result, CultureInfo.InvariantCulture);
    }

    private static string ProcessMathFunctions(string expr)
    {
        // Handle Math.Round(x, n), Math.Abs(x), Math.Sqrt(x), Math.Min(a,b), Math.Max(a,b),
        // Math.Pow(a,b), Math.Floor(x), Math.Ceiling(x), Math.Clamp(v,min,max)
        expr = Regex.Replace(expr, @"Math\.Round\(([^,]+),\s*(\d+)\)", m =>
        {
            var val = EvaluateSimpleExpression(m.Groups[1].Value);
            var decimals = int.Parse(m.Groups[2].Value);
            return Math.Round(val, decimals).ToString("R", CultureInfo.InvariantCulture);
        });
        expr = Regex.Replace(expr, @"Math\.Abs\(([^)]+)\)", m =>
            Math.Abs(EvaluateSimpleExpression(m.Groups[1].Value)).ToString("R", CultureInfo.InvariantCulture));
        expr = Regex.Replace(expr, @"Math\.Sqrt\(([^)]+)\)", m =>
            Math.Sqrt(EvaluateSimpleExpression(m.Groups[1].Value)).ToString("R", CultureInfo.InvariantCulture));
        expr = Regex.Replace(expr, @"Math\.Floor\(([^)]+)\)", m =>
            Math.Floor(EvaluateSimpleExpression(m.Groups[1].Value)).ToString("R", CultureInfo.InvariantCulture));
        expr = Regex.Replace(expr, @"Math\.Ceiling\(([^)]+)\)", m =>
            Math.Ceiling(EvaluateSimpleExpression(m.Groups[1].Value)).ToString("R", CultureInfo.InvariantCulture));
        expr = Regex.Replace(expr, @"Math\.Min\(([^,]+),\s*([^)]+)\)", m =>
        {
            var a = EvaluateSimpleExpression(m.Groups[1].Value);
            var b = EvaluateSimpleExpression(m.Groups[2].Value);
            return Math.Min(a, b).ToString("R", CultureInfo.InvariantCulture);
        });
        expr = Regex.Replace(expr, @"Math\.Max\(([^,]+),\s*([^)]+)\)", m =>
        {
            var a = EvaluateSimpleExpression(m.Groups[1].Value);
            var b = EvaluateSimpleExpression(m.Groups[2].Value);
            return Math.Max(a, b).ToString("R", CultureInfo.InvariantCulture);
        });
        expr = Regex.Replace(expr, @"Math\.Pow\(([^,]+),\s*([^)]+)\)", m =>
        {
            var a = EvaluateSimpleExpression(m.Groups[1].Value);
            var b = EvaluateSimpleExpression(m.Groups[2].Value);
            return Math.Pow(a, b).ToString("R", CultureInfo.InvariantCulture);
        });
        expr = Regex.Replace(expr, @"Math\.Clamp\(([^,]+),\s*([^,]+),\s*([^)]+)\)", m =>
        {
            var v = EvaluateSimpleExpression(m.Groups[1].Value);
            var min = EvaluateSimpleExpression(m.Groups[2].Value);
            var max = EvaluateSimpleExpression(m.Groups[3].Value);
            return Math.Clamp(v, min, max).ToString("R", CultureInfo.InvariantCulture);
        });

        return expr;
    }
}

/// <summary>
/// Runs built-in aggregate functions (Avg, Sum, Min, Max, Count, RateOfChange, Delta,
/// RunningAvg, RunningMin, RunningMax, StdDev) on one or more source variables.
/// Time-windowed aggregates (RunningAvg/Min/Max/StdDev) maintain a rolling sample buffer.
/// </summary>
internal sealed class AggregateRunner : ICalculatedRunner
{
    private readonly CalculatedVariableConfig _config;
    private readonly SimpleFileServerNodeManager _nodeManager;
    private readonly CancellationToken _token;
    private readonly string[] _sourcePaths;
    private Task? _task;

    // Rolling window sample buffer for time-based aggregates
    private readonly List<(DateTime Timestamp, double Value)> _samples = new();
    private readonly object _sampleLock = new();

    // Previous value tracking for RateOfChange / Delta
    private double? _previousValue;
    private DateTime _previousTimestamp;

    // Cumulative trackers for running aggregates without window
    private double _runningSum;
    private long _runningCount;

    private static readonly HashSet<string> WindowedFunctions = new(StringComparer.OrdinalIgnoreCase)
    {
        "RunningAvg", "RunningMin", "RunningMax", "StdDev"
    };

    public AggregateRunner(CalculatedVariableConfig config, SimpleFileServerNodeManager nodeManager, CancellationToken token)
    {
        _config = config;
        _nodeManager = nodeManager;
        _token = token;
        _sourcePaths = config.AggregateSourcePath
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        _previousTimestamp = DateTime.UtcNow;

        if (_sourcePaths.Length == 0)
            throw new InvalidOperationException($"Calculated variable '{config.Name}': AggregateSourcePath is required when AggregateFunction is set.");
    }

    public void Start()
    {
        _task = Task.Run(async () =>
        {
            while (!_token.IsCancellationRequested)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    var value = Evaluate();
                    var outputPath = $"{_config.FolderPath}.{_config.Name}";
                    _nodeManager.WriteVariable(outputPath, value!);
                    sw.Stop();
                    DiagnosticsCollector.Instance.RecordCycle("Calculated", _config.Name, sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    sw.Stop();
                    DiagnosticsCollector.Instance.SetStatus("Calculated", _config.Name, "Error", ex.Message);
                }

                try { await Task.Delay(_config.IntervalMs, _token); }
                catch (OperationCanceledException) { break; }
            }
        }, _token);
    }

    public void Stop()
    {
        try { _task?.Wait(2000); } catch { }
    }

    private object? Evaluate()
    {
        var fn = _config.AggregateFunction;
        double result;

        if (WindowedFunctions.Contains(fn))
        {
            // Single-source windowed aggregate
            var raw = _nodeManager.ReadVariable(_sourcePaths[0]);
            var current = Convert.ToDouble(raw, CultureInfo.InvariantCulture);
            var now = DateTime.UtcNow;

            lock (_sampleLock)
            {
                _samples.Add((now, current));
                // Trim samples outside the window
                var cutoff = now.AddSeconds(-_config.AggregateWindowSeconds);
                _samples.RemoveAll(s => s.Timestamp < cutoff);

                result = fn.ToLowerInvariant() switch
                {
                    "runningavg" => _samples.Count > 0 ? _samples.Average(s => s.Value) : current,
                    "runningmin" => _samples.Count > 0 ? _samples.Min(s => s.Value) : current,
                    "runningmax" => _samples.Count > 0 ? _samples.Max(s => s.Value) : current,
                    "stddev" => CalculateStdDev(),
                    _ => current
                };
            }
        }
        else if (fn.Equals("RateOfChange", StringComparison.OrdinalIgnoreCase))
        {
            var raw = _nodeManager.ReadVariable(_sourcePaths[0]);
            var current = Convert.ToDouble(raw, CultureInfo.InvariantCulture);
            var now = DateTime.UtcNow;

            if (_previousValue.HasValue)
            {
                var dt = (now - _previousTimestamp).TotalSeconds;
                result = dt > 0 ? (current - _previousValue.Value) / dt : 0;
            }
            else
            {
                result = 0;
            }
            _previousValue = current;
            _previousTimestamp = now;
        }
        else if (fn.Equals("Delta", StringComparison.OrdinalIgnoreCase))
        {
            var raw = _nodeManager.ReadVariable(_sourcePaths[0]);
            var current = Convert.ToDouble(raw, CultureInfo.InvariantCulture);

            result = _previousValue.HasValue ? current - _previousValue.Value : 0;
            _previousValue = current;
        }
        else
        {
            // Multi-source instant aggregates: read all sources
            var values = new double[_sourcePaths.Length];
            for (int i = 0; i < _sourcePaths.Length; i++)
            {
                var raw = _nodeManager.ReadVariable(_sourcePaths[i]);
                values[i] = Convert.ToDouble(raw, CultureInfo.InvariantCulture);
            }

            result = fn.ToLowerInvariant() switch
            {
                "avg" => values.Average(),
                "sum" => values.Sum(),
                "min" => values.Min(),
                "max" => values.Max(),
                "count" => values.Length,
                _ => throw new InvalidOperationException($"Unknown aggregate function: {fn}")
            };
        }

        return _config.Type switch
        {
            "Int32" => Convert.ToInt32(result),
            "Boolean" => result != 0,
            "String" => result.ToString("G6", CultureInfo.InvariantCulture),
            _ => Math.Round(result, 6)
        };
    }

    private double CalculateStdDev()
    {
        if (_samples.Count < 2) return 0;
        var avg = _samples.Average(s => s.Value);
        var sumSq = _samples.Sum(s => (s.Value - avg) * (s.Value - avg));
        return Math.Sqrt(sumSq / (_samples.Count - 1));
    }
}
