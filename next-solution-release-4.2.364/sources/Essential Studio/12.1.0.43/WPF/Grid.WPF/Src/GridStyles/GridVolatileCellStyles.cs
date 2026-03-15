#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Interop;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.Styles;


// DefaultCellStyle, 
// DefaultColumnHeaderStyle, DefaultColumnFooterStyle, 
// DefaultRowHeaderStyle, DefaultRowFooterStyle
// ColumnStyle[], ColumnHeaderStyle[], ColumnBodyStyle[], ColumnFooterStyle[]
// RowStyle[], RowHeaderStyle[], RowBodyStyle[], RowFooterStyle[]

// RemoveRows, MoveRows, DeleteRows etc. events will simply empty the cache.

// Clear will remove all weak references. Although the referenced style will 
// not necessarily be finalized immediately, they won't be accessed through VolatileData 
// anymore. VolatileData will return a new style object instead.


namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Defines the work flow for volatile cell styles rendering.
    /// </summary>
    public interface IGridVolatileCellStylesHost
    {
        /// <summary>
        /// Occurs when the model queries for style information about a specific cell.
        /// </summary>
        void QueryCellInfo(RowColumnIndex cell, GridStyleInfo style);
        /// <summary>
        /// Occurs when the model is about to save style information about a specific cell.
        /// </summary>
        void CommitCellInfo(RowColumnIndex cell, GridStyleInfo style, StyleInfoProperty sip);
        /// <summary>
        /// Occurs when the model queries information about base styles at a specific cell.
        /// </summary>
        IStyleInfo[] QueryBaseStyles(RowColumnIndex cell, GridStyleInfo style);
        /// <summary>
        /// Returns a <see cref="GridCellModelBase"/> for the specified id / cell type name.
        /// </summary>
        /// <param name="id">Cell type name.</param>
        /// <returns>The <see cref="GridCellModelBase"/> for the given id.</returns>
        /// <remarks>
        /// Calls <see cref="IGridData.LookupCellModel"/>.
        /// </remarks>
        GridCellModelBase LookupCellModel(string id);
        /// <summary>
        /// Gets the <see cref="GridBaseStylesMap"/> that is associated with this <see cref="GridModel"/>.
        /// </summary>
        GridBaseStylesMap BaseStylesMap { get; }
    }

    /// <summary>
    /// For internal use.
    /// </summary>
    public class GridCellsInRowCache
    {
        public Dictionary<int, WeakReference> Cells = new Dictionary<int, WeakReference>();
    }

    /// <summary>
    /// For internal use.
    /// </summary>
    public class GridRowsCache : Dictionary<int, GridCellsInRowCache>
    {
    }

    /// <summary>
    /// This Interface defines the methods for the volatile data store used
    /// by the grid to save, retrieve and associate GridStyleInfo objects with cells.
    /// </summary>
    /// <remarks>
    /// <example>
    /// This example shows how to attach a CustomVolatileData object derived from IGridVolatileData.
    /// <code lang="C#">
    /// public Window1()
    /// {
    ///     InitializeComponent();
    /// 
    ///     // Replace default GridVolatileData with CustomVolatileData
    ///     grid.Model.VolatileCellStylesFactoryMethod = delegate(GridModel model)
    ///     {
    ///         return new GridVolatileCellStyles(model, new CustomVolatileData());
    ///     };
    /// 
    ///     // a really large row and column count.
    ///     grid.Model.RowCount = 9900;
    ///     grid.Model.ColumnCount = 100; // 1 million
    /// 
    ///     // fill cell contents on demand.
    ///     grid.Model.QueryCellInfo += new GridQueryCellInfoEventHandler(Model_QueryCellInfo);
    /// }
    /// </code>
    /// </example>
    /// <code lang="VB">
    /// Public Sub New()
    ///     InitializeComponent()
    /// 
    ///     ' Replace default GridVolatileData with CustomVolatileData
    ///     grid.Model.VolatileCellStylesFactoryMethod = 
    ///         Function(model As GridModel) New GridVolatileCellStyles(model, New CustomVolatileData())
    /// 
    ///     ' a really large row and column count.
    ///     grid.Model.RowCount = 9900
    ///     grid.Model.ColumnCount = 100
    ///     ' 1 million
    ///     ' Resize millions of rows instantly - pixel scrolling is updated accordingly.
    ///     grid.Model.TableStyle.CellType = "TextBox"
    ///     ' see checkBoxRows_Checked below for instantly hiding most of the 99 million rows
    /// 
    ///     AddHandler grid.Model.QueryCellInfo, AddressOf Model_QueryCellInfo
    /// End Sub
    /// </code>
    /// </example>
    /// </remarks>
    public interface IGridVolatileData
    {
        /// <summary>
        /// Gets the GridStyleInfo for a cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        GridStyleInfo GetItem(RowColumnIndex cell);

        /// <summary>
        /// Sets the GridStyleInfo for a cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="style"></param>
        void SetItem(RowColumnIndex cell, GridStyleInfo style);

        /// <summary>
        /// Clears all cells.
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears a span of cells.
        /// </summary>
        /// <param name="cellSpan"></param>
        void Clear(CellSpanInfoBase cellSpan);

        /// <summary>
        /// Clears one cells.
        /// </summary>
        /// <param name="cell"></param>
        void Clear(RowColumnIndex cell);
        
        /// <summary>
        /// Insert columns
        /// </summary>
        /// <param name="insertAtColumnIndex"></param>
        /// <param name="count"></param>
        /// <param name="moveCells"></param>
        void InsertColumns(int insertAtColumnIndex, int count, IGridVolatileData moveCells);

        /// <summary>
        /// Insert rows.
        /// </summary>
        /// <param name="insertAtRowIndex"></param>
        /// <param name="count"></param>
        /// <param name="moveCells"></param>
        void InsertRows(int insertAtRowIndex, int count, IGridVolatileData moveCells);
        
        /// <summary>
        /// Remove columns.
        /// </summary>
        /// <param name="removeAtColumnIndex"></param>
        /// <param name="count"></param>
        /// <param name="moveCells"></param>
        void RemoveColumns(int removeAtColumnIndex, int count, IGridVolatileData moveCells);

        /// <summary>
        /// Remove rows.
        /// </summary>
        /// <param name="removeAtRowIndex"></param>
        /// <param name="count"></param>
        /// <param name="moveCells"></param>
        void RemoveRows(int removeAtRowIndex, int count, IGridVolatileData moveCells);
    }

    /// <summary>
    /// For internal use.
    /// </summary>
    public class GridVolatileData : IGridVolatileData
    {
        GridRowsCache data = new GridRowsCache();
        int count = 0;

        System.Threading.Thread guiThread;
        Dictionary<RowColumnIndex, bool> resetCellsList;

        internal int forceGcCollect = int.MaxValue;
        GridVolatileCellStyles volatileCellStyles;
        internal Func<RowColumnIndex, bool> isVisibleCell;

        public GridVolatileData(GridVolatileCellStyles volatileCellStyles)
        {
            resetCellsList = new Dictionary<RowColumnIndex, bool>();
            guiThread = Thread.CurrentThread;
            this.volatileCellStyles = volatileCellStyles;
        }

        /// <summary>
        /// Gets the cached style information for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinate.</param>
        /// <returns>The cached GridStyleInfo; NULL if no cache entry was found.</returns>
        public GridStyleInfo GetItem(RowColumnIndex cell)
        {
            GridCellsInRowCache cellsInRow;
            if (data.TryGetValue(cell.RowIndex, out cellsInRow))
            {
                WeakReference obj;
                if (cellsInRow.Cells.TryGetValue(cell.ColumnIndex, out obj))
                {
                    return (GridStyleInfo)obj.Target;
                }
            }
            return null;
        }

        /// <summary>
        /// Saves style information to be cached.
        /// </summary>
        /// <param name="cell">The cell coordinate.</param>
        /// <param name="style">The GridStyleInfo to be cached.</param>
        public void SetItem(RowColumnIndex cell, GridStyleInfo style)
        {
            try
            {
                if (count >= forceGcCollect)
                    GC.Collect();

                ProcessResetCellsList();

                if (SetItemInternal(cell, style))
                    count++;
            }
            catch (InvalidOperationException ex)
            {
                // should be InvalidOperation_HashInsertFailed=Hashtable insert failed.  Load factor too high.
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    Trace.WriteLine("Detected too many items in volatile date hashtable. Reducing VolatileData.forceGcCollect to a value below " + data.Count.ToString());
                    TraceUtil.TraceCurrentMethodInfo("Hashtable count: ", count);
                }
                forceGcCollect = Math.Max(1000, count - 1000);
                GC.Collect();
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    TraceUtil.TraceCurrentMethodInfo("Hashtable count after GC.Collect: ", count);
                }
                SetItem(cell, style);
            }
        }

        /// <summary>
        /// Saves style information to be cached.
        /// </summary>
        /// <param name="cell">The cell coordinate.</param>
        /// <param name="style">The GridStyleInfo to be cached.</param>
        /// <returns>True if a new entry was created in hashtable; false if entry was replaced.</returns>
        private bool SetItemInternal(RowColumnIndex cell, GridStyleInfo style)
        {
            GridCellsInRowCache cellsInRow;
            if (!data.TryGetValue(cell.RowIndex, out cellsInRow))
            {
                cellsInRow = new GridCellsInRowCache();
                data.Add(cell.RowIndex, cellsInRow);
            }

            WeakReference obj;
            if (!cellsInRow.Cells.TryGetValue(cell.ColumnIndex, out obj))
            {
                obj = new WeakReference(style);
                cellsInRow.Cells.Add(cell.ColumnIndex, obj);
                return true;
            }
            else
            {
                obj.Target = style;
                return false;
            }
        }

        /// <summary>
        /// Resets cache for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinates.</param>
        public void ResetItem(RowColumnIndex cell)
        {
            if (Thread.CurrentThread != guiThread)
            {
                // In GC Thread - prevent ProcessResetCellsList race condition
                Monitor.Enter(this);
                try
                {
                    resetCellsList[cell] = true;
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
            else
            {
                ResetItemHelper(cell);
            }
        }

        private bool ResetItemHelper(RowColumnIndex cell)
        {
            GridCellsInRowCache cellsInRow;
            if (!data.TryGetValue(cell.RowIndex, out cellsInRow))
                return false;

            return ResetItemHelper(cellsInRow, cell);
        }

        private bool ResetItemHelper(GridCellsInRowCache cellsInRow, RowColumnIndex cell)
        {
            if (data.TryGetValue(cell.RowIndex, out cellsInRow))
            {
                WeakReference obj;
                if (cellsInRow.Cells.TryGetValue(cell.ColumnIndex, out obj))
                {
                    obj.Target = null;

                    if (!IsVisibleCell(cell))
                    {
                        count--;
                        if (cellsInRow.Cells.Count > 1)
                            cellsInRow.Cells.Remove(cell.ColumnIndex);
                        else
                            data.Remove(cell.RowIndex);

                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsVisibleCell(RowColumnIndex cell)
        {
            return isVisibleCell(cell);
        }

        /// <summary>
        /// Resets cache for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinates.</param>
        public void Clear(RowColumnIndex cell)
        {
            ResetItem(cell);
        }

        /// <summary>
        /// Empty the cache.
        /// </summary>
        public void Clear()
        {
            data.Clear();
            resetCellsList = new Dictionary<RowColumnIndex, bool>();
            count = 0;
        }

        /// <summary>
        /// Removes the given cell span from the cache.
        /// </summary>
        /// <param name="cellSpan">Cell spanned range.</param>
        public void Clear(CellSpanInfoBase cellSpan)
        {
            long l = (long)cellSpan.Height * cellSpan.Width;

            if (l < 0 || l > data.Count)
            {
                List<RowColumnIndex> toReset = new List<RowColumnIndex>();
                //ProcessResetCellsList();
                foreach (KeyValuePair<int, GridCellsInRowCache> rowEntry in data)
                {
                    if (cellSpan.ContainsRow(rowEntry.Key))
                    {
                        foreach (KeyValuePair<int, WeakReference> cellEntry in rowEntry.Value.Cells)
                        {
                            if (cellSpan.ContainsColumn(cellEntry.Key))
                            {
                                cellEntry.Value.Target = null;
                                RowColumnIndex cell = new RowColumnIndex(rowEntry.Key, cellEntry.Key);
                                if (!IsVisibleCell(cell))
                                {
                                    toReset.Add(cell);
                                }
                            }
                        }
                    }
                }
                foreach (RowColumnIndex cell in toReset)
                    ResetItemHelper(cell);
            }
            else
            {
                for (int rowIndex = cellSpan.Top; rowIndex <= cellSpan.Bottom; rowIndex++)
                {
                    // SH 4/6/10: Avoid checking each cell when row is empty. Also,
                    // keep reusing row object for columns.

                    GridCellsInRowCache cellsInRow;
                    if (!data.TryGetValue(rowIndex, out cellsInRow))
                        continue;

                    for (int columnIndex = cellSpan.Left; columnIndex <= cellSpan.Right; columnIndex++)
                    {
                        RowColumnIndex cell = new RowColumnIndex(rowIndex, columnIndex);
                        ResetItemHelper(cellsInRow, cell);
                    }
                }
            }

            ProcessResetCellsList();
        }

        void ProcessResetCellsList()
        {
            // Wait until list has filled up a bit.
            if (resetCellsList.Count < 10)
                return;

            // Skip processing list at this time if GC thread is accessing resetCellsList.
            // It is fine to process the list at a later SetItem() call. No need to lock the GC thread.
            if (!Monitor.TryEnter(this))
                return;

            Dictionary<RowColumnIndex, bool> list = resetCellsList;

            // Immediately swap out resetCellsList in case GC thread finalizes a
            // style identity and try to modify the list while we process this current list below.
            resetCellsList = new Dictionary<RowColumnIndex, bool>();

            Monitor.Exit(this);

            // We now work on the old resetCellsList. No other object has access to this list
            // anymore at this point.
            foreach (RowColumnIndex cell in list.Keys)
            {
                ResetItemHelper(cell);
            }

        }


        #region Insert and Remove Rows
        /// <summary>
        /// Inserts the given number of rows.
        /// </summary>
        /// <param name="insertAtRowIndex">The insert row index.</param>
        /// <param name="count">Number of rows to insert.</param>
        /// <param name="moveCells">The move cells.</param>
        public void InsertRows(int insertAtRowIndex, int count, IGridVolatileData imoveCells)
        {
            GridRowsCache d = data;
            data = new GridRowsCache();

            foreach (KeyValuePair<int, GridCellsInRowCache> rowEntry in d)
            {
                int rowIndex = rowEntry.Key;
                if (rowIndex >= insertAtRowIndex)
                {
                    rowIndex = rowIndex + count;
                    UpdateCellRowColumnIndex(rowEntry, rowIndex);
                }
                data[rowIndex] = rowEntry.Value;
            }

            GridVolatileData moveCells = imoveCells as GridVolatileData;
            if (moveCells != null)
            {
                foreach (KeyValuePair<int, GridCellsInRowCache> rowEntry in moveCells.data)
                {
                    int rowIndex = rowEntry.Key + insertAtRowIndex;
                    UpdateCellRowColumnIndex(rowEntry, rowIndex);
                    data[rowIndex] = rowEntry.Value;
                }
            }
        }


        private static KeyValuePair<int, GridCellsInRowCache> UpdateCellRowColumnIndex(KeyValuePair<int, GridCellsInRowCache> rowEntry, int rowIndex)
        {
            foreach (KeyValuePair<int, WeakReference> cellEntry in rowEntry.Value.Cells)
            {
                UpdateCellRowColumnIndex(rowIndex, cellEntry.Key, cellEntry.Value);
            }
            return rowEntry;
        }

        private static void UpdateCellRowColumnIndex(int rowIndex, int columnIndex, WeakReference w)
        {
            GC.KeepAlive(w.Target);
            GridStyleInfo style = (GridStyleInfo)w.Target;
            if (style != null)
            {
                RowColumnIndex cell = new RowColumnIndex(rowIndex, columnIndex);
                style.CellIdentity.UpdateCellRowColumnIndex(cell);
                //style.ClearCache();
            }
        }

        /// <summary>
        /// Removes the specified rows.
        /// </summary>
        /// <param name="removeAtRowIndex">The remove row index.</param>
        /// <param name="count">Number of rows to remove.</param>
        /// <param name="moveCells">The move cells.</param>
        public void RemoveRows(int removeAtRowIndex, int count, IGridVolatileData imoveCells)
        {
            GridRowsCache d = data;
            data = new GridRowsCache();

            foreach (KeyValuePair<int, GridCellsInRowCache> rowEntry in d)
            {
                int rowIndex = rowEntry.Key;
                if (rowIndex >= removeAtRowIndex)
                {
                    if (rowIndex >= removeAtRowIndex + count)
                    {
                        rowIndex = rowIndex - count;
                        UpdateCellRowColumnIndex(rowEntry, rowIndex);
                    }
                    else
                    {
                        GridVolatileData moveCells = imoveCells as GridVolatileData;
                        if (moveCells != null)
                        {
                            rowIndex = rowIndex - removeAtRowIndex;
                            moveCells.data[rowIndex] = rowEntry.Value;
                        }
                        else // Dispose removed styles
                        {
                            foreach (var item in rowEntry.Value.Cells)
                            {
                                item.Value.Target = null;
                            }
                            rowEntry.Value.Cells.Clear();
                            rowEntry.Value.Cells = null;
                        }
                        continue;
                    }
                }
                data[rowIndex] = rowEntry.Value;
            }
            // TODO: Dispose removed styles?
        }
        #endregion

        #region Insert and Remove Columns

        /// <summary>
        /// Inserts the given no. of columns into the cell styles dictionary.
        /// </summary>
        /// <param name="insertAtColumnIndex">The column index to insert.</param>
        /// <param name="count">No. of columns to be inserted.</param>
        /// <param name="moveCells">The move cells.</param>
        public void InsertColumns(int insertAtColumnIndex, int count, IGridVolatileData imoveCells)
        {
            foreach (KeyValuePair<int, GridCellsInRowCache> rowEntry in data)
            {
                GridCellsInRowCache moveCellsInRow = null;
                GridVolatileData moveCells = imoveCells as GridVolatileData;
                if (moveCells != null)
                    moveCells.data.TryGetValue(rowEntry.Key, out moveCellsInRow);
                InsertColumnsInRow(rowEntry, insertAtColumnIndex, count, moveCellsInRow);
            }
        }

        private void InsertColumnsInRow(KeyValuePair<int, GridCellsInRowCache> rowEntry, int insertAtColumnIndex, int count, GridCellsInRowCache moveCellsInRow)
        {
            Dictionary<int, WeakReference> d = rowEntry.Value.Cells;
            rowEntry.Value.Cells = new Dictionary<int, WeakReference>();
            int rowIndex = rowEntry.Key;

            foreach (KeyValuePair<int, WeakReference> cellEntry in d)
            {
                int columnIndex = cellEntry.Key;
                if (columnIndex >= insertAtColumnIndex)
                {
                    columnIndex = columnIndex + count;
                    UpdateCellRowColumnIndex(rowIndex, columnIndex, cellEntry.Value);
                }
                rowEntry.Value.Cells[columnIndex] = cellEntry.Value;
            }

            if (moveCellsInRow != null)
            {
                foreach (KeyValuePair<int, WeakReference> cellEntry in moveCellsInRow.Cells)
                {
                    int columnIndex = cellEntry.Key + insertAtColumnIndex;
                    UpdateCellRowColumnIndex(rowIndex, columnIndex, cellEntry.Value);
                    rowEntry.Value.Cells[columnIndex] = cellEntry.Value;
                }
            }
        }

        /// <summary>
        /// Removes the specified columns from the cell styles dictionary.
        /// </summary>
        /// <param name="removeAtColumnIndex">Remove column index.</param>
        /// <param name="count">No. of columns to be removed.</param>
        /// <param name="moveCells">The move cells.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count, IGridVolatileData imoveCells)
        {
            foreach (KeyValuePair<int, GridCellsInRowCache> rowEntry in data)
            {
                GridCellsInRowCache moveCellsInRow = null;
                GridVolatileData moveCells = imoveCells as GridVolatileData;
                if (moveCells != null)
                {
                    moveCellsInRow = new GridCellsInRowCache();
                    moveCells.data[rowEntry.Key] = moveCellsInRow;
                }
                RemoveColumnsInRow(rowEntry, removeAtColumnIndex, count, moveCellsInRow);
            }
        }

        private void RemoveColumnsInRow(KeyValuePair<int, GridCellsInRowCache> rowEntry, int removeAtColumnIndex, int count, GridCellsInRowCache moveCellsInRow)
        {
            Dictionary<int, WeakReference> d = rowEntry.Value.Cells;
            rowEntry.Value.Cells = new Dictionary<int, WeakReference>();
            int rowIndex = rowEntry.Key;

            foreach (KeyValuePair<int, WeakReference> cellEntry in d)
            {
                int columnIndex = cellEntry.Key;
                if (columnIndex >= removeAtColumnIndex)
                {
                    if (columnIndex >= removeAtColumnIndex + count)
                    {
                        columnIndex = columnIndex - count;
                        UpdateCellRowColumnIndex(rowIndex, columnIndex, cellEntry.Value);
                    }
                    else
                    {
                        if (moveCellsInRow != null)
                            moveCellsInRow.Cells[columnIndex - removeAtColumnIndex] = cellEntry.Value;
                        else //Dispose removed styles
                        {
                            cellEntry.Value.Target = null;
                        }
                        continue;
                    }
                }
                rowEntry.Value.Cells[columnIndex] = cellEntry.Value;
            }
        }
        #endregion

    }

    /// <summary>
    /// VolatileData allocates its own style objects and maintains a weak reference to them.
    /// Once the style object is finalized, the weak reference is removed from the hashtable.
    /// </summary>
    public class GridVolatileCellStyles : IDisposable
    {
        IGridVolatileCellStylesHost host;
        IGridVolatileData data;

        Dictionary<RowColumnIndex, GridStyleInfo> queryCellStyleTable;
        internal Dictionary<object, IDictionary> visibleRowIndexes = new Dictionary<object, IDictionary>();
        internal Dictionary<object, IDictionary> visibleColumnIndexes = new Dictionary<object, IDictionary>();

        /// <summary>
        /// Initializes a new <see cref="GridVolatileCellStyles"/>.
        /// </summary>
        /// <param name="host">The <see cref="IGridVolatileCellStylesHost"/> object.</param>
        public GridVolatileCellStyles(IGridVolatileCellStylesHost host)
        {
            this.host = host;
            queryCellStyleTable = new Dictionary<RowColumnIndex, GridStyleInfo>();
            data = CreateVolatileData();
        }

        public GridVolatileCellStyles(IGridVolatileCellStylesHost host, IGridVolatileData volatileData)
        {
            this.host = host;
            queryCellStyleTable = new Dictionary<RowColumnIndex, GridStyleInfo>();
            data = volatileData;
        }

        IGridVolatileData CreateVolatileData()
        {
            GridVolatileData d = new GridVolatileData(this);
            d.isVisibleCell = new Func<RowColumnIndex, bool>(IsVisibleCell);
            return d;
        }

        /// <summary>
        /// Retrieves the host that handles the volatile cell styles.
        /// </summary>
        public IGridVolatileCellStylesHost Host
        {
            get { return host; }
        }

        /// <summary>
        /// Gets the cached style information for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinate.</param>
        /// <returns>The cached GridStyleInfo; NULL if no cache entry was found.</returns>
        public GridStyleInfo GetItem(RowColumnIndex cell)
        {
            return data.GetItem(cell);
        }

        /// <summary>
        /// Saves style information to be cached.
        /// </summary>
        /// <param name="cell">The cell coordinate.</param>
        /// <param name="style">The GridStyleInfo to be cached.</param>
        public void SetItem(RowColumnIndex cell, GridStyleInfo style)
        {
            data.SetItem(cell, style);
        }

        /// <summary>
        /// Resets cache for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinates.</param>
        public void ResetItem(RowColumnIndex cell)
        {
            data.Clear(cell);
        }

        private bool IsVisibleCell(RowColumnIndex cell)
        {
            foreach (KeyValuePair<object, IDictionary> entry in visibleRowIndexes)
            {
                if (entry.Value.Contains(cell.RowIndex) &&
                    visibleColumnIndexes[entry.Key].Contains(cell.ColumnIndex))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Resets cache for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinates.</param>
        public void Clear(RowColumnIndex cell)
        {
            ResetItem(cell);
        }

        /// <summary>
        /// Empty the cache.
        /// </summary>
        public void Clear()
        {
            data.Clear();
        }

        /// <summary>
        /// Removes the given cell span from the cache.
        /// </summary>
        /// <param name="cellSpan">Cell spanned range.</param>
        public void Clear(CellSpanInfoBase cellSpan)
        {
            data.Clear(cellSpan);
        }

        #region Style getter/setter with QueryCellInfo

        /// <summary>
        /// Gives access to a <see cref="GridStyleInfo"/> at a given row and column index. 
        /// </summary>
        public GridStyleInfo this[int rowIndex, int columnIndex]
        {
            get
            {
                return this[new RowColumnIndex(rowIndex, columnIndex)];
            }
            //set
            //{
            //    this[new RowColumnIndex(rowIndex, columnIndex)] = value;
            //}
        }

        /// <summary>
        /// Gives access to a <see cref="GridStyleInfo"/> at a given row and column index. 
        /// </summary>
        public GridStyleInfo this[RowColumnIndex cell]
        {
            get
            {
                // Prevent infinite recursion possibly triggered by QueryCellInfo
                // handler.
                if (queryCellStyleTable.ContainsKey(cell))
                    return queryCellStyleTable[cell];

                GridStyleInfo style;
                style = GetItem(cell);
                if (style == null)
                {
                    // Allocate style object and initialize its identity.
                    style = CreateStyle(cell);

                    // Prevent infinite recursion 
                    queryCellStyleTable.Add(cell, style);

                    style.BeginInit(); // Prevent OnStyleChanged being called.
                    OnQueryCellInfo(cell, style);
                    style.EndInit();

                    queryCellStyleTable.Remove(cell);

                    // Save weak reference to style object.
                    SetItem(cell, style);
                }

                return style;
            }
            //set
            //{
            //    OnSaveCellInfo(cell, value);
            //    if (value != null)
            //        value.Store.ResetChangedBits();
            //}
        }

        protected virtual GridStyleInfo CreateStyle(RowColumnIndex cell)
        {
            return new GridStyleInfo(new GridStyleInfoIdentity(this, cell));
        }

        protected virtual void OnQueryCellInfo(RowColumnIndex cell, GridStyleInfo style)
        {
            if (host != null)
                host.QueryCellInfo(cell, style);
        }

        internal void CommitStyle(RowColumnIndex cell, GridStyleInfo style, StyleInfoProperty sip)
        {
            if (host != null)
                host.CommitCellInfo(cell, style, sip);
        }

        internal GridCellModelBase LookupCellModel(string id)
        {
            return host.LookupCellModel(id);
        }

        /// <summary>
        /// Gets the base styles map.
        /// </summary>
        public GridBaseStylesMap BaseStylesMap
        {
            get { return host.BaseStylesMap; }
        }
        #endregion

        #region Insert and Remove Rows
        /// <summary>
        /// Inserts the given number of rows.
        /// </summary>
        /// <param name="insertAtRowIndex">The row index where rows have been inserted.</param>
        /// <param name="count">The number of rows that were inserted.</param>
        public void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRows(insertAtRowIndex, count, null);
        }

        /// <summary>
        /// Inserts the given number of rows.
        /// </summary>
        /// <param name="insertAtRowIndex">The insert row index.</param>
        /// <param name="count">Number of rows to insert.</param>
        /// <param name="moveCells">The move cells.</param>
        public void InsertRows(int insertAtRowIndex, int count, GridVolatileCellStyles moveCells)
        {
            data.InsertRows(insertAtRowIndex, count, moveCells != null ? moveCells.data : null);
        }

        /// <summary>
        /// Removes the specified rows.
        /// </summary>
        /// <param name="removeAtRowIndex">The remove row index.</param>
        /// <param name="count">Number of rows to remove.</param>
        public void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRows(removeAtRowIndex, count, null);
        }

        /// <summary>
        /// Removes the specified rows.
        /// </summary>
        /// <param name="removeAtRowIndex">The remove row index.</param>
        /// <param name="count">Number of rows to remove.</param>
        /// <param name="moveCells">The move cells.</param>
        public void RemoveRows(int removeAtRowIndex, int count, GridVolatileCellStyles moveCells)
        {
            data.RemoveRows(removeAtRowIndex, count, moveCells != null ? moveCells.data : null);
        }
        #endregion

        #region Insert and Remove Columns
        /// <summary>
        /// Inserts the given no. of columns into the cell styles dictionary.
        /// </summary>
        /// <param name="insertAtColumnIndex">The column index to insert.</param>
        /// <param name="count">No. of columns to be inserted.</param>
        public void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumns(insertAtColumnIndex, count, null);
        }

        /// <summary>
        /// Inserts the given no. of columns into the cell styles dictionary.
        /// </summary>
        /// <param name="insertAtColumnIndex">The column index to insert.</param>
        /// <param name="count">No. of columns to be inserted.</param>
        /// <param name="moveCells">The move cells.</param>
        public void InsertColumns(int insertAtColumnIndex, int count, GridVolatileCellStyles moveCells)
        {
            data.InsertColumns(insertAtColumnIndex, count, moveCells != null ? moveCells.data : null);
        }

        /// <summary>
        /// Removes the specified columns from the cell styles dictionary.
        /// </summary>
        /// <param name="removeAtColumnIndex">Remove column index.</param>
        /// <param name="count">No. of columns to be removed.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumns(removeAtColumnIndex, count, null);
        }

        /// <summary>
        /// Removes the specified columns from the cell styles dictionary.
        /// </summary>
        /// <param name="removeAtColumnIndex">Remove column index.</param>
        /// <param name="count">No. of columns to be removed.</param>
        /// <param name="moveCells">The move cells.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count, GridVolatileCellStyles moveCells)
        {
            data.RemoveColumns(removeAtColumnIndex, count, moveCells!=null?moveCells.data:null);
        }

        #endregion

        public void Dispose()
        {
            if (this.data != null)
            this.data.Clear();
            if (this.visibleColumnIndexes != null)
            this.visibleColumnIndexes.Clear();
            if (this.visibleRowIndexes != null)
            this.visibleRowIndexes.Clear();
            this.queryCellStyleTable.Clear();
        }

    }

    /// <summary>
    /// Provides the event argument values for <see cref="GridControlBase.QueryBaseStyles"/> event.
    /// </summary>
    public sealed class GridQueryBaseStylesEventArgs : SyncfusionHandledEventArgs
    {
        RowColumnIndex cell;
        GridStyleInfo style;
        List<GridStyleInfo> baseStyles = new List<GridStyleInfo>();

        /// <summary>
        /// Initializes a new <see cref="GridQueryBaseStylesEventArgs"/>.
        /// </summary>
        /// <param name="cell">Cell row and column index.</param>
        /// <param name="style">Cell style.</param>
        public GridQueryBaseStylesEventArgs(RowColumnIndex cell, GridStyleInfo style)
        {
            this.cell = cell;
            this.style = style;
        }

        /// <summary>
        /// The cell row column index.
        /// </summary>
        [TraceProperty(true)]
        public RowColumnIndex Cell
        {
            get
            {
                return cell;
            }
        }

        /// <summary>
        /// The cell style.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        /// Gets the base styles for the grid.
        /// </summary>
        public List<GridStyleInfo> BaseStyles
        {
            get { return baseStyles; }
        }

    }

    /// <summary>
    /// Represents a method that handles <see cref="GridControlBase.QueryBaseStyles"/> event.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="e">The <see cref="GridQueryBaseStylesEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryBaseStylesEventHandler(object sender, GridQueryBaseStylesEventArgs e);

    /// <summary>
    /// Provides the event argument values for <see cref="GridControlBase.QueryCellInfo"/> event.
    /// </summary>
    public sealed class GridQueryCellInfoEventArgs : SyncfusionHandledEventArgs
    {
        RowColumnIndex cell;
        GridStyleInfo style;

        /// <summary>
        /// Initializes a new <see cref="GridQueryCellInfoEventArgs"/>.
        /// </summary>
        /// <param name="cell">Cell row column index.</param>
        /// <param name="style">Cell style.</param>
        public GridQueryCellInfoEventArgs(RowColumnIndex cell, GridStyleInfo style)
        {
            this.cell = cell;
            this.style = style;
        }

        /// <summary>
        /// The cell row column index.
        /// </summary>
        [TraceProperty(true)]
        public RowColumnIndex Cell
        {
            get
            {
                return cell;
            }
        }

        /// <summary>
        /// Gets or sets the cell style.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
            set
            {
                style.ModifyStyle(value, StyleModifyType.Copy);
            }
        }
    }

#if !SILVERLIGHT
    public sealed class GridQueryContextMenuInfoEventArgs : SyncfusionHandledEventArgs
    {
        RowColumnIndex cell;
        GridStyleInfo style;
        bool cancel;

        public GridQueryContextMenuInfoEventArgs(RowColumnIndex cell, GridStyleInfo style, bool cancel)
        {
            this.cell = cell;
            this.style = style;
            this.Cancel = cancel;
        }

        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }

        public RowColumnIndex Cell
        {
            get
            {
                return cell;
            }
        }

        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
            set
            {
                style.ModifyStyle(value, StyleModifyType.Copy);
            }
        }
    }

    public delegate void GridQueryContextMenuInfoEventHandler(object sender, GridQueryContextMenuInfoEventArgs e);
