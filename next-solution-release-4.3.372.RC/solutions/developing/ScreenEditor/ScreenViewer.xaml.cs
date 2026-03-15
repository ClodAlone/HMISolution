using System;
using System.Linq;
using System.ComponentModel;
#if !WINDOWS_UWP
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using _3DTools;
using ScreenManager.Adorners;
using Utilities.ProgressDialog;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using log4net;
using DevExpress.Xpf.Printing;
using VFS;
using UnitConverterManager.ComponentService;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
#endif
using DocumentManager.ComponentService;
using Opc.Ua;
using OPCUAViewModel;
using ScreenManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.Animations;
using Utilities.WPF;
using ScreenSettings;
using System.Collections.Generic;
using System.Dynamic;
using ScreenSettings.Entities;
using System.IO.IsolatedStorage;
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Runtime.Serialization;
using WPFUtilities;
using System.Diagnostics;
using StringManager.ComponentService;
using System.Threading.Tasks;
using System.Threading;
using DocumentManager.ComponentService.Helpers;
using System.Runtime.InteropServices;
using UFProjectManager.ComponentService;
using log4net.Core;
using System.IdentityModel.Selectors;


namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for ScreenViewer.xaml
    /// </summary>
    public partial class ScreenViewer : UserControl, INotifyPropertyChanged, IDisposable
    {
        
        #region Members
#if !WINDOWS_UWP
        MessageAdorner _gestureResultAdorner;
#endif
        readonly ScreenManagerComponent ScreenComponent;
        readonly Uri Uri;
        readonly IDocument DocumentParent;
        IUFProjectManager iUFProjectManager;
        readonly UserControl ActiveView;
#if !WINDOWS_UWP
        readonly Dictionary<UIElement, ManipulationAdorner> mapManAdorners = new Dictionary<UIElement, ManipulationAdorner>();
        readonly Dictionary<String, Trackball> mapElementCamera = new Dictionary<String, Trackball>();
        readonly Dictionary<Viewport3D, Trackball> mapElementTrackball = new Dictionary<Viewport3D, Trackball>();
        List<String> listManipulationEntities;
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        private static readonly ILog logUsers = LogManager.GetLogger(Properties.Resources.UsersManager);
        private static Dictionary<byte, byte> monitorNumberMap;
#else
        ScrollViewer Scroller;
        Canvas MainSurface;
#endif

        List<PropertyChangeNotifier> listPropertyChangeNotifier;

        bool bRunningOnServerCtor;
        IStringEditorManager StringEditor;
#if !WINDOWS_UWP
        IUnitConverterEditorManager UnitConverterEditor;
#endif

#if !WINDOWS_UWP
#if DEBUG
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.ScreenManager);
#endif
#endif
        //Storyboard sbmouseOver;
        //Storyboard sbmouseLeave;
        List<String> listAccessEntities;
        List<String> listAccessLevelEntities;
        List<FrameworkElement> listConnectionString;
        Dictionary<UIElement, String> mapAccessLevelEntities;
        List<String> listZoomVisibilityEntities;
        Dictionary<UIElement, CacheMode> mapElementCacheMode;
        bool IsModal;
        readonly bool IsTile;
        bool IsLayout;
        readonly String ParameterFile;
        readonly int nRequestedMonitor;
        Window wndParent;
        DispatcherTimer timerDiscoverWindow;
        DispatcherTimer timerRefreshCommands;
        bool bIsRelative;
        bool bIsCenterOwner;
        bool isTopBottomBar = false;
        bool isLeftRightBar = false;
        #endregion Members

        #region Attached Properties

#if !WINDOWS_UWP
        public static readonly DependencyProperty IsInTestProperty = DependencyProperty.RegisterAttached("IsInTest", typeof(bool), typeof(ScreenViewer), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior));

        public static void SetIsInTest(UIElement element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            element.SetValue(IsInTestProperty, value);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static bool GetIsInTest(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return (bool)(element.GetValue(IsInTestProperty));
        }
#endif
        #endregion

        #region Constructors

        public ScreenViewer(ScreenManagerComponent screenComponent, Uri uri,
            IDocument parent, UserControl activeView, bool test = false, bool isModal = false,
            bool isTile = false, String parameterFile = null, bool bIsLayout = false, int nMonitor = -1,
            double customtop = Double.NaN, double customleft = Double.NaN, int autocloseSeconds = 0,
            bool bRunningOnServer = false, double customWidth = Double.NaN, double customHeight = Double.NaN, bool bIsrelative = false, bool bIscenterOwner = false,
            DevExpress.Xpf.LayoutControl.Dock dockType = DevExpress.Xpf.LayoutControl.Dock.None)
        {
#if !WINDOWS_UWP
            InitializeComponent();
#else
            Scroller = new ScrollViewer()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            MainSurface = new Canvas();
            Scroller.Content = MainSurface;
            Content = Scroller;
#endif
            ScreenComponent = screenComponent;
            Uri = uri;
            DocumentParent = parent;
            ActiveView = activeView;
            if (dockType == DevExpress.Xpf.LayoutControl.Dock.Top || dockType == DevExpress.Xpf.LayoutControl.Dock.Bottom)
                isTopBottomBar = true;
            if (dockType == DevExpress.Xpf.LayoutControl.Dock.Left || dockType == DevExpress.Xpf.LayoutControl.Dock.Right)
                isLeftRightBar = true;
#if !WINDOWS_UWP
            bTest = test;
#endif
            IsModal = isModal;
            IsTile = isTile;
            IsLayout = bIsLayout;
            ParameterFile = parameterFile;
            nRequestedMonitor = nMonitor;
#if !WINDOWS_UWP
            customTop = customtop;
            customLeft = customleft;
            autoCloseSeconds = autocloseSeconds;
            bIsRelative = bIsrelative;
            bIsCenterOwner = bIscenterOwner;
#endif

            Document = ScreenDocument.FromFile(Uri.GetPathString(), DocumentParent, true);
            iUFProjectManager = parent.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            LoadScreenStyle();

            bRunningOnServerCtor = bRunningOnServer;
            if (Document != null)
            {
                if (!bRunningOnServerCtor)
                    Document.ActiveView = ActiveView ?? this;
                else
                {
                    Document.SetIsBlindServer();
                    Parallel.ForEach(Document.MapScreenEntities.Values, c => c.SetIsBlindServer());
                }
                Document.View = this;
                Document.Parent = DocumentParent;
                Document.CommandExecuting += Document_CommandExecuting;
                if (!Double.IsNaN(customWidth))
                    Document.Width = customWidth;
                if (!Double.IsNaN(customHeight))
                    Document.Height = customHeight;

#if !WINDOWS_UWP
                var index = SysInfo.GetIndexPerformance();
                if (Properties.Settings.Default.ForceLowPerformance ||
                    !Double.IsNaN(index) && index < Properties.Settings.Default.LowPerformanceIndex)
#endif
                    ScreenSettings.ScreenDocument.SetRunningOnSlowPC(MainSurface, true);

                StringEditor = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
#if !WINDOWS_UWP
                UnitConverterEditor = Document.GetService(typeof(IUnitConverterEditorManager)) as IUnitConverterEditorManager;
#endif
            }

#if !WINDOWS_UWP
            Unloaded += (o, e) =>
                {
                    CloseAllPopup();

                    Touch3ID = 0;
                    Touch2ID = 0;
                    Touch1ID = 0;

                    previewTouch1ID = 0;
                    previewTouch2ID = 0;
                };
#else
            ScreenViewerControl_Loaded(null, null);
#endif
        }

        #endregion Constructors

        bool bLoaded;
        bool bInitialized;
#if !WINDOWS_UWP
        bool bTest;
#endif
        bool bUserEnabled = false;

#if !WINDOWS_UWP
        internal void ForceLoadingNow()
        {
            ScreenViewerControl_Loaded(null, null);
        }

        internal void ForceActivatingNow()
        {
            if (bDisposed || !bLoaded)
                return;

            if (wndParent == null && !IsLayout)
            {
                var wnd = this.FindParent<Window>();
                if (wnd != null)
                {
                    wndParent = wnd;
                    wnd.Activated += wnd_Activated;
                    wnd.Deactivated += wnd_Deactivated;
                    wnd.SizeChanged += wnd_SizeChanged;
                    wnd_SizeChanged(this, EventArgs.Empty);
                }
            }

            if (wndParent == null || wndParent.IsActive)
                wnd_Activated(null, null);
        }

        internal void ForceDeactivatingNow()
        {
            if (bDisposed || !bLoaded)
                return;

            if (wndParent != null)
            {
                wndParent.Activated -= wnd_Activated;
                wndParent.Deactivated -= wnd_Deactivated;
                wndParent.SizeChanged -= wnd_SizeChanged;
                wndParent = null;
            }

            wnd_Deactivated(null, null);
        }
#endif
        private void ScreenViewerControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (bLoaded)
                return;

            bLoaded = true;
            
#if !WINDOWS_UWP
#if !DEBUG
            bool log3dMessage = false;
            var enable3D = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx+n8+5uFpK3i1pkn2kvC+hQ=="/* G3D */);
            //if (enable3D == false)
            //{
            //    logLicense.Warn(Properties.Resources.No3DLicense);
            //}
#endif
#if DEBUG
            var text = String.Format("Loading Screen {0} took", Uri.GetPathString()) + " : {0}";
            using (var stopwatcher = new StopWatcher(text))
#endif
#endif
            {
                try
                {
#if !WINDOWS_UWP
                    MainSurface.BeginInit();
#endif
                    try
                    {
                        //sbmouseOver = FindResource("sbMouseOver") as Storyboard;
                        //sbmouseLeave = FindResource("sbMouseLeave") as Storyboard;

                        if (Document == null)
                        {
                            ScreenComponent.UIInterface.ShowError(String.Format(Properties.Resources.CouldNotLocateUri, Uri.GetPathString()));
                            return;
                        }
                        Document.InRuntime = true;
#if !WINDOWS_UWP
                        Document.InTest = bTest;
#endif
                        Document.SetParameterFile(ParameterFile);

#if !WINDOWS_UWP
                        if (MonitorNumber > 0 &&
                            MonitorNumber <= System.Windows.Forms.SystemInformation.MonitorCount)
                            IsModal = true;

                        if (IsModal || !Document.ToolbarVisible)
                            toolbar.Visibility = Visibility.Collapsed;
                        else if (ThemeImageHelper.NeedsDarkBitmapResources(Document))
                            toolbar.Background = new SolidColorBrush(Colors.WhiteSmoke);
                        else
                            toolbar.Background = new SolidColorBrush(Colors.DarkSlateGray);
                        ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
#endif
                        ScreenDocument.SetScreenDocument(MainSurface, Document);
#if !WINDOWS_UWP
                        ScreenViewer.SetIsInTest(MainSurface, bTest);

                        if (Document.LayoutView)
                            NameScope.SetNameScope(layoutItems, new NameScope());
                        else
                            NameScope.SetNameScope(MainSurface, new NameScope());

                        Document.GetTagList += (o, ev) =>
                        {
                            ev.list = ScreenComponent.UFUAEditor.GetFlatListTags(Document, inExecution: true);
                        };
                        Document.GetPrototypeList += (o, ev) =>
                        {
                            ev.mapDefinitions = ScreenComponent.UFUAEditor.GetListPrototypesDesc(Document);
                            ev.mapPrototypes = ScreenComponent.UFUAEditor.GetFlatListPrototypeInstances(Document, inExecution: true);
                        };
                        Document.GetChildsTagList += (o, ev) =>
                        {
                            ev.map = ScreenComponent.UFUAEditor.GetFlatListChildTags(Document);
                        };
                        Document.GetChildsPrototypeList += (o, ev) =>
                        {
                            ev.mapChildsDefinitions = ScreenComponent.UFUAEditor.GetListChildsPrototypesDesc(Document);
                            ev.mapChildsPrototypes = ScreenComponent.UFUAEditor.GetFlatListChildsPrototypeInstances(Document);
                        };
                        Document.GetTagEntityReference += (o, ev) =>
                        {
                            var erString = ScreenComponent.UFUAEditor.GetTagEntityReference(Document, ev.Name, ev.Instance, inExecution: true, Project: ev.ChildProject);
                            if (!String.IsNullOrEmpty(erString))
                                ev.entityReference = erString.FromXml<OPCUAEntityReference>();
                        };
#endif
                        if (!bRunningOnServerCtor)
                            Document.ActiveView = ActiveView ?? this;
                        Document.View = this;
                        Document.Parent = DocumentParent;

#if !WINDOWS_UWP
                        if (bTest)
                        {
                            DataContext = Document.dataContextExpando;
                            var dataValueHistory = new SafeObservableCollection<DataValue>();
                            Document.dataContextExpando.DataValueHistory = dataValueHistory;
                            ((INotifyPropertyChanged)Document.dataContextExpando).PropertyChanged += (ob, ev) =>
                            {
                                if (ev.PropertyName == "Value")
                                {
                                    dataValueHistory.Add(new DataValue(new Variant(Document.dataContextExpando.Value),
                                                                       new StatusCode(StatusCodes.Good),
                                                                       DateTime.Now));
                                }
                            };
                        }
#endif

                        if (
#if !WINDOWS_UWP
                            !Document.LayoutView &&
#endif
                            Document.FitInWindow)
                        {
                            if (IsLoaded)
                            {
#if !WINDOWS_UWP
                                Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
#else
                                RunOnUIThread.Run(() =>
#endif
                                OnFillMode(null, null));
                            }
                            else
                            {
                                Loaded += (s, ev) =>
                                {
                                    if (IsInFillMode)
                                        return;

                                    OnFillMode(null, null);
                                };
                            }
                        }
                        Surface = Document.GetCurrentXamlDocument();

                        if (Document.HideScrollBars)
                        {
                            Scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
#if !WINDOWS_UWP
                            var gestureResultAdorner = GestureResultAdorner;
                            if (gestureResultAdorner != null)
                                gestureResultAdorner.ShowMessage(ex.Message, Mouse.GetPosition(MainSurface));
                            else
#endif
                                //MessageBox.Show(ex.Message, Document.Title, MessageBoxButton.OK,
                                //            MessageBoxImage.Error, MessageBoxResult.OK,
                                //            MessageBoxOptions.ServiceNotification);
                                ScreenComponent.UIInterface.ShowError(ex.Message);
                        }
                        catch (Exception exe)
                        {
                            //MessageBox.Show(ex.Message, Document.Title, MessageBoxButton.OK,
                            //            MessageBoxImage.Error, MessageBoxResult.OK,
                            //            MessageBoxOptions.ServiceNotification);
                            ScreenComponent.UIInterface.ShowError(ex.Message);
                        }
                    }

                    if (Document == null)
                        return;

#if !WINDOWS_UWP
                    bool bLoadingSymbols = false;
                    Document.RepositoryItemsLoading += (ob, ev) =>
                    {
                        bLoadingSymbols = true;
                        Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                        {
                            progressLoading.Visibility = Visibility.Visible;
                            progressLoading.IsIndeterminate = true;
                        });
                    };
                    Document.RepositoryItemsLoaded += (ob, ev) =>
                    {
                        bLoadingSymbols = false;
                        Document.OnScreenLoaded();
                        Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                        {
                            progressLoading.IsIndeterminate = false;
                            progressLoading.Visibility = Visibility.Collapsed;

                            if (Document != null)
                            {
                                PrepareAccessLevelEntities();
                                if (bUserEnabled)
                                {
                                    SetVisibleItemsOnServer();
                                    SetVisibileItems(currentAccessMask);
                                    SubscribeAccessLevelControls();
                                }

                                SetZoomVisibilityItemsBackground(false);
                            }

#if !WINDOWS_UWP
                            if (!bTest)
                                LoadContextMenus();
#endif
                        });

                        var listTooltipWhenDisabled = (from c in Document.MapScreenEntities// .AsParallel()
                                                    where c.Value.ShowTooltipWhenDisabled && c.Value.Element != null
                                                    select c.Key).ToList();
                        listTooltipWhenDisabled.ForEach(key =>
                        {
                            ToolTipService.SetShowOnDisabled(Document.MapScreenEntities[key].Element, true);
                        });
                    };

                    if (String.IsNullOrEmpty(ParameterFile))
                        LoadData(GetUniqueTitle(Document));

                    var mainControl = Document.LayoutView ? layoutItems as FrameworkElement : MainSurface as FrameworkElement;
#else
                    var mainControl = MainSurface as FrameworkElement;
#endif
                    Document.LoadResources(mainControl);
                    Document.RefreshEntityStyleBinding(mainControl);

                    if (listConnectionString.Count > 0)
                        Document.PostBindConnectionStringSource(listConnectionString);
                    listConnectionString.Clear();

                    if (StringEditor != null)
                    {
#if !WINDOWS_UWP
                        FrameworkElement mainCntrl = Document.LayoutView ? layoutItems as FrameworkElement : MainSurface as FrameworkElement;
#else
                        FrameworkElement mainCntrl = MainSurface as FrameworkElement;
#endif
                        ChangeLanguage(StringEditor.GetActiveCulture(Document), mainCntrl);
                        StringEditor.CultureChanged += StringEditor_CultureChanged;
                    }

                    Document.PrepareExecution(MainSurface, Document.SessionString,
                        Document.MapScreenEntities.Keys.ToList());

                    if (bRunningOnServerCtor)
                    {
                        var listNotVisibleOnWebClient = (from c in Document.MapScreenEntities// .AsParallel()
                                                       where !c.Value.VisibleOnClient && c.Value.Element != null
                                                       select c.Key).ToList();
                        listNotVisibleOnWebClient.ForEach(key => Document.MapScreenEntities[key].Element.Visibility = Visibility.Collapsed);
                    }

                    if (StringEditor != null)
                    {
                        var activeLanguage = StringEditor.GetActiveCulture(Document);
                        var itemSourceItems = (from c in Document.MapScreenEntities.Values where c.IsItemSourceEntity() select c).ToList();
                        itemSourceItems.ForEach(item =>
                        {
                            if (item.Element is FrameworkElement)
                                ChangeLanguage(activeLanguage, item.Element as FrameworkElement);
                        });
                    }

#if !WINDOWS_UWP
                    Document.UpdateRepositoryItems(this, mainControl);
                    Document.ExecuteScriptCode();

                    // Document.PrepareExecution(MainSurface, ScreenDocument.UISessionString);

                    if (!Document.LayoutView)
                    {
                        //Action action5 = () =>
                        //    {
                        listManipulationEntities = Document.GetEnableManipulationList();
                        listManipulationEntities.ForEach(el =>
                                {
                                    var child = Document.FindInnerControl(MainSurface, el);
                                    if (child != null)
                                    {
                                        var menu = TryFindResource("SharedContextMenu") as ContextMenu;
                                        if (child.ContextMenu != null)
                                        {
                                            var menuchild = new MenuItem() { Header = Properties.Resources.ManipulationMenu };

                                            while (menu.Items.Count > 0)
                                            {
                                                var item = menu.Items[0];
                                                menu.Items.Remove(item);
                                                menuchild.Items.Add(item);
                                            }

                                            child.ContextMenu.Items.Add(menuchild);
                                        }
                                        else
                                            child.ContextMenu = menu;

                                        child.IsManipulationEnabled = false;
                                        //AdornerLayer adorner = AdornerLayer.GetAdornerLayer(child);
                                        //if (adorner == null)
                                        //    adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                                        //if (adorner != null)
                                        //{
                                        //    var nodeadorner = new ManipulationAdorner(child);
                                        //    mapManAdorners.Add(child, nodeadorner);
                                        //    adorner.Add(nodeadorner);
                                        //}
                                    }
                                });

                        Document.GetEnableMouseOverList().ForEach(el =>
                                {
                                    var child = Document.FindInnerControl(MainSurface, el);
                                    if (child != null)
                                    {
                                        AddMouseOverToControl(child);
                                    }
                                });
                        //    };
                        //Dispatcher.BeginInvoke(action5, DispatcherPriority.DataBind);
                    }
                    else
                    {
                        LoadLayout(GetUniqueTitle(Document));
                    }

                    // Document.Mediator.NotifyColleagues<ScreenViewer>("ScreenViewerLoaded", this);

                    var Load3dScreenInvoker = new DelayedSingleActionInvoker(() =>
                    {
                        Load3DScreens();
                    }, TimeSpan.FromSeconds(2));
                    Load3dScreenInvoker.BeginInvoke();

                    //Action action4 = () =>
                    //    {
                    var listCameraEntities = (from c in Document.MapScreenEntities// .AsParallel()
                                              where c.Value.ListCameraTransforms != null && c.Value.ListCameraTransforms.Count > 0
                                              select c.Key).ToList();
                    var listViewPort3Ds = new List<Viewport3D>();
                    listCameraEntities.ForEach(name =>
                        {
                            var child = Document.FindInnerControl(MainSurface, name);
                            if (child != null)
                            {
                                var list3Ds = child.GetChildrenOfType<Viewport3D>().ToList();
                                if (child is Viewport3D)
                                    list3Ds.Insert(0, child as Viewport3D);
                                if (list3Ds.Count > 0)
                                {
                                    var viewport3D = list3Ds.First();
                                    if (listViewPort3Ds.Contains(viewport3D))
                                        return;
#if !DEBUG
                                    if (!enable3D)
                                    {
                                        viewport3D.Visibility = Visibility.Collapsed;
                                        Document.MapScreenEntities.Remove(name);
                                        if (!log3dMessage)
                                        {
                                            log3dMessage = true;
                                            //logLicense.Warn(Properties.Resources.License3DNotAvailable);
                                            if(iUFProjectManager != null)
                                                iUFProjectManager.AddLogEntity(DocumentParent, Properties.Resources.LicenseManager, 
                                                DateTime.UtcNow, Properties.Resources.License3DNotAvailable, 
                                                System.Diagnostics.EventLogEntryType.Warning);
                                    }
                                    }
                                    else
#endif
                                    {
                                        listViewPort3Ds.Add(viewport3D);
                                        viewport3D.IsManipulationEnabled = true;
                                        var trackball = new Trackball { EventSource = viewport3D };

                                        if (Document.MapScreenEntities[name].ListCameraTransforms != null)
                                        {
                                            trackball.ResetToInitialPosition();
                                        }

                                        viewport3D.Camera.Transform = trackball.Transform;
                                        if (LayoutHelper.HasTouchInput())
                                            trackball.SetTouchDevice(true);
                                        if (!mapElementCamera.ContainsKey(name))
                                            mapElementCamera.Add(name, trackball);
                                        if (!mapElementTrackball.ContainsKey(viewport3D))
                                            mapElementTrackball.Add(viewport3D, trackball);
                                        viewport3D.PreviewTouchUp += viewport3D_TouchUp;
                                        viewport3D.PreviewTouchDown += viewport3D_TouchDown;
                                        trackball.MaxRotationAngleX = Document.MapScreenEntities[name].Max3DRotationAngleX;
                                        trackball.MaxRotationAngleY = Document.MapScreenEntities[name].Max3DRotationAngleY;
                                        trackball.MaxRotationAngleZ = Document.MapScreenEntities[name].Max3DRotationAngleZ;
                                        trackball.MinRotationAngleX = Document.MapScreenEntities[name].Min3DRotationAngleX;
                                        trackball.MinRotationAngleY = Document.MapScreenEntities[name].Min3DRotationAngleY;
                                        trackball.MinRotationAngleZ = Document.MapScreenEntities[name].Min3DRotationAngleZ;
                                        trackball.MaxTranslateOffsetX = Document.MapScreenEntities[name].Max3DTranslateOffsetX;
                                        trackball.MinTranslateOffsetX = Document.MapScreenEntities[name].Min3DTranslateOffsetX;
                                        trackball.MaxTranslateOffsetY = Document.MapScreenEntities[name].Max3DTranslateOffsetY;
                                        trackball.MinTranslateOffsetY = Document.MapScreenEntities[name].Min3DTranslateOffsetY;
                                        trackball.MaxZoom = Document.MapScreenEntities[name].Max3DZoom;
                                        trackball.MinZoom = Document.MapScreenEntities[name].Min3DZoom;
                                        if (Document.MapScreenEntities[name].Has3DZoomTag)
                                        {
                                            Document.MapScreenEntities[name].Update3DZoomTag(trackball.ScaleValue);
                                            trackball.ZoomChanged += (ob, ev) =>
                                                {
                                                    Document.MapScreenEntities[name].Update3DZoomTag(trackball.ScaleValue);
                                                };
                                            Document.MapScreenEntities[name]._3DZoomTagChanged += (ob, ev) =>
                                            {
                                                trackball.ScaleValue = Document.MapScreenEntities[name].GetLast3DZoomTagChanged();
                                            };
                                        }

                                        var menu = new ContextMenu() { Tag = Properties.Resources._3DViews };
                                        var listMenus = new List<MenuItem>();
                                        Document.MapScreenEntities[name].ListCameraTransforms.ForEach(item =>
                                            {
                                                var menuItem = new MenuItem()
                                                {
                                                    Header = item.Name
                                                };
                                                listMenus.Add(menuItem);
                                                menuItem.Click += (o, ev) =>
                                                    {
                                                        
                                                        trackball.Animate(item.Scale, item.Rotation, item.TranslateTransform2D,
                                                            item.AnimationTime, new SineEase() { EasingMode = EasingMode.EaseOut });
                                                        menuItem.IsChecked = true;
                                                        listMenus.ForEach(menutouncheck =>
                                                            {
                                                                if (menutouncheck != menuItem)
                                                                    menutouncheck.IsChecked = false;
                                                            });
                                                    };
                                                menu.Items.Add(menuItem);
                                            });
                                        
                                        if (viewport3D.ContextMenu != null && 
                                            viewport3D.ContextMenu.Tag as String != Properties.Resources._3DViews)
                                        {
                                            var menuchild = new MenuItem() { Header = Properties.Resources._3DViews };

                                            while (menu.Items.Count > 0)
                                            {
                                                var item = menu.Items[0];
                                                menu.Items.Remove(item);
                                                menuchild.Items.Add(item);
                                            }

                                            viewport3D.ContextMenu.Items.Add(menuchild);
                                        }
                                        else
                                            viewport3D.ContextMenu = menu;
                                    }
                                }
                            }
                        });
#endif
                    var listChildren = new List<UIElement>();
                    if (Document.ScanChildrenElements)
                        listChildren = mainControl.GetChildrenOfType<UIElement>().ToList();

#if !WINDOWS_UWP
                    var list = (from c in listChildren.OfType<Viewport3D>() select c).ToList();
                    foreach (var el in list)
                    {
                        if (listViewPort3Ds.Contains(el))
                            continue;
#if !DEBUG
                        if (!enable3D)
                        {
                            el.Visibility = Visibility.Collapsed;
                            if (!log3dMessage)
                            {
                                log3dMessage = true;
                                //logLicense.Warn(Properties.Resources.License3DNotAvailable);
                                if (iUFProjectManager != null)
                                    iUFProjectManager.AddLogEntity(DocumentParent, Properties.Resources.LicenseManager, 
                                    DateTime.UtcNow, Properties.Resources.License3DNotAvailable, 
                                    System.Diagnostics.EventLogEntryType.Warning);
                            }
                        }
                        else
#endif
                        {
                            el.IsManipulationEnabled = true;
                            var trackball = new Trackball { EventSource = el };

                            if (Document.MapScreenEntities.ContainsKey(el.Name) &&
                                Document.MapScreenEntities[el.Name].ListCameraTransforms != null)
                            {
                                trackball.ResetToInitialPosition();
                            }
                            else
                            {
                                trackball.GetCameraPositionFromScreen();
                            }

                            el.Camera.Transform = trackball.Transform;
                            if (LayoutHelper.HasTouchInput())
                                trackball.SetTouchDevice(true);
                            if (!mapElementTrackball.ContainsKey(el))
                                mapElementTrackball.Add(el, trackball);
                            el.PreviewTouchUp += viewport3D_TouchUp;
                            el.PreviewTouchDown += viewport3D_TouchDown;
                            if (Document.MapScreenEntities.ContainsKey(el.Name))
                            {
                                trackball.MaxRotationAngleX = Document.MapScreenEntities[el.Name].Max3DRotationAngleX;
                                trackball.MaxRotationAngleY = Document.MapScreenEntities[el.Name].Max3DRotationAngleY;
                                trackball.MaxRotationAngleZ = Document.MapScreenEntities[el.Name].Max3DRotationAngleZ;
                                trackball.MinRotationAngleX = Document.MapScreenEntities[el.Name].Min3DRotationAngleX;
                                trackball.MinRotationAngleY = Document.MapScreenEntities[el.Name].Min3DRotationAngleY;
                                trackball.MinRotationAngleZ = Document.MapScreenEntities[el.Name].Min3DRotationAngleZ;
                                trackball.MaxTranslateOffsetX = Document.MapScreenEntities[el.Name].Max3DTranslateOffsetX;
                                trackball.MinTranslateOffsetX = Document.MapScreenEntities[el.Name].Min3DTranslateOffsetX;
                                trackball.MaxTranslateOffsetY = Document.MapScreenEntities[el.Name].Max3DTranslateOffsetY;
                                trackball.MinTranslateOffsetY = Document.MapScreenEntities[el.Name].Min3DTranslateOffsetY;
                                trackball.MaxZoom = Document.MapScreenEntities[el.Name].Max3DZoom;
                                trackball.MinZoom = Document.MapScreenEntities[el.Name].Min3DZoom;
                                if (Document.MapScreenEntities[el.Name].Has3DZoomTag)
                                {
                                    Document.MapScreenEntities[el.Name].Update3DZoomTag(trackball.ScaleValue);
                                    trackball.ZoomChanged += (ob, ev) =>
                                    {
                                        Document.MapScreenEntities[el.Name].Update3DZoomTag(trackball.ScaleValue);
                                    };
                                    Document.MapScreenEntities[el.Name]._3DZoomTagChanged += (ob, ev) =>
                                        {
                                            trackball.ScaleValue = Document.MapScreenEntities[el.Name].GetLast3DZoomTagChanged();
                                        };
                                }
                            }
                        }
                    }

                    Document.RepositoryItemLoaded += (ob, ev) =>
                        {
                            Load3dScreenInvoker.BeginInvoke();

                            var element = ob as FrameworkElement;
                            if (element != null)
                            {
                                if (Document.MapScreenEntities.ContainsKey(element.Name) &&
                                    Document.MapScreenEntities[element.Name].EnableManipulation)
                                {
                                    var menu = TryFindResource("SharedContextMenu") as ContextMenu;
                                    if (element.ContextMenu != null)
                                    {
                                        var menuchild = new MenuItem() { Header = Properties.Resources.ManipulationMenu };

                                        while (menu.Items.Count > 0)
                                        {
                                            var item = menu.Items[0];
                                            menu.Items.Remove(item);
                                            menuchild.Items.Add(item);
                                        }

                                        element.ContextMenu.Items.Add(menuchild);
                                    }
                                    else
                                        element.ContextMenu = menu;

                                    // element.IsManipulationEnabled = true;
                                }

                                var list3ds = element.GetChildrenOfType<Viewport3D>();
                                foreach (var el in list3ds)
                                {
                                    if (listViewPort3Ds.Contains(el))
                                        continue;
#if !DEBUG
                                    if (!enable3D)
                                    {
                                        el.Visibility = Visibility.Collapsed;
                                        Document.MapScreenEntities.Remove(element.Name);
                                        if (!log3dMessage)
                                        {
                                            log3dMessage = true;
                                            //logLicense.Warn(Properties.Resources.License3DNotAvailable);
                                            if(iUFProjectManager != null)
                                                iUFProjectManager.AddLogEntity(DocumentParent, Properties.Resources.LicenseManager, 
                                                DateTime.UtcNow, Properties.Resources.License3DNotAvailable, 
                                                System.Diagnostics.EventLogEntryType.Warning);
                                        }
                                    }
                                    else
#endif
                                    {
                                        listViewPort3Ds.Add(el);
                                        el.IsManipulationEnabled = true;
                                        var trackball = new Trackball { EventSource = el };
                                        el.Camera.Transform = trackball.Transform;
                                        if (LayoutHelper.HasTouchInput())
                                            trackball.SetTouchDevice(true);
                                        if (!mapElementTrackball.ContainsKey(el))
                                            mapElementTrackball.Add(el, trackball);
                                        el.PreviewTouchUp += viewport3D_TouchUp;
                                        el.PreviewTouchDown += viewport3D_TouchDown;
                                        if (Document.MapScreenEntities.ContainsKey(element.Name))
                                        {
                                            trackball.MaxRotationAngleX = Document.MapScreenEntities[element.Name].Max3DRotationAngleX;
                                            trackball.MaxRotationAngleY = Document.MapScreenEntities[element.Name].Max3DRotationAngleY;
                                            trackball.MaxRotationAngleZ = Document.MapScreenEntities[element.Name].Max3DRotationAngleZ;
                                            trackball.MinRotationAngleX = Document.MapScreenEntities[element.Name].Min3DRotationAngleX;
                                            trackball.MinRotationAngleY = Document.MapScreenEntities[element.Name].Min3DRotationAngleY;
                                            trackball.MinRotationAngleZ = Document.MapScreenEntities[element.Name].Min3DRotationAngleZ;
                                            trackball.MaxTranslateOffsetX = Document.MapScreenEntities[element.Name].Max3DTranslateOffsetX;
                                            trackball.MinTranslateOffsetX = Document.MapScreenEntities[element.Name].Min3DTranslateOffsetX;
                                            trackball.MaxTranslateOffsetY = Document.MapScreenEntities[element.Name].Max3DTranslateOffsetY;
                                            trackball.MinTranslateOffsetY = Document.MapScreenEntities[element.Name].Min3DTranslateOffsetY;
                                            trackball.MaxZoom = Document.MapScreenEntities[element.Name].Max3DZoom;
                                            trackball.MinZoom = Document.MapScreenEntities[element.Name].Min3DZoom;
                                            if (Document.MapScreenEntities[element.Name].Has3DZoomTag)
                                            {
                                                Document.MapScreenEntities[element.Name].Update3DZoomTag(trackball.ScaleValue);
                                                trackball.ZoomChanged += (ob1, ev1) =>
                                                {
                                                    Document.MapScreenEntities[element.Name].Update3DZoomTag(trackball.ScaleValue);
                                                };
                                                Document.MapScreenEntities[element.Name]._3DZoomTagChanged += (ob1, ev1) =>
                                                {
                                                    trackball.ScaleValue = Document.MapScreenEntities[element.Name].GetLast3DZoomTagChanged();
                                                };
                                            }
                                        }

                                        if (Document.MapScreenEntities.ContainsKey(element.Name) &&
                                            Document.MapScreenEntities[element.Name].ListCameraTransforms != null &&
                                            Document.MapScreenEntities[element.Name].ListCameraTransforms.Count > 0)
                                        {
                                            var menu = new ContextMenu() { Tag = Properties.Resources._3DViews };
                                            var listMenus = new List<MenuItem>();
                                            Document.MapScreenEntities[element.Name].ListCameraTransforms.ForEach(item =>
                                            {
                                                var menuItem = new MenuItem()
                                                {
                                                    Header = item.Name
                                                };
                                                listMenus.Add(menuItem);
                                                menuItem.Click += (obj, eve) =>
                                                {
                                                    trackball.Animate(item.Scale, item.Rotation, item.TranslateTransform2D,
                                                        item.AnimationTime, new SineEase() { EasingMode = EasingMode.EaseOut });
                                                    menuItem.IsChecked = true;
                                                    listMenus.ForEach(menutouncheck =>
                                                    {
                                                        if (menutouncheck != menuItem)
                                                            menutouncheck.IsChecked = false;
                                                    });
                                                };
                                                menu.Items.Add(menuItem);
                                            });
                                            
                                            if (element.ContextMenu != null && 
                                                element.ContextMenu.Tag as String != Properties.Resources._3DViews)
                                            {
                                                var menuchild = new MenuItem() { Header = Properties.Resources._3DViews };

                                                while (menu.Items.Count > 0)
                                                {
                                                    var item = menu.Items[0];
                                                    menu.Items.Remove(item);
                                                    menuchild.Items.Add(item);
                                                }

                                                element.ContextMenu.Items.Add(menuchild);
                                            }
                                            else
                                                element.ContextMenu = menu;
                                        }
                                    }
                                }

                                if (StringEditor != null)
                                {
                                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                        {
                                            if (!bDisposed)
                                                ChangeLanguage(StringEditor.GetActiveCulture(Document), element);
                                        });
                                }
#if !WINDOWS_UWP
                                if (UnitConverterEditor != null)
                                {
                                    var converter = UnitConverterEditor.GetActiveConverter(Document);
                                    if (!String.IsNullOrEmpty(converter))
                                        Document.ChangeUnitConverter(converter, UnitConverterEditor, element.Name);
                                }
#endif

                            }

                            SetZoomVisibilityItemsBackground(false);

                            listConnectionString = new List<FrameworkElement>();
                            var listAlarm = new List<FrameworkElement>();
                            var listEventConnectionString = new List<FrameworkElement>();
                            var listScheduler = new List<FrameworkElement>();
                            ScreenDocument.CheckPreBinding(listAlarm, listConnectionString, listEventConnectionString,
                                listScheduler, element);
                            UpdateBinding(listAlarm, listEventConnectionString, listScheduler);
                        };
                    //    };
                    //Dispatcher.BeginInvoke(action4, DispatcherPriority.DataBind);

                    //var listEdit = MainSurface.GetChildrenOfType<TextBox>();
                    //foreach (var el in listEdit)
                    //{
                    //    el.SetValue(VirtualKeyboard.PopupKeyboard.PlacementProperty, PlacementMode.Bottom);
                    //    el.SetValue(VirtualKeyboard.PopupKeyboard.PlacementTargetProperty, el);
                    //    el.SetValue(VirtualKeyboard.PopupKeyboard.HorizontalOffsetProperty, 20.0);
                    //    el.SetValue(VirtualKeyboard.PopupKeyboard.HeightProperty, 220.0);
                    //    el.SetValue(VirtualKeyboard.PopupKeyboard.WidthProperty, 200.0);
                    //    el.SetValue(VirtualKeyboard.PopupKeyboard.IsEnabledProperty, true);
                    //}

                    if (LayoutHelper.HasTouchInput())
                    {
                        var listScrollers = (from c in listChildren.OfType<ScrollViewer>() select c).ToList();
                        ;
                        foreach (var scroller in listScrollers)
                        {
                            scroller.PanningMode = PanningMode.Both;
                        }
                    }

                    Document.SetImagesBaseUri(MainSurface);
                    Document.SetImagesBaseUri(listChildren);

                    if (!Document.LayoutView)
                    {
                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            if (!bDisposed)
                                LoadLayoutData(GetUniqueTitle(Document));
                        });
                    }


                    //Dispatcher.BeginInvoke(action2, DispatcherPriority.DataBind);
                    //};

                    if (!bTest && !IsLayout)
                    {
                        //Action actionShortcuts = () =>
                        //    {
                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            if (!bDisposed)
                            {
                                ActivateCurrentShortcut();
                                MainSurface.Focusable = true;
                                MainSurface.Focus();
                                //    };
                                //Dispatcher.BeginInvoke(actionShortcuts, DispatcherPriority.DataBind);

                                ActivateCurrentMenuBar();
                            }
                        });
                    }

                    //GotFocus += (ob, ev) =>
                    //    {
                    //        System.Diagnostics.Debug.WriteLine("ScreenView Got Focus");
                    //    };
                    //LostFocus += (ob, ev) =>
                    //{
                    //    var focus = Keyboard.FocusedElement as FrameworkElement;
                    //    System.Diagnostics.Debug.WriteLine(String.Format("ScreenView Lost Focus, new Focused Element : {0}", focus.Name));
                    //};

                    //Dispatcher.BeginInvoke(action1, DispatcherPriority.DataBind);


                    MainSurface.ManipulationStarting += Window_ManipulationStarting;
                    MainSurface.ManipulationDelta += Window_ManipulationDelta;
                    // MainSurface.ManipulationInertiaStarting += Window_InertiaStarting;
                    MainSurface.ManipulationCompleted += Window_ManipulationCompleted;
