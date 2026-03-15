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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using Syncfusion.Maps.IO;
    using System.Diagnostics;
    using System.Resources;
    using System.Windows.Documents;
    using System.Windows.Controls.Primitives;
    using System.Xml.Serialization;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    ///  ShapeFileLayer is layer which contains Map elements like shapes
    /// </summary>
    [ContentProperty("AttributesChildren")]
    public class ShapeFileLayer : MapLayer, IDisposable
    {
        #region Private Fields

        private List<ShapeFileRecord> tempSkippedRecords = new List<ShapeFileRecord>();
        private DispatcherTimer zoomTimer = new DispatcherTimer();
        private TimeSpan zoomTime = new TimeSpan();
        private ShapeFileData currentFileData;
        private System.IO.Stream currentxmlStream;
        private ToolTip localAttributeTooltip;
        private ObservableCollection<object> selectedItems;
        private double tempZoomfactor;
#if SILVERLIGHT
        private Brush tempStrokeBrush;
        private double tempStrokeThickness;
        private BindingExpression fillExp;
        private BindingExpression strokeExp;
        private BindingExpression strokeThicknessExp;
        private Brush tempPathfill;
         private Path tempPath;
#endif
        private Point canvasCenter = new Point();
        private bool isload = false;
        private ShapeFileData data;
        private bool isTemplateLoaded = false;
        private ObservableCollection<string> FillColors = new ObservableCollection<string>();
        private ObservableCollection<string> StrokeColors = new ObservableCollection<string>();
        private bool isColorBinded = false;
        private bool isColorPaletteCreated = false;
        private Point TempClickPoint;
        private Point templatlon;
        private Size loadedCanvasSize;
        private int dynamicpathpointscount = 0;
        private int recordIndex = 0;
        private int skippedRecordIndex = 0;

        #endregion

        #region RenderDelegates

        // Render Records
        private delegate void RenderDelegate(ShapeFileData data);
        private RenderDelegate RecordRenderDelegate;

        private delegate void SkippedRecordsDelegate();
        private SkippedRecordsDelegate RenderSkippedRecordsDelegate;

        #endregion

        #region Internal Fields
        internal int shapeCount = 0;
        internal bool isRefreshed = false;
        internal string ClickedItem;
        internal ShapeFileCanvas drawingCanvas;
        internal MapElementPanel mapElementCanvas;
        internal ShapeFilePanel canvas;
        internal MapControl mapControl;
        internal Grid mainGrid;
        internal bool isXmlFileLoaded = false;
       

#if WPF
        internal SelectionPanel selectionPanel;
#endif

        #endregion

        #region cTor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ShapeFileLayer"/> class.
        /// </summary>
        public ShapeFileLayer()
        {
            this.DefaultStyleKey = typeof(ShapeFileLayer);
            this.SkippedRecords = new List<ShapeFileRecord>();
            // initialize zoom transform
            this.ZoomTransform = new ScaleTransform() { ScaleX = 1, ScaleY = 1 };
            //Intialize Pan Transform
            this.PanTransform = new TranslateTransform() { X = 0, Y = 0 };
            this.ViewTransform = new TransformGroup();
            this.ViewTransform.Children.Add(this.ZoomTransform);
            this.ViewTransform.Children.Add(this.PanTransform);

            // initialize the drawing canvas
            this.drawingCanvas = new ShapeFileCanvas();
#if WPF
            this.selectionPanel = new SelectionPanel();
#endif
            this.mapElementCanvas = new MapElementPanel();
            this.CurrentMapColorPalette = new ObservableCollection<MapColorPallette>();
            this.selectedItems = new ObservableCollection<object>();
            this.SymbolCollection = new ObservableCollection<MapSymbols>();
            this.LabelCollections = new ObservableCollection<MapLabel>();
            this.ShapeCollection = new ObservableCollection<MapShapes>();
            this.MapPathCollection = new ObservableCollection<MapPath>();
            if (this.MapPathCollection != null)
            {
                this.MapPathCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(MapPathCollection_CollectionChanged);
            }
            if (this.SymbolCollection != null)
            {
                this.SymbolCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SymbolCollection_CollectionChanged);
            }
            if (this.LabelCollections != null)
            {
                this.LabelCollections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(LabelCollections_CollectionChanged);
            }
            if (this.ShapeCollection != null)
            {
                this.ShapeCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ShapeCollection_CollectionChanged);
            }
            this.zoomTimer.Tick += new EventHandler(zoomTimer_Tick);
        }

        void zoomTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan d = DateTime.Now.TimeOfDay - this.zoomTime;
            if (d.TotalSeconds >= 1.0d)
            {
                this.zoomTimer.Stop();
                this.canvas.IsInSuspend = false;
                this.tempSkippedRecords.Clear();
#if WPF
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, this.RenderSkippedRecordsDelegate);
#else
                this.Dispatcher.BeginInvoke(this.RenderSkippedRecordsDelegate);
