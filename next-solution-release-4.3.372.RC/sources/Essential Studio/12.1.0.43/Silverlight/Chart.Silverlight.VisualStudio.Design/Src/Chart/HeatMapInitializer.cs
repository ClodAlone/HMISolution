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
using Syncfusion.Chart.Silverlight.VisualStudio.Design.Infrastructure;
using Syncfusion.Windows.Chart;
//using System.Windows.Media;


namespace Syncfusion.Chart.Silverlight.VisualStudio.Design
{
    internal class HeatMapInitializer : DefaultInitializer
    {
        public HeatMapInitializer()
            : base() 
        {
        }

        public override void InitializeDefaults(ModelItem item, EditingContext context) 
        {
            Util.SparseSetValue(item.Properties["Width"], 400d);
            Util.SparseSetValue(item.Properties["Height"], 300d);
            Util.SparseSetValue(item.Properties["BorderBrush"], "Black");

            Util.SparseSetValue(item.Properties["WeightValuePath"], "Weight");
            Util.SparseSetValue(item.Properties["ColorWeightValuePath"], "ColorWeight");

            HeatMapItem item1 = new HeatMapItem() { Header = "Item1", Weight=10, ColorWeight=20 };

            HeatMapItem item2 = new HeatMapItem() { Header = "Item2", Weight = 20, ColorWeight = 25 };
            HeatMapItem item3 = new HeatMapItem() { Header = "Item3", Weight = 30, ColorWeight = 30 };
            HeatMapItem item4 = new HeatMapItem() { Header = "Item4", Weight = 40, ColorWeight = 35 };
            HeatMapItem item5 = new HeatMapItem() { Header = "Item5", Weight = 50, ColorWeight = 30 };

            item.Properties["Items"].Collection.Add(item1);
            item.Properties["Items"].Collection.Add(item2);
            item.Properties["Items"].Collection.Add(item3);
            item.Properties["Items"].Collection.Add(item4);
            item.Properties["Items"].Collection.Add(item5);

            item.Properties["Items"].Collection[0].Properties["ColorWeightsInfo"].ClearValue();
            item.Properties["Items"].Collection[1].Properties["ColorWeightsInfo"].ClearValue();
            item.Properties["Items"].Collection[2].Properties["ColorWeightsInfo"].ClearValue();
            item.Properties["Items"].Collection[3].Properties["ColorWeightsInfo"].ClearValue();
            item.Properties["Items"].Collection[4].Properties["ColorWeightsInfo"].ClearValue();

            item.Properties["Items"].Collection[0].Properties["ItemMeasure"].ClearValue();
            item.Properties["Items"].Collection[1].Properties["ItemMeasure"].ClearValue();
            item.Properties["Items"].Collection[2].Properties["ItemMeasure"].ClearValue();
            item.Properties["Items"].Collection[3].Properties["ItemMeasure"].ClearValue();
            item.Properties["Items"].Collection[4].Properties["ItemMeasure"].ClearValue();

            item.Properties["Items"].Collection[0].Properties["Background"].ClearValue();
            item.Properties["Items"].Collection[1].Properties["Background"].ClearValue();
            item.Properties["Items"].Collection[2].Properties["Background"].ClearValue();
            item.Properties["Items"].Collection[3].Properties["Background"].ClearValue();
            item.Properties["Items"].Collection[4].Properties["Background"].ClearValue();

            item.Properties["Items"].Collection[0].Properties["Margin"].ClearValue();
            item.Properties["Items"].Collection[1].Properties["Margin"].ClearValue();
            item.Properties["Items"].Collection[2].Properties["Margin"].ClearValue();
            item.Properties["Items"].Collection[3].Properties["Margin"].ClearValue();
            item.Properties["Items"].Collection[4].Properties["Margin"].ClearValue();
        }
    }
}
