//-------------------------------------------------------------------------------------------------
// <copyright file="GridColumnSet.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    #region GridColumnSetDescriptorCollection
    /// <summary>
    /// An IComparer implementation that compares the <see cref="GridColumnSetDescriptor.Name"/> of
    /// two <see cref="GridColumnSetDescriptor"/> objects.
    /// </summary>
    public class GridColumnSetDescriptorNameComparer : IComparer
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridColumnSetDescriptorNameComparer()
            : base()
        {
        }

        #region IComparer Members

        /// <summary>
        /// Compares two <see cref="GridColumnSetDescriptor"/> objects.
        /// </summary>
        /// <param name="x">The first GridColumnSetDescriptor.</param>
        /// <param name="y">The second GridColumnSetDescriptor.</param>
        /// <returns>
        /// The result of string.Compare for the <see cref="GridColumnSetDescriptor.Name"/> of the
        /// two <see cref="GridColumnSetDescriptor"/> objects; 0 if both are the same.
        /// </returns>
        public int Compare(object x, object y)
        {
            GridColumnSetDescriptor c = (GridColumnSetDescriptor) x;
            GridColumnSetDescriptor d = (GridColumnSetDescriptor) y;
            return c.Name.CompareTo(d.Name);
        }

        #endregion
    }

    /// <summary>
    /// A collection of <see cref="GridColumnSetDescriptor"/> with <see cref="GridColumnSpanDescriptor"/> information
    /// about columns that can spread multiple grid rows or columns. <para/>
    /// An instance of this collection is returned by the <see cref="GridTableDescriptor.ColumnSets"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    public class GridColumnSetDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        internal ArrayList _inner;
        internal SortedList _sorted;
        internal GridTableDescriptor _tableDescriptor;
        internal int version;
        bool modified = false;
        bool readOnly = false;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static GridColumnSetDescriptorCollection Empty = new GridColumnSetDescriptorCollection(null);

        internal bool insideCollectionEditor = false;

        /// <overload>
        /// Initializes a new empty collection.
        /// </overload>
        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public GridColumnSetDescriptorCollection()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new empty collection and attaches it to a <see cref="TableDescriptor"/>.
        /// </summary>
        internal GridColumnSetDescriptorCollection(GridTableDescriptor tableDescriptor)
        {
            this._inner = new ArrayList();
            this._sorted = new SortedList(); ////;new GridColumnSetDescriptorNameComparer());
            _tableDescriptor = tableDescriptor;
        }

        internal GridColumnSetDescriptorCollection(GridTableDescriptor tableDescriptor, GridColumnSetDescriptor[] columnDescriptors)
            : this(tableDescriptor)
        {
            this.AddRange(columnDescriptors);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Reset()
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            _inner.Clear();
            if (_sorted != null)
            {
                _sorted.Clear();
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
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
            return String.Format("GridColumnSetDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        /// <summary>
        /// Gets / sets whether the collection is manipulated inside a collection editor.
        /// </summary>
        public bool InsideCollectionEditor
        {
            get
            {
                return insideCollectionEditor;
            }

            set
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this);
                }
#else
                ;
#endif
                if (insideCollectionEditor != value)
                {
                    insideCollectionEditor = value;
                }
            }
        }

        /// <summary>
        /// Gets the table descriptor this collection belongs to.
        /// </summary>
        public GridTableDescriptor TableDescriptor
        {
            get
            {
                return _tableDescriptor;
            }
        }

        internal void SetTableDescriptor(GridTableDescriptor tableDescriptor)
        {
            _tableDescriptor = tableDescriptor;
        }

        void IInsideCollectionEditorProperty.InitializeFrom(object other)
        {
            InitializeFrom((GridColumnSetDescriptorCollection) other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridColumnSetDescriptorCollection other)
        {
            int i;
            int count = Math.Min(Count, other.Count);

            _sorted = null;

            while (Count > other.Count)
            {
                RemoveAt(Count - 1);
            }

            for (i = 0; i < count; i++)
            {
                this[i].InitializeFrom(other[i]);
            }

            for (; i < other.Count; i++)
            {
                Add(other[i].Clone());
            }

            _sorted = new SortedList();
            foreach (GridColumnSetDescriptor cd in this)
            {
                _sorted.Add(cd.Name, cd);
            }
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="columnSetDescriptors">The array with elements that should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(GridColumnSetDescriptor[] columnSetDescriptors)
        {
            EnsureInitialized(false);
            int index = _inner.Count;
            this._inner.AddRange(columnSetDescriptors);
            for (int n = 0; n < columnSetDescriptors.Length; n++)
            {
                columnSetDescriptors[n].index = index++;
                columnSetDescriptors[n].SetCollection(this);
                if (_sorted != null)
                {
                    this._sorted.Add(columnSetDescriptors[n].Name, columnSetDescriptors[n]);
                }

                this.modified = true;
            }
        }
        #region Change Events
        
        /// <summary>
        /// Occurs after a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            version++;
            modified = true;
            if (!this.insideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, e.Item, e.Index, version);
                }
#else
                ;
#endif
                if (Changed != null)
                {
                    Changed(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanged(GridColumnSetDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                if (_sorted != null)
                {
                    int index = _sorted.IndexOfValue(column);
                    if (index != -1)
                    {
                        _sorted.RemoveAt(index);
                    }

                    _sorted.Add(column.Name, column);
                }
            }

            if (!this.InsideCollectionEditor)
            {
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, Find(column.Name), column, e.PropertyName, e));
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.insideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, e.Item, e.Index, version);
                }
