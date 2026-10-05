// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Model Context Protocol (MCP) support for <see cref="RestApiServer"/>.
/// Exposes the same underlying server capabilities (variables, alarms, recipes,
/// events, diagnostics, redundancy) as MCP "tools" so that LLM clients (e.g. Claude
/// Desktop, IDE copilots) can discover and invoke them over a simple JSON-RPC 2.0
/// transport carried on HTTP POST /mcp.
///
/// This is a minimal "streamable HTTP" style transport: each request is a single
/// JSON-RPC object (or batch array) in the POST body, and the response is the
/// corresponding JSON-RPC result/error, matching the MCP specification's method
/// names (initialize, tools/list, tools/call) without requiring SSE for this
/// request/response-oriented use case.
/// </summary>
public sealed partial class RestApiServer
{
    private const string McpProtocolVersion = "2024-11-05";

    private void HandleMcpRequest(NetworkStream stream, string body)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                SendJsonResponse(stream, 200, "OK", JsonRpcError(null, -32700, "Parse error: empty body"));
                return;
            }

            using var doc = JsonDocument.Parse(body);

            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                var results = new List<object>();
                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    var result = DispatchJsonRpc(element);
                    if (result != null) results.Add(result);
                }
                SendJsonResponse(stream, 200, "OK", results);
                return;
            }

            var single = DispatchJsonRpc(doc.RootElement);
            // Notifications (no "id") produce no response body per JSON-RPC spec.
            SendJsonResponse(stream, 200, "OK", single ?? new { });
        }
        catch (JsonException ex)
        {
            SendJsonResponse(stream, 200, "OK", JsonRpcError(null, -32700, $"Parse error: {ex.Message}"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "MCP: Unhandled error processing request");
            SendJsonResponse(stream, 200, "OK", JsonRpcError(null, -32603, $"Internal error: {ex.Message}"));
        }
    }

    private object? DispatchJsonRpc(JsonElement request)
    {
        JsonElement idElement = default;
        bool hasId = request.TryGetProperty("id", out idElement);
        var id = hasId ? ExtractId(idElement) : null;

        if (!request.TryGetProperty("method", out var methodElement))
            return JsonRpcError(id, -32600, "Invalid Request: missing 'method'");

        var method = methodElement.GetString() ?? "";
        request.TryGetProperty("params", out var paramsElement);

        try
        {
            switch (method)
            {
                case "initialize":
                    return JsonRpcResult(id, BuildInitializeResult());

                case "notifications/initialized":
                case "initialized":
                    return null; // Notification: no response expected.

                case "ping":
                    return JsonRpcResult(id, new { });

                case "tools/list":
                    return JsonRpcResult(id, new { tools = GetMcpToolDefinitions() });

                case "tools/call":
                    return JsonRpcResult(id, CallMcpTool(paramsElement));

                default:
                    return JsonRpcError(id, -32601, $"Method not found: {method}");
            }
        }
        catch (McpToolException ex)
        {
            return JsonRpcResult(id, McpErrorToolResult(ex.Message));
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "MCP: Error handling method '{Method}'", method);
            return JsonRpcError(id, -32603, $"Internal error: {ex.Message}");
        }
    }

    private static object? ExtractId(JsonElement idElement)
    {
        return idElement.ValueKind switch
        {
            JsonValueKind.Number => idElement.GetInt64(),
            JsonValueKind.String => idElement.GetString(),
            _ => null
        };
    }

    private static object BuildInitializeResult() => new
    {
        protocolVersion = McpProtocolVersion,
        capabilities = new { tools = new { listChanged = false } },
        serverInfo = new { name = "hmi-server-mcp", version = "1.0.0" }
    };

    private static object JsonRpcResult(object? id, object result) => new
    {
        jsonrpc = "2.0",
        id,
        result
    };

    private static object JsonRpcError(object? id, int code, string message) => new
    {
        jsonrpc = "2.0",
        id,
        error = new { code, message }
    };

    // ─── Tool catalog ────────────────────────────────────────────────────────

    private static List<object> GetMcpToolDefinitions() => new()
    {
        McpTool("list_variables", "List all OPC UA variables currently exposed by the server, with their value and data type.",
            new JsonObject { ["type"] = "object", ["properties"] = new JsonObject() }),

        McpTool("read_variable", "Read the current value of a single variable by its path.",
            Schema(("path", "string", "Full variable path, e.g. 'Plant/Line1/Temperature'", required: true))),

        McpTool("read_variables", "Read the current values of multiple variables by path in one call.",
            new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["paths"] = new JsonObject { ["type"] = "array", ["items"] = new JsonObject { ["type"] = "string" }, ["description"] = "Variable paths to read" }
                },
                ["required"] = new JsonArray("paths")
            }),

        McpTool("write_variable", "Write a value to a single writable variable by path.",
            new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["path"] = new JsonObject { ["type"] = "string", ["description"] = "Full variable path" },
                    ["value"] = new JsonObject { ["description"] = "New value to write (string, number, or boolean)" }
                },
                ["required"] = new JsonArray("path", "value")
            }),

        McpTool("list_alarms", "List all currently active alarms/conditions.",
            new JsonObject { ["type"] = "object", ["properties"] = new JsonObject() }),

        McpTool("acknowledge_alarm", "Acknowledge an active alarm by its id.",
            Schema(("alarmId", "string", "Alarm NodeId string as returned by list_alarms", required: true))),

        McpTool("shelve_alarm", "Temporarily shelve (suppress) an alarm for a duration.",
            new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["alarmId"] = new JsonObject { ["type"] = "string", ["description"] = "Alarm NodeId string" },
                    ["durationMinutes"] = new JsonObject { ["type"] = "integer", ["description"] = "Shelve duration in minutes (default 60)" }
                },
                ["required"] = new JsonArray("alarmId")
            }),

        McpTool("unshelve_alarm", "Remove the shelve/suppression from an alarm.",
            Schema(("alarmId", "string", "Alarm NodeId string", required: true))),

        McpTool("execute_recipe", "Execute a recipe management action (Load, Save, Activate, Delete) against a target recipe.",
            new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["recipeName"] = new JsonObject { ["type"] = "string", ["description"] = "Name of the recipe definition" },
                    ["action"] = new JsonObject { ["type"] = "string", ["description"] = "One of: Load, Save, Activate, Delete" },
                    ["targetRecipe"] = new JsonObject { ["type"] = "string", ["description"] = "Name of the target recipe instance" }
                },
                ["required"] = new JsonArray("recipeName", "action", "targetRecipe")
            }),

        McpTool("query_events", "Query the server event/audit log with optional filters.",
            new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["category"] = new JsonObject { ["type"] = "string", ["description"] = "Event category filter (optional)" },
                    ["severity"] = new JsonObject { ["type"] = "string", ["description"] = "Event severity filter (optional)" },
                    ["limit"] = new JsonObject { ["type"] = "integer", ["description"] = "Max number of events to return (default 100)" },
                    ["startTime"] = new JsonObject { ["type"] = "string", ["description"] = "ISO-8601 start timestamp (optional)" },
                    ["endTime"] = new JsonObject { ["type"] = "string", ["description"] = "ISO-8601 end timestamp (optional)" }
                }
            }),

        McpTool("get_diagnostics", "Get server-wide diagnostics/performance snapshot (driver cycles, errors, timings).",
            new JsonObject { ["type"] = "object", ["properties"] = new JsonObject() }),

        McpTool("get_redundancy_status", "Get the high-availability redundancy status of this server instance.",
            new JsonObject { ["type"] = "object", ["properties"] = new JsonObject() }),
    };

    private static JsonObject Schema((string name, string type, string description, bool required) prop)
    {
        var obj = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                [prop.name] = new JsonObject { ["type"] = prop.type, ["description"] = prop.description }
            }
        };
        if (prop.required)
            obj["required"] = new JsonArray(prop.name);
        return obj;
    }

    private static object McpTool(string name, string description, JsonObject inputSchema) => new
    {
        name,
        description,
        inputSchema
    };

    // ─── Tool dispatch ───────────────────────────────────────────────────────

    private sealed class McpToolException : Exception
    {
        public McpToolException(string message) : base(message) { }
    }

    private static object McpTextResult(object payload) => new
    {
        content = new object[] { new { type = "text", text = JsonSerializer.Serialize(payload) } },
        isError = false
    };

    private static object McpErrorToolResult(string message) => new
    {
        content = new object[] { new { type = "text", text = message } },
        isError = true
    };

    private object CallMcpTool(JsonElement paramsElement)
    {
        if (paramsElement.ValueKind != JsonValueKind.Object || !paramsElement.TryGetProperty("name", out var nameEl))
            throw new McpToolException("Invalid tool call: missing 'name'");

        var toolName = nameEl.GetString() ?? "";
        var args = paramsElement.TryGetProperty("arguments", out var argsEl) && argsEl.ValueKind == JsonValueKind.Object
            ? argsEl
            : default;

        return toolName switch
        {
            "list_variables" => McpTextResult(ToolListVariables()),
            "read_variable" => McpTextResult(ToolReadVariable(RequireString(args, "path"))),
            "read_variables" => McpTextResult(ToolReadVariables(RequireStringArray(args, "paths"))),
            "write_variable" => McpTextResult(ToolWriteVariable(RequireString(args, "path"), GetRawValue(args, "value"))),
            "list_alarms" => McpTextResult(ToolListAlarms()),
            "acknowledge_alarm" => McpTextResult(ToolAcknowledgeAlarm(RequireString(args, "alarmId"))),
            "shelve_alarm" => McpTextResult(ToolShelveAlarm(RequireString(args, "alarmId"), GetOptionalInt(args, "durationMinutes", 60))),
            "unshelve_alarm" => McpTextResult(ToolUnshelveAlarm(RequireString(args, "alarmId"))),
            "execute_recipe" => McpTextResult(ToolExecuteRecipe(RequireString(args, "recipeName"), RequireString(args, "action"), RequireString(args, "targetRecipe"))),
            "query_events" => McpTextResult(ToolQueryEvents(args)),
            "get_diagnostics" => McpTextResult(ToolGetDiagnostics()),
            "get_redundancy_status" => McpTextResult(ToolGetRedundancyStatus()),
            _ => throw new McpToolException($"Unknown tool: {toolName}")
        };
    }

    private static string RequireString(JsonElement args, string name)
    {
        if (args.ValueKind != JsonValueKind.Object || !args.TryGetProperty(name, out var el) || el.ValueKind != JsonValueKind.String)
            throw new McpToolException($"Missing required string argument '{name}'");
        return el.GetString() ?? "";
    }

    private static List<string> RequireStringArray(JsonElement args, string name)
    {
        if (args.ValueKind != JsonValueKind.Object || !args.TryGetProperty(name, out var el) || el.ValueKind != JsonValueKind.Array)
            throw new McpToolException($"Missing required array argument '{name}'");
        return el.EnumerateArray().Select(e => e.GetString() ?? "").ToList();
    }

    private static int GetOptionalInt(JsonElement args, string name, int fallback)
    {
        if (args.ValueKind == JsonValueKind.Object && args.TryGetProperty(name, out var el) && el.ValueKind == JsonValueKind.Number && el.TryGetInt32(out var v))
            return v;
        return fallback;
    }

    private static string? GetOptionalString(JsonElement args, string name)
    {
        if (args.ValueKind == JsonValueKind.Object && args.TryGetProperty(name, out var el) && el.ValueKind == JsonValueKind.String)
            return el.GetString();
        return null;
    }

    private static object? GetRawValue(JsonElement args, string name)
    {
        if (args.ValueKind != JsonValueKind.Object || !args.TryGetProperty(name, out var el))
            throw new McpToolException($"Missing required argument '{name}'");

        return el.ValueKind switch
        {
            JsonValueKind.String => el.GetString(),
            JsonValueKind.Number => el.TryGetInt64(out var l) ? l : el.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => el.GetRawText()
        };
    }

    // ─── Tool implementations (thin wrappers over existing server services) ───

    private object ToolListVariables()
    {
        if (_nodeManager == null) throw new McpToolException("Node manager not available");
        var variables = _nodeManager._variables.Select(kvp => new
        {
            path = kvp.Key,
            value = kvp.Value.Value,
            dataType = kvp.Value.DataType?.ToString() ?? "Unknown"
        }).ToList();
        return new { variables };
    }

    private object ToolReadVariable(string path)
    {
        if (_nodeManager == null) throw new McpToolException("Node manager not available");
        try
        {
            var value = _nodeManager.ReadVariable(path);
            return new { path, value };
        }
        catch (Exception ex)
        {
            throw new McpToolException($"Failed to read '{path}': {ex.Message}");
        }
    }

    private object ToolReadVariables(List<string> paths)
    {
        if (_nodeManager == null) throw new McpToolException("Node manager not available");
        var results = paths.Select(p =>
        {
            try { return new { path = p, value = _nodeManager.ReadVariable(p), success = true, error = (string?)null }; }
            catch (Exception ex) { return new { path = p, value = (object?)null, success = false, error = (string?)ex.Message }; }
        }).ToList();
        return new { results };
    }

    private object ToolWriteVariable(string path, object? value)
    {
        if (_nodeManager == null) throw new McpToolException("Node manager not available");
        try
        {
            _nodeManager.WriteVariable(path, value ?? "");
            return new { path, success = true };
        }
        catch (Exception ex)
        {
            throw new McpToolException($"Failed to write '{path}': {ex.Message}");
        }
    }

    private object ToolListAlarms()
    {
        if (_nodeManager == null) throw new McpToolException("Node manager not available");
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
        return new { alarms };
    }

    private object ToolAcknowledgeAlarm(string alarmId)
    {
        if (_nodeManager == null) throw new McpToolException("Node manager not available");
        try
        {
            _nodeManager.AcknowledgeAlarm(alarmId);
            return new { alarmId, acknowledged = true };
        }
        catch (Exception ex)
        {
            throw new McpToolException($"Failed to acknowledge alarm '{alarmId}': {ex.Message}");
        }
    }

    private object ToolShelveAlarm(string alarmId, int durationMinutes)
    {
        if (_nodeManager == null) throw new McpToolException("Node manager not available");
        try
        {
            _nodeManager.ShelveAlarm(alarmId, durationMinutes);
            return new { alarmId, shelved = true, durationMinutes };
        }
        catch (Exception ex)
        {
            throw new McpToolException($"Failed to shelve alarm '{alarmId}': {ex.Message}");
        }
    }

    private object ToolUnshelveAlarm(string alarmId)
    {
        if (_nodeManager == null) throw new McpToolException("Node manager not available");
        try
        {
            _nodeManager.UnshelveAlarm(alarmId);
            return new { alarmId, unshelved = true };
        }
        catch (Exception ex)
        {
            throw new McpToolException($"Failed to unshelve alarm '{alarmId}': {ex.Message}");
        }
    }

    private object ToolExecuteRecipe(string recipeName, string action, string targetRecipe)
    {
        if (_recipeManager == null) throw new McpToolException("Recipe manager not available");
        try
        {
            _recipeManager.Execute(recipeName, action, targetRecipe);
            return new { recipeName, action, targetRecipe, success = true };
        }
        catch (Exception ex)
        {
            throw new McpToolException($"Recipe execution failed: {ex.Message}");
        }
    }

    private object ToolQueryEvents(JsonElement args)
    {
        if (_eventLogger == null) throw new McpToolException("Event logger not available");

        var category = GetOptionalString(args, "category") ?? "";
        var severity = GetOptionalString(args, "severity") ?? "";
        var limit = GetOptionalInt(args, "limit", 100);
        var startTimeStr = GetOptionalString(args, "startTime");
        var endTimeStr = GetOptionalString(args, "endTime");

        var startTime = (!string.IsNullOrEmpty(startTimeStr) && DateTime.TryParse(startTimeStr, out var st)) ? st : DateTime.MinValue;
        var endTime = (!string.IsNullOrEmpty(endTimeStr) && DateTime.TryParse(endTimeStr, out var et)) ? et : DateTime.MaxValue;

        var events = _eventLogger.QueryEvents(category, severity, startTime, endTime, limit);
        return new { events };
    }

    private object ToolGetDiagnostics()
    {
        return DiagnosticsCollector.Instance.BuildSnapshot();
    }

    private object ToolGetRedundancyStatus()
    {
        if (_redundancyService == null) throw new McpToolException("Redundancy service not available");
        return new
        {
            activeRole = _redundancyService.ActiveRole.ToString(),
            isActive = _redundancyService.IsActive,
            partnerAlive = _redundancyService.PartnerAlive,
            lastPartnerHeartbeat = _redundancyService.LastPartnerHeartbeat
        };
    }
}
