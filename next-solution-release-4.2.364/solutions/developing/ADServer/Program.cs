using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Threading;
using UFUAServerBase;
using Utilities;
using log4net;
using log4net.Config;
using ADPluginBase;
using Opc.Ua;
using Utilities.Logger;
using System.Diagnostics;

namespace ADServer
{
    class Program
    {
        static Timer notifyIconTimer;
        static bool bLogExceptions = true;

        static readonly ILog logAlarmDispatcher = LogManager.GetLogger(Utilities.Properties.Resources.AlarmDispatcherManager);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            RegistryKeysHelper.ReadLogException(ref bLogExceptions);
            Utilities.WaitCursor.SuppressWaitCursor();

            using (ADServer ServicesToRun = new ADServer())
            {

                XmlConfigurator.Configure();
#if DEBUG
                if (!System.Diagnostics.Debugger.IsAttached &&
                    Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                     String.Format("DebugMe - {0}", ServicesToRun.ServiceName), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    System.Diagnostics.Debugger.Launch();
#endif
                logAlarmDispatcher.Info(String.Format(Properties.Resources.ServerStarting, Environment.Is64BitProcess ? "64" : "32"));

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
                    ServicesToRun.OnSystemEvent(ObjectIds.Server, Properties.Resources.ServerStopping,EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.AlarmDispatcher);

                    uint i = 3;
                    while (i > 0)
                    {
                        ServicesToRun.SetSecondsAndReason(i, Properties.Resources.ServerStopping);
                        Thread.Sleep(1000);
                        --i;
                    }

                    ServicesToRun.SetSecondsAndReason(i, Properties.Resources.ServerStopping);

                    notifyIconTimer.Dispose();

                    ServicesToRun.OnSystemEvent(ObjectIds.Server, Properties.Resources.ServerStopped,EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.AlarmDispatcher);
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
            Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ExceptionObject.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.AlarmDispatcher);
            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show(e.ExceptionObject.ToString());
            }
        }
#endif
    }
}
