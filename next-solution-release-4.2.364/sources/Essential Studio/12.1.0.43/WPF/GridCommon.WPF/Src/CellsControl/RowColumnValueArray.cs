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
using System.Windows;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Text;
using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.Cells
{
    public interface IRowColumnIndexValueArrayCallbacks<T>
    {
        void OnMovedCell(RowColumnIndex cellRowColumnIndex, T value);
        void OnRemoveCell(RowColumnIndex cellRowColumnIndex, T value);
    }

    public class IntegerCellsArray<T> : IDisposable
    {
        public List<T> Cells = new List<T>();

        public void EnsureCellCount(int count)
        {
            if (Cells.Count < count)
                Cells.AddRange(new T[count - Cells.Count]);
        }
        public void Dispose()
        {
            if (Cells != null)
            {
                for (int i = 0; i < Cells.Count; i++)
                {
                    if (Cells[i] is StyleInfoStore)
                        if ((Cells[i] as StyleInfoStore) != null)
                            (Cells[i] as StyleInfoStore).Dispose();
                }
                Cells.Clear();
                Cells = null;
            }
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SerializableKeyValuePair<TKey, TValue>
    {
        private TKey key;
        private TValue value;

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public TKey Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        public TValue Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            if (this.Key != null)
            {
                builder.Append(this.Key.ToString());
            }
            builder.Append(", ");
            if (this.Value != null)
            {
                builder.Append(this.Value.ToString());
            }
            builder.Append(']');
            return builder.ToString();
        }
    }


    /// <summary>
    /// A generic class that emulates a subset of a Dictionary{T}.
    /// Internally rows are hold in a dictionary and each row has a dictionary of cells.
    /// The class also provides support for adding, removing and moving rows and 
    /// columns.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RowColumnIndexValueArray<T> : IEnumerable<SerializableKeyValuePair<RowColumnIndex, T>>, IDisposable
    {
        List<IntegerCellsArray<T>> data = new List<IntegerCellsArray<T>>();
        IRowColumnIndexValueArrayCallbacks<T> callback;

        #region ctor

        public RowColumnIndexValueArray()
        {
        }

        public RowColumnIndexValueArray(IRowColumnIndexValueArrayCallbacks<T> callback)
        {
            this.callback = callback;
        }

        #endregion

        #region Properties

        public List<IntegerCellsArray<T>> Rows
        {
            get { return data; }
        }

        #endregion

        #region Clear and RemoveAll

        /// <summary>
        /// Clears out the dictionary. There are no "OnMoved" callback triggered.
        /// </summary>
        public void Clear()
        {
            Rows.Clear();
        }

        /// <summary>
        /// Removes all entries from the dictionary and calls "OnRemoveCell" callback for each deleted cell.
        /// </summary>
        public virtual void RemoveAll()
        {
            if (callback != null)
            {
                for (int r = 0; r < Rows.Count; r++)
                {
                    IntegerCellsArray<T> row = Rows[r];
                    if (row != null)
                    {
                        for (int c = 0; c < row.Cells.Count; c++)
                        {
                            callback.OnRemoveCell(new RowColumnIndex(r, c), row.Cells[c]);
                        }
                    }
                }
            }
            Clear();
        }

        #endregion

        #region Insert and Remove Rows
        public void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRows(insertAtRowIndex, count, null);
        }

        public void InsertRows(int insertAtRowIndex, int count, RowColumnIndexValueArray<T> moveCells)
        {
            if (moveCells == null || moveCells.Rows.Count == 0)
            {
                if (insertAtRowIndex < Rows.Count)
                    Rows.InsertRange(insertAtRowIndex, new IntegerCellsArray<T>[count]);
            }
            else
            {
                EnsureRowCount(insertAtRowIndex);
                Rows.InsertRange(insertAtRowIndex, moveCells.Rows);
            }
        }

        public void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRows(removeAtRowIndex, count, null);
        }

        public void RemoveRows(int removeAtRowIndex, int count, RowColumnIndexValueArray<T> moveCells)
        {
            if (moveCells != null && removeAtRowIndex < Rows.Count)
            {
                IntegerCellsArray<T>[] moveRows = new IntegerCellsArray<T>[count];
                for (int n = 0; n < count && removeAtRowIndex + n < Rows.Count; n++)
                    moveRows[n] = Rows[removeAtRowIndex + n];
                moveCells.Rows.InsertRange(0, moveRows);
            }
            if (removeAtRowIndex < Rows.Count)
                Rows.RemoveRange(removeAtRowIndex, Math.Min(count, Rows.Count - removeAtRowIndex));
        }
        #endregion

        #region Insert and Remove Columns
        public void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumns(insertAtColumnIndex, count, null);
        }

        public void InsertColumns(int insertAtColumnIndex, int count, RowColumnIndexValueArray<T> moveCells)
        {
            for (int n = 0; n < Rows.Count; n++)
            {
                if (moveCells == null || moveCells.Rows.Count == 0 || (n < moveCells.Rows.Count && (moveCells.Rows[n] == null || moveCells.Rows[n].Cells.Count == 0)) || n >= moveCells.Rows.Count)
                {
                    if (Rows[n] != null && insertAtColumnIndex < Rows[n].Cells.Count)
                        Rows[n].Cells.InsertRange(insertAtColumnIndex, new T[count]);
                }
                else
                {
                    EnsureRowCellCount(new RowColumnIndex(n, insertAtColumnIndex));
                    Rows[n].Cells.InsertRange(insertAtColumnIndex, moveCells.Rows[n].Cells);
                }
            }
        }

        public void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumns(removeAtColumnIndex, count, null);
        }

        public void RemoveColumns(int removeAtColumnIndex, int count, RowColumnIndexValueArray<T> moveCells)
        {
            for (int n = 0; n < Rows.Count; n++)
            {
                if (Rows[n] != null)
                {
                    if (moveCells != null && removeAtColumnIndex < Rows[n].Cells.Count)
                    {
                        IntegerCellsArray<T> moveColumns = new IntegerCellsArray<T>();
                        moveColumns.EnsureCellCount(count);
                        for (int c = 0; c < count && removeAtColumnIndex + c < Rows[n].Cells.Count; c++)
                            moveColumns.Cells[c] = Rows[n].Cells[removeAtColumnIndex + c];
                        moveCells.EnsureRowCount(n + 1);
                        moveCells.Rows[n] = moveColumns;
                    }
                    if (removeAtColumnIndex < Rows[n].Cells.Count)
                        Rows[n].Cells.RemoveRange(removeAtColumnIndex, Math.Min(count, Rows[n].Cells.Count - removeAtColumnIndex));
                }
            }
        }
        #endregion

        #region Dictionary-like API
        public bool ContainsKey(RowColumnIndex cell)
        {
            if (cell.RowIndex < Rows.Count)
            {
                IntegerCellsArray<T> row = Rows[cell.RowIndex];
                if (row != null && cell.ColumnIndex < row.Cells.Count)
                    return cell.ColumnIndex < row.Cells.Count;
            }
            return false;
        }

        public bool TryGetValue(RowColumnIndex cell, out T value)
        {
            if (cell.RowIndex >= 0 && cell.RowIndex < Rows.Count)
            {
                IntegerCellsArray<T> row = Rows[cell.RowIndex];
                if (row != null && cell.ColumnIndex < row.Cells.Count)
                {
                    value = row.Cells[cell.ColumnIndex];
                    return true;
                }
            }

            value = default(T);
            return false;
        }

        void EnsureRowCount(int count)
        {
            if (data.Count < count)
                data.AddRange(new IntegerCellsArray<T>[count - data.Count]);
        }

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
                IntegerCellsArray<T> row = EnsureRowCellCount(cell);
                row.Cells[cell.ColumnIndex] = value;
            }
        }

        private IntegerCellsArray<T> EnsureRowCellCount(RowColumnIndex cell)
        {
            EnsureRowCount(cell.RowIndex + 1);
            IntegerCellsArray<T> row = Rows[cell.RowIndex];
            if (row == null)
            {
                row = new IntegerCellsArray<T>();
                Rows[cell.RowIndex] = row;
            }
            row.EnsureCellCount(cell.ColumnIndex + 1);
            return row;
        }

        public T this[int rowIndex, int columnIndex]
        {
            get
            {
                return this[new RowColumnIndex(rowIndex, columnIndex)];
            }
            set
            {
                this[new RowColumnIndex(rowIndex, columnIndex)] = value;
            }
        }

        /// <summary>
        /// Adds the cell and its value the dictionary.
        /// </summary>
        /// <param name="value"></param>
        public void Add(SerializableKeyValuePair<RowColumnIndex, T> value)
        {
            this[value.Key] = value.Value;
        }

        /// <summary>
        /// Adds the cell and its value the dictionary. There is no "OnMoved" callback triggered.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="value"></param>
        public void Add(RowColumnIndex cell, T value)
        {
            this[cell] = value;
        }

        /// <summary>
        /// Removes the cell from the dictionary. There is no "OnRemoved" callback triggered.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool Clear(RowColumnIndex cell)
        {
            if (cell.RowIndex < Rows.Count)
            {
                IntegerCellsArray<T> row = Rows[cell.RowIndex];
                if (row != null && cell.ColumnIndex < row.Cells.Count)
                {
                    row.Cells[cell.ColumnIndex] = default(T);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the cell from the dictionary. There is no "OnRemoved" callback triggered.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool Remove(RowColumnIndex cell)
        {
            if (cell.RowIndex < Rows.Count)
            {
                IntegerCellsArray<T> row = Rows[cell.RowIndex];
                if (row != null && cell.ColumnIndex < row.Cells.Count)
                {
                    T value = row.Cells[cell.ColumnIndex];
                    if (callback != null)
                        callback.OnRemoveCell(cell, value);
                    row.Cells[cell.ColumnIndex] = default(T);
                    return true;
                }
            }
            return false;
        }

        public void Remove(CellSpanInfoBase cellSpan)
        {
            for (int rowIndex = cellSpan.Top; rowIndex <= cellSpan.Bottom; rowIndex++)
            {
                if (rowIndex >= Rows.Count)
                    return;

                if (rowIndex < 0)
                {
                    continue;
                }

                IntegerCellsArray<T> row = Rows[rowIndex];
                if (row != null)
                {
                    for (int columnIndex = cellSpan.Left; columnIndex <= cellSpan.Right; columnIndex++)
                    {
                        if (columnIndex >= row.Cells.Count)
                            break;
                        if (columnIndex < 0)
                        {
                            continue;
                        }
                        RowColumnIndex cell = new RowColumnIndex(rowIndex, columnIndex);
                        if (callback != null)
                            callback.OnRemoveCell(cell, row.Cells[columnIndex]);
                        row.Cells[cell.ColumnIndex] = default(T);
                    }
                }
            }
        }

        public delegate void RowColumnIndexValueDelegate(RowColumnIndex cell, T value);

        public void Iterate(CellSpanInfoBase cellSpan, RowColumnIndexValueDelegate callback)
        {
            for (int rowIndex = cellSpan.Top; rowIndex <= cellSpan.Bottom; rowIndex++)
            {
                if (rowIndex >= Rows.Count)
                    return;

                IntegerCellsArray<T> row = Rows[rowIndex];
                if (row != null)
                {
                    for (int columnIndex = cellSpan.Left; columnIndex <= cellSpan.Right; columnIndex++)
                    {
                        if (columnIndex >= row.Cells.Count)
                            break;

                        RowColumnIndex cell = new RowColumnIndex(rowIndex, columnIndex);
                        callback(cell, row.Cells[columnIndex]);
                    }
                }
            }
        }

        #endregion

        #region Callbacks
        public void SetCallback(IRowColumnIndexValueArrayCallbacks<T> callback)
        {
            this.callback = callback;
        }

        #endregion

        #region IEnumerable<KeyValuePair<RowColumnIndex,T>> Members

        public IEnumerator<SerializableKeyValuePair<RowColumnIndex, T>> GetEnumerator()
        {
            for (int r = 0; r < Rows.Count; r++)
            {
                IntegerCellsArray<T> row = Rows[r];
                if (row != null)
                {
                    for (int c = 0; c < row.Cells.Count; c++)
                    {
                        RowColumnIndex cell = new RowColumnIndex(r, c);
                        yield return new SerializableKeyValuePair<RowColumnIndex, T>(cell, row.Cells[c]);
                    }
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
        public void Dispose()
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] != null)
                {
                    data[i].Dispose();
                    data[i] = null;
                }
            }
            data.Clear();
            data = null;
        }
    }

}
