using Xunit;
using SharedModels;
using System.Text.Json;

namespace Tests.SharedModels;

public class HolidayCalendarsTests
{
    [Fact]
    public void BuiltIn_ContainsExpectedLocales()
    {
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("US"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("DE"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("IT"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("FR"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("UK"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("ES"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("JP"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("BR"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("CA"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("AU"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("IN"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("CN"));
    }

    [Fact]
    public void BuiltIn_CaseInsensitiveLookup()
    {
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("us"));
        Assert.True(HolidayCalendars.BuiltIn.ContainsKey("Us"));
    }

    [Fact]
    public void BuiltIn_US_ContainsChristmas()
    {
        var holidays = HolidayCalendars.BuiltIn["US"];
        Assert.Contains(holidays, h => h.Month == 12 && h.Day == 25);
    }

    [Fact]
    public void BuiltIn_US_ContainsNewYearsDay()
    {
        var holidays = HolidayCalendars.BuiltIn["US"];
        Assert.Contains(holidays, h => h.Month == 1 && h.Day == 1 && h.Name.Contains("New Year"));
    }

    [Fact]
    public void BuiltIn_IT_ContainsFerragosto()
    {
        var holidays = HolidayCalendars.BuiltIn["IT"];
        Assert.Contains(holidays, h => h.Month == 8 && h.Day == 15 && h.Name == "Ferragosto");
    }

    [Fact]
    public void BuiltIn_DE_ContainsTagDerDeutschenEinheit()
    {
        var holidays = HolidayCalendars.BuiltIn["DE"];
        Assert.Contains(holidays, h => h.Month == 10 && h.Day == 3);
    }

    [Fact]
    public void BuiltIn_FR_ContainsFeteNationale()
    {
        var holidays = HolidayCalendars.BuiltIn["FR"];
        Assert.Contains(holidays, h => h.Month == 7 && h.Day == 14);
    }

    [Fact]
    public void BuiltIn_JP_ContainsBuiltInHolidays()
    {
        var holidays = HolidayCalendars.BuiltIn["JP"];
        Assert.True(holidays.Count >= 10);
        Assert.Contains(holidays, h => h.Month == 1 && h.Day == 1); // New Year
    }

    [Fact]
    public void BuiltIn_AllHolidays_HaveValidMonthAndDay()
    {
        foreach (var (locale, holidays) in HolidayCalendars.BuiltIn)
        {
            foreach (var h in holidays)
            {
                Assert.InRange(h.Month, 1, 12);
                Assert.InRange(h.Day, 1, 31);
                Assert.False(string.IsNullOrEmpty(h.Name), $"Holiday in {locale} has empty name");
            }
        }
    }

    [Fact]
    public void BuiltIn_AllHolidays_HaveYearZero_ForRecurring()
    {
        // All built-in holidays should have Year = 0 (recurring)
        foreach (var (locale, holidays) in HolidayCalendars.BuiltIn)
        {
            foreach (var h in holidays)
            {
                Assert.Equal(0, h.Year);
            }
        }
    }

    [Fact]
    public void AvailableLocales_ReturnsSortedArray()
    {
        var locales = HolidayCalendars.AvailableLocales;
        Assert.True(locales.Length > 0);

        // Should be sorted
        for (int i = 1; i < locales.Length; i++)
        {
            Assert.True(string.Compare(locales[i - 1], locales[i], StringComparison.Ordinal) <= 0,
                $"Locales not sorted: {locales[i - 1]} before {locales[i]}");
        }
    }

    [Fact]
    public void AvailableLocales_MatchesBuiltInKeys()
    {
        var locales = HolidayCalendars.AvailableLocales;
        Assert.Equal(HolidayCalendars.BuiltIn.Count, locales.Length);
    }
}

public class ResourceFileManagerTests
{
    [Theory]
    [InlineData("Simple", "Simple")]
    [InlineData("My Script", "My Script")]
    [InlineData("test/file", "test_file")]
    [InlineData("file:name", "file_name")]
    [InlineData("valid_name-1", "valid_name-1")]
    public void SanitizeFileName_ReplacesInvalidChars(string input, string expected)
    {
        Assert.Equal(expected, ResourceFileManager.SanitizeFileName(input));
    }

    [Fact]
    public void SanitizeFileName_EmptyString_ReturnsEmpty()
    {
        Assert.Equal("", ResourceFileManager.SanitizeFileName(""));
    }

    [Fact]
    public void SanitizeFileName_PreservesNormalCharacters()
    {
        var result = ResourceFileManager.SanitizeFileName("MyScript_v2.0");
        Assert.Equal("MyScript_v2.0", result);
    }
}

public class NodeModelDefaultsTests
{
    [Fact]
    public void NodeModel_DefaultLists_AreEmpty()
    {
        var model = new NodeModel();
        Assert.NotNull(model.Scripts);
        Assert.Empty(model.Scripts);
        Assert.NotNull(model.Screens);
        Assert.Empty(model.Screens);
        Assert.NotNull(model.PlcPrograms);
        Assert.Empty(model.PlcPrograms);
        Assert.NotNull(model.Recipes);
        Assert.Empty(model.Recipes);
        Assert.NotNull(model.Schedulers);
        Assert.Empty(model.Schedulers);
        Assert.NotNull(model.Reports);
        Assert.Empty(model.Reports);
        Assert.NotNull(model.Cameras);
        Assert.Empty(model.Cameras);
        Assert.NotNull(model.Users);
        Assert.Empty(model.Users);
        Assert.NotNull(model.UserGroups);
        Assert.Empty(model.UserGroups);
    }

    [Fact]
    public void Variable_DefaultValues()
    {
        var v = new Variable();
        Assert.Equal("", v.Name);
        Assert.Equal("", v.Type);
        Assert.Equal("ReadWrite", v.Access);
        Assert.Null(v.Alarm);
        Assert.Null(v.DataLogging);
        Assert.Null(v.DriverConfigs);
    }

    [Fact]
    public void ScreenSymbol_DefaultValues()
    {
        var sym = new ScreenSymbol();
        Assert.Equal("", sym.Id);
        Assert.Equal("rect", sym.Type);
        Assert.NotNull(sym.Commands);
        Assert.Empty(sym.Commands);
        Assert.NotNull(sym.Animations);
        Assert.Empty(sym.Animations);
        Assert.NotNull(sym.HdaVariablePaths);
        Assert.Empty(sym.HdaVariablePaths);
        Assert.NotNull(sym.TrendPens);
        Assert.Empty(sym.TrendPens);
        Assert.NotNull(sym.EmbeddedScreens);
        Assert.Empty(sym.EmbeddedScreens);
    }

    [Fact]
    public void AlarmConfig_DefaultValues()
    {
        var alarm = new AlarmConfig();
        Assert.Equal(AlarmTriggerType.Limit, alarm.TriggerType);
        Assert.Equal(0, alarm.Hysteresis);
        Assert.Null(alarm.HighHighLimit);
        Assert.Null(alarm.LowLowLimit);
    }

    [Fact]
    public void ScriptConfig_DefaultValues()
    {
        var script = new ScriptConfig();
        Assert.Equal("", script.Name);
        Assert.True(script.Enabled);
        Assert.Equal("CSharp", script.Language);
        Assert.Equal(1000, script.IntervalMs);
    }

    [Fact]
    public void PlcProgramConfig_DefaultValues()
    {
        var plc = new PlcProgramConfig();
        Assert.Equal("", plc.Name);
        Assert.True(plc.Enabled);
        Assert.Equal("ST", plc.Language);
        Assert.Equal(100, plc.IntervalMs);
    }

    [Fact]
    public void SchedulerConfig_DefaultValues()
    {
        var sched = new SchedulerConfig();
        Assert.Equal("", sched.Name);
        Assert.True(sched.Enabled);
        Assert.Equal(60, sched.SlotMinutes);
        Assert.Equal("Same", sched.WeekendMode);
        Assert.Equal("Same", sched.HolidayMode);
        Assert.NotNull(sched.WeeklySlots);
        Assert.NotNull(sched.WeekendSlots);
        Assert.NotNull(sched.Commands);
        Assert.NotNull(sched.DeactivateCommands);
        Assert.NotNull(sched.CustomHolidays);
    }

    [Fact]
    public void UserConfig_DefaultValues()
    {
        var user = new UserConfig();
        Assert.Equal("", user.Username);
        Assert.False(user.MustChangePasswordOnFirstLogin);
        Assert.False(user.HasLoggedInBefore);
    }

    [Fact]
    public void ScreenConfig_DefaultValues()
    {
        var screen = new ScreenConfig();
        Assert.Equal("", screen.Name);
        Assert.Equal(800, screen.Width);
        Assert.Equal(600, screen.Height);
        Assert.NotNull(screen.Symbols);
        Assert.Empty(screen.Symbols);
    }

    [Fact]
    public void Folder_DefaultValues()
    {
        var folder = new Folder();
        Assert.Equal("", folder.Name);
        Assert.NotNull(folder.Variables);
        Assert.Empty(folder.Variables);
        Assert.NotNull(folder.Folders);
        Assert.Empty(folder.Folders);
    }
}

public class AlarmEvaluatorConditionTests
{
    // ──────────────────────────────────────────────────────────────
    // EvaluateConditionActivation (delegates to EvaluateConditionExpression)
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(">", "50", 100.0, true)]
    [InlineData(">", "50", 30.0, false)]
    [InlineData("<", "50", 30.0, true)]
    [InlineData("==", "50", 50.0, true)]
    [InlineData("!=", "50", 51.0, true)]
    public void EvaluateConditionActivation_MatchesExpression(string op, string compare, double value, bool expected)
    {
        Assert.Equal(expected, AlarmEvaluator.EvaluateConditionActivation(op, compare, value));
    }

    // ──────────────────────────────────────────────────────────────
    // EvaluateConditionDeactivation — more edge cases
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void ConditionDeactivation_GreaterThanOrEqual_WithHysteresis()
    {
        // Activated at value >= 50. Deactivate when value < 50 - 5 = 45
        Assert.True(AlarmEvaluator.EvaluateConditionDeactivation(">=", "50", 5.0, 44.0));
        Assert.False(AlarmEvaluator.EvaluateConditionDeactivation(">=", "50", 5.0, 46.0));
    }

    [Fact]
    public void ConditionDeactivation_LessThanOrEqual_WithHysteresis()
    {
        // Activated at value <= 50. Deactivate when value > 50 + 5 = 55
        Assert.True(AlarmEvaluator.EvaluateConditionDeactivation("<=", "50", 5.0, 56.0));
        Assert.False(AlarmEvaluator.EvaluateConditionDeactivation("<=", "50", 5.0, 54.0));
    }

    [Fact]
    public void ConditionDeactivation_Equal_WithHysteresis()
    {
        // Activated at value == 50. Deactivate when |value - 50| > hysteresis
        Assert.True(AlarmEvaluator.EvaluateConditionDeactivation("==", "50", 5.0, 56.0));
        Assert.False(AlarmEvaluator.EvaluateConditionDeactivation("==", "50", 5.0, 52.0));
    }

    [Fact]
    public void ConditionDeactivation_NotEqual_WithHysteresis()
    {
        // Activated at value != 50. Deactivate when |value - 50| <= hysteresis
        Assert.True(AlarmEvaluator.EvaluateConditionDeactivation("!=", "50", 5.0, 50.0));
        Assert.False(AlarmEvaluator.EvaluateConditionDeactivation("!=", "50", 5.0, 56.0));
    }

    [Fact]
    public void ConditionDeactivation_NonNumericValue_FallsBackToNegation()
    {
        // When value is a non-numeric string, deactivation falls back to !Evaluate
        Assert.True(AlarmEvaluator.EvaluateConditionDeactivation("==", "hello", 5.0, "world"));
        Assert.False(AlarmEvaluator.EvaluateConditionDeactivation("==", "hello", 5.0, "hello"));
    }

    // ──────────────────────────────────────────────────────────────
    // EvaluateConditionExpression — Boolean string values
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void ConditionExpression_True_IntNonZero_ReturnsTrue()
    {
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("True", "", 1));
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("True", "", 42));
    }

    [Fact]
    public void ConditionExpression_True_IntZero_ReturnsFalse()
    {
        Assert.False(AlarmEvaluator.EvaluateConditionExpression("True", "", 0));
    }

    [Fact]
    public void ConditionExpression_True_DoubleNonZero_ReturnsTrue()
    {
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("True", "", 1.0));
    }

    [Fact]
    public void ConditionExpression_True_DoubleZero_ReturnsFalse()
    {
        Assert.False(AlarmEvaluator.EvaluateConditionExpression("True", "", 0.0));
    }

    [Fact]
    public void ConditionExpression_True_StringTrue_ReturnsTrue()
    {
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("True", "", "true"));
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("True", "", "1"));
    }

    [Fact]
    public void ConditionExpression_True_StringFalse_ReturnsFalse()
    {
        Assert.False(AlarmEvaluator.EvaluateConditionExpression("True", "", "false"));
        Assert.False(AlarmEvaluator.EvaluateConditionExpression("True", "", "0"));
    }

    [Fact]
    public void ConditionExpression_False_BoolFalse_ReturnsTrue()
    {
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("False", "", false));
    }

    [Fact]
    public void ConditionExpression_False_IntZero_ReturnsTrue()
    {
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("False", "", 0));
    }

    [Fact]
    public void ConditionExpression_False_StringEmpty_ReturnsTrue()
    {
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("False", "", ""));
    }

