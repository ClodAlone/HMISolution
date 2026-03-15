#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Generic;
#if !WinRT
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridMoveCellsState
    {
        IEditableLineSizeHost lineSizes;
        GridVolatileCellStyles volatileCellStyles;
        GridCellData data;
        GridCoveredCellInfoCollection coveredCells;
        GridCellSpanBackgroundInfoCollection cellSpanBackgrounds;
        GridRangeInfoList selections;
        Dictionary<GridControlBase, GridViewMoveCellsState> gridViews;

        public static GridMoveCellsState Empty = new GridMoveCellsState(true, null);

        public GridMoveCellsState(IEditableLineSizeHost lineSizes)
            : this(false, lineSizes)
        {
        }

        public GridMoveCellsState(bool isEmpty, IEditableLineSizeHost lineSizes)
        {
            if (!isEmpty)
            {
                this.gridViews = new Dictionary<GridControlBase, GridViewMoveCellsState>();
                this.lineSizes = lineSizes;
                this.volatileCellStyles = new GridVolatileCellStyles(null);
                this.cellSpanBackgrounds = new GridCellSpanBackgroundInfoCollection(null);
                this.coveredCells = new GridCoveredCellInfoCollection(null);
                this.data = new GridCellData();
                this.selections = new GridRangeInfoList();
            }
        }

        public bool IsEmpty
        {
            get { return lineSizes == null; }
        }

        public Dictionary<GridControlBase, GridViewMoveCellsState> GridViews
        {
            get { return gridViews; }
        }

        public GridCoveredCellInfoCollection CoveredCells
        {
            get { return coveredCells; }
        }

        public GridCellSpanBackgroundInfoCollection CellSpanBackgrounds
        {
            get { return cellSpanBackgrounds; }
        }

        public IEditableLineSizeHost LineSizes
        {
            get { return lineSizes; }
        }

        public GridVolatileCellStyles VolatileCellStyles
        {
            get { return volatileCellStyles; }
        }

        public GridCellData Data
        {
            get { return data; }
        }

        public GridRangeInfoList Selections
        {
            get { return selections; }
        }

        /// <summary>
        /// Suspends the selection state when GridMoveCellsState is used
        /// </summary>
        public bool SuspendSelections
        {
            get;
            set;
        }
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridViewMoveCellsState
    {
        CellUIElementsDictionary cellUIElements;
        GridRenderStyleInfoDictionary renderStyles;
        GridCurrentCellMoveState currentCell;

        public static GridViewMoveCellsState Empty = new GridViewMoveCellsState(true);

        public GridViewMoveCellsState()
            : this(false)
        {
        }

        public GridViewMoveCellsState(bool isEmpty)
        {
            if (!isEmpty)
            {
                this.cellUIElements = new CellUIElementsDictionary(null);
                this.renderStyles = new GridRenderStyleInfoDictionary();
                this.currentCell = new GridCurrentCellMoveState();
            }
        }

        public bool IsEmpty
        {
            get { return cellUIElements == null; }
        }

        public GridCurrentCellMoveState CurrentCell
        {
            get { return currentCell; }
        }

        public CellUIElementsDictionary CellUIElements
        {
            get { return cellUIElements; }
        }

        public GridRenderStyleInfoDictionary RenderStyles
        {
            get { return renderStyles; }
        }

    }

}
