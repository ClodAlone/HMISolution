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
using System.Globalization;

namespace Syncfusion.Windows.Controls.PivotGrid.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ResourceWrapperKeys
    {
        #region Constants

        const string PIVOTGRID_CONTEXTMENU_SHOWFIELDLIST = "PivotGrid_ContextMenu_ShowFieldList";

        const string PIVOTGRID_CONTEXTMENU_RELOADDATA = "PivotGrid_ContextMenu_ReloadData";

        const string PIVOTGRID_BUSYINDICATOR = "PivotGrid_BusyIndicator";

        const string PIVOTGRID_BUSYINDICATOR_LOADING = "PivotGrid_BusyIndicator_Loading";

        const string PIVOTGRID_GROUPINGBAR_DROPFILTERFILEDS = "PivotGrid_GroupingBar_DropFilterFileds";

        const string PIVOTGRID_HEADER_GRANDTOTAL = "PivotGrid_Header_GrandTotal";

        const string PIVOTGRID_HEADER_SUBTOTAL = "PivotGrid_Header_SubTotal";

        const string PIVOTSCHEMA_LISTBOX_VALUES = "PivotSchema_ListBox_Values";

        const string PIVOTSCHEMA_BUTTON_UPDATE = "PivotSchema_Button_Update";

        const string PIVOTSCHEMA_CHECKBOX_DEFERUPDATE = "PivotSchema_CheckBox_DeferUpdate";

        const string PIVOTSCHEMA_CHECKBOX_SHOWCALULATION = "PivotSchema_CheckBox_ShowCalulation";

        const string PIVOTSCHEMA_HEADER_ADDREPORT = "PivotSchema_Header_AddReport";

        const string PIVOTSCHEMA_HEADER_DRAGAREA = "PivotSchema_Header_DragArea";

        //const string PIVOTSCHEMA_HEADERPANEL_PROPERTIES = "PivotSchema_ListBox_Values";

        const string PIVOTSCHEMA_LISTBOX_COLUMNLABEL = "PivotSchema_ListBox_ColumnLabel";

        const string PIVOTSCHEMA_LISTBOX_REPORTFILTER = "PivotSchema_ListBox_ReportFilter";

        const string PIVOTSCHEMA_LISTBOX_ROWLABEL = "PivotSchema_ListBox_RowLabel";

        const string PIVOTGRID_MENU_RELOADDATA = "PivotGrid_Menu_ReloadData";

        const string PIVOTGRID_CONTEXTSUBMENU_RIGHT = "PivotGrid_ContextSubMenu_Right";

        const string PIVOTGRID_CONTEXTSUBMENU_LEFT = "PivotGrid_ContextSubMenu_Left";

        const string PIVOTGRID_CONTEXTSUBMENU_BEGINNING = "PivotGrid_ContextSubMenu_Beginning";

        const string PIVOTGRID_CONTEXTSUBMENU_END = "PivotGrid_ContextSubMenu_End";

        const string PIVOTGRID_CONTEXTSUBMENU_ASCENDING = "PivotGrid_ContextSubMenu_Ascending";

        const string PIVOTGRID_CONTEXTSUBMENU_DESCENDING = "PivotGrid_ContextSubMenu_Descending";

        const string PIVOTGRID_CONTEXTMENU_ORDER = "PivotGrid_ContextMenu_Order";

        const string PIVOTGRID_DROPCOLUMNFIELDS = "PivotGrid_DropColumnFields";

        const string PIVOTGRID_DROPROWFIELDS = "PivotGrid_DropRowFields";

        const string PIVOTSCHEMA_HEADER_FIELDLIST = "PivotSchema_Header_PivotTableFieldList";

        const string OK_BUTTONTEXT = "OkButton";

        const string CANCEL_BUTTONTEXT = "CancelButton";

        const string  PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_FIELDNAME= "PivotSchema_ComputationalInfoDialog_Caption_Fieldname";

        const string  PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_DEESCRIPTION= "PivotSchema_ComputationalInfoDialog_Caption_Description";

        const string  PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_SUMMARYTYPE= "PivotSchema_ComputationalInfoDialog_Caption_SummaryType";

        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_CALCTYPE = "PivotSchema_ComputationalInfoDialog_Caption_CalculationType";

        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_BASEFIELD = "PivotSchema_ComputationalInfoDialog_Caption_BaseField";

        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_BASEITEM = "PivotSchema_ComputationalInfoDialog_Caption_BaseItem";

        const string  PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_FORMAT= "PivotSchema_ComputationalInfoDialog_Caption_Format";

        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_FILTERHEADER = "PivotSchema_ComputationalInfoDialog_FilterHeader";

        const string PIVOTSCHEMA_FILTERWINDOW_POPUP_CAPTION_FILTER = "PivotSchema_FilterWindow_Popup_Caption_Filter";

        const string SHOWFIELDLIST_DIALOG_HEADERTEXT = "ShowFieldList_Dialog_HeaderText";

        const string PIVOTGRID_EXPANDER_HEADER = "PivotGrid_Expand_Header";

        const string PIVOTGRID_COLLAPSE_HEADER = "PivotGrid_Collapse_Header";

        const string PIVOTGRID_EXPAND_ALL_HEADER = "PivotGrid_Expand_All_Header";

        const string PIVOTGRID_COLLAPSE_ALL_HEADER = "PivotGrid_Collapse_All_Header";

        const string PIVOTGRID_EXPAND_COLLAPSE_HEADER = "PivotGrid_Expand_Collapse_Header";

        const string PIVOTGRID_EXPAND_GROUP = "PivotGrid_Expand_Group";

        const string PIVOTGRID_COLLAPSE_GROUP = "PivotGrid_Collapse_Group";

        const string PIVOTGRID_TOOLTIP_ROW = "PivotGrid_ToolTip_Row";

        const string PIVOTGRID_TOOLTIP_COLUMN = "PivotGrid_ToolTip_Column";

        const string PIVOTGRID_TOOLTIP_VALUE = "PivotGrid_ToolTip_Value";

        const string PIVOTGRID_TOOLTIP_EMPTY = "PivotGrid_ToolTip_Empty";

      
        const string FILTERPOPUP_ALL = "FilterPopUp_All";
        #endregion

        #region Members

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ContextMenu_ShowFieldList;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_GroupingBar_DropFilterFileds;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_Header_GrandTotal;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_Header_SubTotal;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_Button_Update;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_CheckBox_DeferUpdate;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_CheckBox_ShowCalulation;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_Header_AddReport;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_Header_DragArea;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_HeaderPanel_Properties;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ListBox_ColumnLabel;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ListBox_ReportFilter;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ListBox_RowLabel;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ListBox_Values;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ContextMenu_ReloadData;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_BusyIndicator;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_BusyIndicator_Loading;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ContextSubMenu_Beginning;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ContextSubMenu_Left;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ContextSubMenu_Right;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ContextSubMenu_End;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ContextSubMenu_Ascending;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ContextSubMenu_Descending;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ContextMenu_Order;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_DropRowFields;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_DropColumnFields;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_Header_FieldList;

        /// <summary>
        /// 
        /// </summary>
        private string ok_ButtonText;

        /// <summary>
        /// 
        /// </summary>
        private string cancel_ButtonText;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ComputationalInfoDialog_Caption_Fieldname;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ComputationalInfoDialog_Caption_Description;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ComputationalInfoDialog_Caption_SummaryType;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ComputationalInfoDialog_Caption_CalculationType;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ComputationalInfoDialog_Caption_BaseField;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ComputationalInfoDialog_Caption_BaseItem;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_ComputationalInfoDialog_Caption_Format;

        /// <summary>
        /// 
        /// </summary>
        private string pivotSchema_FilterWindow_Popup_Caption_Filter;

        private string pivotSchema_FilterHeader;

        /// <summary>
        /// 
        /// </summary>
        private string showFieldList_Dialog_HeaderText;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_Expand_Header;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_Collapse_Header;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_Expand_All_Header;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_Collapse_All_Header;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_Expand_Collapse_Header;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ToolTip_Row;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ToolTip_Column;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ToolTip_Value;

        /// <summary>
        /// 
        /// </summary>
        private string pivotGrid_ToolTip_Empty;

        private string pivotValueChooser_Apply;
        private string pivot_Field_Chooser;
        private string show_ValueChooser;
        private string allowSorting;
        private string allowFiltering;
        private string hideColumn;
        private string clearFilters;
        private string clearSorts;

        private string pivotGrid_ExpandGroup;
        private string pivotGrid_CollapseGroup;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceWrapperKeys" /> class.
        /// </summary>
        public ResourceWrapperKeys()
        {
           CultureInfo ci = CultureInfo.CurrentUICulture;

           ok_ButtonText = SR.GetString(ci, OK_BUTTONTEXT);

           cancel_ButtonText = SR.GetString(ci, CANCEL_BUTTONTEXT);

           pivotSchema_ComputationalInfoDialog_Caption_Fieldname = SR.GetString(ci, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_FIELDNAME);

           pivotSchema_ComputationalInfoDialog_Caption_Description = SR.GetString(ci, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_DEESCRIPTION);

           pivotSchema_ComputationalInfoDialog_Caption_BaseField = SR.GetString(ci, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_BASEFIELD);

           pivotSchema_ComputationalInfoDialog_Caption_BaseItem = SR.GetString(ci, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_BASEITEM);

           pivotSchema_ComputationalInfoDialog_Caption_SummaryType = SR.GetString(ci, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_SUMMARYTYPE);

           pivotSchema_ComputationalInfoDialog_Caption_CalculationType = SR.GetString(ci, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_CALCTYPE);

           pivotSchema_ComputationalInfoDialog_Caption_Format = SR.GetString(ci, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_FORMAT);

           pivotSchema_FilterHeader = SR.GetString(ci, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_FILTERHEADER);

           pivotSchema_FilterWindow_Popup_Caption_Filter = SR.GetString(ci, PIVOTSCHEMA_FILTERWINDOW_POPUP_CAPTION_FILTER);

           pivotGrid_ContextMenu_ShowFieldList = SR.GetString(ci, PIVOTGRID_CONTEXTMENU_SHOWFIELDLIST);

           pivotGrid_ContextMenu_ReloadData = SR.GetString(ci, PIVOTGRID_CONTEXTMENU_RELOADDATA);

           pivotGrid_BusyIndicator = SR.GetString(ci, PIVOTGRID_BUSYINDICATOR);

           pivotGrid_BusyIndicator_Loading = SR.GetString(ci, PIVOTGRID_BUSYINDICATOR_LOADING);

           pivotGrid_GroupingBar_DropFilterFileds = SR.GetString(ci, PIVOTGRID_GROUPINGBAR_DROPFILTERFILEDS);
                
           pivotGrid_Header_GrandTotal =SR.GetString(ci, PIVOTGRID_HEADER_GRANDTOTAL);

           pivotGrid_Header_SubTotal =SR.GetString(ci, PIVOTGRID_HEADER_SUBTOTAL);

           pivotSchema_ListBox_Values = SR.GetString(ci, PIVOTSCHEMA_LISTBOX_VALUES);

           pivotSchema_Button_Update  =SR.GetString(ci, PIVOTSCHEMA_BUTTON_UPDATE);

           pivotSchema_CheckBox_DeferUpdate =SR.GetString(ci, PIVOTSCHEMA_CHECKBOX_DEFERUPDATE);

           pivotSchema_CheckBox_ShowCalulation=SR.GetString(ci, PIVOTSCHEMA_CHECKBOX_SHOWCALULATION);

           pivotSchema_Header_AddReport  =SR.GetString(ci, PIVOTSCHEMA_HEADER_ADDREPORT);
            
           pivotSchema_Header_DragArea =SR.GetString(ci, PIVOTSCHEMA_HEADER_DRAGAREA);
                
           //pivotSchema_HeaderPanel_Properties=SR.GetString(ci, PIVOTSCHEMA_HEADERPANEL_PROPERTIES);

           pivotSchema_ListBox_ColumnLabel =SR.GetString(ci, PIVOTSCHEMA_LISTBOX_COLUMNLABEL);
                      
           pivotSchema_ListBox_ReportFilter =SR.GetString(ci, PIVOTSCHEMA_LISTBOX_REPORTFILTER);
            
           pivotSchema_ListBox_RowLabel=SR.GetString(ci, PIVOTSCHEMA_LISTBOX_ROWLABEL);

           pivotGrid_ContextSubMenu_Beginning = SR.GetString(ci, PIVOTGRID_CONTEXTSUBMENU_BEGINNING);

           pivotGrid_ContextSubMenu_End = SR.GetString(ci, PIVOTGRID_CONTEXTSUBMENU_END);

           pivotGrid_ContextSubMenu_Ascending = SR.GetString(ci, PIVOTGRID_CONTEXTSUBMENU_ASCENDING);

           pivotGrid_ContextSubMenu_Descending = SR.GetString(ci, PIVOTGRID_CONTEXTSUBMENU_DESCENDING);

           pivotGrid_ContextSubMenu_Left = SR.GetString(ci, PIVOTGRID_CONTEXTSUBMENU_LEFT);

           pivotGrid_ContextSubMenu_Right = SR.GetString(ci, PIVOTGRID_CONTEXTSUBMENU_RIGHT);

           pivotGrid_ContextMenu_Order = SR.GetString(ci, PIVOTGRID_CONTEXTMENU_ORDER);

           pivotGrid_DropColumnFields = SR.GetString(ci, PIVOTGRID_DROPCOLUMNFIELDS);

           pivotGrid_DropRowFields = SR.GetString(ci, PIVOTGRID_DROPROWFIELDS);

           pivotSchema_Header_FieldList = SR.GetString(ci, PIVOTSCHEMA_HEADER_FIELDLIST);

           showFieldList_Dialog_HeaderText = SR.GetString(ci, SHOWFIELDLIST_DIALOG_HEADERTEXT);

           pivotGrid_Expand_Header = SR.GetString(ci, PIVOTGRID_EXPANDER_HEADER); ;

           pivotGrid_Collapse_Header = SR.GetString(ci, PIVOTGRID_COLLAPSE_HEADER);

           pivotGrid_Expand_All_Header = SR.GetString(ci, PIVOTGRID_EXPAND_ALL_HEADER);

           pivotGrid_Collapse_All_Header = SR.GetString(ci, PIVOTGRID_COLLAPSE_ALL_HEADER);

           pivotGrid_Expand_Collapse_Header = SR.GetString(ci, PIVOTGRID_EXPAND_COLLAPSE_HEADER);

           pivotGrid_ToolTip_Row = SR.GetString(ci, PIVOTGRID_TOOLTIP_ROW);

           pivotGrid_ToolTip_Column = SR.GetString(ci, PIVOTGRID_TOOLTIP_COLUMN);

           pivotGrid_ToolTip_Value = SR.GetString(ci, PIVOTGRID_TOOLTIP_VALUE);

           pivotGrid_ToolTip_Empty = SR.GetString(ci, PIVOTGRID_TOOLTIP_EMPTY);

           
           pivotGrid_ExpandGroup = SR.GetString(ci, PIVOTGRID_EXPAND_GROUP);
           pivotGrid_CollapseGroup = SR.GetString(ci, PIVOTGRID_COLLAPSE_GROUP);

           filterPopUpAll = SR.GetString(ci, FILTERPOPUP_ALL);
        }

        #endregion

        #region Properties

        private string filterPopUpAll;
        /// <summary>
        /// Gets or sets the text "All" in FilterPopup
        /// </summary>
        public string FilterPopUp_All
        {
            get { return filterPopUpAll; }
            set { filterPopUpAll = value; }
        }


        /// <summary>
        /// Gets or set the localized string for "Expand this group" context menu item of row/column header
        /// </summary>
        public string PivotGridExpandGroup
        {
            get { return pivotGrid_ExpandGroup; }
            set { pivotGrid_ExpandGroup = value; }
        }

        /// <summary>
        /// Gets or set the localized string for "Collapse this group" context menu item of row/column header
        /// </summary>
        public string PivotGridCollapseGroup
        {
            get { return pivotGrid_CollapseGroup; }
            set { pivotGrid_CollapseGroup = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for ShowValueChooser context menu item of calculation header.
        /// </summary>
        public string Show_Value_Chooser
        {
            get { return show_ValueChooser; }
            set { show_ValueChooser = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for Clear sorts context menu item of calculation header.
        /// </summary>
        public string Clear_Sorts
        {
            get { return clearSorts; }
            set { clearSorts = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for Clear Filters context menu item of calculation header
        /// </summary>
        public string Clear_Filters
        {
            get { return clearFilters; }
            set { clearFilters = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for Hide Column context menu item of calculation header
        /// </summary>
        public string Hide_Column
        {
            get { return hideColumn; }
            set { hideColumn = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for Allow Filtering context menu item of calculation header
        /// </summary>
        public string Allow_Filtering
        {
            get { return allowFiltering; }
            set { allowFiltering = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for Allow Sorting context menu item of calculation header
        /// </summary>
        public string Allow_Sorting
        {
            get { return allowSorting; }
            set { allowSorting = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for apply button of the PivotValueChooser dialog.
        /// </summary>
        public string PivotValueChooser_Apply
        {
            get { return pivotValueChooser_Apply; }
            set { pivotValueChooser_Apply = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for PivotValueChooser dialog header.
        /// </summary>
        public string PivotFieldChooser
        {
            get { return pivot_Field_Chooser; }
            set { pivot_Field_Chooser = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for show field list dialog header text.
        /// </summary>
        /// <value>The show field list dialog header text.</value>
        public string showFieldListDialogHeaderText
        {
            get { return showFieldList_Dialog_HeaderText; }
            set { showFieldList_Dialog_HeaderText = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid context menu show field list.
        /// </summary>
        /// <value>The pivot grid context menu show field list.</value>
        public string pivotGridContextMenuShowFieldList
        {
            get { return pivotGrid_ContextMenu_ShowFieldList; }
            set { pivotGrid_ContextMenu_ShowFieldList = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid context menu reload data.
        /// </summary>
        /// <value>The pivot grid context menu reload data.</value>
        public string pivotGridContextMenuReloadData
        {
            get { return pivotGrid_ContextMenu_ReloadData; }
            set { pivotGrid_ContextMenu_ReloadData = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid busy indicator header text.
        /// </summary>
        /// <value>The pivot grid busy indicator header text.</value>
        public string pivotGridBusyIndicator
        {
            get { return pivotGrid_BusyIndicator; }
            set { pivotGrid_BusyIndicator = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid busy indicator loading description.
        /// </summary>
        /// <value>The pivot grid busy indicator loading description.</value>
        public string pivotGridBusyIndicatorLoadingDescription
        {
            get { return pivotGrid_BusyIndicator_Loading; }
            set { pivotGrid_BusyIndicator_Loading = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid context sub menu beginning.
        /// </summary>
        /// <value>The pivot grid context sub menu beginning.</value>
        public string pivotGridContextSubMenuBeginning
        {
            get { return pivotGrid_ContextSubMenu_Beginning; }
            set { pivotGrid_ContextSubMenu_Beginning = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid context sub menu left.
        /// </summary>
        /// <value>The pivot grid context sub menu left.</value>
        public string pivotGridContextSubMenuLeft
        {
            get { return pivotGrid_ContextSubMenu_Left; }
            set { pivotGrid_ContextSubMenu_Left = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid context sub menu right.
        /// </summary>
        /// <value>The pivot grid context sub menu right.</value>
        public string pivotGridContextSubMenuRight
        {
            get { return pivotGrid_ContextSubMenu_Right; }
            set { pivotGrid_ContextSubMenu_Right = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid context sub menu end.
        /// </summary>
        /// <value>The pivot grid context sub menu end.</value>
        public string pivotGridContextSubMenuEnd
        {
            get { return pivotGrid_ContextSubMenu_End; }
            set { pivotGrid_ContextSubMenu_End = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid context sub menu ascending.
        /// </summary>
        /// <value>The pivot grid context sub menu ascending.</value>
        public string pivotGridContextSubMenuAscending
        {
            get { return pivotGrid_ContextSubMenu_Ascending; }
            set { pivotGrid_ContextSubMenu_Ascending = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid context sub menu descending.
        /// </summary>
        /// <value>The pivot grid context sub menu descending.</value>
        public string pivotGridContextSubMenuDescending
        {
            get { return pivotGrid_ContextSubMenu_Descending; }
            set { pivotGrid_ContextSubMenu_Descending = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid drop column fields.
        /// </summary>
        /// <value>The pivot grid drop column fields.</value>
        public string pivotGridDropColumnFields
        {
            get { return pivotGrid_DropColumnFields; }
            set { pivotGrid_DropColumnFields = value; }
        }
        /// <summary>
        /// Gets or sets the localized text for pivot grid drop row fields.
        /// </summary>
        /// <value>The pivot grid drop row fields.</value>
        public string pivotGridDropRowFields
        {
            get { return pivotGrid_DropRowFields; }
            set { pivotGrid_DropRowFields = value; }
        }
        /// <summary>
        /// Gets or sets the localized text for pivot grid context menu order.
        /// </summary>
        /// <value>The pivot grid context menu order.</value>
        public string pivotGridContextMenuOrder
        {
            get { return pivotGrid_ContextMenu_Order; }
            set { pivotGrid_ContextMenu_Order = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for pivot grid header drop filter fileds.
        /// </summary>
        /// <value>The pivot grid header drop filter fileds.</value>
        public string pivotGridHeaderDropFilterFileds
        {
            get { return pivotGrid_GroupingBar_DropFilterFileds; }
            set { pivotGrid_GroupingBar_DropFilterFileds = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid header grand total.
        /// </summary>
        /// <value>The pivot grid header grand total.</value>
        public string pivotGridHeaderGrandTotal
        {
            get { return pivotGrid_Header_GrandTotal; }
            set { pivotGrid_Header_GrandTotal = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid header sub total.
        /// </summary>
        /// <value>The pivot grid header sub total.</value>
        public string pivotGridHeaderSubTotal
        {
            get { return pivotGrid_Header_SubTotal; }
            set { pivotGrid_Header_SubTotal = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema button update.
        /// </summary>
        /// <value>The pivot schema button update.</value>
        public string pivotSchemaButtonUpdate
        {
            get { return pivotSchema_Button_Update; }
            set { pivotSchema_Button_Update = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema check box defer update.
        /// </summary>
        /// <value>The pivot schema check box defer update.</value>
        public string pivotSchemaCheckBoxDeferUpdate
        {
            get { return pivotSchema_CheckBox_DeferUpdate; }
            set { pivotSchema_CheckBox_DeferUpdate = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema check box show calculation.
        /// </summary>
        /// <value>The pivot schema check box show calculation.</value>
        public string pivotSchemaCheckBoxShowCalculation
        {
            get { return pivotSchema_CheckBox_ShowCalulation; }
            set { pivotSchema_CheckBox_ShowCalulation = value; }
        }

        /// <summary>
        /// Gets or sets the pivot schema header add report.
        /// </summary>
        /// <value>The pivot schema header add report.</value>
        public string pivotSchemaHeaderAddReport
        {
            get { return pivotSchema_Header_AddReport; }
            set { pivotSchema_Header_AddReport = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema header drag area.
        /// </summary>
        /// <value>The pivot schema header drag area.</value>
        public string pivotSchemaHeaderDragArea
        {
            get { return pivotSchema_Header_DragArea; }
            set { pivotSchema_Header_DragArea = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema header panel properties.
        /// </summary>
        /// <value>The pivot schema header panel properties.</value>
        public string pivotSchemaHeaderPanelProperties
        {
            get { return pivotSchema_HeaderPanel_Properties; }
            set { pivotSchema_HeaderPanel_Properties = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema list box column label.
        /// </summary>
        /// <value>The pivot schema list box column label.</value>
        public string pivotSchemaListBoxColumnLabel
        {
            get { return pivotSchema_ListBox_ColumnLabel; }
            set { pivotSchema_ListBox_ColumnLabel = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema list box row label.
        /// </summary>
        /// <value>The pivot schema list box row label.</value>
        public string pivotSchemaListBoxRowLabel
        {
            get { return pivotSchema_ListBox_RowLabel; }
            set { pivotSchema_ListBox_RowLabel = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema list box report filter.
        /// </summary>
        /// <value>The pivot schema list box report filter.</value>
        public string pivotSchemaListBoxReportFilter
        {
            get { return pivotSchema_ListBox_ReportFilter; }
            set { pivotSchema_ListBox_ReportFilter = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema list box values.
        /// </summary>
        /// <value>The pivot schema list box values.</value>
        public string pivotSchemaListBoxValues
        {
            get { return pivotSchema_ListBox_Values; }
            set { pivotSchema_ListBox_Values = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema header field list.
        /// </summary>
        /// <value>The pivot schema header field list.</value>
        public string pivotSchemaHeaderFieldList
        {
            get { return pivotSchema_Header_FieldList; }
            set { pivotSchema_Header_FieldList = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema filter window popup caption filter.
        /// </summary>
        /// <value>The pivot schema filter window popup caption filter.</value>
        public string pivotSchemaFilterWindowPopupCaptionFilter
        {
            get { return pivotSchema_FilterWindow_Popup_Caption_Filter; }
            set { pivotSchema_FilterWindow_Popup_Caption_Filter = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema computational info dialog caption description.
        /// </summary>
        /// <value>The pivot schema computational info dialog caption description.</value>
        public string pivotSchemaComputationalInfoDialogCaptionDescription
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_Description; }
            set { pivotSchema_ComputationalInfoDialog_Caption_Description = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema computational info dialog caption format.
        /// </summary>
        /// <value>The pivot schema computational info dialog caption format.</value>
        public string pivotSchemaComputationalInfoDialogCaptionFormat
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_Format; }
            set { pivotSchema_ComputationalInfoDialog_Caption_Format = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for type of the pivot schema computational info dialog caption summary.
        /// </summary>
        /// <value>The type of the pivot schema computational info dialog caption summary.</value>
        public string pivotSchemaComputationalInfoDialogCaptionSummaryType
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_SummaryType; }
            set { pivotSchema_ComputationalInfoDialog_Caption_SummaryType = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for type of the pivot schema computational info dialog caption calculation.
        /// </summary>
        /// <value>The type of the pivot schema computational info dialog caption calculation.</value>
        public string pivotSchemaComputationalInfoDialogCaptionCalculationType
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_CalculationType; }
            set { pivotSchema_ComputationalInfoDialog_Caption_CalculationType = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema computational info dialog caption base field.
        /// </summary>
        /// <value>The pivot schema computational info dialog caption base field.</value>
        public string pivotSchemaComputationalInfoDialogCaptionBaseField
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_BaseField; }
            set { pivotSchema_ComputationalInfoDialog_Caption_BaseField = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema computational info dialog caption base item.
        /// </summary>
        /// <value>The pivot schema computational info dialog caption base item.</value>
        public string pivotSchemaComputationalInfoDialogCaptionBaseItem
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_BaseItem; }
            set { pivotSchema_ComputationalInfoDialog_Caption_BaseItem = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot schema computational info dialog caption fieldname.
        /// </summary>
        /// <value>The pivot schema computational info dialog caption fieldname.</value>
        public string pivotSchemaComputationalInfoDialogCaptionFieldname
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_Fieldname; }
            set { pivotSchema_ComputationalInfoDialog_Caption_Fieldname = value; }
        }

        
        /// <summary>
        /// Gets or Sets the localized text for computational info field header
        /// </summary>
        public string PivotSchemaFilterHeader
        {
            get { return pivotSchema_FilterHeader; }
            set { pivotSchema_FilterHeader = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for cancel button.
        /// </summary>
        /// <value>The cancel button.</value>
       public string CancelButton
        {
            get { return cancel_ButtonText; }
            set { cancel_ButtonText = value; }
        }

       /// <summary>
       /// Gets or sets the localized text for ok button.
       /// </summary>
       /// <value>The ok button.</value>
        public string OkButton
        {
            get { return ok_ButtonText; }
            set { ok_ButtonText = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid expand header.
        /// </summary>
        /// <value>The pivot grid expand header.</value>
        public string PivotGridExpandHeader
        {
            get { return pivotGrid_Expand_Header; }
            set { pivotGrid_Expand_Header = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for pivot grid collapse header.
        /// </summary>
        /// <value>The pivot grid collapse header.</value>
        public string PivotGridCollapseHeader
        {
            get { return pivotGrid_Collapse_Header; }
            set { pivotGrid_Collapse_Header = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for pivot grid expand all header.
        /// </summary>
        /// <value>The pivot grid expand all header.</value>
        public string PivotGridExpandAllHeader
        {
            get { return pivotGrid_Expand_All_Header; }
            set { pivotGrid_Expand_All_Header = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for pivot grid collapse all header.
        /// </summary>
        /// <value>The pivot grid collapse all header.</value>
        public string PivotGridCollapseAllHeader
        {
            get { return pivotGrid_Collapse_All_Header; }
            set { pivotGrid_Collapse_All_Header = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for pivot grid expand collapse header.
        /// </summary>
        /// <value>The pivot grid expand collapse header.</value>
        public string PivotGridExpandCollapseHeader
        {
            get { return pivotGrid_Expand_Collapse_Header; }
            set { pivotGrid_Expand_Collapse_Header = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for pivot grid tool tip row.
        /// </summary>
        /// <value>The pivot grid tool tip row.</value>
        public string PivotGridToolTipRow
        {
            get { return pivotGrid_ToolTip_Row; }
            set { pivotGrid_ToolTip_Row = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for pivot grid tool tip column.
        /// </summary>
        /// <value>The pivot grid tool tip column.</value>
        public string PivotGridToolTipColumn
        {
            get { return pivotGrid_ToolTip_Column; }
            set { pivotGrid_ToolTip_Column = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for pivot grid tool tip value.
        /// </summary>
        /// <value>The pivot grid tool tip value.</value>
        public string PivotGridToolTipValue
        {
            get { return pivotGrid_ToolTip_Value; }
            set { pivotGrid_ToolTip_Value = value; }
        }


        /// <summary>
        /// Gets or sets the localized text for empty tool tip.
        /// </summary>
        /// <value>The pivot grid tool tip empty.</value>
        public string PivotGridToolTipEmpty
        {
            get { return pivotGrid_ToolTip_Empty; }
            set { pivotGrid_ToolTip_Empty = value; }
        }
        #endregion
    }
}