// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using SharedModels;
using System.Text.RegularExpressions;

namespace ServerEditorWeb.Services;

/// <summary>
/// Scans the entire NodeModel to find all references to a given variable path.
/// Returns structured results grouped by category (Screen, Script, PLC, Alarm, etc.).
/// </summary>
public class CrossReferenceService
{
    /// <summary>
    /// Find all references to a variable path across the entire project model.
    /// Supports exact match and prefix match (e.g. "Plant.Temp" matches "Plant.Temp.Value").
    /// </summary>
    public List<CrossReference> FindReferences(NodeModel model, string variablePath, bool prefixMatch = false)
    {
        if (model == null || string.IsNullOrWhiteSpace(variablePath))
            return [];

        var results = new List<CrossReference>();
        var path = variablePath.Trim();

        ScanScreens(model, path, prefixMatch, results);
        ScanScripts(model, path, prefixMatch, results);
        ScanPlcPrograms(model, path, prefixMatch, results);
        ScanAlarms(model, path, prefixMatch, results);
        ScanDataLogging(model, path, prefixMatch, results);
        ScanRecipes(model, path, prefixMatch, results);
        ScanReports(model, path, prefixMatch, results);
        ScanCameras(model, path, prefixMatch, results);
        ScanSchedulers(model, path, prefixMatch, results);

        return results;
    }

    /// <summary>
    /// Collect all unique variable paths defined in the project's Folder tree.
    /// </summary>
    public List<string> GetAllVariablePaths(NodeModel model)
    {
        var paths = new List<string>();
        if (model?.Folder != null)
        {
            // Skip root folder name - variable paths never include it
            foreach (var v in model.Folder.Variables)
                paths.Add(v.Name);
            foreach (var sub in model.Folder.Folders)
                CollectVariablePaths(sub, "", paths);
        }
        return paths;
    }

    private static void CollectVariablePaths(Folder folder, string prefix, List<string> paths)
    {
        var folderPath = string.IsNullOrEmpty(prefix) ? folder.Name : $"{prefix}.{folder.Name}";
        foreach (var v in folder.Variables)
            paths.Add($"{folderPath}.{v.Name}");
        foreach (var sub in folder.Folders)
            CollectVariablePaths(sub, folderPath, paths);
    }

