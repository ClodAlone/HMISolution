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
    /// Interaction logic for ButtonAdvSmartTag.xaml
    /// </summary>
    public partial class ButtonAdvSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the ButtonAdvSmartTag class.
        /// </summary>
        public ButtonAdvSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This method is called when the ButtonAdvSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(this.txtName, "Name");
            BindTextBox(this.txtLabel, "Label");
            BindSelectorWithEnum(this.SizeModeCombo, "SizeMode", typeof(SizeMode));
            BindCheckBox(this.IsMultiLine, "IsMultiLine");
            BindCheckBox(this.IsCheckable, "IsCheckable");
        }
    }
}
