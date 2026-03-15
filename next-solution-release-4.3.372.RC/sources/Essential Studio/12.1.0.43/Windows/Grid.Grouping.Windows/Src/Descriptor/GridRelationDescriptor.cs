//-------------------------------------------------------------------------------------------------
// <copyright file="GridRelationDescriptor.cs" company="syncfusion">
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Text;
using System.Reflection;
using System.Data;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Grouping;

#if ASPNET
using System.Web.UI;
using System.Xml.Serialization;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// A collection of <see cref="GridRelationDescriptor"/> elements with constraints for a relation
    /// between two tables and schema information of child tables. An instance of this
    /// collection is returned by the <see cref="GridTableDescriptor.Relations"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    public class GridRelationDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        #region IInsideCollectionEditorProperty Members

        void IInsideCollectionEditorProperty.InitializeFrom(object value)
        {
            InitializeFrom((GridRelationDescriptorCollection) value);
        }

        /// <summary>
        /// Copies settings from another collection.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridRelationDescriptorCollection other)
        {
            inner.InitializeFrom(other.inner);
        }

        /// <summary>
        /// Copies settings from another collection.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(RelationDescriptorCollection other)
        {
            inner.InitializeFrom(other);
        }

        bool insideCollectionEditor;

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
                insideCollectionEditor = value;
            }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public object Clone()
        {
            return new GridRelationDescriptorCollection(inner.Clone());
        }

        #endregion

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
                return inner.ShouldPopulate;
            }

            set
            {
                inner.ShouldPopulate = value;
            }
        }

        /// <summary>
        /// When called the ShouldPopulate property will be set true temporarily until
        /// the next EnsureInitialized call and then be reset again to optimize subsequent lookups.
        /// The Engine calls this method when schema changes occured (PropertyChanged was raised).
        /// </summary>
        public void EnableOneTimePopulate()
        {
            inner.EnableOneTimePopulate();
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
            GridRelationDescriptorCollection value = obj as GridRelationDescriptorCollection;
            return inner.Equals(value.inner);
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
            return inner.GetHashCode();
        }

        RelationDescriptorCollection inner;

        /// <summary>
        /// The inner <see cref="RelationDescriptorCollection"/>
        /// </summary>
        public RelationDescriptorCollection Inner
        {
            get
            {
                return inner;
            }
        }

        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public GridRelationDescriptorCollection()
        {
            inner = new RelationDescriptorCollection();
        }

        internal GridRelationDescriptorCollection(RelationDescriptorCollection inner)
        {
            this.inner = inner;
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="values">The array whose elements should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(GridRelationDescriptor[] values)
        {
            inner.AddRange(values);
        }

        /// <summary>
        /// Disposes of the object and collection items.
        /// </summary>
        public void Dispose()
        {
            this.inner.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public int NestedCount
        {
            get
            {
                return inner.NestedCount;
            }
        }

        /// <summary>
        /// Determines if the collection is Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return inner.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public GridRelationDescriptor this[int index]
        {
            get
            {
                return (GridRelationDescriptor) inner[index];
            }

            set
            {
                inner[index] = value;
            }
        }

        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
#if ASPNET
        public GridRelationDescriptor GetRelationDescriptor(string name){return this[name];}
        public void SetRelationDescriptor(string name, GridRelationDescriptor value){this[name] = value;}

        // Designer ser. doesn't like overloaded accessors.
        internal
#else
        public
#endif
            GridRelationDescriptor this[string name]
        {
            get
            {
                return (GridRelationDescriptor) inner[name];
            }

            set
            {
                inner[name] = value;
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            inner.RemoveAt(index);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridRelationDescriptor value)
        {
            inner.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified descriptor element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public void Remove(GridRelationDescriptor value)
        {
            inner.Remove(value);
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public bool Contains(GridRelationDescriptor value)
        {
            return inner.Contains(value);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            inner.Clear();
        }

        /// <summary>
        /// Resets the collection to its default state. If the collection is bound to a <see cref="GridTableDescriptor"/>,
        /// the collection will autopopulate itself the next time an item inside the collection is accessed.
        /// </summary>
        public void Reset()
        {
            inner.Reset();
        }

        /// <summary>
        /// Resets the collection to its default state, autopopulates it, and marks it
        /// as modified. Call this method if you want to load the default items for the collection and then
        /// modify it (e.g. remove members from the auto-populated list).
        /// </summary>
        public void LoadDefault()
        {
            inner.LoadDefault();
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(GridRelationDescriptor value)
        {
            return inner.IndexOf(value);
        }

        /// <summary>
        /// Searches for the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>The zero-based index of the occurrence of the element with matching name within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(string name)
        {
            return inner.IndexOf(name);
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(GridRelationDescriptor value)
        {
            return inner.Add(value);
        }

        /// <summary>
        /// Returns False since this collection has no fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return inner.IsFixedSize;
            }
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return inner.IsSynchronized;
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
                return ((ICollection) inner).Count;
            }
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(Array array, int index)
        {
            inner.CopyTo((GridRelationDescriptor[]) array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null; ////inner.SyncRoot;
            }
        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading of the data in the collection.
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public IEnumerator GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        #region IList Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (GridRelationDescriptor) value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridRelationDescriptor) value);
        }

        void IList.Remove(object value)
        {
            Remove((GridRelationDescriptor) value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridRelationDescriptor) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridRelationDescriptor) value);
        }

        int IList.Add(object value)
        {
            return Add((GridRelationDescriptor) value);
        }

        #endregion

        #region ICustomTypeDescriptor
        System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes()
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
    /// A RelationDescriptor defines constraints for a relation
    /// between two tables and schema information of child tables.
    /// RelationDescriptors are managed by the <see cref="GridRelationDescriptorCollection"/> which
    /// is returned by the <see cref="GridTableDescriptor.Relations"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(GridRelationDescriptorTypeConverter))]
    public class GridRelationDescriptor : RelationDescriptor
    {
        /// <summary>
        /// Initializes a new empty relation descriptor.
        /// </summary>
        /// <summary>
        /// Initializes a new empty relation descriptor.
        /// </summary>
        public GridRelationDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new relation descriptor with the given name.
        /// </summary>
        /// <param name="relationName">Relation name.</param>
        public GridRelationDescriptor(string relationName)
            : base(relationName)
        {
        }

        /// <override/>
        /// <summary>Creates a table descriptor for the child table.</summary>
        /// <returns>Child table descriptor.</returns>
        public override TableDescriptor CreateChildTableDescriptor()
        {
            if (this.ParentTableDescriptor == null || this.ParentTableDescriptor.Engine == null)
            {
                return new GridTableDescriptor(this);
            }

            return this.ParentTableDescriptor.Engine.CreateTableDescriptor(this);
        }

        /// <summary>
        /// The parent TableDescriptor this descriptor belongs to if this object is the child table
        /// descriptor in a relation.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor) base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Creates an exact copy of this object.
        /// </summary>
        /// <returns>A duplicate of this object.</returns>
        public override RelationDescriptor Clone()
        {
            GridRelationDescriptor rd = new GridRelationDescriptor();
            rd.CopyMembersFrom(this);
            return rd;
        }

#if ASPNET
        /// <summary>
        /// The TableDescriptor that describes child table.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [XmlIgnore()]
        [Description("The TableDescriptor that describes child table.")]
        public GridTableDescriptor GridChildTableDescriptor
        {
            get
            {
                return (GridTableDescriptor) base.ChildTableDescriptor;
            }
            set
            {
                base.ChildTableDescriptor.InitializeFrom(value);
            }
        }
        
#endif

        /// <summary>
        /// The TableDescriptor that describes child table.
        /// </summary>
#if ASPNET
        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Documentation.DocumentationExclude()]
#else
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
#endif
        [System.Xml.Serialization.XmlElement("GridChildTableDescriptor")]
        public new GridTableDescriptor ChildTableDescriptor
        {
            get
            {
                return (GridTableDescriptor) base.ChildTableDescriptor;
            }

            set
            {
                ChildTableDescriptor.InitializeFrom(value);
            }
        }
    }

    #region TypeConverter
    /// <summary>
    /// The type converter for <see cref="GridRelationDescriptor"/> objects. <see cref="GridRelationDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// ConvertTo method and adds support for design-time code serialization.
    /// </summary>
    public class GridRelationDescriptorTypeConverter : RelationDescriptorTypeConverter
    {
        /// <override/>
        /// <summary>
        /// Determines whether this object can be converted tothe specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">The type you want to convert the object to.</param>
        /// <returns>True if this conversion is possible.</returns>
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
        /// <summary>
        /// Converts the given value to the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">Current culture information.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(InstanceDescriptor))
            {
                ////System.Reflection.ConstructorInfo constructorInfo = value.GetType().GetConstructor(new Type[]{});

                GridRelationDescriptor relation = (GridRelationDescriptor) value;
                Type type = typeof(GridRelationDescriptor);

////                GridTableDescriptor parentTableDescriptor = (GridTableDescriptor) relation.ParentTableDescriptor;
////                if (parentTableDescriptor != null)
////                {
////                    if (parentTableDescriptor.ParentRelation != null)
////                        return new InstanceDescriptor(type.GetConstructor(
////                            new Type[] { typeof(GridRelationDescriptor) }
////                            ),
////                            new object[] { (GridRelationDescriptor) parentTableDescriptor.ParentRelation }
////                            , false
////                            );
////
////                    else if (parentTableDescriptor.Engine != null)
////                    {
////                        GridGroupingControl groupingControl = parentTableDescriptor.Engine.ParentControl as GridGroupingControl;
////
////                        if (groupingControl != null)
////                            return new InstanceDescriptor(type.GetConstructor(
////                                new Type[] { typeof(GridGroupingControl) }
////                                ),
////                                new object[] { groupingControl }
////                                , false
////                                );
////                    }
////
////                }

                return new InstanceDescriptor(type.GetConstructor(new Type[0]), null, false);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    #endregion
}
