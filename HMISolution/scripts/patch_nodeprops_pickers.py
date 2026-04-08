"""
Patch NodeProperties.razor:
- Fix Scheduler command editors: replace text inputs for screen/language with ScreenPicker/LocalePicker
- Fix Event command editors: add missing actions (Navigate, Language, Report, GenerateReport) + use pickers
"""
import os

path = os.path.join(r"C:\Users\cfior\source\repos", "ServerEditorWeb", "Components", "Editor", "NodeProperties.razor")

with open(path, 'rb') as f:
    content = f.read().decode('utf-8')

original_len = len(content)
print(f"Read {original_len} chars")

changes = 0

# ─── 1. Scheduler Activate Commands: screen input → ScreenPicker ───
old_sched_act_screen = '''                else if (cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Screen name" @bind="cmd.TargetScreen" @bind:after="OnChanged" />
                }
                else if (cmd.Action is "ExecuteScript" or "ExecuteJavaScript")
                {
                    <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="@(cmd.Action == "ExecuteJavaScript" ? "JavaScript code" : "C# script")" @bind="cmd.Script" @bind:after="OnChanged"></textarea>
                }
                else if (cmd.Action is "AcknowledgeAllAlarms" or "ResetAllAlarms")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Comment (optional)" @bind="cmd.Value" @bind:after="OnChanged" />
                }
                else if (cmd.Action == "ChangeLanguage")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Language code (e.g. en, de, it)" @bind="cmd.Value" @bind:after="OnChanged" />
                }
                else if (cmd.Action == "GenerateReport")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />
                }
            </div>
        }
        <button class="btn btn-sm btn-outline-primary" @onclick="() => AddSchedulerCommand(schedulerNode, false)">'''

new_sched_act_screen = '''                else if (cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal")
                {
                    <ScreenPicker Value="@cmd.TargetScreen"
                                  ValueChanged='v => { cmd.TargetScreen = v ?? string.Empty; }'
                                  OnChanged="OnChanged"
                                  Placeholder="(select screen)" />
                }
                else if (cmd.Action is "ExecuteScript" or "ExecuteJavaScript")
                {
                    <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="@(cmd.Action == "ExecuteJavaScript" ? "JavaScript code" : "C# script")" @bind="cmd.Script" @bind:after="OnChanged"></textarea>
                }
                else if (cmd.Action is "AcknowledgeAllAlarms" or "ResetAllAlarms")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Comment (optional)" @bind="cmd.Value" @bind:after="OnChanged" />
                }
                else if (cmd.Action == "ChangeLanguage")
                {
                    <LocalePicker Value="@cmd.TargetLocale"
                                  ValueChanged='v => { cmd.TargetLocale = v ?? string.Empty; }'
                                  OnChanged="OnChanged"
                                  Placeholder="(select language)" />
                }
                else if (cmd.Action == "GenerateReport")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />
                }
            </div>
        }
        <button class="btn btn-sm btn-outline-primary" @onclick="() => AddSchedulerCommand(schedulerNode, false)">'''

if old_sched_act_screen in content:
    content = content.replace(old_sched_act_screen, new_sched_act_screen, 1)
    changes += 1
    print("1. Fixed Scheduler Activate: ScreenPicker + LocalePicker")
else:
    print("1. SKIP Scheduler Activate (anchor not found)")

# ─── 2. Scheduler Deactivate Commands: same fix ───
old_sched_deact_screen = '''                else if (cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Screen name" @bind="cmd.TargetScreen" @bind:after="OnChanged" />
                }
                else if (cmd.Action is "ExecuteScript" or "ExecuteJavaScript")
                {
                    <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="@(cmd.Action == "ExecuteJavaScript" ? "JavaScript code" : "C# script")" @bind="cmd.Script" @bind:after="OnChanged"></textarea>
                }
                else if (cmd.Action is "AcknowledgeAllAlarms" or "ResetAllAlarms")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Comment (optional)" @bind="cmd.Value" @bind:after="OnChanged" />
                }
                else if (cmd.Action == "ChangeLanguage")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Language code (e.g. en, de, it)" @bind="cmd.Value" @bind:after="OnChanged" />
                }
                else if (cmd.Action == "GenerateReport")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />
                }
            </div>
        }
        <button class="btn btn-sm btn-outline-primary" @onclick="() => AddSchedulerCommand(schedulerNode, true)">'''

