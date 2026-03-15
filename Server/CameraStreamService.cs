using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SharedModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace SimpleOpcFileServer;

/// <summary>
/// Captures frames from IP cameras (RTSP via FFmpeg, MJPEG streams, HTTP snapshots),
/// optionally runs YOLOv8 ONNX object detection, and serves MJPEG streams to viewers.
/// </summary>
public class CameraStreamService : IDisposable
{
    private readonly ILogger<CameraStreamService> _logger;
    private readonly ConcurrentDictionary<string, CameraInstance> _cameras = new();

    public CameraStreamService(ILogger<CameraStreamService> logger)
    {
        _logger = logger;
    }

    public void StartCamera(CameraConfig config, Action<string, string, double, int>? onDetection = null)
    {
        if (_cameras.ContainsKey(config.CameraId)) return;
        var instance = new CameraInstance(config, _logger, onDetection);
        _cameras[config.CameraId] = instance;
        instance.Start();
    }

    public void StopCamera(string cameraId)
    {
        if (_cameras.TryRemove(cameraId, out var instance))
            instance.Dispose();
    }

    public byte[]? GetLatestFrame(string cameraId)
    {
        return _cameras.TryGetValue(cameraId, out var instance) ? instance.LatestJpegFrame : null;
    }

    public async Task WriteMjpegStreamAsync(string cameraId, Stream outputStream, CancellationToken ct)
    {
        if (!_cameras.TryGetValue(cameraId, out var instance)) return;

        var boundary = "----frameboundary";
        var boundaryBytes = Encoding.UTF8.GetBytes($"--{boundary}\r\n");
        var headerTemplate = "Content-Type: image/jpeg\r\nContent-Length: {0}\r\n\r\n";
        var newline = Encoding.UTF8.GetBytes("\r\n");

        while (!ct.IsCancellationRequested)
        {
            var frame = instance.LatestJpegFrame;
            if (frame != null && frame.Length > 0)
            {
                try
                {
                    await outputStream.WriteAsync(boundaryBytes, ct);
                    var header = Encoding.UTF8.GetBytes(string.Format(headerTemplate, frame.Length));
                    await outputStream.WriteAsync(header, ct);
                    await outputStream.WriteAsync(frame, ct);
                    await outputStream.WriteAsync(newline, ct);
                    await outputStream.FlushAsync(ct);
                }
                catch (OperationCanceledException) { break; }
                catch { break; }
            }

            try { await Task.Delay(instance.FrameDelay, ct); }
            catch (OperationCanceledException) { break; }
        }
    }

    public string GetMjpegContentType()
    {
        return "multipart/x-mixed-replace; boundary=----frameboundary";
    }

    public IEnumerable<string> GetCameraIds() => _cameras.Keys;

    public void Dispose()
    {
        foreach (var kvp in _cameras)
            kvp.Value.Dispose();
        _cameras.Clear();
    }

    // ─── Inner camera instance ───────────────────────────────

    private class CameraInstance : IDisposable
    {
        private readonly CameraConfig _config;
        private readonly ILogger _logger;
        private readonly Action<string, string, double, int>? _onDetection;
        private CancellationTokenSource? _cts;
        private Task? _captureTask;
        private InferenceSession? _yoloSession;
        private volatile byte[]? _latestFrame;
        private readonly HttpClient _httpClient;

        public byte[]? LatestJpegFrame => _latestFrame;
        public int FrameDelay => Math.Max(33, 1000 / Math.Max(1, _config.Fps));

        private static readonly string[] YoloLabels =
        [
            "person", "bicycle", "car", "motorcycle", "airplane", "bus", "train", "truck", "boat",
            "traffic light", "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat",
            "dog", "horse", "sheep", "cow", "elephant", "bear", "zebra", "giraffe", "backpack",
            "umbrella", "handbag", "tie", "suitcase", "frisbee", "skis", "snowboard", "sports ball",
            "kite", "baseball bat", "baseball glove", "skateboard", "surfboard", "tennis racket",
            "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple",
            "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair",
            "couch", "potted plant", "bed", "dining table", "toilet", "tv", "laptop", "mouse",
            "remote", "keyboard", "cell phone", "microwave", "oven", "toaster", "sink",
            "refrigerator", "book", "clock", "vase", "scissors", "teddy bear", "hair drier", "toothbrush"
        ];

        public CameraInstance(CameraConfig config, ILogger logger, Action<string, string, double, int>? onDetection)
        {
            _config = config;
            _logger = logger;
            _onDetection = onDetection;

            var handler = new HttpClientHandler();
            if (!string.IsNullOrEmpty(config.Username))
            {
                handler.Credentials = new System.Net.NetworkCredential(config.Username, config.Password);
            }
            _httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };

