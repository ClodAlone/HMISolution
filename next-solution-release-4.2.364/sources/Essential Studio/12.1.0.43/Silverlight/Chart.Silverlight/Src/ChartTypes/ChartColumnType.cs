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
    using System.Windows.Markup;

    /// <summary>
    /// Represents column chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class ColumnSegment : Segment
    {
        #region Dependency properties
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for XWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty XWidthProperty =
            DependencyProperty.Register("XWidth", typeof(double), typeof(ColumnSegment), new PropertyMetadata(0d));


        internal static readonly DependencyProperty LayerBrushProperty =
            DependencyProperty.Register("LayerBrush", typeof(Brush), typeof(ColumnSegment), new PropertyMetadata(null));

        internal Brush LayerBrush
        {
            get { return (Brush)GetValue(LayerBrushProperty); }
            set { SetValue(LayerBrushProperty, value); }
        }
        /// <summary>
        /// Identifies the Height depency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(ColumnSegment), new PropertyMetadata(0d));

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ColumnSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Width depency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ColumnSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the X depency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
          DependencyProperty.Register("X", typeof(double), typeof(ColumnSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y depency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
          DependencyProperty.Register("Y", typeof(double), typeof(ColumnSegment), new PropertyMetadata(0d));
        #endregion

        #region memebers
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
        internal ColumnSegment(ChartPoint bottomLeftPnt, ChartPoint topRightPnt, ChartPoint correspondingPoint, ChartSeries series)
            : base(series, new ChartPointsCollection { correspondingPoint })
        {
            if (series.Type == ChartTypes.StackingBar100)
            {
                this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartBarType), ChartTypes.Bar);
            }
            else
            {
                this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartColumnType), ChartTypes.Column);
            }
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = this.Template;
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
        internal double OldX
        {
            get;
            set;
        }

        internal double OldY
        {
            get;
            set;
        }

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
        private double XWidth
        {
            get { return (double)GetValue(XWidthProperty); }
            set { SetValue(XWidthProperty, value); }
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
            OldX = this.X;
            OldY = this.Y;
            base.Update(transformer);
            var bottomLeftPointX = m_bottomLeftPoint.X + (series.XAxis.VisibleRange.Start * (-1));
            var topRightPointX = m_topRightPoint.X + (series.XAxis.VisibleRange.Start * (-1));
            if (bottomLeftPointX >= -1 && bottomLeftPointX <= (series.XAxis.VisibleRange.End - series.XAxis.VisibleRange.Start) + 1)
            {
                
                Point blPoint = transformer.TransformToVisible(bottomLeftPointX, m_bottomLeftPoint.Y - series.YAxis.VisibleRange.Start, series);
                Point trPoint = transformer.TransformToVisible(topRightPointX, m_topRightPoint.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                Rect columnRect = new Rect(blPoint, trPoint);
                this.X = columnRect.X;
                this.Y = columnRect.Y;// -this.series.XAxis.LineStrokeThickness;
                this.Width = columnRect.Width;
                this.Height = columnRect.Height;
                this.XWidth = X + Width;
            }
            else if (series.Type == ChartTypes.StackingBar100 || series.Type == ChartTypes.StackingColumn100)
            {

                Point blPoint = transformer.TransformToVisible(m_bottomLeftPoint.X, m_bottomLeftPoint.Y, series);
                Point trPoint = transformer.TransformToVisible(m_topRightPoint.X, m_topRightPoint.Y, series);
                Rect columnRect = new Rect(blPoint, trPoint);
                this.X = columnRect.X;
                this.Y = columnRect.Y;// -this.series.XAxis.LineStrokeThickness;
                this.Width = columnRect.Width;
                this.Height = columnRect.Height;
                this.XWidth = X + Width;
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            m_bottomLeftPoint = null;
            m_topRightPoint = null;
            //this.Template = null;
        }
    }

    /// <summary>
    /// Represents Fast Line chart type.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class ChartColumnType : ChartType
    {
        #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            if (series.Area != null)
            {
                SetRange(series, points, 1);
                DoubleRange sbsInfo = series.Area.GetSideBySideInfo(series);
                double sbsCenter = 0.4 * (series.XAxis.VisibleInterval < 1 && series.Area.MinDataInterval < 1d ? series.Area.MinDataInterval : 1d);

                if (series.XAxis.ValueType == ChartValueType.DateTime && series.IsIndexed == true && series.XAxis.IsAutoSetRange == false)
                {
                    points = series.Data;
                    series.IsIndexed = false;
                }
                series.Segments.Clear();
                series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y ).Sum();
                for (int i = 0; i < points.Count; i++)
                {
                    if (!double.IsNaN(points[i].Y) && !points[i].EmptyPoint)
                    {
                        double x1 = points[i].X + sbsInfo.Start - sbsCenter;
                        double x2 = points[i].X + sbsInfo.End - sbsCenter ;
                        double y1 = series.XAxis.Origin;
                        double y2 = points[i].Y ;
                        ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                        ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                        
                            series.Segments.Add(new ColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                            if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                            {
                                //this condition is included to give support to TopAndBottom position for adornments
                                if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                                {
                                    double yvalue = points[i].Y ;
                                    ////series.Segments.Add(new ChartAdornment(points[i], points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                                    series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yvalue), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));

                                }
                                else if (series.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                                {
                                    double yvalue = series.YAxis.Origin;
                                    ////series.Segments.Add(new ChartAdornment(points[i], points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                                    series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yvalue), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));

                                }
                                else
                                {
                                    double yvalue = (series.YAxis.Origin + points[i].Y) / 2;
                                    series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yvalue), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                                }
                            }
                     
                    }
                    else if(series.ShowEmptyPoints)
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
                            double x1 = points[i].X + sbsInfo.Start - sbsCenter ;
                            double x2 = points[i].X + sbsInfo.End - sbsCenter ;
                            double y1 = series.XAxis.Origin ;
                            double y2 = points[i].Y ;
                            ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                            ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                               series.Segments.Add(new ColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                                if (series.AdornmentsInfo != null)
                                {
                                    if (series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.TopAndBottom)
                                    {
                                        series.Adornments.Add(new ChartAdornment(points[i], points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                                    }
                                    else
                                    {
                                        double yvalue = (cdpBottomLeft.Y + cdpRightTop.Y) / 2;
                                        series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yvalue), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                                    }
                                }
                           
                        }
                        else if (series.EmptyPointStyle == EmptyPointStyle.Symbol || series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                        {
                            double x1 = points[i].X ;
                            double y1 = points[i].Y ;
                            double x2 = 0;
                            double y2 = series.XAxis.Origin ;
                            ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                            ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                              series.Segments.Add(new ScatterSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                                if (series.AdornmentsInfo != null)
                                {
                                    if (series.AdornmentsInfo.AdornmentsPosition != AdornmentsPosition.TopAndBottom)
                                    {
                                        series.Adornments.Add(new ChartAdornment(points[i], points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                                    }
                                    else
                                    {
                                        double yvalue = (cdpBottomLeft.Y + cdpRightTop.Y) / 2;
                                        series.Adornments.Add(new ChartAdornment(points[i], new ChartPoint(points[i].X, yvalue), points, series, (sbsInfo.Start - 0.4 + sbsInfo.End - 0.4) / 2));
                                    }
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
            return "Column";
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

