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
using System.Windows.Media;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    internal class FontListComboBoxInitializer : DefaultInitializer
    {
        public FontListComboBoxInitializer()
        {
        }

        public override void InitializeDefaults( ModelItem item )
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {             
                item.Properties["Width"].SetValue(180d);
                item.Properties["PopupDropDownHeight"].SetValue(150d);
                item.Properties["Height"].SetValue(30d);
                item.Properties["SelectedFontFamily"].SetValue(new FontFamily("Agency FB"));
                scope.Complete();
            }
        }
    }
}
