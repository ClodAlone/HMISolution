using System.Collections.Concurrent;
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
    public bool IsShelved { get; set; }
    public bool AllowShelving { get; set; }
    public DateTime? ShelvedUntil { get; set; }
    public string? ShelvedBy { get; set; }
    public bool IsSelected { get; set; }
}

/// <summary>Represents a node discovered during an OPC UA Browse operation.</summary>
public class BrowsedTag
{
    public string NodeId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string BrowsePath { get; set; } = "";
    public string NodeClass { get; set; } = "";
    public string DataType { get; set; } = "";
    public bool IsFolder { get; set; }
    public bool IsExpanded { get; set; }
    public int Depth { get; set; }
    public List<BrowsedTag> Children { get; set; } = [];
    public bool ChildrenLoaded { get; set; }
}

public class OpcRuntimeClient : IDisposable
{
    private Session? _session;
    private Subscription? _subscription;
    private ApplicationConfiguration? _appConfig;
    private readonly ConcurrentDictionary<string, string> _values = new();
    private readonly object _lock = new();

    // Multi-caller subscription tracking: each ScreenRenderer registers its paths
    // under a unique caller ID so popup/modal paths merge with the main screen paths.
    private readonly ConcurrentDictionary<string, HashSet<string>> _callerPaths = new();

    public bool IsConnected => _session?.Connected == true;
    public string? ErrorMessage { get; private set; }

    public event Action? ValuesChanged;
    public event Action<string>? ValueChanged;
    public event Action? StateChanged;

