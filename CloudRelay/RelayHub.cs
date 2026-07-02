// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Microsoft.AspNetCore.SignalR;
using SharedModels.CloudRelay;

namespace CloudRelay;

/// <summary>
/// SignalR hub that relays messages between CloudBridge instances and RuntimeViewer clients.
/// Bridge connections join the "Bridges" group; viewer connections join "Viewers".
/// </summary>
public class RelayHub : Hub
{
    // ─── Connection lifecycle ────────────────────────────────

    public async Task JoinAsBridge()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Bridges");
        await Clients.Group("Viewers").SendAsync(HubMethods.BridgeStatusChanged, true);
    }

    public async Task JoinAsViewer()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Viewers");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // If a bridge disconnects, notify all viewers
        // (We don't track who is bridge vs viewer — viewers are resilient to this event)
        await Clients.Group("Viewers").SendAsync(HubMethods.BridgeStatusChanged, false);
        await base.OnDisconnectedAsync(exception);
    }

    // ─── Bridge → Viewers ────────────────────────────────────

    /// <summary>Bridge pushes updated tag values to all viewers.</summary>
    public async Task ValuesUpdated(List<TagValueDto> values)
    {
        await Clients.Group("Viewers").SendAsync(HubMethods.ValuesUpdated, values);
    }

    /// <summary>Bridge pushes alarm list to all viewers.</summary>
    public async Task AlarmsUpdated(List<AlarmEntryDto> alarms)
    {
        await Clients.Group("Viewers").SendAsync(HubMethods.AlarmsUpdated, alarms);
    }

    /// <summary>Bridge returns write result to all viewers (viewer matches by correlationId).</summary>
    public async Task WriteResult(WriteResultDto result)
    {
        await Clients.Group("Viewers").SendAsync(HubMethods.WriteResult, result);
    }

    // ─── Viewers → Bridge ────────────────────────────────────

    /// <summary>Viewer requests the bridge to monitor specific variable paths.</summary>
    public async Task Subscribe(List<string> variablePaths, int namespaceIndex)
    {
        await Clients.Group("Bridges").SendAsync(HubMethods.Subscribe, variablePaths, namespaceIndex);
    }

    /// <summary>Viewer requests a value write, routed to the bridge.</summary>
    public async Task WriteValue(WriteRequestDto request)
    {
        await Clients.Group("Bridges").SendAsync(HubMethods.WriteValue, request);
    }

    /// <summary>Viewer requests alarm acknowledgement.</summary>
    public async Task AcknowledgeAlarm(AlarmActionDto action)
    {
        await Clients.Group("Bridges").SendAsync(HubMethods.AcknowledgeAlarm, action);
    }

    /// <summary>Viewer requests alarm confirmation/reset.</summary>
    public async Task ConfirmAlarm(AlarmActionDto action)
    {
        await Clients.Group("Bridges").SendAsync(HubMethods.ConfirmAlarm, action);
    }

    /// <summary>Viewer requests a fresh alarm list from the bridge.</summary>
    public async Task RequestAlarms()
    {
        await Clients.Group("Bridges").SendAsync(HubMethods.RequestAlarms);
    }
}
