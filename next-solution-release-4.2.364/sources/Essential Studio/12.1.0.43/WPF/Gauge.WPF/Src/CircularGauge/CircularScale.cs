// <copyright file="CircularScale.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the scale visual element.
    /// </summary>
    /// <remarks>
    /// The CircularScale is the <see cref="Visual.VisualParent"/> for the various elements of the Gauge, such as
    /// <see cref="CircularLabelTick"/>, <see cref="CircularMarkTick"/>, <see cref="CircularRange"/>,
    /// and <see cref="CircularPointer"/>.
    /// </remarks>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="CircularScaleSample.Window1" Title="CircularScaleSample" Height="400"
    /// Width="400" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <syncfusion:CircularGauge Name="circularGauge">
    ///         <syncfusion:CircularGauge.Scales>
    ///             <syncfusion:CircularScale Name="circularScale" Radius="100"
    ///                                       Minimum="0" Maximum="100" MajorIntervalValue="10" 
    ///                                       MinorIntervalValue="2" ScaleBarSize="10" 
    ///                                       GapSweepAngle="310" StartAngle="110"
    ///                                       BackgroundBrush="LightBlue">
    ///             </syncfusion:CircularScale>
    ///         </syncfusion:CircularGauge.Scales>
    ///     </syncfusion:CircularGauge>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace CircularScaleSample
    /// {
    ///     public partial class Window1 : Window
    ///     { 
    ///         private CircularScale m_scale;
    ///         private CircularGauge m_gauge;
    ///         public Window1()
    ///         {
    ///             InitializeComponent();<para/>
    ///             m_scale = new CircularScale();
    ///             m_gauge = new CircularGauge();
    ///             m_scale.ShadowOffset = 1;
    ///             m_scale.Minimum = 0;
    ///             m_scale.Maximum = 100;
    ///             m_scale.MinorIntervalValue = 2;
    ///             m_scale.MajorIntervalValue = 10;
    ///             m_scale.StartAngle = 120;
    ///             m_scale.GapSweepAngle = 300;
    ///             m_scale.ScaleBarSize = 5;
    ///             m_scale.Radius = 116;
    ///             this.m_gauge.Scales.Add(m_scale);
    ///             this.Content = m_gauge;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CircularScale : ScaleBase
    {
        #region Private Members
        /// <summary>
        /// The pointer cap shadow.
        /// </summary>
        private PointerCapShadow m_pointerCapShadow = new PointerCapShadow();

        /// <summary>
        /// Stores the gauge's reference
        /// </summary>
        internal CircularGauge gaugeReference = null;

        /// <summary>
        /// Default Scale bool property for Auto Scaling support.
        /// </summary>
        internal bool isDefaultScale = false;

        /// <summary>
        /// Collection of the pointers.
        /// </summary>
        private PointersCollection m_pointers;

        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double m_radiusRatio;

        /// <summary>
        /// The geomerty used to draw the scale.
        /// </summary>
        private Geometry m_scalePath;
        private List<CircularPointer> pointerlist = new List<CircularPointer>();

        #endregion Private Members

        #region CLR Getters & Setters
        /// <summary>
        /// Gets or sets a collection of pointers.
        /// </summary>
        /// <value>
        /// Type: <see cref="PointersCollection"/>
        /// </value>
        public PointersCollection Pointers
        {
            get
            {
                return m_pointers;
            }

            set
            {
                m_pointers = value;
            }
        }

        /// <summary>
        /// Gets or sets the ratio between the scale and the gauge.
        /// </summary>
        internal double RadiusRatio
        {
            get
            {
                return m_radiusRatio;
            }

            set
            {
                m_radiusRatio = value;
            }
        }
        #endregion CLR Getters & Setters

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="GapSweepAngle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GapSweepAngleChanged;

        /// <summary>
        /// Event that is raised when <see cref="PointerCap"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerCapChanged;

        /// <summary>
        /// Event that is raised when <see cref="Radius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback RadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="StartAngle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback StartAngleChanged;

       
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="GapSweepAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GapSweepAngleProperty =
            DependencyProperty.Register("GapSweepAngle", typeof(double), typeof(CircularScale), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGapSweepAngleChanged)));
        /// <summary>
        /// Identifies the <see cref="GapSweepAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableCustomScaleAngleProperty =
            DependencyProperty.Register("IsCustomScalingAngle", typeof(bool), typeof(CircularScale), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIsCustomScalingAngleChanged)));

        /// <summary>
        /// Identifies the <see cref="PointerCap"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerCapProperty =
            DependencyProperty.Register("PointerCap", typeof(PointerCap), typeof(CircularScale), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPointerCapChanged), CoercePointerCap));

        /// <summary>
        /// Identifies the <see cref="Radius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(CircularScale), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnRadiusChanged), CoerceRadius));

        /// <summary>
        /// Identifies the <see cref="StartAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(CircularScale), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnStartAngleChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the number of degrees that the scale will sweep in a circle.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double GapSweepAngle
        {
            get
            {
                return (double)GetValue(GapSweepAngleProperty);
            }

            set
            {
                SetValue(GapSweepAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the PointerCap of the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="PointerCap"/>
        /// </value>       
        public PointerCap PointerCap
        {
            get
            {
                return (PointerCap)GetValue(PointerCapProperty);
            }

            set
            {
                SetValue(PointerCapProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius of the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the start angle from which the scale should be drawn.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double StartAngle
        {
            get
            {
                return (double)GetValue(StartAngleProperty);
            }

            set
            {
                SetValue(StartAngleProperty, value);
            }
        }

        /// <summary>
        /// Sets or Gets the IsCustomScalingAngle property
        /// </summary>
        public bool EnableCustomScaleAngle
        {
            get { return (bool)GetValue(EnableCustomScaleAngleProperty); }
            set { SetValue(EnableCustomScaleAngleProperty, value); }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="CircularScale"/> class.
        /// Overrides some dependency properties.
        /// </summary>
        static CircularScale()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(CircularScale));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularScale), new FrameworkPropertyMetadata(typeof(CircularScale)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularScale"/> class.
        /// </summary>
        /// <remarks>
        /// Initializes all its Child Collections and registers its method CollectionChanged
        /// to its Children's CollectionChanged event.
        /// </remarks>
        public CircularScale()
        {
            this.Loaded += new RoutedEventHandler(CircularScaleLoaded);

            this.Ticks = new TicksCollection(this);
            this.Ranges = new RangesCollection(this);
            this.Pointers = new PointersCollection(this);

            this.Ticks.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            this.Ranges.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            this.Pointers.CollectionChanged += new NotifyCollectionChangedEventHandler(Pointers_CollectionChanged);
            this.PointerCap = new PointerCap();
            this.AddVisualChild(m_pointerCapShadow);
        }

        void Pointers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Visual element in e.NewItems)
                {

                    if (element is CircularPointer)
                    {
                        CircularPointer elem = element as CircularPointer;
                        elem.PointerNeedleTypeChanged += new PropertyChangedCallback(PointerNeedleTypeChanged);
                        elem.SetScope(gaugeReference);
                        this.PointerCap.SetScope(gaugeReference);
                    }
                    if (element is CircularPointer)
                    {
                        AddPointer(element as CircularPointer);
                        this.AddVisualChild(element);
                        (element as CircularPointer).Measure(this.RenderSize);
                        (element as CircularPointer).Arrange(new Rect(this.RenderSize));
                    }
                }
                RefreshCircularScale();
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                for (int i = 0; i < pointerlist.Count; i++)
                {
                    this.RemoveVisualChild(pointerlist[i]);

                }
                pointerlist.Clear();

            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                for (int i = 0; i < pointerlist.Count; i++)
                {
                    if (i == this.Pointers.Count)
                        break;
                    else
                    {
                        CircularPointer pointer1 = this.Pointers[i];
                        CircularPointer pointer2 = pointerlist[i];
                        if (!pointer1.Equals(pointer2))
                        {
                            this.RemoveVisualChild(pointer2);
                            pointerlist.Remove(pointer2);
                        }

                    }
                }

            }
        }


        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value>Type: <see cref="int">Returns the count of the children</see></value>
        /// <remarks>
        /// Returns number of Children it contains. The addition of 2 is to add PointerCap
        /// and PointerCapShadow to the count.
        /// </remarks>
        /// <returns>int</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return this.Ticks.Count + this.Ranges.Count + this.Pointers.Count + 2;
            }
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <remarks>
        /// Since PointerCapShadow is added as a Child element in the Constructor itself,
        /// index 0 represents it.Then comes the Ranges collection, then the Ticks and the
        /// last one in Pointers Collection. Pointers are added at the last because the
        /// latter the elements are added they will be on top of others.
        /// </remarks>
        /// <param name="index">he zero-based index of the requested child element in the
        /// collection.</param>
        /// <returns>
        /// The requested child element.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            FrameworkElement elem = null;

            if (index == 0)
            {
                elem = m_pointerCapShadow;
            }
            else if (index > 0 && index <= this.Ranges.Count)
            {
                elem = this.Ranges[index - 1];
            }
            else if (index > this.Ranges.Count && index <= this.Ranges.Count + this.Ticks.Count)
            {
                elem = this.Ticks[index - this.Ranges.Count - 1];
            }
            else
            {
                int countElems = this.Ticks.Count + this.Ranges.Count + 1;
                if (this.PointerCap.CapOnTop)
                {
                    if (index >= countElems && index < countElems + this.Pointers.Count)
                    {
                        elem = this.Pointers[index - countElems];
                    }
                    else if (index >= countElems + this.Pointers.Count && index <= countElems + this.Pointers.Count + 1)
                    {
                        elem = this.PointerCap;
                    }
                }
                else
                {
                    if (index >= countElems && index < countElems + 1)
                    {
                        elem = this.PointerCap;
                    }
                    else if (index >= countElems + 1 && index <= countElems + this.Pointers.Count + 1)
                    {
                        elem = this.Pointers[index - countElems - 1];
                    }
                }
            }

            return elem;
        }

        /// <summary>
        /// Measures the size in layout required for child elements and determines a size
        /// for the element.
        /// </summary>
        /// <remarks>
        /// The diameter of the CircularGauge is written as the required width and height.
        /// </remarks>
        /// <param name="availableSize">The available size that this element can give to
        /// child elements.</param>
        /// <returns>
        /// The size that this element determines it needs during layout.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            int visualChildrenCount = this.VisualChildrenCount;
            for (int i = 0; i < visualChildrenCount; i++)
            {
                (this.GetVisualChild(i) as UIElement).Measure(availableSize);
            }

            return new Size(this.Radius * 2, this.Radius * 2);
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <remarks>
        /// Loops through and gets each and every child element.Arranges Pointer element to
        /// the center of the Scale in such a way that the scale's center and the Pointer's
        /// height's center coincides. For marker, instead of arranging it at the center of the
        /// Scale it arranges it based on its ScalePlacement's Value, so as Tick are
        /// arranged. Remaining elements, it arranges them at the center of the scale.
        /// Then the Scale's PathFigure is created.
        /// </remarks>
        /// <param name="finalSize">The final area within the parent that this element
        /// should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int visualChildrenCount = this.VisualChildrenCount;
            for (int i = 0; i < visualChildrenCount; i++)
            {
                FrameworkElement el = this.GetVisualChild(i) as FrameworkElement;
                if (el is CircularPointer)
                {
                    CircularPointer pointer = el as CircularPointer;
                    if (pointer.PointerNeedleType == PointerNeedleType.Needle)
                    {
                        pointer.Arrange(new Rect(this.DesiredSize.Width / 2, (this.DesiredSize.Height / 2) - (pointer.PointerWidth / 2), pointer.DesiredSize.Width, pointer.DesiredSize.Height));
                    }
                    else if (pointer.PointerNeedleType == PointerNeedleType.Marker)
                    {
                        double xpos = 0;
                        if (pointer.PointerPlacement == ScalePlacement.Cross)
                        {
                            xpos = (this.DesiredSize.Width / 2) + this.Radius - ((this.ScaleBarSize + pointer.DesiredSize.Width) / 2);
                        }
                        else if (pointer.PointerPlacement == ScalePlacement.Inside)
                        {
                            xpos = (this.DesiredSize.Width / 2) - pointer.DesiredSize.Width;
                            if (this.Radius > this.ScaleBarSize)
                            {
                                xpos -= this.ScaleBarSize - this.Radius;
                            }
                        }
                        else if (pointer.PointerPlacement == ScalePlacement.Outside)
                        {
                            xpos = (this.DesiredSize.Width / 2) + this.Radius;
                        }

                        pointer.Arrange(new Rect(xpos, (this.DesiredSize.Height / 2) - (pointer.DesiredSize.Height / 2), pointer.DesiredSize.Width, pointer.DesiredSize.Height));
                    }
                    else
                    {
                        pointer.Arrange(new Rect(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2, pointer.DesiredSize.Width, pointer.DesiredSize.Height));
                    }
                }
                else if (el is CircularMarkTick)
                {
                    CircularMarkTick tick = el as CircularMarkTick;
                    double xpos = 0;
                    if (tick.TickPlacement == ScalePlacement.Cross)
                    {
                        xpos =
                            (this.DesiredSize.Width / 2) + this.Radius - ((this.ScaleBarSize + tick.DesiredSize.Width) / 2) + tick.DistanceFromScale;
                    }
                    else if (tick.TickPlacement == ScalePlacement.Inside)
                    {
                        xpos = (this.DesiredSize.Width / 2) - tick.DesiredSize.Width;
                        if (this.Radius > this.ScaleBarSize)
                        {
                            xpos = xpos + this.Radius - this.ScaleBarSize - tick.DistanceFromScale;
                            //xpos -= this.ScaleBarSize - this.Radius + tick.DistanceFromScale;
                        }
                    }
                    else if (tick.TickPlacement == ScalePlacement.Outside)
                    {
                        xpos = (this.DesiredSize.Width / 2) + this.Radius + tick.DistanceFromScale;
                    }

                    el.Arrange(new Rect(xpos, (this.DesiredSize.Height / 2) - (el.DesiredSize.Height / 2), el.DesiredSize.Width, el.DesiredSize.Height));
                }
                else
                {
                    el.Arrange(new Rect(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2, el.DesiredSize.Width, el.DesiredSize.Height));
                }
            }

            if (this.ScaleBarSize >= 0)
            {
                Point centerPoint = new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);
                bool isArcLarge = this.GapSweepAngle >= 180;
                double innerRadius = this.Radius - this.ScaleBarSize > 0 ? this.Radius - this.ScaleBarSize : 0;
                double outerRadius = this.Radius;

                if (this.GapSweepAngle == 360)
                {
                    m_scalePath = new GeometryGroup();
                    (m_scalePath as GeometryGroup).Children.Add(new EllipseGeometry(centerPoint, outerRadius, outerRadius));
                    (m_scalePath as GeometryGroup).Children.Add(new EllipseGeometry(centerPoint, innerRadius, innerRadius));
                }
                else
                {
                    m_scalePath = new PathGeometry();
                    Point startPoint1 = this.ConvertToStageCoordinates(innerRadius, this.StartAngle, centerPoint);
                    Point endPoint1 = this.ConvertToStageCoordinates(innerRadius, this.StartAngle + this.GapSweepAngle, centerPoint);
                    Point startPoint2 = this.ConvertToStageCoordinates(outerRadius, this.StartAngle, centerPoint);
                    Point endPoint2 = this.ConvertToStageCoordinates(outerRadius, this.StartAngle + this.GapSweepAngle, centerPoint);

                    ArcSegment asp1 = new ArcSegment(startPoint1, new Size(innerRadius, innerRadius), 360, isArcLarge, SweepDirection.Clockwise, true);
                    ArcSegment asp2 = new ArcSegment(startPoint2, new Size(outerRadius, outerRadius), 360, isArcLarge, SweepDirection.Counterclockwise, true);
                    ArcSegment aep1 = new ArcSegment(endPoint1, new Size(innerRadius, innerRadius), 360, isArcLarge, SweepDirection.Clockwise, true);
                    ArcSegment aep2 = new ArcSegment(endPoint2, new Size(outerRadius, outerRadius), 360, isArcLarge, SweepDirection.Counterclockwise, true);
                    LineSegment lsp1 = new LineSegment(startPoint1, true);
                    LineSegment lep2 = new LineSegment(endPoint2, true);

                    PathFigure figure1 = new PathFigure(
                        startPoint1,
                        new PathSegment[] { asp1, aep1, lep2, aep2, asp2, lsp1 },
                        false);
                    (m_scalePath as PathGeometry).Figures.Add(figure1);
                }
            }

            return finalSize;
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            SetDefaultScales();
            SetCustomStyles();
            base.OnRender(drawingContext);
            drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_scalePath);
        }

        private void SetDefaultScales()
        {
            if (!this.EnableCustomScaleAngle)
            {
                switch (gaugeReference.FrameType)
                {
                    case GaugeFrameType.HalfCircle:
                        this.StartAngle = 180;
                        this.GapSweepAngle = 180;
                        break;
                    case GaugeFrameType.LeftHalfCircle:
                        this.StartAngle = 90;
                        this.GapSweepAngle = 180;
                        break;
                    case GaugeFrameType.RightHalfCircle:
                        this.StartAngle = 270;
                        this.GapSweepAngle = 180;
                        break;
                    case GaugeFrameType.CounterclockwiseHalfCircle:
                        this.StartAngle = 0;
                        this.GapSweepAngle = 180;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the custom styles.
        /// </summary>
        private void SetCustomStyles()
        {
            if (gaugeReference.PointerCapStyle != null)
                this.PointerCap.Style = gaugeReference.PointerCapStyle;
            if (gaugeReference.PointerStyle != null)
                foreach (CircularPointer pointer in this.Pointers)
                {
                    pointer.Style = gaugeReference.PointerStyle;
                }

            foreach (TickBase tick in this.Ticks)
            {
                if (tick.GetType() == typeof(CircularMarkTick))
                {
                    if ((gaugeReference.MajorTickStyle != null)
                     && (tick.TickStyle == TickStyle.MajorTick))
                        tick.Style = gaugeReference.MajorTickStyle;
                    else if ((gaugeReference.MinorTickStyle != null)
                     && (tick.TickStyle == TickStyle.MinorTick))
                        tick.Style = gaugeReference.MinorTickStyle;
                }
                else if ((tick.GetType() == typeof(CircularLabelTick)) && (gaugeReference.LabelTickStyle != null))
                    tick.Style = gaugeReference.LabelTickStyle;
            }
        }

        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Occurs when the scale is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void CircularScaleLoaded(object sender, RoutedEventArgs e)
        {
            foreach (CircularPointer pointer in this.Pointers)
            {
                if (pointer.Value < this.Minimum)
                {
                    pointer.Value = this.Minimum;
                }

                if (pointer.Value > this.Maximum)
                {
                    pointer.Value = this.Maximum;
                }

                pointer.RefreshAnglePosition();
            }
        }

        /// <summary>
        /// Coerces the value of the <see cref="PointerCap"/> property.
        /// </summary>
        /// <param name="d">The <see cref="CircularScale"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoercePointerCap(DependencyObject d, object value)
        {
            CircularScale owner = d as CircularScale;
            return owner.CoercePointerCap(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="PointerCap"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoercePointerCap(object value)
        {
            this.RemoveVisualChild(this.PointerCap);
            return value;
        }

        /// <summary>
        /// Coerces the value of the <see cref="Radius"/> property.
        /// </summary>
        /// <param name="d">The <see cref="CircularScale"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceRadius(DependencyObject d, object value)
        {
            CircularScale owner = d as CircularScale;
            return owner.CoerceRadius(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="Radius"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceRadius(object value)
        {
            double val = (double)value;
            if (val < 0)
            {
                val = 0;
            }

            return val;
        }

        /// <summary>
        /// Converts polar coordinates to stage coordinates.
        /// </summary>
        /// <param name="radius">Radius to convert.</param>
        /// <param name="angle">Angle to convert.</param>
        /// <param name="pointToShift">Point to shift the result point by.</param>
        /// <returns>Calculated coordinates.</returns>
        /// <remarks>A particular point(x,y) on a circle can be calculated if, 
        ///     <list>The radius of the circle is known.</list>     
        ///     <list>Angle between the line connecting the center-point to the StartAngle's point<para/>
        ///     and that of the point(x,y), whose position to be found, is known.
        ///     </list>
        /// Then the point(x,y) can be found out by the following formula:
        ///                x : radius*Cos(angleInRadians)
        ///                y : radius*sin(angleInRadians) 
        /// </remarks>
        internal Point ConvertToStageCoordinates(double radius, double angle, Point pointToShift)
        {
            Point point = new Point(radius * Math.Cos(angle * Math.PI / 180), radius * Math.Sin(angle * Math.PI / 180));
            point.X += pointToShift.X;
            point.Y += pointToShift.Y;
            return point;
        }

        /// <summary>
        /// Gets the angle according to the value.
        /// </summary>
        /// <param name="value">The value to find the angle for.</param>
        /// <returns>Calculated angle.</returns>
        /// <remarks>
        /// <list type="bullet">
        ///     <listheader>Local Variables Used</listheader>
        ///     <item>
        ///         <term>ratio</term>
        ///         <description>Calculates the number of segments(between Maximum and Minimum values),
        ///         that are possible,each of "value - Minimum" length.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>angle</term>
        ///        <description>Calculate the angles, of interval("value-Minimum") relative to Sweep angle 
        ///         of the Gauge and then adding the StartAngle to get the exact angle to place the Pointer.
        ///         </description>
        ///     </item>
        /// </list></remarks>
        internal double GetAngleByValue(double value)
        {
            double ratio;
            double angle = 0;
            if (this.ScaleDirection == ScaleDirection.CounterClockwise)
            {
                if (this.Minimum <= value && value <= this.Maximum)
                {
                    ratio = (this.Maximum - this.Minimum) / (this.Maximum - value);
                    angle = (this.GapSweepAngle / ratio) + this.StartAngle;
                }
            }
            else
            {
                ratio = (this.Maximum - this.Minimum) / (value - this.Minimum);
                angle = (this.GapSweepAngle / ratio) + this.StartAngle;
            }
            return angle;
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="GapSweepAngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGapSweepAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            foreach (CircularPointer pointer in this.Pointers)
            {
                pointer.RefreshAnglePosition();
            }

            if (GapSweepAngleChanged != null)
            {
                GapSweepAngleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGapSweepAngleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGapSweepAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularScale instance = (CircularScale)d;
            instance.OnGapSweepAngleChanged(e);
        }


        private static void OnIsCustomScalingAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularScale instance = (CircularScale)d;
            instance.OnIsCustomScalingAngleChanged(e);
        }

        /// <summary>
        /// This method is called when Custom Scale Angle is changed
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        protected virtual void OnIsCustomScalingAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                var frameworkElement = this.GetVisualChild(i) as FrameworkElement;
                if (frameworkElement != null)
                    frameworkElement.InvalidateVisual();
            }
        }


        /// <summary>
        /// Invoked when <see cref="ScaleBase.Maximum"/> property is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnMaximumChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnMaximumChanged(e);
            /*
            foreach (CircularPointer pointer in this.Pointers)
            {
                if (pointer.Value > this.Maximum)
                {
                    pointer.Value = this.Maximum;
                }
            }*/
        }

        /// <summary> 
        /// Invoked when <see cref="ScaleBase.Minimum"/> property is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnMinimumChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnMinimumChanged(e);
            /*
            foreach (CircularPointer pointer in this.Pointers)
            {
                if (pointer.Value < this.Minimum)
                {
                    pointer.Value = this.Minimum;
                }
            }*/
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerCapChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerCapChanged(DependencyPropertyChangedEventArgs e)
        {
            this.AddVisualChild(this.PointerCap);
            this.PointerCap.PointerCapRadiusChanged += new PropertyChangedCallback(PointerCapPointerCapRadiusChanged);

            if (PointerCapChanged != null)
            {
                this.PointerCapChanged(this, e);
            }
        }


        /// <summary>
        /// Updates property value cache and raises ScaleBarSizeChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnScaleDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnScaleDirectionChanged(e);
            this.RefreshCircularScale();

        }

        /// <summary>
        /// Calls OnPointerCapChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerCapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularScale instance = (CircularScale)d;
            instance.OnPointerCapChanged(e);
        }

        /// <summary>
        /// Calls OnRadiusChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularScale instance = (CircularScale)d;
            instance.OnRadiusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="RadiusChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (CircularPointer pointer in this.Pointers)
            {
                pointer.AnglePosition = this.StartAngle;
                pointer.RefreshAnglePosition();
            }

            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (RadiusChanged != null)
            {
                this.RadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="StartAngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStartAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            foreach (CircularPointer pointer in this.Pointers)
            {
                pointer.RefreshAnglePosition();
            }

            if (StartAngleChanged != null)
            {
                this.StartAngleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnStartAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularScale instance = (CircularScale)d;
            instance.OnStartAngleChanged(e);
        }

        /// <summary>
        /// Occurs when <see cref="CircularScale.PointerCapProperty"/>is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Property changed details.</param>
        private void PointerCapPointerCapRadiusChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (m_pointerCapShadow != null)
            {
                m_pointerCapShadow.PointerCapRadius = this.PointerCap.PointerCapRadius;
            }
        }

        /// <summary>
        /// Occurs when the type of any pointer is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void PointerNeedleTypeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool hide = true;
            foreach (CircularPointer pointer in this.Pointers)
            {
                if (pointer.PointerNeedleType == PointerNeedleType.Needle)
                {
                    hide = false;
                    break;
                }
            }

            if (hide)
            {
                this.PointerCap.Visibility = Visibility.Hidden;
            }
            else
            {
                this.PointerCap.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Refreshes the Circular Scale by invalidating the Visual of all its child elements.
        /// </summary>
        public void RefreshCircularScale()
        {
            double min = Math.Min(this.Maximum, this.Minimum);
            double max = Math.Max(this.Maximum, this.Minimum);

            this.Minimum = min;
            this.Maximum = max;

            foreach (CircularPointer pointer in this.Pointers)
            {
                pointer.AnglePosition = this.StartAngle;
                pointer.RefreshAnglePosition();
            }

            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }
        }
        #endregion Implementation

        #region Added Code
        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> property 
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            NameScope.SetNameScope(this, null);
        }

        /// <summary>
        /// Sets the scope to <see cref="CircularGauge"/>to facilitate Binding. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="gauge">The <see cref="CircularGauge"/> that contains the reference to the Gauge.</param>
        internal void SetScope(CircularGauge gauge)
        {
            gaugeReference = gauge;
            CalculateScope(gauge);
        }

        /// <summary>
        /// Calculates the scope. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> which contains the element to set the scope for.</param>
        internal void CalculateScope(DependencyObject obj)
        {
            DependencyObject ele = obj;
            while (ele != null)
            {
                INameScope ns = NameScope.GetNameScope(ele);
                if (ns != null)
                {
                    if (!(ns is System.Windows.NameScope))
                    {
                        break;
                    }

                    NameScope.SetNameScope(this, ns);
                    break;
                }

                ele = LogicalTreeHelper.GetParent(ele) ?? VisualTreeHelper.GetParent(ele);
            }
        }

        /// <summary>
        /// Occurs when element is added, replaced or removed from the collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        /// <remarks>
        /// Whenever Ticks,Ranges and Pointers are added, i.e when thier CollectionChanged
        /// event is raised, this method will be called. This method adds all the elements
        /// as Scale's Visual child.
        /// </remarks>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Visual element in e.NewItems)
                {

                    if (element is CircularPointer)
                    {
                        CircularPointer elem = element as CircularPointer;
                        elem.PointerNeedleTypeChanged += new PropertyChangedCallback(PointerNeedleTypeChanged);
                        elem.SetScope(gaugeReference);
                        this.PointerCap.SetScope(gaugeReference);
                    }
                    else if (element is CircularMarkTick)
                    {
                        CircularMarkTick elem = element as CircularMarkTick;
                        elem.SetScope(gaugeReference);
                    }
                    else if (element is CircularLabelTick)
                    {
                        CircularLabelTick elem = element as CircularLabelTick;
                        elem.SetScope(gaugeReference);
                    }
                    else if (element is CircularRange)
                    {
                        CircularRange elem = element as CircularRange;
                        elem.SetScope(gaugeReference);
                    }
                    if (!(element is LinearRange))
                    {
                        if (element is CircularPointer)
                        {
                            this.AddVisualChild(element);
                            (element as CircularPointer).Measure(this.RenderSize);
                            (element as CircularPointer).Arrange(new Rect(this.RenderSize));
                        }
                        else if (element is CircularRange)
                        {
                            this.AddVisualChild(element);
                            (element as CircularRange).Measure(this.RenderSize);
                            (element as CircularRange).Arrange(new Rect(this.RenderSize));
                        }
                        else
                        {
                            this.AddVisualChild(element);

                        }

                    }
                }
                RefreshCircularScale();
            }


        }
        private void AddPointer(CircularPointer pointer)
        {
            pointerlist.Add(pointer);

        }
        #endregion Added Code
    }
}
