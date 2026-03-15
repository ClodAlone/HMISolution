#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Scroll;

using System.ComponentModel;

namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// A collection of <see cref="TreeColumn"/> objects. The collection implements
    /// the <see cref="Syncfusion.Windows.Controls.Scroll.ILineSizeHost"/> interface and <see cref="VirtualTreeView"/> assigns
    /// the collection to the <see cref="ScrollAxisControl.ColumnWidthsProvider"/>.
    /// </summary>
    public class TreeColumns : IList<TreeColumn>, ILineSizeHost, ISupportInitialize
    {
        TreeModel treeModel;
        List<TreeColumn> inner;
        int frozenHeaderCount = 0;
        int frozenFooterCount = 0;
        bool isInitializing = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeColumns"/> class.
        /// </summary>
        /// <param name="treeModel">The tree model.</param>
        public TreeColumns(TreeModel treeModel)
        {
            this.treeModel = treeModel;
            inner = new List<TreeColumn>();
        }

        /// <summary>
        /// Gets the tree model.
        /// </summary>
        /// <value>The tree model.</value>
        public TreeModel TreeModel
        {
            get { return treeModel; }
            internal set { treeModel = value; }
        }

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

        #region IList<TreeColumn> Members

        /// <summary>
        /// Determines the index of a the item in the collection.
        /// </summary>
        /// <param name="item">The object to locate.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(TreeColumn item)
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
        public void Insert(int index, TreeColumn item)
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
        /// Gets or sets the <see cref="Syncfusion.Windows.Controls.VirtualTreeView.TreeColumn"/> at the specified index.
        /// </summary>
        /// <value></value>
        public TreeColumn this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {
                inner[index].Columns = this;
                inner[index] = value;
            }
        }

        #endregion

        #region ICollection<TreeColumn> Members

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The object to add to the collection.</param>
        public void Add(TreeColumn item)
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
            foreach (TreeColumn column in inner)
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
        public bool Contains(TreeColumn item)
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
        public void CopyTo(TreeColumn[] array, int arrayIndex)
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
        public bool Remove(TreeColumn item)
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

        #region IEnumerable<TreeColumn> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TreeColumn> GetEnumerator()
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
            return treeModel.DefaultWidth;
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

#pragma warning restore 67 // Disable The event is never used warning.

        /// <summary>
        /// Initializes the scroll axis.
        /// </summary>
        /// <param name="scrollAxis">The scroll axis.</param>
        public void InitializeScrollAxis(ScrollAxisBase scrollAxis)
        {
            PixelScrollAxis pixelScrollAxis = scrollAxis as PixelScrollAxis;

            scrollAxis.DefaultLineSize = treeModel.DefaultWidth;
            scrollAxis.LineCount = Count;

            for (int n = 0; n < Count; n++)
            {
                scrollAxis.SetLineSize(n, n, this[n].Width);
                scrollAxis.SetLineHiddenState(n, n, this[n].IsHidden);
            }
        }

        #endregion

        internal void RaiseWidthChanged(TreeColumn treeColumn)
        {
            if (!IsInitializing && LineSizeChanged != null)
            {
                int index = IndexOf(treeColumn);
                LineSizeChanged(this, new RangeChangedEventArgs(index, index));
            }
        }

        internal void RaiseHiddenChanged(TreeColumn treeColumn)
        {
            if (!IsInitializing && LineHiddenChanged != null)
            {
                int index = IndexOf(treeColumn);
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

        public void Dispose()
        {
            
        }
    }


    /// <summary>
    /// The tree column with appearance and mapping information for the cells in the
    /// column to items in the tree nodes. 
    /// </summary>
    public class TreeColumn
    {
        TreeColumns columns;
        string name = "";
        double width = double.NaN;
        TreeStyleInfo headerStyle = new TreeStyleInfo();
        TreeStyleInfo cellStyle = new TreeStyleInfo();
        string mappingName;
        bool isHidden = false;

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
        public TreeColumns Columns
        {
            get { return columns; }
            internal set { columns = value; }
        }

        /// <summary>
        /// Gets or sets the default style for cells.
        /// </summary>
        /// <value>The cell style.</value>
        public TreeStyleInfo CellStyle
        {
            get { return cellStyle; }
            set { cellStyle.CopyFrom(value); }
        }

        /// <summary>
        /// Gets or sets the header style.
        /// </summary>
        /// <value>The header style.</value>
        public TreeStyleInfo HeaderStyle
        {
            get { return headerStyle; }
            set { headerStyle.CopyFrom(value); }
        }


        /// <summary>
        /// Gets the tree model.
        /// </summary>
        /// <value>The tree model.</value>
        public TreeModel TreeModel
        {
            get
            {
                if (columns == null)
                    return null;
                else
                    return columns.TreeModel;
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
            return TreeModel.DefaultWidth;
        }

        /// <summary>
        /// Gets or sets the mapping name. This should be the name of 
        /// a property in the underlying data the TreeNodes represent.
        /// </summary>
        /// <value>The mapping name.</value>
        public string MappingName
        {
            get { return mappingName; }
            set { mappingName = value; }
        }

    }

}
