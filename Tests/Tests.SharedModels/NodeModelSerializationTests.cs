using Xunit;
using System.Text.Json;
using SharedModels;

namespace Tests.SharedModels;

public class NodeModelSerializationTests
{
    [Fact]
    public void Roundtrip_EmptyModel()
    {
        var model = new NodeModel
        {
            Folder = new Folder { Name = "Root" },
            Server = new ServerSettings()
        };

        var json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
        var deserialized = JsonSerializer.Deserialize<NodeModel>(json);

        Assert.NotNull(deserialized);
        Assert.Equal("Root", deserialized.Folder.Name);
        Assert.NotNull(deserialized.Server);
    }

    [Fact]
    public void Variable_DriverConfigs_PreservedViaJsonExtensionData()
    {
        var json = """
        {
            "Name": "TestVar",
            "Type": "Double",
            "Access": "Read",
            "Value": 0.0,
            "Simulation": {
                "Function": "Sine",
                "Period": 5000,
                "Amplitude": 50
            }
        }
        """;

        var variable = JsonSerializer.Deserialize<Variable>(json);

        Assert.NotNull(variable);
        Assert.Equal("TestVar", variable.Name);
        Assert.NotNull(variable.DriverConfigs);
        Assert.True(variable.DriverConfigs.ContainsKey("Simulation"));

        var simConfig = variable.DriverConfigs["Simulation"];
        Assert.Equal("Sine", simConfig.GetProperty("Function").GetString());
        Assert.Equal(5000, simConfig.GetProperty("Period").GetInt32());
    }

    [Fact]
    public void Variable_DriverConfigs_NullWhenNoExtras()
    {
        var json = """
        {
            "Name": "Simple",
            "Type": "Int32",
            "Access": "ReadWrite",
            "Value": 42
        }
        """;

        var variable = JsonSerializer.Deserialize<Variable>(json);

        Assert.NotNull(variable);
        Assert.Equal("Simple", variable.Name);
        Assert.Null(variable.DriverConfigs);
    }

    [Fact]
    public void Variable_MultipleDriverConfigs()
    {
        var json = """
        {
            "Name": "MultiDriver",
            "Type": "Double",
            "Access": "Read",
            "Simulation": { "Function": "Ramp" },
            "Modbus": { "Address": 100 }
        }
        """;

        var variable = JsonSerializer.Deserialize<Variable>(json);

        Assert.NotNull(variable?.DriverConfigs);
        Assert.Equal(2, variable.DriverConfigs.Count);
        Assert.True(variable.DriverConfigs.ContainsKey("Simulation"));
        Assert.True(variable.DriverConfigs.ContainsKey("Modbus"));
    }

    [Fact]
    public void Folder_NestedStructure_Roundtrip()
    {
        var folder = new Folder
        {
            Name = "Root",
            Folders = new()
            {
                new Folder
                {
                    Name = "Sub1",
                    Variables = new() { new Variable { Name = "Temp", Type = "Double", Value = 25.0 } }
                }
            },
            Variables = new() { new Variable { Name = "Counter", Type = "Int32", Value = 0 } }
        };

        var json = JsonSerializer.Serialize(folder);
        var result = JsonSerializer.Deserialize<Folder>(json);

        Assert.NotNull(result);
        Assert.Equal("Root", result.Name);
        Assert.Single(result.Folders);
        Assert.Equal("Sub1", result.Folders[0].Name);
        Assert.Single(result.Folders[0].Variables);
        Assert.Equal("Temp", result.Folders[0].Variables[0].Name);
        Assert.Single(result.Variables);
    }

    [Fact]
    public void ScreenConfig_Roundtrip()
    {
        var screen = new ScreenConfig
        {
            Name = "Main",
            Width = 1024,
            Height = 768,
            Symbols = new()
            {
                new ScreenSymbol { Id = "s1", Type = "editbox", Label = "Speed", VariablePath = "Plant.Speed" }
            }
        };

        var json = JsonSerializer.Serialize(screen);
        var result = JsonSerializer.Deserialize<ScreenConfig>(json);

        Assert.NotNull(result);
        Assert.Equal("Main", result.Name);
        Assert.Single(result.Symbols);
        Assert.Equal("editbox", result.Symbols[0].Type);
        Assert.Equal("Plant.Speed", result.Symbols[0].VariablePath);
    }

    [Fact]
    public void AlarmConfig_LimitType_Roundtrip()
    {
        var alarm = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 100.0,
            LowLimit = 10.0,
            HighHighLimit = 120.0,
            LowLowLimit = 5.0,
            Hysteresis = 2.0,
            Message = "Temp out of range"
        };

        var json = JsonSerializer.Serialize(alarm);
        var result = JsonSerializer.Deserialize<AlarmConfig>(json);

        Assert.NotNull(result);
        Assert.Equal(AlarmTriggerType.Limit, result.TriggerType);
        Assert.Equal(100.0, result.HighLimit);
        Assert.Equal(10.0, result.LowLimit);
        Assert.Equal(2.0, result.Hysteresis);
    }

    [Fact]
    public void ServerSettings_DefaultValues()
    {
        var settings = new ServerSettings();
        Assert.Equal("opc.tcp://localhost:14840/SimpleOpcFileServer", settings.EndpointUrl);
        Assert.True(settings.EnableAnonymous);
        Assert.Equal(14841, settings.DiagnosticsPort);
    }
}
