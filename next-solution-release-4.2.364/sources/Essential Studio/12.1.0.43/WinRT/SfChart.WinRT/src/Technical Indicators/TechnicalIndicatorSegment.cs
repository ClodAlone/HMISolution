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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart technical indicator segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FinancialTechnicalIndicator"/>
    [ClassReference(IsReviewed = false)]
    public class TechnicalIndicatorSegment : FastLineSegment
    {
        #region ctor

        /// <summary>
        /// 
        /// </summary>
        public TechnicalIndicatorSegment()
        {

        }

        /// <summary>
        /// Called when instance created for TechnicalIndicatorSegment
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        /// <param name="stroke"></param>
        /// <param name="series"></param>
        public TechnicalIndicatorSegment(List<double> xVals, List<double> yVals, Brush stroke, ChartSeriesBase series)
        {
            Series = series;
            this.xChartVals = xVals;
            this.yChartVals = yVals;
            if (series.DataCount > 1)
            {
                if (series.IsIndexed)
                {
                    double X_MAX = series.DataCount - 1;
                    double Y_MAX = yChartVals.Max();
                    double X_MIN = 0;
                    double Y_MIN = yChartVals.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
                else
                {
                    double X_MAX = xChartVals.Max();
                    double Y_MAX = yChartVals.Max();
                    double X_MIN = xChartVals.Min();
                    double Y_MIN = yChartVals.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
            }
            series.Stroke = stroke;
            fastSeries = series as ChartSeries;
            SetData(xVals, yVals);
            SetRange();
            //Stroke = stroke;
        }

        /// <summary>
        /// Called when instance created for TechnicalIndicatorSegment
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        /// <param name="stroke"></param>
        /// <param name="series"></param>
        /// <param name="length"></param>
        public TechnicalIndicatorSegment(List<double> xVals, List<double> yVals, Brush stroke, ChartSeriesBase series, int length)
        {
            Length = length;
            Series = series;
            this.xChartVals = xVals;
            this.yChartVals = yVals;
            if (series.DataCount > 1)
            {
                if (series.IsIndexed)
                {
                    double X_MAX = series.DataCount - 1;
                    double Y_MAX = yChartVals.Max();
                    double X_MIN = 14;
                    double Y_MIN = yChartVals.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
                else
                {
                    double X_MAX = xChartVals.Max();
                    double Y_MAX = yChartVals.Max();
                    double X_MIN = xChartVals.Min();
                    double Y_MIN = yChartVals.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
            }
            series.Stroke = stroke;
            fastSeries =  series as ChartSeries;
            SetData(xVals, yVals);
            // SetRange();
            //Stroke = stroke;
        }

        #endregion

        #region fields
        
        private Size availableSize;

        private int Length = 0;

        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        public override void SetData(IList<double> xVals, IList<double> yVals)
        {
            this.xChartVals = xVals;
            this.yChartVals = yVals;
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
            UIElement element = base.CreateVisual(size);
            base.polyline.Stroke = this.Stroke;
            return element;
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
            if (Length == 0)
            {
                base.Update(transformer);
            }
            else
            if (transformer != null && fastSeries.DataCount > 1)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                bool isLogarithmic = cartesianTransformer.XAxis.IsLogarithmic || cartesianTransformer.YAxis.IsLogarithmic;
                if (!isLogarithmic)
                {
                    TransformToScreenCo(cartesianTransformer);
                }
                else
                {
                    TransformToScreenCoInLog(cartesianTransformer);
                }

                UpdateVisual(true);
            }
        }

        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        void TransformToScreenCo(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
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

            double width = cartesianTransformer.XAxis.RenderedRect.Width;
            double height = cartesianTransformer.YAxis.RenderedRect.Height;
            availableSize = new Size(width, height);

            double xTolerance = Math.Abs((xDelta * 1) / availableSize.Width);
            double yTolerance = Math.Abs((yDelta * 1) / availableSize.Height);
            double left = cartesianTransformer.XAxis.RenderedRect.Left - fastSeries.Area.SeriesClipRect.Left;
            double top = cartesianTransformer.YAxis.RenderedRect.Top - fastSeries.Area.SeriesClipRect.Top;
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
            if (RenderingMode == RenderingMode.Default)
            {
                Points = new PointCollection();
                double xValue = 0;
                double yValue = 0;
                double prevXValue = 0;
                double prevYValue = 0;

                if (fastSeries.IsIndexed)
                {
                    prevXValue = 1;
                    start = Length;
                    start = start < 0 ? 0 : start;
                    count = count > yChartVals.Count ? yChartVals.Count : count;
                    for (i = start; i <= count; i++)
                    {
                        double yVal = 0;
                        if (i >= 0 && i < yChartVals.Count)
                            yVal = yChartVals[i];
                        else
                            continue;
                        if (Math.Abs(prevXValue - i) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValue = left + (availableSize.Width * ((i - xStart) / xDelta));
                            yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                            Points.Add(new Point(xValue, yValue));
                            prevXValue = i;
                            prevYValue = yVal;
                        }
                    }

                    if (start > 0 && start < yChartVals.Count)
                    {
                        i = start - 1;
                        double yVal = yChartVals[i];
                        xValue = left + (availableSize.Width * ((i - xStart) / xDelta));
                        yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        Points.Insert(0, new Point(xValue, yValue));
                    }

                    if (count < yChartVals.Count - 1)
                    {
                        i = count + 1;
                        double yVal = yChartVals[i];
                        xValue = left + (availableSize.Width * ((i - xStart) / xDelta));
                        yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
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
                                xValue = left + (availableSize.Width * ((xVal - xStart) / xDelta));
                                yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
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
                            xValue = left + (availableSize.Width * ((xVal - xStart) / xDelta));
                            yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                            Points.Add(new Point(xValue, yValue));
                            break;
                        }
                    }

                    if (startIndex > 0)
                    {
                        xValue = left + (availableSize.Width * ((xChartVals[startIndex] - xStart) / xDelta));
                        yValue = top + (availableSize.Height * (1 - ((yChartVals[startIndex] - yStart) / yDelta)));
                        Points.Insert(0, new Point(xValue, yValue));
                    }
                }
            }
            else
            {
                xValues.Clear();
                yValues.Clear();
                double prevXValue = 0;
                double prevYValue = 0;
                float xValue = 0;
                float yValue = 0;
                if (fastSeries.IsIndexed)
                {
                    prevXValue = 1;
                    for (i = start; i <= count; i++)
                    {
                        double yVal = yChartVals[i];

                        if (Math.Abs(prevXValue - i) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValue = (float)(left + availableSize.Width * ((i - xStart) / xDelta));
                            yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                            xValues.Add(xValue);
                            yValues.Add(yValue);
                            prevXValue = i;
                            prevYValue = yVal;
                        }
                    }

                    if (start > 0)
                    {
                        i = start - 1;
                        double yVal = yChartVals[i];
                        xValue = (float)(left + availableSize.Width * ((i - xStart) / xDelta));
                        yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        xValues.Insert(0, xValue);
                        yValues.Insert(0, yValue);
                    }

                    if (count < yChartVals.Count - 1)
                    {
                        i = count + 1;
                        double yVal = yChartVals[i];
                        xValue = (float)(left + availableSize.Width * ((i - xStart) / xDelta));
                        yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        xValues.Add(xValue);
                        yValues.Add(yValue);
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
                                xValue = (float)(left + availableSize.Width * ((xVal - xStart) / xDelta));
                                yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                                xValues.Add(xValue);
                                yValues.Add(yValue);
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
                            xValue = (float)(left + availableSize.Width * ((xVal - xStart) / xDelta));
                            yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                            xValues.Add(xValue);
                            yValues.Add(yValue);
                            break;
                        }
                    }

                    if (startIndex > 0)
                    {
                        xValue = (float)(left + availableSize.Width * ((xChartVals[startIndex] - xStart) / xDelta));
                        yValue = (float)(top + availableSize.Height * (1 - ((yChartVals[startIndex] - yStart) / yDelta)));
                        xValues.Insert(0, xValue);
                        yValues.Insert(0, yValue);
                    }
                }
            }
        }

        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        void TransformToScreenCoInLog(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
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

            double width = cartesianTransformer.XAxis.RenderedRect.Width;
            double height = cartesianTransformer.YAxis.RenderedRect.Height;
            availableSize = new Size(width, height);

            double xTolerance = Math.Abs((xDelta * 1) / availableSize.Width);
            double yTolerance = Math.Abs((yDelta * 1) / availableSize.Height);
            double left = cartesianTransformer.XAxis.RenderedRect.Left - fastSeries.Area.SeriesClipRect.Left;
            double top = cartesianTransformer.YAxis.RenderedRect.Top - fastSeries.Area.SeriesClipRect.Top;

            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;

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
            if (RenderingMode == RenderingMode.Default)
            {
                Points = new PointCollection();
                double xValue = 0;
                double yValue = 0;
                double prevXValue = 0;
                double prevYValue = 0;
                if (fastSeries.IsIndexed)
                {
                    prevXValue = 1;
                    for (i = start; i <= count; i++)
                    {
                        double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                        double xVal = xBase == 1 ? i : Math.Log(i, xBase);

                        if (Math.Abs(prevXValue - i) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValue = left + (availableSize.Width * ((xVal - xStart) / xDelta));
                            yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                            Points.Add(new Point(xValue, yValue));
                            prevXValue = i;
                            prevYValue = yVal;
                        }
                    }

                    if (start > 0)
                    {
                        i = start - 1;
                        double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                        double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                        xValue = left + (availableSize.Width * ((i - xStart) / xDelta));
                        yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        Points.Insert(0, new Point(xValue, yValue));
                    }

                    if (count < yChartVals.Count - 1)
                    {
                        i = count + 1;
                        double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                        double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                        xValue = left + (availableSize.Width * ((i - xStart) / xDelta));
                        yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        Points.Add(new Point(xValue, yValue));
                    }
                }
                else
                {
                    int startIndex = 0;
                    for (i = 0; i < xChartVals.Count; i++)
                    {
                        double xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                        double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);

                        if ((xVal <= count) && (xVal >= start))
                        {
                            if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                            {
                                xValue = left + (availableSize.Width * ((xVal - xStart) / xDelta));
                                yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
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
                            xVal = xBase == 1 ? xVal : Math.Log(xVal, xBase);
                            yVal = yBase == 1 ? yVal : Math.Log(yVal, yBase);
                            xValue = left + (availableSize.Width * ((xVal - xStart) / xDelta));
                            yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                            Points.Add(new Point(xValue, yValue));
                            break;
                        }
                    }

                    if (startIndex > 0)
                    {
                        double xVal = xBase == 1 ? xChartVals[startIndex] : Math.Log(xChartVals[startIndex], xBase);
                        double yVal = yBase == 1 ? yChartVals[startIndex] : Math.Log(yChartVals[startIndex], yBase);
                        xValue = left + (availableSize.Width * ((xVal - xStart) / xDelta));
                        yValue = top + (availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        Points.Insert(0, new Point(xValue, yValue));
                    }
                }
            }
            else
            {
                xValues.Clear();
                yValues.Clear();
                double prevXValue = 0;
                double prevYValue = 0;
                float xValue = 0;
                float yValue = 0;
                if (fastSeries.IsIndexed)
                {
                    prevXValue = 1;
                    for (i = start; i <= count; i++)
                    {
                        double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                        double xVal = xBase == 1 ? i : Math.Log(i, xBase);

                        if (Math.Abs(prevXValue - i) >= 1 || Math.Abs(prevYValue - yVal) >= 1)
                        {
                            xValue = (float)(left + availableSize.Width * ((xVal - xStart) / xDelta));
                            yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                            xValues.Add(xValue);
                            yValues.Add(yValue);
                            prevXValue = i;
                            prevYValue = yVal;
                        }
                    }

                    if (start > 0)
                    {
                        i = start - 1;
                        double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                        double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                        xValue = (float)(left + availableSize.Width * ((xVal - xStart) / xDelta));
                        yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        xValues.Insert(0, xValue);
                        yValues.Insert(0, yValue);
                    }

                    if (count < yChartVals.Count - 1)
                    {
                        i = count + 1;
                        double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                        double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                        xValue = (float)(left + availableSize.Width * ((xVal - xStart) / xDelta));
                        yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        xValues.Add(xValue);
                        yValues.Add(yValue);
                    }
                }
                else
                {
                    int startIndex = 0;
                    for (i = 0; i < xChartVals.Count; i++)
                    {
                        double xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                        double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);

                        if ((xVal <= count) && (xVal >= start))
                        {
                            if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                            {
                                xValue = (float)(left + availableSize.Width * ((xVal - xStart) / xDelta));
                                yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                                xValues.Add(xValue);
                                yValues.Add(yValue);
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
                            xVal = xBase == 1 ? xVal : Math.Log(xVal, xBase);
                            yVal = yBase == 1 ? yVal : Math.Log(yVal, yBase);
                            xValue = (float)(left + availableSize.Width * ((xVal - xStart) / xDelta));
                            yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                            xValues.Add(xValue);
                            yValues.Add(yValue);
                            break;
                        }
                    }

                    if (startIndex > 0)
                    {
                        double xVal = xBase == 1 ? xChartVals[startIndex] : Math.Log(xChartVals[startIndex], xBase);
                        double yVal = yBase == 1 ? yChartVals[startIndex] : Math.Log(yChartVals[startIndex], yBase);
                        xValue = (float)(left + availableSize.Width * ((xVal - xStart) / xDelta));
                        yValue = (float)(top + availableSize.Height * (1 - ((yVal - yStart) / yDelta)));
                        xValues.Insert(0, xValue);
                        yValues.Insert(0, yValue);
                    }
                }
            }
        }

        #endregion
    }
}
