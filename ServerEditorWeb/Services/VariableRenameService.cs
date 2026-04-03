using SharedModels;
using System.Text.RegularExpressions;

namespace ServerEditorWeb.Services;

/// <summary>
/// Propagates a variable or folder rename across the entire NodeModel,
/// updating all references in screens, scripts, PLC programs, recipes, reports, etc.
/// Returns a count of how many references were updated.
/// </summary>
public static class VariableRenameService
{
    /// <summary>
    /// Renames all occurrences of <paramref name="oldPath"/> to <paramref name="newPath"/>
    /// across the entire project model. Supports both exact and prefix renaming
    /// (e.g. renaming a folder updates all child variable paths).
    /// </summary>
    public static int RenameAll(NodeModel model, string oldPath, string newPath)
    {
        if (model == null || string.IsNullOrWhiteSpace(oldPath) || string.IsNullOrWhiteSpace(newPath))
            return 0;
        int count = 0;
        count += RenameInScreens(model, oldPath, newPath);
        count += RenameInScripts(model, oldPath, newPath);
        count += RenameInPlcPrograms(model, oldPath, newPath);
        count += RenameInRecipes(model, oldPath, newPath);
        count += RenameInReports(model, oldPath, newPath);
        count += RenameInSchedulers(model, oldPath, newPath);
        count += RenameInCameras(model, oldPath, newPath);
        count += RenameInCalculatedVariables(model, oldPath, newPath);
        return count;
    }

    private static string ReplacePath(string? value, string oldPath, string newPath)
    {
        if (string.IsNullOrEmpty(value)) return value ?? "";
        if (value.Equals(oldPath, StringComparison.OrdinalIgnoreCase))
            return newPath;
        if (value.StartsWith(oldPath + ".", StringComparison.OrdinalIgnoreCase))
            return newPath + value.Substring(oldPath.Length);
        return value;
    }

    private static bool IsMatch(string? value, string oldPath)
    {
        if (string.IsNullOrEmpty(value)) return false;
        return value.Equals(oldPath, StringComparison.OrdinalIgnoreCase)
            || value.StartsWith(oldPath + ".", StringComparison.OrdinalIgnoreCase);
    }

    private static string ReplaceInExpression(string? expr, string oldPath, string newPath)
    {
        if (string.IsNullOrEmpty(expr)) return expr ?? "";
        return Regex.Replace(expr, Regex.Escape(oldPath), newPath, RegexOptions.IgnoreCase);
    }

    private static string ReplaceInCode(string? code, string oldPath, string newPath)
    {
        if (string.IsNullOrEmpty(code)) return code ?? "";
        // Match oldPath inside quoted strings: exact or as prefix (folder rename)
        // Pattern: quote + oldPath + (optional .suffix) + quote
        var escaped = Regex.Escape(oldPath);
        return Regex.Replace(code,
            @"(?<=[""'])" + escaped + @"(?=[""']|\.)",
            newPath, RegexOptions.IgnoreCase);
    }
        private static int RenameInScreens(NodeModel model, string oldPath, string newPath)
    {
        int count = 0;
        foreach (var screen in model.Screens)
        {
            foreach (var sym in screen.Symbols)
            {
                if (IsMatch(sym.VariablePath, oldPath))
                { sym.VariablePath = ReplacePath(sym.VariablePath, oldPath, newPath); count++; }

                for (int i = 0; i < sym.HdaVariablePaths.Count; i++)
                    if (IsMatch(sym.HdaVariablePaths[i], oldPath))
                    { sym.HdaVariablePaths[i] = ReplacePath(sym.HdaVariablePaths[i], oldPath, newPath); count++; }

                foreach (var pen in sym.TrendPens)
                    if (IsMatch(pen.VariablePath, oldPath))
                    { pen.VariablePath = ReplacePath(pen.VariablePath, oldPath, newPath); count++; }

                if (!string.IsNullOrEmpty(sym.FillBinding) && sym.FillBinding.Contains(oldPath, StringComparison.OrdinalIgnoreCase))
                { sym.FillBinding = ReplaceInExpression(sym.FillBinding, oldPath, newPath); count++; }
                if (!string.IsNullOrEmpty(sym.VisibilityBinding) && sym.VisibilityBinding.Contains(oldPath, StringComparison.OrdinalIgnoreCase))
                { sym.VisibilityBinding = ReplaceInExpression(sym.VisibilityBinding, oldPath, newPath); count++; }
                if (!string.IsNullOrEmpty(sym.RotationBinding) && sym.RotationBinding.Contains(oldPath, StringComparison.OrdinalIgnoreCase))
                { sym.RotationBinding = ReplaceInExpression(sym.RotationBinding, oldPath, newPath); count++; }
                if (!string.IsNullOrEmpty(sym.LabelBinding) && sym.LabelBinding.Contains(oldPath, StringComparison.OrdinalIgnoreCase))
                { sym.LabelBinding = ReplaceInExpression(sym.LabelBinding, oldPath, newPath); count++; }

                foreach (var anim in sym.Animations)
                    if (IsMatch(anim.TriggerVariable, oldPath))
                    { anim.TriggerVariable = ReplacePath(anim.TriggerVariable, oldPath, newPath); count++; }

                foreach (var cmd in sym.Commands)
                    if (IsMatch(cmd.VariablePath, oldPath))
                    { cmd.VariablePath = ReplacePath(cmd.VariablePath, oldPath, newPath); count++; }
            }
        }
        return count;
    }
    private static int RenameInScripts(NodeModel model, string oldPath, string newPath)
    {
        int count = 0;
        foreach (var script in model.Scripts)
        {
            if (!string.IsNullOrEmpty(script.Code) && script.Code.Contains(oldPath, StringComparison.OrdinalIgnoreCase))
            {
                script.Code = ReplaceInCode(script.Code, oldPath, newPath);
                count++;
            }
        }
        return count;
    }

