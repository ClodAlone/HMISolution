using System.Net;
using System.Text;
using SharedModels;

namespace ServerEditorWeb.Services;

/// <summary>
/// Options controlling which sections appear in the generated project documentation.
/// </summary>
public class DocOptions
{
    public bool IncludeVariables  { get; set; } = true;
    public bool IncludeAlarms     { get; set; } = true;
    public bool IncludeScreens    { get; set; } = true;
    public bool IncludeScripts    { get; set; } = true;
    public bool IncludePlcPrograms { get; set; } = true;
    public bool IncludeUsers      { get; set; } = true;
}

/// <summary>
/// Generates a self-contained HTML project documentation document from a <see cref="NodeModel"/>.
/// No server connection is required — all data is read directly from the in-memory model.
/// </summary>
public static class ProjectDocumentationService
{
    // ── Public entry point ─────────────────────────────────────────────────

    public static string GenerateHtml(NodeModel model, string projectName, DocOptions options)
    {
        var sb = new StringBuilder(64 * 1024);
        var now = DateTime.Now;

        // Collect flat variable list once (used by both variables and alarms sections)
        var allVars = new List<(string Path, Variable Var)>();
        CollectVariables(model.Folder, "", allVars);

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"UTF-8\" />");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />");
        sb.AppendLine($"<title>{E(projectName)} — Project Documentation</title>");
        sb.AppendLine("<style>");
        sb.AppendLine(GetCss());
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        // ── Cover page ──────────────────────────────────────────────────────
        sb.AppendLine("<div class=\"cover\">");
        sb.AppendLine("  <div class=\"cover-badge\">AI Core HMI</div>");
        sb.AppendLine($" <h1>{E(projectName)}</h1>");
        sb.AppendLine("  <p class=\"cover-sub\">Project Documentation</p>");
        sb.AppendLine($" <p class=\"cover-date\">Generated: {now:yyyy-MM-dd HH:mm:ss}</p>");
        sb.AppendLine("</div>");

        // ── Table of contents ───────────────────────────────────────────────
        sb.AppendLine("<div class=\"toc\">");
        sb.AppendLine("  <h2>Contents</h2>");
        sb.AppendLine("  <ol>");
        if (options.IncludeVariables)   sb.AppendLine("    <li><a href=\"#variables\">Variables</a></li>");
        if (options.IncludeAlarms)      sb.AppendLine("    <li><a href=\"#alarms\">Alarms</a></li>");
        if (options.IncludeScreens)     sb.AppendLine("    <li><a href=\"#screens\">Screens</a></li>");
        if (options.IncludeScripts)     sb.AppendLine("    <li><a href=\"#scripts\">Scripts</a></li>");
        if (options.IncludePlcPrograms) sb.AppendLine("    <li><a href=\"#plc\">PLC Programs</a></li>");
        if (options.IncludeUsers)       sb.AppendLine("    <li><a href=\"#users\">Users &amp; Groups</a></li>");
        sb.AppendLine("  </ol>");
        sb.AppendLine("</div>");

        // ── Variables ───────────────────────────────────────────────────────
        if (options.IncludeVariables)
        {
            sb.AppendLine("<section id=\"variables\">");
            sb.AppendLine("  <h2>Variables</h2>");
            sb.AppendLine($"  <p class=\"section-meta\">Total: {allVars.Count} tag(s)</p>");
            if (allVars.Count == 0)
            {
                sb.AppendLine("  <p class=\"empty\">No variables defined.</p>");
            }
            else
            {
                sb.AppendLine("  <table>");
                sb.AppendLine("    <thead><tr>");
                sb.AppendLine("      <th>Path</th><th>Type</th><th>Access</th><th>Engineering Unit</th>");
                sb.AppendLine("      <th>Description</th><th>Driver Config</th><th>Retentive</th>");
                sb.AppendLine("    </tr></thead>");
                sb.AppendLine("    <tbody>");
                foreach (var (path, v) in allVars)
                {
                    var driverStr = BuildDriverString(v);
                    sb.AppendLine("      <tr>");
                    sb.AppendLine($"        <td class=\"mono\">{E(path)}</td>");
                    sb.AppendLine($"        <td>{E(v.Type)}</td>");
                    sb.AppendLine($"        <td>{E(v.Access)}</td>");
                    sb.AppendLine($"        <td>{E(v.EngineeringUnit)}</td>");
                    sb.AppendLine($"        <td>{E(v.Description)}</td>");
                    sb.AppendLine($"        <td class=\"mono small\">{E(driverStr)}</td>");
                    sb.AppendLine($"        <td>{(v.Retentive ? "✓" : "")}</td>");
                    sb.AppendLine("      </tr>");
                }
                sb.AppendLine("    </tbody>");
                sb.AppendLine("  </table>");
            }
            sb.AppendLine("</section>");
            sb.AppendLine("<div class=\"page-break\"></div>");
        }

