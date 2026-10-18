using System.Globalization;
using System.Xml.Linq;

namespace SharedModels;

/// <summary>Shared display faces and readouts for editor previews and runtime widgets.</summary>
public static class DisplayVisuals
{
    private static readonly XNamespace Html = "http://www.w3.org/1999/xhtml";
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private const string TerminalFont = "Consolas, 'Liberation Mono', monospace";
    private static readonly string[] Segments =
    [
        "10,2 50,2 44,8 16,8", "51,4 51,44 45,39 45,10",
        "51,48 51,88 45,82 45,54", "10,90 50,90 44,84 16,84",
        "9,48 15,54 15,82 9,88", "9,4 15,10 15,39 9,44",
        "10,46 16,41 44,41 50,46 44,51 16,51"
    ];
    private static readonly int[] DigitMasks = [0x3f, 0x06, 0x5b, 0x4f, 0x66, 0x6d, 0x7d, 0x07, 0x7f, 0x6f];

    public static bool IsEnhanced(ScreenSymbol symbol) => symbol.DisplayBackgroundStyle != "classic" ||
        symbol.DisplayFontStyle != "default" || symbol.DisplayAutoFit;

    public static string PresetLabel(string preset) => preset switch
    {
        "classic" => "Classic", "midnight" => "Midnight", "ivory" => "Ivory",
        "metallic" => "Brushed Metal", "lcd" => "LCD", _ => throw new ArgumentOutOfRangeException(nameof(preset))
    };

