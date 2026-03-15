using log4net;
using MSZService.ServerCMS;
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
using MSZServiceCMS;
using Utilities.Logger;

namespace MSZService
{
    public partial class MSZService : ServiceBase
    {
        #region Declarations
        protected MSZServiceCSM serverCSM;
        object lockObject = new object();
        Thread serverThread = null;
        ManualResetEvent serverStopping;
        internal int maxClientAllowed {get; set;}
        internal int clientAllowed {get; set;}

        ComponentHost componentHost = new ComponentHost();
        PluginServices Plugins = new PluginServices();

        static readonly ILog logServer = Logger.GetDestinationLog(LoggerDestination.License);
        #endregion

        public MSZService(string _title)
        {
            InitializeComponent();
            try
            {
                MSZ.MSZView.CheckState();
                serverPort = Convert.ToInt32(MSZ.MSZView.ServerNetPort);// Properties.Settings.Default.ServerPort;
            }
            catch (Exception)
            {
                serverPort = Properties.Settings.Default.ServerPort;
            }
            title = _title;
        }

        #region Virtual Methods

        public virtual void StartService(string[] args)
        {
            OnStartingService(new EventArgs());

            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);

            lock (lockObject)
            {
                if (serverStopping == null)
                    serverStopping = new ManualResetEvent(false);
            }

            var appName = Assembly.GetEntryAssembly().GetName().Name;
            using (var Mutex = new Mutex(false, appName))
            {
                try
                {
                    while (!Mutex.WaitOne(1000))
                    {
                        if (serverStopping.WaitOne(1000))
                            return;

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                            Properties.Resources.ProjectAlreadyRunning,
                            System.Diagnostics.EventLogEntryType.Information, LoggerDestination.License);
                    }
                }
                catch (AbandonedMutexException ex)
                {
                    Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                        ex.Message, System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.License);
                }

                try
                {
                    using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.License, 
                                                    Properties.Resources.InitializingServer,
                                                    Properties.Resources.InitializedServer))
                    {
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
                                serverCSM = new MSZServiceCSM(this);
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
                                                            System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.License);

                                    if ((retries % openServiceHostsMaxRetries) == 0)
                                    {
                                        if (ex is AddressAlreadyInUseException)
                                        {
                                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    Properties.Resources.ProjectAlreadyRunning,
                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.License);
                                        }
                                        else
                                        {
                                            var communicationErrorMessage = String.Format(Properties.Resources.CommunciationErrorOnStartingService.Replace("'newline'", Environment.NewLine), ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                                    communicationErrorMessage,
                                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.License);
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
                                                    System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.License);
                        }
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
            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.License, 
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
        public string Request(MSZRequest request)
        {
            string sTrue = WPFUtilities.CryptString.CryptString.EncryptString(true.ToString());
            string sFalse = WPFUtilities.CryptString.CryptString.EncryptString(false.ToString());
            string sZero =  WPFUtilities.CryptString.CryptString.EncryptString((0).ToString());

            string res = null;
            MSZ.MSZView.CheckState();

            MSZRequestArgs m = new MSZRequestArgs(request);
            OnRequestService(m);

            switch (request.RequestType)
            {
                case "iZEsGneVntCcoR66tw80cQ==":
                    res = string.Format("{1}{0}", WPFUtilities.CryptString.CryptString.EncryptString(string.Format("{0}/{1}", clientAllowed, maxClientAllowed)), request.RequestID);
                    break;    
                case "iQeRnwG8TKuTsKMG7cEmTw==":
                    if (ServerKrytpState)
                        res = string.Empty;
                    else
                        res = string.Format("{1}{0}", WPFUtilities.CryptString.CryptString.EncryptString(MSZ.MSZView.GetSerial()), request.RequestID);
                    break;
                case "IiFxly+MSH6loa1Jm9a/pQ==":
                    if (ServerKrytpState)
                        res = string.Empty;
                    else
                        res = string.Format("{1}{0}", MSZ.MSZView.GetKeyData(), request.RequestID);
                    break;
                default:
                    break;
            }
            return res;
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
                                        System.Diagnostics.EventLogEntryType.Information, LoggerDestination.License);

                        StartService(args);

                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                Properties.Resources.ServiceStarted,
                                                System.Diagnostics.EventLogEntryType.Information, LoggerDestination.License);

                        IsRunningAsService = true;

                        if (serverStopping.WaitOne())
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            Properties.Resources.ServiceStopping,
                                            System.Diagnostics.EventLogEntryType.Information, LoggerDestination.License);

                            StopService();

                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                                    Properties.Resources.ServiceStopped,
                                                    System.Diagnostics.EventLogEntryType.Information, LoggerDestination.License);

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

        bool isStarted;
        public bool IsStarted
        {
            get
            {
                return isStarted;
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

        protected string ProjectConnectionString { get; private set; }

        String title;
        public String Title
        {
            get
            {
                return title;
            }
        }

        bool serverKrytpState;
        public bool ServerKrytpState
        {
            get
            {
                return serverKrytpState;
            }
            set
            {
                serverKrytpState = value;
            }
        }

        int serverPort;
        public int ServerPort
        {
            get
            {
                return serverPort;
            }
        }

        bool communicationStatus;
        public bool CommunicationStatus
        {
            get
            {
                return communicationStatus;
            }
        }

        String statusText;
        public String StatusText
        {
            get
            {
                return statusText;
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
       
        public event EventHandler<MSZRequestArgs> RequestService;
        private void OnRequestService(MSZRequestArgs e)
        {
            EventHandler<MSZRequestArgs> temp = RequestService;
            if (temp != null)
                temp(null, e);
        }
       
        #endregion
        #endregion
    }
}
