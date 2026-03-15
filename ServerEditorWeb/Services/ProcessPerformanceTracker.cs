using System.Diagnostics;

namespace ServerEditorWeb.Services;

/// <summary>
/// Snapshot of process performance metrics.
/// </summary>
public class ProcessPerformanceSnapshot
{
    public double CpuPercent { get; set; }
    public long WorkingSetMB { get; set; }
    public long PrivateMemoryMB { get; set; }
    public int ThreadCount { get; set; }
    public int HandleCount { get; set; }
    public TimeSpan Uptime { get; set; }
    public TimeSpan TotalProcessorTime { get; set; }
}

/// <summary>
/// Tracks performance metrics for a managed Process over time.
/// </summary>
public class ProcessPerformanceTracker
{
    private TimeSpan _lastCpuTime;
    private DateTime _lastSampleTime;
    private double _lastCpuPercent;
    private bool _initialized;

    /// <summary>
    /// Sample the given process and return a snapshot. Returns null if the process is null or exited.
    /// </summary>
    public ProcessPerformanceSnapshot? Sample(Process? process)
    {
        if (process == null)
            return null;

        try
        {
            process.Refresh();

            if (process.HasExited)
                return null;

            var now = DateTime.UtcNow;
            var cpuTime = process.TotalProcessorTime;

            if (_initialized)
            {
                var elapsed = (now - _lastSampleTime).TotalMilliseconds;
                if (elapsed > 0)
                {
                    var cpuDelta = (cpuTime - _lastCpuTime).TotalMilliseconds;
                    _lastCpuPercent = cpuDelta / (elapsed * Environment.ProcessorCount) * 100.0;
                    if (_lastCpuPercent < 0) _lastCpuPercent = 0;
                    if (_lastCpuPercent > 100) _lastCpuPercent = 100;
                }
            }
            else
            {
                _initialized = true;
            }

            _lastCpuTime = cpuTime;
            _lastSampleTime = now;

            return new ProcessPerformanceSnapshot
            {
                CpuPercent = Math.Round(_lastCpuPercent, 2),
                WorkingSetMB = process.WorkingSet64 / (1024 * 1024),
                PrivateMemoryMB = process.PrivateMemorySize64 / (1024 * 1024),
                ThreadCount = process.Threads.Count,
                HandleCount = process.HandleCount,
                Uptime = now - process.StartTime.ToUniversalTime(),
                TotalProcessorTime = cpuTime
            };
        }
        catch
        {
            _initialized = false;
            return null;
        }
    }

    /// <summary>Reset the tracker state (e.g. when a process restarts).</summary>
    public void Reset()
    {
        _initialized = false;
        _lastCpuPercent = 0;
    }
}
