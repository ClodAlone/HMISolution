using System.Globalization;
using System.Xml.Linq;

namespace SharedModels;

/// <summary>Shared SVG appearance for opt-in styled gauges in the editor and runtime.</summary>
public static class GaugeVisuals
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    public static bool IsEnhanced(ScreenSymbol symbol) =>
        symbol.GaugeBackground != "none" || symbol.GaugeFillStyle == "gradient" || symbol.GaugeArcThickness != 18;

    public static void ApplyPreset(ScreenSymbol symbol, string preset)
    {
        var palette = preset switch
        {
            "classic" => (Fill: "#4a90d9", End: "#22d3ee", Track: "#e0e0e0", Text: "#333333"),
            "slate" => (Fill: "#3b82f6", End: "#22d3ee", Track: "#334155", Text: "#e2e8f0"),
            "ivory" => (Fill: "#d97706", End: "#facc15", Track: "#ddd5c5", Text: "#44372a"),
            "metallic" => (Fill: "#0369a1", End: "#38bdf8", Track: "#94a3b8", Text: "#0f172a"),
            _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, "Unknown gauge appearance preset.")
        };
        symbol.GaugeBackground = preset == "classic" ? "none" : preset;
        symbol.GaugeFillStyle = preset == "classic" ? "solid" : "gradient";
        symbol.Fill = palette.Fill;
        symbol.GaugeGradientEndColor = palette.End;
        symbol.GaugeTrackColor = palette.Track;
        symbol.GaugeNeedleColor = palette.Text;
        symbol.GaugeArcThickness = 18;
    }

    public static string Render(ScreenSymbol symbol, string fill, double progress, string valueText, double min, double max)
    {
        // Each rendered instance needs its own paint servers, even when embedded screens reuse symbol IDs.
        var id = "gauge_" + Guid.NewGuid().ToString("N");
        var w = Math.Max(1, double.IsFinite(symbol.Width) ? symbol.Width : 1);
        var h = Math.Max(1, double.IsFinite(symbol.Height) ? symbol.Height : 1);
        var pct = double.IsFinite(progress) ? Math.Clamp(progress, 0, 1) : 0;
        var root = E("svg", A("width", w), A("height", h), A("viewBox", $"0 0 {N(w)} {N(h)}"),
            A("font-family", string.IsNullOrWhiteSpace(symbol.FontFamily) ? "sans-serif" : symbol.FontFamily));
        var defs = E("defs");
        root.Add(defs);
        var paint = fill;
        if (symbol.GaugeFillStyle == "gradient")
        {
            defs.Add(E("linearGradient", A("id", id + "_fill"), A("gradientUnits", "userSpaceOnUse"),
                A("x1", 0), A("y1", h), A("x2", w), A("y2", 0),
                Stop("0%", fill), Stop("100%", symbol.GaugeGradientEndColor)));
            paint = $"url(#{id}_fill)";
        }

        var circular = symbol.GaugeStyle is not ("hbar" or "vbar" or "thermometer");
        var cx = w / 2;
        var cy = h / 2;
        var faceR = Math.Min(w, h) * 0.48;
        if (symbol.GaugeBackground != "none")
        {
            var face = symbol.GaugeBackground switch
            {
                "slate" => E("radialGradient", A("id", id + "_face"), A("cx", "35%"), A("cy", "25%"), A("r", "80%"),
                    Stop("0%", "#334155"), Stop("65%", "#0f172a"), Stop("100%", "#020617")),
                "ivory" => E("radialGradient", A("id", id + "_face"), A("cx", "35%"), A("cy", "25%"), A("r", "80%"),
                    Stop("0%", "#ffffff"), Stop("70%", "#f5f0e6"), Stop("100%", "#d6cbb8")),
                "metallic" => E("linearGradient", A("id", id + "_face"), A("x1", "0%"), A("y1", "0%"), A("x2", "100%"), A("y2", "100%"),
                    Stop("0%", "#f8fafc"), Stop("25%", "#cbd5e1"), Stop("48%", "#f1f5f9"), Stop("52%", "#94a3b8"), Stop("100%", "#e2e8f0")),
                _ => throw new ArgumentOutOfRangeException(nameof(symbol.GaugeBackground), symbol.GaugeBackground, "Unknown gauge background.")
            };
            defs.Add(face);
            var background = circular
                ? E("circle", A("cx", cx), A("cy", cy), A("r", faceR))
                : E("rect", A("x", 1), A("y", 1), A("width", Math.Max(0, w - 2)), A("height", Math.Max(0, h - 2)), A("rx", Math.Min(w, h) * 0.12));
            background.Add(A("fill", $"url(#{id}_face)"), A("stroke", symbol.GaugeBackground == "slate" ? "#475569" : "#94a3b8"), A("stroke-width", 1));
            root.Add(background);
            if (circular)
                root.Add(E("circle", A("cx", cx), A("cy", cy), A("r", faceR * 0.95), A("fill", "none"),
                    A("stroke", symbol.GaugeNeedleColor), A("stroke-opacity", 0.15), A("stroke-width", 1)));
        }

        var text = symbol.GaugeNeedleColor;
        var track = symbol.GaugeTrackColor;
        var ticks = Math.Clamp(symbol.GaugeTicks, 0, 20);
        var thickness = double.IsFinite(symbol.GaugeArcThickness) ? Math.Clamp(symbol.GaugeArcThickness, 6, 30) : 18;
        switch (symbol.GaugeStyle)
        {
            case "hbar":
            case "vbar":
            {
                var horizontal = symbol.GaugeStyle == "hbar";
                var x = horizontal ? w * 0.08 : w * 0.38;
                var y = horizontal ? h * 0.48 : h * 0.12;
                var bw = horizontal ? w * 0.84 : w * 0.24;
                var bh = horizontal ? h * 0.24 : h * (symbol.GaugeShowValue ? 0.66 : 0.76);
                root.Add(Rect(x, y, bw, bh, track));
                defs.Add(E("clipPath", A("id", id + "_bar"), Rect(x, y, bw, bh, "white")));
                if (pct > 0)
                    root.Add(E("g", A("clip-path", $"url(#{id}_bar)"), Rect(x, horizontal ? y : y + bh * (1 - pct),
                        horizontal ? bw * pct : bw, horizontal ? bh : bh * pct, paint, rounded: false)));
                if (symbol.GaugeShowValue)
                    root.Add(Text(cx, horizontal ? h * 0.3 : h * 0.9, valueText, text, Math.Min(w, h) * 0.16));
                break;
            }
            case "thermometer":
            {
                var tubeW = Math.Min(w * 0.18, h * 0.12);
                var bulbR = tubeW * 0.9;
                var top = h * (symbol.GaugeShowValue ? 0.23 : 0.1);
                var bottom = h - bulbR * 2.5;
                var tubeH = Math.Max(0, bottom - top);
                root.Add(Rect(cx - tubeW / 2, top, tubeW, tubeH, track));
                if (pct > 0)
                    root.Add(Rect(cx - tubeW / 2, top + tubeH * (1 - pct), tubeW, tubeH * pct + bulbR, paint));
                root.Add(E("circle", A("cx", cx), A("cy", bottom + bulbR * 0.5), A("r", bulbR), A("fill", paint)));
                for (var i = 0; i <= ticks && ticks > 0; i++)
                {
                    var t = (double)i / ticks;
                    var ty = bottom - t * tubeH;
                    root.Add(Line(cx + tubeW, ty, cx + tubeW * 1.4, ty, text, 1));
                    if (symbol.GaugeShowTickLabels)
                        root.Add(Text(cx + tubeW * 1.7, ty, (min + t * (max - min)).ToString("F0", CultureInfo.InvariantCulture), text, Math.Min(w, h) * 0.09, "start"));
                }
                if (symbol.GaugeShowValue)
                    root.Add(Text(cx, h * 0.1, valueText, text, Math.Min(w, h) * 0.12));
                break;
            }
            default:
            {
                var semicircle = symbol.GaugeStyle == "semicircle";
                var needle = symbol.GaugeStyle is not ("arc" or "semicircle");
                cy = semicircle ? h * 0.65 : h * 0.48;
                var r = semicircle ? Math.Min(w * 0.38, h * 0.46) : Math.Min(w, h) * 0.37;
                var sw = r * thickness / 100;
                var start = semicircle ? 180.0 : 135.0;
                var sweep = semicircle ? 180.0 : 270.0;
                var path = Arc(cx, cy, r, start, sweep);
                root.Add(E("path", A("d", path), A("fill", "none"), A("stroke", track), A("stroke-width", sw), A("stroke-linecap", "round")));
                if (pct > 0)
                    root.Add(E("path", A("d", path), A("fill", "none"), A("stroke", paint), A("stroke-width", sw), A("stroke-linecap", "round"),
                        A("pathLength", 100), A("stroke-dasharray", $"{N(pct * 100)} 100")));
                for (var i = 0; i <= ticks && ticks > 0; i++)
                {
                    var t = (double)i / ticks;
                    var a = (start + t * sweep) * Math.PI / 180;
                    root.Add(Line(cx + r * 0.83 * Math.Cos(a), cy + r * 0.83 * Math.Sin(a),
                        cx + r * 0.9 * Math.Cos(a), cy + r * 0.9 * Math.Sin(a), text, Math.Max(0.5, r * 0.012)));
                    if (symbol.GaugeShowTickLabels)
                        root.Add(Text(cx + r * 0.67 * Math.Cos(a), cy + r * 0.67 * Math.Sin(a),
                            (min + t * (max - min)).ToString("F0", CultureInfo.InvariantCulture), text, r * 0.12));
                }
                if (needle)
                {
                    var a = (start + pct * sweep) * Math.PI / 180;
                    var nx = cx + r * 0.62 * Math.Cos(a);
                    var ny = cy + r * 0.62 * Math.Sin(a);
                    var dx = Math.Sin(a) * r * 0.035;
                    var dy = -Math.Cos(a) * r * 0.035;
                    root.Add(E("path", A("d", $"M {N(cx + dx)} {N(cy + dy)} L {N(nx)} {N(ny)} L {N(cx - dx)} {N(cy - dy)} Z"), A("fill", text)));
                    root.Add(E("circle", A("cx", cx), A("cy", cy), A("r", r * 0.07), A("fill", paint), A("stroke", text), A("stroke-width", 1)));
                }
                if (symbol.GaugeShowValue)
                    root.Add(Text(cx, needle ? cy + r * 0.7 : semicircle ? cy - r * 0.18 : cy, valueText, text, r * (needle ? 0.18 : 0.26)));
                break;
            }
        }
        return root.ToString(SaveOptions.DisableFormatting);
    }

    private static string Arc(double cx, double cy, double r, double start, double sweep)
    {
        var a = start * Math.PI / 180;
        var b = (start + sweep) * Math.PI / 180;
        return $"M {N(cx + r * Math.Cos(a))} {N(cy + r * Math.Sin(a))} A {N(r)} {N(r)} 0 {(sweep > 180 ? 1 : 0)} 1 {N(cx + r * Math.Cos(b))} {N(cy + r * Math.Sin(b))}";
    }

    private static XElement Rect(double x, double y, double w, double h, string fill, bool rounded = true) =>
        E("rect", A("x", x), A("y", y), A("width", w), A("height", h), A("rx", rounded ? Math.Min(w, h) / 2 : 0), A("fill", fill));

    private static XElement Line(double x1, double y1, double x2, double y2, string color, double width) =>
        E("line", A("x1", x1), A("y1", y1), A("x2", x2), A("y2", y2), A("stroke", color), A("stroke-width", width));

    private static XElement Text(double x, double y, string value, string color, double size, string anchor = "middle") =>
        E("text", A("x", x), A("y", y), A("text-anchor", anchor), A("dominant-baseline", "middle"),
            A("fill", color), A("font-size", Math.Max(1, size)), A("font-weight", 600), value);

    private static XElement Stop(string offset, string color) => E("stop", A("offset", offset), A("stop-color", color));
    private static XElement E(string name, params object[] content) => new(Svg + name, content);
    private static XAttribute A(string name, object value) => new(name, value is double number ? N(number) : value);
    private static string N(double number) => number.ToString("0.###", CultureInfo.InvariantCulture);
}
