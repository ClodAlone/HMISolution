using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows.Controls;

namespace WpfApp1.Services
{
    public sealed class PluginInfo
    {
        public string Name { get; init; } = string.Empty;
        public Func<UserControl?> CreateControl { get; init; } = () => null;
        public string AssemblyPath { get; init; } = string.Empty;
    }

    public sealed class PluginManager
    {
        private readonly List<PluginInfo> _plugins = new();

        public IReadOnlyList<PluginInfo> Plugins => _plugins.AsReadOnly();

        public void LoadPlugins(string folderPath)
        {
            _plugins.Clear();

            // discover plugins in the current assembly first
            var current = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            DiscoverFromAssembly(current, "(internal)");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var dlls = Directory.GetFiles(folderPath, "*.dll");
            foreach (var dll in dlls)
            {
                try
                {
                    var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(dll));
                    DiscoverFromAssembly(asm, dll);
                }
                catch
                {
                    // ignore plugin load failures
                }
            }
        }

        private void DiscoverFromAssembly(Assembly asm, string source)
        {
            foreach (var type in asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {

                // prefer types that implement IPlugin
                var iPlugin = typeof(WpfApp1.Plugins.IPlugin);
                if (iPlugin != null && iPlugin.IsAssignableFrom(type))
                {
                    // create factory using the interface
                    Func<UserControl?> factoryPlugin = () =>
                    {
                        try
                        {
                            var inst = Activator.CreateInstance(type) as dynamic;
                            return (UserControl?)inst.CreateControl();
                        }
                        catch
                        {
                            return null;
                        }
                    };

                    string pluginName = type.Name;
                    try { pluginName = ((dynamic?)Activator.CreateInstance(type))?.Name ?? pluginName; } catch { }
                    _plugins.Add(new PluginInfo { Name = pluginName, CreateControl = factoryPlugin, AssemblyPath = source });
                    continue;
                }

                // look for a method CreateControl returning UserControl
                var method = type.GetMethod("CreateControl", BindingFlags.Public | BindingFlags.Static)
                             ?? type.GetMethod("CreateControl", BindingFlags.Public | BindingFlags.Instance);

                if (method == null) continue;
                if (!typeof(UserControl).IsAssignableFrom(method.ReturnType)) continue;

                string name = type.Name;
                // look for Name property
                var nameProp = type.GetProperty("Name", BindingFlags.Public | BindingFlags.Static)
                           ?? type.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                if (nameProp != null && nameProp.PropertyType == typeof(string))
                {
                    try
                    {
                        var val = nameProp.GetValue(null) as string ?? nameProp.GetValue(Activator.CreateInstance(type)) as string;
                        if (!string.IsNullOrEmpty(val)) name = val;
                    }
                    catch { }
                }

                Func<UserControl?> factory2 = () =>
                {
                    try
                    {
                        if (method.IsStatic)
                        {
                            return method.Invoke(null, Array.Empty<object>()) as UserControl;
                        }
                        else
                        {
                            var inst = Activator.CreateInstance(type);
                            return method.Invoke(inst, Array.Empty<object>()) as UserControl;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                };

                _plugins.Add(new PluginInfo { Name = name, CreateControl = factory2, AssemblyPath = source });
            }
        }
    }
}
