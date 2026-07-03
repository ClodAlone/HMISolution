// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System.Text;
using System.Text.Json;

namespace ServerEditorWeb.Services;

public class AiService
{
    public string[] AvailableEngines { get; } = ["OpenAI", "Gemini", "Claude", "Ollama"];

    public string[] OpenAiModels { get; } =
    [
        "gpt-4o-mini",
        "gpt-4o",
        "gpt-4-turbo",
        "gpt-3.5-turbo"
    ];

    private string _openAiModel = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4o-mini";
    public string OpenAiModel { get => _openAiModel; set => _openAiModel = string.IsNullOrWhiteSpace(value) ? "gpt-4o-mini" : value; }

    public string[] ClaudeModels { get; } =
    [
        "claude-sonnet-4-5",
        "claude-opus-4-1",
        "claude-3-7-sonnet-latest",
        "claude-3-5-sonnet-latest",
        "claude-3-5-haiku-latest"
    ];

    private string _claudeModel = Environment.GetEnvironmentVariable("ANTHROPIC_MODEL") ?? "claude-sonnet-4-5";
    public string ClaudeModel { get => _claudeModel; set => _claudeModel = string.IsNullOrWhiteSpace(value) ? "claude-sonnet-4-5" : value; }

    /// <summary>
    /// Very rough token estimate (~4 characters per token for English/JSON text).
    /// Used only for showing a UI warning before hitting provider rate limits.
    /// </summary>
    public static int EstimateTokens(params string?[] parts)
    {
        long chars = 0;
        foreach (var p in parts)
            if (!string.IsNullOrEmpty(p)) chars += p.Length;
        return (int)(chars / 4);
    }

    // Ollama training state
    private bool _isTraining;
    private string _trainingStatus = "";
    private string _ollamaModel = "mistral";
    private string _ollamaBaseUrl = ResolveOllamaUrl();
    private string _trainingContext = "";

