// <copyright file="ChartHiLoOpenCloseType.cs" company="Syncfusion">
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
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents HiLo Open-Close chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
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
        private IChartDataPoint m_sopen;

        /// <summary>
        /// Initializes m_eOpen
        /// </summary>
        private IChartDataPoint m_eopen;

        /// <summary>
        /// Initializes m_sClose
        /// </summary>
        private IChartDataPoint m_sclose;

        /// <summary>
        /// Initializes m_eClose
        /// </summary>
        private IChartDataPoint m_eclose;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ChartHiLoOpenCloseSegment"/> class.
        /// </summary>
        /// <remarks>Default template is being assigned automatically.</remarks>
        static ChartHiLoOpenCloseSegment()
        {
            Type type = typeof(ChartHiLoOpenCloseSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.ChartHiLoOpenCloseSegment">ChartHiLoOpenCloseSegment</see> class. 
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
        internal ChartHiLoOpenCloseSegment(IChartDataPoint hi, IChartDataPoint lo, IChartDataPoint sOpen, IChartDataPoint eopen, IChartDataPoint sclose, IChartDataPoint eclose, ChartIndexedDataPoint correspondingPoint1, ChartSeries series)
            : base(hi, lo, correspondingPoint1, series)
        {
            m_sopen = sOpen;
            m_eopen = eopen;
            m_sclose = sclose;
            m_eclose = eclose;

            xRange += new DoubleRange(ChartMath.Min(sclose.X, eclose.X, sOpen.X, eopen.X), ChartMath.Max(sclose.X, eclose.X, sOpen.X, eopen.X));
            yRange += new DoubleRange(ChartMath.Min(sclose.Y, eclose.Y, sOpen.Y, eopen.Y), ChartMath.Max(sclose.Y, eclose.Y, sOpen.Y, eopen.Y));
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        /// <seealso cref="ChartHiLoOpenCloseSegment"/>
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

            Point sopoint = transformer.TransformToVisible(m_sopen.X, m_sopen.Y);
            Point eopoint = transformer.TransformToVisible(m_eopen.X, m_eopen.Y);
            Point scpoint = transformer.TransformToVisible(m_sclose.X, m_sclose.Y);
            Point ecpoint = transformer.TransformToVisible(m_eclose.X, m_eclose.Y);

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
    }

    /// <summary>
    /// Represents ChartHiLoOpenCloseType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartHiLoOpenCloseType : ChartHiLoType
    {
        #region Properties
        /// <summary>
        /// Gets the require data count.
        /// </summary>
        /// <value>The require data count.</value>
        public override int RequiresDataCount
        {
            get
            {
                return 4;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Converts ChartHiLoOpenCloseType to string 
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartHiLoOpenCloseType"/>
        public override string ToString()
        {
            return "HiLoOpenClose";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);

            double center = sbsInfo.Median;
            double left = sbsInfo.Start;
            double right = sbsInfo.End;

            for (int i = 0; i < points.Length; i++)
            {
                ChartPoint hipoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[1]);
                ChartPoint lopoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[0]);
                ChartPoint sopoint = new ChartPoint();
                ChartPoint eopoint = new ChartPoint();
                ChartPoint scpoint = new ChartPoint();
                ChartPoint ecpoint = new ChartPoint();
                if (points[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints )
                    {
                        if (series.EmptyPointValue == EmptyPointValue.Zero)
                        {
                             sopoint = new ChartPoint(points[i].DataPoint.X + left, points[i].DataPoint.Values[1]);
                             eopoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[1]);

                             scpoint = new ChartPoint(points[i].DataPoint.X + right, points[i].DataPoint.Values[0]);
                             ecpoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[0]);
 
                        }
                        else
                        {
                             sopoint = new ChartPoint(points[i].DataPoint.X + left, points[i].DataPoint.Values[2]);
                             eopoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[2]);

                             scpoint = new ChartPoint(points[i].DataPoint.X + right, points[i].DataPoint.Values[3]);
                             ecpoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[3]);
 
                        }
 
                    }
                }
                else
                {
                     sopoint = new ChartPoint(points[i].DataPoint.X + left, points[i].DataPoint.Values[2]);
                     eopoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[2]);

                     scpoint = new ChartPoint(points[i].DataPoint.X + right, points[i].DataPoint.Values[3]);
                     ecpoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[3]);
                }
                if (points[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            series.Segments.Add(new ChartHiLoOpenCloseSegment(hipoint, lopoint, sopoint, eopoint, scpoint, ecpoint, points[i], series));
                            series.Segments[series.Segments.Count - 1].Interior = series.EmptyPointInterior;
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                        }
                        else
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                        }
                    }
                }
                else
                {
                    series.Segments.Add(new ChartHiLoOpenCloseSegment(hipoint, lopoint, sopoint, eopoint, scpoint, ecpoint, points[i], series));
                }
            }

            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                int index = -1;
                for (int i = 0; i < points.Length; i++)
                {
                    double x = points[i].DataPoint.X;
                    double y1 = points[i].DataPoint.Values[0];
                    double y2 = points[i].DataPoint.Values[1];
                    double y3 = points[i].DataPoint.Values[2];
                    double y4 = points[i].DataPoint.Values[3];
                     //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            {
                                series.Adornments.Add(this.CreateAdornment(series, x + center, y1, y1, y1, y2, points[i], ++index));
                                series.Adornments.Add(this.CreateAdornment(series, x + center, y2, y2, y1, y2, points[i], ++index));
                                series.Adornments.Add(this.CreateAdornment(series, x + left, y3, y3, y1, y2, points[i], ++index));
                                series.Adornments.Add(this.CreateAdornment(series, x + right, y4, y4, y1, y2, points[i], ++index));
                            }
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            {
                                if (y1 > y2)
                                    series.Adornments.Add(this.CreateAdornment(series, x + center, y1, y1, y1, y2, points[i], ++index));
                                else
                                    series.Adornments.Add(this.CreateAdornment(series, x + center, y2, y2, y1, y2, points[i], ++index));
                                if (y3 > y4)
                                    series.Adornments.Add(this.CreateAdornment(series, x + left, y3, y3, y1, y2, points[i], ++index));
                                else
                                    series.Adornments.Add(this.CreateAdornment(series, x + right, y4, y4, y1, y2, points[i], ++index));
                            }
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            {
                                if (y1 < y2)
                                    series.Adornments.Add(this.CreateAdornment(series, x + center, y1, y1, y1, y2, points[i], ++index));
                                else
                                    series.Adornments.Add(this.CreateAdornment(series, x + center, y2, y2, y1, y2, points[i], ++index));
                                if (y3 < y4)
                                    series.Adornments.Add(this.CreateAdornment(series, x + left, y3, y3, y1, y2, points[i], ++index));
                                else
                                    series.Adornments.Add(this.CreateAdornment(series, x + right, y4, y4, y1, y2, points[i], ++index));
                            }
                        }
                    }
                    else
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        {
                            series.Adornments.Add(this.CreateAdornment(series, x + center, y1, y1, y1, y2, points[i], ++index));
                            series.Adornments.Add(this.CreateAdornment(series, x + center, y2, y2, y1, y2, points[i], ++index));
                            series.Adornments.Add(this.CreateAdornment(series, x + left,  y3, y3, y1, y2, points[i], ++index));
                            series.Adornments.Add(this.CreateAdornment(series, x + right, y4, y4, y1, y2, points[i], ++index));
                        }
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                        {
                            if (y1 > y2)
                                series.Adornments.Add(this.CreateAdornment(series, x + center, y1, y1, y1, y2, points[i], ++index));
                            else
                                series.Adornments.Add(this.CreateAdornment(series, x + center, y2, y2, y1, y2, points[i], ++index));
                            if(y3 > y4)
                                series.Adornments.Add(this.CreateAdornment(series, x + left, y3, y3, y1, y2, points[i], ++index));
                            else
                                series.Adornments.Add(this.CreateAdornment(series, x + right, y4, y4, y1, y2, points[i], ++index));
                        }
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                        {
                            if (y1 < y2)
                                series.Adornments.Add(this.CreateAdornment(series, x + center, y1, y1, y1, y2, points[i], ++index));
                            else
                                series.Adornments.Add(this.CreateAdornment(series, x + center, y2, y2, y1, y2, points[i], ++index));
                            if (y3 < y4)
                                series.Adornments.Add(this.CreateAdornment(series, x + left, y3, y3, y1, y2, points[i], ++index));
                            else
                                series.Adornments.Add(this.CreateAdornment(series, x + right, y4, y4, y1, y2, points[i], ++index));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The ChartSeries</param>
        /// <param name="points">The indexed data points</param>
        /// <seealso cref="ChartHiLoOpenCloseType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }
        #endregion
    }
}
