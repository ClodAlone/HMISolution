using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedModels;

namespace SimpleOpcFileServer;

/// <summary>
/// Background service that starts IP camera capture, YOLO detection, and serves MJPEG streams
/// via a lightweight HTTP listener on port 8088.
/// </summary>
public class CameraHostedService : BackgroundService
{
    private readonly ServerConfig _serverConfig;
    private readonly ILogger<CameraHostedService> _logger;
    private readonly CameraStreamService _cameraService;
    private HttpListener? _httpListener;
    private SimpleFileServerNodeManager? _nodeManager;

    // Tracks last detection timestamp per prefix for Alive timeout
    private readonly ConcurrentDictionary<string, DateTime> _lastDetectionTime = new();
    private readonly ConcurrentDictionary<string, int> _detectionTimeouts = new();
    private Timer? _aliveTimer;

    /// <summary>Port for the MJPEG streaming HTTP server.</summary>
    public int StreamPort { get; set; } = 8088;

    public CameraHostedService(
        ServerConfig serverConfig,
        ILogger<CameraHostedService> logger,
        CameraStreamService cameraService)
    {
        _serverConfig = serverConfig;
        _logger = logger;
        _cameraService = cameraService;
    }

    /// <summary>Set the node manager reference for writing detection variables.</summary>
    public void SetNodeManager(SimpleFileServerNodeManager nodeManager)
    {
        _nodeManager = nodeManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait a bit for the OPC server to start and load the config
        await Task.Delay(3000, stoppingToken);

        // Load camera configs
        var cameras = LoadCameraConfigs();
        if (cameras.Count == 0)
        {
            _logger.LogInformation("No cameras configured. Camera service idle.");
            return;
        }

        // Start cameras
        foreach (var cam in cameras)
        {
            if (!string.IsNullOrEmpty(cam.DetectionVariablePrefix) && cam.DetectionTimeoutSeconds > 0)
                _detectionTimeouts[cam.DetectionVariablePrefix] = cam.DetectionTimeoutSeconds;

            _cameraService.StartCamera(cam, (prefix, label, confidence, count) =>
            {
                WriteDetectionVariables(prefix, label, confidence, count);
            },
            varPath => ReadBoolVariable(varPath));
            _logger.LogInformation("Camera started: {Id} ({Protocol}) -> {Url}", cam.CameraId, cam.Protocol, cam.Url);
        }

        // Start periodic timer to reset Alive when detections stop
        _aliveTimer = new Timer(CheckDetectionAlive, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        // Start HTTP listener for MJPEG streams
        try
        {
            _httpListener = new HttpListener();
            try
            {
                // Try wildcard binding first (allows remote access, requires admin or URL ACL)
                _httpListener.Prefixes.Add($"http://+:{StreamPort}/");
                _httpListener.Start();
            }
            catch (HttpListenerException)
            {
                // Fall back to localhost binding (no admin required)
                _httpListener.Close();
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add($"http://localhost:{StreamPort}/");
                _httpListener.Start();
                _logger.LogInformation("Camera stream bound to localhost only (run as admin or register URL ACL for remote access)");
            }
            _logger.LogInformation("Camera MJPEG stream server listening on port {Port}", StreamPort);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var context = await _httpListener.GetContextAsync().WaitAsync(stoppingToken);
                    _ = HandleRequestAsync(context, stoppingToken);
                }
                catch (OperationCanceledException) { break; }
                catch (HttpListenerException) when (stoppingToken.IsCancellationRequested) { break; }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "HTTP listener error");
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start camera HTTP listener on port {Port}", StreamPort);
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context, CancellationToken ct)
    {
        var path = context.Request.Url?.AbsolutePath?.TrimStart('/') ?? "";

        // Routes:
        // /camera/{cameraId}/stream  -> MJPEG stream
        // /camera/{cameraId}/snapshot -> single JPEG frame
        // /cameras -> JSON list of camera IDs

        if (path == "cameras")
        {
            var ids = _cameraService.GetCameraIds().ToList();
            var json = JsonSerializer.Serialize(ids);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.ContentLength64 = bytes.Length;
            await context.Response.OutputStream.WriteAsync(bytes, ct);
            context.Response.Close();
            return;
        }

        var segments = path.Split('/');
        if (segments.Length >= 3 && segments[0] == "camera")
        {
            var cameraId = segments[1];
            var action = segments[2];

            if (action == "stream")
            {
                context.Response.ContentType = _cameraService.GetMjpegContentType();
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Cache-Control", "no-cache");
                context.Response.SendChunked = true;

                try
                {
                    await _cameraService.WriteMjpegStreamAsync(cameraId, context.Response.OutputStream, ct);
                }
                catch { }
                finally
                {
                    try { context.Response.Close(); } catch { }
                }
                return;
            }

            if (action == "snapshot")
            {
                var frame = _cameraService.GetLatestFrame(cameraId);
                if (frame != null)
                {
                    context.Response.ContentType = "image/jpeg";
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.ContentLength64 = frame.Length;
                    await context.Response.OutputStream.WriteAsync(frame, ct);
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
                context.Response.Close();
                return;
            }
        }

        context.Response.StatusCode = 404;
        context.Response.Close();
    }

    private List<CameraConfig> LoadCameraConfigs()
    {
        try
        {
            if (!File.Exists(_serverConfig.NodesConfigFile)) return [];

            using var stream = File.OpenRead(_serverConfig.NodesConfigFile);
            var model = JsonSerializer.Deserialize(stream, ServerJsonContext.Default.NodeModel);

            List<CameraConfig> cameras;
            if (model?.Cameras != null && model.Cameras.Count > 0)
            {
                cameras = model.Cameras;
            }
            else
            {
                // Also scan screens for ipcamera symbols with inline camera configs
                cameras = new List<CameraConfig>();
                if (model?.Screens != null)
                {
                    foreach (var screen in model.Screens)
                    {
                        foreach (var sym in screen.Symbols)
                        {
                            if (sym.Type == "ipcamera" && sym.Camera != null && !string.IsNullOrEmpty(sym.Camera.CameraId))
                            {
                                if (!cameras.Any(c => c.CameraId == sym.Camera.CameraId))
                                    cameras.Add(sym.Camera);
                            }
                        }
                    }
                }
            }

            // Resolve YOLO model paths relative to the project directory
            var projectDir = Path.GetDirectoryName(Path.GetFullPath(_serverConfig.NodesConfigFile));
            if (projectDir != null)
            {
                foreach (var cam in cameras)
                {
                    var modelFile = string.IsNullOrEmpty(cam.YoloModelPath) ? "yolov8n.onnx" : cam.YoloModelPath;
                    if (!Path.IsPathRooted(modelFile))
                    {
                        // Check project directory first, then fall back to working directory
                        var projectRelative = Path.Combine(projectDir, modelFile);
                        if (File.Exists(projectRelative))
                        {
                            cam.YoloModelPath = Path.GetFullPath(projectRelative);
                        }
                        else if (File.Exists(modelFile))
                        {
                            cam.YoloModelPath = Path.GetFullPath(modelFile);
                        }
                        // else leave as-is; CameraStreamService will log "not found"
                    }
                }
            }

            return cameras;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load camera configs");
            return [];
        }
    }

    /// <summary>
    /// Reads a Boolean OPC variable by path. Used by CameraInstance to check YoloEnableVariable.
    /// </summary>
    private bool ReadBoolVariable(string variablePath)
    {
        if (_nodeManager == null) return true;
        try
        {
            var value = _nodeManager.ReadVariable(variablePath);
            return Convert.ToBoolean(value);
        }
        catch
        {
            return true; // variable not found — default to enabled
        }
    }

    private void WriteDetectionVariables(string prefix, string label, double confidence, int count)
    {
        if (_nodeManager == null) return;

        try
        {
            if (_nodeManager._variables.ContainsKey($"{prefix}.Label"))
                _nodeManager.WriteVariable($"{prefix}.Label", label);
            if (_nodeManager._variables.ContainsKey($"{prefix}.Confidence"))
                _nodeManager.WriteVariable($"{prefix}.Confidence", Math.Round(confidence, 4));
            if (_nodeManager._variables.ContainsKey($"{prefix}.Count"))
                _nodeManager.WriteVariable($"{prefix}.Count", count);

            // Update alive heartbeat — mark as alive whenever a detection callback fires
            if (_nodeManager._variables.ContainsKey($"{prefix}.Alive"))
            {
                _lastDetectionTime[prefix] = DateTime.UtcNow;
                _nodeManager.WriteVariable($"{prefix}.Alive", true);
            }
        }
        catch (Exception)
        {
            // Variable might not exist yet — ignore
        }
    }

    /// <summary>
    /// Periodically checks each camera prefix; if no detection has arrived within
    /// the configured timeout, sets the Alive variable to false.
    /// </summary>
    private void CheckDetectionAlive(object? state)
    {
        if (_nodeManager == null) return;

        var now = DateTime.UtcNow;
        foreach (var kvp in _detectionTimeouts)
        {
            var prefix = kvp.Key;
            var timeoutSec = kvp.Value;

            if (_lastDetectionTime.TryGetValue(prefix, out var lastTime))
            {
                if ((now - lastTime).TotalSeconds >= timeoutSec)
                {
                    try
                    {
                        if (_nodeManager._variables.ContainsKey($"{prefix}.Alive"))
                            _nodeManager.WriteVariable($"{prefix}.Alive", false);
                    }
                    catch { }
                }
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _aliveTimer?.Dispose();
        _httpListener?.Stop();
        _cameraService.Dispose();
        return base.StopAsync(cancellationToken);
    }
}
