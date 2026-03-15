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
    /// Represents Stackingcolumn chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class StackingColumnSegment : Segment
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the Height depency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(StackingColumnSegment), new PropertyMetadata(0d));

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(StackingColumnSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Width depency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(StackingColumnSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the X depency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(StackingColumnSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y depency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(StackingColumnSegment), new PropertyMetadata(0d));
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
        internal StackingColumnSegment(ChartPoint bottomLeftPnt, ChartPoint topRightPnt, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(StackingColumnSegment), ChartTypes.StackingColumn);
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = !series.EnableEffects ? this.Template : ResourceManager.GetSeriesTemplateWithEffects(typeof(ChartStackingColumnType), ChartTypes.StackingColumn);
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
        /// Get or Set Template property
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
    }

    /// <summary>
    /// Represents Stacking Column chart type.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class ChartStackingColumnType : ChartType
    {
        #region 

        // Temporily Store the Stack Data
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

            #region Points
                foreach (ChartSeries series1 in series.Area.Series)
                {
                    if (series1.Visibility == Visibility.Visible && series1.Type == ChartTypes.StackingColumn && series1.Data != null)
                    {
                        if (series.IsIndexed == true)
                        {
                            actualdata = series1.IndexActualData;
                        }
                        else
                        {
                            actualdata = series1.Data;
                        }

                        if (k == 0)
                        {
                            k = 1;
                            series1.StackedData.Clear();
                            sum.Clear();
                            foreach (ChartPoint cp in actualdata)
                            {
                                series1.StackedData.Add(new ChartPoint(cp.X, (double.IsNaN(cp.Y) ? 0 : cp.Y)));
                                sum.Add(new ChartPoint(cp.X, (double.IsNaN(cp.Y) ? 0 : cp.Y)));
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
                                        sum[i].Y += (double.IsNaN(actualdata[j].Y) ? 0 : actualdata[j].Y);//actualdata[j].Y;
                                        break;
                                    }
                                }

                                if (!status)
                                {
                                    sum.Add(new ChartPoint(actualdata[j].X, (double.IsNaN(actualdata[j].Y) ? 0 : actualdata[j].Y)));
                                }
                            }

                            series1.StackedData.Clear();
                            foreach (ChartPoint cp in sum)
                            {
                                series1.StackedData.Add(new ChartPoint(cp.X, (double.IsNaN(cp.Y) ? 0 : cp.Y)));
                            }                            
                        }
                    }                    
                }
                #endregion

                double max = 0;
                foreach (ChartSeries s in series.Area.Series)
                {
                    var v = from st in s.StackedData
                            select st.Y;
                    if (v.Count() > 0)
                    {
                        if (v.Max() > max)
                        {
                            max = v.Max();
                            this.tempStackData = s.StackedData;
                        }
                    }
                }

            if (series.Area != null)
            {
                SetRange(series,this.tempStackData, 1);
                DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
                double sbsCenter = 0.4 * (series.XAxis.VisibleInterval < 1 && series.Area.MinDataInterval < 1d ? series.Area.MinDataInterval : 1d);
                series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();

                for (int i = 0; i < points.Count; i++)
                {
                    if (!double.IsNaN(points[i].Y) && !points[i].EmptyPoint)
                    {
                        double x1 = points[i].X + sbsInfo.Start - sbsCenter + (series.XAxis.VisibleRange.Start * (-1));
                        double x2 = points[i].X + sbsInfo.End - sbsCenter + (series.XAxis.VisibleRange.Start * (-1));
                        double y1 = 0;
                        double value = series.Area.Series.IndexOf(series) > 0 ? series.XAxis.Origin : 0;
                        for (int j = 0; j < series.StackedData.Count; j++)
                        {
                            if (series.StackedData[j].X == points[i].X && series.StackedData[j].Y < 0 && points[i].Y > 0)
                            {
                                y1 = 0;
                                break;
                            }
                            if (series.StackedData[j].X == points[i].X)
                            {
                                y1 = (series.StackedData[j].Y - points[i].Y) - value;
                                break;
                            }
                        }

                        double y2 = points[i].Y + y1+ ( series.YAxis.VisibleRange.Start * (-1));
                        y1 = y1 + (series.XAxis.Origin - series.YAxis.VisibleRange.Start);
                        ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                        ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                        series.Segments.Add(new StackingColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                        var diffY =( (-1) * series.YAxis.VisibleRange.Start) - cdpBottomLeft.Y;
                        double yValue = (cdpRightTop.Y - ((-1) * series.YAxis.VisibleRange.Start) - diffY) / 2;
                        if (series.AdornmentsInfo != null)
                        {
                            if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                yValue = (cdpRightTop.Y - cdpBottomLeft.Y - diffY);
                            else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                yValue = series.YAxis.Origin - diffY;
                        }
                       
                        if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible && series.StackedData.Count > 0)
                        {
                            ////series.Segments.Add(new ChartAdornment(series.StackedData[i], points, series, 0d));
                            //series.Adornments.Add(new ChartAdornment(series.StackedData[i], points, series, 0d));
                            series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yValue), points, series, (sbsInfo.Start - sbsCenter + sbsInfo.End - sbsCenter) / 2));
                        }
                    }
                    else if (series.ShowEmptyPoints)
                    {
                        points[i].EmptyPoint = true;
                        if (series.EmptyPointValue == EmptyPointValue.Average)
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
                        else
                        {
                            points[i].Y = 0;
                        }

                        if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                        {
                            double x1 = points[i].X + sbsInfo.Start - 0.4 + (series.XAxis.VisibleRange.Start * (-1));
                            double x2 = points[i].X + sbsInfo.End - 0.4 + (series.XAxis.VisibleRange.Start * (-1));
                            double y1 = 0;
                            for (int j = 0; j < series.StackedData.Count; j++)
                            {
                                if (series.StackedData[j].X == points[i].X)
                                {
                                    y1 = series.StackedData[j].Y - points[i].Y;
                                    break;
                                }
                            }
                            double y2 = points[i].Y + (series.YAxis.VisibleRange.Start * (-1)) + y1;
                            ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                            ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                            series.Segments.Add(new StackingColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                            double yValue = (cdpBottomLeft.Y + cdpRightTop.Y) / 2;
                            if (series.AdornmentsInfo != null)
                            {
                                if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                    yValue = cdpRightTop.Y;
                                else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                    yValue = cdpBottomLeft.Y;
                            }
                            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible && series.StackedData.Count > 0)
                            {
                                series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yValue), points, series, (sbsInfo.Start - sbsCenter + sbsInfo.End - sbsCenter) / 2));
                            }
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Symbol || series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                        {
                            double x1 = points[i].X +sbsInfo.Median - 0.4 + (series.XAxis.VisibleRange.Start * (-1));
                            double x2 = 0;
                            double y1 = 0;
                            for (int j = 0; j < series.StackedData.Count; j++)
                            {
                                if (series.StackedData[j].X == points[i].X)
                                {
                                    y1 = series.StackedData[j].Y -points[i].Y;
                                    break;
                                }
                            }
                            y1 = points[i].Y + (series.YAxis.VisibleRange.Start * (-1)) + y1;
                            double y2 = 0;
                            ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                            ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                            series.Segments.Add(new ScatterSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                            double yValue = (cdpBottomLeft.Y + cdpRightTop.Y) / 2;
                            if (series.AdornmentsInfo != null)
                            {
                                if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                    yValue = cdpRightTop.Y;
                                else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                    yValue = cdpBottomLeft.Y;
                            }
                            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible && series.StackedData.Count > 0)
                            {
                                series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yValue), points, series, (sbsInfo.Start - sbsCenter + sbsInfo.End - sbsCenter) / 2));
                            }
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
            return "StackingColumn";
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
