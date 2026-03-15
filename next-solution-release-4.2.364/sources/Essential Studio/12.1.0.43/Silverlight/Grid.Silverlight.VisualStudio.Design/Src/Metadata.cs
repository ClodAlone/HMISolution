#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Windows.Design.Metadata;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Grid.GridPrint;
using System.Windows.Controls;



#if SyncfusionFramework3_5 && !SyncfusionSLFramework3_0

namespace Syncfusion.Grid.Silverlight.VisualStudio.Design
{
    internal class Metadata : IRegisterMetadata
    {

        // Called by Cider to register any design-time metadata
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            // tool box filtering                
            builder.AddCustomAttributes(typeof(GridControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(GridDataControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(GridDataControlBaseImpl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataGroupDropAreaGridImpl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridControlBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataCellNestedGridEditor), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridCellNestedGridEditor), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataTableProperties), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataHeaderCellControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PopupDragWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PopupPositionWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataInt32SliderFilteringPane), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataSliderFilteringPane<>), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(AnimatedGrid), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataExpandCellControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataFilterToggleButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataTextFilteringPane), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataGroupingIndicator), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridTreeControlImpl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridTreeExpanderCellControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridTreeHeaderCellControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataCheckedListBoxControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataFilterCheckedListBoxItem), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridDataDateTimeFilteringPane), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridDataRowControl), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridDataFilterToggleButton), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(GridCell), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GridTreeExpanderCellControlExt), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GraphicCellControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ThumbControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ResizeThumb), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MoveThumb), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DecoratorControl), new ToolboxBrowsableAttribute(false));
            //// Category Editor
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }

}
#else

/// <summary>
/// Container for any general design-time metadata that we want to initialize.
/// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
/// If found, they will instantiate it and call its Register() method automatically.
/// </summary>
using Microsoft.Windows.Design.Features;

[assembly: ProvideMetadata(typeof(Syncfusion.Grid.Silverlight.VisualStudio.Design.Metadata))]
namespace Syncfusion.Grid.Silverlight.VisualStudio.Design
{
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering      


                builder.AddCustomAttributes(
                    typeof(Syncfusion.Windows.Controls.Grid.GridDataControl),
                    new FeatureAttribute(typeof(Syncfusion.Grid.WPF.VisualStudio.Design.GridContextMenuProvider)));

                builder.AddCustomAttributes(typeof(GridControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(GridDataControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(GridDataControlBaseImpl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataGroupDropAreaGridImpl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridControlBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataCellNestedGridEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridCellNestedGridEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataTableProperties), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataHeaderCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PopupDragWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PopupPositionWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataInt32SliderFilteringPane), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataSliderFilteringPane<>), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AnimatedGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataExpandCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataFilterToggleButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataTextFilteringPane), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataGroupingIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridTreeControlImpl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridTreeExpanderCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridTreeHeaderCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataCheckedListBoxControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataFilterCheckedListBoxItem), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(GridDataDateTimeFilteringPane), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataRowControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataFilterToggleButton), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(GridCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridPrintVisual), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridPrintDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RichTextBoxAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridTreeExpanderCellControlExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PrintPreview1), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GraphicCellControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ThumbControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResizeThumb), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MoveThumb), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GridDataExcelLikeFilterPane), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VirtualizingItemsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DecoratorControl), new ToolboxBrowsableAttribute(false));
              
                return builder.CreateTable();
            }
        }
    }
}

#endif
