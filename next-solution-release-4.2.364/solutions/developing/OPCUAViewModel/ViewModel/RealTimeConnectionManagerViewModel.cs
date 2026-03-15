using System;
using System.Linq;
using System.Collections.Generic;
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Speech.Synthesis;
#endif
using log4net;
#else
using Windows.UI.Core;
using Windows.UI.Xaml;
#endif
using Utilities;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using UFInterfaces;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Net;

namespace OPCUAViewModel
{
    public class RealTimeConnectionManagerViewModel : TreeViewItemViewModel, IEntityReference
    {
        #region Members

//#if !WINDOWS_UWP
//#if !NET_STANDARD
//        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resource.LicenseManager);
//#else
//        static readonly ILog logLicense = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resource.LicenseManager);
//#endif
//#endif

        SessionViewModel sessionViewModel;
        SubscriptionViewModel SubscriptionViewModel;
        readonly String sessionName;
        readonly Dictionary<NodeId, MonitoredItemViewModel> MonitoredItemMapped = new Dictionary<NodeId, MonitoredItemViewModel>();

        bool IgnoreUpdateNodeId;
        bool bReady;

        readonly Dictionary<String, NodeIdViewModel> NodeMapped = new Dictionary<String, NodeIdViewModel>();
        readonly Dictionary<NodeId, NodeIdViewModel> NodeMappedNodeIds = new Dictionary<NodeId, NodeIdViewModel>();

        readonly List<String> itemsBlackList = new List<String>();

        readonly Dictionary<String, NodeId> mapUpdateNodeIdRequest = new Dictionary<String, NodeId>();

        readonly Dictionary<String, List<IEntityReference>> pendingInUseItems = new Dictionary<String, List<IEntityReference>>();
        readonly Dictionary<String, List<IEntityReference>> pendingNotInUseItems = new Dictionary<String, List<IEntityReference>>();
        readonly Dictionary<String, int> inUseItemCounter = new Dictionary<String, int>();

        readonly Dictionary<NodeId, List<IEntityReference>> pendingInUseNodeIds = new Dictionary<NodeId, List<IEntityReference>>();
        readonly Dictionary<NodeId, List<IEntityReference>> pendingNotInUseNodeIds = new Dictionary<NodeId, List<IEntityReference>>();
        readonly Dictionary<NodeId, int> inUseNodeIdCounter = new Dictionary<NodeId, int>();

        readonly List<String> listNodeIdToDiscover = new List<String>();
        readonly List<String> listNodeIdToDiscovered = new List<String>();

        readonly static Dictionary<String, RealTimeConnectionManagerViewModel> mapActiveRealTimeConnectionManager = new Dictionary<String, RealTimeConnectionManagerViewModel>();
        public static SafeObservableCollection<RealTimeConnectionManagerViewModel> listActiveRealTimeConnectionManagers = new SafeObservableCollection<RealTimeConnectionManagerViewModel>();

        public event EventHandler<MonitoreItemListChangedEventArgs> MonitoringItemListchanged;
        public event EventHandler<NodeIdDiscoveredEventArgs> NodeIdDiscovered;
        public event EventHandler<BlackListChangedEventArgs> BlackListChanged;

        readonly static Dictionary<String, SessionSettings> mapSessionSettings = new Dictionary<String, SessionSettings>();
        readonly static SessionSettings defaultSessionSettings = new SessionSettings();

        //static long maxConcurrentItems = 0;
        //static long currentConcurrentItems = 0;

        String savedHostName;
        String savedAppName;
        bool bPublishEnabled = true;
        bool bAllFetched = false;
        bool bFetching = false;

        #endregion

        #region Constructor

        readonly static Timer timerCleanDeadConnection;
        static RealTimeConnectionManagerViewModel()
        {
//#if !DEBUG && !WINDOWS_UWP
//            maxConcurrentItems = MSZ.MSZView.GetModule("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxc85bCd+oTeUhbJmLGsEXBw=="/* CTG */);
//#else
//            maxConcurrentItems = long.MaxValue;
//#endif

            timerCleanDeadConnection = new Timer((o) =>
                        {
                            try
                            {
                                CleanDeadConnections();
                            }
                            catch (Exception ex)
                            {
                            }
                        }, null, 30000, 30000);
        }

        public RealTimeConnectionManagerViewModel(String sessionname, TreeViewItemViewModel parent)
            : base(parent, false)
        {
            Title = parent.Parent is ApplicationDescriptionViewModel ? parent.Parent.Title : parent.Title;
            sessionName = sessionname;
            if (String.IsNullOrEmpty(sessionName))
                sessionName = Properties.Resource.DefaultSessionName;

            CommonConstruct();
        }

        public RealTimeConnectionManagerViewModel(String sessionname, String hostName, String appName, String endpointUrl,
            bool publishEnabled = true)
            : base(null, false)
        {
            HostName = savedHostName = hostName;
            AppName = savedAppName = appName;
            EndpointUrl = endpointUrl;
            sessionName = sessionname;
            bPublishEnabled = publishEnabled;
            if (String.IsNullOrEmpty(sessionName))
                sessionName = Properties.Resource.DefaultSessionName;

            Title = GetComposedTitle(sessionName, HostName, AppName);

            CommonConstruct();
        }

        void CommonConstruct()
        {
            lastTimeUsed = DateTime.Now;

            lock (mapActiveRealTimeConnectionManager)
            {
                if (listActiveRealTimeConnectionManagers.Count == 0)
                {
                    lastHostName = null;
                    lastInErrorConnectionState = false;
                    //currentConcurrentItems = 0;
                }

                if (!listActiveRealTimeConnectionManagers.Contains(this))
                    listActiveRealTimeConnectionManagers.Add(this);

                mapActiveRealTimeConnectionManager.Add(GetComposedTitle(sessionName, HostName, AppName), this);
            }
        }
        #endregion

        #region Sessions

        public static SessionSettings GetSession(String name)
        {
            lock (mapSessionSettings)
            {
                if (mapSessionSettings.ContainsKey(name))
                    return mapSessionSettings[name];

                var nameComposed = String.Format("-{0}", name);
                var found = (from c in mapSessionSettings where c.Key.EndsWith(nameComposed) select c.Value).ToList();
                if (found.Count > 0)
                    return found[0];
                int n = name.LastIndexOf('-');
                if (n >= 0)
                {
                    nameComposed = name.Substring(n + 1);
                    found = (from c in mapSessionSettings where c.Key.EndsWith(nameComposed) select c.Value).ToList();
                    if (found.Count > 0)
                        return found[0];
                }
            }

            return defaultSessionSettings;
        }

        public static void AddSessionSettings(String name, SessionSettings settings)
        {
            lock (mapSessionSettings)
            {
                if (mapSessionSettings.ContainsKey(name))
                    mapSessionSettings.Remove(name);
                mapSessionSettings.Add(name, settings);
            }
        }

        public static string ReplaceServerRenamedOnDataSource(string settings, string sessionString)
        {
#if !WINDOWS_UWP
            var server = XpoHelpers.XpoHelper.GetDataSourceServer(settings, checkprovidertype: false);
            if (server != null)
            {
                var snames = server.Split('\\');

                var newserver = GetSessionServerRenamed(sessionString, snames[0]);
                if (newserver != snames[0])
                {
                    for (int i = 1; snames.Length > i; ++i)
                        newserver = String.Format("{0}\\{1}", newserver, snames[i]);
                    var ret = XpoHelpers.XpoHelper.SetDataSourceServer(settings, newserver, checkprovidertype: false);
                    if (ret != null)
                        settings = ret;
                }
            }
#endif
            return settings;
        }

        public static String GetSessionServerRenamed(String sessionName, String server)
        {
            var settings = GetSession(sessionName);
            var hostname = server;
            var mapSettings = settings.MapAppNameSettings;

            //if server is not in ServerArray means the client needs to connect to a machine which is not
            //used as redundancy server
            if(settings.ServerArray != null)
            {
                var lowerServerArray = settings.ServerArray.Select(s => s.ToLower()).ToList();
                if (!lowerServerArray.Contains(server.ToLower()) &&
                    !server.Contains("(local)"))
                    return server;
            }
            
            
            if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
            {
                var parent = GetSession(settings.ParentTitle);
                if (parent != null)
                    mapSettings = parent.MapAppNameSettings;
            }
            var appNameSettings = (from c in mapSettings
                                   where c.Key == server || server.Contains("(local)")
                                   select c.Value).DefaultIfEmpty(new AppNameSettings(settings)).ToList();

            if (!String.IsNullOrEmpty(appNameSettings[0].HostNameRenamed))
                hostname = appNameSettings[0].HostNameRenamed;

            lock (mapActiveRealTimeConnectionManager)
            {
                var list = (from entry in mapActiveRealTimeConnectionManager/*.AsParallel()*/
                            where entry.Key.Contains(String.Format("{0}@", sessionName)) &&
                                  entry.Value.SessionViewModel != null && entry.Value.SessionViewModel.Connected
                                  && entry.Value.SessionName == sessionName && entry.Value.ServerUriArray != null
                            select entry.Value).ToList();
                var listLocal = (from c in list where String.IsNullOrEmpty(c.HostName) select c).ToList();
                if (listLocal.Count == 0 && list.Count > 0)
                {
                    var serverUri = list[0].ServerUriArray;
                    if (serverUri != null)
                    {
                        var listUri = serverUri.ToList();
                        var listHost = (from entry in mapActiveRealTimeConnectionManager/*.AsParallel()*/
                                        where entry.Key.Contains(String.Format("{0}@", sessionName)) &&
                                              entry.Value.SessionViewModel != null && entry.Value.SessionViewModel.Connected
                                              && listUri.Contains(entry.Value.HostName, StringComparer.OrdinalIgnoreCase)
                                        orderby entry.Value.ServiceLevel descending
                                        select entry.Value.HostName).ToList();
                        if (listHost.Count > 0)
                            hostname = listHost[0];
                    }
                }
            }

            return hostname;
        }

        public static bool IsSessionServerRemote(String sessionname, String appname)
        {
            var thisMachine = System.Net.Dns.GetHostName();
            var settings = RealTimeConnectionManagerViewModel.GetSession(sessionname);
            var mapSettings = settings.MapAppNameSettings;
            if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
            {
                var parent = RealTimeConnectionManagerViewModel.GetSession(settings.ParentTitle);
                if (parent != null)
                    mapSettings = parent.MapAppNameSettings;
            }

            var appNameSettings = (from c in mapSettings
                                   where c.Key == appname
                                   select c.Value).FirstOrDefault();

            return appNameSettings != null && !String.IsNullOrWhiteSpace(appNameSettings.HostNameRenamed) &&
                String.Compare(appNameSettings.HostNameRenamed, "localhost", StringComparison.OrdinalIgnoreCase) != 0 &&
                String.Compare(appNameSettings.HostNameRenamed, thisMachine, StringComparison.OrdinalIgnoreCase) != 0;
        }

