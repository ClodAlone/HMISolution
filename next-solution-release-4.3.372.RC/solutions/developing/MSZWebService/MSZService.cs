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
using MSZWebService.ServerCMS;
using System.Collections.ObjectModel;
using DevExpress.Xpo.DB.Helpers;

namespace MSZWebService
{
    public partial class MSZService : ServiceBase
    {
        #region Declarations
        protected MSZServiceCSM serverCSM;
        static Timer updateMapTimer;
        object lockObject = new object();
        Thread serverThread = null;
        ManualResetEvent serverStopping;
        bool bInsideTimer;
        bool isInStoppingMode;
        String _defaultDataProvider;
        String _defaultConnectionString;
        ComponentHost componentHost = new ComponentHost();
        PluginServices Plugins = new PluginServices();

        Dictionary<string, string> removedMap = null;
        Dictionary<string, int> multiInstanceMap = null;
        List<string> validSerialNumbers = null;
        DateTime validSerialNumbersTimestamp = DateTime.MinValue;

        static string PasswordHeader = "password";
        #endregion

        public MSZService(string _title)
        {
            InitializeComponent();
            title = _title;

#if !DEBUG
            // System.Data.SqlClient
            _defaultDataProvider = Properties.Settings.Default.DefaultDataProvider;
            // data source=SERVERDB;user id=WebLicUser;password=kjs6+m45!fht;initial catalog=ProgeaDB;Persist Security Info=true;
            _defaultConnectionString = Properties.Settings.Default.DefaultConnectionString;
            SetConnectionStringPassword();
#else
            _defaultDataProvider = @"System.Data.SqlClient";
            _defaultConnectionString = @"data source=SERVERDB;user id=WebLicUser;password=kjs6+m45!fht;initial catalog=ProgeaDB_Debug;Persist Security Info=true;";
            SetConnectionStringPassword();
#endif
        }

        public void UpdateRemovedMap()
        {
            if (string.IsNullOrEmpty(_defaultConnectionString) || string.IsNullOrEmpty(_defaultDataProvider))
                return;
            DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                        _defaultConnectionString,
                                                        "SELECT [RemovedSiteCode], [RemovedCode] FROM [tbLicenze]",
                                                        "[RemovedCode] <> '' AND [RemovedSiteCode] <> ''",
                                                        null,
                                                        null));

            Dictionary<string, string> _removedMap = new Dictionary<string, string>();
            foreach (DataRowView rowView in dataView)
            {
                string removedCode = rowView["RemovedCode"].ToString();
                string siteCode = rowView["RemovedSiteCode"].ToString();
                if (IsValidSiteCode(siteCode))
                {
                    if (!_removedMap.ContainsKey(siteCode))
                        _removedMap.Add(siteCode, removedCode);
                    else
                        _removedMap[siteCode] = $"{_removedMap[siteCode]}@{removedCode}";
                }
            }

            lock (lockObject)
            {
                removedMap = new Dictionary<string, string>(_removedMap);
            }

            dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                        _defaultConnectionString,
                                                        "SELECT [NumeroLicenza], [NumeroIstanzePerLicenza] FROM [tbLicenze]",
                                                        "[NumeroIstanzePerLicenza] <> 0 AND [NumeroIstanzePerLicenza] <> 1",
                                                        null,
                                                        null));

            Dictionary<string, int> _multiInstanceMap = new Dictionary<string, int>();
            foreach (DataRowView rowView in dataView)
            {
                string licNumber = rowView["NumeroLicenza"].ToString();
                int instanceNumber = (int)rowView["NumeroIstanzePerLicenza"];
                if (!_multiInstanceMap.ContainsKey(licNumber))
                    _multiInstanceMap.Add(licNumber, instanceNumber);
                else
                    _multiInstanceMap[licNumber] = instanceNumber;
            }

            lock (lockObject)
            {
                readOnlyMultiInstanceMap = null;
                multiInstanceMap = new Dictionary<string, int>(_multiInstanceMap.Where(k => k.Value > 1).ToDictionary(keySelector => keySelector.Key, keySelector => keySelector.Value));
            }

            dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                       _defaultConnectionString,
                                                       "SELECT [NumeroLicenza], [SiteCode] FROM [tbLicenze]",
                                                       String.Format("[DataScadenza] IS NULL OR [DataScadenza] >= {{ts '{0}'}}", 
                                                       DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")), 
                                                       null, 
                                                       null));

            var _disableSerialNumbers = (from key in _multiInstanceMap.Keys.AsParallel()
                                         where _multiInstanceMap[key] < 0
                                         select key).ToList();

            List<string> _validSerialNumbers = new List<string>();
            foreach (DataRowView rowView in dataView)
            {
                var licNumber = rowView["NumeroLicenza"].ToString();
                var siteCode = rowView["SiteCode"].ToString();
                if (!_validSerialNumbers.Contains(licNumber) && !_disableSerialNumbers.Contains(licNumber) &&
                    (IsValidSiteCode(siteCode) || !_removedMap.ContainsKey(siteCode)))
                {
                    _validSerialNumbers.Add(licNumber);
                }
            }

            lock (lockObject)
            {
                readOnlyValidSerialNumbers = null;
                validSerialNumbers = new List<string>(_validSerialNumbers);
                validSerialNumbersTimestamp = DateTime.UtcNow;
            }
        }

        bool IsValidSiteCode(string code)
        {
            return !String.IsNullOrWhiteSpace(code) && String.Compare(code, Properties.Settings.Default.HardwareSiteCode, StringComparison.OrdinalIgnoreCase) != 0;
        }

        void SetConnectionStringPassword()
        {
            try
            {
                var helper = new ConnectionStringParser(_defaultConnectionString);
                var pwd = helper.GetPartByName(PasswordHeader);
                if (String.IsNullOrWhiteSpace(pwd))
                {
                    pwd = Properties.Settings.Default.ConnectionStringPassword;
                    try
                    {
                        pwd = WPFUtilities.CryptString.CryptString.DecryptString(pwd);
                    }
                    catch
                    { }
                    helper.UpdatePartByName(PasswordHeader, pwd);
                    _defaultConnectionString = helper.GetConnectionString();
                }
            }
            catch
            { }
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
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
            updateMapTimer = new Timer((o) =>
            {
                lock (lockObject)
                {
                    if (bInsideTimer || isInStoppingMode)
                        return;
                    bInsideTimer = true;
                }
                try
                {
                    UpdateRemovedMap();
                }
                finally
                {
                    bInsideTimer = false;
                }
            }, null, 2000, Properties.Settings.Default.RestoreRemovedCodesTimeout);

        }
        #endregion

        #region Methods
        internal void StopService()
        {
            isInStoppingMode = true;
            AutoResetEvent waitHandle = null;
            lock (lockObject)
            {
                if (serverStopping != null)
                    serverStopping.Set();

                if (updateMapTimer != null)
                {
                    waitHandle = new AutoResetEvent(false);
                    updateMapTimer.Change(0, System.Threading.Timeout.Infinite);
                    updateMapTimer.Dispose(waitHandle);
                    updateMapTimer = null;
                }
            }

            if (waitHandle != null)
            {
                waitHandle.WaitOne();
                waitHandle.Dispose();
            }

            //using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.License, 
            //                                Properties.Resources.StoppingServer,
            //                                Properties.Resources.StoppedServer))
            {
                OnStoppingService(new EventArgs());
            }
        }
        public string Request(MSZWRequest request)
        {
            string sTrue = WPFUtilities.CryptString.CryptString.EncryptString(true.ToString());
            string sFalse = WPFUtilities.CryptString.CryptString.EncryptString(false.ToString());
            string sZero =  WPFUtilities.CryptString.CryptString.EncryptString((0).ToString());

            //using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.License, 
            //                                Properties.Resources.StartRequest,
            //                                Properties.Resources.StopRequest))
            {
                string res = null;

                MSZRequestArgs m = new MSZRequestArgs(request);
                OnRequestService(m);

                if (!string.IsNullOrEmpty(_defaultDataProvider) && !string.IsNullOrEmpty(_defaultConnectionString))
                    switch (request.RequestType)
                    {
                        case "aoFJf9sqeQYwONvVAvEfHg==":
                            if (ServerKrytpState)
                                res = string.Format("{1}{0}", "DTAIoMRyRAx0s8DI42iwQw==", request.RequestID);
                            break;    
                        case "CdQqW2FK/EEX0tL1T8Qfnw==":

                            try
                            {
                                string rcode = string.Empty;
                                lock (lockObject)
                                {
                                    if (removedMap != null && removedMap.ContainsKey(request.RequestID2))
                                    {
                                        rcode = removedMap[request.RequestID2];
                                    }
                                }
                                res = string.Format("{1}{0}", rcode, request.RequestID);
                            }
                            catch (Exception ex)
                            {
                                Program.LogServer(ex.ToString());
                            }
                            break;    
                        default:
                            break;
                    }

                return res;
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

                        StartService(args);
                        IsRunningAsService = true;

                        if (serverStopping.WaitOne())
                        {
                            StopService();
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

        ReadOnlyDictionary<string, int> readOnlyMultiInstanceMap;
        internal ReadOnlyDictionary<string, int> MultiInstanceMap
        {
            get
            {
                lock (lockObject)
                {
                    if (readOnlyMultiInstanceMap != null)
                        return readOnlyMultiInstanceMap;
                    else if (multiInstanceMap != null)
                        readOnlyMultiInstanceMap = new ReadOnlyDictionary<string, int>(multiInstanceMap);
                    return readOnlyMultiInstanceMap;
                }
            }
        }

        ReadOnlyCollection<string> readOnlyValidSerialNumbers;
        internal ReadOnlyCollection<string> ValidSerialNumbers
        {
            get
            {
                lock (lockObject)
                {
                    if (readOnlyValidSerialNumbers != null)
                        return readOnlyValidSerialNumbers;
                    else if (validSerialNumbers != null)
                        readOnlyValidSerialNumbers = new ReadOnlyCollection<string>(validSerialNumbers);
                    return readOnlyValidSerialNumbers;
                }
            }
        }

        public DateTime ValidSerialNumbersTimestamp
        {
            get
            {
                return validSerialNumbersTimestamp;
            }
        }

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
