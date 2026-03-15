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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Windows.Design;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Chart;

namespace Syncfusion.Chart.Wpf.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for ChartSmartTag.xaml
    /// </summary>
    public partial class ChartSmartTag : SmartTagBase
    {
        static int i = 1;
        public ChartSmartTag()
        {
            InitializeComponent();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(this.NameTextBox, "Name");

            BindSelectorWithBrushes(BorderSelector, "BorderBrush");
            BindThickness(BorderThicknessTxtBox, "BorderThickness");
            BindCornerRadius(CornerRadiusTxtBox, "CornerRadius");
        }

        private void AddArea(object sender, RoutedEventArgs e)
        {
            ModelItem item = ModelFactory.CreateItem(this.Context, typeof(ChartArea), new object[0]);
            ////item.Properties["Name"].SetValue(GetNextChildName());
            this.ModelItem.Properties["Areas"].Collection.Add(item);
        }

        private string GetNextChildName()
        {
            string suffix = "1";

            if (this.ModelItem.Properties["Areas"].Collection.Count == 0)
                return ("area" + suffix);

            ModelItem item = this.ModelItem.Properties["Areas"].Collection[this.ModelItem.Properties["Areas"].Collection.Count - 1];

            string name = item.Name;
            char[] arr = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string[] strName = name.Split(arr, StringSplitOptions.RemoveEmptyEntries);
            string numName = name.Substring(name.LastIndexOf(strName[strName.Length - 1]) + strName[strName.Length - 1].Length);
            int num = 0;
            if (int.TryParse(numName, out num))
            {
                suffix = ((int.Parse(numName)) + 1).ToString();
            }

            string actualstring = string.Concat(strName);
            return (actualstring + suffix);
        }

        private void AddLegend(object sender, RoutedEventArgs e)
        {
            ModelItem item = ModelFactory.CreateItem(this.Context, typeof(ChartLegend), new object[0]);
            item.Properties["Name"].SetValue("legend" + i.ToString());
            this.ModelItem.Properties["Legends"].Collection.Add(item);
            i++;
        }
    }
}
