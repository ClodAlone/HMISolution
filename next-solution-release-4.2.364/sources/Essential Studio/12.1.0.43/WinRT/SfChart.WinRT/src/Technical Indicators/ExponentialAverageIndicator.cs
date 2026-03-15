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
    /// Represents ExponentialAverage technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    [ClassReference(IsReviewed = false)]
    public class ExponentialAverageIndicator:FinancialTechnicalIndicator
    {
        #region constructor

        #endregion

        #region fields

        IList<double> CloseValues = new List<double>();

        List<double> xValues;

        List<double> xPoints = new List<double>();

        List<double> yPoints = new List<double>();

        TechnicalIndicatorSegment fastLineSegment;

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
        /// Using a DependencyProperty as the backing store for Period.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PeriodProperty =
            DependencyProperty.Register("Period", typeof(int), typeof(ExponentialAverageIndicator), new PropertyMetadata(2, OnPeriodChanged));

        /// <summary>
        /// Gets or Sets the signal line color
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
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(ExponentialAverageIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        #endregion

        #region Methods

        private static void OnPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExponentialAverageIndicator indicator = d as ExponentialAverageIndicator;
            indicator.UpdateArea();
        }
        /// <summary>
        /// Called when datasource changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            fastLineSegment = null;
            CloseValues.Clear();
            GeneratePoints(new string[] { Close }, CloseValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            fastLineSegment = null;
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
        /// Creates the Segments of ExponentialAverageIndicator.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            xValues = GetXValues();

            CalculateExponential(Period, xValues, xPoints, yPoints);

            if (fastLineSegment == null)
            {
                fastLineSegment = new TechnicalIndicatorSegment(xValues, yPoints, SignalLineColor, this,Period);
                Segments.Add(fastLineSegment);
            }
            else if (ActualXValues != null)
            {
                fastLineSegment.SetData(xPoints, yPoints);
                fastLineSegment.SetRange();
            }
        }

        private void CalculateExponential(int len, List<double> xValues, List<double> xPoints, List<double> yPoints)
        {
            double lastValue = double.NaN;
            double alpha = 2 / (1d + len);
            double oneMinusAlpha = 1d - alpha;
            xPoints.Clear(); 
            yPoints.Clear();
            for (int j = 0; j < this.DataCount; j++)
            {
                if (double.IsNaN(lastValue))
                {
                    lastValue = CloseValues[j];
                }
                else
                {
                    lastValue = alpha * CloseValues[j] + oneMinusAlpha * lastValue;
                }
                yPoints.Add(lastValue);
                xPoints.Add(xValues[j]);
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

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new ExponentialAverageIndicator() { Period = this.Period, SignalLineColor = this.SignalLineColor };
            return base.CloneSeries(obj);
        }

        #endregion
    }
}
