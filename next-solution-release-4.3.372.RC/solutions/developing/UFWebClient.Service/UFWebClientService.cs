using DocumentManager.ComponentService;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using Utilities;
using Utilities.Logger;

namespace UFWebClient.Service
{
    public partial class UFWebClientService : ServiceBase
    {
        #region Members
        public IDisposable SignalR { get; set; }

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.Server);

        object lockObject = new object();
        Thread serverThread = null;
        ManualResetEvent serverStopping;

        public static Dictionary<String, String> historiansettings = new Dictionary<String, String>();
        public static Dictionary<String, String> eventsettings = new Dictionary<String, String>();
        public static Dictionary<String, String> serverentitysettings = new Dictionary<String, String>();
        public static Dictionary<String, String> schedulersettings = new Dictionary<String, String>();
        static public String defaultUri { get; protected set; }
        static public String ClientSessionName { get; protected set; }
        static public int RefreshPollingTime { get; protected set; }
        static public int RefreshPollingTimeCount { get; protected set; }
        static public int DelayBroadcaster { get; protected set; }
        static public int SessionTimeout { get; protected set; }
        static public int ConcurrentRenderingPipeline { get; protected set; }
        static public String Theme { get; protected set; }
        static public bool LowResolution { get; protected set; }
        static public bool DisablePopupScreen { get; protected set; }
        static public bool DisableAlertOnCommandExecution { get; protected set; }
        static public bool DisableStaticOptimization { get; protected set; }
        static public bool OnlySecureConnection { get; protected set; }
        static public String ActiveSessionCountVariableName { get; protected set; }

        static public bool AllowCredentialsProviderMapping { get; protected set; }

        static internal int DemoModeInterval = 600;
        static internal int DemoModeCountDown = 5;
        static internal int DemoModeClickCounter = 10;
        static internal bool bStoppingService;

        static public Uri currentPage { get; set; }
        static public UFProjectManager.UFProjectDocument projectDocument { get; protected set; }

        static ComponentHost componentHost = new ComponentHost();
        static PluginServices Plugins = new PluginServices();

        static Dictionary<String, ScreenSinkServiceSession> mapCurrentSessions = new Dictionary<String, ScreenSinkServiceSession>();
        static public int AddSession(String id, ScreenSinkServiceSession session)
        {
            RemoveSession(id);
            lock (mapCurrentSessions)
            {
                mapCurrentSessions.Add(id, session);
                log.Info(String.Format(Properties.Resources.NewSession, id, mapCurrentSessions.Count));
                return mapCurrentSessions.Count;
            }
        }
        static public int RemoveSession(String id)
        {
            ScreenSinkServiceSession session = null;
            lock (mapCurrentSessions)
            {
                if (mapCurrentSessions.ContainsKey(id))
                {
                    session = mapCurrentSessions[id];
                    mapCurrentSessions.Remove(id);
                    log.Info(String.Format(Properties.Resources.SessionEnd, id, mapCurrentSessions.Count));
                }

                return mapCurrentSessions.Count;
            }

            if (session != null)
                session.Dispose();
        }
        static public ScreenSinkServiceSession GetSession(String id)
        {
            lock (mapCurrentSessions)
            {
                if (mapCurrentSessions.ContainsKey(id))
                    return mapCurrentSessions[id];
                return null;
            }
        }
        static internal void RemoveAllSessions()
        {
            var sessions = new List<ScreenSinkServiceSession>();
            lock (mapCurrentSessions)
            {
                sessions = mapCurrentSessions.Values.ToList();
                mapCurrentSessions.Clear();
            }

            // first release all license sessions.
            foreach (var session in sessions)
                session.ScreenSink.ReleaseLicense();

            // next dispose all sessions.
            foreach (var session in sessions)
                session.Dispose();
        }

        static ScreenManager.ComponentService.ScreenManagerComponent screenComponent = new ScreenManager.ComponentService.ScreenManagerComponent();
        static public ScreenManager.ComponentService.ScreenManagerComponent ScreenComponent
        {
            get
            {
                return screenComponent;
            }
        }

        static UFRecipeEditor.ComponentService.RecipeEditorManagerComponent recipeComponent = new UFRecipeEditor.ComponentService.RecipeEditorManagerComponent();
        static public UFRecipeEditor.ComponentService.RecipeEditorManagerComponent RecipeComponent
        {
            get
            {
                return recipeComponent;
            }
        }

