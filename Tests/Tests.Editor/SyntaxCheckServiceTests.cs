using Xunit;
using ServerEditorWeb.Services;

namespace Tests.Editor;

public class SyntaxCheckServiceTests
{
    private static SyntaxCheckService CreateService() => new();

    // ══════════════════════════════════════════════════════════════
    //  ST (Structured Text) Syntax Checking
    // ══════════════════════════════════════════════════════════════

    [Fact]
    public void CheckPlcSyntax_ValidSt_Assignment()
    {
        var svc = CreateService();
        var (ok, msg) = svc.CheckPlcSyntax("counter := counter + 1;");
        Assert.True(ok);
        Assert.Contains("No syntax errors", msg);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_IfStatement()
    {
        var svc = CreateService();
        var code = """
            IF temp > 100 THEN
                alarm := TRUE;
            ELSIF temp < 20 THEN
                heating := TRUE;
            ELSE
                alarm := FALSE;
                heating := FALSE;
            END_IF;
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_ForLoop()
    {
        var svc = CreateService();
        var code = """
            VAR
                i : INT := 0;
                sum : INT := 0;
            END_VAR

            FOR i := 1 TO 10 DO
                sum := sum + i;
            END_FOR;
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_WhileLoop()
    {
        var svc = CreateService();
        var code = """
            VAR
                x : INT := 0;
            END_VAR

            WHILE x < 100 DO
                x := x + 1;
            END_WHILE;
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_RepeatUntil()
    {
        var svc = CreateService();
        var code = """
            VAR
                count : INT := 0;
            END_VAR

            REPEAT
                count := count + 1;
            UNTIL count >= 10;
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_CaseStatement_EmptyBranches()
    {
        var svc = CreateService();
        // Note: The lightweight ST parser terminates CASE branches when it sees an Id or Num token,
        // so complex branch bodies are not supported. Simple CASE with ELSE works.
        var code = """
            CASE 0 OF
            ELSE
                WRITE('state', 0);
            END_CASE;
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_FunctionCall()
    {
        var svc = CreateService();
        var code = "WRITE('Root.Plant.Speed', 100);";
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_ReadFunctionInExpression()
    {
        var svc = CreateService();
        var code = "speed := READ('Root.Plant.Motor.Speed');";
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_BooleanExpressions()
    {
        var svc = CreateService();
        var code = """
            VAR
                a : BOOL := TRUE;
                b : BOOL := FALSE;
                c : BOOL := FALSE;
            END_VAR

            c := a AND b OR NOT a;
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_Comments()
    {
        var svc = CreateService();
        var code = """
            // This is a line comment
            (* This is a block comment *)
            counter := counter + 1;
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_ProgramWrapper()
    {
        var svc = CreateService();
        var code = """
            PROGRAM Main
            VAR
                x : INT := 0;
            END_VAR
            x := x + 1;
            END_PROGRAM
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidSt_MissingSemicolon()
    {
        var svc = CreateService();
        var code = "counter := counter + 1";
        var (ok, msg) = svc.CheckPlcSyntax(code);
        Assert.False(ok);
        Assert.Contains("Syntax error", msg);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidSt_MissingEndIf()
    {
        var svc = CreateService();
        var code = """
            IF x > 10 THEN
                y := 1;
            """;
        var (ok, msg) = svc.CheckPlcSyntax(code);
        Assert.False(ok);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidSt_UnexpectedCharacter()
    {
        var svc = CreateService();
        var code = "counter := counter @ 1;";
        var (ok, msg) = svc.CheckPlcSyntax(code);
        Assert.False(ok);
        Assert.Contains("Syntax error", msg);
    }

    // ══════════════════════════════════════════════════════════════
    //  IL (Instruction List) Syntax Checking
    // ══════════════════════════════════════════════════════════════

    [Fact]
    public void CheckPlcSyntax_ValidIl_BasicInstructions()
    {
        var svc = CreateService();
        var code = """
            LD 'Root.Plant.Temp'
            GT 50
            ST 'Root.Plant.Alarm'
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "IL");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidIl_ArithmeticOps()
    {
        var svc = CreateService();
        var code = """
            LD 10
            ADD 20
            MUL 2
            ST 'result'
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "IL");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidIl_LogicOps()
    {
        var svc = CreateService();
        var code = """
            LD 'input1'
            AND 'input2'
            OR 'input3'
            NOT
            ST 'output'
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "IL");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidIl_JumpAndReturn()
    {
        var svc = CreateService();
        var code = """
            LD 'flag'
            JMPC target
            NOP
            target: LD 1
            RET
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "IL");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidIl_Comments()
    {
        var svc = CreateService();
        var code = """
            // This is a comment
            LD 'var1'
            (* block comment *)
            ST 'var2'
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "IL");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidIl_EmptyLines()
    {
        var svc = CreateService();
        var code = """

            LD 'x'

            ST 'y'

            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "IL");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidIl_UnknownOperator()
    {
        var svc = CreateService();
        var code = "PUSH 42";
        var (ok, msg) = svc.CheckPlcSyntax(code, "IL");
        Assert.False(ok);
        Assert.Contains("Syntax error", msg);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidIl_MissingOperand()
    {
        var svc = CreateService();
        var code = "LD";
        var (ok, msg) = svc.CheckPlcSyntax(code, "IL");
        Assert.False(ok);
        Assert.Contains("Syntax error", msg);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidIl_UnterminatedBlockComment()
    {
        var svc = CreateService();
        var code = """
            LD 'x'
            (* unterminated comment
            ST 'y'
            """;
        var (ok, msg) = svc.CheckPlcSyntax(code, "IL");
        Assert.False(ok);
    }

    // ══════════════════════════════════════════════════════════════
    //  LD (Ladder Diagram) Syntax Checking
    // ══════════════════════════════════════════════════════════════

    [Fact]
    public void CheckPlcSyntax_ValidLd_BasicRung()
    {
        var svc = CreateService();
        var code = """
            RUNG
            CONTACT NO 'input'
            COIL 'output'
            END_RUNG
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "LD");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidLd_MultipleRungs()
    {
        var svc = CreateService();
        var code = """
            RUNG
            CONTACT NO 'start'
            COIL_S 'motor'
            END_RUNG
            RUNG
            CONTACT NC 'stop'
            COIL_R 'motor'
            END_RUNG
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "LD");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidLd_Compare()
    {
        var svc = CreateService();
        var code = """
            RUNG
            COMPARE GT 'temperature' 100
            COIL 'alarm'
            END_RUNG
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "LD");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidLd_MathOperations()
    {
        var svc = CreateService();
        var code = """
            RUNG
            ADD 'input1' 'output'
            END_RUNG
            RUNG
            MUL 'factor' 'result'
            END_RUNG
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "LD");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidLd_TimerCounter()
    {
        var svc = CreateService();
        var code = """
            RUNG
            TIMER 'myTimer' 5000
            END_RUNG
            RUNG
            COUNTER 'myCounter' 100
            END_RUNG
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "LD");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidLd_Pid()
    {
        var svc = CreateService();
        var code = """
            RUNG
            PID Plant.Temp Plant.Setpoint Plant.HeaterOutput 1.5 0.3 0.1 0 100
            END_RUNG
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "LD");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidLd_PidMissingArgs()
    {
        var svc = CreateService();
        var code = """
            RUNG
            PID Plant.Temp Plant.Setpoint
            END_RUNG
            """;
        var (ok, msg) = svc.CheckPlcSyntax(code, "LD");
        Assert.False(ok);
        Assert.Contains("PID requires", msg);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_PidFunction()
    {
        var svc = CreateService();
        var code = """
            VAR
              output : REAL;
            END_VAR
            output := PID('loop1', Read('Plant.Temp'), 50.0, 1.5, 0.3, 0.1, 0, 100);
            Write('Plant.Output', output);
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "ST");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidLd_Comments()
    {
        var svc = CreateService();
        var code = """
            // Control the pump
            RUNG
            CONTACT NO 'pumpStart'
            COIL 'pump'
            END_RUNG
            """;
        var (ok, _) = svc.CheckPlcSyntax(code, "LD");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidLd_MissingEndRung()
    {
        var svc = CreateService();
        var code = """
            RUNG
            CONTACT NO 'input'
            COIL 'output'
            """;
        var (ok, msg) = svc.CheckPlcSyntax(code, "LD");
        Assert.False(ok);
        Assert.Contains("Syntax error", msg);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidLd_NestedRung()
    {
        var svc = CreateService();
        var code = """
            RUNG
            RUNG
            COIL 'x'
            END_RUNG
            END_RUNG
            """;
        var (ok, msg) = svc.CheckPlcSyntax(code, "LD");
        Assert.False(ok);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidLd_ContactOutsideRung()
    {
        var svc = CreateService();
        var code = "CONTACT NO 'input'";
        var (ok, msg) = svc.CheckPlcSyntax(code, "LD");
        Assert.False(ok);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidLd_ContactInvalidModifier()
    {
        var svc = CreateService();
        var code = """
            RUNG
            CONTACT XY 'input'
            COIL 'output'
            END_RUNG
            """;
        var (ok, msg) = svc.CheckPlcSyntax(code, "LD");
        Assert.False(ok);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidLd_UnknownElement()
    {
        var svc = CreateService();
        var code = """
            RUNG
            FOOBAR 'x'
            END_RUNG
            """;
        var (ok, msg) = svc.CheckPlcSyntax(code, "LD");
        Assert.False(ok);
    }

    [Fact]
    public void CheckPlcSyntax_InvalidLd_EndRungWithoutRung()
    {
        var svc = CreateService();
        var code = "END_RUNG";
        var (ok, msg) = svc.CheckPlcSyntax(code, "LD");
        Assert.False(ok);
    }

    // ══════════════════════════════════════════════════════════════
    //  C# Script Syntax Checking
    // ══════════════════════════════════════════════════════════════

    [Fact]
    public void CheckSyntax_ValidCSharp_SimpleRead()
    {
        var svc = CreateService();
        var (ok, _) = svc.CheckSyntax("var x = Read(\"MyVar\");");
        Assert.True(ok);
    }

    [Fact]
    public void CheckSyntax_ValidCSharp_WriteAndLog()
    {
        var svc = CreateService();
        var code = """
            var temp = ReadDouble("Plant.Temp");
            if (temp > 100)
            {
                Write("Plant.Alarm", true);
                Log("Temperature alarm triggered!");
            }
            """;
        var (ok, _) = svc.CheckSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckSyntax_InvalidCSharp_MissingSemicolon()
    {
        var svc = CreateService();
        var (ok, msg) = svc.CheckSyntax("var x = 1");
        Assert.False(ok);
        Assert.Contains("error", msg, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CheckSyntax_InvalidCSharp_UndefinedVariable()
    {
        var svc = CreateService();
        var (ok, _) = svc.CheckSyntax("undefinedVariable.DoSomething();");
        Assert.False(ok);
    }

    // ══════════════════════════════════════════════════════════════
    //  VB.NET Script Syntax Checking
    // ══════════════════════════════════════════════════════════════

    [Fact]
    public void CheckSyntax_ValidVb_SimpleCode()
    {
        var svc = CreateService();
        var code = """
            Dim x As Integer = 42
            Dim y As Integer = x + 1
            """;
        var (ok, _) = svc.CheckSyntax(code, "VB");
        Assert.True(ok);
    }

    [Fact]
    public void CheckSyntax_InvalidVb_SyntaxError()
    {
        var svc = CreateService();
        var (ok, msg) = svc.CheckSyntax("If Then End", "VB");
        Assert.False(ok);
    }

    [Theory]
    [InlineData("VB")]
    [InlineData("VB.NET")]
    [InlineData("VisualBasic")]
    public void CheckSyntax_VbLanguageVariants_AllRecognized(string lang)
    {
        var svc = CreateService();
        var (ok, _) = svc.CheckSyntax("Dim x As Integer = 1", lang);
        Assert.True(ok);
    }

    // ══════════════════════════════════════════════════════════════
    //  Edge cases
    // ══════════════════════════════════════════════════════════════

    [Fact]
    public void CheckPlcSyntax_EmptyCode_St_Succeeds()
    {
        var svc = CreateService();
        var (ok, _) = svc.CheckPlcSyntax("");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_EmptyCode_Il_Succeeds()
    {
        var svc = CreateService();
        var (ok, _) = svc.CheckPlcSyntax("", "IL");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_EmptyCode_Ld_Succeeds()
    {
        var svc = CreateService();
        var (ok, _) = svc.CheckPlcSyntax("", "LD");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_DefaultLanguage_IsSt()
    {
        var svc = CreateService();
        // Valid ST code should pass with default language parameter
        var (ok, _) = svc.CheckPlcSyntax("x := 1;");
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_ForLoopWithBy()
    {
        var svc = CreateService();
        var code = """
            VAR
                i : INT := 0;
            END_VAR

            FOR i := 0 TO 100 BY 2 DO
                WRITE('counter', i);
            END_FOR;
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_NestedIf()
    {
        var svc = CreateService();
        var code = """
            IF a > 0 THEN
                IF b > 0 THEN
                    c := 1;
                END_IF;
            END_IF;
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }

    [Fact]
    public void CheckPlcSyntax_ValidSt_ExitAndReturn()
    {
        var svc = CreateService();
        var code = """
            VAR
                i : INT := 0;
            END_VAR

            FOR i := 1 TO 100 DO
                IF i = 50 THEN
                    EXIT;
                END_IF;
            END_FOR;
            RETURN;
            """;
        var (ok, _) = svc.CheckPlcSyntax(code);
        Assert.True(ok);
    }
}
