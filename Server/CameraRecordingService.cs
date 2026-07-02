// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharedModels;

namespace SimpleOpcFileServer;

/// <summary>
/// Records JPEG frames from <see cref="CameraStreamService"/> into MJPEG-wrapped .avi segments
/// using FFmpeg. Manages segment rotation, retention (max age / max size), and provides
/// a listing API for the viewer to browse/play recordings.
/// </summary>
public sealed class CameraRecordingService : IDisposable
{
    private readonly ILogger<CameraRecordingService> _logger;
    private readonly ConcurrentDictionary<string, RecorderInstance> _recorders = new();

    public CameraRecordingService(ILogger<CameraRecordingService> logger)
    {
        _logger = logger;
    }

    /// <summary>Start a recorder for the given camera config. The recorder will pull latest frames from <paramref name="getLatestFrame"/>.</summary>
    public void StartRecorder(CameraConfig config, Func<byte[]?> getLatestFrame, string projectDir,
        Func<string, bool>? readBoolVariable = null, Func<string, double>? readDoubleVariable = null,
        Func<string, string?>? readStringVariable = null)
    {
        if (!config.RecordingEnabled) return;
        if (_recorders.ContainsKey(config.CameraId)) return;

        var recorder = new RecorderInstance(config, getLatestFrame, projectDir, _logger, readBoolVariable, readDoubleVariable, readStringVariable);
        _recorders[config.CameraId] = recorder;
        recorder.Start();
    }

    public void StopRecorder(string cameraId)
    {
        if (_recorders.TryRemove(cameraId, out var recorder))
            recorder.Dispose();
    }

    /// <summary>List recordings for a given camera.</summary>
    public List<CameraRecordingInfo> ListRecordings(string cameraId, string projectDir)
    {
        var subDir = cameraId;
        if (_recorders.TryGetValue(cameraId, out var rec))
            subDir = rec.SubDir;

        var dir = Path.Combine(projectDir, "recordings", subDir);
        if (!Directory.Exists(dir)) return [];

        var result = new List<CameraRecordingInfo>();
        foreach (var file in Directory.GetFiles(dir, "*.avi").OrderByDescending(f => f))
        {
            var fi = new FileInfo(file);
            // File name format: cam1_20250101_120000.avi
            var name = Path.GetFileNameWithoutExtension(fi.Name);
            var parts = name.Split('_');
            DateTime startTime = fi.CreationTimeUtc;
            if (parts.Length >= 3 && DateTime.TryParseExact(
                    $"{parts[^2]}_{parts[^1]}", "yyyyMMdd_HHmmss",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed))
                startTime = DateTime.SpecifyKind(parsed, DateTimeKind.Utc);

            result.Add(new CameraRecordingInfo
            {
                FileName = fi.Name,
                CameraId = cameraId,
                StartTimeUtc = startTime,
                SizeBytes = fi.Length,
                DurationSeconds = 0, // approximate from config; exact requires probing
                Url = $"/camera/{Uri.EscapeDataString(cameraId)}/recordings/{Uri.EscapeDataString(fi.Name)}"
            });
        }
        return result;
    }

    /// <summary>Gets the full file path for a recording, or null if not found.</summary>
    public string? GetRecordingFilePath(string cameraId, string fileName, string projectDir)
    {
        var subDir = cameraId;
        if (_recorders.TryGetValue(cameraId, out var rec))
            subDir = rec.SubDir;

        var path = Path.Combine(projectDir, "recordings", subDir, fileName);
        return File.Exists(path) ? path : null;
    }

    /// <summary>Delete a specific recording file.</summary>
    public bool DeleteRecording(string cameraId, string fileName, string projectDir)
    {
        var path = GetRecordingFilePath(cameraId, fileName, projectDir);
        if (path == null) return false;
        try { File.Delete(path); return true; } catch { return false; }
    }

    /// <summary>Returns true if the given camera is currently recording.</summary>
    public bool IsRecording(string cameraId)
    {
        return _recorders.TryGetValue(cameraId, out var r) && r.IsRecording;
    }

    public void Dispose()
    {
        foreach (var kvp in _recorders)
            kvp.Value.Dispose();
        _recorders.Clear();
    }

    // ─── Inner recorder instance ─────────────────────────────

