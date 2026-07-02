// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestApiDemo.Services;

/// <summary>
/// Typed HTTP client for the HMI Server REST API.
/// Handles API key authentication and provides strongly-typed methods for all endpoints.
/// </summary>
public sealed class HmiApiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public HmiApiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;

        var baseUrl = _config["HmiApi:BaseUrl"] ?? "http://localhost:14842";
        _http.BaseAddress = new Uri(baseUrl);

        var apiKey = _config["HmiApi:ApiKey"];
        if (!string.IsNullOrEmpty(apiKey))
            _http.DefaultRequestHeaders.Add("X-API-Key", apiKey);

        _http.Timeout = TimeSpan.FromSeconds(10);
    }

    // ─── Health ──────────────────────────────────────────────────────

    public async Task<HealthResponse?> GetHealthAsync()
        => await GetAsync<HealthResponse>("/api/health");

    // ─── Variables ───────────────────────────────────────────────────

    public async Task<VariableListResponse?> GetVariablesAsync(string? filter = null)
    {
        var url = string.IsNullOrEmpty(filter) ? "/api/variables" : $"/api/variables?filter={Uri.EscapeDataString(filter)}";
        return await GetAsync<VariableListResponse>(url);
    }

    public async Task<VariableResponse?> GetVariableAsync(string path)
        => await GetAsync<VariableResponse>($"/api/variables/{Uri.EscapeDataString(path)}");

    public async Task<WriteResponse?> WriteVariableAsync(string path, object value)
        => await PostAsync<WriteResponse>($"/api/variables/{Uri.EscapeDataString(path)}", new { value });

    public async Task<VariableListResponse?> BatchReadAsync(string[] paths)
        => await PostAsync<VariableListResponse>("/api/variables/batch-read", new { paths });

    public async Task<BatchWriteResponse?> BatchWriteAsync(Dictionary<string, object> values)
        => await PostAsync<BatchWriteResponse>("/api/variables/batch-write", new { values });

    // ─── Alarms ──────────────────────────────────────────────────────

    public async Task<AlarmListResponse?> GetAlarmsAsync()
        => await GetAsync<AlarmListResponse>("/api/alarms");

    public async Task<ActionResponse?> AcknowledgeAlarmAsync(string path, string? comment = null)
        => await PostAsync<ActionResponse>($"/api/alarms/{Uri.EscapeDataString(path)}/acknowledge",
            comment != null ? new { comment } : null);

    public async Task<ShelveResponse?> ShelveAlarmAsync(string path, int durationMinutes = 60, string username = "demo-user")
        => await PostAsync<ShelveResponse>($"/api/alarms/{Uri.EscapeDataString(path)}/shelve",
            new { durationMinutes, username });

    public async Task<ActionResponse?> UnshelveAlarmAsync(string path, string username = "demo-user")
        => await PostAsync<ActionResponse>($"/api/alarms/{Uri.EscapeDataString(path)}/unshelve",
            new { username });

    // ─── Recipes ─────────────────────────────────────────────────────

    public async Task<RecipeListResponse?> GetRecipesAsync()
        => await GetAsync<RecipeListResponse>("/api/recipes");

    public async Task<ActionResponse?> ExecuteRecipeAsync(string name, string action = "Load", string target = "")
        => await PostAsync<ActionResponse>($"/api/recipes/{Uri.EscapeDataString(name)}/execute",
            new { action, target });

    // ─── Diagnostics ─────────────────────────────────────────────────

    public async Task<JsonElement?> GetDiagnosticsAsync()
        => await GetAsync<JsonElement>("/api/diagnostics");

    // ─── Events / Audit ──────────────────────────────────────────────

    public async Task<EventListResponse?> GetEventsAsync(int max = 100, string? category = null)
    {
        var url = $"/api/events?max={max}";
        if (!string.IsNullOrEmpty(category)) url += $"&category={Uri.EscapeDataString(category)}";
        return await GetAsync<EventListResponse>(url);
    }

    public async Task<AuditListResponse?> GetAuditAsync(int max = 100)
        => await GetAsync<AuditListResponse>($"/api/audit?max={max}");

    // ─── Redundancy ──────────────────────────────────────────────────

    public async Task<JsonElement?> GetRedundancyAsync()
        => await GetAsync<JsonElement>("/api/redundancy");

    // ─── Export ──────────────────────────────────────────────────────

    public async Task<string?> ExportCsvAsync(string? path = null, string? start = null, string? end = null)
    {
        var qs = new List<string>();
        if (path != null) qs.Add($"path={Uri.EscapeDataString(path)}");
        if (start != null) qs.Add($"start={Uri.EscapeDataString(start)}");
        if (end != null) qs.Add($"end={Uri.EscapeDataString(end)}");
        var url = "/api/export/csv" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");
        var response = await _http.GetAsync(url);
        return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
    }

    // ─── Helpers ─────────────────────────────────────────────────────

    private async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            var response = await _http.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch { return default; }
    }

    private async Task<T?> PostAsync<T>(string url, object? payload = null)
    {
        try
        {
            var content = payload != null
                ? new StringContent(JsonSerializer.Serialize(payload, _jsonOptions), Encoding.UTF8, "application/json")
                : new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch { return default; }
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

// ─── Response DTOs ───────────────────────────────────────────────────

public record HealthResponse(string Status, DateTime Timestamp, int Variables);

public record VariableResponse(string Path, JsonElement? Value, string? Type, string? Quality, DateTime? Timestamp);

public record VariableListResponse(int Count, List<VariableResponse> Variables);

public record WriteResponse(string Path, JsonElement? Value, string? Status);

public record BatchWriteResult(string Path, string Status, string? Error);
public record BatchWriteResponse(int Count, List<BatchWriteResult> Results);

public record AlarmInfo(
    string VariablePath, string Message, bool IsActive, bool IsAcknowledged, bool IsConfirmed,
    bool IsShelved, string? ShelvedUntil, string? ShelvedBy, bool AllowShelving,
    int Severity, string? LastTransitionTime, bool Retain);

public record AlarmListResponse(int Count, List<AlarmInfo> Alarms);

public record ActionResponse(string? Path, string? Status, string? Error);
public record ShelveResponse(string? Path, string? Status, int? DurationMinutes, string? Username, string? Error);

public record RecipeListResponse(int Count, List<string> Recipes);

public record EventEntry(DateTime Timestamp, string Category, string Source, string Message, string? Details);
public record EventListResponse(int Count, List<EventEntry> Events);

public record AuditEntry(DateTime Timestamp, string Action, string Username, string? Target, string? Details);
public record AuditListResponse(int Count, List<AuditEntry> Records);
