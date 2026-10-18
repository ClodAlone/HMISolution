using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Xml.Linq;

namespace SharedModels;

/// <summary>Shared tank geometry and level rendering for editor and runtime screens.</summary>
public static class TankVisuals
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static readonly ConditionalWeakTable<ScreenSymbol, string> Ids = new();

    public static string Render(ScreenSymbol symbol, string? liveValue = null, bool responsive = false, double? previewLevel = null)
    {
        var w = Dimension(symbol.Width);
        var h = Dimension(symbol.Height);
        var min = symbol.MinValue ?? symbol.TankMin;
        var max = symbol.MaxValue ?? symbol.TankMax;
        var validRange = double.IsFinite(min) && double.IsFinite(max) && max > min && double.IsFinite(max - min);
        var hasValue = double.TryParse(liveValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) && double.IsFinite(value);
        var preview = previewLevel.HasValue && double.IsFinite(previewLevel.Value);
        var fraction = validRange && (hasValue || preview)
            ? Math.Clamp(preview ? previewLevel!.Value : (value - min) / (max - min), 0, 1) : 0;
        var state = !validRange ? "invalid-range" : hasValue || preview ? "ready" : "unavailable";
        var levelText = state == "ready" ? $"{N(Math.Round(fraction * 100))}%" : state == "invalid-range" ? "Invalid range" : "-";
        var description = state switch
        {
            "invalid-range" => "Invalid tank range: maximum must be greater than minimum and both must be finite.",
            "unavailable" => "Tank level unavailable: bind a finite numeric value.",
            _ => $"Tank level {levelText}" + (preview ? " (preview)" : "")
        };
        var root = E("svg", A("viewBox", $"0 0 {N(w)} {N(h)}"), A("width", responsive ? "100%" : N(w)),
            A("height", responsive ? "100%" : N(h)), A("preserveAspectRatio", "xMidYMid meet"), A("class", "hmi-tank"),
            A("data-state", state), A("data-level", fraction), A("role", "img"), A("aria-label", description),
            A("style", "display:block;overflow:hidden;"));
        root.Add(E("title", description));
        var defs = E("defs");
        root.Add(defs);
        var id = Ids.GetValue(symbol, _ => "tank_" + Guid.NewGuid().ToString("N"));
        var bodyColor = symbol.TankBodyColor ?? "#1e293b";
        var liquidColor = symbol.TankLiquidColor ?? "#3b82f6";
        var surfaceColor = symbol.TankFillStyle == "gradient" ? symbol.TankGradientEndColor ?? "#22d3ee" : liquidColor;
        var liquid = liquidColor;
        if (symbol.TankFillStyle == "gradient")
        {
            defs.Add(E("linearGradient", A("id", id + "_liquid"), A("x1", "0%"), A("y1", "100%"), A("x2", "0%"), A("y2", "0%"),
                Stop("0%", liquidColor), Stop("100%", surfaceColor)));
            liquid = $"url(#{id}_liquid)";
        }
        if (symbol.TankUseGradient)
            defs.Add(E("linearGradient", A("id", id + "_sheen"), A("x1", "0%"), A("y1", "0%"), A("x2", "100%"), A("y2", "0%"),
                Stop("0%", "#000000", 0.55), Stop("20%", "#ffffff", 0.25), Stop("38%", "#ffffff", 0.05),
                Stop("72%", "#000000", 0.12), Stop("100%", "#000000", 0.6)));

        // Geometry is linear between these keyframes, including either side of the cone shoulder.
        void AddLiquid(Func<double, XElement> create, double? shoulderLevel = null)
        {
            if (fraction > 0) root.Add(create(fraction));
            var levels = shoulderLevel.HasValue ? new[] { 0, shoulderLevel.Value, 1 } : new[] { 0d, 1d };
            root.Add(A("data-tank-id", id), A("data-tank-template", create(1).ToString(SaveOptions.DisableFormatting)),
                A("data-tank-keyframes", JsonSerializer.Serialize(levels.Select(level => new
                {
                    level,
                    parts = create(level).Elements().Select(e => e.Attributes()
                        .Where(a => a.Name.LocalName is "d" or "points" or "x" or "y" or "width" or "height" or "cx" or "cy" or "rx" or "ry")
                        .ToDictionary(a => a.Name.LocalName, a => a.Value)).ToArray()
                }))));
        }

        var outline = "#94a3b8";
        double textX = w / 2, textY = h / 2;
        if (symbol.TankStyle == "isometric-cone")
        {
            var cx = w / 2;
            var rx = w * 0.38;
            var ry = Math.Min(h * 0.075, w * 0.15);
            var top = h * 0.06 + ry;
            var shoulder = h * 0.68;
            var tip = h * 0.94;
            var coneHeight = tip - shoulder;
            // The cone silhouette is tangent to its elliptical shoulder, not a triangle cutting through it.
            string Shape(double surfaceY, double surfaceRx, double surfaceRy)
            {
                var baseY = Math.Max(surfaceY, shoulder);
                var baseScale = (tip - baseY) / coneHeight;
                var baseRx = rx * baseScale;
                var baseRy = ry * baseScale;
                var slope = ry / coneHeight;
                var tangentX = baseRx * Math.Sqrt(1 - slope * slope);
                var tangentY = baseY + baseRy * slope;
                return $"M {N(cx - surfaceRx)} {N(surfaceY)} A {N(surfaceRx)} {N(surfaceRy)} 0 0 1 {N(cx + surfaceRx)} {N(surfaceY)} V {N(baseY)} A {N(baseRx)} {N(baseRy)} 0 0 1 {N(cx + tangentX)} {N(tangentY)} L {N(cx)} {N(tip)} L {N(cx - tangentX)} {N(tangentY)} A {N(baseRx)} {N(baseRy)} 0 0 1 {N(cx - baseRx)} {N(baseY)} Z";
            }
            var shape = Shape(top, rx, ry);
            root.Add(Path(shape, bodyColor, "tank-body"));
            var cone = Path(Shape(shoulder, rx, ry), "#000000", "tank-cone");
            cone.Add(A("opacity", 0.12));
            root.Add(cone);
            AddLiquid(amount =>
            {
                var level = tip - (tip - top) * amount;
                var scale = Math.Min(1, (tip - level) / coneHeight);
                var surfaceRx = rx * scale;
                var surfaceRy = ry * scale;
                return E("g", A("class", "tank-liquid"),
                    Path(Shape(level, surfaceRx, surfaceRy), liquid, "tank-liquid-front"),
                    Ellipse(cx, level, surfaceRx, surfaceRy, surfaceColor, "tank-surface"),
                    Ellipse(cx, level, surfaceRx, surfaceRy, "#ffffff", "tank-surface-light", 0.25));
            }, coneHeight / (tip - top));
            if (symbol.TankUseGradient)
                root.Add(Path(shape, $"url(#{id}_sheen)", "tank-sheen"));
            var edge = Path(shape, "none", "tank-outline");
            edge.Add(A("stroke", outline), A("stroke-width", Math.Min(w, h) * 0.012), A("stroke-linejoin", "round"));
            var rim = Ellipse(cx, top, rx, ry, "none", "tank-rim");
            rim.Add(A("stroke", outline), A("stroke-width", Math.Min(w, h) * 0.012));
            var seam = Ellipse(cx, shoulder, rx, ry, "none", "tank-cone-seam");
            seam.Add(A("stroke", outline), A("stroke-opacity", 0.5), A("stroke-width", Math.Min(w, h) * 0.006));
            root.Add(edge, seam, rim);
            textY = (top + shoulder) / 2;
        }
        else if (symbol.TankStyle == "isometric")
        {
            var x = w * 0.08;
            var right = w * 0.72;
            var dx = w * 0.2;
            var dy = Math.Min(h * 0.12, w * 0.16);
            var top = h * 0.08 + dy;
            var bottom = h * 0.92;
            string Side(double y) => Points((right, y), (right + dx, y - dy), (right + dx, bottom - dy), (right, bottom));
            string Surface(double y) => Points((x, y), (right, y), (right + dx, y - dy), (x + dx, y - dy));
            var front = Rect(x, top, right - x, bottom - top, bodyColor, "tank-body");
            var side = Polygon(Side(top), bodyColor, "tank-side");
            root.Add(front, side, Polygon(Side(top), "#000000", "tank-side-shadow", 0.3));
            root.Add(Polygon(Surface(top), bodyColor, "tank-top"));
            AddLiquid(amount =>
            {
                var level = bottom - (bottom - top) * amount;
                return E("g", A("class", "tank-liquid"),
                    Rect(x, level, right - x, bottom - level, liquid, "tank-liquid-front"),
                    Polygon(Side(level), liquid, "tank-liquid-side"), Polygon(Side(level), "#000000", "tank-liquid-shadow", 0.22),
                    Polygon(Surface(level), surfaceColor, "tank-surface"), Polygon(Surface(level), "#ffffff", "tank-surface-light", 0.25));
            });
            if (symbol.TankUseGradient)
                root.Add(Rect(x, top, right - x, bottom - top, $"url(#{id}_sheen)", "tank-sheen"));
            root.Add(E("path", A("d", $"M {N(x)} {N(top)} L {N(x + dx)} {N(top - dy)} H {N(right + dx)} V {N(bottom - dy)} L {N(right)} {N(bottom)} H {N(x)} Z M {N(x)} {N(top)} H {N(right)} L {N(right + dx)} {N(top - dy)} M {N(right)} {N(top)} V {N(bottom)}"),
                A("fill", "none"), A("stroke", outline), A("stroke-width", Math.Min(w, h) * 0.009), A("stroke-linejoin", "round"), A("class", "tank-outline")));
            textX = (x + right) / 2;
            textY = (top + bottom) / 2;
        }
        else if (symbol.TankStyle == "cylinder")
        {
            var x = w * 0.12;
            var right = w * 0.88;
            var rx = (right - x) / 2;
            var ry = Math.Min(h * 0.075, w * 0.15);
            var top = h * 0.06 + ry;
            var bottom = h * 0.94 - ry;
            var shape = $"M {N(x)} {N(top)} A {N(rx)} {N(ry)} 0 0 1 {N(right)} {N(top)} V {N(bottom)} A {N(rx)} {N(ry)} 0 0 1 {N(x)} {N(bottom)} Z";
            root.Add(Path(shape, bodyColor, "tank-body"));
            AddLiquid(amount =>
            {
                var level = bottom - (bottom - top) * amount;
                return E("g", A("class", "tank-liquid"),
                    Rect(x, level, right - x, bottom - level, liquid, "tank-liquid-front"),
                    Ellipse(w / 2, bottom, rx, ry, liquidColor, "tank-liquid-bottom"),
                    Ellipse(w / 2, level, rx, ry, surfaceColor, "tank-surface"),
                    Ellipse(w / 2, level, rx, ry, "#ffffff", "tank-surface-light", 0.25));
            });
            if (symbol.TankUseGradient)
                root.Add(Path(shape, $"url(#{id}_sheen)", "tank-sheen"));
            var edge = Path(shape, "none", "tank-outline");
            edge.Add(A("stroke", outline), A("stroke-width", Math.Min(w, h) * 0.012));
            root.Add(edge);
            var rim = Ellipse(w / 2, top, rx, ry, "none", "tank-rim");
            rim.Add(A("stroke", outline), A("stroke-width", Math.Min(w, h) * 0.012));
            root.Add(rim);
        }
        else
        {
            // Both historical rectangular spellings remain valid.
            var pad = Math.Min(1, Math.Min(w, h) / 12);
            var radius = symbol.TankStyle == "round" ? Math.Min(w, h) * 0.2 : 4 * pad;
            var shell = Rect(2 * pad, 2 * pad, w - 4 * pad, h - 4 * pad, bodyColor, "tank-body", radius);
            shell.Add(A("stroke", outline), A("stroke-width", 2 * pad));
            root.Add(shell);
            var inner = Rect(4 * pad, 4 * pad, w - 8 * pad, h - 8 * pad, "white", "tank-clip", Math.Max(0, radius - 2 * pad));
            defs.Add(E("clipPath", A("id", id + "_clip"), inner));
            AddLiquid(amount =>
            {
                var fillHeight = (h - 8 * pad) * amount;
                return E("g", A("class", "tank-liquid"), A("clip-path", $"url(#{id}_clip)"),
                    Rect(4 * pad, h - 4 * pad - fillHeight, w - 8 * pad, fillHeight, liquid, "tank-liquid-front"));
            });
            if (symbol.TankUseGradient)
                root.Add(Rect(2 * pad, 2 * pad, w - 4 * pad, h - 4 * pad, $"url(#{id}_sheen)", "tank-sheen", radius));
        }

        if (symbol.TankShowLevel || state == "invalid-range")
        {
            var size = Math.Min(Math.Min(w, h) * 0.13, 22);
            if (state == "invalid-range") size = Math.Min(size, w / 10);
            root.Add(E("text", A("x", textX), A("y", textY), A("text-anchor", "middle"), A("dominant-baseline", "middle"),
                A("font-family", "system-ui, sans-serif"), A("font-size", size), A("font-weight", "600"),
                A("fill", "#ffffff"), A("stroke", "#0f172a"), A("stroke-width", size * 0.12), A("paint-order", "stroke"),
                A("class", "tank-level"), levelText));
        }
        return root.ToString(SaveOptions.DisableFormatting);
    }

    private static XElement Rect(double x, double y, double width, double height, string fill, string name, double radius = 0) =>
        E("rect", A("x", x), A("y", y), A("width", width), A("height", height), A("rx", radius), A("fill", fill), A("class", name));
    private static XElement Path(string path, string fill, string name) => E("path", A("d", path), A("fill", fill), A("class", name));
    private static XElement Polygon(string points, string fill, string name, double opacity = 1) =>
        E("polygon", A("points", points), A("fill", fill), A("class", name), A("opacity", opacity));
    private static XElement Ellipse(double cx, double cy, double rx, double ry, string fill, string name, double opacity = 1) =>
        E("ellipse", A("cx", cx), A("cy", cy), A("rx", rx), A("ry", ry), A("fill", fill), A("class", name), A("opacity", opacity));
    private static XElement Stop(string offset, string color, double opacity = 1) =>
        E("stop", A("offset", offset), A("stop-color", color), A("stop-opacity", opacity));
    private static string Points(params (double X, double Y)[] points) => string.Join(" ", points.Select(p => $"{N(p.X)},{N(p.Y)}"));
    private static XElement E(string name, params object[] content) => new(Svg + name, content);
    private static XAttribute A(string name, object value) => new(name, value is double d ? N(d) : value);
    private static double Dimension(double value) => double.IsFinite(value) ? Math.Max(1, value) : 1;
    private static string N(double value) => value.ToString("0.######", CultureInfo.InvariantCulture);
}
