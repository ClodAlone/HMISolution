// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Xunit;
using ServerEditorWeb.Services;
using SharedModels;

namespace Tests.Editor;

public class ClipboardServiceTests
{
    // ──────────────────────────────────────────────────────────────
    // Initial state
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void InitialState_HasNoContent()
    {
        var svc = new ClipboardService();
        Assert.False(svc.HasContent);
        Assert.Null(svc.ContentType);
        Assert.Null(svc.ContentJson);
        Assert.Null(svc.Label);
    }

    // ──────────────────────────────────────────────────────────────
    // CopyVariable
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CopyVariable_SetsContent()
    {
        var svc = new ClipboardService();
        var variable = new Variable { Name = "Temp", Type = "Double", Value = 25.0 };

        svc.CopyVariable(variable);

        Assert.True(svc.HasContent);
        Assert.Equal("Variable", svc.ContentType);
        Assert.Equal("Temp", svc.Label);
        Assert.NotNull(svc.ContentJson);
        Assert.Contains("Temp", svc.ContentJson);
    }

    [Fact]
    public void CopyVariable_PasteReturnsClone()
    {
        var svc = new ClipboardService();
        var original = new Variable { Name = "Pressure", Type = "Double", Value = 3.14 };

        svc.CopyVariable(original);
        var pasted = svc.PasteVariable();

        Assert.NotNull(pasted);
        Assert.Equal("Pressure (Copy)", pasted.Name);
        Assert.Equal("Double", pasted.Type);
        // Pasted should be a different instance
        Assert.NotSame(original, pasted);
    }

    // ──────────────────────────────────────────────────────────────
    // CopyFolder
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CopyFolder_SetsContent()
    {
        var svc = new ClipboardService();
        var folder = new Folder
        {
            Name = "Sensors",
            Variables = new() { new Variable { Name = "T1", Type = "Double" } }
        };

        svc.CopyFolder(folder);

        Assert.True(svc.HasContent);
        Assert.Equal("Folder", svc.ContentType);
        Assert.Equal("Sensors", svc.Label);
    }

    [Fact]
    public void CopyFolder_PasteReturnsClone()
    {
        var svc = new ClipboardService();
        var folder = new Folder
        {
            Name = "Actuators",
            Variables = new() { new Variable { Name = "Motor1", Type = "Boolean" } }
        };

        svc.CopyFolder(folder);
        var pasted = svc.PasteFolder();

        Assert.NotNull(pasted);
        Assert.Equal("Actuators (Copy)", pasted.Name);
        Assert.Single(pasted.Variables);
    }

    // ──────────────────────────────────────────────────────────────
    // CopyScript
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CopyScript_SetsContent()
    {
        var svc = new ClipboardService();
        var script = new ScriptConfig { Name = "Monitor", Code = "Log(\"hello\");", IntervalMs = 1000 };

        svc.CopyScript(script);

        Assert.Equal("Script", svc.ContentType);
        Assert.Equal("Monitor", svc.Label);
    }

    [Fact]
    public void CopyScript_PasteReturnsClone()
    {
        var svc = new ClipboardService();
        var script = new ScriptConfig { Name = "Controller", Code = "var x = 1;", IntervalMs = 500 };

        svc.CopyScript(script);
        var pasted = svc.PasteScript();

        Assert.NotNull(pasted);
        Assert.Equal("Controller (Copy)", pasted.Name);
        Assert.Equal("var x = 1;", pasted.Code);
    }

    // ──────────────────────────────────────────────────────────────
    // CopyPlcProgram
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CopyPlcProgram_SetsContent()
    {
        var svc = new ClipboardService();
        var plc = new PlcProgramConfig { Name = "Main", Code = "counter := counter + 1;", Language = "ST" };

        svc.CopyPlcProgram(plc);

        Assert.Equal("PlcProgram", svc.ContentType);
        Assert.Equal("Main", svc.Label);
    }

    [Fact]
    public void CopyPlcProgram_PasteReturnsClone()
    {
        var svc = new ClipboardService();
        var plc = new PlcProgramConfig { Name = "Ctrl", Code = "x := 1;", Language = "ST", IntervalMs = 100 };

        svc.CopyPlcProgram(plc);
        var pasted = svc.PastePlcProgram();

        Assert.NotNull(pasted);
        Assert.Equal("Ctrl (Copy)", pasted.Name);
        Assert.Equal("ST", pasted.Language);
    }

