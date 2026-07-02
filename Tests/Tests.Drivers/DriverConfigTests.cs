// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Xunit;
using System.Text.Json;
using SimpleOpcFileServer;

namespace Tests.Drivers;

public class ModbusConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new ModbusConfig();
        Assert.Equal("", config.IpAddress);
        Assert.Equal(0, config.Port);
        Assert.Equal(0, config.UnitId);
        Assert.Equal(0, config.Register);
        Assert.Null(config.RegisterType);
        Assert.Equal(1000, config.PollTime);
    }

    [Fact]
    public void Deserialize_CaseInsensitive()
    {
        var json = """{"ipAddress":"10.0.0.1","port":502,"unitId":1,"register":100,"registerType":"Coil","pollTime":500}""";
        var config = JsonSerializer.Deserialize<ModbusConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(config);
        Assert.Equal("10.0.0.1", config.IpAddress);
        Assert.Equal(502, config.Port);
        Assert.Equal(1, config.UnitId);
        Assert.Equal(100, config.Register);
        Assert.Equal("Coil", config.RegisterType);
        Assert.Equal(500, config.PollTime);
    }

    [Fact]
    public void Roundtrip()
    {
        var config = new ModbusConfig { IpAddress = "192.168.1.10", Port = 502, UnitId = 1, Register = 40001, RegisterType = "HoldingRegister", PollTime = 250 };
        var json = JsonSerializer.Serialize(config);
        var result = JsonSerializer.Deserialize<ModbusConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.Equal(config.IpAddress, result.IpAddress);
        Assert.Equal(config.Port, result.Port);
        Assert.Equal(config.UnitId, result.UnitId);
        Assert.Equal(config.Register, result.Register);
        Assert.Equal(config.RegisterType, result.RegisterType);
        Assert.Equal(config.PollTime, result.PollTime);
    }
}

public class S7ConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new S7Config();
        Assert.Equal("", config.IpAddress);
        Assert.Equal(0, config.Rack);
        Assert.Equal(0, config.Slot);
        Assert.Equal("", config.Address);
        Assert.Equal(1000, config.PollTime);
    }

    [Fact]
    public void Deserialize_CaseInsensitive()
    {
        var json = """{"ipAddress":"10.0.0.2","rack":0,"slot":1,"address":"DB1.DBD0","pollTime":200}""";
        var config = JsonSerializer.Deserialize<S7Config>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(config);
        Assert.Equal("10.0.0.2", config.IpAddress);
        Assert.Equal(0, config.Rack);
        Assert.Equal(1, config.Slot);
        Assert.Equal("DB1.DBD0", config.Address);
        Assert.Equal(200, config.PollTime);
    }

    [Fact]
    public void Roundtrip()
    {
        var config = new S7Config { IpAddress = "10.0.0.2", Rack = 0, Slot = 1, Address = "DB1.DBW10", PollTime = 300 };
        var json = JsonSerializer.Serialize(config);
        var result = JsonSerializer.Deserialize<S7Config>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.Equal(config.IpAddress, result.IpAddress);
        Assert.Equal(config.Rack, result.Rack);
        Assert.Equal(config.Slot, result.Slot);
        Assert.Equal(config.Address, result.Address);
        Assert.Equal(config.PollTime, result.PollTime);
    }
}

public class OpcUaClientConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new OpcUaClientConfig();
        Assert.Equal("opc.tcp://localhost:4840", config.EndpointUrl);
        Assert.Equal("ns=2;s=Demo.Static.Scalar.Double", config.NodeId);
        Assert.Equal(1000, config.PollTime);
    }

    [Fact]
    public void Deserialize_CaseInsensitive()
    {
        var json = """{"endpointUrl":"opc.tcp://10.0.0.3:4840","nodeId":"ns=2;s=MyTag","pollTime":500}""";
        var config = JsonSerializer.Deserialize<OpcUaClientConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(config);
        Assert.Equal("opc.tcp://10.0.0.3:4840", config.EndpointUrl);
        Assert.Equal("ns=2;s=MyTag", config.NodeId);
        Assert.Equal(500, config.PollTime);
    }

    [Fact]
    public void Roundtrip()
    {
        var config = new OpcUaClientConfig { EndpointUrl = "opc.tcp://plc:4840", NodeId = "ns=3;i=1001", PollTime = 750 };
        var json = JsonSerializer.Serialize(config);
        var result = JsonSerializer.Deserialize<OpcUaClientConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.Equal(config.EndpointUrl, result.EndpointUrl);
        Assert.Equal(config.NodeId, result.NodeId);
        Assert.Equal(config.PollTime, result.PollTime);
    }
}

