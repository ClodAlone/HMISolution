using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.VisualBasic;

namespace ServerEditorWeb.Services;

/// <summary>Mock event args matching the real VariableChangedEventArgs in the Server project.</summary>
public class VariableChangedEventArgs : EventArgs
{
    public string VariableName { get; set; } = "";
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>Mock globals matching the real ScriptGlobals in the Server project.</summary>
public class ScriptGlobals
{
    public object? Read(string variableName) => null;
    public void Write(string variableName, object value) { }
    public double ReadDouble(string variableName) => 0.0;
    public int ReadInt(string variableName) => 0;
    public bool ReadBool(string variableName) => false;
    public void Log(string message) { }
    public void OnChanged(string variableName, Action<VariableChangedEventArgs> handler) { }
}

public class SyntaxCheckService
{
    private static readonly Lazy<List<MetadataReference>> _csharpReferences = new(BuildCSharpReferences);

    public (bool success, string message) CheckSyntax(string code, string language = "CSharp")
    {
        try
        {
            if (language.Equals("VB", StringComparison.OrdinalIgnoreCase)
                || language.Equals("VB.NET", StringComparison.OrdinalIgnoreCase)
                || language.Equals("VisualBasic", StringComparison.OrdinalIgnoreCase))
            {
                return CheckVbSyntax(code);
            }

            return CheckCSharpSyntax(code);
        }
        catch (Exception ex)
        {
            return (false, $"Error validating script: {ex.Message}");
        }
    }

    public (bool success, string message) CheckPlcSyntax(string code, string language = "ST")
    {
        try
        {
            if (language.Equals("IL", StringComparison.OrdinalIgnoreCase))
            {
                IlSyntaxChecker.Parse(code);
                return (true, "No syntax errors.");
            }

            if (language.Equals("LD", StringComparison.OrdinalIgnoreCase))
            {
                LdSyntaxChecker.Parse(code);
                return (true, "No syntax errors.");
            }

            StSyntaxChecker.Parse(code);
            return (true, "No syntax errors.");
        }
        catch (Exception ex)
        {
            return (false, $"Syntax error: {ex.Message}");
        }
    }

    private (bool success, string message) CheckCSharpSyntax(string code)
    {
        var options = ScriptOptions.Default
            .AddReferences(_csharpReferences.Value.Select(r => ((PortableExecutableReference)r).FilePath!).Distinct().ToArray())
            .AddReferences(typeof(ScriptGlobals).Assembly)
            .AddImports("System", "System.Collections.Generic", "System.Linq",
                         "ServerEditorWeb.Services");

        var script = CSharpScript.Create(code, options, typeof(ScriptGlobals));
        var diagnostics = script.Compile();

        var errors = diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (errors.Count > 0)
        {
            var errorText = string.Join("\n", errors.Select(d =>
                $"Line {d.Location.GetLineSpan().StartLinePosition.Line + 1}: {d.GetMessage()}"));
            return (false, $"Syntax errors found:\n{errorText}");
        }

        return (true, "No syntax errors.");
    }

    private (bool success, string message) CheckVbSyntax(string code)
    {
        var wrappedCode = $@"
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Module ScriptModule
    Public Sub Run(globals As Object)
        Dim Read As Func(Of String, Object) = Function(__varName__) Nothing
        Dim Write As Action(Of String, Object) = Sub(__varName__, __varValue__)
                                                  End Sub
        Dim OnChanged As Action(Of String, Action(Of Object)) = Sub(__varName__, __handler__)
                                                                 End Sub
{code}
    End Sub
End Module";

        var syntaxTree = VisualBasicSyntaxTree.ParseText(wrappedCode);
        var references = GetVbCompilationReferences();

        var compilation = VisualBasicCompilation.Create(
            "VbSyntaxCheck",
            [syntaxTree],
            references,
            new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optionStrict: OptionStrict.Off,
                optionInfer: true));

        var diagnostics = compilation.GetDiagnostics();
        var errors = diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (errors.Count > 0)
        {
            // Offset line numbers to account for the wrapper (9 lines added before user code)
            var errorText = string.Join("\n", errors.Select(d =>
            {
                var line = d.Location.GetLineSpan().StartLinePosition.Line - 9;
                return $"Line {(line > 0 ? line : 1)}: {d.GetMessage()}";
            }));
            return (false, $"Syntax errors found:\n{errorText}");
        }