new_sched_deact_screen = '''                else if (cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal")
                {
                    <ScreenPicker Value="@cmd.TargetScreen"
                                  ValueChanged='v => { cmd.TargetScreen = v ?? string.Empty; }'
                                  OnChanged="OnChanged"
                                  Placeholder="(select screen)" />
                }
                else if (cmd.Action is "ExecuteScript" or "ExecuteJavaScript")
                {
                    <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="@(cmd.Action == "ExecuteJavaScript" ? "JavaScript code" : "C# script")" @bind="cmd.Script" @bind:after="OnChanged"></textarea>
                }
                else if (cmd.Action is "AcknowledgeAllAlarms" or "ResetAllAlarms")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Comment (optional)" @bind="cmd.Value" @bind:after="OnChanged" />
                }
                else if (cmd.Action == "ChangeLanguage")
                {
                    <LocalePicker Value="@cmd.TargetLocale"
                                  ValueChanged='v => { cmd.TargetLocale = v ?? string.Empty; }'
                                  OnChanged="OnChanged"
                                  Placeholder="(select language)" />
                }
                else if (cmd.Action == "GenerateReport")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />
                }
            </div>
        }
        <button class="btn btn-sm btn-outline-primary" @onclick="() => AddSchedulerCommand(schedulerNode, true)">'''

if old_sched_deact_screen in content:
    content = content.replace(old_sched_deact_screen, new_sched_deact_screen, 1)
    changes += 1
    print("2. Fixed Scheduler Deactivate: ScreenPicker + LocalePicker")
else:
    print("2. SKIP Scheduler Deactivate (anchor not found)")

# ─── 3. Event Commands: replace entire action select + handlers ───
# The event commands only have Variables + Scripting + Alarms. Need to add Navigation, Auth, Language, Report.
old_event_cmd = '''                <select class="form-select form-select-sm" style="font-size: 10px;" @bind="cmd.Action" @bind:after="OnChanged">
                    <optgroup label="Variables">
                        <option value="SetVariable">Set Variable</option>
                        <option value="WriteVariable">Write Variable</option>
                        <option value="ResetVariable">Reset Variable</option>
                        <option value="ToggleVariable">Toggle Variable</option>
                        <option value="IncrementVariable">Increment Variable</option>
                        <option value="DecrementVariable">Decrement Variable</option>
                    </optgroup>
                    <optgroup label="Scripting">
                        <option value="ExecuteScript">Execute Script</option>
                    </optgroup>
                    <optgroup label="Alarms">
                        <option value="AcknowledgeAllAlarms">Acknowledge All Alarms</option>
                        <option value="ResetAllAlarms">Reset All Alarms</option>
                    </optgroup>
                </select>
                <button class="btn btn-sm btn-outline-danger py-0 px-1" style="font-size: 9px;" @onclick="() => { eventNode.Event.Commands.RemoveAt(idx); OnChanged(); }">'''

new_event_cmd = '''                <select class="form-select form-select-sm" style="font-size: 10px;" @bind="cmd.Action" @bind:after="OnChanged">
                    <optgroup label="Navigation">
                        <option value="NavigateScreen">Navigate Screen</option>
                        <option value="OpenScreenPopup">Open Screen Popup</option>
                        <option value="OpenScreenModal">Open Screen Modal</option>
                    </optgroup>
                    <optgroup label="Variables">
                        <option value="SetVariable">Set Variable</option>
                        <option value="WriteVariable">Write Variable</option>
                        <option value="ResetVariable">Reset Variable</option>
                        <option value="ToggleVariable">Toggle Variable</option>
                        <option value="IncrementVariable">Increment Variable</option>
                        <option value="DecrementVariable">Decrement Variable</option>
                    </optgroup>
                    <optgroup label="Scripting">
                        <option value="ExecuteScript">Execute Script</option>
                        <option value="ExecuteJavaScript">Execute JavaScript</option>
                    </optgroup>
                    <optgroup label="Authentication">
                        <option value="Login">Login</option>
                        <option value="Logout">Logout</option>
                    </optgroup>
                    <optgroup label="Alarms">
                        <option value="AcknowledgeAllAlarms">Acknowledge All Alarms</option>
                        <option value="ResetAllAlarms">Reset All Alarms</option>
                    </optgroup>
                    <optgroup label="Other">
                        <option value="ChangeLanguage">Change Language</option>
                        <option value="GenerateReport">Generate Report</option>
                    </optgroup>
                </select>
                <button class="btn btn-sm btn-outline-danger py-0 px-1" style="font-size: 9px;" @onclick="() => { eventNode.Event.Commands.RemoveAt(idx); OnChanged(); }">'''

