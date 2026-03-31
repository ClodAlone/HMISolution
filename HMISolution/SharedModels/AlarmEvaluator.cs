using System;
using System.Globalization;

namespace SharedModels;

public static class AlarmEvaluator
{
    public sealed class LimitAlarmResult
    {
        public bool ShouldActivate { get; init; }
        public bool ShouldDeactivate { get; init; }
        public ushort Severity { get; init; }
        public string LimitState { get; init; } = "";
        public string LimitText { get; init; } = "";
    }

    public static LimitAlarmResult? EvaluateLimitAlarm(object? value, AlarmConfig config, bool isCurrentlyActive)
    {
        var numericValue = GetNumericValue(value);
        if (numericValue == null) return null;
        double v = numericValue.Value;
        double hyst = config.Hysteresis;
        double highHigh = config.HighHighLimit ?? config.HighLimit;
        double lowLow = config.LowLowLimit ?? config.LowLimit;

        bool shouldActivate = false;
        bool shouldDeactivate = false;
        ushort severity = 0;
        string limitState = "";
        string limitText = "";

        if (!isCurrentlyActive)
        {
            if (v >= highHigh && config.HighHighLimit.HasValue)
            {
                shouldActivate = true;
                severity = 900;
                limitState = "HighHigh";
                limitText = $"High-High limit ({highHigh}) exceeded";
            }
            else if (v >= config.HighLimit)
            {
                shouldActivate = true;
                severity = 700;
                limitState = "High";
                limitText = $"High limit ({config.HighLimit}) exceeded";
            }

            if (v <= lowLow && config.LowLowLimit.HasValue)
            {
                shouldActivate = true;
                severity = 900;
                limitState = "LowLow";
                limitText = $"Low-Low limit ({lowLow}) violated";
            }
            else if (v <= config.LowLimit)
            {
                shouldActivate = true;
                severity = 500;
                limitState = "Low";
                limitText = $"Low limit ({config.LowLimit}) violated";
            }
        }
        else
        {
            bool inHighZone = v >= (config.HighLimit - hyst);
            bool inLowZone = v <= (config.LowLimit + hyst);
            if (config.HighHighLimit.HasValue) inHighZone = inHighZone || v >= (config.HighHighLimit.Value - hyst);
            if (config.LowLowLimit.HasValue) inLowZone = inLowZone || v <= (config.LowLowLimit.Value + hyst);
            shouldDeactivate = !inHighZone && !inLowZone;
        }
        return new LimitAlarmResult { ShouldActivate = shouldActivate, ShouldDeactivate = shouldDeactivate, Severity = severity, LimitState = limitState, LimitText = limitText };
    }

    public static double? GetNumericValue(object? value)
    {
        if (value == null) return null;
        try { return Convert.ToDouble(value, CultureInfo.InvariantCulture); }
        catch { return null; }
    }

    public static bool EvaluateConditionActivation(string op, string compareValue, object? currentValue)
    {
        if (string.Equals(op, "True", StringComparison.OrdinalIgnoreCase)) return IsTruthy(currentValue);
        if (string.Equals(op, "False", StringComparison.OrdinalIgnoreCase)) return !IsTruthy(currentValue);
        if (string.Equals(op, "Changed", StringComparison.OrdinalIgnoreCase)) return true;
        var numCurrent = GetNumericValue(currentValue);
        var numCompare = double.TryParse(compareValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var cv) ? (double?)cv : null;
        if (numCurrent != null && numCompare != null)
        {
            return op switch
            {
                "==" => Math.Abs(numCurrent.Value - numCompare.Value) < 1e-12,
                "!=" => Math.Abs(numCurrent.Value - numCompare.Value) >= 1e-12,
                ">" => numCurrent.Value > numCompare.Value,
                ">=" => numCurrent.Value >= numCompare.Value,
                "<" => numCurrent.Value < numCompare.Value,
                "<=" => numCurrent.Value <= numCompare.Value,
                _ => false
            };
        }
        string sCurrent = currentValue?.ToString() ?? "";
        return op switch
        {
            "==" => string.Equals(sCurrent, compareValue, StringComparison.Ordinal),
            "!=" => !string.Equals(sCurrent, compareValue, StringComparison.Ordinal),
            _ => false
        };
    }
    public static bool EvaluateConditionDeactivation(string op, string compareValue, double hysteresis, object? currentValue)
    {
        var numCurrent = GetNumericValue(currentValue);
        var numCompare = double.TryParse(compareValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var cv) ? (double?)cv : null;
        if (numCurrent == null || numCompare == null)
            return !EvaluateConditionActivation(op, compareValue, currentValue);
        double v = numCurrent.Value;
        double c = numCompare.Value;
        return op switch
        {
            ">" => v <= c - hysteresis,
            ">=" => v < c - hysteresis,
            "<" => v >= c + hysteresis,
            "<=" => v > c + hysteresis,
            "==" => Math.Abs(v - c) >= hysteresis,
            "!=" => Math.Abs(v - c) < hysteresis,
            _ => !EvaluateConditionActivation(op, compareValue, currentValue)
        };
    }

    private static bool IsTruthy(object? value)
    {
        if (value == null) return false;
        if (value is bool b) return b;
        var numVal = GetNumericValue(value);
        if (numVal != null) return numVal.Value != 0;
        return !string.IsNullOrEmpty(value.ToString());
    }
}
