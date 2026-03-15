using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MSZServiceCMS;

namespace MSZService.ServerCMS
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MSZServiceCSM : IMSZServiceCMS, IDisposable
    {
        #region Declarations
        ServiceHost host;
        MSZService Server;
        String hostName;
        bool isStopping;
        #endregion

        public MSZServiceCSM(MSZService server)
        {
            Server = server;
            InitHostNames();
        }

        #region Server
        void InitHostNames()
        {
            hostName = Dns.GetHostName().ToLower();
        }
        public void HostServer(bool useDiscovery)
        {
            if (host != null)
                return;

            host = new ServiceHost(this);
            host.AddServiceEndpoint(typeof(IMSZServiceCMS), new NetTcpBinding(NetTcpSecurityMode) { ReceiveTimeout = TimeSpan.MaxValue }, 
                String.Format(NetTcpServiceAddress, hostName, Server.ServerPort.ToString()));

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
        SecurityMode netTcpSecurityMode = SecurityMode.None;
        public SecurityMode NetTcpSecurityMode
        {
            get
            {
                return netTcpSecurityMode;
            }
            private set
            {
                netTcpSecurityMode = value;
            }
        }

        String netTcpServiceAddress = "net.tcp://{0}:{1}/MSZService";
        public String NetTcpServiceAddress
        {
            get
            {
                return netTcpServiceAddress;
            }
            private set
            {
                netTcpServiceAddress = value;
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
        }

        #region IScriptServiceCMS
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
        public string Request(MSZRequest request)
        {
            return Server.Request(request);
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
        #endregion
    }
}
