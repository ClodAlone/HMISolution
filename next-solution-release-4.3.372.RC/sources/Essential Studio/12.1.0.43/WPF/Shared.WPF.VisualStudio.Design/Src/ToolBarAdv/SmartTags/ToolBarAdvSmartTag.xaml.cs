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
using Syncfusion.Windows.Shared;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for ToolBarAdvSmartTag.xaml
    /// </summary>
    public partial class ToolBarAdvSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the ToolBarAdvSmartTag class.
        /// </summary>
        public ToolBarAdvSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This method is called when the ToolBarAdvSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            BindTextBox(this.NameTextBox, "Name");
            BindCheckBox(EnableAddRemoveButton, "EnableAddRemoveButton");
            BindCheckBox(IsOverflowOpen, "IsOverflowOpen");
        }
    }
}
