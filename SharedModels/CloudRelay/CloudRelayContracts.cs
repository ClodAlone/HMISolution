using System;
using System.Collections.Generic;

namespace SharedModels.CloudRelay;

/// <summary>
/// Hub method name constants shared by CloudRelay, CloudBridge, and CloudRuntimeClient.
/// </summary>
public static class HubMethods
{
    // Bridge → Hub → Viewers
    public const string ValuesUpdated = "ValuesUpdated";
    public const string AlarmsUpdated = "AlarmsUpdated";
    public const string WriteResult = "WriteResult";
    public const string BridgeStatusChanged = "BridgeStatusChanged";

    // Viewer → Hub → Bridge
    public const string Subscribe = "Subscribe";
    public const string Unsubscribe = "Unsubscribe";
    public const string WriteValue = "WriteValue";
    public const string AcknowledgeAlarm = "AcknowledgeAlarm";
    public const string ConfirmAlarm = "ConfirmAlarm";
    public const string RequestAlarms = "RequestAlarms";
}

/// <summary>A single tag value update.</summary>
public class TagValueDto
{
    public string Path { get; set; } = "";
    public string Value { get; set; } = "";
}

/// <summary>An alarm entry transported through the cloud relay.</summary>
public class AlarmEntryDto
{
    public string Id { get; set; } = "";
    /// <summary>OPC UA ConditionId serialized as string (NamespaceIndex:Identifier).</summary>
    public string ConditionId { get; set; } = "";
    /// <summary>EventId as Base64 for safe JSON transport.</summary>
    public string EventIdBase64 { get; set; } = "";
    public string SourceName { get; set; } = "";
    public string Message { get; set; } = "";
    public string Severity { get; set; } = "";
    public DateTime Time { get; set; }
    public bool IsAcked { get; set; }
    public bool IsConfirmed { get; set; }
}

/// <summary>Write request from a viewer routed through the hub to the bridge.</summary>
public class WriteRequestDto
{
    public string CorrelationId { get; set; } = "";
    public string VariablePath { get; set; } = "";
    public string Value { get; set; } = "";
    public int NamespaceIndex { get; set; } = 2;
}

/// <summary>Write result returned from the bridge via the hub back to the viewer.</summary>
public class WriteResultDto
{
    public string CorrelationId { get; set; } = "";
    public bool Success { get; set; }
}

/// <summary>Alarm acknowledge/confirm request routed through the hub.</summary>
public class AlarmActionDto
{
    public string ConditionId { get; set; } = "";
    public string EventIdBase64 { get; set; } = "";
    public string Comment { get; set; } = "";
}
