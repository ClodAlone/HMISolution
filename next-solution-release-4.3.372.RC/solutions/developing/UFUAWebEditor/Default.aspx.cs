using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace UFUAWebEditor
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Theme
        /* Page PreInit */
        protected void Page_PreInit(object sender, EventArgs e)
        {
            string themeName = "Glass";
            if (Page.Request.Cookies[SiteMaster.GetThemeCookieName()] != null)
            {
                themeName = Page.Request.Cookies[SiteMaster.GetThemeCookieName()].Value;
            }

            string clientScriptBlock = "var DXCurrentThemeCookieName = \"" + SiteMaster.GetThemeCookieName() + "\";";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DXCurrentThemeCookieName", clientScriptBlock, true);

            this.Theme = themeName;
        }
        #endregion

        protected void cbFileProcess_Callback(object source, DevExpress.Web.CallbackEventArgs e)
        {
            using (ServiceController svcController = new ServiceController("UFUAServer"))
            {
                bool bServiceFound = true;
                try
                {
                    bool bRunning = svcController.Status == ServiceControllerStatus.Running;
                }
                catch (Exception ex)
                {
                    bServiceFound = false;
                }

                if (bServiceFound)
                {
                    svcController.Stop();
                    svcController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    svcController.Start();
                }
                else
                {
                    var processes = System.Diagnostics.Process.GetProcessesByName("MovNExTServer.exe");
                    Parallel.ForEach(processes, p =>
                    {
                        p.Close();
                    });
                    Configuration rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
                    KeyValueConfigurationCollection settings = rootWebConfig.AppSettings.Settings;
                    var server = settings["server"];
                    var database = settings["database"];
                    String commandLine = null;
                    if (database != null)
                        commandLine += String.Format(" SQLDatabase={0}", database.Value);
                    if (server != null)
                        commandLine += String.Format(" SQLServer={0}", server.Value);
                    System.Diagnostics.Process.Start("MovNExTServer.exe", commandLine);
                }
            }
        }

        protected void OnTimerCallback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            DigitalGauge.Text = new Random().NextDouble().ToString();
        }
    }
}