        return (true, "No syntax errors.");
    }

    private static List<MetadataReference> GetVbCompilationReferences()
    {
        var refs = new Dictionary<string, MetadataReference>(StringComparer.OrdinalIgnoreCase);

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (asm.IsDynamic || string.IsNullOrEmpty(asm.Location)) continue;
            var name = Path.GetFileName(asm.Location);
            refs.TryAdd(name, MetadataReference.CreateFromFile(asm.Location));
        }

        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var requiredAssemblies = new[]
        {
            "Microsoft.VisualBasic.dll",
            "Microsoft.VisualBasic.Core.dll",
            "System.Runtime.dll",
            "System.Collections.dll",
            "System.Linq.dll",
            "System.Console.dll",
            "netstandard.dll",
            "mscorlib.dll"
        };

        foreach (var name in requiredAssemblies)
        {
            if (refs.ContainsKey(name)) continue;
            var path = Path.Combine(runtimeDir, name);
            if (File.Exists(path))
                refs[name] = MetadataReference.CreateFromFile(path);
        }

        return refs.Values.ToList();
    }

    private static List<MetadataReference> BuildCSharpReferences()
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
            "netstandard.dll",
            "mscorlib.dll"
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

        return refs.Values.ToList();
    }
}

/// <summary>
/// Lightweight IEC 61131-3 Structured Text syntax checker (tokenizer + parser)
/// used by the editor to validate PLC code without the full server runtime.
/// </summary>
internal static class StSyntaxChecker
{
    private enum TT
    {
        Id, Num, Str, Bool,
        Assign, Semi, Colon, Comma, LParen, RParen,
        Plus, Minus, Star, Slash, Mod,
        Eq, Neq, Lt, Gt, Le, Ge,
        And, Or, Xor, Not,
        If, Then, Elsif, Else, EndIf,
        For, To, By, Do, EndFor,
        While, EndWhile,
        Repeat, Until, EndRepeat,
        Case, Of, EndCase,
        Var, VarEnd,
        Exit, Return, True, False,
        DotDot, Eof
    }

    private record Tok(TT Type, string Text, int Line);

    private static readonly Dictionary<string, TT> _kw = new(StringComparer.OrdinalIgnoreCase)
    {
        ["IF"] = TT.If, ["THEN"] = TT.Then, ["ELSIF"] = TT.Elsif,
        ["ELSE"] = TT.Else, ["END_IF"] = TT.EndIf,
        ["FOR"] = TT.For, ["TO"] = TT.To, ["BY"] = TT.By,
        ["DO"] = TT.Do, ["END_FOR"] = TT.EndFor,
        ["WHILE"] = TT.While, ["END_WHILE"] = TT.EndWhile,
        ["REPEAT"] = TT.Repeat, ["UNTIL"] = TT.Until, ["END_REPEAT"] = TT.EndRepeat,
        ["CASE"] = TT.Case, ["OF"] = TT.Of, ["END_CASE"] = TT.EndCase,
        ["VAR"] = TT.Var, ["END_VAR"] = TT.VarEnd,
        ["AND"] = TT.And, ["OR"] = TT.Or, ["XOR"] = TT.Xor, ["NOT"] = TT.Not,
        ["MOD"] = TT.Mod,
        ["TRUE"] = TT.True, ["FALSE"] = TT.False,
        ["EXIT"] = TT.Exit, ["RETURN"] = TT.Return,
    };

