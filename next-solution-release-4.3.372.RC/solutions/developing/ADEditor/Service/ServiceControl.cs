using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Service;

namespace ADEditor.Service
{
    internal class ServiceControl : IServiceControl
    {
        #region Declarations
        readonly string applicationName;
        readonly string serverConn;
        readonly string stringConn;
        readonly string userConn;
        readonly string srvrConfigId;
        #endregion

        #region Constructors
        public ServiceControl(string applicationName, string serverConn, string stringConn, string userConn, string srvrConfigId)
        {
            this.applicationName = applicationName;
            this.serverConn = serverConn;
            this.stringConn = stringConn;
            this.userConn = userConn;
            this.srvrConfigId = srvrConfigId;
        }
        #endregion

        #region IServiceControl
        public string Name
        {
            get
            {
                return ADServerCMS.ADServerCSMHelpers.GetServiceName(applicationName);
            }
        }

        public string FriendlyName
        {
            get
            {
                return ADServerCMS.ADServerCSMHelpers.GetServiceDisplayNameWhitoutDecoration();
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
            ADServerCMS.ADServerCSMHelpers.InstallService(applicationName, serverConn, stringConn, userConn, srvrConfigId);
        }

        public void Install(string username, string password)
        {
            ADServerCMS.ADServerCSMHelpers.InstallServiceWithLogInInformation(applicationName, username, password, serverConn, stringConn, userConn, srvrConfigId);
        }

        public void Uninstall()
        {
            ADServerCMS.ADServerCSMHelpers.UninstallService(applicationName);
        }

        public void Start()
        {
            ADServerCMS.ADServerCSMHelpers.StartService(applicationName);
        }

        public void Stop()
        {
            ADServerCMS.ADServerCSMHelpers.StopService(applicationName);
        }        

        public void OpenServiceControl()
        {
            ADServerCMS.ADServerCSMHelpers.OpenServiceManager(applicationName, serverConn, stringConn, userConn);
        }
        #endregion
    }
}
