import os

path = os.path.join(r"C:\Users\cfior\source\repos", "ServerEditorWeb", "Components", "Editor", "NodeProperties.razor")

with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

print(f"Read {len(content)} chars")
changes = 0

# The scheduler sections use \r\n line endings
nl = '\r\n'

# ─── 1. Scheduler Activate: fix NavigateScreen handler ───
old1 = (
    f'                else if (cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Screen name" @bind="cmd.TargetScreen" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action is "ExecuteScript" or "ExecuteJavaScript"){nl}'
    f'                {{{nl}'
    f'                    <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="@(cmd.Action == "ExecuteJavaScript" ? "JavaScript code" : "C# script")" @bind="cmd.Script" @bind:after="OnChanged"></textarea>{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action is "AcknowledgeAllAlarms" or "ResetAllAlarms"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Comment (optional)" @bind="cmd.Value" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action == "ChangeLanguage"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Language code (e.g. en, de, it)" @bind="cmd.Value" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action == "GenerateReport"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'            </div>{nl}'
    f'        }}{nl}'
    f'        <button class="btn btn-sm btn-outline-primary" @onclick="() => AddSchedulerCommand(schedulerNode, false)">'
)

new1 = (
    f'                else if (cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal"){nl}'
    f'                {{{nl}'
    f'                    <ScreenPicker Value="@cmd.TargetScreen"{nl}'
    f'                                  ValueChanged=\'v => {{ cmd.TargetScreen = v ?? string.Empty; }}\'{nl}'
    f'                                  OnChanged="OnChanged"{nl}'
    f'                                  Placeholder="(select screen)" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action is "ExecuteScript" or "ExecuteJavaScript"){nl}'
    f'                {{{nl}'
    f'                    <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="@(cmd.Action == "ExecuteJavaScript" ? "JavaScript code" : "C# script")" @bind="cmd.Script" @bind:after="OnChanged"></textarea>{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action is "AcknowledgeAllAlarms" or "ResetAllAlarms"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Comment (optional)" @bind="cmd.Value" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action == "ChangeLanguage"){nl}'
    f'                {{{nl}'
    f'                    <LocalePicker Value="@cmd.TargetLocale"{nl}'
    f'                                  ValueChanged=\'v => {{ cmd.TargetLocale = v ?? string.Empty; }}\'{nl}'
    f'                                  OnChanged="OnChanged"{nl}'
    f'                                  Placeholder="(select language)" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action == "GenerateReport"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'            </div>{nl}'
    f'        }}{nl}'
    f'        <button class="btn btn-sm btn-outline-primary" @onclick="() => AddSchedulerCommand(schedulerNode, false)">'
)

if old1 in content:
    content = content.replace(old1, new1, 1)
    changes += 1
    print("1. Fixed Scheduler Activate: ScreenPicker + LocalePicker")
else:
    print("1. SKIP Scheduler Activate (anchor not found)")

# ─── 2. Scheduler Deactivate: same fix ───
old2 = (
    f'                else if (cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Screen name" @bind="cmd.TargetScreen" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action is "ExecuteScript" or "ExecuteJavaScript"){nl}'
    f'                {{{nl}'
    f'                    <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="@(cmd.Action == "ExecuteJavaScript" ? "JavaScript code" : "C# script")" @bind="cmd.Script" @bind:after="OnChanged"></textarea>{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action is "AcknowledgeAllAlarms" or "ResetAllAlarms"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Comment (optional)" @bind="cmd.Value" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action == "ChangeLanguage"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Language code (e.g. en, de, it)" @bind="cmd.Value" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action == "GenerateReport"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'            </div>{nl}'
    f'        }}{nl}'
    f'        <button class="btn btn-sm btn-outline-primary" @onclick="() => AddSchedulerCommand(schedulerNode, true)">'
)

new2 = (
    f'                else if (cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal"){nl}'
    f'                {{{nl}'
    f'                    <ScreenPicker Value="@cmd.TargetScreen"{nl}'
    f'                                  ValueChanged=\'v => {{ cmd.TargetScreen = v ?? string.Empty; }}\'{nl}'
    f'                                  OnChanged="OnChanged"{nl}'
    f'                                  Placeholder="(select screen)" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action is "ExecuteScript" or "ExecuteJavaScript"){nl}'
    f'                {{{nl}'
    f'                    <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="@(cmd.Action == "ExecuteJavaScript" ? "JavaScript code" : "C# script")" @bind="cmd.Script" @bind:after="OnChanged"></textarea>{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action is "AcknowledgeAllAlarms" or "ResetAllAlarms"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Comment (optional)" @bind="cmd.Value" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action == "ChangeLanguage"){nl}'
    f'                {{{nl}'
    f'                    <LocalePicker Value="@cmd.TargetLocale"{nl}'
    f'                                  ValueChanged=\'v => {{ cmd.TargetLocale = v ?? string.Empty; }}\'{nl}'
    f'                                  OnChanged="OnChanged"{nl}'
    f'                                  Placeholder="(select language)" />{nl}'
    f'                }}{nl}'
    f'                else if (cmd.Action == "GenerateReport"){nl}'
    f'                {{{nl}'
    f'                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />{nl}'
    f'                }}{nl}'
    f'            </div>{nl}'
    f'        }}{nl}'
    f'        <button class="btn btn-sm btn-outline-primary" @onclick="() => AddSchedulerCommand(schedulerNode, true)">'
)

if old2 in content:
    content = content.replace(old2, new2, 1)
    changes += 1
    print("2. Fixed Scheduler Deactivate: ScreenPicker + LocalePicker")
else:
    print("2. SKIP Scheduler Deactivate (anchor not found)")

with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(content)

print(f"\nWritten {len(content)} chars ({changes} changes) - Done")
