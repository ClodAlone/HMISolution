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

// DefaultCellStyle, 
// DefaultColumnHeaderStyle, DefaultColumnFooterStyle, 
// DefaultRowHeaderStyle, DefaultRowFooterStyle
// ColumnStyle[], ColumnHeaderStyle[], ColumnBodyStyle[], ColumnFooterStyle[]
// RowStyle[], RowHeaderStyle[], RowBodyStyle[], RowFooterStyle[]

// RemoveRows, MoveRows, DeleteRows etc. events will simply empty the cache.

// Clear will remove all weak references. Although the referenced style will 
// not necessarily be finalized immediately, they won't be accessed through VolatileData 
// anymore. VolatileData will return a new style object instead.

#if !WinRT
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.ComponentModel;
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Styles;
using Windows.UI.Xaml;
using System.Threading;
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface IGridVolatileCellStylesHost
    {
        void QueryCellInfo(RowColumnIndex cell, GridStyleInfo style);
        void CommitCellInfo(RowColumnIndex cell, GridStyleInfo style, StyleInfoProperty sip);
        IStyleInfo[] QueryBaseStyles(RowColumnIndex cell, GridStyleInfo style);
        GridCellModelBase LookupCellModel(string id);
        GridBaseStylesMap BaseStylesMap { get; }
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellsInRowCache
    {
        public Dictionary<int, WeakReference> Cells = new Dictionary<int, WeakReference>();
    }
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridRowsCache : Dictionary<int, GridCellsInRowCache>
    {
    }

    /// <summary>
    /// VolatileData allocates its own style objects and maintains a weak reference to them.
    /// Once the style object is finalized, the weak reference is removed from the hashtable.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridVolatileCellStyles : IDisposable
    {
        IGridVolatileCellStylesHost host;

        GridRowsCache data = new GridRowsCache();
        int count = 0;
#if !WinRT
        System.Threading.Thread guiThread;
#endif
        Dictionary<RowColumnIndex, bool> resetCellsList;
        Dictionary<RowColumnIndex, GridStyleInfo> queryCellStyleTable;
        internal Dictionary<object, IDictionary> visibleRowIndexes = new Dictionary<object, IDictionary>();
        internal Dictionary<object, IDictionary> visibleColumnIndexes = new Dictionary<object, IDictionary>();

        public GridVolatileCellStyles(IGridVolatileCellStylesHost host)
        {
            this.host = host;
            resetCellsList = new Dictionary<RowColumnIndex, bool>();
#if !WinRT
            guiThread = Thread.CurrentThread;
#endif
            queryCellStyleTable = new Dictionary<RowColumnIndex, GridStyleInfo>();
        }

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

        internal int forceGcCollect = int.MaxValue;

#if DEB
		int maxDataCount = 100;
#endif

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

#if DEB
				if (data.Count > maxDataCount)
				{
					Console.WriteLine("VolatileData HashTable: " + data.Count.ToString());
					maxDataCount = data.Count + data.Count/10; // let's just show a Trace statement for larger increases ...
				}
#endif

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
                    count++;
                }
                else
                {
                    obj.Target = style;
                }
            }
            catch (InvalidOperationException)
            {
                // should be InvalidOperation_HashInsertFailed=Hashtable insert failed.  Load factor too high.
                //TraceUtil.TraceExceptionCatched(ex);
                //Trace.WriteLine("Detected too many items in volatile date hashtable. Reducing VolatileData.forceGcCollect to a value below " + data.Count.ToString());
                //TraceUtil.TraceCurrentMethodInfo("Hashtable count: ", count);
                forceGcCollect = Math.Max(1000, count - 1000);
                GC.Collect();
                //TraceUtil.TraceCurrentMethodInfo("Hashtable count after GC.Collect: ", count);
                SetItem(cell, style);
            }
        }

        /// <summary>
        /// Resets cache for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinates.</param>
        public void ResetItem(RowColumnIndex cell)
        {
#if !WinRT
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
#endif
            {
                ResetItemHelper(cell);
            }
        }

        private bool ResetItemHelper(RowColumnIndex cell)
        {
            GridCellsInRowCache cellsInRow;
            if (data.TryGetValue(cell.RowIndex, out cellsInRow))
            {
                WeakReference obj;
                if (cellsInRow.Cells.TryGetValue(cell.ColumnIndex, out obj))
                {
                    if (obj.Target != null)
                        obj.Target = null;

                    if (!IsVisibleCell(cell))
                    {
                        count--;
                        if (cellsInRow.Cells.Count > 1)
                            cellsInRow.Cells.Remove(cell.ColumnIndex);
                        else
                            data.Remove(cell.RowIndex);
                    }
                }
                return true;
            }
            return false;
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
            resetCellsList = new Dictionary<RowColumnIndex, bool>();
        }

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
                    for (int columnIndex = cellSpan.Left; columnIndex <= cellSpan.Right; columnIndex++)
                    {
                        RowColumnIndex cell = new RowColumnIndex(rowIndex, columnIndex);
                        ResetItemHelper(cell);
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

        #region Insert and Remove Rows
        public void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRows(insertAtRowIndex, count, null);
        }

        public void InsertRows(int insertAtRowIndex, int count, GridVolatileCellStyles moveCells)
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

        public void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRows(removeAtRowIndex, count, null);
        }

        public void RemoveRows(int removeAtRowIndex, int count, GridVolatileCellStyles moveCells)
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
                        if (moveCells != null)
                        {
                            rowIndex = rowIndex - removeAtRowIndex;
                            moveCells.data[rowIndex] = rowEntry.Value;
                        }
                        //else 
                        // TODO: Dispose removed styles?
                        continue;
                    }
                }
                data[rowIndex] = rowEntry.Value;
            }
            // TODO: Dispose removed styles?
        }
        #endregion

        #region Insert and Remove Columns
        public void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumns(insertAtColumnIndex, count, null);
        }

        public void InsertColumns(int insertAtColumnIndex, int count, GridVolatileCellStyles moveCells)
        {
            foreach (KeyValuePair<int, GridCellsInRowCache> rowEntry in data)
            {
                GridCellsInRowCache moveCellsInRow = null;
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

        public void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumns(removeAtColumnIndex, count, null);
        }

        public void RemoveColumns(int removeAtColumnIndex, int count, GridVolatileCellStyles moveCells)
        {
            foreach (KeyValuePair<int, GridCellsInRowCache> rowEntry in data)
            {
                GridCellsInRowCache moveCellsInRow = null;
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
                        //else 
                        // TODO: Dispose removed styles?
                        continue;
                    }
                }
                rowEntry.Value.Cells[columnIndex] = cellEntry.Value;
            }
        }
        #endregion

        public void Dispose()
        {
            host = null;
            if (data != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (data.ContainsKey(i) && data[i] != null && data[i].Cells != null)
                    {
                        data[i].Cells.Clear();
                        data[i].Cells = null;
                        data[i] = null;
                    }
                }
            }
            data.Clear();
            data = null;
            resetCellsList.Clear();
            queryCellStyleTable.Clear();
            queryCellStyleTable = null;
            visibleRowIndexes.Clear();
            visibleRowIndexes = null;
            visibleColumnIndexes.Clear();
            visibleColumnIndexes = null;
        }
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridQueryBaseStylesEventArgs : SyncfusionHandledEventArgs
    {
        RowColumnIndex cell;
        GridStyleInfo style;
        List<GridStyleInfo> baseStyles = new List<GridStyleInfo>();

        public GridQueryBaseStylesEventArgs(RowColumnIndex cell, GridStyleInfo style)
        {
            this.cell = cell;
            this.style = style;
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
        }

        public List<GridStyleInfo> BaseStyles
        {
            get { return baseStyles; }
        }

    }

    public delegate void GridQueryBaseStylesEventHandler(object sender, GridQueryBaseStylesEventArgs e);

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridQueryCellInfoEventArgs : SyncfusionHandledEventArgs
    {
        RowColumnIndex cell;
        GridStyleInfo style;

        public GridQueryCellInfoEventArgs(RowColumnIndex cell, GridStyleInfo style)
        {
            this.cell = cell;
            this.style = style;
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

    //public sealed class SaveCellInfoEventArgs : SyncfusionHandledEventArgs
    //{
    //    RowColumnIndex cell;
    //    GridStyleInfo style;

    //    public SaveCellInfoEventArgs(RowColumnIndex cell, GridStyleInfo style)
    //    {
    //        this.cell = cell;
    //        this.style = style;
    //    }

    //    public RowColumnIndex Cell
    //    {
    //        get
    //        {
    //            return cell;
    //        }
    //    }

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

    public delegate void GridQueryCellInfoEventHandler(object sender, GridQueryCellInfoEventArgs e);

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridCommitCellInfoEventArgs : SyncfusionHandledEventArgs
    {
        RowColumnIndex cell;
        GridStyleInfo style;
        StyleInfoProperty sip;

        public GridCommitCellInfoEventArgs(RowColumnIndex cell, GridStyleInfo style, StyleInfoProperty sip)
        {
            this.cell = cell;
            this.style = style;
            this.sip = sip;
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

        public StyleInfoProperty Sip
        {
            get { return sip; }
            set { sip = value; }
        }

    }

    public delegate void GridCommitCellInfoEventHandler(object sender, GridCommitCellInfoEventArgs e);

}