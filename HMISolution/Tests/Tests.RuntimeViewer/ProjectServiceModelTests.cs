using Xunit;
using RuntimeViewer.Shared.Services;
using SharedModels;

namespace Tests.RuntimeViewer;

public class ProjectServiceModelTests
{
    /// <summary>
    /// Creates a ProjectService with the model pre-set via reflection
    /// (same pattern used by LocalizationServiceTests).
    /// </summary>
    private static ProjectService CreateProjectWithModel(NodeModel model)
    {
        var project = new ProjectService();
        var field = typeof(ProjectService).GetField("_model",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(project, model);
        return project;
    }

    // ──────────────────────────────────────────────────────────────
    // Screens property
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void Screens_NoModel_ReturnsEmptyList()
    {
        var project = new ProjectService();
        Assert.Empty(project.Screens);
    }

    [Fact]
    public void Screens_WithModel_ReturnsList()
    {
        var model = new NodeModel
        {
            Screens = new()
            {
                new ScreenConfig { Name = "Main" },
                new ScreenConfig { Name = "Settings" }
            }
        };
        var project = CreateProjectWithModel(model);

        Assert.Equal(2, project.Screens.Count);
        Assert.Equal("Main", project.Screens[0].Name);
    }

    // ──────────────────────────────────────────────────────────────
    // Settings property
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void Settings_NoModel_ReturnsDefaultSettings()
    {
        var project = new ProjectService();
        Assert.NotNull(project.Settings);
        Assert.Equal("opc.tcp://localhost:14840/SimpleOpcFileServer", project.Settings.EndpointUrl);
    }

    [Fact]
    public void Settings_WithModel_ReturnsModelSettings()
    {
        var model = new NodeModel
        {
            Server = new ServerSettings
            {
                EndpointUrl = "opc.tcp://custom:5000/MyServer",
                DiagnosticsPort = 9999
            }
        };
        var project = CreateProjectWithModel(model);

        Assert.Equal("opc.tcp://custom:5000/MyServer", project.Settings.EndpointUrl);
        Assert.Equal(9999, project.Settings.DiagnosticsPort);
    }

    // ──────────────────────────────────────────────────────────────
    // StartupScreen property
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void StartupScreen_NoScreens_ReturnsNull()
    {
        var model = new NodeModel { Server = new ServerSettings() };
        var project = CreateProjectWithModel(model);

        Assert.Null(project.StartupScreen);
    }

    [Fact]
    public void StartupScreen_NoStartupConfigured_ReturnsFirst()
    {
        var model = new NodeModel
        {
            Server = new ServerSettings(),
            Screens = new()
            {
                new ScreenConfig { Name = "Dashboard" },
                new ScreenConfig { Name = "Settings" }
            }
        };
        var project = CreateProjectWithModel(model);

        Assert.NotNull(project.StartupScreen);
        Assert.Equal("Dashboard", project.StartupScreen.Name);
    }

    [Fact]
    public void StartupScreen_ConfiguredByName_ReturnsCorrectScreen()
    {
        var model = new NodeModel
        {
            Server = new ServerSettings { StartupScreen = "Settings" },
            Screens = new()
            {
                new ScreenConfig { Name = "Dashboard" },
                new ScreenConfig { Name = "Settings" }
            }
        };
        var project = CreateProjectWithModel(model);

        Assert.NotNull(project.StartupScreen);
        Assert.Equal("Settings", project.StartupScreen.Name);
    }

    [Fact]
    public void StartupScreen_ConfiguredButNotFound_FallsBackToFirst()
    {
        var model = new NodeModel
        {
            Server = new ServerSettings { StartupScreen = "NonExistent" },
            Screens = new()
            {
                new ScreenConfig { Name = "Main" }
            }
        };
        var project = CreateProjectWithModel(model);

        Assert.NotNull(project.StartupScreen);
        Assert.Equal("Main", project.StartupScreen.Name);
    }

    [Fact]
    public void StartupScreen_CaseInsensitive()
    {
        var model = new NodeModel
        {
            Server = new ServerSettings { StartupScreen = "SETTINGS" },
            Screens = new()
            {
                new ScreenConfig { Name = "Dashboard" },
                new ScreenConfig { Name = "Settings" }
            }
        };
        var project = CreateProjectWithModel(model);

        Assert.NotNull(project.StartupScreen);
        Assert.Equal("Settings", project.StartupScreen.Name);
    }

    // ──────────────────────────────────────────────────────────────
    // IsKiosk property
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsKiosk_DefaultFalse()
    {
        var project = new ProjectService();
        Assert.False(project.IsKiosk);
    }

    [Fact]
    public void IsKiosk_CanBeSet()
    {
        var project = new ProjectService();
        project.IsKiosk = true;
        Assert.True(project.IsKiosk);
    }

    // ──────────────────────────────────────────────────────────────
    // Model property
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void Model_InitiallyNull()
    {
        var project = new ProjectService();
        Assert.Null(project.Model);
    }

    [Fact]
    public void Model_AfterSet_NotNull()
    {
        var model = new NodeModel { Folder = new Folder { Name = "Root" } };
        var project = CreateProjectWithModel(model);

        Assert.NotNull(project.Model);
        Assert.Equal("Root", project.Model!.Folder.Name);
    }

    // ──────────────────────────────────────────────────────────────
    // Load — file not found
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void Load_FileNotFound_ReturnsFalse()
    {
        var project = new ProjectService();
        var (success, message) = project.Load("nonexistent_file_path.json");

        Assert.False(success);
        Assert.Contains("not found", message, StringComparison.OrdinalIgnoreCase);
    }

    // ──────────────────────────────────────────────────────────────
    // ConfigPath property
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void ConfigPath_InitiallyEmpty()
    {
        var project = new ProjectService();
        Assert.Equal("", project.ConfigPath);
    }
}
