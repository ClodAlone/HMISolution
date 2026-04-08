using System.Text.RegularExpressions;
using ServerEditorWeb.Models;
using SharedModels;

namespace ServerEditorWeb.Services;

/// <summary>
/// Manages find &amp; replace state and provides project-wide search across all text content
/// (variables, scripts, PLC programs, screens, JSON editor, strings, schedulers, reports, etc.).
/// </summary>
public class FindReplaceService
{
    private readonly NodeEditorService _editor;

    public FindReplaceService(NodeEditorService editor)
    {
        _editor = editor;
    }

    public string SearchText { get; set; } = "";
    public string ReplaceText { get; set; } = "";
    public bool CaseSensitive { get; set; }
    public bool WholeWord { get; set; }
    public bool UseRegex { get; set; }

    public event Action? StateChanged;
    public void NotifyStateChanged() => StateChanged?.Invoke();

    /// <summary>Describes a single match found in the project.</summary>
    public record FindResult(
        string Category,   // e.g. "Script", "PLC Program", "Variable", "Screen", "String", "JSON Editor"
        string ItemName,   // e.g. script name, variable path
        string Context,    // line or text snippet containing the match
        int Line,          // 0-based line or -1 if not line-based
        int Column,        // 0-based column or -1
        string FieldName   // which field the match is in, e.g. "Code", "Name", "Description"
    );

    /// <summary>Performs a full-text search across the entire loaded project.</summary>
    public List<FindResult> SearchProject()
    {
        var results = new List<FindResult>();
        if (string.IsNullOrEmpty(SearchText)) return results;

        // Search all open projects
        foreach (var project in _editor.OpenProjects)
        {
            var model = project.Model;
            if (model == null) continue;
            var projectPrefix = _editor.OpenProjects.Count > 1 ? $"[{project.Name}] " : "";

            // Scripts
            foreach (var script in model.Scripts)
            {
                SearchInCode(results, $"{projectPrefix}Script", script.Name, "Code", script.Code);
                SearchInField(results, $"{projectPrefix}Script", script.Name, "Name", script.Name);
            }

            // PLC Programs
            foreach (var plc in model.PlcPrograms)
            {
                SearchInCode(results, $"{projectPrefix}PLC Program", plc.Name, "Code", plc.Code);
                SearchInField(results, $"{projectPrefix}PLC Program", plc.Name, "Name", plc.Name);
            }

            // Variables (recursive)
            if (model.Folder != null)
                SearchInFolder(results, model.Folder, "", projectPrefix);

            // Screens
            foreach (var screen in model.Screens)
            {
                SearchInField(results, $"{projectPrefix}Screen", screen.Name, "Name", screen.Name);
                foreach (var sym in screen.Symbols)
                {
                    var symLabel = $"{screen.Name}/{sym.Label ?? sym.Id}";
                    SearchInField(results, $"{projectPrefix}Screen Symbol", symLabel, "Label", sym.Label ?? "");
                    SearchInField(results, $"{projectPrefix}Screen Symbol", symLabel, "VariablePath", sym.VariablePath ?? "");
                    SearchInField(results, $"{projectPrefix}Screen Symbol", symLabel, "FillBinding", sym.FillBinding ?? "");
                    SearchInField(results, $"{projectPrefix}Screen Symbol", symLabel, "VisibilityBinding", sym.VisibilityBinding ?? "");
                    SearchInField(results, $"{projectPrefix}Screen Symbol", symLabel, "LabelBinding", sym.LabelBinding ?? "");
                }
            }

            // Strings
            foreach (var str in model.Strings)
            {
                SearchInField(results, $"{projectPrefix}String", str.Key, "Key", str.Key);
                foreach (var kvp in str.Translations)
                    SearchInField(results, $"{projectPrefix}String", $"{str.Key} [{kvp.Key}]", "Translation", kvp.Value);
            }

            // Schedulers
            foreach (var sched in model.Schedulers)
            {
                SearchInField(results, $"{projectPrefix}Scheduler", sched.Name, "Name", sched.Name);
            }

            // Reports
            foreach (var report in model.Reports)
            {
                SearchInField(results, $"{projectPrefix}Report", report.Name, "Name", report.Name);
            }

            // Calculated variables
            foreach (var calc in model.CalculatedVariables)
            {
                SearchInField(results, $"{projectPrefix}Calculated", calc.Name, "Name", calc.Name);
                SearchInField(results, $"{projectPrefix}Calculated", calc.Name, "Expression", calc.Expression);
            }

            // Recipes
            foreach (var recipe in model.Recipes)
            {
                SearchInField(results, $"{projectPrefix}Recipe", recipe.Name, "Name", recipe.Name);
            }
        }

        // JSON Editor text
        if (!string.IsNullOrEmpty(_editor.JsonEditorText))
            SearchInCode(results, "JSON Editor", "Current", "Content", _editor.JsonEditorText);

        return results;
    }

