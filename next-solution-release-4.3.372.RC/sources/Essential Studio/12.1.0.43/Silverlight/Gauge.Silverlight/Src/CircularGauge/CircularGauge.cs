#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Markup;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using System.Windows.Data;

    /// <summary>
    /// 	<para>Represents the CircularGauge UI element. </para>
    /// 	<para></para>
    /// 	<para>The circular gauges are perfect for presenting and analyzing values from a specific range. It can be used to create sophisticated dashboards, clocks or some industrial or medical equipment.</para>
    /// </summary>
    /// <remarks>
    /// 	<para>A circular gauge can have multiple scales, pointers, ranges, state indicators, labels and images. It has a bounded rectangle around it. The top-left corner of the rectangle has coordinates of (0, 0) and the bottom-right corner has coordinates of (100, 100) and every gauge element is drawn inside the bounded rectangle according to the location property.</para>
    /// 	<para></para>
    /// 	<para><img src="Silverlight-Gauge.png"/></para>
    /// 	<para>
    /// 	</para>
    /// </remarks>
    /// <example>
    /// 	<para>The following example shows how to create a <see cref="CircularGauge">CircularGauge</see> in C#.</para>
    /// 	<para></para>
    /// 	<para></para>
    /// 	<para></para>
    /// 	<list type="table">
    /// 		<listheader>
    /// 			<term>C# :</term></listheader>
    /// 		<item>
    /// 			<description>using Syncfusion.Windows.GaugeSilverlight;
    /// <para></para>
    /// 				<para>     namespace CircularGaugeSample </para>
    /// 				<para></para>
    /// 				<para>     { </para>
    /// 				<para></para>
    /// 				<para>         public partial class Window1 : Window </para>
    /// 				<para></para>
    /// 				<para>         { </para>
    /// 				<para></para>
    /// 				<para>            private CircularScale mscale; </para>
    /// 				<para></para>
    /// 				<para>            private StateIndicator mindicator; </para>
    /// 				<para></para>
    /// 				<para>            public Window1() </para>
    /// 				<para></para>
    /// 				<para>            { </para>
    /// 				<para></para>
    /// 				<para>                 InitializeComponent(); </para>
    /// 				<para></para>
    /// 				<para>                 mscale = new CircularScale(); </para>
    /// 				<para></para>
    /// 				<para>                 mscale.ShadowOffset = 1; </para>
    /// 				<para></para>
    /// 				<para>                 mscale.Minimum = 0; </para>
    /// 				<para></para>
    /// 				<para>                 mscale.Maximum = 100; </para>
    /// 				<para></para>
    /// 				<para>                 mscale.MinorIntervalValue = 2; </para>
    /// 				<para></para>
    /// 				<para>                 mscale.MajorIntervalValue = 10; </para>
    /// 				<para></para>
    /// 				<para>                 mscale.StartAngle = 120; </para>
    /// 				<para></para>
    /// 				<para>                 mscale.GapSweepAngle = 300; </para>
    /// 				<para></para>
    /// 				<para>                 mscale.ScaleBarSize = 1.5; </para>
    /// 				<para></para>
    /// 				<para>                 mscale.Radius = 116; </para>
    /// 				<para></para>
    /// 				<para>                 this.circularGauge1.Scales.Add( mscale ); </para>
    /// 				<para></para>
    /// 				<para>                 LabelTickSet majorLabelTick = new LabelTickSet(); </para>
    /// 				<para></para>
    /// 				<para>                 majorLabelTick.FontSize = 11; </para>
    /// 				<para></para>
    /// 				<para>                 majorLabelTick.TickStyle = TickStyle.MajorTick; </para>
    /// 				<para></para>
    /// 				<para>                 majorLabelTick.TickPlacement = ScalePlacement.Inside; </para>
    /// 				<para></para>
    /// 				<para>                 majorLabelTick.DistanceFromScale = 5; </para>
    /// 				<para></para>
    /// 				<para>                 MarkTickSet majorTick = new MarkTickSet(); </para>
    /// 				<para></para>
    /// 				<para>                 majorTick.TickWidth = 4; </para>
    /// 				<para></para>
    /// 				<para>                 majorTick.TickHeight = 9; </para>
    /// 				<para></para>
    /// 				<para>                 majorTick.TickStyle = TickStyle.MajorTick; </para>
    /// 				<para></para>
    /// 				<para>                 majorTick.Background = new SolidColorBrush( Color.FromRgb(                  255, 0, 59, 137 ) ); </para>
    /// 				<para></para>
    /// 				<para>                 majorTick.TickShape = TickShape.Ellipse; </para>
    /// 				<para></para>
    /// 				<para>                 MarkTickSet minorTick = new MarkTickSet(); </para>
    /// 				<para></para>
    /// 				<para>                 minorTick.TickWidth = 1; </para>
    /// 				<para></para>
    /// 				<para>                 minorTick.TickHeight = 4; </para>
    /// 				<para></para>
    /// 				<para>                 minorTick.TickStyle = TickStyle.MinorTick; </para>
    /// 				<para></para>
    /// 				<para>                 minorTick.Background = new SolidColorBrush( Color.FromRgb(                  255, 0, 59, 137 ) ); </para>
    /// 				<para></para>
    /// 				<para>                 mscale.Ticks.Add( minorTick ); </para>
    /// 				<para></para>
    /// 				<para>                 mscale.Ticks.Add( majorTick ); </para>
    /// 				<para></para>
    /// 				<para>                 mscale.Ticks.Add( majorLabelTick ); </para>
    /// 				<para></para>
    /// 				<para>                 mscale.PointerCap.PointerCapRadius = 15; </para>
    /// 				<para></para>
    /// 				<para>                 mscale.PointerCap.Background = new RadialGradientBrush( Color.FromArgb( 255, 194, 207, 229 ), Color.FromArgb( 255, 46, 94, 160 ) ); </para>
    /// 				<para></para>
    /// 				<para>                 CircularPointer pointer1 = new CircularPointer(); </para>
    /// 				<para></para>
    /// 				<para>                 pointer1.Background = new SolidColorBrush( Color.FromArgb( 255, 194, 207, 229 ) ); </para>
    /// 				<para></para>
    /// 				<para>                 pointer1.PointerPlacement = ScalePlacement.Outside; </para>
    /// 				<para></para>
    /// 				<para>                 pointer1.PointerLength = 100; </para>
    /// 				<para></para>
    /// 				<para>                 pointer1.PointerWidth = 15; </para>
    /// 				<para></para>
    /// 				<para>                 mscale.Pointers.Add( pointer1 ); </para>
    /// 				<para></para>
    /// 				<para>                mindicator = new StateIndicator(); </para>
    /// 				<para></para>
    /// 				<para>                 mindicator.StateRanges.Add( new StateRange( 10, 20 ) ); </para>
    /// 				<para></para>
    /// 				<para>                 mindicator.StateRanges.Add( new StateRange( 50, 60 ) ); </para>
    /// 				<para></para>
    /// 				<para>                 mindicator.Background = new RadialGradientBrush( Colors.White, Colors.Green ); </para>
    /// 				<para></para>
    /// 				<para>                 mindicator.ActiveBackgroundBrush = new RadialGradientBrush( Colors.White, Colors.Red ); </para>
    /// 				<para></para>
    /// 				<para>                 mindicator.IndicatorWidth = 20; </para>
    /// 				<para></para>
    /// 				<para>                 mindicator.IndicatorHeight = 20; </para>
    /// 				<para></para>
    /// 				<para>                 mindicator.Location = new Point( 50, 80 ); </para>
    /// 				<para></para>
    /// 				<para>                 circularGauge1.StateIndicators.Add( mindicator ); </para>
    /// 				<para></para>
    /// 				<para>             } </para>
    /// 				<para></para>
    /// 				<para>         } </para>
    /// 				<para></para>
    /// 				<para>     }</para></description></item></list>
    /// 	<list type="table">
    /// 		<listheader>
    /// 			<term>Xaml :</term></listheader>
    /// 		<item>
    /// 			<description>xmlns:syncfusion="clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight"
    /// <para></para>
    /// 				<para>&lt;syncfusion:CircularGauge Radius="120" Background="Gray" Name="gauge" &gt;</para>
    /// 				<para>                        &lt;syncfusion:CircularGauge.Scales&gt;</para>
    /// 				<para>                            &lt;syncfusion:CircularScale x:Name="m_scale" </para>
    /// 				<para>                                        Radius = "100" </para>
    /// 				<para>                                        Minimum="0" </para>
    /// 				<para>                                        Maximum="100"</para>
    /// 				<para>                                        MinorIntervalValue="2" </para>
    /// 				<para> MajorIntervalValue="10"</para>
    /// 				<para>                                        StartAngle="120" </para>
    /// 				<para> GapSweepAngle="300"</para>
    /// 				<para>                                        ShadowOffset="3"</para>
    /// 				<para>
    /// 				</para>
    /// 				<para> Background="Silver"</para>
    /// 				<para>                                        ScaleBarSize = "2" &gt;</para>
    /// 				<para> &lt;syncfusion:CircularScale.Ticks&gt;</para>
    /// 				<para>                                    &lt;syncfusion:LabelTickSet FontSize="13" Background="Silver" Name="m_label" TickPlacement="Inside" VerticalContentAlignment="Bottom" DistanceFromScale="10" </para>
    /// 				<para>                                                    /&gt;</para>
    /// 				<para>                                    &lt;syncfusion:MarkTickSet TickHeight="8" TickShape="Triangle" TickStyle="MajorTick" Name="majorLabelTick" </para>
    /// 				<para> TickWidth="4" Background="Silver" TickPlacement="Inside"/&gt;</para>
    /// 				<para>                                    &lt;syncfusion:MarkTickSet TickHeight="4" TickWidth="1" TickStyle="MinorTick" TickShape="Triangle" Name="minorTick" </para>
    /// 				<para> Background="Silver" TickPlacement="Inside"/&gt;</para>
    /// 				<para> &lt;/syncfusion:CircularScale.Ticks&gt;</para>
    /// 				<para> &lt;syncfusion:CircularScale.Ranges&gt;</para>
    /// 				<para>                                    &lt;syncfusion:CircularRange StartValue="70" EndValue="100" Name="range" </para>
    /// 				<para>                                                StartWidth="0" EndWidth="15"  </para>
    /// 				<para> RangePosition="Inside" DistanceFromScale="35" Background="Red"&gt;</para>
    /// 				<para> &lt;/syncfusion:CircularRange&gt;</para>
    /// 				<para> &lt;/syncfusion:CircularScale.Ranges&gt;</para>
    /// 				<para> &lt;syncfusion:CircularScale.Pointers&gt;</para>
    /// 				<para>                                    &lt;syncfusion:CircularPointer Background="Blue" NeedleStyle="Triangle" MarkerStyle="Diamond" Name="m_pointer1"</para>
    /// 				<para> BorderBrush="Silver" </para>
    /// 				<para> PointerLength="100" </para>
    /// 				<para> PointerWidth="20" </para>
    /// 				<para> PointerNeedleType="Needle" </para>
    /// 				<para> PointerPlacement="Inside"/&gt;</para>
    /// 				<para> &lt;/syncfusion:CircularScale.Pointers&gt;</para>
    /// 				<para></para>
    /// 				<para> &lt;syncfusion:CircularScale.PointerCap&gt;</para>
    /// 				<para>                                    &lt;syncfusion:PointerCap</para>
    /// 				<para>
    /// 				</para>
    /// 				<para> PointerCapRadius="15" Background="Black" &gt;</para>
    /// 				<para>                                    &lt;/syncfusion:PointerCap&gt;</para>
    /// 				<para> &lt;/syncfusion:CircularScale.PointerCap&gt;</para>
    /// 				<para></para>
    /// 				<para></para>
    /// 				<para>                            &lt;/syncfusion:CircularScale&gt;</para>
    /// 				<para>                        &lt;/syncfusion:CircularGauge.Scales&gt;</para>
    /// 				<para> &lt;syncfusion:CircularGauge.StateIndicators&gt;</para>
    /// 				<para>                            &lt;syncfusion:StateIndicator Name="m_indicator"   Location="55,80" IndicatorHeight="8" IndicatorWidth="8"</para>
    /// 				<para>FontSize="12" FontFamily="Verdana" IndicatorStyle="CircularLED" Text="Normal" Background="Green" ActiveBackgroundBrush="Red" ActiveText="High"  &gt;</para>
    /// 				<para> &lt;syncfusion:StateIndicator.StateRanges&gt;</para>
    /// 				<para>                                    &lt;syncfusion:StateRange StartValue="70" EndValue="100" /&gt;</para>
    /// 				<para> &lt;/syncfusion:StateIndicator.StateRanges&gt;</para>
    /// 				<para>                            &lt;/syncfusion:StateIndicator&gt;</para>
    /// 				<para> &lt;/syncfusion:CircularGauge.StateIndicators&gt;</para>
    /// 				<para>                    &lt;/syncfusion:CircularGauge&gt; </para></description></item></list>
    /// </example>    
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
    //     Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Theming.Blend;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
    //    Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
    //    Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
    //    Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
    //    Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Theming.Default;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
    //    Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Theming.Office2003;component/Gauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
    //   Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Theming.VS2010;component/CircularGauge.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
    //  Type = typeof(CircularGauge), XamlResource = "/Syncfusion.Theming.Metro;component/Gauge.xaml")]
    [ContentProperty("Scales")]
    public class CircularGauge : GaugeBase
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularGauge.FrameType">FrameType</see> dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Gauge can be customized into <see cref="F:Syncfusion.Windows.Gauge.GaugeFrameType">FullCircle</see> or <see cref="F:Syncfusion.Windows.Gauge.GaugeFrameType">HalfCircle</see> frame type.</para>
        /// </remarks>
        /// <returns>
        /// <para>Type: <see cref="P:Syncfusion.Windows.Gauge.CircularGauge.FrameType">GaugeFrameType</see></para>
        /// </returns>
        public static readonly DependencyProperty FrameTypeProperty =
            DependencyProperty.Register("FrameType", typeof(GaugeFrameType), typeof(CircularGauge), new PropertyMetadata(GaugeFrameType.Circular, new PropertyChangedCallback(OnFrameTypeChanged)));
        /// <summary>
        /// Identifies the <see cref="GaugeFrameDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GaugeFrameDirectionProperty =
            DependencyProperty.Register("GaugeFrameDirection", typeof(FrameDirection), typeof(CircularGauge), new PropertyMetadata(FrameDirection.NorthEast, new PropertyChangedCallback(OnGaugeFrameDirectionChanged)));

        /// <summary>
        /// Identifies the <see
        /// cref="P:Syncfusion.Windows.Gauge.CircularGauge.Radius">Radius </see>
        /// dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The default value for the gauge radius is 150d.</para>
        /// </remarks>
        /// <returns>
        /// <para>Type:<see cref="T:System.Double">Syetem.Double</see></para>
        /// </returns>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(CircularGauge), new PropertyMetadata(150d, new PropertyChangedCallback(OnRadiusChanged)));
        /// <summary>
        /// Identifies the <see cref="Scales"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScalesProperty =
         DependencyProperty.Register("Scales", typeof(ScaleCollection), typeof(CircularGauge), new PropertyMetadata(new ScaleCollection()));
        /// <summary>
        /// Identifies the <see cref="ScalePanel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScalePanelProperty =
        DependencyProperty.Register("ScalePanel", typeof(ScalesLayoutPanel), typeof(CircularGauge), new PropertyMetadata(null));


        #endregion

        #region CLR properties

        /// <summary>
        /// Second frame.
        /// </summary>
        private Ellipse msecondCircleBorder;

        /// <summary>
        /// Inner frame.
        /// </summary>
        private Ellipse minnerCircleBorder;

        /// <summary>
        /// First frame.
        /// </summary>
        private Ellipse mfirstCircleBorder;

        /// <summary>
        /// middle frame.
        /// </summary>
        private Ellipse mmiddleCircleBorder;

        private Path mainglasspath;
        private Path PART_SecondGlassPath;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Gauge.CircularGauge">CircularGauge</see> class.
        /// </summary>
        /// <remarks>
        /// <para>Circular Gauge is the main base class for Gauge.This class is invoked
        /// whenever the Gauge is loaded and also if its Size is changed.</para>
        /// </remarks>
        public CircularGauge()
        {
            DefaultStyleKey = typeof(CircularGauge);
            this.Scales = new ScaleCollection();
            this.Scales.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
            this.Loaded += new RoutedEventHandler(this.CircularGaugeLoaded);
            this.SizeChanged += new SizeChangedEventHandler(this.CircularGaugeSizeChanged);
        }            

        /// <summary>
        /// Static constructor.
        /// </summary>
        static CircularGauge()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see
        /// cref="F:Syncfusion.Windows.Gauge.CircularGauge.FrameTypeProperty">FrameTypeProperty</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback FrameTypeChanged;
        /// <summary>
        /// Event that is raised when GaugeFrameDirectionChanged
        /// </summary>
        public event PropertyChangedCallback GaugeFrameDirectionChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="F:Syncfusion.Windows.Gauge.CircularGauge.RadiusProperty">RadiusProperty</see>
        /// is changed.
        /// </summary>
        public event PropertyChangedCallback RadiusChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the CircularGauge's Frametype as <see cref="F:Syncfusion.Windows.Gauge.GaugeFrameType.FullCircle">FullCircle</see> or <see cref="F:Syncfusion.Windows.Gauge.GaugeFrameType.HalfCircle">HalfCircle</see>. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Gauge can be customized into Half circle or Full circle Frame Type.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: <see cref="T:Syncfusion.Windows.Gauge.GaugeFrameType">GaugeFrameType</see></para>
        /// </value>
        /// <seealso cref="GaugeFrameType">GaugeFrameType</seealso>
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
        /// Gets or sets the FrameDirection
        /// </summary>
        public FrameDirection GaugeFrameDirection
        {
            get
            {
                return (FrameDirection)GetValue(GaugeFrameDirectionProperty);
            }

            set
            {
                SetValue(GaugeFrameDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of radius of the CircularGauge. This is dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is 150.</para>
        /// </remarks>
        /// <value>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// <para>  </para>
        /// </value>
        /// <seealso cref="double">double</seealso>
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

        /// <summary>
        /// Gets or sets the Collection of circular scales.
        /// </summary>
        /// <value>
        /// Type: <see cref="ScaleCollection"/>
        /// </value>
        /// <seealso cref="ScaleCollection"/>
        public ScaleCollection Scales
        {
            get
            {
                return (ScaleCollection)GetValue(ScalesProperty);
            }

            set
            {
                SetValue(ScalesProperty,value);
            }
        }

        /// <summary>
        /// Gets or sets the layout internally
        /// </summary>
        /// <value>
        /// Type: <see cref="ScalesLayoutPanel"/>
        /// </value>
        /// <seealso cref="ScalesLayoutPanel"/>
        internal ScalesLayoutPanel ScalePanel
        {
            get
            {
                return (ScalesLayoutPanel) GetValue(ScalePanelProperty);
            }

            set
            {
                SetValue(ScalePanelProperty,value);
            }
        }

        /// <summary>
        /// Gets the Second Circle Border
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        private new  Ellipse SecondCircleBorder
        {
            get
            {
                return this.msecondCircleBorder;
            }
        }

        /// <summary>
        /// Gets First CircleBorder
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        private new  Ellipse FirstCircleBorder
        {
            get
            {
                return this.mfirstCircleBorder;
            }
        }

        /// <summary>
        /// Gets First CircleBorder
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        private new  Ellipse MiddleCircleBorder
        {
            get
            {
                return this.mmiddleCircleBorder;
            }
        }

        /// <summary>
        /// Gets Inner circle border
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        private new  Ellipse InnerCircleBorder
        {
            get
            {
                return this.minnerCircleBorder;
            }
        }

        #endregion

        #region Overrides
        private ItemsControl ScaleItems;

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mfirstCircleBorder = this.GetTemplateChild("PART_FirstBorder") as Ellipse;
            this.msecondCircleBorder = this.GetTemplateChild("PART_SecondBorder") as Ellipse;
            this.minnerCircleBorder = this.GetTemplateChild("PART_InnerBorder") as Ellipse;
            this.mmiddleCircleBorder = this.GetTemplateChild("PART_MiddleBorder") as Ellipse;
            this.mainglasspath = this.GetTemplateChild("PART_GlassPath") as Path;
            this.PART_SecondGlassPath = this.GetTemplateChild("PART_SecondPath") as Path;
            this.ScalePanel = this.GetTemplateChild("PART_ScalesPanel") as ScalesLayoutPanel;
            this.ScaleItems = this.GetTemplateChild("PART_Scales") as ItemsControl;
            ContentPresenter presenter = this.GetTemplateChild("PART_DigitalDisplay") as ContentPresenter;
            foreach (CircularScale scale in this.Scales)
            {
              //  this.ScaleItems.Items.Add(scale);
            }
            if (presenter != null && this.Scales.Count>0)
            {
                Binding heightbinding = new Binding();
                heightbinding.Converter = new RadiusToHeightConverter();
                heightbinding.Source = this.Scales[0];
                heightbinding.Path = new PropertyPath("Radius");
                presenter.SetBinding(CircularScale.HeightProperty, heightbinding);

                Binding widthbinding = new Binding();
                widthbinding.Converter = new RadiusToHeightConverter();
                widthbinding.Source = this.Scales[0];
                widthbinding.Path = new PropertyPath("Radius");
                presenter.SetBinding(CircularScale.WidthProperty, widthbinding);

                Binding visibilityBinding = new Binding();
                visibilityBinding.Converter = new BooleanToVisibilityConverter();
                visibilityBinding.Source = this;
                visibilityBinding.Mode = BindingMode.OneWay;
                visibilityBinding.Path = new PropertyPath("ShowDigitalValue");
                presenter.SetBinding(VisibilityProperty, visibilityBinding);

            }
            if (this.ScalePanel != null)
            {
                this.ChildrenCollection.VisualParent = this.ScalePanel;
                this.ChildrenCollection.UpdatePanelChildren();
          
            }
            if (mainglasspath != null)
            {
                Binding b = new Binding();
                b.Converter = new BooleanToVisibilityConverter();
                b.Source = this;
                b.Mode = BindingMode.OneWay;
                b.Path = new PropertyPath("EnableEffects");
                mainglasspath.SetBinding(VisibilityProperty, b);
            }
            
            this.UpdateVisualState(true, this.FrameType);
            this.UpdateVisualStyle();
            this.RefreshBorders();
            
        }

     

        /// <summary>
        /// It refreshes the scale panel.
        /// </summary>
        internal override void RefreshScalesPanel()
        {
            if (this.ScalePanel != null)
            {
                this.ScalePanel.InvalidateMeasure();
                this.ScalePanel.InvalidateArrange();
            }
        }

        /// <summary>
        /// Assigns the Measure for the guage.
        /// </summary>
        /// <param name="availableSize">The width and height values </param>
        /// <returns>Size property </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            return new Size(this.Radius * 2, this.Radius * 2);
        }

        /// <summary>
        /// Calls EnableEffectsChanged method of the instance, notifies of the dependency
        /// property value change.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected override void IsEnableEffectsChanged(DependencyPropertyChangedEventArgs e)
        {
            base.IsEnableEffectsChanged(e);
            if (this.Scales.Count > 0)
            {
                foreach (CircularScale scale in this.Scales)
                {
                    scale.PointerCap.RefreshPointerCap();
                }
            }
        }

        /// <summary>
        /// Updates CircularGaugeFrame Borders.
        /// </summary>
        protected override void RefreshBorders()
        {
            if (this.FirstCircleBorder != null)
            {
                this.FirstCircleBorder.Height = this.Radius * 2;
                this.FirstCircleBorder.Width = this.Radius * 2;
                if (this.OuterFrameBrush != null)
                {
                    this.FirstCircleBorder.Fill = this.OuterFrameBrush;
                }
                else
                {
                    this.FirstCircleBorder.Fill = this.Background;
                }

                this.FirstCircleBorder.OpacityMask = this.OpacityMask;
                this.FirstCircleBorder.Opacity = this.Opacity;
                if (this.FrameType == GaugeFrameType.Circular)
                {
                    EllipseGeometry geo = new EllipseGeometry();
                    geo.RadiusX = this.Radius * 4;
                    geo.RadiusY = this.Radius * 4;
                    this.FirstCircleBorder.Clip = geo;
                }
                else if (this.FrameType == GaugeFrameType.QuarterCircular)
                {
                    double pathwidth = this.Radius + 3 * (this.InnerFrameOffset + this.MiddleFrameOffset + this.OuterFrameOffset);
                    double decleng = 6 * (this.InnerFrameOffset + this.MiddleFrameOffset + this.OuterFrameOffset);
                    PathGeometry pathgeo = new PathGeometry();
                    PathFigure pathfig = new PathFigure();
                    pathfig.StartPoint = new Point(pathwidth - decleng, 20);

                    ArcSegment arc = new ArcSegment();
                    //arc.IsLargeArc = true;
                    arc.Point = new Point(pathwidth - decleng + 20, 0);
                    arc.Size = new Size(20, 20);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(FirstCircleBorder.Height, pathwidth - 20);
                    arc.Size = new Size((pathwidth), pathwidth);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    arc = new ArcSegment();
                    //arc.IsLargeArc = true;
                    arc.Point = new Point(FirstCircleBorder.Height - 20, pathwidth);
                    arc.Size = new Size(20, 20);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);



                    LineSegment line = new LineSegment();
                    line.Point = new Point(pathwidth - decleng / 2, (pathwidth));
                    pathfig.Segments.Add(line);

                    arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(pathwidth - decleng, pathwidth - decleng / 2);
                    arc.RotationAngle = 180;
                    arc.Size = new Size(pathwidth / 15, pathwidth / 15);
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    line = new LineSegment();
                    line.Point = new Point(pathwidth - decleng, 0);
                    pathfig.Segments.Add(line);


                    pathfig.IsClosed = true;
                    pathgeo.Figures.Add(pathfig);

                    this.FirstCircleBorder.Clip = pathgeo;
                }
                else if (this.FrameType == GaugeFrameType.SemiCircular)
                {
                    double pathwidth = this.Radius + 2 * (this.InnerFrameOffset + this.MiddleFrameOffset + this.OuterFrameOffset);
                    PathGeometry pathgeo = new PathGeometry();
                    PathFigure pathfig = new PathFigure();
                    pathfig.StartPoint = new Point(0 + 30, pathwidth);

                    BezierSegment curve = new BezierSegment();
                    curve.Point1 = new Point(0 + 25, pathwidth);
                    curve.Point2 = new Point(0, pathwidth);
                    curve.Point3 = new Point(0, pathwidth - 25);
                    pathfig.Segments.Add(curve);

                    ArcSegment arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(this.FirstCircleBorder.Width, pathwidth - 25);
                    arc.Size = new Size((pathwidth * 2), pathwidth);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    curve = new BezierSegment();
                    curve.Point1 = new Point(this.FirstCircleBorder.Width, pathwidth - 25);
                    curve.Point2 = new Point(this.FirstCircleBorder.Width, pathwidth);
                    curve.Point3 = new Point(this.FirstCircleBorder.Width - 30, pathwidth);
                    pathfig.Segments.Add(curve);


                    LineSegment line = new LineSegment();
                    line.Point = new Point(3 * FirstCircleBorder.Height / 5, (pathwidth));
                    pathfig.Segments.Add(line);

                    arc = new ArcSegment();
                    arc.Point = new Point(2 * FirstCircleBorder.Height / 5, (pathwidth));
                    arc.RotationAngle = 180;
                    arc.Size = new Size(FirstCircleBorder.Height / 5, 1.5 * FirstCircleBorder.Height / 5);
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    line = new LineSegment();
                    line.Point = new Point(0, (pathwidth));
                    pathfig.Segments.Add(line);

                    pathfig.IsClosed = true;
                    pathgeo.Figures.Add(pathfig);

                    this.FirstCircleBorder.Clip = pathgeo;
                }
            }

            if (this.MiddleCircleBorder != null)
            {
                this.MiddleCircleBorder.Height = (this.Radius - this.OuterFrameOffset) * 2;
                this.MiddleCircleBorder.Width = (this.Radius - this.OuterFrameOffset) * 2;
                if (this.OuterFrameBrush != null)
                {
                    this.MiddleCircleBorder.Fill = this.MiddleFrameBrush;
                }
                else
                {
                    this.MiddleCircleBorder.Fill = this.Background;
                }

                this.MiddleCircleBorder.OpacityMask = this.OpacityMask;
                this.MiddleCircleBorder.Opacity = this.Opacity;
                if (this.FrameType == GaugeFrameType.Circular)
                {
                    EllipseGeometry geo = new EllipseGeometry();
                    geo.RadiusX = (this.Radius - this.OuterFrameOffset) * 4;
                    geo.RadiusY = (this.Radius - this.OuterFrameOffset) * 4;
                    this.minnerCircleBorder.Clip = geo;
                }
                else if (this.FrameType == GaugeFrameType.QuarterCircular)
                {
                    double pathwidth = this.Radius + 2 * (this.InnerFrameOffset + this.MiddleFrameOffset) + (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset);
                    double decleng = 6 * (this.InnerFrameOffset + this.MiddleFrameOffset) + 4 * this.OuterFrameOffset;

                    PathGeometry pathgeo = new PathGeometry();
                    PathFigure pathfig = new PathFigure();
                    pathfig.StartPoint = new Point(pathwidth - decleng, 20);

                    ArcSegment arc = new ArcSegment();
                    //arc.IsLargeArc = true;
                    arc.Point = new Point(pathwidth - decleng + 20, 0);
                    arc.Size = new Size(20, 20);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(MiddleCircleBorder.Height, pathwidth - 20);
                    arc.Size = new Size((pathwidth), pathwidth);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    arc = new ArcSegment();
                    //arc.IsLargeArc = true;
                    arc.Point = new Point(MiddleCircleBorder.Height - 20, pathwidth);
                    arc.Size = new Size(20, 20);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);


                    LineSegment line = new LineSegment();
                    line.Point = new Point(pathwidth - decleng / 2, (pathwidth));
                    pathfig.Segments.Add(line);

                    arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(pathwidth - decleng, pathwidth - decleng / 2);
                    arc.RotationAngle = 180;
                    arc.Size = new Size(pathwidth / 15, pathwidth / 15);
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    line = new LineSegment();
                    line.Point = new Point(pathwidth - decleng, 0);
                    pathfig.Segments.Add(line);


                    pathfig.IsClosed = true;
                    pathgeo.Figures.Add(pathfig);


                    this.MiddleCircleBorder.Clip = pathgeo;
                }
                else if (this.FrameType == GaugeFrameType.SemiCircular)
                {
                    double pathwidth = this.Radius + 2 * (this.InnerFrameOffset + this.MiddleFrameOffset);
                    PathGeometry pathgeo = new PathGeometry();
                    PathFigure pathfig = new PathFigure();
                    pathfig.StartPoint = new Point(0 + 25, pathwidth);

                    BezierSegment curve = new BezierSegment();
                    curve.Point1 = new Point(0 + 25, pathwidth);
                    curve.Point2 = new Point(0, pathwidth);
                    curve.Point3 = new Point(0, pathwidth - 25);
                    pathfig.Segments.Add(curve);

                    ArcSegment arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(this.MiddleCircleBorder.Width, pathwidth - 25);
                    arc.Size = new Size(pathwidth * 2, pathwidth);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    curve = new BezierSegment();
                    curve.Point1 = new Point(this.MiddleCircleBorder.Width, pathwidth - 20);
                    curve.Point2 = new Point(this.MiddleCircleBorder.Width, pathwidth);
                    curve.Point3 = new Point(this.MiddleCircleBorder.Width - 25, pathwidth);
                    pathfig.Segments.Add(curve);


                    LineSegment line = new LineSegment();
                    line.Point = new Point(3 * MiddleCircleBorder.Height / 5, pathwidth);
                    pathfig.Segments.Add(line);

                    arc = new ArcSegment();
                    arc.Point = new Point(2 * MiddleCircleBorder.Height / 5, pathwidth);
                    arc.RotationAngle = 180;
                    arc.Size = new Size(MiddleCircleBorder.Height / 5, 1.5 * MiddleCircleBorder.Height / 5);
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);


                    line = new LineSegment();
                    line.Point = new Point(0, pathwidth);
                    pathfig.Segments.Add(line);

                    pathfig.IsClosed = true;
                    pathgeo.Figures.Add(pathfig);

                    this.MiddleCircleBorder.Clip = pathgeo;
                }
            }

            if (this.SecondCircleBorder != null)
            {
                this.SecondCircleBorder.Height = (this.Radius - (this.MiddleFrameOffset + this.OuterFrameOffset)) * 2;
                this.SecondCircleBorder.Width = (this.Radius - (this.MiddleFrameOffset + this.OuterFrameOffset)) * 2;
                if (this.InnerFrameBrush != null)
                {
                    this.SecondCircleBorder.Fill = this.InnerFrameBrush;
                }
                else
                {
                    this.SecondCircleBorder.Fill = this.Background;
                }

                this.SecondCircleBorder.OpacityMask = this.OpacityMask;
                this.SecondCircleBorder.Opacity = this.Opacity;
                if (this.FrameType == GaugeFrameType.Circular)
                {
                    EllipseGeometry geo = new EllipseGeometry();
                    geo.RadiusX = (this.Radius - (this.MiddleFrameOffset + this.OuterFrameOffset)) * 4;
                    geo.RadiusY = (this.Radius - (this.MiddleFrameOffset + this.OuterFrameOffset)) * 4;
                    this.SecondCircleBorder.Clip = geo;
                }
                else if (this.FrameType == GaugeFrameType.QuarterCircular)
                {
                    double pathwidth = this.Radius + 2 * this.InnerFrameOffset + (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset);
                    double decleng = 6 * (this.InnerFrameOffset) + 4 * (this.MiddleFrameOffset + this.OuterFrameOffset);

                    PathGeometry pathgeo = new PathGeometry();
                    PathFigure pathfig = new PathFigure();
                    pathfig.StartPoint = new Point(pathwidth - decleng, 20);

                    ArcSegment arc = new ArcSegment();
                    //arc.IsLargeArc = true;
                    arc.Point = new Point(pathwidth - decleng + 20, 0);
                    arc.Size = new Size(20, 20);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(SecondCircleBorder.Height, pathwidth - 20);
                    arc.Size = new Size((pathwidth), pathwidth);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    arc = new ArcSegment();
                    //arc.IsLargeArc = true;
                    arc.Point = new Point(SecondCircleBorder.Height - 20, pathwidth);
                    arc.Size = new Size(20, 20);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    LineSegment line = new LineSegment();
                    line.Point = new Point(pathwidth - decleng / 2, (pathwidth));
                    pathfig.Segments.Add(line);

                    arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(pathwidth - decleng, pathwidth - decleng / 2);
                    arc.RotationAngle = 180;
                    arc.Size = new Size(pathwidth / 15, pathwidth / 15);
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    line = new LineSegment();
                    line.Point = new Point(pathwidth - decleng, 0);
                    pathfig.Segments.Add(line);
                    pathfig.IsClosed = true;
                    pathgeo.Figures.Add(pathfig);

                    this.SecondCircleBorder.Clip = pathgeo;
                }
                else if (this.FrameType == GaugeFrameType.SemiCircular)
                {
                    double pathwidth = this.Radius + 2 * this.InnerFrameOffset;
                    PathGeometry pathgeo = new PathGeometry();
                    PathFigure pathfig = new PathFigure();
                    pathfig.StartPoint = new Point(0 + 25, pathwidth);

                    BezierSegment curve = new BezierSegment();
                    curve.Point1 = new Point(0 + 25, pathwidth);
                    curve.Point2 = new Point(0, pathwidth);
                    curve.Point3 = new Point(0, pathwidth - 25);
                    pathfig.Segments.Add(curve);

                    ArcSegment arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(this.SecondCircleBorder.Width, pathwidth - 25);
                    arc.Size = new Size(pathwidth * 2, pathwidth);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    curve = new BezierSegment();
                    curve.Point1 = new Point(this.SecondCircleBorder.Width, pathwidth - 20);
                    curve.Point2 = new Point(this.SecondCircleBorder.Width, pathwidth);
                    curve.Point3 = new Point(this.SecondCircleBorder.Width - 25, pathwidth);
                    pathfig.Segments.Add(curve);


                    LineSegment line = new LineSegment();
                    line.Point = new Point(3 * SecondCircleBorder.Height / 5, pathwidth);
                    pathfig.Segments.Add(line);

                    arc = new ArcSegment();
                    arc.Point = new Point(2 * SecondCircleBorder.Height / 5, pathwidth);
                    arc.RotationAngle = 180;
                    arc.Size = new Size(SecondCircleBorder.Height / 5, 1.5 * SecondCircleBorder.Height / 5);
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    line = new LineSegment();
                    line.Point = new Point(0, pathwidth);
                    pathfig.Segments.Add(line);

                    pathfig.IsClosed = true;
                    pathgeo.Figures.Add(pathfig);

                    this.SecondCircleBorder.Clip = pathgeo;
                }

            }

            if (this.InnerCircleBorder != null)
            {
                this.InnerCircleBorder.Height = (this.Radius - (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset)) * 2;
                this.InnerCircleBorder.Width = (this.Radius - (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset)) * 2;
                // this.InnerCircleBorder.Fill = this.Background;
                this.InnerCircleBorder.OpacityMask = this.OpacityMask;
                this.InnerCircleBorder.Opacity = this.Opacity;

                if (this.FrameType == GaugeFrameType.Circular)
                {
                    EllipseGeometry geo = new EllipseGeometry();
                    geo.RadiusX = ((this.Radius) - (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset)) * 4;
                    geo.RadiusY = ((this.Radius) - (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset)) * 4;
                    this.InnerCircleBorder.Clip = geo;
                }
                else if (this.FrameType == GaugeFrameType.QuarterCircular)
                {
                    double pathwidth = (this.Radius) + (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset);
                    double decleng = 4 * (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset);
                    PathGeometry pathgeo = new PathGeometry();
                    PathFigure pathfig = new PathFigure();
                    pathfig.StartPoint = new Point(pathwidth - decleng, 20);

                    ArcSegment arc = new ArcSegment();
                    //arc.IsLargeArc = true;
                    arc.Point = new Point(pathwidth - decleng + 20, 0);
                    arc.Size = new Size(20, 20);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(InnerCircleBorder.Height, pathwidth - 20);
                    arc.Size = new Size((pathwidth), pathwidth);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    arc = new ArcSegment();
                    //arc.IsLargeArc = true;
                    arc.Point = new Point(InnerCircleBorder.Height - 20, pathwidth);
                    arc.Size = new Size(20, 20);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);


                    LineSegment line = new LineSegment();
                    line.Point = new Point(pathwidth - decleng / 2, (pathwidth));
                    pathfig.Segments.Add(line);

                    arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(pathwidth - decleng, pathwidth - decleng / 2);
                    arc.RotationAngle = 180;
                    arc.Size = new Size(pathwidth / 15, pathwidth / 15);
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    line = new LineSegment();
                    line.Point = new Point(pathwidth - decleng, 0);
                    pathfig.Segments.Add(line);


                    pathfig.IsClosed = true;
                    pathgeo.Figures.Add(pathfig);

                    this.InnerCircleBorder.Clip = pathgeo;
                }
                else if (this.FrameType == GaugeFrameType.SemiCircular)
                {
                    double pathwidth = (this.Radius);
                    PathGeometry pathgeo = new PathGeometry();
                    PathFigure pathfig = new PathFigure();
                    pathfig.StartPoint = new Point(0 + 25, pathwidth);

                    BezierSegment curve = new BezierSegment();
                    curve.Point1 = new Point(0 + 25, pathwidth);
                    curve.Point2 = new Point(0, pathwidth);
                    curve.Point3 = new Point(0, pathwidth - 25);
                    pathfig.Segments.Add(curve);

                    ArcSegment arc = new ArcSegment();
                    arc.IsLargeArc = true;
                    arc.Point = new Point(this.InnerCircleBorder.Width, pathwidth - 25);
                    arc.Size = new Size(pathwidth * 2, pathwidth);
                    arc.RotationAngle = 180;
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);

                    curve = new BezierSegment();
                    curve.Point1 = new Point(this.InnerCircleBorder.Width, pathwidth - 20);
                    curve.Point2 = new Point(this.InnerCircleBorder.Width, pathwidth);
                    curve.Point3 = new Point(this.InnerCircleBorder.Width - 25, pathwidth);
                    pathfig.Segments.Add(curve);

                    LineSegment line = new LineSegment();
                    line.Point = new Point(3 * InnerCircleBorder.Height / 5, pathwidth);
                    pathfig.Segments.Add(line);

                    arc = new ArcSegment();
                    arc.Point = new Point(2 * InnerCircleBorder.Height / 5, pathwidth);
                    arc.RotationAngle = 180;
                    arc.Size = new Size(InnerCircleBorder.Height / 5, 1.5 * InnerCircleBorder.Height / 5);
                    arc.SweepDirection = SweepDirection.Clockwise;
                    pathfig.Segments.Add(arc);


                    line = new LineSegment();
                    line.Point = new Point(0, pathwidth);
                    pathfig.Segments.Add(line);

                    pathfig.IsClosed = true;
                    pathgeo.Figures.Add(pathfig);

                    this.InnerCircleBorder.Clip = pathgeo;
                }
            }

            if (this.InnerCircleBorder != null)
            {
                this.mainglasspath.Height = this.InnerCircleBorder.Height / 2;
                this.mainglasspath.Width = this.InnerCircleBorder.Width;
                this.PART_SecondGlassPath.Width = this.InnerCircleBorder.Width / 4;
                this.PART_SecondGlassPath.Height = this.InnerCircleBorder.Height / 4;

                this.mainglasspath.Margin = new Thickness(this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset - 1, this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset + 3, 0, 0);
                this.PART_SecondGlassPath.Margin = new Thickness(this.Radius / 2, this.Radius / 2, 0, 0);
                if (this.FrameType == GaugeFrameType.QuarterCircular)
                {

                    this.PART_SecondGlassPath.Width = this.PART_SecondGlassPath.Width / 2;
                    this.PART_SecondGlassPath.Height = this.PART_SecondGlassPath.Height / 2;
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 47;

                    rt.CenterX = this.mainglasspath.Width / 2;
                    rt.CenterY = this.mainglasspath.Width / 2;
                    RotateTransform rtt = new RotateTransform();
                    rtt.Angle = 65;

                    rtt.CenterX = this.PART_SecondGlassPath.Width;
                    rtt.CenterY = this.PART_SecondGlassPath.Width;
                    TranslateTransform tt = new TranslateTransform();
                    tt.X = this.PART_SecondGlassPath.Width * 2;
                    tt.Y = 0;
                    TransformGroup tg = new TransformGroup();
                    tg.Children.Add(rtt);
                    tg.Children.Add(tt);

                    this.mainglasspath.RenderTransform = rt;
                    this.PART_SecondGlassPath.RenderTransform = tg;
                    this.mainglasspath.Width = 9.5 * this.InnerCircleBorder.Width / 10;
                }
                else
                {
                    this.mainglasspath.Height = this.InnerCircleBorder.Height / 2;
                    this.mainglasspath.Width = this.InnerCircleBorder.Width;
                    this.PART_SecondGlassPath.Width = this.InnerCircleBorder.Width / 4;
                    this.PART_SecondGlassPath.Height = this.InnerCircleBorder.Height / 4;
                    this.PART_SecondGlassPath.RenderTransform = null;
                    this.mainglasspath.RenderTransform = null;
                    this.mainglasspath.Margin = new Thickness(this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset - 1, this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset + 3, 0, 0);
                    this.PART_SecondGlassPath.Margin = new Thickness(this.Radius / 2, this.Radius / 2, 0, 0);
                }
            }
            foreach (CircularScale scale in this.Scales)
            {
                scale.RefreshScaleFrame();
                scale.RefreshScalePath();
                scale.RefreshScale();
                scale.RefreshTickSet();
                scale.RefreshLabelTickSet();
                scale.RefreshPointers();
            }

        }
      
        #endregion

        #region Implementation
        /// <summary>
        /// Invoked when the control is ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void CircularGaugeLoaded(object sender, RoutedEventArgs e)
        {
            this.Width = this.Radius * 2;
            this.Height = this.Radius * 2;

            this.RefreshBorders();
            this.UpdateChildrenLocation();
        }

        /// <summary>
        /// Method to set the visual state transition.
        /// </summary>
        /// <param name="useTransitions">Whether to apply transition or not.</param>
        /// <param name="stateNames">Which state transition is to be applied</param>
        private void GoToState(bool useTransitions, params string[] stateNames)
        {
            if (stateNames != null)
            {
                foreach (string str in stateNames)
                {
                    if (this.FirstCircleBorder != null)
                    {
                        if (VisualStateManager.GoToState(this, str, useTransitions))
                        {
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to update VisualState.
        /// </summary>
        /// <param name="useTransitions">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="type"></param>
        /// <remarks></remarks>
        private void UpdateVisualState(bool useTransitions, GaugeFrameType type)
        {
            if (type == GaugeFrameType.SemiCircular)
            {
                this.GoToState(useTransitions, new string[] { "SemiCircular" });
            }
            else if (type == GaugeFrameType.QuarterCircular)
            {
                this.GoToState(useTransitions, new string[] { "QuaterCircular" });
            }
        }

        /// <summary>
        /// Invoked when size of control is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void CircularGaugeSizeChanged(object sender, RoutedEventArgs e)
        {
            this.Width = this.Radius * 2;
            this.Height = this.Radius * 2;
        }

        /// <summary>
        /// Updates property value cache and raises FrameTypeChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnFrameTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateVisualState(true, this.FrameType);
            this.RefreshFrameType();
            if (this.FrameTypeChanged != null)
            {
                this.FrameTypeChanged(this, e);
            }
        }

        private void RefreshFrameType()
        {
            if (this.FrameType == GaugeFrameType.QuarterCircular || this.FrameType == GaugeFrameType.SemiCircular)
            {
                RotateTransform rt = new RotateTransform();
                rt.CenterY = this.Radius;
                rt.CenterX = this.Radius;
                if (this.FrameType == GaugeFrameType.QuarterCircular)
                {
                    switch (this.GaugeFrameDirection)
                    {
                        case FrameDirection.East: rt.Angle = 45;
                            break;
                        case FrameDirection.West: rt.Angle = -135;
                            break;
                        case FrameDirection.North: rt.Angle = 315;
                            break;
                        case FrameDirection.South: rt.Angle = 135;
                            break;
                        case FrameDirection.SouthEast: rt.Angle = 90;
                            break;
                        case FrameDirection.SouthWest: rt.Angle = 180;
                            break;
                        case FrameDirection.NorthEast: rt.Angle = 0;
                            break;
                        case FrameDirection.NorthWest: rt.Angle = 270;
                            break;
                    }
                }
                else
                {
                    switch (this.GaugeFrameDirection)
                    {
                        case FrameDirection.East: rt.Angle = 90;
                            break;
                        case FrameDirection.West: rt.Angle = -90;
                            break;
                        case FrameDirection.North: rt.Angle = 0;
                            break;
                        case FrameDirection.South: rt.Angle = 180;
                            break;
                    }
                }

                this.RenderTransformOrigin = new Point(0, 0);
                this.RenderTransform = rt;
                RotateTransform dt = new RotateTransform();
                dt.CenterY = 0.5;
                dt.CenterX = 0.5;
                dt.Angle = -rt.Angle;

                foreach (ScaleBase s in this.Scales)
                {
                    s.RefreshLabelTickSet();
                }
                if (this.DigitalValue != null)
                {
                    this.DigitalValue.RenderTransform = dt;
                    this.DigitalValue.RenderTransformOrigin = new Point(0.5, 0.5);
                }
            }
            else
            {
                this.RenderTransform = null;
                if (this.DigitalValue != null)
                {
                    this.DigitalValue.RenderTransform = null;
                }
            }
            this.RefreshBorders();
        }

        /// <summary>
        /// Updates property value cache and raises GaugeFrameDirectionChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnGaugeFrameDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshFrameType();
            if (this.GaugeFrameDirectionChanged != null)
            {
                this.GaugeFrameDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises RadiusChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();

            if (this.RadiusChanged != null)
            {
                this.RadiusChanged(this, e);
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
        /// Calls OnGaugeFrameDirectionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGaugeFrameDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge instance = (CircularGauge)d;
            instance.OnGaugeFrameDirectionChanged(e);
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

        #endregion
    }
}
