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
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.WinRT.Chart
{
    public class KagiSegment:ChartSegment
    {
        private ChartPoint[] chartPoint1;
        private ChartPoint[] chartPoint2;
        private KagiSeries kagiSeries;
        private Polyline polyLine;

        #region fields

        private PointCollection Points=new PointCollection();

        #endregion

        #region properties

        public bool IsPriceDown { get; set; }

        public bool IsPriceUp { get; set; }

        #endregion
        
        #region constructor
        
        public KagiSegment()
        {

        }

        public KagiSegment(ChartPoint[] chartPoint1, ChartPoint[] chartPoint2, KagiSeries kagiSeries)
        {
            this.chartPoint1 = chartPoint1;
            this.chartPoint2 = chartPoint2;
            this.kagiSeries = kagiSeries;

            for (int i = 0; i < chartPoint1.Length; i++)
            {
                XRange += chartPoint1[i].X;
                YRange += chartPoint1[i].Y;
            }
        }

        #endregion
      
        #region methods

        internal override Windows.UI.Xaml.UIElement CreateVisual(Windows.Foundation.Size size)
        {
            polyLine = new Polyline();
            polyLine.Stroke = new SolidColorBrush(Colors.Red);
            polyLine.StrokeThickness = 2;
            return polyLine;
        }

        internal override Windows.UI.Xaml.UIElement GetRenderedVisual()
        {
            return polyLine;
        }

        internal override void Update(IChartTransformer transformer)
        {
            bool shouldReassignPointCollection = false;
            PointCollection points = new PointCollection();

            for (int i = 0; i < chartPoint1.Length; i++)
            {
                Point point = transformer.TransformToVisible(chartPoint1[i].X, chartPoint1[i].Y);
                if (!shouldReassignPointCollection && !this.Points.Contains(point))
                {
                    shouldReassignPointCollection = true;
                }
                points.Add(point);
            }

            if (shouldReassignPointCollection)
            {
                this.Points = points;
            }
            polyLine.Stroke = this.IsPriceDown ? KagiSeries.GetPriceUpInterior(this.kagiSeries) : KagiSeries.GetPriceDownInterior(this.kagiSeries);
            this.polyLine.Points = this.Points;
        }

        internal override void OnSizeChanged(Windows.Foundation.Size size)
        {
            
        }

        #endregion

    }
}
