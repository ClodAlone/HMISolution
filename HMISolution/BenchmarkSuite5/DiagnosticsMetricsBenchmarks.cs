// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using BenchmarkDotNet.Attributes;
using SimpleOpcFileServer;
using Microsoft.VSDiagnostics;

namespace DiagnosticsMetricsBenchmarks;
[CPUUsageDiagnoser]
public class DiagnosticsMetricsBenchmarks
{
    private DiagnosticsCollector _collector = null!;
    [GlobalSetup]
    public void Setup()
    {
        _collector = new DiagnosticsCollector();
        _collector.Register("Driver", "Modbus", enabled: true, status: "Running");
        _collector.Register("Script", "Main", enabled: true, status: "Running");
        _collector.RecordCycle("Driver", "Modbus", 5.0);
        _collector.RecordCycle("Script", "Main", 12.0);
        // Prime the process metrics cache so ProcessSampleInterval guard is active
        _ = _collector.BuildSnapshot();
    }

    [Benchmark]
    public object BuildSnapshot_Cached()
    {
        // Typical hot path: called within the 1-second ProcessSampleInterval,
        // so SampleProcessMetrics should early-return. Measures snapshot assembly cost.
        return _collector.BuildSnapshot();
    }

    [Benchmark]
    public object BuildSnapshot_ColdMetrics()
    {
        // Force metrics refresh by resetting the interval guard via reflection
        // to simulate the case where the process metrics are stale.
        typeof(DiagnosticsCollector).GetField("_lastProcessSample", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(_collector, default(DateTime));
        return _collector.BuildSnapshot();
    }
}