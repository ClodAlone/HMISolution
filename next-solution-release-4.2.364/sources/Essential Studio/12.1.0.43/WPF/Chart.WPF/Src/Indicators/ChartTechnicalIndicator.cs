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
    public class ChartTechnicalIndicator: DependencyObject, INotifyPropertyChanged, IChartSerializer
    {

        #region DependencyProperty
        /// <summary>
        /// Identifies the VisiblePoints dependency property.
        /// </summary>
        public static readonly DependencyProperty VisiblePointsProperty =
          DependencyProperty.Register("VisiblePoints", typeof(IChartData), typeof(ChartTechnicalIndicator), new PropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Get and Set visiblePointsProperty
        /// </summary>
        public IChartData VisiblePoints
        {
            get
            {
                return (IChartData)GetValue(VisiblePointsProperty);
            }
            set
            {
                SetValue(VisiblePointsProperty, value);

            }
        }
        /// <summary>
        /// Identifies the MACDArea dependency property.
        /// </summary>
        public static readonly DependencyProperty MACDAreaProperty = DependencyProperty.Register("MACDArea", typeof(ChartArea), typeof(ChartTechnicalIndicator), new PropertyMetadata(null));
        /// <summary>
        /// Get and Set MACDAreaProperty
        /// </summary>
        public ChartArea MACDArea
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
        /// Identifies the Series dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
         DependencyProperty.Register("Series", typeof(ChartSeries), typeof(ChartTechnicalIndicator), new PropertyMetadata(null));

        /// <summary>
        /// Get and Set SeriesProperty
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
        /// Identifies the indicatorType dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorTypeProperty =
         DependencyProperty.Register("IndicatorType", typeof(IndicatorTypes), typeof(ChartTechnicalIndicator), new PropertyMetadata(IndicatorTypes.BollingerBands, new PropertyChangedCallback(OnTypeChanged)));

        /// <summary>
        /// Get and Set IndicatorTypeProperty
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

        internal static readonly DependencyProperty MACDIndicatorProperty =
        DependencyProperty.Register("MACDIndicator", typeof(ChartMACD), typeof(ChartTechnicalIndicator), new PropertyMetadata(null));

        internal ChartMACD MACDIndicator
        {
            get
            {
                return (ChartMACD)GetValue(MACDIndicatorProperty);
            }
            set
            {
                SetValue(MACDIndicatorProperty, value);

            }
        }
        internal static readonly DependencyProperty ExponentialIndicatorProperty =
        DependencyProperty.Register("ExponentialIndicator", typeof(ChartExponentialAverage), typeof(ChartTechnicalIndicator), new PropertyMetadata(null));

        internal ChartExponentialAverage ExponentialIndicator
        {
            get
            {
                return (ChartExponentialAverage)GetValue(ExponentialIndicatorProperty);
            }
            set
            {
                SetValue(ExponentialIndicatorProperty, value);

            }
        }
       
        internal ChartSimpleAverage simpleAverage = null;
        internal ChartTriangularAverage triangularIndicator = null;
        internal ChartStochastics stochasticsIndicator = null;
        /// <summary>
        /// Public variable stocasticsArea declaration for access outside of this class
        /// </summary>
        public ChartArea stocasticsArea = null;
        internal ChartAccumulationDistribution accumulationIndicator = null;
        /// <summary>
        /// Public variable accumulationArea declaration for access outside of this class
        /// </summary>
        public ChartArea accumulationArea = null;
        internal ChartRelativeStrengthIndex rsiIndicator = null;
        /// <summary>
        /// Public variable rsiArea declaration for access outside of this class
        /// </summary>
        public ChartArea rsiArea = null;
        internal ChartMomentum momentumIndicator = null;
        /// <summary>
        /// Public variable momentumArea declaration for access outside of this class
        /// </summary>
        public ChartArea momentumArea = null;
        internal ChartAverageTrueRange avtIndicator = null;
        /// <summary>
        /// Public variable avtArea declaration for access outside of this class
        /// </summary>
        public ChartArea avtArea = null;
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
            if (indicator.Series != null && indicator.Series.Data!= null && indicator.VisiblePoints == null)
            {
                indicator.VisiblePoints = indicator.Series.Data;
            }
            indicator.SetChartTypes(indicator);
            if(indicator.Series != null && indicator.Series.Presenter != null && indicator.Series.Presenter.m_IndicatorPresenter != null)
                indicator.Series.Presenter.m_IndicatorPresenter.InvalidateVisual();           
        }


        internal void SetChartTypes(ChartTechnicalIndicator indicator)
        {
            if (indicator.VisiblePoints != null)
            {
                ChartIndexedDataPoint[] Points = new ChartIndexedDataPoint[indicator.VisiblePoints.Count];

                switch (indicator.IndicatorType)
                {
                    case IndicatorTypes.MACD:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.MACDIndicator = new ChartMACD(indicator.VisiblePoints, this);                            
                        }
                        break;
                    case IndicatorTypes.BollingerBands:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            //if (indicator.BollingerIndicator == null)
                            {
                                indicator.BollingerIndicator = new ChartBollingerBand(indicator.VisiblePoints, this);
                            }
                        }
                        break;
                    case IndicatorTypes.ExponentialAverage:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.ExponentialIndicator = new ChartExponentialAverage(indicator.VisiblePoints, this);
                        }
                        break;
                    case IndicatorTypes.SimpleAverage:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.simpleAverage = new ChartSimpleAverage(indicator.VisiblePoints, this);
                        }
                        break;
                    case IndicatorTypes.TriangularAverage:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.triangularIndicator = new ChartTriangularAverage(indicator.VisiblePoints, this);
                        }
                        break;
                    case IndicatorTypes.Stochastics:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.stochasticsIndicator = new ChartStochastics(indicator.VisiblePoints, this);
                        }
                        break;
                    case IndicatorTypes.AccumulationDistribution:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.accumulationIndicator = new ChartAccumulationDistribution(indicator.VisiblePoints, this);
                        }
                        break;
                    case IndicatorTypes.RelativeStrengthIndex:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.rsiIndicator = new ChartRelativeStrengthIndex(indicator.VisiblePoints, this);
                        }
                        break;
                    case IndicatorTypes.Momentum:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.momentumIndicator = new ChartMomentum(indicator.VisiblePoints, this);
                        }
                        break;
                    case IndicatorTypes.AverageTrueRange:
                        if (indicator.VisiblePoints.Count > 0)
                        {
                            indicator.avtIndicator = new ChartAverageTrueRange(indicator.VisiblePoints, this);
                        }
                        break;
                }
            }
            if (indicator.Series != null && indicator.Series.Area != null)
            {
                indicator.Series.Area.isIndicator = true;
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"/> has been updated. The specific dependency property that changed is reported in the event data.
        /// </summary>
        /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(e.Property.Name));
                if (this.Series != null)
                {
                    if (e.Property.ToString() == "BollingerIndicator" || e.Property.Name == "SignalLineInterior" || e.Property.Name == "DivergenceLineColor" || e.Property.Name == "ConvergenceLineColor" || e.Property.Name == "BollingerMovingAverage" || e.Property.Name == "UpperLineColor" || e.Property.Name == "SignalLineColor" || e.Property.Name == "LowerLineColor")
                    {
                        if (this.Series != null && this.Series.Presenter != null && this.Series.Presenter.m_IndicatorPresenter != null)
                        {
                            this.Series.Presenter.m_IndicatorPresenter.InvalidateVisual();
                        }
                    }
                }
            }
        }

        internal PointCollection ComputeMovingAverage(int len, IChartData VisiblePt)
        {
            PointCollection data = null;

            data = new PointCollection();
            double sum = 0d;
            double pad = VisiblePt[len - 1].Values[3];          
            object padDate = VisiblePt[len - 1].X;
            int limit = VisiblePt.Count;
            for (int i = 0; i < VisiblePt.Count; ++i)
            {
                data.Add(new Point());
                if (i >= len - 1 && i < limit)
                {
                    if (i - len >= 0)
                    {
                        sum += VisiblePt[i].Values[3] - VisiblePt[i - len].Values[3];

                    }
                    else
                    {
                        sum += VisiblePt[i].Values[3];
                    }

                    data[i] = new Point(VisiblePt[i].X, sum / len);

                }
                else
                {
                    if (i < len - 1)
                    {
                        sum += VisiblePt[i].Values[3];
                    }

                    //data[i] = new Point(VisiblePt[i].X, pad);
                }
            }
            return data;
        }

        internal PointCollection ComputeMovingAverage(int len, PointCollection source, IChartData VisiblePt)
        {
            PointCollection data = new PointCollection(); ;

            double sum = 0d;
            double pad = source[len - 1].Y;
            double padDate = source[len - 1].X;
            int limit = VisiblePt.Count;
            for (int i = 0; i < VisiblePt.Count; ++i)
            {
                data.Add(new System.Windows.Point());

                if (i >= len - 1 && i < limit)
                {
                    if (i - len >= 0)
                    {
                        sum += source[i].Y - source[i - len].Y;
                    }
                    else
                    {
                        sum += source[i].Y;
                    }
                    data[i] = new System.Windows.Point(source[i].X, sum / len);
                }
                else
                {
                    if (i < len - 1)
                    {
                        sum += source[i].Y;
                    }

                    data[i] = new System.Windows.Point(pad, padDate);
                }
            }
            return data;
        }


        #endregion

        /// <summary>
        /// Empty constructor for ChartTechnicalIndicator
        /// </summary>
        public ChartTechnicalIndicator()
        {

        }

        /// <summary>
        /// Called When instance created for ChartTechnicalIndicator
        /// </summary>
        /// <param name="series"></param>
        /// <param name="data"></param>
        public ChartTechnicalIndicator(ChartSeries series, IChartData data)
        {
            this.Series = series;
            this.VisiblePoints = data;
            SetChartTypes(this);          
             
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Occurs when any property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IChartSerializer Members

        /// <summary>
        /// Method declaration for Serialize
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string _xamlString;
            _xamlString = XamlWriter.Save(this);
            return _xamlString;
        }

        /// <summary>
        /// Method declaration for DeSerialize
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }
}
