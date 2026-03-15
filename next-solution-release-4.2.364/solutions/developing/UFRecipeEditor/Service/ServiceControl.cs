using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Service;

namespace UFRecipeEditor.Service
{
    internal class ServiceControl : IServiceControl
    {
        #region Declarations
        readonly string applicationName;
        readonly string[] dependencies;
        readonly string filePath;
        readonly string stringConn;
        readonly string userConn;
        readonly string docPath;
        #endregion

        #region Constructors
        public ServiceControl(string applicationName, string filePath, string stringConn, string userConn, string docPath) : 
            this (applicationName, null, filePath, stringConn, userConn, docPath)
        { }

        public ServiceControl(string applicationName, string[] dependencies, string filePath, string stringConn, string userConn, string docPath)
        {
            this.applicationName = applicationName;
            this.dependencies = dependencies;
            this.filePath = filePath;
            this.stringConn = stringConn;
            this.userConn = userConn;
            this.docPath = docPath;
        }
        #endregion

        #region IServiceControl
        public string Name
        {
            get
            {
                return RecipeServiceCMS.RecipeServiceCSMHelpers.GetServiceName(applicationName);
            }
        }

        public string FriendlyName
        {
            get
            {
                return RecipeServiceCMS.RecipeServiceCSMHelpers.GetServiceDisplayNameWhitoutDecoration();
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
            RecipeServiceCMS.RecipeServiceCSMHelpers.InstallService(applicationName, dependencies, filePath, stringConn, userConn, docPath);
        }

        public void Install(string username, string password)
        {
            RecipeServiceCMS.RecipeServiceCSMHelpers.InstallServiceWithLogInInformation(applicationName, username, password, dependencies, filePath, stringConn, userConn, docPath);
        }

        public void Uninstall()
        {
            RecipeServiceCMS.RecipeServiceCSMHelpers.UninstallService(applicationName);
        }

        public void Start()
        {
            RecipeServiceCMS.RecipeServiceCSMHelpers.StartService(applicationName);
        }

        public void Stop()
        {
            RecipeServiceCMS.RecipeServiceCSMHelpers.StopService(applicationName);
        }        

        public void OpenServiceControl()
        {
            RecipeServiceCMS.RecipeServiceCSMHelpers.OpenServiceManager(applicationName, dependencies, filePath, stringConn, userConn, docPath);
        }
        #endregion
    }
}
