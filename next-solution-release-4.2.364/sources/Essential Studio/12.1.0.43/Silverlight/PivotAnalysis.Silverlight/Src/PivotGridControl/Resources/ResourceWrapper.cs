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

namespace Syncfusion.Silverlight.Controls.PivotGrid.Resources
{
    /// <summary>
    /// ResourceWrapper is used to apply the static resource to Silverlight-Components
    /// </summary>
    public sealed class ResourceWrapper
    {
        #region Constant Member
        const string CONTEXT_MOVETOBEGIN = "PivotGrid_ContextMenu_MovetoBegin";
        const string CONTEXT_MOVETOLEFT = "PivotGrid_ContextMenu_MovetoLeft";
        const string CONTEXT_MOVETORIGHT = "PivotGrid_ContextMenu_MovetoRight";
        const string CONTEXT_MOVETOEND = "PivotGrid_ContextMenu_MovetoEnd";
        const string CONTEXT_SMALLTOLARGE = "PivtoGrid_ContextMenu_SmalltoLarge";
        const string CONTEXT_LARGETOSMALL = "PivotGrid_ContextMenu_LargetoSmall";
        const string CONTEXT_ORDER = "PivotGrid_ContextMenu_Order";
        const string GROUPINGBAR_DROPFILTERFIELDS = "PivotGrid_GroupingBar_DropFilterFields";
        const string GROUPINGBAR_DROPCOLUMNFIELDS = "PivotGrid_GroupingBar_DropColumnFields";
        const string GROUPINGBAR_DROPROWFIELDS = "PivotGrid_GroupingBar_DropRowFields";
        const string FILTERPOPUP_BUTTON_OK = "PivotGrid_FilterPopup_Button_OK";
        const string FILTERPOPUP_BUTTON_CANCEL = "PivotGrid_FilterPopup_Button_Cancel";

        const string GROUPINGBAR_BUTTON_SHOWFIELDS = "PivotGrid_GroupingBar_Button_ShowFields";
        const string GROUPINGBAR_TOOLTIP_SHOWFIELS = "PivotGrid_GroupingBar_ToolTip_ShowFields";

        const string HEADER_GRANDTOTAL = "PivotGrid_Header_GrandTotal";
        const string HEADER_SUBTOTAL = "PivotGrid_Header_SubTotal";

        const string TOOLTIP_VALUE = "PivotGrid_ToolTip_Value";
        const string TOOLTIP_COLUMN = "PivotGrid_ToolTip_Column";
        const string TOOLTIP_ROW = "PivotGrid_ToolTip_Row";
        const string TOOLTIP_EMPTY = "PivotGrid_ToolTip_Empty";

        const string CONTEXTMENU_EXPANDCOLLAPSE = "PivotGrid_Header_ContextMenu_ExpandCollapse";
        const string CONTEXTMENU_EXPAND = "PivotGrid_Header_ContextMenu_Expand";
        const string CONTEXTMENU_COLLAPSE = "PivotGrid_Header_ContextMenu_Collapse";
        const string CONTEXTMENU_EXPANDENTIRE = "PivotGrid_Header_ContextMenu_ExpandEntire";
        const string CONTEXTMENU_COLLAPSEENTIRE = "PivotGrid_Header_ContextMenu_CollapseEntire";
        const string PIVOTCOMPUTAIONLIST = "PivotGrid_PivotComputationList";

