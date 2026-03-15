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
using Microsoft.Windows.Design.Model;
using System.Windows;
using Syncfusion.Windows.Chart;


namespace Syncfusion.Chart.Wpf.Expression.Design
{
  internal class ChartInitializer : DefaultInitializer
  {
    /// <summary>
    /// Initializes default values for the specified item.
    /// </summary>
    /// <param name="item">The item to initialize. This should not be null.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// 	<paramref name="item"/> is null.
    /// </exception>
    public override void InitializeDefaults(ModelItem item)
    {
      using (ModelEditingScope scope = item.BeginEdit())
      {
       
        ChartSeries series1 = new ChartSeries();
        //series1.Data = new ChartListData()
        //{ 
        //  new ChartPoint(1,1),
        //  new ChartPoint(2,2),
        //  new ChartPoint(3,3),
        //  new ChartPoint(4,4),
        //  new ChartPoint(5,5)
        //};
        ChartSeries series2 = new ChartSeries();
        //series2.Data = new ChartListData()d
        //{ 
        //  new ChartPoint(1,5),
        //  new ChartPoint(2,4),
        //  new ChartPoint(3,3),
        //  new ChartPoint(4,2),
        //  new ChartPoint(5,1)
        //};
        
        ChartArea area= new ChartArea();
        item.Properties["Areas"].Collection.Add(area);
        item.Properties["Areas"].Collection[0].Properties["Series"].Collection.Add(series1);
        item.Properties["Areas"].Collection[0].Properties["Series"].Collection.Add(series2);

        item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["Data"].SetValue("1,1,2,2,3,3,4,4,5,5,6,6");
        item.Properties["Areas"].Collection[0].Properties["Series"].Collection[1].Properties["Data"].SetValue("1,2,2,3,3,4,4,5,5,3,6,2");

        item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["BindingPathX"].ClearValue();
        item.Properties["Areas"].Collection[0].Properties["Series"].Collection[1].Properties["BindingPathX"].ClearValue();

        item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["BindingPathsY"].ClearValue();
        item.Properties["Areas"].Collection[0].Properties["Series"].Collection[1].Properties["BindingPathsY"].ClearValue();

        item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["DataModel"].ClearValue();
        item.Properties["Areas"].Collection[0].Properties["Series"].Collection[1].Properties["DataModel"].ClearValue();

        scope.Complete();
      }
    }
  }
}
