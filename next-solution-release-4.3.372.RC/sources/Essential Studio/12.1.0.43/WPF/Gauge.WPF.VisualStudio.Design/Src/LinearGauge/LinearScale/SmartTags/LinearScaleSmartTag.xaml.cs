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
    public partial class LinearScaleSmartTag : SmartTagBase
    {
        GetChildCount child = new GetChildCount();
        public LinearScaleSmartTag()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindTextBox(txtName, "Name");
            BindNumeric(txtMaximum, "Maximum");
            BindNumeric(txtMinimum, "Minimum");
            BindNumeric(txtMinorIntervalValue, "MinorIntervalValue");
            BindNumeric(txtMajorIntervalValue, "MajorIntervalValue");
            BindNumeric(txtScaleBarLength, "ScaleBarLength");
            BindNumeric(txtScaleBarSize, "ScaleBarSize");
        }

        private void AddStateIndicators(object sender, RoutedEventArgs e)
        {
            StateIndicator si = new StateIndicator();
            si.Name = "stateIndicator" + child.GetSuffix("stateIndicator", "StateIndicators", this.ModelItem);            
            this.ModelItem.Properties["StateIndicators"].Collection.Add(si);
        }

        private void AddLinearBarPointer(object sender, RoutedEventArgs e)
        {
            LinearBarPointer barPointer = new LinearBarPointer();
            barPointer.Name = "linearPointer" + child.GetSuffix("linearPointer", "Pointers", this.ModelItem);
           
            this.ModelItem.Properties["Pointers"].Collection.Add(barPointer);
        }

        private void AddLinearMarkerPointer(object sender, RoutedEventArgs e)
        {
            LinearMarkerPointer markerPointer = new LinearMarkerPointer();
            markerPointer.Name = "linearPointer" + child.GetSuffix("linearPointer", "Pointers", this.ModelItem);
           
            this.ModelItem.Properties["Pointers"].Collection.Add(markerPointer);
        }

        private void AddLinearLabelTick(object sender, RoutedEventArgs e)
        {
            LinearLabelTick labelTick = new LinearLabelTick();
            labelTick.Name = "linearTick" + child.GetSuffix("linearTick", "Ticks", this.ModelItem);
           
            this.ModelItem.Properties["Ticks"].Collection.Add(labelTick);
        }

        private void AddLinearMarkTick(object sender, RoutedEventArgs e)
        {
            LinearMarkTick markTick = new LinearMarkTick();
            markTick.Name = "linearTick" + child.GetSuffix("linearTick", "Ticks", this.ModelItem);
           
            this.ModelItem.Properties["Ticks"].Collection.Add(markTick);
        }

        private void AddLinearRange(object sender, RoutedEventArgs e)
        {
            LinearRange range = new LinearRange();
            range.Name = "linearRange" + child.GetSuffix("linearRange", "Ticks", this.ModelItem);
           
           this.ModelItem.Properties["Ranges"].Collection.Add(range);
        }
    }
}