        const string PIVOTSCHEMA_LISTBOX_VALUES = "PivotSchema_ListBox_Values";
        const string PIVOTSCHEMA_BUTTON_UPDATE = "PivotSchema_Button_Update";
        const string PIVOTSCHEMA_CHECKBOX_DEFERUPDATE = "PivotSchema_CheckBox_DeferUpdate";
        const string PIVOTSCHEMA_CHECKBOX_SHOWCALULATION = "PivotSchema_CheckBox_ShowCalulation";
        const string PIVOTSCHEMA_HEADER_ADDREPORT = "PivotSchema_Header_AddReport";
        const string PIVOTSCHEMA_HEADER_DRAGAREA = "PivotSchema_Header_DragArea";
        const string PIVOTSCHEMA_LISTBOX_COLUMNLABEL = "PivotSchema_ListBox_ColumnLabel";
        const string PIVOTSCHEMA_LISTBOX_REPORTFILTER = "PivotSchema_ListBox_ReportFilter";
        const string PIVOTSCHEMA_LISTBOX_ROWLABEL = "PivotSchema_ListBox_RowLabel";
        const string PIVOTSCHEMA_HEADER_FIELDLIST = "PivotSchema_Header_PivotTableFieldList";
        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_FIELDNAME = "PivotSchema_ComputationalInfoDialog_Caption_Fieldname";
        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_DEESCRIPTION = "PivotSchema_ComputationalInfoDialog_Caption_Description";
        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_SUMMARYTYPE = "PivotSchema_ComputationalInfoDialog_Caption_SummaryType";
        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_FORMAT = "PivotSchema_ComputationalInfoDialog_Caption_Format";
        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_FIELDHEADER = "PivotSchema_ComputationalInfoDialog_FieldHeader";
        const string PIVOTSCHEMA_FILTERWINDOW_POPUP_CAPTION_FILTER = "PivotSchema_FilterWindow_Popup_Caption_Filter";
        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_BASEFIELD = "PivotSchema_ComputationalInfoDialog_Caption_BaseField";
        const string PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_CALCULATIONTYPE = "PivotSchema_ComputationalInfoDialog_Caption_CalculationType";
        const string SHOWFIELDLIST_DIALOG_HEADERTEXT = "ShowFieldList_Dialog_HeaderText";

        const string MOVE_UP = "PivotGrid_FieldList_MoveUp";
        const string MOVE_DOWN = "PivotGrid_FieldList_MoveDown";
        
        #endregion

        #region Private

        private string smalltoLarge;
        private string largetoSmall;
        private string movetoRight;
        private string movetoLeft;
        private string movetoEnd;
        private string movetoBegin;
        private string order;
        private string dropFilterFields;
        private string dropColumnFields;
        private string dropRowFields;
        private string okButtonContent;
        private string cancelButtonContent;
        private string showFields;
        private string toolTip_ShowFields;
        private string grandTotal;
        private string subTotal;
        private string toolTip_Value;
        private string toolTip_Column;
        private string toolTip_Row;
        private string toolTip_Empty;
        private string expandCollapse;
        private string expand;
        private string collapse;
        private string expandEntire;
        private string collapseEntire;
        private string computationList;

        private string pivotSchema_Button_Update;
        private string pivotSchema_CheckBox_DeferUpdate;
        private string pivotSchema_CheckBox_ShowCalulation;
        private string pivotSchema_Header_AddReport;
        private string pivotSchema_Header_DragArea;
        private string pivotSchema_HeaderPanel_Properties;
        private string pivotSchema_ListBox_ColumnLabel;
        private string pivotSchema_ListBox_ReportFilter;
        private string pivotSchema_ListBox_RowLabel;
        private string pivotSchema_ListBox_Values;
        private string pivotSchema_Header_FieldList;
        private string pivotSchema_ComputationalInfoDialog_Caption_Fieldname;
        private string pivotSchema_ComputationalInfoDialog_Caption_Description;
        private string pivotSchema_ComputationalInfoDialog_Caption_SummaryType;
        private string pivotSchema_ComputationalInfoDialog_Caption_Format;
        private string pivotSchema_FilterWindow_Popup_Caption_Filter;
        private string pivotSchema_ComputationalInfoDialog_Caption_CalculationType;
        private string pivotSchema_ComputationalInfoDialog_Caption_BaseField;
        private string pivotSchema_FieldHeader;
        #endregion

        #region Constructor

