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

public class PumpEditorTests
{
    [Fact]
    public async Task Properties_ExposeSelectedPumpAppearanceAndShading()
    {
        var symbol = new ScreenSymbol { Type = "motorcontrol", MotorStyle = "pump", MotorAppearance = "isometric", MotorUseGradient = true };
        var editor = new NodeEditorService();
        editor.SetScreenSelection(symbol, [symbol]);
        using var services = Services(editor);
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<ScreenSymbolProperties>()).ToHtmlString());
        Assert.Contains("<option value=\"isometric\" selected>", html);
        Assert.Contains("<option value=\"centrifugal\">", html);
        Assert.Matches("<input[^>]*id=\"pumpGradient\"[^>]*checked", html);
        Assert.Contains("Pump body color", html);
    }

    [Theory]
    [InlineData("PasteSymbols")]
    [InlineData("DuplicateSelected")]
    public void CopyAndDuplicate_PreserveAllPumpSettings(string command)
    {
        var source = new ScreenSymbol
        {
            Id = "pump", Type = "motorcontrol", MotorStyle = "pump", MotorAppearance = "isometric", MotorUseGradient = true,
            MotorBodyColor = "#123456", MotorRunColor = "#abcdef", MotorStopColor = "#654321", MotorFaultColor = "#ff0000",
            MotorFaultPath = "Plant.Fault", MotorAnimateRotation = false, VariablePath = "Plant.Running"
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
        var copy = Assert.Single(screen.Symbols, s => !ReferenceEquals(s, source));
        var actual = JsonSerializer.SerializeToElement(copy);
        foreach (var property in JsonSerializer.SerializeToElement(source).EnumerateObject().Where(p => p.Name.StartsWith("Motor", StringComparison.Ordinal) || p.Name == "VariablePath"))
            Assert.Equal(property.Value.GetRawText(), actual.GetProperty(property.Name).GetRawText());
    }

    [Fact]
    public async Task ResponsiveEditor_RendersStyledPumpInsteadOfPlaceholder()
    {
        using var services = Services(new NodeEditorService());
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<ResponsiveScreenEditorPanel>(ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(ResponsiveScreenEditorPanel.Node)] = new ScreenNode(new ScreenConfig { LayoutMode = "responsive", Symbols = [new ScreenSymbol
            {
                Id = "pump", Type = "motorcontrol", MotorStyle = "pump", MotorAppearance = "isometric", MotorUseGradient = true
            }] })
        }))).ToHtmlString());
        Assert.Contains("class=\"hmi-pump\"", html);
        Assert.Contains("class=\"pump-depth\"", html);
        Assert.Contains("data-state=\"stopped\"", html);
    }

    private static ServiceProvider Services(NodeEditorService editor) => new ServiceCollection().AddLogging().AddSingleton(editor)
        .AddSingleton<OpcClientService>().AddSingleton<UndoRedoService>().BuildServiceProvider();
    private static void Set(object target, string name, object value) => target.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(target, value);
}
