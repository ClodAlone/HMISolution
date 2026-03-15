using System.Text.Json;
using ServerEditorWeb.Models;
using SharedModels;

namespace ServerEditorWeb.Services;

/// <summary>
/// In-memory clipboard for copying and pasting tree nodes and screen symbols.
/// </summary>
public class ClipboardService
{
    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = null
    };

    /// <summary>Type tag for the clipboard content.</summary>
    public string? ContentType { get; private set; }

    /// <summary>Serialized JSON payload.</summary>
    public string? ContentJson { get; private set; }

    /// <summary>Human-readable label shown in menus.</summary>
    public string? Label { get; private set; }

    public bool HasContent => ContentType != null;

    public event Action? Changed;

    // ─── Copy operations ────────────────────────────────────

    public void CopyVariable(Variable variable)
    {
        ContentType = "Variable";
        ContentJson = JsonSerializer.Serialize(variable, _jsonOpts);
        Label = variable.Name;
        Changed?.Invoke();
    }

    public void CopyFolder(Folder folder)
    {
        ContentType = "Folder";
        ContentJson = JsonSerializer.Serialize(folder, _jsonOpts);
        Label = folder.Name;
        Changed?.Invoke();
    }

    public void CopyScript(ScriptConfig script)
    {
        ContentType = "Script";
        ContentJson = JsonSerializer.Serialize(script, _jsonOpts);
        Label = script.Name;
        Changed?.Invoke();
    }

    public void CopyPlcProgram(PlcProgramConfig plc)
    {
        ContentType = "PlcProgram";
        ContentJson = JsonSerializer.Serialize(plc, _jsonOpts);
        Label = plc.Name;
        Changed?.Invoke();
    }

    public void CopyRecipe(RecipeConfig recipe)
    {
        ContentType = "Recipe";
        ContentJson = JsonSerializer.Serialize(recipe, _jsonOpts);
        Label = recipe.Name;
        Changed?.Invoke();
    }

    public void CopyScreen(ScreenConfig screen)
    {
        ContentType = "Screen";
        ContentJson = JsonSerializer.Serialize(screen, _jsonOpts);
        Label = screen.Name;
        Changed?.Invoke();
    }

    public void CopySymbols(List<ScreenSymbol> symbols)
    {
        ContentType = "Symbols";
        ContentJson = JsonSerializer.Serialize(symbols, _jsonOpts);
        Label = symbols.Count == 1 ? symbols[0].Id : $"{symbols.Count} symbols";
        Changed?.Invoke();
    }

    // ─── Paste operations ───────────────────────────────────

    public Variable? PasteVariable()
    {
        if (ContentType != "Variable" || ContentJson == null) return null;
        var v = JsonSerializer.Deserialize<Variable>(ContentJson, _jsonOpts);
        if (v != null) v.Name += " (Copy)";
        return v;
    }

    public Folder? PasteFolder()
    {
        if (ContentType != "Folder" || ContentJson == null) return null;
        var f = JsonSerializer.Deserialize<Folder>(ContentJson, _jsonOpts);
        if (f != null) f.Name += " (Copy)";
        return f;
    }

    public ScriptConfig? PasteScript()
    {
        if (ContentType != "Script" || ContentJson == null) return null;
        var s = JsonSerializer.Deserialize<ScriptConfig>(ContentJson, _jsonOpts);
        if (s != null) s.Name += " (Copy)";
        return s;
    }

    public PlcProgramConfig? PastePlcProgram()
    {
        if (ContentType != "PlcProgram" || ContentJson == null) return null;
        var p = JsonSerializer.Deserialize<PlcProgramConfig>(ContentJson, _jsonOpts);
        if (p != null) p.Name += " (Copy)";
        return p;
    }

    public RecipeConfig? PasteRecipe()
    {
        if (ContentType != "Recipe" || ContentJson == null) return null;
        var r = JsonSerializer.Deserialize<RecipeConfig>(ContentJson, _jsonOpts);
        if (r != null) r.Name += " (Copy)";
        return r;
    }

    public ScreenConfig? PasteScreen()
    {
        if (ContentType != "Screen" || ContentJson == null) return null;
        var s = JsonSerializer.Deserialize<ScreenConfig>(ContentJson, _jsonOpts);
        if (s != null) s.Name += " (Copy)";
        return s;
    }

    public List<ScreenSymbol>? PasteSymbols()
    {
        if (ContentType != "Symbols" || ContentJson == null) return null;
        return JsonSerializer.Deserialize<List<ScreenSymbol>>(ContentJson, _jsonOpts);
    }

    /// <summary>Checks if the clipboard content can be pasted into the given target context.</summary>
    public bool CanPasteInto(TreeNode? target) => ContentType switch
    {
        "Variable" => target is FolderNode or VariableGroupNode,
        "Folder" => target is FolderNode or VariableGroupNode,
        "Script" => target is ScriptGroupNode,
        "PlcProgram" => target is PlcGroupNode,
        "Recipe" => target is RecipeGroupNode,
        "Screen" => target is ScreenGroupNode,
        _ => false
    };

    public bool CanPasteSymbols => ContentType == "Symbols";

    public void Clear()
    {
        ContentType = null;
        ContentJson = null;
        Label = null;
        Changed?.Invoke();
    }
}
