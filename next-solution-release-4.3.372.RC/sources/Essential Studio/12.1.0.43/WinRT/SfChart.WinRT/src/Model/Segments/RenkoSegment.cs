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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Linesegment = Windows.UI.Xaml.Media.LineSegment;

namespace Syncfusion.WinRT.Chart
{
    public class RenkoSegment:ChartSegment
    {

        #region fields

        private ChartPoint chartPoint;
        private RenkoSeries renkoSeries;
        private Windows.Foundation.Rect[] representedRectangles;
       
        private List<Rect> transformedRectangles = new List<Rect>();
        private PathGeometry Geometry;
        private Path segmentPath;

        #endregion

        #region properties

        internal bool IsPriceDown { get; set; }
        internal bool IsPriceUp { get; set; }

        #endregion



        #region constructor

        public RenkoSegment()
        {

        }

        public RenkoSegment(ChartPoint chartPoint, RenkoSeries renkoSeries, Rect[] representedRectangles)
        {
            this.chartPoint = chartPoint;
            this.renkoSeries = renkoSeries;
            this.representedRectangles = representedRectangles;
            XRange = DoubleRange.Empty;
            YRange = DoubleRange.Empty;
            YRange += representedRectangles[representedRectangles.Length - 1].Bottom;
            XRange += representedRectangles[representedRectangles.Length - 1].Left;
            ////Saving range represented by each rectangle. 
            foreach (Rect rectangle in representedRectangles)
            {
                XRange += rectangle.Right;
                YRange += rectangle.Top;
            }
        }

        #endregion


        #region methods

        internal override UIElement CreateVisual(Windows.Foundation.Size size)
        {
            this.segmentPath = new Path();
            this.segmentPath.Stroke=this.segmentPath.Fill = new SolidColorBrush(Colors.Green);
            this.segmentPath.StrokeThickness = 2;            
            return segmentPath;
        }

        internal override UIElement GetRenderedVisual()
        {
            return segmentPath;
        }

        internal override void Update(IChartTransformer transformer)
        {
            bool shouldReassignGeometry = false;
            PathFigure figure;
            PathFigure[] figures = new PathFigure[this.representedRectangles.Length];
            for (int i = 0; i < this.representedRectangles.Length; i++)
            {
                ////Retrieving real coordinates of renko rectangle in chart area presenter.
                Point leftTopPoint = transformer.TransformToVisible(this.representedRectangles[i].Left, this.representedRectangles[i].Top);
                Point rightBottomPoint = transformer.TransformToVisible(this.representedRectangles[i].Right, this.representedRectangles[i].Bottom);
                ////Renko rectangle in real coordinates.
                Rect rectangle = new Rect(leftTopPoint, rightBottomPoint);
                if (!this.transformedRectangles.Contains(rectangle))
                {
                    if (this.transformedRectangles.Count - 1 >= i)
                    {
                       this.transformedRectangles[i] = rectangle; ////.Add(rectangle);
                    }
                    else
                    {
                        this.transformedRectangles.Add(rectangle);
                    }
                    shouldReassignGeometry = true;
                }
                ////Building rectangle figure.
                figure = new PathFigure();
                figure.StartPoint = new Point(rectangle.Left, rectangle.Top);
                Linesegment lnSeg = new Linesegment();
                lnSeg.Point = new Point(rectangle.Right,rectangle.Top);
                figure.Segments.Add(lnSeg);                
                Linesegment lnSeg2 = new Linesegment();
                lnSeg2.Point = new Point(rectangle.Right, rectangle.Bottom);
                figure.Segments.Add(lnSeg2);
                Linesegment lnSeg3 = new Linesegment();
                lnSeg3.Point = new Point(rectangle.Left, rectangle.Bottom);
                figure.Segments.Add(lnSeg3);                
                figure.IsClosed = true;
                figures[i] = figure;
            }

           if (shouldReassignGeometry)
            {
           
                this.Geometry = new PathGeometry();

                for (int i = 0; i < figures.Length; i++)
                {
                    this.Geometry.Figures.Add(figures[i]);
                }
                this.segmentPath.Stroke = this.IsPriceDown ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
                this.segmentPath.Fill = this.IsPriceDown ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
                this.segmentPath.Data = this.Geometry;

            }

           
        }

        internal override void OnSizeChanged(Windows.Foundation.Size size)
        {
            
        }

        #endregion
    }
}
