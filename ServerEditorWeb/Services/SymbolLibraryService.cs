// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Text.Json;
using SharedModels;

namespace ServerEditorWeb.Services;

public class SymbolLibraryItem
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string SvgContent { get; set; } = "";
    public double DefaultWidth { get; set; } = 64;
    public double DefaultHeight { get; set; } = 64;
    /// <summary>File path on disk for saving animation sidecar data.</summary>
    public string FilePath { get; set; } = "";
    /// <summary>Animations defined for this library symbol.</summary>
    public List<SymbolAnimation> Animations { get; set; } = new();

    /// <summary>Save animations to the .anim.json sidecar file.</summary>
    public void SaveAnimations()
    {
        if (string.IsNullOrEmpty(FilePath)) return;
        var animPath = FilePath + ".anim.json";
        var json = JsonSerializer.Serialize(Animations, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(animPath, json);
    }
}

public class SymbolLibraryService
{
    private readonly string _libraryPath;
    private List<SymbolLibraryItem>? _items;

    public SymbolLibraryService(IWebHostEnvironment env)
    {
        _libraryPath = Path.Combine(env.ContentRootPath, "SymbolLibrary");
    }

    public IReadOnlyList<SymbolLibraryItem> GetSymbols()
    {
        _items ??= LoadSymbols();
        return _items;
    }

    public IReadOnlyList<string> GetCategories()
    {
        return GetSymbols().Select(s => s.Category).Distinct().OrderBy(c => c).ToList();
    }

    public void Refresh()
    {
        _items = null;
    }

    /// <summary>
    /// Deletes an entire category (its on-disk folder, all SVGs, and animation sidecars).
    /// Returns true if the category existed and was removed.
    /// </summary>
    public bool DeleteCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category)) return false;

        // Resolve to an actual on-disk directory name rather than trusting the caller's
        // string directly, so this can't be tricked into deleting something outside
        // _libraryPath (e.g. via "..\\" segments) and only matches an existing category.
        var dirName = Directory.Exists(_libraryPath)
            ? Directory.GetDirectories(_libraryPath)
                .Select(Path.GetFileName)
                .FirstOrDefault(n => string.Equals(n, category, StringComparison.OrdinalIgnoreCase))
            : null;
        if (dirName is null) return false;

        var categoryDir = Path.Combine(_libraryPath, dirName);
        Directory.Delete(categoryDir, recursive: true);
        _items = null;
        return true;
    }

    /// <summary>
    /// Saves uploaded SVG files into the given category folder.
    /// Creates the category directory if it doesn't exist.
    /// Returns the number of files saved and the actual (sanitized/trimmed) category
    /// name used on disk, which may differ from the input if it contained invalid
    /// filename characters or leading/trailing whitespace.
    /// </summary>
    /// <param name="progress">Optional callback invoked with the number of files written so far.</param>
    public (int Count, string Category) ImportSvgFiles(string category, IReadOnlyList<(string FileName, byte[] Content)> files, IProgress<int>? progress = null)
    {
        if (string.IsNullOrWhiteSpace(category)) return (0, category);

        // Trim first: Windows silently strips trailing spaces/dots from directory names,
        // so an un-trimmed category (e.g. "Imported ") would create a folder named
        // "Imported" on disk while the UI kept comparing against "Imported " and never
        // found the freshly imported symbols again.
        var trimmed = category.Trim();
        var safeName = string.Concat(trimmed.Select(c =>
            Path.GetInvalidFileNameChars().Contains(c) ? '_' : c)).Trim();
        if (string.IsNullOrEmpty(safeName)) return (0, category);

        var categoryDir = Path.Combine(_libraryPath, safeName);
        Directory.CreateDirectory(categoryDir);

        int count = 0;
        foreach (var (fileName, content) in files)
        {
            if (!fileName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                continue;

            var safeFn = string.Concat(Path.GetFileName(fileName).Select(c =>
                Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
            safeFn = ShortenFileName(safeFn);
            var dest = Path.Combine(categoryDir, safeFn);
            File.WriteAllBytes(dest, content);
            count++;
            progress?.Report(count);
        }

        _items = null;
        return (count, safeName);
    }

    /// <summary>
    /// Some SVG symbol sets (e.g. FUXA's widget library) ship files with very long,
    /// descriptive names. Combined with the SymbolLibrary folder's own path depth and the
    /// project's bin/publish output path, these can exceed Windows' legacy 260-char MAX_PATH
    /// limit, causing MSBuild's build-time copy of Content items to fail with MSB3027
    /// ("Exceeded retry count"). Truncate the base name (keeping the extension) and append a
    /// short stable hash to avoid collisions between different files that share a long prefix.
    /// </summary>
    private static string ShortenFileName(string fileName, int maxBaseLength = 40)
    {
        var ext = Path.GetExtension(fileName);
        var baseName = Path.GetFileNameWithoutExtension(fileName);
        if (baseName.Length <= maxBaseLength)
            return fileName;

        var hash = Convert.ToHexString(System.Security.Cryptography.MD5.HashData(
            System.Text.Encoding.UTF8.GetBytes(fileName))).Substring(0, 8);
        var truncated = baseName.Substring(0, maxBaseLength).TrimEnd();
        return $"{truncated}_{hash}{ext}";
    }

    private List<SymbolLibraryItem> LoadSymbols()
    {
        var items = new List<SymbolLibraryItem>();
        if (!Directory.Exists(_libraryPath))
            return items;

        foreach (var categoryDir in Directory.GetDirectories(_libraryPath).OrderBy(d => d))
        {
            var category = Path.GetFileName(categoryDir);
            foreach (var svgFile in Directory.GetFiles(categoryDir, "*.svg").OrderBy(f => f))
            {
                try
                {
                    var svg = File.ReadAllText(svgFile).Trim();
                    var name = Path.GetFileNameWithoutExtension(svgFile)
                        .Replace('-', ' ').Replace('_', ' ');

                    double w = 64, h = 64;
                    var vbMatch = System.Text.RegularExpressions.Regex.Match(svg,
                        @"viewBox\s*=\s*""[\d.\-]+\s+[\d.\-]+\s+([\d.]+)\s+([\d.]+)""");
                    if (vbMatch.Success)
                    {
                        if (double.TryParse(vbMatch.Groups[1].Value, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out var vw))
                            w = vw;
                        if (double.TryParse(vbMatch.Groups[2].Value, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out var vh))
                            h = vh;
                    }

                    var scale = Math.Min(80.0 / w, 80.0 / h);
                    w = Math.Round(w * scale);
                    h = Math.Round(h * scale);

                    var item = new SymbolLibraryItem
                    {
                        Name = name,
                        Category = category,
                        SvgContent = svg,
                        DefaultWidth = w,
                        DefaultHeight = h,
                        FilePath = svgFile
                    };

                    // Load animation sidecar
                    var animPath = svgFile + ".anim.json";
                    if (File.Exists(animPath))
                    {
                        try
                        {
                            var animJson = File.ReadAllText(animPath);
                            var anims = JsonSerializer.Deserialize<List<SymbolAnimation>>(animJson);
                            if (anims != null) item.Animations = anims;
                        }
                        catch { /* ignore bad json */ }
                    }

                    items.Add(item);
                }
                catch
                {
                    // Skip invalid SVG files
                }
            }
        }
        return items;
    }
}
