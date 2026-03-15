#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls.Resources
{
    public class ResourceWrapper
    {
        const string CloseButtonTooltipTextName = "CloseButtonTooltipText";
        const string AwlButtonTooltipTextName = "AwlButtonTooltipText";
        const string ContextMenuButtonTooltipTextName = "ContextMenuButtonTooltipText";
        const string MaximizeButtonTooltipTextName = "MaximizeButtonTooltipText";
        const string MinimizeButtonTooltipTextName = "MinimizeButtonTooltipText";
        const string RestoreButtonTooltipTextName = "RestoreButtonTooltipText";
        const string FloatButtonTooltipTextName = "FloatButtonTooltipText";
        const string CustomizeQATName = "CustomizeQAT";
        const string CustomizeQATHeaderName = "CustomizeQATHeader";
        const string QATMoreCommandsName = "QATMoreCommands";
        const string QATShowBelowName = "QATShowBelow";
        const string QATShowAboveName = "QATShowAbove";
        const string MinimizeRibbonName = "MinimizeRibbon";
        const string MaximizeRibbonName = "MaximizeRibbon";
        const string AddToQATName = "AddToQAT";
        const string CustomizeQATContextMenuName = "CustomizeQATContextMenu";
        const string ShowQATBelowName = "ShowQATBelow";
        const string AddItemName = "AddItem";
        const string RemoveItemName = "RemoveItem";
        const string ResetName = "Reset";
        const string ModifyName = "Modify";
        const string ChooseName = "Choose";
        const string QATName = "QAT";
        const string RemoveFromQATName = "RemoveFromQAT";
        const string AccessCalendarTextName = "AccessCalendarText";
        const string AccessWatchTextName = "AccessWatchText";
        const string AccessEmptyDateTextName = "AccessEmptyDateText";
        const string AccessTodayTextName = "AccessTodayText";
        const string DockableName = "Dockable";
        const string TabbedName = "Tabbed";
        const string FloatingName = "Floating";
        const string AutoHideName = "AutoHide";
        const string HideName = "Hide";
        const string MaximizeName = "Maximize";
        const string MinimizeName = "Minimize";
        const string RestoreName = "Restore";
        const string DocumentName = "Document";
        const string MoveToNextTabGroupName = "MoveToNextTabGroup";
        const string NewVerticalTabGroupName = "NewVerticalTabGroup";
        const string MoveToPreviousTabGroupName= "MoveToPreviousTabGroup";
        const string NewHorizontalTabGroupName = "NewHorizontalTabGroup";
        const string TabCloseName = "TabClose";
        const string CloseAllButThisName = "CloseAllButThis";
        const string TabCloseAllName = "TabCloseAll";
        const string MDIRestoreName = "MDIRestore";
        const string MDIMoveName = "MDIMove";
        const string MDIResizeName = "MDIResize";
        const string MDIMinimizeName = "MDIMinimize";
        const string MDIMaximizeName = "MDIMaximize";
        const string MDICloseName = "MDIClose";
        const string MDIFloatingName = "MDIFloating";
        const string MDIDocumentName = "MDIDocument";
        const string MDIDockableName = "MDIDockable";
        const string MinimizeTooltipName = "MinimizeTooltip";
        const string FullScreenTooltipName = "FullScreenTooltip";
        const string MaximizeTooltipName = "MaximizeTooltip";
        const string CloseTooltipName = "CloseTooltip";
        const string RestoreTooltipName = "RestoreTooltip";
        const string ShowmorebuttonsName = "Showmorebuttons";
        const string ShowfewerbuttonsName = "Showfewerbuttons";
        const string OptionsName = "Options";
        const string ButtonsName = "Buttons";
        const string GroupbarMItemCutName = "GroupbarMItemCut";
        const string GroupbarMItemCopyName = "GroupbarMItemCopy";
        const string GroupbarMItemPasteName = "GroupbarMItemPaste";
        const string GroupbarMItemListViewName= "GroupbarMItemListView";
        const string GroupbarMItemSortAscName = "GroupbarMItemSortAsc";
        const string GroupbarMItemSortDecName = "GroupbarMItemSortDec";
        const string GroupbarMItemAddTabName = "GroupbarMItemAddTab";
        const string GroupbarMItemDeleteTabName = "GroupbarMItemDeleteTab";
        const string GroupbarMItemRenameTabName = "GroupbarMItemRenameTab";
        const string GroupbarMItemAddItemName = "GroupbarMItemAddItem";
        const string GroupbarMItemRenameItemName = "GroupbarMItemRenameItem";
        const string GroupbarMItemDeleteItemName = "GroupbarMItemDeleteItem";
        const string GroupbarMItemMoveUpName = "GroupbarMItemMoveUp";
        const string GroupbarMItemMoveDownName = "GroupbarMItemMoveDown";
        const string CancelTextName = "CancelText";
        const string NextTextName = "NextText";
        const string BackTextName = "BackText";
        const string FinishTextName = "FinishText";
        const string HelpTextName = "HelpText";
        const string OkName = "Ok";
        const string CancelName = "Cancel";
        const string QATResetTitleName="QATResetTitle";
        const string QATResetContentName = "QATResetContent";
        const string QATDuplicateAlertName = "QATDuplicateAlert";
        const string QATTabCaptionName = "QATTabCaption";
        const string QATRibbonMenuCaptionName = "QATRibbonMenuCaption";
        const string QATAllCommandsCaptionName = "QATAllCommandsCaption";
        const string MenuItemCancelName = "MenuItemCancel";
        const string NewTabgroupName = "NewTabgroup";
        const string BackStageButtonHeaderName = "BackStageButtonHeader";
        const string ApplicationMenuButtonHeaderName = "ApplicationMenuButtonHeader";

        //Customize Ribbon
        const string CustomizeRibbonName = "CustomizeRibbon";
        const string CustomizeRibbonLabelName = "CustomizeRibbonLabel";
        const string NewTabName = "NewTab";
        const string NewBarName = "NewBar";
        const string RenameItemName = "RenameItem";
        const string DeleteItemName = "DeleteItem";
        const string CustomizationLabelName = "CustomizationLabel";
        const string ResetSelectedTabName = "ResetSelectedTab";
        const string ResetAllName = "ResetAll";  


        const string LoadOnDemandHeaderName = "LoadOnDemandHeader";
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceWrapper"/> class.
        /// </summary>
        public ResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;            

             CloseButtonTooltipText = SR.GetString(ci, CloseButtonTooltipTextName);
             AwlButtonTooltipText = SR.GetString(ci, AwlButtonTooltipTextName);
             ContextMenuButtonTooltipText = SR.GetString(ci, ContextMenuButtonTooltipTextName);
             MaximizeButtonTooltipText = SR.GetString(ci, MaximizeButtonTooltipTextName);
             MinimizeButtonTooltipText = SR.GetString(ci, MinimizeButtonTooltipTextName);
             RestoreButtonTooltipText = SR.GetString(ci, RestoreButtonTooltipTextName);
             FloatButtonTooltipText = SR.GetString(ci, FloatButtonTooltipTextName);
             CustomizeQAT = SR.GetString(ci, CustomizeQATName);
             CustomizeQATHeader = SR.GetString(ci, CustomizeQATHeaderName);
             QATMoreCommands = SR.GetString(ci, QATMoreCommandsName);
             QATShowBelow = SR.GetString(ci, QATShowBelowName);
             QATShowAbove = SR.GetString(ci, QATShowAboveName);
             MinimizeRibbon = SR.GetString(ci, MinimizeRibbonName);
             MaximizeRibbon = SR.GetString(ci, MaximizeRibbonName);
             AddToQAT = SR.GetString(ci, AddToQATName);
             CustomizeQATContextMenu = SR.GetString(ci, CustomizeQATContextMenuName);
             ShowQATBelow = SR.GetString(ci, ShowQATBelowName);
             AddItem = SR.GetString(ci, AddItemName);
             RemoveItem = SR.GetString(ci, RemoveItemName);
             Reset = SR.GetString(ci, ResetName);
             Modify = SR.GetString(ci, ModifyName);
             Choose = SR.GetString(ci, ChooseName);
             QAT = SR.GetString(ci, QATName);
             RemoveFromQAT = SR.GetString(ci, RemoveFromQATName);
             AccessCalendarText = SR.GetString(ci, AccessCalendarTextName);
             AccessWatchText = SR.GetString(ci, AccessWatchTextName);
             AccessEmptyDateText = SR.GetString(ci, AccessEmptyDateTextName);
             AccessTodayText = SR.GetString(ci, AccessTodayTextName);
             Dockable = SR.GetString(ci, DockableName);
             Tabbed = SR.GetString(ci, TabbedName);
             Floating = SR.GetString(ci, FloatingName);
             AutoHide = SR.GetString(ci, AutoHideName);
             Hide = SR.GetString(ci, HideName);
             Maximize = SR.GetString(ci, MaximizeName);
             Minimize = SR.GetString(ci, MinimizeName);
             Restore = SR.GetString(ci, RestoreName);
             Document = SR.GetString(ci, DocumentName);
             MoveToNextTabGroup = SR.GetString(ci, MoveToNextTabGroupName);
             NewVerticalTabGroup = SR.GetString(ci, NewVerticalTabGroupName);
             MoveToPreviousTabGroup = SR.GetString(ci, MoveToPreviousTabGroupName);
             NewHorizontalTabGroup = SR.GetString(ci, NewHorizontalTabGroupName);
             TabClose = SR.GetString(ci, TabCloseName);
             CloseAllButThis = SR.GetString(ci, CloseAllButThisName);
             TabCloseAll = SR.GetString(ci, TabCloseAllName);
             MDIRestore = SR.GetString(ci, MDIRestoreName);
             MDIMove = SR.GetString(ci, MDIMoveName);
             MDIResize = SR.GetString(ci, MDIResizeName);
             MDIMinimize = SR.GetString(ci, MDIMinimizeName);
             MDIMaximize = SR.GetString(ci, MDIMaximizeName);
             MDIClose = SR.GetString(ci, MDICloseName);
             MDIFloating = SR.GetString(ci, MDIFloatingName);
             MDIDocument = SR.GetString(ci, MDIDocumentName);
             MDIDockable = SR.GetString(ci, MDIDockableName);
             MinimizeTooltip = SR.GetString(ci, MinimizeTooltipName);
             MaximizeTooltip = SR.GetString(ci, MaximizeTooltipName);
             FullScreenTooltip = SR.GetString(ci, FullScreenTooltipName);
             CloseTooltip = SR.GetString(ci, CloseTooltipName);
             RestoreTooltip = SR.GetString(ci, RestoreTooltipName);
             Showmorebuttons = SR.GetString(ci, ShowmorebuttonsName);
             Showfewerbuttons = SR.GetString(ci, ShowfewerbuttonsName);
             Options = SR.GetString(ci, OptionsName);
             Buttons = SR.GetString(ci, ButtonsName);
             GroupbarMItemCut = SR.GetString(ci, GroupbarMItemCutName);
             GroupbarMItemCopy = SR.GetString(ci, GroupbarMItemCopyName);
             GroupbarMItemPaste = SR.GetString(ci, GroupbarMItemPasteName);
             GroupbarMItemListView = SR.GetString(ci, GroupbarMItemListViewName);
             GroupbarMItemSortAsc = SR.GetString(ci, GroupbarMItemSortAscName);
             GroupbarMItemSortDec = SR.GetString(ci, GroupbarMItemSortDecName);
             GroupbarMItemAddTab = SR.GetString(ci, GroupbarMItemAddTabName);
             GroupbarMItemDeleteTab = SR.GetString(ci, GroupbarMItemDeleteTabName);
             GroupbarMItemRenameTab = SR.GetString(ci, GroupbarMItemRenameTabName);
             GroupbarMItemAddItem = SR.GetString(ci, GroupbarMItemAddItemName);
             GroupbarMItemRenameItem = SR.GetString(ci, GroupbarMItemRenameItemName);
             GroupbarMItemDeleteItem = SR.GetString(ci, GroupbarMItemDeleteItemName);
             GroupbarMItemMoveUp = SR.GetString(ci, GroupbarMItemMoveUpName);
             GroupbarMItemMoveDown = SR.GetString(ci, GroupbarMItemMoveDownName);
             CancelText = SR.GetString(ci, CancelTextName);
             NextText = SR.GetString(ci, NextTextName);
             BackText = SR.GetString(ci, BackTextName);
             FinishText = SR.GetString(ci, FinishTextName);
             HelpText = SR.GetString(ci, HelpTextName);
             Ok = SR.GetString(ci, OkName);
             Cancel = SR.GetString(ci, CancelName);
             QATResetTitle = SR.GetString(ci, QATResetTitleName);
             QATResetContent = SR.GetString(ci, QATResetContentName);
             QATDuplicateAlert = SR.GetString(ci, QATDuplicateAlertName);
             QATTabCaption = SR.GetString(ci, QATTabCaptionName);
             QATRibbonMenuCaption = SR.GetString(ci, QATRibbonMenuCaptionName);
             QATAllCommandsCaption = SR.GetString(ci, QATAllCommandsCaptionName);
             MenuItemCancel = SR.GetString(ci, MenuItemCancelName);
             NewTabgroup = SR.GetString(ci, NewTabgroupName);
             BackStageButtonHeader = SR.GetString(ci, BackStageButtonHeaderName);
             ApplicationMenuButtonHeader = SR.GetString(ci, ApplicationMenuButtonHeaderName);
             LoadOnDemandHeader = SR.GetString(ci, LoadOnDemandHeaderName);

             CustomizeRibbon = SR.GetString(ci, CustomizeRibbonName);
             CustomizeRibbonLabel = SR.GetString(ci, CustomizeRibbonLabelName);
             NewTab = SR.GetString(ci, NewTabName);
             NewBar = SR.GetString(ci, NewBarName);
             RenameItem = SR.GetString(ci, RenameItemName);
             DeleteItem = SR.GetString(ci, DeleteItemName);
             CustomizationLabel = SR.GetString(ci, CustomizationLabelName);
             ResetSelectedTab = SR.GetString(ci, ResetSelectedTabName);
             ResetAll = SR.GetString(ci, ResetAllName);
          
        }

        public string CloseButtonTooltipText { get; set; }

        public string AwlButtonTooltipText { get; set; }

        public string ContextMenuButtonTooltipText { get; set; }

        public string MaximizeButtonTooltipText { get; set; }

        public string MinimizeButtonTooltipText { get; set; }

        public string RestoreButtonTooltipText { get; set; }

        public string FloatButtonTooltipText { get; set; }

        public string CustomizeQAT { get; set; }

        public string CustomizeQATHeader { get; set; }

        public string QATMoreCommands { get; set; }

        public string QATShowBelow { get; set; }

        public string QATShowAbove { get; set; }

        public string MinimizeRibbon { get; set; }

        public string MaximizeRibbon { get; set; }

        public string AddToQAT { get; set; }

        public string CustomizeQATContextMenu { get; set; }

        public string ShowQATBelow { get; set; }

        public string AddItem { get; set; }

        public string RemoveItem { get; set; }

        public string Reset { get; set; }

        public string Modify { get; set; }

        public string Choose { get; set; }

        public string QAT { get; set; }

        public string RemoveFromQAT { get; set; }

        public string AccessCalendarText { get; set; }

        public string AccessWatchText { get; set; }

        public string AccessEmptyDateText { get; set; }

        public string AccessTodayText { get; set; }

        public string Dockable { get; set; }

        public string Tabbed { get; set; }

        public string Floating { get; set; }

        public string AutoHide { get; set; }

        public string Hide { get; set; }

        public string Maximize { get; set; }

        public string Minimize { get; set; }

        public string Restore { get; set; }

        public string Document { get; set; }

        public string MoveToNextTabGroup { get; set; }

        public string NewVerticalTabGroup { get; set; }

        public string MoveToPreviousTabGroup { get; set; }

        public string NewHorizontalTabGroup { get; set; }

        public string TabClose { get; set; }

        public string CloseAllButThis { get; set; }

        public string TabCloseAll { get; set; }

        public string MDIRestore { get; set; }

        public string MDIMove { get; set; }

        public string MDIResize { get; set; }

        public string MDIMinimize { get; set; }

        public string MDIMaximize { get; set; }

        public string MDIClose { get; set; }

        public string MDIFloating { get; set; }

        public string MDIDocument { get; set; }

        public string MDIDockable { get; set; }

        public string MinimizeTooltip { get; set; }

        public string MaximizeTooltip { get; set; }

        public string FullScreenTooltip { get; set; }

        public string CloseTooltip { get; set; }

        public string RestoreTooltip { get; set; }

        public string Showmorebuttons { get; set; }

        public string Showfewerbuttons { get; set; }

        public string Options { get; set; }

        public string Buttons { get; set; }

        public string GroupbarMItemCut { get; set; }

        public string GroupbarMItemCopy { get; set; }

        public string GroupbarMItemPaste { get; set; }

        public string GroupbarMItemListView { get; set; }

        public string GroupbarMItemSortAsc { get; set; }

        public string GroupbarMItemSortDec { get; set; }

        public string GroupbarMItemAddTab { get; set; }

        public string GroupbarMItemDeleteTab { get; set; }

        public string GroupbarMItemRenameTab { get; set; }

        public string GroupbarMItemAddItem { get; set; }

        public string GroupbarMItemRenameItem { get; set; }

        public string GroupbarMItemDeleteItem { get; set; }

        public string GroupbarMItemMoveUp { get; set; }

        public string GroupbarMItemMoveDown { get; set; }

        public string CancelText { get; set; }

        public string NextText { get; set; }

        public string BackText { get; set; }

        public string FinishText { get; set; }

        public string HelpText { get; set; }

        public string Ok { get; set; }

        public string  Cancel { get; set; }

        public string QATResetTitle { get; set; }

        public string QATResetContent { get; set; }

        public string QATDuplicateAlert { get; set; }

        public string QATTabCaption { get; set; }

        public string QATAllCommandsCaption { get; set; }

        public string  QATRibbonMenuCaption { get; set; }

        public string MenuItemCancel { get; set; }

        public string NewTabgroup { get; set; }

        public string BackStageButtonHeader { get; set; }
        public string ApplicationMenuButtonHeader { get; set; }

        public string LoadOnDemandHeader { get; set; }

        public string CustomizeRibbon { get; set; }
        public string CustomizeRibbonLabel { get; set; }
        public string NewTab { get; set; }
        public string NewBar { get; set; }
        public string RenameItem { get; set; }
        public string DeleteItem { get; set; }
        public string CustomizationLabel { get; set; }
        public string ResetSelectedTab { get; set; }
        public string ResetAll { get; set; }
    }
}

