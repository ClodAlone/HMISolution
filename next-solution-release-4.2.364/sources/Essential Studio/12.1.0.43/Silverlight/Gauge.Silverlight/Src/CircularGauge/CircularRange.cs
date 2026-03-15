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
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Data;

    /// <summary>
    /// Represents the range scale visual element.
    /// </summary>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <term>C#:</term></listheader>
    /// <item>
    /// <description>using Syncfusion.Windows.Gauge; 
    /// <para></para>
    /// <para>CircularRange cirularrange = new CircularRange();</para>
    /// <para>           cirularrange.StartValue = 80;</para>
    /// <para>           cirularrange.EndValue = 100;</para>
    /// <para>            cirularrange.StartWidth = 1;</para>
    /// <para>            cirularrange.EndWidth = 10;</para>
    /// <para>            cirularrange.DistanceFromScale = 3;</para>
    /// <para>            cirularrange.Background = new                SolidColorBrush(Colors.Blue);</para>
    /// <para>            cirularscale.Ranges.Add(cirularrange);</para></description></item></list>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml:</term></listheader>
    /// <item>
    /// <description>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot; 
    /// <para>    </para>
    /// <para></para>
    /// <para>&lt;syncfusion:CircularScale.Ranges&gt;</para>
    /// <para>&lt;syncfusion:CircularRange StartValue=&quot;70&quot; EndValue=&quot;100&quot; Name=&quot;range&quot; StartWidth=&quot;0&quot; EndWidth=&quot;15&quot;                                                RangePosition=&quot;Inside&quot; DistanceFromScale=&quot;35&quot; Background=&quot;Red&quot;&gt;</para>
    /// <para>&lt;/syncfusion:CircularRange&gt;</para>
    /// <para>&lt;/syncfusion:CircularScale.Ranges&gt; </para></description></item></list>
    /// </example>
    public class CircularRange : RangeBase
    {
        #region Private members

        /// <summary>
        /// The path used to draw the range.
        /// </summary>
        private Path mrangePath;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.CircularRange">CircularRange</see> class
        /// </summary>
        /// <remarks>
        /// <para>Cirular Gauge Range is used to indicate any certain region.</para>
        /// </remarks>
        public CircularRange()
        {
            DefaultStyleKey = typeof(CircularRange);
        }

        #endregion

        #region Implemantation

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.mrangePath = this.GetTemplateChild("PART_RangePath") as Path;
            this.UpdateVisualStyle();
            this.RefreshRange();

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Updates visual style
        /// </summary>
        /// <remarks></remarks>
        protected override void UpdateVisualStyle()
        {
            if (this.GaugeElementParent != null)
                this.VisualStyle = ((this.GaugeElementParent as CircularScale).GaugeElementParent as GaugeBase).VisualStyle;
            base.UpdateVisualStyle();
        }

        /// <summary>
        /// Updates property value cache and raises event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected override void OnBindIndicatorChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnBindIndicatorChanged(e);
            
            this.InvalidateArrange();
        }
        /// <summary>
        /// Refreshes the Range
        /// </summary>
        internal void RefreshRange()
        {
            if (this.IndicatorColor == null)
            {
                this.IndicatorColor = this.Background;
            }

           if (this.GaugeElementParent is CircularScale)
            {
                PathGeometry mrangeGeometry = new PathGeometry();
                CircularScale scale = this.GaugeElementParent as CircularScale;

                double startAngle = scale.GetAngleByValue(this.StartValue);
                double endAngle = scale.GetAngleByValue(this.EndValue);
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
                    startInnerRadius = startOuterRadius - this.StartWidth;
                    endInnerRadius = endOuterRadius - this.EndWidth;
                }
                else if (this.RangePosition == ScalePlacement.Cross)
                {
                    startOuterRadius = scale.Radius - ((scale.ScaleBarSize - this.StartWidth) / 2) + this.DistanceFromScale;
                    endOuterRadius = scale.Radius - ((scale.ScaleBarSize - this.StartWidth) / 2) + this.DistanceFromScale;
                    startInnerRadius = startOuterRadius - this.StartWidth;
                    endInnerRadius = endOuterRadius - this.EndWidth;
                }
                else if (this.RangePosition == ScalePlacement.Outside)
                {
                    startOuterRadius = scale.Radius + this.DistanceFromScale;
                    endOuterRadius = scale.Radius + this.DistanceFromScale;
                    startInnerRadius = startOuterRadius + this.StartWidth;
                    endInnerRadius = endOuterRadius + this.EndWidth;
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

                if (scale.IsReversed)
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

                PathFigure figure1 = new PathFigure();
                figure1.StartPoint = startPoint1;

                ArcSegment arc = new ArcSegment();
                arc.Point = startPoint1;
                arc.RotationAngle = 0;
                arc.SweepDirection = st;
                arc.Size = new Size((startInnerRadius + middleInnerRadius) / 2, (startInnerRadius + middleInnerRadius) / 2);
                arc.IsLargeArc = false;
                figure1.Segments.Add(arc);

                arc = new ArcSegment();
                arc.Point = middlePoint1;
                arc.RotationAngle = 0;
                arc.SweepDirection = st;
                arc.Size = new Size((startInnerRadius + middleInnerRadius) / 2, (startInnerRadius + middleInnerRadius) / 2);
                arc.IsLargeArc = false;
                figure1.Segments.Add(arc);

                arc = new ArcSegment();
                arc.Point = middlePoint1;
                arc.RotationAngle = 0;
                arc.SweepDirection =st;
                arc.Size = new Size((middleInnerRadius + endInnerRadius) / 2, (middleInnerRadius + endInnerRadius) / 2);
                arc.IsLargeArc = false;
                figure1.Segments.Add(arc);

                arc = new ArcSegment();
                arc.Point = endPoint1;
                arc.RotationAngle = 0;
                arc.SweepDirection = st;
                arc.Size = new Size((middleInnerRadius + endInnerRadius) / 2, (middleInnerRadius + endInnerRadius) / 2);
                arc.IsLargeArc = false;
                figure1.Segments.Add(arc);

                LineSegment line = new LineSegment();
                line.Point = endPoint2;
                figure1.Segments.Add(line);

                arc = new ArcSegment();
                arc.Point = endPoint2;
                arc.RotationAngle = 0;
                arc.SweepDirection = et;
                arc.Size = new Size(startOuterRadius, startOuterRadius);
                arc.IsLargeArc = false;
                figure1.Segments.Add(arc);

                arc = new ArcSegment();
                arc.Point = middlePoint2;
                arc.RotationAngle = 0;
                arc.SweepDirection = et;
                arc.Size = new Size(startOuterRadius, startOuterRadius);
                arc.IsLargeArc = false;
                figure1.Segments.Add(arc);

                arc = new ArcSegment();
                arc.Point = middlePoint2;
                arc.RotationAngle = 0;
                arc.SweepDirection = et;
                arc.Size = new Size(endOuterRadius, endOuterRadius);
                arc.IsLargeArc = false;
                figure1.Segments.Add(arc);

                arc = new ArcSegment();
                arc.Point = startPoint2;
                arc.RotationAngle = 0;
                arc.SweepDirection = et;
                arc.Size = new Size(endOuterRadius, endOuterRadius);
                arc.IsLargeArc = false;
                figure1.Segments.Add(arc);

                line = new LineSegment();
                line.Point = startPoint1;
                figure1.Segments.Add(line);

                mrangeGeometry.Figures.Add(figure1);
                if (this.mrangePath != null)
                {
                    this.mrangePath.Data = mrangeGeometry;
                }

                if (this.BindIndicator)
                {
                        CircularGauge gauge = scale.GaugeElementParent as CircularGauge;
                        if (gauge.StateIndicators.Count == 0)
                        {
                            StateIndicator indicator = new StateIndicator();
                            indicator.IndicatorHeight = 20;
                            indicator.IndicatorWidth = 20;
                            indicator.Location = new Point(50, 80);
                            indicator.IndicatorStyle = IndicatorStyle.CircularLED;
                            Binding binding = new Binding();
                            binding.Source = scale.Pointers[0];
                            binding.Path = new PropertyPath("Value");
                            indicator.SetBinding(StateIndicator.ValueProperty, binding);
                            gauge.StateIndicators.Add(indicator);
                            gauge.RefreshScalesPanel();
                            
                            
                        }
                }
                if (this.BindIndicator)
                {
                    if (scale.GaugeElementParent is CircularGauge)
                    {
                        CircularGauge gauge = scale.GaugeElementParent as CircularGauge;
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
        
            }
        }

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use 
        /// to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            return new Size(this.StartWidth, 1);
        }

        /// <summary>
        /// Updates the property and raises the event DistanceFromScaleChanged event
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnDistanceFromScaleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshRange();
            base.OnDistanceFromScaleChanged(e);
        }

        /// <summary>
        /// Updates the property and raises the event EndValueChanged event
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnEndValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            this.RefreshRange();
            base.OnEndValueChanged(e);
        }

        /// <summary>
        /// Updates the property and raises the event EndWidthChanged event
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnEndWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshRange();
            base.OnEndWidthChanged(e);
        }

        /// <summary>
        /// Updates the property and raises the event RangePositionChanged event
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnRangePositionChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshRange();
            base.OnRangePositionChanged(e);
        }

        /// <summary>
        /// Updates the property and raises the event StartValueChanged event
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnStartValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            this.RefreshRange();
            base.OnStartValueChanged(e);
        }

        /// <summary>
        /// Updates the property and raises the event StartWidthChanged event
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnStartWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshRange();
            base.OnStartWidthChanged(e);
        }

        #endregion
    }
}
