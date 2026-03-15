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
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using Syncfusion.Windows.PropertyGrid;

[assembly: ProvideMetadata(typeof(Syncfusion.PropertyGrid.Silverlight.dll.Design.Metadata))]
namespace Syncfusion.PropertyGrid.Silverlight.dll.Design
{
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCallback(typeof(Syncfusion.Windows.PropertyGrid.PropertyGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                builder.AddCallback(typeof(Syncfusion.Windows.PropertyGrid.PropertyView), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Windows.PropertyGrid.PropertyViewItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Windows.PropertyGrid.PropertyCatagoryViewItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                return builder.CreateTable();
            }
        }
    }
}
