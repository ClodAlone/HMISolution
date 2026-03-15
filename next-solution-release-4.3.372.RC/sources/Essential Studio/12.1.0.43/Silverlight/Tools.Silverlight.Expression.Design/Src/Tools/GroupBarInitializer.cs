#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Syncfusion.Tools.Silverlight.Expression.Design.Infrastructure;
using Syncfusion.Windows.Tools.Controls;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Tools.Silverlight.Expression.Design
{
    internal class GroupBarInitializer : DefaultInitializer
    {
        public GroupBarInitializer()
            : base() 
        {
        }

        public override void InitializeDefaults(ModelItem item, EditingContext context) 
        {
            Utils.SparseSetValue(item.Properties["Width"], 250d);
            Utils.SparseSetValue(item.Properties["Height"], 250d);
           
            GroupBarItem groupBarItem = new GroupBarItem();
            item.Properties["Items"].Collection.Add(groupBarItem);
            GroupBarItem groupBarItem2 = new GroupBarItem();
            item.Properties["Items"].Collection.Add(groupBarItem2);
            GroupBarItem groupBarItem3 = new GroupBarItem();
            item.Properties["Items"].Collection.Add(groupBarItem3);
            GroupBarItem groupBarItem4 = new GroupBarItem();
            item.Properties["Items"].Collection.Add(groupBarItem4);
            GroupBarItem groupBarItem5 = new GroupBarItem();
            item.Properties["Items"].Collection.Add(groupBarItem5);
            
            item.Properties["VisualMode"].SetValue(VisualMode.MultipleExpansion);

            item.Properties["Items"].Collection[0].Properties["HeaderText"].SetValue("New GroupBarItem");
            item.Properties["Items"].Collection[1].Properties["HeaderText"].SetValue("New GroupBarItem");
            item.Properties["Items"].Collection[2].Properties["HeaderText"].SetValue("New GroupBarItem");
            item.Properties["Items"].Collection[3].Properties["HeaderText"].SetValue("New GroupBarItem");
            item.Properties["Items"].Collection[4].Properties["HeaderText"].SetValue("New GroupBarItem");
            

        }
    }
}
