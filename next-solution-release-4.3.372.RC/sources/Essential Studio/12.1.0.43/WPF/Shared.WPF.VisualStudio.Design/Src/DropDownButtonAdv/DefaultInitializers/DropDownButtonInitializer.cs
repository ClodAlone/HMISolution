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
using Syncfusion.Windows.Tools.Controls;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Shared;
using System.Windows;
namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    class DropDownButtonInitializer:DefaultInitializer
    {
        public DropDownButtonInitializer()
        {
          
        }
        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {

                 DropDownMenuItem item1 = new DropDownMenuItem() { Header = "Item1" };
                 DropDownMenuItem item2 = new DropDownMenuItem() { Header = "Item2" };
                 DropDownMenuGroup dg = new DropDownMenuGroup();
                 DropDownButtonAdv dbutton = new DropDownButtonAdv();
                 dg.Items.Add(item1);
                 dg.Items.Add(item2);
                 item.Properties["Content"].SetValue(dg);
                 item.Properties["Width"].SetValue(150d);
                 item.Properties["Height"].SetValue(30d);
                 scope.Complete();
            }
        }
    }
}
