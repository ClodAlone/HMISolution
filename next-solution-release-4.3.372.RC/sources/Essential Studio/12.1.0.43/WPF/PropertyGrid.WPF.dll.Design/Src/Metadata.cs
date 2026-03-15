#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Windows.Design.Metadata;
using Syncfusion.Windows.PropertyGrid;

#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.PropertyGrid.WPF.dll.Design.Metadata))]
#endif
namespace Syncfusion.PropertyGrid.WPF.dll.Design
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Windows.Design.Metadata;
    using System.ComponentModel;
    using Microsoft.Windows.Design.PropertyEditing;    
    using Microsoft.Windows.Design;
    using Syncfusion.Windows.PropertyGrid;

#if SyncfusionFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(PropertyGrid), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(PropertyView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PropertyViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PropertyCatagoryViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.CollectionEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.ItemsSourceControl), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }    
#elif SyncfusionFramework4_5
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(PropertyGrid), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(PropertyView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PropertyViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PropertyCatagoryViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.CollectionEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.ItemsSourceControl), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
#else
    internal class Metadata : IRegisterMetadata
    {
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(PropertyGrid), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(PropertyView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PropertyViewItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PropertyCatagoryViewItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.CollectionEditor), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.ItemsSourceControl), new ToolboxBrowsableAttribute(false));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