#endif
                    if (ScreenComponent.UserEditor != null)
                        bUserEnabled = ScreenComponent.UserEditor.GetEnableUserManager(Document);

                    PrepareAccessLevelEntities();
                    mapAccessLevelEntities = new Dictionary<UIElement, String>();

                    //if (StringEditor != null)
                    //{
                    //    FrameworkElement mainCntrl = Document.LayoutView ? layoutItems as FrameworkElement : MainSurface as FrameworkElement;
                    //    ChangeLanguage(StringEditor.GetActiveCulture(Document), mainCntrl);
                    //    StringEditor.CultureChanged += StringEditor_CultureChanged;
                    //}

                    if (bUserEnabled)
                        SubscribeAccessLevelControls();

#if !WINDOWS_UWP
                    if (bUserEnabled && Document.RequireUserLogin && ScreenComponent != null &&
                        ScreenComponent.AuthenticationCredentialsProvider != null &&
                        ScreenComponent.AuthenticationCredentialsProvider.IsFaulted(DocumentHelper.GetRootParent(Document, traverse: true).Title))
                    {
                        txtLoginFirst.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (bUserEnabled)
                        {
                            SetVisibileItems(0);
                            SetAccessItems(null, -1);
                        }

                        currentRole = null;
                        currentLevel = -1;
                        currentAccessMask = 0;

                        if (bUserEnabled && Document.RequireUserLogin)
                        {
                            if (ScreenComponent != null && ScreenComponent.AuthenticationCredentialsProvider != null &&
                                ScreenComponent.AuthenticationCredentialsProvider.GetNumberOfUsersOnline(DocumentHelper.GetRootParent(Document, traverse: true).Title) <= 0)
                            {
                                txtLoginFirst.Visibility = Visibility.Visible;
                                // MainSurface.Visibility = Visibility.Collapsed;

                                //Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                //    {
                                //        if (!bDisposed)
                                //            OnLoginUser(null, null);
                                //    });
                            }
                            else
                                SetVisibleControl();
                        }
                        else
                            SetVisibleControl();
                        if (ScreenComponent != null && ScreenComponent.AuthenticationCredentialsProvider != null)
                        {
                            ScreenComponent.AuthenticationCredentialsProvider.UserOnline += AuthenticationCredentialsProvider_UserOnline;
                            if (!IsModal && !IsTile && !IsLayout)
                            {
                                ScreenComponent.AuthenticationCredentialsProvider.FailedLogin += AuthenticationCredentialsProvider_FailedLogin;
                                ScreenComponent.AuthenticationCredentialsProvider.FaultedProvider += AuthenticationCredentialsProvider_FaultedProvider;
                            }
                            ScreenComponent.AuthenticationCredentialsProvider.RefreshCurrentUser(DocumentHelper.GetRootParent(Document, traverse: true).Title);
                        }
                    }
#endif
                    SetZoomVisibilityItemsBackground(false);
                    var cachedElement = (from c in listChildren where c.CacheMode != null select c).ToList();
                    if (cachedElement.Count > 0)
                    {
                        mapElementCacheMode = new Dictionary<UIElement, CacheMode>();
                        cachedElement.ForEach(element => mapElementCacheMode.Add(element, element.CacheMode));
                    }

#if !WINDOWS_UWP
                    if (!IsLayout && !bDisposed && (IsModal || Document.ApplyWindowSettingsOnLoad))
                    {
                        Action action = () =>
                        {
                            if (bDisposed)
                                return;

                            var wnd = this.FindParent<Window>();
                            if (wnd != null)
                            {
                                if (wndParent == null)
                                {
                                    wndParent = wnd;
                                    wnd.Activated += wnd_Activated;
                                    wnd.Deactivated += wnd_Deactivated;
                                    wnd.SizeChanged += wnd_SizeChanged;
                                }

                                wnd.ShowActivated = true;
                                UpdateWindowStateStyle(wnd);

                                wnd.ResizeMode = Document.ResizeMode;
                                //wnd.ShowInTaskbar = Document.ShowInTaskbar;

                                if (StringEditor != null)
                                {
                                    var map = StringEditor.GetListStringForCulture(Document, StringEditor.GetActiveCulture(Document));
                                    if (map == null || map.Count == 0 || !map.ContainsKey(Document.Title))
                                        wnd.Title = Document.Title;
                                    else
                                        wnd.Title = map[Document.Title];
                                }
                                else
                                    wnd.Title = Document.Title;

                                wndParent = wnd;
                                UpdateWindowPosition(wnd);

                                wnd_SizeChanged(this, EventArgs.Empty);
                            }
                        };

                        var wndcheck = this.FindParent<Window>();
                        if (wndcheck != null && IsModal)
                            action();
                        else
                        {
                            if (wndcheck == null)
                            {
                                timerDiscoverWindow = new DispatcherTimer();
                                timerDiscoverWindow.Interval = TimeSpan.FromMilliseconds(50);
                                timerDiscoverWindow.Tick += (o, ev) =>
                                {
                                    if (bDisposed)
                                    {
                                        timerDiscoverWindow.Stop();
                                    }
                                    else
                                    {
                                        wndcheck = this.FindParent<Window>();
                                        if (wndcheck != null)
                                        {
                                            timerDiscoverWindow.Stop();
                                            action();
                                        }
                                    }
                                };
                                timerDiscoverWindow.Start();
                            }
                            else
                                action();
                            // Dispatcher.BeginInvokeAsynchronouslyInRender(action);
                        }
                    }

#if !WINDOWS_UWP
                    if (UnitConverterEditor != null)
                    {
                        var converter = UnitConverterEditor.GetActiveConverter(Document);
                        if (!String.IsNullOrEmpty(converter))
                        {
                            Document.ChangeUnitConverter(converter, UnitConverterEditor);
                            UnitConverterEditor.CurrentConverterChanged += UnitConverterEditor_CurrentConverterChanged;
                        }

                        if (Document != null)
                            Document.ParameterFileChanged += Document_ParameterFileChanged;
                    }
#endif

                    if (!bLoadingSymbols)
                    {
                        // dp.Completed += (o, ev) =>
                        // {
                        progressLoading.IsIndeterminate = false;
                        progressLoading.Visibility = Visibility.Collapsed;

                            if (Document != null)
                            {
                                PrepareAccessLevelEntities();
                                if (bUserEnabled)
                                {
                                    SetVisibleItemsOnServer();
                                    SetVisibileItems(currentAccessMask);
                                    SubscribeAccessLevelControls();
                                }

                                SetZoomVisibilityItemsBackground(false);
                            }
                        // };

#if !WINDOWS_UWP
                        if (!bTest)
                            LoadContextMenus();
#endif
                    }
#endif
                    bInitialized = true;
                    if (!bLoadingSymbols)
                        Document.OnScreenLoaded();
                    // ScreenViewerControl_SizeChanged(null, null);
                }
                finally
                {
#if !WINDOWS_UWP
                    MainSurface.EndInit();
                    SetAutoClose();
#endif

                    if (IsModal && timerRefreshCommands == null)
                    {
                        timerRefreshCommands = new DispatcherTimer();
                        timerRefreshCommands.Interval = TimeSpan.FromMilliseconds(1000);
                        timerRefreshCommands.Tick += (o, ev) =>
                        {
                            if (!bDisposed)
                                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                        };
                        timerRefreshCommands.Start();
                    }

                    if (Document != null)
                    {
                        var rootParent = DocumentHelper.GetRootParent(Document, traverse: false);
                        if (rootParent != null)
                            ScreenCompilerManager.ScreenCompilerManager.CompileScreen(Document.FullPath, rootParent.FilePath);
                    }
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            if (bRunningOnServerCtor && IsInFillMode)
            {
                UpdateZoomFillMode();
            }
            return size;
        }

        internal void ForceIsLayout(bool bSet)
        {
            IsLayout = bSet;
        }

        bool bUpdatingWindowStyle;
        DispatcherOperation pendingSizeToContent;
        DispatcherOperation pendingWindowState;
        private void UpdateWindowStateStyle(Window wnd)
        {
            if (bUpdatingWindowStyle)
                return;
            bUpdatingWindowStyle = true;
            try
            {
                if (Document.WindowOpacity < 1 && IsModal)
                {
                    if (!Document.WindowOpacityOnlyInactive)
                        wnd.Opacity = Document.WindowOpacity;
                }
                else
                {
                    if (Document.WindowStyle == WindowStyle.ToolWindow && !IsModal)
                        wnd.WindowStyle = WindowStyle.SingleBorderWindow;
                    else
                        wnd.WindowStyle = Document.WindowStyle;
                }

                if (!IsTile)
                {
                    if (Document.WindowState != System.Windows.WindowState.Maximized &&
                        (!Document.FitInWindow || IsModal))
                    {
                        if (pendingSizeToContent == null)
                        {
                            wnd.SizeToContent = SizeToContent.WidthAndHeight;

                            pendingSizeToContent = Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                            {
                                pendingSizeToContent = null;
                                if (bDisposed)
                                    return;
                                wnd.SizeToContent = SizeToContent.Manual;
                            });
                        }
                    }
                    else
                        wnd.SizeToContent = SizeToContent.Manual;

                    if (IsModal && Document.WindowState == WindowState.Maximized)
                    {
                        if (pendingWindowState == null)
                        {
                            Visibility = Visibility.Hidden;
                            wnd.WindowState = WindowState.Minimized;

                            pendingWindowState = Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                            {
                                pendingWindowState = null;
                                if (bDisposed)
                                    return;

                                wnd.WindowState = Document.WindowState;
                                Visibility = Visibility.Visible;
                            });
                        }
                    }
                    else if (pendingWindowState == null)
                        wnd.WindowState = Document.WindowState;
                }
                else
                {
                    wnd.SizeToContent = SizeToContent.Manual;
                    wnd.WindowState = Document.WindowState;
                }

                if (wnd.WindowStyle == WindowStyle.None && (wnd is DevExpress.Xpf.Core.DXWindow || wnd is DevExpress.Xpf.Core.ThemedWindow))
                {
                    wnd.Padding = new Thickness(0);
                    wnd.BorderThickness = new Thickness(0);
                    wnd.ResizeMode = ResizeMode.NoResize;
                }
            }
            finally
            {
                bUpdatingWindowStyle = false;
            }
        }

#if !WINDOWS_UWP
        internal void Activate3DCameraView(String controlName, String viewName)
        {
            var child = Document.FindInnerControl(MainSurface, controlName);
            if (child == null || !Document.MapScreenEntities.ContainsKey(controlName))
                return;

            var list3Ds = child.GetChildrenOfType<Viewport3D>().ToList();
            if (child is Viewport3D)
                list3Ds.Insert(0, child as Viewport3D);
            if (list3Ds.Count == 0)
                return;

            var viewport3D = list3Ds.First();
            if (!mapElementTrackball.ContainsKey(viewport3D))
                return;
            var view = (from c in Document.MapScreenEntities[controlName].ListCameraTransforms where c.Name == viewName select c).ToList();
            if (view.Count == 0)
                return;

            mapElementTrackball[viewport3D].Animate(view[0].Scale, view[0].Rotation, view[0].TranslateTransform2D,
                view[0].AnimationTime, new SineEase() { EasingMode = EasingMode.EaseOut });
        }

        double customTop = Double.NaN;
        double customLeft = Double.NaN;
        internal void UpdateWindowPosition(Window wnd, bool bFromViewer = false)
        {
            if (wnd != null && Document != null && (IsModal || Document.ApplyWindowSettingsOnLoad))
            {
                if (wnd.WindowState == WindowState.Minimized)
                {
                    wnd.WindowState = WindowState.Normal;
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        if (!bDisposed)
                            UpdateWindowPosition(wnd, bFromViewer);
                    });
                    return;
                }

                UpdateWindowStateStyle(wnd);
                wnd.WindowStartupLocation = Document.WindowStartupLocation;
                if (!Double.IsNaN(customTop) && !Double.IsNaN(customLeft))
                {
                    wnd.WindowStartupLocation = WindowStartupLocation.Manual;
                    if (bIsCenterOwner)
                    {
                        var ptScreen = wnd.Owner.PointToScreen(new Point(0, 0));
                        var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)ptScreen.X, (int)ptScreen.Y));
                        var workingArea = screen.WorkingArea;
                        wnd.Top = customTop + (workingArea.Height - Document.Height) / 2;
                        wnd.Left = customLeft + (workingArea.Width - Document.Width) / 2;
                    }
                    else
                    {
                        wnd.Top = customTop;
                        wnd.Left = customLeft;
                    }
                }
                else if (wnd.WindowStartupLocation == WindowStartupLocation.Manual)
                {
                    wnd.Top = Document.Top;
                    wnd.Left = Document.Left;
                }
                else if (wnd.WindowStartupLocation == WindowStartupLocation.CenterScreen)
                {
                    if (!wnd.IsArrangeValid || !wnd.IsMeasureValid)
                        wnd.UpdateLayout();
                    wnd.WindowStartupLocation = WindowStartupLocation.Manual;

                    if (wnd.Owner != null && bIsCenterOwner)
                    {
                        var ptScreen = wnd.Owner.PointToScreen(new Point(0, 0));
                        var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)ptScreen.X, (int)ptScreen.Y));
                        var workingArea = screen.WorkingArea;
                        wnd.Top = workingArea.Top + (workingArea.Height - wnd.ActualHeight) / 2;
                        wnd.Left = workingArea.Left + (workingArea.Width - wnd.ActualWidth) / 2;
                    }
                    else
                    {
                        var workingArea = System.Windows.SystemParameters.WorkArea;
                        wnd.Top = (workingArea.Height - wnd.ActualHeight) / 2;
                        wnd.Left = (workingArea.Width - wnd.ActualWidth) / 2;
                    }
                }
                else if (wnd.WindowStartupLocation == WindowStartupLocation.CenterOwner)
                {
                    if (wnd.Owner != null)
                    {
                        if (!wnd.IsArrangeValid || !wnd.IsMeasureValid)
                            wnd.UpdateLayout();
                        wnd.WindowStartupLocation = WindowStartupLocation.Manual;
                        wnd.Top = ((wnd.Owner.ActualHeight - wnd.ActualHeight) / 2) + wnd.Owner.Top;
                        wnd.Left = ((wnd.Owner.ActualWidth - wnd.ActualWidth) / 2) + wnd.Owner.Left;
                    }
                    else
                    {
                        if (!wnd.IsArrangeValid || !wnd.IsMeasureValid)
                            wnd.UpdateLayout();
                        wnd.WindowStartupLocation = WindowStartupLocation.Manual;
                        var workingArea = System.Windows.SystemParameters.WorkArea;
                        wnd.Top = (workingArea.Height - wnd.ActualHeight) / 2;
                        wnd.Left = (workingArea.Width - wnd.ActualWidth) / 2;
                    }
                }

                if (!IsTile && MonitorNumber > 0 && System.Windows.Forms.Screen.AllScreens != null &&
                    MonitorNumber <= System.Windows.Forms.Screen.AllScreens.Length)
                {
                    //var mon = Monitors.GetScreens();
                    //var dev = Monitors.GetDevices();
                    //for (var i = 0; i<dev.Count; i++)
                    //{
                    //    var s = Monitors.GetSettings(dev[i].DeviceName);
                    //    var moninfo = Monitors.GetDeviceInfo(dev[i].DeviceName);
                    //}

                    wnd.WindowStartupLocation = WindowStartupLocation.Manual;

                    //sorting AllScreens as control panel's display settings arrangement (note: AllScreens coordinates are relative to primary monitor's position and may be negative)
                    var allScreensArranged = System.Windows.Forms.Screen.AllScreens.OrderBy(s => s.DeviceName).ToList();

                    //var controlPanelMonitorArrangement = new int[allScreensArranged.Count()];
                    //var coordsAscOrder = allScreensArranged.OrderBy(s => s.WorkingArea.Left).ToList();
                    //int monIndex = 0;
                    //foreach (var s in allScreensArranged)
                    //{
                        //Every left values ordered array's item positioning says how much the display in that position moved respect to primary monitor position:
                        //0 -1920 -3840 -5760 => monitor in position 2 moved 1920 left from primary monitor's position (4), so its on position 3 and so on
                        //Finding controls panel order from values: first monitor is the index of the lowest value and so on
                        //So monitor at current index is the index of its left coord on the ordered array
                        //es. arranged: 0 -1920 -3840 -5760 => asc: -5760 -3840 -1920 0
                        //At first position we have arranged.indexOf(asc[0])+1 = monitor 4
                        //second : arranged.indexOf(asc[1])+1 = monitor 3
                        //[...]
                        //Note: final controlPanelMonitorArrangement will be 0 based
                    //    controlPanelMonitorArrangement[monIndex] = allScreensArranged.IndexOf(coordsAscOrder[monIndex]);
                    //    monIndex++;
                    //}
                    //System.Diagnostics.Debug.Print(controlPanelMonitorArrangement.ToString()); //0 based

                    if (!bIsRelative)
                    {
                        MonitorsInfos infos = new MonitorsInfos();
                        var screenData = infos.GetDisplaySettings(MonitorNumber - 1);
                        float primaryScalingFactor = (float)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth);

                        if (screenData != null)
                        {
                            wnd.Left = screenData.Value.dmPositionX + (((allScreensArranged[MonitorNumber - 1].Bounds.Right - allScreensArranged[MonitorNumber - 1].Bounds.Left) / primaryScalingFactor) - wnd.ActualWidth) / 2;
                            wnd.Top = screenData.Value.dmPositionY + (((allScreensArranged[MonitorNumber - 1].Bounds.Bottom - allScreensArranged[MonitorNumber - 1].Bounds.Top) / primaryScalingFactor) - wnd.ActualHeight) / 2;
                        }
                        else
                        {
                            wnd.Left = allScreensArranged[MonitorNumber - 1].WorkingArea.Left + (((allScreensArranged[MonitorNumber - 1].Bounds.Right - allScreensArranged[MonitorNumber - 1].Bounds.Left) / primaryScalingFactor) - wnd.ActualWidth) / 2;
                            wnd.Top = allScreensArranged[MonitorNumber - 1].WorkingArea.Top + (((allScreensArranged[MonitorNumber - 1].Bounds.Bottom - allScreensArranged[MonitorNumber - 1].Bounds.Top) / primaryScalingFactor) - wnd.ActualHeight) / 2;
                        }

                        if (!Double.IsNaN(customTop) && !Double.IsNaN(customLeft))
                        {
                            wnd.Left = screenData.Value.dmPositionX + customLeft;
                            wnd.Top = screenData.Value.dmPositionY + customTop;
                        }

                        //var targetWindowBounds = new System.Drawing.Rectangle();
                        //targetWindowBounds = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(wnd).Handle).Bounds;
                        //MessageBox.Show("WND BOUNDS: " + targetWindowBounds.Top + ", " + targetWindowBounds.Left);
                    }

                    //wnd.Width = allScreensArranged[controlPanelMonitorArrangement[MonitorNumber - 1]].WorkingArea.Width;
                    //wnd.Height = allScreensArranged[controlPanelMonitorArrangement[MonitorNumber - 1]].WorkingArea.Height;

                    if (Document.WindowOpacity < 1 && IsModal &&
                        wnd.WindowState == WindowState.Maximized)
                    {
                        wnd.WindowState = WindowState.Normal;
                        Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                        {
                            if (!bDisposed)
                                wnd.WindowState = WindowState;
                        });
                    }
                    else if (pendingWindowState == null)
                        wnd.WindowState = WindowState;
                }

                if (bFromViewer)
                {
                    wnd.ResizeMode = Document.ResizeMode;
                    //wnd.ShowInTaskbar = Document.ShowInTaskbar;
                    //wnd.Activate();
                }
            }

#if DEBUG
            if (wnd != null)
                System.Diagnostics.Debug.Write(String.Format("Top {0}, Left {1}, Stack {2}", wnd.Top, wnd.Left,
                                            System.Environment.StackTrace));
#endif
        }