    // â”€â”€â”€ Diagnostic log â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
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
    }

    public string? GetValue(string variablePath)
    {
        return _values.TryGetValue(variablePath, out var v) ? v : null;
    }

    public async Task ConnectAsync(string endpointUrl, string? username = null, string? password = null)
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
                TransportQuotas = new TransportQuotas
                {
                    OperationTimeout = 15000,
                    MaxMessageSize = 16 * 1024 * 1024,
                    MaxBufferSize = 16 * 1024 * 1024,
                    MaxStringLength = 4 * 1024 * 1024,
                    MaxByteStringLength = 4 * 1024 * 1024
                },
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

            // Log supported token types for diagnostics
            var tokenTypes = endpointDescription.UserIdentityTokens?
                .Select(t => t.TokenType.ToString()) ?? [];
            Log($"CONNECT: Endpoint supports token types: [{string.Join(", ", tokenTypes)}]");

            UserIdentity identity;
            if (!string.IsNullOrEmpty(username))
            {
                var token = new UserNameIdentityToken
                {
                    UserName = username,
                    DecryptedPassword = System.Text.Encoding.UTF8.GetBytes(password ?? "")
                };
                identity = new UserIdentity(token);
                Log($"CONNECT: Creating session with user '{username}'...");
            }
            else
            {
                // Prefer anonymous; log a warning if the endpoint does not advertise it
                var supportsAnonymous = endpointDescription.UserIdentityTokens?
                    .Any(t => t.TokenType == UserTokenType.Anonymous) ?? false;

                identity = new UserIdentity(new AnonymousIdentityToken());
                Log(supportsAnonymous
                    ? "CONNECT: Creating session (anonymous)..."
                    : "CONNECT: WARNING — endpoint does not advertise Anonymous; attempting anyway...");
            }

            _session = await Session.Create(
                _appConfig, endpoint, false,
                "RuntimeViewerSession", 60000,
                identity, null);

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
            Log("CONNECT: âœ“ Connected successfully");
            StateChanged?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Log($"CONNECT: âœ— FAILED â€” {ex.GetType().Name}: {ex.Message}");
            StateChanged?.Invoke();
        }
    }

    private const string ServerNamespaceUri = "http://simpleopcfileserver.org/UA";

    /// <summary>Unregisters a caller's paths (e.g. when a popup/modal closes) and updates the subscription.</summary>
    public void UnregisterCaller(string callerId)
    {
        if (_callerPaths.TryRemove(callerId, out _))
        {
            Log($"MONITOR: Unregistered caller '{callerId}'");
            // Recompute the union of all remaining callers
            var union = new HashSet<string>();
            foreach (var kv in _callerPaths)
                union.UnionWith(kv.Value);
            MonitorVariablesCore(union);
        }
    }

    public void MonitorVariables(IEnumerable<string> variablePaths, ushort namespaceIndex = 0)
        => MonitorVariables(variablePaths, "default", namespaceIndex);

    public void MonitorVariables(IEnumerable<string> variablePaths, string callerId, ushort namespaceIndex = 0)
    {
        var pathSet = new HashSet<string>(variablePaths.Where(p => !string.IsNullOrEmpty(p)));
        _callerPaths[callerId] = pathSet;

        // Compute the union of all callers' paths
        var union = new HashSet<string>();
        foreach (var kv in _callerPaths)
            union.UnionWith(kv.Value);

        MonitorVariablesCore(union, namespaceIndex);
    }

    private void MonitorVariablesCore(IEnumerable<string> variablePaths, ushort namespaceIndex = 0)
    {
        if (_subscription == null || _session == null)
        {
            Log($"MONITOR: Skipped â€” subscription={(_subscription != null ? "ok" : "NULL")}, session={(_session != null ? "ok" : "NULL")}");
            return;
        }

        // Resolve namespace index dynamically from the session's namespace table
        if (namespaceIndex == 0 && _session != null)
        {
            var nsIdx = _session.NamespaceUris.GetIndex(ServerNamespaceUri);
            if (nsIdx >= 0)
                namespaceIndex = (ushort)nsIdx;
            else
                namespaceIndex = 2; // fallback
            Log($"MONITOR: Resolved namespace '{ServerNamespaceUri}' to index {namespaceIndex}");
        }

        var desiredPaths = new HashSet<string>(variablePaths.Where(p => !string.IsNullOrEmpty(p)));

        // Build set of currently monitored paths
        var currentPaths = new HashSet<string>(_subscription.MonitoredItems.Select(m => m.DisplayName));

        // Compute diff
        var toRemove = currentPaths.Where(p => !desiredPaths.Contains(p)).ToList();
        var toAdd = desiredPaths.Where(p => !currentPaths.Contains(p)).ToList();

        if (toRemove.Count == 0 && toAdd.Count == 0)
        {
            Log("MONITOR: No changes needed.");
            return;
        }

        Log($"MONITOR: Diff: +{toAdd.Count} add, -{toRemove.Count} remove (desired={desiredPaths.Count}, current={currentPaths.Count})");

        // Remove items no longer needed
        if (toRemove.Count > 0)
        {
            var itemsToRemove = _subscription.MonitoredItems
                .Where(m => toRemove.Contains(m.DisplayName)).ToList();
            _subscription.RemoveItems(itemsToRemove);

            // NOTE: We intentionally do NOT clear _values for removed paths here.
            // Clearing cached values immediately causes a visible UI blink ("—")
            // between the remove and when new OPC notifications arrive.
            // Stale cached values are harmless and will be overwritten when
            // the same path is re-subscribed later.
        }

        // Add new items
        foreach (var path in toAdd)
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

        // Fallback: for items that failed, try dropping the first path segment.
        // This handles cases like "Building.HVAC.AHU1.Temp" where the server uses
        // root-transparent paths ("HVAC.AHU1.Temp") but the alias map or screen
        // references include the project/root folder name as a prefix.
        var failedItems = _subscription.MonitoredItems
            .Where(mi => mi.Status?.Created != true && mi.StartNodeId.IdType == IdType.String)
            .ToList();

        if (failedItems.Count > 0)
        {
            var retryCount = 0;
            foreach (var failed in failedItems)
            {
                var original = (string)failed.StartNodeId.Identifier;
                var dot = original.IndexOf('.');
                if (dot < 0 || dot >= original.Length - 1) continue;

                var shortened = original[(dot + 1)..];
                failed.StartNodeId = new NodeId(shortened, namespaceIndex);
                retryCount++;
            }

            if (retryCount > 0)
            {
                Log($"MONITOR: Retrying {retryCount} failed item(s) with first path segment stripped.");
                _subscription.ApplyChanges();
            }
        }

        // Log the status of each monitored item after ApplyChanges
        foreach (var mi in _subscription.MonitoredItems)
        {
            var statusName = mi.Status?.Error?.StatusCode.ToString() ?? "Good";
            Log($"  ITEM: \"{mi.DisplayName}\" -> NodeId={mi.StartNodeId} | Created={mi.Status?.Created} | Status={statusName}");
        }

        var itemsToRead = _subscription.MonitoredItems
            .Where(mi => mi.Status?.Created == true && !_values.ContainsKey(mi.DisplayName))
            .ToList();
        if (itemsToRead.Count > 0)
        {
            try
            {
                var nodesToRead = new ReadValueIdCollection();
                foreach (var mi in itemsToRead)
                    nodesToRead.Add(new ReadValueId { NodeId = mi.StartNodeId, AttributeId = Attributes.Value });

                _session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead,
                    out DataValueCollection results, out _);

                for (var i = 0; i < results.Count; i++)
                {
                    if (StatusCode.IsGood(results[i].StatusCode))
                    {
                        var rawVal = results[i].WrappedValue.Value;
                        var val = rawVal is IFormattable fmt
                            ? fmt.ToString(null, System.Globalization.CultureInfo.InvariantCulture)
                            : rawVal?.ToString() ?? "";
                        _values[itemsToRead[i].DisplayName] = val;
                    }
                }
                Log($"MONITOR: Pre-read {itemsToRead.Count} initial values.");
            }
            catch (Exception ex)
            {
                Log($"MONITOR: Pre-read failed (non-fatal): {ex.Message}");
            }
        }

        Log($"MONITOR: Done. {_subscription.MonitoredItemCount} active monitored item(s)");
    }

    /// <summary>
    /// Pre-subscribes variables for a predicted next screen without removing existing items.
    /// These items are tagged so they can be identified and replaced when the actual screen loads.
    /// </summary>
    public void PreconnectVariables(IEnumerable<string> variablePaths)
    {
        if (_subscription == null || _session == null) return;

        ushort nsIndex = 0;
        var nsIdx = _session.NamespaceUris.GetIndex(ServerNamespaceUri);
        nsIndex = nsIdx >= 0 ? (ushort)nsIdx : (ushort)2;

        var currentPaths = new HashSet<string>(_subscription.MonitoredItems.Select(m => m.DisplayName));
        var toAdd = variablePaths.Where(p => !string.IsNullOrEmpty(p) && !currentPaths.Contains(p)).ToList();

        if (toAdd.Count == 0) return;

        Log($"PRECONNECT: Adding {toAdd.Count} predicted item(s)");

        foreach (var path in toAdd)
        {
            var nodeId = new NodeId(path, nsIndex);
            var item = new MonitoredItem(_subscription.DefaultItem)
            {
                DisplayName = path,
                StartNodeId = nodeId,
                SamplingInterval = 1000,
                QueueSize = 1,
                DiscardOldest = true
            };
            item.Notification += OnMonitoredItemNotification;
            _subscription.AddItem(item);
        }

        _subscription.ApplyChanges();
        Log($"PRECONNECT: Done. {_subscription.MonitoredItemCount} total item(s)");
    }

    /// <summary>
    /// Re-applies monitored items that failed (e.g. BadNodeIdUnknown) during
    /// initial subscription — typically because the server was still loading
    /// its address space via deferred loading.
    /// </summary>
    public void RetryFailedMonitoredItems()
    {
        if (_subscription == null || _session == null || !_session.Connected) return;

        var failed = _subscription.MonitoredItems
            .Where(m => m.Status?.Error != null && StatusCode.IsBad(m.Status.Error.StatusCode))
            .ToList();

        if (failed.Count == 0) return;

        Log($"RETRY: Re-applying {failed.Count} failed monitored item(s)...");

        _subscription.RemoveItems(failed);

        foreach (var old in failed)
        {
            var item = new MonitoredItem(_subscription.DefaultItem)
            {
                DisplayName = old.DisplayName,
                StartNodeId = old.StartNodeId,
                SamplingInterval = old.SamplingInterval,
                QueueSize = old.QueueSize,
                DiscardOldest = old.DiscardOldest
            };
            item.Notification += OnMonitoredItemNotification;
            _subscription.AddItem(item);
        }

        _subscription.ApplyChanges();

        // Fallback: try stripping first path segment for items still failing
        var stillBad = _subscription.MonitoredItems
            .Where(m => m.Status?.Created != true && m.StartNodeId.IdType == IdType.String)
            .ToList();
        if (stillBad.Count > 0)
        {
            var retryCount = 0;
            foreach (var mi in stillBad)
            {
                var id = (string)mi.StartNodeId.Identifier;
                var dot = id.IndexOf('.');
                if (dot < 0 || dot >= id.Length - 1) continue;
                mi.StartNodeId = new NodeId(id[(dot + 1)..], mi.StartNodeId.NamespaceIndex);
                retryCount++;
            }
            if (retryCount > 0)
            {
                Log($"RETRY: Retrying {retryCount} item(s) with first path segment stripped.");
                _subscription.ApplyChanges();
            }
        }

        var stillFailed = _subscription.MonitoredItems.Count(m => m.Status?.Error != null && StatusCode.IsBad(m.Status.Error.StatusCode));
        Log($"RETRY: Done. {failed.Count - stillFailed} recovered, {stillFailed} still failing.");
    }

    /// <summary>Returns true if any monitored items are in a failed state.</summary>
    public bool HasFailedMonitoredItems
    {
        get
        {
            if (_subscription == null) return false;
            return _subscription.MonitoredItems.Any(m => m.Status?.Error != null && StatusCode.IsBad(m.Status.Error.StatusCode));
        }
    }

        private int _notificationCount;

    private void OnMonitoredItemNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
    {
        if (e.NotificationValue is MonitoredItemNotification notification)
        {
            var rawVal = notification.Value?.WrappedValue.Value;
            var val = rawVal is IFormattable fmt
                ? fmt.ToString(null, System.Globalization.CultureInfo.InvariantCulture)
                : rawVal?.ToString() ?? "";
            var statusCode = notification.Value?.StatusCode ?? StatusCodes.Bad;
            _values[item.DisplayName] = val;
            _notificationCount++;
            // Log first 20 notifications, then every 50th to avoid flooding
            if (_notificationCount <= 20 || _notificationCount % 50 == 0)
            {
                Log($"NOTIFY[{_notificationCount}]: \"{item.DisplayName}\" = \"{val}\" (status={statusCode})");
            }
            ValuesChanged?.Invoke();
            ValueChanged?.Invoke(item.DisplayName);
        }
    }

    /// <summary>Update the local value cache without writing to the server. Fires change events so the UI refreshes immediately.</summary>
    public void UpdateLocalValue(string variablePath, string value)
    {
        _values[variablePath] = value;
        ValuesChanged?.Invoke();
        ValueChanged?.Invoke(variablePath);
    }

    /// <summary>Write a value to an OPC variable.</summary>
    public async Task<bool> WriteValueAsync(string variablePath, string value, ushort namespaceIndex = 2)
    {
        if (_session == null || !_session.Connected)
        {
            Log("WRITE SKIP: session null or disconnected for " + variablePath);
            return false;
        }

        try
        {
            // If we have a monitored item for this path, use its (possibly corrected) NodeId
            var monitoredItem = _subscription?.MonitoredItems.FirstOrDefault(m => m.DisplayName == variablePath);
            var nodeId = monitoredItem?.StartNodeId ?? new NodeId(variablePath, namespaceIndex);

            // Send the value as a string â€” the server's HandleWriteValue
            // converts strings to the correct DataType (Double, Int32, etc.).
            var nodesToWrite = new WriteValueCollection
            {
                new WriteValue
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(new Variant(value))
                }
            };

            StatusCodeCollection? results = null;
            DiagnosticInfoCollection? diagnosticInfos = null;
            await Task.Run(() => _session.Write(null, nodesToWrite, out results, out diagnosticInfos));

            var statusCode = results != null ? results[0] : StatusCodes.Bad;
            var ok = results != null && StatusCode.IsGood(statusCode);
            if (ok)
            {
                // Immediately reflect the written value in the local cache
                // so the UI updates without waiting for a subscription round-trip.
                _values[variablePath] = value;
                ValuesChanged?.Invoke();
                ValueChanged?.Invoke(variablePath);
            }
            else
            {
                Log("WRITE FAIL: " + variablePath + " = " + value + " status=" + statusCode);
            }
            return ok;
        }
        catch (Exception ex)
        {
            Log("WRITE ERROR: " + variablePath + " = " + value + " " + ex.GetType().Name + ": " + ex.Message);
            return false;
        }
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
            Log($"DISCONNECT: Error during close â€” {ex.Message}");
        }
        finally
        {
            _subscription = null;
            _session = null;
        }
        Log("DISCONNECT: Done");
        StateChanged?.Invoke();
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
            // [0] EventId â€” needed for Acknowledge/Confirm calls
            filter.SelectClauses.Add(new SimpleAttributeOperand(
                ObjectTypeIds.BaseEventType, BrowseNames.EventId));
            // [1] Retain â€” true if the alarm is still active
            filter.SelectClauses.Add(new SimpleAttributeOperand(
                ObjectTypeIds.ConditionType, BrowseNames.Retain));
            // [2] SourceNode â€” the NodeId of the condition source
            filter.SelectClauses.Add(new SimpleAttributeOperand(
                ObjectTypeIds.BaseEventType, BrowseNames.SourceNode));
            // [3] NodeId of the condition â€” used as ObjectId for method calls
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
                else if (e.NotificationValue is EventFieldList efl)
                {
                    events.Add(efl);
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
            await Task.Delay(1000);

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

            // [9] SuppressedState/Id (shelving)
            filter.SelectClauses.Add(new SimpleAttributeOperand(
                ObjectTypeIds.AlarmConditionType,
                new QualifiedName[] { BrowseNames.SuppressedState, BrowseNames.Id }));
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
                else if (e.NotificationValue is EventFieldList efl)
                    events.Add(efl);
            };

            sub.AddItem(mi);
            sub.ApplyChanges();

            await Task.Run(() =>
                _session.Call(ObjectTypeIds.ConditionType,
                    MethodIds.ConditionType_ConditionRefresh,
                    new object[] { sub.Id }));

            await Task.Delay(1000);

            int idx = 0;
            foreach (var evt in events)
            {
                if (evt.EventFields.Count < 7) continue;

                var retain = evt.EventFields[1].Value;
                if (retain is not true) continue;

                var eventId = evt.EventFields[0].Value as byte[];
                var conditionId = evt.EventFields[3].Value as NodeId;
                if (eventId == null) continue;

                var sourceName = evt.EventFields[2].Value?.ToString() ?? "";
                var message = evt.EventFields[4].Value is LocalizedText lt ? lt.Text : evt.EventFields[4].Value?.ToString() ?? "";
                var severity = evt.EventFields[5].Value?.ToString() ?? "0";
                var time = evt.EventFields[6].Value is DateTime dt ? dt : DateTime.UtcNow;
                var isAcked = evt.EventFields.Count > 7 && evt.EventFields[7].Value is true;
                var isConfirmed = evt.EventFields.Count > 8 && evt.EventFields[8].Value is true;

                var isShelved = evt.EventFields.Count > 9 && evt.EventFields[9].Value is true;

                result.Add(new AlarmEntry
                {
                    Id = $"alarm_{idx++}",
                    ConditionId = conditionId ?? NodeId.Null,
                    EventId = eventId,
                    SourceName = sourceName,
                    Message = message,
                    Severity = severity,
                    Time = time,
                    IsAcked = isAcked,
                    IsConfirmed = isConfirmed,
                    IsShelved = isShelved
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

    /// <summary>Shelve an alarm by calling the server's ShelveAlarm OPC UA method.</summary>
    public async Task<bool> ShelveAlarmAsync(string variablePath, int durationMinutes, string username = "operator")
    {
        if (_session == null || !_session.Connected) return false;
        try
        {
            var methodId = new NodeId("_AlarmManagement.ShelveAlarm", _session.NamespaceUris.GetIndexOrAppend("http://simpleopcfileserver.org/UA"));
            var objectId = new NodeId("_AlarmManagement", _session.NamespaceUris.GetIndexOrAppend("http://simpleopcfileserver.org/UA"));
            var result = await Task.Run(() =>
                _session.Call(objectId, methodId, new object[] { variablePath, durationMinutes, username }));
            return result != null && result.Count > 0 && result[0] is true;
        }
        catch (Exception ex)
        {
            Log($"SHELVE: Failed to shelve {variablePath} — {ex.Message}");
            return false;
        }
    }

    /// <summary>Unshelve an alarm by calling the server's UnshelveAlarm OPC UA method.</summary>
    public async Task<bool> UnshelveAlarmAsync(string variablePath, string username = "operator")
    {
        if (_session == null || !_session.Connected) return false;
        try
        {
            var methodId = new NodeId("_AlarmManagement.UnshelveAlarm", _session.NamespaceUris.GetIndexOrAppend("http://simpleopcfileserver.org/UA"));
            var objectId = new NodeId("_AlarmManagement", _session.NamespaceUris.GetIndexOrAppend("http://simpleopcfileserver.org/UA"));
            var result = await Task.Run(() =>
                _session.Call(objectId, methodId, new object[] { variablePath, username }));
            return result != null && result.Count > 0 && result[0] is true;
        }
        catch (Exception ex)
        {
            Log($"UNSHELVE: Failed to unshelve {variablePath} — {ex.Message}");
            return false;
        }
    }

    // ─── Tag Browser ─────────────────────────────────────────

    /// <summary>Browse child nodes of the given parent. Returns folders and variables.</summary>
    public async Task<List<BrowsedTag>> BrowseChildrenAsync(string? parentNodeId = null, ushort namespaceIndex = 2)
    {
        var result = new List<BrowsedTag>();
        if (_session == null || !_session.Connected) return result;

        try
        {
            NodeId startNode;
            if (string.IsNullOrEmpty(parentNodeId))
            {
                // Start from the Objects folder
                startNode = ObjectIds.ObjectsFolder;
            }
            else
            {
                startNode = NodeId.Parse(parentNodeId);
            }

            var browseDesc = new BrowseDescription
            {
                NodeId = startNode,
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable),
                ResultMask = (uint)(BrowseResultMask.DisplayName | BrowseResultMask.NodeClass | BrowseResultMask.TypeDefinition)
            };

            ReferenceDescriptionCollection? references = null;
            byte[]? continuationPoint = null;

            await Task.Run(() =>
                _session.Browse(null, null, startNode,
                    0u, browseDesc.BrowseDirection,
                    browseDesc.ReferenceTypeId,
                    browseDesc.IncludeSubtypes,
                    browseDesc.NodeClassMask,
                    out continuationPoint, out references));

            if (references != null)
            {
                foreach (var rd in references)
                {
                    var nodeId = ExpandedNodeId.ToNodeId(rd.NodeId, _session.NamespaceUris);
                    var isFolder = rd.NodeClass == NodeClass.Object;
                    var dataType = "";

                    if (!isFolder)
                    {
                        try
                        {
                            var dtVal = _session.ReadValue(nodeId);
                            dataType = dtVal?.WrappedValue.TypeInfo?.BuiltInType.ToString() ?? "";
                        }
                        catch { }
                    }

                    result.Add(new BrowsedTag
                    {
                        NodeId = nodeId.ToString(),
                        DisplayName = rd.DisplayName?.Text ?? "",
                        BrowsePath = nodeId is { NamespaceIndex: > 0, IdType: IdType.String }
                            ? (string)nodeId.Identifier
                            : rd.DisplayName?.Text ?? "",
                        NodeClass = rd.NodeClass.ToString(),
                        DataType = dataType,
                        IsFolder = isFolder
                    });
                }
            }

            // Release continuation point if any
            while (continuationPoint != null && continuationPoint.Length > 0)
            {
                var cp = continuationPoint;
                var browseNextResult = await Task.Run(() =>
                {
                    _session.BrowseNext(null, false, cp, out var nc, out var nr);
                    return (ContinuationPoint: nc, References: nr);
                });
                continuationPoint = browseNextResult.ContinuationPoint;

                if (browseNextResult.References != null)
                {
                    foreach (var rd in browseNextResult.References)
                    {
                        var nodeId = ExpandedNodeId.ToNodeId(rd.NodeId, _session.NamespaceUris);
                        var isFolder = rd.NodeClass == NodeClass.Object;
                        var dataType = "";

                        if (!isFolder)
                        {
                            try
                            {
                                var dtVal = _session.ReadValue(nodeId);
                                dataType = dtVal?.WrappedValue.TypeInfo?.BuiltInType.ToString() ?? "";
                            }
                            catch { }
                        }

                        result.Add(new BrowsedTag
                        {
                            NodeId = nodeId.ToString(),
                            DisplayName = rd.DisplayName?.Text ?? "",
                            BrowsePath = nodeId is { NamespaceIndex: > 0, IdType: IdType.String }
                                ? (string)nodeId.Identifier
                                : rd.DisplayName?.Text ?? "",
                            NodeClass = rd.NodeClass.ToString(),
                            DataType = dataType,
                            IsFolder = isFolder
                        });
                    }
                }
            }

            Log($"BROWSE: {result.Count} child node(s) under {parentNodeId ?? "Objects"}");
        }
        catch (Exception ex)
        {
            Log($"BROWSE ERROR: {ex.GetType().Name}: {ex.Message}");
        }

        return result;
    }

    /// <summary>Recursively collect all variable tags from the server (up to maxDepth levels).</summary>
    public async Task<List<BrowsedTag>> BrowseAllTagsAsync(int maxDepth = 10, ushort namespaceIndex = 2)
    {
        var allTags = new List<BrowsedTag>();
        if (_session == null || !_session.Connected) return allTags;

        async Task BrowseRecursive(string? parentId, string parentPath, int depth)
        {
            if (depth > maxDepth) return;
            var children = await BrowseChildrenAsync(parentId, namespaceIndex);
            foreach (var child in children)
            {
                var path = string.IsNullOrEmpty(parentPath)
                    ? child.DisplayName
                    : $"{parentPath}.{child.DisplayName}";
                child.BrowsePath = path;
                child.Depth = depth;
                allTags.Add(child);
                if (child.IsFolder)
                    await BrowseRecursive(child.NodeId, path, depth + 1);
            }
        }

        await BrowseRecursive(null, "", 0);
        Log($"BROWSE ALL: Found {allTags.Count} total tag(s)");
        return allTags;
    }

    /// <summary>Read the current value of a tag by its NodeId string.</summary>
    public async Task<string?> ReadTagValueAsync(string nodeIdStr)
    {
        if (_session == null || !_session.Connected) return null;
        try
        {
            var nodeId = NodeId.Parse(nodeIdStr);
            DataValue? val = null;
            await Task.Run(() => val = _session.ReadValue(nodeId));
            if (val?.WrappedValue.Value is IFormattable fmt)
                return fmt.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            return val?.WrappedValue.Value?.ToString();
        }
        catch (Exception ex)
        {
            Log($"READ TAG ERROR: {nodeIdStr} — {ex.Message}");
            return null;
        }
    }

    /// <summary>Write a value to a tag by its NodeId string.</summary>
    public async Task<bool> WriteTagValueAsync(string nodeIdStr, string value)
    {
        if (_session == null || !_session.Connected) return false;
        try
        {
            var nodeId = NodeId.Parse(nodeIdStr);
            var nodesToWrite = new WriteValueCollection
            {
                new WriteValue
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(new Variant(value))
                }
            };
            StatusCodeCollection? results = null;
            DiagnosticInfoCollection? diagnosticInfos = null;
            await Task.Run(() => _session.Write(null, nodesToWrite, out results, out diagnosticInfos));
            var ok = results != null && StatusCode.IsGood(results[0]);
            if (!ok) Log($"WRITE TAG FAIL: {nodeIdStr} = {value}");
            return ok;
        }
        catch (Exception ex)
        {
            Log($"WRITE TAG ERROR: {nodeIdStr} = {value} — {ex.Message}");
            return false;
        }
    }


    public void Dispose()
    {
        Disconnect();
    }
}

