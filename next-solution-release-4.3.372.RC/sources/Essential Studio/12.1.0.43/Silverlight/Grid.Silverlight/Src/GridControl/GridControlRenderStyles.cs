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

#if !WinRT
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.ComponentModel;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using Syncfusion.WinRT.ComponentModel;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    /// <summary>
    /// A class that emulates a Dictionary API and contains GridRenderStyleInfo objects.
    /// Internally rows are held in a dictionary and each row has a dictionary of cells.
    /// The class also provides support for adding, removing and moving rows and 
    /// columns.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
        /// <param name="cellRowColumnIndex"></param>
        /// <param name="value"></param>
        public void OnMovedCell(RowColumnIndex cellRowColumnIndex, GridRenderStyleInfo value)
        {
            value.CellIdentity.UpdateCellRowColumnIndex(cellRowColumnIndex);
        }

        /// <summary>
        /// Called when from <see cref="RowColumnIndexValueDictionary{T}"/> when a cell
        /// is removed either by a RemoveRows, RemoveColumns, Remove or RemoveAll call.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="value">The value.</param>
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
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridControlRenderStyles: Disposable
    {
        GridControlBase gridControl;

        internal GridRenderStyleInfoDictionary visibleCellStyles = new GridRenderStyleInfoDictionary();
        internal GridRenderStyleInfoDictionary unloadCellStylesDictionary;

        internal GridControlRenderStyles(GridControlBase gridControl)
        {
            this.gridControl = gridControl;
        }

        public GridRenderStyleInfo this[int rowIndex, int columnIndex]
        {
            get
            {
                return GetRenderStyleInfo(rowIndex, columnIndex);
            }
        }

        public GridRenderStyleInfo this[RowColumnIndex cell]
        {
            get
            {
                return GetRenderStyleInfo(cell);
            }
        }

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
                GridPrepareRenderCellEventArgs e = new GridPrepareRenderCellEventArgs(cellRowColumnIndex, renderCellStyle, this.gridControl);
                gridControl.RaisePrepareRenderCell(e);
                renderCellStyle.AllowInvalidateCell = true;
                return renderCellStyle;
            }
        }

        public GridRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex, bool createDisposableObject)
        {
            if (createDisposableObject)
            {
                GridStyleInfo modelStyle = gridControl.Model[rowIndex, columnIndex];
                GridRenderStyleInfo renderCellStyle = new GridRenderStyleInfo(gridControl, modelStyle);
                RowColumnIndex cellRowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
                GridPrepareRenderCellEventArgs e = new GridPrepareRenderCellEventArgs(cellRowColumnIndex, renderCellStyle, this.gridControl);
                gridControl.RaisePrepareRenderCell(e);
                return renderCellStyle;
            }

            return GetRenderStyleInfo(rowIndex, columnIndex);
        }

        public GridRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex)
        {
            return GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        public GridRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex, bool createDisposableObject)
        {
            return GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex, createDisposableObject);
        }

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

        public void Clear(CellSpanInfoBase cellSpan)
        {
            visibleCellStyles.Remove(cellSpan);
        }

        public void Clear()
        {
            visibleCellStyles.RemoveAll();
        }

        #region Insert and Remove Rows
        public void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRows(insertAtRowIndex, count, null);
        }

        public void InsertRows(int insertAtRowIndex, int count, GridRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.InsertRows(insertAtRowIndex, count, moveCells);
        }

        public void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRows(removeAtRowIndex, count, null);
        }

        public void RemoveRows(int removeAtRowIndex, int count, GridRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.RemoveRows(removeAtRowIndex, count, moveCells);
        }
        #endregion

        #region Insert and Remove Columns
        public void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumns(insertAtColumnIndex, count, null);
        }

        public void InsertColumns(int insertAtColumnIndex, int count, GridRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.InsertColumns(insertAtColumnIndex, count, moveCells);
        }

        public void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumns(removeAtColumnIndex, count, null);
        }

        public void RemoveColumns(int removeAtColumnIndex, int count, GridRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.RemoveColumns(removeAtColumnIndex, count, moveCells);
        }
        #endregion

        public void PrepareArrange()
        {
            // Create new GridRenderStyleInfo objects for cells scrolled into view or
            // unload GridRenderStyleInfo objects for cells scrolled out of view.
            unloadCellStylesDictionary = this.visibleCellStyles;
            visibleCellStyles = new GridRenderStyleInfoDictionary();
        }

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (unloadCellStylesDictionary != null)
                {
                    foreach (KeyValuePair<RowColumnIndex, GridRenderStyleInfo> entry in unloadCellStylesDictionary)
                    {
                        entry.Value.Identity.IsDisposable = true;
                        entry.Value.Dispose();
                    }
                    unloadCellStylesDictionary.Clear();
                    unloadCellStylesDictionary = null;
                }
                if (this.visibleCellStyles != null)
                {
                    foreach (KeyValuePair<RowColumnIndex, GridRenderStyleInfo> item in visibleCellStyles)
                    {
                        item.Value.Identity.IsDisposable = true;
                        item.Value.Dispose();
                    }
                    this.visibleCellStyles.Clear();
                    this.visibleCellStyles = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
