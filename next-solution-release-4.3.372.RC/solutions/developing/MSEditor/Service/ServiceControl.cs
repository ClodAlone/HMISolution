using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Service;

namespace MSEditor.Service
{
    internal class ServiceControl : IServiceControl
    {
        #region Declarations
        readonly string applicationName;
        readonly string serverConn;
        readonly string stringConn;
        readonly string userConn;
        #endregion

        #region Constructors
        public ServiceControl(string applicationName, string serverConn, string stringConn, string userConn)
        {
            this.applicationName = applicationName;
            this.serverConn = serverConn;
            this.stringConn = stringConn;
            this.userConn = userConn;
        }
        #endregion

        #region IServiceControl
        public string Name
        {
            get
            {
                return MSServerCMS.MSServerCMSHelpers.GetServiceName(applicationName);
            }
        }

        public string FriendlyName
        {
            get
            {
                return MSServerCMS.MSServerCMSHelpers.GetServiceDisplayNameWhitoutDecoration();
            }
        }

        public bool UseCredentialProvider
        {
            get
            {
                return true;
            }
        }

        public void Install()
        {
            MSServerCMS.MSServerCMSHelpers.InstallService(applicationName, serverConn, stringConn, userConn);
        }

        public void Install(string username, string password)
        {
            MSServerCMS.MSServerCMSHelpers.InstallServiceWithLogInInformation(applicationName, username, password, serverConn, stringConn, userConn);
        }

        public void Uninstall()
        {
            MSServerCMS.MSServerCMSHelpers.UninstallService(applicationName);
        }

        public void Start()
        {
            MSServerCMS.MSServerCMSHelpers.StartService(applicationName);
        }

        public void Stop()
        {
            MSServerCMS.MSServerCMSHelpers.StopService(applicationName);
        }        

        public void OpenServiceControl()
        {
            MSServerCMS.MSServerCMSHelpers.OpenServiceManager(applicationName, serverConn, stringConn, userConn);
        }
        #endregion
    }
}
