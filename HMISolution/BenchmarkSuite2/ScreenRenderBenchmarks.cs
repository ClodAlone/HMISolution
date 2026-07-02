// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using BenchmarkDotNet.Attributes;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VSDiagnostics;

[CPUUsageDiagnoser]
public class ScreenRenderBenchmarks
{
    private List<ScreenSymbol> _symbols = null!;
    private List<ImageResource> _images = null!;
    private Dictionary<string, string> _liveValues = null!;
    private List<ScreenSymbol> _sortedSymbols = null!;
    private Dictionary<string, string> _imageLookup = null!;
    private Dictionary<string, string> _fontStyleCache = null!;
    [Params(50, 200)]
    public int SymbolCount;
    [GlobalSetup]
    public void Setup()
    {
        _images = new List<ImageResource>();
        for (int i = 0; i < 20; i++)
            _images.Add(new ImageResource { Id = $"img_{i}", Data = $"data:image/png;base64,{new string ('A', 200)}" });
        _symbols = new List<ScreenSymbol>();
        _liveValues = new Dictionary<string, string>();
        var rng = new Random(42);
        for (int i = 0; i < SymbolCount; i++)
        {
            var sym = new ScreenSymbol
            {
                Id = $"sym_{i}",
                Order = rng.Next(0, SymbolCount),
                VariablePath = $"Var.Path_{i}",
                Fill = "#4a90d9",
                FontFamily = i % 3 == 0 ? "Roboto" : "",
                FontSize = i % 4 == 0 ? 16 : 0,
                FontWeight = i % 5 == 0 ? "bold" : "",
                FontStyle = i % 7 == 0 ? "italic" : "",
                FillBinding = i % 3 == 0 ? "value > 50 ? '#ff0000' : '#00ff00'" : null,
                VisibilityBinding = i % 4 == 0 ? "value >= 0" : null,
                RotationBinding = i % 5 == 0 ? "value * 3.6" : null,
                LabelBinding = i % 3 == 0 ? "value + ' °C'" : null,
                MinValue = 0,
                MaxValue = 100,
            };
            _symbols.Add(sym);
            _liveValues[$"Var.Path_{i}"] = rng.NextDouble() * 100.0 is var v ? v.ToString(CultureInfo.InvariantCulture) : "0";
        }
        _sortedSymbols = _symbols.OrderBy(s => s.Order).ToList();
        _fontStyleCache = new Dictionary<string, string>(StringComparer.Ordinal);
        _imageLookup = new Dictionary<string, string>(_images.Count, StringComparer.Ordinal);
        foreach (var img in _images)
            _imageLookup.TryAdd(img.Id, img.Data);
    }

    [Benchmark(Description = "Full render cycle: OrderBy + Eval per symbol")]
    public int SimulateRenderCycle()
    {
        int renderedCount = 0;
        foreach (var sym in _sortedSymbols)
        {
            _liveValues.TryGetValue(sym.VariablePath ?? "", out var liveValue);
            var fillColor = EvalFillBinding(sym, liveValue);
            var visible = EvalVisibilityBinding(sym, liveValue);
            var labelText = EvalLabelBinding(sym, liveValue);
            var rotation = EvalRotationBinding(sym, liveValue);
            var fontStyle = WidgetFontStyle(sym);
            renderedCount++;
        }

        return renderedCount;
    }

    [Benchmark(Description = "OrderBy sort only")]
    public int OrderBySort()
    {
        int count = 0;
        foreach (var sym in _sortedSymbols)
            count++;
        return count;
    }

    [Benchmark(Description = "WidgetFontStyle × N")]
    public int WidgetFontStyleAll()
    {
        int len = 0;
        foreach (var sym in _symbols)
            len += WidgetFontStyle(sym).Length;
        return len;
    }

    [Benchmark(Description = "EvalFillBinding × N")]
    public int EvalFillAll()
    {
        int len = 0;
        foreach (var sym in _symbols)
        {
            _liveValues.TryGetValue(sym.VariablePath ?? "", out var lv);
            len += EvalFillBinding(sym, lv).Length;
        }

        return len;
    }

    [Benchmark(Description = "ResolveImageData linear scan × N")]
    public int ResolveImageAll()
    {
        int found = 0;
        for (int i = 0; i < _symbols.Count; i++)
        {
            var imageId = $"img_{i % 20}";
            if (ResolveImageData(imageId) != null)
                found++;
        }

        return found;
    }

