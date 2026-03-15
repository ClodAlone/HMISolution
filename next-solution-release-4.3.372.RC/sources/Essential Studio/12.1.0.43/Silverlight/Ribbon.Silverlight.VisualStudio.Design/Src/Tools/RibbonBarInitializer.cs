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
using Syncfusion.Ribbon.Silverlight.VisualStudio.Design.Infrastructure;
using System.Windows.Media;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Ribbon.Silverlight.VisualStudio.Design.Tools
{
    internal class RibbonBarInitializer : DefaultInitializer
    {
        public RibbonBarInitializer()
            : base() 
        {
        }

        public override void InitializeDefaults(ModelItem item, EditingContext context) 
        {
           // ModelItem newautoComplete = ModelFactory.CreateItem(context, AutoCompleteTypes.AutoComplete.TypeId, CreateOptions.InitializeDefaults);
            Utils.SparseSetValue(item.Properties["Width"], 200d);
            Utils.SparseSetValue(item.Properties["Height"], 100d);

            RibbonButton box = new RibbonButton();
            box.Content = "Item 1";
            item.Properties["Items"].Collection.Add(box);
        }
    }
}
