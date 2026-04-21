using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace ServerEditorWeb.Services;

public class RuntimeViewerProcessService : IDisposable
{
    private Process? _viewerProcess;
    private readonly object _outputLock = new();

    public bool IsViewerRunning { get; private set; }
    public Process? ViewerProcess => _viewerProcess;
    public event Action? StateChanged;

    /// <summary>True when the tracked process was started externally (not by this editor).</summary>
    public bool IsExternalProcess { get; private set; }

    /// <summary>Captured stdout/stderr lines from the viewer process.</summary>
    public List<string> OutputLines { get; } = new();

    /// <summary>Raised when a new output line is captured.</summary>
    public event Action? OutputChanged;

    /// <summary>Clears all captured output lines.</summary>
    public void ClearOutput()
    {
        lock (_outputLock) { OutputLines.Clear(); }
        OutputChanged?.Invoke();
    }

    private void AppendOutput(string line)
    {
        lock (_outputLock)
        {
            OutputLines.Add(line);
            if (OutputLines.Count > 500)
                OutputLines.RemoveAt(0);
        }
        OutputChanged?.Invoke();
    }

    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    /// <summary>True when running inside a Docker container (entrypoint manages processes).</summary>
    public static bool IsDocker { get; } = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
                                          || File.Exists("/.dockerenv");

    public string ServiceName { get; set; } = "SimpleOpcRuntimeViewer";
    public string SystemdServiceName { get; set; } = "simpleopcruntimeviewer";

    // ─── Process ─────────────────────────────────────────────