        static ReportManager.ComponentService.ReportManagerComponent reportComponent = new ReportManager.ComponentService.ReportManagerComponent();
        static public ReportManager.ComponentService.ReportManagerComponent ReportComponent
        {
            get
            {
                return reportComponent;
            }
        }

        static UFUAEditor.ComponentService.UFUAEditorManagerComponent ufuaEditorComponent = new UFUAEditor.ComponentService.UFUAEditorManagerComponent();
        static public UFUAEditor.ComponentService.UFUAEditorManagerComponent UFUAEditorComponent
        {
            get
            {
                return ufuaEditorComponent;
            }
        }

        static UFUserEditor.ComponentService.UFUserEditorManagerComponent ufUserEditorComponent = new UFUserEditor.ComponentService.UFUserEditorManagerComponent();
        static public UFUserEditor.ComponentService.UFUserEditorManagerComponent UFUserEditorComponent
        {
            get
            {
                return ufUserEditorComponent;
            }
        }

        static UFProjectManager.ComponentService.UFProjectManagerComponent ufProjectManagerComponent = new UFProjectManager.ComponentService.UFProjectManagerComponent();
        static public UFProjectManager.ComponentService.UFProjectManagerComponent UFProjectManagerComponent
        {
            get
            {
                return ufProjectManagerComponent;
            }
        }

        static ClientEditor.ComponentService.ClientEditorManagerComponent clientEditorManagerComponent = new ClientEditor.ComponentService.ClientEditorManagerComponent();
        static public ClientEditor.ComponentService.ClientEditorManagerComponent ClientEditorManagerComponent
        {
            get
            {
                return clientEditorManagerComponent;
            }
        }

        static StringManager.ComponentService.StringEditorManagerComponent stringEditorComponent = new StringManager.ComponentService.StringEditorManagerComponent();
        static public StringManager.ComponentService.StringEditorManagerComponent StringEditorComponent
        {
            get
            {
                return stringEditorComponent;
            }
        }

        static UnitConverterManager.ComponentService.UnitConverterEditorManagerComponent unitConverterEditorComponent = new UnitConverterManager.ComponentService.UnitConverterEditorManagerComponent();
        static public UnitConverterManager.ComponentService.UnitConverterEditorManagerComponent UnitConverterEditorComponent
        {
            get
            {
                return unitConverterEditorComponent;
            }
        }

        static UFCrossReferenceEditor.ComponentService.CrossReferenceEditorManagerComponent crossReferenceEditorManagerComponent = new UFCrossReferenceEditor.ComponentService.CrossReferenceEditorManagerComponent();
        static public UFCrossReferenceEditor.ComponentService.CrossReferenceEditorManagerComponent CrossReferenceEditorManagerComponent
        {
            get
            {
                return crossReferenceEditorManagerComponent;
            }
        }

        static OPCUAClientStatus.ComponentService.OPCUABrowserComponent oPCUABrowserComponent = new OPCUAClientStatus.ComponentService.OPCUABrowserComponent();
        static public OPCUAClientStatus.ComponentService.OPCUABrowserComponent OPCUABrowserComponent
        {
            get
            {
                return oPCUABrowserComponent;
            }
        }

        static ScriptManager.ComponentService.ScriptManagerComponent scriptManagerComponent = new ScriptManager.ComponentService.ScriptManagerComponent();
        static public ScriptManager.ComponentService.ScriptManagerComponent ScriptManagerComponent
        {
            get
            {
                return scriptManagerComponent;
            }
        }

        static MSEditor.ComponentService.SchedulerEditorManagerComponent schedulerEditorComponent = new MSEditor.ComponentService.SchedulerEditorManagerComponent();
        static public MSEditor.ComponentService.SchedulerEditorManagerComponent SchedulerEditorComponent
        {
            get
            {
                return schedulerEditorComponent;
            }
        }

        #endregion

        #region Constructors

        public UFWebClientService()
        {
            InitializeComponent();
        }

        #endregion

        #region Static Methods
        static void InitProjectRecipes()
        {
            RecipeComponent.Execute(null, projectDocument, DocumentManager.ComponentService.ExecutionMode.Normal, projectDocument.ConfigurationId);
        }

