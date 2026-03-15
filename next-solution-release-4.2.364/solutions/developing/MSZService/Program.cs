using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using log4net;
using log4net.Config;
using MSZServiceCMS;
using System.ServiceProcess;
using Utilities.Logger;

namespace MSZService
{
    class Program
    {
        const int MAX_TOOLTIP_CHAR_LENGTH = 64;

        static System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        static Timer notifyIconTimer;
        static bool bLogExceptions = true;

        static readonly ILog logServer = Logger.GetDestinationLog(LoggerDestination.License);

        static int maxClientNumberAllowed = 1;
        static bool bNFound = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            MSZ.MSZView.CheckState();

            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            Utilities.RegistryKeysHelper.ReadLogException(ref bLogExceptions);

            var ServicesToRun = new MSZService(Properties.Resources.Server);
            Dictionary<string, DateTime> mapClient = new Dictionary<string, DateTime>();
            Dictionary<string, string> mapSessionToClient = new Dictionary<string, string>(); 
            
            #region MSZView
            
            TimeSpan clientPingTimeout = new TimeSpan(0,10,0);
            MSZ.MSZView.KeyChangeEvent += (ob, ev) =>
            {
                long mode = MSZ.MSZView.GetModule("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxbsDOBmBOlIRVAC/8OSgHDQ=="/* NET */);
                maxClientNumberAllowed = mode > 1 ? (int)mode : 1; 

                if (ev.MSZState || mode < 0)
                {
                    bNFound = true;
                }
                else
                    bNFound = false;
            };
            //MSZ.MSZView.Init(true);
            //MSZ.MSZView.CheckState(true);
            #endregion

            XmlConfigurator.Configure();

            var starting = String.Format(Properties.Resources.ServerStarting, Environment.Is64BitProcess ? "64" : "32");
            logServer.Info(starting);

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
                logServer.Info(Properties.Resources.ServerStopping);

                if (notifyIcon != null)
                {
                    notifyIcon.ShowBalloonTip(5000, ServicesToRun.Title,
                        Properties.Resources.ServerStopping,
                        System.Windows.Forms.ToolTipIcon.Info);
                }

                lock (notifyIconTimer)
                {
                    if (notifyIcon != null)
                    {
                        notifyIconTimer.Dispose();
                        notifyIcon.Dispose();
                        notifyIcon = null;
                    }
                }

                logServer.Info(Properties.Resources.ServerStopped);
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
                string requesttype = (e as MSZRequestArgs).MSZRequest.RequestType;
                string requestID = (e as MSZRequestArgs).MSZRequest.RequestID;
                string requestID2 = (e as MSZRequestArgs).MSZRequest.RequestID2;
#if DEBUG
                logServer.Info("RequestService");
                Console.WriteLine("RequestID2: {2} - RequestID: {1} - Type: {0}", WPFUtilities.CryptString.CryptString.DecryptString(requesttype), WPFUtilities.CryptString.CryptString.DecryptString(requestID), WPFUtilities.CryptString.CryptString.DecryptString(requestID2));
#endif
#if !DEBUG
                Console.WriteLine("Request From: {0}", WPFUtilities.CryptString.CryptString.DecryptString(requestID2));
#endif
                if (requesttype == "GggXJhcbIDBw8PDMdtXGPw==")
                {
                    if (mapSessionToClient.ContainsKey(requestID))
                        mapSessionToClient.Remove(requestID);

                    if((from s in mapSessionToClient.Keys where mapSessionToClient[s] == requestID2 select s).ToList().Count == 0 && mapClient.ContainsKey(requestID2))
                        mapClient.Remove(requestID2);

                    ServicesToRun.ServerKrytpState = true;
                }
                else
                {
                    (from c in mapClient.Keys where DateTime.Compare(mapClient[c].Add(clientPingTimeout), DateTime.Now) < 0 select c).ToList().ForEach(x => 
                        {
                            (from s in mapSessionToClient.Keys where mapSessionToClient[s] == x select s).ToList().ForEach(l => mapSessionToClient.Remove(l));
                            mapClient.Remove(x);
                        });

                    if (!mapClient.ContainsKey(requestID2))
                    {
                        if (mapClient.Count >= maxClientNumberAllowed || string.IsNullOrEmpty(requestID2))
                        {
                            ServicesToRun.ServerKrytpState = true;
                        }
                        else
                        {
                            mapClient.Add(requestID2, DateTime.Now);

                            if (!mapSessionToClient.ContainsKey(requestID))
                                mapSessionToClient.Add(requestID, requestID2);

                            ServicesToRun.ServerKrytpState = bNFound;
                        }
                    }
                    else
                    {
                        mapClient[requestID2] = DateTime.Now;
                        ServicesToRun.ServerKrytpState = bNFound;
                    }
                }

                ServicesToRun.maxClientAllowed = maxClientNumberAllowed;
                ServicesToRun.clientAllowed = mapClient.Count;

                if (!isRedirected && Console.In != System.IO.StreamReader.Null)
                {
                    if (!bNFound)
                    {
                        Console.WriteLine(Properties.Resources.ClientQueue + mapClient.Count);
                    }
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

                    Console.WriteLine(Properties.Resources.ServiceSiteCode + MSZ.MSZUtils.GetPrevious());
                    CallRefresh();
                    do
                    {
                        Console.WriteLine(Properties.Resources.PressEnter);
                        var line = Console.ReadKey();
                        if (line.Key == ConsoleKey.F5)
                            CallRefresh();
                    } while (!isStopping);
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

        private static void CallRefresh()
        {
            bool state = MSZ.MSZView.CheckState(true);
            maxClientNumberAllowed = 1;
            long mode = MSZ.MSZView.GetModule("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxbsDOBmBOlIRVAC/8OSgHDQ=="/* NET */);
            maxClientNumberAllowed = mode > 1 ? (int)mode : 1;

            if (state || mode < 0)
            {
                bNFound = true;
                Console.WriteLine(Properties.Resources.NotFound);
            }
            else
            {
                bNFound = false;
                Console.WriteLine(Properties.Resources.Found);
                Console.WriteLine(Properties.Resources.ClientAllowed + maxClientNumberAllowed);
            }

            var serial = MSZ.MSZView.GetSerial();
            Console.WriteLine(Properties.Resources.SerialNumber + serial);
            Console.WriteLine(Properties.Resources.PressEnter);
        }

#if !DEBUG
        readonly static Mindscape.Raygun4Net.RaygunClient _raygunClient = new Mindscape.Raygun4Net.RaygunClient("ALIKoCRi/xBQGYPYLUf9VA==");
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (bLogExceptions)
            {
                _raygunClient.User = System.Environment.UserName;
                _raygunClient.ApplicationVersion = Utilities.AssemblyInfo.FileVersion;
                _raygunClient.Send(e.ExceptionObject as Exception);
            }
            logServer.Fatal(String.Format(Properties.Resources.Exception, e.ExceptionObject),
                e.ExceptionObject as Exception);
            Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ExceptionObject.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.License);
            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show(e.ExceptionObject.ToString());
            }
        }
#endif

    }
}
