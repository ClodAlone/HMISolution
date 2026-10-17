using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Models;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class JsonEditorSessionTests
{
    [Fact]
    public async Task DockRecreation_PreservesRequestBundleStatusSettingsAndTargetWithoutReinitializing()
    {
        using var session = new JsonEditorSession();
        var js = new StorageJsRuntime();
        using var services = Services(js);
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var project = new ProjectNode(new NodeModel(), "Demo.json");
        SessionHost host = null!;
        var root = await renderer.Dispatcher.InvokeAsync(() => renderer.RenderComponentAsync<SessionHost>(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(SessionHost.Session)] = session,
            [nameof(SessionHost.Project)] = project,
            [nameof(SessionHost.Capture)] = (Action<SessionHost>)(value => host = value)
        })));
        var first = host.Editor!;
        await renderer.Dispatcher.InvokeAsync(async () =>
        {
            Set(first, "_aiExpanded", true);
            Set(first, "_prompt", "Create two tanks with my edited request");
            Set(first, "_resourceJson", "{\"Screens\":[{\"Name\":\"__TASK___EditedScreen\"}]}");
            Set(first, "_resourceTarget", project);
            Set(first, "_selectedTaskId", "screen-two-tanks");
            Set(first, "_selectedEngine", "Claude");
            Set(first, "_selectedSamplePath", "C:\\samples\\reference.json");
            Set(first, "_referencePreview", "Reference content");
            Set(first, "_showPreview", true);
            Set(first, "_ollamaModel", "custom-model");
            Set(first, "_ollamaUrl", "http://localhost:9999");
            Set(first, "_chunkChars", 12345);
            Set(first, "_jsonNeedsRefresh", true);
            Set(first, "_aiError", "Previous validation error");
            Set(first, "_aiComments", "Generated setup notes");
            first.ShowMessage(false, "Import failed: keep the preview");
            await host.ShowAsync(false);
            await host.ShowAsync(true);
        });

        Assert.NotSame(first, host.Editor);
        Assert.Same(session, host.Editor!.Session);
        Assert.Equal("Create two tanks with my edited request", Get<string>(host.Editor, "_prompt"));
        Assert.Equal("{\"Screens\":[{\"Name\":\"__TASK___EditedScreen\"}]}", Get<string>(host.Editor, "_resourceJson"));
        Assert.Same(project, Get<ProjectNode>(host.Editor, "_resourceTarget"));
        Assert.Equal("screen-two-tanks", session.SelectedTaskId);
        Assert.Equal("Claude", session.SelectedEngine);
        Assert.Equal("C:\\samples\\reference.json", session.SelectedSamplePath);
        Assert.Equal("http://localhost:9999", session.OllamaUrl);
        Assert.Equal("custom-model", session.OllamaModel);
        Assert.Equal(12345, session.ChunkChars);
        Assert.True(session.JsonNeedsRefresh);
        Assert.Equal(1, js.ReadCount);
        var html = await renderer.Dispatcher.InvokeAsync(root.ToHtmlString);
        Assert.Contains("Create two tanks with my edited request", html);
        Assert.Contains("EditedScreen", html);
        Assert.Contains("Previous validation error", html);
        Assert.Contains("Generated setup notes", html);
        Assert.Contains("Import failed: keep the preview", html);
        Assert.Contains("Reference content", html);
        Assert.Contains("Refresh from Tree before applying", html);
    }

    [Fact]
    public async Task RunningOperation_SurvivesPanelDisposal_UpdatesNewPanel_AndCanBeCancelledThere()
    {
        using var cancellation = new CancellationTokenSource();
        using var session = new JsonEditorSession { AiExpanded = true };
        using var services = Services(new StorageJsRuntime());
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        SessionHost host = null!;
        var root = await renderer.Dispatcher.InvokeAsync(() => renderer.RenderComponentAsync<SessionHost>(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(SessionHost.Session)] = session,
            [nameof(SessionHost.Capture)] = (Action<SessionHost>)(value => host = value)
        })));
        var original = host.Editor!;
        await renderer.Dispatcher.InvokeAsync(async () =>
        {
            Set(original, "_isProcessing", true);
            Set(original, "_streamCts", cancellation);
            await host.ShowAsync(false);
        });
        Assert.False(cancellation.IsCancellationRequested);
        Assert.True(session.IsProcessing);
        await renderer.Dispatcher.InvokeAsync(async () =>
        {
            Set(original, "_streamTail", "Progress while hidden");
            Set(original, "_streamCharCount", 500);
            await InvokeAsync(original, "RefreshSessionAsync");
            await host.ShowAsync(true);
        });
        Assert.NotSame(original, host.Editor);
        Assert.Same(cancellation, Get<CancellationTokenSource>(host.Editor!, "_streamCts"));
        var html = await renderer.Dispatcher.InvokeAsync(root.ToHtmlString);
        Assert.Contains("Progress while hidden", html);
        await renderer.Dispatcher.InvokeAsync(async () =>
        {
            Set(original, "_streamTail", "Progress after reopening");
            await InvokeAsync(original, "RefreshSessionAsync");
        });
        html = await renderer.Dispatcher.InvokeAsync(root.ToHtmlString);
        Assert.Contains("Progress after reopening", html);
        await renderer.Dispatcher.InvokeAsync(() => typeof(JsonEditor).GetMethod("CancelStreaming", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(host.Editor, null));
        Assert.True(cancellation.IsCancellationRequested);
        await renderer.Dispatcher.InvokeAsync(async () =>
        {
            Set(original, "_aiError", "Generation cancelled.");
            Set(original, "_isProcessing", false);
            await InvokeAsync(original, "RefreshSessionAsync");
        });
        html = await renderer.Dispatcher.InvokeAsync(root.ToHtmlString);
        Assert.Contains("Generation cancelled.", html);
        Assert.False(Get<bool>(host.Editor!, "_isProcessing"));
    }

    [Fact]
    public void WorkspaceDisposal_CancelsPendingGeneration_WithoutAffectingOtherSessions()
    {
        using var firstCancellation = new CancellationTokenSource();
        using var secondCancellation = new CancellationTokenSource();
        using var first = new JsonEditorSession { Prompt = "First draft" };
        using var second = new JsonEditorSession { Prompt = "Second draft" };
        first.StreamCts = firstCancellation;
        second.StreamCts = secondCancellation;

        first.Dispose();

        Assert.True(firstCancellation.IsCancellationRequested);
        Assert.False(secondCancellation.IsCancellationRequested);
        Assert.Equal("Second draft", second.Prompt);
    }

    private static ServiceProvider Services(IJSRuntime js)
        => new ServiceCollection().AddLogging().AddSingleton<AiService>().AddSingleton(js).BuildServiceProvider();

    private static void Set(JsonEditor component, string property, object value)
        => typeof(JsonEditor).GetProperty(property, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(component, value);

    private static T Get<T>(JsonEditor component, string property)
        => (T)typeof(JsonEditor).GetProperty(property, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(component)!;

    private static Task InvokeAsync(JsonEditor component, string method)
        => (Task)typeof(JsonEditor).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, null)!;

    public sealed class SessionHost : ComponentBase
    {
        [Parameter] public JsonEditorSession Session { get; set; } = default!;
        [Parameter] public ProjectNode? Project { get; set; }
        [Parameter] public Action<SessionHost> Capture { get; set; } = default!;
        public JsonEditor? Editor { get; private set; }
        private bool _visible = true;

        protected override void OnInitialized() => Capture(this);

        public Task ShowAsync(bool visible)
        {
            _visible = visible;
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!_visible) return;
            builder.OpenComponent<JsonEditor>(0);
            builder.AddAttribute(1, nameof(JsonEditor.Session), Session);
            builder.AddAttribute(2, nameof(JsonEditor.ResourceProject), Project);
            builder.AddAttribute(3, nameof(JsonEditor.JsonText), "{\"Screens\":[]}");
            builder.AddComponentReferenceCapture(4, component => Editor = (JsonEditor)component);
            builder.CloseComponent();
        }
    }

    private sealed class StorageJsRuntime : IJSRuntime
    {
        public int ReadCount { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, default, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            if (identifier == "localStorage.getItem")
            {
                ReadCount++;
                return ValueTask.FromResult(default(TValue)!);
            }
            throw new InvalidOperationException($"Unexpected JavaScript call: {identifier}");
        }
    }
}
