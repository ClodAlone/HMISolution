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
using Syncfusion.Windows.Shared;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for TileViewSmartTag.xaml
    /// </summary>
    public partial class DropDownButtonAdvSmartTag:SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownButtonAdvSmartTag"/> class.
        /// </summary>
        public DropDownButtonAdvSmartTag()
        {
            InitializeComponent();           
        }

        /// <summary>
        /// When overridden in a derived clasns, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(this.NameTextBox, "Name");
            BindTextBox(this.LabelTextBox, "Label");
            BindSelectorWithEnum(SizeModeCombo, "SizeMode", typeof(SizeMode));
            BindCheckBox(IsMultiLine, "IsMultiLine");
            BindSelectorWithEnum(DropDirection, "DropDirection", typeof(DropDirection));
        }

       
    }
}

 