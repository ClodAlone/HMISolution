using SharedModels;
using Microsoft.JSInterop;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Executes SymbolCommands at runtime by interacting with OPC, navigation, auth, and scripts.
/// </summary>
public class CommandService
{
    private readonly OpcRuntimeClient _opc;
    private readonly IJSRuntime _js;

    /// <summary>Raised when a NavigateScreen command fires.</summary>
    public event Action<string>? NavigateScreen;

    /// <summary>Raised when an OpenScreenPopup command fires.</summary>
    public event Action<string>? OpenScreenPopup;

    /// <summary>Raised when an OpenScreenModal command fires.</summary>
    public event Action<string>? OpenScreenModal;

    /// <summary>Raised when a Login command fires to show the login prompt.</summary>
    public event Action? RequestLogin;

    /// <summary>Raised when a Logout command fires.</summary>
    public event Action? RequestLogout;

    /// <summary>Raised when a ChangeLanguage command fires.</summary>
    public event Action<string>? ChangeLanguage;

    public CommandService(OpcRuntimeClient opc, IJSRuntime js)
    {
        _opc = opc;
        _js = js;
    }

    /// <summary>Execute a command. Returns immediately for fire-and-forget actions.</summary>
    public async Task ExecuteAsync(SymbolCommand cmd)
    {
        switch (cmd.Action)
        {
            case "NavigateScreen":
                if (!string.IsNullOrEmpty(cmd.TargetScreen))
                    NavigateScreen?.Invoke(cmd.TargetScreen);
                break;

            case "OpenScreenPopup":
                if (!string.IsNullOrEmpty(cmd.TargetScreen))
                    OpenScreenPopup?.Invoke(cmd.TargetScreen);
                break;

            case "OpenScreenModal":
                if (!string.IsNullOrEmpty(cmd.TargetScreen))
                    OpenScreenModal?.Invoke(cmd.TargetScreen);
                break;

            case "SetVariable":
                if (!string.IsNullOrEmpty(cmd.VariablePath))
                    await _opc.WriteValueAsync(cmd.VariablePath, cmd.Value);
                break;

            case "ResetVariable":
                if (!string.IsNullOrEmpty(cmd.VariablePath))
                    await _opc.WriteValueAsync(cmd.VariablePath, "0");
                break;

            case "ToggleVariable":
                if (!string.IsNullOrEmpty(cmd.VariablePath))
                {
                    var current = _opc.GetValue(cmd.VariablePath);
                    var isTruthy = current is "True" or "true" or "1";
                    await _opc.WriteValueAsync(cmd.VariablePath, isTruthy ? "false" : "true");
                }
                break;

            case "IncrementVariable":
                await AdjustVariable(cmd.VariablePath, cmd.Value, +1);
                break;

            case "DecrementVariable":
                await AdjustVariable(cmd.VariablePath, cmd.Value, -1);
                break;

            case "WriteVariable":
                if (!string.IsNullOrEmpty(cmd.VariablePath) && !string.IsNullOrEmpty(cmd.Value))
                    await _opc.WriteValueAsync(cmd.VariablePath, cmd.Value);
                break;

            case "ExecuteJavaScript":
                if (!string.IsNullOrEmpty(cmd.Script))
                    await ExecuteJavaScriptAsync(cmd.Script);
                break;

            case "ExecuteScript":
                if (!string.IsNullOrEmpty(cmd.Script))
                    await ExecuteScriptAsync(cmd.Script);
                break;

            case "Login":
                RequestLogin?.Invoke();
                break;

            case "Logout":
                RequestLogout?.Invoke();
                break;

            case "AcknowledgeAllAlarms":
                await _opc.AcknowledgeAllAlarmsAsync(
                    string.IsNullOrEmpty(cmd.Value) ? "Acknowledged from RuntimeViewer" : cmd.Value);
                break;

            case "ResetAllAlarms":
                await _opc.ResetAllAlarmsAsync(
                    string.IsNullOrEmpty(cmd.Value) ? "Confirmed from RuntimeViewer" : cmd.Value);
                break;

            case "ChangeLanguage":
                if (!string.IsNullOrEmpty(cmd.Value))
                    ChangeLanguage?.Invoke(cmd.Value);
                break;
        }
    }

    /// <summary>Execute the release action for a WhilePressed command.</summary>
    public async Task ExecuteReleaseAsync(SymbolCommand cmd)
    {
        if (string.IsNullOrEmpty(cmd.VariablePath)) return;

        if (!string.IsNullOrEmpty(cmd.ReleaseValue))
        {
            await _opc.WriteValueAsync(cmd.VariablePath, cmd.ReleaseValue);
        }
        else
        {
            // Default: reset to 0/false
            await _opc.WriteValueAsync(cmd.VariablePath, "0");
        }
    }

