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

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Specifies the type of cell that is returned by the <see cref="GridDataStyleInfo.CellIdentity"/>.
    /// </summary>
    public enum GridDataTableCellType
    {
        /// <summary>
        /// Specifies an Empty cell.
        /// </summary>
        EmptyCell,
        /// <summary>
        /// Specifies an FilterBar cell.
        /// </summary>
        FilterBarCell,
        /// <summary>
        /// Specifies the first cell in the grid.
        /// </summary>
        TopLeftHeaderCell,
        /// <summary>
        /// Specifies the indented column cell.
        /// </summary>
        ColumnHeaderIndentCell,
        /// <summary>
        /// Specifies the column header cell. Use the <see cref="GridDataStyleInfo.CellIdentity"/> to retreive column specific details stored in the Identity.
        /// </summary>
        ColumnHeaderCell,
        /// <summary>
        /// Specifies the stacked column header cell.
        /// </summary>
        StackedColumnHeaderCell,
        /// <summary>
        /// Specifies the unbound column header cell.
        /// </summary>
        UnboundColumnHeaderCell,
        /// <summary>
        /// Specifies the unbound column cell.
        /// </summary>
        UnboundColumnCell,
        /// <summary>
        /// Specifies the row header cell when <see cref="TableProperties.ShowRowHeader"/> is true.
        /// </summary>
        RowHeaderCell,
        /// <summary>
        /// Specifies the plus / minus cell for Grouped / Related objects.
        /// </summary>
        RecordPlusMinusCell,
        /// <summary>
        /// Specifies the record cell.
        /// </summary>
        RecordCell,
        /// <summary>
        /// Specifies the Add new record cell
        /// </summary>
        AddNewRecordCell,
        /// <summary>
        /// Specifies the Add new row header cell
        /// </summary>
        AddNewRowHeaderCell,
        /*AlternateRecordCell,*/
        /// <summary>
        /// Specifies the nested table cell.
        /// </summary>
        NestedTableCell,
        /// <summary>
        /// Specifies the empty nested cell.
        /// </summary>
        NestedTableEmptyCell,
        /// <summary>
        /// Specifies the unbound record cell.
        /// </summary>
        UnboundRecordCell,
        /// <summary>
        /// Specifies the group plus minus cell.
        /// </summary>
        GroupCaptionPlusMinusCell,
        /// <summary>
        /// Specifies the group caption cell.
        /// </summary>
        GroupCaptionCell,
        /// <summary>
        /// Specifies the group caption summary cell. Use the <see cref="GridDataStyleInfo.CellIdentity"/> to get specific values related to summary.
        /// </summary>
        GroupCaptionSummaryRecordCell,
        /// <summary>
        /// Specifies the group caption covered cell.
        /// </summary>
        GroupCaptionSummaryCoveredCell,
        /// <summary>
        /// Specifies the group caption empty cell.
        /// </summary>
        GroupCaptionSummaryEmptyCell,
        /// <summary>
        /// Specifies the group caption title cell. This is usually created when the <see cref="GridDataSummaryRow.Title"/> property is set.
        /// </summary>
        GroupCaptionSummaryTitleCell,
        /// <summary>
        /// Specifies the summary title cell. This is usually created when the <see cref="GridDataSummaryRow.Title"/> property is set.
        /// </summary>
        SummaryTitleCell,
        /// <summary>
        /// Specifies the summary record cell.
        /// </summary>
        SummaryRecordCell,
        /// <summary>
        /// Specifies the summary covered cell.
        /// </summary>
        SummaryCoveredCell,
        /// <summary>
        /// Specifies the summary empty cell.
        /// </summary>
        SummaryEmptyCell,
        /// <summary>
        /// Specifies the DropDownFilterBar cell
        /// </summary>
        DropDownFilterCell,
        /// <summary>
        /// Specifies the Details View(DataBound Template) cell
        /// </summary>
        DetailsViewCell
    }
}
