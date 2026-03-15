#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.IO;

namespace Syncfusion.UI.Xaml.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Collections;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Specialized;
#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.Foundation;
    using Windows.UI.Xaml.Shapes;
    using Windows.UI.Xaml.Data;
    using Windows.UI;
    using System.Threading.Tasks;
#else
    using System.Windows.Media;
    using System.Windows.Data;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Shapes;
    using System.Windows.Input;
#if WPF
    using System.Data;
#elif WINDOWSPHONE_8
    using System.Threading.Tasks;
#endif
#endif


    /// <summary>
    /// Represents the ShapeFileLayer class.
    /// </summary>
    /// <remarks>
    /// The ShapeFileLayer is one of the Layer in the SfMap. This shape file layer will read the ShapeFile(.shp) and dBase(.dbf) file and parse them in to SfMap Elements.
    /// </remarks>
    /// <example>
    /// <para>The Following code demonstrate how to create a ShapeFileLayer in C#. </para>
    /// <code language="C#">
    /// using Syncfusion.UI.Xaml.Maps;
    /// using System;
    /// using System.Collections.Generic;
    /// using System.IO;
    /// using System.Linq;
    /// using Windows.Foundation;
    /// using Windows.Foundation.Collections;
    /// using Windows.UI;
    /// using Windows.UI.Xaml;
    /// using Windows.UI.Xaml.Controls;
    /// using Windows.UI.Xaml.Controls.Primitives;
    /// using Windows.UI.Xaml.Data;
    /// using Windows.UI.Xaml.Input;
    /// using Windows.UI.Xaml.Media;
    /// using Windows.UI.Xaml.Navigation;    
    /// 
    /// namespace MapApp
    /// {        
    ///     public sealed partial class MainPage : Page
    ///     {
    ///         public MainPage()
    ///         {
    ///             this.InitializeComponent();
    ///             SfMap syncMap = new SfMap();
    ///             ShapeFileLayer layer = new ShapeFileLayer();
    ///             layer.Uri = "MapApp.world1.shp";
    ///             syncMap.Layers.Add(layer);
    ///             this.mainGrid.Children.Add(syncMap);
    ///         }
    /// 
    ///     }
    /// 
    /// }
    /// </code>
    /// </example>
    //[ClassReference(IsReviewed = false)]
    public class ShapeFileLayer : MapLayer
    {
        #region Protected Fields

        protected bool isBaseLayer = true;
        protected ShapeFileLayer baselayer;

        #endregion

        #region PrivateFields

        private ItemsControl mapItemsControl;
        private Grid shapeGrid;
        private SfMap mapControl;
        private Size avilabelMapSize;

        #endregion

        #region CLR Properties

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

        #endregion

        #region Internal Fields

        internal bool isLoaded;
        internal Panel pointsPanel;
        internal Panel mapItemsPanel;
        internal List<object> pointValues;
        internal ShapeType shapeType;
        internal ObservableCollection<MapShape> removedItems = new ObservableCollection<MapShape>();
        internal ContentControl mapPopup;
        internal ObservableCollection<MapShape> tempSelectedShapes = new ObservableCollection<MapShape>();
        internal ShapeFileReader shapeReader;
        internal ShapeFileDBFReader dbfReader;
        internal ShapeFileKmlReader kmlReader;
        internal bool canSelect = true;
        internal Point draggingPoint = new Point();
        internal Point dragStartPoint = new Point();
        internal bool isKmlLayer;
#if WINRT
        internal static ResourceDictionary resources = new ResourceDictionary { Source = new Uri("ms-appx:///Syncfusion.SfMaps.WinRT/Themes/Templates.xaml", UriKind.RelativeOrAbsolute) };
        internal static ResourceDictionary colorPaletteResources = new ResourceDictionary { Source = new Uri("ms-appx:///Syncfusion.SfMaps.WinRT/Themes/ColorPalette.xaml", UriKind.RelativeOrAbsolute) };
#else
#if WPF
        internal static ResourceDictionary resources = new ResourceDictionary { Source = new Uri(@"/Syncfusion.SfMaps.WPF;component/Themes/Templates.xaml", UriKind.RelativeOrAbsolute) };
        internal static ResourceDictionary colorPaletteResources = new ResourceDictionary { Source = new Uri(@"/Syncfusion.SfMaps.WPF;component/Themes/ColorPalette.xaml", UriKind.RelativeOrAbsolute) };
#else
#if SILVERLIGHT || SILVERLIGHT_5
        internal static ResourceDictionary resources = new ResourceDictionary { Source = new Uri(@"/Syncfusion.SfMaps.Silverlight;component/Themes/Templates.xaml", UriKind.RelativeOrAbsolute) };
        internal static ResourceDictionary colorPaletteResources = new ResourceDictionary { Source = new Uri(@"/Syncfusion.SfMaps.Silverlight;component/Themes/ColorPalette.xaml", UriKind.RelativeOrAbsolute) };
#else
#if WINDOWSPHONE_8
        internal static ResourceDictionary resources = new ResourceDictionary { Source = new Uri("/Syncfusion.SfMaps.WP8;component/Themes/Templates.xaml", UriKind.RelativeOrAbsolute) };
        internal static ResourceDictionary colorPaletteResources = new ResourceDictionary { Source = new Uri("/Syncfusion.SfMaps.WP8;component/Themes/ColorPalette.xaml", UriKind.RelativeOrAbsolute) };

#else
        internal static ResourceDictionary resources = new ResourceDictionary { Source = new Uri("/Syncfusion.SfMaps.WP7;component/Themes/Templates.xaml", UriKind.RelativeOrAbsolute) };
        internal static ResourceDictionary colorPaletteResources = new ResourceDictionary { Source = new Uri("/Syncfusion.SfMaps.WP7;component/Themes/ColorPalette.xaml", UriKind.RelativeOrAbsolute) };
#endif
#endif
#endif
#endif
        internal double minX;
        internal double maxX;
        internal double minY;
        internal double maxY;
        internal Rect MapRect;
        internal bool isValueBinded = false;
        internal int shapeCount = 0;

        #endregion

        #region InternalProperties


#if WPF
        internal TransformGroup AnimationTransfrom
        {
            get { return (TransformGroup)GetValue(AnimationTransfromProperty); }
            set { SetValue(AnimationTransfromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnimationTransfrom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationTransfromProperty =
            DependencyProperty.Register("AnimationTransfrom", typeof(TransformGroup), typeof(ShapeFileLayer), new PropertyMetadata(null));
#else

        internal CompositeTransform AnimationTransfrom
        {
            get { return (CompositeTransform)GetValue(AnimationTransfromProperty); }
            set { SetValue(AnimationTransfromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnimationTransfrom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationTransfromProperty =
            DependencyProperty.Register("AnimationTransfrom", typeof(CompositeTransform), typeof(ShapeFileLayer), new PropertyMetadata(null));

#endif

        /// <summary>
        /// Gets  the collection of brushes that are applied on the shapes.
        /// </summary>
        /// <remarks>CurrentColorPalette is the read only property to get the brushes that are currently applied on the shape.</remarks>
        public ObservableCollection<MapColorPalette> CurrentColorPalette
        {
            get { return (ObservableCollection<MapColorPalette>)GetValue(CurrentColorPaletteProperty); }
            internal set { SetValue(CurrentColorPaletteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentColorPalette.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentColorPaletteProperty =
            DependencyProperty.Register("CurrentColorPalette", typeof(ObservableCollection<MapColorPalette>), typeof(ShapeFileLayer), new PropertyMetadata(null));





        #endregion

        #region Properties

        #region MapPointPopupTemplate



        public DataTemplate MapPointPopupTemplate
        {
            get { return (DataTemplate)GetValue(MapPointPopupTemplateProperty); }
            set { SetValue(MapPointPopupTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapPointPopupTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPointPopupTemplateProperty =
            DependencyProperty.Register("MapPointPopupTemplate", typeof(DataTemplate), typeof(ShapeFileLayer), new PropertyMetadata(null));




        #endregion

        #region MapPointTemplate



        public DataTemplate MapPointTemplate
        {
            get { return (DataTemplate)GetValue(MapPointTemplateProperty); }
            set { SetValue(MapPointTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapPointTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPointTemplateProperty =
            DependencyProperty.Register("MapPointTemplate", typeof(DataTemplate), typeof(ShapeFileLayer), new PropertyMetadata(null));




        #endregion

        #region MapPoints



        public ObservableCollection<MapPoint> MapPoints
        {
            get { return (ObservableCollection<MapPoint>)GetValue(MapPointsProperty); }
            set { SetValue(MapPointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapPoints.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPointsProperty =
            DependencyProperty.Register("MapPoints", typeof(ObservableCollection<MapPoint>), typeof(ShapeFileLayer), new PropertyMetadata(null));



        #endregion

        #region SubShapeFileLayer

        public ObservableCollection<SubShapeFileLayer> SubShapeFileLayers
        {
            get { return (ObservableCollection<SubShapeFileLayer>)GetValue(SubShapeFileLayersProperty); }
            set { SetValue(SubShapeFileLayersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubShapeFileLayers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubShapeFileLayersProperty =
            DependencyProperty.Register("SubShapeFileLayers", typeof(ObservableCollection<SubShapeFileLayer>), typeof(ShapeFileLayer), new PropertyMetadata(null));
        #endregion

        #region PointData

        public object PointData
        {
            get { return GetValue(PointDataProperty); }
            set { SetValue(PointDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointDataProperty =
            DependencyProperty.Register("PointData", typeof(object), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region MapPointPopupVisibility



        internal Visibility MapPointPopupVisibility
        {
            get { return (Visibility)GetValue(MapPointPopupVisibilityProperty); }
            set { SetValue(MapPointPopupVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapPointPopupVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MapPointPopupVisibilityProperty =
            DependencyProperty.Register("MapPointPopupVisibility", typeof(Visibility), typeof(ShapeFileLayer), new PropertyMetadata(Visibility.Collapsed));



        #endregion

        #region MappointMargin



        public Thickness MapPointMargin
        {
            get { return (Thickness)GetValue(MapPointMarginProperty); }
            set { SetValue(MapPointMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapPointMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPointMarginProperty =
            DependencyProperty.Register("MapPointMargin", typeof(Thickness), typeof(ShapeFileLayer), new PropertyMetadata(new Thickness(0, 0, 0, 0)));



        #endregion

        #region MapItemsVisibility


        /// <summary>
        /// Gets or sets  the Visibility for the MapItems.
        /// </summary>
        /// <remarks>
        /// MapItems are shown the property value of under bound objects of the Shape. This
        /// property sets the Visibility of MapItems.
        /// </remarks>
        /// <value>
        /// Type <see cref="Visibility"/>
        /// </value>
        /// <example>       
        /// <code language="C#">
        ///     using Syncfusion.UI.Xaml.Maps;
        ///     using System;
        ///     using System.Collections.Generic;
        ///     using System.IO;
        ///     using System.Linq;
        ///     using Windows.Foundation;
        ///     using Windows.Foundation.Collections;
        ///     using Windows.UI;
        ///     using Windows.UI.Xaml;
        ///     using Windows.UI.Xaml.Controls;
        ///     using Windows.UI.Xaml.Controls.Primitives;
        ///     using Windows.UI.Xaml.Data;
        ///     using Windows.UI.Xaml.Input;
        ///     using Windows.UI.Xaml.Media;
        ///     using Windows.UI.Xaml.Navigation;
        ///     
        ///     namespace MapApp
        ///     {               
        ///         public sealed partial class MainPage : Page
        ///         {
        ///             public MainPage()
        ///             {
        ///                 this.InitializeComponent();
        ///                 SfMap syncMap = new SfMap();
        ///                 syncMap.EnablePan = true;
        ///                 ShapeFileLayer layer = new ShapeFileLayer();
        ///                 layer.Uri = "MapAPP.world.shp";
        ///                 layer.MapItemsVisibility = Visibility.Visible;
        ///                 syncMap.Layers.Add(layer);                    
        ///             }               
        ///         }
        ///     }
        ///
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public Visibility MapItemsVisibility
        {
            get { return (Visibility)GetValue(MapItemsVisibilityProperty); }
            set { SetValue(MapItemsVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapItemsVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapItemsVisibilityProperty =
            DependencyProperty.Register("MapItemsVisibility", typeof(Visibility), typeof(ShapeFileLayer), new PropertyMetadata(Visibility.Visible));



        #endregion

        #region CustomDataSoruceTemplate



        /// <summary>
        /// Gets or sets the Template for the Items in the CustomDataSource.
        /// </summary>
        /// <remarks>
        /// Use this property to define the template for the Items which are defines in the CustomDataSource.
        /// </remarks>
        /// <value>
        /// Type :<see cref="DataTemplate"/>
        /// </value>
        //[ClassReference(IsReviewed = false)]
        public DataTemplate CustomDataSourceTemplate
        {
            get { return (DataTemplate)GetValue(CustomDataSourceTemplateProperty); }
            set { SetValue(CustomDataSourceTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomDataSourceTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomDataSourceTemplateProperty =
            DependencyProperty.Register("CustomDataSourceTemplate", typeof(DataTemplate), typeof(ShapeFileLayer), new PropertyMetadata(null));





        #endregion

        #region SymbolTemplate



        /// <summary>
        /// Gets or sets the Template for the MapAnnotations.
        /// </summary>
        /// <remarks>
        /// Use the AnnotationTemplate property to re-define its Template. This will override the default Template of the MapAnnotations.  
        /// </remarks>       
        //[ClassReference(IsReviewed = false)]
        public DataTemplate AnnotationTemplate
        {
            get { return (DataTemplate)GetValue(AnnotationTemplateProperty); }
            set { SetValue(AnnotationTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationTemplateProperty =
            DependencyProperty.Register("AnnotationTemplate", typeof(DataTemplate), typeof(ShapeFileLayer), new PropertyMetadata(null));




        #endregion

        #region ShapeSettings



        /// <summary>
        /// Gets or sets the values for a shape's appearance in the SfMap.
        /// </summary>
        /// <remarks>
        /// Use this property to define the Stroke, Fill, StrokeThickness and the Under
        /// bound value of a shape.
        /// </remarks>
        /// <value>
        /// Type : <see cref="ShapeSetting"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        ///    using Syncfusion.UI.Xaml.Maps;
        ///    using System;
        ///    using System.Collections.Generic;
        ///    using System.IO;
        ///    using System.Linq;
        ///    using Windows.Foundation;
        ///    using Windows.Foundation.Collections;
        ///    using Windows.UI;
        ///    using Windows.UI.Xaml;
        ///    using Windows.UI.Xaml.Controls;
        ///    using Windows.UI.Xaml.Controls.Primitives;
        ///    using Windows.UI.Xaml.Data;
        ///    using Windows.UI.Xaml.Input;
        ///    using Windows.UI.Xaml.Media;
        ///    using Windows.UI.Xaml.Navigation;
        ///            
        ///    
        ///    namespace MapApp
        ///    {
        ///       
        ///        public sealed partial class MainPage : Page
        ///        {
        ///            public MainPage()
        ///            {
        ///                this.InitializeComponent();
        ///                SfMap syncMap = new SfMap();
        ///                syncMap.EnablePan = true;
        ///                ShapeFileLayer layer = new ShapeFileLayer();
        ///                ShapeSetting shapeSettings = new ShapeSetting();
        ///                shapeSettings.ShapeFill = new SolidColorBrush(Colors.Red);
        ///                shapeSettings.ShapeStroke = new SolidColorBrush(Colors.Black);
        ///                shapeSettings.ShapeStrokeThickness = 1d;
        ///                layer.ShapeSettings = shapeSettings;
        ///                layer.Uri = "MapApp.world.shp";
        ///                syncMap.Layers.Add(layer);
        ///            }       
        ///        }
        ///    }
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public ShapeSetting ShapeSettings
        {
            get { return (ShapeSetting)GetValue(ShapeSettingsProperty); }
            set { SetValue(ShapeSettingsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeSettingsProperty =
            DependencyProperty.Register("ShapeSettings", typeof(ShapeSetting), typeof(ShapeFileLayer), new PropertyMetadata(null, OnShapeSettingsChanged));

        private static void OnShapeSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer && e.NewValue is ShapeSetting)
            {
                (e.NewValue as ShapeSetting).layer = d as ShapeFileLayer;
                (d as ShapeFileLayer).SetColorPalette((e.NewValue as ShapeSetting).ColorPalette);
                foreach (MapShape shp in (d as ShapeFileLayer).MapShapes)
                {
                    (d as ShapeFileLayer).FillColors(shp.Shape, null, -1);
                }
            }
        }



        #endregion

        #region BubbleMarkerSetting

        /// <summary>
        /// Gets or sets bubbles settings for the SfMap.
        /// </summary>
        /// <remarks>
        /// Use this property to set the maximum size,minimum size, fill, stroke and color mappings for the bubbles.
        /// </remarks>
        /// <value>
        /// Type : <see cref="BubbleMarkerSetting"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// namespace MapApp
        /// {
        ///     
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             syncMap.EnablePan = true;
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
        ///             bubbleSetting.Fill = new SolidColorBrush(Colors.Blue);
        ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Red);
        ///             bubbleSetting.MaxSize = 50;
        ///             bubbleSetting.MinSize = 10;
        ///             layer.BubbleMarkerSetting = bubbleSetting;
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.MapItemsVisibility = Visibility.Visible;
        ///             syncMap.Layers.Add(layer);
        ///             this.mainGrid.Children.Add(syncMap);
        ///         }       
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public BubbleMarkerSetting BubbleMarkerSetting
        {
            get { return (BubbleMarkerSetting)GetValue(BubbleMarkerSettingProperty); }
            set { SetValue(BubbleMarkerSettingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BubbleMarkerSetting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BubbleMarkerSettingProperty =
            DependencyProperty.Register("BubbleMarkerSetting", typeof(BubbleMarkerSetting), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region Bubbles



        /// <summary>
        /// Gets the bubbles of the ShapeFileLayer.
        /// </summary>
        /// <remarks>
        /// This is read only property to read the bubbles of the ShapeFileLayer.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public ObservableCollection<Bubble> Bubbles
        {
            get { return (ObservableCollection<Bubble>)GetValue(BubblesProperty); }
            internal set { SetValue(BubblesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bubbles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BubblesProperty =
            DependencyProperty.Register("Bubbles", typeof(ObservableCollection<Bubble>), typeof(ShapeFileLayer), new PropertyMetadata(null));





        #endregion

        #region Legends

        public ObservableCollection<Legend> Legends
        {
            get { return (ObservableCollection<Legend>)GetValue(LegendsProperty); }
            internal set { SetValue(LegendsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Legend.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendsProperty =
            DependencyProperty.Register("Legends", typeof(ObservableCollection<Legend>), typeof(ShapeFileLayer), new PropertyMetadata(null));



        #endregion

        #region LegendVisibility
        public Visibility LegendVisibility
        {
            get { return (Visibility)GetValue(LegendVisibilityProperty); }
            set { SetValue(LegendVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendVisibilityProperty =
            DependencyProperty.Register("LegendVisibility", typeof(Visibility), typeof(ShapeFileLayer), new PropertyMetadata(Visibility.Collapsed, OnLegendVisibilityChanged));

        private static void OnLegendVisibilityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var instance = (ShapeFileLayer)obj;
            instance.LegendRefresh();
        }
        #endregion

        #region LegendPositionX
        public double LegendPositionX
        {
            get { return (double)GetValue(LegendPositionXProperty); }
            set { SetValue(LegendPositionXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendPositionX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendPositionXProperty =
            DependencyProperty.Register("LegendPositionX", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(0d, OnLegendPositionChanged));
        #endregion

        #region LegendPositionY
        public double LegendPositionY
        {
            get { return (double)GetValue(LegendPositionYProperty); }
            set { SetValue(LegendPositionYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendPositionY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendPositionYProperty =
            DependencyProperty.Register("LegendPositionY", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(0d, OnLegendPositionChanged));
        #endregion

        #region LegendPosition
        public LegendPosition LegendPosition
        {
            get { return (LegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendPositionProperty =
            DependencyProperty.Register("LegendPosition", typeof(LegendPosition), typeof(ShapeFileLayer), new PropertyMetadata(LegendPosition.Default, OnLegendPositionChanged));

        private static void OnLegendPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                (d as ShapeFileLayer).SetLegendPosition();
            }
        }
        #endregion

        #region LegendMargin
        internal Thickness LegendMargin
        {
            get { return (Thickness)GetValue(LegendMarginProperty); }
            set { SetValue(LegendMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LegendMarginProperty =
            DependencyProperty.Register("LegendMargin", typeof(Thickness), typeof(ShapeFileLayer), new PropertyMetadata(new Thickness(0, 0, 0, 0)));
        #endregion

        #region LegendHorizontalAlignment
        internal HorizontalAlignment LegendHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LegendHorizontalAlignmentProperty); }
            set { SetValue(LegendHorizontalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendHorizontalAlignment.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LegendHorizontalAlignmentProperty =
            DependencyProperty.Register("LegendHorizontalAlignment", typeof(HorizontalAlignment), typeof(ShapeFileLayer), new PropertyMetadata(HorizontalAlignment.Stretch));
        #endregion

        #region LegendVerticalAlignment
        internal VerticalAlignment LegendVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(LegendVerticalAlignmentProperty); }
            set { SetValue(LegendVerticalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendVerticalAlignment.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LegendVerticalAlignmentProperty =
            DependencyProperty.Register("LegendVerticalAlignment", typeof(VerticalAlignment), typeof(ShapeFileLayer), new PropertyMetadata(VerticalAlignment.Stretch));
        #endregion

        #region LegendIcon
        public LegendIcons LegendIcon
        {
            get { return (LegendIcons)GetValue(LegendIconProperty); }
            set { SetValue(LegendIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendIconProperty =
            DependencyProperty.Register("LegendIcon", typeof(LegendIcons), typeof(ShapeFileLayer), new PropertyMetadata(LegendIcons.Ellipse, OnLegendPropertyChanged));

        private static void OnLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ShapeFileLayer)d;
            instance.LegendRefresh();
        }
        #endregion

        #region LegendType
        public LegendType LegendType
        {
            get { return (LegendType)GetValue(LegendTypeProperty); }
            set { SetValue(LegendTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendTypeProperty =
            DependencyProperty.Register("LegendType", typeof(LegendType), typeof(ShapeFileLayer), new PropertyMetadata(LegendType.Layers, OnLegendPropertyChanged));
        #endregion

        #region LegendHeader
        public string LegendHeader
        {
            get { return (string)GetValue(LegendHeaderProperty); }
            set { SetValue(LegendHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendHeaderProperty =
            DependencyProperty.Register("LegendHeader", typeof(string), typeof(ShapeFileLayer), new PropertyMetadata(string.Empty, OnLegendPropertyChanged));
        #endregion

        #region LegendWidth
        internal Thickness LegendWidth
        {
            get { return (Thickness)GetValue(LegendWidthProperty); }
            set { SetValue(LegendWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendWidthProperty =
            DependencyProperty.Register("LegendWidth", typeof(Thickness), typeof(ShapeFileLayer), new PropertyMetadata(new Thickness(0, 0, 0, 0)));
        #endregion

        #region LegendColumnSplit
        public int LegendColumnSplit
        {
            get { return (int)GetValue(LegendColumnSplitProperty); }
            set { SetValue(LegendColumnSplitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendColumnSplitProperty =
            DependencyProperty.Register("LegendColumnSplit", typeof(int), typeof(ShapeFileLayer), new PropertyMetadata(1, OnLegendPropertyChanged));
        #endregion

        #region ActualTemplate
        /// <summary>
        /// Gets the actual template applied on the MapItems.
        /// </summary>
        /// <remarks>
        /// This is read only property to read the Template of the MapItems.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public DataTemplate ActualTemplate
        {
            get { return (DataTemplate)GetValue(ActualTemplateProperty); }
            set { SetValue(ActualTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualTemplateProperty =
            DependencyProperty.Register("ActualTemplate", typeof(DataTemplate), typeof(ShapeFileLayer), new PropertyMetadata(null));
        #endregion

        #region ShapeIDTableField
        /// <summary>
        /// Gets or sets dbf table field name to identify or map the value with shapes in the SfMap.
        /// </summary>
        /// <remarks>
        /// Use this property to set the Field name of the dbf file's table to map with items values. The associate dbf file must contain the field with this Property value.         
        /// </remarks>
        /// <value>
        /// Type : <see cref="String"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// namespace MapApp
        /// {
        ///     
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.ShapeIDTableField = "Country";
        ///             layer.Uri = "App2.world1.shp";
        ///             syncMap.Layers.Add(layer);
        ///             this.mainGrid.Children.Add(syncMap);
        ///         }       
        ///     }
        /// }
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public string ShapeIDTableField
        {
            get { return (string)GetValue(ShapeIDTableFieldProperty); }
            set { SetValue(ShapeIDTableFieldProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeIDTableField.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeIDTableFieldProperty =
            DependencyProperty.Register("ShapeIDTableField", typeof(string), typeof(ShapeFileLayer), new PropertyMetadata(string.Empty));
        #endregion

        #region ItemsSource
        /// <summary>
        /// Gets or sets the ItemsSource for the ShapeFileLayer.
        /// </summary>
        /// <remarks>
        /// Use this property to set the ItemsSource for the ShapeFileLayer. ItemsSource items will be bounded to the map shapes.        
        /// </remarks>
        /// <value>
        /// Type : <see cref="object"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        ///
        /// namespace MapApp
        /// {
        ///    
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             syncMap.EnablePan = true;
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.Uri = "MapApp.world1.shp";
        ///             ViewModel viewModel = new ViewModel();
        ///             layer.ItemsSource = viewModel.Models;
        ///             syncMap.Layers.Add(layer);
        ///             this.mainGrid.Children.Add(syncMap);
        ///         }
        ///     }
        ///     public class Weather
        ///     { 
        ///         public int CurrentTemperature { get; set; }
        ///         public int AverageHighTemperature { get; set; }
        ///         public int AverageLowTemperature { get; set; }
        ///         public string Country { get; set; }
        ///         public string Continent { get; set; }
        ///         public string City { get; set; }
        ///         public string WeatherDescription { get; set; }
        ///         public int Humidity { get; set; }
        ///         public string Longitude { get; set; }
        ///         public string Latitude { get; set; }
        /// 
        ///         public static List<![CDATA[Weather]]> GetWeatherData()
        ///         {
        ///             List<![CDATA[Weather]]> weatherCollection = new List<![CDATA[Weather]]>();
        ///             weatherCollection.Add(new Weather() { Humidity = 86, CurrentTemperature = 44, AverageHighTemperature = 63, AverageLowTemperature = 46, City = "Chicago", Continent = "North America", Country = "United States", WeatherDescription = "Partly Cloudy", Latitude = "41.8500N", Longitude = "87.6500W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 94, CurrentTemperature = 77, AverageHighTemperature = 89, AverageLowTemperature = 75, City = "Chennai", Continent = "Asia", Country = "India", WeatherDescription = "Rainy", Latitude = "12.5810N", Longitude = "76.0740E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 60, CurrentTemperature = 70, AverageHighTemperature = 70, AverageLowTemperature = 57, City = "Tokyo", Continent = "Asia", Country = "Japan", WeatherDescription = "Partly Cloudy", Latitude = "35.6833N", Longitude = "139.7667E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 72, CurrentTemperature = 55, AverageHighTemperature = 47, AverageLowTemperature = 38, City = "Moscow", Continent = "Asia", Country = "Russia", WeatherDescription = "Clear", Latitude = "55.7517N", Longitude = "37.6178E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 70, CurrentTemperature = 53, AverageHighTemperature = 69, AverageLowTemperature = 54, City = "Cape Town", Continent = "Africa", Country = "South Africa", WeatherDescription = "Partly Cloudy", Latitude = "33.9767S", Longitude = "18.4244E" });
        ///             weatherCollection.Add(new Weather() { Humidity = 77, CurrentTemperature = 64, AverageHighTemperature = 69, AverageLowTemperature = 56, City = "Anchorage", Continent = "North America", Country = "United States", WeatherDescription = "Mostly Cloudy", Latitude = "61.1919N", Longitude = "149.7621W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 55, CurrentTemperature = 91, AverageHighTemperature = 95, AverageLowTemperature = 74, City = "Panama", Continent = "South America", Country = "Republic Of  Panama", WeatherDescription = "Fair", Latitude = "8.7515N", Longitude = "79.8772W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 88, CurrentTemperature = 61, AverageHighTemperature = 76, AverageLowTemperature = 59, City = "Sao Paulo", Continent = "South America", Country = "Brazil", WeatherDescription = "Fair", Latitude = "23.5000S", Longitude = "46.6167W" });
        ///             weatherCollection.Add(new Weather() { Humidity = 83, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Cairo", Continent = "Africa", Country = "Egypt", WeatherDescription = "Mostly Cloudy", Latitude = "31.2262E", Longitude = "30.0566N" });
        ///             weatherCollection.Add(new Weather() { Humidity = 78, CurrentTemperature = 70, AverageHighTemperature = 85, AverageLowTemperature = 72, City = "Melbourne", Continent = "Oceania", Country = "Australia", WeatherDescription = "Cloudy", Latitude = "35.0833S", Longitude = "142.0667E" });
        ///             return weatherCollection;
        ///         }
        ///     }
        ///     public class ViewModel
        ///     {
        ///         public Listt<![CDATA[Weather]]>  Models
        ///         {
        ///             get;
        ///             set;
        ///         }
        /// 
        ///         public ViewModel()
        ///         {
        /// 
        ///             this.Models = new Listt<![CDATA[Weather]]>();
        ///             this.Models = Weather.GetWeatherData();
        /// 
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(ShapeFileLayer), new PropertyMetadata(null, OnItemSourceChanged));

        private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                var layer = d as ShapeFileLayer;
                if (layer.isLoaded)
                {
                    if (e.NewValue != null)
                    {
                        if (e.NewValue is IEnumerable)
                        {
                            layer.MapItems.Clear();
                            layer.Bubbles.Clear();
                            layer.Annotations.Clear();

#if WPF
                            layer.Refresh();

#else
                            layer.MapData((IEnumerable)e.NewValue);
                            if (layer.CustomDataSource != null)
                            {

                                layer.MapCustomDataSource(layer.CustomDataSource);
                                if (layer.CustomDataSource is INotifyCollectionChanged)
                                {
                                    layer.HookCustomSymbolCollectionChanged(layer.CustomDataSource as INotifyCollectionChanged);
                                }
                            }

                            foreach (MapShape shp in layer.MapShapes)
                            {
                                if (shp.value != null)
                                {
                                    layer.FillColors(shp.Shape, shp.value, -1);
                                }
                            }
#endif
                        }
                        else if (e.NewValue is CollectionViewSource)
                        {
                            layer.MapItems.Clear();
                            layer.Bubbles.Clear();
#if WINRT
                            layer.MapData((e.NewValue as CollectionViewSource).View.AsEnumerable());
#else
                            layer.MapData((e.NewValue as CollectionViewSource).View.SourceCollection);
#endif
                            layer.HookCollectionChanged(
                                (e.NewValue as CollectionViewSource).Source as INotifyCollectionChanged);
                        }

#if WPF
                        else if (e.NewValue is DataTable)
                        {
                            layer.MapItems.Clear();
                            layer.Bubbles.Clear();
                            (e.NewValue as DataTable).RowChanged += layer.ShapeFileLayer_RowChanged;
                            layer.MapData((e.NewValue as DataTable).Rows);
                        }
#endif

                        if (e.NewValue is INotifyCollectionChanged)
                        {
                            layer.HookCollectionChanged(e.NewValue as INotifyCollectionChanged);
                        }
                    }
                }
            }
        }
        #endregion

        #region ItemsTemplate
        /// <summary>
        /// Gets or Sets the Template for the MapItems.
        /// </summary>
        /// <remarks>
        /// Use this property to define the Template for the MapItems, which are generated from the ItemsSource.
        /// </remarks>
        /// <value>
        /// Type <see cref="DataTemplate"/>
        /// </value>
        //[ClassReference(IsReviewed = false)]
        public DataTemplate ItemsTemplate
        {
            get { return (DataTemplate)GetValue(ItemsTemplateProperty); }
            set { SetValue(ItemsTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsTemplateProperty =
            DependencyProperty.Register("ItemsTemplate", typeof(DataTemplate), typeof(ShapeFileLayer), new PropertyMetadata(null, OnItemsTemplateChanged));

        private static void OnItemsTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer && e.NewValue != null)
            {
                var layer = d as ShapeFileLayer;
                layer.ActualTemplate = e.NewValue as DataTemplate;
            }
        }
        #endregion

        #region MapItemSetting
        /// <summary>
        /// Gets or Sets the appearance settings when Default Template applied on the MapItems.
        /// </summary>
        /// <remarks>
        /// Default template for the MapItems will show the under bound value of the Shape in a TextBlock.
        /// To set the appearance like Background,FontStyle and FontFamily, this property will be used.
        /// </remarks>
        /// <value>
        /// Type :<see cref="MapItemSetting"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             MapItemSetting mapItemSetting = new MapItemSetting();
        ///             mapItemSetting.MapItemFontFamily = new Windows.UI.Xaml.Media.FontFamily("Times New Roman");
        ///             mapItemSetting.MapItemFontSize = 10d;
        ///             mapItemSetting.MapItemForeground = new SolidColorBrush(Colors.OldLace);
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.MapItemSetting = mapItemSetting;
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);          
        /// 
        ///         }
        /// 
        ///     }
        /// }
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public MapItemSetting MapItemSetting
        {
            get { return (MapItemSetting)GetValue(MapItemSettingProperty); }
            set { SetValue(MapItemSettingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapItemSetting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapItemSettingProperty =
            DependencyProperty.Register("MapItemSetting", typeof(MapItemSetting), typeof(ShapeFileLayer), new PropertyMetadata(null, OnMapItemSettingChange));

        private static void OnMapItemSettingChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer && e.NewValue != null)
            {
                var layer = d as ShapeFileLayer;
                foreach (MapItem item in layer.MapItems)
                {
                    item.Setting = (MapItemSetting)e.NewValue;
                }
            }
        }
        #endregion

        #region MapItems

        /// <summary>
        /// Get the MapItems of the ShapeFileLayer.
        /// </summary>
        /// <remarks>
        /// This is read only property to get the MapItems elements.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public ObservableCollection<MapItem> MapItems
        {
            get { return (ObservableCollection<MapItem>)GetValue(MapItemsProperty); }
            internal set { SetValue(MapItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapItemsProperty =
            DependencyProperty.Register("MapItems", typeof(ObservableCollection<MapItem>), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region CrossCursorVisibility

        /// <summary>
        /// Gets visibility of the CrossCursor shown when Selecting Multiple Shapes in the SfMap.
        /// </summary>
        /// <remarks>
        /// This is the read only property to get the visibility of the CrossCursor, shown on MultiSelection of the SfMap.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public Visibility CrossCursorVisibility
        {
            get { return (Visibility)GetValue(CrossCursorVisibilityProperty); }
            internal set { SetValue(CrossCursorVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CrossCursorVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CrossCursorVisibilityProperty =
            DependencyProperty.Register("CrossCursorVisibility", typeof(Visibility), typeof(ShapeFileLayer), new PropertyMetadata(Visibility.Collapsed));




        #endregion

        #region CrossCursorStroke


        /// <summary>
        /// Gets or sets the Stroke of the CrossCusrsor.
        /// </summary>
        /// <remarks>
        /// Use this property to set the Cross cursor's color.
        /// </remarks>
        /// <value>
        /// Type : <see cref="Brush"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.EnableMultiSelection = true;
        ///             layer.CrossCursorStroke = new SolidColorBrush(Colors.OliveDrab);
        ///             layer.Uri = "MapApp.world1.shp";
        ///             syncMap.Layers.Add(layer);          
        /// 
        ///         }
        /// 
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public Brush CrossCursorStroke
        {
            get { return (Brush)GetValue(CrossCursorStrokeProperty); }
            set { SetValue(CrossCursorStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CrossCursorStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CrossCursorStrokeProperty =
            DependencyProperty.Register("CrossCursorStroke", typeof(Brush), typeof(ShapeFileLayer), new PropertyMetadata(new SolidColorBrush(Colors.Black)));




        #endregion

        #region CrossCursorStrokeThickness


        /// <summary>
        /// Gets or Sets the thickness of the CrossCursor.
        /// </summary>
        /// <remarks>
        /// Use this property to set the thickness of the CrossCursor.
        /// </remarks>
        /// <value>
        /// Type : <see cref="double"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();           
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.EnableMultiSelection = true;
        ///             layer.CrossCursorStroke = new SolidColorBrush(Colors.OliveDrab);
        ///             layer.CrossCursorStrokeThickness = 1d;
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);          
        /// 
        ///         }
        /// 
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public double CrossCursorStrokeThickness
        {
            get { return (double)GetValue(CrossCursorStrokeThicknessProperty); }
            set { SetValue(CrossCursorStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CrossCursorStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CrossCursorStrokeThicknessProperty =
            DependencyProperty.Register("CrossCursorStrokeThickness", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(1d));





        #endregion

        #region CustomDataSymbols


        /// <summary>
        /// Gets the CustomDataSymbols of the ShapeFileLayer.
        /// </summary>
        /// <remarks>
        /// CustomDataSymbols are generated when <see cref="CustomDataSource"/> is set. This property is used to get that data symbols.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public ObservableCollection<CustomDataSymbol> CustomDataSymbols
        {
            get { return (ObservableCollection<CustomDataSymbol>)GetValue(CustomDataSymbolsProperty); }
            internal set { SetValue(CustomDataSymbolsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomDataSymbols.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomDataSymbolsProperty =
            DependencyProperty.Register("CustomDataSymbols", typeof(ObservableCollection<CustomDataSymbol>), typeof(ShapeFileLayer), new PropertyMetadata(null));





        #endregion

        #region CursorPosition



        internal Point CursorPosition
        {
            get { return (Point)GetValue(CursorPositionProperty); }
            set { SetValue(CursorPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CursorPosition.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CursorPositionProperty =
            DependencyProperty.Register("CursorPosition", typeof(Point), typeof(ShapeFileLayer), new PropertyMetadata(new Point()));



        #endregion

        #region MapHeight


        /// <summary>
        /// Gets the Height of the Current Layer in the SfMap.
        /// </summary>
        /// <remarks>
        /// This is read only property to get the height of the Layer.
        /// </remarks>
        /// <value>
        /// Type : <see cref="double"/>
        /// </value>
        //[ClassReference(IsReviewed = false)]
        public double LayerHeight
        {
            get { return (double)GetValue(LayerHeightProperty); }
            internal set { SetValue(LayerHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayerHeightProperty =
            DependencyProperty.Register("LayerHeight", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(0d));




        #endregion

        #region MapWidth

        /// <summary>
        /// Gets the Width of the Current Layer in the SfMap.
        /// </summary>
        /// <remarks>
        /// This is read only property to get the width of the Layer.
        /// </remarks>
        /// <value>
        /// Type : <see cref="double"/>
        /// </value>
        //[ClassReference(IsReviewed = false)]
        public double LayerWidth
        {
            get { return (double)GetValue(LayerWidthProperty); }
            internal set { SetValue(LayerWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayerWidthProperty =
            DependencyProperty.Register("LayerWidth", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(0d));

        #endregion

        #region MapPopupVisibility


        /// <summary>
        /// Gets or Sets the Visibility of the MapPopup.
        /// </summary>
        /// <remarks>
        /// Use this property to set the Visibility of the MapPopup.
        /// </remarks>
        /// <value>
        /// Type : <see cref="Visibility"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.MapPopupVisibility = Visibility.Visible;
        ///             layer.Uri = "MapApp.world1.shp";
        ///             syncMap.Layers.Add(layer);
        /// 
        ///         }
        /// 
        ///     }
        /// }
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public Visibility MapPopupVisibility
        {
            get { return (Visibility)GetValue(MapPopupVisibilityProperty); }
            set { SetValue(MapPopupVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapPopupVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPopupVisibilityProperty =
            DependencyProperty.Register("MapPopupVisibility", typeof(Visibility), typeof(ShapeFileLayer), new PropertyMetadata(Visibility.Collapsed));




        #endregion

        #region MapPopupObject



        /// <summary>
        /// Gets the Binded object of the MapPopup
        /// </summary>
        /// <remarks>
        /// This is read only property to get the Bounded object of the MapPopup.
        /// </remarks>
        /// <value>
        /// Type :<see cref="Object"/>
        /// </value>
        //[ClassReference(IsReviewed = false)]
        public object MapPopupObject
        {
            get { return GetValue(MapPopupObjectProperty); }
            internal set { SetValue(MapPopupObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPopupObjectProperty =
            DependencyProperty.Register("MapPopupObject", typeof(object), typeof(ShapeFileLayer), new PropertyMetadata(null, OnPopupObjectChanged));

        private static void OnPopupObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                var layer = d as ShapeFileLayer;
                if (e.NewValue == null)
                {
                    layer.PopupVisibility = Visibility.Collapsed;

                    layer.MapPopupTemplate = null;
                }
                else
                {
                    layer.PopupVisibility = Visibility.Visible;
                    if (layer.PopupCustomTemplate == null)
                    {
                        layer.MapPopupTemplate = resources["DefaultPopupTemplate"] as DataTemplate;
                    }
                    else
                    {
                        layer.MapPopupTemplate = layer.PopupCustomTemplate;
                    }
                }
            }
        }
        #endregion

        #region CustomPopupTemplate


        /// <summary>
        /// Gets or sets the Custom Template for the MapPopup.
        /// </summary>
        /// <remarks>
        /// Use this property to set the CustomTemplate for the MapPopup.
        /// </remarks>        
        /// <value>
        /// Type :<see cref="DataTemplate"/>
        /// </value>
        //[ClassReference(IsReviewed = false)]
        public DataTemplate PopupCustomTemplate
        {
            get { return (DataTemplate)GetValue(PopupCustomTemplateProperty); }
            set { SetValue(PopupCustomTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupCustomTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupCustomTemplateProperty =
            DependencyProperty.Register("PopupCustomTemplate", typeof(DataTemplate), typeof(ShapeFileLayer), new PropertyMetadata(null, OnPopupCustomTemplateChanged));

        private static void OnPopupCustomTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer && e.NewValue != null)
            {
                var layer = d as ShapeFileLayer;
                layer.MapPopupTemplate = (DataTemplate)e.NewValue;
            }
        }
        #endregion

        #region MapPopupTemplate



        internal DataTemplate MapPopupTemplate
        {
            get { return (DataTemplate)GetValue(MapPopupTemplateProperty); }
            set { SetValue(MapPopupTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPopupTemplateProperty =
            DependencyProperty.Register("MapPopupTemplate", typeof(DataTemplate), typeof(ShapeFileLayer), new PropertyMetadata(null));




        #endregion

        #region Annotations


        /// <summary>
        /// Gets or sets the Annotations for the ShapFileLayer.
        /// </summary>
        /// <remarks>
        /// Use this property to add the Annotations for the ShapeFileLayer.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             MapAnnotations annotation = new MapAnnotations();
        ///             annotation.AnnotationSymbol = new Ellipse { Height=20,Width=20, Fill=new SolidColorBrush(Colors.PaleTurquoise)};
        ///             layer.Annotations.Add(annotation);
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);
        ///         }
        /// 
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public ObservableCollection<MapAnnotations> Annotations
        {
            get { return (ObservableCollection<MapAnnotations>)GetValue(AnnotationsProperty); }
            set { SetValue(AnnotationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomSymbols.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnnotationsProperty =
            DependencyProperty.Register("Annotations", typeof(ObservableCollection<MapAnnotations>), typeof(ShapeFileLayer), new PropertyMetadata(null));




        #endregion

        #region VisbleShapes


        /// <summary>
        /// Gets the visible shapes of the ShapeFileLayer.
        /// </summary>
        /// <remarks>
        /// This is read only property to get the shapes which are visible on the map.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public ObservableCollection<MapShape> VisibleShapes
        {
            get { return (ObservableCollection<MapShape>)GetValue(VisibleShapesProperty); }
            internal set { SetValue(VisibleShapesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleShapes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleShapesProperty =
            DependencyProperty.Register("VisibleShapes", typeof(ObservableCollection<MapShape>), typeof(ShapeFileLayer), new PropertyMetadata(null));




        #endregion

        #region EnableSelection


        /// <summary>
        /// Enables or Disables the shapes to be selected.
        /// </summary>
        /// <remarks>
        /// Use this property to enable or disable the shape selection in the map.If value is True then MapShapes can be selected.
        /// Otherwise cannot.
        /// </remarks>
        /// <value>
        /// Type : <see cref="bool"/>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.EnableSelection = true;
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);
        ///         }
        /// 
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        /// </value>
        //[ClassReference(IsReviewed = false)]
        public bool EnableSelection
        {
            get { return (bool)GetValue(EnableSelectionProperty); }
            set { SetValue(EnableSelectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableSelectionProperty =
            DependencyProperty.Register("EnableSelection", typeof(bool), typeof(ShapeFileLayer), new PropertyMetadata(false));




        #endregion

        #region EnableMultiSelection


        /// <summary>
        /// Enables or Disables the multiple shapes can be selected.
        /// </summary>
        /// <remarks>
        /// Use this property to enable or disable the multi shape selection in the map.If value is True then MapShapes can be selected.
        /// Otherwise cannot.
        /// </remarks>
        /// <value>
        /// Type : <see cref="bool"/>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.EnableMultiSelection = true;
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);
        ///         }
        /// 
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        /// </value>
        //[ClassReference(IsReviewed = false)]
        public bool EnableMultiSelection
        {
            get { return (bool)GetValue(EnableMultiSelectionProperty); }
            set { SetValue(EnableMultiSelectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableMultiSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableMultiSelectionProperty =
            DependencyProperty.Register("EnableMultiSelection", typeof(bool), typeof(ShapeFileLayer), new PropertyMetadata(false, OnEnableMultiSelectionChanged));

        private static void OnEnableMultiSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                var layer = d as ShapeFileLayer;
                if (layer.mapControl != null)
                {
                    if ((bool)e.NewValue)
                    {
#if WINRT
                        if (layer.mapControl.MapView.Equals(MapViews.NormalView))
                        {
                            layer.SetBinding(CrossCursorVisibilityProperty, new Binding { Source = layer, Path = new PropertyPath("EnableMultiSelection"), Converter = new BooleanToVisibilityConverter() });
                        }
                        else
                        {
                            layer.CrossCursorVisibility = Visibility.Collapsed;
                        }
#else
                        layer.SetBinding(CrossCursorVisibilityProperty,
                            new Binding
                            {
                                Source = layer,
                                Path = new PropertyPath("EnableMultiSelection"),
                                Converter = new BooleanToVisibilityConverter()
                            });
#endif

                    }
                }
            }
        }
        #endregion

        #region TranslateZoomLevel

        public Int32 TranslateZoomLevel
        {
            get { return (Int32)GetValue(TranslateZoomLevelProperty); }
            set
            {
                if (!(value <= 0))
                {
                    SetValue(TranslateZoomLevelProperty, value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for TranslateZoomFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TranslateZoomLevelProperty =
            DependencyProperty.Register("TranslateZoomLevel", typeof(Int32), typeof(ShapeFileLayer), new PropertyMetadata(1));

        #endregion

        #region ZoomFactor



        internal double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(0.2d));




        #endregion

        #region SelectionRect



        internal Rect SelectionRect
        {
            get { return (Rect)GetValue(SelectionRectProperty); }
            set { SetValue(SelectionRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionRect.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SelectionRectProperty =
            DependencyProperty.Register("SelectionRect", typeof(Rect), typeof(ShapeFileLayer), new PropertyMetadata(new Rect()));




        #endregion

        #region SelectionRectStroke
        /// <summary>
        /// Gets or Sets the Stroke Color of the Selection rectangle.
        /// </summary>
        /// <remarks>
        /// Use this property to set the Stroke color of the Selection rect.
        /// </remarks>
        /// <value>
        /// Type :<see cref="Brush"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.EnableMultiSelection = true;
        ///             layer.SelectionRectStroke = new SolidColorBrush(Colors.PeachPuff);
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);
        ///         }
        /// 
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public Brush SelectionRectStroke
        {
            get { return (Brush)GetValue(SelectionRectStrokeProperty); }
            set { SetValue(SelectionRectStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionRectStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionRectStrokeProperty =
            DependencyProperty.Register("SelectionRectStroke", typeof(Brush), typeof(ShapeFileLayer), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        #endregion

        #region SelectionRectStrokeThickness
        /// <summary>
        /// Gets or Sets the thickness of the Selection rectangle.
        /// </summary>
        /// <remarks>
        /// Use this property to set the thickness of the Selection rectangel.
        /// </remarks>
        /// <value>
        /// Type :<see cref="Brush"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.EnableMultiSelection = true;
        ///             layer.SelectionRectStrokeThickness = 2d;
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);
        ///         }
        /// 
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public double SelectionRectStrokeThickness
        {
            get { return (double)GetValue(SelectionRectStrokeThicknessProperty); }
            set { SetValue(SelectionRectStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionRectStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionRectStrokeThicknessProperty =
            DependencyProperty.Register("SelectionRectStrokeThickness", typeof(double), typeof(ShapeFileLayer), new PropertyMetadata(0.75));


        #endregion

        #region SelectedMapShapes


        /// <summary>
        /// Gets the Selected shapes of the ShapeFileLayer
        /// </summary>
        /// <remarks>
        /// This is read only property to get the selected shapes in the ShapeFileLayer.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public ObservableCollection<MapShape> SelectedMapShapes
        {
            get { return (ObservableCollection<MapShape>)GetValue(SelectedMapShapesProperty); }
            internal set { SetValue(SelectedMapShapesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedMapShapes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedMapShapesProperty =
            DependencyProperty.Register("SelectedMapShapes", typeof(ObservableCollection<MapShape>), typeof(ShapeFileLayer), new PropertyMetadata(null));



        #endregion

        #region ShapeIDPath


        /// <summary>
        /// Gets or set the Identification property name of the Shape.
        /// </summary>
        /// <remarks>
        /// Use this property to set the Property name from the ItemsSource's Item to identify the corresponding under bound shape.
        /// </remarks>
        /// <value>
        /// Type :<see cref="String"/>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Windows.UI.Xaml.Shapes;
        /// 
        /// 
        /// namespace MapApp
        /// {
        /// 
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();
        ///             ShapeFileLayer layer = new ShapeFileLayer();
        ///             layer.ShapeIDPath="Country"
        ///             layer.Uri = "MapApp.world.shp";
        ///             syncMap.Layers.Add(layer);
        ///         }
        /// 
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        /// </value>
        //[ClassReference(IsReviewed = false)]
        public string ShapeIDPath
        {
            get { return (string)GetValue(ShapeIDPathProperty); }
            set { SetValue(ShapeIDPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeIDPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeIDPathProperty =
            DependencyProperty.Register("ShapeIDPath", typeof(string), typeof(ShapeFileLayer), new PropertyMetadata(string.Empty));



        #endregion

        #region CustomDataSource


        /// <summary>
        /// Gets or Sets the CustomDataSource for ShapeFileLayer.
        /// </summary>
        /// <remarks>
        /// Use this property to bound the object to a point in the map based on Latitude and Longitude.
        /// </remarks>
        /// <value>
        /// Type : <see cref="IEnumerable"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public IEnumerable CustomDataSource
        {
            get { return (IEnumerable)GetValue(CustomDataSourceProperty); }
            set { SetValue(CustomDataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomDataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomDataSourceProperty =
            DependencyProperty.Register("CustomDataSource", typeof(IEnumerable), typeof(ShapeFileLayer), new PropertyMetadata(null));





        #endregion

        #region MapShapes

        /// <summary>
        /// Gets the  MapShapes of the ShapeFileLayer.       
        /// </summary>
        /// <remarks>
        /// This is the read only property to get the MapShapes of the ShapeFileLayer.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public ObservableCollection<MapShape> MapShapes
        {
            get { return (ObservableCollection<MapShape>)GetValue(MapShapesProperty); }
            set { SetValue(MapShapesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapShapes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapShapesProperty =
            DependencyProperty.Register("MapShapes", typeof(ObservableCollection<MapShape>), typeof(ShapeFileLayer), new PropertyMetadata(null));

        #endregion

        #region ShapeDataPath
        //[ClassReference(IsReviewed = false)]
        public string ShapeDataPath
        {
            get { return (string)GetValue(ShapeDataTableFieldProperty); }
            set { SetValue(ShapeDataTableFieldProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeDataPath.
        public static readonly DependencyProperty ShapeDataTableFieldProperty =
            DependencyProperty.Register("ShapeDataPath", typeof(string), typeof(ShapeFileLayer), new PropertyMetadata(string.Empty, OnShapeDataPathChanged));

        private static void OnShapeDataPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                var layer = d as ShapeFileLayer;
                if (layer.GetType() == typeof(ShapeFileLayer))
                {
                    if (!string.IsNullOrEmpty((string)e.NewValue))
                    {
                        if (layer.ShapeData != null)
                        {
                            layer.UpdateReader();
                        }
                    }
                }
            }
        }
        #endregion

        #region ShapeData
        //[ClassReference(IsReviewed = false)]
        public object ShapeData
        {
            get { return GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShapeDatasItem Source Collection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register("ShapeData", typeof(object), typeof(ShapeFileLayer), new PropertyMetadata(null, OnShapeDataChanged));

        private static void OnShapeDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                var layer = d as ShapeFileLayer;
                if (layer.GetType() == typeof(ShapeFileLayer))
                {
                    if (e.NewValue != null)
                    {
                        if (!string.IsNullOrEmpty(layer.ShapeDataPath))
                        {
                            layer.UpdateReader();
                        }
                    }
                }
            }
        }
        #endregion

        #region LabelPath
        public string LabelPath
        {
            get { return (string)GetValue(LabelPathProperty); }
            set { SetValue(LabelPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelPathProperty =
            DependencyProperty.Register("LabelPath", typeof(string), typeof(ShapeFileLayer), new PropertyMetadata(string.Empty, OnLabelPathChanged));

        private static void OnLabelPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                ShapeFileLayer shapeFileLayer = (d as ShapeFileLayer);
                if (shapeFileLayer.isLoaded)
                {
                    shapeFileLayer.MapItems.Clear();
                    shapeFileLayer.SetDBFDataLabels();
                }
            }
        }
        #endregion

        #region Uri
        /// <summary>
        /// Gets or sets the embedded location of the Shape File.
        /// </summary>
        /// <remarks>
        /// Use this property to set the embedded resource location of the shape file.
        /// </remarks>
        /// <value>
        /// Type :<see cref="String"/>
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///    
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             SfMap syncMap = new SfMap();        
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";           
        ///             syncMap.Layers.Add(layer);            
        ///         }       
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Uri.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.Register("Uri", typeof(string), typeof(ShapeFileLayer), new PropertyMetadata(string.Empty, OnUriChanged));

        private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                var layer = d as ShapeFileLayer;
                if (layer.GetType() == typeof(ShapeFileLayer))
                {
                    if (layer.mapControl == null)
                    {
                        layer.mapControl = SfMap.FindParent<SfMap>(layer);
                    }
                    if (e.NewValue.ToString() != string.Empty)
                    {
                        if (System.IO.Path.GetExtension(layer.Uri) == ".kml")
                        {
                            layer.isKmlLayer = true;
                            layer.kmlReader = new ShapeFileKmlReader();
                            Stream kmlStream =
                                Application.Current.GetType()
                                    .GetTypeInfo()
                                    .Assembly.GetManifestResourceStream(e.NewValue.ToString());
                            layer.LoadKmlStream(kmlStream);
                        }
                        else
                        {
                            layer.shapeReader = new ShapeFileReader(layer.Uri);
                            layer.shapeReader.ReadShapeFile();
                            layer.dbfReader = new ShapeFileDBFReader();
                            layer.dbfReader.ReadDBFData(layer.Uri);
#if WPF || WINRT
                            layer.shapeReader.ShapeFileDownloaded += layer.shapeReader_ShapeFileDownloaded;
                            layer.dbfReader.DBFFileDownloaded += layer.dbfReader_DBFFileDownloaded;
#endif
                            layer.MapRect =
                                new Rect(new Point(layer.shapeReader.Header.MinX, layer.shapeReader.Header.MinY),
                                    new Point(layer.shapeReader.Header.MaxX, layer.shapeReader.Header.MaxY));
                        }
                        if (layer.isLoaded)
                        {
                            layer.Refresh();
                        }
                    }
                }
                else
                {
                    if (System.IO.Path.GetExtension(layer.Uri) == ".kml")
                    {
                        layer.isKmlLayer = true;
                        layer.kmlReader = new ShapeFileKmlReader();
                        Stream kmlStream =
                            Application.Current.GetType()
                                .GetTypeInfo()
                                .Assembly.GetManifestResourceStream(e.NewValue.ToString());
                        layer.LoadKmlStream(kmlStream);
                    }
                    else
                    {
                        layer.shapeReader = new ShapeFileReader(layer.Uri);
                        layer.shapeReader.ReadShapeFile();
                        layer.dbfReader = new ShapeFileDBFReader();
                        layer.dbfReader.ReadDBFData(layer.Uri);
                    }
                }
            }
        }

        public void LoadKmlStream(Stream kmlStream)
        {
            if (kmlStream != null)
            {
                kmlReader = new ShapeFileKmlReader();
                kmlReader.ReadKMLStream(kmlStream, this, isBaseLayer);
                kmlReader.BoundingBox.SetBoundingBoxValues(isBaseLayer, kmlReader);
                MapRect = new Rect(new Point(kmlReader.BoundingBox.West, kmlReader.BoundingBox.South), new Point(kmlReader.BoundingBox.East, kmlReader.BoundingBox.North));
            }
        }

#if WPF || WINRT
        void dbfReader_DBFFileDownloaded(object sender, ProgressChangedEventArgs e)
        {
            Refresh();
        }

        void shapeReader_ShapeFileDownloaded(object sender, ProgressChangedEventArgs e)
        {
            MapRect = new Rect(new Point(shapeReader.Header.MinX, shapeReader.Header.MinY), new Point(shapeReader.Header.MaxX, shapeReader.Header.MaxY));
            if (isLoaded)
            {
                Refresh();
            }
        }
#endif

        #endregion

        #region HideIntersectLabels
        /// <summary>
        /// Gets or sets a value indicating whether the intersected labels should be hidden.
        /// </summary>
        public bool HideIntersectLabels
        {
            get { return (bool)GetValue(HideIntersectLabelsProperty); }
            set { SetValue(HideIntersectLabelsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HideIntersectLabels.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HideIntersectLabelsProperty =
            DependencyProperty.Register("HideIntersectLabels", typeof(bool), typeof(ShapeFileLayer), new PropertyMetadata(true, OnHideIntersectLabelsChanged));

        private static void OnHideIntersectLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShapeFileLayer)
            {
                ShapeFileLayer shapeFileLayer = d as ShapeFileLayer;
                if(shapeFileLayer.isLoaded)
                    shapeFileLayer.Refresh();
            }
        } 
        #endregion

        #endregion

        #region Events

        #region ShapeSelected

        /// <summary>
        /// Occurs when Shapes in the Shape file layer is  Selected. 
        /// </summary>
        /// <remarks>
        /// This event will triggered when a shape or shapes are selected in the ShapeFileLayer. The argument contains the List of selected shapes.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();           
        ///             SfMap syncMap = new SfMap();        
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.ShapesSelected += layer_ShapesSelected;
        ///             syncMap.Layers.Add(layer);
        ///             
        ///         }
        /// 
        ///         void layer_ShapesSelected(object sender, SelectionEventArgs args)
        ///         {
        ///              var selectedItems = args.Items; 
        ///         }   
        ///         
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public event SelectionEventHandler ShapesSelected;

        internal void OnShapesSelected(object sender, SelectionEventArgs args)
        {
            if (ShapesSelected != null)
            {
                ShapesSelected(sender, args);
            }
        }

        #endregion

        #region ShapesUnSelected

        /// <summary>
        /// Occurs when Shapes in the Shape file layer is Un Selected. 
        /// </summary>
        /// <remarks>
        /// This event will triggered when a shape or shapes are unselected  in the ShapeFileLayer. The argument contains the List of unselected shapes.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();           
        ///             SfMap syncMap = new SfMap();        
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.ShapesUnSelected += layer_ShapesUnSelected;
        ///             syncMap.Layers.Add(layer);
        ///             
        ///         }
        /// 
        ///         void layer_ShapesUnSelected(object sender, SelectionEventArgs args)
        ///         {
        ///             var selectedItems = args.Items;
        ///         }        
        ///         
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public event SelectionEventHandler ShapesUnSelected;

        internal void OnShapesUnSelected(object sender, SelectionEventArgs args)
        {
            if (ShapesUnSelected != null)
            {
                ShapesUnSelected(sender, args);
            }
        }

        #endregion

        #endregion

        #region InternalProperties

        #region MapPopupMargin
        internal Thickness MapPopupMargin
        {
            get { return (Thickness)GetValue(MapPopupMarginProperty); }
            set
            {
                SetValue(MapPopupMarginProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for MapPopupMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPopupMarginProperty =
            DependencyProperty.Register("MapPopupMargin", typeof(Thickness), typeof(ShapeFileLayer), new PropertyMetadata(new Thickness(0, 0, 0, 0)));
        #endregion

        #region PopupVisibility
        internal Visibility PopupVisibility
        {
            get { return (Visibility)GetValue(PopupVisibilityProperty); }
            set { SetValue(PopupVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupVisibilityProperty =
            DependencyProperty.Register("PopupVisibility", typeof(Visibility), typeof(ShapeFileLayer), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.ShapeFileLayer">ShapeFileLayer</see> class. 
        /// </summary>
        //[ClassReference(IsReviewed = false)]
        public ShapeFileLayer()
        {
            DefaultStyleKey = typeof(ShapeFileLayer);
            Background = new SolidColorBrush(Color.FromArgb(255, 251, 251, 251));
            MapShapes = new ObservableCollection<MapShape>();
            Legends = new ObservableCollection<Legend>();
            ViewTransform = new TransformGroup();
            ZoomTransform = new ScaleTransform { ScaleX = 1, ScaleY = 1 };
            PanTransform = new TranslateTransform { X = 0, Y = 0 };
            ViewTransform.Children.Add(ZoomTransform);
            ViewTransform.Children.Add(PanTransform);
            SelectedMapShapes = new ObservableCollection<MapShape>();
            VisibleShapes = new ObservableCollection<MapShape>();
            CustomDataSymbols = new ObservableCollection<CustomDataSymbol>();
            CustomDataSymbols.CollectionChanged += CustomDataSymbols_CollectionChanged;
            SelectionRect = new Rect();
            SetBinding(CrossCursorVisibilityProperty, new Binding { Source = this, Path = new PropertyPath("EnableMultiSelection"), Converter = new BooleanToVisibilityConverter() });
            Bubbles = new ObservableCollection<Bubble>();
            MapPoints = new ObservableCollection<MapPoint>();
            if (AnnotationTemplate == null)
            {
                AnnotationTemplate = resources["DefaultAnnotationTemplate"] as DataTemplate;
            }
            ShapeSettings = new ShapeSetting { layer = this };
            CurrentColorPalette = new ObservableCollection<MapColorPalette>();
            SetColorPalette(ShapeSettings.ColorPalette);

            if (PopupCustomTemplate == null)
            {
                MapPopupTemplate = resources["DefaultPopupTemplate"] as DataTemplate;
            }
            else
            {
                MapPopupTemplate = PopupCustomTemplate;
            }
            MapItems = new ObservableCollection<MapItem>();
            Annotations = new ObservableCollection<MapAnnotations>();
            Annotations.CollectionChanged += Annotations_CollectionChanged;
            SelectedMapShapes.CollectionChanged += SelectedMapShapes_CollectionChanged;
#if WPF
            AnimationTransfrom = new TransformGroup();
            AnimationTransfrom.Children.Add(new ScaleTransform(1, 1));
            AnimationTransfrom.Children.Add(new TranslateTransform(0, 0));
#else
            AnimationTransfrom = new CompositeTransform { ScaleX = 1, ScaleY = 1, TranslateX = 0, TranslateY = 0 };
#endif
            RenderTransform = AnimationTransfrom;
            SubShapeFileLayers = new ObservableCollection<SubShapeFileLayer>();
            SubShapeFileLayers.CollectionChanged += SubShapeFileLayers_CollectionChanged;
            MapItemSetting = new MapItemSetting();
        }

        void SubShapeFileLayers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SubShapeFileLayer sublayer in e.NewItems)
                {
                    sublayer.isBaseLayer = false;
                    sublayer.baselayer = this;
                }
            }
        }

        void CustomDataSymbols_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CustomDataSymbol csymbol in CustomDataSymbols)
                {
                    BindingOperations.SetBinding(csymbol, CustomDataSymbol.CustomDataSymbolTemplateProperty, new Binding { Source = this, Path = new PropertyPath("CustomDataSourceTemplate") });
                }
            }
        }

        void SelectedMapShapes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (MapShape shp in e.NewItems)
                    {
                        shp.isSelected = true;
                        shp.tempFill = shp.Shape.Fill;
                        shp.Shape.SetBinding(Shape.FillProperty, new Binding { Source = ShapeSettings, Path = new PropertyPath("SelectedShapeColor") });
                    }
                    var args = new SelectionEventArgs(SelectedMapShapes);
                    OnShapesSelected(this, args);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                removedItems.Clear();
                if (e.OldItems != null)
                {
                    foreach (MapShape shp in e.OldItems)
                    {
                        shp.isSelected = false;
                        shp.Shape.Fill = shp.tempFill;
                        removedItems.Add(shp);
                    }
                }
                shapeCount = 0;
                foreach (MapShape shp in MapShapes)
                {
                    FillColors(shp.Shape, shp.ColorValue, -1);
                }
                var args = new SelectionEventArgs(removedItems);
                OnShapesUnSelected(this, args);
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                shapeCount = 0;
                removedItems.Clear();
                if (e.OldItems != null)
                {
                    foreach (MapShape shp in e.OldItems)
                    {
                        shp.isSelected = false;
                        shp.Shape.Fill = shp.tempFill;
                        removedItems.Add(shp);
                    }
                }
                var args = new SelectionEventArgs(removedItems);
                OnShapesUnSelected(this, args);
            }
        }

#if WINRT

        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            dragStartPoint = e.GetCurrentPoint(this).Position;
            shapeCount = 0;
            int count = SelectedMapShapes.Count;
            if (!(e.OriginalSource is Path))
            {
                MapPopupObject = null;
                if (SelectedMapShapes.Count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        SelectedMapShapes.RemoveAt(((count - 1) - i));
                    }
                }
            }
            base.OnPointerPressed(e);
        }

        protected override void OnPointerWheelChanged(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            mapControl.scrollContent.HorizontalScrollMode = ScrollMode.Disabled;
            mapControl.scrollContent.VerticalScrollMode = ScrollMode.Disabled;
            base.OnPointerWheelChanged(e);
        }

        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            draggingPoint = e.GetCurrentPoint(this).Position;
            CursorPosition = e.GetCurrentPoint(this).Position;
            base.OnPointerMoved(e);
        }

        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.draggingPoint.X = 0;
            this.draggingPoint.Y = 0;
            this.SelectionRect = new Rect(0, 0, 0, 0);
            this.mapControl.isPointerPressed = false;
            this.mapControl.isMousePointerPressed = false;
            if (this.tempSelectedShapes.Count > 0)
            {
                this.SelectedMapShapes.Clear();
                foreach (MapShape shp in this.tempSelectedShapes)
                {
                    this.SelectedMapShapes.Add(shp);
                }
                this.tempSelectedShapes.Clear();
            }
            base.OnPointerReleased(e);
        }
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(this);
            shapeCount = 0;
            int count = SelectedMapShapes.Count;
            if (!(e.OriginalSource is Path))
            {
                if (SelectedMapShapes.Count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        SelectedMapShapes.RemoveAt(((count - 1) - i));
                    }
                }
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            draggingPoint = e.GetPosition(this);
            CursorPosition = e.GetPosition(this);
            base.OnMouseMove(e);
        }

#if WINDOWSPHONE || WINDOWSPHONE_8
        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            draggingPoint.X = 0;
            draggingPoint.Y = 0;
            SelectionRect = new Rect(0, 0, 0, 0);
            if (tempSelectedShapes.Count > 0)
            {

                SelectedMapShapes.Clear();
                foreach (MapShape shp in tempSelectedShapes)
                {
                    SelectedMapShapes.Add(shp);
                }
                tempSelectedShapes.Clear();
            }

        }
#else
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            draggingPoint.X = 0;
            draggingPoint.Y = 0;
            SelectionRect = new Rect(0, 0, 0, 0);
            if (tempSelectedShapes.Count > 0)
            {

                SelectedMapShapes.Clear();
                foreach (MapShape shp in tempSelectedShapes)
                {
                    SelectedMapShapes.Add(shp);
                }
                tempSelectedShapes.Clear();
            }
            base.OnMouseLeftButtonUp(e);
        }
#endif


#endif

        void Annotations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (MapAnnotations annotation in e.NewItems)
                    {
                        SetCustomSymbolMargin(annotation, annotation.midpoint);
                        if (annotation.AnnotationTemplate == null)
                        {
                            BindingOperations.SetBinding(annotation, MapAnnotations.AnnotationTemplateProperty, new Binding { Source = this, Path = new PropertyPath("AnnotationTemplate") });
                        }
                    }
                }
            }
        }




        #endregion

        internal ItemsControl grid = null;
        #region Override Methods
#if WINRT
        protected override void OnApplyTemplate()
        {
#else
        public override void OnApplyTemplate()
        {
#endif
            base.OnApplyTemplate();
            mapControl = SfMap.FindParent<SfMap>(this);
            grid = GetTemplateChild("Control1") as ItemsControl;
            if (grid != null)
                grid.SizeChanged += grid_SizeChanged;
            shapeGrid = GetTemplateChild("PART_ShapeFileGrid") as Grid;
            mapItemsControl = GetTemplateChild("PART_MapItemsControl") as ItemsControl;
            mapPopup = GetTemplateChild("PART_MapPopupContent") as ContentControl;
        }

        private void LegendRefresh()
        {
            SetLegendPosition();
            Legends.Clear();
            if (LegendType == LegendType.Bubbles)
            {
                if (BubbleMarkerSetting != null && BubbleMarkerSetting.ColorMappings.Count > 0)
                {
                    BubbleMarkerSetting.mapControl = mapControl;
                    Legends.Clear();
                    foreach (ColorMapping colorMapping in BubbleMarkerSetting.ColorMappings)
                    {
                        var legend = new Legend();
                        double colorValue = 0d;
                        if (colorMapping is RangeColorMapping)
                        {
                            legend.LegendLabel = (colorMapping as RangeColorMapping).Range.ToString();
                            colorValue = (colorMapping as RangeColorMapping).Range;
                        }
                        else if (colorMapping is EqualsColorMapping)
                        {
                            legend.LegendLabel = (colorMapping as EqualsColorMapping).Value.ToString();
                            colorValue = Convert.ToDouble((colorMapping as EqualsColorMapping).Value);
                        }
                        double bubbleSize = BubbleMarkerSetting.GetBubbleSize(Convert.ToDouble(colorValue));
                        legend.LegendIcon = BubbleMarkerSetting.GetBubbleItem(bubbleSize, colorValue);
                        Legends.Add(legend);
                    }
                }
            }
            else
            {
                if (ShapeSettings.FillSetting.ColorMappings.Count > 0)
                {
                    Legends.Clear();
                    foreach (ColorMapping colorMapping in ShapeSettings.FillSetting.ColorMappings)
                    {
                        if (LegendIcon == LegendIcons.Ellipse)
                        {
                            var legend = new Legend
                            {
                                LegendIcon = new Ellipse
                                {
                                    Height = 15,
                                    Width = 15,
                                    Fill = new SolidColorBrush(colorMapping.Color)
                                }
                            };
                            if (colorMapping is RangeColorMapping)
                            {
                                legend.LegendLabel = (colorMapping as RangeColorMapping).Range.ToString();
                            }
                            else if (colorMapping is EqualsColorMapping)
                            {
                                legend.LegendLabel = (colorMapping as EqualsColorMapping).Value.ToString();
                            }
                            Legends.Add(legend);
                        }
                        if (LegendIcon == LegendIcons.Rectangle)
                        {
                            var legend = new Legend
                            {
                                LegendIcon = new Rectangle
                                {
                                    Height = 15,
                                    Width = 15,
                                    Fill = new SolidColorBrush(colorMapping.Color)
                                }
                            };
                            if (colorMapping is RangeColorMapping)
                            {
                                legend.LegendLabel = (colorMapping as RangeColorMapping).Range.ToString();
                            }
                            else if (colorMapping is EqualsColorMapping)
                            {
                                legend.LegendLabel = (colorMapping as EqualsColorMapping).Value.ToString();
                            }
                            Legends.Add(legend);
                        }
                    }
                }
            }
        }

        private void SetLegendPosition()
        {
            switch (LegendPosition)
            {
                case LegendPosition.TopLeft:
                    LegendMargin = new Thickness();
                    LegendVerticalAlignment = VerticalAlignment.Top;
                    LegendHorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case LegendPosition.TopCenter:
                    LegendMargin = new Thickness();
                    LegendVerticalAlignment = VerticalAlignment.Top;
                    LegendHorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case LegendPosition.TopRight:
                    LegendMargin = new Thickness();
                    LegendVerticalAlignment = VerticalAlignment.Top;
                    LegendHorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case LegendPosition.MidLeft:
                    LegendMargin = new Thickness();
                    LegendVerticalAlignment = VerticalAlignment.Center;
                    LegendHorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case LegendPosition.Center:
                    LegendMargin = new Thickness();
                    LegendVerticalAlignment = VerticalAlignment.Center;
                    LegendHorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case LegendPosition.MidRight:
                    LegendMargin = new Thickness();
                    LegendVerticalAlignment = VerticalAlignment.Center;
                    LegendHorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case LegendPosition.BottomLeft:
                    LegendMargin = new Thickness();
                    LegendVerticalAlignment = VerticalAlignment.Bottom;
                    LegendHorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case LegendPosition.BottomCenter:
                    LegendMargin = new Thickness();
                    LegendVerticalAlignment = VerticalAlignment.Bottom;
                    LegendHorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case LegendPosition.BottomRight:
                    LegendMargin = new Thickness();
                    LegendVerticalAlignment = VerticalAlignment.Bottom;
                    LegendHorizontalAlignment = HorizontalAlignment.Right;
                    break;
                default:
                    LegendMargin = new Thickness { Left = LegendPositionX, Top = LegendPositionY };
                    LegendVerticalAlignment = VerticalAlignment.Stretch;
                    LegendHorizontalAlignment = HorizontalAlignment.Stretch;
                    break;
            }
        }



        void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mapControl != null)
            {
                shapeGrid.Width = LayerWidth = mapControl.ActualWidth;
                shapeGrid.Height = LayerHeight = mapControl.ActualHeight;
            }
            if (isBaseLayer)
            {
                Refresh();
            }

            isLoaded = true;
            RenderSubLayer();

        }
#if  !WINDOWSPHONE && ! SILVERLIGHT && !SILVERLIGHT_5 && !WPF
        private async void RenderSubLayer()
        {
            await Task.Delay(1);
            foreach (SubShapeFileLayer layer in SubShapeFileLayers)
            {
                layer.Refresh();
            }

        }
#else
        private void RenderSubLayer()
        {
            foreach (SubShapeFileLayer layer in SubShapeFileLayers)
            {
                layer.Refresh();
            }
        }
#endif
        #endregion

        #region HelperMethods

        internal void SetCustomDataSymbolMargin(CustomDataSymbol dataSymbol, Point latlonPoint)
        {
#if WINRT
            var marginPoint = ViewTransform.TransformPoint(latlonPoint);
#else
            var marginPoint = ViewTransform.Transform(latlonPoint);
#endif

            dataSymbol.Margin = new Thickness(marginPoint.X, marginPoint.Y, 0, 0);
        }


        internal void SetCustomSymbolMargin(MapAnnotations symbol, Point latlonPoint)
        {

#if WINRT
            var marginPoint = ViewTransform.TransformPoint(latlonPoint);
#else
            var marginPoint = ViewTransform.Transform(latlonPoint);
#endif

            symbol.AnnotationMargin = new Thickness(marginPoint.X, marginPoint.Y, 0, 0);
        }

        internal Point GetMapElementsPosition(Point latLonPt)
        {
            if (ShapeTransform != null)
            {
                if (ShapeTransform != null)
                {
#if WINRT
                    var p2 = ShapeTransform.TransformPoint(latLonPt);
#else
                    var p2 = ShapeTransform.Transform(latLonPt);
#endif

                    var roundedPt = new Point(((p2.X * ZoomTransform.ScaleX) + PanTransform.X), ((p2.Y * ZoomTransform.ScaleY) + PanTransform.Y));
                    return roundedPt;
                }
            }

            return new Point();
        }

        internal void MapCustomDataSource(IEnumerable customDataSource)
        {
            if (customDataSource != null)
            {
                foreach (object obj in customDataSource)
                {
                    object latitude = "null";
                    object longitude = "null";
                    PropertyInfo latitudeproperty = obj.GetType().GetTypeInfo().GetDeclaredProperty("Latitude");
                    if (latitudeproperty != null)
                    {
                        latitude = latitudeproperty.GetValue(obj, null);
                    }
                    PropertyInfo longitudeproperty = obj.GetType().GetTypeInfo().GetDeclaredProperty("Longitude");
                    if (longitudeproperty != null)
                    {
                        longitude = longitudeproperty.GetValue(obj, null);
                    }
                    if (obj is INotifyPropertyChanged)
                    {
                        (obj as INotifyPropertyChanged).PropertyChanged += CustomDataObject_PropertyChanged;
                    }
                    var symbol = new CustomDataSymbol
                    {
                        Data = obj,
                        Latitude = SfMap.ConvertLattideLongitudeValues(latitude.ToString()),
                        Longitude = SfMap.ConvertLattideLongitudeValues(longitude.ToString())
                    };
                    var marginPoint = LatitudeLongitudeToPoint(new Point(symbol.Longitude, symbol.Latitude));
                    //symbol.CustomDataSymbolTemplate = CustomDataSourceTemplate;       
                    symbol.midpoint = marginPoint;
                    SetCustomDataSymbolMargin(symbol, marginPoint);
                    CustomDataSymbols.Add(symbol);

                }
            }
        }

        private void CustomDataObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CustomDataSymbols.Clear();
            MapCustomDataSource(CustomDataSource);
        }

        private void MapData(IEnumerable source)
        {
            string shpID = ShapeIDTableField;
            var objs = new List<object>();
            if (ItemsSource != null)
            {
                if (dbfReader != null && dbfReader.DBFData != null)
                {
                    if (dbfReader.DBFData.DBFFields.Count > 0)
                    {
                        var sel = this.dbfReader.DBFData.DBFFields.Select(data => data.Where(item => item.Key == shpID)).Select(val => val.Any() ? val.First().Value : null);

                        if (BubbleMarkerSetting != null)
                        {
                            BubbleMarkerSetting.shapeFileLayer = this;
                            BubbleMarkerSetting.FindSizeRatio(source);
                            BubbleMarkerSetting.mapControl = mapControl;
                        }
                        if (sel.Count() > 0)
                        {
                            foreach (object obj in source)
                            {
                                object shapeid = "Null";
                                object shapeValue = "Null";
                                object shapeColorValue = null;
                                object bubbleColorValue = null;
                                object bubbleValue = null;
                                PropertyInfo shapeIdProperty = null;
#if WPF
                                if (obj is DataRow)
                                {
                                    if ((ItemsSource as DataTable).Columns.Contains(ShapeIDPath))
                                    {
                                        shapeid = (obj as DataRow)[ShapeIDPath];
                                        shapeValue = (ItemsSource as DataTable).Columns.Contains(ShapeSettings.ShapeValuePath) ? (obj as DataRow)[ShapeSettings.ShapeValuePath] : null;
                                        if(string.IsNullOrEmpty(ShapeSettings.ShapeColorValuePath))
                                        {
                                            shapeColorValue = shapeValue;
                                        }
                                        else
                                        {
                                            shapeColorValue = (ItemsSource as DataTable).Columns.Contains(ShapeSettings.ShapeColorValuePath) ? (obj as DataRow)[ShapeSettings.ShapeColorValuePath] : null;
                                        }
                                        if (BubbleMarkerSetting != null)
                                        {
                                            if ((ItemsSource as DataTable).Columns.Contains(BubbleMarkerSetting.ValuePath))
                                            {
                                                bubbleValue = (obj as DataRow)[BubbleMarkerSetting.ValuePath];
                                                objs.Add(bubbleValue);
                                            }
                                            if ((ItemsSource as DataTable).Columns.Contains(BubbleMarkerSetting.ColorValuePath))
                                            {
                                                bubbleColorValue = (obj as DataRow)[BubbleMarkerSetting.ColorValuePath];

                                            }
                                            if (bubbleColorValue == null)
                                            {
                                                bubbleColorValue = bubbleValue;
                                            }


                                        }
                                    }

                                }
                                else
                                {
                                    shapeIdProperty = obj.GetType().GetTypeInfo().GetDeclaredProperty(ShapeIDPath);


                                    if (shapeIdProperty != null)
                                    {
                                        shapeid = shapeIdProperty.GetValue(obj, null);
                                    }
                                    PropertyInfo shapeVal = obj.GetType().GetTypeInfo().GetDeclaredProperty(ShapeSettings.ShapeValuePath);
                                    if (shapeVal != null)
                                    {
                                        shapeValue = shapeVal.GetValue(obj, null);
                                    }
                                    else
                                    {
                                        shapeValue = null;
                                        ActualTemplate = null;
                                    }
                                    PropertyInfo shapeColorVal = obj.GetType().GetTypeInfo().GetDeclaredProperty(ShapeSettings.ShapeColorValuePath);
                                    shapeColorValue = shapeColorVal != null ? shapeColorVal.GetValue(obj, null) : shapeValue;
                                    if (BubbleMarkerSetting != null)
                                    {
                                        PropertyInfo bubbleVal = obj.GetType().GetTypeInfo().GetDeclaredProperty(BubbleMarkerSetting.ValuePath);
                                        if (bubbleVal != null)
                                        {
                                            bubbleValue = bubbleVal.GetValue(obj, null);
                                        }
                                        PropertyInfo bubbleColorVal = obj.GetType().GetTypeInfo().GetDeclaredProperty(BubbleMarkerSetting.ColorValuePath);
                                        bubbleColorValue = bubbleColorVal != null ? bubbleColorVal.GetValue(obj, null) : bubbleValue;
                                        objs.Add(bubbleValue);
                                    }
                                }

#else
                                shapeIdProperty = obj.GetType().GetTypeInfo().GetDeclaredProperty(ShapeIDPath);
                                if (shapeIdProperty != null)
                                {
                                    shapeid = shapeIdProperty.GetValue(obj, null);
                                }
                                PropertyInfo shapeVal = obj.GetType().GetTypeInfo().GetDeclaredProperty(ShapeSettings.ShapeValuePath);
                                if (shapeVal != null)
                                {
                                    shapeValue = shapeVal.GetValue(obj, null);
                                }
                                else
                                {
                                    shapeValue = null;
                                    ActualTemplate = null;
                                }
                                PropertyInfo shapeColorVal = obj.GetType().GetTypeInfo().GetDeclaredProperty(ShapeSettings.ShapeColorValuePath);
                                if (shapeColorVal != null)
                                {
                                    shapeColorValue = shapeColorVal.GetValue(obj, null);
                                }
                                else
                                {
                                    shapeColorValue = shapeValue;
                                }
                                if (BubbleMarkerSetting != null)
                                {
                                    PropertyInfo bubbleVal = obj.GetType().GetTypeInfo().GetDeclaredProperty(BubbleMarkerSetting.ValuePath);
                                    if (bubbleVal != null)
                                    {
                                        bubbleValue = bubbleVal.GetValue(obj, null);
                                    }
                                    PropertyInfo bubbleColorVal = obj.GetType().GetTypeInfo().GetDeclaredProperty(BubbleMarkerSetting.ColorValuePath);
                                    bubbleColorValue = bubbleColorVal != null ? bubbleColorVal.GetValue(obj, null) : bubbleValue;
                                    objs.Add(bubbleValue);
                                }

#endif
                                if (shapeid.ToString() != "Null" && sel.Contains(shapeid.ToString()))
                                {
                                    int index = sel.ToList().IndexOf(shapeid.ToString());
                                    MapShapes[index].DataContext = obj;
                                    MapShapes[index].ShapeValue = shapeValue;
                                    MapShapes[index].shapeValueIndex = index;
                                    MapShapes[index].value = shapeValue;
                                    MapShapes[index].ColorValue = shapeColorValue;
                                    Point pt = FindMidPointOfPolygon(index);
#if WINRT
                                    Point pt1 = ViewTransform.TransformPoint(pt);
#else
                                    Point pt1 = ViewTransform.Transform(pt);
#endif
                                    var item = new MapItem();
                                    item.index = index;
#if WPF
                                    if (obj is DataRow)
                                    {
                                        item.Data = obj as DataRow;
                                    }
                                    else
                                    {
                                        item.Data = obj;
                                    }
#else
                                    item.Data = obj;
#endif
                                    DBFValues dbfValues = dbfReader.DBFData.DBFFields[index];
                                    foreach (DBFFieldValues dbfFieldValues in dbfValues)
                                    {
                                        item.DBFData.Add(dbfFieldValues.Key, dbfFieldValues.Value);
                                    }
                                    item.Setting = MapItemSetting;
                                    item.MapItemValue = shapeValue;
                                    if (ItemsTemplate != null)
                                        BindingOperations.SetBinding(item, MapItem.TemplateProperty, new Binding { Source = this, Path = new PropertyPath("ItemsTemplate") });
                                    else
                                        item.Template = resources["DefaultLabelTemplate"] as DataTemplate;
                                    MapItems.Add(item);
                                    item.Margin = new Thickness(pt1.X, pt1.Y, 0, 0);
                                    item.mapItemPoint = pt;
                                    if (obj is INotifyPropertyChanged)
                                    {
                                        (obj as INotifyPropertyChanged).PropertyChanged += ShapeFileLayer_PropertyChanged;
                                    }
                                    if (BubbleMarkerSetting != null)
                                    {
                                        var bubble = new Bubble
                                        {
                                            BubbleValue = bubbleValue,
                                            BubbleColorValue = bubbleColorValue,
                                            midpoint = pt,
                                            index = index
                                        };
                                        double bubbleSize = BubbleMarkerSetting.GetBubbleSize(Convert.ToDouble(bubble.BubbleValue));
                                        bubble.BubbleItem = BubbleMarkerSetting.GetBubbleItem(bubbleSize, Convert.ToDouble(bubble.BubbleColorValue));
                                        bubble.Margin = new Thickness(pt.X - bubbleSize / 2, pt.Y - bubbleSize / 2, 0, 0);
                                        Bubbles.Add(bubble);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            mapItemsPanel = SfMap.FindChild<MapItemsPanel>(mapItemsControl, "mapItemsPanel");
        }

        void ShapeFileLayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Bubbles.Clear();
            MapItems.Clear();
            if (ItemsSource is CollectionViewSource)
            {
#if WINRT
                MapData((ItemsSource as CollectionViewSource).View.AsEnumerable());
#else
                MapData((ItemsSource as CollectionViewSource).View.SourceCollection);
#endif
            }
            else
            {
                MapData(ItemsSource as IEnumerable);
            }

        }

        internal MaxMin FindMaxMid(List<int> lst)
        {
            var maxMin = new MaxMin();
            var diff = 0;
            for (int i = 0; i < lst.Count - 1; i++)
            {
                if (diff < (lst[i + 1] - lst[i]))
                {
                    maxMin.Max = lst[i];
                    maxMin.Max = lst[i + 1];
                    diff = (lst[i + 1] - lst[i]);
                }
            }
            return maxMin;
        }

        internal void SetElementsMargin()
        {
            Measure(new Size(LayerHeight, LayerWidth));
#if WINDOWSPHONE || WINDOWSPHONE_8
            ViewTransform = new TransformGroup();
            ViewTransform.Children.Add(ZoomTransform);
            ViewTransform.Children.Add(PanTransform);
#endif
            foreach (MapItem item in MapItems)
            {
#if WINRT
                var pt = ViewTransform.TransformPoint(item.mapItemPoint);
#else
                var pt = ViewTransform.Transform(item.mapItemPoint);
#endif
                item.Margin = new Thickness(pt.X, pt.Y, 0, 0);
            }
            foreach (Bubble bubble in Bubbles)
            {
#if WINRT
                var pt = ViewTransform.TransformPoint(bubble.midpoint);
#else
                var pt = ViewTransform.Transform(bubble.midpoint);
#endif

                bubble.Margin = new Thickness(pt.X - bubble.BubbleItem.Width / 2, pt.Y - bubble.BubbleItem.Height / 2, 0, 0);

            }
            foreach (CustomDataSymbol symbol in CustomDataSymbols)
            {
                SetCustomDataSymbolMargin(symbol, symbol.midpoint);
            }
            foreach (MapPoint point in MapPoints)
            {
#if WINRT
                var pt = ViewTransform.TransformPoint(ShapeTransform.TransformPoint(point.mappt));
#else
                var pt = ViewTransform.Transform(ShapeTransform.Transform(point.mappt));
#endif

                point.Margin = point.PointMargin = new Thickness(pt.X, pt.Y, 0, 0);
                if (PointData is MapPoint)
                {
                    if (point.PointData.Equals((PointData as MapPoint).PointData))
                    {
                        MapPointMargin = point.PointMargin;
                    }
                }


            }
            foreach (SubShapeFileLayer layer in SubShapeFileLayers)
            {
                foreach (MapItem item in layer.MapItems)
                {
#if WINRT
                    var pt = ViewTransform.TransformPoint(item.mapItemPoint);
#else
                    var pt = ViewTransform.Transform(item.mapItemPoint);
#endif
                    //item.RevMargin = new Thickness(-pt.X,- pt.Y, 0, 0);
                    item.Margin = new Thickness(pt.X, pt.Y, 0, 0);
                }
                foreach (Bubble bubble in layer.Bubbles)
                {
#if WINRT
                    var pt = ViewTransform.TransformPoint(bubble.midpoint);
#else
                    var pt = ViewTransform.Transform(bubble.midpoint);
#endif

                    bubble.Margin = new Thickness(pt.X - bubble.BubbleItem.Width / 2, pt.Y - bubble.BubbleItem.Height / 2, 0, 0);

                }
                foreach (CustomDataSymbol symbol in layer.CustomDataSymbols)
                {

                    SetCustomDataSymbolMargin(symbol, symbol.midpoint);
                }
                foreach (MapPoint point in layer.MapPoints)
                {
#if WINRT
                    var pt = ViewTransform.TransformPoint(ShapeTransform.TransformPoint(point.mappt));
#else
                    var pt = ViewTransform.Transform(ShapeTransform.Transform(point.mappt));
#endif
                    point.Margin = point.PointMargin = new Thickness(pt.X, pt.Y, 0, 0);
                    if (layer.PointData is MapPoint)
                    {
                        if (point.PointData.Equals((layer.PointData as MapPoint).PointData))
                        {
                            layer.MapPointMargin = point.PointMargin;
                        }
                    }

                }
            }
        }

        private Geometry CreatePathGeometry(ShapeFileRecord record, object path)
        {

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
#if WINRT
                Point pt = ShapeTransform.TransformPoint(points[0]);
#else
                Point pt = ShapeTransform.Transform(points[0]);
#endif

                figure.StartPoint = pt;
                var polylinesegment = new PolyLineSegment();

                for (int l = 1; l < points.Count(); l++)
                {
#if WINRT
                    var pts = ShapeTransform.TransformPoint(points[l]);
#else
                    var pts = ShapeTransform.Transform(points[l]);
#endif

                    polylinesegment.Points.Add(pts);
                }

                //var ptss = DouglasPeuckerReduction(polylinesegment.Points.ToList<Point>(), 0.5);
                //polylinesegment.Points.Clear();
                //foreach (Point pointt in ptss)
                //{
                //    polylinesegment.Points.Add(pointt);
                //}

                figure.Segments.Add(polylinesegment);
                geometry.Figures.Add(figure);
            }

            return geometry;
        }

        private Geometry CreatePathGeometry(KmlPolygon polygon)
        {
            var geometry = new PathGeometry();
            if (ShapeTransform != null)
            {
                var outerBoundary = polygon.OuterBoundary;
                if (outerBoundary != null)
                {
                    geometry.Figures.Add(CreatePathFigure(outerBoundary));
                    var innerboundaryList = polygon.InnerBoundaryList;
                    if (innerboundaryList != null && innerboundaryList.Count > 0)
                    {
                        foreach (KmlBoundary innerBoundary in innerboundaryList)
                        {
                            geometry.Figures.Add(CreatePathFigure(innerBoundary));
                        }
                    }
                }
            }
            return geometry;
        }

        private PathFigure CreatePathFigure(KmlBoundary boundary)
        {
            var figure = new PathFigure();
#if WINRT
            Point startPoint = ShapeTransform.TransformPoint(boundary.CoordinatePoints[0]);
#else
            Point startPoint = ShapeTransform.Transform(boundary.CoordinatePoints[0]);
#endif
            figure.StartPoint = startPoint;
            var polylinesegment = new PolyLineSegment();
            for (int i = 1; i < boundary.CoordinatePoints.Count; i++)
            {
#if WINRT
                var point = ShapeTransform.TransformPoint(boundary.CoordinatePoints[i]);
#else
                var point = ShapeTransform.Transform(boundary.CoordinatePoints[i]);
#endif
                polylinesegment.Points.Add(point);
            }
            figure.Segments.Add(polylinesegment);
            figure.IsFilled = true;
            return figure;
        }

        private MapShape CreateMapShape(Geometry geometry, KmlPlacemark placemark)
        {
            var mapPath = new Path
            {
                Data = geometry,
                RenderTransform = isBaseLayer ? ViewTransform : baselayer.ViewTransform
            };
            var mapShape = new MapShape
            {
                Shape = mapPath,
                isKmlPolygon = true,
                Placemark = placemark
            };
            return mapShape;
        }

        private Path CreateShape(ShapeFileRecord record, Geometry geometry)
        {
            var path = new Path();
            path.Data = geometry;
            if (isBaseLayer)
            {
                path.RenderTransform = ViewTransform;
            }
            else
            {
                path.RenderTransform = baselayer.ViewTransform;
            }
            FillColors(path, null, -1);
            return path;
        }

        private Geometry CreatePathGeometry(ShapeFileRecord record)
        {
            var geometry = CreatePathGeometry(record, null);
            return geometry;
        }
        private void RenderRecords(ShapeFileReader shapeFileReader)
        {
            shapeType = shapeFileReader.Header.ShapeType;
            for (int i = 0; i < shapeFileReader.Records.Count; i++)
            {
                if (shapeFileReader.Records[i].ShapeType == ShapeType.Polygon)
                {
                    Geometry geometry = CreatePathGeometry(shapeFileReader.Records[i]);
                    MapShape shape = new MapShape();
                    shape.Shape = CreateShape(shapeFileReader.Records[i], geometry);
                    MapShapes.Add(shape);
                    VisibleShapes.Add(shape);
                }
                else
                {
                    MapPoint mpt = new MapPoint();
                    mpt.mappt = shapeFileReader.Records[i].Points[0];
#if WINRT
                    var pt = ShapeTransform.TransformPoint(mpt.mappt);
#else
                    var pt = ShapeTransform.Transform(mpt.mappt);
#endif

                    mpt.Margin = mpt.PointMargin = new Thickness(pt.X, pt.Y, 0, 0);

                    BindingOperations.SetBinding(mpt, MapPoint.ActualTemplateProperty, new Binding { Source = this, Path = new PropertyPath("MapPointTemplate") });
                    BindingOperations.SetBinding(mpt, MapPoint.PointPopupTemplateProperty, new Binding { Source = this, Path = new PropertyPath("MapPointPopupTemplate"), Mode = BindingMode.TwoWay });
                    if (pointValues != null && pointValues.Count > 0)
                    {
                        mpt.TooltipText = pointValues[i].ToString();
                    }
                    if (dbfReader.DBFData != null)
                    {
                        foreach (DBFFieldValues val in dbfReader.DBFData.DBFFields[i])
                        {
                            mpt.PointData.Add(val.Key, val.Value);
                        }
                    }
                    mpt.shapeLayer = this;
                    MapPoints.Add(mpt);
                    if (pointsPanel == null)
                    {
                        pointsPanel = SfMap.FindChild<MapItemsPanel>(shapeGrid, "PointsPanel");
                    }
                }
            }
        }

        private TransformGroup CreateShapeTransform(ShapeFileReader info)
        {
            // Bounding box for the shapefile.
            if (isBaseLayer)
            {
                minX = info.Header.MinX;
                maxX = info.Header.MaxX;
                minY = info.Header.MinY;
                maxY = info.Header.MaxY;
            }
            else
            {
                minX = baselayer.minX;
                maxX = baselayer.maxX;
                minY = baselayer.minY;
                maxY = baselayer.maxY;
            }
            var xformGroup = SetScaleTransformGroup();
            return xformGroup;
        }

        private TransformGroup CreateShapeTransform(KmlBoundingBox boundingBox)
        {
            // Bounding box for the kml file.
            if (isBaseLayer)
            {
                minX = boundingBox.West;
                maxX = boundingBox.East;
                minY = boundingBox.South;
                maxY = boundingBox.North;
            }
            else
            {
                minX = baselayer.minX;
                maxX = baselayer.maxX;
                minY = baselayer.minY;
                maxY = baselayer.maxY;
            }
            var transformGroup = SetScaleTransformGroup();
            return transformGroup;
        }

        internal TransformGroup SetScaleTransformGroup()
        {
            // Width and height of the bounding box.
            var width = Math.Abs(maxX - minX);
            var height = Math.Abs(maxY - minY);

            // Aspect ratio of the bounding box.
            var aspectRatio = width / height;

            // Aspect ratio of the canvas after loaded. Loaded size has been used to maintain the size while save and load the SfMap
            var canvasRatio = avilabelMapSize.Width / avilabelMapSize.Height;

            // Compute a scale factor so that the shapefile geometry
            // will maximize the space used on the canvas while still
            // maintaining its aspect ratio.
            double scaleFactor;
            if (aspectRatio < canvasRatio)
            {
                scaleFactor = avilabelMapSize.Height / height;
            }
            else
            {
                scaleFactor = avilabelMapSize.Width / width;
            }


            // Compute the scale transformation. Note that we flip
            // the Y-values because the lon/lat grid is like a cartesian
            // coordinate system where Y-values increase upwards.
            var scaleTransform = new ScaleTransform();
            if (!double.IsInfinity(scaleFactor))
            {
                scaleTransform.ScaleX = scaleFactor;
                scaleTransform.ScaleY = -scaleFactor;
            }

            // Compute the translate transformation so that the shapefile
            // geometry will be centered on the canvas.
            var translateTransform = new TranslateTransform();
            translateTransform.X = (avilabelMapSize.Width - (minX + maxX) * scaleFactor) / 2;
            translateTransform.Y = (avilabelMapSize.Height + (minY + maxY) * scaleFactor) / 2;

            // Add the two transforms to a transform group.
            var xformGroup = new TransformGroup();
            xformGroup.Children.Add(scaleTransform);
            xformGroup.Children.Add(translateTransform);
            return xformGroup;
        }

        internal Point FindMidPointOfPolygon(int index, ShapeFileLayer layer)
        {
            double A = 0;
            var maxMin = new MaxMin { Max = layer.shapeReader.Records[index].Points.Count, Min = 0 };
            if (layer.shapeReader.Records[index].Parts.Count > 1)
            {
                var diff = 0;
                var lst = layer.shapeReader.Records[index].Parts;
                for (int i = 0; i < lst.Count - 1; i++)
                {
                    if (diff < (lst[i + 1] - lst[i]))
                    {
                        maxMin.Min = lst[i];
                        maxMin.Max = lst[i + 1];
                        diff = (lst[i + 1] - lst[i]);
                    }
                }
                if (maxMin.Max == layer.shapeReader.Records[index].Parts[layer.shapeReader.Records[index].Parts.Count - 1])
                {
                    if (diff < (layer.shapeReader.Records[index].Points.Count - layer.shapeReader.Records[index].Parts[layer.shapeReader.Records[index].Parts.Count - 1]))
                    {
                        maxMin.Min = layer.shapeReader.Records[index].Parts[layer.shapeReader.Records[index].Parts.Count - 1];
                        maxMin.Max = layer.shapeReader.Records[index].Points.Count;
                    }
                }

            }
            for (int i = maxMin.Min; i <= maxMin.Max - 1; i++)
            {
#if WINRT
                var pt = (ShapeTransform.TransformPoint(layer.shapeReader.Records[index].Points[i]));
#else
                var pt = (ShapeTransform.Transform(layer.shapeReader.Records[index].Points[i]));
#endif

                Point pt1;
                if (i == maxMin.Max - 1)
                {
#if WINRT
                    pt1 = (ShapeTransform.TransformPoint(layer.shapeReader.Records[index].Points[maxMin.Min]));
#else
                    pt1 = (ShapeTransform.Transform(layer.shapeReader.Records[index].Points[maxMin.Min]));
#endif

                }
                else
                {
#if WINRT
                    pt1 = (ShapeTransform.TransformPoint(layer.shapeReader.Records[index].Points[i + 1]));
#else
                    pt1 = (ShapeTransform.Transform(layer.shapeReader.Records[index].Points[i + 1]));
#endif

                }
                A = A + (((pt.X * pt1.Y)) - (pt1.X * pt.Y));
            }
            A = 0.5 * A;

            double x = 0;
            double y = 0;
            for (int i = maxMin.Min; i <= maxMin.Max - 1; i++)
            {
#if WINRT
                var pt = (ShapeTransform.TransformPoint(layer.shapeReader.Records[index].Points[i]));
#else
                var pt = (ShapeTransform.Transform(layer.shapeReader.Records[index].Points[i]));
#endif

                Point pt1;
                if (i == maxMin.Max - 1)
                {
#if WINRT
                    pt1 = (ShapeTransform.TransformPoint(layer.shapeReader.Records[index].Points[maxMin.Min]));
#else
                    pt1 = (ShapeTransform.Transform(layer.shapeReader.Records[index].Points[maxMin.Min]));
#endif

                }
                else
                {
#if WINRT
                    pt1 = (ShapeTransform.TransformPoint(layer.shapeReader.Records[index].Points[i + 1]));
#else
                    pt1 = (ShapeTransform.Transform(layer.shapeReader.Records[index].Points[i + 1]));
#endif

                }
                x = x + ((pt.X + pt1.X) * (((pt.X * pt1.Y) - (pt1.X * pt.Y))));
                y = y + ((pt.Y + pt1.Y) * (((pt.X * pt1.Y) - (pt1.X * pt.Y))));
            }
            x = (1 / (6 * A)) * x;
            y = (1 / (6 * A)) * y;
            return new Point(x, y);
        }

        internal Point FindMidPointOfPolygon(int index)
        {
            var pt = FindMidPointOfPolygon(index, this);
            return pt;
        }

        internal void FillColors(Path path, object value, Int32 index)
        {
            path.SetBinding(Shape.StrokeProperty, new Binding { Source = ShapeSettings, Path = new PropertyPath("ShapeStroke") });
            path.SetBinding(Shape.StrokeThicknessProperty, new Binding { Source = ShapeSettings, Path = new PropertyPath("ShapeStrokeThickness"), Mode = BindingMode.TwoWay });
            if (value == null)
            {
                if (!ShapeSettings.FillSetting.AutoFillColors)
                {
                    path.SetBinding(Shape.FillProperty, new Binding { Source = ShapeSettings, Path = new PropertyPath("ShapeFill") });
                }
                else
                {
                    if (CurrentColorPalette.Count > 0)
                    {
                        var mapPallette = CurrentColorPalette[shapeCount];
                        path.SetBinding(Shape.FillProperty, new Binding { Source = mapPallette, Path = new PropertyPath("FillBrush") });
                        shapeCount++;
                        if (shapeCount >= CurrentColorPalette.Count)
                        {
                            shapeCount = 0;
                        }
                    }
                }
            }
            else
            {
                if (ShapeSettings.FillSetting.AutoFillColors == false && ShapeSettings.FillSetting.ColorMappings.Count > 0)
                {
                    if (ShapeSettings.FillSetting.ColorMappings[0] is RangeColorMapping)
                    {
                        path.Fill = new SolidColorBrush(mapControl.GetColor(value, ShapeSettings.FillSetting.ColorMappings, (ShapeSettings.ShapeFill as SolidColorBrush).Color));
                    }
                    else if (ShapeSettings.FillSetting.ColorMappings[0] is ColorMapping)
                    {
                        path.Fill = new SolidColorBrush(mapControl.GetValueColor(value, ShapeSettings.FillSetting.ColorMappings, (ShapeSettings.ShapeFill as SolidColorBrush).Color));
                    }
                }
                else if (ShapeSettings.FillSetting.AutoFillColors == false && ShapeSettings.FillSetting.ColorMappings.Count == 0)
                {
                    path.SetBinding(Shape.FillProperty, new Binding { Source = ShapeSettings, Path = new PropertyPath("ShapeFill") });

                }
                else
                {
                    if (CurrentColorPalette.Count > 0)
                    {
                        var mapPallette = CurrentColorPalette[shapeCount];
                        path.SetBinding(Shape.FillProperty, new Binding { Source = mapPallette, Path = new PropertyPath("FillBrush") });
                        shapeCount++;
                        if (shapeCount >= CurrentColorPalette.Count)
                        {
                            shapeCount = 0;
                        }
                    }
                }
            }
        }

        internal void SetColorPalette(ColorPalettes colorPalette)
        {
            if (CurrentColorPalette == null)
            {
                CurrentColorPalette = new ObservableCollection<MapColorPalette>();
            }
            if (ShapeSettings.ColorPalette != ColorPalettes.CustomPalette)
            {
#if WINRT
                var colors = from obj in ShapeFileLayer.colorPaletteResources.Keys
                             where obj.ToString().Contains(colorPalette.ToString())
                             select obj;
#else
                var colors = new List<object>();
                foreach (object obj in colorPaletteResources.Keys)
                {
                    if (obj.ToString().Contains(colorPalette.ToString()))
                    {
                        colors.Add(obj);
                    }
                }
#endif
                if (CurrentColorPalette.Count != colors.Count())
                {
                    CurrentColorPalette.Clear();
                    foreach (object key in colors)
                    {
                        CurrentColorPalette.Add(new MapColorPalette { FillBrush = colorPaletteResources[key.ToString()] as Brush });
                    }
                }
                else
                {
                    for (int i = 0; i < CurrentColorPalette.Count; i++)
                    {
                        CurrentColorPalette[i].FillBrush = colorPaletteResources[colors.ElementAt(i).ToString()] as Brush;
                    }
                }
            }
            else
            {
                CurrentColorPalette.Clear();
                foreach (MapColorPalette palette in ShapeSettings.CustomColors)
                {
                    CurrentColorPalette.Add(palette);
                }
            }
        }

        /// <summary>
        /// Uses the Douglas Peucker algorithim to reduce the number of points.
        /// </summary>
        /// <param name="Points">The points.</param>
        /// <param name="Tolerance">The tolerance.</param>
        /// <returns></returns>
        internal static List<Point> DouglasPeuckerReduction(List<Point> Points, Double Tolerance)
        {

            if (Points == null || Points.Count < 3)
                return Points;

            const int firstPoint = 0;
            Int32 lastPoint = Points.Count - 1;
            var pointIndexsToKeep = new List<Int32> { firstPoint, lastPoint };

            //Add the first and last index to the keepers


            //The first and the last point can not be the same
            while (Points[firstPoint].Equals(Points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(Points, firstPoint, lastPoint, Tolerance, ref pointIndexsToKeep);

            var returnPoints = new List<Point>();
            pointIndexsToKeep.Sort();
            foreach (Int32 index in pointIndexsToKeep)
            {
                returnPoints.Add(Points[index]);
            }

            return returnPoints;
        }

        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="pointIndexsToKeep">The point indexs to keep.</param>
        private static void DouglasPeuckerReduction(List<Point> points, Int32 firstPoint, Int32 lastPoint, Double tolerance, ref List<Int32> pointIndexsToKeep)
        {
            Double maxDistance = 0;
            Int32 indexFarthest = 0;

            for (Int32 index = firstPoint; index < lastPoint; index++)
            {
                Double distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <summary>
        /// The distance of a point from a line made from point1 and point2.
        /// </summary>
        /// <param name="Point1">The PT1.</param>
        /// <param name="Point2">The PT2.</param>
        /// <param name="Point">The p.</param>
        /// <returns></returns>
        internal static Double PerpendicularDistance(Point Point1, Point Point2, Point Point)
        {
            //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
            //Base = √((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
            //Area = .5*Base*H                                          *Solve for height
            //Height = Area/.5/Base

            Double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X * Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X * Point2.Y - Point1.X * Point.Y));
            Double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + Math.Pow(Point1.Y - Point2.Y, 2));
            Double height = area / bottom * 2;

            return height;

            //Another option
            //Double A = Point.X - Point1.X;
            //Double B = Point.Y - Point1.Y;
            //Double C = Point2.X - Point1.X;
            //Double D = Point2.Y - Point1.Y;

            //Double dot = A * C + B * D;
            //Double len_sq = C * C + D * D;
            //Double param = dot / len_sq;

            //Double xx, yy;

            //if (param < 0)
            //{
            //    xx = Point1.X;
            //    yy = Point1.Y;
            //}
            //else if (param > 1)
            //{
            //    xx = Point2.X;
            //    yy = Point2.Y;
            //}
            //else
            //{
            //    xx = Point1.X + param * C;
            //    yy = Point1.Y + param * D;
            //}

            //Double d = DistanceBetweenOn2DPlane(Point, new Point(xx, yy));
        }

        /// <summary>
        /// Converts the given point to Latitude and Longitude value.
        /// </summary>
        /// <param name="canvasPosition">Points to be converted.</param>
        /// <returns>Latitude and Longitude Values.</returns>
        /// <remarks>
        /// The function will converts the given point coordinates to latitude and longitude values.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///   
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             double x = 200;
        ///             double y = 348;
        ///             SfMap syncMap = new SfMap();        
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";
        ///             Point result = layer.PointToLatitudeLongitude(new Point(x,y));
        ///             syncMap.Layers.Add(layer);                  
        ///         }   
        ///         
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public Point PointToLatitudeLongitude(Point canvasPosition)
        {
            var latLongPt = new Point();
            if (ShapeTransform != null)
            {
                // Convert from canvas position to lon/lat coordinates.
                latLongPt = GetLatLonCoordinates(canvasPosition);
            }

            return latLongPt;
        }

        internal Point GetLatLonCoordinates(Point canvasPosition)
        {
            if (ViewTransform != null && ViewTransform.Inverse != null)
            {
                // Apply the inverse of the view transformation.
#if WINRT
                Point p1 = ViewTransform.Inverse.TransformPoint(canvasPosition);
#else
                Point p1 = ViewTransform.Inverse.Transform(canvasPosition);
#endif

                // Apply the inverse of the shape transformation.
                if (ShapeTransform != null && ShapeTransform.Inverse != null)
                {
#if WINRT
                    Point p2 = ShapeTransform.Inverse.TransformPoint(p1);
#else
                    Point p2 = ShapeTransform.Inverse.Transform(p1);
#endif
                    return p2;
                }
                return p1;
            }
            return new Point();
        }

        /// <summary>
        /// Converts the given Latitude and Longitude value to screen point.
        /// </summary>
        /// <param name="latLonPt">Latitude and Longitude value to be converterd.</param>
        /// <returns>Screen Points</returns>
        /// <remarks>
        /// The function will converts the given latitude and longitude values to point value. Latitude and Longitude need to be given as points.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///   
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             double latitude = 54;
        ///             double longitude = 34;
        ///             SfMap syncMap = new SfMap();        
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";
        ///             Point result = layer.LatitudeLongitudeToPoint(new Point(longitude,latitude));
        ///             syncMap.Layers.Add(layer);                  
        ///         }   
        ///         
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public Point LatitudeLongitudeToPoint(Point latLonPt)
        {
            if (ShapeTransform != null)
            {
                if (ShapeTransform != null)
                {
#if WINRT
                    var p2 = ShapeTransform.TransformPoint(latLonPt);
#else
                    var p2 = ShapeTransform.Transform(latLonPt);
#endif

                    return p2;
                }
            }

            return new Point();

        }

        /// <summary>
        /// Clears all elements from the ShapeFileLayer.
        /// </summary>
        /// <remarks>
        /// This Reset function will clear all the elements from the ShapeFileLayer.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        ///         
        /// namespace MapApp
        /// {    
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();           
        ///             SfMap syncMap = new SfMap();        
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.Reset();
        ///             syncMap.Layers.Add(layer);            
        ///         }   
        ///         
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public void Reset()
        {
            VisibleShapes.Clear();
            MapItems.Clear();
            MapShapes.Clear();
            Bubbles.Clear();
            Legends.Clear();
            MapPoints.Clear();
            //Annotations.Clear();
        }

        /// <summary>
        /// Clears and re-arranges all the elements in the ShapeFileLayer.
        /// </summary>
        /// <remarks>
        /// The Refresh function will clears all the elements in ShapeFileLayer. And re-creates and arranges them on the map.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        ///         
        /// namespace MapApp
        /// {    
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();           
        ///             SfMap syncMap = new SfMap();        
        ///             ShapeFileLayer layer = new ShapeFileLayer();          
        ///             layer.Uri = "MapApp.world1.shp";
        ///             layer.Refresh();
        ///             syncMap.Layers.Add(layer);            
        ///         }   
        ///         
        ///     }
        ///    
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public void Refresh()
        {
            Reset();
            MapPopupObject = null;
            PopupVisibility = Visibility.Collapsed;
            if (isBaseLayer)
            {
                avilabelMapSize = mapControl != null ? new Size(mapControl.ActualWidth, mapControl.ActualHeight) : DesiredSize;
            }
            else
            {
                avilabelMapSize = baselayer.avilabelMapSize;
            }
            if (shapeReader != null)
            {
                ShapeTransform = CreateShapeTransform(shapeReader);
                MapDBFData();
                RenderRecords(shapeReader);
                shapeCount = 0;
                if (ItemsTemplate == null)
                {
                    ActualTemplate = resources["DefaultMapItemTemplate"] as DataTemplate;
                }
                if (ItemsSource != null)
                {
                    if (shapeReader.Header.ShapeType == ShapeType.Polygon)
                    {
                        if (ItemsSource is CollectionViewSource)
                        {
#if WINRT
                            MapData((ItemsSource as CollectionViewSource).View.AsEnumerable());
#else
                            MapData((ItemsSource as CollectionViewSource).View.SourceCollection);
#endif
                            HookCollectionChanged(
                                (ItemsSource as CollectionViewSource).Source as INotifyCollectionChanged);
                        }
                        else
                        {
#if WPF
                            if (ItemsSource is DataTable)
                            {
                                (ItemsSource as DataTable).RowChanged += ShapeFileLayer_RowChanged;
                                MapData((ItemsSource as DataTable).Rows);
                            }
                            else
                            {
                                MapData(ItemsSource as IEnumerable);
                            }
#else
                            MapData(ItemsSource as IEnumerable);
#endif

                        }
                        if (ItemsSource is INotifyCollectionChanged)
                        {
                            HookCollectionChanged(ItemsSource as INotifyCollectionChanged);
                        }
                    }
                }
                SetDBFDataLabels();

                if (CustomDataSource != null)
                {
                    MapCustomDataSource(CustomDataSource);
                    if (CustomDataSource is INotifyCollectionChanged)
                    {
                        HookCustomSymbolCollectionChanged(CustomDataSource as INotifyCollectionChanged);
                    }
                }
                if (Annotations.Count > 0)
                {
                    foreach (MapAnnotations csym in Annotations)
                    {
                        csym.midpoint = LatitudeLongitudeToPoint(new Point(csym.Longitude, csym.Latitude));
                        SetCustomSymbolMargin(csym, csym.midpoint);
                    }
                }

                foreach (MapShape shp in MapShapes)
                {
                    if (shp.value != null)
                    {
                        FillColors(shp.Shape, shp.ColorValue, -1);
                    }
                }
            }
            else if (isKmlLayer)
            {
                RenderKmlElements();
            }
            if (mapControl != null)
            {
                if (mapControl.ZoomLevel > 1)
                {
                    mapControl.Zoom(1 + ((mapControl.ZoomLevel) * ZoomFactor));
                }
            }
            if (EnableMultiSelection)
            {
#if WINRT
                if (mapControl.MapView.Equals(MapViews.NormalView))
                {
                    SetBinding(ShapeFileLayer.CrossCursorVisibilityProperty, new Binding { Source = this, Path = new PropertyPath("EnableMultiSelection"), Converter = new BooleanToVisibilityConverter() });
                }
                else
                {
                    CrossCursorVisibility = Visibility.Collapsed;
                }
#else
                SetBinding(CrossCursorVisibilityProperty, new Binding { Source = this, Path = new PropertyPath("EnableMultiSelection"), Converter = new BooleanToVisibilityConverter() });
#endif
            }
            LegendRefresh();
        }

        private void SetDBFDataLabels()
        {
            if (ItemsSource == null && dbfReader != null && dbfReader.DBFData != null && dbfReader.DBFData.DBFFields.Count > 0)
            {
                if (!string.IsNullOrEmpty(LabelPath))
                {
                    var labels = from inf in dbfReader.DBFData.DBFFields
                                 from item in inf
                                 where item.Key == LabelPath
                                 select item;
                    foreach (var label in labels)
                    {
                        int index = labels.ToList().IndexOf(label);
                        Point midPoint = FindMidPointOfPolygon(index);
#if WINRT
                        Point transformPoint = ViewTransform.TransformPoint(midPoint);
#else
                        Point transformPoint = ViewTransform.Transform(midPoint);
#endif
                        var mapItem = new MapItem
                        {
                            MapItemValue = label.Value,
                            Margin = new Thickness(transformPoint.X, transformPoint.Y, 0, 0),
                            mapItemPoint = midPoint,
                        };
                        DBFValues dbfValues = dbfReader.DBFData.DBFFields[index];
                        foreach (DBFFieldValues dbfFieldValues in dbfValues)
                        {
                            mapItem.DBFData.Add(dbfFieldValues.Key, dbfFieldValues.Value);
                        }
                        if (ItemsTemplate != null)
                            BindingOperations.SetBinding(mapItem, MapItem.TemplateProperty, new Binding { Source = this, Path = new PropertyPath("ItemsTemplate") });
                        else
                            mapItem.Template = resources["DefaultLabelTemplate"] as DataTemplate;
                        MapItems.Add(mapItem);
                    }
                }
            }
        }

        private void RenderKmlElements()
        {
            ShapeTransform = CreateShapeTransform(kmlReader.BoundingBox);
            foreach (KmlPolygon polygon in kmlReader.PolygonList)
            {
                var geometry = CreatePathGeometry(polygon);
                var mapShape = CreateMapShape(geometry, polygon.Placemark);
                mapShape.SetShapePathWithKmlStyle(mapShape.Placemark.NormalStyle);
                MapShapes.Add(mapShape);
                VisibleShapes.Add(mapShape);
            }
            foreach (KmlPoint kmlPoint in kmlReader.PointList)
            {
#if WINRT
                var pt = ShapeTransform.TransformPoint(kmlPoint.Point);
#else
                var pt = ShapeTransform.Transform(kmlPoint.Point);
#endif
                var annotation = new MapAnnotations
                {
                    Longitude = kmlPoint.Point.X,
                    Latitude = kmlPoint.Point.Y,
                    midpoint = LatitudeLongitudeToPoint(kmlPoint.Point),
                };
                annotation.SetAnnotationSymbolWithKmlIcon(kmlPoint.Placemark.NormalStyle);
                annotation.AnnotationMargin = new Thickness(pt.X, pt.Y, 0, 0);
                Annotations.Add(annotation);
            }

        }

#if WPF
        void ShapeFileLayer_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            Bubbles.Clear();
            MapItems.Clear();
            if (ItemsSource is DataTable)
                MapData((ItemsSource as DataTable).Rows);
        }
#endif

        #endregion

        public void LoadFromStream(Stream shpstream)
        {
            if (shpstream != null)
            {
                shapeReader = new ShapeFileReader("Shapefile");
                shapeReader.ReadFromStream(shpstream);
                MapRect = new Rect(new Point(shapeReader.Header.MinX, shapeReader.Header.MinY), new Point(shapeReader.Header.MaxX, shapeReader.Header.MaxY));
                Refresh();

            }
        }

        public void LoadFromStream(Stream shpstream, Stream dbfstream)
        {
            if (shpstream != null)
            {
                shapeReader = new ShapeFileReader("Shapefile");
                shapeReader.ReadFromStream(shpstream);
                dbfReader = null;
                if (dbfstream != null)
                {
                    dbfReader = new ShapeFileDBFReader();
                    dbfReader.ReadFromStream(dbfstream);
                }
                MapRect = new Rect(new Point(shapeReader.Header.MinX, shapeReader.Header.MinY), new Point(shapeReader.Header.MaxX, shapeReader.Header.MaxY));
                Refresh();

            }
        }

        private void MapDBFData()
        {
            if (dbfReader != null && dbfReader.DBFData != null)
            {
                if (dbfReader.DBFData.DBFFields.Count > 0)
                {
                    var sel = from inf in dbfReader.DBFData.DBFFields
                              from item in inf
                              where item.Key == ShapeSettings.ShapeValuePath
                              select item.Value;
#if WINDOWSPHONE7
                    pointValues = sel.Cast<object>().ToList();
#elif SILVERLIGHT || WINDOWSPHONE || SILVERLIGHT_5
                    pointValues = (sel as IEnumerable<object>).ToList();
#else

                    pointValues = sel.ToList<object>();
#endif
                }
            }
        }
        private void HookCollectionChanged(INotifyCollectionChanged source)
        {
            source.CollectionChanged += source_CollectionChanged;
        }

        private void HookCustomSymbolCollectionChanged(INotifyCollectionChanged customSymbolSource)
        {
            customSymbolSource.CollectionChanged += customSymbolSource_CollectionChanged;
        }

        void customSymbolSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                CustomDataSymbols.Clear();
            }
            else
            {
                CustomDataSymbols.Clear();
                MapCustomDataSource(CustomDataSource);
            }
        }
        void source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                MapItems.Clear();
                Bubbles.Clear();
#if SILVERLIGHT
                if (ItemsSource is CollectionViewSource)
                {
                    MapData((ItemsSource as CollectionViewSource).View.SourceCollection);
                }
                else
                {
                    MapData(ItemsSource as IEnumerable);
                }
#endif
            }
            else
            {
                MapItems.Clear();
                Bubbles.Clear();
                if (ItemsSource is CollectionViewSource)
                {
#if WINRT
                    MapData((ItemsSource as CollectionViewSource).View.AsEnumerable());
#else
                    MapData((ItemsSource as CollectionViewSource).View.SourceCollection);
#endif
                }
                else
                {
                    MapData(ItemsSource as IEnumerable);
                }
            }
        }

        private void UpdateReader()
        {
            shapeReader = new ShapeFileReader();
            dbfReader = new ShapeFileDBFReader();

            if (ShapeData is IEnumerable)
            {
                UpdateShapeDBFReader(ShapeData as IEnumerable);
            }
#if !(SILVERLIGHT || SILVERLIGHT_5 || WINDOWSPHONE_8 || WINDOWSPHONE) && !WINRT
            else if (ShapeData is DataTable)
            {
                UpdateShapeDBFReader((ShapeData as DataTable).DefaultView);
            }
            else if (ShapeData is DataSet)
            {
                UpdateShapeDBFReader(((ShapeData as DataSet).Tables[0]).DefaultView);
            }
#endif
            else
            {
                throw new Exception("Collection class or datatable Input is needed");
            }
            MapRect = new Rect(new Point(shapeReader.Header.MinX, shapeReader.Header.MinY),
                new Point(shapeReader.Header.MaxX, shapeReader.Header.MaxY));
            if (isLoaded)
            {
                Refresh();
            }
        }

        private void UpdateShapeDBFReader(IEnumerable datasource)
        {
            dbfReader.DBFData = new ShapeFileDBFData();
            using (var readerEngine = new ShapeDBFReaderEngine())
            {
                readerEngine.UpdateShapeReader(datasource, this);
            }
        }
    }

    /// <summary>
    /// Represents the MapColorPalette class in the map control.Inherites from dependency object.
    /// </summary>
    public class MapColorPalette : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.MapColorPalette">MapColorPalette</see> class. 
        /// </summary>
        #region Constructor
        public MapColorPalette()
        {
        }
        #endregion

        /// <summary>
        /// Gets or sets Fill brush for the MapColorPalette..
        /// </summary>
        /// <value>
        /// Type :<see cref="Brush"/>
        /// </value>
        /// <remarks>
        /// Use this property to set the collection brush to be applied on the shapes while setting color palette for it.
        /// </remarks>
        public Brush FillBrush
        {
            get { return (Brush)GetValue(FillBrushProperty); }
            set { SetValue(FillBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FillBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillBrushProperty =
            DependencyProperty.Register("FillBrush", typeof(Brush), typeof(MapColorPalette), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


    }

    internal class ShapeDBFReaderEngine : IDisposable
    {
        private IEnumerable dataSourceList;

        private object dataSource;

        public Type ItemType { get; set; }

        public object DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    dataSourceList = null;
                    ItemType = null;
                    itemProperties = null;
                }
            }
        }

        public virtual IEnumerable DataSourceList
        {
            get
            {
                if (dataSourceList == null)
                {
                    if (DataSource is IEnumerable)
                    {
                        dataSourceList = DataSource as IEnumerable;
                    }
                }
                return dataSourceList;
            }
            set { dataSourceList = value; }
        }

#if WINRT
        private List<PropertyInfo> itemProperties;

        /// <summary>
        /// Gets a PropertyDescriptorCollection describing the type of the underlying data item.
        /// </summary>
        public List<PropertyInfo> ItemProperties
        {
            set { itemProperties = value; }
            get
            {
                if (itemProperties == null && (DataSourceList != null || ItemType != null))
                {

                    if (ItemType == null)
                    {
                        foreach (object o in DataSourceList)
                        {
                            ItemType = o.GetType();
                            break;
                        }
                    }
                }

                if (ItemType != null && itemProperties == null)
                {
                    itemProperties = ItemType.GetTypeInfo().DeclaredProperties.ToList();
                }
                return itemProperties;
            }
        }

#elif SILVERLIGHT || SILVERLIGHT_5 || WINDOWSPHONE_8 || WINDOWSPHONE
        private List<PropertyInfo> itemProperties;

        /// <summary>
        /// Gets a PropertyDescriptorCollection describing the type of the underlying data item.
        /// </summary>
        public List<PropertyInfo> ItemProperties
        {
            get
            {
                if (itemProperties == null && (DataSourceList != null || ItemType != null))
                {

                    if (ItemType == null)
                    {
                        foreach (object o in DataSourceList)
                        {
                            ItemType = o.GetType();
                            break;
                        }
                    }
                }

                if (ItemType != null && itemProperties == null)
                {
                    itemProperties = ItemType.GetProperties().ToList();
                }
                return itemProperties;
            }
            set { itemProperties = value; }
        }

#else
        private PropertyDescriptorCollection itemProperties;

        public PropertyDescriptorCollection ItemProperties
        {
            get
            {
                if (itemProperties == null && (DataSourceList != null || ItemType != null))
                {
                    if (dataSourceList is ITypedList)
                    {
                        itemProperties = ((ITypedList)dataSourceList).GetItemProperties(null);
                    }
                    else
                    {
                        if (ItemType == null)
                        {
                            foreach (object o in DataSourceList)
                            {
                                ItemType = o.GetType();
                                break;
                            }
                        }
                        if (ItemType != null)
                        {
                            itemProperties = TypeDescriptor.GetProperties(ItemType);
                        }
                    }
                }
                return itemProperties;
            }
            set { itemProperties = value; }
        }
#endif

        internal void UpdateShapeReader(IEnumerable datasource, ShapeFileLayer layer)
        {
            DataSource = datasource;
            IEnumerable list = DataSourceList;

            int index = 0;

            if (layer.dbfReader.DBFData.DBFFields == null)
            {
                layer.dbfReader.DBFData.DBFFields = new List<DBFValues>();
            }

            foreach (object o in list)
            {
                var val = new DBFValues();
#if !(SILVERLIGHT || SILVERLIGHT_5  || WINDOWSPHONE_8 || WINDOWSPHONE) && !WINRT
                if (o is DataRowView)
                {
                    var items = (o as DataRowView).Row.ItemArray;
                    var row = (o as DataRowView).Row;
                    for (int pos = 0; pos < items.Length; pos++)
                    {
                        if (row.Table.Columns[pos].ColumnName == layer.ShapeDataPath)
                        {
                            layer.shapeReader.ReadPathRecord((byte[])items[pos], index);
                        }
                        else
                        {
                            val.Add(new DBFFieldValues { Key = row.Table.Columns[pos].ColumnName, Value = items[pos].ToString() });
                        }
                    }
                }
                else
#endif
                {
                    if (o.GetType().FullName.Contains("Dictionary"))
                    {
                        var fields = (Dictionary<string, object>)o;
                        foreach (var field in fields)
                        {
                            if (field.Key == layer.ShapeDataPath)
                            {
                                layer.shapeReader.ReadPathRecord((byte[])field.Value, index);
                            }
                            else
                            {
                                val.Add(new DBFFieldValues { Key = field.Key, Value = field.Value.ToString() });
                            }

                        }
                    }
                    else
                    {
                        for (int i = 0; i < ItemProperties.Count; i++)
                        {
                            try
                            {
                                if (ItemProperties[i].Name == layer.ShapeDataPath)
                                {
#if SILVERLIGHT || SILVERLIGHT_5 || WINDOWSPHONE
                                    layer.shapeReader.ReadPathRecord((byte[])ItemProperties[i].GetValue(o,null), index);
#else
                                    layer.shapeReader.ReadPathRecord((byte[])ItemProperties[i].GetValue(o), index);
#endif
                                }
                                else
                                {
#if SILVERLIGHT || SILVERLIGHT_5 || WINDOWSPHONE
                                    val.Add(new DBFFieldValues { Key = ItemProperties[i].Name, Value = ItemProperties[i].GetValue(o,null).ToString() });
#else
                                    val.Add(new DBFFieldValues { Key = ItemProperties[i].Name, Value = ItemProperties[i].GetValue(o).ToString() });
#endif
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                layer.dbfReader.DBFData.DBFFields.Add(val);
                index++;
            }
        }

        public void Dispose()
        {
            ItemProperties = null;
            DataSource = null;
            ItemType = null;
            dataSource = null;
            dataSourceList = null;
            DataSourceList = null;
            GC.SuppressFinalize(this);
        }
    }
}

