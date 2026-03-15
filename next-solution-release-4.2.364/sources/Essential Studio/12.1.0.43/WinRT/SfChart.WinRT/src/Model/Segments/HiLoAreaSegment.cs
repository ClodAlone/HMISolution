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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Linesegment = Windows.UI.Xaml.Media.LineSegment;

namespace Syncfusion.WinRT.Chart
{
    public class HiLoAreaSegment:RangeAreaSegment
    {
        #region fields

        private ChartPoint hiPoint;
        private ChartPoint loPoint;
        private ChartPoint chartPoint;
        private HiLoSeries hiLoSeries;
        private List<ChartPoint> segPoints;
        private ChartPoint[] chartPoint1;
        private HiLoAreaSeries hiLoAreaSeries;
        private bool isHighLow;

        #endregion

        #region constructor

        public HiLoAreaSegment()
        {

        }

        public HiLoAreaSegment(ChartPoint hiPoint, ChartPoint loPoint, ChartPoint chartPoint, HiLoSeries hiLoSeries)
        {
            XRange = new DoubleRange(hiPoint.X,loPoint.X);
            YRange = new DoubleRange(hiPoint.Y,loPoint.Y);
            this.hiPoint = hiPoint;
            this.loPoint = loPoint;
            this.chartPoint = chartPoint;
            this.hiLoSeries = hiLoSeries;
        }

        public HiLoAreaSegment(List<ChartPoint> segPoints, ChartPoint[] chartPoint1, HiLoAreaSeries hiLoAreaSeries
            , bool isHighLow):base(segPoints,chartPoint1)
        {
            this.segPoints = segPoints;
            this.chartPoint1 = chartPoint1;
            this.hiLoAreaSeries = hiLoAreaSeries;
            this.isHighLow = isHighLow;
        }
        
        private void SetRange(List<ChartPoint> chartPoints)
        {
            double X_MAX = chartPoints.Max(x => x.X);
            double Y_MAX = chartPoints.Max(y => y.Y);
            double X_MIN = chartPoints.Min(x => x.X);
            double Y_MIN = chartPoints.Min(y => y.Y);
            XRange = new DoubleRange(X_MIN, X_MAX);
            YRange = new DoubleRange(Y_MIN, Y_MAX);
        }
        #endregion

        #region methods

        internal override Windows.UI.Xaml.UIElement CreateVisual(Windows.Foundation.Size size)
        {
            areaSegmentPath = new Path();
            areaSegmentPath.Stroke = new SolidColorBrush(Colors.Green);
            areaSegmentPath.Fill = new SolidColorBrush(Colors.Green);
            areaSegmentPath.StrokeThickness = 1;
            return areaSegmentPath;
        }

        internal override Windows.UI.Xaml.UIElement GetRenderedVisual()
        {
            return areaSegmentPath;
        }

        internal override void Update(IChartTransformer transformer)
        {
            PathFigure figure = new PathFigure();

            int startIndex = 1;
            int endIndex = segPoints.Count - 2;

            if (this.hiLoAreaSeries.Segments.IndexOf(this) == 0)
            {
                startIndex = 2;
            }

            if (this.hiLoAreaSeries.Segments.IndexOf(this) == this.hiLoAreaSeries.Segments.Count - 1)
            {
                endIndex = segPoints.Count - 1;
            }

            if (segPoints.Count > 0)
            {
                figure.StartPoint = transformer.TransformToVisible(segPoints[0].X, segPoints[0].Y);

                for (int i = startIndex; i < segPoints.Count; i += 2)
                {
                    Linesegment lnsegment = new Linesegment();
                    lnsegment.Point = transformer.TransformToVisible(segPoints[i].X, segPoints[i].Y);
                    figure.Segments.Add(lnsegment);
                }

                for (int i = endIndex; i >= 1; i -= 2)
                {
                    Linesegment lnsegment = new Linesegment();
                    lnsegment.Point = transformer.TransformToVisible(segPoints[i].X, segPoints[i].Y);
                    figure.Segments.Add(lnsegment);
                }
            }
            figure.IsClosed = true;
            segmentGeometry = new PathGeometry();
            segmentGeometry.Figures.Add(figure);
            areaSegmentPath.Data = segmentGeometry;
        }

        internal override void OnSizeChanged(Windows.Foundation.Size size)
        {
           
        }
        #endregion
    }
}
