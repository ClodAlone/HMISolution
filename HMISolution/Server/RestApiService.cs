using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Lightweight REST API for external integration (MES/ERP, dashboards, mobile).
/// Runs on a configurable port and supports:
///   GET  /api/variables                — list all variables with current values
///   GET  /api/variables/{path}         — read a single variable
///   POST /api/variables/{path}         — write a variable value
///   POST /api/variables/batch-read     — batch read multiple variables
///   POST /api/variables/batch-write    — batch write multiple variables
///   GET  /api/alarms                   — list active alarm states
///   POST /api/alarms/{path}/acknowledge — acknowledge a specific alarm
///   POST /api/alarms/{path}/shelve     — shelve a specific alarm
///   POST /api/alarms/{path}/unshelve   — unshelve a specific alarm
///   GET  /api/recipes                  — list configured recipes
///   POST /api/recipes/{name}/execute   — execute a recipe action
///   POST /api/reports/{name}/generate  — generate a report
///   GET  /api/redundancy               — get redundancy status
///   POST /api/redundancy/force-active  — force active role
///   POST /api/redundancy/force-standby — force standby role
///   GET  /api/diagnostics              — server diagnostics snapshot
///   GET  /api/events                   — query event log
///   GET  /api/audit                    — query audit trail
///   GET  /api/export/csv               — export historical data as CSV
///   GET  /api/export/json              — export historical data as JSON
///   GET  /api/health                   — health check endpoint
/// </summary>
public sealed class RestApiService : IDisposable
{
    private readonly SimpleFileServerNodeManager _nodeManager;
    private readonly ApiConfig _config;
    private readonly EventLogger? _eventLogger;
    private readonly AuditTrailLogger? _auditLogger;
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _listenTask;
    private RateLimiter? _rateLimiter;

    public RestApiService(SimpleFileServerNodeManager nodeManager, ApiConfig config,
        EventLogger? eventLogger, AuditTrailLogger? auditLogger,
        RateLimitConfig? rateLimitConfig = null)
    {
        _nodeManager = nodeManager;
        _config = config;
        _eventLogger = eventLogger;
        _auditLogger = auditLogger;

        if (rateLimitConfig is { Enabled: true, ApiMaxRequestsPerWindow: > 0 })
        {
            _rateLimiter = new RateLimiter(
                rateLimitConfig.ApiMaxRequestsPerWindow,
                TimeSpan.FromSeconds(rateLimitConfig.WindowSeconds));
            Log.Information("REST API rate limiting enabled: {Max} requests per {Window}s",
                rateLimitConfig.ApiMaxRequestsPerWindow, rateLimitConfig.WindowSeconds);
        }
    }

