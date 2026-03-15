using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFUAServerBase;
using Utilities;
using System.Threading;
using System.ServiceProcess;
using log4net;
using log4net.Config;
using Opc.Ua;
using Utilities.Logger;
using System.Diagnostics;

namespace MSServer
{
    class Program
    {
        static Timer notifyIconTimer;
        static bool bLogExceptions = true;

#if NET_CORE
        static bool bExitMode;
        static readonly ILog logScheduler = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Utilities.Properties.Resources.SchedulerManager);
#else
        static readonly ILog logScheduler = LogManager.GetLogger(Utilities.Properties.Resources.SchedulerManager);
#endif

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if !NET_CORE
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            Utilities.WaitCursor.SuppressWaitCursor();
#else
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
#endif
            RegistryKeysHelper.ReadLogException(ref bLogExceptions);

#if !NET_CORE
            using (MSServer ServicesToRun = new MSServer())
#else
#if !DEBUG
            MSZ.MSZView.InitRemoteRequest(1);
#endif
            MSServer ServicesToRun = new MSServer();
#endif
            {
#if !NET_CORE
                XmlConfigurator.Configure();
#else
                var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("log4net.config"));
#endif
#if DEBUG
#if !NET_CORE
                if (!System.Diagnostics.Debugger.IsAttached &&
                    Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                         String.Format("DebugMe - {0}", ServicesToRun.ServiceName), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    System.Diagnostics.Debugger.Launch();
#else
                Console.WriteLine("if you would like to attach a debugger now is the right moment !");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();

#endif
#endif

                logScheduler.Info(String.Format(Properties.Resources.ServerStarting, Environment.Is64BitProcess ? "64" : "32"));
#if !DEBUG
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif

                ServicesToRun.StartingService += (ob, ev) =>
                {
                    notifyIconTimer = new Timer((o) =>
                    {
#if !DEBUG
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            var ret = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxY+APFmQPtOWSUaMJjYrwAg=="/* DBG */);
                            if (!ret)
                                System.Environment.Exit(-10);
                        }
#endif
                        ServicesToRun.UpdateServiceLevelAsync();
                    }, null, 2000, 1000);
                };

                ServicesToRun.StoppingService += (o, e) =>
                {
                    ServicesToRun.OnSystemEvent(null, Properties.Resources.ServerStopping, EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.Scheduler);
                    //SchedulerLogger.WriteToEventLog(Properties.Resources.SchedulerServer, Properties.Resources.ServerStopping, System.Diagnostics.EventLogEntryType.Information, LoggerDestination.Scheduler);

                    uint i = 3;
                    while (i > 0)
                    {
                        ServicesToRun.SetSecondsAndReason(i, Properties.Resources.ServerStopping);
                        Thread.Sleep(1000);
                        --i;
                    }

                    ServicesToRun.SetSecondsAndReason(i, Properties.Resources.ServerStopping);

                    notifyIconTimer.Dispose();

                    ServicesToRun.OnSystemEvent(null, Properties.Resources.ServerStopped, EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.Scheduler);
                };

                string[] newargs = new string[args.Length + 1];
                args.CopyTo(newargs, 0);
                newargs[args.Length] = string.Format("-CCheck=\"/SPlatform.NExT Scheduler\" \"/E{0}\"", MSServerInfo.MSServerInfo.GetServerConfigFile());

#if !NET_CORE
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
                    {
                        Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);
                        if (commandArgs.ArgPairs.ContainsKey("processId"))
                        {
                            int callingProcessId;
                            if (int.TryParse(commandArgs.ArgPairs["processId"], out callingProcessId) && callingProcessId > 0)
                            {
                                try
                                {
                                    var process = Process.GetProcessById(callingProcessId);
                                    if (process != null)
                                    {
                                        process.EnableRaisingEvents = true;
                                        process.Exited += (o, e) =>
                                        {
                                            ServicesToRun.StopService();
                                            callingProcessTerminated.Set();
                                        };
                                    }
                                }
                                catch { }
                            }
                        }

                        ServicesToRun.StartService(args);
                    }

                    callingProcessTerminated.WaitOne();
                }
#if !DEBUG
                else
                    ServiceBase.Run(ServicesToRun);
#endif
#else
                try
                {
                    ServicesToRun.StartService(args).Wait();
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex, "Unexpected error starting the process.");
                    Console.WriteLine("Unexpected error starting the process: {0}", ex.Message);
                    if (notifyIconTimer != null)
                        notifyIconTimer.Dispose();
                    return;
                }

                Console.WriteLine("Press Ctrl-C to exit...");

                var quitEvent = new ManualResetEvent(false);
                try
                {
                    Console.CancelKeyPress += (sender, eArgs) => {
                        if (!bExitMode)
                        {
                            bExitMode = true;
                            ServicesToRun.StopService();
                            quitEvent.Set();
                        }
                        eArgs.Cancel = true;
                    };
                }
                catch
                { }

                quitEvent.WaitOne();
#endif
            }
        }

#if !DEBUG
        readonly static Mindscape.Raygun4Net.RaygunClient _raygunClient = new Mindscape.Raygun4Net.RaygunClient("VuSmld0CluvmUkQRavboHw==");
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (bLogExceptions)
            {
                _raygunClient.User = System.Environment.UserName;
                _raygunClient.ApplicationVersion = Utilities.AssemblyInfo.FileVersion;
                _raygunClient.Send(e.ExceptionObject as Exception);
            }
            Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ExceptionObject.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Scheduler);
        }
#endif
    }
}