        static string lastHostName;
        static public void NotifyRedundancyHostNameUsed(String hostName)
        {
            if (lastHostName != hostName)
            {
                lastHostName = hostName;
                if (!String.IsNullOrEmpty(hostName))
                {
                    var sysVariables = OPCUAEntityReference.GetDataSinkInterface(SysVariables.SysNames.dataSynkName);
                    if (sysVariables != null)
                        sysVariables.UpdateVariable(SysVariables.SysNames.LastHostNameUsed, new DataValue(new Variant(hostName), StatusCodes.Good));
                }
            }
        }

        static bool lastInErrorConnectionState;
        static void NotifyServerConnectionsInError(bool inError)
        {
            if (lastInErrorConnectionState != inError)
            {
                lastInErrorConnectionState = inError;
                var sysVariables = OPCUAEntityReference.GetDataSinkInterface(SysVariables.SysNames.dataSynkName);
                if (sysVariables != null)
                    sysVariables.UpdateVariable(SysVariables.SysNames.ServerConnectionError, new DataValue(new Variant(inError), StatusCodes.Good));
            }
        }

        static bool CheckServerConnectionsInError()
        {
            var list = new List<RealTimeConnectionManagerViewModel>();
            lock (mapActiveRealTimeConnectionManager)
            {
                list.AddRange(listActiveRealTimeConnectionManagers);
            }

            return (from c in list
                    where c.SessionViewModel == null || !c.SessionViewModel.Connected
                    select c).FirstOrDefault() != null;
        }

        public static List<String> GetActiveAppNames()
        {
            lock (mapActiveRealTimeConnectionManager)
            {
                return (from c in listActiveRealTimeConnectionManagers select c.AppName).ToList();
            }
        }

        #endregion

        #region Commands

        #endregion

        #region Methods

        static String GetComposedTitle(String sessionname, String hostname, String appname)
        {
            if (String.IsNullOrEmpty(sessionname))
                sessionname = Properties.Resource.DefaultSessionName;

            var settings = GetSession(sessionname);
            var mapSettings = settings.MapAppNameSettings;
            if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
            {
                var parent = GetSession(settings.ParentTitle);
                if (parent != null)
                    mapSettings = parent.MapAppNameSettings;
            }
            var appNameSettings = (from c in mapSettings
                                   where c.Key == appname
                                   select c.Value).DefaultIfEmpty(new AppNameSettings(settings)).Single();

            if (!String.IsNullOrEmpty(appNameSettings.AppNameRenamed))
            {
                appname = appNameSettings.AppNameRenamed;
            }
            if (!String.IsNullOrEmpty(appNameSettings.HostNameRenamed))
            {
                var thisMachine = Dns.GetHostName();
                if (String.Compare(thisMachine, appNameSettings.HostNameRenamed, true) != 0)
                {
                    hostname = appNameSettings.HostNameRenamed;
                }
            }

            if (!String.IsNullOrEmpty(hostname) &&
                String.Compare(hostname, Properties.Settings.Default.localhost, true) != 0 &&
                String.Compare(hostname, Properties.Settings.Default.localhostip, true) != 0)
                return String.Format("{0}@{1}\\{2}", sessionname, hostname, appname);

            return String.Format("{0}@{1}", sessionname, appname);
        }

        static Dictionary<String, IUserIdentity> mapSessionUsers = new Dictionary<string, IUserIdentity>();
        static Dictionary<String, StringCollection> mapSessionLocales = new Dictionary<string, StringCollection>();
        static Dictionary<String, String> mapSessionUserName = new Dictionary<string, string>();
        public static void SetUserIdentity(String sessionname, IUserIdentity identity, StringCollection localeIds, bool bIgnoreConnection = false)
        {
            if (String.IsNullOrEmpty(sessionname))
                sessionname = Properties.Resource.DefaultSessionName;

            lock (mapActiveRealTimeConnectionManager)
            {
                var userName = identity?.DisplayName;
                if (mapSessionUserName.ContainsKey(sessionname) &&
                    mapSessionUserName[sessionname] == userName)
                    return;
                if (mapSessionUserName.ContainsKey(sessionname))
                    mapSessionUserName.Remove(sessionname);
                if (!String.IsNullOrEmpty(userName))
                    mapSessionUserName.Add(sessionname, userName);

                if (mapSessionUsers.ContainsKey(sessionname))
                    mapSessionUsers.Remove(sessionname);
                if (mapSessionLocales.ContainsKey(sessionname))
                    mapSessionLocales.Remove(sessionname);
                if (identity != null)
                {
                    mapSessionUsers.Add(sessionname, identity);
                    mapSessionLocales.Add(sessionname, localeIds);
                }

                var list1 = (from entry in mapActiveRealTimeConnectionManager/*.AsParallel()*/
                             where entry.Key.Contains(String.Format("{0}@", sessionname)) &&
                                   entry.Value.SessionViewModel != null && (entry.Value.SessionViewModel.Connected || bIgnoreConnection)
                             select entry.Key).ToList();
                var listEx = new List<Exception>();
                list1.ForEach(n =>
                    {
                        try
                        {
                            mapActiveRealTimeConnectionManager[n].sessionViewModel.RenewUserIdentity(identity, localeIds);
                        }
                        catch (Exception ex)
                        {
                            if (mapActiveRealTimeConnectionManager[n].sessionViewModel != null &&
                                mapActiveRealTimeConnectionManager[n].sessionViewModel.Connected)
                                listEx.Add(ex);
                        }
                    });

                listEx.ForEach(ex =>
                {
                    throw ex;
                });
            }

            OPCUAEntityReference.RefreshAllDataSinkVariables();
        }

