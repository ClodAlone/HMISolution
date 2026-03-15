#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.XlsIO.Interfaces.PivotTables;
using Syncfusion.XlsIO.Parser.Biff_Records.PivotTable;
namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Describes the settings of the pivot table
    /// </summary>
    public class PivotTableOptions : IPivotTableOptions
    {

        #region Members
        /// <summary>
        /// This property specifies the layout setting
        /// </summary>
        PivotTableRowLayout m_rowLayout;
        /// <summary>
        /// Parent of this settings
        /// </summary>
        PivotTableImpl m_pivotTable;
        /// <summary>
        /// Pivot table extented options
        /// </summary>
        private ViewExtendedInfoRecord m_extInfo;
        /// <summary>
        /// Pivot Table extented  Defintion info
        /// </summary>
        private PivotViewDefinitionRecord m_DefinitionInfo;
        /// <summary>
        /// Specifies a boolean value that indicates whether an asterisks should be displayed in
        ///subtotals and totals
        /// </summary>
        private bool m_bShowAsteriskTotals;
        /// <summary>
        /// Specifies the string to be displayed in column header in compact mode
        /// </summary>
        private string m_columnHeaderCaption;
        /// <summary>
        /// Specifies the string to be displayed in Row header in compact mode
        /// </summary>
        private string m_rowHeaderCaption;
        /// <summary>
        /// Specifies a boolean value that indicates whether new fields should have their compact
        ///flag set to true.
        /// </summary>
        private bool m_bIsCompactNewField;
        /// <summary>
        /// Specifies a boolean value that indicates whether the field next to the data field in the
        ///PivotTable should be displayed in the same column of the spreadsheet
        /// </summary>
        private bool m_bIsCompactData;
        /// <summary>
        /// Specifies the version of the application that created the cache
        /// </summary>
        private byte m_btCreatedVersion;
        /// <summary>
        /// Specifies the version of the application that updated the cache
        /// </summary>
        private byte m_btUpdatedVersion;
        /// <summary>
        /// Specifies the mini version of the application that updated the cache
        /// </summary>
        private byte m_btMiniRefreshVersion;
        /// <summary>
        /// Specifies a boolean value that indicates whether the "custom lists" option is offered
        ///when sorting this PivotTable
        /// </summary>
        private bool m_bShowCustomSortList;
        /// <summary>
        /// Specifies the name of the value area field header in the PivotTable. This caption is shown
        ///when the PivotTable when two or more fields are in the values area.
        /// </summary>
        private string m_dataCaption;
        ///// <summary>
        ///// Specifies a boolean value that indicates whether the field representing multiple fields in
        /////the data region is located in the row area or the column area.
        ///// </summary>
        //private bool m_bIsDataOnRows;
        
        /// <summary>
        /// Specifies a boolean value that indicates whether the user is allowed to edit the cells in
        ///the data area of the PivotTable.
        /// </summary>
        private bool m_bIsDataEditable;
        /// <summary>
        /// Specifies a boolean value that indicates whether fields in the PivotTable are sorted in
        ///non-default order in the field list.
        /// </summary>
        private bool m_bIsDefaultSortOrder;
        /// <summary>
        /// Specifies a boolean value that indicates whether the user is prevented from displaying
        ///PivotField properties.
        /// </summary>
        private bool m_bEnableFieldProperties;
        /// <summary>
        /// indicates whether fields in the PivotTable are sorted in
        ///non-default order in the field list.
        /// </summary>
        private bool m_bIsDefaultAutoSort;
        /// <summary>
        /// Specifies a boolean value that indicates whether calculated members should be shown in
        ///the PivotTable view.
        /// </summary>
        private bool m_bShowCalcMembers;
        /// <summary>
        /// Specifies the indentation increment for compact axis and can be used to set the Report
        /// Layout to Compact Form.
        /// </summary>
        private uint m_iIndent;
        /// <summary>
        /// Specifies a boolean value that indicates whether new fields should have their outline
        /// flag set to true.
        /// </summary>
        private bool m_bOutline;
        /// <summary>
        /// Specifies a boolean value that indicates whether data fields in the PivotTable should be
        /// displayed in outline form.
        /// </summary>
        private bool m_bOutlineData;
        /// <summary>
        /// Specifies a boolean value that indicates whether the fields of a PivotTable can have
        /// multiple filters set on them.
        /// </summary>
        private bool m_bMultiFieldFilter;
        /// <summary>
        /// Specifies a boolean value that indicates whether the in-grid drop zones should be
        ///displayed at runtime, and whether classic layout is applied.
        /// </summary>
        private bool m_bShowGridDropZone;
        /// <summary>
        /// Returns or sets the order in which page fields 
        /// are added to the PivotTable report’s layout
        /// </summary>
        private PivotPageAreaFieldsOrder m_pageFieldsOrder;
        /// <summary>
        /// True if formatting is preserved when the report is refreshed or recalculated by 
        /// operations such as pivoting, sorting, or changing page field items.
        /// </summary>
        private bool m_bPreserveFormatting;
        /// <summary>
        /// True, if tooltips displayed for the pivot table cell.
        /// </summary>
        private bool m_bShowTooltips;
        /// <summary>
        /// Indicates whether drills are shown.
        /// </summary>
        private bool m_bShowDrill = true;
        /// <summary>
        /// Controls whether or not filter buttons and PivotField captions for rows
        /// and columns are displayed in the grid.
        /// </summary>
        private bool m_bShowHeaders = true;
        /// <summary>
        /// True if the print titles for the worksheet are set based on the PivotTable report. 
        /// False if the print titles for the worksheet are used.
        /// </summary>
        private bool m_bPrintTitles;
        /// <summary>
        /// Indent maximum value
        /// </summary>
        /// <param name="pivotTable"></param>
        /// <param name="extInfo"></param>
        /// <param name="definitionInfo"></param>
        private const uint DEF_MAX_INDENT = 127;
        #endregion

        #region Initialization
        public PivotTableOptions(PivotTableImpl pivotTable, ViewExtendedInfoRecord extInfo, PivotViewDefinitionRecord definitionInfo)
        {
            m_pivotTable = pivotTable;
            m_extInfo = extInfo;
            m_DefinitionInfo = definitionInfo;
            InitDefault();
        }
        internal void InitDefault()
        {
            RowLayout = PivotTableRowLayout.Compact;
            IsAutoFormat = true;
            Indent = 0;
            Outline = true;
            OutlineData = true;
            IsMultiFieldFilter = false;
            DataCaption = "Values";
            byte version = m_pivotTable.GetPivotVersion();
            MiniRefreshVersion = CreatedVersion = UpdatedVersion = version;
            IsMultiFieldFilter = true;
            ShowGridDropZone = false;
            m_pageFieldsOrder = PivotPageAreaFieldsOrder.DownThenOver;
            ErrorString = "";
            DisplayErrorString = false;
            NullString = "";
            DisplayNullString = true;
            PreserveFormatting = true;
            ShowCustomSortList = true;
            ShowTooltips = true;
            DisplayFieldCaptions = true;
        }
        #endregion

        #region Properties

        /// <summary>
        /// If true apply legacy table autoformat alignment properties.
        /// </summary>
        public bool IsAlignAutoFormat
        {
            get
            {
                return m_DefinitionInfo.IsAlignAutoFormat;
            }
            set
            {
                m_DefinitionInfo.IsAlignAutoFormat = value;
            }
        }
        /// <summary>
        /// If true apply legacy table autoformat border properties
        /// </summary>
        public bool IsBorderAutoFormat
        {
            get
            {
                return m_DefinitionInfo.IsBorderAutoFormat;
            }
            set
            {
                m_DefinitionInfo.IsBorderAutoFormat = value;
            }
        }
        /// <summary>
        /// If true apply legacy table autoformat number format properties.
        /// </summary>
        public bool IsNumberAutoFormat
        {
            get
            {
                return m_DefinitionInfo.IsNumberAutoFormat;
            }
            set
            {
                m_DefinitionInfo.IsNumberAutoFormat = value;
            }
        }
        /// <summary>
        /// If true apply legacy table autoformat pattern properties.
        /// </summary>
        public bool IsPatternAutoFormat
        {
            get
            {
                return m_DefinitionInfo.IsPatternAutoFormat;
            }
            set
            {
                m_DefinitionInfo.IsPatternAutoFormat = value;
            }
        }
        public bool IsWHAutoFormat
        {
            get
            {
                return m_DefinitionInfo.IsWHAutoFormat;
            }
            set
            {
                m_DefinitionInfo.IsWHAutoFormat = value;
            }
        }
        public bool IsAutoFormat
        {
            get
            {
                return m_DefinitionInfo.IsAutoFormat;
            }
            set
            {
                m_DefinitionInfo.IsAutoFormat = value;
            }
        }

        /// <summary>
        /// If true apply legacy table autoformat font properties.
        /// </summary>
        public bool IsFontAutoFormat
        {
            get
            {
                return m_DefinitionInfo.IsFontAutoFormat;
            }
            set
            {
                m_DefinitionInfo.IsFontAutoFormat = value;
            }
        }
        /// <summary>
        /// True if an asterisk (*) is displayed next to each subtotal and grand total 
        /// value in the specified PivotTable report
        /// </summary>
        public bool ShowAsteriskTotals
        {
            get
            {
                return m_bShowAsteriskTotals;
            }
            set
            {
                m_bShowAsteriskTotals = value;
            }
        }
        /// <summary>
        /// Specifies the string to be displayed in column header of pivot Table when in compact mode.
        /// </summary>
        public string ColumnHeaderCaption
        {
            get
            {
                return m_columnHeaderCaption;
            }
            set
            {
                if (!m_pivotTable.Workbook.Loading)
                    m_pivotTable.SetChanged(false);
                m_columnHeaderCaption = value;
            }
        }
        /// <summary>
        /// Specifies the string to be displayed in Row header of pivot table when in compact mode
        /// </summary>
        public string RowHeaderCaption
        {
            get 
            {
                return m_rowHeaderCaption;
            }
            set
            {
                if (!m_pivotTable.Workbook.Loading)
                    m_pivotTable.SetChanged(false);
                m_rowHeaderCaption = value;
            }
        }
        /// <summary>
        /// This property specifies the pivot table row 
        /// layout settings.
        /// </summary>
        public PivotTableRowLayout RowLayout
        {
            get
            {
                return m_rowLayout;
            }
            set
            {
                m_rowLayout = value;
            }
        }
        /// <summary>
        /// Specifies the version of the application that created the cache
        /// </summary>
        public byte CreatedVersion
        {
            get
            {
                return m_btCreatedVersion;
            }
            set
            {
                m_btCreatedVersion = value;
            }
        }
        /// <summary>
        /// Specifies the version of the application that updated the cache
        /// </summary>
        public byte UpdatedVersion
        {
            get
            {
                return m_btUpdatedVersion;
            }
            set
            {
                m_btUpdatedVersion = value;
            }
        }
        /// <summary>
        /// Specifies the mini version of the application that updated the cache
        /// </summary>
        public byte MiniRefreshVersion
        {
            get
            {
                return m_btMiniRefreshVersion;
            }
            set
            {
                m_btMiniRefreshVersion = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the "custom lists" option is offered
        ///when sorting this PivotTable
        /// </summary>
        public bool ShowCustomSortList
        {
            get
            {
                return m_bShowCustomSortList;
            }
            set
            {
                m_bShowCustomSortList = value;
            }
        }
        /// <summary>
        /// Specifies the name of the value area field header in the PivotTable. This caption is shown
        ///when the PivotTable when two or more fields are in the values area.
        /// </summary>
        public string DataCaption
        {
            get
            {
                return m_dataCaption; ;
            }
            set
            {
                m_dataCaption = value;
            }
        }
        ///// <summary>
        ///// Specifies a boolean value that indicates whether the field representing multiple fields in
        /////the data region is located in the row area or the column area.
        ///// </summary>
        //public bool IsDataOnRows
        //{
        //    get
        //    {
        //        return m_bIsDataOnRows;
        //    }
        //    set
        //    {
        //        m_bIsDataOnRows = value;
        //    }
        //}
        /// <summary>
        /// Specifies the position for the field representing multiple data field in the PivotTable,
        ///whether that field is located in the row area or column area.
        /// </summary>
        public ushort DataPosition
        {
            get
            {
                return m_DefinitionInfo.DataPos;
            }
            set
            {
                bool isError = !m_pivotTable.Workbook.Loading && 
                                m_pivotTable.RowFields.Count<=value;
                if ( isError)
                    throw new ArgumentOutOfRangeException("DataPosition must less than are equal to row fields count in the pivot table");
                m_DefinitionInfo.DataPos = value;
            }
        }
        /// <summary>
        /// False to disable the ability to display the field list for the PivotTable. 
        /// If the field list was already being displayed it disappears.
        /// </summary>
        public bool ShowFieldList
        {
            get
            {
                return m_pivotTable.Workbook.HidePivotFieldList;
            }
            set
            {
                m_pivotTable.Workbook.HidePivotFieldList = value;
            }
        }
        /// <summary>
        ///True to disable the alert for when the user overwrites values in the data area of the PivotTable. 
        ///True also allows the user to change data values that previously could not be changed
        /// </summary>
        public bool IsDataEditable
        {
            get
            {
                return m_bIsDataEditable;
            }
            set
            {
                m_bIsDataEditable = value;
            }
        }
        /// <summary>
        /// True if the PivotTable Field dialog box is available when the user double-clicks the PivotTable field
        /// </summary>
        public bool EnableFieldProperties
        {
            get
            {
                return m_bEnableFieldProperties;
            }
            set
            {
                m_bEnableFieldProperties = value;
            }
        }
        /// <summary>
        /// indicates whether fields in the PivotTable are sorted in
        ///non-default order in the field list.
        /// </summary>
        public bool IsDefaultAutoSort
        {
            get
            {
                return m_bIsDefaultAutoSort;
            }
            set
            {
                m_bIsDefaultAutoSort = value;
            }
        }
        /// <summary>
        /// Indicates whether calculated members should be shown in
        ///the PivotTable view.
        /// </summary>
        public bool ShowCalcMembers
        {
            get
            {
                return m_bShowCalcMembers;
            }
            set
            {
                m_bShowCalcMembers = value;
            }
        }
        /// <summary>
        /// Specifies the indentation increment for compact axis and can be used to set the Report
        /// Layout to Compact Form.
        /// </summary>
        public uint Indent
        {
            get
            {
                return m_iIndent;
            }
            set
            {
                m_iIndent = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether new fields should have their outline
        /// flag set to true.
        /// </summary>
        public bool Outline
        {
            get
            {
                return m_bOutline;
            }
            set
            {
                m_bOutline = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether data fields in the PivotTable should be
        /// displayed in outline form.
        /// </summary>
        public bool OutlineData
        {
            get
            {
                return m_bOutlineData;
            }
            set
            {
                m_bOutlineData = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the fields of a PivotTable can have
        /// multiple filters set on them.
        /// </summary>
        public bool IsMultiFieldFilter
        {
            get
            {
                return m_bMultiFieldFilter;
            }
            set
            {
                m_bMultiFieldFilter = value;
            }
        }
        /// <summary>
        /// Specifies a boolean value that indicates whether the in-grid drop zones should be
        ///displayed at runtime, and whether classic layout is applied.
        /// </summary>
        public bool ShowGridDropZone
        {
            get
            {
                return m_bShowGridDropZone;
            }
            set
            {
                m_bShowGridDropZone = value;
            }
        }
        /// <summary>
        /// Returns or sets the string displayed in cells that contain errors
        /// when the DisplayErrorString property is True.
        /// </summary>
        public string ErrorString
        {
            get
            {
                return m_extInfo.ErrorString;
            }
            set
            {
                DisplayErrorString = true;
                m_extInfo.ErrorString = value;
            }
        }
        /// <summary>
        /// True if the PivotTable report displays a custom error string in cells
        /// that contain errors. The default value is False.
        /// </summary>
        public bool DisplayErrorString
        {
            get
            {
                return m_extInfo.IsDisplayErrorString;
            }
            set
            {
                m_extInfo.IsDisplayErrorString = value;
            }
        }
        /// <summary>
        /// True if the PivotTable report displays a custom string in cells
        /// that contain null values. The default value is True. 
        /// </summary>
        public bool DisplayNullString
        {
            get
            {
                return m_extInfo.IsDisplayNullString;
            }
            set
            {
                m_extInfo.IsDisplayNullString = value;
            }
        }
        /// <summary>
        /// True if the specified PivotTable report’s outer-row item,
        /// column item, subtotal, and grand total labels use merged cells.
        /// </summary>
        public bool MergeLabels
        {
            get
            {
                return m_extInfo.IsMergeLabels;
            }
            set
            {
                m_extInfo.IsMergeLabels = value;
            }
        }
        /// <summary>
        /// Returns or sets the number of page fields in each column
        /// or row in the PivotTable report.
        /// </summary>
        public int PageFieldWrapCount
        {
            get
            {
                return m_extInfo.WrapPage;
            }
            set
            {
                if (value < 0 || value > ViewExtendedInfoRecord.DEF_WRAPPAGE_MAXVALUE)
                    throw new ArgumentOutOfRangeException("PageFieldWrapCount");

                m_extInfo.WrapPage = (ushort)value;
            }
        }
        /// <summary>
        /// Returns or sets the order in which page fields 
        /// are added to the PivotTable report’s layout
        /// </summary>
        public PivotPageAreaFieldsOrder PageFieldsOrder
        {
            get
            {
                return m_pageFieldsOrder;
            }
            set
            {
                m_pageFieldsOrder = value;
            }
        }
        /// <summary>
        /// Returns or sets the string displayed in cells that contain null
        /// values when the DisplayNullString property is True.
        /// </summary>
        public string NullString
        {
            get
            {
                return m_extInfo.NullString;
            }
            set
            {
                m_extInfo.NullString = value;
            }
        }
        /// <summary>
        /// True if formatting is preserved when the report is refreshed or recalculated by 
        /// operations such as pivoting, sorting, or changing page field items.
        /// </summary>
        public bool PreserveFormatting
        {
            get
            {
                return m_bPreserveFormatting;
            }
            set
            {
                m_bPreserveFormatting = value;
            }
        }
        /// <summary>
        /// True, if tooltips displayed for the pivot table cell.
        /// </summary>
        public bool ShowTooltips
        {
            get
            {
                return m_bShowTooltips;
            }
            set
            {
                m_bShowTooltips = value;
            }
        }
        /// <summary>
        /// The ShowDrillIndicators property is used for toggling the display of
        /// drill indicators in the PivotTable.
        /// </summary>
        public bool ShowDrillIndicators
        {
            get
            {
                return m_bShowDrill;
            }
            set
            {
                m_bShowDrill = value;
            }
        }
        /// <summary>
        /// Gets/sets value controlling whether or not filter buttons and PivotField
        /// captions for rows and columns are displayed in the grid.
        /// </summary>
        public bool DisplayFieldCaptions
        {
            get
            {
                return m_bShowHeaders;
            }
            set
            {
                if (!m_pivotTable.Workbook.Loading)
                    m_pivotTable.SetChanged(true);
                m_bShowHeaders = value;
            }
        }
        /// <summary>
        /// True if the print titles for the worksheet are set based on the PivotTable report. 
        /// False if the print titles for the worksheet are used.
        /// </summary>
        public bool PrintTitles
        {
            get
            {
                return m_bPrintTitles;
            }
            set
            {
                m_bPrintTitles = value;
            }
        }
        /// <summary>
        ///True if data for the PivotTable report is saved with the workbook. 
        ///False if only the report definition is saved
        /// </summary>
        public bool IsSaveData
        {
            get
            {
                return m_pivotTable.Cache.IsSaveData;
            }
            set
            {
                 m_pivotTable.Cache.IsSaveData = value;
            }
        }
        /// <summary>
        ///Return the maximum indent value
        /// </summary>
        internal uint MaxIndent
        {
            get
            {
                return DEF_MAX_INDENT;
            }
        }

        #endregion


    }
}