count = content.count(old_event_cmd)
if count >= 1:
    content = content.replace(old_event_cmd, new_event_cmd, 1)
    changes += 1
    print(f"3. Fixed Event Commands action select (found {count} occurrence(s), replaced 1st)")
else:
    print("3. SKIP Event Commands select (anchor not found)")

# ─── 4. Event Commands: add screen/language/report handlers after ExecuteScript handler ───
old_event_handlers = '''            else if (cmd.Action is "ExecuteScript")
            {
                <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="C# script" @bind="cmd.Script" @bind:after="OnChanged"></textarea>
            }
        </div>
    }
    <button class="btn btn-sm btn-outline-primary" @onclick="() => AddEventCommand(eventNode, false)">'''

new_event_handlers = '''            else if (cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal")
            {
                <ScreenPicker Value="@cmd.TargetScreen"
                              ValueChanged='v => { cmd.TargetScreen = v ?? string.Empty; }'
                              OnChanged="OnChanged"
                              Placeholder="(select screen)" />
            }
            else if (cmd.Action is "ExecuteScript" or "ExecuteJavaScript")
            {
                <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="@(cmd.Action == "ExecuteJavaScript" ? "JavaScript code" : "C# script")" @bind="cmd.Script" @bind:after="OnChanged"></textarea>
            }
            else if (cmd.Action is "AcknowledgeAllAlarms" or "ResetAllAlarms")
            {
                <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Comment (optional)" @bind="cmd.Value" @bind:after="OnChanged" />
            }
            else if (cmd.Action == "ChangeLanguage")
            {
                <LocalePicker Value="@cmd.TargetLocale"
                              ValueChanged='v => { cmd.TargetLocale = v ?? string.Empty; }'
                              OnChanged="OnChanged"
                              Placeholder="(select language)" />
            }
            else if (cmd.Action == "GenerateReport")
            {
                <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />
            }
        </div>
    }
    <button class="btn btn-sm btn-outline-primary" @onclick="() => AddEventCommand(eventNode, false)">'''

if old_event_handlers in content:
    content = content.replace(old_event_handlers, new_event_handlers, 1)
    changes += 1
    print("4. Fixed Event Commands: added screen/language/report handlers")
else:
    print("4. SKIP Event Commands handlers (anchor not found)")

# ─── 5. Event ClearCommands: replace action select ───
old_event_clear_cmd = '''                <select class="form-select form-select-sm" style="font-size: 10px;" @bind="cmd.Action" @bind:after="OnChanged">
                    <optgroup label="Variables">
                        <option value="SetVariable">Set Variable</option>
                        <option value="WriteVariable">Write Variable</option>
                        <option value="ResetVariable">Reset Variable</option>
                        <option value="ToggleVariable">Toggle Variable</option>
                        <option value="IncrementVariable">Increment Variable</option>
                        <option value="DecrementVariable">Decrement Variable</option>
                    </optgroup>
                    <optgroup label="Scripting">
                        <option value="ExecuteScript">Execute Script</option>
                    </optgroup>
                </select>
                <button class="btn btn-sm btn-outline-danger py-0 px-1" style="font-size: 9px;" @onclick="() => { eventNode.Event.ClearCommands.RemoveAt(idx); OnChanged(); }">'''

