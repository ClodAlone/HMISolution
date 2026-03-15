using DocumentManager.ComponentService;
using log4net;
using System;
using System.Text;

namespace RecipeServiceCMS
{
    public class RecipeServiceCSMHelpers : UFProcessServiceCMS.ProcessServiceCMS<IRecipeServiceCMS>
    {
        #region Constructors
        public RecipeServiceCSMHelpers(string instanceId) : 
            base(instanceId)
        { }
        public RecipeServiceCSMHelpers(string instanceId, ILog log) :
            base(instanceId, log)
        { }

        public RecipeServiceCSMHelpers(RecipeServiceCSMHelpers instance) :
            base(instance)
        { }
        #endregion

        #region Public Static Methods
        public static string GetServiceName(string applicationName)
        {
            using (var processService = new RecipeServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IRecipeServiceCMS>(processService, applicationName);
                return serviceHelper.ServiceName;
            }
        }

        public static string GetServiceDisplayNameWhitoutDecoration()
        {
            return UFInterfaces.Properties.Resources.RecipeService_ServiceDisplayName; ;
        }

        public static String GetSysTrayProcessName()
        {
            return Properties.Settings.Default.SysTrayApp;
        }

        public static void OpenServiceManager(string applicationName, string[] dependencies, params string[] args)
        {
            using (var processService = new RecipeServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IRecipeServiceCMS>(processService, applicationName, dependencies);
                serviceHelper.Open(args);
            }
        }

        public static void InstallService(string applicationName, string[] dependencies, params string[] args)
        {
            using (var processService = new RecipeServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IRecipeServiceCMS>(processService, applicationName, dependencies);
                serviceHelper.Install(args);
            }
        }

        public static void InstallServiceWithLogInInformation(string applicationName, string username, string password, string[] dependencies, params string[] args)
        {
            using (var processService = new RecipeServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IRecipeServiceCMS>(processService, applicationName, dependencies)
                {
                    UserName = username,
                    Password = password
                };
                serviceHelper.Install(args);
            }
        }

        public static void UninstallService(string applicationName)
        {
            using (var processService = new RecipeServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IRecipeServiceCMS>(processService, applicationName);
                serviceHelper.Uninstall();
            }
        }

        public static void StartService(string applicationName)
        {
            using (var processService = new RecipeServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IRecipeServiceCMS>(processService, applicationName);
                serviceHelper.Start();
            }
        }
        public static void StopService(string applicationName)
        {
            using (var processService = new RecipeServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IRecipeServiceCMS>(processService, applicationName);
                serviceHelper.Stop();
            }
        }

        public static ServiceInstaller.ServiceState GetServiceStatus(string applicationName)
        {
            using (var processService = new RecipeServiceCSMHelpers(null, null))
            {
                var serviceHelper = new UFProcessServiceCMS.ProcessServiceManager<IRecipeServiceCMS>(processService, applicationName);
                return serviceHelper.GetServiceStatus();
            }
        }
        #endregion

        #region Overrides
        protected override string ServiceDisplayName
        {
            get
            {
                return UFInterfaces.Properties.Resources.RecipeService_ServiceDisplayName;
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
                return RecipeUAServerInfo.RecipeUAServerInfo.GetServerName();
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
