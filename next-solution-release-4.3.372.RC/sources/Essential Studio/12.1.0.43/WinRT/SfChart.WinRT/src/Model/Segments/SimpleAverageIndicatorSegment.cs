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

namespace Syncfusion.WinRT.Chart
{
    public class SimpleAverageIndicatorSegment:FastLineSegment
    {
        #region fields

        private IList<double> xChartVals;

        private IList<double> yChartVals;

        private Polyline polyline;

        private Size availableSize;

        SimpleAverageIndicator fastSeries;

        #endregion
        #region Properties

        public PointCollection Points
        {
            get;
            set;
        }

        #endregion

        public SimpleAverageIndicatorSegment(List<double> xVals, List<double> yVals,SimpleAverageIndicator series)
        {
            fastSeries = series;
            this.xChartVals = xVals;
            this.yChartVals = yVals;
            double X_MAX = xChartVals.Max();
            double Y_MAX = yChartVals.Max();
            double X_MIN = xChartVals.Min();
            double Y_MIN = yChartVals.Min();
            XRange = new DoubleRange(X_MIN, X_MAX);
            YRange = new DoubleRange(Y_MIN, Y_MAX);

        }

        internal void SetData(List<double> xVals, List<double> yVals)
        {
            this.xChartVals = xVals;
            this.yChartVals = yVals;
        }

        internal override UIElement CreateVisual(Size size)
        {
            Polyline polyLine = new Polyline();
            polyline = polyLine;
            polyline.Stroke = new SolidColorBrush(Colors.Green);
            polyline.StrokeThickness = 1;
            return polyLine;
        }

        internal override UIElement GetRenderedVisual()
        {
            return polyline;
        }

        internal override void Update(IChartTransformer transformer)
        {

            if (transformer != null)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                int i = 0;
                bool x_isInversed = cartesianTransformer.XAxis.IsInversed;
                bool y_isInversed = cartesianTransformer.YAxis.IsInversed;
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double yStart = cartesianTransformer.YAxis.VisibleRange.Start;
                double yEnd = cartesianTransformer.YAxis.VisibleRange.End;
                double xDelta = x_isInversed ? xStart - xEnd : xEnd - xStart;
                double yDelta = y_isInversed ? yStart - yEnd : yEnd - yStart;
                DoubleRange range = cartesianTransformer.XAxis.VisibleRange;
                double xTolerance = Math.Abs((xDelta * 1) / availableSize.Width);
                double yTolerance = Math.Abs((yDelta * 1) / availableSize.Height);

                int count = (int)(Math.Ceiling(xEnd));
                int start = (int)(Math.Floor(xStart));

                if (x_isInversed)
                {
                    double temp = xStart;
                    xStart = xEnd;
                    xEnd = temp;
                }

                if (y_isInversed)
                {
                    double temp = yStart;
                    yStart = yEnd;
                    yEnd = temp;
                }
                Points = new PointCollection();
                double xValue = 0;
                double yValue = 0;
                double prevXValue = 0;
                double prevYValue = 0;
                if (fastSeries.IsIndexed)
                {
                    for (i = start; i <= count; i++)
                    {
                        double yVal = yChartVals[i];

                        if (Math.Abs(prevXValue - i) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValue = (availableSize.Width * ((i - xStart) / xDelta));
                            yValue = (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                            Points.Add(new Point(xValue, yValue));
                            prevXValue = i;
                            prevYValue = yVal;
                        }
                    }

                    if (start > 0)
                    {
                        i = start - 1;
                        double yVal = yChartVals[i];
                        xValue = (availableSize.Width * ((i - xStart) / xDelta));
                        yValue = (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        Points.Insert(0, new Point(xValue, yValue));
                    }

                    if (count < yChartVals.Count - 1)
                    {
                        i = count + 1;
                        double yVal = yChartVals[i];
                        xValue = (availableSize.Width * ((i - xStart) / xDelta));
                        yValue = (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        Points.Add(new Point(xValue, yValue));
                    }
                }
                else
                {
                    int startIndex = 0;
                    for (i = 0; i < xChartVals.Count; i++)
                    {
                        double xVal = xChartVals[i];
                        double yVal = yChartVals[i];

                        if ((xVal <= count) && (xVal >= start))
                        {
                            if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                            {
                                xValue = (availableSize.Width * ((xVal - xStart) / xDelta));
                                yValue = (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                                Points.Add(new Point(xValue, yValue));
                                prevXValue = xVal;
                                prevYValue = yVal;
                            }
                        }
                        else if (xVal < start)
                        {
                            startIndex = i;
                        }
                        else if (xVal > count)
                        {
                            xValue = (availableSize.Width * ((xVal - xStart) / xDelta));
                            yValue = (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                            Points.Add(new Point(xValue, yValue));
                            break;
                        }
                    }

                    if (startIndex > 0)
                    {
                        xValue = (availableSize.Width * ((xChartVals[startIndex] - xStart) / xDelta));
                        yValue = (availableSize.Height * (1 - ((yChartVals[startIndex] - yStart) / yDelta)));
                        Points.Insert(0, new Point(xValue, yValue));
                    }
                }
                polyline.Points = Points;
            }
        }
        internal override void OnSizeChanged(Windows.Foundation.Size size)
        {
            availableSize = size;
        }
    }
}
