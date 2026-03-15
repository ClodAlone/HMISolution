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
    /// Represents chart fast hilo bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="FastLineBitmapSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastColumnBitmapSegment : ChartSegment
    {
        #region fields

        private Brush stroke;

        private WriteableBitmap bitmap;

        private byte[] fastBuffer;

        internal ChartSeries fastColumnSeries;

        private Size availableSize;

        private IList<double> x1ChartVals;

        private IList<double> y1ChartVals;

        private IList<double> x2ChartVals;

        private IList<double> y2ChartVals;

        int start, count;

        double xStart, xEnd, yStart, yEnd, xDelta, yDelta, width, height, left, top;

        bool x_isInversed, y_isInversed;

        double xTolerance, yTolerance;

        #endregion

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public FastColumnBitmapSegment()
        {

        }

        /// <summary>
        /// Called when instance created for FastColumnSegment
        /// </summary>
        /// <param name="series"></param>
        public FastColumnBitmapSegment(ChartSeriesBase series)
        {
            stroke = series.Stroke;
            fastColumnSeries = series as ChartSeries;
        }

        /// <summary>
        /// Called when instance created for FastColumnSegment with following arguments
        /// </summary>
        /// <param name="x1Values"></param>
        /// <param name="y1Values"></param>
        /// <param name="x2Values"></param>
        /// <param name="y2Values"></param>
        /// <param name="series"></param>
        public FastColumnBitmapSegment(IList<double> x1Values, IList<double> y1Values, IList<double> x2Values, IList<double> y2Values, ChartSeriesBase series)
            : this(series)
        {
            base.Series = series;
            SetData(x1Values, y1Values, x2Values, y2Values);
        }
        #endregion

        #region methods

        /// <summary>
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
        [ClassReference(IsReviewed = false)]
        protected override void SetVisualBindings(Shape element)
        {
            base.SetVisualBindings(element);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
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
            bitmap = fastColumnSeries.Area.GetFastRenderSurface();
            fastBuffer = fastColumnSeries.Area.GetFastBuffer();
            return null;
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="x1Values"></param>
        /// <param name="y1Values"></param>
        /// <param name="x2Values"></param>
        /// /// <param name="y2Values"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(IList<double> x1Values, IList<double> y1Values, IList<double> x2Values, IList<double> y2Values)
        {
            this.x1ChartVals = x1Values;
            this.y1ChartVals = y1Values;
            this.x2ChartVals = x2Values;
            this.y2ChartVals = y2Values;
            double X_MAX = x2Values.Max();
            double Y_MAX = y1Values.Max();
            double X_MIN = x1Values.Min();
            double Y_MIN = y1ChartVals.Min() < 0 ? y1ChartVals.Min() : 0;

            XRange = new DoubleRange(X_MIN, X_MAX);
            YRange = new DoubleRange(Y_MIN, Y_MAX);

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
            bitmap = fastColumnSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastColumnSeries.DataCount !=0)
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


                width = cartesianTransformer.XAxis.RenderedRect.Width;
                height = cartesianTransformer.YAxis.RenderedRect.Height;
                availableSize = new Size(width, height);

                xTolerance = Math.Abs((xDelta * 1) / availableSize.Width);
                yTolerance = Math.Abs((yDelta * 1) / availableSize.Height);
                left = cartesianTransformer.XAxis.RenderedRect.Left - fastColumnSeries.Area.SeriesClipRect.Left;
                top = cartesianTransformer.YAxis.RenderedRect.Top - fastColumnSeries.Area.SeriesClipRect.Top;
                count = (int)(Math.Ceiling(xEnd));
                count = (int)Math.Min(count, x1ChartVals.Count);
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

                x1Values.Clear();
                x2Values.Clear();
                y1Values.Clear();
                y2Values.Clear();
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

        List<float> x1Values = new List<float>();
        List<float> x2Values = new List<float>();
        List<float> y1Values = new List<float>();
        List<float> y2Values = new List<float>();
        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="cartesianTransformer"></param>
        void TransformToScreenCo(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            float x1Value = 0, x2Value = 0,y1Value = 0,y2Value = 0;
        
            for (int i = 0; i < count; i++)
            {
                double x1Val = x_isInversed
                                   ? x2ChartVals[i] < xEnd ? xEnd : (x2ChartVals[i] > xStart ? xStart : x2ChartVals[i])
                                   : x1ChartVals[i] < xStart ? xStart : (x1ChartVals[i] > xEnd ? xEnd : x1ChartVals[i]);
                double x2Val = x_isInversed
                                   ? x1ChartVals[i] > xStart ? xStart : (x1ChartVals[i] < xEnd ? xEnd : x1ChartVals[i])
                                   : x2ChartVals[i] > xEnd ? xEnd : (x2ChartVals[i] < xStart ? xStart : x2ChartVals[i]);
                double y2Val = y_isInversed
                                   ? y1ChartVals[i] < yEnd ? yEnd : y1ChartVals[i] > yStart ? yStart : y1ChartVals[i]
                                   : y2ChartVals[i] < yStart ? yStart : y2ChartVals[i] > yEnd ? yEnd : y2ChartVals[i];
                double y1Val = y_isInversed
                                   ? y2ChartVals[i] > yStart ? yStart : y2ChartVals[i] < yEnd ? yEnd : y2ChartVals[i]
                                   : y1ChartVals[i] > yEnd ? yEnd : y1ChartVals[i] < yStart ? yStart : y1ChartVals[i];
                x1Value = (float)(left + (availableSize.Width) * ((x1Val - xStart) / xDelta));
                x2Value = (float)(left + (availableSize.Width) * ((x2Val - xStart) / xDelta));
                y1Value = (float)(top + (availableSize.Height) * (1 - ((y1Val - yStart) / yDelta)));
                y2Value = (float)(top + (availableSize.Height) * (1 - ((y2Val - yStart) / yDelta)));
                x1Values.Add(x1Value);
                x2Values.Add(x2Value);
                y1Values.Add(y1Value);
                y2Values.Add(y2Value);

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

            int count = fastColumnSeries.IsIndexed ? (int)(Math.Ceiling(xEnd)) : x1ChartVals.Count;
            int start = (int)(Math.Floor(xStart));
            float x1Value = 0;
            float x2Value = 0;
            float y1Value = 0;
            float y2Value = 0;
            for (int i = 0; i < count; i++)
            {
                double logx1 = xBase == 1 ? x1ChartVals[i] : Math.Log(x1ChartVals[i], xBase);
                double logx2 = xBase == 1 ? x2ChartVals[i] : Math.Log(x2ChartVals[i], xBase);
                double logy2 = yBase == 1 ? y2ChartVals[i] : Math.Log(y2ChartVals[i], yBase);
                double logy1 = yBase == 1 ? y1ChartVals[i] : Math.Log(y1ChartVals[i], yBase);
              
                double x1Val = x_isInversed ? logx2 < xEnd ? xEnd : logx2 : logx1 < xStart ? xStart : logx1;
                double x2Val = x_isInversed ? logx1 > xStart ? xStart : logx1 : logx2 > xEnd ? xEnd : logx2;
              
                double y1Val = y_isInversed
                                   ? logy2 > yStart ? yStart : logy2 < yEnd ? yEnd : logy2
                                   : logy1 > yEnd ? yEnd : logy1 < yStart ? yStart : logy1;
                double y2Val = y_isInversed
                                   ? logy1 < yEnd ? yEnd : logy1 > yStart ? yStart : logy1
                                   : logy2 < yStart ? yStart : logy2 > yEnd ? yEnd : logy2;
               
                x1Value = (float)(left + (availableSize.Width) * ((x1Val - xStart) / xDelta));
                x2Value = (float)(left + (availableSize.Width) * ((x2Val - xStart) / xDelta));
                y1Value = (float)(top + (availableSize.Height) * (1 - ((y1Val - yStart) / yDelta)));
                y2Value = (float)(top + (availableSize.Height) * (1 - ((y2Val - yStart) / yDelta)));
                x1Values.Add(x1Value);
                x2Values.Add(x2Value);
                y1Values.Add(y1Value);
                y2Values.Add(y2Value);

            }

            

        }

        internal void UpdateVisual(bool updateHiLoLine)
        {
            float x1 = 0;
            float x2 = 0;
            float y1 = 0;
            float y2 = 0;
            bool isMultiColor = fastColumnSeries.Palette != ChartColorPalette.None;
            Color color = ((SolidColorBrush)this.Interior).Color;
            int dataCount = x1Values.Count;
            if (bitmap != null && x1Values.Count !=0)
            {
#if !WINDOWS_PHONE
                fastBuffer = fastColumnSeries.Area.GetFastBuffer();
#endif



                int width = (int)fastColumnSeries.Area.SeriesClipRect.Width;
                int height = (int)fastColumnSeries.Area.SeriesClipRect.Height;

                int leftThickness = (int)fastColumnSeries.StrokeThickness / 2;
                int rightThickness = (int)(fastColumnSeries.StrokeThickness % 2 == 0
                    ? (fastColumnSeries.StrokeThickness / 2) - 1 : fastColumnSeries.StrokeThickness / 2);
#if WPF
                bitmap.BeginWrite();
#endif
                if (fastColumnSeries is FastColumnBitmapSeries || fastColumnSeries is FastStackingColumnBitmapSeries || fastColumnSeries is FastBarBitmapSeries || fastColumnSeries is MACDTechnicalIndicator)
                {
                    for (int i = 0; i < dataCount; i++)
                    {
                        if (isMultiColor)
                            color=((SolidColorBrush)fastColumnSeries.ColorModel.GetBrush(i)).Color;
                        x1 = x1Values[i];
                        x2 = x2Values[i];
                        y1 = y1ChartVals[i] > 0 ? y1Values[i] : y2Values[i];
                        y2 = y1ChartVals[i] > 0 ? y2Values[i] : y1Values[i];
                        if (y1 == 0 && y2 == 0)
                            continue;
#if WINDOWS_PHONE
                        bitmap.FillRectangle((int)(x1), (int)y1, (int)x2, (int)y2, color);
#else
                        bitmap.FillRectangle(fastBuffer, width, height, (int)(x1), (int)y1, (int)x2, (int)y2, color);

#endif


                    }
                }
#if WPF
                bitmap.EndWrite();
#endif

            }
            fastColumnSeries.Area.CanRenderToBuffer = true;
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
            bitmap = fastColumnSeries.Area.GetFastRenderSurface();
#if !WINDOWS_PHONE
            fastBuffer = fastColumnSeries.Area.GetFastBuffer();
#endif
        }
        #endregion
    }
}
