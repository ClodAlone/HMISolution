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

public class HvacPlantTests
{
    private static readonly NodeModel Bundle = AiResourceBundle.Parse(HvacPlantTemplate.Json, "AI_Task");
    private static readonly ScriptRunner<object> Run = CSharpScript.Create(HvacPlantTemplate.SimulationCode.Replace("__TASK__", "AI_Task", StringComparison.Ordinal),
        ScriptOptions.Default.AddImports("System"), typeof(HvacGlobals)).CreateDelegate();

    [Fact]
    public void Bundle_ImportsIndependentCopiesWithAllDependenciesAndPreservesExistingProject()
    {
        var editor = new NodeEditorService();
        editor.NewFile();
        var project = editor.ActiveProject!;
        var settings = project.Model.Server;
        var startup = settings.StartupScreen;
        for (var i = 0; i < 2; i++)
        {
            var result = editor.ImportAiResources(project, HvacPlantTemplate.Json);
            Assert.True(result.success, result.message);
        }
        editor.RefreshJsonFromTree();
        var model = JsonSerializer.Deserialize<NodeModel>(editor.JsonEditorText)!;
        Assert.Same(settings, project.Model.Server);
        Assert.Equal(startup, settings.StartupScreen);
        Assert.Equal(2, model.Screens.Count);
        Assert.Equal(2, model.Scripts.Count);
        var paintIds = new HashSet<string>();
        for (var i = 0; i < 2; i++)
        {
            var prefix = i == 0 ? "AI_Task" : "AI_Task_2";
            var screen = model.Screens[i];
            Assert.Equal(prefix + "_HVAC Plant", screen.Name);
            Assert.Equal(prefix, screen.Group);
            Assert.Equal((1440d, 1080d), (screen.Width, screen.Height));
            Assert.Empty(screen.SwipeLeftScreen);
            Assert.Empty(screen.SwipeRightScreen);
            Assert.Equal(screen.Symbols.Count, screen.Symbols.Select(s => s.Id).Distinct().Count());
            Assert.All(screen.Symbols.Where(s => !string.IsNullOrEmpty(s.VariablePath)), s => Assert.StartsWith(prefix + ".Hvac.", s.VariablePath));
            Assert.Equal(9, screen.Symbols.Count(s => s.Type == "button"));
            Assert.Equal(2, screen.Symbols.Count(s => s.Type == "editbox"));
            var script = model.Scripts[i];
            Assert.False(script.Enabled);
            Assert.Equal(250, script.IntervalMs);
            Assert.Contains($"const string p = \"{prefix}.Hvac.\";", script.Code);
            var folder = Assert.Single(model.Folder.Folders[i].Folders);
            Assert.Equal("Hvac", folder.Name);
            Assert.Equal(32, folder.Variables.Count);
            foreach (var symbol in screen.Symbols.Where(s => s.Type == "svg"))
            {
                var svg = XElement.Parse(symbol.SvgContent!);
                foreach (var id in svg.Descendants().Attributes("id")) Assert.True(paintIds.Add(id.Value), "Duplicate SVG resource: " + id.Value);
                Assert.DoesNotContain("__TASK__", symbol.SvgContent!);
            }
            var fan = Assert.Single(screen.Symbols, s => s.Id == "hvac_fan_running");
            Assert.Contains(prefix + "_hv_spin", fan.SvgContent!);
            Assert.Contains("prefers-reduced-motion:reduce", fan.SvgContent!);
        }
        Assert.Empty(new ProjectValidationService(new SyntaxCheckService(), new CrossReferenceService()).Validate(model).Where(i => i.Severity == IssueSeverity.Error));
    }

