using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Utilities;

namespace UFProcessServiceCMS
{
    public abstract class ProcessServiceCMS<T> : IDisposable where T : IProcessServiceCMS
    {
        #region Declarations
        IProcessServiceCMS serverProxy;
        Process serverProcess;
        long serverProcessTicks;

        static List<string> serverStartedManually = new List<string>();

        bool serverRunning;
        bool serverStopping;
        bool serverStarted;
        bool serverRunningAsService;
        bool taskRunning;

        readonly string instanceId;
        readonly ILog log;

        readonly object lockObject = new object();
        #endregion

        #region Constructors
        protected ProcessServiceCMS(string instanceId) :
           this(instanceId, null)
        { }
        protected ProcessServiceCMS(string instanceId, ILog log)
        {
            this.instanceId = instanceId;
            this.log = log;
            this.hostName = Dns.GetHostName().ToLower();
        }

        protected ProcessServiceCMS(ProcessServiceCMS<T> instance)
        {
            instanceId = instance.instanceId;
            log = instance.log;
            hostName = instance.hostName;
        }
        #endregion

        #region Public Properties
        public String ServerName
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(ProcessFileName);
            }
        }

        public String InstanceId
        {
            get
            {
                return instanceId;
            }
        }

        public bool IsServerRunning
        {
            get
            {
                if (!taskRunning)
                    CheckServerRunning();

                return serverRunning;
            }
        }

        public bool IsServerStopping
        {
            get
            {
                if (!taskRunning)
                    CheckServerStopping();

                return serverStopping;
            }
        }

        public bool IsServerStarted
        {
            get
            {
                if (!taskRunning)
                    CheckServerStarted();

                return serverStarted;
            }
        }

        public bool IsServerRunningAsService
        {
            get
            {
                if (!taskRunning)
                    CheckServerRunningAsService();

                return serverRunningAsService;
            }
        }

        public bool IsServerStartedManually
        {
            get
            {
                return serverStartedManually.Contains(instanceId);
            }
        }

        String hostName;
        public String HostName
        {
            get
            {
                return hostName;
            }
            set
            {
                if (hostName == value || String.IsNullOrWhiteSpace(value))
                    return;
                hostName = value.ToLower();
                CloseServerCSM();
            }
        }

        TransportSchemeType schemeType = TransportSchemeType.NetPipe;
        public TransportSchemeType SchemeType
        {
            get
            {
                return schemeType;
            }
            set
            {
                if (schemeType == value)
                    return;
                schemeType = value;
                CloseServerCSM();
            }
        }

        TimeSpan operationTimeout = TimeSpan.FromMilliseconds(Properties.Settings.Default.DefaultOperationTimeout);
        public TimeSpan OperationTimeout
        {
            get
            {
                return operationTimeout;
            }
            set
            {
                if (operationTimeout == value)
                    return;
                operationTimeout = value;
                CloseServerCSM();
            }
        }

        #endregion

        #region Public Methods
        public bool StartServer(bool suspended, bool manually, params string[] args)
        {
            return StartServer(null, null, suspended, manually, args);
        }

        public bool StartServer(TextBlock textBlock, ScrollViewer scroll, bool suspended, bool manually, params string[] args)
        {
            return StartServer(textBlock, scroll, suspended, manually, false, args);
        }

        public bool StartServer(TextBlock textBlock, ScrollViewer scroll, bool suspended, bool manually, bool runAsCFR21UserIndentiy, params string[] args)
        {
            using (new WaitCursor())
            {
                if (IsServerRunning && !IsServerStopping)
                {
                    if (textBlock != null)
                    {
                        textBlock.Dispatcher.InvokeIfRequired(() =>
                        {
                            textBlock.Text = String.Format("{0}{1}{2}", textBlock.Text, String.Format(Properties.Resources.ServerIsAlreadyRunning, ServerName), Environment.NewLine);
                            scroll.ScrollToBottom();
                        });
                    }
                    return false;
                }
                if (textBlock != null)
                {
                    textBlock.Dispatcher.InvokeIfRequired(() =>
                    {
                        if (textBlock != null)
                        {
                            textBlock.Text = String.Format("{0}{1}{2}", textBlock.Text, String.Format(Properties.Resources.StartingServer, ServerName), Environment.NewLine);
                            scroll.ScrollToBottom();
                        }
                    });
                }

                for (int ii = 0; ii < args.Length; ii++)
                    args[ii] = args[ii].Replace("\"", "\\\"");

                var arguments = new StringBuilder(String.Format("{0} ", Properties.Settings.Default.ServerOption));
                arguments.AppendFormat(ServiceArguments, args);
                if (suspended)
                    arguments.AppendFormat(" {0}", Properties.Settings.Default.SuspendedOption);
                if (!manually)
                    arguments.AppendFormat(String.Format(" {0}", Properties.Settings.Default.CallingProcessOption), Process.GetCurrentProcess().Id);
                var serverRecipePath = GetServerPath();

                if (serverProcess != null)
                {
                    serverProcess.Close();
                    serverProcess.Dispose();
                }
                serverProcess = new Process();
                serverProcess.OutputDataReceived += (o, e) =>
                {
                    serverProcessTicks = DateTime.UtcNow.Ticks;
                    if (textBlock != null)
                    {
                        textBlock.Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            textBlock.FontSize = 12;
                            var output = String.Format("{0}{1}{2}", textBlock.Text, e.Data, Environment.NewLine);
                            int n = 1;
                            while (output.Length > 10240)
                            {
                                var lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Skip(n++).ToArray();
                                output = String.Join(Environment.NewLine, lines);
                            }
                            textBlock.Text = output;
                            scroll.ScrollToBottom();
                        });
                    }
                };
                serverProcess.Exited += (o, e) =>
                {
                    if (serverProcess != null)
                    {
                        serverProcess.Close();
                        serverProcess.Dispose();
                    }
                    serverProcess = null;
                    CloseServerCSM();

                    serverStartedManually.Remove(instanceId);
                };

                if (runAsCFR21UserIndentiy)
                {
                    var cryptedPassword = new System.Security.SecureString();
                    foreach (char passwordChar in WPFUtilities.CryptString.CryptString.DecryptString("ie4HZN8u9uW4NJOdnRNFbMcs9wfWnAaGICcU3qs6fcW8IpbPVp273NEpPMaAlV8B"/* "{45F928C5_1EF2_48D1_9505_14ACf7AD6AF8}" */))
                        cryptedPassword.AppendChar(passwordChar);
                    cryptedPassword.MakeReadOnly();

                    serverProcess.StartInfo.UserName = UFUAServerInfo.UFUAServerInfo.GetCFR21UserName();
                    serverProcess.StartInfo.Domain = UFUAServerInfo.UFUAServerInfo.GetCFR21DomainName();
                    serverProcess.StartInfo.Password = cryptedPassword;
                }

                serverProcess.StartInfo.FileName = serverRecipePath;
                serverProcess.StartInfo.Arguments = arguments.ToString();
                serverProcess.StartInfo.RedirectStandardOutput = true;
                serverProcess.StartInfo.RedirectStandardError = true;
                var encoding = Encoding.GetEncoding(System.Globalization.CultureInfo.CurrentUICulture.TextInfo.OEMCodePage);
                serverProcess.StartInfo.StandardOutputEncoding = encoding;
                serverProcess.StartInfo.StandardErrorEncoding = encoding;
                serverProcess.StartInfo.UseShellExecute = false;
                serverProcess.StartInfo.CreateNoWindow = true;
                serverProcess.EnableRaisingEvents = true;
                serverProcess.Start();
                serverProcess.BeginOutputReadLine();

                if (!serverStartedManually.Contains(instanceId))
                    serverStartedManually.Add(instanceId);

                ConnectToServerCSM();
            }

            return true;
        }

        public bool StopServer()
        {
            using (new WaitCursor())
            {
                try
                {
                    if (IsServerRunning && !IsServerStopping)
                    {
                        serverProxy.StopServer();
                        CloseServerCSM();
                        serverStartedManually.Remove(instanceId);
                    }
                }
                catch (Exception ex)
                {
                    if (log != null)
                        log.Error(String.Format(Properties.Resources.ErrorStoppingServer, ServerName), ex);
                }
            }

            return true;
        }

        public async System.Threading.Tasks.Task<bool> WaitServerStarted()
        {
            return await WaitServerStarted(TimeSpan.Zero);
        }

        public async System.Threading.Tasks.Task<bool> WaitServerStarted(int millisecondsTimeout)
        {
            return await WaitServerStarted(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public async System.Threading.Tasks.Task<bool> WaitServerStarted(TimeSpan timeout)
        {
            var task = Task.Factory.StartNew(() =>
            {
                long ticks = serverProcessTicks;
                DateTime dt = DateTime.UtcNow.Add(timeout);
                while (dt > DateTime.UtcNow)
                {
                    if (serverProcess == null)
                        return false;

                    if (IsServerRunning && IsServerStarted)
                        return true;

                    if (ticks != serverProcessTicks)
                        dt = DateTime.UtcNow.Add(timeout);
                }

                return false;
            });

            await task;

            return task.Result;
        }

        public async System.Threading.Tasks.Task<bool> WaitServerStopped()
        {
            return await WaitServerStopped(TimeSpan.Zero);
        }

        public async System.Threading.Tasks.Task<bool> WaitServerStopped(int millisecondsTimeout)
        {
            return await WaitServerStopped(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public async System.Threading.Tasks.Task<bool> WaitServerStopped(TimeSpan timeout)
        {
            var task = Task.Factory.StartNew(() =>
            {
                long ticks = serverProcessTicks;
                DateTime dt = DateTime.UtcNow.Add(timeout);
                while (dt > DateTime.UtcNow)
                {
                    if (serverProcess == null || serverProcess.WaitForExit(1000))
                        return true;

                    if (ticks != serverProcessTicks)
                        dt = DateTime.UtcNow.Add(timeout);
                }

                return false;
            });

            await task;

            return task.Result;
        }

        public void StartServerStatusInBackground()
        {
            lock (lockObject)
            {
                if (taskRunning)
                    return;
                taskRunning = true;
                var task = Task.Factory.StartNew(() =>
                {
                    var oldPriority = System.Threading.Thread.CurrentThread.Priority;
                    System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.BelowNormal;
                    try
                    {
                        while (!bDisposed)
                        {
                            CheckCustomActions();
                            System.Threading.Thread.Sleep(200);
                            CheckServerRunning();
                            System.Threading.Thread.Sleep(200);
                            CheckServerStopping();
                            System.Threading.Thread.Sleep(200);
                            CheckServerStarted();
                            System.Threading.Thread.Sleep(200);
                            CheckServerRunningAsService();
                            System.Threading.Thread.Sleep(200);
                        }
                    }
                    finally
                    {
                        System.Threading.Thread.CurrentThread.Priority = oldPriority;
                    }
                }, TaskCreationOptions.LongRunning);
            }
        }

        public string GetNetPipeServiceAddress()
        {
            return GetNetPipeServiceAddress(instanceId);
        }

        public string GetNetTcpServiceAddress()
        {
            return GetNetTcpServiceAddress(instanceId, HttpPort);
        }

        public string GetBasicHttpServiceAddress()
        {
            return GetBasicHttpServiceAddress(instanceId, HttpPort);
        }

        public bool IsServerStateRunning()
        {
            ConnectToServerCSM();
            if (ServerProxy != null)
            {
                try
                {
                    return ServerProxy.IsServerStateRunning();
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            return false;
        }

        public String GetServerStatus()
        {
            ConnectToServerCSM();
            if (ServerProxy != null)
            {
                try
                {
                    return ServerProxy.GetServerStatus();
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            return null;
        }

        public BalloonInfo GetBalloonMessage(bool bClear)
        {
            ConnectToServerCSM();
            if (ServerProxy != null)
            {
                try
                {
                    return ServerProxy.GetBalloonMessage(bClear);
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            return null;
        }
        #endregion

        #region Virtual Methods
        protected virtual void CheckCustomActions()
        { }
        #endregion

        #region Protected Properties
        protected T ServerProxy
        {
            get
            {
                if (serverProxy is T)
                    return (T)serverProxy;

                return default(T);
            }
        }

        protected bool TaskIsRunning
        {
            get { return taskRunning; }
        }
        #endregion

        #region Protected Methods
        protected void ConnectToServerCSM()
        {
            if (serverProxy != null || bDisposed)
                return;

            serverProxy = ConnectToHost(instanceId);
            ((IContextChannel)serverProxy).OperationTimeout = OperationTimeout;
        }

        protected void CloseServerCSM()
        {
            if (serverProxy != null)
            {
                try
                {
                    using (serverProxy as IDisposable)
                    {
                        ICommunicationObject proxy = serverProxy as ICommunicationObject;

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
                    }
                }
                catch (Exception)
                {

                }
                serverProxy = null;
            }
        }
        #endregion

        #region Abstracts
        internal protected abstract string ServiceDisplayName { get; }
        internal protected abstract string ServiceArguments { get; }
        protected abstract string ProcessFileName { get; }
        protected abstract string ProcessSubFolderName { get; }
        protected abstract int HttpPort { get; }
        #endregion

        #region Private Methods
        void CheckServerRunning()
        {
            ConnectToServerCSM();
            if (serverProxy != null)
            {
                try
                {
                    var service = serverProxy.IsRunningAsService();
                    serverRunning = true;
                    return;
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            serverRunning = false;
        }

        void CheckServerStopping()
        {
            ConnectToServerCSM();
            if (serverProxy != null)
            {
                try
                {
                    serverStopping = serverProxy.IsStopping();
                    return;
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            serverStopping = false;
        }

        void CheckServerStarted()
        {
            ConnectToServerCSM();
            if (serverProxy != null)
            {
                try
                {
                    serverStarted = serverProxy.IsStarted();
                    return;
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            serverStarted = false;
        }

        void CheckServerRunningAsService()
        {
            ConnectToServerCSM();
            if (serverProxy != null)
            {
                try
                {
                    serverRunningAsService = serverProxy.IsRunningAsService();
                    return;
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            serverRunningAsService = false;
        }

        T ConnectToHost(String connection)
        {
            switch (SchemeType)
            {
                case TransportSchemeType.NetTcp:
                    return ChannelFactory<T>.CreateChannel(new NetTcpBinding(SecurityMode.None),
                        new EndpointAddress(GetNetTcpServiceAddress()));
                case TransportSchemeType.Http:
                    return ChannelFactory<T>.CreateChannel(new BasicHttpBinding(),
                        new EndpointAddress(GetBasicHttpServiceAddress()));
                default:
                    return ChannelFactory<T>.CreateChannel(new NetNamedPipeBinding(),
                        new EndpointAddress(GetNetPipeServiceAddress()));
            }
        }

        string GetNetPipeServiceAddress(String connection)
        {
            return String.Format("net.pipe://{2}/{0}/{1}", ServerName, connection, Dns.GetHostName().ToLower());
        }

        string GetNetTcpServiceAddress(String connection, int port)
        {
            return String.Format("net.tcp://{2}:{3}/{0}/{1}", ServerName, connection, HostName, port);
        }

        string GetBasicHttpServiceAddress(String connection, int port)
        {
            return String.Format("http://{2}:{3}/{0}/{1}", ServerName, connection, HostName, port);
        }

        internal String GetServerPath()
        {
            if (String.IsNullOrEmpty(ProcessSubFolderName))
                return String.Format("{0}\\{1}", GetServerFolder(), ProcessFileName);
            else
                return String.Format("{0}\\{1}\\{2}", GetServerFolder(), ProcessSubFolderName, ProcessFileName);
        }

        String GetServerFolder()
        {
            return GetAssemblyPath();
        }


        string GetAssemblyPath()
        {
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                return System.IO.Path.GetDirectoryName(callingMainAssembly.Location);
            return string.Empty;
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (serverProcess != null)
            {
                serverProcess.Close();
                serverProcess.Dispose();
            }

            CloseServerCSM();
        }
        #endregion

    }
}
