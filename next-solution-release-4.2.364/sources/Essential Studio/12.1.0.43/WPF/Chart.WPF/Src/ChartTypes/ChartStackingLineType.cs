// <copyright file="ChartStackingLineType.cs" company="Syncfusion">
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
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Data;

    /// <summary>
    /// Represents ChartStackingLineType class
    /// </summary>
    /// <remarks>
    /// Stacking Line Charts are similar to regular line charts except that the Y values
    /// stack on top of each other in the specified series order. This helps visualize
    /// the relationship of parts to the whole.
    /// </remarks>
    /// <seealso cref="ChartStackingLineType"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartStackingLineType : ChartLineType
    {
        #region Properties

        /// <summary>
        /// Identifies the RequiresNegativeSeriesStack dependency property.
        /// </summary>
        public static readonly DependencyProperty RequiresNegativeSeriesStackProperty =
              DependencyProperty.RegisterAttached("RequiresNegativeSeriesStack", typeof(bool), typeof(ChartStackingLineType), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRequiresNegativeSeriesStackChanged)));

        /// <summary>
        /// Gets chart type flags. This is a dependency property.
        /// </summary>
        protected override ChartTypeFlags Flags
        {
            get
            {
                return ChartTypeFlags.Stacked | ChartTypeFlags.Indexed;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Called when [requires negative series stack changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRequiresNegativeSeriesStackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            ChartType.UpdateSeriesInArea(area);
        }

        /// <summary>
        /// Specifies that the positive and negative series should be separately stacked.
        /// </summary>
        /// <param name="area">The <see cref="ChartArea"/> where the stacking should happen.</param>
        /// <param name="value">True to stack separately, false otherwise.</param>
        /// <seealso cref="ChartStackingLineType"/>
        public static void SetRequiresNegativeSeriesStack(ChartArea area, bool value)
        {
            area.SetValue(RequiresNegativeSeriesStackProperty, value);
        }

        /// <summary>
        /// Returns whether the positive and negative series should be separately stacked.
        /// </summary>
        /// <param name="area">The <see cref="ChartArea"/> where the stacking should happen.</param>
        /// <returns>True indicates the positive and negative series are stacked separately.</returns>
        public static bool GetRequiresNegativeSeriesStack(ChartArea area)
        {
            return (bool)area.GetValue(RequiresNegativeSeriesStackProperty);
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            IChartDataPoint[] areaPoints = new IChartDataPoint[2 * points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                bool? forPositive = null;
                if (ChartStackingLineType.GetRequiresNegativeSeriesStack(series.Area))
                {
                    forPositive = points[i].DataPoint.Y >= 0;
                }

                areaPoints[i] = new ChartPoint(points[i].DataPoint.X, series.Area.GetStackInfo(series, i, forPositive));
            }
            int firstStackedIndex = 0;

            for (int i = 0; i < series.Area.Series.Count; i++)
            {
                if (series.Area.Series[i].Type == ChartTypes.StackingLine)
                {
                    firstStackedIndex = i;
                    break;
                }

            }
            bool isFirstStackedLineSeries = false;
            if (series.Area != null)
                if (series.Area.Series.Count >= firstStackedIndex)
                {
                    if (series == series.Area.Series[firstStackedIndex])
                    {
                        isFirstStackedLineSeries = true;

                    }
                }
            for (int i = 0; i < points.Length; i++)
            {
                double y = points[i].DataPoint.Y;
                if (!isFirstStackedLineSeries)
                    areaPoints[i].Y = areaPoints[i].Y - series.Area.SecondaryAxis.Origin;
                if(ChartStackingLineType.GetRequiresNegativeSeriesStack(series.Area))
                    areaPoints[2 * points.Length - i - 1] = new ChartPoint(areaPoints[i].X, isFirstStackedLineSeries ? y : areaPoints[i].Y + y);
                else
                    areaPoints[2 * points.Length - i - 1] = new ChartPoint(areaPoints[i].X, isFirstStackedLineSeries ? y : areaPoints[i].Y + Math.Abs(y));
            }

            for (int i = areaPoints.Length-1; i  > points.Length; i--)
            {
                series.Segments.Add(new ChartLineSegment(areaPoints[i],areaPoints[i-1], points[i-points.Length],points[i-1-points.Length], series));
            }

            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                    }
                    else
                    {
                        series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                    }
                }
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The Chart Series</param>
        /// <param name="points">The series points</param>
        /// <seealso cref="ChartStackingLineType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }
      
        #endregion

        /// <summary>
        /// Converts ChartStackingLineType to string
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartStackingLineType"/>
        public override string ToString()
        {
            return "StackingLine";
        }
    }
}
