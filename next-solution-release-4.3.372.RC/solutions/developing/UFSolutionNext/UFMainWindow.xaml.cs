using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Bars.Themes;
using DocumentManager.ComponentService;
using log4net;
using OPCUAViewModelService.ComponentService;
using Tracing.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UFInterfaces.StartupWelcome;
using UFSolution;
using UriResolver.ComponentService;
using Utilities;
using Utilities.WPF;
using WPFUtilities;
using System.Xml.Linq;
using DocumentManager.ComponentService.Helpers;
using DevExpress.Mvvm.Native;
using UFProjectManager.ComponentService;

namespace UFSolutionNext
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UFMainWindow : ThemedWindow
    {
        #region Public Properties
        public bool IsBusy
        {
            get
            {
                return busyComponent.IsBusy;
            }
            set
            {
                busyComponent.IsBusy = value;
            }
        }

        public string BusyContent
        {
            get
            {
                return busyComponent.BusyContent;
            }
            set
            {
                busyComponent.BusyContent = value;
            }
        }

        public ObservableCollection<BarButtonItem> DockedElements
        {
            get;
        } = new ObservableCollection<BarButtonItem>();

        #endregion

        #region Declarations

        readonly ComponentHost componentHost;
        readonly WorkSpaceComponent workSpaceComponent;
        readonly BusyComponent busyComponent;
        ISimpleLogging simpleLogging;
        IStartupWelcome startupWelcome;
        List<FrameworkElement> eliteDockedToBeSaved;
        List<FrameworkElement> listActivated = new List<FrameworkElement>();

        static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);

        readonly DispatcherTimer memoryCheckTimer;

        Dictionary<String, DockType> dockTypesMap = new Dictionary<string, DockType>();
        Dictionary<String, Size> dockSizeMap = new Dictionary<string, Size>();
        Dictionary<Type, TabbedGroup> tbGroups = new Dictionary<Type, TabbedGroup>();
        List<String> closingPanels = new List<string>();

        bool bLoaded;
        internal bool bLoading;
        bool bLoadingDockingState;
        bool bLoadingMenuState;
        bool bLoadingLayoutInfo;
        bool bActiveWindowChanging;
        DelayedSingleActionInvoker activateInvoker;
        FrameworkElement elnew;
        FrameworkElement elold;
        bool isInFullScreenMode;
        bool bItemClosing;
        bool bWindowClosing;
        bool bShuttingDown;
        readonly List<IToolbar> defaultToolbars = new List<IToolbar>();

        internal bool bMenusLoaded;
        List<IDocumentManager> mergingDocumentManagers;
        List<IDocumentManager> postponedMerges = new List<IDocumentManager>();
        Dictionary<int, BarItem> toolbarItems = new Dictionary<int, BarItem>();

#if CONNEXT
        static readonly String stateNoAutoLoadWorkspace = "$Connext.stateNoAutoLoadWorkspace";
        static readonly String stateEasyModeWorkspace = "$Connext.stateEasyModeWorkspace";
#else
        static readonly String stateNoAutoLoadWorkspace = "$stateNoAutoLoadWorkspace";
        static readonly String stateEasyModeWorkspace = "$stateEasyModeWorkspace";
#endif
        static string SerializableSelectedTabPageIndexPropertyName = nameof(LayoutGroup.SerializableSelectedTabPageIndex);

        String GeneralWorkspaceStateFileName 
        {
            get
            {
                return IsInEasyMode ? stateEasyModeWorkspace : stateNoAutoLoadWorkspace;
            }
        }

        //bool bFirstActivated;
        #endregion

        #region DependencyProperties

        static UFMainWindow()
        {
            StatusTextProperty = DependencyProperty.Register("StatusText", typeof(string), typeof(UFMainWindow),
                        new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender));
            BarManager.CheckBarItemNames = false;
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
                simpleLogging?.AddItem(Properties.Resources.statusTextTrace_Title, value);
                SetValue(StatusTextProperty, value);
            }
        }

        #endregion DependencyProperties

        public static string ApplicationID
        {
            get { return string.Format("{0}_{1}", Utilities.AssemblyInfo.Product, Utilities.AssemblyInfo.FileFormatVersion); }
        }

        #region Ctor
        public UFMainWindow()
        {
            InitializeComponent();

#if CONNEXT
            Icon = notifyIcon.Icon = Global.AppIcon;
#endif
            PreCheckCommandArguments();

            busyComponent = new BusyComponent();
            DataContext = busyComponent.Instance;

            //for (int i = 0; i < 100; i++)
            //{
            //    DXSplashScreen.Progress(i);
            //    DXSplashScreen.SetState(string.Format("{0} %", (i + 1)));
            //    Thread.Sleep(40);
            //}

            try
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.FontFamily))
                {
                    FontStyleHelper.hasCustomFontFamily = true;
                    FontStyleHelper.customFontFamily = FontFamily = new FontFamily(Properties.Settings.Default.FontFamily);
                }
                if (!String.IsNullOrEmpty(Properties.Settings.Default.FontSize))
                {
                    FontStyleHelper.hasCustomFontSize = true;
                    FontStyleHelper.customFontSize = FontSize = Convert.ToDouble(Properties.Settings.Default.FontSize);
                }
            }
            catch
            {
            }

            if (FontStyleHelper.hasCustomFontFamily || FontStyleHelper.hasCustomFontSize)
            {
                var style = new Style(typeof(ContentControl));
                if (FontStyleHelper.hasCustomFontFamily)
                    style.Setters.Add(new Setter(FontFamilyProperty, FontFamily));
                if (FontStyleHelper.hasCustomFontSize)
                    style.Setters.Add(new Setter(FontSizeProperty, FontSize));
                MainWnd.Resources.Add(new BarControlThemeKeyExtension() { IsThemeIndependent = true, ResourceKey = BarControlThemeKeys.MenuContentStyle }, style);
                MainWnd.Resources.Add(new BarControlThemeKeyExtension() { IsThemeIndependent = true, ResourceKey = BarControlThemeKeys.MainMenuContentStyle }, style);
                MainWnd.Resources.Add(new BarControlThemeKeyExtension() { IsThemeIndependent = true, ResourceKey = BarControlThemeKeys.BarContentStyle }, style);
                MainWnd.Resources.Add(new BarControlThemeKeyExtension() { IsThemeIndependent = true, ResourceKey = BarControlThemeKeys.ContentExpanderStyle }, style);
            }

            var fontFamily = FontFamily.Source;
            if (FontStyleHelper.hasCustomFontFamily)
                fontFamily = FontStyleHelper.customFontFamily.Source;
            var fontSize = FontSize;
            if (FontStyleHelper.hasCustomFontSize)
                fontSize = FontStyleHelper.customFontSize;
            try
            {
                AutoUpdaterDotNET.AutoUpdater.DefaultFont = new System.Drawing.Font(fontFamily, (float)fontSize, System.Drawing.GraphicsUnit.Pixel);
            }
            catch
            { }

            componentHost = new ComponentHost();
            workSpaceComponent = new WorkSpaceComponent(this);
            componentHost.Components.Add(workSpaceComponent);
            componentHost.Components.Add(busyComponent);

#if !DEBUG
                Numerical.processed += (ob, ev) =>
                {
                    if (ob == null)
                    {
                        Dispatcher.BeginInvokeAsynchronously(() =>
                        {
                            System.Environment.Exit(-10);
                        });
                    }
                };
                Numerical.VerifyNumerical();