#endif
        String previousCulture;
        String PreviousCulture { get { return previousCulture; } }
        String previousConverter;
        String PreviousConverter { get { return previousConverter; } }
        String currentRole;
        int currentLevel = -1;
        int currentAccessMask = 0;
        String requestedRole;
        int requestedLevel = -1;
        String lastUser;
        UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs lastLoginInfo;
        void AuthenticationCredentialsProvider_UserOnline(object sender, UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs ev)
        {
            if (lastUser == ev.User)
                return;
            lastUser = ev.User;

#if !WINDOWS_UWP
            Dispatcher.BeginInvokeIfRequired(() =>
#else
            RunOnUIThread.RunIfRequired(() =>
#endif
                {
                    if (IsVisible)
                        ChangeUser(ev);
                    else 
                    {
                        if (lastLoginInfo == null)
                        {
                            lastLoginInfo = ev;
                            var propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsVisibleProperty, typeof(UIElement));
                            var notifierVisibility = new PropertyChangeNotifier(this, propDesc.Name);
                            if (listPropertyChangeNotifier == null)
                                listPropertyChangeNotifier = new List<PropertyChangeNotifier>();
                            listPropertyChangeNotifier.Add(notifierVisibility);
                            notifierVisibility.ValueChanged += (o, e) =>
                            {
                                if (IsVisible)
                                {
                                    notifierVisibility.Dispose();
                                    listPropertyChangeNotifier.Remove(notifierVisibility);
                                    ChangeUser(lastLoginInfo);
                                    lastLoginInfo = null;
                                }
                            };
                        }
                        else
                            lastLoginInfo = ev;
                    }
                });
        }

        async void ChangeLanguageThread(UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs ev)
        {
            var taskChangingLanguage = Task.Factory.StartNew(() =>
            {
                if (bDisposed)
                    return false;

                var oldPriority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                try
                {
                    if (ev.UserIdentity != null) 
                    {
                        RealTimeConnectionManagerViewModel.SetUserIdentity(
                            Document.SessionString,
                            ev.UserIdentity,
                            new StringCollection());
                    } 
                    else
                    {
                        if (String.IsNullOrEmpty(ev.User))
                            RealTimeConnectionManagerViewModel.SetUserIdentity(Document.SessionString, null, new StringCollection());
                        else
                            RealTimeConnectionManagerViewModel.SetUserIdentity(Document.SessionString, new UserIdentity(ev.User, ev.Password), new StringCollection());
                    }
                }
                catch (Exception ex)
                {
                    // ScreenComponent.UIInterface.ShowError(String.Format(Properties.Resources.FailedToActivateUserOnServer, ex.Message));
#if !WINDOWS_UWP
                    if (ScreenComponent.Workspace != null)
                        ScreenComponent.Workspace.ShowTaskBarTooltip(Document.Title,
                            String.Format(Properties.Resources.FailedToActivateUserOnServer, ex.Message), 5000);
                    logUsers.Warn(String.Format(Properties.Resources.FailedToActivateUserOnServer, ex.Message));
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(DocumentParent, Properties.Resources.UsersManager,
                        DateTime.UtcNow, String.Format(Properties.Resources.FailedToActivateUserOnServer, ex.Message),
                        System.Diagnostics.EventLogEntryType.Warning);
#endif
                }
                finally
                {
                    Thread.CurrentThread.Priority = oldPriority;
                }

                return true;
            });

            await taskChangingLanguage;
        }

        void ChangeUser(UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs ev)
        {
            //using (var cursor = new WaitCursor())
            //{
            //    //try
            //    //{
            //    bool bUserEnabled = true;
            //    if (ScreenComponent.UserEditor != null)
            //        bUserEnabled = ScreenComponent.UserEditor.GetEnableUserManager(Document);
            //}

            {
                if (String.IsNullOrEmpty(ev.User))
                {
                    using (var cursor = new WaitCursor())
                    {
                        if (!ev.IsRefreshing)
                            ChangeLanguageThread(ev);

                        if (bUserEnabled)
                        {
                            SetVisibileItems(0);
                            SetAccessItems(null, -1);
                        }
                    }

                    if (bUserEnabled && Document.RequireUserLogin)
                    {
                        SetVisibleControl(false);
#if !WINDOWS_UWP
                        txtLoginFirst.Visibility = Visibility.Visible;
                        // MainSurface.Visibility = Visibility.Collapsed;

                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
#else
                                    RunOnUIThread.Run(() =>
#endif
                        {
                            if (!bDisposed)
                                OnLoginUser(null, null);
                        });
                    }

                    using (var cursor = new WaitCursor())
                    {
                        currentRole = null;
                        currentLevel = -1;
                        currentAccessMask = 0;

                        //if (!ev.IsRefreshing)
                        //    logUsers.Info(Properties.Resources.UserLogOut);
#if !WINDOWS_UWP
                        txtUser.Text = String.Empty;
#endif
                        if (!bDisposed)
                        {
                            //#if !WINDOWS_UWP
                            //                                    FrameworkElement mainControl = Document.LayoutView ? layoutItems as FrameworkElement : MainSurface as FrameworkElement;
                            //#else
                            //                                    FrameworkElement mainControl = MainSurface as FrameworkElement;
                            //#endif
                            //                                    ChangeLanguage(previousCulture, mainControl);

                            if (!String.IsNullOrEmpty(PreviousCulture))
                                StringEditor.SetActiveCulture(Document, PreviousCulture);
                            if (!String.IsNullOrEmpty(PreviousConverter))
                                (UnitConverterEditor as IDocumentManager).Execute(String.IsNullOrEmpty(PreviousConverter) ? null : new Uri(PreviousConverter,
                                                                                    UriKind.RelativeOrAbsolute), Document, ExecutionMode.Normal, null);
                            //previousCulture = String.Empty;
                        }
                    }
                }
                else
                {
                    using (var cursor = new WaitCursor())
                    {
                        SetVisibleControl();
#if !WINDOWS_UWP
                        txtLoginFirst.Visibility = Visibility.Collapsed;
#endif
                        var role = string.Empty;
                        var accessMask = 0;
                        var accessLevel = 0;
                        var culture = string.Empty;
                        var converter = string.Empty;

                        if (ev.IsExternalAuthenticationActive)
                        {
                            role = ScreenComponent.UserEditor.GetExternalAuthenticationUserRole(DocumentParent, ev.Role);
                            accessLevel = ScreenComponent.UserEditor.GetRoleAccessLevel(DocumentParent, role);
                            accessMask = ScreenComponent.UserEditor.GetRoleAccessMask(DocumentParent, role);
                            culture = ScreenComponent.UserEditor.GetRoleCultureName(Document, role);
                            converter = ScreenComponent.UserEditor.GetRoleConverterName(Document, role);
                        }
                        else
                        {
                            role = ScreenComponent.UserEditor.GetUserRole(Document, ev.User);
                            accessMask = ScreenComponent.UserEditor.GetUserAccessMask(Document, ev.User);
                            accessLevel = ScreenComponent.UserEditor.GetUserAccessLevel(Document, ev.User);
                            culture = ScreenComponent.UserEditor.GetUserCultureName(Document, ev.User);
                            converter = ScreenComponent.UserEditor.GetUserConverterName(Document, ev.User);
                        }

                        ChangeLanguageThread(ev);

                        if (bUserEnabled)
                        {
                            SetVisibileItems(accessMask);
                            SetAccessItems(role, accessLevel);
                        }
#if !WINDOWS_UWP
                        txtUser.Text = ev.User;
#endif

                        currentRole = role;
                        currentLevel = accessLevel;
                        currentAccessMask = accessMask;

                        if (!bDisposed && !ev.IsRefreshing)
                        {
                            if (!String.IsNullOrEmpty(culture))
                                StringEditor.SetActiveCulture(Document, culture);
                            if (!String.IsNullOrEmpty(converter))
                                (UnitConverterEditor as IDocumentManager).Execute(String.IsNullOrEmpty(converter) ? null : new Uri(converter,
                                                                                    UriKind.RelativeOrAbsolute), Document, ExecutionMode.Normal, null);
                        }

#if !WINDOWS_UWP
                        Dispatcher.BeginInvokeAsynchronouslyInInput(() =>
                        {
                            if (currentFocusElement != null)
                            {
                                currentFocusElement.Focus();
                                currentFocusElement = null;
                            }
                        });
#endif
                    }
                }

                //}
                //catch (Exception ex)
                //{
                //    ScreenComponent.UIInterface.ShowError(ex.Message);
                //}
            }
        }

        void AuthenticationCredentialsProvider_FailedLogin(object sender, UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs ev)
        {
            if (IsModal && !IsTile && !IsLayout || bActive)
            {
                if (!ev.IsLocked)
                    ScreenComponent.UIInterface.ShowError(Properties.Resources.FailedToLogin);
                else
                    ScreenComponent.UIInterface.ShowError(Properties.Resources.FailedToLoginUserLocked);
            }
        }

        void AuthenticationCredentialsProvider_FaultedProvider(object sender, UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs ev)
        {
            if (IsModal && !IsTile && !IsLayout || bActive)
            {
                if (string.IsNullOrEmpty(ev.ErrorInfo))
                {
                    ScreenComponent.UIInterface.ShowError(Properties.Resources.FaultMembershipProvider);
                }
                else
                {
                    ScreenComponent.UIInterface.ShowError(ev.ErrorInfo);
                }
            }                
        }

        bool bPendingChangeLanguage;
        void StringEditor_CultureChanged(object sender, EventArgs e)
        {
#if !WINDOWS_UWP
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
#else
            RunOnUIThread.Run(() =>
#endif
            {
                var action = new Action(() =>
                {
                    if (!bDisposed)
                    {
                        var culture = StringEditor.GetActiveCulture(Document);

#if !WINDOWS_UWP
                        FrameworkElement mainControl = Document.LayoutView ? layoutItems as FrameworkElement : MainSurface as FrameworkElement;
#else
                        FrameworkElement mainControl = MainSurface as FrameworkElement;
#endif
                        ChangeLanguage(culture, mainControl);
                    }
                });

                if (IsVisible)
                    action();
                else if (!bPendingChangeLanguage)
                {
                    bPendingChangeLanguage = true;
                    var propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsVisibleProperty, typeof(UIElement));
                    var notifierVisibility = new PropertyChangeNotifier(this, propDesc.Name);
                    if (listPropertyChangeNotifier == null)
                        listPropertyChangeNotifier = new List<PropertyChangeNotifier>();
                    listPropertyChangeNotifier.Add(notifierVisibility);
                    notifierVisibility.ValueChanged += (o, ev) =>
                    {
                        if (IsVisible)
                        {
                            notifierVisibility.Dispose();
                            listPropertyChangeNotifier.Remove(notifierVisibility);
                            bPendingChangeLanguage = false;
                            action();
                        }
                    };
                }
            });
        }

#if !WINDOWS_UWP
        void Document_ParameterFileChanged(object sender, EventArgs e)
        {
            var converter = UnitConverterEditor.GetActiveConverter(Document);
            if (!String.IsNullOrEmpty(converter))
                Document.ChangeUnitConverter(converter, UnitConverterEditor);
        }

        bool bPendingUnitConverter;
        void UnitConverterEditor_CurrentConverterChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                var action = new Action(() =>
                {
                    if (!bDisposed && Document != null)
                    {
                        var converter = UnitConverterEditor.GetActiveConverter(Document);
                        //if (!String.IsNullOrEmpty(converter))
                        Document.ChangeUnitConverter(converter, UnitConverterEditor);
                    }
                });

                if (IsVisible)
                    action();
                else if (!bPendingUnitConverter)
                {
                    bPendingUnitConverter = true;
                    var propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsVisibleProperty, typeof(UIElement));
                    var notifierVisibility = new PropertyChangeNotifier(this, propDesc.Name);
                    if (listPropertyChangeNotifier == null)
                        listPropertyChangeNotifier = new List<PropertyChangeNotifier>();
                    listPropertyChangeNotifier.Add(notifierVisibility);
                    notifierVisibility.ValueChanged += (o, ev) =>
                    {
                        if (IsVisible)
                        {
                            notifierVisibility.Dispose();
                            listPropertyChangeNotifier.Remove(notifierVisibility);
                            bPendingUnitConverter = false;
                            action();
                        }
                    };
                }
            });
        }
#endif

        DispatcherOperation d1;
        void ChangeLanguage(String culture, FrameworkElement mainControl)
        {
            using (var cursor = new WaitCursor())
            {
                var map = StringEditor.GetListStringForCulture(Document, culture);
                if (map == null || map.Count == 0)
                    return;

                Document.ChangeLanguage(culture, mainControl, map, false, mainControl.GetChildrenOfType<SpecialObjects.EmbeddedTabScreen>().Cast<UserControl>().ToList());

#if !WINDOWS_UWP
                if (d1 == null || d1.Status == DispatcherOperationStatus.Completed ||
                    d1.Status == DispatcherOperationStatus.Aborted)
                {
                    d1 = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        DestroyOwnedMenus();
                        LoadContextMenus();

                        ActivateCurrentMenuBar();
                    });
                }
#endif
            }
        }

        void SetVisibleControl(bool bVisible = true)
        {
#if !WINDOWS_UWP
            if (Document.LayoutView)
            {
                layoutItems.Visibility = bVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            else
#endif
            {
                //if (viewBox.Child != null)
                //    viewBox.Visibility = bVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
#if !WINDOWS_UWP
                MainSurface.Visibility = bVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                iconizer.Visibility = bVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
#else
                MainSurface.Visibility = bVisible ? Visibility.Visible : Visibility.Collapsed;
#endif
            }
        }

        void PrepareAccessLevelEntities()
        {
            if (bUserEnabled)
            {
                listAccessEntities = (from c in Document.MapScreenEntities.AsParallel()
                                      where c.Value.HasAccessControl
                                      select c.Key).ToList();

                listAccessLevelEntities = (from c in Document.MapScreenEntities.AsParallel()
                                           where c.Value.HasAccessLevelControl
                                           select c.Key).ToList();
            }

            listZoomVisibilityEntities = (from c in Document.MapScreenEntities.AsParallel()
                                          where c.Value.HasZoomVisibility
                                          select c.Key).ToList();
        }

        private void SubscribeAccessLevelControls()
        {
            if (bRunningOnServerCtor)
                return;

            listAccessLevelEntities.ForEach(name =>
                {
                    var child = Document.FindInnerControl(MainSurface, name);
                    if (child != null && !mapAccessLevelEntities.ContainsKey(child))
                    {
                        child.GotFocus += child_GotFocus;
                        //if (Environment.OSVersion.Version < new Version(6, 2))
                        {
                            child.PreviewTouchDown += child_PreviewTouchDown;
                            child.PreviewTouchUp += child_PreviewTouchDown;
                        }
#if !WINDOWS_UWP
                        child.PreviewMouseDown += child_PreviewMouseDown;
                        child.PreviewMouseUp += child_PreviewMouseDown;
#else
                        child.PointerPressed += child_PreviewMouseDown;
                        child.PointerReleased += child_PreviewMouseDown;
#endif
                        SetIsAccessDeniedPropertyValue(child, currentRole, currentLevel);
                        mapAccessLevelEntities.Add(child, name);
                    }
                });
        }

        private void Document_CommandExecuting(object sender, CancelEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;
            if (!mapAccessLevelEntities.ContainsKey(element) || !Document.MapScreenEntities.ContainsKey(mapAccessLevelEntities[element]))
                return;

            if (!Document.MapScreenEntities[mapAccessLevelEntities[element]].HasAccessLevel(currentRole, currentLevel))
            {
                e.Cancel = true;

                requestedRole = Document.MapScreenEntities[mapAccessLevelEntities[element]].AccessRole;
                requestedLevel = Document.MapScreenEntities[mapAccessLevelEntities[element]].AccessLevel;

#if !WINDOWS_UWP
                bool bWasEnabled = element.IsEnabled;
                element.IsEnabled = false;
                CommandManager.CommandManager.SetIsAccessDenied(element, !element.IsEnabled);
                AnimationManager.AnimationManager.SetIsAccessDenied(element, !element.IsEnabled);
#endif
                currentFocusElement = element;
                OnLoginUser(null, null);
                currentFocusElement = null;

#if !WINDOWS_UWP
                if (bWasEnabled)
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() => {
                        element.IsEnabled = true;
                        CommandManager.CommandManager.SetIsAccessDenied(element, !element.IsEnabled);
                        AnimationManager.AnimationManager.SetIsAccessDenied(element, !element.IsEnabled);
                        if (mapAccessLevelEntities.ContainsKey(element) && 
                            Document.MapScreenEntities.ContainsKey(mapAccessLevelEntities[element]))
                            Document.MapScreenEntities[mapAccessLevelEntities[element]].ReexecuteEnable();
                    });
                }
#endif
            }
        }

        void child_PreviewMouseDown(object sender,
#if !WINDOWS_UWP
            MouseButtonEventArgs e)
#else
            Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            RequestAccessLevel(sender, e);
        }

        private void UnsubscribeAccessLevelControls()
        {
            if (listAccessLevelEntities == null || bRunningOnServerCtor)
                return;

            listAccessLevelEntities.ForEach(name =>
            {
                var child = Document.FindInnerControl(MainSurface, name);
                if (child != null)
                {
                    child.GotFocus -= child_GotFocus;
                    //if (Environment.OSVersion.Version < new Version(6, 2))
                    {
                        child.PreviewTouchDown -= child_PreviewTouchDown;
                        child.PreviewTouchUp -= child_PreviewTouchDown;
                    }
#if !WINDOWS_UWP
                    child.PreviewMouseDown -= child_PreviewMouseDown;
                    child.PreviewMouseUp -= child_PreviewMouseDown;
#else
                    child.PointerPressed -= child_PreviewMouseDown;
                    child.PointerReleased -= child_PreviewMouseDown;
#endif
                }
            });

            mapAccessLevelEntities.Clear();
        }

        private void child_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            RequestAccessLevel(sender, e);
        }

        private void RequestAccessLevel(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;
            if (!mapAccessLevelEntities.ContainsKey(element) || !Document.MapScreenEntities.ContainsKey(mapAccessLevelEntities[element]))
                return;

            if (!Document.MapScreenEntities[mapAccessLevelEntities[element]].HasAccessLevel(currentRole, currentLevel))
            {
                e.Handled = true;

                requestedRole = Document.MapScreenEntities[mapAccessLevelEntities[element]].AccessRole;
                requestedLevel = Document.MapScreenEntities[mapAccessLevelEntities[element]].AccessLevel;

#if !WINDOWS_UWP
                bool bWasEnabled = element.IsEnabled;
                element.IsEnabled = false;
                CommandManager.CommandManager.SetIsAccessDenied(element, !element.IsEnabled);
                AnimationManager.AnimationManager.SetIsAccessDenied(element, !element.IsEnabled);
#endif
                var action = new Action(() =>
                {
                    currentFocusElement = element;
                    OnLoginUser(null, null);
                    currentFocusElement = null;
#if !WINDOWS_UWP
	                if (bWasEnabled)
	                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
	                    {
	                        element.IsEnabled = true;
	                        CommandManager.CommandManager.SetIsAccessDenied(element, !element.IsEnabled);
	                        AnimationManager.AnimationManager.SetIsAccessDenied(element, !element.IsEnabled);
                            if (mapAccessLevelEntities.ContainsKey(element) &&
                                Document.MapScreenEntities.ContainsKey(mapAccessLevelEntities[element]))
                                Document.MapScreenEntities[mapAccessLevelEntities[element]].ReexecuteEnable();
	                    });
#endif
                });

#if !WINDOWS_UWP
                if (e is TouchEventArgs)
                    Dispatcher.BeginInvokeAsynchronouslyInInput(() => action());
                else
#endif
                    action();
            }
        }
        UIElement currentFocusElement;
        void child_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                return;
            if (!mapAccessLevelEntities.ContainsKey(element) || !Document.MapScreenEntities.ContainsKey(mapAccessLevelEntities[element]))
                return;

            if (!Document.MapScreenEntities[mapAccessLevelEntities[element]].HasAccessLevel(currentRole, currentLevel))
            {
#if !WINDOWS_UWP
                bool bWasEnabled = element.IsEnabled;
                element.IsEnabled = false;
                CommandManager.CommandManager.SetIsAccessDenied(element, !element.IsEnabled);
                AnimationManager.AnimationManager.SetIsAccessDenied(element, !element.IsEnabled);
#endif
                currentFocusElement = element;
                OnLoginUser(null, null);
                currentFocusElement = null;

#if !WINDOWS_UWP
                if (bWasEnabled)
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        element.IsEnabled = true;
                        CommandManager.CommandManager.SetIsAccessDenied(element, !element.IsEnabled);
                        AnimationManager.AnimationManager.SetIsAccessDenied(element, !element.IsEnabled);
                        if (mapAccessLevelEntities.ContainsKey(element) &&
                            Document.MapScreenEntities.ContainsKey(mapAccessLevelEntities[element]))
                            Document.MapScreenEntities[mapAccessLevelEntities[element]].ReexecuteEnable();
                    });
#endif
            }
        }

        Dictionary<UIElement, PropertyChangeNotifier> mapVisibilityChangeNotifier = new Dictionary<UIElement, PropertyChangeNotifier>();
        Dictionary<UIElement, bool> mapVisibilityChangeWasVisibile = new Dictionary<UIElement, bool>();
        void UnsubscribeVisibilityChanger(UIElement child, String name)
        {
            if (!mapVisibilityChangeNotifier.ContainsKey(child))
                return;
            mapVisibilityChangeNotifier[child].Dispose();
            mapVisibilityChangeNotifier.Remove(child);
        }

        bool bSettingVisibility;
        void SubscribeVisibilityChanger(UIElement child, String name)
        {
            if (!mapVisibilityChangeNotifier.ContainsKey(child))
            {
                var propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.VisibilityProperty, typeof(UIElement));
                var notifierVisibility = new PropertyChangeNotifier(child, propDesc.Name);
                notifierVisibility.ValueChanged += (o, e) =>
                {
                    if (!bSettingVisibility)
                    {
                        if (mapVisibilityChangeWasVisibile.ContainsKey(child))
                            mapVisibilityChangeWasVisibile.Remove(child);
                        mapVisibilityChangeWasVisibile.Add(child, child.Visibility == Visibility.Visible);

                        if (child.Visibility == Visibility.Visible)
                        {
                            if (Document.MapScreenEntities[name].IsInvisibleForSecurity() ||
                                Document.MapScreenEntities[name].IsInvisibleForZoom())
                            {
                                bSettingVisibility = true;
                                child.Visibility = Visibility.Collapsed;
                                bSettingVisibility = false;
                            }
                        }

                        if (!Document.MapScreenEntities[name].IsInvisibleForSecurity() &&
                            !Document.MapScreenEntities[name].IsInvisibleForZoom())
                        {
                            if (mapVisibilityChangeNotifier.ContainsKey(child))
                            {
                                mapVisibilityChangeNotifier[child].Dispose();
                                mapVisibilityChangeNotifier.Remove(child);
                            }
                        }
                    }
                };

                mapVisibilityChangeNotifier.Add(child, notifierVisibility);
            }
        }

        private void SetVisibileItems(int accessMask)
        {
            if (bRunningOnServerCtor)
                return;

            listAccessEntities.ForEach(name =>
                {
                    var child = Document.FindInnerControl(MainSurface, name);
                    if (child != null)
                    {
                        UnsubscribeVisibilityChanger(child, name);

                        if (Document.MapScreenEntities[name].HasReadableAccess(accessMask))
                        {
                            Document.MapScreenEntities[name].SetInvisibleForSecurity(false);
                            if (!Document.MapScreenEntities[name].IsInvisibleForZoom() &&
                                (!mapVisibilityChangeWasVisibile.ContainsKey(child) ||
                                 mapVisibilityChangeWasVisibile[child] == true))
                            {
                                child.Visibility = Visibility.Visible;

                                Document.MapScreenEntities[name].ReexecuteVisibility();
                            }

                            if (Document.MapScreenEntities[name].IsInvisibleForZoom())
                                SubscribeVisibilityChanger(child, name);
                        }
                        else
                        {
                            if (child.Visibility == Visibility.Visible)
                            {
                                if (mapVisibilityChangeWasVisibile.ContainsKey(child))
                                    mapVisibilityChangeWasVisibile.Remove(child);
                                mapVisibilityChangeWasVisibile.Add(child, true);
                            }
                            Document.MapScreenEntities[name].SetInvisibleForSecurity(true);
                            child.Visibility = Visibility.Collapsed;

                            SubscribeVisibilityChanger(child, name);
                        }

#if !WINDOWS_UWP
                        if (Document.MapScreenEntities[name].HasWritableAccess(accessMask))
                        {
                            CommandManager.CommandManager.SetIsAccessDenied(child, false);
                            AnimationManager.AnimationManager.SetIsAccessDenied(child, false);
                            child.IsEnabled = true;
                        }
                        else
                        {
                            CommandManager.CommandManager.SetIsAccessDenied(child, true);
                            AnimationManager.AnimationManager.SetIsAccessDenied(child, true);
                            child.IsEnabled = false;
                        }
#endif
                        Document.MapScreenEntities[name].EvaluateAccessLevel();
                        Document.MapScreenEntities[name].ReexecuteEnable();
                    }
                });
        }

        private void SetAccessItems(String role, int level)
        {
            listAccessLevelEntities.ForEach(name =>
            {
                var element = Document.FindInnerControl(MainSurface, name);
                if (element == null)
                    return;
                SetIsAccessDeniedPropertyValue(element, role, level);
            });
        }

        private void SetIsAccessDeniedPropertyValue(UIElement element, String role, int level)
        {
            if (!mapAccessLevelEntities.ContainsKey(element) || !Document.MapScreenEntities.ContainsKey(mapAccessLevelEntities[element]))
                return;

            var bHasAccess = Document.MapScreenEntities[mapAccessLevelEntities[element]].HasAccessLevel(role, level);
            CommandManager.CommandManager.SetIsAccessDenied(element, !bHasAccess);
            //AnimationManager.AnimationManager.SetIsAccessDenied(element, !bHasAccess);
            //if (bHasAccess)
            //    Document.MapScreenEntities[mapAccessLevelEntities[element]].ReexecuteEnable();
        }

        void SetVisibleItemsOnServer()
        {
            if (!bRunningOnServerCtor)
                return;

            if (listAccessEntities.Count > 0)
            {
                var accessMask = ScreenDocument.GetAccessMask(MainSurface);

                listAccessEntities.ForEach(name =>
                {
                    var child = Document.FindInnerControl(MainSurface, name);
                    if (child != null)
                    {
                        if (!Document.MapScreenEntities[name].HasReadableAccess(accessMask))
                            child.Visibility = Visibility.Collapsed;
#if !WINDOWS_UWP
                        else if (!Document.MapScreenEntities[name].HasWritableAccess(accessMask))
                        {
                            child.IsEnabled = false;
                            CommandManager.CommandManager.SetIsAccessDenied(child, !child.IsEnabled);
                            AnimationManager.AnimationManager.SetIsAccessDenied(child, !child.IsEnabled);
                        }
#endif
                    }
                });
            }

            if (listAccessLevelEntities.Count > 0)
            {
                var accessRole = ScreenDocument.GetAccessRole(MainSurface);
                var accessLevel = ScreenDocument.GetAccessLevel(MainSurface);

                listAccessLevelEntities.ForEach(name =>
                {
                    var child = Document.FindInnerControl(MainSurface, name);
                    if (child != null)
                    {
                        if (!Document.MapScreenEntities[name].HasAccessLevel(accessRole, accessLevel))
                            child.Visibility = Visibility.Collapsed;
                    }
                });
            }
        }

        List<String> pendingZoomVisibilityAnimation = new List<String>();
        private void SetZoomVisibilityItemsBackground(bool bAnimated = true)
        {
            if (listZoomVisibilityEntities == null)
                return;

            listZoomVisibilityEntities.ForEach(name =>
                {
                    var child = Document.FindInnerControl(MainSurface, name);
                    if (child != null)
                    {
                        UnsubscribeVisibilityChanger(child, name);

                        if (ZoomLevelX > Document.MapScreenEntities[name].ZoomLevelVisibilityX &&
                            ZoomLevelY > Document.MapScreenEntities[name].ZoomLevelVisibilityY)
                        {
                            Document.MapScreenEntities[name].SetInvisibleForZoom(false);
                            if (!Document.MapScreenEntities[name].IsInvisibleForSecurity() &&
                                (!mapVisibilityChangeWasVisibile.ContainsKey(child) ||
                                 mapVisibilityChangeWasVisibile[child] == true))
                            {
                                if (child.Visibility == Visibility.Collapsed)
                                {
                                    /*
                                    if (bAnimated)
                                    {
                                        if (!pendingZoomVisibilityAnimation.Contains(name))
                                            pendingZoomVisibilityAnimation.Add(name);
                                        child.Fade(1, 500, new SineEase() { EasingMode = EasingMode.EaseOut },
                                            (o, e) =>
                                            {
                                                if (pendingZoomVisibilityAnimation.Contains(name))
                                                    pendingZoomVisibilityAnimation.Remove(name);
                                            });
                                    }
                                    else
                                    */
                                    {
                                        child.Visibility = Visibility.Visible;
                                        Document.MapScreenEntities[name].ReexecuteVisibility();
                                    }
                                }
                            }

                            if (Document.MapScreenEntities[name].IsInvisibleForSecurity())
                                SubscribeVisibilityChanger(child, name);
                        }
                        else
                        {
                            if (child.Visibility == Visibility.Visible)
                            {
                                if (mapVisibilityChangeWasVisibile.ContainsKey(child))
                                    mapVisibilityChangeWasVisibile.Remove(child);
                                mapVisibilityChangeWasVisibile.Add(child, true);
                            }

                            Document.MapScreenEntities[name].SetInvisibleForZoom(true);
#if !WINDOWS_UWP
                            if (mapManAdorners.ContainsKey(child))
                            {
                                AdornerLayer adorner = AdornerLayer.GetAdornerLayer(child);
                                if (adorner == null)
                                    adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                                if (adorner != null)
                                {
                                    adorner.Remove(mapManAdorners[child]);
                                }
                                mapManAdorners.Remove(child);
                            }
#endif
                            child.Visibility = Visibility.Collapsed;
                            SubscribeVisibilityChanger(child, name);
                        }
                    }
                });
        }

#if !WINDOWS_UWP
        DispatcherOperation IdleExecutionPending;
        DispatcherOperation IdlePendingExecutionPending;
#endif
        private void SetZoomVisibilityItems()
        {
            if (listZoomVisibilityEntities == null || listZoomVisibilityEntities.Count == 0)
                return;

            if (pendingZoomVisibilityAnimation.Count > 0)
            {
#if !WINDOWS_UWP
                if (IdlePendingExecutionPending != null)
                    return;
#endif
                Action action = () => SetZoomVisibilityItems();
#if !WINDOWS_UWP
                IdlePendingExecutionPending = Dispatcher.BeginInvoke(action, DispatcherPriority.DataBind);

                IdlePendingExecutionPending.Completed += (sender, e) => IdlePendingExecutionPending = null;
#else
                RunOnUIThread.Run(action);
#endif
            }
            else
            {
#if !WINDOWS_UWP
                if (IdleExecutionPending == null)
#endif
                {
                    Action action = () => SetZoomVisibilityItemsBackground();
#if !WINDOWS_UWP
                    IdleExecutionPending = Dispatcher.BeginInvoke(action, DispatcherPriority.DataBind);

                    IdleExecutionPending.Completed += (sender, e) => IdleExecutionPending = null;
#else
                    RunOnUIThread.Run(action);
#endif
                }
            }
        }

#if !WINDOWS_UWP
        DispatcherOperation IdleExecutionPendingCached;
        DispatcherOperation IdlePendingExecutionPendingCached;
#endif
        private void SetZoomCachedItems()
        {
            if (mapElementCacheMode == null || mapElementCacheMode.Count == 0)
                return;

#if !WINDOWS_UWP
            if (IdleExecutionPendingCached == null)
#endif
            {
                Action action = () => SetZoomCachedItemsBackground();
#if !WINDOWS_UWP
                IdleExecutionPendingCached = Dispatcher.BeginInvoke(action, DispatcherPriority.DataBind);

                IdleExecutionPendingCached.Completed += (sender, e) => IdleExecutionPendingCached = null;
#else
                RunOnUIThread.Run(action);
#endif
            }
        }

        private void SetZoomCachedItemsBackground()
        {
            bool bCacheDisabled = ZoomLevelX > Document.ZoomLevelCacheModeX &&
                                  ZoomLevelY > Document.ZoomLevelCacheModeY;

            foreach (var entry in mapElementCacheMode)
            {
                if (bCacheDisabled)
                {
                    if (entry.Key.CacheMode != null)
                        entry.Key.CacheMode = null;
                }
                else
                {
                    if (entry.Key.CacheMode == null)
                        entry.Key.CacheMode = mapElementCacheMode[entry.Key];
                }
            }
        }