    /// <summary>Replaces all matches across the entire project, returning the number of replacements.</summary>
    public int ReplaceAllInProject()
    {
        if (string.IsNullOrEmpty(SearchText)) return 0;
        int count = 0;

        foreach (var project in _editor.OpenProjects)
        {
            var model = project.Model;
            if (model == null) continue;

            // Scripts
            foreach (var script in model.Scripts)
            {
                var (newCode, n) = ReplaceInText(script.Code);
                if (n > 0) { script.Code = newCode; count += n; }

                var (newName, nn) = ReplaceInText(script.Name);
                if (nn > 0) { script.Name = newName; count += nn; }
            }

            // PLC Programs
            foreach (var plc in model.PlcPrograms)
            {
                var (newCode, n) = ReplaceInText(plc.Code);
                if (n > 0) { plc.Code = newCode; count += n; }

                var (newName, nn) = ReplaceInText(plc.Name);
                if (nn > 0) { plc.Name = newName; count += nn; }
            }

            // Variables (recursive)
            if (model.Folder != null)
                count += ReplaceInFolder(model.Folder);

            // Screens
            foreach (var screen in model.Screens)
            {
                var (newName, nn) = ReplaceInText(screen.Name);
                if (nn > 0) { screen.Name = newName; count += nn; }

                foreach (var sym in screen.Symbols)
                {
                    var (newLabel, n1) = ReplaceInText(sym.Label ?? "");
                    if (n1 > 0) { sym.Label = newLabel; count += n1; }

                    var (newVar, n2) = ReplaceInText(sym.VariablePath ?? "");
                    if (n2 > 0) { sym.VariablePath = newVar; count += n2; }

                    var (newFill, n3) = ReplaceInText(sym.FillBinding ?? "");
                    if (n3 > 0) { sym.FillBinding = newFill; count += n3; }

                    var (newVis, n4) = ReplaceInText(sym.VisibilityBinding ?? "");
                    if (n4 > 0) { sym.VisibilityBinding = newVis; count += n4; }

                    var (newLbl, n5) = ReplaceInText(sym.LabelBinding ?? "");
                    if (n5 > 0) { sym.LabelBinding = newLbl; count += n5; }
                }
            }

            // Strings
            foreach (var str in model.Strings)
            {
                var (newKey, nk) = ReplaceInText(str.Key);
                if (nk > 0) { str.Key = newKey; count += nk; }

                foreach (var key in str.Translations.Keys.ToList())
                {
                    var (newVal, nv) = ReplaceInText(str.Translations[key]);
                    if (nv > 0) { str.Translations[key] = newVal; count += nv; }
                }
            }

            // Calculated variables
            foreach (var calc in model.CalculatedVariables)
            {
                var (newName, nn) = ReplaceInText(calc.Name);
                if (nn > 0) { calc.Name = newName; count += nn; }

                var (newExpr, ne) = ReplaceInText(calc.Expression);
                if (ne > 0) { calc.Expression = newExpr; count += ne; }
            }

            if (count > 0)
                project.HasUnsavedChanges = true;
        }

        return count;
    }

    private void SearchInFolder(List<FindResult> results, Folder folder, string prefix, string projectPrefix)
    {
        var folderPath = string.IsNullOrEmpty(prefix) ? folder.Name : $"{prefix}.{folder.Name}";
        SearchInField(results, $"{projectPrefix}Folder", folderPath, "Name", folder.Name);

        foreach (var v in folder.Variables)
        {
            var varPath = $"{folderPath}.{v.Name}";
            SearchInField(results, $"{projectPrefix}Variable", varPath, "Name", v.Name);
            SearchInField(results, $"{projectPrefix}Variable", varPath, "EngineeringUnit", v.EngineeringUnit);
            SearchInField(results, $"{projectPrefix}Variable", varPath, "InitialValue", v.InitialValue);
        }

        foreach (var sub in folder.Folders)
            SearchInFolder(results, sub, folderPath, projectPrefix);
    }

