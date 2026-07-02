// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using System.Reflection;

namespace ServerEditorWeb.Services;

/// <summary>
/// Provides Roslyn-powered IntelliSense completions for C# scripts.
/// Supports dot-member completions, 'using' namespace completions, and general symbol completions.
/// </summary>
public class ScriptCompletionService
{
    private static readonly Lazy<MetadataReference[]> s_defaultReferences = new(BuildDefaultReferences);
    private static readonly Lazy<AdhocWorkspace> s_workspace = new(() =>
    {
        var host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
        return new AdhocWorkspace(host);
    });

    // Cache namespace list (expensive to compute)
    private static readonly Lazy<List<string>> s_systemNamespaces = new(BuildSystemNamespaces);

    /// <summary>
    /// Returns completion items for the given script code at the specified cursor position.
    /// </summary>
    public async Task<List<CompletionItemResult>> GetCompletionsAsync(
        string code, int cursorPosition, IReadOnlyList<string>? extraReferences = null)
    {
        var results = new List<CompletionItemResult>();

        try
        {
            // Wrap script code as if it were inside a class with the globals
            var (wrappedCode, adjustedPosition) = WrapScriptCode(code, cursorPosition);

            var references = new List<MetadataReference>(s_defaultReferences.Value);
            if (extraReferences != null)
            {
                foreach (var path in extraReferences)
                {
                    if (File.Exists(path))
                    {
                        try { references.Add(MetadataReference.CreateFromFile(path)); }
                        catch { /* skip invalid assemblies */ }
                    }
                }
            }

            var projectId = ProjectId.CreateNewId();
            var documentId = DocumentId.CreateNewId(projectId);

            var projectInfo = ProjectInfo.Create(
                projectId,
                VersionStamp.Create(),
                "ScriptCompletion",
                "ScriptCompletion",
                LanguageNames.CSharp,
                compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                parseOptions: new CSharpParseOptions(LanguageVersion.Latest))
                .WithMetadataReferences(references);

            var ws = s_workspace.Value;

            var solution = ws.CurrentSolution
                .AddProject(projectInfo)
                .AddDocument(documentId, "Script.cs", SourceText.From(wrappedCode));

            var document = solution.GetDocument(documentId);
            if (document == null) return results;

            var completionService = CompletionService.GetService(document);
            if (completionService == null) return results;

            var completions = await completionService.GetCompletionsAsync(document, adjustedPosition);
            if (completions == null) return results;

            foreach (var item in completions.ItemsList)
            {
                var kind = GetItemKind(item);
                results.Add(new CompletionItemResult
                {
                    Text = item.DisplayText,
                    DisplayText = item.DisplayTextSuffix != null
                        ? item.DisplayText + item.DisplayTextSuffix
                        : item.DisplayText,
                    Kind = kind
                });

                if (results.Count >= 200) break;
            }
        }
        catch
        {
            // Fail silently — completions are best-effort
        }

        return results;
    }

    /// <summary>
    /// Returns available namespace names for 'using' directive completions.
    /// Includes namespaces from extra referenced assemblies.
    /// </summary>
    public List<string> GetNamespaces(IReadOnlyList<string>? extraReferences = null)
    {
        var namespaces = new HashSet<string>(s_systemNamespaces.Value, StringComparer.Ordinal);

        if (extraReferences != null)
        {
            foreach (var path in extraReferences)
            {
                if (!File.Exists(path)) continue;
                try
                {
                    var asm = MetadataReference.CreateFromFile(path);
                    var compilation = CSharpCompilation.Create("ns",
                        references: [asm, .. s_defaultReferences.Value]);
                    var symbol = compilation.GetAssemblyOrModuleSymbol(asm) as IAssemblySymbol;
                    if (symbol != null)
                        CollectNamespaces(symbol.GlobalNamespace, namespaces);
                }
                catch { }
            }
        }

        return namespaces.Where(n => !string.IsNullOrEmpty(n)).OrderBy(n => n).ToList();
    }

