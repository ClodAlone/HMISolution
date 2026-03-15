using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;
using UFUAServerBase;
using Utilities;
using Utilities.Logger;
using System.Diagnostics;
using Opc.Ua;
using Utilities.AssemblyResolver;

namespace UFUAServerApp.Core
{
    class Program
    {
        static readonly AssemblyResolver assemblyResolver = new AssemblyResolver(addRootPath: false);
        static bool bLogExceptions = true;
        static Timer notifyIconTimer;
        static bool bExitMode;

        static readonly ILog logServer = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.Server);

        static void Main(string[] args)
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            //Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            RegistryKeysHelper.ReadLogException(ref bLogExceptions);
            //Utilities.WaitCursor.SuppressWaitCursor();

#if DEBUG
            Console.WriteLine("if you would like to attach a debugger now is the right moment !");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
#endif

#if !DEBUG
            MSZ.MSZView.InitRemoteRequest(1);
#endif

            UFUAServer ServicesToRun = new UFUAServer();

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
                    ServicesToRun.UpdateServiceLevelAsync();
                }, null, 2000, 1000);
            };

            ServicesToRun.StoppingService += (o, e) =>
            {
                logServer.Info(Properties.Resources.ServerStopping);

                uint i = 3;
                while (i > 0)
                {
                    ServicesToRun.SetSecondsAndReason(i, Properties.Resources.ServerStopping);
                    Thread.Sleep(1000);
                    --i;
                }
                ServicesToRun.SetSecondsAndReason(i, Properties.Resources.ServerStopping);

                notifyIconTimer.Dispose();

                logServer.Info(Properties.Resources.ServerStopped);
            };

            string[] serverargs = new string[args.Length + 1];
            args.CopyTo(serverargs, 0);
            serverargs[args.Length] = string.Format("-CCheck=\"/SPlatform.NExT IOServer\" \"/E{0}\"", UFUAServerInfo.UFUAServerInfo.GetServerConfigFile());

            try
            {
                ServicesToRun.StartService(serverargs).Wait();
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
        }

#if !DEBUG
        readonly static Mindscape.Raygun4Net.RaygunClient _raygunClient = new Mindscape.Raygun4Net.RaygunClient("sITdlxpbHKnut8o+Hdw1hA==");
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
            Logger.WriteToEventLog(Utilities.Properties.Resources.Server, e.ExceptionObject.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
        }
#endif
    }
}
