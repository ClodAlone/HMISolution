#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Data;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart fast line bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastLineBitmapSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastLineBitmapSegment : ChartSegment
    {
        #region fields

        private IList<double> xChartVals;

        private IList<double> yChartVals;

        private Brush stroke;

        private int[] points1;

        private int[] points2;

        private Point intersectingPoint;

        private WriteableBitmap bitmap;

#if !WINDOWS_PHONE
        private byte[] fastBuffer;
#endif

        internal ChartSeries fastSeries;

        private Size availableSize;

        private bool x_isInversed, y_isInversed;

        private double xStart, xEnd, yStart, yEnd, xDelta, yDelta, xSize, ySize, xOffset, yOffset;

        private double xTolerance, yTolerance;

        private int count, start;

        #endregion

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public FastLineBitmapSegment()
        {

        }

        /// <summary>
        /// Called when instance created for FastLineBitmapsegment
        /// </summary>
        /// <param name="series"></param>
        public FastLineBitmapSegment(ChartSeriesBase series)
        {
            stroke = series.Stroke;
            fastSeries = series as ChartSeries;
        }

        /// <summary>
        /// Called when instance created for FastLineBitmapSegment with folloeing arguments
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        /// <param name="series"></param>
        public FastLineBitmapSegment(IList<double> xVals, IList<double> yVals, AdornmentSeries series)
            : this(series)
        {
            base.Series = series;
            this.xChartVals = xVals;
            this.yChartVals = yVals;
            if (series.DataCount > 1)
            {
                if (series.IsIndexed)
                {
                    double X_MAX = series.DataCount - 1;
                    double Y_MAX = yChartVals.Max();
                    double X_MIN = 0;
                    double Y_MIN = 0;
                    var Y_LIST = yChartVals.Cast<double>()
                        .Where(n => !double.IsNaN(n));
                    if (Y_LIST.Count() > 0)
                        Y_MIN = Y_LIST.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
                else
                {
                    double X_MAX = xChartVals.Max();
                    double Y_MAX = yChartVals.Max();
                    double X_MIN = xChartVals.Min();
                    double Y_MIN = 0;
                    var Y_LIST = yChartVals.Cast<double>()
                        .Where(n => !double.IsNaN(n));
                    if (Y_LIST.Count() > 0)
                        Y_MIN = Y_LIST.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        [ClassReference(IsReviewed = false)]
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
            bitmap = fastSeries.Area.GetFastRenderSurface();

#if !WINDOWS_PHONE
            fastBuffer = fastSeries.Area.GetFastBuffer();
#endif
            return null;
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
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        public override void OnSizeChanged(Size size)
        {
            availableSize = size;
            bitmap = fastSeries.Area.GetFastRenderSurface();
#if !WINDOWS_PHONE
            fastBuffer = fastSeries.Area.GetFastBuffer();
#endif
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        private List<double> xValues = new List<double>();
        private List<double> yValues = new List<double>();

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            bitmap = fastSeries.Area.GetFastRenderSurface();
            //if (transformer != null && chartPoints != null && chartPoints.Count > 1)
            if (transformer != null && fastSeries.DataCount > 1)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer =
                    transformer as ChartTransform.ChartCartesianTransformer;
                bool isLogarithmic = cartesianTransformer.XAxis.IsLogarithmic ||
                                     cartesianTransformer.YAxis.IsLogarithmic;
                x_isInversed = cartesianTransformer.XAxis.IsInversed;
                y_isInversed = cartesianTransformer.YAxis.IsInversed;
                xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                yStart = cartesianTransformer.YAxis.VisibleRange.Start;
                yEnd = cartesianTransformer.YAxis.VisibleRange.End;
                xDelta = x_isInversed ? xStart - xEnd : xEnd - xStart;
                yDelta = y_isInversed ? yStart - yEnd : yEnd - yStart;
                if (fastSeries.IsActualTransposed)
                {
                    ySize = cartesianTransformer.YAxis.RenderedRect.Width;
                    xSize = cartesianTransformer.XAxis.RenderedRect.Height;
                    yOffset = cartesianTransformer.YAxis.RenderedRect.Left - fastSeries.Area.SeriesClipRect.Left;
                    xOffset = cartesianTransformer.XAxis.RenderedRect.Top - fastSeries.Area.SeriesClipRect.Top;
                }
                else
                {
                    ySize = cartesianTransformer.YAxis.RenderedRect.Height;
                    xSize = cartesianTransformer.XAxis.RenderedRect.Width;
                    yOffset = cartesianTransformer.YAxis.RenderedRect.Top - fastSeries.Area.SeriesClipRect.Top;
                    xOffset = cartesianTransformer.XAxis.RenderedRect.Left - fastSeries.Area.SeriesClipRect.Left;
                }
                xTolerance = Math.Abs((xDelta*1)/xSize);
                yTolerance = Math.Abs((yDelta*1)/ySize);
                count = (int) (Math.Ceiling(xEnd));
                start = (int) (Math.Floor(xStart));
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
                xValues.Clear();
                yValues.Clear();
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
        private void TransformToScreenCo(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            if (!fastSeries.IsActualTransposed)
            {
                TransformToScreenCoHorizontal();
            }
            else
            {
                TransformToScreenCoVertical();
            }
        }

        private void TransformToScreenCoHorizontal()
        {
            int i = 0;
            double prevXValue = 0;
            double prevYValue = 0;
            if (fastSeries.IsIndexed)
            {
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                prevXValue = 1;
                for (i = start; i <= count; i++)
                {
                    double yVal = yChartVals[i];
                    if (Math.Abs(prevXValue - i) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                    {

                        xValues.Add((xOffset + xSize*((i - xStart)/xDelta)));
                        yValues.Add((yOffset + ySize*(1 - ((yVal - yStart)/yDelta))));
                        prevXValue = i;
                        prevYValue = yVal;
                    }
                }

                if (start > 0)
                {
                    i = start - 1;
                    double yVal = yChartVals[i];
                    xValues.Insert(0, (xOffset + xSize*((i - xStart)/xDelta)));
                    yValues.Insert(0, (yOffset + ySize*(1 - ((yVal - yStart)/yDelta))));
                }

                if (count < yChartVals.Count - 1)
                {
                    i = count + 1;
                    double yVal = yChartVals[i];
                    xValues.Add(xOffset + xSize*((i - xStart)/xDelta));
                    yValues.Add((yOffset + ySize*(1 - ((yVal - yStart)/yDelta))));
                }
            }
            else
            {
                int startIndex = 0;
                prevXValue = xChartVals[0];
                prevYValue = yChartVals[0];
                int cnt = xChartVals.Count - 1;
                for (i = 1; i < cnt; i++)
                {
                    double xVal = xChartVals[i];
                    double yVal = yChartVals[i];

                    if ((xVal <= count) && (xVal >= start))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValues.Add((xOffset + xSize*((xVal - xStart)/xDelta)));
                            yValues.Add((yOffset + ySize*(1 - ((yVal - yStart)/yDelta))));
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
                        xValues.Add((xOffset + xSize*((xVal - xStart)/xDelta)));
                        yValues.Add((yOffset + ySize*(1 - ((yVal - yStart)/yDelta))));
                        break;
                    }
                }

                xValues.Insert(0, xOffset + xSize*((xChartVals[startIndex] - xStart)/xDelta));
                yValues.Insert(0, yOffset + ySize*(1 - ((yChartVals[startIndex] - yStart)/yDelta)));

                if (i == cnt)
                {
                    xValues.Add(xOffset + xSize * ((xChartVals[cnt] - xStart) / xDelta));
                    yValues.Add(yOffset + ySize * (1 - ((yChartVals[cnt] - yStart) / yDelta)));
                }
            }
        }

        private void TransformToScreenCoVertical()
        {
            double prevXValue = 0;
            double prevYValue = 0;
            int i = 0;
            if (fastSeries.IsIndexed)
            {
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                prevXValue = 1;
                for (i = start; i <= count; i++)
                {
                    double yVal = yChartVals[i];
                    if (Math.Abs(prevXValue - i) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                    {
                        xValues.Add((xOffset + xSize*((xEnd - i)/xDelta)));
                        yValues.Add((yOffset + ySize*(1 - ((yEnd - yVal)/yDelta))));
                        prevXValue = i;
                        prevYValue = yVal;
                    }
                }

                if (start > 0)
                {
                    i = start - 1;
                    double yVal = yChartVals[i];
                    xValues.Insert(0, (xOffset + xSize*((xEnd - i)/xDelta)));
                    yValues.Insert(0, (yOffset + ySize*(1 - ((yEnd - yVal))/yDelta)));
                }

                if (count < yChartVals.Count - 1)
                {
                    i = count + 1;
                    double yVal = yChartVals[i];
                    xValues.Add(xOffset + xSize*((xEnd - i)/xDelta));
                    yValues.Add((yOffset + ySize*(1 - ((yEnd - yVal)/yDelta))));
                }
            }
            else
            {
                int startIndex = 0;
                
                prevXValue = xChartVals[0];
                prevYValue = yChartVals[0];
                int cnt = xChartVals.Count - 1;
                for (i = 1; i < cnt; i++)
                {
                    double xVal = xChartVals[i];
                    double yVal = yChartVals[i];

                    if ((xVal <= count) && (xVal >= start))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValues.Add((xOffset + xSize*((xEnd - xVal)/xDelta)));
                            yValues.Add((yOffset + ySize*(1 - ((yEnd - yVal)/yDelta))));
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
                        xValues.Add((xOffset + xSize*((xEnd - xVal)/xDelta)));
                        yValues.Add((yOffset + ySize*(1 - ((yEnd - yVal)/yDelta))));
                        break;
                    }
                }

                xValues.Insert(0, xOffset + xSize*((xEnd - xChartVals[startIndex])/xDelta));
                yValues.Insert(0, yOffset + ySize*(1 - (((yEnd - yChartVals[startIndex]))/yDelta)));
                if (i == cnt)
                {
                    xValues.Add(xOffset + xSize * ((xEnd - xChartVals[cnt]) / xDelta));
                    yValues.Add(yOffset + ySize * (1 - (((yEnd - yChartVals[cnt])) / yDelta)));
                }
            }
        }

        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        private void TransformToScreenCoInLog(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            double xBase = cartesianTransformer.XAxis.IsLogarithmic
                               ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase
                               : 1;
            double yBase = cartesianTransformer.YAxis.IsLogarithmic
                               ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase
                               : 1;
            if (!fastSeries.IsActualTransposed)
            {
                TransformToScreenCoInLogHorizontal(xBase, yBase);
            }
            else
            {
                TransformToScreenCoInLogVertical(xBase, yBase);
            }
        }

        private void TransformToScreenCoInLogVertical(double xBase, double yBase)
        {
            int i = 0, startIndex = 0;
            double prevXValue = 0;
            double prevYValue = 0;
            if (fastSeries.IsIndexed)
            {
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                prevXValue = 1;
                for (i = start; i <= count; i++)
                {
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);

                    if (Math.Abs(prevXValue - i) >= 1 || Math.Abs(prevYValue - yVal) >= 1)
                    {
                        xValues.Add((xOffset + xSize*((xEnd - xVal)/xDelta)));
                        yValues.Add((yOffset + ySize*(1 - ((yEnd - yVal)/yDelta))));
                        prevXValue = i;
                        prevYValue = yVal;
                    }
                }

                if (start > 0)
                {
                    i = start - 1;
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                    xValues.Add((xOffset + xSize*((xEnd - xVal)/xDelta)));
                    yValues.Add((yOffset + ySize*(1 - ((yEnd - yVal)/yDelta))));
                }

                if (count < yChartVals.Count - 1)
                {
                    i = count + 1;
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                    xValues.Add((xOffset + xSize*((xEnd - xVal)/xDelta)));
                    yValues.Add((yOffset + ySize*(1 - ((yEnd - yVal)/yDelta))));
                }
            }
            else
            {
                double xVal, yVal;
                prevXValue = xBase == 1 ? xChartVals[0] : Math.Log(xChartVals[0], xBase); ;
                prevYValue = yBase == 1 ? yChartVals[0] : Math.Log(yChartVals[0], yBase);
                int cnt = xChartVals.Count - 1;
                for (i = 1; i < cnt; i++)
                {
                    xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);

                    if ((xVal <= count) && (xVal >= start))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValues.Add((xOffset + xSize*((xEnd - xVal)/xDelta)));
                            yValues.Add((yOffset + ySize*(1 - ((yEnd - yVal)/yDelta))));
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
                        xVal = xBase == 1 ? xVal : Math.Pow(xVal, xBase);
                        yVal = yBase == 1 ? yVal : Math.Pow(yVal, yBase);
                        xValues.Add((xOffset + xSize*((xEnd - xVal)/xDelta)));
                        yValues.Add((yOffset + ySize*(1 - ((yEnd - yVal)/yDelta))));
                        break;
                    }
                }
                xVal = xBase == 1 ? xChartVals[startIndex] : Math.Log(xChartVals[startIndex], xBase);
                yVal = yBase == 1 ? yChartVals[startIndex] : Math.Log(yChartVals[startIndex], yBase);
                xValues.Insert(0, xOffset + xSize * ((xEnd - xVal) / xDelta));
                yValues.Insert(0, yOffset + ySize * (1 - ((yEnd- yVal) / yDelta)));

                if (i == cnt)
                {
                    xVal = xBase == 1 ? xChartVals[cnt] : Math.Log(xChartVals[cnt], xBase);
                    yVal = yBase == 1 ? yChartVals[cnt] : Math.Log(yChartVals[cnt], yBase);
                    xValues.Add(xOffset + xSize * ((xEnd-xVal) / xDelta));
                    yValues.Add(yOffset + ySize * (1 - ((yEnd - yVal) / yDelta)));
                }
            }
        }

        private void TransformToScreenCoInLogHorizontal(double xBase, double yBase)
        {
            int i = 0, startIndex = 0;
            double prevXValue = 0;
            double prevYValue = 0;
            if (fastSeries.IsIndexed)
            {
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                prevXValue = 1;
                for (i = start; i <= count; i++)
                {
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);

                    if (Math.Abs(prevXValue - i) >= 1 || Math.Abs(prevYValue - yVal) >= 1)
                    {
                        xValues.Add((xOffset + xSize*((xVal - xStart)/xDelta)));
                        yValues.Add((yOffset + ySize*(1 - ((yVal - yStart)/yDelta))));
                        prevXValue = i;
                        prevYValue = yVal;
                    }
                }

                if (start > 0)
                {
                    i = start - 1;
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                    xValues.Add((xOffset + xSize*((xVal - xStart)/xDelta)));
                    yValues.Add((yOffset + ySize*(1 - ((yVal - yStart)/yDelta))));
                }

                if (count < yChartVals.Count - 1)
                {
                    i = count + 1;
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                    xValues.Add((xOffset + xSize*((xVal - xStart)/xDelta)));
                    yValues.Add((yOffset + ySize*(1 - ((yVal - yStart)/yDelta))));
                }
            }
            else
            {
                double xVal, yVal;
                prevXValue = xBase == 1 ? xChartVals[0] : Math.Log(xChartVals[0], xBase); ;
                prevYValue = yBase == 1 ? yChartVals[0] : Math.Log(yChartVals[0], yBase);
                int cnt = xChartVals.Count - 1;
                for (i = 1; i < cnt; i++)
                {
                    xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);

                    if ((xVal <= count) && (xVal >= start))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValues.Add((xOffset + xSize*((xVal - xStart)/xDelta)));
                            yValues.Add((yOffset + ySize*(1 - ((yVal - yStart)/yDelta))));
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
                        xVal = xBase == 1 ? xVal : Math.Pow(xVal, xBase);
                        yVal = yBase == 1 ? yVal : Math.Pow(yVal, yBase);
                        xValues.Add((xOffset + xSize*((xVal - xStart)/xDelta)));
                        yValues.Add((yOffset + ySize*(1 - ((yVal - yStart)/yDelta))));
                        break;
                    }

                }
                xVal = xBase == 1 ? xChartVals[startIndex] : Math.Log(xChartVals[startIndex], xBase);
                yVal = yBase == 1 ? yChartVals[startIndex] : Math.Log(yChartVals[startIndex], yBase);
                xValues.Insert(0, xOffset + xSize * ((xVal - xStart) / xDelta));
                yValues.Insert(0, yOffset + ySize * (1 - ((yVal - yStart) / yDelta)));

                if (i == cnt)
                {
                    xVal = xBase == 1 ? xChartVals[cnt] : Math.Log(xChartVals[cnt], xBase);
                    yVal = yBase == 1 ? yChartVals[cnt] : Math.Log(yChartVals[cnt], yBase);
                    xValues.Add(xOffset + xSize * ((xVal - xStart) / xDelta));
                    yValues.Add(yOffset + ySize * (1 - ((yVal - yStart) / yDelta)));
                }
            }

        }

        internal void SetRange()
        {
            if (fastSeries.DataCount > 1)
            {
                if (fastSeries.IsIndexed)
                {
                    double X_MAX = fastSeries.DataCount - 1;
                    double Y_MAX = yChartVals.Max();
                    double X_MIN = 0;
                    double Y_MIN = 0;
                    var Y_LIST = yChartVals.Cast<double>()
                        .Where(n => !double.IsNaN(n));
                    if (Y_LIST.Count() > 0)
                        Y_MIN = Y_LIST.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
                else
                {
                    double X_MAX = xChartVals.Max();
                    double Y_MAX = yChartVals.Max();
                    double X_MIN = xChartVals.Min();
                    double Y_MIN = 0;
                    var Y_LIST = yChartVals.Cast<double>()
                        .Where(n => !double.IsNaN(n));
                    if (Y_LIST.Count() > 0)
                        Y_MIN = Y_LIST.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
            }
        }

        internal void UpdateSegment(int index, NotifyCollectionChangedAction action, IChartTransformer transformer)
        {
        }

        internal void UpdateVisual(bool updatePolyline)
        {
            double xStart = 0;
            double yStart = 0;
            bool isMultiColor = fastSeries.Palette != ChartColorPalette.None;
            Color color = isMultiColor
                              ? (fastSeries.GetInteriorColor(0) as SolidColorBrush).Color
                              : ((SolidColorBrush) this.Interior).Color;
            //byte[] fastBuffer = new byte[(int)(availableSize.Width * availableSize.Height * 4)];

            if (bitmap != null && xValues.Count > 1)
            {
#if !WINDOWS_PHONE
                fastBuffer = fastSeries.Area.GetFastBuffer();
#endif

                xStart = xValues[0];
                yStart = yValues[0];

                int width = (int) fastSeries.Area.SeriesClipRect.Width;
                int height = (int) fastSeries.Area.SeriesClipRect.Height;

                int leftThickness = (int) fastSeries.StrokeThickness/2;
                int rightThickness = (int) (fastSeries.StrokeThickness%2 == 0
                                                ? (fastSeries.StrokeThickness/2) - 1
                                                : fastSeries.StrokeThickness/2);

#if WPF
                bitmap.BeginWrite();
#endif

                if (fastSeries is FastLineBitmapSeries)
                {
                    var fastLineBitmapSeries = (FastLineBitmapSeries) fastSeries;

                    if (((FastLineBitmapSeries) fastSeries).EnableAntiAliasing)
                    {
                        if (fastLineBitmapSeries.StrokeDashArray == null
                            ||
                            (fastLineBitmapSeries.StrokeDashArray != null &&
                             fastLineBitmapSeries.StrokeDashArray.Count <= 1))
                        {
                            if ((fastSeries.IsActualTransposed))
                                DrawLineAa(yValues, xValues, width, height, color, leftThickness, rightThickness,
                                                   isMultiColor);
                            else
                            {
                                DrawLineAa(xValues, yValues, width, height, color, leftThickness, rightThickness,
                                                     isMultiColor);
                            }
                        }
                        else
                            DrawDashedAaLines(width, height, color, leftThickness, rightThickness);
                    }
                    else
                    {
                        if (fastLineBitmapSeries.StrokeDashArray == null
                            ||
                            (fastLineBitmapSeries.StrokeDashArray != null &&
                             fastLineBitmapSeries.StrokeDashArray.Count <= 1))
                        {
                            if (fastSeries.IsActualTransposed)
                                DrawLine(yValues, xValues, width, height, color, leftThickness,
                                                          rightThickness, isMultiColor);
                            else
                            {

                                DrawLine(xValues, yValues, width, height, color, leftThickness,
                                                            rightThickness, isMultiColor);
                            }
                        }
                        else
                            DrawDashedLines(width, height, color, leftThickness, rightThickness);
                    }
                }

#if WPF
                bitmap.EndWrite();
#endif
                //stream.Position = 0;
                //stream.Write(fastBuffer, 0, fastBuffer.Count());
            }

            fastSeries.Area.CanRenderToBuffer = true;
        }

        private void DrawLine(List<double> xVals, List<double> yVals, int width, int height, Color color,
                              int leftThickness, int rightThickness, bool isMultiColor)
        {
            xStart = xVals[0];
            yStart = yVals[0];
            double xEnd = 0;
            double yEnd = 0;
            if (fastSeries.StrokeThickness <= 1)
            {
                for (int i = 1; i < xVals.Count; i++)
                {
                    xEnd = xVals[i];
                    yEnd = yVals[i];
                    if (double.IsNaN(yEnd) || double.IsNaN(yStart) || double.IsNaN(xEnd) || double.IsNaN(xStart))
                    {
                        xStart = xEnd;
                        yStart = yEnd;
                        continue;
                    }
#if WINDOWS_PHONE
                    bitmap.DrawLineBresenham((int)xStart, (int)yStart, (int)xEnd, (int)yEnd, color);
#else
                    bitmap.DrawLineBresenham(fastBuffer, width, height, (int)xStart, (int)yStart, (int)xEnd, (int)yEnd, color);
#endif
                    xStart = xEnd;
                    yStart = yEnd;
                    if (isMultiColor)
                        color = (fastSeries.GetInteriorColor(i) as SolidColorBrush).Color;
                }
            }
            else
            {
                if (points1 == null)
                    points1 = new int[10];
                xEnd = xVals[1];
                yEnd = yVals[1];
                GetLinePoints(xStart, yStart, xEnd, yEnd, leftThickness, rightThickness, points1);
                for (int i = 1; i < xVals.Count; )
                {
                    points2 = new int[10];
                    xStart = xEnd;
                    yStart = yEnd;
                    i++;
                    if (i < xVals.Count)
                    {
                        xEnd = xVals[i];
                        yEnd = yVals[i];
                        UpdatePoints2(xStart, yStart, xEnd, yEnd, leftThickness, rightThickness);
                    }
                    DrawLine(color, width, height);
                    points1 = points2;
                    points2 = null;
                    if (isMultiColor)
                        color = (fastSeries.GetInteriorColor(i) as SolidColorBrush).Color;
                }

                points1 = null;
                points2 = null;
            }
        }

        private void UpdatePoints2(double xStart, double yStart, double xEnd, double yEnd,
            int leftThickness,int rightThickness)
        {
            GetLinePoints(xStart, yStart, xEnd, yEnd, leftThickness, rightThickness, points2);

            bool isIntersecting = FindIntersectingPoint(points1[0], points1[1], points1[2], points1[3]
                                                        , points2[0], points2[1], points2[2], points2[3]);

            if (isIntersecting)
            {
                var diff1 = intersectingPoint.X - points1[2];
                var diff2 = points2[0] - points1[2];
                bool canSwap = false;
                if (diff1 < 0)
                    canSwap = diff1 >= -3;
                else
                    canSwap = diff2 >= diff1 || (diff1 - diff2) <= 3;
                if (canSwap)
                {
                    points1[2] = (int)intersectingPoint.X;
                    points1[3] = (int)intersectingPoint.Y;

                    points2[0] = (int)intersectingPoint.X;
                    points2[1] = (int)intersectingPoint.Y;

                    points2[8] = (int)intersectingPoint.X;
                    points2[9] = (int)intersectingPoint.Y;
                }
            }

            isIntersecting = FindIntersectingPoint(points1[6], points1[7], points1[4], points1[5]
                                                   , points2[6], points2[7], points2[4], points2[5]);

            if (isIntersecting)
            {
                var diff1 = intersectingPoint.X - points1[4];
                var diff2 = points2[6] - points1[4];
                bool canSwap;
                if (diff1 < 0)
                    canSwap = diff1 >= -3;
                else
                    canSwap = diff2 >= diff1 || (diff1 - diff2) <= 3;
                if (canSwap)
                {
                    points1[4] = (int)intersectingPoint.X;
                    points1[5] = (int)intersectingPoint.Y;

                    points2[6] = (int)intersectingPoint.X;
                    points2[7] = (int)intersectingPoint.Y;
                }
            }
        }

       private void GetLinePoints(double x1, double y1, double x2, double y2,
                                    int leftThickness, int rightThickness, int[] points)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            var radian = Math.Atan2(dy, dx);
            var cos = Math.Cos(-radian);
            var sin = Math.Sin(-radian);
            var x11 = (x1*cos) - (y1*sin);
            var y11 = (x1*sin) + (y1*cos);
            var x12 = (x2*cos) - (y2*sin);
            var y12 = (x2*sin) + (y2*cos);
            cos = Math.Cos(radian);
            sin = Math.Sin(radian);
            var leftTopX = (x11*cos) - ((y11 + leftThickness)*sin);
            var leftTopY = (x11*sin) + ((y11 + leftThickness)*cos);
            var rightTopX = (x12*cos) - ((y12 + leftThickness)*sin);
            var rightTopY = (x12*sin) + ((y12 + leftThickness)*cos);
            var leftBottomX = (x11 * cos) - ((y11 - rightThickness) * sin);
            var leftBottomY = (x11 * sin) + ((y11 - rightThickness) * cos);
            var rightBottomX = (x12 * cos) - ((y12 - rightThickness) * sin);
            var rightBottomY = (x12 * sin) + ((y12 - rightThickness) * cos);
            points[0] = (int) leftTopX;
            points[1] = (int) leftTopY;
            points[2] = (int) rightTopX;
            points[3] = (int) rightTopY;
            points[4] = (int) rightBottomX;
            points[5] = (int) rightBottomY;
            points[6] = (int) leftBottomX;
            points[7] = (int) leftBottomY;
            points[8] = (int) leftTopX;
            points[9] = (int) leftTopY;
        }

        private bool FindIntersectingPoint(Point point11, Point point12, Point point21, Point point22)
        {
            double d = (point22.Y - point21.Y)*(point12.X - point11.X) -
                       (point22.X - point21.X)*(point12.Y - point11.Y);
            double na = (point22.X - point21.X)*(point11.Y - point21.Y) -
                        (point22.Y - point21.Y)*(point11.X - point21.X);
            //double nb = (point12.X - point11.X)*(point11.Y - point21.Y) -
            //            (point12.Y - point11.Y)*(point11.X - point21.X);

            if (d == 0 || d == 1 || d == -1)
                return false;

            double ua = na/d;
            //double ub = nb/d;

            intersectingPoint = new Point(point11.X + (ua*(point12.X - point11.X))
                                          , point11.Y + (ua*(point12.Y - point11.Y)));


            if (point11.X == point12.X)
            {
                return !(point21.X == point22.X && point11.X != point21.X);
            }
            else if (point21.X == point22.X)
            {
                return true;
            }
            else
            {
                // both lines are not parallel to the y-axis
                var m1 = (point11.Y - point12.Y) / (point11.X - point12.X);
                var m2 = (point21.Y - point22.Y) / (point21.X - point22.X);
                return m1 != m2;
            }
        }

        private bool FindIntersectingPoint(int x11, int y11, int x12, int y12,
                                           int x21, int y21, int x22, int y22)
        {
            double d = (y22 - y21)*(x12 - x11) -
                       (x22 - x21)*(y12 - y11);
            double na = (x22 - x21)*(y11 - y21) -
                        (y22 - y21)*(x11 - x21);
            //double nb = (point12.X - point11.X)*(point11.Y - point21.Y) -
            //            (point12.Y - point11.Y)*(point11.X - point21.X);

            if (d == 0 || d == 1 || d == -1)
                return false;

            double ua = na/d;
            //double ub = nb/d;

            intersectingPoint = new Point(x11 + (ua*(x12 - x11))
                                          , y11 + (ua*(y12 - y11)));


            if (x11 == x12)
            {
                return !(x21 == x22 && x11 != x21);
            }
            if (x21 == x22)
            {
                return true;
            }
            // both lines are not parallel to the y-axis
            var m1 = ((double) (y11 - y12)/(double) (x11 - x12));
            var m2 = ((double) (y21 - y22)/(double) (x21 - x22));
            return m1 != m2;
        }

        private void DrawLineAa(List<double> xVals, List<double> yVals, int width, int height, Color color, int leftThickness,
                                int rightThickness, bool isMultiColor)
        {
            xStart = xVals[0];
            yStart = yVals[0];
            double xEnd = 0;
            double yEnd = 0;
            if (fastSeries.StrokeThickness <= 1)
            {
                for (int i = 1; i <= xVals.Count - 1; i++)
                {
                    xEnd = xVals[i];
                    yEnd = yVals[i];

#if WINDOWS_PHONE
                    bitmap.DrawLineAa((int)xStart, (int)yStart, (int)xEnd, (int)yEnd, color);
#else
                bitmap.DrawLineAa(fastBuffer, width, height, (int)xStart, (int)yStart, (int)xEnd, (int)yEnd, color);
#endif
                    if (isMultiColor)
                        color = (fastSeries.GetInteriorColor(i) as SolidColorBrush).Color;
                    xStart = xEnd;
                    yStart = yEnd;

                }
            }
            else
            {
                if (points1 == null)
                    points1 = new int[10];
                xEnd = xVals[1];
                yEnd = yVals[1];
                GetLinePoints(xStart, yStart, xEnd, yEnd, leftThickness, rightThickness, points1);
                for (int i = 1; i < xVals.Count; )
                {
                    points2 = new int[10];
                    xStart = xEnd;
                    yStart = yEnd;
                    i++;
                    if (i < xVals.Count)
                    {
                        xEnd = xVals[i];
                        yEnd = yVals[i];
                        UpdatePoints2(xStart, yStart, xEnd, yEnd, leftThickness, rightThickness);
                    }
                    DrawLineAa(color, width, height);
                    points1 = points2;
                    points2 = null;
                    if (isMultiColor)
                        color = (fastSeries.GetInteriorColor(i) as SolidColorBrush).Color;
                }

                points1 = null;
                points2 = null;
            }
        }

        private void DrawDashedAaLines(int width, int height, Color color, int leftThickness,
                                       int rightThickness)
        {
            if (!fastSeries.IsActualTransposed)
                DrawDashedAaLines(xValues, yValues, width, height,color, leftThickness, rightThickness);
            else
                DrawDashedAaLines(yValues, xValues, width, height, color, leftThickness, rightThickness);
        }

        private void DrawDashedAaLines(List<double> xVals, List<double> yVals, int width, int height, Color color,
                                       int leftThickness,
                                       int rightThickness)
        {
            var bitmapSeries = (FastLineBitmapSeries)fastSeries;
            double multiplier = bitmapSeries.StrokeThickness;
            double xEnd;
            double yEnd;
            xStart = xVals[0];
            yStart = yVals[0];
            DoubleCollection dashes = bitmapSeries.StrokeDashArray;
            bool isDash = true;
            double currentLen = dashes[0] * multiplier;
            double x1, y1, x2, y2;
            int dashIndex = 1;
            bool moveToNext = false;
            bool hasRemaining = true;
            bool isBeginning = true;
            bool isPreviousDash = false;
            if (points1 == null)
                points1 = new int[10];
            for (int i = 1; i < xVals.Count; i++)
            {
                xEnd = xVals[i];
                yEnd = yVals[i];
                x1 = xStart;
                y1 = yStart;
                x2 = xEnd;
                y2 = yEnd;
                double totalLen = CalcLenOfLine(x1, x2, y1, y2);
                moveToNext = false;
                while (!moveToNext)
                {
                    points2 = new int[10];
                    if (totalLen < currentLen)
                    {
                        currentLen = currentLen - totalLen;
                        hasRemaining = true;
                        moveToNext = true;
                    }
                    else
                    {
                        if (totalLen != currentLen)
                        {
                            //Calculating the point at certain distance in a line segment.
                            double lenRatio = currentLen / totalLen;
                            x2 = x1 + (lenRatio * (x2 - x1));
                            y2 = y1 + (lenRatio * (y2 - y1));
                        }
                        totalLen = totalLen - currentLen;
                        currentLen = dashes[dashIndex] * multiplier;
                        dashIndex = dashIndex + 1 == dashes.Count() ? 0 : dashIndex + 1;
                        moveToNext = totalLen == 0;
                        hasRemaining = false;
                    }

                    if (isBeginning)
                    {
                        GetLinePoints(x1, y1, x2, y2, leftThickness, rightThickness, points1);
                    }
                    else
                    {
                        UpdatePoints2(x1, y1, x2, y2, leftThickness, rightThickness);
                    }


                    if (isPreviousDash)
                    {
                        DrawLineAa(color, width, height);
                    }

                    if (!isBeginning)
                    {
                        points1 = points2;
                        points2 = null;
                    }
                    isPreviousDash = isDash;
                    isBeginning = false;
                    isDash = hasRemaining ? isDash : !isDash;

                    x1 = x2;
                    y1 = y2;
                    x2 = xEnd;
                    y2 = yEnd;
                }
                xStart = xEnd;
                yStart = yEnd;
            }

            if (isPreviousDash)
            {
                DrawLineAa(color, width, height);
            }

            points1 = null;
            points2 = null;
        }

        private void DrawDashedLines(int width, int height, Color color, int leftThickness,
                                       int rightThickness)
        {
            if(!fastSeries.IsActualTransposed)
                DrawDashedLines(xValues, yValues, width, height, color, leftThickness,rightThickness);
            else
                DrawDashedLines(yValues, xValues, width, height, color, leftThickness, rightThickness);
        }

        private void DrawDashedLines(List<double> xVals, List<double> yVals, int width, int height, Color color,
                                     int leftThickness,
                                     int rightThickness)
        {
            var bitmapSeries = (FastLineBitmapSeries)fastSeries;
            double multiplier = bitmapSeries.StrokeThickness;
            double xEnd;
            double yEnd;
            xStart = xVals[0];
            yStart = yVals[0];
            DoubleCollection dashes = bitmapSeries.StrokeDashArray;
            bool isDash = true;
            double currentLen = dashes[0] * multiplier;
            double x1, y1, x2, y2;
            int dashIndex = 1;
            bool moveToNext = false;
            bool hasRemaining = true;
            bool isBeginning = true;
            bool isPreviousDash = false;
            if (points1 == null)
                points1 = new int[10];
            for (int i = 1; i < xVals.Count; i++)
            {
                xEnd = xVals[i];
                yEnd = yVals[i];
                x1 = xStart;
                y1 = yStart;
                x2 = xEnd;
                y2 = yEnd;
                double totalLen = CalcLenOfLine(x1, x2, y1, y2);
                moveToNext = false;
                while (!moveToNext)
                {
                    points2 = new int[10];
                    if (totalLen < currentLen)
                    {
                        currentLen = currentLen - totalLen;
                        hasRemaining = true;
                        moveToNext = true;
                    }
                    else
                    {
                        if (totalLen != currentLen)
                        {
                            //Calculating the point at certain distance in a line segment.
                            double lenRatio = currentLen / totalLen;
                            x2 = x1 + (lenRatio * (x2 - x1));
                            y2 = y1 + (lenRatio * (y2 - y1));
                        }
                        totalLen = totalLen - currentLen;
                        currentLen = dashes[dashIndex] * multiplier;
                        dashIndex = dashIndex + 1 == dashes.Count() ? 0 : dashIndex + 1;
                        moveToNext = totalLen == 0;
                        hasRemaining = false;
                    }

                    if (isBeginning)
                    {
                        GetLinePoints(x1, y1, x2, y2, leftThickness, rightThickness, points1);
                    }
                    else
                    {
                        UpdatePoints2(x1, y1, x2, y2, leftThickness, rightThickness);
                    }


                    if (isPreviousDash)
                    {
                        DrawLine(color, width, height);
                    }

                    if (!isBeginning)
                    {
                        points1 = points2;
                        points2 = null;
                    }
                    isPreviousDash = isDash;
                    isBeginning = false;
                    isDash = hasRemaining ? isDash : !isDash;

                    x1 = x2;
                    y1 = y2;
                    x2 = xEnd;
                    y2 = yEnd;
                }
                xStart = xEnd;
                yStart = yEnd;
            }

            if (isPreviousDash)
            {
                DrawLine(color,width, height);
            }

            points1 = null;
            points2 = null;
        }

        private void DrawLine(Color color,int width, int height)
        {
#if WINDOWS_PHONE
            bitmap.DrawLineBresenham(points1[0], points1[1], points1[2], points1[3], color);
            bitmap.FillPolygon(points1, color);
            bitmap.DrawLineBresenham(points1[4], points1[5], points1[6], points1[7], color);
#else
            bitmap.DrawLineBresenham(fastBuffer, width, height, points1[0], points1[1], points1[2], points1[3], color);
            bitmap.FillPolygon(fastBuffer, points1, width, height, color);
            bitmap.DrawLineBresenham(fastBuffer, width, height, points1[4], points1[5], points1[6], points1[7], color);
#endif
        }

        private void DrawLineAa(Color color, int width, int height)
        {
#if WINDOWS_PHONE
            bitmap.DrawLineAa(points1[0], points1[1], points1[2], points1[3], color);
            bitmap.FillPolygon(points1, color);
            bitmap.DrawLineAa(points1[4], points1[5], points1[6], points1[7], color);
#else
            bitmap.DrawLineAa(fastBuffer, width, height, points1[0], points1[1], points1[2], points1[3], color);
            bitmap.FillPolygon(fastBuffer, points1, width, height, color);
            bitmap.DrawLineAa(fastBuffer, width, height, points1[4], points1[5], points1[6], points1[7], color);
#endif
        }

        double CalcLenOfLine(double x1, double x2, double y1, double y2)
        {
            double x = x2 - x1;
            double y = y2 - y1;
            return Math.Sqrt((x*x) + (y*y));
        }

        #endregion
    }
}
