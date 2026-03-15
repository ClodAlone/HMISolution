#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents Fast Line chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class FastLineSegment : Segment
    {
        #region Dependency properties
       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(FastLineSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Points property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
          DependencyProperty.Register("Points", typeof(PointCollection), typeof(FastLineSegment), new PropertyMetadata(new PointCollection()));
        #endregion

        #region members
        /// <summary>
        /// Mpoint variable declaration
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
        internal FastLineSegment(ChartPointsCollection points, ChartPointsCollection correspondingPoint, ChartSeries series)
            : base(series, correspondingPoint)
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartFastLineType), ChartTypes.FastLine);
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

        internal FastLineSegment(ChartPointsCollection points, ChartPointsCollection correspondingPoint, ChartSeries series, bool ishistogram)
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
        public PointCollection Points
        {
            get
            {
                return (PointCollection)GetValue(PointsProperty);
            }

            set
            {
                SetValue(PointsProperty, value);
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
            this.Points.Clear();
            this.Points = new PointCollection();
            foreach (ChartPoint data in MPoints)
            {
                var pointX = data.X + (series.XAxis.VisibleRange.Start * (-1));
                var pointY = data.Y + (series.YAxis.VisibleRange.Start * (-1));
                if (pointX >= -1 && pointX <= (series.XAxis.VisibleRange.End - series.XAxis.VisibleRange.Start) + 1)
                {
                    Point pt = transformer.TransformToVisible(pointX, pointY, series);
                    this.Points.Add(pt);
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
            if (MPoints != null)
            {
                for (int temp = 0; temp < MPoints.Count; temp++)
                    MPoints[temp] = null;
                MPoints = null;
            }
        }
    }

    /// <summary>
    /// Represents Fast Line chart type
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class ChartFastLineType : ChartType
    {
        #region member
        ChartPointsCollection fastlinepoints = new ChartPointsCollection();
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
                if (double.IsNaN(points[i].Y) && series.ShowEmptyPoints)
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
                    fastlinepoints.Add(new ChartPoint(x1, y1));
                            
                        if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                        {
                            if (series.EmptyPointStyle == EmptyPointStyle.Interior)
                            {
                                segmentsdata.Add(new ChartAdornment(points[i], points, series, 0d));
                            }
                            else if (series.EmptyPointStyle == EmptyPointStyle.Symbol || series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                            {
                                series.Segments.Add(new ScatterSegment(new ChartPoint(x1, y1), new ChartPoint(x1, y1), points[i], series));
                            }
                        }
                }
                else
                {
                    if (double.IsNaN(points[i].Y))
                        continue;
                    double x1 = points[i].X ;
                    double y1 = points[i].Y ;
                    
                        fastlinepoints.Add(new ChartPoint(x1, y1));
                        if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                        {
                            segmentsdata.Add(new ChartAdornment(points[i], points, series, 0d));
                        }
                }
            }

            series.Segments.Add(new FastLineSegment(fastlinepoints, points, series));
            foreach (Segment seg in segmentsdata)
            {
                ////series.Segments.Add(seg);
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
            return "FastLine";
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
