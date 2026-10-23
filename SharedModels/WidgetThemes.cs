using System.Globalization;

namespace SharedModels;

/// <summary>Colour palette for one modern widget theme.</summary>
public sealed record WidgetPalette(
    string Name,
    string Surface,
    string SurfaceSolid,
    string Header,
    string Border,
    string Text,
    string Muted,
    string Accent,
    string AccentEnd,
    string Track,
    string Grid,
    string Plot,
    string Hover,
    string Selected,
    string Button,
    string ButtonText,
    string Primary,
    string Shadow,
    string BezelLight,
    string BezelDark,
    string KnobLight,
    string KnobDark);

/// <summary>
/// Opt-in modern themes shared by panel widgets (alarm list, analytics, trend, HDA, event log)
/// and controls (switch, rotary switch, knob, sliders, button) in the editor and runtime.
/// "classic" keeps the legacy appearance untouched.
/// </summary>
public static class WidgetThemes
{
    public const string Classic = "classic";

    /// <summary>Theme identifiers in display order.</summary>
    public static IReadOnlyList<string> Names { get; } = ["classic", "midnight", "ivory", "metallic"];

    /// <summary>Symbol types that honour <see cref="ScreenSymbol.WidgetTheme"/>.</summary>
    public static IReadOnlySet<string> ThemeableTypes { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "alarmlist", "alarmanalytics", "trend", "hdachart", "hdagrid", "historicaltrend", "historicaltable", "eventlog",
        "switch", "rotaryswitch", "knob", "hslider", "vslider", "rangeslider", "button"
    };

    /// <summary>Symbol types drawn by <see cref="ControlVisuals"/> when themed.</summary>
    public static IReadOnlySet<string> ControlTypes { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "switch", "rotaryswitch", "knob", "hslider", "vslider", "rangeslider"
    };

    private static readonly Dictionary<string, WidgetPalette> Palettes = new(StringComparer.Ordinal)
    {
        ["midnight"] = new(
            "midnight",
            Surface: "linear-gradient(155deg, #1e293b 0%, #0f172a 55%, #020617 100%)",
            SurfaceSolid: "#0f172a",
            Header: "rgba(30, 41, 59, 0.85)",
            Border: "#334155",
            Text: "#e2e8f0",
            Muted: "#94a3b8",
            Accent: "#22d3ee",
            AccentEnd: "#3b82f6",
            Track: "#1e293b",
            Grid: "#1e293b",
            Plot: "#0b1220",
            Hover: "rgba(34, 211, 238, 0.08)",
            Selected: "rgba(34, 211, 238, 0.16)",
            Button: "rgba(51, 65, 85, 0.55)",
            ButtonText: "#e2e8f0",
            Primary: "#2563eb",
            Shadow: "0 10px 30px rgba(2, 6, 23, 0.55), inset 0 1px 0 rgba(255, 255, 255, 0.05)",
            BezelLight: "#475569",
            BezelDark: "#020617",
            KnobLight: "#334155",
            KnobDark: "#0f172a"),
        ["ivory"] = new(
            "ivory",
            Surface: "linear-gradient(145deg, #ffffff 0%, #f5f0e6 65%, #e7dcc8 100%)",
            SurfaceSolid: "#fbf8f2",
            Header: "rgba(245, 240, 230, 0.92)",
            Border: "#d6cbb8",
            Text: "#44372a",
            Muted: "#8a7a66",
            Accent: "#d97706",
            AccentEnd: "#facc15",
            Track: "#e4dccd",
            Grid: "#ece4d6",
            Plot: "#fffdf8",
            Hover: "rgba(217, 119, 6, 0.07)",
            Selected: "rgba(217, 119, 6, 0.14)",
            Button: "rgba(255, 255, 255, 0.75)",
            ButtonText: "#44372a",
            Primary: "#d97706",
            Shadow: "0 8px 24px rgba(68, 55, 42, 0.18), inset 0 1px 0 #ffffff",
            BezelLight: "#ffffff",
            BezelDark: "#cbbfa8",
            KnobLight: "#ffffff",
            KnobDark: "#e7dcc8"),
        ["metallic"] = new(
            "metallic",
            Surface: "linear-gradient(160deg, #f8fafc 0%, #eef2f6 40%, #dfe5ec 75%, #cfd8e2 100%)",
            SurfaceSolid: "#e2e8f0",
            Header: "rgba(241, 245, 249, 0.9)",
            Border: "#94a3b8",
            Text: "#0f172a",
            Muted: "#475569",
            Accent: "#0369a1",
            AccentEnd: "#38bdf8",
            Track: "#cbd5e1",
            Grid: "#d8dee6",
            Plot: "#f8fafc",
            Hover: "rgba(3, 105, 161, 0.07)",
            Selected: "rgba(3, 105, 161, 0.14)",
            Button: "rgba(255, 255, 255, 0.7)",
            ButtonText: "#0f172a",
            Primary: "#0369a1",
            Shadow: "0 8px 24px rgba(15, 23, 42, 0.22), inset 0 1px 0 rgba(255, 255, 255, 0.8)",
            BezelLight: "#f8fafc",
            BezelDark: "#64748b",
            KnobLight: "#f1f5f9",
            KnobDark: "#94a3b8")
    };

    /// <summary>Returns a known theme id; unknown or empty values fall back to "classic".</summary>
    public static string Normalize(string? theme)
    {
        var value = theme?.Trim().ToLowerInvariant() ?? "";
        return Palettes.ContainsKey(value) ? value : Classic;
    }

    public static bool IsThemed(string? theme) => Normalize(theme) != Classic;

    public static bool IsThemed(ScreenSymbol symbol) => IsThemed(symbol.WidgetTheme);

