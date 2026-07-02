// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

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

    public void CopyScheduler(SchedulerConfig scheduler)
    {
        ContentType = "Scheduler";
        ContentJson = JsonSerializer.Serialize(scheduler, _jsonOpts);
        Label = scheduler.Name;
        Changed?.Invoke();
    }

    public void CopyReport(ReportConfig report)
    {
        ContentType = "Report";
        ContentJson = JsonSerializer.Serialize(report, _jsonOpts);
        Label = report.Name;
        Changed?.Invoke();
    }

    public void CopyEvent(EventConfig evt)
    {
        ContentType = "Event";
        ContentJson = JsonSerializer.Serialize(evt, _jsonOpts);
        Label = evt.Name;
        Changed?.Invoke();
    }

    public void CopyAliasMap(VariableAliasMap map)
    {
        ContentType = "AliasMap";
        ContentJson = JsonSerializer.Serialize(map, _jsonOpts);
        Label = map.Name;
        Changed?.Invoke();
    }

    public void CopyCalculated(CalculatedVariableConfig calc)
    {
        ContentType = "Calculated";
        ContentJson = JsonSerializer.Serialize(calc, _jsonOpts);
        Label = calc.Name;
        Changed?.Invoke();
    }

    public void CopyAsset(AssetConfig asset)
    {
        ContentType = "Asset";
        ContentJson = JsonSerializer.Serialize(asset, _jsonOpts);
        Label = asset.Name;
        Changed?.Invoke();
    }

    public void CopyBatch(BatchSequenceConfig batch)
    {
        ContentType = "Batch";
        ContentJson = JsonSerializer.Serialize(batch, _jsonOpts);
        Label = batch.Name;
        Changed?.Invoke();
    }

        public void CopySymbols(List<ScreenSymbol> symbols)
    {
        ContentType = "Symbols";
        ContentJson = JsonSerializer.Serialize(symbols, _jsonOpts);
        Label = symbols.Count == 1 ? symbols[0].Id : $"{symbols.Count} symbols";
        Changed?.Invoke();
    }

    // ─── Multi-copy operations ──────────────────────────────

    public void CopyVariables(List<Variable> items)
    {
        ContentType = "Variables";
        ContentJson = JsonSerializer.Serialize(items, _jsonOpts);
        Label = $"{items.Count} variables";
        Changed?.Invoke();
    }

    public void CopyFolders(List<Folder> items)
    {
        ContentType = "Folders";
        ContentJson = JsonSerializer.Serialize(items, _jsonOpts);
        Label = $"{items.Count} folders";
        Changed?.Invoke();
    }

    public void CopyScripts(List<ScriptConfig> items)
    {
        ContentType = "Scripts";
        ContentJson = JsonSerializer.Serialize(items, _jsonOpts);
        Label = $"{items.Count} scripts";
        Changed?.Invoke();
    }

    public void CopyPlcPrograms(List<PlcProgramConfig> items)
    {
        ContentType = "PlcPrograms";
        ContentJson = JsonSerializer.Serialize(items, _jsonOpts);
        Label = $"{items.Count} PLC programs";
        Changed?.Invoke();
    }

    public void CopyRecipes(List<RecipeConfig> items)
    {
        ContentType = "Recipes";
        ContentJson = JsonSerializer.Serialize(items, _jsonOpts);
        Label = $"{items.Count} recipes";
        Changed?.Invoke();
    }

    public void CopyScreens(List<ScreenConfig> items)
    {
        ContentType = "Screens";
        ContentJson = JsonSerializer.Serialize(items, _jsonOpts);
        Label = $"{items.Count} screens";
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

    public SchedulerConfig? PasteScheduler()
    {
        if (ContentType != "Scheduler" || ContentJson == null) return null;
        var s = JsonSerializer.Deserialize<SchedulerConfig>(ContentJson, _jsonOpts);
        if (s != null) s.Name += " (Copy)";
        return s;
    }

    public ReportConfig? PasteReport()
    {
        if (ContentType != "Report" || ContentJson == null) return null;
        var r = JsonSerializer.Deserialize<ReportConfig>(ContentJson, _jsonOpts);
        if (r != null) r.Name += " (Copy)";
        return r;
    }

    public EventConfig? PasteEvent()
    {
        if (ContentType != "Event" || ContentJson == null) return null;
        var e = JsonSerializer.Deserialize<EventConfig>(ContentJson, _jsonOpts);
        if (e != null) e.Name += " (Copy)";
        return e;
    }

    public VariableAliasMap? PasteAliasMap()
    {
        if (ContentType != "AliasMap" || ContentJson == null) return null;
        var m = JsonSerializer.Deserialize<VariableAliasMap>(ContentJson, _jsonOpts);
        if (m != null) m.Name += " (Copy)";
        return m;
    }

    public CalculatedVariableConfig? PasteCalculated()
    {
        if (ContentType != "Calculated" || ContentJson == null) return null;
        var c = JsonSerializer.Deserialize<CalculatedVariableConfig>(ContentJson, _jsonOpts);
        if (c != null) c.Name += " (Copy)";
        return c;
    }

    public AssetConfig? PasteAsset()
    {
        if (ContentType != "Asset" || ContentJson == null) return null;
        var a = JsonSerializer.Deserialize<AssetConfig>(ContentJson, _jsonOpts);
        if (a != null) a.Name += " (Copy)";
        return a;
    }

    public BatchSequenceConfig? PasteBatch()
    {
        if (ContentType != "Batch" || ContentJson == null) return null;
        var b = JsonSerializer.Deserialize<BatchSequenceConfig>(ContentJson, _jsonOpts);
        if (b != null) b.Name += " (Copy)";
        return b;
    }

        public List<ScreenSymbol>? PasteSymbols()
    {
        if (ContentType != "Symbols" || ContentJson == null) return null;
        return JsonSerializer.Deserialize<List<ScreenSymbol>>(ContentJson, _jsonOpts);
    }

    // ─── Multi-paste operations ─────────────────────────────

    public List<Variable>? PasteVariables()
    {
        if (ContentType != "Variables" || ContentJson == null) return null;
        var items = JsonSerializer.Deserialize<List<Variable>>(ContentJson, _jsonOpts);
        items?.ForEach(v => v.Name += " (Copy)");
        return items;
    }

    public List<Folder>? PasteFolders()
    {
        if (ContentType != "Folders" || ContentJson == null) return null;
        var items = JsonSerializer.Deserialize<List<Folder>>(ContentJson, _jsonOpts);
        items?.ForEach(f => f.Name += " (Copy)");
        return items;
    }

    public List<ScriptConfig>? PasteScripts()
    {
        if (ContentType != "Scripts" || ContentJson == null) return null;
        var items = JsonSerializer.Deserialize<List<ScriptConfig>>(ContentJson, _jsonOpts);
        items?.ForEach(s => s.Name += " (Copy)");
        return items;
    }

    public List<PlcProgramConfig>? PastePlcPrograms()
    {
        if (ContentType != "PlcPrograms" || ContentJson == null) return null;
        var items = JsonSerializer.Deserialize<List<PlcProgramConfig>>(ContentJson, _jsonOpts);
        items?.ForEach(p => p.Name += " (Copy)");
        return items;
    }

    public List<RecipeConfig>? PasteRecipes()
    {
        if (ContentType != "Recipes" || ContentJson == null) return null;
        var items = JsonSerializer.Deserialize<List<RecipeConfig>>(ContentJson, _jsonOpts);
        items?.ForEach(r => r.Name += " (Copy)");
        return items;
    }

    public List<ScreenConfig>? PasteScreens()
    {
        if (ContentType != "Screens" || ContentJson == null) return null;
        var items = JsonSerializer.Deserialize<List<ScreenConfig>>(ContentJson, _jsonOpts);
        items?.ForEach(s => s.Name += " (Copy)");
        return items;
    }

    /// <summary>Checks if the clipboard content can be pasted into the given target context.</summary>
    public bool CanPasteInto(TreeNode? target) => ContentType switch
    {
        "Variable" or "Variables" => target is FolderNode or VariableGroupNode,
        "Folder" or "Folders" => target is FolderNode or VariableGroupNode,
        "Script" or "Scripts" => target is ScriptGroupNode or ResourceFolderNode { ResourceKind: "Script" },
        "PlcProgram" or "PlcPrograms" => target is PlcGroupNode or ResourceFolderNode { ResourceKind: "PlcProgram" },
        "Recipe" or "Recipes" => target is RecipeGroupNode,
        "Scheduler" => target is SchedulerGroupNode,
        "Report" => target is ReportGroupNode,
        "Event" => target is EventGroupNode,
        "AliasMap" => target is AliasMapGroupNode,
        "Calculated" => target is CalculatedGroupNode,
        "Asset" => target is AssetGroupNode,
        "Batch" => target is BatchGroupNode,
        "Screen" or "Screens" => target is ScreenGroupNode or ResourceFolderNode { ResourceKind: "Screen" },
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
