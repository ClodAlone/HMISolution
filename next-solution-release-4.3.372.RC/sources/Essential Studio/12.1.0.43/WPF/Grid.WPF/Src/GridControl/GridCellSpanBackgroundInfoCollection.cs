#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
{
   /// <summary>
    /// A collection with elements derived from type <see cref="CellSpanBackgroundInfo"/>. Internally
    /// this collection maintains both a List of CellSpanBackgroundInfo and a so called pool. The
    /// pool allows immediate lookup of cell spans given a cells row and column index. The
    /// list allows looping through cell spans in the order they were added. <para/>
    /// CellOverlappSpanInfoCollection is similar to <see cref="CellSpanInfoCollection{T}"/> but
    /// with the difference that it allows overlaps between cell spans. If a cell has
    /// an overlap and <see cref="GetCellSpans"/> is called for this cell all cell spans
    /// will be returned in a list that contain the specified cell.
    /// </summary>
    public class GridCellSpanBackgroundInfoCollection : CellOverlappSpanInfoCollection<CellSpanBackgroundInfo>, ICellSpanBackgroundsProvider
    {
        GridModel gridModel;
        /// <summary>
        /// Initializes a new <see cref="GridCellSpanBackgroundInfoCollection"/>.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        public GridCellSpanBackgroundInfoCollection(GridModel gridModel)
        {
            this.gridModel = gridModel;
        }

        #region ICellSpanBackgroundsProvider Members

        /// <summary>
        /// Gets the cell span backgrounds that include
        /// the specified cells row and column index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>Cell span backgrounds.</returns>
        public List<CellSpanBackgroundInfo> GetCellSpanBackgrounds(int rowIndex, int columnIndex)
        {
            return base.GetCellSpans(rowIndex, columnIndex);
        }

        protected override bool OnGetCellSpans(int rowIndex, int columnIndex, out List<CellSpanBackgroundInfo> result)
        {
            if (gridModel == null)
            {
                result = null;
                return false;
            }

            GridQueryCellSpanBackgroundsEventArgs e = new GridQueryCellSpanBackgroundsEventArgs(new RowColumnIndex(rowIndex, columnIndex));
            gridModel.RaiseQueryCellSpanBackgrounds(e);
            result = e.Range;
            return e.Handled;
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                return false;
            }
        }

        #endregion
    }


    /// <summary>
    /// Represents the method that handles a <see cref="GridModel.QueryCellSpanBackgrounds"/> event which can be
    /// marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name=" e">A <see cref="GridQueryCellSpanBackgroundsEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryCellSpanBackgroundsEventHandler(object sender, GridQueryCellSpanBackgroundsEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridModel.QueryCellSpanBackgrounds"/> event which can be marked as handled.
    /// </summary>
    /// <remarks>
    /// GridQueryCellSpanBackgroundsEventArgs is a custom event argument class used by the
    /// <see cref="GridModel.QueryCellSpanBackgrounds"/> event to query information about 
    /// a bannered range at a specified cell. 
    /// </remarks>
    public class GridQueryCellSpanBackgroundsEventArgs : SyncfusionHandledEventArgs
    {
        List<CellSpanBackgroundInfo> range;
        RowColumnIndex cellRowColumnIndex;

        /// <overload>
        /// Initalizes a new object.
        /// </overload>
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridQueryCellSpanBackgroundsEventArgs(RowColumnIndex cellRowColumnIndex)
        {
            this.range = null;
            this.cellRowColumnIndex = cellRowColumnIndex;
        }

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">A <see cref="List<CellSpanBackgroundInfo>"/> that will receive the resulting range for the covered cell.</param>
        public GridQueryCellSpanBackgroundsEventArgs(RowColumnIndex cellRowColumnIndex, List<CellSpanBackgroundInfo> range)
        {
            this.range = range;
            this.cellRowColumnIndex = cellRowColumnIndex;
        }

        /// <summary>
        /// The row and column index.
        /// </summary>
        public RowColumnIndex CellRowColumnIndex
        {
            get { return cellRowColumnIndex; }
        }

        /// <summary>
        /// A <see cref="List<CellSpanBackgroundInfo>"/> that will receive the resulting range for the covered cell.
        /// </summary>
        [TraceProperty(true)]
        public List<CellSpanBackgroundInfo> Range
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
