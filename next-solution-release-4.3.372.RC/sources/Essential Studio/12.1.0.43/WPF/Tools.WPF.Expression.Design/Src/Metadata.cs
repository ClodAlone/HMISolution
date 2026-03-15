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
using Microsoft.Windows.Design.Metadata;
 #if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Tools.WPF.Expression.Design.Metadata))]
#endif
namespace Syncfusion.Tools.WPF.Expression.Design
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Windows.Design.Metadata;
    using System.ComponentModel;
    using Microsoft.Windows.Design.PropertyEditing;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using Syncfusion.Windows.Tools.Controls;
    using Microsoft.Windows.Design;
    using Microsoft.Windows.Design.Features;
    using Syncfusion.Windows.Design;
    using Syncfusion.Tools.WPF.VisualStudio.Design;



    /// <summary>
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    /// </summary>
    #if SyncfusionFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {

            get
            {


                // Called by Cider to register any design-time metadata
                //public void Register() {
                AttributeTableBuilder builder = new AttributeTableBuilder();


                // tool box filtering
                builder.AddCustomAttributes(typeof(QATCustomizeRibbonDialog),new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabScrollViewer),new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GalleryFilterSelector), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GalleryGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GalleryGroupPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GalleryItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GalleryStackPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonComboBoxItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ApplicationMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ApplicationMenuGroup), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(BorderEyeDrop), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BottomThumb), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BottomLine), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ButtonLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ButtonPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CheckableBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextMenuBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextTabGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomContextMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomItemsControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomMenuItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TwoLinesLayoutPanel), new ToolboxBrowsableAttribute(false));
     builder.AddCustomAttributes(typeof(CardGroupControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CardViewItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CardViewPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ComboBoxItemAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DirectTabPanel), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(DockedElementHost), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockedElementsContainer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockedElementTabbedHost), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockHeaderPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockPreviewMainButtonVS2005), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DraggedElementPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ExpanderExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ExtendedPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FontListBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TablePickerItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TablePickerUI), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FloatWindowBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FontListBox), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(FontListBoxInternal), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FontListBoxInternalItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupBarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupBarItemHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupBarSplitter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupViewItem), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(HorizontalLine), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InRibbonItemsPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InternalPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ItemsPresenterExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LabelTextBlock), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LargeButtonPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MenuButton), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(MultilinePanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(NavigationToolbar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OpacityDockPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PopupResizeThumb), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QuickAccessToolBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QuickAccessToolBarPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonBar), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(RibbonColorPicker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonButtonChecker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonCheckBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonComboBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QuickAccessToolBarPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomPopup), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(RibbonButtonChecker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonCheckBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonComboBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QuickAccessToolBarPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonButtonChecker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonCheckBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonComboBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonGallery), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonGalleryGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonGalleryItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonListBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonMenuGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonMenuItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonPage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonRadioButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonSeparator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonStatusBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonTab), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonTextBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonWindowPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScreenTip), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SidePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SimpleMenuButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitMenuButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Splitter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MiniToolbar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BackStageSeparator), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(Backstage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonTabItemsControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BackStageButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BackStageCommandButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BackstageTabItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonToggleButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownMenuGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownMenuItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonAdorner), new ToolboxBrowsableAttribute(false));
                

                builder.AddCustomAttributes(typeof(TabButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TaskBarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TaskBarStackPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TextImageControl), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.TitleBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToggleButtonExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewAdvItemsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewAdvVirtualizingPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewItemAdv), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(TreeViewItemAdvVirtualizingPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TwoItemsLayoutPanelEx), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VistaSpecificProgressBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WindowTitleBarButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeRootLine), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(WrapPanelExt), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(SplitterItemBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WizardNavigationArea), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WizardPage), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(OneNoteTabBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DocumentContextMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TDILayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NativeFloatWindow), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(CornerPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomWrapPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DocumentPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ItemHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SelectRectangle), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabPanelAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Office2007TabBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Office2010TabBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VS2008TabBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HeaderPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScrollingPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabItemExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QuickTabSwicthPreviewControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VistaFlipSwitchPreviewControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VS2005SwitchPreviewControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DocumentTabControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DocumentHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MDILayoutPanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(TrackContainer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ListSwicthPreviewControl), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(HierarchyNavigatorItem), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorBarContent), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorDropDownItem), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorHistoryControl), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorHistoryListBox), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorItemsControl), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorModel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(PagesLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitHeaderPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabSplitterItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabSplitter), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(TabSplitterItemPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VS2008SplitterItemBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomGridSplitter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitterPage), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(TreeViewColumnHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewHeaderRowPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewRowPresenter), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(CustomPanelTicks), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomPanelLabel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(BalloonTipBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BalloonTipHeader), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ColorBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BalloonTip), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(NotifyIcon), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(Office2007SplitterItemBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OneNoteSplitterItemBorder), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DocMenuItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CheckListBox), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(CheckListBoxItem), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(ColorPickerDockPanel), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(Hexagon), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AdornerFloatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AdornerWindowsLayoutPanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(FackItemsAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CutawayAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DraggedAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DraggedObjectShader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FloatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HostAdornerVS2003), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HostAdornerVS2005), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InsertionAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MainHost), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MDIWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MoveMDIAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NewTabLayout), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Panel3DAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QATCustomizationDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonContextMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitterAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TDISplitPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewColumnHeaderAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewItemAdvDragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewRowDragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewRowDragMarkerTemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewColumnHeaderTemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewItemAdvDragMarkerAdornerrInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UIElementAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AutoTemplatedItemsControl), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(AutoTemplatedItemsControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CutawayAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DraggedAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DraggedObjectShader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FloatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FackItemsAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HostAdornerVS2003), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HostAdornerVS2005), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InsertionAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MainHost), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MDIWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MoveMDIAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NewTabLayout), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Panel3DAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QATCustomizationDialog), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(RibbonColorPicker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonContextMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitterAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TDISplitPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewColumnHeaderAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewColumnHeaderTemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewItemAdvDragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewItemAdvDragMarkerAdornerrInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewRowDragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewRowDragMarkerTemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UIElementAdorner), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(TabPreviewAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonAdornerInternalControl), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(ItemsCollection), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(CardGroupControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CardViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CardViewPanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(TabNavigationItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NavigationToolTip), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScrollButtonsBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PopupSidePanel), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(PolygonItem), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ColorGroup), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ColorGroupItem), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(MoreColorsWindow), new ToolboxBrowsableAttribute(false));	

				
    #region TabSplitter
                builder.AddCustomAttributes(typeof(TabSplitter), new FeatureAttribute(typeof(TabSplitterInitializer)));
                builder.AddCustomAttributes(typeof(TabSplitter), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(TabSplitter), new FeatureAttribute(typeof(TabSplitterAdornerProvider)));
                # endregion

    #region Ribbon

    #region RibbonButton
                builder.AddCustomAttributes(typeof(RibbonButton), new ToolboxBrowsableAttribute(true));

                builder.AddCustomAttributes(typeof(RibbonButton), new FeatureAttribute(typeof(RibbonButtonInitializer)));
                #endregion

                builder.AddCustomAttributes(typeof(Ribbon), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(Ribbon), new FeatureAttribute(typeof(RibbonAdornerProvider)));
                builder.AddCustomAttributes(typeof(Ribbon), new FeatureAttribute(typeof(RibbonInitializer)));
                builder.AddCustomAttributes(typeof(Ribbon), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(RibbonTab), new FeatureAttribute(typeof(RibbonTabAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonTab), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                // builder.AddCustomAttributes(typeof(ButtonPanel), new FeatureAttribute(typeof(RibbonButtonPanelAdornerProvider)));

                builder.AddCustomAttributes(typeof(RibbonGallery), new FeatureAttribute(typeof(RibbonGalleryAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonGallery), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(RibbonButton), new FeatureAttribute(typeof(RibbonButtonAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonButton), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                //builder.AddCustomAttributes(typeof(SplitButton), new FeatureAttribute(typeof(RibbonSplitButtonAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonTextBox), new FeatureAttribute(typeof(RibbonTextBoxAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonTextBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(RibbonCheckBox), new FeatureAttribute(typeof(RibbonCheckBoxAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonCheckBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(RibbonRadioButton), new FeatureAttribute(typeof(RibbonRadioButtonAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonRadioButton), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(RibbonComboBox), new FeatureAttribute(typeof(RibboncomboboxAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonComboBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(DropDownButton), new FeatureAttribute(typeof(RibbonDropDownButtonAdornerProvider)));
                builder.AddCustomAttributes(typeof(DropDownButton), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));


                builder.AddCustomAttributes(typeof(RibbonBar), new FeatureAttribute(typeof(RibbonBarAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonBar), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(ApplicationMenu), new FeatureAttribute(typeof(AppMenuAdornerProvider)));
                builder.AddCustomAttributes(typeof(ApplicationMenu), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                #endregion

    #region GroupBar

                builder.AddCustomAttributes(typeof(GroupBar), new FeatureAttribute(typeof(GroupBarInitializer)));

                builder.AddCustomAttributes(typeof(GroupBar), new FeatureAttribute(typeof(GroupBarAdornerProvider)));
                builder.AddCustomAttributes(typeof(GroupBar), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(GroupBarItem), new FeatureAttribute(typeof(GroupBarItemAdornerProvider)));
                builder.AddCustomAttributes(typeof(GroupBarItem), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(GroupView), new FeatureAttribute(typeof(GroupViewAdornerProvider)));
                builder.AddCustomAttributes(typeof(GroupView), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(GroupViewItem), new FeatureAttribute(typeof(GroupViewItemAdornerProvider)));
                builder.AddCustomAttributes(typeof(GroupViewItem), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(GroupBarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupBarItemHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupBarSplitter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NavigationToolbar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NavigationToolbarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TextImageControl), new ToolboxBrowsableAttribute(false));

                #endregion

    #region TabControlExt
                builder.AddCustomAttributes(typeof(TabControlExt), new FeatureAttribute(typeof(TabControlExtInitializer)));
                builder.AddCustomAttributes(typeof(TabControlExt), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(TabControlExt), new FeatureAttribute(typeof(TabControlExtAdornerProvider)));
                builder.AddCustomAttributes(typeof(TabItemExt), new FeatureAttribute(typeof(TabItemExtAdornerProvider)));
                builder.AddCustomAttributes(typeof(TabItemExt), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(TabControlExt), "Items", new NewItemTypesAttribute(typeof(TabItemExt)));

                #endregion

    #region Docking
                builder.AddCustomAttributes(typeof(DockingManager), new FeatureAttribute(typeof(DockingInitializer)));
                builder.AddCustomAttributes(typeof(DockingManager), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(DockingManager), new FeatureAttribute(typeof(DockingAdornerProvider)));
                builder.AddCustomAttributes(typeof(DockingManager), "Children", new NewItemTypesAttribute(typeof(Grid)));
                #endregion

    #region Document Container

                builder.AddCustomAttributes(typeof(DocumentContainer), new FeatureAttribute(typeof(DocumentContainerInitializer)));
                builder.AddCustomAttributes(typeof(DocumentContainer), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(DocumentContainer), new FeatureAttribute(typeof(DocumentContainerAdornerProvider)));
                builder.AddCustomAttributes(typeof(DocumentContainer), "Items", new NewItemTypesAttribute(typeof(Grid)));
                #endregion

    #region AutoComplete
                builder.AddCustomAttributes(typeof(AutoComplete), new FeatureAttribute(typeof(AutoCompleteInitializer)));
                builder.AddCustomAttributes(typeof(AutoComplete), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(AutoComplete), new FeatureAttribute(typeof(AutoCompleteAdornerProvider)));
                #endregion

    #region FontListBox
                builder.AddCustomAttributes(typeof(FontListBox), new FeatureAttribute(typeof(FontListBoxInitializer)));
                builder.AddCustomAttributes(typeof(FontListBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(FontListBox), new FeatureAttribute(typeof(FontListBoxAdornerProvider)));
                #endregion

    #region FontListComboBox
                builder.AddCustomAttributes(typeof(FontListComboBox), new FeatureAttribute(typeof(FontListComboBoxInitializer)));
                builder.AddCustomAttributes(typeof(FontListComboBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(FontListComboBox), new FeatureAttribute(typeof(FontListComboBoxAdornerProvider)));
                #endregion



    # region CheckListbox
                builder.AddCustomAttributes(typeof(CheckListBox), new FeatureAttribute(typeof(CheckListBoxInitializer)));
                builder.AddCustomAttributes(typeof(CheckListBox), "Items", new NewItemTypesAttribute(typeof(CheckListBoxItem)));
                builder.AddCustomAttributes(typeof(CheckListBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(CheckListBox), new FeatureAttribute(typeof(CheckListBoxAdornerProvider)));

                #endregion

    #region Gallery
                builder.AddCustomAttributes(typeof(Gallery), new FeatureAttribute(typeof(GalleryInitializer)));
                builder.AddCustomAttributes(typeof(Gallery), new FeatureAttribute(typeof(GalleryAdornerProvider)));
                builder.AddCustomAttributes(typeof(Gallery), "Items", new NewItemTypesAttribute(typeof(GalleryItem)));
                #endregion




    #region TreeViewAdv

                builder.AddCustomAttributes(typeof(TreeViewAdv), new FeatureAttribute(typeof(TreeViewAdvInitializer)));
                builder.AddCustomAttributes(typeof(TreeViewAdv), new FeatureAttribute(typeof(TreeViewAdvAdornerProvider)));
                builder.AddCustomAttributes(typeof(TreeViewAdv), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(TreeViewItemAdv), new FeatureAttribute(typeof(TreeViewItemAdvAdornerProvider)));
                builder.AddCustomAttributes(typeof(TreeViewItemAdv), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));


                #endregion

    #region HierarchyNavigator
                builder.AddCustomAttributes(typeof(HierarchyNavigator), new FeatureAttribute(typeof(HierarchyNavigatorInitializer)));
                builder.AddCustomAttributes(typeof(HierarchyNavigator), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(HierarchyNavigator), new FeatureAttribute(typeof(HierarchyNavigatorAdornerProvider)));
                #endregion

    #region TaskBar

                builder.AddCustomAttributes(typeof(TaskBar), new FeatureAttribute(typeof(TaskBarInitializer)));
                builder.AddCustomAttributes(typeof(TaskBar), new FeatureAttribute(typeof(TaskBarAdornerProvider)));
                builder.AddCustomAttributes(typeof(TaskBar), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(TaskBarItem), new FeatureAttribute(typeof(TaskBarItemAdornerProvider)));
                builder.AddCustomAttributes(typeof(TaskBarItem), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));


                #endregion

    #region RangeSlider
                builder.AddCustomAttributes(typeof(RangeSliderControl), new FeatureAttribute(typeof(RangeSliderInitializer)));
                builder.AddCustomAttributes(typeof(RangeSliderControl), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(RangeSliderControl), new FeatureAttribute(typeof(RangeSliderAdornerProvider)));
                #endregion

              



    #region Wizard
                builder.AddCustomAttributes(typeof(WizardControl), new FeatureAttribute(typeof(WizardControlAdornerProvider)));
                builder.AddCustomAttributes(typeof(WizardControl), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(WizardPageAdornerProvider)));
                builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(WizardPageInitializer)));
                builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                #endregion
                return builder.CreateTable();
            }
        }
    }
#elif SyncfusionFramework4_5
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {

            get
            {


                // Called by Cider to register any design-time metadata
                //public void Register() {
                AttributeTableBuilder builder = new AttributeTableBuilder();


                // tool box filtering
                builder.AddCustomAttributes(typeof(QATCustomizeRibbonDialog),new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabScrollViewer),new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GalleryFilterSelector), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GalleryGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GalleryGroupPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GalleryItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GalleryStackPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonComboBoxItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ApplicationMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ApplicationMenuGroup), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(BorderEyeDrop), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BottomThumb), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BottomLine), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ButtonLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ButtonPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CheckableBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextMenuBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextTabGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomContextMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomItemsControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomMenuItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TwoLinesLayoutPanel), new ToolboxBrowsableAttribute(false));
     builder.AddCustomAttributes(typeof(CardGroupControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CardViewItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CardViewPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ComboBoxItemAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DirectTabPanel), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(DockedElementHost), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockedElementsContainer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockedElementTabbedHost), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockHeaderPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DockPreviewMainButtonVS2005), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DraggedElementPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ExpanderExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ExtendedPopup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FontListBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TablePickerItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TablePickerUI), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FloatWindowBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FontListBox), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(FontListBoxInternal), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FontListBoxInternalItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupBarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupBarItemHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupBarSplitter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupViewItem), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(HorizontalLine), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InRibbonItemsPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InternalPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ItemsPresenterExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LabelTextBlock), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LargeButtonPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MenuButton), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(MultilinePanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(NavigationToolbar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OpacityDockPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PopupResizeThumb), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QuickAccessToolBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QuickAccessToolBarPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonBar), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(RibbonColorPicker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonButtonChecker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonCheckBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonComboBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QuickAccessToolBarPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomPopup), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(RibbonButtonChecker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonCheckBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonComboBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QuickAccessToolBarPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonButtonChecker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonCheckBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonComboBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonGallery), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonGalleryGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonGalleryItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonListBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonMenuGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonMenuItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonPage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonRadioButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonSeparator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonStatusBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonTab), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonTextBox), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonWindowPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScreenTip), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SidePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SimpleMenuButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitMenuButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Splitter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MiniToolbar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BackStageSeparator), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(Backstage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonTabItemsControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BackStageButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BackStageCommandButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BackstageTabItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonToggleButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownMenuGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DropDownMenuItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonAdorner), new ToolboxBrowsableAttribute(false));
                

                builder.AddCustomAttributes(typeof(TabButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TaskBarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TaskBarStackPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TextImageControl), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.TitleBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ToggleButtonExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewAdvItemsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewAdvVirtualizingPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewItemAdv), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(TreeViewItemAdvVirtualizingPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TwoItemsLayoutPanelEx), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VistaSpecificProgressBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WindowTitleBarButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeRootLine), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(WrapPanelExt), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(SplitterItemBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WizardNavigationArea), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WizardPage), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(OneNoteTabBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DocumentContextMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TDILayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NativeFloatWindow), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(CornerPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomWrapPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DocumentPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ItemHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SelectRectangle), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabPanelAdv), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Office2007TabBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Office2010TabBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VS2008TabBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HeaderPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScrollingPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabItemExt), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QuickTabSwicthPreviewControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VistaFlipSwitchPreviewControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VS2005SwitchPreviewControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DocumentTabControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DocumentHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MDILayoutPanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(TrackContainer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ListSwicthPreviewControl), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(HierarchyNavigatorItem), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorBarContent), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorDropDownItem), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorHistoryControl), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorHistoryListBox), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorItemsControl), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(HierarchyNavigatorModel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(PagesLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitHeaderPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabSplitterItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TabSplitter), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(TabSplitterItemPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VS2008SplitterItemBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomGridSplitter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitterPage), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(TreeViewColumnHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewHeaderRowPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewRowPresenter), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(CustomPanelTicks), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CustomPanelLabel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(BalloonTipBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BalloonTipHeader), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ColorBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BalloonTip), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(NotifyIcon), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(Office2007SplitterItemBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(OneNoteSplitterItemBorder), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DocMenuItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CheckListBox), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(CheckListBoxItem), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(ColorPickerDockPanel), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(Hexagon), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AdornerFloatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AdornerWindowsLayoutPanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(FackItemsAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CutawayAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DraggedAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DraggedObjectShader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FloatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HostAdornerVS2003), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HostAdornerVS2005), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InsertionAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MainHost), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MDIWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MoveMDIAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NewTabLayout), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Panel3DAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QATCustomizationDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonContextMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitterAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TDISplitPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewColumnHeaderAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewItemAdvDragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewRowDragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewRowDragMarkerTemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewColumnHeaderTemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewItemAdvDragMarkerAdornerrInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UIElementAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AutoTemplatedItemsControl), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(AutoTemplatedItemsControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CutawayAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DraggedAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DraggedObjectShader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FloatWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FackItemsAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HostAdornerVS2003), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HostAdornerVS2005), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(InsertionAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MainHost), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MDIWindow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MoveMDIAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NewTabLayout), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Panel3DAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QATCustomizationDialog), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(RibbonColorPicker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonContextMenu), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SplitterAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TDISplitPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewColumnHeaderAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewColumnHeaderTemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewItemAdvDragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewItemAdvDragMarkerAdornerrInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewRowDragMarkerAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeViewRowDragMarkerTemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UIElementAdorner), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(TabPreviewAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RibbonAdornerInternalControl), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(ItemsCollection), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(CardGroupControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CardViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CardViewPanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(TabNavigationItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NavigationToolTip), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScrollButtonsBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PopupSidePanel), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(PolygonItem), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ColorGroup), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ColorGroupItem), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(MoreColorsWindow), new ToolboxBrowsableAttribute(false));	

				
    #region TabSplitter
                builder.AddCustomAttributes(typeof(TabSplitter), new FeatureAttribute(typeof(TabSplitterInitializer)));
                builder.AddCustomAttributes(typeof(TabSplitter), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(TabSplitter), new FeatureAttribute(typeof(TabSplitterAdornerProvider)));
                # endregion

    #region Ribbon

    #region RibbonButton
                builder.AddCustomAttributes(typeof(RibbonButton), new ToolboxBrowsableAttribute(true));

                builder.AddCustomAttributes(typeof(RibbonButton), new FeatureAttribute(typeof(RibbonButtonInitializer)));
                #endregion

                builder.AddCustomAttributes(typeof(Ribbon), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(Ribbon), new FeatureAttribute(typeof(RibbonAdornerProvider)));
                builder.AddCustomAttributes(typeof(Ribbon), new FeatureAttribute(typeof(RibbonInitializer)));
                builder.AddCustomAttributes(typeof(Ribbon), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(RibbonTab), new FeatureAttribute(typeof(RibbonTabAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonTab), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                // builder.AddCustomAttributes(typeof(ButtonPanel), new FeatureAttribute(typeof(RibbonButtonPanelAdornerProvider)));

                builder.AddCustomAttributes(typeof(RibbonGallery), new FeatureAttribute(typeof(RibbonGalleryAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonGallery), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(RibbonButton), new FeatureAttribute(typeof(RibbonButtonAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonButton), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                //builder.AddCustomAttributes(typeof(SplitButton), new FeatureAttribute(typeof(RibbonSplitButtonAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonTextBox), new FeatureAttribute(typeof(RibbonTextBoxAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonTextBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(RibbonCheckBox), new FeatureAttribute(typeof(RibbonCheckBoxAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonCheckBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(RibbonRadioButton), new FeatureAttribute(typeof(RibbonRadioButtonAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonRadioButton), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(RibbonComboBox), new FeatureAttribute(typeof(RibboncomboboxAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonComboBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(DropDownButton), new FeatureAttribute(typeof(RibbonDropDownButtonAdornerProvider)));
                builder.AddCustomAttributes(typeof(DropDownButton), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));


                builder.AddCustomAttributes(typeof(RibbonBar), new FeatureAttribute(typeof(RibbonBarAdornerProvider)));
                builder.AddCustomAttributes(typeof(RibbonBar), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(ApplicationMenu), new FeatureAttribute(typeof(AppMenuAdornerProvider)));
                builder.AddCustomAttributes(typeof(ApplicationMenu), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                #endregion

    #region GroupBar

                builder.AddCustomAttributes(typeof(GroupBar), new FeatureAttribute(typeof(GroupBarInitializer)));

                builder.AddCustomAttributes(typeof(GroupBar), new FeatureAttribute(typeof(GroupBarAdornerProvider)));
                builder.AddCustomAttributes(typeof(GroupBar), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(GroupBarItem), new FeatureAttribute(typeof(GroupBarItemAdornerProvider)));
                builder.AddCustomAttributes(typeof(GroupBarItem), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(GroupView), new FeatureAttribute(typeof(GroupViewAdornerProvider)));
                builder.AddCustomAttributes(typeof(GroupView), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(GroupViewItem), new FeatureAttribute(typeof(GroupViewItemAdornerProvider)));
                builder.AddCustomAttributes(typeof(GroupViewItem), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(GroupBarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupBarItemHeader), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GroupBarSplitter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NavigationToolbar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NavigationToolbarItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TextImageControl), new ToolboxBrowsableAttribute(false));

                #endregion

    #region TabControlExt
                builder.AddCustomAttributes(typeof(TabControlExt), new FeatureAttribute(typeof(TabControlExtInitializer)));
                builder.AddCustomAttributes(typeof(TabControlExt), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(TabControlExt), new FeatureAttribute(typeof(TabControlExtAdornerProvider)));
                builder.AddCustomAttributes(typeof(TabItemExt), new FeatureAttribute(typeof(TabItemExtAdornerProvider)));
                builder.AddCustomAttributes(typeof(TabItemExt), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(TabControlExt), "Items", new NewItemTypesAttribute(typeof(TabItemExt)));

                #endregion

    #region Docking
                builder.AddCustomAttributes(typeof(DockingManager), new FeatureAttribute(typeof(DockingInitializer)));
                builder.AddCustomAttributes(typeof(DockingManager), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(DockingManager), new FeatureAttribute(typeof(DockingAdornerProvider)));
                builder.AddCustomAttributes(typeof(DockingManager), "Children", new NewItemTypesAttribute(typeof(Grid)));
                #endregion

    #region Document Container

                builder.AddCustomAttributes(typeof(DocumentContainer), new FeatureAttribute(typeof(DocumentContainerInitializer)));
                builder.AddCustomAttributes(typeof(DocumentContainer), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(DocumentContainer), new FeatureAttribute(typeof(DocumentContainerAdornerProvider)));
                builder.AddCustomAttributes(typeof(DocumentContainer), "Items", new NewItemTypesAttribute(typeof(Grid)));
                #endregion

    #region AutoComplete
                builder.AddCustomAttributes(typeof(AutoComplete), new FeatureAttribute(typeof(AutoCompleteInitializer)));
                builder.AddCustomAttributes(typeof(AutoComplete), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(AutoComplete), new FeatureAttribute(typeof(AutoCompleteAdornerProvider)));
                #endregion

    #region FontListBox
                builder.AddCustomAttributes(typeof(FontListBox), new FeatureAttribute(typeof(FontListBoxInitializer)));
                builder.AddCustomAttributes(typeof(FontListBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(FontListBox), new FeatureAttribute(typeof(FontListBoxAdornerProvider)));
                #endregion

    #region FontListComboBox
                builder.AddCustomAttributes(typeof(FontListComboBox), new FeatureAttribute(typeof(FontListComboBoxInitializer)));
                builder.AddCustomAttributes(typeof(FontListComboBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(FontListComboBox), new FeatureAttribute(typeof(FontListComboBoxAdornerProvider)));
                #endregion



    # region CheckListbox
                builder.AddCustomAttributes(typeof(CheckListBox), new FeatureAttribute(typeof(CheckListBoxInitializer)));
                builder.AddCustomAttributes(typeof(CheckListBox), "Items", new NewItemTypesAttribute(typeof(CheckListBoxItem)));
                builder.AddCustomAttributes(typeof(CheckListBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(CheckListBox), new FeatureAttribute(typeof(CheckListBoxAdornerProvider)));

                #endregion

    #region Gallery
                builder.AddCustomAttributes(typeof(Gallery), new FeatureAttribute(typeof(GalleryInitializer)));
                builder.AddCustomAttributes(typeof(Gallery), new FeatureAttribute(typeof(GalleryAdornerProvider)));
                builder.AddCustomAttributes(typeof(Gallery), "Items", new NewItemTypesAttribute(typeof(GalleryItem)));
                #endregion




    #region TreeViewAdv

                builder.AddCustomAttributes(typeof(TreeViewAdv), new FeatureAttribute(typeof(TreeViewAdvInitializer)));
                builder.AddCustomAttributes(typeof(TreeViewAdv), new FeatureAttribute(typeof(TreeViewAdvAdornerProvider)));
                builder.AddCustomAttributes(typeof(TreeViewAdv), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(TreeViewItemAdv), new FeatureAttribute(typeof(TreeViewItemAdvAdornerProvider)));
                builder.AddCustomAttributes(typeof(TreeViewItemAdv), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));


                #endregion

    #region HierarchyNavigator
                builder.AddCustomAttributes(typeof(HierarchyNavigator), new FeatureAttribute(typeof(HierarchyNavigatorInitializer)));
                builder.AddCustomAttributes(typeof(HierarchyNavigator), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(HierarchyNavigator), new FeatureAttribute(typeof(HierarchyNavigatorAdornerProvider)));
                #endregion

    #region TaskBar

                builder.AddCustomAttributes(typeof(TaskBar), new FeatureAttribute(typeof(TaskBarInitializer)));
                builder.AddCustomAttributes(typeof(TaskBar), new FeatureAttribute(typeof(TaskBarAdornerProvider)));
                builder.AddCustomAttributes(typeof(TaskBar), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(TaskBarItem), new FeatureAttribute(typeof(TaskBarItemAdornerProvider)));
                builder.AddCustomAttributes(typeof(TaskBarItem), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));


                #endregion

    #region RangeSlider
                builder.AddCustomAttributes(typeof(RangeSliderControl), new FeatureAttribute(typeof(RangeSliderInitializer)));
                builder.AddCustomAttributes(typeof(RangeSliderControl), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(RangeSliderControl), new FeatureAttribute(typeof(RangeSliderAdornerProvider)));
                #endregion

              



    #region Wizard
                builder.AddCustomAttributes(typeof(WizardControl), new FeatureAttribute(typeof(WizardControlAdornerProvider)));
                builder.AddCustomAttributes(typeof(WizardControl), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(WizardPageAdornerProvider)));
                builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(WizardPageInitializer)));
                builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                #endregion
                return builder.CreateTable();
            }
        }
    }
