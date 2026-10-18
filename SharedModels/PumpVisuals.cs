using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace SharedModels;

/// <summary>Opt-in pump surfaces and impeller animation shared by editor and runtime.</summary>
public static class PumpVisuals
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static readonly ConditionalWeakTable<ScreenSymbol, string> Ids = new();

    public static bool IsEnhanced(ScreenSymbol symbol) => symbol.MotorStyle == "pump" &&
        (symbol.MotorAppearance is "centrifugal" or "isometric" || symbol.MotorUseGradient);

    public static bool IsOn(string? value) => bool.TryParse(value?.Trim(), out var enabled) ? enabled :
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && double.IsFinite(number) && number != 0;

    public static string Render(ScreenSymbol symbol, string? liveValue, string? faultValue = null, bool responsive = false)
    {
        var running = IsOn(liveValue);
        var fault = IsOn(faultValue);
        var state = fault ? "fault" : running ? "running" : "stopped";
        var color = fault ? symbol.MotorFaultColor : running ? symbol.MotorRunColor : symbol.MotorStopColor;
        var width = double.IsFinite(symbol.Width) ? Math.Max(1, symbol.Width) : 1;
        var height = double.IsFinite(symbol.Height) ? Math.Max(1, symbol.Height) : 1;
        var root = E("svg", A("viewBox", "0 0 160 120"), A("width", responsive ? "100%" : N(width)),
            A("height", responsive ? "100%" : N(height)), A("preserveAspectRatio", "xMidYMid meet"),
            A("class", "hmi-pump"), A("data-state", state), A("role", "img"), A("aria-label", "Pump " + state), A("style", "display:block;"));
        root.Add(E("title", "Pump " + state));
        var defs = E("defs");
        root.Add(defs);
        var id = Ids.GetValue(symbol, _ => "pump_" + Guid.NewGuid().ToString("N"));
        var body = symbol.MotorBodyColor ?? "#475569";
        var paint = body;
        if (symbol.MotorUseGradient)
        {
            defs.Add(E("linearGradient", A("id", id + "_body"), A("x1", "0%"), A("y1", "0%"), A("x2", "0%"), A("y2", "100%"),
                Stop("0%", "#000000", 0.6), Stop("28%", "#ffffff", 0.45), Stop("48%", "#ffffff", 0.08), Stop("100%", "#000000", 0.65)));
            paint = $"url(#{id}_body)";
        }
        var iso = symbol.MotorAppearance == "isometric";
        if (iso)
        {
            root.Add(E("path", A("d", "M 24 100 L 43 88 H 146 L 128 106 H 24 Z"), A("fill", "#334155"), A("stroke", "#94a3b8"), A("class", "pump-base")));
            root.Add(E("path", A("d", "M 58 33 L 78 21 H 114 L 94 33 Z M 94 33 L 114 21 V 67 L 94 81 Z"),
                A("fill", body), A("stroke", "#94a3b8"), A("class", "pump-depth")));
            root.Add(E("ellipse", A("cx", 78), A("cy", 57), A("rx", 34), A("ry", 37), A("fill", body), A("stroke", "#94a3b8")));
        }
        root.Add(Rect(0, 50, 42, 20, body, "pump-inlet"), Rect(103, 50, 57, 20, body, "pump-outlet"));
        if (symbol.MotorUseGradient)
            root.Add(Rect(0, 50, 42, 20, paint, "pump-inlet-sheen"), Rect(103, 50, 57, 20, paint, "pump-outlet-sheen"));
        root.Add(Rect(42, 88, 13, 15, body, "pump-foot"), Rect(91, 88, 13, 15, body, "pump-foot"));
        root.Add(E("circle", A("cx", 73), A("cy", 60), A("r", 36), A("fill", body), A("stroke", color ?? "#64748b"), A("stroke-width", 2.5), A("class", "pump-body")));
        if (symbol.MotorUseGradient)
            root.Add(E("circle", A("cx", 73), A("cy", 60), A("r", 35), A("fill", paint), A("class", "pump-sheen")));
        root.Add(E("circle", A("cx", 73), A("cy", 60), A("r", 26), A("fill", "#0f172a"), A("stroke", "#94a3b8"), A("stroke-width", 1.5)));
        var rotor = E("g", A("class", "hmi-pump-rotor"), A("fill", color ?? "#64748b"));
        for (var i = 0; i < 6; i++)
            rotor.Add(E("path", A("d", "M 73 56 Q 91 35 96 49 Q 82 47 78 62 Z"), A("transform", $"rotate({i * 60} 73 60)")));
        if (running && !fault && symbol.MotorAnimateRotation)
        {
            rotor.Add(A("style", "transform-origin:73px 60px;animation:hmiPumpSpin 0.8s linear infinite;"));
            defs.Add(E("style", "@keyframes hmiPumpSpin{from{transform:rotate(0deg)}to{transform:rotate(360deg)}}@media(prefers-reduced-motion:reduce){.hmi-pump-rotor{animation:none!important}}"));
        }
        root.Add(rotor, E("circle", A("cx", 73), A("cy", 60), A("r", 7), A("fill", "#cbd5e1"), A("stroke", "#475569")));
        root.Add(E("circle", A("cx", 122), A("cy", 32), A("r", 5), A("fill", color ?? "#64748b"), A("class", "pump-status")));
        root.Add(E("text", A("x", 80), A("y", 116), A("text-anchor", "middle"), A("font-family", "system-ui, sans-serif"),
            A("font-size", 9), A("font-weight", "600"), A("fill", color ?? "#64748b"), A("class", "pump-state-label"), state.ToUpperInvariant()));
        return root.ToString(SaveOptions.DisableFormatting);
    }

    private static XElement Rect(double x, double y, double w, double h, string fill, string name) =>
        E("rect", A("x", x), A("y", y), A("width", w), A("height", h), A("fill", fill), A("class", name));
    private static XElement Stop(string offset, string color, double opacity) => E("stop", A("offset", offset), A("stop-color", color), A("stop-opacity", opacity));
    private static XElement E(string name, params object[] content) => new(Svg + name, content);
    private static XAttribute A(string name, object value) => new(name, value is double d ? N(d) : value);
    private static string N(double value) => value.ToString("0.######", CultureInfo.InvariantCulture);
}
