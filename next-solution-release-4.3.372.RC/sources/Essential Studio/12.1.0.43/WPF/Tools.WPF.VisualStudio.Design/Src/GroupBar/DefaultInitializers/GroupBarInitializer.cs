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
    internal class GroupBarInitializer : DefaultInitializer
    {
        public GroupBarInitializer( )
        {
        }

        public override void InitializeDefaults( ModelItem item )
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                GroupViewItem groupViewItem = new GroupViewItem();
                groupViewItem.Text = "New GroupViewItem";

                GroupView groupView = new GroupView();
                groupView.IsListViewMode = true;
                groupView.Items.Add(groupViewItem);

                GroupBarItem groupBarItem = new GroupBarItem();

                item.Properties["Items"].Collection.Add(groupBarItem);
                item.Properties["VisualMode"].SetValue(VisualMode.MultipleExpansion);
                item.Properties["MinWidth"].SetValue(150d);
                item.Properties["MinHeight"].SetValue(100d);
                item.Properties["Items"].Collection[0].Properties["HeaderText"].SetValue("New GroupBarItem");
                item.Properties["Items"].Collection[0].Properties["ShowInGroupBar"].SetValue(true);
                item.Properties["Items"].Collection[0].Properties["Content"].SetValue(groupView);
				
				
                scope.Complete();
            }
        }
    }
}
