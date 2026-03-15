using System;
using ViewModelLib;
using Opc.Ua;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using OPCUAViewModel.ContextMenuKey;
using System.Windows.Threading;
#endif
using System.Reflection;
using System.IO;
using UFInterfaces;
using System.Collections.Generic;
using Utilities;
using System.Threading;

namespace OPCUAViewModel
{
    public class EndpointDescriptionViewModel : TreeViewItemViewModel, IEntityReference
    {
#region Members
        public EndpointDescription endpointDescription { get; protected set; }
        public ApplicationConfiguration configuration { get; protected set; }
#endregion

#region Constructor
        public EndpointDescriptionViewModel(EndpointDescription epd, TreeViewItemViewModel parent)
            : base(parent, false)
        {
            if (epd == null)
                throw new ArgumentNullException("EndpointDescription");

            endpointDescription = epd;
            Title = epd.EndpointUrl;
        }

        public EndpointDescriptionViewModel(TreeViewItemViewModel parent)
            : base(parent, false)
        {
            endpointDescription = new EndpointDescription();
            Title = endpointDescription.EndpointUrl;
        }

#endregion


        /// <summary>
        /// Finds the endpoint that best matches the current settings.
        /// </summary>
        /// <param name="discoveryUrl">The discovery URL.</param>
        /// <param name="useSecurity">if set to <c>true</c> select an endpoint that uses security.</param>
        /// <returns>The best available endpoint.</returns>
        /// 
        static OPCUADiscoveryViewModel DiscoveryViewModel = new OPCUADiscoveryViewModel(null, null);
        public static EndpointDescriptionViewModel SelectEndpoint(string hostname, string appname, bool useSecurity)
        {
            if (appname == null)
                throw new ArgumentNullException("appname must not be null");

            if (String.IsNullOrEmpty(hostname))
                hostname = Properties.Settings.Default.localhost;

            bool bLocal = String.Compare(hostname, Properties.Settings.Default.localhost, true) == 0;

            bool hasChildren = false;
            List<TreeViewItemViewModel> children = null;
            lock (DiscoveryViewModel)
            {
                //DiscoveryViewModel.DiscoverHostNameNow(hostname);
                //while (!DiscoveryViewModel.IsHostDiscovered(hostname) ||
                //    DiscoveryViewModel.IsHostDiscovering(hostname) ||
                //    DiscoveryViewModel.IsHostDiscoveringQueue(hostname))
                //    WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, null);
                DiscoveryViewModel.StartDiscoveringSynchro(hostname);

                hasChildren = DiscoveryViewModel.HasChildren;
                if (hasChildren)
                    children = new List<TreeViewItemViewModel>(DiscoveryViewModel.Children);
            }

            EndpointDescriptionViewModel selectedEndpoint = null;
            if (hasChildren)
            {
                foreach (TreeViewItemViewModel advm in children)
                {
                    if (advm is ApplicationDescriptionViewModel &&
                        (advm as ApplicationDescriptionViewModel).Workstation == hostname &&
                        (advm as ApplicationDescriptionViewModel).Title == appname)
                    {
                        if (!advm.BackgroundWorkProcessed)
                        {
                            while (!advm.BackgroundWorkProcessed)
#if !WINDOWS_UWP && !NET_STANDARD
                                WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, null);
#else
                                Thread.Sleep(100);
#endif
                        }
                        hasChildren = advm.HasChildren;
                        if (hasChildren)
                        {
                            foreach (TreeViewItemViewModel edvm in advm.Children)
                            {
                                EndpointDescriptionViewModel epmodel = edvm as EndpointDescriptionViewModel;
#if !NET_STANDARD
                                if (!bLocal)
                                {
                                    if (epmodel.EndpointUrl.StartsWith(Utils.UriSchemeNetPipe))
                                        continue;
                                }
#endif
                                if (!useSecurity)
                                {
                                    if (epmodel.SecurityMode == MessageSecurityMode.None)
                                    {
                                        selectedEndpoint = epmodel;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (epmodel.SecurityMode != MessageSecurityMode.None)
                                    {
                                        // The security level is a relative measure assigned by the server to the 
                                        // endpoints that it returns. Clients should always pick the highest level
                                        // unless they have a reason not too.
                                        if (selectedEndpoint == null || epmodel.SecurityLevel > selectedEndpoint.SecurityLevel)
                                        {
                                            selectedEndpoint = epmodel;
                                        }
                                    }
                                }
                            }

                            if (selectedEndpoint == null && advm.Children.Count > 0)
                                selectedEndpoint = advm.Children[0] as EndpointDescriptionViewModel;
                            break;
                        }
                    }
                }
            }

            return selectedEndpoint;
        }

