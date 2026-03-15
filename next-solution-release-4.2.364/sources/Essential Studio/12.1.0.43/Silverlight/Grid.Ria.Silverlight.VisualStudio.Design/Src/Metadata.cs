#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Windows.Design.Metadata;

#if SyncfusionFramework4_0
using Microsoft.Windows.Design.Features;
[assembly: ProvideMetadata(typeof(Syncfusion.Grid.Ria.Silverlight.VisualStudio.Design.Metadata))]
#endif

namespace Syncfusion.Grid.Ria.Silverlight.VisualStudio.Design
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Windows.Design.Metadata;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using System.Collections.ObjectModel;
    using Microsoft.Windows.Design;
    using Syncfusion.Windows.Controls.Grid.Ria;

#if SyncfusionFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering                
                builder.AddCustomAttributes(typeof(DomainGridDataControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(DomainGridDataTableModel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DomainGridDataTableProperties), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
#else

    /// <summary>
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    /// </summary>
    internal class Metadata : IRegisterMetadata
    {

        // Called by Cider to register any design-time metadata
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            // tool box filtering                
            builder.AddCustomAttributes(typeof(DomainGridDataControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(DomainGridDataTableModel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DomainGridDataTableProperties), new ToolboxBrowsableAttribute(false));

            //// Category Editor
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}