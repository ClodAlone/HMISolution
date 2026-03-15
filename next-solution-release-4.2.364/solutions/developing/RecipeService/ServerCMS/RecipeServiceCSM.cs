using DocumentManager.ComponentService;
using RecipeServiceCMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RecipeService.ServerCMS
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class RecipeServiceCSM : IRecipeServiceCMS, IDisposable
    {
        #region Declarations
        readonly RecipeService Server;
        readonly RecipeServiceCSMHelpers helper;

        ServiceHost host;
        bool isStopping;
        #endregion

        public RecipeServiceCSM(String connection, RecipeService server)
        {
            Connection = connection;
            Server = server;
            helper = new RecipeServiceCSMHelpers(connection);
        }

        #region Server

        public void HostServer(bool useDiscovery)
        {
            if (host != null)
                return;

            host = new ServiceHost(this);

            host.AddServiceEndpoint(typeof(IRecipeServiceCMS), new NetNamedPipeBinding() { ReceiveTimeout = TimeSpan.MaxValue, MaxReceivedMessageSize = Properties.Settings.Default.MaxReceivedMessageSize },
                helper.GetNetPipeServiceAddress());
            host.AddServiceEndpoint(typeof(IRecipeServiceCMS), new NetTcpBinding(SecurityMode.None) { ReceiveTimeout = TimeSpan.MaxValue, MaxReceivedMessageSize = Properties.Settings.Default.MaxReceivedMessageSize },
                helper.GetNetTcpServiceAddress());

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

        #region IRecipeServiceCMS
        public bool IsStarted()
        {
            return Server.IsStarted;
        }

        public bool IsRunningAsService()
        {
            return Server.IsRunningAsService;
        }
        public bool IsReady(Uri uri, UFRecipeExecutionContext.RecipeCommandType commandType)
        {
            return Server.IsReady(uri, commandType);
        }

        public void Execute(Uri uri, ExecutionMode mode, ExecutionArgs args)
        {
            Server.Execute(uri, mode, args);
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
        #endregion
    }
}
