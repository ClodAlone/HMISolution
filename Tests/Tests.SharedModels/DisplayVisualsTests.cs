using System.Text.Json;
using System.Xml.Linq;
using SharedModels;
using Xunit;

namespace Tests.SharedModels;

public class DisplayVisualsTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static readonly XNamespace Html = "http://www.w3.org/1999/xhtml";

    [Theory]
    [InlineData("editbox")]
    [InlineData("numericdisplay")]
    public void LegacyJson_LeavesAppearanceUnchanged(string type)
    {
        var symbol = JsonSerializer.Deserialize<ScreenSymbol>($"{{\"Type\":\"{type}\"}}")!;
        Assert.False(DisplayVisuals.IsEnhanced(symbol));
        Assert.Equal("classic", symbol.DisplayBackgroundStyle);
        Assert.Equal("default", symbol.DisplayFontStyle);
        Assert.False(symbol.DisplayAutoFit);
    }

    [Theory]
    [InlineData("midnight", "#22d3ee")]
    [InlineData("ivory", "#78350f")]
    [InlineData("metallic", "#0c4a6e")]
    [InlineData("lcd", "#24351c")]
    public void PresetAndRoundTrip_PreserveAppearanceAndProcessConfiguration(string preset, string color)
    {
        foreach (var type in new[] { "editbox", "numericdisplay" })
        {
            var symbol = new ScreenSymbol { Type = type, Fill = "#abcdef", VariablePath = "Demo.Setpoint", EditBoxStep = 0.5, EditBoxFormat = "F3", NumericDisplayUnit = "bar", FontSize = 32 };
            DisplayVisuals.ApplyPreset(symbol, preset);
            var restored = JsonSerializer.Deserialize<ScreenSymbol>(JsonSerializer.Serialize(symbol))!;
            Assert.True(DisplayVisuals.IsEnhanced(restored));
            Assert.Equal(preset, restored.DisplayBackgroundStyle);
            Assert.Equal(color, DisplayVisuals.Foreground(restored));
            Assert.Equal("Demo.Setpoint", restored.VariablePath);
            Assert.Equal(0.5, restored.EditBoxStep);
            Assert.Equal("F3", restored.EditBoxFormat);
            Assert.Equal("bar", restored.NumericDisplayUnit);
            Assert.Equal(32, restored.FontSize);
            if (type == "editbox") Assert.Equal("#abcdef", restored.Fill);
            DisplayVisuals.ApplyPreset(restored, "classic");
            Assert.False(DisplayVisuals.IsEnhanced(restored));
        }
    }

    [Fact]
    public void SevenSegment_RendersCorrectDigitMasksAndDecimalWithoutExternalFont()
    {
        var symbol = new ScreenSymbol { Type = "numericdisplay", DisplayFontStyle = "seven-segment", DisplayAutoFit = true };
        var root = XElement.Parse(DisplayVisuals.Render(symbol, "0123456789.-+"));
        var svg = Assert.Single(root.Descendants(Svg + "svg"));
        Assert.Equal("0123456789.-+", svg.Attribute("aria-label")!.Value);
        var expectedCounts = new[] { 6, 2, 5, 5, 4, 5, 6, 3, 7, 6 };
        var groups = svg.Elements(Svg + "g").ToList();
        for (var i = 0; i < 10; i++)
            Assert.Equal(expectedCounts[i], groups[i].Elements(Svg + "polygon").Count(p => (string?)p.Attribute("opacity") == "1"));
        Assert.Single(groups[10].Elements(Svg + "circle"));
        Assert.Single(groups[11].Elements(Svg + "polygon"), p => (string?)p.Attribute("opacity") == "1");
        Assert.Single(groups[12].Elements(Svg + "rect"));
        Assert.Contains("width:100%;height:100%", svg.Attribute("style")!.Value);
    }

    [Fact]
    public void NonNumericText_FallsBackToEscapedTerminalReadout()
    {
        var symbol = new ScreenSymbol { DisplayFontStyle = "seven-segment", DisplayAutoFit = true };
        var root = XElement.Parse(DisplayVisuals.Render(symbol, "<script>alert('&')</script>"));
        Assert.Empty(root.Descendants(Svg + "svg"));
        var text = Assert.Single(root.Descendants(Html + "span"));
        Assert.Equal("<script>alert('&')</script>", text.Value);
        Assert.Contains("monospace", text.Attribute("style")!.Value);
        Assert.Empty(root.Descendants(Html + "script"));
    }

    [Fact]
    public void AutoFit_UsesContainerDimensionsAndRecalculatesForValueLength()
    {
        var symbol = new ScreenSymbol { DisplayAutoFit = true, DisplayFontStyle = "terminal" };
        var shortCss = DisplayVisuals.FontCss(symbol, "12");
        var longCss = DisplayVisuals.FontCss(symbol, "-123456.789");
        Assert.Contains("80cqh", shortCss);
        Assert.Contains("100cqw", shortCss);
        Assert.Contains("/ 1.5", shortCss);
        Assert.Contains("/ 8.25", longCss);
        Assert.DoesNotContain("vw", longCss);
        symbol.DisplayAutoFit = false;
        symbol.FontSize = 31;
        Assert.Contains("font-size:31px", DisplayVisuals.FontCss(symbol, "-123456.789"));
        var fixedSvg = XElement.Parse(DisplayVisuals.Readout(new ScreenSymbol { DisplayFontStyle = "seven-segment", FontSize = 31 }, "123"));
        Assert.Contains("height:31px", Assert.Single(fixedSvg.Descendants(Svg + "svg")).Attribute("style")!.Value);
    }

    [Fact]
    public void FormattingAndUnits_PreserveSignsMissingValuesAndText()
    {
        var symbol = new ScreenSymbol { Type = "numericdisplay", NumericDisplayFormat = "F3", NumericDisplayUnit = "bar & psi", DisplayAutoFit = true };
        Assert.Equal("-12.500", DisplayVisuals.FormatValue(symbol, "-12.5"));
        Assert.Equal("—", DisplayVisuals.FormatValue(symbol, null));
        Assert.Equal("offline", DisplayVisuals.FormatValue(symbol, "offline"));
        var root = XElement.Parse(DisplayVisuals.Render(symbol, "-12.500", "#ef4444"));
        Assert.Contains(root.Descendants(Html + "div"), e => (string?)e.Attribute("class") == "display-unit" && e.Value == "bar & psi");
        Assert.Contains(root.Descendants().Attributes("style"), a => a.Value.Contains("color:#ef4444"));
    }
}
