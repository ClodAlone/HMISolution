#region Copyright
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
    /// Represents Line chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class LineSegment : Segment
    {
        #region members
        internal ChartPoint m_bottomLeftPnt;
        internal ChartPoint m_topRightPnt;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
                DependencyProperty.Register("Template", typeof(DataTemplate), typeof(LineSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the X1 depency property.
        /// </summary>
        public static readonly DependencyProperty X1Property =
                DependencyProperty.Register("X1", typeof(double), typeof(LineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y2 depency property.
        /// </summary>
        public static readonly DependencyProperty X2Property =
                DependencyProperty.Register("X2", typeof(double), typeof(LineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y1 depency property.
        /// </summary>
        public static readonly DependencyProperty Y1Property =
                DependencyProperty.Register("Y1", typeof(double), typeof(LineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y2 depency property.
        /// </summary>
        public static readonly DependencyProperty Y2Property =
                DependencyProperty.Register("Y2", typeof(double), typeof(LineSegment), new PropertyMetadata(0d));
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see>
        ///                                       <cref>ChartColumnSegment</cref>
        ///                                   </see>
        ///     class.
        /// </summary>
        /// <param name="bottomLeftPnt">The bottom left PNT.</param>
        /// <param name="topRightPnt">The top right PNT.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal LineSegment(ChartPoint bottomLeftPnt, ChartPoint topRightPnt, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
            if (series.SegmentTemplate == null)
            {
                this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartLineType), ChartTypes.Line);
                series.ActualSegmentTemplate = !series.EnableEffects ? this.Template : ResourceManager.GetSeriesTemplateWithEffects(typeof(ChartLineType), ChartTypes.Line);
                this.SegmentTemplate = series.ActualSegmentTemplate;
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
				this.SegmentTemplate = series.SegmentTemplate;
            }

            m_bottomLeftPnt = bottomLeftPnt;
            m_topRightPnt = topRightPnt;
            this.SetXRange(bottomLeftPnt.X, topRightPnt.X);
            this.SetYRange(bottomLeftPnt.Y, topRightPnt.Y);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set Template property
        /// </summary>
        public DataTemplate Template
        {
            get
            {
                return (DataTemplate)GetValue(TemplateProperty);
            }

            set
            {
                SetValue(TemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the starting X line co-ordinate. This is a depency property.
        /// </summary>
        /// <value>The X1 Dependency Property value.</value>
        public double X1
        {
            get
            {
                return (double)GetValue(X1Property);
            }

            set
            {
                SetValue(X1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the ending X line co-ordinate. This is a depency property.
        /// </summary>
        /// <value>The X2 Dependency Property value.</value>
        public double X2
        {
            get
            {
                return (double)GetValue(X2Property);
            }

            set
            {
                SetValue(X2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the starting Y line co-ordinate. This is a depency property.
        /// </summary>
        /// <value>The Y1 Dependency Property value.</value>
        public double Y1
        {
            get
            {
                return (double)GetValue(Y1Property);
            }

            set
            {
                SetValue(Y1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the ending Y line co-ordinate. This is a depency property.
        /// </summary>
        /// <value>The Y2 Dependency Property value.</value>
        public double Y2
        {
            get
            {
                return (double)GetValue(Y2Property);
            }

            set
            {
                SetValue(Y2Property, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            var bottomLeftPointX = m_bottomLeftPnt.X + (series.XAxis.VisibleRange.Start * (-1));
            var topRightPointX = m_topRightPnt.X + (series.XAxis.VisibleRange.Start * (-1));
            if (bottomLeftPointX >= -1 && bottomLeftPointX <= (series.XAxis.VisibleRange.End - series.XAxis.VisibleRange.Start) + 1)
            {
                base.Update(transformer);
                Point blPoint = transformer.TransformToVisible(m_bottomLeftPnt.X + (series.XAxis.VisibleRange.Start * (-1)), m_bottomLeftPnt.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                Point trPoint = transformer.TransformToVisible(m_topRightPnt.X + (series.XAxis.VisibleRange.Start * (-1)), m_topRightPnt.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                this.X1 = blPoint.X;
                this.X2 = trPoint.X;
                this.Y1 = blPoint.Y;
                this.Y2 = trPoint.Y;
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            m_bottomLeftPnt = null;
            m_topRightPnt = null;
            this.Template = null;
            //this.Template = null;
        }
    }

    /// <summary>
    /// Represents Line chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class ChartLineType : ChartType
    {
        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            SetRange(series, points, 1);
            if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true)
            {
                points = series.Data;
                series.IsIndexed = false;
            }
            for (int i = 0; i < points.Count; i++)
            {
                if (double.IsNaN(points[i].Y))
                {
                    points[i].EmptyPoint = true;
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
            }
            series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
            for (int i = 0; i < points.Count; i++)
            {
                if (!double.IsNaN(points[i].Y) && !points[i].EmptyPoint)
                {
                    double x1 = points[i].X;// + (series.XAxis.VisibleRange.Start * (-1));
                    double y1 = points[i].Y;// +(series.YAxis.VisibleRange.Start * (-1));

                    if (series.Area.Host == Host.OLAPChart)
                    {
                        x1 -= 0.5;
                        //y1 -= 0.5;
                    }

                    if (i == points.Count - 1)
                    {
                        series.Segments.Add(new LineSegment(new ChartPoint(x1, y1), new ChartPoint(x1, y1), points[i], series));
                        continue;
                    }
                    if (double.IsNaN(points[i + 1].Y))
                    {
                        //SD12372- This Line is added because the adornment is not added to segment when next point is emptypoint. 
                        if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                        {                            
                            series.Adornments.Add(new ChartAdornment(series.Area.Host == Host.OLAPChart ? new ChartPoint(points[i].X - 0.5, points[i].Y) : points[i], points, series, 0d));
                        }
                        continue;
                    }

                    double x2 = points[i + 1].X;// +(series.XAxis.VisibleRange.Start * (-1));
                    double y2 = points[i + 1].Y;// +(series.YAxis.VisibleRange.Start * (-1));
                       

                    if (series.Area.Host == Host.OLAPChart)
                    {
                        x2 -= 0.5;
                        //y2 -= 0.5;
                    }

                   
                        if (!points[i + 1].EmptyPoint)
                            series.Segments.Add(new LineSegment(new ChartPoint(x1, y1), new ChartPoint(x2, y2), points[i], series));
                   
                    if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                    {
                        ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                        series.Adornments.Add(new ChartAdornment(series.Area.Host == Host.OLAPChart ? new ChartPoint(points[i].X - 0.5, points[i].Y) : points[i], points, series, 0d));
                    }
                }
                else if (series.ShowEmptyPoints)
                {
                    if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                    {
                        if (i == 0)
                        {
                        }
                        double x1 = points[i].X ;
                        double y1 = points[i].Y ;
                        if (i == points.Count - 1)
                        {
                            series.Segments.Add(new LineSegment(new ChartPoint(x1, y1), new ChartPoint(x1, y1), points[i], series));
                            continue;
                        }
                        if (double.IsNaN(points[i + 1].Y))
                            continue;
                        double x2 = points[i + 1].X ;
                        double y2 = points[i + 1].Y ;

                        double x3 = i - 1 >= 0 ? points[i - 1].X : points[i].X;
                        double y3 = i - 1 >= 0 ? points[i - 1].Y  : points[i].Y;
                            
                            series.Segments.Add(new LineSegment(new ChartPoint(x1, y1), new ChartPoint(x2, y2), points[i], series));
                            series.Segments.Add(new LineSegment(new ChartPoint(x1, y1), new ChartPoint(x3, y3), points[i], series));
                            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                            {
                                ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                                series.Adornments.Add(new ChartAdornment(points[i], points, series, 0d));
                            }
                    }
                    else if (series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior || series.EmptyPointStyle == EmptyPointStyle.Symbol)
                    {
                        double x1 = points[i].X ;
                        double y1 = points[i].Y ;
                        double x2 = 0;
                        double y2 = 0;
                        ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                        ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                       
                            series.Segments.Add(new ScatterSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                            {
                                ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                                series.Adornments.Add(new ChartAdornment(points[i], points, series, 0d));
                            }
                        }
                }
            }
            if (points.Count > 0)
            {
                if (!double.IsNaN(points[points.Count - 1].Y) && !points[points.Count - 1].EmptyPoint)
                {
                    if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                    {
                        ////series.Segments.Add(new ChartAdornment(points[points.Count-1], points, series, 0d));
                        series.Adornments.Add(new ChartAdornment(points[points.Count - 1], points, series, 0d));
                    }
                }
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
            return "Line";
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