        // ── Alarms ──────────────────────────────────────────────────────────
        if (options.IncludeAlarms)
        {
            var alarmedVars = allVars.Where(x => x.Var.Alarm != null).ToList();
            sb.AppendLine("<section id=\"alarms\">");
            sb.AppendLine("  <h2>Alarms</h2>");
            sb.AppendLine($"  <p class=\"section-meta\">Total: {alarmedVars.Count} alarm(s)</p>");
            if (alarmedVars.Count == 0)
            {
                sb.AppendLine("  <p class=\"empty\">No alarms configured.</p>");
            }
            else
            {
                sb.AppendLine("  <table>");
                sb.AppendLine("    <thead><tr>");
                sb.AppendLine("      <th>Variable</th><th>Trigger Type</th><th>High-High</th><th>High</th>");
                sb.AppendLine("      <th>Low</th><th>Low-Low</th><th>Message</th><th>Notify</th>");
                sb.AppendLine("    </tr></thead>");
                sb.AppendLine("    <tbody>");
                foreach (var (path, v) in alarmedVars)
                {
                    var a = v.Alarm!;
                    sb.AppendLine("      <tr>");
                    sb.AppendLine($"        <td class=\"mono\">{E(path)}</td>");
                    sb.AppendLine($"        <td>{E(a.TriggerType.ToString())}</td>");
                    sb.AppendLine($"        <td>{FormatLimit(a.HighHighLimit)}</td>");
                    sb.AppendLine($"        <td>{FormatLimit(a.HighLimit)}</td>");
                    sb.AppendLine($"        <td>{FormatLimit(a.LowLimit)}</td>");
                    sb.AppendLine($"        <td>{FormatLimit(a.LowLowLimit)}</td>");
                    sb.AppendLine($"        <td>{E(a.Message)}</td>");
                    sb.AppendLine($"        <td>{(a.NotifyOnActivation ? "✓" : "")}</td>");
                    sb.AppendLine("      </tr>");
                }
                sb.AppendLine("    </tbody>");
                sb.AppendLine("  </table>");
            }
            sb.AppendLine("</section>");
            sb.AppendLine("<div class=\"page-break\"></div>");
        }

        // ── Screens ─────────────────────────────────────────────────────────
        if (options.IncludeScreens)
        {
            sb.AppendLine("<section id=\"screens\">");
            sb.AppendLine("  <h2>Screens</h2>");
            sb.AppendLine($"  <p class=\"section-meta\">Total: {model.Screens.Count} screen(s)</p>");
            if (model.Screens.Count == 0)
            {
                sb.AppendLine("  <p class=\"empty\">No screens defined.</p>");
            }
            else
            {
                sb.AppendLine("  <table>");
                sb.AppendLine("    <thead><tr>");
                sb.AppendLine("      <th>Name</th><th>Size</th><th>Group</th><th>Layout</th>");
                sb.AppendLine("      <th>Symbols</th><th>Show in Nav</th><th>Allowed Groups</th>");
                sb.AppendLine("    </tr></thead>");
                sb.AppendLine("    <tbody>");
                foreach (var s in model.Screens)
                {
                    var groups = s.AllowedGroups.Count > 0 ? string.Join(", ", s.AllowedGroups) : "All";
                    sb.AppendLine("      <tr>");
                    sb.AppendLine($"        <td>{E(s.Name)}</td>");
                    sb.AppendLine($"        <td>{s.Width} × {s.Height}</td>");
                    sb.AppendLine($"        <td>{E(s.Group)}</td>");
                    sb.AppendLine($"        <td>{E(s.LayoutMode)}</td>");
                    sb.AppendLine($"        <td>{s.Symbols.Count}</td>");
                    sb.AppendLine($"        <td>{(s.ShowInNavigation ? "✓" : "")}</td>");
                    sb.AppendLine($"        <td>{E(groups)}</td>");
                    sb.AppendLine("      </tr>");
                }
                sb.AppendLine("    </tbody>");
                sb.AppendLine("  </table>");
            }
            sb.AppendLine("</section>");
            sb.AppendLine("<div class=\"page-break\"></div>");
        }

