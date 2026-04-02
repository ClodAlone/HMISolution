using System.Text.Json;
using SharedModels;

namespace ServerEditorWeb.Services;

/// <summary>
/// Compares two NodeModel snapshots and produces a structured, categorised diff.
/// </summary>
public class ProjectDiffService
{
    /// <summary>
    /// Deserialise a JSON string into a NodeModel, returning null on failure.
    /// </summary>
    public NodeModel? Parse(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<NodeModel>(json);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Compare two NodeModel snapshots and return a list of categorised diff entries.
    /// </summary>
    public List<DiffEntry> Compare(NodeModel left, NodeModel right)
    {
        var entries = new List<DiffEntry>();

        CompareNamedList("Variable", FlattenVariables(left.Folder), FlattenVariables(right.Folder), entries);
        CompareNamedList("Screen", left.Screens.Select(s => s.Name).ToList(), right.Screens.Select(s => s.Name).ToList(), entries);
        CompareScreenSymbolCounts(left, right, entries);
        CompareNamedList("Script", left.Scripts.Select(s => s.Name).ToList(), right.Scripts.Select(s => s.Name).ToList(), entries);
        CompareNamedList("PLC Program", left.PlcPrograms.Select(p => p.Name).ToList(), right.PlcPrograms.Select(p => p.Name).ToList(), entries);
        CompareNamedList("Recipe", left.Recipes.Select(r => r.Name).ToList(), right.Recipes.Select(r => r.Name).ToList(), entries);
        CompareNamedList("Scheduler", left.Schedulers.Select(s => s.Name).ToList(), right.Schedulers.Select(s => s.Name).ToList(), entries);
        CompareNamedList("Report", left.Reports.Select(r => r.Name).ToList(), right.Reports.Select(r => r.Name).ToList(), entries);
        CompareNamedList("User", left.Users.Select(u => u.Username).ToList(), right.Users.Select(u => u.Username).ToList(), entries);
        CompareNamedList("User Group", left.UserGroups.Select(g => g.Name).ToList(), right.UserGroups.Select(g => g.Name).ToList(), entries);
        CompareNamedList("String", left.Strings.Select(s => s.Key).ToList(), right.Strings.Select(s => s.Key).ToList(), entries);
        CompareNamedList("Image", left.Images.Select(i => i.Id).ToList(), right.Images.Select(i => i.Id).ToList(), entries);
        CompareNamedList("Calculated Variable", left.CalculatedVariables.Select(c => c.Name).ToList(), right.CalculatedVariables.Select(c => c.Name).ToList(), entries);

        CompareServerSettings(left.Server, right.Server, entries);

        return entries;
    }

    private static List<string> FlattenVariables(Folder folder, string prefix = "")
    {
        var result = new List<string>();
        var path = string.IsNullOrEmpty(prefix) ? folder.Name : $"{prefix}.{folder.Name}";
        if (string.IsNullOrEmpty(folder.Name)) path = prefix;

        foreach (var v in folder.Variables)
        {
            var vPath = string.IsNullOrEmpty(path) ? v.Name : $"{path}.{v.Name}";
            result.Add(vPath);
        }
        foreach (var sub in folder.Folders)
        {
            result.AddRange(FlattenVariables(sub, path));
        }
        return result;
    }

    private static void CompareNamedList(string category, List<string> leftNames, List<string> rightNames, List<DiffEntry> entries)
    {
        var leftSet = new HashSet<string>(leftNames, StringComparer.Ordinal);
        var rightSet = new HashSet<string>(rightNames, StringComparer.Ordinal);

        foreach (var name in rightNames)
        {
            if (!leftSet.Contains(name))
                entries.Add(new DiffEntry(category, name, DiffKind.Added));
        }

        foreach (var name in leftNames)
        {
            if (!rightSet.Contains(name))
                entries.Add(new DiffEntry(category, name, DiffKind.Removed));
        }
    }

    private static void CompareScreenSymbolCounts(NodeModel left, NodeModel right, List<DiffEntry> entries)
    {
        var leftScreens = left.Screens.ToDictionary(s => s.Name, s => s.Symbols.Count, StringComparer.Ordinal);
        var rightScreens = right.Screens.ToDictionary(s => s.Name, s => s.Symbols.Count, StringComparer.Ordinal);

        foreach (var name in leftScreens.Keys.Intersect(rightScreens.Keys))
        {
            var lc = leftScreens[name];
            var rc = rightScreens[name];
            if (lc != rc)
                entries.Add(new DiffEntry("Screen Symbols", name, DiffKind.Modified, $"{lc} → {rc} symbols"));
        }
    }

    private static void CompareServerSettings(ServerSettings left, ServerSettings right, List<DiffEntry> entries)
    {
        var opts = new JsonSerializerOptions { WriteIndented = false };
        var leftJson = JsonSerializer.Serialize(left, opts);
        var rightJson = JsonSerializer.Serialize(right, opts);
        if (leftJson != rightJson)
            entries.Add(new DiffEntry("Server Settings", "(root)", DiffKind.Modified, "Settings changed"));
    }
}

public enum DiffKind
{
    Added,
    Removed,
    Modified
}

public record DiffEntry(string Category, string Name, DiffKind Kind, string? Detail = null)
{
    public string Icon => Kind switch
    {
        DiffKind.Added => "+",
        DiffKind.Removed => "−",
        DiffKind.Modified => "~",
        _ => "?"
    };

    public string CssClass => Kind switch
    {
        DiffKind.Added => "diff-added",
        DiffKind.Removed => "diff-removed",
        DiffKind.Modified => "diff-modified",
        _ => ""
    };
}
