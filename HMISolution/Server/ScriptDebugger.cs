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

        for (int i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].TrimStart();

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
                && trimmed != "}"
                && trimmed != "{}"
                && !trimmed.StartsWith("using ")
                && !trimmed.StartsWith("#");

            if (isCodeLine)
            {
                // Preserve original indentation, prepend checkpoint on the same line
                var indent = lines[i][..^trimmed.Length];
                sb.Append(indent);
                sb.Append($"__DebugCheckpoint({i}); ");
                sb.AppendLine(trimmed);
            }
            else
            {
                sb.AppendLine(lines[i]);
            }
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
        public List<ScriptBreakpoint> Breakpoints = new();
        public Dictionary<string, string> WatchVariables = new();
        public SemaphoreSlim Signal = new(0, 1);
    }
}
