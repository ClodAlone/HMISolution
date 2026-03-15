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
    /// Class implementation for ChartStochastics
    /// </summary>
    public class ChartStochastics:DependencyObject
    {
        /// <summary>
        ///  Identifies the UpperLineColor dependency property.
        /// </summary>
        public static readonly DependencyProperty UpperLineColorProperty =
          DependencyProperty.RegisterAttached("UpperLineColor", typeof(Brush), typeof(ChartStochastics), new ChartPropertyMetadata(Brushes.Red, ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Return UpperLineColor value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetUpperLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(UpperLineColorProperty);
        }

        /// <summary>
        /// Set UpperLineColor value to the corresponding given DependencyObject from the given Brush value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetUpperLineColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(UpperLineColorProperty, value);
        }

        /// <summary>
        ///  Identifies the LowerLineColor dependency property.
        /// </summary>
        public static readonly DependencyProperty LowerLineColorProperty =
          DependencyProperty.RegisterAttached("LowerLineColor", typeof(Brush), typeof(ChartStochastics), new ChartPropertyMetadata(Brushes.Blue, ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// return LowerLineColor value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetLowerLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(LowerLineColorProperty);
        }

        /// <summary>
        /// Set LowerLineColor value to the corresponding DependencyObject from the given Brush value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetLowerLineColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(LowerLineColorProperty, value);
        }

        /// <summary>
        ///  Identifies the SignalLineColor dependency property.
        /// </summary>
        public static readonly DependencyProperty SignalLineColorProperty =
        DependencyProperty.RegisterAttached("SignalLineColor", typeof(Brush), typeof(ChartStochastics), new ChartPropertyMetadata(Brushes.DarkGreen, ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Return SignalLineColor value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetSignalLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SignalLineColorProperty);
        }

        /// <summary>
        /// Set SignalLineColor value from the given DependencyObject from the given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSignalLineColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(SignalLineColorProperty, value);
        }

        internal PointCollection UpperLinePoitns = null;
        internal PointCollection LowerLinePoints = null;
        internal PointCollection StochasticsPoints = null;

        private IChartData points = null;
        private ChartTechnicalIndicator chartIndicator = null;
        /// <summary>
        /// Called when ChartStochastics class instance Created
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indicator"></param>
        public ChartStochastics(IChartData data, ChartTechnicalIndicator indicator)
        {
            points = data;
            chartIndicator = indicator;
            AddPoints();
            if (this.UpperLinePoitns != null && this.LowerLinePoints != null && this.StochasticsPoints != null)
            {
                if (indicator.stocasticsArea == null)
                {
                    ChartArea area = new ChartArea();
                    if (indicator.Series.Area.SyncChartArea != null)
                    {
                        SyncChartAreas sarea = indicator.Series.Area.SyncChartArea as SyncChartAreas;
                        sarea.Areas.Add(area);

                        //sarea.Areas[sarea.Areas.Count - 1].Width = sarea.ActualWidth;
                        sarea.Areas[sarea.Areas.Count - 1].ElementMargin = indicator.Series.Area.ElementMargin;
                        sarea.Areas[sarea.Areas.Count - 1].Margin = indicator.Series.Area.Margin;
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.LowerLinePoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartStochastics.GetLowerLineColor(indicator), ToolTip = "Stochastics Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.UpperLinePoitns, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartStochastics.GetUpperLineColor(indicator), ToolTip = "Stochastics Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.StochasticsPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartStochastics.GetSignalLineColor(indicator), ToolTip = "Stochastics Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].isIndicator = true;
                        foreach (ChartSeries ser in sarea.Areas[sarea.Areas.Count - 1].Series)
                        {
                            if (sarea.Areas[sarea.Areas.Count - 1].Series.IndexOf(ser) == 2)
                            {
                                for (int i = 0; i < ser.Data.Count - 5; i++)
                                {
                                    ser.Data[i].X = this.StochasticsPoints[i].X;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < ser.Data.Count; i++)
                                {
                                    ser.Data[i].X = this.LowerLinePoints[i].X;
                                }
                            }
                        }
                        sarea.Areas[0].SplitterPosition = double.IsNaN(area.SplitterPosition) || double.IsPositiveInfinity(area.SplitterPosition) || area.SplitterPosition == 0 ? 0.5 : area.SplitterPosition;
                        indicator.stocasticsArea = area;
                        sarea.Areas[sarea.Areas.Count - 1].PrimaryAxis.ZoomFactor = indicator.Series.Area.PrimaryAxis.ZoomFactor;
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
            if(this.points.Count > 8)
            {
                this.StochasticsPoints = ComputeStochastics(5, 3);
                this.UpperLinePoitns = new PointCollection();
                this.LowerLinePoints = new PointCollection();
                for (int i = 0; i < this.points.Count; ++i)
                {
                    this.UpperLinePoitns.Add(new Point() { X = points[i].X, Y = 80});
                    this.LowerLinePoints.Add(new Point() { X = points[i].X, Y = 20});
                }
            }
        }

        private PointCollection ComputeStochastics(int len1, int len2)
        {
            int len = len1 + len2;

            PointCollection sd1 = new PointCollection();
            List<double> mins = new List<double>();
            List<double> maxs = new List<double>();
            double max;
            double min;
            double top = 0;
            double bottom = 0;
            for (int i = 0; i < len1 - 1; ++i)
            {
                maxs.Add(0);
                mins.Add(0);
                double Xvalue = 0d;
                
                if (chartIndicator.Series.XAxis.ValueType == ChartValueType.DateTime)
                {
                    if (chartIndicator.Series.Area.SyncChartArea == null)
                    {
                        double val = DateTime.Parse(points[i].StringItem.ToString()).ToOADate();
                        Xvalue = Convert.ToDouble(points[i].StringItem != null ? val : points[i].X);
                    }
                    else
                    {
                        double val = points[i].X;
                        Xvalue = Convert.ToDouble(val);
                    }
                }
                else
                {
                    Xvalue = Convert.ToDouble(points[i].StringItem != null ? points[i].StringItem : points[i].X);
                }
                //sd1.Add(new Point() { X = this.points[i].X });
            }

            for (int i = len1 - 1; i < this.points.Count; ++i)
            {
                min = double.MaxValue;
                max = double.MinValue;
                for (int j = 0; j < len1; ++j)
                {
                    min = Math.Min(min, this.points[i - j].Values[1]);
                    max = Math.Max(max, this.points[i - j].Values[0]);
                }
                maxs.Add(max);
                mins.Add(min);
                if (i < len - 1)
                {
                    //sd1.Add(new Point() { X = this.points[i].X });
                }
            }

            for (int i = len - 1; i < this.points.Count; ++i)
            {

                top = 0;
                bottom = 0;
                for (int j = 0; j < len2; ++j)
                {
                    top += this.points[i - j].Values[3] - mins[i - j];
                    bottom += maxs[i - j] - mins[i - j];
                }
                sd1.Add(new Point() { X = this.points[i].X, Y = top / bottom * 100 });

            }

            double pad = sd1[len - 1].Y;
            for (int i = 0; i < len - 1; ++i)
            {
                //sd1[i] = new Point(sd1[i].X, pad);

            }

            return sd1;
        }    

    }
}