    /// <summary>
    /// Resolves the Ollama base URL from the OLLAMA_HOST environment variable
    /// (set by Docker entrypoint), falling back to http://127.0.0.1:11434.
    /// Uses 127.0.0.1 instead of localhost to avoid IPv6 loopback mismatch in containers.
    /// </summary>
    private static string ResolveOllamaUrl()
    {
        var host = Environment.GetEnvironmentVariable("OLLAMA_HOST");
        if (!string.IsNullOrEmpty(host))
        {
            // OLLAMA_HOST is typically "127.0.0.1:11434" (no scheme)
            if (!host.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !host.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                host = "http://" + host;
            return host;
        }
        return "http://127.0.0.1:11434";
    }

    public bool IsTraining => _isTraining;
    public string TrainingStatus => _trainingStatus;
    public string OllamaModel { get => _ollamaModel; set => _ollamaModel = value; }
    public string OllamaBaseUrl { get => _ollamaBaseUrl; set => _ollamaBaseUrl = value; }

    public async Task<(string newJson, string comments)> AskAsync(string engine, string prompt, string currentJson, string? referenceJson = null, string? referenceFileName = null)
    {
        return engine switch
        {
            "OpenAI" => await CallOpenAiAsync(prompt, currentJson, referenceJson, referenceFileName),
            "Gemini" => await CallGeminiAsync(prompt, currentJson, referenceJson, referenceFileName),
            "Claude" => await CallClaudeAsync(prompt, currentJson, referenceJson, referenceFileName),
            "Ollama" => await CallOllamaAsync(prompt, currentJson, referenceJson, referenceFileName),
            _ => ("", $"Engine '{engine}' is not supported.")
        };
    }

    /// <summary>
    /// Streaming variant of <see cref="AskAsync"/>. Invokes <paramref name="onDelta"/> as new
    /// text arrives from the model (each call receives the full accumulated raw text so far),
    /// then returns the parsed (json, comments) pair once the stream completes.
    /// For engines/models that do not support server-side streaming (currently Gemini),
    /// the callback is invoked once at the end with the complete result.
    /// </summary>
    /// <param name="rawMode">
    /// When true, <paramref name="prompt"/> is sent verbatim (no wrapping, no
    /// "return full JSON" boilerplate). Used by chunked import to send a compact
    /// additions-only prompt without the full current JSON.
    /// </param>
    public async Task<(string newJson, string comments)> AskStreamingAsync(
        string engine,
        string prompt,
        string currentJson,
        string? referenceJson,
        string? referenceFileName,
        Func<string, Task> onDelta,
        CancellationToken cancellationToken = default,
        bool rawMode = false)
    {
        return engine switch
        {
            "OpenAI" => await CallOpenAiStreamingAsync(prompt, currentJson, referenceJson, referenceFileName, onDelta, cancellationToken, rawMode),
            "Claude" => await CallClaudeStreamingAsync(prompt, currentJson, referenceJson, referenceFileName, onDelta, cancellationToken, rawMode),
            "Ollama" => await CallOllamaStreamingAsync(prompt, currentJson, referenceJson, referenceFileName, onDelta, cancellationToken, rawMode),
            "Gemini" =>
                await CallAndReportAsync(() => CallGeminiAsync(prompt, currentJson, referenceJson, referenceFileName), onDelta),
            _ => ("", $"Engine '{engine}' is not supported.")
        };
    }

    private static async Task<(string, string)> CallAndReportAsync(
        Func<Task<(string, string)>> call,
        Func<string, Task> onDelta)
    {
        var (json, comments) = await call();
        var combined = string.IsNullOrEmpty(comments) ? json : $"{json}\n///COMMENTS///\n{comments}";
        await onDelta(combined);
        return (json, comments);
    }

    /// <summary>
    /// Iterative "chunked" import for reference files that don't fit in a single request,
    /// using an <b>additions-only</b> merge strategy so the current JSON is never sent whole.
    /// For each chunk of the reference file, we ask the AI to output only a JSON array of new
    /// items to append (variables, folders). We then merge those additions locally into the
    /// existing model. This keeps every request small regardless of how large the base
    /// project is.
    /// </summary>
    public async Task<(string newJson, string comments)> AskInChunksAsync(
        string engine,
        string userPrompt,
        string currentJson,
        string referenceContent,
        string? referenceFileName,
        int chunkChars,
        Func<int, int, int, Task> onChunkStart,
        Func<string, Task> onDelta,
        CancellationToken cancellationToken = default)
    {
        SharedModels.NodeModel? current;
        try
        {
            current = JsonSerializer.Deserialize<SharedModels.NodeModel>(
                currentJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            return ("", $"Chunked import needs a valid current JSON project. Parse error: {ex.Message}");
        }
        if (current?.Folder == null)
            return ("", "Chunked import needs a valid current JSON with a Root folder.");

        var chunks = SplitReferenceIntoChunks(referenceContent, chunkChars);
        if (chunks.Count == 0)
            return await AskStreamingAsync(engine, userPrompt, currentJson, referenceContent, referenceFileName, onDelta, cancellationToken);

        var summary = BuildFolderSummary(current.Folder);
        int totalAdded = 0;
        var allComments = new StringBuilder();

        for (int i = 0; i < chunks.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var chunk = chunks[i];
            await onChunkStart(i + 1, chunks.Count, chunk.Length);

            var chunkPrompt = BuildAdditionsPrompt(userPrompt, summary, chunk, referenceFileName, i + 1, chunks.Count);

            var (rawText, err) = await AskStreamingAsync(
                engine, chunkPrompt, currentJson: "", referenceJson: null, referenceFileName: null,
                onDelta, cancellationToken, rawMode: true);

            if (string.IsNullOrWhiteSpace(rawText) && !string.IsNullOrEmpty(err))
                return ("", $"Chunk {i + 1}/{chunks.Count} failed: {err}");

            var (addedInChunk, chunkDiag) = TryMergeAdditions(current, rawText);
            totalAdded += addedInChunk;

            if (!string.IsNullOrEmpty(err))
                allComments.AppendLine($"[Part {i + 1}/{chunks.Count}] {err}");
            else
                allComments.AppendLine($"[Part {i + 1}/{chunks.Count}] {chunkDiag}");

            // Refresh summary so subsequent chunks won't recreate the same folders.
            summary = BuildFolderSummary(current.Folder);
        }

        var mergedJson = JsonSerializer.Serialize(current, new JsonSerializerOptions { WriteIndented = true });
        var summaryLine = $"Chunked import complete: {totalAdded} item(s) added across {chunks.Count} chunk(s).\n";
        return (mergedJson, summaryLine + allComments);
    }

    /// <summary>
    /// Fast, additions-only variant of a folder-scope edit. Instead of asking the AI to echo
    /// back the full subtree with modifications, we only ask for an <c>additions</c> array of
    /// items to add/update/delete, and apply them locally. Output is orders of magnitude smaller,
    /// which is what usually dominates the wall-clock time for "add N variables" style prompts.
    /// </summary>
    public async Task<(string newJson, string comments)> AskFolderAdditionsAsync(
        string engine,
        string userPrompt,
        string currentJson,
        string scopePath,
        Func<string, Task> onDelta,
        CancellationToken cancellationToken = default)
    {
        SharedModels.NodeModel? model;
        try
        {
            model = JsonSerializer.Deserialize<SharedModels.NodeModel>(
                currentJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            return ("", $"Additions edit needs a valid current JSON. Parse error: {ex.Message}");
        }
        if (model?.Folder == null)
            return ("", "Additions edit needs a valid current JSON with a Root folder.");

        var scopeFolder = FindFolder(model.Folder, scopePath);
        if (scopeFolder == null)
            return ("", $"Scope folder '{scopePath}' not found.");

        var subtreeSummary = BuildFolderSummary(scopeFolder, includeVarNames: true);
        var scopeLabel = string.IsNullOrEmpty(scopePath) ? "Root" : scopePath;

        var sb = new StringBuilder(subtreeSummary.Length + 2048);
        sb.Append("You are editing folder '").Append(scopeLabel).Append("' of an OPC UA HMI project.\n\n");
        sb.Append("Existing items in this subtree (folder path → variable list):\n");
        sb.Append(subtreeSummary);
        sb.Append("\nUser instruction: ").Append(userPrompt).Append("\n\n");
        sb.Append(
            "IMPORTANT: Return ONLY a compact JSON object of this shape (no markdown, no fences):\n" +
            "{\n" +
            "  \"additions\": [\n" +
            "    { \"folderPath\": \"" + scopeLabel + ".SubFolder\", \"variable\": { \"Name\": \"NewVar\", \"Type\": \"Double\", \"Access\": \"Read\", \"EngineeringUnit\": \"°C\", \"Description\": \"...\" } }\n" +
            "  ]\n" +
            "}\n\n" +
            "Rules:\n" +
            " - folderPath is dot-separated. Missing folders are created automatically.\n" +
            " - To add a variable at the root of the current scope, use folderPath = \"" + scopeLabel + "\".\n" +
            " - Do NOT re-add variables that already exist.\n" +
            " - Variable schema: Name, Type (Double|Int32|Boolean|String), Access (Read|Write|ReadWrite), EngineeringUnit, Description, InitialValue.\n" +
            " - Return ONLY the JSON, nothing else. Keep it small.\n" +
            "Optionally append '///COMMENTS///' and a one-line summary after the JSON.");

        var (rawText, err) = await AskStreamingAsync(
            engine, sb.ToString(), currentJson: "", referenceJson: null, referenceFileName: null,
            onDelta, cancellationToken, rawMode: true);

        if (string.IsNullOrWhiteSpace(rawText))
            return ("", string.IsNullOrEmpty(err) ? "Empty response." : err);

        var (added, diag) = TryMergeAdditions(model, rawText);
        if (added == 0)
        {
            // Surface the failure — with the raw response so the user can inspect what the AI actually returned.
            return ("", $"AI response could not be merged into '{scopeLabel}' ({diag}).\n\nRaw AI response:\n{Truncate(rawText, 4000)}");
        }
        var mergedJson = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
        return (mergedJson, $"Additions applied to '{scopeLabel}': {diag}.");
    }

    private static string BuildFolderSummary(SharedModels.Folder root, string prefix = "", bool includeVarNames = false)
    {
        var sb = new StringBuilder();
        void Walk(SharedModels.Folder f, string path)
        {
            var here = string.IsNullOrEmpty(path) ? f.Name : $"{path}.{f.Name}";
            sb.Append("- ").Append(here).Append(" (").Append(f.Variables.Count).Append(" vars)");
            if (includeVarNames && f.Variables.Count > 0)
            {
                sb.Append(": ");
                sb.Append(string.Join(", ", f.Variables.Select(v => v.Name)));
            }
            sb.Append('\n');
            foreach (var sub in f.Folders) Walk(sub, here);
        }
        Walk(root, prefix);
        return sb.ToString();
    }

    private static string BuildAdditionsPrompt(string userPrompt, string folderSummary, string referenceChunk, string? referenceFileName, int part, int totalParts)
    {
        var sb = new StringBuilder(referenceChunk.Length + 2048);
        sb.Append("You are extending an existing OPC UA HMI project. The current project already has these folders:\n\n");
        sb.Append(folderSummary);
        sb.Append("\nUser instruction: ").Append(userPrompt);
        sb.Append("\n\n--- REFERENCE FILE PART ").Append(part).Append(" of ").Append(totalParts);
        if (!string.IsNullOrEmpty(referenceFileName)) sb.Append(" (").Append(referenceFileName).Append(")");
        sb.Append(" ---\n").Append(referenceChunk).Append("\n--- END PART ---\n\n");
        sb.Append(
            "IMPORTANT: Do NOT return the full project JSON. Return ONLY a compact JSON object of the following shape:\n" +
            "{\n" +
            "  \"additions\": [\n" +
            "    { \"folderPath\": \"Energy.Meters\", \"variable\": { \"Name\": \"KWh_Total\", \"Type\": \"Double\", \"Access\": \"Read\", \"EngineeringUnit\": \"kWh\", \"Description\": \"...\" } },\n" +
            "    { \"folderPath\": \"HVAC.AHU1\", \"variable\": { \"Name\": \"SupplyAirTemp\", \"Type\": \"Double\", \"Access\": \"Read\" } }\n" +
            "  ]\n" +
            "}\n\n" +
            "Rules:\n" +
            " - folderPath uses dot-separated names starting from the first child of Root (never include 'Root').\n" +
            " - Missing folders in folderPath will be created automatically.\n" +
            " - Do not re-add items that already exist (see folder listing above).\n" +
            " - Do not include markdown fences. Output pure JSON only.\n" +
            " - variable objects follow the Variable schema (Name, Type, Access, EngineeringUnit, Description, InitialValue, etc.).\n" +
            "After the JSON, optionally add the delimiter '///COMMENTS///' followed by a one-line summary.\n");
        return sb.ToString();
    }

    private static (int added, string diag) TryMergeAdditions(SharedModels.NodeModel model, string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText) || model.Folder == null)
            return (0, "empty response");

        var jsonPart = rawText.Split(["///COMMENTS///"], 2, StringSplitOptions.None)[0]
            .Replace("```json", "").Replace("```", "").Trim();

        // Locate the JSON payload. Try object form first, fall back to bare array.
        JsonElement arrEl = default;
        bool haveArray = false;
        try
        {
            int oStart = jsonPart.IndexOf('{');
            int oEnd = jsonPart.LastIndexOf('}');
            if (oStart >= 0 && oEnd > oStart)
            {
                using var doc = JsonDocument.Parse(jsonPart.Substring(oStart, oEnd - oStart + 1));
                // Accept several key names the model might use.
                foreach (var key in new[] { "additions", "add", "items", "variables", "new", "result" })
                {
                    if (doc.RootElement.TryGetProperty(key, out var el) && el.ValueKind == JsonValueKind.Array)
                    {
                        arrEl = el.Clone();
                        haveArray = true;
                        break;
                    }
                }
            }
        }
        catch (JsonException) { /* try bare-array fallback below */ }

        if (!haveArray)
        {
            try
            {
                int aStart = jsonPart.IndexOf('[');
                int aEnd = jsonPart.LastIndexOf(']');
                if (aStart >= 0 && aEnd > aStart)
                {
                    using var doc = JsonDocument.Parse(jsonPart.Substring(aStart, aEnd - aStart + 1));
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        arrEl = doc.RootElement.Clone();
                        haveArray = true;
                    }
                }
            }
            catch (JsonException) { }
        }

        if (!haveArray)
            return (0, "response did not contain an additions array — got:\n" + Truncate(jsonPart, 1200));

        int added = 0;
        int skippedDup = 0;
        int skippedInvalid = 0;
        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        foreach (var entry in arrEl.EnumerateArray())
        {
            // Support both { folderPath, variable: {...} } and flat { folderPath, Name, Type, ... }
            string path = "";
            JsonElement varEl = default;
            bool haveVar = false;

            if (entry.ValueKind == JsonValueKind.Object)
            {
                if (entry.TryGetProperty("folderPath", out var pathEl) && pathEl.ValueKind == JsonValueKind.String)
                    path = pathEl.GetString() ?? "";
                else if (entry.TryGetProperty("folder", out pathEl) && pathEl.ValueKind == JsonValueKind.String)
                    path = pathEl.GetString() ?? "";
                else if (entry.TryGetProperty("path", out pathEl) && pathEl.ValueKind == JsonValueKind.String)
                    path = pathEl.GetString() ?? "";

                if (entry.TryGetProperty("variable", out varEl) && varEl.ValueKind == JsonValueKind.Object)
                {
                    haveVar = true;
                }
                else if (entry.TryGetProperty("Name", out _) || entry.TryGetProperty("name", out _))
                {
                    varEl = entry;
                    haveVar = true;
                }
            }

            if (!haveVar) { skippedInvalid++; continue; }

            SharedModels.Variable? variable;
            try { variable = varEl.Deserialize<SharedModels.Variable>(opts); }
            catch { skippedInvalid++; continue; }
            if (variable == null || string.IsNullOrWhiteSpace(variable.Name)) { skippedInvalid++; continue; }

            var folder = EnsureFolder(model.Folder, path);
            if (folder.Variables.Any(v => string.Equals(v.Name, variable.Name, StringComparison.OrdinalIgnoreCase)))
            {
                skippedDup++;
                continue;
            }
            folder.Variables.Add(variable);
            added++;
        }

        var diag = $"parsed {arrEl.GetArrayLength()} entries, added {added}, {skippedDup} duplicate(s), {skippedInvalid} invalid";
        return (added, diag);
    }

