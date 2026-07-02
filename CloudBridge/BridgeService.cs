// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Microsoft.AspNetCore.SignalR.Client;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using SharedModels;
using SharedModels.CloudRelay;

namespace CloudBridge;

/// <summary>
/// Core bridge service: connects to the local OPC UA server AND to the cloud
/// SignalR relay hub. Forwards tag values and alarms from OPC to the cloud,
/// and routes write/alarm commands from the cloud back to OPC.
/// Both connections are outbound — no inbound firewall ports required.
/// </summary>
public sealed class BridgeService : IDisposable
{
    private readonly string _opcEndpoint;
    private readonly string _hubUrl;
    private readonly string _apiKey;

    private HubConnection? _hub;
    private Session? _session;
    private Subscription? _subscription;
    private ApplicationConfiguration? _appConfig;

    private readonly Dictionary<string, string> _latestValues = new();
    private readonly object _lock = new();

    // Store-and-forward
    private readonly StoreAndForwardQueue? _spool;
    private readonly StoreAndForwardConfig? _safConfig;
    private Timer? _drainTimer;

    public BridgeService(string opcEndpoint, string hubUrl, string apiKey,
        StoreAndForwardConfig? safConfig = null, string? projectDir = null)
    {
        _opcEndpoint = opcEndpoint;
        _hubUrl = hubUrl;
        _apiKey = apiKey;
        _safConfig = safConfig;

        if (safConfig is { Enabled: true })
        {
            _spool = new StoreAndForwardQueue(safConfig, projectDir ?? AppContext.BaseDirectory);
        }
    }

    public async Task RunAsync(CancellationToken ct)
    {
        // 1. Connect to the cloud relay hub
        await ConnectToHubAsync(ct);

        // 2. Connect to the local OPC UA server
        await ConnectToOpcAsync();

        // 3. Start the store-and-forward drain timer
        StartDrainTimer();

        // 4. Keep running, reconnecting as needed
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(5000, ct);

                // Reconnect hub if dropped
                if (_hub?.State != HubConnectionState.Connected)
                {
                    Log("Hub disconnected — reconnecting...");
                    await ConnectToHubAsync(ct);
                }

                // Reconnect OPC if dropped
                if (_session == null || !_session.Connected)
                {
                    Log("OPC session lost — reconnecting...");
                    await ConnectToOpcAsync();
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                Log($"Reconnect error: {ex.Message}");
            }
        }

