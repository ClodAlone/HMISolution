using log4net;
using ScriptService.ServerCMS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using Utilities.Logger;

namespace ScriptService
{
    public partial class ScriptService : ServiceBase
    {
        #region Declarations
        protected ScriptServiceCSM serverCSM;
        object lockObject = new object();
        Thread serverThread = null;
        ManualResetEvent serverStopping;

        ComponentHost componentHost = new ComponentHost();
        PluginServices Plugins = new PluginServices();
        
        static readonly ILog logServer = Logger.GetDestinationLog(LoggerDestination.ScriptService);

        UFUAEditor.ComponentService.UFUAEditorManagerComponent ufuaEditorComponent = new UFUAEditor.ComponentService.UFUAEditorManagerComponent();
        ClientEditor.ComponentService.ClientEditorManagerComponent clientEditorManagerComponent = new ClientEditor.ComponentService.ClientEditorManagerComponent();
        UFProjectManager.ComponentService.UFProjectManagerComponent ufProjectManagerComponent = new UFProjectManager.ComponentService.UFProjectManagerComponent();
        ScriptManager.ComponentService.ScriptManagerComponent scriptManager = new ScriptManager.ComponentService.ScriptManagerComponent();
        UriResolver.ComponentService.UriResolverComponent uriRisolver = new UriResolver.ComponentService.UriResolverComponent();
        StringManager.ComponentService.StringEditorManagerComponent stringManager = new StringManager.ComponentService.StringEditorManagerComponent();
        UFProjectManager.UFProjectDocument projectDocument;
        #endregion

        public ScriptService()
        {
            InitializeComponent();
        }

        #region Virtual Methods

        public virtual void StartService(string[] args)
        {
            OnStartingService(new EventArgs());

            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);
            
            if (commandArgs.ArgPairs.Count == 0)
            {
                if (Environment.UserInteractive)
                {
                    MessageBox.Show(Properties.Resources.MissingCommandLine, Properties.Resources.Server, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                System.Environment.Exit(-11);
            }

            lock (lockObject)
            {
                if (serverStopping == null)
                    serverStopping = new ManualResetEvent(false);
            }

            try
            {
                projectDocument = UFProjectManager.UFProjectDocument.FromFile(commandArgs.ArgPairs["conn"], ufProjectManagerComponent);

                if (projectDocument == null)
                {
                    throw new ArgumentNullException("Missing project to load or project cannot be loaded!");
                }
            }
            catch
            {
                throw new Exception("Connection string is missing!");
            }

            var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
            var uniqueStartupName =
                XpoHelpers.XpoHelper.GetServiceUniqueStartupName(assemblyName, ".", projectDocument.ConfigurationId.ToString());

            using (var Mutex = new Mutex(false, uniqueStartupName))
            {
                try
                {
                    while (!Mutex.WaitOne(1000))
                    {
                        if (serverStopping.WaitOne(1000))
                            return;

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                            Properties.Resources.ProjectAlreadyRunning,
                            System.Diagnostics.EventLogEntryType.Information, LoggerDestination.ScriptService);
                    }
                }
                catch (AbandonedMutexException ex)
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        ex.Message, System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.ScriptService);
                }