        void UpdateSessionUserIdentity()
        {
            lock (mapActiveRealTimeConnectionManager)
            {
                if (mapSessionUsers.ContainsKey(sessionName) && mapSessionLocales.ContainsKey(sessionName))
                {
                    try
                    {
                        sessionViewModel.RenewUserIdentity(mapSessionUsers[sessionName], mapSessionLocales[sessionName]);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static RealTimeConnectionManagerViewModel Find(String sessionname, String hostname, String appname, String endpointUrl)
        {
            lock (mapActiveRealTimeConnectionManager)
            {
                RealTimeConnectionManagerViewModel ret;
                if (mapActiveRealTimeConnectionManager.TryGetValue(GetComposedTitle(sessionname, hostname, appname), out ret))
                    return ret;
            }

            return null;
        }

        public static RealTimeConnectionManagerViewModel FindOrCreate(String sessionname, String hostname, String appname, String endpointUrl,
            bool bDelayed = true, bool bPublishEnabled = true)
        {
            lock (mapActiveRealTimeConnectionManager)
            {
                RealTimeConnectionManagerViewModel ret;
                if (mapActiveRealTimeConnectionManager.TryGetValue(GetComposedTitle(sessionname, hostname, appname), out ret))
                    return ret;

                ret = new RealTimeConnectionManagerViewModel(sessionname, hostname, appname, endpointUrl, bPublishEnabled);
                if (bDelayed)
                    ret.PromoteIdleExecution(250);
                else
                    ret.Create();
                return ret;
            }
        }

        public static void CleanDeadConnections(bool bForce = false)
        {
            lock (mapActiveRealTimeConnectionManager)
            {
                if (bForce)
                {
                    mapActiveRealTimeConnectionManager.Values.ToList().ForEach(c =>
                    {
                        c.SetToBeDisposed();
                    });
                }
                else
                {
                    var list = (from c in mapActiveRealTimeConnectionManager.Keys
                                where mapActiveRealTimeConnectionManager[c].IsEmpty()
                                select c).ToList();
                    list.ForEach(c =>
                    {
                        mapActiveRealTimeConnectionManager[c].SetToBeDisposed();
                    });
                }
                /*
                if (bForce)
                {
                    mapActiveRealTimeConnectionManager.Values.ToList().ForEach(item => 
                        {
                            item.ElaborateInUseRequests(false);
                            item.CleanDisabledMonitoredItems(bForce);
                        });

                }

                var list = (from c in mapActiveRealTimeConnectionManager.Keys
                            where mapActiveRealTimeConnectionManager[c].IsEmpty() || bForce
                            select c).ToList();
                list.ForEach(c =>
                    {
                        mapActiveRealTimeConnectionManager[c].SetToBeDisposed();
                    });
                */
            }
        }

        DateTime lastTimeUsed;

        public static void CleanSpecificDeadConnection(string name)
        {
            var list = (from c in mapActiveRealTimeConnectionManager.Keys
                        where c.Contains(name)
                        select c).ToList();
            list.ForEach(c =>
            {
                mapActiveRealTimeConnectionManager[c].SetToBeDisposed();
            });
        }

        public bool IsEmpty()
        {
            lock (lockObject)
            {
                if (lastTimeUsed.AddSeconds(60) > DateTime.Now)
                    return false;
                
                if (bFetching)
                    return false;
                /*
                if (sessionViewModel == null || SubscriptionViewModel == null)
                    return true;
                */

                var inUseItem = (from c in inUseItemCounter.Keys where inUseItemCounter[c] > 0 select c).ToList();
                var inUseNode = (from c in inUseNodeIdCounter.Keys where inUseNodeIdCounter[c] > 0 select c).ToList();
                return inUseItem.Count == 0 && inUseNode.Count == 0 && mapUpdateNodeIdRequest.Count == 0/* && 
                    pendingInUseNodeIds.Count == 0 && pendingInUseItems.Count == 0*/;
            }
        }

        PropertyObserver<SessionViewModel> observer;
        bool bRefreshInIdle;
        bool Create()
        {
            var settings = GetSession(sessionName);
            var endpoint = EndpointUrl;
            var appname = AppName;
            var hostname = HostName;
            var mapSettings = settings.MapAppNameSettings;
            if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
            {
                var parent = GetSession(settings.ParentTitle);
                if (parent != null)
                    mapSettings = parent.MapAppNameSettings;
            }
            var appNameSettings = (from c in mapSettings 
                                   where c.Key == appname
                                   select c.Value).DefaultIfEmpty(new AppNameSettings(settings)).Single();

            if (appNameSettings.AlwaysDiscoverEndpoint || appNameSettings.UseAlwaysSecureConnections)
                endpoint = String.Empty;
            if (!String.IsNullOrEmpty(appNameSettings.EndpointRenamed))
                endpoint = appNameSettings.EndpointRenamed;
            if (!String.IsNullOrEmpty(appNameSettings.AppNameRenamed))
            {
                appname = appNameSettings.AppNameRenamed;
                endpoint = String.Empty;
            }
            if (!String.IsNullOrEmpty(appNameSettings.HostNameRenamed))
            {
                var thisMachine = Dns.GetHostName();
                if (String.Compare(thisMachine, appNameSettings.HostNameRenamed, true) != 0)
                {
                    hostname = appNameSettings.HostNameRenamed;
                    endpoint = String.Empty;
                }
            }
            if (!bPublishEnabled)
                endpoint = String.Empty;

            bool useSecurity = settings.UseAlwaysSecureConnections;
            if (mapSettings.ContainsKey(appname))
                useSecurity = appNameSettings.UseAlwaysSecureConnections;
#if !WINDOWS_UWP
            if (!useSecurity && !String.IsNullOrEmpty(hostname) && appNameSettings.UseSecurityWhenNotLocal &&
                !LanExtensions.IsLanIP(hostname))
                useSecurity = true;
#endif
            try
            {
                sessionViewModel = SessionViewModel.FindOrCreate(sessionName, hostname, appname, endpoint, useSecurity);
                if (sessionViewModel == null)
                    throw new Exception(String.Format("No endpoints found on {0}", hostname));
                EndpointUrl = endpoint;
                AppName = appname;
                HostName = hostname;
                OnPropertyChanged("SessionViewModel");
                bFetching = false;
            }
            catch (Exception ex)
            {
                if (!String.IsNullOrEmpty(appNameSettings.BackupAppName) ||
                    !String.IsNullOrEmpty(appNameSettings.BackupHostName))
                {
                    if (!String.IsNullOrEmpty(appNameSettings.BackupAppName))
                        appname = appNameSettings.BackupAppName;
                    if (!String.IsNullOrEmpty(appNameSettings.BackupHostName))
                        hostname = appNameSettings.BackupHostName;
                    endpoint = String.Empty;

#if !WINDOWS_UWP
                    if (!useSecurity && !String.IsNullOrEmpty(hostname) && appNameSettings.UseSecurityWhenNotLocal &&
                        !LanExtensions.IsLanIP(hostname))
                        useSecurity = true;
#endif
                    try
                    {
                        sessionViewModel = SessionViewModel.FindOrCreate(sessionName, hostname, appname, endpoint, useSecurity);
                        if (sessionViewModel == null)
                            throw new Exception(String.Format("No endpoints found on {0}", hostname));
                        EndpointUrl = endpoint;
                        AppName = appname;
                        HostName = hostname;
                        OnPropertyChanged("SessionViewModel");
                    }
                    catch (Exception exe)
                    {
                        LastMessage = exe.Message;
                        PromoteIdleExecution(2000);
                        return false;
                    }
                }
                else
                {
                    LastMessage = ex.Message;
                    PromoteIdleExecution(2000);
                    return false;
                }
            }
            if (sessionViewModel == null)
            {
                PromoteIdleExecution(2000);
                return false;
            }
            if (!sessionViewModel.Connected)
            {
                if (observer != null)
                    observer.Dispose();

                observer = new PropertyObserver<SessionViewModel>(sessionViewModel)
                    .RegisterHandler(n => n.Connected, n =>
                        {
                            if (n.Connected)
                            {
                                observer.UnregisterHandler(p => p.Connected);

                                // Initialize();
                                bRefreshInIdle = true;
                                n.PromoteIdleExecution(500);
                                PromoteIdleExecution(100);
                            }
                            else
                            {
                                bReady = false;
                                bFetching = false;
                                PromoteIdleExecution(2000);
                            }
                        });

                sessionViewModel.AutoConnect = true;
                sessionViewModel.PromoteIdleExecution(500);
                PromoteIdleExecution(1000);
            }
            else
            {
                // Initialize();
                sessionViewModel.PromoteIdleExecution(500);
                PromoteIdleExecution(1000);

                UpdateSessionUserIdentity();
                FetchShutdownSupport();
            }

            return true;
        }

        internal NodeIdViewModel FindNodeIdItem(String StartingAddress, String RelativePath)
        {
            var item = GetItemInstanceString(RelativePath, StartingAddress);
            lock (lockObject)
            {
                if (!NodeMapped.ContainsKey(item))
                    return null;
                return NodeMapped[item];
            }
        }

        internal NodeIdViewModel FindNodeIdItem(NodeId nodeid)
        {
            lock (lockObject)
            {
                if (!NodeMappedNodeIds.ContainsKey(nodeid))
                    return null;
                return NodeMappedNodeIds[nodeid];
            }
        }
        
        internal MonitoredItemViewModel FindMonitoredItem(String StartingAddress, String RelativePath)
        {
            var item = GetItemInstanceString(RelativePath, StartingAddress);
            if (String.IsNullOrEmpty(item))
                return null;

            lock (lockObject)
            {
                if (!NodeMapped.ContainsKey(item))
                    return null;
                
                if (!MonitoredItemMapped.ContainsKey((NodeId)NodeMapped[item].nodeId))
                    return null;

                return MonitoredItemMapped[(NodeId)NodeMapped[item].nodeId];
            }
        }

        internal MonitoredItemViewModel FindMonitoredItem(NodeId nodeid)
        {
            if (NodeId.IsNull(nodeid))
                return null;

            lock (lockObject)
            {
                if (!MonitoredItemMapped.ContainsKey(nodeid))
                    return null;

                return MonitoredItemMapped[nodeid];
            }
        }

        private void Initialize()
        {
            if (bFetching || sessionViewModel == null)
                return;
            bFetching = true;
            if (NamespacesUris == null)
                NamespacesUris = sessionViewModel.NamespaceUris;
            //IgnoreUpdateNodeId = sessionViewModel.NamespaceUris != NamespacesUris;

            var settings = GetSession(sessionName);
            if (settings == null)
                return;

            /*
            if (SubscriptionViewModel != null)
            {
                try
                {
                    sessionViewModel.RemoveSubscription(SubscriptionViewModel);
                }
                catch (Exception ex)
                {
                    LastMessage = ex.Message;                    
                }
                finally
                {
                    try
                    {
                        SubscriptionViewModel.Dispose();
                    }
                    catch (Exception ex)
                    {
                        LastMessage = ex.Message;                                            
                    }
                    SubscriptionViewModel = null;
                }
            }
            */

            AppNameSettings appNameSettings = null;
            var mapSettings = settings.MapAppNameSettings;
            if (mapSettings != null)
            {
                if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
                {
                    var parent = GetSession(settings.ParentTitle);
                    if (parent != null)
                        mapSettings = parent.MapAppNameSettings;
                }
                appNameSettings = (from c in mapSettings where c.Key == AppName select c.Value).FirstOrDefault();
            }
            // if (SubscriptionViewModel == null)
                SubscriptionViewModel = sessionViewModel.CreateSubscription(String.Format(Properties.Resource.NormalSubscriptionName, HostName, AppName),
                    appNameSettings != null ? appNameSettings.UsePollingRead : false);

            if (SubscriptionViewModel != null)
            {
                if (appNameSettings != null)
                {
                    SubscriptionViewModel.PublishingInterval = appNameSettings.PublishingInterval;
                }
                else
                    SubscriptionViewModel.PublishingInterval = settings.PublishingInterval;

                SubscriptionViewModel.PublishingEnabled = bPublishEnabled;

                try
                {
                    SubscriptionViewModel.subscription.Modify();
                }
                catch (Exception ex)
                {
                    // sever may not support subscription changing 
                    LastMessage = ex.Message;
                }

#if !NET_STANDARD
                FetchServerCapabilities();
#endif
                FetchRedundancySupport();
                    
                if (bAllFetched)
                {
                    bReady = true;
                    bFetching = false;
                }

                PromoteIdleExecution(200);
            }
            else
                PromoteIdleExecution(2000);
        }

        /*
        void RestorePublish()
        {
            SubscriptionViewModel.PublishingEnabled = bPublishEnabled;

            try
            {
                SubscriptionViewModel.subscription.Modify();
            }
            catch (Exception ex)
            {
                // sever may not support subscription changing 
                LastMessage = ex.Message;
            }
        }
        */

        protected override void IdleExecution()
        {
            if (bToBeDisposed)
                Dispose();

            if (bDisposed)
                return;

            if (sessionViewModel != null && !sessionViewModel.DataTypeFetched)
                PromoteIdleExecution(1000);
            else if (!bReady && sessionViewModel != null && sessionViewModel.Connected)
                Initialize();
            else if (bReady && sessionViewModel != null && sessionViewModel.Connected)
            {
                ElaborateInUseRequests();
                CleanDisabledMonitoredItems();
                PromoteIdleExecution(2000);
            }
            else if (bReady && sessionViewModel != null && !sessionViewModel.Connected)
            {
                ElaborateInUseRequests(false);
                CleanDisabledMonitoredItems();
                CleanDeadConnections();
                var bResult = Create();
                if (!bResult || !sessionViewModel.Connected)
                    NotifyServerConnectionsInError(true);
            }
            else if (sessionViewModel == null || !sessionViewModel.Connected)
            {
                ElaborateInUseRequests(false);
                var bResult = Create();
                if (!bResult || !sessionViewModel.Connected)
                    NotifyServerConnectionsInError(true);
            }

            if (bRefreshInIdle && sessionViewModel != null && sessionViewModel.Connected)
            {
                bRefreshInIdle = false;
                UpdateSessionUserIdentity();
                FetchShutdownSupport();

                var inError = CheckServerConnectionsInError();
                NotifyServerConnectionsInError(inError);
            }

            PromoteIdleExecution(1000);
        }

        void SetToBeDisposed()
        {
            bToBeDisposed = true;
            PromoteIdleExecution(1);
        }

        bool bToBeDisposed;
        bool bDisposed;
        protected override void OnDispose()
        {
            bDisposed = true;

            base.OnDispose();

            DisposeShutdown();
            DisposeRedundancy();

            lock (lockObject)
            {
                foreach (var v in MonitoredItemMapped.Values)
                    v.Dispose();
                MonitoredItemMapped.Clear();

                foreach (var v in NodeMapped.Values)
                    v.Dispose();
                NodeMapped.Clear();
                foreach (var v in NodeMappedNodeIds.Values)
                    v.Dispose();
                NodeMappedNodeIds.Clear();

                if (sessionViewModel != null && SubscriptionViewModel != null)
                {
                    sessionViewModel.RemoveSubscription(SubscriptionViewModel);
                    SubscriptionViewModel.Dispose();
                    SubscriptionViewModel = null;
                }

                if (sessionViewModel != null)
                {
                    sessionViewModel.Dispose();
                    sessionViewModel = null;
                }
            }

            lock (mapActiveRealTimeConnectionManager)
            {
                if (listActiveRealTimeConnectionManagers.Contains(this))
                    listActiveRealTimeConnectionManagers.Remove(this);

                var title = GetComposedTitle(sessionName, savedHostName, savedAppName);
                if (mapActiveRealTimeConnectionManager.ContainsKey(title))
                    mapActiveRealTimeConnectionManager.Remove(title);

                if (mapSessionUserName.ContainsKey(sessionName))
                    mapSessionUserName.Remove(sessionName);
                if (mapSessionUsers.ContainsKey(sessionName))
                    mapSessionUsers.Remove(sessionName);
                if (mapSessionLocales.ContainsKey(sessionName))
                    mapSessionLocales.Remove(sessionName);
                //var list1 = (from entry in mapActiveRealTimeConnectionManager/*.AsParallel()*/ where entry.Key.Contains(String.Format("{0}@", sessionName)) select entry.Key).ToList();
                //list1.ForEach(n =>
                //    {
                //        mapActiveRealTimeConnectionManager.Remove(n);
                //    });

                var inError = CheckServerConnectionsInError();
                NotifyServerConnectionsInError(inError);
            }

#if !WINDOWS_UWP && !NET_STANDARD
            MonitoringItemListchanged.CheckEventHasNoSubscribers();
            NodeIdDiscovered.CheckEventHasNoSubscribers();
            BlackListChanged.CheckEventHasNoSubscribers();
#endif
            if (observer != null)
            {
                observer.Dispose();
                observer = null;
            }

            //if (observerSecondsTillShutdown != null)
            //{
            //    observerSecondsTillShutdown.Dispose();
            //    observerSecondsTillShutdown = null;
            //}
        }

        public void SetInUse(NodeId nodeId, bool bInUse, IEntityReference subscriber = null)
        {
            Dictionary<NodeId, List<IEntityReference>> pending = bInUse ? pendingInUseNodeIds : pendingNotInUseNodeIds;

            lastTimeUsed = DateTime.Now;

            lock (lockObject)
            {
                List<IEntityReference> List;
                if (!pending.TryGetValue(nodeId, out List))
                {
                    List = new List<IEntityReference>();
                    pending.Add(nodeId, List);
                }
                if (subscriber != null)
                {
                    //List.Add(subscriber);
                }

                if (!inUseNodeIdCounter.ContainsKey(nodeId))
                    inUseNodeIdCounter.Add(nodeId, 0);
                if (bInUse)
                    inUseNodeIdCounter[nodeId] += 1;
                else
                    inUseNodeIdCounter[nodeId] -= 1;
#if DEBUGTRACE
                System.Diagnostics.Debug.WriteLine(String.Format("SetInUse for {0}, inUse {1}, subscriber {2}, inUseNodeIdCounter {3}, AppName {4}",
                    nodeId, bInUse, subscriber?.GetType(), inUseNodeIdCounter[nodeId], Title));
#endif
            }

            if (bReady)
                PromoteIdleExecution(100);
        }

        public void SetInUse(List<String> listItems, bool bInUse, IEntityReference subscriber = null)
        {
            if (listItems.Count == 0)
                return;

            lastTimeUsed = DateTime.Now;

            Dictionary<String, List<IEntityReference>> pending = bInUse ? pendingInUseItems : pendingNotInUseItems;

            lock (lockObject)
            {
                listItems.ForEach(item =>
                    {
                        List<IEntityReference> List;
                        if (!pending.TryGetValue(item, out List))
                        {
                            List = new List<IEntityReference>();
                            pending.Add(item, List);
                        }

                        if (subscriber != null)
                        {
                            //List.Add(subscriber);
                        }

                        if (!inUseItemCounter.ContainsKey(item))
                            inUseItemCounter.Add(item, 0);
                        if (bInUse)
                            inUseItemCounter[item] += 1;
                        else
                            inUseItemCounter[item] -= 1;
#if DEBUGTRACE
                        System.Diagnostics.Debug.WriteLine(String.Format("SetInUse for {0}, inUse {1}, subscriber {2}, inUseItemCounter {3}, AppName {4}",
                                    item, bInUse, subscriber?.GetType(), inUseItemCounter[item], Title));
#endif
                    });
            }

            if (bReady)
                PromoteIdleExecution(100);
        }

        static readonly Char InstanceSeparator = '@';
        public static String GetItemInstanceString(String item, String instance)
        {
            if (String.IsNullOrEmpty(instance))
                return item;
            return String.Format("{0}{1}{2}", instance, InstanceSeparator, item);
        }

        static bool ItemsContainsInstance(String path)
        {
            return path.IndexOf(InstanceSeparator) >= 0;
        }

        static String GetItemRelativePath(String path)
        {
            if (!ItemsContainsInstance(path))
                return path;
            String[] paths = path.Split(InstanceSeparator);
            if (paths.Length < 2)
                return path;

            return paths[1].Trim();
        }

        NodeId GetItemInstanceId(String path)
        {
            if (bDisposed)
                return null;

            if (!ItemsContainsInstance(path))
                return Objects.ObjectsFolder;

            String[] paths = path.Split(InstanceSeparator);
            if (paths.Length == 0)
                return Objects.ObjectsFolder;

            String sInstance = paths[0].Trim();
            if (String.IsNullOrEmpty(sInstance))
                return Objects.ObjectsFolder;

            var map = new Dictionary<String, NodeId>();
            var mapModels = new Dictionary<String, NodeIdViewModel>();

            lock (lockObject)
            {
                if (itemsBlackList.Contains(sInstance))
                    return null;

                if (!NodeMapped.ContainsKey(sInstance))
                {
                    var ret = sessionViewModel.GetNodeIds(Objects.ObjectsFolder, NamespacesUris, sInstance);
                    if (ret.ContainsKey(sInstance))
                    {
                        var model = new NodeIdViewModel(ret[sInstance], sessionViewModel);
                        // if (model.NodeAttributes.Count > 0) // reads attributes and check if they are valid
                        {
                            NodeMapped.Add(sInstance, model);

                            map.Add(sInstance, (NodeId)model.nodeId);
                            mapModels.Add(sInstance, model);
                        }
                    }
                }
                else
                {
                    map.Add(sInstance, (NodeId)NodeMapped[sInstance].nodeId);
                    mapModels.Add(sInstance, NodeMapped[sInstance]);
                }
            }

            if (mapModels.ContainsKey(sInstance))
            {
                lock (lockObject)
                {
                    if (itemsBlackList.Contains(sInstance))
                        itemsBlackList.Remove(sInstance);
                }

                OnNodeIdDiscovered(new NodeIdDiscoveredEventArgs(map, mapModels, true));
                return (NodeId)mapModels[sInstance].nodeId;
            }

            lock (lockObject)
            {
                itemsBlackList.Add(sInstance);
            }

            var list = new List<String>();
            list.Add(sInstance);
            OnBlackListChanged(new BlackListChangedEventArgs(list, true));

            return null;
        }

        public NodeId GetNodeId(String item)
        {
            lock (lockObject)
            {
                if (NodeMapped.ContainsKey(item))
                    return (NodeId)NodeMapped[item].nodeId;
            }

            return null;
        }

        public List<NodeId> GetNodeId(List<String> listItem)
        {
            var listNode = new List<NodeId>();
            lock (lockObject)
            {
                listItem.ForEach(item =>
                    {
                        listNode.Add(NodeMapped.ContainsKey(item) ? (NodeId)NodeMapped[item].nodeId : new NodeId(0));
                    });
            }

            return listNode;
        }

        void UpdateNodeMap(Dictionary<String, NodeId> map)
        {
            if (IgnoreUpdateNodeId || bDisposed)
                return;

            // var list = new List<String>();
            var mapModels = new Dictionary<String, NodeIdViewModel>();
            lock (lockObject)
            {
                foreach (var item in map.Keys)
                {
                    if (!NodeMapped.ContainsKey(item))
                    {
                        var model = new NodeIdViewModel(map[item], sessionViewModel);
                        if (model.Exist)
                        // if (model.NodeAttributes.Count > 0) // reads attributes and checks if they are valid
                        {
                            NodeMapped.Add(item, model);
                            mapModels.Add(item, model);
                        }
                        else
                            DiscoverNodeId(null, item);
                        //else if (!itemsBlackList.Contains(item))
                        //{
                        //    itemsBlackList.Add(item);
                        //    list.Add(item);
                        //}
                    }
                }
            }

            if (mapModels.Count > 0)
                OnNodeIdDiscovered(new NodeIdDiscoveredEventArgs(map, mapModels, false));
            //if (list.Count > 0)
            //    OnBlackListChanged(new BlackListChangedEventArgs(list, false));
        }

        public void DiscoverNodeId(String startingAddress, String item)
        {
            var instance = GetItemInstanceString(item, startingAddress);
            lock (lockObject)
            {
                if (!listNodeIdToDiscover.Contains(instance) && !listNodeIdToDiscovered.Contains(instance))
                {
                    listNodeIdToDiscover.Add(instance);
                    listNodeIdToDiscovered.Add(instance);

                    if (bReady)
                        PromoteIdleExecution(100);
                }
            }
        }

        public void UpdateNodeMapRequest(String startingAddress, String item, NodeId node)
        {
            var instance = GetItemInstanceString(item, startingAddress);
            lock (lockObject)
            {
                if (!mapUpdateNodeIdRequest.ContainsKey(instance) || mapUpdateNodeIdRequest[instance] != node)
                {
                    if (mapUpdateNodeIdRequest.ContainsKey(instance))
                        mapUpdateNodeIdRequest.Remove(instance);
                    mapUpdateNodeIdRequest.Add(instance, node);

                    if (bReady)
                        PromoteIdleExecution(100);
                }
            }
        }

        public void SetInUse(String item, String instance, bool bInUse, IEntityReference subscriber = null)
        {
            Dictionary<String, List<IEntityReference>> pending = bInUse ? pendingInUseItems : pendingNotInUseItems;

            lastTimeUsed = DateTime.Now;

            lock (lockObject)
            {
                List<IEntityReference> List;
                string itemInstanceString = GetItemInstanceString(item, instance);
                if (!pending.TryGetValue(itemInstanceString, out List))
                {
                    List = new List<IEntityReference>();
                    pending.Add(itemInstanceString, List);
                }
                if (subscriber != null)
                    List.Add(subscriber);

                if (!inUseItemCounter.ContainsKey(itemInstanceString))
                    inUseItemCounter.Add(itemInstanceString, 0);
                if (bInUse)
                    inUseItemCounter[itemInstanceString] += 1;
                else
                    inUseItemCounter[itemInstanceString] -= 1;

#if DEBUGTRACE
                System.Diagnostics.Debug.WriteLine(String.Format("SetInUse for {0}, inUse {1}, subscriber {2}, inUseCounter {3}",
                    itemInstanceString, bInUse, subscriber?.GetType(), inUseItemCounter[itemInstanceString]));
#endif
            }

            if (bReady)
                PromoteIdleExecution(100);
        }

        private void CheckDiscoverNodeIds(List<String> listNodeToMap)
        {
            if (listNodeToMap == null || listNodeToMap.Count == 0 || bDisposed)
                return;

            var mapItemToMapNodes = new Dictionary<String, String>();
            var mapDiscoveryNodes = new Dictionary<NodeId, List<String>>();
            listNodeToMap.ForEach(item =>
                {
                    var idInstance = GetItemInstanceId(item);
                    if (idInstance != null)
                    {
                        if (!mapDiscoveryNodes.ContainsKey(idInstance))
                            mapDiscoveryNodes.Add(idInstance, new List<String>());
                        mapDiscoveryNodes[idInstance].Add(GetItemRelativePath(item));

                        var i = GetItemInstanceString(GetItemRelativePath(item), 
                                                                idInstance.ToString());
                        if (!mapItemToMapNodes.ContainsKey(i))
                            mapItemToMapNodes.Add(i, item);
                    }
                });

            var list = new List<String>();
            var map = new Dictionary<String, NodeId>();
            var mapModels = new Dictionary<String, NodeIdViewModel>();
            foreach (var v in mapDiscoveryNodes.Keys)
            {
                String[] mapDiscoveryNodesToArray = mapDiscoveryNodes[v].ToArray();
                var var = sessionViewModel.GetNodeIds(v, NamespacesUris, mapDiscoveryNodesToArray);

                lock (lockObject)
                {
                    foreach(var item in mapDiscoveryNodesToArray)
                    {
                        String iteminstance = GetItemInstanceString(item, v.ToString());
                        String realItemInstance = mapItemToMapNodes[iteminstance];
                        if (var.ContainsKey(item))
                        {
                            if (!NodeMapped.ContainsKey(realItemInstance))
                            {
                                var model = new NodeIdViewModel(var[item], sessionViewModel);
                                // if (model.NodeAttributes.Count > 0) // reads attributes nd checks if they are valid
                                {
                                    NodeMapped.Add(realItemInstance, model);
                                    map.Add(realItemInstance, (NodeId)model.nodeId);
                                    mapModels.Add(realItemInstance, model);
                                }

                                if (itemsBlackList.Contains(realItemInstance))
                                    itemsBlackList.Remove(realItemInstance);

                                //else
                                //{
                                //    if (!itemsBlackList.Contains(realItemInstance))
                                //    {
                                //        itemsBlackList.Add(realItemInstance);
                                //        list.Add(realItemInstance);
                                //    }
                                //}
                            }
                        }
                        else
                        {
                            if (!itemsBlackList.Contains(realItemInstance))
                            {
                                itemsBlackList.Add(realItemInstance);
                                list.Add(realItemInstance);
                            }
                        }
                    }
                }
            }

            if (map.Count > 0)
            {
                OnNodeIdDiscovered(new NodeIdDiscoveredEventArgs(map, mapModels, false));
            }

            if (list.Count > 0)
                OnBlackListChanged(new BlackListChangedEventArgs(list, false));
        }

        int GetInUseCounter(NodeId node)
        {
            lock (lockObject)
            {
                if (inUseNodeIdCounter.ContainsKey(node))
                    return inUseNodeIdCounter[node];

                var items = (from c in NodeMapped/*.AsParallel()*/ where c.Value.nodeId == node select c.Key).ToList();
                if (items.Count > 0)
                {
                    if (inUseItemCounter.ContainsKey(items[0]))
                        return inUseItemCounter[items[0]];
                }
            }

            return 1;
        }

        void ElaborateInUseRequests(Dictionary<String, List<IEntityReference>> tempInUse, bool bAddReferences)
        {
            if (bDisposed || tempInUse.Count == 0)
                return;

            var settings = GetSession(sessionName);
            int fastSamplingInterval = settings.FastSamplingInterval;
            int slowSamplingInterval = settings.SlowSamplingInterval;
            var mapSettings = settings.MapAppNameSettings;
            if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
            {
                var parent = GetSession(settings.ParentTitle);
                if (parent != null)
                    mapSettings = parent.MapAppNameSettings;
            }
            var appNameSettings = (from c in mapSettings where c.Key == AppName select c.Value).ToList();
            if (appNameSettings.Count > 0)
            {
                fastSamplingInterval = appNameSettings[0].FastSamplingInterval;
                slowSamplingInterval = appNameSettings[0].SlowSamplingInterval;
            }
            if (fastSamplingInterval == slowSamplingInterval)
                slowSamplingInterval++;

            var listToAddVariables = new NodeIdCollection();
            // var listToAddEvent = new NodeIdCollection();
            var mapEntities = new Dictionary<NodeId, List<IEntityReference>>();
            var addMap = new Dictionary<NodeId, MonitoredItemViewModel>();
            var listChangedMonitoredItems = new List<MonitoredItem>();

            lock (lockObject)
            {
                if (sessionViewModel == null)
                    return;

                foreach (var v in tempInUse.Keys)
                {
                    if (!(NodeMapped.ContainsKey(v)))
                    {
                        var model = new NodeIdViewModel(v, sessionViewModel);
                        // if (model.NodeAttributes.Count > 0) // reads attributes and checks if they are valid
                        NodeMapped.Add(v, model);
                    }

                    if (!(NodeMapped.ContainsKey(v) && (NodeMapped[v].IsVariable || NodeMapped[v].IsEventNotifier)))
                        continue;

                    if (bAddReferences && inUseItemCounter[v] > 0 ||
                        !bAddReferences && inUseItemCounter[v] <= 0)
                    {
                        // if (NodeMapped[v].IsVariable)
                        //if (NodeMapped[v].IsEventNotifier)
                        //    listToAddEvent.Add(NodeMapped[v].nodeId);
                        if (mapEntities.ContainsKey((NodeId)NodeMapped[v].nodeId))
                            mapEntities[(NodeId)NodeMapped[v].nodeId].AddRange(tempInUse[v]);
                        else
                        {
                            mapEntities.Add((NodeId)NodeMapped[v].nodeId, tempInUse[v]);
                            listToAddVariables.Add((NodeId)NodeMapped[v].nodeId);
                        }
                    }
                    else
                    {
                        if (mapEntities.ContainsKey((NodeId)NodeMapped[v].nodeId))
                            mapEntities[(NodeId)NodeMapped[v].nodeId].AddRange(tempInUse[v]);
                        else
                        {
                            mapEntities.Add((NodeId)NodeMapped[v].nodeId, tempInUse[v]);
                        }
                    }
                }
            }

            //if (listToAddEvent.Count > 0)
            //{
            //    var listCreateMonItem = new NodeIdCollection();
            //    listToAddEvent.ForEach(nodeid =>
            //        {
            //            if (!MonitoredItemMapped.ContainsKey(nodeid))
            //                listCreateMonItem.Add(nodeid);
            //            else
            //            {
            //                MonitoredItemMapped[nodeid].MonitoringMode = bAddReferences ? MonitoringMode.Reporting : MonitoringMode.Disabled;
            //                MonitoredItemMapped[nodeid].SamplingInterval = bAddReferences ? FastSamplingInterval : SlowSamplingInterval;
            //            }
            //        });


            //    if (listCreateMonItem.Count > 0)
            //    {
            //        var map = SubscriptionViewModel.AddAllEventMonitoredItem(listCreateMonItem);
            //        lock (lockObject)
            //        {
            //            foreach (var v in map.Keys)
            //            {
            //                if (bAddReferences)
            //                    map[v].AddEntityReferences(mapEntities[v]);
            //                else
            //                    map[v].RemoveEntityReferences(mapEntities[v]);
            //                addMap.Add(v, map[v]);
            //                MonitoredItemMapped.Add(v, map[v]);
            //                MonitoredItemMapped[v].MonitoringMode = bAddReferences ? MonitoringMode.Reporting : MonitoringMode.Disabled;
            //                MonitoredItemMapped[v].SamplingInterval = bAddReferences ? FastSamplingInterval : SlowSamplingInterval;
            //            }
            //        }
            //    }
            //}

            if (listToAddVariables.Count > 0)
            {
                var listCreateMonItem = new NodeIdCollection();
                listToAddVariables.ForEach(nodeid =>
                {
                    lock (lockObject)
                    {
                        if (!MonitoredItemMapped.ContainsKey(nodeid))
                        {
                            if (bAddReferences)
                                listCreateMonItem.Add(nodeid);
                        }
                        else
                        {
                            var inuse = GetInUseCounter(nodeid) > 0;
                            
                            if (settings.DisableWhenNotUsed)
                                MonitoredItemMapped[nodeid].MonitoringMode = inuse ? MonitoringMode.Reporting : MonitoringMode.Disabled;
                            else
                                MonitoredItemMapped[nodeid].MonitoringMode = MonitoringMode.Reporting;
                            
                            listChangedMonitoredItems.Add(MonitoredItemMapped[nodeid].monitoredItem);
                            MonitoredItemMapped[nodeid].SamplingInterval = inuse ?
                                fastSamplingInterval : slowSamplingInterval;
                        }
                    }
                });

                if (listCreateMonItem.Count > 0)
                {
                    var monitems = SubscriptionViewModel.AddMonitoredItem(listCreateMonItem);

                    lock (lockObject)
                    {
                        if (sessionViewModel == null)
                            return;

                        bool logged = false;
                        foreach (var v in monitems.Keys)
                        {
                            addMap.Add(v, monitems[v]);
                            MonitoredItemMapped.Add(v, monitems[v]);

//#if !DEBUG
//                            currentConcurrentItems += monitems[v].ArrayDimension;
//#endif

                            var inuse = GetInUseCounter(v) > 0;

                            listChangedMonitoredItems.Add(MonitoredItemMapped[v].monitoredItem);
                            MonitoredItemMapped[v].SamplingInterval = inuse ? fastSamplingInterval : slowSamplingInterval;
//#if !DEBUG && !WINDOWS_UWP
//                            if (currentConcurrentItems > maxConcurrentItems)
//                            {
//                                MonitoredItemMapped[v].MonitoringMode = MonitoringMode.Disabled;
//                                MonitoredItemMapped[v].LastMessage = String.Format(Properties.Resource.LicenseClientExceeded, maxConcurrentItems);
//                                MonitoredItemMapped[v].Quality = StatusCodes.GetBrowseName(StatusCodes.BadTooManySubscriptions);
//                                if(!logged)
//                                    logLicense.Warn(String.Format(Properties.Resource.LicenseClientExceeded, maxConcurrentItems));
//                                logged = true;
//                            }
//#endif
                        }
                    }
                }
            }

            var tempMonitoredItemMapped = new Dictionary<NodeId, MonitoredItemViewModel>();
            lock (lockObject)
            {
                if (sessionViewModel == null)
                    return;

                foreach (var key in MonitoredItemMapped.Keys)
                    tempMonitoredItemMapped.Add(key, MonitoredItemMapped[key]);
            }

            foreach (var v in mapEntities.Keys)
            {
                if (!tempMonitoredItemMapped.ContainsKey(v))
                    continue;

                if (bAddReferences)
                    tempMonitoredItemMapped[v].AddEntityReferences(mapEntities[v]);
                else
                    tempMonitoredItemMapped[v].RemoveEntityReferences(mapEntities[v]);
            }

            //if (listChangedMonitoredItems.Count > 0)
            //    SubscriptionViewModel.SetMonitoringMode(bAddReferences ? MonitoringMode.Reporting : MonitoringMode.Disabled,
            //                                                                        listChangedMonitoredItems);

            CleanDisabledMonitoredItems();
            if (listToAddVariables.Count > 0 || listChangedMonitoredItems.Count > 0)
                SubscriptionViewModel.ApplyChanges();

            if (addMap.Count > 0)
            {
#if DEBUGTRACE
                foreach (var key in addMap.Keys)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Notifing monitored item create {0}, {1}",
                        key, addMap[key].Title));
                }
#endif

                OnMonitoringItemChanged(new MonitoreItemListChangedEventArgs(addMap));
            }
        }

        DateTime lastTimeCheckedDisabledMonitoredItem;
        void CleanDisabledMonitoredItems(bool bForce = false)
        {
            var settings = GetSession(sessionName);
            int fastSamplingInterval = settings.FastSamplingInterval;
            int slowSamplingInterval = settings.SlowSamplingInterval;
            int removeDisabledItemAfterSecs = settings.RemoveDisabledItemAfterSecs;
            int maxCleanCount = settings.MaxCleanCount;
            var mapSettings = settings.MapAppNameSettings;
            if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
            {
                var parent = GetSession(settings.ParentTitle);
                if (parent != null)
                    mapSettings = parent.MapAppNameSettings;
            }
            var appNameSettings = (from c in mapSettings where c.Key == AppName select c.Value).ToList();
            if (appNameSettings.Count > 0)
            {
                fastSamplingInterval = appNameSettings[0].FastSamplingInterval;
                slowSamplingInterval = appNameSettings[0].SlowSamplingInterval;
                removeDisabledItemAfterSecs = appNameSettings[0].RemoveDisabledItemAfterSecs;
                maxCleanCount = appNameSettings[0].MaxCleanCount;
                if (appNameSettings[0].RemoveDisabledItemAfterSecs <= 0 || settings.ConnectItemsAtStartup)
                    return;
            }
            else
            {
                if (settings.RemoveDisabledItemAfterSecs <= 0 || settings.ConnectItemsAtStartup)
                    return;
            }
            if (fastSamplingInterval == slowSamplingInterval)
                slowSamplingInterval++;

            var now = DateTime.UtcNow;
            if (!bForce && lastTimeCheckedDisabledMonitoredItem != null && lastTimeCheckedDisabledMonitoredItem.AddSeconds(5) > now)
                return;
            lastTimeCheckedDisabledMonitoredItem = now;

            if (bForce)
                maxCleanCount = Int32.MaxValue;

            var list = new List<MonitoredItemViewModel>();
            lock (lockObject)
            {
                var listDisabled = (from c in MonitoredItemMapped.Values// .AsParallel()
                                    where c.SamplingInterval == slowSamplingInterval && !pendingInUseNodeIds.ContainsKey((NodeId)c.NodeIdModel.nodeId) &&
                                    (bForce || c.MonitoredItemChangedTime.AddSeconds(removeDisabledItemAfterSecs) < now)
                                    select c).Take(maxCleanCount).ToList();
                
                foreach (var item in listDisabled)
                {
                    var node = MonitoredItemMapped.SingleOrDefault(x => x.Value == item).Key;
//#if !DEBUG
//                    currentConcurrentItems -= MonitoredItemMapped[node].ArrayDimension;
//#endif
                    MonitoredItemMapped.Remove(node);

#if DEBUGTRACE
                    System.Diagnostics.Debug.WriteLine(String.Format("Removing Disabled Item {0}", item.NodeIdModel.nodeId));
#endif
                    list.Add(item);
                }
            }

            lock (lockObject)
            {
                var listDisabled2 = (from c in MonitoredItemMapped.Values// .AsParallel()
                                     where c.MonitoringMode == MonitoringMode.Disabled && !pendingInUseNodeIds.ContainsKey((NodeId)c.NodeIdModel.nodeId) &&
                                     c.MonitoredItemChangedTime.AddSeconds(removeDisabledItemAfterSecs) < now
                                     select c)/*.Take(maxCleanCount)*/.ToList();

                foreach (var item in listDisabled2)
                {
                    var node = MonitoredItemMapped.SingleOrDefault(x => x.Value == item).Key;
//#if !DEBUG
//                    currentConcurrentItems -= MonitoredItemMapped[node].ArrayDimension;
//#endif
                    MonitoredItemMapped.Remove(node);

#if DEBUGTRACE
                    System.Diagnostics.Debug.WriteLine(String.Format("Removing Disabled Item {0}", item.NodeIdModel.nodeId));
#endif
                    list.Add(item);
                }
            }

            if (list.Count > 0)
            {
                list.ForEach(item =>
                    {
                        item.Dispose();
                        // SubscriptionViewModel.RemoveMonitoredItem(item);
                    });

                SubscriptionViewModel.ApplyChanges();
            }
        }

        void ElaborateInUseRequests(Dictionary<NodeId, List<IEntityReference>> tempInUse, bool bAddReferences)
        {
            if (bDisposed || tempInUse.Count == 0)
                return;

            var settings = GetSession(sessionName);
            int fastSamplingInterval = settings.FastSamplingInterval;
            int slowSamplingInterval = settings.SlowSamplingInterval;
            var mapSettings = settings.MapAppNameSettings;
            if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
            {
                var parent = GetSession(settings.ParentTitle);
                if (parent != null)
                    mapSettings = parent.MapAppNameSettings;
            }
            var appNameSettings = (from c in mapSettings where c.Key == AppName select c.Value).ToList();
            if (appNameSettings.Count > 0)
            {
                fastSamplingInterval = appNameSettings[0].FastSamplingInterval;
                slowSamplingInterval = appNameSettings[0].SlowSamplingInterval;
            }
            if (fastSamplingInterval == slowSamplingInterval)
                slowSamplingInterval++;

            var listToAddVariables = new NodeIdCollection();
            // var listToAddEvent = new NodeIdCollection();
            var mapEntities = new Dictionary<NodeId, List<IEntityReference>>();
            var addMap = new Dictionary<NodeId, MonitoredItemViewModel>();
            var listChangedMonitoredItems = new List<MonitoredItem>();

            lock (lockObject)
            {
                if (sessionViewModel == null)
                    return;

                foreach (var v in tempInUse.Keys)
                {
                    if (!(NodeMappedNodeIds.ContainsKey(v)))
                    {
                        var model = new NodeIdViewModel(v, sessionViewModel);
                        // if (model.NodeAttributes.Count > 0) // reads attributes and checks if they are valid
                            NodeMappedNodeIds.Add(v, model);
                    }

                    if (!(NodeMappedNodeIds.ContainsKey(v) && (NodeMappedNodeIds[v].IsVariable || NodeMappedNodeIds[v].IsEventNotifier)))
                        continue;

                    if (bAddReferences && inUseNodeIdCounter[v] > 0 ||
                        !bAddReferences && inUseNodeIdCounter[v] <= 0)
                    {
                        // if (NodeMapped[v].IsVariable)
                        //if (NodeMapped[v].IsEventNotifier)
                        //    listToAddEvent.Add(NodeMapped[v].nodeId);
                        if (mapEntities.ContainsKey((NodeId)NodeMappedNodeIds[v].nodeId))
                            mapEntities[(NodeId)NodeMappedNodeIds[v].nodeId].AddRange(tempInUse[v]);
                        else
                        {
                            mapEntities.Add((NodeId)NodeMappedNodeIds[v].nodeId, tempInUse[v]);
                            listToAddVariables.Add((NodeId)NodeMappedNodeIds[v].nodeId);
                        }
                    }
                    else
                    {
                        if (mapEntities.ContainsKey((NodeId)NodeMappedNodeIds[v].nodeId))
                            mapEntities[(NodeId)NodeMappedNodeIds[v].nodeId].AddRange(tempInUse[v]);
                        else
                        {
                            mapEntities.Add((NodeId)NodeMappedNodeIds[v].nodeId, tempInUse[v]);
                        }
                    }
                }
            }

            //if (listToAddEvent.Count > 0)
            //{
            //    var listCreateMonItem = new NodeIdCollection();
            //    listToAddEvent.ForEach(nodeid =>
            //        {
            //            if (!MonitoredItemMapped.ContainsKey(nodeid))
            //                listCreateMonItem.Add(nodeid);
            //            else
            //            {
            //                MonitoredItemMapped[nodeid].MonitoringMode = bAddReferences ? MonitoringMode.Reporting : MonitoringMode.Disabled;
            //                MonitoredItemMapped[nodeid].SamplingInterval = bAddReferences ? FastSamplingInterval : SlowSamplingInterval;
            //            }
            //        });


            //    if (listCreateMonItem.Count > 0)
            //    {
            //        var map = SubscriptionViewModel.AddAllEventMonitoredItem(listCreateMonItem);
            //        lock (lockObject)
            //        {
            //            foreach (var v in map.Keys)
            //            {
            //                if (bAddReferences)
            //                    map[v].AddEntityReferences(mapEntities[v]);
            //                else
            //                    map[v].RemoveEntityReferences(mapEntities[v]);
            //                addMap.Add(v, map[v]);
            //                MonitoredItemMapped.Add(v, map[v]);
            //                MonitoredItemMapped[v].MonitoringMode = bAddReferences ? MonitoringMode.Reporting : MonitoringMode.Disabled;
            //                MonitoredItemMapped[v].SamplingInterval = bAddReferences ? FastSamplingInterval : SlowSamplingInterval;
            //            }
            //        }
            //    }
            //}

            if (listToAddVariables.Count > 0)
            {
                var listCreateMonItem = new NodeIdCollection();
                listToAddVariables.ForEach(nodeid =>
                {
                    lock (lockObject)
                    {
                        if (!MonitoredItemMapped.ContainsKey(nodeid))
                        {
                            if (bAddReferences)
                                listCreateMonItem.Add(nodeid);
                        }
                        else
                        {
                            var inuse = GetInUseCounter(nodeid) > 0;

                            if (settings.DisableWhenNotUsed)
                                MonitoredItemMapped[nodeid].MonitoringMode = inuse ? MonitoringMode.Reporting : MonitoringMode.Disabled;
                            else
                                MonitoredItemMapped[nodeid].MonitoringMode = MonitoringMode.Reporting;
                            
                            listChangedMonitoredItems.Add(MonitoredItemMapped[nodeid].monitoredItem);
                            MonitoredItemMapped[nodeid].SamplingInterval = inuse ?
                                fastSamplingInterval : slowSamplingInterval;
                        }
                    }
                });

                if (listCreateMonItem.Count > 0)
                {
                    var monitems = SubscriptionViewModel.AddMonitoredItem(listCreateMonItem);

                    lock (lockObject)
                    {
                        if (sessionViewModel == null)
                            return;

                        bool logged = false;
                        foreach (var v in monitems.Keys)
                        {
                            addMap.Add(v, monitems[v]);
                            MonitoredItemMapped.Add(v, monitems[v]);

//#if !DEBUG
//                            currentConcurrentItems += monitems[v].ArrayDimension;
//#endif

                            var inuse = GetInUseCounter(v) > 0;

                            listChangedMonitoredItems.Add(MonitoredItemMapped[v].monitoredItem);
                            MonitoredItemMapped[v].SamplingInterval = inuse ? fastSamplingInterval : slowSamplingInterval;
//#if !DEBUG && !WINDOWS_UWP
//                            if (currentConcurrentItems > maxConcurrentItems)
//                            {
//                                MonitoredItemMapped[v].MonitoringMode = MonitoringMode.Disabled;
//                                MonitoredItemMapped[v].LastMessage = String.Format(Properties.Resource.LicenseClientExceeded, maxConcurrentItems);
//                                MonitoredItemMapped[v].Quality = StatusCodes.GetBrowseName(StatusCodes.BadTooManySubscriptions);
//                                if(!logged)
//                                    logLicense.Warn(String.Format(Properties.Resource.LicenseClientExceeded, maxConcurrentItems));
//                                logged = true;
//                            }
//#endif
                        }
                    }
                }
            }

            var tempMonitoredItemMapped = new Dictionary<NodeId, MonitoredItemViewModel>();
            lock (lockObject)
            {
                if (sessionViewModel == null)
                    return;

                foreach (var key in MonitoredItemMapped.Keys)
                    tempMonitoredItemMapped.Add(key, MonitoredItemMapped[key]);
            }

            foreach (var v in mapEntities.Keys)
            {
                if (!tempMonitoredItemMapped.ContainsKey(v))
                    continue;

                if (bAddReferences)
                    tempMonitoredItemMapped[v].AddEntityReferences(mapEntities[v]);
                else
                    tempMonitoredItemMapped[v].RemoveEntityReferences(mapEntities[v]);
            }

            //if (listChangedMonitoredItems.Count > 0)
            //    SubscriptionViewModel.SetMonitoringMode(bAddReferences ? MonitoringMode.Reporting : MonitoringMode.Disabled,
            //                                                                        listChangedMonitoredItems);

            CleanDisabledMonitoredItems();
            if (listToAddVariables.Count > 0 || listChangedMonitoredItems.Count > 0)
                SubscriptionViewModel.ApplyChanges();

            if (addMap.Count > 0)
            {
#if DEBUGTRACE
                foreach (var key in addMap.Keys)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Notifing monitored item create {0}, {1}",
                        key, addMap[key].Title));
                }
#endif

