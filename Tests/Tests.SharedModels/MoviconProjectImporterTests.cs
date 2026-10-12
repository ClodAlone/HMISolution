// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.IO;
using Xunit;
using SharedModels;
using SharedModels.Import;

namespace Tests.SharedModels;

/// <summary>
/// Tests against a synthetic but structurally-accurate Movicon 11 project folder, built from the
/// real element/attribute names observed in a genuine sample project (ServerHMI.movprj and siblings):
/// multi-file layout (.movprj/.movrealtimedb/.movalr/RESOURCES\*.movscr), element-text tag names with
/// a flat Group attribute, named reusable alarm definitions referenced via &lt;AlarmList n0="..."/&gt;,
/// and the nested child/object/Mechanic/Name screen symbol structure.
/// </summary>
public class MoviconProjectImporterTests : IDisposable
{
    private readonly string _root;

    public MoviconProjectImporterTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "MoviconImportTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    public void Dispose()
    {
        try { Directory.Delete(_root, recursive: true); } catch { /* best effort cleanup */ }
    }

    private string WriteProject(string baseName = "DemoPlant", bool includeResources = true)
    {
        File.WriteAllText(Path.Combine(_root, $"{baseName}.movprj"), $"""
            <?xml version="1.0" encoding="ISO-8859-1" ?>
            <ProjectSettings>
              <ProjectExecution StartupSynoptic="Overview" />
            </ProjectSettings>
            """);

        File.WriteAllText(Path.Combine(_root, $"{baseName}.movrealtimedb"), """
            <?xml version="1.0" encoding="ISO-8859-1" ?>
            <RealTimeDB>
              <VariableList>
                <Variable>
                  <Name Type="0" AreaType="1" Address="0" Bit="0" Group="Area1" Shared="0" Retentive="0">Pump1_Run</Name>
                  <EnableTrace DurationDays="0">0</EnableTrace>
                  <AlarmList n0="Digital Alarm"/>
                </Variable>
                <Variable>
                  <Name Type="7" AreaType="1" Address="2" Bit="0" Group="Area1" Shared="0" Retentive="1" Factor="1" EU="%">Tank1_Level</Name>
                  <EnableTrace DurationDays="0">0</EnableTrace>
                  <AlarmList n0="Analog Alarm"/>
                </Variable>
                <Variable>
                  <Name Type="2" AreaType="1" Address="4" Bit="0" Group="" Shared="0" Retentive="0">LineSpeed</Name>
                  <EnableTrace DurationDays="0">0</EnableTrace>
                </Variable>
                <Variable>
                  <Name Type="99" AreaType="1" Address="6" Bit="0" Group="" Shared="0" Retentive="0">WeirdFutureType</Name>
                </Variable>
                <Variable>
                  <Name Type="0" AreaType="1" Address="0" Bit="0" Group="Area1" Shared="0" Retentive="0" DynamicSettings="[DRV]Modbus TCPIP.Sta=PLC1|Unit=1|FC=0|SA=100|Typ=0|TaskType=0">Modbus_Bit</Name>
                  <EnableTrace DurationDays="0">0</EnableTrace>
                </Variable>
                <Variable>
                  <Name Type="7" AreaType="1" Address="0" Bit="0" Group="Area1" Shared="0" Retentive="0" DynamicSettings="[DRV]Modbus TCPIP.Sta=PLC1|Unit=2|FC=2|SA=12689|Typ=4|TaskType=2">Modbus_Analog</Name>
                  <EnableTrace DurationDays="0">0</EnableTrace>
                </Variable>
                <Variable>
                  <Name Type="0" AreaType="1" Address="0" Bit="0" Group="Area1" Shared="0" Retentive="0" DynamicSettings="[DRV]Modbus TCPIP.Sta=UnknownStation|Unit=1|FC=0|SA=5|Typ=0|TaskType=0">Modbus_UnknownStation</Name>
                  <EnableTrace DurationDays="0">0</EnableTrace>
                </Variable>
                <Variable>
                  <Name Type="0" AreaType="1" Address="0" Bit="0" Group="Area1" Shared="0" Retentive="0" DynamicSettings="[DRV]Fronius.Sta=Inverter1|Channel=3">Fronius_Value</Name>
                  <EnableTrace DurationDays="0">0</EnableTrace>
                </Variable>
              </VariableList>
            </RealTimeDB>
            """);

        File.WriteAllText(Path.Combine(_root, $"{baseName}.movalr"), """
            <?xml version="1.0" encoding="ISO-8859-1" ?>
            <Alarms>
              <AlarmList>
                <Alarm>
                  <Name Device="" Variable="" Area="" Hysteresis="0">Digital Alarm</Name>
                  <ThresholdList>
                    <Threshold>
                      <Name Area="" Title="">Digital</Name>
                      <Execution Condition="2" Threshold="1"/>
                    </Threshold>
                  </ThresholdList>
                </Alarm>
                <Alarm>
                  <Name Device="" Variable="" Area="" Hysteresis="2">Analog Alarm</Name>
                  <ThresholdList>
                    <Threshold>
                      <Name Area="" Title="">LowLow</Name>
                      <Execution Condition="1" Threshold="5"/>
                    </Threshold>
                    <Threshold>
                      <Name Area="" Title="">Low</Name>
                      <Execution Condition="1" Threshold="10"/>
                    </Threshold>
                    <Threshold>
                      <Name Area="" Title="">High</Name>
                      <Execution Condition="1" Threshold="90"/>
                    </Threshold>
                    <Threshold>
                      <Name Area="" Title="">HighHigh</Name>
                      <Execution Condition="1" Threshold="95"/>
                    </Threshold>
                  </ThresholdList>
                </Alarm>
              </AlarmList>
            </Alarms>
            """);

        File.WriteAllText(Path.Combine(_root, $"{baseName}.movdlrec"), """
            <?xml version="1.0" encoding="ISO-8859-1" ?>
            <DLAndRecipe>
              <DLRecipeList>
                <DLRecipe>
                  <Name CRWFile="" printer="" Enabled="1" IsRecipe="0" RecordOnTime="1" RecordOnChange="0" EnableDeadBand="1" DeadBand="2" TimeRecSec="5">Log5sec</Name>
                  <DSN DurationDays="7" DurationHours="0" DurationMinutes="0"/>
                  <ColumnList>
                    <Column>
                      <Name Variable="Tank1_Level" Type="7">Tank1_Level</Name>
                    </Column>
                  </ColumnList>
                </DLRecipe>
                <DLRecipe>
                  <Name CRWFile="" printer="" Enabled="1" IsRecipe="0" RecordOnTime="0" RecordOnChange="1" EnableDeadBand="0" DeadBand="30">Hoover</Name>
                  <DSN DurationDays="180" DurationHours="0" DurationMinutes="0"/>
                  <ColumnList>
                    <Column>
                      <Name Variable="Pump1_Run" Type="0">Pump1_Run</Name>
                    </Column>
                    <Column>
                      <Name Variable="UndefinedVar" Type="0">UndefinedVar</Name>
                    </Column>
                  </ColumnList>
                </DLRecipe>
                <DLRecipe>
                  <Name CRWFile="" printer="" Enabled="1" IsRecipe="1" AlwaysInUse="1">SceneRecipe</Name>
                  <ColumnList>
                    <Column>
                      <Name Variable="SceneRecipe" TempVar="" Type="0" RecipeIndex="1">Recipe Index</Name>
                    </Column>
                    <Column>
                      <Name Variable="Pump1_Run" TempVar="" Type="0" RecipeIndex="0">Pump1_Run</Name>
                    </Column>
                    <Column>
                      <Name Variable="LineSpeed" TempVar="" Type="0" RecipeIndex="0">LineSpeed</Name>
                    </Column>
                  </ColumnList>
                </DLRecipe>
              </DLRecipeList>
            </DLAndRecipe>
            """);

        if (includeResources)
        {
            var resourcesDir = Path.Combine(_root, "RESOURCES", baseName);
            Directory.CreateDirectory(resourcesDir);

            File.WriteAllText(Path.Combine(resourcesDir, "Overview.movscr"), """
                <?xml version="1.0" encoding="ISO-8859-1" ?>
                <MovResource>
                  <General SizeX="1024" SizeY="768"/>
                  <ListSynopticDraw>
                    <child>
                      <object>Button</object>
                      <type x="10" y="20" width="100" height="50" fill="rgb(255,0,0)" stroke="rgb(0,0,0)">normal</type>
                      <Mechanic CheckVariable="Pump1_Run" MechanicStyle="1"/>
                      <Name acr="0" StatusVariable="Pump1_Run"/>
                      <text>Pump1_Run</text>
                      <Rotation>0</Rotation>
                    </child>
                    <child>
                      <object>Edit</object>
                      <editbox x="150" y="20" width="60" height="30" fill="syscolor(WINDOW SysColor)" stroke="syscolor(WINDOWTEXT SysColor)"/>
                      <execution var="Tank1_Level"/>
                      <Name acr="0" StatusVariable="Tank1_Level"/>
                    </child>
                    <child>
                      <object>group</object>
                      <childs>
                        <child>
                          <object>base</object>
                          <type x="0" y="0" width="200" height="200" fill="rgb(10,20,30)" stroke="none"/>
                        </child>
                      </childs>
                    </child>
                    <child>
                      <object>poly</object>
                    </child>
                    <child>
                      <object>WeirdFutureWidget</object>
                      <type x="0" y="0" width="10" height="10" fill="rgb(1,2,3)"/>
                    </child>
                  </ListSynopticDraw>
                </MovResource>
                """);

            // Screens are also stored in nested subfolders in real projects (IO\, ROOMS\, ...); the
            // importer must find these too, and disambiguate same-named screens across folders.
            var ioDir = Path.Combine(resourcesDir, "IO");
            Directory.CreateDirectory(ioDir);
            File.WriteAllText(Path.Combine(ioDir, "Lights.movscr"), """
                <?xml version="1.0" encoding="ISO-8859-1" ?>
                <MovResource>
                  <General SizeX="640" SizeY="480"/>
                  <ListSynopticDraw>
                    <child>
                      <object>Button</object>
                      <type x="0" y="0" width="50" height="30" fill="rgb(0,255,0)" stroke="none"/>
                      <Name acr="0" StatusVariable="Pump1_Run"/>
                      <text>Toggle</text>
                    </child>
                  </ListSynopticDraw>
                </MovResource>
                """);

            File.WriteAllText(Path.Combine(resourcesDir, "ModBusTCPIP.drvsettings"), """
                <?xml version="1.0" encoding="ISO-8859-1" ?>
                <DriverSettings>
                  <GeneralSettings PollingTime="0"/>
                  <StationList>
                    <Station>
                      <Name StateVariable="PLC1Status">PLC1</Name>
                      <Server ServerAddress="192.168.0.222" ServerPort="502"/>
                    </Station>
                  </StationList>
                </DriverSettings>
                """);
        }

        return Path.Combine(_root, $"{baseName}.movprj");
    }