        public ResourceWrapper()
        {
            CultureInfo cultureInfo = CultureInfo.CurrentUICulture;
            order = SR.GetString(cultureInfo, CONTEXT_ORDER);
            movetoBegin = SR.GetString(cultureInfo, CONTEXT_MOVETOBEGIN);
            movetoEnd = SR.GetString(cultureInfo, CONTEXT_MOVETOEND);
            movetoLeft = SR.GetString(cultureInfo, CONTEXT_MOVETOLEFT);
            movetoRight = SR.GetString(cultureInfo, CONTEXT_MOVETORIGHT);
            smalltoLarge = SR.GetString(cultureInfo, CONTEXT_SMALLTOLARGE);
            largetoSmall = SR.GetString(cultureInfo, CONTEXT_LARGETOSMALL);
            dropFilterFields = SR.GetString(cultureInfo, GROUPINGBAR_DROPFILTERFIELDS);
            dropColumnFields = SR.GetString(cultureInfo, GROUPINGBAR_DROPCOLUMNFIELDS);
            dropRowFields = SR.GetString(cultureInfo, GROUPINGBAR_DROPROWFIELDS);
            cancelButtonContent = SR.GetString(cultureInfo, FILTERPOPUP_BUTTON_CANCEL);
            okButtonContent = SR.GetString(cultureInfo, FILTERPOPUP_BUTTON_OK);

            showFields = SR.GetString(cultureInfo, GROUPINGBAR_BUTTON_SHOWFIELDS);
            toolTip_ShowFields = SR.GetString(cultureInfo, GROUPINGBAR_TOOLTIP_SHOWFIELS);

            grandTotal = SR.GetString(cultureInfo, HEADER_GRANDTOTAL);
            subTotal = SR.GetString(cultureInfo, HEADER_SUBTOTAL);

            toolTip_Column = SR.GetString(cultureInfo, TOOLTIP_COLUMN);
            toolTip_Row = SR.GetString(cultureInfo, TOOLTIP_ROW);
            toolTip_Value = SR.GetString(cultureInfo, TOOLTIP_VALUE);
            toolTip_Empty = SR.GetString(cultureInfo, TOOLTIP_EMPTY);

            expand = SR.GetString(cultureInfo, CONTEXTMENU_EXPAND);
            collapse = SR.GetString(cultureInfo, CONTEXTMENU_COLLAPSE);
            expandCollapse = SR.GetString(cultureInfo, CONTEXTMENU_EXPANDCOLLAPSE);
            expandEntire = SR.GetString(cultureInfo, CONTEXTMENU_EXPANDENTIRE);
            collapseEntire = SR.GetString(cultureInfo, CONTEXTMENU_COLLAPSEENTIRE);

            pivotSchema_ListBox_Values = SR.GetString(cultureInfo, PIVOTSCHEMA_LISTBOX_VALUES);
            pivotSchema_Button_Update = SR.GetString(cultureInfo, PIVOTSCHEMA_BUTTON_UPDATE);
            pivotSchema_CheckBox_DeferUpdate = SR.GetString(cultureInfo, PIVOTSCHEMA_CHECKBOX_DEFERUPDATE);
            pivotSchema_CheckBox_ShowCalulation = SR.GetString(cultureInfo, PIVOTSCHEMA_CHECKBOX_SHOWCALULATION);
            pivotSchema_Header_AddReport = SR.GetString(cultureInfo, PIVOTSCHEMA_HEADER_ADDREPORT);
            pivotSchema_Header_DragArea = SR.GetString(cultureInfo, PIVOTSCHEMA_HEADER_DRAGAREA);
            pivotSchema_ListBox_ColumnLabel = SR.GetString(cultureInfo, PIVOTSCHEMA_LISTBOX_COLUMNLABEL);
            pivotSchema_ListBox_ReportFilter = SR.GetString(cultureInfo, PIVOTSCHEMA_LISTBOX_REPORTFILTER);
            pivotSchema_ListBox_RowLabel = SR.GetString(cultureInfo, PIVOTSCHEMA_LISTBOX_ROWLABEL);
            pivotSchema_Header_FieldList = SR.GetString(cultureInfo, PIVOTSCHEMA_HEADER_FIELDLIST);
            pivotSchema_ComputationalInfoDialog_Caption_Fieldname = SR.GetString(cultureInfo, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_FIELDNAME);
            pivotSchema_ComputationalInfoDialog_Caption_Description = SR.GetString(cultureInfo, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_DEESCRIPTION);
            pivotSchema_ComputationalInfoDialog_Caption_BaseField = SR.GetString(cultureInfo, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_BASEFIELD);
            pivotSchema_ComputationalInfoDialog_Caption_SummaryType = SR.GetString(cultureInfo, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_SUMMARYTYPE);
            pivotSchema_ComputationalInfoDialog_Caption_CalculationType = SR.GetString(cultureInfo, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_CALCULATIONTYPE);
            pivotSchema_ComputationalInfoDialog_Caption_Format = SR.GetString(cultureInfo, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_CAPTION_FORMAT);
            pivotSchema_FieldHeader = SR.GetString(cultureInfo, PIVOTSCHEMA_COMPUTATIONALINFODIALOG_FIELDHEADER);

            moveDown = SR.GetString(cultureInfo, MOVE_DOWN);
            moveUp = SR.GetString(cultureInfo, MOVE_UP);
        }

