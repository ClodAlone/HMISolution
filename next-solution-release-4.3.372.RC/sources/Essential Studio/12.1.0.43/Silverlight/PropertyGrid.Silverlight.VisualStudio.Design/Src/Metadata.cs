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
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Syncfusion.Windows.PropertyGrid;

[assembly: ProvideMetadata(typeof(Syncfusion.PropertyGrid.Silverlight.VisualStudio.Design.Metadata))]
namespace Syncfusion.PropertyGrid.Silverlight.VisualStudio.Design
{
   
    internal class Metadata : IProvideAttributeTable
    {
        AttributeTable IProvideAttributeTable.AttributeTable
        {
            get
            {
                PropertyGridAttributesTableBuilder builder = new PropertyGridAttributesTableBuilder();
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyGrid), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyCatagoryViewItem), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
}
