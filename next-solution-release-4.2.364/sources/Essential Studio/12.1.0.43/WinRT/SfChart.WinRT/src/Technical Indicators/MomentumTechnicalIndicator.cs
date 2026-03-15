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
    /// Represents Momentum technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    [ClassReference(IsReviewed = false)]
    public class MomentumTechnicalIndicator : FinancialTechnicalIndicator
    {
        #region ctor

        #endregion

        #region fields

        IList<double> closeValues = new List<double>();

        List<double> xValues;

        List<double> momentumXPoints = new List<double>();

        List<double> momentumYPoints = new List<double>();

        List<double> centerXPoints = new List<double>();

        List<double> centerYPoints = new List<double>();

        TechnicalIndicatorSegment momentumLineSegment;

        TechnicalIndicatorSegment centerlLineSegment;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets the momentum time span
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public int Period
        {
            get { return (int)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for MovingAverage.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PeriodProperty =
            DependencyProperty.Register("Period", typeof(int), typeof(MomentumTechnicalIndicator), new PropertyMetadata(14));

        /// <summary>
        /// Gets or sets the momentum line color
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush MomentumLineColor
        {
            get { return (Brush)GetValue(MomentumLineColorProperty); }
            set { SetValue(MomentumLineColorProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for UpperLineColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MomentumLineColorProperty =
            DependencyProperty.Register("MomentumLineColor", typeof(Brush), typeof(MomentumTechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Gets or Sets the center line color
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush CenterLineColor
        {
            get { return (Brush)GetValue(CenterLineColorProperty); }
            set { SetValue(CenterLineColorProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LowerLineColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CenterLineColorProperty =
            DependencyProperty.Register("CenterLineColor", typeof(Brush), typeof(MomentumTechnicalIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        #endregion

        #region Methods

        private static void OnMovingAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MomentumTechnicalIndicator indicator = d as MomentumTechnicalIndicator;
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
            closeValues.Clear();
            GeneratePoints(new string[] { Close }, closeValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            closeValues.Clear();
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
                this.closeValues = Clone(series.ActualSeriesYValues[0]);
                this.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Method implementation for GeneratePoints for TechnicalIndicator
        /// </summary>
        protected internal override void GeneratePoints()
        {
            GeneratePoints(new string[] { Close }, closeValues);
        }

        /// <summary>
        /// Creates the segments of MomentumTechnicalIndicator.
        /// </summary>
        public override void CreateSegments()
        {
            xValues = GetXValues();
            if ((int)Period < DataCount)
            {
                ComputeMomentum(Period);
                centerXPoints.Clear();
                centerYPoints.Clear();
                for (int i = 0; i < DataCount; i++)
                {
                    if (!(i <= Period - 1))
                    {
                        centerXPoints.Add(xValues[i]);
                        centerYPoints.Add(100);
                    }
                }
                if (momentumLineSegment == null || centerlLineSegment == null)
                {
                    Segments.Clear();
                    momentumLineSegment = new TechnicalIndicatorSegment(momentumXPoints, momentumYPoints, MomentumLineColor, this);
                    Segments.Add(momentumLineSegment);
                    centerlLineSegment = new TechnicalIndicatorSegment(centerXPoints, centerYPoints, CenterLineColor, this);
                    Segments.Add(centerlLineSegment);
                }
                else
                {
                    momentumLineSegment.SetData(momentumXPoints, momentumYPoints);
                    momentumLineSegment.SetRange();
                    centerlLineSegment.SetData(centerXPoints, centerYPoints);
                    centerlLineSegment.SetRange();
                }
            }
            if (momentumLineSegment != null && centerlLineSegment != null && DataCount > 0)
            {
                momentumLineSegment.SetRange();
                centerlLineSegment.SetRange();
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
            base.UpdateSegments(index, action);
            this.Area.ScheduleUpdate();
        }

        private void ComputeMomentum(int len)
        {
            momentumXPoints.Clear();
            momentumYPoints.Clear();
            double pad = 0;
            for (int i = 0; i < DataCount; ++i)
            {
                if (!(i < len))
                {
                    momentumXPoints.Add(xValues[i]);
                    momentumYPoints.Add((pad = closeValues[i] - closeValues[i - len + 1]));
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new MomentumTechnicalIndicator() { Period = this.Period, MomentumLineColor = this.MomentumLineColor, CenterLineColor = this.CenterLineColor };
            return base.CloneSeries(obj);
        }

        #endregion

    }
}
