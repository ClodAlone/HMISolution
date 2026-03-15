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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using System.IO;
    using System.Reflection;
    using System.Windows.Controls.Primitives;
    using System.Diagnostics;
    using System.Windows.Media.Imaging;
    using System.Globalization;
    using System.Windows.Markup;

#if WPF
    using Syncfusion.Licensing;
    using Syncfusion.Windows.Shared;
    using System.Security.Permissions;
    using System.Xml;
    using Microsoft.Win32;

#endif

    /// <summary>
    ///  A map is a graphical representation of geographical data. It is used to
    /// represent the statistical data of a particular geographical area on Earth. Using
    /// pan and zoom, the maps can be navigated. 
    /// </summary>
    public class MapControl : ContentControl
    {
        #region Private Fields

        private Storyboard opacityAnimator1;
        private Storyboard opacityAnimator2;
        internal NavigationControl navigationControl;
        private LegendPanel LegendPanel;
        private SaveFileDialog saveFileDialog = new SaveFileDialog();
        private OpenFileDialog openFileDialog = new OpenFileDialog();

#if WPF
        private BitmapEncoder bitmapCoder;
        private bool isImageCaptured = false;
#endif
        #endregion

        #region Internal Fields

        internal Point CurrentPoint;
        internal double tempZoomFactor = 1;
        internal bool isMouseUp = false;
        internal bool isMouseDown = false;
#if SILVERLIGHT
        internal Popup symbolPreviewPopup;
#endif
        internal Point startPoint;
        internal Point panTranslatePoint;
#if WPF
        internal Border selectionadorner;
        internal Rect selectedArea;
#endif
        internal ScrollViewer mapScrollViewer;
        internal static bool isXmlContent = false;
        internal bool resetFlag = false;
        internal TextBlock coordinateLabel;
        internal PanMode panMode;
        internal bool isLabelEdited = false;

        #endregion

        #region Internal Properties



        internal object SelectedSymbolItem
        {
            get { return (object)GetValue(SelectedSymbolItemProperty); }
            set { SetValue(SelectedSymbolItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedSymbolItem.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectedSymbolItemProperty =
            DependencyProperty.Register("SelectedSymbolItem", typeof(object), typeof(MapControl), new PropertyMetadata(null));



        #endregion

        #region Properties

#if SILVERLIGHT

        #region VisualStyle(Dependency Property)



        /// <summary>
        /// Gets or sets Visual Style for the Map Control.
        /// </summary>
        /// <value>
        /// VisualStyle
        /// </value>
        public VisualStyles VisualStyle
        {
            get { return (VisualStyles)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for VisualStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyles", typeof(VisualStyles), typeof(MapControl), new PropertyMetadata(VisualStyles.Default));

        #endregion


#endif
        #region Legends


        /// <summary>
        /// Gets or sets Legends.
        /// </summary>
        /// <value>
        /// Collection of Legend
        /// </value>
        public ObservableCollection<Legend> Legends
        {
            get { return (ObservableCollection<Legend>)GetValue(LegendsProperty); }
            set { SetValue(LegendsProperty, value); }
        }



        // Using a DependencyProperty as the backing store for Legends.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies the Legend dependency property
        /// </summary>
        public static readonly DependencyProperty LegendsProperty =
            DependencyProperty.Register("Legends", typeof(ObservableCollection<Legend>), typeof(MapControl), new PropertyMetadata(null));


        #endregion

        #region LegendPosition



        /// <summary>
        /// Gets or sets Legend Position.
        /// </summary>
        /// <value>
        /// Legend
        /// </value>
        public LegendPosition LegendPosition
        {
            get { return (LegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies the LegendPosition dependency property
        /// </summary>
        public static readonly DependencyProperty LegendPositionProperty =
            DependencyProperty.Register("Position", typeof(LegendPosition), typeof(MapControl), new PropertyMetadata(LegendPosition.Default, new PropertyChangedCallback(OnLegendPositionChanged)));

             
        private static void OnLegendPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapControl mapControl = d as MapControl;
            if (e.NewValue != null)
            {
                mapControl.ChangeLegendPosition();
            }
        }

        #endregion

        #region LegendVisibility



        /// <summary>
        /// Gets or sets LegendVisibility.
        /// </summary>
        /// <value>
        /// Visibility
        /// </value>
        public Visibility LegendVisibility
        {
            get { return (Visibility)GetValue(LegendVisibilityProperty); }
            set { SetValue(LegendVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendVisibility.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies LegendVisibility Dependency property
        /// </summary>
        public static readonly DependencyProperty LegendVisibilityProperty =
            DependencyProperty.Register("LegendVisibility", typeof(Visibility), typeof(MapControl), new PropertyMetadata(Visibility.Collapsed));



        #endregion

        #region LatitudeLongitudeType

        /// <summary>
        /// Gets or sets the type of the latitude longitude.
        /// </summary>
        /// <value>The type of the latitude longitude.This type only displayed in the Map.</value>
        public LatitudeLongitudeType LatitudeLongitudeType
        {
            get { return (LatitudeLongitudeType)GetValue(LatitudeLongitudeTypeProperty); }
            set { SetValue(LatitudeLongitudeTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LatitudeLongitudeType.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies LatitudeLongtitudeType Dependency property
        /// </summary>
        public static readonly DependencyProperty LatitudeLongitudeTypeProperty =
            DependencyProperty.Register("LatitudeLongitudeType", typeof(LatitudeLongitudeType), typeof(MapControl), new PropertyMetadata(LatitudeLongitudeType.DMS, new PropertyChangedCallback(OnLatLanTypeChanged)));
                
        private static void OnLatLanTypeChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs dargs)
        {
            MapControl mapControl = dobj as MapControl;
            if (dargs.NewValue != null)
            {
                mapControl.ChangeLatitudeAndLogitudeType((LatitudeLongitudeType)dargs.NewValue);
            }
        }

        #endregion

        #region LatLonConverter (Dependency Property)

        /// <summary>
        /// Gets or sets LagtitudeLontitude Converter.
        /// </summary>
        /// <value>
        /// IValueConverter
        /// </value>
        public IValueConverter LatLonConverter
        {
            get { return (IValueConverter)GetValue(LatLonConverterProperty); }
            set { SetValue(LatLonConverterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LanLanConverter.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies LatLonConverter dependency property
        /// </summary>
        public static readonly DependencyProperty LatLonConverterProperty =
            DependencyProperty.Register("LatLonConverter", typeof(IValueConverter), typeof(MapControl), new PropertyMetadata(new LatitudeLongitudeToTextConverter()));

        #endregion

        #region LatLonPoint (Dependency Property)

        /// <summary>
        /// Gets or sets Latitude and Lontitude Point.
        /// </summary>
        /// <value>
        /// Point
        /// </value>
        public Point LatLonPoint
        {
            get { return (Point)GetValue(LatLonPointProperty); }
            set { SetValue(LatLonPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LatLonPoint.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies LatLon Dependency property
        /// </summary>
        public static readonly DependencyProperty LatLonPointProperty =
            DependencyProperty.Register("LatLonPoint", typeof(Point), typeof(MapControl), new PropertyMetadata(new Point(0, 0)));

        #endregion

        #region CurrentMapColorPalette

        /// <summary>
        /// Gets or sets CurrentMapColorPalette.
        /// </summary>
        /// <value>
        /// Collection of MapColorPalette
        /// </value>
        public ObservableCollection<MapColorPallette> CurrentMapColorPalette
        {
            get { return (ObservableCollection<MapColorPallette>)GetValue(CurrentMapColorPaletteProperty); }
            set { SetValue(CurrentMapColorPaletteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentMapPalette.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies CurrentMapColorPalette dependency property
        /// </summary>
        public static readonly DependencyProperty CurrentMapColorPaletteProperty =
            DependencyProperty.Register("CurrentMapColorPalette", typeof(ObservableCollection<MapColorPallette>), typeof(MapControl), new PropertyMetadata(new ObservableCollection<MapColorPallette>()));

        #endregion

        #region ShowNavigationControl (Dependency Property)

        /// <summary>
        /// Gets or sets a value indicating whether Navigation Control to be shown or not.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool ShowNavigationControl
        {
            get { return (bool)GetValue(ShowNavigationControlProperty); }
            set { SetValue(ShowNavigationControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNavigationControl.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies ShowNavigationControl Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowNavigationControlProperty =
            DependencyProperty.Register("ShowNavigationControl", typeof(bool), typeof(MapControl), new PropertyMetadata(true));


        #endregion

        #region NavigationControlPosition (Dependency Property)

        /// <summary>
        /// Gets or sets Position where the Navigation control should placed.
        /// </summary>
        /// <remarks>
        /// Default NavigationPosition is Left
        /// </remarks>
        /// <value>
        /// NavigationControlPositions
        /// </value>
        public NavigationControlPositions NavigationControlPosition
        {
            get { return (NavigationControlPositions)GetValue(NavigationControlPositionProperty); }
            set { SetValue(NavigationControlPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationControlPosition.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies NavigationControlposition dependency property
        /// </summary>
        public static readonly DependencyProperty NavigationControlPositionProperty =
            DependencyProperty.Register("NavigationControlPosition", typeof(NavigationControlPositions), typeof(MapControl), new PropertyMetadata(NavigationControlPositions.Left));


        #endregion

        #region ShowLatLonPoint

        /// <summary>
        /// Gets or sets a value indicating whether to Show LatitudeLontitude Points or not.
        /// </summary>
        /// <remarks>
        /// Default Value is True
        /// </remarks>
        /// <value>
        /// 	<see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool ShowLatLonPoints
        {
            get { return (bool)GetValue(ShowLatLonPointsProperty); }
            set { SetValue(ShowLatLonPointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowLatLonPoints.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies ShowLatLonPoints Dependency property
        /// </summary>
        public static readonly DependencyProperty ShowLatLonPointsProperty =
            DependencyProperty.Register("ShowLatLonPoints", typeof(bool), typeof(MapControl), new PropertyMetadata(true));

        #endregion

        #region TranslateZoomFactor

        /// <summary>
        /// Gets or sets TranslateZoomFactor.
        /// </summary>
        /// <remarks>
        /// Default Value is 0
        /// </remarks>
        /// <value>
        /// double
        /// </value>
        public double TranslateZoomFactor
        {
            get { return (double)GetValue(TranslateZoomFactorProperty); }
            set { SetValue(TranslateZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TranslateZoomFactor.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies TranslateZoomFactor Dependency Property
        /// </summary>
        public static readonly DependencyProperty TranslateZoomFactorProperty =
            DependencyProperty.Register("TranslateZoomFactor", typeof(double), typeof(MapControl), new PropertyMetadata(0d));

        #endregion

        #region LayeredContent (Dependency Property)

        /// <summary>
        /// Gets or sets LayeredContent. It May be always ShapeFileLayer.
        /// </summary>
        /// <value>
        /// UIElement
        /// </value>
        public UIElement LayeredContent
        {
            get { return (UIElement)GetValue(LayeredContentProperty); }
            set { SetValue(LayeredContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LayeredElement.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies LayeredContent Dependency Property
        /// </summary>
        public static readonly DependencyProperty LayeredContentProperty =
            DependencyProperty.Register("LayeredContent", typeof(UIElement), typeof(MapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnLayeredContentChanged)));


        private static void OnLayeredContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapControl map = d as MapControl;
            if (e.OldValue != null)
            {
                if (e.OldValue is MapLayer)
                {
                    map.UnWireZoomEvents(e.OldValue as MapLayer);
                }
            }
            if (e.NewValue != null)
            {
                if (e.NewValue is MapLayer)
                {                   
                    if (e.NewValue is ShapeFileLayer)
                    {
                        (e.NewValue as ShapeFileLayer).isRefreshed = false;
                        if (e.OldValue != null && e.OldValue is ShapeFileLayer)
                        {
                            (e.NewValue as ShapeFileLayer).ZoomTransform.ScaleX = (e.OldValue as ShapeFileLayer).ZoomTransform.ScaleX;
                            (e.NewValue as ShapeFileLayer).ZoomTransform.ScaleY = (e.OldValue as ShapeFileLayer).ZoomTransform.ScaleY;
                            (e.NewValue as ShapeFileLayer).ZoomTransform.CenterX = (e.OldValue as ShapeFileLayer).ZoomTransform.CenterX;
                            (e.NewValue as ShapeFileLayer).ZoomTransform.CenterY = (e.OldValue as ShapeFileLayer).ZoomTransform.CenterY;
                            (e.NewValue as ShapeFileLayer).PanTransform.X = (e.OldValue as ShapeFileLayer).PanTransform.X;
                            (e.NewValue as ShapeFileLayer).PanTransform.Y = (e.OldValue as ShapeFileLayer).PanTransform.Y;

                        }
                    }
                    MapLayer mapLayer = e.NewValue as MapLayer;
                    mapLayer.SetBinding(MapLayer.ZoomFactorProperty, new Binding { Source = map, Path = new PropertyPath("ZoomFactor"), Mode = BindingMode.TwoWay });
                    mapLayer.SetBinding(MapLayer.ZoomLevelProperty, new Binding { Source = map, Path = new PropertyPath("ZoomLevel"), Mode = BindingMode.TwoWay });
                    mapLayer.SetBinding(MapLayer.ZoomModeProperty, new Binding { Source = map, Path = new PropertyPath("ZoomMode"), Mode = BindingMode.TwoWay });
                    mapLayer.SetBinding(MapLayer.MinZoomProperty, new Binding { Source = map, Path = new PropertyPath("MinZoom"), Mode = BindingMode.TwoWay });
                    mapLayer.SetBinding(MapLayer.MaxZoomProperty, new Binding { Source = map, Path = new PropertyPath("MaxZoom"), Mode = BindingMode.TwoWay });
                    mapLayer.SetBinding(MapLayer.PanFactorXProperty, new Binding { Source = map, Path = new PropertyPath("PanFactorX"), Mode = BindingMode.TwoWay });
                    mapLayer.SetBinding(MapLayer.PanFactorYProperty, new Binding { Source = map, Path = new PropertyPath("PanFactorY"), Mode = BindingMode.TwoWay });
                    mapLayer.SetBinding(MapLayer.EnablePanProperty, new Binding { Source = map, Path = new PropertyPath("EnablePan"), Mode = BindingMode.TwoWay });
                    mapLayer.SetBinding(MapLayer.EnableZoomProperty, new Binding { Source = map, Path = new PropertyPath("EnableZoom"), Mode = BindingMode.TwoWay });
                    map.SetBinding(MapControl.TranslateZoomFactorProperty, new Binding { Source = mapLayer, Path = new PropertyPath("TranslateZoomFactor"), Mode = BindingMode.TwoWay });
                    if (mapLayer is ShapeFileLayer)
                    {
                        mapLayer.SetBinding(ShapeFileLayer.CustomMapColorPaletteProperty, new Binding { Source = map, Path = new PropertyPath("CustomMapColorPalette"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.EnableColorPaletteProperty, new Binding { Source = map, Path = new PropertyPath("EnableColorPalette"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.ColorPaletteProperty, new Binding { Source = map, Path = new PropertyPath("ColorPalette"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.CurrentMapColorPaletteProperty, new Binding { Source = map, Path = new PropertyPath("CurrentMapColorPalette"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.ColorPaletteModeProperty, new Binding { Source = map, Path = new PropertyPath("ColorPaletteMode"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.SelectedItemProperty, new Binding { Source = map, Path = new PropertyPath("SelectedItem"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.MouseOverItemProperty, new Binding { Source = map, Path = new PropertyPath("MouseOverItem"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.EnableHoverEffectsProperty, new Binding { Source = map, Path = new PropertyPath("EnableHoverEffects"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.ShapeHoverFillProperty, new Binding { Source = map, Path = new PropertyPath("ShapeHoverFill"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.ShapeHoverStrokeProperty, new Binding { Source = map, Path = new PropertyPath("ShapeHoverStroke"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.ShapeHoverStrokeThicknessProperty, new Binding { Source = map, Path = new PropertyPath("ShapeHoverStrokeThickness"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.ShapeFillProperty, new Binding { Source = map, Path = new PropertyPath("ShapeFill"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.ShapeStrokeProperty, new Binding { Source = map, Path = new PropertyPath("ShapeStroke"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.ShapeStrokeThicknessProperty, new Binding { Source = map, Path = new PropertyPath("ShapeStrokeThickness"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.PolygonFillProperty, new Binding { Source = map, Path = new PropertyPath("PolygonFill"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.PolygonStrokeProperty, new Binding { Source = map, Path = new PropertyPath("PolygonStroke"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.ShowLatLonPointsProperty, new Binding { Source = map, Path = new PropertyPath("ShowLatLonPoints"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.LatLonPointProperty, new Binding { Source = map, Path = new PropertyPath("LatLonPoint"), Mode = BindingMode.TwoWay });
                        mapLayer.SetBinding(ShapeFileLayer.LatLonConverterProperty, new Binding { Source = map, Path = new PropertyPath("LatLonConverter"), Mode = BindingMode.TwoWay });
                    }
                    map.WireZoomEvents(e.NewValue as MapLayer);
                }


            }
        }

        #endregion

        #region Layers (Dependency Property)

        /// <summary>
        /// Gets or sets Layers in Map Control .
        /// </summary>
        /// <value>
        /// Layers
        /// </value>
        public Layers Layers
        {
            get { return (Layers)GetValue(LayersProperty); }
            set { SetValue(LayersProperty, value); }
        }


        // Using a DependencyProperty as the backing store for Layers.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies Layers Dependency Property
        /// </summary>
        public static readonly DependencyProperty LayersProperty =
            DependencyProperty.Register("Layers", typeof(Layers), typeof(MapControl), new PropertyMetadata(null));

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
        /// Identifies ZoomFactor Dependency Property
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register("ZoomFactor", typeof(double), typeof(MapControl), new PropertyMetadata(1.0d));

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
        ///  Identifies PanFactorX Dependency Property
        /// </summary>
        public static readonly DependencyProperty PanFactorXProperty = DependencyProperty.Register("PanFactorX", typeof(double), typeof(MapControl), new PropertyMetadata(0.0d));

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
        /// Identifies PanFactorY Dependency Property
        /// </summary>
        public static readonly DependencyProperty PanFactorYProperty = DependencyProperty.Register("PanFactorY", typeof(double), typeof(MapControl), new PropertyMetadata(0.0d));

        #endregion

        #region EnableZoom (DependencyProperty)

        /// <summary>
        /// Gets / sets if the control can zoom.
        /// </summary>
        /// /// <value>
        /// 	<see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool EnableZoom
        {
            get { return (bool)GetValue(EnableZoomProperty); }
            set { SetValue(EnableZoomProperty, value); }
        }

        /// <summary>
        ///  Identifies EnableZoom Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableZoomProperty = DependencyProperty.Register("EnableZoom", typeof(bool), typeof(MapControl), new PropertyMetadata(true));

        #endregion

        #region EnablePan (DependencyProperty)

        /// <summary>
        /// Gets / sets if the control can pan.
        /// </summary>
        /// /// <value>
        /// 	<see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool EnablePan
        {
            get { return (bool)GetValue(EnablePanProperty); }
            set { SetValue(EnablePanProperty, value); }
        }

        /// <summary>
        ///  Identifies EnablePan Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnablePanProperty = DependencyProperty.Register("EnablePan", typeof(bool), typeof(MapControl), new PropertyMetadata(true));

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
                if (this.ZoomLevel >= 1)
                {
                    SetValue(ZoomLevelProperty, value);
                }
                else
                {
                    SetValue(ZoomLevelProperty, 1.0d);
                }
            }
        }

        /// <summary>
        ///  Identifies ZoomLevel Dependency Property
        /// </summary>
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(double), typeof(MapControl), new PropertyMetadata(1.0d));

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
        ///  Identifies MinZoom Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(double), typeof(MapControl), new PropertyMetadata(0.3d, new PropertyChangedCallback(OnMinZoomChanged)));

        
        private static void OnMinZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapControl mapControl = d as MapControl;
            mapControl.ChangeNaviagtionControlSliderValue();
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
        ///  Identifies MaxZoom Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register("MaxZoom", typeof(double), typeof(MapControl), new PropertyMetadata(20.0d, new PropertyChangedCallback(OnMaxZoomChanged)));

        private static void OnMaxZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapControl mapControl = d as MapControl;
            mapControl.ChangeNaviagtionControlSliderValue();
        }
        #endregion

        #region ZoomMode (Dependency Property)

        /// <summary>
        /// Gets or sets ZoomMode whether Zooming should Invoke in Single Click or Double
        /// Click.
        /// </summary>
        /// <remarks>
        /// Default ZoomMode is DoubleClick
        /// </remarks>
        /// <value>
        /// ZoomingMode
        /// </value>
        public ZoomingMode ZoomMode
        {
            get { return (ZoomingMode)GetValue(ZoomModeProperty); }
            set { SetValue(ZoomModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomMode.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies ZoomMode Dependency Property
        /// </summary>
        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register("ZoomMode", typeof(ZoomingMode), typeof(MapControl), new PropertyMetadata(ZoomingMode.DoubleClick));

        #endregion

        #region EnableHoverEffects (DependencyProperty)

        /// <summary>
        /// Gets / sets if the control can enable hover effects over paths.
        /// </summary>
        /// <remarks>
        /// Default Value is False
        /// </remarks>
        /// <value>
        /// 	<see langword="true"/> if; <see langword="false"/> Otherwise
        /// </value>
        public bool EnableHoverEffects
        {
            get { return (bool)GetValue(EnableHoverEffectsProperty); }
            set { SetValue(EnableHoverEffectsProperty, value); }
        }

        /// <summary>
        /// Identifies EnableHoverEffectes Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableHoverEffectsProperty = DependencyProperty.Register("EnableHoverEffects", typeof(bool), typeof(MapControl), new PropertyMetadata(false));

        #endregion

        #region ShapeHoverFill (DependencyProperty)

        /// <summary>
        /// Gets / sets the fill property of the path when it is in hover state.
        /// </summary>
        public Brush ShapeHoverFill
        {
            get { return (Brush)GetValue(ShapeHoverFillProperty); }
            set { SetValue(ShapeHoverFillProperty, value); }
        }

        /// <summary>
        ///  Identifies ShapeHoverFill Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShapeHoverFillProperty = DependencyProperty.Register("ShapeHoverFill", typeof(Brush), typeof(MapControl), new PropertyMetadata(Brushes.AliceBlue));

        #endregion

        #region ShapeHoverStroke (DependencyProperty)

        /// <summary>
        /// Gets / sets the stroke property of the path when it is in hover state.
        /// </summary>
        public Brush ShapeHoverStroke
        {
            get
            {
                return (Brush)GetValue(ShapeHoverStrokeProperty);
            }

            set
            {
                SetValue(ShapeHoverStrokeProperty, value);
            }
        }

        /// <summary>
        ///  Identifies ShapeHoverStroke Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShapeHoverStrokeProperty = DependencyProperty.Register("ShapeHoverStroke", typeof(Brush), typeof(MapControl), new PropertyMetadata(Brushes.Black));

        #endregion

        #region ColorPalette (Dependency Property)

        /// <summary>
        /// Gets or sets ColorPalette for MapControl.
        /// </summary>
        /// <remarks>
        /// By dafault no colorpalette is set
        /// </remarks>
        /// <value>
        /// ColorPalettes
        /// </value>
        public ColorPalettes ColorPalette
        {
            get { return (ColorPalettes)GetValue(ColorPaletteProperty); }
            set { SetValue(ColorPaletteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorPalette.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies ColorPalette Dependency Property
        /// </summary>
        public static readonly DependencyProperty ColorPaletteProperty =
            DependencyProperty.Register("ColorPalette", typeof(ColorPalettes), typeof(MapControl), new PropertyMetadata(ColorPalettes.None));

        #endregion

        #region CustomMapColorPalette

        /// <summary>
        /// Gets or sets CustomColorPalette.
        /// </summary>
        /// <value>
        /// Collection of MapColorPalette
        /// </value>
        public ObservableCollection<MapColorPallette> CustomMapColorPalette
        {
            get { return (ObservableCollection<MapColorPallette>)GetValue(CustomMapColorPaletteProperty); }
            set { SetValue(CustomMapColorPaletteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentMapPalette.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies CustomMapColorPalette Dependency Property
        /// </summary>
        public static readonly DependencyProperty CustomMapColorPaletteProperty =
            DependencyProperty.Register("CustomMapColorPalette", typeof(ObservableCollection<MapColorPallette>), typeof(MapControl), new PropertyMetadata(new ObservableCollection<MapColorPallette>()));

        #endregion

        #region ShapeHoverStrokeThickness

        /// <summary>
        /// Gets or sets ShapeHoverStrokeThickness.
        /// </summary>
        /// <remarks>
        /// Default Value is 0.3
        /// </remarks>
        /// <value>
        /// Double
        /// </value>
        public double ShapeHoverStrokeThickness
        {
            get
            {
                return (double)GetValue(ShapeHoverStrokeThicknessProperty);
            }

            set
            {
                SetValue(ShapeHoverStrokeThicknessProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ShapeHoverStrokeThickness.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies ShapeHoverStrokeThickness Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShapeHoverStrokeThicknessProperty =
            DependencyProperty.Register("ShapeHoverStrokeThickness", typeof(double), typeof(MapControl), new PropertyMetadata(0.3d));

        #endregion

        #region ColorPaletteMode (Dependency Property)

        /// <summary>
        /// Gets or sets Mode of Color Palette.
        /// </summary>
        /// <remarks>
        /// Default Value is Sequential
        /// </remarks>
        /// <value>
        /// ColorPaletteMode
        /// </value>
        public ColorPaletteMode ColorPaletteMode
        {
            get { return (ColorPaletteMode)GetValue(ColorPaletteModeProperty); }
            set { SetValue(ColorPaletteModeProperty, value); }
        }

        /// <summary>
        ///  Identifies ColorPaletteMode Dependency Property
        /// </summary>
        public static readonly DependencyProperty ColorPaletteModeProperty =
            DependencyProperty.Register("ColorPaletteMode", typeof(ColorPaletteMode), typeof(MapControl), new PropertyMetadata(ColorPaletteMode.Sequential));

        #endregion

        #region EnableColorPalette (Dependency Property)

        /// <summary>
        /// Gets or sets EnableColorPalette Property
        /// </summary>
        /// <value><c>true</c> if [enable color palette]; otherwise, <c>false</c>.</value>
        public bool EnableColorPalette
        {
            get { return (bool)GetValue(EnableColorPaletteProperty); }
            set { SetValue(EnableColorPaletteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableColorPalette.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies EnableColorPalette Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableColorPaletteProperty =
           DependencyProperty.Register("EnableColorPalette", typeof(bool), typeof(MapControl), new PropertyMetadata(false));

        #endregion

        #region SelectedItems code

        /// <summary>
        /// Gets / sets the SelectedItem property.
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        ///  Identifies SelectedItem Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(MapControl), new PropertyMetadata(null));

        #endregion

        #region MouseOverItem (DependencyProperty)

        /// <summary>
        /// Gets / sets the mouse hover item when the path is hovered upon.
        /// </summary>
        public object MouseOverItem
        {
            get { return (object)GetValue(MouseOverItemProperty); }
            private set { SetValue(MouseOverItemProperty, value); }
        }

        /// <summary>
        ///  Identifies MouseHoverItem Dependency Property
        /// </summary>
        public static readonly DependencyProperty MouseOverItemProperty = DependencyProperty.Register("MouseOverItem", typeof(object), typeof(MapControl), new PropertyMetadata(null));

        #endregion

        #region EnableLayerTransitionEffects Dependency Property

        /// <summary>
        /// Gets or sets a value indicating whetherto enable LayerTransition Effects or not.
        /// </summary>
        /// <remarks>
        /// Default Value is true
        /// </remarks>
        /// <value>
        /// 	<see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool EnableLayerTransitionEffects
        {
            get { return (bool)GetValue(EnableLayerTransitionEffectsProperty); }
            set { SetValue(EnableLayerTransitionEffectsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableLayerTransitionEffects.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies EnableLayerTransitionEffects Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableLayerTransitionEffectsProperty =
            DependencyProperty.Register("EnableLayerTransitionEffects", typeof(bool), typeof(MapControl), new PropertyMetadata(true));

        #endregion

        #region ShapeFill (DependencyProperty)

        /// <summary>
        /// Gets / sets the Path.Fill for shapes that are created. This property will be used as the default for all kinds of shapes.
        /// </summary>
        public Brush ShapeFill
        {
            get { return (Brush)GetValue(ShapeFillProperty); }
            set { SetValue(ShapeFillProperty, value); }
        }

        /// <summary>
        ///  Identifies ShapeFill Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShapeFillProperty = DependencyProperty.Register("ShapeFill", typeof(Brush), typeof(MapControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 250, 225, 200))));

        #endregion

        #region ShapeStroke (DependencyProperty)

        /// <summary>
        /// Gets / sets the Path.Stroke for shapes that are created. This property will be used as the default for all kinds of strokes.
        /// </summary>
        public Brush ShapeStroke
        {
            get { return (Brush)GetValue(ShapeStrokeProperty); }
            set { SetValue(ShapeStrokeProperty, value); }
        }

        /// <summary>
        ///  Identifies ShapeStroke Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShapeStrokeProperty = DependencyProperty.Register("ShapeStroke", typeof(Brush), typeof(MapControl), new PropertyMetadata(Brushes.Black));

        #endregion

        #region ShapeStrokeThickness (DependencyProperty)

        /// <summary>
        /// Gets / sets the thickess for the Path when the shapes are created.
        /// </summary>
        public double ShapeStrokeThickness
        {
            get { return (double)GetValue(ShapeStrokeThicknessProperty); }
            set { SetValue(ShapeStrokeThicknessProperty, value); }
        }

        /// <summary>
        ///  Identifies ShapeStrokeThickness Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShapeStrokeThicknessProperty = DependencyProperty.Register("ShapeStrokeThickness", typeof(double), typeof(MapControl), new PropertyMetadata(0.5d));

        #endregion

        #region PolygonFill (DependencyProperty)

        /// <summary>
        /// Gets / sets the Path.Fill for Polygons. If this is not specified the default value from ShapeFill will be used.
        /// </summary>
        public Brush PolygonFill
        {
            get { return (Brush)GetValue(PolygonFillProperty); }
            set { SetValue(PolygonFillProperty, value); }
        }

        /// <summary>
        ///  Identifies PolygonFill Dependency Property
        /// </summary>
        public static readonly DependencyProperty PolygonFillProperty = DependencyProperty.Register("PolygonFill", typeof(Brush), typeof(MapControl), new PropertyMetadata(null));

        #endregion

        #region PolygonStroke (DependencyProperty)

        /// <summary>
        /// Gets / sets the Path.Stroke for Polygons. If this is not specified the default value from ShapeStroke will be used.
        /// </summary>
        public Brush PolygonStroke
        {
            get { return (Brush)GetValue(PolygonStrokeProperty); }
            set { SetValue(PolygonStrokeProperty, value); }
        }

        /// <summary>
        ///  Identifies PolygonStroke Dependency Property
        /// </summary>
        public static readonly DependencyProperty PolygonStrokeProperty = DependencyProperty.Register("PolygonStroke", typeof(Brush), typeof(MapControl), new PropertyMetadata(null));

        #endregion

        #region SymbolPaletteVisibility

        /// <summary>
        /// Gets or sets Visibility of SymbolColorPalette.
        /// </summary>
        /// <remarks>
        /// ByDefault SymbolPalette is Collapsed
        /// </remarks>
        /// <value>
        /// Visibility
        /// </value>
        public Visibility SymbolPaletteVisibility
        {
            get { return (Visibility)GetValue(SymbolPaletteVisibilityProperty); }
            set { SetValue(SymbolPaletteVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolPaletteVisibility.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies SymbolPaletteVisibility Dependency Property
        /// </summary>
        public static readonly DependencyProperty SymbolPaletteVisibilityProperty =
            DependencyProperty.Register("SymbolPaletteVisibility", typeof(Visibility), typeof(MapControl), new PropertyMetadata(Visibility.Collapsed));

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether enable virtualization.
        /// </summary>
        /// <value><c>true</c> if [enable virtualization]; otherwise, <c>false</c>.</value>
        public bool EnableVirtualization
        {
            get { return (bool)GetValue(EnableVirtualizationProperty); }
            set { SetValue(EnableVirtualizationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableVirtualization.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies EnableVirtualization Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableVirtualizationProperty =
            DependencyProperty.Register("EnableVirtualization", typeof(bool), typeof(MapControl), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableVirtualizationChanged)));

        private static void OnEnableVirtualizationChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            MapControl mapControl = dobj as MapControl;
            if (args.NewValue != null)
            {
                if (!(bool)args.NewValue)
                {
                    mapControl.ReloadChildrens();
                }
            }
        }



#if WPF

        /// <summary>
        /// Gets or sets EnableImageCapture Property
        /// </summary>
        /// <value><c>true</c> if [enable color palette]; otherwise, <c>false</c>.</value>
        public bool EnableImageCapture
        {
            get { return (bool)GetValue(EnableImageCaptureProperty); }
            set { SetValue(EnableImageCaptureProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableImageCapture.  This enables animation, styling, binding, etc...
        /// <summary>
        ///  Identifies EnableImageCapture Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableImageCaptureProperty =
            DependencyProperty.Register("EnableImageCapture", typeof(bool), typeof(MapControl), new UIPropertyMetadata(false));



#endif

        /// <summary>
        /// Gets or sets SymbolPalette.
        /// </summary>
        public SymbolPalette SymbolPalette { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.MapControl"/> class.
        /// </summary>
        public MapControl()
        {

            this.Legends = new ObservableCollection<Legend>();
            this.LegendPanel = new LegendPanel();
            this.DefaultStyleKey = typeof(MapControl);
            this.panResetCommand = new PanResetCommand(this);
            this.zoomResetCommand = new ZoomResetCommand(this);
            this.panCommand = new PanCommand(this);
            this.zoomInCommand = new ZoomInCommand(this);
            this.zoomOutCommand = new ZoomOutCommand(this);
            this.resetCommand = new ShapeFileResetCommand(this);
            this.refreshCommand = new ShapeFileRefreshCommand(this);
            this.selectedItemNullCommand = new ShapeFileSelectedItemNullCommand(this);
            if (this.Layers == null)
            {
                this.Layers = new Layers();
            }
            this.SymbolPalette = new SymbolPalette();
            this.Legends.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Legends_CollectionChanged);
            this.KeyDown += new KeyEventHandler(MapControl_KeyDown);

        }

        void MapControl_KeyDown(object sender, KeyEventArgs e)
        {
#if WPF
            if (this.selectionadorner != null)
            {
                if ((this.LayeredContent as ShapeFileLayer).selectionPanel.Children.Contains(this.selectionadorner))
                {
                    (this.LayeredContent as ShapeFileLayer).selectionPanel.Children.Remove(this.selectionadorner);
                    this.isMouseDown = false;
                }
            }
#endif
        }

        #endregion

        /// <summary>
        /// Invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"></see>.
        /// </summary>
        public override void OnApplyTemplate()
        {
#if WPF
            // if (EnvironmentTestMapsWPF.IsSecurityGranted)
            //{
            //    EnvironmentTestMapsWPF.StartValidateLicense(typeof(Syncfusion.Windows.Controls.Map.MapControl));
            //}
#endif
            this.mapScrollViewer = this.GetTemplateChild("PART_MapScrollViewer") as ScrollViewer;
            this.navigationControl = this.GetTemplateChild("PART_NavigationControl") as NavigationControl;
            this.coordinateLabel = this.GetTemplateChild("PART_coordinatesText") as TextBlock;
            this.tempZoomFactor = this.ZoomFactor;
            this.SetBinding(MapControl.ZoomLevelProperty, new Binding { Source = this, Path = new PropertyPath("ZoomFactor"), Converter = new ZoomLevelConverter(), Mode = BindingMode.TwoWay, ConverterParameter = this });
            Grid mapGrid = this.GetTemplateChild("PART_MapMainGrid") as Grid;
            mapGrid.Children.Add(this.SymbolPalette);
            Grid.SetColumn(this.SymbolPalette, 0);
            (mapGrid.Children[0] as Grid).Children.Add(this.LegendPanel);
            this.LegendPanel.SetBinding(Panel.VisibilityProperty, new Binding { Source = this, Path = new PropertyPath("LegendVisibility"), Mode = BindingMode.TwoWay });
            this.SymbolPalette.SetBinding(Syncfusion.Windows.Controls.Map.SymbolPalette.VisibilityProperty, new Binding { Source = this, Path = new PropertyPath("SymbolPaletteVisibility"), Mode = BindingMode.TwoWay });
#if SILVERLIGHT
            symbolPreviewPopup = this.GetTemplateChild("PART_SymbolPreviewPopup") as Popup;
#endif
            ResourceDictionary resourceDic = new ResourceDictionary();
#if WPF
            resourceDic.Source = new Uri("/Syncfusion.Maps.Wpf;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);

#endif
#if SILVERLIGHT
            resourceDic.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);

#endif
            base.OnApplyTemplate();
            this.opacityAnimator1 = resourceDic["myStoryboard"] as Storyboard;
            this.opacityAnimator2 = resourceDic["myStoryboard1"] as Storyboard;
            this.MouseMove += new MouseEventHandler(MapControl_MouseMove);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(MapControl_MouseLeftButtonUp);
            this.Loaded += new RoutedEventHandler(MapControl_Loaded);
            this.ChangeLatitudeAndLogitudeType(this.LatitudeLongitudeType);
        }

        void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.ChangeLegendPosition();
        }

        void Legends_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                if (this.Legends != null)
                {
                    foreach (Legend Legend in e.OldItems)
                    {
                        this.LegendPanel.Children.Remove(Legend);
                    }
                }
            }
            if (e.NewItems != null)
            {
                if (this.Legends != null)
                {
                    foreach (Legend Legend in e.NewItems)
                    {
                        this.LegendPanel.Children.Add(Legend);
                    }
                }
            }
            this.ChangeLegendPosition();
        }

        /// <summary>
        ///  Call a Method when KeyUp
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Input.KeyEventArgs"/> that
        /// contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (this.isLabelEdited)
            {
                this.isLabelEdited = false;
                return;
            }
            if (this.LayeredContent is ShapeFileLayer)
            {
                if (e.Key == Key.Escape)
                {
                    if ((this.LayeredContent as ShapeFileLayer).SelectedMapLabel != null)
                    {
                        (this.LayeredContent as ShapeFileLayer).SelectedMapLabel.IsSelected = false;
                        (this.LayeredContent as ShapeFileLayer).SelectedMapLabel = null;
                    }
                    if ((this.LayeredContent as ShapeFileLayer).SelectedMapSymbol != null)
                    {
                        (this.LayeredContent as ShapeFileLayer).SelectedMapSymbol.IsSelected = false;
                        (this.LayeredContent as ShapeFileLayer).SelectedMapSymbol = null;
                    }
                }
            }
        }

        void MapControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isMouseUp = true;
            this.isMouseDown = false;
            this.Cursor = Cursors.Arrow;
#if SILVERLIGHT
            this.symbolPreviewPopup.IsOpen = false;
#endif

#if WPF
            if (this.selectionadorner != null)
            {
                if (selectedArea != Rect.Empty)
                {
                    if (this.isImageCaptured)
                    {
                        var savefileDialog = new SaveFileDialog();
                        savefileDialog.Filter = "Tiff Image (*.tiff)|*.tiff|Gif Image (*.gif)|*.gif|Jpeg Image (*.jpg)|*.jpg|Png Image (*.png)|*.png|Bmp Image (*.bmp)|*.bmp";
                        if (savefileDialog.ShowDialog() == true)
                        {
                            string extension = savefileDialog.SafeFileName;
                            if (extension.ToLower(CultureInfo.InvariantCulture).Contains(".tiff") || extension.ToLower(CultureInfo.InvariantCulture).Contains(".gif") || extension.ToLower(CultureInfo.InvariantCulture).Contains(".png") || extension.ToLower(CultureInfo.InvariantCulture).Contains(".jpg") || extension.ToLower(CultureInfo.InvariantCulture).Contains(".bmp"))
                            {
                                this.bitmapCoder = this.GetEncoders(savefileDialog.SafeFileName);
                                this.Save(savefileDialog.OpenFile(), this.selectedArea);

                            }

                        }
                        else
                        {
                            if (this.selectionadorner != null)
                            {
                                (this.LayeredContent as ShapeFileLayer).selectionPanel.Children.Remove(selectionadorner);
                            }
                        }

                        this.selectedArea = Rect.Empty;
                    }
                }
            }
#endif

            if (this.LayeredContent is ShapeFileLayer)
            {
                if ((this.LayeredContent as ShapeFileLayer).IsPan && !(this.LayeredContent as ShapeFileLayer).panHandle)
                {
                    Point p = (this.LayeredContent as ShapeFileLayer).PointToLatitudeLongitude(CurrentPoint);
                    (this.LayeredContent as ShapeFileLayer).IsPan = false;
#if WPF
                    (this.LayeredContent as ShapeFileLayer).RaisePannedEvent(this.LatLonPoint.X, this.LatLonPoint.Y, this.panMode);

#endif
#if SILVERLIGHT
                    PanEventArgs panEvtArgs = new PanEventArgs(this.LatLonPoint.X,this.LatLonPoint.Y, this.panMode);
                    (this.LayeredContent as ShapeFileLayer).OnPanned(this.LayeredContent, panEvtArgs);
#endif
                }
            }
            else if (this.LayeredContent is ImageryLayer)
            {
                if ((this.LayeredContent as ImageryLayer).IsPan && !(this.LayeredContent as ImageryLayer).panHandle)
                {
                    Point p = (this.LayeredContent as ImageryLayer).PointToLatitudeLongitude(CurrentPoint);
                    (this.LayeredContent as ImageryLayer).IsPan = false;
#if WPF
                    (this.LayeredContent as ImageryLayer).RaisePannedEvent(this.LatLonPoint.Y, this.LatLonPoint.X, this.panMode);

#endif
#if SILVERLIGHT
                    PanEventArgs panEvtArgs = new PanEventArgs(this.LatLonPoint.Y,this.LatLonPoint.X, this.panMode);
                    (this.LayeredContent as ImageryLayer).OnPanned(this.LayeredContent, panEvtArgs);
#endif
                }
            }
            this.ReleaseMouseCapture();

        }

        void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                this.CurrentPoint = e.GetPosition(this);
                if (this.SelectedSymbolItem != null)
                {
                    if (!(MapControl.FindParent<SymbolPalette>(e.OriginalSource as UIElement) is SymbolPalette))
                    {
                        if (!this.isMouseUp && this.isMouseDown)
                        {
#if SILVERLIGHT
                        this.symbolPreviewPopup.IsOpen = true;
                        this.symbolPreviewPopup.HorizontalOffset = (e.GetPosition(this).X - (this.SelectedSymbolItem as FrameworkElement).ActualWidth - 5);
                        this.symbolPreviewPopup.VerticalOffset = e.GetPosition(this).Y;

#endif
                        }
                        else
                        {
#if WPF
                            this.Cursor = Cursors.Arrow;
#endif
                        }
                    }
                }
                else
                {
                    if (this.isMouseDown)
                    {
#if WPF
                        if (this.EnableImageCapture)
                        {
                            if (selectionadorner != null)
                            {
                                (this.LayeredContent as ShapeFileLayer).selectionPanel.Children.Remove(selectionadorner);
                            }

                            selectedArea = new Rect(this.startPoint, e.GetPosition(this));
                            selectionadorner = new Border();
                            selectionadorner.IsHitTestVisible = false;
                            selectionadorner.BorderBrush = new SolidColorBrush(Colors.Black);
                            selectionadorner.BorderThickness = new Thickness(2);
                            Canvas.SetLeft(selectionadorner, selectedArea.X);
                            Canvas.SetTop(selectionadorner, selectedArea.Y);
                            selectionadorner.Width = selectedArea.Width;
                            selectionadorner.Height = selectedArea.Height;
                            selectionadorner.Opacity = .5;
                            this.Cursor = Cursors.Cross;
                            (this.LayeredContent as ShapeFileLayer).selectionPanel.Children.Add(selectionadorner);
                            this.isImageCaptured = true;
                        }
                        else
                        {
                            this.isImageCaptured = false;
#endif
                            this.CaptureMouse();
                            if (!(this.LayeredContent as ShapeFileLayer).IsDynamicCreatePath)
                            {
                                if (this.EnablePan)
                                {
                                    this.Cursor = Cursors.Hand;
                                    (this.LayeredContent as MapLayer).Pan(this.CurrentPoint.X - this.startPoint.X + this.panTranslatePoint.X, this.CurrentPoint.Y - this.startPoint.Y + this.panTranslatePoint.Y);
                                    (this.LayeredContent as MapLayer).IsPan = true;

                                }
                            }
#if WPF
                        }
#endif
                    }
                }
            }
        }

        private void ReloadChildrens()
        {
            if ((this.LayeredContent as ShapeFileLayer) != null)
            {
                if ((this.LayeredContent as ShapeFileLayer).canvas != null)
                {
                    foreach (MapShapes shp in (this.LayeredContent as ShapeFileLayer).ShapeCollection)
                    {
                        if ((this.LayeredContent as ShapeFileLayer).canvas.Children.Contains(shp))
                        {
                            (this.LayeredContent as ShapeFileLayer).canvas.Children.Remove(shp);
                        }
                    }
                    foreach (MapShapes shp in (this.LayeredContent as ShapeFileLayer).ShapeCollection)
                    {
                        if (!(this.LayeredContent as ShapeFileLayer).canvas.Children.Contains(shp))
                        {
                            (this.LayeredContent as ShapeFileLayer).canvas.Children.Add(shp);
                        }
                    }
                }
            }
        }

        private void MapLayer_ZoomedOut(object sender, ZoomEventArgs args)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                if (this.GetLayeredContent() != null)
                {
                    if (this.LayeredContent != ((this.GetLayeredContent() as List<ShapeFileLayer>).ElementAt(0) as UIElement))
                    {
                        if (this.EnableLayerTransitionEffects)
                        {
                            this.DoAnimation();
                        }
                        else
                        {
                            this.RemoveAnimation();
                        }

                        this.LayeredContent = ((this.GetLayeredContent() as List<ShapeFileLayer>).ElementAt(0) as UIElement);
                    }
                }
            }

        }

        private void MapLayer_ZoomedIn(object sender, ZoomEventArgs args)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                if (this.GetLayeredContent() != null)
                {
                    if (this.LayeredContent != ((this.GetLayeredContent() as List<ShapeFileLayer>).ElementAt(0) as UIElement))
                    {
                        if (this.EnableLayerTransitionEffects)
                        {
                            this.DoAnimation();
                        }
                        else
                        {
                            this.RemoveAnimation();
                        }
                        this.LayeredContent = ((this.GetLayeredContent() as List<ShapeFileLayer>).ElementAt(0) as UIElement);
                    }
                }
            }
        }



        #region HelperMethods


        private void ChangeLatitudeAndLogitudeType(LatitudeLongitudeType type)
        {
            if (type == LatitudeLongitudeType.Decimal)
            {
                LatLonConverter = new LatitudeLongitudeToTextConverter();
            }
            else
            {
                LatLonConverter = new LatitudeLongitudeDegreeToTextConverter();
            }
            if (this.coordinateLabel != null)
            {
                this.coordinateLabel.SetBinding(TextBlock.TextProperty, new Binding { Source = this, Path = new PropertyPath("LatLonPoint"), Converter = this.LatLonConverter });
            }
        }

        private void ChangeLegendPosition()
        {
            var position = this.LegendPosition;
            var height = from child in this.Legends
                         select child.DesiredSize.Width;
            var maxWidth = 0.0d;
            if (height.ToList().Count() > 0)
            {
                maxWidth = height.Max();
            }
            this.LegendPanel.Margin = new Thickness(0);
            switch (position)
            {
                case LegendPosition.BottomLeft:
                    this.LegendPanel.HorizontalAlignment = HorizontalAlignment.Left;
                    this.LegendPanel.VerticalAlignment = VerticalAlignment.Bottom;
                    this.LegendPanel.Margin = new Thickness(0, 0, 0, (this.Legends.Count) * 25);
                    break;
                case LegendPosition.BottomRight:
                    this.LegendPanel.HorizontalAlignment = HorizontalAlignment.Right;
                    this.LegendPanel.VerticalAlignment = VerticalAlignment.Bottom;
                    this.LegendPanel.Margin = new Thickness(0, 0, maxWidth, (this.Legends.Count + 2) * 25);
                    break;
                case LegendPosition.BottomMiddle:
                    this.LegendPanel.HorizontalAlignment = HorizontalAlignment.Center;
                    this.LegendPanel.VerticalAlignment = VerticalAlignment.Bottom;
                    this.LegendPanel.Margin = new Thickness(0, 0, 0, (this.Legends.Count + 2) * 25);
                    break;
                case LegendPosition.TopLeft:
                    this.LegendPanel.HorizontalAlignment = HorizontalAlignment.Left;
                    this.LegendPanel.VerticalAlignment = VerticalAlignment.Top;

                    break;
                case LegendPosition.TopRight:
                    this.LegendPanel.HorizontalAlignment = HorizontalAlignment.Right;
                    this.LegendPanel.VerticalAlignment = VerticalAlignment.Top;
                    this.LegendPanel.Margin = new Thickness(0, 0, maxWidth, 0);
                    break;
                case LegendPosition.TopMiddle:
                    this.LegendPanel.HorizontalAlignment = HorizontalAlignment.Center;
                    this.LegendPanel.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case LegendPosition.RightMiddle:
                    this.LegendPanel.HorizontalAlignment = HorizontalAlignment.Right;
                    this.LegendPanel.VerticalAlignment = VerticalAlignment.Center;
                    this.LegendPanel.Margin = new Thickness(0, 0, maxWidth, 0);
                    this.LegendPanel.RenderTransform = new TranslateTransform { X = 0, Y = -((this.LegendPanel.Children.Count * 25) / 2) };

                    break;
                case LegendPosition.LeftMiddle:
                    this.LegendPanel.HorizontalAlignment = HorizontalAlignment.Left;
                    this.LegendPanel.VerticalAlignment = VerticalAlignment.Center;
                    this.LegendPanel.RenderTransform = new TranslateTransform { X = 0, Y = -((this.LegendPanel.Children.Count * 25) / 2) };
                    break;

            }
        }

        private void WireZoomEvents(MapLayer maplayer)
        {
            maplayer.ZoomedIn += new ZoomEventHandler(MapLayer_ZoomedIn);
            maplayer.ZoomedOut += new ZoomEventHandler(MapLayer_ZoomedOut);
        }

        private void UnWireZoomEvents(MapLayer maplayer)
        {
            maplayer.ZoomedIn -= new ZoomEventHandler(MapLayer_ZoomedIn);
            maplayer.ZoomedOut -= new ZoomEventHandler(MapLayer_ZoomedOut);
        }

        private object GetLayeredContent()
        {
            List<object> lst = new List<object>();
            foreach (object obj in this.Layers.Items)
            {
                lst.Add(obj);
            }
            var orderedLayers = from layers in lst
                                where (layers as ShapeFileLayer).TranslateZoomFactor <= this.ZoomFactor
                                select (layers as ShapeFileLayer).TranslateZoomFactor;
            if (orderedLayers.Count() > 0)
            {
                List<ShapeFileLayer> desiredLayer = (from layer in lst
                                                     where (layer as ShapeFileLayer).TranslateZoomFactor == orderedLayers.Max()
                                                     select (layer as ShapeFileLayer)).ToList();
                return desiredLayer;
            }

            return null;
        }

        private void DoAnimation()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
#if WPF
                this.opacityAnimator1.SetValue(Storyboard.TargetProperty, (this.LayeredContent as ShapeFileLayer));
                this.opacityAnimator1.Begin();
                this.opacityAnimator2.SetValue(Storyboard.TargetProperty, ((this.GetLayeredContent() as List<ShapeFileLayer>).ElementAt(0) as ShapeFileLayer));
                this.opacityAnimator2.Begin();
#endif
#if SILVERLIGHT
                this.opacityAnimator1.Stop();
                this.opacityAnimator2.Stop();
                Storyboard.SetTarget(opacityAnimator1, this.LayeredContent as ShapeFileLayer);
                this.opacityAnimator1.Begin();
                Storyboard.SetTarget(opacityAnimator2, ((this.GetLayeredContent() as List<ShapeFileLayer>).ElementAt(0) as ShapeFileLayer));
                this.opacityAnimator2.Begin();
#endif
            }
        }

        private void RemoveAnimation()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
#if WPF
                this.opacityAnimator1.SetValue(Storyboard.TargetProperty, (this.LayeredContent as ShapeFileLayer));
                this.opacityAnimator1.Stop();
                this.opacityAnimator2.SetValue(Storyboard.TargetProperty, ((this.GetLayeredContent() as List<ShapeFileLayer>).ElementAt(0) as ShapeFileLayer));
                this.opacityAnimator2.Stop();
#endif
#if SILVERLIGHT
                this.opacityAnimator1.Stop();
                this.opacityAnimator2.Stop();
                Storyboard.SetTarget(opacityAnimator1, this.LayeredContent as ShapeFileLayer);
                this.opacityAnimator1.Stop();
                Storyboard.SetTarget(opacityAnimator2, ((this.GetLayeredContent() as List<ShapeFileLayer>).ElementAt(0) as ShapeFileLayer));
                this.opacityAnimator2.Stop();
#endif
            }
        }

        internal static T FindParent<T>(UIElement control) where T : UIElement
        {
            if (control != null)
            {
                UIElement p = VisualTreeHelper.GetParent(control) as UIElement;
                if (p != null)
                {
                    if (p is T)
                        return p as T;
                    else
                        return MapControl.FindParent<T>(p);
                }
            }
            return null;
        }

        internal object Clone(object obj, int isPath)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            object cloneObj = obj.GetType().GetConstructors()[0].Invoke(null);
            foreach (PropertyInfo property in properties)
            {
                if (!property.Name.Contains("Name"))
                {
                    object value = property.GetValue(obj, null);
                    if (value != null)
                    {
                        try
                        {
                            if (IsPresentationFrameworkCollection(value.GetType()))
                            {
                                object collection = property.GetValue(obj, null);
                                int count = (int)collection.GetType().GetProperty("Count").GetValue(collection, null);
                                for (int i = 0; i < count; i++)
                                {
                                    object child = collection.GetType().GetProperty("Item").GetValue(collection, new object[] { i });
                                    object cloneChild = this.Clone(child, 0);
                                    object cloneCollection = property.GetValue(cloneObj, null);
                                    collection.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, cloneCollection, new object[] { cloneChild });
                                }
                            }

                            if (value is UIElement)
                            {
                                object obj2 = property.PropertyType.GetConstructors()[0].Invoke(null);
                                Clone(obj2, 0);
                                property.SetValue(cloneObj, obj2, null);
                            }
                            else if (property.CanWrite)
                            {
                                if (property.ToString().Contains("Data") && isPath > 0)
                                {
                                    PathGeometry geo = value as PathGeometry;
                                    PathGeometry pathgeo = (PathGeometry)this.Clone(geo, 0);
                                    property.SetValue(cloneObj, pathgeo, null);
                                }
                                else
                                {
                                    property.SetValue(cloneObj, value, null);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return cloneObj;
        }

        /// <summary>
        /// Determines whether [is presentation framework collection] [the specified type].
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <returns>
        /// <c>true</c> if [is presentation framework collection] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPresentationFrameworkCollection(Type type)
        {
            if (type == typeof(object))
            {
                return false;
            }

            if (type.Name.StartsWith("PresentationFrameworkCollection"))
            {
                return true;
            }

            return this.IsPresentationFrameworkCollection(type.BaseType);
        }

        /// <summary>
        /// Clones the Path object
        /// </summary>
        /// <param name="element">Element to be clone</param>
        /// <param name="obj">object that is cloned</param>
        /// <returns></returns>
        internal object ClonePath(object element, object obj)
        {
            PropertyInfo[] properties = element.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(element, null);
                if (value != null)
                {
                    try
                    {
                        if (this.IsPresentationFrameworkCollection(value.GetType()))
                        {
                            object collection = property.GetValue(element, null);
                            int count = (int)collection.GetType().GetProperty("Count").GetValue(collection, null);
                            for (int i = 0; i < count; i++)
                            {
                                object child = collection.GetType().GetProperty("Item").GetValue(collection, new object[] { i });
                                object cloneChild = Clone(child, 0);
                                object cloneCollection = property.GetValue(obj, null);
                                collection.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, cloneCollection, new object[] { cloneChild });
                            }
                        }

                        if (value is UIElement)
                        {
                            object obj2 = property.PropertyType.GetConstructors()[0].Invoke(null);
                            this.Clone(obj2, 0);
                            property.SetValue(element, obj2, null);
                        }
                        else if (property.CanWrite)
                        {
                            if (!property.ToString().Contains("Data"))
                            {
                                property.SetValue(obj, value, null);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return obj;
        }

        private void ChangeNaviagtionControlSliderValue()
        {
            if (this.navigationControl != null)
            {
                this.navigationControl.MinSliderValue = this.MinZoom / this.tempZoomFactor;
                this.navigationControl.MaxSliderValue = this.MaxZoom / this.tempZoomFactor;
            }
        }

        /// <summary>
        /// Saves the Current Layered Content
        /// </summary>
        public void Save()
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
#if WPF
                saveFileDialog.Filter = "XML File (*.xml)|*.xml |Tiff Image (*.tiff)|*.tiff|Gif Image (*.gif)|*.gif|Jpeg Image (*.jpg)|*.jpg|Png Image (*.png)|*.png|Bmp Image (*.bmp)|*.bmp";
#else
            saveFileDialog.Filter = "XML File (*.xml)|*.xml";
#endif

                if (saveFileDialog.ShowDialog() == true)
                {
                    if (saveFileDialog.SafeFileName != string.Empty)
                    {
                        string extension = saveFileDialog.SafeFileName;
                        if (extension.ToLower(CultureInfo.InvariantCulture).Contains(".xml"))
                        {
#if WPF
                            this.Save(saveFileDialog.FileName);
#else
                        this.Save(saveFileDialog.OpenFile(), SaveMode.Xml);
#endif
                        }
#if WPF

                        else if (extension.ToLower(CultureInfo.InvariantCulture).Contains(".tiff") || extension.ToLower(CultureInfo.InvariantCulture).Contains(".gif") || extension.ToLower(CultureInfo.InvariantCulture).Contains(".png") || extension.ToLower(CultureInfo.InvariantCulture).Contains(".jpg") || extension.ToLower(CultureInfo.InvariantCulture).Contains(".bmp"))
                        {
                            this.Save(saveFileDialog.FileName);
                        }
#endif

                        else
                        {
                            MessageBox.Show("Invalid File Formate");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save the Current LayeredContent with given file name
        /// </summary>
        /// <param name="saveFileStream">The Stream of the file which will be saved.</param>
        /// <param name="savemode">SaveMode</param>
        public void Save(Stream saveFileStream, SaveMode savemode)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                this.PreSave();
                if (savemode == SaveMode.Xml)
                {
                    this.SaveXml(saveFileStream);
                }
#if WPF
                else
                {
                    this.Save(saveFileStream, this.selectedArea);
                }
#endif
            }
        }

#if WPF
        /// <summary>
        ///  Save function which saves the map as given file name
        /// </summary>
        /// <param name="Filename">The name of the file, the map will be saved.</param>
        public void Save(string Filename)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                FileStream fs = new FileStream(Filename, FileMode.Create);
                if (Filename.ToLower(CultureInfo.InvariantCulture).Contains(".xml"))
                {
                    this.Save(fs, SaveMode.Xml);
                }

                else if (Filename.ToLower(CultureInfo.InvariantCulture).Contains(".tiff") || Filename.ToLower(CultureInfo.InvariantCulture).Contains(".gif") || Filename.ToLower(CultureInfo.InvariantCulture).Contains(".png") || Filename.ToLower(CultureInfo.InvariantCulture).Contains(".jpg") || Filename.ToLower(CultureInfo.InvariantCulture).Contains(".bmp"))
                {
                    this.bitmapCoder = this.GetEncoders(Filename);
                    this.Save(fs, SaveMode.Image);
                }
            }

        }

        /// <summary>
        /// Save function,which saves the given portion of a map as image.
        /// </summary>
        /// <param name="imageFilename">Name of the File</param>
        /// <param name="area">Portion which will be saved</param>
        public void Save(string imageFilename, Rect area)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                FileStream fs = new FileStream(imageFilename, FileMode.Append);
                if (imageFilename.ToLower(CultureInfo.InvariantCulture).Contains(".tiff") || imageFilename.ToLower(CultureInfo.InvariantCulture).Contains(".gif") || imageFilename.ToLower(CultureInfo.InvariantCulture).Contains(".png") || imageFilename.ToLower(CultureInfo.InvariantCulture).Contains(".jpg") || imageFilename.ToLower(CultureInfo.InvariantCulture).Contains(".bmp"))
                {
                    this.bitmapCoder = this.GetEncoders(imageFilename);
                    this.Save(fs, SaveMode.Image);
                }
                else
                {
                    MessageBox.Show("Invalid Image Format");
                }
            }
        }


#endif




#if WPF
        /// <summary>
        ///  This Method helps to save the Image stream
        /// </summary>
        /// <param name="imageStream">ImageStream</param>
        /// <param name="area">Rect</param>
        public void Save(Stream imageStream, Rect area)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                if (this.selectionadorner != null)
                {
                    (this.LayeredContent as ShapeFileLayer).selectionPanel.Children.Remove(selectionadorner);
                }
                RenderTargetBitmap renderTargetBitmap;
                if ((area.Height == 0 && area.Width == 0) || double.IsNegativeInfinity(area.Width) || double.IsNegativeInfinity(area.Height) || double.IsPositiveInfinity(area.Width) || double.IsPositiveInfinity(area.Height))
                {
                    area = new Rect(new Point(0, 0), new Size((this.LayeredContent as ShapeFileLayer).ActualWidth, (this.LayeredContent as ShapeFileLayer).ActualHeight));
                }
                renderTargetBitmap = new RenderTargetBitmap((int)(area.Width), (int)(area.Height), 96, 96, PixelFormats.Default);
                VisualBrush visualBrush = new VisualBrush(this.LayeredContent as ShapeFileLayer);
                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
                System.Windows.Shapes.Rectangle backgroundRect = new System.Windows.Shapes.Rectangle();

                backgroundRect.Fill = Brushes.White;
                backgroundRect.Arrange(new Rect(area.Size));
                visualBrush.ViewboxUnits = BrushMappingMode.Absolute;
                if (area.X == 0 && area.Y == 0)
                {
                    visualBrush.Viewbox = new Rect(area.X + (this.LayeredContent as ShapeFileLayer).Margin.Left, area.Y + (this.LayeredContent as ShapeFileLayer).Margin.Top, area.Width, area.Height);
                }
                else
                {
                    if (this.SymbolPaletteVisibility == Visibility.Visible)
                    {
                        visualBrush.Viewbox = new Rect(area.X - this.SymbolPalette.ActualWidth, area.Y, area.Width, area.Height);
                    }
                    else
                    {
                        visualBrush.Viewbox = new Rect(area.X, area.Y, area.Width, area.Height);
                    }
                }

                rect.Fill = visualBrush;
                rect.Stretch = Stretch.Fill;
                rect.Arrange(new Rect(area.Size));

                renderTargetBitmap.Render(backgroundRect);
                renderTargetBitmap.Render(rect);


                this.bitmapCoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                this.bitmapCoder.Save(imageStream);
                imageStream.Close();
            }

        }

        /// <summary>
        ///  The Function which returns the BitMapEncoder based on the Filename extension.
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns>BitMapEncoder</returns>
        internal BitmapEncoder GetEncoders(string Filename)
        {
            if (Filename.ToLower(CultureInfo.InvariantCulture).Contains(".tiff"))
            {
                return new TiffBitmapEncoder();
            }
            else if (Filename.ToLower(CultureInfo.InvariantCulture).Contains(".gif"))
            {
                return new GifBitmapEncoder();
            }
            else if (Filename.ToLower(CultureInfo.InvariantCulture).Contains(".png"))
            {
                return new PngBitmapEncoder();
            }
            else if (Filename.ToLower(CultureInfo.InvariantCulture).Contains(".jpg"))
            {
                return new JpegBitmapEncoder();
            }
            else if (Filename.ToLower(CultureInfo.InvariantCulture).Contains(".bmp"))
            {
                return new BmpBitmapEncoder();
            }
            else
            {
                return null;
            }

        }

#endif
        /// <summary>
        /// A function the save the Maps as xml docuemnt.
        /// </summary>
        /// <param name="stream"></param>
        /// <remarks>This fumction will save the given stream as xml docuement</remarks>
        internal void SaveXml(Stream stream)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                // Instance for the MapXamlWriter has been created.
                MapXamlWriter xamlWriter = new MapXamlWriter();
                string xaml = xamlWriter.WriteXaml(this.LayeredContent);
                StreamWriter str = new StreamWriter(stream);
                str.Write(xaml);
                str.Close();
                stream.Close();
            }
        }

        /// <summary>
        /// Load the ShapeLayer from the xml file.
        /// </summary>
        public void Load()
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                // Filter files in the File dialog box.
                openFileDialog.Filter = "XML File (*.xml)|*.xml";
                if (openFileDialog.ShowDialog() == true)
                {
#if SILVERLIGHT
                if (openFileDialog.File.Name != string.Empty)
                {
#else
                    //Check weather files are selected or not.
                    if (openFileDialog.FileName != string.Empty)
                    {
#endif
#if SILVERLIGHT
                    //Get the Extension of the file that has been opened from the Open Dialog box.
                    string extension = openFileDialog.File.Name;
#else
                        //Get the Extension of the file that has been opened from the Open Dialog box.
                        string extension = openFileDialog.FileName;
#endif
                        if (extension.ToLower(CultureInfo.InvariantCulture).Contains(".xml"))
                        {
                            MapControl.isXmlContent = false;
#if SILVERLIGHT
                        this.Load(openFileDialog.File.OpenRead());
#else
                            this.Load(openFileDialog.OpenFile());
#endif
                        }
                        else
                        {
                            MessageBox.Show("Invalid File Formate");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load the ShapeLayer from the xml file.
        /// </summary>
        /// <param name="openFileStream">The Stream of the File which will be loaded</param>
        /// <remarks></remarks>
        public void Load(Stream openFileStream)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                this.resetFlag = true;
                ShapeFileLayer newLayer;
                StreamReader reader = new StreamReader(openFileStream);
                string str = reader.ReadToEnd();
                if (this.LayeredContent is ShapeFileLayer)
                {
                    var tempuri = (this.LayeredContent as ShapeFileLayer).Uri;
                    // To add new elements in the Map, the existing elements in the map has been cleared.
                    // If xml file loaded with Uri property of the ShapeFileLayer do not clear thing.
                    if (!tempuri.ToLower(CultureInfo.InvariantCulture).Contains(".xml") || !MapControl.isXmlContent)
                    {
                        (this.LayeredContent as ShapeFileLayer).Uri = string.Empty;
                        (this.LayeredContent as ShapeFileLayer).canvas.Children.Clear();
                        (this.LayeredContent as ShapeFileLayer).drawingCanvas.Children.Clear();
                        (this.LayeredContent as ShapeFileLayer).mapElementCanvas.Children.Clear();
                        (this.LayeredContent as ShapeFileLayer).ShapeCollection.Clear();
                        (this.LayeredContent as ShapeFileLayer).LabelCollections.Clear();
                        (this.LayeredContent as ShapeFileLayer).SymbolCollection.Clear();
                        (this.LayeredContent as ShapeFileLayer).MapPathCollection.Clear();
                    }

#if WPF
                    // New Shapelayer Instance created from the Loaded xml file.
                    using (var xamlstr = new StringReader(str))
                    using (var xmlfile = XmlReader.Create(xamlstr))
                        newLayer = XamlReader.Load(xmlfile) as ShapeFileLayer;



#else
                // New Shapelayer Instance created from the Loaded xml file.
                newLayer = XamlReader.Load(str) as ShapeFileLayer;

#endif




                    // If Bounding boxes values are 0 or Layered Content loaded a xml file then Bounding box values has been set.
                    // To set the ShapeTransform, these values have been set.
                    if ((this.LayeredContent as ShapeFileLayer).BoundMaxX == 0 && (this.LayeredContent as ShapeFileLayer).BoundMaxY == 0 && (this.LayeredContent as ShapeFileLayer).BoundMinX == 0 && (this.LayeredContent as ShapeFileLayer).BoundMinY == 0)
                    {
                        (this.LayeredContent as ShapeFileLayer).BoundMaxX = newLayer.BoundMaxX;
                        (this.LayeredContent as ShapeFileLayer).BoundMaxY = newLayer.BoundMaxY;
                        (this.LayeredContent as ShapeFileLayer).BoundMinX = newLayer.BoundMinX;
                        (this.LayeredContent as ShapeFileLayer).BoundMinY = newLayer.BoundMinY;
                    }
                    // Reset the Shape Transform if Bounded values of ShapeFileLayer changed.
                    // While Load a Map with different sets Shapes, the ShapeTranxform has be refreshed. Else Latitude longitude values will be wrong.
                    if ((newLayer.BoundMaxX != this.GetRoundValue((this.LayeredContent as ShapeFileLayer).BoundMaxX) && newLayer.BoundMaxY != this.GetRoundValue((this.LayeredContent as ShapeFileLayer).BoundMaxY) && newLayer.BoundMinX != this.GetRoundValue((this.LayeredContent as ShapeFileLayer).BoundMinX) && newLayer.BoundMinY != this.GetRoundValue((this.LayeredContent as ShapeFileLayer).BoundMinY)))
                    {
                        // ShapeTransform has been reset.
                        (this.LayeredContent as ShapeFileLayer).ShapeTransform = (this.LayeredContent as ShapeFileLayer).SetScaleTransformGroup(newLayer.BoundMinX, newLayer.BoundMinY, newLayer.BoundMaxX, newLayer.BoundMaxY);
                    }

                    //Instead of Setting the new ShapeFileLayer as Layered content, the newLayers elements has been added in the layered content.
                    foreach (MapShapes shps in newLayer.ShapeCollection)
                    {
                        var path = this.CreatePath(shps.Shape.Data.ToString());
                        path.Fill = shps.Shape.Fill;
                        path.Stroke = shps.Shape.Stroke;
                        path.StrokeThickness = shps.Shape.StrokeThickness;
                        shps.Shape = path;
                        (this.LayeredContent as ShapeFileLayer).ShapeCollection.Add(shps);
                    }
                    foreach (MapPath mapPath in newLayer.MapPathCollection)
                    {
                        (this.LayeredContent as ShapeFileLayer).MapPathCollection.Add(mapPath);
                    }
                    foreach (MapLabel lbls in newLayer.LabelCollections)
                    {
                        (this.LayeredContent as ShapeFileLayer).LabelCollections.Add(lbls);
                    }
                    foreach (MapSymbols symbs in newLayer.SymbolCollection)
                    {
                        (this.LayeredContent as ShapeFileLayer).SymbolCollection.Add(symbs);
                    }
                    // Reset the Transforms 

                    (this.LayeredContent as ShapeFileLayer).PanTransform.X = 0;
                    (this.LayeredContent as ShapeFileLayer).PanTransform.Y = 0;
                    (this.LayeredContent as ShapeFileLayer).ZoomTransform.ScaleX = 1;
                    (this.LayeredContent as ShapeFileLayer).ZoomTransform.ScaleX = 1;
                    (this.LayeredContent as ShapeFileLayer).ZoomLevel = 1;
                }
                this.resetFlag = false;
                reader.Close();
                openFileStream.Close();
            }
        }

#if WPF
        /// <summary>
        /// Load the map from the given location
        /// </summary>
        /// <param name="location">Location of the File.</param>
        public void Load(string location)
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                if (location.ToLower(CultureInfo.InvariantCulture).Contains(".xml"))
                {
                    FileStream fs = new FileStream(location, FileMode.Open);
                    this.Load(fs);
                    fs.Close();
                }
            }

        }
#endif
        /// <summary>
        /// A function which Rounds the given decimal value
        /// </summary>
        /// <param name="val"></param>
        private double GetRoundValue(double val)
        {
            return Math.Round(val, 12);
        }
        /// <summary>
        /// Actions that should perform before saving the Shape File Layer
        /// </summary>
        /// <remarks>Fuction will clear all the selected items in the Maps.</remarks>
        internal void PreSave()
        {
            if (this.LayeredContent is ShapeFileLayer)
            {
                // Clear selected Items of the Layered content.
                (this.LayeredContent as ShapeFileLayer).MouseOverItem = null;
                (this.LayeredContent as ShapeFileLayer).SelectedMapLabel = null;
                (this.LayeredContent as ShapeFileLayer).SelectedMapSymbol = null;
                (this.LayeredContent as ShapeFileLayer).SelectedShape = null;
                (this.LayeredContent as ShapeFileLayer).SelectedMapPath = null;
                (this.LayeredContent as ShapeFileLayer).SelectedItems.Clear();
                (this.LayeredContent as ShapeFileLayer).Uri = string.Empty;

            }
        }


        /// <summary>
        /// Private function that Creates the Path from the given geomentry values
        /// </summary>
        /// <remarks>
        /// This function written for implement virtualization.To properly get the Shapes
        /// Data bounded values.With those bounded values virtualization has been
        /// imaplementd.If this function not implmented, Shape's Data Bounds(Rect)
        /// Value cannot be properly get
        /// </remarks>
        /// <param name="stringPath">String</param>
        /// <returns>
        /// Path
        /// </returns>
        private System.Windows.Shapes.Path CreatePath(string stringPath)
        {
            var resultpath = new System.Windows.Shapes.Path();
            var geomenty = new PathGeometry();
            // Split the given geomenty value as segments of strings
            string[] segmentStr = stringPath.ToString().Split('M');
            for (int j = 1; j < segmentStr.Length; j++)
            {
                var pathFigure = new PathFigure();
                if (segmentStr[j] != string.Empty)
                {
                    var modstr = segmentStr[j].Replace('L', ' ');
                    string[] str = modstr.ToString().Split(' ');
                    for (int i = 0; i <= str.Length - 1; i++)
                    {
                        if (i == 0)
                        {
                            pathFigure.StartPoint = this.StringToPoint(str[i]);
                        }
                        else
                        {
                            var linesegment = new LineSegment();
                            if (str[i] != string.Empty)
                            {
                                linesegment.Point = this.StringToPoint(str[i]);
                                pathFigure.Segments.Add(linesegment);
                            }
                        }
                    }
                }
                if (pathFigure.Segments.Count > 1)
                {
                    geomenty.Figures.Add(pathFigure);
                }
            }
            //View Transform has been applied for geomenty.To get correct Data Bounds(Rect).
            geomenty.Transform = (this.LayeredContent as ShapeFileLayer).ViewTransform;
            resultpath.Data = geomenty;
            resultpath.Tag = stringPath.ToString();
            return resultpath;
        }

        /// <summary>
        /// A function that converts the given string as points.
        /// </summary>
        /// <param name="pointString"></param>
        /// <returns>Point</returns>
        /// <remarks>The parameter for that should be like "xxxx,xxxxx"</remarks>
        private Point StringToPoint(string pointString)
        {
            string[] str = pointString.Split(',');
            return new Point(double.Parse(str[0]), double.Parse(str[1]));
        }

        #endregion

        #region Commands

        private ICommand resetCommand;

        /// <summary>
        /// Gets ResetCommand.
        /// </summary>
        public ICommand ResetCommand
        {
            get
            {
                return this.resetCommand;
            }
        }

        private ICommand refreshCommand;

        /// <summary>
        /// Gets Refresh Command.
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                return this.refreshCommand;
            }
        }
        private ICommand selectedItemNullCommand;

        /// <summary>
        /// Gets SelectedItemNullCommand.
        /// </summary>
        public ICommand SelectedItemNullCommand
        {
            get
            {
                return this.selectedItemNullCommand;
            }
        }


        private ICommand panResetCommand;

        /// <summary>
        /// Gets PanResetCommand.
        /// </summary>
        public ICommand PanResetCommand
        {
            get
            {
                return this.panResetCommand;
            }
        }

        private ICommand zoomResetCommand;

        /// <summary>
        /// Gets ZoomResetCommand.
        /// </summary>
        public ICommand ZoomResetCommand
        {
            get
            {
                return this.zoomResetCommand;
            }
        }

        private ICommand zoomInCommand;

        /// <summary>
        /// Gets ZoomInCommand.
        /// </summary>
        public ICommand ZoomInCommand
        {
            get
            {
                return this.zoomInCommand;
            }
        }

        private ICommand zoomOutCommand;

        /// <summary>
        /// Gets ZoomOutCommand.
        /// </summary>
        /// <value>
        /// ICommand
        /// </value>
        public ICommand ZoomOutCommand
        {
            get
            {
                return this.zoomOutCommand;
            }
        }

        private ICommand panCommand;

        /// <summary>
        /// Gets PanCommand.
        /// </summary>
        /// <value>
        /// ICommand
        /// </value>
        public ICommand PanCommand
        {
            get
            {
                return this.panCommand;
            }
        }

        #endregion
    }
    #region Helper Classes

    #region Layer Class



    /// <summary>
    ///  Layers contains shapes of the map
    /// </summary>
    public class Layers : ItemsControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.Layers"/> class.
        /// </summary>
        public Layers()
        {
        }
    }

    #endregion

    #region MapTrustTest class
#if WPF
    /// <summary>
    ///  MapTrustTest Class allows  Permission of MapControl
    /// </summary>
    [ContentProperty("xaml")]
    public class MapTrustTest : MarkupExtension
    {
        internal static bool IsTrusted
        {
            get;
            private set;
        }

        internal string xaml
        {
            get;
            set;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.MapTrustTest"/> class.
        /// </summary>
        public MapTrustTest()
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.MapTrustTest"/> class.
        /// </summary>
        static MapTrustTest()
        {
            try
            {
                var status = PermissionState.Unrestricted;
                var permission = new UIPermission(status);
                permission.Assert();
                IsTrusted = true;
            }
            catch { }

        }
        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the
        /// value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide
        /// services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            object result = null;
            if (IsTrusted)
            {
                try
                {

                    using (var xamlstr = new StringReader(xaml))
                    using (var xmlfile = XmlReader.Create(xamlstr))
                        result = XamlReader.Load(xmlfile);

                }
                catch { }
            }
            return result;
        }

    }
#endif
    #endregion

    #endregion

}