        static Dictionary<String, EndpointDescription> mapCacheDiscoveredEndpoint = new Dictionary<String, EndpointDescription>();
        static Timer timerCleanDataLayer;

        public static EndpointDescription SelectEndpoint(string discoveryUrl, bool useSecurity)
        {
            var keyCache = String.Format("{0}-{1}", discoveryUrl, useSecurity);
            lock (mapCacheDiscoveredEndpoint)
            {
                if (timerCleanDataLayer == null)
                {
                    var delay = TimeSpan.FromSeconds(30);
                    timerCleanDataLayer = new Timer((o) =>
                    {
                        lock (mapCacheDiscoveredEndpoint)
                        {
                            mapCacheDiscoveredEndpoint.Clear();
                        }
                    }, null, delay, delay);
                }

                if (mapCacheDiscoveredEndpoint.ContainsKey(keyCache))
                    return mapCacheDiscoveredEndpoint[keyCache];
            }

            // get a valid discovery url that can be used to get endpoints.
            discoveryUrl = Helpers.GetDiscoveryUrl(discoveryUrl);

            // parse the selected URL.
            Uri uri = new Uri(discoveryUrl);

            // set a short timeout because this is happening in the drop down event.
            EndpointConfiguration configuration = EndpointConfiguration.Create();
            configuration.OperationTimeout = 5000;

            EndpointDescription selectedEndpoint = null;

            // Connect to the server's discovery endpoint and find the available configuration.
            using (DiscoveryClient client = DiscoveryClient.Create(uri, configuration))
            {
                try
                {
                    EndpointDescriptionCollection endpoints = client.GetEndpoints(null);

                    // select the best endpoint to use based on the selected URL and the UseSecurity checkbox. 
                    for (int ii = 0; ii < endpoints.Count; ii++)
                    {
                        EndpointDescription endpoint = endpoints[ii];

                        // pick the first available endpoint by default.
                        if (selectedEndpoint == null)
                        {
                            selectedEndpoint = endpoint;
                        }

                        // check for a match on the URL scheme.
                        if (endpoint.EndpointUrl.StartsWith(uri.Scheme))
                        {
                            // check if security was requested.
                            if (useSecurity)
                            {
                                if (endpoint.SecurityMode != MessageSecurityMode.None)
                                {
                                    // The security level is a relative measure assigned by the server to the 
                                    // endpoints that it returns. Clients should always pick the highest level
                                    // unless they have a reason not too.
                                    if (endpoint.SecurityLevel > selectedEndpoint.SecurityLevel)
                                    {
                                        selectedEndpoint = endpoint;
                                    }
                                }
                            }

                            // look for an unsecured endpoint if requested.
                            else
                            {
                                if (endpoint.SecurityMode == MessageSecurityMode.None)
                                {
                                    selectedEndpoint = endpoint;
                                    break;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    client.Close();
                }
            }

            // if a server is behind a firewall it may return URLs that are not accessible to the client.
            // This problem can be avoided by assuming that the domain in the URL used to call 
            // GetEndpoints can be used to access any of the endpoints. This code makes that conversion.
            // Note that the conversion only makes sense if discovery uses the same protocol as the endpoint.
            Uri endpointUrl = Utils.ParseUri(selectedEndpoint.EndpointUrl);

            if (endpointUrl != null && endpointUrl.Scheme == uri.Scheme)
            {
                UriBuilder builder = new UriBuilder(endpointUrl) 
                                        { Host = uri.DnsSafeHost, Port = uri.Port };
                selectedEndpoint.EndpointUrl = builder.ToString();
            }

            lock (mapCacheDiscoveredEndpoint)
            {
                mapCacheDiscoveredEndpoint.Add(keyCache, selectedEndpoint);
            }
            // return the selected endpoint.
            return selectedEndpoint;
        }

        public void UpdateEndpointDescription(bool useSecurity)
        {
            string rawUrl = Helpers.GetDiscoveryUrl(Title);

            Uri discoveryUrl = new Uri(rawUrl);

            if (configuration == null)
            {
                if (Parent is EndpointDescriptionViewModel)
                {
                    if ((Parent as EndpointDescriptionViewModel).configuration != null)
                        configuration = (Parent as EndpointDescriptionViewModel).configuration;
                    else if (Parent.Parent is ApplicationDescriptionViewModel)
                        configuration = (Parent.Parent as ApplicationDescriptionViewModel).configuration;
                    else if (Parent.Parent is OPCUADiscoveryViewModel)
                        configuration = (Parent.Parent as OPCUADiscoveryViewModel).configuration;
                }
                else if (Parent is OPCUADiscoveryViewModel)
                    configuration = (Parent as OPCUADiscoveryViewModel).configuration;

                if (configuration == null)
                    throw new ArgumentNullException("ApplicationConfiguration");
            }


            // set a short timeout.
            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);
            endpointConfiguration.OperationTimeout = 5000;

            // create the object used to connect to the local discovery server.
            using (DiscoveryClient client = DiscoveryClient.Create(discoveryUrl, endpointConfiguration))
            {
                try
                {
                    // find the servers.
                    EndpointDescriptionCollection endpoints = client.GetEndpoints(null);

                    int maxSecurityLevel = -1;
                    EndpointDescription endpointToUse = null;

                    for (int ii = 0; ii < endpoints.Count; ii++)
                    {
                        // match scheme.
                        if (!endpoints[ii].EndpointUrl.StartsWith(discoveryUrl.Scheme))
                        {
                            continue;
                        }

                        // select the first match.
                        if (endpointToUse == null)
                        {
                            endpointToUse = endpoints[ii];
                        }

                        // check if security is required.
                        if (!useSecurity)
                        {
                            if (endpoints[ii].SecurityMode == MessageSecurityMode.None)
                            {
                                endpointToUse = endpoints[ii];
                                break;
                            }
                        }
                        // select the best available security.
                        else
                        {
                            if (endpoints[ii].SecurityLevel > maxSecurityLevel)
                            {
                                endpointToUse = endpoints[ii];
                                maxSecurityLevel = endpoints[ii].SecurityLevel;
                            }
                        }
                    }

                    // if a server is behind a firewall it may return URLs that are not accessible to the client.
                    // This problem can be avoided by assuming that the domain in the URL used to call 
                    // GetEndpoints can be used to access any of the endpoints. This code makes that conversion.
                    // Note that the conversion only makes sense if discovery uses the same protocol as the endpoint.
                    Uri endpointUrl = Utils.ParseUri(endpointToUse.EndpointUrl);

                    if (endpointUrl != null && endpointUrl.Scheme == discoveryUrl.Scheme)
                    {
                        UriBuilder builder = new UriBuilder(endpointUrl) { Host = discoveryUrl.DnsSafeHost, Port = discoveryUrl.Port };
                        endpointToUse.EndpointUrl = builder.ToString();
                    }

                    endpointDescription = endpointToUse;
                }
                finally
                {
                    client.Close();
                }
            }
        }

#region Properties

        public String DisplayTitle
        {
            get
            {
                try
                {
                    Uri uri = new Uri(EndpointUrl);

#if !WINDOWS_UWP && !NET_STANDARD
                    FieldInfo fi = SecurityMode.GetType().GetField(SecurityMode.ToString());
                    LocalizableDescriptionAttribute[] attributes =
                        (LocalizableDescriptionAttribute[])fi.GetCustomAttributes(typeof(LocalizableDescriptionAttribute), false);
                    String security = ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : SecurityMode.ToString();
                    return String.Format(Properties.Settings.Default.EndpointDisplaytitle, uri.Scheme, security, SecurityLevel);
#else
                    return String.Format("{0}, {1} (Level {2})", uri.Scheme, SecurityMode, SecurityLevel);
#endif
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex.ToString());
                    return Title;
                }
            }
        }

        public String EndpointUrl
        {
            get
            {
                return endpointDescription.EndpointUrl;
            }
            set
            {
                if (value == endpointDescription.EndpointUrl)
                    return;

                Title = endpointDescription.EndpointUrl = value;
                OnPropertyChanged("EndpointUrl");
            }
        }

        public BinaryEncodingSupport EncodingSupport
        {
            get
            {
                return endpointDescription.EncodingSupport;
            }
        }

        public Uri ProxyUrl
        {
            get
            {
                return endpointDescription.ProxyUrl;
            }
            set
            {
                if (value == endpointDescription.ProxyUrl)
                    return;

                endpointDescription.ProxyUrl = value;
                OnPropertyChanged("ProxyUrl");
            }
        }

        public byte SecurityLevel
        {
            get
            {
                return endpointDescription.SecurityLevel;
            }
            set
            {
                if (value == endpointDescription.SecurityLevel)
                    return;

                endpointDescription.SecurityLevel = value;
                OnPropertyChanged("SecurityLevel");
            }
        }

        public MessageSecurityMode SecurityMode
        {
            get
            {
                return endpointDescription.SecurityMode;
            }
            set
            {
                if (value == endpointDescription.SecurityMode)
                    return;

                endpointDescription.SecurityMode = value;
                OnPropertyChanged("SecurityMode");
            }
        }

        public bool UseSecurity { get; set; }
#endregion

#if !WINDOWS_UWP && !NET_STANDARD
#region Validations

        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "EndpointUrl")
            {
                if (!Uri.IsWellFormedUriString(EndpointUrl, UriKind.Absolute))
                    return Properties.Resource.EndpointDescription_InvalidUri;
            }

            return base.PerformValidation(propertyName);
        }

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

#region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                //BitmapImage bm = new BitmapImage();
                //bm.BeginInit();
                //Assembly assembly = Assembly.GetExecutingAssembly();

                //String str = String.Format("pack://application:,,,/{0};component/Images/connect_16x16.png",
                //    Path.GetFileNameWithoutExtension(assembly.Location));
                //bm.UriSource = new Uri(str);
                //bm.EndInit();
                var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, $"OPCUAVMConnect", false);

                return bm;
#else
                return null;
#endif
            }
        }

        public ContextMenu contextMenu
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                EndpointDescriptionMenu wm = new EndpointDescriptionMenu();
                wm.InitializeComponent();

                return wm["ContextMenuKey"] as ContextMenu;
#else
                return null;
#endif
            }
        }

        public Object Tooltip
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                return new OPCUAViewModel.UserControls.EndpointDescriptionViewModel();
#else
                return null;
#endif
            }
        }

        public ImageSource ExpandedImageSource
        {
            get { return null; }
        }

        public object ContainedObject
        {
            get { return endpointDescription; }
        }

        public object EntityParent
        {
            get { return Parent; }
        }

        public String TypeDefinitionString
        {
            get { return null; }
        }

#endregion
    }
}
