using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace ADServerCMS
{
    public class ADServerCSMHelpers : UFProcessServiceCMS.ProcessServiceCMS<IADServerCMS>
    {
        #region Constructors
        public ADServerCSMHelpers(string instanceId) : 
            base(instanceId)
        { }
        public ADServerCSMHelpers(string instanceId, ILog log) :
            base(instanceId, log)
        { }
        #endregion

        #region IADServerCMS
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
            using (var processService = new ADServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IADServerCMS>(processService, applicationName);
                return serviceHelper.ServiceName;
            }
        }

        public static string GetServiceDisplayNameWhitoutDecoration()
        {
            return UFInterfaces.Properties.Resources.ADServer_ServiceDisplayName;
        }

        public static void OpenServiceManager(string applicationName, params string[] args)
        {
            using (var processService = new ADServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IADServerCMS>(processService, applicationName);
                serviceHelper.Open(args);
            }
        }

        public static void InstallService(string applicationName, params string[] args)
        {
            using (var processService = new ADServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IADServerCMS>(processService, applicationName);
                serviceHelper.Install(args);
            }
        }

        public static void InstallServiceWithLogInInformation(string applicationName, string username, string password, params string[] args)
        {
            using (var processService = new ADServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IADServerCMS>(processService, applicationName)
                {
                    UserName = username,
                    Password = password
                };
                serviceHelper.Install(args);
            }
        }

        public static void UninstallService(string applicationName)
        {
            using (var processService = new ADServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IADServerCMS>(processService, applicationName);
                serviceHelper.Uninstall();
            }
        }

        public static void StartService(string applicationName)
        {
            using (var processService = new ADServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IADServerCMS>(processService, applicationName);
                serviceHelper.Start();
            }
        }

        public static void StopService(string applicationName)
        {
            using (var processService = new ADServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IADServerCMS>(processService, applicationName);
                serviceHelper.Stop();
            }
        }

        public static ServiceInstaller.ServiceState GetServiceStatus(string applicationName)
        {
            using (var processService = new ADServerCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IADServerCMS>(processService, applicationName);
                return serviceHelper.GetServiceStatus();
            }
        }
        #endregion

        #region Overrides
        protected override string ServiceDisplayName
        {
            get
            {
                return UFInterfaces.Properties.Resources.ADServer_ServiceDisplayName;
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
                return ADServerInfo.ADServerInfo.GetProcessName();
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