    private sealed class RecorderInstance : IDisposable
    {
        private readonly CameraConfig _config;
        private readonly Func<byte[]?> _getLatestFrame;
        private readonly Func<string, bool>? _readBoolVariable;
        private readonly Func<string, double>? _readDoubleVariable;
        private readonly Func<string, string?>? _readStringVariable;
        private readonly ILogger _logger;
        private readonly string _recordingDir;
        private CancellationTokenSource? _cts;
        private Task? _recordTask;
        private Task? _cleanupTask;
        private volatile bool _isRecording;
        private Process? _ffmpeg;
        private Stream? _ffmpegStdin;
        private DateTime _segmentStart;

        public string SubDir { get; }
        public bool IsRecording => _isRecording;

        public RecorderInstance(CameraConfig config, Func<byte[]?> getLatestFrame, string projectDir,
            ILogger logger, Func<string, bool>? readBoolVariable, Func<string, double>? readDoubleVariable,
            Func<string, string?>? readStringVariable)
        {
            _config = config;
            _getLatestFrame = getLatestFrame;
            _readBoolVariable = readBoolVariable;
            _readDoubleVariable = readDoubleVariable;
            _readStringVariable = readStringVariable;
            _logger = logger;

            SubDir = string.IsNullOrEmpty(config.RecordingPath) ? config.CameraId : config.RecordingPath;
            _recordingDir = Path.Combine(projectDir, "recordings", SubDir);
            Directory.CreateDirectory(_recordingDir);
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _recordTask = RecordLoopAsync(_cts.Token);
            _cleanupTask = CleanupLoopAsync(_cts.Token);
        }

