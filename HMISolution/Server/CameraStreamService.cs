using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Manages camera stream connections and frame processing.
/// Registered as a singleton in DI.
/// </summary>
public sealed class CameraStreamService : IDisposable
{
    private readonly List<CameraConfig> _cameras = new();
    private readonly object _lock = new();

    /// <summary>Configure cameras from the node model.</summary>
    public void Configure(IEnumerable<CameraConfig> cameras)
    {
        lock (_lock)
        {
            _cameras.Clear();
            _cameras.AddRange(cameras);
        }
        Log.Information("CameraStreamService configured with {Count} camera(s)", _cameras.Count);
    }

    /// <summary>Start processing all configured camera streams.</summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Placeholder: connect to camera RTSP/HTTP streams and begin frame analysis.
        return Task.CompletedTask;
    }

    /// <summary>Stop all camera stream processing.</summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Clean up stream connections
    }
}
