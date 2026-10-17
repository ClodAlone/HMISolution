using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Services;
using Xunit;

namespace Tests.Editor;

public class AiTaskCatalogTests
{
    public static TheoryData<string> TaskIds => new(AiTaskCatalog.Tasks.Select(task => task.Id));

    [Fact]
    public void Catalog_HasUniqueTasksAcrossSupportedResourceCategories()
    {
        Assert.Equal(AiTaskCatalog.Tasks.Count, AiTaskCatalog.Tasks.Select(task => task.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(AiTaskCatalog.Tasks.Count, AiTaskCatalog.Tasks.Select(task => task.Title).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(new[]
        {
            "Scripts", "Screens", "Variables", "Calculated Tags", "PLC Programs", "Recipes", "Schedulers", "Reports",
            "Assets", "Events", "Automation Rules", "Batch Sequences", "Users and Groups", "Localization", "Project Tasks"
        }, AiTaskCatalog.Tasks.Select(task => task.Category).Distinct());
    }

    [Theory]
    [MemberData(nameof(TaskIds))]
    public void SelectTask_PreparesEditableRequestInTheAppropriateModeWithoutGeneratingOrApplying(string id)
    {
        var editor = new JsonEditor();
        const string original = "{\"Folder\":{\"Name\":\"Root\",\"Folders\":[],\"Variables\":[]}}";
        typeof(JsonEditor).GetProperty(nameof(JsonEditor.JsonText))!.SetValue(editor, original);
        SetField(editor, "_scopePath", "folder:ExistingArea");
        SetField(editor, "_chunkedMode", true);
        SetField(editor, "_showSuggestions", true);
        var task = AiTaskCatalog.Tasks.Single(task => task.Id == id);

        SelectTask(editor, id);

        Assert.Equal(task.Prompt + "\n\n" + (task.CreatesResources ? AiTaskCatalog.ResourceInstructions : AiTaskCatalog.CommonInstructions), GetField<string>(editor, "_prompt"));
        Assert.Equal(task.CreatesResources, GetField<bool>(editor, "_resourceOnly"));
        Assert.Equal(id, GetField<string>(editor, "_selectedTaskId"));
        Assert.Equal("", GetField<string>(editor, "_scopePath"));
        Assert.False(GetField<bool>(editor, "_chunkedMode"));
        Assert.False(GetField<bool>(editor, "_isProcessing"));
        Assert.False(GetField<bool>(editor, "_showSuggestions"));
        Assert.Equal(original, editor.JsonText);
        Assert.Empty(GetField<List<string>>(editor, "_recentPrompts"));
    }

    [Fact]
    public void ReadWriteLoopTask_UsesRuntimeGlobalsFiniteLoopAndHostInterval()
    {
        var prompt = AiTaskCatalog.Tasks.Single(task => task.Id == "script-read-write-loop").Prompt;

        Assert.Contains("IntervalMs 1000", prompt);
        Assert.Contains("bounded for loop", prompt);
        Assert.Contains("ReadDouble", prompt);
        Assert.Contains("Write", prompt);
        Assert.Contains("Process the list once per invocation", prompt);
        Assert.Contains("DemoLoop.Input3", prompt);
        Assert.Contains("DemoLoop.Output3", prompt);
        Assert.Contains("disabled", AiTaskCatalog.CommonInstructions);
        Assert.Contains("Preserve existing resources", AiTaskCatalog.CommonInstructions);
    }

    [Fact]
    public void TwoTankTask_SpecifiesRealWidgetsMeetingGeometryAndBindings()
    {
        var prompt = AiTaskCatalog.Tasks.Single(task => task.Id == "screen-two-tanks").Prompt;

        Assert.Contains("two actual Type tank symbols", prompt);
        Assert.Contains("tank1 at X=100,Y=140,Width=160,Height=240", prompt);
        Assert.Contains("tank2 at X=700,Y=140,Width=160,Height=240", prompt);
        Assert.Contains("Type pipe at X=260,Y=310,Width=440,Height=20", prompt);
        Assert.Contains("TwoTanks.Tank1Level", prompt);
        Assert.Contains("TwoTanks.Tank2Level", prompt);
        Assert.Contains("TwoTanks.Flow", prompt);
        Assert.Contains("TankMin=0", prompt);
        Assert.Contains("TankMax=100", prompt);
    }

    [Fact]
    public void SelectTask_WhileGenerating_DoesNotChangeTheInFlightRequest()
    {
        var editor = new JsonEditor();
        SetField(editor, "_isProcessing", true);
        SetField(editor, "_prompt", "Existing request");
        SetField(editor, "_scopePath", "section:Scripts");

        SelectTask(editor, "screen-two-tanks");

        Assert.Equal("Existing request", GetField<string>(editor, "_prompt"));
        Assert.Equal("section:Scripts", GetField<string>(editor, "_scopePath"));
        Assert.Equal("", GetField<string>(editor, "_selectedTaskId"));
    }

    [Fact]
    public void SelectUnknownTask_ReportsErrorWithoutReplacingTheRequest()
    {
        var editor = new JsonEditor();
        SetField(editor, "_prompt", "Keep my request");

        SelectTask(editor, "missing-task");

        Assert.Equal("Keep my request", GetField<string>(editor, "_prompt"));
        Assert.Equal("The selected common task is not available. Choose another task.", GetField<string>(editor, "_aiError"));
    }

    [Fact]
    public async Task PlainEnter_DoesNotSendTheMultilineTask()
    {
        var editor = new JsonEditor();
        SelectTask(editor, "script-read-write-loop");
        var prompt = GetField<string>(editor, "_prompt");

        await (Task)typeof(JsonEditor).GetMethod("OnPromptKeyDown", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(editor, [new KeyboardEventArgs { Key = "Enter" }])!;

        Assert.Equal(prompt, GetField<string>(editor, "_prompt"));
        Assert.False(GetField<bool>(editor, "_isProcessing"));
        Assert.Empty(GetField<List<string>>(editor, "_recentPrompts"));
    }

    [Fact]
    public async Task ExpandedAssistant_RendersEveryTaskByCategoryAndEditablePrompt()
    {
        using var services = new ServiceCollection().AddLogging().AddSingleton<AiService>()
            .AddSingleton<IJSRuntime>(new NoBrowserJsRuntime()).BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());

        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var result = await renderer.RenderComponentAsync<ExpandedJsonEditor>();
            return result.ToHtmlString();
        });

        foreach (var task in AiTaskCatalog.Tasks)
            Assert.Contains($"value=\"{task.Id}\"", html);
        foreach (var category in AiTaskCatalog.Tasks.Select(task => task.Category).Distinct())
            Assert.Contains($"label=\"{category}\"", html);
        Assert.Contains("<textarea", html);
        Assert.Contains("AI task request", html);
        Assert.Contains("Task loaded, not sent", html);
        Assert.Contains("Resource-only generation selected", html);
        Assert.Contains("Generate new resources only", html);
        Assert.Contains("Ask AI", html);
        Assert.Contains("common prompts", html);
    }

    [Fact]
    public void ProviderPrompt_IncludesTaskCurrentProjectAndVerifiedResourceGuidance()
    {
        var service = new AiService { CompactJsonBeforeSend = false };
        const string current = "{\"Server\":{\"StartupScreen\":\"Existing\"},\"Screens\":[]}";
        var task = AiTaskCatalog.Tasks.Single(task => task.Id == "screen-two-tanks");

        var prompt = (string)typeof(AiService).GetMethod("BuildPrompt", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(service, [task.Prompt, current, null, null])!;

        Assert.Contains(current, prompt);
        Assert.Contains(task.Prompt, prompt);
        Assert.Contains("retain every existing property", prompt);
        Assert.Contains("ReadDouble(path)", prompt);
        Assert.Contains("host invokes Code cyclically at IntervalMs", prompt);
        Assert.Contains("TankLiquidColor", prompt);
        Assert.Contains("PipeFlowColor", prompt);
        Assert.Contains("NavButtonTarget", prompt);
        Assert.Contains("VariablePath", prompt);
        Assert.Contains("Return the FULL valid JSON", prompt);
    }

    private static void SelectTask(JsonEditor editor, string id)
        => typeof(JsonEditor).GetMethod("OnTaskChanged", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(editor, [new ChangeEventArgs { Value = id }]);

    private static void SetField(JsonEditor editor, string name, object value)
        => typeof(JsonEditor).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(editor, value);

    private static T GetField<T>(JsonEditor editor, string name)
        => (T)typeof(JsonEditor).GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(editor)!;

    public sealed class ExpandedJsonEditor : JsonEditor
    {
        public ExpandedJsonEditor()
        {
            SetField(this, "_aiExpanded", true);
            SelectTask(this, "screen-two-tanks");
        }
    }

    private sealed class NoBrowserJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, default, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            if (identifier == "localStorage.getItem") return ValueTask.FromResult(default(TValue)!);
            throw new InvalidOperationException($"Unexpected JavaScript call: {identifier}");
        }
    }
}