    private static List<Tok> Tokenize(string code)
    {
        var tokens = new List<Tok>();
        int i = 0, line = 1;
        while (i < code.Length)
        {
            if (char.IsWhiteSpace(code[i])) { if (code[i] == '\n') line++; i++; continue; }
            if (i + 1 < code.Length && code[i] == '/' && code[i + 1] == '/') { while (i < code.Length && code[i] != '\n') i++; continue; }
            if (i + 1 < code.Length && code[i] == '(' && code[i + 1] == '*')
            {
                i += 2;
                while (i + 1 < code.Length && !(code[i] == '*' && code[i + 1] == ')')) { if (code[i] == '\n') line++; i++; }
                if (i + 1 < code.Length) i += 2; continue;
            }
            if (code[i] == '\'') { i++; int s = i; while (i < code.Length && code[i] != '\'') i++; tokens.Add(new(TT.Str, code[s..i], line)); if (i < code.Length) i++; continue; }
            if (char.IsDigit(code[i]) || (code[i] == '.' && i + 1 < code.Length && char.IsDigit(code[i + 1])))
            {
                int s = i;
                while (i < code.Length && (char.IsDigit(code[i]) || code[i] == '.')) i++;
                if (i < code.Length && (code[i] == 'e' || code[i] == 'E')) { i++; if (i < code.Length && (code[i] == '+' || code[i] == '-')) i++; while (i < code.Length && char.IsDigit(code[i])) i++; }
                tokens.Add(new(TT.Num, code[s..i], line)); continue;
            }
            if (char.IsLetter(code[i]) || code[i] == '_')
            {
                int s = i;
                while (i < code.Length && (char.IsLetterOrDigit(code[i]) || code[i] == '_' || code[i] == '.')) i++;
                var t = code[s..i];
                if (_kw.TryGetValue(t, out var k)) tokens.Add(new(k, t, line));
                else tokens.Add(new(TT.Id, t, line));
                continue;
            }
            if (i + 1 < code.Length)
            {
                var two = code.Substring(i, 2);
                if (two == ":=") { tokens.Add(new(TT.Assign, ":=", line)); i += 2; continue; }
                if (two == "<>") { tokens.Add(new(TT.Neq, "<>", line)); i += 2; continue; }
                if (two == "<=") { tokens.Add(new(TT.Le, "<=", line)); i += 2; continue; }
                if (two == ">=") { tokens.Add(new(TT.Ge, ">=", line)); i += 2; continue; }
                if (two == "..") { tokens.Add(new(TT.DotDot, "..", line)); i += 2; continue; }
            }
            var tt = code[i] switch
            {
                ';' => TT.Semi, ':' => TT.Colon, ',' => TT.Comma,
                '(' => TT.LParen, ')' => TT.RParen,
                '+' => TT.Plus, '-' => TT.Minus, '*' => TT.Star, '/' => TT.Slash,
                '=' => TT.Eq, '<' => TT.Lt, '>' => TT.Gt,
                _ => throw new InvalidOperationException($"Unexpected character '{code[i]}' at line {line}")
            };
            tokens.Add(new(tt, code[i].ToString(), line)); i++;
        }
        tokens.Add(new(TT.Eof, "", line));
        return tokens;
    }

