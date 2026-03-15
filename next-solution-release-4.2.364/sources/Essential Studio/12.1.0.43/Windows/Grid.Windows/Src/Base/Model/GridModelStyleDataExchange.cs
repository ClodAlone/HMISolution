//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelStyleDataExchange.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Manages copy and paste of style objects in the grid. Allows you to add and remove selections, determine
    /// selection state of a specific cell, and more.
    /// </summary>
    public class GridModelStyleDataExchange : GridModelBound
    {
        /// <summary>
        /// Initializes a new <see cref="GridModelStyleDataExchange"/> object and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        public GridModelStyleDataExchange(GridModel model)
            : base(model)
        {
        }

        /// <overload>
        /// Creates a  <see cref="GridData"/> object and initializes it with style objects and covered cell information of a range of cells in the grid.
        /// </overload>
        /// <summary>
        /// Creates a  <see cref="GridData"/> object and initializes it with style objects and covered cell information of a range of cells in the grid.
        /// </summary>
        /// <param name="data">A placeholder for the <see cref="GridData"/> object that is created by the method.</param>
        /// <param name="range">The range of cells to be copied.</param>
        /// <param name="bLoadBaseStyles">True if information from base styles should also be copied; False if only
        /// the settings that were initialized for the cells should be copied.</param>
        /// <param name="dragDropFlags">A <see cref="GridDragDropFlags"/> with further options (currently ignored.)</param>
        /// <param name="rowCount">A placeholder where the number of copied rows is returned.</param>
        /// <param name="colCount">A placeholder where the number of copied columns is returned.</param>
        /// <returns>True if the operation completed successfully; False otherwise.</returns>
        public virtual bool CopyCellsToDataObject(out GridData data, GridRangeInfo range, bool bLoadBaseStyles, int dragDropFlags, out int rowCount, out int colCount)
        {
            GridRangeInfoList rangeList = new GridRangeInfoList();
            rangeList.Add(range);
            return CopyCellsToDataObject(out data, rangeList, bLoadBaseStyles, dragDropFlags, out rowCount, out colCount);
        }

        /// <summary>
        /// Creates a <see cref="GridData"/> object and initializes it with style objects and covered cell information of a range of cells in the grid.
        /// </summary>
        /// <param name="data">A placeholder for the <see cref="GridData"/> object that is created by the method.</param>
        /// <param name="rangeList">A collection with ranges of cells to be copied.</param>
        /// <param name="bLoadBaseStyles">(currently ignored.) True if information from base styles should also be copied; False if only
        /// the settings that were initialized for the cells should be copied.</param>
        /// <param name="dragDropFlags">(currently ignored.) A <see cref="GridDragDropFlags"/> with further options.</param>
        /// <param name="rowCount">A placeholder where the number of copied rows is returned.</param>
        /// <param name="colCount">A placeholder where the number of copied columns is returned.</param>
        /// <returns>True if the operation completed successfully; False otherwise.</returns>
        public bool CopyCellsToDataObject(out GridData data, GridRangeInfoList rangeList, bool bLoadBaseStyles, int dragDropFlags, out int rowCount, out int colCount)
        {
            // Store rows / columns indexes to process in an array.
            GridRangeInfoList rowRanges = rangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
            GridRangeInfoList colRanges = rangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);

            // Determine number of rows / cols to process.
            int numRows = 0;
            foreach (GridRangeInfo range in rowRanges)
            {
                numRows += range.Height;
            }

            int numCols = 0;
            foreach (GridRangeInfo range in colRanges)
            {
                numCols += range.Width;
            }

            int dwSize = numRows * numCols;

            int numRowsDone = 0, numColsDone = 0;

            // Status message, let the user abort the operation.
            using (OperationFeedback op = new OperationFeedback(Model))
            {
                op.Description = SR.GetString("GRID_IDM_COPYINTERNAL");
                op.AllowCancel = true;

                bool canceled = false;

                data = new GridData();

                GridRangeInfoList coveredRanges = new GridRangeInfoList();

                try
                {
                    data.RowCount = numRows;
                    data.ColCount = numCols;

                    // Fill pOldCellsArray row by row.
                    for (int rowindex = 0; !canceled && rowindex < rowRanges.Count; rowindex++)
                    {
                        for (int colindex = 0; !canceled && colindex < colRanges.Count; colindex++)
                        {
                            // REVIEW: does this also work with GridRangeInfoType.Rows and Cols?
                            GridRangeInfo intersectRange = GridRangeInfo.IntersectRange(rowRanges[rowindex], colRanges[colindex]);
                            GridRangeInfoList cl = Model.CoveredRanges.Ranges.GetRangesContained(intersectRange);

                            foreach (GridRangeInfo range in cl)
                            {
                                coveredRanges.Add(range.OffsetRange(-intersectRange.Top, -intersectRange.Left));
                            }
                        }

                        for (int nRow = rowRanges[rowindex].Top; nRow <= rowRanges[rowindex].Bottom; nRow++)
                        {
                            numColsDone = 0;
                            for (int colindex = 0; !canceled && colindex < colRanges.Count; colindex++)
                            {
                                for (int nCol = colRanges[colindex].Left; nCol <= colRanges[colindex].Right; nCol++)
                                {
                                    // Store styles in array, but allow user to abort.
                                    GridStyleInfo pStyle = new GridStyleInfo();
                                    int dwIndex;

                                    dwIndex = (numRowsDone * numCols) + numColsDone;

                                    if (GetClipboardStyleRowCol(nRow, nCol, pStyle, bLoadBaseStyles, dragDropFlags))
                                    {
                                        data[numRowsDone, numColsDone] = (GridStyleInfoStore)pStyle.Store;
                                    }

                                    // Check, if user pressed ESC to cancel.
                                    op.PercentComplete = (int)(dwIndex * 100 / dwSize);
                                    if (op.ShouldCancel)
                                    {
                                        throw new GridUserCanceledException();
                                    }

                                    numColsDone++;
                                }
                            }

                            numRowsDone++;
                        }
                    }

                    // REVIEW: shouldn't there be a flag to turn this on/off?
                    if (coveredRanges.Count > 0)
                    {
                        data.ExtendedInfo = coveredRanges;
                    }
                }
                catch (GridUserCanceledException ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    canceled = true;
                }
                //// You may use these internal attributes to determine
                //// the number of written rows and cols after this function
                //// returns.

                rowCount = numRows;
                colCount = numCols;

                return !canceled;
            }
        }

        internal bool GetClipboardStyleRowCol(int nRow, int nCol, GridStyleInfo style, bool bLoadBaseStyles, int dragDropFlags)
        {
            Model.GetCellInfo(nRow, nCol, style);

            //// Shall I compose the style with all its base style settings?
            //// TODO: if (bLoadBaseStyles)

            return true;
        }

        /// <summary>
        /// Initializes a given range of cells in a grid with style objects and covered ranges information from a <see cref="GridData"/> object.
        /// </summary>
        /// <param name="data">The <see cref="GridData"/> object with cell styles and covered ranges.</param>
        /// <param name="range">The destination range where cell information should be copied to.</param>
        /// <param name="bIgnoreDiffRange">True if difference in width and height of <paramref name="range"/> and
        /// the row and column count of <paramref name="data"/> should be ignored; False if a message box should be displayed.</param>
        /// <param name="dragDropFlags"><see cref="GridDragDropFlags"/> options let you specify if rows or columns can be appended 
        /// See <see cref="GridDragDropFlags.NoAppendRows"/> and <see cref="GridDragDropFlags.NoAppendCols"/>
        /// of the <see cref="GridDragDropFlags"/> class.</param>
        /// <returns>True if operation was successful; False otherwise.</returns>
        public bool PasteCellsFromDataObject(GridData data, GridRangeInfo range, bool bIgnoreDiffRange, int dragDropFlags)
        {
            ////int nValueType = 0; //pGrid.m_nExpressionValueType;

            bool canceled = false;

            int numRows = data.RowCount;
            int numCols = data.ColCount;

            int top = range.Top;
            int left = range.Left;
            int bottom = range.Top + numRows - 1;
            int right = range.Left + numCols - 1;

            if (!bIgnoreDiffRange && (bottom != range.Bottom || right != range.Right) &&
                !Model.CutPaste.OnPasteDiffRange())
            {
                canceled = true;
            }
            else
            {
                Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, "PasteCellsFromDataObject");
                try
                {
                    string s = SR.GetString("Grid_IDM_PASTEDATA");
                    Model.CommandStack.BeginTrans(s);

                    using (OperationFeedback op = new OperationFeedback(Model))
                    {
                        op.Description = SR.GetString("GRID_IDM_PASTINGDATA");
                        op.AllowRollback = true;
                        // op.SetLockedState(true);

                        // Store and deactivate current cell.
                        Model.ConfirmChanges();

                        if (bottom > Model.RowCount)
                        {
                            if (GridUtil.IsNotSet(dragDropFlags, GridDragDropFlags.NoAppendRows))
                            {
                                Model.RowCount = bottom;
                            }

                            bottom = Model.RowCount;
                        }

                        if (right > Model.ColCount)
                        {
                            if (GridUtil.IsNotSet(dragDropFlags, GridDragDropFlags.NoAppendCols))
                            {
                                Model.ColCount = right;
                            }

                            right = Model.ColCount;
                        }

                        GridRangeInfo r = GridRangeInfo.Cells(top, left, bottom, right);

                        Model.CoveredRanges.Remove(Model.CoveredRanges.Ranges.GetOuterRange(r));

                        GridRangeInfoList coveredRanges = data.ExtendedInfo as GridRangeInfoList;
                        if (coveredRanges != null)
                        {
                            foreach (GridRangeInfo rc in coveredRanges)
                            {
                                GridRangeInfo rcOffset = rc.OffsetRange(top, left);
                                if (!Model.CoveredRanges.Ranges.AnyRangeIntersects(rcOffset))
                                {
                                    Model.CoveredRanges.Add(rcOffset);
                                }
                            }
                        }

                        int index = 0;
                        GridStyleInfo[] pCells = new GridStyleInfo[r.Width * r.Height];
                        for (int rowIndex = r.Top; rowIndex <= r.Bottom; rowIndex++)
                        {
                            if (model.ActiveGridView != null && rowIndex > Model.ActiveGridView.ViewLayout.VisibleRows)
                            {
                                model.ScrollCellInView(GridRangeInfo.Row(rowIndex), GridScrollCurrentCellReason.Activate);
                            }
                            for (int colIndex = r.Left; colIndex <= r.Right; colIndex++)
                            {
                                if (model.ActiveGridView != null && colIndex > model.ActiveGridView.ViewLayout.VisibleCols)
                                {
                                    model.ScrollCellInView(GridRangeInfo.Col(colIndex), GridScrollCurrentCellReason.Activate);
                                }
                                pCells[index++] = new GridStyleInfo(data[rowIndex - r.Top, colIndex - r.Left]);
                            }
                        }

                        if (r.Top <= Model.RowCount && r.Left <= Model.ColCount)
                        {
                            canceled = !Model.ChangeCells(r, pCells, StyleModifyType.Copy);
                        }
                        else
                        {
                            canceled = true;
                        }

                        canceled |= op.RollbackConfirmed;
                    }
                }
                finally
                {
                    Model.EndUpdate();
                }
            }

            if (canceled)
            {
                Model.CommandStack.Rollback();
            }
            else
            {
                Model.CommandStack.CommitTrans();
            }

            return !canceled;
        }
    }
}
