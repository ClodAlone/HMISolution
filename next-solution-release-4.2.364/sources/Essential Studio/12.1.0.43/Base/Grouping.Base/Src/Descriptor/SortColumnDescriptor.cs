//-------------------------------------------------------------------------------------------------
// <copyright file="SortColumnDescriptor.cs" company="syncfusion">
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
using System.Data;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping.Internals;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Implement this interface if you want to implement a custom categorizer.
    /// </summary>
    public interface IGroupByColumnCategorizer
    {
        /// <summary>
        /// Return a key for the specified column and record.
        /// </summary>
        /// <param name="column">The column descriptor.</param>
        /// <param name="isForeignKey">True if this is a foreign key field.</param>
        /// <param name="record">The record to be evaluated.</param>
        /// <returns>A key identifying the category this record belongs to.</returns>
        /// <remarks>
        /// When categorizing records, all records are first sorted and then looped through
        /// in the order they were sorted. Only neighboring records in that sort order
        /// can have the same category key.
        /// </remarks>
        object GetGroupByCategoryKey(SortColumnDescriptor column, bool isForeignKey, Record record);
        
        /// <summary>
        /// Determines if this record belongs to the same category as the previous record.
        /// </summary>
        /// <param name="column">The column descriptor.</param>
        /// <param name="isForeignKey">True if this is a foreign key field.</param>       
        /// <param name="category">The category of the previous record.</param>
        /// <param name="record">The record to be evaluated.</param>
        /// <returns>True if this record belongs to the same category; False otherwise.</returns>
        int CompareCategoryKey(SortColumnDescriptor column, bool isForeignKey, object category, Record record);
    }

    /// <summary>
    /// A collection of <see cref="SortColumnDescriptor"/> descriptors that define the sort order or grouping of a table.
    /// An instance of this collection is returned by the <see cref="TableDescriptor.SortedColumns"/> or <see cref="TableDescriptor.GroupedColumns"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class SortColumnDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        ArrayList inner = new ArrayList();
        internal int version;
        internal TableDescriptor tableDescriptor;
        internal bool insideCollectionEditor = false;
        IComparer groupedColumnsComparer;
        SortColumnDescriptorCollection copy;
        bool inInitializeFrom = false;
        bool raiseChangeEvents = true;
        bool modified = false;
        bool inReset = false;
        bool shouldPopulate = true;

        bool disableShouldPopulate = false;

        /// <summary>
        /// Gets or sets whether collection should check for changes
        /// in engine schema or underlying datasource schema when EnsureInitialized gets called.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShouldPopulate
        {
            get
            {
                return shouldPopulate;
            }

            set
            {
                shouldPopulate = value;
            }
        }

        /// <summary>
        /// When called the ShouldPopulate property will be set true temporarily until
        /// the next EnsureInitialized call and then be reset again to optimize subsequent lookups.
        /// The Engine calls this method when schema changes occured (PropertyChanged was raised).
        /// </summary>
        public void EnableOneTimePopulate()
        {
            this.disableShouldPopulate = true;
            if (!this.Engine.InInitializeFrom)
            {
                this.shouldPopulate = true;
            }
        }

        Engine Engine
        {
            get
            {
                return this.tableDescriptor != null ? this.tableDescriptor.Engine : null;
            }
        }

        /// <summary>
        /// Occurs after a property in a nested element or the collection was changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;

        /// <summary>
        /// Resets the collection to its default state. If the collection is bound to a <see cref="TableDescriptor"/>,
        /// the collection will autopopulate itself the next time an item inside the collection is accessed.
        /// </summary>
        public void Reset()
        {
            if (this.modified || this.inner.Count > 0)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inReset = true;
                modified = false;
                version++;
                inner.Clear();
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }

            inReset = false;
            this.modified = false;
        }

        /// <summary>
        /// Resets the collection to its default state, autopopulates the collection, and marks the collection
        /// as modified. Call this method if you want to load the default items for the collection and then
        /// modify them (e.g. remove members from the auto-populated list).
        /// </summary>
        public void LoadDefault()
        {
            if (this.IsModified)
            {
                Reset();
            }

            this.EnsureFieldDescriptors();
            this.modified = true;
        }

        /// <summary>
        /// Determines if the collection was modified from its default state.
        /// </summary>
        public bool IsModified
        {
            get
            {
                return modified;
            }
        }
        
        internal SortColumnDescriptorCollection GetShadowedCopy()
        {
            if (copy == null || copy.version != version)
            {
                copy = this.Clone();
                copy.version = version;
            }

            return copy;
        }

        internal IComparer GetGroupedColumnsComparer()
        {
            if (groupedColumnsComparer == null)
            {
                groupedColumnsComparer = new GroupedColumnsComparer(this);
            }

            return groupedColumnsComparer;
        }

        /// <summary>Returns a string representation of the current object.</summary>
        /// <returns>A string holding the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}: Count {1}", GetType().Name, inner.Count);
            foreach (SortColumnDescriptor sd in inner)
            {
                sb.AppendFormat(" {0} ", sd.Name);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns an array of SortColumnDescriptor objects where elements reference items in this collection. (Elements are not copied).
        /// </summary>
        /// <returns>An array of SortColumnDescriptor objects.</returns>
        public SortColumnDescriptor[] ToArray()
        {
            SortColumnDescriptor[] columnDescriptors = new SortColumnDescriptor[Count];
            for (int n = 0; n < columnDescriptors.Length; n++)
            {
                columnDescriptors[n] = (SortColumnDescriptor)inner[n];
            }

            return columnDescriptors;
        }

        SortColumnDescriptor[] CloneToArray()
        {
            SortColumnDescriptor[] columnDescriptors = new SortColumnDescriptor[Count];
            for (int n = 0; n < columnDescriptors.Length; n++)
            {
                columnDescriptors[n] = ((SortColumnDescriptor)inner[n]).Clone();
            }

            return columnDescriptors;
        }

        static SortColumnDescriptor[] CloneArray(SortColumnDescriptor[] src)
        {
            SortColumnDescriptor[] columnDescriptors = new SortColumnDescriptor[src.Length];
            for (int n = 0; n < columnDescriptors.Length; n++)
            {
                columnDescriptors[n] = src[n].Clone();
            }

            return columnDescriptors;
        }

        /// <summary>
        /// Ensure type correctness when a new element is added to the collection.
        /// </summary>
        /// <param name="obj">The newly added object.</param>
        protected virtual void CheckType(object obj)
        {
            if (obj != null && !(obj is SortColumnDescriptor))
            {
                throw new ArgumentException("Wrong type");
            }
        }

        internal int fieldsVersion = -1;

        internal bool isPrimaryKeyColumns = false;
        internal bool isRelationChildColumns = false;
        int relationVersion = -1;

        bool inEnsureFieldDescriptors = false;

        internal void EnsureFieldDescriptors()
        {
            if (inInitializeFrom || tableDescriptor == null || !shouldPopulate)
            {
                return;
            }

            if (disableShouldPopulate)
            {
                shouldPopulate = false;
            }

            inEnsureFieldDescriptors = true;
            try
            {
                if (!modified && isPrimaryKeyColumns
                    && tableDescriptor.ParentRelation != null
                    && tableDescriptor.ParentRelation.RelationKind == RelationKind.ForeignKeyReference)
                {
                    if (CheckOutOfDate()
                        || (tableDescriptor != null && fieldsVersion != this.tableDescriptor.Fields.Version)
                        || relationVersion != this.tableDescriptor.ParentRelation.Collection.Version)
                    {
                        // Auto-poulate PrimaryKeyColumns from ParentRelation.RelationKeys
                        this.inner.Clear();
                        foreach (RelationKeyDescriptor rdKey in this.tableDescriptor.ParentRelation.RelationKeys)
                        {
                            this.inner.Add(new SortColumnDescriptor(rdKey.ChildKeyFieldName));
                        }

                        foreach (SortColumnDescriptor cd in this.inner)
                        {
                            cd.SetCollection(this);
                            cd.InitFieldDescriptor(tableDescriptor);
                        }

                        fieldsVersion = this.tableDescriptor.Fields.Version;
                        relationVersion = this.tableDescriptor.ParentRelation.Collection.Version;
                        modified = false;
                    }
                }
                else if (!modified && isPrimaryKeyColumns)
                {
                    if (CheckOutOfDate()
                        || (tableDescriptor != null && fieldsVersion != this.tableDescriptor.Fields.Version))
                    {
                        DataTable dataTable = ListUtil.GetDataTable(tableDescriptor.GetList());
                        if (dataTable != null)
                        {
                            foreach (Constraint constraint in dataTable.Constraints)
                            {
                                if (constraint is UniqueConstraint)
                                {
                                    UniqueConstraint uniqueKeyConstraint = (UniqueConstraint)constraint;

                                    // Auto-poulate PrimaryKeyColumns from ParentRelation.RelationKeys
                                    this.inner.Clear();
                                    foreach (DataColumn dataColumn in uniqueKeyConstraint.Columns)
                                    {
                                        this.inner.Add(new SortColumnDescriptor(dataColumn.ColumnName));
                                    }

                                    break;
                                }
                            }
                        }

                        foreach (SortColumnDescriptor cd in this.inner)
                        {
                            cd.SetCollection(this);
                            cd.InitFieldDescriptor(tableDescriptor);
                        }

                        fieldsVersion = this.tableDescriptor.Fields.Version;
                        modified = false;
                    }
                }
                else if (!modified && isRelationChildColumns && tableDescriptor.IsForeignKeyRelationChildTableDescriptor())
                {
                    if (CheckOutOfDate()
                        || (inner.Count > 0 && tableDescriptor != null && !tableDescriptor.IsDisposed && fieldsVersion != this.tableDescriptor.Fields.Version)
                        || (!tableDescriptor.IsDisposed && relationVersion != this.tableDescriptor.ParentRelation.Collection.Version))
                    {
                        // Auto-poulate PrimaryKeyColumns from ParentRelation.RelationKeys
                        this.inner.Clear();
                        RelationKeyDescriptorCollection rkdKeys = this.tableDescriptor.ParentRelation.RelationKeys;
                        int count = rkdKeys.Count;
                        if (tableDescriptor.ParentRelation.RelationKind == RelationKind.ForeignKeyReference)
                        {
                            count--;
                        }

                        for (int n = 0; n < count; n++)
                        {
                            this.inner.Add(new RelationChildColumnDescriptor(rkdKeys[n].ParentKeyFieldName, rkdKeys[n].ChildKeyFieldName));
                        }

                        foreach (SortColumnDescriptor cd in this.inner)
                        {
                            cd.SetCollection(this);
                            cd.InitFieldDescriptor(tableDescriptor);
                        }

                        fieldsVersion = this.tableDescriptor.Fields.Version;
                        relationVersion = this.tableDescriptor.ParentRelation.Collection.Version;
                        modified = false;
                    }
                }
                else
                {
                    if (CheckOutOfDate()
                        || (inner.Count > 0 && tableDescriptor != null && fieldsVersion != this.tableDescriptor.Fields.Version))
                    {
                        foreach (SortColumnDescriptor cd in this.inner)
                        {
                            cd.InitFieldDescriptor(tableDescriptor);
                        }

                        fieldsVersion = this.tableDescriptor.Fields.Version;
                    }
                }
            }
            finally
            {
                inEnsureFieldDescriptors = false;
            }
        }

        /// <summary>
        /// Called internally to ensure all field descriptors are up to date after table descriptor is changed.
        /// </summary>
        /// <returns>true if field descriptors need to be reinitialized</returns>
        protected virtual bool CheckOutOfDate()
        {
            return false;
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
                if (insideCollectionEditor != value)
                {
                    insideCollectionEditor = value;
                }
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this);
                }
