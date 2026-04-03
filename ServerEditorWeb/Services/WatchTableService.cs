using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System.Text.Json;

namespace ServerEditorWeb.Services;

/// <summary>
/// A single entry in the watch table — maps a project variable path to a live OPC subscription.
/// </summary>
public class WatchEntry
{
    public string VariablePath { get; set; } = "";
    public string Value { get; set; } = "";
    public string Quality { get; set; } = "";
    public string Timestamp { get; set; } = "";
    public bool IsWritable { get; set; }
    public bool IsConnected { get; set; }

    // internal OPC state (not serialized)
    internal NodeId? ResolvedNodeId { get; set; }
}

/// <summary>
/// Named watch list that can be saved/loaded per project.
/// </summary>
public class WatchList
{
    public string Name { get; set; } = "Default";
    public List<string> VariablePaths { get; set; } = new();
}

/// <summary>
/// Manages a set of OPC UA subscriptions for project variable paths.
/// Connects to the server using the same endpoint as the editor, resolves
/// variable paths to NodeIds using the server namespace, and maintains
/// live value updates via OPC subscriptions.
/// </summary>
public class WatchTableService : IDisposable
{
    private const string ServerNamespaceUri = "http://simpleopcfileserver.org/UA";
    private static readonly string WatchListDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                     "SimpleOpcFileServer", "watchlists");

    private Session? _session;
    private Subscription? _subscription;
    private ApplicationConfiguration? _appConfig;
    private ushort _nsIndex;
    private string _lastEndpoint = "";

    public List<WatchEntry> Entries { get; } = new();
    public List<WatchList> SavedLists { get; } = new();
    public string? ActiveListName { get; set; }
    public bool IsConnected => _session?.Connected == true;
    public string? Error { get; private set; }

    public event Action? StateChanged;
    private void NotifyChanged() => StateChanged?.Invoke();

    /// <summary>
    /// Connect to the OPC server and start monitoring all current entries.
    /// </summary>
    public async Task ConnectAsync(string endpointUrl)
    {
        Error = null;
        try
        {
            if (_session?.Connected == true && _lastEndpoint == endpointUrl)
                return; // already connected to the same endpoint

            Disconnect();

            if (_appConfig == null)
            {
                _appConfig = new ApplicationConfiguration
                {
                    ApplicationName = "WatchTableClient",
                    ApplicationUri = Utils.Format("urn:{0}:WatchTableClient", System.Net.Dns.GetHostName()),
                    ApplicationType = ApplicationType.Client,
                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new CertificateIdentifier
                        {
                            StoreType = CertificateStoreType.Directory,
                            StorePath = "%LocalApplicationData%/ServerEditorWeb/pki/own",
                            SubjectName = "WatchTableClient"
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

            var endpointDescription =
                endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None
                    && e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
                ?? endpoints.FirstOrDefault(e => e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
                ?? endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None)
                ?? endpoints.FirstOrDefault();

            if (endpointDescription == null)
                throw new Exception("No endpoints found");

            if (Uri.TryCreate(endpointDescription.EndpointUrl, UriKind.Absolute, out var discoveredUri) &&
                Uri.TryCreate(endpointUrl, UriKind.Absolute, out var userUri))
            {
                var builder = new UriBuilder(discoveredUri) { Host = userUri.Host, Port = userUri.Port };
                endpointDescription.EndpointUrl = builder.Uri.ToString();
            }

            var endpointConfiguration = EndpointConfiguration.Create(_appConfig);
            var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

            _session = await Session.Create(
                _appConfig, endpoint, false,
                "WatchTableSession", 60000,
                new UserIdentity(new AnonymousIdentityToken()), null);

            if (_session?.Connected != true)
                throw new Exception("Session could not be created");

            _lastEndpoint = endpointUrl;

            // Resolve namespace index
            _nsIndex = (ushort)_session.NamespaceUris.GetIndex(ServerNamespaceUri);

            // Subscribe to all current entries
            await RebuildSubscription();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            foreach (var e in Entries)
            {
                e.IsConnected = false;
                e.Quality = "Disconnected";
            }
        }
        NotifyChanged();
    }

    public void Disconnect()
    {
        if (_subscription != null)
        {
            try { _subscription.Delete(true); } catch { }
            _subscription = null;
        }
        if (_session != null)
        {
            try { _session.Close(); } catch { }
            _session = null;
        }
        _lastEndpoint = "";
        foreach (var e in Entries)
        {
            e.IsConnected = false;
            e.Quality = "Disconnected";
            e.Value = "";
            e.Timestamp = "";
        }
        NotifyChanged();
    }

    /// <summary>
    /// Add a variable path to the watch table. Starts monitoring if connected.
    /// </summary>
    public async Task AddVariable(string variablePath)
    {
        if (string.IsNullOrWhiteSpace(variablePath)) return;
        if (Entries.Any(e => e.VariablePath == variablePath)) return; // already watching

        var entry = new WatchEntry { VariablePath = variablePath };
        Entries.Add(entry);

        if (IsConnected)
            await RebuildSubscription();

        NotifyChanged();
    }

    /// <summary>
    /// Remove a variable from the watch table.
    /// </summary>
    public async Task RemoveVariable(string variablePath)
    {
        var entry = Entries.FirstOrDefault(e => e.VariablePath == variablePath);
        if (entry == null) return;
        Entries.Remove(entry);

        if (IsConnected)
            await RebuildSubscription();

        NotifyChanged();
    }

    public async Task RemoveAll()
    {
        Entries.Clear();
        if (IsConnected)
            await RebuildSubscription();
        NotifyChanged();
    }

    /// <summary>
    /// Write a new value to a watched variable via OPC UA.
    /// </summary>
    public async Task<(bool Success, string Message)> WriteValueAsync(WatchEntry entry, string newValue)
    {
        if (_session == null || !_session.Connected || entry.ResolvedNodeId == null)
            return (false, "Not connected");

        try
        {
            var nodesToRead = new ReadValueIdCollection
            {
                new ReadValueId { NodeId = entry.ResolvedNodeId, AttributeId = Attributes.DataType }
            };
            DataValueCollection readResults = null!;
            DiagnosticInfoCollection readDiag = null!;
            await Task.Run(() => _session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead, out readResults, out readDiag));

            var typedValue = ConvertToExpectedType(
                readResults.Count > 0 && Opc.Ua.StatusCode.IsGood(readResults[0].StatusCode)
                    ? readResults[0].Value as NodeId
                    : null,
                newValue);

            var nodesToWrite = new WriteValueCollection
            {
                new WriteValue
                {
                    NodeId = entry.ResolvedNodeId,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(new Variant(typedValue))
                }
            };

            StatusCodeCollection? results = null;
            DiagnosticInfoCollection? diagnosticInfos = null;
            await Task.Run(() => _session.Write(null, nodesToWrite, out results, out diagnosticInfos));

            if (results == null || Opc.Ua.StatusCode.IsBad(results[0]))
                return (false, $"Write failed: {results?[0]}");

            return (true, "Value written");
        }
        catch (Exception ex)
        {
            return (false, $"Write error: {ex.Message}");
        }
    }

    /// <summary>
    /// Rebuild the OPC subscription for all entries.
    /// </summary>
    private async Task RebuildSubscription()
    {
        if (_session == null || !_session.Connected) return;

        // Remove old subscription
        if (_subscription != null)
        {
            try { _subscription.Delete(true); } catch { }
            _subscription = null;
        }

        _subscription = new Subscription(_session.DefaultSubscription)
        {
            PublishingInterval = 500,
            PublishingEnabled = true
        };
        _session.AddSubscription(_subscription);
        _subscription.Create();

        foreach (var entry in Entries)
        {
            var nodeId = new NodeId(entry.VariablePath, _nsIndex);
            entry.ResolvedNodeId = nodeId;

            var mi = new MonitoredItem(_subscription.DefaultItem)
            {
                DisplayName = entry.VariablePath,
                StartNodeId = nodeId,
                AttributeId = Attributes.Value
            };

            mi.Notification += (monitoredItem, args) =>
            {
                if (args.NotificationValue is MonitoredItemNotification notification && notification.Value != null)
                {
                    var sc = notification.Value.StatusCode;
                    var text = Opc.Ua.StatusCodes.GetBrowseName(sc.Code);
                    if (text == "Unknown") text = $"{sc} (0x{sc.Code:X8})";

                    entry.Value = notification.Value.WrappedValue.ToString() ?? "";
                    entry.Quality = text;
                    entry.Timestamp = notification.Value.SourceTimestamp.ToLocalTime().ToString("HH:mm:ss.fff");
                    entry.IsConnected = true;
                    NotifyChanged();
                }
            };

            _subscription.AddItem(mi);

            // Check writability in background
            _ = Task.Run(() =>
            {
                try
                {
                    var nodesToRead = new ReadValueIdCollection
                    {
                        new ReadValueId { NodeId = nodeId, AttributeId = Attributes.UserAccessLevel }
                    };
                    _session.Read(null, 0, TimestampsToReturn.Neither, nodesToRead, out var results, out _);
                    if (results.Count > 0 && Opc.Ua.StatusCode.IsGood(results[0].StatusCode) && results[0].Value is byte b)
                        entry.IsWritable = (b & AccessLevels.CurrentWrite) == AccessLevels.CurrentWrite;
                }
                catch { }
            });
        }

        await Task.Run(() => _subscription.ApplyChanges());
    }

    #region Watch List Persistence

    public void LoadSavedLists(string projectName)
    {
        SavedLists.Clear();
        var dir = GetProjectWatchDir(projectName);
        if (!Directory.Exists(dir)) return;

        foreach (var file in Directory.GetFiles(dir, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var list = JsonSerializer.Deserialize<WatchList>(json);
                if (list != null) SavedLists.Add(list);
            }
            catch { }
        }
    }

    public void SaveList(string projectName, string listName)
    {
        var list = new WatchList
        {
            Name = listName,
            VariablePaths = Entries.Select(e => e.VariablePath).ToList()
        };

        var dir = GetProjectWatchDir(projectName);
        Directory.CreateDirectory(dir);
        var safeName = string.Join("_", listName.Split(Path.GetInvalidFileNameChars()));
        var path = Path.Combine(dir, safeName + ".json");
        File.WriteAllText(path, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));

        // Update saved lists
        SavedLists.RemoveAll(l => l.Name == listName);
        SavedLists.Add(list);
        ActiveListName = listName;
        NotifyChanged();
    }

    public async Task LoadList(string projectName, string listName)
    {
        var list = SavedLists.FirstOrDefault(l => l.Name == listName);
        if (list == null) return;

        Entries.Clear();
        foreach (var path in list.VariablePaths)
            Entries.Add(new WatchEntry { VariablePath = path });

        ActiveListName = listName;

        if (IsConnected)
            await RebuildSubscription();

        NotifyChanged();
    }

    public void DeleteList(string projectName, string listName)
    {
        var dir = GetProjectWatchDir(projectName);
        var safeName = string.Join("_", listName.Split(Path.GetInvalidFileNameChars()));
        var path = Path.Combine(dir, safeName + ".json");
        if (File.Exists(path)) File.Delete(path);
        SavedLists.RemoveAll(l => l.Name == listName);
        if (ActiveListName == listName) ActiveListName = null;
        NotifyChanged();
    }

    private static string GetProjectWatchDir(string projectName)
    {
        var safe = string.Join("_", projectName.Split(Path.GetInvalidFileNameChars()));
        return Path.Combine(WatchListDir, safe);
    }

    #endregion

    private static object ConvertToExpectedType(NodeId? dataTypeId, string value)
    {
        if (dataTypeId == null) return value;
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
