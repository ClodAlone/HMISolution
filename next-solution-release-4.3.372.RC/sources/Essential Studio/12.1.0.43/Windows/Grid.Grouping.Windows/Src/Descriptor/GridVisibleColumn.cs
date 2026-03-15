//-------------------------------------------------------------------------------------------------
// <copyright file="GridVisibleColumn.cs" company="syncfusion">
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
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;

#if ASPNET
using System.Web.UI;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    #region GridVisibleColumnDescriptor

    /// <summary>
    /// GridVisibleColumnDescriptor references a <see cref="GridColumnDescriptor"/> or <see cref="GridColumnSetDescriptor"/>.
    /// The order of GridVisibleColumnDescriptors in the <see cref="GridTableDescriptor.VisibleColumns"/> collection defines
    /// the left to right order of columns shown in the grid.
    /// <para/>
    /// Columns are managed by the <see cref="GridVisibleColumnDescriptorCollection"/> that
    /// is returned by the <see cref="GridTableDescriptor.VisibleColumns"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(GridVisibleColumnDescriptorTypeConverter))]
    public class GridVisibleColumnDescriptor : DescriptorBase, IStandardValuesProvider, ICloneable
    {
        #region Fields

        string name;

        #endregion

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                this.collection = null;
            }

            base.Dispose(disposing);
        }

        /// <override/>
        /// <summary>Determines if this object was modified.</summary>
        /// <returns>returns the boolean value false.</returns>
        public override bool ShouldSerialize()
        {
            return false;
        }

        /// <override/>
        /// <summary>Gets the descriptor name.</summary>
        /// <returns>Descriptor name.</returns>
        public override string GetName()
        {
            return Name;
        }

        #region CtorWithName

        /// <overload>
        /// Initializes a new column.
        /// </overload>
        /// <summary>
        /// Initializes a new empty column.
        /// </summary>
        public GridVisibleColumnDescriptor()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new column with a name.
        /// </summary>
        /// <param name="name">Descriptor name.</param>
        public GridVisibleColumnDescriptor(string name)
        {
            this.name = name;
        }

        #endregion

        #region ParentCollection

        GridVisibleColumnDescriptorCollection collection;

        internal void SetCollection(GridVisibleColumnDescriptorCollection collection)
        {
            this.collection = collection;
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridVisibleColumnDescriptorCollection Collection
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
        public GridVisibleColumnDescriptor Clone()
        {
            GridVisibleColumnDescriptor cd = new GridVisibleColumnDescriptor();
            cd.InitializeFrom(this);
            return cd;
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridVisibleColumnDescriptor other)
        {
            Name = other.name;
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
            else if (!(obj is GridVisibleColumnDescriptor))
            {
                return false;
            }

            return Equals((GridVisibleColumnDescriptor) obj);
        }

        bool Equals(GridVisibleColumnDescriptor other)
        {
            return other.name == name;
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
            return collection.GetStandardValues();
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InSetName
        {
            get
            {
                return inSetName;
            }
        }

        /// <summary>
        /// The mapping for this column. You should specify which <see cref="GridColumnDescriptor"/> (by it's Name property)
        /// or <see cref="GridColumnSetDescriptor"/> you want to display in the grid at this column.
        /// </summary>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of this column.")]
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

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return GetType().Name + " { " + Name + " }";
        }
    }
    #endregion

    #region TypeConverter

    /// <summary>
    /// The type converter for <see cref="GridVisibleColumnDescriptor"/> objects. <see cref="GridVisibleColumnDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class GridVisibleColumnDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridVisibleColumnDescriptorTypeConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Determines if the current object can be converted to the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">The type you want the object to convert to.</param>
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
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
                && (value is GridVisibleColumnDescriptor))
            {
                GridVisibleColumnDescriptor typename = (GridVisibleColumnDescriptor)value;
                System.Type[] args;
                args = new System.Type[1];
                args[0] = typeof(string);

                System.Reflection.ConstructorInfo constructorInfo;
                constructorInfo = typeof(GridVisibleColumnDescriptor).GetConstructor(args);
                if (constructorInfo != null)
                {
                    object[] argValues;
                    argValues = (object[])new System.Object[1];
                    argValues[0] = typename.Name;
                    return (object)new InstanceDescriptor(constructorInfo, argValues);
                }
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
            };

            return pds.Sort(atts);
        }
    }

    #endregion

    #region GridVisibleColumnDescriptorCollection

    /// <summary>
    /// A collection of <see cref="GridVisibleColumnDescriptor"/> columns each referencing
    /// a <see cref="GridColumnDescriptor"/> or <see cref="GridColumnSetDescriptor"/>.
    /// The order of GridVisibleColumnDescriptors in the <see cref="GridTableDescriptor.VisibleColumns"/> collection defines
    /// the left to right order of columns shown in the grid.
    /// <para/>
    /// An instance of this collection
    /// is returned by the <see cref="GridTableDescriptor.VisibleColumns"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class GridVisibleColumnDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        ArrayList inner = new ArrayList();
        internal int version;
        internal bool insideCollectionEditor = false;
        bool autoPopulated = false;
        bool modified = false;
        bool inReset = false;
        internal bool shouldPopulate = false;
        internal bool columnWidthsDirty = true;
        int columnWidthsEngineAppearanceVersion = -1;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static readonly GridVisibleColumnDescriptorCollection Empty = new GridVisibleColumnDescriptorCollection();

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
            if (!Engine.InInitializeFrom)
            {
                this.shouldPopulate = true;
            }
        }

        GridEngine Engine
        {
            get
            {
                return this.tableDescriptor != null ? this.tableDescriptor.Engine : null;
            }
        }

        #region ctor

        /// <overload>
        /// Initializes a new empty collection.
        /// </overload>
        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public GridVisibleColumnDescriptorCollection()
        {
            this.tableDescriptor = null;
        }

        internal GridVisibleColumnDescriptorCollection(GridTableDescriptor tableDescriptor)
        {
            SetOwner(tableDescriptor);
        }

        internal GridVisibleColumnDescriptorCollection(GridVisibleColumnDescriptor[] columnDescriptors)
        {
            this.AddRange(columnDescriptors);
        }

        #endregion

        #region Owner

        GridTableDescriptor tableDescriptor = null;

        internal void SetOwner(GridTableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
        }

        /// <summary>
        /// The table descriptor this collection belongs to.
        /// </summary>
        public GridTableDescriptor TableDescriptor
        {
            get
            {
                return tableDescriptor;
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
            InitializeFrom((GridVisibleColumnDescriptorCollection) other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridVisibleColumnDescriptorCollection other)
        {
            bool savedshouldPopulateThis = shouldPopulate;
            bool savedshouldPopulateOther = other.shouldPopulate;
            this.shouldPopulate = false;
            other.shouldPopulate = false;
            try
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
            finally
            {
                this.shouldPopulate = true;
                if (!savedshouldPopulateThis)
                {
                    this.disableShouldPopulate = true;
                }

                other.shouldPopulate = savedshouldPopulateOther;
            }
        }

        /// <summary>
        /// Resets the collection to its default state. If the collection is bound to a <see cref="TableDescriptor"/>,
        /// the collection will auto-populate itself the next time an item inside the collection is accessed.
        /// </summary>
        public void Reset()
        {
            if (this.modified || this.inner.Count > 0)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inReset = true;
                isReset = true;
                autoPopulated = false;
                modified = false;
                inner.Clear();
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inReset = false;
            }

            isReset = true;
            modified = false;
            if (!shouldPopulate)
            {
                this.shouldPopulate = true;
                this.disableShouldPopulate = true;
            }
        }

        internal ICollection GetStandardValues()
        {
            ArrayList al = new ArrayList();
            SortedList sl = new SortedList();
            foreach (GridColumnSetDescriptor cd in TableDescriptor.ColumnSets)
            {
                al.Add(cd.Name);
                foreach (GridColumnSpanDescriptor sd in cd.ColumnSpans)
                {
                    sl.Add(sd.Name, sd);
                }
            }

            foreach (GridColumnDescriptor cd in TableDescriptor.Columns)
            {
                if (!sl.ContainsKey(cd.Name))
                {
                    al.Add(cd.Name);
                }
            }

            return al;
        }

        bool inEnsureInitialized = false;
        bool isReset = true;
        int columnsVersion = -1;
        int columnSetsVersion = -1;

        /// <override/>
        protected virtual void EnsureInitialized(bool populate)
        {
            //// || tableDescriptor.Engine == null || tableDescriptor.Engine.Initializing)
            if (inEnsureInitialized || !shouldPopulate || this.tableDescriptor == null)
            {
                return;
            }

            if (disableShouldPopulate)
            {
                shouldPopulate = false;
            }

            GridEngine engine = this.tableDescriptor.Engine;

            if (this.tableDescriptor != null && engine != null && columnWidthsEngineAppearanceVersion != engine.AppearanceVersion)
            {
                columnWidthsDirty = true;
            }

            //// compare versions
            if (this.columnSetsVersion != this.tableDescriptor.ColumnSets.Version
                || this.columnsVersion != this.tableDescriptor.Columns.Version)
            {
                this.version++;
                this.columnsVersion = this.tableDescriptor.Columns.Version;
                this.columnSetsVersion = this.tableDescriptor.ColumnSets.Version;

                if (!this.modified && this.autoPopulated)
                {
                    isReset = true;
                }
            }

            if (populate && !modified && isReset)
            {
                TraceUtil.TraceCalledFromIf(Switches.AutoPopulate.TraceVerbose, 10, version);
                isReset = false;
                inEnsureInitialized = true;
                columnWidthsDirty = true;
                widthColumns = null;

                try
                {
                    isReset = false;
                    this.Clear();
                    foreach (string s in GetStandardValues())
                    {
                        GridVisibleColumnDescriptor column = new GridVisibleColumnDescriptor(s);
                        column.SetCollection(this);
                        GridQueryAddVisibleColumnEventArgs e = new GridQueryAddVisibleColumnEventArgs(this.tableDescriptor, column);
                        if (Engine != null)
                        {
                            Engine.RaiseQueryAddVisibleColumn(e);
                        }

                        if (!e.Cancel)
                        {
                            Add(column);
                        }
#if DEBUG
                        if (Switches.AutoPopulate.TraceVerbose)
                        {
                            TraceUtil.TraceCurrentMethodInfo("Add", column);
                        }
#else
                        ;
#endif
                    }

                    autoPopulated = true;
                    modified = false;
                    ////OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                }
                finally
                {
                    inEnsureInitialized = false;
                }
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public GridVisibleColumnDescriptorCollection Clone()
        {
            GridVisibleColumnDescriptorCollection coll = new GridVisibleColumnDescriptorCollection(tableDescriptor);
            coll.inner = new ArrayList();
            coll.insideCollectionEditor = insideCollectionEditor;
            coll.version = version+1000;
            int count = Count;
            GridVisibleColumnDescriptor[] columnSpanDescriptors = new GridVisibleColumnDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                coll.inner.Add(this[n].Clone());
                coll[n].SetCollection(coll);
            }

            coll.autoPopulated = this.autoPopulated;
            coll.modified = this.modified;
            coll.columnWidthsDirty = true;
            coll.widthColumns = null;

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
            else if (!(obj is GridVisibleColumnDescriptorCollection))
            {
                return false;
            }

            return Equals((GridVisibleColumnDescriptorCollection) obj);
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
        /// collection or an element within the collection was modified. When getting the version,
        /// <see cref="EnsureInitialized"/> is called to ensure the collection is auto-populated
        /// if needed.
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

        bool Equals(GridVisibleColumnDescriptorCollection other)
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
        /// Marks the collection as modified and avoids auto-population.
        /// </summary>
        public void Modify()
        {
            this.modified = true;
        }

        /// <summary>
        /// Resets the collection to its default state, autopopulates it, and marks it
        /// as modified. Call this method if you want to load the default items for the collection and then
        /// modify them (e.g. remove members from the auto-populated list).
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// gridGroupingControl1.TableDescriptor.VisibleColumns.LoadDefault();
        /// gridGroupingControl1.TableDescriptor.VisibleColumns.Remove("MyChildTable.ForeignCategoryID");
        /// </code>
        /// </example>
        public void LoadDefault()
        {
            if (this.IsModified)
            {
                Reset();
            }

            this.EnsureInitialized(true);
            this.modified = true;
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

            set
            {
                modified = value;
            }
        }

        #endregion

        #region Item

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public GridVisibleColumnDescriptor this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (GridVisibleColumnDescriptor) inner[index];
            }

            set
            {
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
#if ASPNET
        public GridVisibleColumnDescriptor GetVisibleColumnDescriptor(string name){return this[name];}
#endif

#if ASPNET
        // Designer ser. doesn't like overloaded accessors.
        internal
#else
        public
#endif
            GridVisibleColumnDescriptor this[string name]
        {
            get
            {
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (GridVisibleColumnDescriptor) inner[index];
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
            if (!modified)
            {
                EnsureInitialized(true);
            }

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
        /// <param name="columnDescriptors">The array with elements that should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(GridVisibleColumnDescriptor[] columnDescriptors)
        {
            foreach (GridVisibleColumnDescriptor cd in columnDescriptors)
            {
                Add(cd);
            }
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
        /// <para/>
        /// The method calls <see cref="EnsureInitialized"/>.
        /// </remarks>
        public bool Contains(GridVisibleColumnDescriptor value)
        {
            if (!modified)
            {
                EnsureInitialized(true);
            }

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
        /// <returns>The zero-based index of the occurrence of the element with matching name within the entire collection, if found; otherwise, -1.</returns>
        public bool Contains(string name)
        {
            return Find(name) != -1;
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public int IndexOf(GridVisibleColumnDescriptor value)
        {
            if (!modified)
            {
                EnsureInitialized(true);
            }

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
        public void CopyTo(GridVisibleColumnDescriptor[] array, int index)
        {
            if (!modified)
            {
                EnsureInitialized(true);
            }

            int count = Count;
            for (int n = 0; n < count; n++)
            { 
                array[index+n] = this[n];
            }
        }

        GridVisibleColumnDescriptorCollection SyncRoot
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
        public GridVisibleColumnDescriptorCollectionEnumerator GetEnumerator()
        {
            return new GridVisibleColumnDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public void Insert(int index, GridVisibleColumnDescriptor value)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this, index, value.Name);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
            inner.Insert(index, value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="name">The name of the element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public void Insert(int index, string name)
        {
            Insert(index, new GridVisibleColumnDescriptor(name));
        }

        /// <summary>
        /// Moves an element within the collection.
        /// </summary>
        /// <param name="src">The original index of the element within the collection.</param>
        /// <param name="dest">The target index of the element within the collection </param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public void Move(int src, int dest)
        {
            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Move, src, null, null));
            object value = inner[src];
            inner.RemoveAt(src);
            inner.Insert(dest, value);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Move, dest, value, null));
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
        public void Remove(GridVisibleColumnDescriptor value)
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
        public int Add(GridVisibleColumnDescriptor value)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this, value.Name);

            //// Fixes defect 1368 - Setting the VisibleColumns in designer doesn't work. The 
            //// fix is to mark the collection modified when AddRange is called from within InitializeComponent
            //// code block.
            //// Also fixes issue that xml serialization did not work (check for Datasource == null fixes this)
            if (this.Engine == null
                || this.Engine.DataSource == null
                || (this.Engine.Initializing && this.inner.Count == 0))
            {
                this.modified = true;
            }

            // Changed Add behavior for 3.0.0.12 - user should call explicitly Clear()
            // if collection should be reset before adding fields.
            if (!this.inEnsureInitialized)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));

                this.EnsureInitialized(true);
                if (!this.modified && this.inner.Count > 0)
                {
                    if (Contains(value.Name) && Engine.VersionInfo.CompareTo("3.0.0.12") <= 0)
                    {
                        FieldDescriptorCollection.ShowAddRangeChangedWarning("VisibleColumns");
                    }
                }
            }

            //// If value is added with a name that already exist, replace 
            //// the old value with the new descriptor.
            int index = -1;
            if (value.Name != null && value.Name.Length > 0)
            {
                index = IndexOf(value.Name);
                if (index != -1)
                {
                    inner[index] = value;
                }
            }
            else 
            {
                SuggestName(value);
            }

            if (index == -1)
            {
                index = inner.Add(value);
            }

            value.SetCollection(this);

            if (!this.inEnsureInitialized)
            {
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            }

            return index;
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="name">The name of the element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name)
        {
            return Add(new GridVisibleColumnDescriptor(name));
        }

        /// <summary>
        /// Removes the specified descriptor element with the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the element to remove from the collection. If no element with that name is found
        /// in the collection, the method will do nothing.</param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public void Remove(string name)
        {
            if (!modified)
            {
                EnsureInitialized(true);
            }

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
            if (!modified)
            {
                EnsureInitialized(true);
            }

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
            tableDescriptor = null;

            foreach (DescriptorBase db in inner)
            {
                db.Dispose();
            }

            inner.Clear();
            inner = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            ////            TraceUtil.TraceCurrentMethodInfo(this);
            if (this.inner.Count > 0)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inner.Clear();
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }

            this.modified = true;
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
        /// <remarks>
        /// The method calls <see cref="EnsureInitialized"/>.
        /// </remarks>
        public int Count
        {
            get
            {
                if (!modified)
                {
                    EnsureInitialized(true);
                }

                return inner.Count;
            }
        }

        #endregion

        GridColumnDescriptor[] widthColumns;
        int[] widthFactor = null;

        /// <summary>
        /// Returns an array of columns that affect the width of the table in the grid.
        /// </summary>
        /// <param name="factor">An array in the same order as the returned array with
        /// width factors indicating the number of grid columns spanned by a column descriptor.</param>
        /// <returns>An array of columns that affect the width of this column set. Each column
        /// will also have a width factor returned through the <paramref name="factor"/> array.</returns>
        public GridColumnDescriptor[] GetWidthColumns(out int[] factor)
        {
            if (widthColumns != null)
            {
                // Fix issue in case GridColumnDescriptors are out of date and were disposed.
                // In such case do not reuse cached columns.
                if (widthColumns.Length == 0 || (widthColumns[0]!=null && !widthColumns[0].IsDisposed))
                {
                    factor = widthFactor;
                    return widthColumns;
                }
            }

            ArrayList columns = new ArrayList();
            ArrayList factors = new ArrayList();
            
            foreach (GridVisibleColumnDescriptor column in this)
            {
                columns.AddRange(this.GetWidthColumns(column.Name, out factor));
                factors.AddRange(factor);
            }

            factor = widthFactor = (int[]) factors.ToArray(typeof(int));
            widthColumns = (GridColumnDescriptor[]) columns.ToArray(typeof(GridColumnDescriptor));
            return widthColumns;
        }

        /// <summary>
        /// Returns an array of columns that affect the width of this table in the grid
        /// for a specifc column set only.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <param name="factor">The int array factor.</param>
        /// <returns>returns array of GridColumnDescriptor</returns>
        internal GridColumnDescriptor[] GetWidthColumns(string name, out int[] factor)
        {
            int n = this.TableDescriptor.ColumnSets.IndexOf(name);
            if (n != -1)
            {
                GridColumnSetDescriptor columnSet = this.TableDescriptor.ColumnSets[n];
                if (columnSet != null)
                {
                    return columnSet.GetWidthColumns(out factor);
                }
            }
            else
            {
                GridColumnDescriptor cd = this.TableDescriptor.Columns[name];
                factor = new int[] { 1 };
                return new GridColumnDescriptor[] { cd };
            }

            factor = new int[1];
            return new GridColumnDescriptor[] { null };
        }

        /// <summary>
        /// Occurs when the grid is recalculating the total horizontal width of the table inside the grid.
        /// </summary>
        public event CancelEventHandler TotalWidthRequest;

        /// <summary>
        /// Raises the <see cref="TotalWidthRequest"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnTotalWidthRequest(CancelEventArgs e)
        {
            if (Syncfusion.Grouping.Engine.HelpTracing)
            {
                TraceUtil.TraceCurrentMethodInfo(this, e);
            }

            if (TotalWidthRequest != null)
            {
                TotalWidthRequest(this, e);
            }
        }

        private int totalWidth;

        /// <summary>
        /// The total horizontal width of the table inside the grid.
        /// </summary>
        public int TotalWidth
        {
            get
            {
                GridEngine engine = this.tableDescriptor.Engine;
                if (this.columnWidthsDirty || (engine != null && columnWidthsEngineAppearanceVersion != engine.AppearanceVersion))
                {
                    totalWidth = 0;
                    CancelEventArgs e = new CancelEventArgs();
                    OnTotalWidthRequest(e);
                    if (e.Cancel)
                    {
                        return 0;
                    }

                    int[] factor;
                    GridColumnDescriptor[] columns = tableDescriptor.VisibleColumns.GetWidthColumns(out factor);

                    for (int n = 0; n < columns.Length; n++)
                    {
                        if (columns[n] != null)
                        {
                            totalWidth += columns[n].Width / factor[n];
                        }
                    }

                    columnWidthsDirty = false;
                    columnWidthsEngineAppearanceVersion = engine != null ? engine.AppearanceVersion : -1;
                }

                return this.totalWidth;
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
            columnWidthsDirty = true;
            widthColumns = null;

            if (!this.inEnsureInitialized && !this.inReset)
            {
                modified = true;
            }

            if (!this.inEnsureInitialized && !this.insideCollectionEditor)
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

        internal void RaisePropertyItemChanged(GridVisibleColumnDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            columnWidthsDirty = true;
            widthColumns = null;
            ////            if (e.PropertyName == "Name")
////            {
////                foreach (GridVisibleColumnDescriptor sc in this)
////                {
////                    if (sc != column && sc.Name == column.Name)
////                        throw new Exception(String.Format("Column '{0}': Duplicates are not allowed ", column.Name));
////                }
////            }

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
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, inner.IndexOf(column), column, e.PropertyName, e));
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.insideCollectionEditor && !this.inEnsureInitialized)
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

        internal void RaisePropertyItemChanging(GridVisibleColumnDescriptor column, DescriptorPropertyChangedEventArgs e)
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
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, inner.IndexOf(column), column, e.PropertyName, e));
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
                this[index] = (GridVisibleColumnDescriptor) value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridVisibleColumnDescriptor) value);
        }

        void IList.Remove(object value)
        {
            Remove((GridVisibleColumnDescriptor) value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridVisibleColumnDescriptor) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridVisibleColumnDescriptor) value);
        }

        int IList.Add(object value)
        {
            return Add((GridVisibleColumnDescriptor) value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridVisibleColumnDescriptor[]) array, index);
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
            return String.Format("GridVisibleColumnDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        void SuggestName(GridVisibleColumnDescriptor value)
        {
            if (value.Name == null || value.Name.Length == 0)
            {
                ICollection values = GetStandardValues();
                foreach (string name in values)
                {
                    if (IndexOf(name) == -1)
                    {
                        value.Name = name;
                        return;
                    }
                }
            }
        }

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
    /// Enumerator class for <see cref="GridVisibleColumnDescriptor"/> elements of a <see cref="GridVisibleColumnDescriptorCollection"/>.
    /// </summary>
    public class GridVisibleColumnDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridVisibleColumnDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GridVisibleColumnDescriptorCollectionEnumerator(GridVisibleColumnDescriptorCollection collection)
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
        public GridVisibleColumnDescriptor Current
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

    #region QueryAddVisibleColumn

    // eva GridQueryAddVisibleColumn SyncfusionCancel GridTableDescriptor tableDescriptor GridVisibleColumnDescriptor column
    
    /// <summary>
    /// Represents a method that handles an event with <see cref="GridQueryAddVisibleColumnEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void GridQueryAddVisibleColumnEventHandler(object sender, GridQueryAddVisibleColumnEventArgs e);

    /// <summary>
    /// The GridEngine.QueryAddVisibleColumn event affects the auto-population of the GridVisibleColumnDescriptorCollection. <para/>
    /// It is called for each column and lets you control at run-time if the column should be added to the 
    /// GridVisibleColumnDescriptorCollection. You can set e.Cancel = True to avoid specific columns
    /// being added.
    /// </summary>
    public sealed class GridQueryAddVisibleColumnEventArgs : SyncfusionCancelEventArgs
    {
        GridTableDescriptor tableDescriptor;
        GridVisibleColumnDescriptor column;

        /// <summary>
        /// Initializes the event args
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor.</param>
        /// <param name="column">The column.</param>
        public GridQueryAddVisibleColumnEventArgs(GridTableDescriptor tableDescriptor, GridVisibleColumnDescriptor column)
        {
            this.tableDescriptor = tableDescriptor;
            this.column = column;
        }

        /// <summary>
        /// The TableDescriptor
        /// </summary>
        [TraceProperty(true)]
        public GridTableDescriptor GridTableDescriptor
        {
            get
            {
                return tableDescriptor;
            }
        }

        /// <summary>
        /// The Column
        /// </summary>
        [TraceProperty(true)]
        public GridVisibleColumnDescriptor GridVisibleColumn
        {
            get
            {
                return column;
            }
        }
    }
    #endregion
}

