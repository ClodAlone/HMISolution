using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ServiceProcess;
using Utilities.Logger;
using MSZServiceCMS;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MSZUtilsWebService
{
    class Program
    {
        const int MAX_TOOLTIP_CHAR_LENGTH = 64;

        private static string logFile;
        static System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        static Timer notifyIconTimer;
        static bool bLogExceptions = false;
        static string[] allowedRequests;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            allowedRequests = Enum.GetNames(typeof(MSZUtilsServiceHelper.RequestType));

            logFile = AppDomain.CurrentDomain.BaseDirectory + "Log\\ServiceLog_";

            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            Utilities.RegistryKeysHelper.ReadLogException(ref bLogExceptions);

            var ServicesToRun = new MSZUtilsService(Properties.Resources.Server);

            var starting = String.Format(Properties.Resources.ServerStarting, Environment.Is64BitProcess ? "64" : "32", DateTime.Now.ToString("HH:mm:ss"));
            LogServer(starting);

#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                    String.Format("DebugMe - {0}", ServicesToRun.ServiceName), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif
            ServicesToRun.StartingService += (ob, ev) =>
            {
                #region SysTray

                notifyIcon.Visible = true;
                notifyIcon.Text = ServicesToRun.Title?.Substring(0, Math.Min(ServicesToRun.Title.Length, MAX_TOOLTIP_CHAR_LENGTH - 1));
                notifyIcon.Icon = Properties.Resources.CommNeutro;
                notifyIcon.ShowBalloonTip(5000, ServicesToRun.Title, starting, System.Windows.Forms.ToolTipIcon.Info);

                bool bBlink = false;
                var lastStatus = ServicesToRun.CommunicationStatus;
                notifyIconTimer = new Timer((o) =>
                {
                    lock (notifyIconTimer)
                    {
#if !DEBUG
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            var ret = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxY+APFmQPtOWSUaMJjYrwAg=="/* DBG */);
                            if (!ret)
                                System.Environment.Exit(-10);
                        }
#endif
                        if (notifyIcon == null)
                            return;

                        var status = ServicesToRun.CommunicationStatus;
                        if (status != lastStatus)
                        {
                            lastStatus = status;
                            notifyIcon.ShowBalloonTip(5000, ServicesToRun.Title,
                                String.Format(Properties.Resources.ServerStatusChanged, ServicesToRun.StatusText),
                                System.Windows.Forms.ToolTipIcon.Info);
                        }
                        else
                            if (ServicesToRun.BalloonMessage.Length > 0)
                        {
                            notifyIcon.ShowBalloonTip(30000, ServicesToRun.Title,
                            ServicesToRun.BalloonMessage,
                            ServicesToRun.BalloonIcon);
                            ServicesToRun.BalloonMessage = String.Empty;
                        }

                        var prevIcon = notifyIcon.Icon;
                        notifyIcon.Icon = bBlink ?
                            (status ? Properties.Resources.CommOK : Properties.Resources.CommError)
                                : Properties.Resources.CommNeutro;
                        notifyIcon.Visible = true;
                        notifyIcon.Text = ServicesToRun.Title?.Substring(0, Math.Min(ServicesToRun.Title.Length, MAX_TOOLTIP_CHAR_LENGTH - 1));
                        bBlink = !bBlink;
                        if (prevIcon != null)
                            prevIcon.Dispose();
                    }
                }, null, 2000, 1000);

                #endregion
            };

            ServicesToRun.StoppingService += (o, e) =>
            {
                LogServer(string.Format(Properties.Resources.ServerStopping, DateTime.Now.ToString("HH:mm:ss")));

                if (notifyIcon != null)
                {
                    notifyIcon.ShowBalloonTip(5000, ServicesToRun.Title,
                        string.Format(Properties.Resources.ServerStopping, DateTime.Now.ToString("HH:mm:ss")),
                        System.Windows.Forms.ToolTipIcon.Info);
                }

                lock (notifyIconTimer)
                {
                    notifyIconTimer.Dispose();
                    notifyIcon.Dispose();
                    notifyIcon = null;
                }

                LogServer(Properties.Resources.ServerStopped);
            };

            bool isRedirected;

            try
            {
                isRedirected = Console.CursorVisible && false;
            }
            catch
            {
                isRedirected = true;
            }

            ServicesToRun.RequestService += (o, e) =>
            {
                int continuationPoint = (e as MSZUWResponseArgs).MSZUtilsWRequest.ContinuationPoint;
                List<string> response = (e as MSZUWResponseArgs).MSZUtilsWRequest.Response;
                MSZUWRequest clientRequest = (e as MSZUWResponseArgs).ClientRequest;
                try
                {
                    StringBuilder log = new StringBuilder($"RequestType: {MSZUtilsService.DecryptString(clientRequest.RequestType)}" +
                        $" ContinuationPoint: {continuationPoint}" +
                        $" Request: {MSZUtilsService.DecryptString(clientRequest.Request)}");
#if DEBUG
                    if(response != null)
                    {
                        response.ForEach(x =>
                        {
                            try
                            {
                                log.Append($"{Environment.NewLine}{MSZUtilsService.DecryptString(x)}");
                            }
                            catch
                            {
                            }
                        });
                    }
#endif
                    if (!isRedirected)
                    {
                        Console.WriteLine(log);
                        Console.WriteLine(Properties.Resources.PressEnter);
                    }

                    LogServer(log.ToString());
                }
                catch (Exception ex)
                {
                    LogServer("Bad Request Parameters");
                }

                if (!allowedRequests.Contains(MSZUtilsService.DecryptString(clientRequest.RequestType)))
                {
                    LogServer("Bad Request Type");
                }
            };

#if !DEBUG
            if (new List<String>(args).Contains("-noservice"))
#endif
            {
                var callingProcessTerminated = new ManualResetEvent(false);
                if (!isRedirected && Console.In != System.IO.StreamReader.Null)
                {
                    bool isStopping = false;
                    Console.TreatControlCAsInput = false;
                    Console.CancelKeyPress += (s, e) =>
                    {
                        e.Cancel = true;
                        if (isStopping)
                            return;
                        isStopping = true;

                        ServicesToRun.StopService();
                        callingProcessTerminated.Set();
                    };

                    ServicesToRun.StartService(args);
                    Console.WriteLine(Properties.Resources.PressEnter);
                }
                else
                    ServicesToRun.StartService(args);

                callingProcessTerminated.WaitOne();
            }
#if !DEBUG
            else
                ServiceBase.Run(ServicesToRun);
#endif
        }

        internal static void LogServer(string logmessage)
        {
            if (!Properties.Settings.Default.IsServerLogEnabled)
                return;
            string currlog = logFile + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            string curfolder = System.IO.Path.GetDirectoryName(currlog);
            if (!System.IO.Directory.Exists(curfolder))
                System.IO.Directory.CreateDirectory(curfolder);

            using (System.IO.StreamWriter writeFile = new System.IO.StreamWriter(currlog, true))
            {
                writeFile.WriteLine(logmessage);
            }
        }

#if !DEBUG
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogServer(Properties.Resources.LoggerSource + " - " + e.ExceptionObject.ToString() + " - " + System.Diagnostics.EventLogEntryType.Error + " - " + LoggerDestination.License);
            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show(e.ExceptionObject.ToString());
            }
        }
#endif
    }
}
