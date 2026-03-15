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
    /// Class implementation for ChartSimpleAverage
    /// </summary>
    public class ChartSimpleAverage:DependencyObject
    {
        #region attachedProperty
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
                DependencyProperty.RegisterAttached("SignalLineInterior", typeof(Brush), typeof(ChartSimpleAverage), new ChartPropertyMetadata(Brushes.Navy, new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));


        /// <summary>
        /// Indicates the MovingAverage Dependency Property
        /// </summary>
        public static readonly DependencyProperty MovingAverageProperty =
       DependencyProperty.RegisterAttached("MovingAverage", typeof(int), typeof(ChartSimpleAverage), new ChartPropertyMetadata(20, new PropertyChangedCallback(OnAverageChanged),ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Return the int Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetMovingAverage(DependencyObject obj)
        {
            return (int)obj.GetValue(MovingAverageProperty);
        }

        /// <summary>
        /// Set MovingAverage to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMovingAverage(DependencyObject obj, int value)
        {
            obj.SetValue(MovingAverageProperty, value);
        }
        #endregion
        internal PointCollection SimpleAverageSignalPoints = null;        
        internal ChartTechnicalIndicator indicator = null;
        internal IChartData data = null;
        #region Constructor
        /// <summary>
        /// Called when instance created for ChartSimpleAverage
        /// </summary>
        /// <param name="VisiblePoints"></param>
        /// <param name="indicator"></param>
        public ChartSimpleAverage(IChartData VisiblePoints, ChartTechnicalIndicator indicator)
        {
            this.data = VisiblePoints;
            this.indicator = indicator;
            SimpleAverageSignalPoints = this.indicator.ComputeMovingAverage(ChartSimpleAverage.GetMovingAverage(indicator), VisiblePoints);
        }
        #endregion

        private static void OnAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartTechnicalIndicator indicator = d as ChartTechnicalIndicator;
            if (indicator != null && indicator.Series != null && indicator.Series.Presenter != null && indicator.Series.Presenter.m_IndicatorPresenter != null)
            {
                if (e.Property.ToString() == "MovingAverage")
                {
                    if (indicator.simpleAverage.data.Count > ChartSimpleAverage.GetMovingAverage(indicator))
                    {
                        indicator.simpleAverage.SimpleAverageSignalPoints = indicator.ComputeMovingAverage(ChartSimpleAverage.GetMovingAverage(indicator), indicator.simpleAverage.data);
                    }
                }
                indicator.Series.Presenter.m_IndicatorPresenter.InvalidateVisual();
            }
        }

    }
}