    public static void Parse(string code)
    {
        var tokens = Tokenize(code);
        int pos = 0;

        Tok Peek() => pos < tokens.Count ? tokens[pos] : new(TT.Eof, "", 0);
        Tok Adv() { var t = Peek(); pos++; return t; }
        void Expect(TT type) { var t = Adv(); if (t.Type != type) throw new InvalidOperationException($"Expected {type} but got {t.Type} ('{t.Text}') at line {t.Line}"); }

        // Optional PROGRAM wrapper
        if (Peek().Type == TT.Id && Peek().Text.Equals("PROGRAM", StringComparison.OrdinalIgnoreCase))
        { Adv(); if (Peek().Type == TT.Id) Adv(); }

        // VAR blocks
        while (Peek().Type == TT.Var)
        {
            Adv();
            while (Peek().Type != TT.VarEnd && Peek().Type != TT.Eof)
            {
                Adv(); // name
                while (Peek().Type == TT.Comma) { Adv(); Adv(); } // additional comma-separated names
                Expect(TT.Colon);
                Adv(); // type
                if (Peek().Type == TT.Assign) { Adv(); ParseExpr(); }
                Expect(TT.Semi);
            }
            Expect(TT.VarEnd);
        }

        // Statements
        while (Peek().Type != TT.Eof)
        {
            if (Peek().Type == TT.Id && Peek().Text.Equals("END_PROGRAM", StringComparison.OrdinalIgnoreCase)) { Adv(); continue; }
            ParseStmt();
        }

        void ParseStmt()
        {
            var c = Peek();
            switch (c.Type)
            {
                case TT.If: ParseIf(); break;
                case TT.For: ParseForStmt(); break;
                case TT.While: ParseWhileStmt(); break;
                case TT.Repeat: ParseRepeatStmt(); break;
                case TT.Case: ParseCaseStmt(); break;
                case TT.Exit: Adv(); Expect(TT.Semi); break;
                case TT.Return: Adv(); Expect(TT.Semi); break;
                case TT.Var: Adv(); while (Peek().Type != TT.VarEnd && Peek().Type != TT.Eof) { Adv(); while (Peek().Type == TT.Comma) { Adv(); Adv(); } Expect(TT.Colon); Adv(); if (Peek().Type == TT.Assign) { Adv(); ParseExpr(); } Expect(TT.Semi); } Expect(TT.VarEnd); break;
                case TT.Semi: Adv(); break;
                case TT.Id: ParseAssignOrCall(); break;
                default: throw new InvalidOperationException($"Unexpected token '{c.Text}' ({c.Type}) at line {c.Line}");
            }
        }

        void ParseAssignOrCall()
        {
            Adv(); // identifier
            if (Peek().Type == TT.LParen)
            {
                Adv(); // (
                while (Peek().Type != TT.RParen && Peek().Type != TT.Eof) { ParseExpr(); if (Peek().Type == TT.Comma) Adv(); }
                Expect(TT.RParen);
                Expect(TT.Semi);
            }
            else
            {
                Expect(TT.Assign);
                ParseExpr();
                Expect(TT.Semi);
            }
        }

        void ParseIf()
        {
            Expect(TT.If); ParseExpr(); Expect(TT.Then);
            ParseStmtList(TT.Elsif, TT.Else, TT.EndIf);
            while (Peek().Type == TT.Elsif) { Adv(); ParseExpr(); Expect(TT.Then); ParseStmtList(TT.Elsif, TT.Else, TT.EndIf); }
            if (Peek().Type == TT.Else) { Adv(); ParseStmtList(TT.EndIf); }
            Expect(TT.EndIf);
            if (Peek().Type == TT.Semi) Adv();
        }

        void ParseForStmt()
        {
            Expect(TT.For); Adv(); Expect(TT.Assign); ParseExpr(); Expect(TT.To); ParseExpr();
            if (Peek().Type == TT.By) { Adv(); ParseExpr(); }
            Expect(TT.Do); ParseStmtList(TT.EndFor); Expect(TT.EndFor);
            if (Peek().Type == TT.Semi) Adv();
        }

        void ParseWhileStmt()
        {
            Expect(TT.While); ParseExpr(); Expect(TT.Do); ParseStmtList(TT.EndWhile); Expect(TT.EndWhile);
            if (Peek().Type == TT.Semi) Adv();
        }

        void ParseRepeatStmt()
        {
            Expect(TT.Repeat); ParseStmtList(TT.Until); Expect(TT.Until); ParseExpr(); Expect(TT.Semi);
        }

        void ParseCaseStmt()
        {
            Expect(TT.Case); ParseExpr(); Expect(TT.Of);
            while (Peek().Type != TT.EndCase && Peek().Type != TT.Else && Peek().Type != TT.Eof)
            {
                ParseExpr();
                while (Peek().Type == TT.Comma) { Adv(); ParseExpr(); }
                Expect(TT.Colon);
                ParseStmtList(TT.EndCase, TT.Else, TT.Num, TT.Id);
            }
            if (Peek().Type == TT.Else) { Adv(); ParseStmtList(TT.EndCase); }
            Expect(TT.EndCase);
            if (Peek().Type == TT.Semi) Adv();
        }

        void ParseStmtList(params TT[] terminators)
        {
            while (!terminators.Contains(Peek().Type) && Peek().Type != TT.Eof)
                ParseStmt();
        }

        void ParseExpr() => ParseOr();
        void ParseOr() { ParseXor(); while (Peek().Type == TT.Or) { Adv(); ParseXor(); } }
        void ParseXor() { ParseAnd(); while (Peek().Type == TT.Xor) { Adv(); ParseAnd(); } }
        void ParseAnd() { ParseComp(); while (Peek().Type == TT.And) { Adv(); ParseComp(); } }
        void ParseComp() { ParseAdd(); while (Peek().Type is TT.Eq or TT.Neq or TT.Lt or TT.Gt or TT.Le or TT.Ge) { Adv(); ParseAdd(); } }
        void ParseAdd() { ParseMul(); while (Peek().Type is TT.Plus or TT.Minus) { Adv(); ParseMul(); } }
        void ParseMul() { ParseUnary(); while (Peek().Type is TT.Star or TT.Slash or TT.Mod) { Adv(); ParseUnary(); } }
        void ParseUnary()
        {
            if (Peek().Type == TT.Not) { Adv(); ParseUnary(); return; }
            if (Peek().Type == TT.Minus) { Adv(); ParsePrimary(); return; }
            ParsePrimary();
        }
        void ParsePrimary()
        {
            var t = Peek();
            switch (t.Type)
            {
                case TT.Num: Adv(); break;
                case TT.Str: Adv(); break;
                case TT.True: Adv(); break;
                case TT.False: Adv(); break;
                case TT.LParen: Adv(); ParseExpr(); Expect(TT.RParen); break;
                case TT.Id:
                    Adv();
                    if (Peek().Type == TT.LParen)
                    {
                        Adv();
                        while (Peek().Type != TT.RParen && Peek().Type != TT.Eof) { ParseExpr(); if (Peek().Type == TT.Comma) Adv(); }
                        Expect(TT.RParen);
                    }
                    break;
                default: throw new InvalidOperationException($"Unexpected token '{t.Text}' ({t.Type}) at line {t.Line} in expression");
            }
        }
    }
}

