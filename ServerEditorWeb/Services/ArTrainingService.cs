// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ServerEditorWeb.Services;

/// <summary>
/// Manages custom object-detection model training for the AR feature, launched directly from
/// the ServerEditorWeb "AR Object Mapping" panel. Wraps a Python + Ultralytics YOLO training
/// run as an external process (kept out-of-process so a crash/hang in the training script can
/// never take down the editor), streaming stdout into a log the UI can show live and parsing
/// epoch progress out of Ultralytics' console output to drive a progress bar.
///
/// Trained models (best.pt, exported to ONNX) are copied beside the ArObjectServer executable
/// so the local OR server can be pointed at them without any manual file shuffling.
/// </summary>
public class ArTrainingService
{
    private readonly ILogger<ArTrainingService> _logger;
    private Process? _process;
    private CancellationTokenSource? _cts;
    private readonly StringBuilder _log = new();
    private readonly List<string> _knownClasses = new();

    public event Action? StateChanged;

    public bool IsRunning { get; private set; }
    public double Progress { get; private set; }
    public string StatusText { get; private set; } = "Idle.";
    public string LogText => _log.ToString();
    public string? LastModelPath { get; private set; }
    public IReadOnlyList<string> KnownClasses => _knownClasses;

    public ArTrainingService(ILogger<ArTrainingService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Kicks off a YOLO training run against a user-supplied labeled dataset folder
    /// (expects Ultralytics' standard layout: images/train, images/val, labels/train,
    /// labels/val, and a classes.txt or data.yaml). Requires Python with the "ultralytics"
    /// package installed on PATH — this is intentionally a thin process wrapper, not a
    /// reimplementation of training, since state-of-the-art detector training is squarely
    /// Ultralytics'/PyTorch's job, not something to hand-roll in C#.
    /// </summary>
    public async Task StartAsync(string datasetFolder, string newClassName, int epochs = 50, string baseModel = "yolov8n.pt")
    {
        if (IsRunning) return;
        if (string.IsNullOrWhiteSpace(datasetFolder) || !Directory.Exists(datasetFolder))
        {
            AppendLog($"[error] Dataset folder not found: {datasetFolder}");
            NotifyChanged();
            return;
        }

        _cts = new CancellationTokenSource();
        IsRunning = true;
        Progress = 0;
        StatusText = "Preparing dataset...";
        _log.Clear();
        LastModelPath = null;
        NotifyChanged();

        try
        {
            var dataYamlPath = EnsureDataYaml(datasetFolder, newClassName);

            var runName = $"ar-train-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
            var projectDir = Path.Combine(datasetFolder, "runs");

            // Uses the Ultralytics CLI (`yolo` on PATH after `pip install ultralytics`) rather
            // than a bespoke Python script, so the editor stays decoupled from any specific
            // training script version — users can `pip install -U ultralytics` independently.
            var args = $"detect train data=\"{dataYamlPath}\" model={baseModel} epochs={epochs} " +
                       $"project=\"{projectDir}\" name=\"{runName}\" exist_ok=True";

            AppendLog($"[info] Starting: yolo {args}");
            StatusText = "Training started...";
            NotifyChanged();

            await RunProcessAsync("yolo", args, datasetFolder, _cts.Token);

            if (_cts.IsCancellationRequested)
            {
                StatusText = "Training cancelled.";
                AppendLog("[info] Training cancelled by user.");
            }
            else
            {
                var bestWeights = Path.Combine(projectDir, runName, "weights", "best.pt");
                if (File.Exists(bestWeights))
                {
                    StatusText = "Training complete. Exporting to ONNX...";
                    NotifyChanged();
                    await ExportToOnnxAsync(bestWeights, datasetFolder, _cts.Token);
                }
                else
                {
                    StatusText = "Training finished but no weights file was produced — check the log.";
                    AppendLog($"[warn] Expected weights not found at {bestWeights}");
                }
            }
        }
        catch (Exception ex)
        {
            StatusText = $"Training failed: {ex.Message}";
            AppendLog($"[error] {ex}");
            _logger.LogError(ex, "AR custom model training failed");
        }
        finally
        {
            IsRunning = false;
            _process = null;
            NotifyChanged();
        }
    }

    public void Cancel()
    {
        try
        {
            _cts?.Cancel();
            if (_process != null && !_process.HasExited)
                _process.Kill(entireProcessTree: true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cancel AR training process");
        }
    }

    /// <summary>
    /// Writes (or reuses) a minimal Ultralytics data.yaml pointing at the dataset folder's
    /// standard images/labels subfolders, appending the requested new class name to
    /// classes.txt if it isn't already tracked.
    /// </summary>
    private string EnsureDataYaml(string datasetFolder, string newClassName)
    {
        var classesFile = Path.Combine(datasetFolder, "classes.txt");
        var classes = File.Exists(classesFile)
            ? File.ReadAllLines(classesFile).Where(l => !string.IsNullOrWhiteSpace(l)).ToList()
            : new List<string>();

        if (!string.IsNullOrWhiteSpace(newClassName) && !classes.Contains(newClassName, StringComparer.OrdinalIgnoreCase))
        {
            classes.Add(newClassName);
            File.WriteAllLines(classesFile, classes);
        }

        _knownClasses.Clear();
        _knownClasses.AddRange(classes);

        var dataYamlPath = Path.Combine(datasetFolder, "data.yaml");
        var namesBlock = string.Join(Environment.NewLine, classes.Select((c, i) => $"  {i}: {c}"));
        var yaml = $"""
            path: {datasetFolder.Replace("\\", "/")}
            train: images/train
            val: images/val
            names:
            {namesBlock}
            """;
        File.WriteAllText(dataYamlPath, yaml);
        return dataYamlPath;
    }

    private async Task ExportToOnnxAsync(string weightsPath, string workingDir, CancellationToken ct)
    {
        await RunProcessAsync("yolo", $"export model=\"{weightsPath}\" format=onnx", workingDir, ct);

        var onnxPath = Path.ChangeExtension(weightsPath, ".onnx");
        if (File.Exists(onnxPath))
        {
            LastModelPath = onnxPath;
            StatusText = $"Done. Trained model ready: {onnxPath}";
            AppendLog($"[info] Exported ONNX model: {onnxPath}");

            // Copy beside the ArObjectServer executable so it can be selected as the
            // active detection model without manual file moves.
            try
            {
                var arServerDir = FindArObjectServerDirectory();
                if (arServerDir != null)
                {
                    var dest = Path.Combine(arServerDir, $"custom-{Path.GetFileName(onnxPath)}");
                    File.Copy(onnxPath, dest, overwrite: true);
                    AppendLog($"[info] Copied model to {dest} — point ArObjectServer at this file to use it.");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"[warn] Could not copy model next to ArObjectServer: {ex.Message}");
            }
        }
        else
        {
            StatusText = "Training succeeded but ONNX export did not produce a file — check the log.";
        }
    }

    private static string? FindArObjectServerDirectory()
    {
        var d = AppContext.BaseDirectory;
        for (var i = 0; i < 6 && !string.IsNullOrEmpty(d); i++)
        {
            var candidate = Path.Combine(d, "ArObjectServer");
            if (Directory.Exists(candidate)) return candidate;
            d = Directory.GetParent(d)?.FullName ?? "";
        }
        return null;
    }

    private async Task RunProcessAsync(string fileName, string arguments, string workingDir, CancellationToken ct)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process = new Process { StartInfo = psi, EnableRaisingEvents = true };
        _process.OutputDataReceived += (_, e) => HandleOutputLine(e.Data);
        _process.ErrorDataReceived += (_, e) => HandleOutputLine(e.Data);

        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        using (ct.Register(() => { try { if (!_process.HasExited) _process.Kill(entireProcessTree: true); } catch { } }))
        {
            await _process.WaitForExitAsync(CancellationToken.None);
        }
    }

    // Ultralytics prints progress lines like "  3/50      2.1G ..." — extract "epoch/total" to
    // drive a simple percentage progress bar without depending on any structured output format.
    private static readonly Regex EpochRegex = new(@"^\s*(\d+)/(\d+)\s", RegexOptions.Compiled);

    private void HandleOutputLine(string? line)
    {
        if (line == null) return;
        AppendLog(line);

        var match = EpochRegex.Match(line);
        if (match.Success &&
            int.TryParse(match.Groups[1].Value, out var epoch) &&
            int.TryParse(match.Groups[2].Value, out var total) && total > 0)
        {
            Progress = Math.Clamp(epoch * 100.0 / total, 0, 100);
            StatusText = $"Training epoch {epoch}/{total}...";
            NotifyChanged();
        }
    }

    private void AppendLog(string line)
    {
        _log.AppendLine(line);
        // Keep the in-memory log bounded so a long run can't grow unbounded editor memory.
        if (_log.Length > 200_000)
            _log.Remove(0, _log.Length - 150_000);
        NotifyChanged();
    }

    private void NotifyChanged() => StateChanged?.Invoke();
}
