using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DocumentManager.ComponentService;
using ScreenManager.ComponentService;
using Utilities;
using Utilities.WPF;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using WPFUtilities;
using DevExpress.Xpf.LayoutControl;
using System.IO.IsolatedStorage;
using System.Xml;
using System.IO;
using System.Text;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Controls.Primitives;
using GadgetLibrary;
using System.Runtime.Serialization;
using System.Windows.Data;
using log4net;
using StringManager.ComponentService;
using TranslationHelpers;
using DocumentManager.ComponentService.Helpers;
using DevExpress.Xpf.WindowsUI;
using DevExpress.Xpf.WindowsUI.Navigation;
using System.Collections.ObjectModel;
using Gma.System.MouseKeyHook;
using UFProjectManager.ComponentService;

namespace ScreenManager
{
    public class NavigationFrameEx : NavigationFrame
    {
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
        }
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            //var uie = e.Source as UIElement;
            //if (uie != null && !uie.IsManipulationEnabled)
            //    e.Cancel();
        }
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            //var uie = e.Source as UIElement;
            //if (uie != null && !uie.IsManipulationEnabled)
            //    e.Cancel();
        }
        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            //var uie = e.Source as UIElement;
            //if (uie != null && !uie.IsManipulationEnabled)
            //    e.Cancel();
        }
    }

    public class NavigatedItems
    {
        public EventHandler SelectedIndexChanged;
        int selectedIndex = int.MinValue;
        public int SelectedIndex {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (selectedIndex != value && value >= 0 && value < Items.Count)
                {
                    selectedIndex = value;
                    SelectedIndexChanged?.Invoke(this, null);
                }
            }
        }
        public NavigationItem CurrentItem
        {
            get
            {
                if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                    return Items[SelectedIndex];
                return null;
            }
        }
        public List<NavigationItem> Items { get; private set; }

        public NavigatedItems()
        {
            Items = new List<NavigationItem>();
        }
        public void GoHome()
        {
            SelectedIndex = 0;
        }
        public void GoBack()
        {
            SelectedIndex--;
        }
        public void GoNext()
        {
            SelectedIndex++;
        }
        public void GoLast()
        {
            SelectedIndex = Items.Count - 1;
        }
        public bool SearchAndSelectContent(object content)
        {
            var item = (from i in Items where i.Content == content select i).FirstOrDefault();
            if (item != null)
            {
                SelectedIndex = Items.IndexOf(item);
                return true;
            }
            return false;
        }
        public bool SearchAndSelectFirstOfType(Type type)
        {
            var item = (from i in Items where i.Content.GetType() == type select i).FirstOrDefault();
            if (item != null)
            {
                SelectedIndex = Items.IndexOf(item);
                return true;
            }
            return false;
        }
        public bool RemoveItem(object itemContent)
        {
            var item = (from i in Items where i.Content == itemContent select i).FirstOrDefault();
            if (item != null)
            {
                var currentItem = CurrentItem;
                Items.Remove(item);
                selectedIndex = Items.IndexOf(currentItem);
                if (Items.Count == 0)
                    GoHome();
                else if (SelectedIndex >= Items.Count)
                    GoLast();
                return true;
            }
            return false;
        }
    }

    public class NavigationItem
    {
        public readonly String Title;
        public readonly object Content;

        public NavigationItem(String title, object content)
        {
            Title = title;
            Content = content;
        }
    }

    /// <summary>
    /// Interaction logic for ScreenTransitionViewer.xaml
    /// </summary>
    public partial class ScreenTransitionViewer : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region Declarations

        private static readonly ILog logUsers = LogManager.GetLogger(Properties.Resources.UsersManager);

        List<String> listAutoLoadedOrdered = new List<String>();
        readonly Dictionary<String, ScreenViewer> mapLoaded = new Dictionary<String, ScreenViewer>(StringComparer.InvariantCultureIgnoreCase);
        readonly Dictionary<String, String> mapParameterFiles = new Dictionary<String, String>(StringComparer.InvariantCultureIgnoreCase);
        readonly Dictionary<String, int> mapRequestedMonitors = new Dictionary<String, int>(StringComparer.InvariantCultureIgnoreCase);
        readonly Dictionary<String, IDocument> mapParents = new Dictionary<String, IDocument>(StringComparer.InvariantCultureIgnoreCase);
        readonly Dictionary<String, UserControl> mapLoadedItems = new Dictionary<String, UserControl>(StringComparer.InvariantCultureIgnoreCase);
        readonly Dictionary<IScreenController, UserControl> mapLoadedControllers = new Dictionary<IScreenController, UserControl>();
        readonly Dictionary<String, DateTime> mapLoadedItemTimes = new Dictionary<String, DateTime>(StringComparer.InvariantCultureIgnoreCase);
        readonly ScreenManagerComponent EditorComponent;
        readonly IDocument DocumentParent;
        readonly IScreenController ScreenController;
        public ObservableCollection<Button> TabStripButtons
        {
            get;
            set;
        } = new ObservableCollection<Button>();
        bool tabStripVisible;
        public bool TabStripVisible
        {
            get
            {
                return tabStripVisible;
            }
            set
            {
                if (value != tabStripVisible)
                {
                    tabStripVisible = value;
                    OnPropertyChanged("TabStripVisible");
                }
            }
        }
        bool navigationButtonsVisible;
        public bool NavigationButtonVisible
        {
            get
            {
                return navigationButtonsVisible;
            }
            set
            {
                if (value != navigationButtonsVisible)
                {
                    navigationButtonsVisible = value;
                    OnPropertyChanged("NavigationButtonVisible");
                }
            }
        }
        bool bLoadedOnce;
        bool bLoaded;
        bool bLoading;
        bool bLoadingAutoLoadingScreen;
        //ScreensTileViewer sharedScreen;
        internal Uri ActivateOnLoad;
        bool bTest;
        //PropertyChangeNotifier notifier;
        int prevSelectedIndex = -1;
        DispatcherOperation dpForceActivating;
        DispatcherOperation dpForceDeactivating;
        readonly List<String> ListRecentScreens = new List<String>();
        DispatcherTimer delayHide;
        DispatcherTimer delayScreens;
        DispatcherTimer delaySetBusy;

        ScreenViewer layoutTop;
        ScreenViewer layoutLeft;
        ScreenViewer layoutRight;
        ScreenViewer layoutBottom;

        ScreenViewer appBarTop;
        ScreenViewer appBarBottom;
        ScreenViewer appBarLeft;
        ScreenViewer appBarRight;
        TouchHelper appBarTopBehavior;
        TouchHelper appBarLeftBehavior;
        TouchHelper appBarBottomBehavior;
        TouchHelper appBarRightBehavior;

        Dictionary<Uri, GadgetSettings> mapGadgetSetting;

        GeoViewer geoViewControl;

        bool bUserEnabled = false;

        bool bIsOnlyRuntime;
        String previousCulture;
        String PreviousCulture { get { return previousCulture; } }

        List<ScrollBar> listScrollBars;
        NavigatedItems navigatedItems = new NavigatedItems();
        double dHomePageWidth = Double.NaN;
        double dHomePageHeight = Double.NaN;
        object navigatingTo;
        bool bNavigationPending;
        IUFProjectManager iUFProjectManager;
        #endregion

        #region Events
        public event EventHandler ActiveContentChanged;
        void OnActiveContentChanged()
        {
            ActiveContentChanged?.Invoke(null/*this*/, new EventArgs());
        }
        #endregion

        #region Constructors

        public ScreenTransitionViewer(ScreenManagerComponent editorComponent, 
            IDocument parent, IScreenController screenController = null, bool test = false)
        {
            InitializeComponent();

            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(Window), cbShowClientStatus);
            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(Window), cbShowLog);
            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(Window), cbShowCrossReference);
            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(Window), cbShowWatchWindow);
            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(Window), cbLoginUser);
            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(Window), cbLogoutUser);
            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(Window), cbResetGadegets);
            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(Window), cbBack);
            System.Windows.Input.CommandManager.RegisterClassCommandBinding(typeof(Window), cbNext);

            // SetBusy(true);

            EditorComponent = editorComponent;
            DocumentParent = parent;
            ScreenController = screenController;
            bTest = test;

            if (stringManager == null)
                stringManager = DocumentParent.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            iUFProjectManager = DocumentParent.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            if (stringManager != null)
                stringManager.CultureChanged += StringEditor_CultureChanged;

            bIsOnlyRuntime = ApplicationPropertiesHelper.GetProperty<bool>("IsOnlyRuntime", false);

            if (Properties.Settings.Default.DisablePageTransitions)
                navigationControl.AnimationType = AnimationType.None;
            var oldTransition = navigationControl.AnimationType;
            navigationControl.AnimationType = AnimationType.None;
            navigationControl.SizeChanged += new SizeChangedEventHandler(TabNavigationControl_SizeChanged);
            Loaded += (o, e) =>
            {
                if (!bLoadedOnce)
                {
                    bLoadedOnce = true;
                    Dispatcher.BeginInvokeAsynchronously(() =>
                    {
                        bLoading = true;

                        //bLoadingAutoLoadingScreen = true;
                        //try
                        //{
                        //    LoadAutoLoadScreens();
                        //}
                        //finally
                        //{
                        //    bLoadingAutoLoadingScreen = false;
                        //}

                        Action action = () =>
                        {
                            navigationControl.Navigated += OnNavigated;
                            navigatedItems.SelectedIndexChanged += OnNavigationIndexChanged;

                            if (screenController != null)
                            {
                                if (screenController.GetStartType() == StartType.MainScreen)
                                {
                                    var uri = screenController.GetStartupScreen().GetPathString();
                                    AddTabStripItem(Path.GetFileNameWithoutExtension(uri), uri);
                                }
                                else
                                    AddTabStripItem(screenController.GetTitle(), null);
                                OpenOrActivate(screenController, bIsHome: true);
                            }
                            else
                            {
                                if (ActivateOnLoad != null)
                                {
                                    AddNewTabControlItem(mapLoaded[ActivateOnLoad.GetPathString()], ActivateOnLoad.GetPathString(), bIsHome: true);
                                }
                            // _transContainer.control = mapLoaded[ActivateOnLoad];
                                else if (mapLoaded.Count > 0)
                                    AddNewTabControlItem(mapLoaded.Values.First(), mapLoaded.Keys.First(), bIsHome: true);

                                // _transContainer.control = mapLoaded.Values.First();
                            }

                            if (listAutoLoadedOrdered != null && listAutoLoadedOrdered.Count() > 0)
                            {
                                bLoadingAutoLoadingScreen = true;

                                try
                                {
                                    LoadAutoLoadScreens();
                                }
                                finally
                                {
                                    bLoadingAutoLoadingScreen = false;
                                }
                                //navigatedItems.GoLast();
                                //navigatedItems.GoHome();

                                RestoreWindowPosition();
                            }

                            navigationControl.AnimationType = oldTransition;
                            OnActiveContentChanged();

                            // SetBusy(false);

                            ShowUserLogin();
                            bLoading = false;
                            bLoaded = true;
                        };

                        // if (listAutoLoadedOrdered == null || listAutoLoadedOrdered.Count() == 0)
                            action();
                        //else
                        //    Dispatcher.BeginInvokeAsynchronouslyInRender(action);
                    });
                }
            };

            navigationControl.GotFocus += (o, e) =>
                {
                    if (e.OriginalSource == navigationControl && navigatingTo != null)
                    {
                        var control = navigatingTo;
                        if (control is FrameworkElement)
                        {
                            var element = control as FrameworkElement;
                            element.Focusable = true;
                            element.Focus();
                        }
                    }
                };
        }

        private void TabNavigationControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateRect(e.NewSize.Width, e.NewSize.Height);
        }

        void OnNavigationIndexChanged(object sender, EventArgs e)
        {
            var navigatedItems = sender as NavigatedItems;
            var currentItem = navigatedItems.CurrentItem;
            if (currentItem == null || currentItem.Content == null)
            {
                ShowHideLayoutScreens(true);
                return;
            }

            navigatingTo = currentItem.Content;
            var action = new Action(() =>
            {
                bNavigationPending = true;
                navigationControl.ClearCache();
                navigationControl.Journal.ClearNavigationHistory();
                navigationControl.Navigate(currentItem.Content, null, saveToJournal: false);
                navigationFrameAdorner.Header = currentItem.Title;
                UpdateSelectedButton(currentItem.Title);

                var viewer = currentItem.Content as ScreenViewer;
                if (viewer != null && viewer.Document != null)
                    ShowHideLayoutScreens(!viewer.Document.HideLayoutScreens);
                else
                    ShowHideLayoutScreens(true);

                if (!bLoadingAutoLoadingScreen && currentItem.Content is ScreenViewer)
                {
                    var wnd = this.FindParent<Window>();
                    if (wnd == null || wnd.IsActive)
                    {
                        try
                        {
                            var relative = DocumentParent.MakeRelativeUri(new Uri(viewer.Document.FullPath, UriKind.RelativeOrAbsolute));
                            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.ActiveScreen, WPFUtilities.Converters.UriConverter.ToString(relative, DocumentParent.fileSystemProviderBase != null));
                        }
                        catch
                        {
                            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.ActiveScreen, String.Empty);
                        }
                    }
                }
                else
                    SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.ActiveScreen, String.Empty);
            });

            if (!bLoadingAutoLoadingScreen && bNavigationPending)
                Dispatcher.BeginInvokeAsynchronouslyInRender(action);
            else
                action();
        }

        void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (prevSelectedIndex != -1 && prevSelectedIndex < navigatedItems.Items.Count)
            {
                var prevcontrol = navigatedItems.Items[prevSelectedIndex].Content;
                var currenturi = mapLoadedItems.SingleOrDefault(x => x.Value == prevcontrol).Key;
                if (currenturi != null)
                {
                    if (mapLoadedItemTimes.ContainsKey(currenturi))
                        mapLoadedItemTimes.Remove(currenturi);
                    mapLoadedItemTimes.Add(currenturi, DateTime.Now);
                }
                if (prevcontrol is ScreenViewer)
                {
                    var viewer = prevcontrol as ScreenViewer;
                    viewer.SetActiveViewer(false);
                    viewer.Visibility = Visibility.Collapsed;

                    if (dpForceDeactivating != null &&
                        dpForceDeactivating.Status != DispatcherOperationStatus.Aborted &&
                        dpForceDeactivating.Status != DispatcherOperationStatus.Completed)
                        dpForceDeactivating.Abort();
                    dpForceDeactivating = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        dpForceDeactivating = null;
                        viewer.ForceDeactivatingNow();
                    });
                }
            }

            //var bFirstSelectedObject = navigatedItems.Items[0].Content; //navigationControl.Journal.BackStack.Last().Source;
            //var bWasFirstObjectActive = prevSelectedObject == bFirstSelectedObject;
            //prevSelectedObject = (sender as NavigationFrame).Content;
            //var bIsFirstObjectActive = prevSelectedObject == bFirstSelectedObject;
            var oldprevSelectedIndex = prevSelectedIndex;
            prevSelectedIndex = navigatedItems.SelectedIndex;
            bNavigationPending = false;

            var control = navigatedItems.CurrentItem;
            if (control.Content is FrameworkElement)
            {
                var fe = control.Content as FrameworkElement;

                if (navigatedItems.Items.Count > 0 && oldprevSelectedIndex == 0)
                {
                    var fePrev = navigatedItems.Items[0].Content as FrameworkElement;

                    if (fePrev != null && !(fePrev is ScreenViewer))
                    {
                        listScrollBars = (from c in fePrev.GetChildrenOfType<ScrollBar>()
                                          where c.Visibility == Visibility.Visible
                                          select c).ToList();
                        listScrollBars.ForEach(bar => bar.Visibility = Visibility.Collapsed);

                        if (dHomePageWidth == Double.NaN)
                            dHomePageWidth = fePrev.Width;
                        if (dHomePageHeight == Double.NaN)
                            dHomePageHeight = fePrev.Height;
                        var workingArea = System.Windows.SystemParameters.WorkArea;
                        fePrev.Width = workingArea.Width - 300;
                        fePrev.Height = workingArea.Height - 300;
                    }
                }
                else if (prevSelectedIndex == 0 && listScrollBars != null)
                {
                    fe.Width = dHomePageWidth;
                    fe.Height = dHomePageHeight;
                    listScrollBars.ForEach(bar => bar.Visibility = Visibility.Visible);
                }

                fe.Visibility = Visibility.Visible;
                var uri = mapLoadedItems.SingleOrDefault(x => x.Value == control.Content).Key;
                if (uri != null)
                {
                    // navigationControl.EnableTouch = true;
                    if (!listAutoLoadedOrdered.Contains(uri))
                    {
                        if (!ListScreens.Contains(uri))
                            ListScreens.Add(uri);
                        else
                        {
                            if (mapLoadedItemTimes.ContainsKey(uri))
                                mapLoadedItemTimes.Remove(uri);
                            mapLoadedItemTimes.Add(uri, DateTime.Now);
                        }
                    }
                }

                if (!bLoadingAutoLoadingScreen && fe is ScreenViewer)
                {
                    var viewer = fe as ScreenViewer;
                    viewer.SetActiveViewer(true);
                    viewer.Visibility = Visibility.Visible;
                    viewer.ForceLoadingNow();
                    if (!viewer.IsLoaded)
                    {
                        viewer.Loaded += viewer_Loaded;
                    }
                    else
                        UpdateWindowsPositionFromViewer(viewer);

                    if (dpForceActivating != null &&
                        dpForceActivating.Status != DispatcherOperationStatus.Aborted &&
                        dpForceActivating.Status != DispatcherOperationStatus.Completed)
                        dpForceActivating.Abort();
                    dpForceActivating = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        dpForceActivating = null;
                        viewer.ForceActivatingNow();
                    });
                }
            }

            //else
            //{
            //    navigationControl.EnableTouch = false;
            //    navigationControl.IsManipulationEnabled = false;
            //}

            StartDelayAutoHide();

            OnActiveContentChanged();

            ShowUserLogin();
        }

        //DispatcherOperation dp;
        void delayHide_Tick(object sender, EventArgs e)
        {
            if (delayHide != null)
                delayHide.Stop();

            if (navigatedItems.SelectedIndex < 0)
            {
                if (navigatedItems.Items.Count > 0)
                {
                    navigatedItems.GoHome();
                    var fe = navigatingTo as FrameworkElement;
                    if (fe != null)
                        fe.Visibility = Visibility.Visible;
                }
                return;
            }

            var control = navigatingTo;
            foreach (NavigationItem element in navigatedItems.Items)
            {
                var fe = element.Content as FrameworkElement;
                if (fe == null)
                    continue;
                if (fe == control)
                {
                    fe.Visibility = Visibility.Visible;
                    continue;
                }
                fe.Visibility = Visibility.Collapsed;
                //SkinStorage.SetOverrideVisualStyle(fe, true);
            }

            //if (!bLoadingAutoLoad)
            //{
            //    var viewer = control.Content as ScreenViewer;
            //    if (viewer != null)
            //    {
            //        if (dp != null && dp.Status != DispatcherOperationStatus.Aborted &&
            //            dp.Status != DispatcherOperationStatus.Completed)
            //            dp.Abort();
            //        dp = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            //        {
            //            dp = null;
            //            UpdateWindowsPositionFromViewer(viewer);
            //        });
            //    }
            //}
        }

        #endregion

        #region Users

        IStringEditorManager stringManager;
        static readonly string stringPlaceolder = "UserManagement";
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DocumentParent != null)
            {
                if (stringManager == null)
                    stringManager = DocumentParent.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                if (stringManager != null)
                {
                    var stringlist = stringManager.GetListStringForCulture(DocumentParent, stringManager.GetActiveCulture(DocumentParent));

                    loginUser.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LoginUserName", stringlist, Properties.UICommandResource.LoginUserName);
                    logoutUser.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LogoutUserName", stringlist, Properties.UICommandResource.LogoutUserName);
                }
            }

            flyoutControl.IsOpen = true;
        }

        bool? bSavedUserEnabled;
        void ShowUserLogin()
        {
            if (!bSavedUserEnabled.HasValue)
            {
                if (EditorComponent.UserEditor != null)
                    bUserEnabled = EditorComponent.UserEditor.GetEnableUserManager(DocumentParent) &&
                        EditorComponent.UserEditor.GetLoginControlVisible(DocumentParent);
                bSavedUserEnabled = bUserEnabled;
            }

            bool bVisible = bSavedUserEnabled.Value;
            if (bVisible)
            {
                var control = navigatingTo;
                var uri = mapLoadedItems.SingleOrDefault(x => x.Value == control).Key;
                bVisible = uri == null || homeScreenController != null &&
                    homeScreenController.GetStartType() == StartType.MainScreen &&
                    uri == homeScreenController.GetStartupScreen().GetPathString();
            }

            gridUser.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;

            if (!bLoaded && EditorComponent.AuthenticationCredentialsProvider != null && DocumentHelper.GetRootParent(DocumentParent, traverse: true) != null)
            {
                EditorComponent.AuthenticationCredentialsProvider.UserOnline += AuthenticationCredentialsProvider_UserOnline;
                EditorComponent.AuthenticationCredentialsProvider.RefreshCurrentUser(DocumentHelper.GetRootParent(DocumentParent, traverse: true).Title);
            }
        }

        void AuthenticationCredentialsProvider_UserOnline(object sender, UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs ev)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
                {
                    if (!String.IsNullOrEmpty(ev.User))
                    {
                        SetAutoLogout(ev.User);
                        if (!ev.IsRefreshing)
                        {
                            var username = ev.User;
                            if (EditorComponent.UserEditor != null)
                            {
                                var esign = EditorComponent.UserEditor.GetUserElectronicSignature(DocumentParent, username);
                                if (!String.IsNullOrEmpty(esign))
                                    username = String.Format("{0} ({1})", username, esign);

                                var role = EditorComponent.UserEditor.GetUserRole(DocumentParent, ev.User);
                                var level = EditorComponent.UserEditor.GetUserAccessLevel(DocumentParent, ev.User);
                                var mask = EditorComponent.UserEditor.GetUserAccessMask(DocumentParent, ev.User);
                                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentUser, ev.User);
                                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentRole, role);
                                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentAccessLevel, level);
                                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentAccessMask, mask);
                            }

                            logUsers.Info(String.Format(Properties.Resources.UserLogIn, username));
                            if (iUFProjectManager != null)
                                iUFProjectManager.AddLogEntity(DocumentParent, Properties.Resources.UsersManager,
                                  DateTime.UtcNow, String.Format(Properties.Resources.UserLogIn, username),
                                  System.Diagnostics.EventLogEntryType.Information);
                        }
                    }
                    else
                    {
                        ResetAutoLogout();
                        if (!ev.IsRefreshing)
                        {
                            logUsers.Info(Properties.Resources.UserLogOut);
                            if (iUFProjectManager != null)
                                iUFProjectManager.AddLogEntity(DocumentParent, Properties.Resources.UsersManager,
                                  DateTime.UtcNow, Properties.Resources.UserLogOut,
                                  System.Diagnostics.EventLogEntryType.Information);
                            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentUser, String.Empty);
                            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentRole, String.Empty);
                            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentAccessLevel, 0);
                            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.CurrentAccessMask, 0);
                        }
                        if (!String.IsNullOrEmpty(PreviousCulture))
                            StringEditor.SetActiveCulture(DocumentParent, PreviousCulture);
                    }
                    txtUser.Text = ev.User;
                });
        }
        IStringEditorManager StringEditor;
        private void OnLoginUser(object sender, ExecutedRoutedEventArgs e)
        {
            flyoutControl.IsOpen = false;
            if (e != null)
                e.Handled = true;
            if(DocumentParent != null)
            {
                if (StringEditor == null)
                    StringEditor = DocumentParent.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                if (StringEditor != null)
                    previousCulture = StringEditor.GetActiveCulture(DocumentParent);
            }
            var wnd = this.FindParent<Window>();
            EditorComponent.AuthenticationCredentialsProvider.ValidateUsingCredentialsProvider(DocumentParent, DocumentHelper.GetRootParent(DocumentParent, traverse: true).Title, owner: wnd);

            Focusable = true;
            Focus();
        }

        private void CanLoginUser(object sender, CanExecuteRoutedEventArgs e)
        {
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                var ret = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxY+APFmQPtOWSUaMJjYrwAg=="/* DBG */);
                if (!ret)
                    System.Environment.Exit(-10);
            }
