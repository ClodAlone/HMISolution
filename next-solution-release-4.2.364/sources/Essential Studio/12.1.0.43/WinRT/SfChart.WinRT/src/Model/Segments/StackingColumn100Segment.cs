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
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.Chart
{
    public class StackingColumn100Segment:ColumnSegment
    {
        #region fields

        private ChartPoint chartPoint;

        private StackingColumn100Series stackingColumn100Series;

        private ChartPoint ChartPoint1;

        private ChartPoint ChartPoint2;

        private double YRangeDelta;

        private Brush Stroke;

        #endregion

        #region properties


        #endregion
        
        #region constructor

        public StackingColumn100Segment()
        {

        }

        public StackingColumn100Segment(ChartPoint chartPoint1, ChartPoint chartPoint2, double yRangeDelta,  ChartPoint chartPoint, 
            StackingColumn100Series Series):base(chartPoint1,chartPoint2)
        {
            this.ChartPoint1 = chartPoint1;
            this.ChartPoint2 = chartPoint2;
            this.YRangeDelta = yRangeDelta;
            this.chartPoint = chartPoint;
            this.stackingColumn100Series = Series;
            this.Stroke = Series.Stroke;
        }


        #endregion

        #region methods

        internal override void Update(IChartTransformer transformer)
        {
            base.columnSegment.Fill = this.Stroke;
            base.Update(transformer);
        }
      
        #endregion
    }
}
