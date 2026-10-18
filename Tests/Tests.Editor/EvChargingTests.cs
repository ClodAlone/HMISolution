using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class EvChargingTests
{
    private static readonly NodeModel Bundle = AiResourceBundle.Parse(EvChargingTemplate.Json, "AI_Task");
    private static readonly ScriptRunner<object> Run = CSharpScript.Create(
        EvChargingTemplate.SimulationCode.Replace("__TASK__", "AI_Task", StringComparison.Ordinal),
        ScriptOptions.Default.AddImports("System"), typeof(EvChargingGlobals)).CreateDelegate();

    [Fact]
    public void Bundle_ImportsTwiceWithIsolatedNamespacesValidBindingsAndUniqueSvgResources()
    {
        var editor = new NodeEditorService();
        editor.NewFile();
        var project = editor.ActiveProject!;
        var settings = project.Model.Server;
        var startup = settings.StartupScreen;
        for (var i = 0; i < 2; i++)
        {
            var result = editor.ImportAiResources(project, EvChargingTemplate.Json);
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
            Assert.Equal(prefix + "_EV Charging Station", screen.Name);
            Assert.Equal(prefix, screen.Group);
            Assert.Equal((1440d, 1040d), (screen.Width, screen.Height));
            Assert.Equal(prefix + "_EvChargingSimulation", script.Name);
            Assert.False(script.Enabled);
            Assert.Equal(250, script.IntervalMs);
            Assert.Contains($"const string p = \"{prefix}.EvCharging.\";", script.Code);
            Assert.DoesNotContain("__TASK__", script.Code);
            Assert.Equal("EvCharging", folder.Name);
            Assert.Equal(23, folder.Variables.Count);
            Assert.All(folder.Variables, v => Assert.Equal("ReadWrite", v.Access));
            Assert.All(screen.Symbols.Where(s => !string.IsNullOrEmpty(s.VariablePath)), s => Assert.StartsWith(prefix + ".EvCharging.", s.VariablePath));
            var artworkIndex = screen.Symbols.FindIndex(s => s.Id.EndsWith("artwork", StringComparison.Ordinal));
            Assert.True(artworkIndex >= 0);
            Assert.All(screen.Symbols.Where(s => s.Type == "pipe"), s => Assert.True(screen.Symbols.IndexOf(s) < artworkIndex));
            Assert.Equal(3, screen.Symbols.Count(s => s.Type == "button"));
            Assert.Equal(2, screen.Symbols.Count(s => s.Type == "editbox"));
            foreach (var symbol in screen.Symbols.Where(s => s.Type == "svg"))
            {
                Assert.DoesNotContain("__TASK__", symbol.SvgContent!);
                var svg = XElement.Parse(symbol.SvgContent!);
                foreach (var id in svg.DescendantsAndSelf().Attributes("id"))
                {
                    Assert.StartsWith(prefix + "_evc_", id.Value);
                    Assert.True(resourceIds.Add(id.Value), "Duplicate EV paint resource: " + id.Value);
                }
                foreach (var href in svg.DescendantsAndSelf().Attributes("href")) Assert.Contains(href.Value.TrimStart('#'), resourceIds);
                foreach (var value in svg.DescendantsAndSelf().Attributes().Select(a => a.Value))
                    foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(value, "url\\(#([^)]*)\\)"))
                        Assert.Contains(match.Groups[1].Value, resourceIds);
            }
            for (var stall = 1; stall <= 3; stall++)
            {
                var car = Assert.Single(screen.Symbols, s => s.Id.EndsWith("car" + stall, StringComparison.Ordinal));
                Assert.Equal(prefix + ".EvCharging.Present" + stall, car.VariablePath);
                Assert.Equal("value == true", car.VisibilityBinding);
                Assert.Equal(prefix + ".EvCharging.Soc" + stall, Assert.Single(screen.Symbols, s => s.Id.EndsWith("socbar" + stall, StringComparison.Ordinal)).VariablePath);
            }
        }
        Assert.Empty(new ProjectValidationService(new SyntaxCheckService(), new CrossReferenceService()).Validate(model).Where(i => i.Severity == IssueSeverity.Error));
    }

    [Theory]
    [InlineData("run", "RunRequested", "ToggleVariable", "")]
    [InlineData("reset", "ResetRequested", "WriteVariable", "true")]
    [InlineData("arrival", "ArrivalRequested", "WriteVariable", "true")]
    public void Controls_BindRunResetAndManualArrivalsToDeclaredInputs(string id, string variable, string action, string value)
    {
        var screen = Assert.Single(Bundle.Screens);
        var button = Assert.Single(screen.Symbols, s => s.Id == "evcharging_" + id);
        var command = Assert.Single(button.Commands);
        Assert.Equal("Click", command.Trigger);
        Assert.Equal(action, command.Action);
        Assert.Equal(value, command.Value);
        Assert.Equal("AI_Task.EvCharging." + variable, command.VariablePath);
        Assert.Contains(Assert.Single(Bundle.Folder.Folders).Variables, v => v.Name == variable && v.Type == "Boolean");
        Assert.All(screen.Symbols.Where(s => s.Type == "pipe"), pipe => Assert.True(pipe.PipeAnimate));
    }

    [Fact]
    public void ChargerBranches_AreSeparateAnimatedVerticalFeedsWithTheirOwnPowerBindings()
    {
        var screen = Bundle.Screens.Single();
        for (var stall = 1; stall <= 3; stall++)
        {
            var idle = Assert.Single(screen.Symbols, s => s.Id == "evcharging_branch" + stall + "_idle");
            var active = Assert.Single(screen.Symbols, s => s.Id == "evcharging_branch" + stall);
            Assert.Equal("vertical", active.PipeOrientation);
            Assert.Equal("top-bottom", active.PipeFlowDirection);
            Assert.Equal("AI_Task.EvCharging.Charger" + stall + "Kw", active.VariablePath);
            Assert.Equal("value > 0", active.VisibilityBinding);
            Assert.Equal((idle.X, idle.Y, idle.Width, idle.Height), (active.X, active.Y, active.Width, active.Height));
            Assert.True(active.PipeAnimate && active.PipeShowFlow);
            Assert.False(idle.PipeShowFlow);
        }
    }

    [Fact]
    public async Task SharedCharging_RespectsSiteAndPerChargerLimitsAndConservesMeteredEnergy()
    {
        var g = new EvChargingGlobals();
        g.Set("SiteLimitKw", 12);
        g.Write("AI_Task.EvCharging.RunRequested", true);
        await Run(g);
        Assert.Equal(12, g.Get("SitePowerKw"), 10);
        Assert.Equal(4, g.Get("Charger1Kw"), 10);
        Assert.Equal(4, g.Get("Charger2Kw"), 10);
        Assert.Equal(4, g.Get("Charger3Kw"), 10);
        Assert.Equal(g.Get("Charger1Kw") + g.Get("Charger2Kw") + g.Get("Charger3Kw"), g.Get("SitePowerKw"), 10);
        Assert.True(g.Get("SitePowerKw") <= g.Get("SiteLimitKw"));
        var batteryGainKwh = ((g.Get("Soc1") - 35) + (g.Get("Soc2") - 55) + (g.Get("Soc3") - 72)) * 60 / 100;
        Assert.Equal(g.Get("EnergyKwh") * 0.92, batteryGainKwh, 9);
        Invariants(g);

        g.Set("SiteLimitKw", 33);
        await Run(g);
        Assert.All(new[] { "Charger1Kw", "Charger2Kw", "Charger3Kw" }, name => Assert.InRange(g.Get(name), 0, 11));
        Assert.InRange(g.Get("SitePowerKw"), 0, 33);
        Invariants(g);
    }

    [Fact]
    public async Task ManualAndAutomaticArrivalsQueueFairlyAndDepartAtTarget()
    {
        var g = new EvChargingGlobals();
        g.Set("ArrivalBatch", 2);
        g.Write("AI_Task.EvCharging.ArrivalRequested", true);
        await Run(g);
        Assert.Equal(0, g.Get("QueueCount"));
        Assert.True(g.Flag("ArrivalRequested"));
        g.Write("AI_Task.EvCharging.RunRequested", true);
        await Run(g);
        Assert.Equal(2, g.Get("QueueCount"));
        Assert.Equal(5, g.Get("Arrived"));
        Assert.False(g.Flag("ArrivalRequested"));
        g.Set("Soc1", 84.999);
        await Run(g);
        Assert.Equal(85, g.Get("Soc1"), 8);
        await Run(g);
        Assert.Equal(1, g.Get("Departed"));
        Assert.Equal(1, g.Get("QueueCount"));
        Assert.True(g.Flag("Present1"));
        Assert.Equal(20, g.Get("Soc1"));
        Assert.Equal(g.Get("Arrived"), g.Get("Departed") + g.Get("QueueCount") + g.Get("ActiveCount"));
        Invariants(g);
    }

    [Fact]
    public async Task AutomaticDayCycle_AdvancesClockAndKeepsEveryStallAndQueueBounded()
    {
        var g = new EvChargingGlobals();
        g.Set("SiteLimitKw", 33);
        g.Write("AI_Task.EvCharging.RunRequested", true);
        for (var i = 0; i < 1440; i++)
        {
            await Run(g);
            Invariants(g);
            Assert.InRange(g.Get("QueueCount"), 0, 500);
            Assert.True(g.Flag("RunRequested"));
        }
        Assert.Equal(8, g.Get("Hour"), 7);
        Assert.Equal(1440, g.Get("Cycle"));
        Assert.Equal(15, g.Get("Arrived"));
        Assert.True(g.Get("EnergyKwh") > 0);
        Assert.True(g.Get("Departed") > 0);
    }

    [Fact]
    public async Task PauseFreezesTimeSocQueueAndEnergyAndResetRestoresAllNumericControls()
    {
        var g = new EvChargingGlobals();
        g.Write("AI_Task.EvCharging.RunRequested", true);
        await Run(g);
        var frozen = new[] { "Hour", "Cycle", "QueueCount", "EnergyKwh", "Soc1", "Soc2", "Soc3", "Arrived", "Departed" }
            .ToDictionary(name => name, g.Get);
        g.Write("AI_Task.EvCharging.RunRequested", false);
        await Run(g);
        foreach (var (name, value) in frozen) Assert.Equal(value, g.Get(name));
        Stopped(g);

        g.Set("SiteLimitKw", 3); g.Set("ArrivalBatch", 3); g.Set("Soc2", 84); g.Set("Hour", 20);
        g.Write("AI_Task.EvCharging.ResetRequested", true);
        await Run(g);
        Assert.Equal(22, g.Get("SiteLimitKw")); Assert.Equal(1, g.Get("ArrivalBatch"));
        Assert.Equal(8, g.Get("Hour")); Assert.Equal(0, g.Get("Cycle")); Assert.Equal(0, g.Get("QueueCount"));
        Assert.Equal(3, g.Get("Arrived")); Assert.Equal(0, g.Get("Departed")); Assert.Equal(3, g.Get("ActiveCount"));
        Assert.Equal(0, g.Get("EnergyKwh")); Assert.Equal(35, g.Get("Soc1")); Assert.Equal(55, g.Get("Soc2")); Assert.Equal(72, g.Get("Soc3"));
        Assert.False(g.Flag("RunRequested")); Assert.False(g.Flag("ResetRequested")); Assert.False(g.Flag("ArrivalRequested"));
        Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.EvCharging.Status"]);
        Stopped(g);
    }

    [Theory]
    [InlineData("SiteLimitKw", double.NaN)]
    [InlineData("SiteLimitKw", 34)]
    [InlineData("ArrivalBatch", 1.5)]
    [InlineData("Hour", 24)]
    [InlineData("Cycle", double.PositiveInfinity)]
    [InlineData("QueueCount", -1)]
    [InlineData("Soc2", 86)]
    [InlineData("EnergyKwh", -1)]
    [InlineData("ActiveCount", 2)]
    [InlineData("Charger1Kw", double.NaN)]
    public async Task InvalidNumericInputOrStateStopsAndResetRecovers(string field, double value)
    {
        var g = new EvChargingGlobals();
        g.Write("AI_Task.EvCharging.RunRequested", true);
        g.Set(field, value);
        await Run(g);
        Assert.Equal("Invalid numeric input/state - press Reset", g.Values["AI_Task.EvCharging.Status"]);
        Assert.False(g.Flag("RunRequested"));
        Stopped(g);
        g.Write("AI_Task.EvCharging.ResetRequested", true);
        await Run(g);
        Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.EvCharging.Status"]);
        Assert.Equal(22, g.Get("SiteLimitKw")); Assert.Equal(1, g.Get("ArrivalBatch"));
        Assert.Equal(0, g.Get("EnergyKwh")); Assert.Equal(3, g.Get("ActiveCount"));
        Assert.Equal(35, g.Get("Soc1")); Assert.Equal(55, g.Get("Soc2")); Assert.Equal(72, g.Get("Soc3"));
        Assert.False(g.Flag("RunRequested"));
        Stopped(g);
    }

    private static void Invariants(EvChargingGlobals g)
    {
        Assert.All(g.Values.Values.OfType<double>(), value => Assert.True(double.IsFinite(value)));
        Assert.InRange(g.Get("SiteLimitKw"), 3, 33);
        Assert.InRange(g.Get("SitePowerKw"), 0, g.Get("SiteLimitKw"));
        Assert.InRange(g.Get("QueueCount"), 0, 500);
        Assert.InRange(g.Get("ActiveCount"), 0, 3);
        Assert.Equal(g.Get("Arrived"), g.Get("Departed") + g.Get("QueueCount") + g.Get("ActiveCount"));
        for (var i = 1; i <= 3; i++)
        {
            Assert.InRange(g.Get("Charger" + i + "Kw"), 0, 11);
            Assert.InRange(g.Get("Soc" + i), 0, 85);
            Assert.Equal(g.Flag("Present" + i), g.Get("Soc" + i) > 0 || g.Get("Soc" + i) == 85);
        }
    }

    private static void Stopped(EvChargingGlobals g)
    {
        Assert.Equal(0, g.Get("SitePowerKw"));
        for (var i = 1; i <= 3; i++) Assert.Equal(0, g.Get("Charger" + i + "Kw"));
    }

    public class EvChargingGlobals
    {
        public Dictionary<string, object> Values { get; } = Bundle.Folder.Folders.Single().Variables.ToDictionary(
            v => "AI_Task.EvCharging." + v.Name,
            v => v.Type == "Boolean" ? (object)bool.Parse(v.InitialValue) : v.Type == "String" ? v.InitialValue : double.Parse(v.InitialValue, CultureInfo.InvariantCulture));

        public double ReadDouble(string path) => Convert.ToDouble(Values[path], CultureInfo.InvariantCulture);
        public bool ReadBool(string path) => Convert.ToBoolean(Values[path], CultureInfo.InvariantCulture);
        public void Write(string path, object value)
        {
            Assert.True(Values.ContainsKey(path), "Script wrote an undeclared EV variable: " + path);
            Values[path] = value;
        }
        public void Set(string name, double value) => Write("AI_Task.EvCharging." + name, value);
        public double Get(string name) => ReadDouble("AI_Task.EvCharging." + name);
        public bool Flag(string name) => ReadBool("AI_Task.EvCharging." + name);
    }
}