#if !WINDOWS_UWP
        private void AddMouseOverToControl(UIElement child)
        {
            if (child is Panel)
            {
                var panel = child as Panel;
                foreach (UIElement el in panel.Children)
                    AddMouseOverToControl(el);
            }
            else
            {
                child.SetTransform<ScaleTransform>(true);

                if (listPropertyChangeNotifier == null)
                    listPropertyChangeNotifier = new List<PropertyChangeNotifier>();
                DependencyPropertyDescriptor propDesc = DependencyPropertyDescriptor.FromProperty(UIElement.IsMouseOverProperty, typeof(UIElement));
                var notifier = new PropertyChangeNotifier(child, propDesc.Name);
                notifier.ValueChanged += (o, e) =>
                    {
                        if (child.IsMouseOver)
                        {
                            //Storyboard.SetTarget(sbmouseOver, child);
                            //sbmouseOver.Begin();
                            child.Scale(1.3, 500, false, false, new ElasticEase());
                        }
                        else
                        {
                            //Storyboard.SetTarget(sbmouseLeave, child);
                            //sbmouseLeave.Begin();
                            child.Scale(1, 500, false, false, new SineEase(), false, (ob, ev) =>
                                {
                                    child.Scale(1, -1, false, false, new ElasticEase());
                                });
                        }
                    };
                listPropertyChangeNotifier.Add(notifier);
            }
        }
#endif
        //double savedMainSurfaceWidth = Double.NaN;
        //double savedMainSurfaceHeight = Double.NaN;
        //private void ScreenViewerControl_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (!bInitialized)
        //        return;

        //    if (MainSurface.ActualWidth < LayoutGrid.ActualWidth &&
        //        MainSurface.Width != Double.NaN)
        //        MainSurface.Width = LayoutGrid.ActualWidth - 30;
        //    else
        //        MainSurface.Width = savedMainSurfaceWidth;

        //    if (MainSurface.ActualHeight < LayoutGrid.ActualHeight &&
        //        MainSurface.Height != Double.NaN)
        //        MainSurface.Height = LayoutGrid.ActualHeight - 30;
        //    else
        //        MainSurface.Height = savedMainSurfaceHeight;
        //}

        private void CleanSurface()
        {
#if !WINDOWS_UWP
            foreach (var uie in MainSurface.Children)
            {
                if (uie is FrameworkElement)
                    Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, uie as FrameworkElement);
            }
#endif
            MainSurface.Children.Clear();
        }

        #region Properties

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        internal Uri CurrentUri
        {
            get
            {
                return Uri;
            }
        }

        ScreenDocument _Document;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public ScreenDocument Document
        {
            get
            {
                return _Document;
            }
            private set
            {
                _Document = value;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public Canvas Surface
        {
#if !WINDOWS_UWP
            get
            {
                Canvas cv = new Canvas
                {
                    Height = MainSurface.Height,
                    Width = MainSurface.Width,
                    Background = MainSurface.Background,
                    Resources = MainSurface.Resources
                };

                NameScope.SetNameScope(cv, new NameScope());

                foreach (FrameworkElement uie in MainSurface.Children)
                {
                    String xamlData = uie.XamlWriterFormatted();
                    FrameworkElement element = null;
                    try
                    {
                        element = xamlData.ReadUIElement() as FrameworkElement;
                    }
                    catch (Exception ex)
                    {
                        ScreenComponent.UIInterface.ShowError(String.Format("{0} : {1}", uie.Name, ex.Message));
                    }
                    if (element == null)
                        continue;
                    //just add the element, it wasn't another Canvas that we
                    //added to the dataobject as a container
                    element.Width = uie.ActualWidth;
                    element.Height = uie.ActualHeight;

                    //if (MainSurface is InkCanvas)
                    //{
                    //    Canvas.SetLeft(element, InkCanvas.GetLeft(uie));
                    //    Canvas.SetTop(element, InkCanvas.GetTop(uie));
                    //    element.SetValue(InkCanvas.TopProperty, DependencyProperty.UnsetValue);
                    //    element.SetValue(InkCanvas.LeftProperty, DependencyProperty.UnsetValue);
                    //}
                    //else
                    {
                        Canvas.SetLeft(element, Canvas.GetLeft(uie));
                        Canvas.SetTop(element, Canvas.GetTop(uie));
                    }
                    try
                    {
                        cv.Children.Add(element);
                    }
                    catch (Exception ex)
                    {
                        ScreenComponent.UIInterface.ShowError(String.Format("{0} : {1}", uie.Name, ex.Message));
                    }

                    if (element is FrameworkElement)
                        Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, element as FrameworkElement, false, false);
                }

                return cv;
            }
#endif
            set
            {
#if !WINDOWS_UWP
                var visibilityMain = MainSurface.Visibility;
                MainSurface.Visibility = Visibility.Collapsed;
                var visibilityLayout = layoutItems.Visibility;
                layoutItems.Visibility = Visibility.Collapsed;

                CleanSurface();

                // int nFadeMs = 500;
                if (value != null)
                {
                    MainSurface.Background = value.Background;
                    if (!(MainSurface.Background is VisualBrush))
                        LayoutGrid.Background = MainSurface.Background;
                    if (Document.LayoutView)
                    {
                        layoutItems.Height = value.Height;
                        layoutItems.Width = value.Width;
                    }
                    else
                    {
                        MainSurface.Height = value.Height;
                        MainSurface.Width = value.Width;
                    }

                    if (!Double.IsNaN(Document.Width) && Document.Width > 0)
                        progressLoading.Width = MainSurface.Width = Document.Width;
                    if (!Double.IsNaN(Document.Height) && Document.Height > 0)
                        MainSurface.Height = Document.Height;

                    //this.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    //this.Arrange(new Rect(new Point(0, 0), new Size(MainSurface.Width, MainSurface.Height)));
                    //this.Refresh();

                    listConnectionString = new List<FrameworkElement>();
                    var listAlarm = new List<FrameworkElement>();
                    var listEventConnectionString = new List<FrameworkElement>();
                    var listScheduler = new List<FrameworkElement>();
                    while (value.Children.Count > 0)
                    {
                        UIElement child = value.Children[0] as UIElement;

                        //if (MainSurface is InkCanvas)
                        //{
                        //    double left = Canvas.GetLeft(child);
                        //    double top = Canvas.GetTop(child);

                        //    left = double.IsNaN(left) ? 0 : left;
                        //    top = double.IsNaN(top) ? 0 : top;

                        //    InkCanvas.SetLeft(child, left);
                        //    InkCanvas.SetTop(child, top);
                        //    child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                        //    child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                        //}

                        value.Children.Remove(child);
                        if (Document.LayoutView)
                            layoutItems.AvailableItems.Add(child as FrameworkElement);
                        else
                            MainSurface.Children.Add(child);

                        if (child is FrameworkElement)
                        {
                            var fe = child as FrameworkElement;
                            if (Document.LayoutView)
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(layoutItems, fe, false, false);
                            else
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, fe, false, false);

                            ScreenDocument.CheckPreBinding(listAlarm, listConnectionString, listEventConnectionString,
                                listScheduler, fe);
                            //fe.GetChildrenOfType<FrameworkElement>().ToList().ForEach(element =>
                            //{
                            //    ScreenDocument.CheckPreBinding(listAlarm, listConnectionString, listEventConnectionString,
                            //        listScheduler, element);
                            //});
                        }
                        /*
                        var setTo = child.Opacity;
                        child.Opacity = 0;
                        if (value.Children.Count > 0)
                        {
                            child.Fade(setTo, nFadeMs, new BackEase() { EasingMode = EasingMode.EaseInOut });
                            nFadeMs += 200;
                        }
                        else
                            child.Fade(setTo, nFadeMs,
                                new BackEase() { EasingMode = EasingMode.EaseInOut });
                         * */
                    }

                    UpdateBinding(listAlarm, listEventConnectionString, listScheduler);
                    // Action action1 = () =>
                    //     {

                    //Document.PrepareExecution(MainSurface, Document.SessionString, 
                    //    Document.MapScreenEntities.Keys.ToList());
                    //     };
                    // Dispatcher.BeginInvoke(action1, DispatcherPriority.DataBind);
                }

                MainSurface.Visibility = visibilityMain;
                layoutItems.Visibility = visibilityLayout;
#else
                MainSurface = value;
                ScreenDocument.SetScreenDocument(MainSurface, Document);
                Scroller.Content = MainSurface;

                var trasformGroup = new TransformGroup();
                MainSurface.RenderTransform = trasformGroup;
                var scaleTransform = new ScaleTransform();
                var translateTranform = new TranslateTransform();
                trasformGroup.Children.Add(scaleTransform);
                trasformGroup.Children.Add(translateTranform);

                var bindingScaleX = new Binding() { Source = this, Path = new PropertyPath("ZoomLevelX") };
                BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, bindingScaleX);
                var bindingScaleY = new Binding() { Source = this, Path = new PropertyPath("ZoomLevelY") };
                BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, bindingScaleY);

                var bindingX = new Binding() { Source = this, Path = new PropertyPath("OffsetLevelX") };
                BindingOperations.SetBinding(scaleTransform, TranslateTransform.XProperty, bindingX);
                var bindingY = new Binding() { Source = this, Path = new PropertyPath("OffsetLevelY") };
                BindingOperations.SetBinding(scaleTransform, TranslateTransform.YProperty, bindingY);

                //< TransformGroup >
                //    < ScaleTransform ScaleX = "{Binding ElementName=ScreenViewerControl, Path=ZoomLevelX}"
                //                    ScaleY = "{Binding ElementName=ScreenViewerControl, Path=ZoomLevelY}" />
                //    < TranslateTransform X = "{Binding ElementName=ScreenViewerControl, Path=OffsetLevelX}"
                //                        Y = "{Binding ElementName=ScreenViewerControl, Path=OffsetLevelY}" />
                //</ TransformGroup >

                /*
                var listAlarm = new List<FrameworkElement>();
                var listConnectionString = new List<FrameworkElement>();
                var listEventConnectionString = new List<FrameworkElement>();
                var listScheduler = new List<FrameworkElement>();
                */
                //foreach(var child in MainSurface.Children)
                //{
                //    if (child is FrameworkElement)
                //    {
                //        var fe = child as FrameworkElement;

                //        ScreenDocument.CheckPreBinding(listAlarm, listConnectionString, listEventConnectionString,
                //            listScheduler, fe);
                //        fe.GetChildrenOfType<FrameworkElement>().ToList().ForEach(element =>
                //        {
                //            ScreenDocument.CheckPreBinding(listAlarm, listConnectionString, listEventConnectionString,
                //                listScheduler, element);
                //        });
                //    }
                //}
#endif
            }
        }

#if !WINDOWS_UWP
        public void UpdateBinding(List<FrameworkElement> listAlarm,
            List<FrameworkElement> listEventConnectionString,
            List<FrameworkElement> listScheduler)
        {
            if (listConnectionString.Count > 0)
            {
                Document.PreBindConnectionStringSource(listConnectionString, ScreenComponent.UFUAEditor.GetHistorianDefaultConnection(Document));
            }
            if (listEventConnectionString.Count > 0)
            {
                var settings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(ScreenComponent.UFUAEditor.GetEventDefaultConnection(Document), Document.SessionString);
                Document.PreBindConnectionStringSource(listEventConnectionString, settings);
            }
            if (listAlarm.Count > 0)
            {
                var server = ScreenComponent.UFUAEditor.GetServerEntityReference(Document);
                if (server != null)
                    Document.PreBindAlarmSource(listAlarm, server);
            }
            if (listScheduler.Count > 0)
            {
                var scheduler = ScreenComponent.SchedulerEditor.GetServerEntityReference(Document);
                if (scheduler != null)
                    Document.PreBindSchedulerSource(listScheduler, scheduler);
            }
        }
#endif
        public static readonly DependencyProperty ZoomLevelXProperty = DependencyProperty.Register("ZoomLevelX",
            typeof(double), typeof(ScreenViewer),
#if !WINDOWS_UWP
            new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnZoomLevelXChanged), new CoerceValueCallback(OnCoerceZoomLevelX)));
#else
            new PropertyMetadata(1.0));
#endif
        private static object OnCoerceZoomLevelX(DependencyObject o, object value)
        {
            ScreenViewer screenViewer = o as ScreenViewer;
            if (screenViewer != null)
                return screenViewer.OnCoerceZoomLevelX((double)value);
            else
                return value;
        }

        private static void OnZoomLevelXChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScreenViewer screenViewer = o as ScreenViewer;
            if (screenViewer != null)
                screenViewer.OnZoomLevelXChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceZoomLevelX(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnZoomLevelXChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!bZoomAnimating)
            {
                SetZoomVisibilityItems();
                SetZoomCachedItems();
            }
            UpdateScrollbarsVisibility();
        }

        public double ZoomLevelX
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(ZoomLevelXProperty);
            }
            set
            {
                if (Document != null && Document.DisableZoom && !bUpdatingFitInWindow)
                    return;
                SetValue(ZoomLevelXProperty, value);
            }
        }


        public static readonly DependencyProperty ZoomLevelYProperty = DependencyProperty.Register("ZoomLevelY", typeof(double), typeof(ScreenViewer),
#if !WINDOWS_UWP
            new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnZoomLevelYChanged), new CoerceValueCallback(OnCoerceZoomLevelY)));
#else
            new PropertyMetadata(1.0));
#endif
        private static object OnCoerceZoomLevelY(DependencyObject o, object value)
        {
            ScreenViewer screenViewer = o as ScreenViewer;
            if (screenViewer != null)
                return screenViewer.OnCoerceZoomLevelY((double)value);
            else
                return value;
        }

        private static void OnZoomLevelYChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScreenViewer screenViewer = o as ScreenViewer;
            if (screenViewer != null)
                screenViewer.OnZoomLevelYChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceZoomLevelY(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnZoomLevelYChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!bZoomAnimating)
            {
                SetZoomVisibilityItems();
                SetZoomCachedItems();
            }
            UpdateScrollbarsVisibility();
        }

        public double ZoomLevelY
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(ZoomLevelYProperty);
            }
            set
            {
                if (Document != null && Document.DisableZoom && !bUpdatingFitInWindow)
                    return;
                SetValue(ZoomLevelYProperty, value);
            }
        }

        public static readonly DependencyProperty OffsetLevelXProperty = DependencyProperty.Register("OffsetLevelX", typeof(double), typeof(ScreenViewer),
#if !WINDOWS_UWP
            new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnOffsetLevelXChanged), new CoerceValueCallback(OnCoerceOffsetLevelX)));
#else
            new PropertyMetadata(0.0));
#endif
        private static object OnCoerceOffsetLevelX(DependencyObject o, object value)
        {
            ScreenViewer screenViewer = o as ScreenViewer;
            if (screenViewer != null)
                return screenViewer.OnCoerceOffsetLevelX((double)value);
            else
                return value;
        }

        private static void OnOffsetLevelXChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScreenViewer screenViewer = o as ScreenViewer;
            if (screenViewer != null)
                screenViewer.OnOffsetLevelXChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceOffsetLevelX(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnOffsetLevelXChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double OffsetLevelX
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(OffsetLevelXProperty);
            }
            set
            {
                SetValue(OffsetLevelXProperty, value);
            }
        }

        public static readonly DependencyProperty OffsetLevelYProperty = DependencyProperty.Register("OffsetLevelY", typeof(double), typeof(ScreenViewer),
#if !WINDOWS_UWP
            new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnOffsetLevelYChanged), new CoerceValueCallback(OnCoerceOffsetLevelY)));
#else
            new PropertyMetadata(0.0));
#endif

        private static object OnCoerceOffsetLevelY(DependencyObject o, object value)
        {
            ScreenViewer screenViewer = o as ScreenViewer;
            if (screenViewer != null)
                return screenViewer.OnCoerceOffsetLevelY((double)value);
            else
                return value;
        }

        private static void OnOffsetLevelYChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ScreenViewer screenViewer = o as ScreenViewer;
            if (screenViewer != null)
                screenViewer.OnOffsetLevelYChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceOffsetLevelY(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnOffsetLevelYChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double OffsetLevelY
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(OffsetLevelYProperty);
            }
            set
            {
                SetValue(OffsetLevelYProperty, value);
            }
        }


        bool bZoomAnimating;
        public void ZoomIn()
        {
            if (Document == null || Document.DisableZoom)
                return;

            var animationx = new DoubleAnimation();
            animationx.To = ZoomLevelX + 0.5;
            animationx.Duration = new Duration(TimeSpan.FromSeconds(1));
            animationx.EasingFunction = new SineEase();
            Storyboard.SetTarget(animationx, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animationx, new PropertyPath("ZoomLevelX"));
#else
            Storyboard.SetTargetProperty(animationx, "ZoomLevelX");
#endif
            Storyboard sb = new Storyboard();
            sb.Children.Add(animationx);

            var animationy = new DoubleAnimation();
            animationy.To = ZoomLevelY + 0.5;
            animationy.Duration = new Duration(TimeSpan.FromSeconds(1));
            animationy.EasingFunction = new SineEase();
            Storyboard.SetTarget(animationy, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animationy, new PropertyPath("ZoomLevelY"));
#else
            Storyboard.SetTargetProperty(animationy, "ZoomLevelY");
#endif
            sb.Children.Add(animationy);
            bZoomAnimating = true;
            sb.Begin();
            sb.Completed += (o1, e1) =>
            {
                bZoomAnimating = false;
                SetZoomVisibilityItems();
                SetZoomCachedItems();
            };
        }

        public void ZoomOut()
        {
            if (Document == null || Document.DisableZoom)
                return;

            var animationx = new DoubleAnimation();
            animationx.To = ZoomLevelX - 0.5;
            animationx.Duration = new Duration(TimeSpan.FromSeconds(1));
            animationx.EasingFunction = new SineEase();
            Storyboard.SetTarget(animationx, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animationx, new PropertyPath("ZoomLevelX"));
#else
            Storyboard.SetTargetProperty(animationx, "ZoomLevelX");
#endif
            Storyboard sb = new Storyboard();
            sb.Children.Add(animationx);

            var animationy = new DoubleAnimation();
            animationy.To = ZoomLevelY - 0.5;
            animationy.Duration = new Duration(TimeSpan.FromSeconds(1));
            animationy.EasingFunction = new SineEase();
            Storyboard.SetTarget(animationy, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animationy, new PropertyPath("ZoomLevelY"));
#else
            Storyboard.SetTargetProperty(animationy, "ZoomLevelY");
#endif
            sb.Children.Add(animationy);
            bZoomAnimating = true;
            sb.Begin();
            sb.Completed += (o1, e1) =>
            {
                bZoomAnimating = false;
                SetZoomVisibilityItems();
                SetZoomCachedItems();
            };
        }

        public void ResetZoom()
        {
            if (Document == null || Document.DisableZoom)
                return;

            var animationx = new DoubleAnimation();
            animationx.To = 1.0;
            animationx.Duration = new Duration(TimeSpan.FromSeconds(1));
            animationx.EasingFunction = new SineEase();
            Storyboard.SetTarget(animationx, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animationx, new PropertyPath("ZoomLevelX"));
#else
            Storyboard.SetTargetProperty(animationx, "ZoomLevelX");
#endif
            Storyboard sb = new Storyboard();
            sb.Children.Add(animationx);

            var animationy = new DoubleAnimation();
            animationy.To = 1.0;
            animationy.Duration = new Duration(TimeSpan.FromSeconds(1));
            animationy.EasingFunction = new SineEase();
            Storyboard.SetTarget(animationy, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animationy, new PropertyPath("ZoomLevelY"));
#else
            Storyboard.SetTargetProperty(animationy, "ZoomLevelY");
#endif
            sb.Children.Add(animationy);
            bZoomAnimating = true;
            sb.Begin();
            sb.Completed += (o1, e1) =>
            {
                bZoomAnimating = false;
                SetZoomVisibilityItems();
                SetZoomCachedItems();
            };
        }

        public void ScrollRight()
        {
            var animation = new DoubleAnimation();
            animation.To = OffsetLevelX + 0.5;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.EasingFunction = new SineEase();
            Storyboard.SetTarget(animation, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animation, new PropertyPath("OffsetLevelX"));
#else
            Storyboard.SetTargetProperty(animation, "OffsetLevelX");
#endif
            Storyboard sb = new Storyboard();
            sb.Children.Add(animation);
            sb.Begin();
        }

        public void ScrollLeft()
        {
            var animation = new DoubleAnimation();
            animation.To = OffsetLevelX - 0.5;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.EasingFunction = new SineEase();
            Storyboard.SetTarget(animation, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animation, new PropertyPath("OffsetLevelX"));
#else
            Storyboard.SetTargetProperty(animation, "OffsetLevelX");
#endif
            Storyboard sb = new Storyboard();
            sb.Children.Add(animation);
            sb.Begin();
        }

        public void ScrollDown()
        {
            var animation = new DoubleAnimation();
            animation.To = OffsetLevelY + 0.5;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.EasingFunction = new SineEase();
            Storyboard.SetTarget(animation, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animation, new PropertyPath("OffsetLevelY"));
#else
            Storyboard.SetTargetProperty(animation, "OffsetLevelY");
#endif
            Storyboard sb = new Storyboard();
            sb.Children.Add(animation);
            sb.Begin();
        }

        public void ScrollUp()
        {
            var animation = new DoubleAnimation();
            animation.To = OffsetLevelY - 0.5;
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            animation.EasingFunction = new SineEase();
            Storyboard.SetTarget(animation, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animation, new PropertyPath("OffsetLevelY"));
#else
            Storyboard.SetTargetProperty(animation, "OffsetLevelY");
#endif
            Storyboard sb = new Storyboard();
            sb.Children.Add(animation);
            sb.Begin();
        }

        /// <summary>
        /// An Adorner which is used for displaying the gesture feeback.
        /// </summary>
#if !WINDOWS_UWP
        [Browsable(false)]
        private MessageAdorner GestureResultAdorner
        {
            get
            {
                // Initialize the MessageAdorner if it isn't created yet.
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(MainSurface);
                if (adornerLayer == null)
                    return null;
                if (_gestureResultAdorner != null)
                    adornerLayer.Remove(_gestureResultAdorner);

                _gestureResultAdorner = new MessageAdorner(MainSurface);

                // The MessageAdorner only needs to be rendered. Disable the HitTest on it.
                _gestureResultAdorner.IsHitTestVisible = false;
                adornerLayer.Add(_gestureResultAdorner);
                return _gestureResultAdorner;
            }
        }