    // ──────────────────────────────────────────────────────────────
    // CopyScreen
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CopyScreen_SetsContent()
    {
        var svc = new ClipboardService();
        var screen = new ScreenConfig
        {
            Name = "Dashboard",
            Width = 1920,
            Height = 1080,
            Symbols = new()
            {
                new ScreenSymbol { Id = "s1", Type = "label", Label = "Title" }
            }
        };

        svc.CopyScreen(screen);

        Assert.Equal("Screen", svc.ContentType);
        Assert.Equal("Dashboard", svc.Label);
    }

    [Fact]
    public void CopyScreen_PastePreservesSymbols()
    {
        var svc = new ClipboardService();
        var screen = new ScreenConfig
        {
            Name = "Main",
            Width = 800,
            Height = 600,
            Symbols = new()
            {
                new ScreenSymbol { Id = "s1", Type = "editbox", VariablePath = "Root.Temp" },
                new ScreenSymbol { Id = "s2", Type = "button" }
            }
        };

        svc.CopyScreen(screen);
        var pasted = svc.PasteScreen();

        Assert.NotNull(pasted);
        Assert.Equal("Main (Copy)", pasted.Name);
        Assert.Equal(2, pasted.Symbols.Count);
    }

    // ──────────────────────────────────────────────────────────────
    // CopyRecipe
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CopyRecipe_SetsContent()
    {
        var svc = new ClipboardService();
        var recipe = new RecipeConfig
        {
            Name = "Batch1",
            Variables = new() { new RecipeVariable { Index = 0, VariablePath = "Root.Temp" } }
        };

        svc.CopyRecipe(recipe);

        Assert.Equal("Recipe", svc.ContentType);
        Assert.Equal("Batch1", svc.Label);
    }

    // ──────────────────────────────────────────────────────────────
    // CopyScheduler
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CopyScheduler_SetsContent()
    {
        var svc = new ClipboardService();
        var scheduler = new SchedulerConfig { Name = "DayShift" };

        svc.CopyScheduler(scheduler);

        Assert.Equal("Scheduler", svc.ContentType);
        Assert.Equal("DayShift", svc.Label);
    }

    // ──────────────────────────────────────────────────────────────
    // CopyReport
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CopyReport_SetsContent()
    {
        var svc = new ClipboardService();
        var report = new ReportConfig { Name = "Daily" };

        svc.CopyReport(report);

        Assert.Equal("Report", svc.ContentType);
        Assert.Equal("Daily", svc.Label);
    }

    // ──────────────────────────────────────────────────────────────
    // CopySymbols
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CopySymbols_SetsContent()
    {
        var svc = new ClipboardService();
        var symbols = new List<ScreenSymbol>
        {
            new() { Id = "s1", Type = "label", Label = "A" },
            new() { Id = "s2", Type = "label", Label = "B" }
        };

        svc.CopySymbols(symbols);

        Assert.Equal("Symbols", svc.ContentType);
        Assert.True(svc.HasContent);
    }

    [Fact]
    public void CopySymbols_PasteReturnsAll()
    {
        var svc = new ClipboardService();
        var symbols = new List<ScreenSymbol>
        {
            new() { Id = "s1", Type = "label" },
            new() { Id = "s2", Type = "rect" },
            new() { Id = "s3", Type = "button" }
        };

        svc.CopySymbols(symbols);
        var pasted = svc.PasteSymbols();

        Assert.NotNull(pasted);
        Assert.Equal(3, pasted.Count);
    }

    // ──────────────────────────────────────────────────────────────
    // Changed event
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CopyVariable_FiresChangedEvent()
    {
        var svc = new ClipboardService();
        int fired = 0;
        svc.Changed += () => fired++;

        svc.CopyVariable(new Variable { Name = "V1", Type = "Double" });

        Assert.Equal(1, fired);
    }

    [Fact]
    public void CopyScreen_FiresChangedEvent()
    {
        var svc = new ClipboardService();
        int fired = 0;
        svc.Changed += () => fired++;

        svc.CopyScreen(new ScreenConfig { Name = "S1" });

        Assert.Equal(1, fired);
    }

    // ──────────────────────────────────────────────────────────────
    // Overwrite behavior
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void Copy_OverwritesPreviousContent()
    {
        var svc = new ClipboardService();

        svc.CopyVariable(new Variable { Name = "V1", Type = "Double" });
        Assert.Equal("Variable", svc.ContentType);
        Assert.Equal("V1", svc.Label);

        svc.CopyScript(new ScriptConfig { Name = "S1" });
        Assert.Equal("Script", svc.ContentType);
        Assert.Equal("S1", svc.Label);
    }

    [Fact]
    public void PasteVariable_WhenContentIsScript_ReturnsNull()
    {
        var svc = new ClipboardService();
        svc.CopyScript(new ScriptConfig { Name = "S1" });

        var pasted = svc.PasteVariable();
        Assert.Null(pasted);
    }
}
