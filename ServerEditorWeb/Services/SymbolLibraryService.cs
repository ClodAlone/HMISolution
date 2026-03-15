namespace ServerEditorWeb.Services;

public class SymbolLibraryItem
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string SvgContent { get; set; } = "";
    public double DefaultWidth { get; set; } = 64;
    public double DefaultHeight { get; set; } = 64;
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

        // Sanitize category name to a safe directory name
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

        // Force reload on next access
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

                    // Try to extract viewBox dimensions for default size
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

                    // Normalize to fit inside a reasonable bounding box
                    var scale = Math.Min(80.0 / w, 80.0 / h);
                    w = Math.Round(w * scale);
                    h = Math.Round(h * scale);

                    items.Add(new SymbolLibraryItem
                    {
                        Name = name,
                        Category = category,
                        SvgContent = svg,
                        DefaultWidth = w,
                        DefaultHeight = h
                    });
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