        private async Task RecordLoopAsync(CancellationToken ct)
        {
            _logger.LogInformation("Recording service started for camera {Id} (trigger={Trigger})",
                _config.CameraId, _config.RecordingTrigger);

            var frameDelay = Math.Max(33, 1000 / Math.Max(1, _config.Fps));
            DateTime? triggerInactiveTime = null;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    bool shouldRecord = ShouldRecord();

                    if (shouldRecord)
                    {
                        triggerInactiveTime = null;

                        // Start a new segment if not recording
                        if (!_isRecording)
                            StartSegment();

                        // Write frame
                        var frame = _getLatestFrame();
                        if (frame != null && frame.Length > 0 && _ffmpegStdin != null)
                        {
                            try
                            {
                                await _ffmpegStdin.WriteAsync(frame, ct);
                                await _ffmpegStdin.FlushAsync(ct);
                            }
                            catch (IOException) { StopSegment(); }
                        }

                        // Check max duration
                        if (_config.RecordingMaxDurationSeconds > 0 &&
                            (DateTime.UtcNow - _segmentStart).TotalSeconds >= _config.RecordingMaxDurationSeconds)
                        {
                            StopSegment();
                            // Will start a new segment on next iteration
                        }
                    }
                    else if (_isRecording)
                    {
                        // Post-roll: keep recording for a few more seconds after trigger stops
                        triggerInactiveTime ??= DateTime.UtcNow;
                        if ((DateTime.UtcNow - triggerInactiveTime.Value).TotalSeconds >= _config.RecordingPostRollSeconds)
                        {
                            StopSegment();
                            triggerInactiveTime = null;
                        }
                        else
                        {
                            // Still in post-roll, write frame
                            var frame = _getLatestFrame();
                            if (frame != null && frame.Length > 0 && _ffmpegStdin != null)
                            {
                                try
                                {
                                    await _ffmpegStdin.WriteAsync(frame, ct);
                                    await _ffmpegStdin.FlushAsync(ct);
                                }
                                catch (IOException) { StopSegment(); }
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Recording loop error for {Id}", _config.CameraId);
                }

                try { await Task.Delay(frameDelay, ct); }
                catch (OperationCanceledException) { break; }
            }

            StopSegment();
        }

        private bool ShouldRecord()
        {
            return _config.RecordingTrigger.ToLowerInvariant() switch
            {
                "always" => true,
                "detection" => HasActiveDetection(),
                "variable" => ReadTriggerVariable(),
                _ => true
            };
        }

        private bool HasActiveDetection()
        {
            if (string.IsNullOrEmpty(_config.DetectionVariablePrefix)) return false;

            // Check if detection is alive
            if (_readBoolVariable != null)
            {
                try { if (!_readBoolVariable($"{_config.DetectionVariablePrefix}.Alive")) return false; }
                catch { return false; }
            }
            else { return false; }

            // Check confidence threshold
            if (_config.RecordingDetectionConfidenceThreshold > 0 && _readDoubleVariable != null)
            {
                try
                {
                    var confidence = _readDoubleVariable($"{_config.DetectionVariablePrefix}.Confidence");
                    if (confidence < _config.RecordingDetectionConfidenceThreshold) return false;
                }
                catch { return false; }
            }

            // Check label filter
            if (!string.IsNullOrWhiteSpace(_config.RecordingDetectionLabels) && _readStringVariable != null)
            {
                try
                {
                    var currentLabel = _readStringVariable($"{_config.DetectionVariablePrefix}.Label") ?? "";
                    if (string.IsNullOrEmpty(currentLabel)) return false;
                    var allowedLabels = _config.RecordingDetectionLabels
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (!allowedLabels.Any(l => l.Equals(currentLabel, StringComparison.OrdinalIgnoreCase)))
                        return false;
                }
                catch { return false; }
            }

            return true;
        }

        private bool ReadTriggerVariable()
        {
            if (string.IsNullOrEmpty(_config.RecordingTriggerVariable) || _readBoolVariable == null)
                return false;
            try { return _readBoolVariable(_config.RecordingTriggerVariable); }
            catch { return false; }
        }

        private void StartSegment()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var fileName = $"{_config.CameraId}_{timestamp}.avi";
            var filePath = Path.Combine(_recordingDir, fileName);

            try
            {
                // Use FFmpeg to mux piped JPEG frames into an MJPEG AVI container
                _ffmpeg = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $"-y -f mjpeg -framerate {_config.Fps} -i pipe:0 -c:v copy \"{filePath}\"",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                _ffmpeg.Start();
                _ffmpegStdin = _ffmpeg.StandardInput.BaseStream;
                _segmentStart = DateTime.UtcNow;
                _isRecording = true;

                _logger.LogInformation("Recording started: {File}", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to start FFmpeg recording for {Id}", _config.CameraId);
                _isRecording = false;
            }
        }

        private void StopSegment()
        {
            if (!_isRecording) return;
            _isRecording = false;

            try
            {
                _ffmpegStdin?.Close();
                _ffmpegStdin = null;

                if (_ffmpeg != null && !_ffmpeg.HasExited)
                {
                    _ffmpeg.WaitForExit(5000);
                    if (!_ffmpeg.HasExited) _ffmpeg.Kill();
                }
                _ffmpeg?.Dispose();
                _ffmpeg = null;

                _logger.LogInformation("Recording segment stopped for {Id}", _config.CameraId);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error stopping recording segment for {Id}", _config.CameraId);
            }
        }

        /// <summary>Periodically cleans up old recordings based on max age and max size.</summary>
        private async Task CleanupLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try { await Task.Delay(TimeSpan.FromMinutes(5), ct); }
                catch (OperationCanceledException) { break; }

                try
                {
                    if (!Directory.Exists(_recordingDir)) continue;

                    var files = Directory.GetFiles(_recordingDir, "*.avi")
                        .Select(f => new FileInfo(f))
                        .OrderBy(f => f.CreationTimeUtc)
                        .ToList();

                    // Delete files older than max age
                    if (_config.RecordingMaxAgeHours > 0)
                    {
                        var cutoff = DateTime.UtcNow.AddHours(-_config.RecordingMaxAgeHours);
                        foreach (var fi in files.Where(f => f.CreationTimeUtc < cutoff).ToList())
                        {
                            try { fi.Delete(); files.Remove(fi); _logger.LogDebug("Deleted expired recording: {File}", fi.Name); }
                            catch { }
                        }
                    }

                    // Delete oldest files if total size exceeds limit
                    if (_config.RecordingMaxSizeMB > 0)
                    {
                        long maxBytes = (long)_config.RecordingMaxSizeMB * 1024 * 1024;
                        long totalSize = files.Sum(f => f.Length);
                        while (totalSize > maxBytes && files.Count > 0)
                        {
                            var oldest = files[0];
                            totalSize -= oldest.Length;
                            try { oldest.Delete(); _logger.LogDebug("Deleted recording (size limit): {File}", oldest.Name); }
                            catch { }
                            files.RemoveAt(0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Recording cleanup error for {Id}", _config.CameraId);
                }
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            StopSegment();
            try { _recordTask?.Wait(3000); } catch { }
            try { _cleanupTask?.Wait(1000); } catch { }
            _cts?.Dispose();
        }
    }
}
