using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using OPCUAViewModelService.ComponentService;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using Tracing.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using Utilities;
using Utilities.WPF;
using System.Collections.Generic;
using UriResolver.ComponentService;
using System.Windows.Media.Imaging;
using DocumentManager.ComponentService;
using DevExpress.Xpf.Core;
using System.Threading;
using CommonControls;
using System.Reflection;
using log4net;
using WPFUtilities;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace UFSolution
{
    /// <summary>
    /// Interaction logic for UFSolutionMainWindow.xaml
    /// </summary>
    public partial class UFMainWindow : RibbonWindow
    {
        #region Declarations

        readonly ComponentHost componentHost;
        readonly WorkSpaceComponent workSpaceComponent;
        ISimpleLogging simpleLogging;
        bool bLoadingDockingState;
        List<FrameworkElement> eliteDockedToBeSaved;

        static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);

        readonly DispatcherTimer memoryCheckTimer;
        #endregion Declarations

        #region DependencyProperties

        static UFMainWindow()
        {
            StatusTextProperty = DependencyProperty.Register("StatusText", typeof(string), typeof(UFMainWindow),
                        new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender));
        }

        public static DependencyProperty StatusTextProperty;

        public string StatusText
        {
            get
            {
                return (string)GetValue(StatusTextProperty);
            }
            set
            {
                simpleLogging.AddItem(Properties.Resources.statusTextTrace_Title, value);
                SetValue(StatusTextProperty, value);
            }
        }

        #endregion DependencyProperties

        #region ctor

        public UFMainWindow()
        {
            System.Diagnostics.Trace.TraceInformation(Properties.Resources.LoadingText);

            if (Properties.Settings.Default.Aliased)
                RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            //var commonFolder = String.Format("{0}\\{1}\\{2}",
            //                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            //                Properties.Settings.Default.CompanyName,
            //                Properties.Settings.Default.CommonApplicationFolder);
            //ApplicationPropertiesHelper.SetProperty("CommonFolder", commonFolder);

            //var userFolder = String.Format("{0}\\{1}\\{2}",
            //                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            //                Properties.Settings.Default.CompanyName,
            //                Properties.Settings.Default.CommonApplicationFolder);
            //ApplicationPropertiesHelper.SetProperty("UserFolder", userFolder);

            //var projectFolder = String.Format("{0}\\{1}\\{2}",
            //    Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
            //    Properties.Settings.Default.CompanyName,
            //    Properties.Settings.Default.CommonApplicationFolder);
            //ApplicationPropertiesHelper.SetProperty("ProjectFolder", projectFolder);
            InitializeComponent();

            try
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.FontFamily))
                {
                    CustomFontHelper.bCustomFont = true;
                    FontFamily = new FontFamily(Properties.Settings.Default.FontFamily);
                }
                if (!String.IsNullOrEmpty(Properties.Settings.Default.FontSize))
                {
                    CustomFontHelper.bCustomFont = true;
                    FontSize = Convert.ToDouble(Properties.Settings.Default.FontSize);
                }
            }
            catch
            {
            }

            foreach (var el in dockingManager.Children.OfType<FrameworkElement>())
                DockingManager.SetAnimateOnNewItemAdded(el, false);

            componentHost = new ComponentHost();
            workSpaceComponent = new WorkSpaceComponent(this);
            componentHost.Components.Add(workSpaceComponent);

            MainWnd.ContextMenu = GetDockingManagerDockedList();
            MainWnd.ContextMenuOpening += (o, e) =>
                {
                    MainWnd.ContextMenu = GetDockingManagerDockedList();
                };

            memoryCheckTimer = new DispatcherTimer(DispatcherPriority.Send);
            memoryCheckTimer.Interval = TimeSpan.FromSeconds(3);
            memoryCheckTimer.Tick += (o, e) => 
            { 
                CheckMemoryUsage(); 
            };
            memoryCheckTimer.Start();

            PreviewKeyDown += (o, e) =>
            {
                if (e.Key == Key.System && e.SystemKey == Key.F4)
                {
                    e.Handled = true;
                    try
                    {
                        Close();
                    }
                    catch
                    { }
                }
            };
        }

        #endregion ctor

        public void AddAndInitializeComponent(IComponent component)
        {
            componentHost.Components.Add(component);
            if (component is IUFInterfaceBase)
                (component as IUFInterfaceBase).Initialize();
        }

        #region Command arguments

        internal void CheckCommandArguments(String[] commands = null)
        {
            var commandLine = commands;
            if (commands == null)
                commandLine = Environment.GetCommandLineArgs();
            else
                commandLine = commands;
            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(commandLine);

            bool bRegister = commandArgs.ArgPairs.ContainsKey("registertypes");
            bool bUnregister = commandArgs.ArgPairs.ContainsKey("unregistertypes");
            if (bRegister || bUnregister)
            {
                var uriRisolver = componentHost.GetService(typeof(IUriRisolver)) as IUriRisolver;
                uriRisolver.RegisterFileTypes(bRegister);
                Visibility = System.Windows.Visibility.Hidden;
                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                    {
                        Visibility = System.Windows.Visibility.Hidden;
                        Dispatcher.CurrentDispatcher.InvokeShutdown();
                    });
                return;
            }

            // if (commandArgs.Params.Count > 1)
            {
                var uriRisolver = componentHost.GetService(typeof(IUriRisolver)) as IUriRisolver;

                String file = String.Empty;
                if (commandArgs.ArgPairs.ContainsKey("file"))
                    file = commandArgs.ArgPairs["file"];
                else if (commandArgs.Params.Count > 1)
                    file = commandArgs.Params[1];

                //for (int i = 1; i < commandArgs.Params.Count; ++i)
                if (!String.IsNullOrEmpty(file))
                {
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

                        var manager = uriRisolver.ResolveUri(uri) as IDocumentManager;
                        if (uriFile != null)
                            manager.Edit(uriFile, null);
                        else
                            manager.Edit(uri, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format(Properties.Resources.ErrorEditingCommandArgument, file),
                            Properties.Settings.Default.MainWindowTitle, MessageBoxButton.OK);
                    }
                }
            }
        }

        #endregion

        #region Docking Manager

        ContextMenu GetDockingManagerDockedList()
        {
            var menuitem = new ContextMenu();
            bool bCanApplyCustomFont = CustomFontHelper.CanApplyCustomFont();

            for (int i = 0; i < dockingManager.Children.Count; i++)
            {
                FrameworkElement dockitem = dockingManager.Children[i] as FrameworkElement;
                if (dockitem != null)
                {
                    if ((UFInterfaces.DockState)DockingManager.GetState(dockitem) != UFInterfaces.DockState.Document)
                    {
                        //var image = (DockingManager.GetIcon(dockitem) as ImageBrush).ImageSource as BitmapImage;
                        //var icon = new BitmapImage(image.UriSource);
                        var childMenuItem = new MenuItem()
                        {
                            Header = DockingManager.GetHeader(dockitem)
                            // Icon = icon
                        };
                        if (bCanApplyCustomFont)
                        {
                            childMenuItem.FontSize = CustomFontHelper.GetCustomFontSize();
                            childMenuItem.FontFamily = CustomFontHelper.GetCustomFontFamily();
                        }
                        menuitem.Items.Add(childMenuItem);

                        childMenuItem.Click += (o, e) =>
                            {
                                workSpaceComponent.FlashDockedElement(dockitem);
                            };
                    }
                }
            }

            return menuitem;
        }

        #endregion Overrides

        #region Window events

        public bool HasContentRendered()
        {
            return bContentRenderedExecuted;
        }

        bool bContentRenderedExecuted;
#if !DEBUG
        DispatcherTimer timer;