        #endregion

        #region Public properties

        private string moveUp;

        public string MoveUp
        {
            get { return moveUp; }
            set { moveUp = value; }
        }

        private string moveDown;

        public string MoveDown
        {
            get { return moveDown; }
            set { moveDown = value; }
        }


        /// <summary>
        /// Gets or Sets the ComputationList for localization use.
        /// </summary>
        public string ComputationList
        {
            get { return computationList; }
            set { computationList = value; }
        }

        /// <summary>
        /// Gets or Sets the GrandTotal for localization use.
        /// </summary>
        public string GrandTotal
        {
            get { return grandTotal; }
            set { grandTotal = value; }
        }

        /// <summary>
        /// Gets or Sets the SubTotal for localization use.
        /// </summary>
        public string SubTotal
        {
            get { return subTotal; }
            set { subTotal = value; }
        }

        /// <summary>
        /// Gets or Sets the ToolTip_Value for localization use.
        /// </summary>
        public string ToolTip_Value
        {
            get { return toolTip_Value; }
            set { toolTip_Value = value; }
        }

        /// <summary>
        /// Gets or Sets the ToolTip_Column for localization use.
        /// </summary>
        public string ToolTip_Column
        {
            get { return toolTip_Column; }
            set { toolTip_Column = value; }
        }

        /// <summary>
        /// Gets or Sets the ToolTip_Row for localization use.
        /// </summary>
        public string ToolTip_Row
        {
            get { return toolTip_Row; }
            set { toolTip_Row = value; }
        }

        /// <summary>
        /// Gets or Sets the ToolTip_Empty for localization use.
        /// </summary>
        public string ToolTip_Empty
        {
            get { return toolTip_Empty; }
            set { toolTip_Empty = value; }
        }

        /// <summary>
        /// Gets or Sets the ExpandCollapse for localization use.
        /// </summary>
        public string ExpandCollapse
        {
            get { return expandCollapse; }
            set { expandCollapse = value; }
        }

        /// <summary>
        /// Gets or Sets the Expand for localization use.
        /// </summary>
        public string Expand
        {
            get { return expand; }
            set { expand = value; }
        }

        /// <summary>
        /// Gets or Sets the Collapse for localization use.
        /// </summary>
        public string Collapse
        {
            get { return collapse; }
            set { collapse = value; }
        }

        /// <summary>
        /// Gets or Sets the ExpandEntire for localization use.
        /// </summary>
        public string ExpandEntire
        {
            get { return expandEntire; }
            set { expandEntire = value; }
        }

        /// <summary>
        /// Gets or Sets the CollapseEntire for localization use.
        /// </summary>
        public string CollapseEntire
        {
            get { return collapseEntire; }
            set { collapseEntire = value; }
        }

        /// <summary>
        /// Gets or Sets the ShowFields for localization use.
        /// </summary>
        public string ShowFields
        {
            get { return showFields; }
            set { showFields = value; }
        }

        /// <summary>
        /// Gets or Sets the ToolTip_ShowFields for localization use.
        /// </summary>
        public string ToolTip_ShowFields
        {
            get { return toolTip_ShowFields; }
            set { toolTip_ShowFields = value; }
        }

        /// <summary>
        /// Gets or Sets the CancelButtonContent for localization use.
        /// </summary>
        public string CancelButtonContent
        {
            get { return cancelButtonContent; }
            set { cancelButtonContent = value; }
        }

