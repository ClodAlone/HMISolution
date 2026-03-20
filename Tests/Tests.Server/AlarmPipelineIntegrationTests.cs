using Opc.Ua;
using SharedModels;
using Xunit;

namespace Tests.Server;

/// <summary>
/// Integration tests that verify the OPC UA alarm pipeline works end-to-end
/// using real SDK classes (BaseDataVariableState, ExclusiveLimitAlarmState, etc.)
/// without needing a full running server.
/// </summary>
public class AlarmPipelineIntegrationTests
{
    /// <summary>
    /// This test class verifies that:
    /// 1. Setting Value on a BaseDataVariableState sets ChangeMasks
    /// 2. ClearChangeMasks fires OnStateChanged when ChangeMasks != None
    /// 3. The alarm evaluation logic correctly activates/deactivates alarms
    /// 4. The full Write → Value → ClearChangeMasks → OnStateChanged → EvaluateAlarm chain works
    /// </summary>

    [Fact]
    public void Value_Setter_Sets_ChangeMasks()
    {
        // Arrange: Create a variable with an initial double value
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Test.Var", 2),
            BrowseName = new QualifiedName("Var", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0
        };

        // Clear any initial ChangeMasks from construction
        variable.ClearChangeMasks(null, false);

        // Act: Set a new value
        variable.Value = 100.0;

        // Assert: ChangeMasks should include Value
        Assert.True((variable.ChangeMasks & NodeStateChangeMasks.Value) != 0,
            "Setting Value should set ChangeMasks.Value");
    }

    [Fact]
    public void Value_Setter_Same_Boxed_Value_Does_Not_Set_ChangeMasks()
    {
        // Arrange: Create a variable and assign the SAME boxed reference
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Test.Var", 2),
            BrowseName = new QualifiedName("Var", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar
        };

        object boxedValue = 50.0;
        variable.Value = boxedValue;
        variable.ClearChangeMasks(null, false);

        // Act: Set the SAME boxed reference again
        variable.Value = boxedValue;

        // Assert: ChangeMasks should NOT include Value (same reference)
        Assert.True((variable.ChangeMasks & NodeStateChangeMasks.Value) == 0,
            "Setting the same boxed reference should NOT set ChangeMasks.Value");
    }

    [Fact]
    public void Value_Setter_New_Boxed_Value_Sets_ChangeMasks()
    {
        // Arrange
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Test.Var", 2),
            BrowseName = new QualifiedName("Var", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0
        };
        variable.ClearChangeMasks(null, false);

        // Act: Set a NEW boxed double (different reference even if same value)
        variable.Value = 50.0; // This creates a new boxed double

        // Assert: ChangeMasks SHOULD include Value because ReferenceEquals is false
        Assert.True((variable.ChangeMasks & NodeStateChangeMasks.Value) != 0,
            "Setting a new boxed value (even same numeric value) should set ChangeMasks.Value");
    }

    [Fact]
    public void ClearChangeMasks_Fires_OnStateChanged_When_Masks_Not_None()
    {
        // Arrange
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Test.Var", 2),
            BrowseName = new QualifiedName("Var", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0
        };
        variable.ClearChangeMasks(null, false);

        bool stateChangedFired = false;
        NodeStateChangeMasks capturedMasks = NodeStateChangeMasks.None;
        variable.OnStateChanged += (context, state, masks) =>
        {
            stateChangedFired = true;
            capturedMasks = masks;
        };

        // Act: Set value then clear
        variable.Value = 100.0;
        variable.ClearChangeMasks(null, false);

        // Assert
        Assert.True(stateChangedFired, "OnStateChanged should fire after Value change + ClearChangeMasks");
        Assert.True((capturedMasks & NodeStateChangeMasks.Value) != 0,
            "OnStateChanged masks should include Value");
    }

    [Fact]
    public void ClearChangeMasks_Does_Not_Fire_When_Masks_None()
    {
        // Arrange
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Test.Var", 2),
            BrowseName = new QualifiedName("Var", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0
        };
        variable.ClearChangeMasks(null, false); // Clear initial masks

        bool stateChangedFired = false;
        variable.OnStateChanged += (context, state, masks) =>
        {
            stateChangedFired = true;
        };

        // Act: Clear again without any changes
        variable.ClearChangeMasks(null, false);

        // Assert
        Assert.False(stateChangedFired, "OnStateChanged should NOT fire when ChangeMasks is None");
    }

