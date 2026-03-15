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
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using System.Collections;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// A histogram consists of tabular frequencies, shown as adjacent rectangles, erected over discrete intervals, with an area equal to the frequency of the observations in the <see cref="HistogramSeries.HistogramInterval"/>. 
    /// </summary>
    /// <remarks>    
    /// The height of a rectangle is also equal to the frequency density of the interval.A histogram may also be normalized displaying relative frequencies.
    /// You can also draw a normal distribution curve for the given data points, by enabling the <see cref="HistogramSeries.ShowNormalDistributionCurve"/>
    /// </remarks>
    /// <seealso cref="HistogramSegment"/>
    /// <seealso cref="ColumnSeries"/>
    /// <seealso cref="BarSeries"/>
   [ClassReference(IsReviewed = false)]
    public class HistogramSeries:ChartSeries ,ISupportAxes2D
   {
       #region fields

       private List<double> yValues;


       #endregion

       #region Constants

       /// <summary>
       /// Initializes c_distributionPointsCount
       /// </summary>
       private const int C_distributionPointsCount = 500;

       /// <summary>
       /// Initializes c_sqrtDoublePI
       /// </summary>
       private readonly static double c_sqrtDoublePI = Math.Sqrt(2 * Math.PI);

       #endregion

       #region Properties

       /// <summary>
       /// Gets or Sets the property path to retrieve y data from ItemsSource
       /// </summary>
       [ClassReference(IsReviewed = false)]
       public string YBindingPath
       {
           get { return (string)GetValue(YBindingPathProperty); }
           set { SetValue(YBindingPathProperty, value); }
       }

       
       /// <summary>
       /// Using a DependencyProperty as the backing store for YBindingPath.  This enables animation, styling, binding, etc...
       /// </summary>
       public static readonly DependencyProperty YBindingPathProperty =
           DependencyProperty.Register("YBindingPath", typeof(string), typeof(HistogramSeries), new PropertyMetadata(null, OnYPathChanged));

       private static void OnYPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
       {
           (d as HistogramSeries).OnBindingPathChanged(e);
       }


       /// <summary>
       /// Gets or Sets histogram interval
       /// </summary>
       [ClassReference(IsReviewed = false)]
       public double HistogramInterval
       {
           get { return (double)GetValue(HistogramIntervalProperty); }
           set { SetValue(HistogramIntervalProperty, value); }
       }

       
       /// <summary>
       /// Using a DependencyProperty as the backing store for HistogramInterval.  This enables animation, styling, binding, etc...
       /// </summary>
       public static readonly DependencyProperty HistogramIntervalProperty =
           DependencyProperty.Register("HistogramInterval", typeof(double), typeof(HistogramSeries), new PropertyMetadata(1d));

       /// <summary>
       /// Gets or Sets a value whether to show normal distribution curve.
       /// </summary>
       [ClassReference(IsReviewed = false)]
       public bool ShowNormalDistributionCurve
       {
           get { return (bool)GetValue(ShowNormalDistributionCurveProperty); }
           set { SetValue(ShowNormalDistributionCurveProperty, value); }
       }

      
       /// <summary>
       /// Using a DependencyProperty as the backing store for ShowNormalDistributionCurve.  This enables animation, styling, binding, etc...
       /// </summary>
       public static readonly DependencyProperty ShowNormalDistributionCurveProperty =
           DependencyProperty.Register("ShowNormalDistributionCurve", typeof(bool), typeof(HistogramSeries), new PropertyMetadata(true));

        /// <summary>
        /// Get or Set  XRange property
        /// </summary>
        public DoubleRange XRange { get; internal set; }

        /// <summary>
        /// Get or set YRange property
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
       /// Using a DependencyProperty as the backing store for XAxis.  This enables animation, styling, binding, etc...
       /// </summary>
       public static readonly DependencyProperty XAxisProperty =
           DependencyProperty.Register("XAxis", typeof(ChartAxisBase2D), typeof(HistogramSeries), new PropertyMetadata(null, OnXAxisChanged));

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
           DependencyProperty.Register("YAxis", typeof(RangeAxisBase), typeof(HistogramSeries), new PropertyMetadata(null, OnYAxisChanged));

       ChartAxis ISupportAxes.ActualXAxis
       {
           get { return ActualXAxis; }
       }

       ChartAxis ISupportAxes.ActualYAxis
       {
           get { return ActualYAxis; }
       }

       #endregion

       #region constructor

       /// <summary>
       /// Called when instance created for HistogramSeries
       /// </summary>
       public HistogramSeries()
       {
           yValues = new List<double>();
       }

       #endregion

       #region methods

       private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
       {
           (d as HistogramSeries).OnYAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
       }

       private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
       {
           (d as HistogramSeries).OnXAxisChanged(e.OldValue as ChartAxis, e.NewValue as ChartAxis);
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
               newAxis.Orientation = Orientation.Vertical;
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
           if (Area != null) Area.ScheduleUpdate();
       }
       /// <summary>
       /// Called when XAxis value changed
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
               newAxis.Orientation = Orientation.Horizontal;
               if (!newAxis.RegisteredSeries.Contains(this))
                   newAxis.RegisteredSeries.Add(this);
           }
           if (Area != null) Area.ScheduleUpdate();
       }
       /// <summary>
       /// Called when DataSource property changed
       /// </summary>
       /// <param name="oldValue"></param>
       /// <param name="newValue"></param>
       protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
       {
           yValues.Clear();
           GeneratePoints(new string[] { YBindingPath }, yValues);
           this.UpdateArea();
       }

       protected override void OnBindingPathChanged(DependencyPropertyChangedEventArgs args)
       {
           yValues.Clear();
           base.OnBindingPathChanged(args);
       }

        /// <summary>
        /// method declaration for generatepoints in Chartseries
        /// </summary>
        protected internal override void GeneratePoints()
       {
           GeneratePoints(new string[] { YBindingPath }, yValues);
       }

       /// <summary>
       /// Creates the segments of HistogramSeries.
       /// </summary>
       [ClassReference(IsReviewed = false)]
       public override void CreateSegments()
        {
            List<double> xValues = GetXValues();

            if (xValues != null)
            {
                Segments.Clear();
                List<Point> dataPoints = new List<Point>();

                List<ChartSegment> segmentsCollection = new List<ChartSegment>();

                for (int i=0;i<DataCount;i++)
                {
                    dataPoints.Add(new Point(xValues[i],yValues[i]));
                }  

                Array.Sort(dataPoints.ToArray(), new PointsSortByXComparer());

                double interval = HistogramInterval;
                double start = 0;
                if (dataPoints.Count > 0)
                {
                    start = ChartMath.Round(dataPoints[0].X, interval, false);
                }

                double position = start;
                int intervalsCount = 0;
                List<Point> points = new List<Point>();

                for (int i = 0, ci = dataPoints.Count; i < ci; i++)
                {
                    Point cdpt = dataPoints[i];

                    while (cdpt.X > position + interval)
                    {
                        if (points.Count > 0)
                        {
                            //Point cdpt1 = new Point(position, points.Count);
                            //Point cdpt2 = new Point(position + interval, 0);
                            segmentsCollection.Add(new HistogramSegment(position,points.Count,position+interval,0, this));
                            points.Clear();
                        }
                        position += interval;
                        intervalsCount++;
                    }
                    points.Add(dataPoints[i]);
                }

                if (points.Count > 0)
                {
                    //Point cdpt1 = new Point(position, points.Count);
                    //Point cdpt2 = new Point(position + interval, 0);
                    intervalsCount++;
                    segmentsCollection.Add(new HistogramSegment(position, points.Count, position + interval, 0, this));
                    points.Clear();
                }

                #region Normal Distribution

                if (ShowNormalDistributionCurve)
                {
                    double m, dev;
                    GetHistogramMeanAndDeviation(dataPoints, out m, out dev);

                    PointCollection distributionPoints = new PointCollection();

                    double min = start;
                    double max = start + intervalsCount * interval;
                    double del = (max - min) / (C_distributionPointsCount - 1);

                    for (int i = 0; i < C_distributionPointsCount; i++)
                    {
                        double tx = min + i * del;
                        double ty = NormalDistribution(tx, m, dev) * dataPoints.Count * interval;
                        distributionPoints.Add(new Point(tx, ty));
                    }
                    segmentsCollection.Add(new HistogramDistributionSegment(distributionPoints, this));
                }
                segmentsCollection.Reverse();

                foreach (var item in segmentsCollection)
                {
                    Segments.Add(item);
                }

                #endregion
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

        /// <summary>
        /// Gets the histogram mean and deviation.
        /// </summary>
       /// <param name="points">The cpwi A.</param>
        /// <param name="mean">The mean value.</param>
        /// <param name="standartDeviation">The standart deviation.</param>
        private static void GetHistogramMeanAndDeviation(List<Point>points, out double mean, out double standartDeviation)
        {
            int count = points.Count;
            double sum = 0;

            for (int i = 0; i < count; i++)
            {
                sum += points[i].X;
            }

            mean = sum / count;

            sum = 0;

            for (int i = 0; i < count; i++)
            {
                double dif = points[i].X - mean;
                sum += dif * dif;
            }
            
            standartDeviation = Math.Sqrt(sum / count);
        }

        /// <summary>
        /// Normal Distribution function.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="m">The m value.</param>
        /// <param name="sigma">The sigma value.</param>
        /// <returns>The Normal Distribution</returns>
        private static double NormalDistribution(double x, double m, double sigma)
        {
            return Math.Exp(-(x - m) * (x - m) / (2 * sigma * sigma)) / (sigma * c_sqrtDoublePI);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new HistogramSeries()
            {
                XAxis = this.XAxis,
                YAxis = this.YAxis,
                HistogramInterval = this.HistogramInterval,
                ShowNormalDistributionCurve = this.ShowNormalDistributionCurve,
                YBindingPath = this.YBindingPath
            });
        }

       #endregion
   }
}
