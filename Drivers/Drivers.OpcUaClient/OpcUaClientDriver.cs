using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Client;
using Serilog;

namespace SimpleOpcFileServer
{
    public class OpcUaClientConfig
    {
        public string EndpointUrl { get; set; } = "opc.tcp://localhost:4840";
        public string NodeId { get; set; } = "ns=2;s=Demo.Static.Scalar.Double";
        public int PollTime { get; set; } = 1000;
    }

    public class OpcUaClientDriver : IDriver
    {
        public string Key => "OpcUaClient";
        public event Action<string, string>? OnError;
        public event Action<double>? OnCycleCompleted;

        private readonly Dictionary<string, OpcUaDevice> _devices = new();
        private readonly object _lock = new();
        private readonly ISystemContext _context;

        public OpcUaClientDriver(ISystemContext context) { _context = context; }

        private void RaiseError(string source, string message) => OnError?.Invoke(source, message);
        private void RaiseCycle(double ms) => OnCycleCompleted?.Invoke(ms);

        public void AddItem(BaseDataVariableState variable, string configJson)
        {
            var opcConfig = JsonSerializer.Deserialize<OpcUaClientConfig>(configJson);
            if (opcConfig == null) return;

            lock (_lock)
            {
                if (!_devices.TryGetValue(opcConfig.EndpointUrl, out var device))
                {
                    device = new OpcUaDevice(opcConfig.EndpointUrl, _context, RaiseError, RaiseCycle);
                    _devices[opcConfig.EndpointUrl] = device;
                }
                device.AddItem(new OpcUaItem { Variable = variable, Config = opcConfig });
            }
        }

        public void Start()
        {
            lock (_lock) { foreach (var d in _devices.Values) d.Start(); }
        }

        public void Dispose()
        {
            lock (_lock) { foreach (var d in _devices.Values) d.Dispose(); _devices.Clear(); }
        }

        private sealed class OpcUaDevice : IDisposable
        {
            private readonly string _endpointUrl;
            private readonly ISystemContext _context;
            private readonly Action<string, string>? _onError;
            private readonly Action<double>? _onCycle;
            private readonly List<OpcUaItem> _items = new();
            private readonly object _deviceLock = new();
            private Timer? _timer;
            private int _pollInterval = 1000;
            private int _minPollTime = int.MaxValue;
            private Session? _session;
            private ApplicationConfiguration? _config;
            private bool _disposed;
            private string? _lastLoggedError;
            private int _consecutiveErrors;
            private const int MaxBackoffMs = 30_000;

            public OpcUaDevice(string endpointUrl, ISystemContext context, Action<string, string>? onError = null, Action<double>? onCycle = null) { _endpointUrl = endpointUrl; _context = context; _onError = onError; _onCycle = onCycle; }

            public void AddItem(OpcUaItem item)
            {
                lock (_deviceLock)
                {
                    _items.Add(item);
                    if (item.Config.PollTime > 0 && item.Config.PollTime < _minPollTime)
                        _minPollTime = item.Config.PollTime;
                    _pollInterval = _minPollTime < int.MaxValue ? _minPollTime : 1000;
                }
            }

            public void Start()
            {
                lock (_deviceLock)
                {
                    if (_timer == null && _items.Count > 0)
                        _timer = new Timer(Poll, null, _pollInterval, Timeout.Infinite);
                }
            }