/// <summary>
/// Lightweight IEC 61131-3 Instruction List (IL) syntax checker.
/// Validates that each line is a valid IL instruction with known operators.
/// </summary>
internal static class IlSyntaxChecker
{
    private static readonly HashSet<string> _validOps = new(StringComparer.OrdinalIgnoreCase)
    {
        "LD", "LDN", "ST", "STN", "S", "R",
        "AND", "ANDN", "OR", "ORN", "XOR", "NOT",
        "ADD", "SUB", "MUL", "DIV", "MOD",
        "GT", "GE", "EQ", "NE", "LE", "LT",
        "JMP", "JMPC", "JMPCN",
        "CAL", "CALC", "CALCN",
        "RET", "RETC", "RETCN",
        "NOP", "ABS", "SQRT"
    };

    public static void Parse(string code)
    {
        var lines = code.Split('\n');
        int lineNum = 0;
        bool inBlockComment = false;

        foreach (var rawLine in lines)
        {
            lineNum++;
            var line = rawLine.Trim();

            // Handle block comments
            if (inBlockComment)
            {
                if (line.Contains("*)")) inBlockComment = false;
                continue;
            }
            if (line.StartsWith("(*"))
            {
                if (!line.Contains("*)")) inBlockComment = true;
                continue;
            }

            if (string.IsNullOrEmpty(line)) continue;
            if (line.StartsWith("//")) continue;

            // Strip inline comments
            var ci = line.IndexOf("(*");
            if (ci >= 0) line = line[..ci].Trim();
            ci = line.IndexOf("//");
            if (ci >= 0) line = line[..ci].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // Check for label
            var colonIdx = line.IndexOf(':');
            if (colonIdx > 0 && colonIdx < line.Length - 1 && !line[..colonIdx].Contains(' '))
            {
                line = line[(colonIdx + 1)..].Trim();
                if (string.IsNullOrEmpty(line)) continue; // Label-only line
            }

            var parts = line.Split([' ', '\t'], 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            var op = parts[0].ToUpperInvariant();
            if (!_validOps.Contains(op))
                throw new InvalidOperationException($"Unknown IL operator '{parts[0]}' at line {lineNum}");

            // Operators that require an operand
            if (op is "LD" or "LDN" or "ST" or "STN" or "S" or "R"
                or "AND" or "ANDN" or "OR" or "ORN" or "XOR"
                or "ADD" or "SUB" or "MUL" or "DIV" or "MOD"
                or "GT" or "GE" or "EQ" or "NE" or "LE" or "LT"
                or "JMP" or "JMPC" or "JMPCN"
                or "CAL" or "CALC" or "CALCN")
            {
                if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                    throw new InvalidOperationException($"IL operator '{op}' requires an operand at line {lineNum}");
            }
        }

        if (inBlockComment)
            throw new InvalidOperationException("Unterminated block comment (* ... *)");
    }
}

/// <summary>
/// Lightweight IEC 61131-3 Ladder Diagram (LD) text syntax checker.
/// Validates RUNG/END_RUNG structure and known element types.
/// </summary>
internal static class LdSyntaxChecker
{
    private static readonly HashSet<string> _validElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "CONTACT", "COIL", "COIL_S", "COIL_R", "COMPARE",
        "MOVE", "ADD", "SUB", "MUL", "DIV", "TIMER", "COUNTER"
    };

