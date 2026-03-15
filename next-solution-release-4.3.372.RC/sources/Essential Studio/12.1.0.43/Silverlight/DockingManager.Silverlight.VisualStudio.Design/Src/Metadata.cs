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
using Syncfusion.Windows.Tools.Controls;


#if SyncfusionFramework3_5 && !SyncfusionSLFramework3_0
namespace Syncfusion.DockingManager.Silverlight.VisualStudio.Design
{ 

    /// <summary>
    /// Represents the Metadata class.
    /// </summary>
    public class Metadata : IRegisterMetadata
    {
        /// <summary>
        /// Attaches design-time metadata to a particular control type.
        /// </summary>
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCallback(typeof(CustomTabControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CustomTabItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CustomTabPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DockingGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DockPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SideButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(SidePanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Window), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
	        builder.AddCallback(typeof(DockManager), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(WindowContainer), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(CustomGridSplitter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
    builder.AddCallback(typeof(ItemsCollection), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
           
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }    
    }
}

#else
using Microsoft.Windows.Design.Features;

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
[assembly: ProvideMetadata(typeof(Syncfusion.DockingManager.Silverlight.VisualStudio.Design.Metadata))]
namespace Syncfusion.DockingManager.Silverlight.VisualStudio.Design
{
    // Container for any general design-time metadata to initialize.
    // Designers look for a type in the design-time assembly that 
    // implements IProvideAttributeTable. If found, designers instantiate 
    // this class and access its AttributeTable property automatically.

    internal class Metadata : IProvideAttributeTable
    {
        // Accessed by the designer to register any design-time metadata.
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                builder.AddCallback(typeof(CustomTabControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(CustomTabItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(CustomTabPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DockingGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DockPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SideButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(SidePanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Window), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DockManager), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(WindowContainer), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(CustomGridSplitter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ItemsCollection), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                return builder.CreateTable();
            }
        }
    }

}
#endif