public class MqttConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new MqttConfig();
        Assert.Equal("", config.Broker);
        Assert.Equal(0, config.Port);
        Assert.Equal("", config.Topic);
        Assert.Null(config.JsonPath);
    }

    [Fact]
    public void Deserialize_CaseInsensitive()
    {
        var json = """{"broker":"mqtt.local","port":1883,"topic":"sensors/temp","jsonPath":"value"}""";
        var config = JsonSerializer.Deserialize<MqttConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(config);
        Assert.Equal("mqtt.local", config.Broker);
        Assert.Equal(1883, config.Port);
        Assert.Equal("sensors/temp", config.Topic);
        Assert.Equal("value", config.JsonPath);
    }

    [Fact]
    public void Roundtrip()
    {
        var config = new MqttConfig { Broker = "192.168.1.50", Port = 1883, Topic = "factory/line1", JsonPath = "data.temperature" };
        var json = JsonSerializer.Serialize(config);
        var result = JsonSerializer.Deserialize<MqttConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.Equal(config.Broker, result.Broker);
        Assert.Equal(config.Port, result.Port);
        Assert.Equal(config.Topic, result.Topic);
        Assert.Equal(config.JsonPath, result.JsonPath);
    }
}

public class RestConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new RestConfig();
        Assert.Equal("", config.Url);
        Assert.Null(config.JsonPath);
        Assert.Equal(1000, config.PollTime);
    }

    [Fact]
    public void Deserialize_CaseInsensitive()
    {
        var json = """{"url":"https://api.example.com/data","jsonPath":"result.value","pollTime":2000}""";
        var config = JsonSerializer.Deserialize<RestConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(config);
        Assert.Equal("https://api.example.com/data", config.Url);
        Assert.Equal("result.value", config.JsonPath);
        Assert.Equal(2000, config.PollTime);
    }

    [Fact]
    public void Roundtrip()
    {
        var config = new RestConfig { Url = "http://localhost:8080/api", JsonPath = "temp", PollTime = 500 };
        var json = JsonSerializer.Serialize(config);
        var result = JsonSerializer.Deserialize<RestConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.Equal(config.Url, result.Url);
        Assert.Equal(config.JsonPath, result.JsonPath);
        Assert.Equal(config.PollTime, result.PollTime);
    }
}

public class CsvConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new CsvConfig();
        Assert.Equal("", config.FilePath);
        Assert.Equal(0, config.RowIndex);
        Assert.Equal(0, config.ColumnIndex);
        Assert.Null(config.Key);
        Assert.Equal(1000, config.PollTime);
    }

    [Fact]
    public void Deserialize_CaseInsensitive()
    {
        var json = """{"filePath":"C:\\data.csv","rowIndex":2,"columnIndex":3,"key":"sensor1","pollTime":500}""";
        var config = JsonSerializer.Deserialize<CsvConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(config);
        Assert.Equal("C:\\data.csv", config.FilePath);
        Assert.Equal(2, config.RowIndex);
        Assert.Equal(3, config.ColumnIndex);
        Assert.Equal("sensor1", config.Key);
        Assert.Equal(500, config.PollTime);
    }

    [Fact]
    public void Roundtrip()
    {
        var config = new CsvConfig { FilePath = "/tmp/test.csv", RowIndex = 1, ColumnIndex = 2, Key = "row1", PollTime = 250 };
        var json = JsonSerializer.Serialize(config);
        var result = JsonSerializer.Deserialize<CsvConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.Equal(config.FilePath, result.FilePath);
        Assert.Equal(config.RowIndex, result.RowIndex);
        Assert.Equal(config.ColumnIndex, result.ColumnIndex);
        Assert.Equal(config.Key, result.Key);
        Assert.Equal(config.PollTime, result.PollTime);
    }
}

public class TcpConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new TcpConfig();
        Assert.Equal("", config.IpAddress);
        Assert.Equal(0, config.Port);
        Assert.Equal("", config.Command);
        Assert.Null(config.Regex);
        Assert.Equal(1000, config.PollTime);
    }

    [Fact]
    public void Deserialize_CaseInsensitive()
    {
        var json = """{"ipAddress":"10.0.0.5","port":9100,"command":"READ\\r\\n","regex":"(\\d+\\.\\d+)","pollTime":500}""";
        var config = JsonSerializer.Deserialize<TcpConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(config);
        Assert.Equal("10.0.0.5", config.IpAddress);
        Assert.Equal(9100, config.Port);
        Assert.Equal("READ\\r\\n", config.Command);
        Assert.Equal("(\\d+\\.\\d+)", config.Regex);
        Assert.Equal(500, config.PollTime);
    }

    [Fact]
    public void Roundtrip()
    {
        var config = new TcpConfig { IpAddress = "10.0.0.5", Port = 9100, Command = "MEASURE\n", Regex = @"(\d+)", PollTime = 300 };
        var json = JsonSerializer.Serialize(config);
        var result = JsonSerializer.Deserialize<TcpConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.Equal(config.IpAddress, result.IpAddress);
        Assert.Equal(config.Port, result.Port);
        Assert.Equal(config.Command, result.Command);
        Assert.Equal(config.Regex, result.Regex);
        Assert.Equal(config.PollTime, result.PollTime);
    }
}