    private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "…";

    private static SharedModels.Folder EnsureFolder(SharedModels.Folder root, string dottedPath)
    {
        if (string.IsNullOrWhiteSpace(dottedPath)) return root;
        var parts = dottedPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var current = root;
        foreach (var name in parts)
        {
            if (string.Equals(name, "Root", StringComparison.OrdinalIgnoreCase) && current == root) continue;
            var next = current.Folders.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
            if (next == null)
            {
                next = new SharedModels.Folder { Name = name };
                current.Folders.Add(next);
            }
            current = next;
        }
        return current;
    }

    /// <summary>Describes an editable scope in the project.</summary>
    public readonly record struct ScopeEntry(string Key, string Label, int ItemCount, int ApproxChars);

    /// <summary>
    /// Returns every editable scope (folder subtrees + top-level sections like Screens/Scripts).
    /// Scope key format: "folder:Path" (empty path = Root), "section:PropertyName", or "" for whole project.
    /// </summary>
    public static List<ScopeEntry> EnumerateScopes(string currentJson)
    {
        var result = new List<ScopeEntry>();
        SharedModels.NodeModel? model;
        try
        {
            model = JsonSerializer.Deserialize<SharedModels.NodeModel>(
                currentJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch { return result; }
        if (model == null) return result;

        // Folder subtrees
        if (model.Folder != null)
        {
            void Walk(SharedModels.Folder f, string path)
            {
                var subtreeVars = CountVarsRecursive(f);
                var label = string.IsNullOrEmpty(path)
                    ? $"📁 Root  ({subtreeVars} vars total)"
                    : $"📁 {path}  ({f.Variables.Count} vars, {subtreeVars} in subtree)";
                var approx = ApproxSize(f);
                result.Add(new ScopeEntry($"folder:{path}", label, subtreeVars, approx));
                foreach (var sub in f.Folders)
                {
                    var childPath = string.IsNullOrEmpty(path) ? sub.Name : $"{path}.{sub.Name}";
                    Walk(sub, childPath);
                }
            }
            Walk(model.Folder, "");
        }

        // Top-level sections (only list ones that would actually be useful to edit)
        AddSection(result, "Screens", model.Screens);
        AddSection(result, "Scripts", model.Scripts);
        AddSection(result, "PlcPrograms", model.PlcPrograms);
        AddSection(result, "Recipes", model.Recipes);
        AddSection(result, "Users", model.Users);
        AddSection(result, "UserGroups", model.UserGroups);
        AddSection(result, "Cameras", model.Cameras);
        AddSection(result, "Schedulers", model.Schedulers);
        AddSection(result, "Reports", model.Reports);
        AddSection(result, "CalculatedVariables", model.CalculatedVariables);
        AddSection(result, "Assets", model.Assets);
        AddSection(result, "BatchSequences", model.BatchSequences);
        AddSection(result, "Events", model.Events);
        AddSection(result, "AutomationRules", model.AutomationRules);
        AddSection(result, "Strings", model.Strings);
        AddSection(result, "Images", model.Images);
        AddSection(result, "AliasMaps", model.AliasMaps);
        AddSection(result, "UserSymbolGroups", model.UserSymbolGroups);

        return result;
    }

    private static void AddSection<T>(List<ScopeEntry> results, string sectionName, List<T> list)
    {
        var count = list.Count;
        var approx = ApproxSize(list);
        var icon = sectionName switch
        {
            "Screens" => "🖥",
            "Scripts" => "📜",
            "PlcPrograms" => "🔌",
            "Recipes" => "🍳",
            "Users" or "UserGroups" => "👥",
            "Cameras" => "🎥",
            "Schedulers" => "⏰",
            "Reports" => "📊",
            "Images" => "🖼",
            _ => "📦"
        };
        results.Add(new ScopeEntry($"section:{sectionName}", $"{icon} {sectionName}  ({count} item(s))", count, approx));
    }

    private static int ApproxSize(object? obj)
    {
        if (obj == null) return 0;
        try
        {
            return JsonSerializer.Serialize(obj).Length;
        }
        catch { return 0; }
    }

    /// <summary>Backwards-compat wrapper used by callers that only care about folder paths.</summary>
    public static List<(string Path, int VarCount, int TotalVarsUnder)> EnumerateFolderPaths(string currentJson)
    {
        var result = new List<(string, int, int)>();
        SharedModels.NodeModel? model;
        try
        {
            model = JsonSerializer.Deserialize<SharedModels.NodeModel>(
                currentJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch { return result; }
        if (model?.Folder == null) return result;

        void Walk(SharedModels.Folder f, string path)
        {
            var subtreeCount = CountVarsRecursive(f);
            result.Add((path, f.Variables.Count, subtreeCount));
            foreach (var sub in f.Folders)
            {
                var childPath = string.IsNullOrEmpty(path) ? sub.Name : $"{path}.{sub.Name}";
                Walk(sub, childPath);
            }
        }
        Walk(model.Folder, "");
        return result;
    }

    private static int CountVarsRecursive(SharedModels.Folder f)
    {
        int c = f.Variables.Count;
        foreach (var s in f.Folders) c += CountVarsRecursive(s);
        return c;
    }

    /// <summary>
    /// Scoped edit: send only the selected subset to the AI instead of the whole project.
    /// <paramref name="scopeKey"/> is one of:
    ///   - "folder:Path.Dotted"  (empty path = Root folder)
    ///   - "section:PropertyName" (e.g. "section:Screens")
    /// After the AI returns the modified subset, it is spliced back into the full model locally.
    /// </summary>
    public async Task<(string newJson, string comments)> AskScopedAsync(
        string engine,
        string userPrompt,
        string currentJson,
        string scopeKey,
        Func<string, Task> onDelta,
        CancellationToken cancellationToken = default)
    {
        SharedModels.NodeModel? model;
        try
        {
            model = JsonSerializer.Deserialize<SharedModels.NodeModel>(
                currentJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            return ("", $"Scoped edit needs a valid current JSON. Parse error: {ex.Message}");
        }
        if (model == null)
            return ("", "Scoped edit needs a valid current JSON.");

        if (scopeKey.StartsWith("folder:", StringComparison.Ordinal))
        {
            var scopePath = scopeKey["folder:".Length..];
            return await ScopedFolderEdit(engine, userPrompt, model, scopePath, onDelta, cancellationToken);
        }
        if (scopeKey.StartsWith("section:", StringComparison.Ordinal))
        {
            var sectionName = scopeKey["section:".Length..];
            return await ScopedSectionEdit(engine, userPrompt, model, sectionName, onDelta, cancellationToken);
        }
        return ("", $"Unknown scope key '{scopeKey}'.");
    }

    private async Task<(string, string)> ScopedFolderEdit(
        string engine, string userPrompt, SharedModels.NodeModel model, string scopePath,
        Func<string, Task> onDelta, CancellationToken cancellationToken)
    {
        if (model.Folder == null)
            return ("", "Project has no Root folder.");

        var scopeFolder = FindFolder(model.Folder, scopePath);
        if (scopeFolder == null)
            return ("", $"Scope folder '{scopePath}' not found in current JSON.");

        var summary = BuildFolderSummary(model.Folder);
        var scopeJson = JsonSerializer.Serialize(scopeFolder, new JsonSerializerOptions { WriteIndented = true });

        var sb = new StringBuilder(scopeJson.Length + 2048);
        sb.Append("You are editing a subtree of an existing OPC UA HMI project.\n\n");
        sb.Append("Project outline (all folders, for context — do NOT modify these):\n");
        sb.Append(summary);
        sb.Append("\nUser instruction: ").Append(userPrompt).Append("\n\n");
        sb.Append("You are editing this folder subtree: '")
          .Append(string.IsNullOrEmpty(scopePath) ? "Root" : scopePath)
          .Append("'.\n\nCurrent JSON of the subtree:\n").Append(scopeJson).Append("\n\n");
        sb.Append(
            "IMPORTANT: Return ONLY the modified JSON for this ONE folder object (same schema as above). " +
            "Do NOT return the full project. Do NOT include markdown fences. " +
            "Keep the 'Name' property equal to the current folder name. " +
            "After the JSON, optionally add '///COMMENTS///' followed by a one-line summary.");

        var (rawText, err) = await AskStreamingAsync(
            engine, sb.ToString(), currentJson: "", referenceJson: null, referenceFileName: null,
            onDelta, cancellationToken, rawMode: true);

        if (string.IsNullOrWhiteSpace(rawText))
            return ("", string.IsNullOrEmpty(err) ? "Empty response." : err);

        var jsonPart = ExtractJsonObject(rawText);
        if (jsonPart == null)
            return ("", "AI did not return a JSON object for the subtree.\n\n" + rawText);

        SharedModels.Folder? newFolder;
        try
        {
            newFolder = JsonSerializer.Deserialize<SharedModels.Folder>(
                jsonPart,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            return ("", $"AI response is not valid Folder JSON: {ex.Message}\n\n{jsonPart}");
        }
        if (newFolder == null)
            return ("", "AI returned null Folder JSON.");

        if (string.IsNullOrEmpty(scopePath))
        {
            model.Folder = newFolder;
        }
        else
        {
            var parts = scopePath.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var parent = model.Folder;
            for (int i = 0; i < parts.Length - 1; i++)
                parent = parent.Folders.First(f => string.Equals(f.Name, parts[i], StringComparison.OrdinalIgnoreCase));
            var idx = parent.Folders.FindIndex(f => string.Equals(f.Name, parts[^1], StringComparison.OrdinalIgnoreCase));
            if (idx < 0) return ("", $"Could not locate parent slot for '{scopePath}' when merging.");
            newFolder.Name = parts[^1];
            parent.Folders[idx] = newFolder;
        }

        var mergedJson = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
        var scopeVars = CountVarsRecursive(newFolder);
        return (mergedJson, $"Scoped edit applied to folder '{(string.IsNullOrEmpty(scopePath) ? "Root" : scopePath)}' ({scopeVars} variable(s) in subtree after edit).");
    }

    private async Task<(string, string)> ScopedSectionEdit(
        string engine, string userPrompt, SharedModels.NodeModel model, string sectionName,
        Func<string, Task> onDelta, CancellationToken cancellationToken)
    {
        var prop = typeof(SharedModels.NodeModel).GetProperty(sectionName);
        if (prop == null || !prop.CanRead || !prop.CanWrite)
            return ("", $"Unknown or read-only section '{sectionName}'.");
        if (!prop.PropertyType.IsGenericType || prop.PropertyType.GetGenericTypeDefinition() != typeof(List<>))
            return ("", $"Section '{sectionName}' is not a list; scoped editing not supported.");

        var currentList = prop.GetValue(model);
        var currentJson = JsonSerializer.Serialize(currentList, new JsonSerializerOptions { WriteIndented = true });

        var sb = new StringBuilder(currentJson.Length + 1024);
        sb.Append("You are editing the '").Append(sectionName).Append("' section of an OPC UA HMI project.\n");
        sb.Append("Other sections of the project are not shown and must not be affected.\n\n");
        sb.Append("User instruction: ").Append(userPrompt).Append("\n\n");
        sb.Append("Current JSON of this section (a JSON array):\n").Append(currentJson).Append("\n\n");
        sb.Append(
            "IMPORTANT: Return ONLY the modified JSON array for this section (same schema). " +
            "Do NOT return the whole project. Do NOT include markdown fences. " +
            "After the JSON, optionally add '///COMMENTS///' followed by a one-line summary.");

        var (rawText, err) = await AskStreamingAsync(
            engine, sb.ToString(), currentJson: "", referenceJson: null, referenceFileName: null,
            onDelta, cancellationToken, rawMode: true);

        if (string.IsNullOrWhiteSpace(rawText))
            return ("", string.IsNullOrEmpty(err) ? "Empty response." : err);

        var jsonPart = ExtractJsonArray(rawText);
        if (jsonPart == null)
            return ("", "AI did not return a JSON array for the section.\n\n" + rawText);

        object? newList;
        try
        {
            newList = JsonSerializer.Deserialize(jsonPart, prop.PropertyType,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            return ("", $"AI response is not a valid '{sectionName}' array: {ex.Message}\n\n{jsonPart}");
        }
        if (newList == null)
            return ("", $"AI returned null for section '{sectionName}'.");

        prop.SetValue(model, newList);

        var mergedJson = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
        var count = (newList as System.Collections.ICollection)?.Count ?? 0;
        return (mergedJson, $"Scoped edit applied to section '{sectionName}' ({count} item(s) after edit).");
    }

    private static string? ExtractJsonObject(string rawText)
    {
        var text = rawText.Split(["///COMMENTS///"], 2, StringSplitOptions.None)[0]
            .Replace("```json", "").Replace("```", "").Trim();
        int s = text.IndexOf('{');
        int e = text.LastIndexOf('}');
        return (s < 0 || e <= s) ? null : text.Substring(s, e - s + 1);
    }

    private static string? ExtractJsonArray(string rawText)
    {
        var text = rawText.Split(["///COMMENTS///"], 2, StringSplitOptions.None)[0]
            .Replace("```json", "").Replace("```", "").Trim();
        int s = text.IndexOf('[');
        int e = text.LastIndexOf(']');
        return (s < 0 || e <= s) ? null : text.Substring(s, e - s + 1);
    }

    private static SharedModels.Folder? FindFolder(SharedModels.Folder root, string dottedPath)
    {
        if (string.IsNullOrWhiteSpace(dottedPath)) return root;
        var parts = dottedPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var current = root;
        foreach (var name in parts)
        {
            var next = current.Folders.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
            if (next == null) return null;
            current = next;
        }
        return current;
    }

    /// <summary>
    /// Extracts every complete brace-matched <c>{ ... }</c> object inside the <c>additions</c>
    /// array of a partially-streamed AI response. Used to build a live preview of the merged
    /// JSON without waiting for the whole stream to finish.
    /// </summary>
    public static List<string> ExtractCompleteAdditionEntries(string rawText)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(rawText)) return result;
        var text = rawText.Split(["///COMMENTS///"], 2, StringSplitOptions.None)[0]
                          .Replace("```json", "").Replace("```", "");

        // Try several key names the model may use.
        int arrStart = -1;
        foreach (var key in new[] { "additions", "add", "items", "variables", "new", "result" })
        {
            var idx = text.IndexOf($"\"{key}\"", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                var candidate = text.IndexOf('[', idx);
                if (candidate >= 0) { arrStart = candidate; break; }
            }
        }
        // Fallback: bare JSON array at top level.
        if (arrStart < 0) arrStart = text.IndexOf('[');
        if (arrStart < 0) return result;

        int i = arrStart + 1;
        int depth = 0;
        int entryStart = -1;
        bool inString = false;
        bool escape = false;
        while (i < text.Length)
        {
            char c = text[i];
            if (escape) { escape = false; i++; continue; }
            if (c == '\\' && inString) { escape = true; i++; continue; }
            if (c == '"') { inString = !inString; i++; continue; }
            if (inString) { i++; continue; }
            if (c == '{')
            {
                if (depth == 0) entryStart = i;
                depth++;
            }
            else if (c == '}')
            {
                depth--;
                if (depth == 0 && entryStart >= 0)
                {
                    result.Add(text.Substring(entryStart, i - entryStart + 1));
                    entryStart = -1;
                }
            }
            else if (c == ']' && depth == 0) break;
            i++;
        }
        return result;
    }

    /// <summary>
    /// Applies every complete addition entry currently visible in <paramref name="rawStreamText"/>
    /// to a fresh clone of <paramref name="originalJson"/> and returns the resulting merged JSON.
    /// Returns <paramref name="originalJson"/> unchanged if nothing has parsed yet.
    /// Cheap enough to call from a throttled UI callback (≤ a few Hz).
    /// </summary>
    public static string BuildLiveAdditionsPreview(string originalJson, string rawStreamText)
    {
        if (string.IsNullOrEmpty(originalJson)) return originalJson;
        SharedModels.NodeModel? model;
        try
        {
            model = JsonSerializer.Deserialize<SharedModels.NodeModel>(
                originalJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch { return originalJson; }
        if (model?.Folder == null) return originalJson;

        var entries = ExtractCompleteAdditionEntries(rawStreamText);
        if (entries.Count == 0) return originalJson;

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        int applied = 0;
        foreach (var entryJson in entries)
        {
            try
            {
                using var doc = JsonDocument.Parse(entryJson);
                var rootEl = doc.RootElement;

                string path = "";
                if (rootEl.TryGetProperty("folderPath", out var pEl) && pEl.ValueKind == JsonValueKind.String) path = pEl.GetString() ?? "";
                else if (rootEl.TryGetProperty("folder", out pEl) && pEl.ValueKind == JsonValueKind.String) path = pEl.GetString() ?? "";
                else if (rootEl.TryGetProperty("path", out pEl) && pEl.ValueKind == JsonValueKind.String) path = pEl.GetString() ?? "";

                JsonElement varEl;
                if (rootEl.TryGetProperty("variable", out varEl) && varEl.ValueKind == JsonValueKind.Object) { }
                else if (rootEl.TryGetProperty("Name", out _) || rootEl.TryGetProperty("name", out _)) varEl = rootEl;
                else continue;

                var variable = varEl.Deserialize<SharedModels.Variable>(opts);
                if (variable == null || string.IsNullOrWhiteSpace(variable.Name)) continue;
                var folder = EnsureFolder(model.Folder, path);
                if (!folder.Variables.Any(v => string.Equals(v.Name, variable.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    folder.Variables.Add(variable);
                    applied++;
                }
            }
            catch { /* partial or malformed — skip */ }
        }

        return applied == 0
            ? originalJson
            : JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Splits reference text into ~<paramref name="chunkChars"/>-sized pieces, breaking on
    /// newline boundaries so records / XML tags / CSV rows are not cut in half.
    /// </summary>
    public static List<string> SplitReferenceIntoChunks(string content, int chunkChars)
    {
        var chunks = new List<string>();
        if (string.IsNullOrEmpty(content) || chunkChars <= 0) return chunks;
        if (content.Length <= chunkChars)
        {
            chunks.Add(content);
            return chunks;
        }

        var sb = new StringBuilder(chunkChars + 256);
        foreach (var raw in content.Split('\n'))
        {
            var line = raw + "\n";
            if (sb.Length > 0 && sb.Length + line.Length > chunkChars)
            {
                chunks.Add(sb.ToString());
                sb.Clear();
            }
            // Extremely long single line: hard-split it into chunkChars pieces.
            if (line.Length > chunkChars)
            {
                if (sb.Length > 0) { chunks.Add(sb.ToString()); sb.Clear(); }
                for (int i = 0; i < line.Length; i += chunkChars)
                    chunks.Add(line.Substring(i, Math.Min(chunkChars, line.Length - i)));
                continue;
            }
            sb.Append(line);
        }
        if (sb.Length > 0) chunks.Add(sb.ToString());
        return chunks;
    }

    /// <summary>
    /// Lists available sample project files from a directory.
    /// Returns tuples of (displayName, fullPath, groupName).
    /// Scans for nodes.json in each sample folder, plus any .json files in imports/ subdirectories.
    /// </summary>
    public List<(string Name, string Path, string Group)> GetSampleFiles(string samplesDirectory)
    {
        var results = new List<(string, string, string)>();
        if (!Directory.Exists(samplesDirectory))
            return results;

        foreach (var dir in Directory.GetDirectories(samplesDirectory).OrderBy(d => d))
        {
            var dirName = System.IO.Path.GetFileName(dir);

            // Main nodes.json for the sample
            var nodesFile = System.IO.Path.Combine(dir, "nodes.json");
            if (File.Exists(nodesFile))
                results.Add((dirName, nodesFile, "Sample Projects"));

            // Import files in imports/ subdirectory (any format: .json, .xml, .csv, etc.)
            var importsDir = System.IO.Path.Combine(dir, "imports");
            if (Directory.Exists(importsDir))
            {
                foreach (var importFile in Directory.GetFiles(importsDir).OrderBy(f => f))
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(importFile);
                    var ext = System.IO.Path.GetExtension(importFile).ToLowerInvariant();
                    var displayName = $"{dirName} / {fileName}{ext}";
                    results.Add((displayName, importFile, "Driver Import Files"));
                }
            }
        }
        return results;
    }

    /// <summary>
    /// Maximum characters of a reference file to include in a prompt. Anything beyond this
    /// is truncated with a clear marker so the model still sees the shape of the file
    /// without blowing past provider context limits (Claude ≤ 200 K tokens, gpt-4o-mini ≤ 200 K TPM).
    /// </summary>
    public int MaxReferenceChars { get; set; } = 40_000;

    /// <summary>When true, current JSON is minified (parsed then re-serialized without indentation)
    /// before being embedded in the prompt. Typically saves 30–40% on tokens.</summary>
    public bool CompactJsonBeforeSend { get; set; } = true;

    /// <summary>
    /// Reads and returns the content of a reference file.
    /// </summary>
    public string? ReadReferenceFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return null;
        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// If <paramref name="json"/> is valid JSON, returns a minified representation; otherwise
    /// returns the input unchanged. Safe to call on any text; result is functionally identical.
    /// </summary>
    public static string CompactJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return json;
        try
        {
            using var doc = JsonDocument.Parse(json);
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false }))
                doc.WriteTo(writer);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        catch
        {
            return json;
        }
    }

    private string BuildPrompt(string userPrompt, string oldJson, string? referenceJson, string? referenceFileName)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("You are a JSON editor helper for an OPC UA Server configuration. The user wants to modify the following JSON:\n\n");
        sb.Append(CompactJsonBeforeSend ? CompactJson(oldJson) : oldJson);

        if (!string.IsNullOrEmpty(referenceJson))
        {
            var truncated = false;
            if (referenceJson.Length > MaxReferenceChars)
            {
                referenceJson = referenceJson[..MaxReferenceChars];
                truncated = true;
            }

            sb.Append("\n\n--- REFERENCE FILE");
            if (!string.IsNullOrEmpty(referenceFileName))
                sb.Append($" ({referenceFileName})");
            if (truncated)
                sb.Append($" [TRUNCATED to first {MaxReferenceChars:N0} chars]");
            sb.Append(" ---\n");

            // Detect file type and provide format-specific parsing instructions
            var ext = System.IO.Path.GetExtension(referenceFileName ?? "").ToLowerInvariant();
            sb.Append(ext switch
            {
                ".xml" => """
The reference file is a PLC program export (e.g. Siemens TIA Portal SimaticML DB export).
Parse the XML to extract variables. For each <Member> inside a <SW.Blocks.GlobalDB>, create a variable with:
- Folder path derived from the DB name (e.g. DB5_Filler → "Filler" folder)
- Name from the Member's Name attribute
- Type mapped from Siemens types: Real→Double, Int→Int32, DInt→Int32, Bool→Boolean, String[...]→String
- Access: "ReadWrite" for Remanence="Retain", "Read" for "NonRetain"
- Value from <StartValue>
- S7 driver config: { "IpAddress": from the XML comment or "192.168.0.10", "Rack": 0, "Slot": 1, "Address": "DB<Number>.DBB0" (calculate offsets), "PollTime": 1000 }
For <Tag> entries in <SW.Tags.PlcTagTable>, map Address (%I=input, %Q=output) to S7 addresses.
Also check the XML comments at the top for PLC IP address, Rack, and Slot information.

""",
                ".csv" when (referenceJson ?? "").Contains("Group Address") => """
The reference file is a KNX ETS group address export (CSV with semicolons).
Parse each row to extract variables. For each group address, create a variable with:
- Folder path derived from the Building/Floor/Room columns
- Name derived from the Description column (clean it into a valid identifier)
- Type mapped from KNX DPT: DPST-1-x→Boolean, DPST-5-x→Double (0-100), DPST-9-x→Double, DPST-12-x→Int32
- Access: "ReadWrite" for switches/dimmers/setpoints, "Read" for sensors
- KNX driver config: { "ConnectionType": "Tunneling", "GatewayAddress": "192.168.1.100", "GroupAddress": "<the group address>", "DPT": "<the datapoint type id>", "PollTime": 5000 }

""",
                ".csv" => """
The reference file is a Modbus register map (CSV).
Lines starting with # or ## are comments — read them for IP/port info. Parse data rows to extract variables. For each register, create a variable with:
- Folder path derived from the logical grouping in the Name (e.g. AHU1_SupplyAirTemp → "AHU1" folder)
- Name from the Name column (cleaned)
- Type mapped from DataType: Float32→Double, UInt16→Int32, Boolean→Boolean
- Access from R/W column: "R"→"Read", "RW"→"ReadWrite"
- Modbus driver config: { "IpAddress": from comments or "10.0.1.50", "Port": 502, "SlaveId": <SlaveID>, "RegisterType": "<RegisterType>", "Address": <Address>, "DataType": "<DataType>", "ScaleFactor": <ScaleFactor>, "PollTime": 1000 }
- Unit from the Unit column can be used for alarm/display purposes

""",
                _ => "The following is a reference file that the user wants to import from. Extract variables, folders, screens, scripts, PLC programs, recipes, or other elements as instructed by the user:\n\n"
            });

            sb.Append(referenceJson);
            sb.Append("\n--- END REFERENCE FILE ---\n");
        }

        sb.Append("\n\nUser Instruction: ");
        sb.Append(userPrompt);
        sb.Append("\n\nReturn the FULL valid JSON content as the result, incorporating the requested changes. Do not return just the difference or a snippet. Do not use markdown blocks around the JSON.\n\nAt the end of the JSON, print the delimiter '///COMMENTS///', followed by a summary of what has been changed.");
        return sb.ToString();
    }

    private async Task<(string, string)> CallOpenAiAsync(string userPrompt, string oldJson, string? referenceJson = null, string? referenceFileName = null)
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return ("", "Set OPENAI_API_KEY environment variable to use OpenAI.");

        string endpoint = "https://api.openai.com/v1/chat/completions";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var prompt = BuildPrompt(userPrompt, oldJson, referenceJson, referenceFileName);

        var requestBody = new
        {
            model = _openAiModel,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.1
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var contentText = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        return ParseAiResponse(contentText);
    }

    private async Task<(string, string)> CallGeminiAsync(string userPrompt, string oldJson, string? referenceJson = null, string? referenceFileName = null)
    {
        string apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return ("", "Set GEMINI_API_KEY environment variable to use Gemini.");

        string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

        using var client = new HttpClient();

        var prompt = BuildPrompt(userPrompt, oldJson, referenceJson, referenceFileName);

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var contentText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

        return ParseAiResponse(contentText);
    }

    private async Task<(string, string)> CallClaudeAsync(string userPrompt, string oldJson, string? referenceJson = null, string? referenceFileName = null)
    {
        string apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return ("", "Set ANTHROPIC_API_KEY environment variable to use Claude.");

        string endpoint = "https://api.anthropic.com/v1/messages";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var prompt = BuildPrompt(userPrompt, oldJson, referenceJson, referenceFileName);

        var requestBody = new
        {
            model = _claudeModel,
            max_tokens = 4096,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.1
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var contentText = doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString();

        return ParseAiResponse(contentText);
    }

    private static (string newJson, string comments) ParseAiResponse(string? contentText)
    {
        if (contentText == null)
            return ("", "");

        var parts = contentText.Split(["///COMMENTS///"], StringSplitOptions.None);
        string newJson = parts[0].Replace("```json", "").Replace("```", "").Trim();
        string comments = parts.Length > 1 ? parts[1].Trim() : "";
        return (newJson, comments);
    }


    // ── Ollama ──────────────────────────────────────────────────────────

    private async Task<(string, string)> CallOllamaAsync(string userPrompt, string oldJson, string? referenceJson = null, string? referenceFileName = null)
    {
        var prompt = BuildPrompt(userPrompt, oldJson, referenceJson, referenceFileName);

        // Prepend training context if available
        if (!string.IsNullOrEmpty(_trainingContext))
            prompt = _trainingContext + "\n\n" + prompt;

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };

        var requestBody = new
        {
            model = _ollamaModel,
            prompt,
            stream = false,
            options = new { temperature = 0.1 }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsync($"{_ollamaBaseUrl}/api/generate", content);
        }
        catch (HttpRequestException ex)
        {
            return ("", $"Cannot reach Ollama at {_ollamaBaseUrl}. Make sure Ollama is running. ({ex.Message})");
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            return ("", $"Ollama returned {response.StatusCode}: {errorBody}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var contentText = doc.RootElement.GetProperty("response").GetString();

        return ParseAiResponse(contentText);
    }

    // ── Streaming implementations ──────────────────────────────────────

    private async Task<(string, string)> CallOpenAiStreamingAsync(
        string userPrompt, string oldJson, string? referenceJson, string? referenceFileName,
        Func<string, Task> onDelta, CancellationToken cancellationToken, bool rawMode = false)
    {
        string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return ("", "Set OPENAI_API_KEY environment variable to use OpenAI.");

        string endpoint = "https://api.openai.com/v1/chat/completions";
        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var prompt = rawMode ? userPrompt : BuildPrompt(userPrompt, oldJson, referenceJson, referenceFileName);
        var requestBody = new
        {
            model = _openAiModel,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.1,
            stream = true
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                var estimated = EstimateTokens(prompt);
                return ("", $"OpenAI rate-limit / context exceeded (model '{_openAiModel}', ~{estimated:N0} input tokens).\n\n" +
                             "Suggestions:\n" +
                             "  • Switch OpenAI model to 'gpt-4o-mini' (200 K TPM) — the dropdown next to the engine.\n" +
                             "  • Reduce 'Max reference chars' in AI options (currently truncates large reference files).\n" +
                             "  • Keep 'Compact JSON before send' enabled (already saves 30–40%).\n" +
                             "  • For very large inputs (>200 K tokens), switch engine to Claude, which supports much larger contexts.\n\n" + err);
            }
            return ("", $"OpenAI returned {response.StatusCode}: {err}");
        }

        var accumulated = new StringBuilder();
        await ReadSseAsync(response, async line =>
        {
            if (!line.StartsWith("data:", StringComparison.Ordinal)) return;
            var payload = line[5..].Trim();
            if (payload == "[DONE]" || payload.Length == 0) return;
            try
            {
                using var doc = JsonDocument.Parse(payload);
                if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var delta = choices[0].GetProperty("delta");
                    if (delta.TryGetProperty("content", out var contentProp) && contentProp.ValueKind == JsonValueKind.String)
                    {
                        var piece = contentProp.GetString();
                        if (!string.IsNullOrEmpty(piece))
                        {
                            accumulated.Append(piece);
                            await onDelta(accumulated.ToString());
                        }
                    }
                }
            }
            catch (JsonException) { /* ignore malformed SSE line */ }
        }, cancellationToken);

        return ParseAiResponse(accumulated.ToString());
    }

    private async Task<(string, string)> CallClaudeStreamingAsync(
        string userPrompt, string oldJson, string? referenceJson, string? referenceFileName,
        Func<string, Task> onDelta, CancellationToken cancellationToken, bool rawMode = false)
    {
        string apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "";
        if (string.IsNullOrEmpty(apiKey))
            return ("", "Set ANTHROPIC_API_KEY environment variable to use Claude.");

        string endpoint = "https://api.anthropic.com/v1/messages";
        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        var prompt = rawMode ? userPrompt : BuildPrompt(userPrompt, oldJson, referenceJson, referenceFileName);
        var requestBody = new
        {
            model = _claudeModel,
            max_tokens = 8192,
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.1,
            stream = true
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return ("", $"Claude model '{_claudeModel}' not found for your account. Pick a different model from the dropdown (e.g. 'claude-3-5-sonnet-latest' or 'claude-3-5-haiku-latest').\n\n{err}");
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest && err.Contains("prompt is too long", StringComparison.OrdinalIgnoreCase))
            {
                var estimated = EstimateTokens(prompt);
                return ("", $"Claude prompt exceeds the 200 000-token context window (model '{_claudeModel}', ~{estimated:N0} input tokens).\n\n" +
                             "Ways to shrink the request:\n" +
                             "  • Reduce 'Max reference chars' (top of AI panel) — try 20 000 or 10 000.\n" +
                             "  • Keep 'Compact JSON before send' enabled.\n" +
                             "  • Split the work: ask smaller edits one at a time instead of an all-in-one prompt.\n" +
                             "  • Trim the current JSON in the editor (remove folders/screens you don't need modified).\n" +
                             "  • For structured PLC/CSV imports, consider preprocessing the file into a small summary before pasting.\n\n" + err);
            }
            return ("", $"Claude returned {response.StatusCode}: {err}");
        }

        var accumulated = new StringBuilder();
        await ReadSseAsync(response, async line =>
        {
            if (!line.StartsWith("data:", StringComparison.Ordinal)) return;
            var payload = line[5..].Trim();
            if (payload.Length == 0) return;
            try
            {
                using var doc = JsonDocument.Parse(payload);
                if (!doc.RootElement.TryGetProperty("type", out var typeProp)) return;
                var type = typeProp.GetString();
                if (type == "content_block_delta" &&
                    doc.RootElement.TryGetProperty("delta", out var deltaEl) &&
                    deltaEl.TryGetProperty("text", out var textEl) &&
                    textEl.ValueKind == JsonValueKind.String)
                {
                    var piece = textEl.GetString();
                    if (!string.IsNullOrEmpty(piece))
                    {
                        accumulated.Append(piece);
                        await onDelta(accumulated.ToString());
                    }
                }
            }
            catch (JsonException) { /* ignore malformed */ }
        }, cancellationToken);

        return ParseAiResponse(accumulated.ToString());
    }

    private async Task<(string, string)> CallOllamaStreamingAsync(
        string userPrompt, string oldJson, string? referenceJson, string? referenceFileName,
        Func<string, Task> onDelta, CancellationToken cancellationToken, bool rawMode = false)
    {
        var prompt = rawMode ? userPrompt : BuildPrompt(userPrompt, oldJson, referenceJson, referenceFileName);
        if (!string.IsNullOrEmpty(_trainingContext) && !rawMode)
            prompt = _trainingContext + "\n\n" + prompt;

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(15) };
        var requestBody = new
        {
            model = _ollamaModel,
            prompt,
            stream = true,
            options = new { temperature = 0.1 }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_ollamaBaseUrl}/api/generate")
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            return ("", $"Cannot reach Ollama at {_ollamaBaseUrl}. Make sure Ollama is running. ({ex.Message})");
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync(cancellationToken);
                return ("", $"Ollama returned {response.StatusCode}: {err}");
            }

            var accumulated = new StringBuilder();
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                if (line.Length == 0) continue;
                try
                {
                    using var doc = JsonDocument.Parse(line);
                    if (doc.RootElement.TryGetProperty("response", out var respEl) &&
                        respEl.ValueKind == JsonValueKind.String)
                    {
                        var piece = respEl.GetString();
                        if (!string.IsNullOrEmpty(piece))
                        {
                            accumulated.Append(piece);
                            await onDelta(accumulated.ToString());
                        }
                    }
                }
                catch (JsonException) { /* ignore malformed line */ }
            }

            return ParseAiResponse(accumulated.ToString());
        }
    }

