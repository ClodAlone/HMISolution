using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using WpfApp3.Contracts;

namespace WpfApp3.Services
{
    public class PluginManager
    {
        private static PluginManager? _instance;
        public static PluginManager Instance => _instance ??= new PluginManager();

        public ObservableCollection<IPlugin> LoadedPlugins { get; } = new();

        public event Action<IPlugin>? PluginLoaded;

        private PluginManager() { }

        public void LoadPlugin(IPlugin plugin)
        {
            if (!LoadedPlugins.Any(p => p.GetType() == plugin.GetType()))
            {
                LoadedPlugins.Add(plugin);
                PluginLoaded?.Invoke(plugin);
            }
        }

        public void LoadPluginsFromFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;

            var dllFiles = Directory.GetFiles(folderPath, "*.dll");
            foreach (var file in dllFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    LoadPluginsFromAssembly(assembly);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading plugin from {file}: {ex.Message}");
                }
            }
        }

        public void LoadPluginsFromAssembly(Assembly assembly)
        {
            try
            {
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in pluginTypes)
                {
                    // Check if already loaded to avoid duplicates
                    if (LoadedPlugins.Any(p => p.GetType() == type)) continue;

                    if (Activator.CreateInstance(type) is IPlugin plugin)
                    {
                        LoadPlugin(plugin);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading plugins from assembly {assembly.FullName}: {ex.Message}");
            }
        }
    }
}