    [Fact]
    public void Timestamp_Change_Also_Sets_ChangeMasks()
    {
        // Arrange
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Test.Var", 2),
            BrowseName = new QualifiedName("Var", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0,
            Timestamp = DateTime.UtcNow
        };
        variable.ClearChangeMasks(null, false);

        bool stateChangedFired = false;
        variable.OnStateChanged += (context, state, masks) =>
        {
            stateChangedFired = true;
        };

        // Act: Only change Timestamp (not Value)
        variable.Timestamp = DateTime.UtcNow.AddSeconds(1);
        variable.ClearChangeMasks(null, false);

        // Assert: Timestamp change should also fire OnStateChanged
        Assert.True(stateChangedFired, "Timestamp change should fire OnStateChanged");
    }

    [Fact]
    public void Full_Write_To_Alarm_Pipeline_Activates_Alarm()
    {
        // Arrange: Create a variable with initial value within limits
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Process.Temperature", 2),
            BrowseName = new QualifiedName("Temperature", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0,
            Timestamp = DateTime.UtcNow,
            StatusCode = StatusCodes.Good
        };
        variable.ClearChangeMasks(null, false);

        // Set up alarm config matching AlarmDemo
        var alarmConfig = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 80,
            LowLimit = 20,
            HighHighLimit = 95,
            LowLowLimit = 5,
            Hysteresis = 2,
            Message = "Temperature alarm"
        };

        // Track alarm evaluations
        bool alarmEvaluated = false;
        bool alarmActivated = false;
        bool isActive = false;

        // Wire up OnStateChanged to simulate the alarm evaluation
        variable.OnStateChanged += (context, state, masks) =>
        {
            if ((masks & NodeStateChangeMasks.Value) != 0)
            {
                alarmEvaluated = true;
                var result = AlarmEvaluator.EvaluateLimitAlarm(variable.Value, alarmConfig, isActive);
                if (result != null && result.ShouldActivate)
                {
                    alarmActivated = true;
                    isActive = true;
                }
            }
        };

        // Act: Simulate what HandleWriteValue does
        // 1. Convert string "100" to double (as HandleWriteValue does for DataType=Double)
        string incoming = "100";
        double convertedValue = double.Parse(incoming, System.Globalization.CultureInfo.InvariantCulture);

        // 2. Set value, status, timestamp
        variable.Value = convertedValue;
        variable.StatusCode = StatusCodes.Good;
        variable.Timestamp = DateTime.UtcNow;

        // 3. ClearChangeMasks (fires OnStateChanged)
        variable.ClearChangeMasks(null, false);

