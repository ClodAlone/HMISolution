using SharedModels;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Scoped service that resolves localized strings at runtime.
/// Labels prefixed with @ are treated as string IDs (e.g., "@btn_start").
/// </summary>
public class LocalizationService
{
    private readonly ProjectService _project;

    public string ActiveLanguage { get; private set; } = "en";
    public event Action? LanguageChanged;

    public LocalizationService(ProjectService project)
    {
        _project = project;
    }

    /// <summary>
    /// Set the active language code. Triggers LanguageChanged so all screens re-render.
    /// </summary>
    public void SetLanguage(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode)) return;
        ActiveLanguage = languageCode.Trim().ToLowerInvariant();
        LanguageChanged?.Invoke();
    }

    /// <summary>
    /// Resolve a label string. If it starts with "@", look up the string ID
    /// in the project's Strings table for the active language.
    /// If no translation is found, falls back to the first available language, then the key itself.
    /// </summary>
    public string Resolve(string? label)
    {
        if (string.IsNullOrEmpty(label)) return "";
        if (!label.StartsWith('@')) return label;

        var key = label[1..]; // strip the @ prefix
        var strings = _project.Model?.Strings;
        if (strings == null || strings.Count == 0) return key;

        var entry = strings.FirstOrDefault(s =>
            s.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (entry == null) return key;

        // Try active language
        if (entry.Translations.TryGetValue(ActiveLanguage, out var text) && !string.IsNullOrEmpty(text))
            return text;

        // Fallback: first non-empty translation
        var fallback = entry.Translations.Values.FirstOrDefault(v => !string.IsNullOrEmpty(v));
        return fallback ?? key;
    }

    /// <summary>
    /// Get all available language codes from the project's Strings table.
    /// </summary>
    public List<string> GetAvailableLanguages()
    {
        var strings = _project.Model?.Strings;
        if (strings == null) return new() { "en" };

        var langs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in strings)
        {
            foreach (var lang in entry.Translations.Keys)
                langs.Add(lang);
        }

        return langs.Count > 0 ? langs.OrderBy(l => l).ToList() : new() { "en" };
    }
}

