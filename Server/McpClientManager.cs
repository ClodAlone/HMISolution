// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Lightweight Model Context Protocol (MCP) <b>client</b> used to connect the HMI platform
    /// (primarily <see cref="AiAgentManager"/>) to external MCP tool servers configured via
    /// <see cref="McpConnectionConfig"/> (<c>NodeModel.McpConnections</c>).
    ///
    /// This is the client-side counterpart of <see cref="RestApiServer"/>'s MCP server support
    /// (see McpServer.cs): it speaks the same minimal JSON-RPC 2.0 "tools/list" / "tools/call"
    /// subset of the MCP spec over HTTP (SSE/streamable-HTTP style, one request per call).
    ///
    /// Only remote (Sse/Http) connections are supported by this lightweight client; Stdio
    /// connections are listed/validated but not yet spawned as local processes.
    /// </summary>
    public sealed class McpClientManager : IDisposable
    {
        private const string McpProtocolVersion = "2024-11-05";

        private readonly ConcurrentDictionary<string, McpConnectionConfig> _connections = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, List<McpToolInfo>> _toolCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(30) };
        private int _requestId;

        public void Initialize(List<McpConnectionConfig> connections)
        {
            _connections.Clear();
            _toolCache.Clear();
            foreach (var conn in connections.Where(c => c.Enabled))
            {
                _connections[conn.Name] = conn;
                DiagnosticsCollector.Instance.Register("McpConnection", conn.Name, conn.Enabled);
            }

            if (_connections.Count > 0)
                Log.Information("McpClientManager initialized with {Count} connection(s): {Names}",
                    _connections.Count, string.Join(", ", _connections.Keys));
        }

        /// <summary>
        /// Lists tools available on all configured (enabled, remote) MCP connections, qualified
        /// as "ConnectionName.toolName". Results are cached per-connection for the process lifetime;
        /// call <see cref="RefreshTools"/> to force a re-fetch.
        /// </summary>
        public async Task<List<string>> ListAllQualifiedToolNamesAsync()
        {
            var result = new List<string>();
            foreach (var conn in _connections.Values)
            {
                var tools = await GetOrFetchToolsAsync(conn);
                result.AddRange(tools.Select(t => $"{conn.Name}.{t.Name}"));
            }
            return result;
        }

        public void RefreshTools(string connectionName) => _toolCache.TryRemove(connectionName, out _);

        /// <summary>
        /// Calls a tool identified by its qualified name "ConnectionName.toolName" with a JSON
        /// arguments object, returning the tool's text result (or an error description).
        /// </summary>
        public async Task<string> CallToolAsync(string qualifiedToolName, string? jsonArguments, CancellationToken ct = default)
        {
            var dot = qualifiedToolName.IndexOf('.');
            if (dot <= 0) throw new InvalidOperationException($"Invalid MCP tool reference '{qualifiedToolName}' (expected 'Connection.tool').");

            var connName = qualifiedToolName[..dot];
            var toolName = qualifiedToolName[(dot + 1)..];

            if (!_connections.TryGetValue(connName, out var conn))
                throw new InvalidOperationException($"MCP connection '{connName}' is not configured or not enabled.");

            if (!string.Equals(conn.TransportType, "Sse", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(conn.TransportType, "Http", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException($"MCP connection '{connName}' uses transport '{conn.TransportType}', which is not supported by this client (only Sse/Http).");

            JsonElement? args = null;
            if (!string.IsNullOrWhiteSpace(jsonArguments))
            {
                try { args = JsonSerializer.Deserialize<JsonElement>(jsonArguments); }
                catch (Exception ex) { throw new InvalidOperationException($"Invalid JSON arguments for MCP tool call: {ex.Message}"); }
            }

            var callParams = new { name = toolName, arguments = args };
            var response = await SendJsonRpcAsync(conn, "tools/call", callParams, ct);

            if (response.TryGetProperty("error", out var err))
                throw new InvalidOperationException($"MCP tool '{qualifiedToolName}' returned error: {err.GetRawText()}");

            if (response.TryGetProperty("result", out var result))
            {
                if (result.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
                {
                    var texts = content.EnumerateArray()
                        .Where(c => c.TryGetProperty("type", out var t) && t.GetString() == "text")
                        .Select(c => c.TryGetProperty("text", out var tx) ? tx.GetString() ?? "" : "")
                        .Where(s => !string.IsNullOrEmpty(s));
                    return string.Join("\n", texts);
                }
                return result.GetRawText();
            }

            return "(no result)";
        }

        private async Task<List<McpToolInfo>> GetOrFetchToolsAsync(McpConnectionConfig conn)
        {
            if (_toolCache.TryGetValue(conn.Name, out var cached))
                return cached;

            var tools = new List<McpToolInfo>();
            try
            {
                if (string.Equals(conn.TransportType, "Sse", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(conn.TransportType, "Http", StringComparison.OrdinalIgnoreCase))
                {
                    var response = await SendJsonRpcAsync(conn, "tools/list", new { });
                    if (response.TryGetProperty("result", out var result) &&
                        result.TryGetProperty("tools", out var toolsEl) &&
                        toolsEl.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var t in toolsEl.EnumerateArray())
                        {
                            var name = t.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
                            var desc = t.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "";
                            if (!string.IsNullOrEmpty(name))
                                tools.Add(new McpToolInfo(name, desc));
                        }
                    }
                }
                else
                {
                    Log.Warning("MCP connection '{Name}': Stdio transport is configured but not supported by this lightweight client.", conn.Name);
                }
            }
            catch (Exception ex)
            {
                Log.Warning("MCP connection '{Name}': failed to list tools: {Error}", conn.Name, ex.Message);
            }

            _toolCache[conn.Name] = tools;
            return tools;
        }

        private async Task<JsonElement> SendJsonRpcAsync(McpConnectionConfig conn, string method, object @params, CancellationToken ct = default)
        {
            var id = Interlocked.Increment(ref _requestId);
            var request = new { jsonrpc = "2.0", id, method, @params };
            var json = JsonSerializer.Serialize(request);

            using var req = new HttpRequestMessage(HttpMethod.Post, conn.Endpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrWhiteSpace(conn.ApiKeyEnvVar))
            {
                var key = Environment.GetEnvironmentVariable(conn.ApiKeyEnvVar);
                if (!string.IsNullOrEmpty(key))
                    req.Headers.Add("Authorization", $"Bearer {key}");
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(Math.Max(1, conn.TimeoutSeconds)));

            using var resp = await _http.SendAsync(req, cts.Token);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync(cts.Token);
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.Clone();
        }

        public void Dispose() => _http.Dispose();

        private readonly record struct McpToolInfo(string Name, string Description);
    }
}
