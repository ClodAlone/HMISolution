#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// A class that emulates a Dictionary API and contains GridRenderStyleInfo objects.
    /// Internally rows are held in a dictionary and each row has a dictionary of cells.
    /// The class also provides support for adding, removing and moving rows and 
    /// columns.
    /// </summary>
    public class GridRenderStyleInfoDictionary : RowColumnIndexValueDictionary<GridRenderStyleInfo>, IRowColumnIndexValueDictionaryCallbacks<GridRenderStyleInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridRenderStyleInfoDictionary"/> class.
        /// </summary>
        public GridRenderStyleInfoDictionary()
        {
            SetCallback(this);
        }

        #region IRowColumnIndexValueDictionaryCallbacks<GridRenderStyleInfo> Members

        /// <summary>
        /// Called from <see cref="RowColumnIndexValueDictionary{T}"/> when a cell
        /// is moved after a InsertRows, RemoveRows, InsertColumns or RemoveColumns
        /// method call.
        /// </summary>
        /// <param name="cellRowColumnIndex">The cell row column index.</param>
        /// <param name="value">The cell style.</param>
        public void OnMovedCell(RowColumnIndex cellRowColumnIndex, GridRenderStyleInfo value)
        {
            value.CellIdentity.UpdateCellRowColumnIndex(cellRowColumnIndex);
        }

        /// <summary>
        /// Called when from <see cref="RowColumnIndexValueDictionary{T}"/> when a cell
        /// is removed either by a RemoveRows, RemoveColumns, Remove or RemoveAll call.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="value">The cell style.</param>
        public void OnRemoveCell(RowColumnIndex cellRowColumnIndex, GridRenderStyleInfo value)
        {
            value.Dispose();
        }

        #endregion
    }
   