#endif
        DispatcherTimer tmr;
        private void MainWnd_ContentRendered(object sender, EventArgs e)
        {
            if (bContentRenderedExecuted)
                return;

            bContentRenderedExecuted = true;

            #region Loading RibbonState

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                using (new WaitCursor())
                {
                    try
                    {
                        Ribbons.PersistElements.Add(RibbonElements.QuickAccessToolbar);
                        Ribbons.PersistElements.Add(RibbonElements.Ribbon);
                        Ribbons.LoadRibbonState();
                    }
                    catch (Exception ex)
                    {
                        logGeneral.Error(Properties.Resources.FailedToLoadAppState, ex);
                        System.Diagnostics.Trace.TraceError(ex.ToString());
                    }
                }
            }

            #endregion

            if (tmr == null)
            {
                tmr = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.ApplicationIdle, (ob, ev) =>
                {
                    try
                    {
                        if (!bActiveWindowChangedInExecution)
                        {
                            var el = FocusManager.GetFocusedElement(this) as FrameworkElement;
                            if (el != null && dockingManager.Children.Contains(el) && dockingManager.ActiveWindow != el)
                                dockingManager.ActiveWindow = el;
                            // System.Diagnostics.Debug.Write(String.Format("Focus element = {0}, Focus Scope = {1}", el, active));
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }, Dispatcher.CurrentDispatcher);
                tmr.Start();
            }

#if !DEBUG
            bool bFound = false;
            int nCounter = 0;
            MSZ.MSZView.KeyChangeEvent += (ob, ev) =>
            {
                var mode = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxWuNSo2Dv1fX3PHz66oYOgA=="/* DEV */);
                Dispatcher.InvokeIfRequired(() =>
                {
                    if (ev.MSZState || !mode)
                    {
                        if (!bFound || !mode)
                        {
                            bFound = true;
                            if (timer == null)
                            {
                                timer = new DispatcherTimer();
                                timer.Interval = TimeSpan.FromMinutes(0);
                                timer.IsEnabled = true;
                                timer.Start();
                                nCounter = 0;
                            timer.Tick += (to, te) =>
                            {
                                timer.Stop();
                                //var control = new MSZui.UserControl1(++nCounter);
                                nCounter++;
                                var control = new MSZui.UserControl1(nCounter, (nCounter > 1 ? Properties.Resources.LicenseNotFound : null));
                                //SetBusyContent(control.GetText());
                                //SetBusy(true);

                                // simpleLogging.AddItem(Properties.Resources.LicenseManager, Properties.Resources.LicenseNotFound);
                                logLicense.Warn(Properties.Resources.LicenseNotFound);

                                var wnd = new Window()
                                {
                                    Owner = this,
                                    Topmost = true,
                                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner,
                                    Content = control,
                                    WindowStyle = System.Windows.WindowStyle.None,
                                    SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                                    WindowState = System.Windows.WindowState.Normal,
                                    ShowInTaskbar = false,
                                    ResizeMode = System.Windows.ResizeMode.NoResize
                                };

                                wnd.Closing += (obj, eve) =>
                                {
                                    eve.Cancel = !control.canClose;
                                };
                                wnd.ShowDialog();
                                wnd.Close();

                                //SetBusy(false);
                                //SetBusyContent("");

                                if (nCounter > 40)
                                    System.Environment.Exit(-10);
                                else if (nCounter > 20)
                                    Application.Current.Shutdown();

                                timer.Interval = TimeSpan.FromMinutes(120);
                                timer.Start();
                            };
                            }
                        }
                    }
                    else
                    {
                        if (bFound)
                        {
                            bFound = false;
                            nCounter = 0;
                            if (timer != null)
                                timer.Stop();
                        }
                    }
                });
            };
            MSZ.MSZView.Read();
#endif

#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                var ret = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxY+APFmQPtOWSUaMJjYrwAg=="/* DBG */);
                if (!ret)
                    return;
            }
