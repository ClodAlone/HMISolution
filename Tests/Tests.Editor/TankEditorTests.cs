using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Models;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class TankEditorTests
{
    [Theory]
    [InlineData("rectangular", "rect")]
    [InlineData("rect", "rect")]
    [InlineData("round", "round")]
    [InlineData("cylinder", "cylinder")]
    [InlineData("isometric", "isometric")]
    [InlineData("isometric-cone", "isometric-cone")]
    public async Task Properties_SelectSavedStyleAndExposeShadingAndLiquidControls(string saved, string selected)
    {
        var symbol = new ScreenSymbol { Type = "tank", TankStyle = saved, TankUseGradient = true, TankFillStyle = "gradient" };
        var editor = new NodeEditorService();
        editor.SetScreenSelection(symbol, [symbol]);
        using var services = Services(editor);
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<ScreenSymbolProperties>()).ToHtmlString());
        Assert.Contains($"<option value=\"{selected}\" selected>", html);
        Assert.Contains("<option value=\"isometric\"", html);
        Assert.Matches("<input[^>]*id=\"tankGradient\"[^>]*checked", html);
        Assert.Contains("<option value=\"gradient\" selected>", html);
        Assert.Contains("Liquid gradient surface color", html);
        Assert.Contains("Preview Filling", html);
        Assert.Equal(saved, symbol.TankStyle);
    }

    [Theory]
    [InlineData("PasteSymbols", "isometric")]
    [InlineData("DuplicateSelected", "isometric")]
    [InlineData("PasteSymbols", "isometric-cone")]
    [InlineData("DuplicateSelected", "isometric-cone")]
    public void CopyAndDuplicate_PreserveAllTankAppearanceAndRangeSettings(string command, string style)
    {
        var source = new ScreenSymbol
        {
            Id = "source", Type = "tank", Width = 210, Height = 270, TankStyle = style, TankUseGradient = true,
            TankFillStyle = "gradient", TankLiquidColor = "#0d9488", TankGradientEndColor = "#a7f3d0", TankBodyColor = "#64748b",
            TankFillColor = "#123456", TankMin = -20, TankMax = 300, TankUnit = "litres", TankShowLevel = false,
            MinValue = 10, MaxValue = 200, VariablePath = "Plant.Level"
        };
        var screen = new ScreenConfig { Symbols = [source] };
        var panel = new ScreenEditorPanel();
        Set(panel, "Node", new ScreenNode(screen));
        Set(panel, "Editor", new NodeEditorService());
        Set(panel, "UndoRedo", new UndoRedoService());
        var clipboard = new ClipboardService();
        clipboard.CopySymbols([source]);
        Set(panel, "Clipboard", clipboard);
        typeof(ScreenEditorPanel).GetField("_selectedSymbols", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(panel, new HashSet<ScreenSymbol> { source });
        typeof(ScreenEditorPanel).GetMethod(command, BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(panel, null);
        var copy = Assert.Single(screen.Symbols, s => !ReferenceEquals(source, s));
        var expected = JsonSerializer.SerializeToElement(source);
        var actual = JsonSerializer.SerializeToElement(copy);
        foreach (var property in expected.EnumerateObject().Where(p => p.Name.StartsWith("Tank", StringComparison.Ordinal) || p.Name is "MinValue" or "MaxValue" or "VariablePath" or "Width" or "Height"))
            Assert.Equal(property.Value.GetRawText(), actual.GetProperty(property.Name).GetRawText());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task FillPreview_IsSimulatedAndDoesNotModifyBindingOrSavedProperties(double percent)
    {
        var symbol = new ScreenSymbol { Type = "tank", TankStyle = "isometric", TankUseGradient = true, VariablePath = "Plant.Level" };
        var before = JsonSerializer.Serialize(symbol);
        var editor = new NodeEditorService();
        editor.SetScreenSelection(symbol, [symbol]);
        using var services = Services(editor);
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<PreviewProperties>(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(PreviewProperties.Percent)] = percent
        }))).ToHtmlString());
        Assert.Contains("(preview)", html);
        Assert.Contains($">{percent:0}%</text>", html);
        Assert.Contains("does not write to the bound variable", html);
        Assert.Equal(percent > 0, html.Contains("class=\"tank-liquid-side\"", StringComparison.Ordinal));
        Assert.Equal(before, JsonSerializer.Serialize(symbol));
    }

    [Theory]
    [InlineData("isometric")]
    [InlineData("cylinder")]
    [InlineData("isometric-cone")]
    public async Task ResponsiveEditor_RendersRealTankAndExplicitUnboundPreview(string style)
    {
        using var services = Services(new NodeEditorService());
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<ResponsiveScreenEditorPanel>(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(ResponsiveScreenEditorPanel.Node)] = new ScreenNode(new ScreenConfig
            {
                LayoutMode = "responsive", Symbols = [new ScreenSymbol { Type = "tank", TankStyle = style, TankUseGradient = true }]
            })
        }))).ToHtmlString());
        Assert.Contains("class=\"hmi-tank\"", html);
        Assert.Contains("class=\"tank-sheen\"", html);
        Assert.Contains(">65%</text>", html);
        Assert.Contains("(preview)", html);
        if (style == "isometric") Assert.Contains("class=\"tank-liquid-side\"", html);
        else Assert.Contains("class=\"tank-rim\"", html);
    }

    private static ServiceProvider Services(NodeEditorService editor) => new ServiceCollection().AddLogging().AddSingleton(editor)
        .AddSingleton<OpcClientService>().AddSingleton<UndoRedoService>().BuildServiceProvider();

    private static void Set(object target, string name, object value) => target.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(target, value);

    public class PreviewProperties : ScreenSymbolProperties
    {
        [Parameter] public double Percent { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            typeof(ScreenSymbolProperties).GetField("_widgetPreviewActive", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(this, true);
            typeof(ScreenSymbolProperties).GetField("_tankPreviewLevel", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(this, Percent);
        }
    }
}
