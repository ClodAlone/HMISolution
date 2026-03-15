//-------------------------------------------------------------------------------------------------
// <copyright file="GridConditionalFormat.cs" company="syncfusion">
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
using System.Xml;
using System.Web.UI;
using RecordFilterDescriptorCollection = Syncfusion.Web.UI.WebControls.Grid.Grouping.GridRecordFilterDescriptorCollection;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    #region TypeConverter
    /// <summary>
    /// The type converter for <see cref="GridConditionalFormatDescriptor"/> objects. <see cref="GridConditionalFormatDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class GridConditionalFormatDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridConditionalFormatDescriptorTypeConverter()
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
        /// Converts the given value to the speified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">Current culture information.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="destinationType">Type to convert to.</param>
        /// <returns>Converted object.</returns>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor) && (value is GridConditionalFormatDescriptor))
            {
                GridConditionalFormatDescriptor summaryRow = (GridConditionalFormatDescriptor)value;
                Type type = typeof(GridConditionalFormatDescriptor);

                return new InstanceDescriptor(type.GetConstructor(new Type[0]), null, false);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } //// end of method ConvertTo

        ////        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        ////        {
        ////            System.ComponentModel.PropertyDescriptorCollection pds
        ////                = TypeDescriptor.GetProperties(value.GetType(), attributes);
        ////
        ////            string[] atts = new string[]
        ////            {
        ////                "Name",
        ////                "Title",
        ////                "RecordFilters"
        ////            };
        ////
        ////            return pds.Sort(atts);
        ////        }
    }

    #endregion

    #region GridConditionalFormatDescriptor

    /// <summary>
    /// <see cref="GridConditionalFormatDescriptor"/> provides filter criteria for displaying a
    /// subset of records from the underlying datasource with conditional cell formatting.
    /// <para/>
    /// Conditional format descriptors are managed by the <see cref="GridConditionalFormatDescriptorCollection"/> that
    /// is returned by the <see cref="GridTableDescriptor.ConditionalFormats"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(GridConditionalFormatDescriptorTypeConverter))]
    public class GridConditionalFormatDescriptor : DescriptorBase, ICloneable, IGridTableCellAppearanceSource
    {
        string name = string.Empty;
        GridConditionalFormatDescriptorCollection parentCollection;
        RecordFilterDescriptorCollection recordFilters;
        GridTableDescriptor tableDescriptor;
        string expression = string.Empty;
        string compiledExpression = null;

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <overload>
        /// Initializes new conditional format descriptors.
        /// </overload>
        /// <summary>
        /// Initializes new empty conditional format descriptors.
        /// </summary>
        public GridConditionalFormatDescriptor()
        {
            recordFilters = new RecordFilterDescriptorCollection();
            WireRecordFilters();
        }

        /// <summary>
        /// Initializes new empty conditional format descriptors with a name.
        /// </summary>
        /// <param name="name">The name of the descriptor.</param>
        public GridConditionalFormatDescriptor(string name)
            : this()
        {
            this.name = name;
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

        /// <override/>
        /// <summary>Gets the descriptor name.</summary>
        /// <returns>Descriptor name.</returns>
        public override string GetName()
        {
            return Name;
        }

        void WireRecordFilters()
        {
            recordFilters.Changed += new ListPropertyChangedEventHandler(recordFilters_Changed);
            recordFilters.Changing += new ListPropertyChangedEventHandler(recordFilters_Changing);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                parentCollection = null;
                if (recordFilters != null)
                {
                    recordFilters.Dispose();
                }

                recordFilters = null;
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
        public void InitializeFrom(GridConditionalFormatDescriptor other)
        {
            this.Name = other.Name;
            this.RecordFilters.InitializeFrom(other.RecordFilters);
            this.Appearance.InitializeFrom(other.Appearance);
            this.Expression = other.Expression;
        }

        internal void SetCollection(GridConditionalFormatDescriptorCollection parentCollection)
        {
            this.parentCollection = parentCollection;
            if (parentCollection.tableDescriptor != null)
            {
                this.tableDescriptor = parentCollection.tableDescriptor;
            }

            if (this.name == string.Empty)
            {
                this.name = "ConditionalFormat " + this.parentCollection.Count;
            }

            RecordFilters.TableDescriptor = tableDescriptor;
            lastCompareRecord = null;
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("The collection that this descriptor belongs to."),
        Category("TableDescriptors")]
        [Browsable(false)]
        public GridConditionalFormatDescriptorCollection Collection
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
        [Description("The TableDescriptor that this descriptor belongs to."),
        Category("TableDescriptors")]
        public GridTableDescriptor TableDescriptor
        {
            get
            {
                return tableDescriptor;
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
        public GridConditionalFormatDescriptor Clone()
        {
            GridConditionalFormatDescriptor rd = new GridConditionalFormatDescriptor();
            rd.InitializeFrom(this);
            rd.WireRecordFilters();
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
            else if (!(obj is GridConditionalFormatDescriptor))
            {
                return false;
            }

            return Equals((GridConditionalFormatDescriptor)obj);
        }

        bool Equals(GridConditionalFormatDescriptor other)
        {
            return other.name == name
                && other.RecordFilters.Equals(this.RecordFilters)
                && other.Appearance.Equals(this.Appearance)
                && other.Expression == Expression;
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
        /// Gets the collection of <see cref="RecordFilterDescriptor"/> objects defining filter criteria
        /// for records in the table. Each <see cref="RecordFilterDescriptor"/> in the collection references
        /// one or multiple <see cref="FieldDescriptor"/> of the <see cref="Syncfusion.Grouping.TableDescriptor.Fields"/> collection. Multiple
        /// criteria can be combined with logical "And" or "Or" operations.
        /// </summary>
        [Category("Conditions Filter")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Description("Gets the collection of RecordFilterDescriptor objects defining filter criteria for records in the table. Multiple criteria can be combined with logical 'And' or 'Or' operations.")]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
#if ASPNET
        [System.Web.UI.PersistenceMode(System.Web.UI.PersistenceMode.InnerProperty)]
#endif
        public RecordFilterDescriptorCollection RecordFilters
        {
            get
            {
                return recordFilters;
            }
        }

        /// <summary>
        /// Determines if the <see cref="RecordFilters"/> collection contains values.
        /// </summary>
        /// <returns>True if it contains values; False otherwise.</returns>
        public bool ShouldSerializeRecordFilters()
        {
            return RecordFilters.Count > 0;
        }

        /// <summary>
        /// Clears the <see cref="RecordFilters"/> collection.
        /// </summary>
        public void ResetRecordFilters()
        {
            RecordFilters.Clear();
        }

        /// <summary>
        /// A formula expression similar to expressions used in the <see cref="ExpressionFieldDescriptor"/>.
        /// </summary>
        [Category("Expression Filter")]
        [DefaultValue("")]
        [NotifyParentProperty(true)]
        [Description("A formula expression similar to expressions used in the ExpressionFieldDescriptor.")]
        public string Expression
        {
            get
            {
                return this.expression;
            }

            set
            {
                if (this.expression != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Expression"));
                    this.expression = value;
                    this.ResetCompiledExpression();
                    string s = GetCompiledExpression(); // Force recalc
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Expression"));
                }
            }
        }

        int fieldsVersion = -1;

        /// <summary>
        /// Gets a string that holds pre-compiled information about the expression.
        /// </summary>
        /// <returns>A string that holds pre-compiled information about the expression.</returns>
        public string GetCompiledExpression()
        {
            if (expression.Length > 0 && TableDescriptor != null)
            {
                int tableDescriptorfieldsVersion = this.TableDescriptor.Fields.Version;
                if (this.fieldsVersion != tableDescriptorfieldsVersion)
                {
                    IExpressionFieldEvaluator eval = GetExpressionEvaluator();
                    string s = eval.PutTokensInFormula(expression.ToLower());
                    compiledExpression = eval.Parse(s);
                    this.fieldsVersion = tableDescriptorfieldsVersion;
                }
            }

            return compiledExpression;
        }

        /// <summary>
        /// Resets the compiled expression. It will be recompiled later on demand.
        /// </summary>
        public void ResetCompiledExpression()
        {
            compiledExpression = null;
            this.fieldsVersion = -1;
        }

        IExpressionFieldEvaluator GetExpressionEvaluator()
        {
            return TableDescriptor.ExpressionFieldEvaluator;
        }

        /// <summary>
        /// The name of this conditional format. This name is used to look up items in the ConditionalFormats collection of the parent table descriptor.
        /// </summary>
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        [NotifyParentProperty(true)]
        [Description("The name of this conditional format."),
        Category("TableDescriptors")]
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

        private void recordFilters_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            OnPropertyChanging(new DescriptorPropertyChangedEventArgs("RecordFilters", e));
            lastCompareRecord = null;
        }

        private void recordFilters_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            //// foreach (RecordFilterDescriptor recordFilter in recordFilters)
            ////if (recordFilter.ParentRow != this)
            //// return;
            OnPropertyChanged(new DescriptorPropertyChangedEventArgs("RecordFilters", e));
        }

        GridTableCellAppearance appearance;

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for field cell elements in a record that matches the filter criteria specified with
        /// <see cref="Expression"/> or <see cref="RecordFilters"/>.
        /// </summary>
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Specifies the appearance settings for field cell elements in a record that matches the filter criteria specified with Expressions or RecordFilters.")]
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
                    appearance.PropertyFilter = GridTableCellAppearance.ConditionalFormatDescriptorPropertyFilter;
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
        /// Gets the <see cref="GridEngine"/> that this conditional format descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("Gets the GridEngine that this conditional format descriptor belongs to."),
        Category("TableDescriptors")]
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

        bool lastCompareRecordResult = false;
        Record lastCompareRecord = null;
        int lastCompareRecordVersion = -1;

        /// <summary>
        /// Evaluates this filter for the given record and returns True if the record
        /// meets the filters criteria.
        /// </summary>
        /// <param name="record">The record to be evaluated.</param>
        /// <returns>True if the record
        /// meets the filters criteria; False otherwise.</returns>
        public bool CompareRecord(Record record)
        {
            if (lastCompareRecord != null && Object.ReferenceEquals(lastCompareRecord, record)
                && lastCompareRecordVersion == record.ParentTable.SourceListVersion)
            {
                return lastCompareRecordResult;
            }

            bool result = false;

            if (Expression != string.Empty)
            {
                string expr = GetCompiledExpression();
                IExpressionFieldEvaluator eval = this.GetExpressionEvaluator();
                if (eval != null && expr != null)
                {
                    string s = eval.ComputeFormulaValueAt(expr, record);
                    if (s != string.Empty)
                    {
                        result = s == "1";
                    }
                }
            }

            if (RecordFilters.Count > 0)
            {
                result |= RecordFilters.CompareRecord(record);
            }

            lastCompareRecordResult = result;
            lastCompareRecord = record;
            lastCompareRecordVersion = record.ParentTable.SourceListVersion;

            return result;
        }
    }

    #endregion

    #region GridConditionalFormatDescriptorCollection

    /// <summary>
    /// A collection of <see cref="GridConditionalFormatDescriptor"/> which provide filter criteria for displaying a
    /// subset of records from the underlying datasource with conditional cell formatting.<para/>
    /// An instance of this collection is returned by the <see cref="GridTableDescriptor.ConditionalFormats"/> property
    /// of a <see cref="GridTableDescriptor"/>.
    /// </summary>
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [ListBindableAttribute(false)]
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    public class GridConditionalFormatDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
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
            return String.Format("GridConditionalFormatDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
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
            InitializeFrom((GridConditionalFormatDescriptorCollection)other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridConditionalFormatDescriptorCollection other)
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
        public GridConditionalFormatDescriptorCollection()
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

        internal GridConditionalFormatDescriptorCollection(GridTableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public GridConditionalFormatDescriptorCollection Clone()
        {
            GridConditionalFormatDescriptorCollection coll = new GridConditionalFormatDescriptorCollection(this.tableDescriptor);
            coll.version = this.version + 1000;
            int count = Count;
            GridConditionalFormatDescriptor[] columnDescriptors = new GridConditionalFormatDescriptor[count];
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
            else if (!(obj is GridConditionalFormatDescriptorCollection))
            {
                return false;
            }

            return Equals((GridConditionalFormatDescriptorCollection)obj);
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

        bool Equals(GridConditionalFormatDescriptorCollection other)
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
        public GridConditionalFormatDescriptor this[int index]
        {
            get
            {
                return (GridConditionalFormatDescriptor)inner[index];
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
        public GridConditionalFormatDescriptor GetSummaryColDescriptor(string name){return this[name];}
        // Designer ser. doesn't like overloaded accessors.
        internal
#else
        public
#endif
 GridConditionalFormatDescriptor this[string name]
        {
            get
            {
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (GridConditionalFormatDescriptor)inner[index];
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
        public bool Contains(GridConditionalFormatDescriptor value)
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
        public int IndexOf(GridConditionalFormatDescriptor value)
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
        public void CopyTo(GridConditionalFormatDescriptor[] array, int index)
        {
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        GridConditionalFormatDescriptorCollection SyncRoot
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
        public GridConditionalFormatDescriptorCollectionEnumerator GetEnumerator()
        {
            return new GridConditionalFormatDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridConditionalFormatDescriptor value)
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
        public void Remove(GridConditionalFormatDescriptor value)
        {
            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.Remove(value);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds a filter descriptor to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(GridConditionalFormatDescriptor value)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            int index = inner.Add(value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Removes the specified descriptor element with the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the element to remove from the collection. If no element with that name is found
        /// in the collection, the method will do nothing.</param>
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
        /// Gets the number of elements contained in the collection.
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

        internal void RaisePropertyItemChanging(GridConditionalFormatDescriptor column, DescriptorPropertyChangedEventArgs e)
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

        internal void RaisePropertyItemChanged(GridConditionalFormatDescriptor column, DescriptorPropertyChangedEventArgs e)
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
                this[index] = (GridConditionalFormatDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridConditionalFormatDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((GridConditionalFormatDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridConditionalFormatDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridConditionalFormatDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((GridConditionalFormatDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridConditionalFormatDescriptor[])array, index);
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
    /// Enumerator class for <see cref="GridConditionalFormatDescriptor"/> elements of a <see cref="GridConditionalFormatDescriptorCollection"/>.
    /// </summary>
    public class GridConditionalFormatDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridConditionalFormatDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="parentCollection">The parent collection to enumerate.</param>
        public GridConditionalFormatDescriptorCollectionEnumerator(GridConditionalFormatDescriptorCollection parentCollection)
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
        public GridConditionalFormatDescriptor Current
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
