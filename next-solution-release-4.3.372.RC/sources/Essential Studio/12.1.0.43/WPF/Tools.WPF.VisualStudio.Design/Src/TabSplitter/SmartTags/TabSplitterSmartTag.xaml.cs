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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;
using System.Diagnostics;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for TabSplitter.xaml
    /// </summary>
    public partial class TabSplitterSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the TabSplitterSmartTag class.
        /// </summary>
        public TabSplitterSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This method is called when the TabSplitterSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindTextBox(NameTextBox, "Name");
            BindSelectorWithBrushes(Backgroundselector, "MouseOverBackground");
            BindSelectorWithBrushes(Foregroundselector, "MouseOverForeground");
            BindSelectorWithBrushes(SelectedBackgroundelector, "SelectedBackground");
            BindSelectorWithBrushes(Selectedforeground, "SelectedForeground");

        }
    }
}
