namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Manages a pool of pre-loaded screens kept alive but hidden.
/// Screens are cached based on navigation prediction and recent history.
/// Memory pressure is monitored to evict screens when needed.
/// </summary>
public class ScreenPreloadService
{
    /// <summary>Maximum cached screens (excluding the active one).</summary>
    private const int DefaultMaxCached = 3;

    /// <summary>Memory threshold in MB. When available memory drops below this, evict.</summary>
    private const long MemoryThresholdMb = 200;

    private readonly List<CachedScreen> _cache = [];
    private string? _activeScreenName;

    /// <summary>All screen names that should have a live component (active + cached).</summary>
    public IReadOnlyList<CachedScreen> Cache => _cache;

    /// <summary>The currently visible screen name.</summary>
    public string? ActiveScreenName => _activeScreenName;

    /// <summary>Maximum number of cached screens (configurable).</summary>
    public int MaxCached { get; set; } = DefaultMaxCached;

    /// <summary>Whether preloading is enabled.</summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Called when the user navigates to a screen. Promotes it to active,
    /// demotes the previous active to cached, and queues predicted screens.
    /// </summary>
    public void Navigate(string screenName, IReadOnlyList<string> predictedScreenNames)
    {
        if (string.IsNullOrEmpty(screenName)) return;

        _activeScreenName = screenName;

        // Ensure the active screen is in the cache
        var active = _cache.FirstOrDefault(c => c.Name.Equals(screenName, StringComparison.OrdinalIgnoreCase));
        if (active == null)
        {
            active = new CachedScreen(screenName);
            _cache.Insert(0, active);
        }
        active.LastAccessed = DateTime.UtcNow;
        active.AccessCount++;

        if (!Enabled) return;

        // Add predicted screens to cache if not already there
        foreach (var predicted in predictedScreenNames)
        {
            if (string.IsNullOrEmpty(predicted)) continue;
            if (_cache.Any(c => c.Name.Equals(predicted, StringComparison.OrdinalIgnoreCase))) continue;
            _cache.Add(new CachedScreen(predicted));
        }

        // Evict under memory pressure or over capacity
        Evict();
    }
    /// <summary>Check if a screen should be visible (is the active screen).</summary>
    public bool IsVisible(string screenName)
    {
        return _activeScreenName != null &&
               _activeScreenName.Equals(screenName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Check if a screen is in the cache (should have a live component).</summary>
    public bool IsCached(string screenName)
    {
        return _cache.Any(c => c.Name.Equals(screenName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Returns the list of screen names that should be alive (for rendering).</summary>
    public List<string> GetAliveScreenNames()
    {
        return _cache.Select(c => c.Name).ToList();
    }
    private void Evict()
    {
        // Check memory pressure
        var memInfo = GC.GetGCMemoryInfo();
        long availableMb = (memInfo.TotalAvailableMemoryBytes - memInfo.MemoryLoadBytes) / (1024 * 1024);
        bool memoryPressure = availableMb < MemoryThresholdMb;

        int maxAllowed = memoryPressure ? 1 : MaxCached;

        // Never evict the active screen
        var evictable = _cache
            .Where(c => !c.Name.Equals(_activeScreenName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.AccessCount)
            .ThenBy(c => c.LastAccessed)
            .ToList();

        // +1 for the active screen
        while (_cache.Count > maxAllowed + 1 && evictable.Count > 0)
        {
            var victim = evictable[0];
            evictable.RemoveAt(0);
            _cache.Remove(victim);
        }
    }

    /// <summary>Force evict all cached screens (e.g. on low memory warning).</summary>
    public void Clear()
    {
        _cache.RemoveAll(c => !c.Name.Equals(_activeScreenName, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>Represents a cached screen entry with usage statistics.</summary>
public class CachedScreen
{
    public string Name { get; }
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
    public int AccessCount { get; set; }

    public CachedScreen(string name) => Name = name;
}