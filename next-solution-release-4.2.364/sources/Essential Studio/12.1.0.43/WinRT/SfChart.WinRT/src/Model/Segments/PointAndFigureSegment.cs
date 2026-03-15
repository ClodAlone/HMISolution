#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.WinRT.Chart
{
    public class PointAndFigureSegment : ChartSegment
    {
        #region fields

        private ChartPoint[] PointColl1;

        private ChartPoint[] PointColl2;

        private ChartPoint[] correspondingPoints;

        private ChartSeries series;

        private double delta;

        private Path segmentPath;

        private Canvas _Canvas;        

        #endregion

        #region properties

        public ChartPointAndFigure Shape { get; set; }

        public SolidColorBrush StrokeColor { get; set; }

        public GeometryGroup Geometry { get; set; }

        #endregion

        #region constructor

        public PointAndFigureSegment()
        {

        }

        public PointAndFigureSegment(ChartPoint[] ptcoll1, ChartPoint[] ptcoll2, ChartPoint[] correspondingPoints, ChartSeries series, double delta)
        {
            this.PointColl1 = ptcoll1;
            this.PointColl2 = ptcoll2;
            this.correspondingPoints = correspondingPoints;
            this.series = series;
            this.delta = delta;
            SetRange(ptcoll1, ptcoll2);
        }

        private void SetRange(ChartPoint[] ptcoll1, ChartPoint[] ptcoll2)
        {
            double X_MAX = (ptcoll2.ToList()).Max(x => x.X);
            double Y_MAX = (ptcoll2.ToList()).Max(y => y.Y);
            double X_MIN = (ptcoll1.ToList()).Min(x => x.X);
            double Y_MIN = (ptcoll1.ToList()).Min(y => y.Y);
            XRange = new DoubleRange(X_MIN, X_MAX);
            YRange = new DoubleRange(Y_MIN, Y_MAX);
        }

        #endregion

        #region methods
        internal override Windows.UI.Xaml.UIElement CreateVisual(Windows.Foundation.Size size)
        {
            _Canvas = new Canvas();
            segmentPath = new Path();
            segmentPath.Stretch = Stretch.Fill;
            segmentPath.Stroke = new SolidColorBrush(Colors.Green);
            segmentPath.StrokeThickness = 1d;
            _Canvas.Children.Add(segmentPath);
            return _Canvas;
        }

        internal override Windows.UI.Xaml.UIElement GetRenderedVisual()
        {
            return _Canvas;
        }

        internal override void Update(IChartTransformer transformer)
        {

            Point blPoint = transformer.TransformToVisible(this.PointColl1[0].X, this.PointColl1[0].Y);
            Point trPoint = transformer.TransformToVisible(this.PointColl2[this.PointColl2.Length - 1].X, this.PointColl2[this.PointColl2.Length - 1].Y);
            Rect columnRect = new Rect(blPoint, trPoint);

            List<PathFigure> figurecoll = new List<PathFigure>();
            GeometryGroup ellipses = new GeometryGroup();
            double each_ht = columnRect.Height / PointColl1.Count();
            if (this.PointColl1.Length > 0)
            {
                for (int i = 0; i < this.PointColl1.Length; i++)
                {
                    List<Point> pointcoll = new List<Point>();

                    if (this.PointColl1[i] != null && this.Shape == ChartPointAndFigure.Figure)
                    {
                        PathFigure figure = new PathFigure();
                        Point actualpt = transformer.TransformToVisible(this.PointColl1[i].X, this.PointColl1[i].Y);
                        Point linept = transformer.TransformToVisible(this.PointColl2[i].X, this.PointColl2[i].Y);
                        figure.StartPoint = actualpt;
                        Windows.UI.Xaml.Media.LineSegment lnSegment = new Windows.UI.Xaml.Media.LineSegment();
                        lnSegment.Point = linept;
                        figure.IsClosed = false;
                        figure.Segments.Add(lnSegment);                        
                        figurecoll.Add(figure);

                        PathFigure figure1 = new PathFigure();
                        Point actualpt1 = transformer.TransformToVisible(this.PointColl2[i].X, this.PointColl1[i].Y);
                        Point linept1 = transformer.TransformToVisible(this.PointColl1[i].X, this.PointColl2[i].Y);
                        figure1.StartPoint = actualpt1;
                        Windows.UI.Xaml.Media.LineSegment lnSegment1 = new Windows.UI.Xaml.Media.LineSegment();
                        lnSegment1.Point = linept1;
                        figure1.IsClosed = false;
                        figure1.Segments.Add(lnSegment1);                        
                        figurecoll.Add(figure1);
                    }
                    else if (this.PointColl1[i] != null && this.Shape == ChartPointAndFigure.Point)
                    {
                        Point actualpt = transformer.TransformToVisible(this.PointColl1[i].X, this.PointColl1[i].Y);
                        EllipseGeometry ellipse = new EllipseGeometry();
                        ellipse.Center = actualpt;
                        ellipse.RadiusX = (this.PointColl2[i].X - this.PointColl1[i].X) / 2;
                        ellipse.RadiusY = each_ht / 2;
                        ellipses.Children.Add(ellipse);
                    }
                }

            }
            if (this.Shape == ChartPointAndFigure.Point)
            {
                this.StrokeColor = new SolidColorBrush(Colors.Red);
                this.Geometry = ellipses;
            }
            else
            {
                this.StrokeColor = new SolidColorBrush(Colors.Green);
                PathGeometry geomet = new PathGeometry();
                foreach (PathFigure item in figurecoll)
                {
                    geomet.Figures.Add(item);
                }
                ellipses.Children.Add(geomet);
                this.Geometry = ellipses;
            }

            segmentPath.SetValue(Canvas.LeftProperty, columnRect.X);
            segmentPath.SetValue(Canvas.TopProperty, columnRect.Y);
            segmentPath.Width = columnRect.Width;
            segmentPath.Height = columnRect.Height;
            segmentPath.Data = this.Geometry;
        }


        internal override void OnSizeChanged(Windows.Foundation.Size size)
        {

        }

        #endregion



    }
}
