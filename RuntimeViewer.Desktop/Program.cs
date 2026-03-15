using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Photino.NET;
using SharedModels;

namespace RuntimeViewer.Desktop;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        // Install crash reporter
        var crashDir = Path.Combine(AppContext.BaseDirectory, "crash_reports");
        CrashReporter.Install(crashDir, "RuntimeViewer.Desktop");

        // Parse CLI arguments
        var isKiosk = args.Any(a => a.Equals("--kiosk", StringComparison.OrdinalIgnoreCase));
        var configPath = args.FirstOrDefault(a => !a.StartsWith("-")) ?? "nodes.json";
        if (!Path.IsPathRooted(configPath))
            configPath = Path.GetFullPath(configPath);

        // Find the RuntimeViewer web executable
        var viewerExe = FindViewerExe();
        if (viewerExe == null)
        {
            Console.Error.WriteLine("RuntimeViewer web executable not found. Build the RuntimeViewer project first.");
            return;
        }

        // Pick a free port
        int port = GetFreePort();
        var url = $"http://127.0.0.1:{port}";

        // Start the RuntimeViewer web process
        var webProcess = new Process();
        webProcess.StartInfo.FileName = viewerExe;
        webProcess.StartInfo.Arguments = $"\"{configPath}\"" + (isKiosk ? " --kiosk" : "");
        webProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(viewerExe)!;
        webProcess.StartInfo.UseShellExecute = false;
        webProcess.StartInfo.CreateNoWindow = true;
        webProcess.StartInfo.EnvironmentVariables["ASPNETCORE_URLS"] = url;
        webProcess.Start();

        try
        {
            // Wait for the web server to be ready
            WaitForServer(url, TimeSpan.FromSeconds(15));
            // Give ASP.NET Core a moment to finish initializing after accepting TCP
            Thread.Sleep(1500);

            // Open a Photino window pointing to the local web server
            var title = $"Runtime Viewer — {Path.GetFileNameWithoutExtension(configPath)}";
            var window = new PhotinoWindow()
                .SetTitle(title)
                .SetUseOsDefaultSize(false)
                .SetSize(1280, 800)
#if DEBUG
                .SetDevToolsEnabled(true)
#endif
                .Load(new Uri(url));

            if (isKiosk)
            {
                window.SetFullScreen(true).SetChromeless(true);
            }

            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    CrashReporter.Report(ex, "AppDomain");
                    Console.Error.WriteLine($"Fatal: {ex}");
                }
            };

            window.WaitForClose();
        }
        finally
        {
            try { webProcess.Kill(entireProcessTree: true); } catch { }
            webProcess.Dispose();
        }
    }

    private static string? FindViewerExe()
    {
        var isWindows = OperatingSystem.IsWindows();
        var exeName = isWindows ? "RuntimeViewer.exe" : "RuntimeViewer";
        var baseDir = AppContext.BaseDirectory;

        // 1. Adjacent to this executable
        var adjacent = Path.Combine(baseDir, exeName);
        if (File.Exists(adjacent)) return adjacent;

        // 2. Sibling project (development layout)
        var dir = new DirectoryInfo(baseDir);
        for (int i = 0; i < 6; i++)
        {
            if (dir == null) break;
            var candidate = Path.Combine(dir.FullName, "RuntimeViewer", "bin", "Debug", "net10.0", exeName);
            if (File.Exists(candidate)) return candidate;
            candidate = Path.Combine(dir.FullName, "RuntimeViewer", "bin", "Release", "net10.0", exeName);
            if (File.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }

        return null;
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static void WaitForServer(string url, TimeSpan timeout)
    {
        var uri = new Uri(url);
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                using var tcp = new TcpClient();
                tcp.Connect(uri.Host, uri.Port);
                return;
            }
            catch
            {
                Thread.Sleep(200);
            }
        }
    }
}
