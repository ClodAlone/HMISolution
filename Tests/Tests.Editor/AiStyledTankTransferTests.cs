using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Models;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class AiStyledTankTransferTests
{
    private const string TaskId = "screen-styled-tank-transfer";

    [Fact]
    public void Task_IncludesExactBundleAndLoadsPreviewWithoutImportingOrCallingProvider()
    {
        var task = AiTaskCatalog.Tasks.Single(t => t.Id == TaskId);
        Assert.Equal("Screens", task.Category);
        Assert.True(task.CreatesResources);
        Assert.Equal(StyledTankTransferTemplate.Json, task.ResourceBundle);
        Assert.Contains(StyledTankTransferTemplate.Json, task.Prompt);
        var editor = NewEditor();
        var project = editor.ActiveProject!;
        var component = new JsonEditor();
        typeof(JsonEditor).GetProperty(nameof(JsonEditor.ResourceProject))!.SetValue(component, project);
        Invoke(component, "OnTaskChanged", new ChangeEventArgs { Value = TaskId });
        Invoke(component, "LoadTaskTemplate");

        Assert.Equal(StyledTankTransferTemplate.Json, component.Session.ResourceJson);
        Assert.Same(project, component.Session.ResourceTarget);
        Assert.True(component.Session.ResourceOnly);
        Assert.False(component.Session.IsProcessing);
        Assert.Empty(component.Session.AiError);
        Assert.Contains("enable", component.Session.AiComments);
        Assert.Empty(project.Model.Screens);
        Assert.Empty(project.Model.Scripts);
        Assert.Empty(project.Model.Folder.Folders);
        Assert.Empty(component.Session.RecentPrompts);
    }

    [Fact]
    public async Task LoadedTemplate_RendersExactCommandAndEditablePreviewAndUsesExistingImportCallback()
    {
        var project = NewEditor().ActiveProject!;
        var component = new JsonEditor();
        typeof(JsonEditor).GetProperty(nameof(JsonEditor.ResourceProject))!.SetValue(component, project);
        Invoke(component, "OnTaskChanged", new ChangeEventArgs { Value = TaskId });
        Invoke(component, "LoadTaskTemplate");
        component.Session.AiExpanded = true;
        component.Session.IsInitialized = true;
        using var services = new ServiceCollection().AddLogging().AddSingleton<AiService>()
            .AddSingleton<IJSRuntime, NoJs>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<JsonEditor>(
            ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(JsonEditor.ResourceProject)] = project,
                [nameof(JsonEditor.Session)] = component.Session
            }))).ToHtmlString());
        Assert.Contains("Load exact sample resources", html);
        Assert.Contains("Styled tank transfer - exact sample with simulation", html);
        Assert.Contains("id=\"json-ai-resource-preview\"", html);
        Assert.Contains("isometric-cone", html);
        Assert.Contains("Add resources to", html);
        Assert.Contains("Enable the simulation script after review", html);

        AiResourceImportRequest? imported = null;
        typeof(JsonEditor).GetProperty(nameof(JsonEditor.OnImportResources))!.SetValue(component,
            EventCallback.Factory.Create<AiResourceImportRequest>(this, request => imported = request));
        await (Task)typeof(JsonEditor).GetMethod("ImportResources", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null)!;
        Assert.NotNull(imported);
        Assert.Same(project, imported.Project);
        Assert.Equal(StyledTankTransferTemplate.Json, imported.Json);
        Assert.Empty(project.Model.Screens);
    }

    [Fact]
    public void LoadTemplate_RejectsMissingOrLockedTargetAndDoesNotChangeInFlightGeneration()
    {
        var component = new JsonEditor();
        Invoke(component, "OnTaskChanged", new ChangeEventArgs { Value = TaskId });
        Invoke(component, "LoadTaskTemplate");
        Assert.Contains("unlock", component.Session.AiError);
        Assert.Empty(component.Session.ResourceJson);
        var project = NewEditor().ActiveProject!;
        typeof(JsonEditor).GetProperty(nameof(JsonEditor.ResourceProject))!.SetValue(component, project);
        project.IsLocked = true;
        Invoke(component, "LoadTaskTemplate");
        Assert.Empty(component.Session.ResourceJson);
        project.IsLocked = false;
        component.Session.IsProcessing = true;
        component.Session.ResourceJson = "In-flight preview";
        Invoke(component, "LoadTaskTemplate");
        Assert.Equal("In-flight preview", component.Session.ResourceJson);
        Assert.Null(component.Session.ResourceTarget);
    }

    [Fact]
    public void ImportTwice_IsolatesAllBindingsAndScriptPathsAndSurvivesTreeRefreshAndSerialization()
    {
        var editor = NewEditor();
        var project = editor.ActiveProject!;
        var settings = project.Model.Server;
        settings.StartupScreen = "Existing screen";
        for (var i = 0; i < 2; i++)
        {
            var result = editor.ImportAiResources(project, StyledTankTransferTemplate.Json);
            Assert.True(result.success, result.message);
        }
        Assert.Same(settings, project.Model.Server);
        Assert.Equal("Existing screen", settings.StartupScreen);
        editor.RefreshJsonFromTree();
        var model = JsonSerializer.Deserialize<NodeModel>(editor.JsonEditorText)!;
        Assert.Equal(2, model.Screens.Count);
        Assert.Equal(2, model.Scripts.Count);
        for (var i = 0; i < 2; i++)
        {
            var prefix = i == 0 ? "AI_Task" : "AI_Task_2";
            var screen = model.Screens[i];
            var script = model.Scripts[i];
            var folder = model.Folder.Folders[i];
            Assert.Equal(prefix, folder.Name);
            Assert.Equal(prefix, screen.Group);
            Assert.Equal(prefix + "_Tank Transfer", screen.Name);
            Assert.Equal(prefix + "_TankTransferSimulation", script.Name);
            Assert.False(script.Enabled);
            Assert.Equal(250, script.IntervalMs);
            Assert.Contains($"const string p = \"{prefix}.Transfer.\";", script.Code);
            Assert.DoesNotContain("__TASK__", script.Code);
            Assert.Empty(screen.SwipeLeftScreen);
            Assert.Empty(screen.SwipeRightScreen);
            var transfer = Assert.Single(folder.Folders);
            Assert.Equal("Transfer", transfer.Name);
            Assert.Equal(new[] { "SourceLevel", "DestinationLevel", "RunRequested", "RefillRequested", "PumpOn", "Status" }, transfer.Variables.Select(v => v.Name));
            Assert.Equal("100", transfer.Variables[0].InitialValue);
            Assert.Equal("0", transfer.Variables[1].InitialValue);
            Assert.All(screen.Symbols.Where(s => !string.IsNullOrEmpty(s.VariablePath)), s => Assert.StartsWith(prefix + ".Transfer.", s.VariablePath));
            Assert.Equal(16, screen.Symbols.Count);
            var tanks = screen.Symbols.Where(s => s.Type == "tank").ToArray();
            Assert.Equal(2, tanks.Length);
            Assert.All(tanks, t =>
            {
                Assert.Equal("isometric-cone", t.TankStyle);
                Assert.True(t.TankUseGradient);
                Assert.Equal("gradient", t.TankFillStyle);
                Assert.Equal(240, t.Width);
                Assert.Equal(320, t.Height);
                Assert.Equal(180, t.Y);
            });
            Assert.Equal(new[] { 100d, 940d }, tanks.Select(t => t.X));
            var pipes = screen.Symbols.Where(s => s.Type == "pipe").ToArray();
            Assert.Equal(2, pipes.Length);
            Assert.All(pipes, p =>
            {
                Assert.True(screen.Symbols.IndexOf(p) < screen.Symbols.IndexOf(tanks[0]));
                Assert.Equal(prefix + ".Transfer.PumpOn", p.VariablePath);
                Assert.True(p.PipeUseGradient && p.PipeAnimate && p.PipeShowFlow);
            });
            var pump = Assert.Single(screen.Symbols, s => s.Type == "motorcontrol");
            Assert.Equal("pump", pump.MotorStyle);
            Assert.Equal("isometric", pump.MotorAppearance);
            Assert.Equal(prefix + ".Transfer.PumpOn", pump.VariablePath);
            var buttons = screen.Symbols.Where(s => s.Type == "button").ToArray();
            Assert.Equal(2, buttons.Length);
            var toggle = Assert.Single(buttons[0].Commands);
            Assert.Equal("ToggleVariable", toggle.Action);
            Assert.Equal(prefix + ".Transfer.RunRequested", toggle.VariablePath);
            var refill = Assert.Single(buttons[1].Commands);
            Assert.Equal("WriteVariable", refill.Action);
            Assert.Equal(prefix + ".Transfer.RefillRequested", refill.VariablePath);
            Assert.Equal("true", refill.Value);
        }
    }

    [Fact]
    public async Task ImportedSimulation_CompilesAndTransfersPausesRefillsCompletesAndRejectsInvalidLevels()
    {
        var model = AiResourceBundle.Parse(StyledTankTransferTemplate.Json, "AI_Task");
        var script = Assert.Single(model.Scripts);
        var options = ScriptOptions.Default.AddImports("System");
        var run = CSharpScript.Create(script.Code, options, typeof(TransferGlobals)).CreateDelegate();
        var globals = new TransferGlobals();
        await run(globals);
        Assert.False(globals.ReadBool("AI_Task.Transfer.PumpOn"));
        Assert.Equal(100, globals.ReadDouble("AI_Task.Transfer.SourceLevel"));
        globals.Write("AI_Task.Transfer.RunRequested", true);
        await run(globals);
        Assert.Equal(99, globals.ReadDouble("AI_Task.Transfer.SourceLevel"));
        Assert.Equal(1, globals.ReadDouble("AI_Task.Transfer.DestinationLevel"));
        Assert.True(globals.ReadBool("AI_Task.Transfer.PumpOn"));
        globals.Write("AI_Task.Transfer.RunRequested", false);
        await run(globals);
        Assert.Equal(99, globals.ReadDouble("AI_Task.Transfer.SourceLevel"));
        Assert.False(globals.ReadBool("AI_Task.Transfer.PumpOn"));
        globals.Write("AI_Task.Transfer.RefillRequested", true);
        await run(globals);
        Assert.Equal(100, globals.ReadDouble("AI_Task.Transfer.SourceLevel"));
        Assert.Equal(0, globals.ReadDouble("AI_Task.Transfer.DestinationLevel"));
        Assert.True(globals.ReadBool("AI_Task.Transfer.RunRequested"));
        Assert.False(globals.ReadBool("AI_Task.Transfer.RefillRequested"));
        Assert.False(globals.ReadBool("AI_Task.Transfer.PumpOn"));
        for (var cycle = 1; cycle <= 100; cycle++)
        {
            await run(globals);
            Assert.Equal(100 - cycle, globals.ReadDouble("AI_Task.Transfer.SourceLevel"));
            Assert.Equal(cycle, globals.ReadDouble("AI_Task.Transfer.DestinationLevel"));
            Assert.Equal(cycle < 100, globals.ReadBool("AI_Task.Transfer.PumpOn"));
        }
        Assert.False(globals.ReadBool("AI_Task.Transfer.RunRequested"));
        globals.Write("AI_Task.Transfer.RunRequested", true);
        await run(globals);
        Assert.False(globals.ReadBool("AI_Task.Transfer.PumpOn"));
        foreach (var invalid in new[] { double.NaN, -1, 101, 20 })
        {
            globals.Write("AI_Task.Transfer.SourceLevel", invalid);
            globals.Write("AI_Task.Transfer.RunRequested", true);
            await run(globals);
            Assert.False(globals.ReadBool("AI_Task.Transfer.PumpOn"));
            Assert.False(globals.ReadBool("AI_Task.Transfer.RunRequested"));
            Assert.Equal("Invalid levels - press Refill & Restart", globals.Values["AI_Task.Transfer.Status"]);
        }
    }

    public class TransferGlobals
    {
        public Dictionary<string, object> Values { get; } = new()
        {
            ["AI_Task.Transfer.SourceLevel"] = 100d, ["AI_Task.Transfer.DestinationLevel"] = 0d,
            ["AI_Task.Transfer.RunRequested"] = false, ["AI_Task.Transfer.RefillRequested"] = false,
            ["AI_Task.Transfer.PumpOn"] = false, ["AI_Task.Transfer.Status"] = "Ready - press Start / Stop"
        };
        public double ReadDouble(string path) => Convert.ToDouble(Values[path]);
        public bool ReadBool(string path) => Convert.ToBoolean(Values[path]);
        public void Write(string path, object value)
        {
            Assert.True(Values.ContainsKey(path), "Script wrote an undeclared variable: " + path);
            Values[path] = value;
        }
    }

    private static NodeEditorService NewEditor()
    {
        var editor = new NodeEditorService();
        editor.NewFile();
        return editor;
    }

    private static void Invoke(JsonEditor editor, string name, params object[] args)
        => typeof(JsonEditor).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(editor, args);

    private sealed class NoJs : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => throw new InvalidOperationException("Unexpected browser call: " + identifier);
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => InvokeAsync<TValue>(identifier, args);
    }
}
