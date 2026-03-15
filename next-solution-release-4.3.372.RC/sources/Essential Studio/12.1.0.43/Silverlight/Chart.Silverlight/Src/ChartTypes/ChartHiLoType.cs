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
    /// Represents Hilo chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    public class ChartHiLoSegment : Segment
    {
        #region dependency properties
        /// <summary>
        /// Identifies the template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
        DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartHiLoSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the HighX dependency property.
        /// </summary>
        public static readonly DependencyProperty HighXProperty =
            DependencyProperty.Register("HighX", typeof(double), typeof(ChartHiLoSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the HighY dependency property.
        /// </summary>
        public static readonly DependencyProperty HighYProperty =
            DependencyProperty.Register("HighY", typeof(double), typeof(ChartHiLoSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the LowX dependency property.
        /// </summary>
        public static readonly DependencyProperty LowXProperty =
            DependencyProperty.Register("LowX", typeof(double), typeof(ChartHiLoSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the LowY dependency property.
        /// </summary>
        public static readonly DependencyProperty LowYProperty =
            DependencyProperty.Register("LowY", typeof(double), typeof(ChartHiLoSegment), new PropertyMetadata(0d));
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_point1
        /// </summary>
        private ChartPoint m_point1;

        /// <summary>
        /// Initializes m_point2
        /// </summary>
        private ChartPoint m_point2;
        #endregion

        #region Properties

        /// <summary>
        /// Get or Set TemplateProperty
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
        /// Gets or sets the high X value. This is a dependency property.
        /// </summary>
        /// <value>The high X.</value>
        public double HighX
        {
            get { return (double)GetValue(HighXProperty); }
            set { SetValue(HighXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the high Y value. This is a dependency property.
        /// </summary>
        /// <value>The high Y.</value>
        public double HighY
        {
            get { return (double)GetValue(HighYProperty); }
            set { SetValue(HighYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the low X value. This is a dependency property.
        /// </summary>
        /// <value>The low X.</value>
        public double LowX
        {
            get { return (double)GetValue(LowXProperty); }
            set { SetValue(LowXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the low Y value. This is a dependency property.
        /// </summary>
        /// <value>The low Y.</value>
        public double LowY
        {
            get { return (double)GetValue(LowYProperty); }
            set { SetValue(LowYProperty, value); }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartHiLoSegment"/> class.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="correspondingPoint1">The corresponding point1.</param>
        /// <param name="series">The series.</param>
        internal ChartHiLoSegment(ChartPoint point1, ChartPoint point2, ChartPoint correspondingPoint1, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint1 })
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartHiLoType), ChartTypes.HiLo);
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
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            Point point1 = transformer.TransformToVisible(m_point1.X + (series.XAxis.VisibleRange.Start * (-1)), m_point1.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
            Point point2 = transformer.TransformToVisible(m_point2.X + (series.XAxis.VisibleRange.Start * (-1)), m_point2.Y + (series.YAxis.VisibleRange.Start * (-1)), series);

            this.HighX = point1.X;
            this.LowX = point2.X;
            this.HighY = point1.Y;
            this.LowY = point2.Y;
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
        }
    }

    /// <summary>
    /// Class implementation for ChartHiloType
    /// </summary>
    public class ChartHiLoType : ChartType
    {
        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            SetRange(series, points, 2);
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
            double sbsCenter = 0.4 * (series.XAxis.VisibleInterval < 1 && series.Area.MinDataInterval < 1d ? series.Area.MinDataInterval : 1d);

            double center = (sbsInfo.Start - sbsCenter + sbsInfo.End - sbsCenter) / 2;

            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
            for (int i = 0; i < points.Count && points[i].Values.Length >= 2; i++)
            {
                ChartPoint hiPoint = new ChartPoint(points[i].X + center , points[i].Values[1] );
                ChartPoint loPoint = new ChartPoint(points[i].X + center , points[i].Values[0] );
                if (!double.IsNaN(points[i].Y))
                    series.Segments.Add(new ChartHiLoSegment(hiPoint, loPoint, points[i], series));
            }

            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
            {
                for (int i = 0; i < points.Count && points[i].Values.Length >= 2; i++)
                {
                    double yValue = (points[i].Values[0] + points[i].Values[1]) / 2;
                    if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                    {
                        if (points[i].Values[0] > points[i].Values[1])
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[0] - sbsInfo.End)), points, series, center) { adornemntLabelIndex = 0 });
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[1] - sbsInfo.End)), points, series, center) { adornemntLabelIndex = 1 });
                        }   
                    }
                    else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                    {

                        if (points[i].Values[0] < points[i].Values[1])
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[0] - sbsInfo.End)), points, series, center) { adornemntLabelIndex = 0 });
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[1] - sbsInfo.End)), points, series, center) { adornemntLabelIndex = 1 });
                        }     
                    }
                    else
                    {

                        ////series.Segments.Add(new ChartAdornment(points[i], points, series, center));
                        if (!double.IsNaN(points[i].Y))
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[0] + sbsInfo.End)), points, series, center) { adornemntLabelIndex = 0 });
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[1] - sbsInfo.End)), points, series, center) { adornemntLabelIndex = 1 });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
            series.Segments.Clear();
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
            return "HiLo";
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
