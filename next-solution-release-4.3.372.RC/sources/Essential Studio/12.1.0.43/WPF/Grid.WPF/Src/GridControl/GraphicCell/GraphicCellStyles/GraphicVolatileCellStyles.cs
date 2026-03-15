#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Styles;
using System.Threading;
using System.Windows.Interop;
using Syncfusion.Windows.Diagnostics;
using System.Diagnostics;
using System.Collections;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicVolatileCellStyles : IDisposable
    {
        IGraphicVolatileCellStylesHost host;
        IGraphicVolatileData data;
        Dictionary<int, GraphicStyleInfo> queryCellStyleTable;
        internal Dictionary<object, IDictionary> visibleCellIndexes = new Dictionary<object, IDictionary>();

        public GraphicVolatileCellStyles(IGraphicVolatileCellStylesHost host)
        {
            this.host = host;
            queryCellStyleTable = new Dictionary<int, GraphicStyleInfo>();
            data = CreateVolatileData();
        }

        IGraphicVolatileData CreateVolatileData()
        {
            GraphicVolatileData d = new GraphicVolatileData(this);
            d.isVisibleCell = new Func<int, bool>(IsVisibleCell);
            return d;
        }

        private bool IsVisibleCell(int cell)
        {
            foreach (KeyValuePair<object, IDictionary> entry in visibleCellIndexes)
            {
                if (entry.Value.Contains(cell))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves the host that handles the volatile cell styles.
        /// </summary>
        public IGraphicVolatileCellStylesHost Host
        {
            get { return host; }
        }

        /// <summary>
        /// Gets the cached style information for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinate.</param>
        /// <returns>The cached GridStyleInfo; NULL if no cache entry was found.</returns>
        public GraphicStyleInfo GetItem(int cell)
        {
            return data.GetItem(cell);
        }

        /// <summary>
        /// Saves style information to be cached.
        /// </summary>
        /// <param name="cell">The cell coordinate.</param>
        /// <param name="style">The GridStyleInfo to be cached.</param>
        public void SetItem(int index, GraphicStyleInfo style)
        {
            data.SetItem(index, style);
        }

        /// <summary>
        /// Resets cache for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinates.</param>
        public void ResetItem(int index)
        {
            data.Clear(index);
        }

        /// <summary>
        /// Resets cache for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinates.</param>
        public void Clear(int index)
        {
            ResetItem(index);
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

        /// <summary>
        /// Gives access to a <see cref="GridStyleInfo"/> at a given row and column index. 
        /// </summary>
        //public GraphicStyleInfo this[int rowIndex, int columnIndex]
        //{
        //    get
        //    {
        //        return this[new RowColumnIndex(rowIndex, columnIndex)];
        //    }
        //}

        /// <summary>
        /// Gives access to a <see cref="GridStyleInfo"/> at a given row and column index. 
        /// </summary>
        public GraphicStyleInfo this[int index]
        {
            get
            {
                // Prevent infinite recursion possibly triggered by QueryCellInfo
                // handler.
                if (queryCellStyleTable.ContainsKey(index))
                    return queryCellStyleTable[index];

                GraphicStyleInfo style;
                style = GetItem(index);
                if (style == null)
                {
                    // Allocate style object and initialize its identity.
                    style = CreateStyle(index);

                    // Prevent infinite recursion 
                    queryCellStyleTable.Add(index, style);

                    style.BeginInit(); // Prevent OnStyleChanged being called.
                    OnQueryCellInfo(index, style);
                    style.EndInit();

                    queryCellStyleTable.Remove(index);

                    // Save weak reference to style object.
                    SetItem(index, style);
                }

                return style;
            }
        }

        protected virtual GraphicStyleInfo CreateStyle(int index)
        {
            return new GraphicStyleInfo(new GraphicStyleInfoIdentity(this, index));
        }

        protected virtual void OnQueryCellInfo(int index, GraphicStyleInfo style)
        {
            if (host != null)
                host.QueryGraphicCellInfo(index, style);
        }

        internal void CommitStyle(int index, GraphicStyleInfo style, StyleInfoProperty sip)
        {
            if (host != null)
                host.CommitGraphicCellInfo(index, style, sip);
        }

        internal GraphicCellModelBase LookupCellModel(string id)
        {
            return host.LookupGraphicCellModel(id);
        }

        public void Dispose()
        {
            if (this.data != null)
                this.data.Clear();
            if (this.visibleCellIndexes != null)
                this.visibleCellIndexes.Clear();
            this.queryCellStyleTable.Clear();
            if (host != null)
                host = null;
        }
    }

    public interface IGraphicVolatileCellStylesHost
    {
        /// <summary>
        /// Occurs when the model queries for style information about a specific cell.
        /// </summary>
        void QueryGraphicCellInfo(int index, GraphicStyleInfo style);
        /// <summary>
        /// Occurs when the model is about to save style information about a specific cell.
        /// </summary>
        void CommitGraphicCellInfo(int index, GraphicStyleInfo style, StyleInfoProperty sip);
        /// <summary>
        /// Occurs when the model queries information about base styles at a specific cell.
        /// </summary>
        IStyleInfo[] QueryBaseGraphicStyles(int index, GraphicStyleInfo style);
        /// <summary>
        /// Returns a <see cref="GridCellModelBase"/> for the specified id / cell type name.
        /// </summary>
        /// <param name="id">Cell type name.</param>
        /// <returns>The <see cref="GridCellModelBase"/> for the given id.</returns>
        /// <remarks>
        /// Calls <see cref="IGridData.LookupCellModel"/>.
        /// </remarks>
        GraphicCellModelBase LookupGraphicCellModel(string id);
    }

    public interface IGraphicVolatileData
    {
        /// <summary>
        /// Gets the GridStyleInfo for a cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        GraphicStyleInfo GetItem(int index);

        /// <summary>
        /// Sets the GridStyleInfo for a cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="style"></param>
        void SetItem(int index, GraphicStyleInfo style);

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
        void Clear(int index);
    }

    public class GraphicVolatileData : IGraphicVolatileData
    {
        GridCellsInRowCache data = new GridCellsInRowCache();
        int count = 0;

        System.Threading.Thread guiThread;
        Dictionary<int, bool> resetCellsList;

        internal int forceGcCollect = int.MaxValue;
        GraphicVolatileCellStyles volatileCellStyles;
        internal Func<int, bool> isVisibleCell;

        public GraphicVolatileData(GraphicVolatileCellStyles volatileCellStyles)
        {
            resetCellsList = new Dictionary<int, bool>();
            guiThread = Thread.CurrentThread;
            this.volatileCellStyles = volatileCellStyles;
        }

        public GraphicStyleInfo GetItem(int cell)
        {
            WeakReference obj;
            if (data.Cells.TryGetValue(cell, out obj))
            {
                return (GraphicStyleInfo)obj.Target;
            }
            return null;
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

            Dictionary<int, bool> list = resetCellsList;

            // Immediately swap out resetCellsList in case GC thread finalizes a
            // style identity and try to modify the list while we process this current list below.
            resetCellsList = new Dictionary<int, bool>();

            Monitor.Exit(this);

            // We now work on the old resetCellsList. No other object has access to this list
            // anymore at this point.
            foreach (int index in list.Keys)
            {
                ResetItemHelper(index);
            }

        }

        private bool ResetItemHelper(int index)
        {
            WeakReference obj;
            if (data.Cells.TryGetValue(index, out obj))
            {
                obj.Target = null;
                data.Cells.Remove(index);
                count--;
                return true;
            }
            return false;
        }

        public bool IsVisibleCell(int cell)
        {
            return isVisibleCell(cell);
        }

        /// <summary>
        /// Saves style information to be cached.
        /// </summary>
        /// <param name="cell">The cell coordinate.</param>
        /// <param name="style">The GridStyleInfo to be cached.</param>
        /// <returns>True if a new entry was created in hashtable; false if entry was replaced.</returns>
        private bool SetItemInternal(int index, GraphicStyleInfo style)
        {
            WeakReference obj;
            if (!data.Cells.TryGetValue(index, out obj))
            {
                obj = new WeakReference(style);
                data.Cells.Add(index, obj);
                return true;
            }
            else
            {
                obj.Target = style;
                return false;
            }
        }


        public void SetItem(int cell, GraphicStyleInfo style)
        {
            try
            {
                if (count >= forceGcCollect)
                    GC.Collect();

                ProcessResetCellsList();

                if (SetItemInternal(cell, style))
                    count++;
            }
            catch (InvalidOperationException)
            {
                forceGcCollect = Math.Max(1000, count - 1000);
                GC.Collect();
                SetItem(cell, style);
            }
        }

        public void Clear()
        {
            data.Cells.Clear();
            resetCellsList = new Dictionary<int, bool>();
            count = 0;
        }

        public void Clear(CellSpanInfoBase cellSpan)
        {
            long l = (long)cellSpan.Height * cellSpan.Width;

            if (l < 0 || l > data.Cells.Count)
            {
                List<int> toReset = new List<int>();
                //ProcessResetCellsList();
                foreach (KeyValuePair<int, WeakReference> cellEntry in data.Cells)
                {
                    if (cellSpan.ContainsColumn(cellEntry.Key))
                    {
                        cellEntry.Value.Target = null;
                        if (!IsVisibleCell(cellEntry.Key))
                        {
                            toReset.Add(cellEntry.Key);
                        }
                    }
                }
                foreach (int cell in toReset)
                    ResetItemHelper(cell);
            }
            else
            {
                for (int rowIndex = cellSpan.Top; rowIndex <= cellSpan.Bottom; rowIndex++)
                {
                    WeakReference obj;
                    if (!data.Cells.TryGetValue(rowIndex, out obj))
                        continue;
                    ResetItemHelper(rowIndex);
                }
                for (int columnIndex = cellSpan.Left; columnIndex <= cellSpan.Right; columnIndex++)
                {
                    WeakReference obj;
                    if (!data.Cells.TryGetValue(columnIndex, out obj))
                        continue;
                    ResetItemHelper(columnIndex);
                }
            }

            ProcessResetCellsList();
        }

        /// <summary>
        /// Resets cache for a specific cell.
        /// </summary>
        /// <param name="cell">The cell coordinates.</param>
        public void ResetItem(int index)
        {
            if (Thread.CurrentThread != guiThread)
            {
                // In GC Thread - prevent ProcessResetCellsList race condition
                Monitor.Enter(this);
                try
                {
                    resetCellsList[index] = true;
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
            else
            {
                ResetItemHelper(index);
            }
        }


        public void Clear(int index)
        {
            ResetItem(index);
        }
    }
}
