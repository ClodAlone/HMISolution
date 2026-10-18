using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Services;
using SharedModels;
using Xunit;

namespace Tests.Editor;

public class ScreenSymbolPropertiesTests
{
    [Theory]
    [InlineData("nlquery", "NL Query")]
    [InlineData("rect", "Rectangle")]
    public async Task TypeDropdown_SelectsCurrentSymbolType(string type, string label)
    {
        var editor = new NodeEditorService();
        var symbol = new ScreenSymbol { Type = type };
        editor.SetScreenSelection(symbol, [symbol]);
        using var services = new ServiceCollection().AddLogging()
            .AddSingleton(editor)
            .AddSingleton<OpcClientService>()
            .AddSingleton<UndoRedoService>()
            .BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());

        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var result = await renderer.RenderComponentAsync<ScreenSymbolProperties>();
            return result.ToHtmlString();
        });

        Assert.Contains($"<option value=\"{type}\" selected>{label}</option>", html);
        Assert.Equal(type, symbol.Type);
    }
}
