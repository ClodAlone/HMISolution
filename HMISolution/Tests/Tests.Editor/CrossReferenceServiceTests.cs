using Xunit;
using ServerEditorWeb.Services;
using SharedModels;

namespace Tests.Editor;

public class CrossReferenceServiceTests
{
    private static CrossReferenceService CreateService() => new();

    private static NodeModel CreateTestModel()
    {
        return new NodeModel
        {
            Folder = new Folder
            {
                Name = "Root",
                Folders = new()
                {
                    new Folder
                    {
                        Name = "Plant",
                        Variables = new()
                        {
                            new Variable { Name = "Temperature", Type = "Double" },
                            new Variable { Name = "Pressure", Type = "Double" }
                        },
                        Folders = new()
                        {
                            new Folder
                            {
                                Name = "Motor",
                                Variables = new()
                                {
                                    new Variable { Name = "Speed", Type = "Double" },
                                    new Variable { Name = "Running", Type = "Boolean" }
                                }
                            }
                        }
                    }
                },
                Variables = new()
                {
                    new Variable { Name = "SystemReady", Type = "Boolean" }
                }
            },
            Server = new ServerSettings()
        };
    }

    // ──────────────────────────────────────────────────────────────
    // GetAllVariablePaths
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void GetAllVariablePaths_ReturnsAllPaths()
    {
        var svc = CreateService();
        var model = CreateTestModel();

        var paths = svc.GetAllVariablePaths(model);

        Assert.Contains("Root.SystemReady", paths);
        Assert.Contains("Root.Plant.Temperature", paths);
        Assert.Contains("Root.Plant.Pressure", paths);
        Assert.Contains("Root.Plant.Motor.Speed", paths);
        Assert.Contains("Root.Plant.Motor.Running", paths);
        Assert.Equal(5, paths.Count);
    }

    [Fact]
    public void GetAllVariablePaths_EmptyFolder_ReturnsEmpty()
    {
        var svc = CreateService();
        var model = new NodeModel
        {
            Folder = new Folder { Name = "Root" },
            Server = new ServerSettings()
        };

        var paths = svc.GetAllVariablePaths(model);
        Assert.Empty(paths);
    }

    [Fact]
    public void GetAllVariablePaths_NullModel_ReturnsEmpty()
    {
        var svc = CreateService();
        var paths = svc.GetAllVariablePaths(null!);
        Assert.Empty(paths);
    }

    [Fact]
    public void GetAllVariablePaths_NullFolder_ReturnsEmpty()
    {
        var svc = CreateService();
        var model = new NodeModel { Server = new ServerSettings() };

        var paths = svc.GetAllVariablePaths(model);
        Assert.Empty(paths);
    }

    // ──────────────────────────────────────────────────────────────
    // FindReferences — Screen bindings
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void FindReferences_ScreenVariablePath_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Screens.Add(new ScreenConfig
        {
            Name = "Main",
            Symbols = new()
            {
                new ScreenSymbol { Id = "s1", Type = "editbox", VariablePath = "Root.Plant.Temperature" }
            }
        });

        var refs = svc.FindReferences(model, "Root.Plant.Temperature");

