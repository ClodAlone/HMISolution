using Xunit;
using ServerEditorWeb.Services;
using SharedModels;

namespace Tests.Editor;

/// <summary>
/// Tests for BackupService: model defaults, FormatSize utility, and snapshot
/// operations using a temporary directory.
/// </summary>
public class BackupServiceTests
{
    /// <summary>
    /// Creates a NodeEditorService with a project pointing to the given file path.
    /// Uses reflection to set the active project since OpenFile requires license validation.
    /// </summary>
    private static NodeEditorService CreateEditorWithFilePath(string filePath)
    {
        var editor = new NodeEditorService();
        var model = new NodeModel
        {
            Folder = new Folder { Name = "Root" },
            Server = new ServerSettings()
        };
        var projType = typeof(ServerEditorWeb.Models.ProjectNode);
        var proj = new ServerEditorWeb.Models.ProjectNode(model, filePath);

        // Use the internal AddProjectNode via reflection
        var method = typeof(NodeEditorService).GetMethod("AddProjectNode",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(editor, [proj]);

        return editor;
    }

    // ──────────────────────────────────────────────────────────────
    //  BackupSnapshot model defaults
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void BackupSnapshot_DefaultValues()
    {
        var snap = new BackupSnapshot();
        Assert.Equal("", snap.FileName);
        Assert.Equal("", snap.FilePath);
        Assert.Equal("", snap.Label);
        Assert.Equal("", snap.Trigger);
        Assert.Equal(default, snap.CreatedUtc);
        Assert.Equal(0, snap.SizeBytes);
    }

    [Fact]
    public void BackupSnapshot_SetProperties()
    {
        var now = DateTime.UtcNow;
        var snap = new BackupSnapshot
        {
            FileName = "project_20250101_120000.zip",
            FilePath = "/backups/project_20250101_120000.zip",
            CreatedUtc = now,
            SizeBytes = 12345,
            Label = "Before upgrade",
            Trigger = "manual"
        };

        Assert.Equal("project_20250101_120000.zip", snap.FileName);
        Assert.Equal("/backups/project_20250101_120000.zip", snap.FilePath);
        Assert.Equal(now, snap.CreatedUtc);
        Assert.Equal(12345, snap.SizeBytes);
        Assert.Equal("Before upgrade", snap.Label);
        Assert.Equal("manual", snap.Trigger);
    }

    // ──────────────────────────────────────────────────────────────
    //  FormatSize utility
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0, "0 B")]
    [InlineData(512, "512 B")]
    [InlineData(1023, "1023 B")]
    public void FormatSize_Bytes(long bytes, string expected)
    {
        Assert.Equal(expected, BackupService.FormatSize(bytes));
    }

    [Theory]
    [InlineData(1024, "1.0 KB")]
    [InlineData(1536, "1.5 KB")]
    [InlineData(10240, "10.0 KB")]
    public void FormatSize_Kilobytes(long bytes, string expected)
    {
        Assert.Equal(expected, BackupService.FormatSize(bytes));
    }

    [Theory]
    [InlineData(1048576, "1.0 MB")]
    [InlineData(5242880, "5.0 MB")]
    public void FormatSize_Megabytes(long bytes, string expected)
    {
        Assert.Equal(expected, BackupService.FormatSize(bytes));
    }

    [Fact]
    public void FormatSize_Gigabytes()
    {
        long oneGb = 1024L * 1024 * 1024;
        var result = BackupService.FormatSize(oneGb);
        Assert.Equal("1.00 GB", result);
    }

    // ──────────────────────────────────────────────────────────────
    //  BackupService without an open project
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void GetBackupDirectory_NoProject_ReturnsNull()
    {
        var editor = new NodeEditorService();
        var svc = new BackupService(editor);
        Assert.Null(svc.GetBackupDirectory());
    }

    [Fact]
    public void ListSnapshots_NoProject_ReturnsEmpty()
    {
        var editor = new NodeEditorService();
        var svc = new BackupService(editor);
        Assert.Empty(svc.ListSnapshots());
    }

    [Fact]
    public void CreateSnapshot_NoProject_ReturnsFailure()
    {
        var editor = new NodeEditorService();
        var svc = new BackupService(editor);
        var (success, message) = svc.CreateSnapshot();
        Assert.False(success);
        Assert.Contains("No project", message);
    }

    [Fact]
    public void RestoreSnapshot_NoProject_ReturnsFailure()
    {
        var editor = new NodeEditorService();
        var svc = new BackupService(editor);
        var (success, message) = svc.RestoreSnapshot("nonexistent.zip");
        Assert.False(success);
        Assert.Contains("No project", message);
    }

    [Fact]
    public void GetBackupStats_NoProject_ReturnsZero()
    {
        var editor = new NodeEditorService();
        var svc = new BackupService(editor);
        var (count, totalBytes) = svc.GetBackupStats();
        Assert.Equal(0, count);
        Assert.Equal(0, totalBytes);
    }

    [Fact]
    public void DeleteSnapshot_NonexistentFile_ReturnsFalse()
    {
        var editor = new NodeEditorService();
        var svc = new BackupService(editor);
        Assert.False(svc.DeleteSnapshot("nonexistent_path.zip"));
    }

    // ──────────────────────────────────────────────────────────────
    //  Integration: create, list, delete snapshot with temp project
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void CreateAndListSnapshot_WithTempProject()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"backup_test_{Guid.NewGuid():N}");
        try
        {
            Directory.CreateDirectory(tempDir);
            var projectFile = Path.Combine(tempDir, "test_project.json");
            File.WriteAllText(projectFile, "{}");

            var editor = CreateEditorWithFilePath(projectFile);
            var svc = new BackupService(editor);

            // Create a snapshot
            var (success, message) = svc.CreateSnapshot("Test snapshot", "manual");
            Assert.True(success, $"Snapshot creation failed: {message}");
            Assert.Contains("Snapshot created", message);

            // List should contain the snapshot
            var snapshots = svc.ListSnapshots();
            Assert.Single(snapshots);
            Assert.Equal("Test snapshot", snapshots[0].Label);
            Assert.Equal("manual", snapshots[0].Trigger);
            Assert.True(snapshots[0].SizeBytes > 0);

            // Stats should match
            var (count, totalBytes) = svc.GetBackupStats();
            Assert.Equal(1, count);
            Assert.True(totalBytes > 0);

            // Delete the snapshot
            Assert.True(svc.DeleteSnapshot(snapshots[0].FilePath));
            Assert.Empty(svc.ListSnapshots());
        }
        finally
        {
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }

    [Fact]
    public void PreviewSnapshot_ShowsContents()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"backup_preview_{Guid.NewGuid():N}");
        try
        {
            Directory.CreateDirectory(tempDir);
            var projectFile = Path.Combine(tempDir, "preview_project.json");
            File.WriteAllText(projectFile, """{"Server":{}}""");

            var editor = CreateEditorWithFilePath(projectFile);
            var svc = new BackupService(editor);
            var (success, _) = svc.CreateSnapshot("Preview test", "manual");
            Assert.True(success);

            var snapshots = svc.ListSnapshots();
            Assert.Single(snapshots);

            var entries = svc.PreviewSnapshot(snapshots[0].FilePath);
            Assert.NotEmpty(entries);
            Assert.Contains(entries, e => e.name == "preview_project.json");
        }
        finally
        {
            try { Directory.Delete(tempDir, true); } catch { }
        }
    }
}
