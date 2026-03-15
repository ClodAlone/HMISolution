//-------------------------------------------------------------------------------------------------
// <copyright file="RecordFilterDescriptor.cs" company="syncfusion">
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
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.Serialization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping.Internals;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
using System.Diagnostics;

namespace Syncfusion.Grouping
{
    /// <summary>
    ///     Logical operator used by <see cref="RecordFilterDescriptor"/>.
    /// </summary>
    public enum FilterLogicalOperator
    {
        /// <summary>
        /// All conditions must be True.
        /// </summary>
        And,

        /// <summary>
        /// One of the conditions must be True.
        /// </summary>
        Or
    }

    /// <summary>
    /// Comparison operator used by <see cref="FilterCondition"/>.
    /// </summary>
    public enum FilterCompareOperator
    {
        /// <summary>
        /// The value is equal.
        /// </summary>
        Equals,

        /// <summary>
        /// The value is not equal.
        /// </summary>
        NotEquals,

        /// <summary>
        /// The left value is less than the right value.
        /// </summary>
        LessThan,

        /// <summary>
        /// The left value is less than or equal to the right value.
        /// </summary>
        LessThanOrEqualTo,

        /// <summary>
        /// The left value is greater than the right value.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// The left value is greater than or equal to the right value.
        /// </summary>
        GreaterThanOrEqualTo,

        /// <summary>
        /// The left string matches the right pattern with wildcard characters, character lists, or character ranges.
        /// </summary>
        /// <remarks>
        /// The pattern-matching of the like operator allows you to match strings using wildcard characters,
        /// character lists, or character ranges in any combination. The following table shows the characters
        /// allowed in pattern and what they match:
        /// <list type="table">
        /// <listheader><term>Characters in pattern
        /// </term><description>Matches in string
        /// </description></listheader>
        /// <item><term>?</term><description>Any single character
        /// </description></item>
        /// <item><term>*
        /// </term><description>Zero or more characters
        /// </description></item>
        /// <item><term>#
        /// </term><description>Any single digit (0�9)
        /// </description></item>
        /// <item><term>[charlist]
        /// </term><description>Any single character in charlist
        /// </description></item>
        /// <item><term>[!charlist]
        /// </term><description>Any single character not in charlist
        /// </description></item>
        /// </list>
        /// For further information and examples, see the "Like operator" in the MSDN help (Visual Basic Language Reference).
        /// </remarks>
        Like,

        /// <summary>
        /// The left string matches the right regular expression pattern. See ".NET Framework Regular Expressions" in MSDN Help
        /// for discussion and examples for regular expressions.
        /// </summary>
        Match,

        /// <summary>
        /// A custom filter. A implementation object of <see cref="ICustomFilter"/> should be applied to the 
        /// <see cref="FilterCondition.CustomFilter"/> property of the <see cref="FilterCondition"/>  object.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Provides a <see cref="CompareDescriptor"/> method when <see cref="FilterCompareOperator.Custom"/>
    /// is specified for a <see cref="FilterCondition"/>.
    /// </summary>
    public interface ICustomFilter
    {
        /// <summary>
        /// Called to determine if the record meets filter criteria of the specified condition.
        /// </summary>
        /// <param name="filterDescriptor">The condition.</param>
        /// <param name="record">The record to be tested.</param>
        /// <returns>True if record meets filter criteria; False otherwise.</returns>
        bool CompareDescriptor(FilterCondition filterDescriptor, Record record);
    }

    /// <summary>
    /// The type converter for <see cref="RecordFilterDescriptor"/> objects. <see cref="RecordFilterDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class RecordFilterDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <summary>Determines whether this object can be converted to the specified type, using the given format.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>True if this conversion is supported; False otherwise.</returns>
        /// <override/>
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

