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
using Syncfusion.Windows.Tools.Controls;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for RibbonSmartTag.xaml
    /// </summary>
    public partial class RibbonButtonPanelSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the RibbonButtonPanelSmartTag class.
        /// </summary>
        public RibbonButtonPanelSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This method is called when the RibbonButtonPanelSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(NameTextBox, "Name"); 
            BindSelectorWithEnum(HorizontalAlignmentSelector, "HorizontalAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(VerticalAlignmentSelector, "VerticalAlignment", typeof(VerticalAlignment));
            BindSelectorWithEnum(HorizontalContentAlignmentSelector, "HorizontalContentAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(VerticalContentAlignmentSelector, "VerticalContentAlignment", typeof(VerticalAlignment));
            BindCheckBox(isEnabled, "IsEnabled");
        }
    }
}
