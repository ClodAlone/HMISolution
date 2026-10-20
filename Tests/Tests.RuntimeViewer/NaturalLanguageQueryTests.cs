using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using RuntimeViewer.Shared.Components.Viewer;
using RuntimeViewer.Shared.Services;
using SharedModels;
using Xunit;

namespace Tests.RuntimeViewer;

public class NaturalLanguageQueryTests : IDisposable
{
    private const string VariablePath = "Plant.Temperature";
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "nlquery_" + Guid.NewGuid().ToString("N"));
    private string DatabasePath => Path.Combine(_directory, "history.db");

    public NaturalLanguageQueryTests() => Directory.CreateDirectory(_directory);

    [Theory]
    [InlineData("svg")]
    [InlineData("responsive")]
    public async Task Screen_RendersUsableQueryWidgetInBothLayouts(string layout)
    {
        var project = CreateProject();
        await using var services = Services(project);
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
            (await renderer.RenderComponentAsync<ScreenRenderer>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(ScreenRenderer.Screen)] = new ScreenConfig
                {
                    LayoutMode = layout,
                    Symbols = [new ScreenSymbol { Id = "query", Type = "nlquery", Width = 500, Height = 350 }]
                }
            }))).ToHtmlString());

        Assert.Contains("class=\"nlquery-widget\"", html);
        Assert.Contains("Process Data Assistant", html);
        Assert.Contains("placeholder=\"Test process question\"", html);
        Assert.Contains("<input type=\"text\"", html);
        Assert.Contains("Ask", html);
        if (layout == "svg")
        {
            Assert.Contains("<foreignObject width=\"500\" height=\"350\">", html);
            Assert.Contains("width: 500px; height: 350px;", html);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Submit_UsesRequestedHistoricalDayAndDisplaysAiReply(bool enterKey)
    {
        var day = DateTime.UtcNow.Date;
        WriteHistory((day.AddDays(-2).AddHours(12), -900, null),
            (day.AddDays(-1).AddHours(10), 20, null),
            (day.AddDays(-1).AddHours(11), 40, null),
            (day.AddMinutes(1), 999, null));
        using var listener = StartListener(out var url);
        var project = CreateProject(url);
        await using var services = Services(project);
        await using var renderer = new QueryRenderer(services);
        await renderer.StartAsync();
        const string question = "What was the max Temperature yesterday?";
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var submission = renderer.SubmitAsync(question, enterKey);
        var context = await listener.GetContextAsync().WaitAsync(timeout.Token);
        string body;
        using (var reader = new StreamReader(context.Request.InputStream))
            body = await reader.ReadToEndAsync(timeout.Token);
        const string answer = "Yesterday's maximum Temperature was 40.";
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { response = answer }));
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = bytes.Length;
        await context.Response.OutputStream.WriteAsync(bytes, timeout.Token);
        context.Response.Close();
        await submission.WaitAsync(timeout.Token);

        using var request = JsonDocument.Parse(body);
        var prompt = request.RootElement.GetProperty("prompt").GetString()!;
        Assert.Equal("/api/generate", context.Request.Url!.AbsolutePath);
        Assert.Equal("test-model", request.RootElement.GetProperty("model").GetString());
        Assert.False(request.RootElement.GetProperty("stream").GetBoolean());
        Assert.Contains($"Time range: {day.AddDays(-1):yyyy-MM-dd} 00:00 UTC to {day:yyyy-MM-dd} 00:00 UTC", prompt);
        Assert.Contains("Data points: 2", prompt);
        Assert.Contains("Max: 40", prompt);
        Assert.Contains(question, prompt);
        Assert.DoesNotContain("999", prompt);
        Assert.DoesNotContain("-900", prompt);
        Assert.Contains(question, renderer.Html);
        Assert.Contains(answer, renderer.Html);
        Assert.Contains(VariablePath, renderer.Html);
        Assert.False(renderer.Loading);
        Assert.Equal("", renderer.Question);
    }

    [Fact]
    public async Task Reader_ExplicitRangeHonorsDatesAndPointLimit()
    {
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        WriteHistory((start.AddMinutes(-1), -1, null), (start, 10, null),
            (start.AddMinutes(1), 20, null), (start.AddMinutes(2), 30, null), (start.AddDays(1), 999, null));
        var reader = new HdaReaderService(CreateProject());
        var series = Assert.Single(await reader.ReadHistoryAsync([VariablePath], start, start.AddHours(1), 2, TestContext.Current.CancellationToken));
        Assert.Equal(VariablePath, series.VariableName);
        Assert.Equal(new double?[] { 10, 20 }, series.Points.Select(p => p.Value));
        Assert.Equal(new[] { start, start.AddMinutes(1) }, series.Points.Select(p => p.Time));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Reader_RejectsInvalidPointLimits(int maxPoints)
    {
        var reader = new HdaReaderService(CreateProject());
        var start = DateTime.UtcNow.Date;
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => reader.ReadHistoryAsync(
            [VariablePath], start, start.AddHours(1), maxPoints, TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Reader_RejectsEmptyOrReversedTimeRange(int endOffset)
    {
        var reader = new HdaReaderService(CreateProject());
        var start = DateTime.UtcNow.Date;
        await Assert.ThrowsAsync<ArgumentException>(() => reader.ReadHistoryAsync(
            [VariablePath], start, start.AddHours(endOffset), 10, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Reader_PreservesRollingWindowBehavior()
    {
        var now = DateTime.UtcNow;
        WriteHistory((now.AddHours(-2), -1, null), (now.AddMinutes(-10), 25, null));
        var reader = new HdaReaderService(CreateProject());
        var recent = Assert.Single(await reader.ReadHistoryAsync([VariablePath], 60, 10));
        var all = Assert.Single(await reader.ReadHistoryAsync([VariablePath], 0, 10));
        Assert.Equal(25, Assert.Single(recent.Points).Value);
        Assert.Equal(new double?[] { -1, 25 }, all.Points.Select(p => p.Value));
    }

    [Fact]
    public async Task Context_IncludesAllSmallSeriesSamplesAndCorrectNumericTimestamps()
    {
        var start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var samples = Enumerable.Range(1, 15).Select(i => (start.AddMinutes(i), (double?)i, (string?)null)).ToList();
        samples.Insert(0, (start, null, "starting"));
        samples.Add((start.AddMinutes(16), null, "stopped"));
        WriteHistory(samples.ToArray());
        var project = CreateProject();
        var service = new NaturalLanguageQueryService(new HdaReaderService(project), project);
        var context = await (Task<string>)typeof(NaturalLanguageQueryService)
            .GetMethod("BuildDataContextAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(service, [new List<string> { VariablePath }, start, start.AddHours(1), 500, TestContext.Current.CancellationToken])!;

        Assert.Contains("Data points: 17", context);
        Assert.Contains("First value: 1 at 2026-01-01 00:01:00", context);
        Assert.Contains("Last value: 15 at 2026-01-01 00:15:00", context);
        Assert.Contains("2026-01-01 00:15:00", context);
        Assert.Contains("stopped", context);
    }

    [Theory]
    [InlineData("missing", "Historical database is not configured")]
    [InlineData("empty", "No historical data was found")]
    [InlineData("schema", "no such table")]
    public async Task Query_ReportsHistoryFailuresWithoutCallingAi(string scenario, string message)
    {
        if (scenario != "missing") WriteHistory();
        var project = CreateProject();
        if (scenario == "schema") project.Model!.Database!.TableName = "missing_table";
        var result = await new NaturalLanguageQueryService(new HdaReaderService(project), project)
            .AskAsync("Temperature yesterday", TestContext.Current.CancellationToken);
        Assert.StartsWith("Error:", result.Answer);
        Assert.Contains(message, result.Answer);
        Assert.Empty(result.VariablesUsed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Widget_DisplaysConfigurationOrVariableErrorsAndAllowsAnotherQuestion(bool enabled)
    {
        var project = CreateProject();
        project.Settings.NaturalLanguageQuery!.Enabled = enabled;
        if (enabled) project.Model!.Folder.Variables.Clear();
        await using var services = Services(project);
        await using var renderer = new QueryRenderer(services);
        await renderer.StartAsync();
        await renderer.SubmitAsync("Temperature?", false);
        Assert.Contains(enabled ? "No variables with data logging enabled" : "Natural language query is not enabled", renderer.Html);
        Assert.False(renderer.Loading);
        await renderer.SubmitAsync("Try again", true);
        Assert.Contains("Try again", renderer.Html);
        Assert.False(renderer.Loading);
    }

    private ProjectService CreateProject(string url = "http://127.0.0.1:1")
    {
        var project = new ProjectService();
        typeof(ProjectService).GetField("_configPath", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(project, Path.Combine(_directory, "nodes.json"));
        typeof(ProjectService).GetField("_model", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(project, new NodeModel
        {
            Database = new DatabaseConfig { Provider = "Sqlite", ConnectionString = DatabasePath },
            Folder = new Folder
            {
                Name = "Plant",
                Variables = [new Variable { Name = "Temperature", DataLogging = new DataLoggingConfig { Enabled = true } }]
            },
            Server = new ServerSettings
            {
                NaturalLanguageQuery = new NaturalLanguageQueryConfig
                {
                    Enabled = true, Engine = "Ollama", OllamaBaseUrl = url, Model = "test-model", Placeholder = "Test process question"
                }
            }
        });
        return project;
    }

    private void WriteHistory(params (DateTime Time, double? Value, string? Text)[] points)
    {
        using var connection = new SqliteConnection($"Data Source={DatabasePath};Pooling=False");
        connection.Open();
        using var create = connection.CreateCommand();
        create.CommandText = "CREATE TABLE variable_data (time TEXT, variable_name TEXT, value REAL, value_str TEXT)";
        create.ExecuteNonQuery();
        foreach (var point in points)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO variable_data VALUES ($time, $name, $value, $text)";
            command.Parameters.AddWithValue("$time", point.Time.ToString("o", CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("$name", VariablePath);
            command.Parameters.AddWithValue("$value", (object?)point.Value ?? DBNull.Value);
            command.Parameters.AddWithValue("$text", (object?)point.Text ?? DBNull.Value);
            command.ExecuteNonQuery();
        }
    }

    private static HttpListener StartListener(out string url)
    {
        int port;
        using (var reservation = new TcpListener(IPAddress.Loopback, 0))
        {
            reservation.Start();
            port = ((IPEndPoint)reservation.LocalEndpoint).Port;
        }
        url = $"http://127.0.0.1:{port}";
        var listener = new HttpListener();
        listener.Prefixes.Add(url + "/");
        listener.Start();
        return listener;
    }

    private static ServiceProvider Services(ProjectService project) => new ServiceCollection().AddLogging()
        .AddSingleton(project).AddSingleton<OpcRuntimeClient>().AddSingleton<CommandService>()
        .AddSingleton<RuntimeAuthService>().AddSingleton<LocalizationService>().AddSingleton<HdaReaderService>()
        .AddSingleton<NaturalLanguageQueryService>().AddSingleton<IJSRuntime, NoBrowserJsRuntime>().BuildServiceProvider();

    private sealed class NoBrowserJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            throw new InvalidOperationException($"Unexpected JavaScript: {identifier}");
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) => InvokeAsync<TValue>(identifier, args);
    }

#pragma warning disable BL0006 // Exercise the real widget's submit and keyboard lifecycle.
    private sealed class QueryRenderer(IServiceProvider services) : Renderer(services, services.GetRequiredService<ILoggerFactory>())
    {
        private NlQueryWidget _widget = null!;
        private int _root;
        public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();
        public string Html { get; private set; } = "";
        public bool Loading => (bool)Field("_loading").GetValue(_widget)!;
        public string Question => (string)Field("_currentQuestion").GetValue(_widget)!;
        private static FieldInfo Field(string name) => typeof(NlQueryWidget).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
        public Task StartAsync() => Dispatcher.InvokeAsync(async () =>
        {
            _widget = (NlQueryWidget)InstantiateComponent(typeof(NlQueryWidget));
            _root = AssignRootComponentId(_widget);
            await RenderRootComponentAsync(_root, ParameterView.Empty);
        });
        public Task SubmitAsync(string question, bool enterKey) => Dispatcher.InvokeAsync(async () =>
        {
            Field("_currentQuestion").SetValue(_widget, question);
            var method = typeof(NlQueryWidget).GetMethod(enterKey ? "OnKeyDown" : "Submit", BindingFlags.Instance | BindingFlags.NonPublic)!;
            await (Task)method.Invoke(_widget, enterKey ? [new KeyboardEventArgs { Key = "Enter" }] : [])!;
        });
        protected override void HandleException(Exception exception) => throw new InvalidOperationException("Query widget render failed.", exception);
        protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
        {
            var frames = GetCurrentRenderTreeFrames(_root);
            Html = string.Join(" ", frames.Array.Take(frames.Count).Select(f => f.FrameType switch
            {
                RenderTreeFrameType.Text => f.TextContent,
                RenderTreeFrameType.Markup => f.MarkupContent,
                _ => ""
            }));
            return Task.CompletedTask;
        }
    }
#pragma warning restore BL0006

    public void Dispose()
    {
        using var connection = new SqliteConnection($"Data Source={DatabasePath};Mode=ReadOnly");
        SqliteConnection.ClearPool(connection);
        Directory.Delete(_directory, true);
    }
}
