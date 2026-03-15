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
    /// Represents chart fast bar bitmap segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WinRT Chart building system.</remarks>
    /// <seealso cref="FastBarBitmapSeries"/>
    [ClassReference(IsReviewed = false)]
    public class FastBarBitmapSegment : ChartSegment
    {
        #region fields

        private WriteableBitmap bitmap;
#if NETFX_CORE
        private byte[] fastBuffer;
#endif
        internal ChartSeries fastBarSeries;

        private Size availableSize;

        private IList<double> x1ChartVals, y1ChartVals, x2ChartVals, y2ChartVals;

        int start, count;

        double xStart, xEnd, yStart, yEnd, xDelta, yDelta, width, height, left , top;

        bool x_isInversed, y_isInversed;

        #endregion

        #region ctor

        public FastBarBitmapSegment()
        {

        }

        public FastBarBitmapSegment(ChartSeriesBase series)
        {
            fastBarSeries = series as ChartSeries;
        }

        public FastBarBitmapSegment(IList<double> x1Values, IList<double> y1Values, IList<double> x2Values, IList<double> y2Values, ChartSeriesBase series)
            : this(series)
        {
            base.Series = series;
            SetData(x1Values, y1Values, x2Values, y2Values);
        }
        #endregion

        #region methods

        [ClassReference(IsReviewed = false)]
        protected override void SetVisualBindings(Shape element)
        {
        }

        [ClassReference(IsReviewed = false)]
        public override UIElement CreateVisual(Size size)
        {
            return null;
        }

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

        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        public override void Update(IChartTransformer transformer)
        {
            bitmap = fastBarSeries.Area.GetFastRenderSurface();
            if (transformer != null && fastBarSeries.DataCount != 0)
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

                width = cartesianTransformer.XAxis.RenderedRect.Height;
                height = cartesianTransformer.YAxis.RenderedRect.Width;

                left = cartesianTransformer.YAxis.RenderedRect.Left - fastBarSeries.Area.SeriesClipRect.Left;
                top = cartesianTransformer.XAxis.RenderedRect.Top - fastBarSeries.Area.SeriesClipRect.Top;

                availableSize = new Size(width, height);

                count = (int)(Math.Ceiling(xEnd));
                count = (int)Math.Min(count, x1ChartVals.Count);
                start = (int)(Math.Ceiling(xStart));

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
                UpdateVisual();
            }
        }

        List<float> x1Values = new List<float>();
        List<float> x2Values = new List<float>();
        List<float> y1Values = new List<float>();
        List<float> y2Values = new List<float>();
        /// <summary>
        /// Transforms for non logarithmic axis
        /// </summary>
        /// <param name="transformer"></param>
        void TransformToScreenCo(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            float x1Value = 0;
            float x2Value = 0;
            float y1Value = 0;
            float y2Value = 0;
            for (int i = 0; i < count; i++)
            {
                double x1Val = x_isInversed
                                   ? x2ChartVals[i] < xEnd ? xEnd : x2ChartVals[i]
                                   : x1ChartVals[i] < xStart ? xStart : x1ChartVals[i];
                double x2Val = x_isInversed
                                   ? x1ChartVals[i] > xStart ? xStart : x1ChartVals[i]
                                   : x2ChartVals[i] > xEnd ? xEnd : x2ChartVals[i];

                double y1Val = y_isInversed
                                   ? y2ChartVals[i] > yStart ? yStart : y2ChartVals[i] < yEnd ? yEnd : y2ChartVals[i]
                                   : y1ChartVals[i] > yEnd ? yEnd : y1ChartVals[i] < yStart ? yStart : y1ChartVals[i];
                double y2Val = y_isInversed
                                   ? y1ChartVals[i] < yEnd ? yEnd : y1ChartVals[i] > yStart ? yStart : y1ChartVals[i]
                                   : y2ChartVals[i] < yStart ? yStart : y2ChartVals[i] > yEnd ? yEnd : y2ChartVals[i];

                x1Value = (float)(top + (availableSize.Width) * (((x1Val) - xStart) / xDelta)) - 1;
                x2Value = (float)(top+ (availableSize.Width) * ((x2Val - xStart) / xDelta));
                y1Value = (float)(left + (availableSize.Height) * (1 - ((y1Val - yStart) / yDelta)));
                y2Value = (float)(left+ (availableSize.Height) * (1 - ((y2Val - yStart) / yDelta)));
                x1Values.Add(x1Value);
                x2Values.Add(x2Value);
                y1Values.Add(y1Value);
                y2Values.Add(y2Value);

            }
        }

        /// <summary>
        /// Transforms for logarithmic axis
        /// </summary>
        /// <param name="transformer"></param>
        void TransformToScreenCoInLog(ChartTransform.ChartCartesianTransformer cartesianTransformer)
        {
            int i = 0;
            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            double yBase = cartesianTransformer.YAxis.IsLogarithmic ? (cartesianTransformer.YAxis as LogarithmicAxis).LogarithmicBase : 1;
            int count = fastBarSeries.IsIndexed ? (int)(Math.Ceiling(xEnd)) : x1ChartVals.Count;
            int start = (int)(Math.Floor(xStart));
            float x1Value, x2Value, y1Value, y2Value;
            for (i = 0; i < count; i++)
            {
                double logx1 = xBase == 1 ? x1ChartVals[i] : Math.Log(x1ChartVals[i], xBase);
                double logx2 = xBase == 1 ? x2ChartVals[i] : Math.Log(x2ChartVals[i], xBase);
                double logy1 = yBase == 1 ? y1ChartVals[i] : Math.Log(y1ChartVals[i], yBase);
                double logy2 = yBase == 1 ? y2ChartVals[i] : Math.Log(y2ChartVals[i], yBase);
                
                double x1Val = x_isInversed ? logx2 < xEnd ? xEnd : logx2 : logx1 < xStart ? xStart : logx1;
                double x2Val = x_isInversed ? logx1 > xStart ? xStart : logx1 : logx2 > xEnd ? xEnd : logx2;

                double y1Val = y_isInversed
                                   ? logy2 > yStart ? yStart : logy2 < yEnd ? yEnd : logy2
                                   : logy1 > yEnd ? yEnd : logy1 < yStart ? yStart : logy1;

                double y2Val = y_isInversed
                                   ? logy1 < yEnd ? yEnd : logy1 > yStart ? yStart : logy1
                                   : logy2 < yStart ? yStart : logy2 > yEnd ? yEnd : logy2;
                
                x1Value = (float)(top + (availableSize.Width) * ((x1Val - xStart) / xDelta));
                x2Value = (float)(top + (availableSize.Width) * ((x2Val - xStart) / xDelta));
                y1Value = (float)(left + (availableSize.Height) * (1 - ((y1Val - yStart) / yDelta)));
                y2Value = (float)(left + (availableSize.Height) * (1 - ((y2Val - yStart) / yDelta)));
                x1Values.Add(x1Value);
                x2Values.Add(x2Value);
                y1Values.Add(y1Value);
                y2Values.Add(y2Value);
            }
        }

        internal void UpdateVisual()
        {
            double actualIndex = 0;
            float x1 = 0, x2, y1, y2, diff = 0;
            Color color = ((SolidColorBrush)this.Interior).Color;
            int dataCount = x1Values.Count;
            if (bitmap != null && x1Values.Count != 0)
            {
#if !WINDOWS_PHONE
                fastBuffer = fastBarSeries.Area.GetFastBuffer();
#endif
                double width = (int)fastBarSeries.Area.SeriesClipRect.Width;
                double height = (int)fastBarSeries.Area.SeriesClipRect.Height;
#if WPF
                bitmap.BeginWrite();
#endif
                    for (int i = 0; i < dataCount; i++)
                    {
                        if (fastBarSeries.Palette != ChartColorPalette.None)
                        {
                            Brush brush = fastBarSeries.ColorModel.GetBrush((fastBarSeries.IsIndexed ? (int)Math.Ceiling(actualIndex) : i));
                            color = ((SolidColorBrush)brush).Color;
                        }
                        x1 = x1Values[i];
                        x2 = x2Values[i];
                        y1 = y1ChartVals[i] > 0 ? y1Values[i] : y2Values[i];
                        y2 = y1ChartVals[i] > 0 ? y2Values[i] : y1Values[i];
                        diff = x2 - x1;
#if WINDOWS_PHONE
                        bitmap.FillRectangle((int)(width-y2), (int)(height - x1 - diff), (int)(width - y1), (int)(height - x1), color);
#else
                        bitmap.FillRectangle(fastBuffer, (int)width, (int)height, (int)(width-y2), (int)(height - x1 - diff), (int)(width - y1), (int)(height - x1), color);
#endif
                        actualIndex++;
                    }
                
#if WPF
                bitmap.EndWrite();
#endif

            }
            fastBarSeries.Area.CanRenderToBuffer = true;
        }

        public override void OnSizeChanged(Size size)
        {
        }
        #endregion
    }
}
