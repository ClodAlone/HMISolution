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
using Syncfusion.Tools.Silverlight.Expression.Design.Infrastructure;
using Syncfusion.Windows.Tools.Controls;
using Microsoft.Windows.Design;

namespace Syncfusion.Tools.Silverlight.Expression.Design
{
    internal class TabControlAdvInitializer : DefaultInitializer
    {
        public TabControlAdvInitializer()
            : base() 
        {
        }

        public override void InitializeDefaults(ModelItem item, EditingContext context) 
        {
            Utils.SparseSetValue(item.Properties["Width"], 250d);
            Utils.SparseSetValue(item.Properties["Height"], 250d);

            TabItemAdv tabItemAdv = new TabItemAdv();
            item.Properties["Items"].Collection.Add(tabItemAdv);
            TabItemAdv tabItemAdv2 = new TabItemAdv();
            item.Properties["Items"].Collection.Add(tabItemAdv2);
            TabItemAdv tabItemAdv3 = new TabItemAdv();
            item.Properties["Items"].Collection.Add(tabItemAdv3);
            TabItemAdv tabItemAdv4 = new TabItemAdv();
            item.Properties["Items"].Collection.Add(tabItemAdv4);
            TabItemAdv tabItemAdv5 = new TabItemAdv();
            item.Properties["Items"].Collection.Add(tabItemAdv5);

            item.Properties["Items"].Collection[0].Properties["Header"].SetValue("New TabItem1");
            item.Properties["Items"].Collection[1].Properties["Header"].SetValue("New TabItem2");
            item.Properties["Items"].Collection[2].Properties["Header"].SetValue("New TabItem3");
            item.Properties["Items"].Collection[3].Properties["Header"].SetValue("New TabItem4");
            item.Properties["Items"].Collection[4].Properties["Header"].SetValue("New TabItem5");
        }
    }
}
