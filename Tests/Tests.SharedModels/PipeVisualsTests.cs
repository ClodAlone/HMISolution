using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using SharedModels;
using Xunit;

namespace Tests.SharedModels;

public class PipeVisualsTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static XElement Path(XElement root, string name) => Assert.Single(root.Elements(Svg + "path"), e => (string?)e.Attribute("class") == name);
    private static double Number(XElement element, string name) => double.Parse(element.Attribute(name)!.Value, CultureInfo.InvariantCulture);

    [Theory]
    [InlineData("horizontal", 300, 40, "M 0 20 H 300", 24)]
    [InlineData("vertical", 40, 300, "M 20 0 V 300", 24)]
    [InlineData("corner-tl", 120, 100, "M 120 15 H 45 Q 15 15 15 45 V 100", 30)]
    [InlineData("corner-tr", 120, 100, "M 0 15 H 75 Q 105 15 105 45 V 100", 30)]
    [InlineData("corner-bl", 120, 100, "M 15 0 V 55 Q 15 85 45 85 H 120", 30)]
    [InlineData("corner-br", 120, 100, "M 105 0 V 55 Q 105 85 75 85 H 0", 30)]
    public void EveryOrientation_FlowFollowsEntireBodyPath(string orientation, double width, double height, string expected, double thickness)
    {
        var symbol = new ScreenSymbol { Width = width, Height = height, PipeOrientation = orientation };
        var root = XElement.Parse(PipeVisuals.Render(symbol));
        var body = Path(root, "pipe-body");
        var flow = Path(root, "hmi-pipe-flow");
        Assert.Equal(expected, body.Attribute("d")!.Value);
        Assert.Equal(expected, flow.Attribute("d")!.Value);
        Assert.Equal(thickness, Number(body, "stroke-width"));
        Assert.Equal(thickness * 0.18, Number(flow, "stroke-width"), 6);
        Assert.Equal($"{(thickness * 0.85).ToString("0.######", CultureInfo.InvariantCulture)} {(thickness * 0.6).ToString("0.######", CultureInfo.InvariantCulture)}", flow.Attribute("stroke-dasharray")!.Value);
        Assert.Empty(root.Descendants(Svg + "line"));
    }

    [Theory]
    [InlineData("horizontal", 200, 40)]
    [InlineData("vertical", 40, 200)]
    [InlineData("corner-tl", 120, 100)]
    [InlineData("corner-tr", 120, 100)]
    [InlineData("corner-bl", 120, 100)]
    [InlineData("corner-br", 120, 100)]
    public void Resize_ScalesStripeWidthSpacingAndSeamlessAnimationDistance(string orientation, double width, double height)
    {
        var symbol = new ScreenSymbol { Width = width, Height = height, PipeOrientation = orientation };
        var first = Path(XElement.Parse(PipeVisuals.Render(symbol)), "hmi-pipe-flow");
        symbol.Width *= 3;
        symbol.Height *= 3;
        var resized = Path(XElement.Parse(PipeVisuals.Render(symbol)), "hmi-pipe-flow");
        Assert.Equal(Number(first, "stroke-width") * 3, Number(resized, "stroke-width"), 6);
        var originalDashes = first.Attribute("stroke-dasharray")!.Value.Split(' ').Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
        var resizedDashes = resized.Attribute("stroke-dasharray")!.Value.Split(' ').Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
        Assert.Equal(originalDashes[0] * 3, resizedDashes[0], 6);
        Assert.Equal(originalDashes[1] * 3, resizedDashes[1], 6);
        Assert.Contains($"--pipe-cycle:{resizedDashes.Sum().ToString("0.######", CultureInfo.InvariantCulture)};", resized.Attribute("style")!.Value);
        Assert.Contains("hmiPipeFlow 1s linear infinite", resized.Attribute("style")!.Value);
    }

    [Fact]
    public void ExtendingStraightPipe_KeepsDiameterAndSpacingButExtendsFlowToNewEnd()
    {
        var symbol = new ScreenSymbol { Width = 200, Height = 40 };
        var first = Path(XElement.Parse(PipeVisuals.Render(symbol)), "hmi-pipe-flow");
        symbol.Width = 1500;
        var extended = Path(XElement.Parse(PipeVisuals.Render(symbol)), "hmi-pipe-flow");
        Assert.Equal(first.Attribute("stroke-dasharray")!.Value, extended.Attribute("stroke-dasharray")!.Value);
        Assert.Equal("M 0 20 H 1500", extended.Attribute("d")!.Value);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("False", false)]
    [InlineData(" FALSE ", false)]
    [InlineData("true", true)]
    [InlineData("0", false)]
    [InlineData("0.000", false)]
    [InlineData("-0", false)]
    [InlineData("0e3", false)]
    [InlineData("-5.5", true)]
    [InlineData("12.5", true)]
    public void BoundValue_ControlsFlowVisibility(string? value, bool active)
    {
        var symbol = new ScreenSymbol();
        Assert.Equal(active, PipeVisuals.IsFlowActive(symbol, value));
        var root = XElement.Parse(PipeVisuals.Render(symbol, value));
        Assert.Equal(active ? 1 : 0, root.Elements(Svg + "path").Count(e => (string?)e.Attribute("class") == "hmi-pipe-flow"));
        Assert.Single(root.Elements(Svg + "path"), e => (string?)e.Attribute("class") == "pipe-body");
    }

    [Theory]
    [InlineData("left-right", "34.8")]
    [InlineData("right-left", "-34.8")]
    [InlineData("top-bottom", "34.8")]
    [InlineData("bottom-top", "-34.8")]
    public void DirectionAndDuration_ControlAnimation(string direction, string distance)
    {
        var symbol = new ScreenSymbol { Width = 200, Height = 40, PipeFlowDirection = direction, PipeFlowSpeed = 0.1 };
        var root = XElement.Parse(PipeVisuals.Render(symbol));
        var style = Path(root, "hmi-pipe-flow").Attribute("style")!.Value;
        Assert.Contains($"--pipe-cycle:{distance};", style);
        Assert.Contains("hmiPipeFlow 0.1s linear infinite", style);
        Assert.Contains("prefers-reduced-motion:reduce", root.ToString());
    }

    [Fact]
    public void ShowFlowAndAnimation_AreIndependentAndPreviewCanBeStatic()
    {
        var symbol = new ScreenSymbol { PipeAnimate = false };
        Assert.Null(Path(XElement.Parse(PipeVisuals.Render(symbol)), "hmi-pipe-flow").Attribute("style"));
        symbol.PipeAnimate = true;
        Assert.Null(Path(XElement.Parse(PipeVisuals.Render(symbol, animate: false)), "hmi-pipe-flow").Attribute("style"));
        symbol.PipeShowFlow = false;
        Assert.Single(XElement.Parse(PipeVisuals.Render(symbol)).Elements(Svg + "path"));
    }

    [Theory]
    [InlineData("horizontal")]
    [InlineData("vertical")]
    [InlineData("corner-tl")]
    [InlineData("corner-tr")]
    [InlineData("corner-bl")]
    [InlineData("corner-br")]
    public void GradientIsOptInAndRetainsBaseColorAndFlowPath(string orientation)
    {
        var symbol = JsonSerializer.Deserialize<ScreenSymbol>("{\"Type\":\"pipe\",\"PipeColor\":\"#ef4444\"}")!;
        symbol.PipeOrientation = orientation;
        var flat = XElement.Parse(PipeVisuals.Render(symbol));
        Assert.False(symbol.PipeUseGradient);
        Assert.Empty(flat.Descendants(Svg + "linearGradient"));
        symbol.PipeUseGradient = true;
        var restored = JsonSerializer.Deserialize<ScreenSymbol>(JsonSerializer.Serialize(symbol))!;
        Assert.True(restored.PipeUseGradient);
        var shaded = XElement.Parse(PipeVisuals.Render(restored));
        Assert.Equal("#ef4444", Path(shaded, "pipe-body").Attribute("stroke")!.Value);
        Assert.Equal(Path(flat, "hmi-pipe-flow").Attribute("d")!.Value, Path(shaded, "hmi-pipe-flow").Attribute("d")!.Value);
        Assert.Single(shaded.Elements(Svg + "path"), e => (string?)e.Attribute("class") == "pipe-surface");
        if (orientation.StartsWith("corner")) Assert.Equal(12, shaded.Elements(Svg + "path").Count(e => (string?)e.Attribute("class") == "pipe-shading"));
        else
        {
            var gradient = Assert.Single(shaded.Descendants(Svg + "linearGradient"));
            Assert.Equal(new[] { "0%", "18%", "40%", "53%", "75%", "100%" }, gradient.Elements(Svg + "stop").Select(s => s.Attribute("offset")!.Value).ToArray());
            Assert.Equal(new[] { "#000000", "#000000", "#ffffff", "#ffffff", "#000000", "#000000" }, gradient.Elements(Svg + "stop").Select(s => s.Attribute("stop-color")!.Value).ToArray());
            if (orientation == "horizontal")
            {
                Assert.Equal(Number(gradient, "x1"), Number(gradient, "x2"));
                Assert.Equal(Number(Path(shaded, "pipe-body"), "stroke-width"), Number(gradient, "y2") - Number(gradient, "y1"), 6);
            }
            else
            {
                Assert.Equal(Number(gradient, "y1"), Number(gradient, "y2"));
                Assert.Equal(Number(Path(shaded, "pipe-body"), "stroke-width"), Number(gradient, "x2") - Number(gradient, "x1"), 6);
            }
        }
    }

    [Fact]
    public void ResponsiveRendering_FillsContainerAndGradientIdsDoNotCollide()
    {
        var symbol = new ScreenSymbol { Id = "reused", Width = 420, Height = 50, PipeUseGradient = true };
        var first = XElement.Parse(PipeVisuals.Render(symbol, responsive: true));
        var duplicate = new ScreenSymbol { Id = symbol.Id, Width = 420, Height = 50, PipeUseGradient = true };
        var second = XElement.Parse(PipeVisuals.Render(duplicate, responsive: true));
        Assert.Equal("100%", first.Attribute("width")!.Value);
        Assert.Equal("100%", first.Attribute("height")!.Value);
        Assert.Equal("0 0 420 50", first.Attribute("viewBox")!.Value);
        Assert.NotEqual(Assert.Single(first.Descendants(Svg + "linearGradient")).Attribute("id")!.Value, Assert.Single(second.Descendants(Svg + "linearGradient")).Attribute("id")!.Value);
        Assert.Equal(PipeVisuals.Render(symbol, "true"), PipeVisuals.Render(symbol, "12.5"));
    }

    [Fact]
    public void LegacyNullOrientation_RendersHorizontal()
    {
        var symbol = JsonSerializer.Deserialize<ScreenSymbol>("{\"PipeOrientation\":null,\"Width\":200,\"Height\":40}")!;
        Assert.Equal("M 0 20 H 200", Path(XElement.Parse(PipeVisuals.Render(symbol)), "pipe-body").Attribute("d")!.Value);
    }
}
