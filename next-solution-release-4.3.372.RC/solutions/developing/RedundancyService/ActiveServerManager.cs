using DriverBaseInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.Logger;
using System.ComponentModel;

namespace RedundancyService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single
#if DEBUG
        , IncludeExceptionDetailInFaults = true
#endif
        )]
    public class ActiveServerManager : Observable, IRedundancyService, IDisposable
    {
        #region Declarations
        String hostName;
        String pollingServer;
        String activatingServer;
        String deactivatingServer;
        int nArrayPosition;
        ServiceHost announcementServiceHost;
        ServiceHost host;
        IRedundancyService clientUdp;
        IRedundancyService clientTcp;

        List<String> listActiveServer = new List<String>();
        List<String> listAliveServer = new List<String>();

        int nPendingKeepAlive;
        Timer keepAliveTimer;
        
        // bool bListActiveServerChanged;
        Timer timer;
        Timer fullSynchronizationTimer;
        Timer delayActivateServerTimer;
        DateTime fullSynchronizationLastExecutionTime;
        bool fullSynchronizationInExecution;
        bool bRecreateChannelOnPingError;
        int nPendingPings;
        int nActivatingServers;
        int nDeactivatingServers;
        int nSwitchingActiveServer;
        DateTime startTimeActivation = DateTime.MinValue;
        bool bIsActiveServer;
        bool bIsStarted;
        bool bIsStarting;
        bool bWasActive;
        bool bIsNetworkAvailable;
        bool bWasNetworkAvailable;
        bool discoveringServers; //set true when DiscoverActiveServer is in execution
        bool forceRecreateChannelOnPingError; //bool used to force to recreate the channel despite the exceptions

        Object lockudpObject = new Object();
        Object locktcpObject = new Object();
        Object lockObject = new Object();
        #endregion

        #region ctors
        public ActiveServerManager()
        {
            InitHostNames();
        }
        #endregion

        #region Methods
        void InitHostNames()
        {
            hostName = Dns.GetHostName().ToLower();
            //hostIps = (from c in Dns.GetHostEntry(hostName).AddressList 
            //           where c.AddressFamily == AddressFamily.InterNetwork 
            //           select c.ToString()).ToList();
        }
        
        void Start()
        {
            try
            {
                if (ArrayServers == null || ArrayServers.Length < 2)
                {
                    OnServerActive();
                    OnActiveServerChanged(hostName);
                    OnAliveServerListChanged();
                    return;
                }
                else if (KeepActiveServerActive && ArrayServers.Length > 2)
                {
                    KeepActiveServerActive = false;
                    OnErrorOccuredTags(new InvalidOperationException(Properties.Resources.InvalidKeepActiveServerActiveOption));
                }

                if (!IsValidServer(hostName))
                    throw new InvalidOperationException(Properties.Resources.RedundancyStartupErrorForMissingServer);

                if (bIsStarted || bDisposed)
                    return;
                bStopping = false;
                bIsStarted = true;
                bIsStarting = true;
                // bListActiveServerChanged = true;

                OnServerStateChanging(RedundancyServerState.Starting);

                RedundancyService.Helper.NetworkStatus.AvailabilityChanged += (s, e) =>
                {
                    try
                    {
                        DoNetworkAvailabilityChanged(e.IsAvailable);
                    }
                    catch (Exception ex)
                    {
                        OnErrorOccuredTags(ex);
                    }
                };

                bIsNetworkAvailable = RedundancyService.Helper.NetworkStatus.IsAvailable;
                DoNetworkAvailabilityChanged(bIsNetworkAvailable);

                if (bIsStarted && bIsNetworkAvailable)
                {
                    DiscoverActiveServer();
                    ElaborateActiveServer();
                }

                //StartFindDeadServerManager();
            }
            catch(Exception ex)
            {
                OnErrorOccuredTags(ex);
            }
            finally
            {
                bIsStarting = false;            
            }
        }

        void DoNetworkAvailabilityChanged(bool isAvailable)
        {
            if (!bIsStarted || bStopping)
                return;

            bWasNetworkAvailable = bIsNetworkAvailable;
            bIsNetworkAvailable = isAvailable;

            DiscoverActiveServer();
            ElaborateActiveServer();

            if (isAvailable)
            {
                OpenServiceHosts();
                AnnounceOnline();
            }
            else
            {
                CleanTimer();

                CleanClientUdp();
                CleanClientTcp();

                CloseServiceHosts();
            }
        }

        void AnnounceOnline()
        {
            if (!bIsStarted || bStopping)
                return;

            using (var announcementClient = new AnnouncementClient(new UdpAnnouncementEndpoint()))
            {
                foreach (System.ServiceModel.Description.ServiceEndpoint endpoint in host.Description.Endpoints)
                {
                    EndpointDiscoveryMetadata endpointDiscoveryMetadata = EndpointDiscoveryMetadata.FromServiceEndpoint(endpoint);
                    announcementClient.AnnounceOnline(endpointDiscoveryMetadata);
                }
            }
        }

        void OpenServiceHosts()
        {
            if (!bIsStarted || bStopping)
                return;

            lock (lockObject)
            {
                if (host == null)
                {
                    host = new ServiceHost(this);
                    host.AddServiceEndpoint(typeof(IRedundancyService), new BinaryUdpBinding(), String.Format(UdpBaseAddress, UdpServiceIpAddress, PortNumber));
                    host.AddServiceEndpoint(typeof(IRedundancyService), new BinaryNetTcpBinding(NetTcpSecurityMode), String.Format(NetTcpBaseAddress, hostName, PortNumber));

                    #region Discovery Settings

                    var discoveryBehavior = new ServiceDiscoveryBehavior();
                    host.Description.Behaviors.Add(discoveryBehavior);
                    host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                    discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());

                    #endregion

                    host.Open();
                }

                if (announcementServiceHost == null)
                {
                    // Create an AnnouncementService instance
                    var announcementService = new AnnouncementService();

                    // Subscribe the announcement events
                    announcementService.OnlineAnnouncementReceived += (o, e) =>
                    {
                        if (bStopping)
                            return;

                    // Console.WriteLine("Received an online announcement from {0}", e.EndpointDiscoveryMetadata.Address);
                    for (int i = 0; i < e.EndpointDiscoveryMetadata.ContractTypeNames.Count; ++i)
                        {
                            if (e.EndpointDiscoveryMetadata.ContractTypeNames[i].Name == typeof(IRedundancyService).Name)
                            {
                                var server = GetHostAddress(e.EndpointDiscoveryMetadata);
                                var port = GetPortAddress(e.EndpointDiscoveryMetadata);
                                if (IsValidServer(server) && server.ToLower() != hostName && port == PortNumber)
                                {
                                    AddServerToActiveList(server);
                                    ElaborateActiveServer();
                                }
                                break;
                            }
                        }
                    };

                    announcementService.OfflineAnnouncementReceived += (o, e) =>
                    {
                        if (bStopping)
                            return;

                    // Console.WriteLine("Received an offline announcement from {0}", e.EndpointDiscoveryMetadata.Address);
                    for (int i = 0; i < e.EndpointDiscoveryMetadata.ContractTypeNames.Count; ++i)
                        {
                            if (e.EndpointDiscoveryMetadata.ContractTypeNames[i].Name == typeof(IRedundancyService).Name)
                            {
                                var server = GetHostAddress(e.EndpointDiscoveryMetadata);
                                var port = GetPortAddress(e.EndpointDiscoveryMetadata);
                                if (IsValidServer(server) && server.ToLower() != hostName && port == PortNumber)
                                {
                                    RemoveServerFromActiveList(server);
                                    ElaborateActiveServer();
                                }
                                break;
                            }
                        }
                    };

                    // Create ServiceHost for the AnnouncementService
                    announcementServiceHost = new ServiceHost(announcementService);
                    // Listen for the announcements sent over UDP multicast
                    announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());
                    announcementServiceHost.Open();
                }
            }
        }

        void CloseServiceHosts()
        {
            lock (lockObject)
            {
                if (host != null)
                {
                    host.Close();
                    host = null;
                }

                if (announcementServiceHost != null)
                {
                    announcementServiceHost.Close();
                    announcementServiceHost = null;
                }
            }
        }

        void CleanClientUdp()
        {
            lock (lockudpObject)
            {
                if (clientUdp != null)
                {
                    var proxy = clientUdp as ICommunicationObject;

                    //Done with the service, let's close it.
                    try
                    {
                        if (proxy.State != CommunicationState.Faulted)
                        {
                            proxy.Close();
                        }
                    }
                    catch (Exception)
                    {
                        proxy.Abort();
                    }

                    try
                    {
                        if (clientUdp is IDisposable)
                            (clientUdp as IDisposable).Dispose();
                    }
                    catch (Exception)
                    {
                    }
                    clientUdp = null;
                }
            }
        }

        void CleanClientTcp()
        {
            lock (locktcpObject)
            {
                if (clientTcp != null)
                {
                    var proxy = clientTcp as ICommunicationObject;

                    //Done with the service, let's close it.
                    try
                    {
                        if (proxy.State != CommunicationState.Faulted)
                        {
                            proxy.Close();
                        }
                        else
                        {
                            proxy.Abort();
                        }
                    }
                    catch (Exception ex)
                    {
                        proxy.Abort();
                    }

                    try
                    {
                        if (clientTcp is IDisposable)
                            (clientTcp as IDisposable).Dispose();
                    }
                    catch (Exception ex)
                    {
                        OnErrorOccuredTags(ex);
                    }
                    clientTcp = null;
                }
            }
        }

        void DiscoverActiveServer()
        {
            bool bNotify = false;
            lock (lockObject)
            {
                bNotify = listActiveServer.Count > 0;
                listActiveServer.Clear();
                StopKeepAlive();
            }
            try
            {
                discoveringServers = true;               
                using (var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint()))
                {
                    var criteria = new FindCriteria(typeof(IRedundancyService));
                    criteria.Duration = StartupTimeout;
                    FindResponse discoveryResponse = discoveryClient.Find(criteria);
                    foreach (var endpoint in discoveryResponse.Endpoints)
                        // Console.WriteLine("Found online at {0}", endpoint.Address);
                        AddServerToActiveList(endpoint);
                }
            }
            finally 
            { 
                discoveringServers = false;
                forceRecreateChannelOnPingError = true;
            }
            if (bNotify || bIsStarting)
                OnAliveServerListChanged();
        }

        void CleanTimer()
        {
            lock (locktcpObject)
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                    pollingServer = null;
                    activatingServer = null;
                    deactivatingServer = null;
                    fullSynchronizationLastExecutionTime = DateTime.MinValue;
                }

                if (fullSynchronizationTimer != null)
                {
                    fullSynchronizationTimer.Dispose();
                    fullSynchronizationTimer = null;
                    fullSynchronizationInExecution = false;
                }
            }

            lock (lockObject)
            {
                if (delayActivateServerTimer != null)
                {
                    delayActivateServerTimer.Dispose();
                    delayActivateServerTimer = null;
                }

                StopKeepAlive();
            }
        }

        bool bStopping;
        void Stop()
        {
            if (!bIsStarted || bStopping)
                return;
            bStopping = true;

            OnServerStateChanging(RedundancyServerState.Stopping);

            CleanTimer();

            CleanClientUdp();
            CleanClientTcp();

            CloseServiceHosts();

            bIsStarted = false;
        }

        String GetCurrentMessageIp()
        {
            var context = OperationContext.Current;
            var messageProperties = context.IncomingMessageProperties;
            var endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            var ip = endpointProperty.Address;
            return ip;
        }

        String GetHostAddress(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            return endpointDiscoveryMetadata.Address.Uri.DnsSafeHost;
        }

        int GetPortAddress(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            return endpointDiscoveryMetadata.Address.Uri.Port;
        }

        void UpdateArrayPosition()
        {
            nArrayPosition = GetArrayPosition(hostName);
        }

        bool IsValidServer(String server)
        {
            if (ArrayServers == null)
                return false;
            server = server.ToLower();
            return (from c in ArrayServers where c == server select c).ToList().Count > 0;
        }

        int GetArrayPosition(String server)
        {
            if (ArrayServers == null)
                return -1;

            for (int i = 0; i < ArrayServers.Length; ++i)
            {
                if (ArrayServers[i] == server)
                    return i;
            }

            return -1;
        }

        //void StartFindDeadServerManager()
        //{
        //    var thread = new Thread((o) =>
        //    {
        //        while (!bStopping)
        //        {
        //            Thread.Sleep(100);

        //            try
        //            {
        //                var listServer = new List<String>();
        //                using (var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint()))
        //                {
        //                    var criteria = new FindCriteria(typeof(IRedundancyService));
        //                    criteria.Duration = StartupTimeout;
        //                    var discoveryResponse = discoveryClient.Find(criteria);
        //                    foreach (var endpoint in discoveryResponse.Endpoints)
        //                    {
        //                        if (bStopping)
        //                            break;
        //                        // Console.WriteLine("Found online at {0}", endpoint.Address);
        //                        var server = GetHostAddress(endpoint);
        //                        if (String.IsNullOrEmpty(server))
        //                            continue;
        //                        if (!listServer.Contains(server))
        //                            listServer.Add(server);
        //                    }

        //                    bool bActiveServerNotFound = false;
        //                    lock (lockObject)
        //                    {
        //                        var listServerChecking = new List<String>(listActiveServer);
        //                        listServerChecking.ForEach(server =>
        //                            {
        //                                if (!listServer.Contains(server))
        //                                {
        //                                    listActiveServer.Remove(server);
        //                                    if (server == pollingServer)
        //                                        bActiveServerNotFound = true;
        //                                }
        //                            });
        //                    }

        //                    if (!String.IsNullOrEmpty(pollingServer) &&
        //                        !listServer.Contains(pollingServer))
        //                        bActiveServerNotFound = true;
        //                    if (bActiveServerNotFound)
        //                    {
        //                        ElaborateActiveServer();
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                OnErrorOccuredTags(ex);
        //            }
        //        }
        //    });
        //    thread.IsBackground = true;
        //    thread.Start();
        //}


        void AddServerToActiveList(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            var server = GetHostAddress(endpointDiscoveryMetadata);
            var port = GetPortAddress(endpointDiscoveryMetadata);
            if (port == PortNumber)
                AddServerToActiveList(server);
        }

        void AddServerToActiveList(String server)
        {
            if (!IsValidServer(server) && server.ToLower() != hostName)
                return;

            bool bNotify = false;
            server = server.ToLower();
            lock (lockObject)
            {
                if (!listActiveServer.Contains(server))
                {
                    listActiveServer.Add(server);
                    if (!listAliveServer.Contains(server) && server != hostName && IsActiveServer)
                        listAliveServer.Add(server);
                    bNotify = true;
                    //activeHistorySettings.Remove(server);
                    //pendingSyncServer.Remove(server);
                    // bListActiveServerChanged = true;

                    if (listAliveServer.Count == 1)
                        StartKeepAlive();
                }
            }

            if (bNotify)
                OnAliveServerListChanged();
        }

        void RemoveServerFromActiveList(EndpointDiscoveryMetadata endpointDiscoveryMetadata)
        {
            var server = GetHostAddress(endpointDiscoveryMetadata);
            var port = GetPortAddress(endpointDiscoveryMetadata);
            if (port == PortNumber)
                RemoveServerFromActiveList(server);
        }

        void RemoveServerFromActiveList(String server)
        {
            bool bNotify = false;
            server = server.ToLower();
            lock (lockObject)
            {
                if (listActiveServer.Contains(server))
                {
                    listActiveServer.Remove(server);
                    if (listAliveServer.Contains(server))
                        listAliveServer.Remove(server);
                    bNotify = true;
                    //activeHistorySettings.Remove(server);
                    //pendingSyncServer.Remove(server);
                    // bListActiveServerChanged = true;
                }

                if (listActiveServer.Count == 0 || (listActiveServer.Count == 1 && listActiveServer[0] == hostName))
                    StopKeepAlive();
            }

            if (bNotify)
                OnAliveServerListChanged();
        }

        void RemoveDeadServersFromActiveList()
        {
            try
            {
                var listServer = new List<String>();
                using (var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint()))
                {
                    var criteria = new FindCriteria(typeof(IRedundancyService));
                    criteria.Duration = StartupTimeout;
                    var discoveryResponse = discoveryClient.Find(criteria);
                    foreach (var endpoint in discoveryResponse.Endpoints)
                    {
                        if (bStopping)
                            break;
                        // Console.WriteLine("Found online at {0}", endpoint.Address);
                        var server = GetHostAddress(endpoint);
                        var port = GetPortAddress(endpoint);
                        if (String.IsNullOrEmpty(server) || port != PortNumber)
                            continue;
                        if (!listServer.Contains(server))
                            listServer.Add(server);
                    }

                    bool bNotify = false;
                    lock (lockObject)
                    {
                        var listServerChecking = new List<String>(listActiveServer);
                        listServerChecking.ForEach(server =>
                        {
                            if (!listServer.Contains(server))
                            {
                                listActiveServer.Remove(server);
                                bNotify = true;
                            }
                        });
                    }

                    if (bNotify)
                        OnAliveServerListChanged();
                }
            }
            catch (Exception ex)
            {
                OnErrorOccuredTags(ex);
            }
        }

        void StartKeepAlive()
        {
            lock (lockObject)
            {
                if (keepAliveTimer == null && IsActiveServer && !bDisposed && !bStopping)
                {
                    keepAliveTimer = new Timer((o) =>
                    {
                        try
                        {
                            var pendingKeepAlive = Interlocked.Increment(ref nPendingKeepAlive);
                            if (pendingKeepAlive == 1 && !bDisposed && !bStopping && IsActiveServer)
                            {
                                try
                                {
                                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                                }
                                catch { }

                                String[] activeServers = null;
                                List<String> aliveServers = null;
                                lock (lockObject)
                                {
                                    activeServers = listActiveServer.ToArray();
                                    aliveServers = new List<String>(listAliveServer);
                                    listAliveServer.Clear();
                                }

                                foreach (var server in activeServers)
                                {
                                    if (!aliveServers.Contains(server) && server != hostName)
                                        RemoveServerFromActiveList(server);
                                }
                            }
                        }
                        finally
                        {
                            Interlocked.Decrement(ref nPendingKeepAlive);
                        }
                    }, this, TimeSpan.FromTicks(SynchronizeTimeout.Ticks * 10), TimeSpan.FromTicks(SynchronizeTimeout.Ticks * 10));
                }
            }
        }

        void StopKeepAlive()
        {
            lock (lockObject)
            {
                if (keepAliveTimer != null)
                { 
                    keepAliveTimer.Dispose();
                    keepAliveTimer = null;
                }

                listAliveServer.Clear();
            }
        }

        void ElaborateActiveServer()
        {
            try
            {
                lock (lockObject)
                {
                    //if (!bListActiveServerChanged)
                    //    return;
                    //bListActiveServerChanged = false;

                    bool bFoundActiveServer = false;
                    if (KeepActiveServerActive)
                    {
                        if (listActiveServer.Contains(hostName))
                        {
                            bFoundActiveServer = listActiveServer.Count > 1;
                        }
                        else
                            bFoundActiveServer = listActiveServer.Count > 0;

                        if (bIsActiveServer)
                        {
                            if (activatingServer != null)
                            {
                                var serverIndex = GetArrayPosition(activatingServer);
                                if (serverIndex >= 0 && serverIndex < nArrayPosition)
                                {
                                    DeactivateServerProcedure();
                                    return;
                                }
                            }
                            else if (!bWasNetworkAvailable && bFoundActiveServer)
                            {
                                string activeServer = null;
                                foreach (var server in listActiveServer)
                                {
                                    var serverIndex = GetArrayPosition(server);
                                    if (serverIndex >= 0 && serverIndex < nArrayPosition)
                                    {
                                        activeServer = server;
                                        break;
                                    }
                                }

                                DeactivateServerProcedure();
                                if (activatingServer != null)
                                {
                                    StartPollingToServer(activeServer);
                                    OnActiveServerChanged(activeServer);
                                }
                                return;
                            }

                            startTimeActivation = DateTime.MinValue;
                            ActivateServerProcedure();
                            return;
                        }

                        if (bIsStarting && bFoundActiveServer && activatingServer == null)
                        {
                            bool bStartDelayActivateServerTimer = true;
                            foreach (var server in listActiveServer)
                            {
                                var serverIndex = GetArrayPosition(server);
                                if (serverIndex >= 0 && serverIndex < nArrayPosition)
                                {
                                    bStartDelayActivateServerTimer = false;
                                    break;
                                }
                            }

                            if (bStartDelayActivateServerTimer)
                            {
                                delayActivateServerTimer = new Timer((o) =>
                                {
                                    lock (lockObject)
                                    {
                                        if (delayActivateServerTimer != null)
                                        {
                                            delayActivateServerTimer.Dispose();
                                            delayActivateServerTimer = null;
                                        }
                                    }

                                    ActivateServerProcedure();
                                }, this, StartupTimeout, TimeSpan.FromMilliseconds(-1));
                            }
                        }
                    }
                    else
                    {
                        foreach (var server in listActiveServer)
                        {
                            var serverIndex = GetArrayPosition(server);
                            if (serverIndex >= 0 && serverIndex < nArrayPosition)
                            {
                                bFoundActiveServer = true;
                                break;
                            }
                        }
                    }

                    if (!bFoundActiveServer)
                        ActivateServerProcedure();
                    else
                        DeactivateServerProcedure();
                }
            }
            catch (Exception ex)
            {
                OnErrorOccuredTags(ex);
            }
        }

        void ActivateServerProcedure()
        {
            lock (lockudpObject)
            {
                if (bIsActiveServer && clientUdp != null)
                {
                    CleanClientUdp();
                    clientUdp = ChannelFactory<IRedundancyService>.CreateChannel(new BinaryUdpBinding() { MaxRetransmitCount = MaxRetransmitCount }, new EndpointAddress(String.Format(UdpBaseAddress, UdpClientIpAddress, PortNumber)));
                    ((IContextChannel)clientUdp).OperationTimeout = Timeout;
                    clientUdp.ServerActivating(hostName);
                    return;
                }
            }

            bool isActiveServerCopy = bIsActiveServer; //a copy is create in order to use a variable without any modification

            CleanTimer();

            CleanClientTcp();

            if(!isActiveServerCopy)
                OnServerStateChanging(RedundancyServerState.Activating);

            if (bIsStarting)
            {
                String[] activeServers = null;
                lock (lockObject)
                {
                    activeServers = (from c in ArrayServers where listActiveServer.Contains(c) select c).ToArray();
                }

                bool bSuccessfullySynchronized = true;
                for (int i = 0; i < activeServers.Length; ++i)
                {
                    if (activeServers[i] != hostName)
                    {
                        try
                        {
                            Dictionary<Opc.Ua.NodeId, LiveDataValue> tags = null;
                            Dictionary<Opc.Ua.NodeId, WrappedAlarmStatusCollection> alarms = null;
                            HistorySettings historySettings = null;

                            var server = activeServers[i];
                            lock (locktcpObject)
                            {
                                CleanClientTcp();
                                var address = String.Format(NetTcpBaseAddress, server, PortNumber);
                                clientTcp = ChannelFactory<IRedundancyService>.CreateChannel(
                                    new BinaryNetTcpBinding(NetTcpSecurityMode) { MaxReceivedMessageSize = MaxReceivedMessageSize, SendTimeout = SynchronizeTimeout },
                                    new EndpointAddress(address));
                                ((IContextChannel)clientTcp).OperationTimeout = Timeout;

                                try
                                {
                                    tags = clientTcp.GetAllLiveData();
                                }
                                catch (Exception ex)
                                {
                                    OnErrorOccuredTags(ex);
                                }

                                try
                                {
                                    alarms = clientTcp.GetAllAlarmsStatus();
                                }
                                catch (Exception ex)
                                {
                                    OnErrorOccuredTags(ex);
                                }

                                if (!SkipHistoryDataSynchronization)
                                {
                                    try
                                    {
                                        historySettings = clientTcp.GetHistorySettings();
                                    }
                                    catch (Exception ex)
                                    {
                                        OnErrorOccuredTags(ex);
                                    }
                                }
                            }

                            OnSynchronizeAllLiveData(tags, server);
                            OnSynchronizeAllAlarmsStatus(alarms, server);
                            if (!SkipHistoryDataSynchronization)
                                OnSynchronizeHistoryData(historySettings, DateTime.MinValue, startTimeActivation, server, isStarting: true);
                            bSuccessfullySynchronized = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            bSuccessfullySynchronized = false;
                            OnErrorOccuredTags(ex);
                        }
                    }
                }

                if (!bSuccessfullySynchronized)
                {
                    Stop();
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        String.Format(Properties.Resources.RedundancyStartupErrorForMissingSyncronization),
                                        System.Diagnostics.EventLogEntryType.Error,
                                        LoggerDestination.Redundancy);
                    return;
                }
            }

            bWasActive = false;
            bIsActiveServer = true;
            startTimeActivation = DateTime.UtcNow;

            CleanClientTcp();
            SetUpFullSynchronizationTimer();

            if (bIsNetworkAvailable)
            {
                OpenServiceHosts();
                lock (lockudpObject)
                {
                    CleanClientUdp();
                    clientUdp = ChannelFactory<IRedundancyService>.CreateChannel(new BinaryUdpBinding() { MaxRetransmitCount = MaxRetransmitCount }, new EndpointAddress(String.Format(UdpBaseAddress, UdpClientIpAddress, PortNumber)));
                    ((IContextChannel)clientUdp).OperationTimeout = Timeout;
                    clientUdp.ServerActivating(hostName);
                }
            }

            if (!isActiveServerCopy)
            {
                OnServerActive();
                OnActiveServerChanged(hostName);
            }
        }

        bool bFirstTime;
        void DeactivateServerProcedure()
        {
            if (!bIsActiveServer && bFirstTime)
                return;

            bool isActiveServerCopy = bIsActiveServer; //a copy is create in order to use a variable without any modification

            if(isActiveServerCopy)
                OnServerStateChanging(RedundancyServerState.Deactivating);

            bWasActive = bIsActiveServer;
            bIsActiveServer = false;
            bFirstTime = true;
            startTimeActivation = DateTime.MinValue;

            lock (lockudpObject)
            {
                if (bWasActive && clientUdp != null)
                {
                    CleanClientUdp();
                    clientUdp = ChannelFactory<IRedundancyService>.CreateChannel(new BinaryUdpBinding() { MaxRetransmitCount = MaxRetransmitCount }, new EndpointAddress(String.Format(UdpBaseAddress, UdpClientIpAddress, PortNumber)));
                    ((IContextChannel)clientUdp).OperationTimeout = Timeout;
                    clientUdp.ServerDeactivating(hostName);
                    CleanClientUdp();
                }
            }

            if (isActiveServerCopy)
                OnServerInactive();
        }

        void SetUpFullSynchronizationTimer()
        {
            if (SkipHistoryDataSynchronization || 
                (FullSynchronizationStartTime == DateTime.MinValue && FullSynchronizationTimeSpan == TimeSpan.Zero))
                return;

            lock (locktcpObject)
            {
                if (fullSynchronizationTimer != null)
                    return;

                TimeSpan dueTime = FullSynchronizationTimeSpan;
                if (dueTime == TimeSpan.Zero)
                {
                    TimeSpan nowTimeOfDay = DateTime.Now.TimeOfDay;
                    if (FullSynchronizationStartTime.TimeOfDay > nowTimeOfDay)
                        dueTime = FullSynchronizationStartTime.TimeOfDay - nowTimeOfDay;
                    else
                        dueTime = TimeSpan.FromDays(1) - nowTimeOfDay + FullSynchronizationStartTime.TimeOfDay;
                }

                TimeSpan period = FullSynchronizationTimeSpan;
                if (period == TimeSpan.Zero)
                {
                    period = TimeSpan.FromHours(24.0);
                }

                fullSynchronizationTimer = new Timer((o) =>
                {
                    StartFullSynchronizationOnActiveServer();
                }, this, dueTime, period);
            }
        }

        void StartFullSynchronizationOnActiveServer()
        {
            lock (locktcpObject)
            {
                if (fullSynchronizationInExecution)
                    return;
                fullSynchronizationInExecution = true;
            }

            try
            {
                if (IsActiveServer)
                {
                    String[] activeServers = null;
                    lock (lockObject)
                    {
                        activeServers = listActiveServer.ToArray();
                    }

                    for (int i = 0; i < activeServers.Length; ++i)
                    {
                        if (activeServers[i] != hostName)
                        {
                            var server = activeServers[i];
                            lock (locktcpObject)
                            {
                                if (bStopping || !bIsActiveServer)
                                    break;

                                try
                                {
                                    CleanClientTcp();
                                    var address = String.Format(NetTcpBaseAddress, server, PortNumber);
                                    clientTcp = ChannelFactory<IRedundancyService>.CreateChannel(
                                        new BinaryNetTcpBinding(NetTcpSecurityMode) { MaxReceivedMessageSize = MaxReceivedMessageSize, SendTimeout = SynchronizeTimeout },
                                        new EndpointAddress(address));
                                    ((IContextChannel)clientTcp).OperationTimeout = Timeout;

                                    var historySettings = clientTcp.GetHistorySettings();
                                    OnSynchronizeHistoryData(historySettings, System.Data.SqlTypes.SqlDateTime.MinValue.Value, startTimeActivation, server);

                                    TimeSpan period = FullSynchronizationTimeSpan;
                                    if (period == TimeSpan.Zero)
                                    {
                                        period = TimeSpan.FromHours(24.0);
                                    }

                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        String.Format(Properties.Resources.StartFullHistoricalDataSynchronization, server, DateTime.Now + period),
                                        System.Diagnostics.EventLogEntryType.Information,
                                        LoggerDestination.Redundancy);

                                    CleanClientTcp();
                                }
                                catch (Exception ex)
                                {
                                    OnErrorOccuredTags(ex);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                lock (locktcpObject)
                {
                    fullSynchronizationInExecution = false;
                }
            }
        }

        bool NeedToExecuteFullSynchronizationOnInactiveServer()
        {
            if (FullSynchronizationStartTime == DateTime.MinValue && FullSynchronizationTimeSpan == TimeSpan.Zero)
                return false;

            if (FullSynchronizationTimeSpan == TimeSpan.Zero)
            {
                return DateTime.Now - fullSynchronizationLastExecutionTime >= TimeSpan.FromHours(24.0);
            }
            else
            {
                return fullSynchronizationLastExecutionTime <= DateTime.Now - FullSynchronizationTimeSpan;
            }
        }

        void SetFullSynchronizationLastExecutionTime()
        {
            if (FullSynchronizationTimeSpan == TimeSpan.Zero)
            {
                fullSynchronizationLastExecutionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    FullSynchronizationStartTime.Hour, FullSynchronizationStartTime.Minute, FullSynchronizationStartTime.Second);
                if (DateTime.Now.TimeOfDay < FullSynchronizationStartTime.TimeOfDay)
                    fullSynchronizationLastExecutionTime = fullSynchronizationLastExecutionTime.AddDays(-1);
            }
            else
            {
                fullSynchronizationLastExecutionTime = DateTime.Now;
            }
        }

        public void SendChangedTags(List<ChangedTags> changedTags)
        {
            if (!bIsActiveServer/* || bIsStarting*/)
                return;

            var helper = new Helper.ChangedTagsHelper(changedTags);
            while (!helper.IsTerminated)
            {
                if (!bIsActiveServer || bStopping)
                    return;

                var sectionTags = helper.GetChangedElementsBlock();

                lock (lockudpObject)
                {
                    if (clientUdp == null)
                        return;

                    try
                    {
                        clientUdp.ChangedTags(hostName, sectionTags);
                        helper.GoToNextChangedElementsBlock();
                    }
                    catch (Exception ex)
                    {
                        if (!helper.SplitChangedElementsBlock())
                        {
                            helper.GoToNextChangedElementsBlock();
                            OnErrorOccuredTags(ex);
                        }
                    }
                }
            }
        }

        public void SendChangedTags(ChangedTags changedTags)
        {
            if (!bIsActiveServer/* || bIsStarting*/)
                return;

            lock (lockudpObject)
            {
                if (clientUdp == null)
                    return;

                try
                {
                    clientUdp.ChangedTags(hostName, new List<ChangedTags> { changedTags });
                }
                catch (Exception ex)
                {
                    OnErrorOccuredTags(ex);
                }
            }
        }

        public void SendChangedAlarms(ChangedAlarms changedAlarms)
        {
            if (!bIsActiveServer/* || bIsStarting*/)
                return;

            var helper = new Helper.ChangedAlarmsHelper(changedAlarms);
            while (!helper.IsTerminated)
            {
                if (!bIsActiveServer || bStopping)
                    return;

                var sectionAlarms = helper.GetChangedElementsBlock();

                lock (lockudpObject)
                {
                    if (clientUdp == null)
                        return;

                    try
                    {
                        clientUdp.ChangedAlarms(hostName, sectionAlarms[0]);
                        helper.GoToNextChangedElementsBlock();
                    }
                    catch (Exception ex)
                    {
                        if (!helper.SplitChangedElementsBlock())
                        {
                            helper.GoToNextChangedElementsBlock();
                            OnErrorOccuredTags(ex);
                        }
                    }
                }
            }
        }

        public Opc.Ua.ServiceResult SwitchActiveServer()
        {
            if (!IsActiveServer || !KeepActiveServerActive)
                return Opc.Ua.StatusCodes.BadNotSupported;

            lock (lockObject)
            {
                if (listActiveServer.Count == 0 ||
                    listActiveServer.Count == 1 && listActiveServer.Contains(hostName))
                    return Opc.Ua.StatusCodes.Good;
            }

            var switchingActiveServer = Interlocked.Increment(ref nSwitchingActiveServer);
            if (switchingActiveServer == 1)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        DeactivateServerProcedure();
                    }
                    finally
                    {
                        Interlocked.Decrement(ref nSwitchingActiveServer);
                    }
                });
            }
            else
            {
                Interlocked.Decrement(ref nSwitchingActiveServer);
            }

            return Opc.Ua.StatusCodes.Good;
        }

        int errorCounter = 0;
        void StartPollingToServer(String server)
        {
            try
            {
                bool bSyncronizeLiveData = false;
                Dictionary<Opc.Ua.NodeId, LiveDataValue> tags = null;
                Dictionary<Opc.Ua.NodeId, WrappedAlarmStatusCollection> alarms = null;

                lock (locktcpObject)
                {
                    if (bIsActiveServer || timer != null && pollingServer == server || bStopping)
                        return;

                    CleanTimer();

                    CleanClientTcp();
                    var address = String.Format(NetTcpBaseAddress, server, PortNumber);
                    clientTcp = ChannelFactory<IRedundancyService>.CreateChannel(
                        new BinaryNetTcpBinding(NetTcpSecurityMode) { MaxReceivedMessageSize = MaxReceivedMessageSize, SendTimeout = SynchronizeTimeout },
                        new EndpointAddress(address));
                    ((IContextChannel)clientTcp).OperationTimeout = Timeout;

                    bSyncronizeLiveData = !bWasActive || !bWasNetworkAvailable;
                    if (bSyncronizeLiveData)
                    {
                        tags = clientTcp.GetAllLiveData();
                        alarms = clientTcp.GetAllAlarmsStatus();
                    }

                    HistorySettings historySettings = null;
                    if (!SkipHistoryDataSynchronization)
                        historySettings = clientTcp.GetHistorySettings();

                    errorCounter = 0;
                    bRecreateChannelOnPingError = true;
                    bWasActive = false;
                    bWasNetworkAvailable = bIsNetworkAvailable;
                    pollingServer = server;
                    SetFullSynchronizationLastExecutionTime();
                    timer = new Timer((o) =>
                    {
                        try
                        {
                            var pendingPings = Interlocked.Increment(ref nPendingPings);
                            if (pendingPings == 1 && !bDisposed && !bStopping && !IsActiveServer && !discoveringServers)
                            {
                                lock (locktcpObject)
                                {
                                    if (errorCounter < 3)
                                    {
                                        try
                                        {
                                            clientTcp.Ping(hostName);
                                            errorCounter = 0;
                                            bRecreateChannelOnPingError = true;
                                            forceRecreateChannelOnPingError = false;

                                            if (!SkipHistoryDataSynchronization)
                                            {
                                                try
                                                {
                                                    var startTime = DateTime.MinValue;
                                                    if (NeedToExecuteFullSynchronizationOnInactiveServer())
                                                    {
                                                        SetFullSynchronizationLastExecutionTime();
                                                        startTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

                                                        TimeSpan period = FullSynchronizationTimeSpan;
                                                        if (period == TimeSpan.Zero)
                                                        {
                                                            period = TimeSpan.FromHours(24.0);
                                                        }

                                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                            String.Format(Properties.Resources.StartFullHistoricalDataSynchronization, server, DateTime.Now + period),
                                                            System.Diagnostics.EventLogEntryType.Information,
                                                            LoggerDestination.Redundancy);
                                                    }
                                                    OnSynchronizeHistoryData(historySettings, startTime, DateTime.MinValue, server);
                                                }
                                                catch (Exception ex)
                                                {
                                                    OnErrorOccuredTags(ex);
                                                    ++errorCounter;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            if (ex is CommunicationException || // after killing the process on active server.
                                                ex is TimeoutException) // after disabling or unplugging the network card on active server.
                                            {
                                                bRecreateChannelOnPingError = false;
                                            }
                                            
                                            if (bRecreateChannelOnPingError || forceRecreateChannelOnPingError)
                                            {
                                                forceRecreateChannelOnPingError = false;
                                                CleanClientTcp();
                                                clientTcp = ChannelFactory<IRedundancyService>.CreateChannel(
                                                    new BinaryNetTcpBinding(NetTcpSecurityMode) { MaxReceivedMessageSize = MaxReceivedMessageSize, SendTimeout = SynchronizeTimeout },
                                                    new EndpointAddress(address));
                                                ((IContextChannel)clientTcp).OperationTimeout = Timeout;
                                            }

                                            OnErrorOccuredTags(ex);
                                            ++errorCounter;
                                        }
                                    }
                                }

                                if (errorCounter >= 3)
                                {
                                    RemoveDeadServersFromActiveList();
                                    RemoveServerFromActiveList(server);
                                    ElaborateActiveServer();
                                }
                            }
                        }
                        finally
                        {
                            Interlocked.Decrement(ref nPendingPings);
                        }
                    }, this, TimeSpan.FromMilliseconds(-1), SynchronizeTimeout);
                }

                if (bSyncronizeLiveData)
                {
                    OnSynchronizeAllLiveData(tags, server);
                    OnSynchronizeAllAlarmsStatus(alarms, server);
                }

                OnServerStateChanging(RedundancyServerState.StartPolling);

                lock (locktcpObject)
                {
                    if (timer != null && !bStopping)
                        timer.Change(TimeSpan.FromMilliseconds(0), SynchronizeTimeout);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccuredTags(ex);
                if (++errorCounter >= 3)
                {
                    RemoveServerFromActiveList(server);
                    ElaborateActiveServer();
                }
                else
                {
                    var pendingErrors = errorCounter;
                    StartPollingToServer(server);
                    errorCounter = pendingErrors;
                }
            }
        }

        public uint SendWritingData(ChangedTags changedTag)
        {
            lock (locktcpObject)
            {
                if (clientTcp == null)
                    return Opc.Ua.StatusCodes.Bad;

                try
                {
                    return clientTcp.WriteData(changedTag);
                }
                catch (Exception ex)
                {
                    OnErrorOccuredTags(ex);
                }
            }

            return Opc.Ua.StatusCodes.Bad;
        }

        public uint SendUpdatingAlarms(ChangedAlarms changedAlarms)
        {
            lock (locktcpObject)
            {
                if (clientTcp == null)
                    return Opc.Ua.StatusCodes.Bad;

                try
                {
                    return clientTcp.UpdateAlarms(changedAlarms);
                }
                catch (Exception ex)
                {
                    OnErrorOccuredTags(ex);
                }
            }

            return Opc.Ua.StatusCodes.Bad;
        }

        public MethodResult SendCallingMethod(CallMethod callMethod)
        {
            lock (locktcpObject)
            {
                if (clientTcp == null)
                    return new MethodResult { serviceResult = Opc.Ua.StatusCodes.Bad };

                try
                {
                    return clientTcp.CallMethod(callMethod);
                }
                catch (Exception ex)
                {
                    OnErrorOccuredTags(ex);
                }
            }

            return new MethodResult { serviceResult = Opc.Ua.StatusCodes.Bad };
        }

        #endregion

        #region Properties
        public bool Online
        {
            get
            {
                return bIsStarted;
            }
            set
            {
                if (value == true)
                    Start();
                else
                    Stop();
            }
        }

        public bool IsActiveServer
        {
            get
            {
                return bIsActiveServer;
            }
        }

        public bool IsSwitchExecutable
        {
            get
            {
                return KeepActiveServerActive && ArrayServers.Length == 2;
            }
        }

        public int ArrayPosition
        {
            get
            {
                return nArrayPosition;
            }
        }

        /// <summary>
        /// Gets and sets the ArrayServers.
        /// </summary>
        [Display(Name = "ArrayServersLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        String[] arrayServers;
        public String[] ArrayServers
        {
            get
            {
                return arrayServers;
            }
            set
            {
                if (value != null)
                {
                    var lowerList = new List<String>();
                    foreach (var s in value)
                        lowerList.Add(s.ToLower());
                    Set(ref arrayServers, lowerList.ToArray(), "ArrayServers");
                }
                else
                    Set(ref arrayServers, value, "ArrayServers");
                UpdateArrayPosition();
            }
        }

        /// <summary>
        /// Gets and sets the StartupTimeout.
        /// </summary>
        [Display(Name = "StartupTimeoutLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        TimeSpan startupTimeout = TimeSpan.FromSeconds(5);
        public TimeSpan StartupTimeout
        {
            get
            {
                return startupTimeout;
            }
            set
            {
                Set(ref startupTimeout, value, "StartupTimeout");
            }
        }

        
        /// <summary>
        /// Gets and sets the SynchronizeTimeout.
        /// </summary>
        [Display(Name = "SynchronizeTimeoutLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        TimeSpan synchronizeTimeout = TimeSpan.FromSeconds(5);
        public TimeSpan SynchronizeTimeout
        {
            get
            {
                return synchronizeTimeout;
            }
            set
            {
                Set(ref synchronizeTimeout, value, "SynchronizeTimeout");
            }
        }

        /// <summary>
        /// Gets and sets the Timeout.
        /// </summary>
        [Display(Name = "TimeoutLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        TimeSpan timeout = TimeSpan.FromSeconds(30);
        public TimeSpan Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                Set(ref timeout, value, "Timeout");
            }
        }

        /// <summary>
        /// Gets and sets the date time for starting the full syncronization of history data.
        /// </summary>
        [Display(Name = "FullSynchronizationStartTimeLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        DateTime fullSynchronizationStartTime = DateTime.MinValue;
        public DateTime FullSynchronizationStartTime
        {
            get
            {
                return fullSynchronizationStartTime;
            }
            set
            {
                Set(ref fullSynchronizationStartTime, value, "FullSynchronizationStartTime");
            }
        }

        /// <summary>
        /// Gets and sets the time span for starting the full syncronization of history data.
        /// </summary>
        [Display(Name = "FullSynchronizationTimeSpanLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        TimeSpan fullSynchronizationTimeSpan = TimeSpan.Zero;
        public TimeSpan FullSynchronizationTimeSpan
        {
            get
            {
                return fullSynchronizationTimeSpan;
            }
            set
            {
                Set(ref fullSynchronizationTimeSpan, value, "FullSynchronizationTimeSpan");
            }
        }

        /// <summary>
        /// Gets and sets the history thread pool number (default '-1' means based on logical CPU)
        /// </summary>
        [Display(Name = "HistoryThreadPool", ResourceType = typeof(Properties.Resources))]
        [Required]
        int historyThreadPool = -1;
        public int HistoryThreadPool
        {
            get
            {
                return historyThreadPool;
            }
            set
            {
                Set(ref historyThreadPool, value, "HistoryThreadPool");
            }
        }

        /// <summary>
        /// Gets and sets the transport port number.
        /// </summary>
        [Display(Name = "TransportPort", ResourceType = typeof(Properties.Resources))]
        [Required]
        uint portNumber = 40000;
        public uint PortNumber
        {
            get
            {
                return portNumber;
            }
            set
            {
                Set(ref portNumber, value, "PortNumber");
            }
        }

        /// <summary>
        /// Gets the NetTcpBaseAddress.
        /// </summary>
        [Display(Name = "NetTcpBaseAddressLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        String netTcpBaseAddress = "net.tcp://{0}:{1}/RedundancyService";
        public String NetTcpBaseAddress
        {
            get
            {
                return netTcpBaseAddress;
            }
        }

        /// <summary>
        /// Gets the UdpBaseAddress.
        /// </summary>
        [Display(Name = "UdpBaseAddressLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        String udpBaseAddress = "soap.udp://{0}:{1}/RedundancyService";
        public String UdpBaseAddress
        {
            get
            {
                return udpBaseAddress;
            }
        }

        /// <summary>
        /// Gets and sets the UdpServiceIpAddress.
        /// </summary>
        [Display(Name = "UdpServiceIpAddressLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        String udpServiceIpAddress = "224.0.0.1";
        public String UdpServiceIpAddress
        {
            get
            {
                return udpServiceIpAddress;
            }
            set
            {
                Set(ref udpServiceIpAddress, value, "UdpServiceIpAddress");
            }
        }

        /// <summary>
        /// Gets and sets the UdpClientIpAddress.
        /// </summary>
        [Display(Name = "UdpClientIpAddressLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        String udpClientIpAddress = "224.0.0.1";
        public String UdpClientIpAddress
        {
            get
            {
                return udpClientIpAddress;
            }
            set
            {
                Set(ref udpClientIpAddress, value, "UdpClientIpAddress");
            }
        }

        /// <summary>
        /// Gets and sets the Max Retransmit Count.
        /// </summary>
        [Display(Name = "MaxRetransmitCount", ResourceType = typeof(Properties.Resources))]
        [Required]
        int maxRetransmitCount = 0;
        public int MaxRetransmitCount
        {
            get
            {
                return maxRetransmitCount;
            }
            set
            {
                Set(ref maxRetransmitCount, value, "MaxRetransmitCount");
            }
        }        

        /// <summary>
        /// Gets and sets the max received message size
        /// </summary>
        [Display(Name = "MaxReceivedMessageSize", ResourceType = typeof(Properties.Resources))]
        [Required]
        long maxReceivedMessageSize = 524288; // 512 KB
        public long MaxReceivedMessageSize
        {
            get
            {
                return maxReceivedMessageSize;
            }
            set
            {
                Set(ref maxReceivedMessageSize, value, "MaxReceivedMessageSize");
            }
        }

        /// <summary>
        /// Gets and sets the max received message size
        /// </summary>
        [Display(Name = "KeepActiveServerActive", ResourceType = typeof(Properties.Resources))]
        [Required]
        bool keepActiveServerActive = false;
        public bool KeepActiveServerActive
        {
            get
            {
                return keepActiveServerActive;
            }
            set
            {
                Set(ref keepActiveServerActive, value, "KeepActiveServerActive");
            }
        }

        /// <summary>
        /// Gets and sets the max received message size
        /// </summary>
        [Display(Name = "SkipHistoryDataSynchronization", ResourceType = typeof(Properties.Resources))]
        [Required]
        bool skipHistoryDataSynchronization = false;
        public bool SkipHistoryDataSynchronization
        {
            get
            {
                return skipHistoryDataSynchronization;
            }
            set
            {
                Set(ref skipHistoryDataSynchronization, value, "SkipHistoryDataSynchronization");
            }
        }

        /// <summary>
        /// Gets and sets the NetTcp security mode.
        /// </summary>
        [Display(Name = "NetTcpSecurityModeLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        SecurityMode netTcpSecurityMode = SecurityMode.None;
        public SecurityMode NetTcpSecurityMode
        {
            get
            {
                return netTcpSecurityMode;
            }
            set
            {
                Set(ref netTcpSecurityMode, value, "NetTcpSecurityMode");
            }
        }

        String historianConnection;
        [Browsable(false)]
        public String HistorianConnection
        {
            get
            {
                return historianConnection;
            }
            set
            {
                Set(ref historianConnection, value, "HistorianConnection");
            }
        }

        String eventConnection;
        [Browsable(false)]
        public String EventConnection
        {
            get
            {
                return eventConnection;
            }
            set
            {
                Set(ref eventConnection, value, "EventConnection");
            }
        }

        Dictionary<Opc.Ua.NodeId, String> mapHistorianCustomConnection;
        [Browsable(false)]
        public Dictionary<Opc.Ua.NodeId, String> MapHistorianCustomConnection
        {
            get
            {
                return mapHistorianCustomConnection;
            }
            set
            {
                Set(ref mapHistorianCustomConnection, value, "MapHistorianCustomConnection");
            }
        }

        Dictionary<String, DataConnectionParameters> mapDataLoggerConnection;
        [Browsable(false)]
        public Dictionary<String, DataConnectionParameters> MapDataLoggerConnection
        {
            get
            {
                return mapDataLoggerConnection;
            }
            set
            {
                Set(ref mapDataLoggerConnection, value, "MapDataLoggerConnection");
            }
        }
        #endregion

        #region Events
        public event EventHandler<ServerStateArgs> ServerStateChanging;
        void OnServerStateChanging(RedundancyServerState state)
        {
            var t = ServerStateChanging;
            if (t != null)
            {
                var eventArg = new ServerStateArgs() { serverState = state };
                t(this, eventArg);
            }
        }

        public event EventHandler ActiveServer;
        void OnServerActive()
        {
            var t = ActiveServer;
            if (t != null)
                t(this, EventArgs.Empty);
        }

        public event EventHandler InactiveServer;
        void OnServerInactive()
        {
            var t = InactiveServer;
            if (t != null)
                t(this, EventArgs.Empty);
        }

        public event EventHandler<ActiveServerChangedArgs> ActiveServerChanged;
        void OnActiveServerChanged(string activeServer)
        {
            var t = ActiveServerChanged;
            if (t != null)
                t(this, new ActiveServerChangedArgs() { activeServer = activeServer });
        }

        public event EventHandler<AliveServerListChangedArgs> AliveServerListChanged;
        void OnAliveServerListChanged()
        {
            var t = AliveServerListChanged;
            if (t != null)
            {
                string[] activeServers = null;
                lock (lockObject)
                {
                    activeServers = (from c in ArrayServers where listActiveServer.Contains(c) || c == hostName select c).ToArray();
                }
                t(this, new AliveServerListChangedArgs() { aliveServers = activeServers });
            }
        }

        public event EventHandler<SynchronizeHistoryDataEvent> SynchronizeHistoryData;
        bool OnSynchronizeHistoryData(HistorySettings hs, DateTime st, DateTime et, String server, bool isStarting = false)
        {
            var t = SynchronizeHistoryData;
            if (t != null)
            {
                var eventArg = new SynchronizeHistoryDataEvent() 
                {
                    IsStarting = isStarting,
                    historySettings = hs, 
                    startTime = st,
                    endTime = et, 
                    hostName = server
                };
                t(this, eventArg);
                return eventArg.IsExecuted;
            }

            return true;
        }

        public event EventHandler<List<ChangedTagArgs>> TagsChanged;
        void OnChangedTags(List<ChangedTags> changedTags)
        {
            var t = TagsChanged;
            if (t != null)
            {
                var arg = new List<ChangedTagArgs>();
                changedTags.ForEach((item) => 
                {
                    var changedTag = new ChangedTagArgs()
                    {
                        NodeId = item.NodeId,
                        DataValues = item.DataValues
                    };

                    if (item.Statistics != null)
                    {
                        changedTag.Min = item.Statistics.Min;
                        changedTag.Max = item.Statistics.Max;
                        changedTag.TotAverage = item.Statistics.TotAverage;
                        changedTag.CountUpdates = item.Statistics.CountUpdates;
                        changedTag.TotalTimeOn = item.Statistics.TotalTimeOn;
                        changedTag.LastTotalTimeOn = item.Statistics.LastTotalTimeOn;
                        changedTag.LastDoubleValue = item.Statistics.LastDoubleValue;
                    }

                    arg.Add(changedTag);
                });
                t(this, arg);
            }
        }

        public event EventHandler<UFUAAlarm.ChangedAlarmsArgs> AlarmsChanged;
        void OnChangedAlarms(ChangedAlarms changedAlarms)
        {
            var t = AlarmsChanged;
            if (t != null)
            {
                t(this, new UFUAAlarm.ChangedAlarmsArgs() { nodeId = changedAlarms.NodeId, alarmsStatus = changedAlarms.AlarmsStatus });
            }
        }

        public event EventHandler<LiveDataArgs> SynchronizeAllLiveData;
        void OnSynchronizeAllLiveData(Dictionary<Opc.Ua.NodeId, LiveDataValue> data, String server)
        {
            var t = SynchronizeAllLiveData;
            if (t != null)
            {
                t(this, new LiveDataArgs() { liveData = data, hostName = server });
            }
        }

        public event EventHandler<AlarmsStatusArgs> SynchronizeAllAlarmsStatus;
        void OnSynchronizeAllAlarmsStatus(Dictionary<Opc.Ua.NodeId, WrappedAlarmStatusCollection> alarms, String server)
        {
            var t = SynchronizeAllAlarmsStatus;
            if (t != null)
            {
                Dictionary<Opc.Ua.NodeId, List<UFUAAlarm.AlarmStatus>> status = null;
                if (alarms != null)
                {
                    status = new Dictionary<Opc.Ua.NodeId, List<UFUAAlarm.AlarmStatus>>(alarms.Keys.Count);
                    foreach (var key in alarms.Keys)
                        status.Add(key, alarms[key]);
                }

                t(this, new AlarmsStatusArgs() { alarmsStatus = status, hostName = server });
            }
        }
        
        public event EventHandler<LiveDataArgs> GettingAllLiveData;
        Dictionary<Opc.Ua.NodeId, LiveDataValue> OnGettingAllLiveData()
        {
            var t = GettingAllLiveData;
            if (t != null)
            {
                var arg = new LiveDataArgs();
                t(this, arg);
                return arg.liveData;
            }

            return null;
        }

        public event EventHandler<AlarmsStatusArgs> GettingAllAlarmsStatus;
        Dictionary<Opc.Ua.NodeId, List<UFUAAlarm.AlarmStatus>> OnGettingAllAlarmsStatus()
        {
            var t = GettingAllAlarmsStatus;
            if (t != null)
            {
                var arg = new AlarmsStatusArgs();
                t(this, arg);
                return arg.alarmsStatus;
            }

            return null;
        }

        public event EventHandler<ChangedTagArgs> WritingValue;
        uint OnWritingValue(ChangedTags changedTags)
        {
            var t = WritingValue;
            if (t != null)
            {
                var arg = new ChangedTagArgs() { NodeId = changedTags.NodeId, DataValues = changedTags.DataValues, Status = Opc.Ua.StatusCodes.Good };
                t(this, arg);
                return arg.Status;
            }

            return Opc.Ua.StatusCodes.Good;
        }
        
        public event EventHandler<UFUAAlarm.ChangedAlarmsArgs> UpdatingAlarms;
        uint OnUpdatingAlarms(ChangedAlarms changedAlarms)
        {
            var t = UpdatingAlarms;
            if (t != null)
            {
                var arg = new UFUAAlarm.ChangedAlarmsArgs() { nodeId = changedAlarms.NodeId, alarmsStatus = changedAlarms.AlarmsStatus };
                t(this, arg);
            }

            return Opc.Ua.StatusCodes.Good;
        }

        public event EventHandler<CallMethodArgs> CallingMethod;
        MethodResult OnCallingMethod(CallMethod callMethod)
        {
            var t = CallingMethod;
            if (t != null)
            {
                var arg = new CallMethodArgs { methodNodeId = callMethod.methodNodeId, methodRequest = callMethod.methodRequest, methodResult = callMethod.methodResult };
                t(this, arg);
                return new MethodResult { serviceResult = arg.serviceResult, outputArguments = arg.methodResult?.OutputArguments };
            }

            return new MethodResult { serviceResult = Opc.Ua.StatusCodes.Good };
        }

        public event EventHandler<ErrorArgs> ErrorOccured;
        void OnErrorOccuredTags(Exception e)
        {
            if (bStopping)
                return;

            var t = ErrorOccured;
            if (t != null)
            {
                t(this, new ErrorArgs() { Ex = e });
            }
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            Stop();
        }
        #endregion

        #region IRedundancyService
        public void ServerActivating(String server)
        {
            if (server == hostName || bStopping)
                return;

            if (!IsValidServer(server))
                return;

            bool activeServerChanged = activatingServer != server;
            activatingServer = server;
            var activatingServers = Interlocked.Increment(ref nActivatingServers);
            if (activatingServers == 1 || activeServerChanged)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        AddServerToActiveList(server);
                        ElaborateActiveServer();
                        if (!bIsActiveServer)
                        {
                            StartPollingToServer(server);
                            OnActiveServerChanged(server);
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref nActivatingServers);
                    }
                });
            }
            else
            {
                Interlocked.Decrement(ref nActivatingServers);
            }
        }

        public void ServerDeactivating(String server)
        {
            if (server == hostName || !KeepActiveServerActive || bStopping)
                return;

            if (!IsValidServer(server))
                return;

            bool deactiveServerChanged = deactivatingServer != server;
            deactivatingServer = server;
            var deactivatingServers = Interlocked.Increment(ref nDeactivatingServers);
            if (deactivatingServers == 1 || deactiveServerChanged)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        RemoveServerFromActiveList(server);
                        ElaborateActiveServer();
                        AddServerToActiveList(server);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref nDeactivatingServers);
                    }
                });
            }
            else
            {
                Interlocked.Decrement(ref nDeactivatingServers);
            }
        }

        public void ChangedTags(String server, List<ChangedTags> changedTags)
        {
            if (!IsValidServer(server))
                return;

            if (!IsActiveServer)
                OnChangedTags(changedTags);
        }

        public void ChangedAlarms(String server, ChangedAlarms changedAlarms)
        {
            if (!IsValidServer(server))
                return;

            if (!IsActiveServer)
                OnChangedAlarms(changedAlarms);
        }

        public void Ping(String server)
        {
            if (!IsActiveServer || !IsValidServer(server) || server.ToLower() == hostName)
                return;

            lock (lockObject)
            {
                server = server.ToLower();
                if (!listAliveServer.Contains(server))
                    listAliveServer.Add(server);
            }
        }

        public HistorySettings GetHistorySettings()
        {
            return new HistorySettings() 
            { 
                HistorianDefaultConnection = HistorianConnection, 
                EventDefaultConnection = EventConnection,
                HistorianCustomConnection = MapHistorianCustomConnection,
                DataLoggerConnection = MapDataLoggerConnection
            };
        }

        public uint WriteData(ChangedTags changedTags)
        {
            try
            {
                var ret = OnWritingValue(changedTags);
                return ret;
            }
            catch(Exception ex)
            {
                return Opc.Ua.StatusCodes.Bad;
            }
        }

        public uint UpdateAlarms(ChangedAlarms changedAlarms)
        {
            try
            {
                var ret = OnUpdatingAlarms(changedAlarms);
                return ret;
            }
            catch (Exception ex)
            {
                return Opc.Ua.StatusCodes.Bad;
            }
        }

        public MethodResult CallMethod(CallMethod callMethod)
        {
            try
            {
                return OnCallingMethod(callMethod);
            }
            catch
            {
                return new MethodResult { serviceResult = Opc.Ua.StatusCodes.Bad };
            }
        }

        public Dictionary<Opc.Ua.NodeId, LiveDataValue> GetAllLiveData()
        {
            try
            {
                return OnGettingAllLiveData();
            }
            catch(Exception ex)
            {
                OnErrorOccuredTags(ex);
                return null;
            }
        }

        public Dictionary<Opc.Ua.NodeId, WrappedAlarmStatusCollection> GetAllAlarmsStatus()
        {
            try
            {
                var alarms = OnGettingAllAlarmsStatus();
                if (alarms != null)
                {
                    var ret = new Dictionary<Opc.Ua.NodeId, WrappedAlarmStatusCollection>(alarms.Keys.Count);
                    foreach (var key in alarms.Keys)
                        ret.Add(key, new WrappedAlarmStatusCollection(alarms[key]));
                    return ret;
                }
            }
            catch (Exception ex)
            {
                OnErrorOccuredTags(ex);
            }

            return null;
        }
        #endregion
    }
}
