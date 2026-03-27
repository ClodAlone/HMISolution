using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Collects per-subsystem timing metrics and exposes them via a lightweight HTTP endpoint.
    /// Each subsystem calls <see cref="RecordCycle"/> after each execution cycle.
    /// </summary>
    public sealed class DiagnosticsCollector : IDisposable
    {
        private readonly ConcurrentDictionary<string, SubsystemMetrics> _metrics = new();
        private readonly ConcurrentDictionary<string, ProgramDebugInfo> _debugSnapshots = new();
        private TcpListener? _listener;
        private CancellationTokenSource? _cts;
        private Task? _listenerTask;
        private RateLimiter? _rateLimiter;

        // Process-level CPU tracking
        private TimeSpan _lastCpuTime;
        private DateTime _lastCpuSample;
        private double _cpuPercent;
        private bool _cpuInitialized;

        public static DiagnosticsCollector Instance { get; } = new();

        /// <summary>
        /// Configure rate limiting for the diagnostics endpoint.
        /// Call before Start() to enable throttling.
        /// </summary>
        public void ConfigureRateLimit(RateLimitConfig? config)
        {
            if (config is { Enabled: true, DiagnosticsMaxRequestsPerWindow: > 0 })
            {
                _rateLimiter = new RateLimiter(
                    config.DiagnosticsMaxRequestsPerWindow,
                    TimeSpan.FromSeconds(config.WindowSeconds));
                Log.Information("Diagnostics rate limiting enabled: {Max} requests per {Window}s",
                    config.DiagnosticsMaxRequestsPerWindow, config.WindowSeconds);
            }
        }

        /// <summary>
        /// Start the HTTP diagnostics endpoint on the given port. Port 0 = disabled.
        /// </summary>
        public void Start(int port)
        {
            if (port <= 0) return;

            try
            {
                _cts = new CancellationTokenSource();
                _listener = new TcpListener(IPAddress.Loopback, port);
                _listener.Start();
                _listenerTask = Task.Run(() => ListenLoop(_cts.Token));
                Log.Information("Diagnostics endpoint started on http://localhost:{Port}/", port);
            }
            catch (Exception ex)
            {
                Log.Warning("Diagnostics endpoint disabled: {Error}", ex.Message);
            }
        }
        /// <summary>
        /// Record a completed execution cycle for a subsystem.
        /// </summary>
        public void RecordCycle(string category, string name, double elapsedMs, bool enabled = true, string? error = null)
        {
            var key = $"{category}:{name}";
            var m = _metrics.GetOrAdd(key, _ => new SubsystemMetrics { Category = category, Name = name });
            m.Enabled = enabled;
            m.CycleCount++;
            m.LastCycleMs = elapsedMs;
            m.TotalMs += elapsedMs;
            if (elapsedMs > m.MaxCycleMs)
                m.MaxCycleMs = elapsedMs;
            if (error != null)
            {
                m.Status = "Error";
                m.LastError = error;
            }
            else
            {
                m.Status = "Running";
            }
        }

        /// <summary>
        /// Register a subsystem that doesn't cycle (e.g. a driver or logger that runs in the background).
        /// </summary>
        public void Register(string category, string name, bool enabled = true, string status = "Running")
        {
            var key = $"{category}:{name}";
            _metrics.GetOrAdd(key, _ => new SubsystemMetrics { Category = category, Name = name, Enabled = enabled, Status = status });
        }

        /// <summary>
        /// Update the status of a registered subsystem.
        /// </summary>
        public void SetStatus(string category, string name, string status, string? error = null)
        {
            var key = $"{category}:{name}";
            if (_metrics.TryGetValue(key, out var m))
            {
                m.Status = status;
                m.LastError = error;
            }
        }

        /// <summary>
        /// Record a debug snapshot for a running PLC program or script.
        /// </summary>
        public void RecordDebug(ProgramDebugInfo info)
        {
            var key = info.Category + ":" + info.Name;
            _debugSnapshots[key] = info;
        }
        public ServerDiagnostics BuildSnapshot()
        {
            SampleProcessCpu();

            var process = Process.GetCurrentProcess();
            var diag = new ServerDiagnostics
            {
                Timestamp = DateTime.UtcNow,
                ProcessCpuPercent = Math.Round(_cpuPercent, 1),
                MemoryMB = process.WorkingSet64 / (1024 * 1024),
                ThreadCount = process.Threads.Count
            };

            foreach (var kvp in _metrics)
            {
                var m = kvp.Value;
                diag.Subsystems.Add(new SubsystemDiagnostics
                {
                    Category = m.Category,
                    Name = m.Name,
                    Enabled = m.Enabled,
                    CycleCount = m.CycleCount,
                    AvgCycleMs = m.CycleCount > 0 ? Math.Round(m.TotalMs / m.CycleCount, 6) : 0,
                    MaxCycleMs = Math.Round(m.MaxCycleMs, 6),
                    LastCycleMs = Math.Round(m.LastCycleMs, 6),
                    TotalCpuMs = Math.Round(m.TotalMs, 3),
                    Status = m.Status,
                    LastError = m.LastError
                });
            }

            foreach (var kvp2 in _debugSnapshots)
            {
                var info = kvp2.Value;
                // Always refresh debug session from ScriptDebugger so paused state is visible
                // even when the script execution cycle is blocked at a breakpoint.
                if (info.Category == "Script")
                    info.DebugSession = ScriptDebugger.Instance.GetSessionInfo(info.Name);
                diag.ProgramDebug.Add(info);
            }

            return diag;
        }

        private void SampleProcessCpu()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var now = DateTime.UtcNow;
                var cpuTime = process.TotalProcessorTime;

                if (_cpuInitialized)
                {
                    var elapsed = (now - _lastCpuSample).TotalMilliseconds;
                    if (elapsed > 0)
                    {
                        var delta = (cpuTime - _lastCpuTime).TotalMilliseconds;
                        _cpuPercent = delta / (elapsed * Environment.ProcessorCount) * 100.0;
                        if (_cpuPercent < 0) _cpuPercent = 0;
                        if (_cpuPercent > 100) _cpuPercent = 100;
                    }
                }
                else
                {
                    _cpuInitialized = true;
                }

                _lastCpuTime = cpuTime;
                _lastCpuSample = now;
            }
            catch { }
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
                    Log.Debug("Diagnostics listener error: {Error}", ex.Message);
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                using (client)
                {
                    client.ReceiveTimeout = 2000;
                    client.SendTimeout = 2000;

                    var stream = client.GetStream();

                    // Read the full HTTP request headers + possibly body
                    var ms = new System.IO.MemoryStream();
                    var buffer = new byte[4096];
                    int bytesRead;
                    int headerEnd = -1;
                    do
                    {
                        try { bytesRead = stream.Read(buffer, 0, buffer.Length); } catch { bytesRead = 0; }
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

                    // Rate limiting per client IP
                    if (_rateLimiter != null)
                    {
                        var clientIp = ((IPEndPoint?)client.Client.RemoteEndPoint)?.Address.ToString() ?? "unknown";
                        if (!_rateLimiter.IsAllowed(clientIp))
                        {
                            Log.Warning("Diagnostics rate limit exceeded for {ClientIp}", clientIp);
                            var errorBody = "{\"error\":\"Too many requests\"}";
                            var errorBytes = Encoding.UTF8.GetBytes(errorBody);
                            var errorHeader = $"HTTP/1.1 429 Too Many Requests\r\nContent-Type: application/json\r\nContent-Length: {errorBytes.Length}\r\nConnection: close\r\n\r\n";
                            stream.Write(Encoding.ASCII.GetBytes(errorHeader));
                            stream.Write(errorBytes);
                            stream.Flush();
                            return;
                        }
                    }

                    // Handle CORS preflight
                    if (headers.StartsWith("OPTIONS ", StringComparison.OrdinalIgnoreCase))
                    {
                        var corsResp = "HTTP/1.1 204 No Content\r\nAccess-Control-Allow-Origin: *\r\nAccess-Control-Allow-Methods: GET, POST, OPTIONS\r\nAccess-Control-Allow-Headers: Content-Type\r\nConnection: close\r\n\r\n";
                        stream.Write(Encoding.ASCII.GetBytes(corsResp));
                        stream.Flush();
                        return;
                    }

                    // Route POST to debug command handler
                    if (headers.StartsWith("POST ", StringComparison.OrdinalIgnoreCase))
                    {
                        int contentLength = 0;
                        foreach (var line in headers.Split("\r\n"))
                        {
                            if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                            {
                                int.TryParse(line["Content-Length:".Length..].Trim(), out contentLength);
                                break;
                            }
                        }
                        while (Encoding.UTF8.GetByteCount(body) < contentLength)
                        {
                            try { bytesRead = stream.Read(buffer, 0, buffer.Length); } catch { bytesRead = 0; }
                            if (bytesRead == 0) break;
                            body += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        }
                        HandleDebugPost(headers, body, stream);
                        return;
                    }

                    // Default: GET - return diagnostics snapshot
                    var snapshot = BuildSnapshot();
                    var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(snapshot, DiagnosticsJsonContext.Default.ServerDiagnostics);

                    var header = $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nAccess-Control-Allow-Origin: *\r\nContent-Length: {jsonBytes.Length}\r\nConnection: close\r\n\r\n";
                    var headerBytes = Encoding.ASCII.GetBytes(header);

                    stream.Write(headerBytes, 0, headerBytes.Length);
                    stream.Write(jsonBytes, 0, jsonBytes.Length);
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Diagnostics response error: {Error}", ex.Message);
            }
        }


        private static readonly JsonSerializerOptions _debugJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        private void HandleDebugPost(string headers, string body, NetworkStream stream)
        {
            try
            {
                var firstLine = headers.Split('\n')[0];
                var parts = firstLine.Split(' ');
                var path = parts.Length > 1 ? parts[1] : "";

                Log.Debug("[ScriptDebug] POST {Path} body={Body}", path, body);

                string responseJson;

                if (path.StartsWith("/debug/breakpoints", StringComparison.OrdinalIgnoreCase))
                {
                    var req = JsonSerializer.Deserialize<SetBreakpointsRequest>(body, _debugJsonOptions);
                    if (req != null && !string.IsNullOrEmpty(req.ScriptName))
                    {
                        ScriptDebugger.Instance.SetBreakpoints(req.ScriptName, req.Lines ?? new());
                        responseJson = "{\"ok\":true}";
                    }
                    else
                    {
                        responseJson = "{\"error\":\"Invalid request\"}";
                    }
                }
                else if (path.StartsWith("/debug/command", StringComparison.OrdinalIgnoreCase))
                {
                    var req = JsonSerializer.Deserialize<DebugCommandRequest>(body, _debugJsonOptions);
                    if (req != null && !string.IsNullOrEmpty(req.ScriptName))
                    {
                        ScriptDebugger.Instance.SendCommand(req.ScriptName, req.Command);
                        responseJson = "{\"ok\":true}";
                    }
                    else
                    {
                        responseJson = "{\"error\":\"Invalid request\"}";
                    }
                }
                else
                {
                    responseJson = "{\"error\":\"Unknown debug endpoint\"}";
                }

                var responseBytes = Encoding.UTF8.GetBytes(responseJson);
                var respHeader = $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nAccess-Control-Allow-Origin: *\r\nContent-Length: {responseBytes.Length}\r\nConnection: close\r\n\r\n";
                stream.Write(Encoding.ASCII.GetBytes(respHeader));
                stream.Write(responseBytes);
                stream.Flush();
            }
            catch (Exception ex)
            {
                Log.Debug("Debug endpoint error: {Error}", ex.Message);
                var errBody = Encoding.UTF8.GetBytes("{\"error\":\"Internal error\"}");
                var errHeader = $"HTTP/1.1 500 Internal Server Error\r\nContent-Type: application/json\r\nAccess-Control-Allow-Origin: *\r\nContent-Length: {errBody.Length}\r\nConnection: close\r\n\r\n";
                try { stream.Write(Encoding.ASCII.GetBytes(errHeader)); stream.Write(errBody); stream.Flush(); } catch { }
            }
        }
        public void Dispose()
        {
            _cts?.Cancel();
            try { _listener?.Stop(); } catch { }
            _cts?.Dispose();
            _rateLimiter?.Dispose();
        }

        private class SubsystemMetrics
        {
            public string Category { get; set; } = "";
            public string Name { get; set; } = "";
            public bool Enabled { get; set; } = true;
            public long CycleCount;
            public double LastCycleMs;
            public double MaxCycleMs;
            public double TotalMs;
            public string Status = "Running";
            public string? LastError;
        }
    }

    [System.Text.Json.Serialization.JsonSerializable(typeof(ServerDiagnostics))]
    [System.Text.Json.Serialization.JsonSerializable(typeof(ScriptDebugSession))]
    [System.Text.Json.Serialization.JsonSerializable(typeof(ScriptDebugState))]
    [System.Text.Json.Serialization.JsonSerializable(typeof(ScriptBreakpoint))]
    internal partial class DiagnosticsJsonContext : System.Text.Json.Serialization.JsonSerializerContext { }
}
