// <copyright file="ChartHistogramType.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Shapes;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Controls;

    /// <summary>
    /// Represents segment that is a part of histogram chart type.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartHistogramSegment : ChartSegment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ChartHistogramSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ChartHistogramSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartHistogramSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ChartHistogramSegment), new PropertyMetadata(0d));
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_bottomLeftPoint
        /// </summary>
        private IChartDataPoint m_bottomLeftPoint;

        /// <summary>
        /// Initializes m_topRightPoint
        /// </summary>
        private IChartDataPoint m_topRightPoint;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X co-ordinate of segment. This is a dependency property.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate of segment. This is a dependency property.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of segment. This is a dependency property.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of segment. This is a dependency property.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Gets the segment's points count.
        /// </summary>
        /// <value>The point count.</value>
        public int PointCount
        {
            get { return seriesCorrespondingPoints.Length; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ChartHistogramSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Default tenmplate is being assigned automatically.
        /// </remarks>
        static ChartHistogramSegment()
        {
            Type type = typeof(ChartHistogramSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartHistogramSegment"/> class.
        /// </summary>
        /// <param name="bottomLeftPoint">The bottom left point.</param>
        /// <param name="topRightPoint">The top right point.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        internal ChartHistogramSegment(IChartDataPoint bottomLeftPoint, IChartDataPoint topRightPoint, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {
            m_bottomLeftPoint = bottomLeftPoint;
            m_topRightPoint = topRightPoint;

            this.SetXRange(bottomLeftPoint.X, topRightPoint.X);
            this.SetYRange(bottomLeftPoint.Y, topRightPoint.Y);
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        /// <seealso cref="ChartHistogramSegment"/>
        public override void Update(IChartTransformer transformer)
        {
            if (this.Interior.CanFreeze)
            {
                this.Interior.Freeze();
            }
            if (this.Stroke.CanFreeze)
            {
                this.Stroke.Freeze();
            }
            Point blPoint = transformer.TransformToVisible(m_bottomLeftPoint.X, m_bottomLeftPoint.Y);
            Point trPoint = transformer.TransformToVisible(m_topRightPoint.X, m_topRightPoint.Y);
            Rect columnRect = new Rect(blPoint, trPoint);

            this.X = columnRect.X;
            this.Y = columnRect.Y;
            this.Width = columnRect.Width;
            this.Height = columnRect.Height;
        }
        #endregion
    }

    /// <summary>
    /// Represents histogram distribution segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartHistogramDistributionSegment : ChartSegment
    {
        #region Members
        /// <summary>
        /// Initializes m_points
        /// </summary>
        private IChartDataPoint[] m_points;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(ChartHistogramDistributionSegment), new PropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the corresponding points collection.
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
        /// Initializes static members of the <see cref="ChartHistogramDistributionSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Segment template is being assigned automatically.
        /// </remarks>
        static ChartHistogramDistributionSegment()
        {
            Type type = typeof(ChartHistogramDistributionSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartHistogramDistributionSegment"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        internal ChartHistogramDistributionSegment(IChartDataPoint[] points, ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {
            m_points = points;
            xRange = DoubleRange.Empty;
            yRange = DoubleRange.Empty;

            foreach (IChartDataPoint cdpt in points)
            {
                xRange += cdpt.X;
                yRange += cdpt.Y;
            }
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            PointCollection points = new PointCollection();

            for (int i = 0; i < m_points.Length; i++)
            {
                points.Add(transformer.TransformToVisible(m_points[i].X, m_points[i].Y));
            }

            this.Points = points;
        }
        #endregion
    }

    /// <summary>
    /// Represents ChartHistogramType class
    /// </summary>
    /// <remarks>
    /// Histogram is a bar (column) chart of a frequency distribution in which the
    /// widths of the bars are proportional to the classes into which the variable has
    /// been divided and the heights of the bars are proportional to the class
    /// frequencies. The categories are usually specified as non overlapping intervals
    /// of some variable. The categories (bars) must be adjacent.
    /// </remarks>
    /// <seealso cref="ChartHistogramSegment"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartHistogramType : ChartType
    {
        #region Constants
        /// <summary>
        /// Initializes c_distributionPointsCount
        /// </summary>
        private const int C_distributionPointsCount = 500;

        /// <summary>
        /// Initializes c_sqrtDoublePI
        /// </summary>
        private readonly static double c_sqrtDoublePI = Math.Sqrt(2 * Math.PI);
        #endregion

        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for NumberIntervals.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalOfHistogramProperty =
      DependencyProperty.RegisterAttached("IntervalOfHistogram", typeof(double), typeof(ChartHistogramType), new ChartPropertyMetadata(1d, ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Using a DependencyProperty as the backing store for DrawNormalDistribution.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DrawNormalDistributionProperty =
      DependencyProperty.RegisterAttached("DrawNormalDistribution", typeof(bool), typeof(ChartHistogramSegment), new ChartPropertyMetadata(false, ChartPropertyMetadataOptions.AffectsUpdate));
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartHistogramType"/> class.
        /// </summary>
        internal ChartHistogramType()
        {
        }

        #region Properties
        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.Indexed;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the draw normal distribution attached property value.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Normal Distribution</returns>
        public static bool GetDrawNormalDistribution(ChartSeries series)
        {
            return (bool)series.GetValue(DrawNormalDistributionProperty);
        }

        /// <summary>
        /// Sets the draw normal distribution.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetDrawNormalDistribution(ChartSeries series, bool value)
        {
            series.SetValue(DrawNormalDistributionProperty, value);
        }

        /// <summary>
        /// Gets the interval of histogram.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Interval Of Histogram</returns>
        public static double GetIntervalOfHistogram(ChartSeries series)
        {
            return (double)series.GetValue(IntervalOfHistogramProperty);
        }

        /// <summary>
        /// Sets the interval of histogram.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetIntervalOfHistogram(ChartSeries series, double value)
        {
            series.SetValue(IntervalOfHistogramProperty, value);
            series.ChartType.Update(series);
        }

        /// <summary>
        /// Calculates the segments of specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        public override void Calculate(ChartSeries series)
        {
            int count = series.PointsCount;
            List<ChartSegment> drawingList = new List<ChartSegment>(count);
            ChartIndexedDataPoint[] cdpwiA = new ChartIndexedDataPoint[count];

            for (int i = 0; i < count; i++)
            {
                cdpwiA[i] = new ChartIndexedDataPoint(series.GetPoint(i), i);
            }

            Array.Sort(cdpwiA, new ChartIndexedDataPointByXComparer());

            double interval = GetIntervalOfHistogram(series);
            double start = 0;
            if (cdpwiA.Length  > 0)
            {
                start = ChartMath.Round(cdpwiA[0].DataPoint.X, interval, false);
            }
            double position = start;
            int intervalsCount = 0;

            List<ChartIndexedDataPoint> points = new List<ChartIndexedDataPoint>();

            for (int i = 0, ci = cdpwiA.Length; i < ci; i++)
            {
                IChartDataPoint cdpt = cdpwiA[i].DataPoint;

                while (cdpt.X > position + interval)
                {
                    if (points.Count > 0)
                    {
                        ChartPoint cdpt1 = new ChartPoint(position, points.Count);
                        ChartPoint cdpt2 = new ChartPoint(position + interval, 0);

                        drawingList.Add(new ChartHistogramSegment(cdpt1, cdpt2, points.ToArray(), series));
                        points.Clear();
                    }

                    position += interval;
                    intervalsCount++;
                }

                points.Add(cdpwiA[i]);
            }

            if (points.Count > 0)
            {
                ChartPoint cdpt1 = new ChartPoint(position, points.Count);
                ChartPoint cdpt2 = new ChartPoint(position + interval, 0);

                intervalsCount++;

                drawingList.Add(new ChartHistogramSegment(cdpt1, cdpt2, points.ToArray(), series));
                points.Clear();
            }

            #region Normal Distribution
            if (GetDrawNormalDistribution(series))
            {
                double m, dev;
                GetHistogramMeanAndDeviation(cdpwiA, out m, out dev);

                IChartDataPoint[] distributionPoints = new IChartDataPoint[C_distributionPointsCount];

                double min = start;
                double max = start + intervalsCount * interval;
                double del = (max - min) / (C_distributionPointsCount - 1);

                for (int i = 0; i < C_distributionPointsCount; i++)
                {
                    double tx = min + i * del;
                    double ty = NormalDistribution(tx, m, dev) * cdpwiA.Length * interval;

                    distributionPoints[i] = new ChartPoint(tx, ty);
                }

                drawingList.Add(new ChartHistogramDistributionSegment(distributionPoints, cdpwiA, series));
            }
            #endregion

            drawingList.Reverse();

            foreach (ChartSegment segment in drawingList)
            {
                series.Segments.Add(segment);
            }
        }

        /// <summary>
        /// Updates chart.
        /// </summary>
        /// <param name="series">The Chartseries </param>
        ///  /// <seealso cref="ChartHistogramType"/>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.Calculate(series);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the histogram mean and deviation.
        /// </summary>
        /// <param name="cpwiA">The cpwi A.</param>
        /// <param name="mean">The mean value.</param>
        /// <param name="standartDeviation">The standart deviation.</param>
        private static void GetHistogramMeanAndDeviation(ChartIndexedDataPoint[] cpwiA, out double mean, out double standartDeviation)
        {
            int count = cpwiA.Length;
            double sum = 0;

            for (int i = 0; i < count; i++)
            {
                sum += cpwiA[i].DataPoint.X;
            }

            mean = sum / count;

            sum = 0;

            for (int i = 0; i < count; i++)
            {
                double dif = cpwiA[i].DataPoint.X - mean;
                sum += dif * dif;
            }

            standartDeviation = Math.Sqrt(sum / count);
        }

        /// <summary>
        /// Normal Distribution function.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="m">The m value.</param>
        /// <param name="sigma">The sigma value.</param>
        /// <returns>The Normal Distribution</returns>
        private static double NormalDistribution(double x, double m, double sigma)
        {
            return Math.Exp(-(x - m) * (x - m) / (2 * sigma * sigma)) / (sigma * c_sqrtDoublePI);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Histogram";
        }
        #endregion
    }
}
