using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using ServerEditorWeb.Components.Editor;
using ServerEditorWeb.Services;
using Xunit;

namespace Tests.Editor;

public class AiAssistantTests
{
    private const string ProjectJson = "{\"Folder\":{\"Name\":\"Root\",\"Folders\":[{\"Name\":\"Area\",\"Variables\":[]}]},\"Screens\":[{\"Name\":\"Overview\"}]}";

    [Theory]
    [InlineData("{\"Folder\":{\"Name\":\"Root\"}}")]
    [InlineData("Here is the updated JSON with the requested changes:\n{\"Folder\":{\"Name\":\"Root\"}}")]
    [InlineData("Here is the updated JSON with the requested changes:\n```json\n{\"Folder\":{\"Name\":\"Root\"}}\n```\nDone.")]
    [InlineData("```JSON\n{\"Folder\":{\"Name\":\"Root\"}}\n```")]
    public void Response_ReturnsOnlyValidProjectJson(string response)
    {
        var (json, comments) = Parse(response);

        Assert.Equal("{\"Folder\":{\"Name\":\"Root\"}}", json);
        Assert.Equal("", comments);
        var model = JsonSerializer.Deserialize<SharedModels.NodeModel>(json);
        Assert.Equal("Root", model!.Folder.Name);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("Here is the section:\n```json\n[{\"Name\":\"Overview\"}]\n```", "[{\"Name\":\"Overview\"}]")]
    public void Response_SupportsScopedArrays(string response, string expected = "[]")
    {
        var (json, comments) = Parse(response);

        Assert.Equal(expected, json);
        Assert.Equal("", comments);
        using var document = JsonDocument.Parse(json);
        Assert.Equal(JsonValueKind.Array, document.RootElement.ValueKind);
    }

    [Fact]
    public void Response_PreservesDelimitersFencesEscapesAndUnicodeInsideJsonStrings()
    {
        var expected = JsonSerializer.Serialize(new { Code = "Write(\"Area.Value\", \"```json ///COMMENTS/// { [ } ]\");", Label = "\u00e8\u00b0" });
        var (json, comments) = Parse("Here is the JSON:\n" + expected + "\n///COMMENTS///\nUpdated project.");

        Assert.Equal(expected, json);
        Assert.Equal("Updated project.", comments);
    }

    [Theory]
    [InlineData(null, "empty")]
    [InlineData("  ", "empty")]
    [InlineData("Unable to generate a project.", "did not contain")]
    [InlineData("{\"Folder\":{\"Name\":\"Root\"}", "invalid or incomplete")]
    [InlineData("```json\n{invalid}\n```", "invalid or incomplete")]
    [InlineData("{\"Folder\":{}} {}", "multiple JSON")]
    public void Response_RejectsUnusableJsonWithAnExplicitError(string? response, string error)
    {
        var (json, comments) = Parse(response);

        Assert.Equal("", json);
        Assert.Contains(error, comments);
    }