        Log("Shutting down...");
        Dispose();
    }

    // ─── Cloud Hub Connection ────────────────────────────────

    private async Task ConnectToHubAsync(CancellationToken ct)
    {
        _hub?.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(2));

        var url = string.IsNullOrEmpty(_apiKey)
            ? _hubUrl
            : $"{_hubUrl}?apiKey={Uri.EscapeDataString(_apiKey)}";

        _hub = new HubConnectionBuilder()
            .WithUrl(url)
            .WithAutomaticReconnect()
            .Build();

        // Register handlers for commands coming from viewers
        _hub.On<List<string>, int>(HubMethods.Subscribe, OnSubscribeRequested);
        _hub.On<WriteRequestDto>(HubMethods.WriteValue, OnWriteRequested);
        _hub.On<AlarmActionDto>(HubMethods.AcknowledgeAlarm, OnAcknowledgeRequested);
        _hub.On<AlarmActionDto>(HubMethods.ConfirmAlarm, OnConfirmRequested);
        _hub.On(HubMethods.RequestAlarms, OnAlarmsRequested);

        _hub.Reconnected += async _ =>
        {
            Log("Hub reconnected — re-joining as bridge");
            await _hub.SendAsync("JoinAsBridge", ct);
        };

        Log("Connecting to cloud relay hub...");
        await _hub.StartAsync(ct);
        await _hub.SendAsync("JoinAsBridge", ct);
        Log("✓ Connected to cloud relay hub");
    }

    // ─── OPC UA Connection ───────────────────────────────────

    private async Task ConnectToOpcAsync()
    {
        try
        {
            _subscription?.Delete(true);
            _session?.Close();
        }
        catch { }

        var pkiRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CloudBridge", "pki");

        _appConfig = new ApplicationConfiguration
        {
            ApplicationName = "CloudBridge",
            ApplicationUri = $"urn:{Utils.GetHostName()}:CloudBridge",
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(pkiRoot, "own"),
                    SubjectName = "CloudBridge"
                },
                TrustedPeerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(pkiRoot, "trusted")
                },
                TrustedIssuerCertificates = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(pkiRoot, "issuer")
                },
                RejectedCertificateStore = new CertificateTrustList
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = Path.Combine(pkiRoot, "rejected")
                }
            },
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            TraceConfiguration = new TraceConfiguration()
        };

        await _appConfig.Validate(ApplicationType.Client);
        _appConfig.CertificateValidator.CertificateValidation += (s, e) => e.Accept = true;

        var app = new ApplicationInstance
        {
            ApplicationName = _appConfig.ApplicationName,
            ApplicationType = ApplicationType.Client,
            ApplicationConfiguration = _appConfig
        };

        try { await app.CheckApplicationInstanceCertificates(false, 2048); }
        catch
        {
            var storePath = _appConfig.SecurityConfiguration.ApplicationCertificate.StorePath;
            if (Directory.Exists(storePath))
                foreach (var f in Directory.EnumerateFiles(storePath))
                    try { File.Delete(f); } catch { }
            await app.CheckApplicationInstanceCertificates(false, 2048);
        }

        Log($"Discovering OPC endpoints at {_opcEndpoint}...");
        var client = DiscoveryClient.Create(new Uri(_opcEndpoint));
        var endpoints = client.GetEndpoints(null);
        client.Dispose();

        var endpointDesc =
            endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None
                && e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
            ?? endpoints.FirstOrDefault(e => e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
            ?? endpoints.FirstOrDefault();

        if (endpointDesc == null)
            throw new Exception("No OPC UA endpoints discovered");

        // Override host/port to match user-provided URL
        if (Uri.TryCreate(endpointDesc.EndpointUrl, UriKind.Absolute, out var discoveredUri) &&
            Uri.TryCreate(_opcEndpoint, UriKind.Absolute, out var userUri))
        {
            var builder = new UriBuilder(discoveredUri) { Host = userUri.Host, Port = userUri.Port };
            endpointDesc.EndpointUrl = builder.Uri.ToString();
        }

        var epConfig = EndpointConfiguration.Create(_appConfig);
        var endpoint = new ConfiguredEndpoint(null, endpointDesc, epConfig);

        Log("Creating OPC session...");
        _session = await Session.Create(
            _appConfig, endpoint, false,
            "CloudBridgeSession", 60000,
            new UserIdentity(new AnonymousIdentityToken()), null);

        _subscription = new Subscription(_session.DefaultSubscription)
        {
            PublishingInterval = 250,
            KeepAliveCount = 10,
            LifetimeCount = 100,
            MaxNotificationsPerPublish = 1000
        };
        _session.AddSubscription(_subscription);
        _subscription.Create();

        Log($"✓ Connected to OPC UA — SessionId={_session.SessionId}");
    }

    // ─── Hub → Bridge command handlers ───────────────────────

    private void OnSubscribeRequested(List<string> variablePaths, int namespaceIndex)
    {
        if (_subscription == null || _session == null) return;

        Log($"Subscribe request: {variablePaths.Count} variable(s), ns={namespaceIndex}");

        var existing = _subscription.MonitoredItems.ToList();
        if (existing.Count > 0)
        {
            _subscription.RemoveItems(existing);
            _subscription.ApplyChanges();
        }

        foreach (var path in variablePaths.Where(p => !string.IsNullOrEmpty(p)))
        {
            var nodeId = new NodeId(path, (ushort)namespaceIndex);
            var item = new MonitoredItem(_subscription.DefaultItem)
            {
                DisplayName = path,
                StartNodeId = nodeId,
                SamplingInterval = 250,
                QueueSize = 1,
                DiscardOldest = true
            };
            item.Notification += OnOpcNotification;
            _subscription.AddItem(item);
        }

        _subscription.ApplyChanges();
        Log($"  → {_subscription.MonitoredItemCount} monitored item(s) active");
    }

    private async void OnWriteRequested(WriteRequestDto req)
    {
        bool success = false;
        try
        {
            if (_session != null && _session.Connected)
            {
                var nodeId = new NodeId(req.VariablePath, (ushort)req.NamespaceIndex);

                // Read data type
                var nodesToRead = new ReadValueIdCollection
                {
                    new ReadValueId { NodeId = nodeId, AttributeId = Attributes.DataType }
                };
                _session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead,
                    out var readResults, out _);

                var dataTypeId = readResults.Count > 0 && StatusCode.IsGood(readResults[0].StatusCode)
                    ? readResults[0].Value as NodeId : null;
                var typed = ConvertValue(dataTypeId, req.Value);

                var nodesToWrite = new WriteValueCollection
                {
                    new WriteValue
                    {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value,
                        Value = new DataValue(new Variant(typed))
                    }
                };
                _session.Write(null, nodesToWrite, out var results, out _);
                success = results != null && StatusCode.IsGood(results[0]);
            }
        }
        catch (Exception ex) { Log($"Write error: {ex.Message}"); }

        if (_hub?.State == HubConnectionState.Connected)
        {
            await _hub.SendAsync(HubMethods.WriteResult,
                new WriteResultDto { CorrelationId = req.CorrelationId, Success = success });
        }
    }

    private async void OnAcknowledgeRequested(AlarmActionDto action)
    {
        if (_session == null || !_session.Connected) return;
        try
        {
            var conditionId = NodeId.Parse(action.ConditionId);
            var eventId = Convert.FromBase64String(action.EventIdBase64);
            _session.Call(conditionId, MethodIds.AcknowledgeableConditionType_Acknowledge,
                new object[] { eventId, new LocalizedText(action.Comment) });
            Log($"Acknowledged alarm {action.ConditionId}");
        }
        catch (Exception ex) { Log($"Ack error: {ex.Message}"); }
    }

    private async void OnConfirmRequested(AlarmActionDto action)
    {
        if (_session == null || !_session.Connected) return;
        try
        {
            var conditionId = NodeId.Parse(action.ConditionId);
            var eventId = Convert.FromBase64String(action.EventIdBase64);
            _session.Call(conditionId, MethodIds.AcknowledgeableConditionType_Confirm,
                new object[] { eventId, new LocalizedText(action.Comment) });
            Log($"Confirmed alarm {action.ConditionId}");
        }
        catch (Exception ex) { Log($"Confirm error: {ex.Message}"); }
    }

    private async void OnAlarmsRequested()
    {
        // Collect and push alarms (simplified — reuses session)
        if (_session == null || !_session.Connected || _hub?.State != HubConnectionState.Connected) return;
        try
        {
            var alarms = await CollectAlarmsFromOpcAsync();
            await _hub.SendAsync(HubMethods.AlarmsUpdated, alarms);
        }
        catch (Exception ex) { Log($"Alarm collect error: {ex.Message}"); }
    }

    // ─── OPC → Cloud value forwarding ────────────────────────

    private int _pushCount;

    private async void OnOpcNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
    {
        if (e.NotificationValue is not MonitoredItemNotification notification) return;

        var val = notification.Value?.WrappedValue.ToString() ?? "";
        lock (_lock) { _latestValues[item.DisplayName] = val; }

        // Batch: push all current values to the cloud
        List<TagValueDto> snapshot;
        lock (_lock)
        {
            snapshot = _latestValues.Select(kv => new TagValueDto { Path = kv.Key, Value = kv.Value }).ToList();
        }

        if (_hub?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hub.SendAsync(HubMethods.ValuesUpdated, snapshot);
                _pushCount++;
                if (_pushCount <= 10 || _pushCount % 100 == 0)
                    Log($"Pushed {snapshot.Count} value(s) to cloud [#{_pushCount}]");
            }
            catch (Exception ex)
            {
                Log($"Push error: {ex.Message}");
                // Spool failed push for later delivery
                _spool?.Enqueue(snapshot);
            }
        }
        else
        {
            // Hub is disconnected — spool the data for store-and-forward
            _spool?.Enqueue(snapshot);
        }
    }

    // ─── OPC alarm collection ────────────────────────────────

    private async Task<List<AlarmEntryDto>> CollectAlarmsFromOpcAsync()
    {
        var result = new List<AlarmEntryDto>();
        if (_session == null) return result;

        var filter = new EventFilter();
        filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.EventId));
        filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.ConditionType, BrowseNames.Retain));
        filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.SourceName));
        filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.ConditionType, BrowseNames.NodeId));
        filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.Message));
        filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.Severity));
        filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.Time));
        filter.SelectClauses.Add(new SimpleAttributeOperand(
            ObjectTypeIds.AcknowledgeableConditionType,
            new QualifiedName[] { BrowseNames.AckedState, BrowseNames.Id }));
        filter.SelectClauses.Add(new SimpleAttributeOperand(
            ObjectTypeIds.AcknowledgeableConditionType,
            new QualifiedName[] { BrowseNames.ConfirmedState, BrowseNames.Id }));

        var sub = new Subscription(_session.DefaultSubscription)
        {
            PublishingInterval = 100, KeepAliveCount = 5,
            LifetimeCount = 20, MaxNotificationsPerPublish = 1000
        };
        _session.AddSubscription(sub);
        sub.Create();

        var mi = new MonitoredItem(sub.DefaultItem)
        {
            StartNodeId = ObjectIds.Server,
            AttributeId = Attributes.EventNotifier,
            Filter = filter, QueueSize = 1000, DiscardOldest = true
        };

        var events = new List<EventFieldList>();
        mi.Notification += (item, e) =>
        {
            if (e.NotificationValue is EventNotificationList enl)
                foreach (var evt in enl.Events) events.Add(evt);
        };
        sub.AddItem(mi);
        sub.ApplyChanges();

        _session.Call(ObjectTypeIds.ConditionType,
            MethodIds.ConditionType_ConditionRefresh, new object[] { sub.Id });
        await Task.Delay(500);

        int idx = 0;
        foreach (var evt in events)
        {
            if (evt.EventFields.Count < 7) continue;
            if (evt.EventFields[1].Value is not true) continue;

            var eventId = evt.EventFields[0].Value as byte[];
            var conditionId = evt.EventFields[3].Value as NodeId;
            if (conditionId == null || eventId == null) continue;

            result.Add(new AlarmEntryDto
            {
                Id = $"alarm_{idx++}",
                ConditionId = conditionId.ToString(),
                EventIdBase64 = Convert.ToBase64String(eventId),
                SourceName = evt.EventFields[2].Value?.ToString() ?? "",
                Message = evt.EventFields[4].Value is LocalizedText lt ? lt.Text : evt.EventFields[4].Value?.ToString() ?? "",
                Severity = evt.EventFields[5].Value?.ToString() ?? "0",
                Time = evt.EventFields[6].Value is DateTime dt ? dt : DateTime.UtcNow,
                IsAcked = evt.EventFields.Count > 7 && evt.EventFields[7].Value is true,
                IsConfirmed = evt.EventFields.Count > 8 && evt.EventFields[8].Value is true
            });
        }

        sub.Delete(true);
        _session.RemoveSubscription(sub);
        return result;
    }

    // ─── Helpers ─────────────────────────────────────────────

    private static object ConvertValue(NodeId? dataTypeId, string value)
    {
        if (dataTypeId == null) return value;
        var id = dataTypeId.Identifier is uint uid ? uid : 0u;
        return id switch
        {
            DataTypes.Boolean => bool.Parse(value),
            DataTypes.Int16 => short.Parse(value),
            DataTypes.UInt16 => ushort.Parse(value),
            DataTypes.Int32 => int.Parse(value),
            DataTypes.UInt32 => uint.Parse(value),
            DataTypes.Int64 => long.Parse(value),
            DataTypes.UInt64 => ulong.Parse(value),
            DataTypes.Float => float.Parse(value, System.Globalization.CultureInfo.InvariantCulture),
            DataTypes.Double => double.Parse(value, System.Globalization.CultureInfo.InvariantCulture),
            _ => value
        };
    }

    private static void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
    }

    // ─── Store-and-Forward Drain ─────────────────────────────

    private long _drainCount;

    private void StartDrainTimer()
    {
        if (_spool == null || _safConfig == null) return;

        var intervalMs = Math.Max(500, _safConfig.DrainIntervalSeconds * 1000);
        _drainTimer = new Timer(async _ => await DrainSpoolAsync(), null, intervalMs, intervalMs);
        Log($"Store-and-forward drain timer started (interval: {intervalMs}ms, batch: {_safConfig.BatchSize})");
    }

    private async Task DrainSpoolAsync()
    {
        if (_spool == null || _hub?.State != HubConnectionState.Connected) return;

        try
        {
            var batchSize = _safConfig?.BatchSize ?? 200;
            var (values, ids) = _spool.Dequeue(batchSize);
            if (values.Count == 0) return;

            await _hub.SendAsync(HubMethods.ValuesUpdated, values);
            _spool.Acknowledge(ids);

            _drainCount += ids.Count;
            var pending = _spool.GetPendingCount();
            Log($"[SAF] Drained {ids.Count} spooled row(s) → cloud (total drained: {_drainCount}, pending: {pending})");
        }
        catch (Exception ex)
        {
            Log($"[SAF] Drain error (will retry): {ex.Message}");
        }
    }

    public void Dispose()
    {
        _drainTimer?.Dispose();
        _spool?.Dispose();
        try { _subscription?.Delete(true); } catch { }
        try { _session?.Close(); } catch { }
        _hub?.DisposeAsync().AsTask().Wait(TimeSpan.FromSeconds(2));
    }
}
