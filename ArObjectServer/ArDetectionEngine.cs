// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ZXing;
using ZXing.ImageSharp;

namespace ArObjectServer;

/// <summary>
/// A single object detection result in image pixel coordinates.
/// </summary>
public class Detection
{
    public float X { get; set; }
    public float Y { get; set; }
    public float W { get; set; }
    public float H { get; set; }
    public float Confidence { get; set; }
    public string Label { get; set; } = "";
    public int ClassId { get; set; }

    /// <summary>
    /// Optional instance key distinguishing this detection from others of the same class
    /// (e.g. an embedded marker/tag ID). Populated by higher-level tracking logic, not the
    /// raw YOLO output itself.
    /// </summary>
    public string? InstanceKey { get; set; }
}

/// <summary>
/// Per-stage latency breakdown for a single Detect() call, in milliseconds. Used to identify
/// the actual bottleneck (usually ONNX inference / image decode-resize) rather than assuming
/// transport (HTTP vs SignalR/gRPC) is the limiting factor.
/// </summary>
public class DetectionTiming
{
    public long DecodeMs { get; set; }
    public long PreprocessMs { get; set; }
    public long InferenceMs { get; set; }
    public long PostprocessMs { get; set; }
    public long MarkerDecodeMs { get; set; }
    public long TotalMs { get; set; }
}

/// <summary>
/// Runs YOLOv8 ONNX object detection on individual frames with minimal latency.
/// Designed to be called synchronously per-frame by the AR HTTP endpoint — no streaming
/// or background capture loop, since the AR client (browser/device camera) pushes frames
/// directly to this local server over a fast loopback/LAN connection.
/// </summary>
public class ArDetectionEngine : IDisposable
{
    private const int ModelInputSize = 640;
    private readonly InferenceSession _session;
    private readonly string _inputName;
    private readonly float _defaultConfidence;

    private static readonly string[] DefaultLabels =
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

    private readonly string[] _labels;
    private readonly BarcodeReaderGeneric _qrReader;

    public ArDetectionEngine(string modelPath, bool useCuda, int cudaDeviceId, float defaultConfidence, string[]? customLabels, ILogger logger)
    {
        _defaultConfidence = defaultConfidence;
        _labels = customLabels is { Length: > 0 } ? customLabels : DefaultLabels;

        // Used to read an optional QR/barcode marker physically attached to each object
        // instance (e.g. a printed tag on Motor1 vs Motor2) so that two objects sharing the
        // same recognized visual class can still be told apart at runtime.
        _qrReader = new BarcodeReaderGeneric
        {
            AutoRotate = true,
            Options = { TryHarder = true, PossibleFormats = [BarcodeFormat.QR_CODE, BarcodeFormat.DATA_MATRIX, BarcodeFormat.CODE_128] }
        };

        if (useCuda)
        {
            try
            {
                var opts = new Microsoft.ML.OnnxRuntime.SessionOptions();
                opts.AppendExecutionProvider_CUDA(cudaDeviceId);
                opts.AppendExecutionProvider_CPU();
                _session = new InferenceSession(modelPath, opts);
                logger.LogInformation("AR detection model loaded with CUDA (device {DeviceId}): {Path}", cudaDeviceId, modelPath);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "CUDA unavailable for AR detection, falling back to CPU");
                _session = new InferenceSession(modelPath);
            }
        }
        else
        {
            _session = new InferenceSession(modelPath);
        }

