#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
#if !WinRT
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.GridCommon;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{
    /// <summary>
    /// A collection with elements derived from type <see cref="CellSpanInfoBase"/>. Internally
    /// this collection maintains both a List of CellSpanInfoBase and a so called pool. The
    /// pool allows immediate lookup of cell spans given a cells row and column index. The
    /// list allows looping through cell spans in the order they were added. CellSpanInfoCollection
    /// assumes that there is no overlap between cell spans. For any given cell there only 
    /// at most one cell span must exist. CellSpanInfoCollection is a base class for the
    /// grids GridCoveredCellInfoCollection.
    /// </summary>
    /// <typeparam name="T">The element type of this collection.</typeparam>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class CellSpanInfoCollection<T> : IList<T>, ISupportInitialize
        where T : CellSpanInfoBase
    {
        List<T> inner = new List<T>();
        CellSpanPool pool = new CellSpanPool();
        bool isInitializing = false;

        #region Public Members

        // ICellSpanHost
        /// <summary>
        /// Gets the cell span.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>The cell span if found; othwerwise null if there is no cell span for the specified row and column.</returns>
        public T GetCellSpan(int rowIndex, int columnIndex)
        {
            T result;
            if (OnGetCellSpan(rowIndex, columnIndex, out result))
            {
                if (!result.Contains(rowIndex, columnIndex))
                    throw new Exception(String.Format("Unexpected range for cell {0}, {1}: Returned range {2} does not contain cell.", rowIndex, columnIndex, result));
                return result;
            }
            return pool[rowIndex, columnIndex];
        }

        /// <summary>
        /// This virtual method is called form <see cref="GetCellSpan"/>.
        /// Override this method to return a custom cell span that include
        /// the specified cells row and column index. In such case you should also return true.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="result">The result cell span.</param>
        /// <returns></returns>
        protected virtual bool OnGetCellSpan(int rowIndex, int columnIndex, out T result)
        {
            result = default(T);
            return false;
        }


        /// <summary>
        /// Resets the cached information for last found cell span.
        /// </summary>
        public void ResetCache()
        {
#warning No caching has been implemented yet
        }

        /// <summary>
        /// Gets a value indicating whether this collection is empty.
        /// </summary>
        /// <value><c>true</c> if this collection is empty; otherwise, <c>false</c>.</value>
        public virtual bool IsEmpty
        {
            get
            {
                return Count == 0;
            }
        }

        // Public Members
        /// <summary>
        /// Resets the cell span and removes it from the collection.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        public void ResetCellSpan(int rowIndex, int columnIndex)
        {
            T cci = pool[rowIndex, columnIndex];
            if (cci != null)
                Remove(cci);
        }

        /// <summary>
        /// Resets the cell span and removes it from the collection.
        /// </summary>
        /// <param name="cci">The cci.</param>
        public void ResetCellSpan(CellSpanInfoBase cci)
        {
            List<T> cellSpans = pool.SearchCellSpan(cci);
            if (cellSpans.Count == Count)
                Clear();
            else
            {
                foreach (T item in cellSpans)
                    Remove(item);
            }
        }

        /// <summary>
        /// Searches the cell span.
        /// </summary>
        /// <param name="span">The cell span.</param>
        /// <returns></returns>
        public List<T> SearchCellSpan(CellSpanInfoBase span)
        {
            return pool.SearchCellSpan(span);
        }

        /// <summary>
        /// Checks if the cell span exists.
        /// </summary>
        /// <param name="span">The cell span.</param>
        /// <returns></returns>
        public bool CheckExistCellSpan(CellSpanInfoBase span)
        {
            return pool.CheckExistCellSpan(span);
        }

        #endregion

        #region Insert and Remove Rows

        /// <summary>
        /// Inserts the rows.
        /// </summary>
        /// <param name="insertAtRowIndex">Index of the insert at row.</param>
        /// <param name="count">The count.</param>
        public void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRows(insertAtRowIndex, count, null);
        }

        /// <summary>
        /// Inserts the rows.
        /// </summary>
        /// <param name="insertAtRowIndex">Index of the insert at row.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCellsState">State of the move cells.</param>
        public void InsertRows(int insertAtRowIndex, int count, CellSpanInfoCollection<T> moveCellsState)
        {
            List<T> previous = inner;
            inner = new List<T>();
            foreach (T coveredCell in previous)
            {
                if (coveredCell.Top >= insertAtRowIndex)
                    coveredCell.Offset(count, 0);
                inner.Add(coveredCell);
            }
            if (moveCellsState != null)
            {
                foreach (T coveredCell in moveCellsState)
                {
                    coveredCell.Offset(insertAtRowIndex, 0);
                    inner.Add(coveredCell);
                }
            }
            pool.InitalizeFromCollecton(inner);
        }

        /// <summary>
        /// Removes the rows.
        /// </summary>
        /// <param name="removeAtRowIndex">Index of the remove at row.</param>
        /// <param name="count">The count.</param>
        public void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRows(removeAtRowIndex, count, null);
        }

        /// <summary>
        /// Removes the rows.
        /// </summary>
        /// <param name="removeAtRowIndex">Index of the remove at row.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCellsState">State of the move cells.</param>
        public void RemoveRows(int removeAtRowIndex, int count, CellSpanInfoCollection<T> moveCellsState)
        {
            List<T> removed = (moveCellsState != null) ? new List<T>() : null;
            List<T> previous = inner;
            inner = new List<T>();
            foreach (T coveredCell in previous)
            {
                if (coveredCell.Top >= removeAtRowIndex)
                {
                    if (coveredCell.Top >= removeAtRowIndex + count)
                    {
                        coveredCell.Offset(-count, 0);
                        inner.Add(coveredCell);
                    }
                    else if (removed != null)
                    {
                        coveredCell.Offset(-removeAtRowIndex, 0);
                        removed.Add(coveredCell);
                    }
                }
                else
                    inner.Add(coveredCell);
            }
            pool.InitalizeFromCollecton(inner);
            if (moveCellsState != null)
                moveCellsState.inner.AddRange(removed);
        }

        #endregion

        #region Insert and Remove Columns

        /// <summary>
        /// Inserts the columns.
        /// </summary>
        /// <param name="insertAtColumnIndex">Index of the insert at column.</param>
        /// <param name="count">The count.</param>
        public void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumns(insertAtColumnIndex, count, null);
        }

        /// <summary>
        /// Inserts the columns.
        /// </summary>
        /// <param name="insertAtColumnIndex">Index of the insert at column.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCellsState">State of the move cells.</param>
        public void InsertColumns(int insertAtColumnIndex, int count, CellSpanInfoCollection<T> moveCellsState)
        {
            List<T> previous = inner;
            inner = new List<T>();
            foreach (T coveredCell in previous)
            {
                if (coveredCell.Left >= insertAtColumnIndex)
                    coveredCell.Offset(0, count);
                inner.Add(coveredCell);
            }
            if (moveCellsState != null)
            {
                foreach (T coveredCell in moveCellsState)
                {
                    coveredCell.Offset(0, insertAtColumnIndex);
                    inner.Add(coveredCell);
                }
            }
            pool.InitalizeFromCollecton(inner);
        }

        /// <summary>
        /// Removes the columns.
        /// </summary>
        /// <param name="removeAtColumnIndex">Index of the remove at column.</param>
        /// <param name="count">The count.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumns(removeAtColumnIndex, count, null);
        }

        /// <summary>
        /// Removes the columns.
        /// </summary>
        /// <param name="removeAtColumnIndex">Index of the remove at column.</param>
        /// <param name="count">The count.</param>
        /// <param name="moveCellsState">State of the move cells.</param>
        public void RemoveColumns(int removeAtColumnIndex, int count, CellSpanInfoCollection<T> moveCellsState)
        {
            List<T> removed = (moveCellsState != null) ? new List<T>() : null;
            List<T> previous = inner;
            inner = new List<T>();
            foreach (T coveredCell in previous)
            {
                if (coveredCell.Left >= removeAtColumnIndex)
                {
                    if (coveredCell.Left >= removeAtColumnIndex + count)
                    {
                        coveredCell.Offset(0, -count);
                        inner.Add(coveredCell);
                    }
                    else if (removed != null)
                    {
                        coveredCell.Offset(0, -removeAtColumnIndex);
                        removed.Add(coveredCell);
                    }
                }
                else
                    inner.Add(coveredCell);
            }
            pool.InitalizeFromCollecton(inner);
            if (moveCellsState != null)
                moveCellsState.inner.AddRange(removed);
        }

        #endregion


        #region ISupportInitialize Members

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
            isInitializing = true;
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            if (isInitializing)
                pool.InitalizeFromCollecton(inner);
            isInitializing = false;
            ResetCache();
        }

        #endregion

        #region IList<T> Members

        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            if (item == null || item != pool[item.Top, item.Left])
                return -1;

            return inner.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the collection at the specified index. An exception is thrown if
        /// the cell range overlaps with another cell span in the collection.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the collection.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the collection.</exception>
        /// <exception cref="System.ArgumentNullException">The item is null.</exception>
        public virtual void Insert(int index, T item)
        {
            if (item == null)
                throw new ArgumentNullException();
            
            pool.AddCellSpan(item);
            inner.Insert(index, item);
            ResetCache();
        }

        /// <summary>
        /// Removes the collection item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the collection.</exception>
        /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
        public void RemoveAt(int index)
        {
            pool.ResetCellSpan(inner[index]);
            inner.RemoveAt(index);
            ResetCache();
        }

        /// <summary>
        /// Gets or sets the cell span at the specified index.
        /// </summary>
        /// <value></value>
        public T this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                
                pool.ResetCellSpan(inner[index]);
                inner[index] = value;
                ResetCache();
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Adds an item to the collection. An exception is thrown if
        /// the cell range overlaps with another cell span in the collection.
        /// </summary>
        /// <param name="item">The object to add to the collection.</param>
        /// <exception cref="System.ArgumentNullException">The item is null.</exception>
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException();
            pool.AddCellSpan(item);
            inner.Add(item);
            ResetCache();
            Added(item);
        }

        protected virtual void Added(T item)
        {

        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            pool.Clear();
            inner.Clear();
            ResetCache();
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the collection; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return item != null && item == pool[item.Top, item.Left];
        }

        /// <summary>
        /// Copies the elements of the collection to an <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied from collection. The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source collection is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the collection.</returns>
        public int Count
        {
            get { return inner.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>this collection is never read-only.</returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the collection; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original collection.
        /// </returns>
        public bool Remove(T item)
        {
            if (!Contains(item))
                return false;

            pool.ResetCellSpan(item);
            ResetCache();
            bool result = inner.Remove(item);
            if (result)
                Removed(item);
            return result;
        }

        protected virtual void Removed(T item)
        {

        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)inner).GetEnumerator();
        }

        #endregion

        #region Private CellSpanPool
        class CellSpanInfoInRow : Dictionary<int, T>
        {
        }

        class CellSpanRows : Dictionary<int, CellSpanInfoInRow>
        {
        }

        class CellSpanPool
        {
            CellSpanRows rows = new CellSpanRows();

            public T this[int rowIndex, int columnIndex]
            {
                get
                {
                    CellSpanInfoInRow row;
                    if (rows.ContainsKey(rowIndex))
                    {
                        row = rows[rowIndex];
                        if (row.ContainsKey(columnIndex))
                            return row[columnIndex];
                    }
                    return null;
                }
                //set
                //{
                //    CellSpanInfoInRow row;
                //    if (rows.ContainsKey(rowIndex))
                //        row = rows[rowIndex];
                //    else
                //        rows[rowIndex] = row = new CellSpanInfoInRow();

                //    row[columnIndex] = value;
                //}
            }

            public void Clear()
            {
                rows.Clear();
            }

            public void ResetCellSpan(CellSpanInfoBase span)
            {
                for (int rowIndex = span.Top; rowIndex <= span.Bottom; rowIndex++)
                {
                    CellSpanInfoInRow row;
                    if (rows.ContainsKey(rowIndex))
                        row = rows[rowIndex];
                    else
                        continue;

                    for (int columnIndex = span.Left; columnIndex <= span.Right; columnIndex++)
                    {
                        if (row.ContainsKey(columnIndex))
                            row.Remove(columnIndex); // found T
                    }

                    if (row.Count == 0)
                        rows.Remove(rowIndex);
                }
            }

            public bool CheckExistCellSpan(CellSpanInfoBase span)
            {
                for (int rowIndex = span.Top; rowIndex <= span.Bottom; rowIndex++)
                {
                    CellSpanInfoInRow row;
                    if (rows.ContainsKey(rowIndex))
                        row = rows[rowIndex];
                    else
                        continue;

                    for (int columnIndex = span.Left; columnIndex <= span.Right; columnIndex++)
                    {
                        if (row.ContainsKey(columnIndex))
                            return true; // found T
                    }
                }

                return false;
            }

            public List<T> SearchCellSpan(CellSpanInfoBase span)
            {
                List<T> result = new List<T>();

                for (int rowIndex = span.Top; rowIndex <= span.Bottom; rowIndex++)
                {
                    CellSpanInfoInRow row;
                    if (rows.ContainsKey(rowIndex))
                        row = rows[rowIndex];
                    else
                        continue;

                    for (int columnIndex = span.Left; columnIndex <= span.Right; columnIndex++)
                    {
                        if (row.ContainsKey(columnIndex))
                        {
                            T cci = row[columnIndex]; 
                            columnIndex = cci.Right; // skip columns with same T 
                            if (!result.Contains(cci))
                                result.Add(cci);
                        }
                    }
                }

                return result;
            }

            public void AddCellSpan(T item)
            {
                if (CheckExistCellSpan(item))
                    throw new Exception(String.Format("Conflict detected when trying to save {0}", item));
                SaveCellSpan(item);
            }

            public void SaveCellSpan(T span)
            {
                for (int rowIndex = span.Top; rowIndex <= span.Bottom; rowIndex++)
                {
                    CellSpanInfoInRow row;
                    if (rows.ContainsKey(rowIndex))
                        row = rows[rowIndex];
                    else
                        rows[rowIndex] = row = new CellSpanInfoInRow();

                    for (int columnIndex = span.Left; columnIndex <= span.Right; columnIndex++)
                    {
                        if (row.ContainsKey(columnIndex))
                            throw new Exception(String.Format("Conflict occured when saving covered cell {0}. Did you forget to call CheckExistCellSpan?", span)); // conflict occured.

                        row[columnIndex] = span;
                    }
                }
            }

            public void InitalizeFromCollecton(IEnumerable<T> cellSpans)
            {
                rows.Clear();
                foreach (T cci in cellSpans)
                    SaveCellSpan(cci);
            }

        }
        #endregion
    }
}
