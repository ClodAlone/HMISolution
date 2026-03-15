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
    /// Represents price segment that is a part of candle chart type.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    public sealed class ChartCandleSegment : ColumnSegment
    {
        #region dependency properties
        /// <summary>
        /// Identifies the HiX dependency property.
        /// </summary>
        public static readonly DependencyProperty HiXProperty =
            DependencyProperty.Register("HighX", typeof(double), typeof(ChartCandleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the HiY dependency property.
        /// </summary>
        public static readonly DependencyProperty HiYProperty =
            DependencyProperty.Register("HighY", typeof(double), typeof(ChartCandleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the LoX dependency property.
        /// </summary>
        public static readonly DependencyProperty LoXProperty =
            DependencyProperty.Register("LowX", typeof(double), typeof(ChartCandleSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the LoY dependency property.
        /// </summary>
        public static readonly DependencyProperty LoYProperty =
            DependencyProperty.Register("LowY", typeof(double), typeof(ChartCandleSegment), new PropertyMetadata(0d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Hi X value
        /// </summary>
        /// <value>The hi X value.</value>
        public double HiX
        {
            get { return (double)GetValue(HiXProperty); }
            set { SetValue(HiXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the hi Y value.
        /// </summary>
        /// <value>The hi Y value.</value>
        public double HiY
        {
            get { return (double)GetValue(HiYProperty); }
            set { SetValue(HiYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the low X.
        /// </summary>
        /// <value>The lo X value.</value>
        public double LoX
        {
            get { return (double)GetValue(LoXProperty); }
            set { SetValue(LoXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the low Y.
        /// </summary>
        /// <value>The lo Y value.</value>
        public double LoY
        {
            get { return (double)GetValue(LoYProperty); }
            set { SetValue(LoYProperty, value); }
        }
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

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.ChartCandleSegment">ChartCandleSegment</see> class. 
        /// </summary>
        /// <param name="bottomLeftPnt">The bottom left PNT</param>
        /// <param name="topRightPnt">The top right PNT</param>
        /// <param name="hipt">The hi pt.</param>
        /// <param name="lopt">The lo pt.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        /// <remarks></remarks>
        internal ChartCandleSegment(ChartPoint bottomLeftPnt, ChartPoint topRightPnt, ChartPoint hipt, ChartPoint lopt, ChartPoint correspondingPoint, ChartSeries series)
            : base(bottomLeftPnt, topRightPnt, correspondingPoint, series)
        {
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = ResourceManager.GetSeriesTemplate(typeof(ChartCandleType), ChartTypes.Candle);
                this.SegmentTemplate = ResourceManager.GetSeriesTemplate(typeof(ChartCandleType), ChartTypes.Candle);
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
                this.SegmentTemplate = series.SegmentTemplate;
            }

            m_point1 = hipt;
            m_point2 = lopt;
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);

            Point point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y, series);
            Point point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y, series);

            this.HiX = point1.X;
            this.LoX = point2.X;
            this.HiY = point1.Y;
            this.LoY = point2.Y;
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
    /// Class implemeatation  for ChartCandleType
    /// </summary>
    public class ChartCandleType : ChartType
    {
        #region Implementation
        /// <summary>
        /// Converts ChartCandleType to string 
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return "Candle";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            SetRange(series, points, 4);
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
            double sbsCenter = 0.4 * (series.XAxis.VisibleInterval < 1 && series.Area.MinDataInterval < 1d ? series.Area.MinDataInterval : 1d);
            double center = (sbsInfo.Start - sbsCenter + sbsInfo.End - sbsCenter) / 2;
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
            for (int i = 0; i < points.Count; i++)
            {
                if (double.IsNaN(points[i].Y))
                    continue;
                double x1 = points[i].X + sbsInfo.Start - sbsCenter + (series.XAxis.VisibleRange.Start * (-1));
                double x2 = points[i].X + sbsInfo.End - sbsCenter + (series.XAxis.VisibleRange.Start * (-1));
                double y1 = points[i].Values[0] + (series.YAxis.VisibleRange.Start * (-1));
                double y2 = points[i].Values[2] + (series.YAxis.VisibleRange.Start * (-1));

                ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                ChartPoint hiPoint = new ChartPoint(points[i].X + center + (series.XAxis.VisibleRange.Start * (-1)), points[i].Values[1] + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint loPoint = new ChartPoint(points[i].X + center + (series.XAxis.VisibleRange.Start * (-1)), points[i].Values[3] + (series.YAxis.VisibleRange.Start * (-1)));

                series.Segments.Add(new ChartCandleSegment(cdpBottomLeft, cdpRightTop, hiPoint, loPoint, points[i], series));


                if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                {
                    if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                    {
                        if (points[i].Values[0] > points[i].Values[3])
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[0])), points, series, center) { adornemntLabelIndex = 0 });
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[1])), points, series, center) { adornemntLabelIndex = 1 });
                        }

                    }
                    else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                    {
                        if (points[i].Values[0] < points[i].Values[3])
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[0])), points, series, center) { adornemntLabelIndex = 0 });
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[1])), points, series, center) { adornemntLabelIndex = 1 });
                        }
                    }
                    else
                    {
                        series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[0])), points, series, center) { adornemntLabelIndex = 0 });
                        series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[1])), points, series, center) { adornemntLabelIndex = 1 });
                        series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[2])), points, series, center) { adornemntLabelIndex = 2 });
                        series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[3])), points, series, center) { adornemntLabelIndex = 3 });

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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

        }
    }
}
