using System.Globalization;
using System.Text.Json;
using Microsoft.Data.Sqlite;
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

public sealed class HistoricalArchiveTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "historical-tests-" + Guid.NewGuid().ToString("N"));
    private readonly ProjectService _project = new();
    private static readonly DateTime Start = new(2026, 10, 23, 8, 0, 0, DateTimeKind.Utc);
    private string DatabasePath => Path.Combine(_directory, "Data", "history.db");

    public HistoricalArchiveTests()
    {
        Directory.CreateDirectory(Path.Combine(_directory, "Data"));
        var config = Path.Combine(_directory, "nodes.json");
        File.WriteAllText(config, JsonSerializer.Serialize(new NodeModel
        {
            Database = new DatabaseConfig { Provider = "Sqlite", ConnectionString = "history.db", TableName = "variable_data" }
        }));
        Assert.True(_project.Load(config).Success);
        using var db = new SqliteConnection($"Data Source={DatabasePath};Pooling=False");
        db.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "CREATE TABLE variable_data(time TEXT NOT NULL, variable_name TEXT NOT NULL, value REAL, value_str TEXT); CREATE INDEX variable_time ON variable_data(variable_name, time)";
        cmd.ExecuteNonQuery();
    }

    [Fact]
    public async Task Query_SeedsPredecessorsAndDoesNotTruncateDenseSeriesToLegacyLimit()
    {
        Insert("Constant", Start.AddDays(-2), 42);
        for (var i = 0; i <= 1200; i++) Insert("Dense", Start.AddSeconds(i), i);
        Insert("Future", Start.AddHours(1), 99);
        Insert("Text", Start.AddSeconds(10), null, "Running");
        var result = await new HdaReaderService(_project).ReadSampledHistoryAsync(
            ["Constant", "Dense", "Future", "Text", "Constant"], Start, Start.AddSeconds(1200), 60);
        Assert.Equal(4, result.Count);
        Assert.All(result, s => Assert.Equal(21, s.Points.Count));
        Assert.All(result[0].Points, p => Assert.Equal(42, p.Value));
        Assert.Equal(Enumerable.Range(0, 21).Select(i => (double?)(i * 60)), result[1].Points.Select(p => p.Value));
        Assert.All(result[2].Points, p => Assert.Null(p.Value));
        Assert.Null(result[3].Points[0].StringValue);
        Assert.All(result[3].Points.Skip(1), p => Assert.Equal("Running", p.StringValue));
    }

    [Fact]
    public async Task Query_UsesChangesAtBothExactBoundaries()
    {
        Insert("Speed", Start.AddSeconds(-1), 1);
        Insert("Speed", Start, 2);
        Insert("Speed", Start.AddSeconds(30), 3);
        Insert("Speed", Start.AddSeconds(60), 4);
        var result = await new HdaReaderService(_project).ReadSampledHistoryAsync(["Speed"], Start, Start.AddSeconds(60), 30);
        Assert.Equal(new double?[] { 2, 3, 4 }, result.Single().Points.Select(p => p.Value));
    }

    [Fact]
    public async Task Query_RejectsMissingConfigurationInvalidVariablesAndCancellation()
    {
        var reader = new HdaReaderService(_project);
        await Assert.ThrowsAsync<ArgumentException>(() => reader.ReadSampledHistoryAsync([], Start, Start.AddMinutes(1), 10));
        await Assert.ThrowsAsync<ArgumentException>(() => reader.ReadSampledHistoryAsync(Enumerable.Range(0, 21).Select(i => $"V{i}").ToList(), Start, Start.AddMinutes(1), 10));
        await Assert.ThrowsAsync<InvalidOperationException>(() => new HdaReaderService(new ProjectService()).ReadSampledHistoryAsync(["V"], Start, Start.AddMinutes(1), 10));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => reader.ReadSampledHistoryAsync(["V"], Start, Start.AddMinutes(1), 10, cancellation.Token));
    }

    [Theory]
    [InlineData("svg", "historicaltrend")]
    [InlineData("svg", "historicaltable")]
    [InlineData("responsive", "historicaltrend")]
    [InlineData("responsive", "historicaltable")]
    public async Task Widgets_RenderSampledValuesInBothLayouts(string layout, string type)
    {
        Insert("Mixer.Constant", DateTime.UtcNow.AddDays(-2), 42);
        var symbol = new ScreenSymbol { Id = "history", Type = type, Width = 800, Height = 400, HdaVariablePaths = ["Mixer.Constant"], HdaTimeRangeMinutes = 5, HistoricalSamplingIntervalSeconds = 60 };
        WidgetThemes.ApplyPreset(symbol, "ivory");
        await using var services = new ServiceCollection().AddLogging()
            .AddSingleton(_project).AddSingleton<HdaReaderService>()
            .AddSingleton<OpcRuntimeClient>().AddSingleton<CommandService>()
            .AddSingleton<RuntimeAuthService>().AddSingleton<LocalizationService>()
            .AddSingleton<IJSRuntime, NoBrowserJsRuntime>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<ScreenRenderer>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(ScreenRenderer.Screen)] = new ScreenConfig { Name = "Test", LayoutMode = layout, Symbols = [symbol] }
            }))).ToHtmlString());
        Assert.Contains(type == "historicaltable" ? "Historical Table" : "Historical Trend", html);
        Assert.Contains("--wt-text: #44372a", html);
        Assert.Contains("Mixer.Constant", html);
        Assert.Matches("<strong[^>]*>42</strong>", html);
        Assert.DoesNotContain("role=\"alert\"", html);
        if (type == "historicaltable")
        {
            Assert.Equal(6, System.Text.RegularExpressions.Regex.Matches(html, "<td[^>]*>42</td>").Count);
            Assert.Contains("historical-selected", html);
        }
        else
        {
            Assert.Contains(" H204 V125", html);
            Assert.Contains("Historical variables on a shared time axis", html);
        }
    }

    [Fact]
    public async Task LinkedWidgets_SelectTheSameTimestampAndValuesInEitherDirection()
    {
        Insert("Speed", Start, 10);
        Insert("Speed", Start.AddSeconds(30), 20);
        Insert("Speed", Start.AddSeconds(60), 30);
        var group = new HistoricalCursorGroup();
        group.Publish("linked", new HistoricalSelection(Start, Start.AddSeconds(60), 30, null));
        await using var services = new ServiceCollection().AddLogging().AddSingleton(_project).AddSingleton<HdaReaderService>().AddSingleton<IJSRuntime, NoBrowserJsRuntime>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        CapturedHistory? chart = null, table = null;
        await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var chartRoot = await renderer.RenderComponentAsync<CapturedHistory>(Parameters(false, widget => chart = widget));
            var tableRoot = await renderer.RenderComponentAsync<CapturedHistory>(Parameters(true, widget => table = widget));
            Select(chart!, 1);
            Assert.Contains(Start.AddSeconds(30).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"), tableRoot.ToHtmlString());
            Assert.Matches("<strong[^>]*>20</strong>", tableRoot.ToHtmlString());
            Assert.Equal(Start.AddSeconds(30), group.Get("linked")!.CursorUtc);
            Select(table!, 0);
            Assert.Matches("<strong[^>]*>10</strong>", chartRoot.ToHtmlString());
            Assert.Equal(Start, group.Get("linked")!.CursorUtc);
        });

        ParameterView Parameters(bool tableOnly, Action<CapturedHistory> capture) => ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            [nameof(HistoricalTrendWidget.VariablePaths)] = new List<string> { "Speed" },
            [nameof(HistoricalTrendWidget.CursorGroup)] = group,
            [nameof(HistoricalTrendWidget.SyncGroup)] = "linked",
            [nameof(HistoricalTrendWidget.TableOnly)] = tableOnly,
            [nameof(CapturedHistory.Capture)] = capture
        });
        static void Select(HistoricalTrendWidget widget, int index) => typeof(HistoricalTrendWidget)
            .GetMethod("SelectCursor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(widget, [index]);
    }

    [Fact]
    public async Task Widget_DatabaseFailureIsVisibleAndSameRangeCanBeRetriedAndRefreshed()
    {
        Insert("Speed", Start, 42);
        var group = new HistoricalCursorGroup();
        group.Publish("linked", new HistoricalSelection(Start, Start.AddSeconds(60), 30, null));
        _project.Model!.Database.TableName = "missing_table";
        await using var services = new ServiceCollection().AddLogging().AddSingleton(_project).AddSingleton<HdaReaderService>().AddSingleton<IJSRuntime, NoBrowserJsRuntime>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        CapturedHistory? widget = null;
        await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var root = await renderer.RenderComponentAsync<CapturedHistory>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(HistoricalTrendWidget.VariablePaths)] = new List<string> { "Speed" },
                [nameof(HistoricalTrendWidget.CursorGroup)] = group,
                [nameof(HistoricalTrendWidget.SyncGroup)] = "linked",
                [nameof(CapturedHistory.Capture)] = (Action<CapturedHistory>)(value => widget = value)
            }));
            Assert.Contains("role=\"alert\"", root.ToHtmlString());
            Assert.Contains("Unable to read historical data", root.ToHtmlString());
            _project.Model.Database.TableName = "variable_data";
            await Refresh();
            Assert.DoesNotContain("role=\"alert\"", root.ToHtmlString());
            Assert.Matches("<strong[^>]*>42</strong>", root.ToHtmlString());
            Insert("Speed", Start.AddSeconds(45), 99);
            await Refresh();
            Assert.Matches("<strong[^>]*>99</strong>", root.ToHtmlString());
        });

        async Task Refresh()
        {
            await (Task)typeof(HistoricalTrendWidget).GetMethod("ApplyAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(widget, null)!;
            typeof(ComponentBase).GetMethod("StateHasChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(widget, null);
        }
    }

    [Theory]
    [InlineData(45, 30, " H540 V125")]
    [InlineData(60, 120, " M60,125")]
    public async Task Plot_UsesFullRequestedTimeScaleForUnalignedAndSingleSampleRanges(int seconds, int interval, string expectedPath)
    {
        Insert("Speed", Start, 42);
        var group = new HistoricalCursorGroup();
        group.Publish("linked", new HistoricalSelection(Start, Start.AddSeconds(seconds), interval, null));
        await using var services = new ServiceCollection().AddLogging().AddSingleton(_project).AddSingleton<HdaReaderService>().AddSingleton<IJSRuntime, NoBrowserJsRuntime>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<HistoricalTrendWidget>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(HistoricalTrendWidget.VariablePaths)] = new List<string> { "Speed" },
                [nameof(HistoricalTrendWidget.CursorGroup)] = group,
                [nameof(HistoricalTrendWidget.SyncGroup)] = "linked"
            }))).ToHtmlString());
        Assert.Contains(expectedPath, html);
        Assert.Contains(Start.AddSeconds(seconds).ToLocalTime().ToString("MM-dd HH:mm:ss"), html);
        Assert.Contains("<circle", html);
        Assert.DoesNotContain(" H780", html);
    }

    [Theory]
    [InlineData("number")]
    [InlineData("text")]
    [InlineData("none")]
    public async Task Table_HidesOnlyAllUnknownSamplesAndPreservesOriginalCursorIndices(string dataKind)
    {
        if (dataKind == "number") Insert("Signal", Start.AddSeconds(30), 0);
        if (dataKind == "text") Insert("Signal", Start.AddSeconds(30), null, "Idle");
        var group = new HistoricalCursorGroup();
        group.Publish("linked", new HistoricalSelection(Start, Start.AddSeconds(60), 30, null));
        await using var services = new ServiceCollection().AddLogging().AddSingleton(_project).AddSingleton<HdaReaderService>().AddSingleton<IJSRuntime, NoBrowserJsRuntime>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        CapturedHistory? widget = null;
        await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var root = await renderer.RenderComponentAsync<CapturedHistory>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(HistoricalTrendWidget.VariablePaths)] = new List<string> { "Signal", "Unknown" },
                [nameof(HistoricalTrendWidget.CursorGroup)] = group,
                [nameof(HistoricalTrendWidget.SyncGroup)] = "linked",
                [nameof(HistoricalTrendWidget.TableOnly)] = true,
                [nameof(CapturedHistory.Capture)] = (Action<CapturedHistory>)(value => widget = value)
            }));
            var html = root.ToHtmlString();
            var body = System.Text.RegularExpressions.Regex.Match(html, "<tbody[^>]*>(.*?)</tbody>", System.Text.RegularExpressions.RegexOptions.Singleline).Groups[1].Value;
            Assert.Equal(dataKind == "none" ? 0 : 2, System.Text.RegularExpressions.Regex.Matches(body, "<tr[ >]").Count);
            Assert.DoesNotContain(Start.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"), body);
            Assert.Contains(dataKind == "none" ? "3 samples have no recorded values (hidden)." : "1 sample has no recorded values (hidden).", html);
            if (dataKind != "none")
            {
                Assert.Contains(Start.AddSeconds(30).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"), body);
                Assert.Equal(2, System.Text.RegularExpressions.Regex.Matches(body, dataKind == "number" ? "<td[^>]*>0</td>" : "<td[^>]*>Idle</td>").Count);
                typeof(HistoricalTrendWidget).GetMethod("SelectCursor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(widget, [1]);
                Assert.Equal(Start.AddSeconds(30), group.Get("linked")!.CursorUtc);
            }
            else Assert.Contains("No recorded values", html);

            typeof(HistoricalTrendWidget).GetField("_showEmptySamples", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(widget, true);
            typeof(ComponentBase).GetMethod("StateHasChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(widget, null);
            html = root.ToHtmlString();
            body = System.Text.RegularExpressions.Regex.Match(html, "<tbody[^>]*>(.*?)</tbody>", System.Text.RegularExpressions.RegexOptions.Singleline).Groups[1].Value;
            Assert.Equal(3, System.Text.RegularExpressions.Regex.Matches(body, "<tr[ >]").Count);
            Assert.Contains(Start.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"), body);
        });
    }

    public sealed class CapturedHistory : HistoricalTrendWidget
    {
        [Parameter] public Action<CapturedHistory>? Capture { get; set; }
        protected override void OnInitialized() => Capture?.Invoke(this);
    }

    private void Insert(string variable, DateTime time, double? value, string? text = null)
    {
        using var db = new SqliteConnection($"Data Source={DatabasePath};Pooling=False");
        db.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "INSERT INTO variable_data VALUES(@t,@n,@v,@s)";
        cmd.Parameters.AddWithValue("@t", time.ToString("o", CultureInfo.InvariantCulture));
        cmd.Parameters.AddWithValue("@n", variable);
        cmd.Parameters.AddWithValue("@v", (object?)value ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@s", (object?)text ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
        using var pool = new SqliteConnection($"Data Source={DatabasePath};Mode=ReadOnly");
        SqliteConnection.ClearPool(pool);
        Directory.Delete(_directory, recursive: true);
    }

    private sealed class NoBrowserJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) => throw new InvalidOperationException(identifier);
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
    }
}