        /// <summary>Converts the given value to the specified type, using the given format and culture.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">The CultureInfo.</param>
        /// <param name="value">The Value.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        /// <override/>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
                && (value is RecordFilterDescriptor))
            {
                RecordFilterDescriptor rd = (RecordFilterDescriptor)value;
                Type type = value.GetType();

                if (!rd.ShouldSerializeName() && rd.Expression != string.Empty && rd.Conditions.Count == 0)
                {
                    return new InstanceDescriptor(
                        type.GetConstructor(new Type[] { typeof(string) }),
                        new object[] { rd.Expression },
                        true);
                }
                else if (rd.ShouldSerializeName() && rd.Conditions.Count == 0 && rd.Expression == string.Empty && !String.IsNullOrEmpty(rd.MappingName))
                {
                    return new InstanceDescriptor(
                        type.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(FilterLogicalOperator) }),
                        new object[] { rd.Name, rd.MappingName, rd.LogicalOperator },
                        true);
                }
                else if (rd.ShouldSerializeName() && rd.Conditions.Count > 0 && rd.Expression == string.Empty && !String.IsNullOrEmpty(rd.MappingName))
                {
                    FilterCondition[] fcs = new FilterCondition[rd.Conditions.Count];
                    rd.Conditions.CopyTo(fcs, 0);

                    return new InstanceDescriptor(
                        type.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(FilterLogicalOperator), typeof(FilterCondition[]) }),
                        new object[] { rd.Name, rd.MappingName, rd.LogicalOperator, fcs },
                        true);
                }
                else if (rd.ShouldSerializeName() && rd.Conditions.Count == 0 && rd.Expression == string.Empty)
                {
                    return new InstanceDescriptor(
                        type.GetConstructor(new Type[] { typeof(string), typeof(FilterLogicalOperator) }),
                        new object[] { rd.Name, rd.LogicalOperator },
                        true);
                }
                else if (rd.ShouldSerializeName() && rd.Conditions.Count > 0 && rd.Expression == string.Empty)
                {
                    FilterCondition[] fcs = new FilterCondition[rd.Conditions.Count];
                    rd.Conditions.CopyTo(fcs, 0);

                    return new InstanceDescriptor(
                  type.GetConstructor(new Type[] { typeof(string), typeof(FilterLogicalOperator), typeof(FilterCondition[]) }),
                        new object[] { rd.Name, rd.LogicalOperator, fcs },
                        true);
                }
                else
                {
                    return new InstanceDescriptor(
                        type.GetConstructor(new Type[] { }),
                        new object[] { },
                        false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo

        /// <summary>
        /// Returns a collection of properties for a given object type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Object type.</param>
        /// <param name="attributes">An array of objects of type System.Attribute that will be used as a filter.</param>
        /// <returns>Property descriptor collection.</returns>
        /// <override/>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "MappingName",
                "LogicalOperator",
                "Conditions"
            };

            return pds.Sort(atts);
        }
    }

    /// <summary>
    /// A RecordFilterDescriptor provides filter criteria for displaying only a
    /// subset of records from the underlying datasource. A filter can be specified through
    /// a collection of <see cref="FilterCondition"/> elements or with a formula expression
    /// similar to expressions used in <see cref="ExpressionFieldDescriptor"/>.
    /// <para/>
    /// RecordFilterDescriptors are managed by the <see cref="RecordFilterDescriptorCollection"/> that
    /// is returned by the <see cref="Syncfusion.Grouping.TableDescriptor.RecordFilters"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(RecordFilterDescriptorTypeConverter))]
    public class RecordFilterDescriptor : DescriptorBase, ICloneable, IStandardValuesProvider
    {
        string name = string.Empty;
        string mappingName = string.Empty;
        FieldDescriptor field = null;
        RecordFilterDescriptorCollection parentCollection;
        FilterConditionCollection conditions;
        FilterLogicalOperator logicalOperator = FilterLogicalOperator.Or;
        TableDescriptor tableDescriptor;
        string expression = string.Empty;
        object[] uniqueGroupId = null;
        string compiledExpression = null;
        WeakReference lastCompareRecord;
        bool lastCompareRecordResult;
        int lastCompareVersion = -1;
        static ArrayList internalArray;
        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <summary>
        /// Initializes a new empty filter.
        /// </summary>
        public RecordFilterDescriptor()
        {
            this.conditions = CreateFilterConditionCollection(null);
            WireConditions();
        }

        /// <summary>
        /// Initializes a new filter based on a formula expression similar to expressions used in <see cref="ExpressionFieldDescriptor"/>.
        /// </summary>
        /// <param name="expression">A formula expression similar to expressions used in <see cref="ExpressionFieldDescriptor"/>.</param>
        public RecordFilterDescriptor(string expression)
        {
            this.name = string.Empty;
            this.expression = expression;
            this.conditions = this.CreateFilterConditionCollection(null);
            WireConditions();
        }
        private ArrayList filterDisplay = null;
        /// <summary>
        /// Used internally.
        /// </summary>
        [Browsable(false)]
        public virtual ArrayList FilterDisplay
        {
            get
            {
                if (filterDisplay == null && internalArray != null)
                    return internalArray;
                return filterDisplay;
            }
            set
            {
                filterDisplay = value;
                if (value != null)
                    internalArray = value;
            }
        }
        /// <summary>
        /// Initializes a new filter based on a formula expression similar to expressions used in <see cref="ExpressionFieldDescriptor"/>.
        /// </summary>
        /// <param name="name">The name of the field this filter is compared with. This name is used to look up fields in the Fields collection of the parent table descriptor.</param>
        /// <param name="expression">A formula expression similar to expressions used in <see cref="ExpressionFieldDescriptor"/>.</param>
        public RecordFilterDescriptor(string name, string expression)
        {
            this.name = name;
            this.expression = expression;
            this.conditions = CreateFilterConditionCollection(null);
            WireConditions();
        }

        /// <summary>
        /// Creates the <see cref="FilterConditionCollection"/> list.
        /// </summary>
        /// <returns>returns a new instance of FilterConditionCollection.</returns>
        protected virtual FilterConditionCollection CreateFilterConditionCollection(FilterCondition[] conditions)
        {
            if (conditions != null)
            {
                return new FilterConditionCollection(conditions);
            }
            else
            {
                return new FilterConditionCollection();
            }
        }

        /// <summary>
        /// Initializes a new filter based on a collection of <see cref="FilterCondition"/>.
        /// </summary>
        /// <param name="name">The name of the field descriptor.</param>
        /// <param name="mappingName">The name of the field this filter is compared with. This name is used to look up fields in the Fields collection of the parent table descriptor.</param>
        /// <param name="logicalOperator">The logical operator used if multiple conditions are given.</param>
        /// <param name="conditions">The collection of conditions.</param>
        public RecordFilterDescriptor(string name, string mappingName, FilterLogicalOperator logicalOperator, FilterCondition[] conditions)
        {
            this.mappingName = mappingName;
            this.name = name;
            this.logicalOperator = logicalOperator;
            this.conditions = CreateFilterConditionCollection(conditions);
            WireConditions();
        }

        /// <summary>
        /// Initializes a new filter based on a collection of <see cref="FilterCondition"/>.
        /// </summary>
        /// <param name="name">The name of the field this filter is compared with. This name is used to look up fields in the Fields collection of the parent table descriptor.</param>
        /// <param name="logicalOperator">The logical operator used if multiple conditions are given.</param>
        /// <param name="conditions">The collection of conditions.</param>
        public RecordFilterDescriptor(string name, FilterLogicalOperator logicalOperator, FilterCondition[] conditions)
        {
            this.name = name;
            this.logicalOperator = logicalOperator;
            this.conditions = CreateFilterConditionCollection(conditions);
            WireConditions();
        }

        /// <summary>
        /// Initializes a new filter based on a collection of <see cref="FilterCondition"/>.
        /// </summary>
        /// <param name="name">The name of the field this filter is compared with. This name is used to look up fields in the Fields collection of the parent table descriptor.</param>
        /// <param name="condition">The condition.</param>
        public RecordFilterDescriptor(string name, FilterCondition condition)
            : this(name, FilterLogicalOperator.Or, new FilterCondition[] { condition })
        {
        }

        /// <summary>
        /// Initializes a new filter based on a collection of <see cref="FilterCondition"/>.
        /// </summary>
        /// <param name="name">The name of the field descriptor.</param>
        /// <param name="mappingName">The name of the field this filter is compared with. This name is used to look up fields in the Fields collection of the parent table descriptor.</param>
        /// <param name="condition">The condition.</param>
        public RecordFilterDescriptor(string name, string mappingName, FilterCondition condition)
            : this(name, mappingName, FilterLogicalOperator.Or, new FilterCondition[] { condition })
        {
        }

        void WireConditions()
        {
            if (conditions != null)
            {
                conditions.Changed += new ListPropertyChangedEventHandler(conditions_Changed);
                conditions.Changing += new ListPropertyChangedEventHandler(conditions_Changing);
            }
        }

        void UnwireConditions()
        {
            if (conditions != null)
            {
                conditions.Changed -= new ListPropertyChangedEventHandler(conditions_Changed);
                conditions.Changing -= new ListPropertyChangedEventHandler(conditions_Changing);
            }
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                field = null;
                parentCollection = null;
                tableDescriptor = null;
                UnwireConditions();
                conditions.Dispose();
                conditions = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(RecordFilterDescriptor other)
        {
            this.Name = other.name;
            this.Conditions.InitializeFrom(other.conditions);
            this.LogicalOperator = other.logicalOperator;
            this.Expression = other.expression;
            this.UniqueGroupId = other.UniqueGroupId;
            this.MappingName = other.MappingName;
        }

        ICollection IStandardValuesProvider.GetStandardValues(PropertyDescriptor pd)
        {
            ArrayList al = new ArrayList();
            SortedList sl = new SortedList();
            foreach (FieldDescriptor ppd in tableDescriptor.Fields)
            {
                if (this.FilterDisplay != null)
                {
                    if (!FilterDisplay.Contains(ppd.Name))
                        al.Add(ppd.Name);
                }
                else
                    al.Add(ppd.Name);
            }

            return al;
        }

        internal void SetCollection(RecordFilterDescriptorCollection parentCollection)
        {
            this.parentCollection = parentCollection;
            if (parentCollection.tableDescriptor != null)
            {
                this.tableDescriptor = parentCollection.tableDescriptor;
            }

            foreach (FilterCondition condition in this.Conditions)
            {
                condition.SetCollection(Conditions);
                condition.SetFilterDescriptor(this);
            }
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RecordFilterDescriptorCollection Collection
        {
            get
            {
                return parentCollection;
            }
        }

        internal void SetTableDescriptor(TableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
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

            this.lastCompareRecord = null;
            this.fieldsVersion = -1;
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

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a copy of this descriptor and all its conditions.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public virtual RecordFilterDescriptor Clone()
        {
            RecordFilterDescriptor rd = new RecordFilterDescriptor();
            this.CopyAllMembersTo(rd);
            return rd;
        }

        protected void CopyAllMembersTo(RecordFilterDescriptor rd)
        {
            if (this.conditions != null)
            {
                rd.UnwireConditions();
                rd.conditions = this.conditions.Clone();
                rd.WireConditions();
                foreach (FilterCondition cd in rd.Conditions)
                {
                    cd.SetFilterDescriptor(this);
                }
            }

            rd.name = name;
            rd.logicalOperator = logicalOperator;
            rd.tableDescriptor = tableDescriptor;
            rd.expression = expression;
            rd.uniqueGroupId = uniqueGroupId;
            rd.mappingName = mappingName;
        }

        /// <summary>
        /// Determines whether the specified descriptor object is equal to the
        /// current descriptor.
        /// </summary>
        /// <param name="obj">The descriptor object to compare. </param>
        /// <returns>
        /// true if the specified descriptor is equal to the current
        /// descriptor; otherwise, false.
        /// </returns>
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
            else if (!(obj is RecordFilterDescriptor))
            {
                return false;
            }

            return Equals((RecordFilterDescriptor)obj);
        }

        bool Equals(RecordFilterDescriptor other)
        {
            return other.name == name
                && other.mappingName == mappingName
                && other.field == field
                && other.conditions == conditions
                && other.logicalOperator == logicalOperator
                && other.expression == expression
                && other.uniqueGroupId == uniqueGroupId;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// The collection of filter conditions.
        /// </summary>
        [Category("Conditions Filter")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        public FilterConditionCollection Conditions
        {
            get
            {
                return conditions;
            }
        }

        /// <summary>
        /// Determines if filter conditions were added.
        /// </summary>
        /// <returns>True if filter conditions were added; False otherwise.</returns>
        public bool ShouldSerializeConditions()
        {
            return conditions.Count > 0;
        }

        /// <summary>
        /// The logical operator used if multiple conditions are given.
        /// </summary>
        [Category("Conditions Filter")]
        [DefaultValue(FilterLogicalOperator.Or)]
        [Description("The logical operator used if multiple conditions are given.")]
        public FilterLogicalOperator LogicalOperator
        {
            get
            {
                return logicalOperator;
            }

            set
            {
                if (logicalOperator != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    logicalOperator = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                }
            }
        }

        /// <summary>Gets the descriptor name.</summary>
        /// <returns>Descriptor name.</returns>
        /// <override/>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// The name of this filter descriptor or the name of the field this filter is compared with.
        /// </summary>
        [Category("Conditions Filter")]
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of the field this filter is compared with.")]
        public virtual string Name
        {
            get
            {
                if (name == string.Empty)
                {
                    if (expression != string.Empty)
                    {
                        return expression;
                    }

                    if (mappingName != string.Empty)
                    {
                        return mappingName;
                    }
                }

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
        /// Determines if name is not empty.
        /// </summary>
        /// <returns>True if name is not empty; False otherwise.</returns>
        public bool ShouldSerializeName()
        {
            return name != string.Empty;
        }

        /// <summary>
        /// Resets the name to be empty.
        /// </summary>
        public void ResetName()
        {
            name = string.Empty;
        }

        /// <summary>
        /// The name of the field this filter is compared with. This name is used to look up fields in the Fields collection of the parent table descriptor.
        /// </summary>
        [Category("Conditions Filter")]
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of the field this filter is compared with.")]
        public virtual string MappingName
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
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("MappingName"));
                }
            }
        }

        /// <summary>
        /// Determines if name is not empty.
        /// </summary>
        /// <returns>True if the name is not empty.</returns>
        public bool ShouldSerializeMappingName()
        {
            return mappingName != string.Empty;
        }

        /// <summary>
        /// Resets the name to be empty.
        /// </summary>
        public void ResetMappingName()
        {
            mappingName = string.Empty;
        }

        /// <summary>
        /// A <see cref="Group.UniqueGroupId"/> category the filter belongs to. This property is used by FilterBar 
        /// to identify the conditions that only belong to a certain uniqueGroupId. All RecordFilter objects that have
        /// the same UniqueGroupId will be combined with LogicalOperator.And.
        /// </summary>
        [Category("Conditions Filter")]
        [Description("A unique uniqueGroupId id the filter is applied to.")]
        public object[] UniqueGroupId
        {
            get
            {
                return this.uniqueGroupId;
            }

            set
            {
                if (this.uniqueGroupId != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("UniqueGroupId"));
                    this.uniqueGroupId = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("UniqueGroupId"));
                }
            }
        }

        /// <summary>
        /// Resets the <see cref="UniqueGroupId"/> to null.
        /// </summary>
        public void ResetUniqueGroupId()
        {
            this.UniqueGroupId = null;
        }

        /// <summary>
        /// Determines if <see cref="UniqueGroupId"/> should be serialized to code or xml.
        /// </summary>
        /// <returns>True if <see cref="UniqueGroupId"/> should be serialized; False otherwise.</returns>
        public bool ShouldSerializeUniqueGroupId()
        {
            if (uniqueGroupId == null)
            {
                return false;
            }

            // No support for serializing conditions for UniformChildListRelations.
            // Avoid exception instead.
            for (int n = 0; n < uniqueGroupId.Length; n++)
            {
                if (uniqueGroupId[n] is Record)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// A formula expression similar to expressions used in <see cref="ExpressionFieldDescriptor"/>.
        /// </summary>
        [Category("Expression Filter")]
        [DefaultValue("")]
        [Description("A formula expression.")]
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
                    if (eval != null)
                    {
                        string s = eval.PutTokensInFormula(expression.ToLower());
                        compiledExpression = eval.Parse(s);
                        this.fieldsVersion = tableDescriptorfieldsVersion;
                    }
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
        /// Gets the field descriptor this record filter is applied to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FieldDescriptor FieldDescriptor
        {
            get
            {
                if (parentCollection != null)
                {
                    this.parentCollection.EnsureFieldDescriptors();
                }

                return field;
            }
        }

        internal bool InitFieldDescriptor(TableDescriptor tableDescriptor)
        {
            if (this.mappingName != string.Empty)
            {
                field = tableDescriptor.Fields[this.mappingName];
            }
            else
            {
                this.mappingName = this.name;
                field = tableDescriptor.Fields[this.Name];
            }

            foreach (FilterCondition condition in conditions)
            {
                condition.SetFilterDescriptor(this);
            }

            return field != null;
        }

        private void conditions_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            ////            foreach (FilterCondition condition in conditions)
            ////                if (condition.FilterDescriptor != this)
            ////                    return;
            OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Conditions", e));
            foreach (FilterCondition condition in conditions)
            {
                condition.SetFilterDescriptor(this);
            }

            lastCompareRecord = null;
        }

        private void conditions_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Conditions", e));
        }

        /// <summary>
        /// Evaluates this condition for the given record and returns True if the record
        /// meets the condition.
        /// </summary>
        /// <param name="record">The record to be evaluated.</param>
        /// <param name="filterBarFieldDescriptor">If the underlying FilterDescriptor of this condition
        /// is the same as filterBarFieldDescriptor, this method will return True.</param>
        /// <returns>True if the record
        /// meets the condition; False otherwise.</returns>
        public bool CompareRecordFilterBar(Record record, FieldDescriptor filterBarFieldDescriptor)
        {
            bool result = false;

            if (Expression != string.Empty
                && filterBarFieldDescriptor != null
                && (MappingName == filterBarFieldDescriptor.Name || Name == filterBarFieldDescriptor.Name)
                && Conditions.Count == 0)
            {
                return true;
            }

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

                if (Conditions.Count == 0)
                {
                    return result;
                }
            }

            if (Conditions.Count == 0)
            {
                return true;
            }

            // Apply filter only to the specific group that it was selected for.
            if (this.UniqueGroupId != null)
            {
                bool groupMatch = false;
                Group g = record.ParentGroup;
                while (g != null && !g.IsTopLevelGroup)
                {
                    if (EqualsCategory(this.UniqueGroupId, g.UniqueGroupId))
                    {
                        groupMatch = true;
                        break;
                    }

                    g = g.ParentGroup;
                }

                if (!groupMatch)
                {
                    return true;
                }
            }

            if (this.LogicalOperator == FilterLogicalOperator.And)
            {
                foreach (FilterCondition condition in Conditions)
                {
                    condition.SetFilterDescriptor(this);
                    if (!condition.CompareRecordFilterBar(record, filterBarFieldDescriptor))
                    {
                        return false;
                    }
                }

                return true;
            }
            else 
            {
                //// FilterLogicalOperator.Or
                foreach (FilterCondition condition in Conditions)
                {
                    condition.SetFilterDescriptor(this);
                    if (condition.CompareRecordFilterBar(record, filterBarFieldDescriptor))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Evaluates this filter for the given record and returns True if the record
        /// meets the filters criteria.
        /// </summary>
        /// <param name="record">The record to be evaluated.</param>
        /// <returns>True if the record
        /// meets the filters criteria; False otherwise.</returns>
        public bool CompareRecord(Record record)
        {
            // caching for FilterBarChoice condition
            if (lastCompareRecord != null &&
                lastCompareVersion == record.Engine.SourceListVersion &&
                Object.ReferenceEquals(lastCompareRecord.Target, record))
            {
                ////Console.WriteLine("CompareRecord: " + lastCompareRecordResult.ToString());
                return lastCompareRecordResult;
            }

            lastCompareRecord = new WeakReference(record);
            lastCompareRecordResult = _CompareRecord(record);
            lastCompareVersion = record.Engine.SourceListVersion;

            return lastCompareRecordResult;
        }

        /// <summary>For internal use.</summary>
        /// <exclude/>
        public void ResetCache()
        {
            lastCompareRecord = null;
        }

        bool _CompareRecord(Record record)
        {
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

                if (Conditions.Count == 0)
                {
                    return result;
                }
            }

            if (Conditions.Count == 0)
            {
                return true;
            }

            // Apply filter only to the specific group that it was selected for.
            if (this.UniqueGroupId != null)
            {
                bool groupMatch = false;
                Group g = record.ParentGroup;
                while (g != null && !g.IsTopLevelGroup)
                {
                    if (EqualsCategory(this.UniqueGroupId, g.UniqueGroupId))
                    {
                        groupMatch = true;
                        break;
                    }

                    g = g.ParentGroup;
                }

                if (!groupMatch)
                {
                    return true;
                }
            }

            if (this.LogicalOperator == FilterLogicalOperator.And)
            {
                foreach (FilterCondition condition in Conditions)
                {
                    condition.SetFilterDescriptor(this);
                    if (!condition.CompareRecord(record))
                    {
                        return false;
                    }
                }

                return true;
            }
            else 
            {
                //// FilterLogicalOperator.Or
                foreach (FilterCondition condition in Conditions)
                {
                    condition.SetFilterDescriptor(this);
                    if (condition.CompareRecord(record))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Compares the specified Group.UniqueGroupId with this record filters RecordFilterDescriptor.UniqueGroupId.
        /// The method is used by FilterBarCells to find the filter criteria that matches the child group that a filter
        /// bar belongs to.
        /// </summary>
        /// <param name="uniqueId">A Group.UniqueGroupID to compare.</param>
        /// <returns>True if both UniqueGroupID are equal.; False otherwise.</returns>
        public bool CompareUniqueId(object[] uniqueId)
        {
            return EqualsCategory(UniqueGroupId, uniqueId);
        }

        bool EqualsCategory(object[] x, object[] y)
        {
            bool xIsNull = x == null || x.Length == 0;
            bool yIsNull = y == null || y.Length == 0;

            if (yIsNull && xIsNull)
            {
                return true;
            }
            else if (xIsNull || yIsNull || x.Length != y.Length)
            {
                return false;
            }

            bool equalsCategory = true;
            for (int n = 0; n < x.Length; n++)
            {
                equalsCategory &= 0 == _Compare(x[n], y[n]);
            }

            return equalsCategory;
        }

        internal static int _Compare(object x, object y)
        {
            int cmp = 0;
            bool xIsNull = x == null || x is DBNull;
            bool yIsNull = y == null || y is DBNull;

            if (yIsNull && xIsNull)
            {
                cmp = 0;
            }
            else if (xIsNull)
            {
                cmp = -1;
            }
            else if (yIsNull)
            {
                cmp = 1;
            }
            else if (Object.ReferenceEquals(x, y))
            {
                cmp = 0;
            }
            else if (x.GetType() != y.GetType())
            {
                cmp = -1;
            }
            else if (x is IComparable)
            {
                cmp = ((IComparable)x).CompareTo(y);
            }

            return cmp;
        }

        protected override PropertyDescriptorCollection GetCustomPDC(PropertyDescriptorCollection baseprops)
        {
            PropertyDescriptorCollection pds = base.GetCustomPDC(baseprops);
            ArrayList newpds = new ArrayList();
            foreach (PropertyDescriptor pd in pds)
            {
                if (pd.Name == "Conditions")
                {
                    System.Attribute[] atts = new System.Attribute[pd.Attributes.Count + 1];
                    pd.Attributes.CopyTo(atts, 0);
                    // Add the PersistenceMode.InnerProperty attribute via reflection.
                    atts[atts.Length - 1] = DesignTimeUtils.GetPersistenceModeAttribute("InnerProperty");
                    PropertyDescriptor newPD = new AttributesAddingPropertyDescriptor(pd, atts);
                    newpds.Add(newPD);
                }
                else
                {
                    newpds.Add(pd);
                }
            }

            PropertyDescriptorCollection newPDC = new PropertyDescriptorCollection((PropertyDescriptor[])newpds.ToArray(typeof(PropertyDescriptor)));
            return newPDC;
        }
    }

    /// <summary>
    /// A collection of <see cref="RecordFilterDescriptor"/> with filter criteria for displaying only a
    /// subset of records from the underlying datasource.
    /// An instance of this collection is returned by the <see cref="Syncfusion.Grouping.TableDescriptor.RecordFilters"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [ListBindableAttribute(false)]
    public class RecordFilterDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        ArrayList inner = new ArrayList();
        internal int version;
        FilterLogicalOperator logicalOperator;
        internal TableDescriptor tableDescriptor;

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

        /// <summary>
        /// The TableDescriptor that this collection belongs to.
        /// </summary>
        public TableDescriptor TableDescriptor
        {
            get
            {
                return this.tableDescriptor;
            }

            set
            {
                this.tableDescriptor = value;
                foreach (RecordFilterDescriptor cd in this.inner)
                {
                    cd.SetTableDescriptor(value);
                    cd.SetCollection(this);
                }
            }
        }

        internal bool insideCollectionEditor = false;
        int fieldsVersion = -1;

        internal void EnsureFieldDescriptors()
        {
            if (inner.Count > 0 && tableDescriptor != null && fieldsVersion != this.tableDescriptor.Fields.Version)
            {
                foreach (RecordFilterDescriptor cd in this.inner)
                {
                    cd.InitFieldDescriptor(tableDescriptor);
                }

                fieldsVersion = this.tableDescriptor.Fields.Version;
            }
        }

        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return String.Format("RecordFilterDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
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
            InitializeFrom((RecordFilterDescriptorCollection)other);
        }

        bool inInitializeFrom = false;
        bool inInitializeFromChanged = false;

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(RecordFilterDescriptorCollection other)
        {
            inInitializeFrom = true;
            inInitializeFromChanged = false;

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

            inInitializeFrom = false;
            if (inInitializeFromChanged)
            {
                this.OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }
        }

        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public RecordFilterDescriptorCollection()
        {
        }

        /// <summary>
        /// Initializes a new empty collection and attaches it to a <see cref="TableDescriptor"/>.
        /// </summary>
        internal RecordFilterDescriptorCollection(TableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public RecordFilterDescriptorCollection Clone()
        {
            RecordFilterDescriptorCollection coll = new RecordFilterDescriptorCollection();
            coll.inner = new ArrayList();
            coll.logicalOperator = logicalOperator;
            coll.version = version + 1000;
            coll.tableDescriptor = tableDescriptor;
            coll.fieldsVersion = -1;
            int count = Count;
            RecordFilterDescriptor[] filterDescriptors = new RecordFilterDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                coll.inner.Add(this[n].Clone());
                coll[n].SetCollection(coll);
            }

            return coll;
        }

        /// <summary>Determines if the specified object is equivalent to current object.</summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if both the objects are equal; False otherwise.</returns>
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
            else if (!(obj is RecordFilterDescriptorCollection))
            {
                return false;
            }

            return Equals((RecordFilterDescriptorCollection)obj);
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code for the current object.</returns>
        /// <override/>
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

        bool Equals(RecordFilterDescriptorCollection other)
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
        public RecordFilterDescriptor this[int index]
        {
            get
            {
                return (RecordFilterDescriptor)inner[index];
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
        ///  Gets the collection of RecordFilterDescriptor with the specified mapping name.
        /// </summary>
        /// <param name="mappingName">The MappingName of the elements to get.</param>
        /// <returns>Array of RecordFilterDescriptor with the specified mapping name.</returns>
        public RecordFilterDescriptor[] GetRecordFilters(string mappingName)
        {
            ArrayList filterDescriptors = new ArrayList();
            for (int n = 0; n < Count; n++)
            {
                if (this[n].MappingName == mappingName)
                {
                    filterDescriptors.Add(this[n]);
                }
            }

            if (filterDescriptors.Count > 0)
            {
                return (RecordFilterDescriptor[])filterDescriptors.ToArray(typeof(RecordFilterDescriptor));
            }

            return null;
        }

        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
        public RecordFilterDescriptor this[string name]
        {
            get
            {
                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (RecordFilterDescriptor)inner[index];
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
        public bool Contains(RecordFilterDescriptor value)
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
        public int IndexOf(RecordFilterDescriptor value)
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
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(RecordFilterDescriptor[] array, int index)
        {
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        RecordFilterDescriptorCollection SyncRoot
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
        /// <remarks>Enumerators only allow reading the data in the collection.
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public RecordFilterDescriptorCollectionEnumerator GetEnumerator()
        {
            return new RecordFilterDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, RecordFilterDescriptor value)
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
        public void Remove(RecordFilterDescriptor value)
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
        public int Add(RecordFilterDescriptor value)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            int index = inner.Add(value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Creates a RecordFilterDescriptor based on the specified expression and adds it to the end of the collection.
        /// </summary>
        /// <param name="expression">The filter expression. See the Grid User's Guide for valid expressions.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string expression)
        {
            RecordFilterDescriptor rd = new RecordFilterDescriptor(expression);
            return Add(rd);
        }

        /// <summary>
        /// Creates a RecordFilterDescriptor and adds it to the end of the collection.
        /// </summary>
        /// <param name="name">The name of the filter.</param>
        /// <param name="compareOperator">The comparison operator.</param>
        /// <param name="compareValue">The comparison value.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name, FilterCompareOperator compareOperator, object compareValue)
        {
            RecordFilterDescriptor rd = new RecordFilterDescriptor(name, new FilterCondition(compareOperator, compareValue));
            return Add(rd);
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
        /// Disposes the object and collection items.
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
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            version++;
            this.fieldsVersion = -1;
            if (!this.InsideCollectionEditor)
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

        internal void RaisePropertyItemChanged(RecordFilterDescriptor filterDescriptor, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, filterDescriptor.Name, e.PropertyName);
                }
#else
                ;
#endif
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, IndexOf(filterDescriptor), filterDescriptor, e.PropertyName, e));
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
                if (Changing != null)
                {
                    Changing(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanging(RecordFilterDescriptor filterDescriptor, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, IndexOf(filterDescriptor), filterDescriptor, e.PropertyName, e));
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
                this[index] = (RecordFilterDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (RecordFilterDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((RecordFilterDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((RecordFilterDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((RecordFilterDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((RecordFilterDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((RecordFilterDescriptor[])array, index);
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
        /// Evaluates all filters for the given record and returns True if the record
        /// meets the filter's criteria.
        /// </summary>
        /// <param name="record">The record to be evaluated.</param>
        /// <returns>True if the record
        /// meets the filter's criteria; False otherwise.</returns>
        public bool CompareRecord(Record record)
        {
            if (record == null)
            {
                return false;
            }

            if (this.Count == 0)
            {
                return true;
            }

            if (this.LogicalOperator == FilterLogicalOperator.And)
            {
                foreach (RecordFilterDescriptor recordFilter in this)
                {
                    if (!recordFilter.CompareRecord(record))
                    {
                        return false;
                    }
                }

                return true;
            }
            else 
            {
                //// FilterLogicalOperator.Or
                foreach (RecordFilterDescriptor recordFilter in this)
                {
                    if (recordFilter.CompareRecord(record))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>For internal use.</summary>
        /// <exclude/>
        public void ResetCache()
        {
            foreach (RecordFilterDescriptor recordFilter in this)
            {
                recordFilter.ResetCache();
            }
        }

        /// <summary>
        /// Evaluates all filters for the given record and returns True if the record
        /// meets the filter's criteria.
        /// </summary>
        /// <param name="record">The record to be evaluated.</param>
        /// <param name="filterBarFieldDescriptor">The FieldDescriptor of the Field in a FilterBar</param>
        /// <returns>True if the record
        /// meets the filter's criteria; False otherwise.</returns>
        public bool CompareRecordFilterBar(Record record, FieldDescriptor filterBarFieldDescriptor)
        {
            if (record == null)
            {
                return false;
            }

            if (this.Count == 0)
            {
                return true;
            }

            if (this.LogicalOperator == FilterLogicalOperator.And)
            {
                foreach (RecordFilterDescriptor recordFilter in this)
                {
                    if (!recordFilter.CompareRecordFilterBar(record, filterBarFieldDescriptor))
                    {
                        return false;
                    }
                }

                return true;
            }
            else 
            {
                //// FilterLogicalOperator.Or
                foreach (RecordFilterDescriptor recordFilter in this)
                {
                    if (recordFilter.CompareRecordFilterBar(record, filterBarFieldDescriptor))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets / sets the logical operator.
        /// </summary>
        [Description("Gets / Sets the logical operator.")]
        public FilterLogicalOperator LogicalOperator
        {
            get
            {
                return logicalOperator;
            }

            set
            {
                if (logicalOperator != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("LogicalOperator"));
                    logicalOperator = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("LogicalOperator"));
                }
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
    /// Enumerator class for <see cref="RecordFilterDescriptor"/> elements of a <see cref="RecordFilterDescriptorCollection"/>.
    /// </summary>
    public class RecordFilterDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        RecordFilterDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="parentCollection">The parent collection to enumerate.</param>
        public RecordFilterDescriptorCollectionEnumerator(RecordFilterDescriptorCollection parentCollection)
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
        public RecordFilterDescriptor Current
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

    /// <summary>
    /// The type converter for <see cref="FilterCondition"/> objects. <see cref="FilterConditionTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class FilterConditionTypeConverter : DescriptorBaseConverter
    {
        /// <summary>Determines whether this object can be converted to the specified type, using the given format.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>True if this conversion is supported; False otherwise.</returns>
        /// <override/>
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
        /// <summary>Converts the given value to the specified type, using the given format and culture.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">The CultureInfo.</param>
        /// <param name="value">The Value.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>Converted object.</returns>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
                && (value is FilterCondition))
            {
                FilterCondition typename = (FilterCondition)value;
                System.Type[] args;
                args = new System.Type[2];
                args[0] = typeof(FilterCompareOperator);
                args[1] = typeof(object);

                System.Reflection.ConstructorInfo constructorInfo;
                constructorInfo = typeof(FilterCondition).GetConstructor(args);
                if (constructorInfo != null)
                {
                    object[] argValues;
                    argValues = (object[])new System.Object[2];
                    argValues[0] = typename.CompareOperator;
                    argValues[1] = typename.CompareText;
                    return (object)new InstanceDescriptor(constructorInfo, argValues);
                }
            }
            else if (destinationType == typeof(string) && (value is FilterCondition))
            {
                FilterCondition fc = (FilterCondition)value;
                return String.Format("{0} {1}", fc.CompareOperator, fc.CompareText);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo

        /// <summary>
        /// Returns the collection of properties for a specified component type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">The Value specifying type.</param>
        /// <param name="attributes">Array of type System.Attribute that will be used as a filter.</param>
        /// <returns>Property descriptor collection.</returns>
        /// <override/>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "CompareOperator",
                "CompareValue",
            };

            return pds.Sort(atts);
        }
    }

    /// <summary>
    /// A <see cref="FilterCondition"/> is a child of a <see cref="RecordFilterDescriptor"/>
    /// with filter criteria for displaying only a subset of records from the underlying datasource.
    /// <see cref="FilterCondition"/> elements are stored in a collection that is returned by the
    /// <see cref="Syncfusion.Grouping.RecordFilterDescriptor.Conditions"/> property of a <see cref="RecordFilterDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(FilterConditionTypeConverter))]
    public class FilterCondition : DescriptorBase, ICloneable
    {
        FilterConditionCollection parentCollection;
        object compareValue = string.Empty;
        object preparedCompareValue = dirty;
        FilterCompareOperator compareOperator = FilterCompareOperator.Equals;
        ICustomFilter customFilter;
        Regex regexValue = null;
        RecordFilterDescriptor _filterDescriptor;
        FieldDescriptor _pd;
        static object dirty = new object();

        WeakReference lastCompareRecord;
        bool lastCompareRecordResult;
        int lastCompareVersion = -1;

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <summary>
        /// Initializes a new empty condition.
        /// </summary>
        public FilterCondition()
        {
        }

        /// <summary>
        /// Initializes a new condition with comparison operator and comparison value.
        /// </summary>
        /// <param name="compareOperator">The comparison operator.</param>
        /// <param name="compareValue">The comparison value.</param>
        public FilterCondition(FilterCompareOperator compareOperator, object compareValue)
        {
            this.compareOperator = compareOperator;
            this.compareValue = compareValue;
        }

        /// <summary>
        /// Determines the name of this object.
        /// </summary>
        /// <returns>Object name.</returns>
        /// <override/>
        public override string GetName()
        {
            return parentCollection != null ? parentCollection.IndexOf(this).ToString() : string.Empty;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                compareValue = "Disposed";
                parentCollection = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(FilterCondition other)
        {
            this.CompareOperator = other.CompareOperator;
            this.CompareValue = other.CompareValue;
            this.CustomFilter = other.CustomFilter;
        }

        internal void SetCollection(FilterConditionCollection parentCollection)
        {
            this.parentCollection = parentCollection;
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FilterConditionCollection Collection
        {
            get
            {
                return parentCollection;
            }
        }

        /// <summary>
        /// The RecordFilterDescriptor this condition belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RecordFilterDescriptor FilterDescriptor
        {
            get
            {
                return _filterDescriptor;
            }
        }

        internal void SetFilterDescriptor(RecordFilterDescriptor filterDescriptor)
        {
            this._filterDescriptor = filterDescriptor;
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

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns>A copy of this object.</returns>
        public virtual FilterCondition Clone()
        {
            FilterCondition condition = new FilterCondition();
            this.CopyAllMembersTo(condition);
            return condition;
        }

        protected void CopyAllMembersTo(FilterCondition condition)
        {
            condition.compareOperator = this.compareOperator;
            condition.compareValue = this.compareValue;
            condition.customFilter = this.customFilter;
            condition._filterDescriptor = this._filterDescriptor;
            condition.parentCollection = this.parentCollection;
            condition._pd = this._pd;
            condition.preparedCompareValue = this.preparedCompareValue;
            condition.regexValue = this.regexValue;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the
        /// current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// True if the specified object is equal to the current
        /// object; otherwise, false.
        /// </returns>
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
            else if (!(obj is FilterCondition))
            {
                return false;
            }

            return Equals((FilterCondition)obj);
        }

        bool Equals(FilterCondition other)
        {
            return Object.ReferenceEquals(other._filterDescriptor, _filterDescriptor)
                && other.compareOperator == compareOperator
                && other.compareValue == compareValue
                && other.customFilter == customFilter;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code.
        /// </returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// The comparison operator.
        /// </summary>
        public FilterCompareOperator CompareOperator
        {
            get
            {
                return this.compareOperator;
            }

            set
            {
                if (this.compareOperator != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("CompareOperator"));
                    this.compareOperator = value;
                    this.preparedCompareValue = dirty;
                    regexValue = null;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("CompareOperator"));
                }
            }
        }

        /// <summary>
        /// The comparison text.
        /// </summary>
        public string CompareText
        {
            get
            {
                if (CompareValue == null || CompareValue is DBNull)
                {
                    return "(null)";
                }

                return CompareValue.ToString();
            }

            set
            {
                if (value == "(null)")
                {
                    CompareValue = null;
                }
                else
                {
                    CompareValue = value;
                }
            }
        }

        /// <summary>
        /// The comparison value.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object CompareValue
        {
            get
            {
                return this.compareValue;
            }

            set
            {
                if (this.compareValue != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("CompareValue"));
                    this.compareValue = value;
                    this.preparedCompareValue = dirty;
                    regexValue = null;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("CompareValue"));
                }
            }
        }

        /// <summary>
        /// An <see cref="ICustomFilter"/> if FilterCompareOperator.Custom was specified as <see cref="CompareOperator"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ICustomFilter CustomFilter
        {
            get
            {
                return customFilter;
            }

            set
            {
                if (this.customFilter != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("CustomFilter"));
                    this.customFilter = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("CustomFilter"));
                }
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        FieldDescriptor FieldDescriptor
        {
            get
            {
                if (_filterDescriptor == null)
                {
                    return null;
                }

                if (this._pd != _filterDescriptor.FieldDescriptor)
                {
                    this._pd = _filterDescriptor.FieldDescriptor;
                    this.preparedCompareValue = dirty;
                    this.regexValue = null;
                }

                return _pd;
            }
        }

        object GetValue(Record record)
        {
            if (record == null)
            {
                return null;
            }
            else if (FieldDescriptor == null)
            {
                return record.GetData();
            }
            else
            {
                return record.GetValue(FieldDescriptor);
            }
        }

        bool IsNull(object x)
        {
            return x == null || x is DBNull;
        }

        static int Compare(object x, object y)
        {
            return SortColumnComparer._Compare(x, y);
        }

        /// <summary>
        /// Evaluates this condition for the given record and returns True if the record
        /// meets the condition.
        /// </summary>
        /// <param name="record">The record to be evaluated.</param>
        /// <param name="filterBarFieldDescriptor">If the underlying FilterDescriptor of this condition
        /// is the same as filterBarFieldDescriptor, this method will return True.</param>
        /// <returns>True if the record
        /// meets the condition; False otherwise.</returns>
        public bool CompareRecordFilterBar(Record record, FieldDescriptor filterBarFieldDescriptor)
        {
            if (filterBarFieldDescriptor != null && FieldDescriptor != null && FieldDescriptor.Equals(filterBarFieldDescriptor))
            {
                return true;
            }

            // caching for FilterBarChoice condition
            if (lastCompareRecord != null &&
                lastCompareVersion == record.Engine.SourceListVersion &&
                Object.ReferenceEquals(lastCompareRecord.Target, record))
            {
                return lastCompareRecordResult;
            }

            lastCompareRecord = new WeakReference(record);
            lastCompareRecordResult = CompareRecord(record);
            lastCompareVersion = record.Engine.SourceListVersion;

            return lastCompareRecordResult;
        }

        /// <summary>
        /// Evaluates this condition for the given record and returns True if the record
        /// meets the condition.
        /// </summary>
        /// <param name="record">The record to be evaluated.</param>
        /// <returns>True if the record
        /// meets the condition; False otherwise.</returns>
        public bool CompareRecord(Record record)
        {
            try
            {
                if (this.CompareOperator == FilterCompareOperator.Custom)
                {
                    if (this.customFilter != null)
                    {
                        return this.customFilter.CompareDescriptor(this, record);
                    }
                }

                if (FieldDescriptor == null)
                {
                    return true;
                }

                object x = GetValue(record);

                if (object.ReferenceEquals(dirty, preparedCompareValue))
                {
                    preparedCompareValue = compareValue;
                    if (!IsNull(compareValue))
                    {
                        if (this.compareOperator != FilterCompareOperator.Match
                            && this.compareOperator != FilterCompareOperator.Like
                            && this.compareOperator != FilterCompareOperator.Custom)
                        {
                            Type t;
                            if (IsNull(x))
                            {
                                t = FieldDescriptor.GetPropertyType();
                            }
                            else
                            {
                                t = x.GetType();
                            }

                            ////preparedCompareValue = NullableHelper.ChangeType(compareValue, t);
                            try
                            {
                                preparedCompareValue = Syncfusion.Styles.ValueConvert.ChangeType(compareValue, t, this.FieldDescriptor.TableDescriptor.Engine.Culture);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Error in input value" + ex);
                                return false;
                            }
                        }
                        else
                        {
                            preparedCompareValue = compareValue;
                        }

                        if (preparedCompareValue == null)
                        {
                            preparedCompareValue = DBNull.Value;
                        }
                    }
                }

                object y = preparedCompareValue;

                if (x == null)
                {
                    x = DBNull.Value;
                }

                if ((x.GetType().Equals(preparedCompareValue.GetType()) || (this.compareOperator == FilterCompareOperator.Match)) || (this.compareOperator == FilterCompareOperator.Like) || (this.compareOperator == FilterCompareOperator.NotEquals))
                {
                    switch (CompareOperator)
                    {
                        case FilterCompareOperator.Equals:
                            return (IsNull(x) && IsNull(y)) || Compare(x, y) == 0;

                        case FilterCompareOperator.GreaterThan:
                            return Compare(x, y) > 0;

                        case FilterCompareOperator.GreaterThanOrEqualTo:
                            return Compare(x, y) >= 0;

                        case FilterCompareOperator.LessThan:
                            return Compare(x, y) < 0;

                        case FilterCompareOperator.LessThanOrEqualTo:
                            return Compare(x, y) <= 0;

                        case FilterCompareOperator.Like:
                            if (IsNull(x) != IsNull(y))
                            {
                                return false;
                            }
                            else if (IsNull(x))
                            {
                                return true;
                            }
                            else
                            {
                                return Microsoft.VisualBasic.CompilerServices.StringType.StrLike(
                                      x.ToString(),
                                      y.ToString(),
                                      Microsoft.VisualBasic.CompareMethod.Text);
                            }

                        case FilterCompareOperator.NotEquals:
                            return !((IsNull(x) && IsNull(y)) || Compare(x, y) == 0);

                        case FilterCompareOperator.Match:
                            bool match = false;
                            if (IsNull(x) != IsNull(y))
                            {
                                return false;
                            }
                            else if (IsNull(y))
                            {
                                return true;
                            }
                            else
                            {
                                if (regexValue == null)
                                {
                                    regexValue = new Regex(compareValue.ToString());
                                }

                                match = regexValue.IsMatch(x.ToString());
                                return match;
                            }

                        default:
                            ////Debug.Assert(false, "Invalid enum value");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Any invalid character being entered will be ignored. \n Exception :" + ex.Message);
            }
            return false;
        }
    }

    /// <summary>
    /// A collection of <see cref="FilterCondition"/> that are children of a <see cref="RecordFilterDescriptor"/>
    /// with filter criteria for displaying only a subset of records from the underlying datasource.
    /// An instance of this collection is returned by the <see cref="RecordFilterDescriptor.Conditions"/> property
    /// of a <see cref="RecordFilterDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    [ListBindableAttribute(false)]
    public class FilterConditionCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        ArrayList inner = new ArrayList();

        /// <summary>
        /// Occurs after a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;

        ////internal int version;
        internal bool insideCollectionEditor = false;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>String representation of the object.</returns>
        /// <override/>
        public override string ToString()
        {
            return String.Format("FilterConditionCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
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
                    TraceUtil.TraceCurrentMethodInfo(this, value);
                }
#else
                ;
#endif
            }
        }

        void IInsideCollectionEditorProperty.InitializeFrom(object other)
        {
            InitializeFrom((FilterConditionCollection)other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(FilterConditionCollection other)
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
        public FilterConditionCollection()
        {
        }

        protected internal FilterConditionCollection(FilterCondition[] filterDescriptors)
        {
            this.inner.AddRange(filterDescriptors);
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                this[n].SetCollection(this);
            }
        }

        ////        FilterConditionCollection(FilterCondition[] filterDescriptors, int version)
        ////            : this(filterDescriptors)
        ////        {
        ////            this.version = version;
        ////        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public virtual FilterConditionCollection Clone()
        {
            FilterConditionCollection coll = new FilterConditionCollection();
            this.CopyAllMembersTo(coll);
            return coll;
        }

        protected void CopyAllMembersTo(FilterConditionCollection coll)
        {
            ////            coll.version = this.version;
            coll.inner = new ArrayList();
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                coll.inner.Add(this[n].Clone());
                coll[n].SetCollection(coll);
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the
        /// current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// True if the specified object is equal to the current
        /// object; otherwise, false.
        /// </returns>
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
            else if (!(obj is FilterConditionCollection))
            {
                return false;
            }

            return Equals((FilterConditionCollection)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code.
        /// </returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        ////        public int Version
        ////        {
        ////            get
        ////            {
        ////                return version;
        ////            }
        ////        }

        bool Equals(FilterConditionCollection other)
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
        public FilterCondition this[int index]
        {
            get
            {
                return (FilterCondition)inner[index];
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
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(FilterCondition value)
        {
            if (value == null)
            {
                return false;
            }

            return inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(FilterCondition value)
        {
            return inner.IndexOf(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(FilterCondition[] array, int index)
        {
            int count = Count;
            for (int n = 0; n < count; n++)
            {
                array[index + n] = this[n];
            }
        }

        FilterConditionCollection SyncRoot
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
        public FilterConditionCollectionEnumerator GetEnumerator()
        {
            return new FilterConditionCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, FilterCondition value)
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
        public void Remove(FilterCondition value)
        {
            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            inner.Remove(value);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds FilterCondition to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(FilterCondition value)
        {
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));
            int index = inner.Add(value);
            value.SetCollection(this);
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            return index;
        }

        /// <summary>
        /// Creates a FilterCondition and adds it to the end of the collection.
        /// </summary>
        /// <param name="compareOperator">The comparison operator.</param>
        /// <param name="compareValue">The comparison value.</param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(FilterCompareOperator compareOperator, object compareValue)
        {
            return Add(new FilterCondition(compareOperator, compareValue));
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
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            ////            version++;
            if (!this.InsideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, e.Item, e.Index); ////, version);
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

        /// <summary>
        /// Raises the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
                if (Changing != null)
                {
                    Changing(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanged(FilterCondition filterDescriptor, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, IndexOf(filterDescriptor), e.PropertyName, e);
                }
#else
                ;
#endif
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, IndexOf(filterDescriptor), filterDescriptor, e.PropertyName));
            }
        }

        internal void RaisePropertyItemChanging(FilterCondition filterDescriptor, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, IndexOf(filterDescriptor), filterDescriptor, e.PropertyName, e));
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
                this[index] = (FilterCondition)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (FilterCondition)value);
        }

        void IList.Remove(object value)
        {
            Remove((FilterCondition)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((FilterCondition)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((FilterCondition)value);
        }

        int IList.Add(object value)
        {
            return Add((FilterCondition)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((FilterCondition[])array, index);
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
    /// Enumerator class for <see cref="FilterCondition"/> elements of a <see cref="FilterConditionCollection"/>.
    /// </summary>
    public class FilterConditionCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        FilterConditionCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="parentCollection">The parent collection to enumerate.</param>
        public FilterConditionCollectionEnumerator(FilterConditionCollection parentCollection)
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
        public FilterCondition Current
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