    private static int RenameInPlcPrograms(NodeModel model, string oldPath, string newPath)
    {
        int count = 0;
        foreach (var plc in model.PlcPrograms)
        {
            if (!string.IsNullOrEmpty(plc.Code) && plc.Code.Contains(oldPath, StringComparison.OrdinalIgnoreCase))
            {
                plc.Code = ReplaceInCode(plc.Code, oldPath, newPath);
                count++;
            }
        }
        return count;
    }

    private static int RenameInRecipes(NodeModel model, string oldPath, string newPath)
    {
        int count = 0;
        foreach (var recipe in model.Recipes)
            foreach (var rv in recipe.Variables)
                if (IsMatch(rv.VariablePath, oldPath))
                { rv.VariablePath = ReplacePath(rv.VariablePath, oldPath, newPath); count++; }
        return count;
    }
    private static int RenameInReports(NodeModel model, string oldPath, string newPath)
    {
        int count = 0;
        foreach (var report in model.Reports)
            foreach (var section in report.Sections)
            {
                for (int i = 0; i < section.ChartVariablePaths.Count; i++)
                    if (IsMatch(section.ChartVariablePaths[i], oldPath))
                    { section.ChartVariablePaths[i] = ReplacePath(section.ChartVariablePaths[i], oldPath, newPath); count++; }
                for (int i = 0; i < section.TableVariablePaths.Count; i++)
                    if (IsMatch(section.TableVariablePaths[i], oldPath))
                    { section.TableVariablePaths[i] = ReplacePath(section.TableVariablePaths[i], oldPath, newPath); count++; }
                if (IsMatch(section.ValueVariablePath, oldPath))
                { section.ValueVariablePath = ReplacePath(section.ValueVariablePath, oldPath, newPath); count++; }
            }
        return count;
    }

    private static int RenameInSchedulers(NodeModel model, string oldPath, string newPath)
    {
        int count = 0;
        foreach (var sched in model.Schedulers)
        {
            foreach (var cmd in sched.Commands)
                if (IsMatch(cmd.VariablePath, oldPath))
                { cmd.VariablePath = ReplacePath(cmd.VariablePath, oldPath, newPath); count++; }
            foreach (var cmd in sched.DeactivateCommands)
                if (IsMatch(cmd.VariablePath, oldPath))
                { cmd.VariablePath = ReplacePath(cmd.VariablePath, oldPath, newPath); count++; }
        }
        return count;
    }

    private static int RenameInCameras(NodeModel model, string oldPath, string newPath)
    {
        int count = 0;
        foreach (var cam in model.Cameras)
            if (IsMatch(cam.DetectionVariablePrefix, oldPath))
            { cam.DetectionVariablePrefix = ReplacePath(cam.DetectionVariablePrefix, oldPath, newPath); count++; }
        return count;
    }

    private static int RenameInCalculatedVariables(NodeModel model, string oldPath, string newPath)
    {
        int count = 0;
        foreach (var cv in model.CalculatedVariables)
        {
            if (!string.IsNullOrEmpty(cv.Expression) && cv.Expression.Contains(oldPath, StringComparison.OrdinalIgnoreCase))
            { cv.Expression = ReplaceInExpression(cv.Expression, oldPath, newPath); count++; }
        }
        return count;
    }
}