#endif
        #endregion Properties

        #region Methods
        internal void wnd_SizeChanged(object sender, EventArgs e)
        {
            if (!IsInFillMode)
                return;

            UpdateActualSize();

            UpdateZoomFillMode();
        }

        void AnimateZoom(double x, double y, int delay = 200, double finalx = Double.NaN, double finaly = Double.NaN)
        {
            var animationx = new DoubleAnimation();
            animationx.To = x;
            animationx.Duration = new Duration(TimeSpan.FromMilliseconds(delay));
            animationx.EasingFunction = new SineEase();
            animationx.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(animationx, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animationx, new PropertyPath(ScreenViewer.ZoomLevelXProperty));
#else
            Storyboard.SetTargetProperty(animationx, "ZoomLevelX");
#endif
            Storyboard sb = new Storyboard();
            sb.Children.Add(animationx);

            var animationy = new DoubleAnimation();
            animationy.To = y;
            animationy.Duration = new Duration(TimeSpan.FromMilliseconds(delay));
            animationy.EasingFunction = new SineEase();
            animationy.FillBehavior = FillBehavior.Stop;
            Storyboard.SetTarget(animationy, this);
#if !WINDOWS_UWP
            Storyboard.SetTargetProperty(animationy, new PropertyPath(ScreenViewer.ZoomLevelYProperty));
#else
            Storyboard.SetTargetProperty(animationy, "ZoomLevelY");
#endif
            sb.Children.Add(animationy);

            sb.Completed += (o, e) =>
            {
                sb.Stop();
                ZoomLevelY = y;
                ZoomLevelX = x;

                UpdateScrollbarsVisibility();

                if (!Double.IsNaN(finalx) && !Double.IsNaN(finaly))
                {
                    ZoomLevelY = finaly;
                    ZoomLevelX = finalx;

                    UpdateScrollbarsVisibility();
                }
            };

            sb.Begin();
        }

        void AnimateToFill()
        {
            double x = 0, y = 0;
            if (MainSurface.ActualWidth > 0)
                x = GetFillZoomFactorX();
            if (MainSurface.ActualHeight > 0)
                y = GetFillZoomFactorY();

            if (ZoomLevelX < x || ZoomLevelY < y)
                AnimateZoom(x, y);
        }

        double GetFillZoomFactorX()
        {
            if (MainSurface.ActualWidth > 0 && Document != null && Document.FitInWindow)
            {
                LayoutGrid.UpdateLayout();

                if (Document.KeepAspectRatio && LayoutGrid.ActualHeight < LayoutGrid.ActualWidth) //factorX must be limited by factorY in order to keep aspect ratio
                    return GetFillZoomFactorY();

#if !WINDOWS_UWP
                var ret = LayoutGrid.ActualWidth / MainSurface.ActualWidth;
#else
                var ret = ActualWidth / MainSurface.ActualWidth;
#endif
                return ret;
            }
            return 1;
        }

        double GetFillZoomFactorY()
        {
            if (MainSurface.ActualHeight > 0 && Document != null && Document.FitInWindow)
            {
                LayoutGrid.UpdateLayout();

                if (Document.KeepAspectRatio && LayoutGrid.ActualHeight > LayoutGrid.ActualWidth) //factorY must be limited by factorX in order to keep aspect ratio
                    return GetFillZoomFactorX();

#if !WINDOWS_UWP
                var ret = LayoutGrid.ActualHeight / MainSurface.ActualHeight;
#else
                var ret = ActualHeight / MainSurface.ActualHeight;
#endif
                return ret;
            }
            return 1;
        }

        bool bUpdatingFitInWindow;
        void UpdateZoomFillMode()
        {
            bUpdatingFitInWindow = true;
            try
            {
                if (Document.FitInWindow && Document.ResizeControlsOnFit)
                {
                    ResizeContent();
                    ZoomLevelX = 1;
                    ZoomLevelY = 1;
                    Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    Scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    return;
                }
                else
                {
                    if (MainSurface.ActualWidth > 0)
                        ZoomLevelX = GetFillZoomFactorX();
                    if (MainSurface.ActualHeight > 0)
                        ZoomLevelY = GetFillZoomFactorY();
                }

                //Working with bars, the combination of KeepAspectRatio and FitInWindow must be taken into account
                if (Document.KeepAspectRatio && Document.FitInWindow)
                {
                    //Identy that this is a bar,then apply the same scaling factor
                    if (isTopBottomBar)
                        ZoomLevelX = ZoomLevelY = LayoutGrid.ActualWidth / MainSurface.ActualWidth;
                    
                    if (isLeftRightBar)
                        ZoomLevelX = ZoomLevelY = LayoutGrid.ActualHeight / MainSurface.ActualHeight;
                    }
            }
            finally
            {
                bUpdatingFitInWindow = false;
            }
        }

        void UpdateScrollbarsVisibility()
        {
            Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            if (!IsInFillMode)
                return;
            if (ZoomLevelY == GetFillZoomFactorY() || isTopBottomBar || isLeftRightBar)
                Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            if (ZoomLevelX == GetFillZoomFactorX() || isTopBottomBar || isLeftRightBar)
                Scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        void UpdateActualSize()
        {
            if (Document.FitInWindow && Document.ResizeControlsOnFit)
            {
                MainSurface.Width = LayoutGrid.ActualWidth;
                MainSurface.Height = LayoutGrid.ActualHeight;
            }
        }
    
        DelayedSingleActionInvoker delayInvokerResize;
        Dictionary<FrameworkElement, Rect> mapOriginalRect;
        Dictionary<UIElement, PointCollection> mapOriginalPoints;
        void ResizeContent()
        {
            if (!(Document.FitInWindow && Document.ResizeControlsOnFit) || MainSurface.Children.Count == 0 ||
                double.IsNaN(MainSurface.ActualWidth) || double.IsNaN(MainSurface.ActualHeight) || 
                double.IsNaN(Document.Height) || double.IsNaN(Document.Width) || Document.Width == 0 || Document.Height == 0)
            {
                return;
            }

            if (delayInvokerResize == null)
                delayInvokerResize = new DelayedSingleActionInvoker(() =>
                {
                    if (bDisposed)
                        return;

                    var widthRatio = MainSurface.ActualWidth / Document.Width;
                    var heightRatio = MainSurface.ActualHeight / Document.Height;

                    if (mapOriginalRect == null)
                        mapOriginalRect = new Dictionary<FrameworkElement, Rect>();

                    foreach (FrameworkElement element in MainSurface.Children)
                    {
                        if (!mapOriginalRect.ContainsKey(element))
                            mapOriginalRect.Add(element, new Rect(Canvas.GetLeft(element), Canvas.GetTop(element),
                                element.Width, element.Height));
                        var factor = mapOriginalRect[element].Y * heightRatio;
                        Canvas.SetTop(element, factor);
                        factor = mapOriginalRect[element].X * widthRatio;
                        Canvas.SetLeft(element, factor);

                        factor = mapOriginalRect[element].Height * heightRatio;
                        element.Height = factor;
                        factor = mapOriginalRect[element].Width * widthRatio;
                        element.Width = factor;

                        TransformsPoints(element, widthRatio, heightRatio);
                    }
                }, TimeSpan.FromMilliseconds(50));

            delayInvokerResize.BeginInvoke();
        }

        void TransformsPoints(UIElement control, double factorX, double factorY)
        {
            if (mapOriginalPoints == null)
                mapOriginalPoints = new Dictionary<UIElement, PointCollection>();

            if (control is Line)
            {
                var line = control as Line;
                var newPoints = new PointCollection();

                if (!mapOriginalPoints.ContainsKey(control))
                {
                    var points = new PointCollection();
                    points.Add(new Point(line.X1, line.Y1));
                    points.Add(new Point(line.X2, line.Y2));
                    mapOriginalPoints.Add(control, points);
                }

                foreach (var point in mapOriginalPoints[control])
                {
                    newPoints.Add(new Point(point.X * factorX, point.Y * factorY));
                }
                line.X1 = newPoints[0].X;
                line.Y1 = newPoints[0].Y;
                line.X2 = newPoints[1].X;
                line.Y2 = newPoints[1].Y;
            }
            else
            {
                var propertyPoints = control.GetType().GetProperty("Points");
                if (propertyPoints != null)
                {
                    var points = propertyPoints.GetValue(control) as PointCollection;

                    if (!mapOriginalPoints.ContainsKey(control))
                        mapOriginalPoints.Add(control, points);

                    var newPoints = new PointCollection();
                    foreach (var point in mapOriginalPoints[control])
                    {
                        newPoints.Add(new Point(point.X * factorX, point.Y * factorY));
                    }
                    propertyPoints.SetValue(control, newPoints);
                }
            }
        }


#if !WINDOWS_UWP
        private void MainSurface_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Document == null || Document.DisableZoom/* || IsInFillMode*/)
                return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                var deltax = e.Delta > 0 ? ZoomLevelX + 0.1 : ZoomLevelX - 0.1;
                // ZoomLevelX = Math.Max(ZoomLevelX, GetFillZoomFactorX());
                var deltay = e.Delta > 0 ? ZoomLevelY + 0.1 : ZoomLevelY - 0.1;
                // ZoomLevelY = Math.Max(ZoomLevelY, GetFillZoomFactorY());

                var x = GetFillZoomFactorX();
                var y = GetFillZoomFactorY();
                if (deltax < x || deltay < y)
                    AnimateZoom(deltax, deltay, 200, x, y);
                else
                    AnimateZoom(deltax, deltay, 200);

                e.Handled = true;
            }
        }

        bool bStoryBoardStarted;
        private void ShowOrHideToolBar()
        {
            if (IsModal)
                return;

            if (bStoryBoardStarted)
            {
                var storyBoard = TryFindResource("MouseLeaveOpacity") as Storyboard;
                if (storyBoard == null)
                    return;
                storyBoard.Begin();
                bStoryBoardStarted = false;
            }
            else
            {
                var storyBoard = TryFindResource("MouseOverOpacity") as Storyboard;
                if (storyBoard == null)
                    return;
                storyBoard.Begin();
                bStoryBoardStarted = true;
            }
        }

        private void MainSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is UIElement)
            {
                Adorner adorner = null;
                if (e.OriginalSource is Visual || e.OriginalSource is Visual3D)
                    adorner = (e.OriginalSource as DependencyObject).FindParent<Adorner>();
                if (adorner != null)
                    return;

                // var child = e.OriginalSource as UIElement;
                // ActivateManipulationAdorner(child);
                Point endPoint = e.GetPosition(MainSurface);

                for (int i = MainSurface.Children.Count - 1; i >= 0; --i)
                {
                    UIElement uie = MainSurface.Children[i];

                    Rect itemBounds = Rect.Empty;
                    Rect itemRect = VisualTreeHelper.GetDescendantBounds(uie);
                    if (itemRect.IsEmpty || (itemRect.Width == 0 && itemRect.Height == 0))
                    {
                        var fe = uie as FrameworkElement;
                        if (fe != null)
                        {
                            //if (MainSurface is InkCanvas)
                            //{
                            //    itemRect = new Rect(InkCanvas.GetLeft(uie), InkCanvas.GetTop(uie),
                            //        fe.ActualWidth, fe.ActualHeight);
                            //}
                            //else
                            {
                                itemRect = new Rect(Canvas.GetLeft(uie), Canvas.GetTop(uie),
                                    fe.ActualWidth, fe.ActualHeight);
                            }
                            itemBounds = itemRect;
                        }
                    }
                    else
                        itemBounds = uie.TransformToAncestor(MainSurface).TransformBounds(itemRect);

                    if (itemBounds.Contains(endPoint))
                    {
                        ActivateManipulationAdorner(uie, true);
                        break;
                    }
                }
            }
        }

        readonly Dictionary<UIElement, Visibility> mapVisibilityPreZoom = new Dictionary<UIElement, Visibility>();
        // readonly Dictionary<UIElement, double> mapOpacityPreZoom = new Dictionary<UIElement, double>();
        readonly Dictionary<Adorner, Visibility> mapVisibilityAdornerPreZoom = new Dictionary<Adorner, Visibility>();
        Rect preZoomRect;
        UIElement zoomedControl;
        void ZoomControl(UIElement element, bool bSaveData = true)
        {
            var listManipulatedElementsTemp = new List<UIElement>(listManipulatedElements);

            if (bSaveData)
                SaveLayoutData(GetUniqueTitle(Document));

            mapElementToMan.Keys.ToList().ForEach(e =>
            {
                EnableManipulationControl(e);
            });

            if (bSaveData)
            {
                listManipulatedElementsTemp.ForEach(parent =>
                {
                    if (mapTransformPreMan.ContainsKey(parent))
                    {
                        parent.RenderTransform = mapTransformPreMan[parent];
                        mapTransformPreMan.Remove(parent);
                    }
                    else
                        parent.RenderTransform = null;
                });
            }

            mapVisibilityPreZoom.Clear();
            // mapOpacityPreZoom.Clear();
            mapVisibilityAdornerPreZoom.Clear();

            var fe = element as FrameworkElement;
            var p = LogicalTreeHelper.GetParent(fe) as FrameworkElement;
            if (p is ContentControl && (p as ContentControl).Content is UIElement &&
                !(p is UserControl))
                element = p;

            if (!listManipulatedElements.Contains(element))
                listManipulatedElements.Add(element);

            foreach (UIElement control in MainSurface.Children)
            {
                if (control == element)
                    continue;

                var c = control;
                if (c is ContentControl && (c as ContentControl).Content is UIElement &&
                    !(c is UserControl))
                {
                    c = (control as ContentControl).Content as UIElement;
                }

                mapVisibilityPreZoom[c] = c.Visibility;
                //mapOpacityPreZoom[control] = control.Opacity;
                //if (control.Visibility == Visibility.Visible)
                //    control.Fade(0, 250, new SineEase() { EasingMode = EasingMode.EaseOut });
                c.Visibility = Visibility.Collapsed;

                AdornerLayer adorner = AdornerLayer.GetAdornerLayer(c);
                if (adorner == null)
                    adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                if (adorner != null)
                {
                    Adorner[] adornerGetAdorners = adorner.GetAdorners(c);
                    if (adornerGetAdorners != null)
                    {
                        var list = (from a in adornerGetAdorners where a != null select a).ToList();
                        foreach (var a in list)
                        {
                            mapVisibilityAdornerPreZoom[a] = a.Visibility;
                            a.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }

            //if (MainSurface is InkCanvas)
            //{
            //    preZoomRect = new Rect(InkCanvas.GetLeft(element), InkCanvas.GetTop(element),
            //                            fe.ActualWidth, fe.ActualHeight);
            //    element.Animate(InkCanvas.LeftProperty, 50, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            //    element.Animate(InkCanvas.TopProperty, 50, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            //}
            //else
            {
                preZoomRect = new Rect(Canvas.GetLeft(element), Canvas.GetTop(element),
                                        fe.ActualWidth, fe.ActualHeight);
                element.Animate(Canvas.LeftProperty, 50, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
                element.Animate(Canvas.TopProperty, 50, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            }

            var width = Math.Min(MainSurface.ActualWidth, LayoutGrid.ActualWidth);
            var height = Math.Min(MainSurface.ActualHeight, LayoutGrid.ActualHeight);

            element.Animate(FrameworkElement.WidthProperty, width - 100, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            element.Animate(FrameworkElement.HeightProperty, height - 100, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            zoomedControl = element;
        }

        void UnzoomControl(UIElement element)
        {
            var fe = element as FrameworkElement;
            var p = LogicalTreeHelper.GetParent(fe) as FrameworkElement;
            if (p is ContentControl && (p as ContentControl).Content is UIElement &&
                !(p is UserControl))
                element = p;

            foreach (UIElement control in MainSurface.Children)
            {
                if (control == element)
                    continue;

                var c = control;
                if (c is ContentControl && (c as ContentControl).Content is UIElement &&
                    !(c is UserControl))
                {
                    c = (control as ContentControl).Content as UIElement;
                }

                if (mapVisibilityPreZoom.ContainsKey(c))
                {
                    //if (mapVisibilityPreZoom[control] == Visibility.Visible)
                    //    control.Fade(mapOpacityPreZoom[control], 250, new SineEase() { EasingMode = EasingMode.EaseOut });

                    c.Visibility = mapVisibilityPreZoom[c];
                }

                AdornerLayer adorner = AdornerLayer.GetAdornerLayer(c);
                if (adorner == null)
                    adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                if (adorner != null)
                {
                    Adorner[] adornerGetAdorners = adorner.GetAdorners(c);
                    if (adornerGetAdorners != null)
                    {
                        var list = (from a in adornerGetAdorners where a != null select a).ToList();
                        foreach (var a in list)
                        {
                            if (mapVisibilityAdornerPreZoom.ContainsKey(a))
                                a.Visibility = mapVisibilityAdornerPreZoom[a];
                        }
                    }
                }
            }
            mapVisibilityPreZoom.Clear();
            // mapOpacityPreZoom.Clear();
            mapVisibilityAdornerPreZoom.Clear();

            //if (MainSurface is InkCanvas)
            //{
            //    element.Animate(InkCanvas.LeftProperty, preZoomRect.Left, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            //    element.Animate(InkCanvas.TopProperty, preZoomRect.Top, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            //}
            //else
            {
                element.Animate(Canvas.LeftProperty, preZoomRect.Left, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
                element.Animate(Canvas.TopProperty, preZoomRect.Top, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            }

            element.Animate(FrameworkElement.WidthProperty, preZoomRect.Width, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            element.Animate(FrameworkElement.HeightProperty, preZoomRect.Height, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            zoomedControl = null;

            LoadLayoutData(GetUniqueTitle(Document));
        }

        readonly Dictionary<UIElement, Rect> mapRectPreIcon = new Dictionary<UIElement, Rect>();
        void IconControl(UIElement element)
        {
            var fe = element as FrameworkElement;
            if (!(fe is ContentControl))
            {
                var p = LogicalTreeHelper.GetParent(fe) as FrameworkElement;
                if (p is ContentControl && (p as ContentControl).Content is UIElement &&
                    !(p is UserControl))
                    element = p;
            }

            if (mapRectPreIcon.ContainsKey(element))
                return;

            if (mapManAdorners.ContainsKey(element))
            {
                AdornerLayer adorner = AdornerLayer.GetAdornerLayer(element);
                if (adorner == null)
                    adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                if (adorner != null)
                {
                    adorner.Remove(mapManAdorners[element]);
                }
                mapManAdorners.Remove(element);
            }

            if (!listManipulatedElements.Contains(element))
                listManipulatedElements.Add(element);

            var preiconRect = new Rect();
            //if (MainSurface is InkCanvas)
            //{
            //    preiconRect = new Rect(InkCanvas.GetLeft(element), InkCanvas.GetTop(element),
            //                             fe.ActualWidth, fe.ActualHeight);
            //}
            //else
            {
                preiconRect = new Rect(Canvas.GetLeft(element), Canvas.GetTop(element),
                                         fe.ActualWidth, fe.ActualHeight);
            }

            if (mapRectPreIcon.ContainsKey(element))
                mapRectPreIcon.Remove(element);
            mapRectPreIcon.Add(element, preiconRect);

            DataTemplate item = TryFindResource("ItemTemplate") as DataTemplate;
            var btn = item.LoadContent() as Button;

            var width = Math.Min(MainSurface.ActualWidth, LayoutGrid.ActualWidth);
            var height = Math.Min(MainSurface.ActualHeight, LayoutGrid.ActualHeight);
            //if (MainSurface is InkCanvas)
            //{
            //    element.Animate(InkCanvas.LeftProperty, (width / 2) - (iconizer.Height / 2), 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            //    element.Animate(InkCanvas.TopProperty, height - iconizer.Height, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            //}
            //else
            {
                element.Animate(Canvas.LeftProperty, (width / 2) - (iconizer.Height / 2), 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
                element.Animate(Canvas.TopProperty, height - iconizer.Height, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            }

            element.Animate(FrameworkElement.WidthProperty, btn.Width - 10, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            element.Animate(FrameworkElement.HeightProperty, btn.Height - 10, 500,
                new QuinticEase() { EasingMode = EasingMode.EaseIn },
                (o, e) =>
                {
                    MainSurface.Children.Remove(element);

                    if (mapTransformPreIcon.ContainsKey(element))
                        mapTransformPreIcon.Remove(element);
                    if (element.RenderTransform != null && !mapTransformPreIcon.ContainsKey(element))
                    {
                        mapTransformPreIcon.Add(element, element.RenderTransform);
                        element.RenderTransform = null;
                    }

                    btn.ClipToBounds = false;
                    btn.Content = element;

                    iconizer.Children.Add(btn);

                    var bindingWidth = new Binding()
                    {
                        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Button), 1),
                        Path = new PropertyPath("ActualWidth")
                    };
                    var bindingHeight = new Binding()
                    {
                        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Button), 1),
                        Path = new PropertyPath("ActualHeight")
                    };

                    fe.BeginAnimation(FrameworkElement.WidthProperty, null);
                    fe.BeginAnimation(FrameworkElement.HeightProperty, null);

                    fe.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
                    fe.SetBinding(FrameworkElement.HeightProperty, bindingHeight);
                });

            //Dispatcher.BeginInvokeIfRequired(() =>
            //        {
            //            var map = Document.GetDynamicMapForElementAndChilds(element as FrameworkElement);
            //            foreach (var el in map.Keys)
            //            {
            //                foreach(var entity in map[el])
            //                    entity.SetInUse(
            //            }
            //        }, DispatcherPriority.ApplicationIdle);
        }

        void UniconControl(Button btn)
        {
            var element = btn.Content as FrameworkElement;

            var fe = element as FrameworkElement;
            if (!(fe is ContentControl))
            {
                var p = LogicalTreeHelper.GetParent(fe) as FrameworkElement;
                if (p is ContentControl && (p as ContentControl).Content is UIElement &&
                    !(p is UserControl))
                    element = p;
            }

            var relativeRect = LayoutHelper.GetRelativeElementRect(element, LayoutGrid);
            iconizer.Children.Remove(btn);
            btn.Content = null;

            //BindingOperations.ClearBinding(element, FrameworkElement.WidthProperty);
            //BindingOperations.ClearBinding(element, FrameworkElement.HeightProperty);

            //if (MainSurface is InkCanvas)
            //{
            //    InkCanvas.SetLeft(element, relativeRect.Left);
            //    InkCanvas.SetTop(element, relativeRect.Top);
            //}
            //else
            {
                Canvas.SetLeft(element, relativeRect.Left);
                Canvas.SetTop(element, relativeRect.Top);
            }
            MainSurface.Children.Add(element);

            var rect = mapRectPreIcon[element];
            mapRectPreIcon.Remove(element);

            //if (MainSurface is InkCanvas)
            //{
            //    element.Animate(InkCanvas.LeftProperty, rect.Left, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            //    element.Animate(InkCanvas.TopProperty, rect.Top, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            //}
            //else
            {
                element.Animate(Canvas.LeftProperty, rect.Left, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
                element.Animate(Canvas.TopProperty, rect.Top, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            }

            element.Animate(FrameworkElement.WidthProperty, rect.Width, 500, new QuinticEase() { EasingMode = EasingMode.EaseIn });
            element.Animate(FrameworkElement.HeightProperty, rect.Height, 500,
                new QuinticEase() { EasingMode = EasingMode.EaseIn },
                (o, e) =>
                {
                    //if (MainSurface is InkCanvas)
                    //{
                    //    element.BeginAnimation(InkCanvas.LeftProperty, null);
                    //    element.BeginAnimation(InkCanvas.TopProperty, null);
                    //}
                    //else
                    {
                        element.BeginAnimation(Canvas.LeftProperty, null);
                        element.BeginAnimation(Canvas.TopProperty, null);
                    }

                    element.BeginAnimation(FrameworkElement.WidthProperty, null);
                    element.BeginAnimation(FrameworkElement.HeightProperty, null);
                    //if (MainSurface is InkCanvas)
                    //{
                    //    InkCanvas.SetLeft(element, rect.Left);
                    //    InkCanvas.SetTop(element, rect.Top);
                    //}
                    //else
                    {
                        Canvas.SetLeft(element, rect.Left);
                        Canvas.SetTop(element, rect.Top);
                    }

                    element.Width = rect.Width;
                    element.Height = rect.Height;

                    if (mapTransformPreIcon.ContainsKey(element))
                    {
                        element.RenderTransform = mapTransformPreIcon[element];
                        mapTransformPreIcon.Remove(element);
                    }
                });
        }

        readonly List<UIElement> listManipulatedElements = new List<UIElement>();
        List<ScreenLayoutData> GetListElementLayout()
        {
            var list = new List<ScreenLayoutData>();
            listManipulatedElements.ForEach(el =>
                    {
                        var data = new ScreenLayoutData();

                        var frameworkElement = el as FrameworkElement;
                        var parent = LogicalTreeHelper.GetParent(frameworkElement) as FrameworkElement;
                        if (parent is ContentControl && (parent as ContentControl).Content is UIElement &&
                            !(parent is UserControl))
                        {
                            var inner = (parent as ContentControl).Content as FrameworkElement;
                            if (inner == frameworkElement)
                                data.Name = parent.Name;
                            else
                                data.Name = frameworkElement.Name;
                        }
                        else
                            data.Name = frameworkElement.Name;
                        //if (MainSurface is InkCanvas)
                        //{
                        //    data.Left = InkCanvas.GetLeft(el);
                        //    data.Top = InkCanvas.GetTop(el);
                        //}
                        //else
                        //{
                        //    data.Left = Canvas.GetLeft(frameworkElement);
                        //    data.Top = Canvas.GetTop(frameworkElement);
                        //}
                        //data.Width = frameworkElement.Width;
                        //data.Height = frameworkElement.Height;
                        data.IsMinimized = mapRectPreIcon.ContainsKey(el);
                        data.IsMaximized = el == zoomedControl;
                        data.RenderTransform = frameworkElement.RenderTransform;
                        data.RenderTransformOrigin = frameworkElement.RenderTransformOrigin;

                        list.Add(data);
                    });

            listManipulatedElements.Clear();

            return list;
        }

        void RestoreElementLayout(List<ScreenLayoutData> list)
        {
            listManipulatedElements.Clear();
            FrameworkElement toZoom = null;
            list.ForEach(data =>
                {
                    var frameworkElement = Document.FindInnerControl(MainSurface, data.Name);
                    if (frameworkElement != null)
                    {
                        var el = frameworkElement;
                        if (frameworkElement is ContentControl && (frameworkElement as ContentControl).Content is UIElement &&
                            !(frameworkElement is UserControl))
                        {
                            frameworkElement = (frameworkElement as ContentControl).Content as FrameworkElement;
                        }

                        if (data.IsMinimized)
                            IconControl(frameworkElement);
                        else if (data.IsMaximized)
                            toZoom = frameworkElement;
                        else
                        {
                            EnableManipulationControl(frameworkElement, bTransformOnly: true);

                            //if (MainSurface is InkCanvas)
                            //{
                            //    InkCanvas.SetLeft(frameworkElement, data.Left);
                            //    InkCanvas.SetTop(frameworkElement, data.Top);
                            //}
                            //else
                            //{
                            //    Canvas.SetLeft(el, data.Left);
                            //    Canvas.SetTop(el, data.Top);
                            //}

                            //el.Width = data.Width;
                            //el.Height = data.Height;
                            frameworkElement.RenderTransform = data.RenderTransform;
                            frameworkElement.RenderTransformOrigin = data.RenderTransformOrigin;
                        }

                        if (!listManipulatedElements.Contains(frameworkElement))
                            listManipulatedElements.Add(frameworkElement);
                    }
                });

            if (toZoom != null)
            {
                Dispatcher.BeginInvokeAsynchronously(() =>
                {
                    SaveLayoutData(GetUniqueTitle(Document));
                    ZoomControl(toZoom, false);
                });
            }
        }

        private void OnHome(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            OnHomeGesture();
        }

        private void CanHome(object sender, CanExecuteRoutedEventArgs e)
        {
#if !DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                var ret = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxY+APFmQPtOWSUaMJjYrwAg=="/* DBG */);
                if (!ret)
                    System.Environment.Exit(-10);
            }
#endif

            e.CanExecute = true;
        }

        private void OnBack(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            OnClosingGesture();
        }

        private void CanBack(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OnCanCloseGesture();
        }

        private void OnZoomControl(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var parent = e.Parameter as UIElement;
            if (parent == null)
            {
                if (e.OriginalSource == this)
                    return;
                parent = e.OriginalSource as UIElement;
            }
            var p = LogicalTreeHelper.GetParent(parent) as FrameworkElement;
            if (p is ContentControl && (p as ContentControl).Content is UIElement &&
                !(p is UserControl))
                parent = p;

            if (parent == zoomedControl)
                UnzoomControl(zoomedControl);
            else
                ZoomControl(parent);
        }

        private void CanZoomControl(object sender, CanExecuteRoutedEventArgs e)
        {
            var parent = e.Parameter as UIElement;
            if (parent == null)
            {
                if (e.OriginalSource != this)
                    parent = e.OriginalSource as UIElement;
            }
            var p = LogicalTreeHelper.GetParent(parent) as FrameworkElement;
            if (p is ContentControl && (p as ContentControl).Content is UIElement &&
                !(p is UserControl))
                parent = p;

            e.CanExecute = (e.Parameter is UIElement || e.OriginalSource != this) && parent != null && !mapElementToMan.ContainsKey(parent);
        }

        private void OnIconControl(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var parent = e.Parameter as UIElement;
            if (parent == null)
            {
                if (e.OriginalSource == this)
                    return;
                parent = e.OriginalSource as UIElement;
            }
            var p = LogicalTreeHelper.GetParent(parent) as FrameworkElement;
            if (p is ContentControl && (p as ContentControl).Content is UIElement &&
                !(p is UserControl))
                parent = p;

            IconControl(parent);
        }

        private void CanIconControl(object sender, CanExecuteRoutedEventArgs e)
        {
            var parent = e.Parameter as UIElement;
            if (parent == null)
            {
                if (e.OriginalSource != this)
                    parent = e.OriginalSource as UIElement;
            }
            var p = LogicalTreeHelper.GetParent(parent) as FrameworkElement;
            if (p is ContentControl && (p as ContentControl).Content is UIElement &&
                !(p is UserControl))
                parent = p;

            e.CanExecute = parent != null && parent != zoomedControl && !mapElementToMan.ContainsKey(parent) &&
                !mapRectPreIcon.ContainsKey(parent) && wndParent != null/* && ThemeHelper.GetTheme(wndParent) == "Blend"*/;
        }

        private void IconizerButton_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            Button btn = sender as Button;
            UniconControl(btn);
        }

        private void IconizerButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = sender as Button;
            UniconControl(btn);
        }

        readonly Dictionary<UIElement, ScreenManager.Behaviors.TranslateZoomRotateBehavior> mapElementToMan = new Dictionary<UIElement, ScreenManager.Behaviors.TranslateZoomRotateBehavior>();
        readonly Dictionary<UIElement, List<UIElement>> mapElementToListHitTest = new Dictionary<UIElement, List<UIElement>>();
        readonly Dictionary<UIElement, Transform> mapTransformPreMan = new Dictionary<UIElement, Transform>();
        readonly Dictionary<UIElement, Transform> mapTransformPreIcon = new Dictionary<UIElement, Transform>();
        readonly Dictionary<UIElement, Brush> mapBrushPreMan = new Dictionary<UIElement, Brush>();
        readonly Dictionary<UIElement, Effect> mapEffectPreMan = new Dictionary<UIElement, Effect>();

        private void OnResetManipulationControl(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            //if (!LayoutHelper.HasTouchInput())
            //{
            //    GestureResultAdorner.ShowMessage(Properties.Resources.NoTouchDevicesFound, Mouse.GetPosition(MainSurface));
            //    return;
            //}

            var parent = e.Parameter as UIElement;
            if (parent == null)
            {
                if (e.OriginalSource == this)
                    return;
                parent = e.OriginalSource as UIElement;
            }

            if (zoomedControl != null)
                UnzoomControl(zoomedControl);

            (from btn in iconizer.Children.OfType<Button>() where btn.Content == parent select btn).ToList().ForEach(el =>
            {
                UniconControl(el);
            });

            if (mapElementToMan.ContainsKey(parent))
            {
                EnableManipulationControl(parent);
            }

            if (mapTransformPreMan.ContainsKey(parent))
            {
                parent.RenderTransform = mapTransformPreMan[parent];
                mapTransformPreMan.Remove(parent);
            }
            else
                parent.RenderTransform = null;

            if (listManipulatedElements.Contains(parent))
                listManipulatedElements.Remove(parent);
        }

        private void CanResetManipulationControl(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is UIElement)
            {
                var parent = e.Parameter as UIElement;
                e.CanExecute = parent != zoomedControl && listManipulatedElements.Contains(parent);
            }
            else if (e.OriginalSource != this && e.OriginalSource is FrameworkElement)
            {
                var element = e.OriginalSource as FrameworkElement;
                e.CanExecute = element.IsManipulationEnabled && element != zoomedControl && listManipulatedElements.Contains(element);
            }
        }

        private void OnEnableManipulationControl(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            //if (!LayoutHelper.HasTouchInput())
            //{
            //    GestureResultAdorner.ShowMessage(Properties.Resources.NoTouchDevicesFound, Mouse.GetPosition(MainSurface));
            //    return;
            //}

            var parent = e.Parameter as UIElement;
            if (parent == null)
            {
                if (e.OriginalSource == this)
                    return;
                parent = e.OriginalSource as UIElement;
            }

            EnableManipulationControl(parent, false);
        }

        private void EnableManipulationControl(UIElement parent, bool bReset = true, bool bTransformOnly = false)
        {
            if (!listManipulatedElements.Contains(parent))
                listManipulatedElements.Add(parent);

            if (mapElementToMan.ContainsKey(parent))
            {
                //foreach (var behavior in System.Windows.Interactivity.Interaction.GetBehaviors(parent))
                //{
                //    behavior.Detach();
                //}
                // parent.IsManipulationEnabled = false;
                mapElementToMan[parent].Detach();
                System.Windows.Interactivity.Interaction.GetBehaviors(parent).Remove(mapElementToMan[parent]);
                mapElementToMan.Remove(parent);

                if (mapElementToListHitTest.ContainsKey(parent))
                {
                    mapElementToListHitTest[parent].ForEach(el =>
                        {
                            el.IsHitTestVisible = true;
                        });
                    mapElementToListHitTest.Remove(parent);
                }

                if (bReset)
                {
                    if (mapTransformPreMan.ContainsKey(parent))
                    {
                        parent.RenderTransform = mapTransformPreMan[parent];
                        mapTransformPreMan.Remove(parent);
                    }
                    else
                        parent.RenderTransform = null;
                }

                if (mapBrushPreMan.ContainsKey(parent))
                {
                    if (parent is Grid)
                    {
                        var grid = parent as Grid;
                        grid.Background = mapBrushPreMan[parent];
                    }
                    else if (parent is Panel)
                    {
                        var panel = parent as Panel;
                        panel.Background = mapBrushPreMan[parent];
                    }
                    else if (parent is ContentControl)
                    {
                        var panel = parent as ContentControl;
                        panel.Background = mapBrushPreMan[parent];
                    }

                    mapBrushPreMan.Remove(parent);
                }

                if (mapEffectPreMan.ContainsKey(parent))
                {
                    parent.Effect = mapEffectPreMan[parent];
                    mapEffectPreMan.Remove(parent);
                }
                else
                    parent.Effect = null;

                if (mapManAdorners.ContainsKey(parent))
                {
                    UIElement el = parent;
                    AdornerLayer adorner = AdornerLayer.GetAdornerLayer(parent);
                    if (adorner == null)
                    {
                        el = MainSurface;
                        adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                    }
                    if (adorner != null && adorner.GetAdorners(el) != null &&
                        !adorner.GetAdorners(el).ToList().Contains(mapManAdorners[parent]))
                    {
                        adorner.Add(mapManAdorners[parent]);
                    }
                }

                parent.IsManipulationEnabled = false;

                /*
                var list3D = parent.GetChildrenOfType<Viewport3D>().ToList();
                if (parent is Viewport3D)
                    list3D.Add(parent as Viewport3D);
                foreach (var el in list3D)
                {
                    if (mapElementTrackball.ContainsKey(el))
                        mapElementTrackball[el].EventSource = el;
                }
                */
            }
            else
            {
                /*
                var list3D = parent.GetChildrenOfType<Viewport3D>().ToList();
                if (parent is Viewport3D)
                    list3D.Add(parent as Viewport3D);
                foreach (var el in list3D)
                {
                    if (mapElementTrackball.ContainsKey(el))
                        mapElementTrackball[el].EventSource = null;
                }
                */

                if (!bReset)
                    parent.IsManipulationEnabled = true;
                if (!bTransformOnly && mapManAdorners.ContainsKey(parent))
                {
                    UIElement el = parent;
                    AdornerLayer adorner = AdornerLayer.GetAdornerLayer(parent);
                    if (adorner == null)
                    {
                        el = MainSurface;
                        adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                    }
                    //if (adorner != null && adorner.GetAdorners(el) != null &&
                    //    adorner.GetAdorners(el).ToList().Contains(mapManAdorners[parent]))
                    //{
                    //    adorner.Remove(mapManAdorners[parent]);
                    //}
                }

                if (parent.RenderTransform != null)
                {
                    if (!mapTransformPreMan.ContainsKey(parent))
                        mapTransformPreMan.Add(parent, parent.RenderTransform);
                }

                if (!bTransformOnly)
                {
                    Canvas.SetZIndex(parent, MainSurface.Children.Count - 1);

                    var var = new ScreenManager.Behaviors.TranslateZoomRotateBehavior
                    {
                        TranslateFriction = 1,
                        RotationalFriction = 1,
                        ConstrainToParentBounds = true
                    };
                    // System.Windows.Interactivity.Interaction.GetBehaviors(parent).Add(var);
                    var.Attach(parent);
                    mapElementToMan[parent] = var;

                    bool bCatchHitVisible = false;
                    if (parent is Grid)
                    {
                        var grid = parent as Grid;

                        if (grid.Background == Brushes.Transparent ||
                            grid.Background == null)
                        {
                            mapBrushPreMan.Add(parent, grid.Background);
                            if (!(MainSurface.Background is VisualBrush))
                                grid.Background = MainSurface.Background;
                        }
                        bCatchHitVisible = true;
                    }
                    else if (parent is Panel)
                    {
                        var panel = parent as Panel;

                        if (panel.Background == Brushes.Transparent ||
                            panel.Background == null)
                        {
                            mapBrushPreMan.Add(parent, panel.Background);
                            if (!(MainSurface.Background is VisualBrush))
                                panel.Background = MainSurface.Background;
                        }
                        bCatchHitVisible = true;
                    }
                    else if (parent is ContentControl)
                    {
                        var geometry = VisualTreeHelper.GetClip(parent);
                        var panel = parent as ContentControl;

                        if (panel.Background == Brushes.Transparent ||
                            panel.Background == null)
                        {
                            mapBrushPreMan.Add(parent, panel.Background);
                            if (!(MainSurface.Background is VisualBrush))
                                // panel.Background = MainSurface.Background;
                                panel.Background = new DrawingBrush(new GeometryDrawing(MainSurface.Background, 
                                    new Pen(MainSurface.Background, 0), geometry));
                        }
                        bCatchHitVisible = true;
                    }

                    if (bCatchHitVisible)
                    {
                        var list = parent.GetChildrenOfType<UIElement>();
                        mapElementToListHitTest.Add(parent, new List<UIElement>());
                        foreach (var el in list)
                        {
                            if (el.IsHitTestVisible)
                            {
                                el.IsHitTestVisible = false;
                                mapElementToListHitTest[parent].Add(el);
                            }
                        }
                    }

                    // Canvas.SetZIndex(parent, MainSurface.Children.Count - 1);
                    var effect = new DropShadowEffect
                    {
                        ShadowDepth = 50,
                        BlurRadius = 20,
                        Opacity = 0.5
                    };
                    if (mapEffectPreMan.ContainsKey(parent))
                        mapEffectPreMan.Remove(parent);
                    if (parent.Effect != null)
                        mapEffectPreMan.Add(parent, parent.Effect);
                    parent.Effect = effect;
                }
            }
        }

        private void CanEnableManipulationControl(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is UIElement)
            {
                var parent = e.Parameter as UIElement;
                e.CanExecute = parent != zoomedControl;
            }
            else if (e.OriginalSource != this && e.OriginalSource is FrameworkElement)
            {
                var element = e.OriginalSource as FrameworkElement;
                e.CanExecute = element.IsManipulationEnabled && element != zoomedControl;
            }
        }
#endif
        #endregion Methods

        #region Manipulation management


#if !WINDOWS_UWP
        TouchDevice Touch1IDDevice;
        int Touch1ID;
        int Touch2ID;
        int Touch3ID;
        TouchPoint lastTouchPoint;
        TouchPoint tapTouchPoint;
        DateTime lastTimeTap;

        readonly Stopwatch _doubleTapStopwatch = new Stopwatch();
        Point _lastTapLocation;
        Object _lastTouchSource;
        static double GetDistanceBetweenPoints(Point p, Point q)
        {
            if (p == q)
                return 0;

            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }

        static readonly int doubleClickTime = System.Windows.Forms.SystemInformation.DoubleClickTime;
        // int nDoubleTapTouchDeviceId = 0;
        bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapPosition = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = GetDistanceBetweenPoints(currentTapPosition, _lastTapLocation) < 40;
            _lastTapLocation = currentTapPosition;

            TimeSpan elapsed = _doubleTapStopwatch.Elapsed;
            _doubleTapStopwatch.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromMilliseconds(doubleClickTime));

            /*
            if (nDoubleTapTouchDeviceId == 0)
            {
                nDoubleTapTouchDeviceId = e.TouchDevice.Id;
                return false;
            }
            else if (nDoubleTapTouchDeviceId != e.TouchDevice.Id)
            {
                nDoubleTapTouchDeviceId = 0;
                return false;
            }
            else
                nDoubleTapTouchDeviceId = 0;
            */

            var ret = tapsAreCloseInDistance && tapsAreCloseInTime;
            if (ret)
                ret = _lastTouchSource == e.Source;
            _lastTouchSource = e.Source;

            if (ret)
                _lastTapLocation = new Point(0, 0);

            return ret;
        }

        Point lastTapDownPoint;
        DateTime _TapAndHoldStopwatch = DateTime.Now;
        void viewport3D_TouchDown(object sender, TouchEventArgs e)
        {
            lastTapDownPoint = e.GetTouchPoint(this).Position;
            _TapAndHoldStopwatch = DateTime.Now;
        }

        void viewport3D_TouchUp(object sender, TouchEventArgs e)
        {
            Point currentTapPosition = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = GetDistanceBetweenPoints(currentTapPosition, lastTapDownPoint) < 40;
            lastTapDownPoint = currentTapPosition;
            if (tapsAreCloseInDistance)
            {
                CheckClickOn3DModel(e);
                if (!e.Handled)
                {
                    var elapsed = DateTime.Now - _TapAndHoldStopwatch;
                    bool tapsAreCloseInTime = (elapsed < TimeSpan.FromMilliseconds(1000));
                    if (!tapsAreCloseInTime)
                    {
                        var element = e.Source as FrameworkElement;

                        if (element != MainSurface && !MainSurface.Children.Contains(element))
                        {
                            while ((element = LogicalTreeHelper.GetParent(element) as FrameworkElement) != null)
                            {
                                if (MainSurface.Children.Contains(element))
                                    break;
                            }
                        }

                        if (element == null)
                            element = e.Source as FrameworkElement;
                        if (element == null || String.IsNullOrEmpty(element.Name))
                            return;

                        if (!Document.MapScreenEntities.ContainsKey(element.Name))
                            return;
                        var entry = Document.MapScreenEntities[element.Name];

                        var listViewport3Ds = element.GetChildrenOfType<Viewport3D>().ToList();
                        if (listViewport3Ds.Count == 0 && element is Viewport3D)
                            listViewport3Ds.Add(element as Viewport3D);
                        foreach (var viewport3D in listViewport3Ds)
                        {
                            if (viewport3D.ContextMenu != null)
                            {
                                // for some unknown reason we need to force theme on the contextmenu if it appears on touch the first time
                                //if (wndParent != null)
                                //    ThemeHelper.SetTheme(element.ContextMenu, ThemeHelper.GetTheme(wndParent));
                                viewport3D.ContextMenu.IsOpen = true;
                            }
                        }
                        if (element.ContextMenu != null)
                        {
                            // for some unknown reason we need to force theme on the contextmenu if it appears on touch the first time
                            //if (wndParent != null)
                            //    ThemeHelper.SetTheme(element.ContextMenu, ThemeHelper.GetTheme(wndParent));
                            element.ContextMenu.IsOpen = true;
                        }
                    }
                }
                e.Handled = false;
                //                if (e.Handled)
                //                {
                //#if DEBUG
                //                    log.Info("touch executed on 3D model");
                //#endif
                //                    return;
                //                }
            }
        }

        int previewTouch1ID;
        int previewTouch2ID;
        private void MainSurface_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (previewTouch1ID == 0)
            {
                if (e.Source is CheckBox || e.Source is UserControl || e.Source is ComboBox)
                    MainSurface.IsManipulationEnabled = false;
                else
                {
                    previewTouch1ID = e.TouchDevice.Id;
                    lastTouchPoint = e.GetTouchPoint(MainSurface);
                }
            }
            else if (previewTouch2ID == 0)
            {
                previewTouch2ID = e.TouchDevice.Id;
                MainSurface.IsManipulationEnabled = true;
            }
            else
            {
                Touch3ID = e.TouchDevice.Id;
                MainSurface.IsManipulationEnabled = false;
                try
                {
                    // Scroller.ReleaseTouchCapture(e.TouchDevice);
                    MainSurface.CaptureTouch(e.TouchDevice);
                }
                catch (Exception ex)
                {

                }
                lastTouchPoint = e.GetTouchPoint(MainSurface);
                e.Handled = true;
            }
        }

        private void MainSurface_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            CleanAllOpenPopups();

            if (Touch3ID == e.TouchDevice.Id)
            {
                Touch3ID = 0;
                // MainSurface.IsManipulationEnabled = false;
                foreach (TouchDevice device in MainSurface.TouchesOver)
                {
                    try
                    {
                        MainSurface.ReleaseTouchCapture(device);
                    }
                    catch
                    {

                    }
                }
                e.Handled = true;
            }
            if (previewTouch2ID == e.TouchDevice.Id)
                previewTouch2ID = 0;
            if (previewTouch1ID == e.TouchDevice.Id)
            {
                previewTouch1ID = 0;
                MainSurface.IsManipulationEnabled = true;
            }
        }

        private void MainSurface_TouchLeave(object sender, TouchEventArgs e)
        {
            MainSurface_PreviewTouchUp(sender, e);
        }

        //bool bDoubleTappedZoom;
        //double dPreviousZoomX;
        //double dPreviousZoomY;

        TouchPoint firstTouchPosition;
        TouchPoint secondTouchPosition;
        Point cornerPointPinch;
        Point centerPointPinch;
        Point ptDifferencePinch;
        private void MainSurface_TouchDown(object sender, TouchEventArgs e)
        {
            if (e.Source is ScreenManager.SpecialObjects.EmbeddedTabScreen || e.Source is UserControl)
            {
                Scroller.PanningMode = PanningMode.None;
                System.Diagnostics.Debug.WriteLine("PanningMode.None Embedded Screen");
                return;
            }
            var sourceUIElement = e.Source as UIElement;
            if (sourceUIElement != null)
            {
                var embedded = sourceUIElement.FindParent<ScreenManager.SpecialObjects.EmbeddedTabScreen>();
                if (embedded != null)
                {
                    //Scroller.PanningMode = PanningMode.Both;
                    //System.Diagnostics.Debug.WriteLine("PanningMode.Both Embedded Screen");
                    return;
                }
            }
            if (e.Source != MainSurface && e.Source != Scroller)
            {
                // Scroller.PanningMode = PanningMode.None;

                var fe = e.Source as FrameworkElement;
                if (mapElementToMan.ContainsKey(fe))
                    return;
                var contentControl = fe.FindParent<ContentControl>();
                if (contentControl != null && mapElementToMan.ContainsKey(contentControl))
                    return;

                /*
                bool hasContextMenu = false;
                if (fe.ContextMenu != null)
                    hasContextMenu = true;
                else
                {
                    var contentControl = fe.FindParent<ContentControl>();
                    if (contentControl != null && contentControl.ContextMenu != null)
                        hasContextMenu = true;
                }

                if (hasContextMenu)
                {
                    tapTouchPoint = e.GetTouchPoint(MainSurface);
                    lastTimeTap = DateTime.UtcNow;
#if DEBUG
                    log.Info("touch executed on Context Menu");
#endif
                    return;
                }
                */
                if (e.Source is Viewport3D && fe.IsHitTestVisible)
                {
#if DEBUG
                    log.Info(Properties.Resources.TouchOnViewport3D);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(DocumentParent, Properties.Resources.ScreenManager,
                        DateTime.UtcNow, Properties.Resources.TouchOnViewport3D,
                        System.Diagnostics.EventLogEntryType.Information);
#endif
                    return;
                }
            }
            //else
            //    Scroller.PanningMode = PanningMode.Both;
            tapTouchPoint = null;

            Adorner adorner = null;
            if (e.OriginalSource is Visual || e.OriginalSource is Visual3D)
                adorner = (e.OriginalSource as DependencyObject).FindParent<Adorner>();
            if (adorner != null)
            {
#if DEBUG
                log.Info(Properties.Resources.TouchOnAdorner);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(DocumentParent, Properties.Resources.ScreenManager,
                    DateTime.UtcNow, Properties.Resources.TouchOnAdorner,
                    System.Diagnostics.EventLogEntryType.Information);
#endif
                return;
            }

            if ((e.Source == MainSurface || e.Source == Scroller) &&
                IsDoubleTap(e))
            {
                e.Handled = true;

                //ZoomLevelX = 1;
                //ZoomLevelY = 1;
                System.Diagnostics.Debug.WriteLine(String.Format("Double Tap Device Id {0}", e.TouchDevice.Id));

                //if (Document.FitInWindow && !Document.DisableZoom)
                //    OnFillMode(null, null);
                if (!Document.DisableZoom)
                {
                    var x = GetFillZoomFactorX();
                    var y = GetFillZoomFactorY();
                    // if (!bDoubleTappedZoom)
                    if (ZoomLevelX == x && ZoomLevelY == y)
                    {
                        //bDoubleTappedZoom = true;
                        //dPreviousZoomX = ZoomLevelX;
                        //dPreviousZoomY = ZoomLevelY;

                        AnimateZoom(x * 2, y * 2, 200);
                    }
                    else
                    {
                        //bDoubleTappedZoom = false;
                        //AnimateZoom(dPreviousZoomX, dPreviousZoomY);
                        AnimateZoom(x - 0.1, y - 0.1, 200, x, y);
                    }
                }
#if DEBUG
                log.Info(Properties.Resources.TouchOnTap);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(DocumentParent, Properties.Resources.ScreenManager,
                    DateTime.UtcNow, Properties.Resources.TouchOnTap,
                    System.Diagnostics.EventLogEntryType.Information);
#endif
                return;
            }

            // MainSurface.EditingMode = InkCanvasEditingMode.None;
            bEnableTouchGesture = true;
            if (Touch1ID == 0)
            {
                //first touch encountered.
                Touch1ID = e.TouchDevice.Id;
                Touch1IDDevice = e.TouchDevice;
                // MainSurface.IsManipulationEnabled = true;
                //try
                //{
                //    MainSurface.CaptureTouch(e.TouchDevice);
                //    // Scroller.CaptureTouch(e.TouchDevice);
                //}
                //catch
                //{

                //}
                firstTouchPosition = lastTouchPoint = e.GetTouchPoint(MainSurface);
            }
            else if (Touch2ID == 0)
            {
                //second touch encountered.
                Touch2ID = e.TouchDevice.Id;
                MainSurface.IsManipulationEnabled = true;
                try
                {
                    // Scroller.ReleaseTouchCapture(device);
                    MainSurface.CaptureTouch(e.TouchDevice);
                    if (Touch1ID != 0 && Touch1IDDevice != null)
                        MainSurface.CaptureTouch(Touch1IDDevice);
                }
                catch
                {

                }
                lastTouchPoint = null;
                secondTouchPosition = e.GetTouchPoint(MainSurface);

                cornerPointPinch = new Point(Scroller.HorizontalOffset, Scroller.VerticalOffset);
                double centerX = 0, centerY = 0;
                if (secondTouchPosition.Position.X > firstTouchPosition.Position.X)
                    centerX = ((secondTouchPosition.Position.X - firstTouchPosition.Position.X) / 2) + firstTouchPosition.Position.X;
                else
                    centerX = ((firstTouchPosition.Position.X - secondTouchPosition.Position.X) / 2) + secondTouchPosition.Position.X;
                if (secondTouchPosition.Position.Y > firstTouchPosition.Position.Y)
                    centerY = ((secondTouchPosition.Position.Y - firstTouchPosition.Position.Y) / 2) + firstTouchPosition.Position.Y;
                else
                    centerY = ((firstTouchPosition.Position.Y - secondTouchPosition.Position.Y) / 2) + secondTouchPosition.Position.Y;
                var diff = new Point(centerX, centerY);
                ptDifferencePinch = new Point(Math.Abs(diff.X), Math.Abs(diff.Y));
                centerPointPinch = new Point(centerX * ZoomLevelX, centerY * ZoomLevelY);
            }
            else
            {
                Touch3ID = e.TouchDevice.Id;
                MainSurface.IsManipulationEnabled = false;
                foreach (TouchDevice device in MainSurface.TouchesOver)
                {
                    try
                    {
                        MainSurface.ReleaseTouchCapture(device);
                    }
                    catch
                    {

                    }
                }

                try
                {
                    MainSurface.ReleaseTouchCapture(e.TouchDevice);
                    if (Touch1IDDevice != null)
                    {
                        MainSurface.ReleaseTouchCapture(Touch1IDDevice);
                        Touch1IDDevice = null;
                    }
                    // Scroller.ReleaseTouchCapture(e.TouchDevice);
                }
                catch (Exception ex)
                {

                }
                lastTouchPoint = e.GetTouchPoint(MainSurface);
            }
#if DEBUG
            log.Info(String.Format(Properties.Resources.TouchOnDevice, e.TouchDevice.Id));
            if (iUFProjectManager != null)
                iUFProjectManager.AddLogEntity(DocumentParent, Properties.Resources.ScreenManager,
                DateTime.UtcNow, String.Format(Properties.Resources.TouchOnDevice, e.TouchDevice.Id),
                System.Diagnostics.EventLogEntryType.Information);
#endif

            // e.Handled = true;
            System.Diagnostics.Debug.WriteLine(String.Format("Touch Down Device Id {0}", e.TouchDevice.Id));
        }

        bool bEnableTouchGesture;
        private void MainSurface_TouchMove(object sender, TouchEventArgs e)
        {
            if (lastTouchPoint == null || !bEnableTouchGesture)
                return;

            if (Touch3ID == e.TouchDevice.Id ||
                Touch3ID == 0 && Touch1ID == e.TouchDevice.Id && Scroller.ScrollableWidth == 0)
            {
                if (/*Scroller.ScrollableWidth == 0 &&
                */e.GetTouchPoint(MainSurface).Position.X > (lastTouchPoint.Position.X + (MainSurface.ActualWidth / 2)/* 200*/))
                {
                    bEnableTouchGesture = false;
                    OnPrevGesture();
                }
                else if (/*Scroller.ScrollableWidth == 0 &&
                */e.GetTouchPoint(MainSurface).Position.X < (lastTouchPoint.Position.X - (MainSurface.ActualWidth / 2)/* 200*/))
                {
                    bEnableTouchGesture = false;
                    OnNextGesture();
                }
            }
        }

        private void MainSurface_TouchUp(object sender, TouchEventArgs e)
        {
            if (Touch3ID == e.TouchDevice.Id)
            {
                Touch3ID = 0;
                try
                {
                    MainSurface.ReleaseTouchCapture(e.TouchDevice);
                    // Scroller.ReleaseTouchCapture(e.TouchDevice);
                }
                catch (Exception ex)
                {

                }
            }
            if (Touch2ID == e.TouchDevice.Id)
            {
                Touch2ID = 0;
                try
                {
                    MainSurface.ReleaseTouchCapture(e.TouchDevice);
                    // Scroller.ReleaseTouchCapture(e.TouchDevice);
                }
                catch (Exception ex)
                {

                }
            }
            if (Touch1ID == e.TouchDevice.Id)
            {
                Touch1ID = 0;
                if (Touch1IDDevice != null)
                {
                    try
                    {
                        MainSurface.ReleaseTouchCapture(Touch1IDDevice);
                        // Scroller.ReleaseTouchCapture(e.TouchDevice);
                    }
                    catch (Exception ex)
                    {

                    }
                    Touch1IDDevice = null;
                }
            }

            // e.Handled = true;
            System.Diagnostics.Debug.WriteLine(String.Format("Touch Up Device Id {0}", e.TouchDevice.Id));
            if (lastTouchPoint != null)
            {
                var touchPoint = e.GetTouchPoint(MainSurface);
                if (Math.Abs(lastTouchPoint.Position.X - touchPoint.Position.X) < 10 &&
                    Math.Abs(lastTouchPoint.Position.Y - touchPoint.Position.Y) < 10 &&
                    (touchPoint.Position.Y < 100 || bStoryBoardStarted))
                {
                    ShowOrHideToolBar();
                }
                lastTouchPoint = null;
            }

            bEnableTouchGesture = false;
            Adorner adorner = null;
            if (e.OriginalSource is Visual || e.OriginalSource is Visual3D)
                adorner = (e.OriginalSource as DependencyObject).FindParent<Adorner>();
            if (adorner != null)
                return;

            if (sender == MainSurface)
                ActivateManipulationAdorner(e.Source as UIElement, true);

            /*
            if (e.Source != MainSurface && e.Source != Scroller)
            {
                if (tapTouchPoint != null)
                {
                    var touchPoint = e.GetTouchPoint(MainSurface);
                    var x1 = touchPoint.Position.X;
                    var y1 = touchPoint.Position.Y;
                    var x2 = tapTouchPoint.Position.X;
                    var y2 = tapTouchPoint.Position.Y;
                    var diffX = Math.Abs(x1 - x2);
                    var diffY = Math.Abs(y1 - y2);
                    var timeSpan = DateTime.UtcNow - lastTimeTap;
                    if (timeSpan.Seconds >= 2 && diffX < 5 && diffY < 5 && e.Source is FrameworkElement)
                    {
                        var fe = e.Source as FrameworkElement;
                        if (fe.ContextMenu != null)
                            fe.ContextMenu.IsOpen = true;
                        else
                        {
                            var contentControl = fe.FindParent<ContentControl>();
                            if (contentControl != null && contentControl.ContextMenu != null)
                                contentControl.ContextMenu.IsOpen = true;
                        }
                    }
                }
                return;
            }
            if (sender == Scroller && Touch1ID == 0)
                return;
            */
        }

        bool ActivateManipulationAdorner(UIElement child, bool bCheckRemove = false)
        {
            if (child == null)
                return false;

            UIElement found = null;
            do
            {
                if (/*!child.IsManipulationEnabled || */!MainSurface.Children.Contains(child) ||
                    child.Visibility != System.Windows.Visibility.Visible)
                {
                    if (child == MainSurface || child == this)
                        break;
                    else
                        continue;
                }
                if (listManipulationEntities != null)
                {
                    var list = (from c in listManipulationEntities
                                where Document.MapScreenEntities.ContainsKey(c) && Document.MapScreenEntities[c].Entity == child
                                select c).ToList();
                    if (list.Count > 0)
                        found = child;
                }

            } while ((child = VisualTreeHelper.GetParent(child) as UIElement) != null);

            if (found != null)
            {
                if (found is ContentControl && (found as ContentControl).Content is UIElement &&
                    !(found is UserControl))
                {
                    found = (found as ContentControl).Content as UIElement;
                }

                var adorner = AdornerLayer.GetAdornerLayer(found);
                if (adorner == null)
                    adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                if (adorner != null)
                {
                    if (mapManAdorners.ContainsKey(found))
                    {
                        if (!bCheckRemove)
                        {
                            adorner.Remove(mapManAdorners[found]);
                            mapManAdorners.Remove(found);
                        }
                    }
                    else
                    {
                        var nodeadorner = new ManipulationAdorner(found);
                        var fe = found as FrameworkElement;
                        if (mapElementCamera.ContainsKey(fe.Name))
                        {
                            if (Document.MapScreenEntities.ContainsKey(fe.Name))
                            {
                                Document.MapScreenEntities[fe.Name].ListCameraTransforms.ForEach(item =>
                                {
                                    var btn = new Button()
                                    {
                                        Content = item.Name
                                    };
                                    btn.Click += (o, ev) =>
                                    {
                                        mapElementCamera[fe.Name].Animate(item.Scale, item.Rotation, item.TranslateTransform2D,
                                            item.AnimationTime, new SineEase() { EasingMode = EasingMode.EaseOut });
                                    };
                                    nodeadorner.AddButton(btn);
                                });
                            }
                        }

                        mapManAdorners.Add(found, nodeadorner);
                        adorner.Add(nodeadorner);
                    }
                }

                return true;
            }

            return false;
        }

        private void Window_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element == MainSurface)
            {
                bIgnorenext = false;
                // Scroller.PanningMode = PanningMode.Both;
                // System.Diagnostics.Debug.WriteLine("PanningMode.Both");

                //OffsetLevelX = 0;
                //OffsetLevelY = 0;
                //MainSurface.LayoutTransform = MainSurface.RenderTransform;
                //MainSurface.RenderTransform = null;
                e.Handled = true;

                AnimateToFill();
            }
            else
            {
                ActivateManipulationAdorner(element);
                ActivateManipulationAdorner(element, true);
            }
        }

        void Window_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            if (e.Source is UserControl)
                return;
            var uie = e.OriginalSource as UIElement;
            //if (mapElementToMan.ContainsKey(uie) || uie != MainSurface)
            //    return;

            if (uie == MainSurface || !uie.IsManipulationEnabled)
            {
                // Scroller.PanningMode = PanningMode.None;
                // MainSurface.EditingMode = InkCanvasEditingMode.None;
                e.Mode = ManipulationModes.All;

                //MainSurface.RenderTransform = MainSurface.LayoutTransform;
                //MainSurface.LayoutTransform = null;
                //OffsetLevelX = Scroller.HorizontalOffset * Scroller.ViewportWidth / Scroller.ExtentWidth;
                //OffsetLevelY = Scroller.VerticalOffset * Scroller.ViewportHeight / Scroller.ExtentHeight;
                //Scroller.ScrollToHome();

                System.Diagnostics.Debug.WriteLine("PanningMode.None Masnipulation Starting");
            }

            e.ManipulationContainer = MainSurface;
            e.Handled = true;
            //if (savedEffect != null)
            //    return;
            /*
            if (uie != null)
            {
                // uie.RenderTransformOrigin = new Point(0.5, 0.5);
                Canvas.SetZIndex(uie, MainSurface.Children.Count - 1);

                var effect = new DropShadowEffect
                {
                    ShadowDepth = 50,
                    Opacity = 0.5
                };
                savedEffect = uie.Effect;
                uie.Effect = effect;
            }
            */
        }

        /*
        private void Window_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;
            //if (uie != null)
            //    uie.Effect = savedEffect;
            //savedEffect = null;
            e.Handled = true;
        }
        */

        public event EventHandler<OpenUriEventArgs> OpenRecentUri;
        #region OnRecentListUris
        /// <summary>
        /// Triggers the ClosingGesture event.
        /// </summary>
        public virtual void OnOpenRecentUri(Uri uriToOpen)
        {
            var e = OpenRecentUri;
            if (e != null)
            {
                var arg = new OpenUriEventArgs() { uri = uriToOpen };
                e(this, arg);
            }
        }
        #endregion






        public event EventHandler ClosingGesture;
        #region OnClosingGesture
        /// <summary>
        /// Triggers the ClosingGesture event.
        /// </summary>
        public virtual void OnClosingGesture()
        {
            var e = ClosingGesture;
            if (e != null)
                e(this, new EventArgs());
        }
        #endregion

        public event EventHandler<CancelEventArgs> CanCloseGesture;
        #region OnCanCloseGesture
        /// <summary>
        /// Triggers the ClosingGesture event.
        /// </summary>
        public virtual bool OnCanCloseGesture()
        {
            var cancelEventArg = new CancelEventArgs();
            var e = CanCloseGesture;
            if (e != null)
            {
                e(this, cancelEventArg);
            }

            return cancelEventArg.Cancel;
        }
        #endregion


        public event EventHandler HomeGesture;
        #region OnHomeGesture
        /// <summary>
        /// Triggers the HomeGesture event.
        /// </summary>
        public virtual void OnHomeGesture()
        {
            var e = HomeGesture;
            if (e != null)
                e(this, new EventArgs());
        }
        #endregion

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

        bool bIgnorenext;
        void Window_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element == MainSurface)
            {
                if (bIgnorenext)
                {
                    bIgnorenext = false;
                }
                else
                {
                    if (Document == null || Document.DisableZoom)
                        return;

                    bIgnorenext = true;
                    var zoomx = ZoomLevelX * e.DeltaManipulation.Scale.X;
                    var zoomy = ZoomLevelY * e.DeltaManipulation.Scale.Y;

                    //ZoomLevelX = Math.Max(1, zoomx);
                    //ZoomLevelY = Math.Max(1, zoomy);
                    ZoomLevelX = zoomx;
                    ZoomLevelY = zoomy;
                    //MainSurface.Measure(new Size(MainSurface.Width, MainSurface.Height));
                    //MainSurface.Arrange(new Rect(new Point(0, 0), MainSurface.DesiredSize));

                    if (Touch2ID == 0)
                    {
                        Scroller.ScrollToHorizontalOffset(Scroller.HorizontalOffset - (e.DeltaManipulation.Translation.X * ZoomLevelX));
                        Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset - (e.DeltaManipulation.Translation.Y * ZoomLevelY));
                    }
                    else
                    {
                        var x = ptDifferencePinch.X * ZoomLevelX;
                        var y = ptDifferencePinch.Y * ZoomLevelY;

                        var scrollX = (x - centerPointPinch.X) + cornerPointPinch.X;
                        var scrollY = (y - centerPointPinch.Y) + cornerPointPinch.Y;
                        System.Diagnostics.Debug.WriteLine(String.Format("scrollX x = {0}, scrollY = {1}, currentscrollX x = {2}, currentscrollY = {3}, ptDifferencePinch.X = {4}, ptDifferencePinch.Y = {5}",
                                scrollX, scrollY, Scroller.HorizontalOffset, Scroller.VerticalOffset, x, y));

                        Scroller.ScrollToHorizontalOffset(scrollX);
                        Scroller.ScrollToVerticalOffset(scrollY);
                    }
                    System.Diagnostics.Debug.WriteLine(String.Format("Zooming x = {0}, y = {1}, deltax = {2}, deltay = {3}, translateX {4}, tranlsateY {5}",
                            ZoomLevelX, ZoomLevelY, e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.Y, e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y));
                }
                e.Handled = true;
            }
            else
            {
                if (listManipulationEntities != null)
                {
                    var list = (from c in listManipulationEntities
                                where Document.MapScreenEntities.ContainsKey(c) && Document.MapScreenEntities[c].Entity == element
                                select c).ToList();
                    if (list.Count > 0)
                    {
                        if (e.DeltaManipulation.Scale.X > 1.0 || e.DeltaManipulation.Scale.Y > 1.0)
                        {
                            if (element != zoomedControl)
                            {
                                e.Handled = true;
                                ZoomControl(element);
                            }
                        }
                        else if (e.DeltaManipulation.Scale.X < 1.0 || e.DeltaManipulation.Scale.Y < 1.0)
                        {
                            if (element == zoomedControl)
                            {
                                e.Handled = true;
                                UnzoomControl(element);
                            }
                            else if (iconizer.Children.Contains(element))
                            {
                                e.Handled = true;
                                IconControl(element);
                            }
                        }
                    }
                }
            }
            /*
            RotateTransform rotation = Utilities.Animations.Animations.SetTransform<RotateTransform>(element, true);
            ScaleTransform scale = Utilities.Animations.Animations.SetTransform<ScaleTransform>(element, true);
            TranslateTransform translation = Utilities.Animations.Animations.SetTransform<TranslateTransform>(element, true);

            var container = (FrameworkElement)e.ManipulationContainer;
            var transformGroup = element.RenderTransform as TransformGroup;
            Rect containerBounds = new Rect(container.RenderSize);
            // Rect objectBounds = transformGroup.TransformBounds(new Rect(element.RenderSize));
            List<UIElement> list = new List<UIElement>();
            list.Add(element);
            Rect objectBounds = Utilities.WPF.DependencyObjectExtensions.CalculateBoundRect(list, MainSurface);

            e.Handled = true;
            if (e.IsInertial && !containerBounds.Contains(objectBounds))
            {
                e.ReportBoundaryFeedback(e.DeltaManipulation);
                e.Complete();
            }
            else
            {
                // the center never changes in this sample, although we always compute it.
                Point center = new Point(element.ActualWidth / 2.0, element.ActualHeight / 2.0);

                // apply the rotation at the center of the rectangle if it has changed
                rotation.CenterX = center.X;
                rotation.CenterY = center.Y;
                rotation.Angle = e.DeltaManipulation.Rotation;

                // Scale is always uniform, by definition, so the x and y will always have the same magnitude if it has changed

                scale.CenterX = center.X;
                scale.CenterY = center.Y;
                scale.ScaleX *= e.DeltaManipulation.Scale.X;
                scale.ScaleY *= e.DeltaManipulation.Scale.Y;

                translation.X += e.DeltaManipulation.Translation.X;
                translation.Y += e.DeltaManipulation.Translation.Y;
            }
            */
        }

        /*
        void Window_InertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            var TranslateFriction = 0.1;
            var RotationalFriction = 0.1;

            double num = (TranslateFriction == 1.0) ? 1.0 : (-0.00666 * Math.Log(1.0 - TranslateFriction));
            double num2 = e.InitialVelocities.LinearVelocity.Length * num;
            InertiaTranslationBehavior behavior = new InertiaTranslationBehavior
            {
                InitialVelocity = e.InitialVelocities.LinearVelocity,
                DesiredDeceleration = Math.Max(num2, 0.0)
            };
            e.TranslationBehavior = behavior;
            double num3 = (RotationalFriction == 1.0) ? 1.0 : (-0.00666 * Math.Log(1.0 - RotationalFriction));
            double num4 = Math.Abs(e.InitialVelocities.AngularVelocity) * num3;
            InertiaRotationBehavior behavior2 = new InertiaRotationBehavior
            {
                InitialVelocity = e.InitialVelocities.AngularVelocity,
                DesiredDeceleration = Math.Max(num4, 0.0)
            };
            e.RotationBehavior = behavior2;
            e.Handled = true;
        }
        */
