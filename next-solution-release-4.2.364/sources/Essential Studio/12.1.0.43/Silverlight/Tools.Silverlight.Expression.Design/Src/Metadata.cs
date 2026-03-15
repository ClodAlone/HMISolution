#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Tools.Controls;
using Microsoft.Windows.Design.PropertyEditing;

#if SyncfusionFramework3_5
using Microsoft.Windows.Design.Features;

/// <summary>
/// Represents the Metadata class.
/// </summary>
public class Metadata : IRegisterMetadata
{

    /// <summary>
    /// Attaches design-time metadata to a particular control type.
    /// </summary>
    public void Register()
    {

            AttributeTableBuilder builder = new AttributeTableBuilder();

            builder.AddCallback(typeof(TreeViewItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DayCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DayGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DayNameCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DayCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DayGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(MonthCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(MonthGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(WeekNumberCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(WeekNumbersGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(YearCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(YearGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DayCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(NonStackGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(StackGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(GroupBarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(NavigationToolbar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(NavigationToolbarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(PopupMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(PopupMenuItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabPopupMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabPopupMenuItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TreeViewItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CloseButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabContentPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ScrollingPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabControlPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdvBorder), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdvBorderClassic), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(MenuButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ScrollingButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabLayoutPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabPanelAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
           
            builder.AddCallback(typeof(TaskBarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabLayoutPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabPanelAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(TabItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(YearRangeCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DragMarker), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(AutoCompleteListBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CheckedListBoxItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DragMarker), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ColorGroup), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ColorGroupItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ColorPickerPalette), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
            builder.AddCallback(typeof(GradientCollection), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(MoreColorsWindow), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(PolygonItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CustomPanelLabel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CustomPanelTicks), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			builder.AddCallback(typeof(FileUploadControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
			
			builder.AddCustomAttributes(typeof(TreeViewAdv), "Items", new NewItemTypesAttribute(typeof(TreeViewItemAdv)));
            builder.AddCustomAttributes(typeof(AutoComplete), "Items", new NewItemTypesAttribute(typeof(string)));
            builder.AddCustomAttributes(typeof(CheckedListBox), "Items", new NewItemTypesAttribute(typeof(CheckedListBoxItem)));
            builder.AddCustomAttributes(typeof(GroupBar), "Items", new NewItemTypesAttribute(typeof(GroupBarItem)));
            builder.AddCustomAttributes(typeof(TaskBar), "Items", new NewItemTypesAttribute(typeof(TaskBarItem)));
            builder.AddCustomAttributes(typeof(TabControlAdv), "Items", new NewItemTypesAttribute(typeof(TabItemAdv)));

  builder.AddCallback(typeof(TabNavigationItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(NavigationToolTip), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
				
            MetadataStore.AddAttributeTable(builder.CreateTable());
    }
}
#elif SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Tools.Silverlight.Expression.Design.RegisterToolstControlsMetadata))]
namespace Syncfusion.Tools.Silverlight.Expression.Design 
{
    internal class RegisterToolstControlsMetadata : IProvideAttributeTable 
    {
        public AttributeTable AttributeTable 
        {
            get 
            {
                ToolsControlsAttributeTableBuilder builder = new ToolsControlsAttributeTableBuilder();

                builder.AddCallback(typeof(TreeViewItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DayCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DayGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DayNameCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DayCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DayGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(MonthCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(MonthGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(WeekNumberCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(WeekNumbersGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(YearCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(YearGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DayCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(NonStackGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(StackGrid), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(GroupBarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(NavigationToolbar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(NavigationToolbarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(PopupMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(PopupMenuItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabPopupMenu), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabPopupMenuItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TreeViewItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(CloseButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabContentPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ScrollingPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabControlPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdvBorder), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdvBorderClassic), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(MenuButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ScrollingButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabLayoutPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabPanelAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ComboBoxItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
     
                builder.AddCallback(typeof(TaskBarItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabLayoutPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabPanelAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(TabItemAdv), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(YearRangeCell), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DragMarker), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(AutoCompleteListBox), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(CheckedListBoxItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(DragMarker), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(ColorGroup), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(ColorGroupItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(ColorPickerPalette), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                //builder.AddCallback(typeof(GradientCollection), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(MoreColorsWindow), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                //builder.AddCallback(typeof(PolygonItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(CustomPanelLabel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(CustomPanelTicks), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
				builder.AddCallback(typeof(FileUploadControl), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
				
				builder.AddCustomAttributes(typeof(TreeViewAdv), "Items", new NewItemTypesAttribute(typeof(TreeViewItemAdv)));
                builder.AddCustomAttributes(typeof(AutoComplete), "Items", new NewItemTypesAttribute(typeof(string)));
                builder.AddCustomAttributes(typeof(CheckedListBox), "Items", new NewItemTypesAttribute(typeof(CheckedListBoxItem)));
                builder.AddCustomAttributes(typeof(GroupBar), "Items", new NewItemTypesAttribute(typeof(GroupBarItem)));
                builder.AddCustomAttributes(typeof(TaskBar), "Items", new NewItemTypesAttribute(typeof(TaskBarItem)));
                builder.AddCustomAttributes(typeof(TabControlAdv), "Items", new NewItemTypesAttribute(typeof(TabItemAdv)));

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
}
#endif