    public static void Parse(string code)
    {
        var lines = code.Split('\n');
        int lineNum = 0;
        bool inRung = false;
        bool inBlockComment = false;

        foreach (var rawLine in lines)
        {
            lineNum++;
            var line = rawLine.Trim();

            if (inBlockComment)
            {
                if (line.Contains("*)")) inBlockComment = false;
                continue;
            }
            if (line.StartsWith("(*"))
            {
                if (!line.Contains("*)")) inBlockComment = true;
                continue;
            }

            if (string.IsNullOrEmpty(line)) continue;
            if (line.StartsWith("//")) continue;

            var ci = line.IndexOf("(*");
            if (ci >= 0) line = line[..ci].Trim();
            ci = line.IndexOf("//");
            if (ci >= 0) line = line[..ci].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var parts = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
            var keyword = parts[0].ToUpperInvariant();

            switch (keyword)
            {
                case "RUNG":
                    if (inRung) throw new InvalidOperationException($"Nested RUNG at line {lineNum} — missing END_RUNG");
                    inRung = true;
                    break;

                case "END_RUNG":
                    if (!inRung) throw new InvalidOperationException($"END_RUNG without matching RUNG at line {lineNum}");
                    inRung = false;
                    break;

                case "CONTACT":
                    if (!inRung) throw new InvalidOperationException($"CONTACT outside RUNG at line {lineNum}");
                    if (parts.Length < 3) throw new InvalidOperationException($"CONTACT requires modifier (NO/NC) and variable at line {lineNum}");
                    if (parts[1].ToUpperInvariant() is not ("NO" or "NC"))
                        throw new InvalidOperationException($"CONTACT modifier must be NO or NC, got '{parts[1]}' at line {lineNum}");
                    break;

                case "COIL" or "COIL_S" or "COIL_R":
                    if (!inRung) throw new InvalidOperationException($"{keyword} outside RUNG at line {lineNum}");
                    if (parts.Length < 2) throw new InvalidOperationException($"{keyword} requires a variable at line {lineNum}");
                    break;

                case "COMPARE":
                    if (!inRung) throw new InvalidOperationException($"COMPARE outside RUNG at line {lineNum}");
                    if (parts.Length < 4) throw new InvalidOperationException($"COMPARE requires operator, variable, and value at line {lineNum}");
                    if (parts[1].ToUpperInvariant() is not ("GT" or "GE" or "LT" or "LE" or "EQ" or "NE"))
                        throw new InvalidOperationException($"Unknown COMPARE operator '{parts[1]}' at line {lineNum}");
                    break;

                case "MOVE" or "ADD" or "SUB" or "MUL" or "DIV":
                    if (!inRung) throw new InvalidOperationException($"{keyword} outside RUNG at line {lineNum}");
                    if (parts.Length < 3) throw new InvalidOperationException($"{keyword} requires source and destination at line {lineNum}");
                    break;

                case "TIMER" or "COUNTER":
                    if (!inRung) throw new InvalidOperationException($"{keyword} outside RUNG at line {lineNum}");
                    if (parts.Length < 3) throw new InvalidOperationException($"{keyword} requires variable and preset at line {lineNum}");
                    break;

                default:
                    if (!_validElements.Contains(keyword))
                        throw new InvalidOperationException($"Unknown ladder element '{parts[0]}' at line {lineNum}");
                    break;
            }
        }

        if (inRung)
            throw new InvalidOperationException("Missing END_RUNG at end of program");

        if (inBlockComment)
            throw new InvalidOperationException("Unterminated block comment (* ... *)");
    }
}
