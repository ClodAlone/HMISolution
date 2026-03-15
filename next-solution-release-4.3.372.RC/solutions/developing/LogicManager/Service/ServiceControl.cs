using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Service;

namespace LogicManager.Service
{
    internal class ServiceControl : IServiceControl
    {
        #region Declarations
        readonly string applicationName;
        readonly string[] dependencies;
        readonly string filePath;
        #endregion

        #region Constructors
        public ServiceControl(string applicationName, string[] dependencies, string filePath)
        {
            this.applicationName = applicationName;
            this.dependencies = dependencies;
            this.filePath = filePath;
        }
        #endregion

        #region IServiceControl
        public string Name
        {
            get
            {
                return LogicServiceCMS.LogicServiceCSMHelpers.GetServiceName(applicationName);
            }
        }

        public string FriendlyName
        {
            get
            {
                return LogicServiceCMS.LogicServiceCSMHelpers.GetServiceDisplayNameWhitoutDecoration();
            }
        }

        public bool UseCredentialProvider
        {
            get
            {
                return false;
            }
        }

        public void Install()
        {
            LogicServiceCMS.LogicServiceCSMHelpers.InstallService(applicationName, dependencies, filePath);
        }

        public void Install(string username, string password)
        {
            LogicServiceCMS.LogicServiceCSMHelpers.InstallServiceWithLogInInformation(applicationName, username, password, dependencies, filePath);
        }

        public void Uninstall()
        {
            LogicServiceCMS.LogicServiceCSMHelpers.UninstallService(applicationName);
        }

        public void Start()
        {
            LogicServiceCMS.LogicServiceCSMHelpers.StartService(applicationName);
        }

        public void Stop()
        {
            LogicServiceCMS.LogicServiceCSMHelpers.StopService(applicationName);
        }        

        public void OpenServiceControl()
        {
            LogicServiceCMS.LogicServiceCSMHelpers.OpenServiceManager(applicationName, dependencies, filePath);
        }
        #endregion
    }
}
