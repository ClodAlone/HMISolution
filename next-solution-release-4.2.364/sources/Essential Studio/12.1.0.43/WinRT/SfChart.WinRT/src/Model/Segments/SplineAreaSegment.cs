#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using WindowsLineSegment = System.Windows.Media.LineSegment;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart SplineArea segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="SplineAreaSeries"/>
    [ClassReference(IsReviewed = false)]
    public class SplineAreaSegment : AreaSegment
    {
        internal List<Point> segmentPoints = new List<Point>();

        private SplineAreaSeries containerSeries;

        Path segPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="series"></param>
        public SplineAreaSegment(List<Point> Points, List<double> xValues, IList<double> yValues, SplineAreaSeries series)
            : base(xValues, yValues)
        {
            Series = containerSeries = series;
            SetData(Points, xValues, yValues);
        }

        /// <summary>
        /// Method implementation for SetData
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        public void SetData(List<Point> points, List<double> xValues, IList<double> yValues)
        {
            segmentPoints = points;
            XRange = new DoubleRange(points.Min(item => item.X), points.Max(item => item.X));
            YRange = new DoubleRange(points.Min(item => item.Y), points.Max(item => item.Y));
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// retuns UIElement
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement CreateVisual(Size size)
        {
            segPath = new Path();
            segPath.Tag = this;
            SetVisualBindings(segPath);
            return segPath;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return segPath;
        }

        /// <summary>
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
        [ClassReference(IsReviewed = false)]
        protected override void SetVisualBindings(Shape element)
        {
            base.SetVisualBindings(element);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            Series.SeriesRootPanel.Clip = null;
            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
            double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
            double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
            double yStart = cartesianTransformer.YAxis.VisibleRange.Start;
            double yEnd = cartesianTransformer.YAxis.VisibleRange.End;
            DoubleRange range = cartesianTransformer.XAxis.VisibleRange;
            
            PathFigure figure = new PathFigure();
            PathGeometry segmentGeometry = new PathGeometry();
            double origin = containerSeries.ActualYAxis.Origin;
            WindowsLineSegment lineSegment;
            figure.StartPoint = transformer.TransformToVisible(segmentPoints[0].X, 0);
            lineSegment = new WindowsLineSegment();
            lineSegment.Point=transformer.TransformToVisible(segmentPoints[0].X, segmentPoints[0].Y);
            figure.Segments.Add(lineSegment);
            int i;
            for (i = 1; i < segmentPoints.Count; i += 3)
            {
               double xVal = segmentPoints[i].X;
               if (xVal >= range.Start && xVal <= range.End || xEnd>=range.Start && xEnd <=range.End)
                {
                    BezierSegment segment = new BezierSegment();
                    segment.Point1 = transformer.TransformToVisible(segmentPoints[i].X, segmentPoints[i].Y);
                    segment.Point2 = transformer.TransformToVisible(segmentPoints[i + 1].X, segmentPoints[i + 1].Y);
                    segment.Point3 = transformer.TransformToVisible(segmentPoints[i + 2].X, segmentPoints[i + 2].Y);
                    figure.Segments.Add(segment);
                }
            }
            lineSegment = new WindowsLineSegment();
            lineSegment.Point = transformer.TransformToVisible(segmentPoints[i - 1].X, 0);
            figure.Segments.Add(lineSegment);
            figure.IsClosed = true;
            segmentGeometry.Figures.Add(figure);
            this.segPath.Data = segmentGeometry;
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        [ClassReference(IsReviewed = false)]
        public override void OnSizeChanged(Size size)
        {

        }

    }
}
