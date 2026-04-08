using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Photino.NET;

namespace ServerEditorWeb.Desktop;

static class Program
{
    private static PhotinoWindow? _window;

    [STAThread]
    static void Main(string[] args)
    {
        // Install crash reporter
        var crashDir = Path.Combine(AppContext.BaseDirectory, "crash_reports");
        SharedModels.CrashReporter.Install(crashDir, "ServerEditorWeb.Desktop");

        // Find the ServerEditorWeb executable
        var editorExe = FindEditorExe();
        if (editorExe == null)
        {
            Console.Error.WriteLine("ServerEditorWeb executable not found. Build the ServerEditorWeb project first.");
            return;
        }

        // Pick a free port
        int port = GetFreePort();
        var url = $"http://127.0.0.1:{port}";

        // Start the ServerEditorWeb process
        var webProcess = new Process();
        webProcess.StartInfo.FileName = editorExe;
        webProcess.StartInfo.Arguments = string.Join(" ", args.Select(a => $"\"{a}\""));
        webProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(editorExe)!;
        webProcess.StartInfo.UseShellExecute = false;
        webProcess.StartInfo.CreateNoWindow = true;
        webProcess.StartInfo.EnvironmentVariables["ASPNETCORE_URLS"] = url;
        webProcess.StartInfo.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Development";
        webProcess.Start();

        try
        {
            // Wait for the web server to be ready
            WaitForServer(url, TimeSpan.FromSeconds(30));
            // Give ASP.NET Core a moment to finish initializing after accepting TCP
            Thread.Sleep(1500);

            // Open a Photino window pointing to the local web server
            _window = new PhotinoWindow()
                .SetTitle("HMI Editor")
                .SetUseOsDefaultSize(false)
                .SetSize(1440, 900)
                .SetMaximized(true)
#if DEBUG
                .SetDevToolsEnabled(true)
#endif
                .Load(new Uri(url));

            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    SharedModels.CrashReporter.Report(ex, "AppDomain");
                    Console.Error.WriteLine($"Fatal: {ex}");
                }
            };

            _window.WaitForClose();
        }
        finally
        {
            try { webProcess.Kill(entireProcessTree: true); } catch { }
            webProcess.Dispose();
        }
    }

    private static string? FindEditorExe()
    {
        var isWindows = OperatingSystem.IsWindows();
        var exeName = isWindows ? "ServerEditorWeb.exe" : "ServerEditorWeb";
        var baseDir = AppContext.BaseDirectory;

        // 1. Adjacent to this executable
        var adjacent = Path.Combine(baseDir, exeName);
        if (File.Exists(adjacent)) return adjacent;

        // 2. Sibling project (development layout)
        var dir = new DirectoryInfo(baseDir);
        for (int i = 0; i < 6; i++)
        {
            if (dir == null) break;
            var candidate = Path.Combine(dir.FullName, "ServerEditorWeb", "bin", "Debug", "net10.0", exeName);
            if (File.Exists(candidate)) return candidate;
            candidate = Path.Combine(dir.FullName, "ServerEditorWeb", "bin", "Release", "net10.0", exeName);
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
