using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFUAServerCMS;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using System.Timers;
using System.Threading.Tasks;
using System.Threading;

namespace UFUAServerBase.ServerCSM
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class UFUAServerCSM : IUFUAServerCMS, IDisposable
    {
        #region Declarations
        readonly UFUAServer Server;
        readonly UFUAServerCSMHelpers helper;

        ServiceHost host;
        bool isStopping;
        #endregion

        public UFUAServerCSM(String connection, UFUAServer server)
        {
            Connection = connection;
            Server = server;
            helper = new UFUAServerCSMHelpers(connection);
        }

        #region Server

        public void HostServer(bool useDiscovery)
        {
            if (host != null)
                return;

            host = new ServiceHost(this);

            host.AddServiceEndpoint(typeof(IUFUAServerCMS), new NetNamedPipeBinding() { ReceiveTimeout = TimeSpan.MaxValue },
                helper.GetNetPipeServiceAddress());

            #region Discovery Settings

            if (useDiscovery)
            {
                var discoveryBehavior = new ServiceDiscoveryBehavior();
                host.Description.Behaviors.Add(discoveryBehavior);
                host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());
            }

            #endregion

            host.Open();
        }

        #endregion

        #region Properties

        private string _Connection;
        public string Connection
        {
            get { return _Connection; }
            private set
            {
                _Connection = value;
            }
        }
        
        #endregion

        public void Dispose()
        {
            if (host != null)
            {
                host.Close();
                host = null;
            }

            helper.Dispose();
        }

        #region IUFUAServerCMS
        public bool IsActiveServer()
        {
            return Server.IsActiveServer;
        }

        public bool IsStarted()
        {
            return Server.IsStarted;
        }

        public bool IsRunningAsService()
        {
            return Server.IsRunningAsService;
        }

        public bool IsStopping()
        {
            return isStopping;
        }

        public void StopServer()
        {
            if (isStopping)
                return;
            isStopping = true;

            var task1 = Task.Factory.StartNew(() =>
            {
                //Dispose();
                Server.StopService();
                Thread.Sleep(1000);
                Environment.Exit(1);
            });
        }


        public bool EnableScriptDebugging(Guid id, bool bEnable)
        {
            return Server.EnableScriptDebugging(id, bEnable);
        }

        public List<string> ScriptSynchronizing(Guid id, List<string> data)
        {
            return Server.ScriptSynchronizing(id, data);
        }

        public bool IsServerStateRunning()
        {
            return Server.CommunicationStatus;
        }

        public String GetServerStatus()
        {
            return Server.StatusText;
        }

        public UFProcessServiceCMS.BalloonInfo GetBalloonMessage(bool bClear)
        {
            var balloonInfo = new UFProcessServiceCMS.BalloonInfo()
            {
                Message = Server.BalloonMessage,
                IconType = (int)Server.BalloonIcon
            };

            if (bClear)
                Server.BalloonMessage = string.Empty;

            return balloonInfo;
        }

        public String GetAlertMessage(bool bClear)
        {
            var alertMessage = Server.AlertMessage;
            if (bClear)
                Server.AlertMessage = string.Empty;

            return alertMessage;
        }

        public String GetApplicationName()
        {
            return Server.application.ApplicationName;
        }

        
        #endregion
    }
}
