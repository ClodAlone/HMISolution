#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Chart;
using Syncfusion.Chart.Silverlight.Expression.Design.Infrastructure;

namespace Syncfusion.Chart.Silverlight.Expression.Design.Chart
{
    internal class ChartControlInitializer : DefaultInitializer
    {
        public ChartControlInitializer()
            : base() 
        {
        }

        public override void InitializeDefaults(ModelItem item, EditingContext context) 
        {
            Util.SparseSetValue(item.Properties["Width"], 400d);
            Util.SparseSetValue(item.Properties["Height"], 300d);

            ChartArea area = new ChartArea();
            item.Properties["Areas"].Collection.Add(area);

            ChartSeries series = new ChartSeries();
            item.Properties["Areas"].Collection[0].Properties["Series"].Collection.Add(series);

            ChartPoint point1 = new ChartPoint() { X = 1d, Y = 15d };
            ChartPoint point2 = new ChartPoint() { X = 2d, Y = 30d };
            ChartPoint point3 = new ChartPoint() { X = 3d, Y = 20d };
            ChartPoint point4 = new ChartPoint() { X = 4d, Y = 25d };
            item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["Interior"].ClearValue();
            item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["Data"].Collection.Add(point1);
            item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["Data"].Collection.Add(point2);
            item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["Data"].Collection.Add(point3);
            item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["Data"].Collection.Add(point4);

        }
    }
}
