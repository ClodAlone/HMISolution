using SharedModels;

namespace SimpleOpcFileServer;

/// <summary>
/// Loads external resource files (scripts, screens, PLC programs) from
/// subdirectories relative to the main configuration file.
/// </summary>
internal static class ResourceFileManager
{
    /// <summary>
    /// Scans the standard subdirectories (scripts/, screens/, plcprograms/) next to
    /// <paramref name="configPath"/> and merges their content into the <paramref name="model"/>.
    /// </summary>
    public static void LoadExternalResources(NodeModel model, string configPath)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(configPath));
        if (dir == null) return;

        // Load script files
        var scriptsDir = Path.Combine(dir, "scripts");
        if (Directory.Exists(scriptsDir) && model.Scripts != null)
        {
            foreach (var script in model.Scripts)
            {
                if (!string.IsNullOrEmpty(script.Name))
                {
                    var file = Path.Combine(scriptsDir, script.Name + ".cs");
                    if (File.Exists(file) && string.IsNullOrEmpty(script.Code))
                        script.Code = File.ReadAllText(file);
                }
            }
        }
    }
}
