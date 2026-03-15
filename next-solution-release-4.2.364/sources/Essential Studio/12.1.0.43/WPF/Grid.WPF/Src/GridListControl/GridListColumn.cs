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
using Syncfusion.Windows.Controls.Scroll;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// A collection of <see cref="GridListColumn"/> objects. The collection implements
    /// the <see cref="Syncfusion.Windows.Controls.Scroll.ILineSizeHost"/> interface and <see cref="VirtualGridListView"/> assigns
    /// the collection to the <see cref="ScrollAxisControl.ColumnWidthsProvider"/>.
    /// </summary>
    public class GridListColumns : IList<GridListColumn>, IPaddedEditableLineSizeHost, ISupportInitialize
    {
        GridListModel gridModel;
        List<GridListColumn> inner;
        int frozenHeaderCount = 0;
        int frozenFooterCount = 0;
        bool isInitializing = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridListColumns"/> class.
        /// </summary>
        /// <param name="gridModel">The grid model.</param>
        public GridListColumns(GridListModel gridModel)
        {
            this.gridModel = gridModel;
            inner = new List<GridListColumn>();
        }

        /// <summary>
        /// Gets the grid model.
        /// </summary>
        /// <value>The grid model.</value>
        public GridListModel Model
        {
            get { return gridModel; }
            internal set { gridModel = value; }
        }

        #region ISupportInitialize

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
            isInitializing = false;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is initializing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initializing; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitializing
        {
            get { return isInitializing; }
        }

        #endregion

        #region IList<GridListColumn> Members

        /// <summary>
        /// Determines the index of a the item in the collection.
        /// </summary>
        /// <param name="item">The object to locate.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(GridListColumn item)
        {
            return inner.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the collection.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the collection.</exception>
        public void Insert(int index, GridListColumn item)
        {
            item.Columns = this;
            inner.Insert(index, item);
            RaiseLineCountChanged();
        }

        /// <summary>
        /// Removes the collection item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the collection.</exception>
        public void RemoveAt(int index)
        {
            if (inner[index].Columns == this)
                inner[index].Columns = null;
            inner.RemoveAt(index);
            RaiseLineCountChanged();
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Tools.Controls.GridListColumn"/> at the specified index.
        /// </summary>
        /// <value></value>
        public GridListColumn this[int index]
        {
            get
            {
                if (index < inner.Count && index>=0)//Temp Index>=0 Added
                {
                    return inner[index];
                }

                return GridListColumn.Empty;
            }
            set
            {
                inner[index].Columns = this;
                inner[index] = value;
            }
        }

        #endregion

        #region ICollection<GridListColumn> Members

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The object to add to the collection.</param>
        public void Add(GridListColumn item)
        {
            item.Columns = this;
            inner.Add(item);
            RaiseLineCountChanged();
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            foreach (GridListColumn column in inner)
            {
                if (column.Columns == this)
                    column.Columns = null;
            }
            inner.Clear();
            RaiseLineCountChanged();
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the collection; otherwise, false.
        /// </returns>
        public bool Contains(GridListColumn item)
        {
            return item.Columns == this;
        }

        /// <summary>
        /// Copies the elements of the collection to an <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from collection. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source collection is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(GridListColumn[] array, int arrayIndex)
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
        public bool Remove(GridListColumn item)
        {
            if (inner.Remove(item))
            {
                if (item.Columns == this)
                    item.Columns = null;
                RaiseLineCountChanged();
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<GridListColumn> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<GridListColumn> GetEnumerator()
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

        #region ILineSizeHost Members

        private void RaiseLineCountChanged()
        {
            if (!IsInitializing && LineCountChanged != null)
                LineCountChanged(this, EventArgs.Empty);
        }

        double ILineSizeHost.GetDefaultLineSize()
        {
            return gridModel.DefaultColumnWidth;
        }

        int ILineSizeHost.GetLineCount()
        {
            return Count;
        }

        double ILineSizeHost.GetSize(int index, out int repeatValueCount)
        {
            repeatValueCount = 1;
            return this[index].Width;
        }

        int ILineSizeHost.GetHeaderLineCount()
        {
            return this.FrozenHeaderCount;
        }

        int ILineSizeHost.GetFooterLineCount()
        {
            return this.FrozenFooterCount;
        }

        bool ILineSizeHost.GetHidden(int index, out int repeatValueCount)
        {
            repeatValueCount = 1;
            return this[index].IsHidden;
        }

        /// <summary>
        /// Occurs when a lines size was changed.
        /// </summary>
        public event RangeChangedEventHandler LineSizeChanged;

        /// <summary>
        /// Occurs when a lines hidden state changed.
        /// </summary>
        public event HiddenRangeChangedEventHandler LineHiddenChanged;

        event DefaultLineSizeChangedEventHandler ILineSizeHost.DefaultLineSizeChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Occurs when the line count was changed.
        /// </summary>
        public event EventHandler LineCountChanged;

        /// <summary>
        /// Occurs when the header line count was changed.
        /// </summary>
        public event EventHandler HeaderLineCountChanged;

        /// <summary>
        /// Occurs when the footer line count was changed.
        /// </summary>
        public event EventHandler FooterLineCountChanged;

#pragma warning disable 67 // Disable The event is never used warning.

        /// <summary>
        /// Occurs when lines were inserted.
        /// </summary>
        public event LinesInsertedEventHandler LinesInserted;

        /// <summary>
        /// Occurs when lines were removed.
        /// </summary>
        public event LinesRemovedEventHandler LinesRemoved;

#pragma warning restore 67 

        /// <summary>
        /// Initializes the scroll axis.
        /// </summary>
        /// <param name="scrollAxis">The scroll axis.</param>
        public void InitializeScrollAxis(ScrollAxisBase scrollAxis)
        {
            PixelScrollAxis pixelScrollAxis = scrollAxis as PixelScrollAxis;

            scrollAxis.DefaultLineSize = gridModel.DefaultColumnWidth;
            scrollAxis.LineCount = Count;

            for (int n = 0; n < Count; n++)
            {
                scrollAxis.SetLineSize(n, n, this[n].Width);
                scrollAxis.SetLineHiddenState(n, n, this[n].IsHidden);
            }
        }

        #endregion

        internal void RaiseWidthChanged(GridListColumn gridColumn)
        {
            if (!IsInitializing && LineSizeChanged != null)
            {
                int index = IndexOf(gridColumn);
                LineSizeChanged(this, new RangeChangedEventArgs(index, index));
            }
        }

        internal void RaiseHiddenChanged(GridListColumn gridColumn)
        {
            if (!IsInitializing && LineHiddenChanged != null)
            {
                int index = IndexOf(gridColumn);
                LineHiddenChanged(this, new HiddenRangeChangedEventArgs(index, index, false));
            }
        }

        /// <summary>
        /// Gets or sets the frozen footer count.
        /// </summary>
        /// <value>The frozen footer count.</value>
        public int FrozenFooterCount
        {
            get { return frozenFooterCount; }
            set
            {
                if (frozenFooterCount != value)
                {
                    frozenFooterCount = value;
                    if (!IsInitializing && FooterLineCountChanged != null)
                        FooterLineCountChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the frozen header count.
        /// </summary>
        /// <value>The frozen header count.</value>
        public int FrozenHeaderCount
        {
            get { return frozenHeaderCount; }
            set
            {
                if (frozenHeaderCount != value)
                {
                    frozenHeaderCount = value;
                    if (!IsInitializing && HeaderLineCountChanged != null)
                        HeaderLineCountChanged(this, EventArgs.Empty);
                }
            }
        }

        #region  Members - Editing

        public double DefaultLineSize
        {
            get
            {
                return Model.DefaultColumnWidth;
            }
            set
            {
                Model.DefaultColumnWidth = value;
            }
        }

        public int FooterLineCount
        {
            get
            {
                return FrozenFooterCount;
            }
            set
            {
                FrozenFooterCount = value;
            }
        }


        public bool SupportsNestedLines
        {
            get { return false; }
        }

        public IEditableLineSizeHost GetNestedLines(int index)
        {
            return null;
        }

        public void SetNestedLines(int index, IEditableLineSizeHost nestedLines)
        {
        }

        public int HeaderLineCount
        {
            get
            {
                return FrozenHeaderCount;
            }
            set
            {
                FrozenHeaderCount = value;
            }
        }

        public void InsertLines(int insertAtLine, int count)
        {
        }

        public void InsertLines(int insertAtLine, int count, LineSizeCollection moveLines)
        {
        }

        public int LineCount
        {
            get
            {
                return Count;
            }
            set
            {
            }
        }

        public void RemoveLines(int removeAtLine, int count)
        {
        }

        public void RemoveLines(int removeAtLine, int count, LineSizeCollection moveLines)
        {
        }

        public void SetHidden(int from, int to, bool hide)
        {
            for (int n = from; n <= to; n++)
                this[n].IsHidden = true;
        }

        public void SetRange(int from, int to, double size)
        {
            for (int n = from; n <= to; n++)
                this[n].Width = size;
        }

        double IEditableLineSizeHost.this[int index]
        {
            get
            {
                return this[index].Width;
            }
            set
            {
                this[index].Width = value;
            }
        }

        public double TotalExtent
        {
            get 
            {
                return double.NaN;
            }
        }

        #endregion

        #region  Members - InsertRemove

        public bool SupportsInsertRemove
        {
            get { return true; }
        }

        public void InsertLines(int insertAtLine, int count, IEditableLineSizeHost movelines)
        {
            GridListColumns moveLines = (GridListColumns)movelines;
            if (movelines != null)
            {
                inner.InsertRange(insertAtLine, moveLines.inner);
            }
            else
            {
                GridListColumn[] d = new GridListColumn[count];
                inner.InsertRange(insertAtLine, d);
            }
        }

        public void RemoveLines(int removeAtLine, int count, IEditableLineSizeHost movelines)
        {
            GridListColumns moveLines = (GridListColumns)movelines;
            List<GridListColumn> d = inner;
            this.inner = new List<GridListColumn>();

            for (int n = 0; n < d.Count; n++)
            {
                if (n >= removeAtLine)
                {
                    if (n >= removeAtLine + count)
                        inner.Add(d[n]);
                    else if (moveLines != null)
                        moveLines.inner.Add(d[n]);
                }
                else
                    inner.Add(d[n]);
            }
        }

        public IEditableLineSizeHost CreateMoveLines()
        {
            return new GridListColumns(null);
        }

        #endregion

        #region IPaddedEditableLineSizeHost Members

        public double PaddingDistance
        {
            get;
            set;
        }

        #endregion

        public IDisposable DeferRefresh()
        {
            return null;
        }

        public void Dispose()
        {
            
        }
    }


    /// <summary>
    /// The grid column with appearance and mapping information for the cells in the
    /// column to items in the grid nodes. 
    /// </summary>
    public class GridListColumn
    {
        GridListColumns columns;
        string name = "";
        double width = double.NaN;
        GridStyleInfo headerStyle = new GridStyleInfo();
        GridStyleInfo cellStyle = new GridStyleInfo();
        string mappingName;
        bool isHidden = false;

        public static GridListColumn Empty
        {
            get
            {
                return new GridListColumn() { Width = -1 };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column is hidden.
        /// </summary>
        /// <value><c>true</c> if this column is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden
        {
            get { return isHidden; }
            set
            {
                if (isHidden != value)
                {
                    isHidden = value;
                    if (columns != null)
                        columns.RaiseHiddenChanged(this);
                }
            }
        }

        /// <summary>
        /// Gets the columns collection this column belongs to.
        /// </summary>
        /// <value>The columns.</value>
        public GridListColumns Columns
        {
            get { return columns; }
            internal set { columns = value; }
        }

        /// <summary>
        /// Gets or sets the default style for cells.
        /// </summary>
        /// <value>The cell style.</value>
        public GridStyleInfo CellStyle
        {
            get { return cellStyle; }
            set { cellStyle.CopyFrom(value); }
        }

        /// <summary>
        /// Gets or sets the header style.
        /// </summary>
        /// <value>The header style.</value>
        public GridStyleInfo HeaderStyle
        {
            get { return headerStyle; }
            set { headerStyle.CopyFrom(value); }
        }


        /// <summary>
        /// Gets the grid model.
        /// </summary>
        /// <value>The grid model.</value>
        public GridListModel Model
        {
            get
            {
                if (columns == null)
                    return null;
                else
                    return columns.Model;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    if (columns != null)
                        columns.RaiseWidthChanged(this);
                }
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <returns></returns>
        public double GetWidth()
        {
            if (!double.IsNaN(width))
                return width;
            return Model.DefaultColumnWidth;
        }

        /// <summary>
        /// Gets or sets the mapping name. This should be the name of 
        /// a property in the underlying data the GridListNodes represent.
        /// </summary>
        /// <value>The mapping name.</value>
        public string MappingName
        {
            get { return mappingName; }
            set { mappingName = value; }
        }

    }

}
