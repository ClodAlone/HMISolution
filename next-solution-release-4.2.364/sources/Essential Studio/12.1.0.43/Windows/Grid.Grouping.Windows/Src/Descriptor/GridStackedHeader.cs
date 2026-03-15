//-------------------------------------------------------------------------------------------------
// <copyright file="GridStackedHeader.cs" company="syncfusion">
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
    /// The type converter for <see cref="GridStackedHeaderDescriptor"/> objects. <see cref="GridStackedHeaderDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// ConvertTo method and adds support for design-time code serialization.
    /// </summary>
    public class GridStackedHeaderDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridStackedHeaderDescriptorTypeConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Determines whether this object can be converted to the specified type.
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
        /// Converts the given object to the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">Current culture information.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
                && (value is GridStackedHeaderDescriptor))
            {
                GridStackedHeaderDescriptor column = (GridStackedHeaderDescriptor)value;
                Type type = typeof(GridStackedHeaderDescriptor);

                bool isComplete = !column.ShouldSerializeAppearance();

                if (!isComplete)
                {
                    return new InstanceDescriptor(type.GetConstructor(new Type[0]), new object[0], isComplete);
                }

                if (column.VisibleColumns.Count == 0)
                {
                    if (column.ShouldSerializeHeaderText())
                    {
                        return new InstanceDescriptor(
                            type.GetConstructor(new Type[] { typeof(string), typeof(string) }),
                            new object[] { column.Name, column.HeaderText },
                            isComplete);
                    }
                    else
                    {
                        return new InstanceDescriptor(
                            type.GetConstructor(new Type[] { typeof(string) }),
                            new object[] { column.Name },
                            isComplete);
                    }
                }
                else
                {
                    GridStackedHeaderVisibleColumnDescriptor[] visibleColumns = new GridStackedHeaderVisibleColumnDescriptor[column.VisibleColumns.Count];
                    column.VisibleColumns.CopyTo(visibleColumns, 0);

                    if (column.ShouldSerializeHeaderText())
                    {
                        return new InstanceDescriptor(
                            type.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(GridStackedHeaderVisibleColumnDescriptor[]) }),
                            new object[] { column.Name, column.HeaderText, visibleColumns },
                            isComplete);
                    }
                    else
                    {
                        return new InstanceDescriptor(
                            type.GetConstructor(new Type[] { typeof(string), typeof(GridStackedHeaderVisibleColumnDescriptor[]) }),
                            new object[] { column.Name, visibleColumns },
                            isComplete);
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo

        /// <override/>
        /// <summary>
        /// A collection of properties for the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Value specifying the type.</param>
        /// <param name="attributes">An array of System.Attribute objects that will be used as a filter.</param>
        /// <returns>A list of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "HeaderText",
                "VisibleColumns",
                "Appearance",
            };

            return pds.Sort(atts);
        }
    }

    #endregion

    #region GridStackedHeaderDescriptor

    /// <summary>
    /// A GridStackedHeaderDescriptor declares a StackedHeader column within a StackedHeader row.
    /// GridStackedHeaderDescriptor descriptors are managed by the <see cref="GridStackedHeaderDescriptorCollection"/> which
    /// is returned by the <see cref="GridStackedHeaderRowDescriptor.Headers"/> property
    /// of a <see cref="GridStackedHeaderRowDescriptor"/>.
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
    [TypeConverter(typeof(GridStackedHeaderDescriptorTypeConverter))]
    public class GridStackedHeaderDescriptor : DescriptorBase, ICloneable, IGridTableCellAppearanceSource
    {
        GridStackedHeaderDescriptorCollection parentCollection;
        GridStackedHeaderRowDescriptor _row;
        GridStackedHeaderVisibleColumnDescriptorCollection _visibleColumns;
        string name = string.Empty;
        string headerText = string.Empty;
        bool headerTextModified = false;

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <summary>
        /// Initializes a new empty descriptor.
        /// </summary>
        public GridStackedHeaderDescriptor()
        {
            this.name = string.Empty;
        }

        /// <summary>
        /// Initializes a new descriptor for the specified StackedHeader in the parent table.
        /// </summary>
        /// <param name="name">The descriptor name which also identifies the GridColumnDescriptor.</param>
        public GridStackedHeaderDescriptor(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Initializes a new descriptor for the specified StackedHeader in the parent table.
        /// </summary>
        /// <param name="name">The descriptor name which also identifies the GridColumnDescriptor.</param>
        /// <param name="headerText">The text to be displayed in the grid cell.</param>
        public GridStackedHeaderDescriptor(string name, string headerText)
        {
            this.name = name;
            this.headerText = headerText;
            this.headerTextModified = true;
        }

        /// <summary>
        /// Initializes a new descriptor for the specified StackedHeader in the parent table.
        /// </summary>
        /// <param name="name">The descriptor name which also identifies the GridColumnDescriptor.</param>
        /// <param name="headerText">The text to be displayed in the grid cell.</param>
        /// <param name="columns">The columns that should be displayed as one combined header cell</param>
        public GridStackedHeaderDescriptor(string name, string headerText, GridStackedHeaderVisibleColumnDescriptor[] columns)
        {
            this.name = name;
            this.headerText = headerText;
            this.headerTextModified = true;
            this.VisibleColumns.AddRange(columns);
        }

        /// <summary>
        /// Initializes a new descriptor for the specified StackedHeader in the parent table.
        /// </summary>
        /// <param name="name">The descriptor name which also identifies the GridColumnDescriptor.</param>
        /// <param name="columns">The columns that should be displayed as one combined header cell</param>
        public GridStackedHeaderDescriptor(string name, GridStackedHeaderVisibleColumnDescriptor[] columns)
        {
            this.name = name;
            this.VisibleColumns.AddRange(columns);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_visibleColumns != null)
                {
                    _visibleColumns.Changed -= new ListPropertyChangedEventHandler(visibleColumns_Changed);
                    _visibleColumns.Changing -= new ListPropertyChangedEventHandler(visibleColumns_Changing);
                    _visibleColumns.Dispose();
                    _visibleColumns = null;
                }

                name = "Disposed";
                _row = null;
                parentCollection = null;
            }

            base.Dispose(disposing);
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
                return _row != null ? _row.TableDescriptor : null;
            }
        }

        /// <summary>
        /// The name of this descriptor. This name is used to look up the StackedHeader in the
        /// <see cref="GridStackedHeaderDescriptorCollection"/>.
        /// </summary>
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        [NotifyParentProperty(true)]
        [Description("The name of this descriptor."), Category("Design")]
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
        /// <summary>Gets the descriptor name.</summary>
        /// <returns>Descriptor name.</returns>
        public override string GetName()
        {
            return Name;
        }

        #region HeaderText
        /// <summary>
        /// The headerText text to be displayed in the grid.
        /// </summary>
        [LocalizableAttribute(true)]
        [Description("The headerText text to be displayed in the column headerText.")]
        public string HeaderText
        {
            get
            {
                if (!headerTextModified)
                {
                    return GetName();
                }

                return headerText;
            }

            set
            {
                if (headerText != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("HeaderText"));
                    headerText = value;
                    headerTextModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("HeaderText"));
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="HeaderText"/> has been modified
        /// and its value should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeHeaderText()
        {
            return headerTextModified;
        }

        /// <summary>
        /// Resets the headerText text.
        /// </summary>
        public void ResetHeaderText()
        {
            HeaderText = string.Empty;
            headerTextModified = false;
        }
        #endregion

        /// <summary>
        /// The collection of columns that should be displayed as one combined header cell.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [ListBindableAttribute(false)]
        [Description("The collection of columns that should be displayed as one combined header cell."),
        Category("TableDescriptors")]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
#if ASPNET
        [System.Web.UI.PersistenceMode(System.Web.UI.PersistenceMode.InnerProperty)]
#endif
        public GridStackedHeaderVisibleColumnDescriptorCollection VisibleColumns
        {
            get
            {
                if (_visibleColumns == null)
                {
                    _visibleColumns = new GridStackedHeaderVisibleColumnDescriptorCollection(this);
                    _visibleColumns.Changed += new ListPropertyChangedEventHandler(visibleColumns_Changed);
                    _visibleColumns.Changing += new ListPropertyChangedEventHandler(visibleColumns_Changing);
                }

                return _visibleColumns;
            }
        }

        private void visibleColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            OnPropertyChanged(new DescriptorPropertyChangedEventArgs("VisibleColumns", e));
        }

        private void visibleColumns_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            OnPropertyChanging(new DescriptorPropertyChangedEventArgs("VisibleColumns", e));
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridStackedHeaderDescriptor other)
        {
            this.Name = other.Name;
            if (other.ShouldSerializeHeaderText())
            {
                this.HeaderText = other.HeaderText;
            }
            else
            {
                this.ResetHeaderText();
            }

            this.Appearance.InitializeFrom(other.Appearance);
            this.VisibleColumns.InitializeFrom(other.VisibleColumns);
        }

        internal void SetCollection(GridStackedHeaderDescriptorCollection parentCollection)
        {
            this.parentCollection = parentCollection;
            this._row = parentCollection.parentRow;
            if (this.name == string.Empty)
            {
                this.name = "StackedHeader " + this.parentCollection.Count;
            }

            try
            {
            }
            catch (FormatException ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (parentCollection.insideCollectionEditor)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridStackedHeaderDescriptorCollection Collection
        {
            get
            {
                return parentCollection;
            }
        }

        /// <summary>
        /// The GridStackedHeaderRowDescriptor this StackedHeader belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridStackedHeaderRowDescriptor ParentRow
        {
            get
            {
                return _row;
            }
        }

        internal void SetParentRow(GridStackedHeaderRowDescriptor row)
        {
            this._row = row;

            try
            {
            }
            catch (FormatException ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (parentCollection.insideCollectionEditor)
                {
                    throw;
                }
            }
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

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            if (parentCollection != null)
            {
                parentCollection.RaisePropertyItemChanged(this, e);
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
        public GridStackedHeaderDescriptor Clone()
        {
            GridStackedHeaderDescriptor cd = new GridStackedHeaderDescriptor();
            cd.InitializeFrom(this);
            return cd;
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
            else if (!(obj is GridStackedHeaderDescriptor))
            {
                return false;
            }

            return Equals((GridStackedHeaderDescriptor)obj);
        }

        bool Equals(GridStackedHeaderDescriptor other)
        {
            return Object.ReferenceEquals(other._row, _row)
                && other.name == name
                && other.headerText == this.headerText
                && other.headerTextModified == this.headerTextModified
                && other.VisibleColumns.Equals(this.VisibleColumns)
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

        GridTableCellAppearance appearance;

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements that display data of this stackedHeader column.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Specifies the appearance settings for all cell elements that display data of this stackedHeader column."), Category("Look And Feel")]
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

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool HasApparance
        {
            get { return ShouldSerializeAppearance(); }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetBaseAppearance()
        {
            if (this.ParentRow != null)
            {
                return this.ParentRow.Appearance;
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
        /// Gets the <see cref="GridEngine"/> that this StackedHeader descriptor belongs to.
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
    }

    #endregion

    #region GridStackedHeaderDescriptorCollection

    /// <summary>
    /// A collection of <see cref="GridStackedHeaderDescriptor"/> that declares StackedHeader columns within a StackedHeader row.
    /// An instance of this collection is returned by the <see cref="GridStackedHeaderRowDescriptor.Headers"/> property
    /// of a <see cref="GridStackedHeaderRowDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// Each group in the GridTable has a GridStackedHeaderSection. The StackedHeader section has as many
    /// rows as there are GridStackedHeaderRowDescriptors. Each GridStackedHeaderRowDescriptor
    /// has a collection of GridStackedHeaderDescriptor columns. The GridStackedHeaderDescriptor
    /// defines the VisibleColumns or ColumnSets for which a combined header should be displayed 
    /// before the normal column headers.
    /// </remarks>
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [ListBindableAttribute(false)]
    public class GridStackedHeaderDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        ArrayList inner = new ArrayList();
        internal int version;
        internal bool insideCollectionEditor = false;
        internal GridStackedHeaderRowDescriptor parentRow;

        /// <summary>
        /// Occurs after a property in a nested element or the collection was changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;

        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public GridStackedHeaderDescriptorCollection()
        {
        }

        internal GridStackedHeaderDescriptorCollection(GridStackedHeaderRowDescriptor parentRow)
        {
            this.parentRow = parentRow;
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
            return String.Format("GridStackedHeaderDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        /// <summary>
        /// Gets / sets whether the collection was manipulated inside the collection editor.
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
                    TraceUtil.TraceCurrentMethodInfo(this, value);
                }
#else
                ;
#endif
            }
        }

        void IInsideCollectionEditorProperty.InitializeFrom(object other)
        {
            InitializeFrom((GridStackedHeaderDescriptorCollection)other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridStackedHeaderDescriptorCollection other)
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
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="stackedHeaderDescriptors">The array with elements that should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(GridStackedHeaderDescriptor[] stackedHeaderDescriptors)
        {
            this.inner.AddRange(stackedHeaderDescriptors);
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                ////Console.WriteLine("Add " + this[n].Name);
                this[n].SetCollection(this);
            }
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public GridStackedHeaderDescriptorCollection Clone()
        {
            GridStackedHeaderDescriptorCollection coll = new GridStackedHeaderDescriptorCollection();
            coll.version = this.version + 1000;
            coll.inner = new ArrayList();
            coll.parentRow = parentRow;
            int count = Count;
            GridStackedHeaderDescriptor[] columnDescriptors = new GridStackedHeaderDescriptor[count];
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
            else if (!(obj is GridStackedHeaderDescriptorCollection))
            {
                return false;
            }

            return Equals((GridStackedHeaderDescriptorCollection)obj);
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

        bool Equals(GridStackedHeaderDescriptorCollection other)
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
        public GridStackedHeaderDescriptor this[int index]
        {
            get
            {
                return (GridStackedHeaderDescriptor)inner[index];
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
#if ASPNET
        public GridStackedHeaderDescriptor GetStackedHeaderColDescriptor(string name){return this[name];}
        // Designer ser. doesn't like overloaded accessors.
        internal
#else
        public
#endif
 GridStackedHeaderDescriptor this[string name]
        {
            get
            {
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (GridStackedHeaderDescriptor)inner[index];
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

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(GridStackedHeaderDescriptor value)
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
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(GridStackedHeaderDescriptor value)
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
        public void CopyTo(GridStackedHeaderDescriptor[] array, int index)
        {
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        GridStackedHeaderDescriptorCollection SyncRoot
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
        public GridStackedHeaderDescriptorCollectionEnumerator GetEnumerator()
        {
            return new GridStackedHeaderDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridStackedHeaderDescriptor value)
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
        /// in the collection the method will do nothing.</param>
        public void Remove(GridStackedHeaderDescriptor value)
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
        public int Add(GridStackedHeaderDescriptor value)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));

            if (value.Name != null && value.Name.Length > 0)
            {
                if (Contains(value.Name))
                {
                    throw new Exception(String.Format("StackedHeader '{0}': Duplicates are not allowed ", value.Name));
                }
            }

            int index = inner.Add(value);
            value.SetCollection(this);
            ////Console.WriteLine("Add " + value.Name);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The System.Collections.ArrayList index at which the GridStackedHeaderDescriptor has been added.</returns>
        /// <exclude/>
        public int InnerAdd(GridStackedHeaderDescriptor value)
        {
            return inner.Add(value);
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
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            inner.Clear();
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
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

        internal void RaisePropertyItemChanging(GridStackedHeaderDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, IndexOf(column), e.PropertyName);
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

        internal void RaisePropertyItemChanged(GridStackedHeaderDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, IndexOf(column), e.PropertyName);
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
                this[index] = (GridStackedHeaderDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridStackedHeaderDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((GridStackedHeaderDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridStackedHeaderDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridStackedHeaderDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((GridStackedHeaderDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridStackedHeaderDescriptor[])array, index);
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

        ////internal void RemoveSilent(string name, GridStackedHeaderDescriptor owner)
        ////{
        ////foreach (GridStackedHeaderDescriptor header in inner)
        ////{
        ////if (!Object.ReferenceEquals(header, owner))
        ////{
        ////if (header.VisibleColumns.Contains(name))
        ////header.VisibleColumns.Remove(name);
        ////}
        ////}
        ////}

        ////internal int GetHeaderSpanPos(GridStackedHeaderSpan cd)
        ////{
        ////return cd.index;
        ////}
    }

    /// <summary>
    /// Enumerator class for <see cref="GridStackedHeaderDescriptor"/> elements of a <see cref="GridStackedHeaderDescriptorCollection"/>.
    /// </summary>
    public class GridStackedHeaderDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridStackedHeaderDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="parentCollection">The parent collection to enumerate.</param>
        public GridStackedHeaderDescriptorCollectionEnumerator(GridStackedHeaderDescriptorCollection parentCollection)
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
        public GridStackedHeaderDescriptor Current
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