#else
                ;
#endif
                if (Changing != null)
                {
                    Changing(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanging(GridColumnSetDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.insideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif

                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, Find(column.Name), column, e.PropertyName, e));
            }
        }
        #endregion
        
        void EnsureInitialized(bool populate)
        {
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public GridColumnSetDescriptorCollection Clone()
        {
            int count = Count;
            GridColumnSetDescriptor[] columnDescriptors = new GridColumnSetDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                columnDescriptors[n] = this[n].Clone();
            }

            GridColumnSetDescriptorCollection c = new GridColumnSetDescriptorCollection(_tableDescriptor, columnDescriptors);
            c.modified = modified;
            c.version = version+1000;
            return c;
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
            else if (!(obj is GridColumnSetDescriptorCollection))
            {
                return false;
            }

            return Equals((GridColumnSetDescriptorCollection) obj);
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
        /// The version number of this collection. The version is increased each time the
        /// collection or an element within the collection is modified.
        /// </summary>
        public int Version
        {
            get
            {
                this.EnsureInitialized(true);
                return version;
            }
////            set
////            {
////                version = value;
////            }
        }

        /// <summary>
        /// Gets / sets whether the collection is modified from its default state.
        /// </summary>
        public bool IsModified
        {
            get
            {
                return modified;
            }
        }
        
        bool Equals(GridColumnSetDescriptorCollection other)
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
        public GridColumnSetDescriptor this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (GridColumnSetDescriptor) _inner[index];
            }

            set
            {
                if (readOnly)
                {
                    throw new InvalidOperationException("Collection is Read-only.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                EnsureInitialized(false);
                if (_inner[index] != value)
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    if (_sorted != null)
                    {
                        _sorted.Remove(this[index].Name);
                    }

                    _inner[index] = value;
                    value.index = index;
                    value.SetCollection(this);
                    if (_sorted != null)
                    {
                        _sorted.Add(value.Name, value);
                    }

                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
#if ASPNET
        public GridColumnSetDescriptor GetColumnSetDescriptor(string name){return this[name];}
        public void SetColumnSetDescriptor(string name, GridColumnSetDescriptor value){this[name] = value;}

        // Designer ser. doesn't like overloaded accessors.
        internal
#else
        public
#endif
            GridColumnSetDescriptor this[string name]
        {
            get
            {
                EnsureInitialized(true);
                int index = Find(name);
                if (index == -1)
                {
                    if (!readOnly)
                    {
                        return this[Add(new GridColumnSetDescriptor(name))];
                    }
                    else
                    {
                        return null;
                    }
                }

                return (GridColumnSetDescriptor) _inner[index];
            }

            set
            {
                if (readOnly)
                {
                    throw new InvalidOperationException("Collection is Read-only.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                EnsureInitialized(false);
                int index = Find(name);
                value.Name = name;
                if (index == -1)
                {
                    Add(value);
                }
                else
                {
                    this[index] = value;
                }
            }
        }

        internal int Find(string name)
        {
            if (_sorted != null)
            {
                    GridColumnSetDescriptor cd = (GridColumnSetDescriptor) _sorted[name];
                    if (cd != null)
                    {
                        return cd.index;
                    }
            }

            return -1;
        }

        internal int Find(PropertyDescriptor pd)
        {
            return Find(pd.Name);
        }

        internal bool ContainsColumn(string columnName)
        {
            foreach (GridColumnSetDescriptor gcsd in this)
            {
                if (gcsd.ColumnSpans.Find(columnName) != -1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(GridColumnSetDescriptor value)
        {
            if (value == null)
            {
                return false;
            }

            EnsureInitialized(true);
            return _sorted != null && _sorted.Contains(value.Name);
        }

        /// <summary>
        /// Determines if an element with the specified name belongs to this collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(string name)
        {
            EnsureInitialized(true);
            return _sorted != null && _sorted.Contains(name);
        }
        
        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(GridColumnSetDescriptor value)
        {
            EnsureInitialized(true);
            if (!Contains(value))
            {
                return -1;
            }

            return this[value.Name].index;
        }

        /// <summary>
        /// Searches for the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>The zero-based index of the occurrence of the element with matching name within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(string name)
        {
            EnsureInitialized(true);
            return Find(name);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(GridColumnSetDescriptor[] array, int index)
        {
            int n = 0;
            foreach (GridColumnSetDescriptor item in this)
            {
                array[index+n] = item;
                n++;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        GridColumnSetDescriptorCollection SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading of the data in the collection.
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public GridColumnSetDescriptorCollectionEnumerator GetEnumerator()
        {
            return new GridColumnSetDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridColumnSetDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
            this.EnsureInitialized(true);
            _inner.Insert(index, value);
            if (_sorted != null)
            {
                _sorted.Add(value.Name, value);
            }

            value.SetCollection(this);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
        }

        /// <summary>
        /// Removes the specified descriptor element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(GridColumnSetDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                return;
            }

            this.EnsureInitialized(true);
            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            if (_sorted != null)
            {
                _sorted.Remove(value.Name);
            }

            _inner.Remove(value);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(GridColumnSetDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            this.EnsureInitialized(false);
            if (value.Name == null || value.Name.Length == 0)
            {
                SuggestName(value);
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            int index = _inner.Add(value);
            value.index = index;
            if (_sorted != null)
            {
                _sorted.Add(value.Name, value);
            }

            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));

            return index;
        }

        void SuggestName(GridColumnSetDescriptor value)
        {
            int n = 1;
            foreach (GridColumnSetDescriptor col in this)
            {
                if (col.Name.StartsWith("ColumnSet "))
                {
                    double d;
                    if (double.TryParse(col.Name.Substring("ColumnSet ".Length), System.Globalization.NumberStyles.Number, null, out d))
                    {
                        n = (int)d + 1;
                    }
                }
            }

            value.Name = "ColumnSet " + n.ToString();
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="name">The name of the element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name)
        {
            return Add(new GridColumnSetDescriptor(name));
        }

        /// <summary>
        /// Removes the specified descriptor element with the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the element to remove from the collection. If no element with that name is found
        /// in the collection, the method will do nothing.</param>
        public void Remove(string name)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            this.EnsureInitialized(true);
            int index = Find(name);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            this.EnsureInitialized(true);
            GridColumnSetDescriptor value = (GridColumnSetDescriptor) _inner[index];
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            if (_sorted != null)
            {
                _sorted.Remove(value.Name);
            }

            _inner.RemoveAt(index);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Disposes of the object and collection items.
        /// </summary>
        public void Dispose()
        {
            foreach (DescriptorBase db in _inner)
            {
                db.Dispose();
            }

            _inner.Clear();
            if (_sorted != null)
            {
                _sorted.Clear();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            this.EnsureInitialized(false);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            _inner.Clear();
            if (_sorted != null)
            {
                _sorted.Clear();
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
        }

        /// <summary>
        /// Determines if the collection is Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return readOnly;
            }
        }

        /// <summary>
        /// Returns normally False since this collection has no fixed size. Only when it is Read-only
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
        /// Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. The property also
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </summary>
        public int Count
        {
            get
            {
                this.EnsureInitialized(true);
                return _inner.Count;
            }
        }

        #region ICloneable Private Members
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region IList Private Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (GridColumnSetDescriptor) value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridColumnSetDescriptor) value);
        }

        void IList.Remove(object value)
        {
            Remove((GridColumnSetDescriptor) value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridColumnSetDescriptor) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridColumnSetDescriptor) value);
        }

        int IList.Add(object value)
        {
            return Add((GridColumnSetDescriptor) value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridColumnSetDescriptor[]) array, index);
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

        #region ICustomTypeDescriptor
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor) this).GetProperties(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            ArrayList pds = new ArrayList();
            Attribute[] att = new Attribute[] 
            {
                                                  new BrowsableAttribute(true),
                                                  new System.Xml.Serialization.XmlIgnoreAttribute(),
                                                  new RefreshPropertiesAttribute(RefreshProperties.All),
                                                  new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden),
                                                  new CategoryAttribute("Items")
                                              };

            ArrayList names = new ArrayList();
            foreach (DescriptorBase descriptor in this)
            {
                pds.Add(new DescriptorBasePropertyDescriptor(descriptor.GetName(), descriptor, att, GetType()));
                names.Add(descriptor.GetName());
            }

            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection((PropertyDescriptor[]) pds.ToArray(typeof(PropertyDescriptor)));
            return pdc.Sort((string[]) names.ToArray(typeof(string)));
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="GridColumnSetDescriptor"/> elements of a <see cref="GridColumnSetDescriptorCollection"/>.
    /// </summary>
    public class GridColumnSetDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridColumnSetDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GridColumnSetDescriptorCollectionEnumerator(GridColumnSetDescriptorCollection collection)
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
        public GridColumnSetDescriptor Current
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
            if (_next == -1)
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

    #region TypeConverter
    /// <summary>
    /// The type converter for <see cref="GridColumnSetDescriptor"/> objects. <see cref="GridColumnSetDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class GridColumnSetDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridColumnSetDescriptorTypeConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Determines whether the current object can be converted to the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">The type you want to convert the object to.</param>
        /// <returns>True if this conversion is supported.</returns>
        public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                return true;
            }
            else if (destinationType == typeof(string))
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
        /// Converts the given value to the type specified.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">Current culture information used for conversion.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destinationType">Type to convert to.</param>
        /// <returns>Converted object.</returns>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(InstanceDescriptor))
            {
                ////System.Reflection.ConstructorInfo constructorInfo = value.GetType().GetConstructor(new Type[]{});

                GridColumnSetDescriptor columnSet = (GridColumnSetDescriptor) value;
                Type type = typeof(GridColumnSetDescriptor);
                return new InstanceDescriptor(type.GetConstructor(new Type[0]), null, false);
            }
            else if (destinationType == typeof(string))
            {
                return String.Format(((GridColumnSetDescriptor)value).Name);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } //// end of method ConvertTo

        /// <override/>
        /// <summary>
        /// Gets a collection of properties for the tyep specified.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Value that specifies the type.</param>
        /// <param name="attributes">An array of System.Attribute objects that will be used as a filter.</param>
        /// <returns>A list of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "Width",
                "ColumnSpans",
            };

            return pds.Sort(atts);
        }
    }
    #endregion

    #region GridColumnSetDescriptor

    /// <summary>
    /// <see cref="GridColumnSetDescriptor"/> provides information
    /// about columns that can spread multiple grid rows or columns. <para/>
    /// ColumnSets are managed by the <see cref="GridColumnSetDescriptorCollection"/> that
    /// is returned by the <see cref="GridTableDescriptor.ColumnSets"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(GridColumnSetDescriptorTypeConverter))]
    public class GridColumnSetDescriptor : DescriptorBase, ICloneable
    {
        string name;
        internal int index;
        GridColumnSpanDescriptorCollection _columnSpans;
        GridColumnDescriptor[] widthColumns;
        int[] widthFactor = null;
        
        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        #region ctor

        /// <overload>
        /// Initializes a new column set.
        /// </overload>
        /// <summary>
        /// Initializes a new empty column set.
        /// </summary>
        public GridColumnSetDescriptor()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new column set with a name.
        /// </summary>
        /// <param name="name">Descriptor name.</param>
        public GridColumnSetDescriptor(string name)
        {
            this.name = name;
        }

        #endregion

        /// <summary>
        /// Determine number of columns.
        /// </summary>
        /// <returns>Column count.</returns>
        public int GetColCount()
        {
            // Determine number of columns
            int colCount = 0;
            foreach (GridColumnSpanDescriptor columnSpan in ColumnSpans)
            {
                colCount = Math.Max(colCount, columnSpan.Range.Right + 1);
            }

            return colCount;
        }

        /// <summary>
        /// Returns an array of columns that affect the width of this column set.
        /// </summary>
        /// <param name="factor">An array in the same order as the returned array with
        /// width factors indicating the number of grid columns spanned by a column descriptor.</param>
        /// <returns>An array of columns that affect the width of this column set. Each column
        /// will also have a width factor returned through the <paramref name="factor"/> array.</returns>
        public GridColumnDescriptor[] GetWidthColumns(out int[] factor)
        {
            if (widthColumns != null)
            {
                factor = widthFactor;
                return widthColumns;
            }

            // Determine number of columns
            int colCount = 0;
            foreach (GridColumnSpanDescriptor columnSpan in ColumnSpans)
            {
                colCount = Math.Max(colCount, columnSpan.Range.Right + 1);
            }

            widthColumns = new GridColumnDescriptor[colCount];
            widthFactor = new int[colCount];

            // Check columns that do not span across columns
            // First column in colletion have higher precedence
            foreach (GridColumnSpanDescriptor columnSpan in ColumnSpans)
            {
                if (columnSpan.Range.Right == columnSpan.Range.Left
                    && widthColumns[columnSpan.Range.Left] == null)
                {
                    int n = TableDescriptor.Columns.IndexOf(columnSpan.Name);
                    if (n != -1)
                    {
                        widthColumns[columnSpan.Range.Left] = TableDescriptor.Columns[n];
                        widthFactor[columnSpan.Range.Left] = 1;
                    }
                }
            }

            // Now check columns that do span across columns
            // First column in colletion have higher precedence
            foreach (GridColumnSpanDescriptor columnSpan in ColumnSpans)
            {
                if (columnSpan.Range.Right != columnSpan.Range.Left)
                {
                    int n = TableDescriptor.Columns.IndexOf(columnSpan.Name);
                    if (n != -1)
                    {
                        for (int c = columnSpan.Range.Left; c <= columnSpan.Range.Right; c++)
                        {
                            if (widthColumns[c] == null)
                            {
                                widthColumns[c] = TableDescriptor.Columns[n];
                                widthFactor[c] = columnSpan.Range.Width;
                            }
                        }
                    }
                }
            }

            factor = widthFactor;
            return widthColumns;
        }

        /// <override/>
        /// <summary>Returns the descriptor name.</summary>
        /// <returns>Descriptor name.</returns>
        public override string GetName()
        {
            return Name;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                if (_columnSpans != null)
                {
                    _columnSpans.Changed -= new ListPropertyChangedEventHandler(columnSpans_Changed);
                    _columnSpans.Changing -= new ListPropertyChangedEventHandler(_columnSpans_Changing);
                    _columnSpans.Dispose();
                    _columnSpans = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// The collection of columns spans that one or multiple column descriptors with a range of cells covered
        /// by cells of a column.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [ListBindableAttribute(false)]
        [Description("The collection of columns spans that one or multiple column descriptors with a range of cells covered by cells of a column."),
        Category("TableDescriptors")]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
#if ASPNET
        [System.Web.UI.PersistenceMode(System.Web.UI.PersistenceMode.InnerProperty)]
#endif
        public GridColumnSpanDescriptorCollection ColumnSpans
        {
            get
            {
                if (_columnSpans == null)
                {
                    _columnSpans = new GridColumnSpanDescriptorCollection(this);
                    _columnSpans.Changed += new ListPropertyChangedEventHandler(columnSpans_Changed);
                    _columnSpans.Changing += new ListPropertyChangedEventHandler(_columnSpans_Changing);
                }

                return _columnSpans;
            }
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridColumnSetDescriptor other)
        {
            this.Name = other.Name;
            this.ColumnSpans.InitializeFrom(other.ColumnSpans);
        }

        #region ParentCollection

        GridColumnSetDescriptorCollection collection;

        internal void SetCollection(GridColumnSetDescriptorCollection collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridColumnSetDescriptorCollection Collection
        {
            get
            {
                return collection;
            }
        }

        /// <summary>
        /// The TableDescriptor that this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTableDescriptor TableDescriptor
        {
            get
            {
                return collection == null ? null : collection.TableDescriptor;
            }
        }

        #endregion

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="PropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanging(DescriptorPropertyChangedEventArgs e)
        {
            if (this.Disposing)
            {
                return;
            }

            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }

            if (collection != null)
            {
                collection.RaisePropertyItemChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanged(DescriptorPropertyChangedEventArgs e)
        {
            if (this.Disposing)
            {
                return;
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            if (collection != null)
            {
                collection.RaisePropertyItemChanged(this, e);
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public GridColumnSetDescriptor Clone()
        {
            GridColumnSetDescriptor rd = new GridColumnSetDescriptor();
            rd.index = index;
            rd.name = name;

            foreach (GridColumnSpanDescriptor cd in ColumnSpans)
            {
                rd.ColumnSpans.Add(cd.Clone());
            }

            rd.collection = collection;
            return rd;
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
            else if (!(obj is GridColumnSetDescriptor))
            {
                return false;
            }

            return Equals((GridColumnSetDescriptor) obj);
        }

        bool Equals(GridColumnSetDescriptor other)
        {
            return other.name == name
               ////&& other.width == width
                && other.ColumnSpans.Equals(ColumnSpans);
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
        /// The name of the column set.
        /// </summary>
        [Description("The name of the column set."),
        Category("TableDescriptors")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    string str = name;
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    name = value;
                    try
                    {
                        OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                    }
                    catch
                    {
                        name = str;
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the column sets name was modified (not empty).
        /// </summary>
        /// <returns>True if name is modified.</returns>
        public bool ShouldSerializeName()
        {
            return Name != string.Empty;
        }

        /// <summary>
        /// Resets the column set's name to an empty string.
        /// </summary>
        public void ResetName()
        {
            Name = string.Empty;
        }

        private void columnSpans_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            widthColumns = null;
            OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ColumnSpans", e));
        }

        private void _columnSpans_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ColumnSpans", e));
        }
    }

    #endregion
}

