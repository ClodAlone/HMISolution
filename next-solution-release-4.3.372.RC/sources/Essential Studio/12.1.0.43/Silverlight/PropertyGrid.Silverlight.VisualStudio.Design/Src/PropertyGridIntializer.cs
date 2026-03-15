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
using Syncfusion.PropertyGrid.Silverlight.VisualStudio.Design.Infrastructure;

namespace Syncfusion.PropertyGrid.Silverlight.VisualStudio.Design
{
    internal class PropertyGridInitializer : DefaultInitializer
    {
        public PropertyGridInitializer()
            : base()
        {

        }
        public override void InitializeDefaults(ModelItem item, Microsoft.Windows.Design.EditingContext context)
        {
            base.InitializeDefaults(item);
            Utils.SparseSetValue(item.Properties["Width"], 100d);
            Utils.SparseSetValue(item.Properties["Height"], 150d);

            //ModelFactory.CreateItem(item.Context, typeof(Syncfusion.PropertyGird.Silverlight.PropertyGrid), new object[0]);
        }
    }
}
