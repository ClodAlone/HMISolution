using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Threading;
using System.Threading.Tasks;
using MSZServiceCMS;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.IdentityModel.Configuration;

namespace MSZUtilsWebService.ServerCMS
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MSZUtilsServiceCSM : IMSZWUServiceCMS, IDisposable
    {
        #region Declarations
        ServiceHost host;
        MSZUtilsService Server;
        bool isStopping;
        #endregion

        public MSZUtilsServiceCSM(MSZUtilsService server)
        {
            Server = server;
        }

        #region Server
        public void HostServer()
        {
            if (host != null)
                return;
            host = new ServiceHost(this);
            host.Open();

            if (Environment.UserInteractive && Console.CursorVisible)
            {
                Console.WriteLine("Service Host Server Name: licmng.progea.com - On Port Number: 63916");
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
        MSZUWResponse IMSZWUServiceCMS.Request(MSZUWRequest request)
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
