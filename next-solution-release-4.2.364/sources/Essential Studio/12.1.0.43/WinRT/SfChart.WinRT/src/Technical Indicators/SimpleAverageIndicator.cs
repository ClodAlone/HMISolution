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
    /// Represents SimpleAverage technical indicator.
    /// </summary>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    /// <seealso cref="TechnicalIndicatorSegment"/>
    [ClassReference(IsReviewed = false)]
    public class SimpleAverageIndicator:FinancialTechnicalIndicator
    {

        #region constructor

        #endregion

        #region fields

        IList<double> YValues = new List<double>();

        List<double> xValues;

        List<double> xPoints=new List<double>();

        List<double> yPoints=new List<double>();

        TechnicalIndicatorSegment fastLineSegment;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the visible points
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public PointCollection VisiblePoints
        {
            get { return (PointCollection)GetValue(VisiblePointsProperty); }
            set { SetValue(VisiblePointsProperty, value); }
        }

        
       /// <summary>
        /// Using a DependencyProperty as the backing store for VisiblePoints.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty VisiblePointsProperty =
            DependencyProperty.Register("VisiblePoints", typeof(PointCollection), typeof(SimpleAverageIndicator), new PropertyMetadata(null));

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
        /// Using a DependencyProperty as the backing store for MovingAverage.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PeriodProperty =
            DependencyProperty.Register("Period", typeof(int), typeof(SimpleAverageIndicator), new PropertyMetadata(2,OnMovingAverageChanged));

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
            DependencyProperty.Register("SignalLineColor", typeof(Brush), typeof(SimpleAverageIndicator), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));


        #endregion

        #region Methods

        private static void OnMovingAverageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleAverageIndicator indicator = d as SimpleAverageIndicator;
            //indicator.ComputeMovingAverage((double)e.NewValue, indicator.xValues,indicator.YValues,indicator.xPoints,indicator.yPoints);
            //indicator.fastLineSegment.SetData(indicator.xPoints, indicator.yPoints);
            //indicator.fastLineSegment.SetRange();
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
            fastLineSegment = null;
            YValues.Clear();
            GeneratePoints(new string[] { Close }, YValues);
            this.UpdateArea();
        }

        protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
        {
            fastLineSegment = null;
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
        /// Creates the segments of SimpleAverageIndicator.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            xValues = GetXValues();
            ComputeMovingAverage(Period, xValues, YValues, xPoints, yPoints);

            if (fastLineSegment == null)
            {
                TechnicalIndicatorSegment segment = new TechnicalIndicatorSegment(xValues, yPoints, SignalLineColor, this,Period);
                fastLineSegment = segment;
                Segments.Add(segment);
                
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

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            obj = new SimpleAverageIndicator() { SignalLineColor = this.SignalLineColor, Period = this.Period, VisiblePoints = this.VisiblePoints };
            return base.CloneSeries(obj);
        }

        #endregion

    }
}
