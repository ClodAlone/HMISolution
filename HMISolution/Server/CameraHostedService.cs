using Microsoft.Extensions.Hosting;

namespace SimpleOpcFileServer;

/// <summary>
/// Hosted service that starts and stops camera stream processing.
/// </summary>
public sealed class CameraHostedService : IHostedService
{
    private readonly CameraStreamService _cameraService;

    public CameraHostedService(CameraStreamService cameraService)
    {
        _cameraService = cameraService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => _cameraService.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken)
        => _cameraService.StopAsync(cancellationToken);
}
