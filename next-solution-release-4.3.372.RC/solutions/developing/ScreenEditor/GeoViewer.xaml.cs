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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Map;
using DevExpress.Map;
using ScreenSettings;
using DocumentManager.ComponentService;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using Utilities.Animations;
using System.Dynamic;
using System.Xml.Linq;
using ScreenManager.ComponentService;
using Utilities;
using OPCUAViewModel;
using ViewModelLib;
using System.ComponentModel;
using UFInterfaces;
using WPFUtilities;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using System.Threading.Tasks;
using StringManager.ComponentService;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for GeoViewer.xaml
    /// </summary>
    public partial class GeoViewer : UserControl, IDisposable, IEntityReference
    {
        #region Declarations

        bool bLoaded;
        bool bLoadedNow;
        bool bCultureChanged;
        int nExpanderExpanded;
        readonly ScreenManagerComponent ScreenComponent;
        readonly IScreenController screenController;
        readonly ScreenTransitionViewer container;
        readonly IDocument DocumentParent;
        readonly IScreenController backController;

        List<PropertyObserver<OPCUAEntityReference>> listPropertyObserverEntityReference = new List<PropertyObserver<OPCUAEntityReference>>();
        List<PropertyObserver<MonitoredItemViewModel>> listPropertyObserverMonitoredItem = new List<PropertyObserver<MonitoredItemViewModel>>();
        List<PropertyObserver<HistoryReadViewModel>> listPropertyObserverHistoryItem = new List<PropertyObserver<HistoryReadViewModel>>();
        List<OPCUAEntityReference> listReferences = new List<OPCUAEntityReference>();
        Dictionary<Uri, List<MapItem>> mapControllerToPath = new Dictionary<Uri, List<MapItem>>();
        Dictionary<Uri, MonitoredItemViewModel> mapControllerLatMon = new Dictionary<Uri, MonitoredItemViewModel>();
        Dictionary<Uri, MonitoredItemViewModel> mapControllerLonMon = new Dictionary<Uri, MonitoredItemViewModel>();
        Dictionary<Uri, GeoViewerItemModel> mapControllerModel = new Dictionary<Uri, GeoViewerItemModel>();
        List<MapCustomElement> listElementLatInError = new List<MapCustomElement>();
        List<MapCustomElement> listElementLonInError = new List<MapCustomElement>();

        PropertyChangeNotifier notifierZoom;
        PropertyChangeNotifier notifierCenterPoint;

        DelayedSingleActionInvoker delayInvoker;

        bool bMapLiveScreenHide;
        #endregion

        public GeoViewer(ScreenManagerComponent screenComponent, IScreenController screenCont,
            ScreenTransitionViewer cont, IDocument documentParent, IScreenController backCont = null, bool bIsModal = false)
        {
            InitializeComponent();

            ScreenComponent = screenComponent;
            screenController = screenCont;
            container = cont;
            DocumentParent = documentParent;
            backController = backCont;

            bMapLiveScreenHide = Properties.Settings.Default.MapLiveScreenHide;

            if (bIsModal)
                btnClose.Visibility = Visibility.Visible;

            Action actionDelay = () =>
            {
                UpdateVisibility();
            };

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    if (Properties.Settings.Default.UseOpenStreetMapProvider)
                    {
                        if (String.IsNullOrEmpty(Properties.Settings.Default.CustomMapDataProviderUrl))
                        {
                            var osdp = new OpenStreetMapDataProvider();
                            osdp.WebRequest += (sender, ev) =>
                            {
                                ev.UserAgent = DocumentParent.Title;
                            };

                            switch (screenCont.GetBingMapKind())
                            {
                                case DocumentManager.ComponentService.BingMapKind.Area: osdp.Kind = OpenStreetMapKind.CycleMap; break;
                                case DocumentManager.ComponentService.BingMapKind.Hybrid: osdp.Kind = OpenStreetMapKind.Basic; break;
                                case DocumentManager.ComponentService.BingMapKind.Road: osdp.Kind = OpenStreetMapKind.Transport; break;
                            }
                            imageTilesLayer.DataProvider = osdp;
                            //imageMiniMapTilesLayer.DataProvider = osdp;
                        }
                        else
                        {
                            var dp = new CustomMapDataProvider(Properties.Settings.Default.CustomMapDataProviderUrl);
                            dp.WebRequest += (sender, ev) =>
                            {
                                ev.UserAgent = DocumentParent.Title;
                            };
                            imageTilesLayer.DataProvider = dp;
                            //imageMiniMapTilesLayer.DataProvider = dp;
                        }
                    }
                    else
                    {
                        bingMapDataProvider.BingKey = Properties.Settings.Default.BingMapKey;
                        switch (screenCont.GetBingMapKind())
                        {
                            case DocumentManager.ComponentService.BingMapKind.Area: bingMapDataProvider.Kind = DevExpress.Xpf.Map.BingMapKind.Area; break;
                            case DocumentManager.ComponentService.BingMapKind.Hybrid: bingMapDataProvider.Kind = DevExpress.Xpf.Map.BingMapKind.Hybrid; break;
                            case DocumentManager.ComponentService.BingMapKind.Road: bingMapDataProvider.Kind = DevExpress.Xpf.Map.BingMapKind.Road; break;
                        }
                    }

                    if (screenCont.GetHasGeoCoordinates())
                    {
                        try
                        {
                            mapControl.CenterPoint = new GeoPoint(screenCont.GetLatitude(), screenCont.GetLongitude());
                        }
                        catch
                        {

                        }
                    }

                    LoadCustomLayers();

					if (ScreenComponent.StringEditor != null)
                    	ScreenComponent.StringEditor.CultureChanged += StringEditor_CultureChanged;

                    var propDesc = DependencyPropertyDescriptor.FromProperty(MapControl.ZoomLevelProperty, typeof(MapControl));
                    if (notifierZoom == null)
                    {
                        notifierZoom = new PropertyChangeNotifier(mapControl, propDesc.Name);
                        notifierZoom.ValueChanged += (ob, ev) =>
                        {
                            bFirstLoad = bForceAll = bBeenForced = false;
                            DelayedInvokeUpdateVisibility(actionDelay);
                        };
                    }
                    propDesc = DependencyPropertyDescriptor.FromProperty(MapControl.CenterPointProperty, typeof(MapControl));
                    if (notifierCenterPoint == null)
                    {
                        notifierCenterPoint = new PropertyChangeNotifier(mapControl, propDesc.Name);
                        notifierCenterPoint.ValueChanged += (ob, ev) =>
                        {
                            DelayedInvokeUpdateVisibility(actionDelay);
                        };
                    }

                    FillGeo();

                    ActivateCurrentMenuBar();
                    ActivateCurrentShortcut();

                    DelayedInvokeUpdateVisibility(actionDelay);
                }

                if (!bLoadedNow)
                {
                    bLoadedNow = true;
                    DelayedInvokeUpdateVisibility(actionDelay);
                    mapControl.Focusable = true;
                    mapControl.Focus();
                }
            };

            GotFocus += (o, e) =>
            {
                if (bCultureChanged && nExpanderExpanded == 0)
                {
                    bCultureChanged = false;
                    FillGeo();

                    DelayedInvokeUpdateVisibility(actionDelay);
                }
            };

            Unloaded += (o, e) =>
            {
                bLoadedNow = false;

                if (bLoaded)
                {
                    SaveLayout(screenController.GetTitle());
                    // bLoaded = false;
                }
            };
        }

        void DelayedInvokeUpdateVisibility(Action action)
        {
            if (delayInvoker == null)
                delayInvoker = new DelayedSingleActionInvoker(action, TimeSpan.FromMilliseconds(1000), false);

            if (delayInvoker != null)
                delayInvoker.BeginInvoke();
        }
        List<MapDataElement> pendingListToCreate;
        void AddPendingListToCreate()
        {
            if (pendingListToCreate == null || pendingListToCreate.Count == 0 || bDisposed)
            {
                progressBar.Visibility = Visibility.Collapsed;
                return;
            }

            var mapDataElement = pendingListToCreate[0];
            pendingListToCreate.RemoveAt(0);

            if (mapDataElement.mapCustomElement == null)
            {
                var point = new MapCustomElement()
                {
                    Location = new GeoPoint(mapDataElement.Latitude,
                                            mapDataElement.Longitude)
                };
                var uri = DocumentParent.MakeAbosoluteUri(mapDataElement.Uri);
                var expander = CreateExpander(currentLocalizationMap, uri,
                    container, DocumentParent, mapDataElement.Color, mapDataElement.TileSize);
                point.Content = expander;
                point.Tag = mapDataElement;
                screenLayer.Items.Add(point);
                mapDataElement.mapCustomElement = point;

                listSourcePoints.Add(point.Location as GeoPoint);

                SubscribeGeoPointTags(screenController, point, uri);
            }

            if (bBeenForced)
                AddPendingListToCreate();
            else
                Dispatcher.BeginInvokeAsynchronouslyInBackground(AddPendingListToCreate);
        }

        bool bBeenForced;
        List<FrameworkElement> pendingZoomVisibilityAnimation = new List<FrameworkElement>();
        Task<List<MapDataElement>> task1;
        bool bForceAll = false;
        async void UpdateVisibility()
        {
            if (mapDataElements == null || mapDataElements.Count == 0 || bBeenForced || !bLoadedNow || bDisposed)
                return;

            if (bForceAll)
                bBeenForced = true;

            progressBar.Visibility = Visibility.Visible;
            double zoomLevel = mapControl.ZoomLevel;
            var geoTopLeft = mapControl.ScreenPointToCoordPoint(mapControl.PointToScreen(new Point(0, 0))) as GeoPoint;
            var geoBottomRight = mapControl.ScreenPointToCoordPoint(mapControl.PointToScreen(new Point(mapControl.ActualWidth, mapControl.ActualHeight))) as GeoPoint;

            pendingListToCreate = null;
            var pendingListToShow = new List<MapDataElement>();
            var pendingListToHide = new List<MapDataElement>();
            bool abortPending = false;
            if (task1 != null)
            {
                abortPending = true;
                await task1;
                abortPending = false;
            }

            task1 = Task.Factory.StartNew(delegate
            {
                var listToCreate = (from c in mapDataElements.AsParallel()
                                           where c.mapCustomElement == null &&
                                          ((c.Latitude < geoTopLeft.Latitude && c.Latitude > geoBottomRight.Latitude &&
                                          c.Longitude > geoTopLeft.Longitude && c.Longitude < geoBottomRight.Longitude || bForceAll) &&
                                          zoomLevel >= c.MinZoomLevel && zoomLevel <= c.MaxZoomLevel)
                                           select c).ToList();

                pendingListToShow = (from c in mapDataElements.AsParallel()
                                     where c.mapCustomElement != null &&
                                    ((c.Latitude < geoTopLeft.Latitude && c.Latitude > geoBottomRight.Latitude &&
                                    c.Longitude > geoTopLeft.Longitude && c.Longitude < geoBottomRight.Longitude || bForceAll) &&
                                    zoomLevel >= c.MinZoomLevel && zoomLevel <= c.MaxZoomLevel)
                                     select c).ToList();

                pendingListToHide = (from c in mapDataElements.AsParallel()
                                        where c.mapCustomElement != null &&
                                    (!(c.Latitude < geoTopLeft.Latitude && c.Latitude > geoBottomRight.Latitude &&
                                    c.Longitude > geoTopLeft.Longitude && c.Longitude < geoBottomRight.Longitude && !bForceAll) ||
                                    zoomLevel < c.MinZoomLevel || zoomLevel > c.MaxZoomLevel)
                                        select c).ToList();

                pendingListToShow.ForEach(element =>
                {
                    if (pendingListToHide.Contains(element))
                        pendingListToHide.Remove(element);
                });

                if (pendingListToShow.Count > 0 || pendingListToHide.Count > 0)
                {
                    while (pendingZoomVisibilityAnimation.Count > 0)
                        System.Threading.Thread.Sleep(100);
                }

                return listToCreate;
            });

            if (abortPending)
                return;

            if (bForceAll)
            {
                task1.Wait();
                ProcessMapPoints(task1, pendingListToShow, pendingListToHide, abortPending);
            }
            else
            {
                var task2 = task1.ContinueWith(ret =>
                {
                    task1 = null;
                    ProcessMapPoints(ret, pendingListToShow, pendingListToHide, abortPending);

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void ProcessMapPoints(Task<List<MapDataElement>> ret, List<MapDataElement> pendingListToShow, List<MapDataElement> pendingListToHide, bool abortPending)
        {
            if (bDisposed)
                return;

            pendingListToCreate = ret.Result;
            if (!abortPending)
            {
                if (pendingListToCreate != null && pendingListToCreate.Count > 0)
                {
                    if (bBeenForced)
                        AddPendingListToCreate();
                    else
                        Dispatcher.BeginInvokeAsynchronouslyInBackground(AddPendingListToCreate);
                }
                else
                    progressBar.Visibility = Visibility.Collapsed;
            }

            bool bRespawn = false;
            using (var cursor = new WaitCursor())
            {
                foreach (var mapDataElement in pendingListToShow)
                {
                    var child = mapDataElement.mapCustomElement.Content as FrameworkElement;
                    if (child != null && child.Visibility == Visibility.Collapsed)
                    {
                        child.Visibility = Visibility.Visible;
                        if (!bMapLiveScreenHide && child is Grid && mapDataElement.TileSize == TileSize.Live)
                        {
                            var grid = child as Grid;
                            if (grid.Children.Count == 0 || !(grid.Children[0] is ScreenViewer))
                            {
                                var viewer = CreateScreenViewer(DocumentParent.MakeAbosoluteUri(mapDataElement.Uri),
                                    container, DocumentParent, grid);
                                listViewers.Add(viewer);
                                if (!bForceAll)
                                {
                                    bRespawn = true;
                                    break;
                                }
                            }
                        }
                        /*
                        if (!pendingZoomVisibilityAnimation.Contains(child))
                            pendingZoomVisibilityAnimation.Add(child);
                        child.Fade(1, 250, new SineEase() { EasingMode = EasingMode.EaseOut },
                            (o, e) =>
                            {
                                if (pendingZoomVisibilityAnimation.Contains(child))
                                    pendingZoomVisibilityAnimation.Remove(child);
                            });
                            */
                    }
                }

                foreach (var mapDataElement in pendingListToHide)
                {
                    var child = mapDataElement.mapCustomElement.Content as FrameworkElement;
                    if (child != null && child.Visibility == Visibility.Visible)
                    {
                        child.Visibility = Visibility.Collapsed;
                        if (!bMapLiveScreenHide && child is Grid)
                        {
                            var grid = child as Grid;
                            if (grid.Children.Count > 0 && grid.Children[0] is ScreenViewer)
                            {
                                var viewer = grid.Children[0] as ScreenViewer;
                                grid.Children.Remove(viewer);
                                if (listViewers.Contains(viewer))
                                    listViewers.Remove(viewer);
                                viewer.Dispose();
                                if (!bForceAll)
                                {
                                    bRespawn = true;
                                    break;
                                }
                            }
                        }
                        /*
                        if (!pendingZoomVisibilityAnimation.Contains(child))
                            pendingZoomVisibilityAnimation.Add(child);
                        child.Fade(0, 250, new SineEase() { EasingMode = EasingMode.EaseOut },
                            (o, e) =>
                            {
                                if (pendingZoomVisibilityAnimation.Contains(child))
                                    pendingZoomVisibilityAnimation.Remove(child);
                            });
                            */
                    }
                }

                if (bRespawn)
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(UpdateVisibility);
                }
            }
        }

        void StringEditor_CultureChanged(object sender, EventArgs e)
        {
#if !WINDOWS_UWP
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
#else
            RunOnUIThread.Run(() =>
#endif
            {
                if (IsFocused)
                    FillGeo();
                else
                    bCultureChanged = true;
            });
        }

        void SubscribePointsInInitialization()
        {
            foreach (MapDataElement mapElement in mapDataElements)
            {
                var point = new MapCustomElement()
                {
                    Location = new GeoPoint(mapElement.Latitude,
                                        mapElement.Longitude)
                };
                var uri = DocumentParent.MakeAbosoluteUri(mapElement.Uri);
                var expander = CreateExpander(currentLocalizationMap, uri,
                    container, DocumentParent, mapElement.Color, mapElement.TileSize);
                point.Content = expander;
                point.Tag = mapElement;
                screenLayer.Items.Add(point);
                mapElement.mapCustomElement = point;

                listSourcePoints.Add(point.Location as GeoPoint);

                SubscribeGeoPointTags(screenController, point, uri);
            }
        }

        SafeObservableCollection<GeoPoint> listSourcePoints = new SafeObservableCollection<GeoPoint>();
        IDictionary<String, String> currentLocalizationMap;
        void FillGeo()
        {
            using (var cursor = new WaitCursor())
            {
                CleanGeo();

                listSourcePoints.Clear();
                //listSourceDataAdapter.DataSource = listSourcePoints;

                currentLocalizationMap = ScreenComponent.StringEditor.GetListStringForCulture(DocumentParent,
                                        ScreenComponent.StringEditor.GetActiveCulture(DocumentParent));

                if (mapDataElements != null)
                    mapDataElements.Clear();

                LoadMapDataElements();

                SubscribePointsInInitialization();

                if (mapDataElements != null && mapDataElements.Count > 0)
                    return;

                mapDataElements = new List<MapDataElement>();
                if (backController != null && backController.GetHasGeoCoordinates())
                {
                    var mapDataElement = new MapDataElement()
                    {
                        Latitude = backController.GetLatitude(),
                        Longitude = backController.GetLongitude(),
                        Uri = DocumentParent.MakeRelativeUri(backController.GetStartupScreen()),
                        MinZoomLevel = backController.GetMapMinZoomLevelVisibility(),
                        MaxZoomLevel = backController.GetMapMinZoomLevelVisibility(),
                        Color = backController.GetIdentityColor(),
                        TileSize = backController.TileSize()
                    };
                    mapDataElements.Add(mapDataElement);
                }

                AddScreenController(screenController, container, DocumentParent, currentLocalizationMap);

                screenController.GetScreenLists().ForEach(uri =>
                {
                    if (screenController.GetHasGeoCoordinates(uri) &&
                        screenController.IsVisible(uri))
                    {
                        var mapDataElement = new MapDataElement()
                        {
                            Latitude = screenController.GetLatitude(uri),
                            Longitude = screenController.GetLongitude(uri),
                            Uri = DocumentParent.MakeRelativeUri(uri),
                            MinZoomLevel = screenController.GetMapMinZoomLevelVisibility(uri),
                            MaxZoomLevel = screenController.GetMapMaxZoomLevelVisibility(uri),
                            Color = screenController.GetIdentityColor(uri),
                            TileSize = screenController.TileSize(uri)
                        };
                        mapDataElements.Add(mapDataElement);
                    }
                });

                SubscribePointsInInitialization();

                /*
                if (backController != null && backController.GetHasGeoCoordinates())
                {
                    var point = new MapCustomElement()
                    {
                        Location = new GeoPoint(backController.GetLatitude(),
                                                backController.GetLongitude())
                    };
                    var expander = CreateExpander(currentLocalizationMap, backController.GetStartupScreen(),
                        container, DocumentParent, backController.GetIdentityColor(), backController.TileSize(), backController.GetMapZoomLevelVisibility());
                    point.Content = expander;
                    point.Tag = backController.GetMapZoomLevelVisibility();
                    screenLayer.Items.Add(point);

                    listSourcePoints.Add(point.Location as GeoPoint);
                }

                AddScreenController(screenController, container, DocumentParent, map);

                screenController.GetScreenLists().ForEach(uri =>
                {
                    if (screenController.GetHasGeoCoordinates(uri) &&
                        screenController.IsVisible(uri))
                    {
                        var point = new MapCustomElement()
                        {
                            Location = new GeoPoint(screenController.GetLatitude(uri),
                                screenController.GetLongitude(uri))
                        };

                        var expander = CreateExpander(map, uri, container, DocumentParent,
                            screenController.GetIdentityColor(uri), screenController.TileSize(uri), screenController.GetMapZoomLevelVisibility(uri));
                        point.Content = expander;
                        point.Tag = screenController.GetMapZoomLevelVisibility(uri);
                        screenLayer.Items.Add(point);

                        SubscribeGeoPointTags(screenController, point, uri);

                        listSourcePoints.Add(point.Location as GeoPoint);
                    }
                });
                */

                Task.Factory.StartNew(delegate
                {
                    SaveMapDataElements();
                });

                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    LoadLayout(screenController.GetTitle());
            }
        }

        void SubscribeGeoPointTags(IScreenController screenController, MapCustomElement point, Uri uri)
        {
            var latTag = screenController.GetLatitudeTag(uri);
            var lonTag = screenController.GetLongitudeTag(uri);
            if (latTag == null || lonTag == null)
                return;

            try
            {
                var tagLat = latTag.FromXml<OPCUAEntityReference>();
                var tagLon = lonTag.FromXml<OPCUAEntityReference>();
                if (tagLat == null || !tagLat.IsValid || tagLon == null || !tagLon.IsValid)
                    return;
                SubscribeTag(tagLat, screenController, point, uri, true);
                SubscribeTag(tagLon, screenController, point, uri, false);
            }
            catch (Exception ex)
            {

            }
        }

        void SetGeoPointInError(MapCustomElement point, String error, bool bIsLat)
        {
            var content = point.Content as FrameworkElement;
            if (content == null)
                return;
            if (!String.IsNullOrEmpty(error))
            {
                var effect = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    BlurRadius = 10,
                    Color = Colors.Red
                };
                content.Effect = effect;
                content.ClipToBounds = false;
                content.ToolTip = error;

                if (bIsLat)
                {
                    if (!listElementLatInError.Contains(point))
                        listElementLatInError.Add(point);
                }
                else
                {
                    if (!listElementLonInError.Contains(point))
                        listElementLonInError.Add(point);
                }
            }
            else
            {
                if (bIsLat)
                {
                    if (!listElementLatInError.Contains(point))
                        return;
                    listElementLatInError.Remove(point);
                }
                else
                {
                    if (!listElementLonInError.Contains(point))
                        return;
                    listElementLonInError.Remove(point);
                }
                content.ToolTip = null;
                content.Effect = null;
            }
        }

        void UpdateValue(Opc.Ua.DataValue value, MapCustomElement point, bool isLat)
        {
            point.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                String error = null;
                if (Opc.Ua.StatusCode.IsGood(value.StatusCode) ||
                    value.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                {
                    try
                    {
                        double v = Convert.ToDouble(value.Value);

                        if (isLat)
                            point.Location = new GeoPoint(v, point.Location.GetX());
                        else
                            point.Location = new GeoPoint(point.Location.GetY(), v);

                        var mapElementFound = (from c in mapDataElements.AsParallel() where c.mapCustomElement == point select c).ToList();
                        if (mapElementFound.Count > 0)
                        {
                            var geoPoint = point.Location as GeoPoint;
                            mapElementFound[0].Latitude = geoPoint.Latitude;
                            mapElementFound[0].Longitude = geoPoint.Longitude;

                            Action actionDelay = () =>
                            {
                                UpdateVisibility();
                            };
                            DelayedInvokeUpdateVisibility(actionDelay);
                        }

                        SetGeoPointInError(point, null, isLat);
                        return;
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                }

                if (String.IsNullOrEmpty(error))
                    error = value.StatusCode.ToString();
                SetGeoPointInError(point, error, isLat);
            });
        }

        void SubscribeTag(OPCUAEntityReference tag, IScreenController screenController, MapCustomElement point,
                          Uri uri, bool isLat = true)
        {
            var observer = new PropertyObserver<OPCUAEntityReference>(tag);
            listPropertyObserverEntityReference.Add(observer);
            observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
            {
                observer.UnregisterHandler(p => p.MonitoredItemViewModel);

                if (n.MonitoredItemViewModel != null && n.MonitoredItemViewModel.DataValue != null)
                {
                    UpdateValue(n.MonitoredItemViewModel.DataValue, point, isLat);

                    if (isLat)
                        mapControllerModel[uri].IsHistorizingLat = n.MonitoredItemViewModel.NodeIdModel.IsHistorizing;
                    else
                        mapControllerModel[uri].IsHistorizingLon = n.MonitoredItemViewModel.NodeIdModel.IsHistorizing;
                }

                var observerMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                listPropertyObserverEntityReference.Remove(observer);
                observer.Dispose();
                listPropertyObserverMonitoredItem.Add(observerMonitoredModel);
                observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                {
                    UpdateValue(m.DataValue, point, isLat);
                });

                if (isLat)
                {
                    if (!mapControllerLatMon.ContainsKey(uri))
                        mapControllerLatMon.Add(uri, n.MonitoredItemViewModel);
                }
                else
                {
                    if (!mapControllerLonMon.ContainsKey(uri))
                        mapControllerLonMon.Add(uri, n.MonitoredItemViewModel);
                }
            });

            if (tag.MonitoredItemViewModel != null &&
                tag.MonitoredItemViewModel.DataValue != null)
                UpdateValue(tag.MonitoredItemViewModel.DataValue, point, isLat);
            else
                SetGeoPointInError(point, Properties.Resources.WaitingForGeoTagConnection, isLat);

            tag.Resolve(screenController.GetTitle(), DocumentParent);
            tag.SetInUse(this, true);
            listReferences.Add(tag);
        }

        bool bNewSearch;
        internal bool ShowPath(Uri controller)
        {
            if (!mapControllerLatMon.ContainsKey(controller) || !mapControllerLonMon.ContainsKey(controller))
                return false;
            if (!mapControllerLatMon[controller].NodeIdModel.IsHistorizing ||
                !mapControllerLonMon[controller].NodeIdModel.IsHistorizing)
                return false;

            bNewSearch = true;

            mapControllerModel[controller].IsBusy = true;

            mapControllerLatMon[controller].HistoryReadModel.ReadType = HistoryReadViewModel.ReadTypeDefinition.Raw;
            mapControllerLatMon[controller].HistoryReadModel.Aggregate = Opc.Ua.BrowseNames.AggregateFunction_Average;
            mapControllerLatMon[controller].HistoryReadModel.UseStartTime = true;
            mapControllerLatMon[controller].HistoryReadModel.StartTime = mapControllerModel[controller].StartDate;
            mapControllerLatMon[controller].HistoryReadModel.UseEndTime = true;
            mapControllerLatMon[controller].HistoryReadModel.EndTime = mapControllerModel[controller].EndDate;
            mapControllerLatMon[controller].HistoryReadModel.UseMaxReturnValues = false;

            lock (listPropertyObserverHistoryItem)
            {
                var observerHistoryModelLat = new PropertyObserver<HistoryReadViewModel>(mapControllerLatMon[controller].HistoryReadModel);
                listPropertyObserverHistoryItem.Add(observerHistoryModelLat);
                observerHistoryModelLat.RegisterHandler(m => m.Values, m =>
                {
                    lock (listPropertyObserverHistoryItem)
                    {
                        listPropertyObserverHistoryItem.Remove(observerHistoryModelLat);
                        observerHistoryModelLat.Dispose();
                    }

                    Dispatcher.BeginInvokeIfRequired(() =>
                    {
                        FillPathLine(controller);
                    });
                });
            }
            mapControllerLatMon[controller].HistoryReadModel.GetResults.Execute(null);

            mapControllerLonMon[controller].HistoryReadModel.ReadType = HistoryReadViewModel.ReadTypeDefinition.Raw;
            mapControllerLonMon[controller].HistoryReadModel.Aggregate = Opc.Ua.BrowseNames.AggregateFunction_Average;
            mapControllerLonMon[controller].HistoryReadModel.UseStartTime = true;
            mapControllerLonMon[controller].HistoryReadModel.StartTime = mapControllerModel[controller].StartDate;
            mapControllerLonMon[controller].HistoryReadModel.UseEndTime = true;
            mapControllerLonMon[controller].HistoryReadModel.EndTime = mapControllerModel[controller].EndDate;
            mapControllerLonMon[controller].HistoryReadModel.UseMaxReturnValues = false;

            lock (listPropertyObserverHistoryItem)
            {
                var observerHistoryModelLon = new PropertyObserver<HistoryReadViewModel>(mapControllerLonMon[controller].HistoryReadModel);
                listPropertyObserverHistoryItem.Add(observerHistoryModelLon);
                observerHistoryModelLon.RegisterHandler(m => m.Values, m =>
                {
                    lock (listPropertyObserverHistoryItem)
                    {
                        listPropertyObserverHistoryItem.Remove(observerHistoryModelLon);
                        observerHistoryModelLon.Dispose();
                    }

                    Dispatcher.BeginInvokeIfRequired(() =>
                    {
                        FillPathLine(controller);
                    });
                });
            }
            mapControllerLonMon[controller].HistoryReadModel.GetResults.Execute(null);

            return true;
        }

        internal void RemovePathLine(Uri controller)
        {
            if (!mapControllerToPath.ContainsKey(controller))
                return;
            mapControllerToPath[controller].ForEach(item =>
            {
                if (pathLayer.Items.Contains(item))
                    pathLayer.Items.Remove(item);
            });
            mapControllerToPath.Remove(controller);
        }

        void FillPathLine(Uri controller)
        {
            if (bNewSearch)
            {
                bNewSearch = false;
                return;
            }
            mapControllerModel[controller].IsBusy = false;

            RemovePathLine(controller);
            using (var cursor = new WaitCursor())
            {
                /*
                var reachTime = DateTime.Now + TimeSpan.FromSeconds(30);
                bool bTimeout = true;
                while (DateTime.Now < reachTime)
                {
                    if (!mapControllerLatMon[controller].HistoryReadModel.GetResults.CanExecute(null) ||
                        !mapControllerLonMon[controller].HistoryReadModel.GetResults.CanExecute(null))
                    {
                        System.Threading.Thread.Sleep(200);
                        continue;
                    }
                    bTimeout = false;
                    break;
                }

                if (bTimeout)
                {
                    ScreenComponent.UIInterface.ShowError(Properties.Resources.GeoHistoryTimeout);
                    return;
                }
                */

                if (mapControllerLatMon[controller].HistoryReadModel.Values == null || mapControllerLatMon[controller].HistoryReadModel.Values.Count == 0 ||
                    mapControllerLonMon[controller].HistoryReadModel.Values == null || mapControllerLonMon[controller].HistoryReadModel.Values.Count == 0)
                {
                    ScreenComponent.UIInterface.ShowError(Properties.Resources.GeoHistoryNoValues);
                    return;
                }

                mapControllerLatMon[controller].HistoryReadModel.Values.Add(mapControllerLatMon[controller].DataValue);
                mapControllerLonMon[controller].HistoryReadModel.Values.Add(mapControllerLonMon[controller].DataValue);

                bool bFirstIsLat = false;
                var DataValueCollection1 = new Opc.Ua.DataValueCollection();
                var DataValueCollection2 = new Opc.Ua.DataValueCollection();
                if (mapControllerLatMon[controller].HistoryReadModel.Values.Count >=
                    mapControllerLonMon[controller].HistoryReadModel.Values.Count)
                {
                    bFirstIsLat = true;
                    DataValueCollection1.AddRange(mapControllerLatMon[controller].HistoryReadModel.Values);
                    DataValueCollection2.AddRange(mapControllerLonMon[controller].HistoryReadModel.Values);
                }
                else
                {
                    bFirstIsLat = true;
                    DataValueCollection2.AddRange(mapControllerLatMon[controller].HistoryReadModel.Values);
                    DataValueCollection1.AddRange(mapControllerLonMon[controller].HistoryReadModel.Values);
                }

                var geopointCollection = new CoordPointCollection();
                var mapTimestamp = new Dictionary<CoordPoint, DateTime>();
                foreach (var latValue in DataValueCollection1)
                {
                    if (!Opc.Ua.StatusCode.IsGood(latValue.StatusCode))
                        continue;

                    var lotValueList = (from c in DataValueCollection2
                                        where Opc.Ua.StatusCode.IsGood(c.StatusCode) &&
                                        c.SourceTimestamp.ToString() == latValue.SourceTimestamp.ToString()
                                        select c).ToList();

                    if (lotValueList.Count <= 0)
                    {
                        lotValueList = (from c in DataValueCollection2
                                        where Opc.Ua.StatusCode.IsGood(c.StatusCode) && c.SourceTimestamp < latValue.SourceTimestamp
                                        orderby c.SourceTimestamp ascending
                                        select c).ToList();
                        if (lotValueList.Count <= 0)
                        {
                            lotValueList.Add(DataValueCollection2[0]);
                        }
                    }

                    try
                    {
                        double lat = 0, lon = 0;
                        if (bFirstIsLat)
                        {
                            lat = Convert.ToDouble(latValue.Value);
                            lon = Convert.ToDouble(lotValueList[0].Value);
                        }
                        else
                        {
                            lon = Convert.ToDouble(latValue.Value);
                            lat = Convert.ToDouble(lotValueList[0].Value);
                        }
                        var geopoint = new GeoPoint(lat, lon);
                        geopointCollection.Add(geopoint);
                        mapTimestamp.Add(geopoint, latValue.SourceTimestamp);
                    }
                    catch (Exception)
                    {
                    }
                }

                var listRemaining = (from c in DataValueCollection2
                                     where Opc.Ua.StatusCode.IsGood(c.StatusCode) &&
                                     c.SourceTimestamp > DataValueCollection1.Last().SourceTimestamp
                                     orderby c.SourceTimestamp ascending
                                     select c).ToList();
                foreach (var latValue in listRemaining)
                {
                    if (!Opc.Ua.StatusCode.IsGood(latValue.StatusCode))
                        continue;

                    var lotValueList = new Opc.Ua.DataValueCollection();
                    lotValueList.Add(DataValueCollection1.Last());

                    try
                    {
                        double lat = 0, lon = 0;
                        if (bFirstIsLat)
                        {
                            lon = Convert.ToDouble(latValue.Value);
                            lat = Convert.ToDouble(lotValueList[0].Value);
                        }
                        else
                        {
                            lat = Convert.ToDouble(latValue.Value);
                            lon = Convert.ToDouble(lotValueList[0].Value);
                        }
                        var geopoint = new GeoPoint(lat, lon);
                        geopointCollection.Add(geopoint);
                        mapTimestamp.Add(geopoint, latValue.SourceTimestamp);
                    }
                    catch (Exception)
                    {
                    }
                }


                if (mapControllerToPath.ContainsKey(controller))
                    mapControllerToPath.Remove(controller);
                var mapItems = new List<MapItem>();
                mapControllerToPath.Add(controller, mapItems);

                var polyline = new MapPolyline()
                {
                    Points = geopointCollection,
                    Fill = new SolidColorBrush(Colors.Transparent),
                    Stroke = new SolidColorBrush(Colors.Red),
                    StrokeStyle = new StrokeStyle() { Thickness = 5 }
                };
                pathLayer.Items.Add(polyline);
                mapItems.Add(polyline);
                var template = Resources["nodeTemplate"] as DataTemplate;
                foreach (var point in geopointCollection)
                {
                    var contentTemplate = template.LoadContent() as FrameworkElement;
                    var item = new MapCustomElement() { Location = point, Content = contentTemplate };
                    contentTemplate.ToolTip = String.Format(Properties.Resources.GeoPointTooltip,
                        mapTimestamp[point], point.GetX(), point.GetY());
                    pathLayer.Items.Add(item);
                    mapItems.Add(item);
                }
            }
        }

        private void AddScreenController(IScreenController screenController, ScreenTransitionViewer container, IDocument DocumentParent, IDictionary<String, String> map)
        {
            screenController.GetScreenControllers().ForEach(controller =>
            {
                if (controller.GetHasGeoCoordinates() && controller.IsVisible())
                {
                    var mapDataElement = new MapDataElement()
                    {
                        Latitude = controller.GetLatitude(),
                        Longitude = controller.GetLongitude(),
                        Uri = DocumentParent.MakeRelativeUri(controller.GetStartupScreen()),
                        MinZoomLevel = controller.GetMapMinZoomLevelVisibility(),
                        MaxZoomLevel = controller.GetMapMaxZoomLevelVisibility(),
                        Color = controller.GetIdentityColor(),
                        TileSize = controller.TileSize()
                    };
                    mapDataElements.Add(mapDataElement);
                    /*
                    var point = new MapCustomElement()
                    {
                        Location = new GeoPoint(controller.GetLatitude(),
                                                controller.GetLongitude())
                    };
                    var expander = CreateExpander(map, controller.GetStartupScreen(),
                        container, DocumentParent, controller.GetIdentityColor(), controller.TileSize(), controller.GetMapZoomLevelVisibility());
                    point.Content = expander;
                    point.Tag = controller.GetMapZoomLevelVisibility();
                    screenLayer.Items.Add(point);

                    listSourcePoints.Add(point.Location as GeoPoint);
                    */
                }

                controller.GetScreenLists().ForEach(uri =>
                {
                    if (screenController.GetHasGeoCoordinates(uri) &&
                        screenController.IsVisible(uri))
                    {
                        var mapDataElement = new MapDataElement()
                        {
                            Latitude = screenController.GetLatitude(uri),
                            Longitude = screenController.GetLongitude(uri),
                            Uri = DocumentParent.MakeRelativeUri(uri),
                            MinZoomLevel = screenController.GetMapMinZoomLevelVisibility(uri),
                            MaxZoomLevel = screenController.GetMapMaxZoomLevelVisibility(uri),
                            Color = screenController.GetIdentityColor(uri),
                            TileSize = screenController.TileSize(uri)
                        };
                        mapDataElements.Add(mapDataElement);

                        /*
                        var point = new MapCustomElement()
                        {
                            Location = new GeoPoint(screenController.GetLatitude(uri),
                                screenController.GetLongitude(uri))
                        };

                        var expander = CreateExpander(map, uri, container, DocumentParent,
                            screenController.GetIdentityColor(uri), screenController.TileSize(uri), screenController.GetMapZoomLevelVisibility(uri));
                        point.Content = expander;
                        point.Tag = screenController.GetMapZoomLevelVisibility(uri);
                        screenLayer.Items.Add(point);

                        listSourcePoints.Add(point.Location as GeoPoint);

                        SubscribeGeoPointTags(controller, point, uri);
                        */
                    }
                });

                AddScreenController(controller, container, DocumentParent, map);
            });
        }

        readonly List<ScreenViewer> listViewers = new List<ScreenViewer>();
        FrameworkElement CreateExpander(IDictionary<string, string> map, Uri uri,
            ScreenTransitionViewer container, IDocument DocumentParent, Color color, TileSize size)
        {
            var title = System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
            var model = new GeoViewerItemModel(uri, this);
            mapControllerModel.Add(uri, model);

            if (size == TileSize.ExtraSmall)
            {
                var button = new Button()
                {
                    Content = map != null && map.ContainsKey(title) ? map[title] : title,
                    Background = new SolidColorBrush(color)
                };
                button.Click += (ob, ev) =>
                {
                    ScreenComponent.Execute(uri, DocumentParent, ExecutionMode.Normal, null);
                    // container.OpenOrActivate(uri);
                };
                var effectBtn = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    BlurRadius = 50,
                    Color = color
                };
                button.Effect = effectBtn;
                button.ClipToBounds = false;

                button.Blink(500, 0.5, 1, new BackEase() { EasingMode = EasingMode.EaseOut });
                button.DataContext = model;
                return button;
            }
            var expander = new Expander()
            {
                Header = map != null && map.ContainsKey(title) ? map[title] : title,
                Background = new SolidColorBrush(color)
            };
            switch (size)
            {
                //case TileSize.ExtraSmall:
                //    expander.FontSize = 8;
                //    expander.IsExpanded = false;
                //    break;
                case TileSize.Small:
                    expander.FontSize = 24;
                    expander.IsExpanded = false;
                    expander.DataContext = model;
                    break;
                case TileSize.Large:
                    expander.FontSize = 36;
                    expander.IsExpanded = false;
                    expander.DataContext = model;
                    break;
                case TileSize.ExtraLarge:
                    expander.FontSize = 48;
                    expander.IsExpanded = false;
                    expander.DataContext = model;
                    break;
                case TileSize.Live:
                    {
                        //expander.FontSize = 12;
                        //expander.IsExpanded = true;
                        //Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                        //    {
                        //        FillExpander(uri, container, DocumentParent, expander);
                        //    });
                        //break;
                        var template = Resources["barTemplate"] as DataTemplate;
                        var grid = template.LoadContent() as Grid;
                        grid.DataContext = model;
                        var viewer = CreateScreenViewer(uri, container, DocumentParent, grid);
                        listViewers.Add(viewer);
                        return grid;
                    }
            }

            var effect = new DropShadowEffect
            {
                ShadowDepth = 10,
                Color = Colors.Black
            };
            expander.Effect = effect;
            expander.ClipToBounds = false;

            expander.Collapsed += (ob, ev) =>
            {
                --nExpanderExpanded;
            };
            expander.Expanded += (ob, ev) =>
            {
                ++nExpanderExpanded;

                FillExpander(uri, container, DocumentParent, expander);
                var mapPoint = (from c in screenLayer.Items.OfType<MapCustomElement>() where c.Content == expander select c).ToList();
                if (mapPoint.Count > 0)
                {
                    screenLayer.Items.Remove(mapPoint[0]);
                    screenLayer.Items.Add(mapPoint[0]);
                }
            };
            expander.TouchDown += (ob, ev) =>
            {
                expander.IsExpanded = !expander.IsExpanded;
            };

            return expander;
        }

        private ScreenViewer CreateScreenViewer(Uri uri, ScreenTransitionViewer container, IDocument DocumentParent, Grid grid)
        {
            var viewer = new ScreenViewer(ScreenComponent, uri, DocumentParent, container,
                isModal: true, bIsLayout: true);
            grid.Children.Insert(0, viewer);

            viewer.Loaded += (o, e) =>
            {
                viewer.Width = viewer.MainSurface.Width;
                viewer.Height = viewer.MainSurface.Height;
                viewer.Background = null;

                grid.RenderTransform = new TranslateTransform(-(viewer.Width / 2), -(viewer.Height / 2));
            };
            return viewer;
        }

        void FillExpander(Uri uri, ScreenTransitionViewer container, IDocument DocumentParent, Expander expander)
        {
            if (expander.Content == null)
            {
                using (var cursor = new WaitCursor())
                {
                    var viewer = new ScreenViewer(ScreenComponent, uri, DocumentParent, container,
                        isModal: true, bIsLayout: true);
                    viewer.Loaded += (o, e) =>
                    {
                        viewer.Width = viewer.MainSurface.Width;
                        viewer.Height = viewer.MainSurface.Height;
                    };

                    expander.Content = viewer;
                    listViewers.Add(viewer);
                }
            }
        }

        void LoadCustomLayers()
        {
            var file = String.Format("{0}customlayers.xml", DocumentParent.GetSpecialFolder(SpecialFolders.RuntimeData));
            var uri = new Uri(file, UriKind.RelativeOrAbsolute);

            var index = mapControl.Layers.IndexOf(imageTilesLayer);

            try
            {
                using (var stream = new FileStream(uri.GetPathString(), FileMode.Open))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (var reader = XmlReader.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(List<CustomLayer>));
                        var list = serializer.ReadObject(reader) as IList<CustomLayer>;
                        foreach(var customLayer in list)
                        {
                            var imageLayer = new ImageTilesLayer();
                            if (!Double.IsNaN(customLayer.MaxZoomLevel))
                                imageLayer.MaxZoomLevel = customLayer.MaxZoomLevel;
                            if (!Double.IsNaN(customLayer.MinZoomLevel))
                                imageLayer.MinZoomLevel = customLayer.MinZoomLevel;

                            MapDataProviderBase dataProvider = new BingMapDataProvider()
                            {
                                Kind = customLayer.BingMapKind,
                                BingKey = Properties.Settings.Default.BingMapKey
                            };
                            if (customLayer.UseOpenStreetProvider)
                            {
                                if (String.IsNullOrEmpty(Properties.Settings.Default.CustomMapDataProviderUrl))
                                    dataProvider = new OpenStreetMapDataProvider() { Kind = customLayer.OpenStreetMapKind };
                                else
                                    dataProvider = new CustomMapDataProvider(Properties.Settings.Default.CustomMapDataProviderUrl);
                            }

                            imageLayer.DataProvider = dataProvider;
                            //imageMiniMapTilesLayer.DataProvider = dataProvider;
                            if (customLayer.Bounds != null)
                            {
                                imageLayer.Bounds = new CoordPointCollection();
                                foreach (var point in customLayer.Bounds)
                                    imageLayer.Bounds.Add(point);
                            }

                            mapControl.Layers.Insert(++index, imageLayer);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        List<MapDataElement> mapDataElements = new List<MapDataElement>();
        static readonly String mapDataElementFileName = "{0}MapDataElements.xml";
        void LoadMapDataElements()
        {
            var file = String.Format(mapDataElementFileName, DocumentParent.GetSpecialFolder(SpecialFolders.RuntimeData));
            var uri = new Uri(file, UriKind.RelativeOrAbsolute);

            try
            {
                using (var stream = new FileStream(uri.GetPathString(), FileMode.Open))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (var reader = XmlReader.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(List<MapDataElement>));
                        mapDataElements = serializer.ReadObject(reader) as List<MapDataElement>;
                    }
                }
            }
            catch (Exception ex)
            {
                mapDataElements.Clear();
            }
        }

        bool SaveMapDataElements()
        {
            var file = String.Format(mapDataElementFileName, DocumentParent.GetSpecialFolder(SpecialFolders.RuntimeData));
            var uri = new Uri(file, UriKind.RelativeOrAbsolute);

            CleanMapDataElements(DocumentParent);

            bool bRet = false;
            try
            {
                using (var ostrm = File.Open(uri.GetPathString(), FileMode.Create, FileAccess.ReadWrite))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        Indent = true,
                        CloseOutput = true
                    };

                    using (XmlWriter writer = XmlDictionaryWriter.Create(ostrm, settings))
                    {
                        try
                        {
                            var serializer = new DataContractSerializer(typeof(List<MapDataElement>));
                            serializer.WriteObject(writer, mapDataElements);
                            bRet = true;
                        }
                        finally
                        {
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return bRet;
        }

        internal static void CleanMapDataElements(IDocument parent)
        {
            var file = String.Format(mapDataElementFileName, parent.GetSpecialFolder(SpecialFolders.RuntimeData));
            var uri = new Uri(file, UriKind.RelativeOrAbsolute);
            try
            {
                File.Delete(uri.GetPathString());
            }
            catch
            { }
        }
        #region Isolated Storage

        static String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.TileViewer.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
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
            /*
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.Create, isoStorage))
                {
                    dynamic store = new ElasticObject("Settings");
                    store.ZoomLevel = mapControl.ZoomLevel;
                    store.Latitude = mapControl.CenterPoint.GetX();
                    store.Longitude = mapControl.CenterPoint.GetY();
                    var el = store > FormatType.Xml;

                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(el.ToString());
                    }
                }
            }
            catch (Exception ex)
            {

            }
            */
        }

        void LoadLayout(String title)
        {
            /*
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var text = reader.ReadToEnd();
                        var store = XElement.Parse(text).ToElastic();
                        mapControl.ZoomLevel = Convert.ToInt32(store.ZoomLevel);
                        var latitude = Convert.ToDouble(store.Latitude, CultureInfo.InvariantCulture);
                        var longitude = Convert.ToDouble(store.Longitude, CultureInfo.InvariantCulture);
                        mapControl.CenterPoint = new GeoPoint(latitude, longitude);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            */
        }

        #endregion

        #region Shortcuts
#if !WINDOWS_UWP
        private void GeoViewerControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ScreenComponent.ShortcutEditor == null)
                return;
            e.Handled = ScreenComponent.ShortcutEditor.ExecuteGestureOnDown(DocumentParent, e);
        }

        private void GeoViewerControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (ScreenComponent.ShortcutEditor == null)
                return;
            e.Handled = ScreenComponent.ShortcutEditor.ExecuteGestureOnUp(DocumentParent, e);
        }

        void ActivateCurrentShortcut()
        {
            if (ScreenComponent.ShortcutEditor == null)
                return;

            var path = String.Format("{0}{1}", 
                Properties.Settings.Default.DefaultGeoPageMenuAndShortcutName ?? String.Empty, 
                (ScreenComponent.ShortcutEditor as IDocumentManager).FileType);
            ScreenComponent.ShortcutEditor.Activate(DocumentParent, new Uri(path, UriKind.RelativeOrAbsolute), 
                screenController.GetTitle(), false);
        }

        void DestroyOwnedShortcut()
        {
            if (ScreenComponent.ShortcutEditor == null)
                return;

            ScreenComponent.ShortcutEditor.Terminate(DocumentParent);
        }

        void ActivateCurrentMenuBar()
        {
            if (ScreenComponent.MenuEditor == null)
                return;

            var path = String.Format("{0}{1}", 
                Properties.Settings.Default.DefaultGeoPageMenuAndShortcutName ?? String.Empty, 
                (ScreenComponent.MenuEditor as IDocumentManager).FileType);
            var menu = ScreenComponent.MenuEditor.GetMenu(DocumentParent, new Uri(path, UriKind.RelativeOrAbsolute),
                screenController.GetTitle(), false, false, false);

            if (menu != null)
            {
                if (gridMenu.Children.Count > 1)
                {
                    var menuold = gridMenu.Children[gridMenu.Children.Count - 1] as MenuBase;
                    gridMenu.Children.RemoveAt(gridMenu.Children.Count - 1);
                    if (menuold != null)
                        ScreenComponent.MenuEditor.TerminateMenu(menuold);
                }
                gridMenu.Children.Add(menu);
            }
        }

        void DestroyOwnedMenus()
        {
            if (ScreenComponent.MenuEditor == null)
                return;

            if (gridMenu.Children.Count > 1)
                gridMenu.Children.RemoveAt(gridMenu.Children.Count - 1);
        }