                OnMonitoringItemChanged(new MonitoreItemListChangedEventArgs(addMap));
            }
        }

#if WINDOWS_UWP
        async
#endif
        void OnMonitoringItemChanged(MonitoreItemListChangedEventArgs e)
        {
            EventHandler<MonitoreItemListChangedEventArgs> handler = MonitoringItemListchanged;
            if (handler != null)
            {
#if !WINDOWS_UWP && !NET_STANDARD
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
#else
#if !NET_STANDARD
                var dispobj = handler.Target as DependencyObject;
                if (dispobj != null)
                {
                    Utilities.RunOnUIThread.RunIfRequired(() => handler(this, e));
                }
                else
#endif
                    handler(this, e);
#endif
            }
        }

#if WINDOWS_UWP
        async
#endif
        void OnNodeIdDiscovered(NodeIdDiscoveredEventArgs e)
        {
            EventHandler<NodeIdDiscoveredEventArgs> handler = NodeIdDiscovered;
            if (handler != null)
            {
#if !WINDOWS_UWP && !NET_STANDARD
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
#else
#if !NET_STANDARD
                var dispobj = handler.Target as DependencyObject;
                if (dispobj != null)
                {
                    Utilities.RunOnUIThread.RunIfRequired(() => handler(this, e));
                }
                else
#endif
                    handler(this, e);
#endif
            }
        }

