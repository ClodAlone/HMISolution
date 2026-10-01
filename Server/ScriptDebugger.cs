// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer;

/// <summary>
/// Manages breakpoint-based debugging for scripts.
/// Each script gets a debug session that can pause execution at instrumented checkpoints.
/// The editor communicates breakpoints and commands via the HTTP diagnostics endpoint.
/// </summary>
public sealed class ScriptDebugger
{
    private readonly ConcurrentDictionary<string, DebugSession> _sessions = new(StringComparer.OrdinalIgnoreCase);

    public static ScriptDebugger Instance { get; } = new();

    /// <summary>
    /// Set breakpoints for a script. Creates a session if one doesn't exist.
    /// </summary>
    public void SetBreakpoints(string scriptName, List<int> lines)
    {
        var session = GetOrCreateSession(scriptName);
        lock (session.Lock)
        {
            session.Breakpoints.Clear();
            foreach (var line in lines)
                session.Breakpoints.Add(new ScriptBreakpoint { ScriptName = scriptName, Line = line, Enabled = true });

            // If breakpoints were added, ensure session is active
            if (lines.Count > 0 && session.State == ScriptDebugState.Detached)
                session.State = ScriptDebugState.Running;

            // If all breakpoints removed and not paused, detach
            if (lines.Count == 0 && session.State == ScriptDebugState.Running)
                session.State = ScriptDebugState.Detached;
        }
    }

    /// <summary>
    /// Send a debug command to a script's session.
    /// </summary>
    public void SendCommand(string scriptName, ScriptDebugCommand command)
    {
        var session = GetOrCreateSession(scriptName);
        lock (session.Lock)
        {
            switch (command)
            {
                case ScriptDebugCommand.Continue:
                    session.State = ScriptDebugState.Running;
                    session.PausedAtLine = -1;
                    TryRelease(session);
                    break;

                case ScriptDebugCommand.StepOver:
                    session.State = ScriptDebugState.Stepping;
                    TryRelease(session);
                    break;

                case ScriptDebugCommand.Pause:
                    session.State = ScriptDebugState.Paused;
                    break;

                case ScriptDebugCommand.Detach:
                    session.State = ScriptDebugState.Detached;
                    session.PausedAtLine = -1;
                    session.Breakpoints.Clear();
                    session.WatchVariables.Clear();
                    TryRelease(session); // unblock if paused
                    break;
            }
        }
    }

    /// <summary>
    /// Called by the script runner at each instrumented checkpoint line.
    /// Blocks if the line has a breakpoint or if stepping/paused.
    /// </summary>
    public void CheckBreakpoint(string scriptName, int line, Dictionary<string, string> currentVars, CancellationToken ct)
    {
        if (!_sessions.TryGetValue(scriptName, out var session)) return;

        bool shouldPause;
        lock (session.Lock)
        {
            // Always track the current execution line for live visualization
            session.LastCheckpointLine = line;

            if (session.State == ScriptDebugState.Detached) return;

            shouldPause = session.State == ScriptDebugState.Paused
                       || session.State == ScriptDebugState.Stepping
                       || session.Breakpoints.Any(bp => bp.Enabled && bp.Line == line);
        }

        if (!shouldPause) return;

        // Pause: update state and block until a command is received
        lock (session.Lock)
        {
            session.State = ScriptDebugState.Paused;
            session.PausedAtLine = line;
            session.WatchVariables = new Dictionary<string, string>(currentVars);

            // Drain any leftover signals
            while (session.Signal.CurrentCount > 0)
                session.Signal.Wait(0);
        }

        Log.Debug("[ScriptDebug] {Script} paused at line {Line}", scriptName, line);

        try
        {
            session.Signal.Wait(ct);
        }
        catch (OperationCanceledException)
        {
            lock (session.Lock)
            {
                session.State = ScriptDebugState.Detached;
                session.PausedAtLine = -1;
            }
            throw;
        }
    }

    /// <summary>
    /// Get the debug session state for diagnostics reporting.
    /// </summary>
    public ScriptDebugSession? GetSessionInfo(string scriptName)
    {
        if (!_sessions.TryGetValue(scriptName, out var session)) return null;
        lock (session.Lock)
        {
            if (session.State == ScriptDebugState.Detached && session.Breakpoints.Count == 0)
                return null;

            return new ScriptDebugSession
            {
                State = session.State,
                PausedAtLine = session.PausedAtLine,
                LastCheckpointLine = session.LastCheckpointLine,
                Breakpoints = session.Breakpoints.Select(bp => new ScriptBreakpoint
                {
                    ScriptName = bp.ScriptName,
                    Line = bp.Line,
                    Enabled = bp.Enabled
                }).ToList(),
                WatchVariables = new Dictionary<string, string>(session.WatchVariables)
            };
        }
    }

    /// <summary>
    /// Returns true if there is an active debug session for the script.
    /// </summary>
    public bool HasActiveSession(string scriptName)
    {
        return _sessions.TryGetValue(scriptName, out var s) && s.State != ScriptDebugState.Detached;
    }

