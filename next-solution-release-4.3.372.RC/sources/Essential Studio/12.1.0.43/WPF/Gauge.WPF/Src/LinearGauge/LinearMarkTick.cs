// <copyright file="LinearMarkTick.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the Major/Minor tick of the linear scale. The Major/Minor ticks are set by
    /// using the <see cref="TickStyle"/> property.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="LinearMarkTickSample.Window1" Title="LinearMarkTickSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:LinearGauge CenterFrameFillColor="Brown" Name="linearGauge1">
    ///             <syncfusion:LinearGauge.Scales>
    ///                 <syncfusion:LinearScale Name="LinearScale" Minimum="0" Maximum="100" 
    ///                                         MinorIntervalValue="2" MajorIntervalValue="10" 
    ///                                         ScaleBarSize="20" ScaleBarLength="260">
    ///                     <syncfusion:LinearScale.Ticks>
    ///                         <syncfusion:LinearMarkTick TickHeight="9" TickShape="Rectangle" 
    ///                                                    TickStyle="MajorTick" TickWidth="4" 
    ///                                                    BackgroundBrush="Pink"/>
    ///                         <syncfusion:LinearMarkTick TickHeight="4" TickWidth="1" TickStyle="MinorTick" 
    ///                                                    BackgroundBrush="Aqua" />
    ///                     </syncfusion:LinearScale.Ticks> 
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
    /// namespace LinearMarkTickSample
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
    ///                 LinearMarkTick markTick1 = new LinearMarkTick();
    ///                 markTick1.TickHeight = 9;
    ///                 markTick1.TickShape = TickShape.Rectangle;
    ///                 markTick1.TickStyle = TickStyle.MajorTick;
    ///                 markTick1.TickWidth = 4;
    ///                 markTick1.BackgroundBrush = new SolidColorBrush(Colors.Pink);
    ///                 scale.Ticks.Add(markTick1);
    ///                 LinearMarkTick markTick2 = new LinearMarkTick();
    ///                 markTick2.TickHeight = 4;
    ///                 markTick2.TickWidth = 1;
    ///                 markTick2.TickStyle = TickStyle.MinorTick;
    ///                 markTick2.BackgroundBrush = new SolidColorBrush(Colors.White);
    ///                 scale.Ticks.Add(markTick2);<para/>
    ///                 this.Content = linearGauge1;
    ///             }
    ///         }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LinearMarkTick : TickBase
    {
        #region Private Members
        /// <summary>
        /// Rect object used to draw the tick.
        /// </summary>
        private Rect m_rect;
        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double m_startsizeRatio;

        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double m_endsizeRatio;

        #endregion Private Members

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="TickHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TickHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="TickShape"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TickShapeChanged;

        /// <summary>
        /// Event that is raised when <see cref="TickWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TickWidthChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="TickHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickHeightProperty =
            DependencyProperty.Register("TickHeight", typeof(double), typeof(LinearMarkTick), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnTickHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="TickShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickShapeProperty =
            DependencyProperty.Register("TickShape", typeof(TickShape), typeof(LinearMarkTick), new FrameworkPropertyMetadata(TickShape.Rectangle, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnTickShapeChanged)));

        /// <summary>
        /// Identifies the <see cref="TickWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickWidthProperty =
            DependencyProperty.Register("TickWidth", typeof(double), typeof(LinearMarkTick), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnTickWidthChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the height of the mark tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="TickWidth"/>
        public double TickHeight
        {
            get
            {
                return (double)GetValue(TickHeightProperty);
            }

            set
            {
                SetValue(TickHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the mark tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="TickHeight"/>
        public double TickWidth
        {
            get
            {
                return (double)GetValue(TickWidthProperty);
            }

            set
            {
                SetValue(TickWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the shapes of the mark tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="TickShape"/>
        /// Default value is TickShape.Rectangle.
        /// </value>
        public TickShape TickShape
        {
            get
            {
                return (TickShape)GetValue(TickShapeProperty);
            }

            set
            {
                SetValue(TickShapeProperty, value);
            }
        }
                 

        /// <summary>
        /// Gets or sets the ratio of ScaleBarSize to that of LinearGauge's Width.
        /// </summary>
        internal double startSizeRatio
        {
            get
            {
                return m_startsizeRatio;
            }

            set
            {
                m_startsizeRatio = value;
            }
        }
        /// <summary>
        /// Gets or sets the ratio of ScaleBarSize to that of LinearGauge's Width.
        /// </summary>
        internal double endSizeRatio
        {
            get
            {
                return m_endsizeRatio;
            }

            set
            {
                m_endsizeRatio = value;
            }
        }


        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="LinearMarkTick"/> class.
        /// Overrides the meta data for default template.
        /// </summary>
        static LinearMarkTick()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinearMarkTick), new FrameworkPropertyMetadata(typeof(LinearMarkTick)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearMarkTick"/> class.
        /// </summary>
        public LinearMarkTick()
        {
        }
        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(this.TickHeight, this.TickWidth);
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.VisualParent is LinearScale)
            {
                // Ticks are drawn in horizontal position.
                m_rect = new Rect(0, 0, this.TickHeight, this.TickWidth);
                this.InvalidateVisual();
            }

            return finalSize;
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        /// <remarks>
        /// <list type="bullet">
        ///     <listheader>The local variables used</listheader>
        ///     <item>
        ///         <term>ratio</term>
        ///         <description>stores the number of segments(relative to Maximum and Minimum values),
        ///         that can be formed,each of "valueInterval" length.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>interval</term>
        ///        <description>Stores the length of one such segment(relative to ScaleBarLength 
        ///         deducting YDistanceFromScale.
        ///         </description>
        ///     </item>
        /// </list>
        /// <list type="bullet">
        ///     <listheader>The transforms used</listheader>
        ///     <item>The ticks arranged at the Center of the Gauge are first transformed, to the angle set.</item>
        ///     <item>Then the ticks are moved to the appropriate position from the center.</item>
        ///     <item>The positioning of tick starts from the top of scale.</item>
        /// </list>
        /// </remarks>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (this.VisualParent is LinearScale)
            {
                LinearScale scale = this.VisualParent as LinearScale;
                double i = 0;
                double valueInterval = 0;
                double ratio = 0d;
                if (this.TickStyle == TickStyle.MajorTick)
                {
                    valueInterval = scale.IsNumberDivision ? (scale.Maximum - scale.Minimum) / scale.NumberMajorDivision : scale.MajorIntervalValue;
                    ratio = (scale.Maximum - scale.Minimum) / valueInterval;
                }
                else
                {
                    valueInterval = scale.IsNumberDivision ? scale.NumberMajorDivision * scale.NumberMinorDivision : scale.MinorIntervalValue;
                    ratio = scale.IsNumberDivision? valueInterval:(scale.Maximum - scale.Minimum) / valueInterval;
                }

                
                double interval = (scale.ScaleBarLength - this.YDistanceFromScale) / ratio;

                double currentValue;
                if (scale.ScaleDirection == ScaleDirection.CounterClockwise)
                    currentValue = scale.Maximum;
                else
                    currentValue = scale.Minimum;

                Brush currentBrush = this.BackgroundBrush;

                if (interval > 0)
                {
                    i = -this.TickWidth / 2;
                    while (i <= (scale.ScaleBarLength - this.YDistanceFromScale))
                    {
                        double posX = 0;
                        if (this.TickPlacement == ScalePlacement.Cross)
                        {
                            posX = (-this.TickHeight / 2) - this.DistanceFromScale;
                        }
                        else if (this.TickPlacement == ScalePlacement.Inside)
                        {
                            posX = (-scale.ScaleBarSize / 2) - this.TickHeight - this.DistanceFromScale;
                        }
                        else if (this.TickPlacement == ScalePlacement.Outside)
                        {
                            posX = (scale.ScaleBarSize / 2) + this.DistanceFromScale;
                        }

                        TransformGroup transform = new TransformGroup();
                        transform.Children.Add(new RotateTransform(this.Angle, this.DesiredSize.Width / 2, this.DesiredSize.Height / 2));
                        if (scale.Orientation == GaugeOrientation.Vertical)
                        {
                            transform.Children.Add(new TranslateTransform(posX, scale.ScaleDirection == ScaleDirection.CounterClockwise ? ((-(scale.ScaleBarLength - this.YDistanceFromScale) / 2) + i) : (-(scale.ScaleBarLength - this.YDistanceFromScale) / 2) + i));
                        }
                        else
                        {
                            transform.Children.Add(new RotateTransform(-90d, this.DesiredSize.Width / 2, this.DesiredSize.Width / 2));
                            transform.Children.Add(new TranslateTransform(0, this.DesiredSize.Height / 2));

                            transform.Children.Add(new TranslateTransform(scale.ScaleDirection == ScaleDirection.CounterClockwise ? ((-(scale.ScaleBarLength - this.YDistanceFromScale) / 2) + i) : (-(scale.ScaleBarLength - this.YDistanceFromScale) / 2) + i, posX));
                        }
                        drawingContext.PushTransform(transform);

                        if (this.RangedBrush != null)
                        {
                            if (currentValue >= RangedBrushStartValue && currentValue <= RangedBrushEndValue)
                            {
                                currentBrush = this.RangedBrush;
                            }
                            else
                            {
                                currentBrush = this.BackgroundBrush;
                            }
                        }

                        if (this.TickShape == TickShape.Rectangle)
                        {
                            drawingContext.DrawRectangle(currentBrush, new Pen(this.BorderBrush, this.BorderWidth), m_rect);
                        }
                        else if (this.TickShape == TickShape.RoundedRectangle)
                        {
                            drawingContext.DrawRoundedRectangle(currentBrush, new Pen(this.BorderBrush, this.BorderWidth), m_rect, m_rect.Height / 3, m_rect.Height / 3);
                        }
                        else if (this.TickShape == TickShape.Ellipse)
                        {
                            drawingContext.DrawEllipse(currentBrush, new Pen(this.BorderBrush, this.BorderWidth), new Point(this.DesiredSize.Width / 2, this.DesiredSize.Height / 2), m_rect.Width / 2, m_rect.Height / 2);
                        }
                        else if (this.TickShape == TickShape.Triangle)
                        {
                            PathGeometry trianglePath = new PathGeometry();
                            LineSegment l1 = new LineSegment(new Point(0, m_rect.Height / 2), true);
                            LineSegment l2 = new LineSegment(new Point(m_rect.Width, 0), true);
                            LineSegment l3 = new LineSegment(new Point(m_rect.Width, m_rect.Height), true);
                            LineSegment l4 = new LineSegment(new Point(0, m_rect.Height / 2), true);
                            PathFigure figure1 = new PathFigure(
                                new Point(0, m_rect.Height / 2),
                                new PathSegment[] { l1, l2, l3, l4 },
                                true);
                            trianglePath.Figures.Add(figure1);
                            drawingContext.DrawGeometry(currentBrush, new Pen(this.BorderBrush, this.BorderWidth), trianglePath);
                        }

                        drawingContext.Pop();
                        // if (scale.ScaleDirection == ScaleDirection.Clockwise)
                        i += interval;
                        // else
                        //    i -= interval;
                        if (scale.ScaleDirection == ScaleDirection.CounterClockwise)
                            currentValue -= valueInterval;
                        else
                            currentValue += valueInterval;
                    }
                }
            }
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Calls OnTickHeightChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearMarkTick instance = (LinearMarkTick)d;
            instance.OnTickHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TickHeightChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTickHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TickHeightChanged != null)
            {
                this.TickHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TickShapeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTickShapeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TickShapeChanged != null)
            {
                this.TickShapeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTickShapeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearMarkTick instance = (LinearMarkTick)d;
            instance.OnTickShapeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TickWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTickWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TickWidthChanged != null)
            {
                this.TickWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTickWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearMarkTick instance = (LinearMarkTick)d;
            instance.OnTickWidthChanged(e);
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
        /// <param name="gauge">The <see cref="LinearGauge"/> that contains the reference to the Gauge.</param>
        internal void SetScope(LinearGauge gauge)
        {
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
        #endregion Added Code
    }
}
