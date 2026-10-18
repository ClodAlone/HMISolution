using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SharedModels;
using Xunit;

namespace Tests.SharedModels;

public class TankAnimationTests
{
    private static readonly Regex Numbers = new(@"-?\d*\.?\d+(?:e[-+]?\d+)?", RegexOptions.IgnoreCase);

    [Theory]
    [InlineData("rect")]
    [InlineData("rectangular")]
    [InlineData("round")]
    [InlineData("cylinder")]
    [InlineData("isometric")]
    [InlineData("isometric-cone")]
    public void AnimationKeyframes_ReproduceRealGeometryIncludingConeShoulder(string style)
    {
        var symbol = new ScreenSymbol { TankStyle = style, Width = 200, Height = 240, TankFillStyle = "gradient" };
        var root = XElement.Parse(TankVisuals.Render(symbol, "20"));
        using var document = JsonDocument.Parse(root.Attribute("data-tank-keyframes")!.Value);
        var keys = document.RootElement.EnumerateArray().ToArray();
        Assert.Equal(style == "isometric-cone" ? 3 : 2, keys.Length);
        Assert.Equal(0, keys[0].GetProperty("level").GetDouble());
        Assert.Equal(1, keys[^1].GetProperty("level").GetDouble());
        if (style == "isometric-cone") Assert.Equal(62.4 / 193.2, keys[1].GetProperty("level").GetDouble(), 10);
        var template = XElement.Parse(root.Attribute("data-tank-template")!.Value);
        Assert.Equal("tank-liquid", template.Attribute("class")!.Value);
        Assert.Equal(keys[0].GetProperty("parts").GetArrayLength(), template.Elements().Count());

        foreach (var level in new[] { 0.0001, 0.01, 0.1, 0.2, 0.32, 62.4 / 193.2, 0.33, 0.35, 0.5, 0.75, 0.99, 1 })
        {
            var rendered = XElement.Parse(TankVisuals.Render(symbol, previewLevel: level));
            var parts = Assert.Single(rendered.Descendants(), e => (string?)e.Attribute("class") == "tank-liquid").Elements().ToArray();
            var next = Array.FindIndex(keys, k => k.GetProperty("level").GetDouble() >= level);
            var start = keys[next - 1];
            var end = keys[next];
            var t = (level - start.GetProperty("level").GetDouble()) / (end.GetProperty("level").GetDouble() - start.GetProperty("level").GetDouble());
            var a = start.GetProperty("parts");
            var b = end.GetProperty("parts");
            Assert.Equal(parts.Length, a.GetArrayLength());
            for (var i = 0; i < parts.Length; i++)
            {
                foreach (var attribute in a[i].EnumerateObject())
                {
                    var from = Parse(attribute.Value.GetString()!);
                    var to = Parse(b[i].GetProperty(attribute.Name).GetString()!);
                    var expected = Parse(parts[i].Attribute(attribute.Name)!.Value);
                    Assert.Equal(expected.Length, from.Length);
                    Assert.Equal(expected.Length, to.Length);
                    for (var n = 0; n < expected.Length; n++)
                        Assert.InRange(Math.Abs(expected[n] - (from[n] + (to[n] - from[n]) * t)), 0, 0.000002);
                }
            }
        }
    }

    [Fact]
    public void AnimationIdentity_IsStablePerInstanceButNeverSharedByCopiedOrRestoredSymbols()
    {
        var symbol = new ScreenSymbol { Id = "tank", TankStyle = "isometric-cone" };
        var first = XElement.Parse(TankVisuals.Render(symbol, "20"));
        var second = XElement.Parse(TankVisuals.Render(symbol, "50"));
        Assert.Equal(first.Attribute("data-tank-id")!.Value, second.Attribute("data-tank-id")!.Value);
        Assert.Equal(first.Attribute("data-tank-keyframes")!.Value, second.Attribute("data-tank-keyframes")!.Value);
        var restored = JsonSerializer.Deserialize<ScreenSymbol>(JsonSerializer.Serialize(symbol))!;
        var copy = XElement.Parse(TankVisuals.Render(restored, "50"));
        Assert.NotEqual(first.Attribute("data-tank-id")!.Value, copy.Attribute("data-tank-id")!.Value);
        Assert.Equal("0.2", first.Attribute("data-level")!.Value);
        Assert.Equal("0.5", second.Attribute("data-level")!.Value);
    }

    private static double[] Parse(string value) => Numbers.Matches(value).Select(m => double.Parse(m.Value, CultureInfo.InvariantCulture)).ToArray();
}
