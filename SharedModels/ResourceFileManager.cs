// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SharedModels
{
    /// <summary>
    /// Manages external resource files for Scripts, Screens, and PLC Programs.
    /// Resources are stored as individual JSON files in subfolders next to the main config file:
    ///   scripts/{name}.json
    ///   screens/{name}.json
    ///   plcprograms/{name}.json
    /// On load, inline resources in the main JSON are supported for backward compatibility.
    /// On save, resources are always written to external files and removed from the main JSON.
    /// </summary>
    public static class ResourceFileManager
    {
        private const string ScriptsFolder = "scripts";
        private const string ScreensFolder = "screens";
        private const string PlcProgramsFolder = "plcprograms";
        private const string RecipesFolder = "recipes";

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            WriteIndented = true
        };

        /// <summary>
        /// After deserializing a NodeModel from the main config file,
        /// load any external resource files and merge them into the model.
        /// Inline resources (from legacy files) are kept as-is — they will be
        /// migrated to external files on the next save.
        /// </summary>
        public static void LoadExternalResources(NodeModel model, string configPath)
        {
            if (model == null) return;
            var dir = Path.GetDirectoryName(Path.GetFullPath(configPath));
            if (string.IsNullOrEmpty(dir)) return;

            // Load external scripts
            var scriptsDir = Path.Combine(dir, ScriptsFolder);
            if (Directory.Exists(scriptsDir))
            {
                foreach (var file in Directory.GetFiles(scriptsDir, "*.json"))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var script = JsonSerializer.Deserialize<ScriptConfig>(json);
                        if (script != null && !model.Scripts.Any(s =>
                            s.Name.Equals(script.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            model.Scripts.Add(script);
                        }
                    }
                    catch { /* skip corrupt files */ }
                }
            }

            // Load external screens
            var screensDir = Path.Combine(dir, ScreensFolder);
            if (Directory.Exists(screensDir))
            {
                foreach (var file in Directory.GetFiles(screensDir, "*.json"))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var screen = JsonSerializer.Deserialize<ScreenConfig>(json);
                        if (screen != null && !model.Screens.Any(s =>
                            s.Name.Equals(screen.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            model.Screens.Add(screen);
                        }
                    }
                    catch { /* skip corrupt files */ }
                }
            }

            // Load external PLC programs
            var plcDir = Path.Combine(dir, PlcProgramsFolder);
            if (Directory.Exists(plcDir))
            {
                foreach (var file in Directory.GetFiles(plcDir, "*.json"))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var plc = JsonSerializer.Deserialize<PlcProgramConfig>(json);
                        if (plc != null && !model.PlcPrograms.Any(p =>
                            p.Name.Equals(plc.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            model.PlcPrograms.Add(plc);
                        }
                    }
                    catch { /* skip corrupt files */ }
                }
            }

            // Load external recipes
            var recipesDir = Path.Combine(dir, RecipesFolder);
            if (Directory.Exists(recipesDir))
            {
                foreach (var file in Directory.GetFiles(recipesDir, "*.json"))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var recipe = JsonSerializer.Deserialize<RecipeConfig>(json);
                        if (recipe != null && !model.Recipes.Any(r =>
                            r.Name.Equals(recipe.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            model.Recipes.Add(recipe);
                        }
                    }
                    catch { /* skip corrupt files */ }
                }
            }
        }

        /// <summary>
        /// Saves each resource to its own external JSON file and clears the inline
        /// lists so the main config file stays clean.
        /// Also removes external files for resources that no longer exist.
        /// Returns the model with inline lists cleared (caller should serialize this).
        /// </summary>
        public static void SaveExternalResources(NodeModel model, string configPath)
        {
            if (model == null) return;
            var dir = Path.GetDirectoryName(Path.GetFullPath(configPath));
            if (string.IsNullOrEmpty(dir)) return;

            SaveResourceList(model.Scripts, dir, ScriptsFolder, s => s.Name);
            SaveResourceList(model.Screens, dir, ScreensFolder, s => s.Name);
            SaveResourceList(model.PlcPrograms, dir, PlcProgramsFolder, p => p.Name);
            SaveResourceList(model.Recipes, dir, RecipesFolder, r => r.Name);
        }

        /// <summary>
        /// Clears the inline resource lists so the main config file doesn't contain them.
        /// Call this AFTER SaveExternalResources and BEFORE serializing the main config.
        /// Returns a snapshot of the lists so the caller can restore them after serialization.
        /// </summary>
        public static ResourceSnapshot DetachResources(NodeModel model)
        {
            var snapshot = new ResourceSnapshot
            {
                Scripts = new List<ScriptConfig>(model.Scripts),
                Screens = new List<ScreenConfig>(model.Screens),
                PlcPrograms = new List<PlcProgramConfig>(model.PlcPrograms),
                Recipes = new List<RecipeConfig>(model.Recipes)
            };

            model.Scripts = new List<ScriptConfig>();
            model.Screens = new List<ScreenConfig>();
            model.PlcPrograms = new List<PlcProgramConfig>();
            model.Recipes = new List<RecipeConfig>();

            return snapshot;
        }

        /// <summary>
        /// Restores the inline resource lists after serialization of the main config.
        /// </summary>
        public static void ReattachResources(NodeModel model, ResourceSnapshot snapshot)
        {
            model.Scripts = snapshot.Scripts;
            model.Screens = snapshot.Screens;
            model.PlcPrograms = snapshot.PlcPrograms;
            model.Recipes = snapshot.Recipes;
        }

        private static void SaveResourceList<T>(List<T> items, string baseDir, string subfolder,
            Func<T, string> getName)
        {
            var folder = Path.Combine(baseDir, subfolder);
            Directory.CreateDirectory(folder);

            // Build set of current resource names
            var currentNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                var name = getName(item);
                if (string.IsNullOrWhiteSpace(name)) continue;

                currentNames.Add(name);
                var safeName = SanitizeFileName(name);
                var filePath = Path.Combine(folder, safeName + ".json");
                var json = JsonSerializer.Serialize(item, _jsonOpts);
                File.WriteAllText(filePath, json);
            }

            // Remove external files for deleted resources
            foreach (var file in Directory.GetFiles(folder, "*.json"))
            {
                var fileResourceName = Path.GetFileNameWithoutExtension(file);
                if (!currentNames.Any(n =>
                    SanitizeFileName(n).Equals(fileResourceName, StringComparison.OrdinalIgnoreCase)))
                {
                    try { File.Delete(file); }
                    catch { /* best effort */ }
                }
            }
        }

        /// <summary>
        /// Sanitizes a resource name for use as a file name.
        /// Replaces invalid characters with underscores.
        /// </summary>
        public static string SanitizeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var chars = name.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (Array.IndexOf(invalid, chars[i]) >= 0)
                    chars[i] = '_';
            }
            return new string(chars).Trim();
        }
    }

    /// <summary>Holds a snapshot of resource lists for temporary detach/reattach during serialization.</summary>
    public class ResourceSnapshot
    {
        public List<ScriptConfig> Scripts { get; set; } = new();
        public List<ScreenConfig> Screens { get; set; } = new();
        public List<PlcProgramConfig> PlcPrograms { get; set; } = new();
        public List<RecipeConfig> Recipes { get; set; } = new();
    }
}
