using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using ServerEditor.Services;
using ServerEditor.ViewModels;

namespace ServerEditor.Controls
{
    public partial class OpcBrowseControl : UserControl
    {
        private List<Session> _sessions = new();
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
            else
            {
                UrlComboBox.Text = $"opc.tcp://{System.Net.Dns.GetHostName()}:14840/SimpleOpcFileServer";
            }
            
            OpcSessionManager.Instance.SessionErrorMessage += OnSessionError;
        }

        private void OnSessionError(object? sender, string message)
        {
            Dispatcher.Invoke(() =>
            {
                RootNodes.Clear();
                var errorNode = OpcNodeViewModel.CreateError(message);
                RootNodes.Add(errorNode);
                NodesTree.IsEnabled = false;
            });
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            var url = UrlComboBox.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(url))
            {
                Connect(url);
            }
        }

        public async void Connect(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            // Save history
            var existing = RecentEndpoints.FirstOrDefault(x => x.Equals(url, StringComparison.OrdinalIgnoreCase));
            if (existing != null) RecentEndpoints.Remove(existing);
            RecentEndpoints.Insert(0, url);
            while (RecentEndpoints.Count > 10) RecentEndpoints.RemoveAt(RecentEndpoints.Count - 1);

            var settings = ConfigurationService.Load();
            settings.RecentEndpoints = new List<string>(RecentEndpoints);
            ConfigurationService.Save(settings);

            UrlComboBox.SelectedItem = url;
            UrlComboBox.Text = url; // Ensure text is set

            LoadingOverlay.Visibility = Visibility.Visible;
            
            try
            {
                 // Close existing sessions if desired? No, allow multiple?
                 // For simplified browser, let's clear previous connection.
                 foreach(var sess in _sessions) sess.Close();
                 _sessions.Clear();
                 RootNodes.Clear();

                 await Task.Run(async () =>
                 {
                     var appConfig = new ConfigurationBuilder()
                         .SetBasePath(AppContext.BaseDirectory)
                         .AddJsonFile("appsettings.json", optional: true)
                         .Build();

                     var opcSection = appConfig.GetSection("OpcUa");
                     var applicationName = opcSection["ApplicationName"] ?? "ServerEditorClient";
                     var pkiRoot = opcSection["PkiRoot"] ?? "%LocalApplicationData%/ServerEditor/pki";
                     var autoAccept = bool.TryParse(opcSection["AutoAcceptUntrustedCertificates"], out var aa) && aa;
                     var sessionTimeout = int.TryParse(opcSection["DefaultSessionTimeout"], out var st) ? st : 60000;

                     var config = new ApplicationConfiguration
                     {
                         ApplicationName = applicationName,
                         ApplicationUri = Utils.Format(@"urn:{0}:{1}", System.Net.Dns.GetHostName(), applicationName),
                         ApplicationType = ApplicationType.Client,
                         SecurityConfiguration = new SecurityConfiguration
                         {
                             ApplicationCertificate = new CertificateIdentifier
                             {
                                 StoreType = CertificateStoreType.Directory,
                                 StorePath = $"{pkiRoot}/own",
                                 SubjectName = applicationName
                             },
                             TrustedPeerCertificates = new CertificateTrustList
                             {
                                 StoreType = CertificateStoreType.Directory,
                                 StorePath = $"{pkiRoot}/trusted"
                             },
                             TrustedIssuerCertificates = new CertificateTrustList
                             {
                                 StoreType = CertificateStoreType.Directory,
                                 StorePath = $"{pkiRoot}/issuer"
                             },
                             RejectedCertificateStore = new CertificateTrustList
                             {
                                 StoreType = CertificateStoreType.Directory,
                                 StorePath = $"{pkiRoot}/rejected"
                             }
                         },
                         TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                         ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = sessionTimeout },
                         TraceConfiguration = new TraceConfiguration()
                     };
                     
                     await config.Validate(ApplicationType.Client);

                     if (autoAccept)
                     {
                         config.CertificateValidator.CertificateValidation += (s, e) =>
                         {
                             if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
                             {
                                 e.Accept = true;
                             }
                         };
                     }

                     var application = new ApplicationInstance
                     {
                         ApplicationName = config.ApplicationName,
                         ApplicationType = ApplicationType.Client,
                         ApplicationConfiguration = config
                     };
                     
                     await application.CheckApplicationInstanceCertificates(false, (ushort)2048);
                     
                     var client = DiscoveryClient.Create(new Uri(url));
                     var endpoints = client.GetEndpoints(null);
                     var endpointDescription =
                         endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None
                             && e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
                         ?? endpoints.FirstOrDefault(e => e.EndpointUrl.StartsWith("opc.tcp://", StringComparison.OrdinalIgnoreCase))
                         ?? endpoints.FirstOrDefault(e => e.SecurityMode == MessageSecurityMode.None)
                         ?? endpoints.FirstOrDefault();
                     client.Dispose();

                     if (endpointDescription == null) throw new Exception("No endpoints found");

                     // Override only the host/port in the discovered endpoint URL with what
                     // the user typed, preserving the transport scheme (opc.tcp:// vs http://).
                     if (Uri.TryCreate(endpointDescription.EndpointUrl, UriKind.Absolute, out var discoveredUri) &&
                         Uri.TryCreate(url, UriKind.Absolute, out var userUri))
                     {
                         var builder = new UriBuilder(discoveredUri)
                         {
                             Host = userUri.Host,
                             Port = userUri.Port
                         };
                         endpointDescription.EndpointUrl = builder.Uri.ToString();
                     }

                     var endpointConfiguration = EndpointConfiguration.Create(config);
                     var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);
                     
                      var session = await Session.Create(config, endpoint, false, "ServerEditorSession", (uint)sessionTimeout, new UserIdentity(new AnonymousIdentityToken()), null);
                     
                     if (session != null && session.Connected)
                     {
                         session.KeepAlive += (s, e) =>
                         {
                             if (ServiceResult.IsBad(e.Status))
                             {
                                 Dispatcher.Invoke(() => OpcSessionManager.Instance.ReportError($"Disconnected: {e.Status}"));
                             }
                         };

                         // Load root node
                         var rootRef = new ReferenceDescription { NodeId = ObjectIds.ObjectsFolder, NodeClass = NodeClass.Object, DisplayName = "Objects", BrowseName = "Objects", TypeDefinition = ObjectTypeIds.FolderType };
                         var rootVm = new OpcNodeViewModel(rootRef, session);
                         
                         Application.Current.Dispatcher.Invoke(() =>
                         {
                             NodesTree.IsEnabled = true;
                             _sessions.Add(session);
                             OpcSessionManager.Instance.Register(session);
                             RootNodes.Add(rootVm);
                             rootVm.IsExpanded = true;
                         });
                    }
                 });
            }
            catch (Exception ex)
            {
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    MessageBox.Show(window, $"Connection failed: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Connection failed: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
        }
    }
}