    [Fact]
    public void ConditionExpression_False_Null_ReturnsTrue()
    {
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("False", "", null));
    }

    [Fact]
    public void ConditionExpression_StringEquality_CaseInsensitive()
    {
        Assert.True(AlarmEvaluator.EvaluateConditionExpression("==", "Hello", "hello"));
    }

    [Fact]
    public void ConditionExpression_StringInequality_CaseInsensitive()
    {
        Assert.False(AlarmEvaluator.EvaluateConditionExpression("!=", "Hello", "hello"));
    }

    [Fact]
    public void ConditionExpression_UnknownOperator_NumericValues_ReturnsFalse()
    {
        Assert.False(AlarmEvaluator.EvaluateConditionExpression("??", "50", 50.0));
    }

    [Fact]
    public void ConditionExpression_UnknownOperator_StringValues_ReturnsFalse()
    {
        Assert.False(AlarmEvaluator.EvaluateConditionExpression("??", "hello", "hello"));
    }

    // ──────────────────────────────────────────────────────────────
    // LimitAlarm — HighHigh vs High distinction
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void LimitAlarm_ValueAboveHigh_BelowHighHigh_ActivatesHigh()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20, HighHighLimit = 100 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(85.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.True(result.ShouldActivate);
        Assert.Equal("High", result.LimitState);
        Assert.Equal((ushort)700, result.Severity);
    }

    [Fact]
    public void LimitAlarm_ValueBelowLow_AboveLowLow_ActivatesLow()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20, LowLowLimit = 5 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(15.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.True(result.ShouldActivate);
        Assert.Equal("Low", result.LimitState);
        Assert.Equal((ushort)500, result.Severity);
    }

    [Fact]
    public void LimitAlarm_LimitTextContainsDescription()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(100.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.Contains("limit", result.LimitText, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void LimitAlarm_WithinLimits_NoText()
    {
        var cfg = new AlarmConfig { HighLimit = 80, LowLimit = 20 };
        var result = AlarmEvaluator.EvaluateLimitAlarm(50.0, cfg, isCurrentlyActive: false);

        Assert.NotNull(result);
        Assert.Equal("", result.LimitState);
        Assert.Equal("", result.LimitText);
        Assert.Equal((ushort)0, result.Severity);
    }
}

public class ResourceFileManagerDetachReattachTests
{
    [Fact]
    public void DetachResources_ClearsLists()
    {
        var model = new NodeModel
        {
            Scripts = new() { new ScriptConfig { Name = "S1" } },
            Screens = new() { new ScreenConfig { Name = "Scr1" } },
            PlcPrograms = new() { new PlcProgramConfig { Name = "P1" } },
            Recipes = new() { new RecipeConfig { Name = "R1" } }
        };

        var snapshot = ResourceFileManager.DetachResources(model);

        Assert.Empty(model.Scripts);
        Assert.Empty(model.Screens);
        Assert.Empty(model.PlcPrograms);
        Assert.Empty(model.Recipes);
        Assert.Single(snapshot.Scripts);
        Assert.Single(snapshot.Screens);
        Assert.Single(snapshot.PlcPrograms);
        Assert.Single(snapshot.Recipes);
    }

    [Fact]
    public void ReattachResources_RestoresLists()
    {
        var model = new NodeModel
        {
            Scripts = new() { new ScriptConfig { Name = "S1" } },
            Screens = new() { new ScreenConfig { Name = "Scr1" } },
            PlcPrograms = new() { new PlcProgramConfig { Name = "P1" } },
            Recipes = new() { new RecipeConfig { Name = "R1" } }
        };

        var snapshot = ResourceFileManager.DetachResources(model);
        Assert.Empty(model.Scripts);

        ResourceFileManager.ReattachResources(model, snapshot);

        Assert.Single(model.Scripts);
        Assert.Equal("S1", model.Scripts[0].Name);
        Assert.Single(model.Screens);
        Assert.Single(model.PlcPrograms);
        Assert.Single(model.Recipes);
    }

    [Fact]
    public void DetachReattach_Roundtrip_PreservesAllData()
    {
        var model = new NodeModel
        {
            Scripts = new()
            {
                new ScriptConfig { Name = "Script1", Code = "var x = 1;", IntervalMs = 500 },
                new ScriptConfig { Name = "Script2", Code = "var y = 2;", IntervalMs = 1000 }
            },
            Screens = new()
            {
                new ScreenConfig { Name = "Main", Width = 1920, Height = 1080 }
            },
            PlcPrograms = new(),
            Recipes = new()
        };

        var snapshot = ResourceFileManager.DetachResources(model);

        // Model can now be serialized without resources
        var json = JsonSerializer.Serialize(model);
        Assert.DoesNotContain("Script1", json);

        ResourceFileManager.ReattachResources(model, snapshot);

        Assert.Equal(2, model.Scripts.Count);
        Assert.Equal("Script1", model.Scripts[0].Name);
        Assert.Equal("var x = 1;", model.Scripts[0].Code);
    }
}
