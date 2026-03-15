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
using Microsoft.Windows.Design;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Controls;


#if SyncfusionFramework4_0
using Microsoft.Windows.Design.Features;
using Syncfusion.Windows.Shared.Controls;
using System.Windows.Controls;

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
[assembly: ProvideMetadata(typeof(Syncfusion.Shared.Silverlight.VisualStudio.Design.Metadata))]
namespace Syncfusion.Shared.Silverlight.VisualStudio.Design
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

#region Editors
                //builder.AddCallback(typeof(NumericTextBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCustomAttributes(typeof(NumericTextBox), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(MaskedTextBoxAdv), new ToolboxBrowsableAttribute(true));
                builder.AddCallback(typeof(EditorBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCustomAttributes(typeof(EditorBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CurrencyTextBox), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(DoubleTextBox), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(MaskedTextBox), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(PercentTextBox), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(IntegerTextBox), new ToolboxBrowsableAttribute(true));
                
                builder.AddCustomAttributes(typeof(DateTimeBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DateTimeEdit), new ToolboxBrowsableAttribute(true));
#endregion

#region Button Controls
                builder.AddCustomAttributes(typeof(DropDown), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownMenuGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownMenuItem), new ToolboxBrowsableAttribute(false));
#endregion

#region WindowControl
                builder.AddCustomAttributes(typeof(MessageContent), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MoveCursor), new ToolboxBrowsableAttribute(false));
#endregion

                #region Carousel
                builder.AddCustomAttributes(typeof(CarouselItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CarouselPanel), new ToolboxBrowsableAttribute(false));
                #endregion

                builder.AddCustomAttributes(typeof(ButtonSpinner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextMenuAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextMenuItemAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DragArrow), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(DragDecorator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SpellCheckDialog), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(FishEyePanel), new ToolboxBrowsableAttribute(true));
                builder.AddCallback(typeof(ColorGroup), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ColorGroupItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ColorPickerPalette), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                builder.AddCustomAttributes(typeof(HeaderedContentControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ListTimePickerPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MenuAdv), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(MenuItemAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MenuItemSeparator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NumericUpDown), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RangeTimePickerPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TileViewControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(TileViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TileViewItemAnimationBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TileViewItemBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TimePicker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TimeUpDown), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TemplateSelector), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(ComboBoxItemAdv), new ToolboxBrowsableAttribute(false));
                
                //builder.AddCustomAttributes(typeof(ToolbarAdv), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ToolbarItemAdv), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ToolbarSplitItemAdv), new ToolboxBrowsableAttribute(false)); 

                builder.AddCustomAttributes(typeof(ToolBarManagerPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarPanelAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarOverflowPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TrayPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FloatingToolBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarManager), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarTrayAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarItemSeparator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WrapPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DataPagerExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BackStageScreenButton), new ToolboxBrowsableAttribute(false));
#region Overview
                builder.AddCustomAttributes(typeof(Overview), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(IScrollOverviewContentHolder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OverviewContentHolder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OverviewCustomPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OverviewResizer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VisualImage), new ToolboxBrowsableAttribute(false));
#endregion
                builder.AddCustomAttributes(typeof(PrintPreviewControl), new ToolboxBrowsableAttribute(false));

                return builder.CreateTable();
            }
        }
    }

#elif SyncfusionFramework3_5
namespace Syncfusion.Shared.Silverlight.VisualStudio.Design
{
    /// <summary>
    /// Represents the metadata class.
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

            #region Carousel
                builder.AddCustomAttributes(typeof(CarouselItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CarouselPanel), new ToolboxBrowsableAttribute(false));
            #endregion

            builder.AddCallback(typeof(EditorBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            //builder.AddCallback(typeof(NumericTextBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCustomAttributes(typeof(ButtonSpinner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ContextMenuAdv), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ContextMenuItemAdv), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CurrencyTextBox), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(DockPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DoubleTextBox), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(SpellCheckDialog),new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(EditorBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DateTimeBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ColorGroup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ColorGroupItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MoreColorsWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PolygonItem), new ToolboxBrowsableAttribute(false));
           // builder.AddCustomAttributes(typeof(FishEyePanel), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(HeaderedContentControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(IntegerTextBox), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(ListTimePickerPopup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MaskedTextBox), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(MenuAdv), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(MenuItemAdv), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MenuItemSeparator), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(NumericUpDown), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PercentTextBox), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(RangeTimePickerPopup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TileViewControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(TileViewItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TileViewItemAnimationBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TileViewItemBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TimePicker), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TimeUpDown), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(ToolbarAdv), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(ToolbarItemAdv), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(ToolbarSplitItemAdv), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(ToolBarManagerPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ToolBarPanelAdv), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ToolBarOverflowPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TrayPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FloatingToolBar), new ToolboxBrowsableAttribute(false));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        

        #endregion
    }
#endif
}