    /// <summary>
    /// Instruments script source code by injecting __DebugCheckpoint(line) calls before each code line.
    /// The instrumented code preserves original line numbers by prepending on the same line.
    /// </summary>
    public static string InstrumentSource(string code)
    {
        var lines = code.Split('\n');
        var sb = new StringBuilder();
        bool inBlockComment = false;
        string prevTrimmed = "";

        for (int i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].TrimStart().TrimEnd('\r', '\n');

            // Track block comments
            if (inBlockComment)
            {
                if (trimmed.Contains("*/"))
                    inBlockComment = false;
                sb.AppendLine(lines[i]);
                continue;
            }

            if (trimmed.StartsWith("/*"))
            {
                inBlockComment = !trimmed.Contains("*/");
                sb.AppendLine(lines[i]);
                continue;
            }

            bool isCodeLine = !string.IsNullOrWhiteSpace(trimmed)
                && !trimmed.StartsWith("//")
                && trimmed != "{"
                && !trimmed.StartsWith("}")
                && trimmed != "{}"
                && !trimmed.StartsWith("using ")
                && !trimmed.StartsWith("#")
                && !trimmed.StartsWith("else")
                && !trimmed.StartsWith("catch")
                && !trimmed.StartsWith("finally");

            // Skip instrumenting single-statement bodies of braceless control flow.
            // If the previous code line was a control-flow head without opening a brace,
            // the current line is its body and adding a checkpoint would break syntax.
            if (isCodeLine && IsBracelessControlFlowHead(prevTrimmed))
                isCodeLine = false;

            if (isCodeLine)
            {
                var indent = lines[i][..^lines[i].TrimStart().Length];
                sb.Append(indent);
                sb.Append($"__DebugCheckpoint({i}); ");
                sb.AppendLine(trimmed);
            }
            else
            {
                sb.AppendLine(lines[i]);
            }

            if (!string.IsNullOrWhiteSpace(trimmed) && !trimmed.StartsWith("//"))
                prevTrimmed = trimmed;
        }

        return sb.ToString();
    }

    private static bool IsBracelessControlFlowHead(string trimmed)
    {
        if (string.IsNullOrEmpty(trimmed))
            return false;
        if (trimmed == "else")
            return true;
        if (trimmed.EndsWith(')') &&
            (trimmed.StartsWith("if ") || trimmed.StartsWith("if(") ||
             trimmed.StartsWith("else ") ||
             trimmed.StartsWith("for ") || trimmed.StartsWith("for(") ||
             trimmed.StartsWith("foreach ") || trimmed.StartsWith("foreach(") ||
             trimmed.StartsWith("while ") || trimmed.StartsWith("while(")))
            return true;
        return false;
    }

    /// <summary>
    /// Instruments Python script source by injecting __DebugCheckpoint(line) calls before each
    /// executable statement, preserving indentation (Python is indentation-sensitive, so the
    /// checkpoint call is appended on the same physical line via ';' rather than on its own line).
    /// Block headers (if/elif/else/for/while/try/except/finally/def/class/with ending in ':'),
    /// comments, blank lines, continuation lines, and lines inside open brackets or triple-quoted
    /// strings are left untouched.
    /// </summary>
    public static string InstrumentPythonSource(string code)
    {
        var lines = code.Replace("\r\n", "\n").Split('\n');
        var sb = new StringBuilder();
        int bracketDepth = 0;
        bool inTripleString = false;
        string? tripleQuote = null;
        bool prevContinuation = false;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.Trim();

            bool wasInTripleString = inTripleString;
            bool wasContinuation = prevContinuation || bracketDepth > 0;

            // Track triple-quoted strings (best-effort, ignores escaping edge cases)
            if (inTripleString)
            {
                if (tripleQuote != null && line.Contains(tripleQuote))
                    inTripleString = false;
                sb.Append(line).Append('\n');
                continue;
            }

            bool isCodeLine = !string.IsNullOrWhiteSpace(trimmed)
                && !trimmed.StartsWith("#")
                && !wasContinuation;

            // Detect start of a triple-quoted string on this line (for docstrings/multi-line strings)
            foreach (var q in new[] { "\"\"\"", "'''" })
            {
                int firstIdx = line.IndexOf(q, StringComparison.Ordinal);
                if (firstIdx >= 0)
                {
                    int secondIdx = line.IndexOf(q, firstIdx + 3, StringComparison.Ordinal);
                    if (secondIdx < 0)
                    {
                        inTripleString = true;
                        tripleQuote = q;
                    }
                    break;
                }
            }

            // Block headers (end with ':') must not be prefixed — they open a new indented suite
            bool isBlockHeader = trimmed.EndsWith(':');

            // Track explicit line continuation and open brackets (approximate: count unmatched brackets)
            bool endsWithBackslash = line.TrimEnd().EndsWith("\\");
            int opens = trimmed.Count(c => c is '(' or '[' or '{');
            int closes = trimmed.Count(c => c is ')' or ']' or '}');
            bracketDepth = Math.Max(0, bracketDepth + opens - closes);

            if (isCodeLine && !isBlockHeader && !wasInTripleString && bracketDepth == 0 && !endsWithBackslash)
            {
                var indent = line[..(line.Length - line.TrimStart().Length)];
                sb.Append(indent).Append($"__DebugCheckpoint({i}); ").Append(trimmed).Append('\n');
            }
            else
            {
                sb.Append(line).Append('\n');
            }

            prevContinuation = endsWithBackslash;
        }

        return sb.ToString();
    }

    private DebugSession GetOrCreateSession(string scriptName)
    {
        return _sessions.GetOrAdd(scriptName, _ => new DebugSession());
    }

    private static void TryRelease(DebugSession session)
    {
        if (session.Signal.CurrentCount == 0)
        {
            try { session.Signal.Release(); } catch { }
        }
    }

    private sealed class DebugSession
    {
        public readonly object Lock = new();
        public ScriptDebugState State = ScriptDebugState.Detached;
        public int PausedAtLine = -1;
        public int LastCheckpointLine = -1;
        public List<ScriptBreakpoint> Breakpoints = new();
        public Dictionary<string, string> WatchVariables = new();
        public SemaphoreSlim Signal = new(0, 1);
    }
}
