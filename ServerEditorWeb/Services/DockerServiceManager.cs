// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServerEditorWeb.Services;

/// <summary>
/// Manages multiple HMI Server and RuntimeViewer process instances inside a Docker container.
/// Persists the desired configuration to <c>/data/.hmi-services.json</c> so it survives container restarts.
/// The entrypoint script reads this file on boot to auto-start saved instances.
/// </summary>
public class DockerServiceManager : IDisposable
{
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetEnvironmentVariable("HMI_DATA") ?? "/data",
        ".hmi-services.json");

    private readonly object _lock = new();
    private readonly List<ManagedInstance> _instances = new();
    private Timer? _watchdog;

    /// <summary>Current managed instances (snapshot).</summary>
    public IReadOnlyList<ManagedInstance> Instances
    {
        get { lock (_lock) return _instances.ToList(); }
    }

    public event Action? StateChanged;

    public DockerServiceManager()
    {
        // Load persisted config and detect already-running processes
        LoadConfig();
        DetectRunningInstances();

        // Watchdog: every 5 s check if processes are still alive
        _watchdog = new Timer(_ => Watchdog(), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    // ─── Public API ─────────────────────────────────────────────

    /// <summary>Start a new server instance for the given project file.</summary>
    public (bool success, string message) StartServer(string projectPath)
    {
        lock (_lock)
        {
            var fullPath = Path.GetFullPath(projectPath);

            // Check if already running
            var existing = _instances.FirstOrDefault(i =>
                i.Kind == InstanceKind.Server &&
                string.Equals(i.ProjectPath, fullPath, StringComparison.OrdinalIgnoreCase));

            if (existing != null && existing.IsRunning)
                return (false, $"Server for '{Path.GetFileName(fullPath)}' is already running (PID {existing.Pid}).");

            var (proc, error) = LaunchDotnet("/opt/hmi/server/Server.dll", fullPath, null);
            if (proc == null)
                return (false, $"Failed to start server: {error}");

            var instance = existing ?? new ManagedInstance();
            instance.Kind = InstanceKind.Server;
            instance.ProjectPath = fullPath;
            instance.Pid = proc.Id;
            instance.IsRunning = true;
            instance.AutoStart = true;
            instance.Process = proc;
            instance.StartedUtc = DateTime.UtcNow;

            if (existing == null)
                _instances.Add(instance);

            AttachExitHandler(instance);
            SaveConfig();
            StateChanged?.Invoke();
            return (true, $"Server started for '{Path.GetFileName(fullPath)}' (PID {proc.Id}).");
        }
    }

    /// <summary>Start a RuntimeViewer instance for the given project file.</summary>
    public (bool success, string message) StartViewer(string projectPath, int port)
    {
        lock (_lock)
        {
            var fullPath = Path.GetFullPath(projectPath);

            var existing = _instances.FirstOrDefault(i =>
                i.Kind == InstanceKind.Viewer &&
                string.Equals(i.ProjectPath, fullPath, StringComparison.OrdinalIgnoreCase));

            if (existing != null && existing.IsRunning)
                return (false, $"Viewer for '{Path.GetFileName(fullPath)}' is already running (PID {existing.Pid}).");

            var envVars = new Dictionary<string, string> { ["ASPNETCORE_URLS"] = $"http://+:{port}" };
            var (proc, error) = LaunchDotnet("/opt/hmi/viewer/RuntimeViewer.dll", fullPath, envVars);
            if (proc == null)
                return (false, $"Failed to start viewer: {error}");

            var instance = existing ?? new ManagedInstance();
            instance.Kind = InstanceKind.Viewer;
            instance.ProjectPath = fullPath;
            instance.Port = port;
            instance.Pid = proc.Id;
            instance.IsRunning = true;
            instance.AutoStart = true;
            instance.Process = proc;
            instance.StartedUtc = DateTime.UtcNow;

            if (existing == null)
                _instances.Add(instance);

            AttachExitHandler(instance);
            SaveConfig();
            StateChanged?.Invoke();
            return (true, $"Viewer started on port {port} for '{Path.GetFileName(fullPath)}' (PID {proc.Id}).");
        }
    }

    /// <summary>Stop a specific instance by its ID.</summary>
    public (bool success, string message) StopInstance(string instanceId)
    {
        lock (_lock)
        {
            var instance = _instances.FirstOrDefault(i => i.Id == instanceId);
            if (instance == null)
                return (false, "Instance not found.");

            if (!instance.IsRunning)
                return (false, "Instance is not running.");

            try
            {
                if (instance.Process != null && !instance.Process.HasExited)
                {
                    instance.Process.Kill(entireProcessTree: true);
                    instance.Process.Dispose();
                }
                else if (instance.Pid > 0)
                {
                    // Process was started by entrypoint — kill by PID
                    try
                    {
                        var proc = Process.GetProcessById(instance.Pid);
                        proc.Kill(entireProcessTree: true);
                        proc.Dispose();
                    }
                    catch { /* already dead */ }
                }
            }
            catch { /* process already exited */ }

            instance.IsRunning = false;
            instance.Process = null;
            instance.Pid = 0;
            SaveConfig();
            StateChanged?.Invoke();
            return (true, $"Stopped {instance.Kind} instance.");
        }
    }

    /// <summary>Remove an instance entirely (stops it first if running).</summary>
    public (bool success, string message) RemoveInstance(string instanceId)
    {
        lock (_lock)
        {
            var instance = _instances.FirstOrDefault(i => i.Id == instanceId);
            if (instance == null)
                return (false, "Instance not found.");

            if (instance.IsRunning)
                StopInstance(instanceId);

            _instances.Remove(instance);
            SaveConfig();
            StateChanged?.Invoke();
            return (true, $"Removed {instance.Kind} instance.");
        }
    }

    /// <summary>Toggle whether an instance auto-starts on container boot.</summary>
    public void SetAutoStart(string instanceId, bool autoStart)
    {
        lock (_lock)
        {
            var instance = _instances.FirstOrDefault(i => i.Id == instanceId);
            if (instance == null) return;
            instance.AutoStart = autoStart;
            SaveConfig();
            StateChanged?.Invoke();
        }
    }

    /// <summary>List all sample project files available in the data directory.</summary>
    public List<string> GetAvailableProjects()
    {
        var dataDir = Environment.GetEnvironmentVariable("HMI_DATA") ?? "/data";
        if (!Directory.Exists(dataDir))
            return [];

        return Directory.GetFiles(dataDir, "nodes.json", SearchOption.AllDirectories)
            .OrderBy(p => p)
            .ToList();
    }

    // ─── Internals ──────────────────────────────────────────────

    private static (Process? proc, string? error) LaunchDotnet(string dllPath, string projectPath, Dictionary<string, string>? envVars)
    {
        if (!File.Exists(dllPath))
            return (null, $"DLL not found: {dllPath}");

        var workingDir = Path.GetDirectoryName(dllPath) ?? "/opt/hmi/server";
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"{dllPath} \"{projectPath}\"",
            WorkingDirectory = workingDir,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = false,
            RedirectStandardError = false
        };

        if (envVars != null)
        {
            foreach (var kv in envVars)
                psi.EnvironmentVariables[kv.Key] = kv.Value;
        }

        try
        {
            var proc = Process.Start(psi);
            if (proc == null)
                return (null, "Process.Start returned null.");

            // Brief check that it didn't crash immediately
            if (proc.WaitForExit(500))
                return (null, $"Process exited immediately with code {proc.ExitCode}.");

            return (proc, null);
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    }

    private void AttachExitHandler(ManagedInstance instance)
    {
        if (instance.Process == null) return;
        instance.Process.EnableRaisingEvents = true;
        instance.Process.Exited += (_, _) =>
        {
            lock (_lock)
            {
                instance.IsRunning = false;
                instance.Pid = 0;
                instance.Process = null;
            }
            StateChanged?.Invoke();
        };
    }

    private void Watchdog()
    {
        bool changed = false;
        lock (_lock)
        {
            foreach (var inst in _instances)
            {
                if (!inst.IsRunning) continue;
                bool alive = false;
                try
                {
                    if (inst.Process != null)
                        alive = !inst.Process.HasExited;
                    else if (inst.Pid > 0)
                    {
                        var p = Process.GetProcessById(inst.Pid);
                        alive = !p.HasExited;
                        p.Dispose();
                    }
                }
                catch { /* process not found */ }

                if (!alive)
                {
                    inst.IsRunning = false;
                    inst.Process = null;
                    inst.Pid = 0;
                    changed = true;
                }
            }
        }
        if (changed)
            StateChanged?.Invoke();
    }

    /// <summary>On startup, detect processes that the entrypoint started before the editor.</summary>
    private void DetectRunningInstances()
    {
        lock (_lock)
        {
            foreach (var inst in _instances.Where(i => !i.IsRunning && i.Pid == 0))
            {
                // Try to find a matching dotnet process
                var proc = FindDotnetProcess(inst.Kind == InstanceKind.Server ? "Server.dll" : "RuntimeViewer.dll", inst.ProjectPath);
                if (proc != null)
                {
                    inst.Process = proc;
                    inst.Pid = proc.Id;
                    inst.IsRunning = true;
                    AttachExitHandler(inst);
                }
            }

            // Also detect instances started by entrypoint that aren't in config yet
            DetectEntrypointProcesses("Server.dll", InstanceKind.Server);
            DetectEntrypointProcesses("RuntimeViewer.dll", InstanceKind.Viewer);
        }
    }

    private void DetectEntrypointProcesses(string dllName, InstanceKind kind)
    {
        try
        {
            foreach (var proc in Process.GetProcessesByName("dotnet"))
            {
                try
                {
                    if (proc.HasExited) { proc.Dispose(); continue; }
                    if (proc.Id == Environment.ProcessId) { proc.Dispose(); continue; }

                    var cmdLine = GetCmdLine(proc.Id);
                    if (string.IsNullOrEmpty(cmdLine) || !cmdLine.Contains(dllName, StringComparison.OrdinalIgnoreCase))
                    {
                        proc.Dispose();
                        continue;
                    }

                    // Check if we already track this PID
                    if (_instances.Any(i => i.Pid == proc.Id))
                    {
                        proc.Dispose();
                        continue;
                    }

                    // Extract project path from command line
                    var projectPath = ExtractProjectPath(cmdLine);

                    var inst = new ManagedInstance
                    {
                        Kind = kind,
                        ProjectPath = projectPath ?? "unknown",
                        Pid = proc.Id,
                        IsRunning = true,
                        AutoStart = true,
                        Process = proc,
                        StartedUtc = DateTime.UtcNow
                    };
                    AttachExitHandler(inst);
                    _instances.Add(inst);
                }
                catch
                {
                    proc.Dispose();
                }
            }
        }
        catch { }
    }

    private Process? FindDotnetProcess(string dllName, string projectPath)
    {
        try
        {
            foreach (var proc in Process.GetProcessesByName("dotnet"))
            {
                try
                {
                    if (proc.HasExited || proc.Id == Environment.ProcessId)
                    {
                        proc.Dispose();
                        continue;
                    }

                    var cmdLine = GetCmdLine(proc.Id);
                    if (!string.IsNullOrEmpty(cmdLine) &&
                        cmdLine.Contains(dllName, StringComparison.OrdinalIgnoreCase) &&
                        cmdLine.Contains(projectPath, StringComparison.OrdinalIgnoreCase))
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

    private static string? ExtractProjectPath(string cmdLine)
    {
        // Command lines look like: dotnet /opt/hmi/server/Server.dll "/data/WaterTreatment/nodes.json"
        // Try to extract the last quoted or unquoted path
        var parts = cmdLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = parts.Length - 1; i >= 0; i--)
        {
            var part = parts[i].Trim('"');
            if (part.EndsWith(".json", StringComparison.OrdinalIgnoreCase) && File.Exists(part))
                return part;
        }
        return null;
    }

    private static string? GetCmdLine(int pid)
    {
        try
        {
            var cmdLineFile = $"/proc/{pid}/cmdline";
            if (File.Exists(cmdLineFile))
                return File.ReadAllText(cmdLineFile).Replace('\0', ' ');
        }
        catch { }
        return null;
    }

    // ─── Persistence ────────────────────────────────────────────

    private void LoadConfig()
    {
        try
        {
            if (!File.Exists(ConfigPath)) return;
            var json = File.ReadAllText(ConfigPath);
            var entries = JsonSerializer.Deserialize<List<ServiceConfigEntry>>(json, JsonOpts);
            if (entries == null) return;

            lock (_lock)
            {
                _instances.Clear();
                foreach (var e in entries)
                {
                    _instances.Add(new ManagedInstance
                    {
                        Id = e.Id ?? Guid.NewGuid().ToString("N")[..8],
                        Kind = e.Kind,
                        ProjectPath = e.ProjectPath,
                        Port = e.Port,
                        AutoStart = e.AutoStart
                    });
                }
            }
        }
        catch { /* corrupt config — start fresh */ }
    }

    private void SaveConfig()
    {
        try
        {
            List<ServiceConfigEntry> entries;
            lock (_lock)
            {
                entries = _instances.Select(i => new ServiceConfigEntry
                {
                    Id = i.Id,
                    Kind = i.Kind,
                    ProjectPath = i.ProjectPath,
                    Port = i.Port,
                    AutoStart = i.AutoStart
                }).ToList();
            }

            var dir = Path.GetDirectoryName(ConfigPath);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(entries, JsonOpts);
            File.WriteAllText(ConfigPath, json);
        }
        catch { /* best-effort persistence */ }
    }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public void Dispose()
    {
        _watchdog?.Dispose();
    }

    // ─── Models ─────────────────────────────────────────────────

    public enum InstanceKind { Server, Viewer }

    public class ManagedInstance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
        public InstanceKind Kind { get; set; }
        public string ProjectPath { get; set; } = "";
        public int Port { get; set; }
        public bool AutoStart { get; set; } = true;

        // Runtime state (not persisted)
        [JsonIgnore] public bool IsRunning { get; set; }
        [JsonIgnore] public int Pid { get; set; }
        [JsonIgnore] public Process? Process { get; set; }
        [JsonIgnore] public DateTime? StartedUtc { get; set; }

        [JsonIgnore]
        public string DisplayName => Path.GetFileName(Path.GetDirectoryName(ProjectPath)) ?? Path.GetFileName(ProjectPath);
    }

    private class ServiceConfigEntry
    {
        public string? Id { get; set; }
        public InstanceKind Kind { get; set; }
        public string ProjectPath { get; set; } = "";
        public int Port { get; set; }
        public bool AutoStart { get; set; }
    }
}