        // ── Scripts ─────────────────────────────────────────────────────────
        if (options.IncludeScripts)
        {
            sb.AppendLine("<section id=\"scripts\">");
            sb.AppendLine("  <h2>Scripts</h2>");
            sb.AppendLine($"  <p class=\"section-meta\">Total: {model.Scripts.Count} script(s)</p>");
            if (model.Scripts.Count == 0)
            {
                sb.AppendLine("  <p class=\"empty\">No scripts defined.</p>");
            }
            else
            {
                sb.AppendLine("  <table>");
                sb.AppendLine("    <thead><tr>");
                sb.AppendLine("      <th>Name</th><th>Language</th><th>Interval (ms)</th><th>Enabled</th><th>Group</th><th>Signature / First Line</th>");
                sb.AppendLine("    </tr></thead>");
                sb.AppendLine("    <tbody>");
                foreach (var sc in model.Scripts)
                {
                    var sig = ExtractSignature(sc.Code);
                    sb.AppendLine("      <tr>");
                    sb.AppendLine($"        <td>{E(sc.Name)}</td>");
                    sb.AppendLine($"        <td>{E(sc.Language)}</td>");
                    sb.AppendLine($"        <td>{sc.IntervalMs}</td>");
                    sb.AppendLine($"        <td>{(sc.Enabled ? "✓" : "—")}</td>");
                    sb.AppendLine($"        <td>{E(sc.Group)}</td>");
                    sb.AppendLine($"        <td class=\"mono small\">{E(sig)}</td>");
                    sb.AppendLine("      </tr>");
                }
                sb.AppendLine("    </tbody>");
                sb.AppendLine("  </table>");
            }
            sb.AppendLine("</section>");
            sb.AppendLine("<div class=\"page-break\"></div>");
        }

        // ── PLC Programs ────────────────────────────────────────────────────
        if (options.IncludePlcPrograms)
        {
            sb.AppendLine("<section id=\"plc\">");
            sb.AppendLine("  <h2>PLC Programs</h2>");
            sb.AppendLine($"  <p class=\"section-meta\">Total: {model.PlcPrograms.Count} program(s)</p>");
            if (model.PlcPrograms.Count == 0)
            {
                sb.AppendLine("  <p class=\"empty\">No PLC programs defined.</p>");
            }
            else
            {
                sb.AppendLine("  <table>");
                sb.AppendLine("    <thead><tr>");
                sb.AppendLine("      <th>Name</th><th>Language</th><th>Interval (ms)</th><th>Enabled</th><th>Group</th><th>Lines of Code</th>");
                sb.AppendLine("    </tr></thead>");
                sb.AppendLine("    <tbody>");
                foreach (var p in model.PlcPrograms)
                {
                    var loc = string.IsNullOrWhiteSpace(p.Code) ? 0
                        : p.Code.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
                    sb.AppendLine("      <tr>");
                    sb.AppendLine($"        <td>{E(p.Name)}</td>");
                    sb.AppendLine($"        <td>{E(p.Language)}</td>");
                    sb.AppendLine($"        <td>{p.IntervalMs}</td>");
                    sb.AppendLine($"        <td>{(p.Enabled ? "✓" : "—")}</td>");
                    sb.AppendLine($"        <td>{E(p.Group)}</td>");
                    sb.AppendLine($"        <td>{loc}</td>");
                    sb.AppendLine("      </tr>");
                }
                sb.AppendLine("    </tbody>");
                sb.AppendLine("  </table>");
            }
            sb.AppendLine("</section>");
            sb.AppendLine("<div class=\"page-break\"></div>");
        }

