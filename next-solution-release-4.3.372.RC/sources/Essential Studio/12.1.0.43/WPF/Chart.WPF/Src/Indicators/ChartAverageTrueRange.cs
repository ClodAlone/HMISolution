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
    /// ChartAverageTrueRange class Implementation
    /// </summary>
    public class ChartAverageTrueRange: DependencyObject
    {
        internal PointCollection ATRpoints = null;        
        
        private IChartData Points = null;
        private ChartTechnicalIndicator indicator = null;
        /// <summary>
        /// Called when instance created for ChartAverageTrueRange
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indic"></param>
        public ChartAverageTrueRange(IChartData data, ChartTechnicalIndicator indic)
        {
            Points = data;
            indicator = indic;
            ATRpoints = ComputeAverageTrueRange(14);
            if (this.ATRpoints != null)
            {
                if (indicator.avtArea == null)
                {
                    ChartArea area = new ChartArea();
                    if (indicator.Series.Area.SyncChartArea != null)
                    {
                        SyncChartAreas sarea = indicator.Series.Area.SyncChartArea as SyncChartAreas;
                        sarea.Areas.Add(area);

                        sarea.Areas[sarea.Areas.Count - 1].Width = indicator.Series.Area.ActualWidth;
                        sarea.Areas[sarea.Areas.Count - 1].ElementMargin = indicator.Series.Area.ElementMargin;
                        sarea.Areas[sarea.Areas.Count - 1].Margin = indicator.Series.Area.Margin;
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.ATRpoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.Aqua, ToolTip = "ATR Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].isIndicator = true;
                        foreach (ChartSeries ser in sarea.Areas[sarea.Areas.Count - 1].Series)
                        {
                            for (int i = 0; i < ser.Data.Count; i++)
                            {
                                //ser.Data[i].X = this.ATRpoints[i].X;
                            }
                        }
                        sarea.Areas[0].SplitterPosition = double.IsNaN(area.SplitterPosition) || double.IsPositiveInfinity(area.SplitterPosition) || area.SplitterPosition == 0 ? 0.5 : area.SplitterPosition;
                        area.SecondaryAxis.RangePadding = ChartRangePaddingType.Additional;
                        indicator.avtArea = area;
                        sarea.Areas[sarea.Areas.Count - 1].PrimaryAxis.ZoomFactor = indicator.Series.Area.PrimaryAxis.ZoomFactor;
                        if (sarea.PrimaryAxis.ZoomFactor != 1)
                        {
                            ChartAreaCommands.ZoomIn.Execute(null, sarea);
                        }
                    }
                }
            }
        }

        private PointCollection ComputeAverageTrueRange(int len)
        {
            int expN = 2 * len - 1;
            PointCollection sd = new PointCollection();
            double alpha = 2 / (1d + len);
            double oneMinusAlpha = 1d - alpha;
            double lastClose = double.NaN; ;
            for(int i=0; i< Points.Count;i++)
            {
                if (double.IsNaN(lastClose))
                {
                    double Xvalue = 0d;

                    if (indicator.Series.XAxis.ValueType == ChartValueType.DateTime)
                    {
                        if (indicator.Series.Area.SyncChartArea == null)
                        {
                            double val = DateTime.Parse(Points[i].StringItem.ToString()).ToOADate();
                            Xvalue = Convert.ToDouble(Points[i].StringItem != null ? val : Points[i].X);
                        }
                        else
                        {
                            double val = Points[i].X;
                            Xvalue = Convert.ToDouble(val);
                        }
                    }
                    else
                    {
                        Xvalue = Convert.ToDouble(Points[i].StringItem != null ? Points[i].StringItem : Points[i].X);
                    }

                    sd.Add(new Point() { X = Xvalue, Y = double.NaN});
                }
                else
                {
                    double Xvalue = 0d;

                    if (indicator.Series.XAxis.ValueType == ChartValueType.DateTime)
                    {
                        if (indicator.Series.Area.SyncChartArea == null)
                        {
                            double val = DateTime.Parse(Points[i].StringItem.ToString()).ToOADate();
                            Xvalue = Convert.ToDouble(Points[i].StringItem != null ? val : Points[i].X);
                        }
                        else
                        {
                            double val = Points[i].X;
                            Xvalue = Convert.ToDouble(val);
                        }
                    }
                    else
                    {
                        Xvalue = Convert.ToDouble(Points[i].StringItem != null ? Points[i].StringItem : Points[i].X);
                    }

                    sd.Add(new Point()
                    {
                        X = Xvalue,
                        Y = Math.Max(Points[i].Values[1], lastClose) - Math.Min(Points[i].Values[0], lastClose),
                        
                    });
                }

                lastClose = Points[i].Y;
            }

            sd[0] = new Point() { X = sd[0].X, Y = sd[1].Y };

            sd = ComputeExponentialAverage(expN, sd);

            sd[0] = new Point() { X = sd[0].X, Y = sd[1].Y };
                
            return sd;
        }

        private PointCollection ComputeExponentialAverage(int len, PointCollection data)
        {            
            double alpha = 2 / (1d + len);
            PointCollection sd = new PointCollection();
            double lastValue = double.NaN;
            double oneMinusAlpha = 1d - alpha;
            
            for(int i=0; i<data.Count; i++)
            {
                if (double.IsNaN(lastValue))
                {
                    lastValue = data[i].Y;
                }
                else
                {
                    lastValue = alpha * data[i].Y + oneMinusAlpha * lastValue;
                }
                sd.Add(new Point() { X = data[i].X, Y = lastValue});
                
            }
            return sd;
        }
    }
}
