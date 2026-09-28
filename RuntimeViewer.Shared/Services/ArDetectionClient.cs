// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace RuntimeViewer.Shared.Services;

/// <summary>Result of querying the OR server's /health endpoint.</summary>
public enum OrServerStatus
{
    /// <summary>The server process could not be reached at all (not running, wrong port/URL, network error).</summary>
    Unreachable,
    /// <summary>The server is running and reachable but has no detection model loaded — /detect will fail until one is provided.</summary>
    NoModel,
    /// <summary>The server is running, reachable, and has a detection model loaded.</summary>
    Ready
}

/// <summary>A single AR detection returned by the local object-recognition server, in source-frame pixel coordinates.</summary>
public class ArDetectionResult
{
    [JsonPropertyName("label")] public string Label { get; set; } = "";
    [JsonPropertyName("confidence")] public double Confidence { get; set; }
    [JsonPropertyName("x")] public double X { get; set; }
    [JsonPropertyName("y")] public double Y { get; set; }
    [JsonPropertyName("width")] public double Width { get; set; }
    [JsonPropertyName("height")] public double Height { get; set; }
    [JsonPropertyName("classId")] public int ClassId { get; set; }
    [JsonPropertyName("instanceKey")] public string? InstanceKey { get; set; }
}

/// <summary>Response payload from the local OR server's /detect endpoint.</summary>
public class ArDetectionResponse
{
    [JsonPropertyName("width")] public int Width { get; set; }
    [JsonPropertyName("height")] public int Height { get; set; }
    [JsonPropertyName("detections")] public List<ArDetectionResult> Detections { get; set; } = new();
}

/// <summary>
/// Thin HTTP client that submits camera frames to the local object-recognition (OR) server and
/// returns detections. The OR server is expected to run on the same machine or local network as
/// the AR client (see AugmentedRealityConfig.OrServerUrl) to keep round-trip latency low enough
/// for a smooth live-camera overlay experience.
/// </summary>
public class ArDetectionClient
{
    private readonly HttpClient _http;

    public ArDetectionClient(HttpClient http)
    {
        _http = http;
        _http.Timeout = TimeSpan.FromSeconds(5);
    }

    /// <summary>Whether the OR server process is reachable at all (regardless of whether a model is loaded).</summary>
    public async Task<bool> IsServerReadyAsync(string orServerUrl, CancellationToken ct = default)
    {
        var status = await GetServerStatusAsync(orServerUrl, ct);
        return status != OrServerStatus.Unreachable;
    }

    /// <summary>
    /// Queries the OR server's /health endpoint and distinguishes an unreachable process from a
    /// reachable process that has no detection model loaded yet, so the UI can show an accurate
    /// message instead of conflating the two ("unreachable" vs "running, but no model").
    /// </summary>
    public async Task<OrServerStatus> GetServerStatusAsync(string orServerUrl, CancellationToken ct = default)
    {
        try
        {
            var resp = await _http.GetFromJsonAsync<Dictionary<string, object>>($"{orServerUrl.TrimEnd('/')}/health", ct);
            if (resp == null) return OrServerStatus.Unreachable;
            var status = resp.TryGetValue("status", out var s) ? s?.ToString() : null;
            return status switch
            {
                "ready" => OrServerStatus.Ready,
                "no-model" => OrServerStatus.NoModel,
                _ => OrServerStatus.Unreachable
            };
        }
        catch
        {
            return OrServerStatus.Unreachable;
        }
    }

    /// <summary>
    /// Posts a single JPEG frame (as bytes) to the OR server's /detect endpoint and returns
    /// the recognized objects. Designed to be called repeatedly at AugmentedRealityConfig.TargetFps.
    /// </summary>
    public async Task<ArDetectionResponse?> DetectAsync(string orServerUrl, byte[] jpegBytes, double minConfidence, CancellationToken ct = default)
    {
        try
        {
            using var content = new ByteArrayContent(jpegBytes);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            var url = $"{orServerUrl.TrimEnd('/')}/detect?minConfidence={minConfidence.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            using var response = await _http.PostAsync(url, content, ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ArDetectionResponse>(ct);
        }
        catch
        {
            return null;
        }
    }
}
