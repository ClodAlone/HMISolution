// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer
{
    /// <summary>
    /// Manages IEC 61131-3 Structured Text PLC programs.
    /// Each enabled program runs cyclically at its configured interval,
    /// reading and writing OPC variables through the node manager.
    /// </summary>
    public class PlcManager : IDisposable
    {
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly List<PlcProgramRunner> _runners = new();
        private readonly CancellationTokenSource _cts = new();

        public PlcManager(SimpleFileServerNodeManager nodeManager)
        {
            _nodeManager = nodeManager;
        }

        public void Initialize(List<PlcProgramConfig> programs)
        {
            foreach (var program in programs)
            {
                DiagnosticsCollector.Instance.Register("PlcProgram", program.Name, program.Enabled);
                if (program.Enabled)
                {
                    var runner = new PlcProgramRunner(program, _nodeManager, _cts.Token);
                    _runners.Add(runner);
                    runner.Start();
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            foreach (var r in _runners) r.Stop();
            _cts.Dispose();
        }
    }

    internal class PlcProgramRunner
    {
        private readonly PlcProgramConfig _config;
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly CancellationToken _token;
        private Task? _task;
        private StProgram? _compiledSt;
        private List<IlInstruction>? _compiledIl;
        private LdProgram? _compiledLd;

        private int _consecutiveErrors;
        private const int MaxBackoffMs = 30_000;

        public PlcProgramRunner(PlcProgramConfig config, SimpleFileServerNodeManager nodeManager, CancellationToken token)
        {
            _config = config;
            _nodeManager = nodeManager;
            _token = token;
        }

        private bool IsIl => _config.Language.Equals("IL", StringComparison.OrdinalIgnoreCase);
        private bool IsLd => _config.Language.Equals("LD", StringComparison.OrdinalIgnoreCase);

        public void Start()
        {
            _task = Task.Run(async () =>
            {
                try
                {
                    if (IsIl)
                        _compiledIl = IlParser.Parse(_config.Code);
                    else if (IsLd)
                        _compiledLd = LdParser.Parse(_config.Code);
                    else
                        _compiledSt = StParser.Parse(_config.Code);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "PLC '{Name}': compilation failed: {Message}", _config.Name, ex.Message);
                    DiagnosticsCollector.Instance.RecordCycle("PlcProgram", _config.Name, 0, error: ex.Message);
                    return;
                }

                long cycleCount = 0;
                // Seed diagnostics so the program is visible immediately
                // (before the first cycle completes — avoids stuck "Connecting"
                // when a breakpoint pauses the very first execution).
                RecordDebugSnapshot(new PlcDebugContext(), 0, "Running", null);

                while (!_token.IsCancellationRequested)
                {
                    cycleCount++;
                    var editorConnected = DiagnosticsCollector.Instance.IsEditorConnected;
                    var debug = editorConnected ? new PlcDebugContext() : null;
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    try
                    {
                        if (IsIl)
                            IlInterpreter.Execute(_compiledIl!, _nodeManager, debug);
                        else if (IsLd)
                            LdInterpreter.Execute(_compiledLd!, _nodeManager, debug);
                        else
                            StInterpreter.Execute(_compiledSt!, _nodeManager, debug, _config.Name, _token);

                        sw.Stop();
                        DiagnosticsCollector.Instance.RecordCycle("PlcProgram", _config.Name, sw.Elapsed.TotalMilliseconds);
                        if (editorConnected) RecordDebugSnapshot(debug!, cycleCount, "Running", null);
                        _consecutiveErrors = 0;
                    }
                    catch (Exception ex)
                    {
                        sw.Stop();
                        _consecutiveErrors++;
                        DiagnosticsCollector.Instance.RecordCycle("PlcProgram", _config.Name, sw.Elapsed.TotalMilliseconds, error: ex.Message);
                        if (editorConnected) RecordDebugSnapshot(debug!, cycleCount, "Error", ex.Message);
                        if (_consecutiveErrors == 1 || _consecutiveErrors % 10 == 0)
                            Log.Error(ex, "PLC '{Name}': execution error: {Message}", _config.Name, ex.Message);
                    }

                    try
                    {
                        var delay = _config.IntervalMs;
                        if (_consecutiveErrors > 1)
                            delay = Math.Min(delay * (1 << Math.Min(_consecutiveErrors - 1, 10)), MaxBackoffMs);

                        await Task.Delay(delay, _token);
                    }
                    catch (OperationCanceledException) { break; }
                }
            }, _token);
        }

        private void RecordDebugSnapshot(PlcDebugContext debug, long cycleCount, string status, string? error)
        {
            var info = new SharedModels.ProgramDebugInfo
            {
                Category = "PlcProgram",
                Name = _config.Name,
                CycleCount = cycleCount,
                Status = status,
                LastError = error
            };
            // Merge reads (OPC inputs) and writes (OPC outputs) plus locals
            foreach (var kvp in debug.OpcReads)
                info.Variables[kvp.Key] = kvp.Value;
            foreach (var kvp in debug.OpcWrites)
                info.Variables[kvp.Key] = kvp.Value;
            info.ExecutedLines.AddRange(debug.ExecutedLines);
            info.WriteLines.AddRange(debug.WriteLines);
            info.LastExecutedLine = debug.LastExecutedLine;
            foreach (var kvp in debug.LineAnnotations)
                info.LineAnnotations[kvp.Key] = string.Join(", ", kvp.Value);
            info.DebugSession = ScriptDebugger.Instance.GetSessionInfo(_config.Name);
            DiagnosticsCollector.Instance.RecordDebug(info);
        }

        public void Stop() { }
    }

    /// <summary>
    /// Tracks OPC variable reads/writes during a PLC execution cycle for debug visualization.
    /// </summary>
    internal class PlcDebugContext
    {
        public Dictionary<string, string> OpcReads { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> OpcWrites { get; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<int> ExecutedLines { get; } = new();
        public List<int> WriteLines { get; } = new();
        /// <summary>The most recently executed 0-based line number (-1 if none).</summary>
        public int LastExecutedLine { get; private set; } = -1;
        /// <summary>Per-line annotations: 0-based line -> list of "name=value" strings.</summary>
        public Dictionary<int, List<string>> LineAnnotations { get; } = new();

        public void RecordRead(string varName, object? value)
        {
            OpcReads[varName] = value?.ToString() ?? "null";
        }

        public void RecordWrite(string varName, object? value)
        {
            OpcWrites[varName] = value?.ToString() ?? "null";
        }

        public void RecordExecutedLine(int line)
        {
            if (line >= 0)
            {
                ExecutedLines.Add(line);
                LastExecutedLine = line;
            }
        }

        public void RecordWriteLine(int line)
        {
            if (line >= 0) WriteLines.Add(line);
        }

        public void Annotate(int line, string name, object? value)
        {
            if (line < 0) return;
            if (!LineAnnotations.TryGetValue(line, out var list))
            {
                list = new List<string>();
                LineAnnotations[line] = list;
            }
            var text = name + " = " + (value?.ToString() ?? "null");
            // Replace existing annotation for same variable name
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].StartsWith(name + " = ", StringComparison.OrdinalIgnoreCase))
                {
                    list[i] = text;
                    return;
                }
            }
            list.Add(text);
        }
    }

    // ─── IEC 61131-3 Structured Text AST ────────────────────

    internal enum StNodeType
    {
        Assignment, If, For, While, Repeat, Case, Exit, Return,
        VarDeclaration, FunctionCall, Comment
    }

    internal abstract class StStatement { public int SourceLine { get; set; } }

    internal class StAssignment : StStatement
    {
        public string Variable { get; set; } = "";
        public StExpression Value { get; set; } = null!;
    }

    internal class StIf : StStatement
    {
        public StExpression Condition { get; set; } = null!;
        public List<StStatement> ThenBlock { get; set; } = new();
        public List<(StExpression Condition, List<StStatement> Block)> ElsifBlocks { get; set; } = new();
        public List<StStatement> ElseBlock { get; set; } = new();
    }

    internal class StFor : StStatement
    {
        public string Variable { get; set; } = "";
        public StExpression From { get; set; } = null!;
        public StExpression To { get; set; } = null!;
        public StExpression? By { get; set; }
        public List<StStatement> Body { get; set; } = new();
    }

    internal class StWhile : StStatement
    {
        public StExpression Condition { get; set; } = null!;
        public List<StStatement> Body { get; set; } = new();
    }

    internal class StRepeat : StStatement
    {
        public StExpression Condition { get; set; } = null!;
        public List<StStatement> Body { get; set; } = new();
    }

    internal class StCase : StStatement
    {
        public StExpression Expression { get; set; } = null!;
        public List<(List<StExpression> Values, List<StStatement> Body)> Branches { get; set; } = new();
        public List<StStatement> ElseBlock { get; set; } = new();
    }

    internal class StExit : StStatement { }
    internal class StReturn : StStatement { }

    internal class StVarDeclaration : StStatement
    {
        public string Name { get; set; } = "";
        public string TypeName { get; set; } = "REAL";
        public StExpression? InitialValue { get; set; }
    }

    internal class StFunctionCallStatement : StStatement
    {
        public string FunctionName { get; set; } = "";
        public List<StExpression> Arguments { get; set; } = new();
    }

    // ─── Expressions ────────────────────────────────────────

    internal abstract class StExpression { }

    internal class StLiteral : StExpression
    {
        public object Value { get; set; } = 0.0;
    }

    internal class StVariableRef : StExpression
    {
        public string Name { get; set; } = "";
    }

    internal class StBinaryOp : StExpression
    {
        public string Operator { get; set; } = "";
        public StExpression Left { get; set; } = null!;
        public StExpression Right { get; set; } = null!;
    }

    internal class StUnaryOp : StExpression
    {
        public string Operator { get; set; } = "";
        public StExpression Operand { get; set; } = null!;
    }

    internal class StFunctionCall : StExpression
    {
        public string FunctionName { get; set; } = "";
        public List<StExpression> Arguments { get; set; } = new();
    }

    internal class StProgram
    {
        public List<StVarDeclaration> Variables { get; set; } = new();
        public List<StStatement> Statements { get; set; } = new();
    }

    // ─── Tokenizer ──────────────────────────────────────────

    internal enum TokenType
    {
        Identifier, Number, StringLiteral, BoolLiteral,
        Assign,     // :=
        Semicolon,  // ;
        Colon,      // :
        Comma,      // ,
        LParen, RParen,
        Plus, Minus, Star, Slash, Mod,
        Eq, Neq, Lt, Gt, Le, Ge,
        And, Or, Xor, Not,
        If, Then, Elsif, Else, EndIf,
        For, To, By, Do, EndFor,
        While, EndWhile,
        Repeat, Until, EndRepeat,
        Case, Of, EndCase,
        Var, VarEnd, // VAR / END_VAR
        Exit, Return,
        True, False,
        DotDot, // ..
        Eof
    }

    internal record Token(TokenType Type, string Text, int Line);

    internal static class StTokenizer
    {
        private static readonly Dictionary<string, TokenType> _keywords = new(StringComparer.OrdinalIgnoreCase)
        {
            ["IF"] = TokenType.If, ["THEN"] = TokenType.Then, ["ELSIF"] = TokenType.Elsif,
            ["ELSE"] = TokenType.Else, ["END_IF"] = TokenType.EndIf,
            ["FOR"] = TokenType.For, ["TO"] = TokenType.To, ["BY"] = TokenType.By,
            ["DO"] = TokenType.Do, ["END_FOR"] = TokenType.EndFor,
            ["WHILE"] = TokenType.While, ["END_WHILE"] = TokenType.EndWhile,
            ["REPEAT"] = TokenType.Repeat, ["UNTIL"] = TokenType.Until, ["END_REPEAT"] = TokenType.EndRepeat,
            ["CASE"] = TokenType.Case, ["OF"] = TokenType.Of, ["END_CASE"] = TokenType.EndCase,
            ["VAR"] = TokenType.Var, ["END_VAR"] = TokenType.VarEnd,
            ["AND"] = TokenType.And, ["OR"] = TokenType.Or, ["XOR"] = TokenType.Xor, ["NOT"] = TokenType.Not,
            ["MOD"] = TokenType.Mod,
            ["TRUE"] = TokenType.True, ["FALSE"] = TokenType.False,
            ["EXIT"] = TokenType.Exit, ["RETURN"] = TokenType.Return,
        };

        public static List<Token> Tokenize(string code)
        {
            var tokens = new List<Token>();
            int i = 0, line = 1;

            while (i < code.Length)
            {
                // Whitespace
                if (char.IsWhiteSpace(code[i]))
                {
                    if (code[i] == '\n') line++;
                    i++;
                    continue;
                }

                // Line comment //
                if (i + 1 < code.Length && code[i] == '/' && code[i + 1] == '/')
                {
                    while (i < code.Length && code[i] != '\n') i++;
                    continue;
                }

                // Block comment (* ... *)
                if (i + 1 < code.Length && code[i] == '(' && code[i + 1] == '*')
                {
                    i += 2;
                    while (i + 1 < code.Length && !(code[i] == '*' && code[i + 1] == ')'))
                    {
                        if (code[i] == '\n') line++;
                        i++;
                    }
                    if (i + 1 < code.Length) i += 2;
                    continue;
                }

                // String literal
                if (code[i] == '\'')
                {
                    i++;
                    int start = i;
                    while (i < code.Length && code[i] != '\'') i++;
                    tokens.Add(new Token(TokenType.StringLiteral, code[start..i], line));
                    if (i < code.Length) i++; // skip closing '
                    continue;
                }

                // Number (integer or real)
                if (char.IsDigit(code[i]) || (code[i] == '.' && i + 1 < code.Length && char.IsDigit(code[i + 1])))
                {
                    int start = i;
                    while (i < code.Length && (char.IsDigit(code[i]) || code[i] == '.')) i++;
                    // Handle exponent
                    if (i < code.Length && (code[i] == 'e' || code[i] == 'E'))
                    {
                        i++;
                        if (i < code.Length && (code[i] == '+' || code[i] == '-')) i++;
                        while (i < code.Length && char.IsDigit(code[i])) i++;
                    }
                    tokens.Add(new Token(TokenType.Number, code[start..i], line));
                    continue;
                }

                // Identifier or keyword (may contain underscores)
                if (char.IsLetter(code[i]) || code[i] == '_')
                {
                    int start = i;
                    while (i < code.Length && (char.IsLetterOrDigit(code[i]) || code[i] == '_' || code[i] == '.')) i++;
                    var text = code[start..i];

                    // Check for multi-word keywords (END_IF, END_FOR, etc.)
                    if (_keywords.TryGetValue(text, out var kwType))
                        tokens.Add(new Token(kwType, text, line));
                    else if (text.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                        tokens.Add(new Token(TokenType.True, text, line));
                    else if (text.Equals("FALSE", StringComparison.OrdinalIgnoreCase))
                        tokens.Add(new Token(TokenType.False, text, line));
                    else
                        tokens.Add(new Token(TokenType.Identifier, text, line));
                    continue;
                }

                // Two-character operators
                if (i + 1 < code.Length)
                {
                    var two = code.Substring(i, 2);
                    switch (two)
                    {
                        case ":=": tokens.Add(new Token(TokenType.Assign, ":=", line)); i += 2; continue;
                        case "<>": tokens.Add(new Token(TokenType.Neq, "<>", line)); i += 2; continue;
                        case "<=": tokens.Add(new Token(TokenType.Le, "<=", line)); i += 2; continue;
                        case ">=": tokens.Add(new Token(TokenType.Ge, ">=", line)); i += 2; continue;
                        case "..": tokens.Add(new Token(TokenType.DotDot, "..", line)); i += 2; continue;
                    }
                }

                // Single-character operators
                switch (code[i])
                {
                    case ';': tokens.Add(new Token(TokenType.Semicolon, ";", line)); break;
                    case ':': tokens.Add(new Token(TokenType.Colon, ":", line)); break;
                    case ',': tokens.Add(new Token(TokenType.Comma, ",", line)); break;
                    case '(': tokens.Add(new Token(TokenType.LParen, "(", line)); break;
                    case ')': tokens.Add(new Token(TokenType.RParen, ")", line)); break;
                    case '+': tokens.Add(new Token(TokenType.Plus, "+", line)); break;
                    case '-': tokens.Add(new Token(TokenType.Minus, "-", line)); break;
                    case '*': tokens.Add(new Token(TokenType.Star, "*", line)); break;
                    case '/': tokens.Add(new Token(TokenType.Slash, "/", line)); break;
                    case '=': tokens.Add(new Token(TokenType.Eq, "=", line)); break;
                    case '<': tokens.Add(new Token(TokenType.Lt, "<", line)); break;
                    case '>': tokens.Add(new Token(TokenType.Gt, ">", line)); break;
                    default:
                        throw new InvalidOperationException($"Unexpected character '{code[i]}' at line {line}");
                }
                i++;
            }

            tokens.Add(new Token(TokenType.Eof, "", line));
            return tokens;
        }
    }

    // ─── Parser ─────────────────────────────────────────────

    internal static class StParser
    {
        public static StProgram Parse(string code)
        {
            var tokens = StTokenizer.Tokenize(code);
            var pos = 0;
            var program = new StProgram();

            // Parse optional PROGRAM ... END_PROGRAM wrapper
            if (Peek(tokens, pos).Type == TokenType.Identifier &&
                Peek(tokens, pos).Text.Equals("PROGRAM", StringComparison.OrdinalIgnoreCase))
            {
                pos++; // PROGRAM
                if (Peek(tokens, pos).Type == TokenType.Identifier) pos++; // program name
            }

            // Parse VAR blocks
            while (Peek(tokens, pos).Type == TokenType.Var)
            {
                pos++; // VAR
                while (Peek(tokens, pos).Type != TokenType.VarEnd && Peek(tokens, pos).Type != TokenType.Eof)
                {
                    var decls = ParseVarDeclaration(tokens, ref pos);
                    program.Variables.AddRange(decls);
                }
                Expect(tokens, ref pos, TokenType.VarEnd);
            }

            // Parse statements
            while (Peek(tokens, pos).Type != TokenType.Eof)
            {
                // Skip optional END_PROGRAM
                if (Peek(tokens, pos).Type == TokenType.Identifier &&
                    Peek(tokens, pos).Text.Equals("END_PROGRAM", StringComparison.OrdinalIgnoreCase))
                {
                    pos++;
                    continue;
                }

                var stmt = ParseStatement(tokens, ref pos);
                if (stmt != null)
                    program.Statements.Add(stmt);
            }

            return program;
        }

        private static Token Peek(List<Token> tokens, int pos) =>
            pos < tokens.Count ? tokens[pos] : new Token(TokenType.Eof, "", 0);

        private static Token Advance(List<Token> tokens, ref int pos)
        {
            var t = Peek(tokens, pos);
            pos++;
            return t;
        }

        private static void Expect(List<Token> tokens, ref int pos, TokenType type)
        {
            var t = Advance(tokens, ref pos);
            if (t.Type != type)
                throw new InvalidOperationException($"Expected {type} but got {t.Type} ('{t.Text}') at line {t.Line}");
        }

        private static List<StVarDeclaration> ParseVarDeclaration(List<Token> tokens, ref int pos)
        {
            // Collect one or more comma-separated names: "a, b, c : REAL;"
            var names = new List<string>();
            names.Add(Advance(tokens, ref pos).Text);
            while (Peek(tokens, pos).Type == TokenType.Comma)
            {
                pos++; // skip comma
                names.Add(Advance(tokens, ref pos).Text);
            }

            Expect(tokens, ref pos, TokenType.Colon);

            var typeName = Advance(tokens, ref pos).Text.ToUpperInvariant();

            StExpression? initValue = null;
            if (Peek(tokens, pos).Type == TokenType.Assign)
            {
                pos++; // :=
                initValue = ParseExpression(tokens, ref pos);
            }

            Expect(tokens, ref pos, TokenType.Semicolon);

            return names.Select(n => new StVarDeclaration
            {
                Name = n,
                TypeName = typeName,
                InitialValue = initValue
            }).ToList();
        }

        private static StStatement? ParseStatement(List<Token> tokens, ref int pos)
        {
            var current = Peek(tokens, pos);
            var sourceLine = current.Line;

            StStatement? result;
            switch (current.Type)
            {
                case TokenType.If:
                    result = ParseIf(tokens, ref pos); break;
                case TokenType.For:
                    result = ParseFor(tokens, ref pos); break;
                case TokenType.While:
                    result = ParseWhile(tokens, ref pos); break;
                case TokenType.Repeat:
                    result = ParseRepeat(tokens, ref pos); break;
                case TokenType.Case:
                    result = ParseCase(tokens, ref pos); break;
                case TokenType.Exit:
                    pos++;
                    Expect(tokens, ref pos, TokenType.Semicolon);
                    result = new StExit(); break;
                case TokenType.Return:
                    pos++;
                    Expect(tokens, ref pos, TokenType.Semicolon);
                    result = new StReturn(); break;
                case TokenType.Var:
                    // Inline VAR block (e.g. mid-code local declarations)
                    pos++; // skip VAR
                    while (Peek(tokens, pos).Type != TokenType.VarEnd && Peek(tokens, pos).Type != TokenType.Eof)
                    {
                        var inlineDecls = ParseVarDeclaration(tokens, ref pos);
                        // Inline VAR declarations are treated as no-op statements
                    }
                    Expect(tokens, ref pos, TokenType.VarEnd);
                    return null;
                case TokenType.Semicolon:
                    pos++; // empty statement
                    return null;
                case TokenType.Identifier:
                    result = ParseAssignmentOrCall(tokens, ref pos); break;
                default:
                    throw new InvalidOperationException($"Unexpected token '{current.Text}' ({current.Type}) at line {current.Line}");
            }

            result.SourceLine = sourceLine - 1; // Convert 1-based token line to 0-based
            return result;
        }

        private static StStatement ParseAssignmentOrCall(List<Token> tokens, ref int pos)
        {
            var name = Advance(tokens, ref pos).Text;

            if (Peek(tokens, pos).Type == TokenType.LParen)
            {
                // Function call statement
                pos++; // (
                var args = new List<StExpression>();
                while (Peek(tokens, pos).Type != TokenType.RParen && Peek(tokens, pos).Type != TokenType.Eof)
                {
                    args.Add(ParseExpression(tokens, ref pos));
                    if (Peek(tokens, pos).Type == TokenType.Comma) pos++;
                }
                Expect(tokens, ref pos, TokenType.RParen);
                Expect(tokens, ref pos, TokenType.Semicolon);
                return new StFunctionCallStatement { FunctionName = name, Arguments = args };
            }

            Expect(tokens, ref pos, TokenType.Assign);
            var value = ParseExpression(tokens, ref pos);
            Expect(tokens, ref pos, TokenType.Semicolon);
            return new StAssignment { Variable = name, Value = value };
        }

        private static StIf ParseIf(List<Token> tokens, ref int pos)
        {
            Expect(tokens, ref pos, TokenType.If);
            var ifStmt = new StIf { Condition = ParseExpression(tokens, ref pos) };
            Expect(tokens, ref pos, TokenType.Then);

            ifStmt.ThenBlock = ParseStatementList(tokens, ref pos,
                TokenType.Elsif, TokenType.Else, TokenType.EndIf);

            while (Peek(tokens, pos).Type == TokenType.Elsif)
            {
                pos++;
                var cond = ParseExpression(tokens, ref pos);
                Expect(tokens, ref pos, TokenType.Then);
                var block = ParseStatementList(tokens, ref pos,
                    TokenType.Elsif, TokenType.Else, TokenType.EndIf);
                ifStmt.ElsifBlocks.Add((cond, block));
            }

            if (Peek(tokens, pos).Type == TokenType.Else)
            {
                pos++;
                ifStmt.ElseBlock = ParseStatementList(tokens, ref pos, TokenType.EndIf);
            }

            Expect(tokens, ref pos, TokenType.EndIf);
            // Optional semicolon after END_IF
            if (Peek(tokens, pos).Type == TokenType.Semicolon) pos++;
            return ifStmt;
        }

        private static StFor ParseFor(List<Token> tokens, ref int pos)
        {
            Expect(tokens, ref pos, TokenType.For);
            var forStmt = new StFor();
            forStmt.Variable = Advance(tokens, ref pos).Text;
            Expect(tokens, ref pos, TokenType.Assign);
            forStmt.From = ParseExpression(tokens, ref pos);
            Expect(tokens, ref pos, TokenType.To);
            forStmt.To = ParseExpression(tokens, ref pos);

            if (Peek(tokens, pos).Type == TokenType.By)
            {
                pos++;
                forStmt.By = ParseExpression(tokens, ref pos);
            }

            Expect(tokens, ref pos, TokenType.Do);
            forStmt.Body = ParseStatementList(tokens, ref pos, TokenType.EndFor);
            Expect(tokens, ref pos, TokenType.EndFor);
            if (Peek(tokens, pos).Type == TokenType.Semicolon) pos++;
            return forStmt;
        }

        private static StWhile ParseWhile(List<Token> tokens, ref int pos)
        {
            Expect(tokens, ref pos, TokenType.While);
            var whileStmt = new StWhile { Condition = ParseExpression(tokens, ref pos) };
            Expect(tokens, ref pos, TokenType.Do);
            whileStmt.Body = ParseStatementList(tokens, ref pos, TokenType.EndWhile);
            Expect(tokens, ref pos, TokenType.EndWhile);
            if (Peek(tokens, pos).Type == TokenType.Semicolon) pos++;
            return whileStmt;
        }

        private static StRepeat ParseRepeat(List<Token> tokens, ref int pos)
        {
            Expect(tokens, ref pos, TokenType.Repeat);
            var repeatStmt = new StRepeat();
            repeatStmt.Body = ParseStatementList(tokens, ref pos, TokenType.Until);
            Expect(tokens, ref pos, TokenType.Until);
            repeatStmt.Condition = ParseExpression(tokens, ref pos);
            Expect(tokens, ref pos, TokenType.Semicolon);
            return repeatStmt;
        }

        private static StCase ParseCase(List<Token> tokens, ref int pos)
        {
            Expect(tokens, ref pos, TokenType.Case);
            var caseStmt = new StCase { Expression = ParseExpression(tokens, ref pos) };
            Expect(tokens, ref pos, TokenType.Of);

            while (Peek(tokens, pos).Type != TokenType.EndCase && Peek(tokens, pos).Type != TokenType.Else &&
                   Peek(tokens, pos).Type != TokenType.Eof)
            {
                var values = new List<StExpression>();
                values.Add(ParseExpression(tokens, ref pos));
                while (Peek(tokens, pos).Type == TokenType.Comma)
                {
                    pos++;
                    values.Add(ParseExpression(tokens, ref pos));
                }
                Expect(tokens, ref pos, TokenType.Colon);
                var body = ParseCaseBranchBody(tokens, ref pos);

                caseStmt.Branches.Add((values, body));
            }

            if (Peek(tokens, pos).Type == TokenType.Else)
            {
                pos++;
                caseStmt.ElseBlock = ParseStatementList(tokens, ref pos, TokenType.EndCase);
            }

            Expect(tokens, ref pos, TokenType.EndCase);
            if (Peek(tokens, pos).Type == TokenType.Semicolon) pos++;
            return caseStmt;
        }

        private static List<StStatement> ParseStatementList(List<Token> tokens, ref int pos, params TokenType[] terminators)
        {
            var stmts = new List<StStatement>();
            while (!terminators.Contains(Peek(tokens, pos).Type) && Peek(tokens, pos).Type != TokenType.Eof)
            {
                var stmt = ParseStatement(tokens, ref pos);
                if (stmt != null) stmts.Add(stmt);
            }
            return stmts;
        }

        /// <summary>
        /// Parses the body of a CASE branch, stopping when the next case label, ELSE, or END_CASE is found.
        /// A case label is a Number or Identifier followed by ':' (but not ':=').
        /// </summary>
        private static List<StStatement> ParseCaseBranchBody(List<Token> tokens, ref int pos)
        {
            var stmts = new List<StStatement>();
            while (Peek(tokens, pos).Type != TokenType.EndCase &&
                   Peek(tokens, pos).Type != TokenType.Else &&
                   Peek(tokens, pos).Type != TokenType.Eof)
            {
                if (IsCaseLabel(tokens, pos))
                    break;

                var stmt = ParseStatement(tokens, ref pos);
                if (stmt != null) stmts.Add(stmt);
            }
            return stmts;
        }

        /// <summary>
        /// Returns true when the token at <paramref name="pos"/> starts a new CASE label,
        /// i.e. it is a Number/Identifier followed by ':' (not ':='), or by ',' or '..' (multi-value/range labels).
        /// </summary>
        private static bool IsCaseLabel(List<Token> tokens, int pos)
        {
            var cur = Peek(tokens, pos);
            if (cur.Type != TokenType.Number && cur.Type != TokenType.Identifier)
                return false;

            var next = Peek(tokens, pos + 1);
            // number/identifier directly followed by ':' is a case label
            if (next.Type == TokenType.Colon)
                return true;
            // comma means multi-value label like "1, 2, 3:"
            if (next.Type == TokenType.Comma)
                return true;
            // range like "1..5:"
            if (next.Type == TokenType.DotDot)
                return true;
            return false;
        }

        // ─── Expression parsing (precedence climbing) ──────

        internal static StExpression ParseExpression(List<Token> tokens, ref int pos)
        {
            return ParseOr(tokens, ref pos);
        }

        private static StExpression ParseOr(List<Token> tokens, ref int pos)
        {
            var left = ParseXor(tokens, ref pos);
            while (Peek(tokens, pos).Type == TokenType.Or)
            {
                pos++;
                left = new StBinaryOp { Operator = "OR", Left = left, Right = ParseXor(tokens, ref pos) };
            }
            return left;
        }

        private static StExpression ParseXor(List<Token> tokens, ref int pos)
        {
            var left = ParseAnd(tokens, ref pos);
            while (Peek(tokens, pos).Type == TokenType.Xor)
            {
                pos++;
                left = new StBinaryOp { Operator = "XOR", Left = left, Right = ParseAnd(tokens, ref pos) };
            }
            return left;
        }

        private static StExpression ParseAnd(List<Token> tokens, ref int pos)
        {
            var left = ParseComparison(tokens, ref pos);
            while (Peek(tokens, pos).Type == TokenType.And)
            {
                pos++;
                left = new StBinaryOp { Operator = "AND", Left = left, Right = ParseComparison(tokens, ref pos) };
            }
            return left;
        }

        private static StExpression ParseComparison(List<Token> tokens, ref int pos)
        {
            var left = ParseAddSub(tokens, ref pos);
            while (Peek(tokens, pos).Type is TokenType.Eq or TokenType.Neq or TokenType.Lt or TokenType.Gt or TokenType.Le or TokenType.Ge)
            {
                var op = Advance(tokens, ref pos).Text;
                left = new StBinaryOp { Operator = op, Left = left, Right = ParseAddSub(tokens, ref pos) };
            }
            return left;
        }

        private static StExpression ParseAddSub(List<Token> tokens, ref int pos)
        {
            var left = ParseMulDiv(tokens, ref pos);
            while (Peek(tokens, pos).Type is TokenType.Plus or TokenType.Minus)
            {
                var op = Advance(tokens, ref pos).Text;
                left = new StBinaryOp { Operator = op, Left = left, Right = ParseMulDiv(tokens, ref pos) };
            }
            return left;
        }

        private static StExpression ParseMulDiv(List<Token> tokens, ref int pos)
        {
            var left = ParseUnary(tokens, ref pos);
            while (Peek(tokens, pos).Type is TokenType.Star or TokenType.Slash or TokenType.Mod)
            {
                var op = Advance(tokens, ref pos).Text;
                left = new StBinaryOp { Operator = op, Left = left, Right = ParseUnary(tokens, ref pos) };
            }
            return left;
        }

        private static StExpression ParseUnary(List<Token> tokens, ref int pos)
        {
            if (Peek(tokens, pos).Type == TokenType.Not)
            {
                pos++;
                return new StUnaryOp { Operator = "NOT", Operand = ParseUnary(tokens, ref pos) };
            }
            if (Peek(tokens, pos).Type == TokenType.Minus)
            {
                pos++;
                return new StUnaryOp { Operator = "-", Operand = ParsePrimary(tokens, ref pos) };
            }
            return ParsePrimary(tokens, ref pos);
        }

        private static StExpression ParsePrimary(List<Token> tokens, ref int pos)
        {
            var t = Peek(tokens, pos);

            switch (t.Type)
            {
                case TokenType.Number:
                    pos++;
                    if (t.Text.Contains('.') || t.Text.Contains('e') || t.Text.Contains('E'))
                        return new StLiteral { Value = double.Parse(t.Text, CultureInfo.InvariantCulture) };
                    return new StLiteral { Value = long.Parse(t.Text, CultureInfo.InvariantCulture) };

                case TokenType.StringLiteral:
                    pos++;
                    return new StLiteral { Value = t.Text };

                case TokenType.True:
                    pos++;
                    return new StLiteral { Value = true };

                case TokenType.False:
                    pos++;
                    return new StLiteral { Value = false };

                case TokenType.LParen:
                    pos++;
                    var expr = ParseExpression(tokens, ref pos);
                    Expect(tokens, ref pos, TokenType.RParen);
                    return expr;

                case TokenType.Identifier:
                    pos++;
                    // Function call?
                    if (Peek(tokens, pos).Type == TokenType.LParen)
                    {
                        pos++; // (
                        var args = new List<StExpression>();
                        while (Peek(tokens, pos).Type != TokenType.RParen && Peek(tokens, pos).Type != TokenType.Eof)
                        {
                            args.Add(ParseExpression(tokens, ref pos));
                            if (Peek(tokens, pos).Type == TokenType.Comma) pos++;
                        }
                        Expect(tokens, ref pos, TokenType.RParen);
                        return new StFunctionCall { FunctionName = t.Text, Arguments = args };
                    }
                    return new StVariableRef { Name = t.Text };

                default:
                    throw new InvalidOperationException($"Unexpected token '{t.Text}' ({t.Type}) at line {t.Line} in expression");
            }
        }
    }

    // ─── Interpreter ────────────────────────────────────────

    internal class ExitException : Exception { }
    internal class ReturnException : Exception { }

    internal static class StInterpreter
    {
        public static void Execute(StProgram program, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug = null, string? programName = null, CancellationToken ct = default)
        {
            var locals = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            // Initialize local variables
            foreach (var decl in program.Variables)
            {
                object? initVal = decl.TypeName.ToUpperInvariant() switch
                {
                    "BOOL" => false,
                    "INT" or "DINT" or "SINT" or "LINT" => 0L,
                    "UINT" or "UDINT" or "USINT" or "ULINT" or "WORD" or "DWORD" => 0L,
                    "REAL" or "LREAL" => 0.0,
                    "STRING" => "",
                    "TIME" => 0L,
                    _ => 0.0
                };

                if (decl.InitialValue != null)
                    initVal = EvalExpression(decl.InitialValue, locals, nodeManager, debug);

                locals[decl.Name] = initVal;
            }

            try
            {
                ExecuteBlock(program.Statements, locals, nodeManager, debug, programName, ct);
            }
            catch (ReturnException) { }

            // Capture local variable values for debug visualization
            if (debug != null)
            {
                foreach (var kvp in locals)
                    debug.OpcReads["[local] " + kvp.Key] = kvp.Value?.ToString() ?? "null";
            }
        }

        private static void ExecuteBlock(List<StStatement> statements, Dictionary<string, object?> locals, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug, string? programName = null, CancellationToken ct = default)
        {
            foreach (var stmt in statements)
            {
                ExecuteStatement(stmt, locals, nodeManager, debug, programName, ct);
            }
        }

        private static void ExecuteStatement(StStatement stmt, Dictionary<string, object?> locals, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug, string? programName = null, CancellationToken ct = default)
        {
            debug?.RecordExecutedLine(stmt.SourceLine);

            // Breakpoint check — only build vars and check when an active debug session exists
            if (!string.IsNullOrEmpty(programName) && stmt.SourceLine >= 0
                && ScriptDebugger.Instance.HasActiveSession(programName))
            {
                var allVars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (debug != null)
                {
                    foreach (var kvp in debug.OpcReads) allVars["[read] " + kvp.Key] = kvp.Value;
                    foreach (var kvp in debug.OpcWrites) allVars["[write] " + kvp.Key] = kvp.Value;
                }
                foreach (var kvp in locals)
                    allVars["[local] " + kvp.Key] = kvp.Value?.ToString() ?? "null";
                ScriptDebugger.Instance.CheckBreakpoint(programName, stmt.SourceLine, allVars, ct);
            }

            switch (stmt)
            {
                case StAssignment assign:
                {
                    var value = EvalExpression(assign.Value, locals, nodeManager, debug);
                    debug?.Annotate(assign.SourceLine, assign.Variable, value);
                    // If the variable name contains a dot, it's an OPC variable path
                    if (assign.Variable.Contains('.'))
                    {
                        nodeManager.WriteVariable(assign.Variable, value ?? 0);
                        debug?.RecordWrite(assign.Variable, value);
                        debug?.RecordWriteLine(assign.SourceLine);
                    }
                    else if (locals.ContainsKey(assign.Variable))
                    {
                        locals[assign.Variable] = value;
                    }
                    else
                    {
                        // Try as OPC variable, fall back to local
                        try
                        {
                            nodeManager.WriteVariable(assign.Variable, value ?? 0);
                            debug?.RecordWrite(assign.Variable, value);
                            debug?.RecordWriteLine(assign.SourceLine);
                        }
                        catch
                        {
                            locals[assign.Variable] = value;
                        }
                    }
                    break;
                }

                case StIf ifStmt:
                {
                    if (IsTrue(EvalExpression(ifStmt.Condition, locals, nodeManager, debug)))
                    {
                        ExecuteBlock(ifStmt.ThenBlock, locals, nodeManager, debug, programName, ct);
                    }
                    else
                    {
                        bool handled = false;
                        foreach (var (cond, block) in ifStmt.ElsifBlocks)
                        {
                            if (IsTrue(EvalExpression(cond, locals, nodeManager, debug)))
                            {
                                ExecuteBlock(block, locals, nodeManager, debug, programName, ct);
                                handled = true;
                                break;
                            }
                        }
                        if (!handled && ifStmt.ElseBlock.Count > 0)
                            ExecuteBlock(ifStmt.ElseBlock, locals, nodeManager, debug, programName, ct);
                    }
                    break;
                }

                case StFor forStmt:
                {
                    var from = ToDouble(EvalExpression(forStmt.From, locals, nodeManager, debug));
                    var to = ToDouble(EvalExpression(forStmt.To, locals, nodeManager, debug));
                    var by = forStmt.By != null ? ToDouble(EvalExpression(forStmt.By, locals, nodeManager, debug)) : 1.0;
                    if (by == 0) break;

                    locals[forStmt.Variable] = from;
                    try
                    {
                        if (by > 0)
                        {
                            for (double i = from; i <= to; i += by)
                            {
                                locals[forStmt.Variable] = i;
                                ExecuteBlock(forStmt.Body, locals, nodeManager, debug, programName, ct);
                            }
                        }
                        else
                        {
                            for (double i = from; i >= to; i += by)
                            {
                                locals[forStmt.Variable] = i;
                                ExecuteBlock(forStmt.Body, locals, nodeManager, debug, programName, ct);
                            }
                        }
                    }
                    catch (ExitException) { }
                    break;
                }

                case StWhile whileStmt:
                {
                    int safety = 100_000;
                    try
                    {
                        while (IsTrue(EvalExpression(whileStmt.Condition, locals, nodeManager, debug)) && safety-- > 0)
                        {
                            ExecuteBlock(whileStmt.Body, locals, nodeManager, debug, programName, ct);
                        }
                    }
                    catch (ExitException) { }
                    break;
                }

                case StRepeat repeatStmt:
                {
                    int safety = 100_000;
                    try
                    {
                        do
                        {
                            ExecuteBlock(repeatStmt.Body, locals, nodeManager, debug, programName, ct);
                        } while (!IsTrue(EvalExpression(repeatStmt.Condition, locals, nodeManager, debug)) && safety-- > 0);
                    }
                    catch (ExitException) { }
                    break;
                }

                case StCase caseStmt:
                {
                    var val = EvalExpression(caseStmt.Expression, locals, nodeManager, debug);
                    bool matched = false;
                    foreach (var (values, body) in caseStmt.Branches)
                    {
                        foreach (var v in values)
                        {
                            var caseVal = EvalExpression(v, locals, nodeManager, debug);
                            if (AreEqual(val, caseVal))
                            {
                                ExecuteBlock(body, locals, nodeManager, debug, programName, ct);
                                matched = true;
                                break;
                            }
                        }
                        if (matched) break;
                    }
                    if (!matched && caseStmt.ElseBlock.Count > 0)
                        ExecuteBlock(caseStmt.ElseBlock, locals, nodeManager, debug, programName, ct);
                    break;
                }

                case StExit:
                    throw new ExitException();

                case StReturn:
                    throw new ReturnException();

                case StFunctionCallStatement callStmt:
                    EvalFunctionCall(callStmt.FunctionName, callStmt.Arguments, locals, nodeManager, debug);
                    break;
            }
        }

        internal static object? EvalExpression(StExpression expr, Dictionary<string, object?> locals, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug = null)
        {
            switch (expr)
            {
                case StLiteral lit:
                    return lit.Value;

                case StVariableRef varRef:
                    if (locals.TryGetValue(varRef.Name, out var localVal))
                        return localVal;
                    // Try reading from OPC
                    try
                    {
                        var opcVal = nodeManager.ReadVariable(varRef.Name);
                        debug?.RecordRead(varRef.Name, opcVal);
                        return opcVal;
                    }
                    catch { return null; }

                case StBinaryOp bin:
                {
                    var left = EvalExpression(bin.Left, locals, nodeManager, debug);
                    var right = EvalExpression(bin.Right, locals, nodeManager, debug);
                    return EvalBinaryOp(bin.Operator, left, right);
                }

                case StUnaryOp un:
                {
                    var operand = EvalExpression(un.Operand, locals, nodeManager, debug);
                    return un.Operator switch
                    {
                        "NOT" => !IsTrue(operand),
                        "-" => -ToDouble(operand),
                        _ => operand
                    };
                }

                case StFunctionCall call:
                    return EvalFunctionCall(call.FunctionName, call.Arguments, locals, nodeManager, debug);

                default:
                    return null;
            }
        }

        private static object? EvalBinaryOp(string op, object? left, object? right)
        {
            // Boolean operations
            if (op is "AND" or "OR" or "XOR")
            {
                bool l = IsTrue(left), r = IsTrue(right);
                return op switch
                {
                    "AND" => l && r,
                    "OR" => l || r,
                    "XOR" => l ^ r,
                    _ => false
                };
            }

            // String concatenation
            if (left is string ls && right is string rs && op == "+")
                return ls + rs;

            // Numeric operations
            double lv = ToDouble(left), rv = ToDouble(right);
            return op switch
            {
                "+" => lv + rv,
                "-" => lv - rv,
                "*" => lv * rv,
                "/" => rv != 0 ? lv / rv : 0.0,
                "MOD" => rv != 0 ? lv % rv : 0.0,
                "=" => lv == rv,
                "<>" => lv != rv,
                "<" => lv < rv,
                ">" => lv > rv,
                "<=" => lv <= rv,
                ">=" => lv >= rv,
                _ => 0.0
            };
        }

        private static object? EvalFunctionCall(string name, List<StExpression> args,
            Dictionary<string, object?> locals, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug = null)
        {
            var uName = name.ToUpperInvariant();
            var evaluated = args.Select(a => EvalExpression(a, locals, nodeManager, debug)).ToList();

            return uName switch
            {
                "ABS" => Math.Abs(ToDouble(evaluated.FirstOrDefault())),
                "SQRT" => Math.Sqrt(ToDouble(evaluated.FirstOrDefault())),
                "SIN" => Math.Sin(ToDouble(evaluated.FirstOrDefault())),
                "COS" => Math.Cos(ToDouble(evaluated.FirstOrDefault())),
                "TAN" => Math.Tan(ToDouble(evaluated.FirstOrDefault())),
                "ASIN" => Math.Asin(ToDouble(evaluated.FirstOrDefault())),
                "ACOS" => Math.Acos(ToDouble(evaluated.FirstOrDefault())),
                "ATAN" => Math.Atan(ToDouble(evaluated.FirstOrDefault())),
                "ATAN2" => Math.Atan2(ToDouble(evaluated.ElementAtOrDefault(0)), ToDouble(evaluated.ElementAtOrDefault(1))),
                "EXP" => Math.Exp(ToDouble(evaluated.FirstOrDefault())),
                "LN" => Math.Log(ToDouble(evaluated.FirstOrDefault())),
                "LOG" => EvalLog(name, evaluated),
                "POW" or "EXPT" => Math.Pow(ToDouble(evaluated.ElementAtOrDefault(0)), ToDouble(evaluated.ElementAtOrDefault(1))),
                "MIN" => Math.Min(ToDouble(evaluated.ElementAtOrDefault(0)), ToDouble(evaluated.ElementAtOrDefault(1))),
                "MAX" => Math.Max(ToDouble(evaluated.ElementAtOrDefault(0)), ToDouble(evaluated.ElementAtOrDefault(1))),
                "LIMIT" => Math.Max(ToDouble(evaluated.ElementAtOrDefault(0)),
                           Math.Min(ToDouble(evaluated.ElementAtOrDefault(1)), ToDouble(evaluated.ElementAtOrDefault(2)))),
                "SEL" => IsTrue(evaluated.ElementAtOrDefault(0)) ? evaluated.ElementAtOrDefault(2) : evaluated.ElementAtOrDefault(1),
                "MUX" => MuxSelect(evaluated),
                "TRUNC" => (double)(long)ToDouble(evaluated.FirstOrDefault()),
                "ROUND" => Math.Round(ToDouble(evaluated.FirstOrDefault())),
                "FLOOR" => Math.Floor(ToDouble(evaluated.FirstOrDefault())),
                "CEIL" => Math.Ceiling(ToDouble(evaluated.FirstOrDefault())),
                "BOOL_TO_INT" or "BOOL_TO_REAL" => IsTrue(evaluated.FirstOrDefault()) ? 1.0 : 0.0,
                "INT_TO_REAL" or "DINT_TO_REAL" => ToDouble(evaluated.FirstOrDefault()),
                "REAL_TO_INT" or "REAL_TO_DINT" => (double)(long)ToDouble(evaluated.FirstOrDefault()),
                "READ" or "READDOUBLE" => EvalRead(evaluated, nodeManager, debug),
                "READBOOL" => EvalReadBool(evaluated, nodeManager, debug),
                "READINT" => EvalReadInt(evaluated, nodeManager, debug),
                "WRITE" => EvalWrite(evaluated, nodeManager, debug),
                "RANDOM" => Random.Shared.NextDouble(),
                "PID" => EvalPid(evaluated),
                _ => throw new InvalidOperationException($"Unknown function: {name}")
            };
        }

        private static object? EvalRead(List<object?> evaluated, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug)
        {
            var path = evaluated.FirstOrDefault()?.ToString() ?? "";
            var val = nodeManager.ReadVariable(path);
            debug?.RecordRead(path, val);
            return ToDouble(val);
        }

        private static object? EvalReadBool(List<object?> evaluated, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug)
        {
            var path = evaluated.FirstOrDefault()?.ToString() ?? "";
            var val = nodeManager.ReadVariable(path);
            debug?.RecordRead(path, val);
            return IsTrue(val);
        }

        private static object? EvalReadInt(List<object?> evaluated, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug)
        {
            var path = evaluated.FirstOrDefault()?.ToString() ?? "";
            var val = nodeManager.ReadVariable(path);
            debug?.RecordRead(path, val);
            return (double)(long)ToDouble(val);
        }

        private static object? EvalWrite(List<object?> evaluated, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug)
        {
            var path = evaluated.ElementAtOrDefault(0)?.ToString() ?? "";
            var value = evaluated.ElementAtOrDefault(1);
            nodeManager.WriteVariable(path, value ?? 0.0);
            debug?.RecordWrite(path, value);
            return null;
        }

        // ─── PID Function Block State ──────────────────────────
        // PID(instanceName, pv, sp, kp, ki, kd, outMin, outMax)
        // Returns the clamped PID output. State (integral, prevError) persists by instanceName.
        private static readonly ConcurrentDictionary<string, (double integral, double prevError)> _pidStates = new();

        private static object? EvalPid(List<object?> evaluated)
        {
            // Args: instanceName, pv, sp, kp, ki, kd, outMin, outMax
            var name = evaluated.ElementAtOrDefault(0)?.ToString() ?? "pid0";
            double pv     = ToDouble(evaluated.ElementAtOrDefault(1));
            double sp     = ToDouble(evaluated.ElementAtOrDefault(2));
            double kp     = ToDouble(evaluated.ElementAtOrDefault(3));
            double ki     = ToDouble(evaluated.ElementAtOrDefault(4));
            double kd     = ToDouble(evaluated.ElementAtOrDefault(5));
            double outMin = evaluated.Count > 6 ? ToDouble(evaluated.ElementAtOrDefault(6)) : 0.0;
            double outMax = evaluated.Count > 7 ? ToDouble(evaluated.ElementAtOrDefault(7)) : 100.0;

            var state = _pidStates.GetOrAdd(name, _ => (0.0, 0.0));

            double error = sp - pv;

            // Integral with anti-windup
            double integral = state.integral + error;
            if (ki != 0)
            {
                if (integral * ki > outMax) integral = outMax / ki;
                if (integral * ki < outMin) integral = outMin / ki;
            }

            // Derivative
            double derivative = error - state.prevError;

            // Output
            double output = kp * error + ki * integral + kd * derivative;

            // Clamp
            if (output > outMax) output = outMax;
            if (output < outMin) output = outMin;

            _pidStates[name] = (integral, error);

            return output;
        }

        /// <summary>
        /// Handles the LOG/Log ambiguity: case-sensitive 'Log' with a string arg is diagnostic logging;
        /// otherwise it is the IEC 61131-3 LOG (base-10 logarithm).
        /// </summary>
        private static object? EvalLog(string originalName, List<object?> evaluated)
        {
            if (originalName == "Log" && evaluated.FirstOrDefault() is string msg)
            {
                Serilog.Log.Information("[PLC Script] {Message}", msg);
                return null;
            }
            return Math.Log10(ToDouble(evaluated.FirstOrDefault()));
        }

        private static object? MuxSelect(List<object?> args)
        {
            if (args.Count < 2) return null;
            var idx = (int)ToDouble(args[0]);
            var i = Math.Max(0, Math.Min(idx, args.Count - 2)) + 1;
            return args[i];
        }

        internal static bool IsTrue(object? value) => value switch
        {
            bool b => b,
            double d => d != 0.0,
            long l => l != 0,
            int i => i != 0,
            string s => !string.IsNullOrEmpty(s),
            _ => value != null
        };

        internal static double ToDouble(object? value) => value switch
        {
            double d => d,
            long l => l,
            int i => i,
            float f => f,
            bool b => b ? 1.0 : 0.0,
            string s => double.TryParse(s, CultureInfo.InvariantCulture, out var r) ? r : 0.0,
            _ => Convert.ToDouble(value ?? 0)
        };

        private static bool AreEqual(object? a, object? b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            try { return ToDouble(a) == ToDouble(b); }
            catch { return Equals(a, b); }
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  IEC 61131-3 Instruction List (IL)
    // ═══════════════════════════════════════════════════════════

    internal class IlInstruction
    {
        public string? Label { get; set; }
        public string Operator { get; set; } = "";
        public string? Operand { get; set; }
        /// <summary>0-based source line number for debug annotations.</summary>
        public int SourceLine { get; set; } = -1;
    }

    internal static class IlParser
    {
        public static List<IlInstruction> Parse(string code)
        {
            var instructions = new List<IlInstruction>();
            var lines = code.Split('\n');
            int lineNum = 0;

            foreach (var rawLine in lines)
            {
                lineNum++;
                var line = rawLine.Trim();

                // Skip empty lines and comments
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("(*"))
                {
                    // Block comment on a single line
                    if (line.Contains("*)")) continue;
                    continue;
                }
                if (line.StartsWith("//")) continue;

                // Strip inline comments
                var commentIdx = line.IndexOf("(*");
                if (commentIdx >= 0) line = line[..commentIdx].Trim();
                commentIdx = line.IndexOf("//");
                if (commentIdx >= 0) line = line[..commentIdx].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var instr = new IlInstruction { SourceLine = lineNum - 1 };

                // Check for label (ends with ':')
                var colonIdx = line.IndexOf(':');
                if (colonIdx > 0 && !line[..colonIdx].Contains(' '))
                {
                    instr.Label = line[..colonIdx].Trim();
                    line = line[(colonIdx + 1)..].Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        instructions.Add(instr);
                        continue;
                    }
                }

                // Parse operator and operand
                var parts = line.Split([' ', '\t'], 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                instr.Operator = parts[0].ToUpperInvariant();
                if (parts.Length > 1)
                    instr.Operand = parts[1].Trim();

                instructions.Add(instr);
            }

            return instructions;
        }
    }

    internal static class IlInterpreter
    {
        public static void Execute(List<IlInstruction> instructions, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug = null)
        {
            double accumulator = 0.0;
            var locals = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            // Build label index
            var labelIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i].Label != null)
                    labelIndex[instructions[i].Label!] = i;
            }

            int pc = 0;
            int safety = 100_000;

            while (pc < instructions.Count && safety-- > 0)
            {
                var instr = instructions[pc];
                var op = instr.Operator;
                var operand = instr.Operand;
                pc++;

                if (string.IsNullOrEmpty(op)) continue;

                debug?.RecordExecutedLine(instr.SourceLine);

                switch (op)
                {
                    case "LD":
                        accumulator = ResolveValue(operand, locals, nodeManager);
                        debug?.Annotate(instr.SourceLine, operand ?? "acc", accumulator);
                        break;
                    case "LDN":
                        accumulator = ResolveValue(operand, locals, nodeManager) != 0.0 ? 0.0 : 1.0;
                        debug?.Annotate(instr.SourceLine, operand ?? "acc", accumulator);
                        break;
                    case "ST":
                        StoreValue(operand, accumulator, locals, nodeManager);
                        debug?.Annotate(instr.SourceLine, operand ?? "acc", accumulator);
                        debug?.RecordWrite(operand ?? "", accumulator);
                        debug?.RecordWriteLine(instr.SourceLine);
                        break;
                    case "STN":
                    {
                        var stVal = accumulator != 0.0 ? 0.0 : 1.0;
                        StoreValue(operand, stVal, locals, nodeManager);
                        debug?.Annotate(instr.SourceLine, operand ?? "acc", stVal);
                        debug?.RecordWrite(operand ?? "", stVal);
                        debug?.RecordWriteLine(instr.SourceLine);
                        break;
                    }
                    case "S": // Set (to TRUE/1 if accumulator is true)
                        if (accumulator != 0.0) { StoreValue(operand, 1.0, locals, nodeManager); debug?.Annotate(instr.SourceLine, operand ?? "acc", 1.0); debug?.RecordWrite(operand ?? "", 1.0); debug?.RecordWriteLine(instr.SourceLine); }
                        break;
                    case "R": // Reset (to FALSE/0 if accumulator is true)
                        if (accumulator != 0.0) { StoreValue(operand, 0.0, locals, nodeManager); debug?.Annotate(instr.SourceLine, operand ?? "acc", 0.0); debug?.RecordWrite(operand ?? "", 0.0); debug?.RecordWriteLine(instr.SourceLine); }
                        break;
                    case "AND":
                        accumulator = (accumulator != 0.0 && ResolveValue(operand, locals, nodeManager) != 0.0) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "ANDN":
                        accumulator = (accumulator != 0.0 && ResolveValue(operand, locals, nodeManager) == 0.0) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "OR":
                        accumulator = (accumulator != 0.0 || ResolveValue(operand, locals, nodeManager) != 0.0) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "ORN":
                        accumulator = (accumulator != 0.0 || ResolveValue(operand, locals, nodeManager) == 0.0) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "XOR":
                        accumulator = ((accumulator != 0.0) ^ (ResolveValue(operand, locals, nodeManager) != 0.0)) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "NOT":
                        accumulator = accumulator != 0.0 ? 0.0 : 1.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "ADD":
                        accumulator += ResolveValue(operand, locals, nodeManager);
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "SUB":
                        accumulator -= ResolveValue(operand, locals, nodeManager);
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "MUL":
                        accumulator *= ResolveValue(operand, locals, nodeManager);
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "DIV":
                    {
                        var d = ResolveValue(operand, locals, nodeManager);
                        accumulator = d != 0.0 ? accumulator / d : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    }
                    case "MOD":
                    {
                        var d = ResolveValue(operand, locals, nodeManager);
                        accumulator = d != 0.0 ? accumulator % d : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    }
                    case "GT":
                        accumulator = accumulator > ResolveValue(operand, locals, nodeManager) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "GE":
                        accumulator = accumulator >= ResolveValue(operand, locals, nodeManager) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "EQ":
                        accumulator = accumulator == ResolveValue(operand, locals, nodeManager) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "NE":
                        accumulator = accumulator != ResolveValue(operand, locals, nodeManager) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "LE":
                        accumulator = accumulator <= ResolveValue(operand, locals, nodeManager) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "LT":
                        accumulator = accumulator < ResolveValue(operand, locals, nodeManager) ? 1.0 : 0.0;
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "JMP":
                        if (operand != null && labelIndex.TryGetValue(operand, out var jmpTarget))
                            pc = jmpTarget;
                        break;
                    case "JMPC": // Jump if accumulator true
                        if (accumulator != 0.0 && operand != null && labelIndex.TryGetValue(operand, out var jmpcTarget))
                            pc = jmpcTarget;
                        break;
                    case "JMPCN": // Jump if accumulator false
                        if (accumulator == 0.0 && operand != null && labelIndex.TryGetValue(operand, out var jmpcnTarget))
                            pc = jmpcnTarget;
                        break;
                    case "RET":
                        return;
                    case "RETC":
                        if (accumulator != 0.0) return;
                        break;
                    case "RETCN":
                        if (accumulator == 0.0) return;
                        break;
                    case "NOP":
                        break;
                    case "ABS":
                        accumulator = Math.Abs(accumulator);
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "SQRT":
                        accumulator = Math.Sqrt(accumulator);
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    case "CAL":
                    case "CALC":
                    case "CALCN":
                    {
                        // CALC = call if accumulator true, CALCN = call if accumulator false
                        if (op == "CALC" && accumulator == 0.0) break;
                        if (op == "CALCN" && accumulator != 0.0) break;
                        accumulator = ExecuteCall(operand, accumulator, locals, nodeManager);
                        debug?.Annotate(instr.SourceLine, "acc", accumulator);
                        break;
                    }
                    // Ignore unknown operators silently
                }
            }
        }

        private static double ExecuteCall(string? operand, double accumulator, Dictionary<string, double> locals, SimpleFileServerNodeManager nodeManager)
        {
            if (string.IsNullOrEmpty(operand)) return accumulator;

            // Parse function call: FuncName(arg1, arg2, ...)
            var parenOpen = operand.IndexOf('(');
            if (parenOpen < 0) return accumulator;

            var funcName = operand[..parenOpen].Trim().ToUpperInvariant();
            var argsStr = operand[(parenOpen + 1)..].TrimEnd().TrimEnd(')');
            var args = argsStr.Split(',', StringSplitOptions.TrimEntries);

            switch (funcName)
            {
                case "READ":
                {
                    var path = args.ElementAtOrDefault(0)?.Trim('\'', '"') ?? "";
                    if (!string.IsNullOrEmpty(path))
                    {
                        try
                        {
                            var val = nodeManager.ReadVariable(path);
                            return StInterpreter.ToDouble(val);
                        }
                        catch { return 0.0; }
                    }
                    return 0.0;
                }
                case "WRITE":
                {
                    var path = args.ElementAtOrDefault(0)?.Trim('\'', '"') ?? "";
                    var valueArg = args.ElementAtOrDefault(1)?.Trim() ?? "";
                    double value = ResolveValue(valueArg, locals, nodeManager);
                    if (!string.IsNullOrEmpty(path))
                    {
                        try { nodeManager.WriteVariable(path, value); }
                        catch { /* ignore */ }
                    }
                    return accumulator;
                }
                default:
                    return accumulator;
            }
        }

        private static double ResolveValue(string? operand, Dictionary<string, double> locals, SimpleFileServerNodeManager nodeManager)
        {
            if (string.IsNullOrEmpty(operand)) return 0.0;

            // Numeric literal
            if (double.TryParse(operand, CultureInfo.InvariantCulture, out var num))
                return num;

            // Boolean literals
            if (operand.Equals("TRUE", StringComparison.OrdinalIgnoreCase)) return 1.0;
            if (operand.Equals("FALSE", StringComparison.OrdinalIgnoreCase)) return 0.0;

            // Local variable
            if (locals.TryGetValue(operand, out var localVal))
                return localVal;

            // OPC variable
            try
            {
                var val = nodeManager.ReadVariable(operand);
                return StInterpreter.ToDouble(val);
            }
            catch { return 0.0; }
        }

        private static void StoreValue(string? operand, double value, Dictionary<string, double> locals, SimpleFileServerNodeManager nodeManager)
        {
            if (string.IsNullOrEmpty(operand)) return;

            // If it contains a dot, treat as OPC variable path
            if (operand.Contains('.'))
            {
                try { nodeManager.WriteVariable(operand, value); }
                catch { locals[operand] = value; }
            }
            else
            {
                // Try OPC first, fall back to local
                try { nodeManager.WriteVariable(operand, value); }
                catch { locals[operand] = value; }
            }
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  IEC 61131-3 Ladder Diagram (LD) — text representation
    // ═══════════════════════════════════════════════════════════
    //
    //  Text format (one rung per line block):
    //    RUNG "optional label"
    //      CONTACT NO <variable>       (* Normally Open contact *)
    //      CONTACT NC <variable>       (* Normally Closed contact *)
    //      COIL <variable>             (* Output coil *)
    //      COIL_S <variable>           (* Set coil — latching *)
    //      COIL_R <variable>           (* Reset coil *)
    //      COMPARE GT <var> <value>    (* Comparison contact *)
    //      COMPARE LT <var> <value>
    //      COMPARE GE <var> <value>
    //      COMPARE LE <var> <value>
    //      COMPARE EQ <var> <value>
    //      COMPARE NE <var> <value>
    //      MOVE <source> <destination> (* Move/assign on rung true *)
    //      ADD <source> <destination>  (* Add source to destination *)
    //      SUB <source> <destination>
    //      MUL <source> <destination>
    //      DIV <source> <destination>
    //      TIMER <variable> <preset_ms>(* TON timer *)
    //      COUNTER <variable> <preset> (* CTU counter *)
    //    END_RUNG

    internal class LdRung
    {
        public string? Label { get; set; }
        public List<LdElement> Elements { get; set; } = new();
        /// <summary>0-based source line number for debug annotations.</summary>
        public int SourceLine { get; set; } = -1;
    }

    internal class LdElement
    {
        public string Type { get; set; } = "";  // CONTACT, COIL, COIL_S, COIL_R, COMPARE, MOVE, ADD, SUB, MUL, DIV, TIMER, COUNTER, PID
        public string Modifier { get; set; } = ""; // NO, NC for contacts; GT, LT, GE, LE, EQ, NE for compare
        public string Operand1 { get; set; } = "";
        public string Operand2 { get; set; } = "";
        /// <summary>0-based source line number for debug annotations.</summary>
        public int SourceLine { get; set; } = -1;
    }

    internal class LdProgram
    {
        public List<LdRung> Rungs { get; set; } = new();
    }

    internal static class LdParser
    {
        public static LdProgram Parse(string code)
        {
            var program = new LdProgram();
            var lines = code.Split('\n');
            LdRung? currentRung = null;
            int lineNum = 0;

            foreach (var rawLine in lines)
            {
                lineNum++;
                var line = rawLine.Trim();

                // Skip empty/comments
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("//")) continue;
                if (line.StartsWith("(*"))
                {
                    if (line.Contains("*)")) continue;
                    continue;
                }

                // Strip inline comments
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
                        currentRung = new LdRung { SourceLine = lineNum - 1 };
                        if (parts.Length > 1)
                            currentRung.Label = string.Join(' ', parts.Skip(1)).Trim('"', '\'');
                        break;

                    case "END_RUNG":
                        if (currentRung != null)
                            program.Rungs.Add(currentRung);
                        currentRung = null;
                        break;

                    case "CONTACT":
                        if (currentRung == null) throw new InvalidOperationException($"CONTACT outside RUNG at line {lineNum}");
                        if (parts.Length < 3) throw new InvalidOperationException($"CONTACT requires modifier and variable at line {lineNum}");
                        currentRung.Elements.Add(new LdElement
                        {
                            Type = "CONTACT",
                            Modifier = parts[1].ToUpperInvariant(), // NO or NC
                            Operand1 = parts[2],
                            SourceLine = lineNum - 1
                        });
                        break;

                    case "COIL":
                        if (currentRung == null) throw new InvalidOperationException($"COIL outside RUNG at line {lineNum}");
                        if (parts.Length < 2) throw new InvalidOperationException($"COIL requires variable at line {lineNum}");
                        currentRung.Elements.Add(new LdElement { Type = "COIL", Operand1 = parts[1], SourceLine = lineNum - 1 });
                        break;

                    case "COIL_S":
                        if (currentRung == null) throw new InvalidOperationException($"COIL_S outside RUNG at line {lineNum}");
                        if (parts.Length < 2) throw new InvalidOperationException($"COIL_S requires variable at line {lineNum}");
                        currentRung.Elements.Add(new LdElement { Type = "COIL_S", Operand1 = parts[1], SourceLine = lineNum - 1 });
                        break;

                    case "COIL_R":
                        if (currentRung == null) throw new InvalidOperationException($"COIL_R outside RUNG at line {lineNum}");
                        if (parts.Length < 2) throw new InvalidOperationException($"COIL_R requires variable at line {lineNum}");
                        currentRung.Elements.Add(new LdElement { Type = "COIL_R", Operand1 = parts[1], SourceLine = lineNum - 1 });
                        break;

                    case "COMPARE":
                        if (currentRung == null) throw new InvalidOperationException($"COMPARE outside RUNG at line {lineNum}");
                        if (parts.Length < 4) throw new InvalidOperationException($"COMPARE requires operator, variable and value at line {lineNum}");
                        currentRung.Elements.Add(new LdElement
                        {
                            Type = "COMPARE",
                            Modifier = parts[1].ToUpperInvariant(),
                            Operand1 = parts[2],
                            Operand2 = parts[3],
                            SourceLine = lineNum - 1
                        });
                        break;

                    case "MOVE" or "ADD" or "SUB" or "MUL" or "DIV":
                        if (currentRung == null) throw new InvalidOperationException($"{keyword} outside RUNG at line {lineNum}");
                        if (parts.Length < 3) throw new InvalidOperationException($"{keyword} requires source and destination at line {lineNum}");
                        currentRung.Elements.Add(new LdElement
                        {
                            Type = keyword,
                            Operand1 = parts[1],
                            Operand2 = parts[2],
                            SourceLine = lineNum - 1
                        });
                        break;

                    case "TIMER" or "COUNTER":
                        if (currentRung == null) throw new InvalidOperationException($"{keyword} outside RUNG at line {lineNum}");
                        if (parts.Length < 3) throw new InvalidOperationException($"{keyword} requires variable and preset at line {lineNum}");
                        currentRung.Elements.Add(new LdElement
                        {
                            Type = keyword,
                            Operand1 = parts[1],
                            Operand2 = parts[2],
                            SourceLine = lineNum - 1
                        });
                        break;

                    case "PID":
                        // PID pvVar spVar outVar kp ki kd [outMin outMax]
                        if (currentRung == null) throw new InvalidOperationException("PID outside RUNG at line " + lineNum);
                        if (parts.Length < 7) throw new InvalidOperationException($"PID requires pvVar spVar outVar kp ki kd at line {lineNum}");
                        // Pack all params into Operand1 (semicolon-separated) and output var into Operand2
                        // Operand1 = "pvVar;spVar;kp;ki;kd;outMin;outMax", Operand2 = outVar
                        var pidOutMin = parts.Length > 7 ? parts[7] : "0";
                        var pidOutMax = parts.Length > 8 ? parts[8] : "100";
                        currentRung.Elements.Add(new LdElement
                        {
                            Type = "PID",
                            Operand1 = $"{parts[1]};{parts[2]};{parts[4]};{parts[5]};{parts[6]};{pidOutMin};{pidOutMax}",
                            Operand2 = parts[3],
                            SourceLine = lineNum - 1
                        });
                        break;

                    default:
                        throw new InvalidOperationException($"Unknown ladder element '{keyword}' at line {lineNum}");
                }
            }

            if (currentRung != null)
                throw new InvalidOperationException("Missing END_RUNG at end of program");

            return program;
        }
    }

    internal static class LdInterpreter
    {
        // Timer/counter/PID state persists across cycles
        private static readonly ConcurrentDictionary<string, (DateTime startTime, bool running, bool done)> _timers = new();
        private static readonly ConcurrentDictionary<string, (int count, bool prevRung)> _counters = new();
        private static readonly ConcurrentDictionary<string, (double integral, double prevError)> _pidStates = new();

        public static void Execute(LdProgram program, SimpleFileServerNodeManager nodeManager, PlcDebugContext? debug = null)
        {
            foreach (var rung in program.Rungs)
            {
                // Power rail starts TRUE
                bool rungState = true;

                foreach (var elem in rung.Elements)
                {
                    debug?.RecordExecutedLine(elem.SourceLine);

                    switch (elem.Type)
                    {
                        case "CONTACT":
                        {
                            double val = ReadValue(elem.Operand1, nodeManager);
                            debug?.RecordRead(elem.Operand1, val);
                            bool contact = val != 0.0;
                            if (elem.Modifier == "NC") contact = !contact;
                            rungState = rungState && contact;
                            debug?.Annotate(elem.SourceLine, elem.Operand1, contact);
                            break;
                        }

                        case "COMPARE":
                        {
                            double left = ReadValue(elem.Operand1, nodeManager);
                            double right = ParseNumericOrRead(elem.Operand2, nodeManager);
                            debug?.RecordRead(elem.Operand1, left);
                            bool result = elem.Modifier switch
                            {
                                "GT" => left > right,
                                "GE" => left >= right,
                                "LT" => left < right,
                                "LE" => left <= right,
                                "EQ" => left == right,
                                "NE" => left != right,
                                _ => false
                            };
                            rungState = rungState && result;
                            debug?.Annotate(elem.SourceLine, elem.Operand1 + " " + elem.Modifier + " " + elem.Operand2, result);
                            break;
                        }

                        case "COIL":
                        {
                            var coilVal = rungState ? 1.0 : 0.0;
                            WriteValue(elem.Operand1, coilVal, nodeManager);
                            debug?.Annotate(elem.SourceLine, elem.Operand1, coilVal);
                            debug?.RecordWrite(elem.Operand1, coilVal);
                            debug?.RecordWriteLine(elem.SourceLine);
                            break;
                        }

                        case "COIL_S":
                            if (rungState) { WriteValue(elem.Operand1, 1.0, nodeManager); debug?.RecordWrite(elem.Operand1, 1.0); debug?.RecordWriteLine(elem.SourceLine); }
                            debug?.Annotate(elem.SourceLine, elem.Operand1, rungState ? 1.0 : ReadValue(elem.Operand1, nodeManager));
                            break;

                        case "COIL_R":
                            if (rungState) { WriteValue(elem.Operand1, 0.0, nodeManager); debug?.RecordWrite(elem.Operand1, 0.0); debug?.RecordWriteLine(elem.SourceLine); }
                            debug?.Annotate(elem.SourceLine, elem.Operand1, rungState ? 0.0 : ReadValue(elem.Operand1, nodeManager));
                            break;

                        case "MOVE":
                            if (rungState)
                            {
                                double src = ParseNumericOrRead(elem.Operand1, nodeManager);
                                WriteValue(elem.Operand2, src, nodeManager);
                                debug?.Annotate(elem.SourceLine, elem.Operand2, src);
                                debug?.RecordWrite(elem.Operand2, src);
                                debug?.RecordWriteLine(elem.SourceLine);
                            }
                            break;

                        case "ADD":
                            if (rungState)
                            {
                                double src = ParseNumericOrRead(elem.Operand1, nodeManager);
                                double dst = ReadValue(elem.Operand2, nodeManager);
                                var result = dst + src;
                                WriteValue(elem.Operand2, result, nodeManager);
                                debug?.Annotate(elem.SourceLine, elem.Operand2, result);
                                debug?.RecordWrite(elem.Operand2, result);
                                debug?.RecordWriteLine(elem.SourceLine);
                            }
                            break;

                        case "SUB":
                            if (rungState)
                            {
                                double src = ParseNumericOrRead(elem.Operand1, nodeManager);
                                double dst = ReadValue(elem.Operand2, nodeManager);
                                var result = dst - src;
                                WriteValue(elem.Operand2, result, nodeManager);
                                debug?.Annotate(elem.SourceLine, elem.Operand2, result);
                                debug?.RecordWrite(elem.Operand2, result);
                                debug?.RecordWriteLine(elem.SourceLine);
                            }
                            break;

                        case "MUL":
                            if (rungState)
                            {
                                double src = ParseNumericOrRead(elem.Operand1, nodeManager);
                                double dst = ReadValue(elem.Operand2, nodeManager);
                                var result = dst * src;
                                WriteValue(elem.Operand2, result, nodeManager);
                                debug?.Annotate(elem.SourceLine, elem.Operand2, result);
                                debug?.RecordWrite(elem.Operand2, result);
                                debug?.RecordWriteLine(elem.SourceLine);
                            }
                            break;

                        case "DIV":
                            if (rungState)
                            {
                                double src = ParseNumericOrRead(elem.Operand1, nodeManager);
                                double dst = ReadValue(elem.Operand2, nodeManager);
                                var divResult = src != 0 ? dst / src : 0.0;
                                WriteValue(elem.Operand2, divResult, nodeManager);
                                debug?.Annotate(elem.SourceLine, elem.Operand2, divResult);
                                debug?.RecordWrite(elem.Operand2, divResult);
                                debug?.RecordWriteLine(elem.SourceLine);
                            }
                            break;

                        case "TIMER":
                        {
                            // TON: starts timing when rung is true, sets output when elapsed
                            var key = elem.Operand1;
                            var presetMs = ParseNumericOrRead(elem.Operand2, nodeManager);
                            var state = _timers.GetOrAdd(key, _ => (DateTime.MinValue, false, false));

                            if (rungState)
                            {
                                if (!state.running)
                                    state = (DateTime.UtcNow, true, false);
                                else if ((DateTime.UtcNow - state.startTime).TotalMilliseconds >= presetMs)
                                    state = (state.startTime, true, true);
                            }
                            else
                            {
                                state = (DateTime.MinValue, false, false);
                            }

                            _timers[key] = state;
                            WriteValue(key, state.done ? 1.0 : 0.0, nodeManager);
                            debug?.Annotate(elem.SourceLine, key, state.done);
                            debug?.RecordWrite(key, state.done ? 1.0 : 0.0);
                            rungState = state.done;
                            break;
                        }

                        case "COUNTER":
                        {
                            // CTU: counts rising edges, sets output when count >= preset
                            var key = elem.Operand1;
                            var preset = (int)ParseNumericOrRead(elem.Operand2, nodeManager);
                            var state = _counters.GetOrAdd(key, _ => (0, false));

                            bool risingEdge = rungState && !state.prevRung;
                            int count = state.count;
                            if (risingEdge) count++;

                            _counters[key] = (count, rungState);
                            bool done = count >= preset;
                            WriteValue(key, done ? 1.0 : 0.0, nodeManager);
                            debug?.Annotate(elem.SourceLine, key, count);
                            debug?.RecordWrite(key, done ? 1.0 : 0.0);
                            rungState = done;
                            break;
                        }

                        case "PID":
                        {
                            // PID block: Operand1 = "pvVar;spVar;kp;ki;kd;outMin;outMax", Operand2 = outVar
                            if (rungState)
                            {
                                var pidParts = elem.Operand1.Split(';');
                                double pv     = ReadValue(pidParts[0], nodeManager);
                                double sp     = ParseNumericOrRead(pidParts[1], nodeManager);
                                double kp     = double.Parse(pidParts[2], System.Globalization.CultureInfo.InvariantCulture);
                                double ki     = double.Parse(pidParts[3], System.Globalization.CultureInfo.InvariantCulture);
                                double kd     = double.Parse(pidParts[4], System.Globalization.CultureInfo.InvariantCulture);
                                double outMin = pidParts.Length > 5 ? double.Parse(pidParts[5], System.Globalization.CultureInfo.InvariantCulture) : 0;
                                double outMax = pidParts.Length > 6 ? double.Parse(pidParts[6], System.Globalization.CultureInfo.InvariantCulture) : 100;

                                var pidKey = elem.Operand2;
                                var pidState = _pidStates.GetOrAdd(pidKey, _ => (0.0, 0.0));

                                double error = sp - pv;
                                double integral = pidState.integral + error;
                                if (ki != 0)
                                {
                                    if (integral * ki > outMax) integral = outMax / ki;
                                    if (integral * ki < outMin) integral = outMin / ki;
                                }
                                double derivative = error - pidState.prevError;
                                double output = kp * error + ki * integral + kd * derivative;
                                if (output > outMax) output = outMax;
                                if (output < outMin) output = outMin;

                                _pidStates[pidKey] = (integral, error);
                                WriteValue(pidKey, output, nodeManager);
                                debug?.Annotate(elem.SourceLine, pidKey, output);
                                debug?.RecordWrite(pidKey, output);
                                debug?.RecordWriteLine(elem.SourceLine);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private static double ReadValue(string operand, SimpleFileServerNodeManager nodeManager)
        {
            try
            {
                var val = nodeManager.ReadVariable(operand);
                return StInterpreter.ToDouble(val);
            }
            catch { return 0.0; }
        }

        private static double ParseNumericOrRead(string operand, SimpleFileServerNodeManager nodeManager)
        {
            if (double.TryParse(operand, CultureInfo.InvariantCulture, out var num))
                return num;
            if (operand.Equals("TRUE", StringComparison.OrdinalIgnoreCase)) return 1.0;
            if (operand.Equals("FALSE", StringComparison.OrdinalIgnoreCase)) return 0.0;
            return ReadValue(operand, nodeManager);
        }

        private static void WriteValue(string operand, double value, SimpleFileServerNodeManager nodeManager)
        {
            try { nodeManager.WriteVariable(operand, value); }
            catch { /* variable may not exist */ }
        }
    }
}
