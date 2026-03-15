//-------------------------------------------------------------------------------------------------
// <copyright file="RelationChildColumn.cs" company="syncfusion">
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
    /// A collection from <see cref="RelationChildColumnDescriptor"/> that are children of a <see cref="TableDescriptor"/>.
    /// A RelationChildColumnDescriptor defines the sort order of a related table which is defined by the child columns in a 
    /// a master details relation. <para/>
    /// An instance of this collection is returned by the <see cref="TableDescriptor.RelationChildColumns"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class RelationChildColumnDescriptorCollection : SortColumnDescriptorCollection
    {
        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static new readonly RelationChildColumnDescriptorCollection Empty = new RelationChildColumnDescriptorCollection((TableDescriptor)null);

        int relationChildColumnsFieldVersion;
        
        /// <summary>
        /// Called internally to ensure all field descriptors are up to date after table descriptor is changed.
        /// </summary>
        /// <returns>
        /// true if field descriptors need to be reinitialized
        /// </returns>
        /// <override/>
        protected override bool CheckOutOfDate()
        {
            if (tableDescriptor != null)
            {
                RelationDescriptor rd = this.tableDescriptor.ParentRelation;
                if (rd != null)
                {
                    //// Be careful: Fields.Version can possible Dispose ParentRelation!
                    if (rd.ParentTableDescriptor != null && relationChildColumnsFieldVersion != rd.ParentTableDescriptor.Fields.Version
                        && !rd.IsDisposed)  
                    {
                        relationChildColumnsFieldVersion = this.tableDescriptor.ParentRelation.ParentTableDescriptor.Fields.Version;
                        return true;
                    }
                    else if (rd.IsDisposed)
                    {
                        return false;
                    }
                }
            }

            return base.CheckOutOfDate();
        }
        
        internal RelationChildColumnDescriptorCollection()
            : base()
        {
        }

        internal RelationChildColumnDescriptorCollection(TableDescriptor tableDescriptor)
            : base(tableDescriptor)
        {
        }

        /// <override/>
        protected override void CheckType(object obj)
        {
            if (obj != null && !(obj is RelationChildColumnDescriptor))
            {
                throw new ArgumentException("Wrong type");
            }
        }

        /// <summary>
        /// Creates a copy of this collection and all its inner elements. This method is called from Clone.
        /// </summary>
        /// <returns>returns SortColumnDescriptorCollection</returns>
        /// <override/>
        protected override SortColumnDescriptorCollection InternalClone()
        {
            RelationChildColumnDescriptorCollection coll = new RelationChildColumnDescriptorCollection(null);
            CopyAllMembersTo(coll);
            return coll;
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="columnDescriptors">The array whose elements should be added to the end of the collection. 
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic). 
        /// </param>
        public void AddRange(RelationChildColumnDescriptor[] columnDescriptors)
        {
            base.AddRange((SortColumnDescriptor[])columnDescriptors);
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public new RelationChildColumnDescriptorCollection Clone()
        {
            return (RelationChildColumnDescriptorCollection)InternalClone();
        }

        /// <overload>
        /// Copies settings from another collection and raises <see cref="SortColumnDescriptorCollection.Changing"/> and <see cref="SortColumnDescriptorCollection.Changed"/>
        /// events if differences to the other collections are detected.
        /// </overload>
        /// <summary>
        /// Copies settings from another collection and raises <see cref="SortColumnDescriptorCollection.Changing"/> and <see cref="SortColumnDescriptorCollection.Changed"/>
        /// events if differences to the other collections are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(RelationKeyDescriptorCollection other)
        {
            InitializeFrom(other, true);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="SortColumnDescriptorCollection.Changing"/> and <see cref="SortColumnDescriptorCollection.Changed"/>
        /// events if differences to the other collections are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        /// <param name="raiseChangeEvents">Specifies if Changing and Changed events should be raised.</param>
        /// <returns>True if the operation is successfully completed.</returns>
        public bool InitializeFrom(RelationKeyDescriptorCollection other, bool raiseChangeEvents)
        {
            RelationChildColumnDescriptorCollection coll = new RelationChildColumnDescriptorCollection((TableDescriptor)null);
            foreach (RelationKeyDescriptor key in other)
            {
                RelationChildColumnDescriptor rdc = new RelationChildColumnDescriptor();
                rdc.Name = key.ChildKeyFieldName;
                rdc.ParentColumnName = key.ParentKeyFieldName;
                coll.Add(rdc);
            }

            return base.InitializeFrom(coll, raiseChangeEvents);
        }

        /// <summary>Determines if the specified object is equivalent to the current object.</summary>
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
            else if (!(obj is RelationChildColumnDescriptorCollection))
            {
                return false;
            }

            return Equals((RelationChildColumnDescriptorCollection)obj);
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        bool Equals(RelationChildColumnDescriptorCollection other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public new RelationChildColumnDescriptor this[int index]
        {
            get
            {
                return (RelationChildColumnDescriptor)base[index];
            }

            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
        public new RelationChildColumnDescriptor this[string name]
        {
            get
            {
                return (RelationChildColumnDescriptor)base[name];
            }

            set
            {
                base[name] = value;
            }
        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection. 
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public new RelationChildColumnDescriptorCollectionEnumerator GetEnumerator()
        {
            return new RelationChildColumnDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Adds a RelationChildColumnDescriptor to the end of the collection.
        /// </summary>
        /// <param name="parentColumnName">The name of the index field in the parent table.</param>
        /// <param name="childColumnName">The name of the index field in the child table.</param>       
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string parentColumnName, string childColumnName)
        {
            return Add(new RelationChildColumnDescriptor(parentColumnName, childColumnName));
        }

        /// <summary>
        /// Called to create a SortColumnDescriptor and add it to the end of the collection.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns>
        /// The zero-based collection index at which the value has been added.
        /// </returns>
        /// <override/>
        protected override int InternalAdd(string name, ListSortDirection sortDirection)
        {
            return Add(new RelationChildColumnDescriptor(name, name));
        }
    }

    /// <summary>
    /// A RelationChildColumnDescriptor defines the sort order of a related table which is defined by the child columns in a
    /// a master details relation. <para/>
    /// RelationChildColumnDescriptorare managed by the <see cref="RelationChildColumnDescriptorCollection"/> that is returned by the <see cref="TableDescriptor.RelationChildColumns"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    public class RelationChildColumnDescriptor : SortColumnDescriptor
    {
        FieldDescriptor parentFieldDescriptor;

        /// <summary>
        /// The FieldDescriptor for the <see cref="ParentColumnName"/>.
        /// </summary>
        public FieldDescriptor ParentFieldDescriptor
        {
            get
            {
                return parentFieldDescriptor;
            }
        }

        /// <summary>
        /// Called from <see cref="Clone"/> to create a new object of the correct type and copies all its members to this
        /// new object with <see cref="CopyAllMembersTo"/>.
        /// </summary>
        /// <returns>The new object.</returns>
        /// <override/>
        protected override object InternalClone()
        {
            RelationChildColumnDescriptor sd = new RelationChildColumnDescriptor();
            base.CopyAllMembersTo(sd);
            sd.parentFieldDescriptor = this.parentFieldDescriptor;
            sd.parentColumnName = this.parentColumnName;
            return sd;
        }
        
        /// <summary>Initializes a field descriptor.</summary>
        /// <param name="tableDescriptor">Table descriptor.</param>
        /// <returns>True if the field descriptor is initialized successfully.</returns>
        /// <override/>
        public override bool InitFieldDescriptor(TableDescriptor tableDescriptor)
        {
            if (tableDescriptor.ParentTableDescriptor != null)
            {
                parentFieldDescriptor = tableDescriptor.ParentTableDescriptor.Fields[this.ParentColumnName];
            }

            return base.InitFieldDescriptor(tableDescriptor);
        }

        /// <summary>
        /// Initalizes a new empty descriptor.
        /// </summary>
        public RelationChildColumnDescriptor()
        {
        }

        /// <summary>
        /// Initalizes a new descriptor with parent and child column names identifying the fields
        /// that establish the relation between two tables.
        /// </summary>
        /// <param name="parentColumnName">Parent column name.</param>
        /// <param name="childColumnName">Child column name.</param>
        public RelationChildColumnDescriptor(string parentColumnName, string childColumnName)
            : base(childColumnName)
        {
            this.parentColumnName = parentColumnName;
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new RelationChildColumnDescriptorCollection Collection
        {
            get
            {
                return (RelationChildColumnDescriptorCollection)base.Collection;
            }
        }

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public new RelationChildColumnDescriptor Clone()
        {
            return (RelationChildColumnDescriptor)InternalClone();
        }

        /// <summary>
        /// Initializes this object and copies properties from another object.
        /// </summary>
        /// <param name="other">The source object.</param>
        /// <override/>
        public override void InitializeFrom(SortColumnDescriptor other)
        {
            RelationChildColumnDescriptor fd = (RelationChildColumnDescriptor)other;
            this.ParentColumnName = fd.ParentColumnName;
            base.InitializeFrom(other);
        }

        /// <summary>Determines if the specified object is equivalent to the current object.</summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if both objects are equal; False otherwise.</returns>
        /// <override/>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        string parentColumnName;

        /// <summary>
        /// The field name of the column in the parent table that establishes the relation between the two tables.
        /// </summary>
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        public string ParentColumnName
        {
            get
            {
                return parentColumnName;
            }

            set
            {
                if (parentColumnName != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ParentColumnName"));
                    parentColumnName = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ParentColumnName"));
                }
            }
        }
    }

    /// <summary>
    /// Enumerator class for <see cref="RelationChildColumnDescriptor"/> elements of a <see cref="RelationChildColumnDescriptorCollection"/>.
    /// </summary>
    public class RelationChildColumnDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        RelationChildColumnDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public RelationChildColumnDescriptorCollectionEnumerator(RelationChildColumnDescriptorCollection collection)
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
        public RelationChildColumnDescriptor Current
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