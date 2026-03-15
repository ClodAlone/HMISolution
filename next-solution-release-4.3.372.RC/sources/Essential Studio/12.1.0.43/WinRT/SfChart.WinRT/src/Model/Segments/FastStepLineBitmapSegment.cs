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
    /// Represents chart fast stepline bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastStepLineBitmapSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastStepLineBitmapSegment : ChartSegment
    {
        #region fields

        private IList<double> xChartVals;

        private IList<double> yChartVals;

        double xStart, xEnd, yStart, yEnd, xDelta, yDelta, xSize, ySize, xOffset, yOffset;

        double xTolerance, yTolerance;

        int count, start;

        bool x_isInversed, y_isInversed;

        private WriteableBitmap bitmap;

#if !WINDOWS_PHONE
        private byte[] fastBuffer;
#endif

        internal ChartSeries fastSeries;

        List<double> xValues = new List<double>();

        List<double> yValues = new List<double>();

        #endregion

        #region ctor

        public FastStepLineBitmapSegment(ChartSeriesBase series)
        {
            fastSeries = series as ChartSeries;
        }

        public FastStepLineBitmapSegment(IList<double> xVals, IList<double> yVals, AdornmentSeries series)
            : this(series)
        {
            Series = series;
            xChartVals = xVals;
            yChartVals = yVals;

            if (series.DataCount > 1)
            {
                if (series.IsIndexed)
                {
                    double xMax = series.DataCount - 1;
                    double yMax = yChartVals.Max();
                    double yMin = yChartVals.Min();
                    XRange = new DoubleRange(0, xMax);
                    YRange = new DoubleRange(yMin, yMax);
                }
                else
                {
                    double xMax = xChartVals.Max();
                    double yMax = yChartVals.Max();
                    double xMin = xChartVals.Min();
                    double yMin = yChartVals.Min();
                    XRange = new DoubleRange(xMin, xMax);
                    YRange = new DoubleRange(yMin, yMax);
                }
            }
        }

        #endregion

        #region methods

        [ClassReference(IsReviewed = false)]
        public override void SetData(IList<double> xVals, IList<double> yVals)
        {
            this.xChartVals = xVals;
            this.yChartVals = yVals;
        }

        [ClassReference(IsReviewed = false)]
        public override UIElement CreateVisual(Size size)
        {
            bitmap = fastSeries.Area.GetFastRenderSurface();

#if !WINDOWS_PHONE
            fastBuffer = fastSeries.Area.GetFastBuffer();
#endif
            return null;
        }

        protected override void SetVisualBindings(Shape element) { }

        public override void OnSizeChanged(Size size) { }

        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        public override void Update(IChartTransformer transformer)
        {
            bitmap = fastSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastSeries.DataCount > 1)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                bool isLogarithmic = cartesianTransformer.XAxis.IsLogarithmic || cartesianTransformer.YAxis.IsLogarithmic;
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
                    xSize = cartesianTransformer.XAxis.RenderedRect.Height;
                    ySize = cartesianTransformer.YAxis.RenderedRect.Width;
                    xOffset = cartesianTransformer.XAxis.RenderedRect.Top - fastSeries.Area.SeriesClipRect.Top;
                    yOffset = cartesianTransformer.YAxis.RenderedRect.Left - fastSeries.Area.SeriesClipRect.Left;
                }
                else
                {
                    xSize = cartesianTransformer.XAxis.RenderedRect.Width;
                    ySize = cartesianTransformer.YAxis.RenderedRect.Height;
                    xOffset = cartesianTransformer.XAxis.RenderedRect.Left - fastSeries.Area.SeriesClipRect.Left;
                    yOffset = cartesianTransformer.YAxis.RenderedRect.Top - fastSeries.Area.SeriesClipRect.Top;
                }
                //to calculate the minimum area of single point
                xTolerance = Math.Abs((xDelta * 1) / xSize);
                yTolerance = Math.Abs((yDelta * 1) / ySize);
                count = (int)(Math.Ceiling(xEnd));
                start = (int)(Math.Floor(xStart));
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
                xValues.Clear();
                if (!isLogarithmic)
                {
                    TransformToScreenCo(cartesianTransformer);
                }
                else
                {
                    TransformToScreenCoInLog(cartesianTransformer);
                }
                UpdateVisual();
            }
        }



        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        void TransformToScreenCo(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            if (!fastSeries.IsActualTransposed)
                TransformToScreenCoHorizontal();
            else
                TransformToScreenCoVertical();
        }

        private void TransformToScreenCoVertical()
        {
            int i;
            double prevXValue = 0, prevYValue = 0;
            if (fastSeries.IsIndexed)
            {
                var categoryAxis = fastSeries.ActualXAxis as CategoryAxis;
                double offsetX = categoryAxis != null && categoryAxis.LabelPlacement == LabelPlacement.BetweenTicks
                    ? 0.5
                    : 0;
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;

                prevXValue = 1;
                double yVal = 0d;
                for (i = start; i <= count; i++)
                {
                    yVal = yChartVals[i];
                    double pos = i - offsetX;
                    if (Math.Abs(prevXValue - pos) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                    {
                        xValues.Add((xOffset + xSize * ((xEnd - i) / xDelta)));
                        yValues.Add(yOffset + ySize * (1 - ((yEnd - yVal) / yDelta)));
                        prevXValue = pos;
                        prevYValue = yVal;
                    }
                }
                if (!(offsetX > 0)) return;
                xValues.Add((xOffset + xSize * ((xEnd - count + offsetX) / xDelta)));
                yValues.Add(yOffset + ySize * (1 - ((yEnd - yVal) / yDelta)));
            }
            else
            {
                for (i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xChartVals[i];
                    double yVal = yChartVals[i];

                    if ((xVal <= count) && (xVal >= start))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValues.Add((xOffset + xSize * ((xEnd - xVal) / xDelta)));
                            yValues.Add(yOffset + ySize * (1 - ((yEnd - yVal) / yDelta)));
                            prevXValue = xVal;
                            prevYValue = yVal;
                        }
                    }
                }
            }
        }

        private void TransformToScreenCoHorizontal()
        {
            int i;
            double prevXValue = 0, prevYValue = 0;
            if (fastSeries.IsIndexed)
            {
                var categoryAxis = fastSeries.ActualXAxis as CategoryAxis;
                double offsetX = categoryAxis != null && categoryAxis.LabelPlacement == LabelPlacement.BetweenTicks
                    ? 0.5
                    : 0;
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                prevXValue = 1;
                double yVal = 0d;
                for (i = start; i <= count; i++)
                {
                    yVal = yChartVals[i];
                    double pos = i - offsetX;
                    if (Math.Abs(prevXValue - pos) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                    {
                        
                        xValues.Add((xOffset + xSize * ((i - xStart) / xDelta)));
                        yValues.Add(yOffset + ySize * (1 - ((yVal - yStart) / yDelta)));
                        prevXValue = pos;
                        prevYValue = yVal;
                    }
                }
                if (!(offsetX > 0)) return;
                xValues.Add((xOffset + xSize * ((count + offsetX)- xStart / xDelta)));
                yValues.Add(yOffset + ySize * (1 - ((yVal - yStart) / yDelta)));
            }
            else
            {
                for (i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xChartVals[i];
                    double yVal = yChartVals[i];

                    if ((xVal <= count) && (xVal >= start))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValues.Add((xOffset + xSize * ((xVal - xStart) / xDelta)));
                            yValues.Add(yOffset + ySize * (1 - ((yVal - yStart) / yDelta)));
                            prevXValue = xVal;
                            prevYValue = yVal;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="transformer"></param>
        void TransformToScreenCoInLog(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;
            if (!fastSeries.IsActualTransposed)
                TransformToScreenCoInLogHorizontal(xBase, yBase);
            else
                TransformToScreenCoInLogVertical(xBase, yBase);
           
        }

        private void TransformToScreenCoInLogVertical(double xBase, double yBase)
        {
            double prevXValue = 0, prevYValue = 0;
            int i = 0;
            if (fastSeries.IsIndexed)
            {
                var categoryAxis = fastSeries.ActualXAxis as CategoryAxis;
                double offsetX = categoryAxis != null && categoryAxis.LabelPlacement == LabelPlacement.BetweenTicks
                    ? 0.5
                    : 0;
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                prevXValue = 1;
                double yVal = 0;
                for (i = start; i <= count; i++)
                {
                    double pos = i - offsetX;
                    yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? pos : Math.Log(pos, xBase);

                    if (Math.Abs(prevXValue - pos) >= 1 || Math.Abs(prevYValue - yVal) >= 1)
                    {
                        xValues.Add((xOffset + xSize * ((xEnd-xVal) / xDelta)));
                        yValues.Add(yOffset + ySize * (1 - ((yEnd - yVal) / yDelta)));
                        prevXValue = pos;
                        prevYValue = yVal;
                    }
                }
                if (!(offsetX > 0)) return;

                xValues.Add((xOffset + xSize * ((xEnd - count + offsetX) / xDelta)));
                yValues.Add(yOffset + ySize * (1 - ((yEnd-yVal) / yDelta)));
            }
            else
            {
                for (i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    if ((xVal <= count) && (xVal >= start))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValues.Add((xOffset + xSize * ((xEnd - xVal) / xDelta)));
                            yValues.Add(yOffset + ySize * (1 - ((yEnd - yVal) / yDelta)));
                            prevXValue = xVal;
                            prevYValue = yVal;
                        }
                    }
                }
            }
        }

        private void TransformToScreenCoInLogHorizontal(double xBase, double yBase)
        {
            double prevXValue = 0, prevYValue = 0;
            int i = 0;
            if (fastSeries.IsIndexed)
            {
                var categoryAxis = fastSeries.ActualXAxis as CategoryAxis;
                double offsetX = categoryAxis != null && categoryAxis.LabelPlacement == LabelPlacement.BetweenTicks
                    ? 0.5
                    : 0;
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                prevXValue = 1;
                double yVal = 0;
                for (i = start; i <= count; i++)
                {
                    double pos = i - offsetX;
                    yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? pos : Math.Log(pos, xBase);

                    if (Math.Abs(prevXValue - pos) >= 1 || Math.Abs(prevYValue - yVal) >= 1)
                    {
                        xValues.Add((xOffset + xSize * ((xVal - xStart) / xDelta)));
                        yValues.Add(yOffset + ySize * (1 - ((yVal - yStart) / yDelta)));
                        prevXValue = pos;
                        prevYValue = yVal;
                    }
                }
                if (!(offsetX > 0)) return;
               
                xValues.Add((xOffset + xSize * ((count + offsetX - xStart) / xDelta)));
                yValues.Add(yOffset + ySize * (1 - ((yVal - yStart) / yDelta)));
            }
            else
            {
                for (i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    if ((xVal <= count) && (xVal >= start))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            xValues.Add((xOffset + xSize * ((xVal - xStart) / xDelta)));
                            yValues.Add(yOffset + ySize * (1 - ((yVal - yStart) / yDelta)));
                            prevXValue = xVal;
                            prevYValue = yVal;
                        }
                    }
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
        }

        internal void UpdateSegment(int index, NotifyCollectionChangedAction action, IChartTransformer transformer)
        {
        }

        internal void UpdateVisual()
        {
            double xStart, yStart;
            double actualIndex = start;
            Color color = ((SolidColorBrush)this.Interior).Color;

            if (bitmap != null && xValues.Count > 1)
            {
#if !WINDOWS_PHONE
                fastBuffer = fastSeries.Area.GetFastBuffer();
#endif

                xStart = xValues[0];
                yStart = yValues[0];

                int width = (int)fastSeries.Area.SeriesClipRect.Width;
                int height = (int)fastSeries.Area.SeriesClipRect.Height;

                int leftThickness = (int)fastSeries.StrokeThickness / 2;
                int rightThickness = (int)(fastSeries.StrokeThickness % 2 == 0
                    ? (fastSeries.StrokeThickness / 2) - 1 : fastSeries.StrokeThickness / 2);

#if WPF
                bitmap.BeginWrite();
#endif

                if (fastSeries is FastStepLineBitmapSeries)
                {
                    if (!fastSeries.IsActualTransposed)
                    {
                        UpdateVisualHorizontal(xStart, yStart, width, height, color, leftThickness, rightThickness);
                    }
                    else
                    {
                        UpdateVisualVertical(xStart, yStart, width, height, color, leftThickness, rightThickness);
                    }
                }

#if WPF
                bitmap.EndWrite();
#endif
            }

            fastSeries.Area.CanRenderToBuffer = true;
        }

        private void UpdateVisualVertical(double xStart, double yStart, int width, int height, Color color, int leftThickness, int rightThickness)
        {
            double xEnd, yEnd;
            double actualIndex = start;
            if (((FastStepLineBitmapSeries)fastSeries).EnableAntiAliasing)
            {
                for (int i = 1; i < xValues.Count; i++)
                {
                    if (Series.Palette != ChartColorPalette.None)
                    {
                        Brush brush = Series.ColorModel.GetBrush((fastSeries.IsIndexed ? (int)Math.Ceiling(actualIndex) : i));
                        color = ((SolidColorBrush)brush).Color;
                    }
                    xEnd = xValues[i];
                    yEnd = yValues[i];
#if WINDOWS_PHONE
                    var x1 = xStart - leftThickness;
                    var x2 = xStart + rightThickness;
                    if (yEnd < yStart)
                        bitmap.FillRectangle((int)yEnd, (int)x1, (int)yStart, (int)x2, color);
                    else
                        bitmap.FillRectangle((int)yStart, (int)x1, (int)yEnd, (int)x2, color);
                    bitmap.DrawLineAa((int)yStart, (int)x1, (int)yEnd, (int)x1, color);
                    bitmap.DrawLineAa((int)yStart, (int)x1, (int)yStart, (int)x2, color);
                    bitmap.DrawLineAa((int)yStart, (int)x2, (int)yEnd, (int)x2, color);
                    bitmap.DrawLineAa((int)yEnd, (int)x1, (int)yEnd, (int)x2, color);
                    x1 = yEnd - leftThickness;
                    x2 = yEnd + rightThickness;
                    bitmap.FillRectangle((int)x1, (int)xEnd, (int)x2, (int)xStart, color);
                    bitmap.DrawLineAa((int)x1, (int)xEnd, (int)x2, (int)xEnd, color);
                    bitmap.DrawLineAa((int)x1, (int)xEnd, (int)x1, (int)xStart, color);
                    bitmap.DrawLineAa((int)x1, (int)xStart, (int)x2, (int)xStart, color);
                    bitmap.DrawLineAa((int)x2, (int)xEnd, (int)x2, (int)xStart, color);
#else
                    var leftOffset = xStart - leftThickness;
                    var rightOffset = xStart + rightThickness;
                    if (yEnd < yStart)
                        bitmap.FillRectangle(fastBuffer, width, height, (int)yEnd, (int)leftOffset, (int)yStart, (int)rightOffset, color);
                    else
                        bitmap.FillRectangle(fastBuffer, width, height, (int)yStart, (int)leftOffset, (int)yEnd, (int)rightOffset, color);
                    bitmap.DrawLineAa(fastBuffer,width, height, (int)yStart, (int)leftOffset, (int)yEnd, (int)leftOffset, color);
                    bitmap.DrawLineAa(fastBuffer, width, height,(int)yStart, (int)leftOffset, (int)yStart, (int)rightOffset, color);
                    bitmap.DrawLineAa(fastBuffer, width, height,(int)yStart, (int)rightOffset, (int)yEnd, (int)rightOffset, color);
                    bitmap.DrawLineAa(fastBuffer,width, height, (int)yEnd, (int)leftOffset, (int)yEnd, (int)rightOffset, color);
                    leftOffset = yEnd - leftThickness;
                    rightOffset = yEnd + rightThickness;
                    bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)xEnd, (int)rightOffset, (int)xStart, color);
                    bitmap.DrawLineAa(fastBuffer,width, height, (int)leftOffset, (int)xEnd, (int)rightOffset, (int) xEnd, color);
                    bitmap.DrawLineAa(fastBuffer,width, height, (int)leftOffset, (int)xEnd, (int)leftOffset, (int)xStart, color);
                    bitmap.DrawLineAa(fastBuffer, width, height, (int)leftOffset, (int)xStart, (int)rightOffset, (int)xStart, color);
                    bitmap.DrawLineAa(fastBuffer, width, height, (int)rightOffset, (int)xEnd, (int)rightOffset, (int)xStart, color);
                    
#endif
                    xStart = xEnd;
                    yStart = yEnd;

                    actualIndex++;
                }
            }
            else
            {
                for (int i = 1; i < xValues.Count; i++)
                {
                    if (Series.Palette != ChartColorPalette.None)
                    {
                        Brush brush = Series.ColorModel.GetBrush((fastSeries.IsIndexed ? (int)Math.Ceiling(actualIndex) : i));
                        color = ((SolidColorBrush)brush).Color;
                    }
                    xEnd = xValues[i];
                    yEnd = yValues[i];
#if WINDOWS_PHONE
                    var leftOffset = xStart - leftThickness;
                    var rightOffset = xStart + rightThickness;
                    if (yEnd < yStart)
                        bitmap.FillRectangle((int)yEnd, (int)leftOffset, (int)yStart, (int)rightOffset, color);
                    else
                        bitmap.FillRectangle((int)yStart, (int)leftOffset, (int)yEnd, (int)rightOffset, color);
                    leftOffset = yEnd - leftThickness;
                    rightOffset = yEnd + rightThickness;
                    bitmap.FillRectangle((int)leftOffset, (int)xEnd, (int)rightOffset, (int)xStart, color);
                     
#else
                    var leftOffset = xStart - leftThickness;
                    var rightOffset = xStart + rightThickness;
                    if(yEnd<yStart)
                        bitmap.FillRectangle(fastBuffer, width, height, (int)yEnd, (int)leftOffset, (int)yStart, (int)rightOffset, color);
                    else
                        bitmap.FillRectangle(fastBuffer, width, height, (int)yStart, (int)leftOffset, (int)yEnd, (int)rightOffset, color);
                    leftOffset = yEnd - leftThickness;
                    rightOffset = yEnd + rightThickness;
                    bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)xEnd, (int)rightOffset, (int)xStart, color);
#endif
                    xStart = xEnd;
                    yStart = yEnd;
                    actualIndex++;
                }
            }
        }

        private void UpdateVisualHorizontal(double xStart, double yStart, int width, int height, Color color, int leftThickness, int rightThickness)
        {
            double xEnd, yEnd;
            double actualIndex = start;
            if (((FastStepLineBitmapSeries)fastSeries).EnableAntiAliasing)
            {
                for (int i = 1; i < xValues.Count; i++)
                {
                    if (Series.Palette != ChartColorPalette.None)
                    {
                        Brush brush = Series.ColorModel.GetBrush((fastSeries.IsIndexed ? (int)Math.Ceiling(actualIndex) : i));
                        color = ((SolidColorBrush)brush).Color;
                    }
                    xEnd = xValues[i];
                    yEnd = yValues[i];
#if WINDOWS_PHONE
                    var leftOffset = yStart - leftThickness;
                    var rightOffset = yStart + rightThickness;
                    bitmap.DrawLineAa((int)xStart, (int)leftOffset, (int)xEnd, (int)leftOffset, color);
                    bitmap.DrawLineAa((int)xStart, (int)leftOffset, (int)xStart, (int)rightOffset, color);
                    bitmap.DrawLineAa((int)xStart, (int)rightOffset, (int)xEnd, (int)rightOffset, color);
                    bitmap.DrawLineAa((int)xEnd, (int)leftOffset, (int)xEnd, (int)rightOffset, color);
                    bitmap.FillRectangle((int)xStart, (int)leftOffset, (int)xEnd, (int)rightOffset, color);
                    leftOffset = xEnd - leftThickness;
                    rightOffset = xEnd + rightThickness;
                    bitmap.DrawLineAa((int)leftOffset, (int)yStart, (int)rightOffset, (int)yStart, color);
                    bitmap.DrawLineAa((int)leftOffset, (int)yStart, (int)leftOffset, (int)yEnd, color);
                    bitmap.DrawLineAa((int)leftOffset, (int)yEnd, (int)rightOffset, (int)yEnd, color);
                    bitmap.DrawLineAa((int)rightOffset, (int)yStart, (int)rightOffset, (int)yEnd, color);
                    if (yStart < yEnd)
                        bitmap.FillRectangle((int)leftOffset, (int)yStart, (int)rightOffset, (int)yEnd, color);
                    else
                        bitmap.FillRectangle((int)leftOffset, (int)yEnd, (int)rightOffset, (int)yStart, color);
#else
                    var leftOffset = yStart - leftThickness;
                    var rightOffset = yStart + rightThickness;
                    bitmap.DrawLineAa(fastBuffer, width, height, (int)xStart, (int)leftOffset, (int)xEnd, (int)leftOffset, color);
                    bitmap.DrawLineAa(fastBuffer, width, height, (int)xStart, (int)leftOffset, (int)xStart, (int)rightOffset, color);
                    bitmap.DrawLineAa(fastBuffer, width, height, (int)xStart, (int)rightOffset, (int)xEnd, (int)rightOffset, color);
                    bitmap.DrawLineAa(fastBuffer, width, height, (int)xEnd, (int)leftOffset, (int)xEnd, (int)rightOffset, color);
                    bitmap.FillRectangle(fastBuffer, width, height, (int)xStart, (int)leftOffset, (int)xEnd, (int)rightOffset, color);
                    leftOffset = xEnd - leftThickness;
                    rightOffset = xEnd + rightThickness;
                    bitmap.DrawLineAa(fastBuffer, width, height, (int)leftOffset, (int)yStart, (int)rightOffset, (int)yStart, color);
                    bitmap.DrawLineAa(fastBuffer,width,height,(int)leftOffset, (int)yStart, (int)leftOffset, (int)yEnd, color);
                    bitmap.DrawLineAa(fastBuffer, width, height, (int)leftOffset, (int)yEnd, (int)rightOffset, (int)yEnd, color);
                    bitmap.DrawLineAa(fastBuffer, width, height, (int)rightOffset, (int)yStart, (int)rightOffset, (int)yEnd, color);
                    if (yStart < yEnd)
                        bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)yStart, (int)rightOffset, (int)yEnd, color);
                    else
                        bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)yEnd, (int)rightOffset, (int)yStart, color);