#else
    internal class Metadata : IRegisterMetadata {

        // Called by Cider to register any design-time metadata
        public void Register() {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            // tool box filtering
            builder.AddCustomAttributes(typeof(QATCustomizeRibbonDialog),new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TabScrollViewer),new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GalleryFilterSelector), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GalleryGroup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GalleryGroupPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GalleryItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GalleryStackPanel), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(RibbonComboBoxItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ApplicationMenu), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ApplicationMenuGroup), new ToolboxBrowsableAttribute(false));
           // builder.AddCustomAttributes(typeof(BorderEyeDrop), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(BottomThumb), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(BottomLine), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ButtonLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ButtonPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CheckableBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ContextMenuBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ContextTabGroup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CustomContextMenu), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CustomItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CustomMenuItem), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(TwoLinesLayoutPanel), new ToolboxBrowsableAttribute(false));
            
            builder.AddCustomAttributes(typeof(DirectTabPanel), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(DockedElementHost), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DockedElementsContainer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DockedElementTabbedHost), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DockHeaderPresenter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DockPreviewMainButtonVS2005), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CardGroupControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CardViewItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CardViewPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ComboBoxItemAdv), new ToolboxBrowsableAttribute(false));
            
            builder.AddCustomAttributes(typeof(DraggedElementPopup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DropDownButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ExpanderExt), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ExtendedPopup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FloatWindowBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FontListBox), new ToolboxBrowsableAttribute(true));     
            builder.AddCustomAttributes(typeof(FontListBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FontListBoxInternal), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FontListBoxInternalItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TablePickerItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TablePickerUI), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GroupBarItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GroupBarItemHeader), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GroupBarSplitter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GroupHeader), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GroupPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GroupView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GroupViewItem), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(HorizontalLine), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(InRibbonItemsPresenter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(InternalPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ItemsPresenterExt), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LabelTextBlock), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LargeButtonPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MenuButton), new ToolboxBrowsableAttribute(false));
            
            builder.AddCustomAttributes(typeof(MultilinePanel), new ToolboxBrowsableAttribute(false));
           
            builder.AddCustomAttributes(typeof(NavigationToolbar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(NativeFloatWindow), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(OpacityDockPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PopupResizeThumb), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(QuickAccessToolBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(QuickAccessToolBarPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonBar), new ToolboxBrowsableAttribute(false));
            
            builder.AddCustomAttributes(typeof(RibbonButtonChecker), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonCheckBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonComboBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(QuickAccessToolBarPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonBar), new ToolboxBrowsableAttribute(false));
            
            //builder.AddCustomAttributes(typeof(RibbonColorPicker), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonButtonChecker), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonCheckBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonComboBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(QuickAccessToolBarPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonButtonChecker), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonCheckBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonComboBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonGallery), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonGalleryGroup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonGalleryItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonListBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonMenuGroup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonMenuItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonPage), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonRadioButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonSeparator), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonStatusBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonTab), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonTextBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonWindowPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ScreenTip), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SidePanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SimpleMenuButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SplitButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SplitMenuButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Splitter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MiniToolbar), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(Backstage), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonTabItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(BackStageButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(BackStageCommandButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(BackstageTabItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonToggleButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DropDownMenuGroup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DropDownMenuItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonAdorner), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(TabButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TabPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TaskBarItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TaskBarStackPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TextImageControl), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(Syncfusion.Windows.Tools.Controls.TitleBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ToggleButtonExt), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewAdvItemsPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewAdvVirtualizingPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewItemAdv), new ToolboxBrowsableAttribute(false));

            //builder.AddCustomAttributes(typeof(TreeViewItemAdvVirtualizingPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TwoItemsLayoutPanelEx), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VistaSpecificProgressBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(WindowTitleBarButton), new ToolboxBrowsableAttribute(false));
			 builder.AddCustomAttributes(typeof(TreeRootLine), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(WrapPanelExt), new ToolboxBrowsableAttribute(false));
            

            builder.AddCustomAttributes(typeof(SplitterItemBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(WizardNavigationArea), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(WizardPage), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(OneNoteTabBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DocumentContextMenu), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TDILayoutPanel), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(CornerPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CustomWrapPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DocumentPresenter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ItemHeader), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SelectRectangle), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TabPanelAdv), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Office2007TabBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Office2010TabBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VS2008TabBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HeaderPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ScrollingPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TabItemExt), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TabLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(QuickTabSwicthPreviewControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VistaFlipSwitchPreviewControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VS2005SwitchPreviewControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DocumentTabControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DocumentHeader), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MDILayoutPanel), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(TrackContainer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ListSwicthPreviewControl), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(HierarchyNavigatorItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HierarchyNavigatorBarContent), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HierarchyNavigatorDropDownItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HierarchyNavigatorHistoryControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HierarchyNavigatorHistoryListBox), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HierarchyNavigatorItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HierarchyNavigatorModel), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(PagesLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SplitHeaderPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TabSplitterItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TabSplitter), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(TabSplitterItemPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VS2008SplitterItemBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CustomGridSplitter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SplitterPage), new ToolboxBrowsableAttribute(false));


            builder.AddCustomAttributes(typeof(TreeViewColumnHeader), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewHeaderRowPresenter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewRowPresenter), new ToolboxBrowsableAttribute(false));

           

            builder.AddCustomAttributes(typeof(BalloonTipBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(BalloonTipHeader), new ToolboxBrowsableAttribute(false));
          //  builder.AddCustomAttributes(typeof(ColorBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(BalloonTip), new ToolboxBrowsableAttribute(false));


            builder.AddCustomAttributes(typeof(NotifyIcon), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(Office2007SplitterItemBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(OneNoteSplitterItemBorder), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(DocMenuItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CheckListBox), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(CheckListBoxItem), new ToolboxBrowsableAttribute(false));

            //builder.AddCustomAttributes(typeof(ColorPickerDockPanel), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(Hexagon), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(AdornerFloatWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(AdornerWindowsLayoutPanel), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(FackItemsAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ContextAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CutawayAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DraggedAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DraggedObjectShader), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DragMarkerAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FloatWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HostAdornerVS2003), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HostAdornerVS2005), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(InsertionAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MainHost), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MDIWindow), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MoveMDIAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(NewTabLayout), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Panel3DAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(QATCustomizationDialog), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonContextMenu), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SplitterAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TDISplitPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewColumnHeaderAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewItemAdvDragMarkerAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewRowDragMarkerAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewRowDragMarkerTemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewColumnHeaderTemplatedAdornerInternalControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TreeViewItemAdvDragMarkerAdornerrInternalControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(UIElementAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(AutoTemplatedItemsControl), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(TabPreviewAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RibbonAdornerInternalControl), new ToolboxBrowsableAttribute(false));
			builder.AddCustomAttributes(typeof(ItemsCollection), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(CardGroupControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CardViewItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CardViewPanel), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(CustomPanelLabel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CustomPanelTicks), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CustomPopup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ScrollButtonsBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PopupSidePanel), new ToolboxBrowsableAttribute(false));

    //builder.AddCustomAttributes(typeof(PolygonItem), new ToolboxBrowsableAttribute(false));
    //            builder.AddCustomAttributes(typeof(ColorGroup), new ToolboxBrowsableAttribute(false));
    //            builder.AddCustomAttributes(typeof(ColorGroupItem), new ToolboxBrowsableAttribute(false));
    //            builder.AddCustomAttributes(typeof(MoreColorsWindow), new ToolboxBrowsableAttribute(false));	
			
    #region TabSplitter
            builder.AddCustomAttributes(typeof(TabSplitter), new FeatureAttribute(typeof(TabSplitterInitializer)));
            builder.AddCustomAttributes(typeof(TabSplitter), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(TabSplitter), new FeatureAttribute(typeof(TabSplitterAdornerProvider)));
    # endregion

    #region Ribbon

    #region RibbonButton
            builder.AddCustomAttributes( typeof( RibbonButton ), new ToolboxBrowsableAttribute( true ) );

            builder.AddCustomAttributes( typeof( RibbonButton ), new FeatureAttribute( typeof( RibbonButtonInitializer ) ) );
    #endregion

            builder.AddCustomAttributes( typeof( Ribbon ), new ToolboxBrowsableAttribute( true ) );
            builder.AddCustomAttributes( typeof( Ribbon ), new FeatureAttribute( typeof( RibbonAdornerProvider ) ) );
            builder.AddCustomAttributes( typeof( Ribbon ), new FeatureAttribute( typeof( RibbonInitializer ) ) );
            builder.AddCustomAttributes(typeof(Ribbon), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(RibbonTab), new FeatureAttribute(typeof(RibbonTabAdornerProvider)));
            builder.AddCustomAttributes(typeof(RibbonTab), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
           // builder.AddCustomAttributes(typeof(ButtonPanel), new FeatureAttribute(typeof(RibbonButtonPanelAdornerProvider)));

            builder.AddCustomAttributes(typeof(RibbonGallery), new FeatureAttribute(typeof(RibbonGalleryAdornerProvider)));
            builder.AddCustomAttributes(typeof(RibbonGallery), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(RibbonButton), new FeatureAttribute(typeof(RibbonButtonAdornerProvider)));
            builder.AddCustomAttributes(typeof(RibbonButton), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            //builder.AddCustomAttributes(typeof(SplitButton), new FeatureAttribute(typeof(RibbonSplitButtonAdornerProvider)));
            builder.AddCustomAttributes(typeof(RibbonTextBox), new FeatureAttribute(typeof(RibbonTextBoxAdornerProvider)));
            builder.AddCustomAttributes(typeof(RibbonTextBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(RibbonCheckBox), new FeatureAttribute(typeof(RibbonCheckBoxAdornerProvider)));
            builder.AddCustomAttributes(typeof(RibbonCheckBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(RibbonRadioButton), new FeatureAttribute(typeof(RibbonRadioButtonAdornerProvider)));
            builder.AddCustomAttributes(typeof(RibbonRadioButton), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(RibbonComboBox), new FeatureAttribute(typeof(RibboncomboboxAdornerProvider)));
            builder.AddCustomAttributes(typeof(RibbonComboBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(DropDownButton), new FeatureAttribute(typeof(RibbonDropDownButtonAdornerProvider)));
            builder.AddCustomAttributes(typeof(DropDownButton), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));


            builder.AddCustomAttributes(typeof(RibbonBar), new FeatureAttribute(typeof(RibbonBarAdornerProvider)));
            builder.AddCustomAttributes(typeof(RibbonBar), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(ApplicationMenu), new FeatureAttribute(typeof(AppMenuAdornerProvider)));
            builder.AddCustomAttributes(typeof(ApplicationMenu), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
    #endregion

    #region GroupBar
			
			builder.AddCustomAttributes( typeof( GroupBar ), new FeatureAttribute( typeof( GroupBarInitializer ) ) );

			builder.AddCustomAttributes( typeof( GroupBar ), new FeatureAttribute( typeof( GroupBarAdornerProvider ) ) );
            builder.AddCustomAttributes(typeof(GroupBar), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
			builder.AddCustomAttributes( typeof( GroupBarItem ), new FeatureAttribute( typeof( GroupBarItemAdornerProvider ) ) );
            builder.AddCustomAttributes(typeof(GroupBarItem), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
			builder.AddCustomAttributes( typeof( GroupView ), new FeatureAttribute( typeof( GroupViewAdornerProvider ) ) );
            builder.AddCustomAttributes(typeof(GroupView), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
			builder.AddCustomAttributes( typeof( GroupViewItem ), new FeatureAttribute( typeof( GroupViewItemAdornerProvider ) ) );
            builder.AddCustomAttributes(typeof(GroupViewItem), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

			builder.AddCustomAttributes( typeof( GroupBarItem ), new ToolboxBrowsableAttribute( false ) );
			builder.AddCustomAttributes( typeof( GroupView ), new ToolboxBrowsableAttribute( false ) );
			builder.AddCustomAttributes( typeof( GroupViewItem ), new ToolboxBrowsableAttribute( false ) );
			builder.AddCustomAttributes( typeof( GroupBarItemHeader ), new ToolboxBrowsableAttribute( false ) );
			builder.AddCustomAttributes( typeof( GroupBarSplitter ), new ToolboxBrowsableAttribute( false ) );
			builder.AddCustomAttributes( typeof( NavigationToolbar ), new ToolboxBrowsableAttribute( false ) );
			builder.AddCustomAttributes( typeof( NavigationToolbarItem ), new ToolboxBrowsableAttribute( false ) );
			builder.AddCustomAttributes( typeof( TextImageControl ), new ToolboxBrowsableAttribute( false ) );

    #endregion

    #region TabControlExt
            builder.AddCustomAttributes(typeof(TabControlExt), new FeatureAttribute(typeof(TabControlExtInitializer)));
            builder.AddCustomAttributes(typeof(TabControlExt), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(TabControlExt), new FeatureAttribute(typeof(TabControlExtAdornerProvider)));
            builder.AddCustomAttributes(typeof(TabItemExt), new FeatureAttribute(typeof(TabItemExtAdornerProvider)));
            builder.AddCustomAttributes(typeof(TabItemExt), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(TabControlExt), "Items", new NewItemTypesAttribute(typeof(TabItemExt)));

    #endregion

    #region Docking
            builder.AddCustomAttributes(typeof(DockingManager), new FeatureAttribute(typeof(DockingInitializer)));
            builder.AddCustomAttributes(typeof(DockingManager), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(DockingManager), new FeatureAttribute(typeof(DockingAdornerProvider)));
            builder.AddCustomAttributes(typeof(DockingManager), "Children", new NewItemTypesAttribute(typeof(Grid)));
    #endregion

    #region Document Container

            builder.AddCustomAttributes(typeof(DocumentContainer), new FeatureAttribute(typeof(DocumentContainerInitializer)));
            builder.AddCustomAttributes(typeof(DocumentContainer), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(DocumentContainer), new FeatureAttribute(typeof(DocumentContainerAdornerProvider)));
            builder.AddCustomAttributes(typeof(DocumentContainer), "Items", new NewItemTypesAttribute(typeof(Grid)));
    #endregion


    #region AutoComplete
            builder.AddCustomAttributes(typeof(AutoComplete), new FeatureAttribute(typeof(AutoCompleteInitializer)));
            builder.AddCustomAttributes(typeof(AutoComplete), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(AutoComplete), new FeatureAttribute(typeof(AutoCompleteAdornerProvider)));
    #endregion

    #region FontListBox
            builder.AddCustomAttributes(typeof(FontListBox), new FeatureAttribute(typeof(FontListBoxInitializer)));
            builder.AddCustomAttributes(typeof(FontListBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(FontListBox), new FeatureAttribute(typeof(FontListBoxAdornerProvider)));
    #endregion

    #region FontListComboBox
            builder.AddCustomAttributes(typeof(FontListComboBox), new FeatureAttribute(typeof(FontListComboBoxInitializer)));
            builder.AddCustomAttributes(typeof(FontListComboBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(FontListComboBox), new FeatureAttribute(typeof(FontListComboBoxAdornerProvider)));
    #endregion

    # region CheckListbox
            builder.AddCustomAttributes(typeof(CheckListBox), new FeatureAttribute(typeof(CheckListBoxInitializer)));
            builder.AddCustomAttributes(typeof(CheckListBox), "Items", new NewItemTypesAttribute(typeof(CheckListBoxItem)));
            builder.AddCustomAttributes(typeof(CheckListBox), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(CheckListBox), new FeatureAttribute(typeof(CheckListBoxAdornerProvider)));

    #endregion

    #region Gallery
            builder.AddCustomAttributes(typeof(Gallery), new FeatureAttribute(typeof(GalleryInitializer)));
            builder.AddCustomAttributes(typeof(Gallery), new FeatureAttribute(typeof(GalleryAdornerProvider)));
            builder.AddCustomAttributes(typeof(Gallery), "Items", new NewItemTypesAttribute(typeof(GalleryItem)));
    #endregion

    #region TreeViewAdv

            builder.AddCustomAttributes(typeof(TreeViewAdv), new FeatureAttribute(typeof(TreeViewAdvInitializer)));
            builder.AddCustomAttributes(typeof(TreeViewAdv), new FeatureAttribute(typeof(TreeViewAdvAdornerProvider)));
            builder.AddCustomAttributes(typeof(TreeViewAdv), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(TreeViewItemAdv), new FeatureAttribute(typeof(TreeViewItemAdvAdornerProvider)));
            builder.AddCustomAttributes(typeof(TreeViewItemAdv), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));


    #endregion

    #region HierarchyNavigator
            builder.AddCustomAttributes(typeof(HierarchyNavigator), new FeatureAttribute(typeof(HierarchyNavigatorInitializer)));
            builder.AddCustomAttributes(typeof(HierarchyNavigator), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(HierarchyNavigator), new FeatureAttribute(typeof(HierarchyNavigatorAdornerProvider)));
    #endregion

    #region TaskBar

            builder.AddCustomAttributes(typeof(TaskBar), new FeatureAttribute(typeof(TaskBarInitializer)));
            builder.AddCustomAttributes(typeof(TaskBar), new FeatureAttribute(typeof(TaskBarAdornerProvider)));
            builder.AddCustomAttributes(typeof(TaskBar), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(TaskBarItem), new FeatureAttribute(typeof(TaskBarItemAdornerProvider)));
            builder.AddCustomAttributes(typeof(TaskBarItem), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));


    #endregion

    //#region ColorPicker

    //        builder.AddCustomAttributes(typeof(ColorPicker), new FeatureAttribute(typeof(ColorPickerInitializer)));
    //        builder.AddCustomAttributes(typeof(ColorPicker), new FeatureAttribute(typeof(ColorPickerAdornerProvider)));
    //        builder.AddCustomAttributes(typeof(ColorPicker), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));



    //#endregion

    //#region ColorEditor

    //        builder.AddCustomAttributes(typeof(ColorEdit), new FeatureAttribute(typeof(ColorEditorInitializer)));
    //        builder.AddCustomAttributes(typeof(ColorEdit), new FeatureAttribute(typeof(ColorEditorAdornerProvider)));
    //        builder.AddCustomAttributes(typeof(ColorEdit), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

    //#endregion

            builder.AddCustomAttributes(typeof(RangeSliderControl), new FeatureAttribute(typeof(RangeSliderInitializer)));
            builder.AddCustomAttributes(typeof(RangeSliderControl), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(RangeSliderControl), new FeatureAttribute(typeof(RangeSliderAdornerProvider)));

    #region Wizard
            builder.AddCustomAttributes(typeof(WizardControl), new FeatureAttribute(typeof(WizardControlAdornerProvider)));
            builder.AddCustomAttributes(typeof(WizardControl), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(WizardPageAdornerProvider)));
            builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(WizardPageInitializer)));
            builder.AddCustomAttributes(typeof(WizardPage), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

    #endregion
        
            builder.AddCustomAttributes(typeof(BackStageSeparator), new ToolboxBrowsableAttribute(false));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