        /// <summary>
        /// Gets or Sets the OkButtonContent for localization use.
        /// </summary>
        public string OkButtonContent
        {
            get { return okButtonContent; }
            set { okButtonContent = value; }
        }

        /// <summary>
        /// Gets or Sets the DropRowFields for localization use.
        /// </summary>
        public string DropRowFields
        {
            get { return dropRowFields; }
            set { dropRowFields = value; }
        }

        /// <summary>
        /// Gets or Sets the DropColumnFields for localization use.
        /// </summary>
        public string DropColumnFields
        {
            get { return dropColumnFields; }
            set { dropColumnFields = value; }
        }

        /// <summary>
        /// Gets or Sets the DropFilterFields for localization use.
        /// </summary>
        public string DropFilterFields
        {
            get { return dropFilterFields; }
            set { dropFilterFields = value; }
        }

        /// <summary>
        /// Gets or Sets the Order for localization use.
        /// </summary>
        public string Order
        {
            get { return order; }
            set { order = value; }
        }

        /// <summary>
        /// Gets or Sets the MovetoBegin for localization use.
        /// </summary>
        public string MovetoBegin
        {
            get { return movetoBegin; }
            set { movetoBegin = value; }
        }

        /// <summary>
        /// Gets or Sets the MovetoEnd for localization use.
        /// </summary>
        public string MovetoEnd
        {
            get { return movetoEnd; }
            set { movetoEnd = value; }
        }

        /// <summary>
        /// Gets or Sets the MovetoLeft for localization use.
        /// </summary>
        public string MovetoLeft
        {
            get { return movetoLeft; }
            set { movetoLeft = value; }
        }

        /// <summary>
        /// Gets or Sets the MovetoRight for localization use.
        /// </summary>
        public string MovetoRight
        {
            get { return movetoRight; }
            set { movetoRight = value; }
        }

        /// <summary>
        /// Gets or Sets the SmalltoLarge for localization use.
        /// </summary>
        public string SmalltoLarge
        {
            get { return smalltoLarge; }
            set { smalltoLarge = value; }
        }

        /// <summary>
        /// Gets or Sets the LargetoSmall for localization use.
        /// </summary>
        public string LargetoSmall
        {
            get { return largetoSmall; }
            set { largetoSmall = value; }
        }

        #region PivotSchemaDesigner related properties

        /// <summary>
        /// Gets or Sets the Update button Content for localization use.
        /// </summary>
        public string PivotSchemaButtonUpdate
        {
            get { return pivotSchema_Button_Update; }
            set { pivotSchema_Button_Update = value; }
        }

        /// <summary>
        /// Gets or Sets the Defer update Content for localization use.
        /// </summary>
        public string PivotSchemaCheckBoxDeferUpdate
        {
            get { return pivotSchema_CheckBox_DeferUpdate; }
            set { pivotSchema_CheckBox_DeferUpdate = value; }
        }

        /// <summary>
        /// Gets or Sets the ShowCalculation Content for localization use.
        /// </summary>
        public string PivotSchemaCheckBoxShowCalulation
        {
            get { return pivotSchema_CheckBox_ShowCalulation; }
            set { pivotSchema_CheckBox_ShowCalulation = value; }
        }

        /// <summary>
        /// Gets or Sets the AddReport Content for localization use.
        /// </summary>
        public string PivotSchemaHeaderAddReport
        {
            get { return pivotSchema_Header_AddReport; }
            set { pivotSchema_Header_AddReport = value; }
        }

        /// <summary>
        /// Gets or Sets the DragArea Content for localization use.
        /// </summary>
        public string PivotSchemaHeaderDragArea
        {
            get { return pivotSchema_Header_DragArea; }
            set { pivotSchema_Header_DragArea = value; }
        }

        /// <summary>
        /// Gets or Sets the Panel Content for localization use.
        /// </summary>
        public string PivotSchemaHeaderPanelProperties
        {
            get { return pivotSchema_HeaderPanel_Properties; }
            set { pivotSchema_HeaderPanel_Properties = value; }
        }

        /// <summary>
        /// Gets or Sets the Content for Column lable ListBox for localization use.
        /// </summary>
        public string PivotSchemaListBoxColumnLabel
        {
            get { return pivotSchema_ListBox_ColumnLabel; }
            set { pivotSchema_ListBox_ColumnLabel = value; }
        }

