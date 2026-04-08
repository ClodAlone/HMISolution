using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.VisualBasic;
using Opc.Ua;
using SharedModels;
using Serilog;

namespace SimpleOpcFileServer
{
    public class VariableChangedEventArgs : EventArgs
    {
        public string VariableName { get; }
        public object? OldValue { get; }
        public object? NewValue { get; }
        public DateTime Timestamp { get; }

        public VariableChangedEventArgs(string variableName, object? oldValue, object? newValue, DateTime timestamp)
        {
            VariableName = variableName;
            OldValue = oldValue;
            NewValue = newValue;
            Timestamp = timestamp;
        }
    }

    public class ScriptManager : IDisposable
    {
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly List<ScriptRunner> _runners = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly ConcurrentDictionary<string, List<Action<VariableChangedEventArgs>>> _changeHandlers = new();

        public ScriptManager(SimpleFileServerNodeManager nodeManager)
        {
            _nodeManager = nodeManager;
        }

        public void Initialize(List<ScriptConfig> scripts)
        {
            foreach (var script in scripts)
            {
                DiagnosticsCollector.Instance.Register("Script", script.Name, script.Enabled);
                if (script.Enabled)
                {
                    var runner = new ScriptRunner(script, _nodeManager, this, _cts.Token);
                    _runners.Add(runner);
                    runner.Start();
                }
            }
        }

        internal void LogMessage(string message)
        {
            Log.Information("[Script] {Message}", message);
        }

        internal void Subscribe(string variableName, Action<VariableChangedEventArgs> handler)
        {
            var handlers = _changeHandlers.GetOrAdd(variableName, _ => new());
            lock (handlers)
            {
                handlers.Add(handler);
            }
        }

