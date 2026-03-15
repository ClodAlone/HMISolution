using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using log4net;
using MSModel;

namespace MSServerCMS
{
    public class MSServerCMSHelpers : UFProcessServiceCMS.ProcessServiceCMS<IMSServerCMS>
    {
        #region Constructors
        public MSServerCMSHelpers(string instanceId) : 
            base(instanceId)
        { }
        public MSServerCMSHelpers(string instanceId, ILog log) :
            base(instanceId, log)
        { }
        #endregion

        #region IMSServerCMS
        public bool UpdateScheduler(ChangedSchedArgs sched)
        {
            ConnectToServerCSM();
            if (ServerProxy != null)
            {
                try
                {
                    return ServerProxy.UpdateScheduler(sched);
                }
                catch (Exception)
                {
                    CloseServerCSM();
                }
            }

            return false;
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
            using (var processService = new MSServerCMSHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IMSServerCMS>(processService, applicationName);
                return serviceHelper.ServiceName;
            }
        }

        public static string GetServiceDisplayNameWhitoutDecoration()
        {
            return UFInterfaces.Properties.Resources.MSServer_ServiceDisplayName;
        }

        public static void OpenServiceManager(string applicationName, params string[] args)
        {
            using (var processService = new MSServerCMSHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IMSServerCMS>(processService, applicationName);
                serviceHelper.Open(args);
            }
        }

        public static void InstallService(string applicationName, params string[] args)
        {
            using (var processService = new MSServerCMSHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IMSServerCMS>(processService, applicationName);
                serviceHelper.Install(args);
            }
        }

        public static void InstallServiceWithLogInInformation(string applicationName, string username, string password, params string[] args)
        {
            using (var processService = new MSServerCMSHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IMSServerCMS>(processService, applicationName)
                {
                    UserName = username,
                    Password = password
                };
                serviceHelper.Install(args);
            }
        }

        public static void UninstallService(string applicationName)
        {
            using (var processService = new MSServerCMSHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IMSServerCMS>(processService, applicationName);
                serviceHelper.Uninstall();
            }
        }

        public static void StartService(string applicationName)
        {
            using (var processService = new MSServerCMSHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IMSServerCMS>(processService, applicationName);
                serviceHelper.Start();
            }
        }

        public static void StopService(string applicationName)
        {
            using (var processService = new MSServerCMSHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IMSServerCMS>(processService, applicationName);
                serviceHelper.Stop();
            }
        }

        public static ServiceInstaller.ServiceState GetServiceStatus(string applicationName)
        {
            using (var processService = new MSServerCMSHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IMSServerCMS>(processService, applicationName);
                return serviceHelper.GetServiceStatus();
            }
        }
        #endregion

        #region Overrides
        protected override string ServiceDisplayName
        {
            get
            {
                return UFInterfaces.Properties.Resources.MSServer_ServiceDisplayName;
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
                return MSServerInfo.MSServerInfo.GetProcessName();
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
