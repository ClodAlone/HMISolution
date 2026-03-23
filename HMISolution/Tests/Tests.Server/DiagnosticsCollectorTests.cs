using Xunit;
using SimpleOpcFileServer;
using SharedModels;

namespace Tests.Server;

public class DiagnosticsCollectorTests
{
    /// <summary>
    /// Creates a fresh DiagnosticsCollector instance for isolated testing.
    /// We avoid using the singleton Instance since tests run in parallel.
    /// </summary>
    private static DiagnosticsCollector CreateCollector() => new();

    // ──────────────────────────────────────────────────────────────
    // Register
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void Register_AddsSubsystemToSnapshot()
    {
        var collector = CreateCollector();
        collector.Register("Driver", "Modbus", enabled: true, status: "Running");

        var snapshot = collector.BuildSnapshot();
        var sub = Assert.Single(snapshot.Subsystems);
        Assert.Equal("Driver", sub.Category);
        Assert.Equal("Modbus", sub.Name);
        Assert.True(sub.Enabled);
        Assert.Equal("Running", sub.Status);
    }

    [Fact]
    public void Register_DisabledSubsystem()
    {
        var collector = CreateCollector();
        collector.Register("Script", "Unused", enabled: false, status: "Disabled");

        var snapshot = collector.BuildSnapshot();
        var sub = Assert.Single(snapshot.Subsystems);
        Assert.False(sub.Enabled);
        Assert.Equal("Disabled", sub.Status);
    }

    [Fact]
    public void Register_MultipleSubsystems()
    {
        var collector = CreateCollector();
        collector.Register("Driver", "Modbus");
        collector.Register("Driver", "S7");
        collector.Register("Script", "Main");

        var snapshot = collector.BuildSnapshot();
        Assert.Equal(3, snapshot.Subsystems.Count);
    }

    // ──────────────────────────────────────────────────────────────
    // RecordCycle
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void RecordCycle_TracksMetrics()
    {
        var collector = CreateCollector();
        collector.RecordCycle("Script", "Main", 10.5);
        collector.RecordCycle("Script", "Main", 20.3);
        collector.RecordCycle("Script", "Main", 5.2);

        var snapshot = collector.BuildSnapshot();
        var sub = Assert.Single(snapshot.Subsystems);
        Assert.Equal(3, sub.CycleCount);
        Assert.Equal(5.2, sub.LastCycleMs, 1);
        Assert.Equal(20.3, sub.MaxCycleMs, 1);
        Assert.True(sub.AvgCycleMs > 0);
        Assert.Equal("Running", sub.Status);
    }

    [Fact]
    public void RecordCycle_WithError_SetsErrorStatus()
    {
        var collector = CreateCollector();
        collector.RecordCycle("PLC", "Prog1", 15.0, error: "Division by zero");

        var snapshot = collector.BuildSnapshot();
        var sub = Assert.Single(snapshot.Subsystems);
        Assert.Equal("Error", sub.Status);
        Assert.Equal("Division by zero", sub.LastError);
    }

    [Fact]
    public void RecordCycle_AfterError_ClearsErrorOnSuccess()
    {
        var collector = CreateCollector();
        collector.RecordCycle("PLC", "Prog1", 15.0, error: "fail");
        collector.RecordCycle("PLC", "Prog1", 10.0); // success

        var snapshot = collector.BuildSnapshot();
        var sub = Assert.Single(snapshot.Subsystems);
        Assert.Equal("Running", sub.Status);
    }

    [Fact]
    public void RecordCycle_MaxCycleMs_TracksHighestValue()
    {
        var collector = CreateCollector();
        collector.RecordCycle("Script", "A", 10.0);
        collector.RecordCycle("Script", "A", 50.0);
        collector.RecordCycle("Script", "A", 30.0);

        var snapshot = collector.BuildSnapshot();
        var sub = Assert.Single(snapshot.Subsystems);
        Assert.Equal(50.0, sub.MaxCycleMs, 1);
    }

