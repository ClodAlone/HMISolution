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

public class WidgetThemeRenderingTests
{
    [Theory]
    [InlineData("svg", "switch")]
    [InlineData("svg", "knob")]
    [InlineData("svg", "hslider")]
    [InlineData("svg", "rangeslider")]
    [InlineData("responsive", "switch")]
    [InlineData("responsive", "rotaryswitch")]
    [InlineData("responsive", "vslider")]
    public async Task ThemedControls_RenderModernSvgInEveryLayout(string layout, string type)
    {
        var symbol = new ScreenSymbol { Id = "ctl", Type = type, Width = 120, Height = 60 };
        WidgetThemes.ApplyPreset(symbol, "ivory");
        var html = await RenderAsync(new ScreenConfig { LayoutMode = layout, Symbols = [symbol] });
        Assert.Contains("class=\"wt-control wt-ivory\"", html);
        // Gradient ids are scoped per renderer instance and symbol so multiple screens never collide.
        Assert.Matches("id=\"wt_[0-9a-f]{8}_ctl_", html);
        if (layout == "responsive") Assert.Contains("wt-control-wrap", html);
    }

    [Theory]
    [InlineData("svg", "switch")]
    [InlineData("svg", "hslider")]
    [InlineData("responsive", "knob")]
    public async Task ClassicControls_KeepLegacyMarkup(string layout, string type)
    {
        var html = await RenderAsync(new ScreenConfig
        {
            LayoutMode = layout, Symbols = [new ScreenSymbol { Id = "ctl", Type = type, Width = 120, Height = 60 }]
        });
        Assert.DoesNotContain("wt-control", html);
        Assert.DoesNotContain("wt-themed", html);
    }

    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task ThemedTrend_RendersThemedPanelScopeAndVariables(string layout)
    {
        var symbol = new ScreenSymbol { Id = "trend", Type = "trend", Width = 400, Height = 240 };
        WidgetThemes.ApplyPreset(symbol, "midnight");
        var html = await RenderAsync(new ScreenConfig { LayoutMode = layout, Symbols = [symbol] });
        Assert.Contains("wt-themed wt-midnight", html);
        Assert.Contains("--wt-accent: #22d3ee;", html);
        Assert.Contains("wt-panel wt-trend", html);
        Assert.Contains("wt-toolbar", html);
    }

    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task ClassicTrend_HasNoThemeMarkup(string layout)
    {
        var html = await RenderAsync(new ScreenConfig
        {
            LayoutMode = layout, Symbols = [new ScreenSymbol { Id = "trend", Type = "trend", Width = 400, Height = 240 }]
        });
        Assert.DoesNotContain("wt-", html);
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
