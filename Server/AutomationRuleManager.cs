using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Evaluates no-code AutomationRules cyclically (every second).
    /// For each enabled rule the manager:
    ///   - evaluates all Conditions (AND / OR depending on ConditionLogic)
    ///   - fires Actions on a rising edge (false → true)
    ///   - fires ClearActions on a falling edge (true → false)
    ///   - respects CooldownSeconds to prevent repeated firings
    /// </summary>
    public sealed class AutomationRuleManager : IDisposable
    {
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly List<RuleState> _states = new();
        private Timer? _timer;
        private readonly CancellationTokenSource _cts = new();

        public AutomationRuleManager(SimpleFileServerNodeManager nodeManager)
        {
            _nodeManager = nodeManager;
        }

        public void Initialize(List<AutomationRule> rules)
        {
            foreach (var rule in rules)
            {
                DiagnosticsCollector.Instance.Register("AutomationRule", rule.Name, rule.Enabled);
                if (rule.Enabled)
                    _states.Add(new RuleState(rule));
            }

            if (_states.Count > 0)
            {
                _timer = new Timer(Tick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                Log.Information("AutomationRuleManager initialized with {Count} rule(s).", _states.Count);
            }
        }

        // ─── Timer tick ─────────────────────────────────────────────────────────

        private void Tick(object? _)
        {
            if (_cts.IsCancellationRequested) return;

            var now = DateTime.Now;
            foreach (var state in _states)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                try
                {
                    EvaluateRule(state, now);
                    sw.Stop();
                    DiagnosticsCollector.Instance.RecordCycle("AutomationRule", state.Rule.Name, sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    Log.Error(ex, "AutomationRule '{Name}' tick error: {Error}", state.Rule.Name, ex.Message);
                    DiagnosticsCollector.Instance.RecordCycle("AutomationRule", state.Rule.Name, sw.Elapsed.TotalMilliseconds, error: ex.Message);
                }
            }
        }

        private void EvaluateRule(RuleState state, DateTime now)
        {
            bool conditionMet = EvaluateConditions(state.Rule, now);

            if (conditionMet && !state.WasActive)
            {
                // Rising edge — check cooldown
                if (state.Rule.CooldownSeconds > 0 && state.LastFiredAt.HasValue)
                {
                    var elapsed = (now - state.LastFiredAt.Value).TotalSeconds;
                    if (elapsed < state.Rule.CooldownSeconds)
                        return;
                }

                state.WasActive = true;
                state.LastFiredAt = now;
                Log.Debug("AutomationRule '{Name}' fired (THEN actions).", state.Rule.Name);
                ExecuteActions(state.Rule.Actions, state.Rule.Name, "THEN");
            }
            else if (!conditionMet && state.WasActive)
            {
                // Falling edge
                state.WasActive = false;
                if (state.Rule.ClearActions.Count > 0)
                {
                    Log.Debug("AutomationRule '{Name}' cleared (OTHERWISE actions).", state.Rule.Name);
                    ExecuteActions(state.Rule.ClearActions, state.Rule.Name, "OTHERWISE");
                }
            }
        }

        // ─── Condition evaluation ────────────────────────────────────────────────

        private bool EvaluateConditions(AutomationRule rule, DateTime now)
        {
            if (rule.Conditions.Count == 0) return false;

            bool useAnd = !rule.ConditionLogic.Equals("Any", StringComparison.OrdinalIgnoreCase);

            foreach (var cond in rule.Conditions)
            {
                bool result = EvaluateCondition(cond, now);
                if (useAnd && !result) return false;
                if (!useAnd && result) return true;
            }

            return useAnd; // AND: all passed; OR: none passed
        }

        private bool EvaluateCondition(AutomationCondition cond, DateTime now)
        {
            try
            {
                return cond.Type switch
                {
                    "VariableThreshold" => EvalThreshold(cond),
                    "VariableChange"    => EvalChange(cond),
                    "AlarmActive"       => EvalAlarmActive(cond),
                    "TimeOfDay"         => EvalTimeOfDay(cond, now),
                    _                   => false
                };
            }
            catch (Exception ex)
            {
                Log.Warning("AutomationRule condition eval error ({Type}, {Path}): {Error}",
                    cond.Type, cond.VariablePath, ex.Message);
                return false;
            }
        }

        private bool EvalThreshold(AutomationCondition cond)
        {
            if (string.IsNullOrEmpty(cond.VariablePath)) return false;

            var rawLeft = _nodeManager.ReadVariable(cond.VariablePath);
            if (!TryParseDouble(rawLeft, out var left)) return false;

            double right;
            if (!string.IsNullOrEmpty(cond.CompareVariablePath))
            {
                var rawRight = _nodeManager.ReadVariable(cond.CompareVariablePath);
                if (!TryParseDouble(rawRight, out right)) return false;
            }
            else
            {
                if (!TryParseDouble(cond.ThresholdValue, out right)) return false;
            }

            return cond.Operator switch
            {
                "==" => Math.Abs(left - right) < 1e-9,
                "!=" => Math.Abs(left - right) >= 1e-9,
                ">"  => left > right,
                ">=" => left >= right,
                "<"  => left < right,
                "<=" => left <= right,
                _    => false
            };
        }

        private bool EvalChange(AutomationCondition cond)
        {
            if (string.IsNullOrEmpty(cond.VariablePath)) return false;

            var raw = _nodeManager.ReadVariable(cond.VariablePath)?.ToString() ?? "";

            if (!_lastValues.TryGetValue(cond.VariablePath, out var prev))
            {
                _lastValues[cond.VariablePath] = raw;
                return false;
            }

            bool changed = !string.Equals(prev, raw, StringComparison.Ordinal);
            _lastValues[cond.VariablePath] = raw;
            return changed;
        }

        private bool EvalAlarmActive(AutomationCondition cond)
        {
            if (string.IsNullOrEmpty(cond.VariablePath)) return false;
            // Alarm active is stored as the alarm variable being non-zero / "Active"
            var raw = _nodeManager.ReadVariable(cond.VariablePath);
            if (raw == null) return false;
            var s = raw.ToString() ?? "";
            if (bool.TryParse(s, out var b)) return b;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return d != 0;
            return s.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
                   s.Equals("1", StringComparison.Ordinal);
        }

        private static bool EvalTimeOfDay(AutomationCondition cond, DateTime now)
        {
            if (!TimeSpan.TryParse(cond.TimeStart, out var start)) return false;
            if (!TimeSpan.TryParse(cond.TimeEnd, out var end)) return false;

            var tod = now.TimeOfDay;
            return start <= end
                ? tod >= start && tod < end           // normal range e.g. 08:00-17:00
                : tod >= start || tod < end;           // overnight range e.g. 22:00-06:00
        }

        // ─── Action execution ────────────────────────────────────────────────────

        private void ExecuteActions(List<AutomationAction> actions, string ruleName, string phase)
        {
            foreach (var action in actions)
            {
                try
                {
                    ExecuteAction(action, ruleName, phase);
                }
                catch (Exception ex)
                {
                    Log.Warning("AutomationRule '{Rule}' {Phase} action '{Type}' failed: {Error}",
                        ruleName, phase, action.Type, ex.Message);
                }
            }
        }

        private void ExecuteAction(AutomationAction action, string ruleName, string phase)
        {
            switch (action.Type)
            {
                case "WriteVariable":
                    if (!string.IsNullOrEmpty(action.VariablePath))
                        _nodeManager.WriteVariable(action.VariablePath, action.Value ?? "");
                    break;

                case "SendNotification":
                    _nodeManager.NotificationService?.SendNotification(
                        title:   $"Rule: {ruleName}",
                        message: action.Message,
                        severity: 500);
                    break;

                case "ActivateRecipe":
                    if (!string.IsNullOrEmpty(action.RecipeName))
                        _nodeManager.RecipeManager?.ActivateRecipe(action.RecipeName);
                    break;

                case "ExecuteScript":
                    if (!string.IsNullOrEmpty(action.ScriptName))
                        _nodeManager.ScriptManager?.TriggerByName(action.ScriptName);
                    break;

                case "LogEvent":
                    _nodeManager.EventLogger?.LogSystem(
                        severity: "Info",
                        source:   $"Rule:{ruleName}",
                        message:  string.IsNullOrEmpty(action.Message) ? $"Rule '{ruleName}' {phase}" : action.Message);
                    break;

                default:
                    Log.Warning("AutomationRule '{Rule}': unknown action type '{Type}'.", ruleName, action.Type);
                    break;
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private readonly Dictionary<string, string> _lastValues = new(StringComparer.OrdinalIgnoreCase);

        private static bool TryParseDouble(object? raw, out double value)
        {
            value = 0;
            if (raw == null) return false;
            var s = raw.ToString() ?? "";
            if (bool.TryParse(s, out var b)) { value = b ? 1 : 0; return true; }
            return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _timer?.Dispose();
            _cts.Dispose();
        }

        // ─── State tracking per rule ─────────────────────────────────────────────

        private sealed class RuleState
        {
            public AutomationRule Rule { get; }
            public bool WasActive { get; set; }
            public DateTime? LastFiredAt { get; set; }

            public RuleState(AutomationRule rule)
            {
                Rule = rule;
            }
        }
    }
}
