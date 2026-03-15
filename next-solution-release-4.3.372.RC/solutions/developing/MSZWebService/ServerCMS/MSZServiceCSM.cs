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

namespace MSZWebService.ServerCMS
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MSZServiceCSM : IMSZWServiceCMS, IDisposable
    {
        #region Declarations
        ServiceHost host;
        MSZService Server;
#if !DEBUG
        String wHostName = "UXS2WhNPVnwZg0iWAgn9/SVomrJFXjpHre4ikv0d5fE="; // collect.progea.com
#else
        // *** Command Prompt (Administrator) ***
        // https://docs.microsoft.com/en-US/dotnet/framework/wcf/feature-details/configuring-http-and-https?redirectedfrom=MSDN
        // netsh http add urlacl url=http://+:63917/MSZWService/ user=PROGEA\<UserName>
        // 

        String wHostName = "a++o5Q04867wsaqAIBGKIw=="; // localhost
#endif
        String wServerPort = "DuBKGKqkNu9oG9/M843aSA=="; // 63917
        bool isStopping;
        #endregion

        public MSZServiceCSM(MSZService server)
        {
            Server = server;
        }

        #region Server
        public void HostServer(bool useDiscovery)
        {
            if (host != null)
                return;
            string hostaddress = String.Format(NetTcpServiceAddress, 
                WPFUtilities.CryptString.CryptString.DecryptString(wHostName), 
                WPFUtilities.CryptString.CryptString.DecryptString(wServerPort));
            host = new ServiceHost(this);
            host.AddServiceEndpoint(typeof(IMSZWServiceCMS), 
                new BasicHttpBinding() { ReceiveTimeout = TimeSpan.MaxValue },
                hostaddress);

            if (Environment.UserInteractive && Console.CursorVisible)
            {
                Console.WriteLine("Host Server: " + WPFUtilities.CryptString.CryptString.DecryptString(wHostName) + " - Port Opened: " + WPFUtilities.CryptString.CryptString.DecryptString(wServerPort));
            }

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

        String netTcpServiceAddress = "http://{0}:{1}/MSZWService";
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
        public string Request(MSZWRequest request)
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
