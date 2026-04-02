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
        // This handles cases where the project dir is a subfolder of the repo.
        var d = dir;
        while (!string.IsNullOrEmpty(d))
        {
            if (Directory.Exists(Path.Combine(d, ".git")))
            {
                _workingDir = d;
                return;
            }
            var parent = Directory.GetParent(d)?.FullName;
            if (parent == d) break; // root
            d = parent;
        }
        // No .git found — use the original directory (init will create one here)
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
            // Stage the specific file. If it's a bare filename and the repo root
            // differs from the original project dir, resolve the relative path.
            await RunGitCommandAsync($"add \"{filePath}\"");

            // Also stage any other tracked-but-modified files so the commit
            // captures a complete snapshot (e.g. external script/screen files).
            await RunGitCommandAsync("add -u");

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

        // Always use --set-upstream to handle both first push and subsequent pushes.
        // Git silently succeeds if upstream is already set.
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

    /// <summary>Reads the current git config user.name (local, then global).</summary>
    public async Task<string> GetConfigValueAsync(string key)
    {
        if (!IsGitRepository()) return "";
        try
        {
            return (await RunGitCommandAsync($"config {key}")).Trim();
        }
        catch
        {
            return "";
        }
    }

    /// <summary>Reads the remote origin URL, or "" if not set.</summary>
    public async Task<string> GetRemoteUrlAsync()
    {
        if (!IsGitRepository()) return "";
        try
        {
            return (await RunGitCommandAsync("remote get-url origin")).Trim();
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// Returns the commit log for a specific file (or the whole repo if filePath is null).
    /// Each entry: "hash|ISO-date|author|subject"
    /// </summary>
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
        catch { /* no commits yet or other git error */ }

        return result;
    }

    /// <summary>
    /// Returns the contents of a file at a specific commit hash.
    /// Returns null if the file doesn't exist at that commit.
    /// </summary>
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

public class GitCommitInfo
{
    public string Hash { get; set; } = "";
    public DateTimeOffset Date { get; set; }
    public string Author { get; set; } = "";
    public string Subject { get; set; } = "";

    public string ShortHash => Hash.Length >= 7 ? Hash[..7] : Hash;
    public string DisplayText => $"{ShortHash} — {Date.LocalDateTime:g} — {Subject}";
}
