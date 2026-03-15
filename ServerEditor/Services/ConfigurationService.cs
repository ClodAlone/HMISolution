using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace ServerEditor.Services
{
    public class WindowSettings
    {
        public double Top { get; set; } = 100;
        public double Left { get; set; } = 100;
        public double Width { get; set; } = 1000;
        public double Height { get; set; } = 600;
        public WindowState WindowState { get; set; } = WindowState.Normal;
        public bool IsDarkTheme { get; set; } = true;
        private List<string> _recentEndpoints = new();
        public List<string> RecentEndpoints 
        { 
            get => _recentEndpoints; 
            set => _recentEndpoints = value ?? new List<string>(); 
        }
    }

    public static class ConfigurationService
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "window.config.json");

        public static void Save(WindowSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        public static WindowSettings Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    return JsonSerializer.Deserialize<WindowSettings>(json) ?? new WindowSettings();
                }
            }
            catch 
            { 
                // Ignore parse errors 
            }
            return new WindowSettings();
        }
    }
}