        static void ChangeConfigurationFile(string newConfigFilePath)
        {
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", newConfigFilePath);

            typeof(System.Configuration.ConfigurationManager)
                .GetField("s_initState", System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static)
                .SetValue(null, 0);

            typeof(System.Configuration.ConfigurationManager)
                .GetField("s_configSystem", System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static)
                .SetValue(null, null);

            typeof(System.Configuration.ConfigurationManager)
                .Assembly.GetTypes()
                .Where(x => x.FullName ==
                "System.Configuration.ClientConfigPaths")
                .First()
                .GetField("s_current", System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static)
                .SetValue(null, null);
        }

        #endregion

        #region Overrides

        protected override void OnStart(string[] args)
        {
            lock (lockObject)
            {
                if (serverThread == null)
                {
                    serverStopping = new ManualResetEvent(false);
                    serverThread = new Thread(() =>
                    {
                        if (args.Length == 0)
                            args = Environment.GetCommandLineArgs();

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        Properties.Resources.ServiceStarting,
                                        System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

                        StartService(args);

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                Properties.Resources.ServiceStarted,
                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

                        IsRunningAsService = true;

                        if (serverStopping.WaitOne())
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            Properties.Resources.ServiceStopping,
                                            System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

                            StopService();

                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    Properties.Resources.ServiceStopped,
                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);

                            IsRunningAsService = false;
                        }
                    });
                    serverThread.Name = ServiceName;
                    serverThread.IsBackground = true;
                }
                serverThread.Start();
            }
        }

        protected override void OnStop()
        {
            Thread thread = null;
            lock (lockObject)
            {
                thread = serverThread;
                if (serverStopping != null)
                    serverStopping.Set();
            }

            if (thread != null)
            {
                while (serverThread.IsAlive && !serverThread.Join(2000))
                {
                    RequestAdditionalTime(5000);
                }
            }

            lock (lockObject)
            {
                serverThread = null;
                if (serverStopping != null)
                {
                    serverStopping.Dispose();
                    serverStopping = null;
                }
            }
        }

        #endregion

        #region Properties

        private bool _IsRunningAsService;
        public bool IsRunningAsService
        {
            get { return _IsRunningAsService; }
            internal set
            {
                _IsRunningAsService = value;
            }
        }

        public String Title
        {
            get
            {
                return Properties.Resources.Title;
            }
        }

        public bool CommunicationStatus
        {
            get
            {
                return true;
            }
        }

        public String StatusText
        {
            get
            {
                return String.Empty;
            }
        }

        String _BalloonMessage = String.Empty;
        public String BalloonMessage
        {
            get { return _BalloonMessage; }
            set { _BalloonMessage = value; }
        }
        
        System.Windows.Forms.ToolTipIcon _BalloonIcon = System.Windows.Forms.ToolTipIcon.None;
        public System.Windows.Forms.ToolTipIcon BalloonIcon
        {
            get { return _BalloonIcon; }
            set { _BalloonIcon = value; }
        }
        #endregion

        #region Events

        public event EventHandler StartingService;
        #region OnStartingService
        /// <summary>
        /// Triggers the StoppingService event.
        /// </summary>
        public virtual void OnStartingService(EventArgs ea)
        {
            var t = StartingService;
            if (t != null)
                t(this, ea);
        }
        #endregion

        public event EventHandler StoppingService;
        #region OnStoppingService
        /// <summary>
        /// Triggers the StoppingService event.
        /// </summary>
        public virtual void OnStoppingService(EventArgs ea)
        {
            var t = StoppingService;
            if (t != null)
                t(this, ea);
        }

        #endregion
        #endregion

        #region Virtual Methods

        public virtual void StartService(string[] args)
        {
            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                            Properties.Resources.ServiceStarting,
                                            Properties.Resources.ServiceStarted))
            {
                OnStartingService(new EventArgs());

                Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);

                DevExpress.Xpf.Core.DXGridDataController.DisableThreadingProblemsDetection = true;

                string configFilePath = null;
                if (commandArgs.ArgPairs.ContainsKey("ConfigFilePath"))
                    configFilePath = commandArgs.ArgPairs["ConfigFilePath"];

                KeyValueConfigurationCollection appSettings = null;
                if (!string.IsNullOrEmpty(configFilePath) && System.IO.File.Exists(configFilePath))
                {
                    try
                    {
                        ChangeConfigurationFile(configFilePath);
                    }
                    catch
                    { }

                    try
                    {
                        ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                        configMap.ExeConfigFilename = configFilePath;
                        appSettings = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None).AppSettings.Settings;
                    }
                    catch
                    {
                        appSettings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;
                    }
                }
                else
                    appSettings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;

                if (appSettings["ClientSessionName"] != null)
                    ClientSessionName = appSettings["ClientSessionName"].Value;
                if (appSettings["Theme"] != null)
                    Theme = appSettings["Theme"].Value;
                if (!String.IsNullOrEmpty(appSettings["ActiveSessionCountVariableName"]?.Value))
                    ActiveSessionCountVariableName = appSettings["ActiveSessionCountVariableName"].Value;
                RefreshPollingTime = 1000;
                var refreshPollingTime = appSettings["RefreshPollingTime"];
                if (refreshPollingTime != null)
                {
                    try
                    {
                        RefreshPollingTime = Convert.ToInt32(refreshPollingTime.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var refreshPollingTimeCount = appSettings["RefreshPollingTimeCount"];
                if (refreshPollingTimeCount != null)
                {
                    try
                    {
                        RefreshPollingTimeCount = Convert.ToInt32(refreshPollingTimeCount.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                DelayBroadcaster = 100;
                var delayBroadcaster = appSettings["DelayBroadcaster"];
                if (delayBroadcaster != null)
                {
                    try
                    {
                        DelayBroadcaster = Convert.ToInt32(delayBroadcaster.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var sessionTimeout = appSettings["SessionTimeout"];
                if (sessionTimeout != null)
                {
                    try
                    {
                        SessionTimeout = Convert.ToInt32(sessionTimeout.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var concurrentRenderingPipeline = appSettings["ConcurrentRenderingPipeline"];
                if (concurrentRenderingPipeline != null)
                {
                    try
                    {
                        ConcurrentRenderingPipeline = Convert.ToInt32(concurrentRenderingPipeline.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var softwareRendering = appSettings["SoftwareRendering"];
                if (softwareRendering != null)
                {
                    try
                    {
                        bool bSet = Convert.ToBoolean(softwareRendering.Value);
                        if (bSet)
                            ScreenManager.ComponentService.ScreenManagerComponent.EnableSoftwareRendering();
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var lowResolution = appSettings["LowResolution"];
                if (lowResolution != null)
                {
                    try
                    {
                        LowResolution = Convert.ToBoolean(lowResolution.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var disablePopupScreen = appSettings["DisablePopupScreen"];
                if (disablePopupScreen != null)
                {
                    try
                    {
                        DisablePopupScreen = Convert.ToBoolean(disablePopupScreen.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var disableAlertOnCommandExecution = appSettings["DisableAlertOnCommandExecution"];
                if (disableAlertOnCommandExecution != null)
                {
                    try
                    {
                        DisableAlertOnCommandExecution = Convert.ToBoolean(disableAlertOnCommandExecution.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var disableStaticOptimization = appSettings["DisableStaticOptimization"];
                if (disableStaticOptimization != null)
                {
                    try
                    {
                        DisableStaticOptimization = Convert.ToBoolean(disableStaticOptimization.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                var onlySecureConnection = appSettings["OnlySecureConnection"];
                if (onlySecureConnection != null)
                {
                    try
                    {
                        OnlySecureConnection = Convert.ToBoolean(onlySecureConnection.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                var allowCredentialsProviderMapping = appSettings["AllowCredentialsProviderMapping"];
                if (allowCredentialsProviderMapping != null)
                {
                    try
                    {
                        AllowCredentialsProviderMapping = Convert.ToBoolean(allowCredentialsProviderMapping.Value);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (commandArgs.ArgPairs.ContainsKey("ProjectUri"))
                    defaultUri = commandArgs.ArgPairs["ProjectUri"];
                if (String.IsNullOrEmpty(defaultUri) && appSettings["ProjectUri"] != null)
                    defaultUri = appSettings["ProjectUri"].Value;
                projectDocument = UFProjectManager.UFProjectDocument.FromFile(defaultUri, UFProjectManagerComponent);
                if (projectDocument == null)
                    throw new ArgumentNullException("Missing project to load or project cannot be loaded!");

                componentHost.Components.Add(UFProjectManagerComponent);
                componentHost.Components.Add(ClientEditorManagerComponent);
                componentHost.Components.Add(UFUAEditorComponent);
                componentHost.Components.Add(ScreenComponent);
                componentHost.Components.Add(RecipeComponent);
                componentHost.Components.Add(ReportComponent);
                componentHost.Components.Add(UFUserEditorComponent);
                componentHost.Components.Add(StringEditorComponent);
                componentHost.Components.Add(UnitConverterEditorComponent);
                componentHost.Components.Add(CrossReferenceEditorManagerComponent);
                componentHost.Components.Add(OPCUABrowserComponent);
                componentHost.Components.Add(ScriptManagerComponent);

                projectDocument.SetCurrentLogFileName();
                UpdateProjectSettings(projectDocument);

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var index = baseDirectory.LastIndexOf('\\', baseDirectory.Length - 2);
                if (index != -1)
                    baseDirectory = baseDirectory.Substring(0, index);

                Plugins.FindPlugins(String.Format("{0}\\DataSinks\\", baseDirectory));

                foreach (var pluginOn in Plugins.AvailablePlugins)
                {
                    try
                    {
                        ((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance.Initialize();
                    }
                    catch (Exception ex)
                    {
                        log.Error(Properties.Resources.FailedToInitializePlugin, ex);
                    }
                }

                //OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
                //OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();

                ScreenComponent.LoadRequiredAssemblies();
                StringEditorComponent.LoadRuntimeStrings(projectDocument);
                if (projectDocument.GetWholeDocumentLists((IDocumentManager)RecipeComponent).Count > 0)
                    RecipeComponent.PreSubscribeServerSession(projectDocument);

                InitProjectRecipes();

                currentPage = projectDocument.GetStartupScreen(ScreenComponent);
                if (currentPage == null)
                    throw new ArgumentNullException("Missing default uri to load !");

                projectDocument.PreSubscribeServerSession(UFUAEditorComponent);

                var serverURI = "http://+:8089";
                if (appSettings["ServerURI"] != null)
                    serverURI = appSettings["ServerURI"].Value;

                var options = new StartOptions();
                try
                {
                    var uris = serverURI.Split(';');
                    foreach (var uri in uris)
                    {
                        if (!OnlySecureConnection || uri.Trim().StartsWith("https"))
                            options.Urls.Add(uri.Trim());
                    }
                    SignalR = WebApp.Start(options);
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    String.Format(Properties.Resources.ServerRunningAt, String.Join(";", options.Urls)),
                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Server);
                }
                catch (TargetInvocationException ex)
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            String.Format(Properties.Resources.ServiceNotRunning, String.Join(";", options.Urls), ex.InnerException.Message),
                                            System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
                    return;
                }
            }
        }

        static void UpdateProjectSettings(UFProjectManager.UFProjectDocument doc, UFProjectManager.UFProjectDocument parent = null)
        {
            doc.Parent = parent;
            doc.UpdateSessionSettings();

            historiansettings[doc.Title] = UFUAEditorComponent.GetHistorianDefaultConnection(doc);
            eventsettings[doc.Title] = UFUAEditorComponent.GetEventDefaultConnection(doc);
            serverentitysettings[doc.Title] = UFUAEditorComponent.GetServerEntityReference(doc);
            schedulersettings[doc.Title] = SchedulerEditorComponent.GetServerEntityReference(doc);

            doc.ListChildProjectPaths.ForEach(project =>
            {
                var abs = doc.MakeAbosoluteUri(project, UFProjectManagerComponent);
                if (abs != null)
                {
                    using (var childProject = UFProjectManager.UFProjectDocument.FromFile(abs.GetPathString(), UFProjectManagerComponent))
                    {
                        if (childProject != null)
                            UpdateProjectSettings(childProject, doc);
                    }
                }
            });
        }

        public void StopService()
        {
            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Server,
                                            Properties.Resources.StoppingServer,
                                            Properties.Resources.StoppedServer))
            {
                bStoppingService = true;
                OnStoppingService(new EventArgs());

                if (SignalR != null)
                {
                    SignalR.Dispose();
                    SignalR = null;
                }

                if (projectDocument != null)
                {
                    RecipeComponent.UnsubscribeServerSession(projectDocument);
                    projectDocument.UnsubscribeServerSession();
                    projectDocument.Dispose();
                    projectDocument = null;
                }

                RemoveAllSessions();
                
                ScreenSinkServiceSession.SessionCounter?.Dispose();
            }
        }

        #endregion
    }

    /// <summary>
    /// Used by OWIN's startup process. 
    /// </summary>
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

            var hubConfiguration = new HubConfiguration() { EnableJSONP = true };
#if DEBUG
            hubConfiguration.EnableDetailedErrors = true;
#endif
            app.MapSignalR(hubConfiguration);
            // app.MapSignalR();
        }
    }
}