    public (bool success, string message) StartViewer(string nodesPath, int port, bool kiosk, string mode = "web")
    {
        if (IsViewerRunning)
            return (false, "RuntimeViewer is already running.");

        // Check if there's already an external viewer running for this config
        DetectExternalProcess(nodesPath);
        if (IsViewerRunning)
            return (false, "RuntimeViewer is already running externally. Use Stop to terminate it first.");

        bool isDesktop = mode == "desktop";

        // For web mode, check that the HTTP port is available before starting
        if (!isDesktop)
        {
            this.AppendOutput($"[Port Check] Checking availability: HTTP port {port}");
            if (ServerProcessService.IsPortInUse(port))
            {
                var errorMsg = $"❌ Cannot start RuntimeViewer: HTTP port {port} is already in use. Please stop any running viewer instances, change the port number, or use Desktop mode which auto-selects a free port.";
                this.AppendOutput($"[Port Check] {errorMsg}");
                return (false, errorMsg);
            }
            this.AppendOutput($"[Port Check] OK: HTTP port {port} is available");
        }

        string? viewerExe = isDesktop
            ? FindDesktopExecutable(nodesPath)
            : FindViewerExecutable(nodesPath);

        if (viewerExe == null)
        {
            var target = isDesktop ? "RuntimeViewer.Desktop" : "RuntimeViewer";
            return (false, $"Could not find {target} executable. Please ensure the {target} project is built.");
        }

        try
        {
            string fullNodesPath = Path.GetFullPath(nodesPath);
            string nodesDir = Path.GetDirectoryName(fullNodesPath) ?? ".";
            string exeDir = Path.GetDirectoryName(viewerExe) ?? ".";

            // When the executable is "dotnet" (Docker), we must pass RuntimeViewer.dll as first arg
            var args = viewerExe == "dotnet"
                ? $"/opt/hmi/viewer/RuntimeViewer.dll \"{fullNodesPath}\""
                : $"\"{fullNodesPath}\"";
            if (kiosk)
                args += " --kiosk";

            ClearOutput();

            _viewerProcess = new Process();
            _viewerProcess.StartInfo.FileName = viewerExe;
            // Use the executable's own directory as working directory so native libraries
            // (Photino.Native, WebView2Loader, etc.) can be found via runtimes/ subfolder.
            _viewerProcess.StartInfo.WorkingDirectory = viewerExe == "dotnet" ? "/opt/hmi/viewer" : exeDir;
            _viewerProcess.StartInfo.Arguments = args;
            _viewerProcess.StartInfo.UseShellExecute = false;
            _viewerProcess.StartInfo.RedirectStandardOutput = true;
            _viewerProcess.StartInfo.RedirectStandardError = true;
            _viewerProcess.EnableRaisingEvents = true;

            if (isDesktop)
            {
                // Desktop app: show the window, no port needed
                _viewerProcess.StartInfo.CreateNoWindow = false;
            }
            else
            {
                // Web app: headless, set the HTTP port
                _viewerProcess.StartInfo.CreateNoWindow = true;
                _viewerProcess.StartInfo.EnvironmentVariables["ASPNETCORE_URLS"] = $"http://*:{port}";
            }

            // Capture stderr asynchronously for diagnostics on early exit
            string? earlyStderr = null;
            _viewerProcess.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                    AppendOutput(e.Data);
            };
            _viewerProcess.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    earlyStderr = (earlyStderr == null ? "" : earlyStderr + "\n") + e.Data;
                    AppendOutput($"[ERR] {e.Data}");
                }
            };

            _viewerProcess.Exited += (s, e) =>
            {
                IsViewerRunning = false;
                IsExternalProcess = false;
                StateChanged?.Invoke();
            };

            _viewerProcess.Start();
            _viewerProcess.BeginOutputReadLine();
            _viewerProcess.BeginErrorReadLine();

            // Give the process a brief moment to fail if there's a startup crash
            if (_viewerProcess.WaitForExit(1500))
            {
                // Process already exited — it crashed on startup
                int exitCode = _viewerProcess.ExitCode;
                _viewerProcess.Dispose();
                _viewerProcess = null;
                IsViewerRunning = false;

                var detail = !string.IsNullOrWhiteSpace(earlyStderr)
                    ? $"\n{earlyStderr}"
                    : "";
                return (false, $"RuntimeViewer exited immediately (exit code {exitCode}).{detail}");
            }

            IsViewerRunning = true;
            IsExternalProcess = false;
            StateChanged?.Invoke();
            return (true, isDesktop
                ? "RuntimeViewer Desktop started."
                : $"RuntimeViewer started on port {port}.");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to start RuntimeViewer: {ex.Message}");
        }
    }

    public (bool success, string message) StopViewer()
    {
        if (_viewerProcess == null || _viewerProcess.HasExited)
        {
            IsViewerRunning = false;
            IsExternalProcess = false;
            StateChanged?.Invoke();
            return (false, "RuntimeViewer is not running.");
        }

        try
        {
            _viewerProcess.Kill(entireProcessTree: true);
            _viewerProcess.Dispose();
            _viewerProcess = null;
            IsViewerRunning = false;
            IsExternalProcess = false;
            StateChanged?.Invoke();
            return (true, "RuntimeViewer stopped.");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to stop RuntimeViewer: {ex.Message}");
        }
    }

    // ─── External Process Detection ──────────────────────────

    /// <summary>
    /// Scan running processes for a RuntimeViewer instance matching the given config path.
    /// Checks both web and desktop process names.
    /// </summary>
    public void DetectExternalProcess(string nodesPath)
    {
        if (_viewerProcess != null && !_viewerProcess.HasExited)
            return;

        var fullConfigPath = Path.GetFullPath(nodesPath);
        // Check both RuntimeViewer (web) and RuntimeViewer.Desktop process names
        var proc = FindRunningProcess("RuntimeViewer", fullConfigPath)
                ?? FindRunningProcess("RuntimeViewer.Desktop", fullConfigPath)
                ?? (IsDocker ? FindDotnetProcess("RuntimeViewer.dll") : null);
        if (proc != null)
        {
            _viewerProcess = proc;
            _viewerProcess.EnableRaisingEvents = true;
            _viewerProcess.Exited += (s, e) =>
            {
                IsViewerRunning = false;
                IsExternalProcess = false;
                StateChanged?.Invoke();
            };
            IsViewerRunning = true;
            IsExternalProcess = true;
            StateChanged?.Invoke();
        }
        else
        {
            if (IsViewerRunning && _viewerProcess == null)
            {
                IsViewerRunning = false;
                IsExternalProcess = false;
                StateChanged?.Invoke();
            }
        }
    }

    // ─── Windows Service ─────────────────────────────────────

    public bool IsServiceInstalled()
    {
        if (!IsWindows) return false;
        try
        {
            using var sc = new ServiceController(ServiceName);
            _ = sc.Status;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public ServiceControllerStatus GetServiceStatus()
    {
        if (!IsWindows) return 0;
        try
        {
            using var sc = new ServiceController(ServiceName);
            sc.Refresh();
            return sc.Status;
        }
        catch
        {
            return (ServiceControllerStatus)0;
        }
    }

    public async Task<string> InstallServiceAsync(string nodesPath, int port, bool kiosk)
    {
        if (!IsWindows) return "Windows services are not available on this platform.";
        string? viewerExe = FindViewerExecutable(nodesPath);
        if (viewerExe == null) return "RuntimeViewer executable not found.";

        string configPath = Path.GetFullPath(nodesPath);
        var extraArgs = kiosk ? " --kiosk" : "";
        string binPath = $"\\\"{viewerExe}\\\" \\\"{configPath}\\\"{extraArgs}";
        string displayName = $"Simple OPC RuntimeViewer - {Path.GetFileNameWithoutExtension(nodesPath)}";

        // The port is set via environment variable in the service; for sc create we set it in the binPath
        // Actually for Windows services we embed the URL via --urls
        binPath = $"\\\"{viewerExe}\\\" \\\"{configPath}\\\"{extraArgs} --urls http://*:{port}";

        return await RunCommandAsync("sc", $"create \"{ServiceName}\" binPath= \"{binPath}\" displayName= \"{displayName}\" start= auto");
    }

    public async Task<string> UninstallServiceAsync()
    {
        if (!IsWindows) return "Windows services are not available on this platform.";
        try
        {
            using var sc = new ServiceController(ServiceName);
            if (sc.Status != ServiceControllerStatus.Stopped)
            {
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            }
        }
        catch { }

        return await RunCommandAsync("sc", $"delete \"{ServiceName}\"");
    }

    public async Task<string> StartServiceAsync()
    {
        if (!IsWindows) return "Windows services are not available on this platform.";
        return await RunCommandAsync("sc", $"start \"{ServiceName}\"");
    }

    public async Task<string> StopServiceAsync()
    {
        if (!IsWindows) return "Windows services are not available on this platform.";
        return await RunCommandAsync("sc", $"stop \"{ServiceName}\"");
    }

    // ─── Linux systemd ───────────────────────────────────────

    public async Task<bool> IsSystemdServiceInstalledAsync()
    {
        if (!IsLinux) return false;
        var output = await RunCommandAsync("systemctl", $"list-unit-files {SystemdServiceName}.service --no-pager --plain");
        return output.Contains(SystemdServiceName);
    }

    public async Task<string> GetSystemdStatusAsync()
    {
        if (!IsLinux) return "N/A";
        var output = await RunCommandAsync("systemctl", $"is-active {SystemdServiceName}.service");
        return output.Trim();
    }

    public string GenerateSystemdUnitFile(string nodesPath, int port, bool kiosk)
    {
        var viewerExe = FindViewerExecutable(nodesPath);
        var fullNodesPath = Path.GetFullPath(nodesPath);
        var workingDir = Path.GetDirectoryName(fullNodesPath) ?? "/opt/simpleopcruntimeviewer";
        var kioskArg = kiosk ? " --kiosk" : "";

        return $"""
            [Unit]
            Description=Simple OPC RuntimeViewer
            After=network.target

            [Service]
            Type=simple
            WorkingDirectory={workingDir}
            ExecStart={viewerExe ?? "/usr/local/bin/RuntimeViewer"} "{fullNodesPath}"{kioskArg}
            Environment=ASPNETCORE_URLS=http://*:{port}
            Restart=on-failure
            RestartSec=5
            User=root

            [Install]
            WantedBy=multi-user.target
            """;
    }

    public async Task<string> InstallSystemdServiceAsync(string nodesPath, int port, bool kiosk)
    {
        if (!IsLinux) return "systemd is not available on this platform.";

        try
        {
            var unitContent = GenerateSystemdUnitFile(nodesPath, port, kiosk);
            var unitPath = $"/etc/systemd/system/{SystemdServiceName}.service";
            await File.WriteAllTextAsync(unitPath, unitContent);
            await RunCommandAsync("systemctl", "daemon-reload");
            await RunCommandAsync("systemctl", $"enable {SystemdServiceName}.service");
            return $"Service installed at {unitPath} and enabled.";
        }
        catch (Exception ex)
        {
            return $"Error installing systemd service: {ex.Message}";
        }
    }

    public async Task<string> UninstallSystemdServiceAsync()
    {
        if (!IsLinux) return "systemd is not available on this platform.";

        try
        {
            await RunCommandAsync("systemctl", $"stop {SystemdServiceName}.service");
            await RunCommandAsync("systemctl", $"disable {SystemdServiceName}.service");
            var unitPath = $"/etc/systemd/system/{SystemdServiceName}.service";
            if (File.Exists(unitPath))
                File.Delete(unitPath);
            await RunCommandAsync("systemctl", "daemon-reload");
            return "Service uninstalled.";
        }
        catch (Exception ex)
        {
            return $"Error uninstalling systemd service: {ex.Message}";
        }
    }

    public async Task<string> StartSystemdServiceAsync()
    {
        if (!IsLinux) return "systemd is not available on this platform.";
        return await RunCommandAsync("systemctl", $"start {SystemdServiceName}.service");
    }

    public async Task<string> StopSystemdServiceAsync()
    {
        if (!IsLinux) return "systemd is not available on this platform.";
        return await RunCommandAsync("systemctl", $"stop {SystemdServiceName}.service");
    }

    public async Task<string> GetSystemdLogsAsync(int lines = 50)
    {
        if (!IsLinux) return "";
        return await RunCommandAsync("journalctl", $"-u {SystemdServiceName}.service --no-pager -n {lines}");
    }

    // ─── Helpers ─────────────────────────────────────────────

    private string? FindViewerExecutable(string nodesPath)
    {
        string editorDir = AppContext.BaseDirectory;
        string exeName = IsWindows ? "RuntimeViewer.exe" : "RuntimeViewer";

        // 1. Adjacent to editor
        string adjacent = Path.Combine(editorDir, exeName);
        if (File.Exists(adjacent)) return adjacent;

        // 2. Relative to editor project structure (development)
        var dir = new DirectoryInfo(editorDir);
        for (int i = 0; i < 6; i++)
        {
            if (dir == null) break;
            if (Directory.Exists(Path.Combine(dir.FullName, "RuntimeViewer")))
            {
                var projectDir = Path.Combine(dir.FullName, "RuntimeViewer");
                var debugPath = Path.Combine(projectDir, "bin", "Debug", "net10.0", exeName);
                var releasePath = Path.Combine(projectDir, "bin", "Release", "net10.0", exeName);
                if (File.Exists(debugPath)) return debugPath;
                if (File.Exists(releasePath)) return releasePath;
            }
            dir = dir.Parent;
        }

        // 3. Relative to config file
        string configDir = Path.GetDirectoryName(Path.GetFullPath(nodesPath)) ?? ".";
        string[] attempts =
        [
            Path.Combine(configDir, "bin", "Debug", "net10.0", exeName),
            Path.Combine(configDir, "bin", "Release", "net10.0", exeName),
            Path.Combine(configDir, exeName)
        ];

        foreach (var p in attempts)
        {
            if (File.Exists(p)) return p;
        }

        // 4. Linux: check common install paths
        if (IsLinux)
        {
            string[] linuxPaths = ["/usr/local/bin/RuntimeViewer", "/opt/simpleopcruntimeviewer/RuntimeViewer", "/opt/hmi/viewer/RuntimeViewer"];
            foreach (var p in linuxPaths)
            {
                if (File.Exists(p)) return p;
            }

            // Docker: framework-dependent (no native exe), use "dotnet RuntimeViewer.dll"
            if (IsDocker && File.Exists("/opt/hmi/viewer/RuntimeViewer.dll"))
                return "dotnet";
        }

        return null;
    }

    private string? FindDesktopExecutable(string nodesPath)
    {
        string editorDir = AppContext.BaseDirectory;
        string exeName = IsWindows ? "RuntimeViewer.Desktop.exe" : "RuntimeViewer.Desktop";

        // 1. Adjacent to editor
        string adjacent = Path.Combine(editorDir, exeName);
        if (File.Exists(adjacent)) return adjacent;

        // 2. Relative to editor project structure (development)
        var dir = new DirectoryInfo(editorDir);
        for (int i = 0; i < 6; i++)
        {
            if (dir == null) break;
            if (Directory.Exists(Path.Combine(dir.FullName, "RuntimeViewer.Desktop")))
            {
                var projectDir = Path.Combine(dir.FullName, "RuntimeViewer.Desktop");
                var debugPath = Path.Combine(projectDir, "bin", "Debug", "net10.0", exeName);
                var releasePath = Path.Combine(projectDir, "bin", "Release", "net10.0", exeName);
                if (File.Exists(debugPath)) return debugPath;
                if (File.Exists(releasePath)) return releasePath;
            }
            dir = dir.Parent;
        }

        // 3. Relative to config file
        string configDir = Path.GetDirectoryName(Path.GetFullPath(nodesPath)) ?? ".";
        string[] attempts =
        [
            Path.Combine(configDir, "bin", "Debug", "net10.0", exeName),
            Path.Combine(configDir, "bin", "Release", "net10.0", exeName),
            Path.Combine(configDir, exeName)
        ];

        foreach (var p in attempts)
        {
            if (File.Exists(p)) return p;
        }

        // 4. Linux: check common install paths
        if (IsLinux)
        {
            string[] linuxPaths =
            [
                "/usr/local/bin/RuntimeViewer.Desktop",
                "/opt/simpleopcruntimeviewer/RuntimeViewer.Desktop"
            ];
            foreach (var p in linuxPaths)
            {
                if (File.Exists(p)) return p;
            }
        }

        return null;
    }

    private async Task<string> RunCommandAsync(string fileName, string arguments)
    {
        var tcs = new TaskCompletionSource<string>();

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        process.Exited += (sender, args) =>
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            tcs.SetResult(process.ExitCode == 0 ? output : $"Error: {error}\nOutput: {output}");
            process.Dispose();
        };

        try
        {
            process.Start();
            return await tcs.Task;
        }
        catch (Exception ex)
        {
            return $"Error executing {fileName}: {ex.Message}";
        }
    }

    /// <summary>
    /// Find a running process by name whose command line contains the given config path.
    /// </summary>
    private static Process? FindRunningProcess(string processName, string configPath)
    {
        var normalizedConfig = NormalizePath(configPath);
        var myPid = Environment.ProcessId;

        try
        {
            foreach (var proc in Process.GetProcessesByName(processName))
            {
                if (proc.Id == myPid)
                {
                    proc.Dispose();
                    continue;
                }
                try
                {
                    if (proc.HasExited)
                    {
                        proc.Dispose();
                        continue;
                    }

                    var cmdLine = GetCommandLine(proc);
                    if (!string.IsNullOrEmpty(cmdLine) &&
                        cmdLine.Contains(normalizedConfig, StringComparison.OrdinalIgnoreCase))
                    {
                        return proc;
                    }
                }
                catch { }
                proc.Dispose();
            }
        }
        catch { }

        return null;
    }

    private static string NormalizePath(string path) =>
        Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private static string? GetCommandLine(Process process)
    {
        try
        {
            if (IsWindows) return GetCommandLineWindows(process.Id);
            if (IsLinux) return GetCommandLineLinux(process.Id);
        }
        catch { }
        return null;
    }

    private static string? GetCommandLineWindows(int pid)
    {
#pragma warning disable CA1416
        using var searcher = new ManagementObjectSearcher(
            $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {pid}");
        foreach (var obj in searcher.Get())
            return obj["CommandLine"]?.ToString();
#pragma warning restore CA1416
        return null;
    }

    private static string? GetCommandLineLinux(int pid)
    {
        var path = $"/proc/{pid}/cmdline";
        if (File.Exists(path))
            return File.ReadAllText(path).Replace('\0', ' ').Trim();
        return null;
    }

    /// <summary>
    /// Find a running 'dotnet' process whose command line contains the given DLL name.
    /// Used inside Docker where processes are launched via 'dotnet Xxx.dll'.
    /// </summary>
    private static Process? FindDotnetProcess(string dllName)
    {
        var myPid = Environment.ProcessId;
        try
        {
            foreach (var proc in Process.GetProcessesByName("dotnet"))
            {
                if (proc.Id == myPid) { proc.Dispose(); continue; }
                try
                {
                    if (proc.HasExited) { proc.Dispose(); continue; }
                    var cmdLine = GetCommandLine(proc);
                    if (!string.IsNullOrEmpty(cmdLine) &&
                        cmdLine.Contains(dllName, StringComparison.OrdinalIgnoreCase))
                    {
                        return proc;
                    }
                }
                catch { /* access denied or exited */ }
                proc.Dispose();
            }
        }
        catch { }
        return null;
    }

    public void Dispose()
    {
        if (_viewerProcess != null && !_viewerProcess.HasExited)
        {
            if (!IsExternalProcess)
            {
                try { _viewerProcess.Kill(); } catch { }
            }
            _viewerProcess.Dispose();
        }
    }
}
