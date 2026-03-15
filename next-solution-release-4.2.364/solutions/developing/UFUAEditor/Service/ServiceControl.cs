using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Service;

namespace UFUAEditor.Service
{
    internal class ServiceControl : IServiceControl
    {
        #region Declarations
        readonly string applicationName;
        readonly string serverConn;
        readonly string stringConn;
        readonly string userConn;
        readonly string docPath;
        readonly bool useCFR21UserIndenity;
        #endregion

        #region Constructors
        public ServiceControl(string applicationName, string serverConn, string stringConn, string userConn, string docPath, bool useCFR21UserIndenity)
        {
            this.applicationName = applicationName;
            this.serverConn = serverConn;
            this.stringConn = stringConn;
            this.userConn = userConn;
            this.docPath = docPath;
            this.useCFR21UserIndenity = useCFR21UserIndenity;
        }
        #endregion

        #region IServiceControl
        public string Name
        {
            get
            {
                return UFUAServerCMS.UFUAServerCSMHelpers.GetServiceName(applicationName);
            }
        }

        public string FriendlyName
        {
            get
            {
                return UFUAServerCMS.UFUAServerCSMHelpers.GetServiceDisplayNameWhitoutDecoration();
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
            if (useCFR21UserIndenity)
            {
                var username = UFUAServerInfo.UFUAServerInfo.GetCFR21UserName();
                var domainName = UFUAServerInfo.UFUAServerInfo.GetCFR21DomainName();
                if (!String.IsNullOrEmpty(domainName))
                    username = String.Format("{0}\\{1}", domainName, username);
                var password = "ie4HZN8u9uW4NJOdnRNFbMcs9wfWnAaGICcU3qs6fcW8IpbPVp273NEpPMaAlV8B"/* "{45F928C5_1EF2_48D1_9505_14ACf7AD6AF8}" */;
                UFUAServerCMS.UFUAServerCSMHelpers.InstallServiceWithLogInInformation(applicationName, username, password, serverConn, stringConn, userConn, docPath);
            }
            else
                UFUAServerCMS.UFUAServerCSMHelpers.InstallService(applicationName, serverConn, stringConn, userConn, docPath);
        }

        public void Install(string username, string password)
        {
            UFUAServerCMS.UFUAServerCSMHelpers.InstallServiceWithLogInInformation(applicationName, username, password, serverConn, stringConn, userConn, docPath);
        }

        public void Uninstall()
        {
            UFUAServerCMS.UFUAServerCSMHelpers.UninstallService(applicationName);
        }

        public void Start()
        {
            UFUAServerCMS.UFUAServerCSMHelpers.StartService(applicationName);
        }

        public void Stop()
        {
            UFUAServerCMS.UFUAServerCSMHelpers.StopService(applicationName);
        }        

        public void OpenServiceControl()
        {
            UFUAServerCMS.UFUAServerCSMHelpers.OpenServiceManager(applicationName, serverConn, stringConn, userConn, docPath);
        }
        #endregion
    }
}
