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
// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
//using Microsoft.Windows.Design.Metadata;
#if SyncfusionFramework4_0
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
#endif
#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Grid.WPF.VisualStudio.Design.Metadata))]
#endif
namespace Syncfusion.Grid.WPF.VisualStudio.Design
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Windows.Design.Features;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using Syncfusion.Windows.Controls.Grid;
    using Microsoft.Windows.Design;

    using Microsoft.Windows.Design.Interaction;
    using Microsoft.Windows.Design.Metadata;
 
    //using Microsoft.Windows.Design.Features;



#if SyncfusionFramework4_0
    public class Metadata : Microsoft.Windows.Design.Metadata.IProvideAttributeTable
    {
        // Accessed by the designer to register any design-time metadata.
        public Microsoft.Windows.Design.Metadata.AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                // Add the menu provider to the design-time metadata.
                builder.AddCustomAttributes(
                    typeof(Syncfusion.Windows.Controls.Grid.GridDataControl),
                    new FeatureAttribute(typeof(ContextMenuProvider)));
                builder.AddCustomAttributes(typeof(GridControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(GridDataControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(GridTreeControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(GridControlBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataCellNestedGridEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridCellNestedGridEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridCellNestedScrollGridEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataTableProperties), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataVisibleColumn), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridCellComboBoxDropDown), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridCellDropDownControlBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridCellGridListControlDropDown), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridCellsControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataCheckedListBoxControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataHeaderCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridListControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PopupDragWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PopupPositionWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataFilterCheckedListBoxItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridTreeControlImpl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataColumnOptionsPane), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataInt32SliderFilteringPane), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataTextFilteringPane), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataSliderFilteringPane<>), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridPrintVisual), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridPrintDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridListControlImpl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Grid.Automation.Peers.GridCellElement), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataDateTimeFilteringPane), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(GridDataDateTimeVisibleColumnControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataFilterToggleButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataColumnChooserWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataRowControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataVisibleColumnControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataCheckBoxVisibleColumnControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataNumberFormatStyleControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataUpDownEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataIntegerEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataPercentEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataDoubleEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataCurrencyEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataMaskEditVisibleColumnControl), new ToolboxBrowsableAttribute(false)); 
                builder.AddCustomAttributes(typeof(GridTreeHeaderCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataFilterBarStyle), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DataPagerExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HoverListBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GraphicCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResizeThumb), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MoveThumb), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataExcelLikeFilterPane), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VirtualizingItemsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HoverListBoxItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AdvanceFilteringWindow), new ToolboxBrowsableAttribute(false));
                //Commented for compilation error in trunk source
                //builder.AddCustomAttributes(typeof(GridRow), new ToolboxBrowsableAttribute(false));
         
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

    


    internal class Metadata : IRegisterMetadata {

        // Called by Cider to register any design-time metadata
        public void Register() {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            // tool box filtering
            builder.AddCustomAttributes(
                   typeof(Syncfusion.Windows.Controls.Grid.GridDataControl),
                   new FeatureAttribute(typeof(ContextMenuProvider)));
            builder.AddCustomAttributes(typeof(GridControl), new ToolboxBrowsableAttribute(true));
			builder.AddCustomAttributes(typeof(GridDataControl), new ToolboxBrowsableAttribute(true));
			builder.AddCustomAttributes(typeof(GridTreeControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(GridControlBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataCellNestedGridEditor), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridCellNestedGridEditor), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridCellNestedScrollGridEditor), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridDataTableProperties), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridDataVisibleColumn), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridCellComboBoxDropDown), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridCellDropDownControlBase), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridCellGridListControlDropDown), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridCellsControl), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridDataCheckedListBoxControl), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridDataHeaderCellControl), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridListControl), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(PopupDragWindow), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(PopupPositionWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataFilterCheckedListBoxItem), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridTreeControlImpl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataColumnOptionsPane), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataInt32SliderFilteringPane), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataTextFilteringPane), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataSliderFilteringPane<>), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridPrintVisual), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridPrintDialog), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridListControlImpl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Controls.Grid.Automation.Peers.GridCellElement), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataDateTimeFilteringPane), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataExcelLikeFilterPane), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HoverListBoxItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VirtualizingItemsPanel), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(GridDataDateTimeVisibleColumnControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridCell), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataFilterToggleButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataColumnChooserWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataRowControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataVisibleColumnControl), new ToolboxBrowsableAttribute(false));           
            builder.AddCustomAttributes(typeof(GridTreeHeaderCellControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataCheckBoxVisibleColumnControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataNumberFormatStyleControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataUpDownEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataIntegerEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataPercentEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataDoubleEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataCurrencyEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataMaskEditVisibleColumnControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DataPagerExt), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HoverListBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GraphicCellControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ResizeThumb), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MoveThumb), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataExcelLikeFilterPane), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VirtualizingItemsPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HoverListBoxItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(AdvanceFilteringWindow), new ToolboxBrowsableAttribute(false));
            //Commented for compilation error in trunk source
            //builder.AddCustomAttributes(typeof(GridRow), new ToolboxBrowsableAttribute(false));
         
            // Category Editor
           

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