    public void Start()
    {
        if (!_config.Enabled || _config.Port <= 0) return;

        try
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, _config.Port);
            _listener.Start();
            _listenTask = Task.Run(() => ListenLoop(_cts.Token));
            Log.Information("REST API started on http://0.0.0.0:{Port}/", _config.Port);
        }
        catch (Exception ex)
        {
            Log.Warning("REST API disabled: {Error}", ex.Message);
        }
    }

    private async Task ListenLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var client = await _listener!.AcceptTcpClientAsync(ct);
                _ = Task.Run(() => HandleClient(client), ct);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex) { Log.Debug("REST API accept error: {Error}", ex.Message); }
        }
    }

    private async Task HandleClient(TcpClient client)
    {
        try
        {
            using (client)
            {
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;

                using var stream = client.GetStream();
                var buffer = new byte[8192];
                int read = await stream.ReadAsync(buffer);
                var request = Encoding.UTF8.GetString(buffer, 0, read);

                var (method, path, query, body) = ParseHttpRequest(request);

                // CORS preflight
                if (method == "OPTIONS")
                {
                    await SendResponse(stream, 204, "", corsHeaders: true);
                    return;
                }

                // API key authentication
                if (!string.IsNullOrEmpty(_config.ApiKey))
                {
                    var keyFromHeader = GetHeaderValue(request, "X-API-Key");
                    var keyFromQuery = GetQueryParam(query, "apiKey");
                    if (!_config.ApiKey.Equals(keyFromHeader, StringComparison.Ordinal) &&
                            !_config.ApiKey.Equals(keyFromQuery, StringComparison.Ordinal))
                        {
                            await SendJsonResponse(stream, 401, new { error = "Unauthorized — invalid or missing API key" });
                            return;
                        }
                    }

                    // Rate limiting per client IP
                    if (_rateLimiter != null)
                    {
                        var clientIp = ((IPEndPoint?)client.Client.RemoteEndPoint)?.Address.ToString() ?? "unknown";
                        if (!_rateLimiter.IsAllowed(clientIp))
                        {
                            var remaining = _rateLimiter.GetRemaining(clientIp);
                            Log.Warning("REST API rate limit exceeded for {ClientIp}", clientIp);
                            await SendJsonResponse(stream, 429, new { error = "Too many requests — rate limit exceeded", retryAfterSeconds = _rateLimiter.Window.TotalSeconds });
                            return;
                        }
                    }

                    await RouteRequest(stream, method, path, query, body);
            }
        }
        catch (Exception ex)
        {
            Log.Debug("REST API request error: {Error}", ex.Message);
        }
    }
    private async Task RouteRequest(NetworkStream stream, string method, string path, string query, string body)
    {
        try
        {
            // --- Health ---
            if (path == "/api/health" && method == "GET")
            {
                await SendJsonResponse(stream, 200, new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    variables = _nodeManager._variables.Count
                });
            }
            // --- Variables: batch operations (must match before /api/variables/{path}) ---
            else if (path == "/api/variables/batch-read" && method == "POST")
            {
                await HandleBatchRead(stream, body);
            }
            else if (path == "/api/variables/batch-write" && method == "POST")
            {
                await HandleBatchWrite(stream, body);
            }
            // --- Variables: list / read / write ---
            else if (path == "/api/variables" && method == "GET")
            {
                await HandleGetVariables(stream, query);
            }
            else if (path.StartsWith("/api/variables/") && method == "GET")
            {
                var varPath = Uri.UnescapeDataString(path["/api/variables/".Length..]);
                await HandleGetVariable(stream, varPath);
            }
            else if (path.StartsWith("/api/variables/") && method == "POST")
            {
                var varPath = Uri.UnescapeDataString(path["/api/variables/".Length..]);
                await HandleWriteVariable(stream, varPath, body);
            }
            // --- Alarms: list / acknowledge / shelve / unshelve ---
            else if (path == "/api/alarms" && method == "GET")
            {
                await HandleGetAlarms(stream);
            }
            else if (path.EndsWith("/acknowledge") && path.StartsWith("/api/alarms/") && method == "POST")
            {
                var alarmPath = Uri.UnescapeDataString(
                    path["/api/alarms/".Length..^"/acknowledge".Length]);
                await HandleAcknowledgeAlarm(stream, alarmPath, body);
            }
            else if (path.EndsWith("/shelve") && path.StartsWith("/api/alarms/") && method == "POST")
            {
                var alarmPath = Uri.UnescapeDataString(
                    path["/api/alarms/".Length..^"/shelve".Length]);
                await HandleShelveAlarm(stream, alarmPath, body);
            }
            else if (path.EndsWith("/unshelve") && path.StartsWith("/api/alarms/") && method == "POST")
            {
                var alarmPath = Uri.UnescapeDataString(
                    path["/api/alarms/".Length..^"/unshelve".Length]);
                await HandleUnshelveAlarm(stream, alarmPath, body);
            }
            // --- Recipes ---
            else if (path == "/api/recipes" && method == "GET")
            {
                await HandleGetRecipes(stream);
            }
            else if (path.EndsWith("/execute") && path.StartsWith("/api/recipes/") && method == "POST")
            {
                var recipeName = Uri.UnescapeDataString(
                    path["/api/recipes/".Length..^"/execute".Length]);
                await HandleExecuteRecipe(stream, recipeName, body);
            }
            // --- Reports ---
            else if (path.EndsWith("/generate") && path.StartsWith("/api/reports/") && method == "POST")
            {
                var reportName = Uri.UnescapeDataString(
                    path["/api/reports/".Length..^"/generate".Length]);
                await HandleGenerateReport(stream, reportName);
            }
            // --- Redundancy ---
            else if (path == "/api/redundancy" && method == "GET")
            {
                await HandleGetRedundancy(stream);
            }
            else if (path == "/api/redundancy/force-active" && method == "POST")
            {
                await HandleForceRedundancy(stream, body, active: true);
            }
            else if (path == "/api/redundancy/force-standby" && method == "POST")
            {
                await HandleForceRedundancy(stream, body, active: false);
            }
            // --- Diagnostics ---
            else if (path == "/api/diagnostics" && method == "GET")
            {
                await HandleGetDiagnostics(stream);
            }
            // --- Events / Audit ---
            else if (path == "/api/events" && method == "GET")
            {
                await HandleGetEvents(stream, query);
            }
            else if (path == "/api/audit" && method == "GET")
            {
                await HandleGetAudit(stream, query);
            }
            // --- Export ---
            else if (path == "/api/export/csv" && method == "GET")
            {
                await HandleExportCsv(stream, query);
            }
            else if (path == "/api/export/json" && method == "GET")
            {
                await HandleExportJson(stream, query);
            }
            else
            {
                await SendJsonResponse(stream, 404, new { error = "Not found", path });
            }
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 500, new { error = ex.Message });
        }
    }
    // === Variables ====================================================

    private async Task HandleGetVariables(NetworkStream stream, string query)
    {
        var filter = GetQueryParam(query, "filter") ?? "";
        var variables = new List<object>();

        foreach (var kv in _nodeManager._variables)
        {
            if (!string.IsNullOrEmpty(filter) &&
                !kv.Key.Contains(filter, StringComparison.OrdinalIgnoreCase))
                continue;

            variables.Add(new
            {
                path = kv.Key,
                value = kv.Value.Value,
                type = kv.Value.DataType?.ToString(),
                quality = kv.Value.StatusCode.ToString(),
                timestamp = kv.Value.Timestamp
            });
        }

        await SendJsonResponse(stream, 200, new { count = variables.Count, variables });
    }

    private async Task HandleGetVariable(NetworkStream stream, string varPath)
    {
        try
        {
            var value = _nodeManager.ReadVariable(varPath);
            if (_nodeManager._variables.TryGetValue(varPath, out var vs))
            {
                await SendJsonResponse(stream, 200, new
                {
                    path = varPath,
                    value = vs.Value,
                    type = vs.DataType?.ToString(),
                    quality = vs.StatusCode.ToString(),
                    timestamp = vs.Timestamp
                });
            }
            else
            {
                await SendJsonResponse(stream, 404, new { error = $"Variable '{varPath}' not found" });
            }
        }
        catch
        {
            await SendJsonResponse(stream, 404, new { error = $"Variable '{varPath}' not found" });
        }
    }

    private async Task HandleWriteVariable(NetworkStream stream, string varPath, string body)
    {
        try
        {
            var doc = JsonDocument.Parse(body);
            object? writeValue = null;

            if (doc.RootElement.TryGetProperty("value", out var valElem))
            {
                writeValue = ParseJsonValue(valElem);
            }

            if (writeValue == null)
            {
                await SendJsonResponse(stream, 400, new { error = "Missing 'value' in request body" });
                return;
            }

            _nodeManager.WriteVariable(varPath, writeValue);
            await SendJsonResponse(stream, 200, new { path = varPath, value = writeValue, status = "ok" });
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 400, new { error = ex.Message });
        }
    }

    private async Task HandleBatchRead(NetworkStream stream, string body)
    {
        try
        {
            var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("paths", out var pathsElem) ||
                pathsElem.ValueKind != JsonValueKind.Array)
            {
                await SendJsonResponse(stream, 400, new { error = "Missing 'paths' array in request body" });
                return;
            }

            var results = new List<object>();
            foreach (var pathElem in pathsElem.EnumerateArray())
            {
                var varPath = pathElem.GetString();
                if (string.IsNullOrEmpty(varPath)) continue;

                if (_nodeManager._variables.TryGetValue(varPath, out var vs))
                {
                    results.Add(new
                    {
                        path = varPath,
                        value = vs.Value,
                        type = vs.DataType?.ToString(),
                        quality = vs.StatusCode.ToString(),
                        timestamp = vs.Timestamp
                    });
                }
                else
                {
                    results.Add(new
                    {
                        path = varPath,
                        value = (object?)null,
                        type = (string?)null,
                        quality = "NotFound",
                        timestamp = (DateTime?)null
                    });
                }
            }

            await SendJsonResponse(stream, 200, new { count = results.Count, variables = results });
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 400, new { error = ex.Message });
        }
    }

    private async Task HandleBatchWrite(NetworkStream stream, string body)
    {
        try
        {
            var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("values", out var valuesElem) ||
                valuesElem.ValueKind != JsonValueKind.Object)
            {
                await SendJsonResponse(stream, 400,
                    new { error = "Missing 'values' object in request body (e.g. {\"values\":{\"Folder.Tag\":42}})" });
                return;
            }

            var results = new List<object>();
            foreach (var prop in valuesElem.EnumerateObject())
            {
                var varPath = prop.Name;
                try
                {
                    var writeValue = ParseJsonValue(prop.Value);
                    if (writeValue != null)
                    {
                        _nodeManager.WriteVariable(varPath, writeValue);
                        results.Add(new { path = varPath, status = "ok" });
                    }
                    else
                    {
                        results.Add(new { path = varPath, status = "error", error = "null value" });
                    }
                }
                catch (Exception ex)
                {
                    results.Add(new { path = varPath, status = "error", error = ex.Message });
                }
            }

            await SendJsonResponse(stream, 200, new { count = results.Count, results });
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 400, new { error = ex.Message });
        }
    }
    // === Alarms =======================================================

    private async Task HandleGetAlarms(NetworkStream stream)
    {
        var alarms = _nodeManager.GetAlarmStates();
        await SendJsonResponse(stream, 200, new { count = alarms.Count, alarms });
    }

    private async Task HandleAcknowledgeAlarm(NetworkStream stream, string alarmPath, string body)
    {
        try
        {
            string? comment = null;
            if (!string.IsNullOrWhiteSpace(body))
            {
                var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("comment", out var commentElem))
                    comment = commentElem.GetString();
            }

            var ok = _nodeManager.AcknowledgeAlarm(alarmPath, comment);
            if (ok)
                await SendJsonResponse(stream, 200, new { path = alarmPath, status = "acknowledged" });
            else
                await SendJsonResponse(stream, 404, new { error = $"Alarm '{alarmPath}' not found" });
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 400, new { error = ex.Message });
        }
    }

    private async Task HandleShelveAlarm(NetworkStream stream, string alarmPath, string body)
    {
        try
        {
            int durationMinutes = 0;
            string username = "REST-API";

            if (!string.IsNullOrWhiteSpace(body))
            {
                var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("durationMinutes", out var durElem))
                    durationMinutes = durElem.GetInt32();
                if (doc.RootElement.TryGetProperty("username", out var userElem))
                    username = userElem.GetString() ?? "REST-API";
            }

            var ok = _nodeManager.ShelveAlarm(alarmPath, durationMinutes, username);
            if (ok)
                await SendJsonResponse(stream, 200, new { path = alarmPath, status = "shelved", durationMinutes, username });
            else
                await SendJsonResponse(stream, 400, new { error = $"Alarm '{alarmPath}' not found or shelving not allowed" });
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 400, new { error = ex.Message });
        }
    }

    private async Task HandleUnshelveAlarm(NetworkStream stream, string alarmPath, string body)
    {
        try
        {
            string username = "REST-API";

            if (!string.IsNullOrWhiteSpace(body))
            {
                var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("username", out var userElem))
                    username = userElem.GetString() ?? "REST-API";
            }

            var ok = _nodeManager.UnshelveAlarm(alarmPath, username);
            if (ok)
                await SendJsonResponse(stream, 200, new { path = alarmPath, status = "unshelved", username });
            else
                await SendJsonResponse(stream, 404, new { error = $"Alarm '{alarmPath}' not found" });
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 400, new { error = ex.Message });
        }
    }
    // === Recipes ======================================================

    private async Task HandleGetRecipes(NetworkStream stream)
    {
        var recipes = _nodeManager.GetRecipeNames();
        await SendJsonResponse(stream, 200, new { count = recipes.Count, recipes });
    }

    private async Task HandleExecuteRecipe(NetworkStream stream, string recipeName, string body)
    {
        try
        {
            string action = "Load";
            string target = "";

            if (!string.IsNullOrWhiteSpace(body))
            {
                var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("action", out var actElem))
                    action = actElem.GetString() ?? "Load";
                if (doc.RootElement.TryGetProperty("target", out var tgtElem))
                    target = tgtElem.GetString() ?? "";
            }

            _nodeManager.ExecuteRecipe(recipeName, action, target);
            await SendJsonResponse(stream, 200, new { recipe = recipeName, action, target, status = "executed" });
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 400, new { error = ex.Message });
        }
    }

    // === Reports ======================================================

    private async Task HandleGenerateReport(NetworkStream stream, string reportName)
    {
        try
        {
            var html = await _nodeManager.GenerateReportAsync(reportName);
            if (html != null)
            {
                await SendResponse(stream, 200, html, contentType: "text/html",
                    extraHeaders: $"Content-Disposition: inline; filename=\"{reportName}.html\"\r\n");
            }
            else
            {
                await SendJsonResponse(stream, 404, new { error = $"Report '{reportName}' not found or generation failed" });
            }
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 500, new { error = ex.Message });
        }
    }

    // === Redundancy ===================================================

    private async Task HandleGetRedundancy(NetworkStream stream)
    {
        var status = _nodeManager.GetRedundancyStatus();
        if (status != null)
            await SendJsonResponse(stream, 200, status);
        else
            await SendJsonResponse(stream, 200, new { status = "not configured" });
    }

    private async Task HandleForceRedundancy(NetworkStream stream, string body, bool active)
    {
        try
        {
            string reason = "REST API request";
            if (!string.IsNullOrWhiteSpace(body))
            {
                var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("reason", out var reasonElem))
                    reason = reasonElem.GetString() ?? "REST API request";
            }

            bool ok;
            if (active)
                ok = _nodeManager.ForceRedundancyActive(reason);
            else
                ok = _nodeManager.ForceRedundancyStandby(reason);

            if (ok)
                await SendJsonResponse(stream, 200, new { status = active ? "forced-active" : "forced-standby", reason });
            else
                await SendJsonResponse(stream, 400, new { error = "Redundancy not configured or operation failed" });
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 400, new { error = ex.Message });
        }
    }

    // === Diagnostics ==================================================

    private async Task HandleGetDiagnostics(NetworkStream stream)
    {
        try
        {
            var snapshot = DiagnosticsCollector.Instance.BuildSnapshot();
            await SendJsonResponse(stream, 200, snapshot);
        }
        catch (Exception ex)
        {
            await SendJsonResponse(stream, 500, new { error = ex.Message });
        }
    }
    // === Events / Audit ===============================================

    private async Task HandleGetEvents(NetworkStream stream, string query)
    {
        if (_eventLogger == null)
        {
            await SendJsonResponse(stream, 200, new { count = 0, events = Array.Empty<object>() });
            return;
        }

        var startStr = GetQueryParam(query, "start");
        var endStr = GetQueryParam(query, "end");
        var category = GetQueryParam(query, "category");
        var maxStr = GetQueryParam(query, "max");

        DateTime? start = startStr != null ? DateTime.Parse(startStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) : null;
        DateTime? end = endStr != null ? DateTime.Parse(endStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) : null;
        int max = maxStr != null && int.TryParse(maxStr, out var m) ? m : 500;

        var events = _eventLogger.QueryEvents(start, end, category, max);
        await SendJsonResponse(stream, 200, new { count = events.Count, events });
    }

    private async Task HandleGetAudit(NetworkStream stream, string query)
    {
        if (_auditLogger == null)
        {
            await SendJsonResponse(stream, 200, new { count = 0, records = Array.Empty<object>() });
            return;
        }

        var startStr = GetQueryParam(query, "start");
        var endStr = GetQueryParam(query, "end");
        var action = GetQueryParam(query, "action");
        var username = GetQueryParam(query, "username");
        var maxStr = GetQueryParam(query, "max");

        DateTime? start = startStr != null ? DateTime.Parse(startStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) : null;
        DateTime? end = endStr != null ? DateTime.Parse(endStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) : null;
        int max = maxStr != null && int.TryParse(maxStr, out var m) ? m : 500;

        var records = _auditLogger.Query(start, end, action, username, max);
        await SendJsonResponse(stream, 200, new { count = records.Count, records });
    }

    // === Export ========================================================

    private async Task HandleExportCsv(NetworkStream stream, string query)
    {
        var variable = GetQueryParam(query, "variable");
        var startStr = GetQueryParam(query, "start");
        var endStr = GetQueryParam(query, "end");
        var maxStr = GetQueryParam(query, "max");

        if (string.IsNullOrEmpty(variable))
        {
            await SendResponse(stream, 400, "Missing 'variable' query parameter", contentType: "text/plain");
            return;
        }

        var start = startStr != null
            ? DateTime.Parse(startStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
            : DateTime.UtcNow.AddHours(-1);
        var end = endStr != null
            ? DateTime.Parse(endStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
            : DateTime.UtcNow;
        int max = maxStr != null && int.TryParse(maxStr, out var m) ? m : 10000;

        var data = _nodeManager.ReadHistoricalValues(variable, start, end, max);
        if (data == null || data.Count == 0)
        {
            await SendResponse(stream, 200, "Timestamp,Value\r\n", contentType: "text/csv",
                extraHeaders: $"Content-Disposition: attachment; filename=\"{variable}_export.csv\"\r\n");
            return;
        }

        var sb = new StringBuilder("Timestamp,Value\r\n");
        foreach (var (ts, val) in data)
            sb.AppendLine($"{ts:o},{val.ToString("R", CultureInfo.InvariantCulture)}");

        await SendResponse(stream, 200, sb.ToString(), contentType: "text/csv",
            extraHeaders: $"Content-Disposition: attachment; filename=\"{variable}_export.csv\"\r\n");
    }

    private async Task HandleExportJson(NetworkStream stream, string query)
    {
        var variable = GetQueryParam(query, "variable");
        var startStr = GetQueryParam(query, "start");
        var endStr = GetQueryParam(query, "end");
        var maxStr = GetQueryParam(query, "max");

        if (string.IsNullOrEmpty(variable))
        {
            await SendJsonResponse(stream, 400, new { error = "Missing 'variable' query parameter" });
            return;
        }

        var start = startStr != null
            ? DateTime.Parse(startStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
            : DateTime.UtcNow.AddHours(-1);
        var end = endStr != null
            ? DateTime.Parse(endStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
            : DateTime.UtcNow;
        int max = maxStr != null && int.TryParse(maxStr, out var m) ? m : 10000;

        var data = _nodeManager.ReadHistoricalValues(variable, start, end, max);
        var points = data?.Select(d => new { timestamp = d.Timestamp, value = d.Value }).ToList()
            ?? new List<object>();

        await SendJsonResponse(stream, 200, new { variable, start, end, count = points.Count, data = points });
    }
    // === HTTP helpers =================================================

    private static object? ParseJsonValue(JsonElement elem)
    {
        return elem.ValueKind switch
        {
            JsonValueKind.Number when elem.TryGetDouble(out var d) => d,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => elem.GetString(),
            _ => elem.ToString()
        };
    }

    private static (string Method, string Path, string Query, string Body) ParseHttpRequest(string request)
    {
        var lines = request.Split('\n');
        var firstLine = lines[0].Trim();
        var parts = firstLine.Split(' ');
        var method = parts.Length > 0 ? parts[0] : "GET";
        var fullPath = parts.Length > 1 ? parts[1] : "/";

        var queryIdx = fullPath.IndexOf('?');
        var path = queryIdx >= 0 ? fullPath[..queryIdx] : fullPath;
        var query = queryIdx >= 0 ? fullPath[(queryIdx + 1)..] : "";

        // Find body after empty line
        var bodyIdx = request.IndexOf("\r\n\r\n");
        var body = bodyIdx >= 0 ? request[(bodyIdx + 4)..] : "";

        return (method, path, query, body);
    }

    private static string? GetQueryParam(string query, string key)
    {
        if (string.IsNullOrEmpty(query)) return null;
        foreach (var pair in query.Split('&'))
        {
            var eqIdx = pair.IndexOf('=');
            if (eqIdx > 0)
            {
                var k = Uri.UnescapeDataString(pair[..eqIdx]);
                if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
                    return Uri.UnescapeDataString(pair[(eqIdx + 1)..]);
            }
        }
        return null;
    }

    private static string? GetHeaderValue(string request, string headerName)
    {
        foreach (var line in request.Split('\n'))
        {
            if (line.StartsWith(headerName + ":", StringComparison.OrdinalIgnoreCase))
                return line[(headerName.Length + 1)..].Trim().TrimEnd('\r');
        }
        return null;
    }

    private static async Task SendJsonResponse(NetworkStream stream, int statusCode, object data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = false });
        await SendResponse(stream, statusCode, json, "application/json");
    }

    private static async Task SendResponse(NetworkStream stream, int statusCode, string body,
        string contentType = "text/plain", bool corsHeaders = false, string? extraHeaders = null)
    {
        var statusText = statusCode switch
        {
            200 => "OK",
            204 => "No Content",
            400 => "Bad Request",
            401 => "Unauthorized",
            404 => "Not Found",
            429 => "Too Many Requests",
            500 => "Internal Server Error",
            _ => "OK"
        };

        var bodyBytes = Encoding.UTF8.GetBytes(body);
        var sb = new StringBuilder();
        sb.Append($"HTTP/1.1 {statusCode} {statusText}\r\n");
        sb.Append($"Content-Type: {contentType}; charset=utf-8\r\n");
        sb.Append($"Content-Length: {bodyBytes.Length}\r\n");
        sb.Append("Access-Control-Allow-Origin: *\r\n");
        if (corsHeaders)
        {
            sb.Append("Access-Control-Allow-Methods: GET, POST, OPTIONS\r\n");
            sb.Append("Access-Control-Allow-Headers: Content-Type, X-API-Key\r\n");
        }
        if (extraHeaders != null) sb.Append(extraHeaders);
        sb.Append("Connection: close\r\n\r\n");

        var headerBytes = Encoding.UTF8.GetBytes(sb.ToString());
        await stream.WriteAsync(headerBytes);
        await stream.WriteAsync(bodyBytes);
    }

    public void Dispose()
    {
        _cts?.Cancel();
        try { _listener?.Stop(); } catch { }
        _cts?.Dispose();
        _rateLimiter?.Dispose();
    }
}