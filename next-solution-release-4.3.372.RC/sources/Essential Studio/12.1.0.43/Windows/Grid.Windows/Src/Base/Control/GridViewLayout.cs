//-------------------------------------------------------------------------------------------------
// <copyright file="GridViewLayout.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
    /// pixel scrolling is enabled and cells are only partially visible.
    /// </summary>
    public enum GridCellSizeKind
    {
        /// <summary>
        /// Get the whole rectangle of the cell including the parts that are
        /// above the visible scroll area when pixel scrolling is enabled and cells 
        /// are only partially visible.
        /// </summary>
        ActualSize,

        /// <summary>
        /// Get only the visible rectangle of the cell excluding the parts that are
        /// above the visible scroll area when pixel scrolling is enabled and cells 
        /// are only partially visible.
        /// </summary>
        VisibleSize
    }

    /// <summary>
    /// This class caches layout information about a grid control view. Whenever changes are made to the view
    /// or the grid is scrolled, all information in this class is refreshed.
    /// </summary>
    /// <remarks>
    /// You get access to this class with the <see cref="GridControlBase.ViewLayout"/> property of a <see cref="GridControlBase"/>.
    /// </remarks>
    public class GridViewLayout : GridSubComponent
    {
        const int defaultArraySize = 100;
        int nxMax, nyMax;
        int[] anHeights = new int[defaultArraySize];
        int[] anWidths = new int[defaultArraySize];
        int[] anYOffset = new int[defaultArraySize];
        int[] anXOffset = new int[defaultArraySize];
        int[] anVisibleXOffset = new int[defaultArraySize];
        int[] anVisibleYOffset = new int[defaultArraySize];
        int[] anRows = new int[defaultArraySize];
        int[] anCols = new int[defaultArraySize];
        int nRows;
        int nCols;
        Rectangle bounds;
        int nfr;
        int nfc;
        int frozenClientCols;  // this one counts header as column ...
        int frozenClientRows;  // this one counts header as row ...
        int nTopRow;
        int nLeftCol;
        int nLastRow;
        int nLastCol;
        Rectangle scrollAreaBounds;
        Rectangle hscrollAreaBounds;
        Rectangle vscrollAreaBounds;
        GridControlBase grid;

        GridRangeInfo rgFrozenTL, rgFrozenTop, rgFrozenLeft, rgCells, rgVisible;

        bool initialized = false;

        Hashtable visibleRowsList;
        Hashtable visibleColumnsList;

        /// <summary>
        /// Occurs when layout information has been changed or reinitialized.
        /// </summary>
        public event EventHandler LayoutChanged;

        // TODO: search for more stuff that I can cache here .., that does on
        // TODO: which events should update this view info?

        /// <summary>
        /// Initializes a <see cref="GridViewLayout"/> and associates it with a grid.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public GridViewLayout(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
            grid.WindowScrolling += new ScrollWindowEventHandler(GridWindowScrolling);
        }

        bool locked = false;

        /// <summary>
        /// Prevent subsequent calls to <see cref="Reset"/> method from clearing the layout information. Use this if
        /// you want to optimize Invalidate / Update calls.
        /// </summary>
        /// <example>
        /// Updates a single cell and avoids ViewLayout being reinitialized.
        /// <code lang="C#">
        /// this.gridControl1.ViewLayout.Lock();
        /// GridRangeInfo cell = GridRangeInfo.Cell(row, col);
        /// Rectangle bounds = gridControl1.RangeInfoToRectangle(cell);
        /// this.gridControl1.Invalidate(bounds);
        /// this.gridControl1.Update();
        /// this.gridControl1.ViewLayout.Unlock();
        /// </code>
        /// </example>
        public void Lock()
        {
            locked = true;
        }

        /// <summary>
        /// Restores correct <see cref="Reset"/> method behavior after a call to <see cref="Lock"/> was made. See <see cref="Lock"/>
        /// for a C# example.
        /// </summary>
        public void Unlock()
        {
            locked = false;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                grid.WindowScrolling -= new ScrollWindowEventHandler(GridWindowScrolling);
            }

            base.Dispose(disposing);
        }

        void GridWindowScrolling(object sender, ScrollWindowEventArgs e)
        {
            locked = false;
            this.Reset();
        }

        int[] RedimArray(int[] array, int length)
        {
            int[] newArray = new int[length];
            array.CopyTo(newArray, 0);
            return newArray;
        }

        /// <summary>
        /// Call this method when layout information needs to be refreshed.
        /// </summary>
        public void Reset()
        {
            if (locked)
            {
                return;
            }

            initialized = false;
            grid.currentHScrollPixelPosSaved = -1;
            grid.hScrollPixelWidthSaved = -1;
            grid.currentVScrollPixelPosSaved = -1;
            grid.vScrollPixelHeightSaved = -1;
        }

        /// <exclude/>
        /// <summary>Used internally.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void InternalSetTopRow(int topRowIndex)
        {
            if (initialized)
            {
                nTopRow = topRowIndex;
                for (int rowIndex = 0; rowIndex <= this.nRows; rowIndex++)
                {
                    this.anRows[rowIndex] = grid.GetRow(rowIndex);
                }

                nLastRow = nRows == 0 ? 0 : anRows[nRows - 1];
                rgFrozenTop = GridRangeInfo.InternalCells(0, nLeftCol, nfr, nLastCol);
                rgFrozenLeft = GridRangeInfo.InternalCells(nTopRow, 0, nLastRow, nfc);
                rgCells = GridRangeInfo.InternalCells(nTopRow, nLeftCol, nLastRow, nLastCol);
                rgVisible = GridRangeInfo.InternalCells(rgVisible.Top, rgVisible.Left, nLastRow, nLastCol);
            }
        }

        bool inDemandInitialize = false;

        internal void DemandInitialize()
        {
            if (inDemandInitialize)
            {
                return;
            }

            /*
             * #if DEBUG
                        if (nTopRow != grid.TopRowIndex || nLeftCol != grid.LeftColIndex)
                            throw new InvalidOperationException("nTopRow != grid.TopRowIndex || nLeftCol != grid.LeftColIndex");
#endif*/
            inDemandInitialize = true;

            if (nTopRow != grid.TopRowIndex || nLeftCol != grid.LeftColIndex)
            {
                initialized = false;
            }

            if (!initialized)
            {
                Initialize();
            }

            inDemandInitialize = false;
        }

        bool isRightToLeft()
        {
            return Grid.IsRightToLeft();
        }

        Point ReverseRightToLeft(Point pt)
        {
            return isRightToLeft() ? new Point(bounds.Right - pt.X, pt.Y) : pt;
        }

        bool wasPrintintMode = false;
        int savedTopRow = 0;
        int savedLeftCol = 0;

        void Initialize()
        {
            inDemandInitialize = true;

            ////            TraceUtil.TraceCurrentMethodInfo(grid.TopRowIndex);
            ////                if (!grid.Visible)
            ////                    return;
            ////
            int rowIndex, colIndex;
            bounds = grid.GridBounds;

            if (grid.PrintingMode && !grid.IsWindowless)
            {
                // New for for version 4.4: Also need to set ScrollGrid.m_nTopRow because of new
                // hidden scroll logic, which requires the m_grid.ScrollGrid.HScrollPos/VScrollPos
                // to be in sync
                if (this.grid.ScrollGrid.m_nTopRow != this.grid.PrintInfo.m_nPrintTopRow)
                {
                    this.grid.ScrollGrid.m_nTopRow = this.grid.PrintInfo.m_nPrintTopRow;
                }

                if (this.grid.ScrollGrid.m_nLeftCol != this.grid.PrintInfo.m_nPrintLeftCol)
                {
                    this.grid.ScrollGrid.m_nLeftCol = this.grid.PrintInfo.m_nPrintLeftCol;
                }
                
                if (!wasPrintintMode)
                {
                    this.savedTopRow = this.grid.ScrollGrid.m_nTopRow;
                    this.savedLeftCol = this.grid.ScrollGrid.m_nLeftCol;
                    wasPrintintMode = true;
                }
            }
            else if (wasPrintintMode)
            {
                // need to restore saved values in that case
                wasPrintintMode = false;

                if (this.grid.ScrollGrid.m_nTopRow != savedTopRow)
                {
                    this.grid.ScrollGrid.m_nTopRow = savedTopRow;
                }

                if (this.grid.ScrollGrid.m_nLeftCol != savedLeftCol)
                {
                    this.grid.ScrollGrid.m_nLeftCol = savedLeftCol;
                }
            }

            if (grid.Model.Options.DisplayEmptyRows)
            {
                nRows = int.MaxValue - 2;
            }
            else
            {
                int rowCount = grid.Model.RowCount;
                if (rowCount == 0 && grid.Model.HideRows[0])
                {
                    nRows = -1;
                }
                else
                {
                    nRows = grid.GetClientRow(rowCount);
                }
            }

            if (grid.Model.Options.DisplayEmptyColumns)
            {
                nCols = int.MaxValue - 2;
            }
            else
            {
                int colCount = grid.Model.ColCount;
                if (colCount == 0 && grid.Model.HideCols[0])
                {
                    nCols = -1;
                }
                else
                {
                    nCols = grid.GetClientCol(colCount);
                }
            }

            nfr = grid.InternalGetFrozenRows();
            nfc = grid.InternalGetFrozenCols();
            frozenClientRows = grid.GetVisibleFrozenRows();
            frozenClientCols = grid.GetVisibleFrozenCols();

            if (!grid.PrintingMode && GridControlBase.UseOldHiddenScrollLogic)
            {
                grid.ScrollGrid.m_nTopRow = Math.Max(grid.TopRowIndex, grid.GetFirstScrollableRow());
                grid.ScrollGrid.m_nLeftCol = Math.Max(grid.LeftColIndex, grid.GetFirstScrollableCol());
            }

            nTopRow = grid.TopRowIndex;
            nLeftCol = grid.LeftColIndex;
            nyMax = bounds.Top;
            scrollAreaBounds = bounds;
            hscrollAreaBounds = bounds;
            vscrollAreaBounds = bounds;

            bool isTopRow = false;
            for (rowIndex = 0; rowIndex <= this.nRows && this.nyMax <= bounds.Bottom; rowIndex++)
            {
                this.anRows[rowIndex] = grid.GetRow(rowIndex);
                this.anVisibleYOffset[rowIndex] = this.nyMax;
                if (this.anRows[rowIndex] == nTopRow)
                {
                    isTopRow = true;
                    for (int i = 0; i < rowIndex; i++)
                    {
                        // Avoid overlapping offsets (in case LeftColIndex column is larger than previous column).
                        if (anYOffset[i] > nyMax)
                        {
                            anYOffset[i] = nyMax - 1;
                        }
                    }

                    if (nyMax > scrollAreaBounds.Top)
                    {
                        GridUtil.SetTop(ref scrollAreaBounds, nyMax);
                        GridUtil.SetTop(ref vscrollAreaBounds, nyMax);
                    }

                    this.nyMax -= grid.vScrollPixelDelta;
                }

                this.anHeights[rowIndex] = grid.GetRowHeight(this.anRows[rowIndex]);
                this.anYOffset[rowIndex] = this.nyMax;
                this.nyMax += this.anHeights[rowIndex];
                if (rowIndex == anHeights.Length - 1)
                {
                    anRows = RedimArray(anRows, anRows.Length * 2);
                    anHeights = RedimArray(anHeights, anHeights.Length * 2);
                    anYOffset = RedimArray(anYOffset, anYOffset.Length * 2);
                    anVisibleYOffset = RedimArray(anVisibleYOffset, anVisibleYOffset.Length * 2);
                }
            }

            if (!isTopRow)
            {
                vscrollAreaBounds = new Rectangle(scrollAreaBounds.Left, scrollAreaBounds.Top, scrollAreaBounds.Width, 0);
                scrollAreaBounds = new Rectangle(scrollAreaBounds.Left, scrollAreaBounds.Top, scrollAreaBounds.Width, 0);
            }

            this.nRows = rowIndex;
            while (rowIndex < frozenClientRows)
            {
                anRows[rowIndex] = int.MaxValue;
                anHeights[rowIndex] = 0;
                anYOffset[rowIndex] = this.nyMax;
                anVisibleYOffset[rowIndex] = this.nyMax;
                if (rowIndex == anHeights.Length - 1)
                {
                    anRows = RedimArray(anRows, anRows.Length * 2);
                    anHeights = RedimArray(anHeights, anHeights.Length * 2);
                    anYOffset = RedimArray(anYOffset, anYOffset.Length * 2);
                    anVisibleYOffset = RedimArray(anVisibleYOffset, anVisibleYOffset.Length * 2);
                }

                rowIndex++;
            }

            for (int n = rowIndex; n < anRows.Length; n++)
            {
                anRows[n] = int.MaxValue;
                anHeights[n] = 0;
                anYOffset[n] = int.MaxValue;
                anVisibleYOffset[n] = int.MaxValue;
            }

            anYOffset[nRows] = nyMax;
            anVisibleYOffset[nRows] = nyMax;

            if (this.isRightToLeft())
            {
                bool isLeftCol = false;
                nxMax = bounds.Right;
                for (colIndex = 0; colIndex <= this.nCols && this.nxMax >= bounds.Left; colIndex++)
                {
                    this.anCols[colIndex] = grid.GetCol(colIndex);
                    this.anVisibleXOffset[colIndex] = this.nxMax;

                    if (this.anCols[colIndex] == nLeftCol)
                    {
                        isLeftCol = true;
                        for (int i = 0; i < colIndex; i++)
                        {
                            // Avoid overlapping offsets (in case LeftColIndex column is larger than previous column).
                            if (anXOffset[i] < nxMax)
                            {
                                anXOffset[i] = nxMax + 1;
                            }
                        }

                        if (nxMax < scrollAreaBounds.Right)
                        {
                            GridUtil.SetRight(ref scrollAreaBounds, nxMax);
                            GridUtil.SetRight(ref hscrollAreaBounds, nxMax);
                        }

                        this.nxMax += grid.hScrollPixelDelta;
                    }

                    this.anWidths[colIndex] = grid.GetColWidth(this.anCols[colIndex]);
                    this.anXOffset[colIndex] = this.nxMax;
                    this.nxMax -= this.anWidths[colIndex];
                    if (colIndex == anWidths.Length - 1)
                    {
                        anCols = RedimArray(anCols, anCols.Length * 2);
                        anWidths = RedimArray(anWidths, anWidths.Length * 2);
                        anXOffset = RedimArray(anXOffset, anXOffset.Length * 2);
                        anVisibleXOffset = RedimArray(anVisibleXOffset, anVisibleXOffset.Length * 2);
                    }
                }

                if (!isLeftCol)
                {
                    hscrollAreaBounds = new Rectangle(scrollAreaBounds.Right, scrollAreaBounds.Top, 0, scrollAreaBounds.Height);
                    scrollAreaBounds = new Rectangle(scrollAreaBounds.Right, scrollAreaBounds.Top, 0, scrollAreaBounds.Height);
                }

                this.nCols = colIndex;
                while (colIndex < frozenClientCols)
                {
                    anCols[colIndex] = int.MaxValue;
                    anWidths[colIndex] = 0;
                    anXOffset[colIndex] = this.nxMax;
                    anVisibleXOffset[colIndex] = this.nxMax;
                    if (colIndex == anWidths.Length - 1)
                    {
                        anCols = RedimArray(anCols, anCols.Length * 2);
                        anWidths = RedimArray(anWidths, anWidths.Length * 2);
                        anXOffset = RedimArray(anXOffset, anXOffset.Length * 2);
                        anVisibleXOffset = RedimArray(anVisibleXOffset, anVisibleXOffset.Length * 2);
                    }

                    colIndex++;
                }

                for (int n = colIndex; n < anCols.Length; n++)
                {
                    anCols[n] = int.MaxValue;
                    anWidths[n] = 0;
                    anXOffset[n] = int.MaxValue;
                    anVisibleXOffset[n] = int.MaxValue;
                }

                anXOffset[nCols] = nxMax;
                anVisibleXOffset[nCols] = nxMax;
            }
            else
            {
                nxMax = bounds.Left;
                bool isLeftCol = false;
                for (colIndex = 0; colIndex <= this.nCols && this.nxMax <= bounds.Right; colIndex++)
                {
                    this.anCols[colIndex] = grid.GetCol(colIndex);
                    this.anVisibleXOffset[colIndex] = this.nxMax;

                    if (this.anCols[colIndex] == nLeftCol)
                    {
                        isLeftCol = true;
                        for (int i = 0; i < colIndex; i++)
                        {
                            // Avoid overlapping offsets (in case LeftColIndex column is larger than previous column).
                            if (anXOffset[i] > nxMax)
                            {
                                anXOffset[i] = nxMax - 1;
                            }
                        }

                        if (nxMax > scrollAreaBounds.Left)
                        {
                            GridUtil.SetLeft(ref scrollAreaBounds, nxMax);
                            GridUtil.SetLeft(ref hscrollAreaBounds, nxMax);
                        }

                        this.nxMax -= grid.hScrollPixelDelta;
                    }

                    this.anWidths[colIndex] = grid.GetColWidth(this.anCols[colIndex]);
                    this.anXOffset[colIndex] = this.nxMax;
                    this.nxMax += this.anWidths[colIndex];
                    if (colIndex == anWidths.Length - 1)
                    {
                        anCols = RedimArray(anCols, anCols.Length * 2);
                        anWidths = RedimArray(anWidths, anWidths.Length * 2);
                        anXOffset = RedimArray(anXOffset, anXOffset.Length * 2);
                        anVisibleXOffset = RedimArray(anVisibleXOffset, anVisibleXOffset.Length * 2);
                    }
                }

                if (!isLeftCol)
                {
                    hscrollAreaBounds = new Rectangle(scrollAreaBounds.Left, scrollAreaBounds.Top, 0, scrollAreaBounds.Height);
                    scrollAreaBounds = new Rectangle(scrollAreaBounds.Left, scrollAreaBounds.Top, 0, scrollAreaBounds.Height);
                }

                this.nCols = colIndex;
                while (colIndex < frozenClientCols)
                {
                    anCols[colIndex] = int.MaxValue;
                    anWidths[colIndex] = 0;
                    anXOffset[colIndex] = this.nxMax;
                    anVisibleXOffset[colIndex] = this.nxMax;
                    if (colIndex == anWidths.Length - 1)
                    {
                        anCols = RedimArray(anCols, anCols.Length * 2);
                        anWidths = RedimArray(anWidths, anWidths.Length * 2);
                        anXOffset = RedimArray(anXOffset, anXOffset.Length * 2);
                        anVisibleXOffset = RedimArray(anVisibleXOffset, anVisibleXOffset.Length * 2);
                    }

                    colIndex++;
                }

                for (int n = colIndex; n < anCols.Length; n++)
                {
                    anCols[n] = int.MaxValue;
                    anWidths[n] = 0;
                    anXOffset[n] = int.MaxValue;
                    anVisibleXOffset[n] = int.MaxValue;
                }

                anXOffset[nCols] = nxMax;
                anVisibleXOffset[nCols] = nxMax;
            }

            nLastRow = nRows == 0 ? 0 : anRows[nRows - 1];
            nLastCol = nCols == 0 ? 0 : anCols[nCols - 1];

            rgFrozenTL = GridRangeInfo.InternalCells(0, 0, nfr, nfc);
            rgFrozenTop = GridRangeInfo.InternalCells(0, nLeftCol, nfr, nLastCol);
            rgFrozenLeft = GridRangeInfo.InternalCells(nTopRow, 0, nLastRow, nfc);
            rgCells = GridRangeInfo.InternalCells(nTopRow, nLeftCol, nLastRow, nLastCol);
            rgVisible = GridRangeInfo.InternalCells(grid.GetRow(this.frozenClientRows), grid.GetCol(this.frozenClientCols), nLastRow, nLastCol);
            initialized = true;

            inDemandInitialize = false;

            visibleRowsList = null;
            visibleColumnsList = null;

            if (!grid.PrintingMode)
            {
                IGridVisibleCellLists vd = grid.Model.VolatileData as IGridVisibleCellLists;
                if (vd != null)
                {
                    vd.SetVisibleColumnsList(grid, VisibleColumnsList);
                    vd.SetVisibleRowsList(grid, VisibleRowsList);
                }
            }

            OnLayoutChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Gets a hashtable where each entry consists of the absolute row index as key and
        /// relative row index as value. You can use this dictionary to quickly look up if a
        /// row is visible.
        /// </summary>
        public Hashtable VisibleRowsList
        {
            get
            {
                if (visibleRowsList == null)
                {
                    visibleRowsList = new Hashtable();
                    for (int n = 0; n < nRows; n++)
                    {
                        visibleRowsList.Add(anRows[n], n);
                    }
                }

                return visibleRowsList;
            }
        }

        /// <summary>
        /// Gets a hashtable where each entry consists of the absolute column index as key and
        /// relative column index as value. You can use this dictionary to quickly look up if a
        /// column is visible.
        /// </summary>
        public Hashtable VisibleColumnsList
        {
            get
            {
                if (visibleColumnsList == null)
                {
                    visibleColumnsList = new Hashtable();
                    for (int n = 0; n < nCols; n++)
                    {
                        visibleColumnsList.Add(anCols[n], n);
                    }
                }

                return visibleColumnsList;
            }
        }

        void OnLayoutChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {  
                TraceUtil.TraceCurrentMethodInfo(grid.PaneDesc);
            }
#else
                
            ;
#endif
            if (LayoutChanged != null)
            {
                LayoutChanged(this, e);
            }
        }

        /// <summary>
        /// Gets the current visible range of cells.
        /// </summary>
        public GridRangeInfo VisibleCellsRange
        {
            get
            {
                DemandInitialize();
                return rgVisible;
            }
        }

        /// <summary>
        /// Gets the current range of cells that is scrollable (all rows and columns excluding frozen rows and columns).
        /// </summary>
        public GridRangeInfo ScrollCellsRange
        {
            get
            {
                DemandInitialize();
                return rgCells;
            }
        }

        /// <summary>
        /// Gets the scrollable area in client coordinates (excluding both column and row headers).
        /// </summary>
        public Rectangle ScrollAreaBounds
        {
            get
            {
                DemandInitialize();
                return scrollAreaBounds;
            }
        }

        /// <summary>
        /// Gets the horizontal scrollable area in client coordinates (including column headers).
        /// </summary>
        public Rectangle HscrollAreaBounds
        {
            get
            {
                DemandInitialize();
                return hscrollAreaBounds;
            }
        }

        /// <summary>
        /// Gets the vertical scrollable area in client coordinates (including row headers).
        /// </summary>
        public Rectangle VscrollAreaBounds
        {
            get
            {
                DemandInitialize();
                return vscrollAreaBounds;
            }
        }

        /// <summary>
        /// Gets the InternalVscrollAreaBounds. Internal only.
        /// </summary>
        /// <value>The internal vscroll area bounds.</value>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Rectangle InternalVscrollAreaBounds
        {
            get
            {
                return vscrollAreaBounds;
            }
        }

        /// <summary>
        /// Gets the last visible column.
        /// </summary>
        public int LastVisibleCol
        {
            get
            {
                DemandInitialize();
                return this.nLastCol;
            }
        }

        /// <summary>
        /// Gets the InternalLastVisibleRow. Internal only.
        /// </summary>
        /// <value>The internal last visible row.</value>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int InternalLastVisibleRow
        {
            get
            {
                return this.nLastRow;
            }
        }

        /// <summary>
        /// Gets the last visible row.
        /// </summary>
        public int LastVisibleRow
        {
            get
            {
                DemandInitialize();
                return this.nLastRow;
            }
        }

        /// <summary>
        /// Gets a value indicating whether True if there are more rows to scroll down; False if at bottom of grid.
        /// </summary>
        public bool HasPartialVisibleRows
        {
            get
            {
                DemandInitialize();
                return this.nyMax > bounds.Bottom;
            }
        }

        /// <summary>
        /// Gets a value indicating whether True if there are more columns to scroll right; False if at right edge of grid.
        /// </summary>
        public bool HasPartialVisibleCols
        {
            get
            {
                DemandInitialize();
                return isRightToLeft() ? this.nxMax < bounds.Left : this.nxMax > bounds.Right;
            }
        }

        /// <summary>
        /// Gets the bottom-right corner point in client coordinates of the visible grid. To the right and
        /// to the bottom of this grid is a non-grid background area.
        /// </summary>
        public Point Corner
        {
            get
            {
                DemandInitialize();
                return new Point(this.nxMax, this.nyMax);
            }
        }

        /// <overload>
        /// Returns a client column index for a given point.
        /// </overload>
        /// <summary>
        /// Returns a client column index for a given point.
        /// </summary>
        /// <param name="pt">The point in client coordinates.</param>
        /// <param name="fixBackHidden">True if you want to get the latest visible column and not the index of a hidden column.</param>\
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The client column index for the column under the specified point.</returns>
        public int PointToClientCol(Point pt, bool fixBackHidden, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            int[] xOffset = sizeKind == GridCellSizeKind.VisibleSize ? anVisibleXOffset : anXOffset;
            int col = isRightToLeft() ? SearchLowestValue(xOffset, nCols + 1, pt.X) : SearchHighestValue(xOffset, nCols + 1, pt.X);
            if (col >= 0)
            {
                if (fixBackHidden)
                {
                    while (col > 0 && xOffset[col - 1] == xOffset[col])
                    {
                        col--;
                    }
                }
                else
                {
                    while (col + 1 < xOffset.Length && xOffset[col + 1] == xOffset[col])
                    {
                        col++;
                    }
                }
            }

            return col;
        }

        /// <summary>
        /// Returns a client column index for a given point.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <param name="fixBackHidden">if set to <c>true</c> [fix back hidden].</param>
        /// <returns>Returns a client column index.</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int PointToClientCol(Point pt, bool fixBackHidden)
        {
            return PointToClientCol(pt, fixBackHidden, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns a client column index for a given point.
        /// </overload>
        /// <summary>
        /// Returns a client column index for a given point.
        /// </summary>
        /// <param name="pt">The point in client coordinates.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The client column index for the column under the specified point.</returns>
        public int PointToClientCol(Point pt, GridCellSizeKind sizeKind)
        {
            return PointToClientCol(pt, false, sizeKind);
        }

        /// <summary>
        /// Returns a client column index for a given point.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <returns>returns a client column index for a given point</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int PointToClientCol(Point pt)
        {
            return PointToClientCol(pt, GridCellSizeKind.ActualSize);
        }
        
        /// <overload>
        /// Returns a client row index for a given point.
        /// </overload>
        /// <summary>
        /// Returns a client row index for a given point.
        /// </summary>
        /// <param name="pt">The point in client coordinates.</param>
        /// <param name="fixBackHidden">True if you want to get the latest visible row and not the index of a hidden row.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The client row index for the row under the specified point.</returns>
        public int PointToClientRow(Point pt, bool fixBackHidden, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            int[] yOffset = sizeKind == GridCellSizeKind.VisibleSize ? anVisibleYOffset : anYOffset;
            int row = SearchHighestValue(yOffset, nRows + 1, pt.Y);
            if (row >= 0)
            {
                if (fixBackHidden)
                {
                    while (row > 0 && yOffset[row - 1] == yOffset[row])
                    {
                        row--;
                    }
                }
                else
                {
                    while (row + 1 < yOffset.Length && yOffset[row + 1] == yOffset[row])
                    {
                        row++;
                    }
                }
            }

            return row;
        }

        /// <overload>
        /// Returns a client row index for a given point.
        /// </overload>
        /// <summary>
        /// Returns a client row index for a given point.
        /// </summary>
        /// <param name="pt">The point in client coordinates.</param>
        /// <returns>The client row index for the row under the specified point.</returns>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        public int PointToClientRow(Point pt, GridCellSizeKind sizeKind)
        {
            return PointToClientRow(pt, false, sizeKind);
        }

        /// <summary>
        /// Returns a client row index for a given point.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <param name="fixBackHidden">if set to <c>true</c> [fix back hidden].</param>
        /// <returns>Returns a client row index.</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when VScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int PointToClientRow(Point pt, bool fixBackHidden)
        {
            return PointToClientRow(pt, fixBackHidden, GridCellSizeKind.ActualSize);
        }

        /// <summary>
        /// Gets a client row index for a given point.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <returns>Returns client row index for a given point.</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when VScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int PointToClientRow(Point pt)
        {
            return PointToClientRow(pt, false, GridCellSizeKind.ActualSize);
        }

        /// <summary>
        /// Gets the number of visible rows in the current view.
        /// </summary>
        public int VisibleRows
        {
            get
            {
                DemandInitialize();
                return this.nRows;
            }
        }

        /// <summary>
        /// Gets the number of visible columns in the current view.
        /// </summary>
        public int VisibleCols
        {
            get
            {
                DemandInitialize();
                return this.nCols;
            }
        }

        int SearchHighestValue(int[] offset, int length, int max)
        {
            int index = Array.BinarySearch(offset, 0, length, max);
            return (index < 0) ? (~index) - 1 : index;
        }

        int SearchLowestValue(int[] offset, int length, int max)
        {
            int index = Array.BinarySearch(offset, 0, length, max, new LowestValueComparer());
            return (index < 0) ? (~index) - 1 : index;
        }

        class LowestValueComparer : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                IComparable x1 = x as IComparable;
                IComparable y1 = y as IComparable;
                return y1.CompareTo(x1);
            }
            #endregion
        }
        
        /// <overload>
        /// Returns client row and column indexes under a window rectangle.
        /// </overload>
        /// <summary>
        /// Returns client row and column indexes under a window rectangle.
        /// </summary>
        /// <param name="rect">The window region for which your client row and columns should be determined.</param>
        /// <param name="topRow">A place holder where the upper client row index is returned.</param>
        /// <param name="leftCol">A place holder where the left (RTL: right) client column index is returned.</param>
        /// <param name="bottomRow">A place holder where the bottom client row index is returned.</param>
        /// <param name="rightCol">A place holder where the right (RTL: left) client column index is returned.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        public void RectangleToClientRowCol(Rectangle rect, out int topRow, out int leftCol, out int bottomRow, out int rightCol, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            if (this.isRightToLeft())
            {
                PointToClientRowCol(new Point(rect.Right, rect.Top), out topRow, out leftCol, true, sizeKind);
                PointToClientRowCol(new Point(rect.Left + 1, rect.Bottom - 1), out bottomRow, out rightCol, false, sizeKind);
            }
            else
            {
                PointToClientRowCol(rect.Location, out topRow, out leftCol, true, sizeKind);
                PointToClientRowCol(new Point(rect.Right - 1, rect.Bottom - 1), out bottomRow, out rightCol, false, sizeKind);
            }
        }

        /// <summary>
        /// Returns client row and column indexes under a window rectangle.
        /// </summary>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public void RectangleToClientRowCol(Rectangle rect, out int topRow, out int leftCol, out int bottomRow, out int rightCol)
        {
            RectangleToClientRowCol(rect, out topRow, out leftCol, out bottomRow, out rightCol, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns client row and column indexes under a window point.
        /// </overload>
        /// <summary>
        /// Returns client row and column indexes under a window point.
        /// </summary>
        /// <param name="point">The window point for which your client row and columns should be determined.</param>
        /// <param name="row">A place holder where the client row index is returned.</param>
        /// <param name="col">A place holder where the client column index is returned.</param>
        /// <param name="fixBackHidden">True if you want to get the latest visible row and not the index of a hidden row.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>     
        public void PointToClientRowCol(Point point, out int row, out int col, bool fixBackHidden, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            row = PointToClientRow(point, fixBackHidden, sizeKind);
            col = PointToClientCol(point, fixBackHidden, sizeKind);
        }

        /// <summary>
        /// Returns client row and column indexes under a window point.
        /// </summary>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public void PointToClientRowCol(Point point, out int row, out int col, bool fixBackHidden)
        {
            PointToClientRowCol(point, out row, out col, fixBackHidden, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns a visible client row index for a given absolute row index. If the row is above the
        /// top row, the client row index for the top row index is returned.
        /// </overload>
        /// <summary>
        /// Returns a visible client row index for a given absolute row index. If the row is above the
        /// top row, the client row index for the top row index is returned.
        /// </summary>
        /// <param name="row">The absolute row index.</param>
        /// <returns>The client row index in the current visible grid view area.</returns>
        public int RowIndexToVisibleClient(int row)
        {
            DemandInitialize();
            if (row > nfr)
            {
                if (row < grid.TopRowIndex)
                {
                    row = frozenClientRows + 1;
                }
                else
                {
                    row = grid.GetClientRow(row);
                }
            }

            return Math.Max(0, Math.Min(nRows - 1, row));
        }

        /// <summary>
        /// Returns a visible client column index for a given absolute column index. If the column is left of the
        /// <see cref="GridControlBase.LeftColIndex"/>, the client column index for the <see cref="GridControlBase.LeftColIndex"/> is returned.
        /// </summary>
        /// <param name="col">The absolute column index.</param>
        /// <returns>The client column index in the current visible grid view area.</returns>
        public int ColIndexToVisibleClient(int col)
        {
            DemandInitialize();
            if (col > nfc)
            {
                if (col < grid.LeftColIndex)
                {
                    col = frozenClientCols + 1;
                }
                else
                {
                    col = grid.GetClientCol(col);
                }
            }

            return Math.Max(0, Math.Min(nCols - 1, col));
        }

        /// <summary>
        /// Returns a visible client row index for a given absolute row index. If the row is above the
        /// top row, the client row index for the top row index is returned. If the row is below the
        /// last visible row, the client row index the last visible row index is returned.
        /// </summary>
        /// <param name="row">The absolute row index.</param>
        /// <param name="hidden">A place holder that is set to True if the row index is above the top row
        /// and not a frozen row.</param>
        /// <returns>The client row index in the current visible grid view area.</returns>
        public int RowIndexToVisibleClient(int row, out bool hidden)
        {
            DemandInitialize();
            hidden = false;
            if (row > nfr)
            {
                if (row < grid.TopRowIndex)
                {
                    hidden = true;
                    row = frozenClientRows + 1;
                }
                else
                {
                    row = grid.GetClientRow(row);
                }
            }

            return Math.Min(nRows - 1, row);
        }

        /// <summary>
        /// Returns a visible client column index for a given absolute column index. If the column is left of the
        /// <see cref="GridControlBase.LeftColIndex"/>, the client column index for the left column index is returned.
        /// If the column is to the right of the
        /// last visible column, the client column index the last visible column index is returned.
        /// </summary>
        /// <param name="col">The absolute column index.</param>
        /// <param name="hidden">A place holder that is set to True if the column index is left of the left column index
        /// and not a frozen column.</param>
        /// <returns>The client column index in the current visible grid view area.</returns>
        public int ColIndexToVisibleClient(int col, out bool hidden)
        {
            DemandInitialize();
            hidden = false;
            if (col > nfc)
            {
                if (col < grid.LeftColIndex)
                {
                    hidden = true;
                    col = frozenClientCols + 1;
                }
                else
                {
                    col = grid.GetClientCol(col);
                }
            }

            return Math.Min(nCols - 1, col);
        }

        /// <overload>
        /// Returns the control region right (RTL: left) of a absolute column index. The passed-in column is included in the rectangle.
        /// </overload>
        /// <summary>
        /// Returns the control region right (RTL: left) of a absolute column index. The passed-in column is included in the rectangle.
        /// </summary>
        /// <param name="col">The absolute column index.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The window region that is right of the specified column.</returns>
        public Rectangle RectangleRightOfCol(int col, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            Rectangle r;
            if (isRightToLeft())
            {
                Point p = RowColToPoint(0, col, true, sizeKind);
                r = Rectangle.FromLTRB(grid.GridBounds.Left, p.Y, p.X, grid.GridBounds.Bottom);
            }
            else
            {
                r = new Rectangle(RowColToPoint(0, col, true, sizeKind), grid.GridBounds.Size);
            }

            r.Intersect(grid.GridBounds);
            return r;
        }

        /// <summary>
        /// Returns the control region right (RTL: left) of an absolute column index.
        /// </summary>
        /// <param name="col">The col index.</param>
        /// <returns>returns the control region right (RTL: left) of an absolute column index.</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public Rectangle RectangleRightOfCol(int col)
        {
            return RectangleRightOfCol(col, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the control region below an absolute row index. The passed-in row is included in the rectangle.
        /// </overload>
        /// <summary>
        /// Returns the control region below an absolute row index. The passed-in row is included in the rectangle.
        /// </summary>
        /// <param name="row">The absolute row index.</param>
        /// <returns>The window region that is below the specified row.</returns>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        public Rectangle RectangleBottomOfRow(int row, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            Point p = RowColToPoint(row, 0, true, sizeKind);
            Rectangle r = new Rectangle(new Point(grid.GridBounds.Left, p.Y), grid.GridBounds.Size);
            r.Intersect(grid.GridBounds);
            return r;
        }

        /// <summary>
        /// Returns the control region below an absolute row index.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <returns>Rectanlge region below the specified row index.</returns>
        public Rectangle RectangleBottomOfRow(int row)
        {
            return RectangleBottomOfRow(row, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the location of a cell specified with absolute row and column index.
        /// </overload>
        /// <summary>
        /// Returns the location of a cell specified with absolute row and column index.
        /// </summary>
        /// <param name="row">The absolute row index of the cell.</param>
        /// <param name="col">The absolute column index of the cell.</param>
        /// <param name="sizeKind">Specifies how to handle the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The window point for the location (top-left corner) of the cell.</returns>
        public Point RowColToPoint(int row, int col, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            return RowColToPoint(row, col, false, sizeKind);
        }

        /// <summary>
        /// Returns the location of a cell specified with absolute row and column index.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="col">The col index.</param>
        /// <returns>Returns the location of a cell.</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public Point RowColToPoint(int row, int col)
        {
            return RowColToPoint(row, col, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the location of a cell specified with client row and column index.
        /// </overload>
        /// <summary>
        /// Returns the location of a cell specified with client row and column index.
        /// </summary>
        /// <param name="row">The client row index of the cell.</param>
        /// <param name="col">The client column index of the cell.</param>
        /// <param name="sizeKind">Specifies how to handle the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The window point for the location (top-left corner) of the cell.</returns>
        public Point ClientRowColToPoint(int row, int col, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            int[] xOffset = sizeKind == GridCellSizeKind.VisibleSize ? anVisibleXOffset : anXOffset;
            int[] yOffset = sizeKind == GridCellSizeKind.VisibleSize ? anVisibleYOffset : anYOffset;
            if (col <= this.nCols + 1 && row <= this.nRows + 1)
            {
                return new Point(xOffset[col], yOffset[row]);
            }

            return Point.Empty;
        }

        /// <summary>
        /// Returns the location of a cell specified with client row and column index.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="col">Column index.</param>
        /// <returns>Returns the location of a cell.</returns>
        public Point ClientRowColToPoint(int row, int col)
        {
            return ClientRowColToPoint(row, col, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the location of a cell specified with absolute row and column index.
        /// </overload>
        /// <summary>
        /// Returns the location of a cell specified with absolute row and column index.
        /// </summary>
        /// <param name="row">The absolute row index of the cell.</param>
        /// <param name="col">The absolute column index of the cell.</param>
        /// <param name="ignoreOutsideClientRectangle">Set this True if the grid should only return points within the grid area; 
        /// False if the method should calculate cells that are below or right of the visible area.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The window point for the location (top-left corner / RTL: top-right) of the cell.</returns>
        public Point RowColToPoint(int row, int col, bool ignoreOutsideClientRectangle, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            int[] xOffset = sizeKind == GridCellSizeKind.VisibleSize ? anVisibleXOffset : anXOffset;
            int[] yOffset = sizeKind == GridCellSizeKind.VisibleSize ? anVisibleYOffset : anYOffset;
            if (nRows == 0)
            {
                return Point.Empty;
            }

            Point pt = new Point();

            int nnrow;
            if (GridControlBase.UseOldHiddenScrollLogic)
            {
                nnrow = row;
            }
            else
            {
                nnrow = grid.ScrollGrid.RowIndexToScrollPosition(row);
            }

            if (row <= nfr)
            {
                pt.Y = yOffset[nnrow];
            }
            else if (row < grid.TopRowIndex)
            {
                pt.Y = yOffset[this.frozenClientRows];
                if (!ignoreOutsideClientRectangle)
                {
                    if (sizeKind == GridCellSizeKind.VisibleSize)
                    {
                        for (int r = grid.TopRowIndex - 1; r >= row; r--)
                        {
                            pt.Y -= GetRowHeight(r, sizeKind);
                        }
                    }
                    else 
                    {
                        // Use possible optimized grid version.
                        pt.Y -= GetRowRangeHeight(row, grid.TopRowIndex - 1, sizeKind);
                    }
                }
            }
            else
            {
                int clientRow = grid.GetClientRow(row);
                if (clientRow < nRows)
                {
                    pt.Y = yOffset[clientRow];
                }
                else if (ignoreOutsideClientRectangle)
                {
                    pt.Y = this.nyMax + 1;
                }
                else
                {
                    pt.Y = this.nyMax;
                    if (sizeKind == GridCellSizeKind.VisibleSize)
                    {
                        int count = grid.Model.RowCount;
                        for (int r = this.LastVisibleRow + 1; r <= count && r < row;)
                        {
                            pt.Y += GetRowHeight(r, sizeKind);
                            if (!grid.ScrollGrid.GetNextRowIndex(ref r))
                            {
                                break;
                            }
                        }
                    }
                    else 
                    {
                        //// Use possible optimized grid version.
                        pt.Y += GetRowRangeHeight(this.LastVisibleRow + 1, row - 1, sizeKind);
                    }
                }
            }

            int nncol;
            if (GridControlBase.UseOldHiddenScrollLogic)
            {
                nncol = col;
            }
            else
            {
                nncol = grid.ScrollGrid.ColIndexToScrollPosition(col);
            }
            
            if (this.isRightToLeft())
            {
                if (col <= nfc)
                {
                    pt.X = xOffset[nncol];
                }
                else if (col < grid.LeftColIndex)
                {
                    pt.X = xOffset[this.frozenClientCols];
                    if (!ignoreOutsideClientRectangle)
                    {
                        for (int c = grid.LeftColIndex - 1; c >= col; c--)
                        {
                            pt.X += GetColWidth(c, sizeKind);
                        }
                    }
                }
                else
                {
                    int clientCol = grid.GetClientCol(col);
                    if (clientCol < nCols)
                    {
                        pt.X = xOffset[clientCol];
                    }
                    else if (ignoreOutsideClientRectangle)
                    {
                        pt.X = this.nxMax - 1;
                    }
                    else
                    {
                        pt.X = this.nxMax;
                        int count = grid.Model.ColCount;
                        for (int c = this.LastVisibleCol + 1; c <= count && c < col;)
                        {
                            pt.X -= GetColWidth(c, sizeKind);
                            if (!grid.ScrollGrid.GetNextColIndex(ref c))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (col <= nfc)
                {
                    pt.X = xOffset[nncol];
                }
                else if (col < grid.LeftColIndex)
                {
                    pt.X = xOffset[this.frozenClientCols];
                    if (!ignoreOutsideClientRectangle)
                    {
                        if (sizeKind == GridCellSizeKind.VisibleSize)
                        {
                            for (int c = grid.LeftColIndex - 1; c >= col; c--)
                            {
                                pt.X -= GetColWidth(c, sizeKind);
                            }
                        }
                        else
                        {
                            //// Use possible optimized grid version.
                            pt.X -= GetColRangeWidth(col, grid.LeftColIndex - 1, sizeKind);
                        }
                    }
                }
                else
                {
                    int clientCol = grid.GetClientCol(col);
                    if (clientCol < nCols)
                    {
                        pt.X = xOffset[clientCol];
                    }
                    else if (ignoreOutsideClientRectangle)
                    {
                        pt.X = this.nxMax + 1;
                    }
                    else
                    {
                        pt.X = this.nxMax;
                        if (sizeKind == GridCellSizeKind.VisibleSize)
                        {
                            int count = grid.Model.ColCount;
                            for (int c = this.LastVisibleCol + 1; c <= count && c < col;)
                            {
                                pt.X += GetColWidth(c, sizeKind);
                                if (!grid.ScrollGrid.GetNextColIndex(ref c))
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            //// Use possible optimized grid version.
                            pt.X += GetColRangeWidth(this.LastVisibleCol + 1, col - 1, sizeKind);
                        }
                    }
                }
            }

            return pt;
        }

        /// <summary>
        /// Returns the location of a cell specified with absolute row and column index.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="col">The col index.</param>
        /// <param name="ignoreOutsideClientRectangle">if set to <c>true</c> [ignore outside client rectangle].</param>
        /// <returns>Returns the location of a cell specified with absolute row and column index</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public Point RowColToPoint(int row, int col, bool ignoreOutsideClientRectangle)
        {
            return RowColToPoint(row, col, ignoreOutsideClientRectangle, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Calculates the display area for a given range of cells with cell coordinates specified in absolute row and column indexes.
        /// </overload>
        /// <summary>
        /// Calculates the display area for a given range of cells with cell coordinates specified in absolute row and column indexes.
        /// </summary>
        /// <param name="range">The <see cref="GridRangeInfo"/> with the range of cells.</param>
        /// <param name="sizeKind">Specifies how to handle the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>A <see cref="Rectangle"/> that spans the range of visible cells. If no cells in the given range
        /// are visible, <see cref="Rectangle.Empty"/> is returned.</returns>
        /// <remarks>
        /// If there are covered cells or floating cells, they will treated as regular cells. The range is not enlarged
        /// to fit these spanned cells.
        /// </remarks>
        public Rectangle RangeInfoToRectangle(GridRangeInfo range, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            return RangeInfoToRectangle(range, false, sizeKind);
        }

        /// <summary>
        /// Calculates the display area for a given range of cells with cell coordinates specified in absolute row and column indexes.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>Returns the display area for a given range of cells with cell coordinates specified in absolute row and column indexes.</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public Rectangle RangeInfoToRectangle(GridRangeInfo range)
        {
            return RangeInfoToRectangle(range, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the column width of the column, reducing it by hScrollPixelDelta if is the 
        /// column at LeftColIndex. When horizontal pixel scrolling is enabled, hScrollPixelDelta
        /// will be between 0 and the width of the column at LeftColIndex.
        /// </overload>
        /// <summary>
        /// Returns the column width of the column, reducing it by hScrollPixelDelta if is the 
        /// column at LeftColIndex. When horizontal pixel scrolling is enabled, hScrollPixelDelta
        /// will be between 0 and the width of the column at LeftColIndex.
        /// </summary>
        /// <param name="absoluteColIndex">The absolute column index.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>Column width.</returns>
        public int GetColWidth(int absoluteColIndex, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            int colWidth = grid.GetColWidth(absoluteColIndex);
            if (sizeKind == GridCellSizeKind.VisibleSize && absoluteColIndex == grid.LeftColIndex)
            {
                colWidth -= grid.hScrollPixelDelta;
            }

            return colWidth;
        }

        /// <summary>
        /// Returns the column width of the column, reducing it by hScrollPixelDelta if is the
        /// column at LeftColIndex. When horizontal pixel scrolling is enabled hScrollPixelDelta
        /// will be between 0 and the width of the column at LeftColIndex.
        /// </summary>
        /// <param name="absoluteColIndex">Index of the absolute col.</param>
        /// <returns>Returns the column width of the column</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetColWidth(int absoluteColIndex)
        {
            return GetColWidth(absoluteColIndex, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the column width of the column, reducing it by hScrollPixelDelta if is the 
        /// column at LeftColIndex. When horizontal pixel scrolling is enabled, hScrollPixelDelta
        /// will be between 0 and the width of the column at LeftColIndex.
        /// </overload>
        /// <summary>
        /// Returns the column width of the column, reducing it by hScrollPixelDelta if is the 
        /// column at LeftColIndex. When horizontal pixel scrolling is enabled, hScrollPixelDelta
        /// will be between 0 and the width of the column at LeftColIndex.
        /// </summary>
        /// <param name="clientColIndex">The client column index.</param>
        /// <param name="sizeKind">Specifies how to handle the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>Client column width.</returns>
        public int GetClientColWidth(int clientColIndex, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            return GetColWidth(grid.GetCol(clientColIndex), sizeKind);
        }

        /// <summary>
        /// Returns the column width of the column, reducing it by hScrollPixelDelta if is the
        /// column at LeftColIndex. When horizontal pixel scrolling is enabled, hScrollPixelDelta
        /// will be between 0 and the width of the column at LeftColIndex.
        /// </summary>
        /// <param name="clientColIndex">Index of the client col.</param>
        /// <returns>Returns the column width of the column</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetClientColWidth(int clientColIndex)
        {
            return GetClientColWidth(clientColIndex, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the row height of the row, reducing it by vScrollPixelDelta if is the 
        /// row at TopRowIndex. When horizontal pixel scrolling is enabled, vScrollPixelDelta
        /// will be between 0 and the height of the row at TopRowIndex.
        /// </overload>
        /// <summary>
        /// Returns the row height of the row, reducing it by vScrollPixelDelta if is the 
        /// row at TopRowIndex. When horizontal pixel scrolling is enabled, vScrollPixelDelta
        /// will be between 0 and the height of the row at TopRowIndex.
        /// </summary>
        /// <param name="absoluteRowIndex">The absolute row index.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>Row height.</returns>
        public int GetRowHeight(int absoluteRowIndex, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            int rowHeight = grid.GetRowHeight(absoluteRowIndex);
            if (sizeKind == GridCellSizeKind.VisibleSize && absoluteRowIndex == grid.TopRowIndex)
            {
                rowHeight -= grid.vScrollPixelDelta;
            }

            return rowHeight;
        }

        /// <summary>
        /// Returns the row height of the row, reducing it by vScrollPixelDelta if is the
        /// row at TopRowIndex. When horizontal pixel scrolling is enabled, vScrollPixelDelta
        /// will be between 0 and the height of the row at TopRowIndex.
        /// </summary>
        /// <param name="absoluteRowIndex">Index of the absolute row.</param>
        /// <returns>Returns the row height of the row</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when VScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetRowHeight(int absoluteRowIndex)
        {
            return GetRowHeight(absoluteRowIndex, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the row height of the row, reducing it by vScrollPixelDelta if is the 
        /// row at TopRowIndex. When horizontal pixel scrolling is enabled, vScrollPixelDelta
        /// will be between 0 and the height of the row at TopRowIndex.
        /// </overload>
        /// <summary>
        /// Returns the row height of the row, reducing it by vScrollPixelDelta if is the 
        /// row at TopRowIndex. When horizontal pixel scrolling is enabled, vScrollPixelDelta
        /// will be between 0 and the height of the row at TopRowIndex.
        /// </summary>
        /// <param name="clientRowIndex">The client row index</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>Client row height.</returns>
        public int GetClientRowHeight(int clientRowIndex, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            return GetRowHeight(grid.GetRow(clientRowIndex), sizeKind);
        }

        /// <summary>
        /// Returns the row height of the row, reducing it by vScrollPixelDelta if is the
        /// row at TopRowIndex. When horizontal pixel scrolling is enabled, vScrollPixelDelta
        /// will be between 0 and the height of the row at TopRowIndex.
        /// </summary>
        /// <param name="clientRowIndex">Index of the client row.</param>
        /// <returns>Returns the row height of the row</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when VScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetClientRowHeight(int clientRowIndex)
        {
            return GetClientRowHeight(clientRowIndex, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Calculates the display area for a given range of cells with cell coordinates specified in absolute row and column indexes.
        /// </overload>
        /// <summary>
        /// Calculates the display area for a given range of cells with cell coordinates specified in absolute row and column indexes.
        /// </summary>
        /// <param name="range">The <see cref="GridRangeInfo"/> with the range of cells.</param>
        /// <param name="ignoreOutsideClientRectangle">Set this True if the grid should only return points within the grid area; 
        /// False if the method should calculate cells that are below or right of the visible area.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>A <see cref="Rectangle"/> that spans the range of visible cells. If no cells in the given range
        /// are visible, <see cref="Rectangle.Empty"/> is returned.</returns>    
        /// <genoverload/>
        public Rectangle RangeInfoToRectangle(GridRangeInfo range, bool ignoreOutsideClientRectangle, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            if (nRows == 0 || !IsRangeVisible(range))
            {
                return Rectangle.Empty;
            }

            Point pt1 = RowColToPoint(range.Top, range.Left, ignoreOutsideClientRectangle, sizeKind);
            Point pt2 = RowColToPoint(range.Bottom, range.Right, ignoreOutsideClientRectangle, sizeKind);

            if (this.isRightToLeft())
            {
                if (range.IsRows || range.IsTable)
                {
                    pt1.X = bounds.Right;
                    pt2.X = Corner.X;
                }
                else if (pt2.X != int.MaxValue)
                {
                    pt2.X -= this.GetColWidth(range.Right, sizeKind);
                }

                if (range.IsCols || range.IsTable)
                {
                    pt1.Y = bounds.Top;
                    pt2.Y = Corner.Y;
                }
                else if (pt2.Y != int.MaxValue)
                {
                    pt2.Y += this.GetRowHeight(range.Bottom, sizeKind);
                }

                return Rectangle.FromLTRB(pt2.X, pt1.Y, pt1.X, pt2.Y);
            }
            else
            {
                if (range.IsRows || range.IsTable)
                {
                    pt1.X = bounds.Left;
                    pt2.X = Corner.X;
                }
                else if (pt2.X != int.MaxValue)
                {
                    pt2.X += this.GetColWidth(range.Right, sizeKind);
                }

                if (range.IsCols || range.IsTable)
                {
                    pt1.Y = bounds.Top;
                    pt2.Y = Corner.Y;
                }
                else if (pt2.Y != int.MaxValue)
                {
                    pt2.Y += this.GetRowHeight(range.Bottom, sizeKind);
                }

                return Rectangle.FromLTRB(pt1.X, pt1.Y, pt2.X, pt2.Y);
            }
        }

        /// <summary>
        /// Calculates the display area for a given range of cells with cell coordinates specified in absolute row and column indexes.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="ignoreOutsideClientRectangle">if set to <c>true</c> [ignore outside client rectangle].</param>
        /// <returns>Returns the display area for a given range of cells</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public Rectangle RangeInfoToRectangle(GridRangeInfo range, bool ignoreOutsideClientRectangle)
        {
            return RangeInfoToRectangle(range, ignoreOutsideClientRectangle, GridCellSizeKind.ActualSize);
        }

        /// <summary>
        /// Check if the row specified with an absolute row index is visible.
        /// </summary>
        /// <param name="row">The absolute row index specifying the row to be tested.</param>
        /// <returns>True if visible; False if outside visible area.</returns>
        public bool IsRowVisible(int row)
        {
            DemandInitialize();
            bool hidden;
            /*int clientRow = */
            RowIndexToVisibleClient(row, out hidden);
            return !hidden && row <= nLastRow;
        }

        /// <summary>
        /// Check if the column specified with an absolute column index is visible.
        /// </summary>
        /// <param name="col">The absolute column index specifying the column to be tested.</param>
        /// <returns>true if visible; false if outside visible area</returns>
        public bool IsColVisible(int col)
        {
            DemandInitialize();
            bool hidden;
            /*int clientCol = */
            ColIndexToVisibleClient(col, out hidden);
            return !hidden && col <= nLastCol;
        }

        /// <summary>
        /// Check if parts of the range specified with an absolute row and column indexes are visible.
        /// </summary>
        /// <param name="range">The range to be tested.</param>
        /// <returns>True if visible; False if outside visible area.</returns>
        public bool IsRangeVisible(GridRangeInfo range)
        {
            DemandInitialize();
            if (range.IsEmpty)
            {
                return false;
            }
            else if (range.IsTable)
            {
                return true;
            }

            if (range.IsRows)
            {
                if (range.Top < Grid.TopRowIndex && range.Bottom >= Grid.TopRowIndex)
                {
                    return true;
                }

                return IsRowVisible(range.Top) || IsRowVisible(range.Bottom);
            }
            else if (range.IsCols)
            {
                if (range.Left < Grid.LeftColIndex && range.Right >= Grid.LeftColIndex)
                {
                    return true;
                }

                return IsColVisible(range.Left) || IsColVisible(range.Right);
            }
            else
            {
                if ((range.Top < Grid.TopRowIndex && range.Bottom >= Grid.TopRowIndex)
                    || (range.Left < Grid.LeftColIndex && range.Right >= Grid.LeftColIndex))
                {
                    return true;
                }

                //// REVIEW: still necessary?
                return (IsRowVisible(range.Top) || IsRowVisible(range.Bottom))
                    && (IsColVisible(range.Left) || IsColVisible(range.Right));
            }
        }

        /// <summary>
        /// Enlarges the specified range with any floating or covered ranges that intersect with the range.
        /// </summary>
        /// <param name="range">The original range.</param>
        /// <returns>The enlarged range that contains the original range and any covered or floating cells ranges within the range.</returns>
        public GridRangeInfo CombineSpannedRanges(GridRangeInfo range)
        {
            DemandInitialize();
            GridRowColRangeInfoHandler[] handler = new GridRowColRangeInfoHandler[4];
            handler[0] = new GridRowColRangeInfoHandler(grid.Model.CoveredRanges.FindRange);
            handler[1] = new GridRowColRangeInfoHandler(grid.Model.FloatingCells.FindRange);
            handler[2] = new GridRowColRangeInfoHandler(grid.Model.MergeCells.FindRange);
            handler[3] = new GridRowColRangeInfoHandler(grid.Model.BanneredRanges.FindRange);
            return VisitVisibleCells(range, handler);
        }

        /// <summary>
        /// Executes a delegate for every cell in the specified range that is visible in the current grid view.
        /// </summary>
        /// <param name="range">The range that specifies the cells to visit.</param>
        /// <param name="handler">This delegate represents the method to execute for every visible cell in the specified range.</param>
        /// <returns>The outer range that contains all ranges returned by executing <paramref name="handler"/>.</returns>
        public GridRangeInfo VisitVisibleCells(GridRangeInfo range, GridRowColRangeInfoHandler[] handler)
        {
            DemandInitialize();
            GridRangeInfo savedRange = range;
            GridRangeInfo found;
            if (nRows > 0)
            {
                bool bottomHidden = false, rightHidden = false;
                int top, left, bottom, right;
                GridRangeInfo visibleRange = range.ExpandRange(0, 0, nLastRow, nLastCol);
                if (range.IsCols)
                {
                    top = RowIndexToVisibleClient(visibleRange.Top);
                    bottom = RowIndexToVisibleClient(visibleRange.Bottom, out bottomHidden);
                    left = range.Left;
                    right = range.Right;
                }
                else if (range.IsRows)
                {
                    left = ColIndexToVisibleClient(visibleRange.Left);
                    right = ColIndexToVisibleClient(visibleRange.Right, out rightHidden);
                    top = range.Top;
                    bottom = range.Bottom;
                }
                else
                {
                    top = RowIndexToVisibleClient(visibleRange.Top);
                    bottom = RowIndexToVisibleClient(visibleRange.Bottom, out bottomHidden);
                    left = ColIndexToVisibleClient(visibleRange.Left);
                    right = ColIndexToVisibleClient(visibleRange.Right, out rightHidden);
                }

                for (int row = top; row <= bottom; row++)
                {
                    int rowIndex = row;
                    if (!range.IsRows)
                    {
                        rowIndex = !bottomHidden ? anRows[row] : this.nTopRow - 1;
                    }

                    for (int col = left; col <= right; col++)
                    {
                        try
                        {
                            int colIndex = col;
                            if (!range.IsCols)
                            {
                                colIndex = !rightHidden ? anCols[col] : this.nLeftCol - 1;
                            }

                            for (int n = 0; n < handler.Length; n++)
                            {
                                if (handler[n] != null)
                                {
                                    found = handler[n](rowIndex, colIndex);
                                    if (!found.IsEmpty)
                                    {
                                        range = GridRangeInfo.UnionRange(found, range);
                                        col = Math.Max(col, ColIndexToVisibleClient(found.Right));
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                            if (!ExceptionManager.RaiseExceptionCatched(grid, ex))
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            ////                Trace.WriteLine("VisitVisibleCells: " + savedRange.ToString() + " -> " + range.ToString());
            return range;
        }

        /// <overload>
        /// Returns a one-dimensional array with ints filled with values determined from <see cref="GridControlBase.GetRowHeight"/>.
        /// </overload>
        /// <summary>
        /// Returns a one-dimensional array with ints filled with values determined from <see cref="GridControlBase.GetRowHeight"/>.
        /// </summary>
        /// <param name="fromRowIndex">The first row index.</param>
        /// <param name="toRowIndex">The last row index.</param>
        /// <param name="sizeKind">Specifies how to handle the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The array with row heights.</returns>
        public int[] GetRowHeights(int fromRowIndex, int toRowIndex, GridCellSizeKind sizeKind)
        {
            Debug.Assert(toRowIndex >= fromRowIndex, "toRowIndex < fromRowIndex");
            toRowIndex = Math.Max(fromRowIndex, toRowIndex);
            int nCount = toRowIndex - fromRowIndex + 1;
            int[] values = new int[nCount];
            for (int n = 0; n < nCount; n++)
            {
                values[n] = GetRowHeight(fromRowIndex + n, sizeKind);
            }

            return values;
        }

        /// <summary>
        /// Returns a one-dimensional array with ints filled with values determined from <see cref="GridControlBase.GetRowHeight"/>.
        /// </summary>
        /// <param name="fromRowIndex">Index of from row.</param>
        /// <param name="toRowIndex">Index of to row.</param>
        /// <returns>Returns a one-dimensional array with ints from <see cref="GridControlBase.GetRowHeight"/>.</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when VScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int[] GetRowHeights(int fromRowIndex, int toRowIndex)
        {
            return GetRowHeights(fromRowIndex, toRowIndex, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns a one-dimensional array with ints filled with values determined from <see cref="GridControlBase.GetColWidth"/>.
        /// </overload>
        /// <summary>
        /// Returns a one-dimensional array with ints filled with values determined from <see cref="GridControlBase.GetColWidth"/>.
        /// </summary>
        /// <param name="fromColIndex">The first column index.</param>
        /// <param name="toColIndex">The last column index.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The array with column widths.</returns>
        int[] GetColWidths(int fromColIndex, int toColIndex, GridCellSizeKind sizeKind)
        {
            Debug.Assert(toColIndex >= fromColIndex, "toColIndex < fromColIndex");
            toColIndex = Math.Max(fromColIndex, toColIndex);
            int nCount = toColIndex - fromColIndex + 1;
            int[] values = new int[nCount];
            for (int n = 0; n < nCount; n++)
            {
                values[n] = GetColWidth(fromColIndex + n, sizeKind);
            }

            return values;
        }

        /// <summary>
        /// Returns a one-dimensional array with ints filled with values determined from <see cref="GridControlBase.GetColWidth"/>.
        /// </summary>
        /// <param name="fromColIndex">Index of from col.</param>
        /// <param name="toColIndex">Index of to col.</param>
        /// <returns>Returns a one-dimensional array with ints from <see cref="GridControlBase.GetColWidth"/>.</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int[] GetColWidths(int fromColIndex, int toColIndex)
        {
            return GetColWidths(fromColIndex, toColIndex, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the total height of a range of rows.
        /// </overload>
        /// <summary>
        /// Returns the total height of a range of rows.
        /// </summary>
        /// <param name="fromRowIndex">The first row index.</param>
        /// <param name="toRowIndex">The last row index.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The total height of the specified range of rows.</returns>
        /// <remarks>
        /// Row heights are determined with <see cref="GridControlBase.GetRowHeight"/>.
        /// </remarks>
        public int GetRowRangeHeight(int fromRowIndex, int toRowIndex, GridCellSizeKind sizeKind)
        {
            return GetRowRangeHeight(fromRowIndex, toRowIndex, 0, sizeKind);
        }

        /// <summary>
        /// Returns the total height of a range of rows.
        /// </summary>
        /// <param name="fromRowIndex">Index of from row.</param>
        /// <param name="toRowIndex">Index of to row.</param>
        /// <returns>Returns the total height of a range of rows</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when VScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetRowRangeHeight(int fromRowIndex, int toRowIndex)
        {
            return GetRowRangeHeight(fromRowIndex, toRowIndex, 0, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the total height of a range of rows and aborts calculation if it is greater than a specified maximum value.
        /// </overload>
        /// <summary>
        /// Returns the total height of a range of rows and aborts calculation if it is greater than a specified maximum value.
        /// </summary>
        /// <param name="fromRowIndex">The first row index.</param>
        /// <param name="toRowIndex">The last row index.</param>
        /// <param name="maxSize">Aborts calculation if total height is greater than this specified maximum value.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The total height of the specified range of rows.</returns>
        /// <remarks>
        /// Row heights are determined with <see cref="GridControlBase.GetRowHeight"/>.
        /// </remarks>
        public int GetRowRangeHeight(int fromRowIndex, int toRowIndex, int maxSize /* = 0 */, GridCellSizeKind sizeKind)
        {
            if (sizeKind == GridCellSizeKind.VisibleSize)
            {
                int r = 0;
                for (int n = fromRowIndex; n <= toRowIndex && (maxSize == 0 || r <= maxSize);)
                {
                    r += GetRowHeight(n, sizeKind);
                    if (!grid.ScrollGrid.GetNextRowIndex(ref n))
                    {
                        break;
                    }
                }

                return r;
            }
            // Use possible optimized grid method.
            return grid.GetRowRangeHeight(fromRowIndex, toRowIndex, maxSize);
        }

        /// <summary>
        /// Returns the total height of a range of rows and aborts calculation if it is greater than a specified maximum value.
        /// </summary>
        /// <param name="fromRowIndex">Index of from row.</param>
        /// <param name="toRowIndex">Index of to row.</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <returns>Returns the total height of a range of rows</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when VScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetRowRangeHeight(int fromRowIndex, int toRowIndex, int maxSize /* = 0 */)
        {
            return GetRowRangeHeight(fromRowIndex, toRowIndex, maxSize, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the total width of a range of columns.
        /// </overload>
        /// <summary>
        /// Returns the total width of a range of columns.
        /// </summary>
        /// <param name="fromColIndex">The first column index.</param>
        /// <param name="toColIndex">The last column index.</param>
        /// <param name="sizeKind">Specifies how to handle the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The total width of the specified range of columns.</returns>
        /// <remarks>
        /// Column widths are determined with <see cref="GridControlBase.GetColWidth"/>.
        /// </remarks>
        public int GetColRangeWidth(int fromColIndex, int toColIndex, GridCellSizeKind sizeKind)
        {
            return GetColRangeWidth(fromColIndex, toColIndex, 0, sizeKind);
        }

        /// <summary>
        /// Returns the total width of a range of columns.
        /// </summary>
        /// <param name="fromColIndex">Index of from col.</param>
        /// <param name="toColIndex">Index of to col.</param>
        /// <returns>Returns the total width of a range of columns</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetColRangeWidth(int fromColIndex, int toColIndex)
        {
            return GetColRangeWidth(fromColIndex, toColIndex, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the total width of a range of columns and aborts calculation if it is greater than a specified maximum value.
        /// </overload>
        /// <summary>
        /// Returns the total width of a range of columns and aborts calculation if it is greater than a specified maximum value.
        /// </summary>
        /// <param name="fromColIndex">The first column index.</param>
        /// <param name="toColIndex">The last column index.</param>
        /// <param name="maxSize">Aborts calculation if total width is greater than this specified maximum value.</param>
        /// <param name="sizeKind">Specifies how to handle the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The total width of the specified range of columns.</returns>
        /// <remarks>
        /// Column widths are determined with <see cref="GridControlBase.GetColWidth"/>.
        /// </remarks>
        public int GetColRangeWidth(int fromColIndex, int toColIndex, int maxSize /* = 0 */, GridCellSizeKind sizeKind)
        {
            int r = 0;
            for (int n = fromColIndex; n <= toColIndex && (maxSize == 0 || r <= maxSize);)
            {
                r += GetColWidth(n, sizeKind);
                if (!grid.ScrollGrid.GetNextColIndex(ref n))
                {
                    break;
                }
            }

            return r;
        }

        /// <summary>
        /// Returns the total width of a range of columns and aborts calculation if it is greater than a specified maximum value.
        /// </summary>
        /// <param name="fromColIndex">Index of from col.</param>
        /// <param name="toColIndex">Index of to col.</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <returns>Returns the total width of a range of columns</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetColRangeWidth(int fromColIndex, int toColIndex, int maxSize /* = 0 */)
        {
            return GetColRangeWidth(fromColIndex, toColIndex, maxSize, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the total height of a range of rows specified in client row indexes.
        /// </overload>
        /// <summary>
        /// Returns the total height of a range of rows specified in client row indexes.
        /// </summary>
        /// <param name="fromRowIndex">The first client row index.</param>
        /// <param name="toRowIndex">The last client row index.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The total height of the specified range of rows.</returns>
        public int GetClientRowRangeHeight(int fromRowIndex, int toRowIndex, GridCellSizeKind sizeKind)
        {
            int[] yOffset = sizeKind == GridCellSizeKind.VisibleSize ? anVisibleYOffset : anYOffset;
            fromRowIndex = Math.Min(fromRowIndex, VisibleRows);
            toRowIndex = Math.Min(toRowIndex, VisibleRows);
            return yOffset[toRowIndex + 1] - yOffset[fromRowIndex];
        }

        /// <summary>
        /// Returns the total height of a range of rows specified in client row indexes.
        /// </summary>
        /// <param name="fromRowIndex">Index of from row.</param>
        /// <param name="toRowIndex">Index of to row.</param>
        /// <returns>Returns the total height of a range of rows</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when VScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetClientRowRangeHeight(int fromRowIndex, int toRowIndex)
        {
            return GetClientRowRangeHeight(fromRowIndex, toRowIndex, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the total height of a range of rows specified in client row indexes and aborts calculation if it is greater than a specified maximum value.
        /// </overload>
        /// <summary>
        /// Returns the total height of a range of rows specified in client row indexes and aborts calculation if it is greater than a specified maximum value.
        /// </summary>
        /// <param name="fromRowIndex">The first client row index.</param>
        /// <param name="toRowIndex">The last client row index.</param>
        /// <param name="maxSize">Aborts calculation if total height is greater than this specified maximum value.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The total height of the specified range of rows.</returns>
        public int GetClientRowRangeHeight(int fromRowIndex, int toRowIndex, int maxSize /* = 0 */, GridCellSizeKind sizeKind)
        {
            int r = 0;
            if (fromRowIndex > grid.Model.Rows.FrozenCount)
            {
                //// Can use optimized version here.
                return grid.GetRowRangeHeight(fromRowIndex, toRowIndex, maxSize);
            }

            for (int n = fromRowIndex; n <= toRowIndex && (maxSize == 0 || r <= maxSize); n++)
            {
                r += GetRowHeight(grid.GetRow(n), sizeKind);
            }

            return r;
        }

        /// <summary>
        /// Returns the total height of a range of rows specified in client row indexes and aborts calculation if it is greater than a specified maximum value.
        /// </summary>
        /// <param name="fromRowIndex">Index of from row.</param>
        /// <param name="toRowIndex">Index of to row.</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <returns>Returns the total height of a range of rows</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when VScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetClientRowRangeHeight(int fromRowIndex, int toRowIndex, int maxSize /* = 0 */)
        {
            return GetClientRowRangeHeight(fromRowIndex, toRowIndex, maxSize, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the total width of a range of columns specified in client row indexes.
        /// </overload>
        /// <summary>
        /// Returns the total width of a range of columns specified in client row indexes.
        /// </summary>
        /// <param name="fromColIndex">The first client column index.</param>
        /// <param name="toColIndex">The last client column index.</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The total width of the specified range of columns.</returns>
        public int GetClientColRangeWidth(int fromColIndex, int toColIndex, GridCellSizeKind sizeKind)
        {
            DemandInitialize();
            int[] xOffset = sizeKind == GridCellSizeKind.VisibleSize ? anVisibleXOffset : anXOffset;
            fromColIndex = Math.Min(fromColIndex, VisibleCols);
            toColIndex = Math.Min(toColIndex, VisibleCols);
            if (Grid.IsRightToLeft())
            {
                return xOffset[fromColIndex] - xOffset[toColIndex + 1];
            }

            return xOffset[toColIndex + 1] - xOffset[fromColIndex];
        }

        /// <summary>
        /// Returns the total width of a range of columns specified in client row indexes.
        /// </summary>
        /// <param name="fromColIndex">Start column index.</param>
        /// <param name="toColIndex">End column index.</param>
        /// <returns>Returns the total width of a range of columns</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetClientColRangeWidth(int fromColIndex, int toColIndex)
        {
            return GetClientColRangeWidth(fromColIndex, toColIndex, GridCellSizeKind.ActualSize);
        }

        /// <overload>
        /// Returns the total width of a range of columns specified in client row indexes and aborts calculation if it is greater than a specified maximum value.
        /// </overload>
        /// <summary>
        /// Returns the total width of a range of columns specified in client row indexes and aborts calculation if it is greater than a specified maximum value.
        /// </summary>
        /// <param name="fromColIndex">The first client column index.</param>
        /// <param name="toColIndex">The last client column index.</param>
        /// <param name="maxSize">Aborts calculation if total width is greater than this specified maximum value.</param>
        /// <returns>The total width of the specified range of columns.</returns>
        /// <param name="sizeKind">Specifies how to handle the the top-most row or left-most (RTL: right-most) column when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>The total width of the specified range of columns.</returns>
        public int GetClientColRangeWidth(int fromColIndex, int toColIndex, int maxSize /* = 0 */, GridCellSizeKind sizeKind)
        {
            int r = 0;
            for (int n = fromColIndex; n <= toColIndex && (maxSize == 0 || r <= maxSize); n++)
            {
                r += GetColWidth(grid.GetCol(n), sizeKind);
            }

            return r;
        }

        /// <summary>
        /// Returns the total width of a range of columns specified in client row indexes and aborts calculation if it is greater than a specified maximum value.
        /// </summary>
        /// <param name="fromColIndex">Index of from col.</param>
        /// <param name="toColIndex">Index of to col.</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <returns>Returns the total width of a range of columns</returns>
        [Obsolete("It is recommended that the sizeKind parameter is specified for pixel scrolling (when HScrollPixel = true). GridCellSizeKind.ActualSize is specified as default.")]
        public int GetClientColRangeWidth(int fromColIndex, int toColIndex, int maxSize /* = 0 */)
        {
            return GetClientColRangeWidth(fromColIndex, toColIndex, maxSize, GridCellSizeKind.ActualSize);
        }
    }
}
