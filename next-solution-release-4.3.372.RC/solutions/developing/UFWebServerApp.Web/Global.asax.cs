using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace UFWebServerApp.Web
{
    public class Global : System.Web.HttpApplication
    {
        static public String ConnectionStringData { get; protected set; }
        static public String ConnectionStringEvent { get; protected set; }

        static public Uri defaultUri { get; protected set; }
        static public String ClientSessionName { get; protected set; }
        static public int RefreshPollingTime { get; protected set; }

        static ScreenManager.ComponentService.ScreenManagerComponent screenComponent = new ScreenManager.ComponentService.ScreenManagerComponent();
        static public ScreenManager.ComponentService.ScreenManagerComponent ScreenComponent
        {
            get
            {
                return screenComponent;
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            var rootWebConfig =
                            System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");

            var settings = rootWebConfig.AppSettings.Settings;
            var connectionStringData = settings["ConnectionStringData"];
            if (connectionStringData != null)
                ConnectionStringData = connectionStringData.Value;

            var connectionStringEvent = settings["ConnectionStringEvent"];
            if (connectionStringEvent != null)
                ConnectionStringEvent = connectionStringEvent.Value;

            var uri = settings["Uri"];
            if (uri != null)
                defaultUri = new Uri(uri.Value, UriKind.RelativeOrAbsolute);
            var clientSessionName = settings["ClientSessionName"];
            if (clientSessionName != null)
                ClientSessionName = clientSessionName.Value;
            var refreshPollingTime = settings["RefreshPollingTime"];
            if (refreshPollingTime != null)
            {
                try
                {
                    RefreshPollingTime = Convert.ToInt32(refreshPollingTime.Value);
                }
                catch (Exception ex)
                {

                }
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}