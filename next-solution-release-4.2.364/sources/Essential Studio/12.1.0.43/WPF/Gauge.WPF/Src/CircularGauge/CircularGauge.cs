// <copyright file="CircularGauge.cs" company="Syncfusion Software">
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
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using Syncfusion.Licensing;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the CircularGauge UI element.
    /// </summary>
    /// <remarks>Circular gauges are perfect for presenting values of a specific range. 
    /// It can be used to create sophisticated gaming applications, dashboards, clocks, 
    /// industrial equipments and many more.In short it could be used for almost anything one could 
    /// ever imagine of, for representing a range of values in Circular form.
    /// Circular gauge can accommodate multiple scales, pointers, ranges, state indicators, labels 
    /// and images.It has a virtual bounded rectangle around it. The top-left corner of the virtual bounded 
    /// rectangle is represented by the co-ordinate(0, 0) and that of the bottom-right is represented 
    /// by (100, 100).Each and every gauge element is drawn inside the bounded rectangle 
    /// based on the location property.
    /// </remarks>
    /// <seealso cref="LinearGauge"/>
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
    /// <example><code>
    /// <Window xmlns:local="clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.WPF">
    /// <local:CircularGauge Name="CircularGauge1"/></Window>
    /// </code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example>
    /// <code>
    /// public partial class CircularGauge : Control
    /// </code></example>
    /// </list>
    /// <para/>
    /// </example>
    /// </list>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="CircularGauge"/> in XAML.
    /// <code>
    /// <Window x:Class="GaugeControlTesting.Window1"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:sync="clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.WPF"
    ///  xmlns:sfshared="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.WPF" 
    ///  Title="Circular Gauge">
    ///  <Grid>
    ///     <sync:CircularGauge CenterFrameFillColor="Brown"
    ///                        FrameType="FullCircle"
    ///                        Radius="150"> 
    ///            <sync:CircularGauge.Scales>
    ///                <sync:CircularScale Name="CircularScale" 
    ///                                    Radius = "120" 
    ///                                    Minimum="0" 
    ///                                    Maximum="100"
    ///                                    MinorIntervalValue="2" 
    ///                                    MajorIntervalValue="10"
    ///                                    StartAngle="180" 
    ///                                    GapSweepAngle="360"
    ///                                    ShadowOffset="3"
    ///                                    ScaleBarSize = "5">
    ///                    <sync:CircularScale.Ticks>
    ///                        <sync:CircularLabelTick FontSize="10" TickStyle="MajorTick" BackgroundBrush="Red" 
    ///                                                TickPlacement="Inside" DistanceFromScale="5"/>
    ///                        <sync:CircularMarkTick TickHeight="9" TickShape="Rectangle" TickStyle="MajorTick"
    ///                                                TickWidth="4" BackgroundBrush="Pink"/>
    ///                        <sync:CircularMarkTick TickHeight="4" TickWidth="1" TickStyle="MinorTick"
    ///                                               BackgroundBrush="Aqua" TickPlacement="Inside"/>
    ///                    </sync:CircularScale.Ticks>
    ///                    <sync:CircularScale.Pointers>
    ///                        <sync:CircularPointer BackgroundBrush="Blue"
    ///                                              BorderBrush="Red" 
    ///                                              PointerLength="130" 
    ///                                              PointerWidth="5"
    ///                                              PointerNeedleType="Needle" 
    ///                                              PointerPlacement="Inside"/>                       
    ///                    </sync:CircularScale.Pointers>
    ///                    <sync:CircularScale.PointerCap>
    ///                        <sync:PointerCap BackgroundBrush="Green" 
    ///                                         BorderBrush="DarkGreen" 
    ///                                         BorderWidth="1" 
    ///                                         PointerCapRadius="5">
    ///                        </sync:PointerCap>
    ///                    </sync:CircularScale.PointerCap>
    ///                    <sync:CircularScale.Ranges>                        
    ///                        <sync:CircularRange StartValue="40" EndValue="60" 
    ///                                            StartWidth="0" EndWidth="20" 
    ///                                            RangePosition="Inside" DistanceFromScale="2"/>                        
    ///                    </sync:CircularScale.Ranges>
    ///               </sync:CircularScale>            
    ///            </sync:CircularGauge.Scales>
    ///        </sync:CircularGauge>
    ///    </Grid>
    /// </Window>
    /// </code>
    /// <para/>The following example shows how to create a <see cref="CircularGauge"/> in C#.
    /// <code>
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace CircularGaugeSample
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///        private CircularScale m_scale;
    ///        private StateIndicator m_indicator;<para/>
    ///        public Window1()
    ///        {
    ///             InitializeComponent();<para/>
    ///             m_scale = new CircularScale();
    ///             m_scale.ShadowOffset = 1;
    ///             m_scale.Minimum = 0;
    ///             m_scale.Maximum = 100;
    ///             m_scale.MinorIntervalValue = 2;
    ///             m_scale.MajorIntervalValue = 10;
    ///             m_scale.StartAngle = 120;
    ///             m_scale.GapSweepAngle = 300;
    ///             m_scale.ScaleBarSize = 1.5;
    ///             m_scale.Radius = 116;
    ///             this.circularGauge1.Scales.Add( m_scale );
    ///             CircularLabelTick majorLabelTick = new CircularLabelTick();
    ///             majorLabelTick.FontSize = 11;
    ///             majorLabelTick.TickStyle = TickStyle.MajorTick;
    ///             majorLabelTick.TickPlacement = ScalePlacement.Inside;
    ///             majorLabelTick.DistanceFromScale = 5;
    ///             CircularMarkTick majorTick = new CircularMarkTick();
    ///             majorTick.TickWidth = 4;
    ///             majorTick.TickHeight = 9;
    ///             majorTick.TickStyle = TickStyle.MajorTick;
    ///             majorTick.BackgroundBrush = new SolidColorBrush( Color.FromRgb( 0, 59, 137 ) );
    ///             majorTick.TickShape = TickShape.Ellipse;
    ///             CircularMarkTick minorTick = new CircularMarkTick();
    ///             minorTick.TickWidth = 1;
    ///             minorTick.TickHeight = 4;
    ///             minorTick.TickStyle = TickStyle.MinorTick;
    ///             minorTick.BackgroundBrush = new SolidColorBrush( Color.FromRgb( 0, 59, 137 ) );
    ///             m_scale.Ticks.Add( minorTick );
    ///             m_scale.Ticks.Add( majorTick );
    ///             m_scale.Ticks.Add( majorLabelTick );
    ///             m_scale.PointerCap.PointerCapRadius = 5;
    ///             m_scale.PointerCap.BackgroundBrush = new RadialGradientBrush( Color.FromRgb( 194, 207, 229 ), Color.FromRgb( 46, 94, 160 ) );
    ///             CircularPointer pointer1 = new CircularPointer();
    ///             pointer1.BackgroundBrush = new LinearGradientBrush( Color.FromRgb( 194, 207, 229 ), Color.FromRgb( 46, 94, 160 ), 90 );
    ///             pointer1.PointerPlacement = ScalePlacement.Outside;
    ///             m_scale.Pointers.Add( pointer1 );
    ///             m_indicator = new StateIndicator();
    ///             m_indicator.StateRanges.Add( new StateRange( 10, 20 ) );
    ///             m_indicator.StateRanges.Add( new StateRange( 50, 60 ) );
    ///             m_indicator.BackgroundBrush = new RadialGradientBrush( Colors.White, Colors.DarkGreen );
    ///             m_indicator.ActiveBackgroundBrush = new RadialGradientBrush( Colors.White, Colors.Red );
    ///             m_indicator.IndicatorWidth = 20;
    ///             m_indicator.IndicatorHeight = 20;
    ///             m_indicator.Location = new Point( 50, 80 );
    ///             circularGauge1.StateIndicators.Add( m_indicator );
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example> 
#if SyncfusionFramework4_0
    [DesignTimeVisible(true)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
     Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
     Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
     Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
  Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
     Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/DefaultStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
        Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Gauge.WPF;component/Themes/MetroStyle.xaml")]
    [StyleTypedProperty(Property = "MinorTickStyle", StyleTargetType = typeof(CircularMarkTick))]
    [StyleTypedProperty(Property = "MajorTickStyle", StyleTargetType = typeof(CircularMarkTick))]
    [StyleTypedProperty(Property = "PointerCapStyle", StyleTargetType = typeof(PointerCap))]
    [StyleTypedProperty(Property = "LabelTickStyle", StyleTargetType = typeof(CircularLabelTick))]
    [StyleTypedProperty(Property = "PointerStyle", StyleTargetType = typeof(CircularPointer))]

    public class CircularGauge : GaugeBase
    {
        #region Private Members
        /// <summary>
        /// First half frame.
        /// </summary>
        private HalfCircleBorder m_firstHalfCircleBorder;

        /// <summary>
        /// Inner half frame.
        /// </summary>
        private HalfCircleBorder m_innerHalfCircleBorder;

        /// <summary>
        /// Collection of circular scales.
        /// </summary>
        private ScaleCollection m_scales;

        /// <summary>
        /// Second half frame.
        /// </summary>
        private HalfCircleBorder m_secondHalfCircleBorder;

        private Path mainglasspath;
        private Path secondglasspath;
        private Style pointerStyle;
        private Style pointerCapStyle;
        private Style majorTickStyle;
        private Style minorTickStyle;
        private Style labelTickStyle;

        #endregion Private Members

        #region CLR Getters & Setters
        /// <summary>
        /// Gets or sets a collection of circular scales.
        /// </summary>
        /// <value>
        /// Type: <see cref="ScaleCollection"/>
        /// </value>      
        public ScaleCollection Scales
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

        /// <summary>
        /// Gets half vircle's first frame border.
        /// </summary>
        /// <value>
        /// Type: <see cref="HalfCircleBorder"/>
        /// </value>
        /// <seealso cref="HalfCircleBorder"/>
        internal HalfCircleBorder FirstHalfCircleBorder
        {
            get
            {
                return m_firstHalfCircleBorder;
            }
        }

        /// <summary>
        /// Gets half circle's second frame border.
        /// </summary>
        /// <value>
        /// Type: <see cref="HalfCircleBorder"/>
        /// </value>
        /// <seealso cref="HalfCircleBorder"/>
        internal HalfCircleBorder SecondHalfCircleBorder
        {
            get
            {
                return m_secondHalfCircleBorder;
            }
        }

        /// <summary>
        /// Gets half circle's inner frame border.
        /// </summary>
        /// <value>
        /// Type: <see cref="HalfCircleBorder"/>
        /// </value>
        /// <seealso cref="HalfCircleBorder"/>
        internal HalfCircleBorder InnerHalfCircleBorder
        {
            get
            {
                return m_innerHalfCircleBorder;
            }
        }
        #endregion CLR Getters & Setters

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="FrameType"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="Radius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback RadiusChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="FrameType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameTypeProperty =
            DependencyProperty.Register("FrameType", typeof(GaugeFrameType), typeof(CircularGauge), new FrameworkPropertyMetadata(GaugeFrameType.FullCircle, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnFrameTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="HalfCircleInnerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HalfCircleInnerRadiusProperty =
           DependencyProperty.Register("HalfCircleInnerRadius", typeof(double), typeof(CircularGauge), new FrameworkPropertyMetadata(15d));

        /// <summary>
        /// Identifies the <see cref="HalfCircleInnerSweepDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HalfCircleInnerSweepDirectionProperty =
           DependencyProperty.Register("HalfCircleInnerSweepDirection", typeof(SweepDirection), typeof(CircularGauge), new FrameworkPropertyMetadata(SweepDirection.Counterclockwise));

        /// <summary>
        /// Identifies the <see cref="HalfCircleSweepDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HalfCircleSweepDirectionProperty =
           DependencyProperty.Register("HalfCircleSweepDirection", typeof(SweepDirection), typeof(CircularGauge), new FrameworkPropertyMetadata(SweepDirection.Clockwise));

        /// <summary>
        /// Identifies the <see cref="Radius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(CircularGauge), new FrameworkPropertyMetadata(150d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnRadiusChanged)));

        #endregion Dependency Properties

        #region DP Getters & Setters

        /// <summary>
        /// PointerStyle
        /// </summary>
        public Style PointerStyle
        {
            get { return this.pointerStyle; }
            set { this.pointerStyle = value; }
        }

        /// <summary>
        /// PointerCapStyle
        /// </summary>
        public Style PointerCapStyle
        {
            get { return pointerCapStyle; }
            set { pointerCapStyle = value; }
        }

        /// <summary>
        /// MajorTickStyle
        /// </summary>
        public Style MajorTickStyle
        {
            get { return majorTickStyle; }
            set { majorTickStyle = value; }
        }

        /// <summary>
        /// MinorTickStyle
        /// </summary>
        public Style MinorTickStyle
        {
            get { return minorTickStyle; }
            set { minorTickStyle = value; }
        }

        /// <summary>
        /// LabelTickStyle
        /// </summary>
        public Style LabelTickStyle
        {
            get { return this.labelTickStyle; }
            set { this.labelTickStyle = value; }
        }

        /// <summary>
        /// Gets or sets the CircularGauge's FrameType.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="GaugeFrameType"/>
        /// </value>       
        [Bindable(true)]
        [Category("Appearance")]
        public GaugeFrameType FrameType
        {
            get
            {
                return (GaugeFrameType)GetValue(FrameTypeProperty);
            }

            set
            {
                SetValue(FrameTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the inner radius of the HalfCircle style CircularGauge.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 15.
        /// </value>       
        [Bindable(true)]
        [Category("HalfCircle Properties")]
        public double HalfCircleInnerRadius
        {
            get
            {
                return (double)GetValue(HalfCircleInnerRadiusProperty);
            }

            set
            {
                SetValue(HalfCircleInnerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the inner half circle's sweep direction for HalfCircle style CircularGauge.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="SweepDirection"/>
        /// Default value is <see cref="SweepDirection.Counterclockwise"/>
        /// </value>      
        [Bindable(true)]
        [Category("HalfCircle Properties")]
        public SweepDirection HalfCircleInnerSweepDirection
        {
            get
            {
                return (SweepDirection)GetValue(HalfCircleInnerSweepDirectionProperty);
            }

            set
            {
                SetValue(HalfCircleInnerSweepDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sweep direction of the HalfCircle style CircularGauge.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="SweepDirection"/>
        /// Default value is <see cref="SweepDirection.Clockwise"/>
        /// </value>    
        /// <remarks>
        /// CouuterclockWise halfcircle can be obtained by setting
        /// the FrameType Property to CounterClockwiseHalfCircle.
        /// This Property is Obsolete.
        /// </remarks>
        [Bindable(true)]
        [Category("HalfCircle Properties")]
        public SweepDirection HalfCircleSweepDirection
        {
            get
            {
                return (SweepDirection)GetValue(HalfCircleSweepDirectionProperty);
            }

            set
            {
                SetValue(HalfCircleSweepDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius of the CircularGauge.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 150.
        /// </value>     
        [Bindable(true)]
        [Category("Appearance")]
        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }

            set
            {
                SetValue(RadiusProperty, value);
            }
        }



     
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="CircularGauge"/> class.
        /// Overrides DefaultStyleKeyProperty dependency property.
        /// </summary>
        static CircularGauge()
        {
            EnvironmentTest.ValidateLicense(typeof(CircularGauge));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularGauge), new FrameworkPropertyMetadata(typeof(CircularGauge)));
            Control.BackgroundProperty.OverrideMetadata(typeof(CircularGauge), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBackgroundChanged)));

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularGauge"/> class.
        /// </summary>
        public CircularGauge()
        {
            this.Scales = new ScaleCollection();
            this.Scales.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            this.Loaded += new RoutedEventHandler(CircularGauge_Loaded);
        }

        void CircularGauge_Loaded(object sender, RoutedEventArgs e)
        {
            this.RefreshBorders();
            this.UpdateChildrenLocation();
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
            this.Loaded += new RoutedEventHandler(CircularGaugeLoaded);
            for (int i = 0; i < this.Scales.Count; i++)
            {
                CircularScale scale = this.Scales[i];
                foreach (CircularPointer pointer in scale.Pointers)
                {
                    pointer.PointerRatio = this.Radius - pointer.PointerLength;
                }
            }

            for (int i = 0; i < this.StateIndicators.Count; i++)
            {
                StateIndicator stateIndicator = this.StateIndicators[i];
                stateIndicator.IndicatorRadiusRatio = this.Radius / (stateIndicator.IndicatorHeight + stateIndicator.IndicatorWidth);
                CalculateScope(stateIndicator);
            }

            // this.SetScope();
        }

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
           
            if (this.FrameType == GaugeFrameType.HalfCircle || this.FrameType == GaugeFrameType.CounterclockwiseHalfCircle)
            {
                m_firstHalfCircleBorder = this.Template.FindName("HalfCircleFirstBorder", this) as HalfCircleBorder;
                if (m_firstHalfCircleBorder != null)
                {
                    m_firstHalfCircleBorder.Style = this.FirstFrameStyle;
                }

                m_secondHalfCircleBorder = this.Template.FindName("HalfCircleSecondBorder", this) as HalfCircleBorder;
                if (m_secondHalfCircleBorder != null)
                {
                    m_secondHalfCircleBorder.Style = this.SecondFrameStyle;
                }

                m_innerHalfCircleBorder = this.Template.FindName("HalfCircleInnerBorder", this) as HalfCircleBorder;
                if (m_innerHalfCircleBorder != null)
                {
                    m_innerHalfCircleBorder.Style = this.InnerFrameStyle;
                }
            }
            else if (this.FrameType == GaugeFrameType.LeftHalfCircle)
            {
                m_firstHalfCircleBorder = this.Template.FindName("LeftHalfCircleFirstBorder", this) as HalfCircleBorder;
                if (m_firstHalfCircleBorder != null)
                {
                    m_firstHalfCircleBorder.Style = this.FirstFrameStyle;
                }

                m_secondHalfCircleBorder = this.Template.FindName("LeftHalfCircleSecondBorder", this) as HalfCircleBorder;
                if (m_secondHalfCircleBorder != null)
                {
                    m_secondHalfCircleBorder.Style = this.SecondFrameStyle;
                }

                m_innerHalfCircleBorder = this.Template.FindName("LeftHalfCircleInnerBorder", this) as HalfCircleBorder;
                if (m_innerHalfCircleBorder != null)
                {
                    m_innerHalfCircleBorder.Style = this.InnerFrameStyle;
                }
            }
            else if (this.FrameType == GaugeFrameType.RightHalfCircle)
            {
                m_firstHalfCircleBorder = this.Template.FindName("RightHalfCircleFirstBorder", this) as HalfCircleBorder;
                if (m_firstHalfCircleBorder != null)
                {
                    m_firstHalfCircleBorder.Style = this.FirstFrameStyle;
                }

                m_secondHalfCircleBorder = this.Template.FindName("RightHalfCircleSecondBorder", this) as HalfCircleBorder;
                if (m_secondHalfCircleBorder != null)
                {
                    m_secondHalfCircleBorder.Style = this.SecondFrameStyle;
                }

                m_innerHalfCircleBorder = this.Template.FindName("RightHalfCircleInnerBorder", this) as HalfCircleBorder;
                if (m_innerHalfCircleBorder != null)
                {
                    m_innerHalfCircleBorder.Style = this.InnerFrameStyle;
                }
            }

            this.mainglasspath = this.GetTemplateChild("GlassPath") as Path;
            this.secondglasspath = this.GetTemplateChild("SecondPath") as Path;

            this.RefreshBorders();

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
        /// Called when the size of the Circular Gauge changes.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (SizeToContainer == true)
            {
                double ticksweight = 0d;
                for (int i = 0; i < this.Scales.Count; i++)
                {
                    CircularScale scale = this.Scales[i];
                    if (this.RenderSize.Width > this.RenderSize.Height)
                    {
                        foreach (TickBase tick in scale.Ticks)
                        {
                            if (tick is CircularMarkTick)
                            {
                                CircularMarkTick marktick = tick as CircularMarkTick;
                                if (marktick.TickPlacement == ScalePlacement.Cross || marktick.TickPlacement == ScalePlacement.Outside)
                                {
                                    ticksweight = ((scale.ScaleBarSize + tick.DesiredSize.Width) / 2) + marktick.DistanceFromScale;
                                }
                            }
                        }

                        if (DesignerProperties.GetIsInDesignMode(this))
                        {
                            scale.Radius = this.RenderSize.Height / 2 - (scale.ScaleBarSize + this.FirstFrameThickness.Top + this.SecondFrameThickness.Bottom + ticksweight) / 2;
                        }
                        this.Radius = this.RenderSize.Height / 2;

                    }
                    else
                    {
                        if (DesignerProperties.GetIsInDesignMode(this))
                        {
                            scale.Radius = this.RenderSize.Width / 2 - (scale.ScaleBarSize + this.FirstFrameThickness.Left + this.SecondFrameThickness.Right + ticksweight) / 2;
                        }
                        this.Radius = this.RenderSize.Width / 2;
                    }

                    scale.InvalidateVisual();
                    foreach (CircularPointer pointer in scale.Pointers)
                    {
                        if (pointer.EnableSizeToContainer)
                        {
                            pointer.PointerLength = this.Radius - pointer.PointerRatio;
                            pointer.InvalidateVisual();
                        }
                    }
                    foreach (StateIndicator  indicators in this.StateIndicators)
                    {
                        indicators.IndicatorHeight = (this.Radius / indicators.IndicatorRadiusRatio) - indicators.IndicatorRadiusRatio;
                        indicators.IndicatorWidth = (this.Radius / indicators.IndicatorRadiusRatio) - indicators.IndicatorRadiusRatio;
                        //if (sizeInfo.NewSize.Height < sizeInfo.PreviousSize.Height || sizeInfo.NewSize.Width < sizeInfo.NewSize.Width)
                        //{
                        //    indicators.IndicatorWidth = indicators.IndicatorWidth - (this.Radius / (this.Radius - indicators.IndicatorWidth * 0.5));
                        //    indicators.IndicatorHeight = indicators.IndicatorHeight - (this.Radius / (this.Radius - indicators.IndicatorHeight * 0.5));
                        //}
                        //else
                        //{
                        //    indicators.IndicatorWidth = indicators.IndicatorWidth + (this.Radius / (this.Radius - indicators.IndicatorWidth * 0.5));
                        //    indicators.IndicatorHeight = indicators.IndicatorHeight + (this.Radius / (this.Radius - indicators.IndicatorHeight * 0.5));
                        //}
                    }
                }
            }

            this.UpdateChildrenLocation();

            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire collection is refreshed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        protected override void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.CollectionChanged(sender, e);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    if (e.NewItems[i] is CircularScale)
                    {
                        CircularScale elem = e.NewItems[i] as CircularScale;
                        if (elem != null)
                        {
                            elem.RadiusChanged += new PropertyChangedCallback(ScaleRadiusChanged);
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
                    else if (e.NewItems[i] is CircularCustomLabel)
                    {
                        CircularCustomLabel elem = e.NewItems[i] as CircularCustomLabel;
                        elem.SetScope(this);
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the adorner layer.
        /// </summary>
        protected override void RefreshAdornerLayer()
        {
            if (GaugeAdorner != null && GaugeAdorner is CircularGaugeAdorner)
            {
                (GaugeAdorner as CircularGaugeAdorner).Radius = this.Radius;
            }

            base.RefreshAdornerLayer();
        }

        /// <summary>
        /// Updates CircularGauge frames.
        /// </summary>
        protected override void RefreshBorders()
        {
            base.RefreshBorders();

            if (!this.ApplyFrameStyles)
            {
                if (this.FrameType == GaugeFrameType.HalfCircle || this.FrameType == GaugeFrameType.LeftHalfCircle || this.FrameType == GaugeFrameType.RightHalfCircle || this.FrameType == GaugeFrameType.CounterclockwiseHalfCircle)
                {
                    if (this.FirstHalfCircleBorder != null)
                    {
                        if (FirstFrameFillColor != null)
                        {
                            if (IsColorMergeWithBase)
                            {
                                if (FirstFrameFillColor is SolidColorBrush)
                                {
                                    this.FirstHalfCircleBorder.BackgroundBrush = MergeColor(FirstFrameBrush, (FirstFrameFillColor as SolidColorBrush).Color);
                                }
                                else
                                {
                                    this.FirstHalfCircleBorder.BackgroundBrush = FirstFrameFillColor;
                                }
                            }
                            else
                            {
                                this.FirstHalfCircleBorder.BackgroundBrush = FirstFrameFillColor;
                            }
                        }
                        else
                        {
                            this.FirstHalfCircleBorder.BackgroundBrush = FirstFrameBrush;
                        }

                        this.FirstHalfCircleBorder.OpacityMask = this.OpacityMask;
                        this.FirstHalfCircleBorder.Opacity = this.Opacity;
                    }

                    if (this.SecondHalfCircleBorder != null)
                    {
                        if (SecondFrameFillColor != null)
                        {
                            if (IsColorMergeWithBase)
                            {
                                if (SecondFrameFillColor is SolidColorBrush)
                                {
                                    this.SecondHalfCircleBorder.BackgroundBrush = MergeColor(SecondFrameBrush, (SecondFrameFillColor as SolidColorBrush).Color);
                                }
                                else
                                {
                                    this.SecondHalfCircleBorder.BackgroundBrush = SecondFrameFillColor;
                                }
                            }
                            else
                            {
                                this.SecondHalfCircleBorder.BackgroundBrush = SecondFrameFillColor;
                            }
                        }
                        else
                        {
                            this.SecondHalfCircleBorder.BackgroundBrush = SecondFrameBrush;
                        }

                        this.SecondHalfCircleBorder.OpacityMask = this.OpacityMask;
                        this.SecondHalfCircleBorder.Opacity = this.Opacity;
                    }

                    if (this.InnerHalfCircleBorder != null)
                    {
                        if (this.Background == null)
                        {
                            if (CenterFrameFillColor != null)
                            {
                                if (IsColorMergeWithBase)
                                {
                                    if (CenterFrameFillColor is SolidColorBrush)
                                    {
                                        this.InnerHalfCircleBorder.BackgroundBrush = MergeColor(InnerFrameBrush, (CenterFrameFillColor as SolidColorBrush).Color);
                                    }
                                    else
                                    {
                                        this.InnerHalfCircleBorder.BackgroundBrush = CenterFrameFillColor;
                                    }
                                }
                                else
                                {
                                    this.InnerHalfCircleBorder.BackgroundBrush = CenterFrameFillColor;
                                }
                            }
                            else
                            {
                                this.InnerHalfCircleBorder.BackgroundBrush = InnerFrameBrush;
                            }
                        }
                        else
                        {
                            this.InnerHalfCircleBorder.BackgroundBrush = this.Background;
                        }

                        this.InnerHalfCircleBorder.OpacityMask = this.OpacityMask;
                        this.InnerHalfCircleBorder.Opacity = this.Opacity;
                        if (this.mainglasspath != null)
                        {
                            this.secondglasspath.Visibility = Visibility.Collapsed;
                            if (this.Radius - this.FirstFrameThickness.Top - this.SecondFrameThickness.Top > 0)
                            {
                                this.mainglasspath.Height = (this.Radius - this.FirstFrameThickness.Top - this.SecondFrameThickness.Top) / 2;
                                this.mainglasspath.Width = 2 * 2 * (this.Radius - this.FirstFrameThickness.Top - this.SecondFrameThickness.Top) / 3;
                                this.RenderTransform = null;
                                this.mainglasspath.RenderTransform = new TranslateTransform(this.Radius / 4 + 3, 3);
                            }
                            else
                            {
                                //Disabling effects if the radius less than normal values.
                                this.mainglasspath.Visibility = Visibility.Collapsed;
                                this.secondglasspath.Visibility = Visibility.Collapsed;
                            }
                        }


                    }
                    if (this.FrameType == GaugeFrameType.CounterclockwiseHalfCircle)
                    {
                        if (this.mainglasspath != null)
                        {
                            TransformGroup transformgrup = new TransformGroup();
                            TransformGroup transformgrup1 = new TransformGroup();
                            RotateTransform transform = new RotateTransform(180, 0, 0);
                            TranslateTransform trans1 = new TranslateTransform(this.Radius * .85, this.Radius / 5);
                            transformgrup1.Children.Add(transform);
                            transformgrup1.Children.Add(trans1);
                            this.secondglasspath.RenderTransform = transformgrup1;

                            TranslateTransform trans = new TranslateTransform(this.Radius + this.FirstFrameThickness.Left, 2 * this.Radius / 3);
                            this.secondglasspath.Visibility = Visibility.Collapsed;
                            transformgrup.Children.Add(transform);
                            transformgrup.Children.Add(trans);
                            this.mainglasspath.RenderTransform = transformgrup;
                            this.mainglasspath.Visibility = Visibility.Collapsed;
                        }
                    }
                    else if (this.FrameType == GaugeFrameType.HalfCircle)
                    {
                        if (this.mainglasspath != null)
                            this.mainglasspath.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (this.FirstCircleBorder != null)
                    {
                        if (FirstFrameFillColor != null)
                        {
                            if (IsColorMergeWithBase)
                            {
                                //VS2010 condition checking because merging colors is not needed here.
                                //if (FirstFrameFillColor is SolidColorBrush && FirstFrameFillColor != Brushes.Transparent)
                                if (FirstFrameFillColor is SolidColorBrush && FirstFrameFillColor != Brushes.Transparent && SkinStorage.GetVisualStyle(this).ToString() != "VS2010")
                                {
                                    this.FirstCircleBorder.Background = MergeColor(FirstFrameBrush, (FirstFrameFillColor as SolidColorBrush).Color);
                                }
                                else
                                {
                                    this.FirstCircleBorder.Background = FirstFrameFillColor;
                                }
                            }
                            else
                            {
                                this.FirstCircleBorder.Background = FirstFrameFillColor;
                            }
                        }
                        else
                        {
                            this.FirstCircleBorder.Background = FirstFrameBrush;
                        }

                        this.FirstCircleBorder.OpacityMask = this.OpacityMask;
                        this.FirstCircleBorder.Opacity = this.Opacity;
                    }

                    if (this.SecondCircleBorder != null)
                    {
                        if (SecondFrameFillColor != null)
                        {
                            if (IsColorMergeWithBase)
                            {
                                if (SecondFrameFillColor is SolidColorBrush && SecondFrameFillColor != Brushes.Transparent)
                                {
                                    this.SecondCircleBorder.Background = MergeColor(SecondFrameBrush, (this.SecondFrameFillColor as SolidColorBrush).Color);
                                }
                                else
                                {
                                    this.SecondCircleBorder.Background = this.SecondFrameFillColor;
                                }
                            }
                            else
                            {
                                this.SecondCircleBorder.Background = this.SecondFrameFillColor;
                            }
                        }
                        else
                        {
                            this.SecondCircleBorder.Background = SecondFrameBrush;
                        }

                        this.SecondCircleBorder.OpacityMask = this.OpacityMask;
                        this.SecondCircleBorder.Opacity = this.Opacity;
                    }

                    if (this.InnerCircleBorder != null)
                    {
                        if (this.Background == null)
                        {
                            if (CenterFrameFillColor != null)
                            {
                                if (IsColorMergeWithBase)
                                {
                                    if (CenterFrameFillColor is SolidColorBrush && CenterFrameFillColor != Brushes.Transparent)
                                    {
                                        if (SkinStorage.GetVisualStyle(this).ToString() == "VS2010")
                                            this.InnerCircleBorder.Background = InnerFrameBrush;
                                        else
                                            this.InnerCircleBorder.Background = MergeColor(InnerFrameBrush, (CenterFrameFillColor as SolidColorBrush).Color);
                                    }
                                    else
                                    {
                                        this.InnerCircleBorder.Background = CenterFrameFillColor;
                                    }
                                }
                                else
                                {
                                    this.InnerCircleBorder.Background = CenterFrameFillColor;
                                }
                            }
                            else
                            {
                                this.InnerCircleBorder.Background = InnerFrameBrush;
                            }
                        }
                        else
                        {
                            this.InnerCircleBorder.Background = this.Background;
                        }

                        this.InnerCircleBorder.OpacityMask = this.OpacityMask;
                        this.InnerCircleBorder.Opacity = this.Opacity;
                        if (this.mainglasspath != null)
                        {
                            //this.secondglasspath.Visibility = Visibility.Visible;
                            if (this.Radius - this.FirstFrameThickness.Top - this.SecondFrameThickness.Top > 0)
                            {
                                this.mainglasspath.Height = 2 * (this.Radius - this.FirstFrameThickness.Top - this.SecondFrameThickness.Top) / 2;
                                this.mainglasspath.Width = 2 * (this.Radius - this.FirstFrameThickness.Top - this.SecondFrameThickness.Top);
                                this.secondglasspath.Width = 2 * (this.Radius - this.FirstFrameThickness.Top - this.SecondFrameThickness.Top) / 5;
                                this.secondglasspath.Height = 2 * (this.Radius - this.FirstFrameThickness.Top - this.SecondFrameThickness.Top) / 5;
                                this.RenderTransform = null;
                                this.secondglasspath.RenderTransform = new TranslateTransform(this.Radius / 3, this.Radius / 3);
                                this.mainglasspath.RenderTransform = new TranslateTransform(-1, 3);
                            }
                            else
                            {
                                //Disabling effects if the radius less than normal values.
                                this.mainglasspath.Visibility = Visibility.Collapsed;
                                this.secondglasspath.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
                if (this.mainglasspath != null)
                {
                    if (SkinStorage.GetVisualStyle(this).Equals("Blend") || SkinStorage.GetVisualStyle(this).Equals("Default"))
                    {
                        this.mainglasspath.Opacity = 0.8;
                        this.secondglasspath.Opacity = 0.4;

                    }
                    else
                    {
                        this.mainglasspath.Opacity = 1;
                        this.secondglasspath.Opacity = 1;

                    }
                }
            }
        }


        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Invoked when the control is ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void CircularGaugeLoaded(object sender, RoutedEventArgs e)
        {
            if (GaugeAdorner != null && GaugeAdorner is CircularGaugeAdorner)
            {
                (GaugeAdorner as CircularGaugeAdorner).Radius = this.Radius;
            }

            int count = this.ChildrenCollection.Count;
            if (count > 0)
            {

                for (int i = 0; i < count; i++)
                {
                    if (this.ChildrenCollection[i] is CircularScale)
                    {
                        CircularScale scale = this.ChildrenCollection[i] as CircularScale;
                        if (scale.Radius < this.Radius)
                        {
                            scale.RadiusRatio = scale.Radius / this.Radius;
                        }
                    }
                }
            }
            else
            {
                CircularScale defaultScale = new CircularScale() {isDefaultScale = true, ShadowOffset = 1,  BorderWidth = 1.2,  ScaleBarSize = 30, BorderBrush = new  SolidColorBrush(Colors.Transparent), BackgroundBrush = new  SolidColorBrush(Colors.Transparent), Maximum = 100, StartAngle = 120, GapSweepAngle = 300, MajorIntervalValue = 10, MinorIntervalValue = 2, Minimum = 0, Radius = 120 };
                defaultScale.Ticks.Add(new CircularMarkTick() { TickStyle = TickStyle.MinorTick, TickHeight = 7, TickWidth = 2, DistanceFromScale = 3 });
                defaultScale.Pointers.Add(new CircularPointer() { PointerLength=100, PointerWidth=20});
                defaultScale.PointerCap = new PointerCap() {CapOnTop = true, Width = 25, PointerCapRadius = 8, PointerCapType = PointerCapType.Default }; 
                defaultScale.Ticks.Add(new CircularMarkTick() { TickStyle = TickStyle.MajorTick, TickHeight = 10, TickWidth  = 2, TickShape = TickShape.Rectangle });
                defaultScale.Ticks.Add(new CircularLabelTick() { TickStyle = TickStyle.MajorTick, DistanceFromScale = 3, FontSize = 11, TickPlacement = ScalePlacement.Inside });
                this.Scales.Add(defaultScale);
            }

            this.RefreshBorders();
            this.UpdateChildrenLocation();
        }

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
        /// Invoked when <see cref="Control.BackgroundProperty"/> of the gauge is changed.
        /// </summary>
        /// <param name="sender">The <see cref="CircularGauge"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge instance = sender as CircularGauge;
            instance.RefreshBorders();
        }

        /// <summary>
        /// Invoked when <see cref="Radius"/> property of a scale is changed.
        /// </summary>
        /// <param name="sender">The <see cref="CircularGauge"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private void ScaleRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is CircularScale)
            {
                CircularScale scale = sender as CircularScale;
                if (scale.Radius < this.Radius)
                {
                    scale.RadiusRatio = scale.Radius / this.Radius;
                }
            }
        }

        /// <summary>
        /// Updates property value cache and raises FrameTypeChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFrameTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();
            if (FrameTypeChanged != null)
            {
                FrameTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameTypeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge instance = (CircularGauge)d;
            instance.OnFrameTypeChanged(e);
        }

        /// <summary>
        /// Calls OnRadiusChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge instance = (CircularGauge)d;
            instance.OnRadiusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises RadiusChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRadiusChanged(DependencyPropertyChangedEventArgs e)
        {

            this.RefreshBorders();

            int count = this.ChildrenCollection.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.ChildrenCollection[i] is CircularScale)
                {
                    CircularScale scale = this.ChildrenCollection[i] as CircularScale;
                    //if (scale.Radius < this.Radius)
                    //{
                    //    scale.RadiusRatio = scale.Radius / this.Radius;
                    //}
                    //else 
                    //{
                    //    scale.Radius = this.Radius * 0.75;
                    //}
                    if (scale.RadiusRatio != 0)
                    {
                        scale.Radius = this.Radius * scale.RadiusRatio;
                    }
                    else if (scale.Radius < this.Radius)
                    {
                        scale.RadiusRatio = scale.Radius / this.Radius;
                    }
                }
            }

            if (GaugeAdorner != null)
            {
                (GaugeAdorner as CircularGaugeAdorner).Radius = this.Radius;
            }

            if (RadiusChanged != null)
            {
                RadiusChanged(this, e);
            }
        }
        #endregion Implementation
    }
}
