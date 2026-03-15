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
using System.Collections;

namespace Syncfusion.Chart.Wpf.VisualStudio.Design
{
    internal class ChartInitializer : DefaultInitializer
    {

        public override void InitializeDefaults(ModelItem item, Microsoft.Windows.Design.EditingContext context)
        {

            base.InitializeDefaults(item, context);
            using (ModelEditingScope scope = item.BeginEdit())
            {
#if SyncfusionFramework3_5
                ChartArea area = new ChartArea();
                area.Series.Add(new ChartSeries());
                area.Series.Add(new ChartSeries());

                area.Series[1].ClearValue(ChartSeries.XAxisProperty);
                area.Series[1].ClearValue(ChartSeries.YAxisProperty);

                area.Series[0].ClearValue(ChartSeries.XAxisProperty);
                area.Series[0].ClearValue(ChartSeries.YAxisProperty);

                ChartArea.SetShowGridLines(area.PrimaryAxis, true);
                ChartArea.SetShowGridLines(area.SecondaryAxis, true);

                area.Axes.Clear();

                item.Properties["Areas"].Collection.Clear();
                item.Properties["Areas"].Collection.Add(area);
                item.Properties["Areas"].Collection[0].Properties["ColorModel"].SetValue(new ChartStyleModel(10));

                ChartListData datasource1 = new ChartListData();
                datasource1.AddPoint(1, 1);
                datasource1.AddPoint(2, 2);
                datasource1.AddPoint(3, 3);
                datasource1.AddPoint(4, 4);
                datasource1.AddPoint(5, 5);
                datasource1.AddPoint(6, 6);
                ChartListData datasource2 = new ChartListData();
                datasource2.AddPoint(1, 2);
                datasource2.AddPoint(2, 3);
                datasource2.AddPoint(3, 4);
                datasource2.AddPoint(4, 5);
                datasource2.AddPoint(5, 3);
                datasource2.AddPoint(6, 2);
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection.RemoveAt(2);
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection.RemoveAt(2);
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["Data"].SetValue((object)datasource1);
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[1].Properties["Data"].SetValue((object)datasource2);
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["DataSource"].ClearValue();
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[1].Properties["DataSource"].ClearValue();


#endif
#if SyncfusionFramework4_0

                ChartArea area = new ChartArea();

                area.Series.Add(new ChartSeries());
                area.Series.Add(new ChartSeries());

                area.Series[1].ClearValue(ChartSeries.XAxisProperty);
                area.Series[1].ClearValue(ChartSeries.YAxisProperty);

                area.Series[0].ClearValue(ChartSeries.XAxisProperty);
                area.Series[0].ClearValue(ChartSeries.YAxisProperty);

                area.Series[0].ClearValue(ChartSeries.ZAxisProperty);
                area.Series[1].ClearValue(ChartSeries.ZAxisProperty);

                area.Series[0].ClearValue(ChartSeries.BindingPathXProperty);
                area.Series[1].ClearValue(ChartSeries.BindingPathXProperty);

                area.Series[0].ClearValue(ChartSeries.BindingPathsYProperty);
                area.Series[1].ClearValue(ChartSeries.BindingPathsYProperty);

                area.Series[0].ClearValue(ChartSeries.ShowDataLabelsProperty);
                area.Series[1].ClearValue(ChartSeries.ShowDataLabelsProperty);

                area.Series[0].ClearValue(ChartSeries.LegendIconProperty);
                area.Series[1].ClearValue(ChartSeries.LegendIconProperty);

                ChartArea.SetShowGridLines(area.PrimaryAxis, true);
                ChartArea.SetShowGridLines(area.SecondaryAxis, true);

                area.Axes.Clear();

                item.Properties["Areas"].Collection.Clear();
                item.Properties["Areas"].Collection.Add(area);
                item.Properties["Areas"].Collection[0].Properties["ColorModel"].ClearValue();
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["Data"].SetValue("1,1,2,2,3,3,4,4,5,5,6,6");
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[1].Properties["Data"].SetValue("1,2,2,3,3,4,4,5,5,3,6,2");

                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["DataModel"].ClearValue();
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[1].Properties["DataModel"].ClearValue();

#endif
#if SyncfusionFramework4_5 || SyncfusionFramework4_5_1

                ChartArea area = new ChartArea();

                area.Series.Add(new ChartSeries());
                area.Series.Add(new ChartSeries());

                area.Series[1].ClearValue(ChartSeries.XAxisProperty);
                area.Series[1].ClearValue(ChartSeries.YAxisProperty);

                area.Series[0].ClearValue(ChartSeries.XAxisProperty);
                area.Series[0].ClearValue(ChartSeries.YAxisProperty);

                area.Series[0].ClearValue(ChartSeries.ZAxisProperty);
                area.Series[1].ClearValue(ChartSeries.ZAxisProperty);

                area.Series[0].ClearValue(ChartSeries.BindingPathXProperty);
                area.Series[1].ClearValue(ChartSeries.BindingPathXProperty);

                area.Series[0].ClearValue(ChartSeries.BindingPathsYProperty);
                area.Series[1].ClearValue(ChartSeries.BindingPathsYProperty);

                area.Series[0].ClearValue(ChartSeries.ShowDataLabelsProperty);
                area.Series[1].ClearValue(ChartSeries.ShowDataLabelsProperty);

                area.Series[0].ClearValue(ChartSeries.LegendIconProperty);
                area.Series[1].ClearValue(ChartSeries.LegendIconProperty);

                ChartArea.SetShowGridLines(area.PrimaryAxis, true);
                ChartArea.SetShowGridLines(area.SecondaryAxis, true);

                area.Axes.Clear();

                item.Properties["Areas"].Collection.Clear();
                item.Properties["Areas"].Collection.Add(area);
                item.Properties["Areas"].Collection[0].Properties["ColorModel"].ClearValue();
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["Data"].SetValue("1,1,2,2,3,3,4,4,5,5,6,6");
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[1].Properties["Data"].SetValue("1,2,2,3,3,4,4,5,5,3,6,2");

                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[0].Properties["DataModel"].ClearValue();
                item.Properties["Areas"].Collection[0].Properties["Series"].Collection[1].Properties["DataModel"].ClearValue();

#endif
                scope.Complete();


            }
        }

        /// <summary>
        /// Initializes default values for the specified item.
        /// </summary>
        /// <param name="item">The item to initialize. This should not be null.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="item"/> is null.
        /// </exception>
        public override void InitializeDefaults(ModelItem item)
        {
            try
            {
                using (ModelEditingScope scope = item.BeginEdit())
                {
                    scope.Complete();
                }
            }
            catch (Exception )
            {

            }
        }
    }


}
