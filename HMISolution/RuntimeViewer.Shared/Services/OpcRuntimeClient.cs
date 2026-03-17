using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace RuntimeViewer.Shared.Services;

/// <summary>Represents a single active alarm entry with enough data for display and actions.</summary>
public class AlarmEntry
{
    public string Id { get; set; } = "";
    public NodeId ConditionId { get; set; } = NodeId.Null;
    public byte[] EventId { get; set; } = [];
    public string SourceName { get; set; } = "";
    public string Message { get; set; } = "";
    public string Severity { get; set; } = "";
    public DateTime Time { get; set; }
    public bool IsAcked { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsSelected { get; set; }
}

public class OpcRuntimeClient : IDisposable
{
    private Session? _session;
    private Subscription? _subscription;
    private ApplicationConfiguration? _appConfig;
    private readonly Dictionary<string, string> _values = new();
    private readonly Dictionary<string, string?> _lastErrors = new();
    private readonly object _lock = new();

    public bool IsConnected => _session?.Connected == true;
    public string? ErrorMessage { get; private set; }

    public event Action? ValuesChanged;
    public event Action? StateChanged;

    // ─── Diagnostic log ─────────────────────────────────────
    private readonly List<string> _diagnosticLog = new();
    private const int MaxLogEntries = 200;

    /// <summary>Returns a snapshot of the diagnostic log entries.</summary>
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

    /// <summary>Returns the last error string for a variable, or null if quality is Good.</summary>
    public string? GetLastError(string variablePath)
    {
        lock (_lock)
        {
            return _lastErrors.TryGetValue(variablePath, out var err) ? err : null;
        }
    }

    public async Task ConnectAsync(string endpointUrl)
    {
        try
        {
            Log($"CONNECT: Connecting to {endpointUrl}");
            Disconnect();

            var pkiRoot = "%LocalApplicationData%/RuntimeViewer/pki";

            _appConfig = new ApplicationConfiguration
            {
                ApplicationName = "RuntimeViewer",
                ApplicationUri = $"urn:{Utils.GetHostName()}:RuntimeViewer",
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = $"{pkiRoot}/own",
                        SubjectName = "RuntimeViewer"
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = $"{pkiRoot}/trusted"
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = $"{pkiRoot}/issuer"
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = $"{pkiRoot}/rejected"
                    }
                },
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
                TraceConfiguration = new TraceConfiguration()
            };

            await _appConfig.Validate(ApplicationType.Client);

            _appConfig.CertificateValidator.CertificateValidation += (s, e) =>
            {
                e.Accept = true;
            };

            var app = new ApplicationInstance
            {
                ApplicationName = _appConfig.ApplicationName,
                ApplicationType = ApplicationType.Client,
                ApplicationConfiguration = _appConfig
            };

            try { await app.CheckApplicationInstanceCertificates(false, 2048); }
            catch
            {
                var storePath = Utils.ReplaceSpecialFolderNames(
                    _appConfig.SecurityConfiguration.ApplicationCertificate.StorePath);
                if (Directory.Exists(storePath))
                    foreach (var f in Directory.EnumerateFiles(storePath))
                        try { File.Delete(f); } catch { }
                await app.CheckApplicationInstanceCertificates(false, 2048);
            }

            // Discover endpoints
            Log($"CONNECT: Discovering endpoints at {endpointUrl}");
            var client = DiscoveryClient.Create(new Uri(endpointUrl));
            var endpoints = client.GetEndpoints(null);
            client.Dispose();

            Log($"CONNECT: Discovered {endpoints.Count} endpoint(s)");
            foreach (var ep in endpoints)
                Log($"  EP: {ep.EndpointUrl} | Security: {ep.SecurityMode}/{ep.SecurityPolicyUri?.Split('/').LastOrDefault()}");

            var endpointDescription =
                endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None
                    && e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
                ?? endpoints.FirstOrDefault(e => e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
                ?? endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None)
                ?? endpoints.FirstOrDefault();

            if (endpointDescription == null)
                throw new Exception("No endpoints found");

            Log($"CONNECT: Selected endpoint {endpointDescription.EndpointUrl}");

            // Override host/port with user-provided URL
            if (Uri.TryCreate(endpointDescription.EndpointUrl, UriKind.Absolute, out var discoveredUri) &&
                Uri.TryCreate(endpointUrl, UriKind.Absolute, out var userUri))
            {
                var builder = new UriBuilder(discoveredUri)
                {
                    Host = userUri.Host,
                    Port = userUri.Port
                };
                endpointDescription.EndpointUrl = builder.Uri.ToString();
            }

            var endpointConfiguration = EndpointConfiguration.Create(_appConfig);
            var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

