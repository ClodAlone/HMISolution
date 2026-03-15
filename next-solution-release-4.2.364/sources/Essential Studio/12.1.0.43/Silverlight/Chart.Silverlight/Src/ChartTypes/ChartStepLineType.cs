#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents Step line chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    /// <seealso cref="ChartStepLineType"/>
    public sealed class ChartStepLineSegment : Segment
    {
        #region Members
        /// <summary>
        /// Declares m_point1
        /// </summary>
        private ChartPoint m_point1;

        /// <summary>
        /// Declares m_point2
        /// </summary>
        private ChartPoint m_point2;

        /// <summary>
        /// Declares m_stepPoint
        /// </summary>
        private ChartPoint m_stepPoint;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the X1 dependency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the X2 dependency property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y1 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y2 dependency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the StepX dependency property.
        /// </summary>
        public static readonly DependencyProperty StepXProperty =
            DependencyProperty.Register("StepX", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the StepY dependency property.
        /// </summary>
        public static readonly DependencyProperty StepYProperty =
            DependencyProperty.Register("StepY", typeof(double), typeof(ChartStepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(ChartStepLineSegment), new PropertyMetadata(null));

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
                DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartStepLineSegment), new PropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set Template property
        /// </summary>
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
        /// Gets or sets the x1. This is a dependency property.
        /// </summary>
        /// <value>The x1 value.</value>
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        /// <summary>
        /// Gets or sets the x2. This is a dependency property.
        /// </summary>
        /// <value>The x2 value.</value>
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        /// <summary>
        /// Gets or sets the y1. This is a dependency property.
        /// </summary>
        /// <value>The y1 value.</value>
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        /// <summary>
        /// Gets or sets the y2. This is a dependency property.
        /// </summary>
        /// <value>The y2 value.</value>
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        /// <summary>
        /// Gets or sets the step X. This is a dependency property.
        /// </summary>
        /// <value>The step X.</value>
        public double StepX
        {
            get { return (double)GetValue(StepXProperty); }
            set { SetValue(StepXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the step Y. This is a dependency property.
        /// </summary>
        /// <value>The step Y.</value>
        public double StepY
        {
            get { return (double)GetValue(StepYProperty); }
            set { SetValue(StepYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the segment's points. This is a dependency property.
        /// </summary>
        /// <value>The points.</value>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStepLineSegment"/> class.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="stepPoint">The step point.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="correspondingPoint1">The corresponding point1.</param>
        /// <param name="correspondingPoint2">The corresponding point2.</param>
        /// <param name="series">The series.</param>
        internal ChartStepLineSegment(ChartPoint point1, ChartPoint stepPoint, ChartPoint point2, ChartPoint correspondingPoint1, ChartPoint correspondingPoint2, ChartSeries series)
            : base(series, new ChartPointsCollection() { correspondingPoint1, correspondingPoint2 })
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartStepLineType), ChartTypes.StepLine);
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = this.Template;
                this.SegmentTemplate = this.Template;
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
                this.SegmentTemplate = series.SegmentTemplate;
            }

            m_point1 = point1;
            m_point2 = point2;
            m_stepPoint = stepPoint;
        }
        #endregion

        #region Implmentation

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            Point point1 = transformer.TransformToVisible(m_point1.X + (series.XAxis.VisibleRange.Start * (-1)), m_point1.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
            Point point2 = transformer.TransformToVisible(m_point2.X + (series.XAxis.VisibleRange.Start * (-1)), m_point2.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
            Point stepPoint = transformer.TransformToVisible(m_stepPoint.X + (series.XAxis.VisibleRange.Start * (-1)), m_stepPoint.Values[0] + (series.YAxis.VisibleRange.Start * (-1)), series);
            if (X1 != point1.X || X2 != point2.X || Y1 != point1.Y || Y2 != point2.Y || StepX != stepPoint.X || StepY != stepPoint.Y)
            {
                this.X1 = point1.X;
                this.X2 = point2.X;
                this.Y1 = point1.Y;
                this.Y2 = point2.Y;
                this.StepX = stepPoint.X;
                this.StepY = stepPoint.Y;
                this.Points = new PointCollection() { point1, stepPoint, point2 };
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.m_point1 = null;
            this.m_point2 = null;
            this.m_stepPoint = null;
            if (this.Points != null)
            {
                this.Points.Clear();
                this.Points = null;
            }
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before the <see cref="T:System.Object"/> is reclaimed by garbage collection.
        /// </summary>
        ~ChartStepLineSegment()
        {
        }
    }

    /// <summary>
    /// Class implementation for ChartStepLineType
    /// </summary>
    public class ChartStepLineType : ChartType
    {
        #region Public methods
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            SetRange(series, points, 1);
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (!double.IsNaN(points[i].Y) && !points[i].EmptyPoint)
                {
                    if (double.IsNaN(points[(i + 1 == points.Count ? i : i + 1)].Values[0]))
                        continue;
                    //if (double.IsNaN(points[i].Values[0]))
                    //    continue;
                    ChartPoint point1 = new ChartPoint(points[i].X , points[i].Y );
                    //ChartPoint stepPoint = new ChartPoint(points[i].X + (series.XAxis.VisibleRange.Start * (-1)), points[i + 1].Values[0] + (series.YAxis.VisibleRange.Start * (-1)));
                    ChartPoint stepPoint = new ChartPoint(points[i].X , points[i + 1].Y );
                    ChartPoint point2 = new ChartPoint(points[i + 1].X , points[i + 1].Y );

                    if (series.Area.Host == Host.OLAPChart)
                    {
                        point1.X -= 0.5;
                        point2.X -= 0.5;
                        stepPoint.X -= 0.5;
                    }

                    series.Segments.Add(new ChartStepLineSegment(point1, stepPoint, point2, points[i], points[i + 1], series));
                    if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                    {
                        ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                        if (series.Area.Host == Host.OLAPChart)
                        {
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].X - 0.5, points[i].Y), points, series, 0d));
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], points, series, 0d));
                        }
                    }
                }
                else if (series.ShowEmptyPoints)
                {
                    points[i].EmptyPoint = true;
                    if (series.EmptyPointValue == EmptyPointValue.Zero)
                    {
                        points[i].Y = 0;
                    }
                    else
                    {
                        if (i + 1 == points.Count)
                            points[i].Y = points[i - 1].Y / 2;
                        else
                        {
                            int index;
                            for (index = i + 1; index < points.Count; index++)
                                if (!double.IsNaN(points[index].Y))
                                    break;
                            if (i == 0)
                                points[i].Y = (index == points.Count ? 40 : points[index].Y) / 2;
                            else
                                points[i].Y = points[i - 1].Y / 2 + (index == points.Count ? 40 : points[index].Y) / 2;
                        }
                    }

                    if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                    {
                        if (double.IsNaN(points[(i + 1 == points.Count ? i : i + 1)].Values[0]))
                            continue;
                        ChartPoint point1 = new ChartPoint(points[i].X , points[i].Y );
                        ChartPoint stepPoint = new ChartPoint(points[i].X , points[i + 1].Y );
                        ChartPoint point2 = new ChartPoint(points[i + 1].X , points[i + 1].Y );
                        series.Segments.Add(new ChartStepLineSegment(point1, stepPoint, point2, points[i], points[i + 1], series));
                    }
                    else if (series.EmptyPointStyle == EmptyPointStyle.Symbol || series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                    {
                        if (double.IsNaN(points[(i + 1 == points.Count ? i : i + 1)].Values[0]))
                            continue;
                        ChartPoint stepPoint = new ChartPoint(points[i].X , points[i + 1].Y);
                        series.Segments.Add(new ScatterSegment(stepPoint, new ChartPoint(0,0), points[i], series));
                    }
                }
            }

            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
            {
                ////series.Segments.Add(new ChartAdornment(points[points.Count-1], points, series, 0d));
                series.Adornments.Add(new ChartAdornment(points[points.Count - 1], points, series, 0d));
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "StepLine";
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before the <see cref="T:System.Object"/> is reclaimed by garbage collection.
        /// </summary>
        ~ChartStepLineType ()
        {
        }
    }
}
