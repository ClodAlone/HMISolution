import sys

file_path = "C:\\Users\\cfior\\source\\repos\\ServerEditorWeb\\Components\\Editor\\NodeProperties.razor"

with open(file_path, "rb") as f:
    content = f.read().decode("utf-8")

print(f"Read {len(content)} chars")

# 1. Add EventNode property panel before AssetNode section
marker = "else if (Node is AssetNode assetNode)"
idx = content.find(marker)
if idx < 0:
    print("ERROR: AssetNode section not found")
    sys.exit(1)

event_panel = '''else if (Node is EventNode eventNode)
{
 <div class="properties-panel">
    <h5>\U0001F514 Event</h5>
    <div class="mb-3">
        <label class="form-label">Name</label>
        <input type="text" class="form-control" @bind="eventNode.Name" @bind:after="OnChanged"
               @onfocus="() => CaptureForUndo(eventNode, nameof(eventNode.Name))" />
    </div>
    <div class="mb-2 form-check">
        <input class="form-check-input" type="checkbox" @bind="eventNode.Event.Enabled" @bind:after="OnChanged" />
        <label class="form-check-label">Enabled</label>
    </div>
    <div class="mb-3">
        <label class="form-label">Description</label>
        <input type="text" class="form-control" @bind="eventNode.Event.Description" @bind:after="OnChanged" placeholder="Optional notes..." />
    </div>

    <hr />
    <h6>\U0001F50D Condition</h6>
    <div class="mb-3">
        <label class="form-label">Variable Path</label>
        <VariablePathPicker Value="@eventNode.Event.ConditionVariablePath"
                            ValueChanged='v => { eventNode.Event.ConditionVariablePath = v ?? string.Empty; }'
                            OnChanged="OnChanged"
                            Placeholder="e.g. Sensors.Temperature.Value" />
        <div class="form-text">OPC variable whose value is evaluated as the trigger condition.</div>
    </div>
    <div class="row g-2 mb-3">
        <div class="col-6">
            <label class="form-label">Operator</label>
            <select class="form-select" @bind="eventNode.Event.ConditionOperator" @bind:after="OnChanged">
                <option value="True">True (truthy / non-zero)</option>
                <option value="False">False (falsy / zero)</option>
                <option value="==">== (equals)</option>
                <option value="!=">!= (not equal)</option>
                <option value=">">&gt; (greater than)</option>
                <option value="<">&lt; (less than)</option>
                <option value=">=">&gt;= (greater or equal)</option>
                <option value="<=">&lt;= (less or equal)</option>
            </select>
        </div>
        <div class="col-6">
            <label class="form-label">Value</label>
            <input type="text" class="form-control" @bind="eventNode.Event.ConditionValue" @bind:after="OnChanged"
                   placeholder="Comparison value"
                   disabled="@(eventNode.Event.ConditionOperator is "True" or "False")" />
        </div>
    </div>

    <hr />
    <h6>\u2699\uFE0F Trigger</h6>
    <div class="row g-2 mb-3">
        <div class="col-6">
            <label class="form-label">Trigger Mode</label>
            <select class="form-select" @bind="eventNode.Event.TriggerMode" @bind:after="OnChanged">
                <option value="RisingEdge">Rising Edge (false \u2192 true)</option>
                <option value="FallingEdge">Falling Edge (true \u2192 false)</option>
                <option value="Continuous">Continuous (while true)</option>
            </select>
        </div>
        <div class="col-6">
            <label class="form-label">Hold Time (seconds)</label>
            <input type="number" class="form-control" min="0" @bind="eventNode.Event.HoldTimeSeconds" @bind:after="OnChanged" />
            <div class="form-text">Condition must stay true for this duration before firing. 0 = immediate.</div>
        </div>
    </div>

    <hr />
    <h6>\u25B6 Commands (@eventNode.Event.Commands.Count)</h6>
    <p class="text-muted small">Executed when the event fires.</p>
    @for (int i = 0; i < eventNode.Event.Commands.Count; i++)
    {
        var idx = i;
        var cmd = eventNode.Event.Commands[idx];
        <div class="border rounded p-2 mb-2" style="font-size: 11px;">
            <div class="d-flex gap-1 mb-1">
                <select class="form-select form-select-sm" style="font-size: 10px;" @bind="cmd.Action" @bind:after="OnChanged">
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
                <button class="btn btn-sm btn-outline-danger py-0 px-1" style="font-size: 9px;" @onclick="() => { eventNode.Event.Commands.RemoveAt(idx); OnChanged(); }">\U0001F5D1</button>
            </div>
            @if (cmd.Action is "SetVariable" or "WriteVariable" or "ToggleVariable" or "IncrementVariable" or "DecrementVariable")
            {
                <VariablePathPicker Value="@cmd.VariablePath"
                                    ValueChanged="v => { cmd.VariablePath = v ?? string.Empty; }"
                                    OnChanged="OnChanged"
                                    Placeholder="Variable path" />
                @if (cmd.Action is not "ToggleVariable")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="@(cmd.Action is "IncrementVariable" or "DecrementVariable" ? "Step (default 1)" : "Value")" @bind="cmd.Value" @bind:after="OnChanged" />
                }
            }
            else if (cmd.Action == "ResetVariable")
            {
                <VariablePathPicker Value="@cmd.VariablePath"
                                    ValueChanged="v => { cmd.VariablePath = v ?? string.Empty; }"
                                    OnChanged="OnChanged"
                                    Placeholder="Variable path" />
            }
            else if (cmd.Action is "ExecuteScript")
            {
                <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="C# script" @bind="cmd.Script" @bind:after="OnChanged"></textarea>
            }
        </div>
    }
    <button class="btn btn-sm btn-outline-primary" @onclick="() => AddEventCommand(eventNode, false)">\u2795 Add Command</button>

    <hr />
    <h6>\u23F9 Clear Commands (@eventNode.Event.ClearCommands.Count)</h6>
    <p class="text-muted small">Optional commands executed when the condition clears (edge resets).</p>
    @for (int i = 0; i < eventNode.Event.ClearCommands.Count; i++)
    {
        var idx = i;
        var cmd = eventNode.Event.ClearCommands[idx];
        <div class="border rounded p-2 mb-2" style="font-size: 11px;">
            <div class="d-flex gap-1 mb-1">
                <select class="form-select form-select-sm" style="font-size: 10px;" @bind="cmd.Action" @bind:after="OnChanged">
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
                <button class="btn btn-sm btn-outline-danger py-0 px-1" style="font-size: 9px;" @onclick="() => { eventNode.Event.ClearCommands.RemoveAt(idx); OnChanged(); }">\U0001F5D1</button>
            </div>
            @if (cmd.Action is "SetVariable" or "WriteVariable" or "ToggleVariable" or "IncrementVariable" or "DecrementVariable")
            {
                <VariablePathPicker Value="@cmd.VariablePath"
                                    ValueChanged="v => { cmd.VariablePath = v ?? string.Empty; }"
                                    OnChanged="OnChanged"
                                    Placeholder="Variable path" />
                @if (cmd.Action is not "ToggleVariable")
                {
                    <input type="text" class="form-control form-control-sm" style="font-size: 10px;" placeholder="Value" @bind="cmd.Value" @bind:after="OnChanged" />
                }
            }
            else if (cmd.Action == "ResetVariable")
            {
                <VariablePathPicker Value="@cmd.VariablePath"
                                    ValueChanged="v => { cmd.VariablePath = v ?? string.Empty; }"
                                    OnChanged="OnChanged"
                                    Placeholder="Variable path" />
            }
            else if (cmd.Action is "ExecuteScript")
            {
                <textarea class="form-control form-control-sm" style="font-size: 10px;" rows="3" placeholder="C# script" @bind="cmd.Script" @bind:after="OnChanged"></textarea>
            }
        </div>
    }
    <button class="btn btn-sm btn-outline-primary" @onclick="() => AddEventCommand(eventNode, true)">\u2795 Add Clear Command</button>

    <p class="text-muted small mt-3">
        At runtime the server publishes <code>_Events.@(eventNode.Event.Name).Active</code> (Boolean) and
        <code>.LastFired</code> (DateTime) as OPC variables.
    </p>
 </div>
}
'''

