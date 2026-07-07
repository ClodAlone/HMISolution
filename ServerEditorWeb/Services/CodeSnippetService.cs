// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using SharedModels;
using System.Collections.Concurrent;
using System.Text.Json;

namespace ServerEditorWeb.Services;

/// <summary>
/// Manages code snippets for scripts and PLC programs.
/// Provides built-in default snippets and persists user-defined snippets.
/// </summary>
public class CodeSnippetService
{
    private readonly string _snippetsDir;
    private readonly bool _canPersist;
    private readonly ConcurrentDictionary<string, CodeSnippet> _snippets = new();
    private readonly ILogger<CodeSnippetService> _logger;

    private const string BuiltInSnippetsFile = "builtin-snippets.json";
    private const string UserSnippetsFile = "user-snippets.json";
    private const string SnippetsFolder = "snippets";

    public CodeSnippetService(ILogger<CodeSnippetService> logger)
    {
        _logger = logger;
        (_snippetsDir, _canPersist) = ResolveSnippetsDir();

        // Load built-in and user snippets
        LoadBuiltInSnippets();
        LoadUserSnippets();
    }

    /// <summary>
    /// Picks a writable directory for user snippets. Tries, in order:
    ///   1. &lt;AppContext.BaseDirectory&gt;/data/snippets   (dev / user install)
    ///   2. %LOCALAPPDATA%/HMI Solution/snippets           (Program Files install)
    ///   3. %TEMP%/HMI Solution/snippets                    (last resort)
    /// Never throws; if nothing is writable returns (baseDir, false) and
    /// user snippets simply won't persist.
    /// </summary>
    private (string dir, bool writable) ResolveSnippetsDir()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "data", SnippetsFolder),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "HMI Solution", SnippetsFolder),
            Path.Combine(Path.GetTempPath(), "HMI Solution", SnippetsFolder)
        };

        foreach (var dir in candidates)
        {
            if (TryEnsureWritable(dir)) return (dir, true);
        }

        _logger.LogWarning("No writable snippets directory found; user snippets will not persist.");
        return (candidates[0], false);
    }

    private static bool TryEnsureWritable(string dir)
    {
        if (string.IsNullOrWhiteSpace(dir)) return false;
        try
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var probe = Path.Combine(dir, ".write_test_" + Guid.NewGuid().ToString("N")[..8]);
            File.WriteAllText(probe, "");
            File.Delete(probe);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets all snippets, optionally filtered by language and/or category.
    /// </summary>
    public IEnumerable<CodeSnippet> GetSnippets(string? language = null, string? category = null)
    {
        var query = _snippets.Values.AsEnumerable();

        if (!string.IsNullOrEmpty(language))
            query = query.Where(s => s.Language.Equals(language, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(category))
            query = query.Where(s => s.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        return query.OrderByDescending(s => s.UsageCount).ThenBy(s => s.Name);
    }

    /// <summary>
    /// Searches snippets by name, description, or tags.
    /// </summary>
    public IEnumerable<CodeSnippet> SearchSnippets(string query, string? language = null)
    {
        query = query.ToLower();
        var matches = _snippets.Values.Where(s =>
        {
            if (!string.IsNullOrEmpty(language) && !s.Language.Equals(language, StringComparison.OrdinalIgnoreCase))
                return false;

            return s.Name.ToLower().Contains(query) ||
                   s.Description.ToLower().Contains(query) ||
                   s.Tags.Any(t => t.ToLower().Contains(query)) ||
                   s.Category.ToLower().Contains(query);
        });

        return matches.OrderByDescending(s => s.UsageCount).ThenBy(s => s.Name);
    }

    /// <summary>
    /// Gets a specific snippet by ID.
    /// </summary>
    public CodeSnippet? GetSnippetById(string id)
    {
        _snippets.TryGetValue(id, out var snippet);
        return snippet;
    }

    /// <summary>
    /// Gets all available categories for a language.
    /// </summary>
    public IEnumerable<string> GetCategoriesByLanguage(string language)
    {
        return _snippets.Values
            .Where(s => s.Language.Equals(language, StringComparison.OrdinalIgnoreCase))
            .Select(s => s.Category)
            .Distinct()
            .OrderBy(c => c);
    }

    /// <summary>
    /// Adds a new user-defined snippet.
    /// </summary>
    public async Task<string> AddSnippetAsync(CodeSnippet snippet)
    {
        if (string.IsNullOrWhiteSpace(snippet.Name))
            throw new ArgumentException("Snippet name cannot be empty");

        if (string.IsNullOrWhiteSpace(snippet.Code))
            throw new ArgumentException("Snippet code cannot be empty");

        snippet.IsBuiltIn = false;
        snippet.CreatedAt = DateTime.UtcNow;
        snippet.ModifiedAt = DateTime.UtcNow;

        _snippets[snippet.Id] = snippet;
        await SaveUserSnippetsAsync();

        _logger.LogInformation("Added snippet: {SnippetId} - {SnippetName}", snippet.Id, snippet.Name);
        return snippet.Id;
    }

    /// <summary>
    /// Updates an existing user-defined snippet.
    /// </summary>
    public async Task UpdateSnippetAsync(CodeSnippet snippet)
    {
        if (!_snippets.TryGetValue(snippet.Id, out var existing))
            throw new KeyNotFoundException($"Snippet not found: {snippet.Id}");

        if (existing.IsBuiltIn)
            throw new InvalidOperationException("Cannot modify built-in snippets");

        snippet.ModifiedAt = DateTime.UtcNow;
        snippet.IsBuiltIn = false;
        _snippets[snippet.Id] = snippet;
        await SaveUserSnippetsAsync();

        _logger.LogInformation("Updated snippet: {SnippetId}", snippet.Id);
    }

    /// <summary>
    /// Deletes a user-defined snippet.
    /// </summary>
    public async Task DeleteSnippetAsync(string id)
    {
        if (!_snippets.TryGetValue(id, out var snippet))
            throw new KeyNotFoundException($"Snippet not found: {id}");

        if (snippet.IsBuiltIn)
            throw new InvalidOperationException("Cannot delete built-in snippets");

        _snippets.TryRemove(id, out _);
        await SaveUserSnippetsAsync();

        _logger.LogInformation("Deleted snippet: {SnippetId}", id);
    }

    /// <summary>
    /// Records usage of a snippet.
    /// </summary>
    public async Task RecordUsageAsync(string id)
    {
        if (_snippets.TryGetValue(id, out var snippet))
        {
            snippet.UsageCount++;
            if (!snippet.IsBuiltIn)
            {
                await SaveUserSnippetsAsync();
            }
        }
    }

    /// <summary>
    /// Duplicates a built-in snippet as a user-defined snippet.
    /// </summary>
    public async Task<string> DuplicateSnippetAsync(string id)
    {
        if (!_snippets.TryGetValue(id, out var original))
            throw new KeyNotFoundException($"Snippet not found: {id}");

        var cloned = original.Clone();
        return await AddSnippetAsync(cloned);
    }

    /// <summary>
    /// Loads all built-in default snippets.
    /// </summary>
    private void LoadBuiltInSnippets()
    {
        var snippets = GetDefaultSnippets();
        foreach (var snippet in snippets)
        {
            _snippets[snippet.Id] = snippet;
        }

        _logger.LogInformation("Loaded {Count} built-in snippets", snippets.Count);
    }

    /// <summary>
    /// Loads user-defined snippets from disk.
    /// </summary>
    private void LoadUserSnippets()
    {
        try
        {
            var path = Path.Combine(_snippetsDir, UserSnippetsFile);
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var userSnippets = JsonSerializer.Deserialize<List<CodeSnippet>>(json);
                if (userSnippets != null)
                {
                    foreach (var snippet in userSnippets)
                    {
                        _snippets[snippet.Id] = snippet;
                    }
                    _logger.LogInformation("Loaded {Count} user-defined snippets", userSnippets.Count);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user snippets");
        }
    }

    /// <summary>
    /// Saves user-defined snippets to disk.
    /// </summary>
    private async Task SaveUserSnippetsAsync()
    {
        if (!_canPersist) return;
        try
        {
            var userSnippets = _snippets.Values
                .Where(s => !s.IsBuiltIn)
                .ToList();

            var path = Path.Combine(_snippetsDir, UserSnippetsFile);
            var json = JsonSerializer.Serialize(userSnippets, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(path, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving user snippets");
        }
    }

    /// <summary>
    /// Returns default built-in snippets for all languages.
    /// </summary>
    private static List<CodeSnippet> GetDefaultSnippets()
    {
        return new()
        {
            // ===== C# SNIPPETS =====

            new CodeSnippet
            {
                Id = "cs-for-loop",
                Name = "For Loop",
                Description = "Standard for loop iteration",
                Language = "CSharp",
                Category = "Loops",
                Code = "for (int i = 0; i < 10; i++)\n{\n    // code here\n}",
                IsBuiltIn = true,
                Tags = new() { "loop", "iteration", "i" },
                ExampleUsage = "for (int i = 0; i < Items.Count; i++) { Process(Items[i]); }"
            },

            new CodeSnippet
            {
                Id = "cs-foreach-loop",
                Name = "Foreach Loop",
                Description = "Iterate over collection elements",
                Language = "CSharp",
                Category = "Loops",
                Code = "foreach (var item in collection)\n{\n    // code here\n}",
                IsBuiltIn = true,
                Tags = new() { "loop", "enumeration", "collection" },
                ExampleUsage = "foreach (var item in Items) { Console.WriteLine(item); }"
            },

            new CodeSnippet
            {
                Id = "cs-while-loop",
                Name = "While Loop",
                Description = "Loop while condition is true",
                Language = "CSharp",
                Category = "Loops",
                Code = "while (condition)\n{\n    // code here\n}",
                IsBuiltIn = true,
                Tags = new() { "loop", "conditional" }
            },

            new CodeSnippet
            {
                Id = "cs-if-else",
                Name = "If-Else Statement",
                Description = "Conditional branching",
                Language = "CSharp",
                Category = "Conditionals",
                Code = "if (condition)\n{\n    // code here\n}\nelse\n{\n    // alternative code\n}",
                IsBuiltIn = true,
                Tags = new() { "conditional", "branch" }
            },

            new CodeSnippet
            {
                Id = "cs-try-catch",
                Name = "Try-Catch Block",
                Description = "Exception handling",
                Language = "CSharp",
                Category = "Error Handling",
                Code = "try\n{\n    // code here\n}\ncatch (Exception ex)\n{\n    // handle exception\n    Log($\"Error: {ex.Message}\");\n}",
                IsBuiltIn = true,
                Tags = new() { "exception", "error", "handling" }
            },

            new CodeSnippet
            {
                Id = "cs-switch",
                Name = "Switch Statement",
                Description = "Multi-way branching",
                Language = "CSharp",
                Category = "Conditionals",
                Code = "switch (value)\n{\n    case 1:\n        // code\n        break;\n    default:\n        // code\n        break;\n}",
                IsBuiltIn = true,
                Tags = new() { "conditional", "case" }
            },

            new CodeSnippet
            {
                Id = "cs-read-variable",
                Name = "Read OPC Variable",
                Description = "Read a value from an OPC UA variable by name",
                Language = "CSharp",
                Category = "IO",
                Code = "var value = Read(\"VariableName\");\nLog($\"Read: {value}\");",
                IsBuiltIn = true,
                Tags = new() { "opc", "read", "variable" },
                ExampleUsage = "var temp = Read(\"Boiler.Temperature\");"
            },

            new CodeSnippet
            {
                Id = "cs-read-typed",
                Name = "Read Typed Variable",
                Description = "Read a variable with automatic type conversion",
                Language = "CSharp",
                Category = "IO",
                Code = "double temp    = ReadDouble(\"Boiler.Temperature\");\nint    counter = ReadInt(\"Line1.Counter\");\nbool   running = ReadBool(\"Line1.Running\");\nstring status  = ReadString(\"Line1.Status\");",
                IsBuiltIn = true,
                Tags = new() { "opc", "read", "typed" }
            },

            new CodeSnippet
            {
                Id = "cs-write-variable",
                Name = "Write OPC Variable",
                Description = "Write a value to an OPC UA variable by name",
                Language = "CSharp",
                Category = "IO",
                Code = "Write(\"VariableName\", newValue);\nLog(\"Written successfully\");",
                IsBuiltIn = true,
                Tags = new() { "opc", "write", "variable" },
                ExampleUsage = "Write(\"Line1.Setpoint\", 42.0);"
            },

            new CodeSnippet
            {
                Id = "cs-read-multiple",
                Name = "Read Multiple Variables",
                Description = "Batch-read several variables at once",
                Language = "CSharp",
                Category = "IO",
                Code = "var values = ReadMultiple(\"Line1.Temp\", \"Line1.Pressure\", \"Line1.Flow\");\nforeach (var kvp in values)\n    Log($\"{kvp.Key} = {kvp.Value}\");",
                IsBuiltIn = true,
                Tags = new() { "opc", "read", "batch" }
            },

            new CodeSnippet
            {
                Id = "cs-write-multiple",
                Name = "Write Multiple Variables",
                Description = "Batch-write several variables at once",
                Language = "CSharp",
                Category = "IO",
                Code = "WriteMultiple(new Dictionary<string, object>\n{\n    [\"Line1.Setpoint\"] = 42.0,\n    [\"Line1.Enable\"]   = true,\n    [\"Line1.Mode\"]     = \"Auto\"\n});",
                IsBuiltIn = true,
                Tags = new() { "opc", "write", "batch" }
            },

            new CodeSnippet
            {
                Id = "cs-on-changed",
                Name = "Subscribe to Variable Change",
                Description = "React to a variable value change",
                Language = "CSharp",
                Category = "IO",
                Code = "OnChanged(\"Line1.Alarm\", e =>\n{\n    Log($\"{e.VariableName} changed: {e.OldValue} -> {e.NewValue}\");\n});",
                IsBuiltIn = true,
                Tags = new() { "opc", "subscribe", "event" }
            },

            new CodeSnippet
            {
                Id = "cs-quality",
                Name = "Get / Set Variable Quality",
                Description = "Inspect or override OPC UA data quality",
                Language = "CSharp",
                Category = "IO",
                Code = "bool good = GetQuality(\"Sensor.Value\");\nif (!good)\n{\n    Log(\"Sensor quality is bad — forcing good after recovery\");\n    SetQuality(\"Sensor.Value\", true);\n}",
                IsBuiltIn = true,
                Tags = new() { "opc", "quality", "diagnostics" }
            },

            new CodeSnippet
            {
                Id = "cs-timestamp",
                Name = "Get Variable Timestamp",
                Description = "Get source timestamp of a variable",
                Language = "CSharp",
                Category = "IO",
                Code = "var ts = GetTimestamp(\"Sensor.Value\");\nif (ts.HasValue)\n    Log($\"Last update: {ts.Value:O}\");",
                IsBuiltIn = true,
                Tags = new() { "opc", "timestamp" }
            },

            new CodeSnippet
            {
                Id = "cs-log",
                Name = "Log Message",
                Description = "Append a line to the script log",
                Language = "CSharp",
                Category = "Debugging",
                Code = "Log($\"Value = {value}, time = {DateTime.Now:HH:mm:ss}\");",
                IsBuiltIn = true,
                Tags = new() { "log", "debug" }
            },

            new CodeSnippet
            {
                Id = "cs-log-event",
                Name = "Log Event",
                Description = "Write a structured entry to the event journal",
                Language = "CSharp",
                Category = "Debugging",
                Code = "// category: Alarm | Auth | Driver | System\nLogEvent(\"Alarm\", \"High\", \"Boiler\", \"Temperature above 90°C\", details: null);",
                IsBuiltIn = true,
                Tags = new() { "event", "journal", "audit" }
            },

            new CodeSnippet
            {
                Id = "cs-delay",
                Name = "Cancellation-Aware Delay",
                Description = "Await a delay that respects script cancellation",
                Language = "CSharp",
                Category = "Async",
                Code = "await Delay(500); // milliseconds",
                IsBuiltIn = true,
                Tags = new() { "delay", "await", "sleep" }
            },

            new CodeSnippet
            {
                Id = "cs-notification",
                Name = "Send Notification",
                Description = "Broadcast a notification to all enabled channels",
                Language = "CSharp",
                Category = "Notifications",
                Code = "SendNotification(\n    title:    \"Line 1 Alarm\",\n    message:  \"Temperature exceeded limit\",\n    severity: 800);",
                IsBuiltIn = true,
                Tags = new() { "notification", "alarm" }
            },

            new CodeSnippet
            {
                Id = "cs-math-abs",
                Name = "Absolute Value",
                Description = "Get absolute value of number",
                Language = "CSharp",
                Category = "Math",
                Code = "double result = Math.Abs(value);",
                IsBuiltIn = true,
                Tags = new() { "math", "abs", "number" }
            },

            new CodeSnippet
            {
                Id = "cs-math-min-max",
                Name = "Min/Max Values",
                Description = "Get minimum or maximum of values",
                Language = "CSharp",
                Category = "Math",
                Code = "double min = Math.Min(a, b);\ndouble max = Math.Max(a, b);",
                IsBuiltIn = true,
                Tags = new() { "math", "min", "max" }
            },

            new CodeSnippet
            {
                Id = "cs-debug-log",
                Name = "Debug Log",
                Description = "Output debug information via the script log",
                Language = "CSharp",
                Category = "Debugging",
                Code = "Log($\"Debug: variable = {variable}\");\nLog($\"Info:  data     = {data}\");",
                IsBuiltIn = true,
                Tags = new() { "debug", "log" }
            },

            new CodeSnippet
            {
                Id = "cs-string-format",
                Name = "String Formatting",
                Description = "Format strings with interpolation",
                Language = "CSharp",
                Category = "Strings",
                Code = "string message = $\"Value: {value}, Time: {DateTime.Now:HH:mm:ss}\";\nLog(message);",
                IsBuiltIn = true,
                Tags = new() { "string", "format", "interpolation" }
            },

            new CodeSnippet
            {
                Id = "cs-linq-query",
                Name = "LINQ Query",
                Description = "Query collections with LINQ",
                Language = "CSharp",
                Category = "Collections",
                Code = "var result = from item in collection\n             where item.Value > 10\n             select item.Name;\nforeach (var name in result) { Log(name); }",
                IsBuiltIn = true,
                Tags = new() { "linq", "query", "collection" }
            },

            // ===== VB.NET SNIPPETS =====

            new CodeSnippet
            {
                Id = "vb-read-variable",
                Name = "Read OPC Variable",
                Description = "Read a value from an OPC UA variable",
                Language = "VbNet",
                Category = "IO",
                Code = "Dim value = Read(\"VariableName\")\nLog($\"Read: {value}\")",
                IsBuiltIn = true,
                Tags = new() { "opc", "read", "variable" }
            },

            new CodeSnippet
            {
                Id = "vb-write-variable",
                Name = "Write OPC Variable",
                Description = "Write a value to an OPC UA variable",
                Language = "VbNet",
                Category = "IO",
                Code = "Write(\"VariableName\", newValue)\nLog(\"Written successfully\")",
                IsBuiltIn = true,
                Tags = new() { "opc", "write", "variable" }
            },

            new CodeSnippet
            {
                Id = "vb-on-changed",
                Name = "Subscribe to Variable Change",
                Description = "React to a variable value change",
                Language = "VbNet",
                Category = "IO",
                Code = "OnChanged(\"Line1.Alarm\", Sub(e)\n    Log($\"{e.VariableName} changed: {e.OldValue} -> {e.NewValue}\")\nEnd Sub)",
                IsBuiltIn = true,
                Tags = new() { "opc", "subscribe", "event" }
            },

            new CodeSnippet
            {
                Id = "vb-if-then",
                Name = "If-Then-Else",
                Description = "Conditional branching in VB.NET",
                Language = "VbNet",
                Category = "Conditionals",
                Code = "If condition Then\n    ' code here\nElse\n    ' alternative code\nEnd If",
                IsBuiltIn = true,
                Tags = new() { "conditional", "branch" }
            },

            new CodeSnippet
            {
                Id = "vb-for-loop",
                Name = "For Loop",
                Description = "Standard for loop iteration",
                Language = "VbNet",
                Category = "Loops",
                Code = "For i As Integer = 0 To 9\n    ' code here\nNext",
                IsBuiltIn = true,
                Tags = new() { "loop", "iteration" }
            },

            new CodeSnippet
            {
                Id = "vb-try-catch",
                Name = "Try-Catch Block",
                Description = "Exception handling in VB.NET",
                Language = "VbNet",
                Category = "Error Handling",
                Code = "Try\n    ' code here\nCatch ex As Exception\n    Log($\"Error: {ex.Message}\")\nEnd Try",
                IsBuiltIn = true,
                Tags = new() { "exception", "error" }
            },

            // ===== PLC ST (STRUCTURED TEXT) SNIPPETS =====

            new CodeSnippet
            {
                Id = "st-for-loop",
                Name = "For Loop",
                Description = "Standard for loop in ST",
                Language = "PlcSt",
                Category = "Loops",
                Code = "FOR i := 0 TO 9 DO\n  (* code here *)\nEND_FOR;",
                IsBuiltIn = true,
                Tags = new() { "loop", "iteration" }
            },

            new CodeSnippet
            {
                Id = "st-while-loop",
                Name = "While Loop",
                Description = "While loop in ST",
                Language = "PlcSt",
                Category = "Loops",
                Code = "WHILE condition DO\n  (* code here *)\nEND_WHILE;",
                IsBuiltIn = true,
                Tags = new() { "loop", "conditional" }
            },

            new CodeSnippet
            {
                Id = "st-if-statement",
                Name = "If Statement",
                Description = "Conditional in ST",
                Language = "PlcSt",
                Category = "Conditionals",
                Code = "IF condition THEN\n  (* code here *)\nELSE\n  (* alternative *)\nEND_IF;",
                IsBuiltIn = true,
                Tags = new() { "conditional", "branch" }
            },

            new CodeSnippet
            {
                Id = "st-case-statement",
                Name = "Case Statement",
                Description = "Switch/case in ST",
                Language = "PlcSt",
                Category = "Conditionals",
                Code = "CASE value OF\n  1: (* code *)\n  2: (* code *)\nELSE\n  (* default *)\nEND_CASE;",
                IsBuiltIn = true,
                Tags = new() { "conditional", "case" }
            },

            new CodeSnippet
            {
                Id = "st-var-declaration",
                Name = "Variable Declaration",
                Description = "Declare variable in ST",
                Language = "PlcSt",
                Category = "Variables",
                Code = "VAR\n  myVar : INT := 0;\n  myFlag : BOOL := FALSE;\nEND_VAR;",
                IsBuiltIn = true,
                Tags = new() { "variable", "declaration" }
            },

            new CodeSnippet
            {
                Id = "st-array-access",
                Name = "Array Access",
                Description = "Access array element in ST",
                Language = "PlcSt",
                Category = "Arrays",
                Code = "VAR\n  myArray : ARRAY[0..9] OF INT;\nEND_VAR\n\n(* Access element *)\nvalue := myArray[5];",
                IsBuiltIn = true,
                Tags = new() { "array", "index" }
            },

            new CodeSnippet
            {
                Id = "st-timer",
                Name = "TON Timer",
                Description = "On-delay timer in ST",
                Language = "PlcSt",
                Category = "Function Blocks",
                Code = "timer : TON;\n\ntimer(IN:=startCondition, PT:=T#1S);\nDone := timer.Q;",
                IsBuiltIn = true,
                Tags = new() { "timer", "delay", "ton" }
            },

            new CodeSnippet
            {
                Id = "st-counter",
                Name = "CTU Counter",
                Description = "Up counter in ST",
                Language = "PlcSt",
                Category = "Function Blocks",
                Code = "counter : CTU;\n\ncounter(CU:=incrementSignal, R:=resetSignal, PV:=100);\nCount := counter.CV;",
                IsBuiltIn = true,
                Tags = new() { "counter", "count", "ctu" }
            },

            // ===== PLC IL (INSTRUCTION LIST) SNIPPETS =====

            new CodeSnippet
            {
                Id = "il-load-store",
                Name = "Load Store",
                Description = "Load and store value in IL",
                Language = "PlcIl",
                Category = "Variables",
                Code = "LD      inputValue\nST      outputValue",
                IsBuiltIn = true,
                Tags = new() { "load", "store", "variable" }
            },

            new CodeSnippet
            {
                Id = "il-and-operation",
                Name = "AND Operation",
                Description = "Logical AND in IL",
                Language = "PlcIl",
                Category = "Logic",
                Code = "LD      input1\nAND     input2\nST      result",
                IsBuiltIn = true,
                Tags = new() { "and", "logic", "boolean" }
            },

            new CodeSnippet
            {
                Id = "il-or-operation",
                Name = "OR Operation",
                Description = "Logical OR in IL",
                Language = "PlcIl",
                Category = "Logic",
                Code = "LD      input1\nOR      input2\nST      result",
                IsBuiltIn = true,
                Tags = new() { "or", "logic", "boolean" }
            },

            new CodeSnippet
            {
                Id = "il-jump",
                Name = "Jump Label",
                Description = "Jump to label in IL",
                Language = "PlcIl",
                Category = "Control Flow",
                Code = "LD      condition\nJMPC    SKIP_BLOCK\n(* code to skip *)\nSKIP_BLOCK:",
                IsBuiltIn = true,
                Tags = new() { "jump", "label", "goto" }
            }
        };
    }
}