    /// <summary>True when the symbol is a control that should be drawn by <see cref="ControlVisuals"/>.</summary>
    public static bool UsesControlVisuals(ScreenSymbol symbol) =>
        ControlTypes.Contains(symbol.Type) && IsThemed(symbol);

    /// <summary>Palette for a theme, or null for classic.</summary>
    public static WidgetPalette? GetPalette(string? theme) =>
        Palettes.TryGetValue(Normalize(theme), out var palette) ? palette : null;

    public static string Label(string? theme) => Normalize(theme) switch
    {
        "midnight" => "Midnight",
        "ivory" => "Ivory",
        "metallic" => "Brushed Metal",
        _ => "Classic"
    };

    /// <summary>CSS custom properties (--wt-*) for HTML widgets. Empty for classic.</summary>
    public static string CssVariables(string? theme)
    {
        var p = GetPalette(theme);
        if (p is null) return "";
        return string.Join(" ",
            $"--wt-surface: {p.Surface};",
            $"--wt-surface-solid: {p.SurfaceSolid};",
            $"--wt-header: {p.Header};",
            $"--wt-border: {p.Border};",
            $"--wt-text: {p.Text};",
            $"--wt-muted: {p.Muted};",
            $"--wt-accent: {p.Accent};",
            $"--wt-accent-end: {p.AccentEnd};",
            $"--wt-track: {p.Track};",
            $"--wt-grid: {p.Grid};",
            $"--wt-plot: {p.Plot};",
            $"--wt-hover: {p.Hover};",
            $"--wt-selected: {p.Selected};",
            $"--wt-button: {p.Button};",
            $"--wt-button-text: {p.ButtonText};",
            $"--wt-primary: {p.Primary};",
            $"--wt-shadow: {p.Shadow};",
            // Existing alarm-list variables, so the legacy rules pick up the theme too.
            $"--alarm-bg: {p.Surface};",
            $"--alarm-header-bg: {p.Header};",
            $"--alarm-border: {p.Border};",
            $"--alarm-text: {p.Text};",
            $"--analytics-border: {p.Border};",
            $"--hda-bg: {p.Surface};",
            $"--hda-text: {p.Text};",
            $"--hda-border: {p.Border};",
            $"--evtlog-bg: {p.Surface};",
            $"--evtlog-text: {p.Text};",
            $"--evtlog-border: {p.Border};",
            $"--evtlog-header-bg: {p.Header};");
    }

    /// <summary>
    /// Applies a theme and coordinated accent colours, mirroring <see cref="GaugeVisuals.ApplyPreset"/>.
    /// "classic" restores the legacy defaults.
    /// </summary>
    public static void ApplyPreset(ScreenSymbol symbol, string theme)
    {
        ArgumentNullException.ThrowIfNull(symbol);
        var key = theme?.Trim().ToLowerInvariant() ?? "";
        if (key == Classic)
        {
            symbol.WidgetTheme = Classic;
            symbol.TrendBackground = "#111827";
            symbol.AlarmAnalyticsBackground = "#0f172a";
            symbol.AlarmAnalyticsForeground = "#e2e8f0";
            symbol.SwitchOnColor = "#34c759";
            symbol.SwitchOffColor = "#787880";
            symbol.SwitchKnobColor = "#e0e0e0";
            symbol.SwitchDialColor = "#2d2d2d";
            symbol.KnobColor = "#3a3a3a";
            symbol.KnobPointerColor = "#ff6b35";
            symbol.SliderTrackColor = "#444";
            symbol.SliderFillColor = "#3b82f6";
            symbol.SliderThumbColor = "#ffffff";
            symbol.RangeTrackColor = "#2a2a3a";
            symbol.RangeFillColor = "#22d3ee";
            symbol.RangeLowHandleColor = "#38bdf8";
            symbol.RangeHighHandleColor = "#f97316";
            symbol.ButtonColor = "#3b82f6";
            symbol.ButtonTextColor = "#ffffff";
            return;
        }

        if (!Palettes.TryGetValue(key, out var p))
            throw new ArgumentOutOfRangeException(nameof(theme), theme, "Unknown widget theme.");

        symbol.WidgetTheme = key;
        symbol.TrendBackground = p.Plot;
        symbol.AlarmAnalyticsBackground = p.SurfaceSolid;
        symbol.AlarmAnalyticsForeground = p.Text;
        symbol.SwitchOnColor = p.Accent;
        symbol.SwitchOffColor = p.Border;
        symbol.SwitchKnobColor = p.KnobLight;
        symbol.SwitchDialColor = p.KnobDark;
        symbol.KnobColor = p.KnobDark;
        symbol.KnobPointerColor = p.Accent;
        symbol.SliderTrackColor = p.Track;
        symbol.SliderFillColor = p.Accent;
        symbol.SliderThumbColor = p.KnobLight;
        symbol.RangeTrackColor = p.Track;
        symbol.RangeFillColor = p.Accent;
        symbol.RangeLowHandleColor = p.Accent;
        symbol.RangeHighHandleColor = p.AccentEnd;
        symbol.ButtonColor = p.Primary;
        symbol.ButtonTextColor = "#ffffff";
    }

    /// <summary>Picks the themed value when a palette is active, otherwise the classic value.</summary>
    public static string Pick(WidgetPalette? palette, string classic, Func<WidgetPalette, string> themed) =>
        palette is null ? classic : themed(palette);

    /// <summary>Colour of the caption drawn under a symbol; classic keeps the legacy dark grey.</summary>
    public static string CaptionColor(ScreenSymbol symbol, string classic = "#333") =>
        GetPalette(symbol.WidgetTheme)?.Muted ?? classic;

    internal static string N(double number) => number.ToString("0.###", CultureInfo.InvariantCulture);
}
