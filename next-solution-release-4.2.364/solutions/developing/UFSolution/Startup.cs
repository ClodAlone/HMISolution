using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Windows;
using System.Windows.Threading;
using Utilities;
using Utilities.Animations;
using System.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Runtime.InteropServices;
using log4net;
using log4net.Config;
using Microsoft.Win32;
using System.Configuration;
using WPFUtilities;

namespace UFSolution
{
    internal class Startup
    {
        static Window splashThread;
        static bool bLogExceptions = true;

        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);

        [STAThread]
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            try
            {
                Utilities.LocalizationHelper.ApplyCurrentLanguage();
            }
            catch (Exception e)
            {
                logGeneral.WarnFormat(Properties.Resources.ErrorOccuredApplyingResourceLanguage, e);
            }

            try
            {
                var test = Properties.Settings.Default.CompanyName;
            }
            catch (ConfigurationErrorsException ex)
            {
                string filename = ((ConfigurationErrorsException)ex.InnerException).Filename;

                if (MessageBox.Show(String.Format(Properties.Resources.CorruptUserSettings.Replace("-newline-", Environment.NewLine), 
                    Process.GetCurrentProcess().ProcessName), Process.GetCurrentProcess().ProcessName,
                    MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    System.IO.File.Delete(filename);
                    var process = Process.GetCurrentProcess();
                    var arguments = Environment.CommandLine.Replace(process.MainModule.FileName, "").Replace("\"\"", "");
                    Process.Start(process.MainModule.FileName, arguments);
                }

                Process.GetCurrentProcess().Kill();
            }

            logGeneral.Info(Properties.Resources.StartingApp);
            RegistryKeysHelper.ReadLogException(ref bLogExceptions);

            DevExpress.Xpf.Core.DXGridDataController.DisableThreadingProblemsDetection = true;
            DevExpress.Xpf.Grid.GridControl.AllowInfiniteGridSize = true;
            DevExpress.UserSkins.BonusSkins.Register();
            // WPFTabletSupport.DisableWPFTabletSupport();

            //set the minimum thread pool threads to avoid delays when starting a thread pool thread 
            int currentMinWkThreads;
            int currentMinCompletionPortThreads;
            ThreadPool.GetMinThreads(out currentMinWkThreads, out currentMinCompletionPortThreads);
            int currentMaxWkThreads;
            int currentMaxCompletionPortThreads;
            ThreadPool.GetMaxThreads(out currentMaxWkThreads, out currentMaxCompletionPortThreads);
            ThreadPool.SetMaxThreads(currentMinWkThreads * 8, currentMaxCompletionPortThreads);

            var framerate = Properties.Settings.Default.TimelineDesiredFrameRateValue;
            var index = SysInfo.GetIndexPerformance();
            if (index < Properties.Settings.Default.LowPerformanceIndexFrameRate)
                framerate = Properties.Settings.Default.LowPerformanceTimelineDesiredFrameRateValue;
            if (framerate > 0)
            {
                try
                {
                    Timeline.DesiredFrameRateProperty.OverrideMetadata(
                        typeof(Timeline),
                        new FrameworkPropertyMetadata { DefaultValue = framerate }
                        );
                }
                catch(Exception e)
                {
                    MessageBox.Show(String.Format(Properties.Resources.CannotChangeFrameRate, framerate, e.Message));
                }
            }

            var app = new ComponentArt.UFSolution.Demos.UFApplication();
#if !DEBUG
            app.DispatcherUnhandledException += app_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
            SetPropertyHelper();

            #region CommandLine Arguments

            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(args);

            // output all the argument pairs
            Console.WriteLine("Command Line Arguments:");
            foreach (KeyValuePair<string, string> pair in commandArgs.ArgPairs)
            {
                Console.WriteLine(string.Format("   {0} = {1}", pair.Key, pair.Value));
            }

            if (commandArgs.ArgPairs.ContainsKey("certificate"))
            {
                CheckCertificate();
                return;
            }

            InstanceManager.Initialize();
            if (!InstanceManager.IsFirstInstance())
            {
                if (commandArgs.ArgPairs.ContainsKey("singleinstance"))
                {
                    InstanceManager.ActivateFirstInstance(args);
                    return;
                }
            }

#if !DEBUG
            MSZ.MSZView.InitRemoteRequest();
#endif

            if (Properties.Settings.Default.AllowStartOption && commandArgs.ArgPairs.ContainsKey("start"))
            {
                var runtimeWnd = new RuntimeWindow();
                Application.Current.MainWindow = InstanceManager.MainWindow = runtimeWnd;
                runtimeWnd.Show();
                Application.Current.Run();
                return;
            }
            #endregion            

            //SplashScreen splash = new SplashScreen();
            //splash.Show();

            //var newWindowThread = new Thread((obj) =>
            //    {
                    var wnd = new DesignWindow();
                    splashThread = wnd;
                    if (!commandArgs.ArgPairs.ContainsKey("nosplash"))
                        wnd.Show();
                    // splash.Dispatcher.InvokeIfRequired(() => splash.Close());
                   //  wnd.Closed += (sender1, e1) => wnd.Dispatcher.InvokeShutdown();

                    //// Start the new window's Dispatcher
                    //System.Windows.Threading.Dispatcher.Run();

                    // splashThread = null;
            //    });
            //newWindowThread.SetApartmentState(ApartmentState.STA);
            //newWindowThread.IsBackground = true;
            //newWindowThread.Start();


            //ResourceDictionary dict = new ResourceDictionary
            //{
            //    Source = new Uri("Resources/SkinManagerAddendum.xaml", UriKind.Relative)
            //    // Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/SkinManager.xaml", UriKind.Relative)
            //};
            //app.Resources.MergedDictionaries.Add(dict);

            #region Instance Mng

            if (InstanceManager.IsFirstInstance())
            {
                Application.Current.Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                    {
                        //using (DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint()))
                        //{
                        //    FindResponse discoveryResponse = discoveryClient.Find(new FindCriteria(typeof(IInstanceManager)));
                        //}

                        // Create an AnnouncementService instance
                        AnnouncementService announcementService = new AnnouncementService();

                        // Subscribe the announcement events
                        announcementService.OnlineAnnouncementReceived += (o, e) =>
                                        {
                                            Console.WriteLine("Received an online announcement from {0}", e.EndpointDiscoveryMetadata.Address);

                                            // var Binding = DiscoveryHelper.DiscoverBinding(typeof(T), e.EndpointDiscoveryMetadata.Address.Uri), e.EndpointDiscoveryMetadata.Address);
                                        };

                        announcementService.OfflineAnnouncementReceived += (o, e) =>
                                        {
                                            Console.WriteLine("Received an offline announcement from {0}", e.EndpointDiscoveryMetadata.Address);
                                        };

                        // Create ServiceHost for the AnnouncementService
                        ServiceHost announcementServiceHost = new ServiceHost(announcementService);
                        // Listen for the announcements sent over UDP multicast
                        announcementServiceHost.AddServiceEndpoint(new UdpAnnouncementEndpoint());
                        announcementServiceHost.Open();
                    });
            }
            #endregion

