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

public class CompressedAirTests
{
    private static readonly NodeModel Bundle = AiResourceBundle.Parse(CompressedAirTemplate.Json, "AI_Task");
    private static readonly ScriptRunner<object> Run = CSharpScript.Create(CompressedAirTemplate.SimulationCode.Replace("__TASK__", "AI_Task", StringComparison.Ordinal),
        ScriptOptions.Default.AddImports("System"), typeof(AirGlobals)).CreateDelegate();

    [Fact]
    public void Bundle_ImportsTwiceWithIndependentVariablesScriptsArtworkAndControls()
    {
        var editor = new NodeEditorService(); editor.NewFile();
        var project = editor.ActiveProject!; var settings = project.Model.Server; var startup = settings.StartupScreen;
        for (var i = 0; i < 2; i++) { var result = editor.ImportAiResources(project, CompressedAirTemplate.Json); Assert.True(result.success, result.message); }
        editor.RefreshJsonFromTree();
        var model = JsonSerializer.Deserialize<NodeModel>(editor.JsonEditorText)!;
        Assert.Same(settings, project.Model.Server); Assert.Equal(startup, settings.StartupScreen);
        Assert.Equal(2, model.Screens.Count); Assert.Equal(2, model.Scripts.Count);
        var ids = new HashSet<string>();
        for (var i = 0; i < 2; i++)
        {
            var prefix = i == 0 ? "AI_Task" : "AI_Task_2";
            var screen = model.Screens[i];
            Assert.Equal(prefix + "_Compressed-Air Station", screen.Name);
            Assert.Empty(screen.SwipeLeftScreen); Assert.Empty(screen.SwipeRightScreen);
            Assert.Equal(screen.Symbols.Count, screen.Symbols.Select(s => s.Id).Distinct().Count());
            Assert.Equal(5, screen.Symbols.Count(s => s.Type == "button"));
            Assert.All(screen.Symbols.Where(s => !string.IsNullOrEmpty(s.VariablePath)), s => Assert.StartsWith(prefix + ".CompressedAir.", s.VariablePath));
            Assert.False(model.Scripts[i].Enabled); Assert.Equal(250, model.Scripts[i].IntervalMs);
            Assert.Contains($"const string p = \"{prefix}.CompressedAir.\";", model.Scripts[i].Code);
            Assert.Equal("CompressedAir", Assert.Single(model.Folder.Folders[i].Folders).Name);
            foreach (var symbol in screen.Symbols.Where(s => s.Type == "svg"))
            {
                var svg = XElement.Parse(symbol.SvgContent!);
                foreach (var id in svg.Descendants().Attributes("id")) Assert.True(ids.Add(id.Value), "Duplicate paint ID: " + id.Value);
                foreach (var href in svg.Descendants().Attributes("href")) Assert.Contains(href.Value[1..], ids);
            }
        }
        Assert.Empty(new ProjectValidationService(new SyntaxCheckService(), new CrossReferenceService()).Validate(model).Where(i => i.Severity == IssueSeverity.Error));
    }

