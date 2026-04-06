using System.Text.Json;
using SharedModels;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Loads a nodes.json configuration file and provides the screen list.
/// The JSON file path is passed via command-line args or defaults to "nodes.json".
/// </summary>
public class ProjectService
{
    private NodeModel? _model;
    private string _configPath = "";

    public NodeModel? Model => _model;
    public string ConfigPath => _configPath;
    public List<ScreenConfig> Screens => _model?.Screens ?? [];
    public ServerSettings Settings => _model?.Server ?? new();

    /// <summary>When true, the viewer runs in kiosk mode (no chrome, auto-scale).</summary>
    public bool IsKiosk { get; set; }

    public ScreenConfig? StartupScreen =>
        !string.IsNullOrEmpty(Settings.StartupScreen)
            ? Screens.FirstOrDefault(s => s.Name.Equals(Settings.StartupScreen, StringComparison.OrdinalIgnoreCase))
              ?? Screens.FirstOrDefault()
            : Screens.FirstOrDefault();

    public (bool Success, string Message) Load(string path)
    {
        try
        {
            if (!File.Exists(path))
                return (false, $"File not found: {path}");

            using var stream = File.OpenRead(path);
            _model = JsonSerializer.Deserialize<NodeModel>(stream);
            _configPath = path;

            if (_model == null)
                return (false, "Failed to deserialize JSON.");

            // Load external resource files (scripts/, screens/, plcprograms/)
            ResourceFileManager.LoadExternalResources(_model, path);

            // Configure crash email from project settings
            CrashReporter.ConfigureEmail(_model.Server?.CrashEmail);

            // Validate license
            var licFile = LicenseManager.FindLicenseFile(path);
            LicenseManager.Validate(licFile);

            return (true, $"Loaded {path} — {Screens.Count} screen(s)");
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }
}

