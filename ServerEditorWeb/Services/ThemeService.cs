using System.Text.Json;

namespace ServerEditorWeb.Services;

public class ThemeService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SimpleOpcFileServer", "theme.json");

    public string Theme { get; private set; } = "light";

    public event Action? ThemeChanged;

    public ThemeService()
    {
        Load();
    }

    public void SetTheme(string theme)
    {
        if (Theme == theme) return;
        Theme = theme;
        Save();
        ThemeChanged?.Invoke();
    }

    public void Toggle()
    {
        SetTheme(Theme == "light" ? "dark" : "light");
    }

    private void Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<ThemeSettings>(json);
                if (settings?.Theme is "light" or "dark")
                    Theme = settings.Theme;
            }
        }
        catch { }
    }

    private void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsPath);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(SettingsPath,
                JsonSerializer.Serialize(new ThemeSettings { Theme = Theme }));
        }
        catch { }
    }

    private class ThemeSettings
    {
        public string Theme { get; set; } = "light";
    }
}
