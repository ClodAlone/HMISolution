//-------------------------------------------------------------------------------------------------
// <copyright file="GridStackedHeaderRow.cs" company="syncfusion">
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
using System.Xml.Serialization;
using System.Web.UI;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    #region TypeConverter

    /// <summary>
    /// The type converter for <see cref="GridStackedHeaderRowDescriptor"/> objects. <see cref="GridStackedHeaderRowDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// ConvertTo method and adds support for design-time code serialization.
    /// </summary>
    public class GridStackedHeaderRowDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <override/>
        /// <summary>
        /// Determines whether this object can be converted to the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">The type you want to convert to.</param>
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
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor) && (value is GridStackedHeaderRowDescriptor))
            {
                GridStackedHeaderRowDescriptor stackedHeaderRow = (GridStackedHeaderRowDescriptor) value;
                Type type = typeof(GridStackedHeaderRowDescriptor);

                bool isComplete = !stackedHeaderRow.ShouldSerializeAppearance();

                if (!isComplete)
                {
                    return new InstanceDescriptor(type.GetConstructor(new Type[0]), new object[0], isComplete);
                }

                GridStackedHeaderDescriptor[] headers = new GridStackedHeaderDescriptor[stackedHeaderRow.Headers.Count];
                stackedHeaderRow.Headers.CopyTo(headers, 0);

                return new InstanceDescriptor(
                    type.GetConstructor(new Type[] { typeof(string), typeof(GridStackedHeaderDescriptor[]) }),
                    new object[] { stackedHeaderRow.Name, headers },
                    isComplete);
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
            System.ComponentModel.PropertyDescriptorCollection pds    = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "Headers",
                "Appearance"
            };

            return pds.Sort(atts);
        }
    }

    #endregion

    #region GridStackedHeaderRowDescriptor

    /// <summary>
    /// A GridStackedHeaderRowDescriptor declares a stackedHeader row with one or multiple GridStackedHeaderDescriptor elements.
    /// GridStackedHeaderRowDescriptor descriptors are managed by the <see cref="GridStackedHeaderRowDescriptorCollection"/> which
    /// is returned by the <see cref="GridTableDescriptor.StackedHeaderRows"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// Each group in the GridTable has a GridStackedHeaderSection. The StackedHeader section has as many
    /// rows as there are GridStackedHeaderRowDescriptors. Each GridStackedHeaderRowDescriptor
    /// has a collection of GridStackedHeaderDescriptor columns. The GridStackedHeaderDescriptor
    /// defines the VisibleColumns or ColumnSets for which a combined header should be displayed 
    /// before the normal column headers.
    /// <para/>
    /// If you leave the VisibleColumns collection empty than this header will be used as default
    /// header for all columns that were not explicitly associated with another header.
    /// <para/>
    /// So, if you want to just add an extra Caption then you could add a StackedHeaderRow
    /// with only one StackedHeader that has an empty VisibleColumns collection.
    /// </remarks>
    [TypeConverter(typeof(GridStackedHeaderRowDescriptorTypeConverter))]
    public class GridStackedHeaderRowDescriptor : DescriptorBase, ICloneable, IItemPropertiesSource, IGridTableCellAppearanceSource
    {
        #region Fields
        string name = string.Empty;
        GridStackedHeaderRowDescriptorCollection parentCollection;
        GridStackedHeaderDescriptorCollection headers;
        GridTableDescriptor tableDescriptor;
        #endregion
        #region Events
    
        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        #endregion
        #region Init

        /// <overload>
        /// Initializes a new empty descriptor.
        /// </overload>
        /// <summary>
        /// Initializes a new empty descriptor.
        /// </summary>
        public GridStackedHeaderRowDescriptor()
        {
            headers = new GridStackedHeaderDescriptorCollection(this);
            WireStackedHeaders();
        }

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return GetType().Name + " { " + name + " }";
        }

        void WireStackedHeaders()
        {
            headers.Changed += new ListPropertyChangedEventHandler(stackedHeaders_Changed);
            headers.Changing += new ListPropertyChangedEventHandler(stackedHeaders_Changing);
        }

        /// <summary>
        /// Initializes a new descriptor with a row name.
        /// </summary>
        /// <param name="name">The row name.</param>
        public GridStackedHeaderRowDescriptor(string name)
            : this()
        {
            this.name = name;
        }

        /// <summary>
        /// Initializes a new descriptor with a row name and a collection of GridStackedHeaderDescriptors.
        /// </summary>
        /// <param name="name">The row name.</param>
        /// <param name="headers">A collection of GridStackedHeaderDescriptor</param>
        public GridStackedHeaderRowDescriptor(string name, GridStackedHeaderDescriptor[] headers)
            : this()
        {
            this.name = name;
            this.headers.AddRange(headers);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                parentCollection = null;
                if (headers != null)
                {
                    headers.Dispose();
                }

                tableDescriptor = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridStackedHeaderRowDescriptor other)
        {
            this.Name = other.Name;

            this.Headers.InitializeFrom(other.Headers);
            this.Appearance.InitializeFrom(other.Appearance);
        }

        PropertyDescriptorCollection IItemPropertiesSource.GetItemProperties()
        {
            return tableDescriptor.ItemProperties;
        }

        internal void SetCollection(GridStackedHeaderRowDescriptorCollection parentCollection)
        {
            this.parentCollection = parentCollection;
            if (parentCollection.tableDescriptor != null)
            {
                this.tableDescriptor = parentCollection.tableDescriptor;
            }

            if (this.name == string.Empty)
            {
                this.name = "Row " + this.parentCollection.Count;
            }

            foreach (GridStackedHeaderDescriptor sd in this.Headers)
            {
                sd.SetCollection(this.Headers);
                sd.SetParentRow(this);
            }
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridStackedHeaderRowDescriptorCollection Collection
        {
            get
            {
                return parentCollection;
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

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public GridStackedHeaderRowDescriptor Clone()
        {
            GridStackedHeaderRowDescriptor rd = new GridStackedHeaderRowDescriptor();
            rd.InitializeFrom(this);
            rd.WireStackedHeaders();
            foreach (GridStackedHeaderDescriptor cd in rd.Headers)
            {
                cd.SetParentRow(rd);
            }

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
            else if (!(obj is GridStackedHeaderRowDescriptor))
            {
                return false;
            }

            return Equals((GridStackedHeaderRowDescriptor) obj);
        }

        bool Equals(GridStackedHeaderRowDescriptor other)
        {
            return other.name == name
                && other.Headers.Equals(this.Headers)
                && other.Appearance.Equals(this.Appearance);
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
        #region PropertyChange

        private void stackedHeaders_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Headers", e));
        }

        private void stackedHeaders_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            ////                foreach (GridStackedHeaderDescriptor stackedHeader in headers)
            ////                    if (stackedHeader.ParentRow != this)
            ////                        return;
            OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Headers", e));
            foreach (GridStackedHeaderDescriptor stackedHeader in headers)
            {
                stackedHeader.SetParentRow(this);
            }
            ////this.ResetDisplayColumns();
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
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

            if (parentCollection != null)
            {
                parentCollection.RaisePropertyItemChanging(this, e);
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

            this.visibleColumnsVersion = -1;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            if (parentCollection != null)
            {
                parentCollection.RaisePropertyItemChanged(this, e);
            }
        }
        #endregion
        #region Headers
        /// <summary>
        /// The collection of <see cref="GridStackedHeaderDescriptor"/> elements.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [NotifyParentProperty(true)]
        [Description("The collection of GridStackedHeaderDescriptor elements.")]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        public GridStackedHeaderDescriptorCollection Headers
        {
            get
            {
                return headers;
            }
        }
        #endregion
        #region Name
        /// <summary>
        /// The name of this descriptor. This name is used to look up the stackedHeader in the
        /// <see cref="GridStackedHeaderRowDescriptorCollection"/>.
        /// </summary>
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of this descriptor.")]
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
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    name = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                }
            }
        }

        /// <override/>
        /// <summary>Gets the name of the descriptor.</summary>
        /// <returns>Descriptor name.</returns>
        public override string GetName()
        {
            return Name;
        }
        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements that display data of this stackedHeader row.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Specifies appearance settings for all cell elements that display data of this stackedHeader row.")]
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
                    appearance.PropertyFilter = GridTableCellAppearance.StackedHeaderDescriptorPropertyFilter;
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
        /// and contents should be serialized at design-time.
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

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool HasApparance
        {
            get { return ShouldSerializeAppearance(); }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetBaseAppearance()
        {
            if (this.TableDescriptor != null)
            {
                return TableDescriptor.Appearance;
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
        /// Gets the <see cref="GridEngine"/> that this stackedHeader column descriptor belongs to.
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
        #endregion

        /// <summary>
        /// Returns the stacked header for a given column.
        /// </summary>
        /// <param name="name">Name of the visible column.</param>
        /// <returns>Stacked header.</returns>
        public GridStackedHeaderDescriptor FindHeaderByVisibleColumn(string name)
        {
            GridStackedHeaderDescriptor emptyHeader = null;

            foreach (GridStackedHeaderDescriptor header in Headers)
            {
                if (header.VisibleColumns.Count == 0)
                {
                    emptyHeader = header;
                }
                else if (header.VisibleColumns.Contains(name))
                {
                    return header;
                }
            }

            return emptyHeader;
        }

        GridStackedHeaderSpan[] headerLayout = null;
        int visibleColumnsVersion = -1;

        /// <summary>
        /// Ensures the layout.
        /// </summary>
        public void EnsureLayout()
        {
            if (headerLayout == null || visibleColumnsVersion != TableDescriptor.VisibleColumns.Version)
            {
                CalculateLayout();
                visibleColumnsVersion = TableDescriptor.VisibleColumns.Version;
            }
        }

        /// <summary>
        /// Performs layout calculation.
        /// </summary>
        public void CalculateLayout()
        {
            int colCount = this.TableDescriptor.CalculateColumnSetCols();
            headerLayout = new GridStackedHeaderSpan[colCount];

            if (colCount > 0)
            {
                int index = 0;
                GridStackedHeaderSpan span = null;
                GridStackedHeaderDescriptor header = null;
                int colNum = 0;
                int count;
                foreach (GridVisibleColumnDescriptor visibleColumn in TableDescriptor.VisibleColumns)
                {
                    GridColumnSetDescriptor columnSet = null;
                    count = 1;
                    string name = visibleColumn.Name;

                    int n = TableDescriptor.ColumnSets.IndexOf(name);
                    if (n != -1)
                    {
                        columnSet = TableDescriptor.ColumnSets[n];
                    }

                    if (columnSet != null)
                    {
                        count = columnSet.GetColCount();
                    }
                    
                    GridStackedHeaderDescriptor hd = this.FindHeaderByVisibleColumn(name);

                    if (index == 0 || hd != header)
                    {
                        span = new GridStackedHeaderSpan();
                        span.firstCol = colNum;
                        span.lastCol = colNum + count - 1;
                        span.header = hd;
                        span.index = index++;
                        span.parentRow = this;
                    }
                    else 
                    {
                        span.lastCol += count;
                    }

                    span.visibleColumns.Add(name);

                    for (int i = 0; i < count; i++)
                    {
                        headerLayout[colNum++] = span;
                    }

                    header = hd;
                }
            }
        }

        /// <summary>
        /// Internal only.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>returns the GridStackedHeaderSpan</returns>
        /// <exclude/>
        public GridStackedHeaderSpan GetStackedHeaderSpanAt(int colIndex)
        {
            EnsureLayout();

            if (colIndex < headerLayout.Length)
            {
                return headerLayout[colIndex];
            }

            return null;
        }
    }

    #endregion

    #region GridStackedHeaderRowDescriptorCollection

    /// <summary>
    /// A collection of <see cref="GridStackedHeaderRowDescriptor"/> objects that declares
    /// StackedHeader rows each with one or multiple GridStackedHeaderDescriptor elements.
    /// An instance of this collection is returned by the <see cref="GridTableDescriptor.StackedHeaderRows"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// Each group in the GridTable has a GridStackedHeaderSection. The StackedHeader section has as many
    /// rows as there are GridStackedHeaderRowDescriptors. Each GridStackedHeaderRowDescriptor
    /// has a collection of GridStackedHeaderDescriptor columns. The GridStackedHeaderDescriptor
    /// defines the VisibleColumns or ColumnSets for which a combined header should be displayed 
    /// before the normal column headers.
    /// <para/>
    /// If you leave the VisibleColumns collection empty than this header will be used as default
    /// header for all columns that were not explicitly associated with another header.
    /// <para/>
    /// So, if you want to just add an extra Caption then you could add a StackedHeaderRow
    /// with only one StackedHeader that has an empty VisibleColumns collection.
    /// </remarks>
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [ListBindableAttribute(false)]
    public class GridStackedHeaderRowDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        ArrayList inner = new ArrayList();
        internal int version;
        internal GridTableDescriptor tableDescriptor;
        internal bool insideCollectionEditor = false;

        /// <summary>
        /// Occurs after a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return String.Format("GridStackedHeaderRowDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
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
            InitializeFrom((GridStackedHeaderRowDescriptorCollection) other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridStackedHeaderRowDescriptorCollection other)
        {
            int i;
            int count = Math.Min(Count, other.Count);
            for (i = 0; i < count; i++)
            {
                this[i].InitializeFrom(other[i]);
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

        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public GridStackedHeaderRowDescriptorCollection()
        {
        }

        internal void SetTableDescriptor(GridTableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                this[n].SetCollection(this);
            }
        }

        internal GridStackedHeaderRowDescriptorCollection(GridTableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public GridStackedHeaderRowDescriptorCollection Clone()
        {
            GridStackedHeaderRowDescriptorCollection coll = new GridStackedHeaderRowDescriptorCollection(this.tableDescriptor);
            coll.version = this.version+1000;
            int count = Count;
            GridStackedHeaderRowDescriptor[] columnDescriptors = new GridStackedHeaderRowDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                coll.inner.Add(this[n].Clone());
                coll[n].SetCollection(coll);
            }

            return coll;
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
            else if (!(obj is GridStackedHeaderRowDescriptorCollection))
            {
                return false;
            }

            return Equals((GridStackedHeaderRowDescriptorCollection) obj);
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
        /// collection or an element within the collection was modified.
        /// </summary>
        public int Version
        {
            get
            {
                return version;
            }
        }

        bool Equals(GridStackedHeaderRowDescriptorCollection other)
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
        public GridStackedHeaderRowDescriptor this[int index]
        {
            get
            {
                return (GridStackedHeaderRowDescriptor) inner[index];
            }

            set
            {
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
        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
#if ASPNET
        public GridStackedHeaderRowDescriptor GetStackedHeaderRowDescriptor(string name){return this[name];}
#endif

#if ASPNET
        // Designer ser. doesn't like overloaded accessors.
        internal
#else
        public
#endif
            GridStackedHeaderRowDescriptor this[string name]
        {
            get
            {
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (GridStackedHeaderRowDescriptor) inner[index];
            }

            set
            {
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

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(GridStackedHeaderRowDescriptor value)
        {
            if (value == null)
            {
                return false;
            }

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
        public int IndexOf(GridStackedHeaderRowDescriptor value)
        {
            return inner.IndexOf(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element that matches the name in the collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(string name)
        {
            return Find(name);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(GridStackedHeaderRowDescriptor[] array, int index)
        {
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        GridStackedHeaderRowDescriptorCollection SyncRoot
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
        public GridStackedHeaderRowDescriptorCollectionEnumerator GetEnumerator()
        {
            return new GridStackedHeaderRowDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridStackedHeaderRowDescriptor value)
        {
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
        public void Remove(GridStackedHeaderRowDescriptor value)
        {
            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.Remove(value);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds a StackedHeaderDescriptor to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(GridStackedHeaderRowDescriptor value)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            int index = inner.Add(value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Removes the a descriptor element that matches the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        public void Remove(string name)
        {
            Remove(this[name]);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            object value = inner[index];
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.RemoveAt(index);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
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
            if (inner.Count > 0)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inner.Clear();
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }
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
        public int Count
        {
            get
            {
                return inner.Count;
            }
        }

        int visibleRowCount = -1;

        /// <summary>
        /// Gets the number of rows that have the <see cref="GridStackedHeaderRowDescriptor"/> property set to True. This
        /// is also the number of stackedHeader rows that are displayed in the grid for each group.
        /// </summary>
        public int VisibleRowCount
        {
            get
            {
                if (visibleRowCount == -1)
                {
                    visibleRowCount = 0;
                    foreach (GridStackedHeaderRowDescriptor row in this)
                    {
                        ////if (row.Visible)
                        visibleRowCount++;
                    }
                }

                return visibleRowCount;
            }
        }

        /// <summary>
        /// Raises the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
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

        internal void RaisePropertyItemChanging(GridStackedHeaderRowDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
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

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            visibleRowCount = -1;
            version++;
            if (!this.InsideCollectionEditor)
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

        internal void RaisePropertyItemChanged(GridStackedHeaderRowDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            visibleRowCount = -1;
            if (!this.InsideCollectionEditor)
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
                this[index] = (GridStackedHeaderRowDescriptor) value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridStackedHeaderRowDescriptor) value);
        }

        void IList.Remove(object value)
        {
            Remove((GridStackedHeaderRowDescriptor) value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridStackedHeaderRowDescriptor) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridStackedHeaderRowDescriptor) value);
        }

        int IList.Add(object value)
        {
            return Add((GridStackedHeaderRowDescriptor) value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridStackedHeaderRowDescriptor[]) array, index);
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

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanging(DescriptorPropertyChangedEventArgs e)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanged(DescriptorPropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
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
    /// Enumerator class for <see cref="GridStackedHeaderRowDescriptor"/> elements of a <see cref="GridStackedHeaderRowDescriptorCollection"/>.
    /// </summary>
    public class GridStackedHeaderRowDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridStackedHeaderRowDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="parentCollection">The parent collection to enumerate.</param>
        public GridStackedHeaderRowDescriptorCollectionEnumerator(GridStackedHeaderRowDescriptorCollection parentCollection)
        {
            _coll = parentCollection;
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
        public GridStackedHeaderRowDescriptor Current
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

    /// <exclude/>
    /// <summary>
    /// Helper class that is created when layout of stacked header columns is calculated.
    /// It associates the header, the columns in the grid and the visible columns and
    /// can be used for quick lookups for QueryCoveredRange operations in the grid.
    /// </summary>
    public class GridStackedHeaderSpan
    {
        internal int firstCol;
        internal int lastCol;
        internal GridStackedHeaderDescriptor header;
        internal GridStackedHeaderRowDescriptor parentRow;
        internal int index;
        internal ArrayList visibleColumns = new ArrayList();

        /// <summary>
        /// Property FirstCol (int)
        /// </summary>
        public int FirstCol
        {
            get
            {
                return this.firstCol;
            }
        }

        /// <summary>
        /// Property LastCol (int)
        /// </summary>
        public int LastCol
        {
            get
            {
                return this.lastCol;
            }
        }

        /// <summary>
        /// Property Header (GridStackedHeaderDescriptor)
        /// </summary>
        public GridStackedHeaderDescriptor Header
        {
            get
            {
                return this.header;
            }
        }

        /// <summary>
        /// Property ParentRow (GridStackedHeaderRowDescriptor)
        /// </summary>
        public GridStackedHeaderRowDescriptor ParentRow
        {
            get
            {
                return this.parentRow;
            }
        }

        /// <summary>
        /// Property Index (int)
        /// </summary>
        public int Index
        {
            get
            {
                return this.index;
            }
        }

        /// <summary>
        /// Property VisibleColumns = new ArrayList() (ArrayList)
        /// </summary>
        public ArrayList VisibleColumns
        {
            get
            {
                return this.visibleColumns;
            }
        }
    }
}
