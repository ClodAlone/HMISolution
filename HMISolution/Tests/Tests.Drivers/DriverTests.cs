using Xunit;
using System.Text.Json;
using Opc.Ua;
using SimpleOpcFileServer;

namespace Tests.Drivers;

/// <summary>
/// Shared helpers for driver tests.
/// </summary>
internal static class DriverTestHelpers
{
    public static BaseDataVariableState CreateVariable(string name, NodeId? dataType = null)
    {
        return new BaseDataVariableState(null)
        {
            NodeId = new NodeId(name, 2),
            BrowseName = new QualifiedName(name, 2),
            DisplayName = name,
            DataType = dataType ?? DataTypeIds.Double,
            ValueRank = ValueRanks.Scalar,
            Value = 0.0
        };
    }
}

public class ModbusDriverTests
{
    [Fact]
    public void Key_IsModbus()
    {
        using var driver = new ModbusDriver(null!);
        Assert.Equal("Modbus", driver.Key);
    }

    [Fact]
    public void AddItem_NullConfig_DoesNotThrow()
    {
        using var driver = new ModbusDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("TestModbus");
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void AddItem_EmptyConfig_DoesNotThrow()
    {
        using var driver = new ModbusDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("TestModbus");
        // Empty JSON object deserializes to a config with defaults; no connection attempt
        driver.AddItem(variable, "{}");
    }

    [Fact]
    public void Dispose_AfterCreate_DoesNotThrow()
    {
        var driver = new ModbusDriver(null!);
        driver.Dispose();
    }

    [Fact]
    public void AddItem_MultipleItems_SameDevice_DoesNotThrow()
    {
        using var driver = new ModbusDriver(null!);
        var v1 = DriverTestHelpers.CreateVariable("Mb1");
        var v2 = DriverTestHelpers.CreateVariable("Mb2");

        var config = JsonSerializer.Serialize(new ModbusConfig { IpAddress = "192.0.2.1", Port = 502, Register = 0, PollTime = 5000 });
        driver.AddItem(v1, config);
        driver.AddItem(v2, config);
    }
}

public class S7DriverTests
{
    [Fact]
    public void Key_IsS7()
    {
        using var driver = new S7Driver(null!);
        Assert.Equal("S7", driver.Key);
    }

    [Fact]
    public void AddItem_NullConfig_DoesNotThrow()
    {
        using var driver = new S7Driver(null!);
        var variable = DriverTestHelpers.CreateVariable("TestS7");
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void Dispose_AfterCreate_DoesNotThrow()
    {
        var driver = new S7Driver(null!);
        driver.Dispose();
    }

    }

public class OpcUaClientDriverTests
{
    [Fact]
    public void Key_IsOpcUaClient()
    {
        using var driver = new OpcUaClientDriver(null!);
        Assert.Equal("OpcUaClient", driver.Key);
    }

    [Fact]
    public void AddItem_NullConfig_DoesNotThrow()
    {
        using var driver = new OpcUaClientDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("TestOpcUa");
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void Dispose_AfterCreate_DoesNotThrow()
    {
        var driver = new OpcUaClientDriver(null!);
        driver.Dispose();
    }
}

public class MqttDriverTests
{
    [Fact]
    public void Key_IsMqtt()
    {
        using var driver = new MqttDriver(null!);
        Assert.Equal("Mqtt", driver.Key);
    }

    [Fact]
    public void AddItem_NullConfig_DoesNotThrow()
    {
        using var driver = new MqttDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("TestMqtt");
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void Dispose_AfterCreate_DoesNotThrow()
    {
        var driver = new MqttDriver(null!);
        driver.Dispose();
    }
}

public class RestDriverTests
{
    [Fact]
    public void Key_IsRest()
    {
        using var driver = new RestDriver(null!);
        Assert.Equal("Rest", driver.Key);
    }

    [Fact]
    public void AddItem_NullConfig_DoesNotThrow()
    {
        using var driver = new RestDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("TestRest");
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void Dispose_AfterCreate_DoesNotThrow()
    {
        var driver = new RestDriver(null!);
        driver.Dispose();
    }

    }

public class TcpDriverTests
{
    [Fact]
    public void Key_IsTcp()
    {
        using var driver = new TcpDriver(null!);
        Assert.Equal("Tcp", driver.Key);
    }

    [Fact]
    public void AddItem_NullConfig_DoesNotThrow()
    {
        using var driver = new TcpDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("TestTcp");
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void Dispose_AfterCreate_DoesNotThrow()
    {
        var driver = new TcpDriver(null!);
        driver.Dispose();
    }
}

public class SqlDriverTests
{
    [Fact]
    public void Key_IsSql()
    {
        using var driver = new SqlDriver(null!);
        Assert.Equal("Sql", driver.Key);
    }

    [Fact]
    public void AddItem_NullConfig_DoesNotThrow()
    {
        using var driver = new SqlDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("TestSql");
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void Dispose_AfterCreate_DoesNotThrow()
    {
        var driver = new SqlDriver(null!);
        driver.Dispose();
    }
}

public class EtherNetIPDriverTests
{
    [Fact]
    public void Key_IsEtherNetIP()
    {
        using var driver = new EtherNetIPDriver(null!);
        Assert.Equal("EtherNetIP", driver.Key);
    }

    [Fact]
    public void AddItem_NullConfig_DoesNotThrow()
    {
        using var driver = new EtherNetIPDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("TestEIP");
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void Dispose_AfterCreate_DoesNotThrow()
    {
        var driver = new EtherNetIPDriver(null!);
        driver.Dispose();
    }
}

public class KnxDriverTests
{
    [Fact]
    public void Key_IsKnx()
    {
        using var driver = new KnxDriver(null!);
        Assert.Equal("Knx", driver.Key);
    }

    [Fact]
    public void AddItem_NullConfig_DoesNotThrow()
    {
        using var driver = new KnxDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("TestKnx");
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void Dispose_AfterCreate_DoesNotThrow()
    {
        var driver = new KnxDriver(null!);
        driver.Dispose();
    }
}