    [Fact]
    public void ProjectAndResourcePrompts_RequireJsonOnlyWithoutSummaryInstructions()
    {
        var service = new AiService();
        var projectPrompt = (string)typeof(AiService).GetMethod("BuildPrompt", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(service, ["Create a project", "{}", null, null])!;
        var resourcePrompt = service.BuildResourceBundlePrompt("Create a screen");

        Assert.Contains("No introductory text", projectPrompt);
        Assert.Contains("No introductory text", resourcePrompt);
        Assert.DoesNotContain("///COMMENTS///", projectPrompt);
        Assert.DoesNotContain("///COMMENTS///", resourcePrompt);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Ollama_RequestsJsonModeAndReturnsCleanProjectJson(bool streaming)
    {
        int port;
        using (var reservation = new TcpListener(IPAddress.Loopback, 0))
        {
            reservation.Start();
            port = ((IPEndPoint)reservation.LocalEndpoint).Port;
        }
        var url = $"http://127.0.0.1:{port}";
        using var listener = new HttpListener();
        listener.Prefixes.Add(url + "/");
        listener.Start();
        var service = new AiService { OllamaBaseUrl = url, OllamaModel = "test-model" };
        var deltas = new List<string>();
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var resultTask = streaming
            ? service.AskStreamingAsync("Ollama", "Create a project", "{}", null, null,
                text => { deltas.Add(text); return Task.CompletedTask; }, cancellation.Token)
            : service.AskAsync("Ollama", "Create a project", "{}");
        var context = await listener.GetContextAsync().WaitAsync(cancellation.Token);
        using var requestReader = new StreamReader(context.Request.InputStream);
        var requestText = await requestReader.ReadToEndAsync(cancellation.Token);
        var raw = "Here is the updated JSON with the requested changes:\n```json\n" + ProjectJson + "\n```";
        var responseText = streaming
            ? JsonSerializer.Serialize(new { response = raw[..30], done = false }) + "\n" +
              JsonSerializer.Serialize(new { response = raw[30..], done = true }) + "\n"
            : JsonSerializer.Serialize(new { response = raw });
        var bytes = Encoding.UTF8.GetBytes(responseText);
        context.Response.ContentType = streaming ? "application/x-ndjson" : "application/json";
        context.Response.ContentLength64 = bytes.Length;
        await context.Response.OutputStream.WriteAsync(bytes, cancellation.Token);
        context.Response.Close();
        var (json, comments) = await resultTask;

        using var request = JsonDocument.Parse(requestText);
        Assert.Equal("/api/generate", context.Request.Url!.AbsolutePath);
        Assert.Equal("json", request.RootElement.GetProperty("format").GetString());
        Assert.Equal(streaming, request.RootElement.GetProperty("stream").GetBoolean());
        Assert.Equal("test-model", request.RootElement.GetProperty("model").GetString());
        Assert.Contains("No introductory text", request.RootElement.GetProperty("prompt").GetString());
        Assert.Equal(ProjectJson, json);
        Assert.Equal("", comments);
        Assert.Equal("Root", JsonSerializer.Deserialize<SharedModels.NodeModel>(json)!.Folder.Name);
        if (streaming) Assert.Equal(raw, deltas.Last());
    }

    [Fact]
    public async Task FirstUse_ScopeSelectorIsEnabledAndContainsProjectFolderAndSectionChoices()
    {
        using var services = new ServiceCollection().AddLogging().AddSingleton<AiService>()
            .AddSingleton<IJSRuntime, NoOpJsRuntime>().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var session = new JsonEditorSession { AiExpanded = true };
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var result = await renderer.RenderComponentAsync<JsonEditor>(ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(JsonEditor.Session)] = session,
                [nameof(JsonEditor.JsonText)] = ProjectJson
            }));
            return result.ToHtmlString();
        });
        var selector = Regex.Match(html, "<select[^>]*title=\"Choose new resources only[\\s\\S]*?</select>").Value;

        Assert.NotEmpty(selector);
        Assert.DoesNotContain("disabled", selector);
        Assert.Contains("value=\"resources:\"", selector);
        Assert.Contains("Whole project", selector);
        Assert.Contains("value=\"folder:Area\"", selector);
        Assert.Contains("value=\"section:Screens\"", selector);
        Assert.True(session.ResourceOnly);
    }

    [Theory]
    [InlineData("folder:Area", false)]
    [InlineData("section:Screens", false)]
    [InlineData("", false)]
    [InlineData("resources:", true)]
    public void ScopeSelection_SynchronizesModeAndClearsStaleGenerationState(string scope, bool resourceOnly)
    {
        var component = new JsonEditor();
        var session = component.Session;
        session.ScopePath = "folder:Old";
        session.ChunkedMode = true;
        session.ResourceJson = "old bundle";
        typeof(JsonEditor).GetProperty(nameof(JsonEditor.JsonText))!.SetValue(component, ProjectJson);

        InvokeScope(component, scope);

        Assert.Equal(resourceOnly, session.ResourceOnly);
        Assert.Equal(resourceOnly ? "" : scope, session.ScopePath);
        Assert.False(session.ChunkedMode);
        Assert.Equal("", session.ResourceJson);
        if (!resourceOnly)
            Assert.Contains(session.FolderScopes, entry => entry.Key == "folder:Area");
    }

    [Fact]
    public void ScopeSelection_DoesNotChangeAnInFlightRequest()
    {
        var component = new JsonEditor();
        component.Session.IsProcessing = true;
        component.Session.ScopePath = "folder:Area";
        component.Session.ResourceOnly = false;

        InvokeScope(component, "resources:");

        Assert.False(component.Session.ResourceOnly);
        Assert.Equal("folder:Area", component.Session.ScopePath);
    }

    private static (string json, string comments) Parse(string? response)
        => ((string, string))typeof(AiService).GetMethod("ParseAiResponse", BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, [response])!;

    private static void InvokeScope(JsonEditor component, string scope)
        => typeof(JsonEditor).GetMethod("OnScopeChanged", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(component, [new ChangeEventArgs { Value = scope }]);

    private sealed class NoOpJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => ValueTask.FromResult(default(TValue)!);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => InvokeAsync<TValue>(identifier, args);
    }
}
