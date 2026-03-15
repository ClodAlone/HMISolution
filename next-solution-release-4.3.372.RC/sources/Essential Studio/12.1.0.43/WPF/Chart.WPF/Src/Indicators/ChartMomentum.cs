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
using System.Windows.Media;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for Chartmomentum
    /// </summary>
    public class ChartMomentum: DependencyObject
    {
        /// <summary>
        ///  Identifies the MomentumTimeSpan dependency property.
        /// </summary>
        public static readonly DependencyProperty MomentumTimeSpanProperty =
            DependencyProperty.RegisterAttached("MomentumTimeSpan", typeof(int), typeof(ChartMomentum), new ChartPropertyMetadata(14));

        /// <summary>
        /// Return the int Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetMomentumTimeSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(MomentumTimeSpanProperty);
        }

        /// <summary>
        /// Set MomentumTimeSpan to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMomentumTimeSpan(DependencyObject obj, int value)
        {
            obj.SetValue(MomentumTimeSpanProperty, value);
        }
        internal PointCollection Momentumpoints = null;        
        internal PointCollection lowerPoints = null;
        private IChartData Points = null;
        private ChartTechnicalIndicator indicator = null;
        /// <summary>
        /// Called when instance created for ChartMomentum
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indic"></param>
        public ChartMomentum(IChartData data, ChartTechnicalIndicator indic)
        {
            Points = data;
            indicator = indic;
            Momentumpoints = ComputeMomentum(ChartMomentum.GetMomentumTimeSpan(indicator));
            lowerPoints = new PointCollection();           
            for (int i = 0; i < Points.Count; i++)
            {            
                lowerPoints.Add(new Point() { X = Points[i].X, Y = 100 });
            }
            if (this.Momentumpoints != null && this.lowerPoints != null )
            {
                if (indicator.momentumArea == null)
                {
                    ChartArea area = new ChartArea();
                    if (indicator.Series.Area.SyncChartArea != null)
                    {
                        SyncChartAreas sarea = indicator.Series.Area.SyncChartArea as SyncChartAreas;
                        sarea.Areas.Add(area);

                        //sarea.Areas[sarea.Areas.Count - 1].Width = sarea.ActualWidth;
                        sarea.Areas[sarea.Areas.Count - 1].ElementMargin = indicator.Series.Area.ElementMargin;
                        sarea.Areas[sarea.Areas.Count - 1].Margin = indicator.Series.Area.Margin;
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.Momentumpoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.DarkOrange, ToolTip = "Momentum Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.lowerPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.DarkViolet, ToolTip = "Momentum Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].isIndicator = true;
                        foreach (ChartSeries ser in sarea.Areas[sarea.Areas.Count - 1].Series)
                        {
                            if (sarea.Areas[sarea.Areas.Count - 1].Series.IndexOf(ser) == 0)
                            {
                                for (int i = 0; i < ser.Data.Count - ChartMomentum.GetMomentumTimeSpan(indicator); i++)
                                {
                                    ser.Data[i].X = this.Momentumpoints[i].X;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < ser.Data.Count; i++)
                                {
                                    ser.Data[i].X = this.lowerPoints[i].X;
                                }
                            }
                        }
                        sarea.Areas[0].SplitterPosition = double.IsNaN(area.SplitterPosition) || double.IsPositiveInfinity(area.SplitterPosition) || area.SplitterPosition == 0 ? 0.5 : area.SplitterPosition;
                        area.SecondaryAxis.RangePadding = ChartRangePaddingType.Additional;
                        indicator.momentumArea = area;
                        sarea.Areas[sarea.Areas.Count - 1].PrimaryAxis.ZoomFactor = indic.Series.Area.PrimaryAxis.ZoomFactor;
                        if (sarea.PrimaryAxis.ZoomFactor != 1)
                        {
                            ChartAreaCommands.ZoomIn.Execute(null, sarea);
                        }
                    }
                }
            }
        }

        private PointCollection ComputeMomentum(int len)
        {         
            PointCollection sd1 = new PointCollection();
            double pad = 0;
            for (int i = 0; i < Points.Count; ++i)
            {
                if (i < len - 1)
                {
                    //sd1.Add(new Point() {X = Points[i].X, Y = pad });
                }
                else
                {
                    sd1.Add(new Point() { Y = (pad = Points[i].Values[3] * 100d / Points[i - len + 1].Values[3]), X = Points[i].X });
                }
            }

            //double pad = sd1[len - 1].Y;
            for (int i = 0; i < len - 1; ++i)
            {
                //sd1[i] = new Point(sd1[i].X, pad);
            }
            return sd1;
        }
    }
}
