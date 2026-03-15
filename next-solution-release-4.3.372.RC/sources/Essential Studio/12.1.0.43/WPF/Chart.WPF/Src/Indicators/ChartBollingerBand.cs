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

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartBollingerBand
    /// </summary>
    public class ChartBollingerBand : DependencyObject
    {

        #region AttachedProperty

        /// <summary>
        ///  Identifies the UpperlineColor dependency property.
        /// </summary>
        public static readonly DependencyProperty UpperLineColorProperty =
          DependencyProperty.RegisterAttached("UpperLineColor", typeof(Brush), typeof(ChartBollingerBand), new ChartPropertyMetadata(Brushes.Red, new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Return the Brush value from the Dependency object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetUpperLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(UpperLineColorProperty);
        }

        /// <summary>
        /// Set the UpperLineColor to the corresponding object from the given value.
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
          DependencyProperty.RegisterAttached("LowerLineColor", typeof(Brush), typeof(ChartBollingerBand), new ChartPropertyMetadata(Brushes.Blue, new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Return the brush value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetLowerLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(LowerLineColorProperty);
        }

        /// <summary>
        /// Set the LowerLineColor to the corresponding DependencyObject from the given value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetLowerLineColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(LowerLineColorProperty, value);
        }

        /// <summary>
        ///  Identifies the PrimaryAxisStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty SignalLineColorProperty =
        DependencyProperty.RegisterAttached("SignalLineColor", typeof(Brush), typeof(ChartBollingerBand), new ChartPropertyMetadata(Brushes.DarkGreen,new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        ///  Return the Brush Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetSignalLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SignalLineColorProperty);
        }

        /// <summary>
        /// Set SignalLineColor to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSignalLineColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(SignalLineColorProperty, value);
        }

        /// <summary>
        ///  Identifies the BollingerMovingAverage dependency property.
        /// </summary>
        public static readonly DependencyProperty BollingerMovingAverageProperty =
       DependencyProperty.RegisterAttached("BollingerMovingAverage", typeof(int), typeof(ChartBollingerBand), new ChartPropertyMetadata(20, new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// return the int Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetBollingerMovingAverage(DependencyObject obj)
        {
            return (int)obj.GetValue(BollingerMovingAverageProperty);
        }

        /// <summary>
        /// Set BollingerMovingAverage to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetBollingerMovingAverage(DependencyObject obj, int value)
        {
            obj.SetValue(BollingerMovingAverageProperty, value);
        }

        internal static readonly DependencyProperty UpperPointsProperty =
            DependencyProperty.Register("UpperPoints", typeof(PointCollection), typeof(ChartBollingerBand), new PropertyMetadata(null));

        internal PointCollection UpperPoints
        {
            get
            {
                return (PointCollection)GetValue(UpperPointsProperty);
            }
            set
            {
                SetValue(UpperPointsProperty, value);
            }
        }

        internal static readonly DependencyProperty LowerPointsProperty =
            DependencyProperty.Register("LowerPoints", typeof(PointCollection), typeof(ChartBollingerBand), new PropertyMetadata(null));

        internal PointCollection LowerPoints
        {
            get
            {
                return (PointCollection)GetValue(LowerPointsProperty);
            }
            set
            {
                SetValue(LowerPointsProperty, value);
            }
        }
        internal static readonly DependencyProperty SignalPointsProperty =
            DependencyProperty.Register("SignalPoints", typeof(PointCollection), typeof(ChartBollingerBand), new PropertyMetadata(null));
        internal PointCollection SignalPoints
        {
            get
            {
                return (PointCollection)GetValue(SignalPointsProperty);
            }
            set
            {
                SetValue(SignalPointsProperty, value);
            }
        }

        internal IChartData VisiblePt = null;

        internal double MaxYValue = 0d;
        internal double MinYValue = 0d;
        internal ChartTechnicalIndicator indicator = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Called when instance created for ChartBollingBand
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="indicator"></param>
        public ChartBollingerBand(IChartData Points, ChartTechnicalIndicator indicator)
        {
            this.VisiblePt = Points;
            this.indicator = indicator;
            if (Points.Count > ChartBollingerBand.GetBollingerMovingAverage(indicator))
            {
                AddBollinger(Points);
            }
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"/> has been updated. The specific dependency property that changed is reported in the event data. 
        /// </summary>
        /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "BollingerMovingAverage")
            {

            }
            base.OnPropertyChanged(e);
        }
        #endregion

        #region HelperMethods

        private static void OnAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            ChartTechnicalIndicator indicator = d as ChartTechnicalIndicator;
            if (indicator != null && indicator.Series != null && indicator.Series.Presenter != null && indicator.Series.Presenter.m_IndicatorPresenter != null)
            {
                if (e.Property.ToString() == "BollingerMovingAverage")
                {
                    if (indicator.BollingerIndicator != null && indicator.BollingerIndicator.VisiblePt.Count > ChartBollingerBand.GetBollingerMovingAverage(indicator))
                    {
                        indicator.BollingerIndicator.AddBollinger(indicator.BollingerIndicator.VisiblePt);
                    }
                }
                indicator.Series.Presenter.m_IndicatorPresenter.InvalidateVisual();
            }
        }

        private void AddBollinger(IChartData Points)
        {            
            int pad = 0;

            int parmInt = 0;

            object[] parms = new object[] { ChartBollingerBand.GetBollingerMovingAverage(indicator), 2.0d };
            parmInt = GetInt(0, parms, Convert.ToInt32(ChartBollingerBand.GetBollingerMovingAverage(indicator)));
            double bandWidth = GetDouble(1, parms, 2d);

            SignalPoints = indicator.ComputeMovingAverage(parmInt, VisiblePt);
            List<Point> sd = ComputeStdDeviationOfPrice(parmInt);

            UpperPoints = new PointCollection(VisiblePt.Count);
            LowerPoints = new PointCollection(VisiblePt.Count);
            for (int i = 0; i < VisiblePt.Count; ++i)
            {
                UpperPoints.Add(new Point() { Y = SignalPoints[i].Y + (bandWidth * sd[i].Y), X = SignalPoints[i].X });
                LowerPoints.Add(new Point() { Y = SignalPoints[i].Y - (bandWidth * sd[i].Y), X = SignalPoints[i].X });
            }
            pad = parmInt - 1;
        }

        private int GetInt(int i, object[] parms, int def)
        {
            int val = def;
            if (parms != null && parms.GetLength(0) > i && parms[i] != null)
            {
                int.TryParse(parms[i].ToString(), out val);
            }
            return val;
        }

        private double GetDouble(int i, object[] parms, double def)
        {
            double val = def;
            if (parms != null && parms.GetLength(0) > i && parms[i] != null)
            {
                double.TryParse(parms[i].ToString(), out val);
            }
            return val;
        }

        private List<Point> ComputeStdDeviationOfPrice(int len)
        {
            List<Point> data = new List<Point>();
            double sumX = 0d;
            double sumX2 = 0d;
            double price;
            double price1;
            double pad = 0;
            for (int i = 0; i < VisiblePt.Count; ++i)
            {
                data.Add(new Point());
                price = VisiblePt[i].Values[3];
                if (i >= len - 1)
                {
                    if (i - len >= 0)
                    {
                        price1 = VisiblePt[i - len].Values[3];
                        sumX += price - price1;
                        sumX2 += price * price - price1 * price1;
                    }
                    else
                    {
                        sumX += price;
                        sumX2 += price * price;
                    }

                    data[i] = new Point(VisiblePt[i].X, Math.Sqrt((sumX2 - sumX * sumX / len) / (len - 1)));

                    pad = Math.Sqrt((sumX2 - sumX * sumX / len) / (len - 1));
                }
                else
                {
                    sumX += price;
                    sumX2 += price * price;
                }
            }
            object padDate = VisiblePt[len - 1].X;
            for (int i = 0; i < len - 1; i++)
            {
                data[i] = new Point(Convert.ToDouble(padDate), pad);
            }

            return data;

        }
        #endregion
    }
}
