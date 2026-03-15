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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Design;
using Syncfusion.Windows.Gauge;

namespace Syncfusion.Gauge.WPF.VisualStudio.Design
{
	/// <summary>
	/// Interaction logic for GroupBarSmartTag.xaml
	/// </summary>
    public partial class CircularScaleSmartTag : SmartTagBase
    {
        GetChildCount child = new GetChildCount();
        public CircularScaleSmartTag()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindTextBox(txtName, "Name");
            BindNumeric(txtGapSweepAngle, "GapSweepAngle");
            BindNumeric(txtMinorIntervalValue, "MinorIntervalValue");
            BindNumeric(txtMajorIntervalValue, "MajorIntervalValue");
            BindNumeric(txtRadius, "Radius");
        }

        private void AddCircularLabelTick(object sender, RoutedEventArgs e)
        {
            CircularLabelTick labelTick = new CircularLabelTick();
            labelTick.Name = "circularTick" + child.GetSuffix("circularTick", "Ticks", this.ModelItem);
           
            this.ModelItem.Properties["Ticks"].Collection.Add(labelTick);
        }

        private void AddCircularMarkTick(object sender, RoutedEventArgs e)
        {
            CircularMarkTick markTick = new CircularMarkTick();
            markTick.Name = "circularTick" + child.GetSuffix("circularTick", "Ticks", this.ModelItem);
           
            this.ModelItem.Properties["Ticks"].Collection.Add(markTick);
        }

        private void AddCircularPointer(object sender, RoutedEventArgs e)
        {
            CircularPointer cirPointer = new CircularPointer();
            cirPointer.Name = "circularPointer" + child.GetSuffix("circularPointer", "Pointers", this.ModelItem);
           
            this.ModelItem.Properties["Pointers"].Collection.Add(cirPointer);
        }

        private void AddCircularRange(object sender, RoutedEventArgs e)
        {
            CircularRange cirRange = new CircularRange();
            cirRange.Name = "circularRange" + child.GetSuffix("circularRange", "Ranges", this.ModelItem);
           
            this.ModelItem.Properties["Ranges"].Collection.Add(cirRange);
        }
    }
}