#endif
            e.CanExecute = bUserEnabled && EditorComponent != null && DocumentHelper.GetRootParent(DocumentParent, traverse: true) != null &&
                !EditorComponent.AuthenticationCredentialsProvider.IsFaulted(DocumentHelper.GetRootParent(DocumentParent, traverse: true).Title);
        }

        private void OnLogoutUser(object sender, ExecutedRoutedEventArgs e)
        {
            if (e != null)
                e.Handled = true;
            EditorComponent.AuthenticationCredentialsProvider.Logout(DocumentHelper.GetRootParent(DocumentParent, traverse: true).Title);

            Focusable = true;
            Focus();
        }

        private void CanLogoutUser(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = bUserEnabled && DocumentHelper.GetRootParent(DocumentParent, traverse: true) != null && EditorComponent.AuthenticationCredentialsProvider.GetNumberOfUsersOnline(DocumentHelper.GetRootParent(DocumentParent, traverse: true).Title) > 0;
        }

        #region AutoLogout

        static DispatcherTimer autoLogoutTimer;
        IKeyboardMouseEvents globalHook;
        void SetAutoLogout(String user)
        {
            if (EditorComponent.UserEditor == null)
                return;

            int autoLogoutSeconds = EditorComponent.UserEditor.GetAutoLogoutSeconds(DocumentParent, user);
            if (autoLogoutSeconds == 0)
                return;
            if (autoLogoutTimer != null)
                autoLogoutTimer.Stop();
            else
            {
                autoLogoutTimer = new DispatcherTimer();
                autoLogoutTimer.Interval = TimeSpan.FromSeconds(autoLogoutSeconds);
                autoLogoutTimer.Tick += (o, e) =>
                {
                    OnLogoutUser(null, null);
                };

                if (globalHook != null)
                {
                    globalHook.MouseDownExt -= GlobalHookMouseDownExt;
                    globalHook.KeyPress -= GlobalHookKeyPress;
                    globalHook.Dispose();
                }
                globalHook = Hook.GlobalEvents();
                globalHook.MouseDownExt += GlobalHookMouseDownExt;
                globalHook.KeyPress += GlobalHookKeyPress;
                //PreviewMouseMove += ScreenViewer_PreviewMouseMove;
                //PreviewKeyDown += ScreenViewer_PreviewKeyDown;
            }

            autoLogoutTimer.Start();
        }

        private void GlobalHookKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            RestartAutologoutTimer();
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            RestartAutologoutTimer();
        }

        void ResetAutoLogout()
        {
            if (autoLogoutTimer == null)
                return;
            autoLogoutTimer.Stop();
            globalHook.MouseDownExt -= GlobalHookMouseDownExt;
            globalHook.KeyPress -= GlobalHookKeyPress;
            globalHook.Dispose();
            //PreviewMouseMove -= ScreenViewer_PreviewMouseMove;
            //PreviewKeyDown -= ScreenViewer_PreviewKeyDown;
            autoLogoutTimer = null;
        }

        void RestartAutologoutTimer()
        {
            if (autoLogoutTimer == null)
                return;
            autoLogoutTimer.Stop();
            autoLogoutTimer.Start();
        }

        //void ScreenViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    RestartAutologoutTimer();
        //}

        //void ScreenViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    RestartAutologoutTimer();
        //}

        #endregion

        #endregion

        void RemoveTabControlItem(String uri)
        {
            if (!mapLoadedItems.ContainsKey(uri))
                return;

            //int index = navigationControl.Items.IndexOf(mapLoadedItems[uri]);
            //if (index != 0)
            //    navigationControl.SelectedIndex = 0;
            //else if (navigationControl.Items.Count > 1)
            //    navigationControl.SelectedIndex = navigationControl.Items.Count - 1;

            var control = mapLoadedItems[uri];
            navigatedItems.RemoveItem(control);
            RemoveTabStripItem(uri);
            mapLoadedItems.Remove(uri);
            mapLoadedItemTimes.Remove(uri);

            //if (navigationControl.Items.Count == 0)
            //    navigationControl.SelectedIndex = 0;
            //else if (navigationControl.SelectedIndex >= navigationControl.Items.Count)
            //    navigationControl.SelectedIndex = navigationControl.Items.Count - 1;

            // Uncomment this line when you want use the memory profiler tool
            //minimizeMemory();
        }

        // Uncomment these lines when you want use the memory profiler tool
        //[System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        //private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
        //[return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        //[System.Runtime.InteropServices.DllImport("kernel32.dll")]
        //private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);
        //static DispatcherOperation mimMemoryOper;
        //internal static void minimizeMemory()
        //{
        //    Action action = () =>
        //    {
        //        System.Threading.Tasks.Task.Factory.StartNew(() =>
        //        {
        //            GC.Collect(GC.MaxGeneration);
        //            GC.WaitForPendingFinalizers();
        //            unchecked
        //            {
        //                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, (UIntPtr)(-1), (UIntPtr)(-1));
        //            }
        //        });
        //    };
        //    if (Dispatcher.CurrentDispatcher != null)
        //    {
        //        if (mimMemoryOper == null)
        //        {
        //            mimMemoryOper = Dispatcher.CurrentDispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
        //            mimMemoryOper.Completed += (o, e) =>
        //            {
        //                mimMemoryOper = null;
        //            };
        //        }
        //    }
        //    else
        //        action();
        //}
        FrameworkElement contentRoot;
        FrameworkElement border;
        void UpdateRect(double width, double height)
        {
            if(contentRoot == null)
                contentRoot = navigationControl.GetVisualChildrenOfType<Grid>().FirstOrDefault() as FrameworkElement;
            if (contentRoot != null)
            {
                contentRoot.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), new Size(width, height)) };
            }

            if (border == null)
            {
                border = navigationControl.GetVisualChildrenOfType<DXBorder>().FirstOrDefault();
                if (border != null)
                    border.Margin = new Thickness(0, 0, 0, -5);
            }
        }

        private void UpdateWindowsPositionFromViewer(ScreenViewer viewer)
        {
            UpdateRect(navigationControl.ActualWidth, navigationControl.ActualHeight);

            var wnd = this.FindParent<Window>();
            viewer.UpdateWindowPosition(wnd, bFromViewer: true);
            if (!viewer.ShowHeader && !viewer.ShowTabStrip && !viewer.ShowNavigation)
                navigationFrameAdorner.Visibility = Visibility.Collapsed;
            else
            {
                navigationFrameAdorner.Visibility = Visibility.Visible;
                if (!viewer.ShowHeader)
                    navigationFrameAdorner.Header = String.Empty;
                TabStripVisible = viewer.ShowTabStrip;
                NavigationButtonVisible = viewer.ShowNavigation;
            }

            var list = new List<Uri>();
            ListRecentScreens.ForEach(u => list.Add(new Uri(u, UriKind.RelativeOrAbsolute)));
            viewer.UpdateRecentList(list);

            OnActiveContentChanged();
        }

        void RemoveTabStripItem(String uri)
        {
            var btn = (from Button b in TabStripButtons where b.Tag as String == uri select b).FirstOrDefault();
            if (btn != null)
            {
                btn.Click -= OnTabStripItemClick;
                TabStripButtons.Remove(btn);
            }
        }

        void AddTabStripItem(String tooltip, String uri)
        {
            var bt = (from Button b in TabStripButtons where b.Tag as String == uri select b).FirstOrDefault();
            if (bt != null)
                return;

            var btn = new Button()
            {
                ToolTip = tooltip,
                Margin = new Thickness(3),
                Content = "",
                Style = Resources["btnUnselectedStyle"] as Style,
                Cursor = Cursors.Hand,
                Tag = uri,
            };
            btn.Click += OnTabStripItemClick;
            TabStripButtons.Add(btn);
        }

        void UpdateSelectedButton(string title)
        {
            foreach (var b in TabStripButtons)
                b.Style = (string)b.ToolTip == title ? Resources["btnSelectedStyle"] as Style : Resources["btnUnselectedStyle"] as Style;
        }

        void OnTabStripItemClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            UpdateSelectedButton((string)btn.ToolTip);
            var url = btn.Tag as String;
            if (String.IsNullOrEmpty(url))
                navigatedItems.GoHome();
            else if (mapLoadedItems.ContainsKey(url))
                navigatedItems.SearchAndSelectContent(mapLoadedItems[url]);
                //navigationControl.Navigate(mapLoadedItems[url]);
        }

        Dictionary<ScreenViewer, Window> mapViewerWindows = new Dictionary<ScreenViewer, Window>();
        bool bDoNotSetFocusToNavigation;
        void AddNewTabControlItem(UserControl control, String uri, bool bCheckMonitor = true, bool bIsHome = false)
        {
            if (String.IsNullOrEmpty(uri) || navigationControl == null)
                return;

            StartDelayAutoHide();

            // navigationControl.EnableTouch = true;
            if (mapLoadedItems.ContainsKey(uri))
            {
                //NavigateTo(mapLoadedItems[uri]);
                if (!navigatedItems.SearchAndSelectContent(mapLoadedItems[uri]))
                    navigatedItems.GoHome();
                
                //SkinStorage.SetOverrideVisualStyle(mapLoadedItems[uri] as DependencyObject, false);
                return;
            }

            if (bCheckMonitor && control is ScreenViewer)
            {
                var viewer = control as ScreenViewer;
                if (viewer.MonitorNumber > 0 && viewer.MonitorNumber <= System.Windows.Forms.SystemInformation.MonitorCount)
                {
                    bDoNotSetFocusToNavigation = true;
                    if (mapViewerWindows.ContainsKey(viewer))
                    {
                        viewer.UpdateWindowPosition(mapViewerWindows[viewer]);
                    }
                    else
                    {
                        var wnd = new DXWindow()
                            {
                                BorderEffect = BorderEffect.Default,
                                AllowsTransparency = viewer.Document != null && viewer.Document.WindowOpacity < 1,
                                WindowStyle = WindowStyle.None
                        };
                        wnd.Content = viewer;

                        if (Properties.Settings.Default.Aliased)
                            RenderOptions.SetEdgeMode(wnd, EdgeMode.Aliased);

                        ResourceDictionaryExtensions.AddCommonResources(wnd);
                        if (ScreenController != null)
                            ThemeHelper.SetTheme(wnd, ScreenController.GetTheme().ToString());

                        wnd.Show();
                        viewer.UpdateWindowPosition(wnd);
                        wnd.Closed += (o, e) =>
                            {
                                mapViewerWindows.Remove(viewer);
                                wnd.Content = null;
                                viewer.Dispose();
                                mapLoaded.Remove(uri);
                                EditorComponent.UpdatePreviousListRemove(viewer.CurrentUri);
                            };
                        mapViewerWindows.Add(viewer, wnd);
                    }

                    mapViewerWindows[viewer].Activate();
                    return;
                }
            }

            bDoNotSetFocusToNavigation = false;
            //var newTabItem = new TabNavigationItem()
            //{
            //    Header = System.IO.Path.GetFileNameWithoutExtension(uri),
            //    Content = control
            //};
            mapLoadedItems.Add(uri, control);
            AddTabStripItem(Path.GetFileNameWithoutExtension(uri), uri);
            navigationFrameAdorner.Header = Path.GetFileNameWithoutExtension(uri);
            if (bIsHome)
            {
                navigatedItems.Items.Insert(0, new NavigationItem(Path.GetFileNameWithoutExtension(uri), control));
                navigatedItems.GoHome();
            }
            else
            {
                navigatedItems.Items.Add(new NavigationItem(Path.GetFileNameWithoutExtension(uri), control));
                navigatedItems.GoLast();
            }

            //NavigateTo(control);

            if (control is ScreenViewer)
            {
                var viewer = control as ScreenViewer;
                viewer.ForceLoadingNow();
                if (!viewer.IsLoaded)
                {
                    viewer.Loaded += viewer_Loaded;
                }
                else
                    UpdateWindowsPositionFromViewer(viewer);
            }
        }

        void viewer_Loaded(object sender, RoutedEventArgs e)
        {
            var viewer = sender as ScreenViewer;
            viewer.Loaded -= viewer_Loaded;
            UpdateWindowsPositionFromViewer(viewer);
            //if (bLoaded)
            //    SetBusy(false);
        }

        #region Busy Content

        /*
        Window busyWindow;
        LongOperationControl longOperationControl;
        String busyContent;
        internal String GetBusyContent()
        {
            return busyContent;
        }

        internal void SetBusyContent(String waitText)
        {
            busyContent = waitText;
            if (busyWindow == null)
                return;

            busyWindow.Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
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

        Thread newWindowThread;
        */
        int nBusyCounter;
        internal void SetBusy(bool bSet)
        {
            if (Properties.Settings.Default.DisableBusyWindow)
                return;

            //if (EditorComponent.Workspace != null)
            //{
            //    EditorComponent.Workspace.IsBusy = bSet;
            //}
            //else
            {
                if (bSet)
                {
                    ++nBusyCounter;
                    if (nBusyCounter == 1 && !DevExpress.Xpf.Core.DXSplashScreen.IsActive)
                        DevExpress.Xpf.Core.DXSplashScreen.Show<WaitWindow>();
                }
                else
                {
                    if (nBusyCounter > 0)
                    {
                        --nBusyCounter;
                        if (nBusyCounter == 0 && DevExpress.Xpf.Core.DXSplashScreen.IsActive)
                            DevExpress.Xpf.Core.DXSplashScreen.Close();
                    }
                }
            }

            //if (bSet && busyWindow == null)
            //{
            //    ++nBusyCounter;

            //    // bool bStarted = false;
            //    bool bStopping = false;

            //    var wnd = this.FindParent<Window>();
            //    /*
            //    double top = wnd.Top;
            //    double left = wnd.Left;
            //    if (wnd.WindowState == System.Windows.WindowState.Maximized)
            //    {
            //        top = 0;
            //        left = 0;
            //    }
            //    double width = wnd.ActualWidth;
            //    double height = wnd.ActualHeight;
            //     * */
            //    var wp = SaveWindowPlacementState.Save(wnd);
            //    if (newWindowThread == null)
            //    {
            //        newWindowThread = new Thread((obj) =>
            //        {
            //            longOperationControl = new LongOperationControl();
            //            busyWindow = new Window()
            //            {
            //                WindowStartupLocation = System.Windows.WindowStartupLocation.Manual,
            //                SizeToContent = System.Windows.SizeToContent.Manual,
            //                WindowStyle = System.Windows.WindowStyle.None,
            //                ShowInTaskbar = false,
            //                AllowsTransparency = true,
            //                WindowState = System.Windows.WindowState.Normal,
            //                Background = Brushes.Transparent,
            //                Content = longOperationControl
            //            //Top = top,
            //            //Left = left,
            //            //Width = width,
            //            //Height = height
            //        };

            //            if (!String.IsNullOrEmpty(busyContent))
            //                longOperationControl.BusyContent = busyContent;
            //            longOperationControl.IsHitTestVisible = false;

            //            busyWindow.Loaded += (o, e) =>
            //            {
            //                SaveWindowPlacementState.Load(busyWindow, wp);
            //            };
            //            try
            //            {
            //                busyWindow.Show();
            //            }
            //            catch { }

            //            busyWindow.Closing += (o, e) =>
            //                {
            //                    e.Cancel = !bStopping;
            //                };

            //            busyWindow.Dispatcher.ShutdownStarted += (o, e) =>
            //            {
            //                bStopping = true;
            //                busyWindow.Close();
            //            };

            //            bool bActivating = false;
            //            busyWindow.Activated += (o, e) =>
            //            {
            //                if (!bActivating)
            //                {
            //                    bActivating = true;
            //                    wnd.Dispatcher.BeginInvokeIfRequired(() =>
            //                    {
            //                        wnd.Activate();
            //                        busyWindow.Dispatcher.BeginInvokeIfRequired(() =>
            //                        {
            //                            busyWindow.Activate();
            //                            bActivating = false;
            //                        });
            //                    });
            //                }
            //            };
            //            // bStarted = true;

            //        // Start the new window's Dispatcher
            //        System.Windows.Threading.Dispatcher.Run();
            //        });

            //        newWindowThread.SetApartmentState(ApartmentState.STA);
            //        newWindowThread.IsBackground = true;
            //        newWindowThread.Start();
            //    }
            //}
            //else
            //{
            //    if (bSet)
            //    {
            //        ++nBusyCounter;
            //        var wnd = this.FindParent<Window>();
            //        /*
            //        double top = wnd.Top;
            //        double left = wnd.Left;
            //        if (wnd.WindowState == System.Windows.WindowState.Maximized)
            //        {
            //            top = 0;
            //            left = 0;
            //        }
            //        double width = wnd.ActualWidth;
            //        double height = wnd.ActualHeight;
            //        */
            //        var wp = SaveWindowPlacementState.Save(wnd);
            //        busyWindow.Dispatcher.InvokeIfRequired(() =>
            //        {
            //            if (!String.IsNullOrEmpty(busyContent))
            //                longOperationControl.BusyContent = busyContent;
            //            else
            //                longOperationControl.BusyContent = Properties.Resources.WaitText;
            //            //busyWindow.Top = top;
            //            //busyWindow.Left = left;
            //            //busyWindow.Width = width;
            //            //busyWindow.Height = height;
            //            SaveWindowPlacementState.Load(busyWindow, wp);

            //            try
            //            {
            //                busyWindow.Show();
            //            }
            //            catch { }
            //        });
            //    }
            //    else if (busyWindow != null)
            //    {
            //        if (--nBusyCounter <= 0)
            //        {
            //            busyWindow.Dispatcher.BeginInvokeAsynchronouslyInRender(() => busyWindow.Hide());
            //            SetBusyContent(null);
            //            if (!bDoNotSetFocusToNavigation)
            //                navigationControl.Focus();
            //        }
            //    }
            //    else
            //    {
            //        if (!bSet)
            //            --nBusyCounter;
            //    }
            //}
        }

        #endregion

        public List<String> GetActiveListCommands(String culture)
        {
            var tileView = navigatingTo as TileViewer;
            if (tileView != null)
                return tileView.GetCommandList();
            var screenView = navigatingTo as ScreenViewer;
            if (screenView != null)
                return screenView.GetCommandList(culture);
            return null;
        }

        public void ExecuteCommandName(String command, String culture)
        {
            //var tabItem = navigationControl.Items[navigationControl.SelectedIndex] as TabNavigationItem;
            //if (tabItem == null)
            //    return;
            var tileView = navigatingTo as TileViewer;
            if (tileView != null)
                tileView.ExecuteCommand(command);
            var screenView = navigatingTo as ScreenViewer;
            if (screenView != null)
                screenView.ExecuteCommand(command, culture);
        }

        internal void OpenMap(IDocument parent, Rect zoomTo, bool enableZoomingScrolling,
            bool showMiniMap, bool showNextButton)
        {
            SetBusy(true);
            try
            {
                var screenController = parent as IScreenController;
                if (geoViewControl == null)
                {
                    geoViewControl = new GeoViewer(EditorComponent, screenController, this, parent);
                    navigationFrameAdorner.Header = screenController.GetTitle();
                    //var newTabItem = new TabNavigationItem()
                    //{
                    //    Header = screenController.GetTitle(),
                    //    Content = geoViewControl
                    //};
                    navigatedItems.Items.Add(new NavigationItem(screenController.GetTitle(), geoViewControl));
                    navigatedItems.GoLast();
                }
                else
                {
                    if (!navigatedItems.SearchAndSelectFirstOfType(typeof(GeoViewer)))
                        navigatedItems.GoHome();
                }
                var wnd = this.FindParent<Window>();

                System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.AllScreens[0].WorkingArea;
                wnd.Left = workingArea.Left;
                wnd.Top = workingArea.Top;
                wnd.Width = workingArea.Width;
                wnd.Height = workingArea.Height;
                wnd.SizeToContent = SizeToContent.Manual;

                wnd.WindowState = WindowState.Maximized;

                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    geoViewControl.ZoomTo(zoomTo);
                    geoViewControl.MapScrollingZooming(enableZoomingScrolling);
                    geoViewControl.SetOptions(showMiniMap, showNextButton);
                });
            }
            finally
            {
                SetBusy(false);
            }
        }

        IScreenController homeScreenController;
        public void OpenOrActivate(IScreenController screenController,
            IScreenController backController = null, bool bIsHome = false)
        {
            SetBusy(true);
            try
            {
                Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(Environment.GetCommandLineArgs());

                String file = String.Empty;
                if (commandArgs.ArgPairs.ContainsKey("screen"))
                    file = commandArgs.ArgPairs["screen"];

                Uri uri = null;
                if (!String.IsNullOrEmpty(file))
                {
                    try
                    {
                        uri = new Uri(file, UriKind.RelativeOrAbsolute);
                        uri = DocumentParent.MakeAbosoluteUri(uri);
                        if (uri != null && !ScreenSettings.ScreenDocument.ExistFile(uri.GetPathString(), DocumentParent.fileSystemProviderBase))
                        {
                            throw new ArgumentException(Properties.Resources.DocNotFound);
                        }
                    }
                    catch (Exception ex)
                    {
                        uri = null;
                        // MessageBox.Show(String.Format(Properties.Resources.CannotOpenCommandLineScreen, file, ex.Message));
                        EditorComponent.UIInterface.ShowError(String.Format(Properties.Resources.CannotOpenCommandLineScreen, file, ex.Message));
                    }
                }

                Background = new SolidColorBrush(screenController.GetIdentityColor());
                homeScreenController = screenController;
                // navigationControl.EnableTouch = false;
                navigationControl.IsManipulationEnabled = false;
                if (screenController.GetStartType() == StartType.MainScreen &&
                    screenController.GetStartupScreen() != null || uri != null)
                {
                    //Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    //{
                        if (uri != null)
                        {
                            OpenOrActivate(uri);
                            EditorComponent.UpdatePreviousList(uri);
                        }
                        else
                        {
                            var startupUri = screenController.GetStartupScreen();
                            OpenOrActivate(startupUri, bCheckMonitor: false);
                            EditorComponent.UpdatePreviousList(startupUri);
                        }

                        SetBusy(false);
                    //});
                }
                else
                {
                    if (mapLoadedControllers.ContainsKey(screenController))
                    {
                        if (!navigatedItems.SearchAndSelectContent(mapLoadedControllers[screenController]))
                            navigatedItems.GoHome();
                        SetBusy(false);
                        return;
                    }

                    ShowHideLayoutScreens(true);
                    UserControl control = null;
                    var parent = screenController as IDocument;
                    if (parent == null)
                        parent = DocumentParent;
                    switch (screenController.GetStartType())
                    {
                        case StartType.GeoPage:
                            control = geoViewControl = new GeoViewer(EditorComponent, screenController, this, parent, backController);
                            break;
                        default:
                            control = new TileViewer(EditorComponent, screenController, this, parent, backController);
                            break;
                        //case StartType.GalleryPage:
                        //    control = new GalleryViewer(EditorComponent, screenController, this, parent, backController);
                        //    break;
                    }
                    navigationFrameAdorner.Header = screenController.GetTitle();
                    //var newTabItem = new TabNavigationItem()
                    //{
                    //    Header = screenController.GetTitle(),
                    //    Content = control
                    //};
                    if (bIsHome)
                    {
                        navigatedItems.Items.Insert(0, new NavigationItem(screenController.GetTitle(), control));
                        navigatedItems.GoHome();
                    }
                    //else if (navigationControl.SelectedIndex < navigationControl.Items.Count - 1)
                    //{
                    //    navigationControl.Items.Insert(navigationControl.SelectedIndex + 1, newTabItem);
                    //    navigationControl.SelectedIndex = navigationControl.SelectedIndex + 1;
                    //}
                    else
                    {
                        navigatedItems.Items.Add(new NavigationItem(screenController.GetTitle(), control));
                        navigatedItems.GoLast();
                    }
                    
                    //NavigateTo(control);
                    mapLoadedControllers.Add(screenController, control);

                    var wnd = this.FindParent<Window>();

                    System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.AllScreens[0].WorkingArea;
                    wnd.Left = workingArea.Left;
                    wnd.Top = workingArea.Top;
                    wnd.Width = workingArea.Width;
                    wnd.Height = workingArea.Height;
                    wnd.SizeToContent = SizeToContent.Manual;

                    wnd.WindowState = WindowState.Maximized;

                    SetBusy(false);
                }
            }
            finally
            {
                // SetBusy(false);
            }
        }

        public void OpenPopup(Uri screen, UIElement target, String parameterFile = null,
            double dX = Double.NaN, double dY = Double.NaN, double dWidth = Double.NaN, double dHeight = Double.NaN, bool bOffsetRelativeToScreen = false)
        {
            var path = screen.GetPathString();
            if (!String.IsNullOrEmpty(parameterFile))
            {
                if (mapParameterFiles.ContainsKey(path))
                    mapParameterFiles.Remove(path);
                mapParameterFiles.Add(path, parameterFile);
            }
            else if (mapParameterFiles.ContainsKey(path))
                parameterFile = mapParameterFiles[path];

            if (navigatingTo is ScreenViewer)
            {
                var viewer = navigatingTo as ScreenViewer;
                viewer.OpenPopup(screen, parameterFile, dX, dY, dWidth, dHeight);
                return;
            }

            var view = target.FindParent<ScreenViewer>();
            if (view == null)
                return;
            view.OpenPopup(screen, parameterFile, dX, dY, dWidth, dHeight);
        }

        public bool CloseAllPopup(UIElement target)
        {
            if (target != null)
            {
                var view = target.FindAncestor<Popup>();
                if (view != null)
                {
                    view.IsOpen = false;
                    return true;
                }
            }

            if (navigatingTo is ScreenViewer)
            {
                var viewer = navigatingTo as ScreenViewer;
                return viewer.CloseAllPopup();
            }
            return false;
        }

        public void OpenOrActivate(Uri screen, bool bShared = false, String parameterFile = null, 
            IDocument parent = null, int nRequestedMonitor = -1, bool bCheckMonitor = true)
        {
            var path = screen.GetPathString();
            if (mapRequestedMonitors.ContainsKey(path))
                mapRequestedMonitors.Remove(path);
            if (nRequestedMonitor >= 0)
                mapRequestedMonitors.Add(path, nRequestedMonitor);

            if (!String.IsNullOrEmpty(parameterFile))
            {
                if (mapParameterFiles.ContainsKey(path))
                    mapParameterFiles.Remove(path);
                mapParameterFiles.Add(path, parameterFile);
            }
            else if (mapParameterFiles.ContainsKey(path))
                parameterFile = mapParameterFiles[path];

            if (parent != null)
            {
                if (mapParents.ContainsKey(path))
                    mapParents.Remove(path);
                mapParents.Add(path, parent);
            }
            else if (mapParents.ContainsKey(path))
                mapParents.Remove(path);

            if (!ListScreens.Contains(path))
            {
                if (delaySetBusy == null)
                    SetBusy(true);
                StartDelaySetBusyTimer();
            }

            if (/*!bShared && */ screen != null && !ListScreens.Contains(path))
                ListScreens.Add(path);
            if (bLoaded || bLoading)
            {
                //Dispatcher.BeginInvoke((Action)delegate
                //{
                    //if (bChangeTransitionType)
                    //    _transContainer.SetTransitionTranslate();
                    //else
                    //    _transContainer.SetTransitionFadeAndGrow();
                    //bChangeTransitionType = !bChangeTransitionType;

                    if (screen == null)
                    {
                        ActivatePrevious();
                    }
                    else
                    {
                        //if (bShared)
                        //{
                        //    if (sharedScreen == null)
                        //    {
                        //        sharedScreen = new ScreensTileViewer(EditorComponent, DocumentParent, this);

                        //        sharedScreen.ClosingGesture += (ob, ev) =>
                        //        {
                        //            ActivatePrevious();
                        //        };
                        //        sharedScreen.CanCloseGesture += (ob, ev) =>
                        //        {
                        //            ev.Cancel = CanActivatePrevious();
                        //        };
                        //        sharedScreen.HomeGesture += (ob, ev) =>
                        //        {
                        //            ActivateHome();
                        //        };
                        //        sharedScreen.OpenRecentUri += (ob, ev) =>
                        //        {
                        //            OpenOrActivate(ev.uri);
                        //        };
                        //        sharedScreen.PrevGesture += (ob, ev) =>
                        //        {
                        //            ActivatePrev();
                        //        };
                        //        sharedScreen.NextGesture += (ob, ev) =>
                        //        {
                        //            ActivateNext();
                        //        };
                        //        AddNewTabControlItem(sharedScreen, path);
                        //    }
                        //    else
                        //    {
                        //        if (!navigatedItems.SearchAndSelectFirstOfType(typeof(ScreensTileViewer)))
                        //            navigatedItems.GoHome();
                        //    }
                        //    sharedScreen.OpenOrActivate(screen, parent:parent, nRequestedMonitor:nRequestedMonitor, parameterFile: parameterFile);
                        //        // _transContainer.control = sharedScreen;
                        //}
                        //else
                        {
                            if (mapLoaded.ContainsKey(path))
                                AddNewTabControlItem(mapLoaded[path], path, bCheckMonitor);
                            // _transContainer.control = mapLoaded[screen];
                            ActivateOnLoad = screen;

                            if (!bLoading)
                                StartDelayUnloadTimer();
                        }
                    }
                //}, DispatcherPriority.Normal);
            }
            else
            {
                ActivateOnLoad = screen;
            }
        }

        private void StartDelayAutoHide()
        {
            if (delayHide != null)
                delayHide.Stop();
            else
            {
                delayHide = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(2) };
                delayHide.Tick += delayHide_Tick;
            }
            delayHide.Start();
        }

        private void StartDelayUnloadTimer(bool bPollNow = true)
        {
            if (bPollNow)
                delayScreens_Tick(null, EventArgs.Empty);

            if (delayScreens == null)
            {
                delayScreens = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(2) };
                delayScreens.Tick += delayScreens_Tick;

                delayScreens.Start();
            }
        }

        private void StartDelaySetBusyTimer()
        {
            if (delaySetBusy == null)
            {
                delaySetBusy = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0) };
                delaySetBusy.Tick += delaySetBusy_Tick;

                delaySetBusy.Start();
            }
        }

        void delaySetBusy_Tick(object sender, EventArgs e)
        {
            if (delaySetBusy == null)
                return;

            SetBusy(false);
            delaySetBusy.Stop();
            delaySetBusy.Tick -= delaySetBusy_Tick;
            delaySetBusy = null;
        }

        void delayScreens_Tick(object sender, EventArgs e)
        {
            if (delayScreens != null)
            {
                delayScreens.Stop();
                delayScreens.Tick -= delayScreens_Tick;
                delayScreens = null;
            }

            foreach (var str in ListScreens)
            {
                if (listAutoLoadedOrdered.Contains(str))
                    continue;

                //if (str == ActivateOnLoad)
                //    continue;
                if (!mapLoaded.ContainsKey(str) || !mapLoadedItemTimes.ContainsKey(str) ||
                     mapLoaded[str].CanUnload() && mapLoadedItems.ContainsKey(str) &&
                     navigatingTo != mapLoadedItems[str] &&
                     mapLoadedItemTimes[str].AddSeconds(mapLoaded[str].DelayUnload()) < DateTime.Now)
                {
                    //if (navigationControl.SelectedIndex < navigationControl.Items.Count)
                    //{
                        var screenviewer = navigatingTo as ScreenViewer;
                        if (screenviewer != null && screenviewer.CurrentUri.GetPathString() == str)
                            continue;

                        //if (navigationControl.SelectedTabItem != null)
                        //{
                        //    var screenviewer = navigationControl.SelectedTabItem.Content as ScreenViewer;
                        //    if (screenviewer != null && screenviewer.CurrentUri.GetPathString() == str)
                        //        continue;
                        //}
                    //}

                    ListScreens.Remove(str);
                    break;
                }
            }

            if (ListScreens.Count > 0)
            {
                if (ListScreens.Count == 1 && navigatingTo is ScreenViewer)
                    return;
                
                StartDelayUnloadTimer(false);
            }
        }

        bool CanActivatePrevious()
        {
            return navigatedItems.SelectedIndex > 0; //navigatingTo != navigatedItems.Items.First().Content; //navigationControl.Journal.BackStack.Last()
        }

        bool CanActivateNext()
        {
            return navigatedItems.SelectedIndex < navigatedItems.Items.Count - 1;
        }

        public void ActivatePrevious()
        {
            var oldTransition = navigationControl.AnimationType;
            if (!Properties.Settings.Default.DisablePageTransitions)
                navigationControl.AnimationType = AnimationType.SlideHorizontal;
            if (CanActivatePrevious())
                navigatedItems.GoBack();
            else if (navigatedItems.Items.Count > 1)
                navigatedItems.GoLast();
            else if (mapLoadedControllers.Count > 0)
            {
                var list = GetTotalListUri();
                if (list.Count > 0)
                    OpenOrActivate(list.Last());
            }
            navigationControl.AnimationType = oldTransition;

            StartDelayUnloadTimer();

            RestoreWindowPosition();
        }

        public void ActivatePrev()
        {
            var oldTransition = navigationControl.AnimationType;
            if (!Properties.Settings.Default.DisablePageTransitions)
                navigationControl.AnimationType = AnimationType.SlideHorizontal;
            if (mapLoadedControllers.Count > 0)
            {
                var list = GetTotalListUri();
                if (list.Count > 0)
                {
                    if (navigatingTo != null)
                    {
                        var uriFound = (from c in mapLoadedItems where c.Value == navigatingTo select c.Key).ToList();
                        if (uriFound.Count == 0)
                            OpenOrActivate(list.Last());
                        else
                        {
                            list.Reverse();
                            var next = list.SkipWhile(obj => obj.GetPathString() != uriFound[0]).Skip(1).ToList();
                            if (next.Count > 0)
                                OpenOrActivate(next[0]);
                            else
                                ActivateHome();
                        }
                    }
                }
            }
            else if (CanActivatePrevious())
                navigatedItems.GoBack();
            else if (navigatedItems.Items.Count > 1)
                navigatedItems.GoLast();
            navigationControl.AnimationType = oldTransition;

            StartDelayUnloadTimer();

            RestoreWindowPosition();
        }

        public void ActivateNext()
        {
            var oldTransition = navigationControl.AnimationType;
            if (!Properties.Settings.Default.DisablePageTransitions)
                navigationControl.AnimationType = AnimationType.SlideHorizontal;

            if (mapLoadedControllers.Count > 0)
            {
                var list = GetTotalListUri();
                if (list.Count > 0)
                {
                    if (navigatingTo != null)
                    {
                        var uriFound = (from c in mapLoadedItems where c.Value == navigatingTo select c.Key).ToList();
                        if (uriFound.Count == 0)
                            OpenOrActivate(list.First());
                        else
                        {
                            var next = list.SkipWhile(obj => obj.GetPathString() != uriFound[0]).Skip(1).ToList();
                            if (next.Count > 0)
                                OpenOrActivate(next[0]);
                            else
                                ActivateHome();
                        }
                    }
                }
            }
            else if (CanActivateNext())
                navigatedItems.GoNext();
            else if (CanActivatePrevious())
                navigatedItems.GoHome();

                navigationControl.AnimationType = oldTransition;

            StartDelayUnloadTimer();

            RestoreWindowPosition();
        }

        internal ScreenViewer GetSelectedScreenViewer()
        {
            if (navigatingTo == null)
                return null;
            return navigatingTo as ScreenViewer;
        }

        public void ActivateHome()
        {
            if (homeScreenController != null)
            {
                if (homeScreenController.GetStartType() == StartType.MainScreen &&
                    homeScreenController.GetStartupScreen() != null)
                {
                    OpenOrActivate(homeScreenController.GetStartupScreen());
                    return;
                }
            }
            var oldTransition = navigationControl.AnimationType;
            if (!Properties.Settings.Default.DisablePageTransitions)
                navigationControl.AnimationType = AnimationType.Fade;
            
            navigatedItems.GoHome();
            navigationControl.AnimationType = oldTransition;

            StartDelayUnloadTimer();

            RestoreWindowPosition();
        }

        private void RestoreWindowPosition()
        {
            if (homeScreenController != null && homeScreenController.GetStartType() == StartType.MainScreen)
                return;

            if (navigatedItems.SelectedIndex != 0)
                return;

            var wnd = this.FindParent<Window>();
            wnd.ShowActivated = true;
            wnd.WindowStyle = WindowStyle.None;
            wnd.ShowInTaskbar = true;

            System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.AllScreens[0].WorkingArea;
            wnd.Left = workingArea.Left;
            wnd.Top = workingArea.Top;
            wnd.Width = workingArea.Width;
            wnd.Height = workingArea.Height;
            wnd.SizeToContent = SizeToContent.Manual;
            wnd.WindowState = WindowState.Maximized;
        }
        public void ZoomIn()
        {
            if (navigatingTo != null)
            {
                var uriFound = (from c in mapLoadedItems where c.Value == navigatingTo select c.Key).ToList();
                if (uriFound.Count > 0)
                {
                    mapLoaded[uriFound[0]].ZoomIn();
                }
            }
        }

        public void ZoomOut()
        {
            if (navigatingTo != null)
            {
                var uriFound = (from c in mapLoadedItems where c.Value == navigatingTo select c.Key).ToList();
                if (uriFound.Count > 0)
                {
                    mapLoaded[uriFound[0]].ZoomOut();
                }
            }
        }

        public void Reset()
        {
            if (navigatingTo != null)
            {
                var uriFound = (from c in mapLoadedItems where c.Value == navigatingTo select c.Key).ToList();
                if (uriFound.Count > 0)
                {
                    mapLoaded[uriFound[0]].ResetZoom();
                }
            }
        }

        static List<Uri> GetTotalListUri(IScreenController controller)
        {
            var list = controller.GetScreenLists();

            controller.GetScreenControllers().ForEach(c =>
                {
                    var r = GetTotalListUri(c);
                    list.AddRange(r);
                });

            return list;
        }

        List<Uri> GetTotalListUri()
        {
            var list = new List<Uri>();

            if (mapLoadedControllers.Count > 0)
            {
                var controller = mapLoadedControllers.Keys.First();
                list = GetTotalListUri(controller);
            }

            return list;
        }

        #region Properties

        SafeObservableCollection<String> listScreens;
        [Browsable(false)]
        public SafeObservableCollection<String> ListScreens
        {
            get
            {
                if (listScreens == null)
                {
                    listScreens = new SafeObservableCollection<String>();
                    listScreens.CollectionChanged += (o, e) =>
                    {
                        if (e.NewItems != null && e.NewItems.Count != 0)
                        {
                            foreach (var uri in e.NewItems)
                            {
                                var u = uri as String;

                                if (ListRecentScreens.Contains(u))
                                    ListRecentScreens.Remove(u);
                                ListRecentScreens.Add(u);
                                if (ListRecentScreens.Count > 5)
                                    ListRecentScreens.RemoveAt(0);

                                if (!mapLoaded.ContainsKey(u))
                                {
                                    String parameterFile = null;
                                    if (mapParameterFiles.ContainsKey(u))
                                        parameterFile = mapParameterFiles[u];

                                    int nRequestedMonitor = -1;
                                    if (mapRequestedMonitors.ContainsKey(u))
                                        nRequestedMonitor = mapRequestedMonitors[u];

                                    var parent = DocumentParent;
                                    if (mapParents.ContainsKey(u))
                                        parent = mapParents[u];
                                    else if (parent != null)
                                    {
                                        var uriAbsolute = DocumentParent.MakeAbosoluteUri(new Uri(u, UriKind.RelativeOrAbsolute));
                                        parent = DocumentParent.UpdateParentFromUri(uriAbsolute);
                                    }

                                    var item = new ScreenViewer(EditorComponent, new Uri(u, UriKind.RelativeOrAbsolute), parent, this, bTest,
                                        parameterFile: parameterFile, nMonitor:nRequestedMonitor);

                                    if (item.Document != null)
                                        ShowHideLayoutScreens(!item.Document.HideLayoutScreens);

                                    mapLoaded.Add(u, item);
                                    if (mapLoadedItemTimes.ContainsKey(u))
                                        mapLoadedItemTimes.Remove(u);
                                    mapLoadedItemTimes.Add(u, DateTime.Now);
                                    SubscribeViewerEvents(ref item);
                                }
                            }
                        }

                        if (e.OldItems != null && e.OldItems.Count != 0)
                        {
                            foreach (var uri in e.OldItems)
                            {
                                var u = uri as String;
                                if (!mapLoaded.ContainsKey(u))
                                    continue;

                                RemoveTabControlItem(u);
                                if (mapLoaded[u].Content is IDisposable)
                                    (mapLoaded[u].Content as IDisposable).Dispose();
                                if (mapLoaded[u] is IDisposable)
                                    (mapLoaded[u] as IDisposable).Dispose();

                                var viewer = mapLoaded[u];
                                UnsubscribeViewerEvents(ref viewer);
                                mapLoaded.Remove(u);

                                if (mapParents.ContainsKey(u))
                                    mapParents.Remove(u);
                            }
                        }

                        // ShowUserLogin();
                        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    };
                }
                return listScreens;
            }
        }

        private void SubscribeViewerEvents(ref ScreenViewer item)
        {
            item.ClosingGesture += item_ClosingGesture;
            item.CanCloseGesture += item_CanCloseGesture;
            item.HomeGesture += item_HomeGesture;
            item.OpenRecentUri += item_OpenRecentUri;
            if (ScreenController != null && ScreenController.EnablePageChangeGesture)
            {
                item.PrevGesture += item_PrevGesture;
                item.NextGesture += item_NextGesture;
            }
        }

        private void UnsubscribeViewerEvents(ref ScreenViewer item)
        {
            item.ClosingGesture -= item_ClosingGesture;
            item.CanCloseGesture -= item_CanCloseGesture;
            item.HomeGesture -= item_HomeGesture;
            item.OpenRecentUri -= item_OpenRecentUri;
            item.PrevGesture -= item_PrevGesture;
            item.NextGesture -= item_NextGesture;
        }

        public event EventHandler PrevGesture;
        #region OnHomeGesture
        /// <summary>
        /// Triggers the PrevGesture event.
        /// </summary>
        public virtual void OnPrevGesture()
        {
            var e = PrevGesture;
            if (e != null)
                e(this, new EventArgs());
        }
        #endregion

        public event EventHandler NextGesture;
        
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region OnNextGesture
        /// <summary>
        /// Triggers the NextGesture event.
        /// </summary>
        public virtual void OnNextGesture()
        {
            var e = NextGesture;
            if (e != null)
                e(this, new EventArgs());
        }
        #endregion

        void item_NextGesture(object sender, EventArgs e)
        {
            // ActivateNext();
            OnNextGesture();
        }

        void item_PrevGesture(object sender, EventArgs e)
        {
            // ActivatePrev();
            OnPrevGesture();
        }

        void item_OpenRecentUri(object sender, OpenUriEventArgs e)
        {
            OpenOrActivate(e.uri);
        }

        void item_HomeGesture(object sender, EventArgs e)
        {
            ActivateHome();
        }

        void item_CanCloseGesture(object sender, CancelEventArgs e)
        {
            e.Cancel = CanActivatePrevious();
        }

        void item_ClosingGesture(object sender, EventArgs e)
        {
            // ActivatePrevious();
            OnPrevGesture();
        }
        #endregion

        void StringEditor_CultureChanged(object sender, EventArgs e)
        {
#if !WINDOWS_UWP
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
#else
            RunOnUIThread.Run(() =>
#endif
            {
                OnActiveContentChanged();
            });
        }

        void ShowHideLayoutScreens(bool bShow)
        {
            if (!bShow && (layoutTop != null && layoutTop.Visibility != Visibility.Collapsed 
                        || layoutLeft != null && layoutLeft.Visibility != Visibility.Collapsed
                        || layoutRight != null && layoutRight.Visibility != Visibility.Collapsed
                        || layoutBottom != null && layoutBottom.Visibility != Visibility.Collapsed))
            {
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    SaveLayout(DocumentParent.Title);
            }

            if (layoutTop != null)
                layoutTop.Visibility = bShow ? Visibility.Visible : Visibility.Collapsed;
            if (layoutLeft != null)
                layoutLeft.Visibility = bShow ? Visibility.Visible : Visibility.Collapsed;
            if (layoutRight != null)
                layoutRight.Visibility = bShow ? Visibility.Visible : Visibility.Collapsed;
            if (layoutBottom != null)
                layoutBottom.Visibility = bShow ? Visibility.Visible : Visibility.Collapsed;
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (stringManager != null)
            {
                stringManager.CultureChanged -= StringEditor_CultureChanged;
                stringManager = null;
            }

            if (delaySetBusy != null)
            {
                delaySetBusy.Stop();
                delaySetBusy.Tick -= delaySetBusy_Tick;
                delaySetBusy = null;
            }

            if (delayScreens != null)
            {
                delayScreens.Stop();
                delayScreens.Tick -= delayScreens_Tick;
                delayScreens = null;
            }

            if (delayHide != null)
            {
                delayHide.Stop();
                delayHide.Tick -= delayHide_Tick;
                delayHide = null;
            }

            navigationControl.SizeChanged -= new SizeChangedEventHandler(TabNavigationControl_SizeChanged);
            navigationControl.Navigated -= OnNavigated;
            navigatedItems.SelectedIndexChanged -= OnNavigationIndexChanged;

            if (EditorComponent.AuthenticationCredentialsProvider != null)
                EditorComponent.AuthenticationCredentialsProvider.UserOnline -= AuthenticationCredentialsProvider_UserOnline;
            ResetAutoLogout();

            foreach (var wnd in mapViewerWindows.Values.ToList())
                wnd.Close();
            mapViewerWindows.Clear();

            if (wndClientStatus != null)
                wndClientStatus.Close();
            if (wndLog != null)
                wndLog.Close();
            if (wndCrossReference != null)
                wndCrossReference.Close();
            if (wndWatch != null)
                wndWatch.Close();

            CleanAllGadgets();
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                SaveGadgetLayout(DocumentParent.Title);

            if (layoutTop != null || layoutLeft != null || layoutRight != null || layoutBottom != null)
            {
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    SaveLayout(DocumentParent.Title);
            }

            if (layoutTop != null)
            {
                layoutTop.Dispose();
                layoutTop = null;
            }
            if (layoutLeft != null)
            {
                layoutLeft.Dispose();
                layoutLeft = null;
            }
            if (layoutRight != null)
            {
                layoutRight.Dispose();
                layoutRight = null;
            }
            if (layoutBottom != null)
            {
                layoutBottom.Dispose();
                layoutBottom = null;
            }

            if (appBarTop != null)
            {
                appBarTopBehavior.Detach();
                appBarTopBehavior = null;

                appBarTop.Dispose();
                appBarTop = null;

                topBar = null;
            }
            if (appBarLeft != null)
            {
                appBarLeftBehavior.Detach();
                appBarLeftBehavior = null;

                appBarLeft.Dispose();
                appBarLeft = null;

                leftBar = null;
            }
            if (appBarBottom != null)
            {
                appBarBottomBehavior.Detach();
                appBarBottomBehavior = null;

                appBarBottom.Dispose();
                appBarBottom = null;

                bottomBar = null;
            }
            if (appBarRight != null)
            {
                appBarRightBehavior.Detach();
                appBarRightBehavior = null;

                appBarRight.Dispose();
                appBarRight = null;

                rightBar = null;
            }

            //if (busyWindow != null)
            //    busyWindow.Dispatcher.InvokeShutdown();

            foreach (var u in mapLoaded.Keys)
            {
                // _transContainer.control = null;

                if (mapLoaded[u].Content is IDisposable)
                    (mapLoaded[u].Content as IDisposable).Dispose();

                if (mapLoaded[u] is IDisposable)
                    (mapLoaded[u] as IDisposable).Dispose();

                var viewer = mapLoaded[u];
                UnsubscribeViewerEvents(ref viewer);
            }

            foreach (var u in mapLoadedControllers.Keys)
            {
                if (mapLoadedControllers[u] is IDisposable)
                    (mapLoadedControllers[u] as IDisposable).Dispose();
            }

            if (geoViewControl != null)
            {
                geoViewControl.Dispose();
                geoViewControl = null;
            }
            //if (sharedScreen != null)
            //{
            //    sharedScreen.Dispose();
            //    sharedScreen = null;
            //}
            mapLoaded.Clear();
            mapParents.Clear();
        }

        #endregion

        #region Commands
        GeneralDialogContent wndClientStatus;
        UserControl control;
        private void OnShowClientStatus(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (wndClientStatus != null)
                wndClientStatus.Activate();
            else
            {
                control = EditorComponent.OPCUAClientStatus.GetClientStatusControl();

                wndClientStatus = new GeneralDialogContent(control, GeneralDialogButtons.None)
                {
                    Owner = this.FindParent<Window>(),
                    Title = Properties.Resources.ClientStatusTitle,
                    HelpLink = "ClientStatus"

                };
                wndClientStatus.Closing += (ev, ob) =>
                    {
                        if (wndClientStatus.Owner != null)
                        {
                            wndClientStatus.Owner.Focusable = true;
                            wndClientStatus.Owner.Focus();
                        }

                        wndClientStatus = null;
                    };
                wndClientStatus.Show();

                /*
                var height = control.Height;
                var width = control.Width;
                control.ClearValue(FrameworkElement.WidthProperty);
                control.ClearValue(FrameworkElement.HeightProperty);

                wndClientStatus = new DXWindow()
                {
                    Content = control,
                    SizeToContent = SizeToContent.Manual,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this.FindParent<Window>(),
                    WindowStyle = WindowStyle.ToolWindow,
                    Width = width,
                    Height = height,
                    Title = Properties.Resources.ClientStatusTitle,
                    BorderEffect = BorderEffect.Default
                };

                wndClientStatus.SourceInitialized += (o, ev) =>
                    {
                        // wndClientStatus.Background = Application.Current.MainWindow.Background;
                        ThemeHelper.SetTheme(wndClientStatus);

                        wndClientStatus.WindowState = WindowState.Normal;

                        Utilities.WINDOWPLACEMENT wp = (Utilities.WINDOWPLACEMENT)Properties.Settings.Default.ClientStatusWindowPlacement;
                        wndClientStatus.Top = wp.normalPosition.Top;
                        wndClientStatus.Left = wp.normalPosition.Left;
                        var w = wp.normalPosition.Right - wp.normalPosition.Left;
                        if (w > 0)
                            wndClientStatus.Width = w;
                        var h = wp.normalPosition.Bottom - wp.normalPosition.Top;
                        if (h > 0)
                            wndClientStatus.Height = h;
                    };

                wndClientStatus.Closed += (o, ev) =>
                    {
                        if (control is IDisposable)
                            (control as IDisposable).Dispose();
                        control = null;
                        try
                        {
                            Utilities.WINDOWPLACEMENT wp = (Utilities.WINDOWPLACEMENT)Properties.Settings.Default.ClientStatusWindowPlacement;
                            wp.normalPosition.Top = (int)wndClientStatus.Top;
                            wp.normalPosition.Left = (int)wndClientStatus.Left;
                            wp.normalPosition.Right = (int)(wndClientStatus.Left + wndClientStatus.Width);
                            wp.normalPosition.Bottom = (int)(wndClientStatus.Top + wndClientStatus.Height);

                            Properties.Settings.Default.Save();
                        }
                        catch { }

                        wndClientStatus = null;
                    };
                wndClientStatus.Show();
                */
            }
        }

        private void CanShowClientStatus(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditorComponent.OPCUAClientStatus != null && !bIsOnlyRuntime;
        }

        GeneralDialogContent wndLog;
        Log4NetViewer.Log4NetViewer logView;
        private void OnShowLog(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (wndLog != null)
                wndLog.Activate();
            else
            {
                logView = new Log4NetViewer.Log4NetViewer();
                logView.Document = DocumentParent;

                wndLog = new GeneralDialogContent(logView, GeneralDialogButtons.None)
                {
                    Owner = this.FindParent<Window>(),
                    Title = Properties.Resources.LogTitle,
                    HelpLink = "Log"

                };
                wndLog.Closing += (ev, ob) =>
                    {
                        if (wndLog.Owner != null)
                        {
                            wndLog.Owner.Focusable = true;
                            wndLog.Owner.Focus();
                        }

                        wndLog = null;
                    };
                wndLog.Show();

                /*
                var height = logView.Height;
                var width = logView.Width;
                logView.ClearValue(FrameworkElement.WidthProperty);
                logView.ClearValue(FrameworkElement.HeightProperty);

                wndLog = new DXWindow()
                {
                    Content = logView,
                    SizeToContent = SizeToContent.Manual,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this.FindParent<Window>(),
                    WindowStyle = WindowStyle.ToolWindow,
                    Width = width,
                    Height = height,
                    Title = Properties.Resources.LogTitle,
                    BorderEffect = BorderEffect.Default
                };

                wndLog.SourceInitialized += (o, ev) =>
                {
                    // wndClientStatus.Background = Application.Current.MainWindow.Background;
                    ThemeHelper.SetTheme(wndLog);

                    //Utilities.WINDOWPLACEMENT wp = (Utilities.WINDOWPLACEMENT)Properties.Settings.Default.ClientStatusWindowPlacement;
                    //if (wp.showCmd != 0)
                    //    SaveWindowPlacementState.Load(wndClientStatus, wp);
                    //else
                    wndLog.WindowState = WindowState.Normal;
                };
                wndLog.Closed += (o, ev) =>
                {
                    if (logView is IDisposable)
                        (logView as IDisposable).Dispose();
                    logView = null;
                    //Properties.Settings.Default.ClientStatusWindowPlacement = SaveWindowPlacementState.Save(wndClientStatus);
                    //Properties.Settings.Default.Save();
                    wndLog = null;
                };
                wndLog.Show();
                */
            }
        }

        private void CanShowLog(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !bIsOnlyRuntime;
        }

        GeneralDialogContent wndCrossReference;
        UserControl controlCrossReference;
        private void OnShowCrossReference(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (wndCrossReference != null)
                wndCrossReference.Activate();
            else
            {
                controlCrossReference = EditorComponent.CrossReferenceEditor.GetRuntimeControl(DocumentParent);
                if (controlCrossReference == null)
                    return;
                wndCrossReference = new GeneralDialogContent(controlCrossReference, GeneralDialogButtons.None)
                {
                    Owner = this.FindParent<Window>(),
                    Title = Properties.Resources.CrossReferenceTitle,
                    HelpLink = "CrossReference"

                };
                wndCrossReference.Closing += (ev, ob) =>
                {
                    if (wndCrossReference.Owner != null)
                    {
                        wndCrossReference.Owner.Focusable = true;
                        wndCrossReference.Owner.Focus();
                    }

                    wndCrossReference = null;
                };
                wndCrossReference.Show();
                /*
                var height = controlCrossReference.Height;
                var width = controlCrossReference.Width;
                controlCrossReference.ClearValue(FrameworkElement.WidthProperty);
                controlCrossReference.ClearValue(FrameworkElement.HeightProperty);

                wndCrossReference = new DXWindow()
                {
                    Content = controlCrossReference,
                    SizeToContent = SizeToContent.Manual,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this.FindParent<Window>(),
                    WindowStyle = WindowStyle.ToolWindow,
                    Width = width,
                    Height = height,
                    Title = Properties.Resources.ClientStatusTitle,
                    BorderEffect = BorderEffect.Default
                };

                wndCrossReference.SourceInitialized += (o, ev) =>
                {
                    // wndCrossReference.Background = Application.Current.MainWindow.Background;
                    ThemeHelper.SetTheme(wndCrossReference);

                    //Utilities.WINDOWPLACEMENT wp = (Utilities.WINDOWPLACEMENT)Properties.Settings.Default.ClientStatusWindowPlacement;
                    //if (wp.showCmd != 0)
                    //    SaveWindowPlacementState.Load(wndCrossReference, wp);
                    //else
                    wndCrossReference.WindowState = WindowState.Normal;
                };
                wndCrossReference.Closed += (o, ev) =>
                {
                    if (controlCrossReference is IDisposable)
                        (controlCrossReference as IDisposable).Dispose();
                    controlCrossReference = null;
                    //Properties.Settings.Default.ClientStatusWindowPlacement = SaveWindowPlacementState.Save(wndCrossReference);
                    //Properties.Settings.Default.Save();
                    wndCrossReference = null;
                };
                wndCrossReference.Show();
                */
            }
        }

        private void CanShowCrossReference(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditorComponent.CrossReferenceEditor != null && !bIsOnlyRuntime;
        }

        GeneralDialogContent wndWatch;
        WatchControl.WatchContainer watchContainer;
        private void OnShowWatchWindow(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (wndWatch != null)
                wndWatch.Activate();
            else
            {
                watchContainer = new WatchControl.WatchContainer(DocumentParent);

                wndWatch = new GeneralDialogContent(watchContainer, GeneralDialogButtons.None)
                {
                    Owner = this.FindParent<Window>(),
                    Title = Properties.Resources.WatchTitle,
                    HelpLink = "WatchWindow"
                };
                wndWatch.Closing += (ev, ob) =>
                    {
                        if (wndWatch.Owner != null)
                        {
                            wndWatch.Owner.Focusable = true;
                            wndWatch.Owner.Focus();
                        }
                        wndWatch = null;
                    };
                wndWatch.Show();
            }
        }

        private void CanShowWatchWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !bIsOnlyRuntime;
        }

        private void CanBack(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = navigatedItems.SelectedIndex > 0;
        }

        private void CanNext(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = navigatedItems.SelectedIndex < navigatedItems.Items.Count - 1;
        }

        private void OnBack(object sender, ExecutedRoutedEventArgs e)
        {
            navigatedItems.GoBack();
        }

        private void OnNext(object sender, ExecutedRoutedEventArgs e)
        {
            navigatedItems.GoNext();
        }
        #endregion

        #region AutoLoad
        internal void SetAutoLoadsUris(List<Uri> list)
        {
            listAutoLoadedOrdered = (from c in list orderby c.GetPathString() select c.GetPathString()).ToList();
        }

        bool bLoadingAutoLoad;
        void LoadAutoLoadScreens()
        {
            if (listAutoLoadedOrdered.Count() == 0)
                return;

            //Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            //{
            //SetBusy(true);

            //try
            //{
            // navigationControl.BeginInit();
            bLoadingAutoLoad = true;
            try
            {
                var oldOpacity = navigationControl.Opacity;
                navigationControl.Opacity = 0;
                listAutoLoadedOrdered.ForEach(uri =>
                {
                    if (!mapLoaded.ContainsKey(uri))
                    {
                        var control = new ScreenViewer(EditorComponent, new Uri(uri, UriKind.RelativeOrAbsolute), DocumentParent, this, bTest);
                        control.ForceIsLayout(true);

                        try
                        {
                            mapLoaded.Add(uri, control);
                            navigationFrameAdorner.Header = Path.GetFileNameWithoutExtension(uri);
                            //var newTabItem = new TabNavigationItem()
                            //{
                            //    Header = System.IO.Path.GetFileNameWithoutExtension(uri),
                            //    Content = control
                            //};
                            mapLoadedItems.Add(uri, control);
                            AddTabStripItem(Path.GetFileNameWithoutExtension(uri), uri);
                            navigatedItems.Items.Add(new NavigationItem(Path.GetFileNameWithoutExtension(uri), control));

                            if (control is ScreenViewer)
                            {
                                var viewer = control as ScreenViewer;
                                viewer.ForceLoadingNow();
                                //if (!viewer.IsLoaded)
                                //{
                                //    viewer.Loaded += viewer_Loaded;
                                //}
                                //else
                                //    UpdateWindowsPositionFromViewer(viewer);
                            }
                            SubscribeViewerEvents(ref control);

                            if (navigatedItems.SearchAndSelectContent(mapLoadedItems[uri]))
                                WaitForPriority.Wait(DispatcherPriority.Background, control);

                            ListScreens.Add(uri);
                        }
                        finally
                        {
                            control.ForceIsLayout(false);
                        }
                    }
                });
                
                navigatedItems.GoHome(); //navigationControl.Journal.GoHome(null);
                navigationControl.Opacity = oldOpacity;
            }
            finally
            {
                bLoadingAutoLoad = false;
            }

            // navigationControl.EndInit();
            //}
            //finally
            //{
            //    SetBusy(false);
            //}
            //});
        }
        #endregion

        #region Gadgets
        internal void SetGadgetsUris(List<Uri> list)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                LoadGadgetLayout(DocumentParent.Title);

            if (mapGadgetSetting == null)
                mapGadgetSetting = new Dictionary<Uri, GadgetSettings>();

            foreach(var uri in list)
            {
                var gadgetContainer = new GadgetContainer();
                gadgetContainer.Tag = uri;

                var gadget = new GadgetScreen();

                var item = new ScreenViewer(EditorComponent, uri, DocumentParent, this, bTest, true, bIsLayout: true);
                item.ForceLoadingNow();
                gadget.container.Content = item;
                gadgetContainer.Gadget = gadget;
                gadgetContainer.CanClose = item.CanUnload();

                if (mapGadgetSetting.ContainsKey(uri))
                {
                    if (!mapGadgetSetting[uri].visible)
                        continue;

                    Canvas.SetTop(gadgetContainer, mapGadgetSetting[uri].top);
                    Canvas.SetLeft(gadgetContainer, mapGadgetSetting[uri].left);
                }
                else
                {
                    Canvas.SetTop(gadgetContainer, item.Top);
                    Canvas.SetLeft(gadgetContainer, item.Left);
                }
                gadgetContainer.OptionButtonType = OptionButtonTypes.None;
                gadgetContainer.Close += (sender, e) =>
                    {
                        if (item.CanUnload())
                        {
                            if (mapGadgetSetting == null)
                                mapGadgetSetting = new Dictionary<Uri, GadgetSettings>();
                            else if (!mapGadgetSetting.ContainsKey(uri))
                                mapGadgetSetting.Add(uri, new GadgetSettings());
                            mapGadgetSetting[uri].visible = false;

                            RemoveGadget(sender as GadgetContainer);
                        }
                    };

                _SnapCanvas.Children.Add(gadgetContainer);
            }
        }

        void RemoveGadget(GadgetContainer gadgetContainer)
        {
            if (gadgetContainer == null)
                return;
            if (gadgetContainer.Gadget is GadgetScreen)
            {
                var gadget = gadgetContainer.Gadget as GadgetScreen;
                if (gadget.container.Content is ScreenViewer)
                {
                    var viewer = gadget.container.Content as ScreenViewer;
                    gadget.container.Content = null;
                    viewer.Dispose();
                }
            }

            var uri = gadgetContainer.Tag as Uri;
            if (uri != null)
            {
                if (mapGadgetSetting == null)
                    mapGadgetSetting = new Dictionary<Uri, GadgetSettings>();
                else if (!mapGadgetSetting.ContainsKey(uri))
                    mapGadgetSetting.Add(uri, new GadgetSettings());
                mapGadgetSetting[uri].top = Canvas.GetTop(gadgetContainer);
                mapGadgetSetting[uri].left = Canvas.GetLeft(gadgetContainer);
            }
            _SnapCanvas.Children.Remove(gadgetContainer);
        }

        void CleanAllGadgets()
        {
            var list = (from c in _SnapCanvas.Children.OfType<GadgetContainer>() select c).ToList();
            list.ForEach(gadgetContainer => RemoveGadget(gadgetContainer));
        }


        private void OnResetGadgets(object sender, ExecutedRoutedEventArgs e)
        {
            CleanAllGadgets();
            var list = mapGadgetSetting.Keys.ToList();
            mapGadgetSetting.Clear();
            SetGadgetsUris(list);
        }

        private void CanResetGadgets(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mapGadgetSetting != null && mapGadgetSetting.Count > 0;
        }

        #endregion

        #region Layout
        internal void SetLayoutUris(Uri top, Uri left, Uri right, Uri bottom, bool bOrder)
        {
            Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            {
                //SetBusy(true);
                //try
                //{
                    if (bOrder)
                    {
                        layoutLeft = SetLayoutUri(left, DevExpress.Xpf.LayoutControl.Dock.Left);
                        layoutRight = SetLayoutUri(right, DevExpress.Xpf.LayoutControl.Dock.Right);
                        layoutTop = SetLayoutUri(top, DevExpress.Xpf.LayoutControl.Dock.Top);
                        layoutBottom = SetLayoutUri(bottom, DevExpress.Xpf.LayoutControl.Dock.Bottom);
                    }
                    else
                    {
                        layoutTop = SetLayoutUri(top, DevExpress.Xpf.LayoutControl.Dock.Top);
                        layoutBottom = SetLayoutUri(bottom, DevExpress.Xpf.LayoutControl.Dock.Bottom);
                        layoutLeft = SetLayoutUri(left, DevExpress.Xpf.LayoutControl.Dock.Left);
                        layoutRight = SetLayoutUri(right, DevExpress.Xpf.LayoutControl.Dock.Right);
                    }

                    if (layoutTop != null || layoutLeft != null || layoutRight != null || layoutBottom != null)
                    {
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                            LoadLayout(DocumentParent.Title);
                    }

                    ScreenViewer viewer = null;
                    var control = navigatedItems.CurrentItem;
                    if (control != null && control.Content is ScreenViewer)
                        viewer = control.Content as ScreenViewer;

                    if (viewer != null && viewer.Document != null)
                    {
                        ShowHideLayoutScreens(!viewer.Document.HideLayoutScreens);
                        viewer.wnd_SizeChanged(this, EventArgs.Empty);
                    }
                    else
                        ShowHideLayoutScreens(true);

                //}
                //finally
                //{
                //    SetBusy(false);
                //}
            });
        }

        ScreenViewer SetLayoutUri(Uri uri, DevExpress.Xpf.LayoutControl.Dock dock)
        {
            if (uri == null)
                return null;

            var item = new ScreenViewer(EditorComponent, uri, DocumentParent, this, bTest, true, bIsLayout:true);
            item.Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString()));
            item.ForceLoadingNow();
            item.VerticalAlignment = VerticalAlignment.Stretch;
            item.HorizontalAlignment = HorizontalAlignment.Stretch;
            if (dock == DevExpress.Xpf.LayoutControl.Dock.Left ||
                dock == DevExpress.Xpf.LayoutControl.Dock.Right)
                item.Width = item.MainSurface.Width;
            if (dock == DevExpress.Xpf.LayoutControl.Dock.Top ||
                dock == DevExpress.Xpf.LayoutControl.Dock.Bottom)
                item.Height = item.MainSurface.Height;
            dockPanel.Children.Add(item);
            DockLayoutControl.SetDock(item, dock);
            if (item.Document != null)
            {
                DockLayoutControl.SetAllowHorizontalSizing(item, item.Document.ResizeMode != ResizeMode.NoResize);
                DockLayoutControl.SetAllowVerticalSizing(item, item.Document.ResizeMode != ResizeMode.NoResize);
            }

            item.wnd_SizeChanged(this, EventArgs.Empty);
            dockPanel.SizeChanged += (o, e) =>
            {
                item.wnd_SizeChanged(this, EventArgs.Empty);
            };
            dockPanel.MeasureChanged += (o, e) =>
            {
                item.wnd_SizeChanged(this, EventArgs.Empty);
            };

            return item;
        }
        #endregion

        #region AppBars
        internal void SetAppBarsUris(Uri top, Uri bottom, Uri left, Uri right)
        {
            appBarTopBehavior = new TouchHelper();
            appBarTop = SetAppBarsUris(top, DevExpress.Xpf.WindowsUI.AppBarAlignment.Top, appBarTopBehavior);

            appBarLeftBehavior = new TouchHelper();
            appBarLeft = SetAppBarsUris(left, DevExpress.Xpf.WindowsUI.AppBarAlignment.Left, appBarLeftBehavior);

            appBarBottomBehavior = new TouchHelper();
            appBarBottom = SetAppBarsUris(bottom, DevExpress.Xpf.WindowsUI.AppBarAlignment.Bottom, appBarBottomBehavior);

            appBarRightBehavior = new TouchHelper();
            appBarRight = SetAppBarsUris(right, DevExpress.Xpf.WindowsUI.AppBarAlignment.Right, appBarRightBehavior);
        }

        DevExpress.Xpf.WindowsUI.AppBar topBar;
        DevExpress.Xpf.WindowsUI.AppBar bottomBar;
        DevExpress.Xpf.WindowsUI.AppBar leftBar;
        DevExpress.Xpf.WindowsUI.AppBar rightBar;

        internal void OpenTopBar()
        {
            if (topBar == null)
                return;

            topBar.IsOpen = true;
        }

        internal void OpenLeftBar()
        {
            if (leftBar == null)
                return;

            leftBar.IsOpen = true;
        }

        internal void OpenBottomBar()
        {
            if (bottomBar == null)
                return;

            bottomBar.IsOpen = true;
        }

        internal void OpenRightBar()
        {
            if (rightBar == null)
                return;

            rightBar.IsOpen = true;
        }

        ScreenViewer SetAppBarsUris(Uri uri, DevExpress.Xpf.WindowsUI.AppBarAlignment alignment, TouchHelper behavior)
        {
            if (uri == null)
                return null;

            var item = new ScreenViewer(EditorComponent, uri, DocumentParent, this, bTest, true, bIsLayout: true);
            var appBar = new DevExpress.Xpf.WindowsUI.AppBar();
            appBar.Alignment = alignment;
            appBar.ItemSpacing = 0;
            switch (alignment)
            {
                case DevExpress.Xpf.WindowsUI.AppBarAlignment.Top: topBar = appBar; break;
                case DevExpress.Xpf.WindowsUI.AppBarAlignment.Bottom: bottomBar = appBar; break;
                case DevExpress.Xpf.WindowsUI.AppBarAlignment.Left: leftBar = appBar; break;
                case DevExpress.Xpf.WindowsUI.AppBarAlignment.Right: rightBar = appBar; break;
            }
            gridContainer.Children.Add(appBar);

            appBar.HideMode = DevExpress.Xpf.WindowsUI.AppBarHideMode.Default;
            appBar.IsOpen = false;
            appBar.Items.Add(item);

            var behaviors = DevExpress.Mvvm.UI.Interactivity.Interaction.GetBehaviors(appBar);
            behaviors.Add(behavior);

            var bLoaded = false;
            appBar.Opened += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        using (var cursor = new WaitCursor())
                        {
                            item.Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString()));
                            item.ForceLoadingNow();

                            if (alignment == DevExpress.Xpf.WindowsUI.AppBarAlignment.Top ||
                                alignment == DevExpress.Xpf.WindowsUI.AppBarAlignment.Bottom)
                            {
                                var factor = item.Document.Height / item.Document.Width;
                                appBar.Height = item.Document.Height;
                                var bindingWidth = new Binding()
                                {
                                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                    Path = new PropertyPath("ActualWidth")
                                };
                                item.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
                                item.Height = appBar.Height = appBar.ActualWidth * factor;
                                item.wnd_SizeChanged(this, EventArgs.Empty);

                                var dpd = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ActualWidthProperty, typeof(FrameworkElement));
                                dpd.AddValueChanged(appBar, (ob, ev) =>
                                {
                                    item.Height = appBar.Height = appBar.ActualWidth * factor;
                                    item.wnd_SizeChanged(this, EventArgs.Empty);
                                });
                            }
                            else
                            {
                                var factor = item.Document.Width / item.Document.Height;
                                appBar.Width = item.Document.Width;
                                var bindingHeight = new Binding()
                                {
                                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                    Path = new PropertyPath("ActualHeight")
                                };
                                item.SetBinding(FrameworkElement.HeightProperty, bindingHeight);
                                
                                item.Width = appBar.Width = appBar.ActualHeight * factor;
                                item.wnd_SizeChanged(this, EventArgs.Empty);
                                var dpd = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ActualHeightProperty, typeof(FrameworkElement));
                                dpd.AddValueChanged(appBar, (ob, ev) =>
                                {
                                    item.Width = appBar.Width = appBar.ActualHeight * factor;
                                    item.wnd_SizeChanged(this, EventArgs.Empty);
                                });
                            }
                        }
                    }
                    else
                        item.wnd_SizeChanged(this, EventArgs.Empty);
                };

            return item;
        }
        #endregion

        #region Isolated Storage

        static String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.LayoutViewer.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

        static String GetGadgetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.Gadget.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
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

        void SaveLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.Create, isoStorage))
                {
                    var settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        dockPanel.WriteToXML(writer);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void SaveGadgetLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (mapGadgetSetting == null || null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetGadgetStoreFileName(title), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(Dictionary<Uri, GadgetSettings>));
                        serializer.WriteObject(writer, mapGadgetSetting);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void LoadLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (var reader = XmlReader.Create(stream, settings))
                    {
                        dockPanel.ReadFromXML(reader);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void LoadGadgetLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetGadgetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (XmlReader reader = XmlReader.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(Dictionary<Uri, GadgetSettings>));
                        mapGadgetSetting = serializer.ReadObject(reader) as Dictionary<Uri, GadgetSettings>;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}