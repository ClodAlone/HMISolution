using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Opc.Ua;
using Opc.Ua.Client;

namespace WpfApp3.Plugins
{
    public partial class OpcDiscoveryControl : UserControl
    {
        public OpcDiscoveryControl()
        {
            InitializeComponent();
        }

        private async void Discover_Click(object sender, RoutedEventArgs e)
        {
            var url = UrlTextBox.Text;
            if (string.IsNullOrWhiteSpace(url)) return;

            LoadingOverlay.Visibility = Visibility.Visible;
            ServersListView.ItemsSource = null;

            try
            {
                await Task.Run(() =>
                {
                    // Use a short timeout for discovery
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
                    config.Validate(ApplicationType.Client).Wait();

                    // Create a DiscoveryClient
                    using (var client = DiscoveryClient.Create(new Uri(url)))
                    {
                        var servers = client.FindServers(null);
                        
                        // Update UI
                        Dispatcher.Invoke(() =>
                        {
                            ServersListView.ItemsSource = servers;
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Discovery failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
        }
    }
}
