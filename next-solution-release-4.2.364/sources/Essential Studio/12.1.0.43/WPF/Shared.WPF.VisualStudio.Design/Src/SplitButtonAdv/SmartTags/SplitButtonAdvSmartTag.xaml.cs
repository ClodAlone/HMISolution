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
    /// Interaction logic for SplitButtonAdvSmartTag.xaml
    /// </summary>
    public partial class SplitButtonAdvSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the SplitButtonAdvSmartTag class.
        /// </summary>
        public SplitButtonAdvSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This method is called when the SplitButtonAdvSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(this.NameTextBox, "Name");
            BindTextBox(this.LabelTextBox, "Label");
            BindSelectorWithEnum(SizeMode, "SizeMode",typeof (SizeMode));
            BindCheckBox(IsMultiline, "IsMultiLine");
            BindSelectorWithEnum(DropDownDirection, "DropDirection", typeof(DropDirection));
        }

    }
}
