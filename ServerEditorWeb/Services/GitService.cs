using System.Diagnostics;

namespace ServerEditorWeb.Services;

public class GitService
{
    private string _workingDir = "";

    /// <summary>The resolved git repository root directory.</summary>
    public string WorkingDirectory => _workingDir;

    public void SetWorkingDirectory(string dir)
    {
        // Walk up the directory tree to find the actual git repository root.
        var d = dir;
        while (!string.IsNullOrEmpty(d))
        {
            if (Directory.Exists(Path.Combine(d, ".git")))
            {
                _workingDir = d;
                return;
            }
            var parent = Directory.GetParent(d)?.FullName;
            if (parent == d) break;
            d = parent;
        }
        _workingDir = dir;
    }

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
            await RunGitCommandAsync("add -u");

            var status = await RunGitCommandAsync("diff --cached --stat");
            if (string.IsNullOrWhiteSpace(status))
                return "No changes to commit — files are up to date.";

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

        var tracking = await RunGitCommandAsync("rev-parse --abbrev-ref @{upstream}");
        if (!string.IsNullOrWhiteSpace(tracking) && !tracking.StartsWith("Git execution failed"))
        {
            return await RunGitCommandAsync("push");
        }

        var branch = (await RunGitCommandAsync("rev-parse --abbrev-ref HEAD")).Trim();
        if (string.IsNullOrEmpty(branch) || branch.StartsWith("Git execution failed"))
            branch = "master";

        return await RunGitCommandAsync($"push --set-upstream origin {branch}");
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

        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("git@", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("ssh://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("git://", StringComparison.OrdinalIgnoreCase))
        {
            return "Error: Invalid remote URL. Please use a full URL (e.g. https://github.com/user/repo.git).";
        }

        var result = await RunGitCommandAsync($"remote set-url origin \"{url}\"");
        if (result.StartsWith("Git execution failed", StringComparison.OrdinalIgnoreCase))
        {
            result = await RunGitCommandAsync($"remote add origin \"{url}\"");
        }
        return string.IsNullOrWhiteSpace(result) ? "Remote origin set successfully." : result;
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

    public async Task<string> GetConfigValueAsync(string key)
    {
        if (!IsGitRepository()) return "";
        try
        {
            return (await RunGitCommandAsync($"config {key}")).Trim();
        }
        catch { return ""; }
    }

    public async Task<string> GetRemoteUrlAsync()
    {
        if (!IsGitRepository()) return "";
        try
        {
            return (await RunGitCommandAsync("remote get-url origin")).Trim();
        }
        catch { return ""; }
    }

    public async Task<string> GetCurrentBranchAsync()
    {
        if (!IsGitRepository()) return "";
        try
        {
            return (await RunGitCommandAsync("rev-parse --abbrev-ref HEAD")).Trim();
        }
        catch { return ""; }
    }

    public async Task<List<GitCommitInfo>> GetLogAsync(string? filePath = null, int maxCount = 50)
    {
        var result = new List<GitCommitInfo>();
        if (!IsGitRepository()) return result;

        try
        {
            var fileArg = !string.IsNullOrEmpty(filePath) ? $" -- \"{filePath}\"" : "";
            var raw = await RunGitCommandAsync(
                $"log --format=\"%H|%aI|%an|%s\" -n {maxCount}{fileArg}");

            foreach (var line in raw.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split('|', 4);
                if (parts.Length < 4) continue;
                result.Add(new GitCommitInfo
                {
                    Hash = parts[0].Trim(),
                    Date = DateTimeOffset.TryParse(parts[1].Trim(), out var d) ? d : DateTimeOffset.MinValue,
                    Author = parts[2].Trim(),
                    Subject = parts[3].Trim()
                });
            }
        }
        catch { }

        return result;
    }

    public async Task<string?> ShowFileAtCommitAsync(string commitHash, string filePath)
    {
        if (!IsGitRepository()) return null;
        try
        {
            return await RunGitCommandAsync($"show {commitHash}:\"{filePath}\"");
        }
        catch
        {
            return null;
        }
    }

    private async Task<string> RunGitCommandAsync(string arguments)
    {
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
            }
        };

        try
        {
            process.Start();

            // Read stdout and stderr asynchronously BEFORE WaitForExitAsync.
            // Reading inside the Exited event causes deadlocks when output is large
            // because the OS pipe buffer fills up and blocks the process from exiting.
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            var output = await outputTask;
            var error = await errorTask;

            if (process.ExitCode == 0)
            {
                return !string.IsNullOrWhiteSpace(output) ? output : (error ?? "");
            }
            else
            {
                var msg = !string.IsNullOrWhiteSpace(error) ? error : output;
                return $"Git execution failed: {msg}";
            }
        }
        catch (Exception ex)
        {
            return $"Git execution failed: {ex.Message}";
        }
        finally
        {
            process.Dispose();
        }
    }
}

public class GitCommitInfo
{
    public string Hash { get; set; } = "";
    public DateTimeOffset Date { get; set; }
    public string Author { get; set; } = "";
    public string Subject { get; set; } = "";

    public string ShortHash => Hash.Length >= 7 ? Hash[..7] : Hash;
    public string DisplayText => $"{ShortHash} — {Date.LocalDateTime:g} — {Subject}";
}