    [Fact]
    public void Visuals_UseSignedModeFlowsActualPumpFaultAndBoundedEditableInputs()
    {
        var screen = Assert.Single(Bundle.Screens);
        ScreenSymbol Symbol(string id) => Assert.Single(screen.Symbols, s => s.Id == "hvac_" + id);
        var heat = Symbol("supply_heat");
        var cool = Symbol("supply_cool");
        Assert.Equal("AI_Task.Hvac.HeatingFlow", heat.VariablePath);
        Assert.Equal("AI_Task.Hvac.CoolingFlow", cool.VariablePath);
        Assert.Equal("#fb7185", heat.PipeFlowColor);
        Assert.Equal("#22d3ee", cool.PipeFlowColor);
        Assert.Equal("value == true", heat.VisibilityBinding);
        Assert.Equal("value == true", cool.VisibilityBinding);
        Assert.Equal("left-right", heat.PipeFlowDirection);
        Assert.Equal("left-right", cool.PipeFlowDirection);
        Assert.Equal("right-left", Symbol("return").PipeFlowDirection);
        Assert.Equal("AI_Task.Hvac.PumpOn", Symbol("return").VariablePath);
        Assert.False(Symbol("supply_idle").PipeShowFlow);
        Assert.All(screen.Symbols.Where(s => s.Type == "pipe"), s =>
        {
            Assert.True(s.PipeUseGradient);
            Assert.True(screen.Symbols.IndexOf(s) < screen.Symbols.IndexOf(Symbol("artwork")));
        });
        Assert.Equal("isometric", Symbol("pump").MotorAppearance);
        Assert.Equal("AI_Task.Hvac.PumpOn", Symbol("pump").VariablePath);
        Assert.Equal("AI_Task.Hvac.PumpFault", Symbol("pump").MotorFaultPath);
        Assert.Equal("AI_Task.Hvac.FanOn", Symbol("fan_running").VariablePath);
        Assert.Equal("value == true", Symbol("fan_running").VisibilityBinding);
        Assert.Equal("value == false", Symbol("fan_idle").VisibilityBinding);
        Assert.Equal((16d, 28d, .5d), (Symbol("setpoint").MinValue, Symbol("setpoint").MaxValue, Symbol("setpoint").EditBoxStep));
        Assert.Equal((20d, 100d, 5d), (Symbol("manual_speed").MinValue, Symbol("manual_speed").MaxValue, Symbol("manual_speed").EditBoxStep));
    }

    [Theory]
    [InlineData("run", "RunRequested", "ToggleVariable", "")]
    [InlineData("reset", "ResetRequested", "WriteVariable", "true")]
    [InlineData("heat", "Mode", "WriteVariable", "0")]
    [InlineData("cool", "Mode", "WriteVariable", "1")]
    [InlineData("auto", "AutoControl", "ToggleVariable", "")]
    [InlineData("cold", "Scenario", "WriteVariable", "0")]
    [InlineData("hot", "Scenario", "WriteVariable", "1")]
    [InlineData("window", "Scenario", "WriteVariable", "2")]
    [InlineData("fault", "Scenario", "WriteVariable", "3")]
    public void Controls_UseExistingRuntimeCommandsAndScenarioModeSelection(string id, string variable, string action, string value)
    {
        var button = Assert.Single(Bundle.Screens.Single().Symbols, s => s.Id == "hvac_" + id);
        var command = button.Commands[0];
        Assert.Equal("Click", command.Trigger);
        Assert.Equal(action, command.Action);
        Assert.Equal(value, command.Value);
        Assert.Equal("AI_Task.Hvac." + variable, command.VariablePath);
        if (id is "cold" or "hot" or "window")
        {
            Assert.Equal(2, button.Commands.Count);
            Assert.Equal("AI_Task.Hvac.Mode", button.Commands[1].VariablePath);
            Assert.Equal("WriteVariable", button.Commands[1].Action);
            Assert.Equal(id == "hot" ? "1" : "0", button.Commands[1].Value);
        }
        else Assert.Single(button.Commands);
    }

