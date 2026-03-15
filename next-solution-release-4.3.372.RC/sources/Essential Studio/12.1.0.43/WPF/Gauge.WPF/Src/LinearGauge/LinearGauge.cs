// <copyright file="LinearGauge.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the LinearGauge UI element.
    /// </summary>
    /// <remarks>The Linear Gauge is perfect for showing input values graphically along a linear scale.
    /// A linear gauge can have multiple scales, pointers, ranges, state indicators, labels and images.
    /// It has a bounded rectangle around it. The top-left corner of the rectangle has coordinates of (0, 0) 
    /// and the bottom-right corner has coordinates of (100, 100) and every gauge element is drawn
    /// inside the bounded rectangle according to the <see cref="LocalizableGaugeElement.Location"/> property. 
    /// </remarks>
    /// <seealso cref="CircularGauge"/>
    /// <seealso cref="DigitalGauge"/>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example>
    /// <code>
    /// <Window xmlns:local="clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.WPF">
    /// <local:LinearGauge Name="LinearGauge1"/>
    /// </Window>
    /// </code>
    /// </example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public partial class LinearGauge : Control</code></example>
    /// </list>
    /// <para/>
    /// </example>
    /// </list>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="LinearGauge"/> in XAML.
    /// <code>
    ///    <Window x:Class="GaugeControlTesting.Window1"
    ///      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///      xmlns:sync="clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.WPF"
    ///      xmlns:sfshared="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.WPF" 
    ///      Title="Linear Gauge" Height="366" Width="118">    
    ///  <Grid>    
    ///    <sync:LinearGauge CenterFrameFillColor="Brown" Name="linearGauge1">
    ///    <sync:LinearGauge.Scales>        
    ///        <sync:LinearScale Name="LinearScale"  
    ///                          Minimum="0" 
    ///                          Maximum="100"
    ///                          MinorIntervalValue="2" 
    ///                          MajorIntervalValue="10"
    ///                          ScaleBarSize = "10"
    ///                          ScaleBarLength="260">
    ///            <sync:LinearScale.Ticks>            
    ///            <sync:LinearLabelTick FontSize="10" TickStyle="MajorTick" BackgroundBrush="Red" 
    ///                                  TickPlacement="Inside" DistanceFromScale="5"/>            
    ///            <sync:LinearMarkTick TickHeight="9" TickShape="Rectangle" TickStyle="MajorTick"
    ///                                 TickWidth="4" BackgroundBrush="Pink"/>            
    ///            <sync:LinearMarkTick TickHeight="4" TickWidth="1" TickStyle="MinorTick" 
    ///                                 BackgroundBrush="Aqua"/>            
    ///        </sync:LinearScale.Ticks>
    ///        <sync:LinearScale.Pointers>            
    ///            <sync:LinearBarPointer BackgroundBrush="Blue"
    ///                                   BorderBrush="Red" 
    ///                                   PointerWidth="5"
    ///                                   Value="50"/>            
    ///        </sync:LinearScale.Pointers>
    ///        <sync:LinearScale.Ranges>            
    ///            <sync:LinearRange StartValue="70" EndValue="100" 
    ///                              StartWidth="0" EndWidth="10" 
    ///                              RangePosition="Inside" DistanceFromScale="2"
    ///                              BackgroundBrush="OrangeRed"/>            
    ///        </sync:LinearScale.Ranges>
    ///      </sync:LinearScale>        
    ///    </sync:LinearGauge.Scales>    
    ///  </sync:LinearGauge>    
    ///  </Grid>
    /// </Window>
    /// </code>
    /// <para/>The following example shows how to create a <see cref="LinearGauge"/> in C#.
    /// <code>
    ///  using System;
    ///  using System.Windows;
    ///  using System.Windows.Controls;
    ///  using Syncfusion.Windows.Gauge;
    ///  using System.Windows.Media;<para/>
    ///  namespace GaugeControlTesting
    ///  {
    ///    public partial class Window1 : Window
    ///    {
    ///        public Window1()
    ///        {
    ///            InitializeComponent();<para/>
    ///            this.linearGauge1.CenterFrameFillColor = Colors.Brown;
    ///            LinearScale scale = new LinearScale();
    ///            scale.Minimum = 0;
    ///            scale.Maximum = 100;
    ///            scale.MinorIntervalValue = 2;
    ///            scale.MajorIntervalValue = 10;
    ///            scale.ScaleBarSize = 10;
    ///            scale.ScaleBarLength = 260;
    ///            linearGauge1.Scales.Add( scale );<para/>
    ///            LinearLabelTick labelTick = new LinearLabelTick();
    ///            labelTick.FontSize = 10;
    ///            labelTick.TickStyle = TickStyle.MajorTick;
    ///            labelTick.BackgroundBrush = new SolidColorBrush( Colors.Red );
    ///            labelTick.TickPlacement = ScalePlacement.Inside;
    ///            labelTick.DistanceFromScale = 5;
    ///            scale.Ticks.Add( labelTick );
    ///            LinearMarkTick markTick1 = new LinearMarkTick();
    ///            markTick1.TickHeight = 9;
    ///            markTick1.TickShape = TickShape.Rectangle;
    ///            markTick1.TickStyle = TickStyle.MajorTick;
    ///            markTick1.TickWidth = 4;
    ///            markTick1.BackgroundBrush = new SolidColorBrush( Colors.Pink );
    ///            scale.Ticks.Add( markTick1 );
    ///            LinearMarkTick markTick2 = new LinearMarkTick();
    ///            markTick2.TickHeight = 4;
    ///            markTick2.TickWidth = 1;
    ///            markTick2.TickStyle = TickStyle.MinorTick;
    ///            markTick2.BackgroundBrush = new SolidColorBrush( Colors.White );
    ///            scale.Ticks.Add( markTick2 );<para/>
    ///            LinearBarPointer pointer = new LinearBarPointer();
    ///            pointer.BackgroundBrush = new SolidColorBrush( Colors.Blue );
    ///            pointer.BorderBrush = new SolidColorBrush( Colors.Red );
    ///            pointer.PointerWidth = 5;
    ///            pointer.Value = 50;
    ///            scale.Pointers.Add( pointer );
    ///            LinearRange range = new LinearRange();<para/>
    ///            range.StartValue = 70;
    ///            range.EndValue = 100;
    ///            range.StartWidth = 0;
    ///            range.EndWidth = 10;
    ///            range.RangePosition = ScalePlacement.Inside;
    ///            range.DistanceFromScale = 2;
    ///            range.BackgroundBrush = new SolidColorBrush( Colors.OrangeRed );
    ///        }
    ///    }
    ///  }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [DesignTimeVisible(true)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2003,
  Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/DefaultStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(LinearGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/MetroStyle.xaml")]
    public class LinearGauge : GaugeBase
    {
        #region Private Members
        /// <summary>
        /// Collection of linear scales.
        /// </summary>
        private LinearScaleCollection m_scales;

        private bool isHeightNan , isWidthNan;

        private Path mainglasspath;
        #endregion Private Members

        #region CLR Getters & Setters
        /// <summary>
        /// Gets or sets the collection of linear scales.
        /// </summary>
        /// <value>
        /// Type: <see cref="ScaleCollection"/>
        /// </value>     
        public LinearScaleCollection Scales
        {
            get
            {
                return m_scales;
            }

            set
            {
                m_scales = value;
            }
        }
        #endregion CLR Getters & Setters

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="FrameType"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="Orientation"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback OrientationChanged;
        /// <summary>
        /// Event that is raised when <see cref="AutoSize"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AutoSizeChanged;

        /// <summary>
        /// Event that is raised when <see cref="RotationAngle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback RotationAngleChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="FrameType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameTypeProperty =
            DependencyProperty.Register("FrameType", typeof(LinearGaugeFrameType), typeof(LinearGauge), new FrameworkPropertyMetadata(LinearGaugeFrameType.Rectangle, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnFrameTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(GaugeOrientation), typeof(LinearGauge), new FrameworkPropertyMetadata(GaugeOrientation.Vertical, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoSizeProperty =
            DependencyProperty.Register("AutoSize", typeof(bool), typeof(LinearGauge), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnAutoSizeChanged)));

        /// <summary>
        /// Identifies the <see cref="RotationAngle"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(LinearGauge), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnRotationAngleChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the Linear Gauge frame style.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="GaugeFrameType"/>
        /// </value>
        [Bindable(true)]
        [Category("Behaviors")]
        public LinearGaugeFrameType FrameType
        {
            get
            {
                return (LinearGaugeFrameType)GetValue(FrameTypeProperty);
            }

            set
            {
                SetValue(FrameTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the linear gauge.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="GaugeOrientation"/>
        /// Default value is GaugeOrientation.Horizontal.
        /// </value>
        public GaugeOrientation Orientation
        {
            get
            {
                return (GaugeOrientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the linear gauge.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="GaugeOrientation"/>
        /// Default value is GaugeOrientation.Horizontal.
        /// </value>
        public bool AutoSize
        {
            get
            {
                return (bool)GetValue(AutoSizeProperty);
            }

            set
            {
                SetValue(AutoSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the angle for displaying the gauge.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        internal double RotationAngle
        {
            get
            {
                return (double)GetValue(RotationAngleProperty);
            }

            set
            {
                SetValue(RotationAngleProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="LinearGauge"/> class.
        /// Overrides some dependency properties.
        /// </summary>
        static LinearGauge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinearGauge), new FrameworkPropertyMetadata(typeof(LinearGauge)));
            Control.BackgroundProperty.OverrideMetadata(typeof(LinearGauge), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBackgroundChanged)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGauge"/> class.
        /// </summary>
        public LinearGauge()
        {
            this.Scales = new LinearScaleCollection();
            this.Scales.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
        }
        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> property 
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Loaded += new RoutedEventHandler(LinearGaugeLoaded);
            for (int i = 0; i < this.StateIndicators.Count; i++)
            {
                StateIndicator stateIndicator = this.StateIndicators[i];
                CalculateScope(stateIndicator);
            }

            // this.SetScope();
        }

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire collection is refreshed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        protected override void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.CollectionChanged(sender, e);

            // int count = e.NewItems.Count;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    if (e.NewItems[i] is LinearScale)
                    {
                        LinearScale elem = e.NewItems[i] as LinearScale;
                        if (elem != null)
                        {
                            elem.ScaleBarLengthChanged += new PropertyChangedCallback(ScaleBarLengthChanged);
                            elem.SetScope(this);
                        }
                    }
                    else if (e.NewItems[i] is StateIndicator)
                    {
                        StateIndicator elem = e.NewItems[i] as StateIndicator;
                        elem.SetScope(this);
                    }
                    else if (e.NewItems[i] is GaugeImage)
                    {
                        GaugeImage elem = e.NewItems[i] as GaugeImage;
                        elem.SetScope(this);
                    }
                    else if (e.NewItems[i] is GaugeCustomLabel)
                    {
                        GaugeCustomLabel elem = e.NewItems[i] as GaugeCustomLabel;
                        elem.SetScope(this);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the templates are being applied.
        /// </summary>
        /// <remarks>All the borders,Location and Orientation were set. If the PART_ContainerBorder is
        /// not set, then the Adorned Element(the element underneath the adorner layer)<para/>
        /// is LinearGauge. If the PART_ContainerBorder is set, then the Adorned Element is<para/>
        /// the PART_ContainerBorder</remarks>
        public override void OnApplyTemplate()
        {            
            base.OnApplyTemplate();
            this.mainglasspath = this.GetTemplateChild("GlassPath") as Path;
            double height = 0;
            double maxScaleHeight = 0;
            double maxScaleWidth = 0;
            double width = 0;
            int count = this.Scales.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.Scales[i] is LinearScale)
                {
                    LinearScale scale = this.Scales[i] as LinearScale;
                    if (scale != null)
                    {
                        for (int j = 0; j < scale.Ranges.Count; j++)
                        {
                            LinearRange range = scale.Ranges[j] as LinearRange;
                            height = Math.Max(height, range.EndWidth);
                        }
                        for (int k = 0; k < scale.Ticks.Count; k++)
                        {
                            if (scale.Ticks[k] is LinearMarkTick)
                            {
                                LinearMarkTick tick = scale.Ticks[k] as LinearMarkTick;
                                height = Math.Max(height, tick.TickHeight);
                                width = Math.Max(width, tick.TickWidth);
                            }                            
                        }
                        foreach (LinearPointer pointer in scale.Pointers)
                        {
                            height = Math.Max(height, pointer.PointerWidth);                            
                        }                        
                    }
                    if (scale.ScaleBarLength == 0)
                        scale.ScaleBarLength = 220;
                    if (scale.ScaleBarSize == 0)
                        scale.ScaleBarSize = height;
                    maxScaleWidth = Math.Max(scale.ScaleBarLength, maxScaleWidth);
                    maxScaleHeight = Math.Max(scale.ScaleBarSize, maxScaleHeight);

                }
            }  
            if (Double.IsNaN(this.Height))
                isHeightNan = true;
            if (Double.IsNaN(this.Width))
                isWidthNan = true;
            if (this.Orientation == GaugeOrientation.Vertical)
            {
                if(isHeightNan)
                    this.Height = maxScaleWidth+100;
                if(isWidthNan)
                    this.Width = maxScaleHeight+100;
            }
            else
            {
                if(isHeightNan)
                    this.Height = maxScaleHeight+100;
                if(isWidthNan)
                    this.Width = maxScaleWidth+100;
            }  
            this.RefreshBorders();
            this.UpdateChildrenLocation();
            this.AdjustOrientationSettings();

            if (this.PART_ContainerBorder != null)
            {
                GaugeAdorner = new CircularGaugeAdorner(this.PART_ContainerBorder);
            }
            else
            {
                GaugeAdorner = new CircularGaugeAdorner(this);
            }

            GaugeAdorner.GaugeParent = this;
            this.RefreshAdornerLayer();

        }

        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.SizeChanged"/> event, 
        /// using the specified information as part of the eventual event data. 
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.RefreshBorders();           
            int count = this.Scales.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.Scales[i] is LinearScale)
                {
                    LinearScale scale = this.Scales[i] as LinearScale;
                    if (scale.LengthRatio != 0)
                    {
                        if (scale.Orientation == GaugeOrientation.Vertical)
                        {
                            scale.ScaleBarLength = this.ActualHeight * scale.LengthRatio;
                        }
                        else
                        {
                            scale.ScaleBarLength = this.ActualWidth * scale.LengthRatio;
                        }
                    }
                    else if (scale.ScaleBarLength < this.ActualHeight)
                    {
                        scale.LengthRatio = scale.ScaleBarLength / this.ActualHeight;
                    }

                    if (scale.SizeRatio != 0)
                    {
                        if (this.Orientation == GaugeOrientation.Vertical)
                        {
                            scale.ScaleBarSize = this.ActualWidth * scale.SizeRatio;
                        }
                        else
                        {
                            scale.ScaleBarSize = this.ActualHeight * scale.SizeRatio;
                        }
                    }
                    else if (scale.ScaleBarSize < this.ActualWidth)
                    {
                        scale.SizeRatio = scale.ScaleBarSize / this.ActualWidth;
                    }
                    
                    scale.ScaleWidth = this.ActualWidth;
                    if (this.Orientation == GaugeOrientation.Vertical)
                    {
                        if (scale.ScaleBarLength > this.ActualHeight)
                        {
                            scale.ScaleBarLength = this.ActualHeight;
                        }
                        if (scale.ScaleBarSize > this.ActualWidth)
                        {
                            scale.ScaleBarSize = this.ActualWidth;
                        }
                    }
                    else
                    {
                        if (scale.ScaleBarLength > this.ActualWidth)
                        {
                            scale.ScaleBarLength = this.ActualWidth;
                        }
                        if (scale.ScaleBarSize > this.ActualHeight)
                        {
                            scale.ScaleBarSize = this.ActualHeight;
                        }

                    }
                    for (int j = 0; j < scale.Ranges.Count; j++)
                    {
                        LinearRange range = scale.Ranges[j] as LinearRange;
                        if (range.endSizeRatio != 0)
                        {
                            range.EndWidth = scale.ScaleBarSize * range.endSizeRatio;
                        }
                        if (range.startSizeRatio != 0)
                        {
                            range.StartWidth = scale.ScaleBarSize * range.startSizeRatio;
                        }
                    }                   
                    foreach (LinearPointer pointer in scale.Pointers)
                    {
                        if (pointer.SizeRatio != 0)
                            pointer.PointerWidth = pointer.SizeRatio * scale.ScaleBarLength;
                        if (pointer is LinearMarkerPointer)
                        {
                            LinearMarkerPointer markerPointer = pointer as LinearMarkerPointer;
                            if(markerPointer.LengthSizeRatio!=0)
                            markerPointer.PointerLength = markerPointer.LengthSizeRatio * scale.ScaleBarSize;
                        }
                    }
                    foreach (StateIndicator stateIndicator in StateIndicators)
                    {
                        if (this.Orientation == GaugeOrientation.Vertical)
                        {
                            if (stateIndicator.startSizeRatio != 0)
                                stateIndicator.IndicatorWidth = stateIndicator.startSizeRatio * this.ActualWidth;
                            if (stateIndicator.endSizeRatio != 0)
                                stateIndicator.IndicatorHeight = stateIndicator.endSizeRatio * this.ActualHeight;
                        }
                        else
                        {
                            if (stateIndicator.startSizeRatio != 0)
                                stateIndicator.IndicatorWidth = stateIndicator.startSizeRatio * this.ActualHeight;
                            if (stateIndicator.endSizeRatio != 0)
                                stateIndicator.IndicatorHeight = stateIndicator.endSizeRatio * this.ActualWidth;
                        }
                    }
                    scale.RefreshPointers();
                }
            }
            if (this.SizeToContainer)
            {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Stretch;
                if(isHeightNan)
                this.Height = Double.NaN;
                if (isWidthNan)
                this.Width = Double.NaN;
            }
            this.AdjustOrientationSettings();
        }

        /// <summary>
        /// Refreshes the adorner layer.
        /// </summary>
        protected override void RefreshAdornerLayer()
        {
            base.RefreshAdornerLayer();
            if (mainglasspath != null)
            {
                if (this.Orientation == GaugeOrientation.Vertical)
                {
                    if (this.ActualHeight != 0)
                    {
                        this.mainglasspath.Height = (this.ActualHeight - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left) > 0 ? (this.ActualHeight - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left) : 1;
                        this.mainglasspath.Width = (this.ActualWidth - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left) > 0 ? (this.ActualWidth - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left) : 1;

                    }
                }
                else
                {
                    if (this.ActualWidth != 0)
                    {
                        this.mainglasspath.Height = (this.ActualWidth - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left) > 0 ? (this.ActualWidth - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left) : 1;
                        this.mainglasspath.Width = (this.ActualHeight - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left) > 0 ? (this.ActualHeight - 2 * this.FirstFrameThickness.Left - 2 * this.SecondFrameThickness.Left) : 1;

                    }
                }
                if (SkinStorage.GetVisualStyle(this).Equals("Blend"))
                {
                    this.mainglasspath.Opacity = 0.2;

                }
                else
                {
                    this.mainglasspath.Opacity = 0.7;

                }
                this.AdjustOrientationSettings();

            }
        }

        /// <summary>
        /// Updates the location of children elements.
        /// </summary>
        protected override void UpdateChildrenLocation()
        {
            int count = this.ChildrenCollection.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.ChildrenCollection[i] is LocalizableGaugeElement)
                {
                    LocalizableGaugeElement elem = this.ChildrenCollection[i] as LocalizableGaugeElement;
                    Point location = this.ConvertLocation(elem.Location, this.ActualWidth, this.ActualHeight);
                    elem.RenderTransform = new TranslateTransform(location.X, location.Y);
                }
            }
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Set all needed settings according to the orientation.
        /// </summary>
        private void AdjustOrientationSettings()
        {
            if (this.Orientation == GaugeOrientation.Horizontal)
            {
                foreach (GaugeImage image in this.Images)
                {
                    image.Angle = -90d;
                }

                foreach (StateIndicator indicator in this.StateIndicators)
                {
                    indicator.Angle = -90d;
                }
                if (this.mainglasspath != null)
                {
                    if (this.Height < this.Width || this.AutoSize || double.IsNaN(this.Height) || double.IsNaN(this.Width))
                    {
                        this.mainglasspath.RenderTransform = new RotateTransform(-90, this.mainglasspath.Width / 2, this.mainglasspath.Width / 2);
                    }
                }
            }
            else
            {
                this.RotationAngle = 0d;
                foreach (GaugeImage image in this.Images)
                {
                    image.Angle = 0;
                }

                foreach (StateIndicator indicator in this.StateIndicators)
                {
                    indicator.Angle = 0;
                }
                if (this.mainglasspath != null)
                {
                    if (this.Height > this.Width || this.AutoSize || double.IsNaN(this.Height) || double.IsNaN(this.Width))
                        this.mainglasspath.RenderTransform = null;
                }
            }
            foreach (LinearScale scale in this.Scales)
            {
                scale.Orientation = this.Orientation;
            }

            if (this.AutoSize)
            {
                double width = this.Width, height = this.Height;
                if (this.Orientation == GaugeOrientation.Horizontal)
                {
                    if (width < height)
                    {
                        this.Width = height;
                        this.Height = width;
                    }
                }
                else
                {
                    if (width > height)
                    {
                        this.Width = height;
                        this.Height = width;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the control is ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void LinearGaugeLoaded(object sender, RoutedEventArgs e)
        {
            int count = this.Scales.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.Scales[i] is LinearScale)
                {
                    LinearScale scale = this.Scales[i] as LinearScale;
                    if (scale != null && !(scale.ScaleBarSize == 0) && !(scale.ScaleBarLength == 0))
                    {
                        if (this.Orientation == GaugeOrientation.Vertical)
                        {
                            scale.GaugeActualHeight = this.ActualHeight;
                            scale.GaugeActualWidth = this.ActualWidth;
                            if (scale.ScaleBarLength < this.ActualHeight)
                            {
                                scale.LengthRatio = scale.ScaleBarLength / this.ActualHeight;
                            }
                            if (scale.ScaleBarSize < this.ActualWidth)
                            {
                                scale.SizeRatio = scale.ScaleBarSize / this.ActualWidth;
                            }
                        }
                        else
                        {
                            scale.GaugeActualHeight = this.ActualWidth;
                            scale.GaugeActualWidth = this.ActualHeight;
                            if (scale.ScaleBarLength < this.ActualWidth)
                            {
                                scale.LengthRatio = scale.ScaleBarLength / this.ActualWidth;
                            }
                            if (scale.ScaleBarSize < this.ActualHeight)
                            {
                                scale.SizeRatio = scale.ScaleBarSize / this.ActualHeight;
                            }
                        }
                        for (int j = 0; j < scale.Ranges.Count; j++)
                        {
                            LinearRange range = scale.Ranges[j] as LinearRange;
                            range.startSizeRatio = range.StartWidth / scale.ScaleBarSize;
                            range.endSizeRatio = range.EndWidth / scale.ScaleBarSize;
                        }                       
                        foreach (LinearPointer pointer in scale.Pointers)
                        {
                            pointer.SizeRatio = pointer.PointerWidth / scale.ScaleBarLength;
                            if (pointer is LinearMarkerPointer)
                            {
                                LinearMarkerPointer markerPointer = pointer as LinearMarkerPointer;
                                markerPointer.LengthSizeRatio = markerPointer.PointerLength / scale.ScaleBarSize;
                            }
                        }
                        foreach (StateIndicator stateIndicator in StateIndicators)
                        {
                            if (this.Orientation == GaugeOrientation.Vertical)
                            {
                                stateIndicator.startSizeRatio = stateIndicator.IndicatorWidth / this.ActualWidth;
                                stateIndicator.endSizeRatio = stateIndicator.IndicatorHeight / this.ActualHeight;
                            }
                            else
                            {
                                stateIndicator.startSizeRatio = stateIndicator.IndicatorWidth / this.ActualHeight;
                                stateIndicator.endSizeRatio = stateIndicator.IndicatorHeight / this.ActualWidth;
                            }
                        }
                    }                    
                }
            }            
            this.RefreshBorders();
            this.UpdateChildrenLocation();
            this.AdjustOrientationSettings();
        }

        /// <summary>
        /// Updates property value cache and raises FrameTypeChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFrameTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameTypeChanged != null)
            {
                this.FrameTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameTypeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearGauge instance = (LinearGauge)d;
            instance.OnFrameTypeChanged(e);
        }

        ///// <summary>
        ///// Sets the scope to <see cref="CircularGauge"/>to facilitate Binding. 
        ///// This method is invoked during initialization.
        ///// </summary>
        // internal void SetScope()
        // {
        //    for (int i = 0; i < this.Scales.Count; i++)
        //    {
        //        LinearScale scale = this.Scales[i];
        //        CalculateScope(scale);
        //        foreach (LinearPointer pointer in scale.Pointers)
        //        {
        //            CalculateScope(pointer);
        //        }
        //        foreach (LinearRange range in scale.Ranges)
        //        {
        //            CalculateScope(range);
        //        }
        //        foreach (TickBase tick in scale.Ticks)
        //        {
        //            CalculateScope(tick);
        //        }
        //    }
        //    for (int i = 0; i < this.StateIndicators.Count; i++)
        //    {
        //        StateIndicator stateIndicator = this.StateIndicators[i];
        //        CalculateScope(stateIndicator);
        //        foreach (StateRange range in stateIndicator.StateRanges)
        //        {
        //            CalculateScope(range);
        //        }
        //    }
        //    for (int i = 0; i < this.CustomLabels.Count; i++)
        //    {
        //        CalculateScope(this.CustomLabels[i]);
        //    }
        // }

        /// <summary>
        /// Calculates the scope. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> which contains the element to set the scope for.</param>
        internal void CalculateScope(DependencyObject obj)
        {
            DependencyObject ele = this;
            while (ele != null)
            {
                INameScope ns = NameScope.GetNameScope(ele);
                if (ns != null)
                {
                    if (!(ns is System.Windows.NameScope))
                    {
                        break;
                    }

                    NameScope.SetNameScope(obj, ns);
                    break;
                }

                ele = LogicalTreeHelper.GetParent(ele) ?? VisualTreeHelper.GetParent(ele);
            }
        }

        /// <summary>
        /// Updates property value cache and raises OrientationChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            this.AdjustOrientationSettings();
            if (OrientationChanged != null)
            {
                this.OrientationChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnOrientationChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearGauge instance = (LinearGauge)d;
            instance.OnOrientationChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises OrientationChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAutoSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AutoSizeChanged != null)
            {
                this.AutoSizeChanged(this, e);
            }
            this.AdjustOrientationSettings();
        }

        /// <summary>
        /// Calls OnOrientationChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAutoSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearGauge instance = (LinearGauge)d;
            instance.OnAutoSizeChanged(e);
        }

        /// <summary>
        /// Calls OnRotationAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRotationAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearGauge instance = (LinearGauge)d;
            instance.OnRotationAngleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises RotationAngleChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRotationAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RotationAngleChanged != null)
            {
                this.RotationAngleChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked when <see cref="LinearScale.ScaleBarLength"/> property of a scale is changed.
        /// </summary>
        /// <param name="sender">The <see cref="LinearGauge"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private void ScaleBarLengthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is LinearScale)
            {
                LinearScale scale = sender as LinearScale;
                if (this.Orientation == GaugeOrientation.Vertical)
                {
                    if (scale.ScaleBarLength < this.ActualHeight)
                    {
                        scale.LengthRatio = scale.ScaleBarLength / this.ActualHeight;
                    }
                }
                else
                {
                    if (scale.ScaleBarLength < this.ActualWidth)
                    {
                        scale.LengthRatio = scale.ScaleBarLength / this.ActualWidth;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when <see cref="Control.BackgroundProperty"/> of the gauge is changed.
        /// </summary>
        /// <param name="sender">The <see cref="LinearGauge"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            LinearGauge instance = sender as LinearGauge;
            instance.RefreshBorders();
        }
        #endregion Implementation
    }
}
