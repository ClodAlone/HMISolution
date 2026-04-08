import os

path = os.path.join(r"C:\Users\cfior\source\repos", "ServerEditorWeb.Desktop", "Program.cs")
with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

print(f"Read {len(content)} chars")
nl = '\r\n'
changes = 0

# 1. Add _serverUrl field declaration after _window field
if 'private static string? _serverUrl;' not in content:
    old = '    private static PhotinoWindow? _window;'
    new = old + nl + '    private static string? _serverUrl;'
    if old in content:
        content = content.replace(old, new, 1)
        changes += 1
        print("1. Added _serverUrl field")
    else:
        print("1. SKIP _serverUrl field (anchor not found)")
else:
    print("1. SKIP _serverUrl field (already declared)")

# 2. Add method definitions before FindEditorExe
if 'private static bool OnWindowClosing(' not in content:
    old_find = '    private static string? FindEditorExe()'
    new_block = (
        f'    /// <summary>{nl}'
        f'    /// Called by Photino before closing the window.{nl}'
        f'    /// Returns true to cancel the close, false to allow it.{nl}'
        f'    /// </summary>{nl}'
        f'    private static bool OnWindowClosing(object sender, EventArgs e){nl}'
        f'    {{{nl}'
        f'        if (_serverUrl == null) return false;{nl}'
        f'{nl}'
        f'        try{nl}'
        f'        {{{nl}'
        f'            if (HasUnsavedChanges()){nl}'
        f'            {{{nl}'
        f'                // Show native Yes/No dialog asking the user{nl}'
        f'                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform({nl}'
        f'                    System.Runtime.InteropServices.OSPlatform.Windows)){nl}'
        f'                {{{nl}'
        f'                    var result = NativeMessageBox("HMI Editor",{nl}'
        f'                        "You have unsaved changes. Are you sure you want to close without saving?",{nl}'
        f'                        0x04 | 0x30); // MB_YESNO | MB_ICONWARNING{nl}'
        f'                    return result != 6; // 6 = IDYES = allow close{nl}'
        f'                }}{nl}'
        f'                // Non-Windows: allow close (no native dialog available){nl}'
        f'            }}{nl}'
        f'        }}{nl}'
        f'        catch{nl}'
        f'        {{{nl}'
        f'            // If we cannot determine state, allow close{nl}'
        f'        }}{nl}'
        f'        return false;{nl}'
        f'    }}{nl}'
        f'{nl}'
        f'    private static bool HasUnsavedChanges(){nl}'
        f'    {{{nl}'
        f'        try{nl}'
        f'        {{{nl}'
        f'            using var http = new HttpClient {{ Timeout = TimeSpan.FromSeconds(2) }};{nl}'
        f'            var response = http.GetStringAsync($"{{_serverUrl}}/api/has-unsaved-changes").Result;{nl}'
        f'            return response.Contains("true", StringComparison.OrdinalIgnoreCase);{nl}'
        f'        }}{nl}'
        f'        catch {{ return false; }}{nl}'
        f'    }}{nl}'
        f'{nl}'
        f'    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]{nl}'
        f'    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);{nl}'
        f'{nl}'
        f'    private static int NativeMessageBox(string title, string message, uint flags){nl}'
        f'    {{{nl}'
        f'        return MessageBox(IntPtr.Zero, message, title, flags);{nl}'
        f'    }}{nl}'
        f'{nl}'
        f'    private static string? FindEditorExe()'
    )
    if old_find in content:
        content = content.replace(old_find, new_block, 1)
        changes += 1
        print("2. Added OnWindowClosing, HasUnsavedChanges, NativeMessageBox methods")
    else:
        print("2. SKIP methods (FindEditorExe anchor not found)")
else:
    print("2. SKIP methods (already defined)")

with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(content)

print(f"Written {len(content)} chars ({changes} changes) - Done")
