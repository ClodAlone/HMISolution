using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Server-side report generation engine.
    /// Generates HTML reports from ReportConfig definitions, reading historical and real-time OPC data.
    /// Supports delivery via disk storage and/or email (SMTP).
    /// </summary>
    public class ReportManager : IDisposable
    {
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly List<ReportConfig> _reports = new();
        private readonly CancellationTokenSource _cts = new();

        public ReportManager(SimpleFileServerNodeManager nodeManager)
        {
            _nodeManager = nodeManager;
        }

        public void Initialize(List<ReportConfig> reports)
        {
            foreach (var config in reports)
            {
                DiagnosticsCollector.Instance.Register("Report", config.Name, config.Enabled);
                if (config.Enabled)
                {
                    _reports.Add(config);
                }
            }

            if (_reports.Count > 0)
            {
                Log.Information("ReportManager initialized with {Count} report(s).", _reports.Count);
            }
        }

        /// <summary>
        /// Generate a report by name. Called by command execution (GenerateReport action).
        /// Returns the generated HTML content, or null if the report was not found.
        /// </summary>
        public async Task<string?> GenerateReportAsync(string reportName)
        {
            var config = _reports.FirstOrDefault(r =>
                r.Name.Equals(reportName, StringComparison.OrdinalIgnoreCase));

            if (config == null)
            {
                Log.Warning("ReportManager: Report '{Name}' not found.", reportName);
                return null;
            }

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var html = await BuildReportHtmlAsync(config);

                // Deliver
                await DeliverReportAsync(config, html);

                sw.Stop();
                DiagnosticsCollector.Instance.RecordCycle("Report", config.Name, sw.Elapsed.TotalMilliseconds);
                Log.Information("Report '{Name}' generated in {Elapsed}ms.", config.Name, sw.ElapsedMilliseconds);
                return html;
            }
            catch (Exception ex)
            {
                sw.Stop();
                DiagnosticsCollector.Instance.RecordCycle("Report", config.Name, sw.Elapsed.TotalMilliseconds);
                Log.Error(ex, "Failed to generate report '{Name}'.", config.Name);
                return null;
            }
        }

        private async Task<string> BuildReportHtmlAsync(ReportConfig config)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\" />");
            sb.AppendLine($"<title>{Escape(config.Title)}</title>");
            sb.AppendLine("<style>");
            sb.AppendLine(GetReportCss());
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Report header
            if (!string.IsNullOrEmpty(config.Title))
            {
                sb.AppendLine($"<div class=\"report-header\">");
                sb.AppendLine($"<h1>{Escape(config.Title)}</h1>");
                if (!string.IsNullOrEmpty(config.Description))
                    sb.AppendLine($"<p class=\"subtitle\">{Escape(config.Description)}</p>");
                sb.AppendLine($"<p class=\"timestamp\">Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
                sb.AppendLine("</div>");
            }

            // Sections
            foreach (var section in config.Sections)
            {
                switch (section.Type)
                {
                    case "Header":
                        sb.AppendLine($"<h2 class=\"section-header\">{Escape(section.Title)}</h2>");
                        break;

                    case "Text":
                        if (!string.IsNullOrEmpty(section.Title))
                            sb.AppendLine($"<h3>{Escape(section.Title)}</h3>");
                        sb.AppendLine($"<div class=\"text-section\">{section.Content}</div>");
                        break;

                    case "Chart":
                        await RenderChartSection(sb, section);
                        break;

                    case "Table":
                        await RenderTableSection(sb, section);
                        break;

                    case "Value":
                        RenderValueSection(sb, section);
                        break;

                    case "PageBreak":
                        sb.AppendLine("<div class=\"page-break\"></div>");
                        break;
                }
            }

            sb.AppendLine("<div class=\"report-footer\">");
            sb.AppendLine($"<p>Report: {Escape(config.Name)} | Format: {config.Format} | Page Size: {config.PageSize} {config.Orientation}</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }

        private async Task RenderChartSection(StringBuilder sb, ReportSection section)
        {
            if (!string.IsNullOrEmpty(section.Title))
                sb.AppendLine($"<h3>{Escape(section.Title)}</h3>");

            var chartId = $"chart_{section.Id}";
            var width = Math.Max(200, section.ChartWidth);
            var height = Math.Max(100, section.ChartHeight);

            // Generate SVG chart from historical data
            sb.AppendLine($"<div class=\"chart-container\">");
            sb.AppendLine($"<svg width=\"{width}\" height=\"{height}\" viewBox=\"0 0 {width} {height}\" xmlns=\"http://www.w3.org/2000/svg\" style=\"background: #1e293b; border-radius: 4px;\">");

            // Chart area with padding
            var padL = 60; var padR = 20; var padT = 20; var padB = 40;
            var chartW = width - padL - padR;
            var chartH = height - padT - padB;

            // Grid
            if (section.ChartShowGrid)
            {
                for (int g = 0; g <= 4; g++)
                {
                    var gy = padT + (chartH * g / 4.0);
                    sb.AppendLine($"<line x1=\"{padL}\" y1=\"{gy:F1}\" x2=\"{padL + chartW}\" y2=\"{gy:F1}\" stroke=\"#334155\" stroke-width=\"0.5\" />");
                }
                for (int g = 0; g <= 6; g++)
                {
                    var gx = padL + (chartW * g / 6.0);
                    sb.AppendLine($"<line x1=\"{gx:F1}\" y1=\"{padT}\" x2=\"{gx:F1}\" y2=\"{padT + chartH}\" stroke=\"#334155\" stroke-width=\"0.5\" />");
                }
            }

            // Axes
            sb.AppendLine($"<line x1=\"{padL}\" y1=\"{padT}\" x2=\"{padL}\" y2=\"{padT + chartH}\" stroke=\"#64748b\" stroke-width=\"1\" />");
            sb.AppendLine($"<line x1=\"{padL}\" y1=\"{padT + chartH}\" x2=\"{padL + chartW}\" y2=\"{padT + chartH}\" stroke=\"#64748b\" stroke-width=\"1\" />");

            var colors = new[] { "#38bdf8", "#34d399", "#f97316", "#a78bfa", "#fb7185", "#facc15" };
            var legendItems = new List<(string name, string color)>();
            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddMinutes(-section.ChartTimeRangeMinutes);

            for (int s = 0; s < section.ChartVariablePaths.Count; s++)
            {
                var varPath = section.ChartVariablePaths[s];
                if (string.IsNullOrWhiteSpace(varPath)) continue;

                var color = colors[s % colors.Length];
                var seriesName = varPath.Split('.').LastOrDefault() ?? varPath;
                legendItems.Add((seriesName, color));

                // Read historical data
                var hdaValues = await ReadHistoricalDataAsync(varPath, startTime, endTime, section.ChartMaxPoints);

                if (hdaValues.Count > 1)
                {
                    // Determine Y range
                    var yMin = hdaValues.Min(v => v.Value);
                    var yMax = hdaValues.Max(v => v.Value);
                    if (Math.Abs(yMax - yMin) < 0.001) { yMin -= 1; yMax += 1; }
                    var yRange = yMax - yMin;
                    var tRange = (endTime - startTime).TotalSeconds;

                    if (section.ChartType == "Bar")
                    {
                        var barWidth = Math.Max(2, chartW / (double)hdaValues.Count - 1);
                        for (int di = 0; di < hdaValues.Count; di++)
                        {
                            var t = (hdaValues[di].Timestamp - startTime).TotalSeconds / tRange;
                            var v = (hdaValues[di].Value - yMin) / yRange;
                            var bx = padL + t * chartW;
                            var by = padT + chartH - v * chartH;
                            var bh = v * chartH;
                            sb.AppendLine($"<rect x=\"{bx:F1}\" y=\"{by:F1}\" width=\"{barWidth:F1}\" height=\"{bh:F1}\" fill=\"{color}\" opacity=\"0.7\" />");
                        }
                    }
                    else
                    {
                        // Line or Area
                        var points = new StringBuilder();
                        var areaPoints = new StringBuilder();
                        areaPoints.Append($"{padL},{padT + chartH} ");

                        foreach (var dv in hdaValues)
                        {
                            var t = (dv.Timestamp - startTime).TotalSeconds / tRange;
                            var v = (dv.Value - yMin) / yRange;
                            var px = padL + t * chartW;
                            var py = padT + chartH - v * chartH;
                            points.Append($"{px:F1},{py:F1} ");
                            areaPoints.Append($"{px:F1},{py:F1} ");
                        }
                        areaPoints.Append($"{padL + chartW},{padT + chartH}");

                        if (section.ChartType == "Area")
                        {
                            sb.AppendLine($"<polygon points=\"{areaPoints}\" fill=\"{color}\" opacity=\"0.2\" />");
                        }
                        sb.AppendLine($"<polyline points=\"{points}\" fill=\"none\" stroke=\"{color}\" stroke-width=\"1.5\" />");
                    }

                    // Y-axis labels
                    for (int l = 0; l <= 4; l++)
                    {
                        var lv = yMin + yRange * l / 4.0;
                        var ly = padT + chartH - chartH * l / 4.0;
                        sb.AppendLine($"<text x=\"{padL - 5}\" y=\"{ly:F1}\" text-anchor=\"end\" fill=\"#94a3b8\" font-size=\"9\" dominant-baseline=\"middle\">{lv:F1}</text>");
                    }
                }
                else
                {
                    // No data - show placeholder
                    sb.AppendLine($"<text x=\"{padL + chartW / 2}\" y=\"{padT + chartH / 2}\" text-anchor=\"middle\" fill=\"#64748b\" font-size=\"12\">No historical data for {Escape(varPath)}</text>");
                }
            }

            // Time axis labels
            for (int t = 0; t <= 4; t++)
            {
                var labelTime = startTime.AddSeconds((endTime - startTime).TotalSeconds * t / 4.0);
                var tx = padL + chartW * t / 4.0;
                sb.AppendLine($"<text x=\"{tx:F1}\" y=\"{padT + chartH + 15}\" text-anchor=\"middle\" fill=\"#94a3b8\" font-size=\"9\">{labelTime.ToLocalTime():HH:mm}</text>");
            }

            // Legend
            if (section.ChartShowLegend && legendItems.Count > 0)
            {
                var lx = padL + 10;
                var ly = padT + 5;
                foreach (var (name, color) in legendItems)
                {
                    sb.AppendLine($"<rect x=\"{lx}\" y=\"{ly}\" width=\"10\" height=\"10\" fill=\"{color}\" rx=\"2\" />");
                    sb.AppendLine($"<text x=\"{lx + 14}\" y=\"{ly + 9}\" fill=\"#e2e8f0\" font-size=\"9\">{Escape(name)}</text>");
                    ly += 14;
                }
            }

            sb.AppendLine("</svg>");
            sb.AppendLine("</div>");
        }

        private async Task RenderTableSection(StringBuilder sb, ReportSection section)
        {
            if (!string.IsNullOrEmpty(section.Title))
                sb.AppendLine($"<h3>{Escape(section.Title)}</h3>");

            sb.AppendLine("<table class=\"data-table\">");
            sb.AppendLine("<thead><tr>");
            sb.AppendLine("<th>Variable</th><th>Current Value</th>");
            if (section.TableShowStatistics)
            {
                sb.AppendLine("<th>Min</th><th>Max</th><th>Avg</th>");
            }
            sb.AppendLine("</tr></thead>");
            sb.AppendLine("<tbody>");

            foreach (var varPath in section.TableVariablePaths)
            {
                if (string.IsNullOrWhiteSpace(varPath)) continue;
                var displayName = varPath.Split('.').LastOrDefault() ?? varPath;
                string currentValue; try { currentValue = _nodeManager.ReadVariable(varPath)?.ToString() ?? "N/A"; } catch { currentValue = "N/A"; }

                sb.Append($"<tr><td>{Escape(displayName)}</td><td>{Escape(currentValue)}</td>");
                if (section.TableShowStatistics)
                {
                    var endTime = DateTime.UtcNow;
                    var startTime = endTime.AddMinutes(-section.TableTimeRangeMinutes);
                    var hdaValues = await ReadHistoricalDataAsync(varPath, startTime, endTime, 1000);
                    if (hdaValues.Count > 0)
                    {
                        sb.Append($"<td>{hdaValues.Min(v => v.Value):F2}</td>");
                        sb.Append($"<td>{hdaValues.Max(v => v.Value):F2}</td>");
                        sb.Append($"<td>{hdaValues.Average(v => v.Value):F2}</td>");
                    }
                    else
                    {
                        sb.Append("<td>-</td><td>-</td><td>-</td>");
                    }
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody></table>");
        }

        private void RenderValueSection(StringBuilder sb, ReportSection section)
        {
            string value; try { value = _nodeManager.ReadVariable(section.ValueVariablePath)?.ToString() ?? "N/A"; } catch { value = "N/A"; }
            var label = !string.IsNullOrEmpty(section.ValueLabel) ? section.ValueLabel : section.ValueVariablePath;

            if (!string.IsNullOrEmpty(section.ValueFormat) && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var numVal))
            {
                value = numVal.ToString(section.ValueFormat, CultureInfo.InvariantCulture);
            }

            var unit = !string.IsNullOrEmpty(section.ValueUnit) ? $" {Escape(section.ValueUnit)}" : "";
            sb.AppendLine($"<div class=\"value-section\"><span class=\"value-label\">{Escape(label)}:</span> <span class=\"value-data\">{Escape(value)}{unit}</span></div>");
        }

        private async Task DeliverReportAsync(ReportConfig config, string html)
        {
            var delivery = config.Delivery;

            // Disk delivery
            if (delivery.Method is "Disk" or "Both")
            {
                try
                {
                    var dir = string.IsNullOrWhiteSpace(delivery.DiskPath) ? "Reports" : delivery.DiskPath;
                    if (!Path.IsPathRooted(dir))
                    {
                        var baseDir = Path.GetDirectoryName(AppContext.BaseDirectory) ?? ".";
                        dir = Path.Combine(baseDir, dir);
                    }
                    Directory.CreateDirectory(dir);

                    var fileName = ResolvePattern(delivery.FileNamePattern, config.Name);
                    var ext = config.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase) ? ".pdf" : ".html";
                    var filePath = Path.Combine(dir, fileName + ext);

                    await File.WriteAllTextAsync(filePath, html, _cts.Token);
                    Log.Information("Report '{Name}' saved to {Path}", config.Name, filePath);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to save report '{Name}' to disk.", config.Name);
                }
            }

            // Email delivery
            if (delivery.Method is "Email" or "Both")
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(delivery.SmtpServer) || string.IsNullOrWhiteSpace(delivery.EmailRecipients))
                    {
                        Log.Warning("Report '{Name}': Email delivery skipped — SMTP server or recipients not configured.", config.Name);
                        return;
                    }

                    using var client = new SmtpClient(delivery.SmtpServer, delivery.SmtpPort)
                    {
                        EnableSsl = delivery.SmtpUseSsl,
                        Credentials = !string.IsNullOrWhiteSpace(delivery.SmtpUser)
                            ? new NetworkCredential(delivery.SmtpUser, delivery.SmtpPassword)
                            : null
                    };

                    var from = !string.IsNullOrWhiteSpace(delivery.SmtpFromAddress)
                        ? delivery.SmtpFromAddress
                        : "noreply@hmi-server.local";

                    var subject = delivery.EmailSubject
                        .Replace("{ReportName}", config.Name)
                        .Replace("{DateTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                        .Replace("{Date}", DateTime.Now.ToString("yyyy-MM-dd"));

                    var recipients = delivery.EmailRecipients
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    foreach (var recipient in recipients)
                    {
                        var msg = new MailMessage(from, recipient, subject, html)
                        {
                            IsBodyHtml = true
                        };
                        await client.SendMailAsync(msg, _cts.Token);
                    }

                    Log.Information("Report '{Name}' emailed to {Count} recipient(s).", config.Name, recipients.Length);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to email report '{Name}'.", config.Name);
                }
            }
        }

        private async Task<List<(DateTime Timestamp, double Value)>> ReadHistoricalDataAsync(
            string variablePath, DateTime startTime, DateTime endTime, int maxPoints)
        {
            var result = new List<(DateTime Timestamp, double Value)>();
            try
            {
                // Try reading from the HDA logger if available
                var values = _nodeManager.ReadHistoricalValues(variablePath, startTime, endTime, maxPoints);
                if (values != null)
                    return values;
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to read HDA for '{Path}', using current value.", variablePath);
            }

            // Fallback: read current value as a single data point
            string? current; try { current = _nodeManager.ReadVariable(variablePath)?.ToString(); } catch { current = null; }
            if (current != null && double.TryParse(current, NumberStyles.Any, CultureInfo.InvariantCulture, out var dval))
            {
                result.Add((DateTime.UtcNow, dval));
            }

            await Task.CompletedTask;
            return result;
        }

        private static string ResolvePattern(string pattern, string reportName)
        {
            if (string.IsNullOrWhiteSpace(pattern)) pattern = "{ReportName}_{DateTime}";
            return pattern
                .Replace("{ReportName}", SanitizeFileName(reportName))
                .Replace("{DateTime}", DateTime.Now.ToString("yyyyMMdd_HHmmss"))
                .Replace("{Date}", DateTime.Now.ToString("yyyyMMdd"));
        }

        private static string SanitizeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(name.Length);
            foreach (var c in name)
                sb.Append(invalid.Contains(c) ? '_' : c);
            return sb.ToString();
        }

        private static string Escape(string text) =>
            System.Net.WebUtility.HtmlEncode(text ?? "");

        private static string GetReportCss() => """
            * { margin: 0; padding: 0; box-sizing: border-box; }
            body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #0f172a; color: #e2e8f0; padding: 20px; max-width: 900px; margin: 0 auto; }
            .report-header { text-align: center; margin-bottom: 24px; padding-bottom: 16px; border-bottom: 2px solid #334155; }
            .report-header h1 { font-size: 24px; color: #f1f5f9; margin-bottom: 4px; }
            .report-header .subtitle { font-size: 14px; color: #94a3b8; }
            .report-header .timestamp { font-size: 11px; color: #64748b; margin-top: 4px; }
            h2.section-header { font-size: 18px; color: #38bdf8; margin: 20px 0 8px; padding-bottom: 4px; border-bottom: 1px solid #1e293b; }
            h3 { font-size: 14px; color: #94a3b8; margin: 12px 0 6px; }
            .text-section { font-size: 13px; line-height: 1.6; color: #cbd5e1; margin-bottom: 12px; }
            .chart-container { margin: 12px 0; text-align: center; }
            .data-table { width: 100%; border-collapse: collapse; margin: 12px 0; font-size: 12px; }
            .data-table th { background: #1e293b; color: #38bdf8; padding: 8px 12px; text-align: left; border: 1px solid #334155; }
            .data-table td { padding: 6px 12px; border: 1px solid #334155; color: #e2e8f0; }
            .data-table tr:nth-child(even) { background: #1e293b; }
            .value-section { margin: 8px 0; padding: 8px 12px; background: #1e293b; border-radius: 4px; border-left: 3px solid #38bdf8; font-size: 13px; }
            .value-label { color: #94a3b8; }
            .value-data { color: #f1f5f9; font-weight: 600; font-size: 16px; }
            .report-footer { margin-top: 24px; padding-top: 12px; border-top: 1px solid #334155; font-size: 10px; color: #475569; text-align: center; }
            .page-break { page-break-after: always; margin: 20px 0; border-top: 2px dashed #334155; }
            @media print { body { background: white; color: black; } .data-table th { background: #f0f0f0; color: #333; } }
            """;

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
