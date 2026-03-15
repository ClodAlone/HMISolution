//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelFloatingCells.cs" company="syncfusion">
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
    /// This class manages floating cell ranges for a grid.
    /// </summary>
    /// <remarks>
    /// You access this class from a grid with the <see cref="GridModel.FloatingCells"/>
    /// property of a <see cref="GridModel"/> instance.
    /// <para/>
    /// Covered ranges are saved in two separate collections.
    /// <para/>
    /// The first collection is <see cref="GridRangeInfoList"/>,
    /// which allows quick enumeration through all covered cell ranges in the grid. This is good when covered ranges
    /// need to be recalculated because rows or column have been inserted, moved, or removed.
    /// <para/>
    /// The second collection is <see cref="GridCoveredCellPool"/>, which is optimized to look up if a specific cell
    /// is part of a covered range.
    /// <para/>
    /// The <see cref="GridModel.QueryCoveredRange"/> event in a <see cref="GridModel"/> lets you provide customized
    /// covered cells ranges at run-time. For example, you might want to have a pattern of covered ranges. 
    /// This allows you to customize the grid's default behavior and manage covered
    /// ranges by your own code and not with the <see cref="GridModelCoveredRanges"/> class.
    /// </remarks>
    [Serializable]
    public class GridModelFloatingCells : GridModelBound, ISerializable
    {
        // Constructor

        /// <overload>
        /// Initializes a new <see cref="GridModelFloatingCells"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridModelFloatingCells"/> object and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        public GridModelFloatingCells(GridModel model)
            : base(model)
        {
            model.Options.FloatCellsModeChanged += new EventHandler(ModelFloatCellsModeChanged);
            model.DataProviderChanged += new EventHandler(model_DataProviderChanged);
            SetFloatCellsMode(model.Options.FloatCellsMode);
        }

        /// <summary>
        /// Initializes a new <see cref="GridModelFloatingCells"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridModelFloatingCells(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            floatCellData = new InternalFloatingCellsData();
            floatSpanCellPool = (GridSpanCellPool)info.GetValue("Pool", typeof(GridSpanCellPool));
            floatDelayedRangePool = (GridDelayedRangePool)info.GetValue("Delayed", typeof(GridDelayedRangePool));
        }

        private void model_DataProviderChanged(object sender, EventArgs e)
        {
            // Clean up int/*float*/ cells state
            GridFloatCellsMode mode = this.floatCellsMode;
            SetFloatCellsMode(GridFloatCellsMode.None);
            SetFloatCellsMode(mode);
        }

        void ModelFloatCellsModeChanged(object sender, EventArgs e)
        {
            SetFloatCellsMode(model.Options.FloatCellsMode);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SetFloatCellsMode(GridFloatCellsMode.None);
                model.Options.FloatCellsModeChanged -= new EventHandler(ModelFloatCellsModeChanged);
                model.DataProviderChanged -= new EventHandler(model_DataProviderChanged);
            }

            base.Dispose(disposing);
        }

        ////            protected override void Dispose(bool disposing)
        ////            {
        ////                if (disposing)
        ////                {
        ////                    model.Options.FloatCellsModeChanged -= new EventHandler(ModelFloatCellsModeChanged);
        ////                }
        ////                base.Dispose(disposing);
        ////            }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridModelFloatingCells"/>.
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
            info.AddValue("Pool", floatSpanCellPool); // GridSpanCellPool
            info.AddValue("Delayed", floatDelayedRangePool); // GridDelayedRangePool
        }
        
        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        /// <param name="e">A GridRowColSizeChangedEventArgs that contains the event data. </param>
        internal void OnChanged(GridFloatingCellsChangedEventArgs e)
        {
            try
            {
                Model.RaiseFloatingCellsChanged(e);
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
        /// Marks the specified range of cells to be re-evaluated at a later time if text is floating into neighboring cells.
        /// </summary>
        /// <param name="range">The range to be re-evaluated.</param>
        public void DelayFloatCells(GridRangeInfo range)
        {
            DelayFloatCellsInt(range, GridConstants.Undefined);
        }

        internal GridSpanCellPool GetFloatCellsPool()
        {
            return floatSpanCellPool;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal class InternalFloatingCellsData
        {
            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            public int offsetColumn;

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            public int[] widths = new int[0];         // cached CalculatePreferredCellSize return values for latest row

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            public int savedRow;                // latest row id

            /// <summary>
            /// Default Constructor.
            /// </summary>
            public InternalFloatingCellsData()
                : base()
            {
            }

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            public void InitColOffset(int colIndex)
            {
                offsetColumn = Math.Max(256, colIndex) - 256;
            }

            /// <summary>
            /// Gets the index.
            /// </summary>
            /// <param name="colIndex">Index of the col.</param>
            /// <returns>returns the index</returns>
            /// <internalonly/>
            public int GetIndex(int colIndex)
            {
                if (colIndex > offsetColumn)
                {
                    return (int)(colIndex - offsetColumn);
                }

                return 0;
            }

            /// <internalonly/>
            /// <summary>
            /// Used internally.
            /// </summary>
            public void Reset()
            {
                savedRow = 0;
            }
        }

        internal void InsertRows(int index, int count)
        {
            if (floatCellsMode != GridFloatCellsMode.None)
            {
                floatSpanCellPool.InsertRows(index, count);
            }
        }

        internal void InsertCols(int index, int count)
        {
            if (floatCellsMode != GridFloatCellsMode.None)
            {
                floatSpanCellPool.InsertCols(index, count);
            }
        }

        internal void RemoveRows(int from, int to)
        {
            if (floatCellsMode != GridFloatCellsMode.None)
            {
                floatSpanCellPool.RemoveRows(from, to);
            }
        }

        internal void RemoveCols(int from, int to)
        {
            if (floatCellsMode != GridFloatCellsMode.None)
            {
                floatSpanCellPool.RemoveCols(from, to);
            }
        }

        internal void MoveRows(int from, int last, int dest)
        {
            if (floatCellsMode != GridFloatCellsMode.None)
            {
                floatSpanCellPool.MoveRows(from, last, dest);
            }
        }

        internal void MoveCols(int from, int last, int dest)
        {
            if (floatCellsMode != GridFloatCellsMode.None)
            {
                floatSpanCellPool.MoveCols(from, last, dest);
            }
        }

        [NonSerialized]
        internal InternalFloatingCellsData floatCellData = new InternalFloatingCellsData();

        [NonSerialized]
        internal bool lockEvaluateFloatingCells;
        internal GridFloatCellsMode floatCellsMode = GridFloatCellsMode.None;
        internal GridSpanCellPool floatSpanCellPool = null;
        internal GridDelayedRangePool floatDelayedRangePool = null;
        
        /// <summary>
        /// Temporarily stop evaluating floating cells.
        /// </summary>
        /// <returns>returns True.</returns>
        public bool LockEvaluate()
        {
            bool b = lockEvaluateFloatingCells;
            lockEvaluateFloatingCells = true;
            return b;
        }

        /// <summary>
        /// Start evaluating floating cells again after LockEvaluate call.
        /// </summary>
        public void UnlockEvaluate()
        {
            lockEvaluateFloatingCells = false;
        }
        
        ////        [Obsolete]
        ////        public bool LockEvaluateFloatingCells
        ////        {
        ////            get
        ////            {
        ////                return lockEvaluateFloatingCells;
        ////            }
        ////            set
        ////            {
        ////                lockEvaluateFloatingCells = value;
        ////            }
        ////        }
     
        internal void SetFloatCellsMode(GridFloatCellsMode nMode)
        {
            GridModel model = this.Model;

            if (nMode != floatCellsMode)
            {
                floatCellsMode = nMode;

                //// Clean up int/*float*/ cells state
                floatDelayedRangePool = null;
                floatSpanCellPool = null;

                switch (nMode)
                {
                    case GridFloatCellsMode.None:
                        break;

                    case GridFloatCellsMode.BeforeDisplayCalculation:
                        //// Reinit int/*float*/ cells state
                        floatSpanCellPool = new GridSpanCellPool();
                        break;

                    case GridFloatCellsMode.OnDemandCalculation:
                        //// Reinit int/*float*/ cells state
                        floatDelayedRangePool = new GridDelayedRangePool();
                        floatSpanCellPool = new GridSpanCellPool();

                        //// Force all cells to be recalculated
                        DelayFloatCells(GridRangeInfo.Table());
                        break;
                }
            }
        }

        /// <summary>
        /// Marks the specified range of cells to be re-evaluated at a later time if text is floating into neighboring cells.
        /// </summary>
        /// <param name="range">The range to be re-evaluated.</param>
        /// <param name="nMaxCols">The current column count of the grid or GridConstants.Undefined
        /// if all columns are affected.</param>
        internal void DelayFloatCellsInt(GridRangeInfo range, int nMaxCols)
        {
            GridModel model = this.Model;

            if (floatCellsMode != GridFloatCellsMode.OnDemandCalculation)
            {
                return;
            }
#if DEBUG

            if (Switches.FloatCells.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(range, nMaxCols);
            }
#else

            ;
#endif
            //// Expand range to current dimension.
            GridRangeInfo rg = range.IntersectRange(model.GridCellsRange);

            if (!rg.IsEmpty)
            {
                //// Display
                ////if (!model.IsPrinting)
                {
                    floatDelayedRangePool.DelayRange(rg);

                    if (nMaxCols != GridConstants.Undefined)
                    {
                        floatDelayedRangePool.SetColCount(nMaxCols);
                    }
                }
            }
        }

        /// <overload>
        /// Checks the specified range to see if any cells have been previously marked with <see cref="DelayFloatCells"/> 
        /// to be re-evaluated.
        /// </overload>
        /// <summary>
        /// Checks the specified range to see if any cells have been previously marked with <see cref="DelayFloatCells"/> 
        /// to be re-evaluated.
        /// </summary>
        /// <param name="range">The range to be re-evaluated.</param>
        /// <returns>True if floating state for any cell in the given range was changed.</returns>
        public bool EvaluateFloatingCells(GridRangeInfo range)
        {
            GridRangeInfo r = range;
            return EvaluateFloatingCells(range, ref r);
        }

        /// <summary>
        /// Checks the specified range to see if any cells have been previously marked with <see cref="DelayFloatCells"/>
        /// to be re-evaluated and returns a range that holds all affected cells.
        /// </summary>
        /// <param name="range">The range to be re-evaluated.</param>
        /// <param name="boundsInfo">The range with all affected cells including ranges that intersected with
        /// <paramref name="range"/>.
        /// </param>
        /// <returns>True if floating state for any cell in the given range was changed.</returns>
        internal bool EvaluateFloatingCells(GridRangeInfo range, ref GridRangeInfo boundsInfo)
        {
            GridModel model = this.Model;
            // Is evaluation of floating cells temporary or completely turned off?
            if (lockEvaluateFloatingCells)
            {
                return false;
            }

#if DEBUG
            Trace.WriteLineIf(Switches.FloatCells.TraceVerbose, "EvaluateFloatingCells(" + range.ToString() + ")");
#endif
            //// Different modes.
            switch (floatCellsMode)
            {
                case GridFloatCellsMode.None:
                    return false;

                case GridFloatCellsMode.OnDemandCalculation:
                    return EvalDelayedFloatCells(range, ref boundsInfo);

                case GridFloatCellsMode.BeforeDisplayCalculation:
                    return EvalAllFloatCells(range, ref boundsInfo);

                default:
#if DEBUG
                    Trace.WriteLineIf(Switches.FloatCells.TraceError, "Warning: floatCellsMode invalid");
#endif

                    return false;
            }
        }

        //// EvalAllFloatCells
        ////
        //// Evaluates all cells from the given range if they want to int/*float*/
        //// other cells.

        bool EvalAllFloatCells(GridRangeInfo range, ref GridRangeInfo boundsInfo)
        {
            GridModel model = this.Model;

            range = range.IntersectRange(model.GridCellsRange);
            bool success = false;
            floatCellData.InitColOffset(range.Left);
            for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
            {
                if (!model.HideRows[rowIndex])
                {
                    for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                    {
                        if (!model.HideCols[colIndex])
                        {
                            success |= FloatCell(rowIndex, colIndex, ref boundsInfo);
                        }
                    }
                }

                floatCellData.Reset();
            }
            //// Reset internally cached g, if any
            ////model.ReleaseGraphics();
            return success;
        }

        internal bool EvaluateFloatingCells(GridControlBase pGrid, GridRangeInfo range, int nfr, int nfc, int nTopRow, int nLeftCol, int nLastRow, int nLastCol, ref GridRangeInfo prgBoundary)
        {
#if DEBUG
            Trace.WriteLineIf(Switches.FloatCells.TraceVerbose, "Begin EvaluateFloatingCells(" + range.ToString() + ")");
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
                bEval |= pGrid.Model.FloatingCells.EvaluateFloatingCells(rgFrozenTL, ref prgBoundary);
            }

            if (!rgFrozenTop.IsEmpty)
            {
                bEval |= pGrid.Model.FloatingCells.EvaluateFloatingCells(rgFrozenTop, ref prgBoundary);
            }

            if (!rgFrozenLeft.IsEmpty)
            {
                bEval |= pGrid.Model.FloatingCells.EvaluateFloatingCells(rgFrozenLeft, ref prgBoundary);
            }

            if (!rgCells.IsEmpty)
            {
                bEval |= pGrid.Model.FloatingCells.EvaluateFloatingCells(rgCells, ref prgBoundary);
            }

#if DEBUG
            Trace.Unindent();
            Trace.WriteLineIf(Switches.FloatCells.TraceVerbose, "End EvaluateFloatingCells(" + range.ToString() + ")");
#endif
            return bEval;
        }

        //// EvalDelayedFloatCells
        ////
        //// Evaluates those cells from the given range which
        //// were previously marked as delayed with DelayFloatCells.
        //// ref GridRangeInfo boundsInfo // = null
        bool EvalDelayedFloatCells(GridRangeInfo range, ref GridRangeInfo boundsInfo)
        {
            GridModel model = this.Model;
#if DEBUG
            Trace.WriteLineIf(Switches.FloatCells.TraceVerbose, String.Format("EvalDelayedFloatCells({0})", range));
#endif

            range = range.IntersectRange(model.GridCellsRange);
            bool success = false;
            int[] dwColStart;
            int[] dwColEnd;
            GridDelayedRangePool pPool;
            ////                if (model.IsPrinting)
            ////                {
            ////                    if (delayedPrintFloatCells == null)
            ////                        delayedPrintFloatCells = new GridDelayedRangePool();
            ////                    pPool = delayedPrintFloatCells;
            ////                }
            ////                else
            pPool = floatDelayedRangePool;

            if (pPool.EvalRows(range, out dwColStart, out dwColEnd))
            {
                floatCellData.InitColOffset(range.Left);
                for (int nIndex = 0; nIndex < dwColStart.Length; nIndex++)
                {
                    int rowIndex = (int)nIndex + range.Top;
                    if (dwColEnd[nIndex] > 0 && !model.Rows.Hidden[rowIndex])
                    {
#if DEBUG
                        Trace.WriteLineIf(Switches.FloatCells.TraceVerbose, String.Format("Row = {0}: {1}, {2}", rowIndex, dwColStart[nIndex], dwColEnd[nIndex]));
#endif

                        for (int colIndex = dwColStart[nIndex]; colIndex <= dwColEnd[nIndex]; colIndex++)
                        {
                            if (!model.Cols.Hidden[colIndex])
                            {
                                success |= FloatCell(rowIndex, colIndex, ref boundsInfo);
                            }
                        }
                    }

                    floatCellData.Reset();
                }
            }
            //// Reset internally cached g, if any
            ////model.ReleaseGraphics().
            return success;
        }

        internal bool SetFloatedCellsRowCol(int rowIndex, int colIndex, int toRowIndex, int toColIndex)
        {
            GridModel model = this.Model;
#if DEBUG
            Trace.WriteLineIf(Switches.FloatCells.TraceVerbose, String.Format("SetFloatedCellsRowCol({0})", GridRangeInfo.Cells(rowIndex, colIndex, toRowIndex, toColIndex)));
#endif

            bool success = false;

            Debug.Assert(floatCellsMode != GridFloatCellsMode.None);
            Debug.Assert(toRowIndex >= rowIndex && toColIndex >= colIndex);

            int savedRowIndex = rowIndex,
                savedColIndex = colIndex;

            GridRangeInfo rgFloated;
            if (Find(rowIndex, colIndex, out rgFloated))
            {
                savedRowIndex = rgFloated.Bottom;
                savedColIndex = rgFloated.Right;
                success = true;
                ResetFloatedCells(rgFloated);
            }

            bool bSet = toRowIndex != rowIndex || toColIndex != colIndex;

            //// SyncfusionCommand
            if ((bSet && StoreFloatedCells(GridRangeInfo.Cells(rowIndex, colIndex, toRowIndex, toColIndex)))
                || savedRowIndex != toRowIndex || savedColIndex != toColIndex)
            {
                ////                    if (!model.IsDrawing)  // Don't update when called from OnDrawClientRowCol
                ////                        model.UpdateFloatedCellsRowCol(rowIndex, colIndex, savedRowIndex, savedColIndex);
                OnChanged(new GridFloatingCellsChangedEventArgs(rgFloated, true));
                success = true;
            }

            return success;
        }

        bool ResetFloatedCells(GridRangeInfo range)
        {
            GridModel model = this.Model;
#if DEBUG
            Trace.WriteLineIf(Switches.FloatCells.TraceVerbose, String.Format("ResetFloatedCells({0})", range));
#endif
            //// Check if model is Read-only.
            //// if (Param.lockReadOnly && (IsReadOnly || StandardStyle().GetReadOnly()))
            ////    return false;

            if (floatCellsMode == GridFloatCellsMode.None)
            {
                return false;
            }

            return GetFloatCellsPool().ResetSpanCells(range);
        }

        bool StoreFloatedCells(GridRangeInfo range)
        {
            GridModel model = this.Model;
#if DEBUG
            Trace.WriteLineIf(Switches.FloatCells.TraceVerbose, String.Format("StoreFloatedCells({0})", range));
#endif
            //// Check if model is Read-only.
            //// if (Param.lockReadOnly && (IsReadOnly || StandardStyle().GetReadOnly()))
            ////    return false;

            if (floatCellsMode == GridFloatCellsMode.None)
            {
                return false;
            }

            GridSpanCellPool pool = GetFloatCellsPool();
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

        // new GridControlBase.GridViewLayout.GridRowColRangeInfoHandler(FindRange)

        /// <summary>
        /// Returns a <see cref="GridRangeInfo"/> object that indicates the floating cell's range for the specified cell position
        /// or False if there is no floating cell's range for the given cell position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">The <see cref="GridRangeInfo"/> where the found floating cell's range is returned.</param>
        /// <returns>True if a floating cell's range is at the specified cell position; False if not.</returns>
        public bool Find(int rowIndex, int colIndex, out GridRangeInfo range)
        {
            GridSpanCellPool pool = GetFloatCellsPool();
            range = GridRangeInfo.Cell(rowIndex, colIndex);
            if (floatCellsMode != GridFloatCellsMode.None
                && pool.GetSpanCellsRowCol(rowIndex, colIndex, out range))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a <see cref="GridRangeInfo"/> object that indicates the floating cell's range for the specified cell position
        /// or <see cref="GridRangeInfo.Empty"/> if there is no floating cell's range for the given cell position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>A reference to range if a floating cell's range is at the specified cell position or
        /// <see cref="GridRangeInfo.Empty"/> if not.</returns>
        public GridRangeInfo FindRange(int rowIndex, int colIndex)
        {
            GridRangeInfo foundRange;
            if (Find(rowIndex, colIndex, out foundRange))
            {
                colIndex = foundRange.Right + 1;
                return foundRange;
            }

            return GridRangeInfo.Empty;
        }

        /// <summary>
        /// Combines all floating cell's ranges that intersect with a specified into one outer range that spans over all found ranges.
        /// </summary>
        /// <param name="range">The original range.</param>
        /// <returns>The <see cref="GridRangeInfo"/> with the outer range.</returns>
        public GridRangeInfo Merge(GridRangeInfo range)
        {
            GridModel model = this.Model;

            if (floatCellsMode != GridFloatCellsMode.None)
            {
                GridRangeInfo savedRange = range;
                for (int rowIndex = savedRange.Top; rowIndex <= savedRange.Bottom; rowIndex++)
                {
                    for (int colIndex = savedRange.Left; colIndex <= savedRange.Right; colIndex++)
                    {
                        GridRangeInfo foundRange;
                        if (Find(rowIndex, colIndex, out foundRange))
                        {
                            colIndex = foundRange.Right + 1;
                            range = GridRangeInfo.UnionRange(foundRange, range);
                        }
                    }
                }
            }

            return range;
        }

        /*public bool CanFloatCell(GridCellModelBase cellModel, int rowIndex, int colIndex, GridStyleInfo style, bool bFloatOrFlood)
            {
                // Static cells can float over other cells if value is not empty.
                if (bFloatOrFlood)
                {
                    // rotated cells don't float
                    if (style.Font.Orientation != 0)
                        return false;

                    return style.Text.ToString().Length > 0;
                }
                else
                    // Static cells can be flooded by other floatable cells if value is empty.
                    return style.Text.ToString().Length == 0;
            }*/

        /// <summary>
        /// Determines if a specific cell supports floating or being flooded by a neighboring cell.
        /// </summary>
        /// <param name="cellModel">The cell model</param>
        /// <param name="rowIndex">The row index</param>
        /// <param name="colIndex">The col index</param>
        /// <param name="style">The style info</param>
        /// <param name="query">The query float cell</param>
        /// <returns>return boolean value to determines if a specific cell supports floating.</returns>
        internal bool CanFloatCell(GridCellModelBase cellModel, int rowIndex, int colIndex, GridStyleInfo style, GridQueryFloatCell query)
        {
            ////                TraceUtil.TraceCurrentMethodInfoIf(Switches.GridModel.TraceVerbose, cellModel, rowIndex, colIndex, query, style);

            GridModel model = this.Model;

            if (floatCellsMode == GridFloatCellsMode.None)
            {
                return false;
            }

            if (model.HideCols[colIndex] || model.HideRows[rowIndex])
            {
                if (model.properties.AllowHiddenCellFloating && style.Text == string.Empty)
                {
                    return true;
                }
                else
                    return false;

            }

            GridRangeInfo rgCovered;
            if (model.CoveredRanges.Find(rowIndex, colIndex, out rgCovered))
            {
                return false;
            }

            if (model.MergeCells.Find(GridMergeCellDirection.Both, rowIndex, colIndex, out rgCovered))
            {
                return false;
            }

            // Static cells can int/*float*/ over other cells if value is not empty.
            if (query == GridQueryFloatCell.FloatCell)
            {
                return style.FloatCell && cellModel.OnQueryCanFloatCell(rowIndex, colIndex, style, query);
            }
            else
            {
                //// Don't int/*float*/ over the "Freeze line" or active cells.
                return colIndex != model.Cols.FrozenCount + 1 && style.FloodCell
                    && cellModel.OnQueryCanFloatCell(rowIndex, colIndex, style, query);
            }
        }

        //// FloatCell
        ////
        //// Evaluates the given cell.
        ////
        //// boundsInfo will be extended when the cell has become a floating
        //// cell or if it was a floating cell.

        bool FloatCell(int rowIndex, int colIndex, ref GridRangeInfo boundsInfo)
        {
            GridModel model = this.Model;

            if (floatCellsMode == GridFloatCellsMode.None)
            {
                return false;
            }

            if (colIndex <= model.Cols.HeaderCount || rowIndex <= model.Rows.HeaderCount)
            {
                return false;
            }

            //// Check if there is a cell to the left which could int/*float*/ over this cell.
            GridStyleInfo style = null;
            ////  int nDirty = -1;

            ////GridControlBase.InternalFloatingCellsData floatCellData = /*model.*/this.floatCellData;
            ////int colindex = floatCellData.GetIndex(colIndex);

            ////            if (colIndex < model.this.floatCellData.astylesArray.Count)
            ////                style = (GridStyleInfo) model.this.floatCellData.astylesArray[colIndex];
            ////
            ////            if (style == null)
            ////            {
            style = model[rowIndex, colIndex];
            ////                if (model.this.floatCellData.astylesArray.Count <= colIndex)
            ////                    model.this.floatCellData.astylesArray.AddRange(new GridStyleInfo[colIndex-model.this.floatCellData.astylesArray.Count+1]);
            ////                model.this.floatCellData.astylesArray[colIndex] = style;
            ////            }

            GridCellModelBase cellModel = style.CellModel;

            ////                if (model.HasCurrentCellAt(rowIndex, colIndex))
            ////                {
            ////                    style = style.GetOffLineCopy();
            ////                    //cellModel.LoadStyle(rowIndex, colIndex, style);
            ////                }

            bool bCanFloat = /*model.*/CanFloatCell(cellModel, rowIndex, colIndex, style, GridQueryFloatCell.FloatCell);
            ////            Debug.WriteLine(String.Format("bCanFLoat = {0} for rowIndex = {1}, colIndex = {2}",bCanFloat, rowIndex, colIndex));

            //// If this value is empty, first unfloat cell,
            //// then int/*float*/ cell to the left.

            //// If value is not empty, make sure this cell is not
            //// floated by another cell before checking
            //// if this cell should int/*float*/ cells to the right.

            GridRangeInfo rgFloated;
            int nCol1 = colIndex - 1;
            if (Find(rowIndex, colIndex - 1, out rgFloated))
            {
                nCol1 = rgFloated.Left;
            }

            bool b1 = false;
            bool b2 = false;
            if (bCanFloat)
            {
                b1 = InternalCheckFloatCell(/*model, */rowIndex, nCol1, ref boundsInfo);
                b2 = InternalCheckFloatCell(/*model, */rowIndex, colIndex, ref boundsInfo);
            }
            else
            {
                b2 = InternalCheckFloatCell(/*model, */rowIndex, colIndex, ref boundsInfo);
                b1 = InternalCheckFloatCell(/*model, */rowIndex, nCol1, ref boundsInfo);
            }

            return b1 || b2;
        }

        //// InternalCheckFloatCell
        ////
        //// Implementation for FloatCell

        bool InternalCheckFloatCell(int rowIndex, int colIndex, ref GridRangeInfo boundsInfo)
        {
            GridModel model = this.Model;
            bool success = false;

            GridRangeInfo rgFloated;
            /*model.*/
            Find(rowIndex, colIndex, out rgFloated);
            colIndex = rgFloated.Left;

            //// Get Style and Control.
            InternalFloatingCellsData floatCellData = /*model.*/this.floatCellData;
            int colindex = floatCellData.GetIndex(colIndex);

            GridStyleInfo style = null;

            ////            if (colIndex < floatCellData.astylesArray.Count)
            ////                style = (GridStyleInfo) floatCellData.astylesArray[colIndex];
            ////
            ////            if (style == null)
            ////            {
            style = model[rowIndex, colIndex];
            ////                if (floatCellData.astylesArray.Count <= colIndex)
            ////                    floatCellData.astylesArray.AddRange(new GridStyleInfo[colIndex-floatCellData.astylesArray.Count+1]);
            ////                floatCellData.astylesArray[colIndex] = style;
            ////            }

            GridCellModelBase cellModel = style.CellModel;
            ////
            ////                if (model.HasCurrentCellAt(rowIndex, colIndex))
            ////                {
            ////                    style = style.GetOffLineCopy();
            ////                    //cellModel.LoadStyle(rowIndex, colIndex, style);
            ////                }

            //// Is this cell floatable?
            int nFloat = 0;
            if (CanFloatCell(cellModel, rowIndex, colIndex, style, GridQueryFloatCell.FloatCell))
            {
                //// Calculate horizontal size of cell.

                //// size
                if (rowIndex != floatCellData.savedRow)
                {
                    for (int i = 0; i < floatCellData.widths.Length; i++)
                    {
                        floatCellData.widths[i] = 0;
                    }

                    floatCellData.savedRow = rowIndex;
                }

                Size size = Size.Empty;
                if (colindex < floatCellData.widths.Length)
                {
                    size.Width = floatCellData.widths[colindex];
                }

                if (size.Width == 0)
                {
                    IGraphicsProvider graphicsProvider = model.GetGraphicsProvider();
                    size = cellModel.CalculatePreferredCellSize(graphicsProvider.Graphics, rowIndex, colIndex, style, GridQueryBounds.Width);
                }
              
                // Re: model.CreateGraphics above - not need to dispose this object! It is a cached Display Device context.
                if (colindex >= floatCellData.widths.Length)
                {
                    int[] newArray = new int[(colindex + 1) * 2];
                    floatCellData.widths.CopyTo(newArray, 0);
                    floatCellData.widths = newArray;
                }

                floatCellData.widths[colindex] = size.Width;

                // Check number of cells to be floated
                int nColCount = model.ColCount;
                bool bCanFloat;
                while (colIndex + nFloat < nColCount && size.Width > model.ColWidths[colIndex + nFloat])
                {
                    // Break if next cell cannot be flooded (e.g. if value is not empty).
                    int colIndex2 = floatCellData.GetIndex(colIndex + nFloat + 1);

                    GridStyleInfo style2 = null;

                    ////                    if (colIndex2 < floatCellData.astylesArray.Count)
                    ////                        style2 = (GridStyleInfo) floatCellData.astylesArray[colIndex2];
                    ////
                    ////                    if (style2 == null)
                    ////                    {
                    style2 = model[rowIndex, colIndex + nFloat + 1]; // .Clone();
                    ////                        if (floatCellData.astylesArray.Count <= colIndex2)
                    ////                            floatCellData.astylesArray.AddRange(new GridStyleInfo[colIndex2-floatCellData.astylesArray.Count+1]);
                    ////                        floatCellData.astylesArray[colIndex2] = style2;
                    ////                    }

                    GridCellModelBase cellModel2 = style2.CellModel;
                    ////                        if (model.HasCurrentCellAt(rowIndex, colIndex+nFloat+1))
                    ////                        {
                    ////                            style2 = style2.GetOffLineCopy();
                    ////                            //cellModel2.LoadStyle(rowIndex, colIndex+nFloat+1, style2);
                    ////                        }

                    bCanFloat = /*model.*/CanFloatCell(cellModel2, rowIndex, colIndex + nFloat + 1, style2, GridQueryFloatCell.FloodCell);

                    if (!bCanFloat)
                    {
                        break;
                    }

                    // decrement size
                    size.Width -= (int)model.ColWidths[colIndex + nFloat];
                    nFloat++;
                }
            }

            //// Float cells if status has changed.
            if (rgFloated.Right != colIndex + nFloat)
            {
                success = /*model.*/SetFloatedCellsRowCol(rowIndex, colIndex, rowIndex, colIndex + nFloat);
            }

            //// success |= rgFloated.Right > colIndex || nFloat > 0;

            if (success)
            {
                if (boundsInfo.IsEmpty)
                {
                    boundsInfo = GridRangeInfo.Cell(rowIndex, colIndex);
                }

                boundsInfo = GridRangeInfo.Cells(Math.Min(boundsInfo.Top, rowIndex), Math.Min(boundsInfo.Left, colIndex), Math.Max(boundsInfo.Bottom, rowIndex), Math.Max(Math.Max(boundsInfo.Right, rgFloated.Right), colIndex + nFloat));
            }

            return success;
        }

        internal static void UpdateFloatedCellsRowCol(GridControlBase grid, int rowIndex, int colIndex, int nOldToRow, int nOldToCol)
        {
            int toRowIndex = rowIndex,
                toColIndex = colIndex;

            GridRangeInfo rgFloated;
            if (grid.Model.FloatingCells.Find(rowIndex, colIndex, out rgFloated))
            {
                toRowIndex = rgFloated.Bottom;
                toColIndex = rgFloated.Right;
            }

            grid.InvalidateRange(GridRangeInfo.Cells(rowIndex, colIndex, Math.Max(toRowIndex, nOldToRow), Math.Max(toColIndex, nOldToCol)));
        }
    }
}
