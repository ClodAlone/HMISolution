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

public class GaugeRenderingTests
{
    [Theory]
    [InlineData("svg", "needle")]
    [InlineData("svg", "arc")]
    [InlineData("svg", "semicircle")]
    [InlineData("responsive", "needle")]
    [InlineData("responsive", "arc")]
    [InlineData("responsive", "semicircle")]
    public async Task StyledGauge_RendersGradientAndConfiguredRangeInEveryLayout(string layout, string style)
    {
        var symbol = new ScreenSymbol
        {
            Id = "gauge", Type = "gauge", GaugeStyle = style, Width = 200, Height = 200,
            GaugeTicks = 0, MinValue = -50, MaxValue = 150, GaugeUnit = "bar"
        };
        GaugeVisuals.ApplyPreset(symbol, "ivory");
        var html = await RenderAsync(new ScreenConfig { LayoutMode = layout, Symbols = [symbol] });
        Assert.Contains("linearGradient", html);
        Assert.Contains("radialGradient", html);
        Assert.Contains("stroke-dasharray=\"25 100\"", html);
        Assert.Contains("0.0 bar", html);
        Assert.DoesNotContain("rv-responsive-gauge-bar", html);
    }

    [Fact]
    public async Task ClassicResponsiveGauge_RetainsLegacyBar()
    {
        var html = await RenderAsync(new ScreenConfig
        {
            LayoutMode = "responsive", Symbols = [new ScreenSymbol { Id = "gauge", Type = "gauge" }]
        });
        Assert.Contains("rv-responsive-gauge-bar", html);
        Assert.DoesNotContain("linearGradient", html);
    }

    private static async Task<string> RenderAsync(ScreenConfig screen)
    {
        await using var services = new ServiceCollection().AddLogging()
            .AddSingleton<OpcRuntimeClient>().AddSingleton<CommandService>()
            .AddSingleton<RuntimeAuthService>().AddSingleton<LocalizationService>()
            .AddSingleton<ProjectService>().AddSingleton<IJSRuntime, NoBrowserJsRuntime>()
            .BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<ScreenRenderer>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(ScreenRenderer.Screen)] = screen
            }))).ToHtmlString());
    }

    private sealed class NoBrowserJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            throw new InvalidOperationException($"Unexpected JavaScript during static rendering: {identifier}");
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);
    }
}
