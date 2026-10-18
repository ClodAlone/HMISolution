using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class SolarFarmTests
{
    private static readonly NodeModel Bundle = AiResourceBundle.Parse(SolarFarmTemplate.Json, "AI_Task");
    private static readonly ScriptRunner<object> Run = CSharpScript.Create(SolarFarmTemplate.SimulationCode.Replace("__TASK__", "AI_Task", StringComparison.Ordinal),
        ScriptOptions.Default.AddImports("System"), typeof(SolarGlobals)).CreateDelegate();

    [Fact]
    public void Bundle_ImportsTwiceWithoutExternalDependenciesOrChangesToExistingSettings()
    {
        var editor = new NodeEditorService();
        editor.NewFile();
        var project = editor.ActiveProject!;
        var settings = project.Model.Server;
        var startup = settings.StartupScreen;
        for (var i = 0; i < 2; i++)
        {
            var result = editor.ImportAiResources(project, SolarFarmTemplate.Json);
            Assert.True(result.success, result.message);
        }
        Assert.Same(settings, project.Model.Server);
        Assert.Equal(startup, settings.StartupScreen);
        editor.RefreshJsonFromTree();
        var model = JsonSerializer.Deserialize<NodeModel>(editor.JsonEditorText)!;
        Assert.Equal(2, model.Screens.Count);
        Assert.Equal(2, model.Scripts.Count);
        Assert.Equal(2, model.Folder.Folders.Count);
        var ids = new HashSet<string>();
        for (var i = 0; i < 2; i++)
        {
            var prefix = i == 0 ? "AI_Task" : "AI_Task_2";
            var screen = model.Screens[i];
            Assert.Equal(prefix + "_Solar Farm", screen.Name);
            Assert.Equal(prefix, screen.Group);
            Assert.Equal(1440, screen.Width);
            Assert.Equal(1040, screen.Height);
            Assert.Equal("", screen.SwipeLeftScreen);
            Assert.Equal("", screen.SwipeRightScreen);
            var script = model.Scripts[i];
            Assert.False(script.Enabled);
            Assert.Equal(250, script.IntervalMs);
            Assert.Contains($"const string p = \"{prefix}.Solar.\";", script.Code);
            var folder = Assert.Single(model.Folder.Folders[i].Folders);
            Assert.Equal("Solar", folder.Name);
            Assert.Equal(18, folder.Variables.Count);
            Assert.All(folder.Variables, v => Assert.Equal("ReadWrite", v.Access));
            Assert.All(screen.Symbols.Where(s => !string.IsNullOrEmpty(s.VariablePath)), s => Assert.StartsWith(prefix + ".Solar.", s.VariablePath));
            Assert.Equal(6, screen.Symbols.Count(s => s.Type == "button"));
            var artwork = XElement.Parse(Assert.Single(screen.Symbols, s => s.Type == "svg").SvgContent!);
            foreach (var id in artwork.Descendants().Attributes("id")) Assert.True(ids.Add(id.Value), "Duplicate SVG paint resource: " + id.Value);
            foreach (var href in artwork.Descendants().Attributes("href")) Assert.Contains(href.Value[1..], ids);
        }
        var errors = new ProjectValidationService(new SyntaxCheckService(), new CrossReferenceService()).Validate(model).Where(i => i.Severity == IssueSeverity.Error);
        Assert.Empty(errors);
    }

    [Fact]
    public void Pipes_UseDistinctColorsDirectionsAndMutuallyExclusiveSignedFlowBindings()
    {
        var screen = Assert.Single(Bundle.Screens);
        ScreenSymbol Symbol(string id) => Assert.Single(screen.Symbols, s => s.Id == "solar_" + id);
        var import = Symbol("grid_import");
        var export = Symbol("grid_export");
        Assert.Equal("#22d3ee", import.PipeFlowColor);
        Assert.Equal("right-left", import.PipeFlowDirection);
        Assert.Equal("value > 0", import.VisibilityBinding);
        Assert.Equal("#fb923c", export.PipeFlowColor);
        Assert.Equal("left-right", export.PipeFlowDirection);
        Assert.Equal("value < 0", export.VisibilityBinding);
        Assert.Equal("AI_Task.Solar.GridKw", import.VariablePath);
        Assert.Equal(import.VariablePath, export.VariablePath);
        Assert.Equal((import.X, import.Y, import.Width, import.Height), (export.X, export.Y, export.Width, export.Height));
        var charge = Symbol("battery_charge");
        var discharge = Symbol("battery_discharge");
        Assert.Equal("#a78bfa", charge.PipeFlowColor);
        Assert.Equal("right-left", charge.PipeFlowDirection);
        Assert.Equal("value > 0", charge.VisibilityBinding);
        Assert.Equal("#34d399", discharge.PipeFlowColor);
        Assert.Equal("left-right", discharge.PipeFlowDirection);
        Assert.Equal("value < 0", discharge.VisibilityBinding);
        Assert.Equal("AI_Task.Solar.BatteryKw", charge.VariablePath);
        Assert.Equal(charge.VariablePath, discharge.VariablePath);
        Assert.False(Symbol("grid_idle").PipeShowFlow);
        Assert.False(Symbol("battery_idle").PipeShowFlow);
        Assert.All(screen.Symbols.Where(s => s.Type == "pipe"), p =>
        {
            Assert.True(p.PipeUseGradient);
            Assert.True(p.PipeAnimate);
            Assert.True(screen.Symbols.IndexOf(p) < screen.Symbols.IndexOf(Symbol("artwork")));
        });
        Assert.Equal("#fbbf24", Symbol("pv_ac").PipeFlowColor);
        Assert.Equal("#60a5fa", Symbol("house").PipeFlowColor);
    }

    [Theory]
    [InlineData("run", "RunRequested", "ToggleVariable", "", "Boolean")]
    [InlineData("reset", "ResetRequested", "WriteVariable", "true", "Boolean")]
    [InlineData("auto", "Scenario", "WriteVariable", "0", "Int32")]
    [InlineData("sunny", "Scenario", "WriteVariable", "1", "Int32")]
    [InlineData("cloudy", "Scenario", "WriteVariable", "2", "Int32")]
    [InlineData("evening", "Scenario", "WriteVariable", "3", "Int32")]
    public void Controls_TargetTheDeclaredSimulationInputs(string id, string variable, string action, string value, string type)
    {
        var button = Assert.Single(Assert.Single(Bundle.Screens).Symbols, s => s.Id == "solar_" + id);
        Assert.Equal("button", button.Type);
        var command = Assert.Single(button.Commands);
        Assert.Equal("Click", command.Trigger);
        Assert.Equal(action, command.Action);
        Assert.Equal(value, command.Value);
        Assert.Equal("AI_Task.Solar." + variable, command.VariablePath);
        var input = Assert.Single(Assert.Single(Bundle.Folder.Folders).Variables, v => v.Name == variable);
        Assert.Equal(type, input.Type);
        Assert.Equal("ReadWrite", input.Access);
    }

    [Theory]
    [InlineData(1, 55, -1, 1)]
    [InlineData(2, 55, 1, -1)]
    [InlineData(3, 55, 1, -1)]
    [InlineData(1, 95, -1, 0)]
    [InlineData(3, 10, 1, 0)]
    public async Task Scenarios_RespectDispatchPriorityPowerBalanceAndBatteryLimits(int scenario, double soc, int gridSign, int batterySign)
    {
        var globals = new SolarGlobals();
        globals.Write("AI_Task.Solar.RunRequested", true);
        globals.Set("Scenario", scenario);
        globals.Set("BatterySoc", soc);
        await Run(globals);
        Balanced(globals);
        Assert.Equal(gridSign, Math.Sign(globals.Get("GridKw")));
        Assert.Equal(batterySign, Math.Sign(globals.Get("BatteryKw")));
        if (scenario == 1)
        {
            Assert.Equal(24 * 0.98, globals.Get("PvDcKw"), 10);
            Assert.Equal(24 * 0.98 * 0.96, globals.Get("PvAcKw"), 10);
            Assert.Equal(5, globals.Get("LoadKw"));
            Assert.Equal("EXPORT", globals.Values["AI_Task.Solar.GridMode"]);
        }
        if (scenario == 3)
        {
            Assert.Equal(0, globals.Get("PvDcKw"));
            Assert.Equal(0, globals.Get("SelfConsumptionPct"));
            Assert.Equal(8.5, globals.Get("LoadKw"));
        }
        var battery = globals.Get("BatteryKw");
        var expectedSoc = soc + (battery >= 0 ? battery * .96 : battery / .96) * .05 / 80 * 100;
        Assert.Equal(expectedSoc, globals.Get("BatterySoc"), 10);
    }

    [Theory]
    [InlineData(1, 94.999)]
    [InlineData(3, 10.001)]
    public async Task BatteryBoundary_ClipsPowerToRemainingEnergyInsteadOfOvershootingSoc(int scenario, double soc)
    {
        var globals = new SolarGlobals();
        globals.Set("Scenario", scenario);
        globals.Set("BatterySoc", soc);
        globals.Write("AI_Task.Solar.RunRequested", true);
        await Run(globals);
        Assert.Equal(scenario == 1 ? 95 : 10, globals.Get("BatterySoc"), 10);
        Assert.InRange(Math.Abs(globals.Get("BatteryKw")), 0.00001, .1);
        Balanced(globals);
        await Run(globals);
        Assert.Equal(0, globals.Get("BatteryKw"));
        Balanced(globals);
    }

    [Fact]
    public async Task CompleteAutomaticDay_ExercisesImportExportChargeDischargeAndWrapsClock()
    {
        var globals = new SolarGlobals();
        globals.Set("Hour", 0);
        globals.Write("AI_Task.Solar.RunRequested", true);
        var import = false; var export = false; var charge = false; var discharge = false;
        for (var cycle = 0; cycle < 481; cycle++)
        {
            await Run(globals);
            Balanced(globals);
            import |= globals.Get("GridKw") > .01;
            export |= globals.Get("GridKw") < -.01;
            charge |= globals.Get("BatteryKw") > .01;
            discharge |= globals.Get("BatteryKw") < -.01;
        }
        Assert.True(import && export && charge && discharge);
        Assert.InRange(globals.Get("Hour"), .049999, .050001);
    }

    [Fact]
    public async Task PauseAndReset_StopAllFlowsAndPreserveOrRestoreBatteryAndClock()
    {
        var globals = new SolarGlobals();
        globals.Set("Scenario", 1);
        globals.Write("AI_Task.Solar.RunRequested", true);
        await Run(globals);
        var soc = globals.Get("BatterySoc");
        var hour = globals.Get("Hour");
        globals.Write("AI_Task.Solar.RunRequested", false);
        await Run(globals);
        Assert.Equal(soc, globals.Get("BatterySoc"));
        Assert.Equal(hour, globals.Get("Hour"));
        Stopped(globals);
        globals.Write("AI_Task.Solar.ResetRequested", true);
        await Run(globals);
        Assert.Equal(55, globals.Get("BatterySoc"));
        Assert.Equal(9, globals.Get("Hour"));
        Assert.Equal(0, globals.Get("Scenario"));
        Assert.False(Convert.ToBoolean(globals.Values["AI_Task.Solar.ResetRequested"]));
        Stopped(globals);
    }

    [Theory]
    [InlineData("BatterySoc", double.NaN)]
    [InlineData("BatterySoc", 9)]
    [InlineData("BatterySoc", 96)]
    [InlineData("Hour", 24)]
    [InlineData("Hour", double.PositiveInfinity)]
    [InlineData("Scenario", 1.5)]
    [InlineData("Scenario", 4)]
    public async Task InvalidInputs_StopExplicitlyAndResetRecovers(string field, double value)
    {
        var globals = new SolarGlobals();
        globals.Write("AI_Task.Solar.RunRequested", true);
        globals.Set(field, value);
        await Run(globals);
        Assert.Equal("Invalid inputs - press Reset", globals.Values["AI_Task.Solar.Status"]);
        Assert.False(Convert.ToBoolean(globals.Values["AI_Task.Solar.RunRequested"]));
        Stopped(globals);
        globals.Write("AI_Task.Solar.ResetRequested", true);
        await Run(globals);
        Assert.Equal(55, globals.Get("BatterySoc"));
        Assert.Equal(9, globals.Get("Hour"));
        Assert.Equal("Reset - press Start / Pause", globals.Values["AI_Task.Solar.Status"]);
    }

    [Fact]
    public void CommonTask_LoadsTheExactSolarBundleAndUsesSolarSpecificFeedback()
    {
        var task = AiTaskCatalog.Tasks.Single(t => t.Id == "screen-solar-farm");
        Assert.Equal(SolarFarmTemplate.Json, task.ResourceBundle);
        Assert.Contains(SolarFarmTemplate.Json, task.Prompt);
        var editor = new NodeEditorService(); editor.NewFile();
        var component = new JsonEditor();
        typeof(JsonEditor).GetProperty(nameof(JsonEditor.ResourceProject))!.SetValue(component, editor.ActiveProject);
        typeof(JsonEditor).GetMethod("OnTaskChanged", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, [new ChangeEventArgs { Value = task.Id }]);
        typeof(JsonEditor).GetMethod("LoadTaskTemplate", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null);
        Assert.Equal(SolarFarmTemplate.Json, component.Session.ResourceJson);
        Assert.Same(editor.ActiveProject, component.Session.ResourceTarget);
        Assert.True(component.Session.ResourceOnly);
        Assert.Contains(task.Title, component.Session.AiComments);
        Assert.DoesNotContain("TankTransfer", component.Session.AiComments);
        Assert.Empty(editor.ActiveProject!.Model.Screens);
    }

    private static void Balanced(SolarGlobals globals)
    {
        Assert.Equal(globals.Get("LoadKw") + globals.Get("BatteryKw"), globals.Get("PvAcKw") + globals.Get("GridKw"), 8);
        Assert.Equal(globals.Get("PvAcKw"), globals.Get("Inverter1Kw") + globals.Get("Inverter2Kw"), 8);
        Assert.InRange(globals.Get("Inverter1Kw"), 0, 12);
        Assert.InRange(globals.Get("Inverter2Kw"), 0, 12);
        Assert.InRange(globals.Get("BatterySoc"), 10, 95);
        Assert.InRange(globals.Get("BatteryKw"), -8, 8);
        Assert.Equal(Math.Min(globals.Get("PvAcKw"), globals.Get("LoadKw")), globals.Get("SelfConsumedKw"), 8);
        Assert.InRange(globals.Get("SelfConsumptionPct"), 0, 100);
    }

    private static void Stopped(SolarGlobals globals)
    {
        foreach (var name in new[] { "PvDcKw", "PvAcKw", "Inverter1Kw", "Inverter2Kw", "LoadKw", "BatteryKw", "GridKw", "SelfConsumedKw" })
            Assert.Equal(0, globals.Get(name));
        Assert.Equal("STANDBY", globals.Values["AI_Task.Solar.BatteryMode"]);
        Assert.Equal("IDLE / BALANCED", globals.Values["AI_Task.Solar.GridMode"]);
    }

    public class SolarGlobals
    {
        public Dictionary<string, object> Values { get; } = Bundle.Folder.Folders.Single().Variables.ToDictionary(v => "AI_Task.Solar." + v.Name, v =>
            v.Type == "Boolean" ? (object)bool.Parse(v.InitialValue) : v.Type == "String" ? v.InitialValue : double.Parse(v.InitialValue, CultureInfo.InvariantCulture));
        public double ReadDouble(string path) => Convert.ToDouble(Values[path], CultureInfo.InvariantCulture);
        public bool ReadBool(string path) => Convert.ToBoolean(Values[path], CultureInfo.InvariantCulture);
        public void Write(string path, object value)
        {
            Assert.True(Values.ContainsKey(path), "Write to undeclared variable: " + path);
            Values[path] = value;
        }
        public void Set(string name, double value) => Write("AI_Task.Solar." + name, value);
        public double Get(string name) => ReadDouble("AI_Task.Solar." + name);
    }
}
