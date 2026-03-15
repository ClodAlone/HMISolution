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
using System.IO;
using DevExpress.Xpo.DB.Helpers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml;
using DriverBaseInterfaces;
using Utilities.Logger;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using DocumentManager.ComponentService;
using UFRecipeExecutionContext;
using UFRecipeExecuter;

namespace RecipeUAServer
{
    public partial class RecipeServer : UFUAServer
    {
        #region Declarations
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

        //protected bool enableLog { get; private set; }

#if !NET_CORE
        RecipeServiceCSM serverCMS;
#endif
        ComponentHost componentHost = new ComponentHost();
        PluginServices Plugins = new PluginServices();

        UFUAEditor.ComponentService.UFUAEditorManagerComponent ufuaEditorComponent = new UFUAEditor.ComponentService.UFUAEditorManagerComponent();
        ClientEditor.ComponentService.ClientEditorManagerComponent clientEditorManagerComponent = new ClientEditor.ComponentService.ClientEditorManagerComponent();
        UFProjectManager.ComponentService.UFProjectManagerComponent ufProjectManagerComponent = new UFProjectManager.ComponentService.UFProjectManagerComponent();
        UFRecipeEditor.ComponentService.RecipeEditorManagerComponent recipeManager = new UFRecipeEditor.ComponentService.RecipeEditorManagerComponent();
        UriResolver.ComponentService.UriResolverComponent uriRisolver = new UriResolver.ComponentService.UriResolverComponent();
        UFProjectManager.UFProjectDocument projectDocument;
        #endregion

        #region Constructors
        public RecipeServer()
            : base()
        {

            application.ApplicationType = ApplicationType.ClientAndServer;
#if !NET_CORE
            application.ConfigSectionName = "RecipeUAServer";
#else
            application.ConfigSectionName = "RecipeUAServer.UAServer";
#endif
        }
        #endregion

        internal void StopServerApplication()
        {
            StopService();

#if !NET_CORE
            if (serverCMS != null)
            {
                serverCMS.Dispose();
                serverCMS = null;
            }
#endif

            OnSystemEvent(null, Properties.Resources.RecipeServerStopped, EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, 
                null, null,(int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.RecipeService);
        }

        #region Override Methods
#if !NET_CORE
        protected override void OnStop()
        {
            StopServerApplication();
        }
#endif
#if !NET_CORE
        public override void StartServerApplication()
#else
        public override async Task StartServerApplication()
#endif
        {
            var uaserver = new RecipeUAServer() { ActiveConnectionString = ProjectConnectionString, RecipeServer = this };
            uaserver.BalloonEvent += UFUAServer_BalloonEvent;

            using (new StopWatcherLogger(Utilities.Properties.Resources.RecipeService, LoggerDestination.RecipeService,
                                                    Properties.Resources.StartingRecipes,
                                                    Properties.Resources.StartedRecipes))
            {
                var listStartupRecipesService = projectDocument.GetWholeDocumentLists(recipeManager);
                listStartupRecipesService.ForEach(recipe =>
                {
                    recipeManager.Execute(recipe, projectDocument, ExecutionMode.Synchro, this);
                });
            }

#if !NET_CORE
            application.Start(uaserver);
#else
            await application.Start(uaserver);
#endif
        }

#if !NET_CORE
        public override void StartService(string[] args)
#else
        public override async Task StartService(string[] args)
#endif
        {
            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);
            if (commandArgs.ArgPairs.ContainsKey("project"))
                projectDocument = UFProjectManager.UFProjectDocument.FromFile(commandArgs.ArgPairs["project"], ufProjectManagerComponent);
            if (projectDocument == null)
            {
                Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                    Properties.Resources.MissingProjectFile,
                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

#if !NET_STANDARD
                if (Environment.UserInteractive)
                {
                    System.Windows.MessageBox.Show(Properties.Resources.MissingProjectFile, ServiceName, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                }
#endif
                System.Environment.Exit(-10);
            }

#if !NET_CORE
            base.StartService(args);
#else
            await base.StartService(args);
#endif

            OnSystemEvent(null, Properties.Resources.RecipeServerStarted, EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType,
                null, null, (int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.RecipeService);
        }

        protected override string GetConnectionString(Utility.CommandArgs commandArgs)
        {
            var conn = base.GetConnectionString(commandArgs);
            if (String.IsNullOrEmpty(conn) && projectDocument != null)
                conn = UFRecipeSettings.Documents.RecipeUAServerDocument.GetConnectionString(projectDocument.ProjectFolder
#if !NET_STANDARD
                    , projectDocument.fileSystemProviderBase
#endif
                    );
            return conn;
        }

        protected override void SetDataStoreSchema(DevExpress.Xpo.Metadata.ReflectionDictionary dict)
        {
            dict.GetDataStoreSchema(typeof(UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration).Assembly, typeof(UFUAModel.UFUATag).Assembly);
        }

        protected override void PreInitializeServerApplication()
        {
            using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
            {
                var configurations = (from tag in new XPQuery<UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration>(ufw).AsParallel() select tag).ToList();
                if (configurations.Count == 0)
                    configurations.Add(new UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration(ufw));

                if (configurations.Count > 0)
                {
                    //enableLog = configurations[0].EnableLog;
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
#if NET_STANDARD
                            if (ba.Transport != Opc.Ua.Utils.UriSchemeOpcTcp && ba.Transport != Opc.Ua.Utils.UriSchemeHttps)
                            {
                                Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                    String.Format(UFUAServerBase.Properties.Resources.NotSupportedProtocol, ba.Transport),
                                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.RecipeService);
                                continue;
                            }
#endif
                            if (ba.Enabled)
                            {
                                if (ba.Transport != Opc.Ua.Utils.UriSchemeOpcTcp)
                                {
                                    if (allocatedTransports.Contains(ba.Transport))
                                    {
                                        Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                            String.Format(UFUAServerBase.Properties.Resources.DuplicatedProtocol, ba.Transport),
                                            System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Server);
                                        continue;
                                    }
                                    else
                                        allocatedTransports.Add(ba.Transport);
                                }

                                application.ApplicationConfiguration.ServerConfiguration.BaseAddresses.Add(Opc.Ua.Utils.ReplaceLocalhost(ba.Path));
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
                            serverCMS = new RecipeServiceCSM(configurations[0].ConfigurationId.ToString(), this);
                            serverCMS.HostServer(useDiscovery);
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (serverCMS != null)
                                Opc.Ua.Utils.SilentDispose(serverCMS);

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
                                Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                                        str,
                                                        System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.RecipeService);

                                var thisserver = application.Server as UAServer;
                                if (thisserver != null)
                                    thisserver.OnBalloonEvent(str, System.Windows.Forms.ToolTipIcon.Warning);

                                if ((retries % openServiceHostsMaxRetries) == 0)
                                {
                                    if (ex is System.ServiceModel.AddressAlreadyInUseException)
                                    {
                                        Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                                Properties.Resources.ProjectAlreadyRunning,
                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.RecipeService);
                                    }
                                    else
                                    {
                                        var communicationErrorMessage = String.Format(Properties.Resources.CommunciationErrorOnStartingService.Replace("'newline'", Environment.NewLine), ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                                        Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                                                communicationErrorMessage,
                                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.RecipeService);
                                    }
                                }
                            }
                            else
                                throw;
                        }
                    }

