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
    public class StepLineAreaSegment : ChartSegment
    {
        Path stepLineAreaSegmentPath;

        Linesegment lineSegment = new Linesegment();

        PathGeometry segmentGeometry;

        List<ChartPoint> AreaGraphPoints = new List<ChartPoint>();

        #region ctor

        public StepLineAreaSegment()
        {

        }

        public StepLineAreaSegment(List<ChartPoint> pointsCollection,StepLineAreaSeries series)
        {
            SetRange(pointsCollection);
            this.AreaGraphPoints = pointsCollection;
        }
        #endregion

        #region methods

        internal override UIElement CreateVisual(Size size)
        {
            stepLineAreaSegmentPath = new Path();
            stepLineAreaSegmentPath.Stroke = new SolidColorBrush(Colors.Green);
            stepLineAreaSegmentPath.Fill = new SolidColorBrush(Colors.Green);
            stepLineAreaSegmentPath.StrokeThickness = 1;
            return stepLineAreaSegmentPath;
        }

        internal override UIElement GetRenderedVisual()
        {
            return stepLineAreaSegmentPath;
        }

        internal override void Update(IChartTransformer transformer)
        {
                RenderGraph(transformer);
        }

        internal override void OnSizeChanged(Size size)
        {

        }

        private void SetRange(List<ChartPoint> segmentPoints)
        {
            double X_MAX = segmentPoints.Max(x => x.X);
            double Y_MAX = segmentPoints.Max(y => y.Y);
            double X_MIN = segmentPoints.Min(x => x.X);
            double Y_MIN = segmentPoints.Min(y => y.Y);
            XRange = new DoubleRange(X_MIN, X_MAX);
            YRange = new DoubleRange(Y_MIN, Y_MAX);
        }

        private void RenderGraph(IChartTransformer transformer)
        {
            if (transformer != null && this.AreaGraphPoints != null)
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = transformer.TransformToVisible(this.AreaGraphPoints[0].X, this.AreaGraphPoints[0].Y);
                foreach (var item in this.AreaGraphPoints)
                {
                    Linesegment lineSegment = new Linesegment();
                    lineSegment.Point = transformer.TransformToVisible(item.X, item.Y);
                    figure.Segments.Add(lineSegment);
                }
                figure.IsClosed = true;
                segmentGeometry = new PathGeometry();
                segmentGeometry.Figures.Add(figure);
                this.stepLineAreaSegmentPath.Data = segmentGeometry;
            }
        }
        #endregion
    }
}