    [Theory]
    [InlineData(0, 0, 18, 30, 1)]
    [InlineData(1, 1, 28, 14, -1)]
    public async Task ActivePlant_ConservesRoomAndBufferEnergyAndCalculatesCop(int mode, int scenario, double room, double buffer, int sign)
    {
        var g = new HvacGlobals();
        g.Set("Mode", mode); g.Set("Scenario", scenario); g.Set("RoomTemp", room); g.Set("BufferTemp", buffer);
        g.Write("AI_Task.Hvac.RunRequested", true);
        await Run(g);
        Assert.Equal(sign, Math.Sign(g.Get("ThermalKw")));
        Assert.Equal(sign, Math.Sign(g.Get("CoilKw")));
        Assert.Equal(buffer, g.Get("SupplyTemp"));
        Assert.Equal(buffer - g.Get("CoilKw") / (1.163 * g.Get("FlowM3h")), g.Get("ReturnTemp"), 9);
        Assert.Equal(buffer + (g.Get("ThermalKw") - g.Get("CoilKw") - g.Get("BufferLossKw")) / 60 / .7, g.Get("BufferTemp"), 9);
        Assert.Equal(room + (g.Get("CoilKw") - g.Get("BuildingLossKw") + g.Get("InternalHeatKw")) / 60 / 3, g.Get("RoomTemp"), 9);
        Assert.Equal(Math.Abs(g.Get("ThermalKw")) / g.Get("Cop") + .12 * Math.Pow(g.Get("PumpSpeed") / 100, 3), g.Get("ElectricalKw"), 9);
        Assert.Equal(g.Get("ElectricalKw") / 60, g.Get("EnergyKwh"), 9);
        Assert.Equal(6 + 1.0 / 60, g.Get("Hour"), 9);
        Assert.Equal(mode == 0, g.Flag("HeatingFlow"));
        Assert.Equal(mode == 1, g.Flag("CoolingFlow"));
        Assert.True(g.Flag("FanOn"));
        Assert.True(g.Flag("PumpOn"));
        Invariants(g);
    }

    [Theory]
    [InlineData(0, 0, 21.2, true, true)]
    [InlineData(0, 0, 21.2, false, false)]
    [InlineData(0, 0, 21.4, true, false)]
    [InlineData(0, 0, 20.6, false, true)]
    [InlineData(1, 1, 20.8, true, true)]
    [InlineData(1, 1, 20.8, false, false)]
    [InlineData(1, 1, 20.6, true, false)]
    [InlineData(1, 1, 21.4, false, true)]
    public async Task Thermostat_UsesHysteresisAndRetainsDemandInsideDeadband(int mode, int scenario, double room, bool previous, bool demand)
    {
        var g = new HvacGlobals();
        g.Write("AI_Task.Hvac.RunRequested", true); g.Write("AI_Task.Hvac.Demand", previous);
        g.Set("Mode", mode); g.Set("Scenario", scenario); g.Set("RoomTemp", room);
        await Run(g);
        Assert.Equal(demand, g.Flag("Demand"));
        Assert.Equal(demand, g.Flag("PumpOn"));
        if (!demand)
        {
            Stopped(g);
            Assert.Equal("Setpoint satisfied - thermostat standby", g.Values["AI_Task.Hvac.Status"]);
        }
    }

    [Theory]
    [InlineData(20)]
    [InlineData(65)]
    [InlineData(100)]
    public async Task Manual_ConditionsContinuouslyAtRequestedSpeedEvenWhenThermostatIsSatisfied(double speed)
    {
        var g = new HvacGlobals();
        g.Write("AI_Task.Hvac.RunRequested", true); g.Write("AI_Task.Hvac.AutoControl", false);
        g.Set("RoomTemp", 23); g.Set("ManualSpeed", speed);
        await Run(g);
        Assert.True(g.Flag("Demand"));
        Assert.Equal(speed, g.Get("PumpSpeed"));
        Assert.Equal(3 * speed / 100, g.Get("FlowM3h"), 9);
        Assert.Equal("MANUAL / CONTINUOUS", g.Values["AI_Task.Hvac.ControlText"]);
    }

    [Fact]
    public async Task Fault_StopsPumpAndCompressorButTimeAndBuildingDriftContinue()
    {
        var g = new HvacGlobals();
        g.Write("AI_Task.Hvac.RunRequested", true); g.Set("Scenario", 3);
        await Run(g);
        Stopped(g);
        Assert.True(g.Flag("PumpFault"));
        Assert.True(g.Flag("RunRequested"));
        Assert.True(g.Get("Hour") > 6);
        Assert.True(g.Get("RoomTemp") < 18);
        Assert.Equal("PUMP FAULT - compressor interlocked", g.Values["AI_Task.Hvac.Status"]);
        g.Set("Scenario", 0);
        await Run(g);
        Assert.False(g.Flag("PumpFault"));
        Assert.True(g.Flag("PumpOn"));
    }

