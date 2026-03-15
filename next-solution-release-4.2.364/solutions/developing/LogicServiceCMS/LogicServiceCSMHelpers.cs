using log4net;
using System;
using System.Text;

namespace LogicServiceCMS
{
    public class LogicServiceCSMHelpers : UFProcessServiceCMS.ProcessServiceCMS<ILogicServiceCMS>
    {
        #region Constructors
        public LogicServiceCSMHelpers(string instanceId) : 
            base(instanceId)
        { }
        public LogicServiceCSMHelpers(string instanceId, ILog log) :
            base(instanceId, log)
        { }
        #endregion

        #region Public Static Methods
        public static string GetServiceName(string applicationName)
        {
            using (var processService = new LogicServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<ILogicServiceCMS>(processService, applicationName);
                return serviceHelper.ServiceName;
            }
        }

        public static string GetServiceDisplayNameWhitoutDecoration()
        {
            return UFInterfaces.Properties.Resources.LogicService_ServiceDisplayName;
        }

        public static String GetSysTrayProcessName()
        {
            return Properties.Settings.Default.SysTrayApp;
        }

        public static void OpenServiceManager(string applicationName, string[] dependencies, params string[] args)
        {
            using (var processService = new LogicServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<ILogicServiceCMS>(processService, applicationName, dependencies);
                serviceHelper.Open(args);
            }
        }

        public static void InstallService(string applicationName, string[] dependencies, params string[] args)
        {
            using (var processService = new LogicServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<ILogicServiceCMS>(processService, applicationName, dependencies);
                serviceHelper.Install(args);
            }
        }

        public static void InstallServiceWithLogInInformation(string applicationName, string username, string password, string[] dependencies, params string[] args)
        {
            using (var processService = new LogicServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<ILogicServiceCMS>(processService, applicationName, dependencies)
                {
                    UserName = username,
                    Password = password
                };
                serviceHelper.Install(args);
            }
        }

        public static void UninstallService(string applicationName)
        {
            using (var processService = new LogicServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<ILogicServiceCMS>(processService, applicationName);
                serviceHelper.Uninstall();
            }
        }

        public static void StartService(string applicationName)
        {
            using (var processService = new LogicServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<ILogicServiceCMS>(processService, applicationName);
                serviceHelper.Start();
            }
        }
        public static void StopService(string applicationName)
        {
            using (var processService = new LogicServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<ILogicServiceCMS>(processService, applicationName);
                serviceHelper.Stop();
            }
        }

        public static ServiceInstaller.ServiceState GetServiceStatus(string applicationName)
        {
            using (var processService = new LogicServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<ILogicServiceCMS>(processService, applicationName);
                return serviceHelper.GetServiceStatus();
            }
        }
        #endregion

        #region ILogicServiceCMS
        #endregion

        #region Overrides
        protected override string ServiceDisplayName
        {
            get
            {
                return UFInterfaces.Properties.Resources.LogicService_ServiceDisplayName;
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
                return Properties.Settings.Default.DefaultServerExe;
            }
        }

        protected override string ProcessSubFolderName
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(Properties.Settings.Default.DefaultServerExe);
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
