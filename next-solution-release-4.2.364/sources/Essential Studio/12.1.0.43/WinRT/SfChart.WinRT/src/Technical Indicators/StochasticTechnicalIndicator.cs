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
    /// Represents Stochastic technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    [ClassReference(IsReviewed = false)]
    public class StochasticTechnicalIndicator:FinancialTechnicalIndicator
    {
        #region constructor

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region fields

        IList<double> HighValues = new List<double>();

        IList<double> LowValues = new List<double>();

        IList<double> CloseValues = new List<double>();

        List<double> xValues;

        List<double> upperXPoints = new List<double>();

        List<double> upperYPoints = new List<double>();

        List<double> lowerXPoints = new List<double>();

        List<double> lowerYPoints = new List<double>();

        List<double> signalXPoints = new List<double>();

        List<double> signalYPoints = new List<double>();

        List<double> averageXPoints = new List<double>();

        List<double> averageYPoints = new List<double>();


        TechnicalIndicatorSegment upperLineSegment;

        TechnicalIndicatorSegment lowerLineSegment;

        TechnicalIndicatorSegment signalLineSegment;

        TechnicalIndicatorSegment movingAverageSegment;

        #endregion

        #region Properties



        public int Period
        {
            get { return (int)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Period.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PeriodProperty =
            DependencyProperty.Register("Period", typeof(int), typeof(StochasticTechnicalIndicator), new PropertyMetadata(14));

        

        public int KPeriod
        {
            get { return (int)GetValue(KPeriodProperty); }
            set { SetValue(KPeriodProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KPeriod.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KPeriodProperty =
            DependencyProperty.Register("KPeriod", typeof(int), typeof(StochasticTechnicalIndicator), new PropertyMetadata(5));



        public int DPeriod
        {
            get { return (int)GetValue(DPeriodProperty); }
            set { SetValue(DPeriodProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DPeriod.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DPeriodProperty =
            DependencyProperty.Register("DPeriod", typeof(int), typeof(StochasticTechnicalIndicator), new PropertyMetadata(3));



        public Brush PeriodLineColor
        {
            get { return (Brush)GetValue(PeriodLineColorProperty); }
            set { SetValue(PeriodLineColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PeriodLineColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PeriodLineColorProperty =
            DependencyProperty.Register("PeriodLineColor", typeof(Brush), typeof(StochasticTechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        

        /// <summary>
        /// Gets or Sets the upper line color
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
            DependencyProperty.Register("UpperLineColor", typeof(Brush), typeof(StochasticTechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

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
        ///  Using a DependencyProperty as the backing store for LowerLineColor.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty LowerLineColorProperty =
            DependencyProperty.Register("LowerLineColor", typeof(Brush), typeof(StochasticTechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

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
        /// Using a DependencyProperty as the backing store for SignalLineColor.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty SignalLineColorProperty =
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(StochasticTechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));


        #endregion

        #region Methods

        /// <summary>
        /// Called when DataSource property changed
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            HighValues.Clear();
            LowValues.Clear();
            CloseValues.Clear();
            GeneratePoints(new string[] { High,Low,Close }, HighValues,LowValues,CloseValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            HighValues.Clear();
            LowValues.Clear();
            CloseValues.Clear();
            base.OnBindingPathChanged(args);
        }
        /// <summary>
        /// Method implementation for Set ItemSource to Series
        /// </summary>
        /// <param name="series"></param>
        protected internal override void SetSeriesItemSource(ChartSeriesBase series)
        {
            if (series.ActualSeriesYValues.Length > 2)
            {
                this.ActualXValues = Clone(series.ActualXValues);
                this.HighValues = Clone(series.ActualSeriesYValues[0]);
                this.LowValues = Clone(series.ActualSeriesYValues[1]);
                this.CloseValues = Clone(series.ActualSeriesYValues[2]);
                this.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Method implementation for GeneratePoints for TechnicalIndicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { High, Low, Close }, HighValues, LowValues, CloseValues);
        }

        /// <summary>
        /// Creates the segments of StochasticTechnicalIndicator
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            xValues = GetXValues();
            AddPoints(KPeriod,DPeriod,signalXPoints,signalYPoints);
            AddPoints(Period, DPeriod,averageXPoints,averageYPoints);
            if (upperLineSegment == null || lowerLineSegment == null || signalLineSegment == null)
            {
                Segments.Clear();
                upperLineSegment = new TechnicalIndicatorSegment(upperXPoints, upperYPoints, UpperLineColor, this);
                Segments.Add(upperLineSegment);
                lowerLineSegment = new TechnicalIndicatorSegment(lowerXPoints, lowerYPoints, LowerLineColor, this);
                Segments.Add(lowerLineSegment);
                signalLineSegment = new TechnicalIndicatorSegment(signalXPoints, signalYPoints, SignalLineColor, this, KPeriod+DPeriod);
                Segments.Add(signalLineSegment);
                movingAverageSegment = new TechnicalIndicatorSegment(averageXPoints, averageYPoints, PeriodLineColor, this, Period);
                Segments.Add(movingAverageSegment);
            }
            else
            {
                upperLineSegment.SetData(upperXPoints, upperYPoints);
                upperLineSegment.SetRange();
                lowerLineSegment.SetData(lowerXPoints, lowerYPoints);
                lowerLineSegment.SetRange();
                signalLineSegment.SetData(signalXPoints, signalYPoints);
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

        private void AddPoints(int Period1, int Period2, List<double> xpoint, List<double> ypoint)
        {
            if (this.DataCount > Period1 + Period2)
            {
                ComputeStochastics(Period1, Period2, xpoint, ypoint);

                var xPoints = (from val in xValues select val).ToList();
                upperXPoints.AddRange(xPoints);
                lowerXPoints.AddRange(xPoints);

                for (int i = 0; i < this.DataCount; ++i)
                {
                    upperYPoints.Add(80);
                    lowerYPoints.Add(20);
                }
            }
        }

        private void ComputeStochastics(int len1, int len2, List<double> xPoints, List<double> yPoints)
        {
            xPoints.Clear();
            yPoints.Clear();

            int len = len1 + len2;
            List<double> mins = new List<double>();
            List<double> maxs = new List<double>();
            double max;
            double min;
            double top = 0;
            double bottom = 0;
            for (int i = 0; i < len1 - 1; ++i)
            {
                maxs.Add(0);
                mins.Add(0);
            }

            for (int i = len1 - 1; i < this.DataCount; ++i)
            {
                min = double.MaxValue;
                max = double.MinValue;
                for (int j = 0; j < len1; ++j)
                {
                    min = Math.Min(min, this.LowValues[i - j]);
                    max = Math.Max(max, this.HighValues[i - j]);
                }
                maxs.Add(max);
                mins.Add(min);
            }

            for (int i = len - 1; i < this.DataCount; ++i)
            {

                top = 0;
                bottom = 0;
                for (int j = 0; j < len2; ++j)
                {
                    top += this.CloseValues[i - j] - mins[i - j];
                    bottom += maxs[i - j] - mins[i - j];
                }
                xPoints.Add(xValues[i]);
                yPoints.Add(top / bottom * 100);
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new StochasticTechnicalIndicator() { SignalLineColor = this.SignalLineColor, LowerLineColor = this.LowerLineColor, UpperLineColor = this.UpperLineColor };
            return base.CloneSeries(obj);
        }

        #endregion

    }
}
