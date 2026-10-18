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

public class PipeEditorTests
{
    [Theory]
    [InlineData("horizontal")]
    [InlineData("vertical")]
    [InlineData("corner-tl")]
    [InlineData("corner-tr")]
    [InlineData("corner-bl")]
    [InlineData("corner-br")]
    public async Task Properties_ShowReadableNamesAndSelectedOrientation(string orientation)
    {
        var symbol = new ScreenSymbol { Type = "pipe", Width = 200, Height = 70, PipeOrientation = orientation, PipeUseGradient = true };
        var editor = new NodeEditorService();
        editor.SetScreenSelection(symbol, [symbol]);
        using var services = new ServiceCollection().AddLogging().AddSingleton(editor)
            .AddSingleton<OpcClientService>().AddSingleton<UndoRedoService>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var root = await renderer.Dispatcher.InvokeAsync(() => renderer.RenderComponentAsync<ScreenSymbolProperties>());
        var html = await renderer.Dispatcher.InvokeAsync(root.ToHtmlString);
        Assert.Contains(">Pipe Settings</h6>", html);
        Assert.Contains(">Horizontal</option>", html);
        Assert.Contains(">Vertical</option>", html);
        Assert.Contains("Rounded gradient surface", html);
        Assert.Contains("Flow cycle duration (seconds)", html);
        Assert.Contains($"<option value=\"{orientation}\" selected>", html);
        Assert.DoesNotContain("ðŸ", html);
        Assert.DoesNotContain("â†", html);
    }

    [Theory]
    [InlineData("PasteSymbols")]
    [InlineData("DuplicateSelected")]
    public void CopyAndDuplicate_RetainAllPipeProperties(string command)
    {
        var source = new ScreenSymbol
        {
            Id = "source", Type = "pipe", Width = 350, Height = 110, PipeOrientation = "corner-br",
            PipeUseGradient = true, PipeColor = "#475569", PipeFlowColor = "#22d3ee",
            PipeFlowDirection = "bottom-top", PipeFlowSpeed = 0.3, PipeShowFlow = false,
            PipeAnimate = false, PipeThickness = 9, PipeFluidColor = "#abcdef", VariablePath = "Plant.Running"
        };
        var screen = new ScreenConfig { Symbols = [source] };
        var panel = new ScreenEditorPanel();
        Set(panel, "Node", new ScreenNode(screen));
        Set(panel, "Editor", new NodeEditorService());
        Set(panel, "UndoRedo", new UndoRedoService());
        var clipboard = new ClipboardService();
        clipboard.CopySymbols([source]);
        Set(panel, "Clipboard", clipboard);
        typeof(ScreenEditorPanel).GetField("_selectedSymbols", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(panel, new HashSet<ScreenSymbol> { source });
        typeof(ScreenEditorPanel).GetMethod(command, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(panel, null);
        var copy = Assert.Single(screen.Symbols, s => !ReferenceEquals(source, s));
        var original = JsonSerializer.SerializeToElement(source);
        var actual = JsonSerializer.SerializeToElement(copy);
        foreach (var property in original.EnumerateObject().Where(p => p.Name.StartsWith("Pipe", StringComparison.Ordinal) || p.Name is "Width" or "Height" or "VariablePath"))
            Assert.Equal(property.Value.GetRawText(), actual.GetProperty(property.Name).GetRawText());
    }

    [Fact]
    public async Task ResponsiveEditor_RendersAnimatedPipeInsteadOfPlaceholder()
    {
        using var services = new ServiceCollection().AddLogging().AddSingleton<NodeEditorService>()
            .AddSingleton<OpcClientService>().AddSingleton<UndoRedoService>().BuildServiceProvider();
        var screen = new ScreenConfig
        {
            LayoutMode = "responsive", Symbols = [new ScreenSymbol { Id = "pipe", Type = "pipe", PipeUseGradient = true, Width = 200, Height = 40 }]
        };
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<ResponsiveScreenEditorPanel>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(ResponsiveScreenEditorPanel.Node)] = new ScreenNode(screen)
            }))).ToHtmlString());
        Assert.Contains("class=\"pipe-surface\"", html);
        Assert.Contains("class=\"hmi-pipe-flow\"", html);
        Assert.Contains("hmiPipeFlow", html);
    }

    private static void Set(object target, string name, object value) => target.GetType()
        .GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(target, value);
}
