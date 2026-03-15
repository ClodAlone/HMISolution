using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Opc.Ua;
using Opc.Ua.Configuration;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Utilities;
using UFUAServerBase;
#if !NET_CORE
using UFUAServerBase.ServerCSM;
#endif
using System.Windows;
using System.Xml;
using System.IO;
using DevExpress.Xpo.DB.Helpers;
using DriverBaseInterfaces;
using Utilities.Logger;
using System.Threading.Tasks;
#if !NET_CORE
using ADServer.ServerCSM;
using UFUAServerCMS;
#endif
namespace ADServer
{
    public partial class ADServer : UFUAServer
    {
        #region Declaration
#if !NET_CORE
        ADServerCSM serverCSM;
#endif
        protected override string logPath
        {
            get
            {
#if !NET_STANDARD
                return Properties.Settings.Default.LogPath;
#else
                return Properties.Settings.Default.NetCoreLogPath;
#endif
            }
        }

        String serverConfigurationId;
#endregion


        private static readonly String DataSourceHeader = "data source";

        public ADServer()
            : base()
        {

            application.ApplicationType = ApplicationType.ClientAndServer;
#if !NET_CORE
            application.ConfigSectionName = "ADServer";
#else
            application.ConfigSectionName = "ADServer.ADUAServer";
#endif

        }

        #region Override Methods
#if !NET_CORE
        public override void StartServerApplication()
#else
        public override async Task StartServerApplication()
#endif
        {

            var aduaserver = new ADUAServer() { ActiveConnectionString = ProjectConnectionString, OServer = this };
            aduaserver.BalloonEvent += UFUAServer_BalloonEvent;
#if !NET_CORE
            application.Start(aduaserver);
#else
            await application.Start(aduaserver);
#endif
        }

#if !NET_CORE
        public override void StartService(string[] args)
#else
        public override async Task StartService(string[] args)
#endif
        {
            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);

            if (commandArgs.ArgPairs.ContainsKey("SrvrConfigId"))
            {
                serverConfigurationId = commandArgs.ArgPairs["SrvrConfigId"];
            }
#if !NET_CORE
            base.StartService(args);
#else
            await base.StartService(args);
#endif

            OnSystemEvent(ObjectIds.Server, Properties.Resources.ADServiceStarted, EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, null, null, (int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.AlarmDispatcher);
        }
#if !NET_CORE
        protected override void OnStop()
        {
            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        Properties.Resources.ServiceStopping,
                                        System.Diagnostics.EventLogEntryType.Information, LoggerDestination.AlarmDispatcher);

            base.StopService();

            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    Properties.Resources.ServiceStopped,
                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.AlarmDispatcher);

            OnSystemEvent(ObjectIds.Server, Properties.Resources.ADServiceStopped, EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType);
        }