#endif
                    xStart = xEnd;
                    yStart = yEnd;

                    actualIndex++;
                }
            }
            else
            {
                for (int i = 1; i < xValues.Count; i++)
                {
                    if (Series.Palette != ChartColorPalette.None)
                    {
                        Brush brush = Series.ColorModel.GetBrush((fastSeries.IsIndexed ? (int)Math.Ceiling(actualIndex) : i));
                        color = ((SolidColorBrush)brush).Color;
                    }
                    xEnd = xValues[i];
                    yEnd = yValues[i];

#if WINDOWS_PHONE
                    var leftOffset = yStart - leftThickness;
                    var rightOffset = yStart + rightThickness;
                    bitmap.FillRectangle((int)xStart, (int)leftOffset, (int)xEnd, (int)rightOffset, color);
                    leftOffset = xEnd - leftThickness;
                    rightOffset = xEnd + rightThickness;
                    if (yStart < yEnd)
                        bitmap.FillRectangle((int)leftOffset, (int)yStart, (int)rightOffset, (int)yEnd, color);
                    else
                        bitmap.FillRectangle((int)leftOffset, (int)yEnd, (int)rightOffset, (int)yStart, color);
#else
                    var leftOffset = yStart - leftThickness;
                    var rightOffset = yStart + rightThickness;
                    bitmap.FillRectangle(fastBuffer, width, height, (int)xStart, (int)leftOffset, (int)xEnd, (int)rightOffset, color);
                    leftOffset = xEnd - leftThickness;
                    rightOffset = xEnd + rightThickness;
                    if(yStart<yEnd)
                    bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)yStart, (int)rightOffset, (int)yEnd, color);
                    else
                        bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)yEnd, (int)rightOffset, (int)yStart, color);
#endif

                    xStart = xEnd;
                    yStart = yEnd;

                    actualIndex++;
                }
            }
        }


        #endregion
    }
}