#endif
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
#if !WINDOWS_UWP
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
#else
                var dispobj = handler.Target as DependencyObject;
                if (dispobj != null)
                {
                    Utilities.RunOnUIThread.RunIfRequired(() => OnPropertyChanged(propertyName));
                }
                else
                    handler(this, e);
#endif
            }
        }

        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion INotifyPropertyChanged Members

        #region IDisposable Members

        bool bDisposed = false;

        DispatcherTimer delayTerminate;
        public void Dispose()
        {
#if !WINDOWS_UWP
            if (bLoaded && Document != null)
            {
                Document.OnScreenUnloaded();
                if (!Document.TerminateScriptCode())
                {
                    if (delayTerminate == null)
                    {
                        delayTerminate = new DispatcherTimer();
                        delayTerminate.Interval = TimeSpan.FromSeconds(1);
                        delayTerminate.Tick += (o, e) =>
                        {
                            delayTerminate.Stop();
                            Dispose();
                        };
                    }

                    delayTerminate.Start();
                    return;
                }
            }
#endif

            if (bDisposed)
                return;
            bDisposed = true;

#if !WINDOWS_UWP
            if (d1 != null && d1.Status != DispatcherOperationStatus.Aborted &&
                d1.Status != DispatcherOperationStatus.Completed)
            {
                d1.Abort();
                // d1 = null;
            }

            ResetAutoClose();

            if (wndParent != null)
            {
                wndParent.Activated -= wnd_Activated;
                wndParent.Deactivated -= wnd_Deactivated;
                wndParent.SizeChanged -= wnd_SizeChanged;
                wndParent = null;
            }

            if (timerRefreshCommands != null)
            {
                timerRefreshCommands.Stop();
                timerRefreshCommands = null;
            }

            topListBox.ItemsSource = null;

            if (mapOriginalRect != null)
                mapOriginalRect.Clear();
            if (mapOriginalPoints != null)
                mapOriginalPoints.Clear();

            CleanAllOpenPopups();
            CleanAllOpen3DScreens();
#endif
            UnsubscribeAccessLevelControls();

            if (StringEditor != null)
                StringEditor.CultureChanged -= StringEditor_CultureChanged;

#if !WINDOWS_UWP
            if (UnitConverterEditor != null)
                UnitConverterEditor.CurrentConverterChanged -= UnitConverterEditor_CurrentConverterChanged;

            if (Document != null)
                Document.ParameterFileChanged -= Document_ParameterFileChanged;
#endif
            if (ScreenComponent != null && ScreenComponent.AuthenticationCredentialsProvider != null)
            {
                ScreenComponent.AuthenticationCredentialsProvider.UserOnline -= AuthenticationCredentialsProvider_UserOnline;
                ScreenComponent.AuthenticationCredentialsProvider.FailedLogin -= AuthenticationCredentialsProvider_FailedLogin;
                ScreenComponent.AuthenticationCredentialsProvider.FaultedProvider -= AuthenticationCredentialsProvider_FaultedProvider;
            }

#if !WINDOWS_UWP
            DestroyOwnedShortcut();
            DestroyOwnedMenus();
#endif
            if (listPropertyChangeNotifier != null)
            {
                listPropertyChangeNotifier.ForEach(c => c.Dispose());
                listPropertyChangeNotifier.Clear();
                listPropertyChangeNotifier = null;
            }

            mapVisibilityChangeNotifier.Values.ToList().ForEach(v => v.Dispose());
            mapVisibilityChangeNotifier.Clear();

            if (bLoaded)
            {
                if (Document != null)
                {
#if !WINDOWS_UWP
                    if (String.IsNullOrEmpty(ParameterFile))
                        SaveData(GetUniqueTitle(Document));

                    Document.TerminateScriptCode();
#endif
                    Document.TerminateExecution(MainSurface);
                    Document.UnloadResources(MainSurface);

                    Document.CommandExecuting -= Document_CommandExecuting;

                    // Document.Mediator.NotifyColleagues<ScreenViewer>("ScreenViewerUnloaded", this);
                }
            }

#if !WINDOWS_UWP
            if (Document != null && !Document.LayoutView)
                SaveLayoutData(GetUniqueTitle(Document));
            MainSurface.AbortPendingOperations();
#endif
            MainSurface.Dispose();

#if !WINDOWS_UWP
            mapManAdorners.Clear();
            mapElementCamera.Clear();

            foreach (var value in mapElementTrackball.Keys)
            {
                mapElementTrackball[value].EventSource = null;
                value.PreviewTouchUp -= viewport3D_TouchUp;
                value.PreviewTouchDown -= viewport3D_TouchDown;
            }
            mapElementTrackball.Clear();

            MainSurface.ManipulationStarting -= Window_ManipulationStarting;
            MainSurface.ManipulationDelta -= Window_ManipulationDelta;
            // MainSurface.ManipulationInertiaStarting -= Window_InertiaStarting;
            MainSurface.ManipulationCompleted -= Window_ManipulationCompleted;
#endif
            if (Document != null)
                Document.Dispose();
        }

        #endregion IDisposable Members

