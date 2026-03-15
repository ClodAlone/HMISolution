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
    /// Class implentation for ChartRadarType
    /// </summary>
    public class ChartRadarType : ChartPolarType
    {
        #region Depedency Property
        /// <summary>
        /// Identifies the IsClockWise dependency property.
        /// </summary>
        public new static readonly DependencyProperty IsClockWiseProperty =
                DependencyProperty.RegisterAttached("IsClockWise", typeof(bool), typeof(ChartRadarType), new PropertyMetadata(false, new PropertyChangedCallback(OnDataChanged)));
        /// <summary>
        /// Identifies the IsClosed dependency property.
        /// </summary>
        public new static readonly DependencyProperty IsClosedProperty =
        DependencyProperty.RegisterAttached("IsClosed", typeof(bool), typeof(ChartRadarType), new PropertyMetadata(true, new PropertyChangedCallback(OnDataChanged)));

        /// <summary>
        /// Identifies the DrawType dependency property.
        /// </summary>
        public new static readonly DependencyProperty DrawTypeProperty =
        DependencyProperty.RegisterAttached("DrawType", typeof(ChartPolarDrawType), typeof(ChartRadarType), new PropertyMetadata(ChartPolarDrawType.Line, new PropertyChangedCallback(OnDataChanged)));
        #endregion

        #region Static Methods
        /// <summary>
        /// Gets the IsClosed property value.
        /// </summary>
        /// <param name="area">The Chartarea.</param>
        /// <returns>The IsClosed</returns>
        public new static bool GetIsClosed(ChartArea area)
        {
            return (bool)area.GetValue(IsClosedProperty);
        }

        /// <summary>
        /// Sets the IsClosed property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <param name="value">The value.</param>
        public new static void SetIsClosed(ChartArea area, bool value)
        {
            area.SetValue(IsClosedProperty, value);
        }

        /// <summary>
        /// Gets the DrawType property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <returns>The Drawtype</returns>
        public new static ChartPolarDrawType GetDrawType(ChartArea area)
        {
            return (ChartPolarDrawType)area.GetValue(DrawTypeProperty);
        }

        /// <summary>
        /// Sets the DrawType property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <param name="value">The value.</param>
        public new static void SetDrawType(ChartArea area, ChartPolarDrawType value)
        {
            area.SetValue(DrawTypeProperty, value);
        }

        /// <summary>
        /// Gets the IsClockWise property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <returns>The SplineCoefficient</returns>
        public new static bool GetIsClockWise(ChartArea area)
        {
            return (bool)area.GetValue(IsClockWiseProperty);
        }

        /// <summary>
        /// Sets the IsClockWise property value.
        /// </summary>
        /// <param name="area">The ChartArea.</param>
        /// <param name="value">The value.</param>
        public new static void SetIsClockWise(ChartArea area, bool value)
        {
            area.SetValue(IsClockWiseProperty, value);
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null)
            {
                area.LoadArea();
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
            ChartPolarDrawType drawtype = ChartRadarType.GetDrawType(series.Area);
            bool isclose = ChartRadarType.GetIsClosed(series.Area);
            SetRange(series, points, 1);
            ChartPointsCollection linepoints = new ChartPointsCollection();
            SegmentsCollection segmentsdata = new SegmentsCollection();
            if (points.Count != 0)
            {
                double[] datapoints = (from point in points where point.Visible == true select point.Y).ToArray<double>();
                if (datapoints.Length != 0d)
                {
                    series.sum = datapoints.Sum();
                }
            }

            linepoints.Clear();
            for (int i = 0; i < points.Count; i++)
            {
                if (double.IsNaN(points[i].Y))
                    continue;
                double x1 = points[i].X + (series.XAxis.VisibleRange.Start * (-1));
                double y1 = points[i].Y + (series.YAxis.VisibleRange.Start * (-1));
                if (drawtype == ChartPolarDrawType.Symbol)
                {
                    PolarSegment pol = new PolarSegment(null, new ChartPoint(x1, y1), points, series, false);
                    series.Segments.Add(pol);
                }

                linepoints.Add(new ChartPoint(x1, y1));
                if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible == true)
                {
                    segmentsdata.Add(new ChartAdornment(points[i], new ChartPoint(x1, y1), points, series, 0d));
                }
            }

            if (isclose && linepoints.Count>0 )
            {
                linepoints.Add(new ChartPoint(linepoints[0].X, linepoints[0].Y));
            }
            else if (!isclose && linepoints.Count > 0 && drawtype == ChartPolarDrawType.Area)
            {
                linepoints.Add(new ChartPoint(0, 0));
            }

            if (drawtype != ChartPolarDrawType.Symbol)
            {
                PolarSegment fastseg = new PolarSegment(linepoints, new ChartPoint(0, 0), points, series, true);
                series.Segments.Add(fastseg);
                if (drawtype == ChartPolarDrawType.Line)
                {
                    fastseg.SegInterior = fastseg.Interior;
                    fastseg.FillColor = new SolidColorBrush(Colors.Transparent);
                    
                }
                else
                {
                    fastseg.SegInterior = series.Stroke;
                    fastseg.FillColor = fastseg.Interior;
                }
            }

            foreach (Segment seg in segmentsdata)
            {
                series.Adornments.Add(seg);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Radar";
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
        }
    }
}
