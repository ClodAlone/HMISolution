// <copyright file="AppMenuSmartTag.xaml.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

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
using Syncfusion.Windows.Design;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for AppMenuSmartTag.xaml
    /// </summary>
    public partial class AppMenuSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppMenuSmartTag"/> class.
        /// </summary>
        public AppMenuSmartTag()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(NameTextBox, "Name");
            BindSelectorWithEnum(HorizontalAlignmentSelector, "HorizontalAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(VerticalAlignmentSelector, "VerticalAlignment", typeof(VerticalAlignment));
            BindCheckBox(isBelowAppButton, "IsBelowAppButton");
            BindCheckBox(isEnabled, "IsEnabled");
        }

        /// <summary>
        /// Adds the footer item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddFooterItem(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            AddChildControl(this.ModelItem.Properties["ApplicationItems"], hyperlink.Tag as Type);
        }
    }
}
