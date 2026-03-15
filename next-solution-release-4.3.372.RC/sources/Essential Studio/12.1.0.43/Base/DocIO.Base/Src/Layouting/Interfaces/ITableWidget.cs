#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#if !SILVERLIGHT

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Represents a Widget with table layout.
    /// </summary>
    internal interface ITableWidget : IWidget
    {
        /// <summary>
        /// Gets special table info.
        /// </summary>
        ITableLayoutInfo TableLayoutInfo { get; }
        /// <summary>
        /// Gets the row index which contains max no of cells or columns
        /// </summary>
        int MaxRowIndex { get; }
        /// <summary>
        /// Gets rows count.
        /// </summary>
        int RowsCount { get; }
        /// <summary>
        /// Gets columns count.
        /// </summary>
        int ColumnsCount { get; }
        /// <summary>
        /// Gets cell widget by row/column indexes.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        IWidgetContainer GetCellWidget(int row, int column);
        /// <summary>
        /// Gets row widget by row index.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        IWidget GetRowWidget(int row);
    }
}

#endif