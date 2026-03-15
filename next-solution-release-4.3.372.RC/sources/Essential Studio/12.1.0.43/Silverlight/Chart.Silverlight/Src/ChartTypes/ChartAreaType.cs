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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents Area chart type segment.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class AreaSegment : Segment
    {
       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(AreaSegment), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the AreaPoints depency property.
        /// </summary>
        public static readonly DependencyProperty AreaPointsProperty =
          DependencyProperty.Register("AreaPoints", typeof(Geometry), typeof(AreaSegment), new PropertyMetadata(null));
      /// <summary>
      /// MPolypoint variable declaration
      /// </summary>
        protected ChartPointsCollection MPolypoint=new ChartPointsCollection();

        /// <summary>
        /// Initializes a new instance of the <see>
        ///                                       <cref>ChartColumnSegment</cref>
        ///                                   </see>
        ///     class.
        /// </summary>
        /// <param name="areapoints">Area Points</param>
        /// <param name="correspondingPoint">The corresponding point.</param>
        /// <param name="series">The series.</param>
        internal AreaSegment(ChartPointsCollection areapoints, ChartPointsCollection correspondingPoint, ChartSeries series)
            : base(series, correspondingPoint)
        {
            this.Template = ResourceManager.GetSeriesTemplate(typeof(ChartAreaType), ChartTypes.Area);
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

            MPolypoint = areapoints;
        }

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
        /// Gets or sets the Area Points co-ordinate of segment. This is a depency property.
        /// </summary>
        /// <value>The AreaPoints value.</value>
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
       
        /// <summary>
        /// Updates the real coordinates of segment.
        /// </summary>
        /// <param name="transformer">Instance of class that implements <see cref="IChartTransformer"/> interface.</param>
        public override void Update(IChartTransformer transformer)
        {
            base.Update(transformer);
            PathFigure figure = new PathFigure();
            Point pt = new Point();
            if (MPolypoint.Count > 0)
            {
                figure.StartPoint = transformer.TransformToVisible(MPolypoint[0].X+ (series.XAxis.VisibleRange.Start * (-1)) , MPolypoint[0].Y+ (series.YAxis.VisibleRange.Start * (-1)), series);
                pt = figure.StartPoint;
                pt.X = figure.StartPoint.X;// +series.YAxis.LineStrokeThickness;
                pt.Y = figure.StartPoint.Y;// -series.XAxis.LineStrokeThickness;
                figure.StartPoint = pt;

                double startPt = MPolypoint[0].X;
                double endPt = (series.Type == ChartTypes.RangeArea) ? MPolypoint[MPolypoint.Count/2 - 1].X + (series.XAxis.VisibleRange.Start * (-1)): MPolypoint[MPolypoint.Count - 1].X+ (series.XAxis.VisibleRange.Start * (-1));

                for (int i = 1; i < MPolypoint.Count; i++)
                {
                    System.Windows.Media.LineSegment l=new System.Windows.Media.LineSegment();
                    if (MPolypoint[i].X == startPt)
                    {
                        l.Point = transformer.TransformToVisible(MPolypoint[i].X + (series.XAxis.VisibleRange.Start * (-1)) , MPolypoint[i].Y+ (series.YAxis.VisibleRange.Start * (-1)), series);
                        pt = l.Point;
                        pt.X = l.Point.X;// +series.YAxis.LineStrokeThickness;
                        l.Point = pt;
                    }
                    else if (MPolypoint[i].X == endPt)
                    {
                        l.Point = transformer.TransformToVisible(MPolypoint[i].X+ (series.XAxis.VisibleRange.Start * (-1)), MPolypoint[i].Y+ (series.YAxis.VisibleRange.Start * (-1)), series);
                        pt = l.Point;
                        pt.X = l.Point.X -series.XAxis.GridLineStrokeThickness;
                        pt.Y = l.Point.Y;// -series.XAxis.LineStrokeThickness;
                        l.Point = pt;
                    }
                    else
                        l.Point = transformer.TransformToVisible(MPolypoint[i].X+ (series.XAxis.VisibleRange.Start * (-1)), MPolypoint[i].Y+ (series.YAxis.VisibleRange.Start * (-1)), series);
                    figure.Segments.Add(l);
                }

                figure.IsClosed = true;
            }

            PathGeometry pg = new PathGeometry();
            PathFigureCollection fig=new PathFigureCollection();
            fig.Add(figure);
            pg.Figures = fig;
            this.AreaPoints = pg;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.MPolypoint = null;
            this.Template = null;
            this.Template.ClearValue(TemplateProperty);
            this.SegmentTemplate = null;
            //this.Template = null;
        }
    }

    /// <summary>
    /// Represents Area chart type.
    /// </summary>
    /// <remarks>Class instance is created automaticaly by Silverlight Chart building system.</remarks>
    public class ChartAreaType : ChartType
    {
        #region Implementation
        /// <summary>
        ///new  Instance created for ChartPointCollection
        /// </summary>
        protected ChartPointsCollection Polypoints = new ChartPointsCollection();

        /// <summary>
        /// Calculates the segments.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="points">The points.</param>
        protected override void CalculateSegments(ChartSeries series, ChartPointsCollection points)
        {
            ChartPointsCollection newPoints = new ChartPointsCollection();
            for (int i = 0; i < points.Count; i++)
            {
                if (!double.IsNaN(points[i].Y))
                    newPoints.Add(points[i]);
            }
            SetRange(series, newPoints, 1);
            Polypoints.Clear();
            if (points.Count == 0)
            {
                return;
            }

            series.sum = (from point in points select point.Y).Sum();
            for (int i = 0; i < newPoints.Count; i++)
            {
                if (points[i].EmptyPoint && series.ShowEmptyPoints)
                {
                    if (series.EmptyPointValue == EmptyPointValue.Zero)
                    {
                        newPoints[i].Y = 0;
                    }
                    else
                    {
                        if (i + 1 == points.Count)
                            newPoints[i].Y = newPoints[i - 1].Y / 2;
                        else
                        {
                            int index;
                            for (index = i + 1; index < newPoints.Count; index++)
                                if (!double.IsNaN(newPoints[index].Y))
                                    break;
                            if (i == 0)
                                newPoints[i].Y = (index == points.Count ? 40 : newPoints[index].Y) / 2;
                            else
                                newPoints[i].Y = newPoints[i - 1].Y / 2 + (index == newPoints.Count ? 40 : newPoints[index].Y) / 2;
                        }
                    }
                    double x1 = newPoints[i].X ;
                    double y1 = newPoints[i].Y ;
                    Polypoints.Add(new ChartPoint(x1, y1));
                }
                else if (points[i].EmptyPoint && !series.ShowEmptyPoints && i - 1 >= 0 && i + 1 < points.Count)
                {
                    double x1 = newPoints[i - 1].X ;
                    double y1 = newPoints[i - 1].Y ;
                    double x2 = newPoints[i + 1].X ;
                    double y2 = newPoints[i + 1].Y ;
                    Polypoints.Add(new ChartPoint(x1, 0));
                    Polypoints.Add(new ChartPoint(x2, 0));
                    Polypoints.Add(new ChartPoint(x2, y2));
                }
                else
                {
                    double x1 = newPoints[i].X ;
                    double y1 = newPoints[i].Y ;

                    if (series.Area.Host == Host.OLAPChart)
                    {
                        x1 -= 0.5;
                        //y1 -= 0.5;
                    }

                    Polypoints.Add(new ChartPoint(x1, y1));
                }
            }

            if (series.IsIndexed == false)
            {
                sordpoints();
            }

            double yValue = 0;
            yValue = series.XAxis.Origin ;
            Polypoints.Insert(0, new ChartPoint(Polypoints[0].X, yValue));
            Polypoints.Add(new ChartPoint(Polypoints[Polypoints.Count - 1].X, yValue));


            series.Segments.Add(new AreaSegment(Polypoints, newPoints, series));
            for (int i = 0; i < newPoints.Count; i++)
            {
                if (series.AdornmentsInfo != null && series.AdornmentsInfo.Visible)
                {
                    ////series.Segments.Add(new ChartAdornment(points[i], points, series, 0d));
                    if (newPoints[i].EmptyPoint)
                    {
                        if (series.ShowEmptyPoints)
                            series.Adornments.Add(new ChartAdornment(newPoints[i], newPoints, series, 0d));
                    }
                    else
                        series.Adornments.Add(new ChartAdornment(newPoints[i], newPoints, series, 0d));
                }
            }
        }
        /// <summary>
        /// Method implementation for Sortpoints
        /// </summary>
        protected void sordpoints()
        {
            for (int i = 0; i < Polypoints.Count; i++)
            {
                for (int j = i+1; j < Polypoints.Count; j++)
                {
                    if (Polypoints[i].X > Polypoints[j].X)
                    {
                        double temp = Polypoints[i].X;
                        Polypoints[i].X = Polypoints[j].X;
                        Polypoints[j].X = temp;

                        temp = Polypoints[i].Y;
                        Polypoints[i].Y = Polypoints[j].Y;
                        Polypoints[j].Y = temp;
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
            return "Area";
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
