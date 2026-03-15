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
using Syncfusion.Windows.Edit;
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Edit.Wpf.Expression.Design
{
    internal class EditControlInitializer : DefaultInitializer
    {
        public EditControlInitializer()
        {
        }

        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                item.Properties["Name"].SetValue("editControl1");
                item.Properties["Background"].SetValue(Brushes.White);
                item.Properties["Foreground"].SetValue(Brushes.Black);
                item.Properties["BorderBrush"].SetValue(Brushes.Black);
                item.Properties["BorderThickness"].SetValue(new Thickness(0d));
                item.Properties["Margin"].SetValue(new Thickness(0d));
                scope.Complete();
            }
        }
    }
}
