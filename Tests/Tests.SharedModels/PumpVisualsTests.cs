using System.Text.Json;
using System.Xml.Linq;
using SharedModels;
using Xunit;

namespace Tests.SharedModels;

public class PumpVisualsTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static XElement Element(XElement root, string name) => Assert.Single(root.Descendants(), e => (string?)e.Attribute("class") == name);

    [Theory]
    [InlineData("centrifugal")]
    [InlineData("isometric")]
    public void EnhancedPump_HasPortsVisibleImpellerAndOptionalShading(string appearance)
    {
        var symbol = new ScreenSymbol { MotorStyle = "pump", MotorAppearance = appearance, MotorBodyColor = "#123456" };
        Assert.True(PumpVisuals.IsEnhanced(symbol));
        var flat = XElement.Parse(PumpVisuals.Render(symbol, "true"));
        Assert.Empty(flat.Descendants(Svg + "linearGradient"));
        Assert.Equal("#123456", Element(flat, "pump-body").Attribute("fill")!.Value);
        Assert.Equal("0", Element(flat, "pump-inlet").Attribute("x")!.Value);
        Assert.Equal("57", Element(flat, "pump-outlet").Attribute("width")!.Value);
        Assert.Equal(6, Element(flat, "hmi-pump-rotor").Elements(Svg + "path").Count());
        Assert.Contains("hmiPumpSpin", Element(flat, "hmi-pump-rotor").Attribute("style")!.Value);
        Assert.Null(Element(flat, "pump-body").Attribute("style"));
        symbol.MotorUseGradient = true;
        var shaded = XElement.Parse(PumpVisuals.Render(symbol, "true"));
        Assert.Equal(4, Assert.Single(shaded.Descendants(Svg + "linearGradient")).Elements(Svg + "stop").Count());
        Assert.StartsWith("url(#pump_", Element(shaded, "pump-sheen").Attribute("fill")!.Value);
        Assert.Equal(appearance == "isometric" ? 1 : 0, shaded.Descendants().Count(e => (string?)e.Attribute("class") == "pump-depth"));
    }

    [Theory]
    [InlineData("true", "false", "running", "#22c55e", true)]
    [InlineData("TRUE", "0", "running", "#22c55e", true)]
    [InlineData("1.0", null, "running", "#22c55e", true)]
    [InlineData("false", null, "stopped", "#64748b", false)]
    [InlineData(null, null, "stopped", "#64748b", false)]
    [InlineData("0", null, "stopped", "#64748b", false)]
    [InlineData("true", "true", "fault", "#ef4444", false)]
    [InlineData("false", "1", "fault", "#ef4444", false)]
    public void State_FaultTakesPriorityAndOnlyRunningImpellerRotates(string? run, string? fault, string expected, string color, bool animated)
    {
        var root = XElement.Parse(PumpVisuals.Render(new ScreenSymbol { MotorStyle = "pump", MotorAppearance = "isometric" }, run, fault));
        Assert.Equal(expected, root.Attribute("data-state")!.Value);
        Assert.Equal(expected.ToUpperInvariant(), Element(root, "pump-state-label").Value);
        Assert.Equal(color, Element(root, "pump-status").Attribute("fill")!.Value);
        Assert.Equal(animated, Element(root, "hmi-pump-rotor").Attribute("style") != null);
        if (animated) Assert.Contains("prefers-reduced-motion:reduce", root.ToString());
    }

    [Fact]
    public void AnimationCanBeDisabledAndClassicMotorFanRemainUnchanged()
    {
        var symbol = new ScreenSymbol { MotorStyle = "pump", MotorAnimateRotation = false };
        Assert.False(PumpVisuals.IsEnhanced(symbol));
        symbol.MotorUseGradient = true;
        Assert.True(PumpVisuals.IsEnhanced(symbol));
        Assert.Null(Element(XElement.Parse(PumpVisuals.Render(symbol, "true")), "hmi-pump-rotor").Attribute("style"));
        foreach (var type in new[] { "motor", "fan" })
        {
            symbol.MotorStyle = type;
            symbol.MotorAppearance = "isometric";
            Assert.False(PumpVisuals.IsEnhanced(symbol));
        }
    }

    [Fact]
    public void SettingsRoundTripAndResponsivePumpKeepsAspectAndStablePaintServers()
    {
        var symbol = new ScreenSymbol { MotorStyle = "pump", MotorAppearance = "isometric", MotorUseGradient = true, MotorBodyColor = "#123456" };
        var copy = JsonSerializer.Deserialize<ScreenSymbol>(JsonSerializer.Serialize(symbol))!;
        Assert.Equal("isometric", copy.MotorAppearance);
        Assert.True(copy.MotorUseGradient);
        Assert.Equal("#123456", copy.MotorBodyColor);
        var root = XElement.Parse(PumpVisuals.Render(symbol, "true", responsive: true));
        Assert.Equal("100%", root.Attribute("width")!.Value);
        Assert.Equal("100%", root.Attribute("height")!.Value);
        Assert.Equal("xMidYMid meet", root.Attribute("preserveAspectRatio")!.Value);
        Assert.Equal(PumpVisuals.Render(symbol, "true"), PumpVisuals.Render(symbol, "1.0"));
        Assert.NotEqual(Assert.Single(root.Descendants(Svg + "linearGradient")).Attribute("id")!.Value,
            Assert.Single(XElement.Parse(PumpVisuals.Render(copy, "true")).Descendants(Svg + "linearGradient")).Attribute("id")!.Value);
    }
}
