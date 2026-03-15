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
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    internal class TreeViewAdvInitializer : DefaultInitializer
    {
        public TreeViewAdvInitializer()
        {
        }

        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                TreeViewItemAdv treeViewItem = new TreeViewItemAdv();

                item.Properties["Items"].Collection.Add(treeViewItem);

                item.Properties["Items"].Collection[0].Properties["Header"].SetValue("New TreeViewItem");

                //item.Properties["Items"].Collection[0].Properties["Name"].SetValue("TreeViewItem1");
 
                scope.Complete();
            }
        }
    }
}
