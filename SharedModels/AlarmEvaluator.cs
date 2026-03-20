using System.Globalization;

namespace SharedModels;

/// <summary>
/// Pure-logic alarm evaluation helper. All methods are static and have no OPC UA dependencies,
/// making them easily testable.
/// </summary>
public static class AlarmEvaluator
{
    /// <summary>Result of a limit alarm evaluation.</summary>
    public sealed class LimitResult
    {
        public bool ShouldActivate { get; init; }
        public bool ShouldDeactivate { get; init; }
        public ushort Severity { get; init; }
        public string LimitState { get; init; } = "";
        public string LimitText { get; init; } = "";
    }

    /// <summary>
    /// Evaluate a limit alarm for the given numeric value against the alarm configuration.
    /// Returns activation/deactivation decision, severity, and descriptive text.
    /// </summary>
    public static LimitResult? EvaluateLimitAlarm(object? value, AlarmConfig cfg, bool isCurrentlyActive)
    {
        double? numericValue = GetNumericValue(value);
        if (numericValue == null) return null;

        double val = numericValue.Value;
        double highHigh = cfg.HighHighLimit ?? cfg.HighLimit;
        double lowLow = cfg.LowLowLimit ?? cfg.LowLimit;
        double hyst = cfg.Hysteresis;

        bool shouldActivate = val > cfg.HighLimit || val < cfg.LowLimit;
        bool shouldDeactivate = val <= (cfg.HighLimit - hyst) && val >= (cfg.LowLimit + hyst);

        ushort severity = 0;
        string limitState = "";
        string limitText = "";

        if (shouldActivate && !isCurrentlyActive)
        {
            if (val >= highHigh)
            {
                severity = 900;
                limitState = "HighHigh";
                limitText = $"High-High limit ({highHigh}) exceeded";
            }
            else if (val > cfg.HighLimit)
            {
                severity = 700;
                limitState = "High";
                limitText = $"High limit ({cfg.HighLimit}) exceeded";
            }
            else if (val <= lowLow)
            {
                severity = 900;
                limitState = "LowLow";
                limitText = $"Low-Low limit ({lowLow}) violated";
            }
            else
            {
                severity = 500;
                limitState = "Low";
                limitText = $"Low limit ({cfg.LowLimit}) violated";
            }
        }

        return new LimitResult
        {
            ShouldActivate = shouldActivate && !isCurrentlyActive,
            ShouldDeactivate = shouldDeactivate && isCurrentlyActive,
            Severity = severity,
            LimitState = limitState,
            LimitText = limitText
        };
    }

    /// <summary>
    /// Evaluate a condition alarm for the given value.
    /// </summary>
    public static bool EvaluateConditionActivation(string op, string compareValue, object? value)
    {
        return EvaluateConditionExpression(op, compareValue, value);
    }

    /// <summary>
    /// Evaluate whether a condition alarm should deactivate with hysteresis.
    /// </summary>
    public static bool EvaluateConditionDeactivation(string op, string compareValue, double hysteresis, object? value)
    {
        double? numericValue = GetNumericValue(value);
        if (!numericValue.HasValue || !double.TryParse(compareValue, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var target))
            return !EvaluateConditionExpression(op, compareValue, value);

        double v = numericValue.Value;
        return op switch
        {
            ">" => v <= target - hysteresis,
            ">=" => v < target - hysteresis,
            "<" => v >= target + hysteresis,
            "<=" => v > target + hysteresis,
            "==" => Math.Abs(v - target) > hysteresis,
            "!=" => Math.Abs(v - target) <= hysteresis,
            _ => !EvaluateConditionExpression(op, compareValue, value)
        };
    }

    /// <summary>
    /// Attempts to convert a value to a double for numeric comparison.
    /// </summary>
    public static double? GetNumericValue(object? value)
    {
        return value switch
        {
            double d => d,
            float f => f,
            int i => i,
            short s => s,
            ushort us => us,
            uint ui => ui,
            long l => l,
            ulong ul => ul,
            decimal m => (double)m,
            byte b => b,
            sbyte sb => sb,
            string str when double.TryParse(str, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var parsed) => parsed,
            _ => null
        };
    }

    /// <summary>
    /// Evaluates a condition expression against a variable value.
    /// Supports operators: ==, !=, &gt;, &gt;=, &lt;, &lt;=, True, False.
    /// </summary>
    public static bool EvaluateConditionExpression(string op, string compareValue, object? value)
    {
        if (string.Equals(op, "True", StringComparison.OrdinalIgnoreCase))
        {
            return value switch
            {
                bool b => b,
                int i => i != 0,
                double d => d != 0,
                string s => string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) || s == "1",
                _ => false
            };
        }

        if (string.Equals(op, "False", StringComparison.OrdinalIgnoreCase))
        {
            return value switch
            {
                bool b => !b,
                int i => i == 0,
                double d => d == 0,
                string s => string.Equals(s, "false", StringComparison.OrdinalIgnoreCase) || s == "0" || string.IsNullOrEmpty(s),
                _ => true
            };
        }

        double? numericValue = GetNumericValue(value);
        if (numericValue.HasValue && double.TryParse(compareValue, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var numericCompare))
        {
            double v = numericValue.Value;
            return op switch
            {
                "==" => Math.Abs(v - numericCompare) < 1e-10,
                "!=" => Math.Abs(v - numericCompare) >= 1e-10,
                ">" => v > numericCompare,
                ">=" => v >= numericCompare,
                "<" => v < numericCompare,
                "<=" => v <= numericCompare,
                _ => false
            };
        }

        string strValue = value?.ToString() ?? "";
        return op switch
        {
            "==" => string.Equals(strValue, compareValue, StringComparison.OrdinalIgnoreCase),
            "!=" => !string.Equals(strValue, compareValue, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }
}