#endif
                this.canvas.InvalidateMeasure();

            }
        }


        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets SkippedRecord. It is a Collection of ShapeFileRecord
        /// </summary>
        public List<ShapeFileRecord> SkippedRecords
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets Minimum X value of the Bound.
        /// </summary>
        public double BoundMinX
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets Minimum Y value of the bound.
        /// </summary>
        /// <value>
        /// BoundMinY
        /// </value>
        public double BoundMinY
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets Maximum X value if the bound.
        /// </summary>
        public double BoundMaxX
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets Maximum Y value of the bound.
        /// </summary>
        public double BoundMaxY
        {
            get;
            set;
        }

        #region LabelCollections

        /// <summary>
        /// Gets or sets Collection of Labels.
        /// </summary>
        public ObservableCollection<MapLabel> LabelCollections
        {
            get { return (ObservableCollection<MapLabel>)GetValue(LabelCollectionsProperty); }
            set { SetValue(LabelCollectionsProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelCollections.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelCollectionsProperty =
            DependencyProperty.Register("LabelCollections", typeof(ObservableCollection<MapLabel>), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region ShapeCollections


        /// <summary>
        /// Gets or sets Collection of Shapes.
        /// </summary>
        public ObservableCollection<MapShapes> ShapeCollection
        {
            get { return (ObservableCollection<MapShapes>)GetValue(ShapeCollectionProperty); }
            set { SetValue(ShapeCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShapeCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShapeCollectionProperty =
            DependencyProperty.Register("ShapeCollection", typeof(ObservableCollection<MapShapes>), typeof(ShapeFileLayer), new PropertyMetadata(null));


        #endregion

        #region EnableLabel(Dependency Property)

        /// <summary>
        /// Gets or sets a value indicating whether to enable label or not.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool EnableLabel
        {
            get { return (bool)GetValue(EnableLabelProperty); }
            set { SetValue(EnableLabelProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableLabel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableLabelProperty =
            DependencyProperty.Register("EnableLabel", typeof(bool), typeof(ShapeFileLayer), new PropertyMetadata(false));

        #endregion

        #region SelectedShape(Dependency Property)

        /// <summary>
        /// Gets or sets SelectedShape of the Map.
        /// </summary>
        public object SelectedShape
        {
            get { return (object)GetValue(SelectedShapeProperty); }
            set { SetValue(SelectedShapeProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedShape.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedShapeProperty =
            DependencyProperty.Register("SelectedShape", typeof(object), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region IsDynamicCreatePath

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dynamic create path.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is dynamic create path; otherwise, <c>false</c>.
        /// </value>
        public bool IsDynamicCreatePath
        {
            get { return (bool)GetValue(IsDynamicCreatePathProperty); }
            set { SetValue(IsDynamicCreatePathProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDynamicCreatePath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDynamicCreatePathProperty =
            DependencyProperty.Register("IsDynamicCreatePath", typeof(bool), typeof(ShapeFileLayer), new PropertyMetadata(false, new PropertyChangedCallback(OnIsDynamicCreatePathChanged)));

        private static void OnIsDynamicCreatePathChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ShapeFileLayer ShapeFileLayer = obj as ShapeFileLayer;
            ShapeFileLayer.flag_start_point = 0;

        }

        #endregion

        #region MapPathCollection(Dependency Property)

        /// <summary>
        /// Gets or sets the MapPathCollection.
        /// </summary>
        /// <value>ObservableCollection of MapPath</value>
        /// <remarks>ShapeFileLayer property which sets or gets the MapPathCollections.</remarks>
        public ObservableCollection<MapPath> MapPathCollection
        {
            get { return (ObservableCollection<MapPath>)GetValue(MapPathCollectionProperty); }
            set { SetValue(MapPathCollectionProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for MapPathCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MapPathCollectionProperty =
            DependencyProperty.Register("MapPathCollection", typeof(ObservableCollection<MapPath>), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region AllowLabelIntersection(Dependency Property)



        /// <summary>
        /// Gets or sets a value indicating whether label Interaction is needed or not .
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool AllowLabelIntersection
        {
            get { return (bool)GetValue(AllowLabelIntersectionProperty); }
            set { SetValue(AllowLabelIntersectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowLabelIntersection.  This enables animation, styling, binding, etc...
        /// <summary>
        ///   Using a DependencyProperty as the backing store for AllowLabelIntersection. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowLabelIntersectionProperty =
            DependencyProperty.Register("AllowLabelIntersection", typeof(bool), typeof(ShapeFileLayer), new PropertyMetadata(false));



        #endregion

        #region SelectedMapLabel(Dependency Property)

        /// <summary>
        /// Gets or sets SelectedMapLabel.
        /// </summary>
        /// <value>
        /// MapLabel
        /// </value>
        public MapLabel SelectedMapLabel
        {
            get { return (MapLabel)GetValue(SelectedMapLabelProperty); }
            set { SetValue(SelectedMapLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedMapLabel.  This enables animation, styling, binding, etc...
        /// <summary>
        ///   Using a DependencyProperty as the backing store for SelectedMapLabel.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedMapLabelProperty =
            DependencyProperty.Register("SelectedMapLabel", typeof(MapLabel), typeof(ShapeFileLayer), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedMapLabelChanged)));

        private static void OnSelectedMapLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapLayer mapLayer = d as MapLayer;
            if (e.NewValue != null)
            {
#if WPF
                mapLayer.RaiseLabelSelectedEvent(e.NewValue);
#else
                SelectionEventArgs args = new SelectionEventArgs(e.NewValue);
                mapLayer.OnLabelSelected(mapLayer, args);
#endif
            }
            if (e.OldValue != null)
            {
#if WPF
                mapLayer.RaiseLabelUnSelectedEvent(e.OldValue);
#else
                SelectionEventArgs args = new SelectionEventArgs(e.OldValue);
                mapLayer.OnLabelUnSelected(mapLayer, args);
#endif
            }

        }



        #endregion

        #region SelectedMapSymbol(Dependency Property)

        /// <summary>
        /// Gets or sets SelectedMapSymbol.
        /// </summary>
        /// <value>
        /// MapSymbols
        /// </value>
        public MapSymbols SelectedMapSymbol
        {
            get { return (MapSymbols)GetValue(SelectedMapSymbolProperty); }
            set { SetValue(SelectedMapSymbolProperty, value); }
        }

        
        /// <summary>
        /// // Using a DependencyProperty as the backing store for SelectedMapSymbol.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedMapSymbolProperty =
            DependencyProperty.Register("SelectedMapSymbol", typeof(MapSymbols), typeof(ShapeFileLayer), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedSymbolChanged)));

        private static void OnSelectedSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapLayer mapLayer = d as MapLayer;
            if (e.NewValue != null)
            {
#if WPF
                mapLayer.RaiseSymbolSelectedEvent(e.NewValue);
#else
                SelectionEventArgs args = new SelectionEventArgs(e.NewValue);
                mapLayer.OnSymbolSelected(mapLayer, args);
#endif
            }
            if (e.OldValue != null)
            {
#if WPF
                mapLayer.RaiseSymbolUnSelectedEvent(e.OldValue);
#else
                SelectionEventArgs args = new SelectionEventArgs(e.OldValue);
                mapLayer.OnSymbolUnSelected(mapLayer, args);
#endif
            }
        }

        #endregion

        #region Symbol Collection (Dpendency Property)


        /// <summary>
        /// Gets or sets SymbolCollection. It is a collection of MapSymbols
        /// </summary>
        public ObservableCollection<MapSymbols> SymbolCollection
        {
            get { return (ObservableCollection<MapSymbols>)GetValue(SymbolCollectionProperty); }
            set { SetValue(SymbolCollectionProperty, value); }
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolCollectionProperty =
            DependencyProperty.Register("SymbolCollection", typeof(ObservableCollection<MapSymbols>), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region TranslateZoomFactor

        /// <summary>
        /// Gets or sets TranslateZoomFactor.
        /// </summary>
        public double TranslateZoomFactor
        {
            get { return (double)GetValue(TranslateZoomFactorProperty); }
            set { SetValue(TranslateZoomFactorProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for TranslateZoomFactor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TranslateZoomFactorProperty =
            DependencyProperty.Register("TranslateZoomFactor", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(0d));

        #endregion

        #region LabelSource (Dependency Property)

        /// <summary>
        /// Gets or sets the label source.
        /// </summary>
        /// <value>The label source.</value>
        public IEnumerable LabelSource
        {
            get { return (IEnumerable)GetValue(LabelSourceProperty); }
            set { SetValue(LabelSourceProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelSourceProperty =
            DependencyProperty.Register("LabelSource", typeof(IEnumerable), typeof(ShapeFileLayer), new PropertyMetadata(null, new PropertyChangedCallback(OnLabelSourceChanged)));

        private static void OnLabelSourceChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            ShapeFileLayer ShapeFileLayer = dobj as ShapeFileLayer;
            ShapeFileLayer.PopulateLabels();
        }

        #endregion

        #region (SymbolSource)

        /// <summary>
        /// Gets or sets the symbol sourece.
        /// </summary>
        /// <value>The symbol sourece.</value>
        public IEnumerable SymbolSource
        {
            get { return (IEnumerable)GetValue(SymbolSourceProperty); }
            set { SetValue(SymbolSourceProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolSourece.  This enables animation, styling, binding, etc...
        /// </summary>
        
        public static readonly DependencyProperty SymbolSourceProperty =
            DependencyProperty.Register("SymbolSource", typeof(IEnumerable), typeof(ShapeFileLayer), new PropertyMetadata(null, new PropertyChangedCallback(OnSymbolSourceChanged)));


        private static void OnSymbolSourceChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs args)
        {
            ShapeFileLayer ShapeFileLayer = dobj as ShapeFileLayer;
            if (args.NewValue != null)
            {
                ShapeFileLayer.PopulateSymbols();
            }
        }


        #endregion

        #region PathSource(Dependency Property)



        /// <summary>
        /// Gets or sets the path source.
        /// </summary>
        /// <value>The path source.</value>
        public IEnumerable PathSource
        {
            get { return (IEnumerable)GetValue(PathSourceProperty); }
            set { SetValue(PathSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PathSource.  This enables animation, styling, binding, etc...
         /// </summary>
        public static readonly DependencyProperty PathSourceProperty =
            DependencyProperty.Register("PathSource", typeof(IEnumerable), typeof(ShapeFileLayer), new PropertyMetadata(null, new PropertyChangedCallback(OnPathSourceChanged)));

        private static void OnPathSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeFileLayer ShapeFileLayer = d as ShapeFileLayer;
            if (e.NewValue != null)
            {
                ShapeFileLayer.PopulatePath();
            }
        }

        #endregion

        #region CurrentMapColorPalette

        /// <summary>
        /// Gets or sets CurrentMapPalette. It is a collection of MapColorPalette
        /// </summary>
        public ObservableCollection<MapColorPallette> CurrentMapColorPalette
        {
            get { return (ObservableCollection<MapColorPallette>)GetValue(CurrentMapColorPaletteProperty); }
            set { SetValue(CurrentMapColorPaletteProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentMapPalette.  This enables animation, styling, binding, etc...
        ///</summary>
        public static readonly DependencyProperty CurrentMapColorPaletteProperty =
            DependencyProperty.Register("CurrentMapColorPalette", typeof(ObservableCollection<MapColorPallette>), typeof(ShapeFileLayer), new PropertyMetadata(null, new PropertyChangedCallback(OnMapPaletteChanged)));

        private static void OnMapPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeFileLayer shapeLayer = d as ShapeFileLayer;
            if (shapeLayer.ColorPalette != ColorPalettes.CustomColorPalette)
            {

                shapeLayer.FillShapes();
            }
            else
            {
                shapeLayer.isColorBinded = false;
            }

        }
        #endregion

        #region LatLonConverter (Dependency Property)

        /// <summary>
        /// Gets or sets LatLonConverter. It Converts the Latitude Longtitude points
        /// </summary>
        public IValueConverter LatLonConverter
        {
            get { return (IValueConverter)GetValue(LatLonConverterProperty); }
            set { SetValue(LatLonConverterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LanLanConverter.  This enables animation, styling, binding, etc...
        /// </summary>
        
        public static readonly DependencyProperty LatLonConverterProperty =
            DependencyProperty.Register("LatLonConverter", typeof(IValueConverter), typeof(ShapeFileLayer), new PropertyMetadata(new LatitudeLongitudeToTextConverter()));

        #endregion

        #region ShowLatLonPoint

        /// <summary>
        /// Gets or sets a value indicating whether to show Latitude and Lontitude points or
        /// not .
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool ShowLatLonPoints
        {
            get { return (bool)GetValue(ShowLatLonPointsProperty); }
            set { SetValue(ShowLatLonPointsProperty, value); }
        }
        /// <summary>
        ///Using a DependencyProperty as the backing store for ShowLatLonPoints.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowLatLonPointsProperty =
            DependencyProperty.Register("ShowLatLonPoints", typeof(bool), typeof(ShapeFileLayer), new PropertyMetadata(true));

        #endregion

        #region ColorPalette (Dependency Property)

        /// <summary>
        /// Gets or sets ColorPalette for the map Shapes.
        /// </summary>
        /// <value>
        /// ColorPaletts
        /// </value>
        public ColorPalettes ColorPalette
        {
            get { return (ColorPalettes)GetValue(ColorPaletteProperty); }
            set { SetValue(ColorPaletteProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for ColorPalette.  This enables animation, styling, binding, etc...
         /// </summary>
        public static readonly DependencyProperty ColorPaletteProperty =
            DependencyProperty.Register("ColorPalette", typeof(ColorPalettes), typeof(ShapeFileLayer), new PropertyMetadata(ColorPalettes.None, new PropertyChangedCallback(OnColorPaletteChanged)));

        /// <summary>
        ///  Calls when color palette is changed
        /// </summary>
        /// <param name="d">ShapeFileLayer</param>
        /// <param name="e">An <see cref="T:System.Windows.DependencyPropertyChangedEventArgs"/> that contains the
        /// event data.</param>
        public static void OnColorPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeFileLayer shapeControl = d as ShapeFileLayer;
#if !WPF
            shapeControl.tempPath = null;
#endif
            if (((ColorPalettes)e.OldValue == ColorPalettes.CustomColorPalette) || ((ColorPalettes)e.NewValue == ColorPalettes.CustomColorPalette))
            {
                shapeControl.isColorBinded = false;
                shapeControl.isColorPaletteCreated = false;
                shapeControl.CurrentMapColorPalette.Clear();
            }
            shapeControl.ChangeColorPalette();
        }

        #endregion

        #region CustomMapColorPalette

        /// <summary>
        /// Gets or sets CustomMapColorPalette. It is a collection of MapColorPalette
        /// </summary>
        public ObservableCollection<MapColorPallette> CustomMapColorPalette
        {
            get { return (ObservableCollection<MapColorPallette>)GetValue(CustomMapColorPaletteProperty); }
            set { SetValue(CustomMapColorPaletteProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentMapPalette.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CustomMapColorPaletteProperty =
            DependencyProperty.Register("CustomMapColorPalette", typeof(ObservableCollection<MapColorPallette>), typeof(ShapeFileLayer), new PropertyMetadata(new ObservableCollection<MapColorPallette>()));

        #endregion

        #region ColorPaletteMode (Dependency Property)

        /// <summary>
        /// Gets or sets Mode of the ColorPalette.
        /// </summary>
        public ColorPaletteMode ColorPaletteMode
        {
            get { return (ColorPaletteMode)GetValue(ColorPaletteModeProperty); }
            set { SetValue(ColorPaletteModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ColorPaletteMode.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ColorPaletteModeProperty =
            DependencyProperty.Register("ColorPaletteMode", typeof(ColorPaletteMode), typeof(ShapeFileLayer), new PropertyMetadata(ColorPaletteMode.Sequential, new PropertyChangedCallback(OnColorPaletteModeChanged)));

        private static void OnColorPaletteModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeFileLayer shapeControl = d as ShapeFileLayer;
            if (shapeControl.EnableColorPalette)
            {
                shapeControl.ChangeColorPalette();
            }
        }

        #endregion

        #region EnableColorPalette (Dependency Property)

        /// <summary>
        /// Gets or sets a value indicating whether to Enable ColorPalette or not.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool EnableColorPalette
        {
            get
            {
                return (bool)GetValue(EnableColorPaletteProperty);
            }

            set
            {
                SetValue(EnableColorPaletteProperty, value);
            }
        }
        ///<summary>
        /// Using a DependencyProperty as the backing store for EnableColorPalette.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableColorPaletteProperty =
            DependencyProperty.Register("EnableColorPalette", typeof(bool), typeof(ShapeFileLayer), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableColorPaletteChanged)));

        private static void OnEnableColorPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeFileLayer shapefilecontrol = d as ShapeFileLayer;
#if !WPF
            shapefilecontrol.tempPath = null;
#endif
            if ((bool)e.NewValue)
            {
                shapefilecontrol.isColorBinded = false;
                shapefilecontrol.isColorPaletteCreated = false;
                shapefilecontrol.CurrentMapColorPalette.Clear();
                shapefilecontrol.ChangeColorPalette();
            }
            else
            {
                shapefilecontrol.ResetFill();
            }
        }

        #endregion

        #region ShowTooltipsForAttributes (DependencyProperty)

        /// <summary>
        /// Gets / sets to show tooltips for ShapeFile attributes.
        /// </summary>
        public bool ShowTooltipsForAttributes
        {
            get { return (bool)GetValue(ShowTooltipsForAttributesProperty); }
            set { SetValue(ShowTooltipsForAttributesProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ShowTooltip.  This enables
        /// animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowTooltipsForAttributesProperty = DependencyProperty.Register("ShowTooltipsForAttributes", typeof(bool), typeof(ShapeFileLayer), new PropertyMetadata(true));

        #endregion

        #region AttributeTooltip (DependencyProperty)

        /// <summary>
        /// Gets / sets the custom tooltip for ShapeFile attributes
        /// </summary>
        public ToolTip AttributesTooltip
        {
            get { return (ToolTip)GetValue(AttributeTooltipProperty); }
            set { SetValue(AttributeTooltipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AttributeTooltip.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AttributeTooltipProperty = DependencyProperty.Register("AttributeTooltip", typeof(ToolTip), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region AttributesTooltipConverter (DependencyProperty)

        /// <summary>
        /// Gets / sets the tooltip converter for attributes. Use this property if the <see cref="AttributesTooltip"/> property is not used.
        /// </summary>
        public IValueConverter AttributesTooltipConverter
        {
            get { return (IValueConverter)GetValue(AttributesTooltipConverterProperty); }
            set { SetValue(AttributesTooltipConverterProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for AttributeTooltip
        /// Converter. This enables animation, styling, binding, etc
        /// </summary>
        public static readonly DependencyProperty AttributesTooltipConverterProperty = DependencyProperty.Register("AttributesTooltipConverter", typeof(IValueConverter), typeof(ShapeFileLayer), new PropertyMetadata(null));

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
        ///  Using a DependencyProperty as the backing store for ShapeFill. This enables
        /// animation, styling, binding, etc
        /// </summary>
        public static readonly DependencyProperty ShapeFillProperty = DependencyProperty.Register("ShapeFill", typeof(Brush), typeof(ShapeFileLayer), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 250, 225, 200))));

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
        ///  Using a DependencyProperty as the backing store for ShapeStroke.  This enables
        /// animation, styling, binding, etc
        /// </summary>
        public static readonly DependencyProperty ShapeStrokeProperty = DependencyProperty.Register("ShapeStroke", typeof(Brush), typeof(ShapeFileLayer), new PropertyMetadata(Brushes.Black));

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
        ///  Using a DependencyProperty as the backing store for ShapeStrokeThickness. This
        /// enables animation, styling, binding, etc
        /// </summary>
        public static readonly DependencyProperty ShapeStrokeThicknessProperty = DependencyProperty.Register("ShapeStrokeThickness", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(0.5d, new PropertyChangedCallback(OnShapeStrokeThickessProperty)));

        private static void OnShapeStrokeThickessProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeFileLayer layer = d as ShapeFileLayer;
            if (layer.ViewTransform != null)
            {
                layer.TempShapeStrokeThickness = (double)e.NewValue / layer.ZoomFactor;
            }
        }
        #endregion

        #region TempShapeStrokeThicknessProperty



        internal double TempShapeStrokeThickness
        {
            get { return (double)GetValue(TempShapeStrokeThicknessProperty); }
            set { SetValue(TempShapeStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TempShapeStrokeThickness.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TempShapeStrokeThicknessProperty =
            DependencyProperty.Register("TempShapeStrokeThickness", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(0.5d));



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
        ///  Using a DependencyProperty as the backing store for PolygonFill. This enables
        /// animation, styling, binding, etc
        /// </summary>
        public static readonly DependencyProperty PolygonFillProperty = DependencyProperty.Register("PolygonFill", typeof(Brush), typeof(ShapeFileLayer), new PropertyMetadata(null));

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
        ///  Using a DependencyProperty as the backing store for PolygonStroke. This enables
        /// animation, styling, binding, etc
        /// </summary>
        public static readonly DependencyProperty PolygonStrokeProperty = DependencyProperty.Register("PolygonStroke", typeof(Brush), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region EnableHoverEffects (DependencyProperty)

        /// <summary>
        /// Gets / sets if the control can enable hover effects over paths.
        /// </summary>
        public bool EnableHoverEffects
        {
            get { return (bool)GetValue(EnableHoverEffectsProperty); }
            set { SetValue(EnableHoverEffectsProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for EnableHoverEffects. This
        /// enables animation, styling, binding, etc
        /// </summary>
        public static readonly DependencyProperty EnableHoverEffectsProperty = DependencyProperty.Register("EnableHoverEffects", typeof(bool), typeof(ShapeFileLayer), new PropertyMetadata(false));

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
        ///  Using a DependencyProperty as the backing store for ShapeHoverFill.  This
        /// enables animation, styling, binding, etc
        /// </summary>
        public static readonly DependencyProperty ShapeHoverFillProperty = DependencyProperty.Register("ShapeHoverFill", typeof(Brush), typeof(ShapeFileLayer), new PropertyMetadata(Brushes.AliceBlue));

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
        ///  Using a DependencyProperty as the backing store for ShapeHoverStroke. This
        /// enables animation, styling, binding, etc
        /// </summary>
        public static readonly DependencyProperty ShapeHoverStrokeProperty = DependencyProperty.Register("ShapeHoverStroke", typeof(Brush), typeof(ShapeFileLayer), new PropertyMetadata(Brushes.Black));

        #endregion

        #region ShapeHoverStrokeThickness

        /// <summary>
        /// Gets / sets the thickness property of the path when it is in hover state.
        /// </summary>
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
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShapeHoverStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShapeHoverStrokeThicknessProperty =
            DependencyProperty.Register("ShapeHoverStrokeThickness", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(0.3d));

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
        ///  Using a DependencyProperty as the backing store for SelectedItem.  This enables
        /// animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(ShapeFileLayer), new PropertyMetadata(null));

        /// <summary>
        /// Gets / sets the Collection of SelectedItem property.
        /// </summary>

        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return this.selectedItems;
            }
        }

        /// <summary>
        ///  This Method set the selected item as null.
        /// </summary>
        public void SetSelectedItemAsNull()
        {
            this.SetSelectedItem(null);
        }



#if WPF
        /// <summary>
        /// Occurs when Selection is changed.
        /// </summary>
        [Category("Behavior")]
        public event SelectionChangedEventHandler SelectionChanged
        {
            add
            {
                this.AddHandler(ShapeFileLayer.SelectionChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(ShapeFileLayer.SelectionChangedEvent, value);
            }
        }

        /// <summary>
        ///  This is a Registerd SelectionChangedEvent
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(ShapeFileLayer));

        /// <summary>
        ///  Occured when Selection is changed
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Controls.SelectionChangedEventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            RaiseEvent(e);
        }

        internal void RaiseSelectionChanged(List<object> removedItems, List<object> addedItems)
        {
            var e = new SelectionChangedEventArgs(ShapeFileLayer.SelectionChangedEvent, removedItems, addedItems);
            this.OnSelectionChanged(e);
        }
#else
        /// <summary>
        /// Occurs when Selection is changed.
        /// </summary>
        [Category("Behavior")]
       
        public event SelectionChangedEventHandler SelectionChanged;

        internal void RaiseSelectionChanged(List<object> removedItems, List<object> addedItems)
        {
            var e = new SelectionChangedEventArgs(removedItems, addedItems);
            var selectionChangedHandler = this.SelectionChanged;
            if (selectionChangedHandler != null)
            {
                selectionChangedHandler(this, e);
            }
        }
#endif

        private bool isInSetSelectedItem = false;

        internal void SetSelectedItem(object context)
        {
            if (this.isInSetSelectedItem)
            {
                return;
            }

            this.isInSetSelectedItem = true;

            var isControlPressed = Keyboard.Modifiers == ModifierKeys.Control;
            this.SelectedItem = context;
            if (this.selectedItems.Count > 0 && context != null)
            {
                if (isControlPressed)
                {
                    if (!this.selectedItems.Contains(context))
                    {
                        this.selectedItems.Add(context);
                        this.RaiseSelectionChanged(new List<object>(), selectedItems.ToList());
                    }
                    else
                    {
                        this.selectedItems.Remove(context);
                        this.RaiseSelectionChanged(new List<object>() { context }, new List<object>());
                    }
                }
                else
                {
                    if (this.selectedItems.Count > 0)
                    {
                        if (!this.selectedItems.Contains(context))
                        {
                            var removedItems = this.selectedItems.ToList();
                            this.selectedItems.Clear();
                            this.selectedItems.Add(context);
                            this.RaiseSelectionChanged(removedItems, selectedItems.ToList());
                        }
                    }
                }
            }
            else
            {
                var removedItems = this.selectedItems.ToList();
                this.selectedItems.Clear();
                if (context != null)
                {
                    this.selectedItems.Add(context);
                    this.RaiseSelectionChanged(removedItems, selectedItems.ToList());
                }
                if (removedItems.Count > 0)
                {
                    this.RaiseSelectionChanged(removedItems, selectedItems.ToList());
                }
            }

            this.isInSetSelectedItem = false;
        }
        #endregion

        #region MouseOverItem (DependencyProperty)

        /// <summary>
        /// Gets / sets the mouse hover item when the path is hovered upon.
        /// </summary>
        public object MouseOverItem
        {
            get { return (object)GetValue(MouseOverItemProperty); }
            internal set { SetValue(MouseOverItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MouseHoverItem.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MouseOverItemProperty = DependencyProperty.Register("MouseOverItem", typeof(object), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region Uri (Dependency Property)

        /// <summary>
        /// Gets or sets Uri Value. It is a path where the Shape file is located.
        /// </summary>
        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Uri.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.Register("Uri", typeof(string), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region SelectedMapPath

        /// <summary>
        /// Gets or sets the selected map path.
        /// </summary>
        /// <value>The selected map path.</value>
        /// <remarks>MapPath Property that sets or gets the SelectedMapPath from the Map</remarks>
        public MapPath SelectedMapPath
        {
            get { return (MapPath)GetValue(SelectedMapPathProperty); }
            set { SetValue(SelectedMapPathProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedMapPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedMapPathProperty =
            DependencyProperty.Register("SelectedMapPath", typeof(MapPath), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion
        /// <summary>
        /// Gets the attribute children.
        /// </summary>
        /// <value>The attribute children.</value>
        public UIElementCollection AttributesChildren
        {
            get
            {
                return this.drawingCanvas.Children;
            }
            set { }
        }

        #region AttributesItemsSource (DependencyProperty)

        /// <summary>
        /// Gets / sets the ItemsSource for attributes
        /// </summary>
        public IEnumerable AttributesItemsSource
        {
            get { return (IEnumerable)GetValue(AttributesItemsSourceProperty); }
            set { SetValue(AttributesItemsSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AttributeItemsSource.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AttributesItemsSourceProperty = DependencyProperty.Register("AttributesItemsSource", typeof(IEnumerable), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region DataMapping (DependencyProperty)
        /// <summary>
        /// Gets / sets the DataMapping for custom ItemsSource.
        /// </summary>
        public string AttributesDataMapping
        {
            get { return (string)GetValue(AttributesDataMappingProperty); }
            set { SetValue(AttributesDataMappingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AttributeDataMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AttributesDataMappingProperty = DependencyProperty.Register("AttributesDataMapping", typeof(string), typeof(ShapeFileLayer), new PropertyMetadata(string.Empty, OnDataMappingChanged));

        private static void OnDataMappingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var shapeFile = dpo as ShapeFileLayer;
            if (args.NewValue != null)
            {
                shapeFile.GenerateColumns(args.NewValue.ToString());
            }
        }

        private Dictionary<string, FrameworkElementContext> contextEls = new Dictionary<string, FrameworkElementContext>();
        private List<string> keys = new List<string>();

        private void GenerateColumns(string dataMapping)
        {

            this.keys.Clear();
            this.contextEls.Clear();

            // ID;NAME;CUNTRY_CODE;
            var splitKeys = dataMapping.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var key in splitKeys)
            {
                this.keys.Add(key);
                var frameworkEl = new FrameworkElementContext();
                var binding = new Binding(key) { Mode = BindingMode.TwoWay };
                frameworkEl.SetValueBinding(binding);
                this.contextEls.Add(key, frameworkEl);
            }

            if (this.AttributesItemsSource != null && this.HasFileData)
            {
                this.UpdateAttributes(this.AttributesItemsSource, this.keys, this.currentFileData);

                if (this.ShowTooltipsForAttributes)
                {
                    int i = 0;
                    foreach (var el in this.ShapeCollection)
                    {
                        var rec = this.currentFileData.Records[i];
                        if (rec.Attributes.Count > 0)
                        {
                            this.SetTooltip(el.Shape);
                        }

                        i++;
                    }
                }
            }
        }
        internal void UpdateAttributes(IEnumerable itemsSource, List<string> keys, ShapeFileData fileData)
        {
            Func<string, object, object> getValue = (key, record) =>
                {
                    FrameworkElementContext el = default(FrameworkElementContext);
                    this.contextEls.TryGetValue(key, out el);
                    if (el != default(FrameworkElementContext))
                    {
                        el.DataContext = record;
                        var binding = new Binding(key) { Mode = BindingMode.OneWay };
                        el.SetValueBinding(binding);
                        return el.Value;
                    }

                    return null;
                };

            int i = 0;
            foreach (var record in itemsSource)
            {
                if (i < fileData.Records.Count)
                {
                    var fileRecord = fileData.Records[i];
                    fileRecord.Attributes.Clear();
                    foreach (var key in this.keys)
                    {
                        var value = getValue(key, record);
                        fileRecord.Attributes.Add(key, value != null ? value : string.Empty);
                    }
                }

                i++;
            }
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether map has a File data or not .
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        public bool HasFileData
        {
            get
            {
                return this.currentFileData != null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in suspend.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in suspend; otherwise, <c>false</c>.
        /// </value>
        public bool IsInSuspend
        {
            get;
            internal set;
        }

        #endregion

        #region ShapeFileLayerEvent
#if WPF
        /// <summary>
        /// Gets ShapeLoaded Event. This routed event calls when ever shape is loaded
        /// </summary>
        public static readonly RoutedEvent ShapesLoadedEvent = EventManager.RegisterRoutedEvent("ShapesLoaded", RoutingStrategy.Bubble, typeof(ShapesLoadedEventHandler), typeof(ShapeFileLayer));

        /// <summary>
        /// Occurs when Shape is Loaded.
        /// </summary>
        public event ShapesLoadedEventHandler ShapesLoaded
        {
            add
            {
                AddHandler(ShapesLoadedEvent, value);
            }

            remove
            {
                RemoveHandler(ShapesLoadedEvent, value);
            }
        }

        internal virtual void OnShapesLoaded(ShapesLoadedEventArgs e)
        {
            RaiseEvent(e);
        }

        internal void RaiseShapesLoadedEvent(object shapes)
        {
            ShapesLoadedEventArgs shapesLoadedEvtArgs = new ShapesLoadedEventArgs(shapes);
            shapesLoadedEvtArgs.RoutedEvent = ShapeFileLayer.ShapesLoadedEvent;
            this.OnShapesLoaded(shapesLoadedEvtArgs);
        }
#else
         /// <summary>
        /// Gets ShapeLoaded Event. This routed event calls when ever shape is loaded
        /// </summary>
        public event ShapesLoadedEventHandler ShapesLoaded;
        internal void OnShapesLoaded(object sender, ShapesLoadedEventArgs args)
        {
            if (this.ShapesLoaded != null)
            {
                this.ShapesLoaded(sender, args);
            }
        }
#endif

        #endregion

        #region DrawingCanvas code
        private void WireEvents()
        {
            this.canvas.SizeChanged += new SizeChangedEventHandler(this.DrawingCanvas_SizeChanged);
        }

        private void UnwireEvents()
        {
            this.canvas.SizeChanged -= new SizeChangedEventHandler(this.DrawingCanvas_SizeChanged);
        }

        private bool isSizeChanged = false;

        private void DrawingCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            this.shapeCount = 0;
            if (this.Uri != null)
            {
                string[] str = this.UriSplit();
                StringBuilder filename = new StringBuilder();
                if (str[str.Count() - 1].ToLower() == "shp")
                {
                    if (!this.isSizeChanged)
                    {
                        this.isSizeChanged = true;
                    }
                    if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                    {
                        if (!this.isRefreshed)
                        {
                            this.Refresh();
                            this.isRefreshed = true;
                        }
                    }
                }
                else if ((str[str.Count() - 1].ToLower() == "xml"))
                {
                    if (!this.isRefreshed)
                    {
                        if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                        {
                            this.Refresh();
                            this.isRefreshed = true;
                        }
                    }
                }

            }
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (!this.isRefreshed)
                {
                    this.Refresh();
                    this.isRefreshed = true;
                }
            }
            this.VirtualizeCanvas();
        }
        #endregion

        #region public methods

#if WPF
        /// <summary>
        /// Starts the initialization process for this element.
        /// </summary>
        public override void BeginInit()
#else
            /// <summary>
        /// Starts the initialization process for this element.
        /// </summary>
        public void BeginInit()
#endif
        {
            if (!this.IsInSuspend)
            {
                this.IsInSuspend = true;
            }

            if (this.isTemplateLoaded)
            {
                this.canvas.BeginInit();
                this.drawingCanvas.BeginInit();

            }
#if WPF
            base.BeginInit();
#endif
        }

#if WPF
        /// <summary>
        /// Indicates that the initialization process for the element is complete.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.Windows.FrameworkElement.EndInit"/> was called without <see cref="M:System.Windows.FrameworkElement.BeginInit"/> having previously been
        /// called on the element.</exception>
        public override void EndInit()
#else
            /// <summary>
        /// Indicates that the initialization process for the element is complete.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.Windows.FrameworkElement.EndInit"/> was called without <see cref="M:System.Windows.FrameworkElement.BeginInit"/> having previously been
        /// called on the element.</exception>
        public void EndInit()
#endif
        {
            if (this.IsInSuspend)
            {
                this.IsInSuspend = false;
            }

            if (this.isTemplateLoaded)
            {
                this.canvas.EndInit();
                this.drawingCanvas.EndInit();

            }
#if WPF
            base.EndInit();
#endif
        }



        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mapControl = MapControl.FindParent<MapControl>(this);
            if (this.mainGrid != null)
            {
                this.mainGrid.Children.Clear();
            }

            if (this.canvas != null)
            {
                this.UnwireEvents();
                this.canvas = null;
            }

            this.mainGrid = this.GetTemplateChild("PART_Grid") as Grid;
            this.canvas = this.GetTemplateChild("PART_Canvas") as ShapeFilePanel;
            this.mainGrid.Children.Add(this.drawingCanvas);
            this.mainGrid.Children.Add(this.mapElementCanvas);
#if WPF
            this.mainGrid.Children.Add(selectionPanel);
#endif
            this.tempZoomfactor = ZoomFactor;
            this.WireEvents();
            this.isTemplateLoaded = true;
            this.isload = true;
            //if (!this.isXmlFileLoaded)
            {
                if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    if (Uri != null)
                    {
                        if (Uri != string.Empty)
                        {
                            this.LoadShape();
                        }
                    }
                }
            }

            if (this.ColorPalette != ColorPalettes.None)
            {
                this.ChangeColorPalette();
            }

            this.CurrentMapColorPalette.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CurrentMapColorPalette_CollectionChanged);
            this.LabelCollections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(LabelCollections_CollectionChanged);
            this.ShapeCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ShapeCollection_CollectionChanged);
            this.SymbolCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SymbolCollection_CollectionChanged);
            this.MapPathCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(MapPathCollection_CollectionChanged);
        }

        void SymbolCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.mapElementCanvas != null)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    var items = from symbols in this.mapElementCanvas.Children.OfType<MapSymbols>()
                                select symbols;
                    if (items.ToList().Count > 0)
                    {
                        foreach (MapSymbols symbol in items.ToList())
                        {
                            this.mapElementCanvas.Children.Remove(symbol);
                        }
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (object obj in e.OldItems)
                {
                    this.mapElementCanvas.Children.Remove((obj as MapSymbols));
                }
            }
            if (e.NewItems != null)
            {
                foreach (object obj in e.NewItems)
                {
                    if ((obj as MapSymbols).Parent != null)
                    {
                        if ((obj as MapSymbols).Parent is MapElementPanel)
                        {
                            ((obj as MapSymbols).Parent as MapElementPanel).Children.Remove(obj as MapSymbols);
                        }
                    }

                    if (!(this.mapElementCanvas.Children.Contains(obj as UIElement)))
                    {
                        this.mapElementCanvas.Children.Add(obj as MapSymbols);
                    }
                }
            }
            this.UpdateDrawingCanvas();
        }


        void LabelCollections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.mapElementCanvas != null)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    var items = from labels in this.mapElementCanvas.Children.OfType<MapLabel>()
                                select labels;
                    if (items.ToList().Count > 0)
                    {
                        foreach (MapLabel label in items.ToList())
                        {
                            this.mapElementCanvas.Children.Remove(label);
                        }
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (object obj in e.OldItems)
                {
                    this.mapElementCanvas.Children.Remove((obj as MapLabel));
                }
            }
            if (e.NewItems != null)
            {
                foreach (object obj in e.NewItems)
                {
                    if (!(this.mapElementCanvas.Children.Contains(obj as UIElement)))
                    {
                        (obj as MapLabel).InitializeMapLabel(this);
                        if ((obj as MapLabel).Parent != null)
                        {
                            if ((obj as MapLabel).Parent is MapElementPanel)
                            {
                                ((obj as MapLabel).Parent as MapElementPanel).Children.Remove(obj as MapLabel);
                            }
                        }

                        this.mapElementCanvas.Children.Add((obj as MapLabel));
                    }
                }
            }
            this.UpdateDrawingCanvas();
        }

        void ShapeCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.canvas != null)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    var items = from shapes in this.canvas.Children.OfType<MapShapes>()
                                select shapes;
                    if (items.ToList().Count > 0)
                    {
                        foreach (MapShapes shape in items.ToList())
                        {
                            this.canvas.Children.Remove(shape);
                        }
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (object obj in e.OldItems)
                    {
                        this.canvas.Children.Remove(obj as UIElement);
                    }
                }
                if (this.canvas != null)
                {
                    if (e.NewItems != null)
                    {
                        foreach (object obj in e.NewItems)
                        {
                            if (!(this.canvas.Children.Contains(obj as UIElement)))
                            {
                                this.canvas.Children.Add(obj as UIElement);
                                Canvas.SetZIndex(obj as UIElement, -1);
                            }
                        }
                    }
                }
            }

        }
        void MapPathCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (canvas != null)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    var items = from path in this.canvas.Children.OfType<MapPath>()
                                select path;
                    if (items.ToList().Count > 0)
                    {
                        foreach (MapPath paths in items.ToList())
                        {
                            this.canvas.Children.Remove(paths);
                        }
                    }

                }
            }
            if (e.OldItems != null)
            {
                foreach (object obj in e.OldItems)
                {
                    this.canvas.Children.Remove(obj as UIElement);
                }
            }

            if (this.canvas != null)
            {
                if (e.NewItems != null)
                {
                    foreach (object obj in e.NewItems)
                    {
                        this.AddPaths(obj as MapPath);
                    }
                }
            }
        }



        void CurrentMapColorPalette_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.ColorPalette == ColorPalettes.CustomColorPalette && this.EnableColorPalette)
            {
                this.FillShapes();
                this.isColorBinded = false;
            }

        }

        /// <summary>
        ///  This method refresh all the ShapeCollections and load the XML stream
        /// </summary>
        public void Refresh()
        {

            // Loaded canvas size has been set which is used to maintain the canvas size while map has been resized.
            this.loadedCanvasSize = new Size(this.canvas.ActualWidth, this.canvas.ActualHeight);
            this.ShapeCollection.Clear();
            if (this.currentFileData != null)
            {
                this.LoadFromShapeFileData(this.currentFileData);
            }
            else if (this.currentxmlStream != null)
            {
                this.mapControl.Load(this.currentxmlStream);
            }
            //If Paths are added in the xaml then the Path will be created  
            if (MapPathCollection.Count > 0)
            {
                foreach (MapPath mapPath in this.MapPathCollection)
                {
                    this.AddPaths(mapPath);
                }
            }
            if (this.SymbolCollection.Count > 0 || this.LabelCollections.Count > 0)
            {
                this.UpdateDrawingCanvas();
            }

        }

#if WPF
        /// <summary>
        ///  This Method loads ShapeFileStream in Map Control
        /// </summary>
        /// <param name="fileName">Filename of Stream</param>
        public void LoadFromFile(string fileName)
        {
            using (var stream = new ShapeFileStream(fileName))
            {
                this.isColorBinded = false;
                var data = stream.ReadToEnd();
                this.LoadFromShapeFileData(data);
                this.currentFileData = data;
            }
            if (this.mapElementCanvas != null)
            {
                this.mapElementCanvas.InvalidateArrange();
            }
        }
#endif

        /// <summary>
        ///  This method loads the data from file stream
        /// </summary>
        /// <param name="fileStream">FileStram</param>
        /// <param name="fileName">File name of Stream</param>
        public void LoadFromStream(System.IO.FileStream fileStream, string fileName)
        {
            using (var stream = new ShapeFileStream(fileName))
            {
                var data = stream.ReadFromStream(fileStream);
                this.LoadFromShapeFileData(data);
                this.currentFileData = data;
            }
            if (this.mapElementCanvas != null)
            {
                this.mapElementCanvas.InvalidateArrange();
            }
        }

        private List<ElementsCache> elementsCache = new List<ElementsCache>();

        /// <summary>
        ///  This method Creates the Shapes from ShapeFileData
        /// </summary>
        /// <param name="data">ShapeFileData</param>
        public void LoadFromShapeFileData(ShapeFileData data)
        {
            this.recordIndex = 0;
            this.ShapeCollection.Clear();
            this.RecordRenderDelegate = new RenderDelegate(this.RenderRecords);
            if (data != null)
            {
                this.currentFileData = data;
                if (this.isload)
                {
                    this.Reset();
                    this.ShapeTransform = this.CreateShapeTransform(data);
                    if (this.AttributesItemsSource != null)
                    {
                        this.UpdateAttributes(this.AttributesItemsSource, this.keys, data);
                    }

#if WPF
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, this.RecordRenderDelegate, data);
#else
                    Dispatcher.BeginInvoke(this.RecordRenderDelegate, data);
#endif
                }
            }
        }

        private void RenderRecords(ShapeFileData data)
        {
            var index = this.recordIndex;
            if (index == 0)
            {
                this.BeginInit();
            }
            int incrementVal = (int)data.Records.Count / 4;
            if (data.Records.Count < incrementVal || incrementVal == 0)
            {
                incrementVal = data.Records.Count;
            }
            for (; index < incrementVal + this.recordIndex; index++)
            {
                if (index >= data.Records.Count)
                {
                    this.EndInit();
                    this.canvas.IsInSuspend = false;
                    this.canvas.InvalidateMeasure();
                    this.canvas.InvalidateArrange();
#if WPF
                    this.RaiseShapesLoadedEvent(this.ShapeCollection);
#else
                    ShapesLoadedEventArgs args = new ShapesLoadedEventArgs(this.ShapeCollection);
                    this.OnShapesLoaded(this, args);
#endif
                    return;
                }
                var rec = data.Records[index];
                if (rec.GetPartsCount() == 0)
                {
                    var pointShape = this.CreatPoint(rec);
                    pointShape.DataContext = rec;
                    if (this.ShowTooltipsForAttributes && rec.Attributes.Count > 0)
                    {
                        this.SetTooltip(pointShape);
                    }

                    this.AddToCache(pointShape);
                    this.canvas.Children.Add(pointShape);
                }
                else
                {
                    if (rec.ShapeType == ShapeType.Polygon || rec.ShapeType == ShapeType.Point || rec.ShapeType == ShapeType.Multipoint)
                    {
                        var geometry = this.CreatePathGeometry(rec);
                        if (rec.ShapeType == ShapeType.Polygon)
                        {
                            if (geometry.Bounds.Width < 1 || geometry.Bounds.Height < 1)
                            {
                                this.SkippedRecords.Add(rec);
                                continue;
                            }
                        }
                        this.CreateShapeFromRecord(rec, geometry);
                    }
                    else if (rec.ShapeType == ShapeType.PolyLine)
                    {
                        MapPath path = new MapPath();
                        this.CreatePathGeometry(rec, path);
                        this.MapPathCollection.Add(path);
                    }
                }
            }

            this.recordIndex += incrementVal;


#if WPF
            Dispatcher.BeginInvoke(DispatcherPriority.Background, this.RecordRenderDelegate, data);
#else
            Dispatcher.BeginInvoke(this.RecordRenderDelegate, data);
#endif

        }

        internal void CreateShapeFromRecord(ShapeFileRecord rec, Geometry geometry)
        {
            MapShapes shapes = new MapShapes();
            shapes.Shape = this.CreateShape(rec, geometry);
            shapes.Shape.DataContext = rec;
            shapes.ShapeInfo = new List<object>();
            foreach (object obj in rec.Attributes)
            {
                shapes.ShapeInfo.Add(obj);
            }
            if (this.ShowTooltipsForAttributes && rec.Attributes.Count > 0)
            {
                this.SetTooltip(shapes.Shape);
            }
            this.ShapeCollection.Add(shapes);
        }

        private void RenderSkippedRecord()
        {
            var i = this.skippedRecordIndex;
            var incrementValue = 100;
            for (; i < this.skippedRecordIndex + incrementValue; i++)
            {
                if (i >= this.SkippedRecords.Count)
                {
                    this.SkippedRecords.Clear();
                    foreach (ShapeFileRecord record in this.tempSkippedRecords)
                    {
                        this.SkippedRecords.Add(record);
                    }
                    this.canvas.IsInSuspend = false;
                    this.canvas.InvalidateMeasure();
                    return;
                }
                ShapeFileRecord rec = this.SkippedRecords[i];
                Geometry geometry = this.CreatePathGeometry(rec);
                if ((geometry.Bounds.Width * this.ZoomFactor) < 1 || (geometry.Bounds.Height * this.ZoomFactor) < 1)
                {
                    this.tempSkippedRecords.Add(rec);
                    continue;
                }
                else
                {
                    this.CreateShapeFromRecord(rec, geometry);
                }

            }
            this.skippedRecordIndex += incrementValue;
#if WPF
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, this.RenderSkippedRecordsDelegate);
#else
            this.Dispatcher.BeginInvoke(this.RenderSkippedRecordsDelegate);
#endif
        }


        #region Hover effect code

        #endregion

        private void AddToCache(Path shape)
        {
            Rect rect = Rect.Empty;
#if WPF
            rect = shape.Data.GetRenderBounds(new Pen(Brushes.Transparent, shape.StrokeThickness));
#else
            rect = shape.Data.Bounds;
#endif
            this.elementsCache.Add(new ElementsCache(rect, shape));
        }

        private void SetTooltip(FrameworkElement element)
        {
            if (this.AttributesTooltip == null)
            {
                if (this.localAttributeTooltip == null)
                {
                    var tooltip = new ToolTip();
                    var tooltipBinding = new Binding("Attributes") { Converter = this.AttributesTooltipConverter == null ? new ShapeFileTooltipConverter() : this.AttributesTooltipConverter };
                    tooltip.SetBinding(System.Windows.Controls.ToolTip.ContentProperty, tooltipBinding);
                    this.localAttributeTooltip = tooltip;
                }
#if WPF
                element.ToolTip = this.localAttributeTooltip;
#else
                ToolTipService.SetToolTip(element, this.localAttributeTooltip);
                this.localAttributeTooltip = null;
#endif
            }
            else
            {
#if WPF

                element.ToolTip = this.AttributesTooltip;


#else
                ToolTipService.SetToolTip(element, this.AttributesTooltip);
#endif
            }
        }

        /// <summary>
        ///  This method resets Tooltip and Elements of Canvas
        /// </summary>
        public void Reset()
        {
            if (this.isload)
            {
                this.localAttributeTooltip = null;
                this.elementsCache.Clear();
                this.canvas.Children.Clear();
            }
        }
        #endregion

        #region Zooming and Panning

        /// <summary>
        ///  Calls when Panning is occured.
        /// </summary>
        /// <param name="factorX">Horizontal Panning Value</param>
        /// <param name="factorY">Vertical Panning Value</param>
        protected override void OnPan(double factorX, double factorY)
        {
            base.OnPan(factorX, factorY);
            this.drawingCanvas.InvalidateArrange();
            this.VirtualizeCanvas();
            this.mapElementCanvas.InvalidateArrange();

        }

        /// <summary>
        ///  Calls when Zooming is occured
        /// </summary>
        /// <param name="zoomfactor">ZoomFactor</param>
        protected override void OnZoom(double zoomfactor)
        {
            base.OnZoom(zoomfactor);
            this.canvas.IsInSuspend = true;
            this.zoomTime = DateTime.Now.TimeOfDay;
            this.zoomTimer.Start();
            if (this.canvas != null)
            {
                this.canvasCenter = new Point(this.canvas.ActualWidth / 2, this.canvas.ActualHeight / 2);
                this.canvasCenter = this.ViewTransform.Inverse.Transform(this.canvasCenter);
            }
        }

        /// <summary>
        /// Calls when Zooming operation is completed
        /// </summary>
        /// <param name="zoomfactor">ZoomFactor</param>
        protected override void OnZoomed(double zoomfactor)
        {
            if (this.canvas != null && this.canvas.ActualWidth > 0 && this.canvas.ActualHeight > 0)
            {
                base.OnZoomed(zoomfactor);
                this.ZoomTransform.CenterX = this.canvas.ActualWidth / 2;
                this.ZoomTransform.CenterY = this.canvas.ActualHeight / 2;
                this.RenderTransformOrigin = new Point(this.canvas.ActualWidth / 2, this.canvas.ActualHeight / 2);
                this.UpdateDrawingCanvas();
                this.RenderSkippedRecordsDelegate = new SkippedRecordsDelegate(this.RenderSkippedRecord);
                this.VirtualizeCanvas();
            }
        }


        #endregion

        #region ColorPalette Functions

        private void FillShapes()
        {
            if (!this.isColorBinded)
            {
                int count = 0;
                if (this.canvas != null)
                {
                    foreach (MapShapes shape in this.canvas.Children)
                    {
                        Path path = shape.Shape;
                        if (this.CurrentMapColorPalette.Count > 0)
                        {
                            var mapPallette = this.CurrentMapColorPalette[count];
                            this.SetDataBinding(path, Path.FillProperty, mapPallette, () => mapPallette.ShapeFill);
                            this.SetDataBinding(path, Path.StrokeProperty, mapPallette, () => mapPallette.PathStroke);
                            this.SetDataBinding(path, Path.StrokeThicknessProperty, mapPallette, () => mapPallette.PathStrokeThickness);
                            count++;
                            if (count > this.CurrentMapColorPalette.Count - 1)
                            {
                                count = 0;
                            }
                        }
                    }
                }
                if (this.ColorPalette == ColorPalettes.None)
                {
                    this.isColorBinded = false;
                }
                else
                {
                    this.isColorBinded = true;
                }
            }
        }

        /// <summary>
        ///  This method clear all the fill color of the shapes
        /// </summary>
        public void ResetFill()
        {
            if (!this.EnableColorPalette)
            {
                if (this.canvas != null)
                {
                    foreach (MapShapes mapShape in this.canvas.Children)
                    {
                        if (this.PolygonFill == null)
                        {
                            this.SetDataBinding(mapShape.Shape, Path.FillProperty, () => this.ShapeFill);
                        }
                        else
                        {
                            this.SetDataBinding(mapShape.Shape, Path.FillProperty, () => this.PolygonFill);
                        }

                        if (this.PolygonStroke == null)
                        {
                            this.SetDataBinding(mapShape.Shape, Path.StrokeProperty, () => this.ShapeStroke);
                        }
                        else
                        {
                            this.SetDataBinding(mapShape.Shape, Path.StrokeProperty, () => this.PolygonStroke);
                        }
                        this.SetDataBinding(mapShape.Shape, Path.StrokeThicknessProperty, () => this.TempShapeStrokeThickness);
                    }
                }
            }
        }

        internal void ChangeColorPalette()
        {
            int colorcount = 0;
            Random ran = new Random();
            this.FillColors.Clear();
            this.StrokeColors.Clear();
            if (this.ColorPalette != ColorPalettes.CustomColorPalette && this.ColorPalette != ColorPalettes.None && this.EnableColorPalette)
            {
                // this.CurrentMapColorPalette.Clear();
                ResourceDictionary resourceDic = new ResourceDictionary();
#if WPF
                resourceDic.Source = new Uri("/Syncfusion.Maps.Wpf;component/Themes/ColorPalette1.xaml", UriKind.RelativeOrAbsolute);

#endif
#if SILVERLIGHT
                resourceDic.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/ColorPalette1.xaml", UriKind.RelativeOrAbsolute);

#endif
                if (resourceDic.Source != null)
                {

                    foreach (object obj in resourceDic.Keys)
                    {

                        if (this.ColorPalette == ColorPalettes.ColorPalette1)
                        {
                            this.ShapeHoverFill = resourceDic["ColorPalette1MouseOverFill"] as Brush;
                            this.ShapeHoverStroke = resourceDic["ColorPalette1MouseOverStroke"] as Brush;
                            this.ShapeHoverStrokeThickness = 0.3;
                            if (obj.ToString().Contains("Color1"))
                            {
                                this.FillColors.Add(obj.ToString());
                            }

                            if (obj.ToString().Contains("Stroke1"))
                            {
                                this.StrokeColors.Add(obj.ToString());
                            }
                        }
                        else if (this.ColorPalette == ColorPalettes.ColorPalette2)
                        {
                            this.ShapeHoverFill = resourceDic["ColorPalette2MouseOverFill"] as Brush;
                            this.ShapeHoverStroke = resourceDic["ColorPalette2MouseOverStroke"] as Brush;
                            this.ShapeHoverStrokeThickness = 0.3;
                            if (obj.ToString().Contains("Color2"))
                            {
                                this.FillColors.Add(obj.ToString());
                            }

                            if (obj.ToString().Contains("Stroke2"))
                            {
                                this.StrokeColors.Add(obj.ToString());
                            }
                        }
                        else if (this.ColorPalette == ColorPalettes.ColorPalette3)
                        {
                            this.ShapeHoverFill = resourceDic["ColorPalette3MouseOverFill"] as Brush;
                            this.ShapeHoverStroke = resourceDic["ColorPalette3MouseOverStroke"] as Brush;
                            this.ShapeHoverStrokeThickness = 0.3;
                            if (obj.ToString().Contains("Color3"))
                            {
                                this.FillColors.Add(obj.ToString());
                            }

                            if (obj.ToString().Contains("Stroke3"))
                            {
                                this.StrokeColors.Add(obj.ToString());
                            }
                        }
                        else if (this.ColorPalette == ColorPalettes.ColorPalette4)
                        {
                            this.ShapeHoverFill = resourceDic["ColorPalette4MouseOverFill"] as Brush;
                            this.ShapeHoverStroke = resourceDic["ColorPalette5MouseOverStroke"] as Brush;
                            this.ShapeHoverStrokeThickness = 0.3;
                            if (obj.ToString().Contains("Color4"))
                            {
                                this.FillColors.Add(obj.ToString());
                            }

                            if (obj.ToString().Contains("Stroke4"))
                            {
                                this.StrokeColors.Add(obj.ToString());
                            }
                        }
                        else if (this.ColorPalette == ColorPalettes.ColorPalette5)
                        {
                            this.ShapeHoverFill = resourceDic["ColorPalette5MouseOverFill"] as Brush;
                            this.ShapeHoverStroke = resourceDic["ColorPalette5MouseOverStroke"] as Brush;
                            this.ShapeHoverStrokeThickness = 0.3;
                            if (obj.ToString().Contains("Color5"))
                            {
                                this.FillColors.Add(obj.ToString());
                            }

                            if (obj.ToString().Contains("Stroke5"))
                            {
                                this.StrokeColors.Add(obj.ToString());
                            }
                        }
                        else if (this.ColorPalette == ColorPalettes.MetroPalette)
                        {
                            this.ShapeHoverFill = resourceDic["MetroMouseOverFill"] as Brush;
                            this.ShapeHoverStroke = resourceDic["MetroMouseOverStroke"] as Brush;
                            this.ShapeHoverStrokeThickness = 0.3;
                            if (obj.ToString().Contains("Metro1"))
                            {
                                this.FillColors.Add(obj.ToString());
                            }

                            if (obj.ToString().Contains("Stroke6"))
                            {
                                this.StrokeColors.Add(obj.ToString());
                            }
                        }
                    }
                    if (this.ColorPaletteMode == ColorPaletteMode.Sequential)
                    {
                        var ordercolor = from str in this.FillColors
                                         orderby str
                                         select str;
                        foreach (string str in ordercolor)
                        {
                            if (!this.isColorPaletteCreated || this.CurrentMapColorPalette.Count == 0)
                            {
                                this.isColorPaletteCreated = false;
                                this.CurrentMapColorPalette.Add(new MapColorPallette());
                            }
                            this.CurrentMapColorPalette[colorcount].ShapeFill = resourceDic[str] as Brush;
                            colorcount++;
                        }
                        this.isColorPaletteCreated = true;
                    }
                    else
                    {
                        foreach (string str in this.FillColors)
                        {
                            this.CurrentMapColorPalette.Add(new MapColorPallette());
                            this.CurrentMapColorPalette[colorcount].ShapeFill = resourceDic[str] as Brush;
                            colorcount++;
                        }
                    }
                }
            }
            else if (this.ColorPalette == ColorPalettes.CustomColorPalette && this.EnableColorPalette == true)
            {
                if (this.CustomMapColorPalette != null)
                {
                    if (this.CustomMapColorPalette.Count > 0)
                    {
                        foreach (MapColorPallette palette in this.CustomMapColorPalette)
                        {
                            this.CurrentMapColorPalette.Add(palette);
                        }
                    }
                }
            }
            //this.ResetFill();
            this.FillShapes();
        }

        private void SetRandomColorsCustomColors()
        {


            if (this.CurrentMapColorPalette != null)
            {
                var ordercolors = from color in this.CurrentMapColorPalette
                                  orderby color.ShapeFill.ToString()
                                  select color;
                this.CurrentMapColorPalette.Clear();
                foreach (MapColorPallette palette in ordercolors)
                {
                    this.CurrentMapColorPalette.Add(palette);
                }
            }

        }

        #endregion

        #region Transform helpers
        private TransformGroup CreateShapeTransform(ShapeFileData info)
        {
            // Bounding box for the shapefile.
            var minX = this.BoundMinX = info.FileHeader.MinX;
            var maxX = this.BoundMaxX = info.FileHeader.MaxX;
            var minY = this.BoundMinY = info.FileHeader.MinY;
            var maxY = this.BoundMaxY = info.FileHeader.MaxY;
            var xformGroup = this.SetScaleTransformGroup(minX, minY, maxX, maxY);
            return xformGroup;
        }

        internal TransformGroup SetScaleTransformGroup(double minX, double minY, double maxX, double maxY)
        {
            if (this.loadedCanvasSize.Height == 0 && this.loadedCanvasSize.Width == 0)
            {
                if (this.canvas != null)
                {
                    this.loadedCanvasSize = new Size(this.canvas.ActualWidth, this.canvas.ActualHeight);
                }
            }

            // Width and height of the bounding box.
            var width = Math.Abs(maxX - minX);
            var height = Math.Abs(maxY - minY);

            // Aspect ratio of the bounding box.
            var aspectRatio = width / height;

            // Aspect ratio of the canvas after loaded. Loaded size has been used to maintain the size while save and load the Map
            var canvasRatio = this.loadedCanvasSize.Width / this.loadedCanvasSize.Height;

            // Compute a scale factor so that the shapefile geometry
            // will maximize the space used on the canvas while still
            // maintaining its aspect ratio.
            var scaleFactor = 1.0;
            if (aspectRatio < canvasRatio)
            {
                scaleFactor = this.loadedCanvasSize.Height / height;
            }
            else
            {
                scaleFactor = this.loadedCanvasSize.Width / width;
            }

            // Compute the scale transformation. Note that we flip
            // the Y-values because the lon/lat grid is like a cartesian
            // coordinate system where Y-values increase upwards.
            var scaleTransform = new ScaleTransform { ScaleX = scaleFactor, ScaleY = -scaleFactor };

            // Compute the translate transformation so that the shapefile
            // geometry will be centered on the canvas.
            var translateTransform = new TranslateTransform();
            translateTransform.X = (this.loadedCanvasSize.Width - (minX + maxX) * scaleFactor) / 2;
            translateTransform.Y = (this.loadedCanvasSize.Height + (minY + maxY) * scaleFactor) / 2;

            // Add the two transforms to a transform group.
            var xformGroup = new TransformGroup();
            xformGroup.Children.Add(scaleTransform);
            xformGroup.Children.Add(translateTransform);

            return xformGroup;
        }
        #endregion

        #region HelperMethods
        private void PopulateLabels()
        {
            if (this.LabelCollections.Count > 0)
            {
                this.LabelCollections.Clear();
            }
            foreach (object obj in this.LabelSource)
            {

                MapLabel label = new MapLabel();
                PropertyInfo[] propInfos = obj.GetType().GetProperties();
                foreach (PropertyInfo propinfo in propInfos)
                {
                    if (propinfo.GetValue(obj, null) != null)
                    {
                        switch (propinfo.Name.ToLower(CultureInfo.InvariantCulture))
                        {

                            case "latitude":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(double))
                                {
                                    label.Latitude = (double)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "longitude":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(double))
                                {
                                    label.Longitude = (double)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "text":
                                label.LabelText = propinfo.GetValue(obj, null).ToString();
                                break;
                            case "foreground":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(Brush))
                                {
                                    label.LabelForeground = (Brush)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "background":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(Brush))
                                {
                                    label.LabelBackground = (Brush)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "fontstyle":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(FontStyle))
                                {
                                    label.LabelFontStyle = (FontStyle)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "fontfamily":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(FontFamily))
                                {
                                    label.LabelFontFamily = (FontFamily)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "textwrapping":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(TextWrapping))
                                {
                                    label.LabelTextWarpping = (TextWrapping)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "fontsize":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(double))
                                {
                                    label.LabelFontSize = (double)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "labelwidth":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(double))
                                {
                                    label.LabelWidth = (double)propinfo.GetValue(obj, null);
                                }
                                break;
                        }
                    }
                }
                this.LabelCollections.Add(label);
            }
        }

        private void PopulateSymbols()
        {
            if (this.SymbolCollection.Count > 0)
            {
                this.SymbolCollection.Clear();
            }
            foreach (object obj in this.SymbolSource)
            {
                MapSymbols symbol = new MapSymbols();
                PropertyInfo[] propInfos = obj.GetType().GetProperties();
                foreach (PropertyInfo propinfo in propInfos)
                {
                    if (propinfo.GetValue(obj, null) != null)
                    {
                        switch (propinfo.Name.ToLower(CultureInfo.InvariantCulture))
                        {
                            case "latitude":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(double))
                                {
                                    symbol.Latitude = (double)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "longitude":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(double))
                                {
                                    symbol.Longitude = (double)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "text":
                                symbol.SymbolText = propinfo.GetValue(obj, null).ToString();
                                break;
                            case "foreground":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(Brush))
                                {
                                    symbol.SymbolLabelForeground = (Brush)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "background":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(Brush))
                                {
                                    symbol.SymbolLabelBackground = (Brush)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "fontstyle":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(FontStyle))
                                {
                                    symbol.SymbolLabelFontStyle = (FontStyle)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "fontfamily":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(FontFamily))
                                {
                                    symbol.SymbolLabelFontFamily = (FontFamily)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "textwrapping":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(TextWrapping))
                                {
                                    symbol.SymbolLabelTextWarpping = (TextWrapping)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "fontsize":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(double))
                                {
                                    symbol.SymbolLabelFontSize = (double)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "labelwidth":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(double))
                                {
                                    symbol.SymbolLabelWidth = (double)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "symbol":
                                //  if (propinfo.GetValue(obj, null).GetType() == typeof(FrameworkElement))
                                {
                                    symbol.Symbol = (UIElement)propinfo.GetValue(obj, null);
                                }
                                break;
                        }
                    }
                }
                this.SymbolCollection.Add(symbol);
            }
        }

        private void PopulatePath()
        {
            if (this.MapPathCollection.Count > 0)
            {
                this.MapPathCollection.Clear();
            }
            foreach (object obj in this.PathSource)
            {
                MapPath mapPath = new MapPath();
                PropertyInfo[] propInfos = obj.GetType().GetProperties();
                foreach (PropertyInfo propinfo in propInfos)
                {
                    if (propinfo.GetValue(obj, null) != null)
                    {
                        switch (propinfo.Name.ToLower(CultureInfo.InvariantCulture))
                        {
                            case "points":
                                if (propinfo.GetValue(obj, null).GetType().BaseType == typeof(Collection<Point>))
                                {
                                    mapPath.PathPoints = (ObservableCollection<Point>)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "label":
                                mapPath.PathLabel = propinfo.GetValue(obj, null).ToString();
                                break;
                            case "color":
                                if (propinfo.GetValue(obj, null).GetType().BaseType == typeof(Brush))
                                {
                                    mapPath.PathColor = (Brush)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "labelcolor":
                                if (propinfo.GetValue(obj, null).GetType().BaseType == typeof(Brush))
                                {
                                    mapPath.PathLabelForeground = (Brush)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "labelposition":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(PathLabelPosition))
                                {
                                    mapPath.PathLabelPosition = (PathLabelPosition)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "labelpoint":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(Point))
                                {
                                    mapPath.LabelPoint = (Point)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "fontfamily":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(FontFamily))
                                {
                                    mapPath.PathLabelFontFamily = (FontFamily)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "fontsize":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(double))
                                {
                                    mapPath.PathLabelFontSize = (double)propinfo.GetValue(obj, null);
                                }
                                break;
                            case "fontstyle":
                                if (propinfo.GetValue(obj, null).GetType() == typeof(FontStyle))
                                {
                                    mapPath.PathLabelFontStyle = (FontStyle)propinfo.GetValue(obj, null);
                                }
                                break;

                        }
                    }
                }
                this.MapPathCollection.Add(mapPath);
            }

        }



        /// <summary>
        /// Add the Path in the Map
        /// </summary>
        /// <param name="mapPath">The map path.</param>
        private void AddPaths(MapPath mapPath)
        {
            if (!(this.canvas.Children.Contains(mapPath)))
            {
                this.SetPathElement(mapPath);
                this.canvas.Children.Add(mapPath);
                if (mapPath.LabelPoint.X == 0 && mapPath.LabelPoint.Y == 0 && this.IsDynamicCreatePath == false)
                {
                    mapPath.LabelPoint = mapPath.PathPoints[0];
                }
                mapPath.LabelMargin = this.GetLabelMargin(mapPath.LabelPoint, mapPath);

            }
        }
        /// <summary>
        /// Gets the label margin.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="mapPath">The map path.</param>
        /// <returns></returns>
        internal Thickness GetLabelMargin(Point p, MapPath mapPath)
        {
            var Labelmargin = new Thickness();
            var PathPoint0 = new Point();
            var PathPoint1 = new Point();

            int index = mapPath.PathPoints.IndexOf(mapPath.LabelPoint);

            //Check whether the given LabelPoint is present in the PathPoints or not.
            if (mapPath.PathPoints.Count > 0)
            {
                if (index == -1 && this.IsDynamicCreatePath == false)
                {
                    //Set the visibility to visible
                    mapPath.PathLabelVisibility = Visibility.Collapsed;
                    //throw new Exception("LabelPoint not found in MapPoints");
                }
            }

            if (mapPath.LabelPoint.X != 0 && mapPath.LabelPoint.Y != 0)
            {
                //Set the visibility to visible
                mapPath.PathLabelVisibility = Visibility.Visible;

                bool LabelPoint_in_path = false;
                p = this.ZoomPanPointValue(this.GetMapElementsPosition((mapPath).LabelPoint));

                PathPoint0 = this.GetMapElementsPosition(mapPath.PathPoints[index]);
                if (mapPath.PathPoints.Count - 1 > index)
                {
                    PathPoint1 = this.GetMapElementsPosition(mapPath.PathPoints[index + 1]);
                }
                else
                {
                    PathPoint1 = this.GetMapElementsPosition(mapPath.PathPoints[index - 1]);
                }

                Point anotherpoint = new Point();

                //Count variable for find the next PathPoint for the LabelPoint is available or not
                int count = 0;

                //Check whether the next point of the LabelPoint is available or not
                foreach (Point pathpoint in (mapPath).PathPoints)
                {
                    if (pathpoint == (mapPath).LabelPoint)
                    {
                        //Check whether the LabelPoint is last point in the Points in the path.
                        //If yes means, take the previous PathPoint and find out the midpoint between and place the label there.
                        if ((count + 1) == (mapPath).PathPoints.Count)
                        {
                            anotherpoint = (mapPath).PathPoints[count - 1];  //Use the previous Point to find out the midpoint between them and place the label there.
                            anotherpoint = this.ZoomPanPointValue(this.GetMapElementsPosition(anotherpoint)); //Change that Previous Point into Latitude and Longitude values on the basis  of Zooming and Panning Value.

                        }
                        else
                        {
                            anotherpoint = (mapPath).PathPoints[count + 1];
                            anotherpoint = this.ZoomPanPointValue(this.GetMapElementsPosition(anotherpoint)); //Next Point to yhe Label Point.
                            //Find the midpoint between then and place the Label there.

                        }
                        break;
                    }
                    count++;
                }

                LabelPoint_in_path = mapPath.PathPoints.Contains(mapPath.LabelPoint);

                if (LabelPoint_in_path == true)
                {

                    //If the LabelPoint is OnPoint means, place the label at the point mentioned.
                    if ((mapPath).PathLabelPosition == PathLabelPosition.OnPoint)
                    {
                        Labelmargin = new Thickness(p.X, p.Y, 0, 0);
                    }
                    else
                    {
                        //Count variable for find the next PathPoint for the LabelPoint is available or not
                        //int count = 0;

                        //Check whether the next point of the LabelPoint is available or not
                        foreach (Point pathpoint in (mapPath).PathPoints)
                        {
                            if (pathpoint == (mapPath).LabelPoint)
                            {
                                //Check whether the LabelPoint is last point in the Points in the path.
                                //If yes means, take the previous PathPoint and find out the midpoint between and place the label there.

                                Labelmargin = new Thickness((anotherpoint.X + p.X) / 2, (p.Y + anotherpoint.Y) / 2, 0, 0); // Place the label in the Midpoint

                                break;
                            }
                            count++;
                        }
                    }
                }

                //To find the angle between the Selected PathLine and the nearest Line, use this angle for rotating the Label in the Map.
                var angle = Math.Atan2(PathPoint0.X - PathPoint1.X, PathPoint0.Y - PathPoint1.Y);

                if (angle < 0)
                {
                    //Converting the Radian value into Degree 
                    mapPath.Angle = (Math.Abs(angle * 180 / 3.14) - 90);
                }
                else
                {
                    //Converting the Radian value into Degree 
                    mapPath.Angle = 360 - (Math.Abs(angle * 180 / 3.14) - 90);
                }
            }
            return Labelmargin;

        }
        /// <summary>
        ///  This function returns the Point value on the basis of current Pan and Zoom factors. 
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>Point</returns>
        internal Point ZoomPanPointValue(Point p)
        {
            var p1 = new Point((p.X - (this.mapControl.LayeredContent as ShapeFileLayer).PanTransform.X) / (this.mapControl.LayeredContent as ShapeFileLayer).ZoomFactor, (p.Y - (this.mapControl.LayeredContent as ShapeFileLayer).PanTransform.Y) / (this.mapControl.LayeredContent as ShapeFileLayer).ZoomFactor);
            return p1;
        }
        /// <summary>
        /// Sets the path element.
        /// </summary>
        /// <param name="mapPath">The map path.</param>
        private void SetPathElement(MapPath mapPath)
        {
            var p = new Path();
            p.Data = mapPath.PointsToGeomentry(mapPath.PathPoints, this);
            p.SetBinding(Path.StrokeThicknessProperty, new Binding { Source = mapPath, Path = new PropertyPath("PathStrokeThickness"), Mode = BindingMode.TwoWay });
            p.SetBinding(Path.StrokeProperty, new Binding { Source = mapPath, Path = new PropertyPath("PathColor"), Mode = BindingMode.TwoWay });
            mapPath.RenderTransform = (this.mapControl.LayeredContent as ShapeFileLayer).ViewTransform;
            mapPath.MapPathContent = p;
        }
        //To hold the Start and End Point of the Path while creating dynamically.
        private static Point start = new Point();
        private static Point end = new Point();

        /// <summary>
        /// Creates the dynamic path.
        /// </summary>
        /// <remarks>
        /// The Path Creates Dynamically by clicking the mouse
        /// </remarks>
        /// <param name="Position">Position</param>

        private void CreateDynamicPath(Point Position)
        {
            MapPath path = new MapPath();
            if (flag_start_point == 0)
            {
                start = this.PointToLatitudeLongitude(Position);
                flag_start_point++;
                dynamicpathpointscount++;     //Counts the number of Points in the Dynamic Path
            }
            else
            {
                end = this.PointToLatitudeLongitude(Position);
                path.PathPoints.Add(start);
                path.PathPoints.Add(end);
                this.MapPathCollection.Add(path);
                start = end;
                dynamicpathpointscount++;     //Counts the number of Points in the Dynamic Path
            }
        }


        internal void UpdateDrawingCanvas()
        {
            foreach (FrameworkElement child in this.drawingCanvas.Children)
            {
                child.RenderTransform = this.ZoomTransform;
            }
            foreach (FrameworkElement child in this.mapElementCanvas.Children)
            {
                child.RenderTransform = this.ZoomTransform;
            }
            this.drawingCanvas.InvalidateArrange();
            this.mapElementCanvas.InvalidateArrange();

        }

        #endregion

        #region Shape Helpers
        private Path CreateShape(ShapeFileRecord record, Geometry geometry)
        {
            var path = new System.Windows.Shapes.Path();
            path.Data = geometry;
            path.RenderTransform = this.ViewTransform;
            if (record.ShapeType == ShapeType.Polygon)
            {
                if (!this.EnableColorPalette)
                {
                    if (this.PolygonFill == null)
                    {
                        path.SetBinding(Path.FillProperty, new Binding { Source = this, Path = new PropertyPath("ShapeFill") });
                    }
                    else
                    {
                        path.SetBinding(Path.FillProperty, new Binding { Source = this, Path = new PropertyPath("PolygonFill") });
                    }

                    if (this.PolygonStroke == null)
                    {
                        path.SetBinding(Path.StrokeProperty, new Binding { Source = this, Path = new PropertyPath("ShapeStroke") });
                    }
                    else
                    {
                        path.SetBinding(Path.StrokeProperty, new Binding { Source = this, Path = new PropertyPath("PolygonStroke") });
                    }

                    path.SetBinding(Path.StrokeThicknessProperty, new Binding { Source = this, Path = new PropertyPath("TempShapeStrokeThickness") });
                }
                else
                {
                    this.ResetFill();
                    if (this.CurrentMapColorPalette.Count > 0)
                    {
                        var mapPallette = this.CurrentMapColorPalette[this.shapeCount];
                        this.SetDataBinding(path, Path.FillProperty, mapPallette, () => mapPallette.ShapeFill);
                        this.SetDataBinding(path, Path.StrokeProperty, mapPallette, () => mapPallette.PathStroke);
                        this.SetDataBinding(path, Path.StrokeThicknessProperty, mapPallette, () => mapPallette.PathStrokeThickness);
                        this.shapeCount++;
                        if (this.shapeCount > this.CurrentMapColorPalette.Count - 1)
                        {
                            this.shapeCount = 0;
                        }
                    }
                }
            }
            else
            {
                this.SetDataBinding(path, Path.StrokeProperty, () => this.ShapeStroke);
                this.SetDataBinding(path, Path.StrokeThicknessProperty, () => this.TempShapeStrokeThickness);
                this.SetDataBinding(path, Path.FillProperty, () => this.ShapeFill);
            }

            return path;

        }

        private void SetDataBinding<TProperty>(FrameworkElement targetElement, DependencyProperty dp, Expression<Func<TProperty>> property)
        {
            this.SetDataBinding(targetElement, dp, this, property);
        }

        private void SetDataBinding<TProperty>(FrameworkElement targetElement, DependencyProperty dp, DependencyObject source, Expression<Func<TProperty>> property)
        {
            var bindingExp = targetElement.GetBindingExpression(dp);
            if (bindingExp != null)
            {
                targetElement.ClearValue(dp);
            }

            var lambda = (LambdaExpression)property;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            var path = memberExpression.Member.Name;
            var binding = new Binding(path) { Mode = BindingMode.OneWay, Source = source };
            targetElement.SetBinding(dp, binding);
        }

        private Geometry CreatePathGeometry(ShapeFileRecord record, MapPath path)
        {
#if WPF
            var geometry = new StreamGeometry();
            using (StreamGeometryContext ctx = geometry.Open())
            {
                for (int i = 0; i < record.Parts.Count; i++)
                {
                    // Determine the starting index and the end index
                    // into the points array that defines the figure.
                    int start = record.Parts[i];
                    int end;
                    if (record.Parts.Count > 1 && i != (record.Parts.Count - 1))
                    {
                        end = record.Parts[i + 1];
                    }
                    else
                    {
                        end = record.Points.Count;
                    }

                    var points = (from pta in record.Points.GetRange(start, (end - start))
                                  select this.ShapeTransform.Transform(pta)).ToList();
                    ctx.BeginFigure((Point)points.ElementAt(0), true, false);
                    ctx.PolyLineTo(points, true, false);
                    if (path != null)
                    {
                        foreach (Point pnt in points)
                        {
                            path.PathPoints.Add(pnt);
                        }
                    }
                }
            }

            geometry.Freeze();
            return geometry;

#else
            var geometry = new PathGeometry();
            for (int i = 0; i < record.Parts.Count; i++)
            {
                var figure = new PathFigure();

                // Determine the starting index and the end index
                // into the points array that defines the figure.
                int start = record.Parts[i];
                int end;
                if (record.Parts.Count > 1 && i != (record.Parts.Count - 1))
                {
                    end = record.Parts[i + 1];
                }
                else
                {
                    end = record.Points.Count;
                }

                // Add line segments to the figure.
                var points = (from pta in record.Points.GetRange(start, (end - start))
                              select pta).ToList();
                Point pt = this.ShapeTransform.Transform(points[0]);
                figure.StartPoint = pt;
                PolyLineSegment polylinesegment = new PolyLineSegment();
                if (path != null)
                {
                    path.PathPoints.Add(pt);
                }
                for (int l = 1; l < points.Count(); l++)
                {
                    var pts = this.ShapeTransform.Transform(points[l]);
                    if (path != null)
                    {
                        path.PathPoints.Add(pts);
                    }
                    polylinesegment.Points.Add(pts);
                }
                figure.Segments.Add(polylinesegment);
                geometry.Figures.Add(figure);
            }

            return geometry;
#endif
        }

        private Geometry CreatePathGeometry(ShapeFileRecord record)
        {
            var geometry = this.CreatePathGeometry(record, null);
            return geometry;
        }

        private Path CreatPoint(ShapeFileRecord record)
        {
            var geometry = new GeometryGroup();

            // Add ellipse geometries to the group.
            foreach (var pt in record.Points)
            {
                var ellipse = new EllipseGeometry();

                // Transform center point of the ellipse from lon/lat to
                // canvas coordinates.
                ellipse.Center = this.ShapeTransform.Transform(pt);

                // Set the size of the ellipse.
                ellipse.RadiusX = 0.1;
                ellipse.RadiusY = 0.1;

                // Add the ellipse to the geometry group.
                geometry.Children.Add(ellipse);
            }

            geometry.Transform = this.ViewTransform;

            var path = new System.Windows.Shapes.Path();
            path.Data = geometry;
            this.SetDataBinding(path, Path.FillProperty, () => this.ShapeFill);
            this.SetDataBinding(path, Path.StrokeThicknessProperty, () => this.ShapeStrokeThickness);
            this.SetDataBinding(path, Path.StrokeProperty, () => this.ShapeStroke);
            return path;
        }

        /// <summary>
        ///  This method navigates the element by its position
        /// </summary>
        /// <param name="position">Point</param>
        /// <returns>
        /// FrameworkElement
        /// </returns>
        public FrameworkElement PointToElement(Point position)
        {
            if (!this.HasFileData)
            {
                return null;
            }
            FrameworkElement element = null;
#if WPF
            HitTestResult result = VisualTreeHelper.HitTest(this, position);
            if (result != null)
            {
                if (result.VisualHit is Path)
                {
                    element = result.VisualHit as Path;
                }
                else
                {
                    var colls = from shapes in this.ShapeCollection
                                where shapes.Shape.Data.Bounds.Contains(position)
                                select shapes.Shape;
                    if (colls.ToList().Count > 0)
                    {
                        element = colls.ToList()[0] as Path;
                    }
                }
            }
#else
            if (this.ViewTransform != null)
            {
                foreach (var el in this.ShapeCollection)
                {
                    if (this.ViewTransform.TransformBounds(el.Shape.Data.Bounds).Contains(position))
                    {
                        element = el.Shape;
                        break;
                    }
                }
            }
#endif
            return element;
        }

        internal void
            LoadShape()
        {
#if SyncfusionFramework3_5
            string uri = "empty";
            System.IO.Stream shapeStream;
            var items = Application.Current.GetType().Assembly.GetManifestResourceNames();
            if (items.Count() > 0)
            {
                var item = from res in items.ToList()
                           where res.ToLower() == this.Uri.ToLower()
                           select res;
                if (item.ToList().Count > 0)
                {
                    uri = items[items.ToList().IndexOf(item.ToList()[0].ToString())];
                }
            }
            shapeStream = Application.Current.GetType().Assembly.GetManifestResourceStream(uri);
#else
            string uri = "empty";
            System.IO.Stream shapeStream=null;
            string[] items=null;
            var assembiles = ((dynamic)AppDomain.CurrentDomain).GetAssemblies() as Assembly[];
            Assembly appAssembly = null;
            string currentNamespace = this.Uri.Split('.')[0];
            foreach (Assembly assem in assembiles)
            {
                var namespaces = assem.GetTypes();
                var namespc = from nspce in namespaces
                         where nspce.Namespace == currentNamespace
                         select nspce;
                if (namespc.Count() > 0)
                {
                    appAssembly = assem;
                    break;
                }
            }
            if (appAssembly != null)
            {
                items = appAssembly.GetManifestResourceNames();
            }
#if WPF               
            else
            {
                String[] resources = Application.ResourceAssembly.GetManifestResourceNames();
                    foreach (var item in resources)
                    {
                        if (item.Contains(".shp"))
                        {
                            shapeStream = Application.ResourceAssembly.GetManifestResourceStream(item);
                        }
                    }
            }
#endif
            if (items != null)
            {
                if (items.Count() > 0)
                {
                    var item = from res in items.ToList()
                               where res.ToLower() == this.Uri.ToLower()
                               select res;
                    if (item.ToList().Count > 0)
                    {
                        uri = items[items.ToList().IndexOf(item.ToList()[0].ToString())];
                    }
                }
                if (appAssembly != null)
                {
                    shapeStream = appAssembly.GetManifestResourceStream(uri);
                }
            }
#endif
            if (shapeStream != null)
            {
#if SILVERLIGHT
                string[] str = this.UriSplit();
                if (str[str.Count() - 1].ToLower() == "shp")
                {
                    using (var stream = new ShapeFileStream(" "))
                    {
                        this.data = stream.ReadFromStream(shapeStream);
                        this.currentFileData = this.data;
                    }
                }
                else
                {

                    System.IO.Stream xmlStream = Application.Current.GetType().Assembly.GetManifestResourceStream(uri);
                    this.currentxmlStream = xmlStream;
                    MapControl.isXmlContent = true;

                }
#endif
#if WPF

                string[] extension = this.UriSplit();
                var ext = extension[extension.Count() - 1].ToString();
                // To determine weather load shape file or xml file
                // If shape file then shape file loaded
                if (ext.ToLower(CultureInfo.InvariantCulture) == "shp")
                {
                    string tempuri = string.Empty;
                    if (Application.Current != null)
                    {
                        // To load the dbf file automatically to set the tool tip
                        tempuri = Application.Current.GetType().Assembly.GetName().Name.ToString();
                    }
                    else
                    {
                        tempuri = Application.ResourceAssembly.GetType().Assembly.GetName().Name.ToString();
                    }
                    tempuri = this.Uri.Remove(0, tempuri.Length + 1);
                    string[] str = tempuri.Split('.');
                    StringBuilder filename = new StringBuilder();
                    for (int i = 0; i <= str.Count() - 3; i++)
                    {
                        filename.Append(str[i] + "\\");
                    }
                    filename.Append(str[str.Count() - 2] + ".");
                    filename.Append(str[str.Count() - 1]);
                    using (var stream = new ShapeFileStream(AppDomain.CurrentDomain.BaseDirectory + filename))
                    {
                        this.data = stream.ReadFromStream(shapeStream);
                        this.currentFileData = this.data;
                    }
                }
                // If xml file then xml file will be loaded
                else if (ext.ToLower(CultureInfo.InvariantCulture) == "xml")
                {
                    System.IO.Stream xmlStream = Application.Current.GetType().Assembly.GetManifestResourceStream(uri);
                    this.currentxmlStream = xmlStream;
                    MapControl.isXmlContent = true;
                }
                else
                {
                    return;
                }

#endif
            }

        }

        /// <summary>
        /// Splits the Uri property value
        /// </summary>
        /// <returns></returns>
        private string[] UriSplit()
        {
            string tempuri = string.Empty;
            if (this.Uri != null)
            {
                if (this.Uri != string.Empty)
                {
                    tempuri = this.Uri.Remove(0, tempuri.Length + 1);
                }
            }
            return tempuri.Split('.');
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.currentFileData != null)
            {
                foreach (var rec in this.currentFileData.Records)
                {
                    rec.Attributes.Clear();
                }
                this.currentFileData.Records.Clear();
                this.currentFileData.FileHeader = null;
                this.currentFileData = null;
            }

            if (this.canvas != null)
            {
                this.UnwireEvents();
                this.canvas = null;
            }

            if (this.localAttributeTooltip != null)
            {
                this.localAttributeTooltip = null;
            }

            this.keys.Clear();
            this.contextEls.Clear();
            this.elementsCache.Clear();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches an
        /// element in its route that is derived from this class. Implement this method to
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that
        /// contains the event data.</param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (this.SelectedMapLabel != null)
            {
                if (this.SelectedMapLabel.isMouseDown && !this.SelectedMapLabel.IsLabelEditing)
                {
                    var latlan = this.PointToLatitudeLongitude(new Point((e.GetPosition(this).X - (this.SelectedMapLabel.mousePosition.X * this.ZoomFactor) + (this.SelectedMapLabel.DesiredSize.Width / 2 * this.ZoomFactor)), (e.GetPosition(this).Y - (this.SelectedMapLabel.mousePosition.Y * this.ZoomFactor) + (this.SelectedMapLabel.DesiredSize.Height / 2 * this.ZoomFactor))));
                    this.SelectedMapLabel.Latitude = latlan.Y;
                    this.SelectedMapLabel.Longitude = latlan.X;
                    this.SelectedMapLabel.isDragged = true;
                    this.drawingCanvas.InvalidateArrange();
                    this.mapElementCanvas.InvalidateArrange();
                }
            }
            if (this.SelectedMapSymbol != null)
            {
                if (this.SelectedMapSymbol.isMousedown && !this.SelectedMapSymbol.IsLabelEditing)
                {
                    var latlan = this.PointToLatitudeLongitude(new Point((e.GetPosition(this).X - (this.SelectedMapSymbol.mousePosition.X * this.ZoomFactor) + (this.SelectedMapSymbol.DesiredSize.Width / 2 * this.ZoomFactor)), (e.GetPosition(this).Y - (this.SelectedMapSymbol.mousePosition.Y * this.ZoomFactor) + (this.SelectedMapSymbol.DesiredSize.Height / 2 * this.ZoomFactor))));
                    this.SelectedMapSymbol.Latitude = latlan.Y;
                    this.SelectedMapSymbol.Longitude = latlan.X;
                    this.SelectedMapSymbol.isDragged = true;
                    this.drawingCanvas.InvalidateArrange();
                    this.mapElementCanvas.InvalidateArrange();
                }
            }
           
            if (this.ShapeTransform == null)
            {
                return;
            }
            if (this.DisplayCoordinates)
            {
                var mousePs = e.GetPosition(this);
                var latLonPt = this.PointToLatitudeLongitude(mousePs);
                this.LatLonPoint = latLonPt;
            }



#if SILVERLIGHT
            #region HoverEffects
            Path path;
            if (tempPath != null)
            {
                if (tempPath != e.OriginalSource)
                {
                    if (!this.EnableHoverEffects)
                    {
                        return;
                    }

                    if (this.fillExp != null)
                    {
                        tempPath.SetBinding(Path.FillProperty, fillExp.ParentBinding);
                    }
                    else
                    {
                        tempPath.Fill = tempPathfill;
                    }
                    if (this.strokeExp != null)
                    {
                        tempPath.SetBinding(Path.StrokeProperty, strokeExp.ParentBinding);
                    }

                    if (strokeThicknessExp != null)
                    {
                        tempPath.SetBinding(Path.StrokeThicknessProperty, strokeThicknessExp.ParentBinding);
                    }

                    this.MouseOverItem = null;
                }
            }
            var shape = from shp in this.ShapeCollection
                        where shp.Shape.Equals(e.OriginalSource as Path)
                        select shp;
            if (e.OriginalSource is Path && shape.Count() > 0)
            {
                path = e.OriginalSource as Path;

                if (!this.EnableHoverEffects)
                {
                    return;
                }
                if (path.GetBindingExpression(Path.FillProperty) != null && path.GetBindingExpression(Path.StrokeProperty) != null && path.GetBindingExpression(Path.StrokeThicknessProperty) != null && path != tempPath)
                {
                    fillExp = path.GetBindingExpression(Path.FillProperty);
                    strokeExp = path.GetBindingExpression(Path.StrokeProperty);
                    strokeThicknessExp = path.GetBindingExpression(Path.StrokeThicknessProperty);
                }
                else
                {
                    tempStrokeBrush = path.Stroke;
                    tempPathfill = path.Fill;
                    tempStrokeThickness = path.StrokeThickness;
                }

                path.Fill = this.ShapeHoverFill;
                path.Stroke = this.ShapeHoverStroke;
                path.StrokeThickness = this.ShapeHoverStrokeThickness;
                this.MouseOverItem = path;
                tempPath = e.OriginalSource as Path;
            }
            #endregion
#endif



            base.OnMouseMove(e);
        }
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonDown"/> routed event reaches an
        /// element in its route that is derived from this class. Implement this method to
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data. The event data reports that the right mouse button
        /// was pressed.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (this.IsDynamicCreatePath)
            {
                if (this.mapControl.SymbolPaletteVisibility == Visibility.Visible)
                {
                    this.CreateDynamicPath(new Point(e.GetPosition(mapControl).X - this.Margin.Left - this.mapControl.Margin.Left - this.mapControl.SymbolPalette.ActualWidth, e.GetPosition(mapControl).Y - this.Margin.Top - this.mapControl.Margin.Top));
                }
                else
                {
                    this.CreateDynamicPath(new Point(e.GetPosition(mapControl).X - this.Margin.Left - this.mapControl.Margin.Left, e.GetPosition(mapControl).Y - this.Margin.Top - this.mapControl.Margin.Top));
                }
            }
            flag_start_point = 0;

            e.Handled = true;
            base.OnMouseRightButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised
        /// on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data. The event data reports that the left mouse button
        /// was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            this.mapControl.isMouseDown = true;
            this.mapControl.startPoint = e.GetPosition(this.mapControl);
            this.mapControl.panTranslatePoint = new Point((this.mapControl.LayeredContent as ShapeFileLayer).PanTransform.X, (this.mapControl.LayeredContent as ShapeFileLayer).PanTransform.Y);

            if (this.IsDynamicCreatePath)
            {
                if (this.mapControl.SymbolPaletteVisibility == Visibility.Visible)
                {
                    this.CreateDynamicPath(new Point(e.GetPosition(mapControl).X - this.Margin.Left - this.mapControl.Margin.Left - this.mapControl.SymbolPalette.ActualWidth, e.GetPosition(mapControl).Y - this.Margin.Top - this.mapControl.Margin.Top));
                }
                else
                {
                    this.CreateDynamicPath(new Point(e.GetPosition(mapControl).X - this.Margin.Left - this.mapControl.Margin.Left, e.GetPosition(mapControl).Y - this.Margin.Top - this.mapControl.Margin.Top));
                }
            }
            var mapLbl = from lbl in this.LabelCollections
                         where lbl.IsLabelEditing
                         select lbl;
            if (mapLbl.ToList().Count > 0)
            {
                var maplabel = mapLbl.ToList()[0];
                maplabel.IsLabelEditing = false;
                maplabel.LabelText = maplabel.labelTextBox.Text;
                this.ClickedItem = null;
            }
            var mapSym = from sym in this.SymbolCollection
                         where sym.IsLabelEditing
                         select sym;
            if (mapSym.ToList().Count > 0)
            {
                var mapSymbol = mapSym.ToList()[0];
                mapSymbol.IsLabelEditing = false;
                mapSymbol.SymbolText = mapSymbol.symbollabelTextBox.Text;
                this.ClickedItem = null;

            }

            if (e.OriginalSource is Path)
            {
                this.SelectedShape = this.GetPointedElement(e.GetPosition(this.canvas));

            }
            else
            {
                this.SelectedShape = null;
            }

            if (this.HasFileData)
            {
                if (e.OriginalSource is Path)
                {
                    this.SetSelectedItem((e.OriginalSource as Path).DataContext);
                }
                else
                {
                    this.SetSelectedItem(null);
                }
            }
            this.IsDroped = false;
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an
        /// element in its route that is derived from this class. Implement this method to
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>
        /// that contains the event data. The event data reports that the left mouse button
        /// was released.</param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                this.TempClickPoint = e.GetPosition(this);
                templatlon = this.PointToLatitudeLongitude(this.TempClickPoint);
                if (!this.IsPan)
                {
                    if (this.EnableLabel && this.ClickedItem == null)
                    {
                        MapLabel mapLabel = new MapLabel { LabelText = "Enter Label", IsLabelEditing = true, Latitude = templatlon.Y, Longitude = templatlon.X };
                        mapLabel.CheckSelection(this);
                        mapLabel.IsSelected = true;
                        this.LabelCollections.Add(mapLabel);
                        this.SelectedMapLabel = mapLabel;
                    }
                }
            }
            if (this.mapControl.SelectedSymbolItem != null)
            {
#if SILVERLIGHT
                var point = PointToLatitudeLongitude(new Point(e.GetPosition(this).X - (this.mapControl.SelectedSymbolItem as FrameworkElement).ActualWidth - 5, e.GetPosition(this).Y));
#endif
#if WPF
                var point = PointToLatitudeLongitude(e.GetPosition(this));

#endif
                MapSymbols maps = new MapSymbols();

                maps.Latitude = point.Y;
                maps.Longitude = point.X;
                maps.Symbol = this.mapControl.SelectedSymbolItem as UIElement;
                this.SymbolCollection.Add(maps);
                this.mapControl.SelectedSymbolItem = null;
                this.mapControl.isMouseUp = false;
                this.mapControl.isMouseDown = false;
                this.IsDroped = true;
                maps.CheckSelection(this);
                maps.IsSelected = true;
                this.SelectedMapSymbol = maps;
            }
            if (this.SelectedMapLabel != null)
            {
                this.SelectedMapLabel.isMouseDown = false;
                if (this.SelectedMapLabel.CheckLabelIntersection())
                {

                    this.SelectedMapLabel.Latitude = this.SelectedMapLabel.startlat;
                    this.SelectedMapLabel.Longitude = this.SelectedMapLabel.startlong;
                }
                this.SelectedMapLabel.isDragged = false;
            }
            if (this.SelectedMapSymbol != null)
            {
                if (this.SelectedMapSymbol.CheckSymbolIntersection())
                {
                    this.SelectedMapSymbol.Latitude = this.SelectedMapSymbol.startlat;
                    this.SelectedMapSymbol.Longitude = this.SelectedMapSymbol.startlong;
                }
                this.SelectedMapSymbol.isMousedown = false;

                this.SelectedMapSymbol.isDragged = false;
            }
            if (this.isSizeChanged)
            {
                this.isSizeChanged = false;
                //  this.Refresh();
            }


            this.tempPanX = this.PanTransform.X;
            this.tempPanY = this.PanTransform.Y;
            this.drawingCanvas.InvalidateArrange();
            this.mapElementCanvas.InvalidateArrange();
            base.OnMouseLeftButtonUp(e);
        }




        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave"/> attached event is raised on
        /// this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that
        /// contains the event data.</param>
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            this.ReleaseMouseCapture();
            base.OnMouseLeave(e);
        }

        internal FrameworkElement GetPointedElement(Point position)
        {
            FrameworkElement pointedElement = null;
            var element = this.PointToElement(position);
            var point_element = from shape in this.ShapeCollection
                                where shape.Shape.Equals(element)
                                select shape;
            if (point_element.ToList().Count > 0)
            {
                pointedElement = point_element.ToList()[0] as MapShapes;
            }
            return pointedElement;
        }


        private void VirtualizeCanvas()
        {
            if (this.mapControl != null)
            {
                if (this.mapControl.EnableVirtualization)
                {
                    Action act = delegate
                    {
                        if (this.mapControl != null)
                        {
                            foreach (MapShapes shp in (this.mapControl.LayeredContent as ShapeFileLayer).ShapeCollection)
                            {
                                Rect mapRect = new Rect(new Point(0, 0), new Size(this.mapControl.mapScrollViewer.ViewportWidth, this.mapControl.mapScrollViewer.ViewportHeight));
                                mapRect.Intersect(this.ViewTransform.TransformBounds(shp.Shape.Data.Bounds));
                                if (mapRect == Rect.Empty)
                                {
                                    if ((this.mapControl.LayeredContent as ShapeFileLayer).canvas.Children.Count > 1)
                                    {
                                        (this.mapControl.LayeredContent as ShapeFileLayer).canvas.Children.Remove(shp);
                                    }
                                }
                                else
                                {
                                    if (!(this.mapControl.LayeredContent as ShapeFileLayer).canvas.Children.Contains(shp))
                                    {
                                        (this.mapControl.LayeredContent as ShapeFileLayer).canvas.Children.Add(shp);
                                    }
                                }
                            }
                        }
                    };
#if WPF
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, act);
#else
                    Dispatcher.BeginInvoke(act);
#endif
                }
            }
        }
        #endregion

        #region Private classes
        internal class ElementsCache
        {
            public ElementsCache(Rect rect, FrameworkElement element)
            {
                this.Bounds = rect;
                this.Element = element;
            }

            public Rect Bounds { get; private set; }

            public FrameworkElement Element { get; private set; }
        }
        #endregion

    }

    #region ComparePoints

    internal class PointComparer : IEqualityComparer<Point>
    {
        public bool Equals(Point x, Point y)
        {
            if (double.Parse(String.Format("{0:0.0}", x)) == double.Parse(String.Format("{0:0.0}", y)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Point obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.GetHashCode();
            }
        }
    }


    #endregion
}