#endif
        #endregion

        MapCustomElement currentItem;
        bool bFirstLoad;
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            bool bLoop = false;
            using (var cursor = new WaitCursor())
            {
                if (!bFirstLoad)
                {
                    bFirstLoad = true;
                    bForceAll = true;
                    try
                    {
                        UpdateVisibility();
                    }
                    finally
                    {
                        bForceAll = false;
                    }
                }

                while (true)
                {
                    if (screenLayer.Items.Count == 0)
                        return;

                    if (currentItem != null)
                    {
                        var index = screenLayer.Items.IndexOf(currentItem) + 1;
                        if (index >= screenLayer.Items.Count)
                        {
                            if (bLoop)
                                return;
                            bLoop = true;
                            index = 0;
                        }

                        currentItem = screenLayer.Items[index] as MapCustomElement;
                    }
                    else
                        currentItem = screenLayer.Items[0] as MapCustomElement;

                    var item = currentItem.Content as FrameworkElement;
                    if (item != null && item.Visibility == Visibility.Visible)
                        break;
                }

                mapControl.CenterPoint = currentItem.Location;
            }
        }

        void CleanGeo()
        {
            listViewers.ForEach(viewer => viewer.Dispose());
            listViewers.Clear();
            listElementLatInError.Clear();
            listElementLonInError.Clear();
            mapControllerLatMon.Clear();
            mapControllerLonMon.Clear();
            mapControllerToPath.Clear();
            mapControllerModel.Clear();
            screenLayer.Items.Clear();
            pathLayer.Items.Clear();

            listReferences.ToList().ForEach(item =>
            {
                item.SetInUse(this, false);
            });
            listReferences.Clear();

            listPropertyObserverMonitoredItem.ToList().ForEach(item =>
            {
                if (item != null)
                    item.Dispose();
            });
            listPropertyObserverMonitoredItem.Clear();

            listPropertyObserverHistoryItem.ToList().ForEach(item =>
            {
                if (item != null)
                    item.Dispose();
            });
            listPropertyObserverHistoryItem.Clear();

            listPropertyObserverEntityReference.ToList().ForEach(item =>
            {
                item.Dispose();
            });
            listPropertyObserverEntityReference.Clear();

            if (bLoaded)
            {
                SaveLayout(screenController.GetTitle());
            }
        }

        internal void MapScrollingZooming(bool bEnable)
        {
            mapControl.EnableScrolling = mapControl.EnableZooming = bEnable;
        }

        internal void SetOptions(bool showMiniMap, bool showNextButton)
        {
            btnNext.Visibility = showNextButton ? Visibility.Visible : Visibility.Collapsed;
            miniMap.Visibility = showMiniMap ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void ZoomTo(Rect rect, Point3D openingParameters)
        {
            try
            {
                if (openingParameters != null && openingParameters.X != 0 && openingParameters.Y != 0 && openingParameters.Z >= 1)
                {
                    ZoomToSelectedPoint(openingParameters);
                } 
                else
                {
                    ZoomToSelectedRegion(rect);
                }
            }
            catch { }
        }

        private void ZoomToSelectedPoint(Point3D openingParameters)
        {
            mapControl.CenterPoint = new GeoPoint(openingParameters.Y, openingParameters.X);
            mapControl.ZoomLevel = openingParameters.Z;
        }

        private void ZoomToSelectedRegion(Rect rect)
        {
            if (rect.IsEmpty || rect.Width == 0 || rect.Height == 0)
                return;

            var p1 = new GeoPoint(rect.TopLeft.X, rect.TopLeft.Y);
            var p2 = new GeoPoint(rect.BottomRight.X, rect.BottomRight.Y);
            mapControl.EnableScrolling = mapControl.EnableZooming = true;
            mapControl.ZoomToRegion(p1, p2);
        }

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            CleanGeo();

            if (delayInvoker != null)
            {
                delayInvoker.Terminate();
                delayInvoker = null;
            }

            if (ScreenComponent.StringEditor != null)
            	ScreenComponent.StringEditor.CultureChanged -= StringEditor_CultureChanged;

#if !WINDOWS_UWP
            DestroyOwnedShortcut();
            DestroyOwnedMenus();
#endif

            if (notifierZoom != null)
            {
                notifierZoom.Dispose();
                notifierZoom = null;
            }

            if (notifierCenterPoint != null)
            {
                notifierCenterPoint.Dispose();
                notifierCenterPoint = null;
            }

            if (mapDataElements != null)
                mapDataElements.Clear();
        }

        #region IEntityReference Members

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion
    }

    [DataContract(Name = "CustomLayer", Namespace = Namespaces.UriProgea)]
    public class CustomLayer
    {
        [DataMember]
        public GeoPointCollection Bounds;
        [DataMember]
        public double MaxZoomLevel;
        [DataMember]
        public double MinZoomLevel;
        [DataMember]
        public bool UseOpenStreetProvider;
        [DataMember]
        public OpenStreetMapKind OpenStreetMapKind;
        [DataMember]
        public DevExpress.Xpf.Map.BingMapKind BingMapKind;
    }

    [DataContract(Name = "MapDataElement", Namespace = Namespaces.UriProgea)]
    public class MapDataElement
    {
        [DataMember]
        public Uri Uri;
        [DataMember]
        public double MaxZoomLevel;
        [DataMember]
        public double MinZoomLevel;
        [DataMember]
        public bool ZoomContent;
        [DataMember]
        public double Latitude;
        [DataMember]
        public double Longitude;
        [DataMember]
        public Color Color;
        [DataMember]
        public TileSize TileSize;

        public MapCustomElement mapCustomElement;
    }
}
