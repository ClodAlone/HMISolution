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
using System.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartBollingBand 
    /// </summary>
    public class ChartBollingerBand : DependencyObject
    {

        #region Property

        /// <summary>
        ///  Identifies the UpperLinecolor dependency property.
        /// </summary>
        public static readonly DependencyProperty UpperLineColorProperty =
          DependencyProperty.RegisterAttached("UpperLineColor", typeof(Brush), typeof(ChartBollingerBand), new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Return the Brush Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetUpperLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(UpperLineColorProperty);
        }

        /// <summary>
        /// Set UpperLineColor to the Corresponding DependencyObject from the Given value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetUpperLineColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(UpperLineColorProperty, value);
        }

        /// <summary>
        /// Identifies the LowerLineColor dependency property.
        /// </summary>
        public static readonly DependencyProperty LowerLineColorProperty =
          DependencyProperty.RegisterAttached("LowerLineColor", typeof(Brush), typeof(ChartBollingerBand), new PropertyMetadata(new SolidColorBrush(Colors.Blue), new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Return the Brush Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetLowerLineColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(LowerLineColorProperty);
        }

        /// <summary>
        /// Set LowerLineColor to the Corresponding DependencyObject from the Given value.
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
        DependencyProperty.RegisterAttached("SignalLineColor", typeof(Brush), typeof(ChartBollingerBand), new PropertyMetadata(new SolidColorBrush(Colors.Green), new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Return the Brush Value from the given DependencyObject
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
        /// Identifies the BollingMovingAverage dependency property.
        /// </summary>
        public static readonly DependencyProperty BollingerMovingAverageProperty =
       DependencyProperty.RegisterAttached("BollingerMovingAverage", typeof(int), typeof(ChartBollingerBand), new PropertyMetadata(20, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Called when instance created for GetBollingMovingAverage
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
            DependencyProperty.Register("UpperPoints", typeof(PointCollection), typeof(ChartBollingerBand), new PropertyMetadata(new PointCollection()));

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
            DependencyProperty.Register("LowerPoints", typeof(PointCollection), typeof(ChartBollingerBand), new PropertyMetadata(new PointCollection()));

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
        //internal static readonly DependencyProperty UpperbandPresenterProperty =
        //   DependencyProperty.Register("UpperbandPresenter", typeof(ContentPresenter), typeof(ChartBollingerBand), new PropertyMetadata(null));
        //internal ContentPresenter UpperbandPresenter
        //{
        //    get
        //    {
        //        return (ContentPresenter)GetValue(UpperbandPresenterProperty);
        //    }
        //    set
        //    {
        //        SetValue(UpperbandPresenterProperty, value);
        //    }
        //}

        //internal static readonly DependencyProperty LowerbandpresenterProperty =
        //  DependencyProperty.Register("Lowerbandpresenter", typeof(ContentPresenter), typeof(ChartBollingerBand), new PropertyMetadata(null));
        //internal ContentPresenter Lowerbandpresenter
        //{
        //    get
        //    {
        //        return (ContentPresenter)GetValue(LowerbandpresenterProperty);
        //    }
        //    set
        //    {
        //        SetValue(LowerbandpresenterProperty, value);
        //    }
        //}
        //internal static readonly DependencyProperty SignalPresenterProperty =
        //  DependencyProperty.Register("SignalPresenter", typeof(ContentPresenter), typeof(ChartBollingerBand), new PropertyMetadata(null));
        //internal ContentPresenter SignalPresenter
        //{
        //    get
        //    {
        //        return (ContentPresenter)GetValue(SignalPresenterProperty);
        //    }
        //    set
        //    {
        //        SetValue(SignalPresenterProperty, value);
        //    }
        //}
       

        /// <summary>
        /// Idenfities UpperInterior dependency property.
        /// </summary>
        internal static readonly DependencyProperty UpperInteriorProperty =
            DependencyProperty.Register("UpperInterior", typeof(Brush), typeof(ChartBollingerBand), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the UpperInterior. This is dependency property.
        /// </summary>
        /// <value>The template.</value>
        internal Brush UpperInterior
        {
            get { return (Brush)GetValue(UpperInteriorProperty); }
            set { SetValue(UpperInteriorProperty, value); }
        }

        internal static readonly DependencyProperty LowerInteriorProperty =
           DependencyProperty.Register("LowerInterior", typeof(Brush), typeof(ChartBollingerBand), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        /// <summary>
        /// Gets or sets the LowerInterior. This is dependency property.
        /// </summary>
        /// <value>The template.</value>
        internal Brush LowerInterior
        {
            get { return (Brush)GetValue(LowerInteriorProperty); }
            set { SetValue(LowerInteriorProperty, value); }
        }

        internal static readonly DependencyProperty SignalInteriorProperty =
           DependencyProperty.Register("SignalInterior", typeof(Brush), typeof(ChartBollingerBand), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        /// <summary>
        /// Gets or sets the SignalInterior. This is dependency property.
        /// </summary>
        /// <value>The template.</value>
        internal Brush SignalInterior
        {
            get { return (Brush)GetValue(SignalInteriorProperty); }
            set { SetValue(SignalInteriorProperty, value); }
        }

        internal ChartPointsCollection VisiblePt = new ChartPointsCollection();
        private const string LAST = "Last";
        internal ContentPresenter SignalPresenter = null;
        internal ContentPresenter Lowerbandpresenter = null;
        internal ContentPresenter UpperbandPresenter = null;
        internal ChartTechnicalIndicator indicator = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Called when instance created for  ChartBollingBand
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="indicator"></param>
        public ChartBollingerBand(ChartPointsCollection Points, ChartTechnicalIndicator indicator)
        {
            this.VisiblePt = Points;
            this.indicator = indicator;
            if (Points.Count > ChartBollingerBand.GetBollingerMovingAverage(indicator))
            {
                AddBollinger(Points);
            }
            
        }

        #endregion

        

        #region HelperMethods
        private void AddBollinger(ChartPointsCollection Points)
        {            
            int pad = 0;

            int parmInt = 0;

            object[] parms = new object[] { ChartBollingerBand.GetBollingerMovingAverage(indicator), 2.0d };
            parmInt = GetInt(0, parms,Convert.ToInt32(ChartBollingerBand.GetBollingerMovingAverage(indicator)));
            double bandWidth = GetDouble(1, parms, 2d);
            if (parmInt > 1)
            {
                PointCollection chartPt = indicator.ComputeMovingAverage(parmInt, Points);
                List<Point> sd = ComputeStdDeviationOfPrice(parmInt);

                UpperPoints = new PointCollection();
                LowerPoints = new PointCollection();
                for (int i = 0; i < VisiblePt.Count; ++i)
                {
                    UpperPoints.Add(new Point() { Y = this.indicator.Series.Area.ValueToPoint(this.indicator.Series.YAxis, chartPt[i].Y + (bandWidth * sd[i].Y)), X = this.indicator.Series.Area.ValueToPoint(this.indicator.Series.XAxis, chartPt[i].X) });
                    LowerPoints.Add(new Point() { Y = this.indicator.Series.Area.ValueToPoint(this.indicator.Series.YAxis, chartPt[i].Y - (bandWidth * sd[i].Y)), X = this.indicator.Series.Area.ValueToPoint(this.indicator.Series.XAxis, chartPt[i].X) });
                }
                SignalPoints = new PointCollection();
                for (int i = 0; i < chartPt.Count; ++i)
                {
                    SignalPoints.Add(new Point() { Y = this.indicator.Series.Area.ValueToPoint(this.indicator.Series.YAxis, chartPt[i].Y), X = this.indicator.Series.Area.ValueToPoint(this.indicator.Series.XAxis, chartPt[i].X) });
                }
                ResourceDictionary resources = new SharedResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
                };
                Brush highinterior = ChartBollingerBand.GetUpperLineColor(indicator);
                Brush seriesinterior = indicator.Series.Interior;
                this.UpperInterior = (highinterior == null) ? seriesinterior : highinterior;

                this.UpperbandPresenter = new ContentPresenter() { ContentTemplate = resources["BollingerUpperBandIndicator"] as DataTemplate, Content = this };

                Brush lowinterior = ChartBollingerBand.GetLowerLineColor(indicator);
                this.LowerInterior = (LowerInterior == null) ? seriesinterior : lowinterior;
                this.Lowerbandpresenter = new ContentPresenter() { ContentTemplate = resources["BollingerLowerBandIndicator"] as DataTemplate, Content = this };

                Brush signalInterior = ChartBollingerBand.GetSignalLineColor(indicator);
                this.SignalInterior = (signalInterior == null) ? seriesinterior : signalInterior;
                this.SignalPresenter = new ContentPresenter() { ContentTemplate = resources["BollingerSignalIndicator"] as DataTemplate, Content = this };

                pad = parmInt - 1;
            }
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
                price = VisiblePt[i].Y;
                if (i >= len - 1)
                {
                    if (i - len >= 0)
                    {
                        price1 = VisiblePt[i - len].Y;
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

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartTechnicalIndicator indicator = d as ChartTechnicalIndicator;
            if (indicator != null && indicator.Series != null && indicator.Series.Presenter != null)
            {
                indicator.Series.Presenter.LoadIndicators(indicator.Series);
            }
        }
        #endregion
    }
}

