//-------------------------------------------------------------------------------------------------
// <copyright file="GridSummaryColumn.cs" company="syncfusion">
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

using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

#if ASPNET
using System.Xml.Serialization;
using System.Web.UI;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Defines whether a summary should fill the entire row, only a column or if it should be hidden.
    /// </summary>
    public enum GridSummaryStyle
    {
        /// <summary>
        /// Do not show this summary in grid. (E.g. if only used as referenced another summaries Format.)
        /// </summary>
        Hidden,

        /// <summary>
        /// Display as a total for the column (typically Count, Min, Max are shown at the column).
        /// </summary>
        Column,

        /// <summary>
        /// Display one line for the whole row.
        /// </summary>
        FillRow,
    }

    #region TypeConverter

    /// <summary>
    /// The type converter for <see cref="GridSummaryColumnDescriptor"/> objects. <see cref="GridSummaryColumnDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// ConvertTo method and adds support for design-time code serialization.
    /// </summary>
    public class GridSummaryColumnDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridSummaryColumnDescriptorTypeConverter()
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
                && (value is GridSummaryColumnDescriptor))
            {
                GridSummaryColumnDescriptor column = (GridSummaryColumnDescriptor)value;
                Type type = typeof(GridSummaryColumnDescriptor);

                if (column.ShouldSerializeAppearance()
                    || column.ShouldSerializeStyle()
                    || column.ShouldSerializeDisplayColumn())
                {
                    return new InstanceDescriptor(type.GetConstructor(new Type[0]), null, false);

                    ////string name, SummaryType summaryType, string dataMember, string format)
                }
                else
                {
                    return new InstanceDescriptor(
                        type.GetConstructor(new Type[] { typeof(string), typeof(SummaryType), typeof(string), typeof(string) }),
                        new object[] { column.Name, column.SummaryType, column.DataMember, column.Format },
                        true);
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
                "SummaryType",
                "DataMember",
                "Format",
                "Style",
                "Column",
            };

            return pds.Sort(atts);
        }
    }

    /// <summary>
    /// The type converter for the <see cref="GridSummaryColumnDescriptor.Format"/> string of a
    /// <see cref="GridSummaryColumnDescriptor"/>. <see cref="GridSummaryColumnDescriptorFormatConverter"/>
    /// overrides the GetStandardValues method and returns possible properties for the chosen
    /// <see cref="GridSummaryColumnDescriptor.SummaryType"/>.
    /// </summary>
    public class GridSummaryColumnDescriptorFormatConverter : TypeConverter
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridSummaryColumnDescriptorFormatConverter()
            : base()
        {
        }

        /// <override/>
        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is
        /// designed for when provided with a format context.
        /// </summary>
        /// <param name="context">A format
        /// context that can be used to extract additional information about the environment
        /// from which this converter is invoked. This parameter or properties of this
        /// parameter can be null. </param>
        /// <returns>
        /// A <see cref="TypeConverter.StandardValuesCollection" />
        /// that holds a standard set of valid values, or null if the data type does not
        /// support a standard set of values.
        /// </returns>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                PropertyDescriptorCollection pdc = PropertyDescriptorCollection.Empty;
                GridSummaryColumnDescriptor lm = context.Instance as GridSummaryColumnDescriptor;
                if (lm != null)
                {
                    SummaryDescriptor sd = lm.SummaryDescriptor;
                    if (sd != null)
                    {
                        pdc = TypeDescriptor.GetProperties(sd.CreateSummary(null), new Attribute[] { new BrowsableAttribute(true) });
                    }
                }

                if (pdc.Count > 0)
                {
                    ArrayList keys = new ArrayList();
                    int count = pdc.Count;
                    for (int index = 0; index < count; index++)
                    {
                        PropertyDescriptor pd = pdc[index];
                        if (pd.IsBrowsable)
                        {
                            keys.Add("{" + pd.Name + "}");
                        }
                    }

                    return new TypeConverter.StandardValuesCollection(keys);
                }
            }

            return new TypeConverter.StandardValuesCollection(new string[] { string.Empty });
        }

        /// <override/>
        /// <summary>
        /// Returns whether the collection of standard values returned from <see
        /// cref="GetStandardValuesExclusive" /> is an
        /// exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">The Format context. </param>
        /// <returns>returns False.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;    //// enables support for late bound scenario
        }

        /// <override/>
        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked
        /// from a list, using the specified context.
        /// </summary>
        /// <param name="context">The Format context. </param>
        /// <returns>
        /// returns true.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <param name="sourceType">The type you want to convert from. </param>       
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <override/>
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>
        /// <param name="context">Format context. </param>
        /// <param name="culture">Current culture. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                return (string)value;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// The type converter for the <see cref="GridSummaryColumnDescriptor.DataMember"/> string of a
    /// <see cref="GridSummaryColumnDescriptor"/>. <see cref="GridSummaryColumnDescriptorDataMemberConverter"/>
    /// overrides the GetStandardValues method and returns a list of names for the
    /// <see cref="GridSummaryColumnDescriptor.DataMember"/>.
    /// </summary>
    public class GridSummaryColumnDescriptorDataMemberConverter : TypeConverter
    {
        /// <override/>
        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is
        /// designed for when provided with a format context.
        /// </summary>
        /// <param name="context">A format
        /// context that can be used to extract additional information about the environment
        /// from which this converter is invoked. This parameter or properties of this
        /// parameter can be null. </param>
        /// <returns>
        /// A <see cref="TypeConverter.StandardValuesCollection" />
        /// that holds a standard set of valid values, or null if the data type does not
        /// support a standard set of values.
        /// </returns>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                PropertyDescriptorCollection pdc = PropertyDescriptorCollection.Empty;
                IItemPropertiesSource lm = context.Instance as IItemPropertiesSource;
                if (lm != null)
                {
                    pdc = lm.GetItemProperties();
                }

                if (pdc.Count > 0)
                {
                    ArrayList keys = new ArrayList();
                    keys.Add("(Record)");
                    int count = pdc.Count;
                    for (int index = 0; index < count; index++)
                    {
                        PropertyDescriptor pd = pdc[index];
                        if (pd.IsBrowsable)
                        {
                            keys.Add(pd.Name);
                        }
                    }

                    return new TypeConverter.StandardValuesCollection(keys);
                }
            }

            return new TypeConverter.StandardValuesCollection(new string[] { string.Empty });
        }
        
        /// <override/>
        /// <summary>
        /// Returns whether the collection of standard values returned from <see
        /// cref="GetStandardValuesExclusive" /> is an
        /// exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">The Format context. </param>
        /// <returns>returns False.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <override/>
        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked
        /// from a list, using the specified context.
        /// </summary>
        /// <param name="context">The Format context. </param>
        /// <returns>returns True.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <override/>
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the
        /// type of this converter, using the specified context.
        /// </summary>       
        /// <param name="context">Format
        /// context. </param>
        /// <param name="sourceType">The type
        /// you want to convert from. </param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <override/>
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified
        /// context and culture information.
        /// </summary>      
        /// <param name="context">Format
        /// context. </param>
        /// <param name="culture">Current culture. </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert. </param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                return (string)value;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    #endregion

    #region GridSummaryColumnDescriptor

    /// <summary>
    /// A GridSummaryColumnDescriptor declares a summary column within a summary row.
    /// GridSummaryColumnDescriptor descriptors are managed by the <see cref="GridSummaryColumnDescriptorCollection"/> which
    /// is returned by the <see cref="GridSummaryRowDescriptor.SummaryColumns"/> property
    /// of a <see cref="GridSummaryRowDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// Each group in the GridTable has a summary section. The summary section has as many
    /// rows as there are GridSummaryRowDescriptors that are visible. Each GridSummaryRowDescriptor
    /// has a collection of GridSummaryColumnDescriptor columns. The GridSummaryColumnDescriptor
    /// defines the GridColumnDescriptor to calculate summary information for
    /// the SummaryType and the target column where the summary should be displayed in the grid.
    /// <para/>
    /// The following example shows how to set up a summary. See also the grid\grouping\CustomSummary example for setting up custom summaries:
    /// </remarks>
    /// <example>
    /// <code lang="C#">
    /// ' Setup an integrated summary
    /// Dim sd0 As New GridSummaryColumnDescriptor()
    /// sd0.DataMember = "Quantity"
    /// sd0.DisplayColumn = "Quantity"
    /// sd0.Format = "{Average:#.00}"
    /// sd0.SummaryType = SummaryType.DoubleAggregate
    /// Me.gridGroupingControl1.TableDescriptor.SummaryRows.Add(New GridSummaryRowDescriptor("Row 0", "Average", sd0))
    /// <para/>
    /// </code>
    /// <code lang="VB">
    /// <para/>
    /// // Setup an integrated summary
    /// GridSummaryColumnDescriptor sd0 = new GridSummaryColumnDescriptor();
    /// sd0.DataMember = "Quantity";
    /// sd0.DisplayColumn = "Quantity";
    /// sd0.Format = "{Average:#.00}";
    /// sd0.SummaryType = SummaryType.DoubleAggregate;
    /// this.gridGroupingControl1.TableDescriptor.SummaryRows.Add(new GridSummaryRowDescriptor("Row 0", "Average", sd0));
    /// </code>
    /// </example>
    [TypeConverter(typeof(GridSummaryColumnDescriptorTypeConverter))]
    public class GridSummaryColumnDescriptor : DescriptorBase, ICloneable, IStandardValuesProvider, IGridTableCellAppearanceSource
    {
        GridSummaryColumnDescriptorCollection parentCollection;
        GridSummaryRowDescriptor _row;
        string name = string.Empty;
        string columnName = string.Empty;
        bool columnNameModified = false;
        bool styleModified = false;
        GridSummaryStyle style = GridSummaryStyle.Column;
        bool formatModified = false;
        internal bool ignoreRecordFilter = false;

        string dataMember = "(Record)";
        internal SummaryType summaryType;
        //// when custom is specified then dataMember is used to look up a SummaryDescriptor from SummaryDescriptorCollection is looked up
        //// --> dataMember: ColumnName or SummaryDescriptorName

        //// this is the summary descriptor that is determined by dataMember and summaryType (readonly, needs initializing from parent class)
        SummaryDescriptor summaryDescriptor;

        int maxLength = -1;

        ///// the propery from the summary that should be displayed, e.g. "Max", "Count" etc.
        string format = string.Empty;
        ////        PropertyDescriptor pd; // the PropertyDescriptor for Summary.PropertyName
        PropertyDescriptor[] pds;
        string formatString;

        /// <override/>
        /// <summary>Gets the name of the descriptor.</summary>
        /// <returns>Descriptor name.</returns>
        public override string GetName()
        {
            return Name;
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
        public GridSummaryColumnDescriptor()
        {
            this.name = string.Empty;
            this.columnName = name;
            this.dataMember = "(Record)";
            this.style = GridSummaryStyle.FillRow;
            this.SummaryType = SummaryType.Count;
            this.Format = "  {Count} Records.";
        }

        /// <summary>
        /// Initializes a new descriptor for the specified column (name) in the parent table.
        /// </summary>
        /// <param name="name">The descriptor name which also identifies the GridColumnDescriptor.</param>
        public GridSummaryColumnDescriptor(string name)
        {
            this.name = name;
            this.columnName = name;
            this.dataMember = name;
            this.style = GridSummaryStyle.Column;
            this.SummaryType = SummaryType.Count;
            this.Format = "{Count}";
        }

        /// <summary>
        /// Initializes a new descriptor for the specified column (name) in the parent table,
        /// the summary type and the target column where the summary should be displayed.
        /// </summary>
        /// <param name="name">The descriptor name.</param>
        /// <param name="summaryType">The summary type.</param>
        /// <param name="dataMember">The target column at which to display the summary.</param>
        /// <param name="format">The format string used to format the text to display in the summary column.
        /// A format string consists of the PropertyName of the summaryType and a format specifier known
        /// from String.Format, e.g. {Average:###.00}.</param>
        public GridSummaryColumnDescriptor(string name, SummaryType summaryType, string dataMember, string format)
        {
            this.name = name;
            this.columnName = dataMember;
            this.dataMember = dataMember;
            this.style = GridSummaryStyle.Column;
            this.SummaryType = summaryType;
            this.Format = format;
        }
        
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                columnName = "Disposed";
                _row = null;
                dataMember = string.Empty;
                summaryDescriptor = null;
                pds = null;
                parentCollection = null;
            }

            base.Dispose(disposing);
        }

