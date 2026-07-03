// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

namespace ServerEditorWeb.Services;

/// <summary>
/// Lightweight line-level diff helper used to render "live progress" indicators
/// while the AI assistant is streaming a new JSON document.
/// </summary>
public static class SimpleJsonDiff
{
    public readonly record struct Stats(int Added, int Removed, int Unchanged);

    /// <summary>
    /// Compares two text blobs line-by-line and returns rough add/remove counts.
    /// Uses a multiset intersection (order-insensitive) so it stays cheap even
    /// when called at streaming cadence on multi-thousand-line JSON.
    /// </summary>
    public static Stats Compare(string? oldText, string? newText)
    {
        var oldLines = SplitNonEmpty(oldText);
        var newLines = SplitNonEmpty(newText);

        var oldCounts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var line in oldLines)
        {
            oldCounts.TryGetValue(line, out var c);
            oldCounts[line] = c + 1;
        }

        int unchanged = 0;
        foreach (var line in newLines)
        {
            if (oldCounts.TryGetValue(line, out var c) && c > 0)
            {
                unchanged++;
                oldCounts[line] = c - 1;
            }
        }

        int added = newLines.Count - unchanged;
        int removed = oldLines.Count - unchanged;
        return new Stats(added, removed, unchanged);
    }

    private static List<string> SplitNonEmpty(string? text)
    {
        if (string.IsNullOrEmpty(text)) return [];
        var result = new List<string>(64);
        foreach (var raw in text.Split('\n'))
        {
            var trimmed = raw.TrimEnd('\r');
            if (trimmed.Length == 0) continue;
            result.Add(trimmed);
        }
        return result;
    }
}
