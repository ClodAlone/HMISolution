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
    [InlineData("historicaltrend", "Historical Trend")]
    [InlineData("historicaltable", "Historical Table")]
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

    [Theory]
    [InlineData("historicaltrend")]
    [InlineData("historicaltable")]
    public async Task HistoricalSettings_ShowSamplingAndSyncWithoutLegacyPointLimit(string type)
    {
        var editor = new NodeEditorService();
        var symbol = new ScreenSymbol { Type = type, HistoricalSamplingIntervalSeconds = 15, HistoricalSyncGroup = "mixer-history" };
        editor.SetScreenSelection(symbol, [symbol]);
        using var services = new ServiceCollection().AddLogging().AddSingleton(editor)
            .AddSingleton<OpcClientService>().AddSingleton<UndoRedoService>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<ScreenSymbolProperties>()).ToHtmlString());
        Assert.Contains("Sampling interval (seconds)", html);
        Assert.Contains("Range / cursor sync group", html);
        Assert.Contains("value=\"mixer-history\"", html);
        Assert.Contains("value=\"15\"", html);
        Assert.DoesNotContain("Max Points", html);
        Assert.Contains("Widget Theme", html);
        Assert.Equal(15, symbol.HistoricalSamplingIntervalSeconds);
        Assert.Equal("mixer-history", symbol.HistoricalSyncGroup);
    }

    [Fact]
    public async Task ReportCommand_DisplaysSelectedActionAndReportPickerWithoutChangingConfiguration()
    {
        var editor = new NodeEditorService();
        editor.NewFileFromModel(new NodeModel { Reports = [new ReportConfig { Name = "DailyOperations" }] });
        var command = new SymbolCommand { Action = "GenerateReport", TargetReport = "DailyOperations", Trigger = "Click" };
        var symbol = new ScreenSymbol { Type = "button", Commands = [command] };
        editor.SetScreenSelection(symbol, [symbol]);
        using var services = new ServiceCollection().AddLogging().AddSingleton(editor)
            .AddSingleton<OpcClientService>().AddSingleton<UndoRedoService>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<ScreenSymbolProperties>()).ToHtmlString());

        Assert.Contains("<option value=\"GenerateReport\" selected>Generate Report</option>", html);
        Assert.Contains("Target Report", html);
        Assert.Contains("<option value=\"DailyOperations\" selected>DailyOperations</option>", html);
        Assert.Equal("GenerateReport", command.Action);
        Assert.Equal("DailyOperations", command.TargetReport);
        Assert.Equal("Click", command.Trigger);
    }
}
