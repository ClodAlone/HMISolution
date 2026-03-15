#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if SyncfusionFramework4_0 || SyncfusionFramework4_5
using Microsoft.Windows.Design.Metadata;
[assembly: ProvideMetadata(typeof(Syncfusion.PivotAnalysis.Wpf.VisualStudio.Design.Metadata))]
#endif

namespace Syncfusion.PivotAnalysis.Wpf.VisualStudio.Design
{
    using Microsoft.Windows.Design.Metadata;
    using Syncfusion.Windows.Controls.Grid;
    using Microsoft.Windows.Design;
    using Syncfusion.Windows.Controls.PivotGrid;
    using Syncfusion.Windows.Controls.PivotSchemaDesigner;
#if SyncfusionFramework4_0 || SyncfusionFramework4_5

    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                //Proceeds the toolbox hidden elements.
                builder.AddCustomAttributes(typeof(PivotGridControl), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(PivotGridControlBase), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotExpanderCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotSortHeaderCell), ToolboxBrowsableAttribute.No);

                builder.AddCustomAttributes(typeof(PivotGridHyperlinkCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridTemplateCell), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FilterPopup), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotComputationInfoPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridGroupingBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGroupingItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AnimatedGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridRowGroupBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DragIndicatorAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FieldListAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DragIndicatorButton), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridFieldList), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridComputationList), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ComputationListAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ColumnFilterPopup), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FilterButton), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ValueChooserAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotSchemaDesigner), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(ComputationInfoWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotCommands), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotTableField), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PopupWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Utils), ToolboxBrowsableAttribute.No);
                return builder.CreateTable();
            }
        }

        #endregion
    }

#elif SyncfusionFramework3_5

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
                builder.AddCustomAttributes(typeof(FilterPopup), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotComputationInfoPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridGroupingBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGroupingItemsControl), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(AnimatedGrid), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridRowGroupBar), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DragIndicatorAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FieldListAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DragIndicatorButton), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridFieldList), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotGridComputationList), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ComputationListAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ColumnFilterPopup), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(FilterButton), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ValueChooserAdorner), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotSchemaDesigner), ToolboxBrowsableAttribute.Yes);
                builder.AddCustomAttributes(typeof(ComputationInfoWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotCommands), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PivotTableField), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PopupWindow), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Utils), ToolboxBrowsableAttribute.No);

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }

#endif

}
