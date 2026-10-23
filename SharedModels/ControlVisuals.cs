using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace SharedModels;

/// <summary>
/// Shared SVG appearance for themed controls (switch, rotary switch, knob, sliders) in the editor and runtime.
/// Geometry (track padding, thumb radius, knob centre) matches the classic renderers so drag hit-testing is unchanged.
/// </summary>
public static class ControlVisuals
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private const string Ease = "cubic-bezier(0.34, 1.36, 0.64, 1)";

    /// <summary>Builds the themed SVG for a control symbol, or null when the symbol is not a themed control.</summary>
    public static XElement? Build(ScreenSymbol symbol, string? liveValue, double width, double height, string? idPrefix = null,
        double? rangeLow = null, double? rangeHigh = null)
    {
        if (!WidgetThemes.UsesControlVisuals(symbol)) return null;
        var palette = WidgetThemes.GetPalette(symbol.WidgetTheme)!;
        var id = Sanitize(idPrefix) ?? "wt_" + Guid.NewGuid().ToString("N");
        var w = Math.Max(1, double.IsFinite(width) ? width : 1);
        var h = Math.Max(1, double.IsFinite(height) ? height : 1);
        var svg = symbol.Type switch
        {
            "switch" => Switch(symbol, palette, id, w, h, liveValue is "True" or "true" or "1"),
            "rotaryswitch" => Rotary(symbol, palette, id, w, h, (int)Parse(liveValue)),
            "knob" => Knob(symbol, palette, id, w, h, Parse(liveValue)),
            "hslider" => Slider(symbol, palette, id, w, h, Parse(liveValue), vertical: false),
            "vslider" => Slider(symbol, palette, id, w, h, Parse(liveValue), vertical: true),
            "rangeslider" => Range(symbol, palette, id, w, h,
                rangeLow ?? symbol.RangeLowValue, rangeHigh ?? symbol.RangeHighValue),
            _ => null
        };
        // Labels may sit outside the control face on an arbitrary screen background: a theme-coloured halo keeps them legible.
        foreach (var text in svg?.Descendants(Svg + "text") ?? [])
        {
            text.SetAttributeValue("stroke", palette.SurfaceSolid);
            text.SetAttributeValue("stroke-opacity", "0.85");
            text.SetAttributeValue("stroke-width", "2.5");
            text.SetAttributeValue("stroke-linejoin", "round");
            text.SetAttributeValue("paint-order", "stroke");
        }
        return svg;
    }

    /// <summary>Renders the themed SVG markup for a control symbol, or an empty string when not themed.</summary>
    public static string Render(ScreenSymbol symbol, string? liveValue, double width, double height, string? idPrefix = null,
        double? rangeLow = null, double? rangeHigh = null) =>
        Build(symbol, liveValue, width, height, idPrefix, rangeLow, rangeHigh)?.ToString(SaveOptions.DisableFormatting) ?? "";

    /// <summary>Intrinsic size used when a themed control is laid out in a responsive (flow) screen.</summary>
    public static (double Width, double Height) ResponsiveSize(ScreenSymbol symbol) => symbol.Type switch
    {
        "switch" => (51, 31),
        "rotaryswitch" => (80, 80),
        "knob" => (80, 90),
        "vslider" => (36, 160),
        "rangeslider" when symbol.RangeOrientation == "vertical" => (36, 160),
        _ => (160, 36)
    };

    private static XElement Root(ScreenSymbol symbol, double w, double h, out XElement defs)
    {
        var root = E("svg", A("width", w), A("height", h), A("viewBox", $"0 0 {N(w)} {N(h)}"), A("overflow", "visible"),
            A("class", $"wt-control wt-{WidgetThemes.Normalize(symbol.WidgetTheme)}"),
            A("font-family", string.IsNullOrWhiteSpace(symbol.FontFamily) ? "sans-serif" : symbol.FontFamily));
        defs = E("defs");
        root.Add(defs);
        // Transparent hit area so the whole control box receives pointer events.
        root.Add(E("rect", A("width", w), A("height", h), A("fill", "transparent")));
        return root;
    }

    private static XElement Switch(ScreenSymbol s, WidgetPalette p, string id, double w, double h, bool isOn)
    {
        var root = Root(s, w, h, out var defs);
        var r = h / 2;
        var knobR = Math.Max(1, r - 3);
        var on = s.SwitchOnColor ?? p.Accent;
        var off = s.SwitchOffColor ?? p.Border;
        defs.Add(Linear(id + "_on", "0%", "0%", "100%", "0%", (0, on), (100, p.AccentEnd)));
        defs.Add(Linear(id + "_sheen", "0%", "0%", "0%", "100%",
            (0, "rgba(255,255,255,0.28)"), (55, "rgba(255,255,255,0)"), (100, "rgba(0,0,0,0.18)")));
        defs.Add(E("radialGradient", A("id", id + "_knob"), A("cx", "35%"), A("cy", "30%"), A("r", "75%"),
            Stop("0%", "#ffffff"), Stop("100%", p.KnobLight)));

        root.Add(Pill(0, 0, w, h, r, off, $"stroke:{p.Border};stroke-width:1;"));
        root.Add(Pill(0, 0, w, h, r, $"url(#{id}_on)",
            $"opacity:{(isOn ? 1 : 0)};transition:opacity 0.3s ease;filter:drop-shadow(0 0 {N(Math.Max(2, h * 0.18))}px {on});"));
        root.Add(Pill(0.5, 0.5, w - 1, h - 1, r - 0.5, $"url(#{id}_sheen)", "pointer-events:none;"));
        var travel = Math.Max(0, w - 2 * r);
        var knobX = r;
        root.Add(E("circle", A("cx", knobX), A("cy", r), A("r", knobR), A("fill", $"url(#{id}_knob)"),
            A("stroke", "rgba(0,0,0,0.12)"), A("stroke-width", 0.5), A("class", "wt-switch-knob"),
            A("style", $"transform:translateX({N(isOn ? travel : 0)}px);transition:transform 0.35s {Ease};" +
                       "filter:drop-shadow(0 2px 3px rgba(0,0,0,0.35));cursor:pointer;")));
        return root;
    }

    private static XElement Rotary(ScreenSymbol s, WidgetPalette p, string id, double w, double h, int rawPos)
    {
        var root = Root(s, w, h, out var defs);
        var n = Math.Clamp(s.SwitchPositions, 2, 12);
        var pos = Math.Clamp(rawPos, 0, n - 1);
        var labels = (s.SwitchPositionLabels ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries);
        var size = Math.Min(w, h);
        var cx = w / 2;
        var cy = h / 2;
        var outerR = Math.Max(4, size / 2 - 2);
        var innerR = outerR * 0.55;
        AddBezelDefs(defs, id, p);

        root.Add(Circle(cx, cy, outerR, $"url(#{id}_bezel)", p.Border, 1, "filter:drop-shadow(0 3px 6px rgba(0,0,0,0.35));"));
        root.Add(Circle(cx, cy, outerR - 3, s.SwitchDialColor ?? p.KnobDark, "none", 0));
        root.Add(Circle(cx, cy, outerR - 3, $"url(#{id}_sheen)", "none", 0, "pointer-events:none;"));

        for (var i = 0; i < n; i++)
        {
            var rad = (-150.0 + 300.0 / (n - 1) * i) * Math.PI / 180;
            var active = i == pos;
            root.Add(Line(cx + (outerR - 5) * Math.Sin(rad), cy - (outerR - 5) * Math.Cos(rad),
                cx + (outerR - 9) * Math.Sin(rad), cy - (outerR - 9) * Math.Cos(rad),
                active ? p.Accent : p.Muted, active ? 2.2 : 1.4,
                active ? $"filter:drop-shadow(0 0 3px {p.Accent});transition:stroke 0.3s ease;" : "transition:stroke 0.3s ease;"));
            var label = i < labels.Length ? labels[i].Trim() : i.ToString(CultureInfo.InvariantCulture);
            root.Add(Text(cx + (outerR + 9) * Math.Sin(rad), cy - (outerR + 9) * Math.Cos(rad) + 3, label,
                active ? p.Text : p.Muted, 8, "middle", active ? "700" : "400"));
        }

        var angle = -150.0 + 300.0 / (n - 1) * pos;
        root.Add(Circle(cx, cy, innerR + 2, $"url(#{id}_bezel)", "none", 0));
        root.Add(Circle(cx, cy, innerR, s.SwitchKnobColor ?? p.KnobLight, "rgba(0,0,0,0.2)", 0.5,
            "filter:drop-shadow(0 2px 4px rgba(0,0,0,0.4));cursor:pointer;"));
        root.Add(Circle(cx, cy, innerR, $"url(#{id}_sheen)", "none", 0, "pointer-events:none;"));
        root.Add(Rotated(cx, cy, angle,
            Line(cx, cy - innerR * 0.2, cx, cy - innerR * 0.8, p.Accent, 3, $"filter:drop-shadow(0 0 2px {p.Accent});"),
            E("circle", A("cx", cx), A("cy", cy - innerR * 0.8), A("r", 1.8), A("fill", p.Accent))));
        root.Add(Circle(cx, cy, 2.5, p.BezelDark, "none", 0));
        return root;
    }

    private static XElement Knob(ScreenSymbol s, WidgetPalette p, string id, double w, double h, double value)
    {
        var root = Root(s, w, h, out var defs);
        var min = s.MinValue ?? 0;
        var max = s.MaxValue ?? 100;
        var frac = max > min ? Math.Clamp((value - min) / (max - min), 0, 1) : 0;
        if (!double.IsFinite(frac)) frac = 0;
        var size = Math.Min(w, h);
        var cx = w / 2;
        var cy = h / 2;
        var outerR = Math.Max(4, size / 2 - 4);
        var innerR = outerR * 0.65;
        var arcW = Math.Max(3, outerR * 0.12);
        var arcR = outerR - arcW / 2;
        var pointer = s.KnobPointerColor ?? p.Accent;
        AddBezelDefs(defs, id, p);
        defs.Add(Linear(id + "_arc", "0%", "100%", "100%", "0%", (0, pointer), (100, p.AccentEnd)));

        var arc = Arc(cx, cy, arcR, -150, 300);
        root.Add(E("path", A("d", arc), A("fill", "none"), A("stroke", p.Track), A("stroke-width", arcW),
            A("stroke-linecap", "round")));
        root.Add(E("path", A("d", arc), A("fill", "none"), A("stroke", $"url(#{id}_arc)"), A("stroke-width", arcW),
            A("stroke-linecap", "round"), A("pathLength", 300), A("class", "wt-knob-arc"),
            A("stroke-dasharray", $"{N(Math.Max(0.001, 300 * frac))} 300"),
            A("style", $"transition:stroke-dasharray 0.3s ease;filter:drop-shadow(0 0 3px {pointer});")));

        if (s.KnobShowScale)
        {
            const int ticks = 5;
            for (var i = 0; i <= ticks; i++)
            {
                var rad = (-150.0 + 300.0 / ticks * i) * Math.PI / 180;
                root.Add(E("circle", A("cx", cx + (arcR - arcW - 2.5) * Math.Sin(rad)), A("cy", cy - (arcR - arcW - 2.5) * Math.Cos(rad)),
                    A("r", 1), A("fill", p.Muted)));
                var tick = min + (max - min) / ticks * i;
                root.Add(Text(cx + (outerR + 9) * Math.Sin(rad), cy - (outerR + 9) * Math.Cos(rad) + 3,
                    tick.ToString("F0", CultureInfo.InvariantCulture), p.Muted, 7));
            }
        }

        var bodyR = Math.Min(innerR, Math.Max(2, arcR - arcW - 5));
        root.Add(Circle(cx, cy, bodyR + 2, $"url(#{id}_bezel)", "none", 0, "filter:drop-shadow(0 3px 6px rgba(0,0,0,0.45));"));
        root.Add(Circle(cx, cy, bodyR, s.KnobColor ?? p.KnobDark, "rgba(0,0,0,0.25)", 0.5,
            "cursor:grab;pointer-events:all;"));
        root.Add(Circle(cx, cy, bodyR, $"url(#{id}_sheen)", "none", 0, "pointer-events:none;"));
        root.Add(Rotated(cx, cy, -150 + 300 * frac,
            Line(cx, cy - bodyR * 0.3, cx, cy - bodyR * 0.82, pointer, 3, $"filter:drop-shadow(0 0 2px {pointer});pointer-events:none;")));
        root.Add(Circle(cx, cy, 2.5, pointer, "none", 0, "pointer-events:none;"));

        if (s.KnobShowValue)
            root.Add(Text(cx, h - 2, value.ToString("F1", CultureInfo.InvariantCulture) + (s.KnobUnit ?? ""), p.Text, 9, "middle", "600"));
        return root;
    }

    private static XElement Slider(ScreenSymbol s, WidgetPalette p, string id, double w, double h, double value, bool vertical)
    {
        var root = Root(s, w, h, out var defs);
        var min = s.MinValue ?? 0;
        var max = s.MaxValue ?? 100;
        var frac = max > min ? Math.Clamp((value - min) / (max - min), 0, 1) : 0;
        if (!double.IsFinite(frac)) frac = 0;
        var fill = s.SliderFillColor ?? p.Accent;
        AddSurface(defs, root, id, p, w, h);
        defs.Add(vertical
            ? Linear(id + "_fill", "0%", "100%", "0%", "0%", (0, fill), (100, p.AccentEnd))
            : Linear(id + "_fill", "0%", "0%", "100%", "0%", (0, fill), (100, p.AccentEnd)));
        AddThumbDefs(defs, id, s.SliderThumbColor ?? p.KnobLight);

        var cross = vertical ? w : h;
        var thick = Math.Max(4, cross * 0.15);
        var thumbR = Math.Min(cross * 0.35, 14);
        var pad = thumbR + 2;
        var length = (vertical ? h : w) - pad * 2;
        var mid = cross / 2;
        var track = s.SliderTrackColor ?? p.Track;
        var glow = $"filter:drop-shadow(0 0 4px {fill});";

        if (!vertical)
        {
            var thumbX = pad + length * frac;
            root.Add(Rect(pad, mid - thick / 2, length, thick, thick / 2, track, "rgba(0,0,0,0.15)"));
            root.Add(Rect(pad, mid - thick / 2, length * frac, thick, thick / 2, $"url(#{id}_fill)", "none",
                $"transition:width 0.25s ease;{glow}"));
            if (s.SliderShowTicks)
                for (var i = 0; i <= 5; i++)
                {
                    var tx = pad + length / 5 * i;
                    root.Add(Line(tx, mid + thick / 2 + 3, tx, mid + thick / 2 + 6, p.Muted, 1, "opacity:0.7;"));
                }
            AddThumb(root, id, p, fill, thumbX, mid, thumbR, "cx");
            if (s.SliderShowValue)
                root.Add(Text(thumbX, mid - thumbR - 4, value.ToString("F1", CultureInfo.InvariantCulture), p.Text, 8, "middle", "600",
                    "transition:x 0.25s ease;"));
        }
        else
        {
            var thumbY = h - pad - length * frac;
            root.Add(Rect(mid - thick / 2, pad, thick, length, thick / 2, track, "rgba(0,0,0,0.15)"));
            root.Add(Rect(mid - thick / 2, h - pad - length * frac, thick, length * frac, thick / 2, $"url(#{id}_fill)", "none",
                $"transition:y 0.25s ease, height 0.25s ease;{glow}"));
            if (s.SliderShowTicks)
                for (var i = 0; i <= 5; i++)
                {
                    var ty = pad + length / 5 * i;
                    root.Add(Line(mid + thick / 2 + 3, ty, mid + thick / 2 + 6, ty, p.Muted, 1, "opacity:0.7;"));
                }
            AddThumb(root, id, p, fill, mid, thumbY, thumbR, "cy");
            if (s.SliderShowValue)
                root.Add(Text(mid + thumbR + 6, thumbY + 3, value.ToString("F1", CultureInfo.InvariantCulture), p.Text, 8, "start", "600",
                    "transition:y 0.25s ease;"));
        }
        return root;
    }

    private static XElement Range(ScreenSymbol s, WidgetPalette p, string id, double w, double h, double low, double high)
    {
        var root = Root(s, w, h, out var defs);
        var min = s.MinValue ?? 0;
        var max = s.MaxValue ?? 100;
        var range = max > min ? max - min : 1;
        var lowFrac = Math.Clamp((low - min) / range, 0, 1);
        var highFrac = Math.Clamp((high - min) / range, 0, 1);
        if (!double.IsFinite(lowFrac)) lowFrac = 0;
        if (!double.IsFinite(highFrac)) highFrac = 0;
        var vertical = s.RangeOrientation == "vertical";
        var lowColor = s.RangeLowHandleColor ?? p.Accent;
        var highColor = s.RangeHighHandleColor ?? p.AccentEnd;
        AddSurface(defs, root, id, p, w, h);
        defs.Add(vertical
            ? Linear(id + "_band", "0%", "100%", "0%", "0%", (0, lowColor), (100, highColor))
            : Linear(id + "_band", "0%", "0%", "100%", "0%", (0, lowColor), (100, highColor)));

        var cross = vertical ? w : h;
        var thick = Math.Max(4, cross * 0.15);
        var thumbR = Math.Min(cross * 0.35, 12);
        var pad = thumbR + 2;
        var length = (vertical ? h : w) - pad * 2;
        var mid = cross / 2;
        var track = s.RangeTrackColor ?? p.Track;
        var band = $"filter:drop-shadow(0 0 4px {s.RangeFillColor ?? p.Accent});";

        if (!vertical)
        {
            var lowX = pad + length * lowFrac;
            var highX = pad + length * highFrac;
            root.Add(Rect(pad, mid - thick / 2, length, thick, thick / 2, track, "rgba(0,0,0,0.15)"));
            root.Add(Rect(Math.Min(lowX, highX), mid - thick / 2, Math.Abs(highX - lowX), thick, thick / 2, $"url(#{id}_band)", "none",
                $"transition:x 0.25s ease, width 0.25s ease;{band}"));
            if (s.RangeShowTicks)
                for (var i = 0; i <= 5; i++)
                {
                    var tx = pad + length / 5 * i;
                    root.Add(Line(tx, mid + thick / 2 + 3, tx, mid + thick / 2 + 6, p.Muted, 1, "opacity:0.7;"));
                }
            AddHandle(root, lowX, mid, thumbR, lowColor, "cx");
            AddHandle(root, highX, mid, thumbR, highColor, "cx");
            if (s.RangeShowValues)
            {
                root.Add(Text(lowX, mid - thumbR - 4, low.ToString("F1", CultureInfo.InvariantCulture), p.Text, 8, "middle", "600"));
                root.Add(Text(highX, mid - thumbR - 4, high.ToString("F1", CultureInfo.InvariantCulture), p.Text, 8, "middle", "600"));
            }
        }
        else
        {
            var lowY = h - pad - length * lowFrac;
            var highY = h - pad - length * highFrac;
            root.Add(Rect(mid - thick / 2, pad, thick, length, thick / 2, track, "rgba(0,0,0,0.15)"));
            root.Add(Rect(mid - thick / 2, Math.Min(lowY, highY), thick, Math.Abs(highY - lowY), thick / 2, $"url(#{id}_band)", "none",
                $"transition:y 0.25s ease, height 0.25s ease;{band}"));
            if (s.RangeShowTicks)
                for (var i = 0; i <= 5; i++)
                {
                    var ty = pad + length / 5 * i;
                    root.Add(Line(mid + thick / 2 + 3, ty, mid + thick / 2 + 6, ty, p.Muted, 1, "opacity:0.7;"));
                }
            AddHandle(root, mid, lowY, thumbR, lowColor, "cy");
            AddHandle(root, mid, highY, thumbR, highColor, "cy");
            if (s.RangeShowValues)
            {
                root.Add(Text(mid + thumbR + 6, lowY + 3, low.ToString("F1", CultureInfo.InvariantCulture), p.Text, 8, "start", "600"));
                root.Add(Text(mid + thumbR + 6, highY + 3, high.ToString("F1", CultureInfo.InvariantCulture), p.Text, 8, "start", "600"));
            }
        }
        return root;
    }

    private static void AddSurface(XElement defs, XElement root, string id, WidgetPalette p, double w, double h)
    {
        var stops = p.Name switch
        {
            "midnight" => new[] { (0, "#1e293b"), (55, "#0f172a"), (100, "#020617") },
            "ivory" => new[] { (0, "#ffffff"), (65, "#f5f0e6"), (100, "#e7dcc8") },
            _ => new[] { (0, "#f8fafc"), (40, "#eef2f6"), (75, "#dfe5ec"), (100, "#cfd8e2") }
        };
        defs.Add(Linear(id + "_surface", "0%", "0%", "100%", "100%", stops));
        root.Add(E("rect", A("width", w), A("height", h), A("rx", Math.Min(10, Math.Min(w, h) / 2)),
            A("fill", $"url(#{id}_surface)"), A("stroke", p.Border), A("stroke-width", 1),
            A("style", "filter:drop-shadow(0 2px 6px rgba(0,0,0,0.25));")));
    }

    private static void AddBezelDefs(XElement defs, string id, WidgetPalette p)
    {
        defs.Add(Linear(id + "_bezel", "0%", "0%", "100%", "100%", (0, p.BezelLight), (100, p.BezelDark)));
        defs.Add(E("radialGradient", A("id", id + "_sheen"), A("cx", "35%"), A("cy", "28%"), A("r", "80%"),
            Stop("0%", "rgba(255,255,255,0.35)"), Stop("55%", "rgba(255,255,255,0)"), Stop("100%", "rgba(0,0,0,0.3)")));
    }

    private static void AddThumbDefs(XElement defs, string id, string thumb) =>
        defs.Add(E("radialGradient", A("id", id + "_thumb"), A("cx", "35%"), A("cy", "30%"), A("r", "75%"),
            Stop("0%", "#ffffff"), Stop("100%", thumb)));

    private static void AddThumb(XElement root, string id, WidgetPalette p, string accent, double x, double y, double r, string axis)
    {
        var transition = $"transition:{axis} 0.25s {Ease};";
        root.Add(E("circle", A("cx", x), A("cy", y), A("r", r), A("fill", $"url(#{id}_thumb)"), A("stroke", p.Border),
            A("stroke-width", 0.75), A("class", "wt-thumb"),
            A("style", $"filter:drop-shadow(0 2px 4px rgba(0,0,0,0.4));{transition}cursor:grab;")));
        root.Add(E("circle", A("cx", x), A("cy", y), A("r", Math.Max(1.5, r * 0.32)), A("fill", accent),
            A("style", $"{transition}pointer-events:none;filter:drop-shadow(0 0 2px {accent});")));
    }

    private static void AddHandle(XElement root, double x, double y, double r, string color, string axis) =>
        root.Add(E("circle", A("cx", x), A("cy", y), A("r", r), A("fill", color), A("stroke", "#ffffff"), A("stroke-width", 1.5),
            A("class", "wt-thumb"),
            A("style", $"filter:drop-shadow(0 2px 4px rgba(0,0,0,0.45));transition:{axis} 0.25s {Ease};cursor:grab;")));

    private static XElement Rotated(double cx, double cy, double angle, params XElement[] children) =>
        E("g", A("style", $"transform-origin:{N(cx)}px {N(cy)}px;transform:rotate({N(angle)}deg);transition:transform 0.35s {Ease};"),
            children);

    private static string Arc(double cx, double cy, double r, double start, double sweep)
    {
        static (double X, double Y) Point(double cx, double cy, double r, double deg)
        {
            var rad = deg * Math.PI / 180;
            return (cx + r * Math.Sin(rad), cy - r * Math.Cos(rad));
        }
        var a = Point(cx, cy, r, start);
        var b = Point(cx, cy, r, start + sweep);
        return $"M {N(a.X)} {N(a.Y)} A {N(r)} {N(r)} 0 {(sweep > 180 ? 1 : 0)} 1 {N(b.X)} {N(b.Y)}";
    }

    private static XElement Pill(double x, double y, double w, double h, double r, string fill, string style) =>
        E("rect", A("x", x), A("y", y), A("width", Math.Max(0, w)), A("height", Math.Max(0, h)), A("rx", Math.Max(0, r)),
            A("fill", fill), A("style", style));

    private static XElement Rect(double x, double y, double w, double h, double r, string fill, string stroke, string style = "") =>
        E("rect", A("x", x), A("y", y), A("width", Math.Max(0, w)), A("height", Math.Max(0, h)), A("rx", r), A("fill", fill),
            A("stroke", stroke), A("stroke-width", stroke == "none" ? 0 : 1), A("style", style));

    private static XElement Circle(double cx, double cy, double r, string fill, string stroke, double strokeWidth, string style = "") =>
        E("circle", A("cx", cx), A("cy", cy), A("r", Math.Max(0, r)), A("fill", fill), A("stroke", stroke),
            A("stroke-width", strokeWidth), A("style", style));

    private static XElement Line(double x1, double y1, double x2, double y2, string color, double width, string style = "") =>
        E("line", A("x1", x1), A("y1", y1), A("x2", x2), A("y2", y2), A("stroke", color), A("stroke-width", width),
            A("stroke-linecap", "round"), A("style", style));

    private static XElement Text(double x, double y, string value, string color, double size, string anchor = "middle",
        string weight = "400", string style = "") =>
        E("text", A("x", x), A("y", y), A("text-anchor", anchor), A("fill", color), A("font-size", size),
            A("font-weight", weight), A("style", "pointer-events:none;" + style), value);

    private static XElement Linear(string id, string x1, string y1, string x2, string y2, params (int Offset, string Color)[] stops) =>
        E("linearGradient", A("id", id), A("x1", x1), A("y1", y1), A("x2", x2), A("y2", y2),
            stops.Select(s => Stop($"{s.Offset}%", s.Color)));

    private static XElement Stop(string offset, string color) => E("stop", A("offset", offset), A("stop-color", color));
    private static XElement E(string name, params object[] content) => new(Svg + name, content);
    private static XAttribute A(string name, object value) => new(name, value is double number ? N(number) : value);
    private static string N(double number) => WidgetThemes.N(number);

    private static double Parse(string? value) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) && double.IsFinite(v) ? v : 0;

    private static string? Sanitize(string? prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix)) return null;
        var sb = new StringBuilder(prefix.Length + 3).Append("wt_");
        foreach (var c in prefix)
            sb.Append(char.IsAsciiLetterOrDigit(c) || c is '_' or '-' ? c : '_');
        return sb.ToString();
    }
}
