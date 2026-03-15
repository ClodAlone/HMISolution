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
    /// Represents Area chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class StackingAreaSegment : Segment
    {
        #region Dependency properties
       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(StackingAreaSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the AreaPoints dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaPointsProperty =
          DependencyProperty.Register("AreaPoints", typeof(Geometry), typeof(StackingAreaSegment), new PropertyMetadata(null));
        #endregion

        #region Members
        private ChartPointsCollection m_polypoint = new ChartPointsCollection();
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see>
        ///                                       <cref>ChartColumnSegment</cref>
        ///                                   </see>
        ///     class.
        /// </summary>
        /// <param name="areapoints">The areapoints PNT.</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal StackingAreaSegment(ChartPointsCollection areapoints, ChartPointsCollection correspondingPoint, ChartSeries series)
            : base(series, correspondingPoint)
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartStackingAreaType), ChartTypes.StackingArea);
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

            m_polypoint = areapoints;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set TemplateProperty
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
        /// Gets or sets the X co-ordinate of segment. This is a depency property.
        /// </summary>
        /// <value>The X value.</value>
        public Geometry AreaPoints
        {
            get
            {
                return (Geometry)GetValue(AreaPointsProperty);
            }

            set
            {
                SetValue(AreaPointsProperty, value);
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
            PathFigure figure = new PathFigure();
            Point pt = new Point();
            if (m_polypoint.Count > 0)
            {
                figure.StartPoint = transformer.TransformToVisible(m_polypoint[0].X, m_polypoint[0].Y, series);
                pt = figure.StartPoint;
                pt.X = figure.StartPoint.X + series.YAxis.LineStrokeThickness;
                pt.Y = figure.StartPoint.Y - series.XAxis.LineStrokeThickness;
                figure.StartPoint = pt;

                double startPt = m_polypoint[0].X;
                double endPt = m_polypoint[m_polypoint.Count - 1].X;

                for (int i = 1; i < m_polypoint.Count; i++)
                {
                    System.Windows.Media.LineSegment l = new System.Windows.Media.LineSegment();
                    if (m_polypoint[i].X == startPt)
                    {
                        l.Point = transformer.TransformToVisible(m_polypoint[i].X, m_polypoint[i].Y, series);
                        pt = l.Point;
                        pt.X = l.Point.X + series.YAxis.LineStrokeThickness;
                        pt.Y = l.Point.Y - 2 * series.XAxis.LineStrokeThickness;
                        l.Point = pt;
                    }
                    else if (m_polypoint[i].X == endPt)
                    {
                        l.Point = transformer.TransformToVisible(m_polypoint[i].X, m_polypoint[i].Y, series);
                        pt = l.Point;
                        pt.X = l.Point.X + series.XAxis.GridLineStrokeThickness;
                        pt.Y = l.Point.Y - series.YAxis.LineStrokeThickness;
                        l.Point = pt;
                    }
                    else
                        l.Point = transformer.TransformToVisible(m_polypoint[i].X, m_polypoint[i].Y, series);
                    figure.Segments.Add(l);
                }

                figure.IsClosed = true;
            }

            PathGeometry pg = new PathGeometry();
            PathFigureCollection fig = new PathFigureCollection();
            fig.Add(figure);
            pg.Figures = fig;
            this.AreaPoints = pg;
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (this.m_polypoint != null)
            {
                for (int temp = 0; temp < this.m_polypoint.Count; temp++)
                {
                    this.m_polypoint[temp] = null;
                }
                this.m_polypoint.Clear();
                this.m_polypoint = null;
            }
        }
    }

    /// <summary>
    /// Represents Stacking Area chart type.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class ChartStackingAreaType : ChartType
    {
        #region Members
        ChartPointsCollection polypoints = new ChartPointsCollection();
        internal ChartPointsCollection previouspoints = new ChartPointsCollection();
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
            ChartPointsCollection newPoints = new ChartPointsCollection();
            for (int i = 0; i < points.Count; i++)
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
                if (!double.IsNaN(points[i].Y))
                    newPoints.Add(points[i]);
            }
            foreach (ChartSeries series1 in series.Area.Series)
            {
                if (series1.Visibility == Visibility.Visible && series1.Type == ChartTypes.StackingArea && series1.Data!=null)
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
                    if (v.Max() > max)
                    {
                        max = v.Max();
                        this.tempStackData = s.StackedData;
                    }
                }
            }
            SetRange(series, this.tempStackData, 1);
            SegmentsCollection segmentsdata = new SegmentsCollection();
            series.sum = (from point in points where !point.Y.Equals(double.NaN) select point.Y).Sum();
            polypoints.Clear();
            for (int i = 0; i < newPoints.Count; i++)
            {
                double x1 = newPoints[i].X + (series.XAxis.VisibleRange.Start * (-1));
                double y1 = 0;
                for (int j = 0; j < series.StackedData.Count; j++)
                {
                    if (series.StackedData[j].X == newPoints[i].X)
                    {
                        y1 = series.StackedData[j].Y + (series.YAxis.VisibleRange.Start * (-1));
                        break;
                    }
                }

                if (series.Area.Host == Host.Chart)
                {
                    polypoints.Add(new ChartPoint(x1, y1));
                }
                else
                {
                    polypoints.Add(new ChartPoint(x1-0.5, y1));
                }

                if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                {
                    ////segmentsdata.Add(new ChartAdornment(points[i], points, series, 0d));
                    if (!double.IsNaN(series.StackedData[i].Y))
                        segmentsdata.Add(new ChartAdornment(newPoints[i], series.StackedData[i], newPoints, series, 0d));
                }
            }

            if (series.IsIndexed == false)
            {
                polypoints=sordpoints(polypoints);
            }

            ChartPointsCollection previouspoints = new ChartPointsCollection();
            previouspoints = getpreviousseriespoints(series.StackedData, newPoints);
            previouspoints = sordpoints(previouspoints);
            if (previouspoints.Count == 0 && polypoints.Count>0)
            {
                //if (series.Area.Host == Host.Chart)
                //{
                    double yValue = 0;
                    yValue = series.XAxis.Origin - (series.YAxis.VisibleRange.Start);
                    polypoints.Insert(0, new ChartPoint(polypoints[0].X, yValue));
                    polypoints.Add(new ChartPoint(polypoints[polypoints.Count - 1].X, yValue));
                //}
                //else
                //{
                  //  polypoints.Insert(0, new ChartPoint(polypoints[0].X, 0.5));
                    //polypoints.Add(new ChartPoint(polypoints[polypoints.Count - 1].X, 0.5));
                //}
            }

            double x, y, spaceval = 0d;
            //if (series.StrokeThickness >= 2)
            //{
            //    spaceval = (series.YAxis.VisibleRange.End - series.YAxis.VisibleRange.Start) / 100;
            //    if (series.YAxis.VisibleRange.Start < 0)
            //    {
            //        spaceval -= 2;
            //    }
            //}

            for (int i = 0; i < previouspoints.Count; i++)
            {
                x = previouspoints[i].X + (series.XAxis.VisibleRange.Start * (-1));
                y = previouspoints[i].Y + (series.YAxis.VisibleRange.Start * (-1)) + spaceval;
                if (series.Area.Host == Host.Chart)
                {
                    polypoints.Insert(0, new ChartPoint(x, y));
                }
                else
                {
                    polypoints.Insert(0, new ChartPoint(x-0.5, y));
                }
            }

            series.Segments.Add(new StackingAreaSegment(polypoints, newPoints, series));
            foreach (Segment seg in segmentsdata)
            {
                ////series.Segments.Add(seg);
                series.Adornments.Add(seg);
            }
        }

        private ChartPointsCollection sordpoints(ChartPointsCollection point)
        {
            for (int i = 0; i < point.Count; i++)
            {
                for (int j = i + 1; j < point.Count; j++)
                {
                    if (point[i].X > point[j].X)
                    {
                        double temp = point[i].X;
                        point[i].X = point[j].X;
                        point[j].X = temp;
                        temp = point[i].Y;
                        point[i].Y = point[j].Y;
                        point[j].Y = temp;
                    }
                }
            }

            return point;
        }

        private ChartPointsCollection getpreviousseriespoints(ChartPointsCollection stacked, ChartPointsCollection currect)
        {
            ChartPointsCollection result = new ChartPointsCollection();
            bool status = false;
            for (int i = 0; i < currect.Count; i++)
            {
                if (stacked[i].Y != currect[i].Y)
                {
                    status = true;
                    break;
                }
            }

            if (status == true)
            {
                for (int i = 0; i < stacked.Count; i++)
                {
                    for (int j = 0; j < currect.Count; j++)
                    {
                        if (stacked[i].X == currect[j].X)
                        {
                            result.Add(new ChartPoint(stacked[i].X, stacked[i].Y - currect[j].Y));
                            break;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "StackingArea";
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
            if (this.polypoints != null)
            {
                for (int temp = 0; temp < this.polypoints.Count; temp++)
                {
                    this.polypoints[temp] = null;
                }
                this.polypoints.Clear();
                this.polypoints = null;
            }
            if (this.previouspoints != null)
            {
                for (int temp = 0; temp < this.previouspoints.Count; temp++)
                    this.previouspoints[temp] = null;
                this.previouspoints.Clear();
                this.previouspoints = null;
            }
        }
    }
}