    [Fact]
    public void Import_MapsTagsIntoFolderTreeUsingFlatGroupAttribute()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        Assert.Equal(8, result.TagCount);
        Assert.Equal(1, result.FolderCount);

        var area1 = Assert.Single(result.Project.Folder.Folders);
        Assert.Equal("Area1", area1.Name);
        Assert.Equal(6, area1.Variables.Count);

        var pump = Assert.Single(area1.Variables, v => v.Name == "Pump1_Run");
        Assert.Equal("Boolean", pump.Type);

        var lineSpeed = Assert.Single(result.Project.Folder.Variables, v => v.Name == "LineSpeed");
        Assert.Equal("Int32", lineSpeed.Type);
    }

    [Fact]
    public void Import_UnknownTagType_DefaultsToDoubleAndWarns()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        var weird = Assert.Single(result.Project.Folder.Variables, v => v.Name == "WeirdFutureType");
        Assert.Equal("Double", weird.Type);
        Assert.Contains(result.Warnings, w => w.Contains("WeirdFutureType") && w.Contains("99"));
    }

    [Fact]
    public void Import_ModbusDynamicSettings_ResolvesStationIntoStructuredDriverConfig()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        var area1 = Assert.Single(result.Project.Folder.Folders);

        var bitTag = Assert.Single(area1.Variables, v => v.Name == "Modbus_Bit");
        Assert.NotNull(bitTag.DriverConfigs);
        Assert.True(bitTag.DriverConfigs!.ContainsKey("Modbus"));
        var bitConfig = bitTag.DriverConfigs["Modbus"];
        Assert.Equal("192.168.0.222", bitConfig.GetProperty("IpAddress").GetString());
        Assert.Equal(502, bitConfig.GetProperty("Port").GetInt32());
        Assert.Equal(1, bitConfig.GetProperty("UnitId").GetInt32());
        Assert.Equal(100, bitConfig.GetProperty("Register").GetInt32());
        Assert.Equal("Coil", bitConfig.GetProperty("RegisterType").GetString());

        var analogTag = Assert.Single(area1.Variables, v => v.Name == "Modbus_Analog");
        var analogConfig = analogTag.DriverConfigs!["Modbus"];
        Assert.Equal("192.168.0.222", analogConfig.GetProperty("IpAddress").GetString());
        Assert.Equal(2, analogConfig.GetProperty("UnitId").GetInt32());
        Assert.Equal(12689, analogConfig.GetProperty("Register").GetInt32());
        Assert.Equal("HoldingRegister", analogConfig.GetProperty("RegisterType").GetString());

        // Description fallback is still set for all driver-bound tags regardless of structured mapping success.
        Assert.Contains("Movicon driver settings:", bitTag.Description);
    }

    [Fact]
    public void Import_ModbusDynamicSettings_UnknownStation_WarnsAndSkipsStructuredConfig()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        var area1 = Assert.Single(result.Project.Folder.Folders);
        var tag = Assert.Single(area1.Variables, v => v.Name == "Modbus_UnknownStation");

        Assert.Null(tag.DriverConfigs);
        Assert.Contains(result.Warnings, w => w.Contains("UnknownStation"));
    }

    [Fact]
    public void Import_NonModbusDriverSettings_LeavesDescriptionOnlyFallback()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        var area1 = Assert.Single(result.Project.Folder.Folders);
        var tag = Assert.Single(area1.Variables, v => v.Name == "Fronius_Value");

        Assert.Null(tag.DriverConfigs);
        Assert.Contains("Movicon driver settings:", tag.Description);
    }

    [Fact]
    public void Import_ResolvesAlarmDefinitionsReferencedByTag()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        var area1 = Assert.Single(result.Project.Folder.Folders);
        var pump = Assert.Single(area1.Variables, v => v.Name == "Pump1_Run");
        Assert.NotNull(pump.Alarm);
        Assert.Equal(AlarmTriggerType.Condition, pump.Alarm!.TriggerType);

        var tank = Assert.Single(area1.Variables, v => v.Name == "Tank1_Level");
        Assert.NotNull(tank.Alarm);
        Assert.Equal(AlarmTriggerType.Limit, tank.Alarm!.TriggerType);
        Assert.Equal(90, tank.Alarm.HighLimit);
        Assert.Equal(95, tank.Alarm.HighHighLimit);
        Assert.Equal(10, tank.Alarm.LowLimit);
        Assert.Equal(5, tank.Alarm.LowLowLimit);
        Assert.Equal(2, result.AlarmCount);
    }

    [Fact]
    public void Import_MapsScreenFilesAndSymbolsFromResourcesFolder()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        Assert.Equal(2, result.ScreenCount);
        var screen = Assert.Single(result.Project.Screens, s => s.Name == "Overview");
        Assert.Equal("Overview", screen.Name);
        Assert.Equal(1024, screen.Width);
        Assert.Equal(768, screen.Height);

        // group/poly are skipped/recursed, leaving Button, Edit, base (from the group), and the unknown fallback.
        Assert.Equal(4, screen.Symbols.Count);

        var button = Assert.Single(screen.Symbols, s => s.Type == "button");
        Assert.Equal(10, button.X);
        Assert.Equal("Pump1_Run", button.VariablePath);
        Assert.Equal("Pump1_Run", button.Label);

        var editbox = Assert.Single(screen.Symbols, s => s.Type == "editbox");
        Assert.Equal("Tank1_Level", editbox.VariablePath);

        var baseRect = Assert.Single(screen.Symbols, s => s.Fill == "#0A141E");
        Assert.Equal("rect", baseRect.Type);
    }

    [Fact]
    public void Import_UnknownScreenObjectType_FallsBackToRectAndWarns()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        var screen = Assert.Single(result.Project.Screens, s => s.Name == "Overview");
        var unknown = Assert.Single(screen.Symbols, s => s.Fill == "#010203");
        Assert.Equal("rect", unknown.Type);
        Assert.Contains(result.Warnings, w => w.Contains("WeirdFutureWidget"));
    }

    [Fact]
    public void Import_StartupScreenFromProjectExecution_IsOrderedFirst()
    {
        var movprj = WriteProject();
        // Add a second top-level screen that sorts alphabetically before "Overview" to prove re-ordering happens.
        var resourcesDir = Path.Combine(_root, "RESOURCES", "DemoPlant");
        File.WriteAllText(Path.Combine(resourcesDir, "AAA_Other.movscr"), """
            <?xml version="1.0" encoding="ISO-8859-1" ?>
            <MovResource>
              <General SizeX="800" SizeY="600"/>
              <ListSynopticDraw/>
            </MovResource>
            """);

        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        // WriteProject() also includes RESOURCES\DemoPlant\IO\Lights.movscr by default.
        Assert.Equal(3, result.Project.Screens.Count);
        Assert.Equal("Overview", result.Project.Screens[0].Name);
    }

    [Fact]
    public void Import_ScansScreenSubfoldersRecursivelyAndPrefixesNamesWithRelativePath()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        Assert.Equal(2, result.Project.Screens.Count);
        Assert.Contains(result.Project.Screens, s => s.Name == "Overview");

        var nested = Assert.Single(result.Project.Screens, s => s.Name != "Overview");
        Assert.Equal("IO/Lights", nested.Name);
        Assert.Single(nested.Symbols);
    }

    [Fact]
    public void Import_DataLoggerGroup_EnablesDataLoggingOnReferencedVariableWithDeadBandAndRetention()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        Assert.Equal(2, result.DataLoggerGroupCount); // Log5sec + Hoover groups
        var tank = Assert.Single(result.Project.Folder.Folders, f => f.Name == "Area1")
            .Variables.Single(v => v.Name == "Tank1_Level");

        Assert.NotNull(tank.DataLogging);
        Assert.True(tank.DataLogging!.Enabled);
        Assert.Equal(2, tank.DataLogging.Hysteresis);
        Assert.Equal(TimeSpan.FromDays(7), tank.DataLogging.MaxAge);
    }

    [Fact]
    public void Import_DataLoggerGroup_WithoutDeadBand_LeavesHysteresisZero()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        var pump = Assert.Single(result.Project.Folder.Folders, f => f.Name == "Area1")
            .Variables.Single(v => v.Name == "Pump1_Run");

        Assert.NotNull(pump.DataLogging);
        Assert.Equal(0, pump.DataLogging!.Hysteresis);
        Assert.Equal(TimeSpan.FromDays(180), pump.DataLogging.MaxAge);
    }

    [Fact]
    public void Import_DataLoggerGroup_UndefinedVariableReference_Warns()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        Assert.Contains(result.Warnings, w => w.Contains("Hoover") && w.Contains("UndefinedVar"));
    }

    [Fact]
    public void Import_RecipeDefinition_MapsToRecipeConfigSkippingRecipeIndexColumn()
    {
        var movprj = WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        var recipe = Assert.Single(result.Project.Recipes);
        Assert.Equal("SceneRecipe", recipe.Name);
        Assert.Equal(2, recipe.Variables.Count);
        Assert.Contains(recipe.Variables, v => v.VariablePath == "Pump1_Run");
        Assert.Contains(recipe.Variables, v => v.VariablePath == "LineSpeed");
        Assert.DoesNotContain(recipe.Variables, v => v.VariablePath == "SceneRecipe");
        Assert.Equal(1, result.RecipeCount);
    }

    [Fact]
    public void Import_MissingSiblingFiles_ReportsWarningsButDoesNotThrow()
    {
        var movprj = WriteProject(includeResources: false);
        // Delete the .movrealtimedb and .movalr to simulate a partial/broken project folder.
        File.Delete(Path.Combine(_root, "DemoPlant.movrealtimedb"));
        File.Delete(Path.Combine(_root, "DemoPlant.movalr"));

        var result = MoviconProjectImporter.ImportFromProjectFile(movprj);

        Assert.True(result.IsEmpty);
        Assert.Contains(result.Warnings, w => w.Contains(".movrealtimedb"));
        Assert.Contains(result.Warnings, w => w.Contains("RESOURCES"));
    }

    [Fact]
    public void ImportFromProjectFile_AcceptsFolderPath()
    {
        WriteProject();
        var result = MoviconProjectImporter.ImportFromProjectFile(_root);
        Assert.False(result.IsEmpty);
        Assert.True(result.TagCount > 0);
    }
}
