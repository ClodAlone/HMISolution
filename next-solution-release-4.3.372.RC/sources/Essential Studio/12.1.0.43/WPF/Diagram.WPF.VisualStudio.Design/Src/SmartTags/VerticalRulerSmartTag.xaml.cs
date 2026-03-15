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
using Microsoft.Windows.Design.Model;
using System.Diagnostics;
using Syncfusion.Windows.Design;

namespace Syncfusion.Diagram.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for EditSmartTag.xaml
    /// </summary>
    public partial class VerticalRulerSmartTag : SmartTagBase
    {
        public VerticalRulerSmartTag()
        {
            InitializeComponent();
        }
                
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            BindTextBox(txtName, "Name");
            BindSelectorWithBrushes(cmbBackground, "Background");
            BindSelectorWithBrushes(cmbMajorLinesStroke, "MajorLinesStroke");
            BindNumeric(txtMajorLinesThickness, "MajorLinesThickness");
            BindSelectorWithBrushes(cmbMarkerBrush, "MarkerBrush");
            BindNumeric(txtMarkerThickness, "MarkerThickness");
            BindSelectorWithBrushes(cmbMinorLinesStroke, "MinorLinesStroke");
            BindNumeric(txtMinorLinesThickness, "MinorLinesThickness");
            BindSelectorWithBrushes(cmbLabelFontColor, "LabelFontColor");
        }

        /*
        /// <summary>
        /// Binding the integer numeric value from SmartTag.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="propertyName">Name of the property.</param>
        protected void BindIntNumeric(TextBox control, string propertyName)
        {
            control.Tag = propertyName;
            Binding binding = new Binding();
            binding.Path = new PropertyPath(propertyName);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this.ModelItem.View;
            control.SetBinding(TextBox.TextProperty, binding);

            control.KeyDown += new System.Windows.Input.KeyEventHandler(IntNumericTextBox_KeyDown);
            control.LostFocus += new RoutedEventHandler(IntNumericTextBox_LostFocus);
        }*/


    }
}
