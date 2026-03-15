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
    /// Represents chart fast hilo bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastLineBitmapSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastHiLoSegment : ChartSegment
    {
        #region fields

        private Brush stroke;

        private WriteableBitmap bitmap;

        private byte[] fastBuffer;

        internal ChartSeries fastHiloSeries;

        private Size availableSize;

        private IList<double> xChartVals;

        private IList<double> yHiChartVals;

        private IList<double> yLoChartVals;

        int start, count;

        double xStart, xEnd, yStart, yEnd, xDelta, yDelta, xSize, ySize, xOffset, yOffset;

        bool x_isInversed, y_isInversed;

        double xTolerance, yTolerance;

        DoubleRange sbsInfo;

        double median, center, Left, Right;

        #endregion

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public FastHiLoSegment()
        {

        }

        /// <summary>
        /// Called when instance created for FastHiLoSegment
        /// </summary>
        /// <param name="series"></param>
        public FastHiLoSegment(ChartSeriesBase series)
        {
            stroke = series.Stroke;
            fastHiloSeries = series as ChartSeries;
        }

        /// <summary>
        /// Called when instance created for FastHiLoSegment
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="hiValues"></param>
        /// <param name="loValues"></param>
        /// <param name="series"></param>
        public FastHiLoSegment(IList<double> xValues, IList<double> hiValues, IList<double> loValues, AdornmentSeries series)
            : this(series)
        {
            base.Series = series;
            SetData(xValues, hiValues, loValues);
        }
        #endregion

        #region methods

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
            bitmap = fastHiloSeries.Area.GetFastRenderSurface();
            fastBuffer = fastHiloSeries.Area.GetFastBuffer();
            return null;
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="hiVals"></param>
        /// <param name="lowVals"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(IList<double> xVals, IList<double> hiVals, IList<double> lowVals)
        {
            this.xChartVals = xVals;
            this.yHiChartVals = hiVals;
            this.yLoChartVals = lowVals;
            List<double>yValues = new List<double>();
            yValues.AddRange(hiVals as List<double>);
            yValues.AddRange(lowVals as List<double>);
            if (fastHiloSeries.DataCount > 1)
            {
                if (fastHiloSeries.IsIndexed)
                {
                    double X_MAX = fastHiloSeries.DataCount - 1;
                    double Y_MAX = yValues.Max();
                    double X_MIN = 0;
                    double Y_MIN = yValues.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
                else
                {
                    double X_MAX = xChartVals.Max();
                    double Y_MAX = yValues.Max();
                    double X_MIN = xChartVals.Min();
                    double Y_MIN = yValues.Min();

                    XRange = new DoubleRange(X_MIN, X_MAX);
                    YRange = new DoubleRange(Y_MIN, Y_MAX);
                }
            }
            
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
            bitmap = fastHiloSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastHiloSeries.DataCount > 1)
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
                if (fastHiloSeries.IsActualTransposed)
                {
                    xSize = cartesianTransformer.XAxis.RenderedRect.Height;
                    ySize = cartesianTransformer.YAxis.RenderedRect.Width;
                    xOffset = cartesianTransformer.XAxis.RenderedRect.Top - fastHiloSeries.Area.SeriesClipRect.Top;
                    yOffset = cartesianTransformer.YAxis.RenderedRect.Left - fastHiloSeries.Area.SeriesClipRect.Left;
                }
                else
                {
                    xSize = cartesianTransformer.XAxis.RenderedRect.Width;
                    ySize = cartesianTransformer.YAxis.RenderedRect.Height;
                    xOffset = cartesianTransformer.XAxis.RenderedRect.Left - fastHiloSeries.Area.SeriesClipRect.Left;
                    yOffset = cartesianTransformer.YAxis.RenderedRect.Top - fastHiloSeries.Area.SeriesClipRect.Top;
                }
                xTolerance = Math.Abs((xDelta * 1) / xSize);
                yTolerance = Math.Abs((yDelta * 1) / ySize);
                count = (int)(Math.Ceiling(xEnd));
                start = (int)(Math.Floor(xStart));
                sbsInfo = fastHiloSeries.GetSideBySideInfo(fastHiloSeries);
                median = sbsInfo.Delta / 2;
                center = sbsInfo.Median;
                Left = sbsInfo.Start;
                Right = sbsInfo.End;

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
        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        void TransformToScreenCo(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            if (!fastHiloSeries.IsActualTransposed)
                TransformToScreenCoHorizontal();
            else
                TransformToScreenCoVertical();
        }

        private void TransformToScreenCoHorizontal()
        {
            float xValue = 0;
            float yHiValue = 0;
            float yLoValue = 0;
            if (fastHiloSeries.IsIndexed)
            {
                for (int j = 0; j < xChartVals.Count; j++)
                {
                    double yVal = yHiChartVals[j];
                    double yVal1 = yLoChartVals[j];
                    xValue = (float)(xOffset + xSize * ((j - xStart + center) / xDelta));
                    yHiValue = (float)(yOffset + ySize * (1 - ((yVal - yStart) / yDelta)));
                    yLoValue = (float)(yOffset + ySize * (1 - ((yVal1 - yStart) / yDelta)));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                }
            }
            else
            {
                for (int i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xChartVals[i];
                    double yHiVal = yHiChartVals[i];
                    double yLoVal = yLoChartVals[i];
                    xValue = (float)(xOffset + xSize * ((xVal - xStart + center) / xDelta));
                    yHiValue = (float)(yOffset + ySize * (1 - ((yHiVal - yStart) / yDelta)));
                    yLoValue = (float)(yOffset + ySize * (1 - ((yLoVal - yStart) / yDelta)));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                }
            }
        }

        private void TransformToScreenCoVertical()
        {
            float xValue = 0;
            float yHiValue = 0;
            float yLoValue = 0;
            if (fastHiloSeries.IsIndexed)
            {
                for (int j = 0; j < xChartVals.Count; j++)
                {
                    double yVal = yHiChartVals[j];
                    double yVal1 = yLoChartVals[j];
                    xValue = (float)(xOffset + xSize * ((xEnd + center - j) / xDelta));
                    yHiValue = (float)(yOffset + ySize * (1 - ((yEnd - yVal) / yDelta)));
                    yLoValue = (float)(yOffset + ySize * (1 - ((yEnd - yVal1) / yDelta)));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                }
            }
            else
            {
                for (int i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xChartVals[i];
                    double yHiVal = yHiChartVals[i];
                    double yLoVal = yLoChartVals[i];
                    xValue = (float)(xOffset + xSize * ((xEnd + center - xVal) / xDelta));
                    yHiValue = (float)(yOffset + ySize * (1 - ((yEnd - yHiVal) / yDelta)));
                    yLoValue = (float)(yOffset + ySize * (1 - ((yEnd - yLoVal) / yDelta)));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
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
            if (!fastHiloSeries.IsActualTransposed)
                TransformToScreenCoInLogHorizontal(xBase, yBase);
            else
                TransformToScreenCoInLogVertical(xBase, yBase);
           
        }

        private void TransformToScreenCoInLogVertical(double xBase, double yBase)
        {
            float xValue = 0;
            float yHiValue = 0;
            float yLoValue = 0;
            if (fastHiloSeries.IsIndexed)
            {
                for (int i = start; i < xChartVals.Count; i++)
                {
                    double yHiVal = yBase == 1 ? yHiChartVals[i] : Math.Log(yHiChartVals[i], yBase);
                    double yLoVal = yBase == 1 ? yLoChartVals[i] : Math.Log(yLoChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                    xValue = (float)(xOffset + xSize * ((xEnd + center - xVal) / xDelta));
                    yHiValue = (float)(yOffset + ySize * (1 - ((yEnd - yHiVal) / yDelta)));
                    yLoValue = (float)(yOffset + ySize * (1 - ((yEnd - yLoVal) / yDelta)));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                }
            }
            else
            {
                for (int i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    double yHiVal = yBase == 1 ? yHiChartVals[i] : Math.Log(yHiChartVals[i], yBase);
                    double yLoVal = yBase == 1 ? yLoChartVals[i] : Math.Log(yLoChartVals[i], yBase);
                    xValue = (float)(xOffset + xSize * ((xEnd + center - xVal) / xDelta));
                    yHiValue = (float)(yOffset + ySize * (1 - ((yEnd - yHiVal) / yDelta)));
                    yLoValue = (float)(yOffset + ySize * (1 - ((yEnd - yLoVal) / yDelta)));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                }
            }
        }

        private void TransformToScreenCoInLogHorizontal(double xBase, double yBase)
        {
            float xValue = 0;
            float yHiValue = 0;
            float yLoValue=0;
            if (fastHiloSeries.IsIndexed)
            {
                for (int i = start; i < xChartVals.Count; i++)
                {
                    double yHiVal = yBase == 1 ? yHiChartVals[i] : Math.Log(yHiChartVals[i], yBase);
                    double yLoVal = yBase == 1 ? yLoChartVals[i] : Math.Log(yLoChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);

                    xValue = (float)(xOffset + xSize * ((xVal - xStart + center) / xDelta));
                    yHiValue = (float)(yOffset + ySize * (1 - ((yHiVal - yStart) / yDelta)));
                    yLoValue = (float)(yOffset + ySize * (1 - ((yLoVal - yStart) / yDelta)));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);
                }
            }
            else
            {
                for (int i = 0; i < xChartVals.Count; i++)
                {
                    double xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    double yHiVal = yBase == 1 ? yHiChartVals[i] : Math.Log(yHiChartVals[i], yBase);
                    double yLoVal = yBase == 1 ? yLoChartVals[i] : Math.Log(yLoChartVals[i], yBase);
                    xValue = (float)(xOffset + xSize * ((xVal - xStart + center) / xDelta));
                    yHiValue = (float)(yOffset + ySize * (1 - ((yHiVal - yStart) / yDelta)));
                    yLoValue = (float)(yOffset + ySize * (1 - ((yLoVal - yStart) / yDelta)));
                    xValues.Add(xValue);
                    yHiValues.Add(yHiValue);
                    yLoValues.Add(yLoValue);


                }

                
            }

        }

        internal void UpdateVisual(bool updateHiLoLine)
        {
            bool isMultiColor = fastHiloSeries.Palette != ChartColorPalette.None;
            Color color = ((SolidColorBrush)this.Interior).Color;
            int dataCount = 0;
            if (yLoValues.Count < xValues.Count)
                dataCount = yLoValues.Count;
            else
                dataCount = xValues.Count;
            if (bitmap != null && xValues.Count > 1)
            {
#if !WINDOWS_PHONE
                fastBuffer = fastHiloSeries.Area.GetFastBuffer();
#endif

               

                int width = (int)fastHiloSeries.Area.SeriesClipRect.Width;
                int height = (int)fastHiloSeries.Area.SeriesClipRect.Height;

                int leftThickness = (int)fastHiloSeries.StrokeThickness / 2;
                int rightThickness = (int)(fastHiloSeries.StrokeThickness % 2 == 0
                    ? (fastHiloSeries.StrokeThickness / 2)  : fastHiloSeries.StrokeThickness / 2+1);
#if WPF
                bitmap.BeginWrite();
#endif
                if (fastHiloSeries is FastHiLoBitmapSeries)
                {
                    if (!fastHiloSeries.IsActualTransposed)
                        UpdateVisualHorizontal(width, height, color, leftThickness, rightThickness, isMultiColor,dataCount);
                    else
                        UpdateVisualVertical(width, height, color, leftThickness, rightThickness, isMultiColor, dataCount);
                }
#if WPF
                bitmap.EndWrite();
#endif

            }
            fastHiloSeries.Area.CanRenderToBuffer = true;
        }

        private void UpdateVisualVertical(int width, int height, Color color, int leftThickness, int rightThickness, bool isMultiColor, int dataCount)
        {
            float xStart = 0;
            float yStart = 0;
            float xEnd = 0;
            float yEnd = 0;
            for (int i = 0; i < dataCount; i++)
            {
                xStart = xValues[i];
                yStart = yHiValues[i];
                xEnd = xValues[i];
                yEnd = yLoValues[i];

                var leftOffset = (int)xStart - leftThickness;
                var rightOffset = (int)xEnd + rightThickness;
                if (isMultiColor)
                    color = (fastHiloSeries.ColorModel.GetBrush(i) as SolidColorBrush).Color;
#if WINDOWS_PHONE
                bitmap.FillRectangle((int)yEnd, (int)(leftOffset), (int)yStart, (int)(rightOffset), color);
#else
                bitmap.FillRectangle(fastBuffer, width, height, (int)yEnd, (int)leftOffset, (int)yStart, (int)rightOffset, color);
#endif
            }        
        }

        private void UpdateVisualHorizontal(int width, int height, Color color, int leftThickness, int rightThickness, bool isMultiColor, int dataCount)
        {
            float xStart = 0;
            float yStart = 0;
            float xEnd = 0;
            float yEnd = 0;
            for (int i = 0; i < dataCount; i++)
            {
                xStart = xValues[i];
                yStart = yHiValues[i];
                xEnd = xValues[i];
                yEnd = yLoValues[i];
                var leftOffset = xStart - leftThickness;
                var rightOffset = xStart + rightThickness;
                
                if (isMultiColor)
                    color = (fastHiloSeries.ColorModel.GetBrush(i) as SolidColorBrush).Color;
#if WINDOWS_PHONE
                bitmap.FillRectangle((int)leftOffset, (int)yStart, (int)rightOffset, (int)yEnd, color);
#else
                bitmap.FillRectangle(fastBuffer, width, height, (int)leftOffset, (int)yStart, (int)rightOffset, (int)yEnd, color);
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
            bitmap = fastHiloSeries.Area.GetFastRenderSurface();
#if !WINDOWS_PHONE
            fastBuffer = fastHiloSeries.Area.GetFastBuffer();
#endif
        }
        #endregion
    }
}
