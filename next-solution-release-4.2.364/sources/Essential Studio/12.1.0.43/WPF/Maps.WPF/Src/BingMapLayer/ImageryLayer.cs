#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Collections.ObjectModel;
    using Syncfusion.Maps.Imagery.Common;
    using Syncfusion.Maps.Imagery;
    using System.Windows.Media.Imaging;
    using System.Linq;
    using System.Threading;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    /// <summary>
    ///  ImageryLayer is Layer which contains Imagery Maps
    /// </summary>
    public class ImageryLayer : MapLayer
    {
        // API Method
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);


        #region Internal Fields

        internal ImageryPanel imageryPanel;
        internal Grid bingMapGrid;
        internal bool enableInvalidate = false;
        internal double xcount;
        internal double ycount;
        internal Point temposition = new Point();
        internal Point previousPosition = new Point();
        internal bool isMouseDown;
        internal double diffX;
        internal double diffY;
        internal TranslateTransform bingmapPanTransform;
        internal MapUriRequest mapUriRequest = new MapUriRequest();
        internal MapUriOptions mapUriOptions = new MapUriOptions();
        internal System.ServiceModel.BasicHttpBinding httpBinding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None) { MaxBufferSize = 2147483647, MaxReceivedMessageSize = 2147483647 };
        internal ImageryServiceClient imageryService;
        internal Point panPoint = new Point();
        internal MapControl mapControl;
        internal Point zoomPointPosition;
        internal object zoomPointerObject;
        internal bool isZoomLevelChanged=false;

        #endregion

        #region Properties

        #region Tiles Property

        /// <summary>
        /// Gets or sets the Tile for the Bing Map Layer.
        /// </summary>
        /// <value>Collection of Tiles </value>
        /// <remarks></remarks>
        public ObservableCollection<Tile> Tiles
        {
            get { return (ObservableCollection<Tile>)GetValue(TilesProperty); }
            internal set { SetValue(TilesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tiles.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Used to Identify the Tiles Property of the Bing Map Layer.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty TilesProperty =
            DependencyProperty.Register("Tiles", typeof(ObservableCollection<Tile>), typeof(ImageryLayer), new PropertyMetadata(new ObservableCollection<Tile>()));

        #endregion

        #region BingMapKey Property

        /// <summary>
        /// Gets or sets BingMapKey for Imagery Layer.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string BingMapKey
        {
            get { return (string)GetValue(BingMapKeyProperty); }
            set { SetValue(BingMapKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BingMapKey.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the BingMapKey property of the Imagery Layer.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty BingMapKeyProperty =
            DependencyProperty.Register("BingMapKey", typeof(string), typeof(ImageryLayer), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnBingMapKeyChanged)));

        private static void OnBingMapKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageryLayer ilayer = d as ImageryLayer;
            if (e.NewValue != null)
            {
                // Set credentials using a valid Bing Maps key
                ilayer.mapUriRequest.Credentials = new Credentials();
                ilayer.mapUriRequest.Credentials.ApplicationId = e.NewValue.ToString();
            }
        }

        #endregion

        #region MapStyle Property



        /// <summary>
        /// Gets or sets MapStyle of the ImageryLayer.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public MapStyle MapStyle
        {
            get { return (MapStyle)GetValue(MapStyleProperty); }
            set { SetValue(MapStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapStyle.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the MapStyle property of the BingMaps.
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty MapStyleProperty =
            DependencyProperty.Register("MapStyle", typeof(MapStyle), typeof(ImageryLayer), new PropertyMetadata(MapStyle.Aerial, new PropertyChangedCallback(OnMapStyleChanged)));

        private static void OnMapStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageryLayer ilayer = d as ImageryLayer;
            if (e.NewValue != null)
            {
                ilayer.Tiles.Clear();
                ilayer.LoadBingMap();
            }
        }


        #endregion

        #region DefaultCursor



        /// <summary>
        /// Gets or sets DefaultCursor for the Imagery layer.
        /// </summary>
        /// <value>
        /// Cursor
        /// </value>
        public Cursor DefaultCursor
        {
            get { return (Cursor)GetValue(DefaultCursorProperty); }
            set { SetValue(DefaultCursorProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for DefaultCursor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DefaultCursorProperty =
            DependencyProperty.Register("DefaultCursor", typeof(Cursor), typeof(ImageryLayer), new PropertyMetadata(Cursors.Arrow));



        #endregion

        #region PanCusrsor



        /// <summary>
        /// Gets or sets Panning cursor for the Imagery layer.
        /// </summary>
        /// <value>
        /// Cursor
        /// </value>
        public Cursor PanCursor
        {
            get { return (Cursor)GetValue(PanCursorProperty); }
            set { SetValue(PanCursorProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PanCursor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PanCursorProperty =
            DependencyProperty.Register("PanCursor", typeof(Cursor), typeof(ImageryLayer), new PropertyMetadata(Cursors.Hand));



        #endregion


        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:BingMapApp.BingMap">BingMap</see> class. 
        /// </summary>
        public ImageryLayer()
        {
            this.DefaultStyleKey = typeof(ImageryLayer);
            this.imageryPanel = new ImageryPanel();
            this.bingmapPanTransform = new TranslateTransform { X = 0, Y = 0 };
            imageryService = new ImageryServiceClient(httpBinding, new System.ServiceModel.EndpointAddress("http://dev.virtualearth.net/webservices/v1/imageryservice/imageryservice.svc"));
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mapControl = MapControl.FindParent<MapControl>(this);
            this.Tiles.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Tiles_CollectionChanged);
            this.bingMapGrid = this.GetTemplateChild("PART_BingMapGrid") as Grid;
            this.bingMapGrid.Children.Add(this.imageryPanel);
            this.bingMapGrid.SizeChanged += new SizeChangedEventHandler(bingMapGrid_SizeChanged);
            if (this.BingMapKey != string.Empty)
            {
                this.LoadBingMap();
            }
            
            if (mapControl !=null &&   mapControl.navigationControl != null)
            {
                mapControl.navigationControl.MinSliderValue = (mapControl.navigationControl.MinSliderValue < 1) ? 1 : mapControl.navigationControl.MinSliderValue;                 
            }
        }

        void bingMapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.BingMapKey != string.Empty)
            {
                this.LoadBingMap();
            }
        }

        void Tiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.imageryPanel != null)
            {
                if (e.OldItems != null)
                {

                }
#if WPF
                Dispatcher.BeginInvoke(
                    (Action)delegate
                    {
#endif
                        if (e.NewItems != null)
                        {
                            this.enableInvalidate = false;
                            foreach (object tile in e.NewItems)
                            {
                                {
                                    (tile as Tile).SetTileImage((int)ZoomLevel, this);
                                    this.imageryPanel.Children.Add(tile as UIElement);
                                }
                            }
                            this.enableInvalidate = true;
                        }
#if WPF

                    });
#endif

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    this.imageryPanel.Children.Clear();
                }
            }
            this.imageryPanel.InvalidateArrange();
        }


        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised
        /// on this element. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data. The event data reports that the left mouse button
        /// was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.ReleaseMouseCapture();
            this.isMouseDown = true;
           
        }
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an
        /// element in its route that is derived from this class. Implement this method to
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data. The event data reports that the left mouse button
        /// was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.isMouseDown = false;
            this.temposition = new Point(0, 0);
            this.Cursor = this.DefaultCursor;
            this.ReleaseMouseCapture(); 
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches an element in its route that is derived from this
        /// class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that
        /// contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Tile t = MapControl.FindParent<Tile>(e.OriginalSource as Image);
            if (t != null)
            {
                var pt = Tile.PointToLatLon((t.X * 256) + e.GetPosition(t).X, (t.Y * 256) + e.GetPosition(t).Y, (int)this.ZoomLevel);
                this.mapControl.LatLonPoint = new Point(pt.Y, pt.X);
            }

            var currentposition = e.GetPosition(this);
            if (this.isMouseDown)
            {
                this.CaptureMouse();
                this.Cursor = this.PanCursor;
                if (this.temposition.X == 0 && this.temposition.Y == 0)
                {
                    diffX = 0;
                    diffY = 0;
                }
                else
                {
                    diffX = currentposition.X - temposition.X;
                    diffY = currentposition.Y - temposition.Y;
                }

                this.PanMap(diffX + this.bingmapPanTransform.X, diffY + this.bingmapPanTransform.Y);
                this.temposition = e.GetPosition(this);
            }
        }
        #endregion

        #region HelperMethods

        private static bool IsConnected()
        {
            int Description;
            return InternetGetConnectedState(out Description, 0);
        }

        private void GenerateTiles(int ZoomLevel)
        {
            if (this.bingMapGrid != null)
            {
                xcount = ycount = Math.Pow(2, ZoomLevel);
                var endY = Math.Min(ycount, ((int)(-this.bingmapPanTransform.Y + this.bingMapGrid.ActualHeight) / 256) + 1);
                var endX = Math.Min(xcount, ((int)(-this.bingmapPanTransform.X + this.bingMapGrid.ActualWidth) / 256) + 1);
                var startX = -((int)this.bingmapPanTransform.X / 256);
                var startY = -((int)this.bingmapPanTransform.Y / 256);
                for (int i = startX; i < endX; i++)
                {
                    for (int j = startY; j < endY; j++)
                    {
                        var tempT = from tile in this.Tiles
                                    where tile.X == i && tile.Y == j
                                    select tile;
                        if (tempT.ToList().Count != 0)
                        {
                            continue;
                        }
                        else
                        {
                            Tile tile1 = new Tile { X = i, Y = j };
                            var latlon = Tile.TileXYToLatLong(i, j, (int)this.ZoomLevel);
                            if (latlon.X >= 85 || latlon.X <= -85)
                            {
                                continue;
                            }
                            if (latlon.Y >= 180 || latlon.Y <= -180)
                            {
                                continue;
                            }
                            this.Tiles.Add(tile1);
                        }
                    }
                }
            }
        }


        internal void PanMap(double factorX, double factorY)
        {
            if (this.EnablePan || this.isZoomLevelChanged)
            {
                this.bingmapPanTransform.X = factorX;
                this.bingmapPanTransform.Y = factorY;
                this.GenerateTiles((int)this.ZoomLevel);
                this.imageryPanel.InvalidateArrange();
                var panmode = this.GetPanmode(this.bingmapPanTransform.X, this.bingmapPanTransform.Y);
                this.IsPan = true;
                this.mapControl.panMode = panmode;

#if WPF
                this.panHandle = this.RaisePanningEvent(this.bingmapPanTransform.X, this.bingmapPanTransform.Y, panmode);
#endif
#if SILVERLIGHT
            PanEventArgs panEvtArgs = new PanEventArgs(this.mapControl.LatLonPoint.Y, this.mapControl.LatLonPoint.X, panmode);
            panHandle = this.OnPanning(this, panEvtArgs);
#endif
                this.tempPanX = this.bingmapPanTransform.X;
                this.tempPanY = this.bingmapPanTransform.Y;
                this.IsPan = true;
                this.isZoomLevelChanged = false;
            }

        }

        internal void ZoomMap(int zoomlevel)
        {


            if (this.EnableZoom)
            {
                if ((int)this.ZoomLevel > this.prevZoom)
                {
#if WPF
                    this.zoomInHandle = this.RaisePreviewZoomInEvent(this.mapControl.LatLonPoint.Y, this.mapControl.LatLonPoint.X, this.ZoomLevel);
#endif
#if SILVERLIGHT
                    ZoomEventArgs zoomInEvtArgs = new ZoomEventArgs(this.mapControl.LatLonPoint.Y, this.mapControl.LatLonPoint.X, this.ZoomLevel);
                    zoomInHandle = this.OnPreviewZoomIn(this, zoomInEvtArgs);
#endif
                }
                else
                {
#if WPF
                    this.zoomOutHandle = this.RaisePreviewZoomOutEvent(this.mapControl.LatLonPoint.Y, this.mapControl.LatLonPoint.X, this.ZoomLevel);
#endif
#if SILVERLIGHT
                    ZoomEventArgs zoomOutEvtArgs = new ZoomEventArgs(this.mapControl.LatLonPoint.Y, this.mapControl.LatLonPoint.X, this.ZoomLevel);
                    zoomOutHandle = this.OnPreviewZoomOut(this, zoomOutEvtArgs);
#endif
                }
                Dispatcher.BeginInvoke((Action)delegate()
                {
                    var panpt = Tile.LatLongToPoint(this.mapControl.LatLonPoint.Y, this.mapControl.LatLonPoint.X, (int)zoomlevel);
                
                    if (zoomPointerObject != null)
                    {
                        this.PanMap(-panpt.X + this.zoomPointPosition.X, -panpt.Y + this.zoomPointPosition.Y);
                    }
                    else
                    {
                        var pan = Tile.LatLongToPoint(0, 0, zoomlevel);
                        this.PanMap(-pan.X/2 , -pan.Y /2);
                    }
                    if (this.ZoomLevel > this.prevZoom)
                    {
                        if (!this.zoomInHandle)
                        {
#if WPF
                            this.RaiseZoomedInEvent(this.mapControl.LatLonPoint.Y, this.mapControl.LatLonPoint.X, this.ZoomFactor);

#endif
#if SILVERLIGHT
                            ZoomEventArgs zoomInEvtArgs = new ZoomEventArgs(this.mapControl.LatLonPoint.Y, this.mapControl.LatLonPoint.X, zoomlevel);
                            this.OnZoomIn(this, zoomInEvtArgs);
#endif
                        }
                    }
                    else
                    {
                        if (!this.zoomOutHandle)
                        {
#if WPF
                            this.RaiseZoomedOutEvent(this.mapControl.LatLonPoint.Y, this.mapControl.LatLonPoint.X, this.ZoomLevel);
#endif
#if SILVERLIGHT
                            ZoomEventArgs zoomOutEvtArgs = new ZoomEventArgs(this.mapControl.LatLonPoint.Y, this.mapControl.LatLonPoint.X, zoomlevel);
                            this.OnZoomedOut(this, zoomOutEvtArgs);
#endif
                        }
                    }

                });

            }
        }

        internal void CancelTileBackgroundWork()
        {
            foreach (Tile t in this.Tiles)
            {
#if WPF
                t.worker.CancelAsync();
#endif
            }
            this.Tiles.Clear();
        }

        /// <summary>
        ///  This method helps to load the Bing map in the Imagery Layer
        /// </summary>
        public void LoadBingMap()
        {
            if (this.BingMapKey != string.Empty)
            {
                this.GenerateTiles((int)this.ZoomLevel);
                this.imageryPanel.InvalidateArrange();
            }
        }

        #endregion
    }


}
