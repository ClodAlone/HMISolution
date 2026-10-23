using System.Text.Json;
using SharedModels;
using Xunit;

namespace Tests.SharedModels;

public class WidgetThemesTests
{
    [Fact]
    public void LegacyJson_WithoutTheme_StaysClassic()
    {
        var symbol = JsonSerializer.Deserialize<ScreenSymbol>("{\"Type\":\"alarmlist\"}")!;
        Assert.Equal("classic", symbol.WidgetTheme);
        Assert.False(WidgetThemes.IsThemed(symbol));
        Assert.Null(WidgetThemes.GetPalette(symbol.WidgetTheme));
        Assert.Equal("", WidgetThemes.CssVariables(symbol.WidgetTheme));
    }

    [Theory]
    [InlineData(null, "classic")]
    [InlineData("", "classic")]
    [InlineData("neon", "classic")]
    [InlineData(" Midnight ", "midnight")]
    [InlineData("IVORY", "ivory")]
    [InlineData("metallic", "metallic")]
    public void Normalize_FallsBackToClassicForUnknownValues(string? input, string expected) =>
        Assert.Equal(expected, WidgetThemes.Normalize(input));

    [Fact]
    public void UsesControlVisuals_OnlyForThemedControlTypes()
    {
        Assert.True(WidgetThemes.UsesControlVisuals(new ScreenSymbol { Type = "knob", WidgetTheme = "ivory" }));
        Assert.False(WidgetThemes.UsesControlVisuals(new ScreenSymbol { Type = "knob" }));
        Assert.False(WidgetThemes.UsesControlVisuals(new ScreenSymbol { Type = "alarmlist", WidgetTheme = "ivory" }));
        Assert.False(WidgetThemes.UsesControlVisuals(new ScreenSymbol { Type = "button", WidgetTheme = "ivory" }));
    }

    [Theory]
    [InlineData("midnight")]
    [InlineData("ivory")]
    [InlineData("metallic")]
    public void ApplyPreset_SetsCoordinatedColorsThatSurviveSerialization(string theme)
    {
        var symbol = new ScreenSymbol { Type = "hslider", VariablePath = "Plant.Speed", MinValue = 10, MaxValue = 90 };
        WidgetThemes.ApplyPreset(symbol, theme);
        var restored = JsonSerializer.Deserialize<ScreenSymbol>(JsonSerializer.Serialize(symbol))!;
        var palette = WidgetThemes.GetPalette(theme)!;

        Assert.Equal(theme, restored.WidgetTheme);
        Assert.Equal(palette.Plot, restored.TrendBackground);
        Assert.Equal(palette.SurfaceSolid, restored.AlarmAnalyticsBackground);
        Assert.StartsWith("#", restored.AlarmAnalyticsBackground);
        Assert.Equal(palette.Accent, restored.SliderFillColor);
        Assert.Equal(palette.Track, restored.SliderTrackColor);
        Assert.Equal(palette.Accent, restored.SwitchOnColor);
        Assert.Equal(palette.Primary, restored.ButtonColor);
        Assert.Equal("Plant.Speed", restored.VariablePath);
        Assert.Equal(10, restored.MinValue);
        Assert.Equal(90, restored.MaxValue);
    }

    [Fact]
    public void ApplyClassic_RestoresModelDefaults()
    {
        var defaults = new ScreenSymbol();
        var symbol = new ScreenSymbol();
        WidgetThemes.ApplyPreset(symbol, "metallic");
        WidgetThemes.ApplyPreset(symbol, "classic");

        Assert.Equal(defaults.WidgetTheme, symbol.WidgetTheme);
        Assert.Equal(defaults.TrendBackground, symbol.TrendBackground);
        Assert.Equal(defaults.AlarmAnalyticsBackground, symbol.AlarmAnalyticsBackground);
        Assert.Equal(defaults.AlarmAnalyticsForeground, symbol.AlarmAnalyticsForeground);
        Assert.Equal(defaults.SwitchOnColor, symbol.SwitchOnColor);
        Assert.Equal(defaults.SwitchOffColor, symbol.SwitchOffColor);
        Assert.Equal(defaults.SwitchKnobColor, symbol.SwitchKnobColor);
        Assert.Equal(defaults.SwitchDialColor, symbol.SwitchDialColor);
        Assert.Equal(defaults.KnobColor, symbol.KnobColor);
        Assert.Equal(defaults.KnobPointerColor, symbol.KnobPointerColor);
        Assert.Equal(defaults.SliderTrackColor, symbol.SliderTrackColor);
        Assert.Equal(defaults.SliderFillColor, symbol.SliderFillColor);
        Assert.Equal(defaults.SliderThumbColor, symbol.SliderThumbColor);
        Assert.Equal(defaults.RangeTrackColor, symbol.RangeTrackColor);
        Assert.Equal(defaults.RangeFillColor, symbol.RangeFillColor);
        Assert.Equal(defaults.RangeLowHandleColor, symbol.RangeLowHandleColor);
        Assert.Equal(defaults.RangeHighHandleColor, symbol.RangeHighHandleColor);
        Assert.Equal(defaults.ButtonColor, symbol.ButtonColor);
        Assert.Equal(defaults.ButtonTextColor, symbol.ButtonTextColor);
    }

    [Fact]
    public void ApplyPreset_RejectsUnknownTheme()
    {
        var symbol = new ScreenSymbol { WidgetTheme = "ivory" };
        Assert.Throws<ArgumentOutOfRangeException>(() => WidgetThemes.ApplyPreset(symbol, "neon"));
        Assert.Equal("ivory", symbol.WidgetTheme);
    }

    [Fact]
    public void CssVariables_ExposeThemeAndLegacyWidgetVariables()
    {
        var css = WidgetThemes.CssVariables("midnight");
        var palette = WidgetThemes.GetPalette("midnight")!;
        Assert.Contains($"--wt-accent: {palette.Accent};", css);
        Assert.Contains($"--wt-surface: {palette.Surface};", css);
        Assert.Contains($"--alarm-text: {palette.Text};", css);
        Assert.Contains($"--evtlog-border: {palette.Border};", css);
        Assert.DoesNotContain("\"", css);
    }

    [Fact]
    public void Labels_MatchGaugeNaming()
    {
        Assert.Equal(["Classic", "Midnight", "Ivory", "Brushed Metal"], WidgetThemes.Names.Select(WidgetThemes.Label));
    }

    [Fact]
    public void CaptionColor_UsesPaletteMutedWhenThemed_AndLegacyGreyOtherwise()
    {
        Assert.Equal("#333", WidgetThemes.CaptionColor(new ScreenSymbol { Type = "knob" }));
        Assert.Equal("#94a3b8", WidgetThemes.CaptionColor(new ScreenSymbol { Type = "knob", WidgetTheme = "midnight" }));
        Assert.Equal("#8a7a66", WidgetThemes.CaptionColor(new ScreenSymbol { Type = "switch", WidgetTheme = "ivory" }));
    }
}
