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
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents MACD technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    [ClassReference(IsReviewed = false)]
    public class MACDTechnicalIndicator:FinancialTechnicalIndicator
    {
        #region ctor

        #endregion

        #region fields

        IList<double> CloseValues = new List<double>();

        List<double> xValues;

        List<double> xPoints = new List<double>();

        List<double> yPoints = new List<double>();

        List<double> ConvergenceXPoints = new List<double>();

        List<double> ConvergenceYPoints = new List<double>();

        List<double> DivergenceXPoints = new List<double>();

        List<double> DivergenceYPoints = new List<double>();

        List<double> HistogramYPoints = new List<double>();

        List<double> CenterXPoints = new List<double>();

        List<double> CenterYPoints = new List<double>();

        IList<double> x1Values, x2Values, y1Values, y2Values;

        TechnicalIndicatorSegment ConvergenceLineSegment;

        TechnicalIndicatorSegment DivergenceLineSegment;

        TechnicalIndicatorSegment CenterlLineSegment;

        FastColumnBitmapSegment HistogramSegment; 

        #endregion

        #region Properties        

        protected internal override bool IsSideBySide
        {
            get
            {
                return Type == MACDType.Histogram || Type == MACDType.Both ? true : false;
            }
        }

        public MACDType Type
        {
            get { return (MACDType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(MACDType), typeof(MACDTechnicalIndicator), new PropertyMetadata(MACDType.Line, OnValueChanged));

        

        public int ShortPeriod
        {
            get { return (int)GetValue(ShortPeriodProperty); }
            set { SetValue(ShortPeriodProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShortPeriod.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShortPeriodProperty =
            DependencyProperty.Register("ShortPeriod", typeof(int), typeof(MACDTechnicalIndicator), new PropertyMetadata(12, OnValueChanged));



        public int LongPeriod
        {
            get { return (int)GetValue(LongPeriodProperty); }
            set { SetValue(LongPeriodProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LongPeriod.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LongPeriodProperty =
            DependencyProperty.Register("LongPeriod", typeof(int), typeof(MACDTechnicalIndicator), new PropertyMetadata(26, OnValueChanged));

        
        

        /// <summary>
        /// Gets or sets the moving average
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
            DependencyProperty.Register("Period", typeof(int), typeof(MACDTechnicalIndicator), new PropertyMetadata(9, OnValueChanged));
        
        /// <summary>
        /// Gets or sets the convergence line color
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush ConvergenceLineColor
        {
            get { return (Brush)GetValue(ConvergenceLineColorProperty); }
            set { SetValue(ConvergenceLineColorProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for UpperLineColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ConvergenceLineColorProperty =
            DependencyProperty.Register("ConvergenceLineColor", typeof(Brush), typeof(MACDTechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Gets or sets the divergence line color
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush DivergenceLineColor
        {
            get { return (Brush)GetValue(DivergenceLineColorProperty); }
            set { SetValue(DivergenceLineColorProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LowerLineColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DivergenceLineColorProperty =
            DependencyProperty.Register("DivergenceLineColor", typeof(Brush), typeof(MACDTechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

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
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(MACDTechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));


        #endregion

        #region Methods

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MACDTechnicalIndicator indicator = d as MACDTechnicalIndicator;
            indicator.UpdateArea();
        }

        /// <summary>
        /// Called when DataSource changed
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
        /// Creates the segments of MACDTechnicalIndicator.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            xValues = GetXValues();
            AddMACDPoints();

            if (!Segments.Contains(ConvergenceLineSegment) || !Segments.Contains(DivergenceLineSegment)|| !Segments.Contains(CenterlLineSegment) || !Segments.Contains(HistogramSegment))
            {
                Segments.Clear();
                if (Type == MACDType.Histogram || Type == MACDType.Both)
                {
                    CalculateHistogram();
                    HistogramSegment = new FastColumnBitmapSegment(x1Values, y1Values, x2Values, y2Values, this);
                    Segments.Add(HistogramSegment);
                }
                if (Type == MACDType.Line || Type == MACDType.Both)
                {
                    ConvergenceLineSegment = new TechnicalIndicatorSegment(ConvergenceXPoints, ConvergenceYPoints, ConvergenceLineColor, this, Period);
                    Segments.Add(ConvergenceLineSegment);
                    DivergenceLineSegment = new TechnicalIndicatorSegment(DivergenceXPoints, DivergenceYPoints, DivergenceLineColor, this,Period);
                    Segments.Add(DivergenceLineSegment);
                    CenterlLineSegment = new TechnicalIndicatorSegment(CenterXPoints, CenterYPoints, SignalLineColor, this);
                    Segments.Add(CenterlLineSegment);
                }
            }
            else
            {
                if (Type == MACDType.Both)
                {
                    HistogramSegment.SetData(x1Values, y1Values, x2Values, y2Values);
                    ConvergenceLineSegment.SetData(ConvergenceXPoints, ConvergenceYPoints);
                    ConvergenceLineSegment.SetRange();
                    DivergenceLineSegment.SetData(DivergenceXPoints, DivergenceYPoints);
                    DivergenceLineSegment.SetRange();
                    CenterlLineSegment.SetData(CenterXPoints, CenterYPoints);
                    CenterlLineSegment.SetRange();
                }
                else if (Type == MACDType.Line)
                {
                    Segments.Remove(HistogramSegment);
                    HistogramSegment = null;
                    ConvergenceLineSegment.SetData(ConvergenceXPoints, ConvergenceYPoints);
                    ConvergenceLineSegment.SetRange();
                    DivergenceLineSegment.SetData(DivergenceXPoints, DivergenceYPoints);
                    DivergenceLineSegment.SetRange();
                    CenterlLineSegment.SetData(CenterXPoints, CenterYPoints);
                    CenterlLineSegment.SetRange();
                }
                else if (Type == MACDType.Histogram)
                {
                    Segments.Remove(ConvergenceLineSegment);
                    Segments.Remove(DivergenceLineSegment);
                    Segments.Remove(CenterlLineSegment);
                    ConvergenceLineSegment = null;
                    DivergenceLineSegment = null;
                    CenterlLineSegment = null;
                    HistogramSegment.SetData(x1Values, y1Values, x2Values, y2Values);

                }
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

        /// <summary>
        /// Method implementation  for Add MACD property values
        /// </summary>
        public void AddMACDPoints()
        {
            int len1 = LongPeriod;
            int len2 = ShortPeriod;
            int len3 = Period;
            DivergenceXPoints.Clear();
            DivergenceYPoints.Clear();
            ConvergenceXPoints.Clear();
            ConvergenceYPoints.Clear();
            CenterXPoints.Clear();
            CenterYPoints.Clear();
            PointCollection sp1 = ComputeExponentialAverage1(len1);
            PointCollection sp2 = ComputeExponentialAverage1(len2);

            for (int i = 0; i < DataCount; ++i)
            {
                DivergenceXPoints.Add(sp1[i].X);
                DivergenceYPoints.Add(sp2[i].Y - sp1[i].Y);
            }

            ComputeExponentialAverage2(len3);

        }

        private void ComputeExponentialAverage2(int len)
        {

            if (len < 0)
                throw new ArgumentOutOfRangeException("Exponential Average Length must be greater than 0");
            double alpha = 2 / (1d + len);
            double lastValue = double.NaN;
            double oneMinusAlpha = 1d - alpha;

            for (int j = 0; j < DataCount; j++)
            {
                if (double.IsNaN(lastValue))
                {
                    lastValue = DivergenceYPoints[j];
                }
                else
                {
                    lastValue = alpha * DivergenceYPoints[j] + oneMinusAlpha * lastValue;
                }

                ConvergenceXPoints.Add(DivergenceXPoints[j]);
                ConvergenceYPoints.Add(lastValue);
                CenterXPoints.Add(DivergenceXPoints[j]);
                CenterYPoints.Add(0);
            }
        }

        internal PointCollection ComputeExponentialAverage1(int len)
        {
            PointCollection sd = new PointCollection();
            double lastValue = double.NaN;
            double alpha = 2 / (1d + len);
            double oneMinusAlpha = 1d - alpha;

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
                double Xvalue = xValues[j];

                sd.Add(new Point() { X = Xvalue, Y = lastValue });

            }
            return sd;
        }

        private void CalculateHistogram()
        {
            x1Values = new List<double>();
            x2Values = new List<double>();
            y1Values = new List<double>();
            y2Values = new List<double>();
            this.Area.SBSInfoCalculated = false;
            DoubleRange sbsInfo = this.GetSideBySideInfo(this);
            for (int j = 0; j < DataCount; j++)
            {
                HistogramYPoints.Add(DivergenceYPoints[j] - ConvergenceYPoints[j]);
                if (!this.IsIndexed)
                {
                    x1Values.Add(xValues[j] + sbsInfo.Start);
                    x2Values.Add(xValues[j] + sbsInfo.End);
                    y1Values.Add(HistogramYPoints[j]);
                    y2Values.Add(0);
                }
                else
                {
                    x1Values.Add(j + sbsInfo.Start);
                    x2Values.Add(j + sbsInfo.End);
                    y1Values.Add(HistogramYPoints[j]);
                    y2Values.Add(0);
                }
            }
        }

        private static void OnAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MACDTechnicalIndicator indicator = d as MACDTechnicalIndicator;
            indicator.Invalidate();
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new MACDTechnicalIndicator() { Period = this.Period, SignalLineColor = this.SignalLineColor, ConvergenceLineColor = this.ConvergenceLineColor, DivergenceLineColor = this.DivergenceLineColor };
            return base.CloneSeries(obj);
        }

        #endregion

    }
}