    // ─── Inlined render-path methods (exact copy from ScreenRenderer.razor) ───
    private string WidgetFontStyle(ScreenSymbol sym)
    {
        if (_fontStyleCache.TryGetValue(sym.Id, out var cached))
            return cached;

        bool hasFamily = !string.IsNullOrEmpty(sym.FontFamily);
        bool hasSize = sym.FontSize > 0;
        bool hasWeight = !string.IsNullOrEmpty(sym.FontWeight);
        bool hasStyle = !string.IsNullOrEmpty(sym.FontStyle);

        string result;
        if (!hasFamily && !hasSize && !hasWeight && !hasStyle)
            result = string.Empty;
        else
            result = string.Concat(
                hasFamily ? string.Concat("font-family: ", sym.FontFamily, "; --widget-font-family: ", sym.FontFamily, "; ") : "",
                hasSize ? string.Concat("font-size: ", sym.FontSize.ToString(), "px; --widget-font-size: ", sym.FontSize.ToString(), "px; ") : "",
                hasWeight ? string.Concat("font-weight: ", sym.FontWeight, "; --widget-font-weight: ", sym.FontWeight, "; ") : "",
                hasStyle ? string.Concat("font-style: ", sym.FontStyle, "; --widget-font-style: ", sym.FontStyle, "; ") : "");

        _fontStyleCache[sym.Id] = result;
        return result;
    }

    private string EvalFillBinding(ScreenSymbol sym, string? liveValue)
    {
        if (string.IsNullOrEmpty(sym.FillBinding) || liveValue == null)
            return sym.Fill;
        try
        {
            var numVal = ParseNumericValue(liveValue);
            if (sym.FillBinding.Contains('?') && sym.FillBinding.Contains(':'))
            {
                var result = EvalSimpleCondition(sym.FillBinding, liveValue, numVal);
                if (result != null)
                    return result;
            }
        }
        catch
        {
        }

        return sym.Fill;
    }

    private bool EvalVisibilityBinding(ScreenSymbol sym, string? liveValue)
    {
        if (string.IsNullOrEmpty(sym.VisibilityBinding) || liveValue == null)
            return true;
        try
        {
            var numVal = ParseNumericValue(liveValue);
            return EvalSimpleBool(sym.VisibilityBinding, liveValue, numVal);
        }
        catch
        {
        }

        return true;
    }