    [Theory]
    [InlineData(6.5, false, false, true, false)]
    [InlineData(6.0, false, false, true, true)]
    [InlineData(7.0, true, true, true, true)]
    [InlineData(7.5, true, true, true, false)]
    [InlineData(8.0, true, true, false, false)]
    public async Task Stages_UseIndependentStartStopHysteresis(double pressure, bool lead, bool standby, bool expectedLead, bool expectedStandby)
    {
        var g = new AirGlobals(); g.Set("PressureBar", pressure);
        g.Write("AI_Task.CompressedAir.RunRequested", true); g.Write("AI_Task.CompressedAir.Stage1Demand", lead); g.Write("AI_Task.CompressedAir.Stage2Demand", standby);
        await Run(g);
        Assert.Equal(expectedLead, g.Flag("Compressor1On")); Assert.Equal(expectedStandby, g.Flag("Compressor2On"));
        Assert.Equal((expectedLead ? 1.8 : 0) + (expectedStandby ? 1.8 : 0), g.Get("InletFlow"), 8);
        Assert.Equal((expectedLead ? 11 : 0) + (expectedStandby ? 11 : 0), g.Get("PowerKw"));
        Balance(pressure, g);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task LongRun_ConservesAirAndEnergyAndStagesUnderDemand(bool peak, bool leak)
    {
        var g = new AirGlobals(); g.Write("AI_Task.CompressedAir.RunRequested", true);
        g.Write("AI_Task.CompressedAir.PeakRequested", peak); g.Write("AI_Task.CompressedAir.LeakRequested", leak);
        var c1 = false; var c2 = false; double energy = 0, delivered = 0, leaked = 0;
        for (var i = 0; i < 2400; i++)
        {
            var pressure = g.Get("PressureBar");
            await Run(g); Balance(pressure, g);
            Assert.True(g.Flag("RunRequested"));
            c1 |= g.Flag("Compressor1On"); c2 |= g.Flag("Compressor2On");
            energy += g.Get("PowerKw") * .05 / 60; delivered += g.Get("LoadFlow") * .05; leaked += g.Get("LeakFlow") * .05;
            Assert.Equal(peak ? 2.8 : 1.0, g.Get("LoadFlow") + g.Get("UnmetFlow"), 8);
        }
        Assert.True(c1); if (peak) Assert.True(c2);
        Assert.Equal(energy, g.Get("EnergyKwh"), 8); Assert.Equal(delivered, g.Get("DeliveredNm3"), 8); Assert.Equal(leaked, g.Get("LeakedNm3"), 8);
        Assert.Equal(leak, g.Get("LeakedNm3") > 0);
        Assert.Equal(120, g.Get("Minute"), 8);
    }

    [Fact]
    public async Task Fault_InterlocksLeadAndStandbyServesUntilCapacityIsExceeded()
    {
        var g = new AirGlobals(); g.Write("AI_Task.CompressedAir.RunRequested", true);
        g.Write("AI_Task.CompressedAir.FaultRequested", true); g.Set("PressureBar", 6);
        await Run(g); Assert.False(g.Flag("Compressor1On")); Assert.True(g.Flag("Compressor2On"));
        Assert.Equal(1.8, g.Get("InletFlow")); Assert.Contains("C1 FAULT", g.Values["AI_Task.CompressedAir.Status"].ToString()!);
        g.Write("AI_Task.CompressedAir.PeakRequested", true); g.Set("PressureBar", 0);
        await Run(g); Assert.InRange(g.Get("PressureBar"), 0, 8.6);
        Assert.Equal(1.8, g.Get("LoadFlow"), 8); Assert.Equal(1, g.Get("UnmetFlow"), 8);
        Assert.Contains("LOW PRESSURE", g.Values["AI_Task.CompressedAir.Status"].ToString()!);
        g.Write("AI_Task.CompressedAir.FaultRequested", false); await Run(g);
        Assert.True(g.Flag("Compressor1On")); Assert.True(g.Flag("Compressor2On"));
    }

    [Fact]
    public async Task PauseHoldsTotalsAndStageMemory_ResetRestoresDefaultsAndStops()
    {
        var g = new AirGlobals(); g.Set("PressureBar", 6); g.Write("AI_Task.CompressedAir.RunRequested", true); await Run(g);
        g.Write("AI_Task.CompressedAir.RunRequested", false);
        var held = new[] { "PressureBar", "Minute", "EnergyKwh", "DeliveredNm3", "LeakedNm3" }.ToDictionary(n => n, g.Get);
        await Run(g); Stopped(g); Assert.True(g.Flag("Stage1Demand"));
        foreach (var (key, value) in held) Assert.Equal(value, g.Get(key));
        g.Write("AI_Task.CompressedAir.LeakRequested", true); g.Write("AI_Task.CompressedAir.FaultRequested", true); g.Write("AI_Task.CompressedAir.PeakRequested", true);
        g.Write("AI_Task.CompressedAir.ResetRequested", true); await Run(g); Stopped(g);
        Assert.Equal(7.2, g.Get("PressureBar")); Assert.Equal(0, g.Get("Minute")); Assert.Equal(0, g.Get("EnergyKwh"));
        foreach (var name in new[] { "RunRequested", "ResetRequested", "LeakRequested", "FaultRequested", "PeakRequested", "Stage1Demand", "Stage2Demand" }) Assert.False(g.Flag(name));
    }

    [Theory]
    [InlineData("PressureBar", double.NaN)]
    [InlineData("PressureBar", -1)]
    [InlineData("PressureBar", 8.7)]
    [InlineData("Minute", -1)]
    [InlineData("EnergyKwh", double.PositiveInfinity)]
    [InlineData("DeliveredNm3", -1)]
    [InlineData("LeakedNm3", -1)]
    public async Task InvalidState_StopsExplicitlyAndResetRecovers(string key, double value)
    {
        var g = new AirGlobals(); g.Set(key, value); g.Write("AI_Task.CompressedAir.RunRequested", true);
        await Run(g); Stopped(g); Assert.False(g.Flag("RunRequested"));
        Assert.Equal("Invalid inputs - press Reset", g.Values["AI_Task.CompressedAir.Status"]);
        g.Write("AI_Task.CompressedAir.ResetRequested", true); await Run(g);
        Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.CompressedAir.Status"]); Assert.Equal(7.2, g.Get("PressureBar"));
    }

    [Fact]
    public void ControlsAndVisualBindings_ResolveToActualStatesAndCommonTaskLoadsLocally()
    {
        var screen = Bundle.Screens.Single();
        foreach (var button in screen.Symbols.Where(s => s.Type == "button"))
        {
            var command = Assert.Single(button.Commands); Assert.Equal("Click", command.Trigger);
            Assert.StartsWith("AI_Task.CompressedAir.", command.VariablePath);
            Assert.Equal(button.Id.EndsWith("reset", StringComparison.Ordinal) ? "WriteVariable" : "ToggleVariable", command.Action);
        }
        var c1 = Assert.Single(screen.Symbols, s => s.Id == "compressedair_motor1"); Assert.Equal("AI_Task.CompressedAir.FaultRequested", c1.MotorFaultPath);
        Assert.All(screen.Symbols.Where(s => s.Type == "pipe"), s => Assert.True(s.PipeUseGradient));
        var task = AiTaskCatalog.Tasks.Single(t => t.Id == "screen-compressed-air"); Assert.Equal(CompressedAirTemplate.Json, task.ResourceBundle); Assert.Contains(CompressedAirTemplate.Json, task.Prompt);
        var editor = new NodeEditorService(); editor.NewFile(); var component = new JsonEditor();
        typeof(JsonEditor).GetProperty(nameof(JsonEditor.ResourceProject))!.SetValue(component, editor.ActiveProject);
        typeof(JsonEditor).GetMethod("OnTaskChanged", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, [new ChangeEventArgs { Value = task.Id }]);
        typeof(JsonEditor).GetMethod("LoadTaskTemplate", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null);
        Assert.Equal(CompressedAirTemplate.Json, component.Session.ResourceJson); Assert.Same(editor.ActiveProject, component.Session.ResourceTarget);
        Assert.Empty(editor.ActiveProject!.Model.Screens);
    }

    [Fact]
    public void PressureGauge_RendersSupportedGradientBackgroundAndPressureUnit()
    {
        var gauge = Assert.Single(Bundle.Screens.Single().Symbols, s => s.Type == "gauge");
        var svg = XElement.Parse(GaugeVisuals.Render(gauge, gauge.Fill, .72, "7.20 " + gauge.GaugeUnit, 0, 10));
        XNamespace ns = "http://www.w3.org/2000/svg";
        Assert.Equal("slate", gauge.GaugeBackground);
        Assert.NotEmpty(svg.Descendants(ns + "radialGradient"));
        Assert.NotEmpty(svg.Descendants(ns + "linearGradient"));
        Assert.Contains(svg.Descendants(ns + "text"), t => t.Value == "7.20 bar(g)" && (string?)t.Attribute("fill") == "#e2e8f0");
    }

    private static void Balance(double previous, AirGlobals g)
    {
        Assert.Equal(previous + (g.Get("InletFlow") - g.Get("LoadFlow") - g.Get("LeakFlow") - g.Get("VentFlow")) * .05 / 2, g.Get("PressureBar"), 8);
        Assert.InRange(g.Get("PressureBar"), 0, 8.6); Assert.InRange(g.Get("InletFlow"), 0, 3.6);
        Assert.InRange(g.Get("LoadFlow"), 0, 2.8); Assert.InRange(g.Get("LeakFlow"), 0, 1);
        Assert.InRange(g.Get("PowerKw"), 0, 22);
        foreach (var value in g.Values.Values.OfType<double>()) Assert.True(double.IsFinite(value));
    }
    private static void Stopped(AirGlobals g)
    {
        foreach (var name in new[] { "Compressor1Flow", "Compressor2Flow", "InletFlow", "LoadFlow", "LeakFlow", "VentFlow", "UnmetFlow", "PowerKw" }) Assert.Equal(0, g.Get(name));
        Assert.False(g.Flag("Compressor1On")); Assert.False(g.Flag("Compressor2On"));
    }
    public class AirGlobals
    {
        public Dictionary<string, object> Values { get; } = Bundle.Folder.Folders.Single().Variables.ToDictionary(v => "AI_Task.CompressedAir." + v.Name,
            v => v.Type == "Boolean" ? (object)bool.Parse(v.InitialValue) : v.Type == "String" ? v.InitialValue : double.Parse(v.InitialValue, CultureInfo.InvariantCulture));
        public double ReadDouble(string path) => Convert.ToDouble(Values[path], CultureInfo.InvariantCulture);
        public bool ReadBool(string path) => Convert.ToBoolean(Values[path], CultureInfo.InvariantCulture);
        public void Write(string path, object value) { Assert.True(Values.ContainsKey(path), "Undeclared write: " + path); Values[path] = value; }
        public void Set(string key, double value) => Write("AI_Task.CompressedAir." + key, value);
        public double Get(string key) => ReadDouble("AI_Task.CompressedAir." + key);
        public bool Flag(string key) => ReadBool("AI_Task.CompressedAir." + key);
    }
}
