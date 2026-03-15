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
using Syncfusion.Windows.Shared;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    internal class PinnableListBoxInitializer : DefaultInitializer
    {
        public PinnableListBoxInitializer()
        {

        }

        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {

                PinnableListBox pin = new PinnableListBox();
                PinnableListBoxItem item1 = new PinnableListBoxItem();
                item1.Content = "PinnableItem1";
                PinnableListBoxItem item2 = new PinnableListBoxItem();
                item2.Content = "PinnableItem2";
                PinnableListBoxItem item3 = new PinnableListBoxItem();
                item3.Content = "PinnableItem3";
                item.Properties["Items"].Collection.Add(item1);
                item.Properties["Items"].Collection.Add(item2);
                item.Properties["Items"].Collection.Add(item3);
                item.Properties["Width"].SetValue(250d);
                item.Properties["Height"].SetValue(100d);
                scope.Complete();
            }
        }
    }
}