    private static async Task ReadSseAsync(
        HttpResponseMessage response,
        Func<string, Task> onLine,
        CancellationToken cancellationToken)
    {
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            if (line.Length == 0) continue;
            await onLine(line);
        }
    }

    /// <summary>
    /// Scans a folder for sample projects (nodes.json) and PLC import files (.xml, .csv, .json),
    /// reads their content, and builds a context string that is prepended to every Ollama prompt.
    /// This teaches the local model about HMI project structure and PLC export formats.
    /// </summary>
    public async Task TrainOllamaAsync(string samplesFolder, Action<string>? onProgress = null)
    {
        if (_isTraining) return;
        _isTraining = true;
        _trainingStatus = "Scanning folder...";
        onProgress?.Invoke(_trainingStatus);

        try
        {
            if (!Directory.Exists(samplesFolder))
            {
                _trainingStatus = $"Folder not found: {samplesFolder}";
                onProgress?.Invoke(_trainingStatus);
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== TRAINING CONTEXT: HMI/SCADA Project Examples ===");
            sb.AppendLine("You have been trained on the following real HMI/SCADA project files and PLC export formats.");
            sb.AppendLine("Use this knowledge to produce accurate JSON configurations when the user asks.\n");

            int fileCount = 0;

            // Collect all relevant files
            var allFiles = new List<(string path, string category)>();
            foreach (var dir in Directory.GetDirectories(samplesFolder).OrderBy(d => d))
            {
                var nodesFile = Path.Combine(dir, "nodes.json");
                if (File.Exists(nodesFile))
                    allFiles.Add((nodesFile, "Sample Project"));

                var importsDir = Path.Combine(dir, "imports");
                if (Directory.Exists(importsDir))
                {
                    foreach (var f in Directory.GetFiles(importsDir).OrderBy(x => x))
                        allFiles.Add((f, "PLC Import File"));
                }
            }

            foreach (var (filePath, category) in allFiles)
            {
                fileCount++;
                var dirName = Path.GetFileName(Path.GetDirectoryName(filePath) ?? "");
                var fileName = Path.GetFileName(filePath);
                _trainingStatus = $"Reading {fileCount}/{allFiles.Count}: {dirName}/{fileName}";
                onProgress?.Invoke(_trainingStatus);

                try
                {
                    var text = await File.ReadAllTextAsync(filePath);
                    // Limit individual file size to keep context manageable
                    if (text.Length > 15000)
                        text = text[..15000] + "\n... (truncated)";

                    sb.AppendLine($"--- {category}: {dirName}/{fileName} ---");
                    sb.AppendLine(text);
                    sb.AppendLine($"--- END {dirName}/{fileName} ---\n");
                }
                catch
                {
                    // Skip unreadable files
                }
            }

            sb.AppendLine("=== END TRAINING CONTEXT ===");
            sb.AppendLine("Use the patterns, variable structures, driver configs, folder hierarchies, screen layouts, alarm configs, and PLC export formats shown above as reference when generating JSON.\n");

            _trainingContext = sb.ToString();

            // Verify Ollama is reachable by making a small test call.
            // The first request after container start requires Ollama to load the full model
            // into memory (30-90s for ~4 GB models), so use a generous timeout.
            _trainingStatus = "Verifying Ollama connection (first call may take a minute while the model loads)...";
            onProgress?.Invoke(_trainingStatus);

            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
            try
            {
                var testBody = new
                {
                    model = _ollamaModel,
                    prompt = "Reply with OK.",
                    stream = false,
                    options = new { num_predict = 5 }
                };
                var json = new StringContent(JsonSerializer.Serialize(testBody), System.Text.Encoding.UTF8, "application/json");
                var resp = await client.PostAsync($"{_ollamaBaseUrl}/api/generate", json);
                if (!resp.IsSuccessStatusCode)
                {
                    _trainingStatus = $"Trained with {fileCount} files. Warning: Ollama returned {resp.StatusCode} — check model '{_ollamaModel}' is pulled.";
                    onProgress?.Invoke(_trainingStatus);
                    return;
                }
            }
            catch (Exception ex)
            {
                _trainingStatus = $"Trained with {fileCount} files. Warning: Cannot reach Ollama at {_ollamaBaseUrl} ({ex.Message}).";
                onProgress?.Invoke(_trainingStatus);
                return;
            }

            _trainingStatus = $"Ready — {fileCount} files loaded ({_trainingContext.Length / 1024} KB context). Model: {_ollamaModel}";
            onProgress?.Invoke(_trainingStatus);
        }
        catch (Exception ex)
        {
            _trainingStatus = $"Training error: {ex.Message}";
            onProgress?.Invoke(_trainingStatus);
        }
        finally
        {
            _isTraining = false;
        }
    }

    /// <summary>
    /// Returns the list of available Ollama models from the local instance.
    /// </summary>
    public async Task<string[]> GetOllamaModelsAsync()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var response = await client.GetAsync($"{_ollamaBaseUrl}/api/tags");
            if (!response.IsSuccessStatusCode) return [];

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("models")
                .EnumerateArray()
                .Select(m => m.GetProperty("name").GetString() ?? "")
                .Where(n => !string.IsNullOrEmpty(n))
                .OrderBy(n => n)
                .ToArray();
        }
        catch
        {
            return [];
        }
    }

}