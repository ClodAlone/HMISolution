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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Controls;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Tools.Controls;
using Microsoft.Windows.Design.PropertyEditing;

#if SyncfusionFramework4_0
using Microsoft.Windows.Design.Features;
using Syncfusion.Windows.Controls;
using Syncfusion.Windows.Shared.Controls;
using System.Windows.Controls;


// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
[assembly: ProvideMetadata(typeof(Syncfusion.Shared.Silverlight.Expression.Design.Metadata))]
namespace Syncfusion.Shared.Silverlight.Expression.Design
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


                //builder.AddCallback(typeof(NumericTextBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(CurrencyTextBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(PercentTextBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(MaskedTextBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(DoubleTextBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(MaskedTextBoxAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                //builder.AddCustomAttributes(typeof(NumericTextBox), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(MaskedTextBoxAdv), new ToolboxBrowsableAttribute(true));

                builder.AddCallback(typeof(EditorBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCustomAttributes(typeof(EditorBase), new ToolboxBrowsableAttribute(false));
                builder.AddCallback(typeof(DateTimeBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCustomAttributes(typeof(DateTimeBase), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(IntegerTextBox), new ToolboxBrowsableAttribute(true));
                //builder.AddCustomAttributes(typeof(DoubleTextBox), new ToolboxBrowsableAttribute(true));
                //builder.AddCustomAttributes(typeof(CurrencyTextBox), new ToolboxBrowsableAttribute(true));
                //builder.AddCustomAttributes(typeof(PercentTextBox), new ToolboxBrowsableAttribute(true));
                //builder.AddCustomAttributes(typeof(MaskedTextBox), new ToolboxBrowsableAttribute(true));
                //builder.AddCustomAttributes(typeof(DateTimeEdit), new ToolboxBrowsableAttribute(true));
				builder.AddCustomAttributes(typeof(DragArrow), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(DragDecorator), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(MoveCursor), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(TileViewItemBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TileViewItemAnimationBase), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ContextMenuAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextMenuItemAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SeparatorAdv), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(FishEyePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MenuAdv), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(MenuItemAdv), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ToolbarAdv), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ToolbarItemAdv), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ToolbarSplitItemAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HeaderedContentControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ButtonSpinner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ListTimePickerPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RangeTimePickerPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TimePicker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TimeUpDown), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TemplateSelector), new ToolboxBrowsableAttribute(false));				
                builder.AddCustomAttributes(typeof(TileViewControl), "Items", new NewItemTypesAttribute(typeof(TileViewItem)));

                builder.AddCustomAttributes(typeof(DropDownMenuItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownMenuGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDown), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MessageContent), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(CarouselItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CarouselPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ComboBoxItemAdv), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(ToolBarManagerPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarPanelAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarItemSeparator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarOverflowPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TrayPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FloatingToolBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarTrayAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarManager), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarItemSeparator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WrapPanel), new ToolboxBrowsableAttribute(false));

                #region Overview
                builder.AddCustomAttributes(typeof(Overview), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(IScrollOverviewContentHolder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OverviewContentHolder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OverviewCustomPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OverviewResizer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VisualImage), new ToolboxBrowsableAttribute(false));
                #endregion

                builder.AddCustomAttributes(typeof(PrintPreviewControl), new ToolboxBrowsableAttribute(false));
		        builder.AddCustomAttributes(typeof(BackStageScreenButton), new ToolboxBrowsableAttribute(false));

                return builder.CreateTable();
            }
        }
    }

#elif SyncfusionFramework3_5
namespace Syncfusion.Shared.Silverlight.Expression.Design
{
    /// <summary>
    /// Represents the Metadata class.
    /// </summary>
    public class Metadata : IRegisterMetadata
    {
    #region IRegisterMetadata Members

        /// <summary>
        /// Attaches design-time metadata to a particular control type.
        /// </summary>
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
           
            builder.AddCallback(typeof(EditorBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(NumericTextBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
				builder.AddCustomAttributes(typeof(TileViewItemBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TileViewItemAnimationBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextMenuAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextMenuItemAdv), new ToolboxBrowsableAttribute(false));
               // builder.AddCustomAttributes(typeof(FishEyePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MenuAdv), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(MenuItemAdv), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ToolbarAdv), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ToolbarItemAdv), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ToolbarSplitItemAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HeaderedContentControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ButtonSpinner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ListTimePickerPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RangeTimePickerPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TimePicker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TimeUpDown), new ToolboxBrowsableAttribute(false));

				builder.AddCustomAttributes(typeof(TileViewControl), "Items", new NewItemTypesAttribute(typeof(TileViewItem)));
				
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        

    #endregion
    }
#endif
}
