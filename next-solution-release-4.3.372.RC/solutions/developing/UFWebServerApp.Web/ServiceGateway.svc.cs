using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Security;

namespace UFWebServerApp.Web
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ServiceGateway
    {
        const String sessionName = "sessionObject";

        ScreenSinkServiceSession GetSession(bool bCreate = false)
        {
            var ret = HttpContext.Current.Session[sessionName] as ScreenSinkServiceSession;
            if (ret == null && bCreate)
            {
                ret = new ScreenSinkServiceSession();
                HttpContext.Current.Session[sessionName] = ret;
            }

            return ret;
        }

        void RemoveSession()
        {
            var ret = HttpContext.Current.Session[sessionName] as ScreenSinkServiceSession;
            if (ret != null)
            {
                ret.Dispose();
                HttpContext.Current.Session.Remove(sessionName);
            }
        }

        private void CloseCurrentUri(String clientId)
        {
            var session = GetSession();
            if (session == null)
                return;
            lock (session.lockObject)
            {
                if (session.mapIdToUIElements.ContainsKey(clientId))
                    session.mapIdToUIElements.Remove(clientId);
            }

            session.ScreenSink.CloseUri(session.currentPage);
        }

        [OperationContract]
        public String Register(int width, int height)
        {
            var session = GetSession(true);

            var dc = ServiceSecurityContext.Current;

            session.projectDocument = UFProjectManager.UFProjectDocument.FromFile(Global.defaultUri.OriginalString, null);
            session.currentPage = session.projectDocument.GetStartupScreen(Global.ScreenComponent);

            if (session.currentPage == null)
                throw new ArgumentNullException("Missing default uri to load !");

            if (Global.ClientSessionName != null)
                session.ScreenSink.ClientSessionName = Global.ClientSessionName;
            if (Global.RefreshPollingTime != 0)
                session.ScreenSink.RefreshPollingTime = Global.RefreshPollingTime;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var mbsu = Membership.GetUser(HttpContext.Current.User.Identity.Name);
                session.ScreenSink.Password = mbsu.GetPassword();
                session.ScreenSink.User = HttpContext.Current.User.Identity.Name;

                session.ScreenSink.ClientSessionName = String.Format("{0}-{1}",
                    session.ScreenSink.ClientSessionName, HttpContext.Current.User.Identity.Name);
            }
            var UIElements = session.ScreenSink.OpenUri(session.currentPage, width, height,
                session.projectDocument.fileSystemProviderBase);

            String clientId = Guid.NewGuid().ToString();
            lock (session.lockObject)
            {
                session.mapIdToUIElements.Add(clientId, UIElements);
            }
            return clientId;
        }

        [OperationContract]
        public void Unregister(String clientId)
        {
            CloseCurrentUri(clientId);
            RemoveSession();
        }
    }
}
