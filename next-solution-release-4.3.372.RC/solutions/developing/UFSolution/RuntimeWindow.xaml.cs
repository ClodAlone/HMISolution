using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UFInterfaces.CoreHostComponents;
using System.ComponentModel;
using UFInterfaces;
using Utilities;
using DocumentManager.ComponentService;
using UriResolver.ComponentService;
using System.Windows.Threading;
using System.Reflection;
using Syncfusion.Windows.Shared;
using System.Threading;
using System.Windows.Media.Animation;
using UIMsgBoxAlertService.ComponentService;
using Tracing.ComponentService;
using log4net;
using WPFUtilities;

namespace UFSolution
{
    /// <summary>
    /// Interaction logic for RuntimeWindow.xaml
    /// </summary>
    public partial class RuntimeWindow : Window
    {
        #region Declarations

        readonly ComponentHost componentHost;
        DispatcherTimer iconTimer;
        bool bContentRendered;

        readonly BitmapImage[] arrayImages = new BitmapImage[2];
        int currentIcon;

        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        #endregion Declarations

        public RuntimeWindow()
        {
            InitializeComponent();

            componentHost = new ComponentHost();

            arrayImages[0] = GetControlImage("runtime1.png");
            arrayImages[1] = GetControlImage("runtime2.png");

            try
            {
                var list = FindAndLoadDLL.LoadDLLs<UserControl>(String.Format("{0}Splashes\\", AppDomain.CurrentDomain.BaseDirectory),
                                                    "RuntimeSplashControl.dll", false);

                if (list.Count > 0)
                    splashContent.Content = list[0];
                versionLabel.Content = String.Format(Properties.Resources.Splash_VersionLabel, Utilities.AssemblyInfo.FileFormatVersion);
                revisionLabel.Content = String.Format(Properties.Resources.Splash_RevisionLabel, Utilities.AssemblyInfo.FilePrivatePart);
            }
            catch (Exception ex)
            {
                
            }

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                SetSkin(Properties.Settings.Default.Skin);
            }
            else
            {
                Properties.Settings.Default.Reset();
                SetSkin(Properties.Settings.Default.Skin);
            }
        }

        #region Window events

        bool bLoaded;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bLoaded)
                return;
            bLoaded = true;

            ResourceDictionaryExtensions.AddCommonResources(this);

#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                var ret = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxY+APFmQPtOWSUaMJjYrwAg=="/* DBG */);
                if (!ret)
                    return;
            }