    private async Task AdjustVariable(string variablePath, string stepStr, int direction)
    {
        if (string.IsNullOrEmpty(variablePath)) return;

        var step = 1.0;
        if (!string.IsNullOrEmpty(stepStr))
            double.TryParse(stepStr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out step);

        var current = _opc.GetValue(variablePath);
        var currentVal = 0.0;
        if (current != null)
            double.TryParse(current, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out currentVal);

        var newVal = currentVal + step * direction;
        await _opc.WriteValueAsync(variablePath,
            newVal.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    /// <summary>Execute raw JavaScript in the browser context with access to the SVG DOM.</summary>
    private async Task ExecuteJavaScriptAsync(string script)
    {
        try
        {
            // Wrap in a function to provide a clean scope
            // The script has access to the full browser DOM including SVG elements
            await _js.InvokeVoidAsync("eval", script);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JavaScript execution error: {ex.Message}");
        }
    }

    /// <summary>
    /// Execute a C# script with access to OPC read/write functions and SVG DOM via JS.
    /// The script is a simple line-by-line DSL:
    ///   read(path) — reads a variable value
    ///   write(path, value) — writes a variable value
    ///   js(code) — executes JavaScript
    ///   set(element, attr, value) — sets an SVG element attribute via JS
    ///   log(message) — logs to the browser console
    /// </summary>
    private async Task ExecuteScriptAsync(string script)
    {
        try
        {
            var lines = script.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith("//") || line.StartsWith("#")) continue;

                if (line.StartsWith("read(") && line.EndsWith(")"))
                {
                    // read(path) — no-op standalone, but useful in combined expressions
                    continue;
                }
                else if (line.StartsWith("write(") && line.EndsWith(")"))
                {
                    var args = ParseArgs(line, "write");
                    if (args.Length >= 2)
                        await _opc.WriteValueAsync(args[0], args[1]);
                }
                else if (line.StartsWith("toggle(") && line.EndsWith(")"))
                {
                    var args = ParseArgs(line, "toggle");
                    if (args.Length >= 1)
                    {
                        var cur = _opc.GetValue(args[0]);
                        var truthy = cur is "True" or "true" or "1";
                        await _opc.WriteValueAsync(args[0], truthy ? "false" : "true");
                    }
                }
                else if (line.StartsWith("js(") && line.EndsWith(")"))
                {
                    var jsCode = line[3..^1].Trim().Trim('"', '\'');
                    await _js.InvokeVoidAsync("eval", jsCode);
                }
                else if (line.StartsWith("set(") && line.EndsWith(")"))
                {
                    var args = ParseArgs(line, "set");
                    if (args.Length >= 3)
                    {
                        // set(selector, attribute, value) — set SVG element attribute
                        var jsCode = $"document.querySelector('{EscapeJs(args[0])}')?.setAttribute('{EscapeJs(args[1])}', '{EscapeJs(args[2])}')";
                        await _js.InvokeVoidAsync("eval", jsCode);
                    }
                }
                else if (line.StartsWith("css(") && line.EndsWith(")"))
                {
                    var args = ParseArgs(line, "css");
                    if (args.Length >= 3)
                    {
                        // css(selector, property, value)
                        var jsCode = $"document.querySelector('{EscapeJs(args[0])}').style['{EscapeJs(args[1])}'] = '{EscapeJs(args[2])}'";
                        await _js.InvokeVoidAsync("eval", jsCode);
                    }
                }
                else if (line.StartsWith("log(") && line.EndsWith(")"))
                {
                    var msg = line[4..^1].Trim().Trim('"', '\'');
                    await _js.InvokeVoidAsync("eval", $"console.log('{EscapeJs(msg)}')");
                }
                else if (line.StartsWith("alert(") && line.EndsWith(")"))
                {
                    var msg = line[6..^1].Trim().Trim('"', '\'');
                    await _js.InvokeVoidAsync("eval", $"alert('{EscapeJs(msg)}')");
                }
                else if (line.StartsWith("delay(") && line.EndsWith(")"))
                {
                    var ms = line[6..^1].Trim();
                    if (int.TryParse(ms, out var delayMs) && delayMs is > 0 and <= 10000)
                        await Task.Delay(delayMs);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Script execution error: {ex.Message}");
        }
    }

    private static string[] ParseArgs(string line, string funcName)
    {
        var inner = line[(funcName.Length + 1)..^1];
        // Simple comma-separated args, respecting quoted strings
        var args = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuote = false;
        char quoteChar = '"';

        foreach (var ch in inner)
        {
            if (!inQuote && ch is '"' or '\'')
            {
                inQuote = true;
                quoteChar = ch;
            }
            else if (inQuote && ch == quoteChar)
            {
                inQuote = false;
            }
            else if (!inQuote && ch == ',')
            {
                args.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(ch);
            }
        }

        if (current.Length > 0)
            args.Add(current.ToString().Trim());

        return args.ToArray();
    }

    private static string EscapeJs(string s) =>
        s.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
}

