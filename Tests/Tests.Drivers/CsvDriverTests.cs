using Xunit;
using System.Text.Json;
using Opc.Ua;
using SimpleOpcFileServer;

namespace Tests.Drivers;

public class CsvDriverTests : IDisposable
{
    private readonly string _tempDir;

    public CsvDriverTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "CsvDriverTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    private string CreateCsvFile(string content, string name = "test.csv")
    {
        var path = Path.Combine(_tempDir, name);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void Key_IsCsv()
    {
        using var driver = new CsvDriver(null!);
        Assert.Equal("Csv", driver.Key);
    }

    [Fact]
    public void AddItem_NullConfig_DoesNotThrow()
    {
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvNull");
        driver.AddItem(variable, "null");
    }

    [Fact]
    public void Dispose_AfterCreate_DoesNotThrow()
    {
        var driver = new CsvDriver(null!);
        driver.Dispose();
    }

    [Fact]
    public void Poll_ReadsDoubleFromRowColumn()
    {
        var csvPath = CreateCsvFile("10.5,20.3,30.1\n40.0,50.5,60.9");
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvDouble", DataTypeIds.Double);

        var config = new CsvConfig { FilePath = csvPath, RowIndex = 0, ColumnIndex = 1, PollTime = 100 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();

        Thread.Sleep(500);
        Assert.Equal(20.3, Convert.ToDouble(variable.Value));
        Assert.Equal(StatusCodes.Good, variable.StatusCode.Code);
    }

    [Fact]
    public void Poll_ReadsSecondRow()
    {
        var csvPath = CreateCsvFile("1.0,2.0\n3.0,4.0\n5.0,6.0");
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvRow2", DataTypeIds.Double);

        var config = new CsvConfig { FilePath = csvPath, RowIndex = 2, ColumnIndex = 0, PollTime = 100 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();

        Thread.Sleep(500);
        Assert.Equal(5.0, Convert.ToDouble(variable.Value));
    }

    [Fact]
    public void Poll_ReadsValueByKey()
    {
        var csvPath = CreateCsvFile("sensor1,100\nsensor2,200\nsensor3,300");
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvKey", DataTypeIds.Double);

        var config = new CsvConfig { FilePath = csvPath, Key = "sensor2", ColumnIndex = 1, PollTime = 100 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();

        Thread.Sleep(1000);
        Assert.Equal(200.0, Convert.ToDouble(variable.Value));
    }

    [Fact]
    public void Poll_MissingFile_SetsBadStatus()
    {
        var csvPath = Path.Combine(_tempDir, "nonexistent.csv");
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvMissing", DataTypeIds.Double);
        variable.StatusCode = StatusCodes.Good; // start good

        var config = new CsvConfig { FilePath = csvPath, RowIndex = 0, ColumnIndex = 0, PollTime = 100 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();

        Thread.Sleep(500);
        Assert.Equal(StatusCodes.Bad, variable.StatusCode.Code);
    }

    [Fact]
    public void Poll_ReadsInt32()
    {
        var csvPath = CreateCsvFile("42,hello\n99,world");
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvInt", DataTypeIds.Int32);

        var config = new CsvConfig { FilePath = csvPath, RowIndex = 1, ColumnIndex = 0, PollTime = 100 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();

        Thread.Sleep(500);
        Assert.Equal(99, Convert.ToInt32(variable.Value));
    }

    [Fact]
    public void Poll_ReadsBoolean()
    {
        var csvPath = CreateCsvFile("true,false\nfalse,true");
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvBool", DataTypeIds.Boolean);

        var config = new CsvConfig { FilePath = csvPath, RowIndex = 0, ColumnIndex = 0, PollTime = 100 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();

        Thread.Sleep(500);
        Assert.Equal(true, variable.Value);
    }

    [Fact]
    public void Poll_ReadsString()
    {
        var csvPath = CreateCsvFile("hello,world\nfoo,bar");
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvStr", DataTypeIds.String);

        var config = new CsvConfig { FilePath = csvPath, RowIndex = 1, ColumnIndex = 1, PollTime = 100 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();

        Thread.Sleep(500);
        Assert.Equal("bar", variable.Value?.ToString());
    }

    [Fact]
    public void Poll_SemicolonDelimiter_Works()
    {
        var csvPath = CreateCsvFile("10;20;30\n40;50;60");
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvSemicolon", DataTypeIds.Double);

        var config = new CsvConfig { FilePath = csvPath, RowIndex = 0, ColumnIndex = 2, PollTime = 100 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();

        Thread.Sleep(500);
        Assert.Equal(30.0, Convert.ToDouble(variable.Value));
    }

    [Fact]
    public void OnCycleCompleted_Fires()
    {
        var csvPath = CreateCsvFile("1.0,2.0");
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvCycle", DataTypeIds.Double);

        bool fired = false;
        driver.OnCycleCompleted += _ => fired = true;

        var config = new CsvConfig { FilePath = csvPath, RowIndex = 0, ColumnIndex = 0, PollTime = 100 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();

        Thread.Sleep(500);
        Assert.True(fired);
    }

    [Fact]
    public void Poll_MultipleItems_SameFile()
    {
        var csvPath = CreateCsvFile("10.0,20.0,30.0\n40.0,50.0,60.0");
        using var driver = new CsvDriver(null!);
        var v1 = DriverTestHelpers.CreateVariable("CsvMulti1", DataTypeIds.Double);
        var v2 = DriverTestHelpers.CreateVariable("CsvMulti2", DataTypeIds.Double);

        var config1 = new CsvConfig { FilePath = csvPath, RowIndex = 0, ColumnIndex = 0, PollTime = 100 };
        var config2 = new CsvConfig { FilePath = csvPath, RowIndex = 1, ColumnIndex = 2, PollTime = 100 };
        driver.AddItem(v1, JsonSerializer.Serialize(config1));
        driver.AddItem(v2, JsonSerializer.Serialize(config2));
        driver.Start();

        Thread.Sleep(500);
        Assert.Equal(10.0, Convert.ToDouble(v1.Value));
        Assert.Equal(60.0, Convert.ToDouble(v2.Value));
    }

    [Fact]
    public void Poll_KeyNotFound_SetsBadStatus()
    {
        var csvPath = CreateCsvFile("sensor1,100\nsensor2,200");
        using var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvBadKey", DataTypeIds.Double);
        variable.StatusCode = StatusCodes.Good;

        var config = new CsvConfig { FilePath = csvPath, Key = "nonexistent", ColumnIndex = 1, PollTime = 100 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();

        Thread.Sleep(500);
        Assert.Equal(StatusCodes.Bad, variable.StatusCode.Code);
    }

    [Fact]
    public void Dispose_StopsPollTimer()
    {
        var csvPath = CreateCsvFile("1.0");
        var driver = new CsvDriver(null!);
        var variable = DriverTestHelpers.CreateVariable("CsvDispose", DataTypeIds.Double);

        var config = new CsvConfig { FilePath = csvPath, RowIndex = 0, ColumnIndex = 0, PollTime = 50 };
        driver.AddItem(variable, JsonSerializer.Serialize(config));
        driver.Start();
        Thread.Sleep(200);

        driver.Dispose();
        // Should not throw after dispose
        Thread.Sleep(200);
    }
}
