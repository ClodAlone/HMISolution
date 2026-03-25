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
    /// Saves uploaded SVG files into the given category folder.
    /// Creates the category directory if it doesn't exist.
    /// Returns the number of files saved.
    /// </summary>
    public int ImportSvgFiles(string category, IReadOnlyList<(string FileName, byte[] Content)> files)
    {
        if (string.IsNullOrWhiteSpace(category)) return 0;

        var safeName = string.Concat(category.Select(c =>
            Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        var categoryDir = Path.Combine(_libraryPath, safeName);
        Directory.CreateDirectory(categoryDir);

        int count = 0;
        foreach (var (fileName, content) in files)
        {
            if (!fileName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                continue;

            var safeFn = string.Concat(Path.GetFileName(fileName).Select(c =>
                Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
            var dest = Path.Combine(categoryDir, safeFn);
            File.WriteAllBytes(dest, content);
            count++;
        }

        _items = null;
        return count;
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