#endif

            memoryCheckTimer = new DispatcherTimer(DispatcherPriority.Send);
            memoryCheckTimer.Interval = TimeSpan.FromSeconds(3);
            memoryCheckTimer.Tick += (o, e) =>
            {
                CheckMemoryUsage();

#if !DEBUG
                Numerical.VerifyNumerical();
#endif
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
        #endregion

        #region Window Events
        public bool HasContentRendered()
        {
            return bContentRenderedExecuted;
        }

        bool bContentRenderedExecuted;

#if !DEBUG
        DispatcherTimer timer;
#endif

        private void MainWnd_ContentRendered(object sender, EventArgs e)
        {
            if (bContentRenderedExecuted)
                return;

            bContentRenderedExecuted = true;

            try
            {
                DevExpress.Xpf.Core.DXSplashScreen.Close();
            }
            catch { };


            if (bShuttingDown)
                return;

            //vm.TaskbarProgressValue = 50;
            //vm.TaskbarProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
            //ShowPredefinedNotification("Welcome to Movicon.NExT");

            #region Loading RibbonState

            //if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            //{
            //    using (new WaitCursor())
            //    {
            //        try
            //        {
            //            Ribbons.PersistElements.Add(RibbonElements.QuickAccessToolbar);
            //            Ribbons.PersistElements.Add(RibbonElements.Ribbon);
            //            Ribbons.LoadRibbonState();
            //        }
            //        catch (Exception ex)
            //        {
            //            logGeneral.Error(Properties.Resources.FailedToLoadAppState, ex);
            //            System.Diagnostics.Trace.TraceError(ex.ToString());
            //        }
            //    }
            //}

            #endregion

            //if (tmr == null)
            //{
            //    tmr = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.ApplicationIdle, (ob, ev) =>
            //    {
            //        try
            //        {
            //            if (!bActiveWindowChangedInExecution)
            //            {
            //                var el = FocusManager.GetFocusedElement(this) as FrameworkElement;
            //                if (el != null && dockingManager.IsControlDocked(el) && dockingManager.GetActiveWindow() != el)
            //                    dockingManager.SetActiveWindow(el);
            //                // System.Diagnostics.Debug.Write(String.Format("Focus element = {0}, Focus Scope = {1}", el, active));
            //            }
            //        }
            //        catch (Exception ex)
            //        {

            //        }
            //    }, Dispatcher.CurrentDispatcher);
            //    tmr.Start();
            //}

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
                                if(MSZ.MSZView.IsForciblySuspended())
                                    logLicense.Warn(Properties.Resources.LicenseDuplicated);
                                else
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

            UFSolutionNext.ArrayIndex.processed += (obo, eve) =>
            {
                if (obo == null)
                {
                    System.Environment.Exit(-10);
                }
            };
#if !CONNEXT
            UFSolutionNext.ArrayIndex.VerifyArrayIndex(0x1);
#else
            UFSolutionNext.ArrayIndex.VerifyArrayIndex(0x4);
#endif
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
                    ShowPredefinedNotification(String.Format(Properties.Resources.NoHardwareAccelerationFound,
                        System.Environment.Is64BitProcess ? "64" : "32",
                        SysInfo.GetIndexPerformance(), SysInfo.GetIndexGraphicsPerformance(), SysInfo.GetNumberOfLogicalProcessors()));
                    //System.Windows.Forms.ToolTipIcon.Error
                    //no hardware acceleration
                }
                else if (displayTier == 1)
                {
                    ShowPredefinedNotification(String.Format(Properties.Resources.PartialHardwareAccelerationFound,
                        System.Environment.Is64BitProcess ? "64" : "32",
                        SysInfo.GetIndexPerformance(), SysInfo.GetIndexGraphicsPerformance(), SysInfo.GetNumberOfLogicalProcessors()));
                    //System.Windows.Forms.ToolTipIcon.Warning;
                    //partial hardware acceleration
                }
                else
                {
                    ShowPredefinedNotification(String.Format(Properties.Resources.HardwareAccelerationFound,
                        System.Environment.Is64BitProcess ? "64" : "32",
                        SysInfo.GetIndexPerformance(), SysInfo.GetIndexGraphicsPerformance(), SysInfo.GetNumberOfLogicalProcessors()));
                    //System.Windows.Forms.ToolTipIcon.Info;
                    //supports hardware acceleration
                }
            });

            //#region Expiring

            //if (DateTime.UtcNow > new DateTime(2011, 12, 1))
            //{
            //    Action action = () => Application.Current.Shutdown();
            //    Dispatcher.BeginInvoke(action, DispatcherPriority.Normal);
            //}

            //#endregion

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Normal;

            //Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() =>
            //{
            //    Ribbons.SelectedIndex = 0;
            //}));

            Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            {
                AutoUpdaterDotNET.AutoUpdater.Start(Properties.Settings.Default.UpdateServiceURL);
            });
        }

        private void OnCheckForUpdates(object sender, ExecutedRoutedEventArgs e)
        {
            ChecForUpdates();
        }

        private void ChecForUpdates()
        {
            if (bShuttingDown)
                return;
            bool ret = false;
            IsBusy = true;
            try
            {
                ret = AutoUpdaterDotNET.AutoUpdater.StartSync(Properties.Settings.Default.UpdateServiceURL);
            }
            finally
            {
                IsBusy = false;
                if (!ret)
                    workSpaceComponent.UIInterface.ShowInformation(Properties.Resources.NoUpdatesAvailable);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            bWindowClosing = true;
            var windowArray = new Window[Application.Current.Windows.Count];
            Application.Current.Windows.CopyTo(windowArray, 0);

            foreach (Window window in windowArray)
            {
                if (window == this || window is DevExpress.Xpf.Docking.Platform.FloatingPaneWindow)
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
                        bWindowClosing = false;
                        logGeneral.Error(Properties.Resources.CanceledClosingByAnotherWindow);
                        return;
                    }
                }
            }

            var windowArrayOwner = new Window[OwnedWindows.Count];
            OwnedWindows.CopyTo(windowArrayOwner, 0);

            foreach (Window window in windowArrayOwner)
            {
                if (window == this || window is DevExpress.Xpf.Docking.Platform.FloatingPaneWindow)
                    continue;
                window.Close();

                if (!window.IsVisible)
                    continue;
                foreach (var wnd in OwnedWindows)
                {
                    if (wnd == window)
                    {
                        e.Cancel = true;
                        bWindowClosing = false;
                        logGeneral.Error(Properties.Resources.CanceledClosingByOwnedWindow);
                        return;
                    }
                }
            }

            var lastIsEnable = IsEnabled;
            IsEnabled = false;
            workSpaceComponent.OnClosing(sender, e);
            IsEnabled = lastIsEnable;

            if (e.Cancel == false)
            {
                using (new WaitCursor())
                {

                    if (isInFullScreenMode)
                        OnFullScreen(this, null);

                    dockingManager.BeginUpdate();
                    if (eliteDockedToBeSaved != null)
                    {
                        (from c in dockingManager.GetAllDockedControls()
                         where !eliteDockedToBeSaved.Contains(c)
                         select c).ToList().ForEach(c =>
                         {
                             RemoveDockedControl(c);
                         });
                    }

                    if (!Properties.Settings.Default.AutoLoadWorkspace)
                    {
                        SaveDockStates(GeneralWorkspaceStateFileName);
                        if (bMenusLoaded)
                            SaveMenuStates(GeneralWorkspaceStateFileName);
                    }
                    else if (ApplicationPropertiesHelper.GetProperty("UseLayoutToFile") as bool? != true)
                    {
                        string elName = elold?.GetType().Name;
                        var element = (ChildDocumentGroup.SelectedItem as LayoutPanel)?.Content as FrameworkElement;
                        if (element != null)
                            elName = element.GetType().Name;

                        if (elName != null)
                        {
                            SaveDockStates(elName);
                            if (bMenusLoaded)
                                SaveMenuStates(elName);
                        }
                    }

                    bShuttingDown = true;

                    workSpaceComponent.Shutdown();

                    dockingManager.DockItemActivated -= OnDockItemActivated;
                    dockingManager.DockItemActivating -= OnDockItemActivating;
                    dockingManager.DockItemEndDocking -= OnDockItemEndDocking;
                    dockingManager.DockOperationCompleted -= OnDockOperationCompleted;
                    dockingManager.DockItemClosing -= OnDockItemClosing;
                    dockingManager.DockItemClosed -= OnDockItemClosed;

                    DXSerializer.RemoveEndDeserializingHandler(dockingManager, OnDeserializingEndHandler);
                    DXSerializer.RemoveAllowPropertyHandler(dockingManager, OnAllowPropertyHandler);
                    DXSerializer.RemoveShouldSerializeCollectionItemHandler(dockingManager, OnShouldSerializeCollectionItem);
                
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        using (new WaitCursor())
                        {
                            Properties.Settings.Default.WindowPlacement = SaveWindowPlacementState.Save(this).ToXml();

                            try
                            {
                                lastSavedDocked = null;
                                Properties.Settings.Default.Save();
                                SaveApplicationProperties();
                            }
                            catch (Exception ex)
                            {
                                logGeneral.Error(Properties.Resources.FailedToSaveAppState, ex);
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
                    dockingManager.EndUpdate();
                }
            }
            else
            {
                logGeneral.Error(Properties.Resources.CanceledClosingByComppnent);
                bWindowClosing = false;
            }
        }

        void MergeDocumentManager(IDocumentManager im, Dictionary<int, BarItem> toolbarItems)
        {
            // using (new WaitCursor())
            {
                var tb = im.GetToolbar();
                if (tb != null)
                {
                    if (tb.IsVisible())
                        defaultToolbars.Add(tb);
                    workSpaceComponent.AddBarManagerItem(tb as BarManager, tb.BarCommandBindings, im.TypeTitle);
                    var baritem = tb.ToolbarMenuItem as BarItem;
                    if (baritem != null)
                        toolbarItems.Add(baritem.MergeOrder, baritem);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bLoaded)
                return;
            bLoaded = true;

            (rbEasyMode as BarCheckItem).IsChecked = Properties.Settings.Default.IsInEasyMode;

            using (new WaitCursor())
            {
                System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;

                ResourceDictionaryExtensions.AddCommonResources(this);

                //bLoadingDockingState = true;

                LoadApplicationProperties();

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
                        bLoading = true;

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
                        startupWelcome = componentHost.GetService(typeof(IStartupWelcome)) as IStartupWelcome;
                        // we need to register OPC UA view Model to let it get other services
                        var opcuaservice = componentHost.GetService(typeof(IOPCUAViewModelService)) as IOPCUAViewModelService;

                        #endregion PluginDiscovering

                        workSpaceComponent.OnWorkspaceLoading(this);

                        if (!bShuttingDown)
                        {
                            #region Loading DockingManager State

                            DXSerializer.AddEndDeserializingHandler(dockingManager, OnDeserializingEndHandler);
                            DXSerializer.AddAllowPropertyHandler(dockingManager, OnAllowPropertyHandler);
                            DXSerializer.AddShouldSerializeCollectionItemHandler(dockingManager, OnShouldSerializeCollectionItem);

                            InitLayoutIcons();

                            eliteDockedToBeSaved = dockingManager.GetAllDockedControls().ToList();

                            #endregion

                            #region Menus and Toolbars
                            
                            Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() => { LoadComponentMenu(); });
                            #endregion

                            var mapAlwaysAvailabeCommands = new Dictionary<IDocumentManager, IList<ICommand>>();
                            workSpaceComponent.UriRisolver.GetListInstalledDocumentManagers().ForEach(im =>
                            {
                                im.GetToolbar();
                                var cmds = im.GetAlwaysAvailableCommand();
                                if (cmds != null)
                                    mapAlwaysAvailabeCommands.Add(im, cmds);
                            });

                            if (mapAlwaysAvailabeCommands.Count > 0)
                            {
                                CommandManager.AddPreviewCanExecuteHandler(this, (o, ev) =>
                                {
                                    if (workSpaceComponent.ContextDocument != null)
                                    {
                                        foreach (var manager in mapAlwaysAvailabeCommands.Keys)
                                        {
                                            if (workSpaceComponent.ContextDocument.GetType() != manager.DocumentType)
                                            {
                                                foreach (var cmd in mapAlwaysAvailabeCommands[manager])
                                                {
                                                    var rcmd = cmd as RoutedCommand;
                                                    var checkrcmd = ev.Command as RoutedCommand;
                                                    if (rcmd != null && checkrcmd != null && rcmd.Name == checkrcmd.Name)
                                                    {
                                                        ev.CanExecute = true;
                                                        ev.Handled = true;
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                });

                                bool bExecuting = false;
                                CommandManager.AddPreviewExecutedHandler(this, (o, ev) =>
                                {
                                    foreach (var manager in mapAlwaysAvailabeCommands.Keys)
                                    {
                                        if (workSpaceComponent.ContextDocument != null &&
                                            workSpaceComponent.ContextDocument.GetType() != manager.DocumentType)
                                        {
                                            foreach (var cmd in mapAlwaysAvailabeCommands[manager])
                                            {
                                                var rcmd = cmd as RoutedCommand;
                                                var checkrcmd = ev.Command as RoutedCommand;
                                                if (rcmd != null && checkrcmd != null && rcmd.Name == checkrcmd.Name)
                                                {
                                                    if (workSpaceComponent.ContextDocument != null)
                                                    {
                                                        if (bExecuting)
                                                            return;
                                                        bExecuting = true;
                                                        try
                                                        {
                                                            var parent = DocumentHelper.GetRootParent(workSpaceComponent.ContextDocument, traverse: false);
                                                            var uri = new Uri(parent.rootBase, UriKind.RelativeOrAbsolute);
                                                            workSpaceComponent.IsBusy = true;
                                                            try
                                                            {
                                                                manager.Edit(uri, parent);
                                                                WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, this);

                                                                ev.Handled = true;
                                                                rcmd.Execute(ev.Parameter, workSpaceComponent.ActiveWindow);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                            // workSpaceComponent.UIInterface.ShowError(String.Format(Properties.Resources.ErrorOpeningDocument, uri, ex.Message));
                                                        }

                                                        //if (!listDocumentManagers.Contains(docManager))
                                                        //    listDocumentManagers.Add(docManager);
                                                    }
                                                        finally
                                                        {
                                                            workSpaceComponent.IsBusy = false;
                                                            bExecuting = false;
                                                        }
                                                    }
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                });
                            }
                        }

                        CheckCommandArguments();

                        #region Loading Window Position

                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            if (!String.IsNullOrEmpty(Properties.Settings.Default.WindowPlacement))
                            {
                                WINDOWPLACEMENT wp = Properties.Settings.Default.WindowPlacement.FromXml<WINDOWPLACEMENT>();
                                if (wp.showCmd != 0 && Visibility != Visibility.Hidden)
                                    SaveWindowPlacementState.Load(this, wp);
                            }
                        }
                        else
                        {
                            Properties.Settings.Default.Reset();
                        }

                        #endregion

                        ApplicationPropertiesHelper.SetProperty("AutoLoadWorkspace", Properties.Settings.Default.AutoLoadWorkspace);
                        if (!Properties.Settings.Default.AutoLoadWorkspace)
                        {
                            LoadDockStates(GeneralWorkspaceStateFileName);
                            //LoadMenuStates(GeneralWorkspaceStateFileName);
                        }

                        dockingManager.BeginUpdate();
                        workSpaceComponent.OnWorkspaceLoaded(this);
                        dockingManager.EndUpdate();
                    }
                    finally
                    {
                        Mutex.ReleaseMutex();
                        bLoading = false;
                    }
                }
                //bLoadingDockingState = false;
            }
        }

        void LoadComponentMenu(bool bSynchro = false)
        {
            if (bShuttingDown || bMenusLoaded)
                return;

            menuProgressBar.Visibility = Visibility.Visible;

            if (mergingDocumentManagers == null)
            {
                using (new WaitCursor())
                {
                    mergingDocumentManagers = (from c in workSpaceComponent.UriRisolver.GetListInstalledDocumentManagers() orderby c.TypeScheme select c).ToList();
                }
            }

            if (mergingDocumentManagers.Count == 0)
            {
                bMenusLoaded = true;
                menuProgressBar.Visibility = Visibility.Collapsed;

                foreach (var pm in postponedMerges)
                    MergeDocumentManager(pm, toolbarItems);
                postponedMerges.Clear();                     

                foreach (var item in toolbarItems.OrderBy(item => item.Key).Select(item => item.Value))
                    toolbarsContextMenu.ItemLinks.Add(item);

                if (!Properties.Settings.Default.AutoLoadWorkspace)
                    LoadMenuStates(GeneralWorkspaceStateFileName);

                mainBarManager.Bars.ForEach(bar => {
                    bar.IsCollapsed = false;
                    bar.ShowDragWidget = true;
                });

                postponedMerges.Clear();
                toolbarItems.Clear();
                return;
            }

            var im = mergingDocumentManagers.First();
            mergingDocumentManagers.RemoveAt(0);

            if (Properties.Settings.Default.PostponedDMMerges.Split('|').Contains(im.TypeScheme))
                postponedMerges.Add(im);
            else
                MergeDocumentManager(im, toolbarItems);

            if (!bSynchro)
                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() => { LoadComponentMenu(); });
            else
                LoadComponentMenu(true);
        }

        private void MainWnd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dockingManager.ActiveDockItem == null || LayoutHelper.IsChildElementLogical(dockingManager.ActiveDockItem, e.OriginalSource as DependencyObject))
                return;

            var activeWindow = dockingManager.GetActiveWindow();
            var activeView = workSpaceComponent.ContextDocument?.ActiveView as FrameworkElement;
            if (activeView == null)
            {
                var views = (from c in dockingManager.GetAllDockedControls()
                             where DockingHelper.GetState(c) == DockState.Document && 
                             !tabbedPanels.Contains(DockLayoutManager.GetLayoutItem(c))
                             select c).ToList();
                if (views.Count > 0)
                    activeView = views[0];
            }
            if (activeView != null && activeView != activeWindow && !LayoutHelper.IsChildElementLogical(activeWindow, activeView))
            {
                var cea = new ItemCancelEventArgs(dockingManager.ActiveDockItem);
                workSpaceComponent.OnActiveWindowChanging(activeView, cea);
                if (cea.Cancel)
                    e.Handled = true;
                else if (!LayoutHelper.IsChildElementLogical(dockingManager, e.OriginalSource as DependencyObject))
                    workSpaceComponent.FlashDockedElement(activeView);
                else if (e.OriginalSource is IInputElement)
                    (e.OriginalSource as IInputElement).Focus();
            }
        }

        #endregion

        #region Private Methods

        void OnDeserializingEndHandler(object sender, EndDeserializingEventArgs e)
        {
            if (bLoadingMenuState)
                return;

            //TabbedPanels serialized in AutoHidden mode not opening correctly without resetting AutoHidden property after deserialization (case 22051)
            foreach (var panel in tabbedPanels)
            {
                if ((panel as LayoutPanel).AutoHidden)
                {
                    (panel as LayoutPanel).AutoHidden = false;
                    (panel as LayoutPanel).AutoHidden = true;
                }
            }
            dockingManager.BeginUpdate();
            //Panels that do not exist in the restored layout file are placed into the DockLayoutManager.ClosedPanels collection (https://www.devexpress.com/Support/Center/Question/Details/Q574637/new-element-disappears-when-restoring-old-docking-layout); we must restore them (es. currently opened documentpanels)
            var panels = dockingManager.ClosedPanels.ToList();
            foreach (var panel in panels)
            {
                if (panel.Content as FrameworkElement == null || panel.Content.GetType().Name == "ContentControl")
                    continue;

                var dockType = DockType.Fill;
		        var bToBeHidden = false;
                BaseLayoutItem destinationItem = ChildDocumentGroup;
                if (tabbedPanels.Contains(panel))
                {
                    var tabbedPanel = dockingManager.GetControlParentPanel((panel.Content as FrameworkElement).GetType(), panel);
                    if (tabbedPanel != null)
                        destinationItem = tabbedPanel;
                    else {
			            bToBeHidden = true;
                        dockType = DockType.Left;
		            }
                }
                dockingManager.DockController.Dock(panel, destinationItem, dockType);
		        if (bToBeHidden)
                    dockingManager.DockController.Hide(panel);
            }
            //DocumentPanels present in the restored layout file would be restored; we must remove them
            foreach (var docPanel in ChildDocumentGroup.GetItems())
            {
                var pnl = docPanel as LayoutPanel;
                if (pnl == null)
                    continue;
                if (pnl.Content == null)
                    dockingManager.RemoveDockedControl(docPanel);
                //if (pnl.Content.GetType().Name == "ContentControl")
                //{
                //    //dockingManager.DockController.Dock(docPanel, rootLayoutGroup, DockType.Bottom);
                //    //pnl.AutoHidden = true;
                //    dockingManager.DockInAutoHiddenGroup("CustomBottomAutoHiddenGroup", pnl, DockSide.Bottom);
                //}
            }
            foreach (var tbGroup in dockingManager.GetChildrenOfType<TabbedGroup>())
            {
                if (tbGroup == ChildDocumentGroup)
                    continue;
                tabbedPanels.AddRange((from BaseLayoutItem item in tbGroup.Items where !tabbedPanels.Contains(item) select item).ToList());
            }
            ChildDocumentGroup.Parent.ItemWidth = new GridLength(1, GridUnitType.Star);
            dockingManager.EndUpdate();
        }

        void OnShouldSerializeCollectionItem(object sender, XtraShouldSerailizeCollectionItemEventArgs e)
        {
            if (bLoadingMenuState)
                return;

            BaseLayoutItem panel = e.Value as BaseLayoutItem;
            if (panel != null && panel.GetType() == typeof(DocumentPanel) && panel != ChildDocumentGroup)
            {
                e.ShouldSerailize = false;
            }
        }

        void OnAllowPropertyHandler(object sender, AllowPropertyEventArgs e)
        {
            if (bLoadingMenuState)
                return;

            if (e.DependencyProperty == BaseLayoutItem.CaptionProperty || e.Property.Name == SerializableSelectedTabPageIndexPropertyName)
                e.Allow = false;
        }

        List<ICommand> GetListCommand(ILinksHolder subitem, String Text)
        {
            var list = new List<ICommand>();
            foreach (var item in subitem.Items)
            {
                if (item is ILinksHolder)
                    list.AddRange(GetListCommand(item as ILinksHolder, Text));
                else if (item is BarItem)
                {
                    var baritem = item as BarItem;
                    if (baritem.Command is RoutedUICommand && Regex.IsMatch((baritem.Command as RoutedUICommand).Name, Regex.Escape(Text), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace) &&
                       baritem.Command.CanExecute(null))
                        list.Add(baritem.Command);
                }
            }

            foreach (var item in subitem.MergedChildren.MergedLinks)
            {
                if (item.Item is ILinksHolder)
                    list.AddRange(GetListCommand(item.Item as ILinksHolder, Text));
                else if (item.Item is BarItem)
                {
                    var baritem = item.Item as BarItem;
                    if (baritem.Command is RoutedUICommand && Regex.IsMatch((baritem.Command as RoutedUICommand).Name, Regex.Escape(Text), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace) &&
                       baritem.Command.CanExecute(null))
                        list.Add(baritem.Command);
                }
            }

            return list;
        }

        void Command_OnQuerySubmitted(object sender, AutoSuggestEditQuerySubmittedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Text))
            {
                searchCommand.ItemsSource = null;
                return;
            }

            var activeWindow = dockingManager.GetActiveWindow();

            var baritemCmds = (from x in mainBarManager.GetChildrenOfType<BarButtonItem>()
                                where x.Command is RoutedUICommand &&
                                Regex.IsMatch((x.Command as RoutedUICommand).Name, Regex.Escape(e.Text), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace) &&
                                (x.Command as RoutedUICommand).CanExecute(null, activeWindow)
                                select new Tuple<RoutedUICommand, object>(x.Command as RoutedUICommand, x.CommandParameter)).ToList();

            var listNoDuplicates = baritemCmds.GroupBy(p => p.Item1.Name).Select(g => g.First()).OrderBy(g => g.Item1.Name).ToList();
            searchCommand.ItemsSource = listNoDuplicates;
        }

        void SearchCommand_SuggestionChosen(object sender, AutoSuggestEditSuggestionChosenEventArgs e)
        {
            var cmd = e.SelectedItem as Tuple<RoutedUICommand, object>;
            SearchCommand_Execute(cmd);
        }

        private void SearchCommand_KeyDown(object sender, KeyEventArgs e)
        {
            var ctrl = sender as AutoSuggestEdit;
            var lastCmd = ctrl.Tag as Tuple<RoutedUICommand, object>;
            if (lastCmd != null && e.Key == Key.Enter && ctrl.EditValue.ToString() == lastCmd.Item1.Name)
                SearchCommand_Execute(lastCmd);
        }

        void SearchCommand_Execute(Tuple<RoutedUICommand, object> command)
        {
            var activeWindow = dockingManager.GetActiveWindow();
            if (command == null || command.Item1 == null /*|| activeWindow == null*/)
                return;

            searchCommand.Tag = command;
            command.Item1.Execute(command.Item2, activeWindow);
        }

        void SetSkin(String style)
        {
            using (new WaitCursor())
            {
                //ApplicationPropertiesHelper.SetProperty("CurrentSkinBackColor", Application.Current.MainWindow.Background);

                ApplicationPropertiesHelper.SetProperty("CurrentSkin", style);
                ApplicationPropertiesHelper.SetProperty("MainWindowTitle", Properties.Settings.Default.MainWindowTitle);

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
                ApplicationPropertiesHelper.SetProperty("CurrentSkinForeColor", Application.Current.MainWindow.Foreground);

                //this.SetRibbonButtonStyle(Ribbons);
            }
        }

        private void InitLayoutIcons()
        {
            //gridLogViewLayoutPanel.CaptionImage = DockingHelper.GetBitmapImageSource("WStrace");
        }

        void SetDockedItemSize(BaseLayoutItem item, DockOperation mode)
        {
            if (bLoadingLayoutInfo)
                return;

            var elementName = ((item as ContentItem).Content as FrameworkElement).Name;
            if (String.IsNullOrEmpty(elementName))
                return;

            var newWidth = dockSizeMap.ContainsKey(elementName) && dockSizeMap[elementName].Width > 0 ? dockSizeMap[elementName].Width : item.LayoutSize.Width;
            var newHeight = dockSizeMap.ContainsKey(elementName) && dockSizeMap[elementName].Height > 0 ? dockSizeMap[elementName].Height : item.LayoutSize.Height;
            if (mode == DockOperation.Float)
                item.FloatSize = new Size(newWidth, newHeight);
            else if (mode == DockOperation.Dock && newWidth > 0)
            {
                var panel = item as LayoutPanel;
                if (panel != null)
                {
                    panel.ItemWidth = new GridLength(newWidth);
                    if (panel.Parent as TabbedGroup != null && panel.Parent != ChildDocumentGroup)
                        (panel.Parent as TabbedGroup).ItemWidth = new GridLength(newWidth);
                }
            }
        }

        private DelayedSingleActionInvoker delayInvoker;

        void FlashDocking(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is FrameworkElement)
                workSpaceComponent.FlashDockedElement(e.Parameter as FrameworkElement);
        }

        #endregion

        #region Public Methods
        public void ResetBusy()
        {
            try
            {
                DevExpress.Xpf.Core.DXSplashScreen.Close();
            }
            catch { };

            busyComponent.ResetBusy();
        }

        public void RestoreBusy()
        {
            busyComponent.RestoreBusy();
        }

        double nCounterProgress { get; set; }
        double nMaxCounterProgress { get; set; }
        public void UpdateProgressState(int value, int maxvalue, string description, TaskbarItemProgressState state)
        {
            nMaxCounterProgress = (double)maxvalue;
            nCounterProgress = (double)value;
            busyComponent.Instance.TaskbarDescription = $"{description}";
            busyComponent.Instance.TaskbarProgressState = GetState(state);
            busyComponent.Instance.TaskbarProgressValue = Math.Abs(nMaxCounterProgress != 0 ? nCounterProgress / nMaxCounterProgress : 1);
            workSpaceComponent.OnProgressStateChanged(this, new ProgressStateChangedEventArgs()
            {
                Value = busyComponent.Instance.TaskbarProgressValue,
                MaxValue = nMaxCounterProgress,
                Description = busyComponent.Instance.TaskbarDescription,
                State = state
            });
        }
        public void ResetProgressState()
        {
            busyComponent.Instance.TaskbarDescription = string.Empty;
            busyComponent.Instance.TaskbarProgressState = GetState(TaskbarItemProgressState.None);
            busyComponent.Instance.TaskbarProgressValue = 0d;
            nCounterProgress = 0;
            workSpaceComponent.OnProgressStateChanged(this, new ProgressStateChangedEventArgs()
            {
                Value = 0d,
                MaxValue = 0d,
                Description = busyComponent.Instance.TaskbarDescription,
                State = TaskbarItemProgressState.None
            });
        }
        public void IncrementProgressState(int step = 1)
        {
            nCounterProgress += step;
            busyComponent.Instance.TaskbarProgressState = GetState(TaskbarItemProgressState.Normal);
            busyComponent.Instance.TaskbarProgressValue = Math.Abs(nMaxCounterProgress != 0 ? nCounterProgress / nMaxCounterProgress : 1);
            workSpaceComponent.OnProgressStateChanged(this, new ProgressStateChangedEventArgs()
            {
                Value = busyComponent.Instance.TaskbarProgressValue,
                Description = busyComponent.Instance.TaskbarDescription,
                State = TaskbarItemProgressState.Normal
            });
        }

        private System.Windows.Shell.TaskbarItemProgressState GetState(TaskbarItemProgressState state)
        {
            System.Windows.Shell.TaskbarItemProgressState _state = System.Windows.Shell.TaskbarItemProgressState.None;
            Enum.TryParse<System.Windows.Shell.TaskbarItemProgressState>(state.ToString(), out _state);
            return _state;
        }

        #region Command arguments
        void PreCheckCommandArguments()
        {
            Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(Environment.GetCommandLineArgs());
            bool bRegister = commandArgs.ArgPairs.ContainsKey("registertypes");
            bool bUnregister = commandArgs.ArgPairs.ContainsKey("unregistertypes");
            bool bExportSvg = commandArgs.ArgPairs.ContainsKey("exportsvg");
            if (bRegister || bUnregister | bExportSvg)
            {
                bShuttingDown = true;
                ShowInTaskbar = false;
            }
        }

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
                if (bUnregister)
                {
                    Properties.Settings.Default.Reset();
                    RemoveAllSavedLayoutStates();
                }
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
                        bool bExportSvg = commandArgs.ArgPairs.ContainsKey("exportsvg");
                        if (bExportSvg)
                        {
                            var projectManager = manager as IUFProjectManager;
                            if (projectManager == null)
                                throw new ArgumentException();
                            bool bResult = false;
                            if (uriFile != null)
                                bResult = projectManager.ExportToSvg(uriFile, null);
                            else
                                bResult = projectManager.ExportToSvg(uri, null);
                            if (!bResult)
                            {
                                Thread.Sleep(1000); // wait for logger to be flushed
                                System.Environment.Exit(-1);
                            }
                            Visibility = System.Windows.Visibility.Hidden;
                            Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                            {
                                Visibility = System.Windows.Visibility.Hidden;
                                Dispatcher.CurrentDispatcher.InvokeShutdown();
                            });
                            return;
                        }
                        else
                        {
                            if (uriFile != null)
                                manager.Edit(uriFile, null);
                            else
                                manager.Edit(uri, null);
                        }
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

        public void AddAndInitializeComponent(IComponent component)
        {
            componentHost.Components.Add(component);
            if (component is IUFInterfaceBase)
                (component as IUFInterfaceBase).Initialize();
        }

        public void AddDockedControl(FrameworkElement control, String title, 
                                    DockSide dockSide, DockState dockState, 
                                    bool bCanClose = true, bool bCanFloat = true, 
                                    DockSide layoutGroupDockSide = DockSide.Left)
        {
            if (bShuttingDown)
                return;

            DockType dt;
            LayoutPanel panel = null;

            if (dockSide == DockSide.Tabbed)
            {
                dt = DockType.Fill;
                panel = new LayoutPanel();
                panel.ClosingBehavior = ClosingBehavior.ImmediatelyRemove;
                if (!tabbedPanels.Contains(panel))
                    tabbedPanels.Add(panel);
                if (!tbGroups.ContainsKey(control.GetType()))
                {
                    var tabbedGroup = DockingHelper.AddOrGetTabbedGroup(rootLayoutGroup, control, tbGroups);
                    dockingManager.DockController.Dock(tabbedGroup, rootLayoutGroup, DockingHelper.DockSideToDockType(layoutGroupDockSide));
                }
                dockingManager.SetPanelProperties(panel, control, title, dockState, dockSide, bCanClose, bCanFloat);
                tbGroups[control.GetType()].Add(panel);
                if (tbGroups[control.GetType()].Parent == null)
                    rootLayoutGroup.Items.Add(tbGroups[control.GetType()]);
            }
            else
            {
                dt = dockingManager.AddDockedControl(control, rootLayoutGroup, ChildDocumentGroup, tabbedPanels, title, dockSide, dockState, bCanClose, bCanFloat);
                panel = DockLayoutManager.GetLayoutItem(control) as LayoutPanel;
            }
            if (panel != null)
            {
                SetDockedItemSize(panel, dockState == DockState.Float ? DockOperation.Float : DockOperation.Dock);
                if (!String.IsNullOrEmpty(control.Name))
                    dockTypesMap[control.Name] = dt;
                panel.IsVisibleChanged += OnIsVisibleChanged;
                panel.FloatOnDoubleClick = false;
            }
            AddComponentActivationButton(control);
        }

        void AddComponentActivationButton(FrameworkElement fe)
        {
            var header = DockingHelper.GetHeader(fe);
            var btnExists = (from BarButtonItem bi in DockedElements where (bi as BarButtonItem).Content != null && (bi as BarButtonItem).Content.ToString() == header select bi).FirstOrDefault();
            if (DockingHelper.GetState(fe) != DockState.Document && btnExists == null)
            {
                var cmd = new RoutedUICommand(String.Format(Properties.Resources.OpenDockingWindow, header), String.Format(Properties.Resources.OpenDockingWindow, header), typeof(UFMainWindow));
                var cmdBinding = new CommandBinding(cmd, FlashDocking, CanAlwaysExecute);
                CommandBindings.Add(cmdBinding);
                var btn = new BarButtonItem()
                {
                    Content = header,
                    Command = cmd,
                    CommandParameter = fe
                };
                ToolTipService.SetShowOnDisabled(btn, true);
                DockedElements.Insert(0, btn);
                //biViewContainer.GetBindingExpression(BarLinkContainerItem.ItemLinksSourceProperty).UpdateTarget();
            }
        }

        public void RemoveDockedControl(FrameworkElement control)
        {
            var controlPanel = DockLayoutManager.GetLayoutItem(control);
            dockingManager.RemoveDockedControl(control);
            if (Properties.Settings.Default.AutoLoadWorkspace && ApplicationPropertiesHelper.GetProperty("UseLayoutToFile") as bool? != true && dockingManager.ActiveDockItem == null)
            {
                workSpaceComponent.UriRisolver.GetListInstalledDocumentManagers().ForEach(im =>
                {
                    var tb = im.GetToolbar();
                    if (tb != null && !defaultToolbars.Contains(tb))
                        tb.Hide();
                });
            }
            if (!String.IsNullOrEmpty(control.Name))
            {
                dockTypesMap.Remove(control.Name);
                dockSizeMap.Remove(control.Name);
            }
            if (tabbedPanels.Contains(controlPanel))
                tabbedPanels.Remove(controlPanel);
            if (tbGroups.ContainsKey(control.GetType()) && tbGroups[control.GetType()].Items.Count == 0)
                tbGroups.Remove(control.GetType());
        }
        public void SetDesiredDockSize(FrameworkElement el, Size size)
        {
            if (!String.IsNullOrEmpty(el.Name))
                dockSizeMap[el.Name] = size;
        }
        List<BaseLayoutItem> tabbedPanels = new List<BaseLayoutItem>();
        public void SetState(FrameworkElement el, DockState state)
        {
            if (el == null)
                return;

            var layoutItem = DockLayoutManager.GetLayoutItem(el);
            if (layoutItem != null)
            {
                if (state == DockState.Hidden)
                {
                    //dockingManager.DockController.Close(layoutItem);
                    //dockingManager.DockController.Hide(layoutItem);
                }
                else if (state == DockState.Float)
                {
                    SetDockedItemSize(layoutItem, DockOperation.Float);
                    dockingManager.DockController.Float(layoutItem);
                }
                else if (state == DockState.AutoHidden && layoutItem as LayoutPanel != null)
                    (layoutItem as LayoutPanel).AutoHidden = true;
                else
                {
                    if (layoutItem is LayoutPanel && DockingHelper.GetState(el) == DockState.AutoHidden)
                    {
                        (layoutItem as LayoutPanel).AutoHidden = false;
                        return;
                    }
                    BaseLayoutItem destinationItem = rootLayoutGroup;
                    var dt = DockingHelper.DockSideToDockType(GetSide(el));
                    if (state == DockState.Document)
                    {
                        var tabbedPanel = dockingManager.GetControlParentPanel(el.GetType(), layoutItem);
                        if (tabbedPanel != null)
                        {
                            destinationItem = tabbedPanel;
                            dt = DockType.Fill;
                            if (!tabbedPanels.Contains(layoutItem))
                                tabbedPanels.Add(layoutItem);
                            if (!tabbedPanels.Contains(destinationItem))
                                tabbedPanels.Add(destinationItem);
                        }
                    }
                    SetDockedItemSize(layoutItem, DockOperation.Dock);
                    dockingManager.DockController.Dock(layoutItem, destinationItem, dt);
                }
            }
        }
        public DockSide GetSide(FrameworkElement el)
        {
            if (!String.IsNullOrEmpty(el.Name) && dockTypesMap.ContainsKey(el.Name))
                return DockingHelper.DockTypeToDockSide(dockTypesMap[el.Name]);
            return DockSide.Bottom;
        }
        public DockType GetDockType(FrameworkElement el)
        {
            if (!String.IsNullOrEmpty(el.Name) && dockTypesMap.ContainsKey(el.Name))
                return dockTypesMap[el.Name];
            return DockType.None;
        }
        #endregion

        #region Events Handling

        void OnDockItemActivating(object sender, ItemCancelEventArgs e)
        {
            if (bLoadingDockingState || bLoadingMenuState || bShuttingDown || bActiveWindowChanging)
                return;

            var element = (e.Item as ContentItem)?.Content as FrameworkElement;
            bActiveWindowChanging = true;

            try
            {
                if (Properties.Settings.Default.AutoLoadWorkspace && ApplicationPropertiesHelper.GetProperty("UseLayoutToFile") as bool? != true)
                {
                    if (element != null && (DockingHelper.GetState(element) == DockState.Document))
                        elnew = element;

                    bool bSame = false;
                    if (elnew != null && elold != null && elnew.GetType().Name == elold.GetType().Name)
                        bSame = true;
                    if (!bShuttingDown && !bSame && elnew != elold && (elnew == null || !tabbedPanels.Contains(DockLayoutManager.GetLayoutItem(elnew))))
                    {
                        if (elold != null && (lastLoadedDockingState == null || lastLoadedDockingState == elold.GetType().Name))
                        {
                            SaveDockStates(elold.GetType().Name);
                            SaveMenuStates(elold.GetType().Name);
                        }

                        Action action = () =>
                        {
                            var restoreBusy = false;
                            if (!IsBusy)
                            {
                                IsBusy = true;
                                restoreBusy = true;
                            }

                            if (!bShuttingDown && elnew != null)
                            {
                                var stateToLoad = elnew.GetType().Name;
                                if (lastLoadedDockingState != stateToLoad)
                                {
                                    LoadDockStates(stateToLoad);
                                    LoadMenuStates(stateToLoad);

                                    DockingHelper.SetActiveWindow(dockingManager, elnew);
                                    elnew.Focusable = true;
                                    elnew.Focus();
                                    FocusManager.SetFocusedElement(elnew, elnew);
                                    // elnew = null;
                                }
                            }
                            if (restoreBusy)
                                IsBusy = false;
                        };

                        if (bLoading)
                            action();
                        else
                        {
                            if (activateInvoker == null)
                                activateInvoker = new DelayedSingleActionInvoker(action, TimeSpan.FromMilliseconds(100), DispatcherPriority.Background);

                            if (activateInvoker != null)
                                activateInvoker.BeginInvoke();
                        }

                        elold = element;
                    }
                }
                workSpaceComponent.OnActiveWindowChanging(element, e);
            }
            finally
            {
                bActiveWindowChanging = false;
            }
        }

        void OnDockItemActivated(object sender, DockItemActivatedEventArgs e)
        {
            if (bLoadingDockingState || bLoadingMenuState)
                return;

            var feNew = (e.Item as ContentItem)?.Content as FrameworkElement;
            var feOld = (e.OldItem as ContentItem)?.Content as FrameworkElement;

            if (feOld != feNew)
            {
                OnDockItemDeactivated(feOld);
                if (!bItemClosing)
                    workSpaceComponent.OnActiveWindowChanged(e.Item, new DependencyPropertyChangedEventArgs(DockLayoutManager.ActiveDockItemProperty, feOld, feNew));
            }

            //workSpaceComponent.OnWindowActivated(sender, e);

            if (feNew == null)
                return;
            var state = DockingHelper.GetState(feNew);
            if (state != DockState.AutoHidden)
                return;
            if (listActivated.Contains(feNew))
                return;
            listActivated.Add(feNew);

#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("Activated : Force AutoHideStart '{0}'", feNew.Name));
#endif
            var reaStart = new RoutedEventArgs(DockLayoutManager.DockItemActivatedEvent, feNew);
            workSpaceComponent.OnAutoHideAnimationStart(sender, reaStart);

#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("Activated : Force AutoHideStop '{0}'", feNew.Name));
#endif
            var reaStop = new RoutedEventArgs(DockLayoutManager.DockItemActivatedEvent, feNew);
            workSpaceComponent.OnAutoHideAnimationStop(sender, reaStop);

        }

        void OnDockItemDeactivated(FrameworkElement fe)
        {
            if (bLoadingDockingState || bLoadingMenuState)
                return;

            //workSpaceComponent.OnWindowDeactivated(sender, e);

            if (fe == null)
                return;
            var state = DockingHelper.GetState(fe);
            if (state != DockState.AutoHidden)
                return;
            if (!listActivated.Contains(fe))
                return;
            listActivated.Remove(fe);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("Deactivated : Force AutoHideStart '{0}'", fe.Name));
