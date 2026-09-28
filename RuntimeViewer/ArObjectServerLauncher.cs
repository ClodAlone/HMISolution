// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Diagnostics;
using RuntimeViewer.Shared.Services;

namespace RuntimeViewer;

/// <summary>
/// Auto-launches the bundled local object-recognition server (ArObjectServer.exe) alongside
/// RuntimeViewer when AR mode is enabled and AugmentedRealityConfig.OrServerUrl points at
/// localhost, so an operator doesn't need to start it manually. Controlled by
/// AugmentedRealityConfig.AutoStartOrServer (default true). The child process is stopped when
/// RuntimeViewer shuts down.
/// </summary>
public class ArObjectServerLauncher : IHostedService
{
    private readonly ProjectService _project;
    private readonly ILogger<ArObjectServerLauncher> _logger;
    private Process? _process;

    public ArObjectServerLauncher(ProjectService project, ILogger<ArObjectServerLauncher> logger)
    {
        _project = project;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var config = _project.Settings.AugmentedReality;
        if (config is not { Enabled: true, AutoStartOrServer: true })
            return Task.CompletedTask;

        if (!TryParseLocalPort(config.OrServerUrl, out var port))
        {
            _logger.LogInformation(
                "AR OrServerUrl '{Url}' is not a localhost address — not auto-starting ArObjectServer (assumed to run elsewhere).",
                config.OrServerUrl);
            return Task.CompletedTask;
        }

        var exePath = FindArObjectServerExecutable();
        if (exePath == null)
        {
            _logger.LogWarning(
                "AR mode is enabled with AutoStartOrServer=true, but ArObjectServer executable could not be located. " +
                "Start it manually, or set AugmentedRealityConfig.AutoStartOrServer=false to suppress this message.");
            return Task.CompletedTask;
        }

        try
        {
            var args = $"--port={port}";
            if (!string.IsNullOrWhiteSpace(config.OrModelPath))
                args = $"\"{config.OrModelPath}\" {args}";
            if (config.OrServerUseCuda)
                args += " --cuda";

            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = args,
                    WorkingDirectory = Path.GetDirectoryName(exePath) ?? AppContext.BaseDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            _process.OutputDataReceived += (_, e) => { if (e.Data != null) _logger.LogInformation("[ArObjectServer] {Line}", e.Data); };
            _process.ErrorDataReceived += (_, e) => { if (e.Data != null) _logger.LogWarning("[ArObjectServer] {Line}", e.Data); };

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            _logger.LogInformation("Auto-started local AR object-recognition server: {Exe} {Args}", exePath, args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to auto-start ArObjectServer at {Exe}", exePath);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_process is { HasExited: false })
        {
            try
            {
                _process.Kill(entireProcessTree: true);
                _process.WaitForExit(3000);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to stop auto-started ArObjectServer process");
            }
        }
        _process?.Dispose();
        return Task.CompletedTask;
    }

    private static bool TryParseLocalPort(string? url, out int port)
    {
        port = 0;
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        var isLocal = uri.Host is "127.0.0.1" or "localhost" or "::1";
        if (!isLocal) return false;

        port = uri.Port > 0 ? uri.Port : 8098;
        return true;
    }

    /// <summary>
    /// Locates ArObjectServer(.exe) using the same search strategy as RuntimeViewerProcessService
    /// uses to find RuntimeViewer itself: adjacent to this executable, a sibling project/build
    /// folder (development layout), or a sibling install folder (installer layout).
    /// </summary>
    private static string? FindArObjectServerExecutable()
    {
        var exeName = OperatingSystem.IsWindows() ? "ArObjectServer.exe" : "ArObjectServer";
        var baseDir = AppContext.BaseDirectory;

        // 1. Adjacent to RuntimeViewer's own executable (installer/publish layout)
        var adjacent = Path.Combine(baseDir, exeName);
        if (File.Exists(adjacent)) return adjacent;

        // 2. Sibling "ArObjectServer" folder next to a shared parent (installer layout)
        var parent = Directory.GetParent(baseDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (parent != null)
        {
            var sibling = Path.Combine(parent.FullName, "ArObjectServer", exeName);
            if (File.Exists(sibling)) return sibling;
        }

        // 3. HMI_ROOT environment override
        var hmiRoot = Environment.GetEnvironmentVariable("HMI_ROOT");
        if (!string.IsNullOrWhiteSpace(hmiRoot))
        {
            var envPath = Path.Combine(hmiRoot, "ArObjectServer", exeName);
            if (File.Exists(envPath)) return envPath;
        }

        // 4. Development layout: walk up from this executable looking for a sibling
        // "ArObjectServer" source folder with a Debug/Release build.
        var dir = new DirectoryInfo(baseDir);
        for (int i = 0; i < 8 && dir != null; i++, dir = dir.Parent)
        {
            var projectDir = Path.Combine(dir.FullName, "ArObjectServer");
            if (!Directory.Exists(projectDir)) continue;

            var debugPath = Path.Combine(projectDir, "bin", "Debug", "net10.0", exeName);
            var releasePath = Path.Combine(projectDir, "bin", "Release", "net10.0", exeName);
            if (File.Exists(debugPath)) return debugPath;
            if (File.Exists(releasePath)) return releasePath;
        }

        return null;
    }
}
