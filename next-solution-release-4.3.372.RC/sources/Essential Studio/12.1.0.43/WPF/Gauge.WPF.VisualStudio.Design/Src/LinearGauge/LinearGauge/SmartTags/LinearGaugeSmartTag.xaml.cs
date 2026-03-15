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
	public partial class LinearGaugeSmartTag : SmartTagBase
	{
        GetChildCount child = new GetChildCount();		
        public LinearGaugeSmartTag()
		{
			InitializeComponent();
		}

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            base.OnApplyTemplate();
            BindTextBox(txtName, "Name");
            BindSelectorWithBrushes(comboFirstFrameFillColor, "FirstFrameFillColor");
            BindThickness(txtFirstFrameThickness, "FirstFrameThickness");
            BindSelectorWithBrushes(comboSecondFrameFillColor, "SecondFrameFillColor");
            BindThickness(txtSecondFrameThickness, "SecondFrameThickness");
            BindSelectorWithBrushes(comboCenterFrameColorSelector, "CenterFrameFillColor");
            BindSelectorWithEnum(comboFrameType, "FrameType", typeof(LinearGaugeFrameType));
        }

        private new void AddChildControl(object sender, RoutedEventArgs e)
        {
            LinearScale l_scale = new LinearScale();
            l_scale.Name = "linearScale" + child.GetSuffix("linearScale", "Scales", this.ModelItem);
           
            this.ModelItem.Properties["Scales"].Collection.Add(l_scale);
                        
        }

        private void AddStateIndicators(object sender, RoutedEventArgs e)
        {
            StateIndicator si = new StateIndicator();
            si.Name = "stateIndicator" + child.GetSuffix("stateIndicator", "StateIndicators", this.ModelItem);
            
            this.ModelItem.Properties["StateIndicators"].Collection.Add(si);
        }
	}
}
