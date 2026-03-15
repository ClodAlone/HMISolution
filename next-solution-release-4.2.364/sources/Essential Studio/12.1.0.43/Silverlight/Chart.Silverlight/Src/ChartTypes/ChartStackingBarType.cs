#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart
{
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

    /// <summary>
    /// Represents Stacking Bar chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class StackingBarSegment : Segment
    {
        #region Dependency properties

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for SumWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SumWidthProperty =
            DependencyProperty.Register("SumWidth", typeof(double), typeof(StackingBarSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Height depency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(StackingBarSegment), new PropertyMetadata(0d));

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(StackingBarSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Width depency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(StackingBarSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the X depency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(StackingBarSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y depency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(StackingBarSegment), new PropertyMetadata(0d));
        #endregion

        #region Members
        private ChartPoint m_bottomLeftPoint;
        private ChartPoint m_topRightPoint;
        #endregion

        #region Constructor

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
        internal StackingBarSegment(ChartPoint bottomLeftPnt, ChartPoint topRightPnt, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartStackingBarType), ChartTypes.StackingBar);
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = !series.EnableEffects ? this.Template : ResourceManager.GetSeriesTemplateWithEffects(typeof(ChartStackingBarType), ChartTypes.StackingBar);
                this.SegmentTemplate = series.ActualSegmentTemplate;
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
                this.SegmentTemplate = series.SegmentTemplate;
            }

            m_bottomLeftPoint = bottomLeftPnt;
            m_topRightPoint = topRightPnt;
            this.SetXRange(bottomLeftPnt.X, topRightPnt.X);
            this.SetYRange(bottomLeftPnt.Y, topRightPnt.Y);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the height of segment. This is a depency property.
        /// </summary>
        /// <value>The height value.</value>
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }

            set
            {
                SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// Get or Set TemplateProperty
        /// </summary>
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of segment. This is a depency property.
        /// </summary>
        /// <value>The width value.</value>
        public double Width
        {
            get
            {
                return (double)GetValue(WidthProperty);
            }

            set
            {
                SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the X co-ordinate of segment. This is a depency property.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get
            {
                return (double)GetValue(XProperty);
            }

            set
            {
                SetValue(XProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y co-ordinate of segment. This is a depency property.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            get
            {
                return (double)GetValue(YProperty);
            }

            set
            {
                SetValue(YProperty, value);
            }
        }



        private double SumWidth
        {
            get { return (double)GetValue(SumWidthProperty); }
            set { SetValue(SumWidthProperty, value); }
        }

       
        
        #endregion

        #region Implementation
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);
            Point blPoint = transformer.TransformToVisible(m_bottomLeftPoint.X, m_bottomLeftPoint.Y, series);
            Point trPoint = transformer.TransformToVisible(m_topRightPoint.X, m_topRightPoint.Y, series);
            Rect columnRect = new Rect(blPoint, trPoint);
            this.X = columnRect.X;// +this.series.YAxis.LineStrokeThickness;
            this.Y = columnRect.Y;
            this.Width = columnRect.Width;
            this.Height = columnRect.Height;
            
            ////For the Improve the Look and Feel for Chart Series
            //int seriesindex = series.Area.Series.IndexOf(series);
            //int segmentindex = series.Segments.IndexOf(this);
            this.SumWidth = X + Width;
            //if (seriesindex >= 1)
            //{
            //    for (int i = seriesindex; i >= 0; i--)
            //    {
            //        if (series.Area.Series[i].Type != ChartTypes.StackingBar)
            //        {
            //            continue;
            //        }

            //       this.SumWidth += (segmentindex < series.Area.Series[i].Segments.Count) ? ((StackingBarSegment)series.Area.Series[i].Segments[segmentindex]).Width : 0;
            //    }
            //}
            //else
            //{
            //    this.SumWidth =  this.Width;
            //}
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
    }

    /// <summary>
    /// Represents Stacking Bar chart type.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class ChartStackingBarType : ChartType
    {
        #region PrivateMembers

        // Hold the Latest stack data to maintain the visible range of all series, while creating segments
        private ChartPointsCollection tempStackData = new ChartPointsCollection();

        #endregion

        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            int k = 0;
            ChartPointsCollection sum = new ChartPointsCollection();
            ChartPointsCollection actualdata = new ChartPointsCollection();
            sum.Clear();
            foreach (ChartSeries series1 in series.Area.Series)
            {
                int c = -1;
                ChartPointsCollection actualpoint = new ChartPointsCollection();
                foreach (ChartPoint point in series1.Data)
                {
                    if (point.EmptyPoint && series.ShowEmptyPoints)
                    {
                        c++;
                        if (series.EmptyPointValue == EmptyPointValue.Zero)
                        {
                            point.Y = 0;
                        }
                        else
                        {
                            if (c + 1 == points.Count)
                                point.Y = points[c - 1].Y / 2;
                            else
                            {
                                int index;
                                for (index = c + 1; index < points.Count; index++)
                                    if (!double.IsNaN(points[index].Y))
                                        break;
                                if (index == 0)
                                    point.Y = (index == points.Count ? 40 : points[index].Y) / 2;
                                else
                                    point.Y = points[index - 1].Y / 2 + (index == points.Count ? 40 : points[index].Y) / 2;
                            }
                        }
                    }
                    actualpoint.Add(new ChartPoint(point.X, point.Y));
                }

                if (series1.Visibility == Visibility.Visible && series1.Type == ChartTypes.StackingBar && series1.Data != null)
                {
                    if (series.IsIndexed == true)
                    {
                        actualdata = series1.ConvertToActualData(actualpoint);
                    }
                    else
                    {
                        actualdata = actualpoint;
                    }

                    if (k == 0)
                    {
                        k = 1;
                        series1.StackedData.Clear();
                        sum.Clear();
                        foreach (ChartPoint cp in actualdata)
                        {
                            series1.StackedData.Add(new ChartPoint(cp.X, cp.Y));
                            sum.Add(new ChartPoint(cp.X, cp.Y));
                        }
                    }
                    else
                    {
                        for (int j = 0; j < actualdata.Count; j++)
                        {
                            bool status = false;
                            for (int i = 0; i < sum.Count; i++)
                            {
                                if (sum[i].X == actualdata[j].X)
                                {
                                    status = true;
                                    sum[i].Y += actualdata[j].Y;
                                    break;
                                }
                            }

                            if (!status)
                            {
                                sum.Add(new ChartPoint(actualdata[j].X, actualdata[j].Y));
                            }
                        }

                        series1.StackedData.Clear();
                        foreach (ChartPoint cp in sum)
                        {
                            series1.StackedData.Add(new ChartPoint(cp.X, cp.Y));
                        }                        
                    }
                }               
            }
            double max = 0;
            foreach (ChartSeries s in series.Area.Series)
            {
                var v = from st in s.StackedData
                        select st.Y;
                if (v.Count() > 0)
                {
                    if (v.Max() > max || v.Max() < max)
                    {
                        max = v.Max();
                        this.tempStackData = s.StackedData;
                    }
                }
            }

            if (series.Area != null)
            {
                ChartPointsCollection actualpoint1 = new ChartPointsCollection();
                foreach (ChartPoint point in points)
                {
                    actualpoint1.Add(new ChartPoint(point.X, point.Y));
                }
                SetRange(series, this.tempStackData, 1);
                series.Area.PrimaryAxis.Orientation = Orientation.Vertical;
                series.Area.SecondaryAxis.Orientation = Orientation.Horizontal;
                DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
                double sbsCenter = 0.4 * (series.XAxis.VisibleInterval < 1 && series.Area.MinDataInterval < 1d ? series.Area.MinDataInterval : 1d);
                series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
                for (int i = 0; i < actualpoint1.Count; i++)
                {
                    double x1 = 0;
                    double y1 = actualpoint1[i].X + sbsInfo.Start - sbsCenter + (series.XAxis.VisibleRange.Start * (-1));
                    double y2 = actualpoint1[i].X + sbsInfo.End - sbsCenter + (series.XAxis.VisibleRange.Start * (-1));
                    double value = series.Area.Series.IndexOf(series) > 0 ? series.XAxis.Origin : 0;
                    for (int j = 0; j < series.StackedData.Count; j++)
                    {
                        if (series.StackedData[j].X == actualpoint1[i].X && series.StackedData[j].Y < 0 && actualpoint1[i].Y > 0)
                        {
                            x1 = 0;
                            break;
                        }
                        if (series.StackedData[j].X == actualpoint1[i].X)
                        {
                            x1 = (series.StackedData[j].Y - actualpoint1[i].Y)-value;
                            break;
                        }
                    }

                    double x2 = actualpoint1[i].Y + (series.YAxis.VisibleRange.Start * (-1)) + x1;
                    x1 = x1+(series.XAxis.Origin - series.YAxis.VisibleRange.Start);
                    ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                    ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                    var diffX=((-1) * series.YAxis.VisibleRange.Start) - cdpBottomLeft.X;
                    if (!double.IsNaN(points[i].Y))
                        series.Segments.Add(new StackingBarSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                    double yValue = (cdpRightTop.X - ((-1) * series.YAxis.VisibleRange.Start) - diffX) / 2;
                    if (series.AdornmentsInfo != null)
                    {
                        if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                            yValue = (cdpRightTop.X - cdpBottomLeft.X - diffX);
                        else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                            yValue = series.YAxis.Origin - diffX; 
                    }

                    if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                    {
                        ////series.Segments.Add(new ChartAdornment(series.StackedData[i], points, series, 0d));
                        if (points[i].EmptyPoint)
                        {
                            if (series.ShowEmptyPoints)
                                series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yValue), points, series, (sbsInfo.Start - sbsCenter + sbsInfo.End - sbsCenter) / 2));
                        }
                        else
                        {
                            if (!double.IsNaN(points[i].Y))
                                series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yValue), points, series, (sbsInfo.Start - sbsCenter + sbsInfo.End - sbsCenter) / 2));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "StackingBar";
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
