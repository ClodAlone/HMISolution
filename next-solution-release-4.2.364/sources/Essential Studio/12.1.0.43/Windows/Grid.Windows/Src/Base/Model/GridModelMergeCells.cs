//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelMergeCells.cs" company="syncfusion">
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
    /// This class manages merged cell ranges for a grid.
    /// </summary>
    /// <remarks>
    /// You access this class from a grid with the <see cref="GridModel.MergeCells"/>
    /// property of a <see cref="GridModel"/> instance.
    /// <para/>
    /// Merged ranges are saved in two separate collections:
    /// <para/>
    /// The first collection is <see cref="GridRangeInfoList"/>
    /// which allows quick enumeration through all merged cell ranges in the grid. This is good when merged ranges
    /// need to be recalculated because rows or column have been inserted, moved, or removed.
    /// <para/>
    /// The second collection is <see cref="GridSpanCellPool"/> that is optimized to look up if a specific cell
    /// is part of a merged range.
    /// </remarks>
    [Serializable]
    public class GridModelMergeCells : GridModelBound, ISerializable
    {
        // Constructor.

        /// <overload>
        /// Initializes a new <see cref="GridModelMergeCells"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridModelMergeCells"/> object and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        public GridModelMergeCells(GridModel model)
            : base(model)
        {
            model.Options.MergeCellsModeChanged += new EventHandler(ModelMergeCellsModeChanged);
            SetMergeCellsMode(model.Options.MergeCellsMode);
        }

        /// <summary>
        /// Initializes a new <see cref="GridModelMergeCells"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridModelMergeCells(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            mergeSpanCellPoolRowsInColumn = (GridSpanCellPool)info.GetValue("PoolRowsInColumn", typeof(GridSpanCellPool));
            mergeSpanCellPoolColumnsInRow = (GridSpanCellPool)info.GetValue("PoolColumnsInRow", typeof(GridSpanCellPool));
            mergeDelayedRangePool = (GridDelayedRangePool)info.GetValue("Delayed", typeof(GridDelayedRangePool));
        }

        void ModelMergeCellsModeChanged(object sender, EventArgs e)
        {
            SetMergeCellsMode(model.Options.MergeCellsMode);
        }
        //
        //        protected override void Dispose(bool disposing)
        //        {
        //            if (disposing)
        //            {
        //                model.Options.MergeCellsModeChanged -= new EventHandler(ModelMergeCellsModeChanged);
        //            }
        //            base.Dispose(disposing);
        //        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridModelMergeCells"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("PoolRowsInColumn", mergeSpanCellPoolRowsInColumn); // GridSpanCellPool
            info.AddValue("PoolColumnsInRow", mergeSpanCellPoolColumnsInRow); // GridSpanCellPool
            info.AddValue("Delayed", mergeDelayedRangePool); // GridDelayedRangePool
        }
        
        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        /// <param name="e">A GridRowColSizeChangedEventArgs that contains the event data. </param>
        internal void OnChanged(GridMergeCellsChangedEventArgs e)
        {
            try
            {
                Model.RaiseMergeCellsChanged(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Marks the specified range of cells to be re-evaluated at a later time.
        /// </summary>
        /// <param name="range">The range to be re-evaluated.</param>
        public void DelayMergeCells(GridRangeInfo range)
        {
            DelayMergeCellsInt(range, GridConstants.Undefined, GridConstants.Undefined);
        }

        internal GridSpanCellPool GetMergeCellsPoolColumnsInRow()
        {
            return this.mergeSpanCellPoolColumnsInRow;
        }

        internal GridSpanCellPool GetMergeCellsPoolRowsInColumn()
        {
            return this.mergeSpanCellPoolRowsInColumn;
        }

        internal void InsertRows(int index, int count)
        {
            if (mergeCellsMode != GridMergeCellsMode.None)
            {
                mergeSpanCellPoolColumnsInRow.InsertRows(index, count);
                mergeSpanCellPoolRowsInColumn.InsertCols(index, count);
            }
        }

        internal void InsertCols(int index, int count)
        {
            if (mergeCellsMode != GridMergeCellsMode.None)
            {
                mergeSpanCellPoolColumnsInRow.InsertCols(index, count);
                mergeSpanCellPoolRowsInColumn.InsertRows(index, count);
            }
        }

        internal void RemoveRows(int from, int to)
        {
            if (mergeCellsMode != GridMergeCellsMode.None)
            {
                mergeSpanCellPoolColumnsInRow.RemoveRows(from, to);
                mergeSpanCellPoolRowsInColumn.RemoveCols(from, to);
            }
        }

        internal void RemoveCols(int from, int to)
        {
            if (mergeCellsMode != GridMergeCellsMode.None)
            {
                mergeSpanCellPoolColumnsInRow.RemoveCols(from, to);
                mergeSpanCellPoolRowsInColumn.RemoveRows(from, to);
            }
        }

        internal void MoveRows(int from, int last, int dest)
        {
            if (mergeCellsMode != GridMergeCellsMode.None)
            {
                mergeSpanCellPoolColumnsInRow.MoveRows(from, last, dest);
                mergeSpanCellPoolRowsInColumn.MoveCols(from, last, dest);
            }
        }

        internal void MoveCols(int from, int last, int dest)
        {
            if (mergeCellsMode != GridMergeCellsMode.None)
            {
                mergeSpanCellPoolColumnsInRow.MoveCols(from, last, dest);
                mergeSpanCellPoolRowsInColumn.MoveRows(from, last, dest);
            }
        }

        [NonSerialized]
        internal bool lockEvaluateMergeCells = false;
        internal GridMergeCellsMode mergeCellsMode = GridMergeCellsMode.None;
        internal GridSpanCellPool mergeSpanCellPoolRowsInColumn = null;
        internal GridSpanCellPool mergeSpanCellPoolColumnsInRow = null;
        internal GridDelayedRangePool mergeDelayedRangePool = null;
        
        internal void SetMergeCellsMode(GridMergeCellsMode nMode)
        {
            GridModel model = this.Model;

            if (nMode != mergeCellsMode)
            {
                mergeCellsMode = nMode;

                // Clean up int/*merge*/ cells state
                mergeDelayedRangePool = new GridDelayedRangePool();
                mergeSpanCellPoolColumnsInRow = null;
                mergeSpanCellPoolRowsInColumn = null;

                switch (nMode & (GridMergeCellsMode.BeforeDisplayCalculation | GridMergeCellsMode.OnDemandCalculation))
                {
                    case GridMergeCellsMode.None:
                        break;

                    case GridMergeCellsMode.BeforeDisplayCalculation:
                        // Reinit int/*merge*/ cells state
                        mergeSpanCellPoolRowsInColumn = new GridSpanCellPool();
                        mergeSpanCellPoolColumnsInRow = new GridSpanCellPool();
                        break;

                    case GridMergeCellsMode.OnDemandCalculation:
                        // Reinit int/*merge*/ cells state
                        mergeSpanCellPoolRowsInColumn = new GridSpanCellPool();
                        mergeSpanCellPoolColumnsInRow = new GridSpanCellPool();

                        // Force all cells to be recalculated
                        DelayMergeCells(GridRangeInfo.Table());
                        break;
                }
            }
        }

        /// <summary>
        /// Marks the specified range of cells to be re-evaluated at a later time if text merges into neighboring cells.
        /// </summary>
        /// <param name="range">The range to be re-evaluated.</param>        
        /// <param name="nMaxRows">The current row count of the grid or GridConstants.Undefined
        /// if all rows are affected.</param>
        /// <param name="nMaxCols">The current column count of the grid or GridConstants.Undefined
        /// if all columns are affected.</param>
        internal void DelayMergeCellsInt(GridRangeInfo range, int nMaxRows, int nMaxCols)
        {
            GridModel model = this.Model;

            if ((mergeCellsMode & GridMergeCellsMode.OnDemandCalculation) == 0)
            {
                return;
            }
#if DEBUG
            if (Switches.MergeCells.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(range, nMaxRows, nMaxCols);
            }
#else

            ;
#endif
            //// Expand range to current dimension.
            ////GridRangeInfo rg = range.IntersectRange(model.GridCellsRange);
            GridRangeInfo rg = range.ExpandRange(0, 0, model.RowCount, model.ColCount);

            //// TODO: Expand over Hidden rows and columns?

            if (!rg.IsEmpty)
            {
                //// Display
                mergeDelayedRangePool.DelayRange(rg);

                if (nMaxRows != GridConstants.Undefined)
                {
                    mergeDelayedRangePool.SetRowCount(nMaxRows);
                }

                if (nMaxCols != GridConstants.Undefined)
                {
                    mergeDelayedRangePool.SetColCount(nMaxCols);
                }
            }
        }
        
        /// <overload>
        /// Checks the specified range if any cells have been previously marked with <see cref="DelayMergeCells"/> 
        /// to be re-evaluated.
        /// </overload>
        /// <summary>
        /// Checks the specified range if any cells have been previously marked with <see cref="DelayMergeCells"/> 
        /// to be re-evaluated.
        /// </summary>
        /// <param name="range">The range to be re-evaluated.</param>
        /// <returns>True if merge state for any cell in the given range was changed.</returns>
        public bool EvaluateMergeCells(GridRangeInfo range)
        {
            GridRangeInfo r = range;
            return EvaluateMergeCells(range, ref r);
        }

        /// <summary>
        /// Checks the specified range if any cells have been previously marked with <see cref="DelayMergeCells"/>
        /// to be re-evaluated and returns a range that holds all affected cells.
        /// </summary>
        /// <param name="range">The range to be re-evaluated.</param>
        /// <param name="boundsInfo">The range with all affected cells including ranges that intersected with
        /// <paramref name="range"/>.
        /// </param>
        /// <returns>True if merge state for any cell in the given range was changed.</returns>
        internal bool EvaluateMergeCells(GridRangeInfo range, ref GridRangeInfo boundsInfo)
        {
            GridModel model = this.Model;
            // Is evaluation of merge cells temporary or completely turned off?
            if (lockEvaluateMergeCells)
            {
                return false;
            }

#if DEBUG
            Trace.WriteLineIf(Switches.MergeCells.TraceVerbose, "EvaluateMergeCells(" + range.ToString() + ")");
#endif
            // Different modes.
            switch (mergeCellsMode & (GridMergeCellsMode.OnDemandCalculation | GridMergeCellsMode.BeforeDisplayCalculation))
            {
                case GridMergeCellsMode.None:
                    return false;

                case GridMergeCellsMode.OnDemandCalculation:
                    return EvalDelayedMergeCells(range, ref boundsInfo);

                case GridMergeCellsMode.BeforeDisplayCalculation:
                    return EvalAllMergeCells(range, ref boundsInfo);

                default:
#if DEBUG
                    Trace.WriteLineIf(Switches.MergeCells.TraceError, "Warning: mergeCellsMode invalid");
#endif
                    return false;
            }
        }

        //// EvalAllMergeCells
        ////
        //// Evaluates all cells from the given range if they want to int/*merge*/
        //// other cells.

        bool EvalAllMergeCells(GridRangeInfo range, ref GridRangeInfo boundsInfo)
        {
            GridModel model = this.Model;

            range = range.IntersectRange(model.GridCellsRange);
            bool success = false;
            for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
            {
                // TODO: this.mergeCellsMode & GridMergeCellsMode.SkipHiddencells 
                if (!model.HideRows[rowIndex])
                {
                    for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                    {
                        if (!model.HideCols[colIndex])
                        {
                            success |= MergeCell(rowIndex, colIndex, ref boundsInfo);
                        }
                    }
                }
            }

            return success;
        }

        internal bool EvaluateMergeCells(GridControlBase pGrid, GridRangeInfo range, int nfr, int nfc, int nTopRow, int nLeftCol, int nLastRow, int nLastCol, ref GridRangeInfo prgBoundary)
        {
#if DEBUG
            Trace.WriteLineIf(Switches.MergeCells.TraceVerbose, "Begin EvaluateMergeCells(" + range.ToString() + ")");

            Trace.Indent();
#endif
            if (!range.IsCells)
            {
                range = range.ExpandRange(0, 0, pGrid.Model.RowCount, pGrid.Model.ColCount);
            }

            GridRangeInfo rgFrozenTL, rgFrozenTop, rgFrozenLeft, rgCells;

            rgFrozenTL = GridRangeInfo.IntersectRange(GridRangeInfo.InternalCells(0, 0, nfr, nfc), range);
            rgFrozenTop = GridRangeInfo.IntersectRange(GridRangeInfo.InternalCells(0, nLeftCol, nfr, nLastCol), range);
            rgFrozenLeft = GridRangeInfo.IntersectRange(GridRangeInfo.InternalCells(nTopRow, 0, nLastRow, nfc), range);
            rgCells = GridRangeInfo.IntersectRange(GridRangeInfo.InternalCells(nTopRow, nLeftCol, nLastRow, nLastCol), range);

            bool bEval = false;

            if (!rgFrozenTL.IsEmpty)
            {
                bEval |= pGrid.Model.MergeCells.EvaluateMergeCells(rgFrozenTL, ref prgBoundary);
            }

            if (!rgFrozenTop.IsEmpty)
            {
                bEval |= pGrid.Model.MergeCells.EvaluateMergeCells(rgFrozenTop, ref prgBoundary);
            }

            if (!rgFrozenLeft.IsEmpty)
            {
                bEval |= pGrid.Model.MergeCells.EvaluateMergeCells(rgFrozenLeft, ref prgBoundary);
            }

            if (!rgCells.IsEmpty)
            {
                bEval |= pGrid.Model.MergeCells.EvaluateMergeCells(rgCells, ref prgBoundary);
            }

#if DEBUG
            Trace.Unindent();
            Trace.WriteLineIf(Switches.MergeCells.TraceVerbose, "End EvaluateMergeCells(" + range.ToString() + ")");
#endif
            return bEval;
        }

        // EvalDelayedMergeCells
        //
        // Evaluates those cells from the given range which
        // were previously marked as delayed with DelayMergeCells.
        ////ref GridRangeInfo boundsInfo // = null
        bool EvalDelayedMergeCells(GridRangeInfo range, ref GridRangeInfo boundsInfo)
        {
            GridModel model = this.Model;
#if DEBUG
            Trace.WriteLineIf(Switches.MergeCells.TraceVerbose, String.Format("EvalDelayedMergeCells({0})", range));
#endif
            if (!range.IsCells)
            {
                range = range.ExpandRange(0, 0, model.RowCount, model.ColCount);
            }

            ////range = range.IntersectRange(model.GridCellsRange);
            bool success = false;
            int[] dwColStart;
            int[] dwColEnd;
            GridDelayedRangePool pPool = mergeDelayedRangePool;

            if (pPool.EvalRows(range, out dwColStart, out dwColEnd))
            {
                for (int nIndex = 0; nIndex < dwColStart.Length; nIndex++)
                {
                    int rowIndex = (int)nIndex + range.Top;
                    if (dwColEnd[nIndex] >= 0 && !model.Rows.Hidden[rowIndex])
                    {
#if DEBUG
                        Trace.WriteLineIf(Switches.MergeCells.TraceVerbose, String.Format("Row = {0}: {1}, {2}", rowIndex, dwColStart[nIndex], dwColEnd[nIndex]));
#endif

                        for (int colIndex = dwColStart[nIndex]; colIndex <= dwColEnd[nIndex]; colIndex++)
                        {
                            if (!model.Cols.Hidden[colIndex])
                            {
                                success |= MergeCell(rowIndex, colIndex, ref boundsInfo);
                            }
                        }
                    }
                }
            }

            return success;
        }

        internal bool SetMergedCellsRowCol(GridMergeCellDirection mergeDirection, int rowIndex, int colIndex, int toRowIndex, int toColIndex)
        {
            GridModel model = this.Model;
#if DEBUG
            Trace.WriteLineIf(Switches.MergeCells.TraceVerbose, String.Format("SetMergedCellsRowCol({0})", GridRangeInfo.Cells(rowIndex, colIndex, toRowIndex, toColIndex)));
#endif

            bool success = false;

            Debug.Assert(mergeCellsMode != GridMergeCellsMode.None);
            Debug.Assert(toRowIndex >= rowIndex && toColIndex >= colIndex);

            int savedRowIndex = rowIndex,
                savedColIndex = colIndex;

            GridRangeInfo rgMerged;
            if (Find(mergeDirection, rowIndex, colIndex, out rgMerged))
            {
                savedRowIndex = rgMerged.Bottom;
                savedColIndex = rgMerged.Right;
                success = true;
                ResetMergedCells(mergeDirection, rgMerged);
            }

            bool bSet = toRowIndex != rowIndex || toColIndex != colIndex;

            // SyncfusionCommand
            if ((bSet && StoreMergedCells(mergeDirection, rowIndex, colIndex, toRowIndex, toColIndex))
                || savedRowIndex != toRowIndex || savedColIndex != toColIndex)
            {
                OnChanged(new GridMergeCellsChangedEventArgs(rgMerged, true));
                success = true;
            }

            return success;
        }

        bool ResetMergedCells(GridMergeCellDirection mergeDirection, GridRangeInfo range)
        {
            GridModel model = this.Model;
#if DEBUG
            Trace.WriteLineIf(Switches.MergeCells.TraceVerbose, String.Format("ResetMergedCells({0})", range));
#endif
            //// Check if model is Read-only.
            //// if (Param.lockReadOnly && (IsReadOnly || StandardStyle().GetReadOnly()))
            ////    return false;

            if (mergeCellsMode == GridMergeCellsMode.None)
            {
                return false;
            }

            GridSpanCellPool pool;
            if (mergeDirection == GridMergeCellDirection.ColumnsInRow)
            {
                pool = GetMergeCellsPoolColumnsInRow();
            }
            else
            {
                pool = GetMergeCellsPoolRowsInColumn();
            }

            return pool.ResetSpanCells(range);
        }

        bool StoreMergedCells(GridMergeCellDirection mergeDirection, int rowIndex, int colIndex, int toRowIndex, int toColIndex)
        {
            GridModel model = this.Model;
#if DEBUG
            if (Switches.MergeCells.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(mergeDirection, rowIndex, colIndex, toRowIndex, toColIndex);
            }
#else
            ;
#endif            
            //// Check if model is Read-only.
            //// if (Param.lockReadOnly && (IsReadOnly || StandardStyle().GetReadOnly()))
            ////    return false;

            if (mergeCellsMode == GridMergeCellsMode.None)
            {
                return false;
            }

            GridRangeInfo range = GridRangeInfo.Cells(rowIndex, colIndex, toRowIndex, toColIndex);

            GridSpanCellPool pool;
            if (mergeDirection == GridMergeCellDirection.ColumnsInRow)
            {
                pool = GetMergeCellsPoolColumnsInRow();
            }
            else
            {
                pool = GetMergeCellsPoolRowsInColumn();
            }

            try
            {
                pool.ResetSpanCells(range, true);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                return false;
            }

            return pool.StoreSpanCells(range);
        }

        /// <summary>
        /// Returns a <see cref="GridRangeInfo"/> object that indicates the merge cell's range for the specified cell position
        /// or False if there are no merge cells in range for the given cell position.
        /// </summary>
        /// <param name="mergeDirection">Specifies whether cells merged in row or column (or both) are asked for.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">The <see cref="GridRangeInfo"/> where the found merge cell's range is returned.</param>
        /// <returns>True if a merge cell's range is at the specified cell position; False if not.</returns>
        public bool Find(GridMergeCellDirection mergeDirection, int rowIndex, int colIndex, out GridRangeInfo range)
        {
            range = GridRangeInfo.Cell(rowIndex, colIndex);

            if (mergeCellsMode == GridMergeCellsMode.None)
            {
                return false;
            }

            if ((mergeDirection & GridMergeCellDirection.ColumnsInRow) != 0
                && GetMergeCellsPoolColumnsInRow().GetSpanCellsRowCol(rowIndex, colIndex, out range))
            {
                return true;
            }

            if ((mergeDirection & GridMergeCellDirection.RowsInColumn) != 0
                && GetMergeCellsPoolRowsInColumn().GetSpanCellsRowCol(rowIndex, colIndex, out range))
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Returns a <see cref="GridRangeInfo"/> object that indicates the merge cell's range for the specified cell position
        /// or <see cref="GridRangeInfo.Empty"/> if there are no merge cells in range for the given cell position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>A reference to range if a merge cell's range is at the specified cell position or
        /// <see cref="GridRangeInfo.Empty"/> if not.</returns>
        public GridRangeInfo FindRange(int rowIndex, int colIndex)
        {
            GridRangeInfo foundRange;
            if (Find(GridMergeCellDirection.Both, rowIndex, colIndex, out foundRange))
            {
                colIndex = foundRange.Right + 1;
                return foundRange;
            }
        
            return GridRangeInfo.Empty;
        }

        /// <summary>
        /// Combines all merge cells' ranges that intersect into one outer range that spans over all found ranges.
        /// </summary>
        /// <param name="range">The original range. </param>
        /// <returns>The <see cref="GridRangeInfo"/> with the outer range.</returns>
        public GridRangeInfo Merge(GridRangeInfo range)
        {
            GridModel model = this.Model;

            if (mergeCellsMode != GridMergeCellsMode.None)
            {
                GridRangeInfo savedRange = range;
                for (int rowIndex = savedRange.Top; rowIndex <= savedRange.Bottom; rowIndex++)
                {
                    for (int colIndex = savedRange.Left; colIndex <= savedRange.Right; colIndex++)
                    {
                        GridRangeInfo foundRange;
                        if (Find(GridMergeCellDirection.ColumnsInRow, rowIndex, colIndex, out foundRange))
                        {
                            colIndex = foundRange.Right + 1;
                            range = GridRangeInfo.UnionRange(foundRange, range);
                        }
                    }
                }

                savedRange = range;

                for (int rowIndex = savedRange.Top; rowIndex <= savedRange.Bottom; rowIndex++)
                {
                    for (int colIndex = savedRange.Left; colIndex <= savedRange.Right; colIndex++)
                    {
                        GridRangeInfo foundRange;
                        if (Find(GridMergeCellDirection.RowsInColumn, rowIndex, colIndex, out foundRange))
                        {
                            colIndex = foundRange.Right + 1;
                            range = GridRangeInfo.UnionRange(foundRange, range);
                        }
                    }
                }
            }
            
            return range;
        }

        /// <summary>
        /// Determines if a specific cell supports merging or being flooded by a neighboring cell.
        /// </summary>
        /// <param name="cellModel">The cellModel</param>
        /// <param name="rowIndex">The rowIndex</param>
        /// <param name="colIndex">The colIndex</param>
        /// <param name="style">The GridStyleInfo</param>
        /// <param name="query">The GridMergeCellDirection</param>
        /// <returns>returns boolean value to indicate merge cell</returns>
        internal bool CanMergeCell(GridCellModelBase cellModel, int rowIndex, int colIndex, GridStyleInfo style, GridMergeCellDirection query)
        {
            ////                TraceUtil.TraceCurrentMethodInfoIf(Switches.GridModel.TraceVerbose, cellModel, rowIndex, colIndex, query, style);

            GridModel model = this.Model;

            if (mergeCellsMode == GridMergeCellsMode.None)
            {
                return false;
            }

            GridRangeInfo rgCovered;
            if (model.CoveredRanges.Find(rowIndex, colIndex, out rgCovered))
            {
                return false;
            }

            if (model.HideCols[colIndex] || model.HideRows[rowIndex])
            {
                return false;
            }

            return (style.MergeCell & query) != 0
                && cellModel.OnQueryCanMergeCell(rowIndex, colIndex, style, query);
        }

        internal bool CanMergeCells(GridStyleInfo style1, GridStyleInfo style2)
        {
            GridMergeCellDirection md = style1.MergeCell & style2.MergeCell;

            if (md != GridMergeCellDirection.None)
            {
                GridQueryCanMergeCellsEventArgs qcme = new GridQueryCanMergeCellsEventArgs(style1, style2, false);

                Model.RaiseQueryCanMergeCells(qcme);

                if (qcme.Handled)
                {
                    return qcme.Result;
                }

                if (style1.CellValue == null || (style1.CellType== GridCellTypeName.Header && GridUtil.IsEmpty(style1.Text)))
                {
                    return false;
                }

                return style1.Text == style2.Text;
            }

            return false;
        }

        //// MergeCell
        ////
        //// Evaluates the given cell.
        ////
        //// boundsInfo will be extended when the cell has become a merge
        //// cell or if it was a merge cell.

        bool MergeCell(int rowIndex, int colIndex, ref GridRangeInfo boundsInfo)
        {
            GridModel model = this.Model;

            if (mergeCellsMode == GridMergeCellsMode.None)
            {
                return false;
            }

            ////            if (colIndex <= model.Cols.HeaderCount || rowIndex <= model.Rows.HeaderCount)
            ////                return false;

            //// Check if there is a cell to the left which could int/*merge*/ over this cell.
            GridStyleInfo style = model[rowIndex, colIndex];
            GridCellModelBase cellModel = style.CellModel;

            int rowCount = Model.RowCount;
            int colCount = Model.ColCount;

            bool b1 = false;
            bool b2 = false;
            bool b3 = false;
            bool b4 = false;

            if ((this.mergeCellsMode & GridMergeCellsMode.MergeColumnsInRow) != 0)
            {
                //// Check columns to the left.
                int nCol1 = colIndex - 1;
                //// TODO: hidden rows columns

                b1 = InternalCheckMergeColumnsInRow(rowIndex, nCol1, colIndex, ref boundsInfo);
                if (b1 && !boundsInfo.IsEmpty)
                {
                    int nc = nCol1;
                    while (nc > 0 && InternalCheckMergeColumnsInRow(rowIndex, nc - 1, nc, ref boundsInfo))
                    {
                        nc--;
                    }
                }

                // Check columns to the right.
                int nCol2 = colIndex + 1;
                if (nCol2 <= model.ColCount)
                {
                    b2 = InternalCheckMergeColumnsInRow(rowIndex, colIndex, nCol2, ref boundsInfo);
                    if (b2 && !boundsInfo.IsEmpty)
                    {
                        int nc = nCol2;
                        while (nc < colCount && InternalCheckMergeColumnsInRow(rowIndex, nc, nc + 1, ref boundsInfo))
                        {
                            nc++;
                        }
                    }
                }
            }

            if ((this.mergeCellsMode & GridMergeCellsMode.MergeRowsInColumn) != 0)
            {
                //// Check above rows.
                int nRow1 = rowIndex - 1;
                //// TODO: hidden rows columns

                b3 = InternalCheckMergeRowsInColumn(colIndex, nRow1, rowIndex, ref boundsInfo);
                if (b3 && !boundsInfo.IsEmpty)
                {
                    int nr = nRow1; ////boundsInfo.Top;
                    while (nr > 0 && InternalCheckMergeRowsInColumn(colIndex, nr - 1, nr, ref boundsInfo))
                    {
                        nr--;
                    }
                }

                // Check rows below.
                int nRow2 = rowIndex + 1;
                if (nRow2 <= model.RowCount)
                {
                    b4 = InternalCheckMergeRowsInColumn(colIndex, rowIndex, nRow2, ref boundsInfo);
                    if (b4 && !boundsInfo.IsEmpty)
                    {
                        int nr = nRow2; ////boundsInfo.Bottom;
                        while (nr < rowCount && InternalCheckMergeRowsInColumn(colIndex, nr, nr + 1, ref boundsInfo))
                        {
                            nr++;
                        }
                    }
                }
            }
            
            return b1 || b2 || b3 || b4;
        }

        //// InternalCheckMergeColumnsInRow
        ////
        //// Implementation for MergeCell

        bool InternalCheckMergeColumnsInRow(int rowIndex, int colIndex1, int colIndex2, ref GridRangeInfo boundsInfo)
        {
#if DEBUG
            if (Switches.MergeCells.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex1, colIndex2, boundsInfo);
            }
#else
            ;
#endif

            GridModel model = this.Model;
            bool success = false;

            GridRangeInfo rgMerged1 = GridRangeInfo.Empty;
            bool isMerged1 = colIndex1 >= 0 && Find(GridMergeCellDirection.ColumnsInRow, rowIndex, colIndex1, out rgMerged1);

            GridRangeInfo rgMerged2 = GridRangeInfo.Empty;
            bool isMerged2 = colIndex2 >= 0 && Find(GridMergeCellDirection.ColumnsInRow, rowIndex, colIndex2, out rgMerged2);

            // Get Style and Control
            GridStyleInfo style1 = model[rowIndex, colIndex1];
            GridStyleInfo style2 = model[rowIndex, colIndex2];

            // Is this cell mergeable?
            bool canMerge1 = colIndex1 >= 0 && CanMergeCell(style1.CellModel, rowIndex, colIndex1, style1, GridMergeCellDirection.ColumnsInRow);
            bool canMerge2 = colIndex2 >= 0 && CanMergeCell(style2.CellModel, rowIndex, colIndex2, style2, GridMergeCellDirection.ColumnsInRow);

            bool canMerge = canMerge1 && canMerge2 && CanMergeCells(style1, style2);

            if (canMerge)
            {
                if (rgMerged2.Right != rgMerged1.Right)
                {
                    if (isMerged2)
                    {           // Remove covering.
                        success = SetMergedCellsRowCol(GridMergeCellDirection.ColumnsInRow, rgMerged2.Top, rgMerged2.Left, rgMerged2.Top, rgMerged2.Left);
                    }

                    // Apply new covering.
                    success |= SetMergedCellsRowCol(
                        GridMergeCellDirection.ColumnsInRow,
                        rgMerged1.Top,
                        Math.Min(rgMerged1.Left, rgMerged2.Right),
                        rgMerged1.Bottom,
                        Math.Max(rgMerged1.Left, rgMerged2.Right));
                }
            }
            else
            {
                if (isMerged1 && rgMerged1.Right >= colIndex2)
                {
                    success = SetMergedCellsRowCol(GridMergeCellDirection.ColumnsInRow, rgMerged2.Top, rgMerged2.Left, rgMerged2.Top, rgMerged2.Left);

                    if (colIndex1 > rgMerged1.Left)
                    {
                        success |= SetMergedCellsRowCol(GridMergeCellDirection.ColumnsInRow, rgMerged1.Top, rgMerged1.Left, rgMerged1.Top, colIndex1);
                    }
                }

                if (isMerged2 && rgMerged2.Left <= colIndex1)
                {
                    if (rgMerged1.Right != rgMerged2.Right)
                    {
                        success |= SetMergedCellsRowCol(GridMergeCellDirection.ColumnsInRow, rgMerged2.Top, rgMerged2.Left, rgMerged2.Top, rgMerged2.Left);
                    }

                    if (colIndex2 < rgMerged2.Right)
                    {
                        success |= SetMergedCellsRowCol(GridMergeCellDirection.ColumnsInRow, rgMerged2.Top, colIndex2, rgMerged2.Top, rgMerged2.Right);
                    }
                }
            }

            if (success)
            {
                if (boundsInfo.IsEmpty)
                {
                    boundsInfo = GridRangeInfo.Cell(rowIndex, colIndex2);
                }

                boundsInfo = GridRangeInfo.Cells(
                    Math.Min(boundsInfo.Top, rgMerged1.Top),
                    Math.Min(boundsInfo.Left, rgMerged1.Left),
                    Math.Max(boundsInfo.Bottom, rgMerged1.Bottom),
                    Math.Max(boundsInfo.Right, rgMerged2.Right));
            }

            return success;
        }

        //// InternalCheckMergeRowsInColumn
        ////
        //// Implementation for MergeCell

        bool InternalCheckMergeRowsInColumn(int colIndex, int rowIndex1, int rowIndex2, ref GridRangeInfo boundsInfo)
        {
#if DEBUG
            if (Switches.MergeCells.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(colIndex, rowIndex1, rowIndex2, boundsInfo);
            }
#else
            ;
#endif
            GridModel model = this.Model;
            bool success = false;

            GridRangeInfo rgMerged1 = GridRangeInfo.Empty;
            bool isMerged1 = rowIndex1 >= 0 && Find(GridMergeCellDirection.RowsInColumn, rowIndex1, colIndex, out rgMerged1);

            GridRangeInfo rgMerged2 = GridRangeInfo.Empty;
            bool isMerged2 = rowIndex2 >= 0 && Find(GridMergeCellDirection.RowsInColumn, rowIndex2, colIndex, out rgMerged2);

            // Get Style and Control.
            GridStyleInfo style1 = model[rowIndex1, colIndex];
            GridStyleInfo style2 = model[rowIndex2, colIndex];

            // Is this cell mergeable?
            bool canMerge1 = rowIndex1 >= 0 && CanMergeCell(style1.CellModel, rowIndex1, colIndex, style1, GridMergeCellDirection.RowsInColumn);
            bool canMerge2 = rowIndex2 >= 0 && CanMergeCell(style2.CellModel, rowIndex2, colIndex, style2, GridMergeCellDirection.RowsInColumn);

            bool canMerge = canMerge1 && canMerge2 && CanMergeCells(style1, style2);

            if (canMerge)
            {
                if (rgMerged2.Bottom != rgMerged1.Bottom)
                {
                    if (isMerged2)
                    {       // Remove covering.
                        success = SetMergedCellsRowCol(GridMergeCellDirection.RowsInColumn, rgMerged2.Top, rgMerged2.Left, rgMerged2.Top, rgMerged2.Left);
                    }

                    // Apply new covering.
                    success |= SetMergedCellsRowCol(
                        GridMergeCellDirection.RowsInColumn,
                        Math.Min(rgMerged1.Top, rgMerged2.Bottom),
                        rgMerged1.Left,
                        Math.Max(rgMerged1.Top, rgMerged2.Bottom),
                        rgMerged1.Right);
                }
            }
            else
            {
                if (isMerged1 && rgMerged1.Bottom >= rowIndex2)
                {
                    success = SetMergedCellsRowCol(GridMergeCellDirection.RowsInColumn, rgMerged2.Top, rgMerged2.Left, rgMerged2.Top, rgMerged2.Left);

                    if (rowIndex1 > rgMerged1.Top)
                    {
                        success |= SetMergedCellsRowCol(GridMergeCellDirection.RowsInColumn, rgMerged1.Top, rgMerged1.Left, rowIndex1, rgMerged1.Left);
                    }
                }

                if (isMerged2 && rgMerged2.Top <= rowIndex1)
                {
                    if (rgMerged1.Bottom != rgMerged2.Bottom)
                    {
                        success |= SetMergedCellsRowCol(GridMergeCellDirection.RowsInColumn, rgMerged2.Top, rgMerged2.Left, rgMerged2.Top, rgMerged2.Left);
                    }

                    if (rowIndex2 < rgMerged2.Bottom)
                    {
                        success |= SetMergedCellsRowCol(GridMergeCellDirection.RowsInColumn, rowIndex2, rgMerged2.Left, rgMerged2.Bottom, rgMerged1.Left);
                    }
                }
            }

            if (success)
            {
                if (boundsInfo.IsEmpty)
                {
                    boundsInfo = GridRangeInfo.Cell(rowIndex1, colIndex);
                }

                boundsInfo = GridRangeInfo.Cells(
                    Math.Min(boundsInfo.Top, rgMerged1.Top),
                    Math.Min(boundsInfo.Left, rgMerged1.Left),
                    Math.Max(boundsInfo.Bottom, rgMerged1.Bottom),
                    Math.Max(boundsInfo.Right, rgMerged2.Right));
            }

            return success;
        }

        internal static void UpdateMergedCellsRowCol(GridControlBase grid, int rowIndex, int colIndex, int nOldToRow, int nOldToCol)
        {
            int toRowIndex = rowIndex,
                toColIndex = colIndex;

            GridRangeInfo rgMerged;
            if (grid.Model.MergeCells.Find(GridMergeCellDirection.Both, rowIndex, colIndex, out rgMerged))
            {
                toRowIndex = rgMerged.Bottom;
                toColIndex = rgMerged.Right;
            }

            grid.InvalidateRange(GridRangeInfo.Cells(rowIndex, colIndex, Math.Max(toRowIndex, nOldToRow), Math.Max(toColIndex, nOldToCol)));
        }
    }
}