#endif

            // Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            // {
                workSpaceComponent.OnContentRendered(this);
            //});

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                int displayTier = (RenderCapability.Tier >> 16);

                if (displayTier == 0)
                {
                    notifyIcon.BalloonTipText = String.Format(Properties.Resources.NoHardwareAccelerationFound,
                        System.Environment.Is64BitProcess ? "64" : "32", 
                        SysInfo.GetIndexPerformance(), SysInfo.GetIndexGraphicsPerformance(), SysInfo.GetNumberOfLogicalProcessors());
                    notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Error;
                    //no hardware acceleration
                }
                else if (displayTier == 1)
                {
                    notifyIcon.BalloonTipText = String.Format(Properties.Resources.PartialHardwareAccelerationFound,
                        System.Environment.Is64BitProcess ? "64" : "32",
                        SysInfo.GetIndexPerformance(), SysInfo.GetIndexGraphicsPerformance(), SysInfo.GetNumberOfLogicalProcessors());
                    notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
                    //partial hardware acceleration
                }
                else
                {
                    notifyIcon.BalloonTipText = String.Format(Properties.Resources.HardwareAccelerationFound,
                        System.Environment.Is64BitProcess ? "64" : "32",
                        SysInfo.GetIndexPerformance(), SysInfo.GetIndexGraphicsPerformance(), SysInfo.GetNumberOfLogicalProcessors());
                    notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
                    //supports hardware acceleration
                }

                notifyIcon.BalloonTipTitle = Title;
                notifyIcon.ShowBalloonTip(3000);
            });

            //#region Expiring

            //if (DateTime.UtcNow > new DateTime(2011, 12, 1))
            //{
            //    Action action = () => Application.Current.Shutdown();
            //    Dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
            //}

            //#endregion

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal;

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() =>
            {
                Ribbons.SelectedIndex = 0;
            }));

            Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                {
                    AutoUpdaterDotNET.AutoUpdater.Start(Properties.Settings.Default.UpdateServiceURL);
                });
        }

        /*
        DispatcherOperation checkActiveWindow;
        void CheckActivateWindow()
        {
            foreach (Window wnd in Application.Current.Windows)
            {
                var ret = typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(wnd);
                if (ret != null && ret is bool && (bool)ret == true)
                {
                    gridWait.Visibility = Visibility.Visible;
                    SetBusy(false);
                    // Mouse.OverrideCursor = Cursors.Arrow;
                    return;
                }
            }

            gridWait.Visibility = Visibility.Collapsed;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            if (checkActiveWindow == null)
            {
                Action action = () =>
                {
                    CheckActivateWindow();
                };
                checkActiveWindow = Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);
                checkActiveWindow.Completed += (o, ev) =>
                {
                    checkActiveWindow = null;
                };
            }

            base.OnDeactivated(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            if (checkActiveWindow == null)
            {
                Action action = () =>
                {
                    CheckActivateWindow();
                };
                checkActiveWindow = Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);
                checkActiveWindow.Completed += (o, ev) =>
                    {
                        checkActiveWindow = null;
                    };
            }

            base.OnActivated(e);

#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                var ret = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxY+APFmQPtOWSUaMJjYrwAg=="/* DBG /*);
                if (!ret)
                    System.Environment.Exit(-10);
            }
#endif
        }
        */

        bool bLoaded;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bLoaded)
                return;
            bLoaded = true;

            using (new WaitCursor())
            {
                System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;

                ResourceDictionaryExtensions.AddCommonResources(this);

                bLoadingDockingState = true;

                ApplicationPropertiesHelper.LoadApplicationProperties(GetStorage(), GetStoreFileName());

                var name = Assembly.GetExecutingAssembly().GetName().Name;
                using (var Mutex = new Mutex(false, name))
                {
                    try
                    {
                        Mutex.WaitOne();
                    }
                    catch (AbandonedMutexException ex)
                    {
                        logGeneral.Warn("AbandonedMutexException", ex);
                    }

                    try
                    {
                        #region Theme

                        // Background = Brushes.Black;
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            SetSkin(Properties.Settings.Default.Skin);
                        }
                        else
                        {
                            Properties.Settings.Default.Reset();
                            SetSkin(Properties.Settings.Default.Skin);
                        }

                        #endregion

                        #region PluginDiscovering

                        //Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        //{
                        System.Diagnostics.Trace.TraceInformation("Checking Plugins...");

                        //Call the find plugins routine, to search in our Plugins Folder
                        Global.Plugins.FindPlugins(AppDomain.CurrentDomain.BaseDirectory, "*ViewModel*.dll");
                        // Global.Plugins.FindPlugins(AppDomain.CurrentDomain.BaseDirectory, "*Manager*.dll");
                        // Global.Plugins.FindPlugins(AppDomain.CurrentDomain.BaseDirectory, "*Provider*.dll");

                        Global.Plugins.FindPlugins(String.Format("{0}CommonPlugins\\", AppDomain.CurrentDomain.BaseDirectory));
                        Global.Plugins.FindPlugins(String.Format("{0}DesignPlugins\\", AppDomain.CurrentDomain.BaseDirectory));
                        Global.Plugins.FindPlugins(String.Format("{0}Managers\\", AppDomain.CurrentDomain.BaseDirectory));

                        Global.Plugins.FindPlugins(String.Format("{0}DataSinks\\", AppDomain.CurrentDomain.BaseDirectory));

                        foreach (var pluginOn in Global.Plugins.AvailablePlugins)
                        {
                            if (((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance is IComponent)
                            {
                                try
                                {
                                    componentHost.Components.Add(((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance as IComponent);
                                }
                                catch (Exception ex)
                                {
                                    logGeneral.Error(Properties.Resources.FailedToLoadPlugin, ex);
                                }
                            }
                        }

                        foreach (var pluginOn in Global.Plugins.AvailablePlugins)
                        {
                            try
                            {
                                ((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance.Initialize();
                            }
                            catch (Exception ex)
                            {
                                logGeneral.Error(Properties.Resources.FailedToInitializePlugin, ex);
                            }
                        }

                        simpleLogging = componentHost.GetService(typeof(ISimpleLogging)) as ISimpleLogging;

                        // we need to register OPC UA view Model to let it get other services
                        var opcuaservice = componentHost.GetService(typeof(IOPCUAViewModelService)) as IOPCUAViewModelService;

                        #endregion PluginDiscovering

                        workSpaceComponent.OnWorkspaceLoading(this);

                        #region Loading DockingManager State

                        // Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        //    {
                        // bLoadingDockingState = true;

                        // dockingManager.IgnoreNamesOnDeserialize = true;
                        eliteDockedToBeSaved = (from c in dockingManager.Children.OfType<FrameworkElement>()
                                                select c).ToList();
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            using (new WaitCursor())
                            {
                                try
                                {
                                    lastLoadedDockingState = null;
                                    if (IsInEasyMode)
                                        LoadDockStates(sEasyModeDockState);
                                    else
                                        dockingManager.LoadDockState();
                                }
                                catch (Exception ex)
                                {
                                    logGeneral.Error(Properties.Resources.FailedToLoadAppState, ex);
                                    System.Diagnostics.Trace.TraceError(ex.ToString());
                                }
                            }
                        }

                        // bLoadingDockingState = false;
                        //     });

                        #endregion

                        /*
                        #region Metro Brushes
                        MetroBrushes.Items.Add(new BrushModel() { Brush = Brushes.Magenta, Name = "Magenta" });
                        MetroBrushes.Items.Add(new BrushModel() { Brush = Brushes.Purple, Name = "Purple" });
                        MetroBrushes.Items.Add(new BrushModel() { Brush = Brushes.Teal, Name = "Teal" });
                        MetroBrushes.Items.Add(new BrushModel() { Brush = Brushes.Lime, Name = "Lime" });
                        MetroBrushes.Items.Add(new BrushModel() { Brush = Brushes.Brown, Name = "Brown" });
                        MetroBrushes.Items.Add(new BrushModel() { Brush = Brushes.Pink, Name = "Pink" });
                        MetroBrushes.Items.Add(new BrushModel() { Brush = Brushes.Orange, Name = "Orange" });
                        MetroBrushes.Items.Add(new BrushModel() { Brush = Brushes.LightBlue, Name = "Blue" });
                        MetroBrushes.Items.Add(new BrushModel() { Brush = Brushes.Red, Name = "Red" });
                        MetroBrushes.Items.Add(new BrushModel() { Brush = Brushes.Green, Name = "Green" });

                        BckBrushes.Items.Add(new BrushModel() { Name = "Black", Brush = Brushes.Black });
                        BckBrushes.Items.Add(new BrushModel() { Name = "White", Brush = Brushes.White });
                        BckBrushes.Items.Add(new BrushModel() { Name = "Gray", Brush = Brushes.Gray });

                        FgBrushes.Items.Add(new BrushModel() { Name = "Black", Brush = Brushes.Black });
                        FgBrushes.Items.Add(new BrushModel() { Name = "White", Brush = Brushes.White });
                        FgBrushes.Items.Add(new BrushModel() { Name = "Gray", Brush = Brushes.Gray });
                        FgBrushes.Items.Add(new BrushModel() { Brush = Brushes.Pink, Name = "Pink" });
                        FgBrushes.Items.Add(new BrushModel() { Brush = Brushes.Orange, Name = "Orange" });
                        FgBrushes.Items.Add(new BrushModel() { Brush = Brushes.LightBlue, Name = "Blue" });
                        FgBrushes.Items.Add(new BrushModel() { Brush = Brushes.Red, Name = "Red" });
                        FgBrushes.Items.Add(new BrushModel() { Brush = Brushes.Green, Name = "Green" });
                        #endregion
                        */

                        #region Loading Window Position

                        // Background = Brushes.Black;
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            //try
                            //{
                            //    dockingManager.LoadDockState();
                            //}
                            //catch (Exception ex)
                            //{
                            //    System.Diagnostics.Trace.TraceError(ex.ToString());
                            //}

                            Utilities.WINDOWPLACEMENT wp = (Utilities.WINDOWPLACEMENT)Properties.Settings.Default.WindowPlacement;
                            if (wp.showCmd != 0)
                                SaveWindowPlacementState.Load(this, wp);
                        }
                        else
                        {
                            Properties.Settings.Default.Reset();
                            // dockingManager.ResetState();
                            // Ribbons.ResetRibbonState();
                        }

                        #endregion

                        CheckCommandArguments();

                        //});

                        workSpaceComponent.OnWorkspaceLoaded(this);
                    }
                    finally
                    {
                        Mutex.ReleaseMutex();
                    }
                }

                bLoadingDockingState = false;
            }
        }

        bool bShuttingDown;
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var windowArray = new Window[Application.Current.Windows.Count];
            Application.Current.Windows.CopyTo(windowArray, 0);

            foreach (Window window in windowArray)
            {
                if (window == this)
                    continue;
                try
                {
                    window.Close();
                }
                catch
                { }

                if (!window.IsVisible)
                    continue;
                foreach (var wnd in Application.Current.Windows)
                {
                    if (wnd == window)
                    {
                        e.Cancel = true;
                        logGeneral.Error(Properties.Resources.CanceledClosingByAnotherWindow);
                        return;
                    }
                }
            }

            var windowArrayOwner = new Window[OwnedWindows.Count];
            OwnedWindows.CopyTo(windowArrayOwner, 0);

            foreach (Window window in windowArrayOwner)
            {
                if (window == this)
                    continue;
                window.Close();

                if (!window.IsVisible)
                    continue;
                foreach (var wnd in OwnedWindows)
                {
                    if (wnd == window)
                    {
                        e.Cancel = true;
                        logGeneral.Error(Properties.Resources.CanceledClosingByOwnedWindow);
                        return;
                    }
                }
            }

            workSpaceComponent.OnClosing(sender, e);

            if (e.Cancel == false)
            {
                bShuttingDown = true;
                workSpaceComponent.Shutdown();

                dockingManager.WindowActivated -= DockingManager_WindowActivated;
                dockingManager.WindowDeactivated -= DockingManager_WindowDeactivated;
                dockingManager.WindowDragEnd -= DockingManager_WindowDragEnd;
                dockingManager.WindowDragStart -= DockingManager_WindowDragStart;
                dockingManager.ActiveWindowChanging -= DockingManager_ActiveWindowChanging;
                dockingManager.ActiveWindowChanged -= DockingManager_ActiveWindowChanged;
                dockingManager.BeforeContextMenuOpen -= DockingManager_BeforeContextMenuOpen;

                dockingManager.CloseAllTabs -= DockingManager_CloseAllTabs;
                dockingManager.CloseButtonClick -= DockingManager_CloseButtonClick;
                dockingManager.CloseOtherTabs -= DockingManager_CloseOtherTabs;
                dockingManager.DockStateChanged -= DockingManager_DockStateChanged;
                dockingManager.ElementHidden -= DockingManager_ElementHidden;
                dockingManager.ElementShown -= DockingManager_ElementShown;

                using (new WaitCursor())
                {
                    if (isInFullScreenMode)
                        OnFullScreen(this, null);

                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        using (new WaitCursor())
                        {
                            Properties.Settings.Default.WindowPlacement = SaveWindowPlacementState.Save(this);

                            if (dockingManager.Children.Count > 0)
                            {
                                for (int i = 0; i < dockingManager.Children.Count; i++)
                                {
                                    FrameworkElement dockitem = dockingManager.Children[i] as FrameworkElement;
                                    if (dockitem != null)
                                    {
                                        if ((UFInterfaces.DockState)DockingManager.GetState(dockitem) == UFInterfaces.DockState.Dock)
                                        {
                                            if (DockingManager.GetDockWindowState(dockitem) == WindowState.Maximized)
                                            {
                                                DockingManager.SetDockWindowState(dockitem, WindowState.Normal);
                                            }
                                        }
                                    }
                                }
                            }

                            if (eliteDockedToBeSaved != null)
                            {
                                var docFound = (from c in dockingManager.Children.OfType<FrameworkElement>()
                                                where (UFInterfaces.DockState)DockingManager.GetState(c) == UFInterfaces.DockState.Document
                                                select c).ToList();
                                if (docFound.Count > 0)
                                {
                                    try
                                    {
                                        lastLoadedDockingState = null;
                                        if (IsInEasyMode)
                                            LoadDockStates(sEasyModeDockState);
                                        else
                                            dockingManager.LoadDockState();
                                    }
                                    catch (Exception ex)
                                    {
                                        logGeneral.Error(Properties.Resources.FailedToSaveAppState, ex);
                                    }
                                }

                                (from c in dockingManager.Children.OfType<FrameworkElement>()
                                 where !eliteDockedToBeSaved.Contains(c)
                                 select c)
                                    .ToList().ForEach(c =>
                                        {
                                            // workSpaceComponent.RemoveDockingChildren(c);
                                            dockingManager.Children.Remove(c);
                                        });

                                try
                                {
                                    lastSavedDocked = null;
                                    if (IsInEasyMode)
                                        SaveDockStates(sEasyModeDockState);
                                    else
                                        dockingManager.SaveDockState();
                                    Ribbons.SaveRibbonState();
                                    Properties.Settings.Default.Save();
                                    ApplicationPropertiesHelper.SaveApplicationProperties(GetStorage(), GetStoreFileName());
                                }
                                catch (Exception ex)
                                {
                                    logGeneral.Error(Properties.Resources.FailedToSaveAppState, ex);
                                }
                            }
                        }
                    }

                    workSpaceComponent.OnClosed(sender);

                    Visibility = Visibility.Collapsed;

                    // ThemeHelper.SetTheme(this, "");

                    Global.Plugins.ClosePlugins();

                    //dockingManager.Children.ForEach(child =>
                    //{
                    //    var disposables = (from c in child.GetChildrenOfType<FrameworkElement>() where c is IDisposable select c).ToList();

                    //    if (child is IDisposable)
                    //    {
                    //        IDisposable dispose = child as IDisposable;
                    //        dispose.Dispose();
                    //    }

                    //    disposables.ForEach(c =>
                    //    {
                    //        (c as IDisposable).Dispose();
                    //    });
                    //});

                    //dockingManager.Dispose();
                }
            }
            else
                logGeneral.Error(Properties.Resources.CanceledClosingByComppnent);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //using (new WaitCursor())
            //{
            //    Global.Plugins.ClosePlugins();

            //    notifyIcon.Dispose();

            //    // Environment.Exit(0);
            //}
        }

        #endregion Window events

        #region Skins

        void SetSkin(String style)
        {
            using (new WaitCursor())
            {
                // Ribbon.SetActiveColorScheme(MainWnd, Brushes.Black);

                ApplicationPropertiesHelper.SetProperty("CurrentSkinBackColor", Application.Current.MainWindow.Background);

                // SkinStorage.SetVisualStyle(this, style);
                // SkinStorage.SetVisualStyle(dockingManager, style);
                ApplicationPropertiesHelper.SetProperty("CurrentSkin", style);
                Properties.Settings.Default.Skin = style;

                ThemeHelper.SetTheme(this, style);
                ThemeHelper.SetDefaultUserLookAndFeel();

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

                ApplicationPropertiesHelper.SetProperty("CurrentSkinBackColor", Application.Current.MainWindow.Background);

                this.SetRibbonButtonStyle(Ribbons);
            }
        }

        //void SetRibbonButtonStyle(string skinName)
        //{
        //    var resourceKey = String.Format("{0}RibbonButtonStyle", skinName);
        //    Style resStyle = null;
        //    try
        //    {
        //        resStyle = FindResource(resourceKey) as Style;
        //    }
        //    catch (ResourceReferenceKeyNotFoundException)
        //    {
        //        ResourceDictionary resourceDictionary = new ResourceDictionary();
        //        resourceDictionary.Source = new Uri(String.Format("/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/{0}Style.xaml", skinName), UriKind.Relative);
        //        Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        //        try
        //        {
        //            resStyle = FindResource(resourceKey) as Style;
        //        }
        //        catch
        //        { }
        //    }
        //    if (resStyle != null)
        //    {
        //        Style ribbonButtonStyle = new Style(typeof(RibbonButton), resStyle);
        //        if (!String.IsNullOrEmpty(Properties.Settings.Default.FontSize))
        //            ribbonButtonStyle.Setters.Add(new Setter(FontSizeProperty, FontSize));
        //        if (!String.IsNullOrEmpty(Properties.Settings.Default.FontFamily))
        //            ribbonButtonStyle.Setters.Add(new Setter(FontFamilyProperty, FontFamily));
        //        foreach (var rb in Ribbons.GetChildrenOfType<RibbonButton>())
        //            rb.Style = ribbonButtonStyle;
        //    }
        //}

        /*
        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetBusy(true);
            Hide();
            SetSkin((comboTheme.SelectedItem as ComboBoxItem).Content as String);
            Show();
            SetBusy(false);
        }
        */

        //void SetThemeByRibbon(object sender, EventArgs e)
        //{
        //    RibbonButton button = sender as RibbonButton;
        //    SetSkin(button.Label.ToString());
        //}

        private void skin_change(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem ri in SkinChange.Items)
            {
                ri.IsChecked = false;
            }
            MenuItem menuItem = sender as MenuItem;
            SetSkin(menuItem.Header.ToString());
        }
        /*
        private void MetroBrushes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MetroBrushes.SelectedItem != null)
            {
                SkinStorage.SetMetroBrush(this, ((BrushModel)MetroBrushes.SelectedItem).Brush);
            }
        }

        private void BgBrushes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BckBrushes.SelectedItem != null)
            {
                SkinStorage.SetMetroPanelBackgroundBrush(this, ((BrushModel)BckBrushes.SelectedItem).Brush);
            }
        }

        private void FgBrushes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FgBrushes.SelectedItem != null)
            {
                SkinStorage.SetMetroForegroundBrush(this, ((BrushModel)FgBrushes.SelectedItem).Brush);
            }
        }
        */

        #endregion Skins

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

        #region Busy Content

        Window busyWindow;
        LongOperationControl longOperationControl;
        String busyContent;
        Thread newWindowThread;
        internal String GetBusyContent()
        {
            return busyContent;
        }

        internal void SetBusyContent(String waitText)
        {
            busyContent = waitText;
            if (busyWindow == null)
                return;

            busyWindow.Dispatcher.InvokeIfRequired(() =>
            {
                if (!String.IsNullOrEmpty(busyContent))
                    longOperationControl.BusyContent = busyContent;
                else
                    longOperationControl.BusyContent = Properties.Resources.WaitText;
            });
        }

        internal bool IsBusyVisible()
        {
            if (busyWindow == null)
                return false;
            bool bRet = false;
            busyWindow.Dispatcher.InvokeIfRequired(
                    () =>
                    {
                        bRet = busyWindow.IsVisible;
                    });
            return bRet;
        }

        internal void SetBusy(bool bSet)
        {
            if (bSet && busyWindow == null)
            {
                bool bStarted = false;
                bool bStopping = false;
                /*
                double top = Top;
                double left = Left;
                if (WindowState == System.Windows.WindowState.Maximized)
                {
                    top = 0;
                    left = 0;
                }
                double width = ActualWidth;
                double height = ActualHeight;
                 * */
                var wp = SaveWindowPlacementState.Save(this);
                wp.normalPosition.Left = (int)Left;
                wp.normalPosition.Top = (int)Top;
                wp.normalPosition.Right = wp.normalPosition.Left + (int)Width;
                wp.normalPosition.Bottom = wp.normalPosition.Top + (int)Height;
                if (newWindowThread == null)
                {
                    newWindowThread = new Thread((obj) =>
                    {
                        longOperationControl = new LongOperationControl();
                        busyWindow = new Window()
                        {
                            WindowStartupLocation = System.Windows.WindowStartupLocation.Manual,
                            SizeToContent = System.Windows.SizeToContent.Manual,
                            WindowStyle = System.Windows.WindowStyle.None,
                            ShowInTaskbar = false,
                            AllowsTransparency = true,
                            WindowState = System.Windows.WindowState.Normal,
                            Background = Brushes.Transparent,
                            Content = longOperationControl,
                            Cursor = Cursors.Wait,
                            ShowActivated = false,
                            IsEnabled = false
                        //Top = top,
                        //Left = left,
                        //Width = width,
                        //Height = height
                    };

                        if (!String.IsNullOrEmpty(busyContent))
                            longOperationControl.BusyContent = busyContent;

                        busyWindow.Loaded += (o, e) =>
                            {
                            // SaveWindowPlacementState.Load(busyWindow, wp);
                            if (wp.showCmd == 3 /* MaximizedMode*/)
                                {
                                    var rect = System.Windows.Forms.Screen.GetWorkingArea(new System.Drawing.Point(
                                        wp.normalPosition.Left +
                                        (wp.normalPosition.Right - wp.normalPosition.Left) / 2,
                                        wp.normalPosition.Top + (wp.normalPosition.Bottom - wp.normalPosition.Top) / 2));
                                    busyWindow.Top = rect.Top;
                                    busyWindow.Left = rect.Left;
                                    busyWindow.Width = rect.Right - rect.Left;
                                    busyWindow.Height = rect.Bottom - rect.Top;
                                }
                                else
                                {
                                    busyWindow.Top = wp.normalPosition.Top;
                                    busyWindow.Left = wp.normalPosition.Left;
                                    busyWindow.Width = wp.normalPosition.Right - wp.normalPosition.Left;
                                    busyWindow.Height = wp.normalPosition.Bottom - wp.normalPosition.Top;
                                }
                            };

                        try
                        {
                            busyWindow.Show();
                        }
                        catch { }

                        busyWindow.Closing += (o, e) =>
                        {
                            e.Cancel = !bStopping;
                        };

                        busyWindow.Dispatcher.ShutdownStarted += (o, e) =>
                        {
                            bStopping = true;
                            busyWindow.Close();
                        };
                    // busyWindow.IsEnabled = false;
                    bool bActivating = false;
                        busyWindow.Activated += (o, e) =>
                        {
                            if (!bActivating)
                            {
                                bActivating = true;
                                Dispatcher.BeginInvokeIfRequired(() =>
                                {
                                    Activate();
                                    busyWindow.Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        busyWindow.Activate();
                                        bActivating = false;
                                    });
                                });
                            }
                        };
                        bStarted = true;

                    // Start the new window's Dispatcher
                    System.Windows.Threading.Dispatcher.Run();
                    });

                    newWindowThread.SetApartmentState(ApartmentState.STA);
                    newWindowThread.IsBackground = true;
                    newWindowThread.Start();
                }

                //while (!bStarted)
                //    Thread.Sleep(100);
                    // WaitForPriority.DoEventsSync();
            }
            else
            {
                if (bSet)
                {
                    /*
                    double top = Top;
                    double left = Left;
                    if (WindowState == System.Windows.WindowState.Maximized)
                    {
                        top = 0;
                        left = 0;
                    }
                    double width = ActualWidth;
                    double height = ActualHeight;
                    */
                    var wp = SaveWindowPlacementState.Save(this);
                    wp.normalPosition.Left = (int)Left;
                    wp.normalPosition.Top = (int)Top;
                    wp.normalPosition.Right = wp.normalPosition.Left + (int)Width;
                    wp.normalPosition.Bottom = wp.normalPosition.Top + (int)Height;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(String.Format("Left {0}, Top {1}, Right {2}, Bottom {3}",
                        Left, Top,
                        Width, Height));
#endif
                    busyWindow.Dispatcher.InvokeIfRequired(() =>
                        {
                            if (!String.IsNullOrEmpty(busyContent))
                                longOperationControl.BusyContent = busyContent;
                            else
                                longOperationControl.BusyContent = Properties.Resources.WaitText;
                            if (wp.showCmd == 3 /* MaximizedMode*/)
                            {
                                var rect = System.Windows.Forms.Screen.GetWorkingArea(new System.Drawing.Point(
                                    wp.normalPosition.Left +
                                    (wp.normalPosition.Right - wp.normalPosition.Left) / 2,
                                    wp.normalPosition.Top + (wp.normalPosition.Bottom - wp.normalPosition.Top) / 2));
                                busyWindow.Top = rect.Top;
                                busyWindow.Left = rect.Left;
                                busyWindow.Width = rect.Right - rect.Left;
                                busyWindow.Height = rect.Bottom - rect.Top;
                            }
                            else
                            {
                                busyWindow.Top = wp.normalPosition.Top;
                                busyWindow.Left = wp.normalPosition.Left;
                                busyWindow.Width = wp.normalPosition.Right - wp.normalPosition.Left;
                                busyWindow.Height = wp.normalPosition.Bottom - wp.normalPosition.Top;
                            }
#if DEBUG
                            System.Diagnostics.Debug.WriteLine(String.Format("show {0}, left {1}, top {2}, right {3}, bottom {4}", 
                                wp.showCmd, 
                                wp.normalPosition.Left, wp.normalPosition.Top, 
                                wp.normalPosition.Right, wp.normalPosition.Bottom));
#endif
                            // SaveWindowPlacementState.Load(busyWindow, wp);
                            try
                            {
                                busyWindow.Show();
                            }
                            catch { }

                            // busyWindow.IsEnabled = false;
                            busyWindow.Visibility = Visibility.Visible;
                        });
                }
                else if (busyWindow != null)
                {
                    busyWindow.Dispatcher.InvokeIfRequired(() =>
                        {
                            busyWindow.Visibility = Visibility.Collapsed;
                            busyWindow.Hide();

                            SetBusyContent(null);
                        });
                }
            }
        }

        #endregion

        internal void AddDockingChild(FrameworkElement el)
        {
            var state = (UFInterfaces.DockState)DockingManager.GetState(el);
            var doNotRestoreDockingState = false;
            if (state == UFInterfaces.DockState.AutoHidden)
            {
                if (bLoadingDockingState == false)
                    bLoadingDockingState = true;
                else
                    doNotRestoreDockingState = true;
            }

            try
            {
                var oldVisibility = el.Visibility;
                el.Visibility = Visibility.Collapsed;
                // dockingManager.UpdateLayout();
                // dockingManager.BeginInit();
                dockingManager.Children.Add(el);
                // dockingManager.EndInit();
                el.Visibility = oldVisibility;
            }
            finally
            {
                if (!doNotRestoreDockingState)
                    bLoadingDockingState = false;
            }
        }

        FrameworkElement GetFirstDocumentDocked(ref int i)
        {
            for (; i < dockingManager.Children.Count; ++i)
            {
                if ((UFInterfaces.DockState)DockingManager.GetState(dockingManager.Children[i]) == UFInterfaces.DockState.Document)
                    return dockingManager.Children[i];
            }

            return null;
        }

        internal void RemoveDockingChildren(FrameworkElement el)
        {
            var state = (UFInterfaces.DockState)DockingManager.GetState(el);
            var doNotRestoreDockingState = false;
            if (state == UFInterfaces.DockState.AutoHidden)
            {
                if (bLoadingDockingState == false)
                    bLoadingDockingState = true;
                else
                    doNotRestoreDockingState = true;
            }
            try
            {
                var oldVisibility = el.Visibility;
                // dockingManager.UpdateLayout();
                // dockingManager.BeginInit();
                dockingManager.Children.Remove(el);
                // dockingManager.EndInit();
                el.Visibility = oldVisibility;
            }
            finally
            {
                if (!doNotRestoreDockingState)
                    bLoadingDockingState = false;
            }
        }

        #region Docking Events

        DelayedSingleActionInvoker activateInvoker;
        FrameworkElement elnew;
        FrameworkElement elold;
        bool bLoadedDockingState;
        bool bActiveWindowChanging;
        private void DockingManager_ActiveWindowChanging(FrameworkElement element, ActiveWindowChangingEventArgs e)
        {
            if (bLoadingDockingState || bShuttingDown || bActiveWindowChanging)
                return;

            bActiveWindowChanging = true;
            try
            {
                if (Properties.Settings.Default.AutoLoadWorkspace)
                {
                    var old = e.OldValue as FrameworkElement;
                    if (old != null && ((UFInterfaces.DockState)DockingManager.GetState(old) == UFInterfaces.DockState.Document))
                        elold = old;
                    var neW = e.NewValue as FrameworkElement;
                    if (neW != null && ((UFInterfaces.DockState)DockingManager.GetState(neW) == UFInterfaces.DockState.Document))
                        elnew = neW;

                    bool bSame = false;
                    if (elnew != null && elold != null && elnew.GetType().Name == elold.GetType().Name)
                        bSame = true;
                    if (!bShuttingDown && !bSame && elnew != elold)
                    {
                        if (elold != null && (lastLoadedDockingState == null || lastLoadedDockingState == elold.GetType().Name))
                            SaveDockStates(elold.GetType().Name);

                        if (activateInvoker == null)
                            activateInvoker = new DelayedSingleActionInvoker(() =>
                            {
                                if (!bShuttingDown && elnew != null)
                                {
                                    var stateToLoad = elnew.GetType().Name;
                                    if (lastLoadedDockingState != stateToLoad)
                                    {
                                        if (String.IsNullOrEmpty(lastLoadedDockingState))
                                        {
                                            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                                            {
                                                using (new WaitCursor())
                                                {
                                                    try
                                                    {
                                                        lastSavedDocked = null;
                                                        if (IsInEasyMode)
                                                            SaveDockStates(sEasyModeDockState);
                                                        else
                                                            dockingManager.SaveDockState();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        logGeneral.Error(Properties.Resources.FailedToLoadAppState, ex);
                                                        System.Diagnostics.Trace.TraceError(ex.ToString());
                                                    }
                                                }
                                            }
                                        }

                                        bLoadedDockingState = true;
                                        LoadDockStates(stateToLoad);

                                        dockingManager.ActiveWindow = elnew;
                                        elnew.Focusable = true;
                                        elnew.Focus();
                                        FocusManager.SetFocusedElement(elnew, elnew);
                                    // elnew = null;
                                }
                                }
                            }, TimeSpan.FromMilliseconds(100), DispatcherPriority.Background);

                        if (activateInvoker != null)
                            activateInvoker.BeginInvoke();
                    }
                }

                workSpaceComponent.OnActiveWindowChanging(element, e);
            }
            finally
            {
                bActiveWindowChanging = false;
            }
        }

        bool bActiveWindowChangedInExecution;
        private void DockingManager_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (bLoadingDockingState || bShuttingDown)
                return;

            bActiveWindowChangedInExecution = true;
            try
            {
                workSpaceComponent.OnActiveWindowChanged(d, e);
            }
            finally
            {
                bActiveWindowChangedInExecution = false;
            }
        }

        private void DockingManager_WindowVisibilityChanged(object sender, RoutedEventArgs e)
        {
            if (bLoadingDockingState)
                return;

            workSpaceComponent.OnWindowVisibilityChanged(sender, e);
        }

        internal void ForceAnimationStopEventDockingBogus(FrameworkElement element)
        {
            /*
            if (listAutoStartSent.Contains(element))
            {
                listAutoStartSent.Remove(element);
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("Force AutoHideStop '{0}'", element.Name));
#endif
                var reaStop = new RoutedEventArgs(DockingManager.AutoHideAnimationStopEvent, element);
                workSpaceComponent.OnAutoHideAnimationStop(this, reaStop);
            }
             * */
        }

        List<FrameworkElement> listActivated = new List<FrameworkElement>();
        private void DockingManager_WindowActivated(object sender, RoutedEventArgs e)
        {
            if (bLoadingDockingState)
                return;

            workSpaceComponent.OnWindowActivated(sender, e);

            var fe = e.OriginalSource as FrameworkElement;
            if (fe == null)
                return;
            var state = (UFInterfaces.DockState)DockingManager.GetState(fe);
            if (state != UFInterfaces.DockState.AutoHidden)
                return;
            if (listActivated.Contains(fe))
                return;
            listActivated.Add(fe);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("Activated : Force AutoHideStart '{0}'", fe.Name));
