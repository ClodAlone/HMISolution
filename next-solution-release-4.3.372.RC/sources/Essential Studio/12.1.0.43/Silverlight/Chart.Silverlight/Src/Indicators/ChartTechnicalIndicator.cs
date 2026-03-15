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
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartTechnicalIndicator
    /// </summary>
    public class ChartTechnicalIndicator: DependencyObject, INotifyPropertyChanged
    {

        #region DependencyProperty
        /// <summary>
        /// Identifies the VisiblePoints dependency property.
        /// </summary>
        public static readonly DependencyProperty VisiblePointsProperty =
          DependencyProperty.Register("VisiblePoints", typeof(ChartPointsCollection), typeof(ChartTechnicalIndicator), new PropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Get or Set Visiblepoints property
        /// </summary>
        public ChartPointsCollection VisiblePoints
        {
            get
            {
                return (ChartPointsCollection)GetValue(VisiblePointsProperty);
            }
            set
            {
                SetValue(VisiblePointsProperty, value);

            }
        }
        internal static readonly DependencyProperty MACDAreaProperty = DependencyProperty.Register("MACDArea", typeof(ChartArea), typeof(ChartTechnicalIndicator), new PropertyMetadata(null));
        internal ChartArea MACDArea
        {
            get
            {
                return (ChartArea)GetValue(MACDAreaProperty);
            }
            set
            {
                SetValue(MACDAreaProperty, value);

            }
        }
        /// <summary>
        ///  Identifies the Series dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
         DependencyProperty.Register("Series", typeof(ChartSeries), typeof(ChartTechnicalIndicator), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set Series Property
        /// </summary>
        public ChartSeries Series
        {
            get
            {
                return (ChartSeries)GetValue(SeriesProperty);
            }
            set
            {
                SetValue(SeriesProperty, value);

            }
        }
        /// <summary>
        /// Identifies the IndicatorType dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorTypeProperty =
         DependencyProperty.Register("IndicatorType", typeof(IndicatorTypes), typeof(ChartTechnicalIndicator), new PropertyMetadata(IndicatorTypes.BollingerBands, new PropertyChangedCallback(OnTypeChanged)));

        /// <summary>
        /// Get or Set IndicatorType
        /// </summary>
        public IndicatorTypes IndicatorType
        {
            get
            {
                return (IndicatorTypes)GetValue(IndicatorTypeProperty);
            }
            set
            {
                SetValue(IndicatorTypeProperty, value);

            }
        }


        internal static readonly DependencyProperty BollingerIndicatorProperty =
         DependencyProperty.Register("BollingerIndicator", typeof(ChartBollingerBand), typeof(ChartTechnicalIndicator), new PropertyMetadata(null));

        internal ChartBollingerBand BollingerIndicator
        {
            get
            {
                return (ChartBollingerBand)GetValue(BollingerIndicatorProperty);
            }
            set
            {
                SetValue(BollingerIndicatorProperty, value);

            }
        }

        internal SimpleAverage simpleaverage = null;
        internal ChartTriangularAverage trinagularIndicator = null;
        #endregion
        #region Methods

       
        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartTechnicalIndicator indicator = d as ChartTechnicalIndicator;
            indicator.SetChartTypes(indicator);
        }

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartTechnicalIndicator indicator = d as ChartTechnicalIndicator;
            indicator.SetChartTypes(indicator);
            if (indicator.Series != null && indicator.Series.Presenter != null)
            {
                indicator.Series.Presenter.LoadIndicators(indicator.Series);
            }
            if (indicator.Series != null && indicator.Series.Data!= null && indicator.VisiblePoints == null)
            {
                indicator.VisiblePoints = indicator.Series.Data;
            }
            
        }


        internal void SetChartTypes(ChartTechnicalIndicator indicator)
        {
            if (indicator.VisiblePoints != null)
            {                
                switch (indicator.IndicatorType)
                {                   
                    case IndicatorTypes.BollingerBands:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.BollingerIndicator = new ChartBollingerBand(indicator.VisiblePoints, this);
                        }
                        break;
                    case IndicatorTypes.SimpleAverage:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.simpleaverage = new SimpleAverage(indicator.VisiblePoints, this);
                        }
                        break;
                    case IndicatorTypes.TriangularAverage:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.trinagularIndicator = new ChartTriangularAverage(indicator.VisiblePoints, this);
                        }
                        break;
                }
            }
        }

        #endregion

        #region Implementation
        
        #endregion


        /// <summary>
        /// Called when instance  created for ChartTechnicalIndicator
        /// </summary>
        public ChartTechnicalIndicator()
        {
            this.PropertyChanged += new PropertyChangedEventHandler(ChartTechnicalIndicator_PropertyChanged);
        }

        void ChartTechnicalIndicator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((sender as ChartTechnicalIndicator).Series != null && (sender as ChartTechnicalIndicator).Series.Presenter != null)
            {
                (sender as ChartTechnicalIndicator).Series.Presenter.LoadIndicators((sender as ChartTechnicalIndicator).Series);
            }
        }

        internal PointCollection ComputeMovingAverage(int len, ChartPointsCollection VisiblePt)
        {
            PointCollection data = null;

            data = new PointCollection();
            double sum = 0d;
            double pad = VisiblePt[len - 1].Y;
            object padDate = VisiblePt[len - 1].X;
            int limit = VisiblePt.Count;
            for (int i = 0; i < VisiblePt.Count; ++i)
            {
                data.Add(new Point());
                if (i >= len - 1 && i < limit)
                {
                    if (i - len >= 0)
                    {
                        sum += VisiblePt[i].Y - VisiblePt[i - len].Y;

                    }
                    else
                    {
                        sum += VisiblePt[i].Y;
                    }

                    data[i] = new Point(VisiblePt[i].X, sum / len);

                }
                else
                {
                    if (i < len - 1)
                    {
                        sum += VisiblePt[i].Y;
                    }

                    data[i] = new Point(VisiblePt[i].X, pad);
                }
            }
            return data;
        }
#pragma warning disable 0067
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Occurs when any property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
#pragma warning restore 0067

    }
}
