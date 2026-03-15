using System.Diagnostics;

namespace ServerEditorWeb.Services;

public class GitService
{
    private string _workingDir = "";

    public void SetWorkingDirectory(string dir) => _workingDir = dir;

    public bool IsGitRepository()
    {
        return !string.IsNullOrEmpty(_workingDir) && Directory.Exists(Path.Combine(_workingDir, ".git"));
    }

    public async Task<string> CommitChangesAsync(string filePath, string message)
    {
        if (!IsGitRepository()) return "Not a git repository.";

        try
        {
            await RunGitCommandAsync($"add \"{filePath}\"");
            return await RunGitCommandAsync($"commit -m \"{message}\"");
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

        try
        {
            return await RunGitCommandAsync($"remote add origin \"{url}\"");
        }
        catch
        {
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
