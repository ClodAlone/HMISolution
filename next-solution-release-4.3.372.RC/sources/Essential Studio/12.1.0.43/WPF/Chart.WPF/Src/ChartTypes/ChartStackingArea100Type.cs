// <copyright file="ChartStackingArea100Type.cs" company="Syncfusion">
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


    /// <summary>
    /// Represents stacking 100% area type.
    /// </summary>
    /// <remarks>
    /// In the 100 % Stacked Area Chart, the cumulative proportion of each stacked
    /// element always totals 100%. This type of chart is great to visualize the
    /// relative contribution of each series values to the whole.
    /// </remarks>
    /// <seealso cref="ChartStackingArea100Type"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartStackingArea100Type : ChartStackingAreaType
    {

        #region Dependency properties
        /// <summary>
        /// Identifies ShowValueAsProbability attached dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowValueAsProbabilityProperty =
            DependencyProperty.RegisterAttached("ShowValueAsProbability", typeof(bool), typeof(ChartStackingArea100Type), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowValueAsProbabilityChanged)));


        /// <summary>
        /// Identifies the RequiresNegativeSeriesStack dependency property.
        /// </summary>
        public new static readonly DependencyProperty RequiresNegativeSeriesStackProperty =
              DependencyProperty.RegisterAttached("RequiresNegativeSeriesStack", typeof(bool), typeof(ChartStackingArea100Type), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRequiresNegativeSeriesStackChanged)));
        #endregion

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

        double sum = 0;

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
        /// <seealso cref="ChartStackingAreaType"/>
        public new static void SetRequiresNegativeSeriesStack(ChartArea area, bool value)
        {
            area.SetValue(RequiresNegativeSeriesStackProperty, value);
        }

        /// <summary>
        /// Returns whether the positive and negative series should be separately stacked.
        /// </summary>
        /// <param name="area">The <see cref="ChartArea"/> where the stacking should happen.</param>
        /// <returns>True indicates the positive and negative series are stacked separately.</returns>
        public new static bool GetRequiresNegativeSeriesStack(ChartArea area)
        {
            return (bool)area.GetValue(RequiresNegativeSeriesStackProperty);
        }

        /// <summary>
        /// Called when [show value as probability changed].
        /// </summary>
        /// <param name="d">The DependencyObject d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowValueAsProbabilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            UpdateSeriesInArea(area);
        }

        /// <summary>
        /// Gets the show value as probability.
        /// </summary>
        /// <param name="area">The area value.</param>
        /// <returns>True is the value is to be displayed as probability</returns>
        public static bool GetShowValueAsProbability(ChartArea area)
        {
            return (bool)area.GetValue(ShowValueAsProbabilityProperty);
        }

        /// <summary>
        /// Sets the show value as probability.
        /// </summary>
        /// <param name="area">The <see cref="ChartArea"/>.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetShowValueAsProbability(ChartArea area, bool value)
        {
            area.SetValue(ShowValueAsProbabilityProperty, value);
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
                if (ChartStackingArea100Type.GetRequiresNegativeSeriesStack(series.Area))
                {
                    forPositive = points[i].DataPoint.Y >= 0;
                }


                areaPoints[i] = new ChartPoint(points[i].DataPoint.X, series.Area.GetStackInfo(series, i, forPositive));
            }

            

            for (int i = 0; i < points.Length; i++)
            {
                DoubleRange yRange = series.Area.GetPercentageStackInfo(series, points[i]);
                
                foreach (ChartSeries chartSeries in series.Area.Series)
                {
                    if (chartSeries.PointsCount > points[i].Index && chartSeries.ActualYAxis==series.ActualYAxis)
                    {
                        sum += Math.Abs(chartSeries.Data[i].Y);
                    }
                }
                if(areaPoints[i]!=null)
                    if (ChartStackingArea100Type.GetShowValueAsProbability(series.Area) && ChartStackingArea100Type.GetRequiresNegativeSeriesStack(series.Area))
                    {
                            areaPoints[2 * points.Length - i - 1] = new ChartPoint(areaPoints[i].X, areaPoints[i].Y + ((points[i].DataPoint.Y / Math.Abs(sum)) * 100) / 100);
                            sum = 0;
                    }
                    else if (ChartStackingArea100Type.GetShowValueAsProbability(series.Area))
                    {
                        areaPoints[2 * points.Length - i - 1] = new ChartPoint(areaPoints[i].X, areaPoints[i].Y + ((Math.Abs(points[i].DataPoint.Y) / Math.Abs(sum)) * 100) / 100);
                        sum = 0;
                    }
                    else if (ChartStackingArea100Type.GetRequiresNegativeSeriesStack(series.Area))
                    {
                        areaPoints[2 * points.Length - i - 1] = new ChartPoint(areaPoints[i].X, areaPoints[i].Y + ((points[i].DataPoint.Y / Math.Abs(sum)) * 100));
                        sum = 0;
                    }
                    else
                    {
                        areaPoints[2 * points.Length - i - 1] = new ChartPoint(areaPoints[i].X, areaPoints[i].Y + ((Math.Abs(points[i].DataPoint.Y) / sum) * 100));
                        sum = 0;
                    }
            }

            series.Segments.Add(new ChartAreaSegment(areaPoints, points, series));

            if (series.AdornmentsInfo.Visible)
            {
                series.Adornments.Clear();
                for (int i = 0; i < points.Length; i++)
                {
                    foreach (ChartSeries chartSeries in series.Area.Series)
                    {
                        if (chartSeries.PointsCount > points[i].Index && chartSeries.ActualYAxis == series.ActualYAxis)
                        {
                            sum += Math.Abs(chartSeries.Data[i].Y);
                        }
                    }
                    if (points[i].DataPoint.EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                            series.Adornments.Add(this.CreateAdornment(series, points[i], i));
                    }
                    else
                    {
                        if (ChartStackingArea100Type.GetShowValueAsProbability(series.Area) && ChartStackingArea100Type.GetRequiresNegativeSeriesStack(series.Area))
                        {
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(areaPoints[i].X, areaPoints[i].Y + (((points[i].DataPoint.Y / sum) * 100) / 100)), points[i], series));
                            sum = 0;
                        }
                        else if (ChartStackingArea100Type.GetShowValueAsProbability(series.Area))
                        {
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(areaPoints[i].X, areaPoints[i].Y + (((Math.Abs(points[i].DataPoint.Y) / sum) * 100) / 100)), points[i], series));
                            sum = 0;
                        }
                        else if (ChartStackingArea100Type.GetRequiresNegativeSeriesStack(series.Area))
                        {
                            double y=(areaPoints[i].Y + ((points[i].DataPoint.Y / Math.Abs(sum)) * 100))>100?100:(areaPoints[i].Y + ((points[i].DataPoint.Y / Math.Abs(sum)) * 100));
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(areaPoints[i].X,y), points[i], series));
                            sum = 0;
                        }
                        else
                        {
                            series.Adornments.Add(new ChartAdornment(new ChartPoint(areaPoints[i].X, areaPoints[i].Y + (((Math.Abs(points[i].DataPoint.Y)) / sum) * 100)), points[i], series));
                            sum = 0;
                        }
                    }
                }
            }
            
        }


        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The Chart Series</param>
        /// <param name="points">The series points</param>
        /// <seealso cref="ChartStackingAreaType"/>
        protected override void UpdateSegments(ChartSeries series, ChartIndexedDataPoint[] points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }

        #endregion

        /// <summary>
        /// Converts ChartAreaType to string
        /// </summary>
        /// <returns>The string</returns>
        /// <seealso cref="ChartStackingAreaType"/>
        public override string ToString()
        {
            return "StackingArea100";
        }
    }
}
