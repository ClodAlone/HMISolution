using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using SharedModels;
using Xunit;

namespace Tests.SharedModels;

public class ConicalTankVisualsTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static XElement Element(XElement root, string name) => Assert.Single(root.Descendants(), e => (string?)e.Attribute("class") == name);
    private static double Number(XElement element, string name) => double.Parse(element.Attribute(name)!.Value, CultureInfo.InvariantCulture);
    private static string N(double number) => number.ToString("0.######", CultureInfo.InvariantCulture);
    private static ScreenSymbol Symbol() => new() { TankStyle = "isometric-cone", Width = 200, Height = 240, TankUseGradient = true, TankFillStyle = "gradient" };

    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(99)]
    [InlineData(100)]
    public void FillSurface_NarrowsInsideConeAndStaysFullWidthInsideCylinder(double percent)
    {
        var symbol = Symbol();
        var root = XElement.Parse(TankVisuals.Render(symbol, N(percent)));
        Assert.Equal(N(percent / 100), root.Attribute("data-level")!.Value);
        Assert.Equal("0 0 200 240", root.Attribute("viewBox")!.Value);
        var rim = Element(root, "tank-rim");
        var seam = Element(root, "tank-cone-seam");
        Assert.Equal(100, Number(rim, "cx"));
        Assert.Equal(76, Number(rim, "rx"));
        Assert.Equal(18, Number(rim, "ry"));
        Assert.Equal(32.4, Number(rim, "cy"), 6);
        Assert.Equal(163.2, Number(seam, "cy"), 6);
        Assert.Equal(76, Number(seam, "rx"));
        Assert.Contains("L 100 225.6", Element(root, "tank-body").Attribute("d")!.Value);
        Assert.Equal("#000000", Element(root, "tank-cone").Attribute("fill")!.Value);
        if (percent == 0)
        {
            Assert.Empty(root.Descendants().Where(e => (string?)e.Attribute("class") == "tank-liquid"));
            return;
        }
        var surface = Element(root, "tank-surface");
        var y = 225.6 - 193.2 * percent / 100;
        var scale = Math.Min(1, (225.6 - y) / 62.4);
        Assert.Equal(y, Number(surface, "cy"), 6);
        Assert.Equal(76 * scale, Number(surface, "rx"), 6);
        Assert.Equal(18 * scale, Number(surface, "ry"), 6);
        Assert.StartsWith($"M {N(100 - 76 * scale)} {N(y)} A {N(76 * scale)} {N(18 * scale)}", Element(root, "tank-liquid-front").Attribute("d")!.Value);
        Assert.Contains("L 100 225.6", Element(root, "tank-liquid-front").Attribute("d")!.Value);
        Assert.Equal("#22d3ee", surface.Attribute("fill")!.Value);
        if (percent == 100)
        {
            Assert.Equal(Number(rim, "cy"), Number(surface, "cy"));
            Assert.Equal(Element(root, "tank-body").Attribute("d")!.Value, Element(root, "tank-liquid-front").Attribute("d")!.Value);
        }
    }

    [Fact]
    public void ConeOutline_IsTangentToProjectedShoulderInsteadOfCuttingItsEllipse()
    {
        var root = XElement.Parse(TankVisuals.Render(Symbol(), "50"));
        var q = 18 / 62.4;
        var tangentX = 76 * Math.Sqrt(1 - q * q);
        var tangentY = 163.2 + 18 * q;
        var shape = Element(root, "tank-outline").Attribute("d")!.Value;
        Assert.Contains($"A 76 18 0 0 1 {N(100 + tangentX)} {N(tangentY)} L 100 225.6 L {N(100 - tangentX)} {N(tangentY)}", shape);
        Assert.Equal(shape, Element(root, "tank-body").Attribute("d")!.Value);
        Assert.Equal(shape, Element(root, "tank-sheen").Attribute("d")!.Value);
    }

    [Fact]
    public void LiquidSurface_IsContinuousAcrossCylinderConeJoin()
    {
        var symbol = Symbol();
        var join = 62.4 / 193.2;
        var below = XElement.Parse(TankVisuals.Render(symbol, previewLevel: join - 0.00001));
        var at = XElement.Parse(TankVisuals.Render(symbol, previewLevel: join));
        var above = XElement.Parse(TankVisuals.Render(symbol, previewLevel: join + 0.00001));
        Assert.Equal(76, Number(Element(at, "tank-surface"), "rx"), 6);
        Assert.Equal(163.2, Number(Element(at, "tank-surface"), "cy"), 6);
        Assert.InRange(Number(Element(below, "tank-surface"), "rx"), 75.99, 76);
        Assert.Equal(76, Number(Element(above, "tank-surface"), "rx"));
        Assert.True(Number(Element(below, "tank-surface"), "cy") > Number(Element(above, "tank-surface"), "cy"));
    }

    [Theory]
    [InlineData(-100, "0%", false)]
    [InlineData(200, "100%", true)]
    public void OutOfRangeFill_IsClamped(double value, string label, bool liquid)
    {
        var root = XElement.Parse(TankVisuals.Render(Symbol(), N(value)));
        Assert.Equal(label, Element(root, "tank-level").Value);
        Assert.Equal(liquid, root.Descendants().Any(e => (string?)e.Attribute("class") == "tank-liquid"));
    }

    [Fact]
    public void ConicalStyle_RoundTripsAndScalesProportionallyWithoutChangingBinding()
    {
        var symbol = Symbol();
        symbol.VariablePath = "Transfer.SourceLevel";
        var restored = JsonSerializer.Deserialize<ScreenSymbol>(JsonSerializer.Serialize(symbol))!;
        Assert.Equal("isometric-cone", restored.TankStyle);
        Assert.Equal("Transfer.SourceLevel", restored.VariablePath);
        var first = XElement.Parse(TankVisuals.Render(symbol, "10"));
        symbol.Width *= 3;
        symbol.Height *= 3;
        var resized = XElement.Parse(TankVisuals.Render(symbol, "10", responsive: true));
        Assert.Equal("100%", resized.Attribute("width")!.Value);
        Assert.Equal("xMidYMid meet", resized.Attribute("preserveAspectRatio")!.Value);
        foreach (var name in new[] { "tank-rim", "tank-cone-seam", "tank-surface" })
            foreach (var attr in new[] { "cx", "cy", "rx", "ry" })
                Assert.Equal(Number(Element(first, name), attr) * 3, Number(Element(resized, name), attr), 5);
    }

    [Fact]
    public void TinyDimensionsAndUnavailableValue_StayFiniteAndDoNotInventLiquid()
    {
        var symbol = Symbol();
        symbol.Width = double.NaN;
        symbol.Height = -1;
        var empty = XElement.Parse(TankVisuals.Render(symbol));
        Assert.Equal("unavailable", empty.Attribute("data-state")!.Value);
        Assert.Equal("-", Element(empty, "tank-level").Value);
        Assert.DoesNotContain("NaN", empty.ToString());
        Assert.DoesNotContain("Infinity", empty.ToString());
        Assert.Empty(empty.Descendants().Where(e => (string?)e.Attribute("class") == "tank-liquid"));
        var full = XElement.Parse(TankVisuals.Render(symbol, "100"));
        Assert.DoesNotContain("NaN", full.ToString());
        Assert.Equal("0 0 1 1", full.Attribute("viewBox")!.Value);
    }
}
