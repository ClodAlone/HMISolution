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
    /// Represents Line break chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by Silverlight Chart building system.</remarks>
    public sealed class ChartLineBreakSegment : ColumnSegment
    {
        #region Depedency Properties
        /// <summary>
        /// Identifies the IsPriceUp dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceUpProperty =
            DependencyProperty.Register("IsPriceUp", typeof(bool), typeof(ChartLineBreakSegment), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the IsPriceDown dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPriceDownProperty =
            DependencyProperty.Register("IsPriceDown", typeof(bool), typeof(ChartLineBreakSegment), new PropertyMetadata(false));

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

        #region member
        /// <summary>
        /// Initializes m_bottomLeftPoint
        /// </summary>
        private ChartPoint m_bottomLeftPoint;

        /// <summary>
        /// Initializes m_topRightPoint
        /// </summary>
        private ChartPoint m_topRightPoint;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLineBreakSegment"/> class.
        /// </summary>
        /// <param name="bottomLeftPoint">The bottom left point.</param>
        /// <param name="topRightPoint">The top right point.</param>
        /// <param name="correspondingPoints">The corresponding points.</param>
        /// <param name="series">The series.</param>
        internal ChartLineBreakSegment(ChartPoint bottomLeftPoint, ChartPoint topRightPoint, ChartPointsCollection correspondingPoints, ChartSeries series)
            : base(bottomLeftPoint, topRightPoint, correspondingPoints[0], series)
        {
            m_bottomLeftPoint = bottomLeftPoint;
            m_topRightPoint = topRightPoint;
        }
        #endregion

        #region Implmentation
        /// <summary>
        /// Updates the real coordinates of segment with respect to chart type.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            Point blPoint = transformer.TransformToVisible(m_bottomLeftPoint.X, m_bottomLeftPoint.Y, series);
            Point trPoint = transformer.TransformToVisible(m_topRightPoint.X, m_topRightPoint.Y, series);
            Rect columnRect = new Rect(blPoint, trPoint);

            this.X = columnRect.X;
            this.Y = columnRect.Y;
            this.Width = columnRect.Width;
            this.Height = columnRect.Height;
        }
        #endregion

         /// <summary>
         /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
         /// </summary>
         public override void Dispose()
        {
            base.Dispose();
            this.m_bottomLeftPoint = null;
            this.m_topRightPoint = null;
        }

         /// <summary>
         /// Allows an object to try to free resources and perform other cleanup operations before the <see cref="T:System.Object"/> is reclaimed by garbage collection.
         /// </summary>
         ~ChartLineBreakSegment()
        {
        }
    }
    /// <summary>
    /// Class implementation for ChartThreeLineBreaktype
    /// </summary>
    public class ChartThreeLineBreakType : ChartType
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the BreakLineCount dependency property.
        /// </summary>
        public static readonly DependencyProperty BreakLineCountProperty =
      DependencyProperty.RegisterAttached("BreakLineCount", typeof(int), typeof(ChartThreeLineBreakType), new PropertyMetadata(3, new PropertyChangedCallback(OnDataChanged)));
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the break line count.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>the BreakLineCount</returns>
        public static int GetBreakLineCount(ChartSeries series)
        {
            return (int)series.GetValue(BreakLineCountProperty);
        }

        /// <summary>
        /// Sets the break line count.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="value">The value.</param>
        public static void SetBreakLineCount(ChartSeries series, int value)
        {
            series.SetValue(BreakLineCountProperty, value);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "ThreeLineBreak";
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

            int breakLineCount = GetBreakLineCount(series) <= 0 ? 3 : GetBreakLineCount(series);
            List<LineBreakInformation> infoList = new List<LineBreakInformation>();

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
                if (double.IsNaN(points[i].Y))
                    continue;
                double prevX = points[i - 1].X;
                double nextX = points[i].X;
                if (infoList.Count == 0)
                {
                    bool isNegative = points[i - 1].Y > points[i].Y;

                    ChartPoint ltPoint = new ChartPoint(prevX, points[i - 1].Y);
                    ChartPoint rbPoint = new ChartPoint(nextX, points[i].Y);

                    infoList.Add(new LineBreakInformation(ltPoint, rbPoint, isNegative, new ChartPointsCollection() { points[i - 1], points[i] }));
                }
                else
                {
                    bool isBreakLine = false;
                    LineBreakInformation rCIDInfo = infoList[infoList.Count - 1];
                    LineBreakInformation lCIDInfo = null;

                    if (infoList.Count >= breakLineCount)
                    {
                        lCIDInfo = infoList[infoList.Count - breakLineCount];

                        for (int j = 1; j < breakLineCount; j++)
                        {
                            LineBreakInformation cCIDInfo = infoList[infoList.Count - j];
                            LineBreakInformation nCIDInfo = infoList[infoList.Count - j - 1];

                            if (cCIDInfo.IsNegative == nCIDInfo.IsNegative)
                            {
                                isBreakLine = true;
                            }
                            else
                            {
                                isBreakLine = false;
                                break;
                            }
                        }
                    }

                    if ((isBreakLine && (points[i].Y < Math.Min(lCIDInfo.Low, rCIDInfo.Low)))
                      || ((!isBreakLine) && (points[i].Y < rCIDInfo.Low)))
                    {
                        ChartPointsCollection cidps = new ChartPointsCollection { rCIDInfo.CorrespondingPoints[rCIDInfo.CorrespondingPoints.Count - 1], points[i] };
                        ChartPoint tlPoint = new ChartPoint(prevX, rCIDInfo.Low);
                        ChartPoint brPoint = new ChartPoint(nextX, points[i].Y);

                        infoList.Add(new LineBreakInformation(tlPoint, brPoint, true, cidps));
                    }
                    else if ((isBreakLine && (points[i].Y > Math.Max(lCIDInfo.High, rCIDInfo.High)))
                      || ((!isBreakLine) && (points[i].Y > rCIDInfo.High)))
                    {
                        ChartPointsCollection cidps = new ChartPointsCollection { rCIDInfo.CorrespondingPoints[rCIDInfo.CorrespondingPoints.Count - 1], points[i] };
                        ChartPoint tlPoint = new ChartPoint(prevX, rCIDInfo.High);
                        ChartPoint brPoint = new ChartPoint(nextX, points[i].Y);

                        infoList.Add(new LineBreakInformation(tlPoint, brPoint, false, cidps));
                    }
                    else
                    {
                        ChartPointsCollection cidps = rCIDInfo.CorrespondingPoints;
                        ChartPoint tlPoint = rCIDInfo.LeftTopPoint;
                        ChartPoint rbPoint = new ChartPoint(nextX, rCIDInfo.IsNegative ? Math.Min(points[i].Y, rCIDInfo.Low) : Math.Max(points[i].Y, rCIDInfo.High));

                        cidps.Add(points[i]);
                        infoList[infoList.Count - 1] = new LineBreakInformation(tlPoint, rbPoint, rCIDInfo.IsNegative, cidps);
                    }
                }
            }

            List<Segment> drawingList = new List<Segment>();

            for (int i = 0; i < infoList.Count; i++)
            {
                LineBreakInformation info = infoList[i];
                ChartPoint ltpoint = new ChartPoint(info.LeftTopPoint.X + (series.XAxis.VisibleRange.Start * (-1)), info.LeftTopPoint.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartPoint rbpoint = new ChartPoint(info.RightBottomPoint.X + (series.XAxis.VisibleRange.Start * (-1)), info.RightBottomPoint.Y + (series.YAxis.VisibleRange.Start * (-1)));
                ChartLineBreakSegment segment = new ChartLineBreakSegment(ltpoint, rbpoint, info.CorrespondingPoints, series);

                segment.IsPriceUp = !info.IsNegative;
                segment.IsPriceDown = info.IsNegative;
                drawingList.Add(segment);
                if (info.IsNegative)
                {
                    segment.Interior = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    segment.Interior = new SolidColorBrush(Colors.Green);
                }

                series.Segments.Add(segment);
            }
        }

        /// <summary>
        /// Updates the specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        public override void Update(ChartSeries series)
        {
            series.Segments.Clear();
            series.Adornments.Clear();
            this.Calculate(series);
        }
        #endregion

        #region Internal types
        /// <summary>
        /// Represents LineBreakInformation
        /// </summary>
        internal class LineBreakInformation
        {
            /// <summary>
            /// Declares LeftTopPoint
            /// </summary>
            public ChartPoint LeftTopPoint;

            /// <summary>
            /// Declares RightBottomPoint
            /// </summary>
            public ChartPoint RightBottomPoint;

            /// <summary>
            /// Declares IsNegative
            /// </summary>
            public bool IsNegative;

            /// <summary>
            /// Declares CorrespondingPoints
            /// </summary>
            public ChartPointsCollection CorrespondingPoints;

            /// <summary>
            /// Gets the low.
            /// </summary>
            /// <value>The low value.</value>
            public double Low
            {
                get
                {
                    return Math.Min(LeftTopPoint.Y, RightBottomPoint.Y);
                }
            }

            /// <summary>
            /// Gets the high.
            /// </summary>
            /// <value>The high value.</value>
            public double High
            {
                get
                {
                    return Math.Max(LeftTopPoint.Y, RightBottomPoint.Y);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="LineBreakInformation"/> class.
            /// </summary>
            /// <param name="ltPoint">The lt point.</param>
            /// <param name="rbPoint">The rb point.</param>
            /// <param name="negative">if set to <c>true</c> [negative].</param>
            /// <param name="correspondingPoints">The corresponding points.</param>
            public LineBreakInformation(ChartPoint ltPoint, ChartPoint rbPoint, bool negative, ChartPointsCollection correspondingPoints)
            {
                LeftTopPoint = ltPoint;
                RightBottomPoint = rbPoint;
                IsNegative = negative;
                CorrespondingPoints = correspondingPoints;
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
}
