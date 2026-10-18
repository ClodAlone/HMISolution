using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class WaterTreatmentTests
{
    private static readonly NodeModel Bundle = AiResourceBundle.Parse(WaterTreatmentTemplate.Json, "AI_Task");
    private static readonly ScriptRunner<object> Run = CSharpScript.Create(WaterTreatmentTemplate.SimulationCode.Replace("__TASK__", "AI_Task", StringComparison.Ordinal),
        ScriptOptions.Default.AddImports("System"), typeof(WaterGlobals)).CreateDelegate();

    [Fact]
    public void ImportTwice_IsSelfContainedNamespacedAndPreservesSettings()
    {
        var editor = new NodeEditorService(); editor.NewFile();
        var project = editor.ActiveProject!; var settings = project.Model.Server; var startup = settings.StartupScreen;
        for (var i = 0; i < 2; i++) { var result = editor.ImportAiResources(project, WaterTreatmentTemplate.Json); Assert.True(result.success, result.message); }
        editor.RefreshJsonFromTree();
        var model = JsonSerializer.Deserialize<NodeModel>(editor.JsonEditorText)!;
        Assert.Same(settings, project.Model.Server); Assert.Equal(startup, settings.StartupScreen);
        Assert.Equal(2, model.Screens.Count); Assert.Equal(2, model.Scripts.Count); Assert.Equal(2, model.Folder.Folders.Count);
        var ids = new HashSet<string>();
        for (var i = 0; i < 2; i++)
        {
            var prefix = i == 0 ? "AI_Task" : "AI_Task_2";
            var screen = model.Screens[i];
            Assert.Equal(prefix + "_Water Treatment Plant", screen.Name); Assert.Equal(prefix, screen.Group);
            Assert.Equal((1440d, 1040d), (screen.Width, screen.Height));
            Assert.Equal(screen.Symbols.Count, screen.Symbols.Select(s => s.Id).Distinct().Count());
            var folder = Assert.Single(model.Folder.Folders[i].Folders); Assert.Equal("WaterTreatment", folder.Name);
            Assert.All(folder.Variables, v => Assert.Equal("ReadWrite", v.Access));
            var script = model.Scripts[i]; Assert.Equal(prefix + "_WaterTreatmentSimulation", script.Name);
            Assert.False(script.Enabled); Assert.Equal(250, script.IntervalMs);
            Assert.Contains($"const string p = \"{prefix}.WaterTreatment.\";", script.Code);
            Assert.All(screen.Symbols.Where(s => !string.IsNullOrEmpty(s.VariablePath)), s => Assert.StartsWith(prefix + ".WaterTreatment.", s.VariablePath));
            foreach (var symbol in screen.Symbols.Where(s => s.Type == "svg"))
            {
                Assert.DoesNotContain("__TASK__", symbol.SvgContent!);
                foreach (var id in XElement.Parse(symbol.SvgContent!).Descendants().Attributes("id"))
                {
                    Assert.StartsWith(prefix + "_wt_", id.Value); Assert.True(ids.Add(id.Value));
                }
            }
        }
        Assert.Empty(new ProjectValidationService(new SyntaxCheckService(), new CrossReferenceService()).Validate(model).Where(i => i.Severity == IssueSeverity.Error));
    }

    [Fact]
    public void Equipment_UsesRealWidgetsAndPipesBehindOpaqueArt()
    {
        var screen = Bundle.Screens.Single();
        ScreenSymbol Symbol(string id) => screen.Symbols.Single(s => s.Id == "watertreatment_" + id);
        Assert.Equal("watertreatment_header", screen.Symbols[0].Id);
        Assert.All(screen.Symbols.Where(s => s.Type == "pipe"), s => { Assert.True(s.PipeUseGradient); Assert.True(screen.Symbols.IndexOf(s) < screen.Symbols.IndexOf(Symbol("artwork"))); });
        Assert.Equal("#d6a75a", Symbol("raw").PipeFlowColor); Assert.Equal("#22d3ee", Symbol("treated").PipeFlowColor);
        Assert.Equal("#c084fc", Symbol("wash_return").PipeFlowColor); Assert.Equal("right-left", Symbol("wash_return").PipeFlowDirection);
        Assert.Equal("AI_Task.WaterTreatment.BackwashFlow", Symbol("waste").VariablePath);
        Assert.Equal("pipe", Symbol("waste").Type);
        Assert.Equal("AI_Task.WaterTreatment.WasteTotal", Symbol("waste_total").VariablePath);
        Assert.Equal("numericdisplay", Symbol("waste_total").Type);
        Assert.Equal("tank", Symbol("raw_tank").Type); Assert.Equal(1000, Symbol("raw_tank").MaxValue);
        Assert.Equal("tank", Symbol("clean_tank").Type); Assert.Equal(1200, Symbol("clean_tank").MaxValue);
        Assert.Equal("isometric", Symbol("pump").MotorAppearance); Assert.Equal("AI_Task.WaterTreatment.PumpFault", Symbol("pump").MotorFaultPath);
        Assert.Equal((1d, 5d, 1d), (Symbol("dirt").MinValue, Symbol("dirt").MaxValue, Symbol("dirt").EditBoxStep));
        Assert.Equal((0d, 8d), (Symbol("demand").MinValue, Symbol("demand").MaxValue));
    }

    [Theory]
    [InlineData("run", "RunRequested", "ToggleVariable", "")]
    [InlineData("reset", "ResetRequested", "WriteVariable", "true")]
    [InlineData("wash", "BackwashRequested", "WriteVariable", "true")]
    [InlineData("auto", "AutoBackwash", "ToggleVariable", "")]
    [InlineData("fault", "PumpFault", "ToggleVariable", "")]
    public void Controls_TargetDeclaredInputs(string id, string variable, string action, string value)
    {
        var button = Bundle.Screens.Single().Symbols.Single(s => s.Id == "watertreatment_" + id);
        var command = Assert.Single(button.Commands);
        Assert.Equal("Click", command.Trigger); Assert.Equal(action, command.Action); Assert.Equal(value, command.Value);
        Assert.Equal("AI_Task.WaterTreatment." + variable, command.VariablePath);
        Assert.Contains(Bundle.Folder.Folders.Single().Variables, v => v.Name == variable && v.Type == "Boolean");
    }

    [Fact]
    public async Task Treatment_ConservesEverySourceAndSinkAndIncreasesFilterPressure()
    {
        var g = new WaterGlobals(); g.Flag("RunRequested", true);
        await Run(g);
        Assert.Equal(608 - 6, g.Get("RawLevel"));
        Assert.Equal(300 + 6 * .97 - 5, g.Get("ClarifierLevel"), 9);
        Assert.Equal(603, g.Get("CleanLevel")); Assert.Equal(.18, g.Get("WasteTotal"), 9);
        Assert.Equal(8, g.Get("SourceTotal")); Assert.Equal(2, g.Get("DeliveryTotal"));
        Assert.Equal(.6, g.Get("Clogging"), 9); Assert.Equal(.215, g.Get("DifferentialPressure"), 9);
        Assert.True(g.Bool("PumpOn")); Assert.True(g.Bool("FilterOn")); Balanced(g);
        var dirty = new WaterGlobals(); dirty.Set("DirtyInfluent", 5); dirty.Flag("RunRequested", true); await Run(dirty);
        Assert.Equal(g.Get("Clogging") * 5, dirty.Get("Clogging"), 9);
    }

    [Fact]
    public async Task ManualBackwash_ConsumesCleanStorageAppearsInWasteAndRecoversFilter()
    {
        var g = new WaterGlobals(); g.Set("Clogging", 80); g.Set("Demand", 0);
        g.Flag("AutoBackwash", false); g.Flag("RunRequested", true); g.Flag("BackwashRequested", true);
        for (var i = 0; i < 12; i++)
        {
            await Run(g); Assert.Equal(10, g.Get("BackwashFlow")); Assert.Equal(0, g.Get("FilteredFlow"));
            Assert.True(g.Bool("BackwashOn")); Assert.False(g.Bool("FilterOn")); Balanced(g);
        }
        Assert.Equal(480, g.Get("CleanLevel")); Assert.Equal(120, g.Get("WasteTotal"));
        Assert.Equal(2, g.Get("Clogging"), 9); Assert.Equal(0, g.Get("BackwashRemaining")); Assert.False(g.Bool("BackwashRequested"));
        await Run(g); Assert.Equal(0, g.Get("BackwashFlow")); Assert.True(g.Get("FilteredFlow") > 0); Balanced(g);
    }

    [Fact]
    public async Task LongRun_AutomaticallyBackwashesAndMaintainsBoundsAndConservation()
    {
        var g = new WaterGlobals(); g.Flag("RunRequested", true); g.Set("DirtyInfluent", 5);
        var wash = false; var treat = false;
        for (var i = 0; i < 1800; i++)
        {
            await Run(g); Balanced(g); Assert.True(g.Bool("RunRequested"), g.Values["AI_Task.WaterTreatment.Status"].ToString());
            wash |= g.Bool("BackwashOn"); treat |= g.Bool("FilterOn");
        }
        Assert.True(wash && treat); Assert.True(g.Get("WasteTotal") > 120); Assert.Equal(1800, g.Get("Elapsed"));
    }

    [Fact]
    public async Task DryTankAndPumpFault_InterlockWithoutCreatingWater()
    {
        var g = new WaterGlobals(); g.Set("CleanLevel", 0); g.Set("DeliveryTotal", 600);
        g.Flag("RunRequested", true); g.Flag("BackwashRequested", true); await Run(g);
        Assert.Equal(0, g.Get("BackwashFlow")); Assert.Equal(0, g.Get("DeliveryFlow")); Assert.False(g.Bool("BackwashOn"));
        Assert.Contains("Dry clean tank", g.Values["AI_Task.WaterTreatment.Status"].ToString()); Balanced(g);
        g.Flag("PumpFault", true); var before = g.Get("RawLevel"); await Run(g);
        Stopped(g); Assert.Equal(before, g.Get("RawLevel")); Assert.Contains("PUMP FAULT", g.Values["AI_Task.WaterTreatment.Status"].ToString()); Balanced(g);
        g.Flag("PumpFault", false); await Run(g); Assert.True(g.Bool("FilterOn")); Balanced(g);
    }

    [Fact]
    public async Task PauseFreezesBackwashProgressAndInventories_ResetRestoresEveryNumericAndPauses()
    {
        var g = new WaterGlobals(); g.Flag("RunRequested", true); g.Flag("BackwashRequested", true); await Run(g);
        g.Flag("RunRequested", false);
        var names = new[] { "RawLevel", "ClarifierLevel", "CleanLevel", "Clogging", "BackwashRemaining", "Progress", "Elapsed", "SourceTotal", "DeliveryTotal", "WasteTotal" };
        var state = names.ToDictionary(n => n, g.Get);
        await Run(g); Stopped(g); foreach (var (name, value) in state) Assert.Equal(value, g.Get(name));
        g.Set("DirtyInfluent", 5); g.Set("Demand", 8); g.Flag("PumpFault", true); g.Flag("AutoBackwash", false);
        g.Flag("ResetRequested", true); await Run(g); Stopped(g);
        var initial = new WaterGlobals();
        foreach (var v in Bundle.Folder.Folders.Single().Variables.Where(v => v.Type == "Double")) Assert.Equal(initial.Get(v.Name), g.Get(v.Name));
        Assert.True(g.Bool("AutoBackwash")); Assert.False(g.Bool("RunRequested")); Assert.False(g.Bool("ResetRequested")); Assert.False(g.Bool("PumpFault"));
        Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.WaterTreatment.Status"]); Balanced(g);
    }

    [Theory]
    [InlineData("RawLevel", double.NaN)]
    [InlineData("CleanLevel", double.PositiveInfinity)]
    [InlineData("ClarifierLevel", 801)]
    [InlineData("Clogging", -1)]
    [InlineData("DirtyInfluent", 0)]
    [InlineData("DirtyInfluent", 6)]
    [InlineData("Demand", 9)]
    [InlineData("BackwashRemaining", 1.5)]
    [InlineData("SourceTotal", -1)]
    [InlineData("Progress", 101)]
    [InlineData("FilteredFlow", double.NaN)]
    [InlineData("BackwashFlow", 11)]
    public async Task InvalidNumericState_ExplicitlyStopsAndResetRecovers(string name, double value)
    {
        var g = new WaterGlobals(); g.Set(name, value); g.Flag("RunRequested", true); await Run(g);
        Stopped(g); Assert.False(g.Bool("RunRequested")); Assert.Equal("Invalid inputs - press Reset", g.Values["AI_Task.WaterTreatment.Status"]);
        g.Flag("ResetRequested", true); await Run(g); Balanced(g);
        Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.WaterTreatment.Status"]);
        g.Flag("RunRequested", true); await Run(g); Assert.True(g.Bool("PumpOn"));
    }

    private static void Balanced(WaterGlobals g)
    {
        Assert.Equal(1500 + g.Get("SourceTotal"), g.Get("RawLevel") + g.Get("ClarifierLevel") + g.Get("CleanLevel") + g.Get("DeliveryTotal") + g.Get("WasteTotal"), 6);
        Assert.InRange(g.Get("RawLevel"), 0, 1000); Assert.InRange(g.Get("ClarifierLevel"), 0, 800); Assert.InRange(g.Get("CleanLevel"), 0, 1200);
        Assert.InRange(g.Get("Clogging"), 0, 100); Assert.InRange(g.Get("Progress"), 0, 100);
        Assert.All(new[] { "IntakeFlow", "FeedFlow", "FilteredFlow", "BackwashFlow", "DeliveryFlow", "SludgeFlow" }, n => Assert.InRange(g.Get(n), 0, 10));
    }
    private static void Stopped(WaterGlobals g)
    {
        foreach (var name in new[] { "IntakeFlow", "FeedFlow", "FilteredFlow", "BackwashFlow", "DeliveryFlow", "SludgeFlow" }) Assert.Equal(0, g.Get(name));
        foreach (var name in new[] { "PumpOn", "FilterOn", "BackwashOn" }) Assert.False(g.Bool(name));
    }
    public class WaterGlobals
    {
        public Dictionary<string, object> Values { get; } = Bundle.Folder.Folders.Single().Variables.ToDictionary(v => "AI_Task.WaterTreatment." + v.Name,
            v => v.Type == "Boolean" ? (object)bool.Parse(v.InitialValue) : v.Type == "String" ? v.InitialValue : double.Parse(v.InitialValue, CultureInfo.InvariantCulture));
        public double ReadDouble(string path) => Convert.ToDouble(Values[path], CultureInfo.InvariantCulture);
        public bool ReadBool(string path) => Convert.ToBoolean(Values[path], CultureInfo.InvariantCulture);
        public void Write(string path, object value) { Assert.True(Values.ContainsKey(path), "Undeclared write: " + path); Values[path] = value; }
        public void Set(string name, double value) => Write("AI_Task.WaterTreatment." + name, value);
        public void Flag(string name, bool value) => Write("AI_Task.WaterTreatment." + name, value);
        public double Get(string name) => ReadDouble("AI_Task.WaterTreatment." + name);
        public bool Bool(string name) => ReadBool("AI_Task.WaterTreatment." + name);
    }
}
