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
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart Spline segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="SplineSeries"/>
    [ClassReference(IsReviewed = false)]
    public class SplineSegment:ChartSegment
    {

        #region fields

        Point Point1;

        Point Point2;

        Point Point3;

        Point Point4;

        Path segPath;

        #endregion

        #region properties

        public double X1 { get; set; }

        public double X2 { get; set; }

        public double Y1 { get; set; }

        public double Y2 { get; set; }

        private double _yData;
        public double YData
        {
            get { return _yData; }
            set
            {
                _yData = value;
                OnPropertyChanged("YData");
            }
        }
        #endregion

        #region constructor

        /// <summary>
        /// Called when instance created for SplineSegment
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="series"></param>
        public SplineSegment(Point point1, Point point2,Point point3, Point point4, SplineSeries series)
        {
            base.Series = series;
            SetData(point1, point2, point3, point4);
        }
        public SplineSegment(Point point1, Point point2, Point point3, Point point4, ChartSeriesBase series)
        {
            base.Series = series;
            SetData(point1, point2, point3, point4);
        }
        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(Point point1, Point point2, Point point3,Point point4)
        {
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;
            Point4 = point4;
            XRange = new DoubleRange(point1.X, point4.X);
            YRange = GetYRange(point1.X, point1.Y, point2.X, point2.Y, point3.X, point3.Y, point4.X, point4.Y);
        }
        /// <summary>
        /// return doubleRange values from the given values
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x4"></param>
        /// <param name="y4"></param>
        /// <returns></returns>
        internal protected DoubleRange GetYRange(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            DoubleRange range = DoubleRange.Union(new double[] { y1, y2, y3, y4 });

            double cx = 3 * (x2 - x1);
            double cy = 3 * (y2 - y1);

            double bx = 3 * (x3 - x2) - cx;
            double by = 3 * (y3 - y3) - cy;

            double ay = y4 - y1 - by - cy;

            double r1, r2;

            if (ChartMath.SolveQuadraticEquation(3 * ay, 2 * by, cy, out r1, out r2))
            {
                if (r1 >= 0 && r1 <= 1)
                {
                    double y = ay * r1 * r1 * r1 + by * r1 * r1 + cy * r1 + y1;
                    range = DoubleRange.Union(range, y);
                }

                if (r2 >= 0 && r2 <= 1)
                {
                    double y = ay * r2 * r2 * r2 + by * r2 * r2 + cy * r2 + y1;
                    range = DoubleRange.Union(range, y);
                }
            }

            return range;
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
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Interior");
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
            DoubleCollection collection = this.StrokeDashArray;
            if (collection != null && collection.Count > 0)
            {
                    DoubleCollection doubleCollection = new DoubleCollection();
                    foreach (double value in collection)
                    {
                        doubleCollection.Add(value);
                    }
                    element.StrokeDashArray = doubleCollection;
            }
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return segPath;
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
           
            if (transformer != null)
            {
                Series.SeriesRootPanel.Clip = null;
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double interval = cartesianTransformer.XAxis.VisibleInterval;
                double edgeValue = xIsLogarithmic ? Math.Log(Point1.X, xBase) : Point1.X;
                if (edgeValue >= xStart - interval && edgeValue <= xEnd + interval)
                {
                
                PathFigure figure = new PathFigure();
                BezierSegment bezierSeg = new BezierSegment();
              
                PathGeometry segGeometry = new PathGeometry();
                  
                figure.StartPoint = transformer.TransformToVisible(Point1.X, Point1.Y);                
                bezierSeg.Point1 =  transformer.TransformToVisible(Point2.X, Point2.Y);
                bezierSeg.Point2 = transformer.TransformToVisible(Point3.X, Point3.Y);
                bezierSeg.Point3 = transformer.TransformToVisible(Point4.X, Point4.Y);                
                figure.Segments.Add(bezierSeg);
                segGeometry.Figures = new PathFigureCollection() {figure};
                    var path = this.segPath;
                    if (path != null) path.Data = segGeometry;
                }
                else
                {
                    this.segPath.Data = null;
                }
            }
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

        #endregion
    }
}