        // ── Users & Groups ──────────────────────────────────────────────────
        if (options.IncludeUsers)
        {
            sb.AppendLine("<section id=\"users\">");
            sb.AppendLine("  <h2>Users &amp; Groups</h2>");

            // Groups sub-table
            sb.AppendLine("  <h3>User Groups</h3>");
            if (model.UserGroups.Count == 0)
            {
                sb.AppendLine("  <p class=\"empty\">No user groups defined.</p>");
            }
            else
            {
                sb.AppendLine("  <table>");
                sb.AppendLine("    <thead><tr><th>Name</th><th>Access Level</th><th>Can Access Editor</th><th>Can Access Runtime</th></tr></thead>");
                sb.AppendLine("    <tbody>");
                foreach (var g in model.UserGroups)
                {
                    sb.AppendLine("      <tr>");
                    sb.AppendLine($"        <td>{E(g.Name)}</td>");
                    sb.AppendLine($"        <td>{E(g.AccessLevel)}</td>");
                    sb.AppendLine($"        <td>{(g.CanAccessEditor ? "✓" : "")}</td>");
                    sb.AppendLine($"        <td>{(g.CanAccessRuntime ? "✓" : "")}</td>");
                    sb.AppendLine("      </tr>");
                }
                sb.AppendLine("    </tbody>");
                sb.AppendLine("  </table>");
            }

            // Users sub-table
            sb.AppendLine("  <h3 style=\"margin-top:16px\">Users</h3>");
            if (model.Users.Count == 0)
            {
                sb.AppendLine("  <p class=\"empty\">No users defined.</p>");
            }
            else
            {
                sb.AppendLine("  <table>");
                sb.AppendLine("    <thead><tr><th>Username</th><th>Full Name</th><th>Group</th><th>Auto Log-off (s)</th><th>Password Expiry (days)</th></tr></thead>");
                sb.AppendLine("    <tbody>");
                foreach (var u in model.Users)
                {
                    sb.AppendLine("      <tr>");
                    sb.AppendLine($"        <td>{E(u.Username)}</td>");
                    sb.AppendLine($"        <td>{E(u.FullName)}</td>");
                    sb.AppendLine($"        <td>{E(u.Group)}</td>");
                    sb.AppendLine($"        <td>{(u.AutoLogOffSeconds > 0 ? u.AutoLogOffSeconds.ToString() : "—")}</td>");
                    sb.AppendLine($"        <td>{(u.PasswordExpiryDays > 0 ? u.PasswordExpiryDays.ToString() : "Never")}</td>");
                    sb.AppendLine("      </tr>");
                }
                sb.AppendLine("    </tbody>");
                sb.AppendLine("  </table>");
            }

            sb.AppendLine("</section>");
        }

        // ── Footer ──────────────────────────────────────────────────────────
        sb.AppendLine("<footer>");
        sb.AppendLine($"  <p>Project: {E(projectName)} &nbsp;|&nbsp; Generated by AI Core HMI Editor &nbsp;|&nbsp; {now:yyyy-MM-dd HH:mm:ss}</p>");
        sb.AppendLine("</footer>");

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static void CollectVariables(Folder folder, string prefix, List<(string, Variable)> result)
    {
        CollectVariablesPublic(folder, prefix, result);
    }

    /// <summary>Public overload used by <see cref="ProjectDocumentationPanel"/> for live counts.</summary>
    public static void CollectVariablesPublic(Folder folder, string prefix, List<(string, Variable)> result)
    {
        var folderPath = string.IsNullOrEmpty(prefix)
            ? folder.Name
            : string.IsNullOrEmpty(folder.Name) ? prefix : $"{prefix}.{folder.Name}";

        foreach (var v in folder.Variables)
            result.Add((string.IsNullOrEmpty(folderPath) ? v.Name : $"{folderPath}.{v.Name}", v));

        foreach (var sub in folder.Folders)
            CollectVariables(sub, folderPath, result);
    }

    private static string BuildDriverString(Variable v)
    {
        if (v.DriverConfigs == null || v.DriverConfigs.Count == 0) return "";
        var parts = v.DriverConfigs.Select(kv => $"{kv.Key}: {kv.Value}");
        return string.Join(" | ", parts);
    }

    private static string FormatLimit(double? value) =>
        value.HasValue ? value.Value.ToString("G6") : "";

