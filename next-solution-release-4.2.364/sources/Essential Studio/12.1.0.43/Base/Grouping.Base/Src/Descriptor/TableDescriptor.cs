//-------------------------------------------------------------------------------------------------
// <copyright file="TableDescriptor.cs" company="syncfusion">
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
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;
using System.Collections.Generic;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Maintains schema information for a table. Collections define fields, expressions,
    /// sorted, grouped columns, and related tables.
    /// </summary>
    /// <remarks>
    /// TableDescriptor has several collections that can either be auto-populated from the underlying datasource
    /// or manually modified.
    /// <para/>
    /// <list type="table">
    /// <listheader><term>Collection</term><description>Descriptions</description></listheader>
    /// <para/>
    /// <item><term>Fields</term><description>
    /// The Fields collection is auto-populated from the underlying sourcelists columns. Normally you will not
    /// modify this collection and just use it in its default auto-populated state. A fieldDescriptor holds a
    /// name, mapping name, and type of the column in the list.
    /// </description></item>
    /// <para/>
    /// <item><term>Relations</term><description>
    /// The Relations collection is auto-populated from the underlying sourcelist's relations. It will extract
    /// its information from an ADO.NET datasource. If you have other IList collections that are related, you can
    /// add RelationDescriptor manually to this collection and specify the primary and foreign key between the
    /// two lists.
    /// </description></item>
    /// <para/>
    /// <item><term>ExpressionFields</term><description>
    /// The ExpressionFields collection has ExpressionFields. You need to manually add ExpressionFields. This
    ///  collection is not auto-populated from the datasource. A fieldDescriptor holds a name and the formula
    ///  expression of the column.
    /// </description></item>
    /// <para/>
    /// <item><term>GroupedColumns</term><description>
    /// The GroupedColumns collections contains SortColumnDescriptor objects. It defines the grouping of the
    ///  table. Each SortColumnDescriptor has a name that identifies a field in the Fields or ExpressionFields
    ///  collection, a SortDirection property, and a FieldDescriptor property. The FieldDescriptor is Read-only
    ///  and looked up in the Fields or ExpressionFields collection using the Name of the SortColumnDescriptor.
    ///  A custom categorizer can be specified that allows you to group records into ranges of data, e.g. if
    ///  you want to group by month.
    /// </description></item>
    /// <para/>
    /// <item><term>Sortedcolumns</term><description>
    /// The Sortedcolumns collection contains SortColumnDescriptor objects. It specifies the sort order of
    /// records within a group.
    /// </description></item>
    /// <para/>
    /// <item><term>RecordFilters</term><description>
    /// The RecordFilters collection has RecordFilterDescriptor objects. RecordFilters define selection
    /// criteria to hide or show records based on criteria.
    /// </description></item>
    /// <para/>
    /// <item><term>Summaries</term><description>
    /// The Summaries collection holds SummaryDescriptors. A SummaryDescriptor has a name, a mapping name
    ///  that identifies the field (Fields or ExpressionFields collection) for which summaries should be
    ///  calculated for, and a SummaryType property that defines the type of calculations to be performed.
    /// Possible SummaryTypes are: Count, BooleanAggregate, ByteAggregate, CharAggregate, DistinctCount,
    /// DoubleAggregate, Int32Aggregate, MaxLength, StringAggregate, Vector, DoubleVector, and Custom.
    /// When you specify the SummaryType.Custom type, you need to set the custom method through the
    /// CreateSummaryMethod property of the SummaryDescriptor. It is of type CreateSummaryDelegate and
    /// is called to create a instance of a summary object.
    /// </description></item>
    /// <para/>
    /// </list>
    /// <para/>
    /// The Field and Relations collections feature auto-population on demand
    /// and reflect changes from the collection they depend on. The auto-population will happen when
    /// you access the contents of the collection, e.g. if you query its Count.
    /// <para/>
    /// GroupedColumns and SortedColumns need to be manually initialized. They are not auto-populated.
    /// <para/>
    /// SortedColumnDescriptor also lets you specify a custom comparer that implements the IComparer
    /// interface.
    /// </remarks>
    public class TableDescriptor : SourceListDescriptor, ITypedList, ITableEventsTarget
    {
        #region Fields
        IGroupByCategorizer defaultCategorizer = null;
        IGroupByCategorizer _groupByCategorizer = null;
        IComparer defaultComparer = null;
        IComparer _comparer = null;
        Engine engine;
        TableDescriptor parentTableDescriptor;
        RelationDescriptor parentRelation;
        FieldDescriptorCollection fields;
        UnboundFieldDescriptorCollection unboundFields;
        ExpressionFieldDescriptorCollection expressionFields;
        SortColumnDescriptorCollection groupedColumns;
        SortColumnDescriptorCollection primaryKeyColumns;
        SortColumnDescriptorCollection sortedColumns;
        RelationChildColumnDescriptorCollection relationChildColumns;
        SummaryDescriptorCollection summaryDescriptors;
        ////UFD:SummaryDescriptorCollection unfilteredSummaryDescriptors;
        RecordFilterDescriptorCollection recordFilters;
        RelationDescriptorCollection relations;
        ExpressionFieldEvaluator expressionColumnEvaluator;
        bool _allowNew = true;
        bool _allowRemove = true;
        bool _allowEdit = true;
        bool inInitializeFrom = false;
        bool inInitializeFromChanged = false;
        #if SyncfusionFramework4_0
        private Dictionary<string, Type> types = new Dictionary<string, Type>();
        List<DynamicPropertyDescriptor> props = new List<DynamicPropertyDescriptor>();
        #endif
                    
        #endregion
        #region Construct
        /// <summary>
        /// Initializes a new <see cref="TableDescriptor"/>
        /// </summary>
        public TableDescriptor()
        {
            Construct();
        }

        /// <summary>
        /// Initializes a new <see cref="TableDescriptor"/> that is a child of
        /// a <see cref="RelationDescriptor"/>.
        /// </summary>
        /// <param name="parentRelation">Parent relation.</param>
        public TableDescriptor(RelationDescriptor parentRelation)
        {
            this.parentRelation = parentRelation;
            if (parentRelation != null)
            {
                SetParentTableDescriptor(parentRelation.ParentTableDescriptor);
            }

            Construct();
        }

        /// <summary>
        /// Initializes a new <see cref="TableDescriptor"/> that is a child of
        /// a <see cref="RelationDescriptor"/>.
        /// </summary>
        /// <param name="engine">Grouping engine.</param>
        /// <param name="parentRelation">Parent relation.</param>
        public TableDescriptor(Engine engine, RelationDescriptor parentRelation)
        {
            this.engine = engine;
            this.parentRelation = parentRelation;
            if (parentRelation != null)
            {
                this.parentTableDescriptor = parentRelation.ParentTableDescriptor;
            }

            Construct();
        }

        static int tableCounter = 0;
        int tableId = 0;

        void Construct()
        {
            tableId = ++tableCounter;
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(parentRelation != null ? parentRelation.ToString() : string.Empty, tableId);
            }

            defaultCategorizer = new GroupedColumnCategorizer(groupedColumns);
            defaultComparer = new SortColumnComparer(this);

            expressionFields = new ExpressionFieldDescriptorCollection(this);
            fields = CreateFieldDescriptorCollection();
            fields.shouldPopulate = true;
            unboundFields = CreateUnboundFieldDescriptorCollection();
            unboundFields.shouldPopulate = false;
            relationChildColumns = new RelationChildColumnDescriptorCollection(this);
            relationChildColumns.isRelationChildColumns = true;
            groupedColumns = new SortColumnDescriptorCollection(this);
            primaryKeyColumns = new SortColumnDescriptorCollection(this);
            primaryKeyColumns.isPrimaryKeyColumns = true;
            sortedColumns = new SortColumnDescriptorCollection(this);
            summaryDescriptors = new SummaryDescriptorCollection(this);
            ////UFD:unfilteredSummaryDescriptors = new SummaryDescriptorCollection(this);
            recordFilters = new RecordFilterDescriptorCollection(this);
            relations = new RelationDescriptorCollection(this);
            relations.shouldPopulate = true;

            fields.Changed += new ListPropertyChangedEventHandler(fields_Changed);
            unboundFields.Changed += new ListPropertyChangedEventHandler(unboundFields_Changed);
            expressionFields.Changed += new ListPropertyChangedEventHandler(expressionFields_Changed);
            relationChildColumns.Changed += new ListPropertyChangedEventHandler(relationChildColumns_Changed);
            groupedColumns.Changed += new ListPropertyChangedEventHandler(groupedColumns_Changed);
            primaryKeyColumns.Changed += new ListPropertyChangedEventHandler(primaryKeyColumns_Changed);
            sortedColumns.Changed += new ListPropertyChangedEventHandler(sortedColumns_Changed);
            summaryDescriptors.Changed += new ListPropertyChangedEventHandler(summaryDescriptors_Changed);
            ////UFD:unfilteredSummaryDescriptors.Changed += new ListPropertyChangedEventHandler(unfilteredSummaryDescriptors_Changed);
            recordFilters.Changed += new ListPropertyChangedEventHandler(recordFilters_Changed);
            relations.Changed += new ListPropertyChangedEventHandler(relations_Changed);

            fields.Changing += new ListPropertyChangedEventHandler(fields_Changing);
            unboundFields.Changing += new ListPropertyChangedEventHandler(unboundFields_Changing);
            expressionFields.Changing += new ListPropertyChangedEventHandler(expressionFields_Changing);
            relationChildColumns.Changing += new ListPropertyChangedEventHandler(relationChildColumns_Changing);
            groupedColumns.Changing += new ListPropertyChangedEventHandler(groupedColumns_Changing);
            primaryKeyColumns.Changing += new ListPropertyChangedEventHandler(primaryKeyColumns_Changing);
            sortedColumns.Changing += new ListPropertyChangedEventHandler(sortedColumns_Changing);
            summaryDescriptors.Changing += new ListPropertyChangedEventHandler(summaryDescriptors_Changing);
            ////UFD:unfilteredSummaryDescriptors.Changing += new ListPropertyChangedEventHandler(unfilteredSummaryDescriptors_Changing);
            recordFilters.Changing += new ListPropertyChangedEventHandler(recordFilters_Changing);
            relations.Changing += new ListPropertyChangedEventHandler(relations_Changing);
        }

        SortColumnDescriptor[] _arrayOfColumnDescriptors;
        PropertyDescriptor[] _arrayOfPropertyDescriptor;

        /// <summary>
        /// Returns a combined array of RelationChildColumns, GroupedColumns and SortedColumns
        /// </summary>
        /// <returns>Relatin child columns, grouped columns and sorted columns.</returns>
        public SortColumnDescriptor[] GetSortDescriptors()
        {
            if (_arrayOfColumnDescriptors == null)
            {
                bool isSorted;
                SortColumnDescriptor[] arrayOfColumnDescriptors;
                PropertyDescriptor[] arrayOfPropertyDescriptor;

                GetSortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);
            }

            return _arrayOfColumnDescriptors;
        }

        /// <exclude/>
        internal void GetSortInfo(out bool isSorted, out SortColumnDescriptor[] arrayOfColumnDescriptors, out PropertyDescriptor[] arrayOfPropertyDescriptor)
        {
            isSorted = IsSorted;

            if (_arrayOfColumnDescriptors != null)
            {
                // cached
                arrayOfColumnDescriptors = _arrayOfColumnDescriptors;
                arrayOfPropertyDescriptor = _arrayOfPropertyDescriptor;
                return;
            }

            if (isSorted)
            {
                arrayOfColumnDescriptors = new SortColumnDescriptor[this.RelationChildColumns.Count + this.GroupedColumns.Count + this.SortedColumns.Count];
                int offset = 0;
                this.RelationChildColumns.CopyTo(arrayOfColumnDescriptors, offset);
                offset += this.RelationChildColumns.Count;
                this.GroupedColumns.CopyTo(arrayOfColumnDescriptors, offset);
                offset += this.GroupedColumns.Count;
                this.SortedColumns.CopyTo(arrayOfColumnDescriptors, offset);
                arrayOfPropertyDescriptor = new PropertyDescriptor[arrayOfColumnDescriptors.Length];

                int count = arrayOfColumnDescriptors.Length;
                for (int n = 0; n < count; n++)
                {
                    SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[n];
                    if (columnDescriptor != null)
                    {
                        FieldDescriptor fd = columnDescriptor.FieldDescriptor;
                        if (fd != null && !ShouldSortByDisplayMember(columnDescriptor))
                        {
                            arrayOfPropertyDescriptor[n] = fd.GetSimplePropertyDescriptor();
                        }
                    }
                }
            }
            else
            {
                arrayOfColumnDescriptors = new SortColumnDescriptor[0];
                arrayOfPropertyDescriptor = new PropertyDescriptor[0];
            }

            ////cache it
            _arrayOfColumnDescriptors = arrayOfColumnDescriptors;
            _arrayOfPropertyDescriptor = arrayOfPropertyDescriptor;
        }

        SortColumnDescriptor[] _arrayOfPKColumnDescriptors;
        PropertyDescriptor[] _arrayOfPKPropertyDescriptor;

        /// <summary>
        /// Determines if the specified column should be sorted by the DisplayMember.
        /// Default behavior of the method is to return false.
        /// GridGroupingControl overrides this method and checks whether the GridColumnDescriptor
        /// associated with column has its GridColumnDescriptor.SortByDisplayMember 
        /// property set to true.
        /// </summary>
        /// <param name="cd">The column.</param>
        /// <returns>returns False.</returns>
        public virtual bool ShouldSortByDisplayMember(SortColumnDescriptor cd)
        {
            return false;
        }

        /// <internalonly/>
        internal protected virtual void InitSortByDisplayMemberCols()
        {
        }

        /// <summary>
        /// Returns an array of PrimaryKeyColumns
        /// </summary>
        /// <returns>Primary key columns.</returns>
        public SortColumnDescriptor[] GetPrimaryKeySortDescriptors()
        {
            if (_arrayOfPKColumnDescriptors == null)
            {
                bool isSorted;
                SortColumnDescriptor[] arrayOfColumnDescriptors;
                PropertyDescriptor[] arrayOfPropertyDescriptor;

                GetPrimaryKeySortInfo(out isSorted, out arrayOfColumnDescriptors, out arrayOfPropertyDescriptor);
            }

            return _arrayOfPKColumnDescriptors;
        }

        /// <exclude/>
        internal void GetPrimaryKeySortInfo(out bool isSorted, out SortColumnDescriptor[] arrayOfColumnDescriptors, out PropertyDescriptor[] arrayOfPropertyDescriptor)
        {
            isSorted = this.PrimaryKeyColumns.Count > 0;

            if (_arrayOfPKColumnDescriptors != null)
            {
                // cached
                arrayOfColumnDescriptors = _arrayOfPKColumnDescriptors;
                arrayOfPropertyDescriptor = _arrayOfPKPropertyDescriptor;
                return;
            }

            if (isSorted)
            {
                arrayOfColumnDescriptors = new SortColumnDescriptor[this.PrimaryKeyColumns.Count];
                this.PrimaryKeyColumns.CopyTo(arrayOfColumnDescriptors, 0);
                arrayOfPropertyDescriptor = new PropertyDescriptor[arrayOfColumnDescriptors.Length];

                int count = arrayOfColumnDescriptors.Length;
                for (int n = 0; n < count; n++)
                {
                    SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[n];
                    if (columnDescriptor != null)
                    {
                        FieldDescriptor fd = columnDescriptor.FieldDescriptor;
                        if (fd != null)
                        {
                            arrayOfPropertyDescriptor[n] = fd.GetSimplePropertyDescriptor();
                        }
                    }
                }
            }
            else
            {
                arrayOfColumnDescriptors = new SortColumnDescriptor[0];
                arrayOfPropertyDescriptor = new PropertyDescriptor[0];
            }

            ////cache it
            _arrayOfPKColumnDescriptors = arrayOfColumnDescriptors;
            _arrayOfPKPropertyDescriptor = arrayOfPropertyDescriptor;
        }

        /// <summary>For internal use.</summary>
        /// <exclude/>
        public void ResetSortInfoCache()
        {
            _arrayOfColumnDescriptors = null;
            _arrayOfPKColumnDescriptors = null;
            _arrayOfPropertyDescriptor = null;
            _arrayOfPKPropertyDescriptor = null;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(this.Name, this.tableId);
            }

            if (disposing)
            {
                fields.Changed -= new ListPropertyChangedEventHandler(fields_Changed);
                unboundFields.Changed -= new ListPropertyChangedEventHandler(unboundFields_Changed);
                expressionFields.Changed -= new ListPropertyChangedEventHandler(expressionFields_Changed);
                relationChildColumns.Changed -= new ListPropertyChangedEventHandler(relationChildColumns_Changed);
                groupedColumns.Changed -= new ListPropertyChangedEventHandler(groupedColumns_Changed);
                primaryKeyColumns.Changed -= new ListPropertyChangedEventHandler(primaryKeyColumns_Changed);
                sortedColumns.Changed -= new ListPropertyChangedEventHandler(sortedColumns_Changed);
                summaryDescriptors.Changed -= new ListPropertyChangedEventHandler(summaryDescriptors_Changed);
                ////UFD:unfilteredSummaryDescriptors.Changed -= new ListPropertyChangedEventHandler(unfilteredSummaryDescriptors_Changed);
                recordFilters.Changed -= new ListPropertyChangedEventHandler(recordFilters_Changed);
                relations.Changed -= new ListPropertyChangedEventHandler(relations_Changed);

                expressionFields.Changing -= new ListPropertyChangedEventHandler(expressionFields_Changing);
                fields.Changing -= new ListPropertyChangedEventHandler(fields_Changing);
                unboundFields.Changing -= new ListPropertyChangedEventHandler(unboundFields_Changing);
                relationChildColumns.Changing -= new ListPropertyChangedEventHandler(relationChildColumns_Changing);
                groupedColumns.Changing -= new ListPropertyChangedEventHandler(groupedColumns_Changing);
                primaryKeyColumns.Changing -= new ListPropertyChangedEventHandler(primaryKeyColumns_Changing);
                sortedColumns.Changing -= new ListPropertyChangedEventHandler(sortedColumns_Changing);
                summaryDescriptors.Changing -= new ListPropertyChangedEventHandler(summaryDescriptors_Changing);
                ////UFD:unfilteredSummaryDescriptors.Changing -= new ListPropertyChangedEventHandler(unfilteredSummaryDescriptors_Changing);
                recordFilters.Changing -= new ListPropertyChangedEventHandler(recordFilters_Changing);
                relations.Changing -= new ListPropertyChangedEventHandler(relations_Changing);

                fields.Dispose();
                this.unboundFields.Dispose();
                expressionFields.Dispose();
                groupedColumns.Dispose();
                primaryKeyColumns.Dispose();
                sortedColumns.Dispose();
                relationChildColumns.Dispose();
                summaryDescriptors.Dispose();
                ////UFD:unfilteredSummaryDescriptors.Dispose();
                recordFilters.Dispose();
                relations.Dispose();
                this.tableEventsTarget = null;
                ////                engine = null;
                //                parentTableDescriptor = null;
                //                parentRelation = null;
                //
                //                this.defaultCategorizer = null;
                //                this.defaultComparer = null;
                //                this.fields = null;
                //                this.unboundFields = null;
                //                this.expressionFields = null;
                //                this.groupedColumns = null;
                //                this.primaryKeyColumns = null;
                //                this.sortedColumns = null;
                //                this.relationChildColumns = null;
                //                this.summaryDescriptors = null;
                //                this.recordFilters = null;
                ////                this.relations = null;

                ResetSortInfoCache();
            }

            base.Dispose(disposing);
        }

        #endregion
        #region Parents
        /// <summary>
        /// Gets a reference to the <see cref="Engine"/> object that this table descriptor belongs to.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Engine Engine
        {
            get
            {
                if (engine == null && parentTableDescriptor != null)
                {
                    engine = this.parentTableDescriptor.Engine;
                }

                return engine;
            }
        }

        /// <summary>
        /// Gets a reference to the parent <see cref="RelationDescriptor"/>.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RelationDescriptor ParentRelation
        {
            get
            {
                return this.parentRelation;
            }
        }

        /// <summary>
        /// Gets a reference to the parent <see cref="TableDescriptor"/> which has a <see cref="RelationDescriptor"/>
        /// that references this object.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public TableDescriptor ParentTableDescriptor
        {
            get
            {
                return parentTableDescriptor;
            }
        }

        internal void SetParentTableDescriptor(TableDescriptor parentTableDescriptor)
        {
            if (this.parentTableDescriptor != parentTableDescriptor || engine == null)
            {
                this.parentTableDescriptor = parentTableDescriptor;
                if (parentTableDescriptor != null)
                {
                    this.engine = parentTableDescriptor.Engine;
                }
            }
        }
        #endregion
        #region InitializeFrom, Modified, Reset methods and PropertyChange event
        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public virtual void InitializeFrom(TableDescriptor other)
        {
            inInitializeFromChanged = false;

            this.AllowEdit = other.AllowEdit;
            this.AllowNew = other.AllowNew;
            this.AllowRemove = other.AllowRemove;

            if (other.ShouldSerializeRelations())
            {
                Relations.InitializeFrom(other.Relations);
            }
            else
            {
                ResetRelations();
            }

            if (other.ForceEmptyRelations)
            {
                Relations.Clear();
            }

            if (other.ShouldSerializeExpressionFields())
            {
                ExpressionFields.InitializeFrom(other.ExpressionFields);
            }
            else
            {
                ResetExpressionFields();
            }

            if (other.ShouldSerializeUnboundFields())
            {
                UnboundFields.InitializeFrom(other.UnboundFields);
            }
            else
            {
                ResetUnboundFields();
            }

            if (other.ShouldSerializeFields())
            {
                Fields.InitializeFrom(other.Fields);
            }
            else
            {
                ResetFields();
            }

            if (other.ShouldSerializeName())
            {
                Name = other.Name;
            }
            else if (ShouldSerializeName())
            {
                ResetName();
            }

            if (other.ShouldSerializeRecordFilters())
            {
                RecordFilters.InitializeFrom(other.RecordFilters);
            }
            else
            {
                ResetRecordFilters();
            }

            if (other.ShouldSerializeSortedColumns())
            {
                SortedColumns.InitializeFrom(other.SortedColumns);
            }
            else
            {
                ResetSortedColumns();
            }

            if (other.ShouldSerializeSummaries())
            {
                Summaries.InitializeFrom(other.Summaries);
            }
            else
            {
                ResetSummaries();
            }

            if (other.ShouldSerializeGroupedColumns())
            {
                GroupedColumns.InitializeFrom(other.GroupedColumns);
            }
            else
            {
                ResetGroupedColumns();
            }

            if (other.ShouldSerializePrimaryKeyColumns())
            {
                PrimaryKeyColumns.InitializeFrom(other.PrimaryKeyColumns);
            }
            else
            {
                ResetPrimaryKeyColumns();
            }

            if (other.ShouldSerializeRelationChildColumns())
            {
                RelationChildColumns.InitializeFrom(other.RelationChildColumns);
            }
            else
            {
                ResetRelationChildColumns();
            }

            InitializePropertyDescriptors();

            inInitializeFrom = false;
            if (inInitializeFromChanged)
            {
                this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("InitializeFrom"));
            }
        }

        /// <summary>
        /// Resets all properties to default settings.
        /// </summary>
        public virtual void ResetTableDescriptor()
        {
            AllowEdit = true;
            AllowNew = true;
            AllowRemove = true;
            ResetExpressionFields();
            ResetFields();
            ResetGroupedColumns();
            ResetPrimaryKeyColumns();
            ResetRelationChildColumns();
            ResetName();
            ResetRecordFilters();
            ResetRelations();
            ResetSortedColumns();
            ResetSummaries();
            InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Reset"));
        }

        /// <summary>
        /// Determines if any properties have been modified.
        /// </summary>
        /// <returns>True if any property was modified.</returns>
        public virtual bool GetModified()
        {
            return
                !AllowEdit ||
                !AllowNew ||
                !AllowRemove ||
                ShouldSerializeExpressionFields() ||
                ShouldSerializeFields() ||
                ShouldSerializeGroupedColumns() ||
                ShouldSerializePrimaryKeyColumns() ||
                ShouldSerializeRelationChildColumns() ||
                ShouldSerializeName() ||
                ShouldSerializeRecordFilters() ||
                ShouldSerializeRelations() ||
                ShouldSerializeSortedColumns() ||
                ShouldSerializeSummaries();
        }

        /// <override/>
        /// <summary>
        /// Determines if the <see cref="Name"/>  was modified from its default value.
        /// </summary>
        /// <returns>True if the name was modified.</returns>
        public override bool ShouldSerializeName()
        {
            return this.ParentRelation == null && base.ShouldSerializeName();
        }

        /// <summary>Gets or sets the name of the table descriptor.</summary>
        /// <override/>
        [NotifyParentProperty(true)]
        [Description("Displays the name on top of the control"),
        Category("TableDescriptors")]
        public override string Name
        {
            get
            {
                RelationDescriptor rd = this.ParentRelation;
                if (rd != null)
                {
                    ////if (rd.ShouldSerializeName() || rd.RelationKind == RelationKind.ListItemReference || rd.RelationKind == RelationKind.UniformChildList)
                    return rd.Name;
                    ////else
                    //    return rd.ChildTableName;
                }

                return base.Name;
            }

            set
            {
                RelationDescriptor rd = this.ParentRelation;
                if (rd != null)
                {
                    rd.Name = value;
                }
                else
                {
                    base.Name = value;
                }
            }
        }

        /// <summary>
        /// Occurs when a property or child object has been changed.
        /// </summary>
        [Description("Occurs when a property or child object has been changed")] 
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

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

            if (inInitializeFrom)
            {
                inInitializeFromChanged = true;
                return;
            }

