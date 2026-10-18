using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using SharedModels;
using Xunit;

namespace Tests.SharedModels;

public class GaugeVisualsTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    [Fact]
    public void LegacyJson_KeepsOriginalRendering()
    {
        var symbol = JsonSerializer.Deserialize<ScreenSymbol>("{\"Type\":\"gauge\",\"GaugeStyle\":\"arc\"}")!;
        Assert.False(GaugeVisuals.IsEnhanced(symbol));
        Assert.Equal("solid", symbol.GaugeFillStyle);
        Assert.Equal("none", symbol.GaugeBackground);
        Assert.Equal(18, symbol.GaugeArcThickness);
    }

    [Theory]
    [InlineData("slate", "#3b82f6", "#22d3ee", "#334155", "#e2e8f0")]
    [InlineData("ivory", "#d97706", "#facc15", "#ddd5c5", "#44372a")]
    [InlineData("metallic", "#0369a1", "#38bdf8", "#94a3b8", "#0f172a")]
    public void Presets_SetCoordinatedColorsAndPreserveProcessSettings(string preset, string fill, string end, string track, string text)
    {
        var symbol = new ScreenSymbol { Type = "gauge", GaugeStyle = "semicircle", VariablePath = "Plant.Temperature", MinValue = -50, MaxValue = 150, GaugeUnit = "C" };
        GaugeVisuals.ApplyPreset(symbol, preset);
        var restored = JsonSerializer.Deserialize<ScreenSymbol>(JsonSerializer.Serialize(symbol))!;
        Assert.True(GaugeVisuals.IsEnhanced(restored));
        Assert.Equal(preset, restored.GaugeBackground);
        Assert.Equal("gradient", restored.GaugeFillStyle);
        Assert.Equal(fill, restored.Fill);
        Assert.Equal(end, restored.GaugeGradientEndColor);
        Assert.Equal(track, restored.GaugeTrackColor);
        Assert.Equal(text, restored.GaugeNeedleColor);
        Assert.Equal("semicircle", restored.GaugeStyle);
        Assert.Equal("Plant.Temperature", restored.VariablePath);
        Assert.Equal(-50, restored.MinValue);
        Assert.Equal(150, restored.MaxValue);
        Assert.Equal("C", restored.GaugeUnit);
        GaugeVisuals.ApplyPreset(restored, "classic");
        Assert.False(GaugeVisuals.IsEnhanced(restored));
        Assert.Equal("semicircle", restored.GaugeStyle);
    }

    [Theory]
    [InlineData("needle")]
    [InlineData("arc")]
    [InlineData("semicircle")]
    [InlineData("hbar")]
    [InlineData("vbar")]
    [InlineData("thermometer")]
    public void EveryShape_UsesGradientAndBackgroundAndEscapesReadout(string style)
    {
        foreach (var preset in new[] { "slate", "ivory", "metallic" })
        {
            var symbol = new ScreenSymbol { GaugeStyle = style, Width = 200, Height = 200 };
            GaugeVisuals.ApplyPreset(symbol, preset);
            var root = XElement.Parse(GaugeVisuals.Render(symbol, "#ef4444", 0.5, "50 < C & >", 0, 100));
            var gradient = Assert.Single(root.Descendants(Svg + "linearGradient").Where(e => ((string?)e.Attribute("id"))!.EndsWith("_fill")));
            Assert.Equal(new[] { "#ef4444", symbol.GaugeGradientEndColor }, gradient.Elements().Select(e => (string?)e.Attribute("stop-color")));
            Assert.Contains(root.Descendants(), e => (string?)e.Attribute("fill") == $"url(#{gradient.Attribute("id")!.Value})" || (string?)e.Attribute("stroke") == $"url(#{gradient.Attribute("id")!.Value})");
            Assert.Contains(root.Descendants(Svg + "text"), e => e.Value == "50 < C & >");
            Assert.Contains(root.Descendants(), e => ((string?)e.Attribute("fill"))?.EndsWith("_face)") == true);
            symbol.GaugeShowValue = false;
            symbol.GaugeShowTickLabels = false;
            Assert.Empty(XElement.Parse(GaugeVisuals.Render(symbol, symbol.Fill, 0.5, "50", 0, 100)).Descendants(Svg + "text"));
        }
    }

    [Theory]
    [InlineData(-1, null)]
    [InlineData(0, null)]
    [InlineData(0.25, "25 100")]
    [InlineData(1, "100 100")]
    [InlineData(2, "100 100")]
    public void Arc_ClampsProgressAndHasNoRoundCapDotAtZero(double progress, string? expectedDash)
    {
        var symbol = new ScreenSymbol { GaugeStyle = "arc", Width = 200, Height = 200, GaugeFillStyle = "gradient" };
        var root = XElement.Parse(GaugeVisuals.Render(symbol, symbol.Fill, progress, "0", 0, 100));
        var progressPaths = root.Descendants(Svg + "path").Where(e => e.Attribute("pathLength") != null).ToList();
        if (expectedDash == null) Assert.Empty(progressPaths);
        else Assert.Equal(expectedDash, Assert.Single(progressPaths).Attribute("stroke-dasharray")!.Value);
    }

    [Theory]
    [InlineData(1, 4.44)]
    [InlineData(18, 13.32)]
    [InlineData(100, 22.2)]
    public void ArcThickness_ClampsToSixThroughThirtyPercent(double thickness, double expectedWidth)
    {
        var symbol = new ScreenSymbol { GaugeStyle = "arc", Width = 200, Height = 200, GaugeArcThickness = thickness };
        var root = XElement.Parse(GaugeVisuals.Render(symbol, symbol.Fill, 1, "100", 0, 100));
        Assert.All(root.Descendants(Svg + "path"), path => Assert.Equal(expectedWidth, double.Parse(path.Attribute("stroke-width")!.Value, CultureInfo.InvariantCulture), 3));
    }

    [Theory]
    [InlineData("needle")]
    [InlineData("arc")]
    [InlineData("semicircle")]
    [InlineData("hbar")]
    [InlineData("vbar")]
    [InlineData("thermometer")]
    public void Readout_IsInsideBoundsAndSeparatedFromScaleLabels(string style)
    {
        var symbol = new ScreenSymbol { GaugeStyle = style, Width = 200, Height = 200 };
        GaugeVisuals.ApplyPreset(symbol, "slate");
        var root = XElement.Parse(GaugeVisuals.Render(symbol, symbol.Fill, 0.65, "65.0 bar", 0, 100));
        var readout = Assert.Single(root.Descendants(Svg + "text").Where(t => t.Value == "65.0 bar"));
        var y = double.Parse(readout.Attribute("y")!.Value, CultureInfo.InvariantCulture);
        var font = double.Parse(readout.Attribute("font-size")!.Value, CultureInfo.InvariantCulture);
        Assert.InRange(y - font / 2, 0, 200);
        Assert.InRange(y + font / 2, 0, 200);
        if (style is "needle" or "thermometer")
        {
            foreach (var tick in root.Descendants(Svg + "text").Where(t => t != readout))
            {
                var tickY = double.Parse(tick.Attribute("y")!.Value, CultureInfo.InvariantCulture);
                var tickFont = double.Parse(tick.Attribute("font-size")!.Value, CultureInfo.InvariantCulture);
                Assert.True(Math.Abs(y - tickY) > (font + tickFont) / 2, $"{style}: readout overlaps scale label {tick.Value} vertically.");
            }
        }
    }

    [Fact]
    public void PaintServers_AreUniqueAcrossRepeatedInstancesAndCultureIndependent()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("it-IT");
            var symbol = new ScreenSymbol { Id = "same", GaugeStyle = "arc", Width = 201.5, Height = 200 };
            GaugeVisuals.ApplyPreset(symbol, "slate");
            var first = XElement.Parse(GaugeVisuals.Render(symbol, symbol.Fill, 0.125, "12,5", 0, 100));
            var second = XElement.Parse(GaugeVisuals.Render(symbol, symbol.Fill, 0.125, "12,5", 0, 100));
            Assert.Equal("0 0 201.5 200", first.Attribute("viewBox")!.Value);
            Assert.Equal("12.5 100", Assert.Single(first.Descendants(Svg + "path").Where(e => e.Attribute("pathLength") != null)).Attribute("stroke-dasharray")!.Value);
            Assert.Empty(first.Descendants().Attributes("id").Select(a => a.Value).Intersect(second.Descendants().Attributes("id").Select(a => a.Value)));
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }
}
