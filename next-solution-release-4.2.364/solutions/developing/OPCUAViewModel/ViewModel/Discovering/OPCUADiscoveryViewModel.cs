using System;
using System.Collections.Generic;
using System.Linq;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Threading;
using NetAPIDiscovery;
using OPCUAViewModelService.ComponentService;
using UIMsgBoxAlertService.ComponentService;
#endif
using System.ComponentModel;
using Opc.Ua;
using ViewModelLib;
using System.Threading;
using Utilities;
using System.Windows.Input;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace OPCUAViewModel
{
    public class OPCUADiscoveryViewModel : TreeViewItemViewModel
    {
        #region Declaration
#if !WINDOWS_UWP && !NET_STANDARD
        public NetworkViewModel networkViewModel;
#endif
        public ApplicationConfiguration configuration { get; private set; }

        Dictionary<String, BackgroundWorker> MapWorkerThreads = new Dictionary<String, BackgroundWorker>();

        ConfiguredEndpointCollection listConfiguredEndpointCollection = new ConfiguredEndpointCollection();
        #endregion

        #region Constructor
#if !WINDOWS_UWP && !NET_STANDARD
        public OPCUADiscoveryViewModel(Dispatcher dispatcher, TreeViewItemViewModel parent)
#else
        public OPCUADiscoveryViewModel(Object dispatcher, TreeViewItemViewModel parent)
#endif
            : base(parent, false)
        {
#if !WINDOWS_UWP && !NET_STANDARD
            m_dispatcher = dispatcher;
#endif
            Title = Properties.Resource.OPCUADiscovery;

            ListDiscoveredWorstations = new SafeObservableCollection<String>();
            ListDiscoveringWorstations = new SafeObservableCollection<String>();

            configuration = Helpers.CreateClientConfiguration();
#if !WINDOWS_UWP && !NET_STANDARD
            // m_configuration = ApplicationConfiguration.Load("SampleConfiguration", ApplicationType.ClientAndServer);
            // Helpers.CheckApplicationInstanceCertificate(configuration);
            // check the certificate.
            try
            { 
                X509Certificate2 certificate = Helpers.CheckApplicationInstanceCertificate(configuration, 2048, true, true);
                if (certificate != null)
                {
                    // ensure the application uri matches the certificate.
                    string applicationUri = Utils.GetApplicationUriFromCertficate(certificate);

                    if (applicationUri != null)
                    {
                        configuration.ApplicationUri = applicationUri;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK,
                            MessageBoxImage.Error, MessageBoxResult.OK,
                            MessageBoxOptions.ServiceNotification);
            }

            configuration.CertificateValidator.CertificateValidation += CertificateValidator_CertificateValidation;

            if (m_dispatcher != null)
            {
                networkViewModel = new NetworkViewModel(m_dispatcher);
                networkViewModel.Children.CollectionChanged += ListDomains_CollectionChanged;
            }
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            // LastMessage = String.Format("{0} - Untrusted Certificate : {0}", Title, e.Certificate.Subject);
            StringBuilder buffer = new StringBuilder();

            buffer.AppendFormat("Certificate could not validated: {0}\r\n\r\n", e.Error.StatusCode);
            buffer.AppendFormat("Subject: {0}\r\n", e.Certificate.Subject);
            buffer.AppendFormat("Issuer: {0}\r\n", (e.Certificate.Subject == e.Certificate.Issuer) ? "Self-signed" : e.Certificate.Issuer);
            buffer.AppendFormat("Valid From: {0}\r\n", e.Certificate.NotBefore);
            buffer.AppendFormat("Valid To: {0}\r\n", e.Certificate.NotAfter);
            buffer.AppendFormat("Thumbprint: {0}\r\n\r\n", e.Certificate.Thumbprint);

            buffer.AppendFormat("Accept anyways?");

            //if (MessageBox.Show(buffer.ToString(), caller.Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    e.Accept = true;
            //}
            e.Accept = true;

            if (!configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates && 
                OPCUAViewModelComponent.uiInterfaceAvailable &&
                OPCUAViewModelComponent.uiInterface.ShowYesNo(buffer.ToString(), CustomDialogIcons.Warning) != CustomDialogResults.Yes)
                e.Accept = false;
        }

        void ListDomains_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
            {
                foreach (TreeViewItemViewModel dvm in e.NewItems)
                {
                    if (dvm is DomainViewModel)
                        dvm.Children.CollectionChanged += ListWorkstation_CollectionChanged;
                }
            }

            if (e.OldItems != null && e.OldItems.Count != 0)
            {
                foreach (TreeViewItemViewModel dvm in e.OldItems)
                {
                    if (dvm is DomainViewModel)
                        dvm.Children.CollectionChanged -= ListWorkstation_CollectionChanged;
                }
            }
        }

        void ListWorkstation_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
            {
                foreach (TreeViewItemViewModel wvm in e.NewItems)
                {
                    if (wvm is WorkstationViewModel)
                        AddHostName((wvm as WorkstationViewModel).Title);
                }
            }

            if (e.OldItems != null && e.OldItems.Count != 0)
            {
                foreach (TreeViewItemViewModel wvm in e.OldItems)
                {
                    if (wvm is WorkstationViewModel)
                        RemoveHostName((wvm as WorkstationViewModel).Title);
                }
            }
        }
#endif
#endregion

        #region Properties
        public SafeObservableCollection<String> ListDiscoveredWorstations { get; private set; }
        public SafeObservableCollection<String> ListDiscoveringWorstations { get; private set; }
        public String DiscoverHost { get; set; }
#if !WINDOWS_UWP && !NET_STANDARD
        public NetworkViewModel NetworkViewModel { get { return networkViewModel; } }
#endif
#endregion

#region Methods
        public void RefreshAll()
        {
            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUADiscoveryRefreshAll);

            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                CancelPendingIdleExecution();

                ListDiscoveringWorstations.Clear();
                ListDiscoveredWorstations.Clear();
                if (hasChildren)
                    Children.Clear();
                IsExpanded = false;

                foreach (BackgroundWorker bw in MapWorkerThreads.Values)
                    bw.CancelAsync();
                MapWorkerThreads.Clear();
#if !WINDOWS_UWP && !NET_STANDARD
                if (networkViewModel != null)
                    networkViewModel.RestartDiscovery();
#endif
            }
        }

        public void RefreshHostName(String HostName)
        {
            RemoveHostName(HostName);
            AddHostName(HostName);
        }

        public void RemoveHostName(String HostName)
        {
            LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUARemovingHostName, HostName);

            BackgroundWorker bw = null;
            lock (lockObject)
            {
                MapWorkerThreads.TryGetValue(HostName, out bw);
            }

            if (bw != null && bw.IsBusy)
            {
                bw.CancelAsync();
#if !WINDOWS_UWP && !NET_STANDARD
                using (new WaitCursor())
                {
                    while (bw.IsBusy)
                        Thread.Sleep(1);
                }
#endif
            }

            lock (lockObject)
            {
                ListDiscoveringWorstations.Remove(HostName);
                ListDiscoveredWorstations.Remove(HostName);
            }
        }

        public bool IsHostDiscovered(String HostName)
        {
            lock (lockObject)
            {
                return ListDiscoveredWorstations.Contains(HostName);
            }
        }

        public bool IsHostDiscovering(String HostName)
        {
            lock (lockObject)
            {
                return MapWorkerThreads.ContainsKey(HostName);
            }
        }

        public bool IsHostDiscoveringQueue(String HostName)
        {
            lock (lockObject)
            {
                return ListDiscoveringWorstations.Contains(HostName);
            }
        }

        public bool AddHostName(String HostName)
        {
            LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUAAddingHostName, HostName);

            lock (lockObject)
            {
                HostName = HostName.ToUpper();
                if (IsHostDiscovered(HostName) || IsHostDiscovering(HostName) || IsHostDiscoveringQueue(HostName))
                    return false;

                ListDiscoveringWorstations.Add(HostName);
#if !WINDOWS_UWP && !NET_STANDARD
                PromoteIdleExecution(DispatcherPriority.Invalid);
#else
                PromoteIdleExecution(100);
#endif
                return true;
            }
        }

        public void DiscoverHostNameNow(String HostName)
        {
            if (!IsHostDiscovering(HostName))
                StartBackgroundDiscovery(HostName);
        }

        protected override void IdleExecution()
        {
            String Workstation = null;
            lock (lockObject)
            {
                if (ListDiscoveringWorstations.Count > 0)
                {
                    Workstation = ListDiscoveringWorstations[0];
                    // ListDiscoveringWorstations.RemoveAt(0);
                }
            }

            if (Workstation != null)
                StartBackgroundDiscovery(Workstation);
        }

        public void StartDiscoveringSynchro(String wks)
        {
            lock (lockObject)
            {
                /*
                var listAdPerWorkstation = from ad in Children
                                            where (ad as ApplicationDescriptionViewModel).Workstation == workstation
                                            select ad;
                */
                List<TreeViewItemViewModel> toRemove = new List<TreeViewItemViewModel>();
                foreach (TreeViewItemViewModel advm in Children)
                {
                    if (advm is ApplicationDescriptionViewModel &&
                        String.Compare((advm as ApplicationDescriptionViewModel).Workstation, wks, false) == 0)
                    {
                        toRemove.Add(advm);
                    }
                }

                toRemove.ForEach(model => Children.Remove(model));
            }

            List<Uri> discoveryUrls = new List<Uri>();

            for (int i = 0; i < DiscoveryURL.m_DiscoveryUrls.Count; i++)
            {
                try
                {
                    Uri url = new Uri(String.Format(DiscoveryURL.m_DiscoveryUrls[i], wks));
                    discoveryUrls.Add(url);
                }
                catch (Exception ex)
                {

                }
            }

            ApplicationDescriptionCollection servers = null;

            DiscoveryClient client = null;
            for (int ii = 0; ii < discoveryUrls.Count; ii++)
            {
                LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUADiscoveringServerUrl, discoveryUrls[ii]);

                EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);
                // endpointConfiguration.OperationTimeout = 5000;

                try
                {
                    client = DiscoveryClient.Create(discoveryUrls[ii],
#if !WINDOWS_UWP && !NET_STANDARD
                                                    BindingFactory.Create(configuration, configuration.CreateMessageContext()),
#endif
                                                    endpointConfiguration);

                    if (client == null)
                        return;

                    servers = client.FindServers(null);
                    if (servers.Count > 0)
                        break;
                }
                catch (Exception)
                {
                    servers = null;
                }
                finally
                {
                    try
                    {
                        if (client != null)
                            client.Close();
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        if (client != null)
                            client.Dispose();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            if (servers != null)
            {
                using (var updater = new CollectionUpdater(Children))
                {
                    foreach (ApplicationDescription server in servers)
                    {
                        if (server.ApplicationType == ApplicationType.DiscoveryServer)
                        {
                            continue;
                        }

                        LastMessage = String.Format("{0} - {1} {2} {3}", Title, Properties.Resource.OPCUAServerDiscovered, wks, server.ApplicationName);

                        lock (lockObject)
                        {
                            Children.Add(new ApplicationDescriptionViewModel(server, wks, this, configuration));
                        }
                    }
                }
            }
            else
                LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUADiscoveringNoServerFound, wks);
        }

        private bool StartBackgroundDiscovery(String workstation)
        {
            LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUADiscoveringHostName, workstation);

            if (IsIdleExecutionCancelled())
                return false;

            //try
            //{
            //    Uri testuri = new Uri(workstation);
            //    ApplicationDescription ad = new ApplicationDescription();
            //    ad.ApplicationName = workstation;
            //    ad.ApplicationType = ApplicationType.Server;
            //    ad.ApplicationUri = workstation;

            //    String discoveryUrl = workstation;
            //    if (!discoveryUrl.StartsWith(Utils.UriSchemeOpcTcp))
            //    {
            //        if (!discoveryUrl.EndsWith("/discovery"))
            //        {
            //            discoveryUrl += "/discovery";
            //        }
            //    }
            //    StringCollection sc = new StringCollection();
            //    sc.Add(discoveryUrl);
            //    ad.DiscoveryUrls = sc;
            //    lock (lockObject)
            //    {
            //        Children.Add(new ApplicationDescriptionViewModel(ad, workstation, this, configuration));
            //    }
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    Utils.Trace(ex.ToString());
            //}


            BackgroundWorker bw = null;
            lock (lockObject)
            {
                if (ListDiscoveredWorstations.Contains(workstation))
                    return false;
                ListDiscoveringWorstations.Remove(workstation);

                bw = new BackgroundWorker();
                MapWorkerThreads.Add(workstation, bw);
            }

            bw.DoWork += (o, e) =>
            {
                String wks = e.Argument as String;
                e.Result = e.Argument;
                try
                {
                    lock (lockObject)
                    {
                        /*
                        var listAdPerWorkstation = from ad in Children
                                                   where (ad as ApplicationDescriptionViewModel).Workstation == workstation
                                                   select ad;
                        */
                        List<TreeViewItemViewModel> toRemove = new List<TreeViewItemViewModel>();
                        foreach (TreeViewItemViewModel advm in Children)
                        {
                            if (advm is ApplicationDescriptionViewModel &&
                                String.Compare((advm as ApplicationDescriptionViewModel).Workstation, wks, false) == 0)
                            {
                                toRemove.Add(advm);
                            }
                        }

                        toRemove.ForEach(model => Children.Remove(model));
                    }

                    List<Uri> discoveryUrls = new List<Uri>();

                    for (int i = 0; i < DiscoveryURL.m_DiscoveryUrls.Count; i++)
                    {
                        try
                        {
                            Uri url = new Uri(String.Format(DiscoveryURL.m_DiscoveryUrls[i], wks));
                            discoveryUrls.Add(url);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    ApplicationDescriptionCollection servers = null;

                    int nProgressStep = 100 / (discoveryUrls.Count + 1);
                    int nProgress = nProgressStep;
                    bw.ReportProgress(nProgress);

                    DiscoveryClient client = null;
                    for (int ii = 0; ii < discoveryUrls.Count; ii++)
                    {
                        if (bw.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUADiscoveringServerUrl, discoveryUrls[ii]);

                        EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);
                        // endpointConfiguration.OperationTimeout = 5000;

                        try
                        {
                            client = DiscoveryClient.Create(discoveryUrls[ii],
#if !WINDOWS_UWP && !NET_STANDARD
                                                            BindingFactory.Create(configuration, configuration.CreateMessageContext()),
#endif
                                                            endpointConfiguration);

                            if (client == null || bw.CancellationPending == true)
                            {
                                e.Cancel = true;
                                return;
                            }

                            nProgress += nProgressStep;
                            bw.ReportProgress(nProgress);
                            servers = client.FindServers(null);
                            if (servers.Count > 0)
                                break;
                        }
                        catch (Exception)
                        {
                            servers = null;
                        }
                        finally
                        {
                            try
                            {
                                if (client != null)
                                    client.Close();
                            }
                            catch (Exception ex)
                            {

                            }
                            try
                            {
                                if (client != null)
                                    client.Dispose();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    if (servers != null)
                    {
                        using (var updater = new CollectionUpdater(Children))
                        {
                            foreach (ApplicationDescription server in servers)
                            {
                                if (bw.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                if (server.ApplicationType == ApplicationType.DiscoveryServer)
                                {
                                    continue;
                                }

                                LastMessage = String.Format("{0} - {1} {2} {3}", Title, Properties.Resource.OPCUAServerDiscovered, wks, server.ApplicationName);

                                lock (lockObject)
                                {
                                    Children.Add(new ApplicationDescriptionViewModel(server, wks, this, configuration));
                                }
                            }
                        }
                    }
                    else
                        LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUADiscoveringNoServerFound, wks);
                }
                finally
                {
                    lock (lockObject)
                    {
                        MapWorkerThreads.Remove(wks);
                        ListDiscoveringWorstations.Remove(wks);
                        ListDiscoveredWorstations.Add(wks);
                    }

                    bw.Dispose();
                    bw = null;
                }
            };

            bw.RunWorkerCompleted += (o, e) =>
            {
                // String wks = e.Result as String;
#if !WINDOWS_UWP && !NET_STANDARD
                PromoteIdleExecution(DispatcherPriority.Invalid);
#else
                PromoteIdleExecution(100);
#endif
            };

            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync(workstation);

            return true;
        }
#endregion

#region Commands
        RelayCommand _discoverCommand;
        public ICommand DiscoverCommand
        {
            get
            {
                if (_discoverCommand == null)
                {
                    _discoverCommand = new RelayCommand(
                        param => AddHostName(DiscoverHost),
                        param => CanDiscoverHost
                        );
                }
                return _discoverCommand;
            }
        }

        bool CanDiscoverHost
        {
            get { return String.IsNullOrEmpty(DiscoverHost) && !IsHostDiscovered(DiscoverHost); }
        }

        RelayCommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(
                        param => RefreshHostName(DiscoverHost),
                        param => CanRefresh
                        );
                }
                return _discoverCommand;
            }
        }

        bool CanRefresh
        {
            get { return String.IsNullOrEmpty(DiscoverHost) && !IsHostDiscovered(DiscoverHost); }
        }

        RelayCommand _refreshAllCommand;
        public ICommand RefreshAllCommand
        {
            get
            {
                if (_refreshAllCommand == null)
                {
                    _refreshAllCommand = new RelayCommand(
                        param => RefreshAll(),
                        null
                        );
                }
                return _discoverCommand;
            }
        }
#endregion


#region IDisposable Members
        protected override void OnDispose()
        {
            base.OnDispose();

            foreach (BackgroundWorker bw in MapWorkerThreads.Values)
                bw.CancelAsync();

#if !WINDOWS_UWP && !NET_STANDARD
            configuration.CertificateValidator.CertificateValidation -= CertificateValidator_CertificateValidation;
            if (networkViewModel != null)
            {
                networkViewModel.Children.CollectionChanged -= ListDomains_CollectionChanged;
                networkViewModel.Dispose();
            }
#endif
        }
#endregion

#if !WINDOWS_UWP && !NET_STANDARD
#region Validations
        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }
#endregion
#endif
    }
}
