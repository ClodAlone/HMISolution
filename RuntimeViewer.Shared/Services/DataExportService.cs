using System.Globalization;
using System.Text;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Generates CSV and HTML exports from historical data, trends, and event logs.
/// Works entirely in-memory — no external packages required.
/// </summary>
public class DataExportService
{
    /// <summary>Export HDA series data to CSV format.</summary>
    public string HdaToCsv(List<HdaSeries> series)
    {
        if (series.Count == 0) return "";

        var sb = new StringBuilder();

        // Merge all series by timestamp
        var allTimes = new SortedSet<DateTime>();
        foreach (var s in series)
            foreach (var pt in s.Points)
                allTimes.Add(pt.Time);

        // Header
        sb.Append("Timestamp");
        foreach (var s in series)
            sb.Append(',').Append(CsvEscape(s.VariableName));
        sb.AppendLine();

        // Rows
        var lookup = series.ToDictionary(
            s => s.VariableName,
            s => s.Points.ToDictionary(p => p.Time));

        foreach (var time in allTimes)
        {
            sb.Append(time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
            foreach (var s in series)
            {
                sb.Append(',');
                if (lookup[s.VariableName].TryGetValue(time, out var pt))
                    sb.Append(pt.Value.HasValue ? pt.Value.Value.ToString(CultureInfo.InvariantCulture) : CsvEscape(pt.StringValue ?? ""));
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>Export HDA series data to a printable HTML table.</summary>
    public string HdaToHtml(List<HdaSeries> series, string title = "Historical Data Export")
    {
        if (series.Count == 0) return "";

        var allTimes = new SortedSet<DateTime>();
        foreach (var s in series)
            foreach (var pt in s.Points)
                allTimes.Add(pt.Time);

        var lookup = series.ToDictionary(
            s => s.VariableName,
            s => s.Points.ToDictionary(p => p.Time));

        var sb = new StringBuilder();
        WriteHtmlHeader(sb, title);

        // Summary
        sb.AppendLine("<div class=\"summary\">");
        sb.AppendLine($"<p><strong>Exported:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss} &nbsp; <strong>Points:</strong> {allTimes.Count} &nbsp; <strong>Variables:</strong> {series.Count}</p>");
        foreach (var s in series)
        {
            var pts = s.Points.Where(p => p.Value.HasValue).Select(p => p.Value!.Value).ToList();
            if (pts.Count > 0)
                sb.AppendLine($"<p class=\"stat\"><strong>{Esc(s.VariableName)}</strong>: min {pts.Min():F2} · max {pts.Max():F2} · avg {pts.Average():F2}</p>");
        }
        sb.AppendLine("</div>");

        // Table
        sb.AppendLine("<table><thead><tr><th>Timestamp</th>");
        foreach (var s in series)
            sb.Append($"<th>{Esc(s.VariableName)}</th>");
        sb.AppendLine("</tr></thead><tbody>");

        foreach (var time in allTimes)
        {
            sb.Append($"<tr><td>{time.ToLocalTime():yyyy-MM-dd HH:mm:ss.fff}</td>");
            foreach (var s in series)
            {
                if (lookup[s.VariableName].TryGetValue(time, out var pt))
                    sb.Append($"<td>{(pt.Value.HasValue ? pt.Value.Value.ToString("F4", CultureInfo.InvariantCulture) : Esc(pt.StringValue ?? ""))}</td>");
                else
                    sb.Append("<td></td>");
            }
            sb.AppendLine("</tr>");
        }

        sb.AppendLine("</tbody></table>");
        WriteHtmlFooter(sb);

        return sb.ToString();
    }

    /// <summary>Export event log entries to CSV format.</summary>
    public string EventsToCsv(List<EventLogEntry> events)
    {
        if (events.Count == 0) return "";

        var sb = new StringBuilder();
        sb.AppendLine("Timestamp,Category,Severity,Source,Message,Details");

        foreach (var e in events)
        {
            sb.Append(e.Time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            sb.Append(',').Append(CsvEscape(e.Category));
            sb.Append(',').Append(CsvEscape(e.Severity));
            sb.Append(',').Append(CsvEscape(e.Source));
            sb.Append(',').Append(CsvEscape(e.Message));
            sb.Append(',').Append(CsvEscape(e.Details ?? ""));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>Export event log entries to a printable HTML report.</summary>
    public string EventsToHtml(List<EventLogEntry> events, string title = "Event Log Export")
    {
        if (events.Count == 0) return "";

        var sb = new StringBuilder();
        WriteHtmlHeader(sb, title);

        // Summary
        sb.AppendLine("<div class=\"summary\">");
        sb.AppendLine($"<p><strong>Exported:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss} &nbsp; <strong>Events:</strong> {events.Count}</p>");
        var byCat = events.GroupBy(e => e.Category).OrderBy(g => g.Key);
        foreach (var g in byCat)
            sb.AppendLine($"<span class=\"stat\">{Esc(g.Key)}: {g.Count()} &nbsp;</span>");
        sb.AppendLine("</div>");

        // Table
        sb.AppendLine("<table><thead><tr><th>Time</th><th>Category</th><th>Severity</th><th>Source</th><th>Message</th></tr></thead><tbody>");

        foreach (var e in events)
        {
            var severityClass = e.Severity switch
            {
                "Error" or "Critical" => " class=\"severity-error\"",
                "Warning" => " class=\"severity-warning\"",
                _ => ""
            };
            sb.Append($"<tr><td>{e.Time.ToLocalTime():yyyy-MM-dd HH:mm:ss}</td>");
            sb.Append($"<td>{Esc(e.Category)}</td>");
            sb.Append($"<td{severityClass}>{Esc(e.Severity)}</td>");
            sb.Append($"<td>{Esc(e.Source)}</td>");
            sb.Append($"<td>{Esc(e.Message)}</td></tr>\n");
        }

        sb.AppendLine("</tbody></table>");
        WriteHtmlFooter(sb);

        return sb.ToString();
    }

    private static string CsvEscape(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        return value;
    }

    private static string Esc(string value) =>
        System.Net.WebUtility.HtmlEncode(value);

    private static void WriteHtmlHeader(StringBuilder sb, string title)
    {
        sb.AppendLine("<!DOCTYPE html><html><head><meta charset=\"utf-8\">");
        sb.AppendLine($"<title>{Esc(title)}</title>");
        sb.AppendLine("""
<style>
  body { font-family: 'Segoe UI', Arial, sans-serif; margin: 20px; color: #222; }
  h1 { font-size: 18px; margin-bottom: 8px; }
  .summary { background: #f5f5f5; padding: 10px 14px; border-radius: 4px; margin-bottom: 12px; font-size: 12px; }
  .stat { font-size: 11px; color: #555; }
  table { width: 100%; border-collapse: collapse; font-size: 11px; }
  th { background: #e2e8f0; padding: 6px 8px; text-align: left; border: 1px solid #cbd5e1; font-weight: 600; }
  td { padding: 4px 8px; border: 1px solid #e2e8f0; }
  tr:nth-child(even) { background: #f8fafc; }
  .severity-error { color: #dc2626; font-weight: bold; }
  .severity-warning { color: #d97706; }
  @media print { body { margin: 0; } .no-print { display: none; } }
</style>
""");
        sb.AppendLine("</head><body>");
        sb.AppendLine($"<h1>{Esc(title)}</h1>");
    }

    private static void WriteHtmlFooter(StringBuilder sb)
    {
        sb.AppendLine("</body></html>");
    }
}
