#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for SimpleAverage
    /// </summary>
    public class SimpleAverage:DependencyObject
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
                DependencyProperty.RegisterAttached("SignalLineInterior", typeof(Brush), typeof(SimpleAverage), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Indicates the MovingAverage Dependency Property
        /// </summary>
        public static readonly DependencyProperty MovingAverageProperty =
       DependencyProperty.RegisterAttached("MovingAverage", typeof(int), typeof(SimpleAverage), new PropertyMetadata(20, new PropertyChangedCallback(OnAverageChanged)));
        /// <summary>
        /// Return int value from given dependenct object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetMovingAverage(DependencyObject obj)
        {
            return (int)obj.GetValue(MovingAverageProperty);
        }
        /// <summary>
        /// SetValue of MovingAverage to the corresponding given dependency object 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMovingAverage(DependencyObject obj, int value)
        {
            obj.SetValue(MovingAverageProperty, value);
        }
        internal static readonly DependencyProperty SimpleAverageSignalPointsProperty =
            DependencyProperty.Register("SimpleAverageSignalPoints", typeof(PointCollection), typeof(SimpleAverage), new PropertyMetadata(null));
        internal PointCollection SimpleAverageSignalPoints
        {
            get
            {
                return (PointCollection)GetValue(SimpleAverageSignalPointsProperty);
            }
            set
            {
                SetValue(SimpleAverageSignalPointsProperty, value);
            }
        }

        internal static readonly DependencyProperty SignalInteriorProperty =
           DependencyProperty.Register("SignalInterior", typeof(Brush), typeof(SimpleAverage), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        /// <summary>
        /// Gets or sets the SignalInterior. This is dependency property.
        /// </summary>
        /// <value>The template.</value>
        internal Brush SignalInterior
        {
            get { return (Brush)GetValue(SignalInteriorProperty); }
            set { SetValue(SignalInteriorProperty, value); }
        }

        #endregion
        internal ContentPresenter SimpleAveragePresenter = null;
        internal ChartTechnicalIndicator indicator = null;
        internal ChartPointsCollection data = null;
        #region Constructor
        /// <summary>
        /// Called when instance created for SimpleAverage SimpleAverage
        /// </summary>
        /// <param name="VisiblePoints"></param>
        /// <param name="indicator"></param>
        public SimpleAverage(ChartPointsCollection VisiblePoints, ChartTechnicalIndicator indicator)
        {
            this.data = VisiblePoints;
            this.indicator = indicator;
            PointCollection sd = this.indicator.ComputeMovingAverage(SimpleAverage.GetMovingAverage(indicator), VisiblePoints);
            SimpleAverageSignalPoints = new PointCollection();
            for (int i = 0; i < sd.Count; i++)
            {
                SimpleAverageSignalPoints.Add(new Point() { X = this.indicator.Series.Area.ValueToPoint(this.indicator.Series.XAxis, sd[i].X), Y = this.indicator.Series.Area.ValueToPoint(this.indicator.Series.YAxis, sd[i].Y) });
            }
            ResourceDictionary resources = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
            };
            Brush highinterior = SimpleAverage.GetSignalLineInterior(indicator);
            Brush seriesinterior = indicator.Series.Interior;
            this.SignalInterior = (highinterior == null) ? seriesinterior : highinterior;

            this.SimpleAveragePresenter = new ContentPresenter() { ContentTemplate = resources["SimpleAverageIndicator"] as DataTemplate, Content = this };

        }
        #endregion

        private static void OnAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartTechnicalIndicator indicator = d as ChartTechnicalIndicator;
            if (indicator != null && indicator.Series != null && indicator.Series.Presenter != null)
            {
                indicator.Series.Presenter.LoadIndicators(indicator.Series);
            }
        }

    }
}
