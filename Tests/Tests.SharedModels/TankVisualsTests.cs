using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using SharedModels;
using Xunit;

namespace Tests.SharedModels;

public class TankVisualsTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static XElement Element(XElement root, string name) => Assert.Single(root.Descendants(), e => (string?)e.Attribute("class") == name);
    private static double Number(XElement element, string name) => double.Parse(element.Attribute(name)!.Value, CultureInfo.InvariantCulture);
    private static XElement Render(ScreenSymbol symbol, string? value = "50") => XElement.Parse(TankVisuals.Render(symbol, value));

    [Theory]
    [InlineData("rect")]
    [InlineData("rectangular")]
    [InlineData("round")]
    [InlineData("cylinder")]
    [InlineData("isometric")]
    public void EveryStyle_FillsFromBottomWithRealLevelAndClampsEndpoints(string style)
    {
        var symbol = new ScreenSymbol { Width = 200, Height = 240, TankStyle = style, MinValue = 10, MaxValue = 30 };
        var half = Render(symbol, "20");
        var full = Render(symbol, "30");
        var above = Render(symbol, "900");
        Assert.Equal("ready", half.Attribute("data-state")!.Value);
        Assert.Equal("0.5", half.Attribute("data-level")!.Value);
        Assert.Equal("50%", Element(half, "tank-level").Value);
        var halfFill = Element(half, "tank-liquid-front");
        var fullFill = Element(full, "tank-liquid-front");
        Assert.Equal(Number(fullFill, "height") * 0.5, Number(halfFill, "height"), 6);
        Assert.Equal(Number(fullFill, "y") + Number(fullFill, "height"), Number(halfFill, "y") + Number(halfFill, "height"), 6);
        Assert.Equal(fullFill.ToString(), Element(above, "tank-liquid-front").ToString());
        foreach (var value in new[] { "10", "-900" })
        {
            var empty = Render(symbol, value);
            Assert.Equal("0%", Element(empty, "tank-level").Value);
            Assert.Empty(empty.Descendants().Where(e => (string?)e.Attribute("class") == "tank-liquid"));
        }
    }

    [Fact]
    public void IsometricLiquid_FrontSideAndSurfaceMeetAtOneHeight()
    {
        var root = Render(new ScreenSymbol { TankStyle = "isometric", Width = 200, Height = 240 }, "25");
        var front = Element(root, "tank-liquid-front");
        var side = Points(Element(root, "tank-liquid-side"));
        var surface = Points(Element(root, "tank-surface"));
        Assert.Equal((16d, Number(front, "y")), surface[0]);
        Assert.Equal((144d, Number(front, "y")), surface[1]);
        Assert.Equal(surface[1], side[0]);
        Assert.Equal(surface[2], side[1]);
        Assert.Equal(40, surface[2].X - surface[1].X, 6);
        Assert.Equal(28.8, surface[1].Y - surface[2].Y, 6);
        Assert.Equal(Number(front, "y") + Number(front, "height"), side[3].Y, 6);
    }

    [Fact]
    public void Cylinder_HasEllipticalRimAndLiquidSurfaceAtFillHeight()
    {
        var root = Render(new ScreenSymbol { TankStyle = "cylinder", Width = 200, Height = 240 }, "75");
        var fill = Element(root, "tank-liquid-front");
        var surface = Element(root, "tank-surface");
        var rim = Element(root, "tank-rim");
        Assert.Equal(Svg + "ellipse", surface.Name);
        Assert.Equal(100, Number(surface, "cx"));
        Assert.Equal(76, Number(surface, "rx"));
        Assert.Equal(18, Number(surface, "ry"));
        Assert.Equal(Number(fill, "y"), Number(surface, "cy"), 6);
        Assert.Equal(Number(rim, "rx"), Number(surface, "rx"));
        Assert.Contains(" A 76 18 ", Element(root, "tank-body").Attribute("d")!.Value);
    }

    [Fact]
    public void RoundedAndLegacyRectangularShapes_AreDistinctAndClipTheirLiquid()
    {
        var symbol = new ScreenSymbol { Width = 200, Height = 240, TankStyle = "rectangular" };
        var legacy = Render(symbol);
        Assert.Equal(4, Number(Element(legacy, "tank-body"), "rx"));
        Assert.Equal(2, Number(Element(legacy, "tank-clip"), "rx"));
        var clipId = Assert.Single(legacy.Descendants(Svg + "clipPath")).Attribute("id")!.Value;
        Assert.Equal($"url(#{clipId})", Element(legacy, "tank-liquid").Attribute("clip-path")!.Value);
        symbol.TankStyle = "round";
        var rounded = Render(symbol);
        Assert.Equal(40, Number(Element(rounded, "tank-body"), "rx"));
        Assert.Equal(38, Number(Element(rounded, "tank-clip"), "rx"));
    }

    [Theory]
    [InlineData("rect")]
    [InlineData("round")]
    [InlineData("cylinder")]
    [InlineData("isometric")]
    [InlineData("isometric-cone")]
    public void ShadingAndLiquidGradient_AreIndependentAndUseConfiguredColors(string style)
    {
        var symbol = new ScreenSymbol { TankStyle = style, TankBodyColor = "#475569", TankLiquidColor = "#0369a1", TankGradientEndColor = "#67e8f9" };
        var flat = Render(symbol);
        Assert.Empty(flat.Descendants(Svg + "linearGradient"));
        Assert.Equal("#475569", Element(flat, "tank-body").Attribute("fill")!.Value);
        Assert.Equal("#0369a1", Element(flat, "tank-liquid-front").Attribute("fill")!.Value);
        symbol.TankFillStyle = "gradient";
        var fillOnly = Render(symbol);
        var gradient = Assert.Single(fillOnly.Descendants(Svg + "linearGradient"));
        Assert.Equal(new[] { "#0369a1", "#67e8f9" }, gradient.Elements(Svg + "stop").Select(s => s.Attribute("stop-color")!.Value).ToArray());
        Assert.Equal($"url(#{gradient.Attribute("id")!.Value})", Element(fillOnly, "tank-liquid-front").Attribute("fill")!.Value);
        if (style is "cylinder" or "isometric") Assert.Equal("#67e8f9", Element(fillOnly, "tank-surface").Attribute("fill")!.Value);
        symbol.TankUseGradient = true;
        var shaded = Render(symbol);
        Assert.Equal(2, shaded.Descendants(Svg + "linearGradient").Count());
        Assert.StartsWith("url(#tank_", Element(shaded, "tank-sheen").Attribute("fill")!.Value);
        symbol.TankFillStyle = "solid";
        var shellOnly = Render(symbol);
        Assert.Single(shellOnly.Descendants(Svg + "linearGradient"));
        Assert.Equal("#0369a1", Element(shellOnly, "tank-liquid-front").Attribute("fill")!.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not a number")]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    public void UnavailableValue_IsNotReportedAsMeasuredZero(string? value)
    {
        var root = Render(new ScreenSymbol(), value);
        Assert.Equal("unavailable", root.Attribute("data-state")!.Value);
        Assert.Equal("-", Element(root, "tank-level").Value);
        Assert.Contains("unavailable", root.Element(Svg + "title")!.Value);
        Assert.Empty(root.Descendants().Where(e => (string?)e.Attribute("class") == "tank-liquid"));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(100, 0)]
    [InlineData(double.NaN, 100)]
    [InlineData(0, double.PositiveInfinity)]
    public void InvalidRange_DisplaysExplicitErrorEvenWhenReadoutDisabled(double min, double max)
    {
        var root = Render(new ScreenSymbol { MinValue = min, MaxValue = max, TankShowLevel = false });
        Assert.Equal("invalid-range", root.Attribute("data-state")!.Value);
        Assert.Equal("Invalid range", Element(root, "tank-level").Value);
        Assert.Empty(root.Descendants().Where(e => (string?)e.Attribute("class") == "tank-liquid"));
    }

    [Fact]
    public void SavedTankRange_IsUsedUnlessCommonRangeOverridesItIncludingZeroMaximum()
    {
        var symbol = new ScreenSymbol { TankMin = 100, TankMax = 200 };
        Assert.Equal("50%", Element(Render(symbol, "150"), "tank-level").Value);
        symbol.MinValue = -20;
        symbol.MaxValue = 0;
        Assert.Equal("50%", Element(Render(symbol, "-10"), "tank-level").Value);
        symbol.TankShowLevel = false;
        var hidden = Render(symbol, "-10");
        Assert.Empty(hidden.Descendants(Svg + "text"));
        Assert.Equal("0.5", hidden.Attribute("data-level")!.Value);
        Assert.Equal(1, hidden.Descendants().Count(e => (string?)e.Attribute("class") == "tank-liquid"));
    }

    [Theory]
    [InlineData("rect")]
    [InlineData("round")]
    [InlineData("cylinder")]
    [InlineData("isometric")]
    public void Resize_ScalesAllLiquidGeometry(string style)
    {
        var symbol = new ScreenSymbol { TankStyle = style, Width = 120, Height = 160 };
        var first = Render(symbol);
        symbol.Width *= 3;
        symbol.Height *= 3;
        var resized = Render(symbol);
        var firstFill = Element(first, "tank-liquid-front");
        var resizedFill = Element(resized, "tank-liquid-front");
        if (style is "rect" or "round")
        {
            Assert.Equal((480 - 8) * 0.5, Number(resizedFill, "height"));
            Assert.Equal((360 - 8), Number(resizedFill, "width"));
        }
        else
        {
            foreach (var attribute in new[] { "x", "y", "width", "height" })
                Assert.Equal(Number(firstFill, attribute) * 3, Number(resizedFill, attribute), 6);
        }
    }

    [Fact]
    public void Preview_IsExplicitAndDoesNotChangeModelAndDefaultsRoundTrip()
    {
        var symbol = new ScreenSymbol { TankStyle = "isometric", TankUseGradient = true, TankFillStyle = "gradient" };
        var json = JsonSerializer.Serialize(symbol);
        var preview = XElement.Parse(TankVisuals.Render(symbol, responsive: true, previewLevel: 0.65));
        Assert.Equal("65%", Element(preview, "tank-level").Value);
        Assert.Contains("(preview)", preview.Attribute("aria-label")!.Value);
        Assert.Equal("100%", preview.Attribute("width")!.Value);
        Assert.Equal("100%", preview.Attribute("height")!.Value);
        Assert.Equal("xMidYMid meet", preview.Attribute("preserveAspectRatio")!.Value);
        Assert.Equal(json, JsonSerializer.Serialize(symbol));
        var restored = JsonSerializer.Deserialize<ScreenSymbol>(json)!;
        Assert.True(restored.TankUseGradient);
        Assert.Equal("gradient", restored.TankFillStyle);
        Assert.Equal("isometric", restored.TankStyle);
        Assert.Equal(symbol.TankGradientEndColor, restored.TankGradientEndColor);
        Assert.False(new ScreenSymbol().TankUseGradient);
        Assert.Equal("solid", new ScreenSymbol().TankFillStyle);
    }

    [Fact]
    public void SeparateInstances_HaveUniquePaintServersAndStableMarkup()
    {
        var first = new ScreenSymbol { Id = "reused", TankUseGradient = true };
        var second = new ScreenSymbol { Id = "reused", TankUseGradient = true };
        var a = Render(first);
        var b = Render(second);
        Assert.NotEqual(Assert.Single(a.Descendants(Svg + "linearGradient")).Attribute("id")!.Value, Assert.Single(b.Descendants(Svg + "linearGradient")).Attribute("id")!.Value);
        Assert.Equal(TankVisuals.Render(first, "50"), TankVisuals.Render(first, "50.0"));
    }

    [Theory]
    [InlineData("rect")]
    [InlineData("round")]
    [InlineData("cylinder")]
    [InlineData("isometric")]
    public void TinyDimensions_ProduceFiniteNonnegativeGeometry(string style)
    {
        var symbol = new ScreenSymbol { Width = -5, Height = double.NaN, TankStyle = style };
        var root = Render(symbol);
        Assert.Equal("0 0 1 1", root.Attribute("viewBox")!.Value);
        Assert.DoesNotContain("NaN", root.ToString());
        foreach (var attribute in root.Descendants().Attributes().Where(a => a.Name.LocalName is "width" or "height" or "rx" or "ry"))
            Assert.True(double.Parse(attribute.Value, CultureInfo.InvariantCulture) >= 0, attribute.ToString());
    }

    private static (double X, double Y)[] Points(XElement element) => element.Attribute("points")!.Value.Split(' ').Select(p =>
    {
        var pair = p.Split(',');
        return (double.Parse(pair[0], CultureInfo.InvariantCulture), double.Parse(pair[1], CultureInfo.InvariantCulture));
    }).ToArray();
}
