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
    /// Class implementation for ChartTriangularAverage
    /// </summary>
    public class ChartTriangularAverage:DependencyObject
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
                DependencyProperty.RegisterAttached("SignalLineInterior", typeof(Brush), typeof(ChartTriangularAverage), new PropertyMetadata(new SolidColorBrush(Colors.Red)));


        /// <summary>
        /// Identifies the MovingAverage dependency property.
        /// </summary>
        public static readonly DependencyProperty MovingAverageProperty =
       DependencyProperty.RegisterAttached("MovingAverage", typeof(int), typeof(ChartTriangularAverage), new PropertyMetadata(20, new PropertyChangedCallback(OnAverageChanged)));

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
        internal static readonly DependencyProperty SimpleAverageSignalPointsProperty =
            DependencyProperty.Register("SimpleAverageSignalPoints", typeof(PointCollection), typeof(ChartTriangularAverage), new PropertyMetadata(null));
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
           DependencyProperty.Register("SignalInterior", typeof(Brush), typeof(ChartTriangularAverage), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

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
        /// Called when Instance created for ChartTriangularAverage
        /// </summary>
        /// <param name="VisiblePoints"></param>
        /// <param name="indicator"></param>
        public ChartTriangularAverage(ChartPointsCollection VisiblePoints, ChartTechnicalIndicator indicator)
        {
            this.data = VisiblePoints;
            this.indicator = indicator;
            PointCollection sd = this.AddTriangularPoints(ChartTriangularAverage.GetMovingAverage(indicator));
            SimpleAverageSignalPoints = new PointCollection();
            for (int i = 0; i < sd.Count; i++)
            {
                SimpleAverageSignalPoints.Add(new Point() { X = this.indicator.Series.Area.ValueToPoint(this.indicator.Series.XAxis, sd[i].X), Y = this.indicator.Series.Area.ValueToPoint(this.indicator.Series.YAxis, sd[i].Y) });
            }
            ResourceDictionary resources = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
            };
            Brush highinterior = ChartTriangularAverage.GetSignalLineInterior(indicator);
            Brush seriesinterior = indicator.Series.Interior;
            this.SignalInterior = (highinterior == null) ? seriesinterior : highinterior;

            this.SimpleAveragePresenter = new ContentPresenter() { ContentTemplate = resources["TriangularAverageIndicator"] as DataTemplate, Content = this };

        }
        #endregion

        private PointCollection AddTriangularPoints(int avg)
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

            PointCollection data = this.indicator.ComputeMovingAverage(len1, this.data);
            data = indicator.ComputeMovingAverage(len2, this.data);
            double pad = data[len1 + len2 - 1].Y;
            for (int i = 0; i < len1 + len2 - 1; ++i)
            {
                data[i] = new System.Windows.Point(data[i].X, pad);
            }
            return data;
        }

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