            if (config.EnableYolo)
            {
                try
                {
                    var modelPath = string.IsNullOrEmpty(config.YoloModelPath) ? "yolov8n.onnx" : config.YoloModelPath;
                    if (File.Exists(modelPath))
                    {
                        _yoloSession = new InferenceSession(modelPath);
                        _logger.LogInformation("YOLO model loaded: {Path}", modelPath);
                    }
                    else
                    {
                        _logger.LogWarning("YOLO model not found at {Path} — detection disabled", modelPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load YOLO model");
                }
            }
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _captureTask = _config.Protocol.ToLowerInvariant() switch
            {
                "rtsp" => CaptureRtspAsync(_cts.Token),
                "mjpeg" => CaptureMjpegAsync(_cts.Token),
                _ => CaptureHttpSnapshotAsync(_cts.Token)
            };
        }

        // ─── RTSP via FFmpeg ─────────────────────────────────

        private async Task CaptureRtspAsync(CancellationToken ct)
        {
            _logger.LogInformation("Starting RTSP capture for {Id}: {Url}", _config.CameraId, _config.Url);

            while (!ct.IsCancellationRequested)
            {
                Process? ffmpeg = null;
                try
                {
                    var url = _config.Url;
                    if (!string.IsNullOrEmpty(_config.Username))
                    {
                        var uri = new Uri(url);
                        url = $"{uri.Scheme}://{_config.Username}:{_config.Password}@{uri.Host}:{uri.Port}{uri.PathAndQuery}";
                    }

                    ffmpeg = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "ffmpeg",
                            Arguments = $"-rtsp_transport tcp -i \"{url}\" -f mjpeg -q:v 5 -r {_config.Fps} -",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    ffmpeg.Start();
                    var stream = ffmpeg.StandardOutput.BaseStream;
                    await ReadMjpegFramesAsync(stream, ct);
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "RTSP capture error for {Id}, retrying in 5s", _config.CameraId);
                }
                finally
                {
                    if (ffmpeg != null && !ffmpeg.HasExited)
                    {
                        try { ffmpeg.Kill(); } catch { }
                    }
                    ffmpeg?.Dispose();
                }

                if (!ct.IsCancellationRequested)
                    await Task.Delay(5000, ct).ConfigureAwait(false);
            }
        }

        // ─── MJPEG stream ────────────────────────────────────

        private async Task CaptureMjpegAsync(CancellationToken ct)
        {
            _logger.LogInformation("Starting MJPEG capture for {Id}: {Url}", _config.CameraId, _config.Url);

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var response = await _httpClient.GetAsync(_config.Url, HttpCompletionOption.ResponseHeadersRead, ct);
                    response.EnsureSuccessStatusCode();
                    var stream = await response.Content.ReadAsStreamAsync(ct);
                    await ReadMjpegFramesAsync(stream, ct);
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "MJPEG capture error for {Id}, retrying in 5s", _config.CameraId);
                }