#endif
            var reaStart = new RoutedEventArgs(DockingManager.AutoHideAnimationStartEvent, fe);
            workSpaceComponent.OnAutoHideAnimationStart(sender, reaStart);

#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("Activated : Force AutoHideStop '{0}'", fe.Name));
#endif
            var reaStop = new RoutedEventArgs(DockingManager.AutoHideAnimationStopEvent, fe);
            workSpaceComponent.OnAutoHideAnimationStop(sender, reaStop);

            if (e.OriginalSource == gridLogView && gridLogView.Children.Count == 0)
            {
                gridLogView.Children.Add(new LogViewer.LogViewer());
            }
        }

        private void DockingManager_WindowDeactivated(object sender, RoutedEventArgs e)
        {
            if (bLoadingDockingState)
                return;

            workSpaceComponent.OnWindowDeactivated(sender, e);

            var fe = e.OriginalSource as FrameworkElement;
            if (fe == null)
                return;
            var state = (UFInterfaces.DockState)DockingManager.GetState(fe);
            if (state != UFInterfaces.DockState.AutoHidden)
                return;
            if (!listActivated.Contains(fe))
                return;
            listActivated.Remove(fe);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("Deactivated : Force AutoHideStart '{0}'", fe.Name));
#endif
            var reaStart = new RoutedEventArgs(DockingManager.AutoHideAnimationStartEvent, fe);
            workSpaceComponent.OnAutoHideAnimationStart(sender, reaStart);

