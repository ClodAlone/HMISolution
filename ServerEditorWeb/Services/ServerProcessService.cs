using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace ServerEditorWeb.Services;

public class ServerProcessService : IDisposable
{
    private Process? _serverProcess;

    public bool IsServerRunning { get; private set; }
    public Process? ServerProcess => _serverProcess;
    public event Action? StateChanged;

    /// <summary>True when the tracked process was started externally (not by this editor).</summary>
    public bool IsExternalProcess { get; private set; }

    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    // Windows Service management
    public string ServiceName { get; set; } = "SimpleOpcFileServer";

    // Linux systemd service name
    public string SystemdServiceName { get; set; } = "simpleopcfileserver";

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

    public (bool success, string message) StartServer(string nodesPath)
    {
        if (IsServerRunning)
            return (false, "Server is already running.");

        // Check if there's already an external server running for this config
        DetectExternalProcess(nodesPath);
        if (IsServerRunning)
            return (false, "Server is already running externally. Use Stop to terminate it first.");

        string? serverExe = FindServerExecutable(nodesPath);
        if (serverExe == null)
            return (false, $"Could not find Server executable. Please ensure the Server project is built.");

        try
        {
            string fullNodesPath = Path.GetFullPath(nodesPath);
            string nodesDir = Path.GetDirectoryName(fullNodesPath) ?? ".";

            _serverProcess = new Process();
            _serverProcess.StartInfo.FileName = serverExe;
            _serverProcess.StartInfo.WorkingDirectory = nodesDir;
            _serverProcess.StartInfo.Arguments = $"\"{fullNodesPath}\"";
            _serverProcess.StartInfo.UseShellExecute = false;
            _serverProcess.StartInfo.CreateNoWindow = true;
            _serverProcess.EnableRaisingEvents = true;
            _serverProcess.Exited += (s, e) =>
            {
                IsServerRunning = false;
                IsExternalProcess = false;
                StateChanged?.Invoke();
            };

            _serverProcess.Start();
            IsServerRunning = true;
            IsExternalProcess = false;
            StateChanged?.Invoke();
            return (true, "Server started.");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to start server: {ex.Message}");
        }
    }

    public (bool success, string message) StopServer()
    {
        if (_serverProcess == null || _serverProcess.HasExited)
        {
            IsServerRunning = false;
            IsExternalProcess = false;
            StateChanged?.Invoke();
            return (false, "Server is not running.");
        }

        try
        {
            _serverProcess.Kill(entireProcessTree: true);
            _serverProcess.Dispose();
            _serverProcess = null;
            IsServerRunning = false;
            IsExternalProcess = false;
            StateChanged?.Invoke();
            return (true, "Server stopped.");
        }
        catch (Exception ex)
        {
            return (false, $"Failed to stop server: {ex.Message}");
        }
    }

    // ─── External Process Detection ──────────────────────────

    /// <summary>
    /// Scan running processes for a Server instance matching the given config path.
    /// If found, adopt it so the editor can track and stop it.
    /// </summary>
    public void DetectExternalProcess(string nodesPath)
    {
        // Already tracking a live process
        if (_serverProcess != null && !_serverProcess.HasExited)
            return;

        var fullConfigPath = Path.GetFullPath(nodesPath);
        var proc = FindRunningProcess("Server", fullConfigPath);
        if (proc != null)
        {
            _serverProcess = proc;
            _serverProcess.EnableRaisingEvents = true;
            _serverProcess.Exited += (s, e) =>
            {
                IsServerRunning = false;
                IsExternalProcess = false;
                StateChanged?.Invoke();
            };
            IsServerRunning = true;
            IsExternalProcess = true;
            StateChanged?.Invoke();
        }
        else
        {
            // No external process found — if we thought one was running, clear state
            if (IsServerRunning && _serverProcess == null)
            {
                IsServerRunning = false;
                IsExternalProcess = false;
                StateChanged?.Invoke();
            }
        }
    }

    // ─── Windows Service ─────────────────────────────────────