                if (!ct.IsCancellationRequested)
                    await Task.Delay(5000, ct).ConfigureAwait(false);
            }
        }

        // ─── HTTP snapshot polling ───────────────────────────

        private async Task CaptureHttpSnapshotAsync(CancellationToken ct)
        {
            _logger.LogInformation("Starting HTTP snapshot capture for {Id}: {Url}", _config.CameraId, _config.Url);

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var bytes = await _httpClient.GetByteArrayAsync(_config.Url, ct);
                    await ProcessFrameAsync(bytes);
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    _logger.LogDebug(ex, "HTTP snapshot error for {Id}", _config.CameraId);
                }

                try { await Task.Delay(FrameDelay, ct); }
                catch (OperationCanceledException) { break; }
            }
        }

        // ─── MJPEG frame reader ──────────────────────────────

        private async Task ReadMjpegFramesAsync(Stream stream, CancellationToken ct)
        {
            var buffer = new byte[1024 * 1024]; // 1MB
            using var ms = new MemoryStream();
            bool inFrame = false;

            while (!ct.IsCancellationRequested)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                if (bytesRead == 0) break;

                for (int i = 0; i < bytesRead - 1; i++)
                {
                    if (!inFrame && buffer[i] == 0xFF && buffer[i + 1] == 0xD8)
                    {
                        ms.SetLength(0);
                        inFrame = true;
                    }

                    if (inFrame)
                    {
                        ms.WriteByte(buffer[i]);

                        if (buffer[i] == 0xFF && buffer[i + 1] == 0xD9)
                        {
                            ms.WriteByte(buffer[i + 1]);
                            i++;
                            inFrame = false;
                            await ProcessFrameAsync(ms.ToArray());
                        }
                    }
                }
            }
        }

        // ─── Frame processing + YOLO ─────────────────────────

        private async Task ProcessFrameAsync(byte[] jpegBytes)
        {
            if (_yoloSession != null)
            {
                try
                {
                    var (processedBytes, detections) = await Task.Run(() => RunYoloDetection(jpegBytes));
                    _latestFrame = processedBytes;

                    if (_onDetection != null && !string.IsNullOrEmpty(_config.DetectionVariablePrefix))
                    {
                        if (detections.Count > 0)
                        {
                            var best = detections.OrderByDescending(d => d.Confidence).First();
                            _onDetection(_config.DetectionVariablePrefix, best.Label, best.Confidence, detections.Count);
                        }
                        else
                        {
                            _onDetection(_config.DetectionVariablePrefix, "", 0, 0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "YOLO processing error");
                    _latestFrame = jpegBytes;
                }
            }
            else
            {
                _latestFrame = jpegBytes;
            }
        }

        private (byte[] Jpeg, List<Detection> Detections) RunYoloDetection(byte[] jpegBytes)
        {
            const int ModelInputSize = 640;

            using var image = Image.Load<Rgb24>(jpegBytes);
            int origW = image.Width;
            int origH = image.Height;

            // Resize to model input
            using var resized = image.Clone(ctx => ctx.Resize(ModelInputSize, ModelInputSize));

            // Build input tensor [1, 3, 640, 640]
            var input = new DenseTensor<float>(new[] { 1, 3, ModelInputSize, ModelInputSize });
            resized.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < ModelInputSize; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < ModelInputSize; x++)
                    {
                        input[0, 0, y, x] = row[x].R / 255f;
                        input[0, 1, y, x] = row[x].G / 255f;
                        input[0, 2, y, x] = row[x].B / 255f;
                    }
                }
            });

            var inputName = _yoloSession!.InputNames.FirstOrDefault() ?? "images";
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(inputName, input) };

            using var results = _yoloSession.Run(inputs);
            var output = results.First().AsTensor<float>();

            var detections = ParseYoloOutput(output, origW, origH, ModelInputSize, (float)_config.YoloConfidence);

            // Apply NMS
            detections = NonMaxSuppression(detections, 0.45f);

            // Draw bounding boxes
            if (_config.DrawDetections && detections.Count > 0)
            {
                foreach (var det in detections)
                {
                    var rect = new SixLabors.ImageSharp.Drawing.RectangularPolygon(det.X, det.Y, det.W, det.H);
                    var color = SixLabors.ImageSharp.Color.LimeGreen;
                    image.Mutate(ctx =>
                    {
                        ctx.Draw(color, 2, rect);
                    });
                    // Label text is drawn as a simple rect overlay
                    var labelRect = new SixLabors.ImageSharp.Drawing.RectangularPolygon(det.X, Math.Max(0, det.Y - 16), det.Label.Length * 8 + 8, 16);
                    image.Mutate(ctx => ctx.Fill(SixLabors.ImageSharp.Color.FromRgba(0, 0, 0, 160), labelRect));
                }
            }

            using var outMs = new MemoryStream();
            image.SaveAsJpeg(outMs, new JpegEncoder { Quality = 80 });
            return (outMs.ToArray(), detections);
        }

        private static List<Detection> ParseYoloOutput(Tensor<float> output, int origW, int origH, int modelSize, float confThreshold)
        {
            var detections = new List<Detection>();

            // YOLOv8 output shape: [1, 84, 8400] (transposed from v5)
            int numClasses = output.Dimensions[1] - 4;
            int numBoxes = output.Dimensions[2];

            float xScale = (float)origW / modelSize;
            float yScale = (float)origH / modelSize;

            for (int i = 0; i < numBoxes; i++)
            {
                float cx = output[0, 0, i];
                float cy = output[0, 1, i];
                float w = output[0, 2, i];
                float h = output[0, 3, i];

                float maxConf = 0;
                int maxIdx = 0;
                for (int c = 0; c < numClasses; c++)
                {
                    float conf = output[0, 4 + c, i];
                    if (conf > maxConf) { maxConf = conf; maxIdx = c; }
                }

                if (maxConf < confThreshold) continue;

                float x1 = (cx - w / 2) * xScale;
                float y1 = (cy - h / 2) * yScale;
                float bw = w * xScale;
                float bh = h * yScale;

                string label = maxIdx < YoloLabels.Length ? YoloLabels[maxIdx] : $"class_{maxIdx}";
                detections.Add(new Detection { X = x1, Y = y1, W = bw, H = bh, Confidence = maxConf, Label = label, ClassId = maxIdx });
            }

            return detections;
        }

        private static List<Detection> NonMaxSuppression(List<Detection> detections, float iouThreshold)
        {
            var result = new List<Detection>();
            var sorted = detections.OrderByDescending(d => d.Confidence).ToList();

            while (sorted.Count > 0)
            {
                var best = sorted[0];
                result.Add(best);
                sorted.RemoveAt(0);
                sorted.RemoveAll(d => d.ClassId == best.ClassId && IoU(best, d) > iouThreshold);
            }

            return result;
        }

        private static float IoU(Detection a, Detection b)
        {
            float x1 = Math.Max(a.X, b.X);
            float y1 = Math.Max(a.Y, b.Y);
            float x2 = Math.Min(a.X + a.W, b.X + b.W);
            float y2 = Math.Min(a.Y + a.H, b.Y + b.H);

            float intersection = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
            float aArea = a.W * a.H;
            float bArea = b.W * b.H;
            float union = aArea + bArea - intersection;

            return union > 0 ? intersection / union : 0;
        }

        public void Dispose()
        {
            _cts?.Cancel();
            try { _captureTask?.Wait(2000); } catch { }
            _cts?.Dispose();
            _yoloSession?.Dispose();
            _httpClient.Dispose();
        }
    }

    private class Detection
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        public float Confidence { get; set; }
        public string Label { get; set; } = "";
        public int ClassId { get; set; }
    }
}
