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
    /// Represents RelativeStrengthIndex technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    [ClassReference(IsReviewed = false)]
    public class RSITechnicalIndicator:FinancialTechnicalIndicator
    {
        #region constructor  

        #endregion

        #region fields

        IList<double> CloseValues = new List<double>();

        List<double> xValues;

        List<double> xPoints = new List<double>();

        List<double> yPoints = new List<double>();

        List<double> upperXPoints = new List<double>();

        List<double> upperYPoints = new List<double>();

        List<double> lowerXPoints = new List<double>();

        List<double> lowerYPoints = new List<double>();

        TechnicalIndicatorSegment upperLineSegment;

        TechnicalIndicatorSegment lowerLineSegment;

        TechnicalIndicatorSegment signalLineSegment;

        #endregion

        #region Properties



        public int Period
        {
            get { return (int)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Period.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PeriodProperty =
            DependencyProperty.Register("Period", typeof(int), typeof(RSITechnicalIndicator), new PropertyMetadata(14));

        

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
        /// Using a DependencyProperty as the backing store for UpperLineColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UpperLineColorProperty =
            DependencyProperty.Register("UpperLineColor", typeof(Brush), typeof(RSITechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

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
            DependencyProperty.Register("LowerLineColor", typeof(Brush), typeof(RSITechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

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
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(RSITechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));


        #endregion

        #region Methods

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
          /// Creates the segments of RelativeStrengthIndexIndicator.
         /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
             xValues = GetXValues();
             ComputeRSI(Period);
             var tempxPoints = (from val in xValues select val).ToList();
             upperXPoints.AddRange(tempxPoints);
             lowerXPoints.AddRange(tempxPoints);

             for (int i = 0; i < this.DataCount; ++i)
             {
                 upperYPoints.Add(70);
                 lowerYPoints.Add(30);
             }

             if (upperLineSegment == null || lowerLineSegment == null || signalLineSegment == null)
             {
                 Segments.Clear();
                 upperLineSegment = new TechnicalIndicatorSegment(upperXPoints, upperYPoints,UpperLineColor, this);
                 Segments.Add(upperLineSegment);
                 lowerLineSegment = new TechnicalIndicatorSegment(lowerXPoints, lowerYPoints,LowerLineColor, this);
                 Segments.Add(lowerLineSegment);
                 signalLineSegment = new TechnicalIndicatorSegment(xPoints, yPoints,SignalLineColor, this,Period);
                 Segments.Add(signalLineSegment);
             }
             else
             {
                 upperLineSegment.SetData(upperXPoints, upperYPoints);
                 upperLineSegment.SetRange();
                 lowerLineSegment.SetData(lowerXPoints, lowerYPoints);
                 lowerLineSegment.SetRange();
                 signalLineSegment.SetData(xPoints, yPoints);
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

        private void ComputeRSI(int len)
        {
            xPoints.Clear();
            yPoints.Clear();
            double c = 0, c1 = 0;
            double pmf = 0d;
            double nmf = 0d;
            c1 = CloseValues[0];
            
            for (int i = 1; i < len; ++i)
            {
                c = CloseValues[i];                
                if (c > c1)
                    pmf += c - c1;
                else if (c < c1)
                    nmf += c1 - c;
                c1 = c;
            }
            c = CloseValues[len];
            if (c > c1)
                pmf += c - c1;
            else if (c < c1)
                nmf += c1 - c;
            c1 = c;

            xPoints.Add(xValues[len]);
            yPoints.Add(100 - 100 / (1 + pmf / nmf));

            for (int i = 1; i < DataCount; ++i)
            {
                c = CloseValues[i];
                if (c > c1)
                {
                    pmf = (pmf * (len - 1) + (c - c1)) / len;
                    nmf = (nmf * (len - 1)) / len;
                }
                else if (c < c1)
                {
                    nmf = (nmf * (len - 1) + (c1 - c)) / len;
                    pmf = (pmf * (len - 1)) / len;
                }
                c1 = c;
                xPoints.Add(xValues[i]);
                yPoints.Add(100 - 100 / (1 + pmf / nmf));
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new RSITechnicalIndicator() { LowerLineColor = this.LowerLineColor, UpperLineColor = this.UpperLineColor, SignalLineColor = this.SignalLineColor };
            return base.CloneSeries(obj);
        }

        #endregion
    }
}