    public static void ApplyPreset(ScreenSymbol symbol, string preset)
    {
        var color = preset switch
        {
            "classic" => symbol.Type == "editbox" ? "#e2e8f0" : "#00e5ff",
            "midnight" => "#22d3ee", "ivory" => "#78350f", "metallic" => "#0c4a6e", "lcd" => "#24351c",
            _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, "Unknown display appearance preset.")
        };
        symbol.DisplayBackgroundStyle = preset;
        symbol.DisplayFontStyle = preset switch { "classic" => "default", "ivory" => "terminal", _ => "seven-segment" };
        symbol.DisplayAutoFit = preset != "classic";
        symbol.DisplayTextColor = color;
        if (symbol.Type == "numericdisplay") symbol.Fill = color;
    }

    public static string Background(ScreenSymbol symbol, string? customColor = null) => symbol.DisplayBackgroundStyle switch
    {
        "classic" => customColor ?? (symbol.Type == "editbox" ? symbol.Fill : symbol.NumericDisplayBackground) ?? "#1e293b",
        "midnight" => "linear-gradient(155deg, #334155 0%, #0f172a 48%, #020617 100%)",
        "ivory" => "linear-gradient(145deg, #ffffff 0%, #f5f0e6 65%, #d6cbb8 100%)",
        "metallic" => "linear-gradient(135deg, #f8fafc 0%, #cbd5e1 30%, #f1f5f9 48%, #94a3b8 52%, #e2e8f0 100%)",
        "lcd" => "linear-gradient(180deg, #a2b68a, #d3dfb8 35%, #b4c99a)",
        _ => throw new ArgumentOutOfRangeException(nameof(symbol.DisplayBackgroundStyle), symbol.DisplayBackgroundStyle, "Unknown display background.")
    };

    public static string FrameStyle(ScreenSymbol symbol, string? customColor = null) =>
        $"width:100%;height:100%;min-width:0;min-height:0;box-sizing:border-box;overflow:hidden;position:relative;container-type:size;background:{Background(symbol, customColor)};border:{N(Math.Max(0, symbol.StrokeWidth))}px solid {symbol.Stroke ?? "#475569"};border-radius:8px;box-shadow:inset 0 1px 2px #ffffff30,inset 0 -2px 6px #00000030;";

    public static string Foreground(ScreenSymbol symbol, string? numericColor = null) =>
        symbol.Type == "editbox" ? symbol.DisplayTextColor : numericColor ?? symbol.Fill ?? "#00e5ff";

    public static string FormatValue(ScreenSymbol symbol, string? raw)
    {
        if (raw == null) return "—";
        var format = symbol.Type == "editbox" ? symbol.EditBoxFormat : symbol.NumericDisplayFormat;
        return !string.IsNullOrEmpty(format) && double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? value.ToString(format, CultureInfo.InvariantCulture) : raw;
    }

    public static string FontCss(ScreenSymbol symbol, string value, double heightPercent = 80, double reservedWidth = 20)
    {
        var family = symbol.DisplayFontStyle == "default"
            ? (string.IsNullOrWhiteSpace(symbol.FontFamily) ? "sans-serif" : symbol.FontFamily) : TerminalFont;
        // Container units use this display's actual dimensions, not the browser viewport.
        var size = symbol.DisplayAutoFit
            ? $"min({N(heightPercent)}cqh, calc((100cqw - {N(reservedWidth)}px) / {N(Math.Max(1, value.Length) * 0.75)}))"
            : $"{N(symbol.FontSize > 0 ? symbol.FontSize : 24)}px";
        return $"font-family:{family};font-size:{size};font-weight:{(string.IsNullOrEmpty(symbol.FontWeight) ? "600" : symbol.FontWeight)};font-style:{(string.IsNullOrEmpty(symbol.FontStyle) ? "normal" : symbol.FontStyle)};font-variant-numeric:tabular-nums;line-height:1.1;";
    }

    public static string Render(ScreenSymbol symbol, string value, string? numericColor = null, string? customBackground = null)
    {
        var root = new XElement(Html + "div", new XAttribute("class", "styled-display"), new XAttribute("style", FrameStyle(symbol, customBackground)));
        var unit = symbol.Type == "numericdisplay" ? symbol.NumericDisplayUnit : "";
        root.Add(XElement.Parse(Readout(symbol, value, numericColor, !string.IsNullOrEmpty(unit))));
        if (!string.IsNullOrEmpty(unit))
            root.Add(new XElement(Html + "div", new XAttribute("class", "display-unit"),
                new XAttribute("style", $"position:absolute;bottom:5%;left:8px;right:8px;text-align:center;white-space:nowrap;overflow:hidden;color:{Foreground(symbol, numericColor)};font: min(16cqh, calc((100cqw - 20px) / {N(Math.Max(1, unit.Length) * 0.75)})) sans-serif;"), unit));
        return root.ToString(SaveOptions.DisableFormatting);
    }

    public static string Readout(ScreenSymbol symbol, string value, string? numericColor = null, bool hasUnit = false)
    {
        var root = new XElement(Html + "div", new XAttribute("class", "display-readout"),
            new XAttribute("style", $"position:absolute;inset:8% 10px {(hasUnit ? "28%" : "8%")};display:flex;align-items:center;justify-content:center;overflow:hidden;min-width:0;color:{Foreground(symbol, numericColor)};"));
        if (symbol.DisplayFontStyle == "seven-segment" && value.Length > 0 && value.All(c => char.IsAsciiDigit(c) || c is '-' or '+' or '.' or ',' or ' '))
        {
            var width = value.Sum(c => c is '.' or ',' ? 20 : 64);
            var svg = new XElement(Svg + "svg", new XAttribute("class", "seven-segment-readout"),
                new XAttribute("viewBox", $"0 0 {width} 94"), new XAttribute("role", "img"), new XAttribute("aria-label", value),
                new XAttribute("style", symbol.DisplayAutoFit ? "width:100%;height:100%;display:block;" : $"height:{N(symbol.FontSize > 0 ? symbol.FontSize : 24)}px;width:{N(width / 94.0 * (symbol.FontSize > 0 ? symbol.FontSize : 24))}px;flex-shrink:0;"));
            svg.Add(new XElement(Svg + "title", value));
            var x = 0;
            foreach (var c in value)
            {
                var group = new XElement(Svg + "g", new XAttribute("transform", $"translate({x} 0)"), new XAttribute("fill", "currentColor"));
                if (c is '.' or ',') group.Add(new XElement(Svg + "circle", new XAttribute("cx", 7), new XAttribute("cy", 86), new XAttribute("r", 5)));
                else if (c != ' ')
                {
                    var mask = char.IsAsciiDigit(c) ? DigitMasks[c - '0'] : 0x40;
                    for (var i = 0; i < Segments.Length; i++)
                        group.Add(new XElement(Svg + "polygon", new XAttribute("points", Segments[i]), new XAttribute("opacity", (mask & (1 << i)) == 0 ? "0.07" : "1")));
                    if (c == '+') group.Add(new XElement(Svg + "rect", new XAttribute("x", 27), new XAttribute("y", 27), new XAttribute("width", 6), new XAttribute("height", 38)));
                }
                svg.Add(group);
                x += c is '.' or ',' ? 20 : 64;
            }
            root.Add(svg);
        }
        else
        {
            root.Add(new XElement(Html + "span", new XAttribute("class", "display-text"), new XAttribute("style",
                FontCss(symbol, value, hasUnit ? 52 : 72) + "white-space:pre;"), value));
        }
        return root.ToString(SaveOptions.DisableFormatting);
    }

    private static string N(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
}
