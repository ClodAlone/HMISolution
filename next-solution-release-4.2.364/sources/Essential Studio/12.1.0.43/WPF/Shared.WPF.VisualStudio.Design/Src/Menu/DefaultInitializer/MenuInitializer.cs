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
    internal class MenuInitializer : DefaultInitializer
    {
        public MenuInitializer()
        {

        }

        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                MenuItemAdv menuItem = new MenuItemAdv();

                item.Properties["Items"].Collection.Add(menuItem);
                item.Properties["Items"].Collection[0].Properties["Header"].SetValue("File");                

                item.Properties["Items"].Collection.Add(menuItem);
                item.Properties["Items"].Collection[1].Properties["Header"].SetValue("Edit");

                item.Properties["Items"].Collection.Add(menuItem);
                item.Properties["Items"].Collection[1].Properties["Header"].SetValue("View");                

                item.Properties["Width"].SetValue(150d);                
                item.Properties["Height"].SetValue(30d);
                scope.Complete();
            }
        }
    }
}