content = content[:idx] + event_panel + content[idx:]
print("1. Added EventNode property panel")

# 2. Add AddEventCommand helper method after AddSchedulerCommand
marker2 = "    private async Task AddSchedulerCommand(SchedulerNode node, bool deactivate)"
idx2 = content.find(marker2)
if idx2 < 0:
    print("ERROR: AddSchedulerCommand not found")
    sys.exit(1)

# Find end of method
brace = 0
i = idx2
started = False
while i < len(content):
    if content[i] == '{':
        brace += 1
        started = True
    elif content[i] == '}':
        brace -= 1
        if started and brace == 0:
            break
    i += 1
end_method = i + 1

# Find end of line after method
while end_method < len(content) and content[end_method] in ('\r', '\n'):
    end_method += 1

add_event_cmd = """    private async Task AddEventCommand(EventNode node, bool clear)
    {
        var cmd = new SharedModels.SymbolCommand
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Action = "SetVariable"
        };
        if (clear)
            node.Event.ClearCommands.Add(cmd);
        else
            node.Event.Commands.Add(cmd);
        await OnChanged();
    }
"""

content = content[:end_method] + add_event_cmd + content[end_method:]
print("2. Added AddEventCommand helper method")

with open(file_path, "w", encoding="utf-8", newline="") as f:
    f.write(content)

print(f"Written {len(content)} chars - Done")