#if !WINDOWS_UWP
        void Controller_ModelChanged(object sender, DevExpress.Xpf.LayoutControl.LayoutControlModelChangedEventArgs e)
        {
            bLayoutChanged = true;
        }
#endif
        private void OnLoginUser(object sender,
#if !WINDOWS_UWP
            ExecutedRoutedEventArgs e)
#else
            EventArgs e)
#endif
        {
#if !WINDOWS_UWP
            if (e != null)
                e.Handled = true;
#endif
            if (StringEditor != null && Document != null)
                previousCulture = StringEditor.GetActiveCulture(Document);
            if (UnitConverterEditor != null && Document != null)
                previousConverter = UnitConverterEditor.GetActiveConverter(Document);
            ScreenComponent.AuthenticationCredentialsProvider.ValidateUsingCredentialsProvider(Document, DocumentHelper.GetRootParent(Document, traverse: true).Title, requestedRole, requestedLevel, wndParent);
            requestedRole = null;
            requestedLevel = -1;
        }

#if !WINDOWS_UWP
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
            e.CanExecute = bUserEnabled && ScreenComponent != null && ScreenComponent.AuthenticationCredentialsProvider != null &&
                Document != null && DocumentHelper.GetRootParent(Document, traverse: true) != null &&
                !ScreenComponent.AuthenticationCredentialsProvider.IsFaulted(DocumentHelper.GetRootParent(Document, traverse: true).Title);
        }
#endif
        private void OnLogoutUser(object sender,
#if !WINDOWS_UWP
            ExecutedRoutedEventArgs e)
#else
            EventArgs e)
#endif
        {
#if !WINDOWS_UWP
            if (e != null)
                e.Handled = true;
#endif
            ScreenComponent.AuthenticationCredentialsProvider.Logout(DocumentHelper.GetRootParent(Document, traverse: true).Title);
            currentRole = null;
            currentLevel = -1;
            currentAccessMask = 0;
        }

#if !WINDOWS_UWP
        private void CanLogoutUser(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = bUserEnabled && Document != null && ScreenComponent != null &&
                ScreenComponent.AuthenticationCredentialsProvider != null &&
                ScreenComponent.AuthenticationCredentialsProvider.GetNumberOfUsersOnline(DocumentHelper.GetRootParent(Document, traverse: true).Title) > 0;
        }

        private void OnResetManipulation(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (zoomedControl != null)
                UnzoomControl(zoomedControl);

            (from btn in iconizer.Children.OfType<Button>() select btn).ToList().ForEach(el =>
                        {
                            UniconControl(el);
                        });

            mapElementToMan.Keys.ToList().ForEach(element =>
            {
                EnableManipulationControl(element);
            });

            listManipulatedElements.ForEach(parent =>
                {
                    if (mapTransformPreMan.ContainsKey(parent))
                    {
                        parent.RenderTransform = mapTransformPreMan[parent];
                        mapTransformPreMan.Remove(parent);
                    }
                    else
                        parent.RenderTransform = null;
                });

            listManipulatedElements.Clear();
            DeleteLayoutData(GetUniqueTitle(Document));
        }

        private void CanResetManipulation(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && !Document.LayoutView &&
                (mapElementToMan.Count > 0 || listManipulatedElements.Count > 0 ||
                 zoomedControl != null || mapRectPreIcon.Count > 0);
        }
#endif
        // int nIndex;
        bool bFillMode;
        private void OnFillMode(object sender,
#if !WINDOWS_UWP
            ExecutedRoutedEventArgs e)
#else
            EventArgs e)
#endif
        {
#if !WINDOWS_UWP
            if (e != null)
                e.Handled = true;
#endif
            //int n = LayoutGrid.Children.IndexOf(Scroller);
            //if (n >= 0)
            //{
            //    nIndex = n;
            //    LayoutGrid.Children.Remove(Scroller);
            //    viewBox.Child = Scroller;
            //    viewBox.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    viewBox.Child = null;
            //    viewBox.Visibility = Visibility.Collapsed;
            //    LayoutGrid.Children.Insert(nIndex, Scroller);
            //}
            //OnPropertyChanged("IsInFillMode");

            //if (bInitialized)
            //{
            //    var mainControl = Document.LayoutView ? layoutItems as FrameworkElement : MainSurface as FrameworkElement;
            //    Document.UpdateRepositoryItems(this, mainControl);
            //}
            bFillMode = !bFillMode;
            OnPropertyChanged("IsInFillMode");
            if (bFillMode)
                UpdateZoomFillMode();
            else
            {
                bUpdatingFitInWindow = true;
                try
                {
                    ZoomLevelX = ZoomLevelY = 1;
                }
                finally
                {
                    bUpdatingFitInWindow = false;
                }
            }

            SetZoomVisibilityItems();
            SetZoomCachedItems();
        }

        public bool IsInFillMode
        {
            get
            {
                // return viewBox.Child != null;
                return bFillMode;
            }
        }


#if !WINDOWS_UWP
        private void CanFillMode(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && !Document.LayoutView;
        }
#endif
        /*
        List<FrameworkElement> listZoomMode;
        private void OnZoomMode(object sender, ExecutedRoutedEventArgs e)
        {
            bZoomMode = !bZoomMode;
            if (listZoomMode != null)
            {
                listZoomMode.ForEach(element => element.IsHitTestVisible = true);
                listZoomMode = null;
            }
            else
            {
                listZoomMode = (from c in MainSurface.Children.OfType<FrameworkElement>()
                               where c.IsHitTestVisible == true select c).ToList();
                listZoomMode.ForEach(element => element.IsHitTestVisible = false);
            }
        }

        bool bZoomMode;
        public bool IsInZoomMode
        {
            get
            {
                return bZoomMode;
            }
        }

        private void CanZoomMode(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && !Document.LayoutView;
        }
        */

#if !WINDOWS_UWP
        bool bLayoutChanged;
        private void OnLayoutEdit(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            layoutItems.IsCustomization = !layoutItems.IsCustomization;

            if (!layoutItems.IsCustomization)
            {
                layoutItems.Controller.ModelChanged -= Controller_ModelChanged;
                if (bLayoutChanged)
                    SaveLayout(GetUniqueTitle(Document));
            }
            else
            {
                bLayoutChanged = false;
                layoutItems.Controller.ModelChanged += Controller_ModelChanged;
            }
        }


        private void CanLayoutEdit(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && Document.LayoutView && Document.LayoutViewEditable;
        }
#endif
        internal bool CanUnload()
        {
            return Document == null || Document.CanUnload();
        }

        internal uint DelayUnload()
        {
            if (Document == null)
                return 0;
            return Document.DelayUnloadSecs;
        }

#if !WINDOWS_UWP
        internal bool ShowNavigation
        {
            get
            {
                return Document != null && Document.ShowNavigation;
            }
        }

        internal bool ShowHeader
        {
            get
            {
                return Document != null && Document.ShowHeader;
            }
        }

        internal bool ShowTabStrip
        {
            get
            {
                return Document != null && Document.ShowTabStrip;
            }
        }

        internal byte MonitorNumber
        {
            get
            {
                if (monitorNumberMap == null)
                {
                    monitorNumberMap = new Dictionary<byte, byte>();
                    if (Properties.Settings.Default.MultiMonitorMap != null)
                    {
                        var tuples = Properties.Settings.Default.MultiMonitorMap.Split('|');
                        foreach (var tuple in tuples)
                        {
                            var indexes = tuple.Split(',');
                            if (indexes.Count() == 2)
                            {
                                byte res0;
                                byte res1;
                                var bConv0 = byte.TryParse(indexes[0], out res0);
                                var bConv1 = byte.TryParse(indexes[1], out res1);
                                if (bConv0 && bConv1)
                                    monitorNumberMap[res0] = res1;
                            }
                        }
                    }
                }
                if (nRequestedMonitor != -1)
                {
                    var nRequestedByteMonitor = (byte)Math.Min(nRequestedMonitor, 255);
                    if (monitorNumberMap.ContainsKey(nRequestedByteMonitor))
                        return monitorNumberMap[nRequestedByteMonitor];
                    else
                        return nRequestedByteMonitor;
                }
                if (Document == null)
                {
                    return 0;
                }

                byte docMon;
                var res = byte.TryParse(Document.MonitorNumber.ToString(), out docMon);
                if (res && monitorNumberMap.ContainsKey(docMon))
                    return monitorNumberMap[docMon];
                else
                    return Document.MonitorNumber;
            }
        }
#endif
        internal double Top
        {
            get
            {
                if (Document == null)
                    return 0;
                return Document.Top;
            }
        }

        internal double Left
        {
            get
            {
                if (Document == null)
                    return 0;
                return Document.Left;
            }
        }

#if !WINDOWS_UWP
        internal WindowState WindowState
        {
            get
            {
                if (Document == null)
                    return WindowState.Normal;
                return Document.WindowState;
            }
        }

        internal int UntranslatedMonitorNumber
        {
            get
            {
                return nRequestedMonitor;
            }
        }
#endif
        #region Layout Persistance