    private static (string wrappedCode, int adjustedPosition) WrapScriptCode(string code, int cursorPosition)
    {
        var prefix =
            "using System;\n" +
            "using System.Collections.Generic;\n" +
            "using System.Linq;\n" +
            "using System.Threading;\n" +
            "using System.Threading.Tasks;\n" +
            "using System.Text;\n" +
            "using System.IO;\n" +
            "using System.Net.Http;\n\n" +
            "public class ScriptGlobals\n" +
            "{\n" +
            "    public object? Read(string variableName) => null;\n" +
            "    public void Write(string variableName, object value) { }\n" +
            "    public double ReadDouble(string variableName) => 0.0;\n" +
            "    public int ReadInt(string variableName) => 0;\n" +
            "    public bool ReadBool(string variableName) => false;\n" +
            "    public void Log(string message) { }\n" +
            "    public void OnChanged(string variableName, Action<object> handler) { }\n" +
            "}\n\n" +
            "public class Script : ScriptGlobals\n" +
            "{\n" +
            "    public void Run()\n" +
            "    {\n";

        var suffix = "\n    }\n}\n";

        var adjusted = prefix.Length + cursorPosition;
        return (prefix + code + suffix, adjusted);
    }

    private static string GetItemKind(Microsoft.CodeAnalysis.Completion.CompletionItem item)
    {
        var tags = item.Tags;
        if (tags.Contains("Method")) return "method";
        if (tags.Contains("Property")) return "property";
        if (tags.Contains("Field")) return "field";
        if (tags.Contains("Event")) return "event";
        if (tags.Contains("Class") || tags.Contains("Structure") || tags.Contains("Enum")) return "class";
        if (tags.Contains("Interface")) return "interface";
        if (tags.Contains("Namespace")) return "namespace";
        if (tags.Contains("Keyword")) return "keyword";
        if (tags.Contains("Local") || tags.Contains("Parameter")) return "variable";
        if (tags.Contains("EnumMember") || tags.Contains("Constant")) return "constant";
        return "text";
    }

    private static MetadataReference[] BuildDefaultReferences()
    {
        var refs = new Dictionary<string, MetadataReference>(StringComparer.OrdinalIgnoreCase);
        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        var required = new[]
        {
            "System.Runtime.dll",
            "System.Collections.dll",
            "System.Linq.dll",
            "System.Console.dll",
            "System.ObjectModel.dll",
            "System.Threading.dll",
            "System.Threading.Tasks.dll",
            "System.Text.RegularExpressions.dll",
            "System.Net.Http.dll",
            "System.IO.dll",
            "System.IO.FileSystem.dll",
            "System.Text.Encoding.dll",
            "System.Memory.dll",
            "netstandard.dll",
            "mscorlib.dll",
            "System.Private.CoreLib.dll"
        };

        foreach (var name in required)
        {
            var path = Path.Combine(runtimeDir, name);
            if (File.Exists(path))
                refs.TryAdd(name, MetadataReference.CreateFromFile(path));
        }

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (asm.IsDynamic || string.IsNullOrEmpty(asm.Location)) continue;
            var name = Path.GetFileName(asm.Location);
            refs.TryAdd(name, MetadataReference.CreateFromFile(asm.Location));
        }

        return refs.Values.ToArray();
    }

    private static List<string> BuildSystemNamespaces()
    {
        var namespaces = new HashSet<string>(StringComparer.Ordinal);

        foreach (var reference in s_defaultReferences.Value)
        {
            try
            {
                var compilation = CSharpCompilation.Create("ns", references: [reference]);
                if (reference is PortableExecutableReference peRef)
                {
                    var symbol = compilation.GetAssemblyOrModuleSymbol(peRef) as IAssemblySymbol;
                    if (symbol != null)
                        CollectNamespaces(symbol.GlobalNamespace, namespaces);
                }
            }
            catch { }
        }

        return namespaces.OrderBy(n => n).ToList();
    }

    private static void CollectNamespaces(INamespaceSymbol ns, HashSet<string> result)
    {
        var name = ns.ToDisplayString();
        if (!string.IsNullOrEmpty(name) && name != "<global namespace>")
        {
            if (ns.GetTypeMembers().Any(t => t.DeclaredAccessibility == Accessibility.Public))
                result.Add(name);
        }

        foreach (var child in ns.GetNamespaceMembers())
            CollectNamespaces(child, result);
    }

    public class CompletionItemResult
    {
        public string Text { get; set; } = "";
        public string? DisplayText { get; set; }
        public string Kind { get; set; } = "text";
    }
}
