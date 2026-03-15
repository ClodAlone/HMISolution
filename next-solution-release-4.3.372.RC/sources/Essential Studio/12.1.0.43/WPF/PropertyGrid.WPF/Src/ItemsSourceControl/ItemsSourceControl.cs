#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;

namespace Syncfusion.Windows.PropertyGrid
{
    /// <summary>
    /// Represent the ItemsSourceControl
    /// </summary>
    public class ItemsSourceControl: ItemsControl   
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsSourceControl"/> class.
        /// </summary>
        public ItemsSourceControl()
        {
            DefaultStyleKey = typeof(ItemsSourceControl);
        }

        /// <summary>
        /// Initializes the <see cref="ItemsSourceControl"/> class.
        /// </summary>
        static ItemsSourceControl()
        {

        }

        internal Button Part_CollectionBtn;
        internal TextBlock Part_TextBlock;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.Part_CollectionBtn = this.GetTemplateChild("Part_Btn") as Button;
            this.Part_TextBlock = this.GetTemplateChild("Part_Text") as TextBlock;
            this.Part_CollectionBtn.Click += new RoutedEventHandler(Part_CollectionBtn_Click);
        }

        /// <summary>
        /// Handles the Click event of the Part_CollectionBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void Part_CollectionBtn_Click(object sender, RoutedEventArgs e)
        {
            CollectionEditor collEditor = new CollectionEditor(this);
            collEditor.Show();
        }
    }
}