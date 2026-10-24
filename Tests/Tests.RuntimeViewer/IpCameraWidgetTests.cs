using System.Reflection;
using Microsoft.JSInterop;
using RuntimeViewer.Shared.Components.Viewer;
using Xunit;

namespace Tests.RuntimeViewer;

public class IpCameraWidgetTests
{
    [Fact]
    public async Task ParentRerender_PreservesErrorUntilRetryAndKeepsRetryUrlStable()
    {
        await using var widget = CreateWidget();
        widget.ApplyParameters();
        var originalUrl = Field<string>(widget, "_streamUrl");
        Assert.StartsWith("http://localhost:8088/camera/entrance/stream?client=", originalUrl);
        Invoke(widget, "OnImageError");
        widget.ApplyParameters();
        Assert.True(Field<bool>(widget, "_hasError"));
        Assert.Equal(originalUrl, Field<string>(widget, "_streamUrl"));

        Invoke(widget, "Retry");
        var retryUrl = Field<string>(widget, "_streamUrl");
        Assert.NotEqual(originalUrl, retryUrl);
        Assert.EndsWith("&v=1", retryUrl);
        Assert.False(Field<bool>(widget, "_hasError"));
        widget.ApplyParameters();
        Assert.Equal(retryUrl, Field<string>(widget, "_streamUrl"));
    }

    [Fact]
    public async Task Render_StartsStreamAfterDomExistsAndReusesModule()
    {
        var module = new CameraModule();
        var runtime = new CameraRuntime(Task.FromResult<IJSObjectReference>(module));
        var widget = CreateWidget(runtime);
        widget.ApplyParameters();
        Assert.Empty(module.Urls);
        await widget.RenderAsync();
        await widget.RenderAsync();
        Assert.Equal(1, runtime.Imports);
        Assert.Equal([Field<string>(widget, "_streamUrl"), Field<string>(widget, "_streamUrl")], module.Urls);
        await widget.DisposeAsync();
        Assert.True(module.Disposed);
        await widget.RenderAsync();
        Assert.Equal(2, module.Urls.Count);
    }

    [Fact]
    public async Task DisposalWhileModuleLoads_DoesNotStartAnOrphanStream()
    {
        var pending = new TaskCompletionSource<IJSObjectReference>(TaskCreationOptions.RunContinuationsAsynchronously);
        var module = new CameraModule();
        var widget = CreateWidget(new CameraRuntime(pending.Task));
        widget.ApplyParameters();
        var render = widget.RenderAsync();
        var disposal = widget.DisposeAsync();
        pending.SetResult(module);
        await render;
        await disposal;
        Assert.Empty(module.Urls);
        Assert.True(module.Disposed);
    }

    private static TestCamera CreateWidget(IJSRuntime? runtime = null)
    {
        var widget = new TestCamera();
        typeof(IpCameraWidget).GetProperty(nameof(IpCameraWidget.CameraId))!.SetValue(widget, "entrance");
        typeof(IpCameraWidget).GetProperty(nameof(IpCameraWidget.ServerBaseUrl))!.SetValue(widget, "http://localhost:8088/");
        if (runtime != null)
            typeof(IpCameraWidget).GetProperty("JS", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(widget, runtime);
        return widget;
    }

    private static T Field<T>(IpCameraWidget widget, string name) =>
        (T)typeof(IpCameraWidget).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(widget)!;

    private static void Invoke(IpCameraWidget widget, string name) =>
        typeof(IpCameraWidget).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(widget, null);

    private sealed class TestCamera : IpCameraWidget
    {
        public void ApplyParameters() => base.OnParametersSet();
        public Task RenderAsync() => base.OnAfterRenderAsync(false);
    }

    private sealed class CameraRuntime(Task<IJSObjectReference> module) : IJSRuntime
    {
        public int Imports { get; private set; }
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            InvokeAsync<TValue>(identifier, CancellationToken.None, args);
        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            Assert.Equal("import", identifier);
            Assert.Equal("./_content/RuntimeViewer.Shared/ipcamera.js", args![0]);
            Imports++;
            return (TValue)await module;
        }
    }

    private sealed class CameraModule : IJSObjectReference
    {
        public List<string> Urls { get; } = [];
        public bool Disposed { get; private set; }
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            InvokeAsync<TValue>(identifier, CancellationToken.None, args);
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            Assert.Equal("start", identifier);
            Assert.False(Disposed);
            Urls.Add(Assert.IsType<string>(args![1]));
            return ValueTask.FromResult(default(TValue)!);
        }
        public ValueTask DisposeAsync()
        {
            Disposed = true;
            return ValueTask.CompletedTask;
        }
    }
}