#endif

        protected override void SetDataStoreSchema(DevExpress.Xpo.Metadata.ReflectionDictionary dict)
        {
            dict.GetDataStoreSchema(typeof(ADModel.ADGeneralSettings).Assembly, typeof(UFUAModel.UFUATag).Assembly);
        }

        protected override void PreInitializeServerApplication()
        {
            using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
            {
                var configurations = (from tag in new XPQuery<ADModel.ADGeneralSettings>(ufw).AsParallel() select tag).ToList();
                if (configurations.Count == 0)
                    configurations.Add(new ADModel.ADGeneralSettings(ufw));

                if (configurations.Count > 0)
                {
                    if (configurations[0].MaxSessionTimeout > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxSessionTimeout = configurations[0].MaxSessionTimeout;
                    if (configurations[0].MinSessionTimeout > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MinSessionTimeout = configurations[0].MinSessionTimeout;

                    application.ApplicationConfiguration.ServerConfiguration.DiagnosticsEnabled = configurations[0].DiagnosticEnabled;
                    if (configurations[0].MaxSessionCount > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxSessionCount = configurations[0].MaxSessionCount;
                    if (configurations[0].MaxBrowseContinuationPoints > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxBrowseContinuationPoints = configurations[0].MaxBrowseContinuationPoints;
                    if (configurations[0].MaxHistoryContinuationPoints > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxHistoryContinuationPoints = configurations[0].MaxHistoryContinuationPoints;
                    if (configurations[0].MaxRequestAge > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxRequestAge = configurations[0].MaxRequestAge;
                    if (configurations[0].MinPublishingInterval > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MinPublishingInterval = configurations[0].MinPublishingInterval;
                    if (configurations[0].MaxPublishingInterval > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxPublishingInterval = configurations[0].MaxPublishingInterval;
                    if (configurations[0].PublishingResolution > 0)
                        application.ApplicationConfiguration.ServerConfiguration.PublishingResolution = configurations[0].PublishingResolution;
                    if (configurations[0].MaxSubscriptionLifetime > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxSubscriptionLifetime = configurations[0].MaxSubscriptionLifetime;
                    if (configurations[0].MaxMessageQueueSize > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxMessageQueueSize = configurations[0].MaxMessageQueueSize;
                    if (configurations[0].MaxNotificationQueueSize > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxNotificationQueueSize = configurations[0].MaxNotificationQueueSize;
                    if (configurations[0].MaxNotificationsPerPublish > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MaxNotificationsPerPublish = configurations[0].MaxNotificationsPerPublish;
                    if (configurations[0].MinMetadataSamplingInterval > 0)
                        application.ApplicationConfiguration.ServerConfiguration.MinMetadataSamplingInterval = configurations[0].MinMetadataSamplingInterval;

                    if (configurations[0].BaseAddresses.Count == 0 && application.ApplicationConfiguration.ServerConfiguration.BaseAddresses.Count == 0)
                        configurations[0].EnsureDefaultBaseAddresses(ufw);

                    if (configurations[0].BaseAddresses.Count > 0)
                    {
                        application.ApplicationConfiguration.ServerConfiguration.BaseAddresses.Clear();

                        var allocatedTransports = new List<String>();
                        foreach (var ba in configurations[0].BaseAddresses)
                        {
                            if (ba.Enabled)
                            {
                                if (ba.Transport != Opc.Ua.Utils.UriSchemeOpcTcp)
                                {
                                    if (allocatedTransports.Contains(ba.Transport))
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(UFUAServerBase.Properties.Resources.DuplicatedProtocol, ba.Transport),
                                            System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                                        continue;
                                    }
                                    else
                                        allocatedTransports.Add(ba.Transport);
                                }

                                application.ApplicationConfiguration.ServerConfiguration.BaseAddresses.Add(Utils.ReplaceLocalhost(ba.Path));
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(configurations[0].ApplicationName))
                    {
                        application.ApplicationName = configurations[0].ApplicationName;
                        if (!application.ApplicationConfiguration.ApplicationUri.Contains(String.Format(":{0}", configurations[0].ApplicationName)))
                        {
                            var builder = new StringBuilder(application.ApplicationConfiguration.ApplicationUri);
                            builder.AppendFormat(":{0}", configurations[0].ApplicationName);
                            application.ApplicationConfiguration.ApplicationUri = builder.ToString();
                        }

                        if (application.ApplicationConfiguration.TraceConfiguration != null)
                        {
                            var filename = Path.GetFileName(application.ApplicationConfiguration.TraceConfiguration.OutputFilePath);
                            application.ApplicationConfiguration.TraceConfiguration.OutputFilePath =
                                application.ApplicationConfiguration.TraceConfiguration.OutputFilePath.Replace(filename, String.Format("{0}.log", configurations[0].ApplicationName));
                        }
                    }

                    application.ApplicationConfiguration.ProductUri = configurations[0].ProductUri;
#if !NET_CORE
                    bool useDiscovery = true;
                    int delay = 0;
                    int retries = 0;
                    while (true)
                    {
                        if (serverStopping.WaitOne(delay))
                            return;

                        try
                        {
                            serverCSM = new ADServerCSM(configurations[0].ConfigurationId.ToString(), this);
                            serverCSM.HostServer(useDiscovery);
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (serverCSM != null)
                                Opc.Ua.Utils.SilentDispose(serverCSM);

                            if (ex is System.ServiceModel.AddressAlreadyInUseException ||
                                ex is System.Net.Sockets.SocketException ||
                                ex is System.ServiceModel.CommunicationException)
                            {
                                if (useDiscovery && !(ex is System.ServiceModel.AddressAlreadyInUseException))
                                    useDiscovery = false;

                                if (++retries > openServiceHostsMaxRetries)
                                    retries = 1;

                                delay = Math.Max(retries * 1000, delay);
                                var str = String.Format(Properties.Resources.RetryOpenSocketWarningMessage, retries, openServiceHostsMaxRetries, delay);
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                        str,
                                                        System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.AlarmDispatcher);

                                var thisserver = application.Server as UAServer;
                                if (thisserver != null)
                                    thisserver.OnBalloonEvent(str, System.Windows.Forms.ToolTipIcon.Warning);

                                if ((retries % openServiceHostsMaxRetries) == 0)
                                {
                                    if (ex is System.ServiceModel.AddressAlreadyInUseException)
                                    {
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                Properties.Resources.ProjectAlreadyRunning,
                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.AlarmDispatcher);
                                    }
                                    else
                                    {
                                        var communicationErrorMessage = String.Format(Properties.Resources.CommunciationErrorOnStartingService.Replace("'newline'", Environment.NewLine), ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                                communicationErrorMessage,
                                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.AlarmDispatcher);
                                    }
                                }
                            }
                            else
                                throw;
                        }
                    }

                    if (!useDiscovery)
                    {
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                Properties.Resources.UnableToUseDiscoveryBehaviour,
                                                System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.AlarmDispatcher);
                    }
#endif
                }
            }
        }

        #endregion

#region Properties
#if !NET_CORE
        UFUAServerCSMHelpers serverCMSHelperSync;
        [Browsable(false)]
        internal UFUAServerCSMHelpers ServerCMSHelperSync
        {
            get
            {
                if (string.IsNullOrEmpty(serverConfigurationId))
                    return null;
                if (serverCMSHelperSync != null &&
                    !serverCMSHelperSync.IsServerStartedManually &&
                    serverCMSHelperSync.InstanceId != serverConfigurationId)
                {
                    serverCMSHelperSync.Dispose();
                    serverCMSHelperSync = null;
                }

                if (serverCMSHelperSync == null)
                { 
                    serverCMSHelperSync = new UFUAServerCSMHelpers(serverConfigurationId);
                    serverCMSHelperSync.ServerActivate += (o, e) => 
                    {
                        OnSystemEvent(ObjectIds.Server,
                               String.Format(Properties.Resources.ADServerActive, System.Net.Dns.GetHostName()),
                               EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, 
                               null, null, (int)System.Diagnostics.EventLogEntryType.Information, 
                               (int)LoggerDestination.AlarmDispatcher);
                    };
                    serverCMSHelperSync.ServerDeactivate += (o, e) =>
                    {
                        OnSystemEvent(ObjectIds.Server,
                            String.Format(Properties.Resources.ADServerNotActive, System.Net.Dns.GetHostName()),
                            EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, 
                            null, null, (int)System.Diagnostics.EventLogEntryType.Information, 
                            (int)LoggerDestination.AlarmDispatcher);
                    };
                }

                return serverCMSHelperSync;
            }
        }
#endif
#endregion

        #region Methods

        public event EventHandler<SystemEventArgs> SystemEvent;
        public void OnSystemEvent(object nodeId, String errMessage, EventSeverity severity, NodeId evtype, String details = null, String state = null,
            int logtype = -1, int logdestination = -1)
        {
            var temp = SystemEvent;
            if (temp != null)
            {
                SystemEventArgs e = new SystemEventArgs();
                if (nodeId != null)
                    e.sourceNode = (NodeId)nodeId;
                else
                    e.sourceNode = new NodeId(Properties.Resources.LoggerSource);
                
                e.sourceName = Properties.Resources.LoggerSource;
                e.EventName = errMessage;
                e.severity = severity;
                e.time = DateTime.UtcNow;
                e.eventtype = evtype;
                if (details != null)
                    e.details = details;
                if (state != null)
                    e.state = state;
                e.logtype = logtype;
                e.logdestination = logdestination;

                temp(this, e);
            }
            else
                Logger.WriteToEventLog(Properties.Resources.LoggerSource, errMessage, (logtype > -1 ? (System.Diagnostics.EventLogEntryType)logtype : System.Diagnostics.EventLogEntryType.Error),
                    (logdestination > -1 ? (LoggerDestination)logdestination : LoggerDestination.AlarmDispatcher));
        }

        #endregion
    }
}