        Assert.Single(refs);
        Assert.Equal("Screen", refs[0].Category);
        Assert.Equal("Main", refs[0].Location);
    }

    [Fact]
    public void FindReferences_ScreenHdaVariablePath_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Screens.Add(new ScreenConfig
        {
            Name = "Trends",
            Symbols = new()
            {
                new ScreenSymbol
                {
                    Id = "t1",
                    Type = "trend",
                    HdaVariablePaths = new() { "Root.Plant.Temperature", "Root.Plant.Pressure" }
                }
            }
        });

        var refs = svc.FindReferences(model, "Root.Plant.Pressure");

        Assert.Single(refs);
        Assert.Contains("HDA", refs[0].Detail);
    }

    [Fact]
    public void FindReferences_ScreenTrendPen_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Screens.Add(new ScreenConfig
        {
            Name = "Trends",
            Symbols = new()
            {
                new ScreenSymbol
                {
                    Id = "t1",
                    Type = "trend",
                    TrendPens = new()
                    {
                        new TrendPen { VariablePath = "Root.Plant.Motor.Speed", Label = "Speed" }
                    }
                }
            }
        });

        var refs = svc.FindReferences(model, "Root.Plant.Motor.Speed");

        Assert.Single(refs);
        Assert.Contains("Trend pen", refs[0].Detail);
    }

    [Fact]
    public void FindReferences_ScreenCommand_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Screens.Add(new ScreenConfig
        {
            Name = "Main",
            Symbols = new()
            {
                new ScreenSymbol
                {
                    Id = "btn1",
                    Type = "button",
                    Commands = new()
                    {
                        new SymbolCommand { Action = "SetVariable", VariablePath = "Root.Plant.Motor.Running" }
                    }
                }
            }
        });

        var refs = svc.FindReferences(model, "Root.Plant.Motor.Running");
        Assert.Single(refs);
        Assert.Contains("Command", refs[0].Detail);
    }

    [Fact]
    public void FindReferences_AnimationTrigger_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Screens.Add(new ScreenConfig
        {
            Name = "Main",
            Symbols = new()
            {
                new ScreenSymbol
                {
                    Id = "s1",
                    Type = "rect",
                    Animations = new()
                    {
                        new SymbolAnimation { Name = "Flash", TriggerVariable = "Root.Plant.Motor.Running" }
                    }
                }
            }
        });

        var refs = svc.FindReferences(model, "Root.Plant.Motor.Running");
        Assert.Single(refs);
        Assert.Contains("Animation", refs[0].Detail);
    }

    // ──────────────────────────────────────────────────────────────
    // FindReferences — Script
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void FindReferences_ScriptRead_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Scripts.Add(new ScriptConfig
        {
            Name = "Monitor",
            Code = "var temp = ReadDouble(\"Root.Plant.Temperature\");\nLog(\"temp=\" + temp);"
        });

        var refs = svc.FindReferences(model, "Root.Plant.Temperature");
        Assert.Single(refs);
        Assert.Equal("Script", refs[0].Category);
        Assert.Contains("Read", refs[0].Detail);
    }

    [Fact]
    public void FindReferences_ScriptWrite_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Scripts.Add(new ScriptConfig
        {
            Name = "Controller",
            Code = "Write(\"Root.Plant.Motor.Speed\", 100);"
        });

        var refs = svc.FindReferences(model, "Root.Plant.Motor.Speed");
        Assert.Single(refs);
        Assert.Contains("Write", refs[0].Detail);
    }

    // ──────────────────────────────────────────────────────────────
    // FindReferences — PLC Programs
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void FindReferences_PlcReadSt_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.PlcPrograms.Add(new PlcProgramConfig
        {
            Name = "Control",
            Language = "ST",
            Code = "speed := READ('Root.Plant.Motor.Speed');"
        });

        var refs = svc.FindReferences(model, "Root.Plant.Motor.Speed");
        Assert.Single(refs);
        Assert.Equal("PLC Program", refs[0].Category);
        Assert.Contains("READ", refs[0].Detail);
    }

    [Fact]
    public void FindReferences_PlcWriteSt_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.PlcPrograms.Add(new PlcProgramConfig
        {
            Name = "Control",
            Language = "ST",
            Code = "WRITE('Root.Plant.Motor.Speed', 50);"
        });

        var refs = svc.FindReferences(model, "Root.Plant.Motor.Speed");
        Assert.Single(refs);
        Assert.Contains("WRITE", refs[0].Detail);
    }

    // ──────────────────────────────────────────────────────────────
    // FindReferences — Alarms
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void FindReferences_AlarmOnVariable_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Folder!.Folders[0].Variables[0].Alarm = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 100,
            LowLimit = 10
        };

        var refs = svc.FindReferences(model, "Root.Plant.Temperature");
        Assert.Single(refs);
        Assert.Equal("Alarm", refs[0].Category);
        Assert.Contains("Limit", refs[0].Detail);
    }

    // ──────────────────────────────────────────────────────────────
    // FindReferences — Data Logging
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void FindReferences_DataLogging_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Folder!.Folders[0].Variables[0].DataLogging = new DataLoggingConfig
        {
            Enabled = true,
            Hysteresis = 1.0
        };

        var refs = svc.FindReferences(model, "Root.Plant.Temperature");
        Assert.Single(refs);
        Assert.Equal("Data Logging", refs[0].Category);
    }

    // ──────────────────────────────────────────────────────────────
    // FindReferences — Recipes
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void FindReferences_Recipe_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Recipes.Add(new RecipeConfig
        {
            Name = "Batch1",
            Variables = new()
            {
                new RecipeVariable { Index = 0, VariablePath = "Root.Plant.Temperature", DisplayName = "Temp" }
            }
        });

        var refs = svc.FindReferences(model, "Root.Plant.Temperature");
        Assert.Single(refs);
        Assert.Equal("Recipe", refs[0].Category);
        Assert.Equal("Batch1", refs[0].Location);
    }

    // ──────────────────────────────────────────────────────────────
    // FindReferences — Schedulers
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void FindReferences_SchedulerCommand_Found()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Schedulers.Add(new SchedulerConfig
        {
            Name = "DayShift",
            Commands = new()
            {
                new SymbolCommand { Action = "SetVariable", VariablePath = "Root.Plant.Motor.Running" }
            }
        });

        var refs = svc.FindReferences(model, "Root.Plant.Motor.Running");
        Assert.Single(refs);
        Assert.Equal("Scheduler", refs[0].Category);
    }

    // ──────────────────────────────────────────────────────────────
    // FindReferences — Prefix match
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void FindReferences_PrefixMatch_FindsChildren()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Screens.Add(new ScreenConfig
        {
            Name = "Main",
            Symbols = new()
            {
                new ScreenSymbol { Id = "s1", Type = "editbox", VariablePath = "Root.Plant.Motor.Speed" },
                new ScreenSymbol { Id = "s2", Type = "led", VariablePath = "Root.Plant.Motor.Running" },
                new ScreenSymbol { Id = "s3", Type = "editbox", VariablePath = "Root.Plant.Temperature" }
            }
        });

        var refs = svc.FindReferences(model, "Root.Plant.Motor", prefixMatch: true);

        Assert.Equal(2, refs.Count);
    }

    // ──────────────────────────────────────────────────────────────
    // FindReferences — Edge cases
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void FindReferences_NullModel_ReturnsEmpty()
    {
        var svc = CreateService();
        var refs = svc.FindReferences(null!, "Root.Plant.Temperature");
        Assert.Empty(refs);
    }

    [Fact]
    public void FindReferences_EmptyPath_ReturnsEmpty()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        var refs = svc.FindReferences(model, "");
        Assert.Empty(refs);
    }

    [Fact]
    public void FindReferences_NoReferences_ReturnsEmpty()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        var refs = svc.FindReferences(model, "Root.Plant.Temperature");
        Assert.Empty(refs); // No screens, scripts, etc. reference it
    }

    [Fact]
    public void FindReferences_CaseInsensitive()
    {
        var svc = CreateService();
        var model = CreateTestModel();
        model.Screens.Add(new ScreenConfig
        {
            Name = "Main",
            Symbols = new()
            {
                new ScreenSymbol { Id = "s1", Type = "editbox", VariablePath = "root.plant.temperature" }
            }
        });

        var refs = svc.FindReferences(model, "Root.Plant.Temperature");
        Assert.Single(refs);
    }

    // ──────────────────────────────────────────────────────────────
    // FindReferences — Multiple references
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void FindReferences_MultipleCategories_ReturnsAll()
    {
        var svc = CreateService();
        var model = CreateTestModel();

        // Screen reference
        model.Screens.Add(new ScreenConfig
        {
            Name = "Main",
            Symbols = new()
            {
                new ScreenSymbol { Id = "s1", Type = "editbox", VariablePath = "Root.Plant.Temperature" }
            }
        });

        // Script reference
        model.Scripts.Add(new ScriptConfig
        {
            Name = "Monitor",
            Code = "var t = ReadDouble(\"Root.Plant.Temperature\");"
        });

        // Alarm reference
        model.Folder!.Folders[0].Variables[0].Alarm = new AlarmConfig
        {
            TriggerType = AlarmTriggerType.Limit,
            HighLimit = 100,
            LowLimit = 10
        };

        var refs = svc.FindReferences(model, "Root.Plant.Temperature");

        Assert.Equal(3, refs.Count);
        Assert.Contains(refs, r => r.Category == "Screen");
        Assert.Contains(refs, r => r.Category == "Script");
        Assert.Contains(refs, r => r.Category == "Alarm");
    }
}
