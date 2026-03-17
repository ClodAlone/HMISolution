using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR.Client;
using SharedModels.CloudRelay;

namespace RuntimeViewer.Shared.Services;

/// <summary>
/// Alternative runtime client that connects to the cloud relay hub instead of
/// directly to OPC UA. Used when CloudRelay is enabled in the project settings.
/// Both the viewer and the bridge make outbound connections to the cloud —
/// no firewall changes are required on either side.
/// </summary>
public sealed class CloudRuntimeClient : IDisposable
{
    private HubConnection? _hub;
    private readonly Dictionary<string, string> _values = new();
    private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingWrites = new();
    private readonly object _lock = new();
    private List<AlarmEntry> _lastAlarms = new();

    public bool IsConnected => _hub?.State == HubConnectionState.Connected;
    public bool BridgeOnline { get; private set; }
    public string? ErrorMessage { get; private set; }

    public event Action? ValuesChanged;
    public event Action? StateChanged;
    public event Action<List<AlarmEntry>>? AlarmsReceived;

    // ─── Diagnostic log ─────────────────────────────────
    private readonly List<string> _diagnosticLog = new();
    private const int MaxLogEntries = 200;

    public IReadOnlyList<string> DiagnosticLog
    {
        get { lock (_lock) { return _diagnosticLog.ToList(); } }
    }

    private void Log(string message)
    {
        var entry = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
        lock (_lock)
        {
            _diagnosticLog.Add(entry);
            if (_diagnosticLog.Count > MaxLogEntries)
                _diagnosticLog.RemoveAt(0);
        }
        StateChanged?.Invoke();
    }

    public string? GetValue(string variablePath)
    {
        lock (_lock)
        {
            return _values.TryGetValue(variablePath, out var v) ? v : null;
        }
    }

    public async Task ConnectAsync(string hubUrl, string apiKey = "")
    {
        try
        {
            Log($"CLOUD: Connecting to relay hub {hubUrl}");
            Disconnect();

            var url = string.IsNullOrEmpty(apiKey)
                ? hubUrl
                : $"{hubUrl}?apiKey={Uri.EscapeDataString(apiKey)}";

            _hub = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();

            // Register incoming message handlers
            _hub.On<List<TagValueDto>>(HubMethods.ValuesUpdated, OnValuesUpdated);
            _hub.On<List<AlarmEntryDto>>(HubMethods.AlarmsUpdated, OnAlarmsUpdated);
            _hub.On<WriteResultDto>(HubMethods.WriteResult, OnWriteResult);
            _hub.On<bool>(HubMethods.BridgeStatusChanged, OnBridgeStatusChanged);

            _hub.Reconnected += async _ =>
            {
                Log("CLOUD: Reconnected to hub — re-joining as viewer");
                await _hub.SendAsync("JoinAsViewer");
            };

            _hub.Closed += _ =>
            {
                Log("CLOUD: Hub connection closed");
                ErrorMessage = "Cloud relay connection lost";
                StateChanged?.Invoke();
                return Task.CompletedTask;
            };

            await _hub.StartAsync();
            await _hub.SendAsync("JoinAsViewer");

            ErrorMessage = null;
            BridgeOnline = true;
            Log("CLOUD: ✓ Connected to relay hub");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Cloud relay: {ex.Message}";
            Log($"CLOUD: ✗ FAILED — {ex.GetType().Name}: {ex.Message}");
        }
    }

    /// <summary>Request the bridge to monitor specific OPC variables.</summary>
    public async Task MonitorVariablesAsync(IEnumerable<string> variablePaths, ushort namespaceIndex = 2)
    {
        if (_hub?.State != HubConnectionState.Connected) return;

        var paths = variablePaths.Where(p => !string.IsNullOrEmpty(p)).ToList();
        Log($"CLOUD: Subscribing to {paths.Count} variable(s)");
        await _hub.SendAsync(HubMethods.Subscribe, paths, (int)namespaceIndex);
    }

    /// <summary>Write a value through the cloud bridge.</summary>
    public async Task<bool> WriteValueAsync(string variablePath, string value, ushort namespaceIndex = 2)
    {
        if (_hub?.State != HubConnectionState.Connected) return false;

        var correlationId = Guid.NewGuid().ToString("N");
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pendingWrites[correlationId] = tcs;

        try
        {
            await _hub.SendAsync(HubMethods.WriteValue, new WriteRequestDto
            {
                CorrelationId = correlationId,
                VariablePath = variablePath,
                Value = value,
                NamespaceIndex = namespaceIndex
            });

            // Wait with timeout for the bridge to respond
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            cts.Token.Register(() => tcs.TrySetResult(false));
            return await tcs.Task;
        }
        finally
        {
            _pendingWrites.TryRemove(correlationId, out _);
        }
    }

    /// <summary>Acknowledge an alarm through the cloud bridge.</summary>
    public async Task<bool> AcknowledgeAlarmAsync(string conditionId, string eventIdBase64, string comment = "Acknowledged")
    {
        if (_hub?.State != HubConnectionState.Connected) return false;
        await _hub.SendAsync(HubMethods.AcknowledgeAlarm, new AlarmActionDto
        {
            ConditionId = conditionId,
            EventIdBase64 = eventIdBase64,
            Comment = comment
        });
        return true;
    }

    /// <summary>Confirm/reset an alarm through the cloud bridge.</summary>
    public async Task<bool> ConfirmAlarmAsync(string conditionId, string eventIdBase64, string comment = "Confirmed")
    {
        if (_hub?.State != HubConnectionState.Connected) return false;
        await _hub.SendAsync(HubMethods.ConfirmAlarm, new AlarmActionDto
        {
            ConditionId = conditionId,
            EventIdBase64 = eventIdBase64,
            Comment = comment
        });
        return true;
    }

    /// <summary>Request a fresh alarm list from the bridge.</summary>
    public async Task RequestAlarmsAsync()
    {
        if (_hub?.State != HubConnectionState.Connected) return;
        await _hub.SendAsync(HubMethods.RequestAlarms);
    }

    // ─── Hub message handlers ────────────────────────────────

    private void OnValuesUpdated(List<TagValueDto> values)
    {
        lock (_lock)
        {
            foreach (var v in values)
                _values[v.Path] = v.Value;
        }
        ValuesChanged?.Invoke();
    }

    private void OnAlarmsUpdated(List<AlarmEntryDto> alarms)
    {
        var entries = alarms.Select(a => new AlarmEntry
        {
            Id = a.Id,
            SourceName = a.SourceName,
            Message = a.Message,
            Severity = a.Severity,
            Time = a.Time,
            IsAcked = a.IsAcked,
            IsConfirmed = a.IsConfirmed
        }).ToList();

        _lastAlarms = entries;
        AlarmsReceived?.Invoke(entries);
    }

    private void OnWriteResult(WriteResultDto result)
    {
        if (_pendingWrites.TryRemove(result.CorrelationId, out var tcs))
            tcs.TrySetResult(result.Success);
    }

    private void OnBridgeStatusChanged(bool online)
    {
        BridgeOnline = online;
        Log($"CLOUD: Bridge is {(online ? "ONLINE" : "OFFLINE")}");
        StateChanged?.Invoke();
    }

    public void Disconnect()
    {
        try { _hub?.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(2)); }
        catch { }
        _hub = null;
        BridgeOnline = false;
    }

    public void Dispose() => Disconnect();
}
