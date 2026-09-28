// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Text.Json;
using SharedModels;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Resolves the alias/parameter mapping that should be applied to a screen rendered in
/// Augmented Reality mode for a given recognized object class (and, optionally, instance key
/// such as a QR/AprilTag ID reported by the local object-recognition server).
///
/// Parameter files let a single screen template be reused for many physical instances of the
/// same visually-recognized object class (e.g. "Motor" template mapped to "Plant.Line1.Motor1"
/// for one physical motor and "Plant.Line1.Motor2" for another), without duplicating screens.
/// </summary>
public class ArParameterFileService
{
    private readonly Dictionary<string, Dictionary<string, string>> _fileCache = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Resolves the effective alias map (placeholder -> real OPC path) for the given mapping and
    /// optional instance key. Precedence: per-instance parameter file &gt; single parameter file
    /// &gt; named AliasMap from the project &gt; null (screen uses its own bindings as-is).
    /// </summary>
    public Dictionary<string, string>? Resolve(ArObjectMapping mapping, ProjectService project, string? instanceKey)
    {
        var projectDir = GetProjectDir(project);

        if (!string.IsNullOrEmpty(instanceKey) &&
            mapping.InstanceParameterFiles.TryGetValue(instanceKey, out var instanceFile) &&
            !string.IsNullOrEmpty(instanceFile))
        {
            var loaded = LoadParameterFile(projectDir, instanceFile);
            if (loaded != null) return loaded;
        }

        if (!string.IsNullOrEmpty(mapping.ParameterFilePath))
        {
            var loaded = LoadParameterFile(projectDir, mapping.ParameterFilePath);
            if (loaded != null) return loaded;
        }

        if (!string.IsNullOrEmpty(mapping.AliasMapName))
        {
            var map = project.Model?.AliasMaps?.FirstOrDefault(m =>
                m.Name.Equals(mapping.AliasMapName, StringComparison.OrdinalIgnoreCase));
            if (map?.Mappings is { Count: > 0 }) return map.Mappings;
        }

        return null;
    }

    private static string GetProjectDir(ProjectService project)
    {
        try
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(project.ConfigPath));
            return string.IsNullOrEmpty(dir) ? AppContext.BaseDirectory : dir;
        }
        catch
        {
            return AppContext.BaseDirectory;
        }
    }

    /// <summary>
    /// Loads (and caches) a parameter file. Supports either the raw "{Alias: Path}" map shape,
    /// or the same shape as a VariableAliasMap ("{Name, Mappings: {...}}").
    /// </summary>
    private Dictionary<string, string>? LoadParameterFile(string projectDir, string relativeOrAbsolutePath)
    {
        var path = Path.IsPathRooted(relativeOrAbsolutePath)
            ? relativeOrAbsolutePath
            : Path.Combine(projectDir, relativeOrAbsolutePath);

        var cacheKey = path;
        if (_fileCache.TryGetValue(cacheKey, out var cached))
            return cached;

        try
        {
            if (!File.Exists(path)) return null;

            var json = File.ReadAllText(path);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            Dictionary<string, string>? map = null;

            if (root.TryGetProperty("Mappings", out var mappingsEl) && mappingsEl.ValueKind == JsonValueKind.Object)
            {
                map = mappingsEl.Deserialize<Dictionary<string, string>>();
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                // Flat "{Alias: Path}" shape
                map = root.Deserialize<Dictionary<string, string>>();
            }

            if (map != null)
                _fileCache[cacheKey] = map;

            return map;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Clears the cached parameter file contents (e.g. after a project reload).</summary>
    public void ClearCache() => _fileCache.Clear();
}
