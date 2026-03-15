// <copyright file="CircularRange.cs" company="Syncfusion Software">
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
using System.Windows.Media.Animation;
using System.Windows.Data;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the range visual element, which spans a range of consecutive values 
    /// represented by a visual.
    /// </summary>
    /// <remarks>
    /// The Range's <see cref="RangeBase.EndValue"/> should be specified before <see cref="RangeBase.StartValue"/>
    /// because of the constraint that the <see cref="RangeBase.StartValue"/> should not be greater
    /// than <see cref="RangeBase.EndValue"/>    
    /// </remarks>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="CircularRangeSample.Window1"
    ///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///     xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
    ///     Title="CircularRangeSample" Height="400" Width="400" >
    ///     <syncfusion:CircularGauge Name="circularGauge" >
    ///         <syncfusion:CircularGauge.Scales>
    ///             <syncfusion:CircularScale Name="circularScale"  
    ///                                       Radius="100"
    ///                                       Minimum="0" Maximum="100" 
    ///                                       MajorIntervalValue="10"
    ///                                       MinorIntervalValue="2"          
    ///                                       ScaleBarSize = "10"
    ///                                       GapSweepAngle="310"
    ///                                       StartAngle="110"
    ///                                       BackgroundBrush="LightBlue"
    ///                                       >
    ///                 <syncfusion:CircularScale.Ranges>
    ///                     <syncfusion:CircularRange StartValue="80" EndValue="120"
    ///                                               StartWidth="1" 
    ///                                               EndWidth="15" BorderWidth="1"
    ///                                               BackgroundBrush="Red"
    ///                                               RangePosition="Inside"
    ///                                               DistanceFromScale="7"/>
    ///                 </syncfusion:CircularScale.Ranges>
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
    /// namespace CircularRangeSample
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
    ///             m_scale.ScaleBarSize = 1.5;
    ///             m_scale.Radius = 116;
    ///             this.m_gauge.Scales.Add(m_scale);
    ///             CircularRange range1 = new CircularRange();
    ///             range1.StartValue = 40;
    ///             range1.EndValue = 80;
    ///             range1.StartWidth = 5;
    ///             range1.EndWidth = 20;
    ///             range1.RangePosition = ScalePlacement.Inside;
    ///             range1.DistanceFromScale = 7;
    ///             m_scale.Ranges.Add(range1);
    ///             this.Content = m_gauge;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CircularRange : RangeBase
    {
        #region Private Members
        /// <summary>
        /// The geometry used to draw the range.
        /// </summary>
        private Geometry m_rangePath;
        ///// <summary>
        ///// Collection of state ranges used to turn on/off the state indicator.
        ///// </summary>
        //private RangeCollection m_ranges = new RangeCollection();
        #endregion Private Memebers

        #region CLR Getters & Setters
        ///// <summary>
        ///// Gets the Collection of state ranges used to turn on/off the state indicator.
        ///// </summary>
        ///// <value>
        ///// Type: <see cref="StateRangeCollection"/>
        ///// </value>
        //public RangeCollection Ranges
        //{
        //    get
        //    {
        //        return m_ranges;
        //    }
        //}
        #endregion CLR Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="CircularRange"/> class.
        /// Overrides the meta data for default template.
        /// </summary>
        static CircularRange()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularRange), new FrameworkPropertyMetadata(typeof(CircularRange)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularRange"/> class.
        /// </summary>
        public CircularRange()
        {

        }
        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <remarks>
        /// The startOuterRadius,middleOuterRadius,endOuterRadius, startInnerRadius,middleInnerRadius
        /// and endInnerRadius were calculated according to the appropriate ScalePlacement values
        /// and then Range's PathFigure was created.
        /// </remarks>
        /// <param name="finalSize">The final area within the parent that this element
        /// should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.IndicatorColor == null)
                this.IndicatorColor = this.BackgroundBrush;
            if (this.VisualParent is CircularScale)
            {
                CircularScale scale = this.VisualParent as CircularScale;
                double startValue = this.StartValue;
                double endValue = this.EndValue;

                double startWidth = this.StartWidth;
                double endWidth = this.EndWidth;
                double startAngle;
                double endAngle;
                if (startValue > endValue)
                {
                    double temp;
                    temp = startWidth;
                    startWidth = endWidth;
                    endWidth = temp;
                }

                m_rangePath = new PathGeometry();

                if (startValue > endValue)
                {
                    startAngle = scale.GetAngleByValue(endValue);
                    endAngle = scale.GetAngleByValue(startValue);
                }
                else
                {
                    startAngle = scale.GetAngleByValue(startValue);
                    endAngle = scale.GetAngleByValue(endValue);
                }

                Point centerPoint = new Point(0, 0);
                double startInnerRadius = 0;
                double startOuterRadius = 0;
                double middleInnerRadius = 0;
                double middleOuterRadius = 0;
                double endInnerRadius = 0;
                double endOuterRadius = 0;
                if (this.RangePosition == ScalePlacement.Inside)
                {
                    startOuterRadius = scale.Radius - scale.ScaleBarSize - this.DistanceFromScale;
                    endOuterRadius = scale.Radius - scale.ScaleBarSize - this.DistanceFromScale;
                    startInnerRadius = startOuterRadius - startWidth;
                    endInnerRadius = endOuterRadius - endWidth;
                }
                else if (this.RangePosition == ScalePlacement.Cross)
                {
                    startOuterRadius = scale.Radius - ((scale.ScaleBarSize - startWidth) / 2) + this.DistanceFromScale;
                    endOuterRadius = scale.Radius - ((scale.ScaleBarSize - startWidth) / 2) + this.DistanceFromScale;
                    startInnerRadius = startOuterRadius - startWidth;
                    endInnerRadius = endOuterRadius - endWidth;
                }
                else if (this.RangePosition == ScalePlacement.Outside)
                {
                    startOuterRadius = scale.Radius + this.DistanceFromScale;
                    endOuterRadius = scale.Radius + this.DistanceFromScale;
                    startInnerRadius = startOuterRadius + startWidth;
                    endInnerRadius = endOuterRadius + endWidth;
                }

                middleInnerRadius = (startInnerRadius + endInnerRadius) / 2;
                middleOuterRadius = (startOuterRadius + endOuterRadius) / 2;
                if (startInnerRadius < 0)
                {
                    startInnerRadius = 0;
                }

                if (startOuterRadius < 0)
                {
                    startOuterRadius = 0;
                }

                if (middleInnerRadius < 0)
                {
                    middleInnerRadius = 0;
                }

                if (middleOuterRadius < 0)
                {
                    middleOuterRadius = 0;
                }

                if (endInnerRadius < 0)
                {
                    endInnerRadius = 0;
                }

                if (endOuterRadius < 0)
                {
                    endOuterRadius = 0;
                }
                SweepDirection st = new SweepDirection();
                SweepDirection et = new SweepDirection();

                if (scale.ScaleDirection == ScaleDirection.CounterClockwise)
                {
                    st = SweepDirection.Counterclockwise;
                    et = SweepDirection.Clockwise;
                }
                else
                {
                    st = SweepDirection.Clockwise;
                    et = SweepDirection.Counterclockwise;
                }


                Point startPoint1 = scale.ConvertToStageCoordinates(startInnerRadius, startAngle, centerPoint);
                Point startPoint2 = scale.ConvertToStageCoordinates(startOuterRadius, startAngle, centerPoint);
                Point middlePoint1 = scale.ConvertToStageCoordinates(middleInnerRadius, startAngle + ((endAngle - startAngle) / 2), centerPoint);
                Point middlePoint2 = scale.ConvertToStageCoordinates(middleOuterRadius, startAngle + ((endAngle - startAngle) / 2), centerPoint);
                Point endPoint1 = scale.ConvertToStageCoordinates(endInnerRadius, endAngle, centerPoint);
                Point endPoint2 = scale.ConvertToStageCoordinates(endOuterRadius, endAngle, centerPoint);

                if (startAngle == endAngle || (startAngle - endAngle) == 1 || (startAngle - endAngle) == -1)
                {
                    LineSegment l1 = new LineSegment(endPoint1, true);
                    LineSegment l2 = new LineSegment(endPoint2, true);
                    LineSegment l3 = new LineSegment(startPoint2, true);
                    LineSegment l4 = new LineSegment(startPoint1, true);

                    PathFigure figure1 = new PathFigure(
                        startPoint1,
                        new PathSegment[] { l1, l2, l3, l4 },
                        false);
                    (m_rangePath as PathGeometry).Figures.Add(figure1);
                }
                else if (endInnerRadius <= 3)
                {
                    ArcSegment asp1 = new ArcSegment(startPoint1, new Size((startInnerRadius + middleInnerRadius) / 2, (startInnerRadius + middleInnerRadius) / 2), 0, false, et, true);
                    ArcSegment amp11 = new ArcSegment(middlePoint1, new Size((startInnerRadius + middleInnerRadius) / 2, (startInnerRadius + middleInnerRadius) / 2), 0, false, et, true);
                    ArcSegment aep1 = new ArcSegment(endPoint1, new Size((endInnerRadius + middleInnerRadius), (endInnerRadius + middleInnerRadius)), 0, false, et, true);
                    LineSegment lep2 = new LineSegment(endPoint2, true);
                    ArcSegment aep2 = new ArcSegment(endPoint2, new Size(startOuterRadius, startOuterRadius), 0, false, st, true);
                    ArcSegment amp21 = new ArcSegment(middlePoint2, new Size(startOuterRadius, startOuterRadius), 0, false, st, true);
                    ArcSegment amp22 = new ArcSegment(middlePoint2, new Size(endOuterRadius, endOuterRadius), 0, false, st, true);
                    ArcSegment asp2 = new ArcSegment(startPoint2, new Size(endOuterRadius, endOuterRadius), 0, false, st, true);
                    LineSegment lsp1 = new LineSegment(startPoint1, true);

                    PathFigure figure1 = new PathFigure(
                        startPoint1,
                        new PathSegment[] { amp11, aep1, lep2, aep2, amp21, amp22, asp2, lsp1 },
                        false);
                    (m_rangePath as PathGeometry).Figures.Add(figure1);
                }
                else
                {
                    ArcSegment asp1 = new ArcSegment(startPoint1, new Size((startInnerRadius + middleInnerRadius) / 2, (startInnerRadius + middleInnerRadius) / 2), 0, false, st, true);
                    ArcSegment amp11 = new ArcSegment(middlePoint1, new Size((startInnerRadius + middleInnerRadius) / 2, (startInnerRadius + middleInnerRadius) / 2), 0, false, st, true);
                    ArcSegment amp12 = new ArcSegment(middlePoint1, new Size((middleInnerRadius + endInnerRadius) / 2, (middleInnerRadius + endInnerRadius) / 2), 0, false, st, true);
                    ArcSegment aep1 = new ArcSegment(endPoint1, new Size((middleInnerRadius + endInnerRadius) / 2, (middleInnerRadius + endInnerRadius) / 2), 0, false, st, true);
                    LineSegment lep2 = new LineSegment(endPoint2, true);
                    ArcSegment aep2 = new ArcSegment(endPoint2, new Size(startOuterRadius, startOuterRadius), 0, false, et, true);
                    ArcSegment amp21 = new ArcSegment(middlePoint2, new Size(startOuterRadius, startOuterRadius), 0, false, et, true);
                    ArcSegment amp22 = new ArcSegment(middlePoint2, new Size(endOuterRadius, endOuterRadius), 0, false, et, true);
                    ArcSegment asp2 = new ArcSegment(startPoint2, new Size(endOuterRadius, endOuterRadius), 0, false, et, true);
                    LineSegment lsp1 = new LineSegment(startPoint1, true);

                    PathFigure figure1 = new PathFigure(
                        startPoint1,
                        new PathSegment[] { asp1, amp11, amp12, aep1, lep2, aep2, amp21, amp22, asp2, lsp1 },
                        false);
                    (m_rangePath as PathGeometry).Figures.Add(figure1);
                }
            }


            if (this.BindIndicator)
            {
                if (this.VisualParent is CircularScale)
                {
                    CircularScale scale = this.VisualParent as CircularScale;
                    CircularGauge gauge = scale.gaugeReference as CircularGauge;
                    if (gauge.StateIndicators.Count == 1)
                    {
                        StateIndicator indicator = gauge.StateIndicators[0] as StateIndicator;
                        indicator.StateRanges.Add(new StateRange(this.StartValue, this.EndValue, this.IndicatorColor));
                    }
                    else if (gauge.StateIndicators.Count > 1)
                    {
                        StateIndicator indicator = gauge.StateIndicators[1] as StateIndicator;
                        indicator.StateRanges.Add(new StateRange(this.StartValue, this.EndValue, this.IndicatorColor));

                    }
                    gauge.InvalidateArrange();
                }
            }

            return finalSize;
        }

        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(this.StartWidth, 1);
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (this.VisualParent is CircularScale)
            {

                if (m_rangePath != null)
                {
                    drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), m_rangePath);
                }
            }
        }

        /// <summary>
        /// Updates property value cache and raises StartValueChanged/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnBindIndicatorChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnBindIndicatorChanged(e);
            if (this.BindIndicator)
            {
                if (this.VisualParent is CircularScale)
                {
                    CircularScale scale = this.VisualParent as CircularScale;
                    CircularGauge gauge = scale.gaugeReference as CircularGauge;
                    if (gauge.StateIndicators.Count == 0)
                    {
                        StateIndicator indicator = new StateIndicator();
                        indicator.Height = 20;
                        indicator.Width = 20;
                        indicator.Location = new Point(50, 80);
                        indicator.IndicatorStyle = IndicatorStyle.CircularLED;
                        Binding binding = new Binding();
                        binding.Source = scale.Pointers[0];
                        binding.Path = new PropertyPath("Value");
                        indicator.SetBinding(StateIndicator.ValueProperty, binding);
                        gauge.StateIndicators.Add(indicator);
                        gauge.InvalidateArrange();
                    }
                }
            }
            this.InvalidateArrange();
        }
        #endregion Overrides

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
