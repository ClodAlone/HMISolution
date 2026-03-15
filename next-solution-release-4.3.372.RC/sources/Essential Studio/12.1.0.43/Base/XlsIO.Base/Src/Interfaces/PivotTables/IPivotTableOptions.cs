#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Describes the settings of the pivot table
    /// </summary>
    public interface IPivotTableOptions
    {
        /// <summary>
        /// True if an asterisk (*) is displayed next to each subtotal and grand total 
        /// value in the specified PivotTable report
        /// </summary>
        bool ShowAsteriskTotals { get; set; }
        /// <summary>
        /// Specifies the string to be displayed in column header of pivot Table when in compact layout mode.
        /// </summary>
        string ColumnHeaderCaption { get; set; }
        /// <summary>
        /// Specifies the string to be displayed in Row header of pivot table when in compact layout mode
        /// </summary>
        string RowHeaderCaption { get; set; }
        /// <summary>
        /// Specifies a boolean value that indicates whether the "custom lists" option is offered
        ///when sorting this PivotTable
        /// </summary>
        bool ShowCustomSortList { get; set; }
        /// <summary>
        /// False to disable the ability to display the field list for the PivotTable. 
        /// If the field list was already being displayed it disappears.
        /// </summary>
        bool ShowFieldList { get; set; }
        /// <summary>
        ///True to disable the alert for when the user overwrites values in the data area of the PivotTable. 
        ///True also allows the user to change data values that previously could not be changed
        /// </summary>
        bool IsDataEditable { get; set; }
        /// <summary>
        /// True if the PivotTable Field dialog box is available when the user double-clicks the PivotTable field
        /// </summary>
        bool EnableFieldProperties { get; set; }
        /// <summary>
        /// Specifies the indentation increment for compact axis and can be used to set the Report
        /// Layout to Compact Form.
        /// </summary>
        uint Indent { get; set; }
        /// <summary>
        /// Returns or sets the string displayed in cells that contain errors
        /// when the DisplayErrorString property is True.
        /// </summary>
        string ErrorString { get; set; }
        /// <summary>
        /// True if the PivotTable report displays a custom error string in cells
        /// that contain errors. The default value is False.
        /// </summary>
        bool DisplayErrorString { get; set; }
        /// <summary>
        /// True if the specified PivotTable report’s outer-row item,
        /// column item, subtotal, and grand total labels use merged cells.
        /// </summary>
        bool MergeLabels { get; set; }
        /// <summary>
        /// Returns or sets the number of page fields in each column
        /// or row in the PivotTable report.
        /// </summary>
        int PageFieldWrapCount { get; set; }
        /// <summary>
        /// Returns or sets the order in which page fields 
        /// are added to the PivotTable report’s layout
        /// </summary>
        PivotPageAreaFieldsOrder PageFieldsOrder { get; set; }
        /// <summary>
        /// True if the PivotTable report displays a custom string in cells
        /// that contain null values. The default value is True. 
        /// </summary>
        bool DisplayNullString { get; set; }
        /// <summary>
        /// Returns or sets the string displayed in cells that contain null
        /// values when the DisplayNullString property is True.
        /// </summary>
        string NullString { get; set; }
        /// <summary>
        /// True if formatting is preserved when the report is refreshed or recalculated by 
        /// operations such as pivoting, sorting, or changing page field items.
        /// </summary>
        bool PreserveFormatting { get; set; }
        /// <summary>
        /// True, if tooltips displayed for the pivot table cell.
        /// </summary>
        bool ShowTooltips { get; set; }
        /// <summary>
        /// Gets/sets value controlling whether or not filter buttons and PivotField
        /// captions for rows and columns are displayed in the grid.
        /// </summary>
        bool DisplayFieldCaptions { get; set; }
        /// <summary>
        /// True if the print titles for the worksheet are set based on the PivotTable report. 
        /// False if the print titles for the worksheet are used.
        /// </summary>
        bool PrintTitles { get; set; }
        /// <summary>
        ///True if data for the PivotTable report is saved with the workbook. 
        ///False if only the report definition is saved
        /// </summary>
        bool IsSaveData { get; set; }
        /// <summary>
        /// This property specifies the pivot table row 
        /// layout settings.
        /// </summary>
        PivotTableRowLayout RowLayout { get; set; }
        /// <summary>
        /// The ShowDrillIndicators property is used for toggling the display of
        /// drill indicators in the PivotTable.
        /// </summary>
        bool ShowDrillIndicators { get; set; }
    }
}
