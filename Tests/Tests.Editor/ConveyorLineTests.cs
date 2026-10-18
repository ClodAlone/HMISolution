using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class ConveyorLineTests
{
    private static readonly NodeModel Bundle = AiResourceBundle.Parse(ConveyorLineTemplate.Json, "AI_Task");
    private static readonly ScriptRunner<object> Run = CSharpScript.Create(ConveyorLineTemplate.SimulationCode.Replace("__TASK__", "AI_Task", StringComparison.Ordinal),
        ScriptOptions.Default.AddImports("System"), typeof(ConveyorGlobals)).CreateDelegate();

    [Fact]
    public void ImportTwice_PreservesProjectAndNamespacesAllDependencies()
    {
        var editor = new NodeEditorService(); editor.NewFile(); var project = editor.ActiveProject!;
        var settings = project.Model.Server; var startup = settings.StartupScreen;
        for (var i = 0; i < 2; i++) { var result = editor.ImportAiResources(project, ConveyorLineTemplate.Json); Assert.True(result.success, result.message); }
        editor.RefreshJsonFromTree(); var model = JsonSerializer.Deserialize<NodeModel>(editor.JsonEditorText)!;
        Assert.Same(settings, project.Model.Server); Assert.Equal(startup, settings.StartupScreen);
        Assert.Equal(2, model.Screens.Count); Assert.Equal(2, model.Scripts.Count); Assert.Equal(2, model.Folder.Folders.Count);
        var ids = new HashSet<string>();
        for (var i = 0; i < 2; i++)
        {
            var prefix = i == 0 ? "AI_Task" : "AI_Task_2"; var screen = model.Screens[i];
            Assert.Equal(prefix + "_Conveyor Production Line", screen.Name); Assert.Equal(prefix, screen.Group);
            Assert.Equal((1440d, 1040d), (screen.Width, screen.Height));
            Assert.Equal(screen.Symbols.Count, screen.Symbols.Select(s => s.Id).Distinct().Count());
            var folder = Assert.Single(model.Folder.Folders[i].Folders); Assert.Equal("ConveyorLine", folder.Name);
            Assert.All(folder.Variables, v => Assert.Equal("ReadWrite", v.Access));
            var script = model.Scripts[i]; Assert.False(script.Enabled); Assert.Equal(250, script.IntervalMs);
            Assert.Equal(prefix + "_ConveyorLineSimulation", script.Name);
            Assert.Contains($"const string p = \"{prefix}.ConveyorLine.\";", script.Code);
            Assert.All(screen.Symbols.Where(s => !string.IsNullOrEmpty(s.VariablePath)), s => Assert.StartsWith(prefix + ".ConveyorLine.", s.VariablePath));
            foreach (var symbol in screen.Symbols.Where(s => s.Type == "svg"))
            {
                Assert.DoesNotContain("__TASK__", symbol.SvgContent!);
                var svg = XElement.Parse(symbol.SvgContent!);
                foreach (var marker in svg.Descendants().Attributes().Where(a => a.Name.LocalName is "data-conveyor-line" or "data-conveyor-running"))
                    Assert.Equal(prefix + "_cl", marker.Value);
                foreach (var id in svg.Descendants().Attributes("id"))
                {
                    Assert.StartsWith(prefix + "_cl_", id.Value); Assert.True(ids.Add(id.Value));
                }
            }
        }
        Assert.Empty(new ProjectValidationService(new SyntaxCheckService(), new CrossReferenceService()).Validate(model).Where(i => i.Severity == IssueSeverity.Error));
    }

    [Fact]
    public void VisualPackages_ArePositionDrivenNotIndependentCssAndHaveDistinctRoutes()
    {
        var symbols = Bundle.Screens.Single().Symbols;
        ScreenSymbol Symbol(string id) => symbols.Single(s => s.Id == "conveyorline_" + id);
        Assert.Equal("conveyorline_header", symbols[0].Id);
        Assert.Equal("conveyor", Symbol("belt").Type); Assert.Equal("AI_Task.ConveyorLine.MotorOn", Symbol("belt").VariablePath);
        Assert.Equal("AI_Task.ConveyorLine.Jammed", Symbol("belt").ConveyorFaultPath);
        Assert.True(symbols.IndexOf(Symbol("belt")) < symbols.IndexOf(Symbol("artwork")));
        Assert.Equal("isometric", Symbol("motor").MotorAppearance); Assert.Equal("AI_Task.ConveyorLine.Jammed", Symbol("motor").MotorFaultPath);
        Assert.Equal("AI_Task.ConveyorLine.SensorOn", Symbol("sensor").VariablePath);
        Assert.Equal("AI_Task.ConveyorLine.DiverterOn", Symbol("diverter_active").VariablePath);
        Assert.Equal("value == true", Symbol("diverter_active").VisibilityBinding);
        var packages = symbols.Where(s => s.Id.StartsWith("conveyorline_package_", StringComparison.Ordinal)).ToArray();
        Assert.Equal(400, packages.Length);
        Assert.Equal("AI_Task.ConveyorLine.MotorOn", Symbol("motion_state").VariablePath);
        Assert.Equal("value == true", Symbol("motion_state").VisibilityBinding);
        Assert.Contains("data-conveyor-running=\"AI_Task_cl\"", Symbol("motion_state").SvgContent!);
        Assert.All(packages, s => { Assert.DoesNotContain("animation", s.SvgContent!); Assert.Empty(s.Animations); XElement.Parse(s.SvgContent!); });
        for (var slot = 0; slot < 4; slot++)
        {
            for (var route = 0; route < 2; route++)
            for (var bin = 0; bin < 50; bin++)
            {
                var package = Symbol($"package_{slot}_{route}_{bin}"); var lower = bin * 2 + route * 200;
                Assert.Equal("AI_Task.ConveyorLine.Slot" + slot, package.VariablePath);
                Assert.Equal($"value == {lower}", package.VisibilityBinding);
                var parcel = XElement.Parse(package.SvgContent!).Elements().Single();
                Assert.Equal("AI_Task_cl", (string?)parcel.Attribute("data-conveyor-line"));
                Assert.Equal(slot.ToString(CultureInfo.InvariantCulture), (string?)parcel.Attribute("data-conveyor-parcel"));
                Assert.Equal((bin * 2).ToString(CultureInfo.InvariantCulture), (string?)parcel.Attribute("data-position"));
                Assert.Equal(route.ToString(CultureInfo.InvariantCulture), (string?)parcel.Attribute("data-route"));
            }
        }
        Assert.NotEqual(Symbol("package_0_0_49").Y, Symbol("package_0_1_49").Y);
        Assert.Contains("#9af2ef", Symbol("package_0_0_0").SvgContent!); Assert.Contains("#ffb3a8", Symbol("package_0_1_0").SvgContent!);
        Assert.Equal((1d, 200d), (Symbol("batch").MinValue, Symbol("batch").MaxValue));
        Assert.Equal((2d, 8d, 2d), (Symbol("speed").MinValue, Symbol("speed").MaxValue, Symbol("speed").EditBoxStep));
    }

    [Theory]
    [InlineData("run", "RunRequested", "ToggleVariable", "")]
    [InlineData("reset", "ResetRequested", "WriteVariable", "true")]
    [InlineData("jam", "JamRequested", "WriteVariable", "true")]
    [InlineData("clear", "ClearJamRequested", "WriteVariable", "true")]
    public void Commands_TargetDeclaredBooleanInputs(string id, string variable, string action, string value)
    {
        var command = Assert.Single(Bundle.Screens.Single().Symbols.Single(s => s.Id == "conveyorline_" + id).Commands);
        Assert.Equal("Click", command.Trigger); Assert.Equal(action, command.Action); Assert.Equal(value, command.Value);
        Assert.Equal("AI_Task.ConveyorLine." + variable, command.VariablePath);
        Assert.Contains(Bundle.Folder.Folders.Single().Variables, v => v.Name == variable && v.Type == "Boolean");
    }

    [Fact]
    public async Task Packages_ActuallyMoveAndDetectorAndDiverterFollowTrackedPositions()
    {
        var g = new ConveyorGlobals(); g.Flag("RunRequested", true); await Run(g);
        Assert.Equal(0, g.Get("Slot0")); Assert.Equal(1, g.Get("CreatedTotal")); Assert.True(g.Bool("ArrivalOn"));
        await Run(g); Assert.Equal(4, g.Get("Slot0")); Assert.Equal(0, g.Get("GoodCount"));
        var sensor = false; var diverter = false; var rejectRoute = false;
        for (var i = 0; i < 65; i++)
        {
            await Run(g); Invariants(g); sensor |= g.Bool("SensorOn"); diverter |= g.Bool("DiverterOn");
            rejectRoute |= Enumerable.Range(0, 4).Any(s => g.Get("Slot" + s) >= 200);
        }
        Assert.True(sensor && diverter && rejectRoute); Assert.True(g.Get("GoodCount") > 0); Assert.True(g.Get("RejectCount") > 0);
    }

    [Theory]
    [InlineData(2, 200, 3)]
    [InlineData(4, 24, 3)]
    [InlineData(6, 17, 2)]
    [InlineData(8, 19, 5)]
    public async Task FullBatch_LongRunCountsExactlyOnceAndStopsWithNoActiveSlots(int speed, int batch, int every)
    {
        var g = new ConveyorGlobals(); g.Set("Speed", speed); g.Set("BatchSize", batch); g.Set("RejectEvery", every); g.Flag("RunRequested", true);
        var previousGood = 0d; var previousReject = 0d;
        for (var i = 0; i < batch * 15 + 60; i++)
        {
            await Run(g); Invariants(g);
            Assert.True(g.Get("GoodCount") >= previousGood); Assert.True(g.Get("RejectCount") >= previousReject);
            previousGood = g.Get("GoodCount"); previousReject = g.Get("RejectCount");
        }
        Assert.Equal(batch, g.Get("CreatedTotal")); Assert.Equal(batch / every, g.Get("RejectCount"));
        Assert.Equal(batch - batch / every, g.Get("GoodCount")); Assert.Equal(100, g.Get("Progress"));
        for (var slot = 0; slot < 4; slot++) Assert.Equal(-1, g.Get("Slot" + slot));
        Assert.False(g.Bool("RunRequested")); Stopped(g);
        Assert.Equal("Batch complete - Reset for a new batch", g.Values["AI_Task.ConveyorLine.Status"]);
        var elapsed = g.Get("Elapsed"); g.Flag("RunRequested", true);
        await Run(g); Assert.Equal(elapsed, g.Get("Elapsed")); Assert.Equal(batch, g.Get("GoodCount") + g.Get("RejectCount")); Stopped(g);
    }

    [Fact]
    public async Task ExitOnlyCounting_ClearsSlotImmediatelySoFollowingTicksCannotCountItAgain()
    {
        var g = new ConveyorGlobals(); g.Set("BatchSize", 2); g.Set("CreatedTotal", 2); g.Set("Slot0", 98); g.Set("Slot1", 298); g.Flag("RunRequested", true);
        await Run(g); Assert.Equal(1, g.Get("GoodCount")); Assert.Equal(1, g.Get("RejectCount"));
        Assert.Equal(-1, g.Get("Slot0")); Assert.Equal(-1, g.Get("Slot1"));
        for (var i = 0; i < 20; i++) await Run(g);
        Assert.Equal(1, g.Get("GoodCount")); Assert.Equal(1, g.Get("RejectCount")); Invariants(g);
    }

    [Fact]
    public async Task Jam_StopsMovementMotorAndArrivals_ThenClearResumesWithoutDoubleCounting()
    {
        var g = new ConveyorGlobals(); g.Flag("RunRequested", true);
        for (var i = 0; i < 39; i++) await Run(g);
        var state = Snapshot(g); g.Flag("JamRequested", true); await Run(g);
        Assert.True(g.Bool("Jammed")); Assert.True(g.Bool("RunRequested")); Assert.False(g.Bool("JamRequested")); Stopped(g);
        for (var i = 0; i < 10; i++) { await Run(g); AssertSnapshot(g, state); }
        Assert.Contains("JAM", g.Values["AI_Task.ConveyorLine.Status"].ToString());
        g.Flag("ClearJamRequested", true); await Run(g);
        Assert.False(g.Bool("Jammed")); Assert.False(g.Bool("ClearJamRequested")); Assert.True(g.Bool("MotorOn"));
        Assert.Equal(state["Elapsed"] + 1, g.Get("Elapsed"));
        for (var i = 0; i < 400; i++) { await Run(g); Invariants(g); }
        Assert.Equal(16, g.Get("GoodCount")); Assert.Equal(8, g.Get("RejectCount")); Assert.Equal(24, g.Get("CreatedTotal"));
    }

    [Fact]
    public async Task Pause_FreezesAllPackageAndCounterState_ResetRestoresAllNumericControlsAndPauses()
    {
        var g = new ConveyorGlobals(); g.Flag("RunRequested", true);
        for (var i = 0; i < 40; i++) await Run(g);
        var state = Snapshot(g); g.Flag("RunRequested", false);
        for (var i = 0; i < 5; i++) { await Run(g); Stopped(g); AssertSnapshot(g, state); }
        g.Set("BatchSize", 200); g.Set("Speed", 8); g.Set("RejectEvery", 5); g.Flag("JamRequested", true); await Run(g);
        g.Flag("ResetRequested", true); await Run(g); Stopped(g);
        var initial = new ConveyorGlobals();
        foreach (var v in Bundle.Folder.Folders.Single().Variables.Where(v => v.Type == "Double")) Assert.Equal(initial.Get(v.Name), g.Get(v.Name));
        Assert.All(new[] { "RunRequested", "ResetRequested", "Jammed", "JamRequested", "ClearJamRequested" }, n => Assert.False(g.Bool(n)));
        Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.ConveyorLine.Status"]); Invariants(g);
    }

    [Theory]
    [InlineData("Slot0", double.NaN)]
    [InlineData("Slot0", 100)]
    [InlineData("Slot0", 199)]
    [InlineData("Slot1", -.5)]
    [InlineData("CreatedTotal", 1)]
    [InlineData("GoodCount", -1)]
    [InlineData("ArrivalClock", 15)]
    [InlineData("Elapsed", double.PositiveInfinity)]
    [InlineData("BatchSize", 0)]
    [InlineData("BatchSize", 201)]
    [InlineData("BatchSize", 1.5)]
    [InlineData("Speed", 3)]
    [InlineData("Speed", 10)]
    [InlineData("RejectEvery", 1)]
    [InlineData("RejectEvery", 2.5)]
    [InlineData("Progress", 101)]
    [InlineData("BeltSpeed", double.NaN)]
    public async Task InvalidState_ExplicitlyStopsAndResetRecovers(string name, double value)
    {
        var g = new ConveyorGlobals(); g.Flag("RunRequested", true); g.Set(name, value); await Run(g);
        Stopped(g); Assert.False(g.Bool("RunRequested")); Assert.Equal("Invalid inputs - press Reset", g.Values["AI_Task.ConveyorLine.Status"]);
        g.Flag("ResetRequested", true); await Run(g); Invariants(g); Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.ConveyorLine.Status"]);
        g.Flag("RunRequested", true); await Run(g); Assert.True(g.Bool("MotorOn")); Assert.Equal(1, g.Get("CreatedTotal"));
    }

    private static Dictionary<string, double> Snapshot(ConveyorGlobals g) => new[] { "Slot0", "Slot1", "Slot2", "Slot3", "CreatedTotal", "GoodCount", "RejectCount", "ArrivalClock", "Elapsed", "Progress" }.ToDictionary(n => n, g.Get);
    private static void AssertSnapshot(ConveyorGlobals g, Dictionary<string, double> state) { foreach (var (name, value) in state) Assert.Equal(value, g.Get(name)); }
    private static void Invariants(ConveyorGlobals g)
    {
        var active = 0;
        for (var i = 0; i < 4; i++)
        {
            var s = g.Get("Slot" + i); Assert.True(double.IsFinite(s));
            Assert.True(s == -1 || s >= 0 && s < 100 || s >= 200 && s < 300);
            if (s >= 0) active++;
        }
        Assert.Equal(g.Get("CreatedTotal"), g.Get("GoodCount") + g.Get("RejectCount") + active);
        Assert.InRange(g.Get("CreatedTotal"), 0, 200); Assert.InRange(g.Get("Progress"), 0, 100); Assert.InRange(g.Get("ArrivalClock"), 0, 14);
    }
    private static void Stopped(ConveyorGlobals g)
    {
        Assert.All(new[] { "MotorOn", "ArrivalOn", "SensorOn", "DiverterOn" }, n => Assert.False(g.Bool(n))); Assert.Equal(0, g.Get("BeltSpeed"));
    }
    public class ConveyorGlobals
    {
        public Dictionary<string, object> Values { get; } = Bundle.Folder.Folders.Single().Variables.ToDictionary(v => "AI_Task.ConveyorLine." + v.Name,
            v => v.Type == "Boolean" ? (object)bool.Parse(v.InitialValue) : v.Type == "String" ? v.InitialValue : double.Parse(v.InitialValue, CultureInfo.InvariantCulture));
        public double ReadDouble(string path) => Convert.ToDouble(Values[path], CultureInfo.InvariantCulture);
        public bool ReadBool(string path) => Convert.ToBoolean(Values[path], CultureInfo.InvariantCulture);
        public void Write(string path, object value) { Assert.True(Values.ContainsKey(path), "Undeclared write: " + path); Values[path] = value; }
        public void Set(string name, double value) => Write("AI_Task.ConveyorLine." + name, value);
        public void Flag(string name, bool value) => Write("AI_Task.ConveyorLine." + name, value);
        public double Get(string name) => ReadDouble("AI_Task.ConveyorLine." + name);
        public bool Bool(string name) => ReadBool("AI_Task.ConveyorLine." + name);
    }
}