    private double EvalRotationBinding(ScreenSymbol sym, string? liveValue)
    {
        if (string.IsNullOrEmpty(sym.RotationBinding) || liveValue == null)
            return sym.Rotation;
        try
        {
            var numVal = ParseNumericValue(liveValue);
            if (numVal.HasValue)
            {
                var expr = sym.RotationBinding.Replace("value", numVal.Value.ToString(CultureInfo.InvariantCulture));
                if (expr.Contains('*'))
                {
                    var parts = expr.Split('*');
                    if (parts.Length == 2 && double.TryParse(parts[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var a) && double.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var b))
                        return a * b;
                }

                if (double.TryParse(expr.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    return d;
            }
        }
        catch
        {
        }

        return sym.Rotation;
    }

    private string? EvalLabelBinding(ScreenSymbol sym, string? liveValue)
    {
        if (string.IsNullOrEmpty(sym.LabelBinding) || liveValue == null)
            return null;
        try
        {
            if (sym.LabelBinding.Contains("value"))
            {
                var suffix = ExtractLabelSuffix(sym.LabelBinding, liveValue);
                if (suffix.Length > 0)
                    return string.Concat(liveValue, " ", suffix);
                return liveValue;
            }
        }
        catch
        {
        }

        return liveValue;
    }

    private static string ExtractLabelSuffix(string binding, string liveValue)
    {
        Span<char> buf = stackalloc char[binding.Length];
        int pos = 0;
        for (int i = 0; i < binding.Length; i++)
        {
            char c = binding[i];
            if (c == '+' || c == '\'') continue;
            buf[pos++] = c;
        }
        var cleaned = buf[..pos].Trim();
        int idx = cleaned.IndexOf("value".AsSpan(), StringComparison.Ordinal);
        if (idx >= 0)
        {
            var before = cleaned[..idx];
            var after = cleaned[(idx + 5)..];
            before.CopyTo(buf);
            after.CopyTo(buf[before.Length..]);
            cleaned = buf[..(before.Length + after.Length)];
        }
        Span<char> result = stackalloc char[cleaned.Length];
        int wp = 0;
        bool lastSpace = false;
        for (int i = 0; i < cleaned.Length; i++)
        {
            if (cleaned[i] == ' ') { if (!lastSpace) { result[wp++] = ' '; lastSpace = true; } }
            else { result[wp++] = cleaned[i]; lastSpace = false; }
        }
        return new string(result[..wp].Trim());
    }

    private string? EvalSimpleCondition(string binding, string liveValue, double? numVal)
    {
        var qIdx = binding.IndexOf('?');
        var cIdx = binding.IndexOf(':', qIdx + 1);
        if (qIdx < 0 || cIdx < 0)
            return null;

        var bindSpan = binding.AsSpan();
        var condSpan = bindSpan[..qIdx].Trim();
        var trueSpan = TrimQuotes(bindSpan[(qIdx + 1)..cIdx].Trim());
        var falseSpan = TrimQuotes(bindSpan[(cIdx + 1)..].Trim());

        bool result = EvalSimpleBool(condSpan.ToString(), liveValue, numVal);
        return result ? trueSpan.ToString() : falseSpan.ToString();
    }

    private static ReadOnlySpan<char> TrimQuotes(ReadOnlySpan<char> span)
    {
        if (span.Length >= 2)
        {
            char first = span[0];
            if ((first == '\'' || first == '"' || first == ' ') && span[^1] == first)
                span = span[1..^1];
        }
        return span.Trim();
    }

    private bool EvalSimpleBool(string condition, string liveValue, double? numVal)
    {
        var span = condition.AsSpan().Trim();
        int opIdx = -1;
        int opLen = 0;
        for (int i = 0; i < span.Length - 1; i++)
        {
            char c = span[i];
            char c1 = span[i + 1];
            if (c == '=' && c1 == '=') { opIdx = i; opLen = 2; break; }
            if (c == '!' && c1 == '=') { opIdx = i; opLen = 2; break; }
            if (c == '>' && c1 == '=') { opIdx = i; opLen = 2; break; }
            if (c == '<' && c1 == '=') { opIdx = i; opLen = 2; break; }
            if (opIdx < 0 && (c == '>' || c == '<')) { opIdx = i; opLen = 1; }
        }
        if (opIdx < 0) return true;

        var rightSpan = span[(opIdx + opLen)..].Trim();
        if (rightSpan.Length >= 2 && (rightSpan[0] == '\'' || rightSpan[0] == '"') && rightSpan[^1] == rightSpan[0])
            rightSpan = rightSpan[1..^1];

        if (opLen == 2 && span[opIdx] == '=')
        {
            if (rightSpan.Equals("true", StringComparison.OrdinalIgnoreCase))
                return liveValue.Equals("True", StringComparison.OrdinalIgnoreCase) || liveValue == "1";
            if (rightSpan.Equals("false", StringComparison.OrdinalIgnoreCase))
                return liveValue.Equals("False", StringComparison.OrdinalIgnoreCase) || liveValue == "0";
            if (numVal.HasValue && double.TryParse(rightSpan, NumberStyles.Any, CultureInfo.InvariantCulture, out var rn))
                return Math.Abs(numVal.Value - rn) < 0.0001;
            return liveValue.AsSpan().SequenceEqual(rightSpan);
        }

        if (!numVal.HasValue) return true;
        if (!double.TryParse(rightSpan, NumberStyles.Any, CultureInfo.InvariantCulture, out var threshold))
            return true;

        double val = numVal.Value;
        if (opLen == 2)
        {
            return span[opIdx] switch
            {
                '!' => Math.Abs(val - threshold) > 0.0001,
                '>' => val >= threshold,
                '<' => val <= threshold,
                _ => true
            };
        }
        return span[opIdx] == '>' ? val > threshold : val < threshold;
    }

    private static double? ParseNumericValue(string? val)
    {
        if (val == null)
            return null;
        if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            return d;
        if (bool.TryParse(val, out var b))
            return b ? 1 : 0;
        return null;
    }

    private string? ResolveImageData(string? imageId)
    {
        if (string.IsNullOrEmpty(imageId))
            return null;
        return _imageLookup.TryGetValue(imageId, out var data) ? data : null;
    }
}