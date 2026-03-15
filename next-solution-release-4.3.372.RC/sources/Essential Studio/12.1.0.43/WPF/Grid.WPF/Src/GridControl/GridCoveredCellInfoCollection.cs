#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.ComponentModel;
using System;

namespace Syncfusion.Windows.Controls.Grid
{
     /// <summary>
    /// A collection with elements derived from type <see cref="CoveredCellInfo"/>. It implements
    /// <see cref="ICoveredCellsProvider"/>. Internally this collection maintains both a List of
    /// CoveredCellInfo and a so called pool. The pool allows immediate lookup of cell spans given
    /// a cells row and column index. The list allows looping through cell spans in the order they
    /// were added. GridCoveredCellInfoCollection assumes that there is no overlap between cell
    /// spans. For any given cell there only at most one cell span must exist. 
    /// </summary>
    public class GridCoveredCellInfoCollection : CellSpanInfoCollection<CoveredCellInfo>, ICoveredCellsProvider
    {
        GridModel gridModel;
        GridRangeInfoList ranges;

        /// <summary>
        /// Initializes a new <see cref="GridCoveredCellInfoCollection"/>.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        public GridCoveredCellInfoCollection(GridModel gridModel)
        {
            this.gridModel = gridModel;
        }

        /// <summary>
        /// Gets the covered ranges for the grid.
        /// </summary>
        public GridRangeInfoList Ranges
        {
            get
            {
                if (ranges == null)
                {
                    ranges = new GridRangeInfoList();
                    // TODO: Support for row, column ranges
                    foreach (CoveredCellInfo cc in this)
                        ranges.Add(GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right));
                }
                return ranges;
            }
        }

        /// <summary>
        /// Clears the covered ranges collection.
        /// </summary>
        public void InvalidateRanges()
        {
            if (this.ranges != null)
            {
                this.ranges.Clear();
                this.ranges = null;
            }
        }

        protected override void Added(CoveredCellInfo item)
        {
            base.Added(item);
            if (this.gridModel.CommandStack.ShouldGenerateUndoInfo)
            {
                this.gridModel.CommandStack.Push(new GridModelSetCoveredRangesCommand(gridModel, item, false));
            }
        }

        protected override void Removed(CoveredCellInfo item)
        {
            base.Removed(item);
            if (this.gridModel.CommandStack.ShouldGenerateUndoInfo)
            {
                this.gridModel.CommandStack.Push(new GridModelSetCoveredRangesCommand(gridModel, item, true));
            }
        }

        public void SetCoveredRanges(CoveredCellInfo item, bool setOrReset)
        {
            try
            {
                if (!setOrReset)
                {
                    if (this.Contains(item))
                        this.Remove(item);
                }
                else
                {
                    if (!this.Contains(item))
                        this.Add(item);
                }
            }
            catch (Exception ex)
            {
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }


        #region ICoveredCellsProvider

        /// <summary>
        /// Gets a covered cell from the <see cref="CoveredCellsProvider"/> that includes
        /// the specified cells row and column index.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        /// <returns>The covered cell.</returns>
        public CoveredCellInfo GetCoveredCell(int rowIndex, int columnIndex)
        {
            return base.GetCellSpan(rowIndex, columnIndex);
        }

        /// <summary>
        /// Specifies whether grid has covered cells.
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                // there could be an event handler for QueryCoveredRange
                return Count == 0 && !gridModel.SupportsQueryCoveredCellCallback;
            }
        }

        #endregion

        protected override bool OnGetCellSpan(int rowIndex, int columnIndex, out CoveredCellInfo result)
        {
            if (gridModel == null)
            {
                result = null;
                return false;
            }

            GridQueryCoveredRangeEventArgs e = new GridQueryCoveredRangeEventArgs(new RowColumnIndex(rowIndex, columnIndex));
            gridModel.RaiseQueryCoveredRange(e);
            result = e.Range;
            return e.Handled;
        }

        /// <summary>
        /// Gets a covered cell from the <see cref="CoveredCellsProvider"/> that includes
        /// the specified cells row and column index or if there is no covered range for the cell
        /// the method returns a cell range for the given row and column index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="coveredRange">The resulting covered range. If there is no covered range for the cell
        /// this range will return GridRangeInfo.Cell(rowIndex, columnIndex)</param>
        /// <returns>
        /// true if a covered cell was found; otherwise false.
        /// </returns>
        public bool Find(int rowIndex, int columnIndex, out GridRangeInfo coveredRange)
        {
            CoveredCellInfo cc = GetCoveredCell(rowIndex, columnIndex);
            if (cc != null)
            {
                coveredRange = GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right);
                return true;
            }
            else
            {
                coveredRange = GridRangeInfo.Cell(rowIndex, columnIndex);
                return false;
            }
        }

        internal GridRangeInfo FindRange(int rowIndex, int colIndex)
        {
            CoveredCellInfo cc = GetCoveredCell(rowIndex, colIndex);
            if (cc != null)
            {
                return GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right);
            }
            else
            {
                return GridRangeInfo.Empty;
            }
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.QueryCoveredRange"/> event which can be
    /// marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name=" e">A <see cref="GridQueryCoveredRangeEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryCoveredRangeEventHandler(object sender, GridQueryCoveredRangeEventArgs e);
	
    /// <summary>
    /// GridQueryCoveredRangeEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.QueryCoveredRange"/> event to query information about 
    /// covered cells at a specified cell. 
    /// <para/>
    /// This event allows you to specify covered ranges at run-time, e.g when you have
    /// a large grid with repeating patterns of covered ranges. If the specified row and
    /// column index is part of a covered cell's range, you should assign the coordinates
    /// of the covered cell to <see cref="GridQueryCoveredRangeEventArgs.Range"/> and
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
    /// <para/>
    /// <see cref="SyncfusionHandledEventArgs.Handled"/> indicates that you supplied data
    /// from your event handler and no further querying for data about covered range information 
    /// for this cell is necessary.
    /// </summary>
    public class GridQueryCoveredRangeEventArgs : SyncfusionHandledEventArgs
    {
        CoveredCellInfo range;
        RowColumnIndex cellRowColumnIndex;

        /// <overload>
        /// Initalizes a new object.
        /// </overload>
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridQueryCoveredRangeEventArgs(RowColumnIndex cellRowColumnIndex)
        {
            this.range = null;
            this.cellRowColumnIndex = cellRowColumnIndex;
        }

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">A <see cref="CoveredCellInfo"/> that will receive the resulting range for the covered cell.</param>
        public GridQueryCoveredRangeEventArgs(RowColumnIndex cellRowColumnIndex, CoveredCellInfo range)
        {
            this.range = range;
            this.cellRowColumnIndex = cellRowColumnIndex;
        }

        /// <summary>
        /// The cell row column index.
        /// </summary>
        public RowColumnIndex CellRowColumnIndex
        {
            get { return cellRowColumnIndex; }
        }

        /// <summary>
        /// A <see cref="CoveredCellInfo"/> that will receive the resulting range for the covered cell.
        /// </summary>
        [TraceProperty(true)]
        public CoveredCellInfo Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
            }
        }
    }

}