    [Fact]
    public async Task OpenWindow_IncreasesHeatLossForIdenticalRoomAndOutdoorDifference()
    {
        var g = new HvacGlobals();
        g.Write("AI_Task.Hvac.RunRequested", true); g.Set("Scenario", 2);
        await Run(g);
        Assert.Equal(5, g.Get("OutdoorTemp"));
        Assert.Equal(.65 * (18 - 5), g.Get("BuildingLossKw"), 9);
        Assert.True(g.Get("BuildingLossKw") > .18 * (18 - 5));
        Assert.Equal("Window open - increased heat loss", g.Values["AI_Task.Hvac.Status"]);
    }

    [Theory]
    [InlineData(0, 0, 18)]
    [InlineData(1, 1, 28)]
    public async Task ExtendedOperation_ReachesComfortCyclesThermostatAndPreservesFiniteLimits(int mode, int scenario, double room)
    {
        var g = new HvacGlobals();
        g.Set("Mode", mode); g.Set("Scenario", scenario); g.Set("RoomTemp", room);
        g.Write("AI_Task.Hvac.RunRequested", true);
        var active = false; var idle = false;
        for (var i = 0; i < 1441; i++)
        {
            await Run(g);
            Invariants(g);
            active |= g.Flag("Demand"); idle |= !g.Flag("Demand");
            Assert.True(g.Flag("RunRequested"), g.Values["AI_Task.Hvac.Status"].ToString());
        }
        Assert.InRange(g.Get("RoomTemp"), 20.3, 21.7);
        Assert.True(active && idle);
        Assert.Equal(6 + 1.0 / 60, g.Get("Hour"), 8);
        Assert.True(g.Get("EnergyKwh") > 0);
    }

    [Fact]
    public async Task Pause_FreezesStateAndResetRestoresAllControlsWithoutRestarting()
    {
        var g = new HvacGlobals(); g.Write("AI_Task.Hvac.RunRequested", true);
        await Run(g);
        var state = new[] { "RoomTemp", "BufferTemp", "Hour", "EnergyKwh" }.ToDictionary(n => n, g.Get);
        g.Write("AI_Task.Hvac.RunRequested", false);
        await Run(g);
        Stopped(g);
        foreach (var (name, value) in state) Assert.Equal(value, g.Get(name));
        g.Set("Scenario", 3); g.Set("Mode", 1); g.Set("Setpoint", 27); g.Set("ManualSpeed", 100);
        g.Write("AI_Task.Hvac.AutoControl", false); g.Write("AI_Task.Hvac.ResetRequested", true);
        await Run(g);
        Stopped(g);
        Assert.Equal(18, g.Get("RoomTemp")); Assert.Equal(30, g.Get("BufferTemp"));
        Assert.Equal(6, g.Get("Hour")); Assert.Equal(0, g.Get("EnergyKwh"));
        Assert.Equal(21, g.Get("Setpoint")); Assert.Equal(65, g.Get("ManualSpeed"));
        Assert.Equal(0, g.Get("Scenario")); Assert.Equal(0, g.Get("Mode"));
        Assert.True(g.Flag("AutoControl")); Assert.False(g.Flag("RunRequested")); Assert.False(g.Flag("ResetRequested"));
        Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.Hvac.Status"]);
    }

    [Theory]
    [InlineData("RoomTemp", double.NaN)]
    [InlineData("BufferTemp", double.PositiveInfinity)]
    [InlineData("BufferTemp", -11)]
    [InlineData("Hour", 24)]
    [InlineData("EnergyKwh", -1)]
    [InlineData("Setpoint", 15)]
    [InlineData("Setpoint", 29)]
    [InlineData("ManualSpeed", 0)]
    [InlineData("ManualSpeed", 101)]
    [InlineData("Mode", 2)]
    [InlineData("Scenario", 1.5)]
    [InlineData("Scenario", 4)]
    public async Task InvalidInputs_StopExplicitlyAndResetRecovers(string name, double value)
    {
        var g = new HvacGlobals(); g.Write("AI_Task.Hvac.RunRequested", true); g.Set(name, value);
        await Run(g);
        Stopped(g);
        Assert.False(g.Flag("RunRequested"));
        Assert.Equal("Invalid inputs - press Reset", g.Values["AI_Task.Hvac.Status"]);
        g.Write("AI_Task.Hvac.ResetRequested", true);
        await Run(g);
        Assert.Equal("Reset - press Start / Pause", g.Values["AI_Task.Hvac.Status"]);
        Assert.Equal(18, g.Get("RoomTemp")); Assert.Equal(30, g.Get("BufferTemp"));
        g.Write("AI_Task.Hvac.RunRequested", true);
        await Run(g);
        Assert.True(g.Flag("PumpOn"));
    }

