using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace SharedModels;

/// <summary>Pipe geometry, surface shading and dimension-aware flow shared by all screen views.</summary>
public static class PipeVisuals
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static readonly ConditionalWeakTable<ScreenSymbol, string> SurfaceIds = new();

    public static bool IsFlowActive(ScreenSymbol symbol, string? liveValue)
    {
        if (!symbol.PipeShowFlow) return false;
        if (string.IsNullOrWhiteSpace(liveValue)) return true;
        if (bool.TryParse(liveValue.Trim(), out var enabled)) return enabled;
        return !double.TryParse(liveValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) ||
            (double.IsFinite(number) && number != 0);
    }

    public static string Render(ScreenSymbol symbol, string? liveValue = null, bool animate = true, bool responsive = false)
    {
        var w = Dimension(symbol.Width);
        var h = Dimension(symbol.Height);
        var orientation = symbol.PipeOrientation ?? "horizontal";
        var horizontal = orientation == "horizontal";
        var corner = orientation.StartsWith("corner-", StringComparison.Ordinal);
        var thickness = corner ? Math.Min(w, h) * 0.3 : (horizontal ? h : w) * 0.6;
        var mid = thickness / 2;
        var bend = thickness;
        var path = orientation switch
        {
            "corner-tl" => $"M {N(w)} {N(mid)} H {N(mid + bend)} Q {N(mid)} {N(mid)} {N(mid)} {N(mid + bend)} V {N(h)}",
            "corner-tr" => $"M 0 {N(mid)} H {N(w - mid - bend)} Q {N(w - mid)} {N(mid)} {N(w - mid)} {N(mid + bend)} V {N(h)}",
            "corner-bl" => $"M {N(mid)} 0 V {N(h - mid - bend)} Q {N(mid)} {N(h - mid)} {N(mid + bend)} {N(h - mid)} H {N(w)}",
            "corner-br" => $"M {N(w - mid)} 0 V {N(h - mid - bend)} Q {N(w - mid)} {N(h - mid)} {N(w - mid - bend)} {N(h - mid)} H 0",
            "horizontal" => $"M 0 {N(h / 2)} H {N(w)}",
            _ => $"M {N(w / 2)} 0 V {N(h)}"
        };
        var root = E("svg", A("viewBox", $"0 0 {N(w)} {N(h)}"), A("width", responsive ? "100%" : N(w)),
            A("height", responsive ? "100%" : N(h)), A("preserveAspectRatio", "none"), A("class", "hmi-pipe"),
            A("style", "display:block;overflow:hidden;"));
        var defs = E("defs");
        root.Add(defs);
        var pipeColor = symbol.PipeColor ?? "#64748b";
        root.Add(Stroke(path, pipeColor, thickness, "pipe-body"));
        if (symbol.PipeUseGradient)
        {
            if (!corner)
            {
                // Stable markup prevents live updates from restarting a running flow animation.
                var id = SurfaceIds.GetValue(symbol, _ => "pipe_surface_" + Guid.NewGuid().ToString("N"));
                defs.Add(E("linearGradient", A("id", id), A("gradientUnits", "userSpaceOnUse"),
                    A("x1", horizontal ? 0 : (w - thickness) / 2), A("y1", horizontal ? (h - thickness) / 2 : 0),
                    A("x2", horizontal ? 0 : (w + thickness) / 2), A("y2", horizontal ? (h + thickness) / 2 : 0),
                    Stop("0%", "#000000", 0.65), Stop("18%", "#000000", 0.2), Stop("40%", "#ffffff", 0.55),
                    Stop("53%", "#ffffff", 0.12), Stop("75%", "#000000", 0.15), Stop("100%", "#000000", 0.7)));
                root.Add(Stroke(path, $"url(#{id})", thickness, "pipe-surface"));
            }
            else
            {
                // Nested strokes follow the elbow instead of painting a diagonal band across its bend.
                root.Add(Stroke(path, "#000000", thickness, "pipe-surface", 0.5));
                for (var i = 0; i < 12; i++)
                {
                    root.Add(Stroke(path, pipeColor, thickness * (0.95 - i * 0.05), "pipe-shading", 0.35));
                }
                root.Add(Stroke(path, "#ffffff", thickness * 0.28, "pipe-highlight", 0.22));
            }
        }

        if (IsFlowActive(symbol, liveValue))
        {
            var flowWidth = thickness * 0.18;
            var dash = thickness * 0.85;
            var gap = thickness * 0.6;
            var cycle = dash + gap;
            var reverse = symbol.PipeFlowDirection is "right-left" or "bottom-top";
            var speed = double.IsFinite(symbol.PipeFlowSpeed) && symbol.PipeFlowSpeed > 0
                ? Math.Clamp(symbol.PipeFlowSpeed, 0.1, 10) : 1;
            var flow = Stroke(path, symbol.PipeFlowColor ?? "#3b82f6", flowWidth, "hmi-pipe-flow");
            flow.Add(A("stroke-dasharray", $"{N(dash)} {N(gap)}"), A("stroke-linecap", "round"));
            if (animate && symbol.PipeAnimate)
            {
                flow.Add(A("style", $"--pipe-cycle:{N(reverse ? -cycle : cycle)};animation:hmiPipeFlow {N(speed)}s linear infinite;"));
                defs.Add(E("style", "@keyframes hmiPipeFlow{from{stroke-dashoffset:var(--pipe-cycle)}to{stroke-dashoffset:0}}@media(prefers-reduced-motion:reduce){.hmi-pipe-flow{animation:none!important}}"));
            }
            root.Add(flow);
        }
        return root.ToString(SaveOptions.DisableFormatting);
    }

    private static XElement Stroke(string path, string color, double width, string className, double opacity = 1) =>
        E("path", A("d", path), A("fill", "none"), A("stroke", color), A("stroke-width", width),
            A("stroke-linejoin", "round"), A("class", className), A("opacity", opacity));
    private static XElement Stop(string offset, string color, double opacity = 1) =>
        E("stop", A("offset", offset), A("stop-color", color), A("stop-opacity", opacity));
    private static XElement E(string name, params object[] content) => new(Svg + name, content);
    private static XAttribute A(string name, object value) => new(name, value is double d ? N(d) : value);
    private static double Dimension(double value) => double.IsFinite(value) ? Math.Max(1, value) : 1;
    private static string N(double value) => value.ToString("0.######", CultureInfo.InvariantCulture);
}
