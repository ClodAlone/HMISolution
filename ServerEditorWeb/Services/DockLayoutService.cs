// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Text.Json;

namespace ServerEditorWeb.Services;

public class DockLayoutSettings
{
    public Dictionary<string, PanelLayoutState> Panels { get; set; } = new();
    public string ActiveCenterTab { get; set; } = "";
    public string ActiveCenter2Tab { get; set; } = "";
    public string ActiveRightTab { get; set; } = "";
    public string ActiveBottomTab { get; set; } = "";
    public int RightWidth { get; set; } = 400;
    public int Center2Width { get; set; } = 500;
    public int BottomHeight { get; set; } = 200;
}

public class PanelLayoutState
{
    public string Zone { get; set; } = "center";
    public double FloatX { get; set; }
    public double FloatY { get; set; }
    public double FloatWidth { get; set; } = 500;
    public double FloatHeight { get; set; } = 400;
}

public class DockLayoutService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SimpleOpcFileServer", "dock_layout.json");

    public DockLayoutSettings? Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<DockLayoutSettings>(json);
            }
        }
        catch { }
        return null;
    }

    public void Save(DockLayoutSettings settings)
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsPath);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
        catch { }
    }
}
