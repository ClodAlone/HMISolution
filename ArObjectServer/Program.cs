// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using ArObjectServer;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Minimal footprint — this server exists purely to keep AR detection latency low by running
// entirely on the local machine/LAN next to the camera client (no round trip to the cloud
// or to the main OPC UA server process).
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 20 * 1024 * 1024; // 20 MB per frame is generous headroom
});

// Resolve model path: first CLI arg, else "yolov8n.onnx" beside the executable.
string modelPath = args.FirstOrDefault(a => !a.StartsWith("-")) ?? "yolov8n.onnx";
if (!Path.IsPathRooted(modelPath))
    modelPath = Path.Combine(AppContext.BaseDirectory, modelPath);

bool useCuda = args.Any(a => a.Equals("--cuda", StringComparison.OrdinalIgnoreCase));
int port = 8098; // 8090 is excluded from Windows' dynamic port range on many machines (Hyper-V/WSL NAT reservations)
var portArg = args.FirstOrDefault(a => a.StartsWith("--port="));
if (portArg != null && int.TryParse(portArg["--port=".Length..], out var parsedPort))
    port = parsedPort;

// Default to loopback-only. Binding a wildcard host (http://+:port) requires an admin-level
// URL ACL reservation on Windows and fails with SocketException 10013 for standard users.
// Pass --lan to also expose the server on the local network (e.g. a phone/tablet AR client
// on the same LAN as this machine) — this still requires running `netsh http add urlacl`
// (or an elevated prompt) once on Windows to grant the reservation.
bool lanAccess = args.Any(a => a.Equals("--lan", StringComparison.OrdinalIgnoreCase));
if (lanAccess)
    builder.WebHost.UseUrls($"http://127.0.0.1:{port}", $"http://+:{port}");
else
    builder.WebHost.UseUrls($"http://127.0.0.1:{port}");

var app = builder.Build();

ArDetectionEngine? engine = null;
var logger = app.Services.GetRequiredService<ILogger<Program>>();

if (File.Exists(modelPath))
{
    try
    {
        engine = new ArDetectionEngine(modelPath, useCuda, 0, 0.5f, null, logger);
        logger.LogInformation("Local AR object-recognition server ready. Model: {Path}", modelPath);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to load AR detection model at {Path}", modelPath);
    }
}
else
{
    logger.LogWarning("AR detection model not found at {Path}. /detect will return 503 until a model is available.", modelPath);
}

// ─── Health check ──────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new { status = engine != null ? "ready" : "no-model", modelPath }));

// ─── Single-frame detection endpoint ───────────────────────
// The AR client (browser camera / desktop webcam capture) posts one JPEG frame at a time
// and gets bounding boxes back immediately. Kept stateless and synchronous per-request to
// minimize latency; the client controls its own frame rate (AugmentedRealityConfig.TargetFps).
app.MapPost("/detect", async (HttpRequest request) =>
{
    if (engine == null)
        return Results.Problem("Detection model not loaded", statusCode: 503);

    if (!request.HasFormContentType && request.ContentType?.StartsWith("image/") != true)
        return Results.BadRequest("Expected an image/* body or multipart form with a 'frame' file field.");

    byte[] bytes;
    float? confidence = null;
    bool decodeMarkers = true;
    if (request.Query.TryGetValue("decodeMarkers", out var decodeVal) && bool.TryParse(decodeVal, out var dm))
        decodeMarkers = dm;

    if (request.HasFormContentType)
    {
        var form = await request.ReadFormAsync();
        var file = form.Files["frame"] ?? form.Files.FirstOrDefault();
        if (file == null) return Results.BadRequest("No 'frame' file provided.");
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        bytes = ms.ToArray();

        if (form.TryGetValue("minConfidence", out var confVal) && float.TryParse(confVal, out var c))
            confidence = c;
    }
    else
    {
        using var ms = new MemoryStream();
        await request.Body.CopyToAsync(ms);
        bytes = ms.ToArray();

        if (request.Query.TryGetValue("minConfidence", out var confVal) && float.TryParse(confVal, out var c))
            confidence = c;
    }

    try
    {
        bool includeTiming = request.Query.TryGetValue("debugTiming", out var dtVal) && bool.TryParse(dtVal, out var dt) && dt;
        var (detections, width, height) = engine.Detect(bytes, confidence, decodeMarkers, out var timing);

        if (timing.TotalMs > 150)
        {
            logger.LogWarning(
                "AR detect slow frame: total={Total}ms decode={Decode}ms preprocess={Pre}ms inference={Inf}ms postprocess={Post}ms markers={Markers}ms",
                timing.TotalMs, timing.DecodeMs, timing.PreprocessMs, timing.InferenceMs, timing.PostprocessMs, timing.MarkerDecodeMs);
        }

        return Results.Ok(new
        {
            width,
            height,
            timestampUtc = DateTime.UtcNow,
            timingMs = includeTiming ? timing : null,
            detections = detections.Select(d => new
            {
                label = d.Label,
                confidence = Math.Round(d.Confidence, 4),
                x = Math.Round(d.X, 1),
                y = Math.Round(d.Y, 1),
                width = Math.Round(d.W, 1),
                height = Math.Round(d.H, 1),
                classId = d.ClassId,
                instanceKey = d.InstanceKey
            })
        });
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "AR detection failed for incoming frame");
        return Results.Problem("Detection failed: " + ex.Message, statusCode: 500);
    }
});

app.Run();
