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
    /// Represents TriangularAverage technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    [ClassReference(IsReviewed = false)]
    public class TriangularAverageIndicator : FinancialTechnicalIndicator
    {

        #region fields

        IList<double> YValues = new List<double>();

        List<double> xValues;

        List<double> xPoints = new List<double>();

        List<double> yPoints = new List<double>();

        TechnicalIndicatorSegment fastLineSegment;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets the triangular average
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
            DependencyProperty.Register("Period", typeof(int), typeof(TriangularAverageIndicator), new PropertyMetadata(2, OnMovingAverageChanged));

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
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(TriangularAverageIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Green)));


        #endregion

        #region Methods

        private static void OnMovingAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TriangularAverageIndicator indicator = d as TriangularAverageIndicator;
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
            YValues.Clear();
            GeneratePoints(new string[] { Close }, YValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            YValues.Clear();
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
                this.YValues = Clone(series.ActualSeriesYValues[0]);
                this.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Method implementation for GeneratePoints for TechnicalIndicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { Close }, YValues);
        }


        /// <summary>
        /// Creates the segments of TriangularAverageIndicator.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            xValues = GetXValues();
            double average = Period;

            if (average < this.DataCount)
            {
                this.AddTriangularPoints((int)average);
            }

            if (fastLineSegment == null)
            {
                fastLineSegment = new TechnicalIndicatorSegment(xPoints, yPoints, SignalLineColor, this);
                Segments.Add(fastLineSegment);
            }
            else if (ActualXValues != null)
            {
                fastLineSegment.SetData(xPoints, yPoints);
                fastLineSegment.SetRange();
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

        private void AddTriangularPoints(int avg)
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

            ComputeMovingAverage(Period, xValues, YValues, xPoints, yPoints);
            ComputeMovingAverage(len2, xValues, YValues, xPoints, yPoints);

            double pad = yPoints[len1 + len2 - 1];
            for (int i = 0; i < len1 + len2 - 1; ++i)
            {
                yPoints[i] = pad;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new TriangularAverageIndicator() { SignalLineColor = this.SignalLineColor, Period = this.Period };
            return base.CloneSeries(obj);
        }

        #endregion

    }
}
