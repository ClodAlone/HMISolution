using Opc.Ua;
using Opc.Ua.Client;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace ChatApp9.Web.Services;

public class OpcUaService : IAsyncDisposable
{
    private readonly ApplicationConfiguration _config;
    private readonly ILogger<OpcUaService> _logger;
    private readonly OpcUaSettings _settings;
    private readonly ConcurrentDictionary<string, Session> _sessions = new();
    private readonly ConcurrentDictionary<string, List<(string Id, string Name)>> _nodeCache = new();

    public OpcUaService(ILogger<OpcUaService> logger, IOptions<OpcUaSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
        _config = new ApplicationConfiguration
        {
            ApplicationName = "AI Client",
            ApplicationUri = Utils.Format(@"urn:{0}:AIClient", System.Net.Dns.GetHostName()),
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = "AI Client" },
                TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                AutoAcceptUntrustedCertificates = true
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
        };
        _config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
        if (_config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
        {
            _config.CertificateValidator.CertificateValidation += (s, e) => 
            { 
                e.Accept = true;
            };
        }
    }

    // Tool 1: Search for nodes across servers and return a list of identifiers
    public async Task<List<string>> SearchOpcNodesAsync(string searchString)
    {
        var results = new List<string>();

        if (_settings.Servers == null || _settings.Servers.Count == 0)
        {
             results.Add("No OPC UA servers configured.");
             return results;
        }

        foreach (var server in _settings.Servers)
        {
            try
            {
                // Ensure the cache is populated for this server
                if (!_nodeCache.TryGetValue(server.EndpointUrl, out var cachedNodes))
                {
                    cachedNodes = new List<(string Id, string Name)>();
                    var session = await GetSessionAsync(server.EndpointUrl);
                    if (session != null)
                    {
                        // Index the server (browsing for all items)
                        // Use Task.Run to avoid blocking the calling thread during heavy browsing
                        await Task.Run(() => RecursiveBrowse(session, ObjectIds.ObjectsFolder, 5, cachedNodes));
                        _nodeCache[server.EndpointUrl] = cachedNodes;
                    }
                }

                // Filter results from cache
                var searchKeywords = searchString.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                
                // Build a list of target strings: if keyword matches an alias, use alias value; otherwise use keyword
                var targets = new List<string>();

                foreach (var keyword in searchKeywords)
                {
                    string target = keyword;
                    if (server.Aliases != null)
                    {
                         foreach (var alias in server.Aliases)
                         {
                             if (alias.Key.Equals(keyword, StringComparison.OrdinalIgnoreCase))
                             {
                                 target = alias.Value;
                                 break;
                             }
                         }
                    }
                    targets.Add(target);
                }

                if (cachedNodes != null)
                {
                    var matches = cachedNodes.Where(node => 
                        targets.All(t => node.Name.Contains(t, StringComparison.OrdinalIgnoreCase)));

                    foreach (var node in matches)
                    {
                        results.Add($"Server: {server.EndpointUrl} | Name: {node.Name} | NodeId: {node.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to search on {Url}", server.EndpointUrl);
            }
        }

        if (results.Count == 0)
        {
            results.Add($"No items found matching '{searchString}' in any configured server.");
        }

        return results;
    }

    // Tool 2: Read payload value given a specific Server and NodeId
    public async Task<string> ReadOpcNodeValueAsync(string serverUrl, string nodeIdString)
    {
        try
        {
            var session = await GetSessionAsync(serverUrl);
            if (session == null) return $"Failed to connect to {serverUrl}";
            
            var nodeId = NodeId.Parse(nodeIdString);
            var val = session.ReadValue(nodeId);
            
            return val.Value != null ? val.Value.ToString() : "null";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading node {NodeId} from {Url}", nodeIdString, serverUrl);
            return $"Error: {ex.Message}";
        }
    }

    private async Task<Session?> GetSessionAsync(string serverUrl)
    {
        if (_sessions.TryGetValue(serverUrl, out var session))
        {
            if (session.Connected)
            {
                return session;
            }
            _sessions.TryRemove(serverUrl, out _);
            session.Dispose();
        }

        try
        {
            var newSession = await CreateSessionInternalAsync(serverUrl);
            if (newSession != null)
            {
                _sessions[serverUrl] = newSession;
            }
            return newSession;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session for {Url}", serverUrl);
            return null;
        }
    }

    private async Task<Session?> CreateSessionInternalAsync(string serverUrl)
    {
         var uri = new Uri(serverUrl);
         using var discoveryClient = DiscoveryClient.Create(uri);
         var endpoints = discoveryClient.GetEndpoints(null);
         
         var endpoint = endpoints.OrderBy(e => e.SecurityLevel).FirstOrDefault();
         
         if (endpoint == null) return null;

         // Ensure endpoint application URI validation doesn't fail if mismatch (fixes [80170000])
         endpoint.Server.ApplicationUri = null;

         var endpointConfiguration = EndpointConfiguration.Create(_config);
         var configuredEndpoint = new ConfiguredEndpoint(null, endpoint, endpointConfiguration);
         
         return await Session.Create(_config, configuredEndpoint, false, "", 60000, null, null);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var session in _sessions.Values)
        {
            session.Dispose();
        }
        _sessions.Clear();
    }

    private void RecursiveBrowse(Session session, NodeId startNode, int depth, List<(string Id, string Name)> results)
    {
        if (depth <= 0) return;

        var browser = new Browser(session)
        {
            BrowseDirection = BrowseDirection.Forward,
            ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
            IncludeSubtypes = true,
            NodeClassMask = 0
        };

        ReferenceDescriptionCollection? references = null;
        try 
        {
             references = browser.Browse(startNode);
        }
        catch 
        {
            return;
        }

        if (references == null) return;

        foreach (var reference in references)
        {
            var nodeId = ExpandedNodeId.ToNodeId(reference.NodeId, session.NamespaceUris);

            NodeId nodeIdServer = ExpandedNodeId.ToNodeId(reference.NodeId, session.NamespaceUris);

            if (nodeIdServer == ObjectIds.Server)
            {
                continue;
            }

            if (reference.NodeClass == NodeClass.Variable)
                results.Add((nodeId.ToString(), reference.DisplayName.Text));
        }

        foreach (var reference in references)
        {
             try 
             {
                 var expandedNodeId = reference.NodeId;
                 var nodeId = ExpandedNodeId.ToNodeId(expandedNodeId, session.NamespaceUris);

                if (reference.NodeClass == NodeClass.Object)
                    RecursiveBrowse(session, nodeId, depth - 1, results);
             }
             catch {}
        }
    }
}
