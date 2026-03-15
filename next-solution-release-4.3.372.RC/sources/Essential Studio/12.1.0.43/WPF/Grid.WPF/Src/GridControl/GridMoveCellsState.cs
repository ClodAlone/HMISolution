#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using System.Collections.Generic;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// For internal use. Holds the information about the cells being moved.
    /// </summary>
    public class GridMoveCellsState
    {
        IEditableLineSizeHost lineSizes;
        GridVolatileCellStyles volatileCellStyles;
        GridCellData data;
        GridCoveredCellInfoCollection coveredCells;
        GridCellSpanBackgroundInfoCollection cellSpanBackgrounds;
        GridRangeInfoList selections;
        Dictionary<GridControlBase, GridViewMoveCellsState> gridViews;

        public static GridMoveCellsState Empty = new GridMoveCellsState(true, null, null);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lineSizes">Line size.</param>
        public GridMoveCellsState(IEditableLineSizeHost lineSizes, GridVolatileCellStyles volatileCellStyles)
            : this(false, lineSizes, volatileCellStyles)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="isEmpty">True if empty.</param>
        /// <param name="lineSizes">Line size.</param>
        /// <param name="volatileCellStyles">A reference to a GridVolatileCellStyles object where GridStyleInfo objects can be stored</param>
        public GridMoveCellsState(bool isEmpty, IEditableLineSizeHost lineSizes, GridVolatileCellStyles volatileCellStyles)
        {
            if (!isEmpty)
            {
                this.gridViews = new Dictionary<GridControlBase, GridViewMoveCellsState>();
                this.lineSizes = lineSizes;
                if (volatileCellStyles == null)
                    volatileCellStyles = new GridVolatileCellStyles(null);
                this.volatileCellStyles = volatileCellStyles;
                this.cellSpanBackgrounds = new GridCellSpanBackgroundInfoCollection(null);
                this.coveredCells = new GridCoveredCellInfoCollection(null);
                this.data = new GridCellData();
                this.selections = new GridRangeInfoList();
            }
        }

        /// <summary>
        /// True if empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return lineSizes == null; }
        }

        /// <summary>
        /// Grid views.
        /// </summary>
        public Dictionary<GridControlBase, GridViewMoveCellsState> GridViews
        {
            get { return gridViews; }
        }

        /// <summary>
        /// Covered cells.
        /// </summary>
        public GridCoveredCellInfoCollection CoveredCells
        {
            get { return coveredCells; }
        }

        /// <summary>
        /// Cell spanned backgrounds.
        /// </summary>
        public GridCellSpanBackgroundInfoCollection CellSpanBackgrounds
        {
            get { return cellSpanBackgrounds; }
        }

        /// <summary>
        /// Line sizes for the grid.
        /// </summary>
        public IEditableLineSizeHost LineSizes
        {
            get { return lineSizes; }
        }

        /// <summary>
        /// Volatile cell styles.
        /// </summary>
        public GridVolatileCellStyles VolatileCellStyles
        {
            get { return volatileCellStyles; }
        }

        /// <summary>
        /// Cell data.
        /// </summary>
        public GridCellData Data
        {
            get { return data; }
        }

        /// <summary>
        /// Selected ranges for grid.
        /// </summary>
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

    /// <summary>
    /// Creates a view of the information about the cells being moved.
    /// </summary>
    public class GridViewMoveCellsState
    {
        CellUIElementsDictionary cellUIElements;
        GridRenderStyleInfoDictionary renderStyles;
        GridCurrentCellMoveState currentCell;
        RenderedCellsMoveState drawingVisuals;

        public static GridViewMoveCellsState Empty = new GridViewMoveCellsState(true);

        /// <summary>
        /// Initializes a new <see cref="GridViewMoveCellsState"/>.
        /// </summary>
        public GridViewMoveCellsState()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridViewMoveCellsState"/>.
        /// </summary>
        /// <param name="isEmpty">True if empty.</param>
        public GridViewMoveCellsState(bool isEmpty)
        {
            if (!isEmpty)
            {
                this.cellUIElements = new CellUIElementsDictionary(null);
                this.renderStyles = new GridRenderStyleInfoDictionary();
                this.currentCell = new GridCurrentCellMoveState();
                this.drawingVisuals = new RenderedCellsMoveState();
            }
        }

        /// <summary>
        /// True if empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return cellUIElements == null; }
        }

        /// <summary>
        /// Current cell state.
        /// </summary>
        public GridCurrentCellMoveState CurrentCell
        {
            get { return currentCell; }
        }

        /// <summary>
        /// Dictionary of UI elements of the cells.
        /// </summary>
        public CellUIElementsDictionary CellUIElements
        {
            get { return cellUIElements; }
        }

        /// <summary>
        /// A collection of cell styles.
        /// </summary>
        public GridRenderStyleInfoDictionary RenderStyles
        {
            get { return renderStyles; }
        }

        /// <summary>
        /// State of the rendered cells that are moving.
        /// </summary>
        public RenderedCellsMoveState DrawingVisuals
        {
            get { return drawingVisuals; }
        }

    }

}
