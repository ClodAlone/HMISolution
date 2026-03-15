using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using System.Timers;
using System.Threading.Tasks;
using System.Threading;
using ADServerCMS;

namespace ADServer.ServerCSM
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class ADServerCSM : IADServerCMS, IDisposable
    {
        #region Declarations
        readonly ADServer Server;
        readonly ADServerCSMHelpers helper;

        ServiceHost host;
        bool isStopping;
        #endregion

        public ADServerCSM(String connection, ADServer server)
        {
            Connection = connection;
            Server = server;
            helper = new ADServerCSMHelpers(connection);
        }

        #region Server

        public void HostServer(bool useDiscovery)
        {
            if (host != null)
                return;

            host = new ServiceHost(this);

            host.AddServiceEndpoint(typeof(IADServerCMS), new NetNamedPipeBinding() { ReceiveTimeout = TimeSpan.MaxValue },
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

        #region IADServerCMS

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
                Dispose();
                Server.StopService();
                Thread.Sleep(1000);
                Environment.Exit(1);
            });
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

        public String GetApplicationName()
        {
            return Server.application.ApplicationName;
        }
        #endregion
    }
}
