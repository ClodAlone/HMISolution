using SharedModels;
using System.Text.RegularExpressions;

namespace ServerEditorWeb.Services;

/// <summary>
/// Severity level for a project validation issue.
/// </summary>
public enum IssueSeverity
{
    Error,
    Warning,
    Info
}

/// <summary>
/// A single validation issue found in the project.
/// </summary>
public record ProjectIssue(
    IssueSeverity Severity,
    string Category,
    string Message,
    string Location,
    string? NavigationTarget = null);

/// <summary>
/// Scans the entire NodeModel for configuration errors, broken bindings,
/// script/PLC syntax issues, and other problems.
/// </summary>
public class ProjectValidationService
{
    private readonly SyntaxCheckService _syntaxCheck;
    private readonly CrossReferenceService _xref;

    public ProjectValidationService(SyntaxCheckService syntaxCheck, CrossReferenceService xref)
    {
        _syntaxCheck = syntaxCheck;
        _xref = xref;
    }

    /// <summary>
    /// Run a full project validation and return all issues found.
    /// </summary>
    public List<ProjectIssue> Validate(NodeModel model)
    {
        if (model == null)
            return [new ProjectIssue(IssueSeverity.Info, "General", "No project loaded.", "")];

        var issues = new List<ProjectIssue>();
        var allVarPaths = new HashSet<string>(_xref.GetAllVariablePaths(model), StringComparer.OrdinalIgnoreCase);

        ValidateScreens(model, allVarPaths, issues);
        ValidateScripts(model, issues);
        ValidatePlcPrograms(model, issues);
        ValidateAlarms(model, allVarPaths, issues);
        ValidateDataLogging(model, allVarPaths, issues);
        ValidateRecipes(model, allVarPaths, issues);
        ValidateReports(model, allVarPaths, issues);
        ValidateSchedulers(model, issues);
        ValidateUsers(model, issues);
        ValidateServer(model, issues);

        return issues;
    }

    // ── Screens ──────────────────────────────────────────────

    private static void ValidateScreens(NodeModel model, HashSet<string> vars, List<ProjectIssue> issues)
    {
        if (model.Screens.Count == 0)
        {
            issues.Add(new(IssueSeverity.Info, "Screen", "No screens defined.", "Project"));
            return;
        }

        var screenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var screen in model.Screens)
        {
            var loc = $"Screen '{screen.Name}'";

            // Duplicate screen name
            if (!screenNames.Add(screen.Name))
                issues.Add(new(IssueSeverity.Error, "Screen", $"Duplicate screen name '{screen.Name}'.", loc));

            // Empty screen
            if (screen.Symbols.Count == 0)
                issues.Add(new(IssueSeverity.Info, "Screen", "Screen has no symbols.", loc, $"screen:{screen.Name}"));

            var symIds = new HashSet<string>();
            foreach (var sym in screen.Symbols)
            {
                var symLoc = $"{loc} → Symbol '{sym.Type}' ({sym.Id})";

                // Duplicate symbol ID
                if (!string.IsNullOrEmpty(sym.Id) && !symIds.Add(sym.Id))
                    issues.Add(new(IssueSeverity.Error, "Screen", $"Duplicate symbol ID '{sym.Id}'.", symLoc, $"screen:{screen.Name}"));

                // Broken VariablePath binding
                if (!string.IsNullOrEmpty(sym.VariablePath) && !vars.Contains(sym.VariablePath))
                    issues.Add(new(IssueSeverity.Error, "Binding", $"Variable '{sym.VariablePath}' does not exist.", symLoc, $"screen:{screen.Name}"));

                // Broken HDA variable paths
                foreach (var hp in sym.HdaVariablePaths)
                {
                    if (!string.IsNullOrEmpty(hp) && !vars.Contains(hp))
                        issues.Add(new(IssueSeverity.Error, "Binding", $"HDA variable '{hp}' does not exist.", symLoc, $"screen:{screen.Name}"));
                }

                // Broken trend pen variable paths
                foreach (var pen in sym.TrendPens)
                {
                    if (!string.IsNullOrEmpty(pen.VariablePath) && !vars.Contains(pen.VariablePath))
                        issues.Add(new(IssueSeverity.Error, "Binding", $"Trend pen variable '{pen.VariablePath}' does not exist.", symLoc, $"screen:{screen.Name}"));
                }

                // Animation trigger variables
                foreach (var anim in sym.Animations)
                {
                    if (!string.IsNullOrEmpty(anim.TriggerVariable) && !vars.Contains(anim.TriggerVariable))
                        issues.Add(new(IssueSeverity.Warning, "Binding", $"Animation trigger variable '{anim.TriggerVariable}' does not exist.", symLoc, $"screen:{screen.Name}"));
                }

                // Command variable paths
                foreach (var cmd in sym.Commands)
                {
                    if (!string.IsNullOrEmpty(cmd.VariablePath) && !vars.Contains(cmd.VariablePath))
                        issues.Add(new(IssueSeverity.Warning, "Binding", $"Command target variable '{cmd.VariablePath}' does not exist.", symLoc, $"screen:{screen.Name}"));
                }

                // Embedded screens must exist
                foreach (var es in sym.EmbeddedScreens)
                {
                    if (!string.IsNullOrEmpty(es) && !model.Screens.Any(s => s.Name.Equals(es, StringComparison.OrdinalIgnoreCase)))
                        issues.Add(new(IssueSeverity.Error, "Screen", $"Embedded screen '{es}' does not exist.", symLoc, $"screen:{screen.Name}"));
                }

                // Recipe widget references
                if (sym.Type == "recipe" && !string.IsNullOrEmpty(sym.RecipeName)
                    && !model.Recipes.Any(r => r.Name.Equals(sym.RecipeName, StringComparison.OrdinalIgnoreCase)))
                    issues.Add(new(IssueSeverity.Error, "Screen", $"Recipe '{sym.RecipeName}' does not exist.", symLoc, $"screen:{screen.Name}"));

                // Scheduler widget references
                if (sym.Type == "weeklyplanner" && !string.IsNullOrEmpty(sym.SchedulerName)
                    && !model.Schedulers.Any(s => s.Name.Equals(sym.SchedulerName, StringComparison.OrdinalIgnoreCase)))
                    issues.Add(new(IssueSeverity.Error, "Screen", $"Scheduler '{sym.SchedulerName}' does not exist.", symLoc, $"screen:{screen.Name}"));

                // Report viewer widget references
                if (sym.Type == "reportviewer" && !string.IsNullOrEmpty(sym.ReportName)
                    && !model.Reports.Any(r => r.Name.Equals(sym.ReportName, StringComparison.OrdinalIgnoreCase)))
                    issues.Add(new(IssueSeverity.Error, "Screen", $"Report '{sym.ReportName}' does not exist.", symLoc, $"screen:{screen.Name}"));

                // Gauge min/max sanity
                if (sym.Type == "gauge" && sym.MinValue.HasValue && sym.MaxValue.HasValue && sym.MinValue >= sym.MaxValue)
                    issues.Add(new(IssueSeverity.Warning, "Screen", "Gauge MinValue >= MaxValue.", symLoc, $"screen:{screen.Name}"));
            }
        }

