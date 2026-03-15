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
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for FastScatterSegment 
    /// </summary>
    public class FastScatterSegment : Segment
    {
        #region Dependency properties
       
       /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(FastScatterSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Points property.
        /// </summary>
        public static readonly DependencyProperty ShapeGeometryProperty =
          DependencyProperty.Register("ShapeGeometry", typeof(GeometryGroup), typeof(FastScatterSegment), new PropertyMetadata(new GeometryGroup()));
        #endregion

        #region members
        /// <summary>
        /// MPoint variable declaration
        /// </summary>
        protected ChartPointsCollection MPoints = new ChartPointsCollection();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FastLineSegment"/> class.
        /// </summary>
        /// <param name="points">ploy line Points</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series value.</param>
        internal FastScatterSegment(ChartPointsCollection points, ChartPointsCollection correspondingPoint, ChartSeries series)
            : base(series, correspondingPoint)
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartFastScatterType), ChartTypes.FastScatter);
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = this.Template;
                this.SegmentTemplate = this.Template;
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
                this.SegmentTemplate = series.SegmentTemplate;
            }

            MPoints = points;
        }

        internal FastScatterSegment(ChartPointsCollection points, ChartPointsCollection correspondingPoint, ChartSeries series, bool ishistogram)
            : base(series, correspondingPoint)
        {
            MPoints = points;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Template. This is a depency property.
        /// </summary>
        /// <value>The DataTemplate value.</value>
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
        /// Gets or sets the Points co-ordinate of segment. This is a depency property.
        /// </summary>
        /// <value>The Points value.</value>
        public GeometryGroup ShapeGeometry
        {
            get
            {
                return (GeometryGroup)GetValue(ShapeGeometryProperty);
            }

            set
            {
                SetValue(ShapeGeometryProperty, value);
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
            this.ShapeGeometry.Children.Clear();
            this.ShapeGeometry = new GeometryGroup();
            this.ShapeGeometry.FillRule = FillRule.Nonzero;
            foreach (ChartPoint data in MPoints)
            {
                var pointX = data.X + (series.XAxis.VisibleRange.Start * (-1));
                var pointY = data.Y + (series.YAxis.VisibleRange.Start * (-1));
                if (pointX >= -1 && pointX <= (series.XAxis.VisibleRange.End - series.XAxis.VisibleRange.Start) + 1)
                {
                    Point pt = transformer.TransformToVisible(pointX, pointY, series);
                    this.ShapeGeometry.Children.Add(new EllipseGeometry() { Center = pt, RadiusX = ChartFastScatterType.GetFastScatterWidth(series), RadiusY = ChartFastScatterType.GetFastScatterHeight(series) });
                }
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            //if (m_points != null)
            //{
            //    for (int temp = 0; temp < m_points.Count; temp++)
            //        m_points[temp] = null;
            //    m_points = null;
            //}
        }
    }
    /// <summary>
    /// Class implementation for FastColumnSegment
    /// </summary>
    public class FastColumnSegment : Segment
    {
        #region Dependency properties
       
       /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(FastColumnSegment), new PropertyMetadata(ResourceManager.GetSeriesTemplate(typeof(ChartFastScatterType), ChartTypes.FastScatter)));

        /// <summary>
        /// Identifies the Points property.
        /// </summary>
        public static readonly DependencyProperty ShapeGeometryProperty =
          DependencyProperty.Register("ShapeGeometry", typeof(GeometryGroup), typeof(FastColumnSegment), new PropertyMetadata(new GeometryGroup()));
        #endregion

        #region members
        /// <summary>
        /// MPoint variable declaration
        /// </summary>
        protected ObservableCollection<FastRect> MPoints = new ObservableCollection<FastRect>();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FastLineSegment"/> class.
        /// </summary>
        /// <param name="points">ploy line Points</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series value.</param>
        internal FastColumnSegment(ObservableCollection<FastRect> points, ChartPointsCollection correspondingPoint, ChartSeries series)
            : base(series, correspondingPoint)
        {
            if (series.SegmentTemplate == null)
            {
                series.ActualSegmentTemplate = this.Template;
                this.SegmentTemplate = this.Template;
            }
            else
            {
                series.ActualSegmentTemplate = series.SegmentTemplate;
                this.SegmentTemplate = series.SegmentTemplate;
            }

            MPoints = points;
        }

        internal FastColumnSegment(ObservableCollection<FastRect> points, ChartPointsCollection correspondingPoint, ChartSeries series, bool ishistogram)
            : base(series, correspondingPoint)
        {
            MPoints = points;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Template. This is a depency property.
        /// </summary>
        /// <value>The DataTemplate value.</value>
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
        /// Gets or sets the Points co-ordinate of segment. This is a depency property.
        /// </summary>
        /// <value>The Points value.</value>
        public GeometryGroup ShapeGeometry
        {
            get
            {
                return (GeometryGroup)GetValue(ShapeGeometryProperty);
            }

            set
            {
                SetValue(ShapeGeometryProperty, value);
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
            this.ShapeGeometry.Children.Clear();
            this.ShapeGeometry = new GeometryGroup();
            this.ShapeGeometry.FillRule = FillRule.Nonzero;
            foreach (FastRect data in MPoints)
            {
                var bottomLeftPointX = data.BottomLeft.X + (series.XAxis.VisibleRange.Start * (-1));
                var topRightPointX = data.RightTop.X + (series.XAxis.VisibleRange.Start * (-1));
                if (bottomLeftPointX >= -1 && bottomLeftPointX <= (series.XAxis.VisibleRange.End - series.XAxis.VisibleRange.Start) + 1)
                {
                    Point rightToppt = transformer.TransformToVisible(topRightPointX, data.RightTop.Y + (series.YAxis.VisibleRange.Start * (-1)), series);
                    Point bottomLeftpt = transformer.TransformToVisible(bottomLeftPointX , data.BottomLeft.Y - series.YAxis.VisibleRange.Start, series);
                    double width = Math.Abs(bottomLeftpt.X - rightToppt.X);
                    double height = Math.Abs(bottomLeftpt.Y - rightToppt.Y);
                    this.ShapeGeometry.Children.Add(new RectangleGeometry() { Rect = new Rect(bottomLeftpt.X, rightToppt.Y,width, height) });
                }
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            //if (m_points != null)
            //{
            //    for (int temp = 0; temp < m_points.Count; temp++)
            //        m_points[temp] = null;
            //    m_points = null;
            //}
        }
    }

    /// <summary>
    /// Class implementation for FastColumnType
    /// </summary>
    public class FastColumnType : ChartType
    {
                #region Implementation
        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            ObservableCollection<FastRect> chartPoints = new ObservableCollection<FastRect>();
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
                series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
                for (int i = 0; i < points.Count; i++)
                {
                    if (!double.IsNaN(points[i].Y) && !points[i].EmptyPoint)
                    {
                        double x1 = points[i].X + sbsInfo.Start - sbsCenter ;
                        double x2 = points[i].X + sbsInfo.End - sbsCenter ;
                        double y1 = series.XAxis.Origin ;
                        double y2 = points[i].Y ;
                        if (y2 < y1)
                        {
                            double swap = y2;
                            y2 = y1;
                            y1 = swap;
                        }
                        ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                        ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                       
                            //series.Segments.Add(new ColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                            chartPoints.Add(new FastRect() { BottomLeft = cdpBottomLeft, RightTop = cdpRightTop });
                        
                    }
                    else if (points[i].EmptyPoint == true && series.ShowEmptyPoints == true)
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
                        double x1 = points[i].X + sbsInfo.Start - sbsCenter ;
                        double x2 = points[i].X + sbsInfo.End - sbsCenter ;
                        double y1 = series.XAxis.Origin ;
                        double y2 = points[i].Y ;
                        if (y2 < y1)
                        {
                            double swap = y2;
                            y2 = y1;
                            y1 = swap;
                        }
                        ChartPoint cdpBottomLeft = new ChartPoint(x1, y1);
                        ChartPoint cdpRightTop = new ChartPoint(x2, y2);
                       
                            if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                            {
                                //If any performance issue with empty point can use FastRect. But Interior can't be customized.
                                //FastRect rect = new FastRect() { BottomLeft = cdpBottomLeft, RightTop = cdpRightTop };
                                //chartPoints.Add(rect);
                                series.Segments.Add(new ColumnSegment(cdpBottomLeft, cdpRightTop, points[i], series));
                            }
                            else
                            {
                                cdpBottomLeft = new ChartPoint(x1 + (x2 - x1) / 2, y2);
                                cdpRightTop = new ChartPoint(x2, y2);
                                ScatterSegment scatter = new ScatterSegment(cdpBottomLeft, cdpRightTop, points[i], series);
                                series.Segments.Add(scatter);
                            }
                        
                    }
                }

                series.Segments.Add(new FastColumnSegment(chartPoints, points, series));
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
            return "FastColumn";
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

    /// <summary>
    /// Class implementation for FastRect
    /// </summary>
    public class FastRect
    {
        /// <summary>
        /// Get or Set BottomLeft property   /// </summary>
        public ChartPoint BottomLeft
        {
            get;
            set;
        }
        /// <summary>
        /// get or Set RightTop property
        /// </summary>
        public ChartPoint RightTop
        {
            get;
            set;

        }
    }
}
