#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that is responsible for &lt;TABLE&gt; Tag.
    /// </summary>
    [ElementTag(TagName.Table)]
    public class TABLEElementImpl : BaseElement
    {
        #region Class constants

        /// <summary>
        /// Name of the tag this class indicates.
        /// </summary>
        private const string DEF_TAG_NAME = TagName.Table;

        /// <summary>
        /// Supported events.
        /// </summary>
        private static string[] DEF_SUPP_EVENTS;

        /// <summary>
        ///  Holds all events which this class supports.
        /// </summary>
        private static Hashtable m_eventHash;
        #endregion

        #region Class members
        /// <summary>
        /// Number of rows in the table.
        /// </summary>
        private int m_numRows;

        /// <summary>
        /// Number of columns in the table.
        /// </summary>
        private int m_numCols;

        /// <summary>
        /// Holds all cells of the table.
        /// </summary>
        private TDElementImpl[,] m_cells;

        /// <summary>
        /// Contains an array of values which indicates if column has fixed width.
        /// </summary>
        private bool[] m_bFixedColumns;

        /// <summary>
        /// Number of columns which are resizable.
        /// </summary>
        private int m_resizableCount = -1;

        /// <summary>
        /// Holds maximum colspan number in the column.
        /// </summary>
        private int[] m_maxColspanInColumn;

        /// <summary>
        /// Holds maximum rowspan number in the row.
        /// </summary>
        private int[] m_maxRowspanInRow;

        /// <summary>
        /// Maximum width of the table.
        /// </summary>
        private int m_maxWidth;

        /// <summary>
        /// Maximum height of the table.
        /// </summary>
        private int m_maxHeight;

        /// <summary>
        /// Array which holds first cells in each column with equal colspan number.
        /// </summary>
        private ArrayList m_firstCellsInColumn;

        /// <summary>
        /// Array which holds first cells in each row with equal rowspan number.
        /// </summary>
        private ArrayList m_firstCellsInRow;

        /// <summary>
        /// Maximum colspan number of the cell in the table.
        /// </summary>
        private int m_maxColspanInTable;

        /// <summary>
        /// Maximum rowspan number of the cell in the table.
        /// </summary>
        private int m_maxRowspanInTable;

        /// <summary>
        /// Indicates whether it is the first time position is calculated.
        /// </summary>
        private bool m_bFirstRender;
        #endregion

        #region Class Properties
        /// <summary>
        /// Overridden. Returns an array of supported events.
        /// </summary>
        public override string[] SupportedEvents
        {
            get
            {
                return DEF_SUPP_EVENTS;
            }
        }

        /// <summary>
        /// Overridden. Gets or sets the format which is special for tag A (Hyperlink).
        /// </summary>
       protected internal override HTMLFormat OwnFormat
        {
            get
            {
                if (m_ownFormat == null)
                {
                    SetOwnFormat();
                }

                return m_ownFormat;
            }
        }

        /// <summary>
        /// Gets the number of rows in the table.
        /// </summary>
        [Browsable(false)]
        public int RowsCount
        {
            get
            {
                return m_numRows;
            }
        }

        /// <summary>
        /// Gets the number of columns in the table.
        /// </summary>
        [Browsable(false)]
        public int ColsCount
        {
            get
            {
                return m_numCols;
            }
        }

        /// <summary>
        /// Gets a value indicating whether positioning is being calculated for the first time.
        /// </summary>
        internal bool FirstRender
        {
            get
            {
                return m_bFirstRender;
            }
        }

        /// <summary>
        /// Gets the matrix of cells in the table.
        /// </summary>
        internal TDElementImpl[,] Matrix
        {
            get
            {
                return m_cells;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes static members of the TABLEElementImpl class 
        /// </summary>
        static TABLEElementImpl()
        {
            Type type = typeof(TABLEElementImpl);
            DEF_SUPP_EVENTS = FillEventsInfo(type, out m_eventHash);
        }

        /// <summary>
        /// Initializes a new instance of the TABLEElementImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public TABLEElementImpl(IHTMLElement parent)
            : base(parent, DEF_TAG_NAME)
        {
        }

        /// <summary>
        /// Overridden. Disposes element.
        /// </summary>
        protected override void OnDispose()
        {
            EventBaseCollection children = this.Children as EventBaseCollection;

            if (children != null)
            {
                children.Inserted -= new CollectionEventHandler(ChildrenChanged);
                children.Removed -= new CollectionEventHandler(ChildrenChanged);
            }

            base.OnDispose();

            // Dispose all arrays.
            if (m_cells != null)
            {
                m_cells = null;
            }

            if (m_bFixedColumns != null)
            {
                m_bFixedColumns = null;
            }

            if (m_maxColspanInColumn != null)
            {
                m_maxColspanInColumn = null;
            }

            if (m_maxRowspanInRow != null)
            {
                m_maxRowspanInRow = null;
            }

            if (m_firstCellsInColumn != null)
            {
                m_firstCellsInColumn.Clear();
                m_firstCellsInColumn = null;
            }

            if (m_firstCellsInRow != null)
            {
                m_firstCellsInRow.Clear();
                m_firstCellsInRow = null;
            }
        }
        #endregion

        #region Class event handlers
        /// <summary>
        /// Raised when the collection of children has changed.
        /// Here we have to update our data about table dimensions.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        protected internal void ChildrenChanged(object sender, CollectionEventArgs e)
        {
            InitializeData();
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Returns an instance of the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>Event object.</returns>
        protected override IHTMLEvent CreateEventInternal(string name)
        {
            return new HashElementEvents(this, m_eventHash, name);
        }

        /// <summary>
        /// Overridden. Calculates the size of the element for rendering.
        /// </summary>
        /// <returns>Size object</returns>
        protected override Size CalculateSizeInternal()
        {
            this.Type = ElementType.BlockNewLineSimple;
            Size size = Size.Empty;
            this.Size = size;

            if (!this.IsVisible) return this.Size;

            this.Size = GetFinalSizeInternal(size);
            this.MinWidth = this.Size.Width;

            SetElementStyle();
            size = CalculateSizeOfTRChildren();
            m_maxWidth = size.Width;
            m_maxHeight = size.Height;

            if (this.InsideTable)
            {
                this.Width = m_maxWidth;
            }
            else
            {
                this.Width = this.MinWidth;
            }

            this.Height = size.Height;

            return this.Size;
        }

        /// <summary>
        /// Overridden. Calculates the position of the element for rendering.
        /// </summary>
        protected override void CalculatePositionInternal()
        {
            BaseElement parent = (BaseElement)this.Parent;
            this.CurrentPosition = parent.CurrentPosition;

            CalculateChildPositions(this.CurrentPosition, parent.Bounds);
        }

        /// <summary>
        /// Overridden. Calculates the format of the element from the array of possible formats.
        /// </summary>
        protected override void CalculateFormatInternal()
        {
            DefaultCalculateFormatInternal();
        }

        /// <summary>
        /// Overridden. Calculates the positions in the table.
        /// </summary>
        /// <param name="curPosition">Current global position.</param>
        /// <param name="bounds">Max bounds for this element.</param>
        /// <returns>Array of blocks of this element.</returns>
        protected override BlocksCollection CalculateChildPositions(Point curPosition, Rectangle bounds)
        {
            Point firstPos = curPosition;
            m_bFirstRender = true;
            this.QuietMode = true;

            // Calculate first time all positioning.
            // Set type to fixed to skip resizing table size in first position
            // calculating algorithm.
            ElementType oldType = this.Type;
            this.Type = ElementType.BlockNewLineFixedSize;

            // If element is invisible, skip position calculating method.
            if (!this.IsVisible)
            {
                this.QuietMode = false;

                return new BlocksCollection();
            }

            BlocksCollection result = base.CalculateChildPositions(curPosition, bounds);

            this.Type = oldType;

            if (this.Type == ElementType.BlockNewLineSimple)
            {
                this.Size = Size.Empty;
            }

            ArrangeColumns();
            SetRowsWidth();

            result = base.CalculateChildPositions(firstPos, bounds);
            ArrangeRows();

            // Recalculate positioning if table has rowspan > 1 or height is resizable.
            m_bFirstRender = false;
            if (ResizeRows() || m_maxRowspanInTable > 1)
            {
                result = base.CalculateChildPositions(firstPos, bounds);
            }

            // Calculate table size after cell's modification.
            SetTableSize();
            this.QuietMode = false;
            OnLocationCalculated(EventArgs.Empty);

            return result;
        }

        /// <summary>
        /// Overridden. Initializes an element.
        /// </summary>
        protected internal override void InitializeElement()
        {
            base.InitializeElement();

            InitializeData();

            EventBaseCollection children = this.Children as EventBaseCollection;
            children.Inserted += new CollectionEventHandler(ChildrenChanged);
            children.Removed += new CollectionEventHandler(ChildrenChanged);
        }

        /// <summary>
        /// Overridden. Re-converts all children to tree of objects.
        /// </summary>
        /// <param name="xmlCurrent">Current XML element.</param>
        /// <param name="elementParent">Parent HTML element object.</param>
        /// <returns>Created HTML element object.</returns>
        protected override IHTMLElement ReConvertInnerHTML(XmlElement xmlCurrent, IHTMLElement elementParent)
        {
            IHTMLElement elm = base.ReConvertInnerHTML(xmlCurrent, elementParent);
            InitializeData();

            return elm;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Initializes all data needed for table rendering.
        /// </summary>
        private void InitializeData()
        {
            CalculateItemsCount();

            m_cells = null;
            m_cells = new TDElementImpl[this.RowsCount, this.ColsCount];
            m_maxColspanInColumn = new int[this.ColsCount];
            m_maxRowspanInRow = new int[this.RowsCount];
            m_bFixedColumns = new bool[this.ColsCount];
            m_firstCellsInColumn = new ArrayList();
            m_firstCellsInRow = new ArrayList();

            InfillMatrix();
            InFillMaxColspanValues();
            InFillMaxRowspanValues();
            InFillArrayByFirstColCells();
            InFillArrayByFirstRowCells();
        }

        /// <summary>
        /// Calculates the number of rows and columns in the table.
        /// </summary>
        private void CalculateItemsCount()
        {
            if (this.HasChildren)
            {
                IHTMLElement[] rows = GetChildRows();
                m_numRows = rows.Length;

                for (int i = 0; i < rows.Length; i++)
                {
                    TRElementImpl element = rows[i] as TRElementImpl;
                    m_numCols = Math.Max(m_numCols, element.VirtualCellsCount);
                }
            }
        }

        /// <summary>
        /// Calculates the sizes of all TR children and returns Size(MaxWidthOfTR, SumOfTRHeight).
        /// </summary>
        /// <returns>Size of children.</returns>
        private Size CalculateSizeOfTRChildren()
        {
            Size result = Size.Empty;
            int rowsMinHeight = 0;

            IHTMLElement[] rows = GetChildRows();
            if (rows.Length > 0)
            {
                BaseElement row = null;
                for (int i = 0, len = rows.Length; i < len; i++)
                {
                    row = rows[i] as BaseElement;
                    row.CalculateSize();

                    this.MinWidth = Math.Max(this.MinWidth, row.MinWidth);
                    rowsMinHeight += row.MinHeight;
                    //// this.MinHeight += row.MinHeight;

                    //// ALEXK:
                    result.Height += row.Height;
                    //// result.Height = Math.Max( result.Height, row.Height );
                    result.Width = Math.Max(result.Width, row.Width);
                }
            }
            else
            {
                result = Size.Empty;
            }

            result.Width = Math.Max(result.Width, this.Width);
            result.Height = Math.Max(result.Height, this.Height);
            this.MinHeight = Math.Max(this.MinHeight, rowsMinHeight);

            //// Expand width and height.
            if ((this.Type & ElementType.BlockFixed) == 0)
            {
                result.Width = ExpandWidth(result.Width);
                result.Height = ExpandHeight(result.Height);

                this.MinWidth = ExpandWidth(this.MinWidth);
                this.MinHeight = ExpandHeight(this.MinHeight);
            }

            return result;
        }

        /// <summary>
        /// Infills two-dimensional matrix by cells of the table. (Makes model of the table.)
        /// </summary>
        private void InfillMatrix()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            int i = 0;
            int j = 0;

            if (this.HasChildren)
            {
                IHTMLElement[] rows = GetChildRows();
                IHTMLElement[] colsList = null;

                // Go through rows.
                TRElementImpl row = null;
                for (int index = 0, len = rows.Length; index < len; index++)
                {
                    row = rows[index] as TRElementImpl;
                    row.Table = this;

                    if (row.HasChildren)
                    {
                        j = 0;
                        colsList = row.GetCells();

                        // Go through cells in row.
                        TDElementImpl cell = null;

                        for (int index2 = 0, len2 = colsList.Length; index2 < len2; index2++)
                        {
                            cell = colsList[index2] as TDElementImpl;
                            cell.Table = this;

                            InFillCellByColspan(cell, i, ref j);
                            //// InFillCellByRowspan( cell, i, j );
                            j += cell.Colspan;
                        }
                    }

                    i++;
                }
            }
        }

        /// <summary>
        /// Searches for empty cell in the matrix, in the current row starting from the specified j position.
        /// </summary>
        /// <param name="i">X coordinate of the cell.</param>
        /// <param name="j">Y coordinate of the cell.</param>
        /// <returns>True if free cell exists.</returns>
        private bool IsFreeCellInRow(int i, ref int j)
        {
            if (i < 0 || i >= this.RowsCount)
                throw new ArgumentOutOfRangeException("i", "Index i is not in array range.");

            if (j < 0 || j >= this.ColsCount)
                throw new ArgumentOutOfRangeException("j", "Index j is not in array range.");

            while (true)
            {
                j++;
                if (j >= this.ColsCount) return false;

                if (GetCell(i, j) == null) return true;
            }
        }

        /// <summary>
        /// Infills cells of matrix through all colspans of cell in the current row.
        /// </summary>
        /// <param name="cell">Cell object.</param>
        /// <param name="i">X coordinate of the cell.</param>
        /// <param name="j">Y coordinate of the cell.</param>
        private void InFillCellByColspan(TDElementImpl cell, int i, ref int j)
        {
            if (cell == null)
                throw new ArgumentNullException("cell");

            if (i < 0 || i >= this.RowsCount)
                throw new ArgumentOutOfRangeException("i", "Index i is not in array range.");

            if (j < 0 || j >= this.ColsCount)
                throw new ArgumentOutOfRangeException("j", "Index j is not in array range.");
            cell.SetIndex(i, false);
            cell.SetIndex(j, true);
            for (int l = 0, len = Math.Min(cell.Rowspan, this.RowsCount); l < len; l++)
            {
                int newj = j - 1;
                int newi = i + l;

                // Go through all colspans of the cell.
                for (int k = 0; k < cell.Colspan; k++)
                {
                    newj++;
                    if (GetCell(newi, newj) == null) 
                    {
                        //// Cell in matrix is empty.
                        SetCell(newi, newj, cell);
                    }
                    else 
                    {
                        //// Cell in matrix already busy.
                        if (IsFreeCellInRow(newi, ref newj)) 
                        {
                            //// Find empty cell in matrix in current row.
                            SetCell(newi, newj, cell);
                            cell.SetIndex(newj, true);
                            j = newj;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Infills cells of matrix through all rowspans of cell in the current column.
        /// </summary>
        /// <param name="cell">Cell object.</param>
        /// <param name="i">X coordinate of the cell.</param>
        /// <param name="j">Y coordinate of the cell.</param>
        private void InFillCellByRowspan(TDElementImpl cell, int i, int j)
        {
            if (cell == null)
                throw new ArgumentNullException("cell");

            if (i < 0 || i >= this.RowsCount)
                throw new ArgumentOutOfRangeException("i", "Index i is not in array range.");

            if (j < 0 || j >= this.ColsCount)
                throw new ArgumentOutOfRangeException("j", "Index j is not in array range.");

            cell.SetIndex(i, false);

            // Go through all rowspans of current cell ( in column )
            for (int l = 0, len = Math.Min(cell.Rowspan, this.RowsCount); l < len; l++)
            {
                if (GetCell(i + l, j) == null)
                {
                    SetCell(i + l, j, cell);
                }
            }
        }

        /// <summary>
        /// Arranges columns.
        /// </summary>
        private void ArrangeColumns()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            RangeColumns();
            if (!WidthIsGood() || (this.Type & ElementType.BlockNewLineSimple) == 0)
            {
                ResizeColumns();
            }
        }

        /// <summary>
        /// Arranges rows to equal height.
        /// </summary>
        private void ArrangeRows()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            TDElementImpl firstCell = null;

            for (int index = 0, len = m_firstCellsInRow.Count; index < len; index++)
            {
                firstCell = m_firstCellsInRow[index] as TDElementImpl;

                /* Get array of cells which are in the same row with current cell and calculate
                 * minimum height of the row and maximum height of the row.
                 */
                Size dimension;
                TDElementImpl[,] row = GetCellsInRow(firstCell.RowIndex, firstCell.Rowspan, out dimension);

                // Run through all cells in row and calculate MinHeight and MaxHeight of row.
                if (dimension.Width == 0 || dimension.Height == 0) return;

                Size height = GetMaxRowHeight(row, dimension);

                int maxMinHeight = height.Height;
                int maxHeight = height.Width;

                /*
                 * Assign to all cells in the row correct min height corresponding to each cell height and
                 * maximum height corresponding to cell height.
                 */
                ArrayList unprocessedCols = new ArrayList();

                for (int j = 0; j < dimension.Width; j++)
                {
                    if (NonEmptyColumn(row, j))
                    {
                        int colRowspan = GetRowspanOfColumn(row, j, dimension.Height);

                        if (colRowspan == firstCell.Rowspan)
                        {
                            float colHeight = GetColInRowHeight(row, j, dimension.Height);
                            ArrangeColumnInRow(row, j, dimension, colHeight, maxHeight, false);

                            // Expand min height of the row.
                            float colMinHeight = GetColInRowMinHeight(row, j, dimension.Height);
                            ArrangeColumnInRow(row, j, dimension, colMinHeight, maxMinHeight, true);
                        }
                        else if (colRowspan > 0)
                        {
                            unprocessedCols.Add(j);
                        }
                    }
                }
                //// Align columns which are less than row by rowspan value.
                for (int j = 0, len2 = unprocessedCols.Count; j < len2; j++)
                {
                    int colIndex = (int)unprocessedCols[j];
                    int colRowspan = GetRowspanOfColumn(row, colIndex, dimension.Height);
                    Size processedColHeight = GetArrangedColHeightForColumn(row, unprocessedCols, colRowspan);

                    //// Resize column.
                    if (processedColHeight.Width >= 0)
                    {
                        float colHeight = GetColInRowHeight(row, colIndex, dimension.Height);
                        maxHeight = processedColHeight.Width;
                        ArrangeColumnInRow(row, colIndex, dimension, colHeight, maxHeight, false);

                        float colMinHeight = GetColInRowMinHeight(row, colIndex, dimension.Height);
                        maxMinHeight = processedColHeight.Height;
                        ArrangeColumnInRow(row, colIndex, dimension, colMinHeight, maxMinHeight, true);
                    }
                }

                unprocessedCols.Clear();
            }

            m_maxHeight = CalculateMaxTableHeight();
        }

        /// <summary>
        /// Resizes width of cells in the same columns.
        /// </summary>
        private void RangeColumns()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            // Run through all columns in the table and arrange their.
            TDElementImpl firstCell = null;

            CellsEnumerator enumerator = new CellsEnumerator(m_firstCellsInColumn);
            while (enumerator.MoveNext())
            {
                firstCell = enumerator.CurrentCell;

                /* Get array of cells which are in the same column with current cell and calculate
                 * minimum width of the column and maximum width of the column.
                 */
                Size dimension;
                TDElementImpl[,] column = GetCellsInColumn(firstCell.ColumnIndex, firstCell.Colspan, out dimension);

                // Run through all cells in column and calculate MinWidth and MaxWidth of column.
                if (dimension.Width == 0 || dimension.Height == 0) return;

                Size width = GetMaxColumnWidth(column, dimension);
                int maxMinWidth = width.Height;
                int maxWidth = width.Width;

                /*
                * Assign to all cells in the column correct min width corresponding to each cell width and
                * maximum width corresponding to cell width.
                */
                ArrayList unprocessedRows = new ArrayList();

                for (int i = 0; i < dimension.Height; i++)
                {
                    if (NonEmptyRow(column, i))
                    {
                        int rowColspan = GetColspanOfRow(column, i, dimension.Width);

                        if (rowColspan == firstCell.Colspan)
                        {
                            float rowWidth = GetRowInColumnWidth(column, i, dimension.Width);
                            ArrangeRowInColumn(column, i, dimension, rowWidth, maxWidth, false);

                            float rowMinWidth = GetRowInColumnMinWidth(column, i, dimension.Width);
                            ArrangeRowInColumn(column, i, dimension, rowMinWidth, maxMinWidth, true);
                        }
                        else if (rowColspan > 0)
                        {
                            unprocessedRows.Add(i);
                        }
                    }
                }

                // Align rows which are less than column by colspan value.
                for (int i = 0, len2 = unprocessedRows.Count; i < len2; i++)
                {
                    int rowIndex = (int)unprocessedRows[i];
                    int rowColspan = GetColspanOfRow(column, rowIndex, dimension.Width);
                    Size processedRowWidth = GetArrangedRowWidthForRow(column, unprocessedRows, rowColspan);

                    // Resize row.
                    if (processedRowWidth.Width >= 0)
                    {
                        float rowWidth = GetRowInColumnWidth(column, rowIndex, dimension.Width);
                        maxWidth = processedRowWidth.Width;
                        ArrangeRowInColumn(column, rowIndex, dimension, rowWidth, maxWidth, false);

                        float rowMinWidth = GetRowInColumnMinWidth(column, rowIndex, dimension.Width);
                        maxMinWidth = processedRowWidth.Height;
                        ArrangeRowInColumn(column, rowIndex, dimension, rowMinWidth, maxMinWidth, true);
                    }
                }

                unprocessedRows.Clear();
            }

            m_maxWidth = CalculateMaxTableWidth();
        }

        /// <summary>
        /// Returns the maximum height the table can be.
        /// </summary>
        /// <returns>Height of the table.</returns>
        private int GetMaxTableHeight()
        {
            switch (this.Type)
            {
                // Table has fixed width value.
                case ElementType.BlockNewLineFixedSize: return this.Height;

                // Table hasn't any attributes on size.
                case ElementType.BlockNewLineSimple:

                // Table has percentage value of width
                case ElementType.BlockNewLineResizable:

                default: return GetResizableHeight();
            }
        }

        /// <summary>
        /// If the content of the rows is greater than the maximum size of the table, then changes 
        /// the width of the cells in each column.
        /// </summary>
        private void ResizeColumns()
        {
            // Difference between max table width and preferred (can be > 0 or < 0).
            int tableWidth = GetTableWidth();
            float difference = ExpandWidth(m_maxWidth - tableWidth);

            if (difference == 0) return;

            // Part of the table width which is resizable.
            float resTableWidth = GetResizableWidthOfTable((difference > 0));

            // Run through all columns in the table (including different colspans).
            TDElementImpl firstCell = null;

            for (int index = 0, len = m_firstCellsInColumn.Count; index < len; index++)
            {
                firstCell = m_firstCellsInColumn[index] as TDElementImpl;

                //// Column is resizable.
                if (firstCell.MinWidth < firstCell.Width || difference < 0)
                {
                    int columnWidth = firstCell.Width;
                    //// Get array of cells of current firstCell with the same colspan number.
                    ArrayList column = GetCellsInColumnWithSameColspan(firstCell);

                    bool isColumnFixed = IsColumnFixed(column);
                    float resizableColWidth = 0f;

                    if (difference < 0 && m_maxColspanInTable == 1 && !isColumnFixed)
                    {
                        resizableColWidth = columnWidth;
                    }
                    else
                    {
                        resizableColWidth = firstCell.Width - firstCell.MinWidth;
                    }

                    // Calculate value by what we must decrease column.
                    int decreaseValue = (int)Math.Floor(difference * resizableColWidth / resTableWidth);

                    // Calculate new width of the column.
                    int newWidth = (int)(firstCell.Width - decreaseValue);
                    newWidth = Math.Max(newWidth, firstCell.MinWidth);

                    // Run through column and change size.
                    if (newWidth != columnWidth)
                    {
                        TDElementImpl cell = null;

                        for (int i = 0, length = column.Count; i < length; i++)
                        {
                            cell = column[i] as TDElementImpl;

                            if (!(cell.IsStyleWidth && cell.GetWidthType() == SizeTypeEx.Number))
                            {
                                cell.Width = newWidth;
                            }
                        }
                    }
                }
            }

            // NOTE: If table has cell with colspan attribute > 1
            // range columns again.
            if (m_maxColspanInTable > 1)
            {
                RangeColumns();
            }
        }

        /// <summary>
        /// Checks whether column is fixed size or not by width (width is set).
        /// </summary>
        /// <param name="column"> Column of the table.</param>
        /// <returns>True if the column is fixed.</returns>
        private bool IsColumnFixed(ArrayList column)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            bool isFixed = false;

            for (int i = 0, len = column.Count; i < len; i++)
            {
                TDElementImpl cell = column[i] as TDElementImpl;
                if (cell != null && cell.IsStyleWidth && cell.GetWidthType() == SizeTypeEx.Number)
                {
                    isFixed = true;
                    break;
                }
            }

            return isFixed;
        }

        /// <summary>
        /// Resizes rows (increases height) if height of the table defined is greater
        /// than its inner content.
        /// </summary>
        /// <returns>bool value</returns>
        private bool ResizeRows()
        {
            if (!IsStyleHeight) return false;

            // Difference between max table height and preferred (can be > 0 or < 0).
            float difference = ExpandHeight(m_maxHeight - GetTableHeight());

            if (difference == 0) return false;

            // Part of the table width which is resizable.
            float resTableHeight = GetResizableHeightOfTable();

            // Run through all columns in the table (including different colspans).
            TDElementImpl firstCell = null;

            for (int index = 0, len = m_firstCellsInRow.Count; index < len; index++)
            {
                firstCell = m_firstCellsInRow[index] as TDElementImpl;

                //// Row is resizable. 
                //// TODO: Detect resizable row.
                //// if( firstCell.MinHeight < firstCell.Height )
                {
                    int rowHeight = firstCell.Height;

                    // Get array of cells of current firstCell with the same rowspan number.
                    ArrayList column = GetCellsInRowWithSameRowspan(firstCell);

                    // Get resizable part of column width.
                    float resizableRowHeight = firstCell.Height - firstCell.MinHeight;

                    //// if( resizableRowHeight != 0 )
                    {
                        // Calculate value by what we must decrease column.
                        int decreaseValue = (int)Math.Round(difference * rowHeight / m_maxHeight);

                        // Calculate new width of the column.
                        int newHeight = (int)(firstCell.Height - decreaseValue);
                        newHeight = Math.Max(newHeight, firstCell.MinHeight);

                        // Run through column and change size.
                        if (newHeight != rowHeight)
                        {
                            TDElementImpl cell = null;
                            for (int i = 0, length = column.Count; i < length; i++)
                            {
                                cell = column[i] as TDElementImpl;
                                cell.Height = newHeight;
                                cell.MainBlock.FinalHeight = newHeight;
                            }
                        }
                    }
                }
            }

            // NOTE: If table has cell with rowspan attribute > 1
            // range columns again.
            if (m_maxRowspanInTable > 1)
            {
                ArrangeRows();
            }

            return true;
        }

        /// <summary>
        /// Returns the width of the row (width of rows are the same).
        /// </summary>
        /// <returns>Gets width of the row.</returns>
        private int GetRowWidth()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            int result = 0;
            int colIndex = 0;
            TDElementImpl cell = null;

            while (true)
            {
                if (colIndex > this.ColsCount - 1) break;
                cell = GetCell(0, colIndex);

                if (cell != null)
                {
                    result += cell.Width;
                    colIndex += cell.Colspan;
                }
                else
                {
                    colIndex++;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the height of the inner content in the table.
        /// </summary>
        /// <returns>Height of the row.</returns>
        private int GetRowsHeight()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            int result = 0;
            int rowIndex = 0;
            TDElementImpl cell = null;

            while (true)
            {
                if (rowIndex > this.RowsCount - 1) break;

                cell = GetCell(rowIndex, 0);
                if (cell != null)
                {
                    result += cell.Height;
                    rowIndex += cell.Rowspan;
                }
                else
                {
                    rowIndex++;
                }
            }

            return result;
        }

        /// <summary>
        /// Indicates whether the width of the row is less than the max width of the table.
        /// </summary>
        /// <returns>True if width of the table is less than max width.</returns>
        private bool WidthIsGood()
        {
            return GetTableWidth() >= m_maxWidth;
        }

        /// <summary>
        /// Returns the number of columns which are resizable.
        /// </summary>
        /// <returns>Number of columns which can resize themselves.</returns>
        private int GetResizableColsCount()
        {
            if (m_resizableCount > -1)
            {
                return m_resizableCount;
            }

            int result = 0;
            for (int i = 0; i < m_bFixedColumns.Length; i++)
            {
                if (!m_bFixedColumns[i])
                {
                    result++;
                }
            }
            m_resizableCount = result;

            return m_resizableCount;
        }

        /// <summary>
        /// Returns the sum of width of all resizable columns.
        /// </summary>
        /// <returns>Width of columns which can resize themselves.</returns>
        private int GetResizableColumnsWidth()
        {
            if (GetResizableColsCount() == 0) return 0;

            int result = 0;
            TDElementImpl cell = null;
            for (int j = 0; j < m_bFixedColumns.Length; j++)
            {
                if (!m_bFixedColumns[j])
                {
                    cell = GetCell(0, j);
                    if (cell != null)
                    {
                        result += cell.Width - cell.MinWidth;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the minimum width of the specified column.
        /// </summary>
        /// <param name="index">Index of column.</param>
        /// <returns>Returns minimum width of column.</returns>
        private int GetMinWidthInColumn(int index)
        {
            if (index < 0 || index > this.ColsCount)
                throw new ArgumentOutOfRangeException("index", "index is out of range.");

            int result = 0;
            TDElementImpl cell = null;

            for (int i = 0; i < this.RowsCount; i++)
            {
                cell = GetCell(i, index);
                if (cell != null)
                {
                    result = Math.Max(result, cell.MinWidth);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the width of the column with the specified index.
        /// </summary>
        /// <param name="index">Index of the column.</param>
        /// <returns>Width of the column.</returns>
        private int GetColumnWidth(int index)
        {
            if (index < 0 || index > this.ColsCount)
                throw new ArgumentOutOfRangeException("index", "index is out of range");

            TDElementImpl cell = GetCell(0, index);

            if (cell != null)
            {
                return cell.Width;
            }

            throw new ArgumentNullException("cell", "cell can't be null");
        }

        /// <summary>
        /// Returns the height of the specified row.
        /// </summary>
        /// <param name="index">Index of the row.</param>
        /// <returns>Height of the row.</returns>
        private int GetRowHeight(int index)
        {
            if (index < 0 || index > this.RowsCount)
                throw new ArgumentOutOfRangeException("index", "index is out of range");

            TDElementImpl cell = GetCell(index, 0);

            if (cell != null && cell.Rowspan == 1)
            {
                return cell.Height;
            }

            throw new ArgumentNullException("cell", "cell can't be null");
        }

        /// <summary>
        /// Returns the rows for this table.
        /// </summary>
        /// <returns>Rows for this table.</returns>
        private IHTMLElement[] GetChildRows()
        {
            IHTMLElement[] rows = this.Children.GetElementsByName(TagName.Tr);
            if (rows.Length > 0) return rows;

            IHTMLElementsCollection children = this.Children;

            IHTMLElement child = null;
            for (int i = 0, len = children.Count; i < len; i++)
            {
                child = children[i];
                rows = child.Children.GetElementsByName(TagName.Tr);
                if (rows.Length > 0) return rows;
            }

            return new IHTMLElement[] { };
        }

        /// <summary>
        /// Sets own format for table (attaches attribute background color to element).
        /// </summary>
        private void SetOwnFormat()
        {
            m_ownFormat = base.OwnFormat;

            if (this.Attributes.Contains(AttributeName.BgColor))
            {
                IHTMLAttribute attr = this.Attributes[AttributeName.BgColor];
                if (attr != null)
                {
                    this.Control.FormatManager.SetBackgroundColor(m_ownFormat, attr.Value);
                    m_ownFormat.Merge |= MergeMask.BgColor;
                }
            }
        }

        /// <summary>
        /// Infills the array by maximum colspan in each column.
        /// </summary>
        private void InFillMaxColspanValues()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            int colspanCount = 0;
            TDElementImpl cell = null;
            m_maxColspanInTable = 0;

            for (int j = 0; j < this.ColsCount; j++)
            {
                for (int i = 0; i < this.RowsCount; i++)
                {
                    cell = GetCell(i, j);
                    if (cell != null && cell.ColumnIndex == j)
                    {
                        colspanCount = Math.Max(colspanCount, cell.Colspan);
                    }
                }
                m_maxColspanInColumn[j] = colspanCount;
                m_maxColspanInTable = Math.Max(m_maxColspanInTable, colspanCount);
                colspanCount = 0;
            }
        }

        /// <summary>
        /// Infills the array by maximum rowspan in each row.
        /// </summary>
        private void InFillMaxRowspanValues()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            int rowspanCount = 0;
            TDElementImpl cell = null;
            m_maxRowspanInTable = 0;

            for (int i = 0; i < this.RowsCount; i++)
            {
                for (int j = 0; j < this.ColsCount; j++)
                {
                    cell = GetCell(i, j);
                    if (cell != null && cell.RowIndex == i)
                    {
                        rowspanCount = Math.Max(rowspanCount, cell.Rowspan);
                    }
                }
                m_maxRowspanInRow[i] = rowspanCount;
                m_maxRowspanInTable = Math.Max(m_maxRowspanInTable, rowspanCount);
                rowspanCount = 0;          
            }
        }

        /// <summary>
        /// Returns the width of the table.
        /// </summary>
        /// <returns>Width of the table.</returns>
        private int GetTableWidth()
        {
            switch (this.Type)
            {
                // Table has fixed width value.
                case ElementType.BlockNewLineFixedSize:
                    return this.Width;

                // Table has percentage value of width.
                case ElementType.BlockNewLineResizable:
                    return Math.Max(this.MinWidth, GetResizableWidth());

                // Table hasn't any attributes on size.
                case ElementType.BlockNewLineSimple:
                default:
                    return Math.Max(this.MinWidth, Math.Min(m_maxWidth, GetResizableWidth()));
            }
        }

        /// <summary>
        /// Returns the height of the table.
        /// </summary>
        /// <returns>Height of the table.</returns>
        private int GetTableHeight()
        {
            if (IsStyleHeight && ((HTMLFormat)this.Format).HeightType == SizeTypeEx.Percent)
            {
                return Math.Max(this.MinHeight, GetResizableHeight());
            }

            if (IsStyleHeight && ((HTMLFormat)this.Format).HeightType == SizeTypeEx.Number)
            {
                return Math.Max(this.MinHeight, this.Height);
            }
             return m_maxHeight;
        }

        /// <summary>
        /// Infills the array by first cells in each column with equal colspan number.
        /// </summary>
        private void InFillArrayByFirstColCells()
        {
            if (m_firstCellsInColumn == null)
            {
                m_firstCellsInColumn = new ArrayList();
            }

            m_firstCellsInColumn.Clear();
            TDElementImpl cell = null;

            // Run from one to maximum colspan cell in the column.
            for (int colspanCount = 1; colspanCount <= m_maxColspanInTable; colspanCount++)
            {
                // Run through all columns.
                for (int j = 0; j < this.ColsCount; j++)
                {
                    // Do not check column if we already checked it all.
                    if (m_maxColspanInColumn[j] >= colspanCount)
                    {
                        // Run through all cells in row.
                        for (int i = 0; i < this.RowsCount; i++)
                        {
                            cell = GetCell(i, j);

                            if (cell != null && cell.Colspan == colspanCount)
                            {
                                m_firstCellsInColumn.Add(cell);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Infills the array by first cells in each row with equal rowspan number.
        /// </summary>
        private void InFillArrayByFirstRowCells()
        {
            if (m_firstCellsInRow == null)
            {
                m_firstCellsInRow = new ArrayList();
            }

            m_firstCellsInRow.Clear();
            TDElementImpl cell = null;

            // Run from one to maximum rowspan cell in the row.
            for (int rowspanCount = 1; rowspanCount <= m_maxRowspanInTable; rowspanCount++)
            {
                // Run through all cells in row.
                for (int i = 0; i < this.RowsCount; i++)
                {
                    // Do not check column if we already checked it all.
                    if (m_maxRowspanInRow[i] >= rowspanCount)
                    {
                        // Run through all columns.
                        for (int j = 0; j < this.ColsCount; j++)
                        {
                            cell = GetCell(i, j);

                            if (cell != null && cell.Rowspan == rowspanCount &&
                              !m_firstCellsInRow.Contains(cell))
                            {
                                m_firstCellsInRow.Add(cell);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a multidimensional array in current column. Returns only cells which are all inside current column.
        /// If in the row at least one cell exceeds the column, no cells in this row will be added to the result.
        /// </summary>
        /// <param name="columnIndex">Start column index.</param>
        /// <param name="colspan">Colspan number of the column.</param>
        /// <param name="arDim">Maximum dimension of output array.</param>
        /// <returns>Array of cells in order.</returns>
        private TDElementImpl[,] GetCellsInColumn(int columnIndex, int colspan, out Size arDim)
        {
            TDElementImpl[,] result = new TDElementImpl[this.RowsCount, colspan];
            int startIndex = columnIndex + colspan - 1;
            int endIndex = startIndex - colspan;
            TDElementImpl cell = null;
            //// TDElementImpl leftCell = null;
            int colIndex = 0;
            Size dimension = Size.Empty;
            ArrayList inserted = new ArrayList();

            // Move from right to left for good searching cells (we check by right border).
            for (int i = 0; i < this.RowsCount; i++)
            {
                for (int j = startIndex; j > endIndex; j--)
                {
                    // It seems that we don't have check it.
                    // Check if in current row from left border starts a cell.
                    /*leftCell = GetCell( i, columnIndex );

                    // If cell in left border starts earlier, skip this row.
                    if( leftCell == null || leftCell.ColumnIndex != columnIndex ) break;*/

                    // Check if cell is inside column and ends in subcolumn where we currently are.
                    cell = GetCell(i, j);
                    if (cell == null) break;

                    int rightBorder = cell.ColumnIndex + cell.Colspan - 1;

                    // Cell is inside column.
                    if (rightBorder == j && cell.ColumnIndex > endIndex && !inserted.Contains(cell))
                    {
                        // Define column index for inserting in result array.
                        colIndex = j - columnIndex - (cell.Colspan - 1);
                        result[i, colIndex] = cell;
                        inserted.Add(cell);

                        dimension.Width = Math.Max(dimension.Width, colIndex + 1);
                    }
                }
                dimension.Height++;
            }

            inserted.Clear();

            arDim = dimension;
            return result;
        }

        /// <summary>
        /// Returns a multidimensional array in current row. Returns only cells which are all inside current row.
        /// If in the row at least one cell exceeds row, no cells in this row will be added to the result.
        /// </summary>
        /// <param name="rowIndex">Start row index.</param>
        /// <param name="rowspan">Rowspan number of the cell.</param>
        /// <param name="arDim">Maximum dimension of output array.</param>
        /// <returns>Array of cells in order.</returns>
        private TDElementImpl[,] GetCellsInRow(int rowIndex, int rowspan, out Size arDim)
        {
            TDElementImpl[,] result = new TDElementImpl[rowspan, this.ColsCount];
            int startIndex = rowIndex + rowspan - 1;
            int endIndex = startIndex - rowspan;
            TDElementImpl cell = null;
            //// TDElementImpl topCell = null;
            int rowsIndex = 0;
            bool bwasAdded = false;
            Size dimension = Size.Empty;
            ArrayList inserted = new ArrayList();

            // Move from right to left for good searching cells (we check by right border).
            for (int j = 0; j < this.ColsCount; j++)
            {
                for (int i = startIndex; i > endIndex; i--)
                {
                    // It seems that we don't have check it.
                    // Check if in current column from top border starts some cell.
                    /*topCell = GetCell( rowIndex, j );

                    // If cell in top border starts earlier, skip this column.
                    if( topCell == null || topCell.RowIndex != rowIndex ) break;*/

                    // Check if cell is inside row and ends in subrow where we currently are.
                    cell = GetCell(i, j);
                    if (cell == null) break;

                    int bottomBorder = cell.RowIndex + cell.Rowspan - 1;

                    // Cell is inside row.
                    if (bottomBorder == i && cell.RowIndex > endIndex && !inserted.Contains(cell))
                    {
                        // Define row index for inserting in result array.
                        rowsIndex = i - rowIndex - (cell.Rowspan - 1);
                        result[rowsIndex, dimension.Width] = cell;
                        bwasAdded = true;
                        inserted.Add(cell);

                        dimension.Height = Math.Max(dimension.Height, rowsIndex + 1);
                    }
                }

                if (bwasAdded)
                {
                    dimension.Width++;
                    bwasAdded = false;
                }
            }

            inserted.Clear();

            arDim = dimension;
            return result;
        }

        /// <summary>
        /// Returns an array of the cells in the same column with current cell
        /// and with the same colspan number.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <returns>Array of the cells.</returns>
        private ArrayList GetCellsInColumnWithSameColspan(TDElementImpl cell)
        {
            if (cell == null)
                throw new ArgumentNullException("cell");

            ArrayList column = new ArrayList();
            TDElementImpl curCell = null;

            for (int i = 0; i < this.RowsCount; i++)
            {
                curCell = GetCell(i, cell.ColumnIndex);

                if (curCell != null && curCell.Colspan == cell.Colspan &&
                  curCell.ColumnIndex == cell.ColumnIndex)
                {
                    column.Add(curCell);
                }
            }

            return column;
        }

        /// <summary>
        /// Returns an array of the cells in the same row with current cell
        /// and with the same rowspan number.
        /// </summary>
        /// <param name="cell">Current cell.</param>
        /// <returns>Array of the cells.</returns>
        private ArrayList GetCellsInRowWithSameRowspan(TDElementImpl cell)
        {
            if (cell == null)
                throw new ArgumentNullException("cell");

            ArrayList row = new ArrayList();
            TDElementImpl curCell = null;

            for (int j = 0; j < this.ColsCount; j++)
            {
                curCell = GetCell(cell.RowIndex, j);

                if (curCell != null && curCell.Rowspan == cell.Rowspan &&
                  curCell.RowIndex == cell.RowIndex)
                {
                    row.Add(curCell);
                }
            }

            return row;
        }

        /// <summary>
        /// Returns the width of the row in the specified column.
        /// </summary>
        /// <param name="column">Column value</param>
        /// <param name="rowIndex">Index of the row in column.</param>
        /// <param name="rowLength">Length of the row.</param>
        /// <returns>Width of the row.</returns>
        private int GetRowInColumnWidth(TDElementImpl[,] column, int rowIndex, int rowLength)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            if (rowLength <= 0) return 0;

            int rowWidth = 0;
            TDElementImpl cell = null;

            for (int j = 0; j < rowLength; j++)
            {
                cell = column[rowIndex, j];

                if (cell != null)
                {
                    rowWidth += cell.Width;
                }
            }

            return rowWidth;
        }

        /// <summary>
        /// Returns the height of the column in the specified row.
        /// </summary>
        /// <param name="row">Current row.</param>
        /// <param name="colIndex">X index of column.</param>
        /// <param name="colLength">Length of columns.</param>
        /// <returns>Height of the column in the row.</returns>
        private int GetColInRowHeight(TDElementImpl[,] row, int colIndex, int colLength)
        {
            if (row == null)
                throw new ArgumentNullException("column");

            if (colLength <= 0) return 0;

            int colHeight = 0;
            TDElementImpl cell = null;

            for (int i = 0; i < colLength; i++)
            {
                cell = GetCellInRowFromLeft(row, i, colIndex); //// row[ i, colIndex ];
                if (cell != null)
                {
                    colHeight += cell.Height;
                }
            }

            return colHeight;
        }

        /// <summary>
        /// Searches for the cell by the specified coordinates.If cell by those coordinates is empty, 
        /// searches at the left because cell at the left has colspan > 1.
        /// </summary>
        /// <param name="row">Matrix of the cells.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the column.</param>
        /// <returns>Cell if found; Null otherwise.</returns>
        private TDElementImpl GetCellInRowFromLeft(TDElementImpl[,] row, int rowIndex, int colIndex)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            TDElementImpl result = row[rowIndex, colIndex];

            // Cell is found, return it.
            if (result != null) return result;

            // If cell in the upper occupies this column.
            if (IsUpperCellInColumn(row, rowIndex, colIndex)) return result;

            for (int j = colIndex; j >= 0; j--)
            {
                result = row[rowIndex, j];

                if (result != null) break;
            }

            return result;
        }

        /// <summary>
        /// Moves up on the column and searches the cells with big rowspan.
        /// </summary>
        /// <param name="row">Matrix of the cells.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the column.</param>
        /// <returns>True if cell in the top occupies defined place; false otherwise.</returns>
        private bool IsUpperCellInColumn(TDElementImpl[,] row, int rowIndex, int colIndex)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            bool result = false;
            TDElementImpl cell = null;

            for (int i = rowIndex; i >= 0; i--)
            {
                cell = row[i, colIndex];

                if (cell != null && ((cell.Rowspan + i) > rowIndex))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the min width of the row in the specified column.
        /// </summary>
        /// <param name="column">Column value</param>
        /// <param name="rowIndex">Index of the row in column.</param>
        /// <param name="rowLength">Length of the row.</param>
        /// <returns>Min width of the row in column.</returns>
        private int GetRowInColumnMinWidth(TDElementImpl[,] column, int rowIndex, int rowLength)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            if (rowLength <= 0) return 0;

            int rowMinWidth = 0;
            TDElementImpl cell = null;

            for (int j = 0; j < rowLength; j++)
            {
                cell = column[rowIndex, j];

                if (cell != null)
                {
                    rowMinWidth += cell.MinWidth;
                }
            }

            return rowMinWidth;
        }

        /// <summary>
        /// Returns the min height of the column in the specified row.
        /// </summary>
        /// <param name="row">Row value</param>
        /// <param name="colIndex">Index of the column in the row.</param>
        /// <param name="colLength">Length of the column.</param>
        /// <returns>Min height of the column in the row.</returns>
        private int GetColInRowMinHeight(TDElementImpl[,] row, int colIndex, int colLength)
        {
            if (row == null)
                throw new ArgumentNullException("column");

            if (colLength <= 0) return 0;

            int colMinHeight = 0;
            TDElementImpl cell = null;

            for (int i = 0; i < colLength; i++)
            {
                cell = row[i, colIndex];

                if (cell != null)
                {
                    colMinHeight += cell.MinHeight;
                }
            }

            return colMinHeight;
        }

        /// <summary>
        /// Calculates the maximum width of the table.
        /// </summary>
        /// <returns>Max width of the table.</returns>
        private int CalculateMaxTableWidth()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            int width = 0;
            int colIndex = 0;
            TDElementImpl cell = null;

            while (true)
            {
                if (colIndex > this.ColsCount - 1) break;

                cell = GetCell(0, colIndex);

                if (cell != null)
                {
                    width += cell.Width;
                    colIndex += cell.Colspan;
                }
                else
                {
                    colIndex++;
                }
            }

            return width;
        }

        /// <summary>
        /// Calculates the maximum height of the table.
        /// </summary>
        /// <returns>Max height of the table.</returns>
        private int CalculateMaxTableHeight()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            int height = 0;
            int rowIndex = 0;
            TDElementImpl cell = null;

            while (true)
            {
                if (rowIndex > this.RowsCount - 1) break;

                cell = GetCell(rowIndex, 0);

                if (cell != null)
                {
                    height += cell.Height;
                    rowIndex += cell.Rowspan;
                }
                else
                {
                    rowIndex++;
                }
            }

            return height;
        }

        /// <summary>
        /// Returns the resizable width of the table.
        /// </summary>
        /// <param name="bReduceTable">bool value</param>
        /// <returns>Width of resizable part of table.</returns>
        private int GetResizableWidthOfTable(bool bReduceTable)
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            int result = 0;
            int colIndex = 0;
            TDElementImpl cell = null;

            while (true)
            {
                if (colIndex > this.ColsCount - 1) break;

                cell = GetCell(0, colIndex);

                if (cell != null)
                {
                    if (!(!bReduceTable && m_maxColspanInTable == 1))
                    {
                        result += cell.Width - cell.MinWidth;
                    }
                    else
                    {
                        ArrayList column = GetCellsInColumnWithSameColspan(cell);
                        bool isFixed = IsColumnFixed(column);
                        if (!isFixed)
                        {
                            result += cell.Width;
                        }
                    }
                    colIndex += cell.Colspan;
                }
                else
                {
                    colIndex++;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the resizable height of the table.
        /// </summary>
        /// <returns>Height of resizable part of table.</returns>
        private int GetResizableHeightOfTable()
        {
            if (m_cells == null)
                throw new ArgumentNullException("m_cells");

            int result = 0;
            int rowIndex = 0;
            TDElementImpl cell = null;

            while (true)
            {
                if (rowIndex > this.RowsCount - 1) break;

                cell = GetCell(rowIndex, 0);

                if (cell != null)
                {
                    result += cell.Height - cell.MinHeight;
                    rowIndex += cell.Rowspan;
                }
                else
                {
                    rowIndex++;
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the size of the table after arranging cells in the table.
        /// </summary>
        private void SetTableSize()
        {
            if (this.MainBlock != null)
            {
                int tableWidth = ExpandWidth(GetRowWidth());
                int tableHeight = ExpandHeight(GetRowsHeight());

                this.Width = tableWidth;
                this.MainBlock.FinalWidth = tableWidth;

                this.Height = tableHeight;
                this.MainBlock.FinalHeight = tableHeight;
            }
        }

        /// <summary>
        /// Fixes the width of all row elements in the table.
        /// </summary>
        private void SetRowsWidth()
        {
            IHTMLElement[] rows = GetChildRows();
            if (rows.Length > 0)
            {
                BaseElement row = null;
                int width = GetRowWidth();
                for (int i = 0, len = rows.Length; i < len; i++)
                {
                    row = rows[i] as BaseElement;
                    row.Width = width;
                }
            }
        }

        /// <summary>
        /// Calculates the colspan number for the row.
        /// </summary>
        /// <param name="column">Array of cells.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="rowLength">Length of the row.</param>
        /// <returns>Colspan number for the row.</returns>
        private int GetColspanOfRow(TDElementImpl[,] column, int rowIndex, int rowLength)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            if (rowLength <= 0) return 0;

            int colspan = 0;

            TDElementImpl cell = null;

            for (int j = 0; j < rowLength; j++)
            {
                cell = column[rowIndex, j];

                if (cell != null)
                {
                    colspan += cell.Colspan;
                }
            }

            return colspan;
        }

        /// <summary>
        /// Calculates the rowspan number for the column.
        /// </summary>
        /// <param name="row">Array of cells.</param>
        /// <param name="colIndex">Index of the column.</param>
        /// <param name="colLength">Length of the column.</param>
        /// <returns>Rowspan number for the column.</returns>
        private int GetRowspanOfColumn(TDElementImpl[,] row, int colIndex, int colLength)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            if (colLength <= 0) return 0;

            int rowspan = 0;

            TDElementImpl cell = null;

            for (int j = 0; j < colLength; j++)
            {
                cell = row[j, colIndex];

                if (cell != null)
                {
                    rowspan += cell.Rowspan;
                }
            }

            return rowspan;
        }

        /// <summary>
        /// Searches the width of the processed row for row with the same start
        /// index by X and with same colspan value.
        /// </summary>
        /// <param name="column">Array of all cells.</param>
        /// <param name="unprocessedRows">List of unprocessed rows.</param>
        /// <param name="unprocessedColspan">Colspan of unprocessed current row.</param>
        /// <returns>Width and MinWidth of the processed row if found; -1 otherwise.</returns>
        private Size GetArrangedRowWidthForRow(TDElementImpl[,] column, ArrayList unprocessedRows, int unprocessedColspan)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            int width = column.GetLength(1);
            int height = column.GetLength(0);
            TDElementImpl cell = null;
            Size rowWidth = new Size(-1, -1);

            for (int i = 0; i < height; i++)
            {
                // Try to check row only if it's processed alreday.
                if (!unprocessedRows.Contains(i))
                {
                    int colspan = 0;
                    Size curWidth = Size.Empty;

                    for (int j = 0; j < width; j++)
                    {
                        cell = column[i, j];

                        if (cell != null)
                        {
                            colspan += cell.Colspan;
                            curWidth.Width += cell.Width;
                            curWidth.Height += cell.MinWidth;
                        }

                        if (unprocessedColspan == colspan)
                        {
                            rowWidth = curWidth;

                            return rowWidth;
                        }
                    }
                }
            }

            return rowWidth;
        }

        /// <summary>
        /// Searches the height of the processed column for column height the same start
        /// index by Y and height same rowspan value.
        /// </summary>
        /// <param name="row">Array of all cells.</param>
        /// <param name="unprocessedCols">List of unprocessed columns.</param>
        /// <param name="unprocessedRowspan">Rowspan of unprocessed current column.</param>
        /// <returns>Height and MinHeight of the processed column if found; -1 otherwise.</returns>
        private Size GetArrangedColHeightForColumn(TDElementImpl[,] row, ArrayList unprocessedCols, int unprocessedRowspan)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            int width = row.GetLength(1);
            int height = row.GetLength(0);
            TDElementImpl cell = null;
            Size rowHeight = new Size(-1, -1);

            for (int j = 0; j < width; j++)
            {
                // Try to check row only if it's processed alreday.
                if (!unprocessedCols.Contains(j))
                {
                    int rowspan = 0;
                    Size curHeight = Size.Empty;

                    for (int i = 0; i < height; i++)
                    {
                        cell = row[i, j];

                        if (cell != null)
                        {
                            rowspan += cell.Rowspan;
                            curHeight.Width += cell.Height;
                            curHeight.Height += cell.MinHeight;
                        }

                        if (unprocessedRowspan == rowspan)
                        {
                            rowHeight = curHeight;

                            return rowHeight;
                        }
                    }
                }
            }

            return rowHeight;
        }

        /// <summary>
        /// Indicates whether the row is empty.
        /// </summary>
        /// <param name="column">Array of cells.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>True if row is not empty; False otherwise.</returns>
        private bool NonEmptyRow(TDElementImpl[,] column, int rowIndex)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            return column[rowIndex, 0] != null;
        }

        /// <summary>
        /// Indicates whether the column is empty.
        /// </summary>
        /// <param name="row">Array of cells.</param>
        /// <param name="colIndex">Index of the column.</param>
        /// <returns>True if column is not empty; False otherwise.</returns>
        private bool NonEmptyColumn(TDElementImpl[,] row, int colIndex)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            return row[0, colIndex] != null;
        }

        /// <summary>
        /// Calculates the maximum width and maximum min width for the column.
        /// </summary>
        /// <param name="column">Array of cells.</param>
        /// <param name="dimension">Dimension of the array.</param>
        /// <returns>Size struct, where Width - maximum width, Height - maximum min width.</returns>
        private Size GetMaxColumnWidth(TDElementImpl[,] column, Size dimension)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            Size maxWidth = Size.Empty;
            TDElementImpl innerCell = null;

            for (int i = 0; i < dimension.Height; i++)
            {
                int innerMinWidth = 0;
                int innerMaxWidth = 0;

                if (NonEmptyRow(column, i))
                {
                    for (int j = 0; j < dimension.Width; j++)
                    {
                        innerCell = column[i, j];

                        //// Sum all attributes for cells in the same row of the current column.
                        if (innerCell != null)
                        {
                            innerMinWidth += innerCell.MinWidth;
                            innerMaxWidth += innerCell.Width;
                            innerCell.Type = ElementType.BlockSimple;
                        }
                    }
                    //// Define maximum of min and max width between different rows of the column.
                    maxWidth.Height = Math.Max(maxWidth.Height, innerMinWidth);
                    maxWidth.Width = Math.Max(maxWidth.Width, innerMaxWidth);
                }
            }

            return maxWidth;
        }

        /// <summary>
        /// Calculates the maximum height and maximum min height for the row.
        /// </summary>
        /// <param name="row">Array of cells.</param>
        /// <param name="dimension">Dimension of the array.</param>
        /// <returns>Size struct, where Width - maximum height, Height - maximum min height.</returns>
        private Size GetMaxRowHeight(TDElementImpl[,] row, Size dimension)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            Size maxHeight = Size.Empty;
            TDElementImpl innerCell = null;

            for (int j = 0; j < dimension.Width; j++)
            {
                int innerMinHeight = 0;
                int innerMaxHeight = 0;

                for (int i = 0; i < dimension.Height; i++)
                {
                    innerCell = row[i, j];

                    // Sum all attributes for cells in the same column inside current row.
                    if (innerCell != null)
                    {
                        Rectangle rect = innerCell.MainBlock.Rectangle;
                        innerMinHeight += rect.Height;
                        innerMaxHeight += innerCell.Height;
                    }
                }

                // Define maximum of min and max height between different columns of the row.
                maxHeight.Height = Math.Max(maxHeight.Height, innerMinHeight);
                maxHeight.Width = Math.Max(maxHeight.Width, innerMaxHeight);
            }

            return maxHeight;
        }

        /// <summary>
        /// Arranges the row inside column.
        /// </summary>
        /// <param name="column">Array of all cells in the column.</param>
        /// <param name="rowIndex">Row index</param>
        /// <param name="dimension">Dimension of the array.</param>
        /// <param name="rowWidth">Current width of the row.</param>
        /// <param name="maxWidth">Maximum width of the column.</param>
        /// <param name="bIsMinWidth">If true - we arrange minimum width, if False - just width of the row.</param>
        private void ArrangeRowInColumn(TDElementImpl[,] column, int rowIndex, Size dimension, float rowWidth, float maxWidth, bool bIsMinWidth)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            if (rowWidth < maxWidth)
            {
                TDElementImpl innerCell = null;
                float difference = maxWidth - rowWidth;

                for (int j = 0; j < dimension.Width; j++)
                {
                    innerCell = column[rowIndex, j];

                    if (innerCell != null)
                    {
                        float cellWidth = bIsMinWidth ? innerCell.MinWidth : innerCell.Width;

                        // Part of cell width to width of column containing this cell.
                        float cellPart = (rowWidth != 0.0f) ? (cellWidth / rowWidth) :
                          ((float)innerCell.Colspan / (float)dimension.Width);

                        int increaseVal = (int)Math.Round(difference * cellPart);

                        if (bIsMinWidth)
                        {
                            innerCell.MinWidth += increaseVal;
                        }
                        else
                        {
                            innerCell.Width += increaseVal;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Arranges the column inside row.
        /// </summary>
        /// <param name="row">Array of all cells in the row.</param>
        /// <param name="colIndex">Index of the column in the row.</param>
        /// <param name="dimension">Dimension of the array.</param>
        /// <param name="colHeight">Current height of the column.</param>
        /// <param name="maxHeight">Maximum height of the column.</param>
        /// <param name="bIsMinHeight">If true - we arrange minimum height, if False - just height of the row.</param>
        private void ArrangeColumnInRow(TDElementImpl[,] row, int colIndex, Size dimension, float colHeight, float maxHeight, bool bIsMinHeight)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            if (colHeight < maxHeight)
            {
                float difference = maxHeight - colHeight;
                TDElementImpl innerCell = null;

                for (int i = 0; i < dimension.Height; i++)
                {
                    innerCell = row[i, colIndex];

                    if (innerCell != null)
                    {
                        float cellHeight = bIsMinHeight ? innerCell.MinHeight : innerCell.Height;

                        // Part of the cell height to height of column containing cell.
                        float cellPart = (colHeight != 0.0f) ?
                          (cellHeight / colHeight) : ((float)innerCell.Rowspan / (float)dimension.Height);
                        int increaseVal = (int)Math.Round(difference * cellPart);

                        if (bIsMinHeight)
                        {
                            innerCell.MinHeight += increaseVal;
                        }
                        else
                        {
                            innerCell.Height += increaseVal;
                            innerCell.MainBlock.FinalHeight = innerCell.Height;
                            innerCell.MainBlock.FinalWidth = innerCell.Width;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets cell into the matrix of the cells.
        /// </summary>
        /// <param name="i">Index of the row.</param>
        /// <param name="j">Index of the column.</param>
        /// <param name="cell">Cell object.</param>
        /// <returns>True - if cell added to the matrix, false otherwise.</returns>
        private bool SetCell(int i, int j, TDElementImpl cell)
        {
            bool result = false;

            int rowsCount = m_cells.GetLength(0);
            int colsCount = m_cells.GetLength(1);

            if (m_cells != null && cell != null &&
              rowsCount > i && colsCount > j)
            {
                m_cells[i, j] = cell;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Returns cell by its indices.
        /// </summary>
        /// <param name="i">Index of the row.</param>
        /// <param name="j">Index of the column.</param>
        /// <returns>Cell by its indices if found, null otherwise.</returns>
        private TDElementImpl GetCell(int i, int j)
        {
            TDElementImpl cell = null;

            int rowsCount = m_cells.GetLength(0);
            int colsCount = m_cells.GetLength(1);

            if (m_cells != null && rowsCount > i && colsCount > j)
            {
                cell = m_cells[i, j];
            }

            return cell;
        }
        #endregion

        #region Internal classes
        /// <summary>
        /// Class that allows movement by cells during table aligning.
        /// </summary>
        private class CellsEnumerator
        {
            #region Class members
            /// <summary>
            /// Collection of cells.
            /// </summary>
            private ArrayList m_cells;

            /// <summary>
            /// Current cell returned.
            /// </summary>
            private TDElementImpl m_currentCell;

            /// <summary>
            /// Current index during running through collection.
            /// </summary>
            private int m_currentIndex;

            /// <summary>
            /// Indicates which cells were processed.
            /// </summary>
            private bool[] m_usedCells;

            /// <summary>
            /// Order of the cells in the column / row of the table.
            /// </summary>
            private int m_currentOrder;

            /// <summary>
            /// Colspan of the current cell.
            /// </summary>
            private int m_currentColspan;
            #endregion

            #region Class properties
            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public object Current
            {
                get
                {
                    return m_currentCell;
                }
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public TDElementImpl CurrentCell
            {
                get
                {
                    return m_currentCell;
                }
            }
            #endregion

            #region Class Initialize/Finalize methods
            /// <summary>
            /// Initializes a new instance of the CellsEnumerator class
            /// </summary>
            /// <param name="cells">Collection of the cells.</param>
            public CellsEnumerator(ArrayList cells)
            {
                if (cells == null)
                    throw new ArgumentNullException("cells");

                m_cells = cells;

                Reset();
            }
            #endregion

            #region Class Public Methods
            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                m_usedCells = new bool[m_cells.Count];
                m_currentCell = null;
                m_currentIndex = -1;
                m_currentOrder = 0;
                m_currentColspan = 1;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>True if the enumerator was successfully advanced to the next element; 
            /// false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                bool bMoveNext = false;
                int cellIndex;

                TDElementImpl maxCell = GetMaxColspanCellInColumn(m_currentOrder, m_currentColspan, out cellIndex);

                if (maxCell != null)
                {
                    m_currentCell = maxCell;
                    SetUsed(cellIndex);
                    bMoveNext = true;
                }
                else
                {
                    int nextIndex = GetFirstUnusedCellIndex();

                    if (nextIndex >= 0)
                    {
                        m_currentIndex = nextIndex;
                        m_currentCell = m_cells[nextIndex] as TDElementImpl;

                        m_currentOrder = m_currentCell.ColumnIndex + m_currentCell.Colspan - 1;
                        m_currentColspan = m_currentCell.Colspan;

                        SetUsed(nextIndex);
                        bMoveNext = true;
                    }
                }

                return bMoveNext;
            }
            #endregion

            #region Class helper methods
            /// <summary>
            /// Returns the first unused cell in the collection.
            /// </summary>
            /// <returns>Index of the unused cell if found; -1 otherwise.</returns>
            private int GetFirstUnusedCellIndex()
            {
                int index = -1;

                for (int i = m_currentIndex + 1, len = m_cells.Count; i < len; i++)
                {
                    bool isUsed = IsUsed(i);

                    if (!isUsed)
                    {
                        index = i;
                        break;
                    }
                }

                return index;
            }

            /// <summary>
            /// Indicates whether the cell was used.
            /// </summary>
            /// <param name="cellIndex">Index of the cell.</param>
            /// <returns>True if cell was used; False otherwise.</returns>
            private bool IsUsed(int cellIndex)
            {
                if (cellIndex < 0 || cellIndex >= m_cells.Count)
                    throw new ArgumentOutOfRangeException("cellIndex", cellIndex, "Value can not be less 0 and greater m_cells.Count");

                return m_usedCells[cellIndex];
            }

            /// <summary>
            /// Searches for a cell ending by specified index by column with maximum colspan value.
            /// </summary>
            /// <param name="colEndIndex">Index of ending column.</param>
            /// <param name="curColspan">Current colspan processing.</param>
            /// <param name="cellIndex">Index of the cell</param>
            /// <returns>Cell ending by specified index by column with maximum colspan
            /// value if found; Null otherwise.</returns>
            private TDElementImpl GetMaxColspanCellInColumn(int colEndIndex, int curColspan, out int cellIndex)
            {
                TDElementImpl result = null;
                cellIndex = -1;

                for (int i = m_currentIndex + 1, len = m_cells.Count; i < len; i++)
                {
                    TDElementImpl cell = (TDElementImpl)m_cells[i];

                    if (cell != null && (cell.Colspan > curColspan) && !IsUsed(i))
                    {
                        int cellColEndIndex = cell.ColumnIndex + cell.Colspan - 1;

                        if (cellColEndIndex == colEndIndex)
                        {
                            result = cell;
                            cellIndex = i;
                            break;
                        }
                    }
                }

                return result;
            }

            /// <summary>
            /// Sets the cell as used.
            /// </summary>
            /// <param name="index">Index of the cell.</param>
            private void SetUsed(int index)
            {
                if (index >= 0 && index < m_usedCells.Length)
                {
                    m_usedCells[index] = true;
                }
            }
            #endregion
        }
        #endregion
    }
}
