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
using System.Collections.Generic;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents Chart Kagi type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    public sealed class ChartKagiSegment : FastLineSegment
    {
        #region Depedency Properties
        /// <summary>
        /// Identifies the IsPriceUp dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceUpProperty =
            DependencyProperty.Register("IsPriceUp", typeof(bool), typeof(ChartKagiSegment), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the IsPriceDown dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceDownProperty =
            DependencyProperty.Register("IsPriceDown", typeof(bool), typeof(ChartKagiSegment), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this segment is price up. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this segment is price up; otherwise, <c>false</c>.
        /// </value>
        public bool IsPriceUp
        {
            get { return (bool)GetValue(IsPriceUpProperty); }
            set { SetValue(IsPriceUpProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this segment is price down. This is a dependency property.
        /// </summary>
        /// <value>
        /// <c>true</c> if this segment is price down; otherwise, <c>false</c>.
        /// </value>
        public bool IsPriceDown
        {
            get { return (bool)GetValue(IsPriceDownProperty); }
            set { SetValue(IsPriceDownProperty, value); }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLineBreakSegment"/> class.
        /// </summary>
        /// <param name="points">chart points.</param>
        /// <param name="correspondingPoint">The corresponding points.</param>
        /// <param name="series">The series.</param>
        internal ChartKagiSegment(ChartPointsCollection points, ChartPointsCollection correspondingPoint, ChartSeries series)
            : base(points, correspondingPoint, series)
        {
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
    /// Class implementation for ChartKagiType
    /// </summary>
    public class ChartKagiType : ChartType
    {
        #region Dependecy properties
        /// <summary>
        /// Identifies the ReversalAmount dependency property.
        /// </summary>
        public static readonly DependencyProperty ReversalAmountProperty =
          DependencyProperty.RegisterAttached("ReversalAmount", typeof(double), typeof(ChartKagiType), new PropertyMetadata(1d, new PropertyChangedCallback(OnDataChanged)));
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the reversal amount.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>The Reversal Amount</returns>
        public static double GetReversalAmount(ChartSeries series)
        {
            return (double)series.GetValue(ReversalAmountProperty);
        }

        /// <summary>
        /// Sets the reversal amount.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetReversalAmount(ChartSeries series, double value)
        {
            series.SetValue(ReversalAmountProperty, value);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Kagi";
        }

        #endregion

        #region Implementation
        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            if (series.Area != null)
            {
                series.Area.LoadArea();
            }
        }

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

            double reversalAmount = GetReversalAmount(series);
            ChartPointsCollection correspondingPoints = new ChartPointsCollection();
            ChartPointsCollection points1 = new ChartPointsCollection();
            bool isNegative = true;
            bool isGoingDown = true;
            double prevY = points.Count > 0 ? points[0].Y : 0d;
            double breakMinY = prevY;
            double breakMaxY = prevY;
            double currX = points.Count > 0 ? points[0].X : 0d;
            correspondingPoints.Add(points.Count > 0 ? points[0] : new ChartPoint(0, 0));
            points1.Add(new ChartPoint(currX, prevY));

            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].EmptyPoint && series.ShowEmptyPoints)
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
                }
                correspondingPoints.Add(points[i]);
                double currY = points[i].Y;

                if (i == 1)
                {
                    isNegative = currY < prevY;
                    isGoingDown = currY < prevY;
                }

                if (Math.Abs(currY - prevY) > reversalAmount)
                {
                    if ((isGoingDown && prevY < currY) || (!isGoingDown && prevY > currY))
                    {
                        currX = points[i].X;
                        points1.Add(new ChartPoint(currX, prevY));
                        breakMinY = currY > prevY ? prevY : breakMinY;
                        breakMaxY = currY < prevY ? prevY : breakMaxY;
                        isGoingDown = !isGoingDown;
                    }

                    if ((isNegative && (currY > breakMaxY)) || (!isNegative && (currY < breakMinY)))
                    {
                        ChartPoint icdp = new ChartPoint(currX, isNegative ? breakMaxY : breakMinY);
                        points1.Add(icdp);

                        ////adding CIDs data to lists:
                        ChartKagiSegment segment = new ChartKagiSegment(ConvertToInternalPoints(points1, series), points, series);

                        segment.IsPriceDown = isNegative;
                        segment.IsPriceUp = !isNegative;

                        series.Segments.Add(segment);
                        if (isNegative)
                        {
                            segment.Interior = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            segment.Interior = new SolidColorBrush(Colors.Green);
                        }

                        points1 = new ChartPointsCollection();
                        correspondingPoints = new ChartPointsCollection();
                        points1.Add(new ChartPoint(currX, isNegative ? breakMaxY : breakMinY));
                        correspondingPoints.Add(points[i]);
                        isNegative = !isNegative;
                    }

                    points1.Add(new ChartPoint(currX, currY));
                    prevY = currY;
                }

                if (i == points.Count - 1)
                {
                    ////adding CIDs data to lists:
                    ChartKagiSegment segment = new ChartKagiSegment(ConvertToInternalPoints(points1, series), correspondingPoints, series);

                    segment.IsPriceDown = isNegative;
                    segment.IsPriceUp = !isNegative;
                    series.Segments.Add(segment);
                    if (isNegative)
                    {
                        segment.Interior = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        segment.Interior = new SolidColorBrush(Colors.Green);
                    }
                }               
            }
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
