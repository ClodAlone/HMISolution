//-------------------------------------------------------------------------------------------------
// <copyright file="GridNewVolatileData.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

using Syncfusion.Styles;
using Syncfusion.Diagnostics;

////using Syncfusion.Windows.Forms.Grid.Common;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the cache to save the style information for grid cells.
    /// </summary>
    public class GridCellsInRowCache
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridCellsInRowCache()
            : base()
        {
        }
        /// <summary>
        /// dictionary table for Cells
        /// </summary>
        public Dictionary<int, WeakReference> Cells = new Dictionary<int, WeakReference>();
    }

    /// <summary>
    /// Defines the cache to save the style information for grid rows.
    /// </summary>
    public class GridRowsCache : Dictionary<int, GridCellsInRowCache>
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridRowsCache()
            : base()
        {
        }
    }

    /// <summary>
    /// Defines the interface to maintain volatile data.
    /// </summary>
    public interface IGridVisibleCellLists
    {
        /// <summary>
        /// Used internally.
        /// </summary>
        void SetVisibleRowsList(object view, IDictionary list);
        /// <summary>
        /// Used internally.
        /// </summary>
        void SetVisibleColumnsList(object view, IDictionary list);
    }

    /// <summary>
    /// GridVolatileData provides GridStyleInfo objects on demand that are returned by the 
    /// grid model's indexer. GridVolatileData caches the objects and their identity information 
    /// holding a weak reference to them.
    /// </summary>
    /// <remarks>
    /// GridModel holds a GridData object. This object contains the plain data with style settings 
    /// specific to a cell. When GridVolatileData creates a style object, it asks the grid to 
    /// return the cell settings from GridData and then associate these settings with identity
    /// information. This allows access to style properties using inheritance from base styles. 
    /// Identity information also ensures that changes to the style object will be written back 
    /// correctly to GridData.
    /// </remarks>
    public class GridNewVolatileData : IGridVolatileData, IDisposable, IGridVisibleCellLists
    {
        private GridModel grid;

        GridRowsCache data = new GridRowsCache();
        int count = 0;

        System.Threading.Thread guiThread;
        Dictionary<GridCellPos, bool> resetCellsList;
        Dictionary<GridCellPos, GridStyleInfo> queryCellStyleTable;

        /// <summary>
        /// Saves row style information.
        /// </summary>
        /// <param name="view">Row identity information.</param>
        /// <param name="list">The row style settings to be saved.</param>
        public void SetVisibleRowsList(object view, IDictionary list)
        {
            visibleRowIndexes[view] = list;
        }

        /// <summary>
        /// Saves column style information.
        /// </summary>
        /// <param name="view">Column identity information.</param>
        /// <param name="list">The column style settings to be saved.</param>
        public void SetVisibleColumnsList(object view, IDictionary list)
        {
            visibleColumnIndexes[view] = list;
        }

        internal Dictionary<object, IDictionary> visibleRowIndexes = new Dictionary<object, IDictionary>();
        internal Dictionary<object, IDictionary> visibleColumnIndexes = new Dictionary<object, IDictionary>();

        private int rowCount;
        private int colCount;
        private bool hasRowCount;
        private bool hasColCount;

        /// <summary>
        /// Attaches the object to the GridModel.
        /// </summary>
        /// <param name="grid">The parent grid model that will access style information through this object.</param>
        public GridNewVolatileData(GridModel grid)
        {
            resetCellsList = new Dictionary<GridCellPos, bool>();
            guiThread = Thread.CurrentThread;
            queryCellStyleTable = new Dictionary<GridCellPos, GridStyleInfo>();
            this.grid = grid;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object and collection items.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
        /// <summary>
        /// Gets the base styles map.
        /// </summary>
        public GridBaseStylesMap BaseStylesMap
        {
            get { return grid.BaseStylesMap; }
        }

        /// <summary>
        /// Looks up a <see cref="GridCellModelBase"/> for a given cell type as specified with <see cref="GridStyleInfo.CellType"/>.
        /// </summary>
        /// <param name="id">Cell type name.</param>
        /// <returns>Cell model.</returns>
        public GridCellModelBase LookupCellModel(string id)
        {
            return grid.CellModels[id];
        }

        /// <summary>
        /// Gets a reference to the parent grid model.
        /// </summary>
        public GridModel Grid
        {
            get
            {
                return (GridModel)grid;
            }
        }

        // RemoveRows, MoveRows, DeleteRows etc. events will simply empty the cache.

        // Clear will remove all weak references. Although the referenced style will 
        // not necessarily be finalized immediately, they won't be accessed through GridVolatileData 
        // anymore. GridVolatileData will return a new style object instead.

        /// <summary>
        /// Gets or sets a cached value for row count.
        /// </summary>
        public int RowCount
        {
            get
            {
                return rowCount;
            }

            set
            {
                rowCount = value;
                hasRowCount = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a value for row count has been cached.
        /// </summary>
        public bool HasRowCount
        {
            get
            {
                return hasRowCount;
            }
        }

        /// <summary>
        /// Clears cached value for row count.
        /// </summary>
        public void ResetRowCount()
        {
            hasRowCount = false;
        }

        /// <summary>
        /// Gets or sets a cached value for column count.
        /// </summary>
        public int ColCount
        {
            get
            {
                return colCount;
            }

            set
            {
                colCount = value;
                hasColCount = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a value for column count has been cached.
        /// </summary>
        public bool HasColCount
        {
            get
            {
                return hasColCount;
            }
        }

        /// <summary>
        /// Clears cached value for column count.
        /// </summary>
        public void ResetColCount()
        {
            hasColCount = false;
        }

        /// <summary>
        /// Gets the cached style information for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinate.</param>
        /// <returns>The cached GridStyleInfo; NULL if no cache entry was found.</returns>
        public GridStyleInfo GetItem(GridCellPos cell)
        {
            GridCellsInRowCache cellsInRow;
            if (data.TryGetValue(cell.RowNumber, out cellsInRow))
            {
                WeakReference obj;
                if (cellsInRow.Cells.TryGetValue(cell.ColumnNumber, out obj))
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
        public void SetItem(GridCellPos cell, GridStyleInfo style)
        {
            try
            {
                if (count >= forceGcCollect)
                {
                    GC.Collect();
                }

                ProcessResetCellsList();
#if DEB
                if (data.Count > maxDataCount)
                {
                    Console.WriteLine("VolatileData HashTable: " + data.Count.ToString());
                    maxDataCount = data.Count + data.Count/10; // let's just show a Trace statement for larger increases ...
                }
#endif
                GridCellsInRowCache cellsInRow;
                if (!data.TryGetValue(cell.RowNumber, out cellsInRow))
                {
                    cellsInRow = new GridCellsInRowCache();
                    data.Add(cell.RowNumber, cellsInRow);
                }

                WeakReference obj;
                if (!cellsInRow.Cells.TryGetValue(cell.ColumnNumber, out obj))
                {
                    obj = new WeakReference(style);
                    cellsInRow.Cells.Add(cell.ColumnNumber, obj);
                    count++;
                }
                else
                {
                    obj.Target = style;
                }
            }
            catch (InvalidOperationException ex)
            {
                //// should be InvalidOperation_HashInsertFailed=Hashtable insert failed.  Load factor too high.
                TraceUtil.TraceExceptionCatched(ex);
                Trace.WriteLine("Detected too many items in volatile date hashtable. Reducing VolatileData.forceGcCollect to a value below " + data.Count.ToString());
                TraceUtil.TraceCurrentMethodInfo("Hashtable count: ", count);
                forceGcCollect = Math.Max(1000, count - 1000);
                GC.Collect();
                TraceUtil.TraceCurrentMethodInfo("Hashtable count after GC.Collect: ", count);
                SetItem(cell, style);
            }
        }

        /// <summary>
        /// Resets cache for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinates.</param>
        public void ResetItem(GridCellPos cell)
        {
            if (Thread.CurrentThread != guiThread)
            {
                // In GC Thread - prevent ProcessResetCellsList race condition
#if SyncfusionFramework4_0
                bool lockTaken=false;
                Monitor.Enter(this,ref lockTaken);
#else
                Monitor.Enter(this);
#endif
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

        private bool ResetItemHelper(GridCellPos cell)
        {
            GridCellsInRowCache cellsInRow;
            if (data.TryGetValue(cell.RowNumber, out cellsInRow))
            {
                WeakReference obj;
                if (cellsInRow.Cells.TryGetValue(cell.ColumnNumber, out obj))
                {
                    obj.Target = null;

                    if (!IsVisibleCell(cell))
                    {
                        count--;
                        if (cellsInRow.Cells.Count > 1)
                        {
                            cellsInRow.Cells.Remove(cell.ColumnNumber);
                        }
                        else
                        {
                            data.Remove(cell.RowNumber);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        private bool IsVisibleCell(GridCellPos cell)
        {
            foreach (KeyValuePair<object, IDictionary> entry in visibleRowIndexes)
            {
                if (entry.Value.Contains(cell.RowNumber) &&
                  visibleColumnIndexes[entry.Key].Contains(cell.ColumnNumber))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Resets cache for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinates.</param>
        public void Clear(GridCellPos cell)
        {
            ResetItem(cell);
        }

        /// <summary>
        /// Empty the cache.
        /// </summary>
        public void Clear()
        {
            data.Clear();
            resetCellsList = new Dictionary<GridCellPos, bool>();
            hasRowCount = false;
            hasColCount = false;
        }

        ////public void Clear(CellSpanInfoBase cellSpan)
        ////{
        ////    if (cellSpan.Height * cellSpan.Width > data.Count)
        ////    {
        ////        List<GridCellPos> toReset = new List<GridCellPos>();
        ////        ////ProcessResetCellsList();
        ////        foreach (KeyValuePair<int, GridCellsInRowCache> rowEntry in data)
        ////        {
        ////            if (cellSpan.ContainsRow(rowEntry.Key))
        ////            {
        ////                foreach (KeyValuePair<int, WeakReference> cellEntry in rowEntry.Value.Cells)
        ////                {
        ////                    if (cellSpan.ContainsColumn(cellEntry.Key))
        ////                    {
        ////                        cellEntry.Value.Target = null;
        ////                        GridCellPos cell = new GridCellPos(rowEntry.Key, cellEntry.Key);
        ////                        if (!IsVisibleCell(cell))
        ////                        {
        ////                            toReset.Add(cell);
        ////                        }
        ////                    }
        ////                }
        ////            }
        ////        }
        ////        foreach (GridCellPos cell in toReset)
        ////            ResetItemHelper(cell);
        ////    }
        ////    else
        ////    {
        ////        for (int rowIndex = cellSpan.Top; rowIndex <= cellSpan.Bottom; rowIndex++)
        ////        {
        ////            for (int columnIndex = cellSpan.Left; columnIndex <= cellSpan.Right; columnIndex++)
        ////            {
        ////                GridCellPos cell = new GridCellPos(rowIndex, columnIndex);
        ////                ResetItemHelper(cell);
        ////            }
        ////        }
        ////    }

        ////    ProcessResetCellsList();
        ////}

        void ProcessResetCellsList()
        {
            //// Wait until list has filled up a bit.
            if (resetCellsList.Count < 10)
            {
                return;
            }

            //// Skip processing list at this time if GC thread is accessing resetCellsList.
            //// It is fine to process the list at a later SetItem() call. No need to lock the GC thread.
#if SyncfusionFramework4_0
            bool lockTaken = false;
            Monitor.TryEnter(this, ref lockTaken);
            if (!lockTaken)
            {
                return;
            }
#else
            if (!Monitor.TryEnter(this))
            {
                return;
            }
#endif

            Dictionary<GridCellPos, bool> list = resetCellsList;

            //// Immediately swap out resetCellsList in case GC thread finalizes a
            //// style identity and try to modify the list while we process this current list below.
            resetCellsList = new Dictionary<GridCellPos, bool>();

            Monitor.Exit(this);

            //// We now work on the old resetCellsList. No other object has access to this list
            //// anymore at this point.
            foreach (GridCellPos cell in list.Keys)
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
                return this[new GridCellPos(rowIndex, columnIndex)];
            }

            set
            {
                this[new GridCellPos(rowIndex, columnIndex)] = value;
            }
        }
        /// <summary>
        /// get / set the grid style info got purticular cell position
        /// </summary>
        /// <param name="cell">Grid Cell </param>
        /// <returns></returns>
        public GridStyleInfo this[GridCellPos cell]
        {
            get
            {
                // Prevent infinite recursion possibly triggered by QueryCellInfo
                // handler.
                if (queryCellStyleTable.ContainsKey(cell))
                {
                    return queryCellStyleTable[cell];
                }

                GridStyleInfo style;
                style = GetItem(cell);
                if (style == null)
                {
                    // Allocate style object and initialize its identity.
                    style = CreateStyle(cell);

                    // Prevent infinite recursion 
                    queryCellStyleTable.Add(cell, style);

                    style.BeginInit(); // Prevent OnStyleChanged being called.
                    // Load data.
                    grid.GetCellInfo(cell.RowNumber, cell.ColumnNumber, style);
                    style.EndInit();

                    queryCellStyleTable.Remove(cell);

                    // Save weak reference to style object.
                    SetItem(cell, style);
                }

                return style;
            }

            set
            {
                grid.ChangeCells(GridRangeInfo.Cell(cell.RowNumber, cell.ColumnNumber), value, StyleModifyType.Changes);
                if (value != null)
                {
                    value.Store.ResetChangedBits();
                }
            }
        }

        private GridStyleInfo CreateStyle(GridCellPos cell)
        {
            return new GridStyleInfo(new GridStyleInfoIdentity(this, cell));
        }

        /// <summary>
        /// Gets an array that consists of table, row, and column base styles for the specified row and column index.
        /// </summary>
        /// <param name="name">Name of the base style.</param>
        /// <param name="level">The maximum number of levels to look at when walking referenced base styles. </param>
        /// <returns>An array of table, row and column base styles.</returns>
        public GridStyleInfo[] GetBaseStylesMapStyles(string name, out int level)
        {
            GridBaseStylesMap styleInfoMap = this.BaseStylesMap;
            return styleInfoMap.GetBaseStylesMapStyles(name, out level);
        }

        //bool inGetBaseStyles = false;

        /// <summary>
        /// Gets an array that consists of table, row, and column base styles for the specified row and column index.
        /// </summary>
        /// <param name="thisStyleInfo">Cell style information.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <returns>An array of table, row and column base styles.</returns>
        public GridStyleInfo[] GetBaseStyles(GridStyleInfo thisStyleInfo, int rowIndex, int colIndex)
        {
            GridStyleInfo[] cellStyles;
            int cellStyleCount = 0;

            // Row, Column, and Table style
            int headerRowCount = grid.Rows.HeaderCount;
            int headerColCount = grid.Cols.HeaderCount;
            if (colIndex > headerColCount && rowIndex > headerRowCount)
            {
                if (rowIndex == 0)
                {
                    cellStyles = new GridStyleInfo[] 
                        {
                            RowStyles[rowIndex],
                            TableStyle
                        };
                }
                else if (colIndex == 0)
                {
                    cellStyles = new GridStyleInfo[] 
                        {
                            ColStyles[colIndex],
                            TableStyle
                        };
                }
                else
                {
                    cellStyles = new GridStyleInfo[] 
                        {
                            RowStyles[rowIndex],
                            ColStyles[colIndex],
                            TableStyle
                        };
                }

                cellStyleCount = cellStyles.Length;
            }
            else if ((colIndex >= 0 && rowIndex < 0) || (rowIndex >= 0 && colIndex < 0))
            {
                cellStyles = new GridStyleInfo[] 
                    {
                        TableStyle
                    };
                cellStyleCount = cellStyles.Length;
            }
            else
            {  
                cellStyles = null;
            }

            // Base style.
            GridStyleInfo item = thisStyleInfo;
            if (thisStyleInfo == null)
            {
                item = this[rowIndex, colIndex];
            }

            string baseStyleName = item.HasBaseStyle ? item.BaseStyle : string.Empty;

            if (baseStyleName.Length == 0 && cellStyles != null)
            {
                foreach (GridStyleInfo si in cellStyles)
                {
                    if (si.HasBaseStyle)
                    {
                        baseStyleName = si.BaseStyle;
                        if (baseStyleName.Length > 0)
                        {
                            break;
                        }
                    }
                }
            }

            // Row or column header.
            if (baseStyleName.Length == 0)
            {
                if (rowIndex >= 0 && colIndex >= 0)
                {
                    if (rowIndex <= headerRowCount && colIndex <= headerColCount)
                    {
                        baseStyleName = "Header";
                    }
                    else if (rowIndex <= headerRowCount)
                    {
                        baseStyleName = "Column Header";
                    }
                    else if (colIndex <= headerColCount)
                    {
                        baseStyleName = "Row Header";
                    }
                }
            }

            // Load all parent base styles (including standard style).
            int level;
            GridStyleInfo[] infoMapStyles = GetBaseStylesMapStyles(baseStyleName, out level);

            // Combine the two arrays.
            GridStyleInfo[] baseStyles = new GridStyleInfo[cellStyleCount + level];
            if (cellStyles != null)
            {
                Array.Copy(cellStyles, 0, baseStyles, 0, cellStyleCount);
            }

            if (infoMapStyles != null)
            {
                Array.Copy(infoMapStyles, 0, baseStyles, cellStyleCount, level);
            }

            // Each GridStyleInfoIdentity will cache the baseStyles.
            return baseStyles;
        }

        // GridControlBaseCore.this[row, col] will forward its requests to GridVolatileData.this[].

        // GridVolatileData allocates its own style objects and maintains a weak reference to them.
        // Obsolete -> Once the style object is finalized, the weak reference is removed from the hashtable. <-

        // The user is !!!always!!! operating on "smart" copy of a style object. This style object 
        // knows about its base styles and other information (see GridStyleInfoIdentity). Changes in this object will result in 
        // SetStyleRange/StoreStyleRowCol to update changes to GridData. GridData can be dumb. It does 
        // not need to know about rowindex, colindex, base style etc. 

        // GridVolatileData allocates its own style objects and maintains a weak reference to them.
        // Once the style object is finalized, the weak reference is removed from the hashtable.

        /// <summary>
        /// Cached row styles within GridVolatileData.
        /// </summary>
        public class RowStylesIndexer
        {
            GridNewVolatileData dataParent;
            internal RowStylesIndexer(GridNewVolatileData data)
            {
                dataParent = data;
            }

            /// <summary>
            /// Gives access to a <see cref="GridStyleInfo"/> at a given row index. 
            /// </summary>
            public GridStyleInfo this[int rowIndex]
            {
                get
                {
                    return dataParent[rowIndex, -1];
                }

                set
                {
                    dataParent[rowIndex, -1] = value;
                }
            }
        }

        private RowStylesIndexer rowStyles = null;

        /// <summary>
        /// Gets cached row styles within GridVolatileData.
        /// </summary>
        public RowStylesIndexer RowStyles
        {
            get
            {
                if (rowStyles == null)
                {
                    rowStyles = new RowStylesIndexer(this);
                }

                return rowStyles;
            }
        }

        /// <summary>
        /// Cached column styles within GridVolatileData.
        /// </summary>
        public class ColStylesIndexer
        {
            GridNewVolatileData dataParent;
            internal ColStylesIndexer(GridNewVolatileData data)
            {
                dataParent = data;
            }

            /// <summary>
            /// Gives access to a <see cref="GridStyleInfo"/> at a given column index. 
            /// </summary>
            public GridStyleInfo this[int colIndex]
            {
                get
                {
                    return dataParent[-1, colIndex];
                }

                set
                {
                    dataParent[-1, colIndex] = value;
                }
            }
        }

        private ColStylesIndexer colStyles = null;

        /// <summary>
        /// Gets cached column styles within GridVolatileData
        /// </summary>
        public ColStylesIndexer ColStyles
        {
            get
            {
                if (colStyles == null)
                {
                    colStyles = new ColStylesIndexer(this);
                }

                return colStyles;
            }
        }

        /// <summary>
        /// Gets or sets access to a <see cref="GridStyleInfo"/> for the table style. 
        /// </summary>
        public GridStyleInfo TableStyle
        {
            get
            {
                return this[-1, -1];
            }

            set
            {
                this[-1, -1] = value;
            }
        }
    }
#endif
}