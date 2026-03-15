using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.AssemblyResolver;
using Utilities.Logger;

namespace UFWebClient.Service
{
    static class Program
    {
        const int MAX_TOOLTIP_CHAR_LENGTH = 64;

        static readonly AssemblyResolver assemblyResolver = new AssemblyResolver();
        static bool bLogExceptions = true;
        static System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        static Timer notifyIconTimer;

        static readonly ILog logServer = LogManager.GetLogger(Properties.Resources.Server);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static void Main(params String[] args)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            RegistryKeysHelper.ReadLogException(ref bLogExceptions);

            UFWebClientService ServicesToRun = new UFWebClientService();

            XmlConfigurator.Configure();

            var starting = String.Format(Properties.Resources.ServerStarting, Environment.Is64BitProcess ? "64" : "32");
            logServer.Info(starting);

            DevExpress.Xpf.Bars.ElementRegistrator.GlobalSkipUniquenessCheck = true;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !", "DebugMe", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif
            LoadRequiredAssemblies();
            ServicesToRun.StartingService += (ob, ev) =>
                {
                    #region SysTray

                    notifyIcon.Visible = true;
                    notifyIcon.Text = ServicesToRun.Title?.Substring(0, Math.Min(ServicesToRun.Title.Length, Math.Min(ServicesToRun.Title.Length, MAX_TOOLTIP_CHAR_LENGTH - 1)));
                    notifyIcon.Icon = Properties.Resources.CommNeutro;
                    notifyIcon.ShowBalloonTip(5000, ServicesToRun.Title, starting, System.Windows.Forms.ToolTipIcon.Info);

                    bool bBlink = false;
                    var lastStatus = ServicesToRun.CommunicationStatus;
                    notifyIconTimer = new Timer((o) =>
                    {
                        lock (notifyIconTimer)
                        {
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

                    ServicesToRun.StoppingService += (o, e) =>
                    {
                        logServer.Info(Properties.Resources.ServerStopping);

                        notifyIcon.ShowBalloonTip(5000, ServicesToRun.Title,
                            Properties.Resources.ServerStopping,
                            System.Windows.Forms.ToolTipIcon.Info);

                        notifyIconTimer.Dispose();
                        notifyIcon.Dispose();

                        logServer.Info(Properties.Resources.ServerStopped);
                    };

                    #endregion
                };

#if !DEBUG
            if (new List<String>(args).Contains("-noservice"))
#endif
            {
                bool isRedirected;

                try
                {
                    isRedirected = Console.CursorVisible && false;
                }
                catch
                {
                    isRedirected = true;
                }

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
                    Console.WriteLine("Press Ctrl+C or Ctrl+Break to stop the service");
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

        static void LoadRequiredAssemblies()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.RequiredAssemblies))
            {
                var array = Properties.Settings.Default.RequiredAssemblies.Split(';');
                foreach (var assembly in array)
                {
                    try
                    {
                        Assembly.Load(assembly);
                    }
                    catch (Exception ex)
                    {
                        logServer.Debug(ex.Message);
                    }
                }
            }
        }

#if !DEBUG
        readonly static Mindscape.Raygun4Net.RaygunClient _raygunClient = new Mindscape.Raygun4Net.RaygunClient("oAwabllIR18uGc0lo+bwzQ==");
#endif
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#if !DEBUG
            if (bLogExceptions)
            {
                _raygunClient.User = System.Environment.UserName;
                _raygunClient.ApplicationVersion = Utilities.AssemblyInfo.FileVersion;
                _raygunClient.Send(e.ExceptionObject as Exception);
            }
#endif
            try
            {
                UFWebClientService.RemoveAllSessions();
                ScreenSinkServiceSession.SessionCounter?.Dispose();
            }
            catch (Exception ex)
            {
                logServer.Debug(String.Format(Properties.Resources.Exception, e.ExceptionObject), e.ExceptionObject as Exception);
            }

            logServer.Fatal(String.Format(Properties.Resources.Exception, e.ExceptionObject),
                e.ExceptionObject as Exception);
            Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ExceptionObject.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show(e.ExceptionObject.ToString());
            }
        }
    }
}
