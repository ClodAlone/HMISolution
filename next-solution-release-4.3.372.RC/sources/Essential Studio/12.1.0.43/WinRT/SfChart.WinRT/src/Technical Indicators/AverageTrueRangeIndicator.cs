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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Collections;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Collections;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents AverageTrueRange technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    [ClassReference(IsReviewed = false)]
    public class AverageTrueRangeIndicator:FinancialTechnicalIndicator
    {
        #region constructor

        #endregion

        #region fields

        IList<double> _closeValues = new List<double>();

        IList<double> _highValues = new List<double>();

        IList<double> _lowValues = new List<double>();


        List<double> _xValues;

        List<double> xPoints = new List<double>();

        List<double> yPoints = new List<double>();

        TechnicalIndicatorSegment _fastLineSegment;

        #endregion

        #region Properties

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

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
            DependencyProperty.Register("Period", typeof(int), typeof(AverageTrueRangeIndicator), new PropertyMetadata(14, OnMovingAverageChanged));

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
        ///  Using a DependencyProperty as the backing store for SignalLineColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SignalLineColorProperty =
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(AverageTrueRangeIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        
        #endregion

        #region Methods

        private static void OnMovingAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var indicator = d as AverageTrueRangeIndicator;
            if (indicator != null) indicator.UpdateArea();
        }
        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            _fastLineSegment = null;
            _highValues.Clear();
            _lowValues.Clear();
            _closeValues.Clear();
            GeneratePoints(new[] {High,Low, Close },_highValues,_lowValues,_closeValues);
            UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            _fastLineSegment = null;
            _highValues.Clear();
            _lowValues.Clear();
            _closeValues.Clear();
            base.OnBindingPathChanged(args);
        }
        /// <summary>
        /// Method implementation for Set ItemSource to Series
        /// </summary>
        /// <param name="series"></param>
        protected internal override void SetSeriesItemSource(ChartSeriesBase series)
        {
            if (series.ActualSeriesYValues.Length > 3)
            {
                ActualXValues = Clone(series.ActualXValues);
                _highValues = Clone(series.ActualSeriesYValues[0]);
                _lowValues = Clone(series.ActualSeriesYValues[1]);
                _closeValues = Clone(series.ActualSeriesYValues[2]);
                Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Method implementation for GeneratePoints for TechnicalIndicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new[] { High, Low, Close }, _highValues, _lowValues, _closeValues);
        }

        /// <summary>
        /// Creates the segments of AverageTrueRangeIndicator.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            _xValues = GetXValues();

            ComputeAverageTrueRange(Period);

            if (_fastLineSegment == null)
            {
                _fastLineSegment = new TechnicalIndicatorSegment(xPoints, yPoints, SignalLineColor, this,Period);
                Segments.Add(_fastLineSegment);
            }
            else
            {
                _fastLineSegment.SetData(xPoints, yPoints);
                _fastLineSegment.SetRange();
            }
        }

        private void ComputeAverageTrueRange(int len)
        {
            xPoints.Clear();
            yPoints.Clear();

            int expN = 2 * len - 1;
            double lastClose = double.NaN;
            for (int i = 0; i < DataCount; i++)
            {
                if (double.IsNaN(lastClose))
                {
                    xPoints.Add(_xValues[i]);
                    yPoints.Add(double.NaN);
                }
                else
                {
                    xPoints.Add(_xValues[i]);
                    yPoints.Add(Math.Max(_lowValues[i], lastClose) - Math.Min(_highValues[i], lastClose));
                }

                lastClose = _highValues[i];
            }

            //sd[0] = new Point() { X = sd[0].X, Y = sd[1].Y };
            xPoints[0] = xPoints[0];
            yPoints[0] = yPoints[1];

            ComputeExponentialAverage(expN);

            //sd[0] = new Point() { X = sd[0].X, Y = sd[1].Y };
            xPoints[0] = xPoints[0];
            yPoints[0] = yPoints[1];
        }

        private void ComputeExponentialAverage(int len)
        {
            double alpha = 2 / (1d + len);
            double lastValue = double.NaN;
            double oneMinusAlpha = 1d - alpha;
            int count = xPoints.Count;
            for (int i = 0; i < count; i++)
            {
                if (double.IsNaN(lastValue))
                {
                    lastValue = yPoints[i];
                }
                else
                {
                    lastValue = alpha * yPoints[i] + oneMinusAlpha * lastValue;
                }
                xPoints.Add(xPoints[i]);
                yPoints.Add(lastValue);
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
            Area.ScheduleUpdate();
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new AverageTrueRangeIndicator() { SignalLineColor = this.SignalLineColor, Period = this.Period };
            return base.CloneSeries(obj);
        }

        #endregion

    }
}
