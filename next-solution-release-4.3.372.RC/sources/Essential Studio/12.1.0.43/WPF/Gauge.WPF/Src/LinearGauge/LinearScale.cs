// <copyright file="LinearScale.cs" company="Syncfusion Software">
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
    /// The LinearScale is the <see cref="Visual.VisualParent"/> for the various elements of the Gauge, such as
    /// <see cref="LinearLabelTick"/>, <see cref="LinearMarkTick"/>, <see cref="LinearRange"/>,
    /// <see cref="LinearBarPointer"/> and <see cref="LinearMarkerPointer"/>.
    /// </remarks>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="LinearScaleSample.Window1" Title="LinearScaleSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:LinearGauge CenterFrameFillColor="Brown" Name="linearGauge1">
    ///             <syncfusion:LinearGauge.Scales>
    ///                 <syncfusion:LinearScale Name="LinearScale" Minimum="0" Maximum="100" 
    ///                                         MinorIntervalValue="2" MajorIntervalValue="10" 
    ///                                         ScaleBarSize="20" ScaleBarLength="260">
    ///                 </syncfusion:LinearScale>
    ///             </syncfusion:LinearGauge.Scales>
    ///         </syncfusion:LinearGauge>
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Media;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace LinearScaleSample
    /// {
    ///         public partial class Window1 : Window
    ///         {
    ///             private LinearGauge linearGauge1;<para/>
    ///             public Window1()
    ///             {                
    ///                 InitializeComponent();<para/>
    ///                 linearGauge1 = new LinearGauge();
    ///                 this.linearGauge1.CenterFrameFillColor = Colors.Brown;<para/>
    ///                 LinearScale scale = new LinearScale();
    ///                 scale.Minimum = 0;
    ///                 scale.Maximum = 100;
    ///                 scale.MinorIntervalValue = 2;
    ///                 scale.MajorIntervalValue = 10;
    ///                 scale.ScaleBarSize = 20;
    ///                 scale.ScaleBarLength = 260;
    ///                 linearGauge1.Scales.Add(scale);<para/>
    ///                 this.Content = linearGauge1;
    ///             }
    ///         }
    ///     }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LinearScale : ScaleBase
    {
        #region Private Members
        /// <summary>
        /// Store LinearGauge's Actual Height.
        /// </summary>
        private double m_gaugeActualHeight;

        /// <summary>
        /// Stores the gauge's reference
        /// </summary>
        internal LinearGauge gaugeReference = null;

        /// <summary>
        /// Store LinearGauge's Actual Width.
        /// </summary>
        private double m_gaugeActualWidth;

        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double m_lengthRatio;

        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double m_sizeRatio;

        /// <summary>
        /// The pointer cap shadow.
        /// </summary>
        private PointerCapShadow m_pointerCapShadow = new PointerCapShadow();

        /// <summary>
        /// Collection of the linear pointers.
        /// </summary>
        private LinearPointersCollection m_pointers;

        /// <summary>
        /// The geomerty used to draw the scale.
        /// </summary>
        private Geometry m_scalePath;

        /// <summary>
        /// Horizontal distance between the scale and gauge.
        /// </summary>
        private double m_scaleWidth;
        #endregion Private Members

        #region CLR Getters & Setters
        /// <summary>
        /// Gets or sets the actual Height of Linear Gauge.
        /// </summary>
        /// <seealso cref="GaugeActualWidth"/>
        internal double GaugeActualHeight
        {
            get
            {
                return m_gaugeActualHeight;
            }

            set
            {
                m_gaugeActualHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the actual Width of Linear Gauge.
        /// </summary>
        /// <seealso cref="GaugeActualHeight"/>
        internal double GaugeActualWidth
        {
            get
            {
                return m_gaugeActualWidth;
            }

            set
            {
                m_gaugeActualWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the ratio of ScaleBarLength to that of LinearGauge's Height.
        /// </summary>
        internal double LengthRatio
        {
            get
            {
                return m_lengthRatio;
            }

            set
            {
                m_lengthRatio = value;
            }
        }

        /// <summary>
        /// Gets or sets the ratio of ScaleBarSize to that of LinearGauge's Width.
        /// </summary>
        internal double SizeRatio
        {
            get
            {
                return m_sizeRatio;
            }

            set
            {
                m_sizeRatio = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of linear pointers.
        /// </summary>
        /// <value>
        /// Type: <see cref="LinearPointersCollection"/>
        /// </value>
        public LinearPointersCollection Pointers
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
        /// Gets or sets the length between the scale size and gauge size.
        /// </summary>
        internal double ScaleWidth
        {
            get
            {
                return m_scaleWidth;
            }

            set
            {
                m_scaleWidth = value;
            }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value>
        /// Type: <see cref="int"/>
        /// Returns the Visual Children count.
        /// </value>
        protected override int VisualChildrenCount
        {
            get
            {
                return this.Ticks.Count + this.Ranges.Count + this.Pointers.Count;
            }
        }
        #endregion CLR Getters & Setters

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="Orientation"/> property is changed.
        /// </summary>
        protected internal event PropertyChangedCallback OrientationChanged;

        /// <summary>
        /// Event that is raised when <see cref="ScaleBarLength"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ScaleBarLengthChanged;

        /// <summary>
        /// Event that is raised when <see cref="ScaleStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ScaleStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="ScaleCustomGeometry"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ScaleCustomGeometryChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(GaugeOrientation), typeof(LinearScale), new FrameworkPropertyMetadata(GaugeOrientation.Vertical, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Identifies the <see cref="RadiusX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(double), typeof(LinearScale), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Identifies the <see cref="RadiusY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(double), typeof(LinearScale), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Identifies the <see cref="ScaleBarLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleBarLengthProperty =
            DependencyProperty.Register("ScaleBarLength", typeof(double), typeof(LinearScale), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnScaleBarLengthChanged)));

        /// <summary>
        /// Identifies the <see cref="ScaleStyleProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleStyleProperty = DependencyProperty.Register("ScaleStyle", typeof(LinearScaleStyle), typeof(LinearScale), new FrameworkPropertyMetadata(LinearScaleStyle.Rectangle, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnScaleStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="ScaleCustomGeometryProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleCustomGeometryProperty = DependencyProperty.Register("ScaleCustomGeometry", typeof(Geometry), typeof(LinearScale), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnScaleCustomGeometryChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the orientation of the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="GaugeOrientation"/>
        /// Default value is GaugeOrientation.Horizontal.
        /// </value>
        protected internal GaugeOrientation Orientation
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
        /// Gets or sets the corner radius(RadiusX) for Rounded Rectangle Scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0d.
        /// </value>
        /// <seealso cref="RadiusY"/>
        public double RadiusX
        {
            get
            {
                return (double)GetValue(RadiusXProperty);
            }

            set
            {
                SetValue(RadiusXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the corner radius(RadiusY) for Rounded Rectangle Scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0d.
        /// </value>
        /// <seealso cref="RadiusX"/>
        public double RadiusY
        {
            get
            {
                return (double)GetValue(RadiusYProperty);
            }

            set
            {
                SetValue(RadiusYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="ScaleBase.ScaleBarSize"/>
        public double ScaleBarLength
        {
            get
            {
                return (double)GetValue(ScaleBarLengthProperty);
            }

            set
            {
                SetValue(ScaleBarLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the linear scale style.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="LinearScaleStyle"/>
        /// Default value is Rectangle.
        /// </value>
        public LinearScaleStyle ScaleStyle
        {
            get
            {
                return (LinearScaleStyle)GetValue(ScaleStyleProperty);
            }

            set
            {
                SetValue(ScaleStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a custom geometry for the Scale bar.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Geometry"/>
        /// Default value is null.
        /// </value>
        public Geometry ScaleCustomGeometry
        {
            get
            {
                return (Geometry)GetValue(ScaleCustomGeometryProperty);
            }

            set
            {
                SetValue(ScaleCustomGeometryProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="LinearScale"/> class.
        /// Overrides some dependency properties.
        /// </summary>
        static LinearScale()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinearScale), new FrameworkPropertyMetadata(typeof(LinearScale)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearScale"/> class.
        /// </summary>
        public LinearScale()
        {
            this.Loaded += new RoutedEventHandler(LinearScaleLoaded);

            this.Ticks = new TicksCollection(this);
            this.Ranges = new RangesCollection(this);
            this.Pointers = new LinearPointersCollection(this);

            this.Ticks.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            this.Ranges.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            this.Pointers.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
        }
        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            int visualChildrenCount = this.VisualChildrenCount;
            for (int i = 0; i < visualChildrenCount; i++)
            {
                (this.GetVisualChild(i) as UIElement).Measure(availableSize);
            }

            if (GaugeActualWidth != 0 && GaugeActualHeight != 0)
            {
                if (this.Orientation == GaugeOrientation.Vertical)
                {
                    if (this.ScaleBarLength > GaugeActualHeight)
                    {
                        // this.ScaleBarLength = GaugeActualHeight;
                    }

                    if (this.ScaleBarSize > GaugeActualWidth)
                    {
                        // this.ScaleBarSize = GaugeActualWidth;
                    }
                }
            }

            return new Size(this.ScaleBarSize, this.ScaleBarLength);
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        /// <remarks>Linear Scale provides all of its child elements with rectangular space of their
        /// DesiredWidth and DesiredHeight,which starts exactly at the center of the LinearScale. 
        /// All of its children render themselves at the appropriate positions by using transforms.
        /// </remarks>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int visualChildrenCount = this.VisualChildrenCount;
            for (int i = 0; i < visualChildrenCount; i++)
            {
                FrameworkElement el = this.GetVisualChild(i) as FrameworkElement;

                el.Arrange(new Rect(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2, el.DesiredSize.Width, el.DesiredSize.Height));
            }

            if (this.ScaleBarSize >= 0 && this.ScaleBarLength >= 0)
            {
                if (this.ScaleStyle == LinearScaleStyle.Thermometer)
                {
                    m_scalePath = new PathGeometry();
                    double bpointerWidth = 0;
                    foreach (Visual element in this.Pointers)
                    {
                        if (element is LinearBarPointer)
                        {
                            LinearBarPointer elem = element as LinearBarPointer;
                            bpointerWidth = this.ScaleBarSize;
                        }
                    }

                    double bulbStart = (this.ScaleBarSize - bpointerWidth) / 2;
                    double bulbEnd = (this.ScaleBarSize + bpointerWidth) / 2;

                    Point topLeft = new Point(0, 0);
                    Point topRight = new Point(this.ScaleBarSize, 0);
                    Point bottomRight = new Point(this.ScaleBarSize, this.ScaleBarLength);
                    Point bottomCenter1 = new Point(bulbEnd, this.ScaleBarLength);
                    Point bottomCenter2 = new Point(bulbStart, this.ScaleBarLength);
                    Point bottomLeft = new Point(0, this.ScaleBarLength);

                    LineSegment tl = new LineSegment(topLeft, true);
                    LineSegment tr = new LineSegment(topRight, true);
                    LineSegment br = new LineSegment(bottomRight, true);
                    LineSegment bc1 = new LineSegment(bottomCenter1, true);
                    ArcSegment abc2 = new ArcSegment(bottomCenter2, new Size(bpointerWidth, bpointerWidth), 0, true, SweepDirection.Clockwise, true);
                    LineSegment bc2 = new LineSegment(bottomCenter2, true);
                    LineSegment bl = new LineSegment(bottomLeft, true);
                    PathFigure figure1 = new PathFigure(
                        new Point(0, 0),
                        new PathSegment[] { tr, br, bc1, abc2, bl, tl },
                        false);
                    (m_scalePath as PathGeometry).Figures.Add(figure1);
                }
                else
                {
                    m_scalePath = new RectangleGeometry();
                    (m_scalePath as RectangleGeometry).Rect = new Rect(this.Orientation == GaugeOrientation.Vertical ? 0 : -this.ScaleBarLength / 2 + this.ScaleBarSize / 2, this.Orientation == GaugeOrientation.Vertical ? 0 : this.ScaleBarLength / 2 - this.ScaleBarSize / 2, this.Orientation == GaugeOrientation.Vertical ? this.ScaleBarSize : this.ScaleBarLength, this.Orientation == GaugeOrientation.Vertical ? this.ScaleBarLength : this.ScaleBarSize);
                    // m_scalePath.Transform = new RotateTransform(-180, 0,0);
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
            base.OnRender(drawingContext);

            if (ScaleStyle == LinearScaleStyle.RoundedRectangle)
            {
                Rect rect = (m_scalePath as RectangleGeometry).Rect;
                drawingContext.DrawRoundedRectangle(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), rect, RadiusX, RadiusY);
            }
            else if (ScaleStyle == LinearScaleStyle.Thermometer)
            {
                drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_scalePath);
            }
            else
            {
                if (ScaleStyle == LinearScaleStyle.Custom)
                {
                    if (ScaleCustomGeometry == null)
                    {
                        drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_scalePath);
                    }
                    else
                    {
                        m_scalePath = ScaleCustomGeometry;
                        drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_scalePath);
                    }
                }
                else
                {
                    drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_scalePath);
                }

            }

            this.RefreshPointers();
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">he zero-based index of the requested child element in the collection.</param>
        /// <returns>The requested child element.</returns>
        /// <remarks>The arrangement is in such a way that Ranges are arranged first, then the Ticks(i.e above
        /// the Ranges) and then the Pointers(i.e above the Ticks and ranges) and hence the pointer are 
        /// always at the top</remarks>
        protected override Visual GetVisualChild(int index)
        {
            FrameworkElement elem = null;

            if (index >= 0 && index < this.Ranges.Count)
            {
                elem = this.Ranges[index];
            }
            else if (index >= this.Ranges.Count && index < this.Ranges.Count + this.Ticks.Count)
            {
                elem = this.Ticks[index - this.Ranges.Count];
            }
            else
            {
                int countElems = this.Ticks.Count + this.Ranges.Count;
                if (index >= countElems && index < countElems + this.Pointers.Count)
                {
                    elem = this.Pointers[index - countElems];
                }
            }

            return elem;
        }

        /// <summary>
        /// Invoked when <see cref="ScaleBase.Maximum"/> property is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnMaximumChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnMaximumChanged(e);
            this.RefreshPointers();

            // foreach( CircularPointer pointer in this.Pointers )
            // {
            //    if( pointer.Value > this.Maximum )
            //    {
            //        pointer.Value = this.Maximum;
            //    }
            // }
        }

        /// <summary>
        /// Invoked when <see cref="ScaleBase.Minimum"/> property is changed.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnMinimumChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnMinimumChanged(e);
            this.RefreshPointers();

            // foreach( LinearPointer pointer in this.Pointers )
            // {
            //    if( pointer.Value < this.Minimum )
            //    {
            //        pointer.Value = this.Minimum;
            //    }
            // }
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Gets the position according to the value.
        /// </summary>
        /// <param name="value">Value whose position to be found</param>
        /// <remarks>
        /// <list type="bullet">
        ///     <listheader>The local variables used</listheader>
        ///     <item>
        ///         <term>ratio</term>
        ///         <description>stores the number of segments(relative to Maximum and Minimum values),
        ///         that can be formed,each of "value - Minimum" length.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>return</term>
        ///         <description>returns the length of one such segment(relative to ScaleBarLength),
        ///          since that is the actual position of the visual
        ///         </description>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// Returns the actual position.
        /// </returns>
        internal double GetPositionByPointerValue(double value)
        {
            if (value > this.Maximum)
            {
                value = this.Maximum;
            }

            if (value < this.Minimum)
            {
                value = this.Minimum;
            }

            double ratio = (this.Maximum - this.Minimum) / (value - this.Minimum);
            return this.ScaleBarLength / ratio;
        }

        internal double GetPositionByValue(double value)
        {
            if (value > this.Maximum)
            {
                value = this.Maximum;
            }

            if (value < this.Minimum)
            {
                value = this.Minimum;
            }
            double ratio = 0;
            if (this.ScaleDirection == ScaleDirection.Clockwise)
            {
                ratio = (this.Maximum - this.Minimum) / (this.Maximum - value);
            }
            else
            {
                ratio = (this.Maximum - this.Minimum) / (value - this.Minimum);
            }

            return this.ScaleBarLength / ratio;
        }


        /// <summary>
        /// Occurs when the scale is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void LinearScaleLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshPointers();
        }

        /// <summary>
        /// Calls OnOrientationChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScale instance = (LinearScale)d;
            instance.InvalidateVisual();
            instance.OnOrientationChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises OrientationChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            this.RefreshPointers();
            if (OrientationChanged != null)
            {
                this.OrientationChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises ScaleBarLengthChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScaleBarLengthChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (ScaleBarLengthChanged != null)
            {
                this.ScaleBarLengthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnScaleBarLengthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScaleBarLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScale instance = (LinearScale)d;
            instance.OnScaleBarLengthChanged(e);
        }

        /// <summary>
        /// Calls OnScaleStyle method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScaleStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScale instance = (LinearScale)d;
            instance.OnScaleStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises OnScaleStyleChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScaleStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ScaleStyleChanged != null)
            {
                this.ScaleStyleChanged(this, e);
            }
        }
        /// <summary>
        /// Updates property value cache and raises OnScaleDirectionChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnScaleDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnScaleDirectionChanged(e);
            this.RefreshPointers();
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

        }

        /// <summary>
        /// Calls OnScaleStyle method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScaleCustomGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearScale instance = (LinearScale)d;
            instance.OnScaleCustomGeometryChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises OnScaleCustomGeometryChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScaleCustomGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ScaleCustomGeometryChanged != null)
            {
                this.ScaleCustomGeometryChanged(this, e);
            }
        }

        /// <summary>
        /// Refreshes pointers position according to the values.
        /// </summary>
        internal void RefreshPointers()
        {
            foreach (LinearPointer pointer in this.Pointers)
            {
                pointer.Position = this.GetPositionByPointerValue(pointer.Value);
                pointer.RefreshPointerPosition();
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
        /// This method is invoked during collection changed.
        /// </summary>
        /// <param name="gauge">The <see cref="LinearGauge"/> that contains the reference to the Gauge.</param>
        internal void SetScope(LinearGauge gauge)
        {
            gaugeReference = gauge;
            CalculateScope(gauge);
        }

        /// <summary>
        /// Calculates the scope. 
        /// This method is invoked during collection changed.
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
        /// <remarks> Adds LabelTicks, MarkTicks, Pointers and Ranges as LinearScale's Visual Children</remarks>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Visual element in e.NewItems)
                {
                    if (element is LinearBarPointer)
                    {
                        LinearBarPointer elem = element as LinearBarPointer;
                        elem.SetScope(gaugeReference);
                    }
                    else if (element is LinearMarkerPointer)
                    {
                        LinearMarkerPointer elem = element as LinearMarkerPointer;
                        elem.SetScope(gaugeReference);
                    }
                    else if (element is LinearLabelTick)
                    {
                        LinearLabelTick elem = element as LinearLabelTick;
                        elem.SetScope(gaugeReference);
                    }
                    else if (element is LinearMarkTick)
                    {
                        LinearMarkTick elem = element as LinearMarkTick;
                        elem.SetScope(gaugeReference);
                    }
                    else if (element is LinearRange)
                    {
                        LinearRange elem = element as LinearRange;
                        elem.SetScope(gaugeReference);
                    }

                    if (!(element is CircularRange))
                    {
                        this.AddVisualChild(element);
                    }

                }

            }
        }
        #endregion Added Code
    }
}