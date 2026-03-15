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
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class Definition for ChartAccumulationDistribution
    /// </summary>
    public class ChartAccumulationDistribution: DependencyObject
    {
        /// <summary>
        /// Gets the value of the SignalLineInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObjectobj.</param>
        /// <returns>The SignalLineInterior brush</returns>
        public static Brush GetSignalLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SignalLineColorProperty);
        }

        /// <summary>
        /// Sets the value of the SignalLineInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetSignalLineColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(SignalLineColorProperty, value);
        }

        /// <summary>
        /// Indicates the SignalLineInterior Dependency Property
        /// </summary>
        public static readonly DependencyProperty SignalLineColorProperty =
                DependencyProperty.RegisterAttached("SignalLineColor", typeof(Brush), typeof(ChartAccumulationDistribution), new ChartPropertyMetadata(Brushes.Navy, new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));


        private IChartData points = null;
        private ChartTechnicalIndicator indicator = null;
        internal PointCollection AccumulationPoints = null;
        /// <summary>
        /// Contructor of ChartAccumulationDistribution class
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indic"></param>
        public ChartAccumulationDistribution(IChartData data, ChartTechnicalIndicator indic)
        {
            points = data;
            indicator = indic;
            this.AddPoints();

            if (this.AccumulationPoints != null)
            {
                if (indicator.accumulationArea == null)
                {
                    ChartArea area = new ChartArea();
                    if (indicator.Series.Area.SyncChartArea != null)
                    {
                        SyncChartAreas sarea = indicator.Series.Area.SyncChartArea as SyncChartAreas;
                        sarea.Areas.Add(area);

                        sarea.Areas[sarea.Areas.Count - 1].Width = sarea.ActualWidth;
                        sarea.Areas[sarea.Areas.Count - 1].ElementMargin = indicator.Series.Area.ElementMargin;
                        sarea.Areas[sarea.Areas.Count - 1].Margin = indicator.Series.Area.Margin;
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.AccumulationPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartAccumulationDistribution.GetSignalLineColor(indic), ToolTip = "Accumulation Distribution Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].isIndicator = true;
                        foreach (ChartSeries ser in sarea.Areas[sarea.Areas.Count - 1].Series)
                        {
                            for (int i = 0; i < ser.Data.Count; i++)
                            {
                                ser.Data[i].X = this.AccumulationPoints[i].X;
                            }
                        }
                        sarea.Areas[0].SplitterPosition = double.IsNaN(area.SplitterPosition) || double.IsPositiveInfinity(area.SplitterPosition) || area.SplitterPosition == 0 ? 0.5 : area.SplitterPosition;
                        indicator.accumulationArea = area;
                        sarea.Areas[sarea.Areas.Count - 1].PrimaryAxis.ZoomFactor = indic.Series.Area.PrimaryAxis.ZoomFactor;
                        if (sarea.PrimaryAxis.ZoomFactor != 1)
                        {
                            ChartAreaCommands.ZoomIn.Execute(null, sarea);
                        }
                    }
                }
            }
        }
        private void AddPoints()
        {
            PointCollection sd = new PointCollection();
            double sum = 0d;
            double close = double.NaN;
            sd.Add(new System.Windows.Point() { X = points[0].X, Y = 0d });

            for (int i = 1; i < points.Count; ++i)
            {
                close = points[i].Values[3];
                if (close != 0d)
                {
                    sum += (points[i].Values[4]) * (2 * close - (points[i].Values[0]) - (points[i].Values[1])) / close;
                }
                sd.Add(new Point() { X = points[i].X, Y = sum});           
            }
            AccumulationPoints = sd;
        }

        private static void OnAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartTechnicalIndicator indicator = d as ChartTechnicalIndicator;
            if (indicator != null && indicator.Series != null && indicator.Series.Presenter != null && indicator.Series.Presenter.m_IndicatorPresenter != null)
            {               
                indicator.Series.Presenter.m_IndicatorPresenter.InvalidateVisual();
            }
        }

    }
}