        internal void NotifyVariableChanged(string variableName, object? oldValue, object? newValue, DateTime timestamp)
        {
            if (_changeHandlers.TryGetValue(variableName, out var handlers))
            {
                List<Action<VariableChangedEventArgs>> snapshot;
                lock (handlers) { snapshot = new List<Action<VariableChangedEventArgs>>(handlers); }

                var args = new VariableChangedEventArgs(variableName, oldValue, newValue, timestamp);
                foreach (var handler in snapshot)
                {
                    try
                    {
                        handler(args);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error in OnChanged handler for {Variable}: {Message}", variableName, ex.Message);
                    }
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            foreach (var r in _runners) r.Stop();
            _changeHandlers.Clear();
            _cts.Dispose();
        }
    }

    public class ScriptRunner
    {
        private readonly ScriptConfig _config;
        private readonly SimpleFileServerNodeManager _nodeManager;
        private readonly ScriptManager _scriptManager;
        private readonly CancellationToken _token;
        private Task? _task;
        private Script<object>? _compiledCSharpScript;
        private Script<object>? _compiledDebugScript;
        private bool _lastDebugActive;
        private (Assembly assembly, MethodInfo method)? _compiledVbScript;

        // Static compilation cache: survives ScriptRunner/ScriptManager disposal (e.g. redundancy failover)
        // so scripts are compiled only once per process lifetime. Script<object> is immutable and thread-safe.
        private static readonly ConcurrentDictionary<string, Script<object>> s_compilationCache = new();
        private static readonly ConcurrentDictionary<string, (Assembly assembly, MethodInfo method)> s_vbCompilationCache = new();

        // Error backoff: prevents tight-loop re-execution on persistent script failures
        private int _consecutiveErrors;
        private const int MaxBackoffMs = 30_000;

        public ScriptRunner(ScriptConfig config, SimpleFileServerNodeManager nodeManager, ScriptManager scriptManager, CancellationToken token)
        {
            _config = config;
            _nodeManager = nodeManager;
            _scriptManager = scriptManager;
            _token = token;
        }

        private bool IsVb => _config.Language.Equals("VB", StringComparison.OrdinalIgnoreCase)
                          || _config.Language.Equals("VB.NET", StringComparison.OrdinalIgnoreCase)
                          || _config.Language.Equals("VisualBasic", StringComparison.OrdinalIgnoreCase);

        public void Start()
        {
            _task = Task.Run(async () =>
            {
                long cycleCount = 0;
                // Seed diagnostics so the program appears immediately
                RecordDebugSnapshot(null, 0, "Running", null);

                while (!_token.IsCancellationRequested)
                {
                    cycleCount++;
                    var editorConnected = DiagnosticsCollector.Instance.IsEditorConnected;
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    ScriptGlobals? globals = null;
                    try
                    {
                        globals = new ScriptGlobals(_nodeManager, _scriptManager, _config.Name, _token, editorConnected);

                        if (IsVb)
                            await RunVbAsync(globals);
                        else
                            await RunCSharpAsync(globals);

                        sw.Stop();
                        DiagnosticsCollector.Instance.RecordCycle("Script", _config.Name, sw.Elapsed.TotalMilliseconds);
                        if (editorConnected) RecordDebugSnapshot(globals, cycleCount, "Running", null);
                        _consecutiveErrors = 0;
                    }
                    catch (Exception ex)
                    {
                        sw.Stop();
                        _consecutiveErrors++;
                        DiagnosticsCollector.Instance.RecordCycle("Script", _config.Name, sw.Elapsed.TotalMilliseconds, error: ex.Message);
                        if (editorConnected) RecordDebugSnapshot(globals, cycleCount, "Error", ex.Message);

                        // Only log the first occurrence and every 10th repeat to avoid Serilog flooding
                        if (_consecutiveErrors == 1 || _consecutiveErrors % 10 == 0)
                            Log.Error(ex, Strings.Script_ExecutionError, _config.Name, ex.Message);
                    }

                    if (_config.IntervalMs > 0)
                    {
                        // Apply exponential backoff on persistent errors
                        var delay = _config.IntervalMs;
                        if (_consecutiveErrors > 1)
                            delay = Math.Min(delay * (1 << Math.Min(_consecutiveErrors - 1, 10)), MaxBackoffMs);

                        try
                        {
                            await Task.Delay(delay, _token);
                        }
                        catch (OperationCanceledException) { break; }
                    }
                    else
                    {
                        // Event-driven script (IntervalMs == 0): run once to register
                        // OnChanged handlers, then stay alive until cancelled.
                        try
                        {
                            await Task.Delay(Timeout.Infinite, _token);
                        }
                        catch (OperationCanceledException) { break; }
                    }
                }
            }, _token);
        }

        private static Script<object> GetOrCompileCSharp(string code, string cacheKey)
        {
            return s_compilationCache.GetOrAdd(cacheKey, _ =>
            {
                var options = ScriptOptions.Default
                    .AddReferences(typeof(SimpleFileServerNodeManager).Assembly)
                    .AddImports("System", "System.Collections.Generic", "System.Linq");

                var script = CSharpScript.Create(code, options, typeof(ScriptGlobals));
                script.Compile();
                return script;
            });
        }

        private async Task RunCSharpAsync(ScriptGlobals globals)
        {
            bool debugActive = ScriptDebugger.Instance.HasActiveSession(_config.Name);

            // Invalidate cached debug script if debug mode changed
            if (debugActive != _lastDebugActive)
            {
                _compiledDebugScript = null;
                _lastDebugActive = debugActive;
            }

            if (debugActive)
            {
                if (_compiledDebugScript == null)
                {
                    var instrumentedCode = ScriptDebugger.InstrumentSource(_config.Code);
                    // Cache key uses the instrumented code so changes to InstrumentSource logic invalidate stale compilations
                    _compiledDebugScript = GetOrCompileCSharp(instrumentedCode, "dbg:" + instrumentedCode);
                }

                await _compiledDebugScript.RunAsync(globals, cancellationToken: _token);
            }
            else
            {
                if (_compiledCSharpScript == null)
                {
                    _compiledCSharpScript = GetOrCompileCSharp(_config.Code, _config.Code);
                }

                await _compiledCSharpScript.RunAsync(globals, cancellationToken: _token);
            }
        }

        private Task RunVbAsync(ScriptGlobals globals)
        {
            if (_compiledVbScript == null)
                _compiledVbScript = s_vbCompilationCache.GetOrAdd(_config.Code, code => CompileVbScript(code));

            var (_, method) = _compiledVbScript.Value;
            method.Invoke(null, [globals]);
            return Task.CompletedTask;
        }

        public static (Assembly assembly, MethodInfo method) CompileVbScript(string code)
        {
            var wrappedCode = $@"
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Module ScriptModule
    Public Sub Run(globals As Object)
        Dim Read As Func(Of String, Object) = Function(__varName__) globals.GetType().GetMethod(""Read"").Invoke(globals, {{__varName__}})
        Dim Write As Action(Of String, Object) = Sub(__varName__, __varValue__) globals.GetType().GetMethod(""Write"").Invoke(globals, {{__varName__, __varValue__}})
        Dim OnChanged As Action(Of String, Action(Of Object)) = Sub(__varName__, __handler__)
            Dim __m__ = globals.GetType().GetMethod(""OnChanged"")
            Dim __paramType__ = __m__.GetParameters()(1).ParameterType
            Dim __wrappedDel__ = [Delegate].CreateDelegate(__paramType__, __handler__, __handler__.GetType().GetMethod(""Invoke""))
            __m__.Invoke(globals, {{__varName__, __wrappedDel__}})
        End Sub
{code}
    End Sub
End Module";

            var syntaxTree = VisualBasicSyntaxTree.ParseText(wrappedCode);
            var references = GetVbCompilationReferences();

            var compilation = VisualBasicCompilation.Create(
                "VbScript_" + Guid.NewGuid().ToString("N"),
                [syntaxTree],
                references,
                new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optionStrict: OptionStrict.Off,
                    optionInfer: true));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var errors = string.Join("\n", result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => d.GetMessage()));
                throw new InvalidOperationException($"VB.NET compilation failed:\n{errors}");
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());
            var method = assembly.GetType("ScriptModule")!.GetMethod("Run")!;
            return (assembly, method);
        }