#endif

            using (new WaitCursor())
            {
                #region PluginDiscovering

                //Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                //{
                System.Diagnostics.Trace.TraceInformation("Checking Plugins...");

                Global.Plugins.FindPlugins(AppDomain.CurrentDomain.BaseDirectory, "*ViewModel*.dll");
                //Global.Plugins.FindPlugins(AppDomain.CurrentDomain.BaseDirectory, "*Manager*.dll");
                //Global.Plugins.FindPlugins(AppDomain.CurrentDomain.BaseDirectory, "*Provider*.dll");

                Global.Plugins.FindPlugins(String.Format("{0}CommonPlugins\\", AppDomain.CurrentDomain.BaseDirectory));
                Global.Plugins.FindPlugins(String.Format("{0}RuntimePlugins\\", AppDomain.CurrentDomain.BaseDirectory));
                Global.Plugins.FindPlugins(String.Format("{0}Managers\\", AppDomain.CurrentDomain.BaseDirectory));

                Global.Plugins.FindPlugins(String.Format("{0}DataSinks\\", AppDomain.CurrentDomain.BaseDirectory));

                foreach (var pluginOn in Global.Plugins.AvailablePlugins)
                {
                    if (((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance is IComponent)
                        componentHost.Components.Add(((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance as IComponent);
                }

                foreach (var pluginOn in Global.Plugins.AvailablePlugins)
                    ((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance.Initialize();

                CheckCommandArguments();

                iconTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) };
                iconTimer.Tick += (o, ev) =>
                {
                    //notifyIcon.Icon = arrayImages[currentIcon];
                    if (++currentIcon >= arrayImages.Length)
                        currentIcon = 0;
                };
                iconTimer.Start();
                //});

                #endregion PluginDiscovering
            }
        }

        DispatcherTimer timer;
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (bContentRendered)
                return;
            bContentRendered = true;

#if !DEBUG
            bool bFound = false;
            MSZ.MSZView.KeyChangeEvent += (ob, ev) =>
            {
                Dispatcher.InvokeIfRequired(() =>
                {
                    if (ev.MSZState)
                    {
                        if (!bFound)
                        {
                            logLicense.Warn(Properties.Resources.LicenseNotFound);
                            bFound = true;
                        }
                    }
                    else
                    {
                        if (bFound)
                        {
                            bFound = false;
                            if (timer != null)
                                timer.Stop();
                        }
                    }
                });
            };
            MSZ.MSZView.Read();
#endif

            #region Expiring

            //if (DateTime.UtcNow > new DateTime(2012, 12, 1))
            //{
            //    Action action = () => Application.Current.Shutdown();
            //    Dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
            //}

            #endregion
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            using (new WaitCursor())
            {
                Global.Plugins.ClosePlugins();

                //notifyIcon.Dispose();
            }
        }

        #endregion Window events

        void SetSkin(String style)
        {
            using (new WaitCursor())
            {
                // Ribbon.SetActiveColorScheme(MainWnd, Brushes.Black);

                // ApplicationPropertiesHelper.SetProperty("CurrentSkinBackColor", Application.Current.MainWindow.Background);
                ApplicationPropertiesHelper.SetProperty("CurrentSkinBackColor", Brushes.Black);

                // SkinStorage.SetVisualStyle(this, style);
                // SkinStorage.SetVisualStyle(dockingManager, style);
                ApplicationPropertiesHelper.SetProperty("CurrentSkin", style);
                Properties.Settings.Default.Skin = style;

                ThemeHelper.SetTheme(this, style);
                //ThemeHelper.SetApplicationTheme();

                /*
                switch (style)
                {
                    case "Blend": ThemeManager.SetThemeName(this, "MetropolisDark"); ThemeManager.ApplicationThemeName = "MetropolisDark"; break;
                    case "VS2010": ThemeManager.SetThemeName(this, "VisualStudio2010"); ThemeManager.ApplicationThemeName = "VisualStudio2010"; break;
                    case "Office2010Black": ThemeManager.SetThemeName(this, "Office2010Black"); ThemeManager.ApplicationThemeName = "Office2010Black"; break;
                    case "Office2007Silver":
                    case "Office2010Silver": ThemeManager.SetThemeName(this, "Office2007Silver"); ThemeManager.ApplicationThemeName = "Office2007Silver"; break;
                    case "Office2007Blue":
                    case "Office2010Blue": ThemeManager.SetThemeName(this, "Office2007Blue"); ThemeManager.ApplicationThemeName = "Office2007Blue"; break;
                    default: ThemeManager.SetThemeName(this, "DevExpressStyle"); ThemeManager.ApplicationThemeName = "DevExpressStyle"; break;
                }
                */
            }
        }

        private static BitmapImage GetControlImage(String iconName)
        {
            BitmapImage bm = new BitmapImage();
            bm.BeginInit();
            Assembly assembly = Assembly.GetExecutingAssembly();

            String str = String.Format("pack://application:,,,/{0};component/Images/{1}",
                System.IO.Path.GetFileNameWithoutExtension(assembly.Location), iconName);
            bm.UriSource = new Uri(str);
            bm.EndInit();
            return bm;
        }

        #region Command arguments

        internal void UpdateCommandArguments(String[] commands = null)
        {
            var commandLine = commands;
            if (commands == null)
                commandLine = Environment.GetCommandLineArgs();
            else
                commandLine = commands;
            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(commandLine);

            if (commandArgs.ArgPairs.ContainsKey("screen"))
            { 
                var file = commandArgs.ArgPairs["screen"];

                var uriRisolver = componentHost.GetService(typeof(IUriRisolver)) as IUriRisolver;
                uriRisolver.Container = componentHost.Components;

                Uri uri = null;
                if (XpoHelpers.XpoHelper.IsDataSource(file))
                    uri = new Uri(String.Format("{0}:{1}", uriRisolver.GetOpenFileScheme(), file));
                else
                    uri = new Uri(String.Format("{0}://{1}", uriRisolver.GetOpenFileScheme(), file));

                var manager = uriRisolver.ResolveUri(uri) as IDocumentManager;
                try
                {
                    manager.Execute(uri, null, ExecutionMode.Normal, null);
                }
                catch (Exception ex)
                {
                }
            }
        }

        void CheckCommandArguments()
        {
             var newWindowThread = new Thread((obj) =>
                 {
                     if (Properties.Settings.Default.ImproveWpfRenderingPerformance)
                         Thread.CurrentThread.Priority = ThreadPriority.Highest;

                     var uimsgbox = componentHost.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(Environment.GetCommandLineArgs());

                    String file = String.Empty;
                    if (commandArgs.ArgPairs.ContainsKey("file"))
                        file = commandArgs.ArgPairs["file"];
                    else if (commandArgs.Params.Count > 1)
                        file = commandArgs.Params[1];
                    if (String.IsNullOrEmpty(file))
                    {
                        uimsgbox.ShowError(Properties.Resources.CommandLineUsage);

                        Dispatcher.InvokeIfRequired(() => Close());
                        return;
                    }

                    var uriRisolver = componentHost.GetService(typeof(IUriRisolver)) as IUriRisolver;
                    uriRisolver.Container = componentHost.Components;

                    Uri uri = null;
                    Uri uriFile = null;
                    try
                    {
                        if (XpoHelpers.XpoHelper.IsDataSource(file))
                            uri = new Uri(String.Format("{0}:{1}", uriRisolver.GetOpenFileScheme(), file));
                        else if (file.StartsWith(@"\\"))
                        {
                             uri = new Uri(String.Format("{0}://file", uriRisolver.GetOpenFileScheme(), file));
                             uriFile = new Uri(String.Format("file://{0}", file));
                        }
                        else
                            uri = new Uri(String.Format("{0}://{1}", uriRisolver.GetOpenFileScheme(), file));
                    }
                    catch(Exception ex)
                    {
                        uimsgbox.ShowError(Properties.Resources.CommandLineUsage);

                        Dispatcher.InvokeIfRequired(() => Close());
                        return;
                    }

                    var manager = uriRisolver.ResolveUri(uri) as IDocumentManager;
                    try
                    {
                         if (uriFile != null)
                             manager.Execute(uriFile, null, ExecutionMode.Synchro, null);
                         else
                            manager.Execute(uri, null, ExecutionMode.Synchro, null);
                    }
                    catch (Exception ex)
                    {
                        uimsgbox.ShowError(ex.Message);

                        manager.Terminate(uri, null);

                        Dispatcher.InvokeShutdown();
                        return;
                    }

                    Dispatcher.InvokeIfRequired(() => Hide());

                    // WPFTabletSupport.DisableWPFTabletSupport();

                    // Start the new window's Dispatcher
                    System.Windows.Threading.Dispatcher.Run();

                    Dispatcher.InvokeIfRequired(() => 
                        {
                            terminatingLabel.Visibility = System.Windows.Visibility.Visible;
                            Show();
                        });

                    manager.Terminate(uri, null);

                    Dispatcher.InvokeIfRequired(() => Close());
                 });

             newWindowThread.SetApartmentState(ApartmentState.STA);
             newWindowThread.IsBackground = true;
             newWindowThread.Start();
        }

        #endregion

        private void OnNotificationAreaIconDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

    }
}
