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
    /// Represents chart fast scatter bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastScatterBitmapSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastScatterBitmapSegment : ChartSegment
    {
        #region fields

        private IList<double> xChartVals, yChartVals;

        List<double> xValues = new List<double>();

        List<double> yValues = new List<double>();

        private WriteableBitmap bitmap;

        int start,count;

        double xStart, xEnd, yStart, yEnd, xDelta, yDelta, xSize, ySize, xOffset, yOffset;

        bool x_isInversed, y_isInversed;

#if !WINDOWS_PHONE
        private byte[] fastBuffer;
#endif

        internal FastScatterBitmapSeries fastSeries;

        #endregion

        #region ctor
        /// <summary>
        /// 
        /// </summary>
        public FastScatterBitmapSegment()
        {

        }

        public FastScatterBitmapSegment(IList<double> xVals, IList<double> yVals, FastScatterBitmapSeries series)
        {
            base.Series = series;
            fastSeries = series;
            this.xChartVals = xVals;
            this.yChartVals = yVals;

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
            return null;
        }

        [ClassReference(IsReviewed = false)]
        protected override void SetVisualBindings(Shape element)
        {

        }

        [ClassReference(IsReviewed = false)]
        public override void OnSizeChanged(Size size)
        {
        }

        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        [ClassReference(IsReviewed = false)]
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
                start = (int)(Math.Floor(xStart));
                count = (int)(Math.Ceiling(xEnd));
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
                UpdateVisual();
            }
        }

        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="transformer"></param>
        [ClassReference(IsReviewed = false)]
        void TransformToScreenCo(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            if (!fastSeries.IsActualTransposed)
                TransformToScreenCoHorizontal();
            else
                TransformToScreenCoVertical();
        }

        private void TransformToScreenCoVertical()
        {
            double yValue = 0;
            int i = 0;
            if (fastSeries.IsIndexed)
            {
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                for (i = start; i <= count; i++)
                {
                    double yVal = yChartVals[i];
                    if (y_isInversed
                            ? (yVal >= yEnd && yVal <= yStart)
                            : (yVal <= yEnd && yVal >= yStart))
                    {
                        xValues.Add((xOffset + xSize * ((xEnd - i) / xDelta)));
                        yValues.Add((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))));
                    }
                    else
                    {
                        yValue = -1;
                        xValues.Add(yValue);
                        yValues.Add(yValue);
                    }
                }

            }
            else
            {
                for (i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xChartVals[i];
                    double yVal = yChartVals[i];
                    if ((y_isInversed
                             ? yVal >= yEnd && yVal <= yStart
                             : yVal <= yEnd && yVal >= yStart) &&
                        (x_isInversed ? xVal >= xEnd && xVal <= xStart : xVal <= xEnd && xVal >= xStart))
                    {
                        xValues.Add((xOffset + xSize * ((xEnd - xVal) / xDelta)));
                        yValues.Add((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))));
                    }
                    else
                    {
                        yValue = -1;
                        xValues.Add(yValue);
                        yValues.Add(yValue);
                    }
                }
            }
        }

        private void TransformToScreenCoHorizontal()
        {
            double yValue = 0;
            int i = 0;
            if (fastSeries.IsIndexed)
            {
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                for (i = start; i <= count; i++)
                {
                    double yVal = yChartVals[i];
                    if (y_isInversed
                            ? (yVal >= yEnd && yVal <= yStart)
                            : (yVal <= yEnd && yVal >= yStart))
                    {
                        xValues.Add((xOffset + xSize * ((i - xStart) / xDelta)));
                        yValues.Add((yOffset + ySize * (1 - ((yVal - yStart) / yDelta))));
                    }
                    else
                    {
                        yValue = -1;
                        xValues.Add(yValue);
                        yValues.Add(yValue);
                    }
                }

            }
            else
            {
                for (i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xChartVals[i];
                    double yVal = yChartVals[i];
                    if ((y_isInversed
                             ? yVal >= yEnd && yVal <= yStart
                             : yVal <= yEnd && yVal >= yStart) &&
                        (x_isInversed ? xVal >= xEnd && xVal <= xStart : xVal <= xEnd && xVal >= xStart))
                    {
                        xValues.Add((xOffset + xSize * ((xVal - xStart) / xDelta)));
                        yValues.Add((yOffset + ySize * (1 - ((yVal - yStart) / yDelta))));
                    }
                    else
                    {
                        yValue = -1;
                        xValues.Add(yValue);
                        yValues.Add(yValue);
                    }
                }
            }
        }

        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="transformer"></param>
        [ClassReference(IsReviewed = false)]
        void TransformToScreenCoInLog(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;
            if (!fastSeries.IsActualTransposed)
                TransformToScreenCoInLogHorizontal(xBase,yBase);
            else
                TransformToScreenCoInLogVertical(xBase,yBase);
            
        }

        private void TransformToScreenCoInLogVertical(double xBase, double yBase)
        {
            float yValue = 0;
            int i = 0;
            if (fastSeries.IsIndexed)
            {
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                for (i = start; i <= count; i++)
                {
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    if (y_isInversed
                            ? yVal >= yEnd && yVal <= yStart
                            : yVal <= yEnd && yVal >= yStart)
                    {
                        double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                        xValues.Add((xOffset + xSize * ((xEnd - xVal) / xDelta)));
                        yValues.Add((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))));
                    }
                    else
                    {
                        yValue = -1;
                        xValues.Add(yValue);
                        yValues.Add(yValue);
                    }
                }
            }
            else
            {
                for (i = 0; i < xChartVals.Count; i++)
                {
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    if ((y_isInversed
                            ? yVal >= yEnd && yVal <= yStart
                            : yVal <= yEnd && yVal >= yStart) && (x_isInversed
                                                                   ? xVal >= xStart && xVal <= xStart
                                                                   : xVal <= xEnd && xVal >= xStart))
                    {
                        xValues.Add((xOffset + xSize * ((xEnd - xVal) / xDelta)));
                        yValues.Add((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))));
                    }
                    else
                    {
                        yValue = -1;
                        xValues.Add(yValue);
                        yValues.Add(yValue);
                    }
                }
            }
        }

        private void TransformToScreenCoInLogHorizontal(double xBase, double yBase)
        {
            float yValue = 0;
            int i = 0;
            if (fastSeries.IsIndexed)
            {
                start = start < 0 ? 0 : start;
                count = count > yChartVals.Count - 1 ? yChartVals.Count - 1 : count;
                for (i = start; i <= count; i++)
                {
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    if (y_isInversed
                            ? yVal >= yEnd && yVal <= yStart
                            : yVal <= yEnd && yVal >= yStart)
                    {
                        double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                        xValues.Add((xOffset + xSize * ((xVal - xStart) / xDelta)));
                        yValues.Add((yOffset + ySize * (1 - ((yVal - yStart) / yDelta))));
                    }
                    else
                    {
                        yValue = -1;
                        xValues.Add(yValue);
                        yValues.Add(yValue);
                    }
                }

            }
            else
            {
                for (i = 0; i < xChartVals.Count; i++)
                {
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    if ((y_isInversed
                            ? yVal >= yEnd && yVal <= yStart
                            : yVal <= yEnd && yVal >= yStart) && (x_isInversed
                                                                   ? xVal >= xStart && xVal <= xStart
                                                                   : xVal <= xEnd && xVal >= xStart))
                    {
                        xValues.Add((xOffset + xSize * ((xVal - xStart) / xDelta)));
                        yValues.Add((yOffset + ySize * (1 - ((yVal - yStart) / yDelta))));
                    }
                    else
                    {
                        yValue = -1;
                        xValues.Add(yValue);
                        yValues.Add(yValue);
                    }
                }
            }
        }

        
        [ClassReference(IsReviewed = false)]
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

        [ClassReference(IsReviewed = false)]
        internal void UpdateSegment(int index, NotifyCollectionChangedAction action, IChartTransformer transformer)
        {
        }

        [ClassReference(IsReviewed = false)]
        internal void UpdateVisual()
        {
            double xr = fastSeries.ScatterHeight, yr = fastSeries.ScatterWidth;
            
            Color color = ((SolidColorBrush)Interior).Color;

            if (bitmap != null && xValues.Count > 1)
            {
#if !WINDOWS_PHONE
                fastBuffer = fastSeries.Area.GetFastBuffer();
#endif
                int width = (int)fastSeries.Area.SeriesClipRect.Width;
                int height = (int)fastSeries.Area.SeriesClipRect.Height;
#if WPF
                bitmap.BeginWrite();
#endif

                if (fastSeries is FastScatterBitmapSeries)
                {
                    if (fastSeries.IsActualTransposed)
                        FillEllipseCenteredVertical(width, height, color, xr, yr);
                    else
                        FillEllipseCenteredHorizontal(width, height, color, xr, yr);
                }

#if WPF
                bitmap.EndWrite();
#endif
            }

            fastSeries.Area.CanRenderToBuffer = true;
        }

        private void FillEllipseCenteredHorizontal(int width, int height, Color color, double xr, double yr)
        {
            double actualIndex = start;
            double xValue = 0, yValue = 0;
            for (int i = 0; i < xValues.Count; i++)
            {
                xValue = xValues[i];
                yValue = yValues[i];
                if (Series.Palette != ChartColorPalette.None)
                {
                    Brush brush = Series.ColorModel.GetBrush((fastSeries.IsIndexed ? (int)Math.Ceiling(actualIndex) : i));
                    color = ((SolidColorBrush)brush).Color;
                }
                if (yValue > -1)
                {
#if WINDOWS_PHONE
                            bitmap.FillEllipseCentered((int)xValue, (int)yValue, (int)xr, (int)yr, color);
#else
                    bitmap.FillEllipseCentered(fastBuffer, height, width, (int)xValue, (int)yValue, (int)xr, (int)yr, color);
#endif
                }
                actualIndex++;
            }
        }

        private void FillEllipseCenteredVertical(int width, int height, Color color, double xr, double yr)
        {
            double actualIndex = start;
            double xValue = 0, yValue = 0;
            for (int i = 0; i < xValues.Count; i++)
            {
                xValue = xValues[i];
                yValue = yValues[i];
                if (Series.Palette != ChartColorPalette.None)
                {
                    Brush brush = Series.ColorModel.GetBrush((fastSeries.IsIndexed ? (int)Math.Ceiling(actualIndex) : i));
                    color = ((SolidColorBrush)brush).Color;
                }
                if (yValue > -1)
                {
#if WINDOWS_PHONE
                            bitmap.FillEllipseCentered((int)yValue, (int)xValue, (int)xr, (int)yr, color);
#else
                    bitmap.FillEllipseCentered(fastBuffer, height, width, (int)yValue, (int)xValue, (int)xr, (int)yr, color);
#endif
                }
                actualIndex++;
            }

        }


        #endregion
    }
}
