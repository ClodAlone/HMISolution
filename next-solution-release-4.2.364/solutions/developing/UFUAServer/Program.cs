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
using System.Reflection;
using System.IO.IsolatedStorage;
using System.Security.AccessControl;
using System.Security.Principal;

namespace UFUAServerApp
{
    static class Program
    {
        static bool bLogExceptions = true;
        static Timer notifyIconTimer;
        
        static readonly ILog logServer = LogManager.GetLogger(Properties.Resources.Server);

        static long maxSessions = 0;
        static Semaphore activeSessions;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(params String[] args)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            RegistryKeysHelper.ReadLogException(ref bLogExceptions);
            Utilities.WaitCursor.SuppressWaitCursor();

#if !DEBUG
            MSZ.MSZView.InitRemoteRequest(1);
#endif

            UFUAServer ServicesToRun = new UFUAServer();

            XmlConfigurator.Configure();

            ApplicationPropertiesHelper.LoadApplicationProperties(GetStorage(), GetStoreFileName());
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

#if !DEBUG
            maxSessions = MSZ.MSZView.GetModule("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxasqQE7XJtxBA3WdmRKB/PA=="/* SIN */);
#else
            maxSessions = 50;
#endif
            if (maxSessions > 0)
            {
                var semaphoreName = Assembly.GetExecutingAssembly().FullName;
                try
                {
                    activeSessions = Semaphore.OpenExisting(semaphoreName);
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    var semaphoreSecurity = new SemaphoreSecurity();
                    semaphoreSecurity.AddAccessRule(new SemaphoreAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                        SemaphoreRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));

                    bool createdNew = false;
                    activeSessions = new Semaphore((int)maxSessions, (int)maxSessions, semaphoreName, out createdNew, semaphoreSecurity);
                }
            }

            bool acquired = false;
            ServicesToRun.StartingService += (ob, ev) =>
            {
                if (maxSessions > 0){
                if (activeSessions != null)
                {
                    try
                    {
                        acquired = activeSessions.WaitOne(1000);
                    }
                    catch (AbandonedMutexException ex)
                    {
                        logServer.Warn(ex.Message);
                    }
                }
                if (!acquired)
                {
                    logServer.Error(Properties.Resources.MaxServerSessionNumberExceded);
                    if (Environment.UserInteractive)
                    {
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.MaxServerSessionNumberExceded);
                    }

                    System.Environment.Exit(-10);
                }
                }
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

                ApplicationPropertiesHelper.SaveApplicationProperties(GetStorage(), GetStoreFileName());
                logServer.Info(Properties.Resources.ServerStopped);

                if (maxSessions > 0 && acquired && activeSessions != null)
                {
                    acquired = false;
                    activeSessions.Release();
                }
            };

            string[] serverargs = new string[args.Length + 1];
            args.CopyTo(serverargs, 0);
            serverargs[args.Length] = string.Format("-CCheck=\"/SPlatform.NExT IOServer\" \"/E{0}\"", UFUAServerInfo.UFUAServerInfo.GetServerConfigFile());
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
            logServer.Fatal(String.Format(Properties.Resources.Exception, e.ExceptionObject),
                e.ExceptionObject as Exception);
            Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ExceptionObject.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Server);
            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show(e.ExceptionObject.ToString());
            }
        }
#endif
        #region Isolated Storage
        static String GetStoreFileName()
        {
            return String.Format("{0}.Properties.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }
        #endregion
    }

}