    [Fact]
    public void CommonTask_LoadsExactEditableBundleWithoutImportingOrChangingStartup()
    {
        var task = AiTaskCatalog.Tasks.Single(t => t.Id == "screen-hvac-plant");
        Assert.Equal(HvacPlantTemplate.Json, task.ResourceBundle);
        Assert.Contains(HvacPlantTemplate.Json, task.Prompt);
        var editor = new NodeEditorService(); editor.NewFile();
        var component = new JsonEditor();
        typeof(JsonEditor).GetProperty(nameof(JsonEditor.ResourceProject))!.SetValue(component, editor.ActiveProject);
        typeof(JsonEditor).GetMethod("OnTaskChanged", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, [new ChangeEventArgs { Value = task.Id }]);
        typeof(JsonEditor).GetMethod("LoadTaskTemplate", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null);
        Assert.Equal(HvacPlantTemplate.Json, component.Session.ResourceJson);
        Assert.Same(editor.ActiveProject, component.Session.ResourceTarget);
        Assert.True(component.Session.ResourceOnly);
        Assert.Contains(task.Title, component.Session.AiComments);
        Assert.Empty(editor.ActiveProject!.Model.Screens);
    }

    private static void Stopped(HvacGlobals g)
    {
        foreach (var name in new[] { "PumpSpeed", "FlowM3h", "ThermalKw", "CoilKw", "Cop", "ElectricalKw" }) Assert.Equal(0, g.Get(name));
        foreach (var name in new[] { "PumpOn", "FanOn", "HeatingFlow", "CoolingFlow", "Demand" }) Assert.False(g.Flag(name));
    }

    private static void Invariants(HvacGlobals g)
    {
        foreach (var value in g.Values.Values.OfType<double>()) Assert.True(double.IsFinite(value));
        Assert.InRange(g.Get("ThermalKw"), -12, 12);
        Assert.InRange(g.Get("CoilKw"), -12, 12);
        Assert.InRange(g.Get("Cop"), 0, 5);
        Assert.InRange(g.Get("FlowM3h"), 0, 3);
        Assert.InRange(g.Get("PumpSpeed"), 0, 100);
        Assert.InRange(g.Get("ElectricalKw"), 0, 6.12);
        Assert.InRange(g.Get("RoomTemp"), -20, 60);
        Assert.InRange(g.Get("BufferTemp"), -10, 80);
        Assert.False(g.Flag("HeatingFlow") && g.Flag("CoolingFlow"));
    }

    public class HvacGlobals
    {
        public Dictionary<string, object> Values { get; } = Bundle.Folder.Folders.Single().Variables.ToDictionary(v => "AI_Task.Hvac." + v.Name,
            v => v.Type == "Boolean" ? (object)bool.Parse(v.InitialValue) : v.Type == "String" ? v.InitialValue : double.Parse(v.InitialValue, CultureInfo.InvariantCulture));
        public double ReadDouble(string path) => Convert.ToDouble(Values[path], CultureInfo.InvariantCulture);
        public bool ReadBool(string path) => Convert.ToBoolean(Values[path], CultureInfo.InvariantCulture);
        public void Write(string path, object value)
        {
            Assert.True(Values.ContainsKey(path), "Write to undeclared variable: " + path);
            Values[path] = value;
        }
        public void Set(string name, double value) => Write("AI_Task.Hvac." + name, value);
        public double Get(string name) => ReadDouble("AI_Task.Hvac." + name);
        public bool Flag(string name) => ReadBool("AI_Task.Hvac." + name);
    }
}