            private void Poll(object? state)
            {
                if (_disposed) return;
                var _sw = System.Diagnostics.Stopwatch.StartNew();
                bool anyError = false;
                List<OpcUaItem> itemsToPoll;
                lock (_deviceLock) { if (_disposed) return; itemsToPoll = new List<OpcUaItem>(_items); }

                try
                {
                    EnsureConnected();
                    if (_session == null || !_session.Connected)
                    {
                        foreach (var item in itemsToPoll) UpdateError(item.Variable, "Not connected");
                        return;
                    }
                    foreach (var item in itemsToPoll)
                    {
                        try
                        {
                            var nodeId = Opc.Ua.NodeId.Parse(item.Config.NodeId);
                            var dv = _session.ReadValue(nodeId);
                            Update(item.Variable, dv.Value, dv.StatusCode);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message != _lastLoggedError)
                            {
                                Log.Error(ex, "OPC UA client read error for {NodeId}: {Message}", item.Config.NodeId, ex.Message);
                                _lastLoggedError = ex.Message;
                            }
                            else Log.Debug("OPC UA client read error (repeated) for {NodeId}: {Message}", item.Config.NodeId, ex.Message);
                            _onError?.Invoke(_endpointUrl, $"Read error ({item.Config.NodeId}): {ex.Message}");
                            UpdateError(item.Variable, ex.Message);
                            anyError = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message != _lastLoggedError)
                    {
                        Log.Error(ex, "OPC UA client poll error for {Endpoint}: {Message}", _endpointUrl, ex.Message);
                        _lastLoggedError = ex.Message;
                    }
                    else Log.Debug("OPC UA client poll error (repeated) for {Endpoint}: {Message}", _endpointUrl, ex.Message);
                    _onError?.Invoke(_endpointUrl, $"Poll error: {ex.Message}");
                    foreach (var item in itemsToPoll) UpdateError(item.Variable, ex.Message);
                    Disconnect();
                    anyError = true;
                }
                finally
                {
                    if (anyError) _consecutiveErrors++; else { _consecutiveErrors = 0; _lastLoggedError = null; }
                    var delay = _consecutiveErrors > 0 ? Math.Min(_pollInterval * (1 << Math.Min(_consecutiveErrors, 10)), MaxBackoffMs) : _pollInterval;
                    _onCycle?.Invoke(_sw.Elapsed.TotalMilliseconds);
                    if (!_disposed) _timer?.Change(delay, Timeout.Infinite);
                }
            }

            private void EnsureConnected()
            {
                if (_session != null && _session.Connected) return;
                Disconnect();
                _config ??= CreateClientConfiguration();
                var endpointDescription = GetEndpointDescription(_endpointUrl);
                var endpointConfiguration = EndpointConfiguration.Create(_config);
                var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
                _session = Session.Create(_config, endpoint, false,
                    $"SimpleOpcFileServer-{Utils.GetHostName()}", 60000,
                    new UserIdentity(new AnonymousIdentityToken()), null).GetAwaiter().GetResult();
            }

            private static EndpointDescription GetEndpointDescription(string url)
            {
                using var client = DiscoveryClient.Create(new Uri(url));
                var endpoints = client.GetEndpoints(null);
                return endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None)
                       ?? endpoints.FirstOrDefault()
                       ?? throw new ServiceResultException(StatusCodes.BadConfigurationError, "No endpoints found");
            }

            private static ApplicationConfiguration CreateClientConfiguration()
            {
                var pkiRoot = "%LocalApplicationData%/SimpleOpcFileServer/pki";
                var config = new ApplicationConfiguration
                {
                    ApplicationName = "SimpleOpcFileServer.OpcUaClient",
                    ApplicationUri = $"urn:{Utils.GetHostName()}:SimpleOpcFileServer:OpcUaClient",
                    ApplicationType = ApplicationType.Client,
                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new CertificateIdentifier { StoreType = CertificateStoreType.Directory, StorePath = $"{pkiRoot}/own", SubjectName = "SimpleOpcFileServer" },
                        TrustedPeerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = $"{pkiRoot}/trusted" },
                        TrustedIssuerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = $"{pkiRoot}/issuer" },
                        RejectedCertificateStore = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = $"{pkiRoot}/rejected" }
                    },
                    TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                    ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
                };
                config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
                config.CertificateValidator.CertificateValidation += (_, e) => { if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted) e.Accept = true; };
                return config;
            }

            private void Disconnect()
            {
                try { _session?.Close(); } catch { }
                try { _session?.Dispose(); } catch { }
                _session = null;
            }

            private void Update(BaseDataVariableState variable, object? value, StatusCode statusCode)
            {
                variable.Value = value; variable.StatusCode = statusCode;
                variable.Timestamp = DateTime.UtcNow; variable.ClearChangeMasks(_context, false);
            }

            private void UpdateError(BaseDataVariableState variable, string? message = null)
            {
                variable.StatusCode = StatusCodes.Bad; variable.Timestamp = DateTime.UtcNow;
                variable.ClearChangeMasks(_context, false);
                if (!string.IsNullOrEmpty(message))
                {
                    var n = variable.FindChild(_context, new QualifiedName("LastError", variable.BrowseName.NamespaceIndex));
                    if (n is BaseVariableState v) { v.Value = message; v.Timestamp = DateTime.UtcNow; v.ClearChangeMasks(_context, false); }
                }
            }

            public void Dispose() { _disposed = true; _timer?.Dispose(); Disconnect(); }
        }

        private sealed class OpcUaItem
        {
            public BaseDataVariableState Variable { get; set; } = null!;
            public OpcUaClientConfig Config { get; set; } = null!;
        }
    }
}