/*
    public interface IGridControlRenderStyles
    {
        void Clear(RowColumnIndex cellRowColumnIndex);
        void Clear();
        void Clear(CellSpanInfoBase cellSpan);
        void ConcludeArrange();
        GridRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex, bool createDisposableObject);
        GridRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex);
        GridRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex, bool createDisposableObject);
        GridRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex);
        void InsertColumns(int insertAtColumnIndex, int count);
        void InsertColumns(int insertAtColumnIndex, int count, GridRenderStyleInfoDictionary moveCells);
        void InsertRows(int insertAtRowIndex, int count, GridRenderStyleInfoDictionary moveCells);
        void InsertRows(int insertAtRowIndex, int count);
        void PrepareArrange();
        void RemoveColumns(int removeAtColumnIndex, int count);
        void RemoveColumns(int removeAtColumnIndex, int count, GridRenderStyleInfoDictionary moveCells);
        void RemoveRows(int removeAtRowIndex, int count, GridRenderStyleInfoDictionary moveCells);
        void RemoveRows(int removeAtRowIndex, int count);
        GridRenderStyleInfo this[int rowIndex, int columnIndex] { get; }
        GridRenderStyleInfo this[RowColumnIndex cell] { get; }
    }
*/
    /// <summary>
    /// Holds a collection of rendering styles for the grid cells.
    /// </summary>
    public class GridControlRenderStyles
    {
        internal GridControlBase gridControl;

        internal GridRenderStyleInfoDictionary visibleCellStyles = new GridRenderStyleInfoDictionary();
        internal GridRenderStyleInfoDictionary unloadCellStylesDictionary;

        internal GridControlRenderStyles(GridControlBase gridControl)
        {
            this.gridControl = gridControl;
        }

        /// <summary>
        /// Returns the render style for a specific cell.
        /// </summary>
        /// <param name="rowIndex">Cell row index.</param>
        /// <param name="columnIndex">Cell column index.</param>
        /// <returns>Render style for the cell.</returns>
        public GridRenderStyleInfo this[int rowIndex, int columnIndex]
        {
            get
            {
                return GetRenderStyleInfo(rowIndex, columnIndex);
            }
        }

        /// <summary>
        /// Returns the render style for a specific cell.
        /// </summary>
        /// <param name="cell">Row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <returns>Render style for the cell.</returns>
        public GridRenderStyleInfo this[RowColumnIndex cell]
        {
            get
            {
                return GetRenderStyleInfo(cell);
            }
        }

        /// <summary>
        /// Returns the render style for a specific cell.
        /// </summary>
        /// <param name="rowIndex">Cell row index.</param>
        /// <param name="columnIndex">Cell column index.</param>
        /// <returns>Render style for the cell.</returns>
        public GridRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex)
        {
            RowColumnIndex cellRowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);

            // When called from ArrangeCellUIElements then look first in old visibleCellStyles 
            // which have been assigned to unloadCellStylesDictionary
            GridRenderStyleInfo renderCellStyle;
            if (unloadCellStylesDictionary != null && unloadCellStylesDictionary.TryGetValue(cellRowColumnIndex, out renderCellStyle))
            {
                unloadCellStylesDictionary.Clear(cellRowColumnIndex);
                visibleCellStyles.Add(cellRowColumnIndex, renderCellStyle);

                return renderCellStyle;
            }

            else if (visibleCellStyles.TryGetValue(cellRowColumnIndex, out renderCellStyle))
            {
                return renderCellStyle;
            }

            else
            {
                GridStyleInfo modelStyle = gridControl.Model[rowIndex, columnIndex];
                renderCellStyle = new GridRenderStyleInfo(gridControl, modelStyle);
                renderCellStyle.Identity.IsDisposable = false;
                renderCellStyle.AllowInvalidateCell = false;
                visibleCellStyles.Add(cellRowColumnIndex, renderCellStyle);
                GridPrepareRenderCellEventArgs e = new GridPrepareRenderCellEventArgs(cellRowColumnIndex, renderCellStyle, GridControlBase.PrepareRenderCellEvent, this.gridControl);
                gridControl.RaisePrepareRenderCell(e);
                renderCellStyle.AllowInvalidateCell = true;
                return renderCellStyle;
            }
        }

        /// <summary>
        /// Gets the rendering style for a cell given its row and column indices.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        /// <param name="createDisposableObject">When true, returns a render style object that can be disposed.</param>
        /// <returns>Rendering cell style.</returns>
        public GridRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex, bool createDisposableObject)
        {
            if (createDisposableObject)
            {
                GridStyleInfo modelStyle = gridControl.Model[rowIndex, columnIndex];
                GridRenderStyleInfo renderCellStyle = new GridRenderStyleInfo(gridControl, modelStyle);
                RowColumnIndex cellRowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
                GridPrepareRenderCellEventArgs e = new GridPrepareRenderCellEventArgs(cellRowColumnIndex, renderCellStyle, GridControlBase.PrepareRenderCellEvent, this.gridControl);
                gridControl.RaisePrepareRenderCell(e);
                return renderCellStyle;
            }

            return GetRenderStyleInfo(rowIndex, columnIndex);
        }

        /// <summary>
        /// Gets the rendering style for a cell given its row and column indices.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <returns>Rendering cell style.</returns>
        public GridRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex)
        {
            return GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        /// <summary>
        /// Gets the rendering style for a cell given its row and column indices.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <param name="createDisposableObject">When true, returns a render style object that can be disposed.</param>
        /// <returns>Rendering cell style.</returns>
        public GridRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex, bool createDisposableObject)
        {
            return GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex, createDisposableObject);
        }

        /// <summary>
        /// Removes the render style entry for the given cell.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row and column indices as <see cref="RowColumnIndex"/>.</param>
        public void Clear(RowColumnIndex cellRowColumnIndex)
        {
            GridRenderStyleInfo renderCellStyle;
            if (visibleCellStyles.TryGetValue(cellRowColumnIndex, out renderCellStyle))
            {
                renderCellStyle.Dispose();
                visibleCellStyles.Clear(cellRowColumnIndex);
            }

            //if (cellRowColumnIndex == gridControl.CurrentCell.CellRowColumnIndex)
            //{
            //    gridControl.CurrentCell.Renderer.Invalidate();
            //}
        }

        /// <summary>
        /// Removes the render style entry for given cell span.
        /// </summary>
        /// <param name="cellSpan">Cell spanned range.</param>
        public void Clear(CellSpanInfoBase cellSpan)
        {
            visibleCellStyles.Remove(cellSpan);
        }

        /// <summary>
        /// Removes all entries from the render style dictionary.
        /// </summary>
        public void Clear()
        {
            visibleCellStyles.RemoveAll();
        }

        #region Insert and Remove Rows
        /// <summary>
        /// Inserts the given number of rows into the render styles dictionary.
        /// </summary>
        /// <param name="insertAtRowIndex">The insert row index.</param>
        /// <param name="count">Number of rows to insert.</param>
        public void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRows(insertAtRowIndex, count, null);
        }

        /// <summary>
        /// Inserts the given number of rows into the render styles dictionary.
        /// </summary>
        /// <param name="insertAtRowIndex">The insert row index.</param>
        /// <param name="count">Number of rows to insert.</param>
        /// <param name="moveCells">The move cells.</param>
        public void InsertRows(int insertAtRowIndex, int count, GridRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.InsertRows(insertAtRowIndex, count, moveCells);
        }

        /// <summary>
        /// Removes the specified rows from the cell styles dictionary.
        /// </summary>
        /// <param name="removeAtRowIndex">The remove row index.</param>
        /// <param name="count">Number of rows to remove.</param>
        public void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRows(removeAtRowIndex, count, null);
        }

        /// <summary>
        /// Removes the specified rows from the cell styles dictionary.
        /// </summary>
        /// <param name="removeAtRowIndex">The remove row index.</param>
        /// <param name="count">Number of rows to remove.</param>
        /// <param name="moveCells">The move cells.</param>
        public void RemoveRows(int removeAtRowIndex, int count, GridRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.RemoveRows(removeAtRowIndex, count, moveCells);
        }
        #endregion

        #region Insert and Remove Columns
        /// <summary>
        /// Inserts the given no. of columns into the cell styles dictionary.
        /// </summary>
        /// <param name="insertAtColumnIndex">The column index to insert.</param>
        /// <param name="count">No. of columns to be inserted.</param>
        public void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumns(insertAtColumnIndex, count, null);
        }

        /// <summary>
        /// Inserts the given no. of columns into the cell styles dictionary.
        /// </summary>
        /// <param name="insertAtColumnIndex">The column index to insert.</param>
        /// <param name="count">No. of columns to be inserted.</param>
        /// <param name="moveCells">The move cells.</param>
        public void InsertColumns(int insertAtColumnIndex, int count, GridRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.InsertColumns(insertAtColumnIndex, count, moveCells);
        }

        /// <summary>
        /// Removes the specified columns from the cell styles dictionary.
        /// </summary>
        /// <param name="removeAtColumnIndex">Remove column index.</param>
        /// <param name="count">No. of columns to be removed.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumns(removeAtColumnIndex, count, null);
        }

        /// <summary>
        /// Removes the specified columns from the cell styles dictionary.
        /// </summary>
        /// <param name="removeAtColumnIndex">Remove column index.</param>
        /// <param name="count">No. of columns to be removed.</param>
        /// <param name="moveCells">The move cells.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count, GridRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.RemoveColumns(removeAtColumnIndex, count, moveCells);
        }
        #endregion

        /// <summary>
        /// Create new GridRenderStyleInfo objects for cells scrolled into view or
        /// unload GridRenderStyleInfo objects for cells scrolled out of view.
        /// </summary>
        public void PrepareArrange()
        {
            unloadCellStylesDictionary = this.visibleCellStyles;
            visibleCellStyles = new GridRenderStyleInfoDictionary();
        }

        /// <summary>
        /// Add GridRenderStyleInfo objects for cells scrolled into view or
        /// unload GridRenderStyleInfo objects for cells scrolled out of view.
        /// </summary>
        public void ConcludeArrange()
        {
            foreach (KeyValuePair<RowColumnIndex, GridRenderStyleInfo> entry in unloadCellStylesDictionary)
            {
                // Do not unload style for cells that are still alive.
                if (gridControl.ArrangedCellUIElements.Contains(entry.Key))
                    visibleCellStyles.Add(entry.Key, entry.Value);
                else
                    entry.Value.Dispose();
            }

            unloadCellStylesDictionary.Clear();
            unloadCellStylesDictionary = null;
        }
    }
}
