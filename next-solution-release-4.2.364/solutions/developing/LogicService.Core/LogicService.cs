using log4net;
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

namespace LogicService.Core
{
    public class LogicService
    {
        #region Declarations
        Thread serverThread = null;

        ComponentHost componentHost = new ComponentHost();
        PluginServices Plugins = new PluginServices();

        static readonly ILog logServer = Logger.GetDestinationLog(LoggerDestination.LogicService);

        //UFUAEditor.ComponentService.UFUAEditorManagerComponent ufuaEditorComponent = new UFUAEditor.ComponentService.UFUAEditorManagerComponent();
        ClientEditor.ComponentService.ClientEditorManagerComponent clientEditorManagerComponent = new ClientEditor.ComponentService.ClientEditorManagerComponent();
        UFProjectManager.ComponentService.UFProjectManagerComponent ufProjectManagerComponent = new UFProjectManager.ComponentService.UFProjectManagerComponent();
        LogicManager.ComponentService.DocumentEditorManagerComponent logicManager = new LogicManager.ComponentService.DocumentEditorManagerComponent();
        UriResolver.ComponentService.UriResolverComponent uriRisolver = new UriResolver.ComponentService.UriResolverComponent();
        UFProjectManager.UFProjectDocument projectDocument;
        #endregion

        #region Virtual Methods

        public virtual void StartService(string[] args)
        {
            OnStartingService(new EventArgs());

            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);
            if (commandArgs.ArgPairs.Count == 0)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                    Properties.Resources.MissingCommandLine, 
                    System.Diagnostics.EventLogEntryType.Warning, 
                    LoggerDestination.LogicService);

                System.Environment.Exit(-11);
            }

            var connectionString = string.Empty;

            try
            {
                connectionString = commandArgs.ArgPairs["conn"];
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
                    Mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        ex.Message, System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.LogicService);
                }

                try
                {
                    using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.LogicService,
                                                    Properties.Resources.InitializingServer,
                                                    Properties.Resources.InitializedServer))
                    {
                        componentHost.Components.Add(ufProjectManagerComponent);
                        componentHost.Components.Add(uriRisolver);
                        //componentHost.Components.Add(ufuaEditorComponent);
                        componentHost.Components.Add(clientEditorManagerComponent);
                        componentHost.Components.Add(logicManager);

                        uriRisolver.RegisterDocumentManager(logicManager, logicManager.TypeScheme);

                        if (commandArgs.ArgPairs.ContainsKey("conn"))
                            projectDocument = UFProjectManager.UFProjectDocument.FromFile(commandArgs.ArgPairs["conn"], ufProjectManagerComponent);
                        if (projectDocument == null)
                            throw new ArgumentNullException("Missing project to load or project cannot be loaded!");

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
                                logServer.Error(Properties.Resources.FailedToInitializePlugin, ex);
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

                    using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.LogicService,
                                                    Properties.Resources.StartingLogics,
                                                    Properties.Resources.StartedLogics))
                    {
                        var listStartupNoService = (from c in projectDocument.ListStartupLogics where c.ExecuteAsService == true select c).ToList();
                        listStartupNoService.ForEach(script =>
                        {
                            logicManager.Execute(script.Uri, projectDocument, script.ExecutionMode, this);
                        });
                    }
                }
                catch (Exception ex)
                {
                    if (ex is System.Net.Sockets.SocketException ||
                        ex is CommunicationException)
                    {
                        Mutex.ReleaseMutex();

                        var communicationErrorMessage = String.Format(Properties.Resources.CommunciationErrorOnStartingService.Replace("'newline'", Environment.NewLine), ex.Message);
                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                communicationErrorMessage,
                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.LogicService);

                        System.Environment.Exit(-10);
                    }
                    else
                        throw ex;
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
            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.LogicService, 
                                            Properties.Resources.StoppingServer,
                                            Properties.Resources.StoppedServer))
            {
                OnStoppingService(new EventArgs());
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

        public String Title
        {
            get
            {
                return Properties.Resources.Server;
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