                    if (!useDiscovery)
                    {
                        Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                                                Properties.Resources.UnableToUseDiscoveryBehaviour,
                                                System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.RecipeService);
                    }
#endif
                }
            }

            componentHost.Components.Add(ufProjectManagerComponent);
            componentHost.Components.Add(uriRisolver);
            componentHost.Components.Add(ufuaEditorComponent);
            componentHost.Components.Add(clientEditorManagerComponent);
            componentHost.Components.Add(recipeManager);

            uriRisolver.RegisterDocumentManager(recipeManager, recipeManager.TypeScheme);

            projectDocument.SetCurrentLogFileName();
            projectDocument.UpdateSessionSettings();

            string baseDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataSinks");
            Plugins.FindPlugins(baseDirectory);

            foreach (var pluginOn in Plugins.AvailablePlugins)
            {
                try
                {
                    ((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance.Initialize();
                }
                catch (Exception ex)
                {
                    var pluginName = System.IO.Path.GetFileNameWithoutExtension(((UFInterfaces.Types.AvailablePlugin)pluginOn).AssemblyPath);
                    Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService,
                        Properties.Resources.FailedToInitializePlugin,
                        System.Diagnostics.EventLogEntryType.Error, LoggerDestination.RecipeService, pluginName, ex.Message);
                }
            }

            OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
            OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();
            OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();
#if !DEBUG
            MSZ.MSZView.KeyChangeEvent += (ob, ev) =>
            {
                OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();
            };
#endif
        }

        #endregion

#region Methods
        internal UFRecipeExecuter.UFRecipeExecuter GetRecipeExecuter(Uri uri)
        {
            if (projectDocument == null)
                return null;

            var uriAbsolute = projectDocument.MakeAbosoluteUri(uri, recipeManager);
            return recipeManager.GetRecipeExecuter(uriAbsolute, projectDocument);
        }

        internal String GetRecipeFolderPath(Uri uri)
        {
            if (projectDocument == null)
                return null;

            var absolutePath = projectDocument.MakeAbosoluteUri(uri, recipeManager).GetPathString().Replace('/', '\\');
            return System.IO.Path.GetDirectoryName(absolutePath.Replace(String.Format("{0}\\{1}\\", projectDocument.ProjectFolder.Replace('/', '\\'), recipeManager.TypeLabel), String.Empty).Replace('\\', Path.DirectorySeparatorChar));
        }

        internal List<Uri> GetWholeRecipeLists()
        {
            if (projectDocument == null)
                return null;

            var relativeUris = new List<Uri>();
            var absoluteUris = projectDocument.GetWholeDocumentLists(recipeManager);
            absoluteUris.ForEach(uri =>
            {
                relativeUris.Add(projectDocument.MakeRelativeUri(uri, recipeManager));
            });

            return relativeUris;
        }

        internal void Execute(Uri uri, ExecutionMode mode, RecipeExecutionContext args)
        {
            recipeManager.Execute(uri, projectDocument, mode, args);
        }

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
                    e.sourceNode = new NodeId(LoggerDestination.RecipeService.ToString());

                e.sourceName = Utilities.Properties.Resources.RecipeService;
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
                Logger.WriteToEventLog(Utilities.Properties.Resources.RecipeService, errMessage, (logtype > -1 ? (System.Diagnostics.EventLogEntryType)logtype : System.Diagnostics.EventLogEntryType.Error),
                    (logdestination > -1 ? (LoggerDestination)logdestination : LoggerDestination.RecipeService));
        }
#endregion
    }
}