#if WINDOWS_UWP
        async
#endif
        void OnBlackListChanged(BlackListChangedEventArgs e)
        {
            EventHandler<BlackListChangedEventArgs> handler = BlackListChanged;
            if (handler != null)
            {
#if !WINDOWS_UWP && !NET_STANDARD
                // If the subscriber is a DispatcherObject and different thread
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
#else
#if !NET_STANDARD
                var dispobj = handler.Target as DependencyObject;
                if (dispobj != null)
                {
                    Utilities.RunOnUIThread.RunIfRequired(() => handler(this, e));
                }
                else
#endif
                    handler(this, e);
#endif
            }
        }

        void ElaborateInUseRequests(bool bConnected = true)
        {
            var settings = GetSession(sessionName);
            int publishingInterval = settings.PublishingInterval;
            var mapSettings = settings.MapAppNameSettings;
            if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
            {
                var parent = GetSession(settings.ParentTitle);
                if (parent != null)
                    mapSettings = parent.MapAppNameSettings;
            }
            var appNameSettings = (from c in mapSettings where c.Key == AppName select c.Value).ToList();
            if (appNameSettings.Count > 0)
            {
                publishingInterval = appNameSettings[0].PublishingInterval;
            }

            if (bConnected)
            {
                try
                {
                    if (sessionViewModel != null && SubscriptionViewModel != null && sessionViewModel.Connected &&
                        SubscriptionViewModel.subscription != null &&
                        SubscriptionViewModel.PublishingInterval != publishingInterval)
                    {
                        SubscriptionViewModel.PublishingInterval = publishingInterval;
                        SubscriptionViewModel.subscription.Modify();
                        SubscriptionViewModel.subscription.ApplyChanges();
                    }
                }
                catch (Exception ex)
                {
                    
                }

                var tempMap = new Dictionary<String, NodeId>();
                lock (lockObject)
                {
                    foreach (var key in mapUpdateNodeIdRequest.Keys)
                        tempMap.Add(key, mapUpdateNodeIdRequest[key]);
                   mapUpdateNodeIdRequest.Clear();
                }

                if (tempMap.Count > 0)
                    UpdateNodeMap(tempMap);
            }

            Dictionary<String, List<IEntityReference>> tempInUse;
            Dictionary<String, List<IEntityReference>> tempNotInUse;
            lock (lockObject)
            {
                tempInUse = new Dictionary<String, List<IEntityReference>>(pendingInUseItems);
                if (bConnected)
                    pendingInUseItems.Clear();
                tempNotInUse = new Dictionary<String, List<IEntityReference>>(pendingNotInUseItems);
                pendingNotInUseItems.Clear();
            }

            List<IEntityReference> tempListInUse;
            List<IEntityReference> tempListNotInUse;
            foreach (var v in tempNotInUse.Keys)
            {
                if (!tempInUse.TryGetValue(v, out tempListInUse))
                    continue;

                tempListNotInUse = tempNotInUse[v];
                tempListNotInUse.ForEach(entity =>
                {
                    if (tempListInUse.Contains(entity))
                    {
                        tempListInUse.Remove(entity);
                        // tempListNotInUse.Remove(entity);
                    }
                });
            }

            if (bConnected)
            {
                List<String> nodeIdToDiscover = null;
                lock (lockObject)
                {
                    if (listNodeIdToDiscover.Count > 0)
                    {
                        nodeIdToDiscover = new List<String>();
                        nodeIdToDiscover.AddRange(listNodeIdToDiscover);
                        listNodeIdToDiscover.Clear();
                    }
                }

                CheckDiscoverNodeIds(nodeIdToDiscover);

                ElaborateInUseRequests(tempInUse, true);
            }
            ElaborateInUseRequests(tempNotInUse, false);


            Dictionary<NodeId, List<IEntityReference>> tempInUseNodeIds;
            Dictionary<NodeId, List<IEntityReference>> tempNotInUseNodeIds;
            lock (lockObject)
            {
                tempInUseNodeIds = new Dictionary<NodeId, List<IEntityReference>>(pendingInUseNodeIds);
                if (bConnected)
                    pendingInUseNodeIds.Clear();
                tempNotInUseNodeIds = new Dictionary<NodeId, List<IEntityReference>>(pendingNotInUseNodeIds);
                pendingNotInUseNodeIds.Clear();
            }

            //List<IEntityReference> tempListInUseNodeIds;
            //List<IEntityReference> tempListNotInUseNodeIds;
            //foreach (var v in tempNotInUseNodeIds.Keys)
            //{
            //    if (!tempInUseNodeIds.TryGetValue(v, out tempListInUseNodeIds))
            //        continue;

            //    tempListNotInUseNodeIds = tempNotInUseNodeIds[v];
            //    tempListNotInUseNodeIds.ForEach(entity =>
            //    {
            //        if (tempListInUseNodeIds.Contains(entity))
            //        {
            //            tempListInUseNodeIds.Remove(entity);
            //            // tempListNotInUseNodeIds.Remove(entity);
            //        }
            //    });
            //}

            if (bConnected)
                ElaborateInUseRequests(tempInUseNodeIds, true);
            if (tempNotInUseNodeIds.Count > 0)
                ElaborateInUseRequests(tempNotInUseNodeIds, false);
        }

