//-------------------------------------------------------------------------------------------------
// <copyright file="RelationKeyDescriptor.cs" company="syncfusion">
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
    /// A RelationKeyDescriptor defines the mapping between parent and child columns in a master details relation.
    /// RelationKeyDescriptor are managed by the <see cref="RelationKeyDescriptorCollection"/> which
    /// is returned by the <see cref="RelationDescriptor.RelationKeys"/> property
    /// of a <see cref="RelationDescriptor"/>.
    /// </summary>
    public class RelationKeyDescriptor : DescriptorBase, ICloneable, IStandardValuesProvider
    {
        RelationDescriptor relationDescriptor;
        string childKeyFieldName;
        string parentKeyFieldName;
        FieldDescriptor childKeyField;
        FieldDescriptor parentKeyField;

        /// <summary>
        /// Initializes a new object.
        /// </summary>
        public RelationKeyDescriptor()
        {
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. 
        /// </summary>
        /// <param name="other">The source object.</param>
        public virtual void InitializeFrom(RelationKeyDescriptor other)
        {
            this.ChildKeyFieldName = other.ChildKeyFieldName;
            this.ParentKeyFieldName = other.ParentKeyFieldName;
        }

        /// <summary>Gets the key field name.</summary>
        /// <returns>Key field name.</returns>
        /// <override/>
        public override string GetName()
        {
            return parentKeyFieldName;
        }

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public RelationKeyDescriptor Clone()
        {
            RelationKeyDescriptor other = new RelationKeyDescriptor();
            other.childKeyFieldName = childKeyFieldName;
            other.parentKeyFieldName = parentKeyFieldName;
            return other;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                parentKeyFieldName = childKeyFieldName = "Disposed";
                childKeyField = null;
                parentKeyField = null;
                relationDescriptor = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>Determines if the specified object and current object are equal.</summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if both objects are equal; False otherwise.</returns>
        /// <override/>
        public override bool Equals(object obj)
        {
            RelationKeyDescriptor other = obj as RelationKeyDescriptor;
            if (obj == null)
            {
                return this == null;
            }
            else if (other == null)
            {
                return false;
            }
            else
            {
                return other.childKeyFieldName == childKeyFieldName
                    && other.parentKeyFieldName == parentKeyFieldName;
            }
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>Hash code for the current object type.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// The name of the key column in the child table.
        /// </summary>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of the key column in the child table.")]
        public string ChildKeyFieldName
        {
            get
            {
                return this.childKeyFieldName;
            }

            set
            {
                if (this.childKeyFieldName != value)
                {
                    this.childKeyFieldName = value;
                    childKeyField = null;
                    this.RaiseChildKeyFieldNameChanged();
                }
            }
        }

        /// <summary>
        /// Determines if a key column for the child table was specified.
        /// </summary>
        /// <returns>True if a key column for the child table was specified; False otherwise.</returns>
        public bool ShouldSerializeChildKeyFieldName()
        {
            return ParentRelationDescriptor == null || !this.ParentRelationDescriptor.ShouldSerializeMappingName();
        }

        internal void RaiseChildKeyFieldNameChanged()
        {
        }

        /// <summary>
        /// The <see cref="FieldDescriptor"/> for the the key column of the child table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FieldDescriptor ChildKeyField
        {
            get
            {
                SetChildKeyField(relationDescriptor.ChildTableDescriptor.Fields[ChildKeyFieldName]);
                ////relationDescriptor.EnsureInitialized();
                return childKeyField;
            }
        }

        internal void SetChildKeyField(FieldDescriptor value)
        {
            childKeyField = value;
        }

        /// <summary>
        /// The name of the key column in the parent table.
        /// </summary>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of the key column in the parent table.")]
        public string ParentKeyFieldName
        {
            get
            {
                return this.parentKeyFieldName;
            }

            set
            {
                if (this.parentKeyFieldName != value)
                {
                    this.parentKeyFieldName = value;
                    parentKeyField = null;
                    this.RaiseParentKeyFieldNameChanged();
                }
            }
        }

        /// <summary>
        /// Determines if a key column for the parent table was specified.
        /// </summary>
        /// <returns>True if a key column for the parent table was specified; False otherwise.</returns>
        public bool ShouldSerializeParentKeyFieldName()
        {
            return ParentRelationDescriptor == null || !this.ParentRelationDescriptor.ShouldSerializeMappingName();
        }

        internal void RaiseParentKeyFieldNameChanged()
        {
        }

        /// <summary>
        /// The <see cref="FieldDescriptor"/> for the the key column of the parent table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FieldDescriptor ParentKeyField
        {
            get
            {
                SetParentKeyField(relationDescriptor.ParentTableDescriptor.Fields[ParentKeyFieldName]);
                ////relationDescriptor.EnsureInitialized();
                return parentKeyField;
            }
        }

        internal void SetParentKeyField(FieldDescriptor value)
        {
            parentKeyField = value;
            if (parentKeyField != null)
            {
                parentKeyField.isForeignKeyField = relationDescriptor.RelationKind == RelationKind.ForeignKeyReference;
            }
        }

        /// <summary>
        /// The RelationDescriptor this key descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RelationDescriptor ParentRelationDescriptor
        {
            get
            {
                return relationDescriptor;
            }
        }

        internal void SetParentRelationDescriptor(RelationDescriptor value)
        {
            relationDescriptor = value;
        }

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region IStandardValuesProvider Members

        ICollection IStandardValuesProvider.GetStandardValues(PropertyDescriptor pd)
        {
            switch (pd.Name)
            {
                case "ChildKeyFieldName":
                    {
                        ArrayList al = new ArrayList();
                        foreach (FieldDescriptor fd in this.ParentRelationDescriptor.ChildTableDescriptor.Fields)
                        {
                            al.Add(fd.Name);
                        }

                        return al.ToArray();
                    }

                case "ParentKeyFieldName":
                    {
                        ArrayList al = new ArrayList();
                        foreach (FieldDescriptor fd in this.ParentRelationDescriptor.ParentTableDescriptor.Fields)
                        {
                            al.Add(fd.Name);
                        }

                        return al.ToArray();
                    }
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// A collection of <see cref="RelationKeyDescriptor"/> that are children of a <see cref="RelationDescriptor"/>.
    /// A RelationKeyDescriptor defines the mapping between parent and child columns in a master details relation.
    /// An instance of this collection is returned by the <see cref="RelationDescriptor.RelationKeys"/> property
    /// of a <see cref="RelationDescriptor"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    public class RelationKeyDescriptorCollection : IList
    {
        internal ArrayList inner;
        RelationDescriptor parentRelationDescriptor;

        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public RelationKeyDescriptorCollection()
            : this(null)
        {
        }

        internal RelationKeyDescriptorCollection(RelationDescriptor parentRelationDescriptor)
        {
            inner = new ArrayList();
            this.parentRelationDescriptor = parentRelationDescriptor;
        }

        ////bool inInitializeFrom = false;

        /// <summary>
        /// Copies settings from another collection.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(RelationKeyDescriptorCollection other)
        {
            int i;
            ////inInitializeFrom = true;
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
            ////inInitializeFrom = false;
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="values">The array whose elements should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(RelationKeyDescriptor[] values)
        {
            this.inner.AddRange(values);
            for (int i = 0; i < values.Length; i++)
            {
                values[i].SetParentRelationDescriptor(this.parentRelationDescriptor);
            }
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public RelationKeyDescriptor this[int index]
        {
            get
            {
                RelationKeyDescriptor value = (RelationKeyDescriptor)inner[index];
                value.SetParentRelationDescriptor(this.parentRelationDescriptor);
                return value;
            }

            set
            {
                if (inner[index] != value)
                {
                    RaiseChanging();
                    value.SetParentRelationDescriptor(this.parentRelationDescriptor);
                    inner[index] = value;
                    RaiseChanged();
                }
            }
        }

        void RaiseChanged()
        {
            if (parentRelationDescriptor != null)
            {
                parentRelationDescriptor.RaisePropertyChanged("RelationKeys", null);
            }
        }

        void RaiseChanging()
        {
            if (parentRelationDescriptor != null)
            {
                parentRelationDescriptor.RaisePropertyChanging("RelationKeys", null);
            }
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(RelationKeyDescriptor value)
        {
            return inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(RelationKeyDescriptor value)
        {
            return inner.IndexOf(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(RelationKeyDescriptor[] array, int index)
        {
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        RelationKeyDescriptorCollection SyncRoot
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
        public RelationKeyDescriptorCollectionEnumerator GetEnumerator()
        {
            return new RelationKeyDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, RelationKeyDescriptor value)
        {
            RaiseChanging();
            value.SetParentRelationDescriptor(this.parentRelationDescriptor);
            inner.Insert(index, value);
            RaiseChanged();
        }

        /// <summary>
        /// Removes the specified descriptor element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        public void Remove(RelationKeyDescriptor value)
        {
            RaiseChanging();
            inner.Remove(value);
            RaiseChanged();
        }

        /// <summary>
        /// Adds a RelationKeyDescriptor to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(RelationKeyDescriptor value)
        {
            RaiseChanging();
            value.SetParentRelationDescriptor(this.parentRelationDescriptor);
            int index = inner.Add(value);
            RaiseChanged();
            return index;
        }

        /// <summary>
        /// Adds a RelationChildColumnDescriptor to the end of the collection.
        /// </summary>
        /// <param name="parentKeyFieldName">The name of the index field in the parent table.</param>
        /// <param name="childKeyFieldName">The name of the index field in the child table.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string parentKeyFieldName, string childKeyFieldName)
        {
            RelationKeyDescriptor relationKeyDescriptor1 = new RelationKeyDescriptor();
            relationKeyDescriptor1.ChildKeyFieldName = childKeyFieldName;
            relationKeyDescriptor1.ParentKeyFieldName = parentKeyFieldName;
            return Add(relationKeyDescriptor1);
        }

        internal int InnerAdd(string parentKeyFieldName, string childKeyFieldName)
        {
            RelationKeyDescriptor relationKeyDescriptor1 = new RelationKeyDescriptor();
            relationKeyDescriptor1.ChildKeyFieldName = childKeyFieldName;
            relationKeyDescriptor1.ParentKeyFieldName = parentKeyFieldName;
            relationKeyDescriptor1.SetParentRelationDescriptor(this.parentRelationDescriptor);
            return inner.Add(relationKeyDescriptor1);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            RaiseChanging();
            inner.RemoveAt(index);
            RaiseChanged();
        }

        /// <summary>
        /// Disposes of the object and collection items.
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
            RaiseChanging();
            inner.Clear();
            RaiseChanged();
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
        /// If changes in the RelationDescriptor or TableDescriptor are detected, the
        /// method will reinitialize the collection before returning the count.
        /// </remarks>
        public int Count
        {
            get
            {
                if (parentRelationDescriptor != null)
                {
                    parentRelationDescriptor.EnsureInitialized();
                }

                return inner.Count;
            }
        }

        /// <summary>Determines if the specified object and current object are equal.</summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if both objects are equal; False otherwise.</returns>
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
            else if (!(obj is RelationKeyDescriptorCollection))
            {
                return false;
            }

            return Equals((RelationKeyDescriptorCollection)obj);
        }

        bool Equals(RelationKeyDescriptorCollection other)
        {
            int count = inner.Count;
            if (other.inner.Count != count)
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

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>Hash code for the current object type.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
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
                this[index] = (RelationKeyDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (RelationKeyDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((RelationKeyDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((RelationKeyDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((RelationKeyDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((RelationKeyDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((RelationKeyDescriptor[])array, index);
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
    /// Enumerator class for the <see cref="RelationKeyDescriptor"/> elements of a <see cref="RelationKeyDescriptorCollection"/>.
    /// </summary>
    public class RelationKeyDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        RelationKeyDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public RelationKeyDescriptorCollectionEnumerator(RelationKeyDescriptorCollection collection)
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
        public RelationKeyDescriptor Current
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
}
