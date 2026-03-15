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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Data;
#else
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart fast candle bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WinRT Chart building system.</remarks>
    /// <seealso cref="FastLineBitmapSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastCandleBitmapSegment : ChartSegment
    {
        #region fields

        private DoubleRange sbsInfo;

        private WriteableBitmap bitmap;
#if NETFX_CORE
        private byte[] fastBuffer;
#endif
        internal ChartSeries fastCandleBitmapSeries;

        private IList<double> xChartVals, openChartVals, closeChartVals, highChartVals, lowChartVals;

        bool x_isInversed, y_isInversed;

        double xStart, xEnd, yStart, yEnd, xDelta, yDelta, xSize, ySize, xOffset, yOffset;

        int count;

        #endregion

        #region ctor

        public FastCandleBitmapSegment()
        {

        }

        public FastCandleBitmapSegment(ChartSeriesBase series)
        {
            fastCandleBitmapSeries = series as ChartSeries;
        }

        public FastCandleBitmapSegment(IList<double> xValues, IList<double> OpenValues, IList<double> CloseValues, IList<double> highValues, IList<double> LowValues, ChartSeriesBase series)
            : this(series)
        {
            base.Series = series;
            SetData(xValues, OpenValues, CloseValues, highValues, LowValues);
        }
        #endregion

        #region methods

        [ClassReference(IsReviewed = false)]
        public override UIElement CreateVisual(Size size)
        {
            return null;
        }

        [ClassReference(IsReviewed = false)]
        public override void SetData(IList<double> xValues, IList<double> openValue, IList<double> closeValue, IList<double> highValue, IList<double> lowValue)
        {
            sbsInfo = fastCandleBitmapSeries.GetSideBySideInfo(fastCandleBitmapSeries);
            this.xChartVals = xValues;
            this.openChartVals = openValue;
            this.closeChartVals = closeValue;
            this.highChartVals = highValue;
            this.lowChartVals = lowValue;

            double X_MAX = xValues.Max() + sbsInfo.End;
            double Y_MAX = highValue.Max();
            double X_MIN = xValues.Min() + sbsInfo.Start;
            double Y_MIN = lowValue.Min();

            XRange = new DoubleRange(X_MIN, X_MAX);
            YRange = new DoubleRange(Y_MIN, Y_MAX);

        }

        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        public override void Update(IChartTransformer transformer)
        {
            bitmap = fastCandleBitmapSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastCandleBitmapSeries.DataCount > 1)
            {
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                bool isLogarithmic = cartesianTransformer.XAxis.IsLogarithmic || cartesianTransformer.YAxis.IsLogarithmic;
                
                count = (int)(Math.Ceiling(xEnd));
                count = (int)Math.Min(count, xChartVals.Count);
                
                x_isInversed = cartesianTransformer.XAxis.IsInversed;
                y_isInversed = cartesianTransformer.YAxis.IsInversed;
                xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                yStart = cartesianTransformer.YAxis.VisibleRange.Start;
                yEnd = cartesianTransformer.YAxis.VisibleRange.End;
                xDelta = x_isInversed ? xStart - xEnd : xEnd - xStart;
                yDelta = y_isInversed ? yStart - yEnd : yEnd - yStart;
                if (fastCandleBitmapSeries.IsActualTransposed)
                {
                    ySize = cartesianTransformer.YAxis.RenderedRect.Width;
                    xSize = cartesianTransformer.XAxis.RenderedRect.Height;
                    yOffset = cartesianTransformer.YAxis.RenderedRect.Left - fastCandleBitmapSeries.Area.SeriesClipRect.Left;
                    xOffset = cartesianTransformer.XAxis.RenderedRect.Top - fastCandleBitmapSeries.Area.SeriesClipRect.Top;
                }
                else
                {
                    ySize = cartesianTransformer.YAxis.RenderedRect.Height;
                    xSize = cartesianTransformer.XAxis.RenderedRect.Width;
                    yOffset = cartesianTransformer.YAxis.RenderedRect.Top - fastCandleBitmapSeries.Area.SeriesClipRect.Top;
                    xOffset = cartesianTransformer.XAxis.RenderedRect.Left - fastCandleBitmapSeries.Area.SeriesClipRect.Left;
                }
                count = (int)(Math.Ceiling(xEnd));
                count = (int)Math.Min(count, xChartVals.Count);
                
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
                isBull.Clear();
                xValues.Clear();
                x1Values.Clear();
                x2Values.Clear();
                openValue.Clear();
                closeValue.Clear();
                highValue.Clear();
                lowValue.Clear();
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
        List<float> xValues = new List<float>();
        List<float> x1Values = new List<float>();
        List<float> x2Values = new List<float>();
        List<float> openValue = new List<float>();
        List<float> closeValue = new List<float>();
        List<float> highValue = new List<float>();
        List<float> lowValue = new List<float>();
        List<bool> isBull = new List<bool>();

        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="transformer"></param>
        void TransformToScreenCo(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            if (!fastCandleBitmapSeries.IsActualTransposed)
                TransformToScreenCoHorizontal();
            else
                TransformToScreenCoVertical();
        }

        private void TransformToScreenCoVertical()
        {
            float xValue, x1Value, x2Value, yOpenValue, yCloseValue, yHighValue, yLowValue;
            double x1Val, x2Val, openVal, closeVal, yHiVal, yLoVal;
            double sbsCenter = sbsInfo.Median;
            double sbsStart = sbsInfo.Start;
            double sbsEnd = sbsInfo.End;
            double tempOpenVal;
            if (!fastCandleBitmapSeries.IsIndexed)
            {
                for (int i = 0; i < count; i++)
                {
                    x1Val = x_isInversed ? (xChartVals[i] + sbsEnd) : (xChartVals[i] + sbsStart);
                    x2Val = x_isInversed ? (xChartVals[i] + sbsStart) : (xChartVals[i] + sbsEnd);
                    openVal = y_isInversed
                                  ? closeChartVals[i] > yStart
                                        ? yStart
                                        : closeChartVals[i] < yEnd ? yEnd : closeChartVals[i]
                                  : openChartVals[i] > yEnd
                                        ? yEnd
                                        : openChartVals[i] < yStart ? yStart : openChartVals[i];
                    closeVal = y_isInversed
                                   ? openChartVals[i] < yEnd
                                         ? yEnd
                                         : openChartVals[i] > yStart ? yStart : openChartVals[i]
                                   : closeChartVals[i] < yStart
                                         ? yStart
                                         : closeChartVals[i] > yEnd ? yEnd : closeChartVals[i];
                    if (y_isInversed ? openVal > closeVal : openVal < closeVal)
                    {
                        isBull.Add(true);
                        tempOpenVal = openVal;
                        openVal = closeVal;
                        closeVal = tempOpenVal;
                    }
                    else
                        isBull.Add(false);
                    yHiVal = highChartVals[i];
                    yLoVal = lowChartVals[i];
                    xValue = (float)(xOffset + (xSize) * ((xEnd + sbsCenter - xChartVals[i]) / xDelta));
                    yHighValue = (float)(yOffset + (ySize) * (1 - ((yEnd - yHiVal) / yDelta)));
                    yLowValue = (float)(yOffset + (ySize) * (1 - ((yEnd - yLoVal) / yDelta)));
                    x1Value = (float)(xOffset + (xSize) * ((x1Val - xStart) / xDelta));
                    x2Value = (float)(xOffset + (xSize) * ((x2Val - xStart) / xDelta));
                    yOpenValue = (float)(yOffset + (ySize) * (1 - ((openVal - yStart) / yDelta)));
                    yCloseValue = (float)(yOffset + (ySize) * (1 - ((closeVal - yStart) / yDelta)));

                    xValues.Add(xValue);
                    x1Values.Add(x1Value);
                    x2Values.Add(x2Value);
                    openValue.Add(yOpenValue);
                    closeValue.Add(yCloseValue);
                    highValue.Add(yHighValue);
                    lowValue.Add(yLowValue);

                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    x1Val = !x_isInversed ? (sbsStart + i) : (sbsEnd + i);
                    x2Val = !x_isInversed ? (sbsEnd + i) : (sbsStart + i);
                    openVal = y_isInversed
                                  ? closeChartVals[i] > yStart
                                        ? yStart
                                        : closeChartVals[i] < yEnd ? yEnd : closeChartVals[i]
                                  : openChartVals[i] > yEnd
                                        ? yEnd
                                        : openChartVals[i] < yStart ? yStart : openChartVals[i];
                    closeVal = y_isInversed
                                   ? openChartVals[i] < yEnd
                                         ? yEnd
                                         : openChartVals[i] > yStart ? yStart : openChartVals[i]
                                   : closeChartVals[i] < yStart
                                         ? yStart
                                         : closeChartVals[i] > yEnd ? yEnd : closeChartVals[i];
                    if (y_isInversed ? openVal > closeVal : openVal < closeVal)
                    {
                        isBull.Add(true);
                        tempOpenVal = openVal;
                        openVal = closeVal;
                        closeVal = tempOpenVal;
                    }
                    else
                        isBull.Add(false);

                    yHiVal = highChartVals[i];
                    yLoVal = lowChartVals[i];
                    xValue = (float)(xOffset + (xSize) * ((xEnd + sbsCenter - i) / xDelta));
                    yHighValue = (float)(yOffset + (ySize) * (1 - ((yEnd - yHiVal) / yDelta)));
                    yLowValue = (float)(yOffset + (ySize) * (1 - ((yEnd - yLoVal) / yDelta)));
                    x1Value = (float)(xOffset + (xSize) * ((x1Val - xStart) / xDelta));
                    x2Value = (float)(xOffset + (xSize) * ((x2Val - xStart) / xDelta));
                    yOpenValue = (float)(yOffset + (ySize) * (1 - ((openVal - yStart) / yDelta)));
                    yCloseValue = (float)(yOffset + (ySize) * (1 - ((closeVal - yStart) / yDelta)));
                    xValues.Add(xValue);
                    x1Values.Add(x1Value);
                    x2Values.Add(x2Value);
                    openValue.Add(yOpenValue);
                    closeValue.Add(yCloseValue);
                    highValue.Add(yHighValue);
                    lowValue.Add(yLowValue);
                }
            }
        }

        private void TransformToScreenCoHorizontal()
        {
            float xValue, x1Value, x2Value, yOpenValue, yCloseValue, yHighValue, yLowValue;
            double x1Val, x2Val, openVal, closeVal, yHiVal, yLoVal;
            double sbsCenter = sbsInfo.Median;
            double sbsStart = sbsInfo.Start;
            double sbsEnd = sbsInfo.End;
            double tempOpenVal;
            if (!fastCandleBitmapSeries.IsIndexed)
            {
                for (int i = 0; i < count; i++)
                {
                    x1Val = x_isInversed ? (xChartVals[i] + sbsEnd) : (xChartVals[i] + sbsStart);
                    x2Val = x_isInversed ? (xChartVals[i] + sbsStart) : (xChartVals[i] + sbsEnd);
                    openVal = y_isInversed
                                  ? closeChartVals[i] > yStart
                                        ? yStart
                                        : closeChartVals[i] < yEnd ? yEnd : closeChartVals[i]
                                  : openChartVals[i] > yEnd
                                        ? yEnd
                                        : openChartVals[i] < yStart ? yStart : openChartVals[i];
                    closeVal = y_isInversed
                                   ? openChartVals[i] < yEnd
                                         ? yEnd
                                         : openChartVals[i] > yStart ? yStart : openChartVals[i]
                                   : closeChartVals[i] < yStart
                                         ? yStart
                                         : closeChartVals[i] > yEnd ? yEnd : closeChartVals[i];
                    if (y_isInversed ? openVal > closeVal : openVal < closeVal)
                    {
                        isBull.Add(true);
                        tempOpenVal = openVal;
                        openVal = closeVal;
                        closeVal = tempOpenVal;
                    }
                    else
                        isBull.Add(false);
                    yHiVal = highChartVals[i];
                    yLoVal = lowChartVals[i];
                    xValue = (float)(xOffset + (xSize) * ((xChartVals[i] - xStart + sbsCenter) / xDelta));
                    yHighValue = (float)(yOffset + (ySize) * (1 - ((yHiVal - yStart) / yDelta)));
                    yLowValue = (float)(yOffset + (ySize) * (1 - ((yLoVal - yStart) / yDelta)));
                    x1Value = (float)(xOffset + (xSize) * ((x1Val - xStart) / xDelta));
                    x2Value = (float)(xOffset + (xSize) * ((x2Val - xStart) / xDelta));
                    yOpenValue = (float)(yOffset + (ySize) * (1 - ((openVal - yStart) / yDelta)));
                    yCloseValue = (float)(yOffset + (ySize) * (1 - ((closeVal - yStart) / yDelta)));

                    xValues.Add(xValue);
                    x1Values.Add(x1Value);
                    x2Values.Add(x2Value);
                    openValue.Add(yOpenValue);
                    closeValue.Add(yCloseValue);
                    highValue.Add(yHighValue);
                    lowValue.Add(yLowValue);

                }
            }
            else
            {

                for (int i = 0; i < count; i++)
                {
                    x1Val = !x_isInversed ? (sbsStart + i) : (sbsEnd + i);
                    x2Val = !x_isInversed ? (sbsEnd + i) : (sbsStart + i);
                    openVal = y_isInversed
                                  ? closeChartVals[i] > yStart
                                        ? yStart
                                        : closeChartVals[i] < yEnd ? yEnd : closeChartVals[i]
                                  : openChartVals[i] > yEnd
                                        ? yEnd
                                        : openChartVals[i] < yStart ? yStart : openChartVals[i];
                    closeVal = y_isInversed
                                   ? openChartVals[i] < yEnd
                                         ? yEnd
                                         : openChartVals[i] > yStart ? yStart : openChartVals[i]
                                   : closeChartVals[i] < yStart
                                         ? yStart
                                         : closeChartVals[i] > yEnd ? yEnd : closeChartVals[i];
                    if (y_isInversed ? openVal > closeVal : openVal < closeVal)
                    {
                        isBull.Add(true);
                        tempOpenVal = openVal;
                        openVal = closeVal;
                        closeVal = tempOpenVal;
                    }
                    else
                        isBull.Add(false);

                    yHiVal = highChartVals[i];
                    yLoVal = lowChartVals[i];
                    xValue = (float)(xOffset + (xSize) * ((i - xStart + sbsCenter) / xDelta));
                    yHighValue = (float)(yOffset + (ySize) * (1 - ((yHiVal - yStart) / yDelta)));
                    yLowValue = (float)(yOffset + (ySize) * (1 - ((yLoVal - yStart) / yDelta)));
                    x1Value = (float)(xOffset + (xSize) * ((x1Val - xStart) / xDelta));
                    x2Value = (float)(xOffset + (xSize) * ((x2Val - xStart) / xDelta));
                    yOpenValue = (float)(yOffset + (ySize) * (1 - ((openVal - yStart) / yDelta)));
                    yCloseValue = (float)(yOffset + (ySize) * (1 - ((closeVal - yStart) / yDelta)));
                    xValues.Add(xValue);
                    x1Values.Add(x1Value);
                    x2Values.Add(x2Value);
                    openValue.Add(yOpenValue);
                    closeValue.Add(yCloseValue);
                    highValue.Add(yHighValue);
                    lowValue.Add(yLowValue);

                }
            }
        }

        /// <summary>
        /// Transforms for logarithmic axis
        /// </summary>
        /// <param name="transformer"></param>
        void TransformToScreenCoInLog(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;
            if (!fastCandleBitmapSeries.IsActualTransposed)
                TransformToScreenCoInLogHorizontal(xBase, yBase);
            else
                TransformToScreenCoInLogVertical(xBase, yBase);
        }

        private void TransformToScreenCoInLogVertical(double xBase, double yBase)
        {
            float xValue, x1Value, x2Value, yOpenValue, yCloseValue, yHighValue, yLowValue;
            double x1Val, x2Val, openVal, closeVal, yHiVal, yLoVal;
            DoubleRange sbsInfo = fastCandleBitmapSeries.GetSideBySideInfo(fastCandleBitmapSeries);
            if (!fastCandleBitmapSeries.IsIndexed)
            {
                for (int j = 0; j < count; j++)
                {
                    double logx1 = xBase == 1 ? xChartVals[j] + sbsInfo.Start : Math.Log(xChartVals[j] + sbsInfo.Start, xBase);
                    double logx2 = xBase == 1 ? xChartVals[j] + sbsInfo.End : Math.Log(xChartVals[j] + sbsInfo.End, xBase);
                    double logOpen = yBase == 1 ? openChartVals[j] : Math.Log(openChartVals[j], yBase);
                    double logClose = yBase == 1 ? closeChartVals[j] : Math.Log(closeChartVals[j], yBase);
                    x1Val = x_isInversed ? logx2 < xEnd ? xEnd : logx2 : logx1 < xStart ? xStart : logx1;
                    x2Val = x_isInversed ? logx1 > xStart ? xStart : logx1 : logx2 > xEnd ? xEnd : logx2;

                    openVal = y_isInversed
                                  ? logClose > yStart ? yStart : logClose < yEnd ? yEnd : logClose
                                  : logOpen > yEnd ? yEnd : logOpen < yStart ? yStart : logOpen;
                    closeVal = y_isInversed
                                       ? logOpen < yEnd ? yEnd : logOpen > yStart ? yStart : logOpen
                                       : logClose < yStart ? yStart : logClose > yEnd ? yEnd : logClose;

                    if (!y_isInversed ? openVal < closeVal : openVal > closeVal)
                    {
                        double tempOpenVal = openVal;
                        openVal = closeVal;
                        closeVal = tempOpenVal;
                        isBull.Add(true);
                    }
                    else
                        isBull.Add(false);
                    yHiVal = yBase == 1 ? highChartVals[j] : Math.Log(highChartVals[j], yBase);
                    yLoVal = yBase == 1 ? lowChartVals[j] : Math.Log(lowChartVals[j], yBase);
                    xValue = (float)(xOffset + xSize * ((xEnd - sbsInfo.Median - xChartVals[j]) / xDelta));
                    yHighValue = (float)(yOffset+ySize * (1 - ((yEnd - yHiVal) / yDelta)));
                    yLowValue = (float)(yOffset + ySize * (1 - ((yEnd - yLoVal) / yDelta)));
                    x1Value = (float)(xOffset+xSize * ((x1Val - xStart) / xDelta));
                    x2Value = (float)(xOffset+xSize * ((x2Val - xStart) / xDelta));
                    yOpenValue = (float)(yOffset+ySize * (1 - ((openVal - yStart) / yDelta)));
                    yCloseValue = (float)(yOffset+ySize * (1 - ((closeVal - yStart) / yDelta)));
                    xValues.Add(xValue);
                    x1Values.Add(x1Value);
                    x2Values.Add(x2Value);
                    openValue.Add(yOpenValue);
                    closeValue.Add(yCloseValue);
                    highValue.Add(yHighValue);
                    lowValue.Add(yLowValue);
                }
            }
            else
            {
                for (int j = 0; j < count; j++)
                {
                    x1Val = x_isInversed
                                ? xBase == 1 ? (sbsInfo.End + j) : Math.Log((sbsInfo.End + j), xBase)
                                : xBase == 1 ? (sbsInfo.Start + j) : Math.Log((sbsInfo.Start + j), xBase);
                    x2Val = x_isInversed
                                ? xBase == 1 ? (sbsInfo.Start + j) : Math.Log((sbsInfo.Start + j), xBase)
                                : xBase == 1 ? (sbsInfo.End + j) : Math.Log((sbsInfo.End + j), xBase);

                    double openLog = yBase == 1 ? openChartVals[j] : Math.Log(openChartVals[j], yBase);
                    double closeLog = yBase == 1 ? closeChartVals[j] : Math.Log(closeChartVals[j], yBase);

                    openVal = y_isInversed
                                  ? closeLog > yStart ? yStart : closeLog < yEnd ? yEnd : closeLog
                                  : openLog > yEnd ? yEnd : openLog < yStart ? yStart : openLog;
                    closeVal = y_isInversed
                                   ? openLog < yEnd ? yEnd : openLog > yStart ? yStart : openLog
                                   : closeLog < yStart ? yStart : closeLog > yEnd ? yEnd : closeLog;

                    if (y_isInversed ? openVal > closeVal : openVal < closeVal)
                    {
                        double tempOpenVal = openVal;
                        openVal = closeVal;
                        closeVal = tempOpenVal;
                        isBull.Add(true);
                    }
                    else
                        isBull.Add(false);

                    yHiVal = yBase == 1 ? highChartVals[j] : Math.Log(highChartVals[j], yBase);
                    yLoVal = yBase == 1 ? lowChartVals[j] : Math.Log(lowChartVals[j], yBase);
                    xValue = (float)(xOffset + xSize * ((xEnd - sbsInfo.Median - j) / xDelta));
                    yHighValue = (float)(yOffset + ySize * (1 - ((yEnd - yHiVal) / yDelta)));
                    yLowValue = (float)(yOffset + ySize * (1 - ((yEnd - yLoVal) / yDelta)));
                    x1Value = (float)(xOffset + xSize * ((x1Val - xStart) / xDelta));
                    x2Value = (float)(xOffset + xSize * ((x2Val - xStart) / xDelta));
                    yOpenValue = (float)(yOffset + ySize * (1 - ((openVal - yStart) / yDelta)));
                    yCloseValue = (float)(yOffset + ySize * (1 - ((closeVal - yStart) / yDelta)));
                    xValues.Add(xValue);
                    x1Values.Add(x1Value);
                    x2Values.Add(x2Value);
                    openValue.Add(yOpenValue);
                    closeValue.Add(yCloseValue);
                    highValue.Add(yHighValue);
                    lowValue.Add(yLowValue);

                }
            }
        }

        private void TransformToScreenCoInLogHorizontal(double xBase, double yBase)
        {
            float xValue, x1Value, x2Value, yOpenValue, yCloseValue, yHighValue, yLowValue;
            double x1Val, x2Val, openVal, closeVal, yHiVal, yLoVal;
            DoubleRange sbsInfo = fastCandleBitmapSeries.GetSideBySideInfo(fastCandleBitmapSeries);
            if (!fastCandleBitmapSeries.IsIndexed)
            {
                for (int j = 0; j < count; j++)
                {
                    double logx1 = xBase == 1 ? xChartVals[j] + sbsInfo.Start : Math.Log(xChartVals[j] + sbsInfo.Start, xBase);
                    double logx2 = xBase == 1 ? xChartVals[j] + sbsInfo.End : Math.Log(xChartVals[j] + sbsInfo.End, xBase);
                    double logOpen = yBase == 1 ? openChartVals[j] : Math.Log(openChartVals[j], yBase);
                    double logClose = yBase == 1 ? closeChartVals[j] : Math.Log(closeChartVals[j], yBase);
                    x1Val = x_isInversed ? logx2 < xEnd ? xEnd : logx2 : logx1 < xStart ? xStart : logx1;
                    x2Val = x_isInversed ? logx1 > xStart ? xStart : logx1 : logx2 > xEnd ? xEnd : logx2;
                    
                    openVal = y_isInversed
                                  ? logClose > yStart ? yStart : logClose < yEnd ? yEnd : logClose
                                  : logOpen > yEnd ? yEnd : logOpen < yStart ? yStart : logOpen;
                    closeVal = y_isInversed
                                       ? logOpen < yEnd ? yEnd : logOpen > yStart ? yStart : logOpen
                                       : logClose < yStart ? yStart : logClose > yEnd ? yEnd : logClose;

                    if (!y_isInversed ? openVal < closeVal : openVal > closeVal)
                    {
                        double tempOpenVal = openVal;
                        openVal = closeVal;
                        closeVal = tempOpenVal;
                        isBull.Add(true);
                    }
                    else
                        isBull.Add(false);
                    yHiVal = yBase == 1 ? highChartVals[j] : Math.Log(highChartVals[j], yBase);
                    yLoVal = yBase == 1 ? lowChartVals[j] : Math.Log(lowChartVals[j], yBase);
                    xValue = (float)(xOffset + xSize * ((xChartVals[j] - xStart - sbsInfo.Median) / xDelta));
                    x1Value = (float)(xOffset + xSize * ((x1Val - xStart) / xDelta));
                    x2Value = (float)(xOffset + xSize * ((x2Val - xStart) / xDelta));
                    yOpenValue = (float)(yOffset + ySize * (1 - ((openVal - yStart) / yDelta)));
                    yCloseValue = (float)(yOffset + ySize * (1 - ((closeVal - yStart) / yDelta)));
                    yHighValue = (float)(yOffset + ySize * (1 - ((yHiVal - yStart) / yDelta)));
                    yLowValue = (float)(yOffset + ySize * (1 - ((yLoVal - yStart) / yDelta)));
                    xValues.Add(xValue);
                    x1Values.Add(x1Value);
                    x2Values.Add(x2Value);
                    openValue.Add(yOpenValue);
                    closeValue.Add(yCloseValue);
                    highValue.Add(yHighValue);
                    lowValue.Add(yLowValue);
                }
            }
            else
            {
                for (int j = 0; j < count; j++)
                {
                    x1Val = x_isInversed
                                ? xBase == 1 ? (sbsInfo.End + j) : Math.Log((sbsInfo.End + j), xBase)
                                : xBase == 1 ? (sbsInfo.Start + j) : Math.Log((sbsInfo.Start + j), xBase);
                    x2Val = x_isInversed
                                ? xBase == 1 ? (sbsInfo.Start + j) : Math.Log((sbsInfo.Start + j), xBase)
                                : xBase == 1 ? (sbsInfo.End + j) : Math.Log((sbsInfo.End + j), xBase);

                    double openLog = yBase == 1 ? openChartVals[j] : Math.Log(openChartVals[j], yBase);
                    double closeLog = yBase == 1 ? closeChartVals[j] : Math.Log(closeChartVals[j], yBase);

                    openVal = y_isInversed
                                  ? closeLog > yStart ? yStart : closeLog < yEnd ? yEnd : closeLog
                                  : openLog > yEnd ? yEnd : openLog < yStart ? yStart : openLog;
                    closeVal = y_isInversed
                                   ? openLog < yEnd ? yEnd : openLog > yStart ? yStart : openLog
                                   : closeLog < yStart ? yStart : closeLog > yEnd ? yEnd : closeLog;
                    
                    if (y_isInversed ? openVal > closeVal : openVal < closeVal)
                    {
                        double tempOpenVal = openVal;
                        openVal = closeVal;
                        closeVal = tempOpenVal;
                        isBull.Add(true);
                    }
                    else
                        isBull.Add(false);
                    yHiVal = yBase == 1 ? highChartVals[j] : Math.Log(highChartVals[j], yBase);
                    yLoVal = yBase == 1 ? lowChartVals[j] : Math.Log(lowChartVals[j], yBase);

                    xValue = (float)(xOffset + (xSize) * ((xChartVals[j] - xStart + sbsInfo.Median) / xDelta));
                    yHighValue = (float)(yOffset + (ySize) * (1 - ((yHiVal - yStart) / yDelta)));
                    yLowValue = (float)(yOffset + (ySize) * (1 - ((yLoVal - yStart) / yDelta)));
                    x1Value = (float)(xOffset + (xSize) * ((x1Val - xStart) / xDelta));
                    x2Value = (float)(xOffset + (xSize) * ((x2Val - xStart) / xDelta));
                    yOpenValue = (float)(yOffset + (ySize) * (1 - ((openVal - yStart) / yDelta)));
                    yCloseValue = (float)(yOffset + (ySize) * (1 - ((closeVal - yStart) / yDelta)));
                    
                    xValues.Add(xValue);
                    x1Values.Add(x1Value);
                    x2Values.Add(x2Value);
                    openValue.Add(yOpenValue);
                    closeValue.Add(yCloseValue);
                    highValue.Add(yHighValue);
                    lowValue.Add(yLowValue);
                  
                }
            }



        }

        internal void UpdateVisual()
        {
            Color color= new Color();
            if (bitmap != null && xValues.Count != 0)
            {
#if !WINDOWS_PHONE
                fastBuffer = fastCandleBitmapSeries.Area.GetFastBuffer();
#endif

                int width = (int)fastCandleBitmapSeries.Area.SeriesClipRect.Width;
                int height = (int)fastCandleBitmapSeries.Area.SeriesClipRect.Height;

                int leftThickness = (int)fastCandleBitmapSeries.StrokeThickness / 2;
                int rightThickness = (int)(fastCandleBitmapSeries.StrokeThickness % 2 == 0.0
                    ? (fastCandleBitmapSeries.StrokeThickness / 2) : (fastCandleBitmapSeries.StrokeThickness / 2) + 1);
#if WPF
                bitmap.BeginWrite();
#endif
                if (fastCandleBitmapSeries is FastCandleBitmapSeries)
                {
                    if (!fastCandleBitmapSeries.IsActualTransposed)
                        UpdateVisualHorizontal(width, height, color, leftThickness, rightThickness);
                    else
                        UpdateVisualVertical(width, height, color, leftThickness, rightThickness);
                }
#if WPF
                bitmap.EndWrite();
#endif
            }
            fastCandleBitmapSeries.Area.CanRenderToBuffer = true;
        }

        private void UpdateVisualVertical(int width, int height, Color color, int leftThickness, int rightThickness)
        {
            float x, x1, x2, open, close, high, low;
            int dataCount = xValues.Count;
            for (int i = 0; i < dataCount; i++)
            {
                if (isBull[i])
                    color = (fastCandleBitmapSeries as FastCandleBitmapSeries).BullFillColor != null ? color = ((SolidColorBrush)(fastCandleBitmapSeries as FastCandleBitmapSeries).BullFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                else
                    color = (fastCandleBitmapSeries as FastCandleBitmapSeries).BearFillColor != null ? color = ((SolidColorBrush)(fastCandleBitmapSeries as FastCandleBitmapSeries).BearFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                x = xValues[i];
                x1 = x1Values[i];
                x2 = x2Values[i];
                open = openChartVals[i] > 0 ? openValue[i] : closeValue[i];
                close = openChartVals[i] > 0 ? closeValue[i] : openValue[i];
                high = highValue[i];
                low = lowValue[i];
                var leftOffset = (int)x - leftThickness;
                var rightOffset = (int)x + rightThickness;
#if WINDOWS_PHONE
                        bitmap.FillRectangle((int)(width - close), (int)(height - x2), (int)(width - open), (int)(height - x1), color);
                        bitmap.FillRectangle((int)low, (int)(leftOffset), (int)high, (int)(rightOffset), color);
#else
                bitmap.FillRectangle(fastBuffer, (int)width, (int)height, (int)(width - close), (int)(height - x2), (int)(width - open), (int)(height - x1), color);
                bitmap.FillRectangle(fastBuffer, width, height, (int)low, (int)(leftOffset), (int)high, (int)(rightOffset), color);
#endif

            }
        }

        private void UpdateVisualHorizontal(int width, int height, Color color, int leftThickness, int rightThickness)
        {
            float x, x1, x2, open, close, high, low;
            int dataCount = xValues.Count;
            for (int i = 0; i < dataCount; i++)
            {
                if (isBull[i])
                    color = (fastCandleBitmapSeries as FastCandleBitmapSeries).BullFillColor != null ? color = ((SolidColorBrush)(fastCandleBitmapSeries as FastCandleBitmapSeries).BullFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                else
                    color = (fastCandleBitmapSeries as FastCandleBitmapSeries).BearFillColor != null ? color = ((SolidColorBrush)(fastCandleBitmapSeries as FastCandleBitmapSeries).BearFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                x = xValues[i];
                x1 = x1Values[i];
                x2 = x2Values[i];
                open = openChartVals[i] > 0 ? openValue[i] : closeValue[i];
                close = openChartVals[i] > 0 ? closeValue[i] : openValue[i];
                high = highValue[i];
                low = lowValue[i];
                var leftOffset = x - leftThickness;
                var rightOffset = x + rightThickness;
#if WINDOWS_PHONE
                        bitmap.FillRectangle((int)(x1), (int)open, (int)x2, (int)close, color);
                        bitmap.FillRectangle((int)leftOffset, (int)high, (int)rightOffset, (int)low, color);
#else
                bitmap.FillRectangle(fastBuffer, width, height, (int)(x1), (int)open, (int)x2, (int)close, color);
                bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)high, (int)rightOffset, (int)low, color);
#endif

            }
        }

        public override void OnSizeChanged(Size size)
        {
        }
        #endregion
    }
}
