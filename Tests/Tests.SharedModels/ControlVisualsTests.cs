using System.Globalization;
using System.Xml.Linq;
using SharedModels;
using Xunit;

namespace Tests.SharedModels;

public class ControlVisualsTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    [Theory]
    [InlineData("switch")]
    [InlineData("rotaryswitch")]
    [InlineData("knob")]
    [InlineData("hslider")]
    [InlineData("vslider")]
    [InlineData("rangeslider")]
    public void ClassicControls_AreNotRedrawn(string type)
    {
        var symbol = new ScreenSymbol { Type = type, Width = 120, Height = 60 };
        Assert.Null(ControlVisuals.Build(symbol, "1", 120, 60));
        Assert.Equal("", ControlVisuals.Render(symbol, "1", 120, 60));
    }

    [Theory]
    [InlineData("switch", "midnight")]
    [InlineData("rotaryswitch", "ivory")]
    [InlineData("knob", "metallic")]
    [InlineData("hslider", "midnight")]
    [InlineData("vslider", "ivory")]
    [InlineData("rangeslider", "metallic")]
    public void ThemedControls_RenderSizedSvgWithThemeClassAndGradients(string type, string theme)
    {
        var symbol = new ScreenSymbol { Type = type };
        WidgetThemes.ApplyPreset(symbol, theme);
        var svg = ControlVisuals.Build(symbol, "50", 120, 80, "sym-1")!;

        Assert.Equal(Svg + "svg", svg.Name);
        Assert.Equal("120", (string?)svg.Attribute("width"));
        Assert.Equal("80", (string?)svg.Attribute("height"));
        Assert.Equal($"wt-control wt-{theme}", (string?)svg.Attribute("class"));
        Assert.NotEmpty(svg.Descendants().Where(e => e.Name.LocalName is "linearGradient" or "radialGradient"));
        Assert.All(svg.Descendants().Select(e => (string?)e.Attribute("id")).Where(id => id is not null),
            id => Assert.StartsWith("wt_sym-1", id));
    }

    [Fact]
    public void HorizontalSliderThumb_MatchesRuntimeDragGeometry()
    {
        var symbol = new ScreenSymbol { Type = "hslider", WidgetTheme = "midnight", MinValue = 0, MaxValue = 100 };
        var svg = ControlVisuals.Build(symbol, "25", 200, 40, "s")!;
        // thumbR = min(40 * 0.35, 14) = 14, pad = 16, length = 200 - 32 = 168 → 16 + 168 * 0.25 = 58
        var thumb = Thumbs(svg).Single();
        Assert.Equal("58", (string?)thumb.Attribute("cx"));
        Assert.Equal("20", (string?)thumb.Attribute("cy"));
        Assert.Equal("14", (string?)thumb.Attribute("r"));
    }

    [Fact]
    public void VerticalSliderThumb_MovesUpAsValueIncreasesAndClampsOutOfRange()
    {
        var symbol = new ScreenSymbol { Type = "vslider", WidgetTheme = "ivory", MinValue = 0, MaxValue = 100 };
        // thumbR = min(36 * 0.35, 14) = 12.6, pad = 14.6, length = 200 - 29.2 = 170.8
        Assert.Equal("185.4", (string?)Thumbs(ControlVisuals.Build(symbol, "0", 36, 200, "v")!).Single().Attribute("cy"));
        Assert.Equal("14.6", (string?)Thumbs(ControlVisuals.Build(symbol, "100", 36, 200, "v")!).Single().Attribute("cy"));
        Assert.Equal("14.6", (string?)Thumbs(ControlVisuals.Build(symbol, "500", 36, 200, "v")!).Single().Attribute("cy"));
        Assert.Equal("185.4", (string?)Thumbs(ControlVisuals.Build(symbol, "not a number", 36, 200, "v")!).Single().Attribute("cy"));
    }

    [Fact]
    public void RangeSlider_UsesExplicitLowAndHighValues()
    {
        var symbol = new ScreenSymbol { Type = "rangeslider", WidgetTheme = "metallic", MinValue = 0, MaxValue = 100 };
        var svg = ControlVisuals.Build(symbol, null, 200, 40, "r", rangeLow: 0, rangeHigh: 100)!;
        // thumbR = min(14, 12) = 12, pad = 14
        Assert.Equal(["14", "186"], Thumbs(svg).Select(t => (string)t.Attribute("cx")!));
    }

    [Fact]
    public void Switch_MovesKnobWhenOn()
    {
        var symbol = new ScreenSymbol { Type = "switch", WidgetTheme = "midnight" };
        static string KnobStyle(XElement svg) =>
            (string)svg.Descendants(Svg + "circle").Single(c => (string?)c.Attribute("class") == "wt-switch-knob").Attribute("style")!;

        Assert.Contains("translateX(0px)", KnobStyle(ControlVisuals.Build(symbol, "False", 60, 30, "sw")!));
        Assert.Contains("translateX(30px)", KnobStyle(ControlVisuals.Build(symbol, "True", 60, 30, "sw")!));
    }

    [Fact]
    public void IdPrefix_IsSanitizedAndUniqueWhenMissing()
    {
        var symbol = new ScreenSymbol { Type = "knob", WidgetTheme = "ivory" };
        var markup = ControlVisuals.Render(symbol, "10", 100, 100, "a b\"<c>");
        Assert.Contains("id=\"wt_a_b__c__", markup);
        Assert.DoesNotContain("<c>", markup);

        var first = (string?)ControlVisuals.Build(symbol, "10", 100, 100)!.Descendants().First(e => e.Attribute("id") is not null).Attribute("id");
        var second = (string?)ControlVisuals.Build(symbol, "10", 100, 100)!.Descendants().First(e => e.Attribute("id") is not null).Attribute("id");
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Markup_UsesInvariantNumbersUnderCommaCultures()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            var symbol = new ScreenSymbol { Type = "hslider", WidgetTheme = "midnight", SliderShowValue = true };
            var svg = ControlVisuals.Build(symbol, "12.5", 201.5, 40, "c")!;
            Assert.Equal("201.5", (string?)svg.Attribute("width"));
            Assert.Contains(svg.Descendants(Svg + "text"), t => t.Value == "12.5");
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Theory]
    [InlineData("switch", null, 51, 31)]
    [InlineData("knob", null, 80, 90)]
    [InlineData("hslider", null, 160, 36)]
    [InlineData("vslider", null, 36, 160)]
    [InlineData("rangeslider", "horizontal", 160, 36)]
    [InlineData("rangeslider", "vertical", 36, 160)]
    public void ResponsiveSize_MatchesControlOrientation(string type, string? orientation, double width, double height)
    {
        var symbol = new ScreenSymbol { Type = type };
        if (orientation is not null) symbol.RangeOrientation = orientation;
        Assert.Equal((width, height), ControlVisuals.ResponsiveSize(symbol));
    }

    private static IEnumerable<XElement> Thumbs(XElement svg) =>
        svg.Descendants(Svg + "circle").Where(c => (string?)c.Attribute("class") == "wt-thumb");
}