#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("Deactivated : Force AutoHideStop '{0}'", fe.Name));
#endif
            var reaStop = new RoutedEventArgs(DockingManager.AutoHideAnimationStopEvent, fe);
            workSpaceComponent.OnAutoHideAnimationStop(sender, reaStop);

            if (e.OriginalSource == gridLogView && gridLogView.Children.Count == 0)
            {
                gridLogView.Children.Add(new LogViewer.LogViewer());
            }
        }

        private void DockingManager_WindowDragEnd(object sender, RoutedEventArgs e)
        {
            workSpaceComponent.OnWindowDragEnd(sender, e);
        }

        private void DockingManager_WindowDragStart(object sender, RoutedEventArgs e)
        {
            workSpaceComponent.OnWindowDragStart(sender, e);
        }

        private void DockingManager_BeforeContextMenuOpen(object sender, RoutedEventArgs e)
        {
            workSpaceComponent.OnBeforeContextMenuOpen(sender, e);
        }

        private void DockingManager_CloseAllTabs(object sender, CloseTabEventArgs e)
        {
            workSpaceComponent.OnCloseAllTabs(sender, e);
        }

        bool bCloseButtonSent;
        private void DockingManager_CloseButtonClick(object sender, Syncfusion.Windows.Tools.Controls.CloseButtonEventArgs e)
        {
            workSpaceComponent.OnCloseButtonClick(sender, e);

            if (e.TargetItem is FrameworkElement && !e.Cancel)
            {
                var fe = e.TargetItem as FrameworkElement;
                //var disposables = (from c in fe.GetChildrenOfType<FrameworkElement>() where c is IDisposable select c).ToList();

                if (e.TargetItem is IDisposable)
                {
                    IDisposable dispose = e.TargetItem as IDisposable;
                    dispose.Dispose();
                }

                bCloseButtonSent = !bCloseButtonSent;
                //disposables.ForEach(c =>
                //{
                //    (c as IDisposable).Dispose();
                //});

                SaveCurrentWorkspace(fe);

                var docFound = (from c in dockingManager.Children.OfType<FrameworkElement>()
                                where (UFInterfaces.DockState)DockingManager.GetState(c) == UFInterfaces.DockState.Document
                                select c).ToList();

                if (docFound.Count > 0)
                    dockingManager.ActiveWindow = docFound[0];
            }
        }

        private DelayedSingleActionInvoker delayInvoker;
        internal void SaveCurrentWorkspace(FrameworkElement fe)
        {
            if (Properties.Settings.Default.AutoLoadWorkspace)
            {
                if ((UFInterfaces.DockState)DockingManager.GetState(fe) == UFInterfaces.DockState.Document)
                {
                    if (lastLoadedDockingState == fe.GetType().Name)
                        SaveDockStates(fe.GetType().Name);
                    if (elold == fe)
                        elold = null;
                    if (elnew == fe)
                        elnew = null;
                }

                if (delayInvoker == null)
                    delayInvoker = new DelayedSingleActionInvoker(() =>
                    {
                        if (bShuttingDown)
                            return;

                        var docFound = (from c in dockingManager.Children.OfType<FrameworkElement>()
                                        where (UFInterfaces.DockState)DockingManager.GetState(c) == UFInterfaces.DockState.Document
                                        select c).ToList();
                        if (docFound.Count == 0)
                        {
                            bLoadedDockingState = false;
                            lastLoadedDockingState = null;
                            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                            {
                                using (new WaitCursor())
                                {
                                    try
                                    {
                                        lastLoadedDockingState = null;
                                        if (IsInEasyMode)
                                            LoadDockStates(sEasyModeDockState);
                                        else
                                            dockingManager.LoadDockState();
                                    }
                                    catch (Exception ex)
                                    {
                                        logGeneral.Error(Properties.Resources.FailedToLoadAppState, ex);
                                        System.Diagnostics.Trace.TraceError(ex.ToString());
                                    }
                                }
                            }
                        }
                    });
                if (!bShuttingDown)
                    delayInvoker.BeginInvoke();
            }
        }

        private void DockingManager_CloseOtherTabs(object sender, CloseTabEventArgs e)
        {
            workSpaceComponent.OnCloseOtherTabs(sender, e);
        }

        private void DockingManager_DockStateChanged(FrameworkElement sender, Syncfusion.Windows.Tools.Controls.DockStateEventArgs e)
        {
            if (workSpaceComponent != null && !bLoadingDockingState)
            {
                if ((UFInterfaces.DockState)e.NewState == UFInterfaces.DockState.AutoHidden && dockingManager.ActiveWindow == sender)
                {
                    dockingManager.ActiveWindow = null;
                }
                else if ((UFInterfaces.DockState)e.NewState == UFInterfaces.DockState.Hidden && (UFInterfaces.DockState)e.OldState == UFInterfaces.DockState.Document)
                {
                    if (bCloseButtonSent)
                        bCloseButtonSent = false;
                    else
                    {
                        var closeArg = new Syncfusion.Windows.Tools.Controls.CloseButtonEventArgs(sender);
                        DockingManager_CloseButtonClick(sender, closeArg);
                        bCloseButtonSent = false;
                        if (closeArg.Cancel)
                        {
                            DockingManager.SetState(sender, Syncfusion.Windows.Tools.Controls.DockState.Document);
                        }
                        else if (dockingManager.Children.Contains(sender))
                            dockingManager.Children.Remove(sender);
                    }
                }
                /*
                if (e.OldState == DockState.AutoHidden || e.NewState == DockState.AutoHidden)
                {
                    // listDockStateChanged.Add(sender);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(String.Format("AutoHideStart '{0}'", sender.Name));
#endif
                    var reaStart = new RoutedEventArgs(DockingManager.AutoHideAnimationStartEvent, sender);
                    workSpaceComponent.OnAutoHideAnimationStart(sender, reaStart);

#if DEBUG
                    System.Diagnostics.Debug.WriteLine(String.Format("AutoHideStop '{0}'", sender.Name));
#endif
                    var reaStop = new RoutedEventArgs(DockingManager.AutoHideAnimationStopEvent, sender);
                    workSpaceComponent.OnAutoHideAnimationStop(sender, reaStop);
                }
                */
            }
            
            workSpaceComponent.OnDockStateChanged(sender, e);
        }

        private void DockingManager_ElementHidden(object sender)
        {
            workSpaceComponent.OnElementHidden(sender);
        }

        private void DockingManager_ElementShown(object sender)
        {
            workSpaceComponent.OnElementShown(sender);
        }

        #endregion Docking Events

        #region Ribbon Events

        private void Ribbons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bLoaded)
                workSpaceComponent.OnRibbonSelectionChanged(sender, e);
        }

        #endregion Ribbon Events

        #region Commands

        private void OnFileExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void OnGotFocus(object sender, RoutedEventArgs e)
        {
            DisplayCommandDescription(e.Source);
        }

        public void OnLostFocus(object sender, RoutedEventArgs e)
        {
            ClearCommandDescription(e.Source);
        }

        public void OnMouseEnter(object sender, MouseEventArgs e)
        {
            DisplayCommandDescription(sender);
        }

        public void OnMouseLeave(object sender, MouseEventArgs e)
        {
            ClearCommandDescription(sender);
        }

        private void DisplayCommandDescription(object sender)
        {
            MenuItem item = sender as MenuItem;
            if ((null != item) && (null != item.Command))
            {
                GeneralCommand command = item.Command as GeneralCommand;
                if (null != command)
                {
                    StatusText = command.Description;
                }
            }

            ToolbarButton button = sender as ToolbarButton;
            if (null != button)
            {
                StatusText = button.Description;
            }
        }

        private void ClearCommandDescription(object sender)
        {
            StatusText = Properties.Resources.ReadyText;
        }

        void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        bool isInFullScreenMode;
        readonly Dictionary<DependencyObject, UFInterfaces.DockState> mapFullScreenStates = new Dictionary<DependencyObject, UFInterfaces.DockState>();
        Syncfusion.Windows.Tools.RibbonState oldRibbonState;
        private void OnFullScreen(object sender, RoutedEventArgs e)
        {
            if (isInFullScreenMode)
            {
                isInFullScreenMode = false;

                Utilities.WINDOWPLACEMENT wp = (Utilities.WINDOWPLACEMENT)Properties.Settings.Default.WindowPlacement;
                if (wp.showCmd != 0)
                    SaveWindowPlacementState.Load(this, wp);
                else
                    WindowState = WindowState.Normal;

                foreach (var child in mapFullScreenStates.Keys)
                {
                    try
                    {
                        DockingManager.SetState(child, (Syncfusion.Windows.Tools.Controls.DockState)mapFullScreenStates[child]);
                    }
                    catch { }
                }

                Ribbons.RibbonState = oldRibbonState;
            }
            else
            {
                isInFullScreenMode = true;
                Properties.Settings.Default.WindowPlacement = SaveWindowPlacementState.Save(this);
                mapFullScreenStates.Clear();
                dockingManager.Children.ForEach(child =>
                    {
                        try
                        {
                            var state = (UFInterfaces.DockState)DockingManager.GetState(child);
                            if (state == UFInterfaces.DockState.Dock)
                            {
                                mapFullScreenStates.Add(child, state);
                                DockingManager.SetState(child, Syncfusion.Windows.Tools.Controls.DockState.AutoHidden);
                            }
                            else if (state == UFInterfaces.DockState.Float)
                            {
                                mapFullScreenStates.Add(child, state);
                                DockingManager.SetState(child, Syncfusion.Windows.Tools.Controls.DockState.Hidden);
                            }
                        }
                        catch { }
                    });
                Ribbons.RibbonState = Syncfusion.Windows.Tools.RibbonState.Hide;
                WindowState = WindowState.Maximized;
            }

            //if (dockingManager.TDIFullScreenMode == FullScreenMode.WindowMode)
            //    dockingManager.TDIFullScreenMode = FullScreenMode.None;
            //else
            //    dockingManager.TDIFullScreenMode = FullScreenMode.WindowMode;

            //UserControl activeWindow = dockingManager.ActiveWindow as UserControl;
            //Panel panel = activeWindow.Content as Panel;
            //UIElement element = panel.Children[0];
            //panel.Children.Remove(element);
            //var fullScreenWindow = new FullScreenWindow();
            //fullScreenWindow.Title = DockingManager.GetHeader(activeWindow) as String;
            ////var icon = DockingManager.GetIcon(activeWindow);
            ////if (icon != null && icon is ImageBrush)
            ////    fullScreenWindow.Icon = (icon as ImageBrush).ImageSource;

            //fullScreenWindow.Closed += (o, ev) =>
            //{
            //    fullScreenWindow.Close();
            //    fullScreenWindow.ClearChildren();

            //    panel.Children.Insert(0, element);
            //    dockingManager.ActiveWindow = activeWindow;
            //    Show();
            //};

            //if (activeWindow.CommandBindings != null)
            //    fullScreenWindow.CommandBindings.AddRange(activeWindow.CommandBindings);

            //ResourceDictionary dict = new ResourceDictionary
            //{
            //    Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/SkinManager.xaml", UriKind.Relative)
            //};
            //fullScreenWindow.Resources.MergedDictionaries.Add(dict);

            //String currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
            //if (currentStyle != null)
            //    SkinStorage.SetVisualStyle(fullScreenWindow, currentStyle);

            //fullScreenWindow.InsertChildren(element);
            //Hide();
            //fullScreenWindow.ShowDialog();
        }

        void CanExecuteFullScreen(object sender, CanExecuteRoutedEventArgs e)
        {
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                var ret = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxY+APFmQPtOWSUaMJjYrwAg=="/* DBG */);
                if (!ret)
                    System.Environment.Exit(-10);
            }
