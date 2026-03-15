#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !(SILVERLIGHT) && !(WINRT) && !(WP) && !SyncfusionFramework2_0
using Syncfusion.XlsIO.Implementation.PivotAnalysis;
#endif

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Represents pivot table object.
    /// </summary>
    public interface IPivotTable
    {
        /// <summary>
        /// Get or set pivot table name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets collection of pivot fields. Read-only.
        /// </summary>
        IPivotFields Fields { get; }
        /// <summary>
        /// Gets collection of pivot table data fields. Read-only.
        /// </summary>
        IPivotDataFields DataFields { get; }
        /// <summary>
        /// Gets/sets value indicating whether the PivotTable contains row with grand totals for columns (same as RowGrand in VBA).
        /// </summary>
        bool RowGrand { get; set; }
        /// <summary>
        /// Gets/sets value indicating whether the PivotTable contains column with grand totals for rows (same as ColumnGrand in VBA).
        /// </summary>
        bool ColumnGrand { get; set; }
        /// <summary>
        /// The ShowDrillIndicators property is used for toggling the display of
        /// drill indicators in the PivotTable.
        /// </summary>
        bool ShowDrillIndicators { get; set; }
        /// <summary>
        /// Gets/sets value controlling whether or not filter buttons and PivotField
        /// captions for rows and columns are displayed in the grid.
        /// </summary>
        bool DisplayFieldCaptions { get; set; }
        /// <summary>
        /// True if row, column, and item labels appear on the first row of each page when
        /// the specified PivotTable report is printed. False if labels are printed only on
        /// the first page. The default value is True.
        /// </summary>
        bool RepeatItemsOnEachPrintedPage { get; set; }
        /// <summary>
        /// Gets/sets built-in pivot style.
        /// </summary>
        PivotBuiltInStyles? BuiltInStyle { get; set; }
        /// <summary>
        /// Gets/sets value indicating whether the PivotTable contains grand totals for rows.
        /// </summary>
        bool ShowRowGrand { get; set; }
        /// <summary>
        /// Gets/sets value indicating whether the PivotTable contains grand totals for columns.
        /// </summary>
        bool ShowColumnGrand { get; set; }
        /// <summary>
        /// Gets Index of the pivot Cache.Read-only.
        /// </summary>
        int CacheIndex { get; }
        /// <summary>
        /// Returns pivot table location.
        /// </summary>
        IRange Location { get; set; }
        /// <summary>
        /// <summary>
        /// Represents the pivot table options.Read-only
        /// </summary>
        IPivotTableOptions Options { get; }
        /// <summary>
        /// Specifies the number of rows per page for this PivotTable that the filter area will occupy.Read-only.
        /// </summary>
        int RowsPerPage { get; }
        /// <summary>
        /// Specifies the number of columns per page for this PivotTable that the filter area will
        ///occupy.
        /// </summary>
        int ColumnsPerPage { get; }
        /// <summary>
        /// Returns the collection of calculated fields of the specified pivot table.Read-only.
        /// </summary>
        IPivotCalculatedFields CalculatedFields { get; }
        /// <summary>
        /// Returns the collection of page field for the specified pivot table,Read-only.
        /// </summary>
        IPivotFields PageFields { get; }
        /// <summary>
        /// Returns the collection of Row field for the specified pivot table.Read-only.
        /// </summary>
        IPivotFields RowFields { get; }
        /// <summary>
        /// Returns the collection of Column field for the specified pivot table.Read-only.
        /// </summary>
        IPivotFields ColumnFields { get; }
        /// <summary>
        /// Gets/sets value indicating whether the PivotTable data fields are shown in rows.
        /// </summary>
        bool ShowDataFieldInRow { get; set; }
        /// <summary>
        /// This method clears all the fields, deletes all filtering and sorting applied to the PivotTable.
        /// </summary>
        void ClearTable();
#if !SILVERLIGHT && !WINRT && !WP && !(WP) && !SyncfusionFramework2_0
        /// <summary>
        /// Method to draw the pivot table
        /// </summary>
        void Layout();
#endif

#if !SILVERLIGHT && !WINRT && !WP && !(WP) && !SyncfusionFramework2_0
        /// <summary>
        /// Values of pivot engine.
        /// </summary>
        PivotEngine PivotEngineValues { get; set; }
#endif
    }
}
