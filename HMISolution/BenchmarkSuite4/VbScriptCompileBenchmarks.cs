using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Reflection;
using SimpleOpcFileServer;
using Microsoft.VSDiagnostics;

namespace VbScriptCompileBenchmarks;
[CPUUsageDiagnoser]
public class VbScriptCompileBenchmarks
{
    private string[] _scripts = null!;
    private static readonly ConcurrentDictionary<string, (Assembly assembly, MethodInfo method)> _cache = new();
    [GlobalSetup]
    public void Setup()
    {
        // Typical small SCADA VB scripts
        _scripts = ["Dim x As Integer = 1 + 2", "Dim s As String = \"hello\" : Dim n As Integer = s.Length", "Dim d As DateTime = DateTime.UtcNow"];
        // Pre-warm the cache for the Cached benchmark
        foreach (var code in _scripts)
            _cache.GetOrAdd(code, c => ScriptRunner.CompileVbScript(c));
    }

    [Benchmark(Baseline = true)]
    public void VbCompileAll_Uncached()
    {
        foreach (var code in _scripts)
        {
            ScriptRunner.CompileVbScript(code);
        }
    }

    [Benchmark]
    public void VbCompileAll_Cached()
    {
        foreach (var code in _scripts)
        {
            _cache.GetOrAdd(code, c => ScriptRunner.CompileVbScript(c));
        }
    }
}