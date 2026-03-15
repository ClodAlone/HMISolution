//-------------------------------------------------------------------------------------------------
// <copyright file="GridColumnDescriptor.cs" company="syncfusion">
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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

using Syncfusion.Design;
using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms; // IItemPropertiesSource

#if ASPNET
using System.Web.UI;
using Syncfusion.Web.Design.UI;
using System.Xml.Serialization;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
using Syncfusion.Windows.Forms.Grid.Grouping.Design;
using System.Drawing;
using System.Data;
using System.Xml.Serialization;
using System.IO;
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    #region GridColumnDescriptorCollection
    /// <summary>
    /// A collection of <see cref="GridColumnDescriptor"/> columns with mapping information to columns of the underlying datasource.
    /// An instance of this collection is returned by the <see cref="GridTableDescriptor.Columns"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    public class GridColumnDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        internal ArrayList _inner;
        internal SortedList _sorted;
        internal GridTableDescriptor _tableDescriptor;
        bool autoPopulated = false;
        internal int version;
        bool modified = false;
        bool readOnly = false;
        int fieldsVersion = -1;
        internal bool shouldPopulate = false;
        bool inInitializeFrom = false;
        bool inInitializeFromChanged = false;

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
                return this._tableDescriptor != null ? this._tableDescriptor.Engine : null;
            }
        }

        /// <summary>
        /// Occurs after a property in a nested element or the collection is changed.
        /// </summary>
        [Category("Columns")]
        [Description("Occurs after a property in a nested element or the collection is changed")]
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        ///  [Category("Columns")]
        [Description("Occurs before a property in a nested element or the collection is changed")]
        public event ListPropertyChangedEventHandler Changing;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static readonly GridColumnDescriptorCollection Empty = new GridColumnDescriptorCollection(null);

        internal bool insideCollectionEditor = false;

        internal void SetTableDescriptor(GridTableDescriptor tableDescriptor)
        {
            this._tableDescriptor = tableDescriptor;
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                this[n].SetCollection(this);
            }
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
            return String.Format("GridColumnDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        /// <summary>
        /// Ensure type correctness when a new element is added to the collection.
        /// </summary>
        /// <param name="obj">The newly added object.</param>
        protected virtual void CheckType(object obj)
        {
            if (obj != null && !(obj is GridColumnDescriptor))
            {
                throw new ArgumentException("Wrong type");
            }
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
                if (Switches.GroupingEngine.TraceVerbose)
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

        void IInsideCollectionEditorProperty.InitializeFrom(object other)
        {
            InitializeFrom((GridColumnDescriptorCollection)other);
            this.EnsureInitialized(true);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridColumnDescriptorCollection other)
        {
            int i;
            bool savedshouldPopulateThis = shouldPopulate;
            bool savedshouldPopulateOther = other.shouldPopulate;
            this.shouldPopulate = false;
            other.shouldPopulate = false;
            inInitializeFrom = true;
            inInitializeFromChanged = false;

            try
            {
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
                    GridColumnDescriptor cd = this.InternalCreateGridColumnDescriptor(other[i].MappingName);
                    cd.InitializeFrom(other[i]);
                    Add(cd);
                }

                inInitializeFrom = false;
                /*                if (inInitializeFromChanged)
                //                {
                //                    this.OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                //                }*/
                this.initFieldDescriptors = true; //// force InitializeMapping calls.
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

        void EnsureSortedList()
        {
            if (_sorted == null)
            {
                _sorted = new SortedList();
                foreach (GridColumnDescriptor cd in this._inner)
                {
                    _sorted.Add(cd.Name, cd);
                }
            }
        }

        /// <overload>
        /// Initializes a new empty collection.
        /// </overload>
        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public GridColumnDescriptorCollection()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new empty collection and attaches it to a <see cref="TableDescriptor"/>.
        /// </summary>
        internal GridColumnDescriptorCollection(GridTableDescriptor tableDescriptor)
        {
            this._inner = new ArrayList();
            _tableDescriptor = tableDescriptor;
            if (_tableDescriptor != null)
            {
                ////_tableDescriptor.ItemPropertiesChanged += new EventHandler(_tableDescriptor_ItemPropertiesChanged);
                ////  if (!(this is ExpressionFieldDescriptorCollection)
                //// _tableDescriptor.ExpressionFields.Changed += new ListPropertyChangedEventHandler(ExpressionFields_Changed);
            }

            object unused = inInitializeFromChanged;
        }

        internal GridColumnDescriptorCollection(GridTableDescriptor tableDescriptor, GridColumnDescriptor[] columnDescriptors)
            : this(tableDescriptor)
        {
            this.AddRange(columnDescriptors);
        }

        /// <summary>
        /// Resets the collection to its default state. If the collection is bound to a <see cref="TableDescriptor"/>,
        /// the collection will autopopulate itself the next time an item inside the collection is accessed.
        /// </summary>
        public void Reset()
        {
            if (this.modified || _inner.Count > 0)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inReset = true;
                isReset = true;
                autoPopulated = false;
                this.modified = false;
                _inner.Clear();
                if (_sorted != null)
                {
                    _sorted.Clear();
                }

                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inReset = false;
            }

            autoPopulated = false;
            isReset = true;
            this.modified = false;
            if (!shouldPopulate)
            {
                this.shouldPopulate = true;
                this.disableShouldPopulate = true;
            }
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="columnDescriptors">The Array with elements that should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(GridColumnDescriptor[] columnDescriptors)
        {
            foreach (GridColumnDescriptor cd in columnDescriptors)
            {
                this.Add(cd);
            }
        }

        /// <summary>
        /// Raises the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.insideCollectionEditor && !this.inEnsureInitialized)
            {
                if (Changing != null)
                {
                    Changing(this, e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            version++;
            this.fieldsVersion = -1;
            if (_tableDescriptor != null)
            {
                _tableDescriptor.ResetSortByDisplayMemberCols();
            }
            if (!this.inEnsureInitialized && !this.inReset)
            {
                modified = true;
            }

            if (!this.insideCollectionEditor && !this.inEnsureInitialized)
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, e.Item, e.Index, version);
                }
#else
                ;
#endif

                ////  if (inInitializeFrom)
                //// {
                //// inInitializeFromChanged = true;
                //// return;
                //// }
                if (Changed != null)
                {
                    Changed(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanged(GridColumnDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            _sorted = null;
            if (!this.InsideCollectionEditor)
            {
                //// if (inInitializeFrom)
                ////  {
                ////  inInitializeFromChanged = true;
                ////   return;
                ////  }

                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, _inner.IndexOf(column), column, e.PropertyName, e));
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif
                this.initFieldDescriptors = true; //// force InitializeMapping calls.
              
                //// if (this._tableDescriptor != null && this._tableDescriptor.IsDesignTime())
                ////  {
                //// _sorted = null;
                ////  this.EnsureSortedList();
                //// }
            }
        }

        internal void RaisePropertyItemChanging(GridColumnDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, _inner.IndexOf(column), column, e.PropertyName, e));
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
                }
#else
                ;
#endif
            }
        }

        bool inEnsureInitialized = false;
        bool isReset = true;
        bool inReset = false;
        int engineSourceListVersion = -1;
        Hashtable fieldToColumn;
        bool initFieldDescriptors = false;

        /// <summary>
        /// Ensures the collection is initialized and auto-populates the collection on demand.
        /// </summary>
        /// <param name="populate">True if collection should auto-populate itself based on properties of
        /// the underlying datasource.</param>
        protected internal virtual void EnsureInitialized(bool populate)
        {
            //// ||
            ////_tableDescriptor.Engine == null || _tableDescriptor.Engine.Initializing)
            if (inEnsureInitialized || !shouldPopulate || _tableDescriptor == null)
            {
                return;
            }

            if (disableShouldPopulate)
            {
                shouldPopulate = false;
            }

            if (!modified && _tableDescriptor.Fields.Count == 0 && this._inner.Count == 0 && !_tableDescriptor.ShouldSerializeItemProperties()
                && !_tableDescriptor.ShouldSerializeFields())
            {
                return;
            }

            GridEngine engine = this._tableDescriptor.Engine;

            if (engine != null && !engine.Initializing &&
                (fieldsVersion != this._tableDescriptor.Fields.Version
                || (engine != null && engineSourceListVersion != engine.SourceListVersion)))
            {
                fieldsVersion = this._tableDescriptor.Fields.Version;
                engineSourceListVersion = engine != null ? this._tableDescriptor.Engine.SourceListVersion : -1;
                this.version++;
                initFieldDescriptors = true;

                if (!this.modified)
                {
                    isReset = true;
                }
            }

            if (!modified && isReset)
            {
                inEnsureInitialized = true;

                if (_tableDescriptor == null || !populate)
                {
                    ////_inner = new ArrayList(0);
                }
                else
                {
                    TraceUtil.TraceCalledFromIf(Switches.AutoPopulate.TraceVerbose, 10, version);
                    FieldDescriptorCollection fields = _tableDescriptor.Fields;
                    isReset = false;
                    foreach (GridColumnDescriptor cd in _inner)
                    {
                        cd.Dispose();
                    }

                    _inner.Clear();
                    if (_sorted != null)
                    {
                        _sorted.Clear();
                    }

                    foreach (FieldDescriptor field in fields)
                    {
                        if (!field.Hide)
                        {
                            GridColumnDescriptor column = InternalCreateGridColumnDescriptor(field.Name);
                            column.MappingName = field.Name;
                            PropertyDescriptor pd = field.GetPropertyDescriptor();
                            if (pd != null)
                            {
                                if (field.IsComplexPropertyField())
                                {
                                    FieldDescriptor ppd = field.GetParentFieldDescriptor();
                                    column.headerText = ppd.GetPropertyDescriptor().DisplayName + "_" + pd.DisplayName;
                                }
                                else
                                {
                                    column.headerText = pd.DisplayName;
                                }
                            }

                            column.field = field;
                            if (field.GetPropertyType() != null && field.GetPropertyType() == typeof(bool) && GridEngine.DisplayCheckBoxForBooleanFields)
                            {
                                column.AllowDropDownCell = false;
                            }

                            ////column.ReadOnly = field.ReadOnly;
                            column.SetCollection(this);
                            GridQueryAddColumnEventArgs e = new GridQueryAddColumnEventArgs(this._tableDescriptor, column);
                            if (Engine != null)
                            {
                                Engine.RaiseQueryAddColumn(e);
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
                    }

                    autoPopulated = true;
                    modified = false;
                    ////OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                    if (Syncfusion.Grouping.Engine.HelpTracing)
                    {
                        TraceUtil.TraceCurrentMethodInfo("Reinitialize", this._tableDescriptor, fields.Count);
                        TraceUtil.TraceCalledFrom();
                    }
                }

                inEnsureInitialized = false;
            }

            if (initFieldDescriptors)
            {
                fieldToColumn = null;

                foreach (GridColumnDescriptor cd in this._inner)
                {
                    if (this.Engine.AllowSwapDataViewWithDataTableList && this.Engine.AllowResetTableDescriptorWhenDataSourceSetNull && (Engine.DataSource != null && !(Engine.DataSource is IBindingList)))
                    {
                        foreach (DataColumn column in ((DataTable)Engine.DataSource).Columns)
                        {
                            if (column.Caption != column.ColumnName && column.ColumnName == cd.MappingName)
                            {
                                cd.HeaderText = column.Caption;
                                break;
                            }
                        }
                    }
                    else
                    {
                        cd.InitalizeMapping(_tableDescriptor);
                    }
                    if (cd.field == null && this._tableDescriptor.Engine.HasSourceList())
                    {
                        Console.WriteLine("Could not find Field with name " + cd.MappingName + " in " + _tableDescriptor.Name + " for column " + cd.Name);
                    }
                }

                fieldsVersion = this._tableDescriptor.Fields.Version;
                if (Syncfusion.Grouping.Engine.HelpTracing)
                {
                    TraceUtil.TraceCurrentMethodInfo("InitFD", this._tableDescriptor, _inner.Count);
                }

                initFieldDescriptors = false;
            }
        }

        /// <summary>
        /// Creates a new empty column descriptor with the specified name.
        /// </summary>
        /// <param name="name">The name of the new column descriptor.</param>
        /// <returns>A new GridColumnDescriptor.</returns>
        protected virtual GridColumnDescriptor InternalCreateGridColumnDescriptor(string name)
        {
            return new GridColumnDescriptor(name);
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public GridColumnDescriptorCollection Clone()
        {
            return InternalClone();
        }

        /// <summary>
        /// Creates a copy of this collection and all its inner elements. This method is called from Clone.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        protected GridColumnDescriptorCollection InternalClone()
        {
            int count = Count;
            GridColumnDescriptor[] columnDescriptors = new GridColumnDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                columnDescriptors[n] = this[n].Clone();
            }

            GridColumnDescriptorCollection c = CreateCollection(_tableDescriptor, columnDescriptors);
            c.modified = modified;
            c.version = version + 1000;
            c.autoPopulated = autoPopulated;
            c.fieldsVersion = -1;
            return c;
        }

        /// <summary>
        /// Called from InternalClone to create a new collection and attach it to the specified table descriptor
        /// and insert the specified columns. The columns have already been cloned.
        /// </summary>
        /// <param name="td">The table descriptor.</param>
        /// <param name="columnDescriptors">The cloned column descriptors.</param>
        /// <returns>A new GridColumnDescriptorCollection.</returns>
        protected virtual GridColumnDescriptorCollection CreateCollection(GridTableDescriptor td, GridColumnDescriptor[] columnDescriptors)
        {
            return new GridColumnDescriptorCollection(td, columnDescriptors);
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
            else if (!(obj is GridColumnDescriptorCollection))
            {
                return false;
            }

            return InternalEquals((GridColumnDescriptorCollection)obj);
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
        /// collection or an element within the collection is modified. When getting the version,
        /// <see cref="EnsureInitialized"/> is called to ensure the collection is auto-populated
        /// if needed.
        /// </summary>
        public int Version
        {
            get
            {
                EnsureInitialized(true);
                return version;
            }
            //// set
            //// {
            //// version = value;
            //// }
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
                if (modified != value)
                {
                    modified = value;
                    if (!modified)
                    {
                        this.Reset();
                    }
                }
            }
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
        /// modify it (e.g. remove members from the auto-populated list).
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// gridGroupingControl1.TableDescriptor.Columns.LoadDefault();
        /// gridGroupingControl1.TableDescriptor.Columns.Remove("MyChildTable.ForeignCategoryID");
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
        /// Compares each element with the element of another collection.
        /// </summary>
        /// <param name="other">The collection to compare to.</param>
        /// <returns>True if all elements are equal and in the same order; False otherwise.</returns>
        protected bool InternalEquals(GridColumnDescriptorCollection other)
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
        public GridColumnDescriptor this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (GridColumnDescriptor)_inner[index];
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

                CheckType(value);

                if (!modified)
                {
                    EnsureInitialized(false);
                }

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
        public GridColumnDescriptor GetColumnDescriptor(string name){return this[name];}
#endif

#if ASPNET
        // Designer ser. doesn't like overloaded accessors.
        internal
#else
        public
#endif
 GridColumnDescriptor this[string name]
        {
            get
            {
                if (!modified)
                {
                    EnsureInitialized(true);
                }

                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (GridColumnDescriptor)_inner[index];
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

                CheckType(value);

                if (!modified)
                {
                    EnsureInitialized(false);
                }

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

        /// <summary>
        /// Searches the column with a matching <see cref="GridColumnDescriptor.MappingName"/>.
        /// </summary>
        /// <param name="mappingName">Mapping name of the column.</param>
        /// <returns>Column descriptor.</returns>
        public GridColumnDescriptor FindByMappingName(string mappingName)
        {
            FieldDescriptor fd = _tableDescriptor.Fields[mappingName];
            if (fd == null)
            {
                return null;
            }

            GridColumnDescriptor gcd = null;
            ////try to get ColumnDescriptor by field, else get it from this
            try
            {
                gcd = FindByField(fd);
            }
            catch (Exception)
            {
                foreach (GridColumnDescriptor cd in this)
                {
                    if (cd.MappingName == mappingName)
                    {
                        gcd = cd;
                        break;
                    }
                }
            }

            return gcd;
            ////foreach (GridColumnDescriptor cd in this)
            ////{
            //// if (cd.MappingName == mappingName)
            //// return cd;
            ////}
            ////return null;
        }

        /// <summary>
        /// Searches the column with a matching <see cref="FieldDescriptor"/>.
        /// </summary>
        /// <param name="fd">The field descriptor.</param>
        /// <returns>The column descriptor.</returns>
        public GridColumnDescriptor FindByField(FieldDescriptor fd)
        {
            this.EnsureInitialized(true);
            if (fieldToColumn == null)
            {
                fieldToColumn = new Hashtable();
                foreach (GridColumnDescriptor cd in this._inner)
                {
                    FieldDescriptor field = cd.FieldDescriptor;
                    if (field != null)
                    {
                        fieldToColumn.Add(this._tableDescriptor.Fields.IndexOf(field), cd);
                    }
                }
            }

            int fieldIndex = _tableDescriptor.Fields.IndexOf(fd);
            if (fieldToColumn.Contains(fieldIndex))
            {
                return (GridColumnDescriptor)fieldToColumn[fieldIndex];
            }

            return null;
        }

        internal int Find(string name)
        {
            EnsureSortedList();
            if (_sorted != null)
            {
                GridColumnDescriptor cd = (GridColumnDescriptor)_sorted[name];

                if (cd == null)
                {
                    bool b = false;

                    if (name.IndexOf(".") != -1)
                    {
                        b = true;
                        cd = (GridColumnDescriptor)_sorted[name.Replace(".", "_")];  // Backward compatibility
                    }
                    else if (name.IndexOf("_") != -1)
                    {
                        b = true;
                        cd = (GridColumnDescriptor)_sorted[name.Replace("_", ".")];  // Backward compatibility
                    }

                    if (cd == null && b)
                    {
                        string s = name.Replace(".", "_");

                        foreach (GridColumnDescriptor fd in _inner)
                        {
                            string t = fd.Name.Replace(".", "_");
                            if (t == s)
                            {
                                cd = fd;
                                break;
                            }
                        }
                    }
                }

                if (cd != null)
                {
                    return cd.index;
                }
            }

            return -1;
        }

        /// <overload>
        /// Determines if the element belongs to this collection.
        /// </overload>
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
        public bool Contains(GridColumnDescriptor value)
        {
            if (value == null)
            {
                return false;
            }

            CheckType(value);

            if (!modified)
            {
                EnsureInitialized(true);
            }

            if (this.inInitializeFrom)
            {
                return _inner.Contains(value);
            }

            EnsureSortedList();
            return _sorted != null && _sorted.Contains(value.Name);
        }

        /// <summary>
        /// Determines if an element with the given name belongs to this collection.
        /// </summary>
        /// <param name="name">The name of Object to locate in the collection.</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="EnsureInitialized"/>.
        /// </remarks>
        public bool Contains(string name)
        {
            if (!modified)
            {
                EnsureInitialized(true);
            }

            return Find(name) != -1;
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(GridColumnDescriptor value)
        {
            if (this.inInitializeFrom)
            {
                return _inner.IndexOf(value);
            }

            if (!modified)
            {
                EnsureInitialized(true);
            }

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
            if (!modified)
            {
                EnsureInitialized(true);
            }

            return Find(name);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(GridColumnDescriptor[] array, int index)
        {
            int n = 0;
            foreach (GridColumnDescriptor item in this)
            {
                array[index + n] = item;
                n++;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        GridColumnDescriptorCollection SyncRoot
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
        public GridColumnDescriptorCollectionEnumerator GetEnumerator()
        {
            return new GridColumnDescriptorCollectionEnumerator(this);
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
        public void Insert(int index, GridColumnDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            CheckType(value);

            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
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
        /// Moves an element within the collection.
        /// </summary>
        /// <param name="src">The original index of the element within the collection.</param>
        /// <param name="dest">The target index of the element within the collection.</param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public void Move(int src, int dest)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Move, src, null, null));
            object value = _inner[src];
            _inner.RemoveAt(src);
            _inner.Insert(dest, value);
            for (int n = 0; n < Count; n++)
            {
                this[n].index = n;
            }

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
        public void Remove(GridColumnDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                return;
            }

            CheckType(value);

            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, -1, value, null));
            if (_sorted != null)
            {
                _sorted.Remove(value.Name);
            }

            int index = IndexOf(value);
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
        public int Add(GridColumnDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            CheckType(value);

            if (!this.inEnsureInitialized)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));

                // && !this.inInitializeFrom)
                if (shouldPopulate)
                {
                    //// Changed Add behavior for 3.0.0.12 - user should call explicitly Clear()
                    //// if collection should be reset before adding fields.
                    this.EnsureInitialized(true);
                    if (!this.modified && this._inner.Count > 0)
                    {
                        if (Contains(value.Name) && Engine.VersionInfo.CompareTo("3.0.0.12") <= 0)
                        {
                            FieldDescriptorCollection.ShowAddRangeChangedWarning("Columns");
                        }
                    }
                }
            }

            string fd = null;

            //// If value is added with a name that already exist, replace
            //// the old value with the new descriptor.
            int index = -1;
            if (value.Name != null && value.Name.Length > 0)
            {
                if (value.Name.StartsWith("#"))
                {
                    Console.WriteLine(value.Name);
                }

                //// Special case for foreign key fields - add them automatically to Fields collection
                //// and rename '.' with '_'
                if (shouldPopulate)
                {
                    if (!this._tableDescriptor.Fields.Contains(value.MappingName))
                    {
                        int dot = value.MappingName.IndexOf('.');
                        if (dot != -1)
                        {
                            value.Name = value.Name.Replace('.', '_');
                            fd = value.MappingName;
                        }
                    }

                    EnsureSortedList();
                    index = IndexOf(value.Name);
                    if (index != -1)
                    {
                        _inner[index] = value;
                        _sorted[value.Name] = value;
                    }
                }
            }
            else
            {
                SuggestName(value);
            }

            if (index == -1)
            {
                bool avail = false;
                foreach (object innerval in _inner)
                {
                    if (innerval.ToString().Equals(value.Name))
                    {
                        avail = true;
                    }
                }
                if (!avail)
                    index = _inner.Add(value);
                if (_sorted != null && !_sorted.Contains(value.Name))
                {
                    _sorted.Add(value.Name, value);
                }
            }

            value.index = index;
            value.SetCollection(this);
            if (!this.inEnsureInitialized)
            {
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));

                // Maybe later ... Right now this causes issues if you add a column and then later
                // a ColumnSet that references the column. The column might appear duplicate, e.g. see EmployeeTerritoriesRelationKind
                // property in EmployeeTerritories sample.
                // if (this._tableDescriptor.VisibleColumns.IsModified
                // && !this._tableDescriptor.VisibleColumns.Contains(value.Name))
                // this._tableDescriptor.VisibleColumns.Add(value.Name);
                if (fd != null && this.Engine != null && this.Engine.HasSourceList())
                {
                    this._tableDescriptor.Fields.Add(fd);
                }
            }

            return index;
        }

        /// <summary>
        /// Called to get a new default name when a new field descriptor is created (e.g. when pressing "Add" in a collection editor).
        /// </summary>
        /// <param name="value">The field descriptor to be named.</param>
        protected virtual void SuggestName(GridColumnDescriptor value)
        {
            if (value.MappingName == null || value.MappingName.Length == 0)
            {
                FieldDescriptorCollection fields = this._tableDescriptor.Fields;
                if (fields.Count > this.Count)
                {
                    foreach (FieldDescriptor field in fields)
                    {
                        if (IndexOf(field.Name) == -1)
                        {
                            value.MappingName = field.Name;
                            return;
                        }
                    }
                }
            }

            int n = 1;
            double d;
            foreach (GridColumnDescriptor col in this)
            {
                if (col.Name.StartsWith("Column"))
                {
                    if (double.TryParse(col.Name.Substring("Column".Length), System.Globalization.NumberStyles.Number, null, out d))
                    {
                        n = Math.Max((int)d + 1, n);
                    }
                }
            }

            value.Name = "Column" + n.ToString();
            value.nameModified = false;
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="name">The name of the element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name)
        {
            return InternalAdd(name);
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="name">The name of the element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <param name="mappingName">The field name.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name, string mappingName)
        {
            GridColumnDescriptor cd = InternalCreateGridColumnDescriptor(name);
            cd.MappingName = mappingName;
            return Add(cd);
        }

        /// <summary>
        /// Called from Add(string name) to create a new field descriptor with the given name.
        /// </summary>
        /// <param name="name">The name of the new field descriptor.</param>
        /// <returns>A new field descriptor.</returns>
        protected virtual int InternalAdd(string name)
        {
            return Add(InternalCreateGridColumnDescriptor(name));
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
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (!modified)
            {
                this.EnsureInitialized(true);
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
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            GridColumnDescriptor value = (GridColumnDescriptor)_inner[index];
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
        /// Disposes the object and collection items.
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

            if (_inner.Count > 0)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                _inner.Clear();
                if (_sorted != null)
                {
                    _sorted.Clear();
                }

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
        /// <remarks>
        /// The method calls <see cref="EnsureInitialized"/>.
        /// </remarks>
        public int Count
        {
            get
            {
                if (!modified)
                {
                    this.EnsureInitialized(true);
                }

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
                this[index] = (GridColumnDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridColumnDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((GridColumnDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridColumnDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridColumnDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((GridColumnDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridColumnDescriptor[])array, index);
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
              new CategoryAttribute("ColumnDescriptors")
            };

            ArrayList names = new ArrayList();
            foreach (GridColumnDescriptor column in this)
            {
                if (column.Name != string.Empty)
                {
                    pds.Add(new DescriptorBasePropertyDescriptor(column.Name, column, att, GetType()));
                    names.Add(column.Name);
                }
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
    /// Enumerator class for <see cref="GridColumnDescriptor"/> elements of a <see cref="GridColumnDescriptorCollection"/>.
    /// </summary>
    public class GridColumnDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridColumnDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GridColumnDescriptorCollectionEnumerator(GridColumnDescriptorCollection collection)
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
        public GridColumnDescriptor Current
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

    #region TypeConverter

    /// <summary>
    /// The type converter for <see cref="GridColumnDescriptor"/> objects. <see cref="GridColumnDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class GridColumnDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridColumnDescriptorTypeConverter()
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
        /// <param name="culture">Current culture information used for conversion.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destinationType">Type to convert to.</param>
        /// <returns>Converted object.</returns>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(InstanceDescriptor))
            {
                GridColumnDescriptor column = (GridColumnDescriptor)value;
                Type type = typeof(GridColumnDescriptor);

                if (column.ShouldSerializeAppearance()
                    || column.ShouldSerializeGroupByAppearance()
                    || column.ShouldSerializeWidth()
                    || column.ShouldSerializeReadOnly()
                    || column.ShouldSerializeGroupByOptions()
                    || column.ShouldSerializeHeaderText()
                    || column.ShouldSerializeImageArray()
                    || column.ShouldSerializeAllowDropDownCell()
                    || column.ShouldSerializeAllowFilter()
                    || column.ShouldSerializeAllowSort()
                    || column.ShouldSerializeAllowGroupByColumn()
                    || column.ShouldSerializeAllowBlink()
                    || column.ShouldSerializeTrackWidthOfParentColumn()
                    || column.ShouldSerializeMaxLength()
                    || column.ShouldSerializeEditFormColumnIndex())
                {
                    return new InstanceDescriptor(type.GetConstructor(new Type[0]), null, false);
                }
                else if (column.ShouldSerializeName())
                {
                    return new InstanceDescriptor(type.GetConstructor(new Type[] { typeof(string), typeof(string) }), new object[] { column.Name, column.MappingName }, true);
                }
                else
                {
                    return new InstanceDescriptor(type.GetConstructor(new Type[] { typeof(string) }), new object[] { column.MappingName }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo
        
        /// <override/>
        /// <summary>
        /// Gets a collection of properties for the specified object type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Value that specifies the type.</param>
        /// <param name="attributes">An array of System.Attribute objects that will be used as a filter.</param>
        /// <returns>A list of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "MappingName",
                "HeaderText",
                "ReadOnly",
                "Width",
                "TrackWidthOfParentColumn",
                "AllowDropDownCell",
                "AllowSort",
                "AllowFilter",
                "AllowGroupByColumn",
                "AllowBlink",
                "Appearance",
                "GroupByApperance",
                "GroupByOptions",
                "EditFormColumnIndex"
            };

            return pds.Sort(atts);
        }
    }
    #endregion

    #region GridColumnDescriptor

    /// <summary>
    /// An IComparer implementation that compares the <see cref="GridColumnDescriptor.Name"/> of
    /// two <see cref="GridColumnDescriptor"/> objects.
    /// </summary>
    public class GridColumnDescriptorNameComparer : IComparer
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridColumnDescriptorNameComparer()
            : base()
        {
        }

        #region IComparer Members

        /// <summary>
        /// Compares two <see cref="GridColumnDescriptor"/> objects.
        /// </summary>
        /// <param name="x">The first GridColumnDescriptor.</param>
        /// <param name="y">The second GridColumnDescriptor.</param>
        /// <returns>
        /// The result of string.Compare for the <see cref="GridColumnDescriptor.Name"/> of the
        /// two <see cref="GridColumnDescriptor"/> objects; 0 if both are the same.
        /// </returns>
        public int Compare(object x, object y)
        {
            GridColumnDescriptor c = (GridColumnDescriptor)x;
            GridColumnDescriptor d = (GridColumnDescriptor)y;
            return c.Name.CompareTo(d.Name);
        }

        #endregion
    }

    /// <summary>
    /// GridColumnDescriptor provides mapping information to a column of the underlying datasource.
    /// <para/>
    /// Columns are managed by the <see cref="GridColumnDescriptorCollection"/> that
    /// is returned by the <see cref="GridTableDescriptor.Columns"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(GridColumnDescriptorTypeConverter))]
    public class GridColumnDescriptor : DescriptorBase, ICloneable, IStandardValuesProvider, IGridTableCellAppearanceSource, IGridGroupOptionsSource
    {
        string name = string.Empty;
        internal FieldDescriptor field = null;
        GridColumnDescriptorCollection collection;
        GridTableDescriptor tableDescriptor;
        string mappingName = string.Empty;
        internal string headerText = string.Empty;
        bool headerModified = false;
        bool readOnly = false;
        bool allowFilter = false;
        bool allowSort = true;
        int maxLength = -1;
        internal int index;
        internal bool nameModified = false;
        ////bool isDefault = false;
        int width = -1;
        bool widthModified;
        bool allowDropDownCell = true;
        bool allowDropDownCellModified = false;
        bool allowGroupByColumn = true;
        bool allowGroupByColumnModified = false;
        bool sortByDisplayMember = false;

        [ThreadStatic]
        static int tableCounter = 0;
        int tableId = 0;
        string trackWidthOfParentColumn = string.Empty;
        bool allowBlink = false;
        bool allowBlinkModified = false;
        int m_nEditFormColumnIndex = 0;

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <overload>
        /// Initializes a new column.
        /// </overload>
        /// <summary>
        /// Initializes a new empty column.
        /// </summary>
        public GridColumnDescriptor()
        {
            tableId = ++tableCounter;
            //// if (tableId == 2)
            //// Console.WriteLine(string.Empty);
        }

        /// <summary>
        /// Initializes a new column with a name.
        /// </summary>
        /// <param name="name">Column name.</param>
        public GridColumnDescriptor(string name)
            : this()
        {
            this.name = name;
            this.headerText = name;
            this.mappingName = name;
        }

        /// <summary>
        /// Initializes a new column with a name and mapping name.
        /// </summary>
        /// <param name="name">Name of the column.</param>
        /// <param name="mappingName">Mapping name of the column.</param>
        public GridColumnDescriptor(string name, string mappingName)
            : this()
        {
            this.name = name;
            this.nameModified = true;
            this.headerText = name;
            this.mappingName = mappingName;
            this.readOnly = false;
            this.allowFilter = false;
            this.width = -1;
        }

        /// <summary>
        /// Initializes a new column with a name, mapping name, and a header text.
        /// </summary>
        /// <param name="name">Name of the column.</param>
        /// <param name="mappingName">Mapping name of the column.</param>
        /// <param name="headerText">column header text.</param>
        public GridColumnDescriptor(string name, string mappingName, string headerText)
            : this()
        {
            this.name = name;
            this.nameModified = true;
            this.headerText = headerText;
            this.headerModified = true;
            this.mappingName = mappingName;
            this.readOnly = false;
            this.allowFilter = false;
            this.width = -1;
        }

        /// <summary>
        /// Initializes a new column with a name, mapping name, and Read-only setting.
        /// </summary>
        /// <param name="name">Name of the column.</param>
        /// <param name="mappingName">Mapping name of the column.</param>
        /// <param name="headerText">Column header text.</param>
        /// <param name="readOnly">Specifies the read-only state of the column.</param>
        public GridColumnDescriptor(string name, string mappingName, string headerText, bool readOnly)
            : this()
        {
            this.name = name;
            this.nameModified = true;
            this.headerText = headerText;
            this.headerModified = true;
            this.mappingName = mappingName;
            this.readOnly = readOnly;
            this.allowFilter = false;
            this.width = -1;
        }

        /// <summary>
        /// Initializes a new column with a name, mapping name, header text, Read-only setting, and column width.
        /// </summary>
        /// <param name="name">Name of the column.</param>
        /// <param name="mappingName">Mapping name of the column.</param>
        /// <param name="headerText">Column header text.</param>
        /// <param name="readOnly">Specifies the read-only state of the column.</param>
        /// <param name="width">Column width.</param>
        public GridColumnDescriptor(string name, string mappingName, string headerText, bool readOnly, int width)
            : this()
        {
            this.name = name;
            this.nameModified = true;
            this.headerText = headerText;
            this.headerModified = true;
            this.mappingName = mappingName;
            this.readOnly = readOnly;
            this.allowFilter = false;
            this.width = width;
            this.widthModified = true;
        }

        /// <summary>
        /// Initializes a new column with a name, mapping name, and header text information retrieved from a FieldDescriptor.
        /// </summary>
        /// <param name="field">The field descriptor provides mapping information to a column of the underlying datasource.</param>
        public GridColumnDescriptor(FieldDescriptor field)
            : this()
        {
            ////this.field = field;
            this.name = field.Name;
            this.headerText = field.Name;
            this.mappingName = field.Name;
        }

        /// <summary>
        /// Initializes a new column with a name, header text. The mapping name is retrieved from a FieldDescriptor.
        /// </summary>
        /// <param name="field">The field descriptor provides mapping information to a column of the underlying datasource.</param>
        /// <param name="name">Column name.</param>
        public GridColumnDescriptor(FieldDescriptor field, string name)
            : this()
        {
            ////this.field = field;
            this.name = name;
            this.nameModified = true;
            this.headerText = name;
            this.mappingName = field.Name;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            ////if (tableId == 2)
            ////    Console.WriteLine(string.Empty);
            if (disposing)
            {
                name = "Disposed";
                field = null;
                collection = null;
                tableDescriptor = null;
                mappingName = string.Empty;
                headerText = string.Empty;
                headerModified = false;
                //// bool readOnly = false;
                //// int index = -1;
                ////  nameModified = false;
                //// width = -1;
                //// widthModified;
            }

            base.Dispose(disposing);
        }

        /// <override/>
        /// <summary>Resets the descriptor settings.</summary>
        public override void Reset()
        {
            this.ResetAppearance();
            this.ResetGroupByAppearance();
            this.ResetGroupByOptions();
            this.ResetHeaderText();
            this.ResetReadOnly();
            this.ResetWidth();
            this.ResetAllowFilter();
            this.ResetAllowDropDownCell();
            this.ResetAllowGroupByColumn();
            this.ResetTrackWidthOfParentColumn();
            this.ResetAllowBlink();
            this.ResetEditFormColumnIndex();
            ////this.ResetName();
        }

        /// <override/>
        /// <summary>Determines whether this object and its childs should be serialized.</summary>
        /// <returns>True if it should be serialized.</returns>
        public override bool ShouldSerialize()
        {
            return
                this.ShouldSerializeAppearance() ||
                this.ShouldSerializeGroupByAppearance() ||
                this.ShouldSerializeGroupByOptions() ||
                this.ShouldSerializeHeaderText() ||
                this.ShouldSerializeReadOnly() ||
                this.ShouldSerializeWidth() ||
                this.ShouldSerializeAllowFilter() ||
                this.ShouldSerializeAllowSort() ||
                this.ShouldSerializeAllowDropDownCell() ||
                this.ShouldSerializeAllowGroupByColumn() ||
                this.ShouldSerializeTrackWidthOfParentColumn() ||
                this.ShouldSerializeAllowBlink() ||
                this.ShouldSerializeEditFormColumnIndex()||
                this.ShouldSerializeImageArray();
            ////this.ResetName();
        }

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return MappingName;
        }

        /// <summary>
        /// Enable/disable sort by this column.
        /// </summary>
        [NotifyParentProperty(true),
            DefaultValue(true),
            Description("Enables sorting by this column if set to true"),
            Category("Behavior")]
        public bool AllowSort
        {
            get
            {
                return allowSort;
            }

            set
            {
                if (allowSort != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowSort"));
                    allowSort = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowSort"));
                }
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public string Info
        {
            get
            {
                string isdisposed = IsDisposed ? ", Disposed" : string.Empty;
                return GetType().Name + " { " + Name + "(" + this.tableId + isdisposed + ") }";
            }
        }

        /// <override/>
        /// <summary>
        /// Retunrs the name of the descriptor.
        /// </summary>
        /// <returns>Descriptor name.</returns>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Specifies column index in form edit mode.
        /// </summary>
#if ASPNET
        [
            NotifyParentProperty( true ),
            Description( "Specifies column index in form edit mode" ),
            Category( "Behavior" ),
        ]
#else
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public int EditFormColumnIndex
        {
            get
            {
                return m_nEditFormColumnIndex;
            }

            set
            {
                if (m_nEditFormColumnIndex != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("EditFormColumnIndex"));
                    m_nEditFormColumnIndex = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("EditFormColumnIndex"));
                }
            }
        }

        [NonSerialized]
        private bool allowAppearanceDeserialization = true;

        /// <summary>
        /// To enable/disable appearance deserialization.
        /// </summary>
        [Description("Enables/disables appearance deserialization")]
        [DefaultValue(true)]
        [XmlIgnore()]
        public bool AllowAppearanceDeserialization
        {
            get
            {
                return allowAppearanceDeserialization;
            }
            set
            {
                if (allowAppearanceDeserialization != value)
                    allowAppearanceDeserialization = value;
            }
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public virtual void InitializeFrom(GridColumnDescriptor other)
        {
            this.MappingName = other.MappingName;
            this.Name = other.Name;
            this.nameModified = other.ShouldSerializeName();
            if (other.ShouldSerializeHeaderText())
            {
                this.HeaderText = other.HeaderText;
            }
            else
            {
                this.ResetHeaderText();
            }
            if (other.ShouldSerializeImageArray())
            {
                this.SerializedImageArray = other.SerializedImageArray;
            }
            else
            {
                this.ResetImageArray();
            }
            if (other.ShouldSerializeReadOnly())
            {
                this.ReadOnly = other.ReadOnly;
            }
            else
            {
                this.ResetReadOnly();
            }

            if (other.ShouldSerializeAllowFilter())
            {
                this.AllowFilter = other.AllowFilter;
            }
            else
            {
                this.ResetAllowFilter();
            }

            if (other.ShouldSerializeAllowSort())
            {
                this.AllowSort = other.AllowSort;
            }
            else
            {
                this.ResetAllowSort();
            }

            if (other.ShouldSerializeAllowDropDownCell())
            {
                this.AllowDropDownCell = other.AllowDropDownCell;
            }
            else
            {
                this.ResetAllowDropDownCell();
            }

            if (other.ShouldSerializeAllowGroupByColumn())
            {
                this.AllowGroupByColumn = other.AllowGroupByColumn;
            }
            else
            {
                this.ResetAllowGroupByColumn();
            }

            if (other.ShouldSerializeWidth())
            {
                this.Width = other.Width;
            }
            else
            {
                this.SetWidthInt(other.width);
            }

            if (!this.GroupByOptions.Equals(other.GroupByOptions))
            {
                this.GroupByOptions.CopyFrom(other.GroupByOptions);
            }

            if (this.TableDescriptor == null)
            {
                if (other.ShouldSerializeAppearance())
                {
                    this.Appearance.InitializeFrom(other.Appearance);
                }
                else
                {
                    this.ResetAppearance();
                }
            }
            else
            {
                if (this.Engine != null && this.Engine.GetDesignMode())
                {
                    if (other.ShouldSerializeAppearance())
                    {
                        this.Appearance.InitializeFrom(other.Appearance);
                    }
                    else
                    {
                        this.ResetAppearance();
                    }
                }
                else
                {
                    if(this.TableDescriptor.SupportColumnAppearanceDeserialization)
                    {
                        if(this.AllowAppearanceDeserialization)
                        {
                            if (other.ShouldSerializeAppearance())
                            {
                                this.Appearance.InitializeFrom(other.Appearance);
                            }
                            else
                            {
                                this.ResetAppearance();
                            }
                        }
                    }
                }
            }

            if (other.ShouldSerializeGroupByAppearance())
            {
                this.GroupByAppearance.InitializeFrom(other.GroupByAppearance);
            }
            else
            {
                this.ResetGroupByAppearance();
            }

            if (other.ShouldSerializeTrackWidthOfParentColumn())
            {
                this.TrackWidthOfParentColumn = other.TrackWidthOfParentColumn;
            }
            else
            {
                this.ResetTrackWidthOfParentColumn();
            }

            if (other.ShouldSerializeAllowBlink())
            {
                this.AllowBlink = other.AllowBlink;
            }
            else
            {
                this.ResetAllowBlink();
            }

            if (other.ShouldSerializeEditFormColumnIndex())
            {
                this.EditFormColumnIndex = other.EditFormColumnIndex;
            }
            else
            {
                ResetEditFormColumnIndex();
            }

            if (other.ShouldSerializeSortByDisplayMember())
            {
                this.SortByDisplayMember = other.SortByDisplayMember;
            }
            else
            {
                ResetSortByDisplayMember();
            }

#if ASPNET
            if(other.ItemTemplate!= null)
                this.ItemTemplate = other.ItemTemplate;
            if(other.EditItemTemplate!=null)
                this.EditItemTemplate=other.EditItemTemplate;
            if(other.GroupItemTemplate!=null)
                this.GroupItemTemplate=other.GroupItemTemplate;
            if(other.HeaderTemplate!=null)
                this.HeaderTemplate=other.HeaderTemplate;
            if(other.FooterTemplate!=null)
                this.FooterTemplate=other.FooterTemplate;

#endif
        }

        ICollection IStandardValuesProvider.GetStandardValues(PropertyDescriptor pd)
        {
            ArrayList al = new ArrayList();
            SortedList sl = new SortedList();
            if (pd.Name == "TrackWidthOfParentColumn")
            {
                if (TableDescriptor.ParentTableDescriptor != null)
                {
                    foreach (GridColumnDescriptor ppd in TableDescriptor.ParentTableDescriptor.Columns)
                    {
                        al.Add(ppd.Name);
                    }
                }
            }
            else
            {
                foreach (FieldDescriptor ppd in TableDescriptor.Fields)
                {
                    al.Add(ppd.Name);
                }
            }

            return al;
        }

#if ASPNET
        #region Templates
        private ITemplate itemTemplate;
        private ITemplate editItemTemplate;
        private ITemplate groupItemTemplate;
        private ITemplate headerTemplate;
        private ITemplate footerTemplate;

        internal bool AreTemplatesAvailable()
        {
            if(this.itemTemplate != null ||
                this.editItemTemplate != null ||
                this.groupItemTemplate != null ||
                this.headerTemplate != null ||
                this.footerTemplate != null)
                return true;
            return false;
        }
        /// <summary>
        /// Gets / sets the template used for displaying a data row within the GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        /// <remarks>
        /// This template lets you customize the display of the data row within the GroupingGridWebControl.
        /// The ContainerType of this template is GridCellTemplated.
        /// </remarks>
        [Description("Gets / sets the template used for displaying a data row within the GroupingGridWebControl control."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore(),
        NotifyParentProperty(true),
        ]
        public virtual ITemplate ItemTemplate {get {return itemTemplate;}set {itemTemplate = value;if(value != null && this.Collection != null)this.Collection.Modify();}
        }
        /// <summary>
        /// Gets / sets the template used to display the edited item.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        /// <remarks>
        /// This template lets you customize the display of row in edit mode.
        /// The ContainerType of this template is GridCellTemplated.
        /// </remarks>
        [Description("Gets / sets the template used to display the edited item."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore(),
        NotifyParentProperty(true)
        ]
        public virtual ITemplate EditItemTemplate {get {return editItemTemplate;}set {editItemTemplate = value;if(value != null && this.Collection != null)this.Collection.Modify();}
        }
        /// <summary>
        /// Gets / sets the template used to display group rows of the GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        /// <remarks>
        /// This template lets you customize the display of the group rows in GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </remarks>
        [Description("Gets / sets the template used to display group rows of the GroupingGridWebControl control."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore(),
        NotifyParentProperty(true)
        ]
        public virtual ITemplate GroupItemTemplate {get {return groupItemTemplate;}set {groupItemTemplate = value;if(value != null && this.Collection != null)this.Collection.Modify();}
        }
        /// <summary>
        /// Gets / sets the template used to display the header section of the GroupingGridWebControl contol.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        /// <remarks>
        /// This template lets you customize the display of header section in the GroupingGridWebControl. 
        /// The ContainerType of this template is GridCellTemplated.
        /// </remarks>
        [Description("Gets / sets the template used to display the header section of the GroupingGridWebControl contol."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore(),
        NotifyParentProperty(true)
        ]
        public virtual ITemplate HeaderTemplate {get {return headerTemplate;}set {headerTemplate = value;if(value != null && this.Collection != null)this.Collection.Modify();}
        }
        /// <summary>
        /// Gets / sets the template used for displaying the footer section of the GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        /// <remarks>
        /// This template lets you customize the display of the footer section in GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </remarks>
        [Description("Gets / sets the template used for displaying the footer section of the GroupingGridWebControl control."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore(),
        NotifyParentProperty(true)
        ]
        public virtual ITemplate FooterTemplate {get {return footerTemplate;}set {footerTemplate = value;if(value != null && this.Collection != null)this.Collection.Modify();}
        }

        public ITemplate GetItemTemplate()
        {
            if(this.itemTemplate != null)
                return this.itemTemplate;
            else
                return this.TableDescriptor.ItemTemplate;
        }

        public ITemplate GetHeaderTemplate()
        {
            if(this.headerTemplate != null)
                return this.headerTemplate;
            else
                return this.TableDescriptor.HeaderTemplate;
        }

        public ITemplate GetEditItemTemplate()
        {
            if(this.editItemTemplate != null)
                return this.editItemTemplate;
            else
                return this.TableDescriptor.EditItemTemplate;
        }

        public ITemplate GetGroupItemTemplate()
        {
            if(this.groupItemTemplate != null)
                return this.groupItemTemplate;
            else
                return this.TableDescriptor.GroupItemTemplate;
        }

        public ITemplate GetRowBtnTemplate()
        {
                return this.TableDescriptor.RowBtnTemplate;
        }

        #endregion

#endif
        //// FieldDescriptorCollection IItemPropertiesSource.GetItemProperties()
        ////{
        //// return tableDescriptor.ItemProperties;
        ////}

        internal void SetCollection(GridColumnDescriptorCollection collection)
        {
            this.collection = collection;
            this.tableDescriptor = collection._tableDescriptor;
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridColumnDescriptorCollection Collection
        {
            get
            {
                return collection;
            }
        }

        internal void SetTableDescriptor(GridTableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
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

#if !ASPNET
            this.avgCharWidth = -1;
#endif
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
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
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public virtual GridColumnDescriptor Clone()
        {
            GridColumnDescriptor newColumnDescriptor = new GridColumnDescriptor();
            newColumnDescriptor.InitializeFrom(this);
            return newColumnDescriptor;
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
            else if (!(obj is GridColumnDescriptor))
            {
                return false;
            }

            return InternalEquals((GridColumnDescriptor)obj);
        }

        /* /// <summary>
        // /// Copies all properties to another element without raising Changing or Changed events.
        // /// </summary>
        // /// <param name="fd">The target object.</param>
        // protected void CopyAllMembersTo(GridColumnDescriptor fd)
        // {
        // GridTableCellAppearance apperance = new GridTableCellAppearance();
        //  appearance.InitializeFrom(this.appearance);
        // fd.appearance = appearance;
        //
        // fd.collection = this.collection;
        //
        // FieldDescriptor field = new FieldDescriptor();
        // field.InitializeFrom(this.field);
        // fd.field = field;
        //
        // GridTableCellAppearance groupByAppearance = new GridTableCellAppearance();
        // groupByAppearance.InitializeFrom(this.groupByAppearance);
        // fd.groupByAppearance = groupByAppearance;
        //
        // GridGroupOptionsStyleInfo groupOptions = new GridTableCellAppearance();
        // groupOptions.InitializeFrom(this.groupOptions);
        // fd.groupOptions = groupOptions;
        //
        // }*/

        bool InternalEquals(GridColumnDescriptor other)
        {
            return other.name == name
                && other.Appearance.Equals(Appearance)
                && other.GroupByAppearance.Equals(GroupByAppearance)
                && other.GroupByOptions.Equals(GroupByOptions)
                && other.ShouldSerializeHeaderText() == ShouldSerializeHeaderText()
                && other.headerText == headerText
                && other.ShouldSerializeImageArray() == ShouldSerializeImageArray()
                && other.mappingName == mappingName
                && other.readOnly == readOnly
                && other.widthModified == widthModified
                && other.width == width
                && other.allowFilter == allowFilter
                && other.allowSort == allowSort
                && other.trackWidthOfParentColumn == trackWidthOfParentColumn;
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
        /// The name of the column.
        /// </summary>
        [RefreshProperties(RefreshProperties.All), NotifyParentProperty(true)]
        [Description("The name of the column"), Category("Design")]
        public virtual string Name
        {
            get
            {
                return name;
            }

            set
            {
                bool raiseChangeEvent = name != value;
                if (raiseChangeEvent)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                }

                string str = name;
                name = value;
                try
                {
                    nameModified = true;
                    if (raiseChangeEvent)
                    {
                        OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                    }

                    if (!ShouldSerializeHeaderText())
                    {
                        headerText = name;
                    }
                }
                catch
                {
                    name = str;
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Determines if the column's name was modified.
        /// </summary>
        /// <returns>True if name is modified.</returns>
        public bool ShouldSerializeName()
        {
            return nameModified;
        }

        /// <summary>
        /// Resets the column's name.
        /// </summary>
        public void ResetName()
        {
            Name = this.MappingName;
            nameModified = false;
        }

        /// <summary>
        /// Gets the field descriptor this descriptor holds. If this column descriptor is
        /// not a bound column, the method will return NULL.
        /// </summary>
        /// <returns>A field descriptor.</returns>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FieldDescriptor FieldDescriptor
        {
            get
            {
                if (Collection != null)
                    Collection.EnsureInitialized(true);
                return field;
            }
        }

        internal bool InitalizeMapping(TableDescriptor tableDescriptor)
        {
            field = tableDescriptor.Fields[this.MappingName];

            if (field != null && !this.ShouldSerializeHeaderText())
            {
                System.Data.DataColumn dataColumn = ListUtil.GetDataColumn(field.GetPropertyDescriptor());
                if (dataColumn != null)
                {
                    this.headerText = dataColumn.Caption;
                }
            }

            return field != null;
        }

        internal bool isImageApplied = false;
        private Image headerImage;
        /// <summary>
        /// Gets or sets the image to draw in header cell
        /// </summary>
        [XmlIgnore]
        public Image HeaderImage
        {
            get
            {
                if (headerImage == null && this.SerializedImageArray!= string.Empty)
                {
                    byte[] array = Convert.FromBase64String(this.SerializedImageArray);
                    headerImage = Image.FromStream(new MemoryStream(array));
                    return headerImage;
                }
                else
                    return headerImage;
            }
            set
            {
                if (headerImage != value)
                {
                    headerImage = value;
                    if (value != null)
                    {
                        isImageApplied = true;
                        MemoryStream ms = new MemoryStream();
                        headerImage.Save(ms, headerImage.RawFormat);
                        byte[] array = ms.ToArray();

                        this.SerializedImageArray = Convert.ToBase64String(array);

                        if (!this.TableDescriptor.columnImageCollection.ContainsKey(this.Name))
                            this.TableDescriptor.columnImageCollection.Add(this.Name, this.SerializedImageArray);
                    }
                    else
                    {
                        if (this.TableDescriptor.columnImageCollection.ContainsKey(this.Name))
                            this.TableDescriptor.columnImageCollection.Remove(this.Name);
                        isImageApplied = false;
                    }
                }
            }
        }

        string serializedImageArray = string.Empty;
        /// <summary>
        ///  string which used for serialization and deserialization of header images 
        /// </summary>
        public string SerializedImageArray
        {
            get
            {
                return this.serializedImageArray;
            }
            set
            {
                this.serializedImageArray = value;
            }
        }

        /// <summary>
        /// Determines if the header images  text was modified.
        /// </summary>
        /// <returns>True </returns>
        public bool ShouldSerializeImageArray()
        {
            return true;
        }

        /// <summary>
        /// Resets the image serialization text.
        /// </summary>
        public void ResetImageArray()
        {
            this.serializedImageArray = string.Empty;
        }



        private HeaderImageAlignment headerImageAlignment;
        /// <summary>
        /// Get or set the alignment of image at header
        /// </summary>
        public HeaderImageAlignment HeaderImageAlignment
        {
            get
            {
                return this.headerImageAlignment;
            }
            set
            {
                this.headerImageAlignment = value;
            }
        }


        /// <summary>
        /// The header text to be displayed in the column header. If no header text has been specified,
        /// the header text will be the same as the <see cref="Name"/>.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [LocalizableAttribute(true), NotifyParentProperty(true)]
        [Description("The header text to be displayed in the column header.")]
        public string HeaderText
        {
            get
            {
                return headerText;
            }

            set
            {
                if (headerText != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("HeaderText"));
                    headerText = value;
                    headerModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("HeaderText"));
                }
                else
                {
                    headerModified = true;
                }
            }
        }
        
        /// <summary>
        /// Determines if the column's header text was modified.
        /// </summary>
        /// <returns>True if header text is modified.</returns>
        public bool ShouldSerializeHeaderText()
        {
            return headerModified && headerText.Length != 0;
        }

        /// <summary>
        /// Resets the header text.
        /// </summary>
        public void ResetHeaderText()
        {
            HeaderText = Name;
            headerModified = false;
        }

        /// <summary>
        /// Changes the <see cref="Width"/> property without raising <see cref="PropertyChanged"/> events. The
        /// grid uses this method internally when column widths are automatically resized based on their contents.
        /// </summary>
        /// <param name="value">The new value.</param>
        public void SetWidthInt(int value)
        {
            width = value;
        }

        /// <summary>
        /// Specifies a column in a parent record that the column in the child record should track. Use
        /// this if you have frozen columns and want to make sure that columns in nested records
        /// are properly aligned.
        /// </summary>
        [RefreshProperties(RefreshProperties.All), NotifyParentProperty(true)]
        [Description("Specifies a column in a parent record that the column in the child record should track.")]
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        public string TrackWidthOfParentColumn
        {
            get
            {
                return trackWidthOfParentColumn;
            }

            set
            {
                if (TrackWidthOfParentColumn != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("TrackWidthOfParentColumn"));
                    trackWidthOfParentColumn = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TrackWidthOfParentColumn"));
                }
            }
        }

        /// <summary>
        /// Determines if the TrackWidthOfParentColumn property was modified.
        /// </summary>
        /// <returns>True if it was modified.</returns>
        public bool ShouldSerializeTrackWidthOfParentColumn()
        {
            return TrackWidthOfParentColumn != string.Empty;
        }

        /// <summary>
        /// Resets the TrackWidthOfParentColumn property.
        /// </summary>
        public void ResetTrackWidthOfParentColumn()
        {
            TrackWidthOfParentColumn = string.Empty;
        }

        /// <summary>
        /// Helper method returns relative column index for column taking layout of column sets into consideration. To get the real column index
        /// you need to add TableDescriptor.GetColumnIndentCount().
        /// </summary>
        /// <returns>Relative column index.</returns>
        public int GetRelativeColumnIndex()
        {
            GridTableDescriptor td = this.tableDescriptor;
            if (tableDescriptor != null)
            {
                int row, col;
                tableDescriptor.ColumnToRowColIndex(MappingName, out row, out col);
                return col;
            }

            return -1;
        }

        /// <summary>
        /// Gets / sets the width of the column.
        /// </summary>
        [RefreshProperties(RefreshProperties.All), NotifyParentProperty(true)]
        [Description("Gets / sets the width of the column."), Category("Layout")]
        public int Width
        {
            get
            {
#if !ASPNET
                if (tableDescriptor != null
                    && this.tableDescriptor.ParentRelation != null
                    && TrackWidthOfParentColumn != string.Empty)
                {
                    GridTableDescriptor td1 = tableDescriptor.ParentTableDescriptor;
                    GridColumnDescriptor cd1 = td1.Columns[TrackWidthOfParentColumn];
                    if (cd1 != null)
                    {
                        int w = cd1.Width;

                        if (td1.RecordRowColumns[0, 0].Name == TrackWidthOfParentColumn)
                        {
                            w = w + td1.GetTotalWidthOfRowHeadersAndIndent(true) - tableDescriptor.GetTotalWidthOfRowHeadersAndIndent(true);
                        }

                        return w;
                    }
                }
#endif
                return width;
            }

            set
            {
                if (width != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Width"));
                    width = value;
                    widthModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Width"));
                }
            }
        }

        /// <summary>
        /// Determines if the column's width was modified.
        /// </summary>
        /// <returns>True if width is modified.</returns>
        public bool ShouldSerializeWidth()
        {
            return widthModified && width >= 0 && TrackWidthOfParentColumn == string.Empty;
        }

        /// <summary>
        /// Resets the column width and allows the grid to auto-size the column based on its contents.
        /// </summary>
        public void ResetWidth()
        {
            Width = -1;
            widthModified = false;
        }

        /// <summary>
        /// Resets EditFormColumnIndex.
        /// </summary>
#if ASPNET
#else
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public void ResetEditFormColumnIndex()
        {
            this.EditFormColumnIndex = 0;
        }

        /// <summary>
        /// Determines whether default value was modified.
        /// </summary>
        /// <returns>True if it was modified.</returns>
#if ASPNET
#else
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public bool ShouldSerializeEditFormColumnIndex()
        {
            return this.EditFormColumnIndex != 0;
        }

        /// <summary>
        /// Specifies the maximum length of the formatted text for this column.
        /// This value will be used for calculating the optimal width of a column.
        /// If the value is -1 the engine will determine the optimal width
        /// of a column either by looping through the first n records or
        /// using a SummaryDescriptor that keeps track of the entry with
        /// most characters (see GridTableOptionsStyleInfo.ColumnsMaxLengthStrategy
        /// and GridTableOptionsStyleInfo.ColumnsMaxLengthFirstNRecords).
        /// </summary>
        [RefreshProperties(RefreshProperties.All), NotifyParentProperty(true)]
        [Description("Specifies the maximum length of the formatted text for this column.")]
        public int MaxLength
        {
            get
            {
                return maxLength;
            }

            set
            {
                if (MaxLength != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("MaxLength"));
                    maxLength = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("MaxLength"));
                }
                else
                {
                    maxLength = value;
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="MaxLength"/> has been modified
        /// and should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeMaxLength()
        {
            return maxLength != -1;
        }

        /// <summary>
        /// Discards any changes for <see cref="MaxLength"/>.
        /// </summary>
        public void ResetMaxLength()
        {
            MaxLength = -1;
        }

        /// <summary>
        /// The mapping for this column. You should specify which column of a DataTable
        /// you want to display in the grid at this column.
        /// </summary>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [DefaultValue("")]
        [Description("Specifies which column of a DataTable to be displayed in the grid at this column")]
        [RefreshProperties(RefreshProperties.Repaint), NotifyParentProperty(true)]
        public string MappingName
        {
            get
            {
                return mappingName;
            }

            set
            {
                bool raiseChangeEvent = mappingName != value;
                if (raiseChangeEvent)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("MappingName"));
                }

                string strName = name;
                string strMappingName = mappingName;

                //// if (allowReplaceDotWithUnderscore)
                ////  mappingName = value.Replace(".", "_");
                //// else
                if (!nameModified && (mappingName == string.Empty || InPropertyGrid()))
                {
                    name = value;
                    nameModified = false;
                    field = null;
                    if (!headerModified)
                    {
                        headerText = name;
                    }
                }

                mappingName = value;
                if (mappingName != name)
                {
                    nameModified = true;
                }

                try
                {
                    if (raiseChangeEvent)
                    {
                        OnPropertyChanged(new DescriptorPropertyChangedEventArgs("MappingName"));
                    }
                }
                catch
                {
                    name = strName;
                    mappingName = strMappingName;
                    if (!headerModified)
                    {
                        headerText = name;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Enable/disable sort this column by display member.
        /// </summary>
        [DefaultValue(false),
        Description("Enables sorting this column by display member if set to true"),
        Category("Behavior")]
        public bool SortByDisplayMember
        {
            get
            {
                return sortByDisplayMember;
            }

            set
            {
                if (sortByDisplayMember != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("SortByDisplayMember"));
                    sortByDisplayMember = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("SortByDisplayMember"));
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="SortByDisplayMember"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeSortByDisplayMember()
        {
            return sortByDisplayMember;
        }

        /// <summary>
        /// Discards any changes for the <see cref="SortByDisplayMember"/> property.
        /// </summary>
        public void ResetSortByDisplayMember()
        {
            SortByDisplayMember = false;
        }

        bool InPropertyGrid()
        {
            return (this.collection != null && collection.insideCollectionEditor) || (tableDescriptor != null && tableDescriptor.IsDesignTime());
        }

        ////        static bool allowReplaceDotWithUnderscore = true;
        ////
        ////        /// <summary>
        ////        /// Using dots for field names causes lots of trouble with databinding. This property
        ////        /// ensures that if you accidentaly set a MappingName with a dot, it will replace
        ////        /// all dots with underscores.
        ////        /// </summary>
        ////        public static bool AllowReplaceDotWithUnderscore
        ////        {
        ////            get
        ////            {
        ////                return allowReplaceDotWithUnderscore;
        ////            }
        ////            set
        ////            {
        ////                allowReplaceDotWithUnderscore = value;
        ////            }
        ////        }

        /// <summary>
        /// Gets / sets Read-only state of the column.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Gets / sets Read-only state of the column.")]
        public bool ReadOnly
        {
            get
            {
                return readOnly || (this.field != null && field.ReadOnly);
            }

            set
            {
                if (readOnly != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ReadOnly"));
                    readOnly = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ReadOnly"));
                }
            }
        }

        /// <summary>
        /// Determines if the columns Read-only setting was modified.
        /// </summary>
        /// <returns>True if Read-only setting is modified.</returns>
        public bool ShouldSerializeReadOnly()
        {
            return readOnly != false;
        }

        /// <summary>
        /// Resets column's Read-only setting.
        /// </summary>
        public void ResetReadOnly()
        {
            ReadOnly = false;
        }

        /// <summary>
        /// Gets / sets if column supports FilterBar.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Gets / Sets if column supports FilterBar.")]

        public bool AllowFilter
        {
            get
            {
                return allowFilter;
            }

            set
            {
                if (allowFilter != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowFilter"));
                    allowFilter = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowFilter"));
                }
            }
        }

        /// <summary>
        /// Determines if the column's AllowFilter property was modified.
        /// </summary>
        /// <returns>True if AllowFilter setting is modified.</returns>
        public bool ShouldSerializeAllowFilter()
        {
            return allowFilter != false;
        }

        /// <summary>
        /// Resets columns AllowFilter setting.
        /// </summary>
        public void ResetAllowFilter()
        {
            AllowFilter = false;
        }

        /// <summary>
        /// Determines if the column's AllowSort property was modified.
        /// </summary>
        /// <returns>True if AllowSort setting is modified.</returns>
        public bool ShouldSerializeAllowSort()
        {
            return allowSort != true;
        }

        /// <summary>
        /// Resets columns AllowFilter setting.
        /// </summary>
        public void ResetAllowSort()
        {
            AllowSort = true;
        }

        /// <summary>
        /// Specified whether the grid can show a drop-down list for cells in this column if
        /// column represents a foreign key field from a related table.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Specifies whether the grid can show a drop-down list for cells in this column")]
        public bool AllowDropDownCell
        {
            get
            {
                if (!allowDropDownCellModified && TableDescriptor != null)
                {
                    return this.TableDescriptor.TableOptions.AllowDropDownCell;
                }

                return allowDropDownCell;
            }
            
            set
            {
                if (AllowDropDownCell != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowDropDownCell"));
                    allowDropDownCell = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowDropDownCell"));
                }

                allowDropDownCellModified = true;
            }
        }

        /// <summary>
        /// Determines if the column's AllowDropDownCell property was modified.
        /// </summary>
        /// <returns>True if AllowDropDownCell property was modified.</returns>
        public bool ShouldSerializeAllowDropDownCell()
        {
            return allowDropDownCellModified;
        }

        /// <summary>
        /// Resets columns AllowDropDownCell setting.
        /// </summary>
        public void ResetAllowDropDownCell()
        {
            if (this.ShouldSerializeAllowDropDownCell())
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowDropDownCell"));
                allowDropDownCell = true;
                allowDropDownCellModified = false;
                OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowDropDownCell"));
            }
        }

        /// <summary>
        /// Specifies whether the grid can be grouped by this column when the user drags the column
        /// over the GropDropArea.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Specifies whether the grid can be grouped by this column when the user drags the column over the GropDropArea.")]
        public bool AllowGroupByColumn
        {
            get
            {
                if (!allowGroupByColumnModified)
                {
                    return true;
                }

                return allowGroupByColumn;
            }

            set
            {
                if (AllowGroupByColumn != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowGroupByColumn"));
                    allowGroupByColumn = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowGroupByColumn"));
                }

                allowGroupByColumnModified = true;
            }
        }

        /// <summary>
        /// Determines if the column's AllowGroupByColumn property was modified.
        /// </summary>
        /// <returns>True if AllowGroupByColumn property was modified.</returns>
        public bool ShouldSerializeAllowGroupByColumn()
        {
            return allowGroupByColumnModified;
        }

        /// <summary>
        /// Resets columns AllowGroupByColumn setting.
        /// </summary>
        public void ResetAllowGroupByColumn()
        {
            if (this.ShouldSerializeAllowGroupByColumn())
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowGroupByColumn"));
                allowGroupByColumn = true;
                allowGroupByColumnModified = false;
                OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowGroupByColumn"));
            }
        }

        /// <summary>
        /// Specifies whether the cell allows being highlighted when its value is changed. In such
        /// case the cell is being highlighted with colors specified in the <see cref="GridGroupingControl.BaseStyles"/> 
        /// collection and the time period specified in <see cref="GridGroupingControl.BlinkTime"/> of the <see cref="GridGroupingControl"/>.
        /// Default value is false.
        /// </summary>
        [NotifyParentProperty(true)]
        [Description("Specifies whether the cell allows being highlighted when its value is changed.")]
        public bool AllowBlink
        {
            get
            {
                if (!allowBlinkModified)
                {
                    return false;
                }

                return allowBlink;
            }

            set
            {
                if (AllowBlink != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowBlink"));
                    allowBlink = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowBlink"));
                }

                allowBlinkModified = true;
            }
        }

        /// <summary>
        /// Determines if the column's AllowBlink property was modified.
        /// </summary>
        /// <returns>True if AllowBlink property was modified.</returns>
        public bool ShouldSerializeAllowBlink()
        {
            return allowBlinkModified;
        }

        /// <summary>
        /// Resets columns AllowBlink setting.
        /// </summary>
        public void ResetAllowBlink()
        {
            if (this.ShouldSerializeAllowBlink())
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowBlink"));
                allowBlink = false;
                allowBlinkModified = false;
                OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowBlink"));
            }
        }

        GridTableCellAppearance appearance;

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements that display data of this column. This property lets you control almost any aspect of
        /// the appearance of the grouping grid like cell backcolor, font, or the cell type.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("Specifies almost every aspect of the appearance of the grid like backcolor, font or the cell type."),
        Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                    appearance.PropertyFilter = GridTableCellAppearance.ColumnDescriptorPropertyFilter;
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        #region IGridTableCellAppearanceSource Members

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetBaseAppearance()
        {
            if (this.TableDescriptor != null)
            {
                return this.TableDescriptor.Appearance;
            }

            return null;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        /// <summary>
        /// Gets the <see cref="GridEngine"/> that this column descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridEngine Engine
        {
            get
            {
                if (this.TableDescriptor != null)
                {
                    return this.TableDescriptor.Engine;
                }

                return null;
            }
        }
        #endregion

        GridTableCellAppearance groupByAppearance;

#if ASPNET
        // Need a set here for the xml serialization (done to serialize data into viewstate) to work properly.
        [XmlElement(typeof(GridTableCellAppearance))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        
        public GridTableCellAppearance GroupByAppearance
        {
            get
            {
                if (groupByAppearance == null)
                {
                    groupByAppearance = new GridTableCellAppearance(this);
                    groupByAppearance.PropertyFilter = GridTableCellAppearance.GroupByDescriptorPropertyFilter;
                }
                return groupByAppearance;
            }
            set
            {
                if(value != null)
                    GroupByAppearance.InitializeFrom(value);
                else
                    this.ResetGroupByAppearance();
            }
        }
#else
        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements in the table that belong to groups that were grouped by this column.
        /// This property only has an effect on the appearance of the table if this column has been added to
        /// the <see cref="Syncfusion.Grouping.TableDescriptor.GroupedColumns"/> collection of a <see cref="TableDescriptor"/>.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        public GridTableCellAppearance GroupByAppearance
        {
            get
            {
                if (groupByAppearance == null)
                {
                    groupByAppearance = new GridTableCellAppearance(this);
                    groupByAppearance.PropertyFilter = GridTableCellAppearance.GroupByDescriptorPropertyFilter;
                }

                return groupByAppearance;
            }
        }
#endif
        /// <summary>
        /// Determines whether <see cref="GroupByAppearance"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGroupByAppearance()
        {
            return groupByAppearance != null && groupByAppearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupByAppearance"/> object.
        /// </summary>
        public void ResetGroupByAppearance()
        {
            if (groupByAppearance != null && groupByAppearance.IsModified)
            {
                groupByAppearance.Reset();
            }
        }

        #region IGridGroupOptionsSource Members
        GridGroupOptionsStyleInfo groupOptions;

        bool IGridGroupOptionsSource.HasGroupOptions
        {
            get
            {
                return groupOptions != null;
            }
        }

        /// <summary>
        /// The <see cref="GridGroupOptionsStyleInfo"/> which lets you control the look and behavior of the child groups.
        /// You can control the caption text, where and if AddNewRow will be displayed, or whether captions, footers, previews and summaries are displayed. <para/>
        /// This property only has an effect on the appearance of the table if this column has been added to
        /// the <see cref="Syncfusion.Grouping.TableDescriptor.GroupedColumns"/> collection of a <see cref="TableDescriptor"/>.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        [Description("Specifies whether to show Caption, GroupHeader, Plusminus... This property only has an effect on the appearance of the table if this column has been added to GroupedColumns collection of a TableDescriptor.")]

        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridGroupOptionsStyleInfo GroupByOptions
        {
            get
            {
                if (groupOptions == null)
                {
                    groupOptions = new GridGroupOptionsStyleInfo(new GridGroupOptionsStyleInfoIdentity(this));
                }

                return groupOptions;
            }
           
            set
            {
                GroupByOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="GroupByOptions"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeGroupByOptions()
        {
            return this.groupOptions != null && !GroupByOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="GroupByOptions"/> object.
        /// </summary>
        public void ResetGroupByOptions()
        {
            if (ShouldSerializeGroupByOptions())
            {
                GroupByOptions = GridGroupOptionsStyleInfo.Empty;
            }
        }

        void IGridGroupOptionsSource.RaiseGroupOptionsChanged(GridGroupOptionsChangedEventArgs e)
        {
            OnPropertyChanged(new DescriptorPropertyChangedEventArgs("GroupByOptions", e));
            ////Engine.RaiseGroupOptionsChanged(e);
        }

        void IGridGroupOptionsSource.RaiseGroupOptionsChanging(GridGroupOptionsChangedEventArgs e)
        {
            OnPropertyChanging(new DescriptorPropertyChangedEventArgs("GroupByOptions", e));
            ////Engine.RaiseGroupOptionsChanged(e);
        }

        GridGroupOptionsStyleInfo IGridGroupOptionsSource.GroupOptions
        {
            get
            {
                return GroupByOptions;
            }
        }

        IGridGroupOptionsSource IGridGroupOptionsSource.GetParentGroupOptionsSource()
        {
            return this.TableDescriptor;
        }

        #endregion
#if !ASPNET
        private float avgCharWidth = -1;

        /// <exclude/>
        /// <summary>Used internally.</summary>
        [System.Xml.Serialization.XmlIgnore, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public float AvgCharWidth
        {
            get
            {
                if (avgCharWidth == -1)
                {
                    if (Engine != null && Engine.ParentControl != null)
                    {
                        GridStyleInfo style = Appearance.AnyRecordFieldCell;
                        Font font = style.GdipFont;
                        using (Graphics g = Engine.ParentControl.TableControl.GetCachedGraphics())
                        {
                            avgCharWidth = g.MeasureString("Abc", font).Width / 3;
                        }
                    }
                }
               
                return avgCharWidth;
            }
           
            set
            {
                avgCharWidth = value;
            }
        }
#endif
    }

    #endregion

    #region QueryAddColumn

    //// eva GridQueryAddColumn SyncfusionCancel GridTableDescriptor tableDescriptor GridColumnDescriptor column

    /// <summary>
    /// Represents a method that handles an event with <see cref="GridQueryAddColumnEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void GridQueryAddColumnEventHandler(object sender, GridQueryAddColumnEventArgs e);

    /// <summary>
    /// The GridEngine.QueryAddColumn event affects the auto-population of the GridColumnDescriptorCollection. <para/>
    /// It is called for each column and lets you control at run-time if the column should be added to the
    /// GridColumnDescriptorCollection. You can set e.Cancel = True to avoid specific columns
    /// being added.
    /// </summary>
    public sealed class GridQueryAddColumnEventArgs : SyncfusionCancelEventArgs
    {
        GridTableDescriptor tableDescriptor;
        GridColumnDescriptor column;

        /// <summary>
        /// Initializes the event args
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor.</param>
        /// <param name="column">The column.</param>
        public GridQueryAddColumnEventArgs(GridTableDescriptor tableDescriptor, GridColumnDescriptor column)
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
        public GridColumnDescriptor GridColumn
        {
            get
            {
                return column;
            }
        }
    }
    #endregion

    /// <summary>
    /// Defines strategy for resizing columns to optimal width.
    /// See also GridColumnDescriptor.MaxLength, GridSummaryColumnDescriptor.MaxLength,
    /// GridTableOptionsStyleInfo.ColumnsMaxLengthStrategy
    /// and GridTableOptionsStyleInfo.ColumnsMaxLengthFirstNRecords
    /// </summary>
    public enum GridColumnsMaxLengthStrategy
    {
        /// <summary>
        /// Initial size for columns will be GridTableOptionsStyleInfo.DefaultColumnWidth
        /// </summary>
        None = 0,

        /// <summary>
        /// A summary is created for each column to keep track of the maximum
        /// length for the column. Best approach for smaller datasources. If
        /// a GridColumnDescriptor.MaxLength was specified by the user, the
        /// GridColumnDescriptor.MaxLength will be used instead.
        /// </summary>
        MaxLengthSummary = 1,

        /// <summary>
        /// The engine will loop through the first n rows at initialization time
        /// and save the width in the table. The number of rows is defined by
        /// GridTableOptionsStyleInfo.ColumnsMaxLengthFirstNRecords. If
        /// a GridColumnDescriptor.MaxLength was specified by the user, the
        /// GridColumnDescriptor.MaxLength will be used instead.
        /// </summary>
        FirstNRecords = 2,

        ////        /// <summary>
        ////        /// When used in combination with <see cref="FirstNRecords"/> the
        ////        /// grdi will resize a column to fit the contents of the currently visible
        ////        /// rows. Without effect if <see cref="MaxLengthSummary"/> is used.
        ////        /// </summary>
        ////        ResizeToVisibleRowsOnDoubleClick = 4,
    }
    /// <summary>
    /// Define Header Image alinement.
    /// </summary>
    public enum HeaderImageAlignment
    {
        /// <summary>
        /// Aligns the image at left of the column : before the header text
        /// </summary>
        Left = 0,

        /// <summary>
        /// Aligns the image at right of the column : next to header text
        /// </summary>
        Right = 1,
    }
}