#endregion

#region Properties

        public SessionViewModel SessionViewModel
        {
            get 
            {
                return sessionViewModel;
            }
        }

#endregion

#region Persistance

        [DataMember(Name = "HostName")]
        String _hostName;
        public String HostName
        {
            get
            {
                return _hostName;
            }
            set
            {
                if (value == _hostName)
                    return;

                _hostName = value;
                OnPropertyChanged("HostName");
            }
        }

        [DataMember(Name = "AppName")]
        String _appName;
        public String AppName
        {
            get
            {
                return _appName;
            }
            set
            {
                if (value == _appName)
                    return;

                _appName = value;
                OnPropertyChanged("AppName");
            }
        }

        [DataMember(Name = "EndpointUrl")]
        String _endpointUrl;
        public String EndpointUrl
        {
            get
            {
                return _endpointUrl;
            }
            set
            {
                if (value == _endpointUrl)
                    return;

                _endpointUrl = value;
                OnPropertyChanged("EndpointUrl");
            }
        }

        [DataMember(Name = "NamespacesUris")]
        NamespaceTable _namespacesUris;
        public NamespaceTable NamespacesUris
        {
            get
            {
                return _namespacesUris;
            }
            set
            {
                if (value == _namespacesUris)
                    return;

                _namespacesUris = value;
                OnPropertyChanged("NamespacesUris");
            }
        }

        public String SessionName
        {
            get
            {
                return sessionName;
            }
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
#region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                //BitmapImage bm = new BitmapImage();
                //bm.BeginInit();
                //Assembly assembly = Assembly.GetExecutingAssembly();

                //String str = String.Format("pack://application:,,,/{0};component/Images/effects_16x16.png",
                //    Path.GetFileNameWithoutExtension(assembly.Location));
                //bm.UriSource = new Uri(str);
                //bm.EndInit();
                var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, $"OPCUAVMEffects", false);
                return bm;
