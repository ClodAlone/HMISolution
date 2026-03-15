using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SystemTrayService
{
    public class SysTrayApp : System.Windows.Forms.Form
    {
        #region Declarations
        //const int MAX_TOOLTIP_CHAR_LENGTH = 64;
        //System.Windows.Forms.NotifyIcon trayIcon = new System.Windows.Forms.NotifyIcon();

        readonly String hostName;
        readonly AnnouncementServiceHost announcementServiceHost;
        #endregion

        #region Constuctors
        public SysTrayApp()
        {
            //var title = UFUAServerCSMHelpers.GetServiceDisplayNameWhitoutDecoration();
            //trayIcon.Text = title?.Substring(0, Math.Min(title.Length, MAX_TOOLTIP_CHAR_LENGTH - 1));
            //trayIcon.Icon = Properties.Resources.CommNeutro;
            //trayIcon.Visible = false;

            //var trayMenu = new System.Windows.Forms.ContextMenu();
            //trayMenu.MenuItems.Add(Properties.Resources.MenuItemExit, OnExit);
            //trayIcon.ContextMenu = trayMenu;

            hostName = System.Net.Dns.GetHostName();
            announcementServiceHost = new AnnouncementServiceHost();
            announcementServiceHost.AddService(typeof(UFUAServerCMS.IUFUAServerCMS));
#if !CONNEXT
            announcementServiceHost.AddService(typeof(ADServerCMS.IADServerCMS));
            announcementServiceHost.AddService(typeof(MSServerCMS.IMSServerCMS));
            announcementServiceHost.AddService(typeof(LogicServiceCMS.ILogicServiceCMS));
            announcementServiceHost.AddService(typeof(RecipeServiceCMS.IRecipeServiceCMS));
            announcementServiceHost.AddService(typeof(ScriptServiceCMS.IScriptServiceCMS));
#endif
            announcementServiceHost.ServiceOnline += ServiceOnline;
            announcementServiceHost.ServiceOffline += ServiceOffline;
            announcementServiceHost.Start();

            ThreadPool.QueueUserWorkItem((o) => 
            {
                announcementServiceHost.DiscoverActiveServices();
            });
        }
        #endregion

        #region Overrides
        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            base.OnLoad(e);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                //if (trayIcon != null)
                //{
                //    trayIcon.Dispose();
                //    trayIcon = null;
                //}

                announcementServiceHost.Stop();
                announcementServiceHost.ServiceOnline -= ServiceOnline;
                announcementServiceHost.ServiceOffline -= ServiceOffline;
            }

            base.Dispose(isDisposing);
        }
        #endregion

        #region Events
        void ServiceOnline(object sender, ServiceArgs e)
        {
            if (String.Compare(e.HostName, hostName, true) != 0 || e.SchemaType != Uri.UriSchemeNetPipe)
                return;

            if (e.ServiceType == typeof(UFUAServerCMS.IUFUAServerCMS))
                RunSysTray(UFUAServerInfo.UFUAServerInfo.GetSysTrayProcessName(), e.InstanceId);
#if !CONNEXT
            else if (e.ServiceType == typeof(ADServerCMS.IADServerCMS))
                RunSysTray(ADServerInfo.ADServerInfo.GetSysTrayProcessName(), e.InstanceId);
            else if (e.ServiceType == typeof(MSServerCMS.IMSServerCMS))
                RunSysTray(MSServerInfo.MSServerInfo.GetSysTrayProcessName(), e.InstanceId);
            else if (e.ServiceType == typeof(LogicServiceCMS.ILogicServiceCMS))
                RunSysTray(LogicServiceCMS.LogicServiceCSMHelpers.GetSysTrayProcessName(), e.InstanceId);
            else if (e.ServiceType == typeof(RecipeServiceCMS.IRecipeServiceCMS))
                RunSysTray(RecipeServiceCMS.RecipeServiceCSMHelpers.GetSysTrayProcessName(), e.InstanceId);
            else if (e.ServiceType == typeof(ScriptServiceCMS.IScriptServiceCMS))
                RunSysTray(ScriptServiceCMS.ScriptServiceCSMHelpers.GetSysTrayProcessName(), e.InstanceId);
#endif
        }

        void ServiceOffline(object sender, ServiceArgs e)
        { }

        void RunSysTray(string fileName, string instanceId)
        {
            using (var process = new System.Diagnostics.Process())
            {
                var callingMainAssembly = Assembly.GetExecutingAssembly();
                if (callingMainAssembly != null)
                    fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), fileName);
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = String.Format("-instanceId={0}", instanceId);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.RedirectStandardInput = false;
                process.StartInfo.CreateNoWindow = true;

                try
                {
                    process.Start();
                }
                catch
                { }
            }
        }
        #endregion
    }
}
