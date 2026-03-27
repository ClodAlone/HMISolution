using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SharedModels;

/// <summary>
/// Debug execution state for a script debug session.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ScriptDebugState>))]
public enum ScriptDebugState
{
    /// <summary>Script is running normally (not paused).</summary>
    Running,
    /// <summary>Script execution is paused at a breakpoint or after a step.</summary>
    Paused,
    /// <summary>Script is executing a single step then will pause.</summary>
    Stepping,
    /// <summary>Debug session not active.</summary>
    Detached
}

/// <summary>
/// Commands that can be sent to a script debug session.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ScriptDebugCommand>))]
public enum ScriptDebugCommand
{
    /// <summary>Resume execution until next breakpoint.</summary>
    Continue,
    /// <summary>Execute one statement/line then pause.</summary>
    StepOver,
    /// <summary>Pause execution at the current point.</summary>
    Pause,
    /// <summary>Stop and detach the debug session.</summary>
    Detach
}

/// <summary>
/// A breakpoint set on a script line.
/// </summary>
public class ScriptBreakpoint
{
    public string ScriptName { get; set; } = "";
    public int Line { get; set; }
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// Debug session state for a single script, included in ProgramDebugInfo.
/// </summary>
public class ScriptDebugSession
{
    public ScriptDebugState State { get; set; } = ScriptDebugState.Detached;

    /// <summary>0-based line number where execution is currently paused. -1 if not paused.</summary>
    public int PausedAtLine { get; set; } = -1;

    /// <summary>Active breakpoints for this script.</summary>
    public List<ScriptBreakpoint> Breakpoints { get; set; } = new();

    /// <summary>Variable values captured at the pause point (name → string representation).</summary>
    public Dictionary<string, string> WatchVariables { get; set; } = new();
}

/// <summary>
/// Request payload for setting breakpoints via the debug HTTP API.
/// </summary>
public class SetBreakpointsRequest
{
    public string ScriptName { get; set; } = "";
    public List<int> Lines { get; set; } = new();
}

/// <summary>
/// Request payload for sending a debug command via the debug HTTP API.
/// </summary>
public class DebugCommandRequest
{
    public string ScriptName { get; set; } = "";
    public ScriptDebugCommand Command { get; set; }
}
