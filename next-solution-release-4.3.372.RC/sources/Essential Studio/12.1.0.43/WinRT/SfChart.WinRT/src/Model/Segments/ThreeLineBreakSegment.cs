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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.WinRT.Chart
{
    public class ThreeLineBreakSegment:ChartSegment
    {
        private Canvas segmentCanvas;
        private Rectangle segmentRectangle;
        private ChartPoint chartPoint1;
        private ChartPoint chartPoint2;
        private ChartPoint[] chartPoint3;
        private ThreeLineBreakSeries threeLineBreakSeries;

        internal bool IsPriceDown { get; set; }
        internal bool IsPriceUp { get; set; }

        public ThreeLineBreakSegment()
        {

        }

        public ThreeLineBreakSegment(ChartPoint chartPoint1, ChartPoint chartPoint2, ChartPoint[] chartPoint3, ThreeLineBreakSeries threeLineBreakSeries)
        {
            this.chartPoint1 = chartPoint1;
            this.chartPoint2 = chartPoint2;
            this.chartPoint3 = chartPoint3;
            this.threeLineBreakSeries = threeLineBreakSeries;
            XRange = new DoubleRange(chartPoint1.X,chartPoint2.X);
            YRange = new DoubleRange(chartPoint1.Y, chartPoint2.Y);
        }
        internal override UIElement CreateVisual(Windows.Foundation.Size size)
        {
            segmentCanvas = new Canvas();
            segmentRectangle = new Rectangle();
            segmentCanvas.Children.Add(segmentRectangle);
            return segmentCanvas;
        }

        internal override UIElement GetRenderedVisual()
        {
            return segmentCanvas;
        }

        internal override void Update(IChartTransformer transformer)
        {
            Point blPoint = transformer.TransformToVisible(this.chartPoint1.X, this.chartPoint1.Y);
            Point trPoint = transformer.TransformToVisible(this.chartPoint2.X,this.chartPoint2.Y);
            Rect columnRect = new Rect(blPoint, trPoint);
            segmentRectangle.SetValue(Canvas.LeftProperty, columnRect.X);
            segmentRectangle.SetValue(Canvas.TopProperty, columnRect.Y);
            segmentRectangle.Width = columnRect.Width;
            segmentRectangle.Height = columnRect.Height;
            segmentRectangle.Fill = this.IsPriceUp ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        }

        internal override void OnSizeChanged(Windows.Foundation.Size size)
        {

        }
    }
}
