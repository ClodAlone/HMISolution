#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartRangeAreaSegments 
    /// </summary>
    public class ChartRangeAreaSegment : AreaSegment
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance is high low.
        /// </summary>
        /// <value>
        ///  <c>true</c> if this instance is high low; otherwise, <c>false</c>.
        /// </value>
        protected bool IsHighLow
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRangeAreaSegment"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        /// <param name="isHighLow">The isHighLow.</param>
        internal ChartRangeAreaSegment(ChartPointsCollection points, ChartPointsCollection correspondingPoints, ChartSeries series, bool isHighLow)
            : base(points, correspondingPoints, series)
        {
            IsHighLow = isHighLow;
            Brush highinterior = ChartRangeAreaType.GetHighValueInterior(series);
            Brush lowinterior = ChartRangeAreaType.GetLowValueInterior(series);
            Brush seriesinterior=new SolidColorBrush();
            Brush[] brush = series.Area.ColorModel.CurrentPalette;
            if (this.series.Interior == null)
            {
                seriesinterior = brush[this.series.Area.Series.IndexOf(series)];
            }
            else
            {
                seriesinterior = series.Interior;
            }
            if (isHighLow)
            {
                this.Interior = (highinterior == null) ? seriesinterior : highinterior;
            }
            else
            {
                this.Interior = (lowinterior == null) ? seriesinterior : lowinterior;
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
    }

    /// <summary>
    /// Class implementation for ChartRangeAreaType
    /// </summary>
    public class ChartRangeAreaType : ChartAreaType
    {
        #region Attached properties

        /// <summary>
        /// Gets the value of the HighValueInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObjectobj.</param>
        /// <returns>The HighValueInterior brush</returns>
        public static Brush GetHighValueInterior(ChartSeries obj)
        {
            return (Brush)obj.GetValue(HighValueInteriorProperty);
        }

        /// <summary>
        /// Sets the value of the HighValueInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetHighValueInterior(ChartSeries obj, Brush value)
        {
            obj.SetValue(HighValueInteriorProperty, value);
        }

        /// <summary>
        /// Indicates the HighValueInterior Dependency Property
        /// </summary>
        public static readonly DependencyProperty HighValueInteriorProperty =
                DependencyProperty.RegisterAttached("HighValueInterior", typeof(Brush), typeof(ChartRangeAreaType), new PropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Gets the value of the LowValueInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <returns>The LowValueInterior Brush</returns>
        public static Brush GetLowValueInterior(ChartSeries obj)
        {
            return (Brush)obj.GetValue(LowValueInteriorProperty);
        }

        /// <summary>
        /// Sets the value of the LowValueInterior dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetLowValueInterior(ChartSeries obj, Brush value)
        {
            obj.SetValue(LowValueInteriorProperty, value);
        }

        /// <summary>
        /// Indicates the LowValueInterior Dependency Property
        /// </summary>
        public static readonly DependencyProperty LowValueInteriorProperty =
                DependencyProperty.RegisterAttached("LowValueInterior", typeof(Brush), typeof(ChartRangeAreaType), new PropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));
        #endregion

        #region Implmentation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            ChartPointsCollection newPoints = new ChartPointsCollection();
            for (int i = 0; i < points.Count; i++)
            {
                if (!double.IsNaN(points[i].Y))
                    newPoints.Add(points[i]);
            }
            SetRange(series, newPoints, 2);

            Point point1;
            Point point2;
            Point point3;
            Point point4;
            Point? crossPoint;
            List<Point> segPoints = new List<Point>();
            List<Point> highPoints = new List<Point>();
            List<Point> lowPoints = new List<Point>();

            highPoints.Add(new Point(newPoints[0].X , newPoints[0].Values[0] ));
            lowPoints.Add(new Point(newPoints[0].X , newPoints[0].Values[1] ));

            bool isHighLow = (highPoints[0].Y > lowPoints[0].Y) ? true : false;
            series.sum = (from point in newPoints where !point.Y.Equals(double.NaN) select point.Y).Sum();
            for (int i = 0; i < newPoints.Count - 1; i++)
            {
                point1 = new Point(newPoints[i].X , newPoints[i].Values[0] );
                point2 = new Point(newPoints[i + 1].X , newPoints[i + 1].Values[0]);
                point3 = new Point(newPoints[i].X , newPoints[i].Values[1] );
                point4 = new Point(newPoints[i + 1].X , newPoints[i + 1].Values[1] );
                crossPoint = ChartRangeAreaType.GetCrossPoint(point1, point2, point3, point4);
                if (crossPoint != null)
                {
                    segPoints.AddRange(highPoints);
                    segPoints.Add(crossPoint.Value);
                    segPoints.AddRange(lowPoints.Reverse<Point>());
                    series.Segments.Add(new ChartRangeAreaSegment(Point2ChartPoint(segPoints), newPoints, series, isHighLow));
                    isHighLow = !isHighLow;
                    highPoints.Clear();
                    lowPoints.Clear();
                    segPoints = new List<Point>();
                    segPoints.Add(crossPoint.Value);
                }

                highPoints.Add(point2);
                lowPoints.Add(point4);
                if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                {
                    series.Adornments.Add(new ChartAdornment(newPoints[i], newPoints, series, 0d));
                }
            }

            segPoints.AddRange(highPoints);
            segPoints.AddRange(lowPoints.Reverse<Point>());
            series.Segments.Add(new ChartRangeAreaSegment(Point2ChartPoint(segPoints), newPoints, series, isHighLow));
            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
            {
                series.Adornments.Add(new ChartAdornment(newPoints[newPoints.Count - 1], newPoints, series, 0d));
            }
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

        /// <summary>
        /// Updates the segments.
        /// </summary>
        /// <param name="series">The Chart Series</param>
        /// <param name="points">The series points</param>
        protected override void UpdateSegments(ChartSeries series, ChartPointsCollection points)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.CalculateSegments(series, points);
        }

        /// <summary>
        /// Point2s the chart point.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>Returns the point array</returns>
        internal ChartPointsCollection Point2ChartPoint(IEnumerable<Point> points)
        {
            ChartPointsCollection cPoints = new ChartPointsCollection();
            foreach (Point pt in points)
            {
                cPoints.Add(new ChartPoint(pt.X, pt.Y));
            }

            return cPoints;
        }

        /// <summary>
        /// Gets the cross point.
        /// </summary>
        /// <param name="p11">The P11 value.</param>
        /// <param name="p12">The P12 value.</param>
        /// <param name="p21">The P21 value.</param>
        /// <param name="p22">The P22 value.</param>
        /// <returns>The CrossPoint</returns>
        protected static Point? GetCrossPoint(Point p11, Point p12, Point p21, Point p22)
        {
            Point pt = new Point();
            double z = ((p12.Y - p11.Y) * (p21.X - p22.X)) - ((p21.Y - p22.Y) * (p12.X - p11.X));
            double ca = ((p12.Y - p11.Y) * (p21.X - p11.X)) - ((p21.Y - p11.Y) * (p12.X - p11.X));
            double cb = ((p21.Y - p11.Y) * (p21.X - p22.X)) - ((p21.Y - p22.Y) * (p21.X - p11.X));

            if ((z == 0) && (ca == 0) && (cb == 0))
            {
                return null;
            }

            double ua = ca / z;
            double ub = cb / z;

            pt.X = p11.X + ((p12.X - p11.X) * ub);
            pt.Y = p11.Y + ((p12.Y - p11.Y) * ub);

            if ((0 <= ua) && (ua <= 1) && (0 <= ub) && (ub <= 1))
            {
                return pt;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts ChartAreaType to string
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return "RangeArea";
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
