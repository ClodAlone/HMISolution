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
using System.Windows.Media.Imaging;
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
    /// Represents chart fast hilo open close bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastLineBitmapSegment"/>
    /// <seealso cref="FastHiLoSegment"/>
    [ClassReference(IsReviewed = false)]
    public class FastHiLoOpenCloseSegment : ChartSegment
    {
        #region fields

        internal AdornmentSeries fastHiLoOpenCloseSeries;

        private WriteableBitmap bitmap;

        private Brush stroke;

        private byte[] fastBuffer;

        private Size availableSize;

        private IList<double> xChartVals;

        private IList<double> yHiChartVals;

        private IList<double> yLoChartVals;

        private IList<double> yOpenChartVals;

        private IList<double> yCloseChartVals;

        double xStart, xEnd, yStart, yEnd, xDelta, yDelta, xSize, ySize, offsetLeft, offsetRight, offsetTop;

        bool x_isInversed, y_isInversed;

        double xTolerance, yTolerance;

        int start, count;

        DoubleRange sbsInfo;

        double median,center,Left,Right;

        #endregion

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public FastHiLoOpenCloseSegment()
        {

        }

        /// <summary>
        /// Called when instance created for FastHiLoOpenCloseSegment
        /// </summary>
        /// <param name="series"></param>
        public FastHiLoOpenCloseSegment(AdornmentSeries series)
        {
            stroke = series.Stroke;
            fastHiLoOpenCloseSeries = series;
        }

        /// <summary>
        /// Called when instance created for FastHiLoOpenCloseSegment with following arguments
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="highValues"></param>
        /// <param name="lowValues"></param>
        /// <param name="openValues"></param>
        /// <param name="closeValues"></param>
        /// <param name="series"></param>
        public FastHiLoOpenCloseSegment(List<double> xValues, IList<double> highValues, IList<double> lowValues, IList<double> openValues, IList<double> closeValues, AdornmentSeries series)
            : this(series)
        {
            base.Series = series;
            this.SetData(xValues, highValues, lowValues, openValues, closeValues);
            
        }

        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yHiValues"></param>
        /// <param name="yLowValues"></param>
        /// /// <param name="yOpenValues"></param>
        /// /// <param name="yCloseValues"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(IList<double> xValues, IList<double> yHiValues, IList<double> yLowValues, IList<double> yOpenValues, IList<double> yCloseValues)
        {
            DoubleRange sbsInfo = fastHiLoOpenCloseSeries.GetSideBySideInfo(fastHiLoOpenCloseSeries);
            this.xChartVals = xValues;
            this.yHiChartVals = yHiValues;
            this.yLoChartVals = yLowValues;
            this.yOpenChartVals = yOpenValues;
            this.yCloseChartVals = yCloseValues;
            List<double>yValues = new List<double>();
            yValues.AddRange(yHiValues);
            yValues.AddRange(yLowValues);

            if (fastHiLoOpenCloseSeries.DataCount > 1)
            {
                double X_MAX, Y_MAX, X_MIN, Y_MIN;
                if (fastHiLoOpenCloseSeries.IsIndexed)
                {
                    X_MAX = fastHiLoOpenCloseSeries.DataCount - 1;
                    X_MIN = 0;
                }
                else
                {
                    X_MAX = xChartVals.Max();
                    X_MIN = xChartVals.Min();
                   
                }
                Y_MIN = yValues.Min();
                Y_MAX = yValues.Max();
                X_MIN += sbsInfo.Start;
                X_MAX += sbsInfo.End;
                XRange = new DoubleRange(X_MIN, X_MAX);
                YRange = new DoubleRange(Y_MIN, Y_MAX);
            }
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
            bitmap = fastHiLoOpenCloseSeries.Area.GetFastRenderSurface();
            fastBuffer = fastHiLoOpenCloseSeries.Area.GetFastBuffer();
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
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            bitmap = fastHiLoOpenCloseSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastHiLoOpenCloseSeries.DataCount > 1)
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

                if (fastHiLoOpenCloseSeries.IsActualTransposed)
                {
                    xSize = cartesianTransformer.XAxis.RenderedRect.Height;
                    ySize = cartesianTransformer.YAxis.RenderedRect.Width;
                    offsetLeft = cartesianTransformer.YAxis.RenderedRect.Left - fastHiLoOpenCloseSeries.Area.SeriesClipRect.Left;
                    offsetRight = cartesianTransformer.YAxis.RenderedRect.Right - fastHiLoOpenCloseSeries.Area.SeriesClipRect.Right;
                    offsetTop = cartesianTransformer.XAxis.RenderedRect.Top - fastHiLoOpenCloseSeries.Area.SeriesClipRect.Top;
                }
                else
                {
                    xSize = cartesianTransformer.XAxis.RenderedRect.Width;
                    ySize = cartesianTransformer.YAxis.RenderedRect.Height;
                    offsetLeft = cartesianTransformer.XAxis.RenderedRect.Left - fastHiLoOpenCloseSeries.Area.SeriesClipRect.Left;
                    offsetRight = cartesianTransformer.XAxis.RenderedRect.Right - fastHiLoOpenCloseSeries.Area.SeriesClipRect.Right;
                    offsetTop = cartesianTransformer.YAxis.RenderedRect.Top - fastHiLoOpenCloseSeries.Area.SeriesClipRect.Top;
                }
                xTolerance = Math.Abs((xDelta * 1) / xSize);
                yTolerance = Math.Abs((yDelta * 1) / ySize);
                
                count = (int)(Math.Ceiling(xEnd));
                start = (int)(Math.Floor(xStart));
                start = start < 0 ? 0 : start;
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
                yHiValues.Clear();
                yLoValues.Clear();
                yOpenStartValues.Clear();
                yOpenEndValues.Clear();
                yCloseValues.Clear();
                yCloseEndValues.Clear();
                isBull.Clear();
                sbsInfo = (fastHiLoOpenCloseSeries as ChartSeriesBase).GetSideBySideInfo(fastHiLoOpenCloseSeries as ChartSeriesBase);
                median = sbsInfo.Delta / 2;
                center = sbsInfo.Median;
                Left = sbsInfo.Start;
                Right = sbsInfo.End;
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

        List<float> xValues = new List<float>();
        List<float> yHiValues = new List<float>();
        List<float> yLoValues = new List<float>();
        List<float> yOpenStartValues = new List<float>();
        List<float> yOpenEndValues = new List<float>();
        List<float> yCloseValues = new List<float>();
        List<float> yCloseEndValues = new List<float>();
        List<bool> isBull = new List<bool>();

        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        void TransformToScreenCo(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            if (!fastHiLoOpenCloseSeries.IsActualTransposed)
                TransformToScreenCoHorizontal();
            else
                TransformToScreenCoVertical();
        }

        private void TransformToScreenCoHorizontal()
        {
            float xValue = 0;
            float yHiValue = 0;
            float yLoValue = 0;
            float yOpenStartValue = 0;
            float yOpenEndValue = 0;
            float yCloseValue = 0;
            float yCloseEndValue = 0;
            if (fastHiLoOpenCloseSeries.IsIndexed)
            {
                for (int j = 0; j < count; j++)
                {
                    double yVal = yHiChartVals[j];
                    double yVal1 = yLoChartVals[j];
                    double yVal2 = yOpenChartVals[j];
                    double yVal3 = yCloseChartVals[j];
                    xValue = (float)(offsetLeft + xSize * ((j - xStart + center) / xDelta));
                    yHiValue = (float)(offsetTop + ySize * (1 - ((yVal - yStart) / yDelta)));
                    yLoValue = (float)(offsetTop + ySize * (1 - ((yVal1 - yStart) / yDelta)));
                    yOpenStartValue = (float)(offsetTop + ySize * (1 - ((yVal2 - yStart) / yDelta)));
                    yCloseValue = (float)(offsetTop + ySize * (1 - ((yVal3 - yStart) / yDelta)));
                    yOpenEndValue = (float)(offsetLeft + xSize * ((j - (xStart - Left)) / xDelta));
                    yCloseEndValue = (float)(offsetLeft + xSize * ((j - (xStart - Right)) / xDelta));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                    yOpenStartValues.Add(yOpenStartValue);
                    yOpenEndValues.Add(yOpenEndValue);
                    yCloseValues.Add(yCloseValue);
                    yCloseEndValues.Add(yCloseEndValue);
                    if (yVal2 < yVal3)
                        isBull.Add(true);
                    else
                        isBull.Add(false);
                }
            }
            else
            {
                for (int i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xChartVals[i];
                    double yHiVal = yHiChartVals[i];
                    double yLoVal = yLoChartVals[i];
                    double yOpenVal = yOpenChartVals[i];
                    double yCloseVal = yCloseChartVals[i];
                    xValue = (float)(offsetLeft + xSize * ((xVal - xStart + center) / xDelta));
                    yHiValue = (float)(offsetTop + ySize * (1 - ((yHiVal - yStart) / yDelta)));
                    yLoValue = (float)(offsetTop + ySize * (1 - ((yLoVal - yStart) / yDelta)));
                    yOpenStartValue = (float)(offsetTop + ySize * (1 - ((yOpenVal - yStart) / yDelta)));
                    yCloseValue = (float)(offsetTop + ySize * (1 - ((yCloseVal - yStart) / yDelta)));
                    yOpenEndValue = (float)(offsetLeft + xSize * ((xVal - (xStart - Left)) / xDelta));
                    yCloseEndValue = (float)(offsetLeft + xSize * ((xVal - (xStart - Right)) / xDelta));
             
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                    yOpenStartValues.Add(yOpenStartValue);
                    yCloseValues.Add(yCloseValue);
                    yOpenEndValues.Add(yOpenEndValue);
                    yCloseEndValues.Add(yCloseEndValue);
                    if (yOpenVal < yCloseVal)
                        isBull.Add(true);
                    else
                        isBull.Add(false);
                }
            }
        }

        private void TransformToScreenCoVertical()
        {
            float xValue = 0;
            float yHiValue = 0;
            float yLoValue = 0;
            float yOpenStartValue = 0;
            float yOpenEndValue = 0;
            float yCloseValue = 0;
            float yCloseEndValue = 0;
            if (fastHiLoOpenCloseSeries.IsIndexed)
            {
                for (int j = 0; j < count; j++)
                {
                    double yVal = yHiChartVals[j];
                    double yVal1 = yLoChartVals[j];
                    double yVal2 = yOpenChartVals[j];
                    double yVal3 = yCloseChartVals[j];
                    if (!fastHiLoOpenCloseSeries.IsActualTransposed)
                    {
                        xValue = (float)(offsetLeft + xSize * ((j - xStart + center) / xDelta));
                        yHiValue = (float)(offsetTop + ySize * (1 - ((yVal - yStart) / yDelta)));
                        yLoValue = (float)(offsetTop + ySize * (1 - ((yVal1 - yStart) / yDelta)));
                        yOpenStartValue = (float)(offsetTop + ySize * (1 - ((yVal2 - yStart) / yDelta)));
                        yCloseValue = (float)(offsetTop + ySize * (1 - ((yVal3 - yStart) / yDelta)));
                        yOpenEndValue = (float)(offsetLeft + xSize * ((j - (xStart - Left)) / xDelta));
                        yCloseEndValue = (float)(offsetLeft + xSize * ((j - (xStart - Right)) / xDelta));
                    }
                    else
                    {
                        xValue = (float)(offsetLeft + xSize * ((xEnd + center - j) / xDelta));
                        yHiValue = (float)(offsetTop + ySize * (1 - ((yEnd - yVal) / yDelta)));
                        yLoValue = (float)(offsetTop + ySize * (1 - ((yEnd - yVal1) / yDelta)));
                        yOpenStartValue = (float)(offsetTop + ySize * (1 - ((yEnd - yVal2) / yDelta)));
                        yCloseValue = (float)(offsetTop + ySize * (1 - ((yEnd - yVal3) / yDelta)));
                        yOpenEndValue = (float)(offsetLeft + xSize * (((xEnd - Left) - j) / xDelta));
                        yCloseEndValue = (float)(offsetLeft + xSize * (((xEnd - Right) - j) / xDelta));
                    }
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                    yOpenStartValues.Add(yOpenStartValue);
                    yOpenEndValues.Add(yOpenEndValue);
                    yCloseValues.Add(yCloseValue);
                    yCloseEndValues.Add(yCloseEndValue);
                    if (yVal2 < yVal3)
                        isBull.Add(true);
                    else
                        isBull.Add(false);
                }
            }
            else
            {
                for (int i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xChartVals[i];
                    double yHiVal = yHiChartVals[i];
                    double yLoVal = yLoChartVals[i];
                    double yOpenVal = yOpenChartVals[i];
                    double yCloseVal = yCloseChartVals[i];
                    xValue = (float)(offsetLeft + xSize * ((xEnd + center - xVal) / xDelta));
                    yHiValue = (float)(offsetTop + ySize * (1 - ((yEnd - yHiVal) / yDelta)));
                    yLoValue = (float)(offsetTop + ySize * (1 - ((yEnd - yLoVal) / yDelta)));
                    yOpenStartValue = (float)(offsetTop + ySize * (1 - ((yEnd - yOpenVal) / yDelta)));
                    yCloseValue = (float)(offsetTop + ySize * (1 - ((yEnd - yCloseVal) / yDelta)));
                    yOpenEndValue = (float)(offsetLeft + xSize * (((xEnd - Left) - xVal) / xDelta));
                    yCloseEndValue = (float)(offsetLeft + xSize * (((xEnd - Right) - xVal) / xDelta));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                    yOpenStartValues.Add(yOpenStartValue);
                    yCloseValues.Add(yCloseValue);
                    yOpenEndValues.Add(yOpenEndValue);
                    yCloseEndValues.Add(yCloseEndValue);
                    if (yOpenVal < yCloseVal)
                        isBull.Add(true);
                    else
                        isBull.Add(false);
                }
            }
        }

        /// <summary>
        /// Transforms for logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        void TransformToScreenCoInLog(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;
            if (!fastHiLoOpenCloseSeries.IsActualTransposed)
                TransformToScreenCoInLogHorizontal(xBase, yBase);
            else
                TransformToScreenCoInLogVeritcal(xBase, yBase);
        }

        private void TransformToScreenCoInLogHorizontal(double xBase, double yBase)
        {
            float xValue = 0;
            float yHiValue = 0;
            float yLoValue = 0;
            float yOpenStartValue = 0;
            float yOpenEndValue = 0;
            float yCloseValue = 0;
            float yCloseEndValue = 0;
            if (fastHiLoOpenCloseSeries.IsIndexed)
            {
                for (int i = start; i < count; i++)
                {
                    double yHiVal = yBase == 1 ? yHiChartVals[i] : Math.Log(yHiChartVals[i], yBase);
                    double yLoVal = yBase == 1 ? yLoChartVals[i] : Math.Log(yLoChartVals[i], yBase);
                    double yOpenVal = yBase == 1 ? yOpenChartVals[i] : Math.Log(yOpenChartVals[i], yBase);
                    double yCloseVal = yBase == 1 ? yCloseChartVals[i] : Math.Log(yCloseChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);

                    xValue = (float)(offsetLeft + xSize * ((xVal - xStart + center) / xDelta));
                    yHiValue = (float)(offsetTop + ySize * (1 - ((yHiVal - yStart) / yDelta)));
                    yLoValue = (float)(offsetTop + ySize * (1 - ((yLoVal - yStart) / yDelta)));
                    yOpenStartValue = (float)(offsetTop + ySize * (1 - ((yOpenVal - yStart) / yDelta)));
                    yCloseValue = (float)(offsetTop + ySize * (1 - ((yCloseVal - yStart) / yDelta)));
                    yOpenEndValue = (float)(offsetLeft + xSize * ((xVal - (xStart - Left)) / xDelta));
                    yCloseEndValue = (float)(offsetLeft + xSize * ((xVal - (xStart - Right)) / xDelta));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                    yOpenStartValues.Add(yOpenStartValue);
                    yCloseValues.Add(yCloseValue);
                    yOpenEndValues.Add(yOpenEndValue);
                    yCloseEndValues.Add(yCloseEndValue);
                    if (yOpenVal < yCloseVal)
                        isBull.Add(true);
                    else
                        isBull.Add(false);
                }
            }
            else
            {
                for (int i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    double yHiVal = yBase == 1 ? yHiChartVals[i] : Math.Log(yHiChartVals[i], yBase);
                    double yLoVal = yBase == 1 ? yLoChartVals[i] : Math.Log(yLoChartVals[i], yBase);
                    double yOpenVal = yBase == 1 ? yOpenChartVals[i] : Math.Log(yOpenChartVals[i], yBase);
                    double yCloseVal = yBase == 1 ? yCloseChartVals[i] : Math.Log(yCloseChartVals[i], yBase);
                    xValue = (float)(offsetLeft + xSize * ((xVal - xStart + center) / xDelta));
                    yHiValue = (float)(offsetTop + ySize * (1 - ((yHiVal - yStart) / yDelta)));
                    yLoValue = (float)(offsetTop + ySize * (1 - ((yLoVal - yStart) / yDelta)));
                    yOpenStartValue = (float)(offsetTop + ySize * (1 - ((yOpenVal - yStart) / yDelta)));
                    yCloseValue = (float)(offsetTop + ySize * (1 - ((yCloseVal - yStart) / yDelta)));
                    yOpenEndValue = (float)(offsetLeft + xSize * ((xVal - (xStart - Left)) / xDelta));
                    yCloseEndValue = (float)(offsetLeft + xSize * ((xVal - (xStart - Right)) / xDelta));

                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                    yOpenStartValues.Add(yOpenStartValue);
                    yCloseValues.Add(yCloseValue);
                    yOpenEndValues.Add(yOpenEndValue);
                    yCloseEndValues.Add(yCloseEndValue);
                    if (yOpenVal < yCloseVal)
                        isBull.Add(true);
                    else
                        isBull.Add(false);
                }
            }

        }

        private void TransformToScreenCoInLogVeritcal(double xBase, double yBase)
        {
            float xValue = 0;
            float yHiValue = 0;
            float yLoValue = 0;
            float yOpenStartValue = 0;
            float yOpenEndValue = 0;
            float yCloseValue = 0;
            float yCloseEndValue = 0;
            if (fastHiLoOpenCloseSeries.IsIndexed)
            {
                for (int i = start; i <= count; i++)
                {
                    double yHiVal = yBase == 1 ? yHiChartVals[i] : Math.Log(yHiChartVals[i], yBase);
                    double yLoVal = yBase == 1 ? yLoChartVals[i] : Math.Log(yLoChartVals[i], yBase);
                    double yOpenVal = yBase == 1 ? yOpenChartVals[i] : Math.Log(yOpenChartVals[i], yBase);
                    double yCloseVal = yBase == 1 ? yCloseChartVals[i] : Math.Log(yCloseChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);

                    xValue = (float)(offsetLeft + xSize * ((xEnd + center - xVal) / xDelta));
                    yHiValue = (float)(offsetTop + ySize * (1 - ((yEnd - yHiVal) / yDelta)));
                    yLoValue = (float)(offsetTop + ySize * (1 - ((yEnd - yLoVal) / yDelta)));
                    yOpenStartValue = (float)(offsetTop + ySize * (1 - ((yEnd - yOpenVal) / yDelta)));
                    yCloseValue = (float)(offsetTop + ySize * (1 - ((yEnd - yCloseVal) / yDelta)));
                    yOpenEndValue = (float)(offsetLeft + xSize * (((xEnd - Left) - xVal) / xDelta));
                    yCloseEndValue = (float)(offsetLeft + xSize * (((xEnd - Right) - xVal) / xDelta));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                    yOpenStartValues.Add(yOpenStartValue);
                    yCloseValues.Add(yCloseValue);
                    yOpenEndValues.Add(yOpenEndValue);
                    yCloseEndValues.Add(yCloseEndValue);
                    if (yOpenVal < yCloseVal)
                        isBull.Add(true);
                    else
                        isBull.Add(false);
                }
            }
            else
            {
                for (int i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    double yHiVal = yBase == 1 ? yHiChartVals[i] : Math.Log(yHiChartVals[i], yBase);
                    double yLoVal = yBase == 1 ? yLoChartVals[i] : Math.Log(yLoChartVals[i], yBase);
                    double yOpenVal = yBase == 1 ? yOpenChartVals[i] : Math.Log(yOpenChartVals[i], yBase);
                    double yCloseVal = yBase == 1 ? yCloseChartVals[i] : Math.Log(yCloseChartVals[i], yBase);
                    xValue = (float)(offsetLeft + xSize * ((xEnd + center - xVal) / xDelta));
                    yHiValue = (float)(offsetTop + ySize * (1 - ((yEnd - yHiVal) / yDelta)));
                    yLoValue = (float)(offsetTop + ySize * (1 - ((yEnd - yLoVal) / yDelta)));
                    yOpenStartValue = (float)(offsetTop + ySize * (1 - ((yEnd - yOpenVal) / yDelta)));
                    yCloseValue = (float)(offsetTop + ySize * (1 - ((yEnd - yCloseVal) / yDelta)));
                    yOpenEndValue = (float)(offsetLeft + xSize * (((xEnd - Left) - xVal) / xDelta));
                    yCloseEndValue = (float)(offsetLeft + xSize * (((xEnd - Right) - xVal) / xDelta));

                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                    yOpenStartValues.Add(yOpenStartValue);
                    yCloseValues.Add(yCloseValue);
                    yOpenEndValues.Add(yOpenEndValue);
                    yCloseEndValues.Add(yCloseEndValue);
                    if (yOpenVal < yCloseVal)
                        isBull.Add(true);
                    else
                        isBull.Add(false);
                }


            }

        }

        internal void UpdateVisual(bool updateHiLoLine)
        {
            if (bitmap != null && xValues.Count > 1)
            {
#if !WINDOWS_PHONE
                fastBuffer = fastHiLoOpenCloseSeries.Area.GetFastBuffer();
#endif


                int width = (int)fastHiLoOpenCloseSeries.Area.SeriesClipRect.Width;
                int height = (int)fastHiLoOpenCloseSeries.Area.SeriesClipRect.Height;

                int leftThickness = (int)fastHiLoOpenCloseSeries.StrokeThickness / 2;
                int rightThickness = (int)(fastHiLoOpenCloseSeries.StrokeThickness % 2 == 0
                    ? (fastHiLoOpenCloseSeries.StrokeThickness / 2) : fastHiLoOpenCloseSeries.StrokeThickness / 2+1);
#if WPF
                bitmap.BeginWrite();
#endif

                if (fastHiLoOpenCloseSeries is FastHiLoOpenCloseBitmapSeries)
                {
                    if (!fastHiLoOpenCloseSeries.IsActualTransposed)
                        UpdateVisualHorizontal(width, height,leftThickness,rightThickness);
                    else
                        UpdateVisualVertical(width, height, leftThickness, rightThickness);
                }
#if WPF
                bitmap.EndWrite();
#endif
            }
            fastHiLoOpenCloseSeries.Area.CanRenderToBuffer = true;
        }

        private void UpdateVisualVertical(int width, int height,int leftThickness, int rightThickness)
        {
            float xStart = 0;
            float yStart = 0;
            float xEnd = 0;
            float yEnd = 0;
            float yOpen = 0;
            float yOpenEnd = 0;
            float yClose = 0;
            float yCloseEnd = 0;
            Color color;
            int leftOffset, rightOffset;
            for (int i = 0; i < xValues.Count; i++)
            {
                if (isBull[i])
                    color = (fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BullFillColor != null ? color = ((SolidColorBrush)(fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BullFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                else
                    color = (fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BearFillColor != null ? color = ((SolidColorBrush)(fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BearFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                xStart = xValues[i];
                yStart = yHiValues[i];
                xEnd = xValues[i];
                yEnd = yLoValues[i];
                yOpen = yOpenStartValues[i];
                yOpenEnd = yOpenEndValues[i];
                yClose = yCloseValues[i];
                yCloseEnd = yCloseEndValues[i];
#if WINDOWS_PHONE
                leftOffset = (int)xStart - leftThickness;
                rightOffset = (int)xStart + rightThickness;
                bitmap.FillRectangle((int)yEnd, leftOffset, (int)yStart, rightOffset, color);
                leftOffset = (int)yOpen - leftThickness;
                rightOffset = (int)yOpen + rightThickness;
                bitmap.FillRectangle(leftOffset, (int)xStart + leftThickness, rightOffset, (int)yOpenEnd, color);
                leftOffset = (int)yClose - leftThickness;
                rightOffset = (int)yClose + rightThickness;
                bitmap.FillRectangle(leftOffset, (int)yCloseEnd, rightOffset, (int)xStart-leftThickness, color);
#else
                leftOffset = (int)xStart - leftThickness;
                rightOffset = (int)xStart + rightThickness;
                bitmap.FillRectangle(fastBuffer, width, height, (int)yEnd, (int)leftOffset, (int)yStart, (int)rightOffset, color);
                leftOffset = (int)yOpen - leftThickness;
                rightOffset = (int)yOpen + rightThickness;
                bitmap.FillRectangle(fastBuffer, width, height, leftOffset, (int)xStart+ leftThickness , (int)rightOffset, (int)yOpenEnd, color);
                leftOffset = (int)yClose - leftThickness;
                rightOffset = (int)yClose + rightThickness;
                bitmap.FillRectangle(fastBuffer, width, height, leftOffset, (int)yCloseEnd, (int)rightOffset, (int)xStart- leftThickness, color);
#endif


            }
        }

        private void UpdateVisualHorizontal(int width, int height, int leftThickness, int rightThickness)
        {
            Color color;
            float xStart = 0;
            float yStart = 0;
            float xEnd = 0;
            float yEnd = 0;
            float yOpen = 0;
            float yOpenEnd = 0;
            float yClose = 0;
            float yCloseEnd = 0;
            int leftOffset, rightOffset;
            for (int i = 0; i < xValues.Count; i++)
            {
                if (isBull[i])
                    color = (fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BullFillColor != null ? color = ((SolidColorBrush)(fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BullFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                else
                    color = (fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BearFillColor != null ? color = ((SolidColorBrush)(fastHiLoOpenCloseSeries as FastHiLoOpenCloseBitmapSeries).BearFillColor).Color : ((SolidColorBrush)this.Interior).Color;
                xStart = xValues[i];
                yStart = yHiValues[i];
                xEnd = xValues[i];
                yEnd = yLoValues[i];
                yOpen = yOpenStartValues[i];
                yOpenEnd = yOpenEndValues[i];
                yClose = yCloseValues[i];
                yCloseEnd = yCloseEndValues[i];
#if WINDOWS_PHONE
                leftOffset = (int)xStart - leftThickness;
                rightOffset = (int)xStart + rightThickness;
                bitmap.FillRectangle(leftOffset, (int)yStart, rightOffset, (int)yEnd, color);
                leftOffset = (int)yOpen - leftThickness;
                rightOffset = (int)yOpen + rightThickness;
                bitmap.FillRectangle((int)yOpenEnd, leftOffset, (int)xStart-leftThickness, (int)rightOffset, color);
                leftOffset = (int)yClose - leftThickness;
                rightOffset = (int)yClose + rightThickness;
                bitmap.FillRectangle((int)xStart + leftThickness, leftOffset, (int)yCloseEnd, (int)rightOffset, color);
#else
                leftOffset=(int)xStart - leftThickness;
                rightOffset= (int)xStart + rightThickness;
                bitmap.FillRectangle(fastBuffer, width, height, leftOffset, (int)yStart, rightOffset, (int)yEnd, color);
                leftOffset = (int)yOpen - leftThickness;
                rightOffset = (int)yOpen + rightThickness;
                bitmap.FillRectangle(fastBuffer, width, height, (int)yOpenEnd,leftOffset, (int)xStart-leftThickness, (int)rightOffset, color);
                leftOffset = (int)yClose - leftThickness;
                rightOffset = (int)yClose + rightThickness;
                bitmap.FillRectangle(fastBuffer, width, height, (int)xStart+ leftThickness, leftOffset, (int)yCloseEnd, (int)rightOffset, color);
#endif


            }

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
            bitmap = fastHiLoOpenCloseSeries.Area.GetFastRenderSurface();
#if !WINDOWS_PHONE
            fastBuffer = fastHiLoOpenCloseSeries.Area.GetFastBuffer();
#endif
        }
        #endregion
    }
}
