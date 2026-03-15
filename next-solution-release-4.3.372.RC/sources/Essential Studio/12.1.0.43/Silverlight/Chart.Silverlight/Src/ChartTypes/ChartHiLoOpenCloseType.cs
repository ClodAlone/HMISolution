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
    /// Represents HiLo Open-Close chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    public sealed class ChartHiLoOpenCloseSegment : ChartHiLoSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the StartOpenX dependency property.
        /// </summary>
        public static readonly DependencyProperty StartOpenXProperty =
            DependencyProperty.Register("StartOpenX", typeof(double), typeof(ChartHiLoOpenCloseSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the StartOpenY dependency property.
        /// </summary>
        public static readonly DependencyProperty StartOpenYProperty =
            DependencyProperty.Register("StartOpenY", typeof(double), typeof(ChartHiLoOpenCloseSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the EndOpenX dependency property.
        /// </summary>
        public static readonly DependencyProperty EndOpenXProperty =
            DependencyProperty.Register("EndOpenX", typeof(double), typeof(ChartHiLoOpenCloseSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the EndOpenY dependency property.
        /// </summary>
        public static readonly DependencyProperty EndOpenYProperty =
            DependencyProperty.Register("EndOpenY", typeof(double), typeof(ChartHiLoOpenCloseSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the StartCloseX dependency property.
        /// </summary>
        public static readonly DependencyProperty StartCloseXProperty =
            DependencyProperty.Register("StartCloseX", typeof(double), typeof(ChartHiLoOpenCloseSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the StartCloseY dependency property.
        /// </summary>
        public static readonly DependencyProperty StartCloseYProperty =
            DependencyProperty.Register("StartCloseY", typeof(double), typeof(ChartHiLoOpenCloseSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the EndCloseX dependency property.
        /// </summary>
        public static readonly DependencyProperty EndCloseXProperty =
            DependencyProperty.Register("EndCloseX", typeof(double), typeof(ChartHiLoOpenCloseSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the EndCloseY dependency property.
        /// </summary>
        public static readonly DependencyProperty EndCloseYProperty =
            DependencyProperty.Register("EndCloseY", typeof(double), typeof(ChartHiLoOpenCloseSegment), new PropertyMetadata(0d));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the end close Y. This is a dependency property.
        /// </summary>
        /// <value>The end close Y.</value>
        public double EndCloseY
        {
            get { return (double)GetValue(EndCloseYProperty); }
            set { SetValue(EndCloseYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end close X. This is a dependency property.
        /// </summary>
        /// <value>The end close X.</value>
        public double EndCloseX
        {
            get { return (double)GetValue(EndCloseXProperty); }
            set { SetValue(EndCloseXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start close Y. This is a dependency property.
        /// </summary>
        /// <value>The start close Y.</value>
        public double StartCloseY
        {
            get { return (double)GetValue(StartCloseYProperty); }
            set { SetValue(StartCloseYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start close X. This is a dependency property.
        /// </summary>
        /// <value>The start close X.</value>
        public double StartCloseX
        {
            get { return (double)GetValue(StartCloseXProperty); }
            set { SetValue(StartCloseXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end open Y. This is a dependency property.
        /// </summary>
        /// <value>The end open Y.</value>
        public double EndOpenY
        {
            get { return (double)GetValue(EndOpenYProperty); }
            set { SetValue(EndOpenYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end open X. This is a dependency property.
        /// </summary>
        /// <value>The end open X.</value>
        public double EndOpenX
        {
            get { return (double)GetValue(EndOpenXProperty); }
            set { SetValue(EndOpenXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start open Y. This is a dependency property.
        /// </summary>
        /// <value>The start open Y.</value>
        public double StartOpenY
        {
            get { return (double)GetValue(StartOpenYProperty); }
            set { SetValue(StartOpenYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start open X. This is a dependency property.
        /// </summary>
        /// <value>The start open X.</value>
        public double StartOpenX
        {
            get { return (double)GetValue(StartOpenXProperty); }
            set { SetValue(StartOpenXProperty, value); }
        }
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_sOpen
        /// </summary>
        private ChartPoint m_sopen;

        /// <summary>
        /// Initializes m_eOpen
        /// </summary>
        private ChartPoint m_eopen;

        /// <summary>
        /// Initializes m_sClose
        /// </summary>
        private ChartPoint m_sclose;

        /// <summary>
        /// Initializes m_eClose
        /// </summary>
        private ChartPoint m_eclose;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ChartHiLoOpenCloseSegment class. 
        /// </summary>
        /// <param name="hi">The hi value.</param>
        /// <param name="lo">The lo value.</param>
        /// <param name="sOpen">The s open.</param>
        /// <param name="eopen">The e open.</param>
        /// <param name="sclose">The s close.</param>
        /// <param name="eclose">The e close.</param>
        /// <param name="correspondingPoint1">The corresponding point1.</param>
        /// <param name="series">The series.</param>
        /// <remarks></remarks>
        internal ChartHiLoOpenCloseSegment(ChartPoint hi, ChartPoint lo, ChartPoint sOpen, ChartPoint eopen, ChartPoint sclose, ChartPoint eclose, ChartPoint correspondingPoint1, ChartSeries series)
            : base(hi, lo, correspondingPoint1, series)
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartHiLoOpenCloseSegment), ChartTypes.HiLoOpenClose);
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

            m_sopen = sOpen;
            m_eopen = eopen;
            m_sclose = sclose;
            m_eclose = eclose;
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

            Point sopoint = transformer.TransformToVisible(m_sopen.X, m_sopen.Y, series);
            Point eopoint = transformer.TransformToVisible(m_eopen.X, m_eopen.Y, series);
            Point scpoint = transformer.TransformToVisible(m_sclose.X, m_sclose.Y, series);
            Point ecpoint = transformer.TransformToVisible(m_eclose.X, m_eclose.Y, series);

            this.StartOpenX = sopoint.X;
            this.StartOpenY = sopoint.Y;
            this.EndOpenX = eopoint.X;
            this.EndOpenY = eopoint.Y;

            this.StartCloseX = scpoint.X;
            this.StartCloseY = scpoint.Y;
            this.EndCloseX = ecpoint.X;
            this.EndCloseY = ecpoint.Y;
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.m_eclose = null;
            this.m_eopen = null;
            this.m_sclose = null;
            this.m_sopen = null;
        }
    }

    /// <summary>
    /// Class implementation for ChartHiloOpenCloseType
    /// </summary>
    public class ChartHiLoOpenCloseType : ChartHiLoType
    {
        #region Public methods
        /// <summary>
        /// Converts ChartHiLoOpenCloseType to string 
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return "HiLoOpenClose";
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
            double left = sbsInfo.Start - sbsCenter;
            double right = sbsInfo.End - sbsCenter;
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
            for (int i = 0; i < points.Count && points[i].Values.Length >= 4; i++)
            {
                ChartPoint hipoint = new ChartPoint(points[i].X + center + (series.XAxis.VisibleRange.Start * (-1)), points[i].Values[1] + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint lopoint = new ChartPoint(points[i].X + center + (series.XAxis.VisibleRange.Start * (-1)), points[i].Values[0] + (series.YAxis.VisibleRange.Start * (-1)));

                ChartPoint sopoint = new ChartPoint(points[i].X + left + (series.XAxis.VisibleRange.Start * (-1)), points[i].Values[2] + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint eopoint = new ChartPoint(points[i].X + center + (series.XAxis.VisibleRange.Start * (-1)), points[i].Values[2] + (series.YAxis.VisibleRange.Start * (-1)));

                ChartPoint scpoint = new ChartPoint(points[i].X + right + (series.XAxis.VisibleRange.Start * (-1)), points[i].Values[3] + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint ecpoint = new ChartPoint(points[i].X + center + (series.XAxis.VisibleRange.Start * (-1)), points[i].Values[3] + (series.YAxis.VisibleRange.Start * (-1)));

                series.Segments.Add(new ChartHiLoOpenCloseSegment(hipoint, lopoint, sopoint, eopoint, scpoint, ecpoint, points[i], series));
            }

            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
            {
                for (int i = 0; i < points.Count && points[i].Values.Length >= 4; i++)
                {

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
                        if (points[i].Values[2] > points[i].Values[3])
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[2])), points, series, center - right) { adornemntLabelIndex = 2 });
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[3])), points, series, center - left) { adornemntLabelIndex = 3 });
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
                        if (points[i].Values[2] < points[i].Values[3])
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[2])), points, series, center - right) { adornemntLabelIndex = 2 });
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, (points[i].Values[3])), points, series, center - left) { adornemntLabelIndex = 3 });
                        }
                    }                 
                    else
                    {
                        ////series.Segments.Add(new ChartAdornment(points[i], points, series, center));
                        series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, points[i].Values[0]), points, series, center) { adornemntLabelIndex = 0 });
                        series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, points[i].Values[1]), points, series, center) { adornemntLabelIndex = 1 });
                        series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, points[i].Values[2]), points, series, center - right) { adornemntLabelIndex = 2 });
                        series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, points[i].Values[3]), points, series, center - left) { adornemntLabelIndex = 3 });
                    }
                }
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        /// <param name="points">The indexed data points</param>
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