            Log($"CONNECT: Creating session...");
            _session = await Session.Create(
                _appConfig, endpoint, false,
                "RuntimeViewerSession", 60000,
                new UserIdentity(new AnonymousIdentityToken()), null);

            Log($"CONNECT: Session created. Connected={_session.Connected}, SessionId={_session.SessionId}");
            Log($"CONNECT: Server namespaces: [{string.Join(", ", _session.NamespaceUris.ToArray())}]");

            _subscription = new Subscription(_session.DefaultSubscription)
            {
                PublishingInterval = 250,
                KeepAliveCount = 10,
                LifetimeCount = 100,
                MaxNotificationsPerPublish = 1000
            };

            _session.AddSubscription(_subscription);
            _subscription.Create();

            Log($"CONNECT: Subscription created. Id={_subscription.Id}, PublishingInterval={_subscription.CurrentPublishingInterval}ms");

            ErrorMessage = null;
            Log("CONNECT: ✓ Connected successfully");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Log($"CONNECT: ✗ FAILED — {ex.GetType().Name}: {ex.Message}");
        }
    }

    public void MonitorVariables(IEnumerable<string> variablePaths, ushort namespaceIndex = 2)
    {
        if (_subscription == null || _session == null)
        {
            Log($"MONITOR: Skipped — subscription={(_subscription != null ? "ok" : "NULL")}, session={(_session != null ? "ok" : "NULL")}");
            return;
        }

        var pathList = variablePaths.Where(p => !string.IsNullOrEmpty(p)).ToList();
        Log($"MONITOR: Setting up {pathList.Count} variable(s) with ns={namespaceIndex}");

        // Remove existing monitored items — snapshot to a list first to avoid
        // modifying the subscription's internal collection while iterating it.
        var existing = _subscription.MonitoredItems.ToList();
        if (existing.Count > 0)
        {
            Log($"MONITOR: Removing {existing.Count} existing monitored item(s)");
            _subscription.RemoveItems(existing);
            _subscription.ApplyChanges();
        }

        foreach (var path in pathList)
        {
            var nodeId = new NodeId(path, namespaceIndex);
            var item = new MonitoredItem(_subscription.DefaultItem)
            {
                DisplayName = path,
                StartNodeId = nodeId,
                SamplingInterval = 250,
                QueueSize = 1,
                DiscardOldest = true
            };

            item.Notification += OnMonitoredItemNotification;
            _subscription.AddItem(item);
        }

        _subscription.ApplyChanges();

        // Log the status of each monitored item after ApplyChanges
        foreach (var mi in _subscription.MonitoredItems)
        {
            var statusName = mi.Status?.Error?.StatusCode.ToString() ?? "Good";
            Log($"  ITEM: \"{mi.DisplayName}\" → NodeId={mi.StartNodeId} | Created={mi.Status?.Created} | Status={statusName}");
        }

        Log($"MONITOR: ✓ ApplyChanges done. {_subscription.MonitoredItemCount} active monitored item(s)");
    }

    private int _notificationCount;

    private void OnMonitoredItemNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
    {
        if (e.NotificationValue is MonitoredItemNotification notification)
        {
            var val = notification.Value?.WrappedValue.ToString() ?? "";
            var statusCode = notification.Value?.StatusCode ?? StatusCodes.Bad;
            lock (_lock)
            {
                _values[item.DisplayName] = val;
                _lastErrors[item.DisplayName] = StatusCode.IsGood(statusCode)
                    ? null
                    : $"{statusCode} ({StatusCode.LookupSymbolicId(statusCode.Code)})";
            }
            _notificationCount++;
            // Log first 20 notifications, then every 50th to avoid flooding
            if (_notificationCount <= 20 || _notificationCount % 50 == 0)
            {
                Log($"NOTIFY[{_notificationCount}]: \"{item.DisplayName}\" = \"{val}\" (status={statusCode})");
            }
            ValuesChanged?.Invoke();
        }
    }

    /// <summary>Write a value to an OPC variable.</summary>
    public async Task<bool> WriteValueAsync(string variablePath, string value, ushort namespaceIndex = 2)
    {
        if (_session == null || !_session.Connected) return false;

        try
        {
            var nodeId = new NodeId(variablePath, namespaceIndex);

            // Read data type for correct conversion
            var nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId { NodeId = nodeId, AttributeId = Attributes.DataType }
            };
            DataValueCollection readResults = null!;
            DiagnosticInfoCollection readDiag = null!;
            await Task.Run(() => _session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead, out readResults, out readDiag));

            var dataTypeId = readResults.Count > 0 && StatusCode.IsGood(readResults[0].StatusCode)
                ? readResults[0].Value as NodeId : null;

            var typedValue = ConvertValue(dataTypeId, value);

            var nodesToWrite = new WriteValueCollection
            {
                new WriteValue
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(new Variant(typedValue))
                }
            };

            StatusCodeCollection? results = null;
            DiagnosticInfoCollection? diagnosticInfos = null;
            await Task.Run(() => _session.Write(null, nodesToWrite, out results, out diagnosticInfos));

            return results != null && StatusCode.IsGood(results[0]);
        }
        catch
        {
            return false;
        }
    }

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

    public void Disconnect()
    {
        Log("DISCONNECT: Closing session...");
        try
        {
            _subscription?.Delete(true);
            _session?.Close();
        }
        catch (Exception ex)
        {
            Log($"DISCONNECT: Error during close — {ex.Message}");
        }
        finally
        {
            _subscription = null;
            _session = null;
        }
        Log("DISCONNECT: Done");
    }

    /// <summary>
    /// Acknowledge all active alarms/conditions on the server.
    /// Browses the Server object for active conditions and acknowledges each one.
    /// </summary>
    public async Task<(int Succeeded, int Failed)> AcknowledgeAllAlarmsAsync(string comment = "Acknowledged from RuntimeViewer")
    {
        if (_session == null || !_session.Connected) return (0, 0);

        try
        {
            var alarms = await CollectActiveAlarmsAsync();
            int ok = 0, fail = 0;

            foreach (var (conditionId, eventId) in alarms)
            {
                try
                {
                    var methodId = MethodIds.AcknowledgeableConditionType_Acknowledge;
                    var inputArgs = new object[] { eventId, new LocalizedText(comment) };
                    var result = await Task.Run(() =>
                        _session.Call(conditionId, methodId, inputArgs));
                    ok++;
                }
                catch { fail++; }
            }

            return (ok, fail);
        }
        catch { return (0, 0); }
    }

    /// <summary>
    /// Confirm (reset) all active alarms/conditions on the server.
    /// </summary>
    public async Task<(int Succeeded, int Failed)> ResetAllAlarmsAsync(string comment = "Confirmed from RuntimeViewer")
    {
        if (_session == null || !_session.Connected) return (0, 0);

        try
        {
            var alarms = await CollectActiveAlarmsAsync();
            int ok = 0, fail = 0;

            foreach (var (conditionId, eventId) in alarms)
            {
                try
                {
                    var methodId = MethodIds.AcknowledgeableConditionType_Confirm;
                    var inputArgs = new object[] { eventId, new LocalizedText(comment) };
                    var result = await Task.Run(() =>
                        _session.Call(conditionId, methodId, inputArgs));
                    ok++;
                }
                catch { fail++; }
            }

            return (ok, fail);
        }
        catch { return (0, 0); }
    }

    private async Task<List<(NodeId ConditionId, byte[] EventId)>> CollectActiveAlarmsAsync()
    {
        var alarms = new List<(NodeId, byte[])>();
        if (_session == null) return alarms;

        try
        {
            // Use ConditionRefresh via a temporary event subscription to collect active alarms
            var filter = new EventFilter();
            // [0] EventId — needed for Acknowledge/Confirm calls
            filter.SelectClauses.Add(new SimpleAttributeOperand(
                ObjectTypeIds.BaseEventType, BrowseNames.EventId));
            // [1] Retain — true if the alarm is still active
            filter.SelectClauses.Add(new SimpleAttributeOperand(
                ObjectTypeIds.ConditionType, BrowseNames.Retain));
            // [2] SourceNode — the NodeId of the condition source
            filter.SelectClauses.Add(new SimpleAttributeOperand(
                ObjectTypeIds.BaseEventType, BrowseNames.SourceNode));
            // [3] NodeId of the condition — used as ObjectId for method calls
            filter.SelectClauses.Add(new SimpleAttributeOperand(
                ObjectTypeIds.ConditionType, BrowseNames.NodeId));

            var sub = new Subscription(_session.DefaultSubscription)
            {
                PublishingInterval = 100,
                KeepAliveCount = 5,
                LifetimeCount = 20,
                MaxNotificationsPerPublish = 1000
            };
            _session.AddSubscription(sub);
            sub.Create();

            var monitoredItem = new MonitoredItem(sub.DefaultItem)
            {
                StartNodeId = ObjectIds.Server,
                AttributeId = Attributes.EventNotifier,
                Filter = filter,
                QueueSize = 1000,
                DiscardOldest = true
            };

            var events = new List<EventFieldList>();
            monitoredItem.Notification += (item, e) =>
            {
                if (e.NotificationValue is EventNotificationList enl)
                {
                    foreach (var evt in enl.Events)
                        events.Add(evt);
                }
            };

            sub.AddItem(monitoredItem);
            sub.ApplyChanges();

            // Request ConditionRefresh to get all active conditions
            await Task.Run(() =>
                _session.Call(ObjectTypeIds.ConditionType,
                    MethodIds.ConditionType_ConditionRefresh,
                    new object[] { sub.Id }));

            // Wait briefly for events to arrive
            await Task.Delay(500);

            foreach (var evt in events)
            {
                if (evt.EventFields.Count < 4) continue;

                var retain = evt.EventFields[1].Value;
                bool isRetained = retain is bool b ? b : false;
                if (!isRetained) continue;

                var eventId = evt.EventFields[0].Value as byte[];
                var conditionId = evt.EventFields[3].Value as NodeId;

                if (conditionId != null && eventId != null)
                    alarms.Add((conditionId, eventId));
            }

            // Clean up temporary subscription
            sub.Delete(true);
            _session.RemoveSubscription(sub);
        }
        catch { }

        return alarms;
    }

    /// <summary>
    /// Collect detailed alarm entries for display in an alarm list widget.
    /// Returns enriched alarm info including message, severity, time, source, and acked/confirmed state.
    /// </summary>
    public async Task<List<AlarmEntry>> CollectDetailedAlarmsAsync()
    {
        var result = new List<AlarmEntry>();
        if (_session == null) return result;

        try
        {
            var filter = new EventFilter();
            // [0] EventId
            filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.EventId));
            // [1] Retain
            filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.ConditionType, BrowseNames.Retain));
            // [2] SourceName
            filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.SourceName));
            // [3] ConditionNodeId
            filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.ConditionType, BrowseNames.NodeId));
            // [4] Message
            filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.Message));
            // [5] Severity
            filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.Severity));
            // [6] Time
            filter.SelectClauses.Add(new SimpleAttributeOperand(ObjectTypeIds.BaseEventType, BrowseNames.Time));
            // [7] AckedState/Id
            filter.SelectClauses.Add(new SimpleAttributeOperand(
                ObjectTypeIds.AcknowledgeableConditionType,
                new QualifiedName[] { BrowseNames.AckedState, BrowseNames.Id }));
            // [8] ConfirmedState/Id
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

            await Task.Run(() =>
                _session.Call(ObjectTypeIds.ConditionType,
                    MethodIds.ConditionType_ConditionRefresh,
                    new object[] { sub.Id }));

            await Task.Delay(500);

            int idx = 0;
            foreach (var evt in events)
            {
                if (evt.EventFields.Count < 7) continue;

                var retain = evt.EventFields[1].Value;
                if (retain is not true) continue;

                var eventId = evt.EventFields[0].Value as byte[];
                var conditionId = evt.EventFields[3].Value as NodeId;
                if (conditionId == null || eventId == null) continue;

                var sourceName = evt.EventFields[2].Value?.ToString() ?? "";
                var message = evt.EventFields[4].Value is LocalizedText lt ? lt.Text : evt.EventFields[4].Value?.ToString() ?? "";
                var severity = evt.EventFields[5].Value?.ToString() ?? "0";
                var time = evt.EventFields[6].Value is DateTime dt ? dt : DateTime.UtcNow;
                var isAcked = evt.EventFields.Count > 7 && evt.EventFields[7].Value is true;
                var isConfirmed = evt.EventFields.Count > 8 && evt.EventFields[8].Value is true;

                result.Add(new AlarmEntry
                {
                    Id = $"alarm_{idx++}",
                    ConditionId = conditionId,
                    EventId = eventId,
                    SourceName = sourceName,
                    Message = message,
                    Severity = severity,
                    Time = time,
                    IsAcked = isAcked,
                    IsConfirmed = isConfirmed
                });
            }

            sub.Delete(true);
            _session.RemoveSubscription(sub);
        }
        catch { }

        return result;
    }

    /// <summary>Acknowledge a single alarm by its condition NodeId and EventId.</summary>
    public async Task<bool> AcknowledgeAlarmAsync(NodeId conditionId, byte[] eventId, string comment = "Acknowledged")
    {
        if (_session == null || !_session.Connected) return false;
        try
        {
            await Task.Run(() =>
                _session.Call(conditionId, MethodIds.AcknowledgeableConditionType_Acknowledge,
                    new object[] { eventId, new LocalizedText(comment) }));
            return true;
        }
        catch { return false; }
    }

    /// <summary>Confirm (reset) a single alarm by its condition NodeId and EventId.</summary>
    public async Task<bool> ConfirmAlarmAsync(NodeId conditionId, byte[] eventId, string comment = "Confirmed")
    {
        if (_session == null || !_session.Connected) return false;
        try
        {
            await Task.Run(() =>
                _session.Call(conditionId, MethodIds.AcknowledgeableConditionType_Confirm,
                    new object[] { eventId, new LocalizedText(comment) }));
            return true;
        }
        catch { return false; }
    }

    public void Dispose()
    {
        Disconnect();
    }
}

