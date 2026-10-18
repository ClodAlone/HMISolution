// Copyright (c) 2026 Claudio Fiorani - All rights reserved.

using System.Globalization;
using System.Security;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ServerEditorWeb.Services;

/// <summary>Shared resource-bundle construction for the industrial simulation examples.</summary>
internal sealed class DemoScreenBuilder(string folder, string screenName, string scriptName)
{
    private readonly JsonArray _variables = [];
    private readonly JsonArray _symbols = [];
    public string Path => "__TASK__." + folder + ".";

    public void Variable(string name, string type, object value, string unit = "", string description = "")
        => _variables.Add(new JsonObject
        {
            ["Name"] = name, ["Type"] = type, ["Access"] = "ReadWrite", ["Value"] = JsonSerializer.SerializeToNode(value),
            ["InitialValue"] = value is bool flag ? (flag ? "true" : "false") : Convert.ToString(value, CultureInfo.InvariantCulture),
            ["EngineeringUnit"] = unit, ["Description"] = description
        });

    public void Symbol(string id, string type, double x, double y, double width, double height, JsonObject properties)
    {
        properties["Id"] = folder.ToLowerInvariant() + "_" + id; properties["Type"] = type;
        properties["X"] = x; properties["Y"] = y; properties["Width"] = width; properties["Height"] = height;
        properties["Stroke"] = "none"; properties["StrokeWidth"] = 0;
        _symbols.Add(properties);
    }

    public void Art(string id, double x, double y, double width, double height, string svg, string? variable = null, string? visible = null)
    {
        var props = new JsonObject { ["SvgContent"] = svg };
        if (variable != null) props["VariablePath"] = Path + variable;
        if (visible != null) props["VisibilityBinding"] = visible;
        Symbol(id, "svg", x, y, width, height, props);
    }

    public void Pipe(string id, double x, double y, double width, double height, string? variable, string color, string direction = "left-right", string? visible = null)
    {
        var props = new JsonObject
        {
            ["PipeOrientation"] = width >= height ? "horizontal" : "vertical", ["PipeUseGradient"] = true,
            ["PipeColor"] = "#425e74", ["PipeFlowColor"] = color, ["PipeShowFlow"] = variable != null,
            ["PipeAnimate"] = true, ["PipeFlowSpeed"] = 0.8, ["PipeFlowDirection"] = direction
        };
        if (variable != null) props["VariablePath"] = Path + variable;
        if (visible != null) props["VisibilityBinding"] = visible;
        Symbol(id, "pipe", x, y, width, height, props);
    }

    public void Display(string id, double x, double y, double width, double height, string variable, string unit = "", string color = "#93c5fd", string format = "F1")
        => Symbol(id, "numericdisplay", x, y, width, height, new JsonObject
        {
            ["VariablePath"] = Path + variable, ["DisplayBackgroundStyle"] = "midnight", ["DisplayFontStyle"] = "terminal", ["DisplayAutoFit"] = true,
            ["Fill"] = color, ["NumericDisplayUnit"] = unit, ["NumericDisplayFormat"] = format, ["FontSize"] = 24
        });

    public void Text(string id, double x, double y, double width, string label, string color = "#7db3cf", double size = 13)
        => Symbol(id, "text", x, y, width, 28, new JsonObject { ["Label"] = label, ["Fill"] = color, ["FontSize"] = size, ["FontFamily"] = "Segoe UI" });

    public void Button(string id, double x, double y, double width, string label, string variable, string value = "", string action = "WriteVariable", string color = "#31597a")
        => Symbol(id, "button", x, y, width, 42, new JsonObject
        {
            ["Label"] = label, ["ButtonStyle"] = "raised", ["ButtonColor"] = color, ["ButtonTextColor"] = "#f1f5f9", ["ButtonBorderRadius"] = 9, ["FontSize"] = 14,
            ["Commands"] = new JsonArray(new JsonObject { ["Id"] = folder.ToLowerInvariant() + "_cmd_" + id, ["Action"] = action, ["Trigger"] = "Click", ["VariablePath"] = Path + variable, ["Value"] = value })
        });

    public void Input(string id, double x, double y, double width, string variable, double min, double max, double step, string format = "F1")
        => Symbol(id, "editbox", x, y, width, 52, new JsonObject
        {
            ["VariablePath"] = Path + variable, ["MinValue"] = min, ["MaxValue"] = max, ["EditBoxStep"] = step, ["EditBoxFormat"] = format,
            ["DisplayBackgroundStyle"] = "ivory", ["DisplayFontStyle"] = "terminal", ["DisplayAutoFit"] = true, ["DisplayTextColor"] = "#16324c"
        });

    public void Header(string title, string subtitle, params string[] metrics)
    {
        var cards = string.Join("", metrics.Select((m, i) => $"<rect x=\"{44 + i * 344}\" y=\"106\" width=\"318\" height=\"96\" rx=\"16\" fill=\"#13263a\" stroke=\"#355069\"/><text x=\"{62 + i * 344}\" y=\"130\" font-size=\"12\" fill=\"#7dd3fc\">{SecurityElement.Escape(m)}</text>"));
        Art("header", 0, 0, 1440, 1040, $"<svg viewBox=\"0 0 1440 1040\"><g font-family=\"Segoe UI,Arial,sans-serif\" fill=\"#e2e8f0\"><text x=\"44\" y=\"50\" font-size=\"30\" font-weight=\"700\" letter-spacing=\"2\">{SecurityElement.Escape(title)}</text><text x=\"44\" y=\"79\" font-size=\"13\" fill=\"#7db3cf\">{SecurityElement.Escape(subtitle)}</text>{cards}<path d=\"M0 224H1440M44 790H1396\" stroke=\"#1d3045\"/><text x=\"44\" y=\"1015\" font-size=\"12\" fill=\"#7db3cf\">LOCAL EDUCATIONAL SIMULATION / NO EXTERNAL DEVICES / Start-Pause freezes simulated time / Reset pauses</text></g></svg>");
    }

    public void StandardControls()
    {
        Button("run", 44, 820, 180, "Start / Pause", "RunRequested", action: "ToggleVariable", color: "#0f766e");
        Button("reset", 238, 820, 130, "Reset", "ResetRequested", "true", color: "#475569");
        Display("status", 44, 925, 1352, 58, "Status", color: "#e2e8f0", format: "");
    }

    public string Build(string code)
    {
        var bundle = new JsonObject
        {
            ["Folder"] = new JsonObject { ["Name"] = "__TASK__", ["Variables"] = new JsonArray(), ["Folders"] = new JsonArray(new JsonObject { ["Name"] = folder, ["Variables"] = _variables, ["Folders"] = new JsonArray() }) },
            ["Scripts"] = new JsonArray(new JsonObject { ["Name"] = "__TASK___" + scriptName, ["Language"] = "CSharp", ["IntervalMs"] = 250, ["Enabled"] = false, ["Code"] = code }),
            ["Screens"] = new JsonArray(new JsonObject { ["Name"] = "__TASK___" + screenName, ["Width"] = 1440, ["Height"] = 1040, ["Background"] = "#080f1e", ["LayoutMode"] = "svg", ["FitMode"] = "contain", ["ShowInNavigation"] = true, ["NavigationTransition"] = "fade", ["NavigationDurationMs"] = 320, ["Symbols"] = _symbols })
        };
        return bundle.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }
}
