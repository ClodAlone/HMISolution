using Xunit;
using System.Text.Json;
using SimpleOpcFileServer;

namespace Tests.Drivers;

public class SimulationConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new SimulationConfig();
        Assert.Equal(SimulationFunction.Sine, config.Function);
        Assert.Equal(10000, config.Period);
        Assert.Equal(100, config.Amplitude);
        Assert.Equal(0, config.Offset);
        Assert.Equal(0, config.Phase);
        Assert.Equal(1000, config.PollTime);
        Assert.Equal(0, config.Min);
        Assert.Equal(100, config.Max);
        Assert.Equal(1, config.Step);
        Assert.Equal(0.5, config.DutyCycle);
    }

    [Fact]
    public void Deserialize_FromJson_CaseInsensitive()
    {
        var json = """
        {
            "function": "Ramp",
            "period": 5000,
            "amplitude": 200,
            "pollTime": 250
        }
        """;

        var config = JsonSerializer.Deserialize<SimulationConfig>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(config);
        Assert.Equal(SimulationFunction.Ramp, config.Function);
        Assert.Equal(5000, config.Period);
        Assert.Equal(200, config.Amplitude);
        Assert.Equal(250, config.PollTime);
    }

    [Fact]
    public void Roundtrip_AllFunctions()
    {
        foreach (SimulationFunction func in Enum.GetValues<SimulationFunction>())
        {
            var config = new SimulationConfig { Function = func };
            var json = JsonSerializer.Serialize(config);
            var result = JsonSerializer.Deserialize<SimulationConfig>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(result);
            Assert.Equal(func, result.Function);
        }
    }
}
