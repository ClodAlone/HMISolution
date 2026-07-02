// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;
using SharedModels;

namespace ServerEditorWeb.Services;

public class ScriptDebugService
{
    private readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(3) };
    private readonly NodeEditorService _editor;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    private static readonly JsonSerializerOptions _readOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ScriptDebugService(NodeEditorService editor) => _editor = editor;

    public string? LastError { get; set; }

    private int DiagPort => _editor.RootModel?.Server?.DiagnosticsPort ?? 14841;

    public async Task SetBreakpointsAsync(string scriptName, List<int> lines)
    {
        var payload = new SetBreakpointsRequest { ScriptName = scriptName, Lines = lines };
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var resp = await _httpClient.PostAsync("http://localhost:" + DiagPort + "/debug/breakpoints", content);
        LastError = resp.IsSuccessStatusCode ? null : "HTTP " + (int)resp.StatusCode;
    }

    public async Task SendCommandAsync(string scriptName, ScriptDebugCommand command)
    {
        var payload = new DebugCommandRequest { ScriptName = scriptName, Command = command };
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var resp = await _httpClient.PostAsync("http://localhost:" + DiagPort + "/debug/command", content);
        LastError = resp.IsSuccessStatusCode ? null : "HTTP " + (int)resp.StatusCode;
    }

    /// <summary>
    /// Poll the server diagnostics directly via HTTP GET (bypasses OPC UA).
    /// </summary>
    public async Task<ServerDiagnostics?> PollDiagnosticsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:" + DiagPort);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServerDiagnostics>(json, _readOptions);
        }
        catch { return null; }
    }
}
