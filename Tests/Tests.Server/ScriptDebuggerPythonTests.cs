// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Xunit;
using SimpleOpcFileServer;

namespace Tests.Server;

public class ScriptDebuggerPythonTests
{
    [Fact]
    public void InstrumentPythonSource_SimpleAssignment_InjectsCheckpoint()
    {
        var code = "x = 1\ny = 2";
        var result = ScriptDebugger.InstrumentPythonSource(code);

        Assert.Contains("__DebugCheckpoint(0); x = 1", result);
        Assert.Contains("__DebugCheckpoint(1); y = 2", result);
    }

    [Fact]
    public void InstrumentPythonSource_IfBlock_DoesNotInstrumentHeader()
    {
        var code = "if temp > 100:\n    Write(\"Alarm\", True)\nelse:\n    Write(\"Alarm\", False)";
        var result = ScriptDebugger.InstrumentPythonSource(code);

        // Block headers (end with ':') must be preserved verbatim - instrumenting them breaks Python syntax.
        Assert.Contains("if temp > 100:", result);
        Assert.DoesNotContain("__DebugCheckpoint(0); if", result);
        Assert.Contains("else:", result);
        Assert.DoesNotContain("__DebugCheckpoint(2); else", result);

        // Body statements inside the block ARE instrumented.
        Assert.Contains("__DebugCheckpoint(1);", result);
        Assert.Contains("__DebugCheckpoint(3);", result);
    }

    [Fact]
    public void InstrumentPythonSource_CommentsAndBlankLines_Untouched()
    {
        var code = "# comment\n\nx = 1";
        var result = ScriptDebugger.InstrumentPythonSource(code);

        Assert.Contains("# comment", result);
        Assert.DoesNotContain("__DebugCheckpoint(0)", result);
        Assert.Contains("__DebugCheckpoint(2); x = 1", result);
    }

    [Fact]
    public void InstrumentPythonSource_PreservesIndentation()
    {
        var code = "for i in range(10):\n    x = i * 2\n    Write(\"Count\", x)";
        var result = ScriptDebugger.InstrumentPythonSource(code);

        var lines = result.Replace("\r\n", "\n").Split('\n');
        Assert.Contains(lines, l => l.StartsWith("    __DebugCheckpoint(1); x = i * 2"));
        Assert.Contains(lines, l => l.StartsWith("    __DebugCheckpoint(2); Write"));
    }

    [Fact]
    public void InstrumentPythonSource_TripleQuotedString_NotInstrumented()
    {
        var code = "s = \"\"\"\nmulti\nline\n\"\"\"\nx = 1";
        var result = ScriptDebugger.InstrumentPythonSource(code);

        // Lines inside the triple-quoted string block must remain untouched.
        Assert.DoesNotContain("__DebugCheckpoint(1); multi", result);
        Assert.DoesNotContain("__DebugCheckpoint(2); line", result);
        // Final statement after the string still gets instrumented.
        Assert.Contains("__DebugCheckpoint(4); x = 1", result);
    }

    [Fact]
    public void HasActiveSession_NoSession_ReturnsFalse()
    {
        Assert.False(ScriptDebugger.Instance.HasActiveSession("NonExistentPythonScript_" + System.Guid.NewGuid()));
    }
}