        // Assert
        Assert.True(alarmEvaluated, "OnStateChanged should have fired and evaluated the alarm");
        Assert.True(alarmActivated, "Alarm should have been activated for value 100 > HighLimit 80");
        Assert.True(isActive, "Alarm should be marked as active");
    }

    [Fact]
    public void Full_Pipeline_Value_Within_Limits_Does_Not_Activate()
    {
        // Arrange
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Process.Temperature", 2),
            BrowseName = new QualifiedName("Temperature", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0
        };
        variable.ClearChangeMasks(null, false);

        var alarmConfig = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 80,
            LowLimit = 20,
            HighHighLimit = 95,
            LowLowLimit = 5,
            Hysteresis = 2
        };

        bool alarmActivated = false;
        bool isActive = false;

        variable.OnStateChanged += (context, state, masks) =>
        {
            if ((masks & NodeStateChangeMasks.Value) != 0)
            {
                var result = AlarmEvaluator.EvaluateLimitAlarm(variable.Value, alarmConfig, isActive);
                if (result != null && result.ShouldActivate)
                {
                    alarmActivated = true;
                    isActive = true;
                }
            }
        };

        // Act: Write value within limits
        variable.Value = 60.0;
        variable.StatusCode = StatusCodes.Good;
        variable.Timestamp = DateTime.UtcNow;
        variable.ClearChangeMasks(null, false);

        // Assert: Should NOT activate
        Assert.False(alarmActivated, "Alarm should NOT activate for value 60 (within limits 20-80)");
    }

    [Fact]
    public void Full_Pipeline_Activate_Then_Deactivate()
    {
        // Arrange
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Process.Temperature", 2),
            BrowseName = new QualifiedName("Temperature", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0
        };
        variable.ClearChangeMasks(null, false);

        var alarmConfig = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 80,
            LowLimit = 20,
            HighHighLimit = 95,
            LowLowLimit = 5,
            Hysteresis = 2
        };

        bool isActive = false;
        string lastEvent = "";

        variable.OnStateChanged += (context, state, masks) =>
        {
            if ((masks & NodeStateChangeMasks.Value) != 0)
            {
                var result = AlarmEvaluator.EvaluateLimitAlarm(variable.Value, alarmConfig, isActive);
                if (result != null)
                {
                    if (result.ShouldActivate)
                    {
                        isActive = true;
                        lastEvent = $"ACTIVATED: {result.LimitState} severity={result.Severity}";
                    }
                    else if (result.ShouldDeactivate)
                    {
                        isActive = false;
                        lastEvent = "DEACTIVATED";
                    }
                }
            }
        };

        // Act 1: Write value above HighHighLimit
        variable.Value = 100.0;
        variable.Timestamp = DateTime.UtcNow;
        variable.ClearChangeMasks(null, false);

        Assert.True(isActive, "Alarm should activate at 100");
        Assert.Contains("ACTIVATED", lastEvent);
        Assert.Contains("HighHigh", lastEvent);
        Assert.Contains("900", lastEvent);

        // Act 2: Write value back to normal (within hysteresis band — should NOT deactivate)
        variable.Value = 79.0; // HighLimit(80) - Hysteresis(2) = 78, so 79 > 78 => still in hysteresis band
        variable.Timestamp = DateTime.UtcNow;
        variable.ClearChangeMasks(null, false);

        Assert.True(isActive, "Alarm should still be active at 79 (within hysteresis band)");

        // Act 3: Write value past hysteresis
        variable.Value = 77.0; // 77 < 78 => past hysteresis, should deactivate
        variable.Timestamp = DateTime.UtcNow;
        variable.ClearChangeMasks(null, false);

        Assert.False(isActive, "Alarm should deactivate at 77 (past hysteresis band)");
        Assert.Equal("DEACTIVATED", lastEvent);
    }

    [Fact]
    public void Multiple_OnStateChanged_Handlers_All_Fire()
    {
        // Arrange: Simulate the actual code where BOTH data logging and alarm handlers are registered
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Process.Temperature", 2),
            BrowseName = new QualifiedName("Temperature", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0
        };
        variable.ClearChangeMasks(null, false);

        bool handler1Called = false;
        bool handler2Called = false;
        bool handler3Called = false;

        // Handler 1: Script notification (line 900 in server)
        variable.OnStateChanged += (context, state, masks) =>
        {
            if ((masks & NodeStateChangeMasks.Value) != 0)
                handler1Called = true;
        };

        // Handler 2: Data logging (line 1974 in server - inside ServerVariableState constructor)
        variable.OnStateChanged += (context, state, masks) =>
        {
            if ((masks & NodeStateChangeMasks.Value) != 0)
                handler2Called = true;
        };

        // Handler 3: Alarm evaluation (line 1377/1396 in server)
        variable.OnStateChanged += (context, state, masks) =>
        {
            if ((masks & NodeStateChangeMasks.Value) != 0)
                handler3Called = true;
        };

        // Act
        variable.Value = 100.0;
        variable.ClearChangeMasks(null, false);

        // Assert: ALL handlers should have fired
        Assert.True(handler1Called, "Handler 1 (script) should fire");
        Assert.True(handler2Called, "Handler 2 (logging) should fire");
        Assert.True(handler3Called, "Handler 3 (alarm) should fire");
    }

    [Fact]
    public void String_To_Double_Conversion_Then_Alarm()
    {
        // This simulates exactly what happens in the runtime:
        // Client sends string "100", HandleWriteValue converts to double, alarm evaluates
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Process.Temperature", 2),
            BrowseName = new QualifiedName("Temperature", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0
        };
        variable.ClearChangeMasks(null, false);

        var alarmConfig = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 80,
            LowLimit = 20,
            HighHighLimit = 95,
            LowLowLimit = 5,
            Hysteresis = 2
        };

        bool activated = false;
        bool isActive = false;
        ushort severity = 0;
        string limitState = "";

        variable.OnStateChanged += (context, state, masks) =>
        {
            if ((masks & NodeStateChangeMasks.Value) != 0)
            {
                var result = AlarmEvaluator.EvaluateLimitAlarm(variable.Value, alarmConfig, isActive);
                if (result != null && result.ShouldActivate)
                {
                    activated = true;
                    isActive = true;
                    severity = result.Severity;
                    limitState = result.LimitState;
                }
            }
        };

        // Simulate HandleWriteValue: incoming is string "100"
        object incoming = "100";
        // String-to-typed conversion (as in HandleWriteValue lines 2026-2040)
        if (incoming is string s && double.TryParse(s, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var d))
        {
            incoming = d;
        }

        // Set value (as in HandleWriteValue line 2044)
        variable.Value = incoming;
        variable.StatusCode = StatusCodes.Good;
        variable.Timestamp = DateTime.UtcNow;
        variable.ClearChangeMasks(null, false);

        // Assert
        Assert.True(activated, "Alarm should activate when writing '100' to Temperature with HighLimit=80");
        Assert.Equal((ushort)900, severity); // 100 >= HighHighLimit(95) => HighHigh severity 900
        Assert.Equal("HighHigh", limitState);
    }

    [Fact]
    public void Low_Alarm_From_String_Input()
    {
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Process.Temperature", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 50.0
        };
        variable.ClearChangeMasks(null, false);

        var config = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 80,
            LowLimit = 20,
            HighHighLimit = 95,
            LowLowLimit = 5,
            Hysteresis = 2
        };

        bool activated = false;
        bool isActive = false;
        string limitState = "";

        variable.OnStateChanged += (context, state, masks) =>
        {
            if ((masks & NodeStateChangeMasks.Value) != 0)
            {
                var result = AlarmEvaluator.EvaluateLimitAlarm(variable.Value, config, isActive);
                if (result != null && result.ShouldActivate)
                {
                    activated = true;
                    isActive = true;
                    limitState = result.LimitState;
                }
            }
        };

        // Write "3" → 3.0 < LowLowLimit(5) → LowLow alarm
        object incoming = "3";
        if (incoming is string s && double.TryParse(s, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var d))
            incoming = d;

        variable.Value = incoming;
        variable.Timestamp = DateTime.UtcNow;
        variable.ClearChangeMasks(null, false);

        Assert.True(activated);
        Assert.Equal("LowLow", limitState);
    }

    [Fact]
    public void Pressure_Alarm_Without_HighHighLimit()
    {
        // Pressure in AlarmDemo: HighLimit=8, LowLimit=2, no HighHigh/LowLow, Hysteresis=0.3
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId("Process.Pressure", 2),
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 5.0
        };
        variable.ClearChangeMasks(null, false);

        var config = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 8,
            LowLimit = 2,
            HighHighLimit = null,  // null → defaults to HighLimit
            LowLowLimit = null,   // null → defaults to LowLimit
            Hysteresis = 0.3
        };

        bool activated = false;
        bool isActive = false;
        ushort severity = 0;
        string limitState = "";

        variable.OnStateChanged += (context, state, masks) =>
        {
            if ((masks & NodeStateChangeMasks.Value) != 0)
            {
                var result = AlarmEvaluator.EvaluateLimitAlarm(variable.Value, config, isActive);
                if (result != null && result.ShouldActivate)
                {
                    activated = true;
                    isActive = true;
                    severity = result.Severity;
                    limitState = result.LimitState;
                }
            }
        };

        // Write 9.0 > HighLimit(8) and >= HighHighLimit(=HighLimit=8) → HighHigh
        variable.Value = 9.0;
        variable.Timestamp = DateTime.UtcNow;
        variable.ClearChangeMasks(null, false);

        Assert.True(activated, "Pressure alarm should activate at 9.0 > HighLimit(8)");
        // When HighHighLimit is null, it defaults to HighLimit (8), so 9 >= 8 → HighHigh
        Assert.Equal("HighHigh", limitState);
        Assert.Equal((ushort)900, severity);
    }
}