            UFMainWindow mainWnd = new UFMainWindow();
            Application.Current.MainWindow = InstanceManager.MainWindow = mainWnd;
            if (splashThread != null)
            {
                mainWnd.ContentRendered += mainWnd_ContentRendered;
            }
            mainWnd.Show();

            Application.Current.Run();
        }

        private static void SetPropertyHelper()
        {
            var commonFolder = String.Format("{0}\\{1}\\{2}",
                             Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                             Properties.Settings.Default.CompanyName,
                             Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("CommonFolder", commonFolder);

            var userFolder = String.Format("{0}\\{1}\\{2}",
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            Properties.Settings.Default.CompanyName,
                            Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("UserFolder", userFolder);

            var projectFolder = String.Format("{0}\\{1}\\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                Properties.Settings.Default.CompanyName,
                Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("ProjectFolder", projectFolder);
        }

        internal static void CheckCertificate(bool bSilent = true)
        {
            var a = Assembly.GetExecutingAssembly();
            var s = a.Location.ToLower();
            var name = a.GetName().Name.ToLower() + ".exe";
            var idx = s.IndexOf(name);
            if (idx != -1)
                s = s.Substring(0, idx);

            string path = "CertificateChecker.exe";
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

            try
            {
                if (bSilent)
                {
                    var arguments = string.Format("\"/SPlatform.NExT MoviconNExT\" \"/E{0}OPCUAClient.Config.xml\" /C", s);
                    var p = RunElevated.RunNoWait(path, arguments);
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                    {
                        //certificate check failed, launch interactive??
                        p.Close();
                        arguments = string.Format("/I \"/SPlatform.NExT MoviconNExT\" \"/E{0}OPCUAClient.Config.xml\" /C", s);
                        RunElevated.Run(path, arguments);
                    }
                    p.Close();
                }
                else
                {
                    var arguments = string.Format("/I \"/SPlatform.NExT MoviconNExT\" \"/E{0}OPCUAClient.Config.xml\" /C", s);
                    RunElevated.Run(path, arguments);
                    // var p = Process.Start("CertificateChecker.exe", arguments);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorOccuredRunningCertificateManager, ex.Message));
            }
        }

        static void  mainWnd_ContentRendered(object sender, EventArgs e)
        {
            Application.Current.MainWindow.ContentRendered -= mainWnd_ContentRendered;
            if (splashThread != null)
                splashThread.Dispatcher.InvokeIfRequired(() => 
                {
                    if (splashThread != null)
                    {
                        splashThread.Close();
                        splashThread = null;
                    }
                });
        }

        // readonly static Object lockObject = new Object();
#if !DEBUG
        readonly static Mindscape.Raygun4Net.RaygunClient _raygunClient = new Mindscape.Raygun4Net.RaygunClient("6zdPg/EmwqK4MYa9G1JLhQ==");
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logGeneral.Fatal(Properties.Resources.ApplicationError, e.ExceptionObject as Exception);
            if (bLogExceptions)
            {
                _raygunClient.User = System.Environment.UserName;
                _raygunClient.ApplicationVersion = Utilities.AssemblyInfo.FileVersion;
                _raygunClient.Send(e.ExceptionObject as Exception);
            }
            if (e.ExceptionObject is TypeInitializationException)
            {
                try
                {
                    CleanPerformaceCounters();
                }
                catch
                { }
            }
        }

        private static void app_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // Bug opening the clipboard, the clipboard may be temporary opened by another process
            var comException = e.Exception as System.Runtime.InteropServices.COMException;
            if (comException != null && comException.ErrorCode == -2147221040)
            {
                 e.Handled = true;
                 return;
            }
            ///////////////////////////////////////////////////////////////////////////////////////

            logGeneral.Fatal(Properties.Resources.ApplicationError, e.Exception);
            if (bLogExceptions)
            {
                _raygunClient.User = System.Environment.UserName;
                _raygunClient.ApplicationVersion = Utilities.AssemblyInfo.FileVersion;
                _raygunClient.Send(e.Exception);
            }
            if (e.Exception is TypeInitializationException)
            {
                try
                {
                    CleanPerformaceCounters();
                }
                catch
                { }
            }

            /*
            lock (lockObject)
            {
                var wnd = Application.Current.MainWindow as UFMainWindow;
                if (wnd != null)
                    wnd.Dispatcher.InvokeIfRequired(() => wnd.SetBusy(false));

                Trace.TraceError(Properties.Resources.Exception_Text, MethodBase.GetCurrentMethod(), e.Exception);

                if (splashThread != null)
                {
                    splashThread.Dispatcher.InvokeIfRequired(() => splashThread.Close());
                    splashThread = null;
                }

                // if (!System.Diagnostics.Debugger.IsAttached)
                {
                    String exceptionText = "Exception:\n";
                    if (e.Exception != null)
                        exceptionText += String.Format("{0}\n{1}", e.Exception.Message, e.Exception.StackTrace);
                    if (e.Exception.InnerException != null)
                        exceptionText += String.Format("\n\nInnerException:\n{0}\n{1}", e.Exception.InnerException.Message, e.Exception.InnerException.StackTrace);

                    ErrorPanel ep = new ErrorPanel()
                    {
                        Height = 450,
                        Width = 510
                    };

                    ep.ExceptionText.Text = exceptionText;
                    var Dialog = new Window()
                    {
                        //Owner = Application.Current.MainWindow,
                        WindowStyle = WindowStyle.ToolWindow,
                        SizeToContent = SizeToContent.WidthAndHeight,
                        Content = ep
                    };
                    if (Dialog.ShowDialog() == true)
                    {
                        e.Handled = true;
                    }
                }
                
#if !DEBUG
            // Tracing.SimpleLogging.LogException(Properties.Resources.Exception_Text, MethodBase.GetCurrentMethod(), e.Exception);

            //if (MessageBox.Show(e.Exception.Message + Properties.Resource.Exception_Text, Properties.Resource.Exception_Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    e.Handled = true;
            //}
#endif
            }
            */
        }

        static bool bPerformaceCountersCleaned;
        static void CleanPerformaceCounters()
        {
            if (!bPerformaceCountersCleaned)
            {
                bPerformaceCountersCleaned = true;
                RunElevated.RunNoWait("cmd.exe", "/C lodctr /R");
            }
        }
#endif
    }
}