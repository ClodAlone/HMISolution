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
    /// Class implementation for ChartTriangularAverage
    /// </summary>
    public class ChartTriangularAverage:DependencyObject
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
                DependencyProperty.RegisterAttached("SignalLineColor", typeof(Brush), typeof(ChartTriangularAverage), new ChartPropertyMetadata(Brushes.Navy, new PropertyChangedCallback(OnAverageChanged)));


        /// <summary>
        ///  Identifies the TriangularAverage dependency property.
        /// </summary>
        public static readonly DependencyProperty TriangularAverageProperty =
       DependencyProperty.RegisterAttached("TriangularAverage", typeof(int), typeof(ChartTriangularAverage), new ChartPropertyMetadata(20, new PropertyChangedCallback(OnAverageChanged), ChartPropertyMetadataOptions.AffectsUpdate));

        /// <summary>
        /// Return TriangularAverage value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetTriangularAverage(DependencyObject obj)
        {
            return (int)obj.GetValue(TriangularAverageProperty);
        }

        /// <summary>
        /// Set the TriangularAverage value to the given corresponding DependencyObject from the given value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetTriangularAverage(DependencyObject obj, int value)
        {
            obj.SetValue(TriangularAverageProperty, value);
        }
        internal PointCollection TriangularIndicatorPoints = null;
        private IChartData VisiblePt = null;
        private ChartTechnicalIndicator indicator = null;

        /// <summary>
        /// Called when instance created for ChartTriangularAverage
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indic"></param>
        public ChartTriangularAverage(IChartData data, ChartTechnicalIndicator indic)
        {
            this.VisiblePt = data;
            this.indicator = indic;
            int average = ChartTriangularAverage.GetTriangularAverage(indicator);
            if (average < this.VisiblePt.Count)
            {
                this.AddTriangularPoints(average);
            }
        }

        private void AddTriangularPoints(int avg)
        {
            int parmInt = avg;
            int len1, len2;
            if (parmInt % 2 == 0)
            {
                len1 = parmInt / 2;
                len2 = len1 + 1;
            }
            else
            {
                len1 = (parmInt + 1) / 2;
                len2 = len1;
            }
            
            PointCollection data = this.indicator.ComputeMovingAverage(len1,this.VisiblePt);
            data = indicator.ComputeMovingAverage(len2, data, this.VisiblePt);
            double pad = data[len1 + len2 - 1].Y;
            for (int i = 0; i < len1 + len2 - 1; ++i)
            {
                data[i] = new System.Windows.Point(data[i].X, pad);
            }
            this.TriangularIndicatorPoints = data;
        }

        private static void OnAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartTechnicalIndicator indicator = d as ChartTechnicalIndicator;
            if (indicator != null && indicator.Series != null && indicator.Series.Presenter != null && indicator.Series.Presenter.m_IndicatorPresenter != null)
            {
                if (e.Property.ToString() == "TriangularAverage")
                {
                    if (indicator.triangularIndicator.VisiblePt.Count > ChartTriangularAverage.GetTriangularAverage(indicator))
                    {
                        indicator.triangularIndicator.AddTriangularPoints(ChartTriangularAverage.GetTriangularAverage(indicator));
                    }
                }
                indicator.Series.Presenter.m_IndicatorPresenter.InvalidateVisual();
            }
        }
   
    }

}
