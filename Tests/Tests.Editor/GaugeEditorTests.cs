using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Models;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class GaugeEditorTests
{
    [Fact]
    public async Task Properties_ExposePresetAndCustomAppearanceControls()
    {
        var symbol = new ScreenSymbol { Type = "gauge", GaugeStyle = "arc" };
        GaugeVisuals.ApplyPreset(symbol, "slate");
        var editor = new NodeEditorService();
        editor.SetScreenSelection(symbol, [symbol]);
        using var services = new ServiceCollection().AddLogging().AddSingleton(editor)
            .AddSingleton<OpcClientService>().AddSingleton<UndoRedoService>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<ScreenSymbolProperties>()).ToHtmlString());
        Assert.Contains("Midnight", html);
        Assert.Contains("Ivory", html);
        Assert.Contains("Brushed Metal", html);
        Assert.Contains("<option value=\"slate\" selected>", html);
        Assert.Contains("<option value=\"gradient\" selected>", html);
        Assert.Contains("Gradient end color", html);
        Assert.Contains("Arc thickness", html);
    }

    [Theory]
    [InlineData("needle")]
    [InlineData("arc")]
    [InlineData("semicircle")]
    public async Task Canvas_UsesSharedGradientRendererAndLiveReadout(string style)
    {
        var symbol = new ScreenSymbol { Type = "gauge", GaugeStyle = style, Width = 200, Height = 200, GaugeTicks = 0, GaugeUnit = "bar" };
        GaugeVisuals.ApplyPreset(symbol, "slate");
        var fragment = (RenderFragment)typeof(ScreenEditorPanel).GetMethod("RenderGauge", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(new ScreenEditorPanel(), [symbol, "#ef4444", "25"])!;
        using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<FragmentHost>(ParameterView.FromDictionary(new Dictionary<string, object?> { [nameof(FragmentHost.ChildContent)] = fragment }))).ToHtmlString());
        Assert.Contains("linearGradient", html);
        Assert.Contains("stop-color=\"#ef4444\"", html);
        Assert.Contains("stroke-dasharray=\"25 100\"", html);
        Assert.Contains("25.0 bar", html);
    }

    [Theory]
    [InlineData("needle")]
    [InlineData("arc")]
    [InlineData("semicircle")]
    [InlineData("hbar")]
    [InlineData("vbar")]
    [InlineData("thermometer")]
    public async Task ClassicCanvas_PreservesDrawingProportionsWhenEnlarged(string style)
    {
        using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        string? originalDrawing = null;
        foreach (var (width, height) in new[] { (80, 80), (800, 800), (1600, 400), (400, 1600) })
        {
            var symbol = new ScreenSymbol
            {
                Type = "gauge", GaugeStyle = style, Width = width, Height = height, GaugeTicks = 5, GaugeUnit = "bar"
            };
            var fragment = (RenderFragment)typeof(ScreenEditorPanel).GetMethod("RenderGauge", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(new ScreenEditorPanel(), [symbol, "#ef4444", "25"])!;
            var html = await renderer.Dispatcher.InvokeAsync(async () =>
                (await renderer.RenderComponentAsync<FragmentHost>(ParameterView.FromDictionary(new Dictionary<string, object?>
                {
                    [nameof(FragmentHost.ChildContent)] = fragment
                }))).ToHtmlString());
            var svg = XElement.Parse(html);
            Assert.Equal("classic-gauge", svg.Attribute("class")!.Value);
            Assert.Equal(width, double.Parse(svg.Attribute("width")!.Value, CultureInfo.InvariantCulture));
            Assert.Equal(height, double.Parse(svg.Attribute("height")!.Value, CultureInfo.InvariantCulture));
            Assert.Equal("0 0 80 80", svg.Attribute("viewBox")!.Value);
            Assert.Equal("xMidYMid meet", svg.Attribute("preserveAspectRatio")!.Value);
            Assert.Equal("visible", svg.Attribute("overflow")!.Value);
            var drawing = string.Concat(svg.Elements().Select(e => e.ToString(SaveOptions.DisableFormatting)));
            originalDrawing ??= drawing;
            Assert.Equal(originalDrawing, drawing);
            Assert.Contains("25.0 bar", drawing);
            Assert.DoesNotContain("linearGradient", drawing);
            Assert.Equal(width, symbol.Width);
            Assert.Equal(height, symbol.Height);
        }
    }

    [Theory]
    [InlineData("PasteSymbols")]
    [InlineData("DuplicateSelected")]
    public void CopyAndDuplicate_RetainAllGaugeProperties(string command)
    {
        var source = new ScreenSymbol
        {
            Id = "original", Type = "gauge", GaugeStyle = "semicircle", GaugeTicks = 7,
            GaugeShowValue = false, GaugeShowTickLabels = false, GaugeUnit = "bar", GaugeValueFormat = "F3",
            VariablePath = "Plant.Pressure", MinValue = -10, MaxValue = 300
        };
        GaugeVisuals.ApplyPreset(source, "metallic");
        source.GaugeArcThickness = 24;
        var screen = new ScreenConfig { Symbols = [source] };
        var panel = new ScreenEditorPanel();
        SetProperty(panel, "Node", new ScreenNode(screen));
        var clipboard = new ClipboardService();
        clipboard.CopySymbols([source]);
        SetProperty(panel, "Clipboard", clipboard);
        SetProperty(panel, "Editor", new NodeEditorService());
        SetProperty(panel, "UndoRedo", new UndoRedoService());
        typeof(ScreenEditorPanel).GetField("_selectedSymbols", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(panel, new HashSet<ScreenSymbol> { source });

        typeof(ScreenEditorPanel).GetMethod(command, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(panel, null);

        var copy = Assert.Single(screen.Symbols.Where(s => !ReferenceEquals(s, source)));
        Assert.NotEqual(source.Id, copy.Id);
        var expected = JsonSerializer.SerializeToElement(source);
        var actual = JsonSerializer.SerializeToElement(copy);
        foreach (var property in expected.EnumerateObject().Where(p => p.Name.StartsWith("Gauge", StringComparison.Ordinal) || p.Name is "Fill" or "VariablePath" or "MinValue" or "MaxValue"))
            Assert.Equal(property.Value.GetRawText(), actual.GetProperty(property.Name).GetRawText());
    }

    [Fact]
    public void PresetChange_IsOneUndoableActionEvenBeforeSelectionNotifications()
    {
        var symbol = new ScreenSymbol { Type = "gauge", Fill = "#abcdef", GaugeStyle = "arc" };
        var original = JsonSerializer.Serialize(symbol);
        var editor = new NodeEditorService();
        editor.SetScreenSelection(symbol, [symbol]);
        var undo = new UndoRedoService();
        using var panel = new ScreenSymbolProperties();
        SetProperty(panel, "Editor", editor);
        SetProperty(panel, "UndoRedo", undo);
        typeof(ScreenSymbolProperties).GetMethod("ApplyGaugePreset", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(panel, ["slate"]);
        var styled = JsonSerializer.Serialize(symbol);
        Assert.Equal(1, undo.UndoCount);
        Assert.Equal("slate", symbol.GaugeBackground);
        Assert.True(undo.Undo());
        Assert.Equal(original, JsonSerializer.Serialize(symbol));
        Assert.True(undo.Redo());
        Assert.Equal(styled, JsonSerializer.Serialize(symbol));
    }

    private static void SetProperty(object target, string name, object value) =>
        target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.SetValue(target, value);

    private sealed class FragmentHost : ComponentBase
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, ChildContent);
    }
}