#else
                return null;
#endif
            }
        }

        public Object Tooltip
        {
            get 
            {
                return null;
            }
        }

        public ImageSource ExpandedImageSource { get { return null; } }
        public ContextMenu contextMenu { get { return null; } }

        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        public object EntityParent
        {
            get
            {
                return Parent;
            }
        }

        public String TypeDefinitionString
        {
            get { return null; }
        }

#endregion

#region shutdown
        bool bShutdownFetched;
        SubscriptionViewModel SubscriptionViewModelShutdown;
        PropertyObserver<MonitoredItemViewModel> observerSecondsTillShutdown;
        void FetchShutdownSupport()
        {
            if (bShutdownFetched || sessionViewModel == null)
                return;
            bShutdownFetched = true;

            SubscriptionViewModelShutdown = sessionViewModel.CreateSubscription(String.Format(Properties.Resource.ShutdownSubscriptionName, HostName, AppName));

            if (SubscriptionViewModelShutdown == null)
                return;

            var settings = GetSession(sessionName);
            var mapSettings = settings.MapAppNameSettings;
            if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
            {
                var parent = GetSession(settings.ParentTitle);
                if (parent != null)
                    mapSettings = parent.MapAppNameSettings;
            }
            var appNameSettings = (from c in mapSettings where c.Key == AppName select c.Value).ToList();
            if (appNameSettings.Count > 0)
            {
                SubscriptionViewModelShutdown.PublishingInterval = appNameSettings[0].PublishingInterval;
            }
            else
                SubscriptionViewModelShutdown.PublishingInterval = settings.PublishingInterval;

            SubscriptionViewModelShutdown.PublishingEnabled = true;

            try
            {
                SubscriptionViewModelShutdown.subscription.Modify();
            }
            catch (Exception ex)
            {
                // sever may not support subscription changing 
                LastMessage = ex.Message;
            }

            var serverSpecialNodes = new NodeIdCollection();
            serverSpecialNodes.Add(Variables.Server_ServerStatus_SecondsTillShutdown);
            serverSpecialNodes.Add(Variables.Server_ServerStatus_State);

            var monitems = SubscriptionViewModelShutdown.AddMonitoredItem(serverSpecialNodes);
            SubscriptionViewModelShutdown.ApplyChanges();

            if (monitems.ContainsKey(Variables.Server_ServerStatus_SecondsTillShutdown))
            {
                observerSecondsTillShutdown = new PropertyObserver<MonitoredItemViewModel>(monitems[Variables.Server_ServerStatus_SecondsTillShutdown])
                    .RegisterHandler(n => n.Value,
                                     n =>
                                     {
                                         try
                                         {
                                             if (monitems.ContainsKey(Variables.Server_ServerStatus_State) && 
                                                 monitems[Variables.Server_ServerStatus_State].DataValue != null &&
                                                     StatusCode.IsGood(monitems[Variables.Server_ServerStatus_State].DataValue.StatusCode))
                                             {
                                                 var state = (ServerState)monitems[Variables.Server_ServerStatus_State].DataValue.Value;
                                                 if (state == ServerState.Shutdown)
                                                 {
                                                     int nValue = Convert.ToInt32(n.Value);
                                                     if (nValue > 0 && nValue <= 5)
                                                     {
                                                         DisposeShutdown();
                                                         SessionViewModel.Connected = false;
                                                     }
                                                 }
                                             }
                                         }
                                         catch
                                         {

                                         }
                                     });
            }
        }

        void DisposeShutdown()
        {
            lock (lockObject)
            {
                if (observerSecondsTillShutdown != null)
                {
                    observerSecondsTillShutdown.Dispose();
                    observerSecondsTillShutdown = null;
                }

                if (sessionViewModel != null && SubscriptionViewModelShutdown != null)
                {
                    sessionViewModel.RemoveSubscription(SubscriptionViewModelShutdown);
                    SubscriptionViewModelShutdown.Dispose();
                    SubscriptionViewModelShutdown = null;
                }
            }

            bShutdownFetched = false;
        }

