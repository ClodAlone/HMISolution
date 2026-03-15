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
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Syncfusion.Windows.Shared;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Controls.Primitives;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Controls;

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table.
#if SyncfusionFramework4_0 
[assembly: ProvideMetadata(typeof(Syncfusion.Shared.WPF.dll.Design.Metadata))]
#endif
namespace Syncfusion.Shared.WPF.dll.Design
{
    // Container for any general design-time metadata to initialize.
    // Designers look for a type in the design-time assembly that 
    // implements IProvideAttributeTable. If found, designers instantiate 
    // this class and access its AttributeTable property automatically.
#if SyncfusionFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
        // Accessed by the designer to register any design-time metadata.
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                #region Carousel
                builder.AddCustomAttributes(typeof(Carousel), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(CarouselItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CarouselPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomPathCarouselPanel), new ToolboxBrowsableAttribute(false));
                #endregion
                #region Overview
                builder.AddCustomAttributes(typeof(OverviewContentHolder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OverviewCustomPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OverviewResizer), new ToolboxBrowsableAttribute(false));
                #endregion
                // tool box filtering
                builder.AddCustomAttributes(typeof(VistaWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VistaTitleBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TitleBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NavigationButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TitleButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ItemsControlInternalItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DrawingHelper), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TrippleBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ItemsControlInternal), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VistaWindowTitleBarButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ChromelessWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NavigationBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Magnifier), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NonStickingPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Border3D), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PreviewBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SkinPicker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SkinPickerItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SpellCheckDialog), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(PolygonItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ColorGroupItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MoreColorsWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomColor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ColorGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ColorBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BorderEyeDrop), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(Hexagon), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ColorPickerDockPanel), new ToolboxBrowsableAttribute(false));


                //builder.AddCustomAttributes(typeof(DateTimeEditInternal), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DayCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DayCellPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DayGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DayNameCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DayNamesGrid), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(DigitalTextBox), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DomainUpDown), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(MonthButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MonthCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MonthGrid), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(NavigateButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NavigateButtonBase), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(RepeatButtonExt), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(RepeatButtonExt), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(RichPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UpDownCursor), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(YearCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(YearGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(YearRangeCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(YearRangeGrid), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(WeekNumberCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WeekNumbersGrid), new ToolboxBrowsableAttribute(false));
				
				builder.AddCustomAttributes(typeof(TileViewItemBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TileViewItemAnimationBase), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(TileViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TileViewItemCloseButton), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(PinnableListBoxItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PinnableItemsControl), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(MagnifierAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TemplatedAdornerBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WeekNumberCellPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WeekNumberCellPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WeekNumberGridPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AutoTemplatedContentControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AutoTemplatedControl), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(CaretAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MenuAdv), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(MenuItemAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MenuItemSeparator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CalendarButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CalendarDayButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CalendarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownMenuGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownMenuItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResizeGripStyle), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Calendar), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DateTimeBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(EditorBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ExtendedScrollingAdorner), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(ComboBoxItemAdv), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(ToolBarManagerPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FloatingToolBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarOverflowPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TrayPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarPanelAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarTrayAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarManager), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarItemSeparator), new ToolboxBrowsableAttribute(false));
                
                builder.AddCustomAttributes(typeof(PrintPreviewControl), new ToolboxBrowsableAttribute(false));     

                return builder.CreateTable();
            }
        }
    }
#elif SyncfusionFramework3_5
    internal class Metadata : IRegisterMetadata {

        // Called by Cider to register any design-time metadata
        public void Register() {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            builder.AddCustomAttributes(typeof(Carousel), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(CarouselItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CarouselPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CustomPathCarouselPanel), new ToolboxBrowsableAttribute(false));

            // tool box filtering
            builder.AddCustomAttributes(typeof(VistaWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VistaTitleBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TitleBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(NavigationButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TitleButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ItemsControlInternalItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DrawingHelper), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TrippleBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ItemsControlInternal), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VistaWindowTitleBarButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ChromelessWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(NavigationBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Magnifier), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(NonStickingPopup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Border3D), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PreviewBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SkinPicker), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SkinPickerItem), new ToolboxBrowsableAttribute(false));

            //builder.AddCustomAttributes(typeof(DateTimeEditInternal), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DayCell), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DayCellPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DayGrid), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DayNameCell), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DayNamesGrid), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(DigitalTextBox), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(DomainUpDown), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(MonthButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MonthCell), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MonthGrid), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(NavigateButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(NavigateButtonBase), new ToolboxBrowsableAttribute(false));

            //builder.AddCustomAttributes(typeof(RepeatButtonExt), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(RepeatButtonExt), new ToolboxBrowsableAttribute(false));

            //builder.AddCustomAttributes(typeof(RichPopup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(UpDownCursor), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(YearCell), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(YearGrid), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(YearRangeCell), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(YearRangeGrid), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(WeekNumberCell), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(WeekNumbersGrid), new ToolboxBrowsableAttribute(false));
	
			builder.AddCustomAttributes(typeof(TileViewItemBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TileViewItemAnimationBase), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(TileViewItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TileViewItemCloseButton), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(PinnableListBoxItem), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(PinnableItemsControl), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(MagnifierAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TemplatedAdornerBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(WeekNumberCellPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(WeekNumberCellPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(WeekNumberGridPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(AutoTemplatedContentControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(AutoTemplatedControl), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(CaretAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MenuAdv), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(MenuItemAdv), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MenuItemSeparator), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(CalendarButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CalendarDayButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CalendarItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DropDownMenuGroup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DropDownMenuItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ResizeGripStyle), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(Calendar), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(PolygonItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ColorGroupItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MoreColorsWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CustomColor), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ColorGroup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ColorBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(BorderEyeDrop), new ToolboxBrowsableAttribute(false));          
            
            builder.AddCustomAttributes(typeof(DateTimeBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(EditorBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ExtendedScrollingAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(IntegerTextBox), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(DoubleTextBox), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(PercentTextBox), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(CurrencyTextBox), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(DateTimeEdit), new ToolboxBrowsableAttribute(true));
			builder.AddCustomAttributes(typeof(ComboBoxItemAdv), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(ToolBarManagerPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FloatingToolBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarOverflowPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TrayPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarPanelAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarTrayAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarManager), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToolBarItemSeparator), new ToolboxBrowsableAttribute(false));

                #region Overview
                builder.AddCustomAttributes(typeof(OverviewContentHolder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OverviewCustomPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OverviewResizer), new ToolboxBrowsableAttribute(false));
                #endregion

                builder.AddCustomAttributes(typeof(PrintPreviewControl), new ToolboxBrowsableAttribute(false));     

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
