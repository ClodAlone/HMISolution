// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Xunit;
using SharedModels;

namespace Tests.Server;

public class AlarmEvaluatorTests
{
    // ──────────────────────────────────────────────────────────────
    // GetNumericValue
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(42.0, 42.0)]
    [InlineData(42f, 42.0)]
    [InlineData(42, 42.0)]
    [InlineData((short)42, 42.0)]
    [InlineData((ushort)42, 42.0)]
    [InlineData(42u, 42.0)]
    [InlineData(42L, 42.0)]
    [InlineData((ulong)42, 42.0)]
    [InlineData((byte)42, 42.0)]
    [InlineData((sbyte)42, 42.0)]
    public void GetNumericValue_NumericTypes_ReturnsDouble(object input, double expected)
    {
        Assert.Equal(expected, AlarmEvaluator.GetNumericValue(input));
    }

    [Fact]
    public void GetNumericValue_DecimalType_ReturnsDouble()
    {
        Assert.Equal(42.5, AlarmEvaluator.GetNumericValue(42.5m));
    }

    [Theory]
    [InlineData("100.0", 100.0)]
    [InlineData("3.14", 3.14)]
    [InlineData("-50", -50.0)]
    [InlineData("0", 0.0)]
    public void GetNumericValue_StringParseable_ReturnsDouble(string input, double expected)
    {
        Assert.Equal(expected, AlarmEvaluator.GetNumericValue(input));
    }

    [Theory]
    [InlineData("hello")]
    [InlineData("")]
    public void GetNumericValue_NonNumericString_ReturnsNull(string input)
    {
        Assert.Null(AlarmEvaluator.GetNumericValue(input));
    }

    [Fact]
    public void GetNumericValue_Null_ReturnsNull()
    {
        Assert.Null(AlarmEvaluator.GetNumericValue(null));
    }

    [Fact]
    public void GetNumericValue_UnsupportedType_ReturnsNull()
    {
        Assert.Null(AlarmEvaluator.GetNumericValue(new object()));
    }

    // ──────────────────────────────────────────────────────────────
    // EvaluateLimitAlarm — Activation
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void LimitAlarm_ValueAboveHighLimit_Activates()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(100.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.True(result.ShouldActivate);
        Assert.False(result.ShouldDeactivate);
        Assert.Equal("HighHigh", result.LimitState);
        Assert.Equal((ushort)900, result.Severity);
    }

    [Fact]
    public void LimitAlarm_ValueAboveHighHighLimit_Activates_Critical()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20, HighHighLimit = 95 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(100.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.True(result.ShouldActivate);
        Assert.Equal("HighHigh", result.LimitState);
        Assert.Equal((ushort)900, result.Severity);
    }

    [Fact]
    public void LimitAlarm_ValueBelowLowLimit_Activates()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(10.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.True(result.ShouldActivate);
        Assert.Equal("LowLow", result.LimitState);
        Assert.Equal((ushort)900, result.Severity);
    }

    [Fact]
    public void LimitAlarm_ValueBelowLowLowLimit_Activates_Critical()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20, LowLowLimit = 5 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(3.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.True(result.ShouldActivate);
        Assert.Equal("LowLow", result.LimitState);
        Assert.Equal((ushort)900, result.Severity);
    }

    [Fact]
    public void LimitAlarm_ValueWithinLimits_DoesNotActivate()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(50.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.False(result.ShouldActivate);
    }

    [Fact]
    public void LimitAlarm_ValueExactlyAtHighLimit_DoesNotActivate()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(80.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.False(result.ShouldActivate);
    }

    [Fact]
    public void LimitAlarm_ValueExactlyAtLowLimit_DoesNotActivate()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(20.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.False(result.ShouldActivate);
    }

    [Fact]
    public void LimitAlarm_AlreadyActive_DoesNotActivateAgain()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(100.0, cfg, isCurrentlyActive: true);

        Assert.NotNull(result);
        Assert.False(result.ShouldActivate);
    }

    // ──────────────────────────────────────────────────────────────
    // EvaluateLimitAlarm — Deactivation
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void LimitAlarm_ValueReturnsToNormal_Deactivates()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(50.0, cfg, isCurrentlyActive: true);

        Assert.NotNull(result);
        Assert.True(result.ShouldDeactivate);
    }

    [Fact]
    public void LimitAlarm_ValueReturnsToNormal_WithHysteresis_DoesNotDeactivate()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20, Hysteresis = 5 };
        // Value just below HighLimit but not past hysteresis band (80-5=75)
        var result = AlarmEvaluator.EvaluateLimitAlarm(78.0, cfg, isCurrentlyActive: true);

        Assert.NotNull(result);
        Assert.False(result.ShouldDeactivate);
    }

    [Fact]
    public void LimitAlarm_ValuePastHysteresis_Deactivates()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20, Hysteresis = 5 };
        // Value past hysteresis band: 80 - 5 = 75, and 50 <= 75
        var result = AlarmEvaluator.EvaluateLimitAlarm(50.0, cfg, isCurrentlyActive: true);

        Assert.NotNull(result);
        Assert.True(result.ShouldDeactivate);
    }

    [Fact]
    public void LimitAlarm_NotActive_DoesNotDeactivate()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(50.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.False(result.ShouldDeactivate);
    }

    // ──────────────────────────────────────────────────────────────
    // EvaluateLimitAlarm — String input (simulating runtime write)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void LimitAlarm_StringValue_ParsedAndEvaluated()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm("100.0", cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.True(result.ShouldActivate);
    }

    [Fact]
    public void LimitAlarm_NonNumericValue_ReturnsNull()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm("hello", cfg, isCurrentlyActive: false);

        Assert.Null(result);
    }

    [Fact]
    public void LimitAlarm_NullValue_ReturnsNull()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(null, cfg, isCurrentlyActive: false);

        Assert.Null(result);
    }

    // ──────────────────────────────────────────────────────────────
    // EvaluateLimitAlarm — Integer types (simulating typed values)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void LimitAlarm_IntValue_AboveHighLimit_Activates()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(100, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.True(result.ShouldActivate);
    }

    [Fact]
    public void LimitAlarm_FloatValue_AboveHighLimit_Activates()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(100.0f, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.True(result.ShouldActivate);
    }

    // ──────────────────────────────────────────────────────────────
    // EvaluateConditionExpression
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(">", "50", 100.0, true)]
    [InlineData(">", "50", 30.0, false)]
    [InlineData(">=", "50", 50.0, true)]
    [InlineData("<", "50", 30.0, true)]
    [InlineData("<", "50", 70.0, false)]
    [InlineData("<=", "50", 50.0, true)]
    [InlineData("==", "50", 50.0, true)]
    [InlineData("==", "50", 51.0, false)]
    [InlineData("!=", "50", 51.0, true)]
    [InlineData("!=", "50", 50.0, false)]
    public void ConditionExpression_NumericOperators(string op, string compare, double value, bool expected)
    {
        Assert.Equal(expected, AlarmEvaluator.EvaluateConditionExpression(op, compare, value));
    }

    [Theory]
    [InlineData("True", true, true)]
    [InlineData("True", false, false)]
    [InlineData("False", false, true)]
    [InlineData("False", true, false)]
    public void ConditionExpression_BooleanOperators(string op, bool value, bool expected)
    {
        Assert.Equal(expected, AlarmEvaluator.EvaluateConditionExpression(op, "", value));
    }

    [Fact]
    public void ConditionExpression_StringEquality()
    {
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("==", "hello", "hello"));
        Assert.False(AlarmEvaluator.EvaluateConditionExpression("==", "hello", "world"));
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("!=", "hello", "world"));
    }

    // ──────────────────────────────────────────────────────────────
    // EvaluateConditionDeactivation (with hysteresis)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void ConditionDeactivation_GreaterThan_WithHysteresis()
    {
        // Activated at value > 50. Deactivate when value <= 50 - 5 = 45
        Assert.True(AlarmEvaluator.EvaluateConditionDeactivation(">", "50", 5.0, 44.0));
        Assert.False(AlarmEvaluator.EvaluateConditionDeactivation(">", "50", 5.0, 48.0));
    }

    [Fact]
    public void ConditionDeactivation_LessThan_WithHysteresis()
    {
        // Activated at value < 50. Deactivate when value >= 50 + 5 = 55
        Assert.True(AlarmEvaluator.EvaluateConditionDeactivation("<", "50", 5.0, 56.0));
        Assert.False(AlarmEvaluator.EvaluateConditionDeactivation("<", "50", 5.0, 52.0));
    }

    // ──────────────────────────────────────────────────────────────
    // Full alarm cycle: activate → stay active → deactivate
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void LimitAlarm_FullCycle_ActivateStayDeactivate()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20, Hysteresis = 5 };

        // Step 1: Value at 50 — normal, no alarm
        var r1 = AlarmEvaluator.EvaluateLimitAlarm(50.0, cfg, isCurrentlyActive: false);
        Assert.NotNull(r1);
        Assert.False(r1.ShouldActivate);
        Assert.False(r1.ShouldDeactivate);

        // Step 2: Value goes to 85 — should activate (high alarm)
        var r2 = AlarmEvaluator.EvaluateLimitAlarm(85.0, cfg, isCurrentlyActive: false);
        Assert.NotNull(r2);
        Assert.True(r2.ShouldActivate);
        // No HighHighLimit set, so defaults to HighHigh
        Assert.Equal("HighHigh", r2.LimitState);
        Assert.Equal((ushort)900, r2.Severity);

        // Step 3: Value drops to 78 — still above hysteresis band (80-5=75), stays active
        var r3 = AlarmEvaluator.EvaluateLimitAlarm(78.0, cfg, isCurrentlyActive: true);
        Assert.NotNull(r3);
        Assert.False(r3.ShouldActivate);
        Assert.False(r3.ShouldDeactivate);

        // Step 4: Value drops to 70 — below hysteresis band (75), should deactivate
        var r4 = AlarmEvaluator.EvaluateLimitAlarm(70.0, cfg, isCurrentlyActive: true);
        Assert.NotNull(r4);
        Assert.True(r4.ShouldDeactivate);
    }

    [Fact]
    public void LimitAlarm_FullCycle_LowAlarm()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20, Hysteresis = 3 };

        // Step 1: Value goes to 15 — should activate (low alarm)
        var r1 = AlarmEvaluator.EvaluateLimitAlarm(15.0, cfg, isCurrentlyActive: false);
        Assert.NotNull(r1);
        Assert.True(r1.ShouldActivate);
        // No LowLowLimit set, so defaults to LowLow
        Assert.Equal("LowLow", r1.LimitState);

        // Step 2: Value rises to 21 — still within hysteresis band (20+3=23), stays active
        var r2 = AlarmEvaluator.EvaluateLimitAlarm(21.0, cfg, isCurrentlyActive: true);
        Assert.NotNull(r2);
        Assert.False(r2.ShouldDeactivate);

        // Step 3: Value rises to 25 — past hysteresis band, should deactivate
        var r3 = AlarmEvaluator.EvaluateLimitAlarm(25.0, cfg, isCurrentlyActive: true);
        Assert.NotNull(r3);
        Assert.True(r3.ShouldDeactivate);
    }

    // ──────────────────────────────────────────────────────────────
    // AlarmDemo scenario: Temperature with HighLimit=80, LowLimit=20
    // Simulates the exact use case of slider writing "100.0"
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void AlarmDemo_Temperature100_ShouldTriggerHighAlarm()
    {
        // This simulates the AlarmDemo scenario exactly:
        // - AlarmConfig: HighLimit=80, LowLimit=20, TriggerType=Limit
        // - User moves slider to 100
        // - HandleWriteValue converts string "100.0" to double 100.0
        // - Value property is set to 100.0 (double)
        // - EvaluateLimitAlarm is called with the double value
        var cfg = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 80,
            LowLimit = 20,
            Hysteresis = 0,
            Message = "Temperature alarm"
        };

        // First verify that 100.0 (double) triggers the alarm
        var result = AlarmEvaluator.EvaluateLimitAlarm(100.0, cfg, isCurrentlyActive: false);
        Assert.NotNull(result);
        Assert.True(result.ShouldActivate, "100.0 (double) should trigger high alarm");
        // When HighHighLimit is not set, it defaults to HighLimit, so any value above HighLimit is HighHigh
        Assert.Equal("HighHigh", result.LimitState);
        Assert.Equal((ushort)900, result.Severity);

        // Also verify that the string "100.0" triggers (in case GetNumericValue handles it)
        var resultStr = AlarmEvaluator.EvaluateLimitAlarm("100.0", cfg, isCurrentlyActive: false);
        Assert.NotNull(resultStr);
        Assert.True(resultStr.ShouldActivate, "string 100.0 should trigger high alarm");
    }

    [Fact]
    public void AlarmDemo_Temperature5_ShouldTriggerLowAlarm()
    {
        var cfg = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 80,
            LowLimit = 20,
            Hysteresis = 0,
            Message = "Temperature alarm"
        };

        var result = AlarmEvaluator.EvaluateLimitAlarm(5.0, cfg, isCurrentlyActive: false);
        Assert.NotNull(result);
        Assert.True(result.ShouldActivate, "5.0 should trigger low alarm");
        // No LowLowLimit set, so defaults to LowLow
        Assert.Equal("LowLow", result.LimitState);
    }

    [Fact]
    public void AlarmDemo_Temperature50_ShouldNotTrigger()
    {
        var cfg = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 80,
            LowLimit = 20,
            Hysteresis = 0,
            Message = "Temperature alarm"
        };

        var result = AlarmEvaluator.EvaluateLimitAlarm(50.0, cfg, isCurrentlyActive: false);
        Assert.NotNull(result);
        Assert.False(result.ShouldActivate, "50.0 should not trigger alarm (within limits)");
    }
}
