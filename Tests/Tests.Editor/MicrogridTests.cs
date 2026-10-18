using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class MicrogridTests
{
    private static readonly NodeModel Bundle = AiResourceBundle.Parse(MicrogridTemplate.Json, "AI_Task");
    private static readonly ScriptRunner<object> Run = CSharpScript.Create(
        MicrogridTemplate.SimulationCode.Replace("__TASK__", "AI_Task", StringComparison.Ordinal),
        ScriptOptions.Default.AddImports("System"), typeof(MicrogridGlobals)).CreateDelegate();

    [Fact]
    public void Bundle_ImportsTwiceWithNamespacedSvgPipesControlsAndValidCrossReferences()
    {
        var editor = new NodeEditorService();
        editor.NewFile();
        var project = editor.ActiveProject!;
        var settings = project.Model.Server;
        var startup = settings.StartupScreen;
        for (var i = 0; i < 2; i++)
        {
            var result = editor.ImportAiResources(project, MicrogridTemplate.Json);
            Assert.True(result.success, result.message);
        }
        Assert.Same(settings, project.Model.Server);
        Assert.Equal(startup, settings.StartupScreen);
        editor.RefreshJsonFromTree();
        var model = JsonSerializer.Deserialize<NodeModel>(editor.JsonEditorText)!;
        Assert.Equal(2, model.Screens.Count);
        Assert.Equal(2, model.Scripts.Count);
        Assert.Equal(2, model.Folder.Folders.Count);
        var resourceIds = new HashSet<string>(StringComparer.Ordinal);
        for (var i = 0; i < 2; i++)
        {
            var prefix = i == 0 ? "AI_Task" : "AI_Task_2";
            var screen = model.Screens[i];
            var script = model.Scripts[i];
            var folder = Assert.Single(model.Folder.Folders[i].Folders);
            Assert.Equal(prefix + "_Microgrid Control Center", screen.Name);
            Assert.Equal(prefix, screen.Group);
            Assert.Equal((1440d, 1040d), (screen.Width, screen.Height));
            Assert.Equal(prefix + "_MicrogridSimulation", script.Name);
            Assert.False(script.Enabled);
            Assert.Equal(250, script.IntervalMs);
            Assert.Contains($"const string p = \"{prefix}.Microgrid.\";", script.Code);
            Assert.DoesNotContain("__TASK__", script.Code);
            Assert.Equal("Microgrid", folder.Name);
            Assert.Equal(25, folder.Variables.Count);
            Assert.All(folder.Variables, v => Assert.Equal("ReadWrite", v.Access));
            Assert.All(screen.Symbols.Where(s => !string.IsNullOrEmpty(s.VariablePath)), s => Assert.StartsWith(prefix + ".Microgrid.", s.VariablePath));
            var artworkIndex = screen.Symbols.FindIndex(s => s.Id.EndsWith("artwork", StringComparison.Ordinal));
            Assert.True(artworkIndex >= 0);
            Assert.All(screen.Symbols.Where(s => s.Type == "pipe"), s => Assert.True(screen.Symbols.IndexOf(s) < artworkIndex));
            foreach (var symbol in screen.Symbols.Where(s => s.Type == "svg"))
            {
                Assert.DoesNotContain("__TASK__", symbol.SvgContent!);
                var svg = XElement.Parse(symbol.SvgContent!);
                foreach (var id in svg.DescendantsAndSelf().Attributes("id"))
                {
                    Assert.StartsWith(prefix + "_mg_", id.Value);
                    Assert.True(resourceIds.Add(id.Value), "Duplicate microgrid paint resource: " + id.Value);
                }
                foreach (var href in svg.DescendantsAndSelf().Attributes("href")) Assert.Contains(href.Value.TrimStart('#'), resourceIds);
                foreach (var value in svg.DescendantsAndSelf().Attributes().Select(a => a.Value))
                    foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(value, "url\\(#([^)]*)\\)"))
                        Assert.Contains(match.Groups[1].Value, resourceIds);
                if (symbol.Id.EndsWith("artwork", StringComparison.Ordinal))
                    foreach (var id in svg.Descendants().Where(e => e.Name.LocalName == "defs").Elements().Select(e => (string)e.Attribute("id")!))
                        Assert.True(symbol.SvgContent!.Contains("#" + id + "\"", StringComparison.Ordinal) || symbol.SvgContent.Contains("#" + id + ")", StringComparison.Ordinal), "Unused artwork definition: " + id);
            }
        }
        Assert.Empty(new ProjectValidationService(new SyntaxCheckService(), new CrossReferenceService()).Validate(model).Where(i => i.Severity == IssueSeverity.Error));
    }

    [Theory]
    [InlineData("run", "RunRequested", "ToggleVariable", "")]
    [InlineData("reset", "ResetRequested", "WriteVariable", "true")]
    [InlineData("cheap", "Scenario", "WriteVariable", "0")]
    [InlineData("peak", "Scenario", "WriteVariable", "1")]
    [InlineData("island", "IslandMode", "ToggleVariable", "")]
    public void Controls_BindRunResetTariffScenariosAndIslandToggle(string id, string variable, string action, string value)
    {
        var button = Assert.Single(Bundle.Screens.Single().Symbols, s => s.Id == "microgrid_" + id);
        var command = Assert.Single(button.Commands);
        Assert.Equal("Click", command.Trigger);
        Assert.Equal(action, command.Action);
        Assert.Equal(value, command.Value);
        Assert.Equal("AI_Task.Microgrid." + variable, command.VariablePath);
        Assert.Contains(Assert.Single(Bundle.Folder.Folders).Variables, v => v.Name == variable);
    }

    [Fact]
    public void FlowWidgets_UseSignedMutuallyExclusiveModesAndBoundedEditableImportCap()
    {
        var screen = Bundle.Screens.Single();
        ScreenSymbol Symbol(string id) => Assert.Single(screen.Symbols, s => s.Id == "microgrid_" + id);
        var import = Symbol("grid_import");
        var export = Symbol("grid_export");
        Assert.Equal("AI_Task.Microgrid.GridKw", import.VariablePath);
        Assert.Equal(import.VariablePath, export.VariablePath);
        Assert.Equal("value > 0", import.VisibilityBinding);
        Assert.Equal("value < 0", export.VisibilityBinding);
        Assert.Equal("right-left", import.PipeFlowDirection);
        Assert.Equal("left-right", export.PipeFlowDirection);
        Assert.Equal("#22d3ee", import.PipeFlowColor);
        Assert.Equal("#fb923c", export.PipeFlowColor);
        var charge = Symbol("battery_charge");
        var discharge = Symbol("battery_discharge");
        Assert.Equal("AI_Task.Microgrid.BatteryKw", charge.VariablePath);
        Assert.Equal(charge.VariablePath, discharge.VariablePath);
        Assert.Equal("value > 0", charge.VisibilityBinding);
        Assert.Equal("value < 0", discharge.VisibilityBinding);
        Assert.Equal("#a78bfa", charge.PipeFlowColor);
        Assert.Equal("#34d399", discharge.PipeFlowColor);
        Assert.Equal("AI_Task.Microgrid.ImportCapKw", Symbol("cap").VariablePath);
        Assert.Equal((0.5, 30d, 0.5), (Symbol("cap").MinValue, Symbol("cap").MaxValue, Symbol("cap").EditBoxStep));
        Assert.All(screen.Symbols.Where(s => s.Type == "pipe"), pipe => Assert.True(pipe.PipeAnimate));
    }

    [Theory]
    [InlineData(0, 0.12, 1)]
    [InlineData(1, 0.48, -1)]
    public async Task TariffScenarios_ChargeAtCheapRateAndPeakShaveToImportCap(int scenario, double tariff, int batterySign)
    {
        var g = new MicrogridGlobals();
        g.Set("Scenario", scenario); g.Set("Hour", 0); g.Set("ImportCapKw", scenario == 0 ? 14 : 6);
        g.Write("AI_Task.Microgrid.RunRequested", true);
        await Run(g);
        Assert.Equal(tariff, g.Get("Tariff"));
        Assert.Equal(0, g.Get("PvKw"));
        Assert.Equal(batterySign, Math.Sign(g.Get("BatteryKw")));
        Assert.InRange(g.Get("GridKw"), 0, g.Get("ImportCapKw"));
        Assert.Equal(g.Get("GridKw") * 0.05, g.Get("ImportEnergyKwh"), 10);
        Assert.Equal(g.Get("ImportEnergyKwh") * tariff, g.Get("OperatingCost"), 10);
        if (scenario == 0)
        {
            Assert.Equal(4, g.Get("BatteryKw"), 8);
            Assert.Equal(55 + 4 * 0.95 * 0.05 / 40 * 100, g.Get("BatterySoc"), 9);
        }
        else
        {
            Assert.Equal(6, g.Get("GridKw"), 8);
            Assert.True(g.Get("BatterySoc") < 55);
        }
        Invariants(g);
    }

    [Fact]
    public async Task SunnyConnectedSite_ExportsSurplusWhenBatteryIsFull()
    {
        var g = new MicrogridGlobals();
        g.Set("Scenario", 0); g.Set("Hour", 12); g.Set("BatterySoc", 95);
        g.Write("AI_Task.Microgrid.RunRequested", true);
        await Run(g);
        Assert.Equal(0.12, g.Get("Tariff"));
        Assert.Equal(0, g.Get("BatteryKw"));
        Assert.True(g.Get("GridKw") < 0);
        Assert.Equal(-Math.Abs(g.Get("GridKw")) * g.Get("ExportTariff") * 0.05, g.Get("OperatingCost"), 10);
        Assert.Equal(0, g.Get("ImportEnergyKwh"));
        Invariants(g);
    }

    [Fact]
    public async Task PeakCapShortfall_IsTrackedAsUnservedDemandRatherThanHiddenGridImport()
    {
        var g = new MicrogridGlobals();
        g.Set("Scenario", 1); g.Set("Hour", 0); g.Set("ImportCapKw", 0.5); g.Set("BatterySoc", 15);
        g.Write("AI_Task.Microgrid.RunRequested", true);
        await Run(g);
        Assert.Equal(0.5, g.Get("GridKw"), 10);
        Assert.Equal(0, g.Get("BatteryKw"));
        Assert.True(g.Get("UnservedKw") > 0);
        Assert.InRange(g.Get("GridKw"), 0, g.Get("ImportCapKw"));
        Invariants(g);
    }

    [Fact]
    public async Task IslandMode_UsesOnlyLocalSourcesAndExplicitlyTracksCurtailmentAndUnservedLoad()
    {
        var sunny = new MicrogridGlobals();
        sunny.Set("Hour", 12); sunny.Write("AI_Task.Microgrid.IslandMode", true);
        sunny.Write("AI_Task.Microgrid.RunRequested", true);
        await Run(sunny);
        Assert.Equal(0, sunny.Get("GridKw"));
        Assert.True(sunny.Get("PvKw") > sunny.Get("HouseDemandKw") + sunny.Get("EvDemandKw"));
        Assert.True(sunny.Get("BatteryKw") > 0);
        Assert.True(sunny.Get("CurtailedPvKw") > 0);
        Assert.Equal(0, sunny.Get("UnservedKw"));
        Assert.Equal("ISLANDED", sunny.Values["AI_Task.Microgrid.GridMode"]);
        Assert.Equal("Island mode - PV surplus curtailed / no grid exchange", sunny.Values["AI_Task.Microgrid.Status"]);
        Invariants(sunny);

        var night = new MicrogridGlobals();
        night.Set("Hour", 0); night.Set("BatterySoc", 15);
        night.Write("AI_Task.Microgrid.IslandMode", true); night.Write("AI_Task.Microgrid.RunRequested", true);
        await Run(night);
        Assert.Equal(0, night.Get("GridKw"));
        Assert.True(night.Get("UnservedKw") > 0);
        Assert.Equal(0, night.Get("CurtailedPvKw"));
        Invariants(night);
    }

    [Fact]
    public async Task AutomaticDayCycle_StaysBoundedAndConservesEveryDispatchStep()
    {
        var g = new MicrogridGlobals();
        g.Set("Scenario", 1); g.Set("Hour", 0); g.Write("AI_Task.Microgrid.RunRequested", true);
        for (var i = 0; i < 481; i++)
        {
            await Run(g);
            Invariants(g);
            Assert.True(g.Flag("RunRequested"));
        }
        Assert.Equal(0.05, g.Get("Hour"), 8);
        Assert.True(g.Get("UnservedKw") >= 0);
        Assert.True(g.Get("ImportEnergyKwh") > 0);
        Assert.True(g.Get("OperatingCost") > 0);
    }

    [Fact]
    public async Task ReportedCycle43State_ValidatesExactlyAndKeepsRunning()
    {
        var g = new MicrogridGlobals();
        g.Write("AI_Task.Microgrid.RunRequested", true);
        for (var cycle = 1; cycle <= 42; cycle++)
        {
            await Run(g);
            Invariants(g);
        }
        Assert.Equal(14.10000000000003, g.Get("Hour"), 12);
        Assert.Equal(84.91523296985191, g.Get("BatterySoc"), 10);
        Assert.Equal(-0.1291119046963351, g.Get("OperatingCost"), 12);
        Assert.Equal(0, g.Get("ImportEnergyKwh"));
        Assert.True(g.Get("EvServedKw") <= g.Get("EvDemandKw"));
        Assert.True(g.Get("HouseServedKw") <= g.Get("HouseDemandKw"));

        // The previous proportional formula persisted 3.5 * d / d = 3.5000000000000004; a one-ULP overshoot must not invalidate the state.
        g.Set("EvServedKw", 3.5000000000000004);
        await Run(g);
        Assert.True(g.Flag("RunRequested"), g.Values["AI_Task.Microgrid.Status"].ToString());
        Assert.Equal(14.15000000000003, g.Get("Hour"), 12);
        Assert.Equal(3.5, g.Get("EvServedKw"));
        Invariants(g);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    public async Task DefaultStart_RunsMoreThanAFullDayWithoutInvalidState(int scenario, bool island)
    {
        var g = new MicrogridGlobals();
        g.Set("Scenario", scenario);
        g.Write("AI_Task.Microgrid.IslandMode", island);
        g.Write("AI_Task.Microgrid.RunRequested", true);
        for (var cycle = 1; cycle <= 600; cycle++)
        {
            await Run(g);
            Invariants(g);
            Assert.True(g.Flag("RunRequested"), "Stopped at cycle " + cycle + ": " + g.Values["AI_Task.Microgrid.Status"]);
        }
        Assert.Equal(18, g.Get("Hour"), 6);
    }

    [Fact]
    public async Task DefaultMiddayCycle_AllowsFiniteExportCreditsAndRemainsRunningThroughCycle100()
    {
        var g = new MicrogridGlobals();
        g.Write("AI_Task.Microgrid.RunRequested", true);
        var sawExportCredit = false;
        for (var cycle = 1; cycle <= 100; cycle++)
        {
            await Run(g);
            Invariants(g);
            Assert.True(g.Flag("RunRequested"), "Simulation stopped at cycle " + cycle + ": " + g.Values["AI_Task.Microgrid.Status"]);
            sawExportCredit |= g.Get("OperatingCost") < 0;
        }
        Assert.True(sawExportCredit, "Sunny export should make the cumulative operating cost negative before evening load returns.");
        Assert.Equal(17, g.Get("Hour"), 8);
        Assert.True(double.IsFinite(g.Get("OperatingCost")));
    }

    [Fact]
    public async Task PauseFreezesTimeStorageAndMetersAndResetRestoresControlsAndState()
    {
        var g = new MicrogridGlobals();
        g.Write("AI_Task.Microgrid.RunRequested", true);
        await Run(g);
        var frozen = new[] { "Hour", "BatterySoc", "ImportEnergyKwh", "OperatingCost" }.ToDictionary(name => name, g.Get);
        g.Write("AI_Task.Microgrid.RunRequested", false);
        await Run(g);
        foreach (var (name, value) in frozen) Assert.Equal(value, g.Get(name));
        Stopped(g);

        g.Set("Scenario", 1); g.Set("ImportCapKw", 27); g.Set("Hour", 23); g.Set("BatterySoc", 90);
        g.Set("ImportEnergyKwh", 100); g.Set("OperatingCost", 25);
        g.Write("AI_Task.Microgrid.IslandMode", true); g.Write("AI_Task.Microgrid.ResetRequested", true);
        await Run(g);
        Assert.Equal(0, g.Get("Scenario")); Assert.Equal(8, g.Get("ImportCapKw"));
        Assert.Equal(12, g.Get("Hour")); Assert.Equal(55, g.Get("BatterySoc"));
        Assert.Equal(0, g.Get("ImportEnergyKwh")); Assert.Equal(0, g.Get("OperatingCost"));
        Assert.False(g.Flag("IslandMode")); Assert.False(g.Flag("RunRequested")); Assert.False(g.Flag("ResetRequested"));
        Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.Microgrid.Status"]);
        Stopped(g);
    }

    [Theory]
    [InlineData("Scenario", 1.5)]
    [InlineData("ImportCapKw", 0)]
    [InlineData("ImportCapKw", 31)]
    [InlineData("Hour", 24)]
    [InlineData("Hour", double.NaN)]
    [InlineData("BatterySoc", 14)]
    [InlineData("BatterySoc", 96)]
    [InlineData("ImportEnergyKwh", -1)]
    [InlineData("OperatingCost", double.PositiveInfinity)]
    [InlineData("PvUsedKw", double.NaN)]
    [InlineData("HouseServedKw", double.NaN)]
    [InlineData("EvServedKw", double.PositiveInfinity)]
    [InlineData("EvServedKw", 0.01)]
    [InlineData("UnservedKw", -1)]
    public async Task InvalidNumericInputOrStateStopsAndResetRecovers(string field, double value)
    {
        var g = new MicrogridGlobals();
        g.Write("AI_Task.Microgrid.RunRequested", true);
        g.Set(field, value);
        await Run(g);
        Assert.Equal("Invalid numeric input/state - press Reset", g.Values["AI_Task.Microgrid.Status"]);
        Assert.False(g.Flag("RunRequested"));
        Stopped(g);
        g.Set("Scenario", 1); g.Set("ImportCapKw", 29); g.Set("BatterySoc", 95); g.Set("Hour", 22);
        g.Write("AI_Task.Microgrid.IslandMode", true); g.Write("AI_Task.Microgrid.ResetRequested", true);
        await Run(g);
        Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.Microgrid.Status"]);
        Assert.Equal(0, g.Get("Scenario")); Assert.Equal(8, g.Get("ImportCapKw"));
        Assert.Equal(55, g.Get("BatterySoc")); Assert.Equal(12, g.Get("Hour"));
        Assert.Equal(0, g.Get("ImportEnergyKwh")); Assert.Equal(0, g.Get("OperatingCost"));
        Assert.False(g.Flag("IslandMode")); Assert.False(g.Flag("RunRequested"));
        Stopped(g);
    }

    private static void Invariants(MicrogridGlobals g)
    {
        Assert.All(g.Values.Values.OfType<double>(), value => Assert.True(double.IsFinite(value)));
        Assert.InRange(g.Get("BatterySoc"), 15, 95);
        Assert.InRange(g.Get("PvKw"), 0, 18);
        Assert.InRange(g.Get("BatteryKw"), -8, 6);
        Assert.InRange(g.Get("UnservedKw"), 0, g.Get("HouseDemandKw") + g.Get("EvDemandKw"));
        Assert.InRange(g.Get("CurtailedPvKw"), 0, g.Get("PvKw"));
        Assert.InRange(g.Get("HouseServedKw"), 0, g.Get("HouseDemandKw"));
        Assert.InRange(g.Get("EvServedKw"), 0, g.Get("EvDemandKw"));
        Assert.InRange(g.Get("PvUsedKw"), 0, g.Get("PvKw"));
        Assert.Equal(g.Get("PvKw") + g.Get("GridKw") + g.Get("UnservedKw"),
            g.Get("HouseDemandKw") + g.Get("EvDemandKw") + g.Get("BatteryKw") + g.Get("CurtailedPvKw"), 8);
        Assert.Equal(g.Get("HouseDemandKw") + g.Get("EvDemandKw") - g.Get("UnservedKw"),
            g.Get("HouseServedKw") + g.Get("EvServedKw"), 8);
        if (g.Flag("IslandMode")) Assert.Equal(0, g.Get("GridKw"));
        else Assert.InRange(g.Get("GridKw"), -30, g.Get("ImportCapKw"));
        Assert.InRange(g.Get("Tariff"), 0, 1);
    }

    private static void Stopped(MicrogridGlobals g)
    {
        foreach (var name in new[] { "PvKw", "PvUsedKw", "LoadTotalKw", "HouseDemandKw", "EvDemandKw", "HouseServedKw", "EvServedKw", "BatteryKw", "GridKw", "UnservedKw", "CurtailedPvKw" })
            Assert.Equal(0, g.Get(name));
        Assert.Equal("STANDBY", g.Values["AI_Task.Microgrid.BatteryMode"]);
    }

    public class MicrogridGlobals
    {
        public Dictionary<string, object> Values { get; } = Bundle.Folder.Folders.Single().Variables.ToDictionary(
            v => "AI_Task.Microgrid." + v.Name,
            v => v.Type == "Boolean" ? (object)bool.Parse(v.InitialValue) : v.Type == "String" ? v.InitialValue : double.Parse(v.InitialValue, CultureInfo.InvariantCulture));

        public double ReadDouble(string path) => Convert.ToDouble(Values[path], CultureInfo.InvariantCulture);
        public bool ReadBool(string path) => Convert.ToBoolean(Values[path], CultureInfo.InvariantCulture);
        public void Write(string path, object value)
        {
            Assert.True(Values.ContainsKey(path), "Script wrote an undeclared microgrid variable: " + path);
            Values[path] = value;
        }
        public void Set(string name, double value) => Write("AI_Task.Microgrid." + name, value);
        public double Get(string name) => ReadDouble("AI_Task.Microgrid." + name);
        public bool Flag(string name) => ReadBool("AI_Task.Microgrid." + name);
    }
}
