using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ServerEditor.Services
{
    public class WindowsServiceManager
    {
        public string ServiceName { get; set; } = "SimpleOpcFileServer"; // Default

        public bool IsServiceInstalled()
        {
            try
            {
                using var sc = new ServiceController(ServiceName);
                // Accessing Status throws if service doesn't exist?
                // Actually InvalidOperationException if service not found on machine.
                _ = sc.Status;
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ServiceControllerStatus GetServiceStatus()
        {
            try
            {
                using var sc = new ServiceController(ServiceName);
                sc.Refresh();
                return sc.Status;
            }
            catch (InvalidOperationException)
            {
                return (ServiceControllerStatus)0; // Not installed
            }
            catch (Exception)
            {
                return (ServiceControllerStatus)0; 
            }
        }

        public async Task<string> InstallServiceAsync(string executablePath, string configPath)
        {
            if (!File.Exists(executablePath)) return "Executable not found.";

            // Ensure absolute paths
            executablePath = Path.GetFullPath(executablePath);
            configPath = Path.GetFullPath(configPath);
            
            // binPath for sc create needs quotes if paths contain spaces.
            // And arguments need to be part of the command line string.
            // binPath= "\"C:\Path\To\Server.exe\" \"C:\Path\To\nodes.json\""
            
            string binPath = $"\\\"{executablePath}\\\" \\\"{configPath}\\\"";
            
            string displayName = $"Simple OPC File Server - {Path.GetFileNameWithoutExtension(configPath)}";
            return await RunScCommandAsync($"create \"{ServiceName}\" binPath= \"{binPath}\" displayName= \"{displayName}\" start= auto");
        }

        public async Task<string> UninstallServiceAsync()
        {
            // Stop first?
            try
            {
                using var sc = new ServiceController(ServiceName);
                if (sc.Status != ServiceControllerStatus.Stopped)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
            }
            catch {}

            return await RunScCommandAsync($"delete \"{ServiceName}\"");
        }

        public async Task<string> StartServiceAsync()
        {
            return await RunScCommandAsync($"start \"{ServiceName}\"");
        }

        public async Task<string> StopServiceAsync()
        {
            return await RunScCommandAsync($"stop \"{ServiceName}\"");
        }

        private async Task<string> RunScCommandAsync(string arguments)
        {
            var tcs = new TaskCompletionSource<string>();

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sc",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas" // Only works with UseShellExecute=true. If false, just inherit permissions.
                },
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                if (process.ExitCode == 0)
                {
                    tcs.SetResult(output);
                }
                else
                {
                    tcs.SetResult($"Error: {error}\nOutput: {output}");
                }
                process.Dispose();
            };

            try
            {
                process.Start();
                return await tcs.Task;
            }
            catch (Exception ex)
            {
                return $"Error executing sc: {ex.Message}";
            }
        }
    }
}