#if DEBUG

            if (Switches.DescriptorPropertyChange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else

            ;
#endif
            ResetSortInfoCache();

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs before a property or child object is changed.
        /// </summary>
        [Description("Occurs before a property or child object is changed")] 
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

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

#if DEBUG

            if (Switches.DescriptorPropertyChange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else

            ;
#endif
            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }
        }

        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                string isdisposed = IsDisposed ? ", Disposed" : string.Empty;
                return GetType().Name + " { " + Name + "(" + this.tableId + isdisposed + ") }";
            }
            else
            {
                return Name;
            }
        }

        #endregion
        #region Relations collection
        /// <summary>
        /// Gets the collection of <see cref="RelationDescriptor"/> objects defining relations
        /// to other tables.
        /// </summary>
        /// <remarks>
        /// The default state of this collection and child objects is auto-populated from relation descriptors
        /// found in the underlying source list for this table. <para/>
        /// If you assign a <see cref="System.Data.DataView"/> or <see cref="System.Data.DataSet"/> to <see cref="Syncfusion.Grouping.Engine.SetSourceList"/>,
        /// the <see cref="Relations"/> collection is auto-populated from <see cref="System.Data.DataRelation"/> objects
        /// found in the <see cref="System.Data.DataSet"/>.
        /// </remarks>
        [XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden), Description("Gets the collection of RelationDescriptor objects defining relations to other tables")]
        [ListBindableAttribute(false)]
        public RelationDescriptorCollection Relations
        {
            get
            {
                return relations;
            }
        }

        /// <summary>
        /// Determines if the <see cref="Relations"/> collection or child objects have been modified from its
        /// default state.
        /// </summary>
        /// <returns>True if the objects have been modified.</returns>
        public virtual bool ShouldSerializeRelations()
        {
            return relations != null && relations.IsModified;
        }

        /// <summary>
        /// Resets the <see cref="Relations"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetRelations()
        {
            Relations.Reset();
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Relations"/> collection should not be autopopulated. When you set this property true 
        /// <see cref="Relations"/>.<see cref="Relations.Clear"/> will be called. When you set this property false, <see cref="ResetRelations"/>
        /// will be called.
        /// </summary>
        /// <returns></returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool ForceEmptyRelations
        {
            get
            {
                return ShouldSerializeRelations() && Relations.Count == 0;
            }

            set
            {
                if (value != ForceEmptyRelations)
                {
                    if (value)
                    {
                        Relations.Clear();
                    }
                    else
                    {
                        ResetRelations();
                    }
                }
            }
        }

        #endregion
        #region Fields collection
        /// <summary>
        /// Gets the collection of <see cref="FieldDescriptor"/> objects defining fields that
        /// represent bound or unbound values for each row in the table.
        /// </summary>
        /// <remarks>
        /// The default state of this collection and child objects is auto-populated from property descriptors
        /// found in the underlying source list for this table and expression fields that have been defined
        /// for this table. <para/>
        /// If you assign a <see cref="System.Data.DataView"/> or <see cref="System.Data.DataTable"/> to <see cref="Syncfusion.Grouping.Engine.SetSourceList"/>,
        /// the <see cref="FieldDescriptorCollection"/> collection is auto-populated from <see cref="System.Data.DataColumn"/> objects
        /// found in the <see cref="System.Data.DataView"/>. Expression fields will be appended to the collection.
        /// <para/>
        /// Bound fields do have a <see cref="FieldDescriptor.MappingName"/> that identifies a column in the underlying list.
        /// <para/>
        /// Unbound field do have an empty <see cref="FieldDescriptor.MappingName"/>.
        /// </remarks>
        [XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [ListBindableAttribute(false)]
        public FieldDescriptorCollection Fields
        {
            get
            {
                return fields;
            }

            set
            {
                if (value != null)
                {
                    Fields.InitializeFrom(value);
                }
                else
                {
                    ResetFields();
                }
            }
        }

        /// <summary>
        /// Gets / sets the <see cref="PropertyDescriptorCollection"/> with properties for
        /// each record in the table.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override PropertyDescriptorCollection ItemProperties
        {
            get
            {
                #if SyncfusionFramework4_0
                if (this.Engine.IsDynamicData)
                {
                    if (!this.ShouldSerializeItemProperties())
                    {
                        this.OnInitializeItemProperties(EventArgs.Empty);
                    }
                    PropertyDescriptorCollection itemProperties = null;
                    IDictionary<string, object> dynamicObject = null;
                   
                    IEnumerable source = this.Engine.GetSourceList();
                    if (source != null)
                    {
                        foreach (object o in source)
                        {
                            dynamicObject = o as IDictionary<string, object>;
                            if (dynamicObject == null)
                                continue;
                            break;
                        }
                        if (dynamicObject != null)
                        {
                            types.Clear();
                            props.Clear();
                            PopulateDynamicPropertiesandTypes(dynamicObject);
                            itemProperties = new PropertyDescriptorCollection(props.ToArray());
                            return itemProperties;
                        }
                    }
                }
                #endif
                return base.ItemProperties;
            }
            set
            {
                base.ItemProperties = value;
            }
        }

        #if SyncfusionFramework4_0
        /// <summary>
        /// Retrieves the dynamic properties and its design time types for internal usage.
        /// </summary>
        /// <param name="obj">dynamic item</param>
        private void PopulateDynamicPropertiesandTypes(IDictionary<string, object> obj)
        {
            foreach (string key in obj.Keys)
            {
                if (obj[key] is IDictionary<string, object>)
                {
                    PopulateDynamicPropertiesandTypes(obj[key] as IDictionary<string, object>);
                }
                else if(!types.ContainsKey(key))
                {
                    props.Add(new DynamicPropertyDescriptor(key, null));
                    if (obj[key] != null)
                        types.Add(key, obj[key].GetType());
                    else
                        types.Add(key,typeof(object));
                }
            }
        }

        /// <summary>
        /// For dynamic datasource only. Returns null for other type sources.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Dictionary<string, Type> ItemPropertyTypes
        {
            get
            {
                return types;
            }
        }
        #endif

        /// <summary>
        /// Determines if the <see cref="Fields"/> collection has been modified from its
        /// default state.
        /// </summary>
        /// <returns>True if it is modified; False otherwise.</returns>
        public virtual bool ShouldSerializeFields()
        {
            return fields != null && fields.IsModified;
        }

        /// <summary>
        /// Resets the <see cref="Fields"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetFields()
        {
            if (ShouldSerializeFields())
            {
                Fields.Reset();
            }
        }

        private FieldDescriptorCollection CreateFieldDescriptorCollection()
        {
            return new FieldDescriptorCollection(this);
        }
        #endregion
        #region Fields collection
        /// <summary>
        /// Gets the collection of <see cref="FieldDescriptor"/> objects defining unbound fields that
        /// are filled by the user.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content), Description("Gets the collection of FieldDescriptor objects defining unbound fields that are filled by the user")]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public UnboundFieldDescriptorCollection UnboundFields
        {
            get
            {
                return unboundFields;
            }

            set
            {
                if (value != null)
                {
                    UnboundFields.InitializeFrom(value);
                }
                else
                {
                    ResetUnboundFields();
                }
            }
        }

        /// <summary>
        /// Determines if the <see cref="UnboundFields"/> collection has been modified from its
        /// default state.
        /// </summary>
        /// <returns>True if the collection has been modified.</returns>
        public virtual bool ShouldSerializeUnboundFields()
        {
            return unboundFields != null && unboundFields.IsModified;
        }

        /// <summary>
        /// Resets the <see cref="UnboundFields"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetUnboundFields()
        {
            if (ShouldSerializeUnboundFields())
            {
                UnboundFields.Reset();
            }
        }

        private UnboundFieldDescriptorCollection CreateUnboundFieldDescriptorCollection()
        {
            return new UnboundFieldDescriptorCollection(this);
        }
        #endregion
        #region ExpressionFields collection
        /// <summary>
        /// Gets the collection of <see cref="ExpressionFieldDescriptor"/> objects defining expression fields that
        /// represent values for each row in the table. Expression fields can reference other fields and
        /// support arithmetic calculatations and boolean expressions.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content), Description("Gets the collection of ExpressionFieldDescriptor objects defining expression fields that represent values for each row in the table")]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ExpressionFieldDescriptorCollection ExpressionFields
        {
            get
            {
                return expressionFields;
            }

            set
            {
                if (value != null)
                {
                    ExpressionFields.InitializeFrom(value);
                }
                else
                {
                    ResetExpressionFields();
                }
            }
        }

        /// <summary>
        /// Determines if the <see cref="ExpressionFields"/> collection contains values.
        /// </summary>
        /// <returns>True if the collection is not empty.</returns>
        public virtual bool ShouldSerializeExpressionFields()
        {
            return expressionFields != null && expressionFields.Count > 0;
        }

        /// <summary>
        /// Clears the <see cref="ExpressionFields"/> collection.
        /// </summary>
        public void ResetExpressionFields()
        {
            if (ShouldSerializeExpressionFields())
            {
                ExpressionFields.Clear();
            }
        }

        #endregion
        #region GroupedColumns collection
        /// <summary>
        /// Gets the collection of <see cref="SortColumnDescriptor"/> objects defining group by
        /// state of the table. Each <see cref="SortColumnDescriptor"/> in the collection references
        /// a <see cref="FieldDescriptor"/> of the <see cref="Fields"/> collection.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content), Description("Gets the collection of SortColumnDescriptor objects defining group by state of the table")]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public SortColumnDescriptorCollection GroupedColumns
        {
            get
            {
                return groupedColumns;
            }

            set
            {
                if (value != null)
                {
                    GroupedColumns.InitializeFrom(value);
                }
                else
                {
                    ResetGroupedColumns();
                }
            }
        }

        /// <summary>
        /// Determines if the <see cref="GroupedColumns"/> collection contains values.
        /// </summary>
        /// <returns>True if the collection is not empty.</returns>
        public virtual bool ShouldSerializeGroupedColumns()
        {
            return GroupedColumns.IsModified;
        }

        /// <summary>
        /// Clears the <see cref="GroupedColumns"/> collection.
        /// </summary>
        public void ResetGroupedColumns()
        {
            if (ShouldSerializeGroupedColumns())
            {
                GroupedColumns.Reset();
            }
        }

        #endregion
        #region PrimaryKeyColumns collection
        /// <summary>
        /// Gets the collection of <see cref="SortColumnDescriptor"/> objects defining the primary key(s)
        /// for records of the table. Each <see cref="SortColumnDescriptor"/> in the collection references
        /// a <see cref="FieldDescriptor"/> of the <see cref="Fields"/> collection.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content), Description("Gets the collection of ortColumnDescriptor objects defining the primary key(s) for records of the table")]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public virtual SortColumnDescriptorCollection PrimaryKeyColumns
        {
            get
            {
                return primaryKeyColumns;
            }

            set
            {
                if (value != null)
                {
                    PrimaryKeyColumns.InitializeFrom(value);
                }
                else
                {
                    ResetPrimaryKeyColumns();
                }
            }
        }

        /// <summary>
        /// Determines if the <see cref="PrimaryKeyColumns"/> collection contains values.
        /// </summary>
        /// <returns>True if the collection contains values.</returns>
        public virtual bool ShouldSerializePrimaryKeyColumns()
        {
            return PrimaryKeyColumns.IsModified;
        }

        /// <summary>
        /// Clears the <see cref="PrimaryKeyColumns"/> collection.
        /// </summary>
        public void ResetPrimaryKeyColumns()
        {
            if (ShouldSerializePrimaryKeyColumns())
            {
                PrimaryKeyColumns.Reset();
            }
        }

        #endregion
        #region Summaries collection
        /// <summary>
        /// Gets the collection of <see cref="SummaryDescriptor"/> objects defining summaries
        /// of the table. Each <see cref="SummaryDescriptor"/> in the collection references
        /// a <see cref="FieldDescriptor"/> of the <see cref="Fields"/> collection. Based
        /// on the summaries defined in this collection each group in the table will have
        /// summaries calculated.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public SummaryDescriptorCollection Summaries
        {
            get
            {
                return summaryDescriptors;
            }

            set
            {
                if (value != null)
                {
                    Summaries.InitializeFrom(value);
                }
                else
                {
                    ResetSummaries();
                }
            }
        }

        /// <summary>
        /// Determines if the <see cref="Summaries"/> collection contains values.
        /// </summary>
        /// <returns>True if it contains values.</returns>
        public virtual bool ShouldSerializeSummaries()
        {
            return Summaries.Count > 0;
        }

        /// <summary>
        /// Clears the <see cref="Summaries"/> collection.
        /// </summary>
        public void ResetSummaries()
        {
            if (ShouldSerializeSummaries())
            {
                Summaries.Clear();
            }
        }

        #endregion
        #region UnfilteredSummaries collection
        /* UFD:
                /// <unfilteredSummary>
                /// Gets the collection of <see cref="UnfilteredSummaryDescriptor"/> objects defining unfilteredSummaries
                /// of the table. Each <see cref="UnfilteredSummaryDescriptor"/> in the collection references
                /// a <see cref="FieldDescriptor"/> of the <see cref="Fields"/> collection. Based
                /// on the unfilteredSummaries defined in this collection, each group in the table will have
                /// unfilteredSummaries calculated.
                /// </unfilteredSummary>
                [RefreshProperties(RefreshProperties.All)]
                [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
                [ListBindableAttribute(false)]
                [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
                public SummaryDescriptorCollection UnfilteredSummaries
                {
                    get
                    {
                        return unfilteredSummaryDescriptors;
                    }
                    set
                    {
                        if (value != null)
                            UnfilteredSummaries.InitializeFrom(value);
                        else
                            ResetUnfilteredSummaries();
                    }
                }

                /// <unfilteredSummary>
                /// Determines if the <see cref="UnfilteredSummaries"/> collection contains values.
                /// </unfilteredSummary>
                /// <returns></returns>
                public virtual bool ShouldSerializeUnfilteredSummaries()
                {
                    return UnfilteredSummaries.Count > 0;
                }

                /// <unfilteredSummary>
                /// Clears the <see cref="UnfilteredSummaries"/> collection.
                /// </unfilteredSummary>
                public void ResetUnfilteredSummaries()
                {
                    if (ShouldSerializeUnfilteredSummaries())
                        UnfilteredSummaries.Clear();
                }
        */
        #endregion
        #region RecordFilters collection
        /// <summary>
        /// Gets the collection of <see cref="RecordFilterDescriptor"/> objects defining filter criteria
        /// for records in the table. Each <see cref="RecordFilterDescriptor"/> in the collection references
        /// one or multiple <see cref="FieldDescriptor"/> of the <see cref="Fields"/> collection. Multiple
        /// criteria can be combined with logical "And" or "Or" operations.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content), Description("Gets the collection of RecordFilterDescriptor objects defining filter criteria for records in the table")]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public RecordFilterDescriptorCollection RecordFilters
        {
            get
            {
                return recordFilters;
            }

            set
            {
                if (value != null)
                {
                    RecordFilters.InitializeFrom(value);
                }
                else
                {
                    ResetRecordFilters();
                }
            }
        }

        /// <summary>
        /// Determines if the <see cref="RecordFilters"/> collection contains values.
        /// </summary>
        /// <returns>True if it contains values; False otherwise.</returns>
        public virtual bool ShouldSerializeRecordFilters()
        {
            return RecordFilters.Count > 0;
        }

        /// <summary>
        /// Clears the <see cref="RecordFilters"/> collection.
        /// </summary>
        public void ResetRecordFilters()
        {
            if (ShouldSerializeRecordFilters())
            {
                RecordFilters.Clear();
            }
        }

        #endregion
        #region SortedColumns collection
        /// <summary>
        /// Gets the collection of <see cref="SortColumnDescriptor"/> objects defining sort state
        /// of the table. Each <see cref="SortColumnDescriptor"/> in the collection references
        /// a <see cref="FieldDescriptor"/> of the <see cref="Fields"/> collection. Multiple
        /// columns can be specified for sorting with the first column having the highest sort precedence.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content), Description("Gets the collection of SortColumnDescriptor objects defining sort state of the table")]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public SortColumnDescriptorCollection SortedColumns
        {
            get
            {
                return sortedColumns;
            }

            set
            {
                if (value != null)
                {
                    SortedColumns.InitializeFrom(value);
                }
                else
                {
                    ResetSortedColumns();
                }
            }
        }

        /// <summary>
        /// Determines if the <see cref="SortedColumns"/> collection contains values.
        /// </summary>
        /// <returns>True if it contains values.</returns>
        public virtual bool ShouldSerializeSortedColumns()
        {
            return SortedColumns.IsModified;
        }

        /// <summary>
        /// Clears the <see cref="SortedColumns"/> collection.
        /// </summary>
        public void ResetSortedColumns()
        {
            if (ShouldSerializeSortedColumns())
            {
                SortedColumns.Clear();
            }
        }

        #endregion
        #region AllowNew
        /// <summary>
        /// Gets / sets a value if adding new records should be allowed when the the underlying
        /// source list supports adding records.
        /// </summary>
        [DefaultValue(true)]
        [NotifyParentProperty(true)]
        [Description("Whether to allow add new records."),
        Category("TableDescriptors")]
        public bool AllowNew
        {
            get
            {
                return _allowNew;
            }

            set
            {
                if (_allowNew != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowNew"));
                    _allowNew = value;
                    OnAllowNewChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="AllowNew"/> property is changed.
        /// </summary>
        [Description("Occurs when the AllowNew property is changed")]
        public event EventHandler AllowNewChanged;

        /// <summary>
        /// Raises the <see cref="AllowNewChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnAllowNewChanged(EventArgs e)
        {
            if (AllowNewChanged != null)
            {
                AllowNewChanged(this, e);
            }

            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowNew"));
        }
        #endregion
        #region AllowRemove
        /// <summary>
        /// Gets / sets a value if removing records should be allowed when the the underlying
        /// source list supports removing records.
        /// </summary>
        [DefaultValue(true)]
        [NotifyParentProperty(true)]
        [Description("Whether to allow remove records"),
        Category("TableDescriptors")]
        public bool AllowRemove
        {
            get
            {
                return _allowRemove;
            }

            set
            {
                if (_allowRemove != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowRemove"));
                    _allowRemove = value;
                    OnAllowRemoveChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="AllowRemove"/> property is changed.
        /// </summary>
        [Description("Occurs when the AllowRemove property is changed")]
        public event EventHandler AllowRemoveChanged;

        /// <summary>
        /// Raises the <see cref="AllowRemoveChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnAllowRemoveChanged(EventArgs e)
        {
            if (AllowRemoveChanged != null)
            {
                AllowRemoveChanged(this, e);
            }

            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowRemove"));
        }

        #endregion
        #region AllowEdit
        /// <summary>
        /// Gets / sets a value if editing records should be allowed when the the underlying
        /// source list supports editing records.
        /// </summary>
        [DefaultValue(true)]
        [NotifyParentProperty(true)]
        [Description("Whether to allow edit records"),
        Category("TableDescriptors")]
        public bool AllowEdit
        {
            get
            {
                return _allowEdit;
            }

            set
            {
                if (_allowEdit != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowEdit"));
                    _allowEdit = value;
                    OnAllowEditChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="AllowEdit"/> property is changed.
        /// </summary>
        [Description("Occurs when the AllowEdit property is changed")]
        public event EventHandler AllowEditChanged;

        /// <summary>
        /// Raises the <see cref="AllowEditChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnAllowEditChanged(EventArgs e)
        {
            if (AllowEditChanged != null)
            {
                AllowEditChanged(this, e);
            }

            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowEdit"));
        }

        #endregion
        #region ExpressionFieldEvaluator Getter
        /// <summary>
        /// Gets the <see cref="ExpressionFieldEvaluator"/> for this table which provides
        /// methods for parsing and calculating formulas that can reference fields of the
        /// <see cref="Fields"/> collection.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ExpressionFieldEvaluator ExpressionFieldEvaluator
        {
            get
            {
                if (expressionColumnEvaluator == null)
                {
                    if (this.Engine != null)
                    {
                        expressionColumnEvaluator = Engine.CreateExpressionFieldEvaluator(this);
                    }
                }

                return expressionColumnEvaluator;
            }
        }
        #endregion
        #region RelationChildColumns Collection
        /// <summary>
        /// A collection from <see cref="RelationChildColumnDescriptor"/> that are children of a <see cref="TableDescriptor"/>.
        /// A RelationChildColumnDescriptor defines the sort order of a related table which is defined by the child columns in a 
        /// a master details relation. 
        /// </summary>
        [Browsable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [ListBindableAttribute(false)]
        public RelationChildColumnDescriptorCollection RelationChildColumns
        {
            get
            {
                return relationChildColumns;
            }
        }

        /// <summary>
        /// Determines if the <see cref="RelationChildColumns"/> collection was manually modified.
        /// </summary>
        /// <returns>True if it was modified.</returns>
        public bool ShouldSerializeRelationChildColumns()
        {
            return RelationChildColumns.IsModified;
        }

        /// <summary>
        /// Resets the <see cref="RelationChildColumns"/> collection.
        /// </summary>
        public void ResetRelationChildColumns()
        {
            RelationChildColumns.Reset();
        }

        #endregion
        #region Collection Changed Handlers
        private void recordFilters_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("RecordFilters", e));
        }

        private void summaryDescriptors_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Summaries", e));
        }

        /*UFD:private void unfilteredSummaryDescriptors_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("UnfilteredSummarys", e));
        }*/

        private void fields_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            ResetSortInfoCache();
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Fields", e));
        }

        private void unboundFields_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            ResetSortInfoCache();
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("UnboundFields", e));
        }

        private void groupedColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            ResetSortInfoCache();
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("GroupedColumns", e));
        }

        private void primaryKeyColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            ResetSortInfoCache();
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("PrimaryKeyColumns", e));
        }

        private void relationChildColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            ResetSortInfoCache();
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("RelationChildColumns", e));
        }

        private void sortedColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            ResetSortInfoCache();
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("SortedColumns", e));
        }

        private void relations_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Relations", e));
            switch (e.Action)
            {
                case ListPropertyChangedType.Add:
                case ListPropertyChangedType.Insert:
                case ListPropertyChangedType.Move:
                case ListPropertyChangedType.Refresh:
                case ListPropertyChangedType.ItemChanged:
                    {
                        if (!this.Fields.IsModified)
                        {
                            this.Fields.Reset();
                        }

                        break;
                    }

                case ListPropertyChangedType.ItemPropertyChanged:
                    {
                        if (e.Property == "ChildTableDescriptor")
                        {
                            DescriptorPropertyChangedEventArgs inner = (DescriptorPropertyChangedEventArgs)e.Inner;
                            if (inner != null)
                            {
                                TableDescriptor td = ((RelationDescriptor)e.Item).ChildTableDescriptor;
                                inner = inner.GetNestedChildTableDescriptorEvent(ref td);
                                e = null;
                                if (inner != null)
                                {
                                    if (inner.PropertyName == "Relations")
                                    {
                                        e = inner.Inner as ListPropertyChangedEventArgs;
                                    }
                                    else if (inner.PropertyName == "Fields")
                                    {
                                        // if nested fields have changed, we need to rebuild this tabledescriptors field collection.
                                        e = inner.Inner as ListPropertyChangedEventArgs;
                                        if (e.Property == "Name" || e.Property == "Hide" || e.Action != ListPropertyChangedType.ItemPropertyChanged)
                                        {
                                            goto case ListPropertyChangedType.Refresh;
                                        }
                                    }

                                    if (e == null)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        if (e.Property == "MappingName" || e.Property == "Name" || e.Property == "ChildTableName" || e.Property == "RelationKind")
                        {
                            goto case ListPropertyChangedType.Refresh;
                        }

                        break;
                    }
            }
            ////            if (e.Action != ListPropertyChangedType.ItemPropertyChanged)
            //            {
            //                if (Engine != null && Engine.ShouldSerializeTable())
            //                {
            //                    Engine.Table.TableDirty = true;
            //                    Engine.Table.SummariesDirty = true;
            //                }
            //            }
        }

        private void expressionFields_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            _arrayOfColumnDescriptors = null;
            _arrayOfPKColumnDescriptors = null;
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ExpressionFields", e));
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public virtual void InitializePropertyDescriptors()
        {
        }

        #endregion
        #region Collection Changing Handlers
        private void recordFilters_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("RecordFilters", e));
        }

        private void summaryDescriptors_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Summaries", e));
        }

        /*UFD:private void unfilteredSummaryDescriptors_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("UnfilteredSummaries", e));
        }*/

        private void fields_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Fields", e));
        }

        private void groupedColumns_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("GroupedColumns", e));
        }

        private void primaryKeyColumns_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("PrimaryKeyColumns", e));
        }

        private void relationChildColumns_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("RelationChildColumns", e));
        }

        private void sortedColumns_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("SortedColumns", e));
        }

        private void relations_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Relations", e));
        }

        private void expressionFields_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ExpressionFields", e));
        }

        private void unboundFields_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("UnboundFields", e));
        }

        /// <override/>
        protected override void OnNameChanged(EventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
            base.OnNameChanged(e);
        }

        /// <override/>
        protected override void OnNameChanging(EventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
            base.OnNameChanging(e);
        }

        #endregion
        #region Callbacks

        /// <summary>
        /// Populates TableDescriptor collections with new data   
        /// </summary>   
        public void EnableOneTimePopulate()
        {
            if (Engine == null)
            {
                return;
            }

            // Collections check repeatedly whether their version still matches
            // the engine version and the version of base collections (e.g. VisibleColumns checks Columns)
            // This check is really only needed after we know that the schema was changed,
            // the following code will force the check being done only once again.
            this.Fields.EnableOneTimePopulate();
            this.Relations.EnableOneTimePopulate();
            this.SortedColumns.EnableOneTimePopulate();
            this.GroupedColumns.EnableOneTimePopulate();
            this.RelationChildColumns.EnableOneTimePopulate();
            this.PrimaryKeyColumns.EnableOneTimePopulate();

            OnEnableOneTimePopulate();
        }

        /// <exclude/>
        protected virtual void OnEnableOneTimePopulate()
        {
            ////in GridTableDescriptor:
            ////this.TableDescriptor.Columns.EnableOneTimePopulate();
            ////this.TableDescriptor.VisibleColumns.EnableOneTimePopulate();
        }

        ////int summaries_savedFieldsVersion = -1;

        /// <summary>
        /// Repeatedly called from SummaryDescriptorCollection. Lets you
        /// recreate the Summaries collection if fields or columns have changed
        /// and you have internal summaries (e.g. GridGroupingControl adds a summary
        /// for each column to determine maximum length of a column).
        /// </summary>
        public virtual void EnsureSummaryDescriptors()
        {
            /*if (summaries_savedFieldsVersion == this.Fields.Version)
                return;

            SummaryDescriptorCollection summaryDescriptors = new SummaryDescriptorCollection();

            // Distinct vector summaries for ForeignKeyKeyWords
            summaryDescriptors.InsideCollectionEditor = true;
            if (this.ParentRelation != null && this.ParentRelation.RelationKind == RelationKind.ForeignKeyKeyWords)    // ForeignListItems
            {
                foreach (FieldDescriptor field in this.Fields)
                {
                    if (!(field.GetPropertyType() == typeof(byte[])))
                    {
                        SummaryDescriptor sd = new SummaryDescriptor(field.Name, field.Name,
                            new CreateSummaryDelegate(DistinctCountSummary.CreateSummaryMethod));
                        summaryDescriptors.Add(sd);
#if DEBUG
                        if (Switches.AutoPopulate.TraceVerbose)
                            TraceUtil.TraceCurrentMethodInfo("Add", sd);
#else
                        ;
#endif
                    }
                }
            }

            summaryDescriptors.InsideCollectionEditor = false;

            Summaries.InitializeFrom(summaryDescriptors);

            summaries_savedFieldsVersion = this.Fields.Version;*/
        }

        /// <summary>
        /// Called to get the number of rows that should be added to each
        /// record. These are the rows that are visible when a record is
        /// expanded.
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public virtual int RowsPerRecord
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Called to get the number of preview rows that should be added to each
        /// record. Preview rows are the rows that are visible when a record is
        /// collapsed.
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public virtual int PreviewRowsPerRecord
        {
            get
            {
                return 0;
            }
        }

        #endregion
        #region Sort and Categorize
        internal IGroupByCategorizer GroupByCategorizer
        {
            get
            {
                return _groupByCategorizer == null ? defaultCategorizer : _groupByCategorizer;
            }

            set
            {
                _groupByCategorizer = value;
            }
        }

        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal IComparer Comparer
        {
            get
            {
                // TODO: before I make this public I have to make sure all RecordsDetails get updated also
                // since they are initialized with a reference to Table.Comparer
                // I might also have to differentiate between PrimaryKey comparer and
                // SortedRecords Comparer.
                return _comparer == null ? defaultComparer : _comparer;
            }

            set
            {
                _comparer = value;
            }
        }

        /// <summary>
        /// Determines if table is sorted. Returns True if any SortedColumns or
        /// GroupedColumns are specified or if the table is a child table in
        /// a relation and has relation child columns.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsSorted
        {
            get
            {
                return _comparer != null || RelationChildColumns.Count > 0 || SortedColumns.Count > 0 || GroupedColumns.Count > 0;
            }
        }

        /// <summary>
        /// Determines if table is grouped. Returns True if
        /// GroupedColumns are specified or if the table is a child table in
        /// a relation and has relation child columns.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsGrouped
        {
            get
            {
                return RelationChildColumns.Count > 0 || GroupedColumns.Count > 0;
            }
        }

        #endregion
        #region SourceListDescriptor Overrides
        // Overrides

        /// <override/>
        protected override void OnItemPropertiesChanged(EventArgs e)
        {
            if (this.Disposing || inOnInitializeItemProperties)
            {
                return;
            }

            ResetSortInfoCache();
            base.OnItemPropertiesChanged(e);
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ItemProperties"));
        }

        /// <override/>
        protected override void OnItemPropertiesChanging(EventArgs e)
        {
            if (this.Disposing || inOnInitializeItemProperties)
            {
                return;
            }

            ResetSortInfoCache();
            base.OnItemPropertiesChanging(e);
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ItemProperties"));
        }

        internal bool inOnInitializeItemProperties = false;

        /// <summary>
        /// Returns True when SetItemProperties is called from within OnInitializeItemProperties. Once SetItemProperties
        /// returns, the property will be reset to False.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InOnInitializeItemProperties
        {
            get
            {
                return inOnInitializeItemProperties;
            }
        }

        /// <override/>
        protected override void OnInitializeItemProperties(EventArgs e)
        {
            if (engine != null)
            {
                inOnInitializeItemProperties = true;
                if (this.parentRelation != null)
                {
                    if (parentRelation.RelationKind == RelationKind.UniformChildList)
                    {
                        parentRelation.EnsureInitialized();
                        SetItemProperties(parentRelation.itemProperties);
                    }
                    else if (parentRelation.ChildTableName != string.Empty)
                    {
                        SourceListSetEntry sourceListEntry = engine.SourceListSet[parentRelation.ChildTableName];
                        if (sourceListEntry != null)
                        {
                            SetItemProperties(sourceListEntry.List);
                        }
                    }
                }
                else
                {
                    SetItemProperties(engine.GetSourceList());
                }

                inOnInitializeItemProperties = false;
            }

            base.OnInitializeItemProperties(e);
        }

        #endregion
        #region Factory Methods
        /// <summary>
        /// Creates a <see cref="RelationDescriptor"/> object.
        /// </summary>
        /// <returns>The new relation descriptor.</returns>
        public RelationDescriptor CreateRelationDescriptor()
        {
            return Engine.CreateRelationDescriptor();
        }

        /// <summary>
        /// Creates a <see cref="ChildTable"/> object and initializes it.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <param name="hasRecords">Specifies if group will be filled with records or nested groups.</param>
        /// <param name="table">The table.</param>     
        /// <param name="fields">The sortColumns that define the category of the group.</param>
        /// <returns>The new child table..</returns>
        public ChildTable CreateChildTable(Element parent, bool hasRecords, Table table, SortColumnDescriptorCollection fields)
        {
            ChildTable childTable = Engine.CreateChildTable(parent);
            bool isExpanded = ParentRelation != null;
            childTable.InitializeDetails(hasRecords, fields, isExpanded);
            InitGroup(childTable, parent as Section, hasRecords, fields);
#if DEBUG
            if (Switches.AutoPopulate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(childTable, hasRecords, fields);
            }
#else
            ;
#endif
            return childTable;
        }

        /// <summary>
        /// Creates a <see cref="Group"/> object and initializes it.
        /// </summary>
        /// <param name="parentSection">The parent section.</param>        
        /// <param name="hasRecords">Specifies if group will be filled with records or nested groups.</param>
        /// <param name="table">The table.</param>
        /// <param name="cd">The sortColumn that defines the category of the group.</param>
        /// <returns>The new group.</returns>
        public Group CreateGroup(Section parentSection, bool hasRecords, Table table, SortColumnDescriptor cd)
        {
            SortColumnDescriptorCollection fields = new SortColumnDescriptorCollection(new SortColumnDescriptor[] { cd });
            return CreateGroup(parentSection, hasRecords, table, fields);
        }

        /// <summary>
        /// Creates a <see cref="Group"/> object and initializes it.
        /// </summary>
        /// <param name="parentSection">The parent section.</param>       
        /// <param name="hasRecords">Specifies if group will be filled with records or nested groups.</param>
        /// <param name="table">The table.</param>
        /// <param name="fields">The sortColumns that define the category of the group.</param>
        /// <returns>The new group.</returns>
        public Group CreateGroup(Section parentSection, bool hasRecords, Table table, SortColumnDescriptorCollection fields)
        {
            Group group = Engine.CreateGroup(parentSection);
            bool isExpanded = ParentRelation != null;
            group.InitializeDetails(hasRecords, fields, isExpanded);
            InitGroup(group, parentSection, hasRecords, fields);
            return group;
        }

        Group InitGroup(Group group, Section parentSection, bool hasRecords, SortColumnDescriptorCollection fields)
        {
            return group;
        }

        /// <summary>
        /// Creates a <see cref="SortColumnDescriptorCollection"/> collection with one <see cref="SortColumnDescriptor"/> entry.
        /// </summary>
        /// <param name="cd">The sort column.</param>
        /// <returns>A new SortColumnDescriptorCollection.</returns>
        public SortColumnDescriptorCollection CreateSortColumnDescriptorCollection(SortColumnDescriptor cd)
        {
            return new SortColumnDescriptorCollection(new SortColumnDescriptor[] { cd });
        }

        /// <summary>
        /// Creates a new <see cref="Section"/> for a caption area within a group.
        /// </summary>
        /// <param name="group">The Group.</param>
        /// <returns>A new section for caption area.</returns>
        public Section CreateCaptionSection(Group group)
        {
            Engine engine = Engine;
            Section s = engine.CreateCaptionSection(group);
            ////s.initializedVersion = engine.Version;
            return s;
        }

        /// <summary>
        /// Creates a new <see cref="Section"/> for a filter bar area within a group.
        /// </summary>
        /// <param name="group">The Group.</param>
        /// <returns>The new filter bar section.</returns>
        public Section CreateFilterBarSection(Group group)
        {
            Engine engine = Engine;
            Section s = engine.CreateFilterBarSection(group);
            ////s.initializedVersion = engine.Version;
            return s;
        }

        /// <summary>
        /// Creates a new <see cref="Section"/> for a column header area within a group.
        /// </summary>
        /// <param name="group">The Group.</param>
        /// <returns>The new column header.</returns>
        public Section CreateColumnHeaderSection(Group group)
        {
            Engine engine = Engine;
            Section s = engine.CreateColumnHeaderSection(group);
            ////s.initializedVersion = engine.Version;
            return s;
        }

        /// <summary>
        /// Creates a new <see cref="AddNewRecordSection"/> for an add new record area within a group.
        /// </summary>
        /// <param name="group">The Group.</param>
        /// <returns>A new AddNewRecordSection.</returns>
        public AddNewRecordSection CreateAddNewRecordSection(Group group)
        {
            AddNewRecordSection s = Engine.CreateAddNewRecordSection(group);
            s.InitializeComparer(SortColumnDescriptorCollection.Empty);
            ////s.initializedVersion = engine.Version;
            return s;
        }

        /// <summary>
        /// Creates a new <see cref="RecordsDetails"/> section for a records area within a group.
        /// </summary>
        /// <param name="group">The parent group.</param>
        /// <param name="fields">The sortColumns that define the category of the group.</param>
        /// <returns>The new element.</returns>
        public RecordsDetails CreateRecordsDetails(Group group, SortColumnDescriptorCollection fields)
        {
            Engine engine = Engine;
            RecordsDetails rd = engine.CreateRecordsDetails(group);
            rd.InitializeComparer(fields);
            ////rd.initializedVersion = engine.Version;
            return rd;
        }

        /// <summary>
        /// Creates a new <see cref="GroupsDetails"/> section for a groups area with nested groups within a group
        /// </summary>
        /// <param name="group">The parent group.</param>
        /// <param name="fields">The sortColumns that define the category of the group.</param>
        /// <returns>The new element.</returns>
        public GroupsDetails CreateGroupsDetails(Group group, SortColumnDescriptorCollection fields)
        {
            Engine engine = Engine;
            GroupsDetails gd = engine.CreateGroupsDetails(group);
            gd.InitializeComparer(fields);
            ////gd.initializedVersion = engine.Version;
            return gd;
        }

        /// <summary>
        /// Creates a new <see cref="Section"/> section for a summary area within a group.
        /// </summary>
        /// <param name="group">The Group.</param>
        /// <returns>The new summary section.</returns>
        public Section CreateSummarySection(Group group)
        {
            Engine engine = Engine;
            Section s = engine.CreateSummarySection(group);
            ////s.initializedVersion = engine.Version;
            return s;
        }

        /// <summary>
        /// Creates a new <see cref="Record"/> object.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>The new record.</returns>
        public Record CreateRecord(Table table)
        {
            Record r = Engine.CreateRecord(table);
            r.ParentElement = table;
            /////r.initializedVersion = engine.Version;
            return r;
        }

        /// <summary>
        /// Creates a new <see cref="Table"/> object for a related table.
        /// </summary>
        /// <param name="td">Table descriptor.</param>
        /// <param name="relationParentTable">Related parent table.</param>
        /// <returns>The new table.</returns>
        public Table CreateRelatedTable(TableDescriptor td, Table relationParentTable)
        {
            Table table = Engine.CreateTable(td, relationParentTable);
            table.TableEventsTarget = td;
            return table;
        }

        #endregion
        #region ITypedList Members

        PropertyDescriptorCollection properties = null;
        int propertiesVersion = -1;

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return GetItemProperties();
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return Name;
        }

        PropertyDescriptorCollection GetItemProperties()
        {
            if (properties == null || propertiesVersion != Fields.Version)
            {
                ArrayList pds = new ArrayList();
                pds.Add(new TableRecordDataPropertyDescriptor(typeof(object)));
                pds.Add(new TableRecordIndexPropertyDescriptor());
                foreach (FieldDescriptor field in this.Fields)
                {
                    pds.Add(new TableFieldPropertyDescriptor(field));
                }

                properties = new PropertyDescriptorCollection((PropertyDescriptor[])pds.ToArray(typeof(PropertyDescriptor)));
                propertiesVersion = Fields.Version;
            }

            return properties;
        }

        #endregion
        #region FieldValue events
        // event FieldValueEventHandler QueryValue

        /// <summary>
        /// Occurs when a value for a field descriptor and record is returned. See the Grid\Grouping\Samples\CustomSummary
        /// sample how to use this event with unbound field descriptors.
        /// </summary>
        [Description("Occurs when a value for a field descriptor and record is returned")] 
        public event FieldValueEventHandler QueryValue;

        /// <summary>
        /// Raises the <see cref="QueryValue"/> event
        /// </summary>
        /// <param name="e">The <see cref="FieldValueEventArgs"/> with event data.</param>
        protected virtual void OnQueryValue(FieldValueEventArgs e)
        {
            if (QueryValue != null)
            {
                QueryValue(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryValue"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FieldValueEventArgs"/> with event data.</param>
        public void RaiseQueryValue(FieldValueEventArgs e)
        {
            if (Engine != null)
            {
                Engine.RaiseQueryValue(e);
            }

            OnQueryValue(e);
        }

        // event FieldValueEventHandler SaveValue

        /// <summary>
        /// Occurs when a value for a field descriptor and record is saved. See the Grid\Grouping\Samples\CustomSummary
        /// sample how to use this event with unbound field descriptors.
        /// </summary>
        [Description("Occurs when a value for a field descriptor and record is saved")]
        public event FieldValueEventHandler SaveValue;

        /// <summary>
        /// Raises the <see cref="SaveValue"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FieldValueEventArgs"/> with event data.</param>
        protected virtual void OnSaveValue(FieldValueEventArgs e)
        {
            if (SaveValue != null)
            {
                SaveValue(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="SaveValue"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FieldValueEventArgs"/> with event data.</param>
        public void RaiseSaveValue(FieldValueEventArgs e)
        {
            if (Engine != null)
            {
                Engine.RaiseSaveValue(e);
            }

            OnSaveValue(e);
        }
        #endregion
        #region Helper Methods (IsChildOf, etc.)
        /// <summary>
        /// Determines if this is the ChildTableDescriptor of a parent relation and also if the
        /// RelationDescriptor.RelationKind of the parent relations is ForeignKeyReference,
        /// RelatedMasterDetails, or ForeignKeyKeyWords.
        /// </summary>
        /// <returns>True if this is child and the relation kind is ForeignKeyReference,
        /// RelatedMasterDetails, or ForeignKeyKeyWords. </returns>
        public bool IsForeignKeyRelationChildTableDescriptor()
        {
            return this.ParentRelation != null && (this.ParentRelation.RelationKind == RelationKind.ForeignKeyReference
                || this.ParentRelation.RelationKind == RelationKind.RelatedMasterDetails
                || this.ParentRelation.RelationKind == RelationKind.ForeignKeyKeyWords
                || (!Engine.UseOldUniformChildListRelation && this.ParentRelation.RelationKind == RelationKind.UniformChildList));
        }

        /// <summary>
        /// Determines if this is the ChildTableDescriptor of a specified parent TableDescriptor.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>True if this is child; False otherwise.</returns>
        public bool IsChildOf(TableDescriptor parent)
        {
            TableDescriptor td = this.ParentTableDescriptor;
            if (td == null)
            {
                return false;
            }

            if (td == parent)
            {
                return true;
            }

            return td.IsChildOf(parent);
        }

        /// <summary>
        /// Returns true if engine is used by a parent control in design-time.
        /// </summary>
        /// <returns>returns False.</returns>
        public virtual bool IsDesignTime()
        {
            return false;
        }
        #endregion

        #region ITableEventsTarget
        // tevent ExceptionRaised ExceptionRaisedEventArgs

        /// <summary>
        /// Occurs when an unknown exception has been cached while modifying underlying data in the data source.
        /// </summary>
        /// <remarks>
        /// If necessary, you can rethrow the exception in your event handler.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs when a unknown exception has been cached while modifying underlying data in the data source.")]
        public event ExceptionRaisedEventHandler ExceptionRaised;
        
        /// <summary>
        /// Raises the <see cref="ExceptionRaised"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ExceptionRaisedEventArgs" /> that contains the event data.</param>
        protected virtual void OnExceptionRaised(ExceptionRaisedEventArgs e)
        {
            if (ExceptionRaised != null)
            {
                ExceptionRaised(this, e);
            }
        }

        void ITableEventsTarget.OnExceptionRaised(ExceptionRaisedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnExceptionRaised(e);
            }

            OnExceptionRaised(e);
        }

        // tevent GroupCollapsing GroupEventArgs

        /// <summary>
        /// Occurs before a group is collapsed.
        /// </summary>
        [Description("Occurs before a group is collapsed.")]
        [Category("Table")]
        public event GroupEventHandler GroupCollapsing;

        /// <summary>
        /// Raises the  <see cref="GroupCollapsing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupCollapsing(GroupEventArgs e)
        {
            if (GroupCollapsing != null)
            {
                GroupCollapsing(this, e);
            }
        }

        void ITableEventsTarget.OnGroupCollapsing(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupCollapsing(e);
            }

            OnGroupCollapsing(e);
        }

        // tevent GroupCollapsed GroupEventArgs

        /// <summary>
        /// Occurs before a group is collapsed.
        /// </summary>
        [Description("Occurs before a group is collapsed.")]
        [Category("Table")]
        public event GroupEventHandler GroupCollapsed;

        /// <summary>
        /// Raises the  <see cref="GroupCollapsed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupCollapsed(GroupEventArgs e)
        {
            if (GroupCollapsed != null)
            {
                GroupCollapsed(this, e);
            }
        }

        void ITableEventsTarget.OnGroupCollapsed(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupCollapsed(e);
            }

            OnGroupCollapsed(e);
        }

        // tevent GroupExpanding GroupEventArgs

        /// <summary>
        /// Occurs before a group is expanded.
        /// </summary>
        [Description("Occurs before a group is expanded.")]
        [Category("Table")]
        public event GroupEventHandler GroupExpanding;

        /// <summary>
        /// Raises the  <see cref="GroupExpanding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupExpanding(GroupEventArgs e)
        {
            if (GroupExpanding != null)
            {
                GroupExpanding(this, e);
            }
        }

        void ITableEventsTarget.OnGroupExpanding(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupExpanding(e);
            }

            OnGroupExpanding(e);
        }

        // tevent GroupExpanded GroupEventArgs

        /// <summary>
        /// Occurs after a group is expanded.
        /// </summary>
        [Description("Occurs after a group was expanded.")]
        [Category("Table")]
        public event GroupEventHandler GroupExpanded;

        /// <summary>
        /// Raises the  <see cref="GroupExpanded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupExpanded(GroupEventArgs e)
        {
            if (GroupExpanded != null)
            {
                GroupExpanded(this, e);
            }
        }

        void ITableEventsTarget.OnGroupExpanded(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupExpanded(e);
            }

            OnGroupExpanded(e);
        }

        // tevent RecordCollapsing RecordEventArgs

        /// <summary>
        /// Occurs before a record with nested tables is collapsed.
        /// </summary>
        [Description("Occurs before a record with nested tables is collapsed.")]
        [Category("Table")]
        public event RecordEventHandler RecordCollapsing;

        /// <summary>
        /// Raises the  <see cref="RecordCollapsing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordCollapsing(RecordEventArgs e)
        {
            if (RecordCollapsing != null)
            {
                RecordCollapsing(this, e);
            }
        }

        void ITableEventsTarget.OnRecordCollapsing(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordCollapsing(e);
            }

            OnRecordCollapsing(e);
        }

        // tevent RecordCollapsed RecordEventArgs

        /// <summary>
        /// Occurs after a record with nested tables is collapsed.
        /// </summary>
        [Description("Occurs after a record with nested tables is collapsed.")]
        [Category("Table")]
        public event RecordEventHandler RecordCollapsed;

        /// <summary>
        /// Raises the <see cref="RecordCollapsed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordCollapsed(RecordEventArgs e)
        {
            if (RecordCollapsed != null)
            {
                RecordCollapsed(this, e);
            }
        }

        void ITableEventsTarget.OnRecordCollapsed(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordCollapsed(e);
            }

            OnRecordCollapsed(e);
        }

        // tevent RecordExpanding RecordEventArgs

        /// <summary>
        /// Occurs before a record with nested tables is expanded.
        /// </summary>
        [Description("Occurs before a record with nested tables is expanded.")]
        [Category("Table")]
        public event RecordEventHandler RecordExpanding;

        /// <summary>
        /// Raises the <see cref="RecordExpanding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordExpanding(RecordEventArgs e)
        {
            if (RecordExpanding != null)
            {
                RecordExpanding(this, e);
            }
        }

        void ITableEventsTarget.OnRecordExpanding(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordExpanding(e);
            }

            OnRecordExpanding(e);
        }

        // tevent RecordExpanded RecordEventArgs

        /// <summary>
        /// Occurs after a record with nested tables is expanded.
        /// </summary>
        [Description("Occurs after a record with nested tables is expanded.")]
        [Category("Table")]
        public event RecordEventHandler RecordExpanded;

        /// <summary>
        /// Raises the <see cref="RecordExpanded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordExpanded(RecordEventArgs e)
        {
            if (RecordExpanded != null)
            {
                RecordExpanded(this, e);
            }
        }

        void ITableEventsTarget.OnRecordExpanded(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordExpanded(e);
            }

            OnRecordExpanded(e);
        }

        // tevent RecordDeleting RecordEventArgs

        /// <summary>
        /// Occurs before a record is deleted.
        /// </summary>
        /// <remarks>
        /// This event is raised only when the <see cref="Table"/> or <see cref="Record"/> triggers the deletion. If
        /// the underlying source list deletes the record, a <see cref="Table.SourceListListChanged"/> event is raised instead.
        /// </remarks>
        [Description("Occurs before a record is deleted.")]
        [Category("Table")]
        public event RecordEventHandler RecordDeleting;

        /// <summary>
        /// Raises the <see cref="RecordDeleting"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordDeleting(RecordEventArgs e)
        {
            if (RecordDeleting != null)
            {
                RecordDeleting(this, e);
            }
        }

        void ITableEventsTarget.OnRecordDeleting(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordDeleting(e);
            }

            OnRecordDeleting(e);
        }

        // tevent RecordDeleted RecordEventArgs

        /// <summary>
        /// Occurs after a record is deleted.
        /// </summary>
        /// <remarks>
        /// This event is raised only when the <see cref="Table"/> or <see cref="Record"/> triggers the deletion. If
        /// the underlying source list deletes the record, a <see cref="Table.SourceListListChanged"/> event is raised instead.
        /// </remarks>
        [Description("Occurs after a record is deleted.")]
        [Category("Table")]
        public event RecordEventHandler RecordDeleted;

        /// <summary>
        /// Raises the <see cref="RecordDeleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordDeleted(RecordEventArgs e)
        {
            if (RecordDeleted != null)
            {
                RecordDeleted(this, e);
            }
        }

        void ITableEventsTarget.OnRecordDeleted(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordDeleted(e);
            }

            OnRecordDeleted(e);
        }

        // tevent CurrentRecordContextChange CurrentRecordContextChangeEventArgs

        /// <summary>
        /// Occurs before and after the status of the current record is changed. Check the <see cref="CurrentRecordContextChangeEventArgs.Action"/>
        /// of the <see cref="CurrentRecordContextChangeEventArgs"/> to get information which current record state was changed.
        /// </summary>
        [Description("Occurs before and after the status of the current record is changed.")]
        [Category("Table")]
        public event CurrentRecordContextChangeEventHandler CurrentRecordContextChange;

        /// <summary>
        /// Raises the  <see cref="CurrentRecordContextChange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CurrentRecordContextChangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentRecordContextChange(CurrentRecordContextChangeEventArgs e)
        {
            if (CurrentRecordContextChange != null)
            {
                CurrentRecordContextChange(this, e);
            }
        }

        void ITableEventsTarget.OnCurrentRecordContextChange(CurrentRecordContextChangeEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCurrentRecordContextChange(e);
            }

            OnCurrentRecordContextChange(e);
        }

        // tevent CurrentRecordManagerReset tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="CurrentRecordManager.Reset"/> method of the <see cref="CurrentRecordManager"/> was called.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this event and resets any "Current Cell" state when this
        /// event is raised.
        /// </remarks>
        [Description("Occurs when the CurrentRecordManager.Reset method of the CurrentRecordManager is called.")]
        [Category("Table")]
        public event TableEventHandler CurrentRecordManagerReset;

        /// <summary>
        /// Raises the  <see cref="CurrentRecordManagerReset"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentRecordManagerReset(TableEventArgs e)
        {
            if (CurrentRecordManagerReset != null)
            {
                CurrentRecordManagerReset(this, e);
            }
        }

        void ITableEventsTarget.OnCurrentRecordManagerReset(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCurrentRecordManagerReset(e);
            }

            OnCurrentRecordManagerReset(e);
        }

        // tevent GroupSummaryInvalidated GroupEventArgs

        /// <summary>
        /// Occurs when a summary has been marked dirty.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this event and will force a repaint of the specified summary if it is visible
        /// when this event is raised.
        /// </remarks>
        [Description("Occurs when a summary has been marked dirty.")]
        [Category("Table")]
        public event GroupEventHandler GroupSummaryInvalidated;

        /// <summary>
        /// Raises the <see cref="GroupSummaryInvalidated"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupSummaryInvalidated(GroupEventArgs e)
        {
            if (GroupSummaryInvalidated != null)
            {
                GroupSummaryInvalidated(this, e);
            }
        }

        void ITableEventsTarget.OnGroupSummaryInvalidated(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupSummaryInvalidated(e);
            }

            OnGroupSummaryInvalidated(e);
        }

        // tevent SourceListListChanged TableListChangedEventArgs

        /// <summary>
        /// Occurs before the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list. More detailed <see cref="SourceListRecordChanged"/> events will be
        /// raised after this event.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event before the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs before the Table processes the IBindingList.ListChanged event.")]
        [Category("Table")]
        public event TableListChangedEventHandler SourceListListChanged;

        /// <summary>
        /// Raises the <see cref="SourceListListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableListChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListListChanged(TableListChangedEventArgs e)
        {
            if (SourceListListChanged != null)
            {
                SourceListListChanged(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListListChanged(TableListChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListListChanged(e);
            }

            OnSourceListListChanged(e);
        }

        // tevent SourceListListChangedCompleted TableListChangedEventArgs

        /// <summary>
        /// Occurs after the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer the chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event right after the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs after the Table processes the IBindingList.ListChanged event.")]
        [Category("Table")]
        public event TableListChangedEventHandler SourceListListChangedCompleted;

        /// <summary>
        /// Raises the <see cref="SourceListListChangedCompleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableListChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListListChangedCompleted(TableListChangedEventArgs e)
        {
            if (SourceListListChangedCompleted != null)
            {
                SourceListListChangedCompleted(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListListChangedCompleted(TableListChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListListChangedCompleted(e);
            }

            OnSourceListListChangedCompleted(e);
        }

        // tevent SourceListRecordChanged RecordChangedEventArgs

        /// <summary>
        /// Occurs when a record in the underlying datasource was added, removed, or changed and after
        /// the <see cref="Table"/> was updated with that change.
        /// </summary>
        [Description("Occurs when a record in the underlying data source was added, removed, or changed and the table was updated.")]
        [Category("Table")]
        public event RecordChangedEventHandler SourceListRecordChanged;

        /// <summary>
        /// Raises the <see cref="SourceListRecordChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListRecordChanged(RecordChangedEventArgs e)
        {
            if (SourceListRecordChanged != null)
            {
                SourceListRecordChanged(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListRecordChanged(RecordChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListRecordChanged(e);
            }

            OnSourceListRecordChanged(e);
        }

        // tevent SourceListRecordChanging RecordChangedEventArgs

        /// <summary>
        /// Occurs when a record in the underlying datasource was added, removed, or changed and before
        /// the <see cref="Table"/> is updated with that change.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when a record in the underlying datasource was added, removed, or changed and before the Table is updated with that change")]
        public event RecordChangedEventHandler SourceListRecordChanging;

        /// <summary>
        /// Raises the <see cref="SourceListRecordChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListRecordChanging(RecordChangedEventArgs e)
        {
            if (SourceListRecordChanging != null)
            {
                SourceListRecordChanging(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListRecordChanging(RecordChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListRecordChanging(e);
            }

            OnSourceListRecordChanging(e);
        }

        /// <exclude/>
        [Obsolete("Use GridGroupingControl.InvalidateAllWhenListChanged instead")]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool InvalidateAllWhenListChanged
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        // tevent GroupAdded GroupEventArgs

        /// <summary>
        /// Occurs when a new group is added in a table after the table was categorized and when a record was changed. The event does not
        /// occur during categorization of the table. See the <see cref="CategorizedRecords"/> elements to when categorization
        /// finished.
        /// </summary>
        [Description("Occurs when a new group is added in a table after the table was categorized and when a record was changed.")]
        [Category("Table")]
        public event GroupEventHandler GroupAdded;

        /// <summary>
        /// Raises the  <see cref="GroupAdded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupAdded(GroupEventArgs e)
        {
            if (GroupAdded != null)
            {
                GroupAdded(this, e);
            }
        }

        void ITableEventsTarget.OnGroupAdded(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupAdded(e);
            }

            OnGroupAdded(e);
        }

        // tevent GroupRemoving GroupEventArgs

        /// <summary>
        /// Occurs when a group was removed from a table after the table was categorized and when a record was changed. The event does not
        /// occur during categorization of the table. See the <see cref="CategorizedRecords"/> elements to when categorization
        /// finished.
        /// </summary>
        [Description("Occurs when a group was removed from a table after the table was categorized and when a record was changed.")]
        [Category("Table")]
        public event GroupEventHandler GroupRemoving;

        /// <summary>
        /// Raises the <see cref="GroupRemoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupRemoving(GroupEventArgs e)
        {
            if (GroupRemoving != null)
            {
                GroupRemoving(this, e);
            }
        }

        void ITableEventsTarget.OnGroupRemoving(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupRemoving(e);
            }

            OnGroupRemoving(e);
        }

        // tevent SortingItemsInGroup GroupEventArgs

        /// <summary>
        /// Occurs before the records for a group are sorted.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of city. It is not necessary to sort the whole table. Instead
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup events are fired in such case when a specific group is sorted on demand.
        /// <para/>
        /// If the whole table was set dirty (see <see cref="Syncfusion.Grouping.Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case, only a CategorizedElements event is raised but no
        /// SortingItemsInGroup event.
        /// </remarks>
        [Description("Occurs before the records for a group are sorted.")]
        [Category("Table")]
        public event GroupEventHandler SortingItemsInGroup;

        /// <summary>
        /// Raises the <see cref="SortingItemsInGroup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnSortingItemsInGroup(GroupEventArgs e)
        {
            if (SortingItemsInGroup != null)
            {
                SortingItemsInGroup(this, e);
            }
        }

        void ITableEventsTarget.OnSortingItemsInGroup(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSortingItemsInGroup(e);
            }

            OnSortingItemsInGroup(e);
        }

        // tevent SortedItemsInGroup GroupEventArgs

        /// <summary>
        /// Occurs after the records for a group were sorted.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// then you change the sort order of city, it is not necessary to sort the whole table. Instead,
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup events are fired in such cases when a specific group was sorted on demand.
        /// <para/>
        /// If the whole table was set dirty (see <see cref="Syncfusion.Grouping.Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case, only a CategorizedElements event is raised but no
        /// SortedItemsInGroup event.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs after the records for a group were sorted.")]
        public event GroupEventHandler SortedItemsInGroup;

        /// <summary>
        /// Raises the  <see cref="SortedItemsInGroup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnSortedItemsInGroup(GroupEventArgs e)
        {
            if (SortedItemsInGroup != null)
            {
                SortedItemsInGroup(this, e);
            }
        }

        void ITableEventsTarget.OnSortedItemsInGroup(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSortedItemsInGroup(e);
            }

            OnSortedItemsInGroup(e);
        }

        // tevent InvalidatingCounters tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="Syncfusion.Grouping.Table.InvalidateCounterTopDown"/> of a <see cref="Table"/> is called
        /// and before all counters are marked dirty.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the Table.InvalidateCounterTopDown method of a table is called.")]
        public event TableEventHandler InvalidatingCounters;

        /// <summary>
        /// Raises the <see cref="InvalidatingCounters"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnInvalidatingCounters(TableEventArgs e)
        {
            if (InvalidatingCounters != null)
            {
                InvalidatingCounters(this, e);
            }
        }

        void ITableEventsTarget.OnInvalidatingCounters(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnInvalidatingCounters(e);
            }

            OnInvalidatingCounters(e);
        }

        // tevent InvalidatingSummaries tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="Syncfusion.Grouping.Table.InvalidateSummariesTopDown"/> of a <see cref="Table"/> is called
        /// and before all summaries in that table are marked dirty.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the Table.InvalidateSummariesTopDown of a table is called.")]
        public event TableEventHandler InvalidatingSummaries;

        /// <summary>
        /// Raises the <see cref="InvalidatingSummaries"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnInvalidatingSummaries(TableEventArgs e)
        {
            if (InvalidatingSummaries != null)
            {
                InvalidatingSummaries(this, e);
            }
        }

        void ITableEventsTarget.OnInvalidatingSummaries(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnInvalidatingSummaries(e);
            }

            OnInvalidatingSummaries(e);
        }

        // tevent CategorizingRecords tableEventsTarget

        /// <summary>
        /// Occurs before records are categorized after a table was marked dirty (<see cref="Syncfusion.Grouping.Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Syncfusion.Grouping.Table.TableDirty"/> is set True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Syncfusion.Grouping.Element.EnsureInitialized"/> of the <see cref="Table"/> will start
        /// categorization.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs before records are categorized after a table was marked dirty.")]
        public event TableEventHandler CategorizingRecords;

        /// <summary>
        /// Raises the <see cref="CategorizingRecords"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnCategorizingRecords(TableEventArgs e)
        {
            if (CategorizingRecords != null)
            {
                CategorizingRecords(this, e);
            }
        }

        void ITableEventsTarget.OnCategorizingRecords(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCategorizingRecords(e);
            }

            OnCategorizingRecords(e);
        }

        // tevent CategorizedRecords tableEventsTarget

        /// <summary>
        /// Occurs after records were categorized after a table was marked dirty (<see cref="Syncfusion.Grouping.Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Syncfusion.Grouping.Table.TableDirty"/> is set True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Element.EnsureInitialized"/> of the <see cref="Table"/>
        /// will start
        /// categorization.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs after records are categorized after a table is marked dirty.")]
        public event TableEventHandler CategorizedRecords;

        /// <summary>
        /// Raises the  <see cref="CategorizedRecords"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnCategorizedRecords(TableEventArgs e)
        {
            if (CategorizedRecords != null)
            {
                CategorizedRecords(this, e);
            }
        }

        void ITableEventsTarget.OnCategorizedRecords(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCategorizedRecords(e);
            }

            OnCategorizedRecords(e);
        }

        // tevent TableSourceListChanged Table

        /// <summary>
        /// Occurs after the data source is replaced.
        /// </summary>
        [Category("Table")]
        [Description("Occurs after the data source is replaced.")]
        public event TableEventHandler TableSourceListChanged;

        /// <summary>
        /// Raises the  <see cref="TableSourceListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableSourceListChanged(TableEventArgs e)
        {
            if (TableSourceListChanged != null)
            {
                TableSourceListChanged(this, e);
            }
        }

        void ITableEventsTarget.OnTableSourceListChanged(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnTableSourceListChanged(e);
            }

            OnTableSourceListChanged(e);
        }

        // tevent RecordValueChanging RecordValueChanging

        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and before Record.SetValue is called.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when a RecordFieldCell cell's value is changed and before Record.SetValue is called.")]
        public event RecordValueChangingEventHandler RecordValueChanging;

        /// <summary>
        /// Raises the  <see cref="RecordValueChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordValueChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordValueChanging(RecordValueChangingEventArgs e)
        {
            if (RecordValueChanging != null)
            {
                RecordValueChanging(this, e);
            }
        }

        void ITableEventsTarget.OnRecordValueChanging(RecordValueChangingEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordValueChanging(e);
            }

            OnRecordValueChanging(e);
        }

        // tevent RecordValueChanged RecordValueChanged

        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and after Record.SetValue returned.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when a RecordFieldCell cell's value is changed and after Record.SetValue returned")] 
        public event RecordValueChangedEventHandler RecordValueChanged;

        /// <summary>
        /// Raises the <see cref="RecordValueChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordValueChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordValueChanged(RecordValueChangedEventArgs e)
        {
            if (RecordValueChanged != null)
            {
                RecordValueChanged(this, e);
            }
        }

        void ITableEventsTarget.OnRecordValueChanged(RecordValueChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordValueChanged(e);
            }

            OnRecordValueChanged(e);
        }

        // tevent DisplayElementChanging DisplayElementChanging

        /// <summary>
        /// When number of visible elements is changed.
        /// </summary>
        [Description("Occurs when number of visible elements is being changed")]
        public event DisplayElementChangingEventHandler DisplayElementChanging;

        /// <summary>
        /// Raises the <see cref="DisplayElementChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DisplayElementChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnDisplayElementChanging(DisplayElementChangingEventArgs e)
        {
            if (DisplayElementChanging != null)
            {
                DisplayElementChanging(this, e);
            }
        }

        void ITableEventsTarget.OnDisplayElementChanging(DisplayElementChangingEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnDisplayElementChanging(e);
            }

            OnDisplayElementChanging(e);
        }

        // tevent DisplayElementChanged DisplayElementChanged

        /// <summary>
        /// When number of visible elements is changed.
        /// </summary>
        [Description("Occurs when number of visible elements is changed")]
        public event DisplayElementChangedEventHandler DisplayElementChanged;

        /// <summary>
        /// Raises the <see cref="DisplayElementChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DisplayElementChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnDisplayElementChanged(DisplayElementChangedEventArgs e)
        {
            if (DisplayElementChanged != null)
            {
                DisplayElementChanged(this, e);
            }
        }

        void ITableEventsTarget.OnDisplayElementChanged(DisplayElementChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnDisplayElementChanged(e);
            }

            OnDisplayElementChanged(e);
        }

        // tevent SelectedRecordsChanging SelectedRecordsChanging

        /// <summary>
        /// Occurs before the <see cref="Table.SelectedRecords"/> collection is modified.
        /// </summary>
        [Description("Occurs before the SelectedRecords collection is modified")]
        public event SelectedRecordsChangedEventHandler SelectedRecordsChanging;

        /// <summary>
        /// Raises the <see cref="SelectedRecordsChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="SelectedRecordsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectedRecordsChanging(SelectedRecordsChangedEventArgs e)
        {
            if (SelectedRecordsChanging != null)
            {
                SelectedRecordsChanging(this, e);
            }
        }

        void ITableEventsTarget.OnSelectedRecordsChanging(SelectedRecordsChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSelectedRecordsChanging(e);
            }

            OnSelectedRecordsChanging(e);
        }

        // tevent SelectedRecordsChanged SelectedRecordsChanged

        /// <summary>
        /// Occurs after the <see cref="Table.SelectedRecords"/> collection was modified.
        /// </summary>
        [Description("Occurs after the SelectedRecords collection is modified")]
        public event SelectedRecordsChangedEventHandler SelectedRecordsChanged;

        /// <summary>
        /// Raises the <see cref="SelectedRecordsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="SelectedRecordsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectedRecordsChanged(SelectedRecordsChangedEventArgs e)
        {
            if (SelectedRecordsChanged != null)
            {
                SelectedRecordsChanged(this, e);
            }
        }

        void ITableEventsTarget.OnSelectedRecordsChanged(SelectedRecordsChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSelectedRecordsChanged(e);
            }

            OnSelectedRecordsChanged(e);
        }

        ITableEventsTarget tableEventsTarget;

        /// <summary>
        /// Gets / sets an object that handles events raised by the <see cref="Table"/> object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ITableEventsTarget ForwardTableEvents
        {
            get
            {
                return this.tableEventsTarget;
            }

            set
            {
                this.tableEventsTarget = value;
            }
        }
        #endregion
    }
}