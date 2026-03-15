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
	public partial class RollingGaugeSmartTag : SmartTagBase
	{
        GetChildCount child = new GetChildCount();
		public RollingGaugeSmartTag()
		{
			InitializeComponent();
		}

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindIntNumeric(txtsegmentCount, "SegmentCount");
            BindTextBox(txtValue, "Value");
            BindTextBox(txtUnit, "Unit");
            BindTextBox(txtName, "Name");
            BindSelectorWithBrushes(comboSegForeground, "SegmentForeground");
            BindSelectorWithBrushes(comboSegBackground, "SegmentBackground");
            BindSelectorWithEnum(comboUnitPosition, "UnitPosition", typeof(UnitPosition));
            BindSelectorWithEnum(comboDirection, "Direction", typeof(Direction));
            BindCheckBox(IsAutomaticSegmentCountEnabled, "IsAutomaticSegmentCountEnabled");
            BindCheckBox(IsNumeric, "IsNumeric");
            BindNumeric(MinValue, "MinValue");
            BindNumeric(MaxValue, "MaxValue");
        }
	}
}
