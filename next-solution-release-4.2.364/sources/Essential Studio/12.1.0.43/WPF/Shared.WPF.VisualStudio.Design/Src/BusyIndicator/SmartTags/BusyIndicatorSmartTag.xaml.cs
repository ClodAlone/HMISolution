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
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools;
using System.Diagnostics;
using Syncfusion.Windows.Design;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for BusyIndicatorSmartTag.xaml
    /// </summary>
    public partial class BusyIndicatorSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the BusyIndicatorSmartTag class.
        /// </summary>
        public BusyIndicatorSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This method is called when the BusyIndicatorSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(this.txtName, "Name");
            BindSelectorWithEnum(this.CancelButtonVisible, "CancelButtonVisibility", typeof(Visibility));
            BindSelectorWithEnum(this.CloseButtonVisible, "CloseButtonVisibility", typeof(Visibility));
            BindCheckBox(this.EnableGrayScaleEffect, "EnableGrayScaleEffect");

            BindSelectorWithEnum(this.DescriptionPlacement, "DescriptionPlacement", typeof(DescriptionPlacement));
            BindSelectorWithEnum(this.HeaderAlignment, "HeaderAlignment", typeof(HorizontalAlignment));

            BindTextBox(this.Header, "Header");
            BindTextBox(this.LoadingDescription, "LoadingDescription");

            BindCheckBox(this.IsIndeterminate, "IsIndeterminate");
            BindTextBox(this.Delay, "Delay");
            BindTextBox(this.ProgressValue, "ProgressValue");
           
        }
    }
}
