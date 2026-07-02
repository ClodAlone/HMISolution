// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.IO.Compression;
using System.Text.Json;

namespace ServerEditorWeb.Services;

/// <summary>
/// Describes a single project backup snapshot.
/// </summary>
public class BackupSnapshot
{
    public string FileName { get; set; } = "";
    public string FilePath { get; set; } = "";
    public DateTime CreatedUtc { get; set; }
    public long SizeBytes { get; set; }
    public string Label { get; set; } = "";
    public string Trigger { get; set; } = ""; // "manual", "auto-save", "scheduled"
}

/// <summary>
/// Manages automated project snapshots (ZIP archives) beyond Git.
/// Snapshots are stored in a <c>backups/</c> folder next to the project file and contain
/// the nodes.json configuration plus all resource subdirectories (scripts/, screens/, etc.).
/// </summary>
public class BackupService
{
    private const string BackupsFolder = "backups";
    private const string MetadataFile = "backup_meta.json";
    private readonly NodeEditorService _editor;
    private System.Threading.Timer? _scheduledTimer;

    public BackupService(NodeEditorService editor)
    {
        _editor = editor;
    }

    /// <summary>
    /// Returns the backups directory for the currently active project.
    /// </summary>
    public string? GetBackupDirectory()
    {
        var projectPath = _editor.CurrentFilePath;
        if (string.IsNullOrEmpty(projectPath)) return null;
        var dir = Path.GetDirectoryName(Path.GetFullPath(projectPath));
        if (string.IsNullOrEmpty(dir)) return null;
        return Path.Combine(dir, BackupsFolder);
    }

    /// <summary>
    /// Lists all backup snapshots for the current project, newest first.
    /// </summary>
    public List<BackupSnapshot> ListSnapshots()
    {
        var backupDir = GetBackupDirectory();
        if (backupDir == null || !Directory.Exists(backupDir))
            return new List<BackupSnapshot>();

        var results = new List<BackupSnapshot>();
        var metaPath = Path.Combine(backupDir, MetadataFile);
        var metaDict = LoadMetadata(metaPath);

        foreach (var file in Directory.EnumerateFiles(backupDir, "*.zip"))
        {
            var fi = new FileInfo(file);
            var snapshot = new BackupSnapshot
            {
                FileName = fi.Name,
                FilePath = fi.FullName,
                CreatedUtc = fi.CreationTimeUtc,
                SizeBytes = fi.Length
            };

            if (metaDict.TryGetValue(fi.Name, out var meta))
            {
                snapshot.Label = meta.Label;
                snapshot.Trigger = meta.Trigger;
                if (meta.CreatedUtc != default)
                    snapshot.CreatedUtc = meta.CreatedUtc;
            }

            results.Add(snapshot);
        }

        return results.OrderByDescending(s => s.CreatedUtc).ToList();
    }

    /// <summary>
    /// Creates a snapshot of the current project.
    /// </summary>
    public (bool success, string message) CreateSnapshot(string label = "", string trigger = "manual")
    {
        var projectPath = _editor.CurrentFilePath;
        if (string.IsNullOrEmpty(projectPath))
            return (false, "No project is open.");

        var fullPath = Path.GetFullPath(projectPath);
        var projectDir = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(projectDir))
            return (false, "Cannot determine project directory.");

        var backupDir = Path.Combine(projectDir, BackupsFolder);
        Directory.CreateDirectory(backupDir);

