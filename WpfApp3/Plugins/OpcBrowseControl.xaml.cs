using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Opc.Ua;
using Opc.Ua.Client;
using WpfApp3.Services;
using WpfApp3.ViewModels;

namespace WpfApp3.Plugins
{
    public partial class OpcBrowseControl : UserControl
    {
        private System.Collections.Generic.List<Session> _sessions = new();

        public ObservableCollection<OpcNodeViewModel> RootNodes { get; } = new();
        public ObservableCollection<string> RecentEndpoints { get; } = new();

        public OpcBrowseControl()
        {
            InitializeComponent();
            NodesTree.ItemsSource = RootNodes;
            UrlComboBox.ItemsSource = RecentEndpoints;
            
            var settings = ConfigurationService.Load();
            foreach (var endpoint in settings.RecentEndpoints)
            {
                RecentEndpoints.Add(endpoint);
            }
            if (RecentEndpoints.Any())
            {
                UrlComboBox.Text = RecentEndpoints.First();
            }

            // Unloaded += OpcBrowseControl_Unloaded;
        }

        private void OpcBrowseControl_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var session in _sessions)
            {
                OpcSessionManager.Instance.Unregister(session);
                if (session.Connected)
                    session.Close();
                session.Dispose();
            }
            _sessions.Clear();
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            var url = UrlComboBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(url)) return;

            // Save to history
            var existing = RecentEndpoints.FirstOrDefault(x => x.Equals(url, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                RecentEndpoints.Remove(existing);
            }
            RecentEndpoints.Insert(0, url);

            // Limit history
            while (RecentEndpoints.Count > 10)
            {
                RecentEndpoints.RemoveAt(RecentEndpoints.Count - 1);
            }
            
            var settings = ConfigurationService.Load();
            settings.RecentEndpoints = new System.Collections.Generic.List<string>(RecentEndpoints);
            ConfigurationService.Save(settings);
            
            // Re-select the inserted item so it shows in the combo box text area if unrelated
            UrlComboBox.SelectedItem = url; 

            LoadingOverlay.Visibility = Visibility.Visible;
            // Removed clearing of RootNodes and closing of existing session
            
            try
            {
                await Task.Run(async () =>
                {
                    var config = new ApplicationConfiguration
                    {
                        ApplicationName = "WpfApp3",
                        ApplicationType = ApplicationType.Client,
                        SecurityConfiguration = new SecurityConfiguration
                        {
                            ApplicationCertificate = new CertificateIdentifier { StoreType = "Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = "WpfApp3" },
                            TrustedIssuerCertificates = new CertificateTrustList { StoreType = "Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                            TrustedPeerCertificates = new CertificateTrustList { StoreType = "Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                            RejectedCertificateStore = new CertificateTrustList { StoreType = "Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                            AutoAcceptUntrustedCertificates = true
                        },
                        TransportConfigurations = new TransportConfigurationCollection(),
                        TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                        ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
                    };
                    await config.Validate(ApplicationType.Client);

                    // Select Endpoint using instance DiscoveryClient
                    EndpointDescription endpointDescription;
                    using (var client = DiscoveryClient.Create(new Uri(url)))
                    {
                        var endpoints = client.GetEndpoints(null);
                        endpointDescription = endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None) ?? endpoints.FirstOrDefault();
                    }
                    
                    if (endpointDescription == null) throw new Exception("No endpoints found");

                    var endpointConfiguration = EndpointConfiguration.Create(config);
                    var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
                    
                    var session = await Session.Create(config, endpoint, false, "WpfApp3 Session", 60000, new UserIdentity(new AnonymousIdentityToken()), null);

                    // Store session for cleanup
                    _sessions.Add(session);
                    OpcSessionManager.Instance.Register(session);

                    // Create Root Node ViewModel
                    // Root is Objects Folder usually, or we can start at RootFolder (84)
                    var rootRef = new ReferenceDescription
                    {
                         NodeId = ObjectIds.RootFolder,
                         DisplayName = new LocalizedText($"{url} ({endpointDescription.Server.ApplicationName})"),
                         NodeClass = NodeClass.Object,
                         TypeDefinition = ObjectTypeIds.FolderType
                    };

                    var rootVm = new OpcNodeViewModel(rootRef, session);
                    
                    Dispatcher.Invoke(() =>
                    {
                        RootNodes.Add(rootVm);
                        rootVm.IsExpanded = true; 
                    });
                });
            }
            catch (Exception ex)
            {
                 Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
        }
    }
}
