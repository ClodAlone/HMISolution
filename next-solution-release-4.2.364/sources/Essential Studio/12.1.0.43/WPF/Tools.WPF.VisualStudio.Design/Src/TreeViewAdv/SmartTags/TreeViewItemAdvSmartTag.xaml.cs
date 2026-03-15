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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;
using System.Diagnostics;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for TreeViewItemAdvSmartTag.xaml
    /// </summary>
    public partial class TreeViewItemAdvSmartTag : SmartTagBase
    {
        static int count;
        /// <summary>
        /// Initializes a new instance of the TreeViewItemAdvSmartTag class.
        /// </summary>
        public TreeViewItemAdvSmartTag()
        {
            InitializeComponent();
        }
        /// <summary>
        /// This method is called when the TreeViewItemAdvSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(NameTextBox, "Name");
            BindTextBox(HeaderTextBox, "Header");
            BindCheckBox(IsExpanded, "IsExpanded");
            count = 0;
        }

        private void AddChildControls(object sender, RoutedEventArgs e)
        {
            count++;
            ModelItem item = ModelFactory.CreateItem(this.Context, typeof(TreeViewItemAdv), new object[0]);
            
            int cnt = this.ModelItem.Properties["Items"].Collection.Count;
        
            this.ModelItem.Properties["Items"].Collection.Add(item);

            this.ModelItem.Properties["Items"].Collection[cnt].Properties["Header"].SetValue("New" + this.ModelItem.Name + count.ToString());

            //this.ModelItem.Properties["Items"].Collection[cnt].Properties["Name"].SetValue( this.ModelItem.Name + count.ToString()); 
        }

       
    }
}