        _inputName = _session.InputNames.FirstOrDefault() ?? "images";
    }

    /// <summary>
    /// Runs detection on a single JPEG/PNG-encoded frame and returns detections in the
    /// original image's pixel coordinates, along with the image's width/height for the
    /// client to scale bounding boxes against its own display surface.
    /// </summary>
    /// <param name="imageBytes">Encoded (JPEG/PNG) frame bytes.</param>
    /// <param name="confidenceOverride">Optional per-request confidence threshold override.</param>
    /// <param name="decodeInstanceMarkers">
    /// When true, each detection's bounding box (plus a small margin) is scanned for a
    /// QR/DataMatrix/barcode marker. If found, its decoded payload becomes
    /// <see cref="Detection.InstanceKey"/>, allowing multiple physical objects that share the
    /// same recognized visual class (e.g. two identical motors) to be distinguished and mapped
    /// to different parameter files. Adds modest per-detection latency, so it can be disabled
    /// for classes/scenes where markers aren't used.
    /// </param>
    public (List<Detection> Detections, int Width, int Height) Detect(byte[] imageBytes, float? confidenceOverride = null, bool decodeInstanceMarkers = true)
        => Detect(imageBytes, confidenceOverride, decodeInstanceMarkers, out _);

    /// <summary>
    /// Same as <see cref="Detect(byte[], float?, bool)"/> but also returns a per-stage latency
    /// breakdown (decode/resize/inference/postprocess/marker-decode), so the actual bottleneck
    /// can be measured instead of guessed — useful when evaluating whether transport (e.g.
    /// switching HTTP to SignalR/gRPC) could meaningfully reduce end-to-end latency.
    /// </summary>
    public (List<Detection> Detections, int Width, int Height) Detect(
        byte[] imageBytes, float? confidenceOverride, bool decodeInstanceMarkers, out DetectionTiming timing)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        long t0 = sw.ElapsedMilliseconds;

        using var image = Image.Load<Rgb24>(imageBytes);
        int origW = image.Width;
        int origH = image.Height;
        long tDecode = sw.ElapsedMilliseconds;

        using var resized = image.Clone(ctx => ctx.Resize(ModelInputSize, ModelInputSize));

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
        long tPreprocess = sw.ElapsedMilliseconds;

        var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(_inputName, input) };
        using var results = _session.Run(inputs);
        var output = results.First().AsTensor<float>();
        long tInference = sw.ElapsedMilliseconds;

        float confThreshold = confidenceOverride ?? _defaultConfidence;
        var detections = ParseOutput(output, origW, origH, ModelInputSize, confThreshold);
        detections = NonMaxSuppression(detections, 0.45f);
        long tPostprocess = sw.ElapsedMilliseconds;

        if (decodeInstanceMarkers && detections.Count > 0)
        {
            foreach (var det in detections)
            {
                det.InstanceKey = TryDecodeInstanceMarker(image, det, origW, origH);
            }
        }
        long tMarkers = sw.ElapsedMilliseconds;

        timing = new DetectionTiming
        {
            DecodeMs = tDecode - t0,
            PreprocessMs = tPreprocess - tDecode,
            InferenceMs = tInference - tPreprocess,
            PostprocessMs = tPostprocess - tInference,
            MarkerDecodeMs = tMarkers - tPostprocess,
            TotalMs = tMarkers - t0
        };

        return (detections, origW, origH);
    }

    /// <summary>
    /// Crops around a detection's bounding box (with a margin, since markers are often placed
    /// just outside the object's tightest bounding box) and attempts to decode a QR/barcode
    /// marker. Returns the decoded text, or null if no marker was found.
    /// </summary>
    private string? TryDecodeInstanceMarker(Image<Rgb24> image, Detection det, int origW, int origH)
    {
        try
        {
            const float marginRatio = 0.25f;
            int marginX = (int)(det.W * marginRatio);
            int marginY = (int)(det.H * marginRatio);

            int cropX = Math.Clamp((int)det.X - marginX, 0, origW - 1);
            int cropY = Math.Clamp((int)det.Y - marginY, 0, origH - 1);
            int cropW = Math.Clamp((int)det.W + marginX * 2, 1, origW - cropX);
            int cropH = Math.Clamp((int)det.H + marginY * 2, 1, origH - cropY);

            using var crop = image.Clone(ctx => ctx.Crop(new Rectangle(cropX, cropY, cropW, cropH)));
            var result = _qrReader.Decode(crop);
            return result?.Text;
        }
        catch
        {
            return null; // marker decode is best-effort; never fail the whole detection for it
        }
    }

    private List<Detection> ParseOutput(Tensor<float> output, int origW, int origH, int modelSize, float confThreshold)
    {
        var detections = new List<Detection>();

        // YOLOv8 output shape: [1, 84, 8400]
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

            string label = maxIdx < _labels.Length ? _labels[maxIdx] : $"class_{maxIdx}";
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

    public void Dispose() => _session.Dispose();
}
