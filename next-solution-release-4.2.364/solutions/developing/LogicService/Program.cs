using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using log4net;
using log4net.Config;
using System.ServiceProcess;
using Utilities.Logger;
using Utilities.AssemblyResolver;
using System.Diagnostics;

namespace LogicService
{
    static class Program
    {
        static readonly AssemblyResolver assemblyResolver = new AssemblyResolver();
        static Timer notifyIconTimer;
        static bool bLogExceptions = true;

        static readonly ILog logServer = Logger.GetDestinationLog(LoggerDestination.LogicService);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            Utilities.RegistryKeysHelper.ReadLogException(ref bLogExceptions);
            Utilities.WaitCursor.SuppressWaitCursor();

            var ServicesToRun = new LogicService();

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
                }, null, 2000, 1000);
            };

            ServicesToRun.StoppingService += (o, e) =>
            {
                logServer.Info(Properties.Resources.ServerStopping);

                notifyIconTimer.Dispose();

                logServer.Info(Properties.Resources.ServerStopped);
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

                var serverThread = new Thread(() =>
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
                            ServicesToRun.IsStarted = false;
                            callingProcessTerminated.Set();
                        };

                        ServicesToRun.StartService(args);
                        ServicesToRun.IsStarted = true;
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
                                            ServicesToRun.IsStarted = false;
                                            callingProcessTerminated.Set();
                                        };
                                    }
                                }
                                catch { }
                            }
                        }

                        ServicesToRun.StartService(args);
                        ServicesToRun.IsStarted = true;
                    }

                    callingProcessTerminated.WaitOne();
                });
                serverThread.SetApartmentState(ApartmentState.STA);
                serverThread.Start();
            }

#if !DEBUG
            else
                ServiceBase.Run(ServicesToRun);
#endif
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
            Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ExceptionObject.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.ScriptService);
            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show(e.ExceptionObject.ToString());
            }
        }
#endif
    }
}