    public async Task<string> InstallServiceAsync(string nodesPath)
    {
        if (!IsWindows) return "Windows services are not available on this platform.";
        string? serverExe = FindServerExecutable(nodesPath);
        if (serverExe == null) return "Server executable not found.";

        string configPath = Path.GetFullPath(nodesPath);
        string binPath = $"\\\"{serverExe}\\\" \\\"{configPath}\\\"";
        string displayName = $"Simple OPC File Server - {Path.GetFileNameWithoutExtension(nodesPath)}";
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

    /// <summary>Check if the systemd service unit is installed.</summary>
    public async Task<bool> IsSystemdServiceInstalledAsync()
    {
        if (!IsLinux) return false;
        var output = await RunCommandAsync("systemctl", $"list-unit-files {SystemdServiceName}.service --no-pager --plain");
        return output.Contains(SystemdServiceName);
    }

    /// <summary>Get the active status of the systemd service (active, inactive, failed, etc.).</summary>
    public async Task<string> GetSystemdStatusAsync()
    {
        if (!IsLinux) return "N/A";
        var output = await RunCommandAsync("systemctl", $"is-active {SystemdServiceName}.service");
        return output.Trim();
    }

    /// <summary>Generate the systemd unit file content for the server.</summary>
    public string GenerateSystemdUnitFile(string nodesPath)
    {
        var serverExe = FindServerExecutable(nodesPath);
        var fullNodesPath = Path.GetFullPath(nodesPath);
        var workingDir = Path.GetDirectoryName(fullNodesPath) ?? "/opt/simpleopcfileserver";

        return $"""
            [Unit]
            Description=Simple OPC File Server
            After=network.target

            [Service]
            Type=simple
            WorkingDirectory={workingDir}
            ExecStart={serverExe ?? "/usr/local/bin/Server"} "{fullNodesPath}"
            Restart=on-failure
            RestartSec=5
            User=root

            [Install]
            WantedBy=multi-user.target
            """;
    }

    /// <summary>Install the systemd service unit file and enable it.</summary>
    public async Task<string> InstallSystemdServiceAsync(string nodesPath)
    {
        if (!IsLinux) return "systemd is not available on this platform.";

        try
        {
            var unitContent = GenerateSystemdUnitFile(nodesPath);
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

    /// <summary>Uninstall the systemd service.</summary>
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

    private string? FindServerExecutable(string nodesPath)
    {
        string editorDir = AppContext.BaseDirectory;
        string exeName = IsWindows ? "Server.exe" : "Server";

        // 1. Adjacent to editor
        string adjacent = Path.Combine(editorDir, exeName);
        if (File.Exists(adjacent)) return adjacent;

        // 2. Relative to editor project structure (development)
        var dir = new DirectoryInfo(editorDir);
        for (int i = 0; i < 6; i++)
        {
            if (dir == null) break;
            if (Directory.Exists(Path.Combine(dir.FullName, "Server")))
            {
                var serverProjectDir = Path.Combine(dir.FullName, "Server");
                var debugPath = Path.Combine(serverProjectDir, "bin", "Debug", "net10.0", exeName);
                var releasePath = Path.Combine(serverProjectDir, "bin", "Release", "net10.0", exeName);
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
            string[] linuxPaths = ["/usr/local/bin/Server", "/opt/simpleopcfileserver/Server"];
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
    /// Works cross-platform (WMI on Windows, /proc on Linux).
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
                catch
                {
                    // Access denied or process exited
                }
                proc.Dispose();
            }
        }
        catch
        {
            // Process enumeration failed
        }

        return null;
    }

    private static string NormalizePath(string path)
    {
        return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private static string? GetCommandLine(Process process)
    {
        try
        {
            if (IsWindows)
                return GetCommandLineWindows(process.Id);
            else if (IsLinux)
                return GetCommandLineLinux(process.Id);
        }
        catch { }
        return null;
    }

    private static string? GetCommandLineWindows(int pid)
    {
#pragma warning disable CA1416 // Platform compatibility
        using var searcher = new ManagementObjectSearcher(
            $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {pid}");
        foreach (var obj in searcher.Get())
        {
            return obj["CommandLine"]?.ToString();
        }
#pragma warning restore CA1416
        return null;
    }

    private static string? GetCommandLineLinux(int pid)
    {
        var cmdLinePath = $"/proc/{pid}/cmdline";
        if (File.Exists(cmdLinePath))
        {
            var raw = File.ReadAllText(cmdLinePath);
            return raw.Replace('\0', ' ').Trim();
        }
        return null;
    }

    public void Dispose()
    {
        if (_serverProcess != null && !_serverProcess.HasExited)
        {
            // Only kill processes we started ourselves
            if (!IsExternalProcess)
            {
                try { _serverProcess.Kill(); } catch { }
            }
            _serverProcess.Dispose();
        }
    }
}
