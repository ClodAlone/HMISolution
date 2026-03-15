#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartFastStackingColumnSegment
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class ChartFastStackingColumnSegment : ChartSegment
    {
        #region Members
        /// <summary>
        /// Initializes m_bottomLeftPoint
        /// </summary>
        private IChartDataPoint m_bottomLeftPoint;

        /// <summary>
        /// Initializes m_topRightPoint
        /// </summary>
        private IChartDataPoint m_topRightPoint;

        /// <summary>
        /// Initializes m_points
        /// </summary>
        private List<IChartDataPoint> m_points;

/*
  private double viewPortwidth;
*/


        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (this.chartPoints != null)
            {
                this.chartPoints.Clear();
                this.chartPoints = null;
            }
            if (this.Points != null)
            {
                this.Points.Clear();
                this.Points = null;
            }
            this.m_bottomLeftPoint = null;
            if (this.m_points != null)
            {
                this.m_points.Clear();
                this.m_points = null;
            }

            this.m_topRightPoint = null;
            this.seriesCorrespondingPoints = null;
            this.Item = null;
            this.m_bottomLeftPoint = null;

        }


        internal static readonly DependencyProperty AffectRenderProperty =
DependencyProperty.RegisterAttached("AffectRender", typeof(bool), typeof(ChartFastStackingColumnSegment), new FrameworkPropertyMetadata(false,FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or Sets the AffectRenderProperty
        /// </summary>
        public bool AffectRender
        {
            get { return (bool)GetValue(AffectRenderProperty); }
            set { SetValue(AffectRenderProperty, value); }
        }

        internal bool affectRender = false;

             /// <summary>
        /// Get or Set PointsProperty
             /// </summary>
             public StackingColumnChartValuesCollection Points
        {
            get { return (StackingColumnChartValuesCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        /// <summary>
             /// Identifies the points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(StackingColumnChartValuesCollection),
            typeof(ChartFastStackingColumnSegment), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        List<IChartDataPoint> chartPoints;
/*
        double origin0;
*/
        DoubleRange sbsInfo;

        static ChartFastStackingColumnSegment()
        {
            Type type = typeof(ChartFastStackingColumnSegment);
            DefaultTemplatePropertyKey.OverrideMetadata(type, new PropertyMetadata(ChartDataUtils.ResolveSegmentTemplate(type)));
        }

        internal ChartFastStackingColumnSegment(ChartIndexedDataPoint[] correspondingPoints, ChartSeries series)
            : base(series, correspondingPoints)
        {
            Points = new StackingColumnChartValuesCollection();
            // series.ActualXAxis.RangeChanged += new ChartAxisRangeEventHandler(ActualXAxis_RangeChanged);
            chartPoints = new List<IChartDataPoint>();
            sbsInfo = series.Area.GetSideBySideInfo(series);

            int seriesIndex = series.Area.Series.IndexOf(series);
            bool isLower = seriesIndex == 0;
            bool isUpper = seriesIndex == series.Area.Series.Count - 1;            
            yRange = new DoubleRange(series.ActualXAxis.Origin, series.ActualXAxis.Origin);
            for (int i = 0; i < correspondingPoints.Length; i++)
            {
                if (!correspondingPoints[i].DataPoint.EmptyPoint)
                {
                    double x1 = 0, x2 = 0;
                    if (series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Relative && !series.IsIndexed)
                    {
                        var relativePos = correspondingPoints[i].DataPoint.Values.Length > 1 ? correspondingPoints[i].DataPoint.Values[1] * 0.5 : 0;
                        x1 = correspondingPoints[i].DataPoint.X - relativePos;
                        x2 = correspondingPoints[i].DataPoint.X + relativePos;
                    }
                    else
                    {
                        x1 = correspondingPoints[i].DataPoint.X + sbsInfo.Start;
                        x2 = correspondingPoints[i].DataPoint.X + sbsInfo.End;
                    }

                    bool? forPositive = null;
                    if (ChartFastStackingColumnType.GetRequiresNegativeSeriesStack(series.Area))
                    {
                        forPositive = correspondingPoints[i].DataPoint.Y >= 0;
                    }
                    double y2 = series.Area.GetStackInfo(series, correspondingPoints[i].DataPoint.X, forPositive);
                    double y1 = 0d;

                    if (forPositive == null)
                        y1 = Math.Abs(correspondingPoints[i].DataPoint.Y) + y2;
                    else
                        y1 = correspondingPoints[i].DataPoint.Y + y2;
                    m_bottomLeftPoint = new ChartPoint(x1, y1);
                    m_topRightPoint = new ChartPoint(x2, y2);

                    if (series.ActualXAxis.RangePadding== ChartRangePaddingType.None && series.ActualXAxis.RangeCalculationMode == RangeCalculationMode.ConsistentAcrossChartTypes)
                    {
                        xRange += ((x1 + x2) / 2);
                    }
                    else
                    {
                        xRange +=x1-0.5;
                        xRange += x2+0.5;                      
                    }
                    yRange += y1;

                    chartPoints.Add(correspondingPoints[i].DataPoint);
                }
            }
            SetRange(series);

            correspondingPoints = null;
        }

        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
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
            if (Points != null)
            {
                Points.Clear();
                AffectRender = affectRender;
                affectRender = true;
                AffectRender = true;
            }


            for (int i = 0; i < chartPoints.Count; i++)
            {
                // sbsInfo = Series.Area.GetSideBySideInfo(Series);
                double x1 = 0, x2 = 0;
                if (this.Series.SegmentWidthMode == Syncfusion.Windows.Chart.ChartSeries.Mode.Relative && !Series.IsIndexed )
                {
                    var relativePos = chartPoints[i].Values.Length > 1 ? chartPoints[i].Values[1] * 0.5 : 0;
                    x1 = chartPoints[i].X - relativePos;
                    x2 = chartPoints[i].X + relativePos;
                }
                else
                {
                    x1 = chartPoints[i].X + sbsInfo.Start;
                    x2 = chartPoints[i].X + sbsInfo.End;
                }
                bool? forPositive = null;
                if (ChartFastStackingColumnType.GetRequiresNegativeSeriesStack(base.Series.Area))// series.Area))
                {
                    forPositive = chartPoints[i].Y >= 0;
                }
                double y2 = base.Series.Area.GetStackInfo(base.Series, chartPoints[i].X, forPositive); //series.Area.GetStackInfo(series, chartPoints[i].X, forPositive);
                double y1 = 0d;
                if (forPositive == null)
                    y1 = Math.Abs(chartPoints[i].Y) + y2;
                else
                    y1 = chartPoints[i].Y + y2;
                //double y1 = chartPoints[i].Y;
                //double y2 = origin0;

                m_bottomLeftPoint = new ChartPoint(x1, y1);
                m_topRightPoint = new ChartPoint(x2, y2);

                Point blpoint = transformer.TransformToVisible(m_bottomLeftPoint.X, m_bottomLeftPoint.Y);
                Point trpoint = transformer.TransformToVisible(m_topRightPoint.X, m_topRightPoint.Y);
                Rect columnRect = new Rect(blpoint, trpoint);
                this.Points.Add(new StackingColumnChartValues() { X = columnRect.X, Y = columnRect.Y, Height = columnRect.Height, Width = columnRect.Width });
                if (i == 0)
                    ChartType.SetColumnInitialSegmentWidthValue(this.Series, x1);
                if (i == chartPoints.Count - 1)
                    ChartType.SetEndSegmentWidth(this.Series, x2);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="series"></param>
        /// <param name="axis"></param>
        protected override void SetPointsForAllSeries(ChartSeries series, ChartAxis axis)
        {
            foreach (ChartSeries item in series.Area.Series)
            {
                if ((item.ActualXAxis == axis || item.ActualYAxis == axis) && item.Segments.Count != 0)
                {
                    if (item.Segments[0].GetType() == typeof(ChartFastLineSegment))//typeof(ChartFastStackingColumnSegment))
                    {
                        //((ChartFastStackingColumnSegment)item.Segments[0]).SetPointToNull();
                        ((ChartFastLineSegment)item.Segments[0]).SetPointToNull();
                    }
                }
            }
        }

        internal void SetPointToNull()
        {
            this.Points = null;
        }

        void ActualXAxis_RangeChanged(object sender, ChartAxisRangeArgs e)
        {
            if (Series != null && Series.Area != null)
            {
                this.SetPointsForAllSeries(Series, Series.Area.PrimaryAxis);
                Series.ChartType.indexedPointsList.Clear();
                Series.ChartType.indexedPointsList = null;
            }
        }
    }

    /// <summary>
    /// Class implementation for ChartFastStackingColumnType
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartFastStackingColumnType : ChartType
    {
        /// <summary>
        /// Identifies the RequiresNegativeSeriesStack dependency property.
        /// </summary>
        public static readonly DependencyProperty RequiresNegativeSeriesStackProperty =
              DependencyProperty.RegisterAttached("RequiresNegativeSeriesStack", typeof(bool), typeof(ChartFastStackingColumnType), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRequiresNegativeSeriesStackChanged)));

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        protected override ChartType.ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.SideBySide | ChartTypeFlags.Stacked | ChartTypeFlags.Indexed;//ChartTypeFlags.SideBySide | 
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "FastStackingColumn";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
            series.Segments.Clear();
            series.Segments.Add(new ChartFastStackingColumnSegment(points, series));
            if (series.AdornmentsInfo.Visible == true)
            {
                series.Adornments.Clear();
                for (int i = 0; i < points.Length; i++)
                {
                    bool? forPositive = null;
                    if (ChartFastStackingColumnType.GetRequiresNegativeSeriesStack(series.Area))
                    {
                        forPositive = points[i].DataPoint.Y >= 0;
                    }

                    double y2 = series.Area.GetStackInfo(series, points[i].DataPoint.X, forPositive);
                    double y1 = 0d;

                    if (forPositive == null)
                        y1 = Math.Abs(points[i].DataPoint.Y) + y2;
                    else
                        y1 = points[i].DataPoint.Y + y2;

                    DoubleRange yRange = new DoubleRange(y1, y2);
                    double bottomPoint = (Math.Abs(yRange.Start) < Math.Abs(yRange.End)) ? yRange.Start : yRange.End;
                    double topPoint = (Math.Abs(yRange.Start) > Math.Abs(yRange.End)) ? yRange.Start : yRange.End;
                    double midPoint = yRange.Median;
                    if (yRange.Start < 0 || yRange.End < 0)
                        series.AdornmentsInfo.m_requiresSymmetricLabelling = true;

                    if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                        series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, bottomPoint), points[i], series, yRange));
                    else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                        series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, topPoint), points[i], series, yRange));
                    else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                        series.Adornments.Add(new ChartAdornment(new ChartPoint(points[i].DataPoint.X + sbsInfo.Median, midPoint), points[i], series, yRange));
                }
            }
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        /// <seealso cref="ChartType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            CalculateSegments(series, points);
        }

        private static void OnRequiresNegativeSeriesStackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            ChartType.UpdateSeriesInArea(area);
        }

        /// <summary>
        /// Set RequiresNegativeSeriesStack to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="area"></param>
        /// <param name="value"></param>
        public static void SetRequiresNegativeSeriesStack(ChartArea area, bool value)
        {
            area.SetValue(RequiresNegativeSeriesStackProperty, value);
        }

        /// <summary>
        /// Return the Bool Value from the given DependencyObject
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public static bool GetRequiresNegativeSeriesStack(ChartArea area)
        {
            return (bool)area.GetValue(RequiresNegativeSeriesStackProperty);
        }


    }

}
