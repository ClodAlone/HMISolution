using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SharedModels;
using Serilog;
using Opc.Ua;

namespace SimpleOpcFileServer;

/// <summary>
/// Lightweight REST API server for exposing variables, alarms, recipes, and audit trail
/// to external systems (MES, ERP, dashboards) via HTTP.
/// </summary>
public sealed class RestApiServer : IDisposable
{
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _listenerTask;
    private readonly string _apiKey;
    private readonly int _port;

    private SimpleFileServerNodeManager? _nodeManager;
    private EventManager? _eventManager;
    private RecipeManager? _recipeManager;
    private EventLogger? _eventLogger;
    private RedundancyService? _redundancyService;

    public RestApiServer(ApiConfig config)
    {
        _apiKey = config.ApiKey ?? "";
        _port = config.Port > 0 ? config.Port : 14842;
    }

    public void SetServices(
        SimpleFileServerNodeManager? nodeManager,
        EventManager? eventManager,
        RecipeManager? recipeManager,
        EventLogger? eventLogger,
        RedundancyService? redundancyService = null)
    {
        _nodeManager = nodeManager;
        _eventManager = eventManager;
        _recipeManager = recipeManager;
        _eventLogger = eventLogger;
        _redundancyService = redundancyService;
    }

    public void Start()
    {
        if (_port <= 0)
        {
            Log.Information("REST API server disabled (port = 0)");
            return;
        }

        try
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            _listenerTask = Task.Run(() => ListenLoop(_cts.Token));
            Log.Information("REST API server started on http://*:{Port}/api/", _port);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to start REST API server on port {Port}", _port);
        }
    }

    public void Stop()
    {
        _cts?.Cancel();
        try { _listener?.Stop(); } catch { }
        _listenerTask?.Wait(TimeSpan.FromSeconds(2));
    }

    public void Dispose()
    {
        Stop();
        _cts?.Dispose();
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
            catch (ObjectDisposedException) { break; }
            catch (Exception ex)
            {
                Log.Debug(ex, "REST API listener error");
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            using (client)
            {
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;

                var stream = client.GetStream();
                var ms = new MemoryStream();
                var buffer = new byte[8192];
                int bytesRead;
                int headerEnd = -1;

                do
                {
                    try { bytesRead = stream.Read(buffer, 0, buffer.Length); }
                    catch { bytesRead = 0; }
                    if (bytesRead == 0) break;
                    ms.Write(buffer, 0, bytesRead);
                    var soFar = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                    headerEnd = soFar.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                }
                while (headerEnd < 0 && ms.Length < 65536);

                var raw = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                if (headerEnd < 0) headerEnd = raw.Length;

                var headers = raw[..headerEnd];
                string body = (headerEnd + 4 <= raw.Length) ? raw[(headerEnd + 4)..] : "";

                if (headers.StartsWith("OPTIONS ", StringComparison.OrdinalIgnoreCase))
                {
                    SendResponse(stream, 204, "No Content", null, "text/plain");
                    return;
                }

                var lines = headers.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {
                    SendJsonError(stream, 400, "Bad Request");
                    return;
                }

                var requestLine = lines[0].Split(' ');
                if (requestLine.Length < 2)
                {
                    SendJsonError(stream, 400, "Bad Request");
                    return;
                }

                var method = requestLine[0].ToUpperInvariant();
                var fullPath = requestLine[1];
                var path = fullPath.Split('?')[0];
                var query = fullPath.Contains('?') ? fullPath.Split('?')[1] : "";

                if (method == "POST" || method == "PUT")
                {
                    int contentLength = 0;
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                        {
                            int.TryParse(line["Content-Length:".Length..].Trim(), out contentLength);
                            break;
                        }
                    }
                    while (Encoding.UTF8.GetByteCount(body) < contentLength && ms.Length < 1_048_576)
                    {
                        try { bytesRead = stream.Read(buffer, 0, buffer.Length); }
                        catch { bytesRead = 0; }
                        if (bytesRead == 0) break;
                        body += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }
                }

                if (!string.IsNullOrEmpty(_apiKey))
                {
                    var authenticated = false;

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("X-API-Key:", StringComparison.OrdinalIgnoreCase))
                        {
                            var key = line["X-API-Key:".Length..].Trim();
                            if (key == _apiKey)
                            {
                                authenticated = true;
                                break;
                            }
                        }
                    }

                    if (!authenticated && query.Contains("apiKey="))
                    {
                        var queryParams = query.Split('&');
                        foreach (var param in queryParams)
                        {
                            if (param.StartsWith("apiKey=", StringComparison.OrdinalIgnoreCase))
                            {
                                var key = param["apiKey=".Length..];
                                if (key == _apiKey)
                                {
                                    authenticated = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!authenticated)
                    {
                        SendJsonError(stream, 401, "Unauthorized");
                        return;
                    }
                }

                RouteRequest(stream, method, path, query, body);
            }
        }
        catch (Exception ex)
        {
            Log.Debug(ex, "REST API request handling error");
        }
    }

    private void RouteRequest(NetworkStream stream, string method, string path, string query, string body)
    {
        try
        {
            if (method == "GET" && path == "/api/health")
            {
                HandleHealth(stream);
                return;
            }

            if (path.StartsWith("/api/variables"))
            {
                HandleVariablesRequest(stream, method, path, query, body);
                return;
            }

            if (path.StartsWith("/api/alarms"))
            {
                HandleAlarmsRequest(stream, method, path, query, body);
                return;
            }

            if (path.StartsWith("/api/recipes"))
            {
                HandleRecipesRequest(stream, method, path, query, body);
                return;
            }

            if (method == "GET" && path == "/api/events")
            {
                HandleEventsRequest(stream, query);
                return;
            }

            if (method == "GET" && path == "/api/audit")
            {
                HandleAuditRequest(stream, query);
                return;
            }

            if (method == "GET" && path == "/api/diagnostics")
            {
                HandleDiagnosticsProxy(stream);
                return;
            }

            if (method == "GET" && path == "/api/redundancy")
            {
                HandleRedundancyProxy(stream);
                return;
            }

            if (method == "GET" && path == "/api/export/csv")
            {
                HandleExportCsv(stream, query);
                return;
            }

            SendJsonError(stream, 404, "Not Found");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "REST API route handler error: {Path}", path);
            SendJsonError(stream, 500, "Internal Server Error");
        }
    }

    private void HandleHealth(NetworkStream stream)
    {
        var response = new
        {
            status = "ok",
            timestamp = DateTime.UtcNow,
            services = new
            {
                nodeManager = _nodeManager != null,
                eventManager = _eventManager != null,
                recipeManager = _recipeManager != null,
                eventLogger = _eventLogger != null,
                redundancy = _redundancyService != null
            }
        };
        SendJsonResponse(stream, 200, "OK", response);
    }

    private void HandleVariablesRequest(NetworkStream stream, string method, string path, string query, string body)
    {
        if (_nodeManager == null)
        {
            SendJsonError(stream, 503, "Node manager not available");
            return;
        }

        if (method == "GET" && path == "/api/variables")
        {
            try
            {
                var variables = _nodeManager._variables.Select(kvp => new
                {
                    path = kvp.Key,
                    value = kvp.Value.Value,
                    dataType = kvp.Value.DataType?.ToString() ?? "Unknown",
                    timestamp = DateTime.UtcNow
                }).ToList();

                SendJsonResponse(stream, 200, "OK", new { variables });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "REST API: Error listing variables");
                SendJsonError(stream, 500, $"Internal error: {ex.Message}");
            }
            return;
        }

        if (method == "GET" && path.StartsWith("/api/variables/"))
        {
            var varPath = Uri.UnescapeDataString(path["/api/variables/".Length..]);
            try
            {
                var value = _nodeManager.ReadVariable(varPath);
                SendJsonResponse(stream, 200, "OK", new { path = varPath, value, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "REST API: Error reading variable '{Path}'", varPath);
                SendJsonError(stream, 404, $"Variable not found: {varPath}");
            }
            return;
        }

        if (method == "POST" && path == "/api/variables")
        {
            try
            {
                var request = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
                if (request == null || !request.ContainsKey("paths"))
                {
                    SendJsonError(stream, 400, "Request body must contain 'paths' array");
                    return;
                }

                var pathsJson = JsonSerializer.Serialize(request["paths"]);
                var paths = JsonSerializer.Deserialize<List<string>>(pathsJson);
                if (paths == null)
                {
                    SendJsonError(stream, 400, "Invalid 'paths' format");
                    return;
                }

                var results = paths.Select(p =>
                {
                    try
                    {
                        return new { path = p, value = _nodeManager.ReadVariable(p), success = true, error = (string?)null };
                    }
                    catch (Exception ex)
                    {
                        return new { path = p, value = (object?)null, success = false, error = ex.Message };
                    }
                }).ToList();

                SendJsonResponse(stream, 200, "OK", new { results });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "REST API: Error in batch read");
                SendJsonError(stream, 400, $"Invalid request: {ex.Message}");
            }
            return;
        }

        if (method == "PUT" && path.StartsWith("/api/variables/"))
        {
            var varPath = Uri.UnescapeDataString(path["/api/variables/".Length..]);
            try
            {
                var request = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
                if (request == null || !request.ContainsKey("value"))
                {
                    SendJsonError(stream, 400, "Request body must contain 'value'");
                    return;
                }

                var value = request["value"];
                _nodeManager.WriteVariable(varPath, value);
                SendJsonResponse(stream, 200, "OK", new { path = varPath, success = true });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "REST API: Error writing variable '{Path}'", varPath);
                SendJsonError(stream, 400, $"Write failed: {ex.Message}");
            }
            return;
        }

        if (method == "PUT" && path == "/api/variables")
        {
            try
            {
                var request = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
                if (request == null || !request.ContainsKey("writes"))
                {
                    SendJsonError(stream, 400, "Request body must contain 'writes' array");
                    return;
                }

                var writesJson = JsonSerializer.Serialize(request["writes"]);
                var writes = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(writesJson);
                if (writes == null)
                {
                    SendJsonError(stream, 400, "Invalid 'writes' format");
                    return;
                }

                var results = writes.Select(w =>
                {
                    try
                    {
                        var wPath = w["path"]?.ToString() ?? "";
                        var value = w["value"];
                        _nodeManager.WriteVariable(wPath, value);
                        return new { path = wPath, success = true, error = (string?)null };
                    }
                    catch (Exception ex)
                    {
                        return new { path = w.GetValueOrDefault("path")?.ToString() ?? "unknown", success = false, error = ex.Message };
                    }
                }).ToList();

                SendJsonResponse(stream, 200, "OK", new { results });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "REST API: Error in batch write");
                SendJsonError(stream, 400, $"Invalid request: {ex.Message}");
            }
            return;
        }

        SendJsonError(stream, 404, "Not Found");
    }

    private void HandleAlarmsRequest(NetworkStream stream, string method, string path, string query, string body)
    {
        if (_nodeManager == null)
        {
            SendJsonError(stream, 503, "Node manager not available");
            return;
        }

        if (method == "GET" && path == "/api/alarms")
        {
            try
            {
                var alarms = _nodeManager.GetActiveAlarms().Select(a => new
                {
                    id = a.NodeId.ToString(),
                    source = a.SourceName,
                    message = a.Message?.Value ?? "",
                    severity = a.Severity?.Value ?? 0,
                    acknowledged = a.AckedState?.Id?.Value ?? false,
                    activeTime = a.Time?.Value ?? DateTime.MinValue,
                    condition = a.ConditionName?.Value ?? ""
                }).ToList();

                SendJsonResponse(stream, 200, "OK", new { alarms });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "REST API: Error listing alarms");
                SendJsonError(stream, 500, $"Internal error: {ex.Message}");
            }
            return;
        }

        if (method == "POST" && path.Contains("/acknowledge"))
        {
            var parts = path.Split('/');
            if (parts.Length >= 4)
            {
                var alarmId = Uri.UnescapeDataString(parts[3]);
                try
                {
                    _nodeManager.AcknowledgeAlarm(alarmId);
                    SendJsonResponse(stream, 200, "OK", new { alarmId, acknowledged = true });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "REST API: Error acknowledging alarm '{AlarmId}'", alarmId);
                    SendJsonError(stream, 400, $"Acknowledge failed: {ex.Message}");
                }
                return;
            }
        }

        if (method == "POST" && path.Contains("/shelve"))
        {
            var parts = path.Split('/');
            if (parts.Length >= 4)
            {
                var alarmId = Uri.UnescapeDataString(parts[3]);
                try
                {
                    var durationMinutes = 60;
                    if (!string.IsNullOrEmpty(body))
                    {
                        var request = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
                        if (request != null && request.ContainsKey("durationMinutes"))
                        {
                            int.TryParse(request["durationMinutes"].ToString(), out durationMinutes);
                        }
                    }
                    _nodeManager.ShelveAlarm(alarmId, durationMinutes);
                    SendJsonResponse(stream, 200, "OK", new { alarmId, shelved = true, durationMinutes });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "REST API: Error shelving alarm '{AlarmId}'", alarmId);
                    SendJsonError(stream, 400, $"Shelve failed: {ex.Message}");
                }
                return;
            }
        }

        if (method == "POST" && path.Contains("/unshelve"))
        {
            var parts = path.Split('/');
            if (parts.Length >= 4)
            {
                var alarmId = Uri.UnescapeDataString(parts[3]);
                try
                {
                    _nodeManager.UnshelveAlarm(alarmId);
                    SendJsonResponse(stream, 200, "OK", new { alarmId, unshelved = true });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "REST API: Error unshelving alarm '{AlarmId}'", alarmId);
                    SendJsonError(stream, 400, $"Unshelve failed: {ex.Message}");
                }
                return;
            }
        }

        SendJsonError(stream, 404, "Not Found");
    }

    private void HandleRecipesRequest(NetworkStream stream, string method, string path, string query, string body)
    {
        if (_recipeManager == null)
        {
            SendJsonError(stream, 503, "Recipe manager not available");
            return;
        }

        if (method == "POST" && path.Contains("/execute"))
        {
            var parts = path.Split('/');
            if (parts.Length >= 4)
            {
                var recipeName = Uri.UnescapeDataString(parts[3]);
                try
                {
                    var request = JsonSerializer.Deserialize<Dictionary<string, object>>(body);
                    if (request == null || !request.ContainsKey("action") || !request.ContainsKey("targetRecipe"))
                    {
                        SendJsonError(stream, 400, "Request body must contain 'action' and 'targetRecipe'");
                        return;
                    }

                    var action = request["action"]?.ToString() ?? "";
                    var targetRecipe = request["targetRecipe"]?.ToString() ?? "";

                    _recipeManager.Execute(recipeName, action, targetRecipe);
                    SendJsonResponse(stream, 200, "OK", new { recipeName, action, targetRecipe, success = true });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "REST API: Error executing recipe '{RecipeName}'", recipeName);
                    SendJsonError(stream, 400, $"Recipe execution failed: {ex.Message}");
                }
                return;
            }
        }

        SendJsonError(stream, 404, "Not Found");
    }

    private void HandleEventsRequest(NetworkStream stream, string query)
    {
        if (_eventLogger == null)
        {
            SendJsonError(stream, 503, "Event logger not available");
            return;
        }

        try
        {
            var category = "";
            var severity = "";
            var limit = 100;
            var startTime = DateTime.MinValue;
            var endTime = DateTime.MaxValue;

            if (!string.IsNullOrEmpty(query))
            {
                var queryParams = query.Split('&');
                foreach (var param in queryParams)
                {
                    var kv = param.Split('=');
                    if (kv.Length == 2)
                    {
                        var key = kv[0];
                        var value = Uri.UnescapeDataString(kv[1]);

                        if (key == "category") category = value;
                        else if (key == "severity") severity = value;
                        else if (key == "limit") int.TryParse(value, out limit);
                        else if (key == "startTime") DateTime.TryParse(value, out startTime);
                        else if (key == "endTime") DateTime.TryParse(value, out endTime);
                    }
                }
            }

            var events = _eventLogger.QueryEvents(category, severity, startTime, endTime, limit);
            SendJsonResponse(stream, 200, "OK", new { events });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "REST API: Error querying events");
            SendJsonError(stream, 500, $"Internal error: {ex.Message}");
        }
    }

    private void HandleAuditRequest(NetworkStream stream, string query)
    {
        try
        {
            var audit = new List<object>();
            SendJsonResponse(stream, 200, "OK", new { audit, message = "Audit trail integration pending" });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "REST API: Error querying audit trail");
            SendJsonError(stream, 500, $"Internal error: {ex.Message}");
        }
    }

    private void HandleDiagnosticsProxy(NetworkStream stream)
    {
        try
        {
            var snapshot = DiagnosticsCollector.Instance.BuildSnapshot();
            SendJsonResponse(stream, 200, "OK", snapshot);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "REST API: Error getting diagnostics");
            SendJsonError(stream, 500, $"Internal error: {ex.Message}");
        }
    }

    private void HandleRedundancyProxy(NetworkStream stream)
    {
        if (_redundancyService == null)
        {
            SendJsonError(stream, 503, "Redundancy service not available");
            return;
        }

        try
        {
            var status = new
            {
                activeRole = _redundancyService.ActiveRole.ToString(),
                isActive = _redundancyService.IsActive,
                partnerAlive = _redundancyService.PartnerAlive,
                lastPartnerHeartbeat = _redundancyService.LastPartnerHeartbeat
            };
            SendJsonResponse(stream, 200, "OK", status);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "REST API: Error getting redundancy status");
            SendJsonError(stream, 500, $"Internal error: {ex.Message}");
        }
    }

    private void HandleExportCsv(NetworkStream stream, string query)
    {
        if (_eventLogger == null)
        {
            SendJsonError(stream, 503, "Event logger not available");
            return;
        }

        try
        {
            var category = "";
            var limit = 1000;

            if (!string.IsNullOrEmpty(query))
            {
                var queryParams = query.Split('&');
                foreach (var param in queryParams)
                {
                    var kv = param.Split('=');
                    if (kv.Length == 2)
                    {
                        var key = kv[0];
                        var value = Uri.UnescapeDataString(kv[1]);

                        if (key == "category") category = value;
                        else if (key == "limit") int.TryParse(value, out limit);
                    }
                }
            }

            var events = _eventLogger.QueryEvents(category, "", DateTime.MinValue, DateTime.MaxValue, limit);

            var csv = new StringBuilder();
            csv.AppendLine("Time,Category,Severity,Source,Message,Details");

            foreach (var evt in events)
            {
                var time = evt.GetValueOrDefault("time")?.ToString() ?? "";
                var cat = evt.GetValueOrDefault("category")?.ToString() ?? "";
                var sev = evt.GetValueOrDefault("severity")?.ToString() ?? "";
                var src = evt.GetValueOrDefault("source")?.ToString() ?? "";
                var msg = evt.GetValueOrDefault("message")?.ToString()?.Replace("\"", "\"\"") ?? "";
                var det = evt.GetValueOrDefault("details")?.ToString()?.Replace("\"", "\"\"") ?? "";

                csv.AppendLine($"{time},{cat},{sev},{src},\"{msg}\",\"{det}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            SendResponse(stream, 200, "OK", bytes, "text/csv");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "REST API: Error exporting CSV");
            SendJsonError(stream, 500, $"Internal error: {ex.Message}");
        }
    }

    private static void SendResponse(NetworkStream stream, int statusCode, string statusText, byte[]? bodyBytes, string contentType)
    {
        bodyBytes ??= Array.Empty<byte>();
        var header = $"HTTP/1.1 {statusCode} {statusText}\r\n" +
                     $"Content-Type: {contentType}\r\n" +
                     $"Access-Control-Allow-Origin: *\r\n" +
                     $"Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS\r\n" +
                     $"Access-Control-Allow-Headers: Content-Type, X-API-Key\r\n" +
                     $"Content-Length: {bodyBytes.Length}\r\n" +
                     $"Connection: close\r\n\r\n";
        stream.Write(Encoding.ASCII.GetBytes(header));
        if (bodyBytes.Length > 0)
            stream.Write(bodyBytes);
        stream.Flush();
    }

    private static void SendJsonResponse(NetworkStream stream, int statusCode, string statusText, object data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
        var bytes = Encoding.UTF8.GetBytes(json);
        SendResponse(stream, statusCode, statusText, bytes, "application/json");
    }

    private static void SendJsonError(NetworkStream stream, int statusCode, string message)
    {
        SendJsonResponse(stream, statusCode, statusCode == 200 ? "OK" : "Error", new { error = message });
    }
}