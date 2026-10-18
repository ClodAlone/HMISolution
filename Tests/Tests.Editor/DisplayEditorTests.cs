using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Models;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class DisplayEditorTests
{
    [Theory]
    [InlineData("editbox")]
    [InlineData("numericdisplay")]
    public async Task Properties_ExposeSelectedAppearanceAndSizingOptions(string type)
    {
        var symbol = new ScreenSymbol { Type = type };
        DisplayVisuals.ApplyPreset(symbol, "lcd");
        var editor = new NodeEditorService();
        editor.SetScreenSelection(symbol, [symbol]);
        using var services = new ServiceCollection().AddLogging().AddSingleton(editor)
            .AddSingleton<OpcClientService>().AddSingleton<UndoRedoService>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<ScreenSymbolProperties>()).ToHtmlString());
        Assert.Contains("Display Appearance", html);
        Assert.Contains("<option value=\"lcd\" selected>", html);
        Assert.Contains("<option value=\"seven-segment\" selected>", html);
        Assert.Contains("Auto-fit value to width and height", html);
        Assert.Contains("id=\"displayAutoFit\" checked", html);
    }

    [Theory]
    [InlineData("PasteSymbols", "editbox")]
    [InlineData("DuplicateSelected", "editbox")]
    [InlineData("PasteSymbols", "numericdisplay")]
    [InlineData("DuplicateSelected", "numericdisplay")]
    public void CopyAndDuplicate_PreserveDisplaySettings(string command, string type)
    {
        var source = new ScreenSymbol
        {
            Id = "source", Type = type, FontSize = 37, FontFamily = "Consolas", FontWeight = "bold",
            NumericDisplayFormat = "F3", NumericDisplayUnit = "kWh", NumericDisplayBackground = "#123456",
            EditBoxFormat = "F2", EditBoxStep = 0.5, EditBoxShowStatistics = true,
            VariablePath = "Demo.Setpoint", MinValue = -10, MaxValue = 100
        };
        DisplayVisuals.ApplyPreset(source, "metallic");
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
        var copy = Assert.Single(screen.Symbols, s => !ReferenceEquals(s, source));
        var expected = JsonSerializer.SerializeToElement(source);
        var actual = JsonSerializer.SerializeToElement(copy);
        foreach (var property in expected.EnumerateObject().Where(p => p.Name.StartsWith("Display") || p.Name.StartsWith("Font") || p.Name.StartsWith("EditBox") || p.Name.StartsWith("NumericDisplay") || p.Name is "Fill" or "MinValue" or "MaxValue" or "VariablePath"))
            Assert.Equal(property.Value.GetRawText(), actual.GetProperty(property.Name).GetRawText());
    }

    [Fact]
    public void AppearancePreset_IsUndoableWithoutChangingBindingOrFormat()
    {
        var symbol = new ScreenSymbol { Type = "editbox", VariablePath = "Demo.Setpoint", EditBoxFormat = "F3", EditBoxStep = 0.25 };
        var before = JsonSerializer.Serialize(symbol);
        var editor = new NodeEditorService();
        editor.SetScreenSelection(symbol, [symbol]);
        var undo = new UndoRedoService();
        using var panel = new ScreenSymbolProperties();
        Set(panel, "Editor", editor);
        Set(panel, "UndoRedo", undo);
        typeof(ScreenSymbolProperties).GetMethod("ApplyDisplayPreset", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(panel, ["lcd"]);
        var after = JsonSerializer.Serialize(symbol);
        Assert.Equal("lcd", symbol.DisplayBackgroundStyle);
        Assert.Equal("Demo.Setpoint", symbol.VariablePath);
        Assert.Equal("F3", symbol.EditBoxFormat);
        Assert.Equal(1, undo.UndoCount);
        Assert.True(undo.Undo());
        Assert.Equal(before, JsonSerializer.Serialize(symbol));
        Assert.True(undo.Redo());
        Assert.Equal(after, JsonSerializer.Serialize(symbol));
    }

    private static void Set(object target, string property, object value) =>
        target.GetType().GetProperty(property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.SetValue(target, value);
}
