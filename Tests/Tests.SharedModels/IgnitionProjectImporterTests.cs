// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Xunit;
using SharedModels;
using SharedModels.Import;

namespace Tests.SharedModels;

public class IgnitionProjectImporterTests
{
    private const string SampleTagExportJson = """
        {
          "name": "default",
          "tagType": "Provider",
          "tags": [
            {
              "name": "Area1",
              "tagType": "Folder",
              "tags": [
                {
                  "name": "Pump1_Run",
                  "tagType": "AtomicTag",
                  "dataType": "Boolean",
                  "valueSource": "memory",
                  "value": false
                },
                {
                  "name": "Tank1_Level",
                  "tagType": "AtomicTag",
                  "dataType": "Float8",
                  "valueSource": "opc",
                  "opcServer": "Ignition OPC UA Server",
                  "opcItemPath": "[default]Area1/Tank1Level",
                  "engUnit": "%",
                  "alarms": [
                    { "name": "Hi", "mode": "AboveValue", "setpointA": 90, "displayPath": "Tank1 High" },
                    { "name": "HiHi", "mode": "AboveValue", "setpointA": 95 },
                    { "name": "Lo", "mode": "BelowValue", "setpointA": 10 },
                    { "name": "LoLo", "mode": "BelowValue", "setpointA": 5 }
                  ]
                }
              ]
            },
            {
              "name": "LineSpeed",
              "tagType": "AtomicTag",
              "dataType": "Int4",
              "valueSource": "memory",
              "value": 0
            }
          ]
        }
        """;

    private const string SampleViewJson = """
        {
          "props": { "defaultSize": { "width": 1024, "height": 768 } },
          "root": {
            "type": "ia.container.coord",
            "meta": { "name": "root" },
            "children": [
              {
                "type": "ia.shapes.rectangle",
                "meta": { "name": "rect1" },
                "position": { "x": 10, "y": 20, "width": 100, "height": 50 },
                "props": {},
                "propConfig": {
                  "props.style.fill": {
                    "binding": { "type": "tag", "config": { "tagPath": "[default]Area1/Pump1_Run" } }
                  }
                }
              },
              {
                "type": "ia.display.led",
                "meta": { "name": "led1" },
                "position": { "x": 150, "y": 20, "width": 30, "height": 30 },
                "props": { "text": "Pump status" }
              },
              {
                "type": "ia.future.widget",
                "meta": { "name": "future1" },
                "position": { "x": 0, "y": 0, "width": 10, "height": 10 }
              }
            ]
          }
        }
        """;

    [Fact]
    public void ImportTags_BuildsFolderTree()
    {
        var result = IgnitionProjectImporter.ImportTags(SampleTagExportJson);

        Assert.Equal(3, result.TagCount);
        Assert.Equal(1, result.FolderCount);

        var area1 = Assert.Single(result.Root.Folders);
        Assert.Equal("Area1", area1.Name);
        Assert.Equal(2, area1.Variables.Count);

        var pump = Assert.Single(area1.Variables, v => v.Name == "Pump1_Run");
        Assert.Equal("Boolean", pump.Type);

        var lineSpeed = Assert.Single(result.Root.Variables, v => v.Name == "LineSpeed");
        Assert.Equal("Int32", lineSpeed.Type);
    }

    [Fact]
    public void ImportTags_MapsOpcBindingIntoDescription()
    {
        var result = IgnitionProjectImporter.ImportTags(SampleTagExportJson);
        var area1 = Assert.Single(result.Root.Folders);
        var tankLevel = Assert.Single(area1.Variables, v => v.Name == "Tank1_Level");

        Assert.Equal("Double", tankLevel.Type);
        Assert.Contains("[default]Area1/Tank1Level", tankLevel.Description);
    }

    [Fact]
    public void ImportTags_MergesFourLevelAlarmsFromMultipleAlarmEntries()
    {
        var result = IgnitionProjectImporter.ImportTags(SampleTagExportJson);
        var area1 = Assert.Single(result.Root.Folders);
        var tankLevel = Assert.Single(area1.Variables, v => v.Name == "Tank1_Level");

        Assert.NotNull(tankLevel.Alarm);
        Assert.Equal(90, tankLevel.Alarm!.HighLimit);
        Assert.Equal(95, tankLevel.Alarm.HighHighLimit);
        Assert.Equal(10, tankLevel.Alarm.LowLimit);
        Assert.Equal(5, tankLevel.Alarm.LowLowLimit);
        Assert.Equal("Tank1 High", tankLevel.Alarm.Message);
        Assert.Equal(1, result.AlarmCount);
    }

    [Fact]
    public void ImportView_MapsComponentsAndSkipsContainer()
    {
        var result = IgnitionProjectImporter.ImportView(SampleViewJson, "MainView");

        Assert.Equal("MainView", result.Screen.Name);
        Assert.Equal(1024, result.Screen.Width);
        Assert.Equal(768, result.Screen.Height);
        Assert.Equal(3, result.SymbolCount);

        var rect = Assert.Single(result.Screen.Symbols, s => s.Id == "rect1");
        Assert.Equal("rect", rect.Type);
        Assert.Equal(10, rect.X);
        Assert.Equal("[default]Area1/Pump1_Run", rect.VariablePath);

        var led = Assert.Single(result.Screen.Symbols, s => s.Id == "led1");
        Assert.Equal("indicator", led.Type);
        Assert.Equal("Pump status", led.Label);
    }

    [Fact]
    public void ImportView_UnknownComponentType_FallsBackToRectAndWarns()
    {
        var result = IgnitionProjectImporter.ImportView(SampleViewJson, "MainView");
        var unknown = Assert.Single(result.Screen.Symbols, s => s.Id == "future1");
        Assert.Equal("rect", unknown.Type);
        Assert.Contains(result.Warnings, w => w.Contains("ia.future.widget"));
    }

    [Fact]
    public void ImportTags_EmptyOrInvalidJson_ReportsWarning()
    {
        var result = IgnitionProjectImporter.ImportTags("{}");
        Assert.Equal(0, result.TagCount);
        Assert.NotEmpty(result.Warnings);

        var badJson = IgnitionProjectImporter.ImportTags("not json");
        Assert.NotEmpty(badJson.Warnings);
    }

    [Fact]
    public void ImportView_NoRoot_ReportsWarning()
    {
        var result = IgnitionProjectImporter.ImportView("{}", "Empty");
        Assert.NotEmpty(result.Warnings);
        Assert.Equal(0, result.SymbolCount);
    }
}