#if !WINDOWS_UWP
        static String GetUniqueTitle(IDocument doc)
        {
            if (doc.Parent == null)
                return doc.Title;
            return String.Format("{0}_{1}", doc.Parent.Title, doc.Title);
        }

        static String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.LayoutViewer.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

        static String GetStoreDataFileName(String title)
        {
            return String.Format("{0}.{1}.Data.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

        static String GetStoreLayoutDataFileName(String title)
        {
            return String.Format("{0}.{1}.LayoutData.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

        void SaveData(String title)
        {
            if (Properties.Settings.Default.DisableTagCache)
                return;

            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title) || Document == null)
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreDataFileName(title), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        try
                        {
                            var serializer = new DataContractSerializer(typeof(IDictionary<String, Object>));
                            serializer.WriteObject(writer, Document.dataContextExpando as IDictionary<String, Object>);
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

        IDictionary<String, Object> LoadData(String title)
        {
            if (Properties.Settings.Default.DisableTagCache ||
                Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Document == null)
                return null;

            try
            {
                var isoStorage = GetStorage();
                if (null != isoStorage && !String.IsNullOrEmpty(title))
                {
                    using (var stream = new IsolatedStorageFileStream(GetStoreDataFileName(title), FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            var serializer = new DataContractSerializer(typeof(IDictionary<String, Object>));
                            var map = serializer.ReadObject(reader) as IDictionary<String, Object>;
                            var dataMap = Document.dataContextExpando as IDictionary<String, Object>;
                            foreach (var entry in map)
                            {
                                dataMap[entry.Key] = entry.Value;
                                if (!Document.mapDataContextExpando.ContainsKey(entry.Key))
                                {
                                    var mi = new MonitoredItemViewModel(true);
                                    mi.DataValue = new DataValue(new Variant(entry.Value), StatusCodes.BadWaitingForInitialData);
                                    Document.mapDataContextExpando.Add(entry.Key, mi);
                                }
                            }
                            return map;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        void DeleteLayoutData(String title)
        {
            var isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(title))
                return;
            using (var stream = new IsolatedStorageFileStream(GetStoreLayoutDataFileName(title), FileMode.Create, isoStorage))
            {
            }
        }

        void SaveLayoutData(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreLayoutDataFileName(title), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        try
                        {
                            var list = GetListElementLayout();

                            var serializer = new DataContractSerializer(typeof(List<ScreenLayoutData>));
                            serializer.WriteObject(writer, list);
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

        List<ScreenLayoutData> LoadLayoutData(String title)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return null;

            try
            {
                var isoStorage = GetStorage();
                if (null != isoStorage && !String.IsNullOrEmpty(title))
                {
                    using (var stream = new IsolatedStorageFileStream(GetStoreLayoutDataFileName(title), FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            var serializer = new DataContractSerializer(typeof(List<ScreenLayoutData>));
                            var list = serializer.ReadObject(reader) as List<ScreenLayoutData>;
                            RestoreElementLayout(list);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return null;
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

                    if (Document.LayoutView)
                    {
                        using (var writer = XmlWriter.Create(stream, settings))
                        {
                            layoutItems.WriteToXML(writer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void LoadLayout(String title)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            bool bLoaded = false;
            try
            {
                var isoStorage = GetStorage();
                if (null != isoStorage && !String.IsNullOrEmpty(title))
                {
                    using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        if (Document.LayoutView)
                        {
                            using (var reader = XmlReader.Create(stream, settings))
                            {
                                layoutItems.ReadFromXML(reader);
                                bLoaded = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            if (bLoaded || !Document.LayoutView)
                return;

            try
            {
                using (var stream = new FileStream(ScreenDocument.GetLayoutFileName(Document.FullPath), FileMode.OpenOrCreate))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (var reader = XmlReader.Create(stream, settings))
                    {
                        layoutItems.ReadFromXML(reader);
                        bLoaded = true;
                    }
                }
            }
            catch (Exception ex)
            {
                layoutItems.IsCustomization = true;
            }
        }
#endif
        #endregion

#if !WINDOWS_UWP
        private void ClickRecentButton(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null)
                return;
            var uri = btn.DataContext as Uri;
            if (uri == null)
                return;
            OnOpenRecentUri(uri);
        }

        internal void UpdateRecentList(List<Uri> list)
        {
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (!bDisposed)
                {
                    if (list.Contains(Uri))
                        list.Remove(Uri);
                    topListBox.ItemsSource = list;
                    topListBox.Visibility = list.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                }
            });
        }

        private void scroller_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void btnLoginClick(object sender, RoutedEventArgs e)
        {
            OnLoginUser(null, null);
        }
#endif

        #region Popup

#if !WINDOWS_UWP
        readonly System.Collections.Specialized.OrderedDictionary mapUriViewPopup = new System.Collections.Specialized.OrderedDictionary();
        internal void OpenPopup(Uri uri, String parameterFile = null, 
            double dX = Double.NaN, double dY = Double.NaN, double dWidth = Double.NaN, double dHeight = Double.NaN)
        {
            if (mapUriViewPopup.Contains(uri))
            {
                var codePopup = mapUriViewPopup[uri] as Popup;
                if (!Double.IsNaN(dX) && !Double.IsNaN(dY))
                {
                    codePopup.Placement = PlacementMode.AbsolutePoint;
                    codePopup.HorizontalOffset = dX;
                    codePopup.VerticalOffset = dY;
                }
                else
                {
                    codePopup.HorizontalOffset = codePopup.VerticalOffset = 0;
                    codePopup.Placement = /*(target != null ? PlacementMode.Right : */PlacementMode.MousePoint;
                }
                codePopup.PlacementTarget = ActiveView as UIElement;

                codePopup.IsOpen = true;
            }
            else
            {
                using (var cursor = new WaitCursor())
                {
                    var viewer = new ScreenViewer(ScreenComponent, uri, DocumentParent, ActiveView,
                        isModal: true, parameterFile: parameterFile, bIsLayout: true, customWidth: dWidth, customHeight: dHeight);

                    var codePopup = new Popup();
                    codePopup.Closed += (o, e) =>
                        {
                            if (!String.IsNullOrEmpty(parameterFile) && mapUriViewPopup.Contains(uri))
                            {
                                mapUriViewPopup.Remove(uri);
                                viewer.Dispose();
                            }
                        };

                    codePopup.AllowsTransparency = true;
                    codePopup.Child = viewer;
                    codePopup.ClipToBounds = false;
                    codePopup.Placement = /*(target != null ? PlacementMode.Right : */PlacementMode.MousePoint;
                    codePopup.PopupAnimation = PopupAnimation.Slide;
                    codePopup.StaysOpen = false;
                    codePopup.Focusable = true;

                    if (!Double.IsNaN(dX) && !Double.IsNaN(dY))
                    {
                        codePopup.Placement = PlacementMode.AbsolutePoint;
                        codePopup.HorizontalOffset = dX;
                        codePopup.VerticalOffset = dY;
                    }
                    codePopup.PlacementTarget = ActiveView as UIElement;
                    codePopup.IsOpen = true;

                    mapUriViewPopup.Add(uri, codePopup);
                }
            }
        }

        internal bool CloseAllPopup()
        {
            bool bRet = false;
            for (int ii = mapUriViewPopup.Values.Count - 1; ii >= 0; ii--)
            {
                var popup = mapUriViewPopup[ii] as Popup;
                bRet |= popup.IsOpen;
                popup.IsOpen = false;
            }
            return bRet;
        }

        void CleanAllOpenPopups()
        {
            for (int ii = mapUriViewPopup.Values.Count - 1; ii >= 0; ii--)
            {
                var popup = mapUriViewPopup[ii] as Popup;
                popup.IsOpen = false;
                var viewer = popup.Child as ScreenViewer;
                popup.Child = null;
                viewer.Dispose();
            }
            mapUriViewPopup.Clear();
        }
#endif
        #endregion

        #region Shortcuts
#if !WINDOWS_UWP
        private void ScreenViewerControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ScreenComponent.ShortcutEditor == null || Document == null)
                return;
            e.Handled = ScreenComponent.ShortcutEditor.ExecuteGestureOnDown(Document, e);
        }

        private void ScreenViewerControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (ScreenComponent.ShortcutEditor == null || Document == null)
                return;
            e.Handled = ScreenComponent.ShortcutEditor.ExecuteGestureOnUp(Document, e);
        }

        List<String> shortcutSpeechCommands;
        void ActivateCurrentShortcut()
        {
            if (ScreenComponent.ShortcutEditor == null || Document == null || IsLayout || bTest)
                return;

            var Parent = Document.Parent;
            if (Parent == null)
                Parent = Document;
            var menuTypeShortcut = (ScreenComponent.ShortcutEditor as IDocumentManager).TypeLabel;
            var uri = Parent.MakeRelativeUri(Uri);
            var path = uri.GetPathString().Replace(String.Format("{0}\\", ScreenComponent.TypeLabel), 
                String.Format("{0}\\", menuTypeShortcut));
            path = path.Replace(String.Format("{0}/", ScreenComponent.TypeLabel), 
                String.Format("{0}/", menuTypeShortcut));

            var ret = ScreenComponent.ShortcutEditor.Activate(Document, new Uri(path, UriKind.RelativeOrAbsolute),
                                    Document.SessionString, true);
            if (ret == null)
                shortcutSpeechCommands = null;
            else
                shortcutSpeechCommands = new List<String>(ret);
        }

        void DestroyOwnedShortcut()
        {
            if (ScreenComponent.ShortcutEditor == null || Document == null)
                return;

            ScreenComponent.ShortcutEditor.Terminate(Document);
        }

        readonly List<MenuBase> ownedMenus = new List<MenuBase>();
        void ActivateCurrentMenuBar()
        {
            if (ScreenComponent.MenuEditor == null || Document == null || IsLayout || bTest)
                return;

            var Parent = Document.Parent;
            if (Parent == null)
                Parent = Document;
            var menuTypeLabel = (ScreenComponent.MenuEditor as IDocumentManager).TypeLabel;
            var uri = Parent.MakeRelativeUri(Uri);
            var path = uri.GetPathString().Replace(String.Format("{0}\\", ScreenComponent.TypeLabel), 
                String.Format("{0}\\", menuTypeLabel));
            path = path.Replace(String.Format("{0}/", ScreenComponent.TypeLabel), 
                String.Format("{0}/", menuTypeLabel));
            path = path.Replace(ScreenComponent.FileType, "");

            if ((from m in ownedMenus where (m.Tag as string) == uri.OriginalString select m).FirstOrDefault() == null)
            {
                var menu = ScreenComponent.MenuEditor.GetMenu(Document, new Uri(path, UriKind.RelativeOrAbsolute),
                                        Document.SessionString, !IsModal, false, false);
                if (menu == null)
                    return;
                menu.Tag = uri.OriginalString;
                if (gridMenu.Children.Count > 1)
                {
                    var menuold = gridMenu.Children[gridMenu.Children.Count - 1] as MenuBase;
                    gridMenu.Children.RemoveAt(gridMenu.Children.Count - 1);
                    if (menuold != null)
                    {
                        if (ownedMenus.Contains(menuold))
                            ownedMenus.Remove(menuold);
                        ScreenComponent.MenuEditor.TerminateMenu(menuold);
                    }
                }
                gridMenu.Children.Add(menu);
                if (!ownedMenus.Contains(menu))
                    ownedMenus.Add(menu);
            }
        }

        void LoadContextMenus()
        {
            if (ScreenComponent.MenuEditor == null || Document == null)
                return;

            var Parent = Document.Parent;
            if (Parent == null)
                Parent = Document;

            var listMenuEntities = Document.GetContextMenuList();
            listMenuEntities.ForEach(el =>
                    {
                        var child = Document.FindInnerControl(MainSurface, el);
                        if (child is ContentControl && (child as ContentControl).Content is UIElement &&
                            !(child is UserControl))
                            child = (child as ContentControl).Content as FrameworkElement;
                        if (child != null)
                        {
                            var menuUri = Document.MapScreenEntities[el].MenuName;

                            //var uri = Parent.MakeRelativeUri(menuUri);
                            //var path = uri.GetPathString().Replace(String.Format("{0}\\", ScreenComponent.TypeLabel), "");
                            //path = path.Replace(String.Format("{0}/", ScreenComponent.TypeLabel), "");
                            var menu = (from m in ownedMenus where (m.Tag as string) == menuUri.OriginalString select m).FirstOrDefault();
                            if (menu == null)
                                menu = ScreenComponent.MenuEditor.GetMenu(Document, menuUri,
                                                    Document.SessionString, false, true, false);
                            if (menu != null)
                            {
                                menu.Tag = menuUri.OriginalString;
                                if (!ownedMenus.Contains(menu))
                                    ownedMenus.Add(menu);
                                child.ContextMenu = menu as ContextMenu;

                                if (Document.MapScreenEntities[el].ShowMenuOnLeft && !ScreenDocument.GetDisableShowMenuOnLef(child))
                                {
                                    child.PreviewMouseDown += (o, e) =>
                                        {
                                            if (e.ChangedButton == MouseButton.Left)
                                            {
                                                child.ContextMenu.IsOpen = true;
                                            }
                                        };
                                }
                            }
                        }
                    });
        }

        void DestroyOwnedMenus()
        {
            if (ScreenComponent.MenuEditor == null || Document == null)
                return;

            if (gridMenu.Children.Count > 1)
                gridMenu.Children.RemoveAt(gridMenu.Children.Count - 1);

            ownedMenus.ForEach(menu =>
                {
                    ScreenComponent.MenuEditor.TerminateMenu(menu);
                });
            ownedMenus.Clear();
        }

        bool bActive;
        internal void SetActiveViewer(bool b)
        {
            bool bChanged = bActive != b;
            bActive = b;

            if (Document != null && Document.KeepAlwaysInMemory)
                Document.SetInIdle(!bActive);
        }

        //DelayedSingleActionInvoker delayActivating;
        //bool bBeingActivatedOnce;
        bool isActive;
        void wnd_Activated(object sender, EventArgs e)
        {
            if (Document != null)
            {
                if (IsModal && !IsTile && !IsLayout || bActive)
                {
                    try
                    {
                        var relative = DocumentParent.MakeRelativeUri(new Uri(Document.FullPath, UriKind.RelativeOrAbsolute));
                        SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.ActiveScreen, WPFUtilities.Converters.UriConverter.ToString(relative, DocumentParent.fileSystemProviderBase != null));
                    }
                    catch
                    {
                        SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.ActiveScreen, String.Empty);
                    }

                    bool wasActive = isActive;
                    isActive = true;
                    if (!wasActive)
                        Document.OnActivated();
                }

                if (wndParent != null && Document.WindowOpacityOnlyInactive && IsModal)
                    wndParent.Opacity = 1;
            }

            if (bTest || IsLayout || IsModal)
                return;

            //if (!bBeingActivatedOnce)
            //{
            //    bBeingActivatedOnce = true;
            //    if (delayActivating == null)
            //        delayActivating = new DelayedSingleActionInvoker(() =>
            //                                {
            //                                    if (!bDisposed)
            //                                        UpdateWindowPosition(wndParent);
            //                                }, TimeSpan.FromMilliseconds(500));

            //    if (wndParent != null && Document != null)
            //    {
            //        delayActivating.BeginInvoke();
            //    }
            //}

            ActivateCurrentShortcut();

            MainSurface.Focusable = true;
            MainSurface.Focus();
        }

        void wnd_Deactivated(object sender, EventArgs e)
        {
            if (Document != null)
            {
                bool wasActive = isActive;
                isActive = false;
                if (wasActive)
                    Document.OnDeactivated();

                if (wndParent != null && Document.WindowOpacityOnlyInactive && IsModal)
                    wndParent.Opacity = Document.WindowOpacity;
            }
        }
#endif
        #endregion

        #region 3d Click

#if !WINDOWS_UWP
        void CheckClickOn3DModel(InputEventArgs e)
        {
            var element = e.Source as FrameworkElement;

            do
            {
                var name = Document.GetEntityName(element, false);
                /*
                if (String.IsNullOrEmpty(name))
                {
                    if (element != MainSurface && !MainSurface.Children.Contains(element))
                    {
                        while ((element = LogicalTreeHelper.GetParent(element) as FrameworkElement) != null)
                        {
                            if (MainSurface.Children.Contains(element))
                                break;
                        }
                    }

                    if (element == null)
                        element = e.Source as FrameworkElement;
                    if (element == null || String.IsNullOrEmpty(element.Name))
                        return;

                    name = element.Name;
                }
                */

                if (!String.IsNullOrEmpty(name) && Document.MapScreenEntities.ContainsKey(name))
                {
                    var entry = Document.MapScreenEntities[name];

                    var listViewport3Ds = element.GetChildrenOfType<Viewport3D>().ToList();
                    if (listViewport3Ds.Count == 0 && element is Viewport3D)
                        listViewport3Ds.Add(element as Viewport3D);
                    foreach (var viewport3D in listViewport3Ds)
                    {
                        Point position = new Point(-1, -1);
                        if (e is TouchEventArgs)
                        {
                            var te = e as TouchEventArgs;
                            position = te.GetTouchPoint(viewport3D).Position;
                        }
                        else if (e is MouseButtonEventArgs)
                        {
                            var me = e as MouseButtonEventArgs;
                            position = me.GetPosition(viewport3D);
                        }

                        var rayMeshResult = VisualTreeHelper.HitTest(viewport3D, position) as RayMeshGeometry3DHitTestResult;

                        //VisualTreeHelper.HitTest(viewport3D, null, (res) =>
                        //    {
                        //        if (res.VisualHit is RayMeshGeometry3DHitTestResult)
                        //        {
                        //        }
                        //        return HitTestResultBehavior.Stop;
                        //    }, new PointHitTestParameters(position));

                        if (rayMeshResult != null && rayMeshResult.ModelHit != null)
                        {
                            if (rayMeshResult.ModelHit is GeometryModel3D)
                            {
                                var model = rayMeshResult.ModelHit as GeometryModel3D;
                                // var hash = Utilities.WPF.DependencyObjectExtensions.Generate3DModelName(model);
                                var hash = Utilities.WPF.DependencyObjectExtensions.RegisterModel3D(MainSurface, element, model);

                                if (entry.Execute3DInnerCommand(hash))
                                {
                                    model.Scale3D(-0.2, 100, false, true, new SineEase(), isRelative: true);
                                    e.Handled = true;
                                    break;
                                }
                            }
                        }
                    }

                    bool bIsCommandSource = element is ICommandSource;
                    if (!bIsCommandSource && element is ContentControl)
                    {
                        var controlelement = (element as ContentControl).Content as FrameworkElement;
                        if (controlelement != null && controlelement is ICommandSource)
                            bIsCommandSource = true;
                    }

                    if (!e.Handled && entry.HasCommands && element != null && !bIsCommandSource &&
                        entry.CanExecuteCommands())
                    {
                        var el = e.Source as FrameworkElement;
                        var el2 = e.OriginalSource as FrameworkElement;
                        if (el2 != null)
                            el2.ReleaseMouseCapture();
                        if (el == null)
                            el = element;
                        el.ReleaseMouseCapture();
                        element.Scale(-0.2, 100, false, true, new SineEase(), isRelative: true, completed: (ob, ev) =>
                        {
                            element.Scale(0, -1, false, true, new SineEase(), isRelative: true);
                        });
                        // e.Handled = true;
                        Dispatcher.BeginInvokeAsynchronously(() =>
                        {
                            entry.ExecuteCommands();
                        });
                        break;
                    }
                }

                if (element != null)
                    element = LogicalTreeHelper.GetParent(element) as FrameworkElement;
            } while (element != null);
        }

        GeometryModel3D lastModelSelected;
        void CheckMouseMoveOn3D(MouseEventArgs e)
        {
            var element = e.Source as FrameworkElement;

            do
            {
                var name = Document.GetEntityName(element, false);
                /*
                if (String.IsNullOrEmpty(name))
                {
                    if (element != MainSurface && !MainSurface.Children.Contains(element))
                    {
                        while ((element = LogicalTreeHelper.GetParent(element) as FrameworkElement) != null)
                        {
                            if (MainSurface.Children.Contains(element))
                                break;
                        }
                    }

                    if (element == null)
                        element = e.Source as FrameworkElement;
                    if (element == null || String.IsNullOrEmpty(element.Name))
                        return;

                    name = element.Name;
                }
                */

                if (!String.IsNullOrEmpty(name) && Document.MapScreenEntities.ContainsKey(name))
                {
                    var entry = Document.MapScreenEntities[name];
                    var listViewport3Ds = element.GetChildrenOfType<Viewport3D>().ToList();
                    if (listViewport3Ds.Count == 0 && element is Viewport3D)
                        listViewport3Ds.Add(element as Viewport3D);
                    foreach (var viewport3D in listViewport3Ds)
                    {
                        var position = e.GetPosition(viewport3D);

                        var rayMeshResult = VisualTreeHelper.HitTest(viewport3D, position) as RayMeshGeometry3DHitTestResult;

                        //VisualTreeHelper.HitTest(viewport3D, null, (res) =>
                        //    {
                        //        if (res.VisualHit is RayMeshGeometry3DHitTestResult)
                        //        {
                        //        }
                        //        return HitTestResultBehavior.Stop;
                        //    }, new PointHitTestParameters(position));

                        if (rayMeshResult != null && rayMeshResult.ModelHit != null)
                        {
                            if (rayMeshResult.ModelHit is GeometryModel3D)
                            {
                                var model = rayMeshResult.ModelHit as GeometryModel3D;
                                // var hash = Utilities.WPF.DependencyObjectExtensions.Generate3DModelName(model);
                                var hash = Utilities.WPF.DependencyObjectExtensions.RegisterModel3D(MainSurface, element, model);

                                if (entry.Has3DInnerCommand(hash))
                                {
                                    if (lastModelSelected != model)
                                    {
                                        model.Scale3D(0.2, 100, false, true, new SineEase(), isRelative: true);
                                        lastModelSelected = model;
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }

                if (element != null)
                    element = LogicalTreeHelper.GetParent(element) as FrameworkElement;
            } while (element != null);

            lastModelSelected = null;
        }

        private void MainSurface_MouseMove(object sender, MouseEventArgs e)
        {
            CheckMouseMoveOn3D(e);
        }

        Point lastMouseDownPoint;
        private void MainSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastMouseDownPoint = e.GetPosition(MainSurface);
        }

        private void MainSurface_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point currentTapPosition = e.GetPosition(MainSurface);
            bool tapsAreCloseInDistance = GetDistanceBetweenPoints(currentTapPosition, lastMouseDownPoint) < 40;
            lastMouseDownPoint = currentTapPosition;
            if (e.ChangedButton == MouseButton.Left && tapsAreCloseInDistance)
                CheckClickOn3DModel(e);
        }
#endif
#endregion

#region 3D Screens

#if !WINDOWS_UWP
        void Load3DScreens()
        {
            var list3DScreenEntities = (from c in Document.MapScreenEntities// .AsParallel()
                                        where c.Value.GetListInner3DScreens() != null &&
                                              c.Value.GetListInner3DScreens().Count > 0
                                        select c.Key).ToList();

            list3DScreenEntities.ForEach(name =>
                {
                    var child = Document.FindInnerControl(MainSurface, name);
                    if (child != null)
                    {
                        var list3Ds = child.GetChildrenOfType<Viewport3D>().ToList();
                        if (child is Viewport3D)
                            list3Ds.Insert(0, child as Viewport3D);
                        if (list3Ds.Count > 0)
                        {
                            var viewport3D = list3Ds.First();
                            ModelVisual3D modelVisual3D = null;
                            var visualModels3Ds = (from c in viewport3D.Children.OfType<ModelVisual3D>() select c).ToList();
                            if (visualModels3Ds.Count > 0)
                                modelVisual3D = visualModels3Ds.First();

                            var list = Document.MapScreenEntities[name].GetListInner3DScreens();

                            list.ForEach(inner =>
                                {
                                    // var modelVisual3D = DependencyObjectExtensions.GetMatchingModelVisual3D(child, inner);
                                    var model = Document.MapScreenEntities[name].MapHashInner3DEntities[inner].Entity3D;
                                    if (model != null && modelVisual3D != null)
                                    {
                                        MeshGeometry3D meshGeometry = null;
                                        Transform3D transform3d = null;
                                        if (model is Model3DGroup)
                                        {
                                            meshGeometry = Utilities.WPF.DependencyObjectExtensions.GetMeshGeometry(model as Model3DGroup);
                                            transform3d = HelixToolkit.Wpf.Viewport3DHelper.GetTransform(modelVisual3D, model) as Transform3D;
                                        }
                                        else if (model is GeometryModel3D)
                                        {
                                            var geometryModel3D = (model as GeometryModel3D);

                                            meshGeometry = new MeshGeometry3D();
                                            var childmeshGeometry = geometryModel3D.Geometry as MeshGeometry3D;

                                            foreach (var normal in childmeshGeometry.Normals)
                                                meshGeometry.Normals.Add(normal);
                                            foreach (var position in childmeshGeometry.Positions)
                                                meshGeometry.Positions.Add(position);
                                            foreach (var texture in childmeshGeometry.TextureCoordinates)
                                                meshGeometry.TextureCoordinates.Add(texture);
                                            foreach (var triangle in childmeshGeometry.TriangleIndices)
                                                meshGeometry.TriangleIndices.Add(triangle);

                                            transform3d = HelixToolkit.Wpf.Viewport3DHelper.GetTransform(modelVisual3D, geometryModel3D) as Transform3D;
                                        }
                                        if (meshGeometry != null)
                                            Open3DScreen(Document.MapScreenEntities[name].MapHashInner3DEntities[inner].Screen3DUri,
                                                            meshGeometry, modelVisual3D, transform3d,
                                                            Document.MapScreenEntities[name].MapHashInner3DEntities[inner].Screen3DParameter);
                                    }
                                });
                        }
                    }
                });
        }

        public static MeshGeometry3D AddPlaneToMesh(MeshGeometry3D mesh, Vector3D normal, Point3D upperLeft, Point3D lowerLeft, Point3D lowerRight, Point3D upperRight)
        {
            int offset = mesh.Positions.Count;

            mesh.Positions.Add(upperLeft);
            mesh.Positions.Add(lowerLeft);
            mesh.Positions.Add(lowerRight);
            mesh.Positions.Add(upperRight);

            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);

            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

            mesh.TriangleIndices.Add(offset + 0);
            mesh.TriangleIndices.Add(offset + 1);
            mesh.TriangleIndices.Add(offset + 2);
            mesh.TriangleIndices.Add(offset + 0);
            mesh.TriangleIndices.Add(offset + 2);
            mesh.TriangleIndices.Add(offset + 3);

            return mesh;
        }

        readonly List<ScreenViewer> list3DScreens = new List<ScreenViewer>();
        void Open3DScreen(Uri uri, MeshGeometry3D model, ModelVisual3D modelVisual3D, Transform3D transform3D,
            String parameterFile = null)
        {
            var bound = model.Bounds;

            model.Normals.Clear();
            model.Positions.Clear();
            model.TextureCoordinates.Clear();
            model.TriangleIndices.Clear();

            AddPlaneToMesh(model,
                      new Vector3D(0, 0, 1),
                      new Point3D(-0.5 * bound.SizeX, 0.5 * bound.SizeY, 0.501 * bound.SizeZ),
                      new Point3D(-0.5 * bound.SizeX, -0.5 * bound.SizeY, 0.501 * bound.SizeZ),
                      new Point3D(0.5 * bound.SizeX, -0.5 * bound.SizeY, 0.501 * bound.SizeZ),
                      new Point3D(0.5 * bound.SizeX, 0.5 * bound.SizeY, 0.501 * bound.SizeZ));

            using (var cursor = new WaitCursor())
            {
                uri = Document.MakeAbosoluteUri(uri);

                var viewer = new ScreenViewer(ScreenComponent, uri, DocumentParent, ActiveView,
                    isModal: true, parameterFile: parameterFile, bIsLayout: true);
                viewer.toolbar.Visibility = Visibility.Collapsed;

                list3DScreens.Add(viewer);

                var myDiffuseMaterial = new DiffuseMaterial(Brushes.White);
                Viewport2DVisual3D.SetIsVisualHostMaterial(myDiffuseMaterial, true);
                var viewport2d = new Viewport2DVisual3D()
                {
                    Geometry = model,
                    Visual = viewer,
                    Material = myDiffuseMaterial,
                    Transform = transform3D
                };

                modelVisual3D.Children.Add(viewport2d);
            }
        }

        void CleanAllOpen3DScreens()
        {
            list3DScreens.ForEach(viewer =>
            {
                viewer.Dispose();
            });
            list3DScreens.Clear();
        }
#endif
#endregion

#region Speech

#if !WINDOWS_UWP
        Dictionary<String, ScreenEntity> mapScreenEntityCommands = new Dictionary<string, ScreenEntity>(StringComparer.InvariantCultureIgnoreCase);
        static readonly String HomeCommand = "Home";
        static readonly String BackCommand = "Back";
        public List<String> GetCommandList(String locale)
        {
            if (Document == null)
                return new List<String>();

            var commands = (from c in Document.MapScreenEntities.Keys
                            where !String.IsNullOrEmpty(Document.MapScreenEntities[c].SpeechCommand) &&
                                  Document.MapScreenEntities[c].HasCommands
                            select c).ToList();
            mapScreenEntityCommands.Clear();
            commands.ForEach(command =>
                {
                    if (!mapScreenEntityCommands.ContainsKey(Document.MapScreenEntities[command].SpeechCommand))
                        mapScreenEntityCommands.Add(Document.MapScreenEntities[command].SpeechCommand, Document.MapScreenEntities[command]);
                });

            var list = new List<String>(mapScreenEntityCommands.Keys);
            if (shortcutSpeechCommands != null)
                list.AddRange(shortcutSpeechCommands);
            list.Add(HomeCommand);
            list.Add(BackCommand);

            if (StringEditor != null && !String.IsNullOrEmpty(locale))
            {
                var mapculture = StringEditor.GetListStringForCulture(Document, locale);
                if (mapculture != null)
                {
                    var listRet = new List<String>();
                    list.ForEach(key =>
                    {
                        if (mapculture.ContainsKey(key))
                            listRet.Add(mapculture[key]);
                        else
                            listRet.Add(key);
                    });

                    return listRet;
                }
            }

            return list;
        }

        public void ExecuteCommand(String command, string locale)
        {
            if (StringEditor != null && !String.IsNullOrEmpty(locale))
            {
                var mapculture = StringEditor.GetListStringForCulture(Document, locale);
                if (mapculture != null)
                {
                    var keys = (from c in mapculture where String.Compare(c.Value, command, true) == 0 select c.Key).ToList();
                    if (keys.Count > 0)
                        command = keys.First();
                }
            }

            if (command == HomeCommand)
                OnHomeGesture();
            else if (command == BackCommand)
                OnClosingGesture();
            else if (mapScreenEntityCommands.ContainsKey(command) &&
                mapScreenEntityCommands[command].CanExecuteCommands())
                mapScreenEntityCommands[command].ExecuteCommands();
            else if (shortcutSpeechCommands != null)
            {
                var found = shortcutSpeechCommands.FindAll(s => s.IndexOf(command, StringComparison.OrdinalIgnoreCase) >= 0);
                if (found.Count > 0)
                    ScreenComponent.ShortcutEditor.ExecuteCommand(Document, found.First());
            }
        }
#endif
#endregion

#if !WINDOWS_UWP
        private void OnCommandPrint(object sender, ExecutedRoutedEventArgs e)
        {
            UtilitiesPrintHelper.PrintElement(MainSurface as FrameworkElement, this.FindParent<Window>(), true, true, System.Drawing.Printing.PaperKind.A4);
        }

        private void CanCommandPrint(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
#endif

#region Style
        void LoadScreenStyle()
        {
            String fileStyle = ScreenDocument.GetStyleFileName(Uri.GetPathString());
#if !WINDOWS_UWP
            bool bTempFile = false;
            if (DocumentParent.fileSystemProviderBase != null)
            {
                if (DocumentParent.fileSystemProviderBase.Exists(new FileManagerFile(DocumentParent.fileSystemProviderBase, fileStyle)))
                {
                    var data = DocumentParent.fileSystemProviderBase.ReadFile(new FileManagerFile(DocumentParent.fileSystemProviderBase, fileStyle));
                    var tempFile = new TemporaryFile();
                    try
                    {
                        File.WriteAllBytes(tempFile.FilePath, data);
                    }
                    catch (Exception ex)
                    {
                        if (Environment.UserInteractive)
                            MessageBox.Show(ex.Message);

                        return;
                    }

                    fileStyle = tempFile.FilePath;
                    bTempFile = true;
                }
            }
#endif
            if (!File.Exists(fileStyle))
                return;

            try
            {
#if !WINDOWS_UWP
                using (var stream = File.OpenRead(fileStyle))
                {
                    var resdict = (ResourceDictionary)XamlReader.Load(stream);
                    //var resdict = new ResourceDictionary
                    //{
                    //    Source = new Uri(fileStyle, UriKind.RelativeOrAbsolute)
                    //};
                    Resources.MergedDictionaries.Add(resdict);
                }
#else
                var resdict = (ResourceDictionary)XamlReader.Load(fileStyle);
                Resources.MergedDictionaries.Add(resdict);
#endif
            }
            catch (Exception ex)
            {
#if !WINDOWS_UWP
                if (Environment.UserInteractive)
                    MessageBox.Show(ex.Message);
#endif
            }

#if !WINDOWS_UWP
            if (bTempFile)
            {
                try
                {
                    File.Delete(fileStyle);
                }
                catch { }
            }
#endif
        }
#endregion

        internal String GetParameterFile()
        {
            return ParameterFile;
        }

#region AutoClose
#if !WINDOWS_UWP
        DispatcherTimer autoCloseTimer;
        int autoCloseSeconds;
        void SetAutoClose()
        {
            if (autoCloseSeconds <= 0)
                return;
            if (autoCloseTimer != null)
                autoCloseTimer.Stop();
            else
            {
                autoCloseTimer = new DispatcherTimer();
                autoCloseTimer.Interval = TimeSpan.FromSeconds(autoCloseSeconds);
                autoCloseTimer.Tick += (o, e) =>
                {
                    if (IsModal && wndParent != null)
                        wndParent.Close();
                    else
                        OnClosingGesture();
                };

                PreviewMouseMove += ScreenViewer_PreviewMouseMove;
                PreviewKeyDown += ScreenViewer_PreviewKeyDown;
            }

            autoCloseTimer.Start();
        }

        void ResetAutoClose()
        {
            if (autoCloseTimer == null)
                return;
            autoCloseTimer.Stop();
            PreviewMouseMove -= ScreenViewer_PreviewMouseMove;
            PreviewKeyDown -= ScreenViewer_PreviewKeyDown;
            autoCloseTimer = null;
        }

        void RestartAutoCloseTimer()
        {
            if (autoCloseTimer == null)
                return;
            autoCloseTimer.Stop();
            autoCloseTimer.Start();
        }

        void ScreenViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RestartAutoCloseTimer();
        }

        void ScreenViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            RestartAutoCloseTimer();
        }
#endif
#endregion
    }

    /// <summary>
    /// This class deals with monitors.
    /// </summary>
    //internal static class Monitors
    //{
    //    private static List<Monitors.Screen> Screens = null;

    //    internal static List<Monitors.Screen> GetScreens()
    //    {
    //        Monitors.Screens = new List<Monitors.Screen>();

    //        var handler = new NativeMethods.DisplayDevicesMethods.EnumMonitorsDelegate(Monitors.MonitorEnumProc);
    //        NativeMethods.DisplayDevicesMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, handler, IntPtr.Zero); // should be sequential

    //        return Monitors.Screens;
    //    }

    //    internal static List<DISPLAY_DEVICE> GetDevices()
    //    {
    //        List<DISPLAY_DEVICE> devicesList = new List<DISPLAY_DEVICE>();
    //        uint i = 0;
    //        while (true)
    //        {
    //            DISPLAY_DEVICE d = new DISPLAY_DEVICE();
    //            d.cb = Marshal.SizeOf(d);
    //            var found = NativeMethods.DisplayDevicesMethods.EnumDisplayDevices(null, i, ref d, 0);
    //            i++;
    //            if (found)
    //                devicesList.Add(d);
    //            else
    //                break;
    //        }
    //        return devicesList;
    //    }

    //    internal static DISPLAY_DEVICE GetDeviceInfo(string deviceName)
    //    {
    //        DISPLAY_DEVICE d = new DISPLAY_DEVICE();
    //        d.cb = Marshal.SizeOf(d);
    //        var found = NativeMethods.DisplayDevicesMethods.EnumDisplayDevices(deviceName, 0, ref d, 0);
    //        return d;
    //    }

    //    internal static DEVMODE GetSettings(string deviceName)
    //    {
    //        DEVMODE dm = new DEVMODE();
    //        var ret = NativeMethods.DisplayDevicesMethods.EnumDisplaySettings(deviceName, NativeMethods.DisplayDevicesMethods.ENUM_CURRENT_SETTINGS, ref dm);
    //        return dm;
    //    }

    //    private static bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, NativeMethods.DisplayDevicesMethods.RECT rect, IntPtr dwData)
    //    {
    //        NativeMethods.DisplayDevicesMethods.MONITORINFO mi = new NativeMethods.DisplayDevicesMethods.MONITORINFO();

    //        if (NativeMethods.DisplayDevicesMethods.GetMonitorInfo(hMonitor, mi))
    //        {
    //            Monitors.Screens.Add(new Monitors.Screen(
    //                (mi.dwFlags & 1) == 1, // 1 = primary monitor
    //                mi.rcMonitor.Left,
    //                mi.rcMonitor.Top,
    //                Math.Abs(mi.rcMonitor.Right - mi.rcMonitor.Left),
    //                Math.Abs(mi.rcMonitor.Bottom - mi.rcMonitor.Top)));
    //        }

    //        return true;
    //    }

    //    /// <summary>
    //    /// Represents a display device on a single system.
    //    /// </summary>
    //    internal sealed class Screen
    //    {
    //        /// <summary>
    //        /// Initializes a new instance of the Screen class.
    //        /// </summary>
    //        /// <param name="primary">A value indicating whether the display is the primary screen.</param>
    //        /// <param name="x">The display's top corner X value.</param>
    //        /// <param name="y">The display's top corner Y value.</param>
    //        /// <param name="w">The width of the display.</param>
    //        /// <param name="h">The height of the display.</param>
    //        internal Screen(bool primary, int x, int y, int w, int h)
    //        {
    //            this.IsPrimary = primary;
    //            this.TopX = x;
    //            this.TopY = y;
    //            this.Width = w;
    //            this.Height = h;
    //        }

    //        /// <summary>
    //        /// Gets a value indicating whether the display device is the primary monitor.
    //        /// </summary>
    //        internal bool IsPrimary { get; private set; }

    //        /// <summary>
    //        /// Gets the display's top corner X value.
    //        /// </summary>
    //        internal int TopX { get; private set; }

    //        /// <summary>
    //        /// Gets the display's top corner Y value.
    //        /// </summary>
    //        internal int TopY { get; private set; }

    //        /// <summary>
    //        /// Gets the width of the display.
    //        /// </summary>
    //        internal int Width { get; private set; }

    //        /// <summary>
    //        /// Gets the height of the display.
    //        /// </summary>
    //        internal int Height { get; private set; }

    //    }
    //}

    //internal static class NativeMethods
    //{
    //    /// <summary>
    //    /// Methods for retrieving display devices.
    //    /// </summary>
    //    internal static class DisplayDevicesMethods
    //    {
    //        internal delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, NativeMethods.DisplayDevicesMethods.RECT rect, IntPtr dwData);

    //        [DllImport("user32.dll")]
    //        [return: MarshalAs(UnmanagedType.Bool)]
    //        internal static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

    //        [DllImport("user32.dll")]
    //        internal static extern bool EnumDisplayDevices(
    //        string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice,
    //        uint dwFlags);

    //        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    //        internal static extern int EnumDisplaySettings(
    //        string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

    //        public const int DMDO_DEFAULT = 0;
    //        public const int DMDO_90 = 1;
    //        public const int DMDO_180 = 2;
    //        public const int DMDO_270 = 3;

    //        public const int ENUM_CURRENT_SETTINGS = -1;

    //        /// <summary>
    //        /// Retrieves information about a display monitor.
    //        /// </summary>
    //        /// <param name="hmonitor">A handle to the display monitor of interest.</param>
    //        /// <param name="info">A pointer to a MONITORINFO or MONITORINFOEX structure that receives information about the specified display monitor.</param>
    //        /// <returns>If the function succeeds, the return value is nonzero.</returns>
    //        [DllImport("user32.dll", CharSet = CharSet.Auto)]
    //        [return: MarshalAs(UnmanagedType.Bool)]
    //        internal static extern bool GetMonitorInfo(IntPtr hmonitor, [In, Out] NativeMethods.DisplayDevicesMethods.MONITORINFO info);

    //        /// <summary>
    //        /// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
    //        /// </summary>
    //        [StructLayout(LayoutKind.Sequential)]
    //        internal struct RECT
    //        {
    //            public int Left;
    //            public int Top;
    //            public int Right;
    //            public int Bottom;
    //        }

    //        // See: https://msdn.microsoft.com/en-us/library/windows/desktop/dd183569(v=vs.85).aspx
    //        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    //        internal struct DISPLAY_DEVICE
    //        {
    //            [MarshalAs(UnmanagedType.U4)]
    //            public int cb;
    //            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    //            public string DeviceName;
    //            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    //            public string DeviceString;
    //            [MarshalAs(UnmanagedType.U4)]
    //            public DisplayDeviceStateFlags StateFlags;
    //            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    //            public string DeviceID;
    //            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    //            public string DeviceKey;
    //        }

    //        // http://www.pinvoke.net/default.aspx/Enums/DisplayDeviceStateFlags.html
    //        [Flags()]
    //        internal enum DisplayDeviceStateFlags : int
    //        {
    //            /// <summary>The device is part of the desktop.</summary>
    //            AttachedToDesktop = 0x1,
    //            MultiDriver = 0x2,
    //            /// <summary>The device is part of the desktop.</summary>
    //            PrimaryDevice = 0x4,
    //            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
    //            MirroringDriver = 0x8,
    //            /// <summary>The device is VGA compatible.</summary>
    //            VGACompatible = 0x10,
    //            /// <summary>The device is removable; it cannot be the primary display.</summary>
    //            Removable = 0x20,
    //            /// <summary>The device has more display modes than its output devices support.</summary>
    //            ModesPruned = 0x8000000,
    //            Remote = 0x4000000,
    //            Disconnect = 0x2000000
    //        }

    //        [Flags()]
    //        internal enum DM : int
    //        {
    //            Orientation = 0x00000001,
    //            PaperSize = 0x00000002,
    //            PaperLength = 0x00000004,
    //            PaperWidth = 0x00000008,
    //            Scale = 0x00000010,
    //            Position = 0x00000020,
    //            NUP = 0x00000040,
    //            DisplayOrientation = 0x00000080,
    //            Copies = 0x00000100,
    //            DefaultSource = 0x00000200,
    //            PrintQuality = 0x00000400,
    //            Color = 0x00000800,
    //            Duplex = 0x00001000,
    //            YResolution = 0x00002000,
    //            TTOption = 0x00004000,
    //            Collate = 0x00008000,
    //            FormName = 0x00010000,
    //            LogPixels = 0x00020000,
    //            BitsPerPixel = 0x00040000,
    //            PelsWidth = 0x00080000,
    //            PelsHeight = 0x00100000,
    //            DisplayFlags = 0x00200000,
    //            DisplayFrequency = 0x00400000,
    //            ICMMethod = 0x00800000,
    //            ICMIntent = 0x01000000,
    //            MediaType = 0x02000000,
    //            DitherType = 0x04000000,
    //            PanningWidth = 0x08000000,
    //            PanningHeight = 0x10000000,
    //            DisplayFixedOutput = 0x20000000
    //        }

    //        // See: https://msdn.microsoft.com/de-de/library/windows/desktop/dd162807(v=vs.85).aspx
    //        [StructLayout(LayoutKind.Sequential)]
    //        internal struct POINTL
    //        {
    //            long x;
    //            long y;
    //        }

    //        // See: https://msdn.microsoft.com/en-us/library/windows/desktop/dd183565(v=vs.85).aspx
    //        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    //        internal struct DEVMODE
    //        {
    //            public const int CCHDEVICENAME = 32;
    //            public const int CCHFORMNAME = 32;

    //            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
    //            [System.Runtime.InteropServices.FieldOffset(0)]
    //            public string dmDeviceName;
    //            [System.Runtime.InteropServices.FieldOffset(32)]
    //            public Int16 dmSpecVersion;
    //            [System.Runtime.InteropServices.FieldOffset(34)]
    //            public Int16 dmDriverVersion;
    //            [System.Runtime.InteropServices.FieldOffset(36)]
    //            public Int16 dmSize;
    //            [System.Runtime.InteropServices.FieldOffset(38)]
    //            public Int16 dmDriverExtra;
    //            [System.Runtime.InteropServices.FieldOffset(40)]
    //            public DM dmFields;

    //            [System.Runtime.InteropServices.FieldOffset(44)]
    //            Int16 dmOrientation;
    //            [System.Runtime.InteropServices.FieldOffset(46)]
    //            Int16 dmPaperSize;
    //            [System.Runtime.InteropServices.FieldOffset(48)]
    //            Int16 dmPaperLength;
    //            [System.Runtime.InteropServices.FieldOffset(50)]
    //            Int16 dmPaperWidth;
    //            [System.Runtime.InteropServices.FieldOffset(52)]
    //            Int16 dmScale;
    //            [System.Runtime.InteropServices.FieldOffset(54)]
    //            Int16 dmCopies;
    //            [System.Runtime.InteropServices.FieldOffset(56)]
    //            Int16 dmDefaultSource;
    //            [System.Runtime.InteropServices.FieldOffset(58)]
    //            Int16 dmPrintQuality;

    //            [System.Runtime.InteropServices.FieldOffset(44)]
    //            public POINTL dmPosition;
    //            [System.Runtime.InteropServices.FieldOffset(52)]
    //            public Int32 dmDisplayOrientation;
    //            [System.Runtime.InteropServices.FieldOffset(56)]
    //            public Int32 dmDisplayFixedOutput;

    //            [System.Runtime.InteropServices.FieldOffset(60)]
    //            public short dmColor;
    //            [System.Runtime.InteropServices.FieldOffset(62)]
    //            public short dmDuplex;
    //            [System.Runtime.InteropServices.FieldOffset(64)]
    //            public short dmYResolution;
    //            [System.Runtime.InteropServices.FieldOffset(66)]
    //            public short dmTTOption;
    //            [System.Runtime.InteropServices.FieldOffset(68)]
    //            public short dmCollate;
    //            [System.Runtime.InteropServices.FieldOffset(72)]
    //            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
    //            public string dmFormName;
    //            [System.Runtime.InteropServices.FieldOffset(102)]
    //            public Int16 dmLogPixels;
    //            [System.Runtime.InteropServices.FieldOffset(104)]
    //            public Int32 dmBitsPerPel;
    //            [System.Runtime.InteropServices.FieldOffset(108)]
    //            public Int32 dmPelsWidth;
    //            [System.Runtime.InteropServices.FieldOffset(112)]
    //            public Int32 dmPelsHeight;
    //            [System.Runtime.InteropServices.FieldOffset(116)]
    //            public Int32 dmDisplayFlags;
    //            [System.Runtime.InteropServices.FieldOffset(116)]
    //            public Int32 dmNup;
    //            [System.Runtime.InteropServices.FieldOffset(120)]
    //            public Int32 dmDisplayFrequency;
    //        }

    //        /// <summary>
    //        /// The MONITORINFO structure contains information about a display monitor.
    //        /// </summary>
    //        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    //        internal class MONITORINFO
    //        {
    //            internal int cbSize = Marshal.SizeOf(typeof(NativeMethods.DisplayDevicesMethods.MONITORINFO));
    //            internal NativeMethods.DisplayDevicesMethods.RECT rcMonitor = new NativeMethods.DisplayDevicesMethods.RECT();
    //            internal NativeMethods.DisplayDevicesMethods.RECT rcWork = new NativeMethods.DisplayDevicesMethods.RECT();
    //            internal int dwFlags;
    //        }
    //    }
    //}
}