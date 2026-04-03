using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace ServerEditorWeb.Services;

public class OpcBrowseNode
{
    public string DisplayName { get; set; } = "";
    public string NodeIdString { get; set; } = "";
    public NodeId ResolvedNodeId { get; set; } = NodeId.Null;
    public string NodeClass { get; set; } = "";
    public string BrowseName { get; set; } = "";
    public bool IsVariable { get; set; }
    public bool HasChildren { get; set; }
    public List<OpcBrowseNode> Children { get; set; } = new();
    public bool ChildrenLoaded { get; set; }
    public bool IsExpanded { get; set; }
}

public class OpcMonitoredValue
{
    public string DisplayName { get; set; } = "";
    public string NodeIdString { get; set; } = "";
    public NodeId ResolvedNodeId { get; set; } = NodeId.Null;
    public string Value { get; set; } = "Pending...";
    public string StatusCode { get; set; } = "";
    public string Timestamp { get; set; } = "";
    public bool IsWritable { get; set; }
}

public class OpcClientService : IDisposable
{
    private static readonly string RecentEndpointsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                     "ServerEditorWeb", "recent-endpoints.txt");

    private Session? _session;
    private Subscription? _subscription;
    private ApplicationConfiguration? _appConfig;
    private readonly object _lock = new();

    public bool IsConnected => _session?.Connected == true;
    public string? ErrorMessage { get; private set; }
    public List<OpcBrowseNode> RootNodes { get; } = new();
    public List<OpcMonitoredValue> MonitoredItems { get; } = new();
    public List<string> RecentEndpoints { get; } = new();

    public event Action? StateChanged;

    private void NotifyChanged() => StateChanged?.Invoke();

    public OpcClientService()
    {
        LoadRecentEndpoints();
    }

    private void LoadRecentEndpoints()
    {
        try
        {
            if (File.Exists(RecentEndpointsPath))
            {
                var lines = File.ReadAllLines(RecentEndpointsPath)
                    .Where(l => !string.IsNullOrWhiteSpace(l)).Take(10);
                RecentEndpoints.AddRange(lines);
            }
        }
        catch { }
    }

    private void SaveRecentEndpoints()
    {
        try
        {
            var dir = Path.GetDirectoryName(RecentEndpointsPath);
            if (dir != null) Directory.CreateDirectory(dir);
            File.WriteAllLines(RecentEndpointsPath, RecentEndpoints);
        }
        catch { }
    }

    private void AddToRecentEndpoints(string url)
    {
        RecentEndpoints.RemoveAll(e => e.Equals(url, StringComparison.OrdinalIgnoreCase));
        RecentEndpoints.Insert(0, url);
        while (RecentEndpoints.Count > 10) RecentEndpoints.RemoveAt(RecentEndpoints.Count - 1);
        SaveRecentEndpoints();
    }

    public async Task ConnectAsync(string endpointUrl)
    {
        ErrorMessage = null;
        AddToRecentEndpoints(endpointUrl);
        try
        {
            Disconnect();

            if (_appConfig == null)
            {
                _appConfig = new ApplicationConfiguration
                {
                    ApplicationName = "ServerEditorWebClient",
                    ApplicationUri = Utils.Format("urn:{0}:ServerEditorWebClient", System.Net.Dns.GetHostName()),
                    ApplicationType = ApplicationType.Client,
                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new CertificateIdentifier
                        {
                            StoreType = CertificateStoreType.Directory,
                            StorePath = "%LocalApplicationData%/ServerEditorWeb/pki/own",
                            SubjectName = "ServerEditorWebClient"
                        },
                        TrustedPeerCertificates = new CertificateTrustList
                        {
                            StoreType = CertificateStoreType.Directory,
                            StorePath = "%LocalApplicationData%/ServerEditorWeb/pki/trusted"
                        },
                        TrustedIssuerCertificates = new CertificateTrustList
                        {
                            StoreType = CertificateStoreType.Directory,
                            StorePath = "%LocalApplicationData%/ServerEditorWeb/pki/issuer"
                        },
                        RejectedCertificateStore = new CertificateTrustList
                        {
                            StoreType = CertificateStoreType.Directory,
                            StorePath = "%LocalApplicationData%/ServerEditorWeb/pki/rejected"
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
                    if (e.Error.StatusCode == Opc.Ua.StatusCodes.BadCertificateUntrusted)
                        e.Accept = true;
                };

                var app = new ApplicationInstance
                {
                    ApplicationName = _appConfig.ApplicationName,
                    ApplicationType = ApplicationType.Client,
                    ApplicationConfiguration = _appConfig
                };
                await app.CheckApplicationInstanceCertificates(false, 2048);
            }

            var client = DiscoveryClient.Create(new Uri(endpointUrl));
            var endpoints = client.GetEndpoints(null);
            client.Dispose();

            // Prefer opc.tcp endpoints without security, then any opc.tcp endpoint
            var endpointDescription =
                endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None
                    && e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
                ?? endpoints.FirstOrDefault(e => e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
                ?? endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None)
                ?? endpoints.FirstOrDefault();

            if (endpointDescription == null)
                throw new Exception("No endpoints found");

            // Override only the host/port in the discovered endpoint URL with what
            // the user typed, so we connect to the user's address while preserving
            // the transport scheme (opc.tcp:// vs http://) from the discovered endpoint.
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

            _session = await Session.Create(
                _appConfig, endpoint, false,
                "ServerEditorWebSession", 60000,
                new UserIdentity(new AnonymousIdentityToken()), null);

            if (_session?.Connected != true)
                throw new Exception("Session could not be created");

            // Load root
            RootNodes.Clear();
            var rootNode = new OpcBrowseNode
            {
                DisplayName = "Objects",
                NodeIdString = ObjectIds.ObjectsFolder.ToString(),
                ResolvedNodeId = ObjectIds.ObjectsFolder,
                NodeClass = "Object",
                HasChildren = true
            };
            RootNodes.Add(rootNode);
            await LoadChildrenAsync(rootNode);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            RootNodes.Clear();
        }
        NotifyChanged();
    }

    public void Disconnect()
    {
        lock (_lock)
        {
            if (_subscription != null)
            {
                try { _subscription.Delete(true); } catch { }
                _subscription = null;
            }
            MonitoredItems.Clear();

            if (_session != null)
            {
                try { _session.Close(); } catch { }
                _session = null;
            }
            RootNodes.Clear();
        }
        NotifyChanged();
    }

    public async Task LoadChildrenAsync(OpcBrowseNode parent)
    {
        if (parent.ChildrenLoaded || _session == null || !_session.Connected) return;

        try
        {
            var refs = await Task.Run(() =>
            {
                var browser = new Browser(_session)
                {
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                    IncludeSubtypes = true,
                    NodeClassMask = (int)(NodeClass.Object | NodeClass.Variable | NodeClass.Method),
                    ResultMask = (uint)BrowseResultMask.All
                };
                return browser.Browse(parent.ResolvedNodeId);
            });

            parent.Children.Clear();
            foreach (var r in refs)
            {
                var childNodeId = ExpandedNodeId.ToNodeId(r.NodeId, _session.NamespaceUris);
                parent.Children.Add(new OpcBrowseNode
                {
                    DisplayName = r.DisplayName.Text,
                    NodeIdString = childNodeId?.ToString() ?? r.NodeId.ToString(),
                    ResolvedNodeId = childNodeId ?? NodeId.Null,
                    NodeClass = r.NodeClass.ToString(),
                    BrowseName = r.BrowseName.ToString(),
                    IsVariable = r.NodeClass == NodeClass.Variable,
                    HasChildren = r.NodeClass == NodeClass.Object
                });
            }
            parent.ChildrenLoaded = true;
        }
        catch (Exception ex)
        {
            parent.Children.Clear();
            parent.Children.Add(new OpcBrowseNode { DisplayName = $"Error: {ex.Message}" });
            parent.ChildrenLoaded = true;
        }
        NotifyChanged();
    }

    public async Task SelectNodeAsync(OpcBrowseNode node)
    {
        if (_session == null || !_session.Connected) return;

        // Clean up old subscription
        lock (_lock)
        {
            if (_subscription != null)
            {
                try { _subscription.Delete(true); } catch { }
                _subscription = null;
            }
            MonitoredItems.Clear();
        }

        try
        {
            // Ensure children are loaded
            if (!node.ChildrenLoaded)
                await LoadChildrenAsync(node);

            _subscription = new Subscription(_session.DefaultSubscription)
            {
                PublishingInterval = 1000,
                PublishingEnabled = true
            };
            _session.AddSubscription(_subscription);
            _subscription.Create();

            // Monitor the node itself if it's a variable
            if (node.IsVariable)
                AddMonitoredItem(node);

            // Monitor variable children
            foreach (var child in node.Children.Where(c => c.IsVariable))
                AddMonitoredItem(child);

            _subscription.ApplyChanges();
        }
        catch (Exception ex)
        {
            MonitoredItems.Clear();
            MonitoredItems.Add(new OpcMonitoredValue { DisplayName = "Error", Value = ex.Message });
        }
        NotifyChanged();
    }

    private void AddMonitoredItem(OpcBrowseNode node)
    {
        if (_subscription == null || _session == null) return;

        var item = new MonitoredItem(_subscription.DefaultItem)
        {
            DisplayName = node.DisplayName,
            StartNodeId = node.ResolvedNodeId,
            AttributeId = Attributes.Value
        };

        var monValue = new OpcMonitoredValue
        {
            DisplayName = node.DisplayName,
            NodeIdString = node.NodeIdString,
            ResolvedNodeId = node.ResolvedNodeId
        };
        MonitoredItems.Add(monValue);

        // Check if writable
        var nodeIdForRead = node.ResolvedNodeId;
        _ = Task.Run(() =>
        {
            try
            {
                var nodesToRead = new ReadValueIdCollection
                {
                    new ReadValueId { NodeId = nodeIdForRead, AttributeId = Attributes.UserAccessLevel }
                };
                _session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead,
                    out var results, out _);
                if (results.Count > 0 && StatusCode.IsGood(results[0].StatusCode) && results[0].Value is byte b)
                    monValue.IsWritable = (b & AccessLevels.CurrentWrite) == AccessLevels.CurrentWrite;
            }
            catch { }
        });

        item.Notification += (monitoredItem, args) =>
        {
            if (args.NotificationValue is MonitoredItemNotification notification && notification.Value != null)
            {
                var sc = notification.Value.StatusCode;
                var text = Opc.Ua.StatusCodes.GetBrowseName(sc.Code);
                if (text == "Unknown") text = $"{sc} (0x{sc.Code:X8})";

                monValue.Value = notification.Value.WrappedValue.ToString() ?? "";
                monValue.StatusCode = text;
                monValue.Timestamp = notification.Value.SourceTimestamp.ToLocalTime().ToString("HH:mm:ss.fff");
                NotifyChanged();
            }
        };

        _subscription.AddItem(item);
    }

    public async Task<(bool Success, string Message)> WriteValueAsync(NodeId nodeId, string newValue)
    {
        if (_session == null || !_session.Connected)
            return (false, "Not connected");

        try
        {
            // Read the node's DataType so we can convert the string to the correct type.
            var nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId { NodeId = nodeId, AttributeId = Attributes.DataType }
            };
            DataValueCollection readResults = null!;
            DiagnosticInfoCollection readDiag = null!;
            await Task.Run(() => _session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead, out readResults, out readDiag));

            var typedValue = ConvertToExpectedType(
                readResults.Count > 0 && StatusCode.IsGood(readResults[0].StatusCode)
                    ? readResults[0].Value as NodeId
                    : null,
                newValue);

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

            if (results == null || StatusCode.IsBad(results[0]))
                return (false, $"Write failed: {results[0]}");

            return (true, "Value written");
        }
        catch (Exception ex)
        {
            return (false, $"Write error: {ex.Message}");
        }
    }

    private static object ConvertToExpectedType(NodeId? dataTypeId, string value)
    {
        if (dataTypeId == null)
            return value;

        var id = dataTypeId.Identifier is uint uid ? uid : 0u;
        return id switch
        {
            DataTypes.Boolean => bool.Parse(value),
            DataTypes.SByte => sbyte.Parse(value),
            DataTypes.Byte => byte.Parse(value),
            DataTypes.Int16 => short.Parse(value),
            DataTypes.UInt16 => ushort.Parse(value),
            DataTypes.Int32 => int.Parse(value),
            DataTypes.UInt32 => uint.Parse(value),
            DataTypes.Int64 => long.Parse(value),
            DataTypes.UInt64 => ulong.Parse(value),
            DataTypes.Float => float.Parse(value),
            DataTypes.Double => double.Parse(value),
            DataTypes.DateTime => DateTime.Parse(value),
            _ => value
        };
    }

    public void Dispose()
    {
        Disconnect();
    }
}
