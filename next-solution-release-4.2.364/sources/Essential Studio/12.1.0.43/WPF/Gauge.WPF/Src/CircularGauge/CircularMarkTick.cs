// <copyright file="CircularMarkTick.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Syncfusion.Licensing;
using System.Diagnostics;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the Tick of the circular scale. The Major/Minor ticks are set by
    /// using the <see cref="TickStyle"/> property.
    /// </summary>
    /// <remarks>
    /// The Ticks are of two types, Major and Minor Tick, which can be set by using the <see cref="TickStyle"/>
    /// property. <see cref="TickStyle.MajorTick"/> is placed at an interval specified by <see cref="ScaleBase.MajorIntervalValue"/> 
    /// and <see cref="TickStyle.MinorTick"/> is placed at an interval specified
    /// by <see cref="ScaleBase.MinorIntervalValue"/> property.
    /// </remarks>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="CircularMarkTickSample.Window1" Title="CircularMarkTickSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <syncfusion:CircularGauge Name="circularGauge">
    ///         <syncfusion:CircularGauge.Scales>
    ///             <syncfusion:CircularScale Name="circularScale" Radius="100"
    ///                                       Minimum="0" Maximum="100" MajorIntervalValue="10"
    ///                                       MinorIntervalValue="2"
    ///                                       ScaleBarSize="10" GapSweepAngle="310" StartAngle="110"
    ///                                       BackgroundBrush="LightBlue">
    ///                 <syncfusion:CircularScale.Ticks>
    ///                     <syncfusion:CircularLabelTick FontSize="15" TickStyle="MajorTick" 
    ///                                                   BackgroundBrush="GhostWhite" Name="CircularLabelTick1"
    ///                                                   TickPlacement="Inside" DistanceFromScale="5" />
    ///                     <syncfusion:CircularMarkTick TickHeight="9" 
    ///                                                  TickShape="Rectangle" TickStyle="MajorTick"
    ///                                                  Name="CircularMajorTick1" TickPlacement="Inside" 
    ///                                                  TickWidth="2" BackgroundBrush="White" />
    ///                     <syncfusion:CircularMarkTick TickHeight="4" TickWidth="1" TickStyle="MinorTick" 
    ///                                                  Name="CircularMinorTick1" BackgroundBrush="White"
    ///                                                  TickPlacement="Inside" />
    ///                 </syncfusion:CircularScale.Ticks>
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
    /// namespace CircularMarkTickSample
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
    ///             CircularMarkTick m_majortick = new CircularMarkTick();
    ///             m_majortick.TickWidth = 4;
    ///             m_majortick.TickHeight = 9;
    ///             m_majortick.TickPlacement = ScalePlacement.Inside;
    ///             m_majortick.TickStyle = TickStyle.MajorTick;
    ///             m_majortick.BackgroundBrush = Brushes.White;
    ///             m_majortick.TickShape = TickShape.Ellipse; 
    ///             CircularMarkTick m_minortick = new CircularMarkTick();
    ///             m_minortick.TickWidth = 1;
    ///             m_minortick.TickHeight = 4;
    ///             m_minortick.TickPlacement = ScalePlacement.Inside;
    ///             m_minortick.TickStyle = TickStyle.MinorTick;
    ///             m_minortick.BackgroundBrush = Brushes.White;
    ///             m_scale.Ticks.Add(m_minortick);
    ///             m_scale.Ticks.Add(m_majortick);
    ///             this.Content = m_gauge;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CircularMarkTick : TickBase
    {

        #region Private Members
        /// <summary>
        /// Rect object used to draw the tick.
        /// </summary>
        private Rect m_rect;
        #endregion Private Members

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="TickShape"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TickShapeChanged;

        #endregion Events

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(CircularMarkTick));

        /// <summary>
        /// Identifies the <see cref="TickHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickHeightProperty =
            DependencyProperty.Register("TickHeight", typeof(double), typeof(CircularMarkTick), new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="TickShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickShapeProperty =
            DependencyProperty.Register("TickShape", typeof(TickShape), typeof(CircularMarkTick), new FrameworkPropertyMetadata(TickShape.Rectangle, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnTickShapeChanged)));

        /// <summary>
        /// Identifies the <see cref="TickWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickWidthProperty =
            DependencyProperty.Register("TickWidth", typeof(double), typeof(CircularMarkTick), new FrameworkPropertyMetadata(0d));
        #endregion Dependency Properties

        #region DP Getters & Setters

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            set { SetValue(ValueProperty, value); }
            get { return (double)GetValue(ValueProperty); }
        }

        /// <summary>
        /// Gets or sets the height of the mark tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the shape of the mark tick.
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
        /// Gets or sets the width of the mark tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="CircularMarkTick"/> class.
        /// Overrides the meta-data for default template.
        /// </summary>
        static CircularMarkTick()
        {
            EnvironmentTest.ValidateLicense(typeof(CircularMarkTick));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularMarkTick), new FrameworkPropertyMetadata(typeof(CircularMarkTick)));
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
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches an element in its route that is derived from this
        /// class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that
        /// contains the event data.</param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.VisualParent is ScaleBase)
            {
                CircularScale scale = this.VisualParent as CircularScale;
                Point p = e.GetPosition(scale);
                double num, deno;
                num = p.Y - scale.Radius;
                deno = p.X - scale.Radius;
                double m = num / deno;
                double tan = (180 * Math.Atan(m)) / Math.PI;
                if (deno < 0)
                {
                    tan = 180 + tan;
                }
                this.Value = this.AngleToValue(tan, this.Value);

                //CircularScale scale = this.VisualParent as CircularScale;
                double angleToPlace = scale.ScaleDirection == ScaleDirection.Clockwise ? scale.StartAngle : scale.StartAngle + scale.GapSweepAngle;
                double interval = 0;
                double ratio = 0d;
                if (this.TickStyle == TickStyle.MajorTick)
                {
                    interval = interval = scale.IsNumberDivision ? (scale.Maximum - scale.Minimum) / scale.NumberMajorDivision : scale.MajorIntervalValue;
                    ratio = (scale.Maximum - scale.Minimum) / interval;
                }
                else
                {
                    interval = scale.IsNumberDivision ? scale.NumberMajorDivision * scale.NumberMinorDivision : scale.MinorIntervalValue;
                    ratio = scale.IsNumberDivision ?interval : (scale.Maximum - scale.Minimum) / interval;
                }

                
                double angleInterval = scale.GapSweepAngle / ratio;
                double currentValue = scale.Minimum;

                if (angleInterval > 0)
                {
                    while (currentValue <= scale.Maximum)
                    {
                        if (this.Value > currentValue && this.Value < (currentValue + interval))
                        {
                            if ((this.Value - currentValue) < ((currentValue + interval) - this.Value))
                                this.Value = currentValue;
                            else
                                this.Value = currentValue + interval;
                            break;
                        }
                        currentValue += interval;
                    }
                }
            }
        }

        internal double AngleToValue(double angle, double pointervalue)
        {
            double value = 0;
            CircularScale scale = this.VisualParent as CircularScale;
            if (scale != null)
            {
                if (scale.ScaleDirection == ScaleDirection.CounterClockwise)
                {
                    if (angle < scale.StartAngle && angle > (scale.StartAngle + scale.GapSweepAngle - 360))
                    {
                        if (angle < scale.StartAngle) //&& this.AnglePosition > scale.StartAngle + scale.GapSweepAngle)
                        {
                            angle = scale.StartAngle;
                            value = scale.Minimum;
                            this.Value = scale.Minimum;
                        }
                        else if (angle < scale.StartAngle)// && this.AnglePosition <= (scale.StartAngle + scale.GapSweepAngle - 360))
                        {
                            angle = scale.StartAngle + scale.GapSweepAngle - 360 + 10;
                            value = scale.Maximum;
                            this.Value = scale.Maximum;
                        }
                        else
                        {
                            return pointervalue;
                        }
                    }
                    else
                    {
                        if (angle < scale.StartAngle)
                        {
                            angle = angle + 360;
                        }

                        double ratio = scale.GapSweepAngle / (angle - scale.StartAngle);
                        value = scale.Maximum - (((scale.Maximum - scale.Minimum) / ratio) + scale.Minimum);
                    }
                }
                else
                {
                    if (angle < scale.StartAngle && angle > (scale.StartAngle + scale.GapSweepAngle - 360))
                    {
                        if (angle < scale.StartAngle)// && this.AnglePosition > scale.StartAngle + scale.GapSweepAngle)
                        {
                            angle = scale.StartAngle;
                            value = scale.Minimum;
                            this.Value = scale.Minimum;
                        }
                        else if (angle < scale.StartAngle)// && this.AnglePosition <= (scale.StartAngle + scale.GapSweepAngle - 360))
                        {
                            angle = scale.StartAngle + scale.GapSweepAngle - 360 + 10;
                            value = scale.Maximum;
                            this.Value = scale.Maximum;
                        }
                        else
                        {
                            return pointervalue;
                        }
                    }
                    else
                    {
                        if (angle < scale.StartAngle)
                        {
                            angle = angle + 360;
                        }

                        double ratio = scale.GapSweepAngle / (angle - scale.StartAngle);
                        value = ((scale.Maximum - scale.Minimum) / ratio) + scale.Minimum;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.VisualParent is CircularScale)
            {
                m_rect = new Rect(0, 0, this.TickHeight, this.TickWidth);
                this.InvalidateVisual();
            }

            return finalSize;
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <remarks>
        /// Number of Segments that are possible of &quot;interval&quot; length is
        /// calculated by the variable ratio. Angleinterval calculates the angle interval
        /// between two such segments. As each tick is placed the angleToPlace and i
        /// are incremented by angleinterval and interval respectively. The 
        /// ticks are then rotated to the appropriate angle and then to the appropriate 
        /// location(based on ScalePlacement).
        /// </remarks>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (this.VisualParent is CircularScale)
            {
                CircularScale scale = this.VisualParent as CircularScale;
                double angleToPlace = scale.ScaleDirection == ScaleDirection.Clockwise ? scale.StartAngle : scale.StartAngle + scale.GapSweepAngle;
                double interval = 0;
                double ratio = 0d;
                if (this.TickStyle == TickStyle.MajorTick)
                {
                    interval = scale.IsNumberDivision ? (scale.Maximum - scale.Minimum) / scale.NumberMajorDivision : scale.MajorIntervalValue;
                    ratio = (scale.Maximum - scale.Minimum) / interval;
                }
                else
                {
                    interval = scale.IsNumberDivision ? scale.NumberMajorDivision * scale.NumberMinorDivision: scale.MinorIntervalValue;
                    ratio = scale.IsNumberDivision ? interval:(scale.Maximum - scale.Minimum) / interval;
                }

                
                double angleInterval = scale.GapSweepAngle / ratio;
                double currentValue = scale.Minimum;
                Brush currentBrush = this.BackgroundBrush;

                if (angleInterval > 0)
                {
                    double k = Math.Ceiling(ratio) - ratio;
                    if (k > 0.5)
                    {
                        k = ratio - Math.Floor(ratio);
                    }

                    while (ratio >= k)
                    {
                        ratio--;
                        double posX = 0;
                        if (this.TickPlacement == ScalePlacement.Cross)
                        {
                            posX = ((scale.ScaleBarSize + this.DesiredSize.Width) / 2) - scale.Radius - this.DistanceFromScale;
                        }
                        else if (this.TickPlacement == ScalePlacement.Inside)
                        {
                            posX = this.DesiredSize.Width;
                            if (scale.Radius > scale.ScaleBarSize)
                            {
                                posX += scale.ScaleBarSize - scale.Radius + this.DistanceFromScale;
                                //posX += scale.ScaleBarSize - scale.Radius - this.DistanceFromScale;
                            }
                        }
                        else if (this.TickPlacement == ScalePlacement.Outside)
                        {
                            posX = -scale.Radius - this.DistanceFromScale;
                        }

                        TransformGroup transform = new TransformGroup();
                        transform.Children.Add(new RotateTransform(this.Angle, this.DesiredSize.Width / 2, this.DesiredSize.Height / 2));
                        transform.Children.Add(new RotateTransform(angleToPlace, posX, this.DesiredSize.Height / 2));
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

                        drawingContext.PushTransform(transform);
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

                        if (scale.ScaleDirection == ScaleDirection.Clockwise)
                            angleToPlace += angleInterval;
                        else
                            angleToPlace -= angleInterval;
                        currentValue += interval;
                    }
                }
            }
        }
        #endregion Overrides

        #region Implementation

        /// <summary>
        /// Updates property value cache and raises <see cref="TickShapeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTickShapeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TickShapeChanged != null)
            {
                TickShapeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTickShapeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularMarkTick instance = (CircularMarkTick)d;
            instance.OnTickShapeChanged(e);
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
            if (gauge != null)
            {
                CalculateScope(gauge);
                if (gauge.MinorTickStyle != null)
                    this.Style = gauge.MajorTickStyle;
            }
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