        try
        {
            var timestamp = DateTime.UtcNow;
            var safeName = Path.GetFileNameWithoutExtension(fullPath);
            var zipName = $"{safeName}_{timestamp:yyyyMMdd_HHmmss}.zip";
            var zipPath = Path.Combine(backupDir, zipName);

            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                // Add the main config file
                if (File.Exists(fullPath))
                    zip.CreateEntryFromFile(fullPath, Path.GetFileName(fullPath));

                // Add resource subdirectories
                var resourceDirs = new[] { "scripts", "screens", "plcprograms", "recipes", "images" };
                foreach (var subDir in resourceDirs)
                {
                    var subPath = Path.Combine(projectDir, subDir);
                    if (Directory.Exists(subPath))
                    {
                        foreach (var file in Directory.EnumerateFiles(subPath, "*", SearchOption.AllDirectories))
                        {
                            var relativePath = Path.GetRelativePath(projectDir, file);
                            zip.CreateEntryFromFile(file, relativePath);
                        }
                    }
                }

                // Add data logging DB if it exists (small projects)
                var dbFile = Path.Combine(projectDir, "datalog.db");
                if (File.Exists(dbFile))
                {
                    var fi = new FileInfo(dbFile);
                    if (fi.Length < 100 * 1024 * 1024) // Only if < 100 MB
                        zip.CreateEntryFromFile(dbFile, "datalog.db");
                }
            }

            // Update metadata
            SaveSnapshotMeta(backupDir, zipName, label, trigger, timestamp);

            // Enforce retention
            EnforceRetention(backupDir);

