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

namespace Syncfusion.Chart.Wpf.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for ChartSmartTag.xaml
    /// </summary>
    public partial class ChartLegendSmartTag : SmartTagBase
    {
        public ChartLegendSmartTag()
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

            BindSelectorWithEnum(LegendVisibilitySelector, "Visibility", typeof(Visibility));
            
            BindSelectorWithEnum(LegendHorizontalAlignment, "HorizontalAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(LegendVerticalAlignment, "VerticalAlignment", typeof(VerticalAlignment));
      }
    }
}