#endregion

#region Client Redundancy

        PropertyObserver<MonitoredItemViewModel> observerServerServiceLevel;
        RedundancySupport redundancySupported = RedundancySupport.None;
        String[] serverUriArray;
        byte serviceLevel = 255;

        public RedundancySupport RedundancySupported
        {
            get
            {
                return redundancySupported;
            }
        }

        public String[] ServerUriArray
        {
            get
            {
                return serverUriArray;
            }
        }

        public byte ServiceLevel
        {
            get
            {
                return serviceLevel;
            }
        }

        public void SetPublishingEnabled(bool bSet)
        {
            if (SubscriptionViewModel == null || bSet == false || SubscriptionViewModel.PublishingEnabled == bSet)
                return;

            SubscriptionViewModel.PublishingEnabled = bSet;

            try
            {
                SubscriptionViewModel.subscription.Modify();
            }
            catch (Exception ex)
            {
                // sever may not support subscription changing 
                LastMessage = ex.Message;
            }
        }

        bool IsRedundancySupported()
        {
            return RedundancySupported != RedundancySupport.None &&
                ServerUriArray != null && ServerUriArray.Length > 1;
        }

#if !NET_STANDARD
        bool bServerCapabilitiesFetched;
        void FetchServerCapabilities()
        {
            if (bServerCapabilitiesFetched || sessionViewModel == null)
                return;
            bServerCapabilitiesFetched = true;

            var serverSpecialNodes = new NodeIdCollection();
            serverSpecialNodes.Add(Variables.Server_ServerCapabilities_OperationLimits_MaxNodesPerBrowse);
            serverSpecialNodes.Add(Variables.Server_ServerCapabilities_OperationLimits_MaxMonitoredItemsPerCall);
            serverSpecialNodes.Add(Variables.Server_ServerCapabilities_OperationLimits_MaxNodesPerTranslateBrowsePathsToNodeIds);

            var expectedTypes = new List<Type>();
            expectedTypes.Add(typeof(UInt32));
            expectedTypes.Add(typeof(UInt32));
            expectedTypes.Add(typeof(UInt32));

            try
            {
                List<object> values;
                List<ServiceResult> errors;

                sessionViewModel.Session.ReadValues(serverSpecialNodes, expectedTypes, out values, out errors);

                for (int ii = 0; ii < errors.Count; ii++)
                {
                    if (ServiceResult.IsNotGood(errors[ii].StatusCode))
                        throw new ServiceResultException(errors[ii]);
                }

                var maxNodesPerRead = (UInt32)values[0];
                var maxMonitoredItemsPerCall = (UInt32)values[1];
                var maxNodesPerTranslateBrowsePathsToNodeIds = (UInt32)values[2];

                SubscriptionViewModel.SetMaxNodesPerRead(maxNodesPerRead);
                SubscriptionViewModel.SetMaxMonitoredItemsPerCall(maxMonitoredItemsPerCall);
                SubscriptionViewModel.SetMaxNodesPerTranslateBrowsePathsToNodeIds(maxNodesPerTranslateBrowsePathsToNodeIds);
            }
            catch (Exception ex)
            {
                LastMessage = ex.Message;
            }
        }
#endif

        bool bRedundancyFetched;
        SubscriptionViewModel SubscriptionViewModelRedundancy;
        void FetchRedundancySupport()
        {
            if (bRedundancyFetched || sessionViewModel == null)
                return;
            bRedundancyFetched = true;

            var serverSpecialNodes = new NodeIdCollection();
            serverSpecialNodes.Add(Variables.Server_ServerRedundancy_RedundancySupport);
            serverSpecialNodes.Add(Variables.Server_ServerRedundancy_ServerUriArray);
            serverSpecialNodes.Add(Variables.Server_ServiceLevel);

            var expectedTypes = new List<Type>();
            expectedTypes.Add(typeof(Int32));
            expectedTypes.Add(typeof(String[]));
            expectedTypes.Add(typeof(byte));

            try
            {
                List<object> values;
                List<ServiceResult> errors;

                sessionViewModel.Session.ReadValues(serverSpecialNodes, expectedTypes, out values, out errors);

                for (int ii = 0; ii < errors.Count; ii++)
                {
                    if (ServiceResult.IsNotGood(errors[ii].StatusCode))
                        throw new ServiceResultException(errors[ii]);
                }

                redundancySupported = (RedundancySupport)values[0];
                OnPropertyChanged("RedundancySupported");

                serverUriArray = (string[])values[1];
                OnPropertyChanged("ServerUriArray");

                serviceLevel = (byte)values[2];
                OnPropertyChanged("ServiceLevel");

                bAllFetched = bReady = true;
                bFetching = false;
            }
            catch (Exception ex)
            {
                LastMessage = ex.Message;
            }

            if (!IsRedundancySupported())
            {
                bAllFetched = bReady = true;
                bFetching = false;
                redundancySupported = RedundancySupport.None;
                return;
            }

            serverSpecialNodes.Clear();
            serverSpecialNodes.Add(Variables.Server_ServiceLevel);

            SubscriptionViewModelRedundancy = sessionViewModel.CreateSubscription(String.Format(Properties.Resource.RedundancySubscriptionName, HostName, AppName));

            if (SubscriptionViewModelRedundancy == null)
                return;

            var settings = GetSession(sessionName);
            var mapSettings = settings.MapAppNameSettings;
            if (mapSettings.Count == 0 && !String.IsNullOrEmpty(settings.ParentTitle))
            {
                var parent = GetSession(settings.ParentTitle);
                if (parent != null)
                    mapSettings = parent.MapAppNameSettings;
            }
            var appNameSettings = (from c in mapSettings where c.Key == AppName select c.Value).ToList();
            if (appNameSettings.Count > 0)
            {
                SubscriptionViewModelRedundancy.PublishingInterval = appNameSettings[0].PublishingInterval;
            }
            else
                SubscriptionViewModelRedundancy.PublishingInterval = settings.PublishingInterval;

            SubscriptionViewModelRedundancy.PublishingEnabled = true;

            try
            {
                SubscriptionViewModelRedundancy.subscription.Modify();
            }
            catch (Exception ex)
            {
                // sever may not support subscription changing 
                LastMessage = ex.Message;
            }

            var monitems = SubscriptionViewModelRedundancy.AddMonitoredItem(serverSpecialNodes);
            SubscriptionViewModelRedundancy.ApplyChanges();

            if (monitems.ContainsKey(Variables.Server_ServiceLevel))
            {
                observerServerServiceLevel = new PropertyObserver<MonitoredItemViewModel>(monitems[Variables.Server_ServiceLevel])
                    .RegisterHandler(n => n.Value,
                                        n =>
                                        {
                                            try
                                            {
                                                serviceLevel = (byte)n.DataValue.Value;
                                                OnPropertyChanged("ServiceLevel");
                                            }
                                            catch (Exception)
                                            { }
                                        });
            }
        }

        void DisposeRedundancy()
        {
            SubscriptionViewModel subscriptionViewModelRedundancy = null; 
            SessionViewModel tempSessionViewModel = null; 
            lock (lockObject)
            {
                if (observerServerServiceLevel != null)
                {
                    observerServerServiceLevel.Dispose();
                    observerServerServiceLevel = null;
                }

                if (sessionViewModel != null && SubscriptionViewModelRedundancy != null)
                {
                    tempSessionViewModel = sessionViewModel;
                    subscriptionViewModelRedundancy = SubscriptionViewModelRedundancy;
                    SubscriptionViewModelRedundancy = null;
                }
            }

            if (tempSessionViewModel != null && subscriptionViewModelRedundancy != null)
            {
                tempSessionViewModel.RemoveSubscription(subscriptionViewModelRedundancy);
                subscriptionViewModelRedundancy.Dispose();
            }
        }
#endregion
    }
}
