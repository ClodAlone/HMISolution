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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Data;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Reflection;
    using System.Collections;

    /// <summary>
    ///  MapLayer is ShapeFileLayer which contains map shapes
    /// </summary>
    public abstract class MapLayer : Control
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.MapLayer">MapLayer</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public MapLayer()
        {
            this.tempZoomfactor = this.ZoomFactor;
            this.IsPan = false;
        }

        #region internal Properties

        internal TransformGroup ShapeTransform
        {
            get;
            set;
        }

        internal TransformGroup ViewTransform
        {
            get;
            set;
        }

        internal ScaleTransform ZoomTransform
        {
            get;
            set;
        }

        internal TranslateTransform PanTransform
        {
            get;
            set;
        }

        internal Point CurrentPoint
        {
            get;
            set;
        }

        internal bool IsPan
        {
            get;
            set;
        }

        #endregion

        #region Private Fields

        private double tempZoom;
        private double tempZoomfactor;
        private DateTime secondClick;
        private MapControl mapControl;

        #endregion

        #region Internal Fields

        internal bool IsDroped = false;
        internal int flag_start_point = 0;
        internal int prevZoom;
        internal bool zoomInHandle = false;
        internal bool zoomOutHandle = false;
        internal bool panHandle = false;
        internal double tempPanX = 0;
        internal double tempPanY = 0;

        #endregion

        #region Properties

        #region LatLonPoint (Dependency Property)

        /// <summary>
        /// Gets or sets Latitude and Logitude Point of the Map.
        /// </summary>
        /// <value>
        /// Point
        /// </value>
        public Point LatLonPoint
        {
            get { return (Point)GetValue(LatLonPointProperty); }
            set { SetValue(LatLonPointProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LatLonPoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LatLonPointProperty =
            DependencyProperty.Register("LatLonPoint", typeof(Point), typeof(MapLayer), new PropertyMetadata(new Point(0, 0)));

        #endregion

        #region DisplayCoordinates (DependencyProperty)

        /// <summary>
        /// Gets / sets a value to display latitude and longitude co-ordinates.
        /// </summary>
        public bool DisplayCoordinates
        {
            get { return (bool)GetValue(DisplayCoordinatesProperty); }
            set { SetValue(DisplayCoordinatesProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for DisplayCoordinates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisplayCoordinatesProperty = DependencyProperty.Register("DisplayCoordinates", typeof(bool), typeof(MapLayer), new PropertyMetadata(true));

        #endregion

        #region ZoomFactor (DependencyProperty)

        /// <summary>
        /// Gets / ses the value of zooming factor
        /// </summary>
        public double ZoomFactor
        {
            get
            {
                return (double)GetValue(ZoomFactorProperty);
            }

            set
            {
                SetValue(ZoomFactorProperty, value);
            }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), typeof(MapLayer), new PropertyMetadata(1.0d, OnZoomFactorChanged));

        private static void OnZoomFactorChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var mapLayer = dpo as MapLayer;
            if (mapLayer != null)
            {

                if (args.NewValue != null)
                {
                    if ((double)args.NewValue < mapLayer.MinZoom)
                    {
                        mapLayer.ZoomFactor = mapLayer.MinZoom;
                    }
                    else if ((double)args.NewValue > mapLayer.MaxZoom)
                    {
                        mapLayer.ZoomFactor = mapLayer.MaxZoom;
                    }
                    else
                    {
                        mapLayer.Zoom((double)args.NewValue);
                    }

                }
            }
        }

        #endregion

        #region PanFactorX (DependencyProperty)

        /// <summary>
        /// Gets / sets the pan factor X value. This property only gives a XAML way to update the PAN position, it doesnot reflects the current state of the Pan transform.
        /// </summary>
        public double PanFactorX
        {
            get { return (double)GetValue(PanFactorXProperty); }
            set { SetValue(PanFactorXProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PanFactorX.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PanFactorXProperty = DependencyProperty.Register("PanFactorX", typeof(double), typeof(MapLayer), new PropertyMetadata(0.0d, OnPanFactorXChanged));

        private static void OnPanFactorXChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var mapLayer = dpo as MapLayer;
            mapLayer.Pan((double)args.NewValue, mapLayer.PanTransform.Y);
        }

        #endregion

        #region EnableZoom (DependencyProperty)

        /// <summary>
        /// Gets / sets if the control can zoom.
        /// </summary>
        public bool EnableZoom
        {
            get { return (bool)GetValue(EnableZoomProperty); }
            set { SetValue(EnableZoomProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableZoom.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableZoomProperty = DependencyProperty.Register("EnableZoom", typeof(bool), typeof(MapLayer), new PropertyMetadata(false));

        #endregion

        #region EnablePan (DependencyProperty)

        /// <summary>
        /// Gets / sets if the control can pan.
        /// </summary>
        public bool EnablePan
        {
            get { return (bool)GetValue(EnablePanProperty); }
            set { SetValue(EnablePanProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for EnablePan.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnablePanProperty = DependencyProperty.Register("EnablePan", typeof(bool), typeof(MapLayer), new PropertyMetadata(true));

        #endregion

        #region PanFactorY (DependencyProperty)

        /// <summary>
        /// Gets / sets the pan factor Y value. This property only gives a XAML way to update the PAN position, it doesnot reflects the current state of the Pan transform.
        /// </summary>
        public double PanFactorY
        {
            get { return (double)GetValue(PanFactorYProperty); }
            set { SetValue(PanFactorYProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for panFactorY.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PanFactorYProperty = DependencyProperty.Register("PanFactorY", typeof(double), typeof(MapLayer), new PropertyMetadata(0.0d, OnPanFactorYChanged));

        private static void OnPanFactorYChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var mapLayer = dpo as MapLayer;
            if (args.NewValue != null)
            {
                mapLayer.Pan(mapLayer.PanTransform.X, (double)args.NewValue);
            }
        }

        #endregion

        #region ZoomLevel (Dependency Property)

        /// <summary>
        /// Gets or sets the zoom level for the Map.
        /// </summary>
        /// <value>The zoom level.</value>
        public double ZoomLevel
        {
            get
            {
                return (double)GetValue(ZoomLevelProperty);
            }

            set
            {

                SetValue(ZoomLevelProperty, value);

            }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ZoomLevel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(double), typeof(MapLayer), new PropertyMetadata(1.0d, new PropertyChangedCallback(OnZoomLevelChanged)));

        private static void OnZoomLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageryLayer ilayer = d as ImageryLayer;
            if (ilayer != null)
            {
                ilayer.isZoomLevelChanged = true;
                var value = e.OldValue.ToString().Split('.').ElementAt(0).ToString();
                ilayer.prevZoom = int.Parse(value);
                ilayer.CancelTileBackgroundWork();
                var val = e.NewValue.ToString().Split('.').ElementAt(0).ToString();
                ilayer.Zoom(int.Parse(val));
            }

        }

        #endregion

        #region MinZoom (Dependency Property)

        /// <summary>
        /// Gets or sets the min zoom value of the Map.
        /// </summary>
        /// <value>The min zoom.</value>
        public double MinZoom
        {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for MinZoom.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(MapLayer), new PropertyMetadata(0.3d, new PropertyChangedCallback(OnMinZoomChanged)));

        private static void OnMinZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                MapLayer mapLayer = d as MapLayer;

                if ((double)e.NewValue > mapLayer.MaxZoom)
                {
                    mapLayer.MinZoom = mapLayer.MaxZoom;
                }
                else
                {
                    if ((double)e.NewValue > mapLayer.ZoomFactor)
                    {
                        mapLayer.ZoomFactor = (double)e.NewValue;
                    }
                }
            }
            else if (d is ImageryLayer)
            {
                var ilayer = d as ImageryLayer;
                if ((double)e.NewValue < 1)
                {
                    ilayer.MinZoom = 1;
                }
                else
                {
                    if ((double)e.NewValue > ilayer.MaxZoom)
                    {
                        ilayer.MinZoom = ilayer.MaxZoom;
                    }
                    else
                    {
                        if ((double)e.NewValue > ilayer.ZoomLevel)
                        {
                            ilayer.ZoomLevel = (double)e.NewValue;
                        }
                    }
                }
            }

        }

        #endregion

        #region MaxZoom (Dependency Property)

        /// <summary>
        /// Gets or sets the max zoom value of the Map.
        /// </summary>
        /// <value>The max zoom.</value>
        public double MaxZoom
        {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxZoom.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(MapLayer), new PropertyMetadata(20.0d, new PropertyChangedCallback(OnMaxZoomChanged)));

        private static void OnMaxZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                MapLayer mapLayer = d as MapLayer;
                if ((double)e.NewValue < mapLayer.MinZoom)
                {
                    mapLayer.MaxZoom = mapLayer.MinZoom;
                }
                else
                {
                    if ((double)e.NewValue < mapLayer.ZoomFactor)
                    {
                        mapLayer.ZoomFactor = (double)e.NewValue;
                    }
                }
            }
            if (d is ImageryLayer)
            {
                var ilayer = d as ImageryLayer;
                if ((double)e.NewValue > 17)
                {
                    ilayer.MaxZoom = 17;
                }
                if ((double)e.NewValue < ilayer.MinZoom)
                {
                    ilayer.MaxZoom = ilayer.MinZoom;
                }
                else
                {
                    if ((double)e.NewValue < ilayer.ZoomLevel)
                    {
                        ilayer.ZoomLevel = (double)e.NewValue;
                    }
                }

            }

        }


        #endregion

        #region ZoomMode (Dependency Property)

        /// <summary>
        /// Gets or sets the Zoom mode of the Map.
        /// </summary>
        /// <value>Zoom mode.</value>
        public ZoomingMode ZoomMode
        {
            get { return (ZoomingMode)GetValue(ZoomModeProperty); }
            set { SetValue(ZoomModeProperty, value); }
        }
        ///<summary>
        /// Using a DependencyProperty as the backing store for ZoomMode.  This enables animation, styling, binding, etc...
        ///</summary>
        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register("ZoomMode", typeof(ZoomingMode), typeof(MapLayer), new PropertyMetadata(ZoomingMode.DoubleClick));

        #endregion

        #endregion

        #region Zooming / Panning

        /// <summary>
        ///  This method helps to zooming the map
        /// </summary>
        /// <param name="zoomValue">Zooming value</param>
        public void Zoom(double zoomValue)
        {
            var mapControl = MapControl.FindParent<MapControl>(this) as MapControl;
            if (this.mapControl == null)
            {
                this.mapControl = MapControl.FindParent<MapControl>(this);
            }
            if (this.GetType() == typeof(ShapeFileLayer))
            {
                Point mouseposition = this.PanTransform.Inverse.Transform(this.CurrentPoint);

                if (!this.EnableZoom)
                {
                    return;
                }

                Point p = this.PointToLatitudeLongitude(this.CurrentPoint);

                if (p == null)
                {
                    p = new Point(0, 0);
                }

                if (this.ZoomFactor > this.tempZoom)
                {
#if WPF
                    this.zoomInHandle = this.RaisePreviewZoomInEvent(p.X, p.Y, this.ZoomFactor);
#endif
#if SILVERLIGHT
                    ZoomEventArgs zoomInEvtArgs = new ZoomEventArgs(p.X, p.Y, ZoomFactor);
                    zoomInHandle = this.OnPreviewZoomIn(this, zoomInEvtArgs);
#endif
                }
                else
                {
#if WPF
                    this.zoomOutHandle = this.RaisePreviewZoomOutEvent(p.X, p.Y, this.ZoomFactor);
#endif
#if SILVERLIGHT
                    ZoomEventArgs zoomOutEvtArgs = new ZoomEventArgs(p.X, p.Y, ZoomFactor);
                    zoomOutHandle = this.OnPreviewZoomOut(this, zoomOutEvtArgs);
#endif
                }

                this.OnZoom(zoomValue);

                // Compute the coordinates of the center of the canvas
                // in terms of pre-view transformation values. We do this
                // by applying the inverse of the view transform.
                // Temporarily reset the panning transformation.
                // Set the new zoom transformation scale factors.
                this.ZoomTransform.ScaleX = zoomValue;
                this.ZoomTransform.ScaleY = zoomValue;
                if (mapControl != null)
                {
                    (mapControl.LayeredContent as ShapeFileLayer).TempShapeStrokeThickness = (mapControl.LayeredContent as ShapeFileLayer).ShapeStrokeThickness / (mapControl.LayeredContent as ShapeFileLayer).ZoomFactor;
                }

                //// Apply the updated view transform to the canvas center.
                // This gives us the updated location of the center point
                // on the canvas. By differencing this with the desired
                // center of the canvas, we can determine the ideal panning
                // transformation parameters.
                this.OnZoomed(zoomValue);

                if (this.ZoomFactor > this.tempZoom)
                {
                    if (!this.zoomInHandle)
                    {
#if WPF
                        this.RaiseZoomedInEvent(p.X, p.Y, this.ZoomFactor);

#endif
#if SILVERLIGHT
                        ZoomEventArgs zoomInEvtArgs = new ZoomEventArgs(p.X, p.Y, ZoomFactor);
                        this.OnZoomIn(this, zoomInEvtArgs);
#endif
                    }
                }
                else
                {
                    if (!this.zoomOutHandle)
                    {
#if WPF
                        this.RaiseZoomedOutEvent(p.X, p.Y, this.ZoomFactor);
#endif
#if SILVERLIGHT
                        ZoomEventArgs zoomOutEvtArgs = new ZoomEventArgs(p.X, p.Y, ZoomFactor);
                        this.OnZoomedOut(this, zoomOutEvtArgs);
#endif
                    }
                }

                this.tempZoom = this.ZoomFactor;
            }
            else if (this.GetType() == typeof(ImageryLayer))
            {
                if (this.mapControl != null)
                {
                    var imageryLayer = this.mapControl.LayeredContent as ImageryLayer;
                    if (imageryLayer != null)
                    {
                        this.ZoomLevel = zoomValue;
                        imageryLayer.ZoomMap((int)zoomValue);
                    }
                }
            }
        }

        /// <summary>
        /// This method helps to panning the map
        /// </summary>
        /// <param name="factorX">double</param>
        /// <param name="factorY">double</param>
       
        public void Pan(double factorX, double factorY)
        {
            if (this.mapControl == null)
            {
                this.mapControl = MapControl.FindParent<MapControl>(this) as MapControl;
            }
            if (this.GetType() == typeof(ShapeFileLayer))
            {
                if (this.GetType() == typeof(ShapeFileLayer))
                {
                    PanMode panmode = this.GetPanmode(factorX, factorY);
                    this.mapControl.panMode = panmode;
                    if (!this.EnablePan)
                    {
                        return;
                    }

                    this.IsPan = true;

#if WPF
                    this.panHandle = this.RaisePanningEvent(this.LatLonPoint.X, this.LatLonPoint.Y, panmode);
#endif
#if SILVERLIGHT
                    PanEventArgs panEvtArgs = new PanEventArgs(this.LatLonPoint.X, this.LatLonPoint.Y, panmode);
                    panHandle = this.OnPanning(this, panEvtArgs);
#endif
                    this.PanTransform.X = factorX;
                    this.PanTransform.Y = factorY;
                    this.tempPanX = this.PanTransform.X;
                    this.tempPanY = this.PanTransform.Y;
                    this.OnPan(factorX, factorY);
                }
            }
            else if (this.GetType() == typeof(ImageryLayer))
            {
                if (this.mapControl.LayeredContent is ImageryLayer)
                {
                    (this.mapControl.LayeredContent as ImageryLayer).PanMap(factorX, factorY);
                }
            }

        }
        /// <summary>
        /// This method helps to panning the map
        /// </summary>
        /// <param name="factorX">double</param>
        /// <param name="factorY">double</param>
        protected virtual void OnPan(double factorX, double factorY)
        {
        }
        /// <summary>
        /// This method helps to Zooming the map
        /// </summary>
        protected virtual void OnZoom(double zoomfactor)
        {
        }
        /// <summary>
        /// This method Called when zooming is completed
        /// </summary>
        protected virtual void OnZoomed(double zoomfactor)
        {
        }

        /// <summary>
        ///  This method converts the point Value to LatitudeLongitude
        /// </summary>
        /// <param name="canvasPosition">Point</param>
        /// <returns>
        /// Point
        /// </returns>
        public Point PointToLatitudeLongitude(Point canvasPosition)
        {
            Point latLongPt = new Point();
            if (this.ShapeTransform != null)
            {
                // Convert from canvas position to lon/lat coordinates.
                latLongPt = this.GetLatLonCoordinates(canvasPosition);
            }

            return latLongPt;
        }

        internal Point GetLatLonCoordinates(Point canvasPosition)
        {
            // Apply the inverse of the view transformation.
            Point p1 = this.ViewTransform.Inverse.Transform(canvasPosition);

            // Apply the inverse of the shape transformation.
            if (this.ShapeTransform != null)
            {
                Point p2 = this.ShapeTransform.Inverse.Transform(p1);
                return p2;
            }

            return p1;
        }

        /// <summary>
        ///  This method converts the LatitudeLongitude to point Value
        /// </summary>
        /// <param name="latLonPt">Point</param>
        /// <returns>
        /// Point
        /// </returns>
        public Point LatitudeLongitudeToPoint(Point latLonPt)
        {
            var p1 = this.ViewTransform.Transform(new Point(0, 0));

            if (this.ShapeTransform != null)
            {
                if (this.ShapeTransform != null)
                {
                    var p2 = this.ShapeTransform.Transform(latLonPt);
                    var resultPoint = new Point((p2.X * this.ZoomFactor) + p1.X, (p2.Y * this.ZoomFactor) + p1.Y);
                    return resultPoint;
                }
            }

            return new Point();
        }
        internal Point GetMapElementsPosition(Point latLonPt)
        {
            var p1 = this.ViewTransform.Transform(latLonPt);

            if (this.ShapeTransform != null)
            {
                if (this.ShapeTransform != null)
                {
                    var p2 = this.ShapeTransform.Transform(latLonPt);
                    var roundedPt = new Point(((p2.X * this.ZoomTransform.ScaleX) + this.PanTransform.X), ((p2.Y * this.ZoomTransform.ScaleY) + this.PanTransform.Y));
                    return roundedPt;
                }
            }

            return new Point();
        }

        internal PanMode GetPanmode(double panX, double panY)
        {
            PanMode panmode;
            double diffX;
            double diffY;
            diffX = panX - this.tempPanX;
            diffY = panY - this.tempPanY;
            if (diffX > 0 && diffY > 0)
            {
                panmode = PanMode.RightBottom;
            }
            else if (diffX > 0 && diffY < 0)
            {
                panmode = PanMode.RightTop;
            }
            else if (diffX < 0 && diffY > 0)
            {
                panmode = PanMode.LeftBottom;
            }
            else if (diffX < 0 && diffY < 0)
            {
                panmode = PanMode.LeftTop;
            }
            else if (diffX < 0 && diffY == 0)
            {
                panmode = PanMode.Left;
            }
            else if (diffX > 0 && diffY == 0)
            {
                panmode = PanMode.Right;
            }
            else if (diffX == 0 && diffY < 0)
            {
                panmode = PanMode.Top;
            }
            else
            {
                panmode = PanMode.Bottom;
            }

            return panmode;
        }


        #endregion

        #region CustomEvents

#if WPF
        /// <summary>
        ///  This event is called when ZoomLevel is Incremented
        /// </summary>
        public static readonly RoutedEvent PreviewZoomInEvent = EventManager.RegisterRoutedEvent("PreviewZoomIn", RoutingStrategy.Bubble, typeof(ZoomEventHandler), typeof(MapLayer));
        /// <summary>
        ///  This event is called when ZoomLevel is Incremented
        /// </summary>
        public event ZoomEventHandler PreviewZoomIn
        {
            add
            {
                AddHandler(PreviewZoomInEvent, value);
            }

            remove
            {
                RemoveHandler(PreviewZoomInEvent, value);
            }
        }

        internal virtual void OnPreviewZoomIn(ZoomEventArgs e)
        {
            RaiseEvent(e);
        }

        internal bool RaisePreviewZoomInEvent(double lat, double log, double zoomfactor)
        {
            ZoomEventArgs zoomEvtArgs = new ZoomEventArgs(lat, log, zoomfactor);
            zoomEvtArgs.RoutedEvent = ShapeFileLayer.PreviewZoomInEvent;
            this.OnPreviewZoomIn(zoomEvtArgs);
            return zoomEvtArgs.Handled;
        }
#else
        /// <summary>
        ///  This event is called when ZoomLevel is Incremented
        /// </summary>
        public event ZoomEventHandler PreviewZoomIn;

        internal bool OnPreviewZoomIn(object sender, ZoomEventArgs args)
        {
            if (PreviewZoomIn != null)
            {
                PreviewZoomIn(sender, args);
                return args.Handled;
            }
            return false;
        }

#endif

#if WPF
        /// <summary>
        ///  This event is called when Zooming is Performed
        /// </summary>
        public static readonly RoutedEvent ZoomedInEvent = EventManager.RegisterRoutedEvent("ZoomedIn", RoutingStrategy.Bubble, typeof(ZoomEventHandler), typeof(MapLayer));
        /// <summary>
        ///  This event is called when Zooming is Performed
        /// </summary>
        public event ZoomEventHandler ZoomedIn
        {
            add
            {
                AddHandler(ZoomedInEvent, value);
            }

            remove
            {
                RemoveHandler(ZoomedInEvent, value);
            }
        }

        internal virtual void OnZoomedIn(ZoomEventArgs e)
        {
            RaiseEvent(e);
        }

        internal void RaiseZoomedInEvent(double lat, double log, double zoomfactor)
        {
            ZoomEventArgs zoomEvtArgs = new ZoomEventArgs(lat, log, zoomfactor);
            zoomEvtArgs.RoutedEvent = ShapeFileLayer.ZoomedInEvent;
            this.OnZoomedIn(zoomEvtArgs);
        }
#else
        /// <summary>
        ///  This event is called when Zooming is Performed
        /// </summary>
        public event ZoomEventHandler ZoomedIn;

        internal void OnZoomIn(object sender, ZoomEventArgs args)
        {
            if (ZoomedIn != null)
            {
                ZoomedIn(sender, args);
            }
        }
#endif

#if WPF
        /// <summary>
        ///  This event is called when Zoom level is decreased
        /// </summary>
        public static readonly RoutedEvent PreviewZoomOutEvent = EventManager.RegisterRoutedEvent("PreviewZoomOut", RoutingStrategy.Bubble, typeof(ZoomEventHandler), typeof(MapLayer));
        /// <summary>
        ///  This event is called when Zoom level is decreased
        /// </summary>
        public event ZoomEventHandler PreviewZoomOut
        {
            add
            {
                AddHandler(PreviewZoomOutEvent, value);
            }

            remove
            {
                RemoveHandler(PreviewZoomOutEvent, value);
            }
        }

        internal virtual void OnPreviewZoomOut(ZoomEventArgs e)
        {
            RaiseEvent(e);
        }

        internal bool RaisePreviewZoomOutEvent(double lat, double log, double zoomfactor)
        {
            ZoomEventArgs zoomEvtArgs = new ZoomEventArgs(lat, log, zoomfactor);
            zoomEvtArgs.RoutedEvent = ShapeFileLayer.PreviewZoomOutEvent;
            this.OnPreviewZoomOut(zoomEvtArgs);
            return zoomEvtArgs.Handled;
        }
#else
        /// <summary>
        ///  This event is called when ZoomLevel is decremented
        /// </summary>
        public event ZoomEventHandler PreviewZoomOut;

        internal bool OnPreviewZoomOut(object sender, ZoomEventArgs args)
        {
            if (PreviewZoomOut != null)
            {
                PreviewZoomOut(sender, args);
                return args.Handled;
            }
            return false;
        }
#endif

#if WPF
        /// <summary>
        ///  This event is called when Zoom level is decreased
        /// </summary>
        public static readonly RoutedEvent ZoomedOutEvent = EventManager.RegisterRoutedEvent("ZoomedOut", RoutingStrategy.Bubble, typeof(ZoomEventHandler), typeof(MapLayer));
        /// <summary>
        ///  This event is called when Zoom level is decreased
        /// </summary>
        public event ZoomEventHandler ZoomedOut
        {
            add
            {
                AddHandler(ZoomedOutEvent, value);
            }

            remove
            {
                RemoveHandler(ZoomedOutEvent, value);
            }
        }

        internal virtual void OnZoomedOut(ZoomEventArgs e)
        {
            RaiseEvent(e);
        }

        internal void RaiseZoomedOutEvent(double lat, double log, double zoomfactor)
        {
            ZoomEventArgs zoomEvtArgs = new ZoomEventArgs(lat, log, zoomfactor);
            zoomEvtArgs.RoutedEvent = ShapeFileLayer.ZoomedOutEvent;
            this.OnZoomedOut(zoomEvtArgs);
        }

#else
        /// <summary>
        ///  This event is called when Zoom level is decreased
        /// </summary>
        public event ZoomEventHandler ZoomedOut;

        internal void OnZoomedOut(object sender, ZoomEventArgs args)
        {
            if (ZoomedOut != null)
            {
                ZoomedOut(sender, args);
            }
        }
#endif

#if WPF
        /// <summary>
        ///  This event is called when Panning is performed
        /// </summary>
        public static readonly RoutedEvent PanningEvent = EventManager.RegisterRoutedEvent("Panning", RoutingStrategy.Bubble, typeof(PanEventHandler), typeof(MapLayer));
        /// <summary>
        ///  This event is called when Panning is performed
        /// </summary>
        public event PanEventHandler Panning
        {
            add
            {
                AddHandler(PanningEvent, value);
            }

            remove
            {
                RemoveHandler(PanningEvent, value);
            }
        }

        internal virtual void OnPanning(PanEventArgs e)
        {
            RaiseEvent(e);
        }

        internal bool RaisePanningEvent(double lat, double log, PanMode panmode)
        {
            PanEventArgs panEvtArgs = new PanEventArgs(lat, log, panmode);
            panEvtArgs.RoutedEvent = ShapeFileLayer.PanningEvent;
            this.OnPanning(panEvtArgs);
            return panEvtArgs.Handled;
        }
#else
        /// <summary>
        ///  This event is called when Panning is Performing
        /// </summary>
        public event PanEventHandler Panning;

        internal bool OnPanning(object sender, PanEventArgs args)
        {
            if (Panning != null)
            {
                Panning(sender, args);
                return args.Handled;
            }
            return false;
        }
#endif

#if WPF
        /// <summary>
        ///  This event is called when Panning is Completed
        /// </summary>
        public static readonly RoutedEvent PannedEvent = EventManager.RegisterRoutedEvent("Panned", RoutingStrategy.Bubble, typeof(PanEventHandler), typeof(MapLayer));

        /// <summary>
        ///  This event is called when Panning is Completed
        /// </summary>
        public event PanEventHandler Panned
        {
            add
            {
                AddHandler(PannedEvent, value);
            }

            remove
            {
                RemoveHandler(PannedEvent, value);
            }
        }

        internal virtual void OnPanned(PanEventArgs e)
        {
            RaiseEvent(e);
        }

        internal void RaisePannedEvent(double lat, double log, PanMode panmode)
        {
            PanEventArgs panEvtArgs = new PanEventArgs(lat, log, panmode);
            panEvtArgs.RoutedEvent = ShapeFileLayer.PannedEvent;
            this.OnPanned(panEvtArgs);
        }
#else
        /// <summary>
        ///  This event is called when Panning is Completed
        /// </summary>
        public event PanEventHandler Panned;

        internal void OnPanned(object sender, PanEventArgs args)
        {
            if (Panned != null)
            {
                Panned(sender, args);
            }
        }
#endif
        #endregion

        #region SymbolSelectedEvent

#if WPF

        /// <summary>
        ///  This event is called when Symbol is Selected
        /// </summary>
        public static readonly RoutedEvent SymbolSelectedEvent = EventManager.RegisterRoutedEvent("SymbolSelected", RoutingStrategy.Bubble, typeof(SelectionEventHandler), typeof(MapLayer));
        /// <summary>
        ///  This event is called when Symbol is Selected
        /// </summary>
        public event SelectionEventHandler SymbolSelected
        {
            add
            {
                AddHandler(SymbolSelectedEvent, value);
            }
            remove
            {
                RemoveHandler(SymbolSelectedEvent, value);
            }

        }

        internal virtual void OnSymbolSelected(SelectionEventArgs e)
        {
            RaiseEvent(e);
        }

        internal void RaiseSymbolSelectedEvent(object item)
        {
            SelectionEventArgs evtargs = new SelectionEventArgs(item);
            evtargs.RoutedEvent = MapLayer.SymbolSelectedEvent;
            this.OnSymbolSelected(evtargs);
        }
#else
        /// <summary>
        ///  This event is called when Symbol is Selected
        /// </summary>
        public event SelectionEventHandler SymbolSelected;

        internal void OnSymbolSelected(object sender, SelectionEventArgs args)
        {
            if (SymbolSelected != null)
            {
                SymbolSelected(sender, args);
            }
        }

#endif


        #endregion

        #region SymbolSelectedEvent

#if WPF
        /// <summary>
        ///  This event is called when Symbol is UnSelected
        /// </summary>
        public static readonly RoutedEvent SymbolUnSelectedEvent = EventManager.RegisterRoutedEvent("SymbolUnSelected", RoutingStrategy.Bubble, typeof(SelectionEventHandler), typeof(MapLayer));
        /// <summary>
        ///  This event is called when Symbol is UnSelected
        /// </summary>
        public event SelectionEventHandler SymbolUnSelected
        {
            add
            {
                AddHandler(SymbolUnSelectedEvent, value);
            }
            remove
            {
                RemoveHandler(SymbolUnSelectedEvent, value);
            }

        }

        internal virtual void OnSymbolUnSelected(SelectionEventArgs e)
        {
            RaiseEvent(e);
        }

        internal void RaiseSymbolUnSelectedEvent(object item)
        {
            SelectionEventArgs evtargs = new SelectionEventArgs(item);
            evtargs.RoutedEvent = MapLayer.SymbolUnSelectedEvent;
            this.OnSymbolUnSelected(evtargs);
        }
#else
        /// <summary>
        ///  This event is called when Symbol is UnSelected
        /// </summary>
        public event SelectionEventHandler SymbolUnSelected;
        internal void OnSymbolUnSelected(object sender, SelectionEventArgs args)
        {
            if (SymbolUnSelected != null)
            {
                this.SymbolUnSelected(sender, args);
            }
        }
#endif


        #endregion

        #region LabelSelectedEvent

#if WPF
        /// <summary>
        ///  This event is called when Label is Selected
        /// </summary>
        public static readonly RoutedEvent LabelSelectedEvent = EventManager.RegisterRoutedEvent("LabelSelected", RoutingStrategy.Bubble, typeof(SelectionEventHandler), typeof(MapLayer));
        /// <summary>
        ///  This event is called when Symbol is Selected
        /// </summary>
        public event SelectionEventHandler LabelSelected
        {
            add
            {
                AddHandler(LabelSelectedEvent, value);
            }
            remove
            {
                RemoveHandler(LabelSelectedEvent, value);
            }

        }

        internal virtual void OnLabelSelected(SelectionEventArgs e)
        {
            RaiseEvent(e);
        }

        internal void RaiseLabelSelectedEvent(object item)
        {
            SelectionEventArgs evtargs = new SelectionEventArgs(item);
            evtargs.RoutedEvent = MapLayer.LabelSelectedEvent;
            this.OnLabelSelected(evtargs);
        }

        /// <summary>
        /// Occurs when MouseHover on shape
        /// </summary>
        public event SelectionEventHandler ShapeHovered
        {
            add
            {
                AddHandler(ShapeHoveredEvent, value);
            }

            remove
            {
                RemoveHandler(ShapeHoveredEvent, value);
            }
        }

        /// <summary>
        ///  This is a Registerd SelectionChangedEvent
        /// </summary>
        public static readonly RoutedEvent ShapeHoveredEvent = EventManager.RegisterRoutedEvent("ShapeHovered", RoutingStrategy.Bubble, typeof(SelectionEventHandler), typeof(MapLayer));

        /// <summary>
        ///  Occured when Selection is changed
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Controls.SelectionChangedEventArgs"/> that contains the
        /// event data.</param>
        internal virtual void OnHoverChanged(SelectionEventArgs e)
        {
            RaiseEvent(e);
        }

        internal void RaiseHoverChanged(object item)
        {
            SelectionEventArgs evtargs = new SelectionEventArgs(item);
            evtargs.RoutedEvent = ShapeHoveredEvent;
            OnHoverChanged(evtargs);
        }

#else
        /// <summary>
        /// Occurs when Label is selected on Map Layer.
        /// </summary>
        public event SelectionEventHandler LabelSelected;

        internal void OnLabelSelected(object sender, SelectionEventArgs args)
        {
            if (this.LabelSelected != null)
            {
                this.LabelSelected(sender, args);
            }
        }

#endif


        #endregion

        #region LabelUnSelectedEvent

#if WPF
        /// <summary>
        ///  This event is called when Label is UnSelected
        /// </summary>
        public static readonly RoutedEvent LabelUnSelectedEvent = EventManager.RegisterRoutedEvent("LabelUnSelected", RoutingStrategy.Bubble, typeof(SelectionEventHandler), typeof(MapLayer));
        /// <summary>
        ///  This event is called when Label is UnSelected
        /// </summary>
        public event SelectionEventHandler LabelUnSelected
        {
            add
            {
                AddHandler(LabelUnSelectedEvent, value);
            }
            remove
            {
                RemoveHandler(LabelUnSelectedEvent, value);
            }

        }

        internal virtual void OnLabelUnSelected(SelectionEventArgs e)
        {
            RaiseEvent(e);
        }

        internal void RaiseLabelUnSelectedEvent(object item)
        {
            SelectionEventArgs evtargs = new SelectionEventArgs(item);
            evtargs.RoutedEvent = MapLayer.LabelUnSelectedEvent;
            this.OnLabelUnSelected(evtargs);
        }
#else
        /// <summary>
        /// Occurs when Label is Unselected on Map Layer.
        /// </summary>
        public event SelectionEventHandler LabelUnSelected;

        internal void OnLabelUnSelected(object sender, SelectionEventArgs args)
        {
            if (this.LabelUnSelected != null)
            {
                this.LabelUnSelected(sender, args);
            }
        }

#endif


        #endregion

        #region Event Handlers

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
            if (this.EnableZoom)
            {
                if (this.mapControl == null)
                {
                    this.mapControl = MapControl.FindParent<MapControl>(this) as MapControl;
                }
                if (this.GetType() == typeof(ShapeFileLayer))
                {
                    if (!(this.mapControl.LayeredContent as ShapeFileLayer).IsDynamicCreatePath)
                    {
                        if (this.ZoomMode == ZoomingMode.SingleClick && !this.IsPan && Keyboard.Modifiers == ModifierKeys.None && !this.IsDroped)
                        {
                            this.ZoomLevel += 0.1;
                        }
                        else
                        {
                            if (!this.IsPan && (DateTime.Now.Subtract(this.secondClick).TotalMilliseconds < 500) && Keyboard.Modifiers == ModifierKeys.None)
                            {
                                this.ZoomLevel += 0.1;
                            }
                        }

                        this.secondClick = DateTime.Now;
                    }
                }
                else
                {
                    if (this.mapControl != null)
                    {
                        var ilayer = this.mapControl.LayeredContent as ImageryLayer;
                        ilayer.zoomPointerObject = MapControl.FindParent<Tile>(e.OriginalSource as UIElement);
                        if (this.ZoomMode == ZoomingMode.SingleClick && !this.IsPan && Keyboard.Modifiers == ModifierKeys.None && !this.IsDroped)
                        {
                            if ((int)ZoomLevel < this.MaxZoom)
                            {
                                ilayer.zoomPointPosition = e.GetPosition(ilayer);
                                ilayer.CancelTileBackgroundWork();
                                ilayer.ZoomLevel++;
                            }
                        }
                        else
                        {
                            if (!this.IsPan && (DateTime.Now.Subtract(this.secondClick).TotalMilliseconds < 500) && Keyboard.Modifiers == ModifierKeys.None)
                            {
                                if ((int)ZoomLevel < this.MaxZoom)
                                {
                                    ilayer.zoomPointPosition = e.GetPosition(ilayer);
                                    ilayer.CancelTileBackgroundWork();
                                    ilayer.ZoomLevel++;
                                }
                            }
                        }


                        this.secondClick = DateTime.Now;
                    }
                }
            }


            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseWheel"/> attached event reaches an element in its route that is derived from this
        /// class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs"/>
        /// that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (this.EnableZoom)
            {
                if (this.mapControl == null)
                {
                    this.mapControl = MapControl.FindParent<MapControl>(this) as MapControl;
                }
                if (this.GetType() == typeof(ShapeFileLayer))
                {
                    if (this.EnableZoom)
                    {
                        if (e.Delta > 0 && this.ZoomFactor <= this.MaxZoom)
                        {
                            if (this.ZoomFactor == this.MaxZoom)
                            {
                                return;
                            }
                            this.ZoomLevel += 0.1;
                        }
                        else
                        {
                            if (this.ZoomFactor >= this.MinZoom)
                            {
                                if ((this.ZoomFactor - 0.1) < this.MinZoom)
                                {
                                    return;
                                }
                                this.ZoomLevel -= 0.1;
                            }
                        }
                    }
                    if (this.DisplayCoordinates)
                    {
                        var mousePs = e.GetPosition(this);
                        var latLonPt = this.PointToLatitudeLongitude(mousePs);
                        this.LatLonPoint = latLonPt;
                    }
                }
                else if (this.GetType() == typeof(ImageryLayer))
                {
                    var ilayer = this.mapControl.LayeredContent as ImageryLayer;
                    if (this.mapControl != null)
                    {
                        ilayer.zoomPointPosition = e.GetPosition(ilayer);
                        ilayer.zoomPointerObject = e.OriginalSource;
                        var zoomcnt = e.Delta / 120;
                        if (e.Delta > 0)
                        {
                            if ((int)ilayer.ZoomLevel < ilayer.MaxZoom)
                            {
                                ilayer.CancelTileBackgroundWork();
                                ilayer.ZoomLevel++;
                            }
                        }
                        else
                        {
                            if ((int)ilayer.ZoomLevel > ilayer.MinZoom)
                            {
                                ilayer.CancelTileBackgroundWork();
                                ilayer.ZoomLevel--;
                            }

                        }
                    }
                }
            }
            base.OnMouseWheel(e);
        }
        #endregion


    }
}
