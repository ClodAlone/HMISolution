using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
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

    [Theory]
    [InlineData("needle")]
    [InlineData("arc")]
    [InlineData("semicircle")]
    [InlineData("hbar")]
    [InlineData("vbar")]
    [InlineData("thermometer")]
    public async Task ClassicGauge_ResizesAsOneProportionalDrawing(string style)
    {
        string? originalDrawing = null;
        foreach (var (width, height) in new[] { (80, 80), (800, 800), (1600, 400), (400, 1600) })
        {
            var symbol = new ScreenSymbol
            {
                Id = "classic", Type = "gauge", GaugeStyle = style, Width = width, Height = height,
                GaugeTicks = 5, GaugeUnit = "bar"
            };
            var html = await RenderAsync(new ScreenConfig { LayoutMode = "svg", Symbols = [symbol] });
            var svg = XElement.Parse(Regex.Match(html, "<svg class=\"classic-gauge\"[\\s\\S]*?</svg>").Value);
            Assert.Equal(width, double.Parse(svg.Attribute("width")!.Value, CultureInfo.InvariantCulture));
            Assert.Equal(height, double.Parse(svg.Attribute("height")!.Value, CultureInfo.InvariantCulture));
            Assert.Equal("0 0 80 80", svg.Attribute("viewBox")!.Value);
            Assert.Equal("xMidYMid meet", svg.Attribute("preserveAspectRatio")!.Value);
            Assert.Equal("visible", svg.Attribute("overflow")!.Value);
            var drawing = string.Concat(svg.Elements().Select(e => e.ToString(SaveOptions.DisableFormatting)));
            originalDrawing ??= drawing;
            Assert.Equal(originalDrawing, drawing);
            Assert.Contains("0.0 bar", drawing);
            Assert.DoesNotContain("linearGradient", drawing);
            foreach (var circle in svg.Descendants("circle").Where(c => c.Attribute("stroke-dasharray") != null || ((string?)c.Attribute("style"))?.Contains("stroke-dasharray") == true))
            {
                var circumference = 2 * Math.PI * double.Parse(circle.Attribute("r")!.Value, CultureInfo.InvariantCulture);
                Assert.True(circumference < 999, "Dash gap must exceed the complete circle to avoid repeated progress segments.");
            }
            Assert.All(svg.Descendants("rect").Attributes("height"), a => Assert.True(double.Parse(a.Value, CultureInfo.InvariantCulture) >= 0));
        }
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