public class SqlConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new SqlConfig();
        Assert.Equal("", config.ConnectionString);
        Assert.Equal("", config.Query);
        Assert.Equal(1000, config.PollTime);
    }

    [Fact]
    public void Deserialize_CaseInsensitive()
    {
        var json = """{"connectionString":"Server=.;Database=TestDB","query":"SELECT TOP 1 Value FROM Sensors","pollTime":2000}""";
        var config = JsonSerializer.Deserialize<SqlConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(config);
        Assert.Equal("Server=.;Database=TestDB", config.ConnectionString);
        Assert.Equal("SELECT TOP 1 Value FROM Sensors", config.Query);
        Assert.Equal(2000, config.PollTime);
    }

    [Fact]
    public void Roundtrip()
    {
        var config = new SqlConfig { ConnectionString = "Data Source=:memory:", Query = "SELECT 1", PollTime = 500 };
        var json = JsonSerializer.Serialize(config);
        var result = JsonSerializer.Deserialize<SqlConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.Equal(config.ConnectionString, result.ConnectionString);
        Assert.Equal(config.Query, result.Query);
        Assert.Equal(config.PollTime, result.PollTime);
    }
}

public class EtherNetIPConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new EtherNetIPConfig();
        Assert.Equal("", config.IpAddress);
        Assert.Equal("", config.Path);
        Assert.Equal("", config.Tag);
        Assert.Equal("", config.Type);
        Assert.Equal(1000, config.PollTime);
    }

    [Fact]
    public void Deserialize_CaseInsensitive()
    {
        var json = """{"ipAddress":"10.0.0.6","path":"1,0","tag":"MyTag","type":"DINT","pollTime":100}""";
        var config = JsonSerializer.Deserialize<EtherNetIPConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(config);
        Assert.Equal("10.0.0.6", config.IpAddress);
        Assert.Equal("1,0", config.Path);
        Assert.Equal("MyTag", config.Tag);
        Assert.Equal("DINT", config.Type);
        Assert.Equal(100, config.PollTime);
    }

    [Fact]
    public void Roundtrip()
    {
        var config = new EtherNetIPConfig { IpAddress = "10.0.0.6", Path = "1,0", Tag = "Counter", Type = "REAL", PollTime = 200 };
        var json = JsonSerializer.Serialize(config);
        var result = JsonSerializer.Deserialize<EtherNetIPConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.Equal(config.IpAddress, result.IpAddress);
        Assert.Equal(config.Path, result.Path);
        Assert.Equal(config.Tag, result.Tag);
        Assert.Equal(config.Type, result.Type);
        Assert.Equal(config.PollTime, result.PollTime);
    }
}

public class KnxConfigTests
{
    [Fact]
    public void DefaultValues()
    {
        var config = new KnxConfig();
        Assert.Equal("", config.IpAddress);
        Assert.Equal(0, config.Port);
        Assert.Equal("", config.GroupAddress);
        Assert.Equal("", config.DptType);
    }

    [Fact]
    public void Deserialize_CaseInsensitive()
    {
        var json = """{"ipAddress":"10.0.0.7","port":3671,"groupAddress":"1/2/3","dptType":"DPT-1"}""";
        var config = JsonSerializer.Deserialize<KnxConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(config);
        Assert.Equal("10.0.0.7", config.IpAddress);
        Assert.Equal(3671, config.Port);
        Assert.Equal("1/2/3", config.GroupAddress);
        Assert.Equal("DPT-1", config.DptType);
    }

    [Fact]
    public void Roundtrip()
    {
        var config = new KnxConfig { IpAddress = "10.0.0.7", Port = 3671, GroupAddress = "0/0/1", DptType = "DPT-9" };
        var json = JsonSerializer.Serialize(config);
        var result = JsonSerializer.Deserialize<KnxConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(result);
        Assert.Equal(config.IpAddress, result.IpAddress);
        Assert.Equal(config.Port, result.Port);
        Assert.Equal(config.GroupAddress, result.GroupAddress);
        Assert.Equal(config.DptType, result.DptType);
    }
}