    private int ReplaceInFolder(Folder folder)
    {
        int count = 0;
        var (newName, nn) = ReplaceInText(folder.Name);
        if (nn > 0) { folder.Name = newName; count += nn; }

        foreach (var v in folder.Variables)
        {
            var (vn, n1) = ReplaceInText(v.Name);
            if (n1 > 0) { v.Name = vn; count += n1; }

            var (vd, n2) = ReplaceInText(v.EngineeringUnit);
            if (n2 > 0) { v.EngineeringUnit = vd; count += n2; }

            var (va, n3) = ReplaceInText(v.InitialValue);
            if (n3 > 0) { v.InitialValue = va; count += n3; }
        }

        foreach (var sub in folder.Folders)
            count += ReplaceInFolder(sub);

        return count;
    }

    private void SearchInCode(List<FindResult> results, string category, string itemName, string fieldName, string? code)
    {
        if (string.IsNullOrEmpty(code)) return;
        var lines = code.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            foreach (var col in FindMatchPositions(line))
            {
                results.Add(new FindResult(category, itemName, line.TrimEnd('\r').Trim(), i, col, fieldName));
            }
        }
    }

    private void SearchInField(List<FindResult> results, string category, string itemName, string fieldName, string? text)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (IsMatch(text))
        {
            results.Add(new FindResult(category, itemName, text.Length > 120 ? text[..120] + "…" : text, -1, -1, fieldName));
        }
    }

    private bool IsMatch(string text)
    {
        if (UseRegex)
        {
            try
            {
                var options = CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                return Regex.IsMatch(text, SearchText, options);
            }
            catch { return false; }
        }

        var comparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        if (WholeWord)
        {
            return ContainsWholeWord(text, SearchText, comparison);
        }
        return text.Contains(SearchText, comparison);
    }

    private List<int> FindMatchPositions(string line)
    {
        var positions = new List<int>();
        if (UseRegex)
        {
            try
            {
                var options = CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                foreach (Match m in Regex.Matches(line, SearchText, options))
                    positions.Add(m.Index);
            }
            catch { }
            return positions;
        }

        var comparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        int idx = 0;
        while (idx < line.Length)
        {
            var pos = line.IndexOf(SearchText, idx, comparison);
            if (pos < 0) break;

            if (WholeWord && !IsWholeWordAt(line, pos, SearchText.Length))
            {
                idx = pos + 1;
                continue;
            }

            positions.Add(pos);
            idx = pos + SearchText.Length;
        }
        return positions;
    }

    private (string result, int count) ReplaceInText(string text)
    {
        if (string.IsNullOrEmpty(text)) return (text, 0);

        if (UseRegex)
        {
            try
            {
                var options = CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                int count = Regex.Matches(text, SearchText, options).Count;
                if (count == 0) return (text, 0);
                return (Regex.Replace(text, SearchText, ReplaceText ?? "", options), count);
            }
            catch { return (text, 0); }
        }

        var comparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        int total = 0;
        var result = text;
        int searchIdx = 0;

        while (searchIdx < result.Length)
        {
            var pos = result.IndexOf(SearchText, searchIdx, comparison);
            if (pos < 0) break;

            if (WholeWord && !IsWholeWordAt(result, pos, SearchText.Length))
            {
                searchIdx = pos + 1;
                continue;
            }

            result = string.Concat(result.AsSpan(0, pos), ReplaceText ?? "", result.AsSpan(pos + SearchText.Length));
            searchIdx = pos + (ReplaceText?.Length ?? 0);
            total++;
        }

        return (result, total);
    }

    private static bool ContainsWholeWord(string text, string word, StringComparison comparison)
    {
        int idx = 0;
        while (idx < text.Length)
        {
            var pos = text.IndexOf(word, idx, comparison);
            if (pos < 0) return false;
            if (IsWholeWordAt(text, pos, word.Length)) return true;
            idx = pos + 1;
        }
        return false;
    }

    private static bool IsWholeWordAt(string text, int pos, int length)
    {
        bool leftOk = pos == 0 || !char.IsLetterOrDigit(text[pos - 1]);
        bool rightOk = pos + length >= text.Length || !char.IsLetterOrDigit(text[pos + length]);
        return leftOk && rightOk;
    }
}
