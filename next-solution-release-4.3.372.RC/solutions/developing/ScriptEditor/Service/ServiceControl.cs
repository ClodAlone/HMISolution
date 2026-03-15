using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Service;

namespace ScriptManager.Service
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
                return ScriptServiceCMS.ScriptServiceCSMHelpers.GetServiceName(applicationName);
            }
        }

        public string FriendlyName
        {
            get
            {
                return ScriptServiceCMS.ScriptServiceCSMHelpers.GetServiceDisplayNameWhitoutDecoration();
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
            ScriptServiceCMS.ScriptServiceCSMHelpers.InstallService(applicationName, dependencies, filePath);
        }

        public void Install(string username, string password)
        {
            ScriptServiceCMS.ScriptServiceCSMHelpers.InstallServiceWithLogInInformation(applicationName, username, password, dependencies, filePath);
        }

        public void Uninstall()
        {
            ScriptServiceCMS.ScriptServiceCSMHelpers.UninstallService(applicationName);
        }

        public void Start()
        {
            ScriptServiceCMS.ScriptServiceCSMHelpers.StartService(applicationName);
        }

        public void Stop()
        {
            ScriptServiceCMS.ScriptServiceCSMHelpers.StopService(applicationName);
        }        

        public void OpenServiceControl()
        {
            ScriptServiceCMS.ScriptServiceCSMHelpers.OpenServiceManager(applicationName, dependencies, filePath);
        }
        #endregion
    }
}
