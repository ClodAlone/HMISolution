import os

path = os.path.join(r"C:\Users\cfior\source\repos", "ServerEditorWeb", "Components", "Editor", "ScreenSymbolProperties.razor")

with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

print(f"Read {len(content)} chars")
changes = 0

# ─── 1. Replace screen dropdown with ScreenPicker ───
old_screen = '''                @if (c.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal")
                {
                    <div class="mb-1">
                        <label class="form-label mb-0" style="font-size: 10px;">Target Screen</label>
                        <select class="form-select form-select-sm" style="font-size: 10px;"
                                @bind="c.TargetScreen" @bind:after="OnChanged">
                            <option value="">(select)</option>
                            @foreach (var scr in GetScreenNames())
                            {
                                <option value="@scr">@scr</option>
                            }
                        </select>
                    </div>
                }'''

new_screen = '''                @if (c.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal")
                {
                    <div class="mb-1">
                        <label class="form-label mb-0" style="font-size: 10px;">Target Screen</label>
                        <ScreenPicker Value="@c.TargetScreen"
                                      ValueChanged='v => { c.TargetScreen = v ?? string.Empty; }'
                                      OnChanged="OnChanged"
                                      Placeholder="(select screen)" />
                    </div>
                }'''

if old_screen in content:
    content = content.replace(old_screen, new_screen, 1)
    changes += 1
    print("1. Replaced screen dropdown with ScreenPicker")
else:
    print("1. SKIP screen dropdown (anchor not found)")

# ─── 2. Replace ChangeLanguage text input with LocalePicker ───
old_lang = '''                @if (c.Action is "ChangeLanguage")
                {
                    <div class="mb-1">
                        <label class="form-label mb-0" style="font-size: 10px;">Language Code</label>
                        <input type="text" class="form-control form-control-sm" style="font-size: 10px;"
                               placeholder="en, de, fr, es, it, ja\u2026"
                               @bind="c.Value" @bind:after="OnChanged" />
                        <span class="text-muted" style="font-size: 9px;">
                            Set active language at runtime. Labels prefixed with @@ are resolved from the String Table.
                        </span>
                    </div>
                }'''

new_lang = '''                @if (c.Action is "ChangeLanguage")
                {
                    <div class="mb-1">
                        <label class="form-label mb-0" style="font-size: 10px;">Language</label>
                        <LocalePicker Value="@c.TargetLocale"
                                      ValueChanged='v => { c.TargetLocale = v ?? string.Empty; }'
                                      OnChanged="OnChanged"
                                      Placeholder="(select language)" />
                        <span class="text-muted" style="font-size: 9px;">
                            Set active language at runtime. Labels prefixed with @@ are resolved from the String Table.
                        </span>
                    </div>
                }'''

if old_lang in content:
    content = content.replace(old_lang, new_lang, 1)
    changes += 1
    print("2. Replaced ChangeLanguage input with LocalePicker")
else:
    print("2. SKIP ChangeLanguage (anchor not found)")
    # Debug
    idx = content.find('c.Action is "ChangeLanguage"')
    if idx >= 0:
        snippet = content[idx:idx+400]
        print(f"   Found at {idx}: {repr(snippet[:200])}")

with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(content)

print(f"\nWritten {len(content)} chars ({changes} changes) - Done")
