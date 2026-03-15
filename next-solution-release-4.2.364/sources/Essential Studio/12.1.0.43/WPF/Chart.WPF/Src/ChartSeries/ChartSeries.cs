// <copyright file="ChartSeries.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using System.Xml.Serialization;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Diagnostics;
    using System.Data;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Class represents chart series. Series is used to draw required chart type on
    /// area.
    /// </summary>
    /// <remarks>
    /// Series is a part of <see cref="ChartArea" /> series collection. Chart points of
    /// series can be assigned either via <see cref="ChartSeries.Data" /> property or as
    /// content of series from XAML.
    /// </remarks>
    /// <example>
    /// <code language="XAML">
    /// &lt;Window.Resources&gt;
    ///         &lt;XmlDataProvider x:Key="myXmlData"&gt;
    ///             &lt;x:XData&gt;
    ///                 &lt;Products xmlns=""&gt;
    ///                     &lt;Product Sales="20" Projected="30" Month="1"/&gt;
    ///                     &lt;Product Sales="12" Projected="28" Month="2"/&gt;
    ///                     &lt;Product Sales="15" Projected="29" Month="3"/&gt;
    ///                     &lt;Product Sales="28" Projected="33" Month="4"/&gt;
    ///                     &lt;Product Sales="24" Projected="30" Month="5"/&gt;
    ///                 &lt;/Products&gt;
    ///             &lt;/x:XData&gt;
    ///         &lt;/XmlDataProvider&gt;
    ///     &lt;/Window.Resources&gt;
    ///     &lt;Grid&gt;
    ///         &lt;sfchart:Chart Name="Chart2"&gt;
    ///             &lt;sfchart:ChartArea &gt;
    ///                 &lt;sfchart:ChartSeries DataSource="{Binding
    /// Source={StaticResource myXmlData},   XPath=Products/Product}"
    /// BindingPathX="Month" BindingPathsY="Sales" Type="Column" Label="Actual
    /// Sales"&gt;
    ///                 &lt;/sfchart:ChartSeries&gt;
    ///                 &lt;sfchart:ChartSeries DataSource="{Binding
    /// Source={StaticResource myXmlData},          
    /// XPath=Products/Product}" BindingPathX="Month" BindingPathsY="Projected"
    /// Type="Column" Label="Projected Sales"&gt;
    ///                 &lt;/sfchart:ChartSeries&gt;
    ///             &lt;/sfchart:ChartArea&gt;
    ///         &lt;/sfchart:Chart&gt;
    ///     &lt;/Grid&gt;
    /// </code>
    /// </example>
    /// <permission cref="System.Security.PermissionSet"> Public Access </permission>
    /// <seealso cref="Chart"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ContentProperty("Data")]
    public class ChartSeries : FrameworkContentElement, IDisposable, IChartSerializer
    {
        #region Events
        /// <summary>
        /// Occurs when series data was changed.
        /// </summary>
        public event EventHandler DataChanged;

        /// <summary>
        /// Occurs when series appearance was changed.
        /// </summary>
        public event EventHandler AppearanceChanged;

        /// <summary>
        /// Occurs when any mouse button is clicked while pointer is over the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseMove;

        /// <summary>
        /// Occurs when any mouse button is clicked while pointer is over the series.
        /// </summary>
        public event ChartMouseEventHandler MouseClick;

        /// <summary>
        /// Occurs when any mouse button is pressed while pointer is over the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseDown;

        /// <summary>
        /// Occurs when mouse pointer enters the bounds of the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseEnter;

        /// <summary>
        /// Occurs when mouse pointer leaves the bounds of the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseLeave;

        /// <summary>
        /// Occurs when any mouse button is released over the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseUp;

        /// <summary>
        /// Occurs when left mouse button is released over the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseLeftButtonUp;

        /// <summary>
        /// Occurs when  mouse button is released over the series.
        /// </summary>
        public event ChartMouseEventHandler MouseDoubleClick;

        /// <summary>
        /// Occurs when right mouse button is released over the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseRightButtonUp;

        /// <summary>
        /// Occurs when left mouse button is pressed over the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseLeftButtonDown;

        /// <summary>
        /// Occurs when right mouse button is pressed over the series.
        /// </summary>
        public new event ChartMouseEventHandler MouseRightButtonDown;

        /// <summary>
        /// Occurs when IsZoomable property is changed.
        /// </summary>
        public event PropertyChangedCallback IsZoomableChanged;

        /// <summary>
        /// Occurs when series Type was changed.
        /// </summary>
        public event PropertyChangedCallback TypeChanged;

        /// <summary>
        /// Called when Type property changed.
        /// </summary>
        public event TypeChangingEventHandeler TypeChanging;

        internal bool IsTypeChanging = false;

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event DependencyPropertyChangedEventHandler PropertyChanged;
        #endregion
        private string seriesType = "Column";
        #region Dependency properties




        /// <summary>
        /// Get and Set ShowDataLabelsProperty
        /// </summary>
        public bool ShowDataLabels
        {
            get { return (bool)GetValue(ShowDataLabelsProperty); }
            set 
            {
                SetValue(ShowDataLabelsProperty, value); 
            }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowDataLabel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowDataLabelsProperty
            = DependencyProperty.Register("ShowDataLabels", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, OnShowDataLabelsChanged));

       private static void OnShowDataLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
       {
        if((d as ChartSeries) != null)
        {
            if((bool)e.NewValue)
            {
            (d as ChartSeries).AdornmentsInfo.Visible = true;
            }
            else
            {
            (d as ChartSeries).AdornmentsInfo.Visible = false;
            }
        }
       }
       

        /// <summary>
        /// Idenfities AnimationDuration dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(TimeSpan), typeof(ChartSeries), new PropertyMetadata(new TimeSpan(0, 0, 2), new PropertyChangedCallback(OnEaseAnimationChanged)));

        /// <summary>
        ///  Identifies the UseOptimization dependency property.
        /// </summary>
        public static readonly DependencyProperty UseOptimizationProperty =
           DependencyProperty.Register("UseOptimization", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the Resolution dependency property.
        /// </summary>
        public static readonly DependencyProperty ResolutionProperty =
          DependencyProperty.Register("Resolution", typeof(double), typeof(ChartSeries), new PropertyMetadata(1d));
        /// <summary>
        /// Idenfities Zorder dependency property.
        /// </summary>
        public static readonly DependencyProperty ZOrderProperty =
            DependencyProperty.Register("ZOrder", typeof(double), typeof(ChartSeries), new PropertyMetadata(0d));

        /// <summary>
        /// Idenfities AnimateOneByOneProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimateOneByOneProperty =
            DependencyProperty.Register("AnimateOneByOne", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnEaseAnimationChanged)));


        /// <summary>
        /// Identifies IsHitTestVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(ChartSeries), new PropertyMetadata(true));

        /// <summary>
        /// Idenfities AnimateOption dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimateOptionProperty =
            DependencyProperty.Register("AnimateOption", typeof(AnimationOptions), typeof(ChartSeries), new PropertyMetadata(AnimationOptions.Top, new PropertyChangedCallback(OnEaseAnimationChanged)));

        /// <summary>
        /// Idenfities EnableAnimation dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableAnimationChanged)));

        /// <summary>
        /// Idenfities EnableEffects dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableEffectsProperty =
            DependencyProperty.Register("EnableEffects", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableEffectsChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AutoDiscardProperty =
      DependencyProperty.Register("AutoDiscard", typeof(AutoDiscardType), typeof(ChartSeries), new PropertyMetadata(AutoDiscardType.None, new PropertyChangedCallback(OnIsAutoDiscardPropertyChanged)));

        internal static readonly DependencyProperty HoldDataUpdateProperty =
     DependencyProperty.Register("HoldDataUpdate", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the IsVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty =
          DependencyProperty.Register("IsVisible", typeof(bool), typeof(ChartSeries), new PropertyMetadata(true, new PropertyChangedCallback(OnAppearancePropertyChanged)));

        /// <summary>
        /// Identifies the IsIndexeddependency property.
        /// </summary>
        public static readonly DependencyProperty IsIndexedProperty =
          DependencyProperty.Register("IsIndexed", typeof(bool), typeof(ChartSeries), new PropertyMetadata(true, new PropertyChangedCallback(OnAppearancePropertyChanged), new CoerceValueCallback(OnIsIndexedCoerce)));

        /// <summary>
        /// Identifies the IsSortData dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSortDataProperty =
          DependencyProperty.Register("IsSortData", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnAppearancePropertyChanged)));

        /// <summary>
        /// Identifies the IsRotated dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRotatedProperty =
          DependencyProperty.Register("IsRotated", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnAppearancePropertyChanged)));

        /// <summary>
        /// Identifies the XAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
          DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(ChartSeries.OnAppearancePropertyChanged), new CoerceValueCallback(OnCoerceXAxis)));

        /// <summary>
        /// Identifies the YAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
          DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(ChartSeries.OnAppearancePropertyChanged), new CoerceValueCallback(OnCoerceYAxis)));

        /// <summary>
        /// Identifies the YAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty ZAxisProperty =
          DependencyProperty.Register("ZAxis", typeof(ChartAxis), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(ChartSeries.OnAppearancePropertyChanged), new CoerceValueCallback(OnCoerceZAxis)));

        /// <summary>
        /// Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
          DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartSeries), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Interior dependency property.
        /// </summary>
        public static readonly DependencyProperty InteriorProperty =
          DependencyProperty.Register("Interior", typeof(Brush), typeof(ChartSeries), new FrameworkPropertyMetadata(null, null, new CoerceValueCallback(OnInteriorCoerce)));
              
        /// <summary>
        /// Identifies the FastSegment dependency property.
        /// </summary>
        public static readonly DependencyProperty FastSegmentPropertiesProperty =
         DependencyProperty.Register("FastSegmentProperties", typeof(FastSegmnetPropertiesCollection), typeof(ChartSeries), new FrameworkPropertyMetadata(new FastSegmnetPropertiesCollection()));



        /// <summary>
        /// Identifies the Stroke dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x50, 0x50, 0x50))));

        /// <summary>
        /// Identifies the StrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
          DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ChartSeries), new PropertyMetadata(1d));

        /// <summary>
        /// Identifies the Data dependency property.
        /// </summary>
        public static readonly DependencyProperty DataProperty =
          DependencyProperty.Register("Data", typeof(IChartData), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the AdornmentsInfo dependency property.
        /// </summary>
        public static readonly DependencyProperty AdornmentsInfoProperty =
          DependencyProperty.Register("AdornmentsInfo", typeof(ChartAdornmentInfo), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnAdornmentsInfoChanged), new CoerceValueCallback(OnCoerceAdornmentsInfo)));

        /// <summary>
        /// Identifies the Type dependency property.
        /// </summary>
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(ChartTypes), typeof(ChartSeries), new FrameworkPropertyMetadata(ChartTypes.Column, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnTypeChanged),new CoerceValueCallback(OnTypeChanging)));

        /// <summary>
        /// Identifies the ChartType dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartTypeProperty =
          DependencyProperty.Register("ChartType", typeof(ChartType), typeof(ChartSeries), new PropertyMetadata(ChartSeries.KnownType(ChartTypes.Column), new PropertyChangedCallback(OnChartTypeChanged)));

        /// <summary>
        /// Identifies the IsZoomable dependency property.
        /// </summary>
        public static readonly DependencyProperty IsZoomableProperty =
            DependencyProperty.Register("IsZoomable", typeof(bool), typeof(ChartSeries), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsZoomableChanged)));

        /// <summary>
        /// Identifies the InactiveSeriesOpacityOnZoom dependency property.
        /// </summary>
        public static readonly DependencyProperty InactiveSeriesOpacityOnZoomProperty =
            DependencyProperty.Register("InactiveSeriesOpacityOnZoom", typeof(double), typeof(ChartSeries), new FrameworkPropertyMetadata(0.25d, new PropertyChangedCallback(OnInactiveSeriesOpacityOnZoomChanged)));

        /// <summary>
        /// Identifies the Label dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ChartSeries), new UIPropertyMetadata(String.Empty));

		/// <summary>
        /// Identifies the Unit dependency property.
        /// </summary>		
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(ChartSeries), new UIPropertyMetadata(null, new PropertyChangedCallback(OnUnitChanged)));

        /// <summary>   
        /// To show the unit of the series in the legend item
        /// </summary>   
        public static readonly DependencyProperty UnitVisibilityProperty =
            DependencyProperty.RegisterAttached("UnitVisibility", typeof(Visibility), typeof(ChartSeries), new FrameworkPropertyMetadata(Visibility.Collapsed, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the DataSource dependency property.
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(object), typeof(ChartSeries), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDataSourceChanged)));

        /// <summary>
        /// Identifies the BindingPathX dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingPathXProperty =
            DependencyProperty.Register("BindingPathX", typeof(string), typeof(ChartSeries), new UIPropertyMetadata(String.Empty, new PropertyChangedCallback(OnBindingPathXChanged)));

        /// <summary>
        /// Identifies the BindingPathsY dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingPathsYProperty =
            DependencyProperty.Register("BindingPathsY", typeof(IEnumerable<string>), typeof(ChartSeries), new UIPropertyMetadata(null, new PropertyChangedCallback(OnBindingPathsYChanged)));

        /// <summary>
        /// Identifies the Highlighted dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightedProperty =
            DependencyProperty.Register("Highlighted", typeof(bool), typeof(ChartSeries), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the Area property key.
        /// </summary>
        protected static readonly DependencyPropertyKey AreaPropertyKey =
            DependencyProperty.RegisterReadOnly("Area", typeof(ChartArea), typeof(ChartSeries), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAreaPropertyChanged)));

        /// <summary>
        /// Identifies the Area dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaProperty = AreaPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ChartSeries), new UIPropertyMetadata(null, null, new CoerceValueCallback(OnSelectedItemCoerce)));

        /// <summary>
        /// Identifies CenterPoint dependency property key.
        /// </summary>
        internal static readonly DependencyPropertyKey CenterPointPropertyKey =
            DependencyProperty.RegisterReadOnly("CenterPoint", typeof(Point), typeof(ChartSeries), new FrameworkPropertyMetadata(new Point()));

        /// <summary>
        /// Identifies CenterPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterPointProperty = CenterPointPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies the EnableSelection dependency property.
        /// </summary>
        internal static readonly DependencyProperty EnableSelectionProperty =
            DependencyProperty.Register("EnableSelection", typeof(bool), typeof(ChartSeries), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the Annotations dependency property.
        /// </summary>
        public static readonly DependencyProperty AnnotationsProperty =
            DependencyProperty.Register("Annotations", typeof(AnnotationsCollection), typeof(ChartSeries), new UIPropertyMetadata(null));


        /// <summary>
        /// Identifies the Indicators dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorsProperty =
           DependencyProperty.Register("Indicators", typeof(IndicatorCollection), typeof(ChartSeries), new PropertyMetadata(null));

        /// <summary>
        /// Get and Set IndicatorsProperty
        /// </summary>
        public IndicatorCollection Indicators
        {
            get { return (IndicatorCollection)GetValue(IndicatorsProperty); }
            set { SetValue(IndicatorsProperty, value); }
        }

        /// <summary>
        ///  Identifies the ShowSmartLabels dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowSmartLabelsProperty =
         DependencyProperty.Register("ShowSmartLabels", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Get and Set ShowSmartLabelsProperty
        /// </summary>
        public bool ShowSmartLabels
        {
            get { return (bool)GetValue(ShowSmartLabelsProperty); }
            set { SetValue(ShowSmartLabelsProperty, value); }
        }

        /// <summary>
        ///  Identifies the AdornmentIntersectAction dependency property.
        /// </summary>
        public static readonly DependencyProperty AdornmentIntersectActionProperty =
 DependencyProperty.Register("AdornmentIntersectAction", typeof(AdornmentIntersectActions), typeof(ChartSeries), new PropertyMetadata(AdornmentIntersectActions.None, new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Get and Set AdornmentIntersectActionProperty
        /// </summary>
        public AdornmentIntersectActions AdornmentIntersectAction
        {
            get { return (AdornmentIntersectActions)GetValue(AdornmentIntersectActionProperty); }
            set { SetValue(AdornmentIntersectActionProperty, value); }
        }

        /// <summary>
        /// Identifies the AdornmentSymbolTemplate dependency property.
        /// </summary>
        internal static readonly DependencyProperty AdornmentSymbolTemplateProperty =
          DependencyProperty.Register("AdornmentSymbolTemplate", typeof(ControlTemplate), typeof(ChartSeries), new PropertyMetadata(null));

        internal static readonly DependencyProperty FastTypePenProperty =
          DependencyProperty.Register("FastTypePen", typeof(Pen), typeof(ChartSeries), new PropertyMetadata(new Pen(Brushes.Black , 2)));

        /// <summary>
        /// Identifies the AdornmentSymbolInterior dependency property.
        /// </summary>
        internal static readonly DependencyProperty AdornmentSymbolInteriorProperty =
     DependencyProperty.Register("AdornmentSymbolInterior", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(Brushes.Transparent));
        /// <summary>
        /// Identifies the AdornmentSymbolStroke dependency property.
        /// </summary>
        internal static readonly DependencyProperty AdornmentSymbolStrokeProperty =
 DependencyProperty.Register("AdornmentSymbolStroke", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(Brushes.Transparent));
        /// <summary>
        /// Identifies the AdornmentSymbolStrokeThickness dependency property.
        /// </summary>
        internal static readonly DependencyProperty AdornmentSymbolStrokeThicknessProperty =
        DependencyProperty.Register("AdornmentSymbolStrokeThickness", typeof(double), typeof(ChartSeries), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the EmptyPointStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyPointStyleProperty =
DependencyProperty.Register("EmptyPointStyle", typeof(EmptyPointStyle), typeof(ChartSeries), new PropertyMetadata(EmptyPointStyle.Symbol));

        /// <summary>
        /// Identifies the EmptyPointInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyPointInteriorProperty =
        DependencyProperty.Register("EmptyPointInterior", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(Brushes.Orange));

        /// <summary>
        /// Identifies the ShowEmptyPoints dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowEmptyPointsProperty =
        DependencyProperty.Register("ShowEmptyPoints", typeof(bool), typeof(ChartSeries), new PropertyMetadata(false));

        /// <summary>   
        /// To show the legend icon of the chart series.
        /// </summary>   
        public static readonly DependencyProperty IsVisibleOnLegendProperty =
            DependencyProperty.Register("IsVisibleOnLegend", typeof(bool), typeof(ChartSeries), new PropertyMetadata(true, new PropertyChangedCallback(OnIsVisibleOnLegend)));

        /// <summary>   
        /// To show the legend icon of the chart series.
        /// </summary>   
        public static readonly DependencyProperty VisibilityOnLegendProperty =
            DependencyProperty.Register("VisibilityOnLegend", typeof(Visibility), typeof(ChartSeries), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnVisibilityOnLegend)));

        /// <summary>   
        /// To set the legend icon of the chart series.
        /// </summary>   
        public static readonly DependencyProperty LegendIconProperty =
             DependencyProperty.Register("LegendIcon", typeof(ChartLegendIcon), typeof(ChartSeries), new PropertyMetadata(ChartLegendIcon.None, new PropertyChangedCallback(OnLegendIconChanged)));

        /// <summary>   
        /// To set template for the legend icon
        /// </summary>   
        public static readonly DependencyProperty LegendIconTemplateProperty =
             DependencyProperty.Register("LegendIconTemplate", typeof(DataTemplate), typeof(ChartSeries), new PropertyMetadata(null,new PropertyChangedCallback(OnLegendIconTemplateChanged)));

        /// <summary>   
        /// To set template for the Internal legend icon
        /// </summary>   
        internal static readonly DependencyProperty InternalLegendIconTemplateProperty =
             DependencyProperty.Register("InternalLegendIconTemplate", typeof(DataTemplate), typeof(ChartSeries), new PropertyMetadata(null));

        /// <summary>
        ///  Identifies the EmptyPointSymbolTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyPointSymbolTemplateProperty =
        DependencyProperty.Register("EmptyPointSymbolTemplate", typeof(DataTemplate), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnEmptyPointSymbolTemplateChanged)));

        /// <summary>   
        /// To set template for the legend icon
        /// </summary>   
        public static readonly DependencyProperty EmptyPointValueProperty =
             DependencyProperty.Register("EmptyPointValue", typeof(EmptyPointValue), typeof(ChartSeries), new PropertyMetadata(EmptyPointValue.Average, new PropertyChangedCallback(OnEmptyPointValueChanged)));


        internal static readonly DependencyProperty TooltipStringProperty =
            DependencyProperty.Register("TooltipString", typeof(object), typeof(ChartSeries), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        /// 
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register("SortDirection", typeof(Direction), typeof(ChartSeries), new FrameworkPropertyMetadata(Direction.Ascending, new PropertyChangedCallback(ChartSeries.OnAppearancePropertyChanged)));


        /// <summary>
        ///  Identifies the SortBy dependency property.
        /// </summary>
        public static readonly DependencyProperty SortByProperty = DependencyProperty.Register("SortBy", typeof(SortingAxis), typeof(ChartSeries), new FrameworkPropertyMetadata(SortingAxis.X, new PropertyChangedCallback(ChartSeries.OnAppearancePropertyChanged)));

        internal object TooltipString
        {
            get { return (object)GetValue(TooltipStringProperty); }
            set { SetValue(TooltipStringProperty, value); }
        }

        internal Pen FastTypePen
        {
            get { return (Pen)GetValue(FastTypePenProperty); }
            set { SetValue(FastTypePenProperty, value); }
        }

        /// <summary>
        /// Get and Set EmptyPointSymbolTemplateProperty
        /// </summary>
        public DataTemplate EmptyPointSymbolTemplate
        {
            get { return (DataTemplate)GetValue(EmptyPointSymbolTemplateProperty); }
            set { SetValue(EmptyPointSymbolTemplateProperty, value); }
        }

        internal static readonly DependencyProperty LayerBrushProperty =
          DependencyProperty.Register("LayerBrush", typeof(Brush), typeof(ChartSeries), new PropertyMetadata(Brushes.Red));
        internal Brush LayerBrush
        {
            get { return (Brush)GetValue(LayerBrushProperty); }
            set { SetValue(LayerBrushProperty, value); }
        }
		 /// <summary>   
        /// To set intersect action for the SeriesAnnotationIntersectAction
        /// </summary>   
        public static readonly DependencyProperty SeriesAnnotationIntersectActionProperty =
            DependencyProperty.Register("SeriesAnnotationIntersectAction", typeof(AnnotationIntersectActions), typeof(ChartSeries), new PropertyMetadata(AnnotationIntersectActions.None));

        /// <summary>
        /// Get and Set SeriesAnnotationIntersectActionProperty
        /// </summary>
        public AnnotationIntersectActions SeriesAnnotationIntersectAction
        {
            get { return (AnnotationIntersectActions)GetValue(SeriesAnnotationIntersectActionProperty); }
            set { SetValue(SeriesAnnotationIntersectActionProperty, value); }
        }

        /// <summary>
        /// Identifies the ColorEach dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorEachProperty =
            DependencyProperty.Register("ColorEach", typeof(bool?), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnColorEachInteriorChanged)));

        /// <summary>
        /// Gets or sets a value that specifies whether each data point of a series is shown in a different color. 
        /// </summary>
        public bool? ColorEach
        {
            get
            {
                return (bool?)GetValue(ColorEachProperty);
            }
            set
            {
                SetValue(ColorEachProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ColorEachPalette dependency property.
        /// </summary>
        public static readonly DependencyProperty PaletteProperty =
            DependencyProperty.Register("Palette", typeof(ChartColorPalette), typeof(ChartSeries), new PropertyMetadata(ChartColorPalette.Default, new PropertyChangedCallback(OnColorEachInteriorChanged)));

        /// <summary>
        /// Gets or sets a value that specifies the palette to apply for the data points of series
        /// </summary>
        public ChartColorPalette Palette
        {
            get 
            { 
                return (ChartColorPalette)GetValue(PaletteProperty); 
            }
            set
            {
                SetValue(PaletteProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ColorEachCustomPalette dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomPaletteProperty =
            DependencyProperty.Register("CustomPalette", typeof(Brush[]), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnColorEachInteriorChanged)));

        /// <summary>
        /// Gets or sets a value that specifies the custom palette to apply for the data points of series
        /// </summary>
        public Brush[] CustomPalette
        {
            get 
            { 
                return (Brush[])GetValue(CustomPaletteProperty); 
            }
            set
            {
                SetValue(CustomPaletteProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ColorEachStrokePalette dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokePaletteProperty =
            DependencyProperty.Register("StrokePalette", typeof(ChartColorPalette), typeof(ChartSeries), new PropertyMetadata(ChartColorPalette.DefaultDark, new PropertyChangedCallback(OnColorEachInteriorChanged)));

        /// <summary>
        /// Gets or sets a value that specifies the stroke palette to apply for the data points of series
        /// </summary>
        public ChartColorPalette StrokePalette
        {
            get
            {
                return (ChartColorPalette)GetValue(StrokePaletteProperty);
            }
            set
            {
                SetValue(StrokePaletteProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ColorEachCustomStrokePalette dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomStrokePaletteProperty =
            DependencyProperty.Register("CustomStrokePalette", typeof(Brush[]), typeof(ChartSeries), new PropertyMetadata(null, new PropertyChangedCallback(OnColorEachInteriorChanged)));

        /// <summary>
        /// Gets or sets a value that specifies the custom stroke palette to apply for the data points of series
        /// </summary>
        public Brush[] CustomStrokePalette
        {
            get
            {
                return (Brush[])GetValue(CustomStrokePaletteProperty);
            }
            set
            {
                SetValue(CustomStrokePaletteProperty, value);
            }
        }
        #endregion

        #region Members
        /// <summary>
        /// Declares m_adornments
        /// </summary>
        private ChartAdornmentsCollection m_adornments = new ChartAdornmentsCollection();

        /// <summary>
        /// Declares m_visibleSegments
        /// </summary>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]      
        private ObservableCollection<ChartSegment> m_visibleSegments = new ObservableCollection<ChartSegment>();

        /// <summary>
        /// Declares m_needUpdateSegments
        /// </summary>
        internal bool m_needUpdateSegments = true;

        /// <summary>
        /// Declares m_needUpdateVisualSegments
        /// </summary>
        private bool m_needUpdateVisualSegments = true;

        /// <summary>
        /// Declares m_needUpdateIndexRange
        /// </summary>
        private bool m_needUpdateIndexRange = true;

        /// <summary>
        /// Declares m_xCidsRange
        /// </summary>
        internal DoubleRange m_xCidsRange = DoubleRange.Empty;

        /// <summary>
        /// Declares m_yCidsRange
        /// </summary>
        internal DoubleRange m_yCidsRange = DoubleRange.Empty;

        /// <summary>
        /// Declares m_yCidsRange
        /// </summary>
        private DoubleRange m_zCidsRange = DoubleRange.Empty;

        /// <summary>
        /// Declares m_isUpdating
        /// </summary>
        private bool m_isUpdating;

        /// <summary>
        /// Declares nextToEmptyPoint
        /// </summary>
        private double[] nextToEmptyPoint;

        /// <summary>
        /// Declares m_segments3D
        /// </summary>
        private Model3DGroup m_segments3D = new Model3DGroup();

        /// <summary>
        /// Declares internaldata_modified
        /// </summary>
        internal bool internaldata_modified = false;

        /// <summary>
        /// Declares m_cachedParentArea
        /// </summary>
        private ChartArea m_cachedParentArea;

        /// <summary>
        /// Declares Zoomactionenabled
        /// </summary>
        internal bool Zoomactionenabled = false;

        /// <summary>
        /// Declares Contains_emptypt
        /// </summary>
        internal bool Contains_emptypt = false;
        #endregion

        #region Properties

        private Mode _SegmentWidthMode = Mode.Fixed;
        /// <summary>
        /// Get and Set SegmentWidthModeProperty
        /// </summary>
        public Mode SegmentWidthMode
        {
            get
            {
                return _SegmentWidthMode;
            }
            set
            {
                _SegmentWidthMode = value;
            }
        }

        /// <summary>
        /// Enum  values for ChartSeries Modes
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Enum value for fixed mode
            /// </summary>
            Fixed,
            /// <summary>
            /// Enum value for relative mode
            /// </summary>
            Relative
        }

        /// <summary>
        /// Gets or sets the ChartSereis presenter
        /// </summary>
        internal ChartSeriesPresenter Presenter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Adornments contents
        /// </summary>
        internal List<UIElement> AdornmentPresenter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Chart Sereis Animation.
        /// </summary>
        internal ChartAnimation Animation
        {
            get;
            set;
        }

        /// <summary>
        /// Get the CLR property VisibleSegments value by internal property
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<ChartSegment> VisibleSegments
        {
            get { return m_visibleSegments; }
        }


        /// <summary>
        /// Gets or sets the AnimationDuration. This is dependency property.
        /// </summary>
        /// <value>The Timespan.</value>
        public TimeSpan AnimationDuration
        {
            get { return (TimeSpan)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        /// <summary>
        /// Get and Set UseOptimizationProperty
        /// </summary>
        public bool UseOptimization
        {
            get { return (bool)GetValue(UseOptimizationProperty); }
            set { SetValue(UseOptimizationProperty, value); }
        }

        /// <summary>
        /// Get and Set ResolutionProperty
        /// </summary>
        public double Resolution
        {
            get { return (double)GetValue(ResolutionProperty); }
            set { SetValue(ResolutionProperty, value); }
        }

        /// <summary>
        /// Get and Set ZOrderProperty
        /// </summary>
        public double ZOrder
        {
            get { return (double)GetValue(ZOrderProperty); }
            set { SetValue(ZOrderProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the AnimateOneByOne. This is dependency property.
        /// </summary>
        /// <value>The bool value.</value>
        public bool AnimateOneByOne
        {
            get { return (bool)GetValue(AnimateOneByOneProperty); }
            set { SetValue(AnimateOneByOneProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsHitTestVisible. This is dependency property.
        /// </summary>
        /// <value>The bool value.</value>
        public bool IsHitTestVisible
        {
            get { return (bool)GetValue(IsHitTestVisibleProperty); }
            set { SetValue(IsHitTestVisibleProperty, value); }
        }

        /// <summary>
        /// Get and Set EmptyPointValueProperty
        /// </summary>
        public EmptyPointValue EmptyPointValue
        {
            get { return (EmptyPointValue)GetValue(EmptyPointValueProperty); }
            set { SetValue(EmptyPointValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the AnimateOption. This is dependency property.
        /// </summary>
        /// <value>The AnimationDirection.</value>
        public AnimationOptions AnimateOption
        {
            get { return (AnimationOptions)GetValue(AnimateOptionProperty); }
            set { SetValue(AnimateOptionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableAnimation. This is dependency property.
        /// </summary>
        /// <value>The bool value.</value>
        public bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableEffects. This is dependency property.
        /// </summary>
        /// <value>The bool value.</value>
        public bool EnableEffects
        {
            get { return (bool)GetValue(EnableEffectsProperty); }
            set { SetValue(EnableEffectsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value for the Auto Discard. This is dependency property.
        /// </summary>
        public AutoDiscardType AutoDiscard
        {
            get { return (AutoDiscardType)GetValue(AutoDiscardProperty); }
            set { SetValue(AutoDiscardProperty, value); }
        }

        /// <summary>
        /// Get and Set HoldDataUpdateProperty
        /// </summary>
        public bool HoldDataUpdate
        {
            get { return (bool)GetValue(HoldDataUpdateProperty); }
            set { SetValue(HoldDataUpdateProperty, value); }
        }
        

        /// <summary>
        /// Gets or sets a value indicating whether to EnableSelection. This is a dependency property.
        /// </summary>
        /// <value>The EnableSelection.</value>
        internal bool EnableSelection
        {
            get { return (bool)GetValue(EnableSelectionProperty); }
            set { SetValue(EnableSelectionProperty, value); }
        }

        /// <summary>
        /// Gets the value indicating the CenterPoint. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Provides value of the real co-ordinates of the middle data point.
        /// </remarks>
        public Point CenterPoint
        {
            get { return (Point)GetValue(CenterPointProperty); }
        }

        /// <summary>
        /// Gets or sets a value indicating  the SelectedItem. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// When <see cref="DataSource "/> property is bound to <see cref="CollectionView"/>, this property stays in sync with CurrentItem.
        /// </remarks>
        /// <value>The SelectedItem.</value>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Highlighted. This is a dependency property.
        /// </summary>
        /// <value>The Highlighted.</value>
        public bool Highlighted
        {
            get { return (bool)GetValue(HighlightedProperty); }
            set { SetValue(HighlightedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the BindingPathsY collection. This is a dependency property.
        /// </summary>
        /// <value>The BindingPathsY collection.</value>
        [Category("Data")]
        [TypeConverter(typeof(ChartPathsConverter))]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]      
        public IEnumerable<string> BindingPathsY
        {
            get { return (IEnumerable<string>)GetValue(BindingPathsYProperty); }
            set { SetValue(BindingPathsYProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the path that should be used to retrieve X values from DataSource. This is a dependency property.
        /// </summary>
        /// <value>The string representation of x values.</value>
        public string BindingPathX
        {
            get { return (string)GetValue(BindingPathXProperty); }
            set { SetValue(BindingPathXProperty, value); }
        }

        /// <summary>
        /// Gets or sets value indicating how series should be represented in the <see
        /// cref="ChartLegend" />. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Label is generally used to represent series on the legend.
        /// </remarks>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 1 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Setting label in user readable form.
        /// chart.Areas[0].Series[0].Label = "Sales representation";
        /// //Setting series name for internal usage.
        /// chart.Areas[0].Series[0].Name = "Series1";
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries
        /// Label="Sales representation"
        /// Name="Series1"
        /// Data="1 1 2 2"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Gets or Sets UnitProperty
        /// </summary> 
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }        

		/// <summary> 
        /// Gets or sets UnitVisibility.
        /// </summary> 
        public Visibility UnitVisibility
        {
            get { return (Visibility)GetValue(ChartSeries.UnitVisibilityProperty); }
            set { SetValue(ChartSeries.UnitVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets series' opacity while it's not currently zoomable. This is a
        /// dependency property.
        /// </summary>
        /// <remarks>
        /// In zooming mode user is asked to pick series to zoom, while all others remains
        /// unchanged. Inactive zooming series will have <see
        /// cref="InactiveSeriesOpacityOnZoom" /> opacity. Default value is 0.25.
        /// </remarks>
        /// <example>
        /// <remarks> In zooming mode user is asked to pick series to zoom, while all others
        /// remains unchanged. Inactive zooming series will have <see
        /// cref="InactiveSeriesOpacityOnZoom" /> opacity. Default value is 0.25. </remarks>
        /// C#: <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 1 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Setting custom opacity for series for inactive zoom mode.
        /// chart.Areas[0].Series[0].InactiveSeriesOpacityOnZoom = 0.5;
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartArea.Series&gt;
        /// &lt;syncfusion:ChartSeries
        /// Data="1 1"
        /// InactiveSeriesOpacityOnZoom="0.5"/&gt;
        /// &lt;/syncfusion:ChartArea.Series&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public double InactiveSeriesOpacityOnZoom
        {
            get { return (double)GetValue(InactiveSeriesOpacityOnZoomProperty); }
            set { SetValue(InactiveSeriesOpacityOnZoomProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether series is zoomable. This is a dependency
        /// property.
        /// </summary>
        /// <remarks>
        /// In zooming mode series IsZoomable property can be set to false to prevent series
        /// from zooming. <para> However, series will get zoomed anyway if it has common
        /// axis withseries that is currently zooming. </para>
        /// </remarks>
        /// <value>
        /// <c>true</c> if series gets zoomed on zoom commands, <c>false</c> if series should remain unzoomed. Value changed automatically depending on zooming context menu actions.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 1 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Restricting series from zooming
        /// chart.Areas[0].Series[0].IsZoomable = false;
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries
        /// IsZoomable="False"
        /// Data="1 1 2 2"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public bool IsZoomable
        {
            get { return (bool)GetValue(IsZoomableProperty); }
            set { SetValue(IsZoomableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this series is visible. This is a
        /// dependency property.
        /// </summary>
        /// <remarks>
        /// Invisible series still receives mouse events.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this series is visible; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 1 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Making series invisible.
        /// chart.Areas[0].Series[0].IsVisible = false;
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries
        /// IsVisible="False"
        /// Data="1 1 2 2"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public bool IsVisible
        {
            get
            {
                return (bool)GetValue(IsVisibleProperty);
            }

            set
            {
                SetValue(IsVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the points corresponds for indexing of series. This is a dependency property.
        /// </summary>
        /// <value><c>true</c> if this series is indexed; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// By default points in a series are plotted against their X and Y values.
        /// However in some cases the X values are meaningless, they simply represent categories,
        /// and you do not want to plotting the points against such X values are not desired.
        /// Such an X axis that ignores the X-values and simply uses the positional value of a point in a
        /// series is said to be Indexed. Series can be shown as indexed if all other series have equal X values.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 1 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Making series invisible.
        /// chart.Areas[0].Series[0].IsIndexed = true;
        /// </code>
        /// XAML:
        /// <code language="XAML">
        /// &lt;syncfusion:Chart xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries
        /// IsIndexed="True"
        /// Data="1 1 2 2"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public bool IsIndexed
        {
            get
            {
                return (bool)GetValue(IsIndexedProperty);
            }

            set
            {
                SetValue(IsIndexedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the series data is required to be sorted
        /// </summary>
        /// <value><c>true</c> if this instance is sorted; otherwise, <c>false</c>.</value>
        public bool IsSortData
        {
            get
            {
                return (bool)GetValue(IsSortDataProperty);
            }

            set
            {
                SetValue(IsSortDataProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether series is rotated. This is a dependency
        /// property.
        /// </summary>
        /// <remarks>
        /// A Rotated series is similar to a regular series. The only difference is that it
        /// would be rotated.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this series is rotated; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 1 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Making series invisible.
        /// chart.Areas[0].Series[0].IsRotated = true;
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries
        /// IsRotated="True"
        /// Data="1 1 2 2"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public bool IsRotated
        {
            get
            {
                return (bool)GetValue(IsRotatedProperty);
            }

            set
            {
                SetValue(IsRotatedProperty, value);
            }
        }

        /// <summary>
        /// Gets range for X axis.
        /// </summary>
        /// <remarks>
        /// Specifies the minimum, maximum and interval for the X-axis. 
        /// Should be used for data points of <see cref="Double"/> type.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 1 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Saving series range for X-Axis.
        /// DoubleRange serisXAxisRange = chart.Areas[0].Series[0].XRange;
        /// </code>
        /// XAML:
        /// <para>
        /// This property is not intended to be used from XAML.
        /// </para>
        /// </example>
        /// <value>Value type of <see cref="DoubleRange"/>.</value>
        public DoubleRange XRange
        {
            get
            {
                Recalculate();
                return m_xCidsRange;
            }
        }

        /// <summary>
        /// Gets range for Y-axis.
        /// </summary>
        /// <remarks>
        /// Specifies the minimum, maximum and interval for the Y-axis. 
        /// Should be used for data points of <see cref="Double"/> type.
        /// </remarks>
        /// <example>
        /// C#:
        /// <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 1 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Saving series range for Y-Axis.
        /// DoubleRange serisXAxisRange = chart.Areas[0].Series[0].XRange;
        /// </code>
        /// XAML:
        /// <para>
        /// This property is not intended to be used from XAML.
        /// </para>
        /// </example>
        /// <value>Value type of <see cref="DoubleRange"/>.</value>
        public DoubleRange YRange
        {
            get
            {
                Recalculate();
                return m_yCidsRange;
            }
        }

        /// <summary>
        /// ZRange CLR property declaration
        /// </summary>
        public DoubleRange ZRange
        {
            get
            {
                Recalculate();
                return m_zCidsRange;
            }
        }

        /// <summary>
        /// Gets or sets the adornments info. Property is intended to customize adornments
        /// on series. This is a dependency property.
        /// </summary>
        /// <value>
        /// The <see cref="ChartAdornmentInfo" /> value.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// ChartSeries series = new ChartSeries();
        /// //...
        /// //Filling series with data.
        /// //...
        /// //Setting adornments information
        /// series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Stretch;
        /// series.AdornmentsInfo.SegmentLabelFontSize = 20;
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartArea.Series&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3"&gt;
        /// &lt;syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;syncfusion:ChartAdornmentInfo
        /// HorizontalAlignment="Stretch"
        /// SegmentLabelFontSize="20"/&gt;
        /// &lt;/syncfusion:ChartSeries.AdornmentsInfo&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea.Series&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public ChartAdornmentInfo AdornmentsInfo
        {
            get
            {
                return (ChartAdornmentInfo)GetValue(AdornmentsInfoProperty);
            }

            set
            {
                SetValue(AdornmentsInfoProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the legend icon.
        /// </summary>
        /// <value>The legend icon.</value>
        public ChartLegendIcon LegendIcon
        {
            get
            {
                return (ChartLegendIcon)GetValue(LegendIconProperty);
            }
            set
            {
                SetValue(LegendIconProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the legend icon template.
        /// </summary>
        /// <value>The legend icon template.</value>
        public DataTemplate LegendIconTemplate
        {
            get
            {
                return (DataTemplate)GetValue(LegendIconTemplateProperty);
            }
            set
            {
                SetValue(LegendIconTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Internal legend icon template.
        /// </summary>
        /// <value>The Internal legend icon template.</value>
        internal DataTemplate InternalLegendIconTemplate
        {
            get
            {
                return (DataTemplate)GetValue(InternalLegendIconTemplateProperty);
            }
            set
            {
                SetValue(InternalLegendIconTemplateProperty, value);
            }
        }

        /// <summary>
        /// Get and Set ShowToolTipProperty
        /// </summary>
        public bool ShowToolTip
        {
            get { return (bool)GetValue(ShowToolTipProperty); }
            set { SetValue(ShowToolTipProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowToolTip.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowToolTipProperty =
            DependencyProperty.Register("ShowToolTip", typeof(bool), typeof(ChartSeries), new UIPropertyMetadata(true));



        #region Area and Axes properties
        /// <summary>
        /// Gets the value of the Area. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Gets area that current series is to be drawn on. When adding series to chart
        /// area this property is initialized automatically.
        /// </remarks>
        /// <value>
        /// Value is type of <see cref="ChartArea" />.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 1 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// // now chart.Areas[0].Series.Area contains parent area.
        /// </code> XAML: <para> This property is not intended to be used from XAML. </para>
        /// </example>
        public ChartArea Area
        {
            get { return (ChartArea)GetValue(AreaProperty); }
            internal set { SetValue(AreaPropertyKey, value); }
        }

        /// <summary>
        /// Gets or sets X-axis for series. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// By default series binds to <see cref="ChartArea.PrimaryAxis" />. This property
        /// allows to explicitly set series' X-axis.
        /// </remarks>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 2 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Creating a new axis.
        /// ChartAxis xAxis = new ChartAxis();
        /// //Setting axis' orientation.
        /// xAxis.Orientation = Orientation.Horizontal;
        /// //Assigning a new axis to series.
        /// chart.Areas[0].Series[1].XAxis = xAxis;
        /// </code> XAML: <code language="XAML">
        /// &lt;Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// Width="300" Height="300"&gt;
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartArea.Series&gt;
        /// &lt;syncfusion:ChartSeries Data="5 5 6 6 7 7"&gt;
        /// &lt;syncfusion:ChartSeries.XAxis&gt;
        /// &lt;syncfusion:ChartAxis Orientation="Horizontal"/&gt;
        /// &lt;/syncfusion:ChartSeries.XAxis&gt;
        /// &lt;syncfusion:ChartSeries.YAxis&gt;
        /// &lt;syncfusion:ChartAxis Orientation="Vertical"/&gt;
        /// &lt;/syncfusion:ChartSeries.YAxis&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea.Series&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// &lt;/Window&gt;
        /// </code>
        /// </example>
        [XmlIgnore]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartAxis XAxis
        {
            get
            {
                return this.GetValue(ChartSeries.XAxisProperty) as ChartAxis;
            }

            set
            {
                this.SetValue(ChartSeries.XAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Y-axis for series. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// By default series binds to <see cref="ChartArea.PrimaryAxis" />. This property
        /// allows to explicitly set series' Y-axis.
        /// </remarks>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 2 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Creating a new axis.
        /// ChartAxis xAxis = new ChartAxis();
        /// //Setting axis' orientation.
        /// xAxis.Orientation = Orientation.Vertical;
        /// //Assigning a new axis to series.
        /// chart.Areas[0].Series[1].YAxis = xAxis;
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries
        /// IsRotated="True"
        /// Data="1 1 2 2"/&gt;
        /// &lt;syncfusion:ChartSeries
        /// Data="3 3 4 4"&gt;
        /// &lt;syncfusion:ChartSeries.XAxis&gt;
        /// &lt;syncfusion:ChartAxis Orientation="Vertical"/&gt;
        /// &lt;/syncfusion:ChartSeries.XAxis&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        [XmlIgnore]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartAxis YAxis
        {
            get
            {
                return this.GetValue(ChartSeries.YAxisProperty) as ChartAxis;
            }

            set
            {
                this.SetValue(ChartSeries.YAxisProperty, value);
            }
        }

        /// <summary>
        /// Get and Set ZAxisProperty
        /// </summary>
        public ChartAxis ZAxis
        {
            get
            {
                return this.GetValue(ChartSeries.ZAxisProperty) as ChartAxis;
            }

            set
            {
                this.SetValue(ChartSeries.ZAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the adornment symbol interior.
        /// </summary>
        /// <value>The adornment symbol interior.</value>
        internal Brush AdornmentSymbolInterior
        {
            get
            {
                return (Brush)GetValue(AdornmentSymbolInteriorProperty);
            }

            set
            {
                SetValue(AdornmentSymbolInteriorProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the adornment symbol Stroke.
        /// </summary>
        /// <value>The adornment symbol Stroke.</value>
        internal Brush AdornmentSymbolStroke
        {
            get
            {
                return (Brush)GetValue(AdornmentSymbolStrokeProperty);
            }

            set
            {
                SetValue(AdornmentSymbolStrokeProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the adornment symbol Stroke Thickness.
        /// </summary>
        /// <value>The adornment symbol Stroke Thickness.</value>
       
        internal double AdornmentSymbolStrokeThickness
        {
            get
            {
                return (double)GetValue(AdornmentSymbolStrokeThicknessProperty);
            }

            set
            {
                SetValue(AdornmentSymbolStrokeThicknessProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the adornment symbol template.
        /// </summary>
        /// <value>The adornment symbol template.</value>
        internal ControlTemplate AdornmentSymbolTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(AdornmentSymbolTemplateProperty);
            }

            set
            {
                SetValue(AdornmentSymbolTemplateProperty, value);
            }
        }
        #endregion

        #region Data content
        /// <summary>
        /// Gets or sets the series data. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Used to assign data points for series.
        /// </remarks>
        /// <value>
        /// Any collection that implements <see cref="IChartData" /> can be assigned.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Adding 1 series to area
        /// chart.Areas[0].Series.Add(new ChartSeries());
        /// //Initializing chart points collection.
        /// ChartListData chartPoints = new ChartListData();
        /// //Adding point to collection.
        /// chartPoints.Add(new ChartPoint(1, 1));
        /// chartPoints.Add(new ChartPoint(2, 2));
        /// chartPoints.Add(new ChartPoint(3, 3));
        /// chartPoints.Add(new ChartPoint(4, 4));
        /// //Assigning chart points collection to series' Data property.
        /// chart.Areas[0].Series[0].Data = chartPoints;
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartArea.Series&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3 4 4"/&gt;
        /// &lt;/syncfusion:ChartArea.Series&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        //[XmlIgnore]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        [TypeConverter(typeof(ChartListDataConverter))]
        public IChartData Data
        {
            get
            {
                return (IChartData)GetValue(DataProperty);
            }

            set
            {
                SetValue(DataProperty, value);

            }
        }

        /// <summary>
        /// Gets or sets the data source. This is a dependency property.
        /// </summary>
        /// <value>The data source.</value>
        //[XmlIgnore]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Data")]
        public object DataSource
        {
            get { return (object)GetValue(DataSourceProperty); }
            set 
            {   
               
                if (value is IEnumerable)
                {
                    SetValue(DataSourceProperty, value);
                }
                else
                {
                    if (value is DataTable)
                    {
                        SetValue(DataSourceProperty, (value as DataTable).Rows);
                    }                   
                    else
                    {
                        SetValue(DataSourceProperty, value);
                    }
                }               
            }
        }

        /// <summary>
        /// Gets the type of the X values of series.
        /// </summary>
        /// <value>The type of the X value.</value>
        public ChartValueType XValueType
        {
            get
            {
                //return this.Data == null ? ChartValueType.Double : this.Data.XValueType;
                return this.DataModel == null ? ChartValueType.Double : this.DataModel.XValueType;
            }
        }
        #endregion

        #region Appearance

        /// <summary>
        /// Gets or sets corresponding chart type. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Chart type can be set from <see cref="ChartTypes" /> enumeration.
        /// </remarks>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series type of Bar.
        /// ChartSeries series = new ChartSeries(ChartTypes.Bar);
        /// //Changing type of series to FastLine.
        /// series.Type = ChartTypes.FastLine;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// //Changing series type back to Bar.
        /// chart.Areas[0].Series[0].Type = ChartTypes.Bar;
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries
        /// Type="Pie"
        /// Data="1 1 2 2"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>

        public ChartTypes Type
        {
            get
            {
                return (ChartTypes)GetValue(TypeProperty);
            }

            set
            {
                SetValue(TypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the chart Type.  This is a dependency property.
        /// </summary>
        /// <remarks>Intended to be used with custom chart types.</remarks>
        [XmlIgnore]
        [DesignerSerializationOptions(DesignerSerializationOptions.SerializeAsAttribute)]
        public ChartType ChartType
        {
            get
            {
                return (ChartType)GetValue(ChartTypeProperty);
            }

            set
            {
                SetValue(ChartTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a template for current series type. THis is a dependency property.
        /// </summary>
        /// <remarks>
        /// Data templates area used to customize look of series' data by overriding the
        /// default template.
        /// </remarks>
        /// <example>
        /// C#: <code language="C#">
        /// This property is not intended to be used from C#.
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2"&gt;
        /// &lt;syncfusion:ChartSeries.Template&gt;
        /// &lt;DataTemplate&gt;
        /// &lt;Canvas&gt;
        /// &lt;Border
        /// Canvas.Left="{Binding X}"
        /// Canvas.Top="{Binding Y}"
        /// Width="{Binding Width}"
        /// Height="{Binding Height}"
        /// BorderBrush="White"
        /// BorderThickness="2"
        /// CornerRadius="15"&gt;
        /// &lt;Rectangle Margin="5" Fill="AliceBlue"/&gt;
        /// &lt;/Border&gt;
        /// &lt;/Canvas&gt;
        /// &lt;/DataTemplate&gt;
        /// &lt;/syncfusion:ChartSeries.Template&gt;
        /// &lt;/syncfusion:ChartSeries&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>

        public DataTemplate Template
        {
            get
            {
                return (DataTemplate)GetValue(TemplateProperty);
            }

            set
            {
                SetValue(TemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets interior for series. This is a dependency property.
        /// </summary>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series type of Bar.
        /// ChartSeries series = new ChartSeries(ChartTypes.Bar);
        /// //Setting interior brush for series.
        /// series.Interior = Brushes.Black;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries
        /// Interior="Black"
        /// Data="1 1 2 2"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public Brush Interior
        {
            get
            {
                return (Brush)GetValue(InteriorProperty);
            }

            set
            {
                SetValue(InteriorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the segments interior list.
        /// </summary>
        /// <value>The segments interior list.</value>
        public FastSegmnetPropertiesCollection FastSegmentProperties
        {
            get
            {
                return (FastSegmnetPropertiesCollection)GetValue(FastSegmentPropertiesProperty);
            }

            set
            {
                SetValue(FastSegmentPropertiesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the brush that chart series border should be filled. This is a
        /// dependency property.
        /// </summary>
        /// <value>
        /// The stroke.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series type of Bar.
        /// ChartSeries series = new ChartSeries(ChartTypes.Bar);
        /// //Setting stroke brush for series.
        /// series.Stroke = Brushes.Black;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries
        /// Stroke="Black"
        /// Data="1 1 2 2"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public Brush Stroke
        {
            get
            {
                return (Brush)GetValue(StrokeProperty);
            }

            set
            {
                SetValue(StrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the stroke thickness. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Property corresponds for pen's thickness that outlines chart series.
        /// </remarks>
        /// <value>
        /// The stroke.
        /// </value>
        /// <example>
        /// C#: <code language="C#">
        /// //Initializing a new chart.
        /// Chart chart = new Chart();
        /// //Adding area.
        /// chart.Areas.Add(new ChartArea());
        /// //Creating new series type of Bar.
        /// ChartSeries series = new ChartSeries(ChartTypes.Bar);
        /// //Setting stroke thickness for series.
        /// series.StrokeThickness = 5;
        /// //Adding series to area.
        /// chart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartSeries
        /// StrokeThickness="5"
        /// Data="1 1 2 2"/&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public double StrokeThickness
        {
            get
            {
                return (double)GetValue(StrokeThicknessProperty);
            }

            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Annotations collection. This is a dependency property.
        /// </summary>
        /// <value>The <see cref="AnnotationsCollection"/>.</value>
        public AnnotationsCollection Annotations
        {
            get { return (AnnotationsCollection)GetValue(AnnotationsProperty); }
            set { SetValue(AnnotationsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show empty points.
        /// </summary>
        /// <value><c>true</c> if show empty points otherwise, <c>false</c>.</value>
        public bool ShowEmptyPoints
        {
            get { return (bool)GetValue(ShowEmptyPointsProperty); }
            set { SetValue(ShowEmptyPointsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the empty point style.
        /// </summary>
        /// <value>The empty point style.</value>
        public EmptyPointStyle EmptyPointStyle
        {
            get { return (EmptyPointStyle)GetValue(EmptyPointStyleProperty); }
            set { SetValue(EmptyPointStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the empty point interior.
        /// </summary>
        /// <value>The empty point interior.</value>
        public Brush EmptyPointInterior
        {
            get { return (Brush)GetValue(EmptyPointInteriorProperty); }
            set { SetValue(EmptyPointInteriorProperty, value); }
        }



        /// <summary> 
        /// Gets or sets IsVisibleOnLegend .It will reflect when IsVisibleOnLegendProperty is true .
        /// </summary> 
        public Visibility VisibilityOnLegend
        {
            get { return (Visibility)GetValue(VisibilityOnLegendProperty); }
            set { SetValue(VisibilityOnLegendProperty, value); }
        }

        /// <summary> 
        /// Gets or sets IsVisibleOnLegend
        /// </summary> 
        public bool IsVisibleOnLegend
        {
            get { return (bool)GetValue(IsVisibleOnLegendProperty); }
            set { SetValue(IsVisibleOnLegendProperty, value); }
        }

        /// <summary>
        /// Get the CLR proeprty values
        /// </summary>
        public bool OriginDependent
        {
            get
            {
                return (Type == ChartTypes.Area||
                    Type == ChartTypes.Bar ||
                    Type == ChartTypes.Column ||
                    Type == ChartTypes.SplineArea ||
                    Type == ChartTypes.StackingArea ||
                    Type == ChartTypes.StackingLine||
                    Type == ChartTypes.StackingSpline ||
                    Type == ChartTypes.StackingSplineArea ||
                    Type == ChartTypes.StackingBar ||
                    Type == ChartTypes.StackingColumn ||
                    Type == ChartTypes.StepArea ||
                    Type == ChartTypes.StackingBar100 ||
                    Type == ChartTypes.Histogram ||
                    Type == ChartTypes.StackingColumn100);
            }
        }

        internal bool ColorEachDependent
        {
            get
            {
               return !(Type == ChartTypes.FastBar || Type == ChartTypes.FastColumn || Type == ChartTypes.FastHiLoOpenClose ||
                        Type == ChartTypes.FastLine || Type==ChartTypes.FastSpline || Type == ChartTypes.FastScatter || Type == ChartTypes.FastStackingColumn ||
                        Type == ChartTypes.Kagi || Type == ChartTypes.PointAndFigure || Type == ChartTypes.ThreeLineBreak ||
                        Type == ChartTypes.Area || Type == ChartTypes.HiLoArea || Type == ChartTypes.StepArea || Type == ChartTypes.Polar ||
                        Type == ChartTypes.RangeArea || Type == ChartTypes.SplineArea || Type == ChartTypes.StackingArea || Type == ChartTypes.StackingLine ||
                        Type == ChartTypes.StackingSpline || Type == ChartTypes.StackingSplineArea);
            }
        }
        #endregion

        #region Internal properties
        /// <summary>
        /// Gets the adornments collection.
        /// </summary>
        /// <value>The adornments.</value>
        public ChartAdornmentsCollection Adornments
        {
            get
            {
                return m_adornments;
            }
        }

        /// <summary>
        /// Gets the Segments. Internal property.
        /// </summary>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObservableCollection<ChartSegment> Segments
        {
            get
            {
                return m_visibleSegments;
            }
        }

        /// <summary>
        /// Gets the 3D segments.
        /// </summary>
        /// <value>The 3D segments collection.</value>
        internal Model3DGroup Segments3D
        {
            get
            {
                return m_segments3D;
            }
        }

        /// <summary>
        /// Gets actual series X-axis.
        /// </summary>
        /// <remarks>
        /// Gets actual XAxis for series with respect to chart type and <see cref="ChartSeries.IsRotated"/> value.
        /// </remarks>
        internal ChartAxis ActualXAxis
        {
            get
            {
                if (Area != null)
                {
                    return XAxis == null ? Area.PrimaryAxis : XAxis;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets actual series Y-axis.
        /// </summary>
        internal ChartAxis ActualYAxis
        {
            get
            {
                if (Area != null)
                {
                    return YAxis == null ? Area.SecondaryAxis : YAxis;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets actual series Z-axis.
        /// </summary>
        internal ChartAxis ActualZAxis
        {
            get
            {
                if (Area != null)
                {
                    return ZAxis == null ? Area.DepthAxis : ZAxis;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the points series count.  This is a dependency property.
        /// </summary>
        /// <value>The points count.</value>
        internal int PointsCount
        {
            get
            {
                return this.Data == null ? 0 : this.Data.Count;
            }
        }
        #endregion
        internal ChartLegendIcon legendicon = ChartLegendIcon.Circle;

        //ChartDataMultibindingConverter converter = new ChartDataMultibindingConverter();

        /// <summary>
        /// Gets or sets Direction for sorting.
        /// </summary>
        /// <value>Enum values Ascending/Descending</value>
        /// <remarks></remarks>
        public Direction SortDirection
        {
            get
            {
                return (Direction)GetValue(SortDirectionProperty);
            }
            set
            {
                SetValue(SortDirectionProperty, value);
            }

        }

        /// <summary>
        /// Get and Set SortByProperty
        /// </summary>
        public SortingAxis SortBy
        {
            get
            {
                return (SortingAxis)GetValue(SortByProperty);
            }
            set
            {
                SetValue(SortByProperty, value);
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="T:Syncfusion.Windows.Chart.ChartSeries">ChartSeries</see> class. 
        /// </summary>
        static ChartSeries()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(ChartSeries));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartSeries), new FrameworkPropertyMetadata(typeof(ChartSeries)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSeries" /> class.
        /// </summary>
        /// <example>
        /// C#: <code language="C#">
        /// //Creating new instance of chart.
        /// Chart mySampleChart = new Chart();
        /// //Adding new chart area.
        /// mySampleChart.Areas.Add(new ChartArea());
        /// //Series1 initialized with empty constructor.
        /// ChartSeries series = new ChartSeries();
        /// //Series1 initialized with passed chart type.
        /// mySampleChart.Areas[0].Series.Add(series);
        /// </code> XAML: <code language="XAML">
        /// &lt;syncfusion:Chart
        /// xmlns:syncfusion="http://www.syncfusion.com/WpfChart.xsd"&gt;
        /// &lt;syncfusion:ChartArea&gt;
        /// &lt;syncfusion:ChartArea.Series&gt;
        /// &lt;syncfusion:ChartSeries Data="1 1 2 2 3 3"/&gt;
        /// &lt;/syncfusion:ChartArea.Series&gt;
        /// &lt;/syncfusion:ChartArea&gt;
        /// &lt;/syncfusion:Chart&gt;
        /// </code>
        /// </example>
        public ChartSeries()
        {
            this.CoerceValue(AdornmentsInfoProperty);
            
            BindingDataModel();
           

        }
        //[XmlIgnore]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]      
        /// <summary>
        ///
        /// </summary>
        public ChartDataModel m_DataModel = new ChartDataModel();
        //[XmlIgnore]
        /// <summary>
        /// Gets or Sets the DataModel property
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartDataModel DataModel
        {
            get { return m_DataModel; }
            set
            {
                m_DataModel = value;
                if (m_DataModel != null)
                    m_DataModel.m_ChartSeries = this;
            }
        }

        /// <summary>
        /// Method implementation for BindingDataModel
        /// </summary>
        public void BindingDataModel()
        {
            //Binding datasourceBinding = new Binding("Source") { Source = this.DataModel, Mode = BindingMode.TwoWay };
            //BindingOperations.SetBinding(this, ChartSeries.DataSourceProperty, datasourceBinding);
            m_DataModel.m_ChartSeries = this;
            //Binding pathXBinding = new Binding("PathX") { Source = this.DataModel, Mode = BindingMode.TwoWay };
            //BindingOperations.SetBinding(this, ChartSeries.BindingPathXProperty, pathXBinding);

            //Binding pathsYBinding = new Binding("PathsY") { Source = this.DataModel, Mode = BindingMode.TwoWay };
            //BindingOperations.SetBinding(this, ChartSeries.BindingPathsYProperty, pathsYBinding);

            //Binding dataBinding = new Binding("ChartPoints") { Source = this.DataModel, Mode = BindingMode.TwoWay };
            //BindingOperations.SetBinding(this, ChartSeries.DataProperty, dataBinding);

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSeries" /> class with passed
        /// chart type.
        /// </summary>
        /// <param name="type">Type of <see cref="ChartTypes" /> series.</param>
        /// <example>
        /// C#: <code>
        /// //Creating new instance of chart.
        /// Chart mySampleChart = new Chart();
        /// //Adding new chart area.
        /// mySampleChart.Areas.Add(new ChartArea());
        /// //Series1 initialized with passed chart type.
        /// ChartSeries series = new ChartSeries(ChartTypes.Pie);
        /// //Adding series to chart area.
        /// mySampleChart.Areas[0].Series.Add(series);
        /// </code> XAML: <code>
        /// <para>
        /// You cannot use method in XAML.
        /// </para>
        /// </code>
        /// </example>
        public ChartSeries(ChartTypes type)
            : this()
        {
            this.Type = type;
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Invalidated the series.
        /// </summary>
        /// <remarks>
        /// Method should be used to explicitly invalidate the series. For normal
        /// consequences series is invalidated internally.
        /// </remarks>
        /// <example>
        /// C#: <code>
        /// ChartSeries series = new ChartSeries();
        /// //...
        /// //Setting series content.
        /// //...
        /// //Assigning new label for series;
        /// series.Label = "New series label";
        /// //Invalidating the series.
        /// series.Invalidate();
        /// </code> XAML: <code>
        /// You cannot use method in XAML.
        /// </code>
        /// </example>
        public void Invalidate()
        {
            if (!m_isUpdating)
            {
                m_needUpdateSegments = true;
                m_needUpdateVisualSegments = true;
                m_needUpdateIndexRange = true;

                if (this.Type == ChartTypes.Custom)
                {
                    if (this.Template == null)
                    {
                        m_needUpdateSegments = false;
                        m_needUpdateVisualSegments = false;
                        m_needUpdateIndexRange = false;
                    }
                }
                if (Area != null)
                {
                    this.Recalculate();
                    ////this.Recalculate3D();
                }
            }
        }

        /// <summary>
        /// Begins the update. Calling this method stops series updating.
        /// </summary>
        /// <seealso cref="ChartSeries"/>
        public void BeginUpdate()
        {
            m_isUpdating = true;
           // this.DataModel.BeginInit();
        }

        /// <summary>
        /// Ends the update. Informs series to start updating.
        /// </summary>
        /// <seealso cref="ChartSeries"/>
        public void EndUpdate()
        {
           
              ////Clearing all segments in order to make sure all changed are reflected.
                Segments.Clear();
                ////Disabling update.
                m_isUpdating = false;
               // this.DataModel.EndInit();
                ////Invalidating current series.
                Invalidate();
                Area.UpdateArea();               
            
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        internal bool isDisposed = false;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            //this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            //{      
            isDisposed = true;
            DataChanged = null;
            AppearanceChanged = null;
            MouseMove = null;
            MouseClick = null;
            MouseDown = null;
            MouseEnter = null;
            MouseLeave = null;
            MouseUp = null;
            MouseLeftButtonUp = null;
            MouseRightButtonUp = null;
            MouseLeftButtonDown = null;
            MouseRightButtonDown = null;
            IsZoomableChanged = null;
            TypeChanged = null;
            PropertyChanged = null;
            if (Data != null)
            {
                Data.Dispose();
            }

            this.m_cachedParentArea = null;
            Area = null;
            XAxis = null;
            YAxis = null;
            Data = null;
            Template = null;
            if (this.DataModel != null)
            {
                this.DataModel.ChartPoints.Clear();
                this.DataModel.Dispose();
                this.DataModel.ChartPoints = null;
                this.DataModel.PathX = null;
                this.DataModel.Source = null;
                // this.DataModel.View = null;
                this.DataModel.ContentPath = null;
                this.DataModel.PositionPath = null;
                this.DataModel = null;
            }

            if (m_visibleSegments != null)
            {
                m_visibleSegments.Clear();
            }

            if (this.Presenter != null)
            {
                //this.Presenter.Series = null;
                //this.Presenter.XAxis = null;
                //this.Presenter.YAxis = null;
                this.Presenter = null;
            }

            if (Adornments != null)
            {
                Adornments.Clear();
            }

            if (m_segments3D != null)
            {
                if (m_segments3D.Children != null)
                    m_segments3D.Children.Clear();
            }
            m_adornments = null;

            this.ClearValue(Chart.AnnotationLabelTemplateProperty);

            if (this.Annotations != null)
            {
                this.Annotations.Dispose();
                this.Annotations = null;
            }

            if (this.Indicators != null)
            {
                this.Indicators.Dispose();
                this.Indicators = null;
            }

            if (m_visibleSegments != null)
            {

                foreach (ChartSegment item in m_visibleSegments)
                {
                    item.Dispose();
                    //item.seriesCorrespondingPoints = null;
                }
            }

            this.Data = null;
            this.DataSource = null;
            if (this.m_adornments != null)
            {
                this.m_adornments.Clear();
                this.m_adornments = null;
            }
            this.m_cachedParentArea = null;
            if (this.m_segments3D != null)
            {
                this.m_segments3D = null;
            }

            if (this.m_visibleSegments != null)
            {
                this.m_visibleSegments.Clear();
                this.m_visibleSegments = null;

            }



            if (this.XAxis != null)
            {
                this.XAxis.Dispose();
                this.XAxis = null;
            }

            if (this.YAxis != null)
            {
                this.YAxis.Dispose();
                this.YAxis = null;
            }


            //if (converter != null)
            //{
            //    converter.Dispose();
            //}

            if (ChartType != null)
            {
                //if (ChartType.indexedPointsList != null)
                //{
                //    foreach (ChartIndexedDataPoint item in ChartType.indexedPointsList)
                //    {
                //        if (item.DataPoint != null)
                //        {
                //            item.DataPoint.Dispose();
                //        }
                //    }
                //    ChartType.indexedPointsList.Clear();

                //}
                ChartType.Dispose();
                ChartType = null;
            }

            //if (this.Template != null)
            //{
            //    if (this.Template.Resources != null)
            //    {
            //        this.Template.Resources.MergedDictionaries.Clear();

            //    }
            //}

            if (this.AdornmentsInfo != null)
            {
                this.AdornmentsInfo.Dispose();
                this.AdornmentsInfo = null;
            }
           
            if (this.oldInfo != null)
            {
                this.oldInfo.PropertyChanged -= new DependencyPropertyChangedEventHandler(this.OnAdornmentsPropertyChanged);
            }
            if (this.newInfo != null)
            {
                this.newInfo.PropertyChanged -= new DependencyPropertyChangedEventHandler(this.OnAdornmentsPropertyChanged);
            }
            this.legendicon = ChartLegendIcon.None;
            this.LegendIconTemplate = null;
            //this.Label = null;
            this.VisibilityOnLegend = Visibility.Collapsed;
            this.seriesType = null;
            this.Interior = null;
            this.ClearValue(ChartSeries.TypeProperty);
            this.ClearValue(ChartSeries.LegendIconProperty);
            this.Template = null;
            this.ClearValue(ChartSeries.InteriorProperty);
            this.ClearValue(ChartSeries.StrokeProperty);
            this.Resources.Clear();
            this.Data = null;
            this.DataModel = null;
            this.BindingPathX = null;
            this.BindingPathsY = null;
            this.Area = null;
           
            if (this.FastSegmentProperties != null)
            {
                this.FastSegmentProperties.Clear();
                this.FastSegmentProperties = null;
            }
            //}));
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Returns known chart type instance by passed enum ChartTypes.
        /// </summary>
        /// <param name="type"><see cref="ChartTypes"/> parameter.</param>
        /// <returns>Returns <see cref="ChartType"/>.</returns>
        internal static ChartType KnownType(ChartTypes type)
        {
            ChartType chart;
            switch (type)
            {
                case ChartTypes.Column:
                    chart = new ChartColumnType();
                    break;
                case ChartTypes.Bar:
                    chart = new ChartBarType();
                    break;
                case ChartTypes.HiLoArea:
                    chart = new ChartHiLoAreaType();
                    break;
                case ChartTypes.RangeColumn:
                    chart = new ChartRangeColumnType();
                    break;
                case ChartTypes.StackingColumn:
                    chart = new ChartStackingColumnType();
                    break;
                case ChartTypes.StackingColumn100:
                    chart = new ChartStackingColumn100Type();
                    break;
                case ChartTypes.StackingBar100:
                    chart = new ChartStackingBar100Type();
                    break;
                case ChartTypes.Line:
                    chart = new ChartLineType();
                    break;
                case ChartTypes.FastLine:
                    chart = new ChartFastLineType();
                    break;
                case ChartTypes.FastSpline:
                    chart = new ChartFastSplineType();
                    break;
                case ChartTypes.FastColumn:
                    chart = new ChartFastColumnType();
                    break;
                case ChartTypes.FastScatter:
                    chart = new ChartFastScatterType();
                    break;
                case ChartTypes.FastStackingColumn:
                    chart = new ChartFastStackingColumnType();
                    break;
                case ChartTypes.FastHiLoOpenClose:
                    chart = new ChartFastHiLoOpenCloseType();
                    break;
                case ChartTypes.FastBar:
                    chart = new ChartFastBarType();
                    break;
                case ChartTypes.Spline:
                    chart = new ChartSplineType();
                    break;
                case ChartTypes.RotatedSpline:
                    chart = new ChartRotatedSplineType();
                    break;
                case ChartTypes.Scatter:
                    chart = new ChartScatterType();
                    break;
                case ChartTypes.Gantt:
                    chart = new ChartGanttType();
                    break;
                case ChartTypes.StackingBar:
                    chart = new ChartStackingBarType();
                    break;
                case ChartTypes.Area:
                    chart = new ChartAreaType();
                    break;
                case ChartTypes.StackingArea:
                    chart = new ChartStackingAreaType();
                    break;
                case ChartTypes.StackingArea100:
                    chart = new ChartStackingArea100Type();
                    break;
                case ChartTypes.StackingLine:
                    chart = new ChartStackingLineType();
                    break;
                case ChartTypes.StackingLine100:
                    chart = new ChartStackingLine100Type();
                    break;
                case ChartTypes.StackingSpline:
                    chart = new ChartStackingSplineType();
                    break;
                case ChartTypes.StackingSpline100:
                    chart = new ChartStackingSpline100Type();
                    break;
                case ChartTypes.StackingSplineArea:
                    chart = new ChartStackingSplineAreaType();
                    break;
                case ChartTypes.StackingSplineArea100:
                    chart = new ChartStackingSplineArea100Type();
                    break;
                case ChartTypes.SplineArea:
                    chart = new ChartSplineAreaType();
                    break;
                case ChartTypes.Pie:
                    chart = new ChartPieType();
                    break;
                case ChartTypes.HiLo:
                    chart = new ChartHiLoType();
                    break;
                case ChartTypes.HiLoOpenClose:
                    chart = new ChartHiLoOpenCloseType();
                    break;
                case ChartTypes.Candle:
                    chart = new ChartCandleType();
                    break;
                case ChartTypes.Bubble:
                    chart = new ChartBubbleType();
                    break;
                case ChartTypes.StepLine:
                    chart = new ChartStepLineType();
                    break;
                case ChartTypes.StepArea:
                    chart = new ChartStepAreaType();
                    break;
                case ChartTypes.Radar:
                    chart = new ChartRadarType();
                    break;
                case ChartTypes.Kagi:
                    chart = new ChartKagiType();
                    break;
                case ChartTypes.Renko:
                    chart = new ChartRenkoType();
                    break;
                case ChartTypes.Polar:
                    chart = new ChartPolarType();
                    break;
                case ChartTypes.ThreeLineBreak:
                    chart = new ChartThreeLineBreakType();
                    break;
                case ChartTypes.PointAndFigure:
                    chart = new ChartPointAndFigureType();
                    break;
                case ChartTypes.BoxAndWhisker:
                    chart = new ChartBoxAndWhiskerType();
                    break;
                case ChartTypes.Histogram:
                    chart = new ChartHistogramType();
                    break;
                case ChartTypes.Tornado:
                    chart = new ChartTornadoType();
                    break;
                case ChartTypes.Doughnut:
                    chart = new ChartDoughnutType();
                    break;
                case ChartTypes.Pyramid:
                    chart = new ChartPyramidType();
                    break;
                case ChartTypes.Funnel:
                    chart = new ChartFunnelType();
                    break;
                case ChartTypes.RangeArea:
                    chart = new ChartRangeAreaType();
                    break;
                case ChartTypes.Surface3D:
                    chart = new ChartSurfaceType();
                    break;
                default:
                    chart = new ChartColumnType();
                    break;
            }

            return chart;
        }

        /// <summary>
        /// Gets the series data point at specified index.
        /// </summary>
        /// <param name="index">Index of chart data point at specified positions.</param>
        /// <returns>
        /// Returns <see cref="IChartDataPoint" /> value.
        /// </returns>
        /// <example>
        /// C#: <code>
        /// ChartSeries series = new ChartSeries();
        /// //...
        /// //Filling series with points.
        /// //...
        /// //Getting second point of series.
        /// ChartPoint point = series.GetPoint(1);
        /// </code> XAML: <code language="XAML">
        /// You cannot use method in XAML.
        /// </code>
        /// </example>
        internal IChartDataPoint GetPoint(int index)
        {
            IChartDataPoint icdp = null;
            var data = this.Data;
            bool IsNegative = false;
            var values1 = data[index].Values;
            var values2 = index+1 < data.Count ? data[index + 1].Values : values1;
            int idx = index;
            if (data != null)
            {
                
                if (data[index] != null)
                {
                    #region Handle Empty points
                    switch (double.IsNaN(values1[0]) || (values1.Length > 1 && double.IsNaN(values1[1])) || (values1.Length > 2 && double.IsNaN(values1[2])) || (values1.Length > 3 && double.IsNaN(values1[3])) || double.IsPositiveInfinity(data[index].Y) || double.IsNegativeInfinity(data[index].Y) || data[index].EmptyPoint)
                    {
                        case true:
                            {

                                //}
                                //if (double.IsNaN(values1[0])||(values1.Length > 1 && double.IsNaN(values1[1]))||(values1.Length > 2 && double.IsNaN(values1[2]))|| (values1.Length > 3 && double.IsNaN(values1[3]))|| double.IsPositiveInfinity(data[index].Y) || double.IsNegativeInfinity(data[index].Y) || data[index].EmptyPoint)
                                //{                        

                                //if (data[index].EmptyPoint)
                                //{
                                //     data[index].Y = double.NaN;
                                //}

                                if (this.EmptyPointValue == Windows.Chart.EmptyPointValue.Average)
                                {
                                    ////If NaN or infinte values, get the previous and next points average and use it
                                    if ((index + 1) > 0 && data.Count >= (index + 1))
                                    {
                                        //if this is the last point
                                        if ((index + 1) == data.Count)
                                        {
                                            if (values1 != null && values1.Length >= 0)
                                            {
                                                //This condition is included to avoid Outof range exception when it is first value of the collection
                                                if (index > 0)
                                                {
                                                    bool isnan=false;
                                                    for(int i=0;i<index && isnan==false ;i++)
                                                    {
                                                        if (!double.IsNaN(data[index - i].Y))
                                                        {
                                                            nextToEmptyPoint = data[index - i].Values;
                                                            isnan = true;
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    nextToEmptyPoint = values1;
                                                }
                                            }
                                            else
                                            {
                                                nextToEmptyPoint = new double[] { 0 };
                                            }
                                        }
                                        else if ((!double.IsNaN(data[index + 1].Y) || !double.IsNaN(values2[0]) || (values2.Length > 1 && !double.IsNaN(values2[1])) || (values2.Length > 2 && !double.IsNaN(values2[2])) || (values2.Length > 2 && !double.IsNaN(values2[3]))) && !(data[index + 1].EmptyPoint))
                                        {
                                            ////if a point is Nan anad has both previous and next points
                                            nextToEmptyPoint = values2;
                                        }
                                        else
                                        {
                                            ////if consecutive points are Nan
                                            for (int i = index; i < data.Count; i++)
                                            {
                                                if ((!double.IsNaN(data[i].Y) || !double.IsNaN(data[i].Values[0]) || (data[i].Values.Length > 1 && !double.IsNaN(data[i].Values[1])) || (data[index].Values.Length > 2 && !double.IsNaN(data[i].Values[2])) || (values1.Length > 2 && !double.IsNaN(data[i].Values[3]))) && !(data[index + 1].EmptyPoint))
                                                {
                                                    nextToEmptyPoint = data[i].Values;
                                                    break;
                                                }
                                                else if (index > 0)
                                                {
                                                    bool isnan = false;
                                                    for (int j = 0; j < index && isnan == false; j++)
                                                    {
                                                        if (!double.IsNaN(data[index - j].Y))
                                                        {
                                                            nextToEmptyPoint = data[index - j].Values;
                                                            isnan = true;
                                                        }

                                                    }
                                                }
                                            }
                                           // nextToEmptyPoint = new double[] { 0 };
                                        }

                                        if (nextToEmptyPoint != null)
                                        {
                                            double[] yValues = new double[nextToEmptyPoint.Length];
                                            for (int i = 0; i < nextToEmptyPoint.Length; i++)
                                            {
                                                ////if this is the first point, consider 0 as previous value
                                                if ((index - 1) < 0)
                                                {
                                                    if (this.Type == ChartTypes.FastLine|| this.Type==ChartTypes.FastBar || this.Type==ChartTypes.FastColumn || this.Type==ChartTypes.FastHiLoOpenClose ||this.Type==ChartTypes.FastScatter||this.Type==ChartTypes.FastStackingColumn)
                                                    {
                                                        yValues[i] = (nextToEmptyPoint[i] - 0) / 2;
                                                    }
                                                    else
                                                    {
                                                        yValues[i] = ShowEmptyPoints ? (nextToEmptyPoint[i] - 0) / 2 + 0 : 0;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.Type == ChartTypes.FastLine || this.Type == ChartTypes.FastBar || this.Type == ChartTypes.FastColumn || this.Type == ChartTypes.FastHiLoOpenClose || this.Type == ChartTypes.FastScatter || this.Type == ChartTypes.FastStackingColumn)
                                                    {
                                                        yValues[i] = (nextToEmptyPoint[i] - data[index - 1].Values[i]) / 2 + data[index - 1].Values[i];
                                                    }
                                                    else
                                                    {
                                                        yValues[i] = ShowEmptyPoints ? (nextToEmptyPoint[i] - data[index - 1].Values[i]) / 2 + data[index - 1].Values[i] : 0;
                                                    }
                                                }
                                            }
                                            //WPF-9336:Empty point range calculation issue fixed
                                            if (!ShowEmptyPoints)
                                            {
                                                for (int i = 0; i < data.Count; i++)
                                                {
                                                    if (data[i].Y < 0)
                                                    {
                                                        IsNegative = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (IsNegative)
                                                yValues[0] = data[index].Y;

                                            data[index].Values = yValues;
                                            data[index].EmptyPoint = true;
                                            this.Contains_emptypt = true;
                                        }
                                        else
                                        {
                                            double[] yValues = this.BindingPathsY!=null? new double[this.BindingPathsY.Count()]: new double[]{0};
                                            if (this.BindingPathsY != null)
                                            for (int i = 0; i < this.BindingPathsY.Count(); i++)
                                            {                                                
                                                    yValues[i] = 0;                                                
                                            }
                                            data[index].Values = yValues;
                                            data[index].EmptyPoint = true;
                                            this.Contains_emptypt = true;
                                        }
                                    }

                                    else
                                    {
                                        data[index].Y = data[index - 1].Y;
                                    }

                                    icdp = data[index];
                                }
                                else
                                {
                                    //WPF-9336:Empty point range calculation issue fixed
                                    if (!ShowEmptyPoints)
                                        for (int i = 0; i < data.Count; i++)
                                        {
                                            if (data[i].Y < 0)
                                            {
                                                IsNegative = true;
                                                break;
                                            }
                                        }

                                    if (!IsNegative)
                                    data[index].Y = 0;
                                    //data[index].Values[1] = 0;
                                    //data[index].Values[2] = 0;
                                    //data[index].Values[3] = 0;
                                    //data[index].Y = 0;
                                    icdp = data[index];
                                }
                                break;
                            }




                    #endregion
                        case false:
                            //else
                            {
                                icdp = data[index];
                                break;
                            }

                    }
                    ////Checking whether it's axis dependent chart type and we're having 1 series only.
                    if (m_cachedParentArea != null && m_cachedParentArea.IsIndexedCompatible && ChartType.RequiresAxis)
                    {
                        double x = this.DataSource != null ? icdp.X : index;
                        //Here i have changed "data[index].Item" to "data[index].Tag" because "Item" should not be assigned to Tag 
                        ChartPoint cp = new ChartPoint(x, icdp.Values) { Tag = data[index].Tag };
                        cp.Item = data[index].Item;
                        if (icdp.EmptyPoint)
                        {
                            cp.EmptyPoint = true;
                            this.Contains_emptypt = true;
                        }

                        icdp = cp;
                    }
                }
            }

            return icdp;
        }

        /// <summary>
        /// Determines whether the specified point should be visible on area.
        /// </summary>
        /// <returns>The bool value indicating whether the point is visible</returns>
        /// <param name="point">The point.</param>
        /// <param name="isXLogarithmic"></param>
        /// <param name="isYLogarithmic"></param>
        internal bool IsPointVisible(IChartDataPoint point, bool isXLogarithmic, bool isYLogarithmic)
        {
            ////Determining whether Axis.Range property is set, data point falls in range 
            ////and chart is being drawn in cartesian coordinate system with positive values in points for Log axes.
            double x = point.X, y = point.Y;
            ////Caching x axis;
            ChartAxis xAxis = XAxis;
            bool inRange = xAxis == null ? true : xAxis.InternalRange.IsEmpty || xAxis.InternalRange.Inside(x);
            bool isPointVisible = !double.IsNaN(x) && !double.IsNaN(y) && (inRange || ChartType.AxesType != ChartAxesType.CartesianAxes);
            if (YAxis != null && isYLogarithmic)
            {
                return y > 0 && isPointVisible;
            }
            else if (xAxis != null && isXLogarithmic)
            {
                return x > 0 && isPointVisible;
            }

            return isPointVisible;
        }

        /// <summary>
        /// Recalculates the 3D series.
        /// </summary>
        internal void Recalculate3D()
        {
            if (this.Segments != null)
            {
                Segments3D.Children.Clear();
                this.Update3D();
                isLastSeries = false;
            }
        }
        internal bool isLastSeries = false;
        /// <summary>
        /// Updates 3D Series.
        /// </summary>
        /// <seealso cref="ChartSeries"/>
        internal void Update3D()
        {
            if (this.Area.View3DMode && this.Area.AreaType != ChartAxesType.PolarAxes)
            {
                IChartTransformer transformer = ChartTransform.CreateTransformer(Area.AreaType, new Rect(0, 0, 1, 1), this);
                Segments3D.Children.Clear();
                if (!(this.Type == ChartTypes.Line || this.Type == ChartTypes.Area || this.Type == ChartTypes.Column || this.Type == ChartTypes.Bar))
                {
                    Area.isClustered = true;
                }
                else if (Area.IsClustered == false)
                {
                    Area.isClustered = false;
                }

                if (Area.Series.Count <= 1)
                {
                    isLastSeries = true;
                }
                if (!this.Area.IsClustered && Area.Series.Count > 1)
                {
                    this.Area.clusterd = true;
                }
                m_needUpdateSegments = true;
                if (this.Type != ChartTypes.Funnel && this.Type != ChartTypes.Pie && this.Type != ChartTypes.Pyramid && this.Type != ChartTypes.Doughnut)
                {
                    Recalculate();
                }
                foreach (ChartSegment cid in this.Segments)
                {
                    cid.Draw3DSegment(transformer);
                    Segments3D.Children.Add(cid.Geometry3D);
                    Segments3D.Children.Add(cid.Geometry3DGroup);
                }

            }
            else
            {
                this.Area.clusterd = false;
                m_needUpdateSegments = true;
                Recalculate();
            }
        }

        /// <summary>
        /// Raises MouseHover event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseMove(object sender, ChartMouseEventArgs args)
        {
            if (MouseMove != null)
            {
                MouseMove(sender, args);
            }
        }

        /// <summary>
        /// Raises MouseDown event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseDown(object sender, ChartMouseEventArgs args)
        {
            if (MouseDown != null)
            {
                MouseDown(sender, args);
                ChartSeries series = sender as ChartSeries;
                if (series != null)
                {
                    series.SelectedItem = args.Segment;
                }
            }
           
            
        }

        internal void OnMouseDoubleClick(object sender, ChartMouseEventArgs args)
        {
            if (MouseDoubleClick != null)
            {
                MouseDoubleClick(sender, args);
            }
        }


        /// <summary>
        /// Raises MouseEnter event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseEnter(object sender, ChartMouseEventArgs args)
        {
            this.Highlighted = true;            
            if (MouseEnter != null)
            {
                MouseEnter(sender, args);
            }
        }

        /// <summary>
        /// Raises MouseLeave event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseLeave(object sender, ChartMouseEventArgs args)
        {
            this.Highlighted = false;            
            if (MouseLeave != null)
            {
                MouseLeave(sender, args);
                ChartSeries series = sender as ChartSeries;
                if (series != null)
                {
                    series.SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// Raises MouseUp event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseUp(object sender, ChartMouseEventArgs args)
        {
            if (MouseUp != null)
            {
                MouseUp(sender, args);
            }
        }

        /// <summary>
        /// Raises MouseClick event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseClick(object sender, ChartMouseEventArgs args)
        {
            if (MouseClick != null)
            {
                MouseClick(sender, args);
            }
        }

        /// <summary>
        /// Raises MouseLeftButtonUp event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseLeftButtonUp(object sender, ChartMouseEventArgs args)
        {
            if (MouseLeftButtonUp != null)
            {
                MouseLeftButtonUp(sender, args);
            }
        }

        /// <summary>
        /// Raises MouseLeftButtonDown event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseLeftButtonDown(object sender, ChartMouseEventArgs args)
        {
            if (MouseLeftButtonDown != null)
            {
                MouseLeftButtonDown(sender, args);
            }
        }

        /// <summary>
        /// Raises MouseRightButtonUp event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseRightButtonUp(object sender, ChartMouseEventArgs args)
        {
            if (MouseRightButtonUp != null)
            {
                MouseRightButtonUp(sender, args);
            }
        }

        /// <summary>
        /// Raises MouseRightButtonDown event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The ChartMouseEvent arguments</param>
        internal void OnMouseRightButtonDown(object sender, ChartMouseEventArgs args)
        {
            if (MouseRightButtonDown != null)
            {
                MouseRightButtonDown(sender, args);
            }
        }

        #endregion

        #region Protected methods
        /// <summary>
        /// Updates property value cache and raises TypeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.m_visibleSegments != null)
            {
                foreach (ChartSegment item in this.m_visibleSegments)
                {
                    item.Dispose();
                }

                this.m_visibleSegments.Clear();
            }

            ChartTypes newtype = (ChartTypes)e.NewValue;
            ChartType type = newtype != ChartTypes.Custom ? KnownType(newtype) : this.ChartType;
            if (newtype == ChartTypes.Custom)
            {
                this.Adornments.Clear();
            }

            if (XAxis != null && YAxis != null)
            {
                if (type.IsRotated && XAxis.Orientation != Orientation.Vertical)
                {
                    XAxis.Orientation = Orientation.Vertical;
                    YAxis.Orientation = Orientation.Horizontal;
                    if (Area.ZoomAllAxes || IsZoomable)
                    {
                        Area.VerticalScrollingAxis = XAxis;
                        Area.HorizontalScrollingAxis = YAxis;
                    }
                }

                if (!type.IsRotated && XAxis.Orientation == Orientation.Vertical)
                {
                    XAxis.Orientation = Orientation.Horizontal;
                    YAxis.Orientation = Orientation.Vertical;
                    if (Area.ZoomAllAxes || IsZoomable)
                    {
                        Area.HorizontalScrollingAxis = XAxis;
                        Area.VerticalScrollingAxis = YAxis;
                    }
                }
            }

            this.ChartType = type;
            if (TypeChanged != null)
            {
                TypeChanged(this, e);
            }
            UpdateLegendIcon(this);
        }

        /// <summary>
        /// Updates property value cache.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnChartTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            ChartType type = (ChartType)e.NewValue;
            if (type != null)
            {
                if (Enum.IsDefined(typeof(ChartTypes), type.ToString()))
                {
                    this.Type = (ChartTypes)Enum.Parse(typeof(ChartTypes), type.ToString());
                }
                seriesType = type.ToString();
                IsTypeChanging = false;
            }

        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"></see> has been updated. The specific dependency property that changed is reported in the event data.
        /// </summary>
        /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            PropertyMetadata propertyMetadata = e.Property.GetMetadata(this.GetType()) as PropertyMetadata;
            if (e.Property == IsVisibleProperty || e.Property == ChartTypeProperty || e.Property == VisibilityOnLegendProperty || e.Property== ZOrderProperty)
            {
                if (Area != null && Area.VisibleSeries != null)
                {
                    Area.VisibleSeries.Refresh();
                }
            }

            if (e.Property == ChartTypeProperty)
            {
                if (m_visibleSegments != null)
                    m_visibleSegments.Clear();
                if (Adornments != null)
                    Adornments.Clear();
            }

            bool affectUpdate = (e.Property.ToString() == "BreakLineForNonIndexedData" || e.Property.ToString() == "BreakLineForDoublePointsDistanceMoreThan" || e.Property.ToString() == "BreakLineForTimeSpanPointsDistanceMoreThan" ||
                e.Property.ToString() == "DoughnutCoefficient" || e.Property.ToString() == "PieCoefficient" || e.Property.ToString() == "PyramidMode" || e.Property.ToString() == "GapRatio" || e.Property.ToString() == "ExplodedAll" || e.Property.ToString() == "ExplodeRadius" || e.Property.ToString() == "ExplodedIndex" || e.Property == ChartTypeProperty || e.Property == AdornmentsInfoProperty || e.Property == ShowEmptyPointsProperty || e.Property == IsVisibleOnLegendProperty ||
                e.Property == EmptyPointInteriorProperty || e.Property == EmptyPointStyleProperty || e.Property.ToString() == "Template") ? true : false || e.Property.ToString() == "DrawNormalDistribution";
           
                if (e.Property.ToString() == "Stroke")
                {
                    if (Adornments != null && AdornmentsInfo.SymbolStroke == null)
                    {
                        AdornmentSymbolStroke = Stroke;
                    }
                }
                if (e.Property.ToString() == "StrokeThickness")
                {
                    if (Adornments != null && double.IsNaN(AdornmentsInfo.SymbolStrokeThickness))
                    {
                        AdornmentSymbolStrokeThickness = StrokeThickness;
                    }
                }
                if (e.Property.ToString() == "Interior")
                {
                    if (Adornments != null && AdornmentsInfo.SymbolInterior == null)
                    {
                        AdornmentSymbolInterior = Interior;
                    }
                }
            if (propertyMetadata != null)
            {
                if (affectUpdate == true)
                {
                    this.Invalidate();
                    this.RaiseAppearanceChanged(this, EventArgs.Empty);
                }
            }

            //if (e.Property == IsVisibleOnLegendProperty && this.Area != null)
            //{
            //    //  this.Area.OnApplyTemplate();
            //}
            if (e.Property == AnnotationsProperty)
            {
                if (this.Area != null && this.Area.annotationAdorner != null)
                {
                    this.Area.annotationAdorner.AnnotationReset();
                }
            }
            //if (e.Property == IndicatorsProperty)
            //{
            //    //if (this.Area != null && this.Area.indicatorAdorner != null)
            //    {
            //        //this.Area.indicatorAdorner.IndicatorReset();
            //    }
            //}

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            base.OnPropertyChanged(e);

        }

        #endregion

        #region Implementation

        /// <summary>
        /// Calls OnTypeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries instance = (ChartSeries)d;
            if (instance != null)
                instance.OnTypeChanged(e);
        }


        private static object OnTypeChanging(DependencyObject d, object value)
        {            
            ChartSeries instance = d as ChartSeries;
            if (instance != null)
            {
                if (instance.TypeChanging != null)
                {
                    if (instance.IsTypeChanging == false && instance.Type.ToString()!=value.ToString())
                    {
                        TypeChangingEventArgs changing = new TypeChangingEventArgs(instance.Type, value);
                        instance.TypeChanging(d, changing);
                        instance.IsTypeChanging = true;
                    }
                }
            }
            return value;
        }

       
        /// <summary>
        /// Calls OnTypeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnChartTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries instance = (ChartSeries)d;


            if (instance != null)
            {
                if (instance.EnableAnimation == true && instance.Animation != null)
                {
                    if (instance.Animation.Storyboard != null)
                    {
                        instance.Animation.Storyboard.Stop();
                        instance.Animation.Storyboard.Children.Clear();
                    }
                    if (instance.Animation.AdornmentStoryboard != null)
                    {
                        instance.Animation.AdornmentStoryboard.Stop();
                        instance.Animation.AdornmentStoryboard.Children.Clear();
                    }
                }

                instance.OnChartTypeChanged(e);
            }
        }

        /// <summary>
        /// Used to invoke the Chart Series Animation
        /// </summary>
        /// <seealso cref="ChartSeries"/>
        public void StartAnimation()
        {
            if (this.Animation != null && this.EnableAnimation == true)
            {
                this.Animation.AnimateSeries();
            }
        }

        /// <summary>
        /// Called when area dependency property changed.
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAreaPropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = dpObj as ChartSeries;
            if (series != null)
            {
                ////Caching correspoiding area property in order aviod DP property lookup.
                series.m_cachedParentArea = e.NewValue as ChartArea;
                if (series.Data != null)
                series.Invalidate();
            }
        }

        /// <summary>
        /// Called when need to coerce adornments info.
        /// </summary>
        /// <param name="d">The d value.</param>
        /// <param name="value">The object value.</param>
        /// <returns>The adornments info</returns>
        private static object OnCoerceAdornmentsInfo(DependencyObject d, object value)
        {
            ChartSeries ser = d as ChartSeries;
            if (ser.isDisposed == false)
            {
                return value == null ? new ChartAdornmentInfo() : value;
            }
            else
            {
                return null;
            }
        }
        private static object OnCoerceYAxis(DependencyObject d, object value)
        {
            ChartSeries ser = d as ChartSeries;
            
            if (ser.isDisposed == false)
            {
                if (value == null)
                {
                    Binding binding = new Binding();

                    binding.Path = new PropertyPath(ChartArea.SecondaryAxisProperty);
                    binding.Source = ser.Area;

                    BindingOperations.SetBinding(ser, ChartSeries.YAxisProperty, binding);
                }
                return value == null ? ser.Area.SecondaryAxis: value;
            }
            else
            {
                return null;
            }
        }
        private static object OnCoerceXAxis(DependencyObject d, object value)
        {
            ChartSeries ser = d as ChartSeries;
           
            if (ser.isDisposed == false)
            {
                if (value == null)
                {
                    Binding binding = new Binding();

                    binding.Path = new PropertyPath(ChartArea.PrimaryAxisProperty);
                    binding.Source = ser.Area;

                    BindingOperations.SetBinding(ser, ChartSeries.XAxisProperty, binding);
                }
                return value==null?ser.Area.PrimaryAxis:value;
            }
            else
            {
                return null;
            }
        }
        private static object OnCoerceZAxis(DependencyObject d, object value)
        {
            ChartSeries ser = d as ChartSeries;
           
            if (ser.isDisposed == false)
            {
                if (value == null && ser.Area != null)
                {
                    if (ser.Area.DepthAxis != null)
                    {
                        Binding binding = new Binding();

                        binding.Path = new PropertyPath(ChartArea.DepthAxisProperty);
                        binding.Source = ser.Area;

                        BindingOperations.SetBinding(ser, ChartSeries.ZAxisProperty, binding);
                    }

                    return value == null ? ser.Area.DepthAxis : value;
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Appearance property changed event handler.
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs e</param>
        private static void OnAppearancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.DataModel != null)
            {
                if (e.Property == IsIndexedProperty)
                {
                    series.DataModel.IsIndexed = series.IsIndexed;
                }

                else if (e.Property == IsSortDataProperty)
                {
                    series.IsSortData = (bool)e.NewValue;
                }

                else if (e.Property == SortDirectionProperty)
                {
                    series.SortDirection = (Direction)e.NewValue;
                }

                else if (e.Property == SortByProperty)
                {
                    series.SortBy = (SortingAxis)e.NewValue;
                }
            }

            if (series.Area != null && e.Property == XAxisProperty && series.XAxis != null)
            {
                if (!series.Area.Axes.Contains(series.XAxis))
                {
                    series.Area.Axes.Add(series.XAxis);
                }
            }

            if (series.Area != null && e.Property == YAxisProperty && series.YAxis != null)
            {
                if (!series.Area.Axes.Contains(series.YAxis))
                {
                    series.Area.Axes.Add(series.YAxis);
                }
            }

            if (series.Area != null && e.Property == IsIndexedProperty)
            {
                if (series.Segments != null)
                {
                    series.Segments.Clear();
                }
                series.Adornments.Clear();
            }

            if (series != null)
            {
                if (series.Area != null)
                    series.Area.UpdateArea();
                series.RaiseAppearanceChanged(series, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles invalidate property changed event.
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs e</param>
        private static void OnInvalidatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;

            if (series != null)
            {
                series.Invalidate();
                series.RaiseAppearanceChanged(series, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles data changed event.
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs e</param>
        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            IChartData oldData = null, newData = null;
            if (e.OldValue is IChartData)
            {
                oldData = e.OldValue as IChartData;
            }

            if (e.NewValue is IChartData)
            {
                newData = e.NewValue as IChartData;
            }

            if (series != null)
            {
                if (oldData != null)
                {
                    oldData.CollectionChanged -= new NotifyCollectionChangedEventHandler(series.OnDataChanged);
                    series.Segments.Clear();
                }

                if (newData != null)
                {
                    newData.CollectionChanged += new NotifyCollectionChangedEventHandler(series.OnDataChanged);
                }

                series.CoerceValue(IsIndexedProperty);
                series.RaiseAppearanceChanged(series, EventArgs.Empty);
                series.RaiseDataChanged(series, EventArgs.Empty);
            }


        }
        private ChartAdornmentInfo oldInfo; 
        private ChartAdornmentInfo newInfo;
        /// <summary>
        /// Called when adornments info is changed.
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAdornmentsInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;

            series.oldInfo = e.OldValue as ChartAdornmentInfo;
            series.newInfo = e.NewValue as ChartAdornmentInfo;

            if (series != null)
            {
                if (series.oldInfo != null)
                {
                    series.oldInfo.PropertyChanged -= new DependencyPropertyChangedEventHandler(series.OnAdornmentsPropertyChanged);
                }

                if (series.newInfo != null)
                {
                    series.newInfo.PropertyChanged += new DependencyPropertyChangedEventHandler(series.OnAdornmentsPropertyChanged);
                    BindingUtils.SetBinding(series, series.newInfo, ChartSeries.ShowDataLabelsProperty, ChartAdornmentInfo.VisibleProperty, BindingMode.OneWay);
                }
            }
        }

        private static void OnEaseAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Presenter != null && series.Animation != null && series.EnableAnimation == true)
            {
                series.Animation.AnimateSeries();
            }
        }

        private static void OnEnableEffectsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.ChartType != null && series.Presenter != null)
            {
                series.BeginUpdate();
                series.EndUpdate();
            }
        }

        private static void OnEnableAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Presenter != null)
            {
                if (series.EnableAnimation == true)
                {
                    series.Animation = new ChartAnimation(series);
                    series.Animation.AnimateSeries();
                    series.Animation.AnimateAdornment(series.AdornmentPresenter);
                }
                else
                {
                    series.Animation.Storyboard.Stop();
                    series.Animation.Storyboard.Children.Clear();
                    series.Animation = null;
                }
            }
            else
            {
                series.Animation = new ChartAnimation(series);
            }
        }

        /// <summary>
        /// Called when data source gets changed.
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ////Retrieving series instance.
            ChartSeries series = d as ChartSeries;
            if (series != null)
            {
                if (e.OldValue == null)
                {
                    Binding dataBinding = new Binding("ChartPoints") { Source = series.DataModel, Mode = BindingMode.OneWay };
                    BindingOperations.SetBinding(series, ChartSeries.DataProperty, dataBinding);
                }
                if (series.DataModel != null)
                {                    
                    series.DataModel.Source = series.DataSource;
                }
                ////Calling data changed method to handle changes for CollectionView.
                series.DataSourceChanged(e.OldValue, e.NewValue);
            }
        }

        private static void OnBindingPathXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.DataModel != null)
            {
                if (string.IsNullOrEmpty(series.BindingPathX))
                {
                    series.Data = null;
                }
                else
                {
                    series.DataModel.PathX = series.BindingPathX;
                }
            }
        }
        private static void OnHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null)
            {

            }
        }
        private static void OnBindingPathsYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null && series.DataModel != null)
            {
                if (series.BindingPathsY == null)
                {
                    series.Data = null;
                }
                else
                    series.DataModel.PathsY = series.BindingPathsY;
            }
        }

        /// <summary>
        /// Called to coerce selected item and syncronize it with CollectionView's CurrentItem.
        /// </summary>
        /// <param name="dpObj">The dp obj.</param>
        /// <param name="value">The object value.</param>
        /// <returns>Returns the selected Item</returns>
        private static object OnSelectedItemCoerce(DependencyObject dpObj, object value)
        {
            ////Retrieving series.
            ChartSeries series = dpObj as ChartSeries;
            if (series != null && series.EnableSelection)
            {
                CollectionView collection = series.DataSource as CollectionView;
                ////Checking for correct datasource
                if (collection != null)
                {
                    ////Trying to set Current item of collection to received value.
                    //if (collection.MoveCurrentTo(value))
                    //SD16050 - IsSelected property is not working properly.
                    if (collection.MoveCurrentTo((value as ChartSegment).CorrespondingPoints[0].DataPoint.Item))
                    {
                        return value;
                    }
                    else
                    {
                        ////If CollectionView doesn't have such value - set SelectedItems to unset value.
                        return DependencyProperty.UnsetValue;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsAutoDiscardPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }


        /// <summary>
        /// Called to coerce value of IsIndexed property.
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="value">The object value.</param>
        /// <returns>The IsIndexed value</returns>
        private static object OnIsIndexedCoerce(DependencyObject d, object value)
        {
            ChartSeries series = d as ChartSeries;
            IChartData data = series.Data;
            if (data != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (double.IsNaN(data[i].X))
                    {
                        return DependencyProperty.UnsetValue;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Gets called to coerce Interior property regarding IsZoomable property.
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="value">The object value</param>
        /// <returns>Returns the interior</returns>
        private static object OnInteriorCoerce(DependencyObject d, object value)
        {
            if (value != null)
            {
                ChartSeries chartSeries = d as ChartSeries;

                if (chartSeries != null && chartSeries.Area != null && chartSeries.Area.SyncChartArea == null)
                {
                    if (!chartSeries.IsZoomable && !chartSeries.Area.ZoomAllAxes && ((chartSeries.XAxis.ZoomFactor < 1 || chartSeries.XAxis.ZoomPosition > 0) || (chartSeries.YAxis.ZoomFactor < 1 || chartSeries.YAxis.ZoomPosition > 0)))
                    {
                        ////Returns transparent brush to series if it's zoomable.
                        Brush transparentBrush = (value as Brush).Clone();
                        transparentBrush.Opacity = chartSeries.InactiveSeriesOpacityOnZoom;
                        return transparentBrush;
                    }
                }
                else
                {
                    if (chartSeries.Area != null)
                    {
                        if (chartSeries.Area.SyncChartArea != null)
                        {
                            if (!chartSeries.Area.SyncChartArea.ZoomAllAxes)
                            {
                                ////Returns transparent brush to series if it's zoomable.
                                Brush transparentBrush = (value as Brush).Clone();
                                transparentBrush.Opacity = chartSeries.InactiveSeriesOpacityOnZoom;
                                return transparentBrush;
                            }
                        }
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Handles ColorEach changed event.
        /// </summary>
        /// <param name="d">The object d</param>
        /// <param name="e">The NotifyCollectionChangedEventArgs e</param>
        private static void OnColorEachInteriorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;

            if (series != null && series.Area != null && series.ColorEachDependent && series.ColorEach != null && series.Segments.Count > 1)
            {
                foreach (ChartSegment segment in series.Segments)
                {
                    if (!series.Area.View3DMode)
                        series.UpdateColorEachSegments(series, segment, series.Segments.IndexOf(segment), null);
                    else
                        segment.Draw3DSegment(ChartTransform.CreateTransformer(series.Area.AreaType, new Rect(0, 0, 1, 1), series));
                }
            }
        }

        internal void UpdateColorEachSegments(ChartSeries series, ChartSegment segment, int colorIndex, DiffuseMaterial diffuseMaterial)
        {
            if (series.LegendIconTemplate == null)
            {
                UpdateLegendIcon(series);
            }
            if (segment is ChartCandleSegment)
            {
                ChartCandleSegment candleSegment = (ChartCandleSegment)segment;
                if ((bool)series.ColorEach)
                {
                    ChartStyleModel m_model, m_strokeModel;
                    if (series.Palette == ChartColorPalette.Custom && series.CustomPalette != null)
                    {
                        m_model = new ChartStyleModel(series.CustomPalette.Length);
                        m_model.Palette = series.Palette;
                        m_model.CustomPalette = series.CustomPalette;
                    }
                    else
                    {
                        m_model = new ChartStyleModel();
                        m_model.Palette = series.Palette;
                    }
                    
                    if (series.StrokePalette == ChartColorPalette.Custom && series.CustomStrokePalette != null)
                    {
                        m_strokeModel = new ChartStyleModel(series.CustomStrokePalette.Length);
                        m_strokeModel.Palette = series.StrokePalette;
                        m_strokeModel.CustomPalette = series.CustomStrokePalette;
                    }
                    else
                    {
                        m_strokeModel = new ChartStyleModel();
                        m_strokeModel.Palette = series.StrokePalette;
                    }

                    if (diffuseMaterial != null && series.Area.View3DMode)
                        m_model.SetBinding(diffuseMaterial, DiffuseMaterial.BrushProperty, colorIndex);
                    else
                    {
                        m_model.SetBinding(candleSegment, ChartCandleSegment.FillBrushProperty, colorIndex);
                        m_strokeModel.SetBinding(candleSegment, ChartCandleSegment.StrokeProperty, colorIndex);
                    }
                }
                else
                {
                    Brush lowValue = ChartCandleType.GetBearFillColor(candleSegment);
                    Brush highvalue = ChartCandleType.GetBullFillColor(candleSegment);
                    Brush seriesInterior = series.Interior;
                    Brush interior = (candleSegment.m_isBull ? ((highvalue != null) ? highvalue : seriesInterior) : ((lowValue != null) ? lowValue : seriesInterior));
                    candleSegment.FillBrush = interior;
                    candleSegment.Stroke = series.Stroke;
                }
            }
            else
            {
                if ((bool)series.ColorEach)
                {
                    ChartStyleModel m_model, m_strokeModel;
                    if (series.Palette == ChartColorPalette.Custom && series.CustomPalette != null)
                    {
                        m_model = new ChartStyleModel(series.CustomPalette.Length);
                        m_model.Palette = series.Palette;
                        m_model.CustomPalette = series.CustomPalette;
                    }
                    else
                    {
                        m_model = new ChartStyleModel();
                        m_model.Palette = series.Palette;
                    }
                    if (series.StrokePalette == ChartColorPalette.Custom && series.CustomStrokePalette != null)
                    {
                        m_strokeModel = new ChartStyleModel(series.CustomStrokePalette.Length);
                        m_strokeModel.Palette = series.StrokePalette;
                        m_strokeModel.CustomPalette = series.CustomStrokePalette;
                    }
                    else
                    {
                        m_strokeModel = new ChartStyleModel();
                        m_strokeModel.Palette = series.StrokePalette;
                    }

                    if (diffuseMaterial != null && series.Area.View3DMode)
                        m_model.SetBinding(diffuseMaterial, DiffuseMaterial.BrushProperty, colorIndex);
                    else
                    {
                        m_model.SetBinding(segment, ChartSegment.InteriorProperty, colorIndex);
                        m_strokeModel.SetBinding(segment, ChartSegment.StrokeProperty, colorIndex);
                    }
                }
                else
                {
                    BindingUtils.SetBinding(segment, series, ChartSegment.InteriorProperty, ChartSeries.InteriorProperty);
                    BindingUtils.SetBinding(segment, series, ChartSegment.StrokeProperty, ChartSeries.StrokeProperty);
                }
            }
        }

        internal ImageBrush GetColorEachImageBrush()
        {
            ChartStyleModel m_model;
            if (this.Palette == ChartColorPalette.Custom && this.CustomPalette != null)
            {
                m_model = new ChartStyleModel(this.CustomPalette.Length);
                m_model.Palette = this.Palette;
                m_model.CustomPalette = this.CustomPalette;
            }
            else
            {
                m_model = new ChartStyleModel();
                m_model.Palette = this.Palette;
            }
            return (new ImageBrush() { ImageSource = m_model.GetIcon(16, 16, m_model.Palette).Source });
        }
						
        /// <summary>
        /// Calls OnIsZoomableChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsZoomableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries instance = (ChartSeries)d;
            instance.OnIsZoomableChanged(e);
        }

        /// <summary>
        /// Forces chart series update it's Interior property.
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="e">The DependencyPropertyChangedEvent Arguments e</param>
        private static void OnInactiveSeriesOpacityOnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries chartSeries = (ChartSeries)d;
            if (chartSeries != null)
            {
                chartSeries.CoerceValue(ChartSeries.InteriorProperty);
            }
        }

 		private static void OnUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = (ChartSeries)d;
            series.UnitVisibility = Visibility.Visible;            
        }

        /// <summary>
        /// Updates property value cache and raises IsZoomableChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnIsZoomableChanged(DependencyPropertyChangedEventArgs e)
        {
            SyncChartAreas syncChartArea = this.Area.ChartAreaParent;
            if (syncChartArea != null)
            {
                if (syncChartArea.IsSyncChartArea == true)
                {
                    if (IsZoomableChanged != null)
                    {
                        IsZoomableChanged(this, e);
                    }
                    CoerceValue(ChartSeries.InteriorProperty);
                    if (this.IsZoomable)
                    {
                        this.IsZoomable = true;
                        foreach (ChartArea chartArea in syncChartArea.Areas)
                        {
                            chartArea.ZoomAllAxes = false;
                        }
                        foreach (ChartArea chartArea in syncChartArea.Areas)
                        {
                            foreach (ChartSeries chartSeries in chartArea.Series)
                            {
                                if (chartSeries != this)
                                {
                                    chartSeries.IsZoomable = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (ChartArea chartArea in syncChartArea.Areas)
                        {
                            if (chartArea != null)
                                chartArea.ZoomAllAxes = true;
                        }
                    }
                }
            }
            else
            {
                if (IsZoomableChanged != null)
                {
                    IsZoomableChanged(this, e);
                }
                ////Updating opacity.
                CoerceValue(ChartSeries.InteriorProperty);
                if (this.IsZoomable)
                {
                    ////Now zooming 1 series only.
                    this.Area.ZoomAllAxes = false;
                    ////Setting IsZooming property to false because cannot zoom more than 1 series.
                    foreach (ChartSeries chartSeries in this.Area.Series)
                    {
                        ////Except our series set IsZooming to false.
                        if (chartSeries != this)
                        {
                            chartSeries.IsZoomable = false;
                        }
                    }
                }
                else
                {
                    ////If value was changed to false.
                    ////Go thru all series and if none of them has IsZoomable set to TRUE
                    foreach (ChartSeries chartSeries in this.Area.Series)
                    {
                        if (chartSeries.IsZoomable)
                        {
                            return;
                        }
                    }
                    ////Setting to zoom whole chart even if it wasnt intend to.
                    this.Area.ZoomAllAxes = true;
                }
            }
        }

        /// <summary>
        /// Recalculates chart series.
        /// </summary>
        internal void Recalculate()
        {
            var data = Data;
            var visibleSeries = Area != null ? Area.VisibleSeries : null;
            if (data != null)
            {
                bool indexedCompatible = true;
                if (visibleSeries != null)
                {
                    if (visibleSeries == null)
                        return;
                    ////In performance considerations checking data length equality for all visible series. 
                    ////If check passes - do point-by-point check.
                    if (this.Type != ChartTypes.FastLine)// || visibleSeries.Count()>1)
                    {
                        foreach (ChartSeries series in visibleSeries)
                        {
                            //Commented the checking data count are equal for index compatability - fixing SD8250, SD8072
                            //It is because it is possible to have series differing in number of points added up in same series, still representing indexed data.
                            if (series.Data == null)// || data.Count != series.Data.Count)
                            {
                                indexedCompatible = false;
                                break;
                            }
                        }

                        ////Iterating over visible series and checking whether all of them can be shown as indexed.
                        for (int i = 0; i < data.Count; i++)
                        {
                            if (indexedCompatible)
                            {
                                if (visibleSeries != null)
                                {
                                    foreach (ChartSeries series in visibleSeries)
                                    {
                                        ////We spot ourself in visible series collection. 
                                        if (series == this)
                                        {
                                            continue;
                                        }
                                        // check has been included (fixing SD8250, SD8072), because series can have lesser count that data's count - then index out of range exception will be thrown, if we miss this check.
                                        if (series.Data.Count > 0 && i < series.Data.Count)
                                        {

                                            ////Making sure X values of all series are equal. Hence conclude the data is index compatible
                                            if (data[i] != null && series.Data[i] != null && (data[i].X != series.Data[i].X))
                                            {
                                                indexedCompatible = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Area.IsIndexedCompatible = indexedCompatible && (Area.PrimarySeries != null) && Area.PrimarySeries.IsIndexed;
                }
            }

            if (this.Segments != null && this.Segments.Count > 0)
            {
                this.UpdateSegments();
            }
            else
            {
                this.RecalculateSegments();
            }

            this.RecalculateIndexRange();
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <seealso cref="ChartSeries"/>
        private void UpdateSegments()
        {
            if (m_needUpdateSegments)
            {
                m_xCidsRange = DoubleRange.Empty;
                m_yCidsRange = DoubleRange.Empty;
                m_zCidsRange = DoubleRange.Empty;

                if (this.ChartType != null && this.EnsureDataIsEnough())
                {
                    this.ChartType.Update(this);

                    if (this.ChartType.RequiresAxis && this.Segments != null)
                    {
                        foreach (ChartSegment cid in this.Segments)
                        {
                            m_xCidsRange += cid.XDataMeasure;
                            m_yCidsRange += cid.YDataMeasure;
                            m_zCidsRange += cid.ZDataMeasure;
                        }
                    }
                }
                ////changed to true
                m_needUpdateSegments = false;
            }
        }

        /// <summary>
        /// Recalculates the segments.
        /// </summary>
        private void RecalculateSegments()
        {
            if (m_needUpdateSegments)
            {
                m_xCidsRange = DoubleRange.Empty;
                m_yCidsRange = DoubleRange.Empty;
                m_zCidsRange = DoubleRange.Empty;
                
                if (this.Segments != null)
                    this.Segments.Clear();
                if (this.Adornments != null)
                {
                    Adornments.Clear();
                }

                if (this.ChartType != null && this.EnsureDataIsEnough())
                {
                    this.ChartType.Calculate(this);

                    if (this.ChartType.RequiresAxis && this.Segments != null)
                    {
                        foreach (ChartSegment cid in this.Segments)
                        {
                            m_xCidsRange += cid.XDataMeasure;
                            m_yCidsRange += cid.YDataMeasure;
                            m_zCidsRange += cid.ZDataMeasure;
                        }
                    }
                }

                m_needUpdateSegments = true;
            }
        }

        /// <summary>
        /// Recalculates the visual segments.
        /// </summary>
        private void RecalculateVisualSegments()
        {
            if (m_needUpdateVisualSegments)
            {
                m_needUpdateVisualSegments = false;
            }
        }

        /// <summary>
        /// Recalculates the index range.
        /// </summary>
        private void RecalculateIndexRange()
        {
            if (m_needUpdateIndexRange)
            {
                int end = this.Segments == null ? 0 : this.Segments.Count - 1;
                ////changed to true
                m_needUpdateIndexRange = false;
            }
        }

        /// <summary>
        /// Ensures the data is correct.
        /// </summary>
        /// <returns>bool value to ensure if data is enough</returns>
        private bool EnsureDataIsEnough()
        {
            //if (this.PointsCount == 0)
            //{
            //    return false;
            //}
            if (this.DataSource != null)
            {
                int requiredDataCount = this.ChartType.RequiresDataCount;
                int yValueInitialized = 0;
                if (this.DataModel != null && this.DataModel.PathsY != null)
                {
                    yValueInitialized = this.DataModel.PathsY.Count<string>();

                    return requiredDataCount <= yValueInitialized;
                }
                else if (this.DataModel != null && this.DataModel.ChartPoints.Count > 0)
                {
                    return true;

                }
                return false;
            }
            else
            {
                int requiredDataCount = this.ChartType.RequiresDataCount;

                for (int i = 0; i < this.PointsCount; i++)
                {
                    IChartDataPoint cdpt = Data[i];
                    if (cdpt != null)
                    {
                        if (cdpt.EmptyPoint == true)
                        {
                            continue;
                        }
                        if (cdpt.Values == null || cdpt.Values.Length < requiredDataCount)
                        {
                            Console.WriteLine("The series {0} doesn't have enough of data", this.Label);
                            return false;
                        }
                    }
                }
            }

            return true;
            //for (int i = 0; i < this.PointsCount; i++)
            //{
            //    IChartDataPoint cdpt = Data[i];
            //    if (cdpt != null)
            //    {
            //        if (cdpt.EmptyPoint == true )
            //        {
            //            continue;
            //        }
            //        if (cdpt.Values == null || cdpt.Values.Length < requiredDataCount)
            //        {
            //            Console.WriteLine("The series {0} doesn't have enough of data", this.Label);
            //            return false;
            //        }
            //    }
            //}

            //return true;
        }

        /// <summary>
        /// Handles data changed event.
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The NotifyCollectionChangedEventArgs e</param>
        private void OnDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Move)
            {
                this.internaldata_modified = true;
            }
            if (!this.HoldDataUpdate)
            {
                ////this.Invalidate();
                this.RaiseDataChanged(this, e);
                this.RaiseAppearanceChanged(this, null);
            }
        }

        /// <summary>
        /// Called when adornments property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnAdornmentsPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyMetadata propertyMetadata = e.Property.GetMetadata(sender.GetType()) as PropertyMetadata;

            if (propertyMetadata != null)
            {
                if ((sender is ChartAdornmentInfo) && !(sender as ChartAdornmentInfo).Visible)
                {
                    this.Adornments.Clear();
                }
                bool affectUpdate = (e.Property.ToString() == "SegmentIsOut" || e.Property.ToString() == "Visible" || e.Property.ToString() == "VerticalAlignment" || e.Property.ToString() == "HorizontalAlignment" || e.Property.ToString() == "SegmentShowLine" ||
                    e.Property.ToString() == "AdornmentsPosition" || e.Property.ToString() == "SegmentIsOut" || e.Property.ToString() == "ConnectorTemplate") || ((e.Property.ToString() == "SegmentLabelRotation" || e.Property.ToString() == "SegmentHorizontalAlignment" || e.Property.ToString() == "SegmentLabelFormat" || e.Property.ToString() == "SegmentVerticalAlignment" || e.Property.ToString() == "IsAdornmentAllignment" || e.Property.ToString() == "IsLabelRotate" || e.Property.ToString() == "OffsetX" || e.Property.ToString() == "OffsetY" || e.Property.ToString() == "SymbolHeight" || e.Property.ToString() == "SymbolWidth" || e.Property.ToString() == "SymbolInterior" || e.Property.ToString() == "SymbolStroke" || e.Property.ToString() == "SymbolStrokeThickness" || e.Property.ToString() == "Symbol") && this.Area != null && this.Area.SyncChartArea == null) ? true : false;

                if (propertyMetadata != null)
                {
                    if (affectUpdate == true)
                    {
                        this.Invalidate();
                        this.RaiseAppearanceChanged(this, EventArgs.Empty);
                    }
                }

                //switch (propertyMetadata.Options)
                //{
                //    case ChartPropertyMetadataOptions.AffectsUpdate:
                //        this.Invalidate();
                //        this.RaiseAppearanceChanged(this, EventArgs.Empty);
                //        break;

                //    case ChartPropertyMetadataOptions.AffectsRedraw:
                //        if (Area != null)
                //        {
                //            Area.Redraw();
                //        }

                //        break;
                //}
            }
        }

        /// <summary>
        /// Raises data changed event
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The Event arguments</param>
        private void RaiseDataChanged(object sender, EventArgs args)
        {
            if (DataChanged != null && !m_isUpdating)
            {
                DataChanged(this, args);
            }
        }

        /// <summary>
        /// Raises appearence changed event
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="args">The Event arguments</param>
        private void RaiseAppearanceChanged(object sender, EventArgs args)
        {
            if (AppearanceChanged != null && !m_isUpdating)
            {
                AppearanceChanged(this, args);
            }
        }

        /// <summary>
        /// Datas the source changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void DataSourceChanged(object oldValue, object newValue)
        {
            ////Retrieving old and new collections.
            CollectionView oldCollection = oldValue as CollectionView;
            CollectionView newCollection = newValue as CollectionView;

            if (oldCollection != null)
            {
                ////Unsubscribing from Item changed events.
                oldCollection.CurrentChanged -= new EventHandler(CurrentItemChanged);
                oldCollection.CurrentChanging -= new CurrentChangingEventHandler(CurrentItemChanging);
            }

            if (newCollection != null)
            {
                ////If DataSource is set to CollectionView - subscribing to Current item changing events in order 
                ////to keep this.SelectedItem property in sync with CurrentItem property of CollectionView.
                newCollection.CurrentChanged += new EventHandler(CurrentItemChanged);
                newCollection.CurrentChanging += new CurrentChangingEventHandler(CurrentItemChanging);
            }
        }

        /// <summary>
        /// Currents the item changing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CurrentChangingEventArgs"/> instance containing the event data.</param>
        private void CurrentItemChanging(object sender, CurrentChangingEventArgs e)
        {
            CollectionView collection = sender as CollectionView;
            ////Making sure we've got right values.
            if (collection != null && SelectedItem != null && EnableSelection)
            {
                ////Setting IsSelected to false for corresponding segment in CollectionView.
                //SD16050 - IsSelected property is not working properly.
                int selectedIndex = collection.IndexOf((SelectedItem as ChartSegment).CorrespondingPoints[0].DataPoint.Item);
                if (selectedIndex >= 0 && selectedIndex < collection.Count && Segments.Count > selectedIndex)
                {
                    this.Segments[selectedIndex].SetValue(ChartSegment.IsSelectedPropertyKey, false);
                }
            }
        }

        /// <summary>
        /// Currents the item changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CurrentItemChanged(object sender, EventArgs e)
        {
            CollectionView collection = sender as CollectionView;
            ////Making sure we've got the right values and position on collection. 
            if (collection != null && collection.CurrentPosition >= 0 && EnableSelection)
            {
                ////Setting our SelectedItem to Collection's Current item in order to keep them syncronized.
                //SD16050 - IsSelected property is not working properly.
                //SelectedItem = collection.CurrentItem;
                ////Setting IsSelected to true on corresponding segment.
                if (collection.CurrentPosition >= 0 && collection.CurrentPosition < Segments.Count)
                {
                    Segments[collection.CurrentPosition].SetValue(ChartSegment.IsSelectedPropertyKey, true);
                }
            }
        }

        private static void OnEmptyPointValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries type = (ChartSeries)d;
            type.m_needUpdateSegments = true;
            //type.BeginUpdate();
            type.UpdateSegments();
            //type.EndUpdate();
        }

        private static void OnLegendIconTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
             ChartSeries type = (ChartSeries)d;
            if(args.NewValue!= null)
                type.InternalLegendIconTemplate = (DataTemplate)args.NewValue;
        }
        private static void OnEmptyPointSymbolTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries type = (ChartSeries)d;
            type.m_needUpdateSegments = true;
            //type.BeginUpdate();
            type.UpdateSegments();
            //type.EndUpdate();
        }

        private static void OnLegendIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries type = (ChartSeries)d;
            UpdateLegendIcon(type);
        }
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries series = d as ChartSeries;
            if (series != null)
            {
                if (series.AdornmentsInfo != null)
                {
                    series.AdornmentsInfo.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    series.AdornmentsInfo.HorizontalAlignment = HorizontalAlignment.Center;
                }
                series.Invalidate();
            }
        }
        private static void OnVisibilityOnLegend(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries type = (ChartSeries)d;
            if (type.Area != null)
            {
                DependencyObject a = new DependencyObject();
                Chart chart = null;
                a = VisualTreeHelper.GetParent(type.Area);
                while (a != null)
                {
                    if (VisualTreeHelper.GetParent(a) is Chart == true)
                    {
                        chart = VisualTreeHelper.GetParent(a) as Chart;
                    }
                    a = VisualTreeHelper.GetParent(a);
                }
                if (chart != null)
                    chart.SetLegendItemSource(chart.Legends);
                if (type.Type != ChartTypes.Doughnut && type.Type != ChartTypes.Pie && type.Type != ChartTypes.Funnel && type.Type != ChartTypes.Pyramid)
                    type.Area.SetLegendItemSource(type);
            }
        }

        private static void OnIsVisibleOnLegend(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries type = (ChartSeries)d;

            if (type.Area != null)
            {
                DependencyObject a = new DependencyObject();
                Chart chart = null;
                a = VisualTreeHelper.GetParent(type.Area);
                while (a != null)
                {
                    if (VisualTreeHelper.GetParent(a) is Chart == true)
                    {
                        chart = VisualTreeHelper.GetParent(a) as Chart;
                    }
                    a = VisualTreeHelper.GetParent(a);
                }

                if (chart != null)
                    chart.SetLegendItemSource(chart.Legends);
                type.Area.SetLegendItemSource(type);
            }
        }

        /// <summary>
        /// Updates the legend.
        /// </summary>
        /// <param name="type">The series type</param>        
        /// <seealso cref="ChartSeries"/>
        private static void UpdateLegendIcon(ChartSeries type)
        {
            if (type != null)
            {
                DataTemplate LegendTemplate;
                ResourceDictionary rd = ChartDictionaries.GenericLegendDictionary;
                //ResourceDictionary rd = new SharedResourceDictionary()
                //{
                //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.Legend.xaml", UriKind.RelativeOrAbsolute)
                //};
                if (rd != null)
                {
                    if (type.LegendIcon != ChartLegendIcon.None)
                    {
                        if (type.LegendIcon != ChartLegendIcon.SeriesType)
                        {

                            LegendTemplate = rd[type.LegendIcon.ToString()] as DataTemplate;
                            if (LegendTemplate != null)
                                type.InternalLegendIconTemplate = LegendTemplate;
                        }
                        else
                        {
                            LegendTemplate = rd[type.seriesType] as DataTemplate;
                            if (LegendTemplate != null)
                                type.InternalLegendIconTemplate = LegendTemplate;
                        }
                    }
                    else
                    {
                        if (type.LegendIconTemplate == null)
                        {
                            LegendTemplate = rd[type.seriesType] as DataTemplate;
                            if (LegendTemplate != null)
                                type.InternalLegendIconTemplate = LegendTemplate;
                        }
                        else
                            type.InternalLegendIconTemplate = type.LegendIconTemplate;
                    }
                }
            }
        }

        #endregion

        #region IChartSerializer Members

        /// <summary>
        /// Method declaration for Serialize
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            //string _xamlString;
            EditorHelper.Register<BindingExpression, BindingConvertor>();
           
            StringBuilder outstr = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(outstr, settings));
            //this string need for turning on expression saving mode 
            dsm.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(this, dsm);
            //_xamlString = outstr.ToString();
            if (this.DataSource != null)
            {
                if (this.DataModel.ChartPoints[0].Tag.GetType() == typeof(XmlElement))
                {
                    outstr = outstr.Remove(outstr.ToString().IndexOf("DataSource=\"{av:Binding}\""), new StringBuilder("DataSource=\"{av:Binding}\"").Length);
                    throw new Exception("Essential Chart doesnot serialize xml Data");
                }
                outstr = outstr.Replace(Chart.SubString(outstr.ToString(), "Data=\"", "\""), string.Empty);
            }
            if (outstr.ToString().Contains("XAxis=\"{av:Binding Path=(ChartArea.PrimaryAxis)}\""))
            {
                outstr = outstr.Remove(outstr.ToString().IndexOf("XAxis=\"{av:Binding Path=(ChartArea.PrimaryAxis)}\""), new StringBuilder("XAxis=\"{av:Binding Path=(ChartArea.PrimaryAxis)}\"").Length);
            }
            //else if (this.Area != null && outstr.ToString().Contains(this.Area.PrimaryAxis.Name))
            //{
            //    outstr = outstr.Replace(this.Area.PrimaryAxis.Name, this.Area.PrimaryAxis.Name + this.Area.Series.IndexOf(this).ToString());
            //}
            if (outstr.ToString().Contains("YAxis=\"{av:Binding Path=(ChartArea.SecondaryAxis)}\""))
            {
                outstr = outstr.Remove(outstr.ToString().IndexOf("YAxis=\"{av:Binding Path=(ChartArea.SecondaryAxis)}\""), new StringBuilder("YAxis=\"{av:Binding Path=(ChartArea.SecondaryAxis)}\"").Length);
            }
            //else if (outstr.ToString().Contains(this.Area.SecondaryAxis.Name))
            //{
            //    outstr = outstr.Replace(this.Area.SecondaryAxis.Name, this.Area.SecondaryAxis.Name + this.Area.Series.IndexOf(this).ToString());
            //}
            if (outstr.ToString().Contains("ZAxis=\"{av:Binding Path=(ChartArea.DepthAxis)}\""))
            {
                outstr = outstr.Remove(outstr.ToString().IndexOf("ZAxis=\"{av:Binding Path=(ChartArea.DepthAxis)}\""), new StringBuilder("ZAxis=\"{av:Binding Path=(ChartArea.DepthAxis)}\"").Length);
            }
            //else if (outstr.ToString().Contains(this.Area.DepthAxis.Name))
            //{
            //    outstr = outstr.Replace(this.Area.DepthAxis.Name, this.Area.DepthAxis.Name + this.Area.Series.IndexOf(this).ToString());
            //}
            //if(this.LegendIcon != ChartLegendIcon.None)
            //    outstr = outstr.Replace(Chart.SubString(outstr.ToString(), "<ChartSeries.LegendIconTemplate>", "</ChartSeries.LegendIconTemplate>"), string.Empty);
           
            
            if (ChartFastSeriesPresenter.GetPen(this) != null)
            {
                Pen pen = ChartFastSeriesPresenter.GetPen(this);
                StringBuilder outstr1 = new StringBuilder();
               
                XamlDesignerSerializationManager dsm1 = new XamlDesignerSerializationManager(XmlWriter.Create(outstr1, settings));
                //this string need for turning on expression saving mode 
                dsm1.XamlWriterMode = XamlWriterMode.Expression;
                XamlWriter.Save(pen, dsm1);
                StringBuilder _string = new StringBuilder("<ChartFastSeriesPresenter.Pen>");
                _string.Append(outstr1);
                _string.Append("</ChartFastSeriesPresenter.Pen></ChartSeries>");
                outstr = outstr.Replace(Chart.SubString(outstr.ToString(), "</ChartSeries>", ""), _string.ToString());

            } 
            return outstr.ToString();
        }

        /// <summary>
        /// Method declaration for DeSerialize
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }



}