#else
                ;
#endif
            }
        }

        void IInsideCollectionEditorProperty.InitializeFrom(object other)
        {
            InitializeFrom((SortColumnDescriptorCollection)other);
        }

        /// <overload>
        /// Copies settings from another collection and raises <see cref="SortColumnDescriptorCollection.Changing"/> and <see cref="SortColumnDescriptorCollection.Changed"/>
        /// events if differences to the other collections are detected.
        /// </overload>
        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(SortColumnDescriptorCollection other)
        {
            InitializeFrom(other, true);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        /// <param name="raiseChangeEvents">Specifies if Changing and Changed events should be raised.</param>
        /// <returns>True if successfully initialized; False otherwise. </returns>
        public bool InitializeFrom(SortColumnDescriptorCollection other, bool raiseChangeEvents)
        {
            bool savedshouldPopulateThis = shouldPopulate;
            bool savedshouldPopulateOther = other.shouldPopulate;
            this.shouldPopulate = false;
            other.shouldPopulate = false;

            try
            {
                int i;
                inInitializeFrom = true;
                this.raiseChangeEvents = raiseChangeEvents;
                int v = version;
                int count = Math.Min(Count, other.Count);
                for (i = 0; i < count; i++)
                {
                    this[i].InitializeFrom(other[i].Clone());
                }

                for (; i < other.Count; i++)
                {
                    Add(other[i].Clone());
                }

                while (Count > other.Count)
                {
                    RemoveAt(Count - 1);
                }

                this.raiseChangeEvents = true;
                if (v != version)
                {
                    this.fieldsVersion = -1;
                    return true;
                }

                return false;
            }
            finally
            {
                inInitializeFrom = false;
                this.shouldPopulate = true;
                if (!savedshouldPopulateThis)
                {
                    this.disableShouldPopulate = true;
                }

                other.shouldPopulate = savedshouldPopulateOther;
            }
        }

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static readonly SortColumnDescriptorCollection Empty = new SortColumnDescriptorCollection((TableDescriptor)null);

        /// <overload>
        /// Initializes a new empty collection.
        /// </overload>
        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public SortColumnDescriptorCollection()
        {
        }

        /// <summary>
        /// Initializes a new empty collection and attaches it to a <see cref="TableDescriptor"/>
        /// </summary>
        /// <param name="tableDescriptor">Table descriptor.</param>
        public SortColumnDescriptorCollection(TableDescriptor tableDescriptor)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this);
            this.tableDescriptor = tableDescriptor;
        }

        internal SortColumnDescriptorCollection(params SortColumnDescriptor[] columnDescriptors)
        {
            this.inner.AddRange(columnDescriptors);
            for (int n = 0; n < columnDescriptors.Length; n++)
            {
                CheckType(columnDescriptors[n]);
                columnDescriptors[n].SetCollection(this);
            }
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="columnDescriptors">The array whose elements should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(SortColumnDescriptor[] columnDescriptors)
        {
            foreach (SortColumnDescriptor columnDescriptor in columnDescriptors)
            {
                Add(columnDescriptor);
            }
        }

        SortColumnDescriptorCollection(SortColumnDescriptor[] columnDescriptors, int version)
            : this(columnDescriptors)
        {
            this.version = version;
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public SortColumnDescriptorCollection Clone()
        {
            return InternalClone();
        }

        /// <summary>
        /// Copies all members to another collection.
        /// </summary>
        /// <param name="coll">The target collection.</param>
        protected void CopyAllMembersTo(SortColumnDescriptorCollection coll)
        {
            coll.fieldsVersion = -1;
            coll.relationVersion = -1;
            coll.inInitializeFrom = this.inInitializeFrom;
            coll.inner = this.inner;
            coll.insideCollectionEditor = this.insideCollectionEditor;
            coll.tableDescriptor = this.tableDescriptor;
            coll.version = this.version + 1000;

            coll.inner = new ArrayList();
            int count = Count;
            SortColumnDescriptor[] columnDescriptors = new SortColumnDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                coll.inner.Add(this[n].Clone());
                coll[n].SetCollection(coll);
            }
        }

        /// <summary>
        /// Creates a copy of this collection and all its inner elements. This method is called from Clone.
        /// </summary>
        /// <returns>returns SortColumnDescriptorCollection</returns>
        protected virtual SortColumnDescriptorCollection InternalClone()
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this);
            SortColumnDescriptorCollection coll = new SortColumnDescriptorCollection();
            CopyAllMembersTo(coll);
            return coll;
        }

        /// <summary>Determines if the specified object and current object are equal.</summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if the objects are equal; False otherwise.</returns>
        /// <override/>
        public override bool Equals(object obj)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this);
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (obj is SortColumnDescriptor[])
            {
                return Equals((SortColumnDescriptor[])obj);
            }

            if (!(obj is SortColumnDescriptorCollection))
            {
                return false;
            }

            return Equals((SortColumnDescriptorCollection)obj);
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code for the current object.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Increases the version counter for this collection.
        /// </summary>
        public void BumpVersion()
        {
            version++;
        }

        /// <summary>
        /// The version number of this collection. The version is increased each time the
        /// collection or an element within the collection was modified.
        /// </summary>
        public int Version
        {
            get
            {
                EnsureFieldDescriptors();
                return version;
            }
        }

        bool Equals(SortColumnDescriptor[] other)
        {
            int count = Count;
            if (other.Length != count)
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

        internal static bool Equals(SortColumnDescriptor[] src, SortColumnDescriptor[] other)
        {
            if (other.Length != src.Length)
            {
                return false;
            }

            for (int n = 0; n < other.Length; n++)
            {
                if (!src[n].Equals(other[n]))
                {
                    return false;
                }
            }

            return true;
        }

        bool Equals(SortColumnDescriptorCollection other)
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
        public SortColumnDescriptor this[int index]
        {
            get
            {
                EnsureFieldDescriptors();
                if(inner.Count > 0)
                    return (SortColumnDescriptor)inner[index];
                return null;
            }

            set
            {
                EnsureFieldDescriptors();
                CheckType(value);
                ////                TraceUtil.TraceCurrentMethodInfo(this, index, value);
                if (inner[index] != value)
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    inner[index] = value;
                    value.SetCollection(this);
                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
        public SortColumnDescriptor this[string name]
        {
            get
            {
                EnsureFieldDescriptors();
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (SortColumnDescriptor)inner[index];
            }

            set
            {
                EnsureFieldDescriptors();
                CheckType(value);
                ////                TraceUtil.TraceCurrentMethodInfo(this, name, value);
                int index = Find(name);
                if (index == -1)
                {
                    value.Name = name;
                    Add(value);
                }
                else
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    inner[index] = value;
                    value.Name = name;
                    value.SetCollection(this);
                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        internal int Find(string name)
        {
            EnsureFieldDescriptors();
            for (int n = 0; n < Count; n++)
            {
                if (this[n].Name == name)
                {
                    return n;
                }
            }

            return -1;
        }

        internal void FixCollection()
        {
            for (int n = 0; n < Count; n++)
            {
                this[n].SetCollection(this);
            }
        }

        internal void CheckCollection()
        {
            for (int n = 0; n < Count; n++)
            {
                if (this[n].Collection != this)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(SortColumnDescriptor value)
        {
            if (value == null)
            {
                return false;
            }

            EnsureFieldDescriptors();
            CheckType(value);

            return inner.Contains(value);
        }

        /// <summary>
        /// Determines if the element with the specified name belongs to this collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection.</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(string name)
        {
            return Find(name) != -1;
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(SortColumnDescriptor value)
        {
            EnsureFieldDescriptors();
            CheckType(value);
            return inner.IndexOf(value);
        }

        /// <summary>
        /// Searches for the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>The zero-based index of the occurrence of the element with matching name within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(string name)
        {
            return Find(name);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(SortColumnDescriptor[] array, int index)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this);
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        SortColumnDescriptorCollection SyncRoot
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
        /// <remarks>Enumerators only allow reading the data in the collection.
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public SortColumnDescriptorCollectionEnumerator GetEnumerator()
        {
            return new SortColumnDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, SortColumnDescriptor value)
        {
            CheckType(value);
            ////            TraceUtil.TraceCurrentMethodInfo(this, index, value.Name);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
            inner.Insert(index, value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
        }

        /// <summary>
        /// Removes the specified descriptor element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(SortColumnDescriptor value)
        {
            CheckType(value);
            ////            TraceUtil.TraceCurrentMethodInfo(this, value.Name);
            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.Remove(value);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds a SortColumnDescriptor to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(SortColumnDescriptor value)
        {
            CheckType(value);
            ////            TraceUtil.TraceCurrentMethodInfo(this, value.Name);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            if (!this.modified && !this.inEnsureFieldDescriptors)
            {
                inner.Clear();
            }

            if (value.Name != null && value.Name.Length > 0)
            {
                if (Contains(value.Name))
                {
                    throw new Exception(String.Format("Column '{0}': Duplicates are not allowed ", value.Name));
                }
            }
            else
            {
                SuggestName(value);
            }

            int index = inner.Add(value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Creates a SortColumnDescriptor with ListSortDirection.Ascending and adds it to the end of the collection.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name)
        {
            return InternalAdd(name, ListSortDirection.Ascending);
        }

        /// <summary>
        /// Creates a SortColumnDescriptor and adds it to the end of the collection.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name, ListSortDirection sortDirection)
        {
            return InternalAdd(name, sortDirection);
        }

        /// <summary>
        /// Called to create a SortColumnDescriptor and add it to the end of the collection.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        protected virtual int InternalAdd(string name, ListSortDirection sortDirection)
        {
            return Add(new SortColumnDescriptor(name, sortDirection));
        }

        /// <summary>
        /// Removes the specified descriptor element with the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the element to remove from the collection. If no element with that name is found
        /// in the collection, the method will do nothing.</param>
        public void Remove(string name)
        {
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
            ////            TraceUtil.TraceCurrentMethodInfo(this, index);
            object value = inner[index];
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.RemoveAt(index);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Disposes the object and collection items.
        /// </summary>
        public void Dispose()
        {
            foreach (DescriptorBase db in inner)
            {
                db.Dispose();
            }

            inner.Clear();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            if (inner.Count > 0)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inner.Clear();
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }

            this.modified = true;
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns False since this collection has no fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
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
        /// <remarks>
        /// If changes in the TableDescriptor are detected, the
        /// method will reinitialize the field descriptors before returning the count.
        /// </remarks>
        public int Count
        {
            get
            {
                this.EnsureFieldDescriptors();
                return inner.Count;
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            groupedColumnsComparer = null;
            version++;
            fieldsVersion = -1;

            // || this.inReset)
            if (this.inEnsureFieldDescriptors)
            {
                return;
            }

            if (!this.inReset)
            {
                modified = true;
            }

            if (!this.insideCollectionEditor && this.raiseChangeEvents)
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
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

        internal void RaisePropertyItemChanged(SortColumnDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            groupedColumnsComparer = null;
            if (!inInitializeFrom && e.PropertyName == "Name")
            {
                foreach (SortColumnDescriptor sc in this)
                {
                    if (sc != column && sc.Name == column.Name)
                    {
                        throw new Exception(String.Format("Column '{0}': Duplicates are not allowed ", column.Name));
                    }
                }
            }

            if (!this.insideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, inner.IndexOf(column), column, e.PropertyName, e));
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.insideCollectionEditor && this.raiseChangeEvents)
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
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

        internal void RaisePropertyItemChanging(SortColumnDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.insideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif

                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, inner.IndexOf(column), column, e.PropertyName, e));
            }
        }

        void SuggestName(SortColumnDescriptor value)
        {
            if (value.Name == null || value.Name.Length == 0)
            {
                FieldDescriptorCollection pdc = this.tableDescriptor.Fields;
                if (pdc.Count > this.Count)
                {
                    foreach (FieldDescriptor pd in pdc)
                    {
                        if (IndexOf(pd.Name) == -1)
                        {
                            value.Name = pd.Name;
                            return;
                        }
                    }
                }
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
                this[index] = (SortColumnDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (SortColumnDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((SortColumnDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((SortColumnDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((SortColumnDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((SortColumnDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((SortColumnDescriptor[])array, index);
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
            return ((ICustomTypeDescriptor)this).GetProperties(null);
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

            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection((PropertyDescriptor[])pds.ToArray(typeof(PropertyDescriptor)));
            return pdc.Sort((string[])names.ToArray(typeof(string)));
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="SortColumnDescriptor"/> elements of a <see cref="SortColumnDescriptorCollection"/>.
    /// </summary>
    public class SortColumnDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        SortColumnDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public SortColumnDescriptorCollectionEnumerator(SortColumnDescriptorCollection collection)
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
        public SortColumnDescriptor Current
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

    /// <summary>
    /// The type converter for <see cref="SortColumnDescriptor"/> objects. <see cref="SortColumnDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class SortColumnDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <override/>
        /// <summary>Determines whether this object can be converted to the specified type, using the given format.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>True if this conversion is supported; False otherwise.</returns>
        /// <override/>
        public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
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
        /// <summary>Converts the given value to the specified type, using the given format and culture.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">The CultureInfo.</param>
        /// <param name="value">The Value.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        /// <override/>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
                && (value is SortColumnDescriptor))
            {
                SortColumnDescriptor typename = (SortColumnDescriptor)value;
                System.Type[] args;
                args = new System.Type[2];
                args[0] = typeof(string);
                args[1] = typeof(ListSortDirection);

                System.Reflection.ConstructorInfo constructorInfo;
                constructorInfo = typeof(SortColumnDescriptor).GetConstructor(args);
                if (constructorInfo != null)
                {
                    object[] argValues;
                    argValues = (object[])new System.Object[2];
                    argValues[0] = typename.Name;
                    argValues[1] = typename.SortDirection;
                    return (object)new InstanceDescriptor(constructorInfo, argValues);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo

        /// <override/>
        /// <summary>
        /// Returns a collection of properties for the specified object type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Object type.</param>
        /// <param name="attributes">An array of System.Attribue objects that will be used as a filter.</param>
        /// <returns>Property descriptor collection.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "SortDirection",
            };

            return pds.Sort(atts);
        }
    }

    /// <summary>
    /// A SortColumnDescriptor defines
    /// the sort order or grouping of a table.
    /// SortColumnDescriptors are managed by the <see cref="SortColumnDescriptorCollection"/> which
    /// is returned by the <see cref="Syncfusion.Grouping.TableDescriptor.SortedColumns"/> or <see cref="Syncfusion.Grouping.TableDescriptor.GroupedColumns"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(SortColumnDescriptorTypeConverter))]
    public class SortColumnDescriptor : DescriptorBase, ICloneable, IStandardValuesProvider
    {
        string name = string.Empty;
        FieldDescriptor pd = null;
        IGroupByColumnCategorizer categorizer = null;
        IComparer comparer = null;
        ListSortDirection sortDirection = ListSortDirection.Ascending;
        IGroupSortOrderComparer groupSortKeyComparer = null;

        ////        bool inSetName = false;
        SortColumnDescriptorCollection collection;
        TableDescriptor tableDescriptor;

        /// <summary>
        /// Occurs when a property was changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        SortColumnDescriptorCollection copy;

        internal SortColumnDescriptorCollection GetShadowedCopy()
        {
            if (copy == null || copy.version != collection.version)
            {
                copy = new SortColumnDescriptorCollection();
                copy.Add(this.Clone());
                copy.version = collection.version;
            }

            return copy;
        }

        /// <summary>
        /// Initializes a new empty object.
        /// </summary>
        public SortColumnDescriptor()
        {
            ////            TraceUtil.TraceCurrentMethodInfo();
        }

        /// <summary>
        /// Initializes a new sort descriptor for the field with the given name.
        /// </summary>
        /// <param name="name">The field name.</param>
        public SortColumnDescriptor(string name)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(name);
            this.name = name;
        }

        /// <summary>
        /// Initializes a new sort descriptor for the field with the given name.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="sortDirection">The sort direction.</param>
        public SortColumnDescriptor(string name, ListSortDirection sortDirection)
        {
            this.sortDirection = sortDirection;
            this.name = name;
        }

        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return GetType().Name + " { " + name + " }";
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                pd = null;
                tableDescriptor = null;
                collection = null;
            }

            base.Dispose(disposing);
        }
        
        internal void SetCollection(SortColumnDescriptorCollection collection)
        {
            this.collection = collection;
            if (this.tableDescriptor == null)
            {
                this.tableDescriptor = collection.tableDescriptor;
            }
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public SortColumnDescriptorCollection Collection
        {
            get
            {
                return collection;
            }
        }

        ICollection IStandardValuesProvider.GetStandardValues(PropertyDescriptor pd)
        {
            ArrayList al = new ArrayList();
            SortedList sl = new SortedList();
            foreach (FieldDescriptor ppd in TableDescriptor.Fields)
            {
                al.Add(ppd.Name);
            }

            return al;
        }

        internal void SetTableDescriptor(TableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
        }

        /// <summary>
        /// The TableDescriptor that this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public TableDescriptor TableDescriptor
        {
            get
            {
                return tableDescriptor;
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

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Called from <see cref="Clone"/> to create a new object of the correct type and copies all its members to this
        /// new object with <see cref="CopyAllMembersTo"/>.
        /// </summary>
        /// <returns>The new object.</returns>
        protected virtual object InternalClone()
        {
            SortColumnDescriptor sd = new SortColumnDescriptor();
            CopyAllMembersTo(sd);
            return sd;
        }

        /// <summary>
        /// Copies all members to another object without raising change events.
        /// </summary>
        /// <param name="sd">The target object.</param>
        protected void CopyAllMembersTo(SortColumnDescriptor sd)
        {
            sd.categorizer = this.categorizer;
            sd.collection = this.collection;
            sd.comparer = this.comparer;
            sd.groupSortKeyComparer = this.groupSortKeyComparer;
            sd.isSorting = this.isSorting;
            sd.name = this.name;
            sd.pd = this.pd;
            sd.sortDirection = this.sortDirection;
            sd.tableDescriptor = this.tableDescriptor;
        }

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public SortColumnDescriptor Clone()
        {
            return (SortColumnDescriptor)InternalClone();
        }

        /// <summary>Determines if the specified object and current object are equal.</summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if the objects are equal; False otherwise.</returns>
        /// <override/>
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
            else if (!(obj is SortColumnDescriptor))
            {
                return false;
            }

            return Equals((SortColumnDescriptor)obj);
        }

        bool Equals(SortColumnDescriptor other)
        {
            return other.name == name
                && other.categorizer == categorizer
                && other.comparer == comparer
                && other.groupSortKeyComparer == groupSortKeyComparer
                && other.sortDirection == sortDirection;
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public virtual void InitializeFrom(SortColumnDescriptor other)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(other.Name);
            this.Name = other.Name;
            this.Categorizer = other.Categorizer;
            this.Comparer = other.Comparer;
            this.GroupSortOrderComparer = other.GroupSortOrderComparer;
            this.SortDirection = other.SortDirection;
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>Gets the name of the descriptor.</summary>
        /// <returns>Descriptor name.</returns>
        /// <override/>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// The name of the field to sort. Usually the same as the corresponding GridColumnDescriptor.MappingName or FieldDescriptor.Name.
        /// </summary>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of the field to sort.")]
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
                    ////                    inSetName = true;
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    name = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                    ////                    inSetName = false;
                }
            }
        }

        /// <summary>
        /// Called internally to initalize field descriptor for the Name of the field.
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor for the field.</param>
        /// <returns>True if field descriptor was found; False otherwise.</returns>
        public virtual bool InitFieldDescriptor(TableDescriptor tableDescriptor)
        {
            pd = tableDescriptor.Fields[this.Name];
            return pd != null;
        }

        /// <summary>
        /// Gets the <see cref="FieldDescriptor"/> in the parent table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FieldDescriptor FieldDescriptor
        {
            get
            {
                if (!isSorting)
                {
                    this.collection.EnsureFieldDescriptors();
                }

                return pd;
            }
        }

        bool isSorting = false;

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsSorting
        {
            get
            {
                return isSorting;
            }

            set
            {
                if (value)
                {
                    this.collection.EnsureFieldDescriptors();
                }

                isSorting = value;
            }
        }

        /// <summary>
        /// Gets / sets a custom categorizer for categorizing records in the table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IGroupByColumnCategorizer Categorizer
        {
            get
            {
                return categorizer;
            }

            set
            {
                if (categorizer != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Categorizer"));
                    categorizer = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Categorizer"));
                }
            }
        }

        /// <summary>
        /// Gets / sets a custom comparer for sorting records in the table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IComparer Comparer
        {
            get
            {
                return comparer;
            }

            set
            {
                if (comparer != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Comparer"));
                    comparer = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Comparer"));
                }
            }
        }

        /// <summary>
        /// Gets / sets a custom comparer for sorting groups if groups should be sorted in a different
        /// order than the category, e.g. sort by summary values of nested groups.
        /// </summary>
        /// <example>This example shows how to customize sorting of groups that were categorized based on ShipVia field. The groups will be sorted by the value of a summary in the group.
        /// <code lang="C#">
        ///  <para/>
        ///     this.gridGroupingControl1.TableDescriptor.GroupedColumns.Clear();
        ///     SortColumnDescriptor gsd = new SortColumnDescriptor("ShipCountry");
        ///     gsd.GroupSortOrderComparer = new ShipViaComparer(summaryColumn1.GetSummaryDescriptorName(), "Average");
        ///     this.gridGroupingControl1.TableDescriptor.GroupedColumns.Add(gsd);
        ///     this.gridGroupingControl1.InvalidateAllWhenListChanged = true;
        ///  <para/>
        ///  <para/>
        /// public class ShipViaComparer : object, IGroupSortOrderComparer
        /// {
        ///     string summaryDescriptorName;
        ///     string propertyName;
        ///  <para/>
        ///     public ShipViaComparer(string summaryDescriptorName, string propertyName)
        ///     {
        ///         this.summaryDescriptorName = summaryDescriptorName;
        ///         this.propertyName = propertyName;
        ///     }
        ///  <para/>
        ///     #region IComparer Members
        ///  <para/>
        ///     public int Compare(object x, object y)
        ///     {
        ///         Group gx = (Group) x;
        ///         Group gy = (Group) y;
        ///  <para/>
        ///         bool strongTyped = true;
        ///  <para/>
        ///         if (strongTyped)
        ///         {
        ///             // strong typed   (propertyName is ignored ...)
        ///             DoubleAggregateSummary dasx = (DoubleAggregateSummary) gx.GetSummary(summaryDescriptorName);
        ///             DoubleAggregateSummary dasy = (DoubleAggregateSummary) gy.GetSummary(summaryDescriptorName);
        ///  <para/>
        ///             int v = dasx.Average.CompareTo(dasy.Average);
        ///             // Console.WriteLine("Compare {0} to {1}: {2}", dasx.Average, dasy.Average, v);
        ///             return v;
        ///         }
        ///         else
        ///         {
        ///             // using reflection (slower but more flexible using propertyName)
        ///             object vx = gx.GetSummaryProperty(summaryDescriptorName, propertyName);
        ///             object vy = gy.GetSummaryProperty(summaryDescriptorName, propertyName);
        ///  <para/>
        ///             return ((IComparable) vx).CompareTo(vy);
        ///         }
        ///     }
        ///  <para/>
        ///     #endregion
        ///  <para/>    
        ///     public string[] GetDependantFields(TableDescriptor td)
        ///     {
        ///         SummaryDescriptor sd = td.Summaries[summaryDescriptorName];
        ///         if (sd == null)
        ///             return new string[0];
        ///         return new string[] { sd.MappingName };
        ///     }
        /// }
        /// </code>
        /// </example>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IGroupSortOrderComparer GroupSortOrderComparer
        {
            get
            {
                return groupSortKeyComparer;
            }

            set
            {
                if (groupSortKeyComparer != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("GroupSortOrderComparer"));
                    groupSortKeyComparer = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("GroupSortOrderComparer"));
                }
            }
        }

        /// <summary>
        /// Sets a custom comparer for sorting groups if groups should be sorted in a different
        /// order than the category, e.g. sort by summary values of nested groups.
        /// </summary>
        /// <param name="summaryDescriptorName">Name of the summary descriptor.</param>
        /// <param name="propertyName">Summary function.</param>
        /// <example>This example shows how to customize sorting of groups that were categorized based on ShipVia field. The groups will be sorted by the value of a summary in the group.
        /// <code lang="C#">
        ///  <para/>
        ///     this.gridGroupingControl1.TableDescriptor.GroupedColumns.Clear();
        ///     SortColumnDescriptor gsd = new SortColumnDescriptor("ShipCountry");
        ///     gsd.SetGroupSummarySortOrder(summaryColumn1.GetSummaryDescriptorName(), "Average", ListSortDirection.Ascending);
        ///     this.gridGroupingControl1.InvalidateAllWhenListChanged = true;
        /// </code>
        /// </example>
        public void SetGroupSummarySortOrder(string summaryDescriptorName, string propertyName)
        {
            this.GroupSortOrderComparer = new GroupSortOrderSummaryComparer(summaryDescriptorName, propertyName, ListSortDirection.Ascending);
        }

        /// <summary>
        /// Sets a custom comparer for sorting groups if groups should be sorted in a different
        /// order than the category, e.g. sort by summary values of nested groups.
        /// </summary>
        /// <param name="summaryDescriptorName">Name of the summary descriptor.</param>
        /// <param name="propertyName">Summary function.</param>
        /// <param name="sortDirection">Sort order.</param>
        /// <example>This example shows how to customize sorting of groups that were categorized based on ShipVia field. The groups will be sorted by the value of a summary in the group.
        /// <code lang="C#">
        /// <para/>
        ///     this.gridGroupingControl1.TableDescriptor.GroupedColumns.Clear();
        ///     SortColumnDescriptor gsd = new SortColumnDescriptor("ShipCountry");
        ///     gsd.SetGroupSummarySortOrder(summaryColumn1.GetSummaryDescriptorName(), "Average", ListSortDirection.Ascending);
        ///     this.gridGroupingControl1.InvalidateAllWhenListChanged = true;
        /// </code>
        /// </example>
        public void SetGroupSummarySortOrder(string summaryDescriptorName, string propertyName, ListSortDirection sortDirection)
        {
            this.GroupSortOrderComparer = new GroupSortOrderSummaryComparer(summaryDescriptorName, propertyName, sortDirection);
        }

        /// <summary>
        /// The sort direction.
        /// </summary>
        [Description("The sort direction.")]
        public ListSortDirection SortDirection
        {
            get
            {
                return sortDirection;
            }

            set
            {
                if (sortDirection != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("SortDirection"));
                    sortDirection = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("SortDirection"));
                }
            }
        }
    }

    /// <summary>For internal use.</summary>
    /// <exclude/>
    public class GroupSortOrderSummaryComparer : IGroupSortOrderComparer
    {
        string summaryDescriptorName;
        string propertyName;
        ListSortDirection sortDirection;

        /// <summary>For internal use.</summary>
        public string SummaryDescriptorName
        {
            get
            {
                return summaryDescriptorName;
            }
        }

        /// <summary>For internal use.</summary>
        public GroupSortOrderSummaryComparer(string summaryDescriptorName, string propertyName, ListSortDirection sortDirection)
        {
            this.summaryDescriptorName = summaryDescriptorName;
            this.propertyName = propertyName;
            this.sortDirection = sortDirection;
        }

        #region IComparer Members
        /// <summary>For internal use.</summary>
        public int Compare(object x, object y)
        {
            Group gx = (Group)x;
            Group gy = (Group)y;

            // using reflection (slower but more flexible using propertyName) 
            object vx = gx.GetSummaryProperty(summaryDescriptorName, propertyName);
            object vy = gy.GetSummaryProperty(summaryDescriptorName, propertyName);

            if (sortDirection == ListSortDirection.Ascending)
            {
                return ((IComparable)vx).CompareTo(vy);
            }

            return ((IComparable)vy).CompareTo(vx);
        }

        #endregion

        #region IGroupSortOrderComparer Members
        /// <summary>For internal use.</summary>
        public string[] GetDependantFields(TableDescriptor td)
        {
            SummaryDescriptor sd = td.Summaries[summaryDescriptorName];
            if (sd == null)
            {
                return new string[0];
            }

            return new string[] { sd.MappingName };
        }
        #endregion
    }
}