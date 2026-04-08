import os

# ─── 1. Add API endpoint to ServerEditorWeb/Program.cs ───
path = os.path.join(r"C:\Users\cfior\source\repos", "ServerEditorWeb", "Program.cs")
with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

print(f"Read Program.cs: {len(content)} chars")

anchor = 'app.MapRazorComponents<App>()\r\n    .AddInteractiveServerRenderMode();'
if anchor in content and 'api/has-unsaved-changes' not in content:
    replacement = anchor + '''

// Lightweight API for Desktop shell to query unsaved state
app.MapGet("/api/has-unsaved-changes", (ServerEditorWeb.Services.NodeEditorService editor) =>
    Results.Json(new { hasUnsavedChanges = editor.AnyUnsavedChanges }));'''
    content = content.replace(anchor, replacement, 1)
    with open(path, 'w', encoding='utf-8', newline='') as f:
        f.write(content)
    print("1. Added /api/has-unsaved-changes endpoint to Program.cs")
else:
    print("1. SKIP Program.cs (already exists or anchor not found)")

# ─── 2. Patch Desktop Program.cs with close handler ───
path2 = os.path.join(r"C:\Users\cfior\source\repos", "ServerEditorWeb.Desktop", "Program.cs")
with open(path2, 'rb') as f:
    content2 = f.read().decode('utf-8')

print(f"Read Desktop Program.cs: {len(content2)} chars")

# Detect line ending
nl = '\r\n' if '\r\n' in content2 else '\n'

# Replace the window creation + WaitForClose section
old_window = (
    '            // Open a Photino window pointing to the local web server' + nl +
    '            _window = new PhotinoWindow()' + nl +
    '                .SetTitle("HMI Editor")' + nl +
    '                .SetUseOsDefaultSize(false)' + nl +
    '                .SetSize(1440, 900)' + nl +
    '                .SetMaximized(true)' + nl +
    '#if DEBUG' + nl +
    '                .SetDevToolsEnabled(true)' + nl +
    '#endif' + nl +
    '                .Load(new Uri(url));' + nl +
    '' + nl +
    '            AppDomain.CurrentDomain.UnhandledException += (_, e) =>' + nl +
    '            {' + nl +
    '                if (e.ExceptionObject is Exception ex)' + nl +
    '                {' + nl +
    '                    SharedModels.CrashReporter.Report(ex, "AppDomain");' + nl +
    '                    Console.Error.WriteLine($"Fatal: {ex}");' + nl +
    '                }' + nl +
    '            };' + nl +
    '' + nl +
    '            _window.WaitForClose();'
)

new_window = (
    '            // Open a Photino window pointing to the local web server' + nl +
    '            _window = new PhotinoWindow()' + nl +
    '                .SetTitle("HMI Editor")' + nl +
    '                .SetUseOsDefaultSize(false)' + nl +
    '                .SetSize(1440, 900)' + nl +
    '                .SetMaximized(true)' + nl +
    '#if DEBUG' + nl +
    '                .SetDevToolsEnabled(true)' + nl +
    '#endif' + nl +
    '                .RegisterWindowClosingHandler(OnWindowClosing)' + nl +
    '                .Load(new Uri(url));' + nl +
    '' + nl +
    '            _serverUrl = url;' + nl +
    '' + nl +
    '            AppDomain.CurrentDomain.UnhandledException += (_, e) =>' + nl +
    '            {' + nl +
    '                if (e.ExceptionObject is Exception ex)' + nl +
    '                {' + nl +
    '                    SharedModels.CrashReporter.Report(ex, "AppDomain");' + nl +
    '                    Console.Error.WriteLine($"Fatal: {ex}");' + nl +
    '                }' + nl +
    '            };' + nl +
    '' + nl +
    '            _window.WaitForClose();'
)

if old_window in content2:
    content2 = content2.replace(old_window, new_window, 1)
    print("2a. Patched Desktop window creation with RegisterWindowClosingHandler")
else:
    print("2a. SKIP window creation (anchor not found)")
    # Debug
    idx = content2.find('.SetMaximized(true)')
    if idx >= 0:
        snippet = content2[idx:idx+300]
        print(f"   Found at {idx}: {repr(snippet[:200])}")

# Add _serverUrl field and OnWindowClosing method + HasUnsavedChanges helper
old_fields = '    private static PhotinoWindow? _window;'
new_fields = (
    '    private static PhotinoWindow? _window;' + nl +
    '    private static string? _serverUrl;'
)

if '_serverUrl' not in content2:
    content2 = content2.replace(old_fields, new_fields, 1)
    print("2b. Added _serverUrl field")

# Add OnWindowClosing and HasUnsavedChanges methods before FindEditorExe
old_find = '    private static string? FindEditorExe()'
new_methods = (
    '    /// <summary>' + nl +
    '    /// Called by Photino before closing the window.' + nl +
    '    /// Returns true to cancel the close, false to allow it.' + nl +
    '    /// </summary>' + nl +
    '    private static bool OnWindowClosing(object sender, EventArgs e)' + nl +
    '    {' + nl +
    '        if (_serverUrl == null) return false;' + nl +
    '' + nl +
    '        try' + nl +
    '        {' + nl +
    '            if (HasUnsavedChanges())' + nl +
    '            {' + nl +
    '                // Ask the user via a JS confirm dialog in the web view' + nl +
    '                _window?.SendWebMessage("CHECK_UNSAVED");' + nl +
    '' + nl +
    '                // Use a simpler synchronous approach: show system dialog' + nl +
    '                // Since Photino does not support synchronous JS eval,' + nl +
    '                // we use a native message box approach via the console prompt' + nl +
    '                var result = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(' + nl +
    '                    System.Runtime.InteropServices.OSPlatform.Windows)' + nl +
    '                    ? NativeMessageBox("HMI Editor",' + nl +
    '                        "You have unsaved changes. Are you sure you want to close without saving?",' + nl +
    '                        0x04 | 0x30) // MB_YESNO | MB_ICONWARNING' + nl +
    '                    : 6; // IDYES on non-Windows (just allow close)' + nl +
    '' + nl +
    '                return result != 6; // 6 = IDYES → allow close; anything else → cancel' + nl +
    '            }' + nl +
    '        }' + nl +
    '        catch' + nl +
    '        {' + nl +
    '            // If we can\'t determine state, allow close' + nl +
    '        }' + nl +
    '        return false;' + nl +
    '    }' + nl +
    '' + nl +
    '    private static bool HasUnsavedChanges()' + nl +
    '    {' + nl +
    '        try' + nl +
    '        {' + nl +
    '            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };' + nl +
    '            var response = http.GetStringAsync($"{_serverUrl}/api/has-unsaved-changes").Result;' + nl +
    '            return response.Contains("true", StringComparison.OrdinalIgnoreCase);' + nl +
    '        }' + nl +
    '        catch { return false; }' + nl +
    '    }' + nl +
    '' + nl +
    '    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]' + nl +
    '    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);' + nl +
    '' + nl +
    '    private static int NativeMessageBox(string title, string message, uint flags)' + nl +
    '    {' + nl +
    '        return MessageBox(IntPtr.Zero, message, title, flags);' + nl +
    '    }' + nl +
    '' + nl +
    '    private static string? FindEditorExe()'
)

if 'OnWindowClosing' not in content2:
    content2 = content2.replace(old_find, new_methods, 1)
    print("2c. Added OnWindowClosing, HasUnsavedChanges, NativeMessageBox methods")

with open(path2, 'w', encoding='utf-8', newline='') as f:
    f.write(content2)

print(f"Written Desktop Program.cs: {len(content2)} chars - Done")
