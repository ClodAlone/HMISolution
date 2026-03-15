using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using MSModel;
using System.Timers;
using System.Threading;
using MSServerCMS;

namespace MSServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class MSServerCMS : IMSServerCMS, IDisposable
    {
        #region Declarations
        readonly MSServer Server;
        readonly MSServerCMSHelpers helper;

        ServiceHost host;
        bool isStopping;
        #endregion

        public MSServerCMS(String connection, MSServer server)
        {
            Connection = connection;
            Server = server;
            helper = new MSServerCMSHelpers(connection);
        }

        #region Server

        public void HostServer(bool useDiscovery)
        {
            if (host != null)
                return;

            host = new ServiceHost(this);

            host.AddServiceEndpoint(typeof(IMSServerCMS), new NetNamedPipeBinding() { ReceiveTimeout = TimeSpan.MaxValue },
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

        #region IMSServerCMS Members

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
                Server.StopServerApplication();
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

        public bool UpdateScheduler(ChangedSchedArgs sched)
        {
            var se = sched as ChangedSchedArgs;
            if (se != null)
            {
                Server.UpdateScheduledAction(se);
                return se.result;
            }
            return false;
        }

        public String GetApplicationName()
        {
            return Server.application.ApplicationName;
        }
        #endregion

    }
}
