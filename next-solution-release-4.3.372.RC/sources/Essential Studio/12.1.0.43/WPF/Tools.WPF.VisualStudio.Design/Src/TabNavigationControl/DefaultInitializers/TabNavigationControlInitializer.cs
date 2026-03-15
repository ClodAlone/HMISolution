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
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Tools.Controls;
using System.Windows;
namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    internal class TabNavigationControlInitializer:DefaultInitializer
    {

        public TabNavigationControlInitializer()
        { }

        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                item.Properties["Height"].SetValue(114d);
                item.Properties["Width"].SetValue(265d);
                item.Properties["Name"].SetValue("TabNavigation");
                item.Properties["TabStripVisibility"].SetValue(Visibility.Visible);
                scope.Complete();
            }
        }
    }
}
