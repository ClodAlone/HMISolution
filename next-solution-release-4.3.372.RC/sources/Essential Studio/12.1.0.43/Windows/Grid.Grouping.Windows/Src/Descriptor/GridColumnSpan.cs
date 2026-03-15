//-------------------------------------------------------------------------------------------------
// <copyright file="GridColumnSpan.cs" company="syncfusion">
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
using Syncfusion.Windows.Forms.Grid;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    #region GridColumnSpanDescriptor

    /// <summary>
    /// <see cref="GridColumnSpanDescriptor"/> provides information
    /// about a column that can span multiple grid rows or columns. <para/>
    /// GridColumnSpanDescriptors are managed by the <see cref="GridColumnSpanDescriptorCollection"/> that
    /// is returned by the <see cref="GridColumnSetDescriptor.ColumnSpans"/> property
    /// of a <see cref="GridColumnSetDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(GridColumnSpanDescriptorTypeConverter))]
    public class GridColumnSpanDescriptor : DescriptorBase, IStandardValuesProvider, ICloneable
    {
        #region Fields

        string name;
        GridRangeInfo range;

        #endregion

        #region CtorWithName

        /// <overload>
        /// Initializes a new column span.
        /// </overload>
        /// <summary>
        /// Initializes a new empty column span.
        /// </summary>
        public GridColumnSpanDescriptor()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new column span with a column name.
        /// </summary>
        public GridColumnSpanDescriptor(string name)
        {
            this.name = name;
            range = GridRangeInfo.Cells(0, 0, 0, 0);
        }

        /// <summary>
        /// Initializes a new column span with a column name and range that specifies the cells the
        /// column covers.
        /// </summary>
        public GridColumnSpanDescriptor(string name, string range)
        {
            this.name = name;
            this.range = GridRangeInfo.Parse(range);
        }

        #endregion

        /// <override/>
        /// <summary>Returns the name of the descriptor.</summary>
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
            }

            base.Dispose(disposing);
        }

        #region ParentCollection

        GridColumnSpanDescriptorCollection collection;

        internal void SetCollection(GridColumnSpanDescriptorCollection collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridColumnSpanDescriptorCollection Collection
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

        /// <summary>
        /// The GridColumnSet that this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridColumnSetDescriptor ColumnSet
        {
            get
            {
                return collection == null ? null : collection.ColumnSet;
            }
        }

        #endregion

        #region PropertyChange

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

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

        #endregion

        #region Copy

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public GridColumnSpanDescriptor Clone()
        {
            GridColumnSpanDescriptor cd = new GridColumnSpanDescriptor();
            cd.InitializeFrom(this);
            return cd;
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridColumnSpanDescriptor other)
        {
            Name = other.name;
            Range = other.Range;
        }

        #endregion

        #region Equals

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
            else if (!(obj is GridColumnSpanDescriptor))
            {
                return false;
            }

            return Equals((GridColumnSpanDescriptor)obj);
        }

        bool Equals(GridColumnSpanDescriptor other)
        {
            return other.name == name
                && other.range == range;
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

        #endregion

        #region Name

        bool inSetName = false;

        ICollection IStandardValuesProvider.GetStandardValues(PropertyDescriptor pd)
        {
            ArrayList al = new ArrayList();
            foreach (GridColumnDescriptor cd in TableDescriptor.Columns)
            {
                al.Add(cd.Name);
            }

            return al;
        }

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool InSetName
        {
            get
            {
                return inSetName;
            }
        }

        /// <summary>
        /// The name of a <see cref="GridColumnDescriptor"/>.
        /// </summary>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [DefaultValue("")]
        [Description("The name of a GridColumnDescriptor."),
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
                    inSetName = true;
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    name = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                    inSetName = false;
                }
            }
        }

        #endregion

        #region Range

        /// <summary>
        /// The range that the column spans. Row and columns are zero based, e.g. R0C0:R0C1 indicates
        /// the column spans two columns at the top.
        /// </summary>
        [TypeConverter(typeof(GridRangeInfoConverter))]
        [System.Xml.Serialization.XmlIgnore]
        [Description("The range that the column spans. Row and columns are zero based, e.g.R0C0:R0C1 indicates the column spans two columns at the top."),
        Category("TableDescriptors")]
        public GridRangeInfo Range
        {
            get
            {
                return range;
            }

            set
            {
                if (range != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Range"));
                    range = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Range"));
                }
            }
        }

        /// <summary>
        /// Determines if the range was modified (not empty).
        /// </summary>
        /// <returns>True if name is modified.</returns>
        public bool ShouldSerializeRange()
        {
            return !range.IsEmpty;
        }

        /// <summary>
        /// Resets the range to an empty range.
        /// </summary>
        public void ResetRange()
        {
            Range = GridRangeInfo.Empty;
        }

        // Need to serialized as text for XML serialization to work properly.

        /// <internalonly/>
        /// <summary>Used internally.</summary>
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

        #endregion
    }

    #endregion

    #region TypeConverter

    /// <summary>
    /// The type converter for <see cref="GridColumnSpanDescriptor"/> objects. <see cref="GridColumnSpanDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class GridColumnSpanDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridColumnSpanDescriptorTypeConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Determines whether the current object can be converted to the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">The type you want to convert the object to.</param>
        /// <returns>True if this conversion is allowed.</returns>
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
        /// <param name="destinationType">Type to which the object should be converted to.</param>
        /// <returns>Converted object.</returns>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(InstanceDescriptor))
            {
                ////System.Reflection.ConstructorInfo constructorInfo = value.GetType().GetConstructor(new Type[]{});

                GridColumnSpanDescriptor columnSpan = (GridColumnSpanDescriptor)value;
                Type type = typeof(GridColumnSpanDescriptor);
                return new InstanceDescriptor(
                    type.GetConstructor(new Type[] { typeof(string), typeof(string) }),
                    new object[] { columnSpan.Name, columnSpan.Range.ToString() },
                    true);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } //// end of method ConvertTo

        /// <override/>
        /// <summary>
        /// Gets a collection of properties for the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Value specifying the type.</param>
        /// <param name="attributes">An array of System.Attribute objects that will be used as a filter.</param>
        /// <returns>A list of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "Range",
            };

            return pds.Sort(atts);
        }
    }

    #endregion

    #region GridColumnSpanDescriptorCollection

    /// <summary>
    /// A collection of <see cref="GridColumnSpanDescriptor"/> with information
    /// about columns that can spread multiple grid rows or columns. <para/>
    /// An instance of this collection is returned by the <see cref="GridColumnSetDescriptor.ColumnSpans"/> property
    /// of a <see cref="GridColumnSetDescriptor"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    public class GridColumnSpanDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        ArrayList inner = new ArrayList();
        internal int version;
        internal bool insideCollectionEditor = false;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static readonly GridColumnSpanDescriptorCollection Empty = new GridColumnSpanDescriptorCollection((GridColumnSetDescriptor)null);

        #region ctor

        /// <overload>
        /// Initializes a new empty collection.
        /// </overload>
        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public GridColumnSpanDescriptorCollection()
        {
            this.owner = null;
        }

        internal GridColumnSpanDescriptorCollection(GridColumnSetDescriptor owner)
        {
            this.owner = owner;
        }

        internal GridColumnSpanDescriptorCollection(GridColumnSpanDescriptor[] columnSpanDescriptors)
        {
            this.AddRange(columnSpanDescriptors);
        }

        #endregion

        #region Owner

        GridColumnSetDescriptor owner = null;

        /// <summary>
        /// The <see cref="GridColumnSetDescriptor"/> this collection belongs to.
        /// </summary>
        public GridColumnSetDescriptor ColumnSet
        {
            get
            {
                return owner;
            }
        }

        internal void SetOwner(GridColumnSetDescriptor owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// The <see cref="GridTableDescriptor"/> this collection belongs to.
        /// </summary>
        public GridTableDescriptor TableDescriptor
        {
            get
            {
                return owner == null ? null : owner.TableDescriptor;
            }
        }

        #endregion

        #region Initialize

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
                if (Switches.GroupingGrid.TraceVerbose)
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
            InitializeFrom((GridColumnSpanDescriptorCollection)other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridColumnSpanDescriptorCollection other)
        {
            int i;
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
        }

        #endregion

        #region Clone

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public GridColumnSpanDescriptorCollection Clone()
        {
            GridColumnSpanDescriptorCollection coll = new GridColumnSpanDescriptorCollection(owner);
            coll.inner = new ArrayList();
            coll.insideCollectionEditor = insideCollectionEditor;
            coll.version = version + 1000;
            int count = Count;
            GridColumnSpanDescriptor[] columnSpanDescriptors = new GridColumnSpanDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                coll.inner.Add(this[n].Clone());
                coll[n].SetCollection(coll);
            }

            return coll;
        }

        #endregion

        #region Equals

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
            ////            TraceUtil.TraceCurrentMethodInfo(this);
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is GridColumnSpanDescriptorCollection))
            {
                return false;
            }

            return Equals((GridColumnSpanDescriptorCollection)obj);
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
                return version;
            }
        }

        bool Equals(GridColumnSpanDescriptorCollection other)
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

        #endregion

        #region Item

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public GridColumnSpanDescriptor this[int index]
        {
            get
            {
                return (GridColumnSpanDescriptor)inner[index];
            }

            set
            {
                ////               TraceUtil.TraceCurrentMethodInfo(this, index, value);
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
#if ASPNET
        public GridColumnSpanDescriptor GetColumnSpanDescriptor(string name){return this[name];}
        public void SetColumnSpanDescriptor(string name, GridColumnSpanDescriptor value){this[name] = value;}

        // Designer ser. doesn't like overloaded accessors.
        internal
#else
        public
#endif
 GridColumnSpanDescriptor this[string name]
        {
            get
            {
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (GridColumnSpanDescriptor)inner[index];
            }

            set
            {
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
            for (int n = 0; n < Count; n++)
            {
                if (this[n].Name == name)
                {
                    return n;
                }
            }

            return -1;
        }

        #endregion

        #region StronglyTypedList

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="columnSpanDescriptors">The array with elements that should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(GridColumnSpanDescriptor[] columnSpanDescriptors)
        {
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this, columnSpanDescriptors.Length);
            }
#else
            ;
#endif
            this.inner.AddRange(columnSpanDescriptors);
            for (int n = 0; n < columnSpanDescriptors.Length; n++)
            {
                columnSpanDescriptors[n].SetCollection(this);
            }
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(GridColumnSpanDescriptor value)
        {
            if (value == null)
            {
                return false;
            }

            return inner.Contains(value);
        }

        /// <summary>
        /// Searches for the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
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
        public int IndexOf(GridColumnSpanDescriptor value)
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
            return Find(name);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(GridColumnSpanDescriptor[] array, int index)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this);
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        GridColumnSpanDescriptorCollection SyncRoot
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
        public GridColumnSpanDescriptorCollectionEnumerator GetEnumerator()
        {
            return new GridColumnSpanDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridColumnSpanDescriptor value)
        {
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
        public void Remove(GridColumnSpanDescriptor value)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this, value.Name);
            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.Remove(value);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(GridColumnSpanDescriptor value)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this, value.Name);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
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
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="name">The column descriptor name of the element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name)
        {
            return Add(new GridColumnSpanDescriptor(name));
        }

        /// <summary>
        /// Removes the specified descriptor element from the collection.
        /// </summary>
        /// <param name="name">The column descriptor name of the element to remove from the collection. If the name is not found
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

        #endregion

        #region TypeNeutral IList Members

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
            ////            TraceUtil.TraceCurrentMethodInfo(this);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            inner.Clear();
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
        }

        /// <summary>
        /// Determines if the collection is Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
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
        public int Count
        {
            get
            {
                return inner.Count;
            }
        }

        #endregion

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

        internal void RaisePropertyItemChanged(GridColumnSpanDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                foreach (GridColumnSpanDescriptor sc in this)
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
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, IndexOf(column), column, e.PropertyName, e));
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

        internal void RaisePropertyItemChanging(GridColumnSpanDescriptor column, DescriptorPropertyChangedEventArgs e)
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
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, IndexOf(column), column, e.PropertyName, e));
            }
        }

        #endregion

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
                this[index] = (GridColumnSpanDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridColumnSpanDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((GridColumnSpanDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridColumnSpanDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridColumnSpanDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((GridColumnSpanDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridColumnSpanDescriptor[])array, index);
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

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return String.Format("GridColumnSpanDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        void SuggestName(GridColumnSpanDescriptor value)
        {
            if (value.Name == null || value.Name.Length == 0)
            {
                PropertyDescriptorCollection pdc = this.TableDescriptor.ItemProperties;
                if (pdc.Count > this.Count)
                {
                    foreach (PropertyDescriptor pd in pdc)
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
    /// Enumerator class for <see cref="GridColumnSpanDescriptor"/> elements of a <see cref="GridColumnSpanDescriptorCollection"/>.
    /// </summary>
    public class GridColumnSpanDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridColumnSpanDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GridColumnSpanDescriptorCollectionEnumerator(GridColumnSpanDescriptorCollection collection)
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
        public GridColumnSpanDescriptor Current
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
}

