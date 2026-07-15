// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Xunit;
using SharedModels;
using SharedModels.Import;

namespace Tests.SharedModels;

public class MoviconProjectImporterTests
{
    private const string SampleProjectXml = """
        <MoviconProject Name="DemoPlant">
          <Tags>
            <Group Name="Area1">
              <Tag Name="Pump1_Run" Type="Bool" Access="ReadWrite" InitialValue="false" Description="Pump running feedback" />
              <Tag Name="Tank1_Level" Type="Real" Access="Read" InitialValue="0" Unit="%" Hi="90" HiHi="95" Lo="10" LoLo="5" AlarmMessage="Tank1 level out of range" />
            </Group>
            <Tag Name="LineSpeed" Type="Long" Access="ReadWrite" InitialValue="0" />
          </Tags>
          <Screens>
            <Screen Name="Overview" Width="1024" Height="768" BackColor="#112233">
              <Objects>
                <Object Id="obj1" Type="Rectangle" X="10" Y="20" Width="100" Height="50" FillColor="#FF0000" Tag="Pump1_Run" />
                <Object Id="obj2" Type="Led" X="150" Y="20" Width="30" Height="30" Tag="Tank1_Level" />
                <Object Id="obj3" Type="WeirdFutureWidget" X="0" Y="0" Width="10" Height="10" />
              </Objects>
            </Screen>
          </Screens>
        </MoviconProject>
        """;

    [Fact]
    public void Import_MapsTagsIntoFolderTree()
    {
        var result = MoviconProjectImporter.ImportFromXml(SampleProjectXml);

        Assert.Equal(3, result.TagCount);
        Assert.Equal(1, result.FolderCount);

        var area1 = Assert.Single(result.Project.Folder.Folders);
        Assert.Equal("Area1", area1.Name);
        Assert.Equal(2, area1.Variables.Count);

        var pump = Assert.Single(area1.Variables, v => v.Name == "Pump1_Run");
        Assert.Equal("Boolean", pump.Type);
        Assert.Equal("ReadWrite", pump.Access);

        var lineSpeed = Assert.Single(result.Project.Folder.Variables, v => v.Name == "LineSpeed");
        Assert.Equal("Int32", lineSpeed.Type);
    }

    [Fact]
    public void Import_MapsAlarmThresholds()
    {
        var result = MoviconProjectImporter.ImportFromXml(SampleProjectXml);

        var area1 = Assert.Single(result.Project.Folder.Folders);
        var tankLevel = Assert.Single(area1.Variables, v => v.Name == "Tank1_Level");

        Assert.NotNull(tankLevel.Alarm);
        Assert.Equal(AlarmTriggerType.Limit, tankLevel.Alarm!.TriggerType);
        Assert.Equal(90, tankLevel.Alarm.HighLimit);
        Assert.Equal(95, tankLevel.Alarm.HighHighLimit);
        Assert.Equal(10, tankLevel.Alarm.LowLimit);
        Assert.Equal(5, tankLevel.Alarm.LowLowLimit);
        Assert.Equal("Tank1 level out of range", tankLevel.Alarm.Message);
        Assert.Equal(1, result.AlarmCount);
    }

    [Fact]
    public void Import_MapsScreensAndSymbols()
    {
        var result = MoviconProjectImporter.ImportFromXml(SampleProjectXml);

        Assert.Equal(1, result.ScreenCount);
        var screen = Assert.Single(result.Project.Screens);
        Assert.Equal("Overview", screen.Name);
        Assert.Equal(1024, screen.Width);
        Assert.Equal(768, screen.Height);
        Assert.Equal("#112233", screen.Background);
        Assert.Equal(3, screen.Symbols.Count);

        var rect = Assert.Single(screen.Symbols, s => s.Id == "obj1");
        Assert.Equal("rect", rect.Type);
        Assert.Equal(10, rect.X);
        Assert.Equal("#FF0000", rect.Fill);
        Assert.Equal("Pump1_Run", rect.VariablePath);

        var led = Assert.Single(screen.Symbols, s => s.Id == "obj2");
        Assert.Equal("indicator", led.Type);
    }

    [Fact]
    public void Import_UnknownSymbolType_FallsBackToRectAndWarns()
    {
        var result = MoviconProjectImporter.ImportFromXml(SampleProjectXml);

        var screen = Assert.Single(result.Project.Screens);
        var unknown = Assert.Single(screen.Symbols, s => s.Id == "obj3");
        Assert.Equal("rect", unknown.Type);
        Assert.Contains(result.Warnings, w => w.Contains("WeirdFutureWidget"));
    }

    [Fact]
    public void Import_ProjectNameUsedAsRootFolderName()
    {
        var result = MoviconProjectImporter.ImportFromXml(SampleProjectXml);
        Assert.Equal("DemoPlant", result.Project.Folder.Name);
    }

    [Fact]
    public void Import_ToleratesAlternateElementNames()
    {
        // "Variables"/"Variable"/"Folder"/"Pages"/"Page"/"Symbols"/"Symbol" alias set.
        const string alt = """
            <Project Name="AltSchema">
              <Variables>
                <Folder Name="Motors">
                  <Variable Name="Motor1_Speed" DataType="Float" Access="ReadOnly" />
                </Folder>
              </Variables>
              <Pages>
                <Page Name="Main" SizeX="640" SizeY="480">
                  <Symbols>
                    <Symbol Name="s1" ObjectType="Ellipse" Left="5" Top="5" />
                  </Symbols>
                </Page>
              </Pages>
            </Project>
            """;

        var result = MoviconProjectImporter.ImportFromXml(alt);

        Assert.Equal(1, result.TagCount);
        var motors = Assert.Single(result.Project.Folder.Folders);
        Assert.Equal("Motors", motors.Name);
        var motorVar = Assert.Single(motors.Variables);
        Assert.Equal("Double", motorVar.Type);
        Assert.Equal("Read", motorVar.Access);

        var screen = Assert.Single(result.Project.Screens);
        Assert.Equal("Main", screen.Name);
        Assert.Equal(640, screen.Width);
        var sym = Assert.Single(screen.Symbols);
        Assert.Equal("ellipse", sym.Type);
    }

    [Fact]
    public void Import_EmptyDocument_ReportsWarningAndEmptyProject()
    {
        var result = MoviconProjectImporter.ImportFromXml("<Nothing/>");

        Assert.True(result.IsEmpty);
        Assert.NotEmpty(result.Warnings);
    }
}
