// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using SharedModels;

namespace ServerEditorWeb.Services;

/// <summary>
/// A lightweight OPC UA subscription service for the Live Tag Browser.
/// Polls all project variable paths and exposes their current values with timestamps.
/// Shares the same OPC UA connection pattern as WatchTableService.
/// </summary>
public class LiveTagEntry
{
    public string Path { get; set; } = "";
    public string Value { get; set; } = "";
    public string Quality { get; set; } = "";
    public string Timestamp { get; set; } = "";
    public bool IsConnected { get; set; }
    internal NodeId? ResolvedNodeId { get; set; }
}

public class LiveTagService : IDisposable
{
    private const string ServerNamespaceUri = "http://simpleopcfileserver.org/UA";

    private Session? _session;
    private Subscription? _subscription;
    private ApplicationConfiguration? _appConfig;
    private ushort _nsIndex;
    private string _lastEndpoint = "";

    public List<LiveTagEntry> Entries { get; } = new();
    public bool IsConnected => _session?.Connected == true;
    public string? Error { get; private set; }

    public event Action? StateChanged;
    private void NotifyChanged() => StateChanged?.Invoke();

    /// <summary>
    /// Connect to the OPC server and subscribe to all project variables.
    /// </summary>
    public async Task ConnectAsync(string endpointUrl, NodeModel model)
    {
        Error = null;
        try
        {
            if (_session?.Connected == true && _lastEndpoint == endpointUrl)
            {
                // Refresh variable list without reconnecting
                RebuildFromModel(model);
                return;
            }

            Disconnect();
            await EnsureConfigAsync();

            var client = DiscoveryClient.Create(new Uri(endpointUrl));
            var endpoints = client.GetEndpoints(null);
            client.Dispose();

            var endpointDesc =
                endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None
                    && e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
                ?? endpoints.FirstOrDefault(e => e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
                ?? endpoints.FirstOrDefault();

            if (endpointDesc == null) throw new Exception("No endpoints found");

            if (Uri.TryCreate(endpointDesc.EndpointUrl, UriKind.Absolute, out var discoveredUri) &&
                Uri.TryCreate(endpointUrl, UriKind.Absolute, out var userUri))
            {
                var builder = new UriBuilder(discoveredUri) { Host = userUri.Host, Port = userUri.Port };
                endpointDesc.EndpointUrl = builder.Uri.ToString();
            }

            var epConfig = EndpointConfiguration.Create(_appConfig);
            var endpoint = new ConfiguredEndpoint(null, endpointDesc, epConfig);

            _session = await Session.Create(
                _appConfig!, endpoint, false,
                "LiveTagSession", 60000,
                new UserIdentity(new AnonymousIdentityToken()), null);

            if (_session?.Connected != true) throw new Exception("Session could not be created");

            _lastEndpoint = endpointUrl;
            _nsIndex = (ushort)_session.NamespaceUris.GetIndex(ServerNamespaceUri);

            RebuildFromModel(model);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            foreach (var e in Entries) { e.IsConnected = false; e.Quality = "Disconnected"; }
        }
        NotifyChanged();
    }

    private void RebuildFromModel(NodeModel model)
    {
        // Collect all variable paths from the model
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (model?.Folder != null)
            CollectPaths(model.Folder, "", paths);

        Entries.Clear();
        foreach (var path in paths.OrderBy(p => p))
            Entries.Add(new LiveTagEntry { Path = path });

        RebuildSubscription();
    }

    private static void CollectPaths(Folder folder, string prefix, HashSet<string> paths)
    {
        var folderPath = string.IsNullOrEmpty(prefix) ? "" : prefix;
        foreach (var v in folder.Variables)
        {
            var fullPath = string.IsNullOrEmpty(folderPath) ? v.Name : $"{folderPath}.{v.Name}";
            paths.Add(fullPath);
        }
        foreach (var sub in folder.Folders)
        {
            var subPath = string.IsNullOrEmpty(folderPath) ? sub.Name : $"{folderPath}.{sub.Name}";
            CollectPaths(sub, subPath, paths);
        }
    }

    private void RebuildSubscription()
    {
        if (_session == null || !_session.Connected) return;

        // Clear existing subscription
        if (_subscription != null)
        {
            try { _subscription.Delete(true); } catch { }
            _subscription = null;
        }

        if (Entries.Count == 0) return;

        _subscription = new Subscription(_session.DefaultSubscription)
        {
            PublishingInterval = 1000,
            LifetimeCount = 100,
            KeepAliveCount = 10,
            PublishingEnabled = true,
            Priority = 0
        };

        foreach (var entry in Entries)
        {
            var nodeId = ResolveNodeId(entry.Path);
            entry.ResolvedNodeId = nodeId;

            var item = new MonitoredItem(_subscription.DefaultItem)
            {
                DisplayName = entry.Path,
                StartNodeId = nodeId,
                AttributeId = Attributes.Value,
                SamplingInterval = 1000,
                QueueSize = 1,
                DiscardOldest = true
            };
            item.Notification += (monItem, args) =>
            {
                foreach (var val in monItem.DequeueValues())
                {
                    entry.Value = val.Value?.ToString() ?? "";
                    entry.Quality = val.StatusCode.ToString();
                    entry.Timestamp = val.SourceTimestamp.ToLocalTime().ToString("HH:mm:ss");
                    entry.IsConnected = true;
                }
                NotifyChanged();
            };
            _subscription.AddItem(item);
        }

        _session.AddSubscription(_subscription);
        _subscription.Create();
        _subscription.ApplyChanges();
    }

    private NodeId ResolveNodeId(string variablePath)
    {
        // Match the server's node ID scheme: ns=<index>;s=<path>
        return new NodeId(variablePath, _nsIndex);
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

    private async Task EnsureConfigAsync()
    {
        if (_appConfig != null) return;
        _appConfig = new ApplicationConfiguration
        {
            ApplicationName = "LiveTagClient",
            ApplicationUri = Utils.Format("urn:{0}:LiveTagClient", System.Net.Dns.GetHostName()),
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Directory,
                    StorePath = "%LocalApplicationData%/ServerEditorWeb/pki/own",
                    SubjectName = "LiveTagClient"
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

    public void Dispose()
    {
        Disconnect();
    }
}
