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

public class TankRenderingTests
{
    [Theory]
    [InlineData("svg", "rect")]
    [InlineData("svg", "round")]
    [InlineData("svg", "cylinder")]
    [InlineData("svg", "isometric")]
    [InlineData("svg", "isometric-cone")]
    [InlineData("responsive", "rect")]
    [InlineData("responsive", "round")]
    [InlineData("responsive", "cylinder")]
    [InlineData("responsive", "isometric")]
    [InlineData("responsive", "isometric-cone")]
    public async Task BothLayouts_RenderConfiguredTankAndBoundFill(string layout, string style)
    {
        var symbol = new ScreenSymbol
        {
            Id = "tank", Type = "tank", TankStyle = style, Width = 180, Height = 240,
            TankUseGradient = true, TankFillStyle = "gradient", TankLiquidColor = "#0d9488",
            TankGradientEndColor = "#a7f3d0", MinValue = 10, MaxValue = 30, VariablePath = "Plant.Level"
        };
        var html = await RenderAsync(symbol, layout, "20");
        Assert.Contains("data-state=\"ready\" data-level=\"0.5\"", html);
        Assert.Contains(">50%</text>", html);
        Assert.Contains("class=\"tank-sheen\"", html);
        Assert.Contains("class=\"tank-liquid-front\"", html);
        Assert.Contains("stop-color=\"#a7f3d0\"", html);
        if (style == "isometric") Assert.Contains("class=\"tank-liquid-side\"", html);
        if (style == "cylinder") Assert.Contains("class=\"tank-rim\"", html);
        if (style == "isometric-cone")
        {
            Assert.Contains("class=\"tank-cone\"", html);
            Assert.Contains("class=\"tank-cone-seam\"", html);
            Assert.Contains("class=\"tank-rim\"", html);
        }
        if (layout == "responsive") Assert.Contains("width=\"100%\" height=\"100%\"", html);
        var empty = await RenderAsync(symbol, layout, "10");
        Assert.Contains(">0%</text>", empty);
        Assert.DoesNotContain("class=\"tank-liquid-front\"", empty);
        var full = await RenderAsync(symbol, layout, "1000");
        Assert.Contains(">100%</text>", full);
        Assert.Contains("data-state=\"ready\" data-level=\"1\"", full);
    }

    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task RuntimeMissingValue_IsNotReplacedWithEditorPreview(string layout)
    {
        var html = await RenderAsync(new ScreenSymbol { Id = "tank", Type = "tank", TankStyle = "isometric" }, layout, null);
        Assert.Contains("data-state=\"unavailable\"", html);
        Assert.Contains(">-</text>", html);
        Assert.DoesNotContain("class=\"tank-liquid\"", html);
        Assert.DoesNotContain("(preview)", html);
    }

    private static async Task<string> RenderAsync(ScreenSymbol symbol, string layout, string? value)
    {
        await using var services = new ServiceCollection().AddLogging().AddSingleton<OpcRuntimeClient>()
            .AddSingleton<CommandService>().AddSingleton<RuntimeAuthService>().AddSingleton<LocalizationService>()
            .AddSingleton<ProjectService>().AddSingleton<IJSRuntime, NoBrowserJs>().BuildServiceProvider();
        if (value != null) services.GetRequiredService<OpcRuntimeClient>().UpdateLocalValue("Plant.Level", value);
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<ScreenRenderer>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(ScreenRenderer.Screen)] = new ScreenConfig { LayoutMode = layout, Symbols = [symbol] }
            }))).ToHtmlString());
    }

    private sealed class NoBrowserJs : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => throw new InvalidOperationException("Unexpected browser call: " + identifier);
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
    }
}
