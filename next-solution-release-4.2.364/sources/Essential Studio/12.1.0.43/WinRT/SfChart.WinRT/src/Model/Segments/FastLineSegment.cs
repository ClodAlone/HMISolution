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
using Syncfusion.UI.Xaml.Charts;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
//using Syncfusion.ChartCPP.WinRT;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
using Windows.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart fast line segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastLineSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastLineSegment: ChartSegment
    {
        #region fields
        /// <summary>
        /// Get or Set xChartVals property
        /// </summary>
        protected IList<double> xChartVals { get; set; }
        /// <summary>
        /// Get or Set yChartVals property
        /// </summary>
        protected IList<double> yChartVals { get; set; }

        private Brush stroke;

        internal Polyline polyline;

        //private Stream stream;

        //private Direct2D direct2D;

        internal ChartSeries fastSeries;

        bool x_isInversed, y_isInversed;

        double xStart, xEnd, yStart, yEnd, xDelta, yDelta, xSize, ySize, xOffset, yOffset;

        double xTolerance, yTolerance;

        int count, start;

        private Size availableSize;

        //private Image image;
        /// <summary>
        /// get or Set renderingMode
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public RenderingMode RenderingMode
        {
            get;
            set;
        }
        /// <summary>
        /// Get or Set Points property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public PointCollection Points
        {
            get;
            set;
        }

        bool segmentUpdated;

        #endregion

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public FastLineSegment()
        {

        }

        /// <summary>
        /// Called when instance created for FastLineSegment
        /// </summary>
        /// <param name="series"></param>
        public FastLineSegment(ChartSeriesBase series)
        {
            stroke = series.Stroke;
            fastSeries = series as ChartSeries;
        }

        /// <summary>
        /// Called when instance created for FastLineSegment
        /// </summary>
        /// <param name="xVals"></param>
        /// <param name="yVals"></param>
        /// <param name="series"></param>
        public FastLineSegment(IList<double> xVals, IList<double> yVals, AdornmentSeries series)
            : this(series)
        {
            base.Series = series;
            this.xChartVals = xVals;
            this.yChartVals = yVals;


            this.SetRange();
           
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
             polyline = new Polyline();
             SetVisualBindings(polyline);
             return polyline;
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
            if (Series is FastLineSeries)
            {
                binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("StrokeDashArray");
                element.SetBinding(Shape.StrokeDashArrayProperty, binding);
                binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("StrokeDashOffset");
                element.SetBinding(Shape.StrokeDashOffsetProperty, binding);
                binding = new Binding();
                binding.Source = Series;
                binding.Path = new PropertyPath("StrokeDashCap");
                element.SetBinding(Shape.StrokeDashCapProperty, binding);
            }
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        [ClassReference(IsReviewed = false)]
        public override void OnSizeChanged(Size size)
        {
            availableSize = size;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return polyline;
        }

        protected List<double> xValues = new List<double>();
        protected List<double> yValues = new List<double>();

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            //if (transformer != null && chartPoints != null && chartPoints.Count > 1)
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
            if (fastSeries.IsActualTransposed)
            {
                TransformToScreenCoVertical();
            }
            else
            {
                TransformToScreenCoHorizontal();
            }
        }

        private void TransformToScreenCoHorizontal()
        {
            int i = 0;
            Points = new PointCollection();
            double prevXValue = 0;
            double prevYValue = 0;
            if (fastSeries.IsIndexed)
            {
                prevXValue = 1;
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
                        Points.Add(new Point((xOffset + xSize * ((i - xStart) / xDelta)), (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                        prevXValue = i;
                        prevYValue = yVal;
                    }
                }

                if (start > 0 && start < yChartVals.Count)
                {
                    i = start - 1;
                    double yVal = yChartVals[i];
                    Points.Insert(0, new Point((xOffset + xSize * ((i - xStart) / xDelta)), (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                }

                if (count < yChartVals.Count - 1)
                {
                    i = count + 1;
                    double yVal = yChartVals[i];
                    Points.Add(new Point((xOffset + xSize * ((i - xStart) / xDelta)), (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
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
                            Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
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
                        Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                        break;
                    }
                }
                Points.Insert(0, new Point((xOffset + xSize * ((xChartVals[startIndex] - xStart) / xDelta)), (yOffset + ySize * (1 - ((yChartVals[startIndex] - yStart) / yDelta)))));
                if(i==cnt)
                    Points.Add(new Point((xOffset + xSize * ((xChartVals[cnt] - xStart) / xDelta)), (yOffset + ySize * (1 - ((yChartVals[cnt] - yStart) / yDelta)))));
            }
        }

        private void TransformToScreenCoVertical()
        {
            Points = new PointCollection();
            double prevXValue = 0;
            double prevYValue = 0;
            int i = 0;
            if (fastSeries.IsIndexed)
            {
                prevXValue = 1;
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
                        Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - i) / xDelta))));
                        prevXValue = i;
                        prevYValue = yVal;
                    }
                }

                if (start > 0 && start < yChartVals.Count)
                {
                    i = start - 1;
                    double yVal = yChartVals[i];
                    Points.Insert(0, new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - i) / xDelta))));
                }

                if (count < yChartVals.Count - 1)
                {
                    i = count + 1;
                    double yVal = yChartVals[i];
                    Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - i) / xDelta))));
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
                            Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
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
                        Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                        break;
                    }
                }
                Points.Insert(0, new Point((yOffset + ySize * (1 - ((yEnd - yChartVals[startIndex]) / yDelta))), (xOffset + xSize * ((xEnd - xChartVals[startIndex]) / xDelta))));
               if(i==cnt)
                   Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yChartVals[cnt]) / yDelta))), (xOffset + xSize * ((xEnd - xChartVals[cnt]) / xDelta))));
            }
        }

        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        void TransformToScreenCoInLog(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;
            if (!fastSeries.IsActualTransposed)
            {
                TransformToScreenCoInLogHorizontal(xBase, yBase);
            }
            else
            {
                TransformToScreenCoInLogVertical(xBase, yBase);
            }
        }

        

        private void TransformToScreenCoInLogHorizontal(double xBase, double yBase)
        {
            Points = new PointCollection();
            double prevXValue = 0;
            double prevYValue = 0;
            int startIndex = 0;
            int i = 0;
            if (fastSeries.IsIndexed)
            {
                prevXValue = 1;
                for (i = start; i <= count; i++)
                {
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);

                    if (Math.Abs(prevXValue - i) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                    {
                        Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), 
                            (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                        prevXValue = i;
                        prevYValue = yVal;
                    }
                }

                if (start > 0)
                {
                    i = start - 1;
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                    Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), 
                        (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                }

                if (count < yChartVals.Count - 1)
                {
                    i = count + 1;
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                    Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), 
                        (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                }
            }
            else
            {
                double xVal, yVal;
                prevXValue = xChartVals[0];
                prevYValue = yChartVals[0];
                int cnt = xChartVals.Count - 1;
                for (i = 1; i < cnt; i++)
                {
                    xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    if ((xVal <= count) && (xVal >= start))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), 
                                (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
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
                        Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), 
                            (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                        break;
                    }
                }

                xVal = xBase == 1 ? xChartVals[startIndex] : Math.Log(xChartVals[startIndex], xBase);
                yVal = yBase == 1 ? yChartVals[startIndex] : Math.Log(yChartVals[startIndex], yBase);
                Points.Insert(0, new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                
                if (i == cnt)
                {
                    xVal = xBase == 1 ? xChartVals[cnt] : Math.Log(xChartVals[cnt], xBase);
                    yVal = yBase == 1 ? yChartVals[cnt] : Math.Log(yChartVals[cnt], yBase);
                    Points.Add(new Point((xOffset + xSize * ((xVal - xStart) / xDelta)), (yOffset + ySize * (1 - ((yVal - yStart) / yDelta)))));
                }
            }
        }

        private void TransformToScreenCoInLogVertical(double xBase, double yBase)
        {
            Points = new PointCollection();
            double prevXValue = 0;
            double prevYValue = 0;
            int startIndex = 0;
            int i = 0;
            if (fastSeries.IsIndexed)
            {
                prevXValue = 1;
                for (i = start; i <= count; i++)
                {
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);

                    if (Math.Abs(prevXValue - i) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                    {
                        Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), 
                            (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                        prevXValue = i;
                        prevYValue = yVal;
                    }
                }

                if (start > 0)
                {
                    i = start - 1;
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                    Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), 
                        (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                }

                if (count < yChartVals.Count - 1)
                {
                    i = count + 1;
                    double yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    double xVal = xBase == 1 ? i : Math.Log(i, xBase);
                    Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), 
                        (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                }
            }
            else
            {
                double xVal, yVal;
                prevXValue = xChartVals[0];
                prevYValue = yChartVals[0];
                int cnt = xChartVals.Count - 1;
                for (i = 1; i < cnt; i++)
                {
                    xVal = xBase == 1 ? xChartVals[i] : Math.Log(xChartVals[i], xBase);
                    yVal = yBase == 1 ? yChartVals[i] : Math.Log(yChartVals[i], yBase);
                    if ((xVal <= count) && (xVal >= start))
                    {
                        if (Math.Abs(prevXValue - xVal) >= xTolerance || Math.Abs(prevYValue - yVal) >= yTolerance)
                        {
                            Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
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
                        Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                        break;
                    }
                }
                
                xVal = xBase == 1 ? xChartVals[startIndex] : Math.Log(xChartVals[startIndex], xBase);
                yVal = yBase == 1 ? yChartVals[startIndex] : Math.Log(yChartVals[startIndex], yBase);
                Points.Insert(0, new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                if (i == cnt)
                {
                    xVal = xBase == 1 ? xChartVals[cnt] : Math.Log(xChartVals[cnt], xBase);
                    yVal = yBase == 1 ? yChartVals[cnt] : Math.Log(yChartVals[cnt], yBase);
                    Points.Add(new Point((yOffset + ySize * (1 - ((yEnd - yVal) / yDelta))), (xOffset + xSize * ((xEnd - xVal) / xDelta))));
                }
            }
        }

        internal void SetRange()
        {
            double X_MAX = 0;
            double Y_MAX = 0;
            double X_MIN = 0;
            double Y_MIN = 0;

            if (fastSeries.DataCount > 0)
            {
                if (fastSeries.IsIndexed)
                {
                    X_MAX = fastSeries.DataCount - 1;
                    Y_MAX = yChartVals.Max();
                    X_MIN = 0;
                    Y_MIN = yChartVals.Min();
                }
                else
                {
                    X_MAX = xChartVals.Max();
                    Y_MAX = yChartVals.Max();
                    X_MIN = xChartVals.Min();
                    Y_MIN = yChartVals.Min();
                }
                XRange = new DoubleRange(X_MIN, X_MAX);
                YRange = new DoubleRange(Y_MIN, Y_MAX);
            }
            
           
          
        }

        internal void UpdateSegment(int index, NotifyCollectionChangedAction action, IChartTransformer transformer)
        {
            if (action == NotifyCollectionChangedAction.Remove)
            {
                this.Points.RemoveAt(index);
            }
            else if (action == NotifyCollectionChangedAction.Add)
            {
                Point point = transformer.TransformToVisible(xChartVals[index], yChartVals[index]);
                Points.Add(point);
                UpdateVisual(false);
            }
        }

        internal void UpdateVisual(bool updatePolyline)
        {
            if (updatePolyline)
            {
                if (polyline != null)
                {
                    if (segmentUpdated)
                        Series.SeriesRootPanel.Clip = null;
                    polyline.Points = Points;
                    segmentUpdated = true;
                }
            }
        }

        #endregion
    }
}
