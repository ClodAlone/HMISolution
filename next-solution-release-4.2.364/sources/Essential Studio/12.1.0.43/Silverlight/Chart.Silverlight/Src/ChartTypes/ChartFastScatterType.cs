#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartFastScatterType
    /// </summary>
    public class ChartFastScatterType : ChartType
    {
        #region member
        ChartPointsCollection fastlinepoints = new ChartPointsCollection();
        #endregion

        #region Dependency Properties

        /// <summary>
        /// Return the double Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetFastScatterHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(FastScatterHeightProperty);
        }

        /// <summary>
        /// Sets the value of the FastScatterHeight dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetFastScatterHeight(DependencyObject obj, double value)
        {
            obj.SetValue(FastScatterHeightProperty, value);
        }

        /// <summary>
        /// Indicates the FastScatterHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty FastScatterHeightProperty =
                DependencyProperty.RegisterAttached("FastScatterHeight", typeof(double), typeof(ChartFastScatterType), new PropertyMetadata(10d, new PropertyChangedCallback(onHeightDataChanged)));


        private static void onHeightDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Return the double Value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetFastScatterWidth(ChartSeries obj)
        {
            return (double)obj.GetValue(FastScatterWidthProperty);
        }

        /// <summary>
        /// Sets the value of the LowValueInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetFastScatterWidth(ChartSeries obj, double value)
        {
            obj.SetValue(FastScatterWidthProperty, value);
        }

        /// <summary>
        /// Indicates the LowValueInterior Dependency Property
        /// </summary>
        public static readonly DependencyProperty FastScatterWidthProperty =
                DependencyProperty.RegisterAttached("FastScatterWidth", typeof(double), typeof(ChartFastScatterType), new PropertyMetadata(10d, new PropertyChangedCallback(OnDataChanged)));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            SetRange(series, points, 1);
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
            {
                points = series.Data;
                series.IsIndexed = false;
            }

            SegmentsCollection segmentsdata = new SegmentsCollection();
            series.sum = (from point in points select point.Y).Sum();
            fastlinepoints.Clear();
            for (int i = 0; i < points.Count; i++)
            {
                if (!double.IsNaN(points[i].Y))
                {
                    double x1 = points[i].X ;
                    double y1 = points[i].Y ;
                    
                        fastlinepoints.Add(new ChartPoint(x1, y1));
                    
                }
                else if (double.IsNaN(points[i].Y) && series.ShowEmptyPoints)
                {
                    if (series.EmptyPointValue == EmptyPointValue.Zero)
                    {
                        points[i].Y = 0;
                    }
                    else
                    {
                        if (i + 1 == points.Count)
                            points[i].Y = points[i - 1].Y / 2;
                        else
                        {
                            int index;
                            for (index = i + 1; index < points.Count; index++)
                                if (!double.IsNaN(points[index].Y))
                                    break;
                            if (i == 0)
                                points[i].Y = (index == points.Count ? 40 : points[index].Y) / 2;
                            else
                                points[i].Y = points[i - 1].Y / 2 + (index == points.Count ? 40 : points[index].Y) / 2;
                        }
                    }
                    double x1 = points[i].X ;
                    double y1 = points[i].Y ;
                   
                        if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            fastlinepoints.Add(new ChartPoint(x1, y1));
                        }
                        else
                        {
                            series.Segments.Add(new ScatterSegment(new ChartPoint(x1, y1),new ChartPoint(x1,y1), points[i], series));
                        }
                   
                }
            }
            
            series.Segments.Add(new FastScatterSegment(fastlinepoints, points, series));
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "FastScatter";
        }

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
            series.Segments.Clear();
            Update(series);
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (fastlinepoints != null)
            {
                for (int temp = 0; temp < fastlinepoints.Count; temp++)
                    fastlinepoints[temp] = null;
            }
        }
    }
}
