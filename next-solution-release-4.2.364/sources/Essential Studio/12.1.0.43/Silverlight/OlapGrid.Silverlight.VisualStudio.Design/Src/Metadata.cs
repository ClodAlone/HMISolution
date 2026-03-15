#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Windows.Design.Metadata;
    using System.ComponentModel;
    using Microsoft.Windows.Design.PropertyEditing;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using Microsoft.Windows.Design;
    using Syncfusion.Silverlight.Grid.Olap;

#if SyncfusionFramework4_0 || SyncfusionSLFramework3_5
using Microsoft.Windows.Design.Features;

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
[assembly: ProvideMetadata(typeof(Syncfusion.OlapGrid.Silverlight.VisualStudio.Design.Metadata))]
#endif
namespace Syncfusion.OlapGrid.Silverlight.VisualStudio.Design
{
#if SyncfusionFramework4_0 || SyncfusionSLFramework3_5
    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(Syncfusion.Silverlight.Grid.Olap.OlapGrid), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(OlapGridBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridExpandHyperlinkCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridHyperlinkCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridKpiCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridTemplateCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FormattingWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Silverlight.Grid.Olap.GroupBox), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();

                
            }
        }

    #endregion
    }
#else
    /// <summary>
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    /// </summary>
    internal class Metadata : IRegisterMetadata
    {
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            // tool box filtering
                 builder.AddCustomAttributes(typeof(Syncfusion.Silverlight.Grid.Olap.OlapGrid), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(OlapGridBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridExpandHyperlinkCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridHyperlinkCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridKpiCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OlapGridTemplateCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FormattingWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Silverlight.Grid.Olap.GroupBox), new ToolboxBrowsableAttribute(false));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