        /// <summary>
        /// Gets or Sets the Content for Row lable ListBox for localization use.
        /// </summary>
        public string PivotSchemaListBoxRowLabel
        {
            get { return pivotSchema_ListBox_RowLabel; }
            set { pivotSchema_ListBox_RowLabel = value; }
        }

        /// <summary>
        /// Gets or Sets the Content for ReportFilter ListBox for localization use.
        /// </summary>
        public string PivotSchemaListBoxReportFilter
        {
            get { return pivotSchema_ListBox_ReportFilter; }
            set { pivotSchema_ListBox_ReportFilter = value; }
        }

        /// <summary>
        /// Gets or Sets the Content for Column Values ListBox for localization use.
        /// </summary>
        public string PivotSchemaListBoxValues
        {
            get { return pivotSchema_ListBox_Values; }
            set { pivotSchema_ListBox_Values = value; }
        }

        /// <summary>
        /// Gets or Sets the Content for FieldList ListBox  for localization use.
        /// </summary>
        public string PivotSchemaHeaderFieldList
        {
            get { return pivotSchema_Header_FieldList; }
            set { pivotSchema_Header_FieldList = value; }
        }

        /// <summary>
        /// Gets or Sets the WindowPopupCaptionFilter for localization use.
        /// </summary>
        public string PivotSchemaFilterWindowPopupCaptionFilter
        {
            get { return pivotSchema_FilterWindow_Popup_Caption_Filter; }
            set { pivotSchema_FilterWindow_Popup_Caption_Filter = value; }
        }

        /// <summary>
        /// Gets or Sets the DialogCaptionDescription for localization use.
        /// </summary>
        public string PivotSchemaComputationalInfoDialogCaptionDescription
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_Description; }
            set { pivotSchema_ComputationalInfoDialog_Caption_Description = value; }
        }

        /// <summary>
        /// Gets or Sets the DialogCaptionFormat for localization use.
        /// </summary>
        public string PivotSchemaComputationalInfoDialogCaptionFormat
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_Format; }
            set { pivotSchema_ComputationalInfoDialog_Caption_Format = value; }
        }

        /// <summary>
        /// Gets or Sets the CaptionSummaryType for localization use.
        /// </summary>
        public string PivotSchemaComputationalInfoDialogCaptionSummaryType
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_SummaryType; }
            set { pivotSchema_ComputationalInfoDialog_Caption_SummaryType = value; }
        }

        /// <summary>
        /// Gets or Sets the CaptionFieldname for localization use.
        /// </summary>
        public string PivotSchemaComputationalInfoDialogCaptionFieldname
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_Fieldname; }
            set { pivotSchema_ComputationalInfoDialog_Caption_Fieldname = value; }
        }

        /// <summary>
        /// Gets or Sets the CaptionCalculationType for localization use.
        /// </summary>
        public string PivotSchemaComputationalInfoDialogCaptionCalculationType
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_CalculationType; }
            set { pivotSchema_ComputationalInfoDialog_Caption_CalculationType = value; }
        }
        
        /// <summary>
        /// Gets or Sets the CaptionBaseField for localization use.
        /// </summary>
        public string PivotSchemaComputationalInfoDialogCaptionBaseField
        {
            get { return pivotSchema_ComputationalInfoDialog_Caption_BaseField; }
            set { pivotSchema_ComputationalInfoDialog_Caption_BaseField = value; }
        }

        /// <summary>
        /// Gets or Sets the localized string for computational info field header
        /// </summary>
        public string PivotSchemaFieldHeader
        {
            get { return pivotSchema_FieldHeader; }
            set { pivotSchema_FieldHeader = value; }
        }


        /// <summary>
        /// Gets or Sets the PivotTableFieldList for localization use.
        /// </summary>
        public string PivotSchemaHeaderPivotTableFieldList
        {
            get { return pivotSchema_Header_FieldList; }
            set { pivotSchema_Header_FieldList = value; }
        }

        #endregion PivotSchemaDesigner related properties
        #endregion Public properties
    }
}
