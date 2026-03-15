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
    /// Represents Box and Whisker chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    public sealed class ChartBoxAndWhiskerSegment : Segment
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the X1 depency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
                DependencyProperty.Register("X1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y1 depency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
                DependencyProperty.Register("Y1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the TopWhiskerY2 dependency property.
        /// </summary>
        public static readonly DependencyProperty TopWhiskerY2Property =
            DependencyProperty.Register("TopWhiskerY2", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the TopWhiskerY1 dependency property.
        /// </summary>
        public static readonly DependencyProperty TopWhiskerY1Property =
            DependencyProperty.Register("TopWhiskerY1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the TopWhiskerX2 dependency property.
        /// </summary>
        public static readonly DependencyProperty TopWhiskerX2Property =
            DependencyProperty.Register("TopWhiskerX2", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the TopWhiskerX1 dependency property.
        /// </summary>
        public static readonly DependencyProperty TopWhiskerX1Property =
            DependencyProperty.Register("TopWhiskerX1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the BottomWhiskery1 dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomWhiskerY2Property =
            DependencyProperty.Register("BottomWhiskerY2", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the BottomWhiskerY1 dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomWhiskerY1Property =
            DependencyProperty.Register("BottomWhiskerY1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the BottomWhiskerX2 dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomWhiskerX2Property =
            DependencyProperty.Register("BottomWhiskerX2", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the BottomWhiskerX1 dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomWhiskerX1Property =
            DependencyProperty.Register("BottomWhiskerX1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerY2 dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterLineWhiskerY2Property =
            DependencyProperty.Register("CenterLineWhiskerY2", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerY1 dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterLineWhiskerY1Property =
            DependencyProperty.Register("CenterLineWhiskerY1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerX2 dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterLineWhiskerX2Property =
            DependencyProperty.Register("CenterLineWhiskerX2", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerX dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterLineWhiskerX1Property =
            DependencyProperty.Register("CenterLineWhiskerX1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerX1 dependency property.
        /// </summary>
        public static readonly DependencyProperty MedianWhiskerX1Property =
            DependencyProperty.Register("MedianWhiskerX1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerX2 dependency property.
        /// </summary>
        public static readonly DependencyProperty MedianWhiskerX2Property =
            DependencyProperty.Register("MedianWhiskerX2", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerY1 dependency property.
        /// </summary>
        public static readonly DependencyProperty MedianWhiskerY1Property =
            DependencyProperty.Register("MedianWhiskerY1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerY2 dependency property.
        /// </summary>
        public static readonly DependencyProperty MedianWhiskerY2Property =
            DependencyProperty.Register("MedianWhiskerY2", typeof(double), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartBoxAndWhiskerSegment), new PropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set X1 property
        /// </summary>
        public double X1
        {
            get
            {
                return (double)GetValue(X1Property);
            }

            set
            {
                SetValue(X1Property, value);
            }
        }

        /// <summary>
        /// Get or Set Y1 property
        /// </summary>
        public double Y1
        {
            get
            {
                return (double)GetValue(Y1Property);
            }

            set
            {
                SetValue(Y1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the X co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The X co-ordinate.</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The Y co-ordinate.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width. This is a dependency property.
        /// </summary>
        /// <value>The double value width.</value>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height. This is a dependency property.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the top whisker x1. This is a dependency property.
        /// </summary>
        /// <value>The top whisker x1 co-ordinate.</value>
        public double TopWhiskerX1
        {
            get { return (double)GetValue(TopWhiskerX1Property); }
            set { SetValue(TopWhiskerX1Property, value); }
        }

        /// <summary>
        /// Gets or sets the top whisker x2 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The top whisker x2.</value>
        public double TopWhiskerX2
        {
            get { return (double)GetValue(TopWhiskerX2Property); }
            set { SetValue(TopWhiskerX2Property, value); }
        }

        /// <summary>
        /// Gets or sets the top whisker y1 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The top whisker y1.</value>
        public double TopWhiskerY1
        {
            get { return (double)GetValue(TopWhiskerY1Property); }
            set { SetValue(TopWhiskerY1Property, value); }
        }

        /// <summary>
        /// Gets or sets the top whisker y2 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The top whisker y2.</value>
        public double TopWhiskerY2
        {
            get { return (double)GetValue(TopWhiskerY2Property); }
            set { SetValue(TopWhiskerY2Property, value); }
        }

        /// <summary>
        /// Gets or sets the bottom whisker x1 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The bottom whisker x1.</value>
        public double BottomWhiskerX1
        {
            get { return (double)GetValue(BottomWhiskerX1Property); }
            set { SetValue(BottomWhiskerX1Property, value); }
        }

        /// <summary>
        /// Gets or sets the bottom whisker x2 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The bottom whisker x2.</value>
        public double BottomWhiskerX2
        {
            get { return (double)GetValue(BottomWhiskerX2Property); }
            set { SetValue(BottomWhiskerX2Property, value); }
        }

        /// <summary>
        /// Gets or sets the bottom whisker y1 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The bottom whisker y1.</value>
        public double BottomWhiskerY1
        {
            get { return (double)GetValue(BottomWhiskerY1Property); }
            set { SetValue(BottomWhiskerY1Property, value); }
        }

        /// <summary>
        /// Gets or sets the bottom whisker y2 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The bottom whisker y2.</value>
        public double BottomWhiskerY2
        {
            get { return (double)GetValue(BottomWhiskerY2Property); }
            set { SetValue(BottomWhiskerY2Property, value); }
        }

        /// <summary>
        /// Gets or sets the median whisker x1 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The median whisker x1.</value>
        public double CenterLineWhiskerX1
        {
            get { return (double)GetValue(CenterLineWhiskerX1Property); }
            set { SetValue(CenterLineWhiskerX1Property, value); }
        }

        /// <summary>
        /// Gets or sets the median whisker x2 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The median whisker x2.</value>
        public double CenterLineWhiskerX2
        {
            get { return (double)GetValue(CenterLineWhiskerX2Property); }
            set { SetValue(CenterLineWhiskerX2Property, value); }
        }

        /// <summary>
        /// Gets or sets the median whisker y1 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The median whisker y1.</value>
        public double CenterLineWhiskerY1
        {
            get { return (double)GetValue(CenterLineWhiskerY1Property); }
            set { SetValue(CenterLineWhiskerY1Property, value); }
        }

        /// <summary>
        /// Gets or sets the median whisker y2 co-ordinate. This is a dependency property.
        /// </summary>
        /// <value>The median whisker y2.</value>
        public double CenterLineWhiskerY2
        {
            get { return (double)GetValue(CenterLineWhiskerY2Property); }
            set { SetValue(CenterLineWhiskerY2Property, value); }
        }

        /// <summary>
        /// Gets or sets the MedianWhiskerY2. This is a dependency property.
        /// </summary>
        /// <value>The MedianWhiskerY2.</value>
        public double MedianWhiskerY2
        {
            get { return (double)GetValue(MedianWhiskerY2Property); }
            set { SetValue(MedianWhiskerY2Property, value); }
        }

        /// <summary>
        /// Gets or sets the MedianWhiskerY1. This is a dependency property.
        /// </summary>
        /// <value>The MedianWhiskerY1.</value>
        public double MedianWhiskerY1
        {
            get { return (double)GetValue(MedianWhiskerY1Property); }
            set { SetValue(MedianWhiskerY1Property, value); }
        }

        /// <summary>
        /// Gets or sets the MedianWhiskerX2. This is a dependency property.
        /// </summary>
        /// <value>The MedianWhiskerX2.</value>
        public double MedianWhiskerX2
        {
            get { return (double)GetValue(MedianWhiskerX2Property); }
            set { SetValue(MedianWhiskerX2Property, value); }
        }

        /// <summary>
        /// Gets or sets the MedianWhiskerX1. This is a dependency property.
        /// </summary>
        /// <value>The MedianWhiskerX1.</value>
        public double MedianWhiskerX1
        {
            get { return (double)GetValue(MedianWhiskerX1Property); }
            set { SetValue(MedianWhiskerX1Property, value); }
        }

        /// <summary>
        /// Get or Set TemplateProperty
        /// </summary>
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }
        #endregion

        #region Members
        /// <summary>
        /// The median1
        /// </summary>
        private ChartPoint m_mB;

        /// <summary>
        /// The median2
        /// </summary>
        private ChartPoint m_mE;

        /// <summary>
        /// The quarter1
        /// </summary>
        private ChartPoint m_q1B;

        /// <summary>
        /// The quarter2
        /// </summary>
        private ChartPoint m_q2E;

        /// <summary>
        /// The whisker1
        /// </summary>
        private ChartPoint m_w1B;

        /// <summary>
        /// The whisker1 
        /// </summary>
        private ChartPoint m_w1M;

        /// <summary>
        /// The whisker1
        /// </summary>
        private ChartPoint m_w1E;

        /// <summary>
        /// The whisker2
        /// </summary>
        private ChartPoint m_w2B;

        /// <summary>
        /// The whisker2
        /// </summary>
        private ChartPoint m_w2M;

        /// <summary>
        /// The whisker2
        /// </summary>
        private ChartPoint m_w2E;
        private ChartPoint m_outlier;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBoxAndWhiskerSegment"/> class.
        /// </summary>
        /// <param name="mB">The ChartPoint m B.</param>
        /// <param name="mE">The ChartPoint m E.</param>
        /// <param name="q1B">The ChartPoint q1 B.</param>
        /// <param name="q2E">The ChartPoint q2 E.</param>
        /// <param name="w1B">The ChartPoint w1 B.</param>
        /// <param name="w1M">The ChartPoint w1 M.</param>
        /// <param name="w1E">The ChartPoint w1 E.</param>
        /// <param name="w2B">The ChartPoint w2 B.</param>
        /// <param name="w2M">The ChartPoint w2 M.</param>
        /// <param name="w2E">The ChartPoint w2 E.</param>
        /// <param name="outlier">The outlier points</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The Chartseries.</param>
        internal ChartBoxAndWhiskerSegment(
            ChartPoint mB,
            ChartPoint mE,
            ChartPoint q1B,
            ChartPoint q2E,
            ChartPoint w1B,
          ChartPoint w1M,
            ChartPoint w1E,
          ChartPoint w2B,
            ChartPoint w2M,
            ChartPoint w2E,
            ChartPoint outlier,
          ChartPoint correspondingPoint,
            ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
            this.Template=ResourceManager.GetSeriesTemplate(typeof(ChartBoxAndWhiskerType), ChartTypes.BoxAndWhisker);
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

            m_mB = mB;
            m_mE = mE;
            m_q1B = q1B;
            m_q2E = q2E;
            m_w1B = w1B;
            m_w1M = w1M;
            m_w1E = w1E;
            m_w2B = w2B;
            m_w2M = w2M;
            m_w2E = w2E;
            m_outlier = outlier;
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);
            Point boxPt1 = transformer.TransformToVisible(m_q1B.X, m_q1B.Values[0], series);
            Point boxPt2 = transformer.TransformToVisible(m_q2E.X, m_q2E.Values[0], series);

            Point topWhiskerPt1 = transformer.TransformToVisible(m_w1B.X, m_w1B.Values[0], series);
            Point topWhiskerPt2 = transformer.TransformToVisible(m_w1E.X, m_w1E.Values[0], series);

            Point bottomWhiskerPt1 = transformer.TransformToVisible(m_w2B.X, m_w2B.Values[0], series);
            Point bottomWhiskerPt2 = transformer.TransformToVisible(m_w2E.X, m_w2E.Values[0], series);

            Point centerLineWhiskerPt1 = transformer.TransformToVisible(m_w1M.X, m_w1M.Values[0], series);
            Point centerLineWhiskerPt2 = transformer.TransformToVisible(m_w2M.X, m_w2M.Values[0], series);

            Point medianWhiskerPt1 = transformer.TransformToVisible(m_mB.X, m_mB.Values[0], series);
            Point medianWhiskerPt2 = transformer.TransformToVisible(m_mE.X, m_mE.Values[0], series);

            Rect boxRect = new Rect(boxPt1, boxPt2);

            Point pt = transformer.TransformToVisible(m_outlier.X, m_outlier.Values[0], series);

            this.X1 = pt.X;
            this.Y1 = pt.Y;

            this.X = boxRect.X;
            this.Y = boxRect.Y;
            this.Width = boxRect.Width;
            this.Height = boxRect.Height;

            this.TopWhiskerX1 = topWhiskerPt1.X;
            this.TopWhiskerX2 = topWhiskerPt2.X;
            this.TopWhiskerY1 = topWhiskerPt1.Y;
            this.TopWhiskerY2 = topWhiskerPt2.Y;

            this.BottomWhiskerX1 = bottomWhiskerPt1.X;
            this.BottomWhiskerX2 = bottomWhiskerPt2.X;
            this.BottomWhiskerY1 = bottomWhiskerPt1.Y;
            this.BottomWhiskerY2 = bottomWhiskerPt2.Y;

            this.CenterLineWhiskerX1 = centerLineWhiskerPt1.X;
            this.CenterLineWhiskerX2 = centerLineWhiskerPt2.X;
            this.CenterLineWhiskerY1 = centerLineWhiskerPt1.Y;
            this.CenterLineWhiskerY2 = centerLineWhiskerPt2.Y;

            this.MedianWhiskerX1 = medianWhiskerPt1.X;
            this.MedianWhiskerX2 = medianWhiskerPt2.X;
            this.MedianWhiskerY1 = medianWhiskerPt1.Y;
            this.MedianWhiskerY2 = medianWhiskerPt2.Y;
        }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.m_mB = null;
            this.m_mE = null;
            this.m_outlier = null;
            this.m_q1B = null;
            this.m_q2E = null;
            this.m_w1B = null;
            this.m_w1E = null;
            this.m_w1M = null;
            this.m_w2B = null;
            this.m_w2E = null;
            this.m_w2M = null;
        }
    }

    /// <summary>
    /// Class implementation for ChartBoxAndWhiskerType.
    /// </summary>
    public class ChartBoxAndWhiskerType : ChartType
    {
        #region Dependency properties

        /// <summary>
        /// DefaultOutlierVisible dependency property 
        /// </summary>
        public static readonly DependencyProperty DefaultOutlierVisibleProperty =
           DependencyProperty.RegisterAttached("DefaultOutlierVisible", typeof(bool), typeof(ChartBoxAndWhiskerType), new PropertyMetadata(false, new PropertyChangedCallback(OnOutlierVisibleChanged)));

        #endregion

        #region Attached properties

        /// <summary>
        /// Gets the default outlier visible.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <returns>Returns bool value OutlierVisibility</returns>
        public static bool GetDefaultOutlierVisible(ChartSeries obj)
        {
            return (bool)obj.GetValue(DefaultOutlierVisibleProperty);
        }

        /// <summary>
        /// Sets the default outlier visible.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetDefaultOutlierVisible(ChartSeries obj, bool value)
        {
            obj.SetValue(DefaultOutlierVisibleProperty, value);
        }
        #endregion

        #region methods

        /// <summary>
        /// Calculates the segments of specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points"></param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            bool outlier = GetDefaultOutlierVisible(series);
            SetRange(series, points, 5);
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
            double sbsCenter = 0.4 * (series.XAxis.VisibleInterval < 1 && series.Area.MinDataInterval < 1d ? series.Area.MinDataInterval : 1d);
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
            for (int i = 0; i < points.Count; i++)
            {
                ChartPoint cdp = points[i];

                #region preparing data of point to retrieve statistical median, quartiles, whiskers etc.
                double[] dwiA = cdp.Values.Clone() as double[];
                int statLen = dwiA.Length;

                Array.Sort<double>(dwiA);

                ChartPoint q1, q2, whisker1, whisker2;
                double interquartileRange;

                ChartPoint median = GetStatisticalMedian(cdp.X, dwiA);
                int mid = (dwiA.Length % 2 == 0) ? dwiA.Length / 2 : (dwiA.Length / 2) + 1;
                double[] d1 = (from dd in dwiA.Take<double>(mid) select dd).ToArray<double>();
                double[] d2 = (from dd in dwiA.Reverse<double>().Take<double>(mid).Reverse<double>() select dd).ToArray<double>();

                q1 = GetStatisticalMedian(cdp.X, d1);
                q2 = GetStatisticalMedian(cdp.X, d2);

                interquartileRange = Math.Abs(q2.Y - q1.Y);
                double outliermin = q1.Y - (1.5d * interquartileRange);
                double outliermax = q2.Y + (1.5d * interquartileRange);

                double extrememin = q1.Y - (3d * interquartileRange);
                double extrememax = q2.Y + (3d * interquartileRange);

                double[] outlierdata = (from d in dwiA where !(d >= outliermin && d <= outliermax) select d).ToArray<double>();
                double[] extremedata = (from d in dwiA where !(d >= extrememin && d <= extrememax) select d).ToArray<double>();

                whisker1 = new ChartPoint(cdp.X, dwiA[0]);
                whisker2 = new ChartPoint(cdp.X, dwiA[statLen - 1]);
                if (outlier == false)
                {
                    foreach (double d in outlierdata)
                    {
                        ChartPoint point = new ChartPoint(cdp.X + sbsInfo.Median - 0.4 + (series.XAxis.VisibleRange.Start * (-1)), d + (series.YAxis.VisibleRange.Start * (-1)));
                        series.Segments.Add(new ScatterSegment(point, new ChartPoint(0, 0), points[i], series));
                    }

                    for (int j = 0; j < statLen; j++)
                    {
                        double dJ = dwiA[j];
                        whisker1 = new ChartPoint(cdp.X, dJ);
                        if (Math.Abs(whisker1.Y - q1.Y) <= 3.0f / 2 * interquartileRange)
                        {
                            break;
                        }
                    }

                    foreach (double d in extremedata)
                    {
                        ChartPoint point = new ChartPoint(cdp.X + sbsInfo.Median - 0.4 + (series.XAxis.VisibleRange.Start * (-1)), d + (series.YAxis.VisibleRange.Start * (-1)));
                        series.Segments.Add(new ScatterSegment(point, new ChartPoint(0, 0), points[i], series));
                    }

                    for (int j = statLen - 1; j >= 0; j--)
                    {
                        double dJ = dwiA[j];
                        whisker2 = new ChartPoint(cdp.X, dJ);
                        if (Math.Abs(whisker2.Y - q2.Y) <= 3.0f / 2 * interquartileRange)
                        {
                            break;
                        }
                    }
                }
                
                #endregion //END preparing data of point to retrieve statistical median and quartiles

                //// ^- E means End
                //// ^- M means Middle
                //// ^- B means Beginning

                ChartPoint mB = new ChartPoint(cdp.X + sbsInfo.Start - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)), median.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint mE = new ChartPoint(cdp.X + sbsInfo.End - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)), median.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint q1B = new ChartPoint(q1.X + sbsInfo.Start - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)), q1.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint q2E = new ChartPoint(q2.X + sbsInfo.End - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)), q2.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint w1B = new ChartPoint(whisker1.X + sbsInfo.Start - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)), whisker1.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint w2B = new ChartPoint(whisker2.X + sbsInfo.Start - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)), whisker2.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint w1M = new ChartPoint(whisker1.X + sbsInfo.Median - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)), whisker1.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint w2M = new ChartPoint(whisker2.X + sbsInfo.Median - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)), whisker2.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint w1E = new ChartPoint(whisker1.X + sbsInfo.End - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)), whisker1.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint w2E = new ChartPoint(whisker2.X + sbsInfo.End - sbsCenter + (series.XAxis.VisibleRange.Start * (-1)), whisker2.Y + (series.YAxis.VisibleRange.Start * (-1)));

                series.Segments.Add(new ChartBoxAndWhiskerSegment(mB, mE, q1B, q2E, w1B, w1M, w1E, w2B, w2M, w2E, points[i], cdp, series));
            }
        }

        private static void OnOutlierVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            this.Calculate(series);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// ChartBoxAndWhiskerType ToString method
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return "BoxAndWhisker";
        }

        /// <summary>
        /// Gets the statistical median.
        /// </summary>
        /// <param name="x">The double value x.</param>
        /// <param name="dwi">The double value dwi.</param>
        /// <returns>The median value</returns>
        private static ChartPoint GetStatisticalMedian(double x, double[] dwi)
        {
            int statLen = dwi.Length;
            ChartPoint median;

            if (statLen % 2 == 1)
            {
                median = new ChartPoint(x, dwi[(statLen - 1) / 2]);
            }
            else
            {
                double d1 = dwi[statLen / 2];
                double d2 = dwi[(statLen / 2) - 1];
                median = new ChartPoint(x, (d1 + d2) / 2.0d);
            }

            return median;
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