new_event_clear_cmd = '''                <select class="form-select form-select-sm" style="font-size: 10px;" @bind="cmd.Action" @bind:after="OnChanged">
                    <optgroup label="Navigation">
                        <option value="NavigateScreen">Navigate Screen</option>
                        <option value="OpenScreenPopup">Open Screen Popup</option>
                        <option value="OpenScreenModal">Open Screen Modal</option>
                    </optgroup>
                    <optgroup label="Variables">
                        <option value="SetVariable">Set Variable</option>
                        <option value="WriteVariable">Write Variable</option>
                        <option value="ResetVariable">Reset Variable</option>
                        <option value="ToggleVariable">Toggle Variable</option>
                        <option value="IncrementVariable">Increment Variable</option>
                        <option value="DecrementVariable">Decrement Variable</option>
                    </optgroup>
                    <optgroup label="Scripting">
                        <option value="ExecuteScript">Execute Script</option>
                        <option value="ExecuteJavaScript">Execute JavaScript</option>
                    </optgroup>
                    <optgroup label="Authentication">
                        <option value="Login">Login</option>
                        <option value="Logout">Logout</option>
                    </optgroup>
                    <optgroup label="Alarms">
                        <option value="AcknowledgeAllAlarms">Acknowledge All Alarms</option>
                        <option value="ResetAllAlarms">Reset All Alarms</option>
                    </optgroup>
                    <optgroup label="Other">
                        <option value="ChangeLanguage">Change Language</option>
                        <option value="GenerateReport">Generate Report</option>
                    </optgroup>
                </select>
                <button class="btn btn-sm btn-outline-danger py-0 px-1" style="font-size: 9px;" @onclick="() => { eventNode.Event.ClearCommands.RemoveAt(idx); OnChanged(); }">'''

if old_event_clear_cmd in content:
    content = content.replace(old_event_clear_cmd, new_event_clear_cmd, 1)
    changes += 1
    print("5. Fixed Event ClearCommands action select")
else:
    print("5. SKIP Event ClearCommands select (anchor not found)")

# ─── 6. Event ClearCommands: add screen/language/report handlers ───
old_event_clear_handlers = '''            else if (cmd.Action is "ExecuteScript")
            {
                <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="C# script" @bind="cmd.Script" @bind:after="OnChanged"></textarea>
            }
        </div>
    }
    <button class="btn btn-sm btn-outline-primary" @onclick="() => AddEventCommand(eventNode, true)">'''

new_event_clear_handlers = '''            else if (cmd.Action is "NavigateScreen" or "OpenScreenPopup" or "OpenScreenModal")
            {
                <ScreenPicker Value="@cmd.TargetScreen"
                              ValueChanged='v => { cmd.TargetScreen = v ?? string.Empty; }'
                              OnChanged="OnChanged"
                              Placeholder="(select screen)" />
            }
            else if (cmd.Action is "ExecuteScript" or "ExecuteJavaScript")
            {
                <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="@(cmd.Action == "ExecuteJavaScript" ? "JavaScript code" : "C# script")" @bind="cmd.Script" @bind:after="OnChanged"></textarea>
            }
            else if (cmd.Action is "AcknowledgeAllAlarms" or "ResetAllAlarms")
            {
                <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Comment (optional)" @bind="cmd.Value" @bind:after="OnChanged" />
            }
            else if (cmd.Action == "ChangeLanguage")
            {
                <LocalePicker Value="@cmd.TargetLocale"
                              ValueChanged='v => { cmd.TargetLocale = v ?? string.Empty; }'
                              OnChanged="OnChanged"
                              Placeholder="(select language)" />
            }
            else if (cmd.Action == "GenerateReport")
            {
                <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Report name" @bind="cmd.TargetReport" @bind:after="OnChanged" />
            }
        </div>
    }
    <button class="btn btn-sm btn-outline-primary" @onclick="() => AddEventCommand(eventNode, true)">'''

if old_event_clear_handlers in content:
    content = content.replace(old_event_clear_handlers, new_event_clear_handlers, 1)
    changes += 1
    print("6. Fixed Event ClearCommands: added screen/language/report handlers")
else:
    print("6. SKIP Event ClearCommands handlers (anchor not found)")

with open(path, 'w', encoding='utf-8', newline='') as f:
    f.write(content)

print(f"\nWritten {len(content)} chars ({changes} changes) - Done")
