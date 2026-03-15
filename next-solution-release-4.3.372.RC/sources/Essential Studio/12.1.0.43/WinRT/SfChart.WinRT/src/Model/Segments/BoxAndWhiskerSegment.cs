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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.WinRT.Chart
{
    public class BoxAndWhiskerSegment : ChartSegment
    {

        #region fields

        private ChartPoint mB;
        private ChartPoint mE;
        private ChartPoint q1B;
        private ChartPoint q2E;
        private ChartPoint w1B;
        private ChartPoint w1M;
        private ChartPoint w1E;
        private ChartPoint w2B;
        private ChartPoint w2M;
        private ChartPoint w2E;
        private ChartPoint chartPoint;
        private BoxAndWhiskerSeries boxAndWhiskerSeries;
        private Line centerWhiskerLine;
        private Line topLine;
        private Line medianLine;
        private Line bottomLine;
        private Rectangle segmentRect;
        private Canvas _Canvas;

        #endregion


        #region properties


        #endregion

        #region constructor

        public BoxAndWhiskerSegment()
        {

        }

        public BoxAndWhiskerSegment(ChartPoint mB, ChartPoint mE, ChartPoint q1B, ChartPoint q2E, ChartPoint w1B, ChartPoint w1M, ChartPoint w1E, ChartPoint w2B, ChartPoint w2M, ChartPoint w2E, ChartPoint chartPoint, BoxAndWhiskerSeries boxAndWhiskerSeries)
        {
            this.mB = mB;
            this.mE = mE;
            this.q1B = q1B;
            this.q2E = q2E;
            this.w1B = w1B;
            this.w1M = w1M;
            this.w1E = w1E;
            this.w2B = w2B;
            this.w2M = w2M;
            this.w2E = w2E;
            this.chartPoint = chartPoint;
            this.boxAndWhiskerSeries = boxAndWhiskerSeries;

            XRange = DoubleRange.Union(new double[] { mB.X,mE.X,mB.X,mE.X });
            YRange = DoubleRange.Union(new double[] { w1B.Y,w2B.Y, w1B.Y,w2B.Y });
        }

        #endregion

        #region methods

        internal override Windows.UI.Xaml.UIElement CreateVisual(Windows.Foundation.Size size)
        {
            _Canvas = new Canvas();
            segmentRect = new Rectangle();
            centerWhiskerLine = new Line();
            centerWhiskerLine.Stroke = new SolidColorBrush(Colors.Green);
            centerWhiskerLine.StrokeThickness = 2;
            topLine = new Line();
            topLine.Stroke = new SolidColorBrush(Colors.Green);
            topLine.StrokeThickness = 2;
            medianLine = new Line();
            medianLine.Stroke = new SolidColorBrush(Colors.Green);
            medianLine.StrokeThickness = 2;
            bottomLine = new Line();
            bottomLine.Stroke = new SolidColorBrush(Colors.Green);
            bottomLine.StrokeThickness = 2;

            _Canvas.Children.Add(centerWhiskerLine);
            _Canvas.Children.Add(topLine);
            _Canvas.Children.Add(medianLine);
            _Canvas.Children.Add(bottomLine);
            _Canvas.Children.Add(segmentRect);

            return _Canvas;

        }

        internal override Windows.UI.Xaml.UIElement GetRenderedVisual()
        {
            return _Canvas;
        }

        internal override void Update(IChartTransformer transformer)
        {
            Point boxPt1 = transformer.TransformToVisible(q1B.X, q1B.Y);
            Point boxPt2 = transformer.TransformToVisible(q2E.X, q2E.Y);

            Point topWhiskerPt1 = transformer.TransformToVisible(w1B.X, w1B.Y);
            Point topWhiskerPt2 = transformer.TransformToVisible(w1E.X, w1E.Y);

            Point bottomWhiskerPt1 = transformer.TransformToVisible(w2B.X, w2B.Y);
            Point bottomWhiskerPt2 = transformer.TransformToVisible(w2E.X, w2E.Y);

            Point centerLineWhiskerPt1 = transformer.TransformToVisible(w1M.X, w1M.Y);
            Point centerLineWhiskerPt2 = transformer.TransformToVisible(w2M.X, w2M.Y);

            Point medianWhiskerPt1 = transformer.TransformToVisible(mB.X, mB.Y);
            Point medianWhiskerPt2 = transformer.TransformToVisible(mE.X, mE.Y);

            Rect boxRect = new Rect(boxPt1, boxPt2);

            segmentRect.SetValue(Canvas.LeftProperty, boxRect.X);
            segmentRect.SetValue(Canvas.TopProperty, boxRect.Y);
            segmentRect.Width = boxRect.Width;
            segmentRect.Height = boxRect.Height;


            this.topLine.X1 = topWhiskerPt1.X;
            this.topLine.X2 = topWhiskerPt2.X;
            this.topLine.Y1 = topWhiskerPt1.Y;
            this.topLine.Y2 = topWhiskerPt2.Y;

            this.bottomLine.X1 = bottomWhiskerPt1.X;
            this.bottomLine.X2 = bottomWhiskerPt2.X;
            this.bottomLine.Y1 = bottomWhiskerPt1.Y;
            this.bottomLine.Y2 = bottomWhiskerPt2.Y;

            this.centerWhiskerLine.X1 = centerLineWhiskerPt1.X;
            this.centerWhiskerLine.X2 = centerLineWhiskerPt2.X;
            this.centerWhiskerLine.Y1 = centerLineWhiskerPt1.Y;
            this.centerWhiskerLine.Y2 = centerLineWhiskerPt2.Y;

            this.medianLine.X1 = medianWhiskerPt1.X;
            this.medianLine.X2 = medianWhiskerPt2.X;
            this.medianLine.Y1 = medianWhiskerPt1.Y;
            this.medianLine.Y2 = medianWhiskerPt2.Y;
        }

        internal override void OnSizeChanged(Windows.Foundation.Size size)
        {
            
        }
        #endregion

    }
}
