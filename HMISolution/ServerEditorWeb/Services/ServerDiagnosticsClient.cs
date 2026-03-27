using System.Text.Json;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using SharedModels;

namespace ServerEditorWeb.Services;

/// <summary>
/// Reads the server's diagnostics snapshot from the OPC UA _Diagnostics.Json variable.
/// All OPC UA work runs on a background thread to avoid blocking the Blazor UI.
/// </summary>
public class ServerDiagnosticsClient : IDisposable
{
    private const string ServerNamespaceUri = "http://simpleopcfileserver.org/UA";
    private const string DiagNodeIdentifier = "_Diagnostics.Json";

    private Session? _session;
    private ApplicationConfiguration? _appConfig;
    private string? _lastEndpoint;
    private string? _lastUsername;
    private NodeId? _diagNodeId;
    private int _polling;               // 0 = idle, 1 = busy (interlocked guard)
    private DateTime _nextConnectAttempt = DateTime.MinValue;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public ServerDiagnostics? Latest { get; private set; }
    public string? Error { get; private set; }

    public async Task PollAsync(string endpointUrl, string? username = null, string? password = null)
    {
        if (string.IsNullOrEmpty(endpointUrl))
        {
            Latest = null;
            Error = "No OPC UA endpoint configured.";
            return;
        }

        // Skip if a previous poll is still running
        if (Interlocked.CompareExchange(ref _polling, 1, 0) != 0)
            return;

        try
        {
            // Run all OPC UA work off the calling (Blazor) thread
            await Task.Run(async () => await PollCoreAsync(endpointUrl, username, password));
        }
        finally
        {
            Interlocked.Exchange(ref _polling, 0);
        }
    }

    private async Task PollCoreAsync(string endpointUrl, string? username, string? password)
    {
        try
        {
            // Back off when connection is failing — don't hammer every 2 seconds
            if (_session == null || !_session.Connected)
            {
                if (DateTime.UtcNow < _nextConnectAttempt)
                    return; // keep existing Latest/Error, skip this cycle
            }

            await EnsureConnectedAsync(endpointUrl, username, password);

            if (_session == null || !_session.Connected)
            {
                Latest = null;
                Error = "OPC UA session not connected.";
                return;
            }

            // Resolve the diagnostics node ID from the server's namespace table
            if (_diagNodeId == null)
            {
                var nsIndex = _session.NamespaceUris.GetIndex(ServerNamespaceUri);
                if (nsIndex < 0)
                {
                    Latest = null;
                    Error = $"Namespace '{ServerNamespaceUri}' not found on server.";
                    return;
                }
                _diagNodeId = new NodeId(DiagNodeIdentifier, (ushort)nsIndex);
            }

            var value = _session.ReadValue(_diagNodeId);

            if (StatusCode.IsGood(value.StatusCode) && value.Value is string json)
            {
                Latest = JsonSerializer.Deserialize<ServerDiagnostics>(json, _jsonOptions);
                Error = null;
            }
            else
            {
                Latest = null;
                Error = $"Read failed: {value.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            Latest = null;
            Error = ex.Message;
            try { _session?.Close(); } catch { }
            _session = null;
            _diagNodeId = null;
            // Back off: wait 10 seconds before trying to connect again
            _nextConnectAttempt = DateTime.UtcNow.AddSeconds(10);
        }
    }

    private async Task EnsureConnectedAsync(string endpointUrl, string? username, string? password)
    {
        // Already connected to the same endpoint
        if (_session?.Connected == true && _lastEndpoint == endpointUrl && _lastUsername == username)
            return;

        // Disconnect from previous
        try { _session?.Close(); } catch { }
        _session = null;
        _diagNodeId = null;

        if (_appConfig == null)
        {
            _appConfig = new ApplicationConfiguration
            {
                ApplicationName = "ServerEditorDiagClient",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:ServerEditorDiagClient",
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = CertificateStoreType.Directory,
                        StorePath = "%LocalApplicationData%/ServerEditorWeb/pki/own",
                        SubjectName = "ServerEditorDiagClient"
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
                TransportQuotas = new TransportQuotas { OperationTimeout = 3000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 30000 },
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
            ?? endpoints.FirstOrDefault();

        if (endpointDescription == null)
            throw new Exception("No OPC UA endpoints found");

        var endpointConfiguration = EndpointConfiguration.Create(_appConfig);
        var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

        _session = await Session.Create(
            _appConfig, endpoint, false,
            "DiagSession", 30000,
            CreateIdentity(username, password), null);

        _lastEndpoint = endpointUrl;
        _lastUsername = username;
        _nextConnectAttempt = DateTime.MinValue; // reset backoff on success
    }

    private static UserIdentity CreateIdentity(string? username, string? password)
    {
        if (!string.IsNullOrEmpty(username))
        {
            var token = new UserNameIdentityToken
            {
                UserName = username,
                DecryptedPassword = System.Text.Encoding.UTF8.GetBytes(password ?? "")
            };
            return new UserIdentity(token);
        }
        return new UserIdentity(new AnonymousIdentityToken());
    }

    public void Dispose()
    {
        try { _session?.Close(); } catch { }
        _session = null;
    }
}
