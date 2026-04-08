"""
Create ScreenPicker.razor and LocalePicker.razor components.
These are simpler picker components - a select dropdown with available options.
"""
import os

BASE = r"C:\Users\cfior\source\repos"
COMP_DIR = os.path.join(BASE, "ServerEditorWeb", "Components", "Editor")

# =========================================================
# ScreenPicker.razor
# =========================================================
screen_picker = r'''@using SharedModels
@using ServerEditorWeb.Services

<div class="d-flex gap-1 align-items-center">
    <select class="form-select form-select-sm" style="font-size: 10px;"
            value="@Value" @onchange="OnSelect">
        <option value="">@Placeholder</option>
        @foreach (var name in GetScreenNames())
        {
            <option value="@name">@name</option>
        }
    </select>
</div>

@code {
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public EventCallback OnChanged { get; set; }
    [Parameter] public string Placeholder { get; set; } = "(select screen)";

    [Inject] private NodeEditorService Editor { get; set; } = default!;

    private List<string> GetScreenNames()
    {
        return Editor.RootModel?.Screens?.Select(s => s.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];
    }

    private async Task OnSelect(ChangeEventArgs e)
    {
        var val = e.Value?.ToString();
        Value = val;
        await ValueChanged.InvokeAsync(val);
        await OnChanged.InvokeAsync();
    }
}
'''

path = os.path.join(COMP_DIR, "ScreenPicker.razor")
with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(screen_picker)
print(f"Created ScreenPicker.razor ({len(screen_picker)} chars)")

# =========================================================
# LocalePicker.razor
# =========================================================
locale_picker = r'''@using SharedModels
@using ServerEditorWeb.Services

<div class="d-flex gap-1 align-items-center">
    <select class="form-select form-select-sm" style="font-size: 10px;"
            value="@Value" @onchange="OnSelect">
        <option value="">@Placeholder</option>
        @foreach (var lang in GetAvailableLanguages())
        {
            <option value="@lang">@lang</option>
        }
    </select>
</div>

@code {
    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public EventCallback OnChanged { get; set; }
    [Parameter] public string Placeholder { get; set; } = "(select language)";

    [Inject] private NodeEditorService Editor { get; set; } = default!;

    private List<string> GetAvailableLanguages()
    {
        var strings = Editor.RootModel?.Strings;
        if (strings == null || strings.Count == 0)
            return [];

        // Collect all language codes from the string table translations
        var langs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in strings)
        {
            foreach (var key in entry.Translations.Keys)
            {
                langs.Add(key);
            }
        }

        return langs.OrderBy(l => l, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private async Task OnSelect(ChangeEventArgs e)
    {
        var val = e.Value?.ToString();
        Value = val;
        await ValueChanged.InvokeAsync(val);
        await OnChanged.InvokeAsync();
    }
}
'''

path = os.path.join(COMP_DIR, "LocalePicker.razor")
with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(locale_picker)
print(f"Created LocalePicker.razor ({len(locale_picker)} chars)")
