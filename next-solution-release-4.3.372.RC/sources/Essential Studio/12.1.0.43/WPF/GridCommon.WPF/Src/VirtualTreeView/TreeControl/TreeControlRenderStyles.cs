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

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// A class that emulates a Dictionary API and contains TreeRenderStyleInfo objects.
    /// Internally rows are held in a dictionary and each row has a dictionary of cells.
    /// The class also provides support for adding, removing and moving rows and 
    /// columns.
    /// </summary>
    public class TreeRenderStyleInfoDictionary : RowColumnIndexValueDictionary<TreeRenderStyleInfo>, IRowColumnIndexValueDictionaryCallbacks<TreeRenderStyleInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeRenderStyleInfoDictionary"/> class.
        /// </summary>
        public TreeRenderStyleInfoDictionary()
        {
            SetCallback(this);
        }

        #region IRowColumnIndexValueDictionaryCallbacks<TreeRenderStyleInfo> Members

        /// <summary>
        /// Called from <see cref="RowColumnIndexValueDictionary{T}"/> when a cell
        /// is moved after a InsertRows, RemoveRows, InsertColumns or RemoveColumns
        /// method call.
        /// </summary>
        /// <param name="cellRowColumnIndex"></param>
        /// <param name="value"></param>
        public void OnMovedCell(RowColumnIndex cellRowColumnIndex, TreeRenderStyleInfo value)
        {
            value.TreeNodeIdentity.UpdateCellRowColumnIndex(cellRowColumnIndex);
        }

        /// <summary>
        /// Called when from <see cref="RowColumnIndexValueDictionary{T}"/> when a cell
        /// is removed either by a RemoveRows, RemoveColumns, Remove or RemoveAll call.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="value">The value.</param>
        public void OnRemoveCell(RowColumnIndex cellRowColumnIndex, TreeRenderStyleInfo value)
        {
            value.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Provides routines for managing cell styles (<see cref="TreeRenderStyleInfo"/>)
    /// of rendered cells that have been scrolled into view.<para/>
    /// An instance of this class can be accessed with the 
    /// <see cref="VirtualTreeView.RenderStyles"/> property
    /// of a <see cref="VirtualTreeView"/>.
    /// </summary>
    public class TreeControlRenderStyles
    {
        VirtualTreeView treeControl;

        internal TreeRenderStyleInfoDictionary visibleCellStyles = new TreeRenderStyleInfoDictionary();
        internal TreeRenderStyleInfoDictionary unloadCellStylesDictionary;

        internal TreeControlRenderStyles(VirtualTreeView treeControl)
        {
            this.treeControl = treeControl;
        }

        /// <summary>
        /// Gets the render cell style for a cell.
        /// </summary>
        public TreeRenderStyleInfo this[int rowIndex, int columnIndex]
        {
            get
            {
                return GetRenderStyleInfo(rowIndex, columnIndex);
            }
        }

        /// <summary>
        /// Gets the render cell style for a cell.
        /// </summary>
        public TreeRenderStyleInfo this[RowColumnIndex cell]
        {
            get
            {
                return GetRenderStyleInfo(cell);
            }
        }

        /// <summary>
        /// Gets the render cell style for a cell.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        public TreeRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex)
        {
            RowColumnIndex cellRowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);

            // When called from ArrangeCellUIElements then look first in old visibleCellStyles 
            // which have been assigned to unloadCellStylesDictionary
            TreeRenderStyleInfo renderCellStyle;
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
                TreeModel model = treeControl.Model;
                TreeStyleInfo modelStyle = GetModelStyle(cellRowColumnIndex, model);

                renderCellStyle = new TreeRenderStyleInfo(treeControl, modelStyle);
                renderCellStyle.Identity.IsDisposable = false;
                //renderCellStyle.AllowInvalidateCell = true;
                visibleCellStyles.Add(cellRowColumnIndex, renderCellStyle);
                TreePrepareRenderCellEventArgs e = new TreePrepareRenderCellEventArgs(cellRowColumnIndex, renderCellStyle);
                treeControl.RaisePrepareRenderCell(e);
                return renderCellStyle;
            }
        }

        private static TreeStyleInfo GetModelStyle(RowColumnIndex cellRowColumnIndex, TreeModel model)
        {
            TreeNode node = model.VisibleNodes[cellRowColumnIndex.RowIndex];
            TreeColumn column = model.Columns[cellRowColumnIndex.ColumnIndex];

            TreeStyleInfo modelStyle;
            if (!node.Cells.ContainsKey(cellRowColumnIndex.ColumnIndex))
            {
                modelStyle = new TreeStyleInfo();
                modelStyle.CellValue = node.Data;

                TreeLevel level = null;
                int depth = node.GetDepth();
                if (model.Levels.Count > depth)
                    level = model.Levels[depth];

                modelStyle.Identity = new TreeStyleInfoIdentity(node, column, level, cellRowColumnIndex);
                node.Cells[cellRowColumnIndex.ColumnIndex] = modelStyle;
            }
            else
                modelStyle = node.Cells[cellRowColumnIndex.ColumnIndex];
            return modelStyle;
        }

        /// <summary>
        /// Gets the render cell style for a cell.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="createDisposableObject">if set to <c>true</c> create disposable object that is not cached.</param>
        /// <returns></returns>
        public TreeRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex, bool createDisposableObject)
        {
            if (createDisposableObject)
            {
                TreeModel model = treeControl.Model;
                RowColumnIndex cellRowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
                TreeStyleInfo modelStyle = GetModelStyle(cellRowColumnIndex, model);
                TreeRenderStyleInfo renderCellStyle = new TreeRenderStyleInfo(treeControl, modelStyle);
                TreePrepareRenderCellEventArgs e = new TreePrepareRenderCellEventArgs(cellRowColumnIndex, renderCellStyle);
                treeControl.RaisePrepareRenderCell(e);
                return renderCellStyle;
            }

            return GetRenderStyleInfo(rowIndex, columnIndex);
        }

        /// <summary>
        /// Gets the render cell style for a cell.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <returns></returns>
        public TreeRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex)
        {
            return GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        /// <summary>
        /// Gets the render cell style for a cell.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="createDisposableObject">if set to <c>true</c> create disposable object that is not cached.</param>
        /// <returns></returns>
        public TreeRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex, bool createDisposableObject)
        {
            return GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex, createDisposableObject);
        }

        /// <summary>
        /// Clears the cached rendered cell information for the specified cell.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        public void Clear(RowColumnIndex cellRowColumnIndex)
        {
            TreeRenderStyleInfo renderCellStyle;
            if (visibleCellStyles.TryGetValue(cellRowColumnIndex, out renderCellStyle))
            {
                renderCellStyle.Dispose();
                visibleCellStyles.Clear(cellRowColumnIndex);
            }

            //if (cellRowColumnIndex == treeControl.CurrentCell.CellRowColumnIndex)
            //{
            //    treeControl.CurrentCell.Renderer.Invalidate();
            //}
        }

        /// <summary>
        /// Clears the cached rendered cell information for all cells in the span.
        /// </summary>
        /// <param name="cellSpan">The cell span.</param>
        public void Clear(CellSpanInfoBase cellSpan)
        {
            visibleCellStyles.Remove(cellSpan);
        }

        /// <summary>
        /// Clears the cached rendered cell information for all visible cells.
        /// </summary>
        public void Clear()
        {
            visibleCellStyles.RemoveAll();
        }

        #region Insert and Remove Rows
        /// <summary>
        /// Update cell styles cell row and column index when rows were inserted.
        /// </summary>
        /// <param name="insertAtRowIndex">The row index.</param>
        /// <param name="count">The number of inserted rows.</param>
        public void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRows(insertAtRowIndex, count, null);
        }

        /// <summary>
        /// Update cell styles cell row and column index when rows were inserted.
        /// </summary>
        /// <param name="insertAtRowIndex">The row index.</param>
        /// <param name="count">The number of inserted rows.</param>
        /// <param name="moveCells">The move cells state used when moving rows.</param>
        public void InsertRows(int insertAtRowIndex, int count, TreeRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.InsertRows(insertAtRowIndex, count, moveCells);
        }

        /// <summary>
        /// Update cell styles cell row and column index when rows were removed.
        /// </summary>
        /// <param name="removeAtRowIndex">Index of the remove at row.</param>
        /// <param name="count">The number of inserted rows.</param>
        public void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRows(removeAtRowIndex, count, null);
        }

        /// <summary>
        /// Update cell styles cell row and column index when rows were removed.
        /// </summary>
        /// <param name="removeAtRowIndex">Index of the remove at row.</param>
        /// <param name="count">The number of inserted rows.</param>
        /// <param name="moveCells">The move cells state used when moving rows.</param>
        public void RemoveRows(int removeAtRowIndex, int count, TreeRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.RemoveRows(removeAtRowIndex, count, moveCells);
        }
        #endregion

        #region Insert and Remove Columns
        /// <summary>
        /// Update cell styles cell row and column index when columns were inserted.
        /// </summary>
        /// <param name="insertAtColumnIndex">The column index.</param>
        /// <param name="count">The number of inserted columns.</param>
        public void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumns(insertAtColumnIndex, count, null);
        }

        /// <summary>
        /// Update cell styles cell row and column index when columns were inserted.
        /// </summary>
        /// <param name="insertAtColumnIndex">The column index.</param>
        /// <param name="count">The number of inserted columns.</param>
        /// <param name="moveCells">The move cells state used when moving columns.</param>
        public void InsertColumns(int insertAtColumnIndex, int count, TreeRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.InsertColumns(insertAtColumnIndex, count, moveCells);
        }

        /// <summary>
        /// Update cell styles cell row and column index when columns were removed.
        /// </summary>
        /// <param name="removeAtColumnIndex">The column index.</param>
        /// <param name="count">The number of inserted columns.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumns(removeAtColumnIndex, count, null);
        }

        /// <summary>
        /// Update cell styles cell row and column index when columns were removed.
        /// </summary>
        /// <param name="removeAtColumnIndex">The column index.</param>
        /// <param name="count">The number of inserted columns.</param>
        /// <param name="moveCells">The move cells state used when moving columns.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count, TreeRenderStyleInfoDictionary moveCells)
        {
            visibleCellStyles.RemoveColumns(removeAtColumnIndex, count, moveCells);
        }
        #endregion

        internal void PrepareArrange()
        {
            // Create new TreeRenderStyleInfo objects for cells scrolled into view or
            // unload TreeRenderStyleInfo objects for cells scrolled out of view.
            unloadCellStylesDictionary = this.visibleCellStyles;
            visibleCellStyles = new TreeRenderStyleInfoDictionary();
        }

        internal void ConcludeArrange()
        {
            foreach (KeyValuePair<RowColumnIndex, TreeRenderStyleInfo> entry in unloadCellStylesDictionary)
            {
                // Do not unload style for cells that are still alive.
                if (treeControl.ArrangedCellUIElements.Contains(entry.Key))
                    visibleCellStyles.Add(entry.Key, entry.Value);
                else
                    entry.Value.Dispose();
            }

            unloadCellStylesDictionary.Clear();
            unloadCellStylesDictionary = null;
        }
    }
}
