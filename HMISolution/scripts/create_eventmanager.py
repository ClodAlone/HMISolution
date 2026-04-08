import os

path = r"C:\Users\cfior\source\repos\Server\EventManager.cs"

content = r'''using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Evaluates event conditions periodically and executes associated commands
/// when conditions are met, supporting rising-edge, falling-edge, and continuous triggers.
///
/// Published variables per event:
///   _Events.{Name}.Active     (Boolean)   — whether the condition is currently met
///   _Events.{Name}.LastFired  (DateTime)  — UTC timestamp of last command execution
/// </summary>
public sealed class EventManager : IDisposable
{
    private readonly SimpleFileServerNodeManager _nodeManager;
    private readonly List<EventState> _events = new();
    private Timer? _timer;
    private readonly CancellationTokenSource _cts = new();

    private const string Folder = "_Events";
    private const int TickIntervalMs = 500;

    public EventManager(SimpleFileServerNodeManager nodeManager)
    {
        _nodeManager = nodeManager;
    }

    public void Initialize(List<EventConfig> configs)
    {
        foreach (var config in configs)
        {
            DiagnosticsCollector.Instance.Register("Event", config.Name, config.Enabled);
            if (!config.Enabled) continue;

            _events.Add(new EventState(config));
        }

        if (_events.Count > 0)
        {
            _timer = new Timer(Tick, null,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromMilliseconds(TickIntervalMs));
            Log.Information("EventManager initialized with {Count} event(s).", _events.Count);
        }
    }

    private void Tick(object? state)
    {
        if (_cts.IsCancellationRequested) return;

        var now = DateTime.UtcNow;

        foreach (var evt in _events)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                EvaluateEvent(evt, now);
                sw.Stop();
                DiagnosticsCollector.Instance.RecordCycle("Event", evt.Config.Name, sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                DiagnosticsCollector.Instance.RecordCycle("Event", evt.Config.Name, sw.Elapsed.TotalMilliseconds, error: ex.Message);
                Log.Error(ex, "EventManager error for '{Name}': {Message}", evt.Config.Name, ex.Message);
            }
        }
    }

    private void EvaluateEvent(EventState evt, DateTime now)
    {
        var prefix = $"{Folder}.{evt.Config.Name}";

        // --- Read condition variable and evaluate ---
        bool conditionMet = false;
        if (!string.IsNullOrEmpty(evt.Config.ConditionVariablePath))
        {
            var rawValue = _nodeManager.ReadVariable(evt.Config.ConditionVariablePath);
            conditionMet = EvaluateCondition(rawValue, evt.Config.ConditionOperator, evt.Config.ConditionValue);
        }

        // --- Hold time logic ---
        bool holdSatisfied = true;
        if (evt.Config.HoldTimeSeconds > 0)
        {
            if (conditionMet)
            {
                evt.ConditionTrueSince ??= now;
                double elapsed = (now - evt.ConditionTrueSince.Value).TotalSeconds;
                holdSatisfied = elapsed >= evt.Config.HoldTimeSeconds;
            }
            else
            {
                evt.ConditionTrueSince = null;
                holdSatisfied = false;
            }
        }

        bool effectiveCondition = conditionMet && holdSatisfied;

        // --- Trigger mode evaluation ---
        bool shouldFireCommands = false;
        bool shouldFireClearCommands = false;

        switch (evt.Config.TriggerMode)
        {
            case "RisingEdge":
                if (effectiveCondition && !evt.PreviousConditionMet)
                    shouldFireCommands = true;
                if (!effectiveCondition && evt.PreviousConditionMet)
                    shouldFireClearCommands = true;
                break;

            case "FallingEdge":
                if (!effectiveCondition && evt.PreviousConditionMet)
                    shouldFireCommands = true;
                if (effectiveCondition && !evt.PreviousConditionMet)
                    shouldFireClearCommands = true;
                break;

            case "Continuous":
                if (effectiveCondition)
                    shouldFireCommands = true;
                if (!effectiveCondition && evt.PreviousConditionMet)
                    shouldFireClearCommands = true;
                break;
        }

        evt.PreviousConditionMet = effectiveCondition;

        // --- Execute commands ---
        if (shouldFireCommands && evt.Config.Commands.Count > 0)
        {
            ExecuteCommands(evt.Config.Commands, evt.Config.Name, "fire");
            evt.LastFired = now;
        }

        if (shouldFireClearCommands && evt.Config.ClearCommands.Count > 0)
        {
            ExecuteCommands(evt.Config.ClearCommands, evt.Config.Name, "clear");
        }

        // --- Publish OPC variables ---
        WriteVar($"{prefix}.Active", effectiveCondition);
        WriteVar($"{prefix}.LastFired", evt.LastFired ?? DateTime.MinValue);
    }

    private static bool EvaluateCondition(object? rawValue, string op, string conditionValue)
    {
        if (rawValue == null) return false;
        var strValue = rawValue.ToString() ?? "";

        switch (op)
        {
            case "True":
                return IsTruthy(rawValue);

            case "False":
                return !IsTruthy(rawValue);

            case "==":
                // Try numeric comparison first, then string
                if (double.TryParse(strValue, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var numVal) &&
                    double.TryParse(conditionValue, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var numCmp))
                    return Math.Abs(numVal - numCmp) < 1e-9;
                return string.Equals(strValue, conditionValue, StringComparison.OrdinalIgnoreCase);

            case "!=":
                if (double.TryParse(strValue, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var neqVal) &&
                    double.TryParse(conditionValue, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var neqCmp))
                    return Math.Abs(neqVal - neqCmp) >= 1e-9;
                return !string.Equals(strValue, conditionValue, StringComparison.OrdinalIgnoreCase);

            case ">":
                return TryNumericCompare(strValue, conditionValue, out var gt) && gt > 0;

            case "<":
                return TryNumericCompare(strValue, conditionValue, out var lt) && lt < 0;

            case ">=":
                return TryNumericCompare(strValue, conditionValue, out var gte) && gte >= 0;

            case "<=":
                return TryNumericCompare(strValue, conditionValue, out var lte) && lte <= 0;

            default:
                return IsTruthy(rawValue);
        }
    }

    private static bool TryNumericCompare(string left, string right, out int result)
    {
        result = 0;
        if (double.TryParse(left, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var l) &&
            double.TryParse(right, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var r))
        {
            result = l.CompareTo(r);
            return true;
        }
        return false;
    }

    private void ExecuteCommands(List<SymbolCommand> commands, string eventName, string phase)
    {
        foreach (var cmd in commands)
        {
            try
            {
                switch (cmd.Action)
                {
                    case "SetVariable":
                    case "WriteVariable":
                        if (!string.IsNullOrEmpty(cmd.VariablePath))
                            _nodeManager.WriteVariable(cmd.VariablePath, cmd.Value ?? "");
                        break;

                    case "ResetVariable":
                        if (!string.IsNullOrEmpty(cmd.VariablePath))
                            _nodeManager.WriteVariable(cmd.VariablePath, "0");
                        break;

                    case "ToggleVariable":
                        if (!string.IsNullOrEmpty(cmd.VariablePath))
                        {
                            var current = _nodeManager.ReadVariable(cmd.VariablePath);
                            var currentStr = current?.ToString() ?? "";
                            var isTruthy = currentStr is "True" or "true" or "1";
                            _nodeManager.WriteVariable(cmd.VariablePath, isTruthy ? false : true);
                        }
                        break;

                    case "IncrementVariable":
                        if (!string.IsNullOrEmpty(cmd.VariablePath))
                        {
                            var incCurrent = _nodeManager.ReadVariable(cmd.VariablePath);
                            if (double.TryParse(incCurrent?.ToString(), System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out var incVal))
                            {
                                var step = 1.0;
                                if (!string.IsNullOrEmpty(cmd.Value) &&
                                    double.TryParse(cmd.Value, System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out var s))
                                    step = s;
                                _nodeManager.WriteVariable(cmd.VariablePath, incVal + step);
                            }
                        }
                        break;

                    case "DecrementVariable":
                        if (!string.IsNullOrEmpty(cmd.VariablePath))
                        {
                            var decCurrent = _nodeManager.ReadVariable(cmd.VariablePath);
                            if (double.TryParse(decCurrent?.ToString(), System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out var decVal))
                            {
                                var step = 1.0;
                                if (!string.IsNullOrEmpty(cmd.Value) &&
                                    double.TryParse(cmd.Value, System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out var s))
                                    step = s;
                                _nodeManager.WriteVariable(cmd.VariablePath, decVal - step);
                            }
                        }
                        break;

                    case "GenerateReport":
                        if (!string.IsNullOrEmpty(cmd.TargetReport))
                        {
                            _ = _nodeManager.GenerateReportAsync(cmd.TargetReport);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Event '{Name}' {Phase} command '{Action}' failed: {Error}",
                    eventName, phase, cmd.Action, ex.Message);
            }
        }
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

    private sealed class EventState
    {
        public EventConfig Config { get; }
        public bool PreviousConditionMet { get; set; }
        public DateTime? ConditionTrueSince { get; set; }
        public DateTime? LastFired { get; set; }

        public EventState(EventConfig config)
        {
            Config = config;
        }
    }
}
'''

os.makedirs(os.path.dirname(path), exist_ok=True)
with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(content)

print(f"Created {path} ({len(content)} chars)")
