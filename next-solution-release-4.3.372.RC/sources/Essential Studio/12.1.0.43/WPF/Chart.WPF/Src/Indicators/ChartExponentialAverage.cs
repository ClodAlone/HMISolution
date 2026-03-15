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
    /// Class implementation for ChartExponentialAverage
    /// </summary>
    public class ChartExponentialAverage : DependencyObject
    {
        #region members
        /// <summary>
        /// Gets the value of the SignalLineInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObjectobj.</param>
        /// <returns>The SignalLineInterior brush</returns>
        public static Brush GetSignalLineInterior(DependencyObject obj)
        {
            return (Brush)obj.GetValue(SignalLineInteriorProperty);
        }

        /// <summary>
        /// Sets the value of the SignalLineInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetSignalLineInterior(DependencyObject obj, Brush value)
        {
            obj.SetValue(SignalLineInteriorProperty, value);
        }

        /// <summary>
        /// Indicates the SignalLineInterior Dependency Property
        /// </summary>
        public static readonly DependencyProperty SignalLineInteriorProperty =
                DependencyProperty.RegisterAttached("SignalLineInterior", typeof(Brush), typeof(ChartExponentialAverage), new FrameworkPropertyMetadata(Brushes.Navy, new PropertyChangedCallback(OnAverageChanged)));
    

        /// <summary>
        /// Indicates the ExponentialAverage Dependency Property
        /// </summary>
        public static readonly DependencyProperty ExponentialAverageProperty =
       DependencyProperty.RegisterAttached("ExponentialAverage", typeof(int), typeof(ChartExponentialAverage), new ChartPropertyMetadata(20, new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Return the int Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetExponentialAverage(DependencyObject obj)
        {
            return (int)obj.GetValue(ExponentialAverageProperty);
        }

        /// <summary>
        /// Set ExponentialAverage to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetExponentialAverage(DependencyObject obj, int value)
        {
            obj.SetValue(ExponentialAverageProperty, value);
        }


        internal PointCollection ExponentialSignalPoints = null;
        internal IChartData VisiblePoints = null;

        internal ChartTechnicalIndicator indicator = null;
        #endregion


        #region Constructor
        /// <summary>
        /// called when instance created for ChartExponentialAverage
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="indicator"></param>
        public ChartExponentialAverage(IChartData Points, ChartTechnicalIndicator indicator)
        {
            this.VisiblePoints = Points;
            this.indicator = indicator;            
            this.ExponentialSignalPoints = this.CalculateExponential(ChartExponentialAverage.GetExponentialAverage(indicator));
        }      
        #endregion

        #region Methods
        private PointCollection CalculateExponential(int len)
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

                
                Xvalue = Convert.ToDouble(VisiblePoints[j].X);
                
                sd.Add(new Point() { X = Xvalue, Y = lastValue });


            }
            return sd;

        }

        private static void OnAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartTechnicalIndicator indicator = d as ChartTechnicalIndicator;
            if (indicator != null && indicator.Series != null && indicator.Series.Presenter != null && indicator.Series.Presenter.m_IndicatorPresenter != null)
            {
                if (e.Property.ToString() == "ExponentialAverage")
                {
                if (indicator.ExponentialIndicator.VisiblePoints.Count > ChartSimpleAverage.GetMovingAverage(indicator))
                {
                    indicator.ExponentialIndicator.ExponentialSignalPoints = indicator.ExponentialIndicator.CalculateExponential(ChartExponentialAverage.GetExponentialAverage(indicator));
                }
                }
                indicator.Series.Presenter.m_IndicatorPresenter.InvalidateVisual();
            }
        }
        #endregion


    }
}
