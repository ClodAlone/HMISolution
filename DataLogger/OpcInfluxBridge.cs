using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Opc.Ua;
using Opc.Ua.Client;

namespace DataLogger
{
    public class OpcInfluxConfig
    {
        public string OpcServerUrl { get; set; } = "opc.tcp://localhost:4840";
        public List<string> NodeIds { get; set; } = new List<string>();
        
        public string InfluxUrl { get; set; } = "http://localhost:8086";
        public string InfluxToken { get; set; } = "my-token";
        public string InfluxBucket { get; set; } = "my-bucket";
        public string InfluxOrg { get; set; } = "my-org";
    }

    public class OpcInfluxBridge : IDisposable
    {
        private readonly OpcInfluxConfig _config;
        private InfluxDBClient? _influxClient;
        private WriteApi? _writeApi;
        private Session? _session;
        private Subscription? _subscription;

        public OpcInfluxBridge(OpcInfluxConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task StartAsync()
        {
            // Setup InfluxDB
            _influxClient = new InfluxDBClient(_config.InfluxUrl, _config.InfluxToken);
            _writeApi = _influxClient.GetWriteApi();

            // Setup OPC UA
            var config = new ApplicationConfiguration()
            {
                ApplicationName = "DataLogger",
                ApplicationUri = Utils.Format(@"urn:{0}:DataLogger", System.Net.Dns.GetHostName()),
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration { 
                    ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = "DataLogger" },
                    TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                    TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                    RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                    AutoAcceptUntrustedCertificates = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
                TraceConfiguration = new TraceConfiguration()
            };
            
            await config.Validate(ApplicationType.Client);
            if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
            }

            EndpointDescription endpointDescription;
            var discoveryUrl = new Uri(_config.OpcServerUrl);
            using (var client = DiscoveryClient.Create(discoveryUrl))
            {
                var endpoints = client.GetEndpoints(null);
                endpointDescription = endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None) ?? endpoints.First();
            }

            var endpointConfig = EndpointConfiguration.Create(config);
            var endpointUrl = new ConfiguredEndpoint(null, endpointDescription, endpointConfig);

            _session = await Session.Create(config, endpointUrl, false, "DataLoggerSession", 60000, new UserIdentity(new AnonymousIdentityToken()), null);

            // Create Subscription
            _subscription = new Subscription(_session.DefaultSubscription) { PublishingInterval = 1000 };
            
            foreach (var nodeId in _config.NodeIds)
            {
                var item = new MonitoredItem(_subscription.DefaultItem)
                {
                    DisplayName = nodeId,
                    StartNodeId = nodeId
                };
                item.Notification += OnNotification;
                _subscription.AddItem(item);
            }

            _session.AddSubscription(_subscription);
            _subscription.Create();
        }

        private void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
             foreach (var value in item.DequeueValues())
             {
                 try
                 {
                     var point = PointData.Measurement("opcua_data")
                         .Tag("nodeId", item.StartNodeId.ToString())
                         .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                     if (value.Value != null)
                     {
                        if (double.TryParse(value.Value.ToString(), out double dVal))
                        {
                            point = point.Field("value", dVal);
                        }
                        else
                        {
                             point = point.Field("value_string", value.Value.ToString());
                        }

                        _writeApi?.WritePoint(point, _config.InfluxBucket, _config.InfluxOrg);
                     }
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine($"Error writing point: {ex.Message}");
                 }
             }
        }

        public void Stop()
        {
            if (_subscription != null)
            {
                _subscription.Delete(true);
                _subscription = null;
            }
            if (_session != null)
            {
                _session.Close();
                _session.Dispose();
                _session = null;
            }
            _writeApi?.Dispose();
            _influxClient?.Dispose();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
