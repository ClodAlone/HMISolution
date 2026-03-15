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
    /// Class implementation for ChartRelativeStrengthIndex
    /// </summary>
    public class ChartRelativeStrengthIndex
    {
        internal PointCollection RSIpoints = null;
        internal PointCollection upperPoints = null;
        internal PointCollection lowerPoints = null;
        private IChartData Points = null;
        private ChartTechnicalIndicator indicator = null;
        /// <summary>
        /// Called when instance created for ChartRelativeStrengthIndex
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indic"></param>
        public ChartRelativeStrengthIndex(IChartData data, ChartTechnicalIndicator indic)
        {
            Points = data;
            indicator = indic;
            RSIpoints = ComputeRSI(14);
            upperPoints = new PointCollection();
            lowerPoints = new PointCollection();
            for (int i = 0; i <Points.Count; i++)
            {
                upperPoints.Add(new Point() { X = Points[i].X, Y = 70 });
                lowerPoints.Add(new Point() { X = Points[i].X, Y = 30 });
            }
            if (this.upperPoints != null && this.lowerPoints != null && this.RSIpoints!= null)
            {
                if (indicator.rsiArea == null)
                {
                    ChartArea area = new ChartArea();
                    if (indicator.Series.Area.SyncChartArea != null)
                    {
                        SyncChartAreas sarea = indicator.Series.Area.SyncChartArea as SyncChartAreas;
                        sarea.Areas.Add(area);

                        //sarea.Areas[sarea.Areas.Count - 1].Width = sarea.ActualWidth;
                        sarea.Areas[sarea.Areas.Count - 1].ElementMargin = indicator.Series.Area.ElementMargin;
                        sarea.Areas[sarea.Areas.Count - 1].Margin = indicator.Series.Area.Margin;
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.upperPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.Orange, ToolTip = "RSI Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.lowerPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.DarkGreen, ToolTip = "RSI Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.RSIpoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.Aqua, ToolTip = "RSI Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].isIndicator = true;
                        foreach (ChartSeries ser in sarea.Areas[sarea.Areas.Count - 1].Series)
                        {
                            if (sarea.Areas[sarea.Areas.Count - 1].Series.IndexOf(ser) == 2)
                            {
                                for (int i = 0; i < ser.Data.Count - 14; i++)
                                {
                                    ser.Data[i].X = this.RSIpoints[i].X;
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
                        indicator.rsiArea = area;
                        sarea.Areas[sarea.Areas.Count - 1].PrimaryAxis.ZoomFactor = indicator.Series.Area.PrimaryAxis.ZoomFactor;
                        if (sarea.PrimaryAxis.ZoomFactor != 1)
                        {
                            ChartAreaCommands.ZoomIn.Execute(null, sarea);
                        }
                    }
                }
            }
        }

        private PointCollection ComputeRSI(int len)
        {            
            double c = 0, c1 = 0;
            double pmf = 0d;
            double nmf = 0d;
            c1 = Points[0].Values[3];
            PointCollection sd1 = new PointCollection();
            //sd1.Add(new Point() { X = Points[0].X });
            for (int i = 1; i < len; ++i)
            {
                c = Points[i].Values[3];
                //sd1.Add(new Point() { X = Points[i].X });
                if (c > c1)
                    pmf += c - c1;
                else if (c < c1)
                    nmf += c1 - c;
                c1 = c;
            }
            c = Points[len].Values[3];
            if (c > c1)
                pmf += c - c1;
            else if (c < c1)
                nmf += c1 - c;
            c1 = c;
            //pmf = pmf / len;
            //nmf = nmf / len;
            sd1.Add(new Point()
            {
                X = Points[len].X,
                Y = 100 - 100 / (1 + pmf / nmf)
            });
            for (int i = len + 1; i < Points.Count; ++i)
            {                
             

                    c = Points[i].Values[3];
                    if (c > c1)
                    {
                        pmf = (pmf * (len - 1) + (c - c1)) / len;
                        nmf = (nmf * (len - 1)) / len;
                    }
                    else if (c < c1)
                    {
                        nmf = (nmf * (len - 1) + (c1 - c)) / len;
                        pmf = (pmf * (len - 1)) / len;
                    }
                    c1 = c;

                    sd1.Add(new Point()
                    {
                        X = Points[i].X,
                        Y = 100 - 100 / (1 + pmf / nmf)
                    });
             
            }

            double pad = sd1[len].Y;
            for (int i = 0; i < len; ++i)
            {
                //sd1[i] = new Point() {X = sd1[i].X, Y = pad };
            }
            return sd1;
        }
    }
}