#endif

            e.CanExecute = dockingManager.ActiveWindow != null && dockingManager.ActiveWindow is UserControl &&
                            (dockingManager.ActiveWindow as UserControl).Content is Panel &&
                            ((dockingManager.ActiveWindow as UserControl).Content as Panel).Children.Count > 0;
        }

        #endregion Commands

        #region Magnifier

        private Magnifier MagnifierToolObj;
        private bool _MagnifierTool = false;

        public bool MagnifierTool
        {
            get
            {
                return _MagnifierTool;
            }
            set
            {
                _MagnifierTool = value;
                if (_MagnifierTool)
                {
                    if (MagnifierToolObj == null)
                        MagnifierToolObj = new Magnifier();
                    MagnifierToolObj.FrameBackground = Brushes.White;
                    MagnifierToolObj.Name = "Magnifier";
                    MagnifierToolObj.FrameHeight = 50;
                    MagnifierToolObj.FrameWidth = 50;
                    MagnifierToolObj.FrameRadius = 50;
                    MagnifierToolObj.ZoomFactor = 0.5;
                    MagnifierToolObj.FrameType = FrameType.Circle;
                    MagnifierToolObj.EnableExport = true;

                    MagnifierToolObj.AssociateWith(dockingManager);
                }
                else
                {
                    MagnifierToolObj.AssociateWith(null);
                }
            }
        }

        #endregion Magnifier


        public class BrushModel
        {
            public Brush Brush { get; set; }

            public string Name { get; set; }
        }

        #region SysTray

        private void OnNotificationAreaIconDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (bShuttingDown)
                return;

            if (e.ChangedButton == MouseButton.Left)
            {
                if (WindowState == WindowState.Minimized)
                    WindowState = WindowState.Normal; 
                Show();
                Activate();
            }
        }

        #endregion SysTray

        private void CheckCertificate_Click(object sender, RoutedEventArgs e)
        {
            Startup.CheckCertificate(false);
        }

        #region Memory
        bool bNotCloseAnyDocuments;
        void CheckMemoryUsage()
        {
            long maxMemory = Properties.Settings.Default.MaxMbMemoryUsage;
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            long totalMbOfMemoryUsed = currentProcess.WorkingSet64 / (1024 * 1024);
            if (maxMemory > 0 && totalMbOfMemoryUsed > maxMemory)
            {
                WPFUtilities.MemoryCompressor.minimizeMemory(false, true);

                currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                totalMbOfMemoryUsed = currentProcess.WorkingSet64 / (1024 * 1024);

                if (!bNotCloseAnyDocuments && 
                    maxMemory > 0 && totalMbOfMemoryUsed > maxMemory &&
                    dockingManager.Children.Count > 1)
                {
                    int i = 0;
                    var toRemove = GetFirstDocumentDocked(ref i);
                    if (toRemove == null)
                        return;

                    if (MessageBox.Show(Properties.Resources.HighMemoryUsage,
                        Properties.Settings.Default.MainWindowTitle, MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        bNotCloseAnyDocuments = true;
                        return;
                    }
                    
                    while (maxMemory > 0 && totalMbOfMemoryUsed > maxMemory &&
                            dockingManager.Children.Count > 1)
                    {
                        var ev = new Syncfusion.Windows.Tools.Controls.CloseButtonEventArgs(toRemove);
                        DockingManager_CloseButtonClick(this, ev);

                        WPFUtilities.MemoryCompressor.minimizeMemory(false, true);

                        currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                        totalMbOfMemoryUsed = currentProcess.WorkingSet64 / (1024 * 1024);

                        toRemove = GetFirstDocumentDocked(ref i);
                        if (toRemove == null)
                            break;
                    }
                }
            }
            else
            {
                bNotCloseAnyDocuments = false;
            }
        }

        #endregion

        private void Ribbon_RibbonContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        private void OnLanguagePreferences(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            string filepath = "LanguagePreferences.exe";
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                filepath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), filepath);
            string args = String.Format("/P{0}", Process.GetCurrentProcess().Id);

            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = filepath;
            process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(filepath);
            process.StartInfo.Arguments = args;

            process.Start();
        }

        #region DockStates

        String lastLoadedDockingState;
        internal void LoadDockStates(String name)
        {
            if (lastLoadedDockingState == name || bShuttingDown)
                return;
            lastLoadedDockingState = name;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                using (new WaitCursor())
                {
                    var storage = GetStorage();
                    try
                    {
                        bLoadingDockingState = true;
                        dockingManager.LoadDockState(storage, name);
                    }
                    catch (Exception ex)
                    {
                        logGeneral.Error(Properties.Resources.FailedToLoadAppState, ex);
                        System.Diagnostics.Trace.TraceError(ex.ToString());
                    }
                    finally
                    {
                        bLoadingDockingState = false;
                    }
                }
            }
        }

        String lastSavedDocked;
        internal void SaveDockStates(String name)
        {
            if (lastSavedDocked == name || bShuttingDown)
                return;
            lastSavedDocked = name;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                using (new WaitCursor())
                {
                    var storage = GetStorage();
                    try
                    {
                        dockingManager.SaveDockState(storage, name);
                    }
                    catch (Exception ex)
                    {
                        logGeneral.Error(Properties.Resources.FailedToLoadAppState, ex);
                        System.Diagnostics.Trace.TraceError(ex.ToString());
                    }
                }
            }
        }
        #endregion

        #region EasyMode

        const String sEasyModeDockState = "EasyModeState";

        public bool IsInEasyMode
        {
            get
            {
                rbEasyMode.IsSelected = Properties.Settings.Default.IsInEasyMode;
                return Properties.Settings.Default.IsInEasyMode;
            }
        }

        private void OnEasyMode(object sender, ExecutedRoutedEventArgs e)
        {
            Properties.Settings.Default.IsInEasyMode = !Properties.Settings.Default.IsInEasyMode;
            rbEasyMode.IsSelected = Properties.Settings.Default.IsInEasyMode;
            workSpaceComponent.OnEasyModeChanged(this);
        }

        #endregion
    }
}