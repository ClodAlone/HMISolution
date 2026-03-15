#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Collections;
using System;

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// An interface for callback functions of a <see cref="RowColumnIndexValueDictionary{T}"/>.
    /// You should call <see cref="RowColumnIndexValueDictionary{T}.SetCallback"/> to register
    /// your object that implements this interface with the collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRowColumnIndexValueDictionaryCallbacks<T>
    {
        /// <summary>
        /// Called from <see cref="RowColumnIndexValueDictionary{T}"/> when a cell
        /// is moved after a InsertRows, RemoveRows, InsertColumns or RemoveColumns
        /// method call.
        /// </summary>
        /// <param name="cellRowColumnIndex"></param>
        /// <param name="value"></param>
        void OnMovedCell(RowColumnIndex cellRowColumnIndex, T value);


        /// <summary>
        /// Called when from <see cref="RowColumnIndexValueDictionary{T}"/> when a cell
        /// is removed either by a RemoveRows, RemoveColumns, Remove or RemoveAll call.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="value">The value.</param>
        void OnRemoveCell(RowColumnIndex cellRowColumnIndex, T value);
    }

    /// <summary>
    /// An object with a Cells dictionary. This class is used by
    /// <see cref="RowColumnIndexValueDictionary{T}"/> for rows
    /// inside that dictionary where each row has cells.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerValueCellsDictionary<T>
    {
        Dictionary<int, T> cells = new Dictionary<int, T>();

        /// <summary>
        /// Gets or sets the dictionary of cells.
        /// </summary>
        /// <value>The cells.</value>
        public Dictionary<int, T> Cells
        {
            get { return cells; }
            set { cells = value; }
        }
    }

    /// <summary>
    /// A generic class that emulates a subset of a Dictionary{T}.
    /// Internally rows are held in a dictionary and each row has a dictionary of cells.
    /// The class also provides support for adding, removing and moving rows and 
    /// columns.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RowColumnIndexValueDictionary<T> : IEnumerable<KeyValuePair<RowColumnIndex, T>>
    {
        Dictionary<int, IntegerValueCellsDictionary<T>> data = new Dictionary<int, IntegerValueCellsDictionary<T>>();
        int count = 0;
        IRowColumnIndexValueDictionaryCallbacks<T> callback;

        /// <summary>
        /// Let derived class specify whether a OnMovedCell needs to be called
        /// for each cell that belongs to a row whose index position was changed after
        /// a move, insert or remove operation. If set (default is true) calls to InsertRows
        /// and RemoveRows will notify each cell in rows above the modified range of row that 
        /// its row index was changed. Set this false when you do not need this information for each cell 
        /// element to avoid unnecessary looping through rows.
        /// <para/>
        /// CellDrawingVisualsDictionary is a derived class which sets this false. This will speed
        /// up insert and remove operations for drawing visuals. Only visuals that were removed
        /// need to be looped through then.
        /// <para/>
        /// This property does not affect the OnRemoveCell callback. For each row that is removed
        /// the class will still notify each cell that it was removed.
        /// </summary>
        public bool NotifyMovedRow { get; set; }
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="RowColumnIndexValueDictionary&lt;T&gt;"/> class.
        /// </summary>
        public RowColumnIndexValueDictionary()
        {
            NotifyMovedRow = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowColumnIndexValueDictionary&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="callback">The callback for moved and removed cell notifications.</param>
        public RowColumnIndexValueDictionary(IRowColumnIndexValueDictionaryCallbacks<T> callback)
            : this()
        {
            this.callback = callback;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public Dictionary<int, IntegerValueCellsDictionary<T>> Rows
        {
            get { return data; }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return count;
            }
        }

        #endregion

        #region Clear and RemoveAll

        /// <summary>
        /// Clears out the dictionary. There are no "OnRemoveCell" notfications triggered.
        /// </summary>
        public void Clear()
        {
            Rows.Clear();
        }

        /// <summary>
        /// Removes all entries from the dictionary and calls <see cref="IRowColumnIndexValueDictionaryCallbacks{T}.OnRemoveCell"/> 
        /// callback for each deleted cell.
        /// </summary>
        public virtual void RemoveAll()
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.RemoveAll")) 
            {
#endif
            if (callback != null)
            {
                foreach (KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry in Rows)
                {
                    foreach (KeyValuePair<int, T> cellEntry in rowEntry.Value.Cells)
                    {
                        callback.OnRemoveCell(new RowColumnIndex(rowEntry.Key, cellEntry.Key), cellEntry.Value);
                    }
                }
            }
            Clear();
#if MEASURETIME
            }
#endif
        }

        #endregion

        #region Dictionary-like API
        /// <summary>
        /// Determines whether a cell exists.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns>
        /// 	<c>true</c> if the cell exists; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(RowColumnIndex cell)
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.ContainsKey(RowColumnIndex)"))
            {
#endif
            IntegerValueCellsDictionary<T> cellsInRow;
            if (data.TryGetValue(cell.RowIndex, out cellsInRow))
            {
                return cellsInRow.Cells.ContainsKey(cell.ColumnIndex);
            }
            return false;
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Tries to get the value if the cell exists.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if cell exits; false otherwise.</returns>
        public bool TryGetValue(RowColumnIndex cell, out T value)
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.TryGetValue(RowColumnIndex, out value)"))
            {
#endif
            IntegerValueCellsDictionary<T> cellsInRow;
            if (data.TryGetValue(cell.RowIndex, out cellsInRow))
            {
                return cellsInRow.Cells.TryGetValue(cell.ColumnIndex, out value);
            }
            else
            {
                value = default(T);
                return false;
            }
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Gets or sets the value for the specified cell.
        /// </summary>
        /// <value></value>
        public T this[RowColumnIndex cell]
        {
            get
            {
                T value;
                TryGetValue(cell, out value);
                return value;
            }
            set
            {
#if MEASURETIME
                using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.SetItem(RowColumnIndex, value)"))
                {
#endif
                IntegerValueCellsDictionary<T> cellsInRow;
                if (!data.TryGetValue(cell.RowIndex, out cellsInRow))
                {
                    cellsInRow = new IntegerValueCellsDictionary<T>();
                    data.Add(cell.RowIndex, cellsInRow);
                }

                if (!cellsInRow.Cells.ContainsKey(cell.ColumnIndex))
                    count++;

                cellsInRow.Cells[cell.ColumnIndex] = value;
            }
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Adds the cell and its value the dictionary. There is no "OnMoveCell" callback triggered.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="value"></param>
        public void Add(RowColumnIndex cell, T value)
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.Add(RowColumnIndex, value)"))
            {
#endif
            IntegerValueCellsDictionary<T> cellsInRow;
            if (!data.TryGetValue(cell.RowIndex, out cellsInRow))
            {
                cellsInRow = new IntegerValueCellsDictionary<T>();
                data.Add(cell.RowIndex, cellsInRow);
            }
            if (!(cellsInRow.Cells.ContainsKey(cell.ColumnIndex)))
                cellsInRow.Cells.Add(cell.ColumnIndex, value);
            count++;
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Removes the cell from the dictionary. There is no "OnRemoveCell" callback triggered.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool Clear(RowColumnIndex cell)
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.Clear(RowColumnIndex)"))
            {
#endif
            IntegerValueCellsDictionary<T> cellsInRow;
            if (data.TryGetValue(cell.RowIndex, out cellsInRow))
            {
                T value;
                if (cellsInRow.Cells.TryGetValue(cell.ColumnIndex, out value))
                {
                    count--;
                    if (cellsInRow.Cells.Count > 1)
                        cellsInRow.Cells.Remove(cell.ColumnIndex);
                    else
                        data.Remove(cell.RowIndex);
                }
                return true;
            }
            return false;
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Removes the cell from the dictionary and calls <see cref="IRowColumnIndexValueDictionaryCallbacks{T}.OnRemoveCell"/> 
        /// callback for the deleted cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool Remove(RowColumnIndex cell)
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.Remove(RowColumnIndex)"))
            {
#endif
            IntegerValueCellsDictionary<T> cellsInRow;
            if (data.TryGetValue(cell.RowIndex, out cellsInRow))
            {
                T value;
                if (cellsInRow.Cells.TryGetValue(cell.ColumnIndex, out value))
                {
                    count--;
                    if (cellsInRow.Cells.Count > 1)
                        cellsInRow.Cells.Remove(cell.ColumnIndex);
                    else
                        data.Remove(cell.RowIndex);
                    if (callback != null)
                        callback.OnRemoveCell(cell, value);
                }
                return true;
            }
            return false;
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Removes the specified cell span and calls <see cref="IRowColumnIndexValueDictionaryCallbacks{T}.OnRemoveCell"/> 
        /// callback for each deleted cell.
        /// </summary>
        /// <param name="cellSpan">The cell span.</param>
        public void Remove(CellSpanInfoBase cellSpan)
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.Remove(CellSpanInfoBase)"))
            {
#endif
            long l = (long) cellSpan.Height * cellSpan.Width;

            if (l < 0 || l > data.Count)
            {
                List<RowColumnIndex> toReset = new List<RowColumnIndex>();
                foreach (KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry in data)
                {
                    if (cellSpan.ContainsRow(rowEntry.Key))
                    {
                        foreach (KeyValuePair<int, T> cellEntry in rowEntry.Value.Cells)
                        {
                            if (cellSpan.ContainsColumn(cellEntry.Key))
                            {
                                RowColumnIndex cell = new RowColumnIndex(rowEntry.Key, cellEntry.Key);
                                toReset.Add(cell);
                            }
                        }
                    }
                }
                foreach (RowColumnIndex cell in toReset)
                    Remove(cell);
            }
            else
            {
                for (int rowIndex = cellSpan.Top; rowIndex <= cellSpan.Bottom; rowIndex++)
                {
                    for (int columnIndex = cellSpan.Left; columnIndex <= cellSpan.Right; columnIndex++)
                    {
                        RowColumnIndex cell = new RowColumnIndex(rowIndex, columnIndex);
                        Remove(cell);
                    }
                }
            }
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// A delegate with a method to be called back while iterating through a
        /// range of cells with the <see cref="Iterate"/> method.
        /// </summary>
        public delegate void RowColumnIndexValueDelegate(RowColumnIndex cell, T value);

        /// <summary>
        /// Iterates through the specified cell span and calls the specified method
        /// for each existing cell in the range.
        /// </summary>
        /// <param name="cellSpan">The cell span.</param>
        /// <param name="callback">The callback.</param>
        public void Iterate(CellSpanInfoBase cellSpan, RowColumnIndexValueDelegate callback)
        {
            long l = (long)cellSpan.Height * cellSpan.Width;

            if (l < 0 || l > data.Count)
            {
                List<RowColumnIndex> toReset = new List<RowColumnIndex>();
                foreach (KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry in data)
                {
                    if (cellSpan.ContainsRow(rowEntry.Key))
                    {
                        foreach (KeyValuePair<int, T> cellEntry in rowEntry.Value.Cells)
                        {
                            if (cellSpan.ContainsColumn(cellEntry.Key))
                            {
                                RowColumnIndex cell = new RowColumnIndex(rowEntry.Key, cellEntry.Key);
                                callback(cell, cellEntry.Value);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int rowIndex = cellSpan.Top; rowIndex <= cellSpan.Bottom; rowIndex++)
                {
                    for (int columnIndex = cellSpan.Left; columnIndex <= cellSpan.Right; columnIndex++)
                    {
                        RowColumnIndex cell = new RowColumnIndex(rowIndex, columnIndex);
                        T value;
                        if (TryGetValue(cell, out value))
                            callback(cell, value);
                    }
                }
            }
        }

        #endregion

        #region Insert and Remove Rows

        /// <summary>
        /// Inserts the rows and calls <see cref="IRowColumnIndexValueDictionaryCallbacks{T}.OnMovedCell"/> 
        /// callback for each moved cell. When you specify moveCells then these cells
        /// will be added beginning at the first insert row.
        /// </summary>
        /// <param name="insertAtRowIndex">Index of the insert at row.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCells">The move cells.</param>
        public void InsertRows(int insertAtRowIndex, int count, RowColumnIndexValueDictionary<T> moveCells)
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.InsertRemove"))
            {
#endif
            Dictionary<int, IntegerValueCellsDictionary<T>> d = data;
            data = new Dictionary<int, IntegerValueCellsDictionary<T>>();

            foreach (KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry in d)
            {
                int rowIndex = rowEntry.Key;
                if (rowIndex >= insertAtRowIndex)
                {
                    rowIndex = rowIndex + count;
                    if (NotifyMovedRow) OnMovedRow(rowEntry, rowIndex);
                }
                data[rowIndex] = rowEntry.Value;
            }

            if (moveCells != null)
            {
                foreach (KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry in moveCells.data)
                {
                    int rowIndex = rowEntry.Key + insertAtRowIndex;
                    if (NotifyMovedRow) OnMovedRow(rowEntry, rowIndex);
                    data[rowIndex] = rowEntry.Value;
                }
            }
#if MEASURETIME
            }
#endif
        }

        /// <summary>
        /// Removes the rows and calls <see cref="IRowColumnIndexValueDictionaryCallbacks{T}.OnRemoveCell"/> 
        /// callback for each removed cell. When you specify moveCells the callback will not be called,
        /// instead the cells will be added to the moveCells dictionary. A subsequent
        /// InsertRows call can then insert the rows at a different location.
        /// </summary>
        /// <param name="removeAtRowIndex">Index of the remove at row.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCells">The move cells.</param>
        public void RemoveRows(int removeAtRowIndex, int count, RowColumnIndexValueDictionary<T> moveCells)
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.InsertRemove"))
            {
#endif
            Dictionary<int, IntegerValueCellsDictionary<T>> d = data;
            data = new Dictionary<int, IntegerValueCellsDictionary<T>>();

            foreach (KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry in d)
            {
                int rowIndex = rowEntry.Key;
                if (rowIndex >= removeAtRowIndex)
                {
                    if (rowIndex >= removeAtRowIndex + count)
                    {
                        rowIndex = rowIndex - count;
                        if (NotifyMovedRow) OnMovedRow(rowEntry, rowIndex);
                    }
                    else
                    {
                        if (moveCells != null)
                        {
                            rowIndex = rowIndex - removeAtRowIndex;
                            moveCells.data[rowIndex] = rowEntry.Value;
                        }
                        else
                            OnRemovedRow(rowEntry);
                        continue;
                    }
                }
                data[rowIndex] = rowEntry.Value;
            }
#if MEASURETIME
            }
#endif
        }
        #endregion

        #region Insert and Remove Columns

        /// <summary>
        /// Inserts the columns and calls <see cref="IRowColumnIndexValueDictionaryCallbacks{T}.OnMovedCell"/> 
        /// callback for each moved cell. When you specify moveCells then these cells
        /// will be added beginning at the first insert column.
        /// </summary>
        /// <param name="insertAtColumnIndex">Index of the insert at column.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCells">The move cells.</param>
        public void InsertColumns(int insertAtColumnIndex, int count, RowColumnIndexValueDictionary<T> moveCells)
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.InsertRemove"))
            {
#endif
            foreach (KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry in data)
            {
                IntegerValueCellsDictionary<T> moveCellsInRow = null;
                if (moveCells != null)
                    moveCells.data.TryGetValue(rowEntry.Key, out moveCellsInRow);
                InsertColumnsInRow(rowEntry, insertAtColumnIndex, count, moveCellsInRow);
            }
#if MEASURETIME
            }
#endif
        }

        private void InsertColumnsInRow(KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry, int insertAtColumnIndex, int count, IntegerValueCellsDictionary<T> moveCellsInRow)
        {
            Dictionary<int, T> d = rowEntry.Value.Cells;
            rowEntry.Value.Cells = new Dictionary<int, T>();
            int rowIndex = rowEntry.Key;

            foreach (KeyValuePair<int, T> cellEntry in d)
            {
                int columnIndex = cellEntry.Key;
                if (columnIndex >= insertAtColumnIndex)
                {
                    columnIndex = columnIndex + count;
                    if (callback != null)
                        callback.OnMovedCell(new RowColumnIndex(rowIndex, columnIndex), cellEntry.Value);
                }
                rowEntry.Value.Cells[columnIndex] = cellEntry.Value;
            }

            if (moveCellsInRow != null)
            {
                foreach (KeyValuePair<int, T> cellEntry in moveCellsInRow.Cells)
                {
                    int columnIndex = cellEntry.Key + insertAtColumnIndex;
                    if (callback != null)
                        callback.OnMovedCell(new RowColumnIndex(rowIndex, columnIndex), cellEntry.Value);
                    rowEntry.Value.Cells[columnIndex] = cellEntry.Value;
                }
            }
        }

        /// <summary>
        /// Removes the columns and calls <see cref="IRowColumnIndexValueDictionaryCallbacks{T}.OnRemoveCell"/> 
        /// callback for each removed cell. When you specify moveCells the callback will not be called,
        /// instead the cells will be added to the moveCells dictionary. A subsequent
        /// InsertColumns call can then insert the columns at a different location.
        /// </summary>
        /// <param name="removeAtColumnIndex">Index of the remove at column.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCells">The move cells.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count, RowColumnIndexValueDictionary<T> moveCells)
        {
#if MEASURETIME
            using (MeasureTime.Measure("RowColumnIndexValueDictionary<T>.InsertRemove"))
            {
#endif
            foreach (KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry in data)
            {
                IntegerValueCellsDictionary<T> moveCellsInRow = null;
                if (moveCells != null)
                {
                    moveCellsInRow = new IntegerValueCellsDictionary<T>();
                    moveCells.data[rowEntry.Key] = moveCellsInRow;
                }
                RemoveColumnsInRow(rowEntry, removeAtColumnIndex, count, moveCellsInRow);
            }
#if MEASURETIME
            }
#endif
        }

        private void RemoveColumnsInRow(KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry, int removeAtColumnIndex, int count, IntegerValueCellsDictionary<T> moveCellsInRow)
        {
            Dictionary<int, T> d = rowEntry.Value.Cells;
            rowEntry.Value.Cells = new Dictionary<int, T>();
            int rowIndex = rowEntry.Key;

            foreach (KeyValuePair<int, T> cellEntry in d)
            {
                int columnIndex = cellEntry.Key;
                if (columnIndex >= removeAtColumnIndex)
                {
                    if (columnIndex >= removeAtColumnIndex + count)
                    {
                        columnIndex = columnIndex - count;
                        if (callback != null)
                            callback.OnMovedCell(new RowColumnIndex(rowIndex, columnIndex), cellEntry.Value);
                    }
                    else
                    {
                        if (moveCellsInRow != null)
                            moveCellsInRow.Cells[columnIndex - removeAtColumnIndex] = cellEntry.Value;
                        else
                        {
                            if (callback != null)
                                callback.OnRemoveCell(new RowColumnIndex(rowIndex, columnIndex), cellEntry.Value);
                        }
                        continue;
                    }
                }
                rowEntry.Value.Cells[columnIndex] = cellEntry.Value;
            }
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Sets the callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void SetCallback(IRowColumnIndexValueDictionaryCallbacks<T> callback)
        {
            this.callback = callback;
        }

        /// <summary>
        /// Called when a row was moved. The default implementation of this method
        /// loops through all cells in the row and calls <see cref="IRowColumnIndexValueDictionaryCallbacks{T}.OnMovedCell"/> 
        /// for each cell.
        /// </summary>
        /// <param name="rowEntry">The row entry.</param>
        /// <param name="rowIndex">Index of the row.</param>
        protected virtual void OnMovedRow(KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry, int rowIndex)
        {
            if (callback != null)
            {
                foreach (KeyValuePair<int, T> cellEntry in rowEntry.Value.Cells)
                {
                    callback.OnMovedCell(new RowColumnIndex(rowIndex, cellEntry.Key), cellEntry.Value);
                }
            }
        }

        /// <summary>
        /// Called when row was removed. The default implementation of this method
        /// loops through all cells in the row and calls <see cref="IRowColumnIndexValueDictionaryCallbacks{T}.OnRemoveCell"/> 
        /// for each cell.
        /// </summary>
        /// <param name="rowEntry">The row entry.</param>
        protected virtual void OnRemovedRow(KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry)
        {
            if (callback != null)
            {
                foreach (KeyValuePair<int, T> cellEntry in rowEntry.Value.Cells)
                {
                    callback.OnRemoveCell(new RowColumnIndex(rowEntry.Key, cellEntry.Key), cellEntry.Value);
                }
            }
        }
        #endregion

        #region IEnumerable<KeyValuePair<RowColumnIndex,T>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<RowColumnIndex, T>> GetEnumerator()
        {
            foreach (KeyValuePair<int, IntegerValueCellsDictionary<T>> rowEntry in data)
            {
                foreach (KeyValuePair<int, T> cellEntry in rowEntry.Value.Cells)
                {
                    RowColumnIndex cell = new RowColumnIndex(rowEntry.Key, cellEntry.Key);
                    yield return new KeyValuePair<RowColumnIndex, T>(cell, cellEntry.Value);
                }
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

}
