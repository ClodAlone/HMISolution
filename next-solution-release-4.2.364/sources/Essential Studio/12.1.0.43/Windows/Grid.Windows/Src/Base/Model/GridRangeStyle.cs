//-------------------------------------------------------------------------------------------------
// <copyright file="GridRangeStyle.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides a collection editor that is tailored to adding and removing <see cref="GridRangeStyle"/> objects
    /// from the <see cref="GridControl.RangeStyles"/> collection in a <see cref="GridControl"/>.
    /// </summary>
    /// <seealso cref="GridBoundColumn"/>
    public class GridRangeStyleCollectionEditor : CollectionEditor
    {
        private PropertyGridContextMenu pgMenu;

        /// <summary>
        /// Initializes a new <see cref="GridRangeStyleCollectionEditor"/> object.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public GridRangeStyleCollectionEditor(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Creates a new form to display and edit the current collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.CollectionEditor.CollectionForm"/> to provide as the user interface for editing the collection.
        /// </returns>
        /// <override/>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm collectionForm = base.CreateCollectionForm();

            PropertyGrid pg = WinFormsUtils.GetPropertyGridInControl(collectionForm);

            if (pg != null)
            {
                this.pgMenu = new PropertyGridContextMenu(pg);
            }

            return collectionForm;
        }
    }

    #region GridRangeStyleCollection
    /// <summary>
    /// A collection of <see cref="GridRangeStyle"/> objects. This collection is a wrapper collection
    /// for cells in the <see cref="GridData"/> object. It provides support for modifying
    /// cells through a CollectionEditor and code serialization at design-time.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GridRangeStyleCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class GridRangeStyleCollection : IDisposable, IList
    {
        internal ArrayList _inner = new ArrayList();
        SFTable data;
        bool modified = false;
        bool readOnly = false;
        internal GridControlBase control;

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        event ListPropertyChangedEventHandler Changing;

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        event ListPropertyChangedEventHandler Changed;

        /// <override/>
        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return String.Format("GridRangeStyleCollection: Count {0}", Count);
        }

        /// <overload>
        /// Initializes a new empty collection.
        /// </overload>
        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        internal GridRangeStyleCollection()
        {
        }

        static bool allowCombineCells = true;

        /// <exclude/>
        /// <summary>Gets or sets a value indicating whether to Allow Combine Cells. Used internally.</summary>
        public static bool AllowCombineCells
        {
            get
            {
                return allowCombineCells;
            }

            set
            {
                allowCombineCells = value;
            }
        }

        internal GridRangeStyleCollection(SFTable data)
        {
            // Create collection from GridData and combine cells that have same parameters.
            if (data != null)
            {
                GridCoveredCellPool pool = new GridCoveredCellPool();

                int rowCount = data.Rows.Count;
                for (int n = 0; n < rowCount; n++)
                {
                    ArrayList al = (ArrayList)data.Rows[n];
                    if (al != null)
                    {
                        for (int i = 0; i < al.Count; i++)
                        {
                            if (al[i] != null)
                            {
                                GridRangeInfo rg;
                                // Check if cell is part of a previous RangeStyle, if yes - skip it.
                                if (pool.GetSpanCellsRowCol(n, i, out rg))
                                {
                                    continue;
                                }

                                GridStyleInfoStore s1 = (GridStyleInfoStore)al[i];

                                if (s1.IsEmpty)
                                {
                                    continue;
                                }

                                // Search for equal cells in same row
                                int i2 = i;
                                for (int i21 = i + 1; AllowCombineCells && i21 < al.Count; i21++)
                                {
                                    GridStyleInfoStore s2 = (GridStyleInfoStore)al[i21];
                                    if (s2 == null || !s1.Equals(s2))
                                    {
                                        break;
                                    }

                                    i2 = i21;
                                }

                                // Search for equal rows where all columns match with previos row.
                                int n2 = n;
                                for (int n21 = n + 1; AllowCombineCells && n21 < rowCount; n21++)
                                {
                                    int i3 = -1;

                                    ArrayList al2 = (ArrayList)data.Rows[n21];
                                    if (al2 != null)
                                    {
                                        i3 = i;
                                        for (; i3 <= i2; i3++)
                                        {
                                            GridStyleInfoStore s2 = (GridStyleInfoStore)al2[i3];
                                            if (s2 == null || !s1.Equals(s2))
                                            {
                                                i3 = -1;
                                                break;
                                            }
                                        }
                                    }

                                    if (i3 == -1)
                                    {
                                        break;
                                    }

                                    n2 = n21;
                                }

                                if (n2 > n)
                                {
                                    // skip these cells when visited again.
                                    pool.StoreSpanCells(GridRangeInfo.Cells(n, i, n2, i2));
                                }

                                GridRangeStyle rs = new GridRangeStyle(n - 1, i - 1, n2 - 1, i2 - 1, (GridStyleInfoStore)al[i]);
                                i = i2;
                                rs.collection = this;
                                this._inner.Add(rs);
                            }
                        }
                    }
                }

                pool.Clear();
                this.data = data;
            }
        }

        bool GetRowColFromRange(GridRangeInfo range, out int rowIndex, out int colIndex, out int rowIndex2, out int colIndex2)
        {
            switch (range.RangeType)
            {
                default:
                    rowIndex = -1;
                    colIndex = -1;
                    rowIndex2 = -1;
                    colIndex2 = -1;
                    break;
                case GridRangeInfoType.Rows:
                    rowIndex = range.Top;
                    colIndex = -1;
                    rowIndex2 = range.Bottom;
                    colIndex2 = -1;
                    break;
                case GridRangeInfoType.Cols:
                    rowIndex = -1;
                    colIndex = range.Left;
                    rowIndex2 = -1;
                    colIndex2 = range.Right;
                    break;
                case GridRangeInfoType.Cells:
                    rowIndex = range.Top;
                    colIndex = range.Left;
                    rowIndex2 = range.Bottom;
                    colIndex2 = range.Right;
                    break;
            }

            return !range.IsEmpty;
        }

        internal void WriteToData()
        {
            WriteToData(data);
        }

        internal void WriteToData(SFTable data)
        {
            if (data == null)
            {
                return;
            }

            data.Rows.Clear();
            foreach (GridRangeStyle cellStyle in this)
            {
                WriteToData(cellStyle, false);
            }

            this.data = data;
            if (control != null)
            {
                control.Model.ResetVolatileData();
                control.Invalidate();
            }
        }

        internal void WriteToData(GridRangeStyle[] cellStyles, bool invalidate)
        {
            foreach (GridRangeStyle cellStyle in cellStyles)
            {
                WriteToData(cellStyle, false);
            }

            if (control != null && invalidate)
            {
                control.Model.ResetVolatileData();
                control.Invalidate();
            }
        }

        internal void WriteToData(GridRangeStyle cellStyle, bool invalidate)
        {
            if (data == null)
            {
                return;
            }

            int rowIndex;
            int colIndex;
            int rowIndex2;
            int colIndex2;
            if (GetRowColFromRange(cellStyle.Range, out rowIndex, out colIndex, out rowIndex2, out colIndex2))
            {
                for (int r = rowIndex; r <= rowIndex2; r++)
                {
                    for (int c = colIndex; c <= colIndex2; c++)
                    {
                        data.RowCount = Math.Max(r + 2, data.RowCount);
                        data.ColCount = Math.Max(c + 2, data.ColCount);
                        data[r + 1, c + 1] = (GridStyleInfoStore)cellStyle.StyleInfo.Store.Clone();
                    }
                }
            }

            if (control != null && invalidate)
            {
                control.Model.ResetVolatileData();
                control.Invalidate();
            }
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="cellStyles">The Array with elements that should be added to the end of the collection. 
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic). 
        /// </param>
        public void AddRange(GridRangeStyle[] cellStyles)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, 0, null, null));
            modified = true;
            int index = this.Count;
            this._inner.AddRange(cellStyles);
            WriteToData(cellStyles, true);
            for (int n = 0; n < cellStyles.Length; n++)
            {
                cellStyles[n].collection = this;
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index++, cellStyles[n], null));
            }

            GridControl grid = this.control as GridControl;
            if (grid != null && !grid.ShouldSerializeSerializeCellsBehavior())
            {
                grid.SerializeCellsBehavior = GridSerializeCellsBehavior.SerializeAsRangeStylesIntoCode;
            }
        }

        /// <summary>
        /// Raises the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (Changing != null)
            {
                Changing(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        void OnChanged(ListPropertyChangedEventArgs e)
        {
            modified = true;
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        /// <override/>
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the
        /// current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current
        /// <see cref="T:System.Object" />. </param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is GridRangeStyleCollection))
            {
                return false;
            }

            return InternalEquals((GridRangeStyleCollection)obj);
        }

        /// <override/>
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the collection is modified from its default state.
        /// </summary>
        bool IsModified
        {
            get
            {
                return modified;
            }

            set
            {
                if (modified != value)
                {
                    modified = value;
                }
            }
        }

        /// <summary>
        /// Compares each element with the elements of another collection.
        /// </summary>
        /// <param name="other">The collection to compare to.</param>
        /// <returns>True if all elements are equal and in the same order; False otherwise.</returns>
        bool InternalEquals(GridRangeStyleCollection other)
        {
            int count = Count;
            if (other.Count != count)
            {
                return false;
            }

            for (int n = 0; n < count; n++)
            {
                if (!this[n].Equals(other[n]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets / sets the elements at the zero-based index.
        /// </summary>
        public GridRangeStyle this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (GridRangeStyle)_inner[index];
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                if (_inner[index] != value)
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    _inner[index] = value;
                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(GridRangeStyle value)
        {
            if (value == null)
            {
                return false;
            }

            return _inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(GridRangeStyle value)
        {
            if (value == null)
            {
                return -1;
            }

            return _inner.IndexOf(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from ArrayList. The Array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(GridRangeStyle[] array, int index)
        {
            int n = 0;
            foreach (GridRangeStyle item in this)
            {
                array[index + n] = item;
                n++;
            }
        }

        /// <summary>
        /// Gets SyncRoot. Not supported.
        /// </summary>
        GridRangeStyleCollection SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An IEnumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection. 
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public GridRangeStyleCollectionEnumerator GetEnumerator()
        {
            return new GridRangeStyleCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridRangeStyle value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
            _inner.Insert(index, value);
            value.collection = this;
            this.WriteToData(value, true);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
        }

        /// <summary>
        /// Removes the specified descriptor element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(GridRangeStyle value)
        {
            if (value == null)
            {
                return;
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, -1, value, null));
            _inner.Remove(value);
            WriteToData();
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, -1, value, null));
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(GridRangeStyle value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));

            int index = _inner.Add(value);
            value.collection = this;
            WriteToData(value, true);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            GridRangeStyle value = (GridRangeStyle)_inner[index];
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            _inner.RemoveAt(index);
            WriteToData();
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Disposes the object and collection items.
        /// </summary>
        public void Dispose()
        {
            foreach (GridRangeStyle db in _inner)
            {
                db.Dispose();
            }

            _inner.Clear();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            if (_inner.Count > 0)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                _inner.Clear();
                WriteToData();
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }

            this.modified = true;
        }

        /// <summary>
        /// Gets a value indicating whether the collection is Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return readOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether collection has no fixed size. Normally False since this collection has no fixed size. Only when it is Read-only 
        /// IsFixedSize returns True.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return readOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Is Synchronized. Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. 
        /// </summary>
        public int Count
        {
            get
            {
                return _inner.Count;
            }
        }

        #region IList Private Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (GridRangeStyle)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridRangeStyle)value);
        }

        void IList.Remove(object value)
        {
            Remove((GridRangeStyle)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridRangeStyle)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridRangeStyle)value);
        }

        int IList.Add(object value)
        {
            return Add((GridRangeStyle)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridRangeStyle[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Private Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="GridRangeStyle"/> elements of a <see cref="GridRangeStyleCollection"/>.
    /// </summary>
    public class GridRangeStyleCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridRangeStyleCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GridRangeStyleCollectionEnumerator(GridRangeStyleCollection collection)
        {
            _coll = collection;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = -1;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public GridRangeStyle Current
        {
            get
            {
                return _coll[_cursor];
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_next == -1 || _next >= _coll.Count)
            {
                return false;
            }

            _cursor = _next;

            _next++;
            if (_next >= _coll.Count)
            {
                _next = -1;
            }

            return _cursor != -1;
        }
        #endregion
    }
    
    #endregion

    #region GridRangeStyle

    /// <summary>
    /// RangeStyles are managed by the <see cref="GridRangeStyleCollection"/>.
    /// </summary>
    [TypeConverter(typeof(GridRangeStyleTypeConverter))]
    public class GridRangeStyle : IDisposable
    {
        GridStyleInfo style;
        GridRangeInfo range;
        internal GridRangeStyleCollection collection;

        /// <overload>
        /// Initializes a new empty <see cref="GridRangeStyle"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridRangeStyle"/>.
        /// </summary>
        public GridRangeStyle()
        {
            range = GridRangeInfo.Empty;
            this.style = new GridBaseStyleInfo(null, new GridStyleInfoStore());
            style.Changed += new StyleChangedEventHandler(style_Changed);
        }

        /// <summary>
        /// Initializes a new <see cref="GridRangeStyle"/>.
        /// </summary>
        /// <param name="rowIndex">The Row index.</param>
        /// <param name="colIndex">The Column index.</param>
        public GridRangeStyle(int rowIndex, int colIndex)
        {
            range = GridRangeInfo.Auto(rowIndex, colIndex);

            // GridBaseStyleInfo is derived from GridStyleInfo and has no default ctor.
            // This forces code serialization to generates statements like gridBaseStyle1.StyleInfo.AutoSize = True;
            // instead of creating a new instance of GridStyleInfo. 
            //
            // Works also well with XmlSerialization. XmlSerialization only checks the return type
            // of GetItem(int index) (which is GridStyleInfo) through reflection and also detects
            // that it can set a style object with SetItem(int index).
            this.style = new GridBaseStyleInfo(null, new GridStyleInfoStore());
            style.Changed += new StyleChangedEventHandler(style_Changed);
        }

        /// <summary>
        /// Initializes a new <see cref="GridRangeStyle"/>.
        /// </summary>
        /// <param name="rowIndex">The Row Index.</param>
        /// <param name="colIndex">The Column Index.</param>
        /// <param name="styleStore">Style store.</param>
        public GridRangeStyle(int rowIndex, int colIndex, GridStyleInfoStore styleStore)
        {
            range = GridRangeInfo.Auto(rowIndex, colIndex);
            this.style = new GridBaseStyleInfo(null, styleStore);
            style.Changed += new StyleChangedEventHandler(style_Changed);
        }

        /// <summary>
        /// Initializes a new <see cref="GridRangeStyle"/>.
        /// </summary>
        /// <param name="top">Top row index.</param>
        /// <param name="left">Left column index.</param>
        /// <param name="bottom">Bottom row index.</param>
        /// <param name="right">Right column index.</param>
        /// <param name="styleStore">Style store.</param>
        public GridRangeStyle(int top, int left, int bottom, int right, GridStyleInfoStore styleStore)
        {
            range = GridRangeInfo.Auto(top, left, bottom, right);
            this.style = new GridBaseStyleInfo(null, styleStore);
            style.Changed += new StyleChangedEventHandler(style_Changed);
        }

        /// <summary>
        /// Initializes a new <see cref="GridRangeStyle"/>.
        /// </summary>
        /// <param name="range">The cell range.</param>
        /// <param name="styleStore">The style store.</param>
        public GridRangeStyle(GridRangeInfo range, GridStyleInfoStore styleStore)
        {
            this.range = range;
            this.style = new GridBaseStyleInfo(null, styleStore);
            style.Changed += new StyleChangedEventHandler(style_Changed);
        }

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return range.ToString() + ": " + StyleInfo.ToString();
        }
        
        /// <summary>
        /// Gets or sets the cell information that should be applied to the range. 
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        public GridStyleInfo StyleInfo
        {
            get
            {
                if (style == null)
                {
                    style = new GridBaseStyleInfo(null, new GridStyleInfoStore());
                    style.Changed += new StyleChangedEventHandler(style_Changed);
                }

                return style;
            }

            set
            {
                StyleInfo.ModifyStyle(value, StyleModifyType.Copy);
            }
        }

        /// <summary>
        /// Gets or sets the range that the cell information should be applied to. 
        /// </summary>
        [TypeConverter(typeof(GridRangeInfoConverter))]
        [System.Xml.Serialization.XmlIgnore]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }

            set
            {
                range = value;
                if (collection != null)
                {
                    collection.WriteToData();
                }
            }
        }

        // Need to serialized as text for XML serialization to work properly.

        /// <internalonly/>
        /// <summary>Gets or sets RangeInfo. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [System.Xml.Serialization.XmlElement("Range")]
        public string RangeInfo
        {
            get
            {
                return Range.ToString();
            }

            set
            {
                Range = GridRangeInfo.Parse(value);
            }
        }

        #region IDisposable Members

        /// <override/>
        /// <summary>Releases all the resources used by this component.</summary>
        public void Dispose()
        {
            if (style != null)
            {
                style.Changed -= new StyleChangedEventHandler(style_Changed);
                this.style.Dispose();
            }

            this.style = null;
        }

        #endregion

        private void style_Changed(object sender, StyleChangedEventArgs e)
        {
            if (collection != null)
            {
                collection.WriteToData(this, true);
            }
        }
    }
    #endregion

    #region GridRangeStyleTypeConverter

    /// <summary>
    /// The type converter for <see cref="GridRangeStyle"/> objects. <see cref="GridRangeStyleTypeConverter"/> 
    /// is a <see cref="ExpandableObjectConverter"/>. It overrides the default behavior of the 
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class GridRangeStyleTypeConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridRangeStyleTypeConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type,
        /// using the specified context.
        /// </summary>       
        /// <param name="context">Format
        /// context. </param>
        /// <param name="destinationType">The
        /// type you want to convert to. </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <override/>
        /// <summary>
        /// Converts the given value object to the specified type, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">Format
        /// context. </param>
        /// <param name="culture">Current culture information. </param>
        /// <param name="value">The object to convert. </param>
        /// <param name="destinationType">The type to convert the
        /// value parameter to. </param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(InstanceDescriptor))
            {
                GridRangeStyle cell = (GridRangeStyle)value;
                Type type = typeof(GridRangeStyle);

                ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { });
                if (constructorInfo != null)
                {
                    return new InstanceDescriptor(constructorInfo, null, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <override/>
        /// <summary>
        /// A list of properties for the specified object type.
        /// </summary>
        /// <param name="context">Graphics context.</param>
        /// <param name="value">An object that specifies the type.</param>
        /// <param name="attributes">An array of System.Attribute objects that will be used as a filter.</param>
        /// <returns>A collection of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(value, attributes, false);

            string[] atts = new string[]
            {
                "Range",
                "StyleInfo",
            };

            return pds.Sort(atts);
        }
    }
    #endregion
}

