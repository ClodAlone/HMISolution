// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

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
                var item = new OpcUaItem { Variable = variable, Config = opcConfig };
                device.AddItem(item);
                variable.OnSimpleWriteValue = (ISystemContext ctx, NodeState node, ref Variant value) => device.Write(item, value);
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

            // Maximum number of NodeIds to request in a single OPC UA Read service call.
            // Kept well under typical server MaxNodesPerRead limits (commonly 1000+).
            private const int MaxNodesPerRead = 200;

            public void Start()
            {
                lock (_deviceLock)
                {
                    // The timer callback kicks off an async poll and returns immediately;
                    // the Timer thread is never blocked waiting on network I/O.
                    if (_timer == null && _items.Count > 0)
                        _timer = new Timer(_ => _ = PollAsync(), null, _pollInterval, Timeout.Infinite);
                }
            }

            private async Task PollAsync()
            {
                if (_disposed) return;
                var _sw = System.Diagnostics.Stopwatch.StartNew();
                bool anyError = false;
                List<OpcUaItem> itemsToPoll;
                lock (_deviceLock) { if (_disposed) return; itemsToPoll = new List<OpcUaItem>(_items); }

                try
                {
                    await EnsureConnectedAsync().ConfigureAwait(false);
                    if (_session == null || !_session.Connected)
                    {
                        foreach (var item in itemsToPoll) UpdateError(item.Variable, "Not connected");
                        return;
                    }

                    // OPC UA's Read service supports reading many NodeIds in a single request.
                    // Instead of issuing one ReadValueAsync round-trip per item, batch all items
                    // belonging to this session into chunked ReadValueIdCollection requests.
                    foreach (var chunk in itemsToPoll.Chunk(MaxNodesPerRead))
                    {
                        var nodesToRead = new List<ReadValueId>();
                        foreach (var item in chunk)
                        {
                            try
                            {
                                nodesToRead.Add(new ReadValueId
                                {
                                    NodeId = Opc.Ua.NodeId.Parse(item.Config.NodeId),
                                    AttributeId = Attributes.Value
                                });
                            }
                            catch (Exception ex)
                            {
                                UpdateError(item.Variable, ex.Message);
                                anyError = true;
                            }
                        }
                        if (nodesToRead.Count == 0) continue;

                        try
                        {
                            var response = await _session.ReadAsync(null, 0, TimestampsToReturn.Both, nodesToRead, default).ConfigureAwait(false);
                            var results = response.Results;
                            for (int i = 0; i < chunk.Length && i < results.Count; i++)
                            {
                                var dv = results[i];
                                if (StatusCode.IsGood(dv.StatusCode)) Update(chunk[i].Variable, dv.WrappedValue, dv.StatusCode);
                                else UpdateError(chunk[i].Variable, dv.StatusCode.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message != _lastLoggedError)
                            {
                                Log.Error(ex, "OPC UA client batch read error ({Count} nodes): {Message}", nodesToRead.Count, ex.Message);
                                _lastLoggedError = ex.Message;
                            }
                            else Log.Debug("OPC UA client batch read error (repeated, {Count} nodes): {Message}", nodesToRead.Count, ex.Message);
                            _onError?.Invoke(_endpointUrl, $"Batch read error: {ex.Message}");
                            foreach (var item in chunk) UpdateError(item.Variable, ex.Message);
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

            private async Task EnsureConnectedAsync()
            {
                if (_session != null && _session.Connected) return;
                Disconnect();
                _config ??= CreateClientConfiguration();
                var endpointDescription = GetEndpointDescription(_endpointUrl);
                var endpointConfiguration = EndpointConfiguration.Create(_config);
                var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
                _session = await new DefaultSessionFactory().CreateAsync(_config, endpoint, false,
                    $"SimpleOpcFileServer-{Utils.GetHostName()}", 60000,
                    new UserIdentity(new AnonymousIdentityToken()), default).ConfigureAwait(false) as Session;
            }

            private EndpointDescription GetEndpointDescription(string url)
            {
                using var client = DiscoveryClient.Create(_config, new Uri(url));
                var endpoints = client.GetEndpoints(default(ArrayOf<string>)).ToList();
                return endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None)
                       ?? endpoints.FirstOrDefault()
                       ?? throw new ServiceResultException(StatusCodes.BadConfigurationError, "No endpoints found");
            }

            private static ApplicationConfiguration CreateClientConfiguration()
            {
                var pkiRoot = "%LocalApplicationData%/SimpleOpcFileServer/pki";
                var telemetry = new ServiceProviderTelemetryContext(new ServiceCollection().BuildServiceProvider());
                var config = new ApplicationConfiguration(telemetry)
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
                if (config.CertificateManager is CertificateManager certManager)
                {
                    certManager.AcceptError = (_, result) => result.StatusCode == StatusCodes.BadCertificateUntrusted;
                }
                return config;
            }

            private void Disconnect()
            {
                try { _session?.CloseAsync().GetAwaiter().GetResult(); } catch { }
                try { _session?.Dispose(); } catch { }
                _session = null;
            }

            public ServiceResult Write(OpcUaItem item, Variant value)
            {
                try
                {
                    EnsureConnectedAsync().GetAwaiter().GetResult();
                    if (_session == null || !_session.Connected)
                        return ServiceResult.Create(StatusCodes.BadNotConnected, "OPC UA session not connected");

                    var nodeId = NodeId.Parse(item.Config.NodeId);
                    var writeValue = new WriteValue
                    {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value,
                        Value = new DataValue(value)
                    };

                    var collection = new List<WriteValue> { writeValue };
                    var response = _session.WriteAsync(null, collection, default).GetAwaiter().GetResult();
                    var results = response.Results;
                    var writeStatus = response.ResponseHeader?.ServiceResult ?? StatusCodes.Good;
                    if (StatusCode.IsBad(writeStatus) || results.Count == 0 || StatusCode.IsBad(results[0]))
                    {
                        var detail = results.Count > 0 ? results[0].ToString() : writeStatus.ToString();
                        return ServiceResult.Create(writeStatus, detail);
                    }

                    item.Variable.Value = value; item.Variable.StatusCode = StatusCodes.Good;
                    item.Variable.Timestamp = DateTime.UtcNow; item.Variable.ClearChangeMasks(_context, false);
                    return ServiceResult.Good;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "OPC UA write error for {Endpoint}: {Message}", _endpointUrl, ex.Message);
                    _onError?.Invoke(_endpointUrl, $"Write error: {ex.Message}");
                    return ServiceResult.Create(ex, StatusCodes.BadUnexpectedError, ex.Message);
                }
            }

            private void Update(BaseDataVariableState variable, Variant value, StatusCode statusCode)
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