        private static List<MetadataReference> GetVbCompilationReferences()
        {
            var refs = new Dictionary<string, MetadataReference>(StringComparer.OrdinalIgnoreCase);

            // Add all loaded assemblies
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.IsDynamic || string.IsNullOrEmpty(asm.Location)) continue;
                var name = Path.GetFileName(asm.Location);
                refs.TryAdd(name, MetadataReference.CreateFromFile(asm.Location));
            }

            // Ensure the VB runtime and key BCL assemblies are present from the runtime directory
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

        private void RecordDebugSnapshot(ScriptGlobals? globals, long cycleCount, string status, string? error)
        {
            var info = new SharedModels.ProgramDebugInfo
            {
                Category = "Script",
                Name = _config.Name,
                CycleCount = cycleCount,
                Status = status,
                LastError = error
            };
            if (globals != null)
            {
                foreach (var kvp in globals.DebugReads)
                    info.Variables["[read] " + kvp.Key] = kvp.Value;
                foreach (var kvp in globals.DebugWrites)
                    info.Variables["[write] " + kvp.Key] = kvp.Value;
            }
            info.DebugSession = ScriptDebugger.Instance.GetSessionInfo(_config.Name);
            if (info.DebugSession != null)
                info.LastExecutedLine = info.DebugSession.LastCheckpointLine;
            DiagnosticsCollector.Instance.RecordDebug(info);
        }

        public void Stop() { }
    }

    public class ScriptGlobals
    {
        private readonly SimpleFileServerNodeManager _manager;
        private readonly ScriptManager _scriptManager;
        private readonly string _scriptName;
        private readonly CancellationToken _ct;
        private readonly bool _debugEnabled;

        /// <summary>Tracks variable reads/writes for debug visualization.</summary>
        internal Dictionary<string, string> DebugReads { get; } = new(StringComparer.OrdinalIgnoreCase);
        internal Dictionary<string, string> DebugWrites { get; } = new(StringComparer.OrdinalIgnoreCase);

        public ScriptGlobals(SimpleFileServerNodeManager manager, ScriptManager scriptManager,
            string scriptName = "", CancellationToken ct = default, bool debugEnabled = true)
        {
            _manager = manager;
            _scriptManager = scriptManager;
            _scriptName = scriptName;
            _ct = ct;
            _debugEnabled = debugEnabled;
        }

        /// <summary>
        /// Debug checkpoint injected at each source line when debugging.
        /// Checks breakpoints and blocks execution if paused.
        /// </summary>
        public void __DebugCheckpoint(int line)
        {
            if (!_debugEnabled && !ScriptDebugger.Instance.HasActiveSession(_scriptName)) return;
            var allVars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in DebugReads) allVars["[read] " + kvp.Key] = kvp.Value;
            foreach (var kvp in DebugWrites) allVars["[write] " + kvp.Key] = kvp.Value;
            ScriptDebugger.Instance.CheckBreakpoint(_scriptName, line, allVars, _ct);
        }

        public object? Read(string variableName)
        {
            var val = _manager.ReadVariable(variableName);
            if (_debugEnabled) DebugReads[variableName] = val?.ToString() ?? "null";
            return val;
        }

        public double ReadDouble(string variableName)
        {
            return Convert.ToDouble(Read(variableName) ?? 0.0);
        }

        public int ReadInt(string variableName)
        {
            return Convert.ToInt32(Read(variableName) ?? 0);
        }

        public bool ReadBool(string variableName)
        {
            return Convert.ToBoolean(Read(variableName) ?? false);
        }

        public void Log(string message)
        {
            _scriptManager.LogMessage(message);
        }

        public void Write(string variableName, object value)
        {
            _manager.WriteVariable(variableName, value);
            if (_debugEnabled) DebugWrites[variableName] = value?.ToString() ?? "null";
        }

        /// <summary>
        /// Subscribe to value changes on a variable. The callback fires each time
        /// the variable's value is updated (by a driver, OPC UA write, or script).
        /// </summary>
        public void OnChanged(string variableName, Action<VariableChangedEventArgs> handler)
        {
            _scriptManager.Subscribe(variableName, handler);
        }
    }
}