    private static string FormatLimit(double value) => value.ToString("G6");

    private static string ExtractSignature(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return "";
        var line = code.Split('\n').FirstOrDefault(l => !string.IsNullOrWhiteSpace(l))?.Trim() ?? "";
        return line.Length > 120 ? line[..120] + "…" : line;
    }

    private static string E(string? text) => WebUtility.HtmlEncode(text ?? "");

    // ── Embedded CSS ───────────────────────────────────────────────────────

    private static string GetCss() => """
        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: #0f172a;
            color: #e2e8f0;
            padding: 0;
            font-size: 13px;
            line-height: 1.5;
        }
        a { color: #38bdf8; text-decoration: none; }
        a:hover { text-decoration: underline; }

        /* Cover */
        .cover {
            background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
            min-height: 280px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            text-align: center;
            padding: 48px 24px;
            border-bottom: 3px solid #38bdf8;
        }
        .cover-badge {
            background: #38bdf8;
            color: #0f172a;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 2px;
            padding: 4px 12px;
            border-radius: 20px;
            margin-bottom: 16px;
        }
        .cover h1 {
            font-size: 32px;
            font-weight: 700;
            color: #f1f5f9;
            margin-bottom: 8px;
        }
        .cover-sub { font-size: 16px; color: #94a3b8; margin-bottom: 12px; }
        .cover-date { font-size: 12px; color: #475569; }

        /* Table of Contents */
        .toc {
            max-width: 900px;
            margin: 32px auto;
            padding: 20px 24px;
            background: #1e293b;
            border-radius: 8px;
            border: 1px solid #334155;
        }
        .toc h2 { font-size: 16px; color: #38bdf8; margin-bottom: 12px; }
        .toc ol { padding-left: 20px; }
        .toc li { padding: 3px 0; font-size: 13px; }

        /* Sections */
        section {
            max-width: 900px;
            margin: 32px auto;
            padding: 0 24px;
        }
        section h2 {
            font-size: 20px;
            color: #38bdf8;
            margin-bottom: 6px;
            padding-bottom: 6px;
            border-bottom: 2px solid #1e293b;
        }
        section h3 {
            font-size: 14px;
            color: #94a3b8;
            margin-bottom: 8px;
        }
        .section-meta { font-size: 11px; color: #64748b; margin-bottom: 12px; }
        .empty { color: #64748b; font-style: italic; margin: 8px 0; }

        /* Tables */
        table {
            width: 100%;
            border-collapse: collapse;
            font-size: 12px;
            margin-bottom: 12px;
        }
        thead th {
            background: #1e293b;
            color: #38bdf8;
            padding: 8px 10px;
            text-align: left;
            border: 1px solid #334155;
            font-weight: 600;
            font-size: 11px;
            text-transform: uppercase;
            letter-spacing: .5px;
            white-space: nowrap;
        }
        tbody td {
            padding: 6px 10px;
            border: 1px solid #1e293b;
            color: #e2e8f0;
            vertical-align: top;
        }
        tbody tr:nth-child(even) td { background: #1e293b; }
        tbody tr:hover td { background: #243347; }
        .mono { font-family: 'Cascadia Code', 'Consolas', monospace; }
        .small { font-size: 10px; color: #94a3b8; }

        /* Footer */
        footer {
            max-width: 900px;
            margin: 40px auto 24px;
            padding: 12px 24px;
            border-top: 1px solid #334155;
            font-size: 10px;
            color: #475569;
            text-align: center;
        }

        /* Page break (print) */
        .page-break { page-break-after: always; height: 0; }

        @media print {
            body { background: #fff; color: #111; }
            .cover { background: #f0f4f8; border-bottom-color: #2563eb; }
            .cover h1, section h2 { color: #1d4ed8; }
            thead th { background: #e2e8f0; color: #1e3a5f; border-color: #cbd5e1; }
            tbody td { border-color: #e2e8f0; color: #111; }
            tbody tr:nth-child(even) td { background: #f8fafc; }
            tbody tr:hover td { background: #eff6ff; }
            .toc { background: #f8fafc; border-color: #e2e8f0; }
            .toc h2 { color: #1d4ed8; }
            footer { color: #64748b; }
            .page-break { page-break-after: always; }
        }
        """;
}
