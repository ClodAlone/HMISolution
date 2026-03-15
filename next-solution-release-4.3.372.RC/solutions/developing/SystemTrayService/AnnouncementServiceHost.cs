using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;

namespace SystemTrayService
{
    public class AnnouncementServiceHost
    {
        #region Declarations
        ServiceHost announcementServiceHost;
        AnnouncementService announcementService;
        Dictionary<String, Type> services;
        #endregion

        #region Public Events
        public event EventHandler<ServiceArgs> ServiceOnline;
        void OnServiceOnline(ServiceArgs service)
        {
            var t = ServiceOnline;
            if (t != null)
                t(this, service);
        }

        public event EventHandler<ServiceArgs> ServiceOffline;
        void OnServiceOffline(ServiceArgs service)
        {
            var t = ServiceOffline;
            if (t != null)
                t(this, service);
        }
        #endregion

        #region Public Methods
        public void Start()
        {
            if (announcementServiceHost == null)
            {
                // Create an AnnouncementService instance
                announcementService = new AnnouncementService();

                // Subscribe the announcement events
                announcementService.OnlineAnnouncementReceived += AnnouncementService_OnlineAnnouncementReceived;
                announcementService.OfflineAnnouncementReceived += AnnouncementService_OfflineAnnouncementReceived;

                // Create ServiceHost for the AnnouncementService
                announcementServiceHost = new ServiceHost(announcementService);
                // Listen for the announcements sent over UDP multicast
                announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());
                announcementServiceHost.Open();
            }
        }

        public void Stop()
        {
            if (announcementServiceHost != null)
            {
                announcementService.OnlineAnnouncementReceived -= AnnouncementService_OnlineAnnouncementReceived;
                announcementService.OfflineAnnouncementReceived -= AnnouncementService_OfflineAnnouncementReceived;

                announcementServiceHost.Close();
                announcementServiceHost = null;
            }
        }

        public void AddService(Type service)
        {
            if (services == null)
                services = new Dictionary<String, Type>();
            if (!services.ContainsKey(service.Name))
                services.Add(service.Name, service);
        }

        public void RemoveService(Type service)
        {
            if (services != null && services.ContainsKey(service.Name))
                services.Remove(service.Name);
        }

        public void DiscoverActiveServices()
        {
            DiscoverActiveServices(Properties.Settings.Default.DiscoverActiveServerTime);
        }
        #endregion

        #region Discovery
        void DiscoverActiveServices(double timeout)
        {
            if (services == null || services.Count == 0)
                return;

            using (var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint()))
            {
                foreach (var type in services.Values)
                {
                    var criteria = new FindCriteria(type);
                    criteria.Duration = TimeSpan.FromMilliseconds(timeout);
                    FindResponse discoveryResponse = discoveryClient.Find(criteria);
                    foreach (var endpoint in discoveryResponse.Endpoints)
                    {
                        var service = GetService(endpoint);
                        if (service != null)
                            OnServiceOnline(service);
                    }
                }
            }
        }

        ServiceArgs GetService(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            for (int i = 0; i < endpointDiscoveryMetadata.ContractTypeNames.Count; ++i)
            {
                if (services.ContainsKey(endpointDiscoveryMetadata.ContractTypeNames[i].Name))
                {
                    return new ServiceArgs()
                    {
                        HostName = endpointDiscoveryMetadata.Address.Uri.DnsSafeHost,
                        InstanceId = GetInstanceId(endpointDiscoveryMetadata.Address.ToString()),
                        SchemaType = endpointDiscoveryMetadata.Address.Uri.Scheme,
                        ServiceType = services[endpointDiscoveryMetadata.ContractTypeNames[i].Name]
                    };
                }
            }

            return null;
        }

        string GetInstanceId(string uri)
        {
            int nFound = uri.LastIndexOf('/');
            if (nFound != -1)
                return uri.Substring(nFound + 1);
            else
                return null;
        }

        void AnnouncementService_OnlineAnnouncementReceived(object sender, AnnouncementEventArgs e)
        {
            var service = GetService(e.EndpointDiscoveryMetadata);
            if (service != null)
                OnServiceOnline(service);
        }

        void AnnouncementService_OfflineAnnouncementReceived(object sender, AnnouncementEventArgs e)
        {
            var service = GetService(e.EndpointDiscoveryMetadata);
            if (service != null)
                OnServiceOffline(service);
        }
        #endregion
    }
}
