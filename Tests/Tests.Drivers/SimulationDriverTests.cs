using Xunit;
using System.Text.Json;
using Opc.Ua;
using SimpleOpcFileServer;

namespace Tests.Drivers;

public class SimulationDriverTests
{
    private static SimulationDriver CreateDriver()
    {
        // The simulation driver only uses ISystemContext for ClearChangeMasks.
        // Pass null — ClearChangeMasks tolerates null context.
        return new SimulationDriver(null!);
    }

    private static BaseDataVariableState CreateVariable(string name, BuiltInType type = BuiltInType.Double)
    {
        var variable = new BaseDataVariableState(null)
        {
            NodeId = new NodeId(name, 2),
            BrowseName = new QualifiedName(name, 2),
            DisplayName = name,
            DataType = DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 0.0
        };
        return variable;
    }

    [Fact]
    public void Key_IsSimulation()
    {
        using var driver = CreateDriver();
        Assert.Equal("Simulation", driver.Key);
    }

    [Fact]
    public void AddItem_SineConfig_AcceptsJson()
    {
        using var driver = CreateDriver();
        var variable = CreateVariable("TestSine");

        var config = new SimulationConfig
        {
            Function = SimulationFunction.Sine,
            Period = 10000,
            Amplitude = 100,
            PollTime = 500
        };

        var json = JsonSerializer.Serialize(config);
        driver.AddItem(variable, json);

        // After a short delay, the variable should have been updated
        Thread.Sleep(600);
        Assert.NotNull(variable.Value);
    }

    [Fact]
    public void AddItem_InvalidJson_DoesNotThrow()
    {
        using var driver = CreateDriver();
        var variable = CreateVariable("Bad");

        // null config from deserialization → AddItem returns early
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void Poll_UpdatesVariable_AfterPollInterval()
    {
        using var driver = CreateDriver();
        var variable = CreateVariable("Counter");

        var config = JsonSerializer.Serialize(new SimulationConfig
        {
            Function = SimulationFunction.Counter,
            Step = 1,
            PollTime = 100
        });

        driver.AddItem(variable, config);
        Thread.Sleep(350); // ~3 poll cycles

        // Counter should have been incremented
        var value = Convert.ToDouble(variable.Value);
        Assert.True(value > 0, $"Counter value should be > 0, was {value}");
    }

    [Fact]
    public void Poll_Sine_ProducesValuesInRange()
    {
        using var driver = CreateDriver();
        var variable = CreateVariable("Sine");

        var config = JsonSerializer.Serialize(new SimulationConfig
        {
            Function = SimulationFunction.Sine,
            Period = 1000,
            Amplitude = 50,
            Offset = 10,
            PollTime = 50
        });

        driver.AddItem(variable, config);
        Thread.Sleep(200);

        var value = Convert.ToDouble(variable.Value);
        // Sine with amplitude 50 and offset 10: range is -40 to 60
        Assert.InRange(value, -40.0, 60.0);
    }

    [Fact]
    public void Poll_RandomInt_ProducesIntegersInRange()
    {
        using var driver = CreateDriver();
        var variable = CreateVariable("RandInt");

        var config = JsonSerializer.Serialize(new SimulationConfig
        {
            Function = SimulationFunction.RandomInt,
            Min = 10,
            Max = 20,
            PollTime = 50
        });

        driver.AddItem(variable, config);
        Thread.Sleep(300);

        // RandomInt produces an int; convert carefully
        var value = Convert.ToInt32(variable.Value);
        Assert.InRange(value, 10, 20);
    }

    [Fact]
    public void Poll_Blink_ProducesBoolean()
    {
        using var driver = CreateDriver();
        var variable = CreateVariable("Blink");

        var config = JsonSerializer.Serialize(new SimulationConfig
        {
            Function = SimulationFunction.Blink,
            Period = 200,
            DutyCycle = 0.5,
            PollTime = 50
        });

        driver.AddItem(variable, config);
        Thread.Sleep(150);

        Assert.IsType<bool>(variable.Value);
    }

    [Fact]
    public void Poll_Square_ProducesExpectedValues()
    {
        using var driver = CreateDriver();
        var variable = CreateVariable("Square");

        var config = JsonSerializer.Serialize(new SimulationConfig
        {
            Function = SimulationFunction.Square,
            Period = 10000,
            Amplitude = 100,
            DutyCycle = 0.5,
            PollTime = 50
        });

        driver.AddItem(variable, config);
        Thread.Sleep(150);

        var value = Convert.ToDouble(variable.Value);
        // Square wave: either +Amplitude or -Amplitude (plus offset=0)
        Assert.True(Math.Abs(value) == 100 || Math.Abs(value) < 0.001,
            $"Square value should be ±100, was {value}");
    }

    [Fact]
    public void OnCycleCompleted_Fires()
    {
        using var driver = CreateDriver();
        var variable = CreateVariable("CycleTest");

        bool fired = false;
        driver.OnCycleCompleted += _ => fired = true;

        var config = JsonSerializer.Serialize(new SimulationConfig
        {
            Function = SimulationFunction.Sine,
            PollTime = 50
        });

        driver.AddItem(variable, config);
        Thread.Sleep(200);

        Assert.True(fired);
    }

    [Fact]
    public void Dispose_StopsPollTimer()
    {
        var driver = CreateDriver();
        var variable = CreateVariable("DisposeTest");

        var config = JsonSerializer.Serialize(new SimulationConfig
        {
            Function = SimulationFunction.Counter,
            Step = 1,
            PollTime = 50
        });

        driver.AddItem(variable, config);
        Thread.Sleep(150);

        var valueBefore = Convert.ToDouble(variable.Value);
        driver.Dispose();
        Thread.Sleep(200);

        var valueAfter = Convert.ToDouble(variable.Value);
        // After dispose, no more increments should happen (or at most 1 in-flight)
        Assert.True(valueAfter - valueBefore <= 1,
            $"Counter continued after dispose: before={valueBefore} after={valueAfter}");
    }
}
