using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using RuntimeViewer.Shared.Components.Viewer;
using RuntimeViewer.Shared.Services;
using SharedModels;
using Xunit;

namespace Tests.RuntimeViewer;

public class PipeRenderingTests
{
    [Theory]
    [InlineData("svg", "horizontal")]
    [InlineData("svg", "vertical")]
    [InlineData("svg", "corner-tl")]
    [InlineData("svg", "corner-tr")]
    [InlineData("svg", "corner-bl")]
    [InlineData("svg", "corner-br")]
    [InlineData("responsive", "horizontal")]
    [InlineData("responsive", "vertical")]
    [InlineData("responsive", "corner-tl")]
    [InlineData("responsive", "corner-tr")]
    [InlineData("responsive", "corner-bl")]
    [InlineData("responsive", "corner-br")]
    public async Task BothLayouts_RenderShadingAndFlowAndHonorBoundState(string layout, string orientation)
    {
        var symbol = new ScreenSymbol
        {
            Id = "pipe", Type = "pipe", PipeOrientation = orientation,
            Width = 300, Height = 80, VariablePath = "Plant.Running", PipeUseGradient = true,
            PipeFlowSpeed = 0.5, PipeFlowDirection = "right-left"
        };
        await using var services = new ServiceCollection().AddLogging().AddSingleton<OpcRuntimeClient>()
            .AddSingleton<CommandService>().AddSingleton<RuntimeAuthService>().AddSingleton<LocalizationService>()
            .AddSingleton<ProjectService>().AddSingleton<IJSRuntime, NoBrowserJs>().BuildServiceProvider();
        var opc = services.GetRequiredService<OpcRuntimeClient>();
        var parameters = ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(ScreenRenderer.Screen)] = new ScreenConfig { LayoutMode = layout, Symbols = [symbol] }
        });
        opc.UpdateLocalValue("Plant.Running", "True");
        string active;
        await using (var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>()))
        {
            active = await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<ScreenRenderer>(parameters)).ToHtmlString());
        }
        Assert.Contains("class=\"pipe-surface\"", active);
        Assert.Contains("class=\"hmi-pipe-flow\"", active);
        Assert.Contains("hmiPipeFlow 0.5s", active);
        Assert.Contains("--pipe-cycle:-", active);
        if (layout == "responsive") Assert.Contains("width=\"100%\"", active);
        opc.UpdateLocalValue("Plant.Running", "0.0");
        await using var stoppedRenderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var stopped = await stoppedRenderer.Dispatcher.InvokeAsync(async () => (await stoppedRenderer.RenderComponentAsync<ScreenRenderer>(parameters)).ToHtmlString());
        Assert.Contains("class=\"pipe-body\"", stopped);
        Assert.DoesNotContain("class=\"hmi-pipe-flow\"", stopped);
    }

    private sealed class NoBrowserJs : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => throw new InvalidOperationException("Unexpected browser call: " + identifier);
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
    }
}
