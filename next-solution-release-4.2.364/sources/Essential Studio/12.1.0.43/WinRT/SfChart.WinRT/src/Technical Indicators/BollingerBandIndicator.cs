#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Collections;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents BollingerBand technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    [ClassReference(IsReviewed = false)]
    public class BollingerBandIndicator:FinancialTechnicalIndicator
    {

        #region fields

        IList<double> CloseValues = new List<double>();

        List<double> xValues;

        List<double> upperXPoints = new List<double>();

        List<double> upperYPoints = new List<double>();

        List<double> lowerXPoints = new List<double>();

        List<double> lowerYPoints = new List<double>();

        List<double> signalXPoints = new List<double>();

        List<double> signalYPoints = new List<double>();

        TechnicalIndicatorSegment upperLineSegment;

        TechnicalIndicatorSegment lowerLineSegment;

        TechnicalIndicatorSegment signalLineSegment;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets the moving average
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public int Period
        {
            get { return (int)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for MovingAverage.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PeriodProperty =
            DependencyProperty.Register("Period", typeof(int), typeof(BollingerBandIndicator), new PropertyMetadata(20, OnMovingAverageChanged));

        /// <summary>
        /// Gets or sets the upper line color
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush UpperLineColor
        {
            get { return (Brush)GetValue(UpperLineColorProperty); }
            set { SetValue(UpperLineColorProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for UpperLineColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UpperLineColorProperty =
            DependencyProperty.Register("UpperLineColor", typeof(Brush), typeof(BollingerBandIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Gets or Sets the lower line color
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush LowerLineColor
        {
            get { return (Brush)GetValue(LowerLineColorProperty); }
            set { SetValue(LowerLineColorProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LowerLineColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LowerLineColorProperty =
            DependencyProperty.Register("LowerLineColor", typeof(Brush), typeof(BollingerBandIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Gets or sets the signal line color
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush SignalLineColor
        {
            get { return (Brush)GetValue(SignalLineColorProperty); }
            set { SetValue(SignalLineColorProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SignalLineColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SignalLineColorProperty =
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(BollingerBandIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        #endregion

        #region Methods

        private static void OnMovingAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BollingerBandIndicator indicator = d as BollingerBandIndicator;
            indicator.UpdateArea();
        }
        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            CloseValues.Clear();
            GeneratePoints(new string[] { Close }, CloseValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            CloseValues.Clear();
            base.OnBindingPathChanged(args);
        }
        /// <summary>
        /// Method implementation for Set ItemSource to Series
        /// </summary>
        /// <param name="series"></param>
        protected internal override void SetSeriesItemSource(ChartSeriesBase series)
        {
            if (series.ActualSeriesYValues.Length > 0)
            {
                this.ActualXValues = Clone(series.ActualXValues);
                this.CloseValues = Clone(series.ActualSeriesYValues[0]);
                this.Area.ScheduleUpdate();
            }
        }


        /// <summary>
        /// Method implementation for GeneratePoints for TechnicalIndicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { Close }, CloseValues);
        }

        /// <summary>
        /// Creates the segments of BollingerBandIndicator.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            xValues = GetXValues();
            Period = Period < xValues.Count ? Period : xValues.Count - 1;
            AddBollinger();
            List<double> tmpUpXPoints = new List<double>();
            List<double> tmpUpYPoints = new List<double>();

            List<double> tmpLwXPoints = new List<double>();
            List<double> tmpLwYPoints = new List<double>();

            List<double> tmpSgXPoints = new List<double>();
            List<double> tmpSgYPoints = new List<double>();

            for (int i = 0; i < upperXPoints.Count - 1; i++)
            {
                tmpUpXPoints.Add(upperXPoints[i]);
                tmpUpYPoints.Add(upperYPoints[i]);
                tmpLwXPoints.Add(lowerXPoints[i]);
                tmpLwYPoints.Add(lowerYPoints[i]);
                tmpSgXPoints.Add(signalXPoints[i]);
                tmpSgYPoints.Add(signalYPoints[i]);
            }
            if (upperLineSegment == null || lowerLineSegment == null || signalLineSegment == null)
            {
                upperLineSegment = new TechnicalIndicatorSegment(tmpUpXPoints, tmpUpYPoints, UpperLineColor, this,Period);
                Segments.Add(upperLineSegment);
                lowerLineSegment = new TechnicalIndicatorSegment(tmpLwXPoints, tmpLwYPoints, LowerLineColor, this,Period);
                Segments.Add(lowerLineSegment);
                signalLineSegment = new TechnicalIndicatorSegment(tmpSgXPoints, tmpSgYPoints, SignalLineColor, this,Period);
                Segments.Add(signalLineSegment);
            }
            else
            {
                upperLineSegment.SetData(tmpUpXPoints, tmpUpYPoints);
                upperLineSegment.SetRange();
                lowerLineSegment.SetData(tmpLwXPoints, tmpLwYPoints);
                lowerLineSegment.SetRange();
                signalLineSegment.SetData(tmpSgXPoints, tmpSgYPoints);
                signalLineSegment.SetRange();
            }
        }

        /// <summary>
        /// Updates the segment at the specified index
        /// </summary>
        /// <param name="index">The index of the segment.</param>
        /// <param name="action">The action that caused the segments collection changed event</param>
        [ClassReference(IsReviewed = false)]
        public override void UpdateSegments(int index, NotifyCollectionChangedAction action)
        {
            this.Area.ScheduleUpdate();
        }

        private void AddBollinger()
        {
            signalXPoints.Clear();
            signalYPoints.Clear();
            upperXPoints.Clear();
            upperYPoints.Clear();
            lowerXPoints.Clear();
            lowerYPoints.Clear();
            int pad = 0;
            int parmInt = 0;

            object[] parms = new object[] { Period, 2.0d };
            parmInt = Period;
            double bandWidth = GetDouble(1, parms, 2d);

            ComputeMovingAverage(parmInt, xValues, CloseValues, signalXPoints, signalYPoints);

            List<double> stdXPoints = new List<double>(),
                         stdYPoints = new List<double>();
            ComputeStdDeviationOfPrice(parmInt, stdXPoints, stdYPoints);

            for (int i = 0; i < DataCount; ++i)
            {
                upperXPoints.Add(signalXPoints[i]);
                lowerXPoints.Add(signalXPoints[i]);
                upperYPoints.Add(signalYPoints[i] + (bandWidth * stdYPoints[i]));
                lowerYPoints.Add(signalYPoints[i] - (bandWidth * stdYPoints[i]));
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

        private void ComputeStdDeviationOfPrice(int len, List<double> stdXPoints, List<double> stdYPoints)
        {
            double sumX = 0d;
            double sumX2 = 0d;
            double price;
            double price1;
            double pad = 0;
            for (int i = 0; i < DataCount; ++i)
            {
                stdXPoints.Add(0);
                stdYPoints.Add(0);
                price = CloseValues[i];
                if (i >= len - 1)
                {
                    if (i - len >= 0)
                    {
                        price1 = CloseValues[i - len];
                        sumX += price - price1;
                        sumX2 += price * price - price1 * price1;
                    }
                    else
                    {
                        sumX += price;
                        sumX2 += price * price;
                    }
                    stdXPoints[i] = xValues[i];
                    stdYPoints[i] = Math.Sqrt((sumX2 - sumX * sumX / len) / (len - 1));
                    pad = stdYPoints[i];
                }
                else
                {
                    sumX += price;
                    sumX2 += price * price;
                }
            }
            object padDate = xValues[len - 1];
            for (int i = 0; i < len - 1; i++)
            {
                stdXPoints[i] = Convert.ToDouble(padDate);
                stdYPoints[i] = pad;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new BollingerBandIndicator() { Period = this.Period, SignalLineColor = this.SignalLineColor, LowerLineColor = this.LowerLineColor, UpperLineColor = this.UpperLineColor };
            return base.CloneSeries(obj);
        }

        #endregion

    }
}
