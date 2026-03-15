//-------------------------------------------------------------------------------------------------
// <copyright file="GridRowHeight.cs" company="syncfusion">
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
    class GridRowHeightComparer : object, IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            GridRowHeight v1 = (GridRowHeight)x;
            GridRowHeight v2 = (GridRowHeight)y;
            return v1.RowIndex - v2.RowIndex;
        }

        #endregion
    }

    #region GridRowHeightCollection
    /// <summary>
    /// A collection of <see cref="GridRowHeight"/> items with information about row height.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(CollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class GridRowHeightCollection : IDisposable, IList
    {
        internal ArrayList _inner = new ArrayList();
        GridModelRowColSizeIndexer rowSizeIndexer;
        bool modified = false;
        bool readOnly = false;
        IComparer comparer = new GridRowHeightComparer();

        event ListPropertyChangedEventHandler Changing;
        event ListPropertyChangedEventHandler Changed;

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return String.Format("GridRowHeightCollection: Count {0}", Count);
        }

        /// <overload>
        /// Initializes a new empty collection.
        /// </overload>
        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        internal GridRowHeightCollection()
        {
        }

        internal GridRowHeightCollection(GridModelRowColSizeIndexer rowSizeIndexer)
        {
            if (rowSizeIndexer != null)
            {
                this.rowSizeIndexer = rowSizeIndexer;
                GridRowColSizeDictionary rowSizeDictionary = (GridRowColSizeDictionary)rowSizeIndexer.Dictionary;
                GridIndexDictionary dict = rowSizeDictionary.InnerDict;

                foreach (DictionaryEntry entry in dict.GetHashTable())
                {
                    int rowIndex = (int)entry.Key;
                    int height = (int)entry.Value;
                    GridRowHeight g = new GridRowHeight(rowIndex, height);
                    g.collection = this;
                    this._inner.Add(g);
                }

                _inner.Sort(comparer);
            }
        }

        internal void WriteToDict()
        {
            WriteToDict(rowSizeIndexer);
        }

        internal void WriteToDict(GridModelRowColSizeIndexer rowSizeIndexer)
        {
            if (rowSizeIndexer == null)
            {
                return;
            }

            this.rowSizeIndexer = rowSizeIndexer;
            GridRowColSizeDictionary rowSizeDictionary = (GridRowColSizeDictionary)rowSizeIndexer.Dictionary;
            GridIndexDictionary dict = rowSizeDictionary.InnerDict;

            rowSizeIndexer.Model.BeginUpdate(BeginUpdateOptions.None);
            dict.Clear();
            foreach (GridRowHeight rowHeight in this)
            {
                dict[rowHeight.RowIndex] = rowHeight.Height;
            }

            rowSizeIndexer.Model.EndUpdate(false);
            if (rowSizeIndexer.Model.ActiveGridView != null)
            {
                rowSizeIndexer.Model.ActiveGridView.Invalidate();
            }

            rowSizeDictionary.Modified = true;
        }

        internal void WriteToDict(GridRowHeight rowHeight)
        {
            if (rowSizeIndexer == null)
            {
                return;
            }

            GridRowColSizeDictionary rowSizeDictionary = (GridRowColSizeDictionary)rowSizeIndexer.Dictionary;
            GridIndexDictionary dict = rowSizeDictionary.InnerDict;
            dict[rowHeight.RowIndex] = rowHeight.Height;
            rowSizeDictionary.Modified = true;
            if (rowSizeIndexer.Model.ActiveGridView != null)
            {
                rowSizeIndexer.Model.ActiveGridView.Invalidate();
            }
        }

        internal void WriteToDict(GridRowHeight[] rowHeights)
        {
            if (rowSizeIndexer == null)
            {
                return;
            }

            GridRowColSizeDictionary rowSizeDictionary = (GridRowColSizeDictionary)rowSizeIndexer.Dictionary;
            GridIndexDictionary dict = rowSizeDictionary.InnerDict;
            foreach (GridRowHeight rowHeight in rowHeights)
            {
                dict[rowHeight.RowIndex] = rowHeight.Height;
            }

            rowSizeDictionary.Modified = true;
            if (rowSizeIndexer.Model.ActiveGridView != null)
            {
                rowSizeIndexer.Model.ActiveGridView.Invalidate();
            }
        }

        /// <summary>
        /// Adds multiple GridRowHeight items.
        /// </summary>
        /// <param name="rowHeights">The Array with elements that should be added to the end of the collection. 
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic). 
        /// </param>
        public void AddRange(GridRowHeight[] rowHeights)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, 0, null, null));
            modified = true;
            int index = this.Count;

            if (this.rowSizeIndexer != null && this.rowSizeIndexer.model.Initializing)
            {
                this._inner.Clear();
            }

            this._inner.AddRange(rowHeights);
            _inner.Sort(comparer);
            for (int n = 0; n < rowHeights.Length; n++)
            {
                rowHeights[n].collection = this;
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index++, rowHeights[n], null));
            }

            if (this.rowSizeIndexer != null)
            {
                this.WriteToDict();
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
            else if (!(obj is GridRowHeightCollection))
            {
                return false;
            }

            return InternalEquals((GridRowHeightCollection)obj);
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
        /// Compares each element with the element of another collection.
        /// </summary>
        /// <param name="other">The collection to compare to.</param>
        /// <returns>True if all elements are equal and in the same order; False otherwise.</returns>
        bool InternalEquals(GridRowHeightCollection other)
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
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public GridRowHeight this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (GridRowHeight)_inner[index];
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
        public bool Contains(GridRowHeight value)
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
        public int IndexOf(GridRowHeight value)
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
        public void CopyTo(GridRowHeight[] array, int index)
        {
            int n = 0;
            foreach (GridRowHeight item in this)
            {
                array[index + n] = item;
                n++;
            }
        }

        /// <summary>
        /// Gets SyncRoot. Not supported.
        /// </summary>
        GridRowHeightCollection SyncRoot
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
        public GridRowHeightCollectionEnumerator GetEnumerator()
        {
            return new GridRowHeightCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a GridRowHeight element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridRowHeight value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
            _inner.Insert(index, value);
            value.collection = this;
            this.WriteToDict();
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
        }

        /// <summary>
        /// Removes the specified GridRowHeight element from the collection.
        /// </summary>
        /// <param name="value">The GridRowHeight to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(GridRowHeight value)
        {
            if (value == null)
            {
                return;
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, -1, value, null));
            _inner.Remove(value);
            this.WriteToDict();
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, -1, value, null));
        }

        /// <summary>
        /// Adds an object to the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The new count.</returns>
        public int Add(GridRowHeight value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));

            if (value.RowIndex == -1)
            {
                if (Count > 0)
                {
                    value.RowIndex = this[Count - 1].RowIndex + 1;
                }
                else
                {
                    value.RowIndex = 1;
                }

                value.Height = this.rowSizeIndexer[value.RowIndex];
            }

            int index = _inner.Add(value);
            value.collection = this;
            WriteToDict(value);
            _inner.Sort(comparer);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            return Count;
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            GridRowHeight value = (GridRowHeight)_inner[index];
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            _inner.RemoveAt(index);
            this.WriteToDict();
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Disposes the object and collection items.
        /// </summary>
        public void Dispose()
        {
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
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }

            this.modified = true;
            this.WriteToDict();
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
        /// Gets a value indicating whether this collection is fixed size. Normally False since this collection has no fixed size. Only when it is Read-only 
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
                this[index] = (GridRowHeight)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridRowHeight)value);
        }

        void IList.Remove(object value)
        {
            Remove((GridRowHeight)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridRowHeight)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridRowHeight)value);
        }

        int IList.Add(object value)
        {
            return Add((GridRowHeight)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridRowHeight[])array, index);
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
    /// Enumerator class for <see cref="GridRowHeight"/> elements of a <see cref="GridRowHeightCollection"/>.
    /// </summary>
    public class GridRowHeightCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridRowHeightCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GridRowHeightCollectionEnumerator(GridRowHeightCollection collection)
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
        public GridRowHeight Current
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

    #region GridRowHeight

    /// <summary>
    /// RowHeights are managed by the <see cref="GridRowHeightCollection"/>.
    /// </summary>
    [TypeConverter(typeof(GridRowHeightTypeConverter))]
    public class GridRowHeight
    {
        int height;
        int rowIndex = -1;
        internal GridRowHeightCollection collection;

        /// <overload>
        /// Initializes a new empty <see cref="GridRowHeight"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridRowHeight"/>.
        /// </summary>
        public GridRowHeight()
        {
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
            return "RowHeight(" + rowIndex.ToString() + ") = " + height.ToString();
        }

        /// <summary>
        /// Initializes a new <see cref="GridRowHeight"/>.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        public GridRowHeight(int rowIndex)
        {
            this.rowIndex = rowIndex;
            this.height = -1;
        }

        /// <summary>
        /// Initializes a new <see cref="GridRowHeight"/>.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="height">Row height.</param>
        public GridRowHeight(int rowIndex, int height)
        {
            this.rowIndex = rowIndex;
            this.height = height;
        }

        /// <summary>
        /// Gets or sets the row height.
        /// </summary>
        [DefaultValue(-1)]
        public int Height
        {
            get
            {
                return height;
            }

            set
            {
                if (height != value)
                {
                    height = value;
                    if (collection != null)
                    {
                        collection.WriteToDict();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the RowIndex.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }

            set
            {
                if (rowIndex != value)
                {
                    rowIndex = value;
                    if (collection != null)
                    {
                        collection.WriteToDict();
                    }
                }
            }
        }
    }
    #endregion

    #region GridRowHeightTypeConverter

    /// <summary>
    /// The type converter for <see cref="GridRowHeight"/> objects. <see cref="GridRowHeightTypeConverter"/> 
    /// is a <see cref="ExpandableObjectConverter"/>. It overrides the default behavior of the 
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class GridRowHeightTypeConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridRowHeightTypeConverter()
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
        /// An object that represents the converted value.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(InstanceDescriptor))
            {
                GridRowHeight gridRowHeight = (GridRowHeight)value;
                Type type = typeof(GridRowHeight);

                ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { typeof(int), typeof(int) });
                if (constructorInfo != null)
                {
                    return new InstanceDescriptor(constructorInfo, new object[] { gridRowHeight.RowIndex, gridRowHeight.Height }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <override/>
        /// <summary>
        /// A collection of properties for the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">The object specifying the type.</param>
        /// <param name="attributes">An array of System.Attribute objects that will be used as a filter.</param>
        /// <returns>A list of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value, attributes, false);

            string[] atts = new string[]
            {
                "RowIndex",
                "Height",
            };

            return pds.Sort(atts);
        }
    }
    #endregion
}