#if ASPNET
        private ITemplate displayTemplate = null;

        /// <summary>
        /// Gets / sets the template used for displaying the column in the summary.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Gets / sets the template used to display this column in the summary."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore(),
        NotifyParentProperty(true)
        ]
        public virtual ITemplate DisplayTemplate {get {return this.displayTemplate;}set {this.displayTemplate = value;}
        }
        internal bool AreTemplatesAvailable()
        {
            if(this.displayTemplate != null)
                return true;
            return false;
        }
#endif
        /// <summary>
        /// The name of this descriptor. This name is used to look up the summary in the
        /// <see cref="GridSummaryColumnDescriptorCollection"/>.
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

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridSummaryColumnDescriptor other)
        {
            this.DataMember = other.DataMember;
            this.SummaryType = other.SummaryType;
            this.IgnoreRecordFilterCriteria = other.IgnoreRecordFilterCriteria;
            this.Format = other.Format;
            this.DisplayColumn = other.DisplayColumn;
            this.Style = other.Style;
            this.Name = other.Name;
            this.Appearance.InitializeFrom(other.Appearance);
            if (other.ShouldSerializeMaxLength())
            {
                this.MaxLength = other.MaxLength;
            }
            else
            {
                ResetMaxLength();
            }
#if ASPNET
if(other.DisplayTemplate!=null)
    this.DisplayTemplate=other.DisplayTemplate;
#endif
        }

        internal void SetCollection(GridSummaryColumnDescriptorCollection parentCollection)
        {
            this.parentCollection = parentCollection;
            this._row = parentCollection.parentRow;
            if (this.name == string.Empty)
            {
                this.name = "Summary " + this.parentCollection.Count;
            }

            try
            {
                this.ParseFormat(ShouldRaiseFormatException());
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
        public GridSummaryColumnDescriptorCollection Collection
        {
            get
            {
                return parentCollection;
            }
        }

        /// <summary>
        /// The GridSummaryRowDescriptor this summary column belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridSummaryRowDescriptor ParentRow
        {
            get
            {
                return _row;
            }
        }

        internal void SetParentRow(GridSummaryRowDescriptor row)
        {
            this._row = row;
            this.pds = null;
            this.summaryDescriptor = null;

            try
            {
                this.ParseFormat(ShouldRaiseFormatException());
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
        public GridSummaryColumnDescriptor Clone()
        {
            GridSummaryColumnDescriptor cd = new GridSummaryColumnDescriptor();
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
            else if (!(obj is GridSummaryColumnDescriptor))
            {
                return false;
            }

            return Equals((GridSummaryColumnDescriptor)obj);
        }

        bool Equals(GridSummaryColumnDescriptor other)
        {
            return Object.ReferenceEquals(other._row, _row)
                && other.columnName == columnName
                && other.dataMember == dataMember
                && other.summaryType == summaryType
                && other.name == name
                && other.ignoreRecordFilter == ignoreRecordFilter
                && other.format == format
                && other.maxLength == maxLength
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

        /// <summary>
        /// Specifies whether RecordFilter criteria should be ignored and
        /// the summary should be calculated for all records.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("Specifies whether RecordFilter criteria should be ignored and the summary should be calculated for all records.")]
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
                    this.summaryDescriptor = null;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("IgnoreRecordFilter"));
                }
            }
        }

        /// <summary>
        /// Specifies the maximum length of the formatted text for this summary column.
        /// This value will be used for calculating the optimal width of a column.
        /// The default is the length of the Format text plus 5.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("Specifies the maximum length of the formatted text.")]
        public int MaxLength
        {
            get
            {
                return maxLength == -1 ? this.Format.Length + 5 : maxLength;
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
        /// Gets the <see cref="SummaryDescriptor"/> that is used for calculating the summaries. The
        /// GridEngine internally creates SummaryDescriptors for each GridColumnDescriptor based
        /// on the SummaryType and DataMember defined in the GridColumnDescriptor. If SummaryType.Custom
        /// was specified, the TableDescriptor.QueryCustomSummary event needs to instantiate
        /// the SummaryDescriptor.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public SummaryDescriptor SummaryDescriptor
        {
            get
            {
                if (summaryDescriptor == null)
                {
                    GridTableDescriptor td = this.TableDescriptor;
                    if (td == null || td.Engine == null)
                    {
                        return null;
                    }

                    SummaryDescriptor sd = new SummaryDescriptor(this.ParentRow.Name + this.name, this.DataMember, SummaryType);
                    GridQueryCustomSummaryEventArgs e = new GridQueryCustomSummaryEventArgs(this, sd);
                    td.RaiseQueryCustomSummary(e);
                    summaryDescriptor = e.SummaryDescriptor;
                    summaryDescriptor.IgnoreRecordFilterCriteria = this.ignoreRecordFilter;
                }

                return summaryDescriptor;
            }
        }

        internal void SetSummaryDescriptor(SummaryDescriptor summaryDescriptor)
        {
            this.summaryDescriptor = summaryDescriptor;
        }

        /// <summary>
        /// Returns the name to be used for the <see cref="Syncfusion.Grouping.SummaryDescriptor"/> in the
        /// <see cref="Syncfusion.Grouping.TableDescriptor.Summaries"/> collection. The name is either the
        /// SummaryRowDescriptorName.SummaryColumnDescriporName or DataMember.SummaryType depending on whether a
        /// <see cref="Name"/> was specified for this SummaryColumnDescriptor or not.
        /// </summary>
        /// <returns>Summary descriptor name.</returns>
        public string GetSummaryDescriptorName()
        {
            if (this.ParentRow != null && this.name != string.Empty)
            {
                return this.ParentRow.Name + this.name;
            }

            string dm = GetDataMember();
            if (SummaryType != SummaryType.Custom)
            {
                return (dm == string.Empty ? Name : dm) + SummaryType.ToString();
            }
            else if (SummaryDescriptor != null)
            {
                return SummaryDescriptor.Name;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the index that can be used to look up summary values from the array
        /// returned by <see cref="GetSummaryIndex"/> of the <see cref="Table"/>.
        /// Table.GetSummaries returns summaries in the same order as the SummaryDescriptors
        /// were added to the TableDescriptor.
        /// </summary>
        /// <returns>An integer index to look up summary values from a summary array.</returns>
        public int GetSummaryIndex()
        {
            GridTableDescriptor td = this.TableDescriptor;
            if (td == null)
            {
                return -1;
            }

            string name = GetSummaryDescriptorName();

            return td.Summaries.IndexOf(name);
            ////return td.LookupSummaryIndex(this);
        }

        /// <summary>
        /// Returns the calculated summary value for a specific run-time row element in the table.
        /// </summary>
        /// <param name="table">The parent table.</param>
        /// <param name="row">The row in the table.</param>
        /// <returns>The ISummary object with summary values.</returns>
        public ISummary GetSummary(Table table, Element row)
        {
            if (pds == null)
            {
                this.ParseFormat(false);
            }

            int index = GetSummaryIndex();
            if (index != -1)
            {
                Group g = row.ParentGroup;
                return g.GetSummary(index);
            }

            return null;
        }

        /// <overload>
        /// Returns the display text to be displayed in the summary grid cell.
        /// </overload>
        /// <summary>
        /// Returns the display text to be displayed in the summary grid cell using the calculated
        /// summary value for that specific run-time row element in the table.
        /// </summary>
        /// <param name="table">The parent table.</param>
        /// <param name="row">The row in the table.</param>
        /// <returns>The text to display in the grid cell.</returns>
        /// <example>
        /// These code snippets demonstrates different alternatives to get to the summary text (strong typed vs GridSummaryColumnDescriptor.Format)
        /// <code lang="C#">
        /// <para/>
        /// // Calling this method to demonstrate different alternatives to get to the summary text
        /// string summaryText = GetSummaryText(el.ParentGroup, "SummaryRow 1", "FreightAverage");
        /// <para/>
        /// // Easier is to simple call built-in routine:
        /// string summaryText = GridEngine.GetSummaryText(el.ParentGroup, "SummaryRow 1", "FreightAverage");
        /// <para/>
        ///         /// <summary>
        ///         /// Demonstrates different alternatives to get to the summary text (strong typed vs GridSummaryColumnDescriptor.Format)
        ///         /// </summary>
        ///         public string GetSummaryText(Group group, string summaryRowName, string summaryColumnName)
        ///         {
        ///             GridTable table = (GridTable) group.ParentTable;
        ///             GridTableDescriptor td = table.TableDescriptor;
        ///             GridSummaryRowDescriptor srd = td.SummaryRows[summaryRowName];
        ///             GridSummaryColumnDescriptor scd = srd.SummaryColumns[summaryColumnName];
        /// <para/>
        ///             return GetSummaryText(group, scd);
        ///         }
        /// <para/>
        ///         /// <summary>
        ///         /// Demonstrates different alternatives to get to the summary text (strong typed vs GridSummaryColumnDescriptor.Format)
        ///         /// </summary>
        ///         string GetSummaryText(Group group, GridSummaryColumnDescriptor scd)
        ///         {
        ///             GridTable table = (GridTable) group.ParentTable;
        ///             GridTableDescriptor td = table.TableDescriptor;
        /// <para/>
        ///             string summaryText = string.Empty;
        /// <para/>
        ///             bool use31Code = true;
        ///             if (use31Code)
        ///             {
        ///                 if (scd != null)
        ///                 {
        ///                     // Option 1: GetDisplayText - this is actually the code used when you simply would call
        ///                     // e.Style.Text = ((GridTable) table).GetSummaryText(group, "SummaryRow 1", "FreightAverage");
        ///                     //
        ///                     // Text is formatted as defined in GridSummaryColumnDescriptor.Format
        ///                     summaryText = scd.GetDisplayText(group);
        /// <para/>
        ///                     // or Option 2: Strong typed access to DoubleAggregateSummary.
        ///                     DoubleAggregateSummary summary1 = (DoubleAggregateSummary) group.GetSummary(scd.SummaryDescriptor);
        ///                     summaryText = string.Format("{0:c}", summary1.Average);
        /// <para/>
        ///                     // or Option 3: Use reflection to get "Average" property of summary
        ///                     summaryText = string.Format("{0:c}", group.GetSummaryProperty(scd.SummaryDescriptor, "Average"));
        ///                 }
        /// <para/>
        ///             }
        ///             else
        ///             {
        ///                 // This is the code you had to use in version 3.0 and earlier (still working but bit more complicate)
        ///                 if (scd != null)
        ///                 {
        ///                     SummaryDescriptor sd1 = scd.SummaryDescriptor;
        ///                     if (sd1 != null)
        ///                     {
        ///                         int indexOfSd1 = table.TableDescriptor.Summaries.IndexOf(sd1);
        /// <para/>
        ///                         ISummary sum1 = group.GetSummaries(table)[indexOfSd1];
        ///                         string text1 = scd.GetDisplayText(sum1);
        ///                         summaryText = text1;
        /// <para/>
        ///                         // - or - (access value directly)
        ///                         // strong typed - you have to cast to Int32AggregateSummary.
        /// <para/>
        ///                         DoubleAggregateSummary summary1 = (DoubleAggregateSummary) group.GetSummaries(table)[indexOfSd1];
        ///                         summaryText = string.Format("{0:c}", summary1.Average);
        ///                     }
        ///                 }
        ///             }
        /// <para/>
        ///             return summaryText;
        ///         }
        /// <para/>
        /// </code>
        /// <para/>
        /// <code lang="VB">
        /// ' Calling this method to demonstrate different alternatives to get to the summary text
        /// summaryText = GetSummaryText(el.ParentGroup, "SummaryRow 1", "FreightAverage")
        /// <para/>
        /// ' Easier is to simple call built-in routine:
        /// summaryText = GridEngine.GetSummaryText(el.ParentGroup, "SummaryRow 1", "FreightAverage")
        /// <para/>
        /// <para/>
        ///     '/ <summary>
        ///     '/ Demonstrates different alternatives to get to the summary text (strong typed vs GridSummaryColumnDescriptor.Format)
        ///     '/ </summary>
        ///     Public Function GetSummaryText(ByVal group As Group, ByVal summaryRowName As String, ByVal summaryColumnName As String) As String
        ///         Dim table As GridTable = CType(group.ParentTable, GridTable)
        ///         Dim td As GridTableDescriptor = table.TableDescriptor
        ///         Dim srd As GridSummaryRowDescriptor = td.SummaryRows(summaryRowName)
        ///         Dim scd As GridSummaryColumnDescriptor = srd.SummaryColumns(summaryColumnName)
        /// <para/>
        ///         Return GetSummaryText(group, scd)
        ///     End Function 'GetSummaryText
        /// <para/>
        /// <para/>
        ///     '/ <summary>
        ///     '/ Demonstrates different alternatives to get to the summary text (strong typed vs GridSummaryColumnDescriptor.Format)
        ///     '/ </summary>
        ///     Function GetSummaryText(ByVal group As Group, ByVal scd As GridSummaryColumnDescriptor) As String
        ///         Dim table As GridTable = CType(group.ParentTable, GridTable)
        ///         Dim td As GridTableDescriptor = table.TableDescriptor
        /// <para/>
        ///         Dim summaryText As String = string.Empty
        /// <para/>
        ///         Dim use31Code As Boolean = True
        ///         If use31Code Then
        ///             If Not (scd Is Nothing) Then
        ///                 ' Option 1: GetDisplayText - this is actually the code used when you simply would call
        ///                 ' e.Style.Text = ((GridTable) table).GetSummaryText(group, "SummaryRow 1", "FreightAverage");
        ///                 '
        ///                 ' Text is formatted as defined in GridSummaryColumnDescriptor.Format
        ///                 summaryText = scd.GetDisplayText(group)
        /// <para/>
        ///                 ' or Option 2: Strong typed access to DoubleAggregateSummary.
        ///                 Dim summary1 As DoubleAggregateSummary = CType(group.GetSummary(scd.SummaryDescriptor), DoubleAggregateSummary)
        ///                 summaryText = String.Format("{0:c}", summary1.Average)
        /// <para/>
        ///                 ' or Option 3: Use reflection to get "Average" property of summary
        ///                 summaryText = String.Format("{0:c}", group.GetSummaryProperty(scd.SummaryDescriptor, "Average"))
        ///             End If
        /// <para/>
        ///         Else
        ///             ' This is the code you had to use in version 3.0 and earlier (still working but bit more complicate)
        ///             If Not (scd Is Nothing) Then
        ///                 Dim sd1 As SummaryDescriptor = scd.SummaryDescriptor
        ///                 If Not (sd1 Is Nothing) Then
        ///                     Dim indexOfSd1 As Integer = table.TableDescriptor.Summaries.IndexOf(sd1)
        /// <para/>
        ///                     Dim sum1 As Syncfusion.Collections.BinaryTree.ITreeTableSummary = group.GetSummaries(table)(indexOfSd1)
        ///                     Dim text1 As String = scd.GetDisplayText(sum1)
        ///                     summaryText = text1
        /// <para/>
        ///                     ' - or - (access value directly)
        ///                     ' strong typed - you have to cast to Int32AggregateSummary.
        ///                     Dim summary1 As DoubleAggregateSummary = CType(group.GetSummaries(table)(indexOfSd1), DoubleAggregateSummary)
        ///                     summaryText = String.Format("{0:c}", summary1.Average)
        ///                 End If
        ///             End If
        ///         End If
        /// <para/>
        ///         Return summaryText
        ///     End Function 'GetSummaryText
        /// <para/>
        /// <para/>
        /// <para/>
        /// </code>
        /// </example>
        /// <seealso cref="GridEngine"/>
        /// <seealso cref="Group"/>
        /// <seealso cref="Group"/>
        public string GetDisplayText(Table table, GridSummaryRow row)
        {
            return GetDisplayText(GetSummary(table, row), row.ParentGroup.PassThroughItem);
        }

        /// <summary>
        /// Returns the display text to be displayed in the summary grid cell given
        /// an ISummary.
        /// </summary>
        /// <param name="summary">The ISummary object with summary values.</param>
        /// <param name="groupPassThroughItem">The group Pass Through Item.</param>
        /// <returns>The text to display in the grid cell.</returns>
        /// <genoverload/>
        public string GetDisplayText(ISummary summary, object groupPassThroughItem)
        {
            if (summary == null)
            {
                if (groupPassThroughItem == null)
                {
                    return string.Empty;
                }
                else
                {
                    // formatString, pds are dependant on groupPassThroughItem. I can't assume that each
                    // groupPassThroughItem has the same properties.
                    // As an optimization I could store pds in Group or GroupedColumns level.
                    pds = null;
                }
            }

            if (pds == null)
            {
                this.ParseFormat(false, groupPassThroughItem);
            }
            // if ParseFormat failed ....
            if (pds == null)
            {
                return string.Empty;
            }

            object[] objs = new object[pds.Length];
            for (int n = 0; n < pds.Length; n++)
            {
                if (pds[n] != null)
                {
                    if (summary == null)
                    {
                        if (groupPassThroughItem != null)
                        {
                            objs[n] = pds[n].GetValue(groupPassThroughItem);
                        }
                    }
                    else
                    {
                        objs[n] = pds[n].GetValue(summary);
                    }
                }
                else
                {
                    objs[n] = null;
                }
            }

            return String.Format(this.formatString, objs);
        }

        /// <summary>
        /// Returns the display text to be displayed in the summary grid cell given
        /// an ISummary.
        /// </summary>
        /// <param name="summary">The ISummary object with summary values.</param>
        /// <returns>The text to display in the grid cell.</returns>
        /// <genoverload/>
        public string GetDisplayText(ISummary summary)
        {
            return GetDisplayText(summary, null);
        }

        /// <summary>
        /// Returns the display text to be displayed in the summary grid cell given
        /// an ISummary.
        /// </summary>
        /// <param name="g">The Group with summary values.</param>
        /// <returns>The text to display in the grid cell.</returns>
        /// <genoverload/>
        public string GetDisplayText(Group g)
        {
            return GetDisplayText(g.GetSummary(this.SummaryDescriptor.Name), g.PassThroughItem);
        }

        /// <summary>
        /// The type of summary.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("The type of summary.")]
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
                    this.pds = null;
                    this.summaryDescriptor = null;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("SummaryType"));
                }
            }
        }

        /// <summary>
        /// The format string used to format the text to display in the summary column.
        /// A format string consists of the PropertyName of the summaryType and a format specifier known
        /// from String.Format, e.g. {Average:###.00}.
        /// </summary>
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(GridSummaryColumnDescriptorFormatConverter))]
        [Description("The format string used to format the text to display in the summary column."), Category("Look And Feel")]
        public string Format
        {
            get
            {
                return format;
            }

            set
            {
                if (format != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Format"));
                    format = value;
                    this.pds = null;
                    this.ParseFormat(ShouldRaiseFormatException());
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Format"));
                }
            }
        }

        bool ShouldRaiseFormatException()
        {
            return true; //// TODO: this.ParentRow != null && this.ParentRow.Collection != null && this.ParentRow.Collection.InsideCollectionEditor;
        }

        void ParseFormat(bool raiseException)
        {
            ParseFormat(raiseException, null);
        }

        void ParseFormat(bool raiseException, object groupPassThroughItem)
        {
            SummaryDescriptor sd = this.SummaryDescriptor;
            if (sd == null)
            {
                return;
            }

            PropertyDescriptorCollection pdc;
            ISummary sum = sd.CreateSummary(null);
            if (sum == null)
            {
                // If this is a SummaryType.Custom and no summary method was associated with the summary
                // then parse the groupPassThroughItem which could be a Linq group with summaries.
                if (groupPassThroughItem != null)
                {
                    pdc = TypeDescriptor.GetProperties(groupPassThroughItem, new Attribute[] { new BrowsableAttribute(true) });
                }
                else
                {
                    return;
                }
            }
            else
            {
                pdc = TypeDescriptor.GetProperties(sum, new Attribute[] { new BrowsableAttribute(true) });
            }

            ArrayList al = new ArrayList();
            int n1 = this.Format.IndexOf("{");
            StringBuilder sb = new StringBuilder();
            int n2 = 0;

            if (n1 == -1)
            {
                sb.Append(Format);
            }
            else
            {
                sb.Append(Format.Substring(0, n1 + 1));
            }

            while (n1 != -1)
            {
                n2 = this.Format.IndexOf("}", n1);
                if (n1 != -1 && n2 != -1 && n2 > n1)
                {
                    int n3 = this.Format.IndexOfAny(new char[] { '}', ':' }, n1);
                    string name = Format.Substring(n1 + 1, n3 - n1 - 1);
                    PropertyDescriptor pd = pdc[name];
                    if (pd != null)
                    {
                        sb.Append(al.Count.ToString());
                        al.Add(pd);
                    }
                    else
                    {
                        if (raiseException)
                        {
                            throw new FormatException("Property not found: " + name);
                        }
                        else
                        {
                            this.formatString = string.Empty;
                            this.pds = new PropertyDescriptor[0];
                            return;
                        }
                    }

                    n1 = this.Format.IndexOf("{", n2);
                    if (n1 == -1)
                    {
                        sb.Append(Format.Substring(n3));
                    }
                    else
                    {
                        sb.Append(Format.Substring(n3, n1 - n3 + 1));
                    }
                }
                else
                {
                    if (raiseException)
                    {
                        throw new FormatException("No closing char found: " + sb.ToString());
                    }
                    else
                    {
                        this.formatString = string.Empty;
                        this.pds = new PropertyDescriptor[0];
                        return;
                    }

                    ////                    sb.Append(Format.Substring(n1));
                    ////                    n1 = -1;
                }
            }

            this.formatString = sb.ToString();
            this.pds = (PropertyDescriptor[])al.ToArray(typeof(PropertyDescriptor));
        }

        /// <summary>
        /// The mapping for this column. You should specify here a field name of a FieldDescriptor ( or GridColumnDescriptor.MappingName)
        /// </summary>
        [DefaultValue("(Record)")]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [Description("Specifies a field name of a FieldDescriptor or GridColumnDescriptor.MappingName")]
        public string DataMember
        {
            get
            {
                return dataMember;
            }

            set
            {
                if (dataMember != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("DataMember"));
                    dataMember = value;
                    this.pds = null;
                    this.summaryDescriptor = null;
                    UpdateColumnName();
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("DataMember"));
                }
            }
        }

        /// <summary>
        /// Gets / sets whether a summary should fill the entire row, only a column, or if it should be hidden.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("Gets / sets whether a summary should fill the entire row, only a column, or if it should be hidden."), Category("Look And Feel")]
        public GridSummaryStyle Style
        {
            get
            {
                return style;
            }

            set
            {
                if (style != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Style"));
                    style = value;
                    styleModified = true;
                    UpdateColumnName();
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Style"));
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Style"/> has been modified
        /// and its value should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeStyle()
        {
            return this.styleModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Style"/> property.
        /// </summary>
        public void ResetStyle()
        {
            if (styleModified)
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Style"));
                this.styleModified = false;
                UpdateColumnName();
                OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Style"));
            }
        }

        void UpdateColumnName()
        {
            if (!this.styleModified)
            {
                if (dataMember == "(Record)")
                {
                    style = GridSummaryStyle.FillRow;
                }
                else
                {
                    style = GridSummaryStyle.Column;
                }
            }

            if (!this.columnNameModified)
            {
                if (style == GridSummaryStyle.FillRow)
                {
                    columnName = string.Empty;
                }
                else
                {
                    columnName = dataMember;
                }
            }

            if (!this.formatModified)
            {
                PropertyDescriptorCollection pdc = PropertyDescriptorCollection.Empty;
                SummaryDescriptor sd = SummaryDescriptor;
                if (sd != null)
                {
                    pdc = TypeDescriptor.GetProperties(sd.CreateSummary(null), new Attribute[] { new BrowsableAttribute(true) });
                }

                if (pdc.Count > 0)
                {
                    this.format = "{" + pdc[0].Name + "}";
                }
            }
        }

        internal string GetDataMember()
        {
            string dm = DataMember;
            if (dm == "(Record)")
            {
                return string.Empty;
            }

            return dm;
        }

        #region IStandardValuesProvider Members

        ICollection IStandardValuesProvider.GetStandardValues(PropertyDescriptor propertyDescriptor)
        {
            if (this.TableDescriptor == null)
            {
                return PropertyDescriptorCollection.Empty;
            }

            switch (propertyDescriptor.Name)
            {
                case "DisplayColumn":
                    {
                        if (TableDescriptor != null)
                        {
                            Engine engine = this.TableDescriptor.Engine;
                            ArrayList al = new ArrayList();
                            foreach (GridColumnDescriptor gcd in TableDescriptor.Columns)
                            {
                                al.Add(gcd.Name);
                            }

                            return al.ToArray();
                        }

                        break;
                    }

                case "DataMember":
                    {
                        if (TableDescriptor != null)
                        {
                            Engine engine = this.TableDescriptor.Engine;
                            ArrayList al = new ArrayList();
                            al.Add("(Record)");
                            foreach (FieldDescriptor fd in TableDescriptor.Fields)
                            {
                                al.Add(fd.Name);
                            }

                            return al.ToArray();
                        }

                        break;
                    }
            }

            return null;
        }
        #endregion

        /// <summary>
        /// The target column at which to display the summary.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [Description("The target column at which to display the summary.")]
        public string DisplayColumn
        {
            get
            {
                return columnName;
            }

            set
            {
                if (columnName != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ColumnName"));
                    columnName = value;
                    if (!styleModified)
                    {
                        if (value != string.Empty)
                        {
                            style = GridSummaryStyle.Column;
                        }
                        else if (style == GridSummaryStyle.Column)
                        {
                            style = GridSummaryStyle.FillRow;
                        }
                    }

                    columnNameModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ColumnName"));
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="DisplayColumn"/> has been modified
        /// and its value should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeDisplayColumn()
        {
            return this.columnNameModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="DisplayColumn"/> property.
        /// </summary>
        public void ResetDisplayColumn()
        {
            if (columnNameModified)
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ColumnName"));
                this.columnNameModified = false;
                UpdateColumnName();
                OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ColumnName"));
            }
        }

        int rowInRecord = -1;
        int colInRecord = -1;

        /// <summary>
        /// Returns the row index relative to the first row in the summary section of a group.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int RowInRecord
        {
            get
            {
                if (ParentRow != null)
                {
                    this.ParentRow.EnsureDisplayColumns();
                }

                return rowInRecord;
            }

            set
            {
                rowInRecord = value;
            }
        }

        /// <summary>
        /// Returns the grid column index of the target cell where the summary is displayed.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int ColInRecord
        {
            get
            {
                if (ParentRow != null)
                {
                    this.ParentRow.EnsureDisplayColumns();
                }

                return colInRecord;
            }

            set
            {
                colInRecord = value;
            }
        }
        
        GridTableCellAppearance appearance;

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements that display data of this summary column.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Specifies the appearance settings for all cell elements that display data of this summary column."), Category("Look And Feel")]
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
                    appearance.PropertyFilter = GridTableCellAppearance.SummaryDescriptorPropertyFilter;
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
        /// Gets the <see cref="GridEngine"/> that this summary column descriptor belongs to.
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

    #region GridSummaryColumnDescriptorCollection

    /// <summary>
    /// A collection of <see cref="GridSummaryColumnDescriptor"/> that declares summary columns within a summary row.
    /// GridSummaryColumnDescriptor descriptors.
    /// An instance of this collection is returned by the <see cref="GridSummaryRowDescriptor.SummaryColumns"/> property
    /// of a <see cref="GridSummaryRowDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// Each group in the GridTable has a summary section. The summary section has as many
    /// rows as there are GridSummaryRowDescriptors that are visible. Each GridSummaryRowDescriptor
    /// has a collection of GridSummaryColumnDescriptor columns. The GridSummaryColumnDescriptor
    /// defines the GridColumnDescriptor to calculate summary information for,
    /// the SummaryType, and the target column where the summary should be displayed in the grid.
    /// </remarks>
    /// <example>
    /// The following example shows how to set up a summary:
    /// <code lang="C#">
    /// ' Setup a integrated summary
    /// Dim sd0 As New GridSummaryColumnDescriptor()
    /// sd0.DataMember = "Quantity"
    /// sd0.DisplayColumn = "Quantity"
    /// sd0.Format = "{Average:#.00}"
    /// sd0.SummaryType = SummaryType.DoubleAggregate
    /// Me.gridGroupingControl1.TableDescriptor.SummaryRows.Add(New GridSummaryRowDescriptor("Row 0", "Average", sd0))
    /// <para/>
    /// </code>
    /// <code lang="VB">
    /// <para/>
    /// // Setup a integrated summary
    /// GridSummaryColumnDescriptor sd0 = new GridSummaryColumnDescriptor();
    /// sd0.DataMember = "Quantity";
    /// sd0.DisplayColumn = "Quantity";
    /// sd0.Format = "{Average:#.00}";
    /// sd0.SummaryType = SummaryType.DoubleAggregate;
    /// this.gridGroupingControl1.TableDescriptor.SummaryRows.Add(new GridSummaryRowDescriptor("Row 0", "Average", sd0));
    /// </code>
    /// </example>
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [ListBindableAttribute(false)]
    public class GridSummaryColumnDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        ArrayList inner = new ArrayList();
        internal int version;
        internal bool insideCollectionEditor = false;
        internal GridSummaryRowDescriptor parentRow;

        /// <summary>
        /// Occurs after a property in a nested element or the collection was changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return String.Format("GridSummaryColumnDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
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
            InitializeFrom((GridSummaryColumnDescriptorCollection)other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(GridSummaryColumnDescriptorCollection other)
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
        public GridSummaryColumnDescriptorCollection()
        {
        }

        internal GridSummaryColumnDescriptorCollection(GridSummaryRowDescriptor parentRow)
        {
            this.parentRow = parentRow;
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="summaryColumnDescriptors">The array with elements that should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(GridSummaryColumnDescriptor[] summaryColumnDescriptors)
        {
            for (int i = 0; i < summaryColumnDescriptors.Length; i++)
            {
                if (summaryColumnDescriptors[i].Name != null && summaryColumnDescriptors[i].Name.Length > 0)
                {
                    if (Contains(summaryColumnDescriptors[i].Name))
                    {
                        throw new ArgumentException(String.Format("SummaryColumn '{0}': Duplicates are not allowed ", summaryColumnDescriptors[i].Name));
                    }
                }
                this.inner.Add(summaryColumnDescriptors[i]);
            }

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
        public GridSummaryColumnDescriptorCollection Clone()
        {
            GridSummaryColumnDescriptorCollection coll = new GridSummaryColumnDescriptorCollection();
            coll.version = this.version + 1000;
            coll.inner = new ArrayList();
            coll.parentRow = parentRow;
            int count = Count;
            GridSummaryColumnDescriptor[] columnDescriptors = new GridSummaryColumnDescriptor[count];
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
            else if (!(obj is GridSummaryColumnDescriptorCollection))
            {
                return false;
            }

            return Equals((GridSummaryColumnDescriptorCollection)obj);
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

        bool Equals(GridSummaryColumnDescriptorCollection other)
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
        public GridSummaryColumnDescriptor this[int index]
        {
            get
            {
                return (GridSummaryColumnDescriptor)inner[index];
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
        public GridSummaryColumnDescriptor GetSummaryColDescriptor(string name){return this[name];}
        // Designer ser. doesn't like overloaded accessors.
        internal
#else
        public
#endif
 GridSummaryColumnDescriptor this[string name]
        {
            get
            {
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (GridSummaryColumnDescriptor)inner[index];
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
        public bool Contains(GridSummaryColumnDescriptor value)
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
        public int IndexOf(GridSummaryColumnDescriptor value)
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
        public void CopyTo(GridSummaryColumnDescriptor[] array, int index)
        {
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        GridSummaryColumnDescriptorCollection SyncRoot
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
        public GridSummaryColumnDescriptorCollectionEnumerator GetEnumerator()
        {
            return new GridSummaryColumnDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, GridSummaryColumnDescriptor value)
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
        public void Remove(GridSummaryColumnDescriptor value)
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
        public int Add(GridSummaryColumnDescriptor value)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));

            if (value.Name != null && value.Name.Length > 0)
            {
                if (Contains(value.Name))
                {
                    throw new Exception(String.Format("SummaryColumn '{0}': Duplicates are not allowed ", value.Name));
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
        /// <param name="value">The GridSummaryColumnDescriptor value.</param>
        /// <returns>returns the index at which the GridSummaryColumnDescriptor has been added</returns>
        /// <exclude/>
        public int InnerAdd(GridSummaryColumnDescriptor value)
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

        internal void RaisePropertyItemChanging(GridSummaryColumnDescriptor column, DescriptorPropertyChangedEventArgs e)
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

        internal void RaisePropertyItemChanged(GridSummaryColumnDescriptor column, DescriptorPropertyChangedEventArgs e)
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
                this[index] = (GridSummaryColumnDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridSummaryColumnDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((GridSummaryColumnDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridSummaryColumnDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridSummaryColumnDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((GridSummaryColumnDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridSummaryColumnDescriptor[])array, index);
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
    }

    /// <summary>
    /// Enumerator class for <see cref="GridSummaryColumnDescriptor"/> elements of a <see cref="GridSummaryColumnDescriptorCollection"/>.
    /// </summary>
    public class GridSummaryColumnDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        GridSummaryColumnDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="parentCollection">The parent collection to enumerate.</param>
        public GridSummaryColumnDescriptorCollectionEnumerator(GridSummaryColumnDescriptorCollection parentCollection)
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
        public GridSummaryColumnDescriptor Current
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

    //// eva GridQueryCustomSummary SyncfusionHandled GridSummaryColumnDescriptor summaryColumn SummaryDescriptor result

    /// <summary>
    /// Represents a method that handles events with the <see cref="GridGroupingControl.QueryCustomSummary"/> event that is
    /// raised by <see cref="GridGroupingControl"/>, <see cref="GridTableDescriptor"/>, or <see cref="GridEngine"/> objects.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridQueryCustomSummaryEventHandler(object sender, GridQueryCustomSummaryEventArgs e);

    /// <summary>
    /// Holds a reference to a <see cref="GridSummaryColumnDescriptor"/> and <see cref="SummaryDescriptor"/>
    /// and lets you fill in the <see cref="Syncfusion.Grouping.SummaryDescriptor.CreateSummaryMethod"/> or <see cref="Syncfusion.Grouping.SummaryDescriptor.CreateSummaryFromElementMethod"/>
    /// with a <see cref="CreateSummaryFromElementDelegate"/> that can instantiate custom <see cref="ISummary"/> objects.
    /// </summary>
    /// <remarks>
    /// See the Grid\Grouping\CustomSummaries example how to implement custom summaries:
    /// </remarks>
    /// <example>
    /// <code lang="C#">
    ///         public Form1()
    ///     {
    ///         //
    ///         // Required for Windows Form Designer support
    ///         //
    ///         InitializeComponent();
    /// <para/>
    ///         // Setup custom summaries
    ///         this.gridGroupingControl1.QueryCustomSummary += new GridQueryCustomSummaryEventHandler(gridGroupingControl1_QueryCustomSummary);
    ///
    ///         GridSummaryColumnDescriptor sd1 = new GridSummaryColumnDescriptor();
    ///         sd1.Name = "QuantityTotal";
    ///         sd1.DataMember = "Quantity";
    ///         sd1.DisplayColumn = "Quantity";
    ///         sd1.Format = "{Total}";
    ///         sd1.SummaryType = SummaryType.Custom;
    ///         this.gridGroupingControl1.TableDescriptor.SummaryRows.Add(new GridSummaryRowDescriptor("Row 1", "Total", sd1));
    ///     }
    /// <para/>
    ///     private void gridGroupingControl1_QueryCustomSummary(object sender, GridQueryCustomSummaryEventArgs e)
    ///     {
    ///         switch (e.SummaryColumn.Name)
    ///         {
    ///             case "QuantityTotal":
    ///             {
    ///                 e.SummaryDescriptor.CreateSummaryMethod = new CreateSummaryDelegate(TotalSummary.CreateSummaryMethod);
    ///                 break;
    ///             }
    /// <para/>
    /// <para/>
    ///         }
    ///     }
    /// <para/>
    /// public sealed class TotalSummary : SummaryBase
    /// {
    ///     double _total;
    ///
    ///     public static readonly TotalSummary Empty = new TotalSummary(0);
    ///
    ///     public static ISummary CreateSummaryMethod(SummaryDescriptor sd, Record record)
    ///     {
    ///         object obj = sd.GetValue(record);
    ///         bool isNull = (obj == null || obj is DBNull);
    ///         if (isNull)
    ///             return Empty;
    ///         else
    ///         {
    ///             double val = Convert.ToDouble(obj);
    ///             return new TotalSummary(val);
    ///         }
    ///     }
    ///
    ///     public TotalSummary(double total)
    ///     {
    ///         _total = total;
    ///     }
    ///
    ///     public double Total
    ///     {
    ///         get
    ///         {
    ///             return _total;
    ///         }
    ///     }
    ///
    ///     public override SummaryBase Combine(SummaryBase other)
    ///     {
    ///         return Combine((TotalSummary) other);
    ///     }
    ///
    ///     /// a new summary object instead of modifying an existing summary object.
    ///     public TotalSummary Combine(TotalSummary other)
    ///     {
    ///         // Summary objects are immutable. That means properties cannot be modified for an
    ///         // existing object. Instead every time a change is made a new object must be created (just like
    ///         // System.String).
    ///         //
    ///         // This allows following optimization: return existing summary object if either one of the values is 0. --
    ///         if (other.Total == 0)
    ///             return this;
    ///         else if (Total == 0)
    ///             return other;
    ///             // -- end of optimization
    ///         else
    ///             return new TotalSummary(this.Total + other.Total);
    ///     }
    ///
    ///     public override string ToString()
    ///     {
    ///         return String.Format("Total = {0:0.00}", Total);
    ///     }
    /// }
    /// </code>
    /// <code lang="VB">
    ///
    /// Public Class Form1
    ///     Inherits System.Windows.Forms.Form
    ///
    ///     Public Sub New()
    ///         '
    ///         ' Required for Windows Form Designer support
    ///         '
    ///         InitializeComponent()
    ///
    ///         ' Setup custom summaries
    ///         AddHandler Me.gridGroupingControl1.QueryCustomSummary, AddressOf gridGroupingControl1_QueryCustomSummary
    ///
    ///         Dim sd1 As New GridSummaryColumnDescriptor()
    ///         sd1.Name = "QuantityTotal"
    ///         sd1.DataMember = "Quantity"
    ///         sd1.DisplayColumn = "Quantity"
    ///         sd1.Format = "{Total}"
    ///         sd1.SummaryType = SummaryType.Custom
    ///         Me.gridGroupingControl1.TableDescriptor.SummaryRows.Add(New GridSummaryRowDescriptor("Row 1", "Total", sd1))
    ///
    ///     End Sub 'New
    ///
    ///     Private Sub gridGroupingControl1_QueryCustomSummary(ByVal sender As Object, ByVal e As GridQueryCustomSummaryEventArgs)
    ///         Select Case e.SummaryColumn.Name
    ///             Case "QuantityTotal"
    ///                 e.SummaryDescriptor.CreateSummaryMethod = New CreateSummaryDelegate(AddressOf TotalSummary.CreateSummaryMethod)
    ///                 Exit Select
    ///
    ///             Case "QuantityDistinctCount"
    ///                 e.SummaryDescriptor.CreateSummaryMethod = New CreateSummaryDelegate(AddressOf DistinctInt32CountSummary.CreateSummaryMethod)
    ///                 Exit Select
    ///
    ///             Case "QuantityMedian"
    ///                 e.SummaryDescriptor.CreateSummaryMethod = New CreateSummaryDelegate(AddressOf StatisticsSummary.CreateSummaryMethod)
    ///                 Exit Select
    ///         End Select
    ///     End Sub 'gridGroupingControl1_QueryCustomSummary
    ///
    ///
    /// NotInheritable Public Class TotalSummary
    ///     Inherits SummaryBase
    ///     Private _total As Double
    ///
    ///     Public Shared Empty As New TotalSummary(0)
    ///
    ///
    ///     Public Shared Function CreateSummaryMethod(ByVal sd As SummaryDescriptor, ByVal record As Record) As ITreeTableSummary
    ///         Dim obj As Object = sd.GetValue(record)
    ///         Dim isNull As Boolean = obj Is Nothing OrElse TypeOf obj Is DBNull
    ///         If isNull Then
    ///             Return Empty
    ///         Else
    ///             Dim val As Double = Convert.ToDouble(obj)
    ///             Return New TotalSummary(val)
    ///         End If
    ///     End Function 'CreateSummaryMethod
    ///
    ///
    ///     Public Sub New(ByVal total As Double)
    ///         _total = total
    ///     End Sub 'New
    ///
    ///     Public ReadOnly Property Total() As Double
    ///         Get
    ///             Return _total
    ///         End Get
    ///     End Property
    ///
    ///     Public Overloads Overrides Function Combine(ByVal other As SummaryBase) As SummaryBase
    ///         Return Combine(CType(other, TotalSummary))
    ///     End Function 'Combine
    ///
    ///     Public Overloads Function Combine(ByVal other As TotalSummary) As TotalSummary
    ///         ' Summary objects are immutable. That means properties cannot be modified for an
    ///         ' existing object. Instead every time a change is made a new object must be created (just like
    ///         ' System.String).
    ///         '
    ///         ' This allows following optimization: return existing summary object if either one of the values is 0. --
    ///         If other.Total = 0 Then
    ///             Return Me
    ///         ElseIf Total = 0 Then
    ///             Return other
    ///             ' -- end of optimization
    ///         Else
    ///             Return New TotalSummary(Me.Total + other.Total)
    ///         End If
    ///     End Function 'Combine
    ///
    ///     Public Overrides Function ToString() As String
    ///         Return String.Format("Total = {0:0.00}", Total)
    ///     End Function 'ToString
    /// End Class 'TotalSummary
    ///
    /// </code>
    /// </example>
    public sealed class GridQueryCustomSummaryEventArgs : SyncfusionHandledEventArgs
    {
        GridSummaryColumnDescriptor summaryColumn;
        SummaryDescriptor sd;

        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="summaryColumn">The summary column descriptor.</param>
        /// <param name="sd">The summary descriptor.</param>
        public GridQueryCustomSummaryEventArgs(GridSummaryColumnDescriptor summaryColumn, SummaryDescriptor sd)
        {
            this.summaryColumn = summaryColumn;
            this.sd = sd;
        }

        /// <summary>
        /// Gets a reference to the summary column descriptor.
        /// </summary>
        [TraceProperty(true)]
        public GridSummaryColumnDescriptor SummaryColumn
        {
            get
            {
                return summaryColumn;
            }
        }

        /// <summary>
        /// Gets a reference to the summary descriptor.
        /// </summary>
        [TraceProperty(true)]
        public SummaryDescriptor SummaryDescriptor
        {
            get
            {
                return sd;
            }
        }
    }
}
