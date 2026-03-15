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
    internal class TabControlExtInitializer : DefaultInitializer
    {
        public TabControlExtInitializer()
        {
        }

        public override void InitializeDefaults( ModelItem item )
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                //Removing first item since TabItem added by default since TabContrlExt inherted from TabControl.
                item.Properties["Items"].Collection.Remove(item.Properties["Items"].Collection[0]);
                TabItemExt tabItem = new TabItemExt();
                tabItem.Header = "New TabItemExt";
				tabItem.MinHeight = 20;
                tabItem.MinWidth = 150;
                item.Properties["Items"].Collection.Add(tabItem);
				
                scope.Complete();
            }
        }
    }
}