#endif
    //public sealed class SaveCellInfoEventArgs : SyncfusionHandledEventArgs
    //{
    //    RowColumnIndex cell;
    //    GridStyleInfo style;

    //    public SaveCellInfoEventArgs(RowColumnIndex cell, GridStyleInfo style)
    //    {
    //        this.cell = cell;
    //        this.style = style;
    //    }

    //    [TraceProperty(true)]
    //    public RowColumnIndex Cell
    //    {
    //        get
    //        {
    //            return cell;
    //        }
    //    }

    //    [TraceProperty(true)]
    //    public GridStyleInfo Style
    //    {
    //        get
    //        {
    //            return style;
    //        }
    //        set
    //        {
    //            style.ModifyStyle(value, StyleModifyType.Copy);
    //        }
    //    }
    //}

    //public delegate void SaveCellInfoEventHandler(object sender, SaveCellInfoEventArgs e);

    /// <summary>
    /// Represents a method that handles <see cref="GridControlBase.QueryCellInfo"/> event.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="e">The <see cref="GridQueryCellInfoEventArgs"/> that contains the event data.</param>
    public delegate void GridQueryCellInfoEventHandler(object sender, GridQueryCellInfoEventArgs e);

    /// <summary>
    /// Provides the event argument values for <see cref="GridControlBase.CommitCellInfo"/> event.
    /// </summary>
    public sealed class GridCommitCellInfoEventArgs : SyncfusionHandledEventArgs
    {
        RowColumnIndex cell;
        GridStyleInfo style;
        StyleInfoProperty sip;

        /// <summary>
        /// Initializes a new <see cref="GridCommitCellInfoEventArgs"/>.
        /// </summary>
        /// <param name="cell">Cell row column index.</param>
        /// <param name="style">Cell style.</param>
        /// <param name="sip">The style info property.</param>
        public GridCommitCellInfoEventArgs(RowColumnIndex cell, GridStyleInfo style, StyleInfoProperty sip)
        {
            this.cell = cell;
            this.style = style;
            this.sip = sip;
        }

        /// <summary>
        /// Gets the cell row column index.
        /// </summary>
        [TraceProperty(true)]
        public RowColumnIndex Cell
        {
            get
            {
                return cell;
            }
        }

        /// <summary>
        /// Gets or sets the cell style.
        /// </summary>
        [TraceProperty(true)]
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
            set
            {
                style.ModifyStyle(value, StyleModifyType.Copy);
            }
        }

        /// <summary>
        /// Gets or sets the style info property.
        /// </summary>
        public StyleInfoProperty Sip
        {
            get { return sip; }
            set { sip = value; }
        }

    }

    /// <summary>
    /// Represents a method that handles <see cref="GridControlBase.CommitCellInfo"/> event.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="e">The <see cref="GridCommitCellInfoEventArgs"/> that contains the event data.</param>
    public delegate void GridCommitCellInfoEventHandler(object sender, GridCommitCellInfoEventArgs e);

}
