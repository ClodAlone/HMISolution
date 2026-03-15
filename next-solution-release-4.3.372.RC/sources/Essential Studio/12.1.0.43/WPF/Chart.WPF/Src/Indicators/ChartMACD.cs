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
using System.Reflection;
using System.Globalization;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// class implementation for ChartMACD
    /// </summary>
    public class ChartMACD : DependencyObject
    {
        #region Dependencyproperty
        internal static readonly DependencyProperty ConvergencePointsProperty =
           DependencyProperty.Register("ConvergencePoints", typeof(PointCollection), typeof(ChartMACD), new PropertyMetadata(null));

        internal PointCollection ConvergencePoints
        {
            get
            {
                return (PointCollection)GetValue(ConvergencePointsProperty);
            }
            set
            {
                SetValue(ConvergencePointsProperty, value);
            }
        }

        internal static readonly DependencyProperty DivergencePointsProperty =
            DependencyProperty.Register("DivergencePoints", typeof(PointCollection), typeof(ChartMACD), new PropertyMetadata(null));

        internal PointCollection DivergencePoints
        {
            get
            {
                return (PointCollection)GetValue(DivergencePointsProperty);
            }
            set
            {
                SetValue(DivergencePointsProperty, value);
            }
        }

        internal static readonly DependencyProperty CenterPointsProperty =
            DependencyProperty.Register("CenterPoints", typeof(PointCollection), typeof(ChartMACD), new PropertyMetadata(null));

        internal PointCollection CenterPoints
        {
            get
            {
                return (PointCollection)GetValue(CenterPointsProperty);
            }
            set
            {
                SetValue(CenterPointsProperty, value);
            }
        }

        /// <summary>
        ///  Identifies the SignalLineInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty SignalLineInteriorProperty =
       DependencyProperty.RegisterAttached("SignalLineInterior", typeof(Brush), typeof(ChartMACD), new ChartPropertyMetadata(Brushes.DarkGreen, new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Return the Brush Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetSignalLineInterior(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SignalLineInteriorProperty);
        }

        /// <summary>
        /// Set SignalLineInterior to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSignalLineInterior(DependencyObject obj, Brush value)
        {
            obj.SetValue(SignalLineInteriorProperty, value);
        }

        /// <summary>
        /// Identifies the drivergenceLineColor dependency property.
        /// </summary>
        public static readonly DependencyProperty DivergenceLineColorProperty =
       DependencyProperty.RegisterAttached("DivergenceLineColor", typeof(Brush), typeof(ChartMACD), new ChartPropertyMetadata(Brushes.Navy, new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Return the Brush Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetDivergenceLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(DivergenceLineColorProperty);
        }

        /// <summary>
        /// Set drivergenceLineColor to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetDivergenceLineColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(DivergenceLineColorProperty, value);
        }

        /// <summary>
        /// Identifies the ConvergenceLineColor dependency property.
        /// </summary>
        public static readonly DependencyProperty ConvergenceLineColorProperty =
       DependencyProperty.RegisterAttached("ConvergenceLineColor", typeof(Brush), typeof(ChartMACD), new ChartPropertyMetadata(Brushes.Maroon, new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Return the Brush Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetConvergenceLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ConvergenceLineColorProperty);
        }

        /// <summary>
        /// Set ConvergenceLineColor to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetConvergenceLineColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(ConvergenceLineColorProperty, value);
        }

        internal ChartTechnicalIndicator indicator = null;
        internal IChartData VisiblePoints = null;
        #endregion

        #region constructor
        /// <summary>
        /// Called when instance created for ChartMACD 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="indic"></param>
        public ChartMACD(IChartData points, ChartTechnicalIndicator indic)
        {
            this.indicator = indic;
            this.VisiblePoints = points;

            AddMACDPoints();
            if (this.DivergencePoints != null && this.CenterPoints != null && this.ConvergencePoints != null)
            {
                if (indic.MACDArea == null)
                {
                    ChartArea area = new ChartArea();
                    if (indic.Series.Area.SyncChartArea != null)
                    {
                        SyncChartAreas sarea = indic.Series.Area.SyncChartArea as SyncChartAreas;
                        sarea.Areas.Add(area);

                        //sarea.Areas[sarea.Areas.Count - 1].Width = sarea.ActualWidth;
                        sarea.Areas[sarea.Areas.Count - 1].ElementMargin = indic.Series.Area.ElementMargin;
                        sarea.Areas[sarea.Areas.Count - 1].Margin = indic.Series.Area.Margin;
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.DivergencePoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartMACD.GetDivergenceLineColor(indic), ToolTip = "MACD Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.ConvergencePoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartMACD.GetConvergenceLineColor(indic), ToolTip = "MACD Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].Series.Add(new ChartSeries() { DataSource = this.CenterPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartMACD.GetSignalLineInterior(indic), ToolTip = "MACD Indicator" });
                        sarea.Areas[sarea.Areas.Count - 1].isIndicator = true;
                        foreach (ChartSeries ser in sarea.Areas[sarea.Areas.Count - 1].Series)
                        {
                            for (int i = 0; i < ser.Data.Count; i++)
                            {
                                ser.Data[i].X = this.DivergencePoints[i].X;
                            }
                        }
                        sarea.Areas[0].SplitterPosition = double.IsNaN(area.SplitterPosition) || double.IsPositiveInfinity(area.SplitterPosition) || area.SplitterPosition ==0  ? 0.5 : area.SplitterPosition;                                                
                        indic.MACDArea = area;
                        sarea.Areas[sarea.Areas.Count - 1].PrimaryAxis.ZoomFactor = indic.Series.Area.PrimaryAxis.ZoomFactor;
                        if (sarea.PrimaryAxis.ZoomFactor != 1)
                        {
                            ChartAreaCommands.ZoomIn.Execute(null, sarea);
                        }
                    }
                }
            }
        }
        #endregion

        #region HelperMethods

        /// <summary>
        /// Method implementation for add points to the collection
        /// </summary>
        public void AddMACDPoints()
        {
            int len1 = 26;
            int len2 = 12;
            int len3 = 9;

            PointCollection sp1 = ComputeExponentialAverage(len1);
            PointCollection sp2 = ComputeExponentialAverage(len2);
            DivergencePoints = new PointCollection();
            for (int i = 0; i < VisiblePoints.Count; ++i)
            {
                DivergencePoints.Add(new Point(sp1[i].X, sp1[i].Y - sp2[i].Y));
            }

            this.ConvergencePoints = ComputeExponentialAverage(len3, DivergencePoints);

        }
        private PointCollection ComputeExponentialAverage(int len, PointCollection data)
        {
            if (len < 0)
                throw new ArgumentOutOfRangeException("Exponential Average Length must be greater than 0");
            double alpha = 2 / (1d + len);
            PointCollection sd = new PointCollection();
            double lastValue = double.NaN;
            double oneMinusAlpha = 1d - alpha;
            int i = 0;
            this.CenterPoints = new PointCollection();
            foreach (var p in data)
            {
                if (double.IsNaN(lastValue))
                {
                    lastValue = p.Y;
                }
                else
                {
                    lastValue = alpha * p.Y + oneMinusAlpha * lastValue;
                }
                sd.Add(new Point() { X = p.X, Y = lastValue });
                this.CenterPoints.Add(new Point(this.DivergencePoints[i].X, 0));
                i++;
            }
            return sd;
        }

        internal PointCollection ComputeExponentialAverage(int len)
        {
            PointCollection sd = new PointCollection();
            double lastValue = double.NaN;
            double alpha = 2 / (1d + len);
            double oneMinusAlpha = 1d - alpha;

            for (int j = 0; j < this.VisiblePoints.Count; j++)
            {
                if (double.IsNaN(lastValue))
                {
                    lastValue = VisiblePoints[j].Values[3];
                }
                else
                {
                    lastValue = alpha * VisiblePoints[j].Values[3] + oneMinusAlpha * lastValue;
                }
                double Xvalue = 0d;

                if (indicator.Series.XAxis.ValueType == ChartValueType.DateTime)
                {
                    if (indicator.Series.Area.SyncChartArea == null)
                    {
                        double val = DateTime.Parse(VisiblePoints[j].StringItem.ToString()).ToOADate();
                        Xvalue = Convert.ToDouble(VisiblePoints[j].StringItem != null ? val : VisiblePoints[j].X);
                    }
                    else
                    {
                        double val = VisiblePoints[j].X;
                        Xvalue = Convert.ToDouble(val);
                    }
                }
                else
                {
                    Xvalue = Convert.ToDouble(VisiblePoints[j].StringItem != null ? VisiblePoints[j].StringItem : VisiblePoints[j].X);
                }
                sd.Add(new Point() { X = Xvalue, Y = lastValue });


            }
            return sd;
        }

        private static void OnAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartTechnicalIndicator indicator = d as ChartTechnicalIndicator;
            if (indicator != null && indicator.Series != null && indicator.Series.Presenter != null && indicator.Series.Presenter.m_IndicatorPresenter != null)
            {
                indicator.Series.Presenter.m_IndicatorPresenter.InvalidateVisual();
            }
        }


        #endregion
    }

}
