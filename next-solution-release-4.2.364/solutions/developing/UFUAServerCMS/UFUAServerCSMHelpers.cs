using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFUAServerCMS;
using log4net;
using System.ServiceModel.Discovery;
using System.Threading.Tasks;

namespace UFUAServerCMS
{
    public class UFUAServerCSMHelpers : UFProcessServiceCMS.ProcessServiceCMS<IUFUAServerCMS>
    {
        #region Constructors
        public UFUAServerCSMHelpers(string instanceId) : 
            base(instanceId)
        { }
        public UFUAServerCSMHelpers(string instanceId, ILog log) :
            base(instanceId, log)
        { }
        #endregion

        #region Declaration
        bool serverIsActiveServer;
        public event EventHandler ServerActivate;
        public event EventHandler ServerDeactivate;
        #endregion

        #region Public Properties
        public bool IsServerActive
        {
            get
            {
                if(!TaskIsRunning)
                    CheckCustomActions();
                return serverIsActiveServer;
            }
        }
        #endregion

        #region Private methods
        void OnServerActivate(Object sender)
        {
            EventHandler temp = ServerActivate;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }
        void OnServerDeactivate(Object sender)
        {
            EventHandler temp = ServerDeactivate;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        bool bFirstTime = true;
        protected override void CheckCustomActions()
        {
            ConnectToServerCSM();
            if (ServerProxy != null)
            {
                try
                {
                    bool bActive = ServerProxy.IsActiveServer();
                    if (bActive && !serverIsActiveServer)
                    {
                        OnServerActivate(this);//fire activation
                        serverIsActiveServer = true;
                    }
                    else if (!bActive && (serverIsActiveServer || bFirstTime))
                    {
                        OnServerDeactivate(this);//fire deactivation
                        serverIsActiveServer = false;
                    }
                    bFirstTime = false;
                    return;
                }
                catch (Exception)
                {
                    CloseServerCSM();
                    if (serverIsActiveServer)
                    {
                        OnServerDeactivate(this);//fire deactivation
                        serverIsActiveServer = false;
                    }
                    bFirstTime = false;
                }
            }
            serverIsActiveServer = false;
        }
        #endregion

        #region IUFUAServerCMS

        public bool EnableScriptDebugging(Guid id, bool bEnable)
        {
            ConnectToServerCSM();
            if (ServerProxy != null)
            {
                try
                {
                    return ServerProxy.EnableScriptDebugging(id, bEnable);
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            return false;
        }

        public List<String> ScriptSynchronizing(Guid id, List<String> data)
        {
            ConnectToServerCSM();
            if (ServerProxy != null)
            {
                try
                {
                    return ServerProxy.ScriptSynchronizing(id, data);
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            return null;
        }

        public String GetAlertMessage(bool bClear)
        {
            ConnectToServerCSM();
            if (ServerProxy != null)
            {
                try
                {
                    return ServerProxy.GetAlertMessage(bClear);
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            return null;
        }

        public String GetApplicationName()
        {
            ConnectToServerCSM();
            if (ServerProxy != null)
            {
                try
                {
                    return ServerProxy.GetApplicationName();
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            return null;
        }
        #endregion

        #region Public Static Methods
        public static string GetServiceName(string applicationName)
        {
            using (var processService = new UFUAServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IUFUAServerCMS>(processService, applicationName);
                return serviceHelper.ServiceName;
            }
        }

        public static string GetServiceDisplayNameWhitoutDecoration()
        {
            return UFInterfaces.Properties.Resources.UFUAServer_ServiceDisplayName;
        }

        public static void OpenServiceManager(string applicationName, params string[] args)
        {
            using (var processService = new UFUAServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IUFUAServerCMS>(processService, applicationName);
                serviceHelper.Open(args);
            }
        }

        public static void OpenServiceManagerWithLogInInformation(string applicationName, string userName, string password, params string[] args)
        {
            using (var processService = new UFUAServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IUFUAServerCMS>(processService, applicationName)
                {
                    UserName = userName,
                    Password = password
                };
                serviceHelper.Open(args);
            }
        }

        public static void InstallService(string applicationName, params string[] args)
        {
            using (var processService = new UFUAServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IUFUAServerCMS>(processService, applicationName);
                serviceHelper.Install(args);
            }
        }

        public static void InstallServiceWithLogInInformation(string applicationName, string username, string password, params string[] args)
        {
            using (var processService = new UFUAServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IUFUAServerCMS>(processService, applicationName)
                {
                    UserName = username,
                    Password = password
                };
                serviceHelper.Install(args);
            }
        }

        public static void UninstallService(string applicationName)
        {
            using (var processService = new UFUAServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IUFUAServerCMS>(processService, applicationName);
                serviceHelper.Uninstall();
            }
        }

        public static void StartService(string applicationName)
        {
            using (var processService = new UFUAServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IUFUAServerCMS>(processService, applicationName);
                serviceHelper.Start();
            }
        }

        public static void StopService(string applicationName)
        {
            using (var processService = new UFUAServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IUFUAServerCMS>(processService, applicationName);
                serviceHelper.Stop();
            }
        }

        public static ServiceInstaller.ServiceState GetServiceStatus(string applicationName)
        {
            using (var processService = new UFUAServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IUFUAServerCMS>(processService, applicationName);
                return serviceHelper.GetServiceStatus();
            }
        }
        #endregion

        #region Overrides
        protected override string ServiceDisplayName
        {
            get
            {
                return UFInterfaces.Properties.Resources.UFUAServer_ServiceDisplayName;
            }
        }

        protected override string ServiceArguments
        {
            get
            {
                return Properties.Settings.Default.ServiceArguments;
            }
        }

        protected override string ProcessFileName
        {
            get
            {
                return UFUAServerInfo.UFUAServerInfo.GetProcessName();
            }
        }

        protected override string ProcessSubFolderName
        {
            get
            {
                return null;
            }
        }

        protected override int HttpPort
        {
            get
            {
                return Properties.Settings.Default.HttpPortNumber;
            }
        }
        #endregion

    }
}
