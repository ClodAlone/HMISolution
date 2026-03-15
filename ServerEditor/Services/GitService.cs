using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ServerEditor.Services
{
    public class GitService
    {
        private readonly string _workingDir;

        public GitService(string workingDir)
        {
            _workingDir = workingDir;
        }

        public bool IsGitRepository()
        {
            return Directory.Exists(Path.Combine(_workingDir, ".git"));
        }

        public async Task<string> CommitChangesAsync(string filePath, string message)
        {
            if (!IsGitRepository()) return "Not a git repository.";

            try
            {
                // git add
                await RunGitCommandAsync($"add \"{filePath}\"");

                // git commit
                var result = await RunGitCommandAsync($"commit -m \"{message}\"");
                
                return result;
            }
            catch (Exception ex)
            {
                return $"Error committing to git: {ex.Message}";
            }
        }

        public async Task<string> PushAsync()
        {
             if (!IsGitRepository()) return "Not a git repository.";
             return await RunGitCommandAsync("push");
        }

        public async Task<string> InitRepoAsync()
        {
            return await RunGitCommandAsync("init");
        }

        public async Task<string> SetUserIdentityAsync(string name, string email)
        {
            try
            {
                await RunGitCommandAsync($"config user.name \"{name}\"");
                return await RunGitCommandAsync($"config user.email \"{email}\"");
            }
            catch (Exception ex)
            {
                return $"Error setting user identity: {ex.Message}";
            }
        }

        public async Task<string> SetRemoteOriginAsync(string url)
        {
            if (!IsGitRepository()) return "Not a git repository.";
            
            // Try to add, if fails (already exists), try set-url
            try
            {
                return await RunGitCommandAsync($"remote add origin \"{url}\"");
            }
            catch
            {
                // Likely already exists, try set-url
                try 
                {
                    return await RunGitCommandAsync($"remote set-url origin \"{url}\"");
                }
                catch (Exception ex) 
                {
                    return $"Error setting remote: {ex.Message}";
                }
            }
        }

        public async Task<string> PullAsync()
        {
             if (!IsGitRepository()) return "Not a git repository.";
             return await RunGitCommandAsync("pull");
        }

        public async Task<string> GetStatusAsync()
        {
             if (!IsGitRepository()) return "Not a git repository.";
             return await RunGitCommandAsync("status");
        }

        private async Task<string> RunGitCommandAsync(string arguments)
        {
            var tcs = new TaskCompletionSource<string>();

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = arguments,
                    WorkingDirectory = _workingDir,
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
                if (process.ExitCode == 0)
                {
                    tcs.SetResult(output);
                }
                else
                {
                    // Git returns exit code 1 if nothing to commit, which is fine sometimes, but generally we want to know
                    if (string.IsNullOrWhiteSpace(error)) tcs.SetResult(output);
                    else tcs.SetException(new Exception(error));
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
                return $"Git execution failed: {ex.Message}";
            }
        }
    }
}
