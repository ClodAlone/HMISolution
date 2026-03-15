using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SystemTrayApp
{
    public class SysTrayApp : System.Windows.Forms.Form
    {
        #region Declarations
        const int MAX_TOOLTIP_CHAR_LENGTH = 64;
        System.Windows.Forms.NotifyIcon trayIcon = new System.Windows.Forms.NotifyIcon();
        Timer trayIconTimer;

        readonly UFUAServerCMS.UFUAServerCSMHelpers serverCMSHelper;
        readonly String title;
        #endregion

        #region Constuctors
        public SysTrayApp(String instanceId)
        {
            serverCMSHelper = new UFUAServerCMS.UFUAServerCSMHelpers(instanceId);

            title = serverCMSHelper.GetApplicationName();
            if (title == null)
                title = UFUAServerCMS.UFUAServerCSMHelpers.GetServiceDisplayNameWhitoutDecoration();
            trayIcon.Text = title?.Substring(0, Math.Min(title.Length, MAX_TOOLTIP_CHAR_LENGTH - 1));
            trayIcon.Icon = Properties.Resources.CommNeutro;
            trayIcon.Visible = true;

            if (!serverCMSHelper.IsServerRunningAsService)
            { 
                var trayMenu = new System.Windows.Forms.ContextMenu();
                trayMenu.MenuItems.Add(Properties.Resources.MenuItemExit, OnExit);
                trayIcon.ContextMenu = trayMenu;
            }

            bool bBlink = false;
            var lastStatus = serverCMSHelper.IsServerStateRunning();
            var listAlerts = new List<String>();
            trayIconTimer = new Timer((o) =>
            {
                string alertMessage = null;
                if (Monitor.TryEnter(trayIconTimer))
                {
                    try
                    {
                        if (trayIcon == null)
                            return;

                        if (!serverCMSHelper.IsServerRunning)
                            System.Windows.Forms.Application.Exit();

                        var status = serverCMSHelper.IsServerStateRunning();
                        if (status != lastStatus)
                        {
                            lastStatus = status;
                            var statusText = serverCMSHelper.GetServerStatus();
                            if (!String.IsNullOrEmpty(statusText))
                            {
                                trayIcon.ShowBalloonTip(5000, title,
                                    String.Format(Properties.Resources.ServerStatusChanged, statusText),
                                    System.Windows.Forms.ToolTipIcon.Info);
                            }
                        }
                        else
                        {
                            var balloonInfo = serverCMSHelper.GetBalloonMessage(bClear: true);
                            if (balloonInfo != null && balloonInfo.Message.Length > 0)
                            {
                                trayIcon.ShowBalloonTip(30000, title,
                                    balloonInfo.Message,
                                    (System.Windows.Forms.ToolTipIcon)balloonInfo.IconType);
                            }
                        }

                        var prevIcon = trayIcon.Icon;
                        trayIcon.Icon = bBlink ?
                            (status ? Properties.Resources.CommOK : Properties.Resources.CommError)
                                : Properties.Resources.CommNeutro;
                        trayIcon.Visible = true;
                        trayIcon.Text = title?.Substring(0, Math.Min(title.Length, MAX_TOOLTIP_CHAR_LENGTH - 1));
                        bBlink = !bBlink;
                        if (prevIcon != null)
                            prevIcon.Dispose();

                        alertMessage = serverCMSHelper.GetAlertMessage(bClear: true);
                        if (!String.IsNullOrEmpty(alertMessage) && !listAlerts.Contains(alertMessage))
                            listAlerts.Add(alertMessage);
                        else
                            alertMessage = null;
                    }
                    finally
                    {
                        Monitor.Exit(trayIconTimer);
                    }

                    if (!String.IsNullOrEmpty(alertMessage))
                    {
                        System.Windows.Forms.MessageBox.Show(alertMessage, title,
                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        lock (trayIconTimer)
                            listAlerts.Remove(alertMessage);
                    }
                }
            }, null, 2000, 1000);
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
                lock (trayIconTimer)
                {
                    if (trayIcon != null)
                    {
                        trayIconTimer.Dispose();
                        trayIcon.Dispose();
                        trayIcon = null;
                    }

                    if (serverCMSHelper != null)
                        serverCMSHelper.Dispose();
                }
            }

            base.Dispose(isDisposing);
        }
        #endregion

        #region Events
        void OnExit(object sender, EventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show(String.Format(Properties.Resources.AskToExitContent, title),
                Properties.Resources.AskToExitTitle, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;

            if (serverCMSHelper != null)
                serverCMSHelper.StopServer();
        }
        #endregion
    }
}
