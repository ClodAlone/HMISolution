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
    
#if SyncfusionFramework4_0 || SyncfusionSLFramework3_5
using Microsoft.Windows.Design.Features;
using Syncfusion.Silverlight.Controls.PivotGrid;
using Syncfusion.Silverlight.Controls.PivotSchemaDesigner;

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
[assembly: ProvideMetadata(typeof(Syncfusion.PivotAnalysis.Silverlight.VisualStudio.Design.Metadata))]
#endif
namespace Syncfusion.PivotAnalysis.Silverlight.VisualStudio.Design
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
                builder.AddCustomAttributes(typeof(PivotGridControl), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(PivotGridControlBase), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotExpanderCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridHyperlinkCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridTemplateCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AnimatedGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridGroupingBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGroupingItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridRowGroupBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridComputationListWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotSortHeaderCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotSchemaDesigner), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(ComputationInfoWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DeletePivotItemCommand), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DeleteFilterCommand), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ShowFilterCommand), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ShowCalculationCommand), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotTableField), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PopupWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Utils), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotFieldListWindow), ToolboxBrowsableAttribute.No);
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
               builder.AddCustomAttributes(typeof(PivotGridControl), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(PivotGridControlBase), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotExpanderCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotSortHeaderCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridHyperlinkCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridTemplateCell), ToolboxBrowsableAttribute.No);
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
