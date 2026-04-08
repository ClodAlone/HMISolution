import os

path = os.path.join(r"C:\Users\cfior\source\repos", "ServerEditorWeb", "Components", "Editor", "ScreenSymbolProperties.razor")

with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

print(f"Read {len(content)} chars")
nl = '\r\n'
changes = 0

# ─── 1. Replace screen dropdown with ScreenPicker ───
old_screen = (
    f'                @if (c.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal"){nl}'
    f'                {{{nl}'
    f'                    <div class="mb-1">{nl}'
    f'                        <label class="form-label mb-0" style="font-size: 10px;">Target Screen</label>{nl}'
    f'                        <select class="form-select form-select-sm" style="font-size: 10px;"{nl}'
    f'                                @bind="c.TargetScreen" @bind:after="OnChanged">{nl}'
    f'                            <option value="">(select)</option>{nl}'
    f'                            @foreach (var scr in GetScreenNames()){nl}'
    f'                            {{{nl}'
    f'                                <option value="@scr">@scr</option>{nl}'
    f'                            }}{nl}'
    f'                        </select>{nl}'
    f'                    </div>{nl}'
    f'                }}'
)

new_screen = (
    f'                @if (c.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal"){nl}'
    f'                {{{nl}'
    f'                    <div class="mb-1">{nl}'
    f'                        <label class="form-label mb-0" style="font-size: 10px;">Target Screen</label>{nl}'
    f'                        <ScreenPicker Value="@c.TargetScreen"{nl}'
    f'                                      ValueChanged=\'v => {{ c.TargetScreen = v ?? string.Empty; }}\'{nl}'
    f'                                      OnChanged="OnChanged"{nl}'
    f'                                      Placeholder="(select screen)" />{nl}'
    f'                    </div>{nl}'
    f'                }}'
)

if old_screen in content:
    content = content.replace(old_screen, new_screen, 1)
    changes += 1
    print("1. Replaced screen dropdown with ScreenPicker")
else:
    print("1. SKIP screen dropdown (anchor not found)")
    idx = content.find('@foreach (var scr in GetScreenNames())')
    if idx >= 0:
        print(f"   GetScreenNames found at pos {idx}")
        snippet = content[max(0,idx-200):idx+200]
        print(f"   Context: {repr(snippet[:200])}")

# ─── 2. Replace ChangeLanguage text input with LocalePicker ───
old_lang = (
    f'                @if (c.Action is "ChangeLanguage"){nl}'
    f'                {{{nl}'
    f'                    <div class="mb-1">{nl}'
    f'                        <label class="form-label mb-0" style="font-size: 10px;">Language Code</label>{nl}'
    f'                        <input type="text" class="form-control form-control-sm" style="font-size: 10px;"{nl}'
    f'                               placeholder="en, de, fr, es, it, ja\u2026"{nl}'
    f'                               @bind="c.Value" @bind:after="OnChanged" />{nl}'
    f'                        <span class="text-muted" style="font-size: 9px;">{nl}'
    f'                            Set active language at runtime. Labels prefixed with @@ are resolved from the String Table.{nl}'
    f'                        </span>{nl}'
    f'                    </div>{nl}'
    f'                }}'
)

new_lang = (
    f'                @if (c.Action is "ChangeLanguage"){nl}'
    f'                {{{nl}'
    f'                    <div class="mb-1">{nl}'
    f'                        <label class="form-label mb-0" style="font-size: 10px;">Language</label>{nl}'
    f'                        <LocalePicker Value="@c.TargetLocale"{nl}'
    f'                                      ValueChanged=\'v => {{ c.TargetLocale = v ?? string.Empty; }}\'{nl}'
    f'                                      OnChanged="OnChanged"{nl}'
    f'                                      Placeholder="(select language)" />{nl}'
    f'                        <span class="text-muted" style="font-size: 9px;">{nl}'
    f'                            Set active language at runtime. Labels prefixed with @@ are resolved from the String Table.{nl}'
    f'                        </span>{nl}'
    f'                    </div>{nl}'
    f'                }}'
)

if old_lang in content:
    content = content.replace(old_lang, new_lang, 1)
    changes += 1
    print("2. Replaced ChangeLanguage input with LocalePicker")
else:
    print("2. SKIP ChangeLanguage (anchor not found)")

with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(content)

print(f"\nWritten {len(content)} chars ({changes} changes) - Done")