    private static bool PathMatches(string candidate, string target, bool prefixMatch)
    {
        if (string.IsNullOrEmpty(candidate)) return false;
        if (prefixMatch)
            return candidate.StartsWith(target, StringComparison.OrdinalIgnoreCase);
        return candidate.Equals(target, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ExpressionContains(string? expression, string path)
    {
        if (string.IsNullOrEmpty(expression)) return false;
        return expression.Contains(path, StringComparison.OrdinalIgnoreCase);
    }

    // ── Screens ──────────────────────────────────────────────

    private static void ScanScreens(NodeModel model, string path, bool prefix, List<CrossReference> results)
    {
        foreach (var screen in model.Screens)
        {
            foreach (var sym in screen.Symbols)
            {
                // Direct variable binding
                if (PathMatches(sym.VariablePath, path, prefix))
                    results.Add(new CrossReference("Screen", screen.Name, $"Symbol '{sym.Type}' ({sym.Id}) — VariablePath", sym.Id));

                // HDA variable paths
                for (int i = 0; i < sym.HdaVariablePaths.Count; i++)
                {
                    if (PathMatches(sym.HdaVariablePaths[i], path, prefix))
                        results.Add(new CrossReference("Screen", screen.Name, $"Symbol '{sym.Type}' ({sym.Id}) — HDA pen #{i + 1}", sym.Id));
                }

                // Trend pens
                foreach (var pen in sym.TrendPens)
                {
                    if (PathMatches(pen.VariablePath, path, prefix))
                        results.Add(new CrossReference("Screen", screen.Name, $"Symbol '{sym.Type}' ({sym.Id}) — Trend pen '{pen.Label}'", sym.Id));
                }

                // Animation bindings (these are expressions — check if path appears as substring)
                if (ExpressionContains(sym.FillBinding, path))
                    results.Add(new CrossReference("Screen", screen.Name, $"Symbol '{sym.Type}' ({sym.Id}) — FillBinding", sym.Id));
                if (ExpressionContains(sym.VisibilityBinding, path))
                    results.Add(new CrossReference("Screen", screen.Name, $"Symbol '{sym.Type}' ({sym.Id}) — VisibilityBinding", sym.Id));
                if (ExpressionContains(sym.RotationBinding, path))
                    results.Add(new CrossReference("Screen", screen.Name, $"Symbol '{sym.Type}' ({sym.Id}) — RotationBinding", sym.Id));
                if (ExpressionContains(sym.LabelBinding, path))
                    results.Add(new CrossReference("Screen", screen.Name, $"Symbol '{sym.Type}' ({sym.Id}) — LabelBinding", sym.Id));

                // Animations
                foreach (var anim in sym.Animations)
                {
                    if (PathMatches(anim.TriggerVariable, path, prefix))
                        results.Add(new CrossReference("Screen", screen.Name, $"Symbol '{sym.Type}' ({sym.Id}) — Animation '{anim.Name}' trigger", sym.Id));
                }

                // Commands
                foreach (var cmd in sym.Commands)
                {
                    if (PathMatches(cmd.VariablePath, path, prefix))
                        results.Add(new CrossReference("Screen", screen.Name, $"Symbol '{sym.Type}' ({sym.Id}) — Command '{cmd.Action}'", sym.Id));
                }
            }
        }
    }

    // ── Scripts ──────────────────────────────────────────────

    private static void ScanScripts(NodeModel model, string path, bool prefix, List<CrossReference> results)
    {
        // Scripts use Read("path"), ReadDouble("path"), ReadInt("path"), ReadBool("path"), Write("path", val)
        var escapedPath = Regex.Escape(path);
        var pattern = prefix
            ? $@"(?:Read(?:Double|Int|Bool)?|Write)\s*\(\s*""({escapedPath}[^""]*?)"""
            : $@"(?:Read(?:Double|Int|Bool)?|Write)\s*\(\s*""{escapedPath}""";

        foreach (var script in model.Scripts)
        {
            if (string.IsNullOrEmpty(script.Code)) continue;

            var matches = Regex.Matches(script.Code, pattern, RegexOptions.IgnoreCase);
            if (matches.Count > 0)
            {
                // Count lines for each match
                foreach (Match m in matches)
                {
                    int line = script.Code[..m.Index].Count(c => c == '\n') + 1;
                    var op = m.Value.StartsWith("Write", StringComparison.OrdinalIgnoreCase) ? "Write" : "Read";
                    results.Add(new CrossReference("Script", script.Name, $"{op} at line {line}", null));
                }
            }
        }
    }

    // ── PLC Programs ─────────────────────────────────────────

    private static void ScanPlcPrograms(NodeModel model, string path, bool prefix, List<CrossReference> results)
    {
        // ST: READ('path') or WRITE('path', value) — or double quotes
        // IL: LD 'path' / ST 'path'
        var escapedPath = Regex.Escape(path);
        var patternPrefix = prefix ? $@"{escapedPath}[^""']*" : escapedPath;
        var stPattern = $@"(?:READ|WRITE)\s*\(\s*[""']({patternPrefix})[""']";
        var ilPattern = $@"(?:LD|ST)\s+[""']({patternPrefix})[""']";

        foreach (var plc in model.PlcPrograms)
        {
            if (string.IsNullOrEmpty(plc.Code)) continue;

            var stMatches = Regex.Matches(plc.Code, stPattern, RegexOptions.IgnoreCase);
            var ilMatches = Regex.Matches(plc.Code, ilPattern, RegexOptions.IgnoreCase);

            foreach (Match m in stMatches)
            {
                int line = plc.Code[..m.Index].Count(c => c == '\n') + 1;
                var op = m.Value.StartsWith("WRITE", StringComparison.OrdinalIgnoreCase) ? "WRITE" : "READ";
                results.Add(new CrossReference("PLC Program", plc.Name, $"{op} at line {line} ({plc.Language})", null));
            }

            foreach (Match m in ilMatches)
            {
                int line = plc.Code[..m.Index].Count(c => c == '\n') + 1;
                var op = m.Value.TrimStart().StartsWith("ST", StringComparison.OrdinalIgnoreCase) ? "ST (store)" : "LD (load)";
                results.Add(new CrossReference("PLC Program", plc.Name, $"{op} at line {line} ({plc.Language})", null));
            }
        }
    }

    // ── Alarms ───────────────────────────────────────────────

    private static void ScanAlarms(NodeModel model, string path, bool prefix, List<CrossReference> results)
    {
        if (model.Folder != null)
            ScanAlarmsInFolder(model.Folder, "", path, prefix, results);
    }

    private static void ScanAlarmsInFolder(Folder folder, string parentPath, string path, bool prefix, List<CrossReference> results)
    {
        var folderPath = string.IsNullOrEmpty(parentPath) ? folder.Name : $"{parentPath}.{folder.Name}";
        foreach (var v in folder.Variables)
        {
            if (v.Alarm != null)
            {
                var varPath = $"{folderPath}.{v.Name}";
                if (PathMatches(varPath, path, prefix))
                {
                    var desc = v.Alarm.TriggerType == AlarmTriggerType.Limit
                        ? $"Limit alarm (H:{v.Alarm.HighLimit} L:{v.Alarm.LowLimit})"
                        : $"Condition alarm ({v.Alarm.Operator} {v.Alarm.CompareValue})";
                    results.Add(new CrossReference("Alarm", varPath, desc, null));
                }
            }
        }
        foreach (var sub in folder.Folders)
            ScanAlarmsInFolder(sub, folderPath, path, prefix, results);
    }

    // ── Data Logging ─────────────────────────────────────────

    private static void ScanDataLogging(NodeModel model, string path, bool prefix, List<CrossReference> results)
    {
        if (model.Folder != null)
            ScanLoggingInFolder(model.Folder, "", path, prefix, results);
    }

    private static void ScanLoggingInFolder(Folder folder, string parentPath, string path, bool prefix, List<CrossReference> results)
    {
        var folderPath = string.IsNullOrEmpty(parentPath) ? folder.Name : $"{parentPath}.{folder.Name}";
        foreach (var v in folder.Variables)
        {
            if (v.DataLogging is { Enabled: true })
            {
                var varPath = $"{folderPath}.{v.Name}";
                if (PathMatches(varPath, path, prefix))
                    results.Add(new CrossReference("Data Logging", varPath, $"Hysteresis: {v.DataLogging.Hysteresis}", null));
            }
        }
        foreach (var sub in folder.Folders)
            ScanLoggingInFolder(sub, folderPath, path, prefix, results);
    }

    // ── Recipes ──────────────────────────────────────────────

    private static void ScanRecipes(NodeModel model, string path, bool prefix, List<CrossReference> results)
    {
        foreach (var recipe in model.Recipes)
        {
            foreach (var rv in recipe.Variables)
            {
                if (PathMatches(rv.VariablePath, path, prefix))
                    results.Add(new CrossReference("Recipe", recipe.Name, $"Variable #{rv.Index} '{rv.DisplayName}'", null));
            }
        }
    }

    // ── Reports ──────────────────────────────────────────────

    private static void ScanReports(NodeModel model, string path, bool prefix, List<CrossReference> results)
    {
        foreach (var report in model.Reports)
        {
            foreach (var section in report.Sections)
            {
                foreach (var vp in section.ChartVariablePaths)
                {
                    if (PathMatches(vp, path, prefix))
                        results.Add(new CrossReference("Report", report.Name, $"Chart section '{section.Title}' — series", null));
                }
                foreach (var vp in section.TableVariablePaths)
                {
                    if (PathMatches(vp, path, prefix))
                        results.Add(new CrossReference("Report", report.Name, $"Table section '{section.Title}' — row", null));
                }
                if (PathMatches(section.ValueVariablePath, path, prefix))
                    results.Add(new CrossReference("Report", report.Name, $"Value section '{section.Title}'", null));
            }
        }
    }

    // ── Cameras ──────────────────────────────────────────────

    private static void ScanCameras(NodeModel model, string path, bool prefix, List<CrossReference> results)
    {
        foreach (var cam in model.Cameras)
        {
            if (!string.IsNullOrEmpty(cam.DetectionVariablePrefix) && PathMatches(cam.DetectionVariablePrefix, path, prefix))
                results.Add(new CrossReference("Camera", cam.CameraId, $"Detection variable prefix", null));
        }

        // Also scan screen-level camera symbols
        foreach (var screen in model.Screens)
        {
            foreach (var sym in screen.Symbols)
            {
                if (sym.Camera != null && !string.IsNullOrEmpty(sym.Camera.DetectionVariablePrefix))
                {
                    if (PathMatches(sym.Camera.DetectionVariablePrefix, path, prefix))
                        results.Add(new CrossReference("Camera", $"{screen.Name} → {sym.Id}", "Detection variable prefix", sym.Id));
                }
            }
        }
    }

    // ── Schedulers ───────────────────────────────────────────

    private static void ScanSchedulers(NodeModel model, string path, bool prefix, List<CrossReference> results)
    {
        foreach (var sched in model.Schedulers)
        {
            ScanSchedulerCommands(sched.Commands, sched.Name, "Activate", path, prefix, results);
            ScanSchedulerCommands(sched.DeactivateCommands, sched.Name, "Deactivate", path, prefix, results);
        }
    }

    private static void ScanSchedulerCommands(List<SymbolCommand> commands, string schedName, string phase, string path, bool prefix, List<CrossReference> results)
    {
        foreach (var cmd in commands)
        {
            if (PathMatches(cmd.VariablePath, path, prefix))
                results.Add(new CrossReference("Scheduler", schedName, $"{phase} command '{cmd.Action}'", null));
        }
    }
}

/// <summary>
/// A single cross-reference result describing where a variable path is used.
/// </summary>
public record CrossReference(
    string Category,       // "Screen", "Script", "PLC Program", "Alarm", "Data Logging", "Recipe", "Report", "Camera", "Scheduler"
    string Location,       // Screen name, script name, variable path, etc.
    string Detail,         // Human-readable description of the usage
    string? SymbolId       // Optional screen symbol ID for navigation
);
