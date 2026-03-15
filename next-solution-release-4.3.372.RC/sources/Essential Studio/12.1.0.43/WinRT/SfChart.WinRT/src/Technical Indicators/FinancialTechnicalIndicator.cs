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
using System.Collections;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Base class for all the Financial technical indicators available in <see cref="SfChart"/> control.
    /// </summary>
    /// <seealso cref="AccumulationDistributionIndicator"/>
    /// <seealso cref="AverageTrueRangeIndicator"/>
    /// <seealso cref="BollingerBandIndicator"/>
    /// <seealso cref="ExponentialAverageIndicator"/>
    /// <seealso cref="MACDTechnicalIndicator"/>
    /// <seealso cref="MomentumTechnicalIndicator"/>
    /// <seealso cref="RSITechnicalIndicator"/>
    /// <seealso cref="SimpleAverageIndicator"/>
    /// <seealso cref="StochasticTechnicalIndicator"/>
    /// <seealso cref="TriangularAverageIndicator"/>
    [ClassReference(IsReviewed = false)]
    public abstract class FinancialTechnicalIndicator:ChartSeries, ISupportAxes2D
    {
        /// <summary>
        /// Get XRange property
        /// </summary>
        public DoubleRange XRange { get; internal set; }

        /// <summary>
        /// Get YRange property
        /// </summary>
        public DoubleRange YRange { get; internal set; }

        /// <summary>
        /// Get or Set XAxis property
        /// </summary>
        public ChartAxisBase2D XAxis
        {
            get { return (ChartAxisBase2D)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for XAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
            DependencyProperty.Register("XAxis", typeof(ChartAxisBase2D), typeof(FinancialTechnicalIndicator), new PropertyMetadata(null, OnXAxisChanged));

        /// <summary>
        /// Get or Set YAxis property
        /// </summary>
        public RangeAxisBase YAxis
        {
            get { return (RangeAxisBase)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for YAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
            DependencyProperty.Register("YAxis", typeof(RangeAxisBase), typeof(FinancialTechnicalIndicator), new PropertyMetadata(null, OnYAxisChanged));

        ChartAxis ISupportAxes.ActualXAxis
        {
            get { return ActualXAxis; }
        }

        ChartAxis ISupportAxes.ActualYAxis
        {
            get { return ActualYAxis; }
        }

        /// <summary>
        /// Gets or Sets Orientation for Chart Series 
        /// </summary>
        public bool IsTransposed
        {
            get { return (bool)GetValue(IsTransposedProperty); }
            set { SetValue(IsTransposedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  
        public static readonly DependencyProperty IsTransposedProperty =
            DependencyProperty.Register("IsTransposed", typeof(bool), typeof(FinancialTechnicalIndicator), new PropertyMetadata(false, OnTransposeChanged));

        private static void OnTransposeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FinancialTechnicalIndicator).OnTransposeChanged(Convert.ToBoolean(e.NewValue));
        }

        

        /// <summary>
        /// Gets or Sets the name of the series that this indicator is associated with.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string SeriesName
        {
            get { return (string)GetValue(SeriesNameProperty); }
            set { SetValue(SeriesNameProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for AxisName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SeriesNameProperty =
            DependencyProperty.Register("SeriesName", typeof(string), typeof(FinancialTechnicalIndicator), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or Sets the property path to retrieve high value from ItemsSource.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string High
        {
            get { return (string)GetValue(HighProperty); }
            set { SetValue(HighProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for High.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HighProperty =
            DependencyProperty.Register("High", typeof(string), typeof(FinancialTechnicalIndicator), new PropertyMetadata(null,OnYPathChanged));

        private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FinancialTechnicalIndicator).OnBindingPathChanged(e);
        }
                
        /// <summary>
        /// Gets or Sets the property path to retrieve low value from ItemsSource.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string Low
        {
            get { return (string)GetValue(LowProperty); }
            set { SetValue(LowProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Low.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LowProperty =
            DependencyProperty.Register("Low", typeof(string), typeof(FinancialTechnicalIndicator), new PropertyMetadata(null,OnYPathChanged));


        /// <summary>
        /// Gets or Sets the property path to retrieve open value from ItemsSource.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string Open
        {
            get { return (string)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Open.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OpenProperty =
            DependencyProperty.Register("Open", typeof(string), typeof(FinancialTechnicalIndicator), new PropertyMetadata(null,OnYPathChanged));


        /// <summary>
        /// Gets or Sets the property path to retrieve close value from ItemsSource
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string Close
        {
            get { return (string)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Close.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("Close", typeof(string), typeof(FinancialTechnicalIndicator), new PropertyMetadata(null, OnYPathChanged));

        /// <summary>
        /// Gets or Sets the property path to retrieve volume data from ItemsSource.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string Volume
        {
            get { return (string)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Close.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(string), typeof(FinancialTechnicalIndicator), new PropertyMetadata(null,OnYPathChanged));


        /// <summary>
        /// Creates the segments of financial technical indictaors.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
        }

        private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var financialTechnicalIndicator = d as FinancialTechnicalIndicator;
            if (financialTechnicalIndicator != null)
                financialTechnicalIndicator.OnYAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var financialTechnicalIndicator = d as FinancialTechnicalIndicator;
            if (financialTechnicalIndicator != null)
                financialTechnicalIndicator.OnXAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
        }

        /// <summary>
        /// Called when [data source changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            ActualXValues = null;
            base.OnDataSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when YAxis property changed
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnYAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (newAxis != null && !newAxis.RegisteredSeries.Contains(this))
            {
                if (Area != null && !Area.Axes.Contains(newAxis))
                    Area.Axes.Add(newAxis);
                newAxis.Area = Area;
                newAxis.RegisteredSeries.Add(this);
            }

            if (oldAxis != null && oldAxis.RegisteredSeries != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                {
                    oldAxis.RegisteredSeries.Remove(this);
                }

                if (Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis) && Area.InternalPrimaryAxis != oldAxis && Area.InternalSecondaryAxis != oldAxis)
                        Area.Axes.Remove(oldAxis);
                }
            }
            if (newAxis != null)
                newAxis.Orientation = IsActualTransposed ? Orientation.Horizontal : Orientation.Vertical;
            if (Area != null) Area.ScheduleUpdate();
        }

        /// <summary>
        /// Called when XAxis property changed
        /// </summary>
        /// <param name="oldAxis"></param>
        /// <param name="newAxis"></param>
        protected virtual void OnXAxisChanged(ChartAxis oldAxis, ChartAxis newAxis)
        {
            if (oldAxis != null && oldAxis.RegisteredSeries != null)
            {
                if (oldAxis.RegisteredSeries.Contains(this))
                    oldAxis.RegisteredSeries.Remove(this);

                if (Area != null && oldAxis.RegisteredSeries.Count == 0)
                {
                    if (Area.Axes.Contains(oldAxis) && Area.InternalPrimaryAxis != oldAxis && Area.InternalSecondaryAxis != oldAxis)
                        Area.Axes.Remove(oldAxis);
                }
            }

            if (newAxis != null)
            {
                if (Area != null && !Area.Axes.Contains(newAxis))
                    Area.Axes.Add(newAxis);
                newAxis.Area = Area;
                if (!newAxis.RegisteredSeries.Contains(this))
                    newAxis.RegisteredSeries.Add(this);
            }
            if (newAxis != null)
                newAxis.Orientation = IsActualTransposed ? Orientation.Vertical : Orientation.Horizontal;
            if (Area != null) Area.ScheduleUpdate();
        }

        /// <summary>
        /// Updates the segment at the specified index
        /// </summary>
        /// <param name="index">The index of the segment.</param>
        /// <param name="action">The action that caused the segments collection changed event</param>
        [ClassReference(IsReviewed = false)]
        public override void UpdateSegments(int index, NotifyCollectionChangedAction action)
        {
         
        }
        /// <summary>
        /// Method implementation for Set ItemSource to Series
        /// </summary>
        /// <param name="series"></param>
        protected internal virtual void SetSeriesItemSource(ChartSeriesBase series)
        { 
          
        }
      /// <summary>
      /// Method implementation for GeneratePoints for TechnicalIndicator
      /// </summary>
        protected internal override void GeneratePoints()
        {
            throw new NotImplementedException();
        }

        internal void ComputeMovingAverage(int len, List<double> xValues, IList<double> yValues, List<double> xPoints, List<double> yPoints)
        {
            xPoints.Clear();
            yPoints.Clear();
            double sum = 0d;
            double pad = yValues[len - 1];
            double padDate = xValues[len - 1];
            int limit = xValues.Count;
            for (int i = 0; i < limit; ++i)
            {
                xPoints.Add(0);
                yPoints.Add(0);
                if (i >= len - 1 && i < limit)
                {
                    if (i - len >= 0)
                    {
                        sum += yValues[i] - yValues[i - len];
                    }
                    else
                    {
                        sum += yValues[i];
                    }
                    xPoints[i] = xValues[i];
                    yPoints[i] = sum / len;
                }
                else
                {
                    if (i < len - 1)
                    {
                        sum += yValues[i];
                    }
                    xPoints[i] = pad;
                    yPoints[i] = padDate;
                }
            }
        }

        internal void ComputeMovingAverage(double len, List<double> xValues, IList<double> yValues, List<double> xPoints, List<double> yPoints)
        {
            xPoints.Clear();
            yPoints.Clear();
            double sum = 0d;
            int limit = xValues.Count();
            var length = (int)len;

            for (int i = 0; i < limit; ++i)
            {
                xPoints.Add(0);
                yPoints.Add(0);
                if (i >= length - 1 && i < limit)
                {
                    if (i - len >= 0)
                    {
                        sum += yValues[i] - yValues[i - length];
                    }
                    else
                    {
                        sum += yValues[i];
                    }
                    xPoints[i] = (xValues[i]);
                    yPoints[i] = (sum / len);

                }
                else
                {
                    if (i < len - 1)
                        sum += yValues[i];
                }
            }
        }

        internal override void UpdateRange()
        {
            XRange = DoubleRange.Empty;
            YRange = DoubleRange.Empty;

            foreach (ChartSegment segment in Segments)
            {
                XRange += segment.XRange;
                YRange += segment.YRange;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            FinancialTechnicalIndicator indicator = obj as FinancialTechnicalIndicator;
            if (this.XAxis != Area.InternalPrimaryAxis)
                indicator.XAxis = (ChartAxisBase2D)this.XAxis.Clone();
            if (this.YAxis != Area.InternalSecondaryAxis)
                indicator.YAxis = (RangeAxisBase)this.YAxis.Clone();
            indicator.High = this.High;
            indicator.Low = this.Low;
            indicator.Open = this.Open;
            indicator.Close = this.Close;
            indicator.Volume = this.Volume;
            return base.CloneSeries(indicator);
        }

    }
}
