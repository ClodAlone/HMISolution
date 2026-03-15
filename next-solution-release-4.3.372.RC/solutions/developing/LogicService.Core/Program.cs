using log4net;
using log4net.Config;
using System;
using System.Threading;
using Utilities.AssemblyResolver;
using Utilities.Logger;

namespace LogicService.Core
{
    class Program
    {
        static bool bLogExceptions = true;
        static Timer notifyIconTimer;
        static bool bExitMode;

        static readonly ILog logServer = Logger.GetDestinationLog(LoggerDestination.LogicService);

        static void Main(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            //Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            Utilities.RegistryKeysHelper.ReadLogException(ref bLogExceptions);
            //Utilities.WaitCursor.SuppressWaitCursor();

#if DEBUG
            Console.WriteLine("if you would like to attach a debugger now is the right moment !");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
#endif

            var ServicesToRun = new LogicService();

            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("log4net.config"));

            var starting = String.Format(Properties.Resources.ServerStarting, Environment.Is64BitProcess ? "64" : "32");
            logServer.Info(starting);

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
                        //Console.WriteLine("License Found = {0}", ret);
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

            ServicesToRun.StartService(args);
            ServicesToRun.IsStarted = true;

            Console.WriteLine("Press Ctrl-C to exit...");

            var quitEvent = new ManualResetEvent(false);
            try
            {
                Console.CancelKeyPress += (sender, eArgs) => {
                    if (!bExitMode)
                    {
                        bExitMode = true;
                        ServicesToRun.StopService();
                        ServicesToRun.IsStarted = false;
                        quitEvent.Set();
                    }
                    eArgs.Cancel = true;
                };
            }
            catch
            { }

            quitEvent.WaitOne();
        }

#if !DEBUG
        readonly static Mindscape.Raygun4Net.RaygunClient _raygunClient = new Mindscape.Raygun4Net.RaygunClient("d221pTLqpBvs90lSvlig");
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
        }
#endif
    }
}
