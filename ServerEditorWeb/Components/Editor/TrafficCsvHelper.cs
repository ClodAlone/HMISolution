using ServerEditorWeb.Services;

namespace ServerEditorWeb.Components.Editor;

/// <summary>CSV helpers for the Protocol Traffic Monitor panel.</summary>
internal static class TrafficCsvHelper
{
    internal static string Escape(string s)
    {
        // Wrap field in quotes and double any embedded quotes
        var sb = new System.Text.StringBuilder(s.Length + 2);
        sb.Append('"');
        foreach (char c in s)
        {
            if (c == '"') sb.Append('"');
            sb.Append(c);
        }
        sb.Append('"');
        return sb.ToString();
    }

    internal static string FormatRow(TrafficFrame f)
        => string.Join(",",
               f.Sequence.ToString(),
               f.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
               f.Protocol.ToString(),
               f.Direction.ToString(),
               f.Level,
               Escape(f.Source),
               Escape(f.Summary));
}