#endif
            var reaStart = new RoutedEventArgs(DockLayoutManager.DockItemActivatedEvent, fe);
            workSpaceComponent.OnAutoHideAnimationStart(DockLayoutManager.GetLayoutItem(fe), reaStart);

#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("Deactivated : Force AutoHideStop '{0}'", fe.Name));
#endif
            var reaStop = new RoutedEventArgs(DockLayoutManager.DockItemActivatedEvent, fe);
            workSpaceComponent.OnAutoHideAnimationStop(DockLayoutManager.GetLayoutItem(fe), reaStop);

        }

        void OnDockItemEndDocking(object sender, DockItemDockingEventArgs e)
        {
            var el = ((sender as DockLayoutManager).ActiveDockItem as ContentItem)?.Content as FrameworkElement;
            if (el != null && el is FrameworkElement && !String.IsNullOrEmpty((el as FrameworkElement).Name))
                dockTypesMap[(el as FrameworkElement).Name] = e.DockType;
            var bAsTab = e.DockTarget is TabbedGroup || e.DockTarget is LayoutPanel && e.DockType == DockType.Fill;
            var layoutItem = DockLayoutManager.GetLayoutItem(el);
            if (bAsTab && !tabbedPanels.Contains(layoutItem))
            {
                tabbedPanels.Add(DockLayoutManager.GetLayoutItem(el));
                if (e.DockTarget is LayoutPanel)
                    tabbedPanels.Add(e.DockTarget);
            }
            if (!bAsTab && tabbedPanels.Contains(layoutItem))
                tabbedPanels.Remove(layoutItem);
        }

        private void OnDockOperationStarting(object sender, DockOperationStartingEventArgs e)
        {
            var lgroup = e.Item as LayoutGroup;
            if (e.DockOperation == DockOperation.Float && lgroup != null)
            {
                var bForbidden = (from item in lgroup.Items where !item.AllowFloat select item).FirstOrDefault() != null;
                if (bForbidden)
                    e.Cancel = true;
            }

            if (e.DockOperation == DockOperation.Hide)
            {
                var activeWindow = dockingManager.GetActiveWindow();
                if (activeWindow != null)
                {
                    var cea = new ItemCancelEventArgs(dockingManager.ActiveDockItem);
                    workSpaceComponent.OnActiveWindowChanging(activeWindow, cea);
                    if (cea.Cancel)
                        e.Cancel = true;
                }
            }
        }

        void OnDockOperationCompleted(object sender, DockOperationCompletedEventArgs e)
        {
            string elName = ((e.Item as ContentItem)?.Content as FrameworkElement)?.Name;
            if (e.Item == null || String.IsNullOrEmpty(elName))
                return;

            if (dockTypesMap.ContainsKey(elName))
            {
                AutoHideType autoHideType;
                if (Enum.TryParse(dockTypesMap[elName].ToString(), out autoHideType))
                    AutoHideGroup.SetAutoHideType(e.Item, autoHideType);
            }
            
            SetDockedItemSize(e.Item, e.DockOperation);
        }

        private void OnDockItemClosed(object sender, DockItemClosedEventArgs e)
        {
            var panel = e.Item as LayoutPanel;
            panel.IsVisibleChanged -= OnIsVisibleChanged;
            closingPanels.Remove(panel.Name);
        }

        void OnDockItemClosing(object sender, ItemCancelEventArgs e)
        {
            if (e.Item is FloatGroup)
            {
                e.Cancel = true;
                return;
            }
            var panel = e.Item as LayoutPanel;
            if (panel != null)
            {
                if (closingPanels.Contains(panel.Name))
                    return;
                closingPanels.Add(panel.Name);
            }
            
            var fe = panel?.Content as FrameworkElement;
            var oldActiveItem = (dockingManager.ActiveDockItem as LayoutPanel)?.Content;

            if (fe != null && Properties.Settings.Default.AutoLoadWorkspace && ApplicationPropertiesHelper.GetProperty("UseLayoutToFile") as bool? != true)
            {
                if (DockingHelper.GetState(fe) == DockState.Document)
                {
                    if (lastLoadedDockingState == fe.GetType().Name)
                    {
                        lastSavedDocked = null;
                        lastSavedBar = null;
                        SaveDockStates(fe.GetType().Name);
                        SaveMenuStates(fe.GetType().Name);
                    }
                    if (elold == fe)
                        elold = null;
                    if (elnew == fe)
                        elnew = null;
                }
            }

            var cancelEventArgs = new CancelEventArgs() { Cancel = e.Cancel };
            bItemClosing = true;
            try
            {
                workSpaceComponent.OnCloseButtonClick(sender, new CloseButtonEventArgs() { Cancel = cancelEventArgs, TargetItem = fe });
            }
            finally
            {
                bItemClosing = false;
            }

            if (cancelEventArgs.Cancel)
            {
                e.Cancel = true;
                if (panel != null)
                    closingPanels.Remove(panel.Name);
                return;
            }

            if (fe != null) 
            {
                //var disposables = (from c in fe.GetChildrenOfType<FrameworkElement>() where c is IDisposable select c).ToList();

                if (fe is IDisposable)
                {
                    IDisposable dispose = fe as IDisposable;
                    dispose.Dispose();
                }

                if (listActivated.Contains(fe))
                    listActivated.Remove(fe);

                var docFound = (from c in dockingManager.GetAllDockedControls()
                                where DockingHelper.GetState(c) == DockState.Document && !tabbedPanels.Contains(DockLayoutManager.GetLayoutItem(c))
                                select c).ToList();

                if (docFound.Count > 0)
                    dockingManager.SetActiveWindow(docFound[0]);
                else
                {

                    if (Properties.Settings.Default.AutoLoadWorkspace && ApplicationPropertiesHelper.GetProperty("UseLayoutToFile") as bool? != true)
                        lastLoadedDockingState = null;
                    lastLoadedBar = null;

                    workSpaceComponent.OnActiveWindowChanged(e.Item, new DependencyPropertyChangedEventArgs(DockLayoutManager.ActiveDockItemProperty, oldActiveItem, null));
                }
            }
        }

        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var panel = sender as ContentItem;
            Dispatcher.BeginInvokeAsynchronously(() =>
            {
                if (bShuttingDown)
                    return;

                var fe = panel?.Content as FrameworkElement;
                if (fe != null)
                {
                    var ea = new DockStateEventArgs() { NewState = (bool)e.NewValue ? DockState.Dock : DockState.AutoHidden, OldState = (bool)e.NewValue ? DockState.AutoHidden : DockState.Dock };
                    fe.ApplyTemplate();
                    workSpaceComponent.OnDockStateChanged(fe, ea);
                }
            });
        }

        #endregion

        #region Commands

        private void OnFileExit(object sender, ExecutedRoutedEventArgs e)
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

        readonly Dictionary<DependencyObject, DockState> mapFullScreenStates = new Dictionary<DependencyObject, DockState>();
        readonly List<Bar> mapFullScreenBars = new List<Bar>();
        private void OnFullScreen(object sender, ExecutedRoutedEventArgs e)
        {
            if (isInFullScreenMode)
            {
                isInFullScreenMode = false;

                if (!String.IsNullOrEmpty(Properties.Settings.Default.WindowPlacement))
                {
                    WINDOWPLACEMENT wp = Properties.Settings.Default.WindowPlacement.FromXml<WINDOWPLACEMENT>();
                    if (wp.showCmd != 0)
                        SaveWindowPlacementState.Load(this, wp);
                    else
                        WindowState = WindowState.Normal;
                }
                else
                    WindowState = WindowState.Normal;

                foreach (var child in mapFullScreenStates.Keys)
                {
                    try
                    {
                        SetState(child as FrameworkElement, mapFullScreenStates[child]);
                    }
                    catch { }
                }

                mapFullScreenBars.ForEach(bar => bar.Visible = true);
            }
            else
            {
                isInFullScreenMode = true;
                Properties.Settings.Default.WindowPlacement = SaveWindowPlacementState.Save(this).ToXml();
                mapFullScreenStates.Clear();
                foreach (var child in dockingManager.GetAllDockedControls())
                {
                    try
                    {
                        var state = DockingHelper.GetState(child);
                        if (state == DockState.Dock || state == DockState.Float || tabbedPanels.Contains(DockLayoutManager.GetLayoutItem(child)))
                        {
                            mapFullScreenStates.Add(child, state);
                            SetState(child, DockState.AutoHidden);
                        }
                        //else if (state == DockState.Float)
                        //{
                        //    mapFullScreenStates.Add(child, state);
                        //    SetState(child, DockState.Hidden);
                        //}
                    }
                    catch { }
                }

                mapFullScreenBars.Clear();
                var toolBars = (from Bar b in mainBarManager.Bars where b.IsMainMenu == false select b).ToList();
                toolBars.ForEach(tb => tb.Visible = false);
                mapFullScreenBars.AddRange(toolBars);

                WindowState = WindowState.Maximized;
            }
            
            (fullScreenCommand as DevExpress.Xpf.Bars.BarCheckItem).IsChecked = WindowState == WindowState.Maximized;
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

            e.CanExecute = dockingManager.GetActiveWindow() != null && dockingManager.GetActiveWindow() is UserControl &&
                            (dockingManager.GetActiveWindow() as UserControl).Content is Panel &&
                            ((dockingManager.GetActiveWindow() as UserControl).Content as Panel).Children.Count > 0;
        }

        #endregion Commands

        #region Magnifier

        //private Magnifier MagnifierToolObj;
        //private bool _MagnifierTool = false;

        //public bool MagnifierTool
        //{
        //    get
        //    {
        //        return _MagnifierTool;
        //    }
        //    set
        //    {
        //        _MagnifierTool = value;
        //        if (_MagnifierTool)
        //        {
        //            if (MagnifierToolObj == null)
        //                MagnifierToolObj = new Magnifier();
        //            MagnifierToolObj.FrameBackground = Brushes.White;
        //            MagnifierToolObj.Name = "Magnifier";
        //            MagnifierToolObj.FrameHeight = 50;
        //            MagnifierToolObj.FrameWidth = 50;
        //            MagnifierToolObj.FrameRadius = 50;
        //            MagnifierToolObj.ZoomFactor = 0.5;
        //            MagnifierToolObj.FrameType = FrameType.Circle;
        //            MagnifierToolObj.EnableExport = true;

        //            MagnifierToolObj.AssociateWith(dockingManager);
        //        }
        //        else
        //        {
        //            MagnifierToolObj.AssociateWith(null);
        //        }
        //    }
        //}

        #endregion Magnifier


        public class BrushModel
        {
            public Brush Brush { get; set; }

            public string Name { get; set; }
        }

        #region SysTray

        private void OnNotificationAreaIconDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (bWindowClosing)
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

        #region Memory

        bool bNotCloseAnyDocuments;
        void CheckMemoryUsage()
        {
            long maxMemory = Properties.Settings.Default.MaxMbMemoryUsage;
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            long totalMbOfMemoryUsed = currentProcess.WorkingSet64 / (1024 * 1024);
            if (maxMemory > 0 && totalMbOfMemoryUsed > maxMemory)
            {
                MemoryCompressor.minimizeMemory(false, true);

                currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                totalMbOfMemoryUsed = currentProcess.WorkingSet64 / (1024 * 1024);

                if (!bNotCloseAnyDocuments &&
                    maxMemory > 0 && totalMbOfMemoryUsed > maxMemory &&
                    dockingManager.GetAllDockedControls().Count() > 1)
                {
                    int i = 0;
                    var toRemove = dockingManager.GetFirstDocumentDocked(ref i);
                    if (toRemove == null)
                        return;

                    if (MessageBox.Show(Properties.Resources.HighMemoryUsage,
                        Properties.Settings.Default.MainWindowTitle, MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        bNotCloseAnyDocuments = true;
                        return;
                    }

                    while (maxMemory > 0 && totalMbOfMemoryUsed > maxMemory &&
                            dockingManager.GetAllDockedControls().Count() > 1)
                    {
                        OnDockItemClosing(this, new ItemCancelEventArgs(DockLayoutManager.GetLayoutItem(toRemove)));

                        MemoryCompressor.minimizeMemory(false, true);

                        currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                        totalMbOfMemoryUsed = currentProcess.WorkingSet64 / (1024 * 1024);

                        toRemove = dockingManager.GetFirstDocumentDocked(ref i);
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

        #region Notification Service
        public virtual INotificationService NotificationService { get { return null; } }

        public void ShowPredefinedNotification(String s1, ImageSource image = null, String s2 = "", String s3 = "")
        {
            INotification notification = notificationService.CreatePredefinedNotification(s1, s2, s3, image);
            ShowNotification(notification);
        }

        void ShowNotification(INotification notification)
        {
            // LogService.LogLine("Showing...");
            notification.ShowAsync().ContinueWith(OnNotificationShown, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void OnNotificationShown(Task<NotificationResult> task)
        {
            try
            {
                switch (task.Result)
                {
                    case NotificationResult.Activated:
                        // LogService.LogLine("Activated");
                        break;
                    case NotificationResult.TimedOut:
                        // LogService.LogLine("Timed out");
                        break;
                    case NotificationResult.UserCanceled:
                        // LogService.LogLine("Canceled by user");
                        break;
                    case NotificationResult.Dropped:
                        // LogService.LogLine("Dropped (the queue is full)");
                        break;
                }
            }
            catch (AggregateException e)
            {
                // LogService.LogLine("Error: " + e.InnerException.Message);
            }
        }
        #endregion

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

        void SaveApplicationProperties()
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage)
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(), FileMode.Create, isoStorage))
                {
                    var settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    var map = new Dictionary<object, object>();
                    foreach (var name in Application.Current.Properties.Keys)
                    {
                        if (Application.Current.Properties[name] is String ||
                            Application.Current.Properties[name] is int ||
                            Application.Current.Properties[name] is bool ||
                            Application.Current.Properties[name] is double ||
                            Application.Current.Properties[name] is float ||
                            Application.Current.Properties[name] is short ||
                            Application.Current.Properties[name] is long)
                            map[name] = Application.Current.Properties[name];
                    }
                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        try
                        {
                            var serializer = new DataContractSerializer(typeof(Dictionary<object, object>));
                            serializer.WriteObject(writer, map);
                        }
                        catch (Exception ex)
                        {
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void LoadApplicationProperties()
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage)
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(), FileMode.OpenOrCreate, isoStorage))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (var reader = XmlReader.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(Dictionary<object, object>));
                        var map = serializer.ReadObject(reader) as Dictionary<object, object>;

                        foreach (var name in map.Keys)
                            Application.Current.Properties[name] = map[name];
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region DockStates

        void RestoreToolbars(List<String> shownSchemes)
        {
            if (bShuttingDown || shownSchemes == null || shownSchemes.Count <= 0)
                return;

            workSpaceComponent.UriRisolver.GetListInstalledDocumentManagers().ForEach(im =>
            {
                if (shownSchemes.Contains(im.TypeScheme))
                    im.GetToolbar()?.Show();
                else
                    im.GetToolbar()?.Hide();
            });
        }

        internal bool LoadDockMenuStates(string path)
        {
            if (bShuttingDown)
                return false;

            using (new WaitCursor())
            {
                try
                {
                    bLoadingDockingState = true;
                    if (File.Exists(path))
                    {
                        using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            dockingManager.RestoreLayoutFromStream(fileStream);
                        }
                    }
                    bLoadingDockingState = false;
                    bLoadingMenuState = true;

                    var activeToolbarsPath = String.Format("{0}_Toolbars", path);
                    var barsLayoutPath = String.Format("{0}_Bar", path);

                    if (File.Exists(activeToolbarsPath))
                    {
                        var shownSchemes = new List<String>();
                        using (var fileStream = File.Open(activeToolbarsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (var reader = new StreamReader(fileStream))
                            {
                                while (!reader.EndOfStream)
                                    shownSchemes.Add(reader.ReadLine().Trim());
                            }
                        }
                        RestoreToolbars(shownSchemes);
                    }
                    
                    if (File.Exists(barsLayoutPath))
                        DXSerializer.Deserialize(mainBarManager, barsLayoutPath, Properties.Settings.Default.ApplicationName, null);
                }
                catch (Exception ex)
                {
                    LogError(Properties.Resources.FailedToLoadAppState, ex, true);
                    return false;
                }
                finally
                {
                    bLoadingDockingState = false;
                    bLoadingMenuState = false;
                }
            }
            workSpaceComponent.OnDockItemRestored(this);
            return true;
        }

        internal bool SaveDockMenuStates(string path, VFS.FileSystemProviderBase fileSystemProvider = null)
        {
            if (bShuttingDown || isInFullScreenMode)
                return false;

            using (new WaitCursor())
            {
                try
                {
                    if (fileSystemProvider != null)
                    {
                        var fileManagerFile = new VFS.FileManagerFile(fileSystemProvider, path);
                        if (fileSystemProvider.Exists(fileManagerFile))
                        {
                            fileSystemProvider.DeleteFile(fileManagerFile);
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            dockingManager.SaveLayoutToStream(memoryStream);
                            fileSystemProvider.UploadFile(null, path, memoryStream.ToArray());
                        }
                    }
                    else
                    {
                        if (File.Exists(path))
                            File.Delete(path);

                        Directory.CreateDirectory(Path.GetDirectoryName(path));

                        using (var fileStream = File.Open(path, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                        {
                            RemoveLayoutExtraGroups(rootLayoutGroup);
                            dockingManager.SaveLayoutToStream(fileStream);
                        }
                    }

                    var activeToolbarsPath = String.Format("{0}_Toolbars", path);
                    var barsLayoutPath = String.Format("{0}_Bar", path);

                    if (fileSystemProvider != null)
                    {
                        var fileManagerFile = new VFS.FileManagerFile(fileSystemProvider, activeToolbarsPath);
                        if (fileSystemProvider.Exists(fileManagerFile))
                        {
                            fileSystemProvider.DeleteFile(fileManagerFile);
                        }

                        var bytes = new UTF8Encoding(true).GetBytes(String.Join("\n", workSpaceComponent.ActiveToolbarsSchemes));
                        fileSystemProvider.UploadFile(null, activeToolbarsPath, bytes);
                    }
                    else
                    {
                        if (File.Exists(activeToolbarsPath))
                            File.Delete(activeToolbarsPath);
                        using (var fileStream = File.Open(activeToolbarsPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                        {
                            var bytes = new UTF8Encoding(true).GetBytes(String.Join("\n", workSpaceComponent.ActiveToolbarsSchemes));
                            fileStream.Write(bytes, 0, bytes.Length);
                        }
                    }

                    if (fileSystemProvider != null)
                    {
                        var fileManagerFile = new VFS.FileManagerFile(fileSystemProvider, barsLayoutPath);
                        if (fileSystemProvider.Exists(fileManagerFile))
                        {
                            fileSystemProvider.DeleteFile(fileManagerFile);
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            DXSerializer.Serialize(mainBarManager, memoryStream, Properties.Settings.Default.ApplicationName, null);
                            fileSystemProvider.UploadFile(null, barsLayoutPath, memoryStream.ToArray());
                        }
                    }
                    else
                    {
                        if (File.Exists(barsLayoutPath))
                            File.Delete(barsLayoutPath);
                        DXSerializer.Serialize(mainBarManager, barsLayoutPath, Properties.Settings.Default.ApplicationName, null);
                    }
                    //using (var fileStream = File.Open(barsLayoutPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                    //{
                    //    mainBarManager.SaveLayoutToStream(fileStream);
                    //}
                }
                catch (Exception ex)
                {
                    LogError(Properties.Resources.FailedToSaveAppState, ex, true);
                    return false;
                }
            }
            return true;
        }

        internal void ReloadDockState(FrameworkElement el, String layoutFilePath, VFS.FileSystemProviderBase fileSystemProvider = null)
        {
            if (bShuttingDown)
                return;

            LoadDockStates(lastLoadedDockingState, true, layoutFilePath, el, fileSystemProvider);
        }

        String lastLoadedDockingState;
        String lastLoadedBar;
        void LoadDockStates(String name, bool bForce = false, string overriddenPanelLayoutPath = null, FrameworkElement overriddenPanelContent = null, VFS.FileSystemProviderBase fileSystemProvider = null)
        {
            if ((lastLoadedDockingState == name && !bForce) || bShuttingDown)
                return;
            lastLoadedDockingState = name;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                var e = new CancelEventArgs();
                workSpaceComponent.OnDockItemRestoring(this, e);
                if (!e.Cancel)
                {
                    using (new WaitCursor())
                    {
                        var storage = GetStorage();
                        try
                        {
                            bLoadingDockingState = true;

                            if (overriddenPanelLayoutPath != null) //Restoring saved single panel position
                            {
                                var panel = DockLayoutManager.GetLayoutItem(overriddenPanelContent);
                                if (panel != null)
                                {
                                    bLoadingLayoutInfo = true;
                                    panel.LoadLayoutInfo(rootLayoutGroup, ChildDocumentGroup, overriddenPanelLayoutPath, tbGroups, fileSystemProvider);
                                    bLoadingLayoutInfo = false;
                                }
                            }
                            else if (storage.FileExists(name))
                            {
                                using (var fileStream = storage.OpenFile(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    dockingManager.RestoreLayoutFromStream(fileStream);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //LogError(Properties.Resources.FailedToLoadAppState, ex);
                        }
                        finally
                        {
                            bLoadingDockingState = false;
                        }
                    }
                    workSpaceComponent.OnDockItemRestored(this);
                }
            }
        }

        void LoadMenuStates(String name)
        {
            var activeToolbarsName = $"{name}_Toolbars";
            var barsLayoutName = $"{name}_Bar";
            if (lastLoadedBar == barsLayoutName || bShuttingDown)
                return;
            lastLoadedBar = barsLayoutName;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                LoadComponentMenu(true);
                using (new WaitCursor())
                {
                    var storage = GetStorage();
                    try
                    {
                        bLoadingMenuState = true;
                        if (storage.FileExists(activeToolbarsName))
                        {
                            var shownSchemes = new List<String>();
                            using (var fileStream = storage.OpenFile(activeToolbarsName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                using (var reader = new StreamReader(fileStream))
                                {
                                    while (!reader.EndOfStream)
                                        shownSchemes.Add(reader.ReadLine().Trim());
                                }
                            }
                            RestoreToolbars(shownSchemes);
                        }
                        if (storage.FileExists(barsLayoutName))
                        {
                            //using (var fileStream = storage.OpenFile(barsLayoutName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                            //{
                            //    mainBarManager.RestoreLayoutFromStream(fileStream);
                            //}
                            String filePath = storage.GetType().GetField("m_RootDir", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(storage).ToString();
                            DXSerializer.Deserialize(mainBarManager, String.Format("{0}{1}", filePath, barsLayoutName), Properties.Settings.Default.ApplicationName, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(Properties.Resources.FailedToLoadAppState, ex);
                    }
                    finally
                    {
                        bLoadingMenuState = false;
                    }
                }
            }
        }

        void LogError(String errMsg, Exception ex, bool bPromptUser = false)
        {
            logGeneral.Error(errMsg, ex);
            System.Diagnostics.Trace.TraceError(ex.ToString());
            if (bPromptUser)
                workSpaceComponent.UIInterface.ShowError(errMsg);
        }

        internal bool SaveDockStates(string layoutFilePath, BaseLayoutItem layoutItem, VFS.FileSystemProviderBase fileSystemProvider = null)
        {
            if (bShuttingDown || isInFullScreenMode || Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return false;
            
            using (new WaitCursor())
            {
                try
                {
                    if (fileSystemProvider != null)
                    {
                        var fileManagerFile = new VFS.FileManagerFile(fileSystemProvider, layoutFilePath);
                        if (fileSystemProvider.Exists(fileManagerFile))
                        {
                            fileSystemProvider.DeleteFile(fileManagerFile);
                        }

                        if (layoutItem == null)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                dockingManager.SaveLayoutToStream(memoryStream);
                                fileSystemProvider.UploadFile(null, layoutFilePath, memoryStream.ToArray());
                            }
                        }
                        else
                            layoutItem.SaveLayoutInfo(ChildDocumentGroup, layoutFilePath, fileSystemProvider);
                    }
                    else
                    {
                        if (File.Exists(layoutFilePath))
                            File.Delete(layoutFilePath);

                        Directory.CreateDirectory(Path.GetDirectoryName(layoutFilePath));

                        if (layoutItem == null)
                        {
                            using (var fileStream = File.Open(layoutFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                            {
                                RemoveLayoutExtraGroups(rootLayoutGroup);
                                dockingManager.SaveLayoutToStream(fileStream);
                            }
                        }
                        else
                            layoutItem.SaveLayoutInfo(ChildDocumentGroup, layoutFilePath);
                    }
                }
                catch (Exception ex)
                {
                    LogError(Properties.Resources.FailedToSaveAppState, ex, true);
                    return false;
                }
            }
            return true;
        }
        
        String lastSavedDocked;
        String lastSavedBar;
        void SaveDockStates(String name)
        {
            if (lastSavedDocked == name || bShuttingDown || isInFullScreenMode)
                return;
            lastSavedDocked = name;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                using (new WaitCursor())
                {
                    var storage = GetStorage();
                    try
                    {
                        if (storage.FileExists(name))
                            storage.DeleteFile(name);
                        using (var fileStream = storage.OpenFile(name, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                        {
                            RemoveLayoutExtraGroups(rootLayoutGroup);
                            dockingManager.SaveLayoutToStream(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(Properties.Resources.FailedToSaveAppState, ex);
                    }
                }
            }
        }
        void SaveMenuStates(String name)
        {
            if (isInFullScreenMode)
                return;
            var activeToolbarsName = $"{name}_Toolbars";
            var barsLayoutName = $"{name}_Bar";
            if (lastSavedBar == barsLayoutName || bShuttingDown)
                return;
            lastSavedBar = barsLayoutName;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                LoadComponentMenu(true);
                using (new WaitCursor())
                {
                    var storage = GetStorage();
                    try
                    {
                        if (storage.FileExists(activeToolbarsName))
                            storage.DeleteFile(activeToolbarsName);
                        using (var fileStream = storage.OpenFile(activeToolbarsName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                        {
                            var bytes = new UTF8Encoding(true).GetBytes(String.Join("\n", workSpaceComponent.ActiveToolbarsSchemes));
                            fileStream.Write(bytes, 0, bytes.Length);
                        }
                        if (storage.FileExists(barsLayoutName))
                            storage.DeleteFile(barsLayoutName);

                        //using (var fileStream = storage.OpenFile(barsLayoutName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                        //{
                        //    mainBarManager.SaveLayoutToStream(fileStream);
                        //}
                        String filePath = storage.GetType().GetField("m_RootDir", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(storage).ToString();
                        DXSerializer.Serialize(mainBarManager, String.Format("{0}{1}", filePath, barsLayoutName), Properties.Settings.Default.ApplicationName, null);
                    }
                    catch (Exception ex)
                    {
                        LogError(Properties.Resources.FailedToSaveAppState, ex);
                    }
                }
            }
        }

        void RemoveAllSavedLayoutStates()
        {
            var storage = GetStorage();
            try
            {
                storage.Remove();
            }
            catch
            { }
        }

        void RemoveLayoutExtraGroups(LayoutGroup root)
        {
            var removedLayouts = new Dictionary<BaseLayoutItem, LayoutGroup>();
            for (int i = 0; i < root.Items.Count; i++)
            {
                var group = root.Items[i] as LayoutGroup;
                if (group != null)
                {
                    var lastGroup = GetLastGroup(group);
                    if (lastGroup != null)
                    {
                        dockingManager.BeginUpdate();
                        removedLayouts.Add(root.Items[i], lastGroup);
                        root.Items[i] = lastGroup;
                        dockingManager.EndUpdate();
                    }
                }
            }

            if (removedLayouts.Count > 0)
            {
                var hiddenLayouts = (from c in dockingManager.GetChildrenOfType<BaseLayoutItem>()
                                     where (c is LayoutPanel || c is LayoutGroup) && c.IsAutoHidden
                                     select c).ToList();
                foreach (var layout in hiddenLayouts)
                {
                    var dockProperties = layout.SerializableDockSituation;
                    for (int i = 0; i < dockProperties.Count; i++)
                    {
                        if (dockProperties[i].Parent != null && removedLayouts.ContainsKey(dockProperties[i].Parent))
                            dockProperties[i] = new DevExpress.Xpf.Docking.Internal.PlaceHolder(layout, removedLayouts[dockProperties[i].Parent]);
                    }
                }
            }
        }

        LayoutGroup GetLastGroup(LayoutGroup root)
        {
            LayoutGroup parentGroup = null;
            var currentGroup = GetSingleChildGroup(root);
            while (currentGroup != null)
            {
                parentGroup = currentGroup;
                currentGroup = GetSingleChildGroup(currentGroup);
            }
            if (parentGroup != null)
                parentGroup.Parent.Remove(parentGroup);
            return parentGroup;
        }

        LayoutGroup GetSingleChildGroup(LayoutGroup group)
        {
            if (group.Items.Count != 1)
            {
                RemoveLayoutExtraGroups(group);
                return null;
            }
            return group.Items[0] as LayoutGroup;
        }

        Dictionary<BaseLayoutItem, LayoutGroup> oldPanelsGroup = new Dictionary<BaseLayoutItem, LayoutGroup>();
        internal void ToggleComponentsVisibility(List<string> hiddenPanels)
        {
            workSpaceComponent.EasyModeToggleToolbars();
            if (hiddenPanels != null && hiddenPanels.Count > 0)
            {
                dockingManager.BeginUpdate();

                var panels = (from FrameworkElement el in dockingManager.GetAllDockedControls() where hiddenPanels.Contains(el.Name) select DockLayoutManager.GetLayoutItem(el)).ToList();
                if (workSpaceComponent.IsInEasyMode)
                {
                    oldPanelsGroup.Clear();
                    foreach (var panel in panels)
                    {
                        LayoutGroup oldGroup = panel.Parent;
                        AutoHideGroup newGroup = null;
                        var newGroupName = String.Format("AutoHideGroup_{0}", panel.Name);
                        if (oldGroup is AutoHideGroup)
                        {
                            newGroup = (AutoHideGroup)oldGroup;
                            if (oldGroup.Items.Count > 1)
                                newGroup = dockingManager.DockInAutoHiddenGroup(newGroupName, panel, DockSide.Top);
                        }
                        else //if (panel.Parent is TabbedGroup || panel.Parent is FloatGroup)
                            newGroup = dockingManager.DockInAutoHiddenGroup(newGroupName, panel, DockSide.Top);

                        if (oldGroup != null && newGroup != null)
                        {
                            oldPanelsGroup[panel] = oldGroup;
                            DockingHelper.HideAutoHideGroup(newGroup);
                        }
                    }
                }
                else
                {
                    foreach (var panel in panels)
                    {
                        LayoutGroup actualGroup = panel.Parent;
                        if (actualGroup == null || !oldPanelsGroup.ContainsKey(panel))
                            continue;

                        if (actualGroup != oldPanelsGroup[panel])
                        {
                            actualGroup = oldPanelsGroup[panel];
                            if (oldPanelsGroup[panel] is AutoHideGroup)
                                dockingManager.DockInAutoHiddenGroup(oldPanelsGroup[panel].Name, panel, DockSide.Top);
                            else if (oldPanelsGroup[panel] is TabbedGroup)
                                dockingManager.DockController.Dock(panel, oldPanelsGroup[panel], DockType.Fill);
                            else if (oldPanelsGroup[panel] is FloatGroup)
                            {
                                var newFloat = dockingManager.DockController.Float(panel);
                                newFloat.FloatLocation = ((FloatGroup)oldPanelsGroup[panel]).FloatLocation;
                                newFloat.FloatSize = ((FloatGroup)oldPanelsGroup[panel]).FloatSize;
                            }
                        }
                        if (actualGroup != null && actualGroup.Resources.Contains(typeof(DevExpress.Xpf.Docking.VisualElements.AutoHideTrayHeadersGroup)))
                            actualGroup.Resources.Remove(typeof(DevExpress.Xpf.Docking.VisualElements.AutoHideTrayHeadersGroup));
                    }
                }

                dockingManager.EndUpdate();
            }
        }

        #endregion

        #region EasyMode

        public bool IsInEasyMode
        {
            get
            {
                (rbEasyMode as BarCheckItem).IsChecked = Properties.Settings.Default.IsInEasyMode;
                return Properties.Settings.Default.IsInEasyMode;
            }
        }

        private void OnEasyMode(object sender, ExecutedRoutedEventArgs e)
        {
            Properties.Settings.Default.IsInEasyMode = !Properties.Settings.Default.IsInEasyMode;
            (rbEasyMode as DevExpress.Xpf.Bars.BarCheckItem).IsChecked = Properties.Settings.Default.IsInEasyMode;
            if (!Properties.Settings.Default.AutoLoadWorkspace)
            {
                LoadDockStates(GeneralWorkspaceStateFileName);
                LoadMenuStates(GeneralWorkspaceStateFileName);
            }
            workSpaceComponent.OnEasyModeChanged(this);
        }

        #endregion

        #region Application Theme
        public static readonly DependencyProperty ThemeKeyProperty = DependencyProperty.Register("ThemeKey", typeof(string), typeof(UFMainWindow), new UIPropertyMetadata(SharedResources.Properties.Settings.Default.ThemeKeyName));

        public string ThemeKey
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ThemeKeyProperty);
            }
            set
            {
                SetValue(ThemeKeyProperty, value);
            }
        }

        const string tmpExtension = ".tmp";
        private void OnChangeApplicationTheme(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var oldThemeKey = ThemeKey;
            var newThemeKey = e.Parameter as String;
            if (!String.IsNullOrEmpty(newThemeKey) && oldThemeKey != newThemeKey)
            {
                ThemeKey = newThemeKey;
                var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var sourcePath = Path.Combine(rootPath, Properties.Settings.Default.ThemeResourcesSubFolderName, newThemeKey);
                bool bError = false;
                try
                {
                    using (var cursor = new WaitCursor())
                    {
                        using (var copyProcess = new System.Diagnostics.Process())
                        {
                            copyProcess.StartInfo.FileName = Path.Combine(rootPath, Properties.Settings.Default.CopyTool);
                            copyProcess.StartInfo.Arguments = String.Format("/S\"{0}\" /D\"{1}\" /A", sourcePath, rootPath);
                            copyProcess.StartInfo.UseShellExecute = true;
                            copyProcess.StartInfo.CreateNoWindow = true;
                            copyProcess.StartInfo.Verb = "runas";
                            copyProcess.Start();
                            copyProcess.WaitForExit();
                            bError = copyProcess.ExitCode != 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    bError = true;
                    logGeneral.Error(ex.Message);
                }

                if (bError)
                {
                    ThemeKey = oldThemeKey;
                    workSpaceComponent.UIInterface.ShowWarning(String.Format(Properties.Resources.ChangeThemeFailled, sourcePath));
                    workSpaceComponent.ShowSystemLog();
                }
                else
                {
                    var skinName = ThemeHelper.GetTheme(newThemeKey);
                    Properties.Settings.Default.Skin = skinName;
                    Properties.Settings.Default.Save();
                    //ApplicationPropertiesHelper.SetProperty("CurrentSkin", skinName);
                    //ThemeHelper.SetTheme(this, skinName);
                    //ThemeHelper.SetDefaultUserLookAndFeel();
                    if (workSpaceComponent.UIInterface.ShowYesNo(Properties.Resources.AskApplicationRestart, UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Yes)
                    {
                        try
                        {
                            Close();
                        }
                        catch
                        { }

                        if (Visibility == Visibility.Collapsed)
                        {
                            var processName = Assembly.GetExecutingAssembly().Location;
                            var commandLine = Environment.CommandLine.Replace(String.Format("\"{0}\"", processName), String.Empty);
                            Process.Start(processName, commandLine);
                        }
                    }
                }
            }
        }
        #endregion

        private void OnLanguagePreferences(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            string filepath = "LanguagePreferences.exe";
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                filepath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), filepath);
            string args = String.Format("/P{0} /Y{1}", Process.GetCurrentProcess().Id, ApplicationPropertiesHelper.GetProperty("CurrentSkin").ToString());

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

        private void OnCheckCertificates(object sender, ExecutedRoutedEventArgs e)
        {
            Startup.CheckCertificate(false);
        }

        private void OnShowToolbar(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter as bool? == true)
                mainToolBar.Visible = true;
            else
                mainToolBar.Visible = false;
        }

        private void CanOpenExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = startupWelcome != null;
        }

        private void OnMainToolbarVisibleChanged(object sender, BarVisibileChangedEventArgs e)
        {
            var bar = sender as Bar;
            if (!e.NewValue && !isInFullScreenMode)
            {
                Dispatcher.BeginInvokeAsynchronously(() =>
                {
                    //bar.DockInfo = new BarDockInfo() { ContainerType = BarContainerType.Top };
                    bar.DockInfo.ContainerType = BarContainerType.Top;
                    bar.Visible = true;
                });
            }
        }

        private void BarManager_OnPreviewMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            LoadComponentMenu(true);
        }
    }
}