    [Fact]
    public void RecordCycle_TotalCpuMs_Accumulates()
    {
        var collector = CreateCollector();
        collector.RecordCycle("Script", "A", 10.0);
        collector.RecordCycle("Script", "A", 20.0);
        collector.RecordCycle("Script", "A", 30.0);

        var snapshot = collector.BuildSnapshot();
        var sub = Assert.Single(snapshot.Subsystems);
        Assert.Equal(60.0, sub.TotalCpuMs, 1);
    }

    // ──────────────────────────────────────────────────────────────
    // SetStatus
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void SetStatus_UpdatesExistingSubsystem()
    {
        var collector = CreateCollector();
        collector.Register("Driver", "Modbus");
        collector.SetStatus("Driver", "Modbus", "Error", "Connection lost");

        var snapshot = collector.BuildSnapshot();
        var sub = Assert.Single(snapshot.Subsystems);
        Assert.Equal("Error", sub.Status);
        Assert.Equal("Connection lost", sub.LastError);
    }

    [Fact]
    public void SetStatus_NonExistentSubsystem_NoException()
    {
        var collector = CreateCollector();
        // Should not throw — just ignore
        collector.SetStatus("Driver", "NonExistent", "Error", "fail");

        var snapshot = collector.BuildSnapshot();
        Assert.Empty(snapshot.Subsystems);
    }

    // ──────────────────────────────────────────────────────────────
    // RecordDebug
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void RecordDebug_AddsDebugInfoToSnapshot()
    {
        var collector = CreateCollector();
        var debugInfo = new ProgramDebugInfo
        {
            Category = "Script",
            Name = "Main",
            Status = "Running",
            CycleCount = 42
        };
        collector.RecordDebug(debugInfo);

        var snapshot = collector.BuildSnapshot();
        var debug = Assert.Single(snapshot.ProgramDebug);
        Assert.Equal("Script", debug.Category);
        Assert.Equal("Main", debug.Name);
        Assert.Equal(42, debug.CycleCount);
    }

    [Fact]
    public void RecordDebug_OverwritesPreviousSnapshot()
    {
        var collector = CreateCollector();
        collector.RecordDebug(new ProgramDebugInfo { Category = "Script", Name = "Main", CycleCount = 1 });
        collector.RecordDebug(new ProgramDebugInfo { Category = "Script", Name = "Main", CycleCount = 99 });

        var snapshot = collector.BuildSnapshot();
        var debug = Assert.Single(snapshot.ProgramDebug);
        Assert.Equal(99, debug.CycleCount);
    }

    // ──────────────────────────────────────────────────────────────
    // BuildSnapshot
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void BuildSnapshot_IncludesProcessMetrics()
    {
        var collector = CreateCollector();
        var snapshot = collector.BuildSnapshot();

        Assert.True(snapshot.MemoryMB > 0);
        Assert.True(snapshot.ThreadCount > 0);
        Assert.True(snapshot.Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void BuildSnapshot_EmptyCollector_ReturnsEmptyLists()
    {
        var collector = CreateCollector();
        var snapshot = collector.BuildSnapshot();

        Assert.Empty(snapshot.Subsystems);
        Assert.Empty(snapshot.ProgramDebug);
    }

    [Fact]
    public void BuildSnapshot_AverageCycleMs_CalculatedCorrectly()
    {
        var collector = CreateCollector();
        collector.RecordCycle("Script", "Avg", 10.0);
        collector.RecordCycle("Script", "Avg", 20.0);

        var snapshot = collector.BuildSnapshot();
        var sub = Assert.Single(snapshot.Subsystems);
        Assert.Equal(15.0, sub.AvgCycleMs, 1);
    }

    // ──────────────────────────────────────────────────────────────
    // Start with port 0 (disabled)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void Start_PortZero_DoesNotThrow()
    {
        var collector = CreateCollector();
        collector.Start(0); // Should be a no-op
        collector.Dispose();
    }

    [Fact]
    public void Start_NegativePort_DoesNotThrow()
    {
        var collector = CreateCollector();
        collector.Start(-1);
        collector.Dispose();
    }
}