        // Startup screen check
        if (!string.IsNullOrEmpty(model.Server?.StartupScreen)
            && !model.Screens.Any(s => s.Name.Equals(model.Server.StartupScreen, StringComparison.OrdinalIgnoreCase)))
            issues.Add(new(IssueSeverity.Error, "Server", $"Startup screen '{model.Server.StartupScreen}' does not exist.", "Server Settings"));
    }

    // ── Scripts ──────────────────────────────────────────────

    private void ValidateScripts(NodeModel model, List<ProjectIssue> issues)
    {
        var scriptNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var script in model.Scripts)
        {
            var loc = $"Script '{script.Name}'";

            if (!scriptNames.Add(script.Name))
                issues.Add(new(IssueSeverity.Error, "Script", $"Duplicate script name '{script.Name}'.", loc));

            if (string.IsNullOrWhiteSpace(script.Code))
            {
                issues.Add(new(IssueSeverity.Warning, "Script", "Script has no code.", loc));
                continue;
            }

            if (script.IntervalMs <= 0)
                issues.Add(new(IssueSeverity.Warning, "Script", $"Invalid interval {script.IntervalMs}ms.", loc));

            var (success, msg) = _syntaxCheck.CheckSyntax(script.Code, script.Language);
            if (!success)
            {
                // Extract first few errors
                var lines = msg.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines.Take(5))
                    issues.Add(new(IssueSeverity.Error, "Script", line.Trim(), loc));
                if (lines.Length > 5)
                    issues.Add(new(IssueSeverity.Error, "Script", $"...and {lines.Length - 5} more errors.", loc));
            }
        }
    }

    // ── PLC Programs ─────────────────────────────────────────

    private void ValidatePlcPrograms(NodeModel model, List<ProjectIssue> issues)
    {
        var plcNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var plc in model.PlcPrograms)
        {
            var loc = $"PLC '{plc.Name}' ({plc.Language})";

            if (!plcNames.Add(plc.Name))
                issues.Add(new(IssueSeverity.Error, "PLC", $"Duplicate PLC program name '{plc.Name}'.", loc));

            if (string.IsNullOrWhiteSpace(plc.Code))
            {
                issues.Add(new(IssueSeverity.Warning, "PLC", "PLC program has no code.", loc));
                continue;
            }

            if (plc.IntervalMs <= 0)
                issues.Add(new(IssueSeverity.Warning, "PLC", $"Invalid interval {plc.IntervalMs}ms.", loc));

            var (success, msg) = _syntaxCheck.CheckPlcSyntax(plc.Code, plc.Language);
            if (!success)
                issues.Add(new(IssueSeverity.Error, "PLC", msg.Trim(), loc));
        }
    }

    // ── Alarms ───────────────────────────────────────────────

    private static void ValidateAlarms(NodeModel model, HashSet<string> vars, List<ProjectIssue> issues)
    {
        if (model.Folder != null)
            ValidateAlarmsInFolder(model.Folder, "", vars, issues);
    }

    private static void ValidateAlarmsInFolder(Folder folder, string parentPath, HashSet<string> vars, List<ProjectIssue> issues)
    {
        var folderPath = string.IsNullOrEmpty(parentPath) ? folder.Name : $"{parentPath}.{folder.Name}";
        foreach (var v in folder.Variables)
        {
            if (v.Alarm == null) continue;
            var varPath = $"{folderPath}.{v.Name}";
            var loc = $"Alarm on '{varPath}'";

            if (v.Alarm.TriggerType == AlarmTriggerType.Limit)
            {
                if (v.Alarm.HighLimit <= v.Alarm.LowLimit)
                    issues.Add(new(IssueSeverity.Warning, "Alarm", "HighLimit <= LowLimit.", loc));

                if (v.Alarm.HighHighLimit.HasValue && v.Alarm.HighHighLimit <= v.Alarm.HighLimit)
                    issues.Add(new(IssueSeverity.Warning, "Alarm", "HighHighLimit <= HighLimit.", loc));

                if (v.Alarm.LowLowLimit.HasValue && v.Alarm.LowLowLimit >= v.Alarm.LowLimit)
                    issues.Add(new(IssueSeverity.Warning, "Alarm", "LowLowLimit >= LowLimit.", loc));
            }

            if (string.IsNullOrWhiteSpace(v.Alarm.Message))
                issues.Add(new(IssueSeverity.Info, "Alarm", "Alarm has no message text.", loc));
        }
        foreach (var sub in folder.Folders)
            ValidateAlarmsInFolder(sub, folderPath, vars, issues);
    }

    // ── Data Logging ─────────────────────────────────────────

    private static void ValidateDataLogging(NodeModel model, HashSet<string> vars, List<ProjectIssue> issues)
    {
        if (model.Folder != null)
            ValidateLoggingInFolder(model.Folder, "", issues);
    }

    private static void ValidateLoggingInFolder(Folder folder, string parentPath, List<ProjectIssue> issues)
    {
        var folderPath = string.IsNullOrEmpty(parentPath) ? folder.Name : $"{parentPath}.{folder.Name}";
        foreach (var v in folder.Variables)
        {
            if (v.DataLogging is not { Enabled: true }) continue;
            var varPath = $"{folderPath}.{v.Name}";

            if (v.DataLogging.Hysteresis < 0)
                issues.Add(new(IssueSeverity.Warning, "Data Logging", "Negative hysteresis value.", $"DataLog on '{varPath}'"));
        }
        foreach (var sub in folder.Folders)
            ValidateLoggingInFolder(sub, folderPath, issues);
    }

    // ── Recipes ──────────────────────────────────────────────

    private static void ValidateRecipes(NodeModel model, HashSet<string> vars, List<ProjectIssue> issues)
    {
        var recipeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var recipe in model.Recipes)
        {
            var loc = $"Recipe '{recipe.Name}'";

            if (!recipeNames.Add(recipe.Name))
                issues.Add(new(IssueSeverity.Error, "Recipe", $"Duplicate recipe name '{recipe.Name}'.", loc));

            if (recipe.Variables.Count == 0)
                issues.Add(new(IssueSeverity.Warning, "Recipe", "Recipe has no variables.", loc));

            foreach (var rv in recipe.Variables)
            {
                if (!string.IsNullOrEmpty(rv.VariablePath) && !vars.Contains(rv.VariablePath))
                    issues.Add(new(IssueSeverity.Error, "Recipe", $"Variable '{rv.VariablePath}' does not exist.", loc));
            }

            // Duplicate indexes
            var indexes = recipe.Variables.GroupBy(v => v.Index).Where(g => g.Count() > 1);
            foreach (var dup in indexes)
                issues.Add(new(IssueSeverity.Error, "Recipe", $"Duplicate variable index {dup.Key}.", loc));
        }
    }

    // ── Reports ──────────────────────────────────────────────

    private static void ValidateReports(NodeModel model, HashSet<string> vars, List<ProjectIssue> issues)
    {
        var reportNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var report in model.Reports)
        {
            var loc = $"Report '{report.Name}'";

            if (!reportNames.Add(report.Name))
                issues.Add(new(IssueSeverity.Error, "Report", $"Duplicate report name '{report.Name}'.", loc));

            if (report.Sections.Count == 0)
                issues.Add(new(IssueSeverity.Warning, "Report", "Report has no sections.", loc));

            foreach (var section in report.Sections)
            {
                foreach (var vp in section.ChartVariablePaths)
                {
                    if (!string.IsNullOrEmpty(vp) && !vars.Contains(vp))
                        issues.Add(new(IssueSeverity.Warning, "Report", $"Chart variable '{vp}' does not exist.", $"{loc} → Section '{section.Title}'"));
                }
                foreach (var vp in section.TableVariablePaths)
                {
                    if (!string.IsNullOrEmpty(vp) && !vars.Contains(vp))
                        issues.Add(new(IssueSeverity.Warning, "Report", $"Table variable '{vp}' does not exist.", $"{loc} → Section '{section.Title}'"));
                }
                if (!string.IsNullOrEmpty(section.ValueVariablePath) && !vars.Contains(section.ValueVariablePath))
                    issues.Add(new(IssueSeverity.Warning, "Report", $"Value variable '{section.ValueVariablePath}' does not exist.", $"{loc} → Section '{section.Title}'"));
            }
        }
    }

    // ── Schedulers ───────────────────────────────────────────

    private static void ValidateSchedulers(NodeModel model, List<ProjectIssue> issues)
    {
        var schedulerNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var sched in model.Schedulers)
        {
            if (!schedulerNames.Add(sched.Name))
                issues.Add(new(IssueSeverity.Error, "Scheduler", $"Duplicate scheduler name '{sched.Name}'.", $"Scheduler '{sched.Name}'"));
        }
    }

    // ── Users ────────────────────────────────────────────────

    private static void ValidateUsers(NodeModel model, List<ProjectIssue> issues)
    {
        var userNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var user in model.Users)
        {
            if (!userNames.Add(user.Username))
                issues.Add(new(IssueSeverity.Error, "User", $"Duplicate username '{user.Username}'.", $"User '{user.Username}'"));

            if (!string.IsNullOrEmpty(user.Group) &&
                !model.UserGroups.Any(g => g.Name.Equals(user.Group, StringComparison.OrdinalIgnoreCase)))
                issues.Add(new(IssueSeverity.Error, "User", $"User group '{user.Group}' does not exist.", $"User '{user.Username}'"));

            if (string.IsNullOrEmpty(user.PasswordHash) && string.IsNullOrEmpty(user.Password))
                issues.Add(new(IssueSeverity.Warning, "User", "User has no password set.", $"User '{user.Username}'"));
        }

        var groupNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var group in model.UserGroups)
        {
            if (!groupNames.Add(group.Name))
                issues.Add(new(IssueSeverity.Error, "User", $"Duplicate user group name '{group.Name}'.", $"Group '{group.Name}'"));
        }
    }

    // ── Server Settings ──────────────────────────────────────

    private static void ValidateServer(NodeModel model, List<ProjectIssue> issues)
    {
        var server = model.Server;
        if (server == null) return;

        if (string.IsNullOrWhiteSpace(server.EndpointUrl))
            issues.Add(new(IssueSeverity.Error, "Server", "No OPC UA endpoint URL configured.", "Server Settings"));

        if (server.EnableEditorLogin && model.Users.Count == 0)
            issues.Add(new(IssueSeverity.Error, "Server", "Editor login is enabled but no users are defined.", "Server Settings"));

        if (server.EnableRuntimeLogin && model.Users.Count == 0)
            issues.Add(new(IssueSeverity.Error, "Server", "Runtime login is enabled but no users are defined.", "Server Settings"));

        if (server.CrashEmail is { Enabled: true })
        {
            if (string.IsNullOrWhiteSpace(server.CrashEmail.SmtpHost))
                issues.Add(new(IssueSeverity.Warning, "Server", "Crash email enabled but SMTP host is empty.", "Server Settings"));
            if (string.IsNullOrWhiteSpace(server.CrashEmail.To))
                issues.Add(new(IssueSeverity.Warning, "Server", "Crash email enabled but recipient is empty.", "Server Settings"));
        }
    }
}