                try
                {
                    using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.ScriptService,
                                                    Properties.Resources.InitializingServer,
                                                    Properties.Resources.InitializedServer))
                    {
                        if (commandArgs.ArgPairs.ContainsKey("conn"))
                            projectDocument = UFProjectManager.UFProjectDocument.FromFile(commandArgs.ArgPairs["conn"], ufProjectManagerComponent);
                        if (projectDocument == null)
                            throw new ArgumentNullException("Missing project to load or project cannot be loaded!");

                        projectDocument.SetCurrentLogFileName();

                        bool useDiscovery = true;
                        int retries = 0;
                        int delay = 0;
                        int openServiceHostsMaxRetries = Properties.Settings.Default.OpenServiceHostsMaxRetries;
                        while (true)
                        {
                            if (serverStopping.WaitOne(delay))
                                return;

                            try
                            {
                                serverCSM = new ScriptServiceCSM(projectDocument.ConfigurationId.ToString(), this);
                                serverCSM.HostServer(useDiscovery);
                                break;
                            }
                            catch (Exception ex)
                            {
                                if (serverCSM != null)
                                {
                                    try
                                    {
                                        serverCSM.Dispose();
                                    }
                                    catch
                                    { }
                                }

                                if (ex is AddressAlreadyInUseException ||
                                    ex is System.Net.Sockets.SocketException ||
                                    ex is CommunicationException)
                                {
                                    if (useDiscovery && !(ex is AddressAlreadyInUseException))
                                        useDiscovery = false;

                                    if (++retries > openServiceHostsMaxRetries)
                                        retries = 1;

                                    delay = Math.Max(retries * 1000, delay);
                                    var str = String.Format(Properties.Resources.RetryOpenSocketWarningMessage, retries, openServiceHostsMaxRetries, delay);
                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                            str,
                                                            System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.LogicService);

                                    if ((retries % openServiceHostsMaxRetries) == 0)
                                    {
                                        if (ex is AddressAlreadyInUseException)
                                        {
                                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    Properties.Resources.ProjectAlreadyRunning,
                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.LogicService);
                                        }
                                        else
                                        {
                                            var communicationErrorMessage = String.Format(Properties.Resources.CommunciationErrorOnStartingService.Replace("'newline'", Environment.NewLine), ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                                    communicationErrorMessage,
                                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.LogicService);
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
                                                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.ScriptService);
                        }

                        componentHost.Components.Add(ufProjectManagerComponent);
                        componentHost.Components.Add(uriRisolver);
                        componentHost.Components.Add(ufuaEditorComponent);
                        componentHost.Components.Add(clientEditorManagerComponent);
                        componentHost.Components.Add(scriptManager);
                        componentHost.Components.Add(stringManager);

                        projectDocument.UpdateSessionSettings();

                        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        uriRisolver.GetListInstalledDocumentManagers(String.Format("{0}\\DocumentManagers\\", baseDirectory));
                        Plugins.FindPlugins(String.Format("{0}\\DataSinks\\", baseDirectory));

                        foreach (var pluginOn in Plugins.AvailablePlugins)
                        {
                            try
                            {
                                ((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance.Initialize();
                            }
                            catch (Exception ex)
                            {
                                logServer.Error(Properties.Resources.FailedToInitializePlugin, ex);
                            }
                        }

                        OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
                        OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();
                    }

                    using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.ScriptService,
                                                    Properties.Resources.StartingScripts,
                                                    Properties.Resources.StartedScripts))
                    {
                        projectDocument.StartupServiceScripts(scriptManager);
                    }
                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
        }
        #endregion

        #region Methods
        public void StopService()
        {
            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.ScriptService, 
                                            Properties.Resources.StoppingServer,
                                            Properties.Resources.StoppedServer))
            {
                lock (lockObject)
                {
                    if (serverStopping != null)
                        serverStopping.Set();
                }

                OnStoppingService(new EventArgs());
            }
        }
        #endregion

        #region Override Methods

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
                                        System.Diagnostics.EventLogEntryType.Information, LoggerDestination.ScriptService);

                        StartService(args);

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                Properties.Resources.ServiceStarted,
                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.ScriptService);

                        IsRunningAsService = true;

                        if (serverStopping.WaitOne())
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            Properties.Resources.ServiceStopping,
                                            System.Diagnostics.EventLogEntryType.Information, LoggerDestination.ScriptService);

                            StopService();

                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    Properties.Resources.ServiceStopped,
                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.ScriptService);

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

        public bool _IsStarted;
        public bool IsStarted
        {
            get
            {
                return _IsStarted;
            }
            internal set
            {
                _IsStarted = value;
            }
        }

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
                return ServiceName;
            }
        }

        public bool CommunicationStatus
        {
            get
            {
                if (serverThread == null)
                    return true;
                return serverThread.ThreadState == System.Threading.ThreadState.Running;
            }
        }

        public String StatusText
        {
            get
            {
                if (serverThread == null)
                    return System.Threading.ThreadState.Unstarted.ToString();
                return serverThread.ThreadState.ToString();
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
        /// Triggers the StartingService event.
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
    }
}
