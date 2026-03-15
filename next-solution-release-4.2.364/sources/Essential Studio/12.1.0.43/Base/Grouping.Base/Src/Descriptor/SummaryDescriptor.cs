//-------------------------------------------------------------------------------------------------
// <copyright file="SummaryDescriptor.cs" company="syncfusion">
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

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping.Internals;

using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Pre-defined summary types to be used in <see cref="SummaryDescriptor"/>.
    /// </summary>
    public enum SummaryType
    {
        /// <summary>
        /// Count items.
        /// </summary>
        Count,

        /// <summary>
        /// Count number of False and / or True items for boolean fields.
        /// </summary>
        BooleanAggregate,

        /// <summary>
        /// Count, Minimum, Maximum, Total, and Average for byte fields.
        /// </summary>
        ByteAggregate,

        /// <summary>
        /// Count, Minimum, and Maximum for char fields.
        /// </summary>
        CharAggregate,

        /// <summary>
        /// The distinct count of elements.
        /// </summary>
        DistinctCount,

        /// <summary>
        /// Count, Minimum, Maximum, Total, and Average for double fields.
        /// </summary>
        DoubleAggregate,

        /// <summary>
        /// Count, Minimum, Maximum, Total, and Average for int32 fields.
        /// </summary>
        Int32Aggregate,

        /// <summary>
        /// Maximum length for text fields.
        /// </summary>
        MaxLength,

        /// <summary>
        /// Maximum length and count for text fields.
        /// </summary>
        StringAggregate,

        /// <summary>
        /// Collects all entries of a column in a sorted vector.
        /// </summary>
        Vector,

        /// <summary>
        /// Collects all entries of a column in a sorted vector. Statistical functions for
        /// Median, Percentile25, Percentile75, and PercentileQ.
        /// </summary>
        DoubleVector,

        /// <summary>
        /// A custom summary type. See the Grid / Grouping / CustomSummaries example.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Create an ISummary object for the given record and SummaryDescriptor.
    /// </summary>
    public delegate ISummary CreateSummaryDelegate(SummaryDescriptor sd, Record record);

    /// <summary>
    /// Create an ISummary object for the given element and SummaryDescriptor.
    /// </summary>
    public delegate ISummary CreateSummaryFromElementDelegate(SummaryDescriptor sd, Element element);

    /// <summary>
    /// The type converter for <see cref="SummaryDescriptor"/> objects. <see cref="SummaryDescriptorTypeConverter"/> 
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the 
    /// ConvertTo method and adds support for design-time code serialization.
    /// </summary>
    public class SummaryDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <override/>
        /// <summary>
        /// Returns a collection of properties for the specified object type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Object type.</param>
        /// <param name="attributes">An array of System.Attribute objects that will be used as a filter.</param>
        /// <returns>A collection of properties.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "MappingName",
                "SummaryType",
                "IsPageLevelSummary",
            };

            return pds.Sort(atts);
        }
    }

    /// <summary>
    /// A SummaryDescriptor
    /// declares summaries for groups in a table.
    /// SummaryDescriptors are managed by the <see cref="SortColumnDescriptorCollection"/> which
    /// is returned by the <see cref="Syncfusion.Grouping.TableDescriptor.Summaries"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(SummaryDescriptorTypeConverter))]
    public class SummaryDescriptor : DescriptorBase, ICloneable, IStandardValuesProvider
    {
        internal string name = string.Empty;
        internal string mappingName = string.Empty;
        internal FieldDescriptor fieldDescriptor = null;
        internal SummaryType summaryType;
        internal Delegate createSummary;
        internal bool nameModified = false;
        TableDescriptor tableDescriptor;
        internal bool ignoreRecordFilter = false;
        internal bool isPageLevelSummary = false;

        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return GetType().Name + " { " + Name + " }";
        }

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        SummaryDescriptorCollection collection;

        /// <summary>
        /// Initializes a new empty object.
        /// </summary>
        public SummaryDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new descriptor for the specified field (mappingName) in the parent table
        /// and summary type.
        /// </summary>
        /// <param name="mappingName">The underlying field name on which calculations are based.</param>
        /// <param name="summaryType">The summary type.</param>
        public SummaryDescriptor(string mappingName, SummaryType summaryType)
            : this(mappingName + summaryType.ToString("G"), mappingName, summaryType)
        {
        }

        /// <summary>
        /// Initializes a new descriptor for the specified field (mappingName) in the parent table
        /// and summary type.
        /// </summary>
        /// <param name="name">The descriptor name.</param>
        /// <param name="mappingName">The underlying field name on which calculations are based.</param>
        /// <param name="summaryType">The summary type.</param>
        public SummaryDescriptor(string name, string mappingName, SummaryType summaryType)
        {
            this.name = name;
            this.mappingName = mappingName;
            this.summaryType = summaryType;
            this.createSummary = null;
        }



        public SummaryDescriptor(string name, string mappingName, SummaryType summaryType, bool isPagesummary)
        {
            this.name = name;
            this.mappingName = mappingName;
            this.summaryType = summaryType;
            this.isPageLevelSummary = isPagesummary;
            this.createSummary = null;
        }

        /// <summary>
        /// Initializes a new descriptor for the specified field (mappingName) in the parent table
        /// and a custom summary.
        /// </summary>
        /// <param name="name">The descriptor name.</param>
        /// <param name="mappingName">The underlying field name on which calculations are based on.</param>
        /// <param name="createSummary">The static summary method that creates a summary object.</param>
        public SummaryDescriptor(string name, string mappingName, CreateSummaryDelegate createSummary)
        {
            this.name = name;
            this.mappingName = mappingName;
            this.summaryType = SummaryType.Custom;
            this.createSummary = createSummary;
        }

        /// <summary>
        /// Initializes a new descriptor for the specified field (mappingName) in the parent table
        /// and a custom summary.
        /// </summary>
        /// <param name="name">The descriptor name.</param>
        /// <param name="mappingName">The underlying field name on which calculations are based.</param>
        /// <param name="createSummary">The static summary method that creates a summary object.</param>
        public SummaryDescriptor(string name, string mappingName, CreateSummaryFromElementDelegate createSummary)
        {
            this.name = name;
            this.mappingName = mappingName;
            this.summaryType = SummaryType.Custom;
            this.createSummary = createSummary;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                createSummary = null;
                fieldDescriptor = null;
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
        public void InitializeFrom(SummaryDescriptor other)
        {
            this.Name = other.Name;
            this.MappingName = other.MappingName;
            this.SummaryType = other.SummaryType;
            this.IgnoreRecordFilterCriteria = other.IgnoreRecordFilterCriteria;
            if (this.SummaryType == SummaryType.Custom)
            {
                if (this.createSummary != other.createSummary)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("CreateSummaryMethod"));
                    this.createSummary = other.createSummary;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("CreateSummaryMethod"));
                }
            }
        }

        internal void SetTableDescriptor(TableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
        }

        ICollection IStandardValuesProvider.GetStandardValues(PropertyDescriptor pd)
        {
            ArrayList al = new ArrayList();
            SortedList sl = new SortedList();
            foreach (FieldDescriptor ppd in tableDescriptor.Fields)
            {
                al.Add(ppd.Name);
            }

            return al;
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

        void UpdateCreateSummaryDelegate()
        {
            switch (summaryType)
            {
                case SummaryType.Count:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(CountSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(CountSummary.CreateSummaryMethod);
                    }
                    break;

                case SummaryType.BooleanAggregate:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(BooleanAggregateSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(BooleanAggregateSummary.CreateSummaryMethod);
                    }
                    break;

                case SummaryType.ByteAggregate:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(ByteAggregateSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(ByteAggregateSummary.CreateSummaryMethod);
                    }
                    break;

                case SummaryType.CharAggregate:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(CharAggregateSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(CharAggregateSummary.CreateSummaryMethod);
                    }
                    
                    break;

                case SummaryType.DistinctCount:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(DistinctCountSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(DistinctCountSummary.CreateSummaryMethod);
                    }
                    break;

                case SummaryType.DoubleAggregate:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(DoubleAggregateSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(DoubleAggregateSummary.CreateSummaryMethod);
                    }
                    
                    break;

                case SummaryType.Int32Aggregate:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(Int32AggregateSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(Int32AggregateSummary.CreateSummaryMethod);
                    }
                    break;

                case SummaryType.MaxLength:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(MaxLengthSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(MaxLengthSummary.CreateSummaryMethod);
                    }
                    break;

                case SummaryType.StringAggregate:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(StringAggregateSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(StringAggregateSummary.CreateSummaryMethod);
                    }
                    break;

                case SummaryType.Vector:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(VectorSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(VectorSummary.CreateSummaryMethod);
                    }
                    break;

                case SummaryType.DoubleVector:
                    if (isPageLevelSummary == true)
                    {
                        createSummary = new CreateSummaryDelegate(DoubleVectorSummary.CreatePageSummaryMethod);
                    }
                    else
                    {
                        createSummary = new CreateSummaryDelegate(DoubleVectorSummary.CreateSummaryMethod);
                    }
                    break;

                case SummaryType.Custom:
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets the value from the record for the field specified with <see cref="MappingName"/>. If record
        /// is an <see cref="AddNewRecord"/>, a NULL value is returned. If MappingName is empty, a reference to
        /// the underlying record data is returned (good for counting RecordCount).
        /// </summary>
        /// <param name="record">The record to be evaluated.</param>
        /// <returns>The value.</returns>
        public object GetValue(Record record)
        {
            // Do not include add new record in summaries ...
            // Do not get values from current record - get original values only
            // summaries will be updated once editing is completed on current record.
            if (record == null || record is AddNewRecord)
            {
                return null;
                ////            else if (record.MeetsFilterCriteria)
                ////                return null;
            }
            else if (this.MappingName == string.Empty)
            {
                return record.GetData();
            }
            else if (FieldDescriptor == null)
            {
                return null;
            }
            else
            {
                return fieldDescriptor.GetValue(record);
            }
        }

        internal void SetCollection(SummaryDescriptorCollection collection)
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
        public SummaryDescriptorCollection Collection
        {
            get
            {
                return collection;
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

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public SummaryDescriptor Clone()
        {
            SummaryDescriptor sd = new SummaryDescriptor();
            sd.name = name;
            sd.mappingName = mappingName;
            sd.summaryType = summaryType;
            sd.ignoreRecordFilter = ignoreRecordFilter;
            sd.createSummary = createSummary;
            sd.nameModified = nameModified;
            return sd;
        }

        /// <summary>Determines if the specified object and current object are equal.</summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if the objects are equal.</returns>
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
            else if (!(obj is SummaryDescriptor))
            {
                return false;
            }

            return Equals((SummaryDescriptor)obj);
        }

        bool Equals(SummaryDescriptor other)
        {
            return other.name == name
                && other.fieldDescriptor == fieldDescriptor
                && other.mappingName == mappingName
                && other.summaryType == summaryType
                && other.ignoreRecordFilter == ignoreRecordFilter
                && other.createSummary == createSummary;
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>Returns the descriptor name.</summary>
        /// <returns>Descriptor name.</returns>
        /// <override/>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// The name of this descriptor. This name is used to look up the summary in the
        /// <see cref="SummaryDescriptorCollection"/>.
        /// </summary>
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
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    nameModified = true;
                    name = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                }
            }
        }


     [DefaultValue(false),
      Description("Calculates the summary for the PageLevel records"),
      NotifyParentProperty(true)
      ]
        public bool IsPageLevelSummary
        {
            get
            {
                return this.isPageLevelSummary;
            }
            set
            {
                if (isPageLevelSummary != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("IsPageLevelSummary"));
                    this.isPageLevelSummary = value;
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("IsPageLevelSummary"));
                }
            }
        }


        /// <summary>
        /// The mapping name which identifies a field in the parent table.
        /// </summary>
        [TypeConverter(typeof(GetItemPropertiesNameConverter))]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        public string MappingName
        {
            get
            {
                return mappingName;
            }

            set
            {
                if (mappingName != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("MappingName"));
                    mappingName = value;
                    if (this.name == string.Empty || !nameModified)
                    {
                        this.name = mappingName + this.summaryType.ToString();
                    }

                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("MappingName"));
                }
            }
        }

        /// <summary>
        /// The field descriptor found for the <see cref="MappingName"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FieldDescriptor FieldDescriptor
        {
            get
            {
                this.collection.EnsureFieldDescriptors();
                return fieldDescriptor;
            }
        }

        internal bool InitFieldDescriptor(TableDescriptor tableDescriptor)
        {
            fieldDescriptor = tableDescriptor.Fields[this.MappingName];
            ////if (fieldDescriptor == null)
            //    Console.WriteLine("SummaryDescriptor " + this.Name + ": Field " + MappingName + " not found.");
            return fieldDescriptor != null;
        }
        
        /// <summary>
        /// The type of summary.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        public SummaryType SummaryType
        {
            get
            {
                return summaryType;
            }

            set
            {
                if (summaryType != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("SummaryType"));
                    summaryType = value;
                    if (this.name == string.Empty || !nameModified)
                    {
                        this.name = mappingName + this.summaryType.ToString();
                    }

                    this.UpdateCreateSummaryDelegate();
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("SummaryType"));
                }
            }
        }

        /// <summary>
        /// Specifies whether RecordFilter criteria should be ignored and
        /// the summary should be calculated for all records.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool IgnoreRecordFilterCriteria
        {
            get
            {
                return ignoreRecordFilter;
            }

            set
            {
                if (ignoreRecordFilter != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("IgnoreRecordFilter"));
                    ignoreRecordFilter = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("IgnoreRecordFilter"));
                }
            }
        }

        /// <summary>
        /// Creates an <see cref="ITreeTableSummary"/> for the specified element. 
        /// </summary>
        /// <param name="element">The element or record.</param>
        /// <returns>A new <see cref="ITreeTableSummary"/> object.</returns>
        /// <remarks>
        /// If the element is a record and <see cref="SummaryDescriptor.IgnoreRecordFilterCriteria"/> is False,
        /// the method checks Record.MeetsFilterCriteria(). If it returns False,
        /// an empty summary is returned. In that case the CreateSummary delegate will 
        /// be called called with a NULL object.
        /// </remarks>
        public ISummary CreateSummary(Element element)
        {
            if (this.createSummary == null)
            {
                this.UpdateCreateSummaryDelegate();
            }

            if (this.createSummary == null)
            {
                return null;
            }
            else
            {
                try
                {
                    if (createSummary is CreateSummaryDelegate)
                    {
                        Record r = element as Record;
                        if (r != null &&
                            this.collection != null && this.collection.AnyDescriptorIgnoreRecordFilter
                            // Above condition makes ensures compatibility with earlier versions 
                            // in case user did override Record.MeetsFilterCriteria()
                            && !(this.ignoreRecordFilter || r.MeetsFilterCriteria()))
                        {
                            return ((CreateSummaryDelegate)createSummary)(this, null);
                        }

                        return ((CreateSummaryDelegate)createSummary)(this, r);
                    }
                    else
                    {
                        return ((CreateSummaryFromElementDelegate)createSummary)(this, element);
                    }
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (element == null)
                    {
                        return null;
                    }

                    return CreateSummary(null);
                }
            }
        }

        /// <summary>
        /// Gets / sets a method that creates ISummary objects for a given record and SummaryDescriptor.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public CreateSummaryDelegate CreateSummaryMethod
        {
            get
            {
                if (this.createSummary == null)
                {
                    this.UpdateCreateSummaryDelegate();
                }

                return createSummary as CreateSummaryDelegate;
            }

            set
            {
                if (createSummary != (Delegate)value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("CreateSummaryMethod"));
                    createSummary = value;
                    this.SummaryType = SummaryType.Custom;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("CreateSummaryMethod"));
                }
            }
        }

        /// <summary>
        /// Gets / sets a method that creates ISummary objects for a given element and SummaryDescriptor.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public CreateSummaryFromElementDelegate CreateSummaryFromElementMethod
        {
            get
            {
                if (createSummary == null)
                {
                    this.UpdateCreateSummaryDelegate();
                }

                return createSummary as CreateSummaryFromElementDelegate;
            }

            set
            {
                if (createSummary != (Delegate)value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("CreateSummaryFromElementMethod"));
                    createSummary = value;
                    this.SummaryType = SummaryType.Custom;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("CreateSummaryFromElementMethod"));
                }
            }
        }
    }

    /// <summary>
    /// A collection from the <see cref="SummaryDescriptor"/> that declares summaries for groups in a table.
    /// An instance of this collection is returned by the <see cref="TableDescriptor.Summaries"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class SummaryDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty
    {
        ArrayList inner = new ArrayList();
        internal int version;
        internal TableDescriptor tableDescriptor;
        internal bool insideCollectionEditor = false;
        int fieldsVersion = -1;
        bool inEnsureFieldDescriptors = false;
        bool anyDescriptorIgnoreRecordFilter = false;
        SortedList sorted;
        
        /// <summary>
        /// Occurs after a property in a nested element or collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;
        
        internal void EnsureFieldDescriptors()
        {
            if (inEnsureFieldDescriptors || inInitializeFrom || tableDescriptor == null)
            {
                return;
            }

            inEnsureFieldDescriptors = true;
            tableDescriptor.EnsureSummaryDescriptors();
            if (inner.Count > 0 && fieldsVersion != this.tableDescriptor.Fields.Version)
            {
                anyDescriptorIgnoreRecordFilter = false;
                foreach (SummaryDescriptor cd in this.inner)
                {
                    cd.SetCollection(this);
                    cd.InitFieldDescriptor(tableDescriptor);
                    anyDescriptorIgnoreRecordFilter |= cd.IgnoreRecordFilterCriteria;
                }

                fieldsVersion = this.tableDescriptor.Fields.Version;
            }

            inEnsureFieldDescriptors = false;
        }

        /// <summary>Returns a sring holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return String.Format("SummaryDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        /// <summary>
        /// Gets whether any summary descriptor has <see cref="SummaryDescriptor.IgnoreRecordFilterCriteria"/> set to True.
        /// </summary>
        public bool AnyDescriptorIgnoreRecordFilter
        {
            get
            {
                return anyDescriptorIgnoreRecordFilter;
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
            InitializeFrom((SummaryDescriptorCollection)other);
        }

        bool inInitializeFrom = false;
        bool inInitializeFromChanged = false;
        bool raiseChangeEvents = true;

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(SummaryDescriptorCollection other)
        {
            InitializeFrom(other, true);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        /// <param name="raiseChangeEvents">Indicates if change events should be raised.</param>
        public void InitializeFrom(SummaryDescriptorCollection other, bool raiseChangeEvents)
        {
            inInitializeFrom = true;
            inInitializeFromChanged = false;
            this.raiseChangeEvents = raiseChangeEvents;

            int i;
            int count = Math.Min(inner.Count, other.Count);
            for (i = 0; i < count; i++)
            {
                this[i].InitializeFrom(other[i]);
            }

            for (; i < other.Count; i++)
            {
                SummaryDescriptor cd = other[i].Clone();
                inner.Add(cd);
                cd.SetCollection(this);
                inInitializeFromChanged = true;
            }

            while (inner.Count > other.Count)
            {
                inner.RemoveAt(inner.Count - 1);
                inInitializeFromChanged = true;
            }

            this.sorted = null;
            inInitializeFrom = false;
            if (inInitializeFromChanged && raiseChangeEvents)
            {
                this.OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }

            this.raiseChangeEvents = true;
        }
        
        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public SummaryDescriptorCollection()
        {
        }

        internal SummaryDescriptorCollection(TableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
        }

        SummaryDescriptorCollection(SummaryDescriptor[] columnDescriptors)
        {
            this.inner.AddRange(columnDescriptors);
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                this[n].SetCollection(this);
            }
        }

        SummaryDescriptorCollection(SummaryDescriptor[] columnDescriptors, int version)
            : this(columnDescriptors)
        {
            this.version = version;
        }

        /// <summary>
        /// Copies all members to another collection.
        /// </summary>
        /// <param name="coll">The target collection.</param>
        protected void CopyAllMembersTo(SummaryDescriptorCollection coll)
        {
            coll.emptySummaries = this.emptySummaries;
            coll.fieldsVersion = -1; ////this.fieldsVersion;
            coll.inEnsureFieldDescriptors = this.inEnsureFieldDescriptors;
            coll.inInitializeFrom = this.inInitializeFrom;
            coll.inInitializeFromChanged = this.inInitializeFromChanged;
            coll.inner = this.inner;
            coll.insideCollectionEditor = this.insideCollectionEditor;
            coll.raiseChangeEvents = this.raiseChangeEvents;
            coll.tableDescriptor = this.tableDescriptor;
            coll.version = this.version + 1000;
            coll.sorted = null;

            coll.inner = new ArrayList();
            int count = Count;
            SummaryDescriptor[] columnDescriptors = new SummaryDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                coll.inner.Add(this[n].Clone());
                coll[n].SetCollection(coll);
            }
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public SummaryDescriptorCollection Clone()
        {
            SummaryDescriptorCollection coll = new SummaryDescriptorCollection();
            return coll;
        }

        /// <summary>Determines if the specified object and current object are equal.</summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if both objects are equal.</returns>
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
            else if (!(obj is SummaryDescriptorCollection))
            {
                return false;
            }

            return Equals((SummaryDescriptorCollection)obj);
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

        bool Equals(SummaryDescriptorCollection other)
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
        public SummaryDescriptor this[int index]
        {
            get
            {
                return (SummaryDescriptor)inner[index];
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
        public SummaryDescriptor this[string name]
        {
            get
            {
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (SummaryDescriptor)inner[index];
            }

            set
            {
                int index = Find(name);
                if (index == -1)
                {
                    Add(value);
                    value.name = name;
                }
                else
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    inner[index] = value;
                    value.name = name;
                    value.SetCollection(this);
                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        internal int Find(string name)
        {
            if (inInitializeFrom)
            {
                for (int n = 0; n < Count; n++)
                {
                    if (this[n].name == name)
                    {
                        return n;
                    }
                }

                return -1;
            }

            if (sorted == null)
            {
                int count = Count;
                SortedList sl = new SortedList();
                for (int n = 0; n < count; n++)
                {
                    sl.Add(this[n].name, n);
                }

                sorted = sl;
            }

            object f = sorted[name];

            if (f != null)
            {
                return (int)f;
            }

            return -1;
        }

        ISummary[] emptySummaries;

        /// <summary>
        /// Gets an array of empty <see cref="ISummary"/> objects. For each SummaryDescriptor in this
        /// collection, an ISummary is created by calling the SummaryDescriptor.CreateSummary method
        /// and passing in NULL as record. 
        /// </summary>
        /// <returns>An array of ISummary objects, one for each SummaryDescriptor in this collection.</returns>
        public ISummary[] EmptySummaries
        {
            get
            {
                if (emptySummaries == null)
                {
                    emptySummaries = CreateSummaries(null);
                }

                return emptySummaries;
            }
        }

        /// <summary>
        /// Helper routine that loops through all SummaryDescriptors of this collection,
        /// calls SummaryDescriptor.CreateSummary(element), and returns all results in an array.
        /// </summary>
        /// <param name="element">The element on which ISummary objects should be based.</param>
        /// <returns>An array of ISummary objects, one for each SummaryDescriptor in this collection.</returns>
        public ISummary[] CreateSummaries(Element element)
        {
            SummaryDescriptorCollection sdc = this;
            int count = sdc.Count;
            ISummary[] summaries = new ISummary[count];
            for (int n = 0; n < count; n++)
            {
                summaries[n] = sdc[n].CreateSummary(element);  // if null - empty summary
            }

            return summaries;
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(SummaryDescriptor value)
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
        public int IndexOf(SummaryDescriptor value)
        {
            if (value == null)
            {
                return -1;
            }

            return Find(value.Name);
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
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(SummaryDescriptor[] array, int index)
        {
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        SummaryDescriptorCollection SyncRoot
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
        public SummaryDescriptorCollectionEnumerator GetEnumerator()
        {
            return new SummaryDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, SummaryDescriptor value)
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
        public void Remove(SummaryDescriptor value)
        {
            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.Remove(value);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds a SummaryDescriptor to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(SummaryDescriptor value)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            int index = inner.Add(value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Creates a SummaryDescriptor and adds it to the end of the collection.
        /// </summary>
        /// <param name="name">The descriptor name.</param>
        /// <param name="mappingName">The underlying field name on which calculations are based on.</param>
        /// <param name="summaryType">The summary type.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name, string mappingName, SummaryType summaryType)
        {
            return Add(new SummaryDescriptor(name, mappingName, summaryType));
        }

        /// <summary>
        /// Creates a SummaryDescriptor and adds it to the end of the collection.
        /// </summary>
        /// <param name="name">The descriptor name.</param>
        /// <param name="mappingName">The underlying field name on which calculations are based.</param>
        /// <param name="createSummary">The static summary method that creates a summary object.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name, string mappingName, CreateSummaryDelegate createSummary)
        {
            return Add(new SummaryDescriptor(name, mappingName, createSummary));
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="values">The array with elements that should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(SummaryDescriptor[] values)
        {
            foreach (SummaryDescriptor sd in values)
            {
                this.Add(sd);
            }
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
        /// Raises the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor && !this.inEnsureFieldDescriptors && raiseChangeEvents)
            {
                if (Changing != null)
                {
                    Changing(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanging(SummaryDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor && !this.inEnsureFieldDescriptors)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, inner.IndexOf(column), column, e.PropertyName, e));
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

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            version++;
            emptySummaries = null;
            this.fieldsVersion = -1;
            this.sorted = null;
            if (!this.InsideCollectionEditor && !this.inEnsureFieldDescriptors && raiseChangeEvents)
            {
                if (inInitializeFrom)
                {
                    inInitializeFromChanged = true;
                    return;
                }
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

        internal void RaisePropertyItemChanged(SummaryDescriptor column, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor && !this.inEnsureFieldDescriptors)
            {
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, inner.IndexOf(column), column, e.PropertyName, e));
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
                this[index] = (SummaryDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (SummaryDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((SummaryDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((SummaryDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((SummaryDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((SummaryDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((SummaryDescriptor[])array, index);
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
    /// Enumerator class for <see cref="SummaryDescriptor"/> elements of a <see cref="SummaryDescriptorCollection"/>.
    /// </summary>
    public class SummaryDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        SummaryDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public SummaryDescriptorCollectionEnumerator(SummaryDescriptorCollection collection)
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
        public SummaryDescriptor Current
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
