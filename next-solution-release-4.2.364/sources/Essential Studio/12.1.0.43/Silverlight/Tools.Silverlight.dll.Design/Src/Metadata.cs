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
using Syncfusion.Windows.Controls;
using Syncfusion.Windows.Controls.Primitives;
using Syncfusion.Windows.Tools.Controls;

#if SyncfusionFramework4_0
using Microsoft.Windows.Design.Features; 

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
[assembly: ProvideMetadata(typeof(Syncfusion.Tools.Silverlight.VisualStudio.Design.Metadata))]
namespace Syncfusion.Tools.Silverlight.VisualStudio.Design
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

                builder.AddCallback(typeof(AutoCompleteListBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(CheckedListBoxItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(CloseButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));               
                builder.AddCallback(typeof(CustomPanelLabel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(CustomPanelTicks), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DayCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));            
                builder.AddCallback(typeof(DayGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DayGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DayNameCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DragMarker), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DragMarker), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(GradientCollection), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(GroupBarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HtmlHost), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(MenuButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(MonthCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(MonthGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(MoreColorsWindow), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(NavigationToolbar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(NavigationToolbarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(NonStackGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(PolygonItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(PopupMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(PopupMenuItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ScrollingButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ScrollingPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(StackGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabContentPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabControlPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));            
                builder.AddCallback(typeof(TabItemAdvBorder), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdvBorderAero), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdvBorderBlend), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdvBorderClassic), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdvBorderIE), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdvBorderOffice2003), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdvBorderOffice2007), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdvBorderVS2008), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));            
                builder.AddCallback(typeof(TabLayoutPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));            
                builder.AddCallback(typeof(TabPanelAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabPopupMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabPopupMenuItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TaskBarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TreeViewItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));          
                builder.AddCallback(typeof(WeekNumberCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(WeekNumbersGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(YearCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(YearGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(YearRangeCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));				
                //builder.AddCallback(typeof(FileUploadControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                builder.AddCallback(typeof(TabNavigationItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(NavigationToolTip), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                #region Hierarchy Meta Data
                builder.AddCallback(typeof(HierarchyNavigatorItemsControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HierarchyNavigatorItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HierarchyNavigatorHistoryListBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HierarchyNavigatorHistoryControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HierarchyNavigatorDropDownItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HierarchyNavigatorBarContent), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HierarchyNavigatorModel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false))); 
                #endregion
				
                return builder.CreateTable();
            }
        }
    }

#elif SyncfusionFramework3_5
namespace Syncfusion.Tools.Silverlight.VisualStudio.Design
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

            builder.AddCallback(typeof(AutoCompleteListBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CheckedListBoxItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CloseButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            //builder.AddCallback(typeof(ColorGroup), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            //builder.AddCallback(typeof(ColorGroupItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            //builder.AddCallback(typeof(ColorPickerPalette), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
            builder.AddCallback(typeof(CustomPanelLabel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CustomPanelTicks), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DayCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DayGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DayGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DayNameCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DragMarker), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DragMarker), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            //builder.AddCallback(typeof(GradientCollection), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(GroupBarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(HtmlHost), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(MenuButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(MonthCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(MonthGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            //builder.AddCallback(typeof(MoreColorsWindow), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(NavigationToolbar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(NavigationToolbarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(NonStackGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            //builder.AddCallback(typeof(PolygonItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(PopupMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(PopupMenuItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ScrollingButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ScrollingPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(StackGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabContentPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabControlPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdvBorder), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdvBorderAero), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdvBorderBlend), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdvBorderClassic), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdvBorderIE), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdvBorderOffice2003), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdvBorderOffice2007), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdvBorderVS2008), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabLayoutPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabPanelAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabPopupMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabPopupMenuItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TaskBarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TreeViewItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(WeekNumberCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(WeekNumbersGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(YearCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(YearGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(YearRangeCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(FileUploadControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

     builder.AddCallback(typeof(TabNavigationItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(NavigationToolTip), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        #endregion
    }
#endif
}