            var size = new FileInfo(zipPath).Length;
            return (true, $"Snapshot created: {zipName} ({FormatSize(size)})");
        }
        catch (Exception ex)
        {
            return (false, $"Snapshot failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Restores a snapshot by extracting the ZIP over the project directory.
    /// The current project is backed up first as a safety net.
    /// </summary>
    public (bool success, string message) RestoreSnapshot(string zipPath)
    {
        var projectPath = _editor.CurrentFilePath;
        if (string.IsNullOrEmpty(projectPath))
            return (false, "No project is open.");

        var fullPath = Path.GetFullPath(projectPath);
        var projectDir = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(projectDir) || !File.Exists(zipPath))
            return (false, "Invalid paths.");

        try
        {
            // Create a safety backup before restoring
            CreateSnapshot("Pre-restore safety backup", "auto-restore");

            // Extract the archive
            using var zip = ZipFile.OpenRead(zipPath);
            foreach (var entry in zip.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name)) continue; // skip directories
                var destPath = Path.Combine(projectDir, entry.FullName);
                var destDir = Path.GetDirectoryName(destPath);
                if (!string.IsNullOrEmpty(destDir))
                    Directory.CreateDirectory(destDir);
                entry.ExtractToFile(destPath, overwrite: true);
            }

            return (true, $"Restored from {Path.GetFileName(zipPath)}. Reload the project to see changes.");
        }
        catch (Exception ex)
        {
            return (false, $"Restore failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a specific backup snapshot.
    /// </summary>
    public bool DeleteSnapshot(string zipPath)
    {
        try
        {
            if (File.Exists(zipPath))
            {
                var backupDir = Path.GetDirectoryName(zipPath);
                var fileName = Path.GetFileName(zipPath);
                File.Delete(zipPath);

                // Remove from metadata
                if (!string.IsNullOrEmpty(backupDir))
                    RemoveSnapshotMeta(backupDir, fileName);

                return true;
            }
            return false;
        }
        catch { return false; }
    }

    /// <summary>
    /// Updates the label on an existing snapshot.
    /// </summary>
    public void UpdateLabel(string zipPath, string newLabel)
    {
        var backupDir = Path.GetDirectoryName(zipPath);
        var fileName = Path.GetFileName(zipPath);
        if (string.IsNullOrEmpty(backupDir)) return;

        var metaPath = Path.Combine(backupDir, MetadataFile);
        var dict = LoadMetadata(metaPath);
        if (dict.TryGetValue(fileName, out var meta))
        {
            meta.Label = newLabel;
            SaveMetadata(metaPath, dict);
        }
    }

    /// <summary>
    /// Called when the user saves the project. Creates an auto-save snapshot
    /// if the backup configuration is enabled.
    /// </summary>
    public void OnProjectSaved()
    {
        var config = _editor.RootModel?.Server?.Backup;
        if (config is not { Enabled: true, SnapshotOnSave: true })
            return;

        CreateSnapshot("Auto-save snapshot", "auto-save");
    }

    /// <summary>
    /// Starts or restarts the scheduled backup timer based on configuration.
    /// </summary>
    public void ConfigureScheduledBackup()
    {
        _scheduledTimer?.Dispose();
        _scheduledTimer = null;

        var config = _editor.RootModel?.Server?.Backup;
        if (config is not { Enabled: true } || config.ScheduleMinutes <= 0)
            return;

        var interval = TimeSpan.FromMinutes(config.ScheduleMinutes);
        _scheduledTimer = new System.Threading.Timer(_ =>
        {
            try
            {
                if (!string.IsNullOrEmpty(_editor.CurrentFilePath))
                    CreateSnapshot("Scheduled snapshot", "scheduled");
            }
            catch { }
        }, null, interval, interval);
    }

    /// <summary>
    /// Gets the total size of all backups.
    /// </summary>
    public (int count, long totalBytes) GetBackupStats()
    {
        var backupDir = GetBackupDirectory();
        if (backupDir == null || !Directory.Exists(backupDir))
            return (0, 0);

        var files = Directory.GetFiles(backupDir, "*.zip");
        long total = 0;
        foreach (var f in files)
        {
            try { total += new FileInfo(f).Length; } catch { }
        }
        return (files.Length, total);
    }

    /// <summary>
    /// Lists the contents of a backup ZIP for preview.
    /// </summary>
    public List<(string name, long size)> PreviewSnapshot(string zipPath)
    {
        var entries = new List<(string name, long size)>();
        try
        {
            using var zip = ZipFile.OpenRead(zipPath);
            foreach (var entry in zip.Entries)
            {
                if (!string.IsNullOrEmpty(entry.Name))
                    entries.Add((entry.FullName, entry.Length));
            }
        }
        catch { }
        return entries;
    }

    private void EnforceRetention(string backupDir)
    {
        var config = _editor.RootModel?.Server?.Backup;
        var maxSnapshots = config?.MaxSnapshots ?? 20;
        if (maxSnapshots <= 0) maxSnapshots = 20;

        var files = Directory.GetFiles(backupDir, "*.zip")
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.CreationTimeUtc)
            .ToList();

        while (files.Count > maxSnapshots)
        {
            var oldest = files[^1];
            try
            {
                oldest.Delete();
                RemoveSnapshotMeta(backupDir, oldest.Name);
            }
            catch { }
            files.RemoveAt(files.Count - 1);
        }
    }

    private void SaveSnapshotMeta(string backupDir, string fileName, string label, string trigger, DateTime createdUtc)
    {
        var metaPath = Path.Combine(backupDir, MetadataFile);
        var dict = LoadMetadata(metaPath);
        dict[fileName] = new SnapshotMeta { Label = label, Trigger = trigger, CreatedUtc = createdUtc };
        SaveMetadata(metaPath, dict);
    }

    private void RemoveSnapshotMeta(string backupDir, string fileName)
    {
        var metaPath = Path.Combine(backupDir, MetadataFile);
        var dict = LoadMetadata(metaPath);
        if (dict.Remove(fileName))
            SaveMetadata(metaPath, dict);
    }

    private static Dictionary<string, SnapshotMeta> LoadMetadata(string metaPath)
    {
        try
        {
            if (File.Exists(metaPath))
            {
                var json = File.ReadAllText(metaPath);
                return JsonSerializer.Deserialize<Dictionary<string, SnapshotMeta>>(json) ?? new();
            }
        }
        catch { }
        return new();
    }

    private static void SaveMetadata(string metaPath, Dictionary<string, SnapshotMeta> dict)
    {
        try
        {
            var json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(metaPath, json);
        }
        catch { }
    }

    public static string FormatSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
        return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
    }

    private class SnapshotMeta
    {
        public string Label { get; set; } = "";
        public string Trigger { get; set; } = "";
        public DateTime CreatedUtc { get; set; }
    }
}
