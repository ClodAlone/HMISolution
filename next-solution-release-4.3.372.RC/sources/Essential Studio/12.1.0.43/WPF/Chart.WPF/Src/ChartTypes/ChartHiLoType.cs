// <copyright file="ChartHiLoType.cs" company="Syncfusion">
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
    using System.Windows.Data;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents Hilo chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartHiLoSegment : ChartSegment
    {
        #region dependency properties
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
        private IChartDataPoint m_point1;

        /// <summary>
        /// Initializes m_point2
        /// </summary>
        private IChartDataPoint m_point2;
        #endregion

        #region Properties
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
        /// Initializes static members of the <see cref="ChartHiLoSegment"/> class.
        /// </summary>
        /// <remarks>
        /// Default segment templates are created automatically.
        /// </remarks>
        static ChartHiLoSegment()
        {
            Type type = typeof(ChartHiLoSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartHiLoSegment"/> class.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="correspondingPoint1">The corresponding point1.</param>
        /// <param name="series">The series.</param>
        internal ChartHiLoSegment(IChartDataPoint point1, IChartDataPoint point2, ChartIndexedDataPoint correspondingPoint1, ChartSeries series)
            : base(series, new ChartIndexedDataPoint[] { correspondingPoint1 })
        {
            m_point1 = point1;
            m_point2 = point2;

            this.SetXRange(point1.X, point2.X);
            this.SetYRange(point1.Y, point2.Y);
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        /// <seealso cref="ChartHiLoSegment"/>
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

            Point point1 = transformer.TransformToVisible(m_point1.X, m_point1.Y);
            Point point2 = transformer.TransformToVisible(m_point2.X, m_point2.Y);

            this.HighX = point1.X;
            this.LowX = point2.X;
            this.HighY = point1.Y;
            this.LowY = point2.Y;
        }
        #endregion
    }

    /// <summary>
    /// Represents ChartHiLoType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartHiLoType : ChartType
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

        /// <summary>
        /// Gets the require data count.
        /// </summary>
        /// <value>The require data count.</value>
        public override int RequiresDataCount
        {
            get
            {
                return 2;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            double center = series.Area.GetSideBySideInfo(series).Median;

            for (int i = 0; i < points.Length; i++)
            {
                ChartPoint hiPoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[1]);
                ChartPoint loPoint = new ChartPoint(points[i].DataPoint.X + center, points[i].DataPoint.Values[0]);
                if (points[i].DataPoint.EmptyPoint)
                {
                    if (series.ShowEmptyPoints)
                    {
                        if (series.EmptyPointStyle == EmptyPointStyle.Symbol)
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            series.Segments.Add(new ChartHiLoSegment(hiPoint, loPoint, points[i], series));
                            series.Segments[series.Segments.Count - 1].Interior = series.EmptyPointInterior;
                        }
                        else
                        {
                            series.Segments.Add(new ChartEmptySymbolSegment(points[i].DataPoint, points[i], series, series.EmptyPointSymbolTemplate));
                        }
                        
                    }
                    else
                    {
 
                    }
                }
                else
                {
                    series.Segments.Add(new ChartHiLoSegment(hiPoint, loPoint, points[i], series));
                }
            }

            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                int index = -1;
                for (int i = 0; i < points.Length; i++)
                {
                    double y1 = points[i].DataPoint.Values[0];
                    double y2 = points[i].DataPoint.Values[1];
                     //Add for SD11357
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                            }
                            else if ((series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && (y1 > y2)) ||
                                     (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (y1 < y2)))
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], i));
                            }
                            else
                            {
                                series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], i));
                            }
                        }
                    }
                    else
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], ++index));
                            series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], ++index));
                        }
                        else if ((series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top && (y1 > y2)) ||
                                 (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom && (y1 < y2)))
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y1, y1, y1, y2, points[i], i));
                        }
                        else
                        {
                            series.Adornments.Add(this.CreateAdornment(series, y2, y2, y1, y2, points[i], i));
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
        /// <seealso cref="ChartHiLoType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
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
        /// <seealso cref="ChartHiLoType"/>
        public override string ToString()
        {
            return "HiLo";
        }
    }
}
