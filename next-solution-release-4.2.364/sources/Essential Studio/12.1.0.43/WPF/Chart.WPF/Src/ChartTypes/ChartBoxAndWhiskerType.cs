// <copyright file="ChartBoxAndWhiskerType.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents Box and Whisker chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartBoxAndWhiskerSegment : ChartSegment
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
            DependencyProperty.Register("MedianWhiskerX1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerX2 dependency property.
        /// </summary>
        public static readonly DependencyProperty MedianWhiskerX2Property =
            DependencyProperty.Register("MedianWhiskerX2", typeof(double), typeof(ChartBoxAndWhiskerSegment), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerY1 dependency property.
        /// </summary>
        public static readonly DependencyProperty MedianWhiskerY1Property =
            DependencyProperty.Register("MedianWhiskerY1", typeof(double), typeof(ChartBoxAndWhiskerSegment), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies the MedianWhiskerY2 dependency property.
        /// </summary>
        public static readonly DependencyProperty MedianWhiskerY2Property =
            DependencyProperty.Register("MedianWhiskerY2", typeof(double), typeof(ChartBoxAndWhiskerSegment), new UIPropertyMetadata(0d));
        #endregion

        #region Properties
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
        #endregion

        #region Members
        /// <summary>
        /// The median1
        /// </summary>
        private IChartDataPoint m_mB;

        /// <summary>
        /// The median2
        /// </summary>
        private IChartDataPoint m_mE;

        /// <summary>
        /// The quarter1
        /// </summary>
        private IChartDataPoint m_q1B;

        /// <summary>
        /// The quarter2
        /// </summary>
        private IChartDataPoint m_q2E;

        /// <summary>
        /// The whisker1
        /// </summary>
        private IChartDataPoint m_w1B;

        /// <summary>
        /// The whisker1 
        /// </summary>
        private IChartDataPoint m_w1M;

        /// <summary>
        /// The whisker1
        /// </summary>
        private IChartDataPoint m_w1E;

        /// <summary>
        /// The whisker2
        /// </summary>
        private IChartDataPoint m_w2B;

        /// <summary>
        /// The whisker2
        /// </summary>
        private IChartDataPoint m_w2M;

        /// <summary>
        /// The whisker2
        /// </summary>
        private IChartDataPoint m_w2E;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes static members of the <see cref="ChartBoxAndWhiskerSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Default segment template is being initialized automatically.
        /// </remarks>
        static ChartBoxAndWhiskerSegment()
        {
            Type type = typeof(ChartBoxAndWhiskerSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBoxAndWhiskerSegment"/> class.
        /// </summary>
        /// <param name="mB">The IChartDataPoint m B.</param>
        /// <param name="mE">The IChartDataPoint m E.</param>
        /// <param name="q1B">The IChartDataPoint q1 B.</param>
        /// <param name="q2E">The IChartDataPoint q2 E.</param>
        /// <param name="w1B">The IChartDataPoint w1 B.</param>
        /// <param name="w1M">The IChartDataPoint w1 M.</param>
        /// <param name="w1E">The IChartDataPoint w1 E.</param>
        /// <param name="w2B">The IChartDataPoint w2 B.</param>
        /// <param name="w2M">The IChartDataPoint w2 M.</param>
        /// <param name="w2E">The IChartDataPoint w2 E.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The Chartseries.</param>
        internal ChartBoxAndWhiskerSegment(
            IChartDataPoint mB,
            IChartDataPoint mE,
            IChartDataPoint q1B,
            IChartDataPoint q2E,
            IChartDataPoint w1B,
          IChartDataPoint w1M,
            IChartDataPoint w1E,
          IChartDataPoint w2B,
            IChartDataPoint w2M,
            IChartDataPoint w2E,
          ChartIndexedDataPoint correspondingPoint,
            ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint })
        {
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

            this.SetXRange(m_mB.X, m_mE.X, m_mB.X, m_mE.X);
            this.SetYRange(m_w1B.Y, m_w2B.Y, m_w1B.Y, m_w2B.Y);
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">The transformer</param>
        /// <seealso cref="ChartBoxAndWhiskerSegment"/>
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
            base.Update(transformer);
            Point boxPt1 = transformer.TransformToVisible(m_q1B.X, m_q1B.Values[0]);
            Point boxPt2 = transformer.TransformToVisible(m_q2E.X, m_q2E.Values[0]);

            Point topWhiskerPt1 = transformer.TransformToVisible(m_w1B.X, m_w1B.Values[0]);
            Point topWhiskerPt2 = transformer.TransformToVisible(m_w1E.X, m_w1E.Values[0]);

            Point bottomWhiskerPt1 = transformer.TransformToVisible(m_w2B.X, m_w2B.Values[0]);
            Point bottomWhiskerPt2 = transformer.TransformToVisible(m_w2E.X, m_w2E.Values[0]);

            Point centerLineWhiskerPt1 = transformer.TransformToVisible(m_w1M.X, m_w1M.Values[0]);
            Point centerLineWhiskerPt2 = transformer.TransformToVisible(m_w2M.X, m_w2M.Values[0]);

            Point medianWhiskerPt1 = transformer.TransformToVisible(m_mB.X, m_mB.Values[0]);
            Point medianWhiskerPt2 = transformer.TransformToVisible(m_mE.X, m_mE.Values[0]);

            Rect boxRect = new Rect(boxPt1, boxPt2);

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
    }

    /// <summary>
    /// Initializes ChartBoxAndWhiskerType
    /// </summary>
    /// <seealso cref="ChartBoxAndWhiskerType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartBoxAndWhiskerType : ChartType
    {
        #region Properties
        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.SideBySide | ChartTypeFlags.Indexed;
            }
        }
        #endregion

        #region Dependency properties

        /// <summary>
        /// DefaultOutlierVisible dependency property 
        /// </summary>
        public static readonly DependencyProperty DefaultOutlierVisibleProperty =
           DependencyProperty.RegisterAttached("DefaultOutlierVisible", typeof(bool), typeof(ChartBoxAndWhiskerType), new ChartPropertyMetadata(true, new PropertyChangedCallback(OnVisibleChanged), ChartPropertyMetadataOptions.AffectsUpdate));


        /// <summary>
        /// Called when DefaultOutlierVisible property changed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ChartSeries ser = obj as ChartSeries;
            if (ser != null)
            {
                if (ser.ChartType is ChartBoxAndWhiskerType)
                {
                    ((ChartBoxAndWhiskerType)(ser.ChartType)).Update(ser);
                }
            }
        }
        #endregion

        #region Attached properties

        /// <summary>
        /// Gets the default outlier visible.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <returns>Returns bool value OutlierVisibility</returns>
        public static bool GetDefaultOutlierVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(DefaultOutlierVisibleProperty);
        }

        /// <summary>
        /// Sets the default outlier visible.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <seealso cref="ChartBoxAndWhiskerType"/>
        public static void SetDefaultOutlierVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(DefaultOutlierVisibleProperty, value);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Calculates the segments of specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        public override void Calculate(ChartSeries series)
        {
            bool outlier = GetDefaultOutlierVisible(series);
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
            int count = series.PointsCount;

            ChartIndexedDataPoint[] cdpwiA = new ChartIndexedDataPoint[count];

            for (int i = 0; i < count; i++)
            {
                cdpwiA[i] = new ChartIndexedDataPoint(series.GetPoint(i), i);
            }

            Array.Sort(cdpwiA, new ChartIndexedDataPointByXComparer());

            for (int i = 0; i < cdpwiA.Length; i++)
            {
                IChartDataPoint cdp = cdpwiA[i].DataPoint;

                #region preparing data of point to retrieve statistical median, quartiles, whiskers etc.
                double[] dwiA = cdp.Values.Clone() as double[];
                int statLen = dwiA.Length;

                Array.Sort<double>(dwiA);

                IChartDataPoint q1, q2, whisker1, whisker2;
                double interquartileRange;

                IChartDataPoint median = GetStatisticalMedian(cdp.X, dwiA);

                if (statLen % 2 == 0)
                {
                    int len = statLen / 2;
                    double[] q1Stat = new double[len];
                    double[] q2Stat = new double[len];

                    Array.Copy(dwiA, 0, q1Stat, 0, len);
                    Array.Copy(dwiA, len, q2Stat, 0, len);

                    q1 = GetStatisticalMedian(cdp.X, q1Stat);
                    q2 = GetStatisticalMedian(cdp.X, q2Stat);
                }
                else
                {
                    int len = (statLen / 2) + 1;
                    double[] q1Stat = new double[len];
                    double[] q2Stat = new double[len];

                    Array.Copy(dwiA, 0, q1Stat, 0, len);
                    Array.Copy(dwiA, len - 1, q2Stat, 0, len);

                    q1 = GetStatisticalMedian(cdp.X, q1Stat);
                    q2 = GetStatisticalMedian(cdp.X, q2Stat);
                }

                interquartileRange = Math.Abs(q2.Y - q1.Y);

                Hashtable distantPointsQ1 = new Hashtable(20, 0.1f);
                Hashtable distantPointsQ2 = new Hashtable(20, 0.1f);

                whisker1 = new ChartPoint(cdp.X, dwiA[0]);

                ////getting number of dots at whiskers 
                for (int j = 0; j < statLen / 2; j++)
                {
                    double dJ = dwiA[j];
                    IChartDataPoint p1 = new ChartPoint(cdp.X, dJ);
                    if  /*(p1.Y >= q1.Y) &&*/ (Math.Abs(p1.Y - q1.Y) > 3.0f / 2 * interquartileRange)
                    {
                        if (distantPointsQ1.Contains(p1.Y))
                        {
                            int ii = (int)distantPointsQ1[p1.Y];
                            ii++;
                            distantPointsQ1[p1.Y] = ii;
                        }
                        else
                        {
                            distantPointsQ1.Add(p1.Y, 1);
                        }
                    }
                }

                if (outlier == false)
                {
                    if (distantPointsQ1.Count > 0)
                    {
                        series.Segments.Add(new ChartSymbolSegment(whisker1, cdpwiA[0], series));
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
                }

                whisker2 = new ChartPoint(cdp.X, dwiA[statLen - 1]);

                ////getting number of dots at whiskers 
                for (int j = statLen - 1; j >= statLen / 2; j--)
                {
                    double dJ = dwiA[j];
                    IChartDataPoint p2 = new ChartPoint(cdp.X, dJ);
                    if (Math.Abs(p2.Y - q2.Y) > 3.0f / 2 * interquartileRange)
                    {
                        if (distantPointsQ2.Contains(p2.Y))
                        {
                            int ii = (int)distantPointsQ2[p2.Y];
                            ii++;
                            distantPointsQ2[p2.Y] = ii;
                        }
                        else
                        {
                            distantPointsQ2.Add(p2.Y, 1);
                        }
                    }
                }

                if (outlier == false)
                {
                    if (distantPointsQ2.Count > 0)
                    {
                        series.Segments.Add(new ChartSymbolSegment(whisker2, cdpwiA[0], series));
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

                ChartPoint mB = new ChartPoint(cdp.X - sbsInfo.Start, median.Y);
                ChartPoint mE = new ChartPoint(cdp.X - sbsInfo.End, median.Y);
                ChartPoint q1B = new ChartPoint(q1.X - sbsInfo.Start, q1.Y);
                ChartPoint q2E = new ChartPoint(q2.X - sbsInfo.End, q2.Y);
                ChartPoint w1B = new ChartPoint(whisker1.X - sbsInfo.Start, whisker1.Y);
                ChartPoint w2B = new ChartPoint(whisker2.X - sbsInfo.Start, whisker2.Y);
                ChartPoint w1M = new ChartPoint(whisker1.X - sbsInfo.Median, whisker1.Y);
                ChartPoint w2M = new ChartPoint(whisker2.X - sbsInfo.Median, whisker2.Y);
                ChartPoint w1E = new ChartPoint(whisker1.X - sbsInfo.End, whisker1.Y);
                ChartPoint w2E = new ChartPoint(whisker2.X - sbsInfo.End, whisker2.Y);

                int[] dPQ1 = new int[distantPointsQ1.Values.Count];
                int[] dPQ2 = new int[distantPointsQ2.Values.Count];

                distantPointsQ1.Values.CopyTo(dPQ1, 0);
                distantPointsQ2.Values.CopyTo(dPQ2, 0);
                if (cdpwiA[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(cdpwiA[i].DataPoint, cdpwiA[i], series, series.EmptyPointSymbolTemplate));
              
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior )
                        {
                            series.Segments.Add(new ChartBoxAndWhiskerSegment(mB, mE, q1B, q2E, w1B, w1M, w1E, w2B, w2M, w2E, cdpwiA[i], series));
                            series.Segments[series.Segments.Count - 1].Interior = series.EmptyPointInterior;
                        }
                        else 
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(cdpwiA[i].DataPoint, cdpwiA[i], series, series.EmptyPointSymbolTemplate));
                        }

                    }
                }
                else
                {
                    series.Segments.Add(new ChartBoxAndWhiskerSegment(mB, mE, q1B, q2E, w1B, w1M, w1E, w2B, w2M, w2E, cdpwiA[i], series));
                }
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        /// <seealso cref="ChartBoxAndWhiskerType"/>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.Calculate(series);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// ChartBoxAndWhiskerType ToString method
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartBoxAndWhiskerType"/>
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
        private static IChartDataPoint GetStatisticalMedian(double x, double[] dwi)
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
    }
}

