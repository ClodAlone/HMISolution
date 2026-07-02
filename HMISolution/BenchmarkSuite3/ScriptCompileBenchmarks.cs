// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.VSDiagnostics;

namespace ScriptCompileBenchmarks;
[CPUUsageDiagnoser]
public class ScriptCompileBenchmarks
{
    private ScriptOptions _options = null!;
    private string[] _scripts = null!;
    private static readonly ConcurrentDictionary<int, Script<object>> _cache = new();
    [GlobalSetup]
    public void Setup()
    {
        _options = ScriptOptions.Default.AddImports("System", "System.Collections.Generic", "System.Linq");
        // Simulate 5 unique scripts (typical HMI/SCADA project)
        _scripts = new[]
        {
            "var x = 1 + 2; x",
            "var list = new List<int>{1,2,3}; list.Sum()",
            "var s = \"hello\"; s.Length",
            "var d = DateTime.UtcNow; d.Ticks",
            "var r = new Random(42); r.Next(100)"
        };
    }

    [Benchmark(Baseline = true)]
    public void CompileAll_Uncached()
    {
        // Simulates the current pattern: compile every script from scratch (as happens on failover/restart)
        foreach (var code in _scripts)
        {
            var script = CSharpScript.Create(code, _options);
            script.Compile();
        }
    }

    [Benchmark]
    public void CompileAll_Cached()
    {
        // Simulates the optimized pattern: only compile if not already in cache
        foreach (var code in _scripts)
        {
            var key = code.GetHashCode();
            if (!_cache.TryGetValue(key, out _))
            {
                var script = CSharpScript.Create(code, _options);
                script.Compile();
                _cache[key] = script;
            }
        }
    }
}